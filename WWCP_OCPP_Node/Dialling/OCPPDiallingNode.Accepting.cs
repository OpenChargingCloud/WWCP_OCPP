/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The connections the other end makes: a CSMS or a local controller that
    /// dials this node, on an OCPP server of its own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OCPP as it always is, with who opens the WebSocket the other way round.
    /// What dials in is written down as a connection (<see
    /// cref="ConnectionEntry.Inbound"/>), and from the moment it is in it is
    /// one: routed under a name of its own, the primary under the name of the
    /// CSMS as well, told that this node started, sent what waits - by the
    /// same code that does all of that for a connection this node dialled.
    /// </para>
    /// <para>
    /// Nothing gets in that is not written down. The server asks for
    /// credentials, and the only ones it knows are those of the inbound
    /// connections that are switched on: the name the other end comes under,
    /// and the password or the one-time password its connection names.
    /// </para>
    /// <para>
    /// A kind of node attaches the servers - one for each version of OCPP it
    /// speaks, on a port of its own - and hands them over with <see
    /// cref="ServeInbound"/>.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        /// <summary>
        /// The OCPP servers of this node, by the version they speak.
        /// </summary>
        private readonly ConcurrentDictionary<OCPPVersion, OCPPWebSocketServer>       inboundServers      = new ();

        /// <summary>
        /// The connection each inbound connection is in over now, by the id of
        /// its entry: what tells the close of that one from the close of one
        /// it replaced.
        /// </summary>
        private readonly ConcurrentDictionary<String, WebSocketServerConnection>      inboundConnections  = new ();

        /// <summary>
        /// The inbound connections reached under the name of the CSMS as well,
        /// by the id of their entry: what is taken away again when they go.
        /// </summary>
        private readonly ConcurrentDictionary<String, NetworkingNode_Id[]>            inboundCSMSNames    = new ();

        private          Int32                                                        followsInboundChanges;

        #endregion

        #region InboundServed(Version)

        /// <summary>
        /// Whether this node serves the given version of OCPP to whoever dials
        /// it.
        /// </summary>
        public Boolean InboundServed(OCPPVersion Version)
            => inboundServers.ContainsKey(Version);

        #endregion

        #region (protected) ServeInbound(Version, Server) / StopServingInbound(Version)

        /// <summary>
        /// Let in over the given server what is written down as an inbound
        /// connection of the given version of OCPP. The server is attached to
        /// the node of that version and asks for credentials; which ones it
        /// knows is decided here, and follows the connections as they change.
        /// </summary>
        /// <param name="Version">The version of OCPP the server speaks.</param>
        /// <param name="Server">The server, attached to the node of that version.</param>
        protected void ServeInbound(OCPPVersion          Version,
                                    OCPPWebSocketServer  Server)
        {

            inboundServers[Version] = Server;

            // Routed under its own names before the 101 goes out, as the node
            // routes the name it came under: what is said to it right after it
            // connected finds it.
            Server.OnNetworkingNodeWebSocketConnectionAccepted += (timestamp, server, connection, networkingNodeId, networkingMode, sharedSubprotocols, eventTrackingId, cancellationToken) => {

                if (ReferenceEquals(inboundServers.GetValueOrDefault(Version), Server) &&
                    InboundOf(Version, networkingNodeId.ToString()) is ConnectionEntry entry)
                {
                    RouteInbound(entry, networkingNodeId, connection);
                }

                return Task.CompletedTask;

            };

            // In: said, and then what is said to every connection that is up.
            Server.OnNetworkingNodeNewWebSocketConnection += (timestamp, server, connection, networkingNodeId, networkingMode, sharedSubprotocols, eventTrackingId, cancellationToken) => {

                if (!ReferenceEquals(inboundServers.GetValueOrDefault(Version), Server))
                    return Task.CompletedTask;

                if (InboundOf(Version, networkingNodeId.ToString()) is not ConnectionEntry entry)
                {

                    // Let in by credentials that are no longer anybody's: taken
                    // away between its handshake and now.
                    Log.Warning($"'{networkingNodeId}' dialled in from {connection.RemoteSocket} and is no inbound connection of this {NodeWording.Noun} any more: it is let go of.",
                                "ocpp", "connections", "inbound");

                    _ = connection.Close(WebSocketFrame.ClosingStatusCode.PolicyViolation, "Not a connection of this node");

                    return Task.CompletedTask;

                }

                Note(entry, $"'{networkingNodeId}' dialled in from {connection.RemoteSocket}" +
                            (connection.Subprotocol is String subprotocol ? $" ({subprotocol})" : "") +
                            ", and is let in again whenever it dials.");

                ConnectionIsUp(entry);

                return Task.CompletedTask;

            };

            Server.OnNetworkingNodeCloseMessageReceived += (timestamp, server, connection, networkingNodeId, eventTrackingId, statusCode, reason, cancellationToken) => {

                InboundIsDown(Version, networkingNodeId, connection,
                              $"{(UInt16) statusCode} {statusCode}{(String.IsNullOrWhiteSpace(reason) ? "" : $": {reason}")}");

                return Task.CompletedTask;

            };

            Server.OnNetworkingNodeTCPConnectionClosed += (timestamp, server, connection, networkingNodeId, eventTrackingId, reason, cancellationToken) => {

                InboundIsDown(Version, networkingNodeId, connection,
                              String.IsNullOrWhiteSpace(reason) ? "its connection ended" : reason);

                return Task.CompletedTask;

            };

            if (Interlocked.Exchange(ref followsInboundChanges, 1) == 0)
                Connections.OnConnectionsChanged += FollowInboundConnections;

            FollowInboundConnections();

        }

        /// <summary>
        /// Let nothing in any more for the given version of OCPP: whoever is
        /// in is let go of, and the server is the kind's to stop.
        /// </summary>
        /// <param name="Version">The version of OCPP.</param>
        /// <param name="Why">Why, as the other end is told.</param>
        protected void StopServingInbound(OCPPVersion  Version,
                                          String       Why)
        {

            if (!inboundServers.TryRemove(Version, out var server))
                return;

            foreach (var entry in Connections.Connections.Where(entry => entry.Inbound && entry.OCPPVersion == Version))
            {

                if (inboundConnections.TryRemove(entry.Id, out var connection))
                {
                    UnrouteInbound(entry, server);
                    _ = connection.Close(WebSocketFrame.ClosingStatusCode.GoingAway, Why);
                }

                WaitFor(entry);

            }

            foreach (var login in server.ClientLogins.Keys.ToArray())
                server.RemoveHTTPBasicAuth(login);

            server.ClientTOTPConfig.Clear();

        }

        #endregion

        #region (private) InboundOf(Version, Name)

        /// <summary>
        /// The inbound connection of the given version of OCPP that comes under
        /// the given name and is switched on, or null.
        /// </summary>
        private ConnectionEntry? InboundOf(OCPPVersion  Version,
                                           String       Name)

            => Connections.Connections.FirstOrDefault(entry => entry.Inbound &&
                                                               entry.AutoConnect &&
                                                               entry.OCPPVersion == Version &&
                                                               entry.InboundName == Name);

        #endregion

        #region (private) FollowInboundConnections()

        /// <summary>
        /// What the servers know of who may come in, made what the connections
        /// say now: the credentials of every inbound connection that is
        /// switched on, and nobody else's. Who is in and may no longer be is
        /// let go of.
        /// </summary>
        private void FollowInboundConnections()
        {

            try
            {

                var authentications = Connections.Authentications.ToDictionary(credentials => credentials.Id);

                foreach (var (version, server) in inboundServers.ToArray())
                {

                    var passwords  = new Dictionary<String, String>    (StringComparer.Ordinal);
                    var totps      = new Dictionary<String, TOTPConfig>(StringComparer.Ordinal);
                    var letIn      = new HashSet<String>               (StringComparer.Ordinal);

                    foreach (var entry in Connections.Connections.Where(entry => entry.Inbound && entry.OCPPVersion == version).ToArray())
                    {

                        var name = entry.InboundName;

                        if (name is not null &&
                            entry.AutoConnect &&
                            entry.AuthenticationId is not null &&
                            authentications.TryGetValue(entry.AuthenticationId, out var credentials) &&
                            credentials.Login == name)
                        {

                            if (credentials.Kind == AuthenticationKind.Basic && !String.IsNullOrEmpty(credentials.Password))
                            {
                                passwords[name] = credentials.Password;
                                letIn.Add(entry.Id);
                            }

                            else if (credentials.Kind == AuthenticationKind.TOTP && !credentials.TLSChannelBinding &&
                                     credentials.ToTOTPConfig() is TOTPConfig totp)
                            {
                                totps[name] = totp;
                                letIn.Add(entry.Id);
                            }

                        }

                        // In, and no longer to be: switched off, its credentials
                        // gone or no longer its own.
                        if (!letIn.Contains(entry.Id))
                        {

                            if (inboundConnections.TryRemove(entry.Id, out var connection))
                            {

                                UnrouteInbound(entry, server);

                                _ = connection.Close(WebSocketFrame.ClosingStatusCode.PolicyViolation, "No longer a connection of this node");

                                Log.Notice($"'{entry.Description}' is let go of: it is no longer let in.", "ocpp", "connections", "inbound");

                            }

                            if (!IsUp(entry))
                                WaitFor(entry);

                        }

                        else if (!IsUp(entry))
                            WaitFor(entry);

                    }

                    foreach (var login in server.ClientLogins.Keys.Where(login => !passwords.ContainsKey(login)).ToArray())
                        server.RemoveHTTPBasicAuth(login);

                    foreach (var login in server.ClientTOTPConfig.Keys.Where(login => !totps.ContainsKey(login)).ToArray())
                        server.ClientTOTPConfig.TryRemove(login, out _);

                    foreach (var (login, password) in passwords)
                        server.AddOrUpdateHTTPBasicAuth(login, password);

                    foreach (var (login, totp) in totps)
                        server.ClientTOTPConfig[login] = totp;

                }

                // What was in for a connection that is not written down any more.
                foreach (var id in inboundConnections.Keys.ToArray())
                {
                    if (Connections.Connections.FirstOrDefault(one => one.Id == id) is not { Inbound: true })
                    {

                        if (inboundConnections.TryRemove(id, out var connection))
                            _ = connection.Close(WebSocketFrame.ClosingStatusCode.PolicyViolation, "No longer a connection of this node");

                        var itsOwnName = NetworkingNode_Id.Parse($"connection-{id}");

                        inboundCSMSNames.TryRemove(id, out var names);

                        foreach (var (version, server) in inboundServers.ToArray())
                        {

                            if (NodeOrNoneOf(version) is not { } node)
                                continue;

                            foreach (var name in (names ?? []).Append(itsOwnName))
                            {
                                if (server.NoLongerReachAs(name) &&
                                    node.Routing.LookupNetworkingNode(name, out var reachability) &&
                                    ReferenceEquals(reachability.Server, server))
                                {
                                    node.Routing.RemoveStaticRouting(name);
                                }
                            }

                        }

                    }
                }

            }
            catch (Exception e)
            {
                Log.Warning($"Who may dial this {NodeWording.Noun} could not be brought up to date: {e.Message}", "ocpp", "connections", "inbound");
            }

        }

        #endregion

        #region (private) IsUp(Connection) / WaitFor(Connection)

        private Boolean IsUp(ConnectionEntry Connection)
            => inboundConnections.ContainsKey(Connection.Id);

        /// <summary>
        /// Say of an inbound connection that nothing is in over it: waited
        /// for, or why nothing can come.
        /// </summary>
        protected void WaitFor(ConnectionEntry Connection)
        {

            if (IsUp(Connection))
                return;

            var what = !Connection.AutoConnect
                           ? $"'{Connection.InboundName}' is not let in: this connection is switched off."
                           : !InboundServed(Connection.OCPPVersion)
                                 ? $"'{Connection.InboundName}' can not dial in: this {NodeWording.Noun} serves no {ConnectionEntry.AsText(Connection.OCPPVersion)} to whoever dials it."
                                 : $"Waiting for '{Connection.InboundName}' to dial in.";

            lock (dialled)
            {

                // Said once, and not again at every change of something else.
                if (dialled.TryGetValue(Connection.Id, out var state) &&
                    state.Status == ConnectionStatus.Waiting &&
                    state.Said   == what)
                {
                    return;
                }

            }

            Record(Connection, ConnectionStatus.Waiting, what);

            Log.Info($"'{Connection.Description}': {what}", "ocpp", "connections", "inbound");

        }

        #endregion

        #region (private) RouteInbound(Connection, Name, WebSocketConnection) / UnrouteInbound(Connection, Server)

        /// <summary>
        /// Reach an inbound connection under the name every connection has in
        /// the routing of its node - one of its own - and the primary under
        /// the name of the CSMS as well: both names of what came in, beside
        /// the one it came under, which the node routes to the server itself.
        /// </summary>
        private void RouteInbound(ConnectionEntry            Connection,
                                  NetworkingNode_Id          Name,
                                  WebSocketServerConnection  WebSocketConnection)
        {

            var node = NodeOf(Connection.OCPPVersion);

            if (!inboundServers.TryGetValue(Connection.OCPPVersion, out var server))
                return;

            inboundConnections[Connection.Id] = WebSocketConnection;

            server.ReachAs(NextHopOf(Connection), Name);

            node.Routing.RemoveStaticRouting     (NextHopOf(Connection));
            node.Routing.AddOrUpdateStaticRouting(NextHopOf(Connection), server);

            var csmsNames = CSMSNamesOf(Connection);

            if (csmsNames.Length > 0)
            {

                foreach (var csmsName in csmsNames)
                {
                    server.ReachAs(csmsName, Name);
                    node.Routing.RemoveStaticRouting     (csmsName);
                    node.Routing.AddOrUpdateStaticRouting(csmsName, server);
                }

                inboundCSMSNames[Connection.Id] = csmsNames;

            }

        }

        /// <summary>
        /// Take away the names an inbound connection was reached under: its
        /// own, the CSMS's where it had it, and the one it came under - where
        /// that still leads to the given server.
        /// </summary>
        private void UnrouteInbound(ConnectionEntry      Connection,
                                    OCPPWebSocketServer  Server)
        {

            var node = NodeOrNoneOf(Connection.OCPPVersion);

            if (node is null)
                return;

            Server.NoLongerReachAs(NextHopOf(Connection));

            node.Routing.RemoveStaticRouting(NextHopOf(Connection));

            if (inboundCSMSNames.TryRemove(Connection.Id, out var csmsNames))
            {
                foreach (var csmsName in csmsNames)
                {

                    Server.NoLongerReachAs(csmsName);

                    // Where it still leads here, and not to a connection that
                    // took the name since.
                    if (node.Routing.LookupNetworkingNode(csmsName, out var toTheCSMS) &&
                        ReferenceEquals(toTheCSMS.Server, Server))
                    {
                        node.Routing.RemoveStaticRouting(csmsName);
                    }

                }
            }

            if (Connection.InboundName is String itsName &&
                NetworkingNode_Id.TryParse(itsName, out var name) &&
                node.Routing.LookupNetworkingNode(name, out var reachability) &&
                ReferenceEquals(reachability.Server, Server))
            {
                node.Routing.RemoveStaticRouting(name);
            }

        }

        #endregion

        #region (private) InboundIsDown(Version, Name, WebSocketConnection, Why)

        /// <summary>
        /// A connection somebody dialled is gone: where it is the one its
        /// inbound connection was in over - and not one that a newer one of
        /// the same name replaced - its names go with it, and it is waited for
        /// again.
        /// </summary>
        private void InboundIsDown(OCPPVersion                Version,
                                   NetworkingNode_Id          Name,
                                   WebSocketServerConnection  WebSocketConnection,
                                   String                     Why)
        {

            var entry = Connections.Connections.FirstOrDefault(one => one.Inbound &&
                                                                      one.OCPPVersion == Version &&
                                                                      one.InboundName == Name.ToString());

            if (entry is null)
                return;

            if (!inboundConnections.TryRemove(KeyValuePair.Create(entry.Id, WebSocketConnection)))
                return;

            if (inboundServers.TryGetValue(Version, out var server))
                UnrouteInbound(entry, server);

            var what = $"'{Name}' went away ({Why}); it is let in again when it dials.";

            Record(entry, ConnectionStatus.Waiting, what);

            Log.Warning($"'{entry.Description}': {what}", "ocpp", "connections", "inbound");

        }

        #endregion

    }

}
