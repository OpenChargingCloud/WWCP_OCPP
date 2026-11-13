/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The back ends this station is dialled to, peer by peer: one endpoint of
    /// each at a time, the next one when that one cannot be reached, and what
    /// is written down taken as it changes (work packages P7 and P8 of EVQI
    /// Transport, sections 11.3.2 and 11.7.1).
    /// </summary>
    /// <remarks>
    /// A peer that dials by itself is dialled at the first of its endpoints
    /// that does; the others wait in reserve. Its attempts are counted, and
    /// after <see cref="NetworkProfileConnectionAttempts"/> of them that failed
    /// - or at once where the answer is final, a password refused, an address
    /// that leads nowhere - that endpoint is hung up and the next one dialled,
    /// round and round, with a pause after every round that reached none. A
    /// peer of one endpoint goes on trying it as before.
    ///
    /// A change of what is written down is taken at once: a peer that dials by
    /// itself now is dialled, one that does not any more, or an endpoint taken
    /// away, is hung up, and one changed in how it is dialled is dialled again.
    /// A description written anew or a host key learned changes nothing that
    /// is up.
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        /// <summary>
        /// What is dialled of each peer that dials by itself: one endpoint.
        /// </summary>
        protected sealed class LiveEndpoint
        {

            public required String           PeerId       { get; init; }

            /// <summary>
            /// The connection as it was when it was dialled: the store changes
            /// its entries where they are, and what is up is what was dialled.
            /// </summary>
            public required ConnectionEntry  Dialled      { get; init; }

            public required Int64            Revision     { get; init; }
            public required Boolean          Primary      { get; init; }
            public required Int32            Index        { get; init; }

            /// <summary>
            /// The endpoint the round of tries started at that this one belongs
            /// to: back at it, a round is over.
            /// </summary>
            public required Int32            RoundStart   { get; set; }

            /// <summary>
            /// The attempts made at it since it was last up, the first one
            /// included.
            /// </summary>
            public Int32                     Attempts     { get; set; }

            public Object?                   Client       { get; set; }

            public Boolean                   LetGo        { get; set; }

            public CancellationTokenSource   Pause        { get; } = new ();

        }

        protected readonly Dictionary<String, LiveEndpoint>  livePeers  = [];
        protected readonly SemaphoreSlim                      liveLock   = new (1, 1);

        /// <summary>
        /// Whether the peers were dialled since this station started or last
        /// hung up: a change written down before is left to that dialling,
        /// which takes it anyway.
        /// </summary>
        protected Boolean                                     peersDialled;

        /// <summary>
        /// How many attempts at an endpoint fail before the next endpoint of
        /// its peer is dialled: OCPPCommCtrlr.NetworkProfileConnectionAttempts,
        /// fixed at 3 until it is read and written for real (P14).
        /// </summary>
        public Int32 NetworkProfileConnectionAttempts { get; set; } = 3;

        /// <summary>
        /// How long a peer waits after a round of all its endpoints reached
        /// none, before the first of them is dialled again.
        /// </summary>
        public TimeSpan FailoverPause { get; set; } = TimeSpan.FromSeconds(30);

        #endregion


        #region (private) DialPeers(CancellationToken)

        /// <summary>
        /// Dial every peer that dials by itself, at its first endpoint that
        /// does - each on its own, in the order they were first dialled, and
        /// none of them affected by how another went.
        /// </summary>
        protected async Task DialPeers(CancellationToken CancellationToken = default)
        {

            var connections  = Connections.Connections.ToDictionary(entry => entry.Id);
            var peers        = Connections.Peers.
                                   Where  (peer => peer.Role != PeerRole.Service).
                                   OrderBy(peer => FirstDialledAt(peer, connections)).
                                   ToArray();

            lock (dialled)
                dialled.Clear();

            await liveLock.WaitAsync(CancellationToken);

            try
            {

                lock (livePeers)
                {
                    foreach (var live in livePeers.Values)
                        live.Pause.Cancel();
                    livePeers.Clear();
                }

                peersDialled = true;

                if (peers.Length == 0)
                {
                    Log.Info(connections.Count == 0
                                 ? $"No connections are configured; this {NodeWording.Noun} connects nowhere."
                                 : $"None of the {connections.Count} configured connection(s) is set to connect by itself.",
                             "ocpp", "connections");
                    return;
                }

                foreach (var peer in peers)
                    await DialPeer(peer, StartOf(peer, connections), null, CancellationToken);

            }
            finally
            {
                liveLock.Release();
            }

        }

        #endregion

        #region (private) DialPeer(Peer, Index, RoundStart, CancellationToken, Left = null)

        /// <summary>
        /// Dial the given endpoint of a peer, and say of its others that they
        /// wait in reserve - but the one it goes on from, which was said of
        /// already. Called under the live lock.
        /// </summary>
        protected async Task DialPeer(PeerEntry          Peer,
                                    Int32              Index,
                                    Int32?             RoundStart,
                                    CancellationToken  CancellationToken,
                                    String?            Left   = null)
        {

            var connections = Connections.Connections.ToDictionary(entry => entry.Id);

            if (Index < 0 || Index >= Peer.EndpointIds.Count ||
                !connections.TryGetValue(Peer.EndpointIds[Index], out var entry))
                return;

            var dialledAs = entry.Copy();

            var live = new LiveEndpoint {
                           PeerId      = Peer.Id,
                           Dialled     = dialledAs,
                           Revision    = Connections.DialRevisionOf(entry.Id),
                           Primary     = IsPrimary(dialledAs),
                           Index       = Index,
                           RoundStart  = RoundStart ?? Index,
                           Attempts    = 1
                       };

            lock (livePeers)
                livePeers[Peer.Id] = live;

            // The endpoint left behind was said of already, and why.
            foreach (var other in Peer.EndpointIds.Where(id => id != entry.Id && id != Left))
                if (connections.TryGetValue(other, out var reserve) && reserve.AutoConnect)
                    Record(reserve, ConnectionStatus.Standby,
                           $"In reserve: '{entry.Description}' is dialled for '{Peer.Description}', and this endpoint " +
                           "when that one cannot be reached.");

            await Dial(dialledAs, CancellationToken);

        }

        #endregion

        #region (private) RememberClient(Connection, Client)

        /// <summary>
        /// The client that was made for the given connection when it was
        /// dialled: what is hung up when its peer goes on to another endpoint,
        /// or when it changes.
        /// </summary>
        protected void RememberClient(ConnectionEntry Connection, Object? Client)
        {
            if (Client is not null && LiveOf(Connection) is LiveEndpoint live)
                live.Client = Client;
        }

        #endregion

        #region (private) EndpointStatus(Connection, Status) / EndpointRetries(Connection)

        /// <summary>
        /// What became of an attempt at the endpoint a peer is dialled at: one
        /// that got through starts the count anew, one that cannot be tried
        /// again goes on to the next endpoint, and so does one the client tries
        /// again where that would be one too many.
        /// </summary>
        protected void EndpointStatus(ConnectionEntry Connection, ConnectionStatus Status)
        {

            if (LiveOf(Connection) is not LiveEndpoint live)
                return;

            switch (Status)
            {

                // Up, or up until a moment ago: what was tried before does not
                // count, and a round of tries starts here.
                case ConnectionStatus.Connected:
                case ConnectionStatus.Lost:
                    live.Attempts    = 0;
                    live.RoundStart  = live.Index;
                    break;

                case ConnectionStatus.Refused:
                case ConnectionStatus.Failed:
                case ConnectionStatus.NotDialled:
                    FailOver(live, "it cannot be tried again");
                    break;

                case ConnectionStatus.Trying:
                    if (live.Attempts >= NetworkProfileConnectionAttempts)
                        FailOver(live, $"{live.Attempts} attempt(s) failed");
                    break;

            }

        }

        /// <summary>
        /// The client is about to try the endpoint a peer is dialled at again.
        /// </summary>
        protected void EndpointRetries(ConnectionEntry Connection)
        {

            if (LiveOf(Connection) is not LiveEndpoint live)
                return;

            live.Attempts++;

            if (live.Attempts > NetworkProfileConnectionAttempts)
                FailOver(live, $"{live.Attempts - 1} attempt(s) failed");

        }

        protected LiveEndpoint? LiveOf(ConnectionEntry Connection)
        {
            lock (livePeers)
                return livePeers.Values.FirstOrDefault(live => !live.LetGo && live.Dialled.Id == Connection.Id);
        }

        #endregion

        #region (private) FailOver(Live, Why)

        /// <summary>
        /// Hang up the endpoint a peer is dialled at, and dial its next one -
        /// after a pause, where that one began the round. A peer of one
        /// endpoint goes on as it is.
        /// </summary>
        protected void FailOver(LiveEndpoint Live, String Why)
        {

            var peer = Connections.Peers.FirstOrDefault(peer => peer.Id == Live.PeerId);

            if (peer is null)
                return;

            var connections  = Connections.Connections.ToDictionary(entry => entry.Id);
            var next         = NextDialable(peer, Live.Index, connections);

            if (next is null || next == Live.Index)
                return;

            lock (livePeers)
            {
                if (Live.LetGo)
                    return;
                Live.LetGo = true;
            }

            // Out of the client's event, which this is called from, and after
            // whatever the live lock is held for now.
            _ = Task.Run(async () => {

                await liveLock.WaitAsync();

                try
                {

                    lock (livePeers)
                        if (!livePeers.TryGetValue(Live.PeerId, out var current) || current != Live)
                            return;

                    var nextName = connections.TryGetValue(peer.EndpointIds[next.Value], out var nextEntry)
                                       ? nextEntry.Description
                                       : peer.EndpointIds[next.Value];

                    Log.Notice($"'{Live.Dialled.Description}' of '{peer.Description}' could not be reached - {Why}; '{nextName}' is dialled instead.",
                               "ocpp", "connections");

                    await HangUpEndpoint(Live, $"Failing over: {Why}.");

                    Record(Live.Dialled, ConnectionStatus.Standby,
                           $"In reserve: it could not be reached ({Why}), so '{nextName}' is dialled for '{peer.Description}', " +
                           "and this endpoint again when that one cannot be reached either.");

                    if (next == Live.RoundStart)
                    {

                        Log.Notice($"None of the endpoints of '{peer.Description}' could be reached; they are tried again in " +
                                   $"{FailoverPause.TotalSeconds:0} s.", "ocpp", "connections");

                        liveLock.Release();

                        try
                        {
                            await Task.Delay(FailoverPause, Live.Pause.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            await liveLock.WaitAsync();
                            return;
                        }

                        await liveLock.WaitAsync();

                        lock (livePeers)
                            if (!livePeers.TryGetValue(Live.PeerId, out var still) || still != Live)
                                return;

                    }

                    var now = Connections.Peers.FirstOrDefault(one => one.Id == Live.PeerId);

                    if (now is not null && now.Role != PeerRole.Service)
                        await DialPeer(now, next.Value, next == Live.RoundStart ? null : Live.RoundStart, default, Live.Dialled.Id);

                }
                catch (Exception e)
                {
                    Log.Warning($"Going on to the next endpoint of '{peer.Description}' failed: {e.Message}", "ocpp", "connections");
                }
                finally
                {
                    liveLock.Release();
                }

            });

        }

        #endregion

        #region (private) ApplyConnectionChanges()

        /// <summary>
        /// Take what is written down now: dial a peer that dials by itself and
        /// is not dialled, hang up what is dialled and should not be, and dial
        /// again an endpoint that was changed in how it is dialled.
        /// </summary>
        /// <remarks>
        /// Told by the store after every change it wrote; nothing here before
        /// this station was started. One at a time, and the last change taken
        /// is the one written last, as each looks at the store afresh.
        /// </remarks>
        protected async Task ApplyConnectionChanges()
        {

            if (!started || !peersDialled)
                return;

            await liveLock.WaitAsync();

            try
            {

                var connections  = Connections.Connections.ToDictionary(entry => entry.Id);
                var peers        = Connections.Peers.ToDictionary(peer => peer.Id);

                LiveEndpoint[] lives;

                lock (livePeers)
                    lives = [.. livePeers.Values];

                var dialAgain  = new Dictionary<String, Int32>();
                var hungUp     = new HashSet<String>();

                foreach (var live in lives)
                {

                    peers.      TryGetValue(live.PeerId,     out var peer);
                    connections.TryGetValue(live.Dialled.Id, out var entry);

                    String? why = null;

                    if (peer is null || peer.Role == PeerRole.Service)
                        why = $"'{live.Dialled.Description}' is no longer dialled by itself";

                    else if (entry is null || !peer.EndpointIds.Contains(entry.Id))
                        why = $"'{live.Dialled.Description}' is no longer an endpoint of '{peer.Description}'";

                    else if (Connections.DialRevisionOf(entry.Id) != live.Revision)
                        why = $"'{entry.Description}' was changed";

                    else if (IsPrimary(entry) != live.Primary)
                        why = $"which back end is in charge of OCPP {ConnectionEntry.AsText(entry.OCPPVersion)} changed";

                    if (why is null)
                        continue;

                    // The same endpoint again, where it still dials by itself;
                    // where not, its peer is dialled where it starts.
                    if (peer is not null && peer.Role != PeerRole.Service && entry is not null && entry.AutoConnect &&
                        peer.EndpointIds.Contains(entry.Id))
                        dialAgain[peer.Id] = peer.EndpointIds.ToList().IndexOf(entry.Id);

                    hungUp.Add(live.Dialled.Id);

                    lock (livePeers)
                    {
                        live.LetGo = true;
                        live.Pause.Cancel();
                        livePeers.Remove(live.PeerId);
                    }

                    Log.Notice($"{why}: it is hung up{(dialAgain.ContainsKey(live.PeerId) ? " and dialled again" : "")}.", "ocpp", "connections");

                    await HangUpEndpoint(live, $"{why}.");

                }

                // Lines of what is not dialled any more, nor waits in reserve,
                // and of what was hung up: what it is now is said below.
                HashSet<String> stillLive;

                lock (livePeers)
                    stillLive = [.. livePeers.Values.Select(live => live.Dialled.Id)];

                lock (dialled)
                    foreach (var id in dialled.Keys.ToArray())
                        if (hungUp.Contains(id) ||
                            !connections.TryGetValue(id, out var entry) ||
                            (!entry.AutoConnect && !stillLive.Contains(id)) ||
                            Connections.PeerOf(id) is not PeerEntry owner ||
                            owner.Role == PeerRole.Service)
                            dialled.Remove(id);

                foreach (var peer in peers.Values.
                                         Where  (peer => peer.Role != PeerRole.Service).
                                         OrderBy(peer => FirstDialledAt(peer, connections)))
                {

                    Boolean isLive;

                    lock (livePeers)
                        isLive = livePeers.ContainsKey(peer.Id);

                    if (isLive)
                    {

                        // An endpoint added to a peer that is up waits in reserve.
                        LiveEndpoint live;

                        lock (livePeers)
                            live = livePeers[peer.Id];

                        foreach (var id in peer.EndpointIds.Where(id => id != live.Dialled.Id))
                            lock (dialled)
                                if (!dialled.ContainsKey(id) && connections.TryGetValue(id, out var reserve) && reserve.AutoConnect)
                                    dialled[id] = new ConnectionState(ConnectionStatus.Standby,
                                                                      TimeProvider.GetUtcNow(),
                                                                      $"In reserve: '{live.Dialled.Description}' is dialled for '{peer.Description}', " +
                                                                       "and this endpoint when that one cannot be reached.",
                                                                      reserve.Description,
                                                                      reserve.URL,
                                                                      reserve.OCPPVersion);

                        continue;

                    }

                    var index = dialAgain.TryGetValue(peer.Id, out var again) && again >= 0
                                    ? again
                                    : StartOf(peer, connections);

                    Log.Info($"'{peer.Description}' is dialled{(dialAgain.ContainsKey(peer.Id) ? " again" : "")}, as written down now.", "ocpp", "connections");

                    await DialPeer(peer, index, null, default);

                }

            }
            catch (Exception e)
            {
                Log.Warning($"Taking the changed connections failed: {e.Message}", "ocpp", "connections");
            }
            finally
            {
                liveLock.Release();
            }

        }

        #endregion

        #region (private) HangUpEndpoint(Live, Reason) / EndRegistrationOf(Connection)

        /// <summary>
        /// Close the connection a peer is dialled at, so that it does not come
        /// back by itself, have the node forget its client, and end what it
        /// was registered as.
        /// </summary>
        protected async Task HangUpEndpoint(LiveEndpoint Live, String Reason)
        {

            try
            {

                switch (Live.Client ?? ClientOf(Live.Dialled))
                {

                    case WWCPSshClient sshClient:
                        await LetGo(sshClient, Reason);
                        break;

                    case Object client:
                        await Disconnect(client, Reason);
                        break;

                }

            }
            catch (Exception e)
            {
                Log.Debug($"The connection to {Live.Dialled.URL} did not close cleanly: {e.Message}", "ocpp", "connections");
            }

            EndRegistrationOf(Live.Dialled);

        }

        /// <summary>
        /// Nothing more is asked of the back end over the given connection, nor
        /// waited for: no BootNotification, no heartbeat.
        /// </summary>
        protected virtual void EndRegistrationOf(ConnectionEntry Connection)
        { }

        #endregion

        #region (private static) StartOf(Peer, Connections) / FirstDialledAt(Peer, Connections)

        /// <summary>
        /// The endpoint a peer is dialled at first: the first of them that dials
        /// by itself.
        /// </summary>
        protected static Int32 StartOf(PeerEntry Peer, IReadOnlyDictionary<String, ConnectionEntry> Connections)
        {

            for (var i = 0; i < Peer.EndpointIds.Count; i++)
                if (Connections.TryGetValue(Peer.EndpointIds[i], out var entry) && entry.AutoConnect)
                    return i;

            return 0;

        }

        /// <summary>
        /// The endpoint of a peer that is dialled after the given one: the
        /// next one that dials by itself, round and round; null where none
        /// does.
        /// </summary>
        protected static Int32? NextDialable(PeerEntry Peer, Int32 After, IReadOnlyDictionary<String, ConnectionEntry> Connections)
        {

            for (var step = 1; step <= Peer.EndpointIds.Count; step++)
            {
                var index = (After + step) % Peer.EndpointIds.Count;
                if (Connections.TryGetValue(Peer.EndpointIds[index], out var entry) && entry.AutoConnect)
                    return index;
            }

            return null;

        }

        protected static DateTimeOffset FirstDialledAt(PeerEntry Peer, IReadOnlyDictionary<String, ConnectionEntry> Connections)

            => Peer.EndpointIds.
                   Select(id => Connections.TryGetValue(id, out var entry) && entry.AutoConnect ? entry.CreatedAt : DateTimeOffset.MaxValue).
                   DefaultIfEmpty(DateTimeOffset.MaxValue).
                   Min();

        #endregion

        #region (private) LetGoOfPeers()

        /// <summary>
        /// Forget what is dialled of the peers, for a station that hangs up.
        /// </summary>
        protected void LetGoOfPeers()
        {
            lock (livePeers)
            {
                peersDialled = false;
                foreach (var live in livePeers.Values)
                {
                    live.LetGo = true;
                    live.Pause.Cancel();
                }
                livePeers.Clear();
            }
        }

        #endregion

    }

}
