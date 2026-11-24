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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.Logins;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// Where what a charging station sends up is forwarded to: the forwarding
    /// target its login names - the first of its peers that is connected -, or
    /// the back end in charge of this node where it names none. And what a
    /// back end may send down: to the stations whose target it is in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A station that names a target is held to it both ways. Up, nothing it
    /// sends goes to a back end outside its target - none of them being
    /// connected, it is told so, and nothing falls back to the back end in
    /// charge, which would be a CSMS nobody chose for it. Down, a back end
    /// outside its target reaches it not: CSMS #2 does not start a
    /// transaction at a station that is CSMS #1's.
    /// </para>
    /// <para>
    /// A station that names none belongs to the back end in charge - the
    /// primary -, as every station did before there were targets: it is
    /// forwarded to it, and reached from it, and from no other back end.
    /// </para>
    /// <para>
    /// What a station's answers to a back end take is not decided here: an
    /// answer goes back to whoever asked.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region Forwarding / ForwardsByDefault / Forwards

        /// <summary>
        /// What the configuration file says of forwarding.
        /// </summary>
        public ForwardingConfiguration Forwarding { get; private set; } = new ();

        /// <summary>
        /// Whether this kind of node forwards where its file does not say: as its OCPP node passes on
        /// what nobody wrote a filter for - a gateway's and a local controller's do, a charging
        /// station's and a CSMS's do not.
        /// </summary>
        public Boolean ForwardsByDefault
            => Node21.OCPP.FORWARD.DefaultForwardingDecision == ForwardingDecisions.FORWARD;

        /// <summary>
        /// Whether this node forwards now.
        /// </summary>
        public Boolean Forwards
            => Forwarding.IsEnabled(ForwardsByDefault);

        private readonly Lock forwardingLock = new ();

        #endregion

        #region (private) ReadForwarding()

        /// <summary>
        /// The forwarding section of the configuration file, read with the station port's: wrong, it
        /// stops the node, as a wrong station port section does.
        /// </summary>
        private void ReadForwarding()
        {

            if (!ForwardingConfiguration.TryParseSectionOf(ConfigurationDocument, out var forwarding, out var problem))
                throw new InvalidOperationException($"'{ConfigFile.Path}': {problem} Repair or remove '{ConfigFile.Path}' and start again.");

            Forwarding = forwarding ?? new ForwardingConfiguration();

            if (!Forwards)
                Log.Notice($"This {NodeNoun} forwards nothing: what is not for itself is refused, as its configuration says.", "ocpp", "forwarding");

        }

        #endregion

        #region ForwardingJSON() / TryUpdateForwardingConfiguration(JSON, out Error [, out NotSaved])

        /// <summary>
        /// Forwarding, as the web interface reads it: whether this node forwards, what its file says
        /// and what its kind does where the file says nothing.
        /// </summary>
        public JObject ForwardingJSON()

            => new (
                   new JProperty("enabled",    Forwards),
                   new JProperty("configured", Forwarding.Enabled),
                   new JProperty("byDefault",  ForwardsByDefault),
                   new JProperty("file",       ConfigFile.Path)
               );

        /// <summary>
        /// Switch forwarding on or off, or back to what this kind of node does.
        /// </summary>
        public Boolean TryUpdateForwardingConfiguration(JObject                           JSON,
                                                        [NotNullWhen(false)] out String?  Error)

            => TryUpdateForwardingConfiguration(JSON, out Error, out _);

        /// <summary>
        /// Switch forwarding on or off: written to the file, then in effect from the next message on,
        /// without a restart.
        /// </summary>
        /// <param name="JSON">The new section.</param>
        /// <param name="Error">What is wrong with it, or what stood in the way.</param>
        /// <param name="NotSaved">True where the configuration file could not be read or written: nothing about the change was wrong, and nothing was changed.</param>
        public Boolean TryUpdateForwardingConfiguration(JObject                           JSON,
                                                        [NotNullWhen(false)] out String?  Error,
                                                        out Boolean                       NotSaved)
        {

            NotSaved = false;

            if (!ForwardingConfiguration.TryParse(JSON, out var wanted, out Error))
                return false;

            lock (forwardingLock)
            {

                if (wanted == Forwarding)
                    return true;

                if (!ConfigFile.TryReplaceSection(ForwardingConfiguration.SectionName, wanted.ToJSON(), out Error))
                {
                    NotSaved = true;
                    return false;
                }

                Forwarding = wanted;

            }

            Log.Notice(Forwards
                           ? $"This {NodeNoun} forwards now: what its charging stations send up goes to their targets, and what their back ends ask of them goes down."
                           : $"This {NodeNoun} forwards nothing now: what is not for itself is refused.",
                       "ocpp", "forwarding");

            return true;

        }

        #endregion


        #region (protected) RouteByTarget(Node, Connection, Action, Destination, NetworkPath)

        /// <summary>
        /// Where what came over the given connection and passed the gates is
        /// forwarded to by the targets: another destination, a refusal, or
        /// neither - it goes where it is addressed to.
        /// </summary>
        /// <param name="Node">The OCPP 2.1 node of this node.</param>
        /// <param name="Connection">What it came over.</param>
        /// <param name="Action">What it is.</param>
        /// <param name="Destination">Where it is addressed to.</param>
        /// <param name="NetworkPath">Where it came from.</param>
        protected (SourceRouting? To, String? Refused) RouteByTarget(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                                                    IMessageConnection                           Connection,
                                                                    String                                       Action,
                                                                    SourceRouting                                Destination,
                                                                    NetworkPath                                  NetworkPath)
        {

            // Switched off: nothing is passed on, and whoever sent it is told.
            if (!Forwards)
                return (null, $"'{Action}' for '{Destination.Next}' is not sent on: this {NodeNoun} forwards nothing.");

            if (WebSocketLogins is null || Connections is null)
                return (null, null);

            // Up, from a charging station of the station port: the gates made
            // sure it sends in its own name.
            if (Connection.Role == ConnectionRole.Server && !IsInbound(Connection))
                return Up(Node, NetworkPath.Source, Action, Destination);

            // Down, from a back end: to a station of this node, by its login.
            if (BackEndOf(Connection) is ConnectionEntry backEnd &&
                User_Id.TryParse(Destination.Next.ToString()) is User_Id login &&
                WebSocketLogins.TryGet(login, out var station))
            {
                return Down(backEnd, station, Action) is String refused
                           ? (null, refused)
                           : (null, null);
            }

            return (null, null);

        }

        #endregion

        #region (private) Up(Node, Station, Action, Destination)

        /// <summary>
        /// What a charging station sends up: to the first connected peer of its
        /// target, or refused where none is - and where it names a back end
        /// outside its target itself.
        /// </summary>
        private (SourceRouting? To, String? Refused) Up(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                                       NetworkingNode_Id                            Station,
                                                       String                                       Action,
                                                       SourceRouting                                Destination)
        {

            if (User_Id.TryParse(Station.ToString()) is not User_Id login ||
                WebSocketLogins.TargetOf(login) is not ForwardingTarget target)
            {
                return (null, null);
            }

            var toTheBackEndInCharge = Destination.Next == NetworkingNode_Id.CSMS ||
                                       Destination.Next == NetworkingNode_Id.CentralSystem;

            // Addressed to a back end by its own name: one of the target's, or
            // refused. Addressed to anything else - this node itself -: as it is.
            if (!toTheBackEndInCharge)
            {

                if (!IsABackEnd(Node, Destination.Next))
                    return (null, null);

                if (EndpointsOf(target).Any(endpoint => NextHopOf(endpoint) == Destination.Next))
                    return (null, null);

                return (null, $"'{Action}' from the charging station '{Station}' is not sent on to '{Destination.Next}': its target '{target.Id}' does not forward to it.");

            }

            if (FirstConnectedOf(target) is ConnectionEntry endpoint)
                return (SourceRouting.To(NextHopOf(endpoint)), null);

            var why = $"'{Action}' from the charging station '{Station}' is not sent on: none of the back ends of its target '{target.Id}' " +
                      $"({String.Join(", ", target.PeerIds.Select(peer => $"'{peer}'"))}) is connected.";

            Log.Warning(why, "ocpp", "station", "forwarding");

            return (null, why);

        }

        #endregion

        #region (private) Down(BackEnd, Station, Action)

        /// <summary>
        /// Why what a back end sends down does not reach the given charging
        /// station - or null, where it does.
        /// </summary>
        private String? Down(ConnectionEntry  BackEnd,
                             WebSocketLogin   Station,
                             String           Action)
        {

            if (Station.Target is null)
                return IsPrimary(BackEnd)
                           ? null
                           : Refused($"'{Action}' from '{BackEnd.Description.FirstText()}' is not sent on to the charging station '{Station.Login}': " +
                                     $"a station without a target belongs to the back end in charge of this {NodeNoun}.");

            var peer = Connections.PeerOf(BackEnd.Id);

            if (peer is not null &&
                WebSocketLogins.GetTarget(Station.Target) is ForwardingTarget target &&
                target.PeerIds.Contains(peer.Id))
            {
                return null;
            }

            return Refused($"'{Action}' from '{BackEnd.Description.FirstText()}' is not sent on to the charging station '{Station.Login}': " +
                           $"its target '{Station.Target}' does not forward to it.");

        }

        private String Refused(String Why)
        {
            Log.Warning(Why, "ocpp", "station", "forwarding", "security");
            return Why;
        }

        #endregion


        #region (override) StationsByPeer()

        /// <summary>
        /// How many of the charging stations this node serves each of its peers
        /// gets - counted by their logins, those switched off left out, and
        /// those a back end that connects here signs in with -, or null where
        /// this node does not forward.
        /// </summary>
        /// <remarks>
        /// A station without a target is counted for every peer that is the back
        /// end in charge: there is one for each version of OCPP.
        /// </remarks>
        public override IReadOnlyDictionary<String, PeerStations>? StationsByPeer()
        {

            if (!Forwards || WebSocketLogins is not WebSocketLogins logins)
                return null;

            var peers        = Connections.Peers;
            var connections  = Connections.Connections.ToDictionary(entry => entry.Id);
            var states       = ConnectionStates;
            var backEnds     = connections.Values.Where (entry => entry.Inbound).
                                              Select(entry => entry.InboundName).
                                              OfType<String>().
                                              ToHashSet(StringComparer.Ordinal);
            var stations     = logins.Logins.Where(login => login.Enabled && !backEnds.Contains(login.Login.ToString())).ToArray();
            var withoutOne   = stations.Count(login => login.Target is null);

            // Where each target forwards to now: the peer of its first connected endpoint.
            var nowAt        = new Dictionary<String, String?>();
            foreach (var target in logins.Targets)
                nowAt[target.Id] = FirstConnectedOf(target) is ConnectionEntry endpoint
                                       ? Connections.PeerOf(endpoint.Id)?.Id
                                       : null;

            var byPeer       = new Dictionary<String, PeerStations>();

            foreach (var peer in peers)
            {

                var endpoints  = peer.EndpointIds.
                                     Select(id => connections.TryGetValue(id, out var entry) ? entry : null).
                                     OfType<ConnectionEntry>().
                                     ToArray();

                var inCharge   = endpoints.Any(IsPrimary);
                var connected  = endpoints.Any(endpoint => states.TryGetValue(endpoint.Id, out var state) &&
                                                           state.Status == ConnectionStatus.Connected);

                var first      = 0;
                var inReserve  = 0;
                var now        = inCharge && connected ? withoutOne : 0;

                foreach (var station in stations)
                {

                    if (station.Target is null ||
                        logins.GetTarget(station.Target) is not ForwardingTarget target)
                        continue;

                    var at = target.PeerIds.ToList().IndexOf(peer.Id);

                    if      (at == 0) first++;
                    else if (at >  0) inReserve++;

                    if (nowAt.TryGetValue(target.Id, out var nowPeer) && nowPeer == peer.Id)
                        now++;

                }

                byPeer[peer.Id] = new PeerStations(
                                      First:          first,
                                      InReserve:      inReserve,
                                      WithoutTarget:  inCharge ? withoutOne : 0,
                                      Now:            now
                                  );

            }

            return byPeer;

        }

        #endregion


        #region (protected) FirstConnectedOf(Target)

        /// <summary>
        /// The endpoint the given target forwards to now: of its peers in their
        /// order, of each peer's endpoints in theirs, the first that is
        /// connected - or null, where none is.
        /// </summary>
        protected ConnectionEntry? FirstConnectedOf(ForwardingTarget Target)
        {

            var states = ConnectionStates;

            return EndpointsOf(Target).FirstOrDefault(endpoint => states.TryGetValue(endpoint.Id, out var state) &&
                                                                  state.Status == ConnectionStatus.Connected);

        }

        /// <summary>
        /// The endpoints of the peers of a target, in the order they are tried.
        /// A peer that is gone is passed over.
        /// </summary>
        private IEnumerable<ConnectionEntry> EndpointsOf(ForwardingTarget Target)
        {

            var peers        = Connections.Peers;
            var connections  = Connections.Connections;

            foreach (var peerId in Target.PeerIds)
                if (peers.FirstOrDefault(peer => peer.Id == peerId) is PeerEntry peer)
                    foreach (var endpointId in peer.EndpointIds)
                        if (connections.FirstOrDefault(connection => connection.Id == endpointId) is ConnectionEntry endpoint &&
                            endpoint.Use == ConnectionUse.OCPP)
                        {
                            yield return endpoint;
                        }

        }

        #endregion

        #region (private) BackEndOf(Connection) / IsABackEnd(Node, Name)

        /// <summary>
        /// The connection of this node the given one is: one the other end
        /// made and was let in as, or one this node made - or null, for a
        /// charging station.
        /// </summary>
        private ConnectionEntry? BackEndOf(IMessageConnection Connection)
        {

            if (Connection.TryGetCustomData(InboundConnectionKey) is String inboundId)
                return Connections.Connections.FirstOrDefault(entry => entry.Id == inboundId);

            if (Connection.WWCPClientOf() is IWWCPClient client)
                return Connections.Connections.FirstOrDefault(entry => !entry.Inbound &&
                                                                       entry.Use == ConnectionUse.OCPP &&
                                                                       ReferenceEquals(ClientOf(entry), client));

            return null;

        }

        /// <summary>
        /// Whether the given name leads to a back end of this node: over a
        /// connection it made, or over one the back end made.
        /// </summary>
        private Boolean IsABackEnd(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                   NetworkingNode_Id                            Name)

            => LeadsToInbound(Name) ||
               (Node.Routing.LookupNetworkingNode(Name, out var reachability) && reachability.Client is not null);

        #endregion

    }

}
