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
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

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
                    UnrouteInbound(entry, server, connection);
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
        /// <param name="ByCertificate">Whether the connection it came under signs in with a client certificate rather than with credentials.</param>
        public sealed record InboundSignIn(Boolean  LetIn,
                                           String   Name,
                                           String   Why,
                                           Boolean  ByCertificate = false);

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

            #region By a client certificate

            // The certificate of the TLS handshake, and nothing else: a
            // password or a one-time password it sends besides opens nothing
            // under this name, and the connection names none.
            if (entry.SignsInWithACertificate)
            {

                var byCertificate = CertificateSignsIn(entry, name, Connection.ClientCertificate, out var how);

                if (!entry.AutoConnect)
                    return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, a connection that is switched off", ByCertificate: true);

                if (entry.AuthenticationId is not null)
                    return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, which names a client certificate and credentials " +
                                                          "as well - one or the other", ByCertificate: true);

                if (!byCertificate)
                    return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, and {how}", ByCertificate: true);

                Connection.TryAddCustomData(InboundConnectionKey, entry.Id);

                return new InboundSignIn(true, name, how, ByCertificate: true);

            }

            #endregion

            var credentials  = entry.AuthenticationId is String authenticationId
                                   ? Connections.Authentications.FirstOrDefault(one => one.Id == authenticationId)
                                   : null;

            var password     = credentials is { Kind: AuthenticationKind.Basic, Password.Length: > 0 }
                                   ? credentials.Password
                                   : null;

            // Bound to the TLS session or not, as the credentials say: a bound
            // one is checked with keying material of the session it came over,
            // which only HTTP/3 exports, and a raw one is not taken instead.
            var totp         = credentials is { Kind: AuthenticationKind.TOTP }
                                   ? credentials.ToTOTPConfig()
                                   : null;

            var bound        = totp is not null && credentials!.TLSChannelBinding;

            var sentType     = request.Authorization switch {
                                   HTTPTOTPAuthentication oneTime  => oneTime.Type,
                                   null                            => request.TOTP?.Type,
                                   _                               => null
                               };

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

            // Said out loud, as the charging stations of a local controller are
            // told: the alternative is a back end whose password is computed
            // right and refused all the same, with nothing in the log to say why.
            if (!right && bound && sentType == TOTPHTTPHeaderType.TLSChannelBinding && Connection.TOTPTLSExporterMaterial is not { Length: > 0 })
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, and it sent a one-time password bound to the TLS session " +
                                                      "over a connection whose TLS exports nothing to check it with: that is checked over HTTP/3 only");

            if (!right && bound && sentType != TOTPHTTPHeaderType.TLSChannelBinding)
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, whose one-time password is bound to the TLS session, " +
                                                      "and it sent one that is not: it has to open its WebSocket over HTTP/3 and bind its one-time password");

            if (!right)
                return new InboundSignIn(false, name, $"'{name}' is the name '{entry.Description}' comes under, and what it signed in with is not what that connection names");

            Connection.TryAddCustomData(InboundConnectionKey, entry.Id);

            return new InboundSignIn(true, name, password is not null && request.Authorization is HTTPBasicAuthentication
                                                     ? "a password"
                                                     : bound
                                                           ? "a one-time password bound to its TLS session"
                                                           : "a one-time password");

        }

        #region (private) CertificateSignsIn(Entry, Name, Certificate, out How)

        /// <summary>
        /// Whether the client certificate of a connection signs it in as the
        /// given inbound connection: by the key pin of its key, where that is
        /// what the connection names, or by its CA - its chain judged as this
        /// node judges the certificate of a server, held to what the
        /// connection names, and its common name the name it came under.
        /// </summary>
        /// <param name="Entry">The inbound connection, which signs in with a certificate.</param>
        /// <param name="Name">The name it came under.</param>
        /// <param name="Certificate">The client certificate of its TLS handshake, or null.</param>
        /// <param name="How">How it signed in, or why not: for the log.</param>
        private Boolean CertificateSignsIn(ConnectionEntry    Entry,
                                           String             Name,
                                           X509Certificate2?  Certificate,
                                           out String         How)
        {

            if (Certificate is null)
            {
                How = "it showed no client certificate, which is what that connection signs in with";
                return false;
            }

            if (Entry.ClientKeyPins.Count > 0)
            {

                var validation = KeyPinValidator.Validate(new KeyPinPolicy(Entry.ClientKeyPins), Certificate);

                How = validation.IsAccepted
                          ? $"a client certificate, by its key pin {validation.Pin}"
                          : $"its client certificate was refused: {validation.Reason ?? "its key is none of the pinned ones"}";

                return validation.IsAccepted;

            }

            if (Entry.ClientPins is not ServerPins pins)
            {
                How = "that connection names nothing its client certificate is held to";
                return false;
            }

            var kept = ChainKeptFor(Certificate);

            try
            {

                var judgement = JudgeClient(Entry, Name, pins, Certificate, kept, null);

                How = judgement.Accepted
                          ? $"a client certificate{(judgement.AnchoredBy is String label ? $" of '{label}'" : "")}, issued to '{Name}'"
                          : $"its client certificate was refused: {judgement.Why}";

                return judgement.Accepted;

            }
            finally
            {
                // The chain does not dispose what was loaded into it.
                if (kept is not null)
                {
                    foreach (var one in kept.ChainPolicy.ExtraStore)
                        one.Dispose();
                    kept.Dispose();
                }
            }

        }

        #endregion

        #region (protected) InboundCertificateCheck(Version, Certificate, Chain, PolicyErrors)

        /// <summary>
        /// For the TLS validator of an OCPP server this node lets in over:
        /// whether a client certificate shown at the handshake is one an
        /// inbound connection of the version that server speaks signs in with,
        /// and then whether it passes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Null where no inbound connection claims it: a key pin of none, and
        /// a common name no connection checked by the CA of its other end
        /// comes under. Then it is one of the others of the server - a
        /// charging station of a local controller -, and the validator's to
        /// decide as before. Success where it passes for one of them, and the
        /// refusal, with what was wrong, where it claims to be one and does
        /// not pass: a certificate that names a connection is let in as that
        /// or not at all.
        /// </para>
        /// <para>
        /// Asked again when the connection signs in, by the name it then comes
        /// under (<see cref="SignInInbound"/>): the handshake knows no name
        /// yet. The certificates the other end sent beside its own are kept
        /// for that, for a little while.
        /// </para>
        /// <para>
        /// Only the connections of the given version claim it: the name of an
        /// inbound connection is its own within its version alone, and a
        /// server that takes the common name of a certificate the handshake
        /// accepted for who it is - one not shared with others - asks nothing
        /// after this. The certificate of a connection of OCPP 1.6 lets nobody
        /// in over OCPP 2.1 under the same name.
        /// </para>
        /// </remarks>
        /// <param name="Version">The version of OCPP the server of the handshake speaks.</param>
        /// <param name="Certificate">The client certificate of the handshake, or null.</param>
        /// <param name="Chain">The chain the machine built for it, with the certificates the other end sent beside it.</param>
        /// <param name="PolicyErrors">What building that chain found.</param>
        protected TLSValidationResult? InboundCertificateCheck(OCPPVersion        Version,
                                                               X509Certificate2?  Certificate,
                                                               X509Chain?         Chain,
                                                               SslPolicyErrors    PolicyErrors)
        {

            if (Certificate is null)
                return null;

            var claimants = CertificateClaimantsOf(Version);

            if (claimants.Length == 0)
                return null;

            #region By its key pin

            var keyPin = KeyPin.FromCertificate(Certificate);

            foreach (var entry in claimants.Where(entry => entry.ClientKeyPins.Contains(keyPin)))
            {

                // Hermod's validator rather than the comparison above: it also
                // refuses a key that cannot be one - a malformed key, an unknown
                // algorithm, a point that is not on its curve.
                var validation = KeyPinValidator.Validate(new KeyPinPolicy(entry.ClientKeyPins), Certificate);

                if (validation.IsAccepted)
                    Log.Debug  ($"The client certificate of '{Certificate.Subject}' was accepted by its key pin {keyPin}, which '{entry.Description}' signs in with.",
                                "ocpp", "connections", "inbound", "tls");
                else
                    Log.Warning($"The client certificate of '{Certificate.Subject}' was refused: {validation.Reason}",
                                "ocpp", "connections", "inbound", "tls");

                return validation.IsAccepted
                           ? TLSValidationResult.Success()
                           : TLSValidationResult.Failed(validation.Reason ?? "The key was refused.");

            }

            #endregion

            #region By its CA, under the name it is issued to

            var commonName = CommonNameOf(Certificate);

            var byCA       = claimants.Where(entry => entry.ClientPins is not null &&
                                                      commonName is not null &&
                                                      entry.InboundName == commonName).
                                       ToArray();

            if (byCA.Length == 0)
                return null;

            var whyNot = new List<String>();

            foreach (var entry in byCA)
            {

                var judgement = JudgeClient(entry, commonName!, entry.ClientPins!, Certificate, Chain, PolicyErrors);

                if (judgement.Accepted)
                {

                    KeepChain(Certificate, Chain);

                    Log.Debug($"The client certificate of '{Certificate.Subject}' was accepted: '{entry.Description}' signs in with a certificate issued to " +
                              $"'{commonName}'{(judgement.AnchoredBy is String label ? $", of '{label}'" : "")}.",
                              "ocpp", "connections", "inbound", "tls");

                    return TLSValidationResult.Success();

                }

                whyNot.Add(judgement.Why);

            }

            var refused = String.Join(" ", whyNot.Distinct());

            Log.Warning($"The client certificate of '{Certificate.Subject}' was refused, which is issued to '{commonName}', the name of a connection the other end makes: {refused}",
                        "ocpp", "connections", "inbound", "tls");

            return TLSValidationResult.Failed(refused);

            #endregion

        }

        #endregion

        #region (protected) IsInboundKeyPinned(Version, Certificate)

        /// <summary>
        /// Whether the given client certificate holds a key an inbound
        /// connection of the given version that is switched on pins: then its
        /// key is all that was checked, and what is written around it - its
        /// common name - names nobody. For the AcceptedByKeyPin of the OCPP
        /// server of that version it comes in over.
        /// </summary>
        /// <param name="Version">The version of OCPP the server speaks.</param>
        /// <param name="Certificate">A client certificate.</param>
        protected Boolean IsInboundKeyPinned(OCPPVersion       Version,
                                             X509Certificate2  Certificate)
        {

            var keyPin = KeyPin.FromCertificate(Certificate);

            return CertificateClaimantsOf(Version).Any(entry => entry.ClientKeyPins.Contains(keyPin));

        }

        #endregion

        #region (protected) IsAnInboundCertificate(Version, Certificate)

        /// <summary>
        /// Whether the given client certificate is one an inbound connection
        /// of the given version that is switched on claims: its key is one it
        /// pins, or it is issued to the name of one checked by the CA of its
        /// other end. Whether it passes is not asked here - see
        /// <see cref="InboundCertificateCheck"/>.
        /// </summary>
        /// <remarks>
        /// For the door of a server shared with others: a certificate that got
        /// through the handshake as the one of an inbound connection has not
        /// been held to anything the others are held to, and is to be judged
        /// as theirs before it lets anybody in as one of them.
        /// </remarks>
        /// <param name="Version">The version of OCPP the server speaks.</param>
        /// <param name="Certificate">A client certificate.</param>
        protected Boolean IsAnInboundCertificate(OCPPVersion       Version,
                                                 X509Certificate2  Certificate)
        {

            var keyPin      = KeyPin.FromCertificate(Certificate);
            var commonName  = CommonNameOf(Certificate);

            return CertificateClaimantsOf(Version).Any(entry => entry.ClientKeyPins.Contains(keyPin) ||
                                                                (entry.ClientPins is not null && commonName is not null && entry.InboundName == commonName));

        }

        #endregion

        #region (protected) InboundCertificateHint(Version, Certificate, Chain, PolicyErrors)

        /// <summary>
        /// For the log of a kind that refused a client certificate no inbound
        /// connection claimed: whether it is one of the CA an inbound
        /// connection of the given version checks the certificate of its other
        /// end by, issued to another name than that connection comes under -
        /// the certificate of that back end with a common name that was
        /// mistyped, or one it was not issued for. Null where it is not.
        /// </summary>
        /// <remarks>
        /// Only for a refusal: a charging station's certificate of the same CA
        /// is one that passes as a station's, and nothing to say anything of.
        /// </remarks>
        /// <param name="Version">The version of OCPP the server of the handshake speaks.</param>
        /// <param name="Certificate">The client certificate of the handshake, or null.</param>
        /// <param name="Chain">The chain the machine built for it, with the certificates the other end sent beside it.</param>
        /// <param name="PolicyErrors">What building that chain found.</param>
        protected String? InboundCertificateHint(OCPPVersion        Version,
                                                 X509Certificate2?  Certificate,
                                                 X509Chain?         Chain,
                                                 SslPolicyErrors    PolicyErrors)
        {

            if (Certificate is null)
                return null;

            var commonName = CommonNameOf(Certificate);

            foreach (var entry in CertificateClaimantsOf(Version).Where(entry => entry.ClientPins is not null &&
                                                                                 entry.InboundName is not null &&
                                                                                 entry.InboundName != commonName))
            {

                var judgement = JudgeClient(entry, entry.InboundName!, entry.ClientPins!, Certificate, Chain, PolicyErrors, ByItsName: false);

                if (judgement.Accepted)
                    return $"It leads to the roots '{entry.Description}' holds the certificate of the back end that dials to, which comes under '{entry.InboundName}', " +
                           $"and is issued to '{commonName ?? Certificate.Subject}': where it is that back end's, it has to be issued to '{entry.InboundName}'.";

            }

            return null;

        }

        #endregion

        #region (private) CertificateClaimantsOf(Version)

        /// <summary>
        /// The inbound connections of the given version that are switched on
        /// and sign in with a client certificate over TLS - none where that
        /// version is not served.
        /// </summary>
        /// <param name="Version">The version of OCPP of a server.</param>
        private ConnectionEntry[] CertificateClaimantsOf(OCPPVersion Version)

            => inboundServers.ContainsKey(Version)
                   ? Connections.Connections.Where(entry => entry.Inbound &&
                                                            entry.OCPPVersion == Version &&
                                                            entry.AutoConnect &&
                                                            entry.IsSecure &&
                                                            entry.SignsInWithACertificate).
                                             ToArray()
                   : [];

        #endregion

        #region (private) JudgeClient(...) / CommonNameOf(...) / KeepChain(...) / ChainKeptFor(...)

        /// <summary>
        /// What became of a client certificate held to what an inbound
        /// connection names.
        /// </summary>
        private sealed record ClientJudgement(Boolean  Accepted,
                                              String   Why,
                                              String?  AnchoredBy);

        /// <summary>
        /// The client certificate of an inbound connection checked by its CA:
        /// issued to the name the connection comes under, by its common name,
        /// and its chain judged as this node judges the chain of a server -
        /// held to the roots or the group of roots the connection names, and
        /// asked whether it is revoked. Only asked: nothing is learned,
        /// remembered or logged as the judgement of a server.
        /// </summary>
        /// <param name="Entry">The inbound connection.</param>
        /// <param name="Name">The name it comes under.</param>
        /// <param name="Pins">What its certificate is held to.</param>
        /// <param name="Certificate">The certificate.</param>
        /// <param name="Chain">The chain built for it at the handshake, or one built with the certificates kept from there; null to build one here.</param>
        /// <param name="PolicyErrors">What building the chain at the handshake found; null where it is built here.</param>
        /// <param name="ByItsName">Whether its common name has to be the name: false to judge its chain alone.</param>
        private ClientJudgement JudgeClient(ConnectionEntry    Entry,
                                            String             Name,
                                            ServerPins         Pins,
                                            X509Certificate2   Certificate,
                                            X509Chain?         Chain,
                                            SslPolicyErrors?   PolicyErrors,
                                            Boolean            ByItsName = true)
        {

            if (ByItsName && CommonNameOf(Certificate) != Name)
                return new ClientJudgement(false, $"it is issued to '{CommonNameOf(Certificate) ?? Certificate.Subject}', and '{Entry.Description}' comes under '{Name}'.", null);

            // The CAs between a root and the certificate that this node knows
            // already, for an other end that shows its certificate alone: they
            // help to build the chain, beside what was sent along, and are
            // believed nothing - the chain still has to end where the
            // connection says.
            var known         = KnownIssuingCAs();

            // What .NET judges a client certificate by - a chain to a root this
            // machine trusts - and nothing about a name: the name was asked above.
            var built         = Chain;
            var ownChain      = Chain is null || PolicyErrors is null || known.Count > 0;
            var policyErrors  = PolicyErrors ?? SslPolicyErrors.None;

            try
            {

                if (ownChain)
                {

                    built = new X509Chain();
                    built.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    if (Chain is not null)
                        built.ChainPolicy.ExtraStore.AddRange(Chain.ChainPolicy.ExtraStore);

                    foreach (var issuingCA in known)
                        built.ChainPolicy.ExtraStore.Add(issuingCA);

                    policyErrors = built.Build(Certificate)
                                       ? SslPolicyErrors.None
                                       : SslPolicyErrors.RemoteCertificateChainErrors;

                }

                var judgement = JudgeServer(ConnectionsCertificateUsage,
                                            Name,
                                            Pins,
                                            Certificate,
                                            built,
                                            policyErrors & ~SslPolicyErrors.RemoteCertificateNameMismatch,
                                            Evidence:  false,
                                            Learn:     null,
                                            Remember:  false);

                return new ClientJudgement(judgement.Accepted,
                                           judgement.Accepted ? "" : WhyRefused(judgement),
                                           judgement.Accepted ? judgement.AnchoredBy : null);

            }
            finally
            {

                if (ownChain && !ReferenceEquals(built, Chain))
                    built?.Dispose();

                foreach (var issuingCA in known)
                    issuingCA.Dispose();

            }

        }

        /// <summary>
        /// The issuing CAs the certificate store of this node keeps to build
        /// chains with (<see cref="CertificateKind.TLSIntermediate"/>), as far
        /// as they can be used now. Whoever asks disposes of them.
        /// </summary>
        private List<X509Certificate2> KnownIssuingCAs()
        {

            var known = new List<X509Certificate2>();

            try
            {
                foreach (var entry in Certificates.UsableByKind(CertificateKind.TLSIntermediate))
                    if (Certificates.TryLoad(entry, out var issuingCA, out _))
                        known.Add(issuingCA);
            }
            catch (Exception e)
            {
                Log.Warning($"The issuing CAs of the certificate store could not be read: {e.Message}", "ocpp", "connections", "inbound", "tls");
            }

            return known;

        }

        /// <summary>
        /// The common name a certificate is issued to, or null.
        /// </summary>
        private static String? CommonNameOf(X509Certificate2 Certificate)
        {
            var name = Certificate.GetNameInfo(X509NameType.SimpleName, forIssuer: false);
            return String.IsNullOrEmpty(name) ? null : name;
        }

        /// <summary>
        /// The certificates sent beside client certificates that passed at the
        /// handshake, by the thumbprint of the client certificate: what its
        /// chain is built with again when it signs in. Kept for
        /// <see cref="ChainKeptFor"/> a little while, and only so many.
        /// </summary>
        private readonly ConcurrentDictionary<String, (Byte[][] Certificates, DateTimeOffset Kept)>  inboundChains = new ();

        private static readonly TimeSpan  InboundChainsKeptFor  = TimeSpan.FromMinutes(2);
        private const           Int32     MaxInboundChainsKept  = 64;

        private void KeepChain(X509Certificate2  Certificate,
                               X509Chain?        Chain)
        {

            var now = TimeProvider.GetUtcNow();

            // Those kept too long, and then the oldest while there are too many:
            // not whichever comes first, which may be one just kept for a
            // connection that has yet to sign in.
            foreach (var (thumbprint, kept) in inboundChains.ToArray().
                                                             Where  (one => now - one.Value.Kept > InboundChainsKeptFor))
                inboundChains.TryRemove(thumbprint, out _);

            foreach (var (thumbprint, _) in inboundChains.ToArray().
                                                       OrderBy(one => one.Value.Kept).
                                                       Take   (Math.Max(0, inboundChains.Count - MaxInboundChainsKept + 1)))
                inboundChains.TryRemove(thumbprint, out _);

            var certificates = new List<Byte[]>();

            if (Chain is not null)
            {

                foreach (var one in Chain.ChainPolicy.ExtraStore)
                    certificates.Add(one.RawData);

                foreach (var element in Chain.ChainElements.Skip(1))
                    certificates.Add(element.Certificate.RawData);

            }

            inboundChains[Certificate.Thumbprint] = ([.. certificates], now);

        }

        /// <summary>
        /// A chain for the given client certificate, with what was sent beside
        /// it at its handshake where that is still kept - or null.
        /// </summary>
        private X509Chain? ChainKeptFor(X509Certificate2 Certificate)
        {

            if (!inboundChains.TryGetValue(Certificate.Thumbprint, out var kept) ||
                TimeProvider.GetUtcNow() - kept.Kept > InboundChainsKeptFor)
            {
                return null;
            }

            var chain = new X509Chain();

            foreach (var raw in kept.Certificates)
                chain.ChainPolicy.ExtraStore.Add(X509CertificateLoader.LoadCertificate(raw));

            return chain;

        }

        #endregion

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

                        // By its client certificate, which the TLS handshake
                        // asks for and nothing a server holds of logins does:
                        // see InboundCertificateCheck. Never with credentials
                        // beside it.
                        if (name is not null &&
                            entry.AutoConnect &&
                            entry.IsSecure &&
                            entry.SignsInWithACertificate &&
                            entry.AuthenticationId is null)
                        {
                            letIn.Add(entry.Id);
                        }

                        else if (name is not null &&
                            entry.AutoConnect &&
                            !entry.SignsInWithACertificate &&
                            entry.AuthenticationId is not null &&
                            authentications.TryGetValue(entry.AuthenticationId, out var credentials) &&
                            credentials.Login == name)
                        {

                            if (credentials.Kind == AuthenticationKind.Basic && !String.IsNullOrEmpty(credentials.Password))
                            {
                                passwords[name] = credentials.Password;
                                letIn.Add(entry.Id);
                            }

                            // Bound to the TLS session or not: a bound one is
                            // checked with the keying material of the session
                            // it came over, which HTTP/3 exports, and refused
                            // over a connection that exports none.
                            else if (credentials.Kind == AuthenticationKind.TOTP &&
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

                                UnrouteInbound(entry, server, connection);

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

                            foreach (var name in (names ?? []).Append(itsOwnName).Concat(connection?.NamesLearned() ?? []))
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

            var server = inboundServers.GetValueOrDefault(Connection.OCPPVersion);

            var what = !Connection.AutoConnect
                           ? $"'{Connection.InboundName}' is not let in: this connection is switched off."
                           : server is null
                                 ? $"'{Connection.InboundName}' can not dial in: this {NodeWording.Noun} serves no {ConnectionEntry.AsText(Connection.OCPPVersion)} to whoever dials it."
                                 : WhyItCanNotSignIn(Connection, server) is String why
                                       ? $"'{Connection.InboundName}' can not sign in: {why}"
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

        /// <summary>
        /// Why what an inbound connection signs in with can not come over the
        /// server it dials, as that server is now - or null.
        /// </summary>
        /// <param name="Connection">An inbound connection that is switched on.</param>
        /// <param name="Server">The server of its version of OCPP.</param>
        private String? WhyItCanNotSignIn(ConnectionEntry      Connection,
                                          OCPPWebSocketServer  Server)
        {

            // A server asks for a client certificate where it requires one or
            // has a validator to judge it; without either, the handshake asks
            // for none, and none is ever shown.
            if (Connection.SignsInWithACertificate &&
                !Server.ClientCertificateRequired &&
                Server.ClientCertificateValidator is null)
            {
                return $"the server it dials asks for no client certificate, which is what this connection signs in with.";
            }

            // Bound to the TLS session: checked with keying material only the
            // TLS of HTTP/3 exports.
            if (!Server.AcceptHTTP3 &&
                Connection.AuthenticationId is String id &&
                Connections.Authentications.FirstOrDefault(credentials => credentials.Id == id) is { Kind: AuthenticationKind.TOTP, TLSChannelBinding: true })
            {
                return $"its one-time password is bound to its TLS session, which is checked over HTTP/3 only, and the server it dials takes no WebSockets over HTTP/3.";
            }

            return null;

        }

        #endregion

        #region (private) RouteInbound(Connection, Name, WebSocketConnection) / UnrouteInbound(Connection, Server, WebSocketConnection)

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
        /// own, the CSMS's where it had it, those learned from what its far end
        /// asked (see LineUpExtensions.IsReachedAsWell), and the one it came
        /// under - where that still leads to the given server.
        /// </summary>
        private void UnrouteInbound(ConnectionEntry            Connection,
                                    OCPPWebSocketServer        Server,
                                    WebSocketServerConnection  WebSocketConnection)
        {

            var node = NodeOrNoneOf(Connection.OCPPVersion);

            if (node is null)
                return;

            Server.NoLongerReachAs(NextHopOf(Connection));

            node.Routing.RemoveStaticRouting(NextHopOf(Connection));

            // Learned from the identification its far end wrote into what it
            // asked: where it is still this connection's, and its route still
            // leads here.
            if (Connection.InboundName is String cameUnder &&
                NetworkingNode_Id.TryParse(cameUnder, out var cameAs))
            {
                foreach (var learned in WebSocketConnection.NamesLearned())
                {

                    if (Server.ReachedAs(learned) == cameAs)
                        Server.NoLongerReachAs(learned);

                    if (node.Routing.LookupNetworkingNode(learned, out var toIt) &&
                        ReferenceEquals(toIt.Server, Server))
                    {
                        node.Routing.RemoveStaticRouting(learned);
                    }

                }
            }

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
                UnrouteInbound(entry, server, WebSocketConnection);

            var what = $"'{Name}' went away ({Why}); it is let in again when it dials.";

            Record(entry, ConnectionStatus.Waiting, what);

            Log.Warning($"'{entry.Description}': {what}", "ocpp", "connections", "inbound");

        }

        #endregion

    }

}
