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

using System.Text;
using System.Collections.Concurrent;
using System.Security.Cryptography;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;
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
    /// <para>
    /// A kind that has a server already, for others - a local controller, for
    /// its charging stations - hands over that one, as shared: who signs in
    /// there is the kind's to decide, and it asks at its own door whether
    /// somebody is a connection written down here (<see
    /// cref="SignInInbound"/>). What is let in that way is the line up of the
    /// kind, not one of the others: <see cref="IsInbound"/>.
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

        /// <summary>
        /// The versions of OCPP whose server is not this node's alone: one the
        /// kind lets others in over as well, and whose sign-ins it decides
        /// itself.
        /// </summary>
        private readonly ConcurrentDictionary<OCPPVersion, Boolean>                   inboundServersShared  = new ();

        /// <summary>
        /// The servers whose events are followed already, by the version they
        /// are followed for: a server served again - stopped and started, as a
        /// kind does that keeps one server - is not followed twice.
        /// </summary>
        private readonly List<(OCPPVersion Version, OCPPWebSocketServer Server)>      inboundServersWired   = [];

        private          Int32                                                        followsInboundChanges;

        /// <summary>
        /// What a connection on a shared server carries that was let in as an
        /// inbound connection: the id of its entry.
        /// </summary>
        public const     String                                                       InboundConnectionKey  = "inboundConnection";

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
        /// <param name="SharedWithOthers">
        /// Whether the server lets others in as well, and the kind decides who
        /// signs in: then the server is told no credentials, the kind asks
        /// <see cref="SignInInbound"/> at its own door, and only what that let
        /// in is a connection of this node.
        /// </param>
        protected void ServeInbound(OCPPVersion          Version,
                                    OCPPWebSocketServer  Server,
                                    Boolean              SharedWithOthers = false)
        {

            inboundServersShared[Version]  = SharedWithOthers;
            inboundServers      [Version]  = Server;

            lock (inboundServersWired)
            {

                if (inboundServersWired.Any(wired => wired.Version == Version && ReferenceEquals(wired.Server, Server)))
                {
                    FollowInboundConnections();
                    return;
                }

                inboundServersWired.Add((Version, Server));

            }

            // Routed under its own names before the 101 goes out, as the node
            // routes the name it came under: what is said to it right after it
            // connected finds it.
            Server.OnNetworkingNodeWebSocketConnectionAccepted += (timestamp, server, connection, networkingNodeId, networkingMode, sharedSubprotocols, eventTrackingId, cancellationToken) => {

                if (ReferenceEquals(inboundServers.GetValueOrDefault(Version), Server) &&
                    WasLetInAsInbound(Version, connection) &&
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

                // One of the others of a shared server: the kind's, not this node's.
                if (!WasLetInAsInbound(Version, connection))
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

            // The logins of a shared server are the kind's, and none of them
            // was put there from here.
            if (inboundServersShared.TryRemove(Version, out var shared) && shared)
                return;

            foreach (var login in server.ClientLogins.Keys.ToArray())
                server.RemoveHTTPBasicAuth(login);

            server.ClientTOTPConfig.Clear();

        }

        #endregion

        #region (protected) SignInInbound(Version, Connection) / IsInbound(Connection)

        /// <summary>
        /// What became of somebody who asked to be let in as an inbound
        /// connection.
        /// </summary>
        /// <param name="LetIn">Whether it is let in.</param>
        /// <param name="Name">The name it came under.</param>
        /// <param name="Why">How it signed in, or why it is turned away: for the log, not for the other end.</param>
        public sealed record InboundSignIn(Boolean  LetIn,
                                           String   Name,
                                           String   Why);

        /// <summary>
        /// At the door of a server shared with others: whether what asks to be
        /// let in comes under the name of an inbound connection of the given
        /// version of OCPP, and then whether it is let in - by the password or
        /// the one-time password its connection names. Null where it comes
        /// under no such name, or nothing is served: then it is one of the
        /// others, and the kind's to decide.
        /// </summary>
        /// <remarks>
        /// The name decides whose it is, before any credentials are looked at:
        /// what comes under the name of a connection written down here is let
        /// in as that or not at all, whatever else the kind would have taken
        /// it for.
        /// </remarks>
        /// <param name="Version">The version of OCPP the server speaks.</param>
        /// <param name="Connection">The connection, with the request that asks for the WebSocket.</param>
        protected InboundSignIn? SignInInbound(OCPPVersion                Version,
                                               WebSocketServerConnection  Connection)
        {

            if (!inboundServers.ContainsKey(Version) ||
                Connection.HTTPRequest is not HTTPRequest request)
            {
                return null;
            }

            // Percent-decoded, segment by segment, by the time it is here.
            var path  = request.Path.ToString();
            var name  = path[(path.LastIndexOf('/') + 1)..];

            var entry = Connections.Connections.FirstOrDefault(entry => entry.Inbound &&
                                                                        entry.OCPPVersion == Version &&
                                                                        entry.InboundName == name);

            if (entry is null)
                return null;

            var credentials  = entry.AuthenticationId is String authenticationId
                                   ? Connections.Authentications.FirstOrDefault(one => one.Id == authenticationId)
                                   : null;

            var password     = credentials is { Kind: AuthenticationKind.Basic, Password.Length: > 0 }
                                   ? credentials.Password
                                   : null;

            var totp         = credentials is { Kind: AuthenticationKind.TOTP, TLSChannelBinding: false }
                                   ? credentials.ToTOTPConfig()
                                   : null;

            // Looked at whether or not it may come in at all, so that a name
            // that is switched off does not answer faster than one that is on.
            var right        = request.Authorization switch {

                                   HTTPBasicAuthentication basic
                                       => basic.Username == name &&
                                          (password is not null
                                               ? CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(basic.Password),
                                                                                         Encoding.UTF8.GetBytes(password))
                                               // What a client that knows nothing but Basic has of a
                                               // TOTP secret: the one-time password, as its password.
                                               : totp is not null &&
                                                 AWebSocketEndpoint.TOTPIsTheOneOf(totp, basic.Password, TOTPHTTPHeaderType.RAW, null)),

                                   HTTPTOTPAuthentication oneTime
                                       => oneTime.Login == name &&
                                          totp is not null &&
                                          AWebSocketEndpoint.TOTPIsTheOneOf(totp, oneTime.TOTP, oneTime.Type, Connection.TOTPTLSExporterMaterial),

                                   null
                                       => request.TOTP is TOTPHTTPHeader header &&
                                          totp is not null &&
                                          AWebSocketEndpoint.TOTPIsTheOneOf(totp, header.Value, header.Type, Connection.TOTPTLSExporterMaterial),

                                   _   => false

                               };

            if (!entry.AutoConnect)
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, a connection that is switched off");

            if (credentials is null || credentials.Login != name || (password is null && totp is null))
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, a connection without credentials it could sign in with");

            if (!right)
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, and what it signed in with is not what that connection names");

            Connection.TryAddCustomData(InboundConnectionKey, entry.Id);

            return new InboundSignIn(true, name, password is not null && request.Authorization is HTTPBasicAuthentication
                                                     ? "a password"
                                                     : "a one-time password");

        }

        /// <summary>
        /// Whether the given connection is one somebody dialled who is written
        /// down as a connection of this node - its CSMS, the controller above
        /// it - and so a line up of this node, although this node accepted it.
        /// </summary>
        /// <param name="Connection">A connection something came over.</param>
        protected Boolean IsInbound(IMessageConnection? Connection)

            => Connection is WebSocketServerConnection webSocketConnection &&
               inboundConnections.Values.Any(itsConnection => ReferenceEquals(itsConnection, webSocketConnection));

        /// <summary>
        /// Whether what is sent to the given name goes over a connection
        /// somebody dialled who is written down as a connection of this node:
        /// the name it came under, the one of its own every connection has,
        /// or the CSMS's where it is the primary.
        /// </summary>
        /// <param name="Name">A name something is sent to.</param>
        protected Boolean LeadsToInbound(NetworkingNode_Id Name)
        {

            foreach (var id in inboundConnections.Keys)
            {

                if (inboundCSMSNames.TryGetValue(id, out var csmsNames) && csmsNames.Contains(Name))
                    return true;

                if (Connections.Connections.FirstOrDefault(one => one.Id == id) is ConnectionEntry entry &&
                    (NextHopOf(entry) == Name || entry.InboundName == Name.ToString()))
                {
                    return true;
                }

            }

            return false;

        }

        /// <summary>
        /// Whether a connection of a server served for the given version was
        /// let in as an inbound connection: every one of a server of this
        /// node's own, and of a shared one those <see cref="SignInInbound"/>
        /// let in.
        /// </summary>
        private Boolean WasLetInAsInbound(OCPPVersion                Version,
                                          WebSocketServerConnection  Connection)

            => !inboundServersShared.GetValueOrDefault(Version) ||
               Connection.TryGetCustomData(InboundConnectionKey) is String;

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

                    // What a shared server knows of who may come in is the
                    // kind's: it asks at its own door, see SignInInbound.
                    if (inboundServersShared.GetValueOrDefault(version))
                        continue;

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

            // Accepted, and the line up of this node all the same: what comes
            // over it in the plain frame of OCPP is for this node, as over a
            // connection it dialled, see LineUpExtensions.
            WebSocketConnection.IsTheLineUp();

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
