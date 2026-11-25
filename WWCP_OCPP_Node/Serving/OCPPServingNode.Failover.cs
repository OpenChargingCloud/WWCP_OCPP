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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.Logins;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What a charging station sends up while its back ends come and go: the back end
    /// it is at is told who it is when it changes - its BootNotification and the state
    /// of each of its connectors -, a charging session stays with the back end it began
    /// at, and what of one cannot reach it is held back and sent when it can.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A station is at the first connected back end of its target that did not turn it
    /// away, or, without a target, at the back end in charge of this node. It changes
    /// when that one goes and another one is first, and back again when the one before
    /// it returns: the order of a target is an order of preference.
    /// </para>
    /// <para>
    /// While none of its back ends is connected, this node answers it what it can
    /// answer for one: a Heartbeat with its own time, a StatusNotification - kept, and
    /// told the back end later -, and every part of a charging session - held back for
    /// the back end it belongs to -, with an answer that says nothing more than that it
    /// was taken: a TransactionEvent has no idTokenInfo then. Nothing else is answered
    /// here; an Authorize least of all.
    /// </para>
    /// <para>
    /// What the station was answered by this node it does not hold back itself: it
    /// believes itself online. So nothing of what is held back is given up as long as
    /// the back end it belongs to is written down; what its back end refuses to take
    /// is.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region Data

        /// <summary>
        /// How long an answer to what this node sends in a station's name is waited for.
        /// </summary>
        public static readonly TimeSpan  OnBehalfTimeout        = TimeSpan.FromSeconds(30);

        /// <summary>
        /// How often this node looks whether the back ends of its stations changed.
        /// </summary>
        public static readonly TimeSpan  UpstreamCheckInterval  = TimeSpan.FromSeconds(5);

        private readonly ConcurrentDictionary<NetworkingNode_Id, StationUpstream>  upstreams  = new ();
        private readonly ConcurrentDictionary<String, Byte>                         sending    = new (StringComparer.Ordinal);
        private readonly Lock                                                       heldBackLock = new ();
        private          HeldBackMessages?                                          heldBack;
        private          Timer?                                                     upstreamTimer;
        private          Int32                                                      reevaluating;
        private          HashSet<String>                                            connectedBefore = new (StringComparer.Ordinal);

        #endregion

        #region Properties

        /// <summary>
        /// Where what is held back for back ends is kept: a directory the node names,
        /// before it forwards anything, or null for memory alone.
        /// </summary>
        protected String? HeldBackDirectory { get; set; }

        /// <summary>
        /// What is held back for the back ends of this node's stations, and which back
        /// end each charging session belongs to.
        /// </summary>
        public HeldBackMessages HeldBack
        {
            get
            {
                lock (heldBackLock)
                {

                    heldBack ??= new HeldBackMessages(
                                     HeldBackDirectory,
                                     OnProblem: problem => Log.Warning(problem, "ocpp", "forwarding")
                                 );

                    return heldBack;

                }
            }
        }

        #endregion


        #region (private class) StationUpstream

        /// <summary>
        /// What this node knows of a charging station towards its back ends.
        /// </summary>
        private sealed class StationUpstream(NetworkingNode_Id Station)
        {

            public NetworkingNode_Id              Station     { get; } = Station;

            /// <summary>
            /// One at a time: what it sends, and what this node tells a back end of it.
            /// </summary>
            public SemaphoreSlim                  Lock        { get; } = new (1, 1);

            public OCPPVersion                    Version     { get; set; }
            public NetworkingMode                 Mode        { get; set; }

            /// <summary>
            /// The BootNotification it sent last.
            /// </summary>
            public JObject?                       Boot        { get; set; }

            /// <summary>
            /// The state of each of its connectors, and the back end that was told it.
            /// </summary>
            public Dictionary<String, (JObject Payload, String? ToldTo)>  Statuses  { get; } = new (StringComparer.Ordinal);

            /// <summary>
            /// The back end it is at: the one its BootNotification went to last.
            /// </summary>
            public String?                        At          { get; set; }

            /// <summary>
            /// The back ends that turned it away, until they are connected anew.
            /// </summary>
            public HashSet<String>                TurnedAway  { get; } = new (StringComparer.Ordinal);

        }

        #endregion


        #region (private) UpstreamDecision(Version, Station, Request)

        /// <summary>
        /// What happens to a request a charging station sends up to its back end in
        /// charge, where its back ends coming and going decides it - or null, where
        /// they do not and the targets decide as before.
        /// </summary>
        /// <param name="Version">The version of OCPP it came in.</param>
        /// <param name="Station">The charging station, which sends in its own name: the gates made sure of it.</param>
        /// <param name="Request">What it sent.</param>
        private async Task<UpstreamResult?> UpstreamDecision(OCPPVersion              Version,
                                                             NetworkingNode_Id        Station,
                                                             OCPP_JSONRequestMessage  Request)
        {

            if (!Forwards || WebSocketLogins is null || Connections is null ||
                User_Id.TryParse(Station.ToString()) is not User_Id login ||
                !WebSocketLogins.TryGet(login, out var station))
            {
                return null;
            }

            var backEnds = BackEndsOf(station, Version);

            if (backEnds.Count == 0)
                return null;

            var upstream = upstreams.GetOrAdd(Station, id => new StationUpstream(id));

            await upstream.Lock.WaitAsync();

            try
            {

                upstream.Version = Version;
                upstream.Mode    = Request.NetworkingMode;

                var action  = Request.Action;
                var payload = Request.Payload;

                // What it says of itself is kept, to tell a back end it changes to.
                if (action == "BootNotification")
                {
                    upstream.Boot = payload;
                    upstream.TurnedAway.Clear();
                }

                var withoutTarget = station.Target is null;

                // Brought up to date first - with what it was not told yet, not with
                // this, which goes to it as it is.
                var current       = await CurrentBackEnd(upstream, backEnds, Sync: action != "BootNotification");

                if (action == "StatusNotification" && StatusKeyOf(Version, payload) is String statusKey)
                    upstream.Statuses[statusKey] = (payload, current is not null ? BackEndKeyOf(current) : null);

                #region A part of a charging session: to the back end it began at

                if (SessionOf(Version, action, payload) is (String session, Boolean ends))
                {

                    var belongsTo = HeldBack.BackEndOf(Station, session);

                    if (belongsTo is null)
                    {
                        belongsTo = current is not null ? BackEndKeyOf(current) : BackEndKeyOf(backEnds[0]);
                        HeldBack.Bind(Station, session, belongsTo);
                    }

                    if (ConnectedEndpointOf(belongsTo, Version) is ConnectionEntry endpoint &&
                        HeldBack.CountFor(belongsTo) == 0 &&
                        !sending.ContainsKey(belongsTo))
                    {

                        if (ends)
                            HeldBack.Unbind(Station, session);

                        return withoutTarget
                                   ? UpstreamResult.Forward(null)
                                   : UpstreamResult.Forward(SourceRouting.To(NextHopOf(endpoint)));

                    }

                    if (!HeldBack.TryAdd(new HeldBackMessage(belongsTo, Station, Version, action, payload, Request.NetworkingMode, TimeProvider.GetUtcNow())))
                    {
                        var full = $"'{action}' of the charging station '{Station}' is not held back for '{belongsTo}': as many as it takes are held back for it already ({HeldBack.Max}).";
                        Log.Warning(full, "ocpp", "station", "forwarding");
                        return UpstreamResult.Refuse(full);
                    }

                    if (ends)
                        HeldBack.Unbind(Station, session);

                    Log.Info($"'{action}' of the charging station '{Station}' is held back for '{belongsTo}', which its charging session '{session}' began at" +
                             (ConnectedEndpointOf(belongsTo, Version) is null ? " and which is not connected." : ", behind what is held back for it already."),
                             "ocpp", "station", "forwarding");

                    _ = SendWhatIsHeldBack(belongsTo);

                    return UpstreamResult.Answer(new JObject());

                }

                #endregion

                #region A charging session begins, in OCPP 1.6: where it goes, it belongs to

                if (Version == OCPPVersion.OCPP1_6 && action == "StartTransaction" && current is not null)
                {

                    var at = BackEndKeyOf(current);

                    return withoutTarget
                               ? UpstreamResult.Forward(null, answer => BindStarted(Station, answer, at))
                               : UpstreamResult.Forward(SourceRouting.To(NextHopOf(current)), answer => BindStarted(Station, answer, at));

                }

                #endregion

                if (current is not null)
                {

                    var at = BackEndKeyOf(current);

                    if (action == "BootNotification")
                    {
                        upstream.At = at;
                        foreach (var key in upstream.Statuses.Keys.ToArray())
                            upstream.Statuses[key] = (upstream.Statuses[key].Payload, null);
                    }

                    // A station without a target goes where it is addressed to, as
                    // before, past the filters of the node; one with a target to the
                    // back end it is at - which may not be the first connected one of
                    // its target, where that one turned it away.
                    return withoutTarget
                               ? null
                               : UpstreamResult.Forward(SourceRouting.To(NextHopOf(current)),
                                                        action == "BootNotification"
                                                            ? answer => TurnedAwayBy(upstream, answer, at)
                                                            : null);

                }

                #region None of its back ends is connected: what can be answered for one

                switch (action)
                {

                    case "Heartbeat":
                        return UpstreamResult.Answer(new JObject(
                                   new JProperty("currentTime", TimeProvider.GetUtcNow().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                               ));

                    case "StatusNotification":
                        Log.Info($"The state of a connector of the charging station '{Station}' is kept, to be told its back end when one is connected: none is.",
                                 "ocpp", "station", "forwarding");
                        return UpstreamResult.Answer(new JObject());

                }

                #endregion

                return null;

            }
            finally
            {
                upstream.Lock.Release();
            }

        }

        #endregion

        #region (private record) UpstreamResult

        /// <summary>
        /// What happens to what a charging station sent up: forwarded - to a destination
        /// of its own, or where it is addressed to -, answered here, or refused.
        /// </summary>
        private sealed record UpstreamResult(ForwardingDecisions     Result,
                                             SourceRouting?          To,
                                             JObject?                Answered,
                                             String?                 Why,
                                             Func<JObject?, Task>?   OnAnswer)
        {

            public static UpstreamResult Forward(SourceRouting? To, Func<JObject?, Task>? OnAnswer = null)
                => new (ForwardingDecisions.FORWARD, To, null, null, OnAnswer);

            public static UpstreamResult Answer(JObject Answer)
                => new (ForwardingDecisions.REJECT, null, Answer, null, null);

            public static UpstreamResult Refuse(String Why)
                => new (ForwardingDecisions.REJECT, null, null, Why, null);

        }

        #endregion


        #region (private) CurrentBackEnd(Upstream, BackEnds, Sync)

        /// <summary>
        /// The back end the given station is at now: the first connected one that did
        /// not turn it away - told who it is first, where it was not at it, unless the
        /// station is about to tell it itself.
        /// </summary>
        private async Task<ConnectionEntry?> CurrentBackEnd(StationUpstream                 Upstream,
                                                            IReadOnlyList<ConnectionEntry>  BackEnds,
                                                            Boolean                         Sync)
        {

            var states = ConnectionStates;

            foreach (var backEnd in BackEnds)
            {

                if (!states.TryGetValue(backEnd.Id, out var state) || state.Status != ConnectionStatus.Connected)
                    continue;

                var key = BackEndKeyOf(backEnd);

                if (Upstream.TurnedAway.Contains(key))
                    continue;

                if (!Sync || await Synced(Upstream, backEnd))
                    return backEnd;

            }

            return null;

        }

        #endregion

        #region (private) Synced(Upstream, BackEnd)

        /// <summary>
        /// Tells the given back end who the given station is where it was not at it -
        /// its BootNotification - and the state of each of its connectors it was not
        /// told: false where it turned it away or did not answer.
        /// </summary>
        private async Task<Boolean> Synced(StationUpstream  Upstream,
                                           ConnectionEntry  BackEnd)
        {

            var key = BackEndKeyOf(BackEnd);

            if (Upstream.At != key)
            {

                if (Upstream.Boot is JObject boot)
                {

                    var (answered, answer) = await SendOnBehalfOf(Upstream, BackEnd, "BootNotification", boot);

                    if (!answered)
                    {
                        Log.Warning($"The charging station '{Upstream.Station}' is not at '{key}' yet: its BootNotification was not answered.",
                                    "ocpp", "station", "forwarding");
                        return false;
                    }

                    if (answer?["status"]?.Value<String>() is not String status || status == "Rejected" || answer is null)
                    {
                        Upstream.TurnedAway.Add(key);
                        Log.Warning($"'{key}' turned the charging station '{Upstream.Station}' away ({(answer is null ? "an error" : answer["status"])}): it goes to the next of its back ends.",
                                    "ocpp", "station", "forwarding");
                        return false;
                    }

                    Log.Notice($"The charging station '{Upstream.Station}' is at '{key}' now{(Upstream.At is null ? "" : $", no longer at '{Upstream.At}'")}: its BootNotification was answered '{status}'.",
                               "ocpp", "station", "forwarding");

                }
                else
                    Log.Notice($"The charging station '{Upstream.Station}' is at '{key}' now{(Upstream.At is null ? "" : $", no longer at '{Upstream.At}'")} - without a BootNotification, as it sent none since this {NodeNoun} started.",
                               "ocpp", "station", "forwarding");

                Upstream.At = key;

            }

            foreach (var (statusKey, (payload, toldTo)) in Upstream.Statuses.ToArray())
            {

                if (toldTo == key)
                    continue;

                var (answered, _) = await SendOnBehalfOf(Upstream, BackEnd, "StatusNotification", payload);

                if (answered)
                    Upstream.Statuses[statusKey] = (payload, key);

            }

            return true;

        }

        #endregion

        #region (private) TurnedAwayBy(Upstream, Answer, BackEnd) / BindStarted(Station, Answer, BackEnd)

        /// <summary>
        /// The answer to a BootNotification a station sent itself: where it turned the
        /// station away, the back end is passed over until it is connected anew.
        /// </summary>
        private Task TurnedAwayBy(StationUpstream  Upstream,
                                  JObject?         Answer,
                                  String           BackEnd)
        {

            if (Answer?["status"]?.Value<String>() == "Rejected")
                lock (Upstream.TurnedAway)
                    Upstream.TurnedAway.Add(BackEnd);

            return Task.CompletedTask;

        }

        /// <summary>
        /// The answer to a StartTransaction of OCPP 1.6: the charging session it names
        /// belongs to the back end that answered.
        /// </summary>
        private Task BindStarted(NetworkingNode_Id  Station,
                                 JObject?           Answer,
                                 String             BackEnd)
        {

            if (Answer?["transactionId"] is JValue { Type: JTokenType.Integer } transactionId)
                HeldBack.Bind(Station, transactionId.ToString(), BackEnd);

            return Task.CompletedTask;

        }

        #endregion


        #region (private) SendOnBehalfOf(Upstream, BackEnd, Action, Payload)

        private Task<(Boolean Answered, JObject? Answer)> SendOnBehalfOf(StationUpstream  Upstream,
                                                                         ConnectionEntry  BackEnd,
                                                                         String           Action,
                                                                         JObject          Payload)

            => SendOnBehalfOf(Upstream.Version, Upstream.Mode, Upstream.Station, BackEnd, Action, Payload);

        /// <summary>
        /// Sends the given request to the given back end in the name of the given
        /// station, as if the station had sent it, and waits for its answer.
        /// </summary>
        private async Task<(Boolean Answered, JObject? Answer)> SendOnBehalfOf(OCPPVersion        Version,
                                                                               NetworkingMode     Mode,
                                                                               NetworkingNode_Id  Station,
                                                                               ConnectionEntry    BackEnd,
                                                                               String             Action,
                                                                               JObject            Payload)
        {

            AWWCPNetworkingNode? node = Version == OCPPVersion.OCPP1_6 ? Node16 : Node21;

            if (node is null)
                return (false, null);

            var now     = TimeProvider.GetUtcNow();

            var request = new OCPP_JSONRequestMessage(
                              now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(NextHopOf(BackEnd)),
                              NetworkPath.From(Station).Append(node.Id),
                              Request_Id.NewRandom(),
                              Action,
                              Payload,
                              now + OnBehalfTimeout
                          );

            try
            {
                return Version == OCPPVersion.OCPP1_6
                           ? await Node16!.OCPP.FORWARD.SendOnBehalfOf(request, OnBehalfTimeout)
                           : await Node21. OCPP.FORWARD.SendOnBehalfOf(request, OnBehalfTimeout);
            }
            catch (Exception e)
            {
                Log.Warning($"'{Action}' could not be sent to '{BackEndKeyOf(BackEnd)}' in the name of the charging station '{Station}': {e.Message}",
                            "ocpp", "station", "forwarding");
                return (false, null);
            }

        }

        #endregion

        #region (private) SendWhatIsHeldBack(BackEnd)

        /// <summary>
        /// Sends what is held back for the given back end, in the order it came, while
        /// it is connected and answers: what it refuses is given up, what it does not
        /// answer is sent again later.
        /// </summary>
        private async Task SendWhatIsHeldBack(String BackEnd)
        {

            if (!sending.TryAdd(BackEnd, 0))
                return;

            var sent = 0;

            try
            {

                while (HeldBack.FirstFor(BackEnd) is HeldBackMessage message)
                {

                    if (ConnectedEndpointOf(BackEnd, message.Version) is not ConnectionEntry endpoint)
                        break;

                    var (answered, answer) = await SendOnBehalfOf(message.Version, message.Mode, message.Station, endpoint, message.Action, message.Payload);

                    if (!answered)
                        break;

                    if (answer is null)
                        Log.Warning($"'{BackEnd}' refused '{message.Action}' of the charging station '{message.Station}', held back since {message.Received:u}: it is given up.",
                                    "ocpp", "station", "forwarding");

                    HeldBack.RemoveFirst(BackEnd, message);
                    sent++;

                }

            }
            finally
            {
                sending.TryRemove(BackEnd, out _);
            }

            if (sent > 0)
                Log.Notice($"{sent} message{(sent == 1 ? "" : "s")} held back for '{BackEnd}' sent{(HeldBack.CountFor(BackEnd) is Int32 left and > 0 ? $", {left} still held back" : "")}.",
                           "ocpp", "station", "forwarding");

            // Came while the last one was sent, and found it sending.
            if (HeldBack.CountFor(BackEnd) > 0 && ConnectedEndpointOf(BackEnd, null) is not null && sent > 0)
                _ = SendWhatIsHeldBack(BackEnd);

        }

        #endregion

        #region (private) StartWatchingUpstreams() / ReevaluateUpstreams()

        /// <summary>
        /// From now on, this node looks every few seconds whether the back ends of its
        /// stations changed.
        /// </summary>
        private void StartWatchingUpstreams()
        {
            lock (heldBackLock)
                upstreamTimer ??= new Timer(_ => _ = ReevaluateUpstreams(), null, UpstreamCheckInterval, UpstreamCheckInterval);
        }

        /// <summary>
        /// Moves each connected station to the back end it is to be at, and sends what
        /// is held back for the back ends that are connected.
        /// </summary>
        protected async Task ReevaluateUpstreams()
        {

            if (Interlocked.Exchange(ref reevaluating, 1) == 1)
                return;

            try
            {

                if (!Forwards || WebSocketLogins is null || Connections is null)
                    return;

                var states    = ConnectionStates;
                var connected = Connections.Connections.
                                    Where (entry => states.TryGetValue(entry.Id, out var state) && state.Status == ConnectionStatus.Connected).
                                    Select(BackEndKeyOf).
                                    ToHashSet(StringComparer.Ordinal);

                // A back end that went is asked anew when it returns.
                var gone = connectedBefore.Except(connected).ToArray();
                connectedBefore = connected;

                foreach (var upstream in upstreams.Values)
                {

                    if (gone.Length > 0)
                        lock (upstream.TurnedAway)
                            upstream.TurnedAway.ExceptWith(gone);

                    if (!IsConnectedHere(upstream) ||
                        User_Id.TryParse(upstream.Station.ToString()) is not User_Id login ||
                        !WebSocketLogins.TryGet(login, out var station))
                    {
                        continue;
                    }

                    var backEnds = BackEndsOf(station, upstream.Version);

                    if (backEnds.Count == 0)
                        continue;

                    await upstream.Lock.WaitAsync();

                    try
                    {
                        await CurrentBackEnd(upstream, backEnds, Sync: true);
                    }
                    finally
                    {
                        upstream.Lock.Release();
                    }

                }

                foreach (var backEnd in HeldBack.BackEnds)
                    if (connected.Contains(backEnd))
                        await SendWhatIsHeldBack(backEnd);

            }
            catch (Exception e)
            {
                Log.Warning($"Looking at the back ends of the charging stations failed: {e.Message}", "ocpp", "forwarding");
            }
            finally
            {
                Interlocked.Exchange(ref reevaluating, 0);
            }

        }

        #endregion


        #region (private) BackEndsOf(Station, Version) / BackEndKeyOf(Connection) / ConnectedEndpointOf(BackEnd, Version)

        /// <summary>
        /// The back ends of the given station, in the order of preference: the endpoints
        /// of its target, or, without one, the back end in charge of this node.
        /// </summary>
        private IReadOnlyList<ConnectionEntry> BackEndsOf(WebSocketLogin  Station,
                                                          OCPPVersion     Version)
        {

            if (Station.Target is not null)
                return WebSocketLogins.GetTarget(Station.Target) is ForwardingTarget target
                           ? EndpointsOf(target, Version).ToArray()
                           : [];

            return Connections.Connections.
                       Where(entry => entry.Use == ConnectionUse.OCPP && entry.OCPPVersion == Version && IsPrimary(entry)).
                       ToArray();

        }

        /// <summary>
        /// What a back end is known by here: its peer, where the connection belongs to
        /// one, else the connection.
        /// </summary>
        private String BackEndKeyOf(ConnectionEntry Connection)

            => Connections.PeerOf(Connection.Id)?.Id ?? Connection.Id;

        /// <summary>
        /// A connected endpoint of the given back end, of the given version where one is given.
        /// </summary>
        private ConnectionEntry? ConnectedEndpointOf(String        BackEnd,
                                                     OCPPVersion?  Version)
        {

            var states = ConnectionStates;

            return Connections.Connections.FirstOrDefault(entry => entry.Use == ConnectionUse.OCPP                            &&
                                                                   (Version is null || entry.OCPPVersion == Version)           &&
                                                                   BackEndKeyOf(entry) == BackEnd                              &&
                                                                   states.TryGetValue(entry.Id, out var state)                 &&
                                                                   state.Status == ConnectionStatus.Connected);

        }

        /// <summary>
        /// Whether the given station is connected to this node now.
        /// </summary>
        private Boolean IsConnectedHere(StationUpstream Upstream)
        {

            AWWCPNetworkingNode? node = Upstream.Version == OCPPVersion.OCPP1_6 ? Node16 : Node21;

            return node is not null &&
                   node.Routing.LookupNetworkingNode(Upstream.Station, out var reachability) &&
                   reachability.Client is null;

        }

        #endregion

        #region (private static) StatusKeyOf(Version, Payload) / SessionOf(Version, Action, Payload)

        /// <summary>
        /// Which connector a StatusNotification tells the state of.
        /// </summary>
        private static String? StatusKeyOf(OCPPVersion  Version,
                                           JObject      Payload)

            => Version == OCPPVersion.OCPP1_6
                   ? Payload["connectorId"]?.ToString()
                   : Payload["evseId"] is JToken evse && Payload["connectorId"] is JToken connector
                         ? $"{evse}/{connector}"
                         : null;

        /// <summary>
        /// The charging session the given request is a part of, and whether it ends it -
        /// or null, where it is a part of none: a TransactionEvent of OCPP 2.1, and in
        /// OCPP 1.6 a StopTransaction and the MeterValues of a transaction.
        /// </summary>
        private static (String Session, Boolean Ends)? SessionOf(OCPPVersion  Version,
                                                                 String       Action,
                                                                 JObject      Payload)
        {

            if (Version == OCPPVersion.OCPP2_1)
                return Action == "TransactionEvent" &&
                       Payload["transactionInfo"]?["transactionId"]?.Value<String>() is String transactionId
                           ? (transactionId, Payload["eventType"]?.Value<String>() == "Ended")
                           : null;

            return Action switch {
                       "StopTransaction" when Payload["transactionId"] is JToken stopped  => (stopped.ToString(), true),
                       "MeterValues"     when Payload["transactionId"] is JToken metered  => (metered.ToString(), false),
                       _                                                                  => null
                   };

        }

        #endregion

    }

}
