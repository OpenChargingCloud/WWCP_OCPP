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

using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.SSH;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.OCPP.SSH;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

using OCPPWebSockets = cloud.charging.open.protocols.OCPP.WebSockets;
using OCPPVersion    = cloud.charging.open.protocols.WWCP.Node.Connections.OCPPVersion;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The HTTP WebSocket server the charging stations below this node - a
    /// local controller or a gateway - connect to: its socket, the certificate
    /// it presents, who it lets in and what it says about them.
    /// </summary>
    /// <remarks>
    /// <b>A second server and not a second path on the first one.</b> The web
    /// interface and this one face different networks, present different
    /// certificates and let in different callers - a person with a password and
    /// a machine with one. One socket would have made one set of rules out of
    /// two, and the looser of the two would have won.
    ///
    /// <b>What changes while running and what waits for a restart.</b> The
    /// certificate, the accepted chains, the charging station logins and the
    /// logging switches are read afresh whenever they are needed, so a change
    /// to them is in effect by the time the page has reloaded. The socket - the
    /// interface, the port, the OCPP versions offered, the TLS versions, and
    /// whether a client certificate is asked for - is decided when the server
    /// is built, and changing it waits for the next start. Which of the two a
    /// change was is reported back rather than left to be discovered.
    ///
    /// <b>And the CSMS, where it is the one that connects.</b> A CSMS this node
    /// cannot connect to connects to the node, on this same port: written down as a
    /// connection the other end makes, let in under the name its URL ends with
    /// by the credentials it names - and from then on the line up of this
    /// node, not a charging station of it. See
    /// <see cref="ServeTheBackEndsThatConnect"/> and <see cref="StationSignIns"/>.
    ///
    /// <b>And over SSH, where somebody switches it on.</b> The charging
    /// stations may speak OCPP over the SSH server of this node as well,
    /// as the subsystem "ocpp@open.charging.cloud", signing in with an SSH key
    /// of their own - see <see cref="WebSocketServerSSH"/> and
    /// <see cref="LoginSSHSignIns"/>. That is served while the station
    /// server is, and switched on and off without a restart. The subsystem is
    /// the commissioning tools' as well, where an account signs in - see
    /// OCPPServingNode.Commissioning.cs.
    ///
    /// <b>Moved here from the LocalController library</b>, as the connecting
    /// moved into WWCP_Node before it, so that a gateway has the same
    /// server, stores, sections and pages as a local controller - see
    /// OCPPServingNode.cs for what a kind still says of its own.
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region Data

        /// <summary>
        /// What the charging station server is configured as, with every field
        /// filled in.
        /// </summary>
        /// <remarks>
        /// Started at the defaults rather than left to the constructor: these
        /// are filled in by <see cref="BuildOCPPServer"/>, which the
        /// constructor calls, and a controller read before that has happened
        /// should say "switched off" rather than throw.
        /// </remarks>
        protected WebSocketServerConfiguration                    ocppServerSettings  = new WebSocketServerConfiguration().Effective();

        /// <summary>
        /// What it was configured as when it was built, to tell a change that
        /// is in effect from one that is waiting for a restart.
        /// </summary>
        private WebSocketServerConfiguration                    ocppServerAsBuilt   = new WebSocketServerConfiguration().Effective();

        /// <summary>
        /// The server of the station port: the one that listens, of the node of
        /// OCPP 2.1, which hands a WebSocket of OCPP 1.6 to the server of the
        /// node of OCPP 1.6 - see <see cref="ocppWebSocketServer16"/>.
        /// </summary>
        protected OCPPWebSockets.OCPPWebSocketServer?        ocppWebSocketServer;

        /// <summary>
        /// The server of the node of OCPP 1.6 behind the station port: it has
        /// no port of its own, and is handed the WebSockets that ask for OCPP
        /// 1.6 by the server that listens - or null, where OCPP 1.6 is not
        /// offered or this node does not speak it.
        /// </summary>
        protected OCPPWebSockets.OCPPWebSocketServer?        ocppWebSocketServer16;

        /// <summary>
        /// Whether this server is speaking TLS, decided once when it started.
        /// </summary>
        /// <remarks>
        /// Once, and not per connection, although the certificate is chosen per
        /// connection: a port either encrypts or it does not, and a certificate
        /// uploaded while the server runs must not turn half the charging
        /// stations on a site into TLS clients and leave the other half plain.
        /// So a first certificate is a restart, and every one after it is not.
        /// </remarks>
        protected Boolean                                    ocppServerTLS;

        protected Boolean                                    ocppServerStarted;

        /// <summary>
        /// What makes this controller look at the chains it accepts without
        /// being asked.
        /// </summary>
        /// <remarks>
        /// The whole value of naming a replacement weeks early is somebody
        /// being told weeks early. Checking only when a page is opened would
        /// mean the warning arrives when somebody was already looking - which
        /// is the one moment it is not needed. What the station port presents
        /// is looked at by the node - see ListenerCertificates.
        ///
        /// On this controller's own <see cref="TimeProvider"/> rather than a
        /// bare timer, so that a test which moves the clock moves this too.
        /// </remarks>
        private ITimer?                                    certificateCheckTimer;

        /// <summary>
        /// How often the chains are looked at. Daily: what is being watched for
        /// moves at the speed of a calendar.
        /// </summary>
        public static readonly TimeSpan CertificateCheckEvery = TimeSpan.FromHours(24);

        /// <summary>
        /// The listener of the node's certificate store the station port is:
        /// what it presents is a TLS server identity told "ocpp", or the
        /// carrier of the server key of "ocpp".
        /// </summary>
        public const String StationListener = "ocpp";

        /// <summary>
        /// Where a connection keeps which charging station this controller let
        /// in on it.
        /// </summary>
        /// <remarks>
        /// Kept by this controller rather than read from the connection's
        /// Login. The server leaves the checking to <see cref="StationSignIns"/>,
        /// and Hermod sets a Login only where it authenticated the upgrade
        /// itself - so to Hermod every one of these connections is anonymous,
        /// and the log said "'?' is connected" of a station it had just let in.
        /// </remarks>
        protected const String StationIdKey = "webSocketServer.chargingStation";

        /// <summary>
        /// What the door of the station port calls the port, who comes to it
        /// and the ways a client authenticates there - OCPP's security
        /// profiles - in the log and in a refusal.
        /// </summary>
        public static readonly WebSocketSignInWords StationWords = new () {
                                                                       Port            = "the station port",
                                                                       Who             = "A charging station or a back end",
                                                                       PinnedLogin     = "the charging station",
                                                                       Realm           = "OCPP",
                                                                       Tags            = [ "ocpp", "station" ],
                                                                       InboundTags     = [ "ocpp", "csms" ],
                                                                       Authentication  = authentication => $"security profile {authentication.SecurityProfile()}"
                                                                   };

        /// <summary>
        /// The server of OCPP-over-SSH: built with the station server, and
        /// handed to the node's SSH server while it is served.
        /// </summary>
        protected OCPPSshServer?                             ocppSshServer;

        /// <summary>
        /// The server of OCPP 1.6 over SSH, beside the one of OCPP 2.1 on the
        /// same subsystem: a station is handed to the one of the version its
        /// HELLO chose - or null, where OCPP 1.6 is not offered over SSH.
        /// </summary>
        protected OCPPSshServer?                             ocppSshServer16;

        /// <summary>
        /// The charging stations, as logins of the node's SSH server.
        /// </summary>
        private LoginSSHSignIns?                         stationSSHSignIns;

        /// <summary>
        /// Whether OCPP-over-SSH is served right now.
        /// </summary>
        protected volatile Boolean                           ocppOverSSHServed;

        /// <summary>
        /// What OCPP-over-SSH is called in what the SSH server says: who, the
        /// SSH page, the log.
        /// </summary>
        public const String OCPPOverSSHLabel = "OCPP";

        #endregion

        #region Properties

        /// <summary>
        /// What the station port presents to the charging stations, out of the
        /// node's certificate store: the listener "ocpp" - see
        /// <see cref="StationListener"/> - which keeps serving the last
        /// certificate it showed where nothing is valid, rather than lock every
        /// station out.
        /// </summary>
        public ListenerCertificate      StationCertificate    { get; private set; } = default!;

        /// <summary>
        /// Which chains a charging station's own certificate may lead to.
        /// </summary>
        public ClientTrustStore         ClientTrust           { get; private set; } = default!;

        /// <summary>
        /// Which charging stations may sign in, and with what.
        /// </summary>
        public WebSocketLogins          WebSocketLogins       { get; private set; } = default!;

        /// <summary>
        /// Who comes in at the station port: the sign-in checks of its door,
        /// for a charging station and a back end that connects alike.
        /// </summary>
        public WebSocketSignIns         StationSignIns        { get; private set; } = default!;

        /// <summary>
        /// The server itself, or null when this controller was built without
        /// one.
        /// </summary>
        public OCPPWebSockets.OCPPWebSocketServer?  StationServer
            => ocppWebSocketServer;

        /// <summary>
        /// The servers behind the station port: the one that listens, and the
        /// one of OCPP 1.6 it hands WebSockets to, where there is one.
        /// </summary>
        public IEnumerable<OCPPWebSockets.OCPPWebSocketServer>  StationServers
        {
            get
            {

                if (ocppWebSocketServer   is { } server)
                    yield return server;

                if (ocppWebSocketServer16 is { } server16)
                    yield return server16;

            }
        }

        /// <summary>
        /// The OCPP security profiles the station port allows: how a client
        /// may authenticate at it, in OCPP's words.
        /// </summary>
        public IReadOnlyList<Byte> OCPPSecurityProfiles

            => ClientAuthenticationProfiles.SecurityProfiles(
                   ocppServerSettings.ClientAuthentication ?? WebSocketServerConfiguration.DefaultClientAuthentication
               );

        /// <summary>
        /// What the charging station server is configured as.
        /// </summary>
        public WebSocketServerConfiguration  OCPPServerSettings
            => ocppServerSettings;

        /// <summary>
        /// Whether this local controller is meant to listen for charging
        /// stations at all.
        /// </summary>
        public Boolean OCPPServerEnabled
            => ocppServerSettings.Enabled == true;

        /// <summary>
        /// Why the station port takes no WebSockets over HTTP/3 although the
        /// file says it shall, where that is known before it is tried.
        /// </summary>
        public const String NoHTTP3WithoutTLS = "HTTP/3 is QUIC, which is TLS always, and the charging station port is unencrypted: " +
                                                "it needs a server certificate, and HTTP authentication over TLS or a TLS client certificate " +
                                                "as a way a client may authenticate (security profile 2 or 3).";

        /// <summary>
        /// Whether it is listening right now.
        /// </summary>
        public Boolean OCPPServerRunning
            => ocppServerStarted;

        /// <summary>
        /// Whether the charging stations are talking to it over TLS.
        /// </summary>
        public Boolean OCPPServerTLS
            => ocppServerTLS;

        /// <summary>
        /// The key pin a charging station pins to reach this controller: the
        /// one of the key presented at the next handshake - or null where the
        /// station port is not encrypted, or nothing is presented.
        /// </summary>
        public KeyPin? OCPPServerKeyPin
            => ocppServerTLS
                   ? StationCertificate.Entry?.KeyPin
                   : null;

        /// <summary>
        /// The key pin of the announced next server key of the station port, to
        /// be handed to the charging stations before the switch - or null where
        /// none was announced.
        /// </summary>
        public KeyPin? OCPPServerNextKeyPin
        {
            get
            {
                Certificates.GetCurrentAndNextKey(StationListener, out var next);
                return next?.KeyPin;
            }
        }

        /// <summary>
        /// Whether the station port has anything to present: an identity of the
        /// store for "ocpp" - or for every listener - or the carrier of a
        /// server key of it.
        /// </summary>
        public Boolean OCPPServerHasCertificate
            => StationCertificate.Choose() is not null;

        /// <summary>
        /// Where the charging stations are told to connect.
        /// </summary>
        public String OCPPServerURL

            => $"{(ocppServerTLS ? "wss" : "ws")}://" +
               $"{(ocppServerSettings.ReachableAs is { Count: > 0 } names ? names[0] : ocppServerSettings.Address?.ToString() ?? "0.0.0.0")}:" +
               $"{ocppServerSettings.TCPPort ?? WebSocketServerConfiguration.DefaultTCPPort}";

        /// <summary>
        /// The server of OCPP-over-SSH, or null when this controller was built
        /// without a station server.
        /// </summary>
        public OCPPSshServer?  StationSSHServer
            => ocppSshServer;

        /// <summary>
        /// Whether the charging stations may connect over SSH right now: the
        /// station server is running, OCPP-over-SSH is switched on, and the
        /// node's SSH server serves it.
        /// </summary>
        public Boolean OCPPOverSSHServed
            => ocppOverSSHServed;

        /// <summary>
        /// Where the charging stations are told to connect over SSH - the SSH
        /// server's port under the name the stations know this controller by,
        /// or where the SSH server listens where no name is given - or null
        /// where there is no SSH server.
        /// </summary>
        /// <remarks>
        /// Not the station server's address: the SSH server listens where the
        /// web interface does, the loopback unless --any says otherwise, and a
        /// station server on every address would otherwise send the stations
        /// to an "ssh://0.0.0.0" nothing answers them on.
        /// </remarks>
        public String? OCPPServerSSHURL

            => SSHURL is String sshURL && SSHPort is IPPort sshPort
                   ? ocppServerSettings.ReachableAs is { Count: > 0 } names
                         ? $"ssh://{names[0]}:{sshPort}"
                         : sshURL
                   : null;

        #endregion


        #region (protected) BuildOCPPServer(Configuration)

        /// <summary>
        /// The stores this server reads from, and the server itself. Nothing
        /// listens yet.
        /// </summary>
        /// <remarks>
        /// Everything lives beside the configuration file rather than beside
        /// the process: a controller pointed at another configuration is a
        /// different controller, and it should not inherit the keys and the
        /// charging stations of the one whose directory the process happens to
        /// sit in.
        /// </remarks>
        protected void BuildOCPPServer(WebSocketServerConfiguration?  Configuration,
                                       CommissioningConfiguration?    Commissioning   = null)
        {

            var directory = Path.GetDirectoryName(ConfigFile.Path) ?? ".";

            // Every kind builds its server once its OCPP nodes exist, which is
            // what the OCPP log needs of them.
            WatchOCPPMessages();

            ocppServerSettings  = (Configuration ?? new WebSocketServerConfiguration()).Effective();
            ocppServerAsBuilt   = ocppServerSettings;

            #region What it presents, out of the node's store

            // A station that cannot be given a new certificate without first
            // connecting is better served by one that ran out than by none.
            ListenerCertificates.KeepServingLast(StationListener);

            StationCertificate = ListenerCertificates.For(StationListener, "the charging station port");

            // Every switch, from wherever it was asked for - the node's page,
            // its JSON API, the command line, OCPP - told to the kind of node:
            // a local controller tells its CSMS.
            Certificates.OnServerKeySwitched += (listener, before, after) => {
                if (listener == StationListener)
                    ServerKeySwitched(before, after);
            };

            #endregion

            #region The two stores, each saying what it finds

            ClientTrust        = new ClientTrustStore(
                                     Path.Combine(directory, ClientTrustStore.DefaultDirectoryName),
                                     TimeProvider,
                                     NodeNoun,
                                     "charging station"
                                 );

            ClientTrust.OnNotice += (level, message) => Log.Log(level, message, "ocpp", "tls", "trust");

            // Their owner's alone, as the node keeps its accounts and its own
            // certificates: a chain added by somebody else lets their charging
            // stations in. What an older controller left open is taken back
            // here, once somebody listens to what cannot be done.
            ClientTrust.KeepToItsOwner();

            WebSocketLogins      = new WebSocketLogins(
                                     Path.Combine(directory, WebSocketLogins.DefaultFileName),
                                     TimeProvider,
                                     NodeNoun
                                 );

            WebSocketLogins.OnNotice += (level, message) => Log.Log(level, message, "ocpp", "station", "auth");

            // A target names peers: asked of the connections, which are built
            // beside these logins and change while this node runs.
            WebSocketLogins.KnownPeerIds = () => Connections?.Peers.Select(peer => peer.Id) ?? [];

            // A file that is there but unreadable is not papered over with an
            // empty list: that would turn every charging station on the site
            // away while looking like nothing was wrong.
            if (!WebSocketLogins.TryLoad(out var loginsError))
                throw new InvalidOperationException($"{loginsError} Repair or remove it and start again.");

            WebSocketLogins.OnChanged += () => {

                foreach (var server in StationServers)
                    StationSignIns.ApplyLogins(server);

                // Over SSH, a station whose key was taken away or that was
                // switched off is signed out now rather than at the next
                // round of the node's own rechecks.
                RecheckSSHSessions();

                // And a back end whose login went, or was switched off, is let
                // go of, and what can not sign in any more is said.
                FollowInboundConnections();

            };

            #endregion

            #region The door of the station port, by the two stores

            // What the port allows, whether it speaks TLS and what is logged
            // are asked at every sign-in, as the stores are: a change of them
            // is in effect at the next one. The version of OCPP is the one of
            // the server a WebSocket came to: the server of OCPP 1.6 is handed
            // those that ask for it.
            StationSignIns     = new WebSocketSignIns(
                                     Logins:               WebSocketLogins,
                                     ClientTrust:          ClientTrust,
                                     SignInBrake:          SignInBrake,
                                     Log:                  Log,
                                     Authentications:      () => ocppServerSettings.ClientAuthentication ?? WebSocketServerConfiguration.DefaultClientAuthentication,
                                     OverTLS:              () => ocppServerTLS,
                                     ChecksRevocation:     () => ocppServerSettings.CheckCertificateRevocation == true,
                                     SaysSignIns:          () => ocppServerSettings.Logging?.Authentication != false,
                                     InboundConnectionOf:  (server, name) => InboundConnectionOf(
                                                                                 ReferenceEquals(server, ocppWebSocketServer16)
                                                                                     ? OCPPVersion.OCPP1_6
                                                                                     : OCPPVersion.OCPP2_1,
                                                                                 name
                                                                             ),
                                     LetInAsInbound:       LetInAsInbound,
                                     LetInOthers:          LetInAsStation,
                                     Words:                StationWords
                                 );

            #endregion

            var settings        = ocppServerSettings;
            var subprotocols    = settings.Subprotocols ?? WebSocketServerConfiguration.DefaultSubprotocols;

            #region The servers: one port, and a node per version of OCPP behind it

            BuildStationServers(settings);

            #endregion

            #region OCPP-over-SSH, on the node's SSH server

            // Built here, as the WebSocket server is, with what it is
            // configured as at the start; served or not while running.
            // One subsystem for every version, as one port: a station is handed
            // by the version its HELLO chose to the node that speaks it - on
            // every kind of node alike, served where the file says so.
            {

                AttachOCPPOverSSH(settings);

                stationSSHSignIns = new LoginSSHSignIns(
                                        WebSocketLogins,
                                        OCPPSshServer.OCPPSubsystem,
                                        () => ocppOverSSHServed,
                                        TimeProvider,
                                        () => SSHSubsystems.Select(subsystem => subsystem.Name),
                                        SSHSubsystemOfLogin
                                    );

                stationSSHSignIns.OnNotice += (level, message) => {
                    if (ocppServerSettings.Logging?.Authentication != false)
                        Log.Log(level, message, "ocpp", "station", "auth", "ssh");
                };

                AddSSHSignIns(stationSSHSignIns);

            }

            #endregion

            #region Commissioning over SSH, on the same subsystem

            // Built here as well, and served or not while running - see
            // OCPPServingNode.Commissioning.cs. The section is every kind's:
            // read here where the kind did not read it itself, and wrong, it
            // stops the node, as a wrong station port section does.
            if (Commissioning is null &&
                !CommissioningConfiguration.TryParseSectionOf(ConfigurationDocument, out Commissioning, out var commissioningProblem))
            {
                throw new InvalidOperationException($"'{ConfigFile.Path}': {commissioningProblem} Repair or remove '{ConfigFile.Path}' and start again.");
            }

            this.Commissioning = Commissioning ?? new CommissioningConfiguration();

            AttachCommissioning();

            #endregion

            // Whether what is not for this node is passed on at all: every
            // kind's, read here as the commissioning section is.
            ReadForwarding();

            Log.Info(
                settings.Enabled == true
                    ? $"The charging station server will listen on {settings.Address}:{settings.TCPPort}, " +
                      $"speaking {String.Join(" and ", subprotocols)}, " +
                      $"letting clients authenticate by {AuthenticationText(settings)}."
                    : "The charging station server is switched off; no charging station can connect. " +
                      "Switch it on under Configuration to let them in.",
                "ocpp", "station"
            );

        }

        #endregion

        #region (private) BuildStationServers(Settings) / AttachOCPPOverSSH(Settings)

        /// <summary>
        /// The servers of the station port, attached to the nodes this node
        /// has now: the one of OCPP 2.1 that listens, and the one of OCPP 1.6
        /// it hands WebSockets to. Nothing listens yet.
        /// </summary>
        private void BuildStationServers(WebSocketServerConfiguration settings)
        {

            var subprotocols    = settings.Subprotocols ?? WebSocketServerConfiguration.DefaultSubprotocols;
            var subprotocols16  = subprotocols.Where( IsOCPP16).ToArray();
            var subprotocols21  = subprotocols.Where(subprotocol => !IsOCPP16(subprotocol)).ToArray();

            // Each WebSocket is handed by the subprotocol it asks for to the
            // node that speaks it: the server of OCPP 2.1 listens, and hands
            // one of OCPP 1.6 to the server of the node of OCPP 1.6. Who comes
            // in is decided by each as by the other - see StationSignIns -
            // and the TLS of the port is the listening server's, before a
            // subprotocol is asked for.
            ocppWebSocketServer = Node21.AttachWebSocketServer(

                                      HTTPServiceName:              $"OpenChargingCloud {Kind.Product} v{Version}",
                                      IPAddress:                    settings.Address,
                                      TCPPort:                      settings.TCPPort,
                                      Description:                  I18NString.Create($"The charging stations of this {NodeNoun}"),

                                      // Off, because this server does the
                                      // deciding itself: a station on security
                                      // profile 3 authenticates with its
                                      // certificate and has no password to send,
                                      // and the check built in here cannot tell
                                      // the two apart. See StationSignIns.
                                      RequireAuthentication:        false,
                                      SecWebSocketProtocols:        subprotocols21,
                                      DisableWebSocketPings:        false,
                                      WebSocketPingEvery:           settings.PingEvery,

                                      ClientCertificateValidator:   StationSignIns.ValidateCertificate,
                                      AllowedTLSProtocols:          settings.MinimumTLSVersion,

                                      // "Ask for one", not "insist on one": a
                                      // station on profile 2 sends none, and
                                      // whether that is acceptable is decided
                                      // in the validator, which knows which
                                      // profiles are allowed.
                                      //
                                      // Hermod asks for one whenever a validator
                                      // is set, which it always is here, so this
                                      // says out loud what is happening rather
                                      // than deciding it.
                                      ClientCertificateRequired:    true,
                                      CheckCertificateRevocation:   settings.CheckCertificateRevocation,

                                      MaxClientConnections:         settings.MaxConnections,

                                      AutoStart:                    false

                                  );

            // Behind the door of the station port: a station's certificate let
            // in by its key pin names nobody, who asks for a WebSocket is
            // decided there, and the logins with a password are put in. The
            // same for a back end that connects: it signs in with a WebSocket
            // login as well.
            StationSignIns.Guard(ocppWebSocketServer);

            // A WebSocket server of WWCP takes HTTP/3 unless told otherwise:
            // here what the file says from the start, and whether the port is
            // encrypted, which HTTP/3 needs, once it starts.
            ocppWebSocketServer.AcceptHTTP3 = settings.TakesHTTP3;

            WireOCPPServerLogging(ocppWebSocketServer);
            FollowStationsSeen(ocppWebSocketServer);

            #region OCPP 1.6, handed on by the server that listens

            if (subprotocols16.Length > 0)
            {

                if (Node16 is { } node16)
                {

                    // Never started: it has no port of its own.
                    ocppWebSocketServer16 = node16.AttachWebSocketServer(

                                                HTTPServiceName:              $"OpenChargingCloud {Kind.Product} v{Version}",
                                                IPAddress:                    settings.Address,
                                                TCPPort:                      settings.TCPPort,
                                                Description:                  I18NString.Create($"The charging stations of OCPP 1.6 of this {NodeNoun}"),

                                                RequireAuthentication:        false,
                                                SecWebSocketProtocols:        subprotocols16,
                                                DisableWebSocketPings:        false,
                                                WebSocketPingEvery:           settings.PingEvery,

                                                MaxClientConnections:         settings.MaxConnections,

                                                AutoStart:                    false

                                            );

                    StationSignIns.Guard(ocppWebSocketServer16);

                    WireOCPPServerLogging(ocppWebSocketServer16);
                    FollowStationsSeen(ocppWebSocketServer16);

                    ocppWebSocketServer.RouteBySubprotocol(ocppWebSocketServer16);

                }

                else
                    Log.Warning(
                        $"The station port is to offer {String.Join(" and ", subprotocols16)}, and this {NodeNoun} does not speak OCPP 1.6: " +
                         "a charging station asking for it is turned away.",
                        "ocpp", "station"
                    );

            }

            #endregion

        }

        /// <summary>
        /// The servers of OCPP-over-SSH, attached to the nodes this node has
        /// now; served or not while running.
        /// </summary>
        private void AttachOCPPOverSSH(WebSocketServerConfiguration settings)
        {

            var subprotocols       = settings.Subprotocols ?? WebSocketServerConfiguration.DefaultSubprotocols;
            var sshSubprotocols    = settings.SSH?.Subprotocols ?? subprotocols;
            var sshSubprotocols16  = sshSubprotocols.Where( IsOCPP16).ToArray();

            ocppSshServer = Node21.AttachOCPPSshSubsystem(
                                Subsystem:       OCPPSshServer.OCPPSubsystem,
                                Subprotocols:    sshSubprotocols.Where(subprotocol => !IsOCPP16(subprotocol)).ToArray(),
                                MaxMessageSize:  settings.SSH?.MaxMessageSize ?? WebSocketServerSSH.DefaultMaxMessageSize,
                                PingEvery:       settings.SSH?.PingEvery      ?? WebSocketServerSSH.DefaultPingEvery
                            );

            WireOCPPOverSSHLogging(ocppSshServer);
            FollowStationsSeen(ocppSshServer);

            if (sshSubprotocols16.Length > 0 && Node16 is { } sshNode16)
            {

                ocppSshServer16 = sshNode16.AttachOCPPSshSubsystem(
                                      Subsystem:       OCPPSshServer.OCPPSubsystem,
                                      Subprotocols:    sshSubprotocols16,
                                      MaxMessageSize:  settings.SSH?.MaxMessageSize ?? WebSocketServerSSH.DefaultMaxMessageSize,
                                      PingEvery:       settings.SSH?.PingEvery      ?? WebSocketServerSSH.DefaultPingEvery
                                  );

                WireOCPPOverSSHLogging(ocppSshServer16);
                FollowStationsSeen(ocppSshServer16);

            }

        }

        #endregion

        #region (protected) ServeOCPPByTheNewNodes(Old16, Old21)

        /// <summary>
        /// After the kind built its OCPP nodes anew: the servers of the station
        /// port are let go of on the old nodes, and built again on the new
        /// ones - listening again where they listened. Whoever was connected
        /// connects again, and is told again what it is told on connecting.
        /// </summary>
        /// <remarks>
        /// A server is attached to a node when it is built, and a node rebuilt
        /// keeps nothing of it: the new node would otherwise answer none of the
        /// WebSockets the old one's server takes. Built as it was built at the
        /// start, so that what waits for a restart goes on waiting. The servers
        /// of OCPP-over-SSH - the stations' and the commissioning tools' - are
        /// built again on the new nodes as well, and the subsystem put on anew:
        /// whoever was connected over SSH connects again.
        /// </remarks>
        /// <param name="Old16">The node of OCPP 1.6 before, where there was one.</param>
        /// <param name="Old21">The node of OCPP 2.1 before.</param>
        protected async Task ServeOCPPByTheNewNodes(OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Old16,
                                                    OCPPv2_1.NetworkingNode.AOCPPNetworkingNode   Old21)
        {

            // Over SSH: the stations' servers and the tools', on the new
            // nodes, and the subsystem anew where it is served.
            if (ocppSshServer is not null)
                AttachOCPPOverSSH(ocppServerAsBuilt);

            AttachCommissioning();

            if (ocppSubsystem is not null && ServeTheOCPPSubsystem(Anew: true) is String problem)
                Log.Critical($"OCPP-over-SSH could not be served by the rebuilt OCPP nodes: {problem}", "ocpp", "ssh");

            if (ocppWebSocketServer is not { } server21)
                return;

            var server16    = ocppWebSocketServer16;
            var wasStarted  = ocppServerStarted;

            await StopOCPPServer();

            if (server16 is not null)
                server21.StopRouting(server16);

            Old21.DetachWebSocketServer(server21);

            if (server16 is not null)
                Old16?.DetachWebSocketServer(server16);

            foreach (var server in new[] { server16, server21 })
            {

                if (server is null)
                    continue;

                try
                {
                    await server.DisposeAsync();
                }
                catch (Exception e)
                {
                    Log.Warning($"A server of the station port could not be let go of on the node it was attached to: {e.GetBaseException().Message}",
                                "ocpp", "station");
                }

            }

            ocppWebSocketServer    = null;
            ocppWebSocketServer16  = null;

            BuildStationServers(ocppServerAsBuilt);

            if (wasStarted)
                await StartOCPPServer();

        }

        #endregion

        #region (protected) StartOCPPServer() / StopOCPPServer()

        /// <summary>
        /// Start listening for charging stations, when this controller is
        /// meant to.
        /// </summary>
        protected async Task StartOCPPServer()
        {

            if (ocppWebSocketServer is null || ocppServerStarted)
                return;

            if (!OCPPServerEnabled)
                return;

            var settings   = ocppServerSettings;
            var wantsTLS   = WantsTLS(settings);

            #region Encrypted or not, decided here - until a change of the ways in opens the port anew

            ocppServerTLS = wantsTLS && OCPPServerHasCertificate;

            if (ocppServerTLS)
            {

                // Chosen at every handshake, read once until the store changes.
                ocppWebSocketServer.ServerCertificateChainSelector = (server, client) => StationCertificate.Current;

                #region Can the chain that would be presented actually be presented?

                // Asked now rather than found out per connection. .NET builds
                // the chain of the server's own certificate before it can
                // present it, and refuses when it cannot reach a root this
                // machine has - a private authority whose root was never
                // installed, most often. What a charging station then sees is a
                // TLS handshake reset with nothing said, and this port would
                // turn every single one of them away while reporting itself as
                // running and encrypted.
                try
                {

                    var chain = StationCertificate.Current;

                    if (chain is null)
                        Log.Critical(
                            "The charging station port is meant to be encrypted, but no certificate could be chosen for it. " +
                            "No charging station will be able to connect.",
                            "ocpp", "station", "tls"
                        );

                    else if (!chain.TryCreateContext(out _, out var why))
                        Log.Critical(
                            $"The charging station port is encrypted, but the certificate it would present cannot be served: {why} " +
                             "Until this is put right every charging station is turned away during the TLS handshake, " +
                             "which looks to them like the port simply dropping the connection.",
                            "ocpp", "station", "tls"
                        );

                }
                catch (Exception e)
                {
                    Log.Critical(
                        $"The certificate for the charging station port could not be examined: {e.Message} " +
                         "Charging stations may be turned away during the TLS handshake.",
                        "ocpp", "station", "tls"
                    );
                }

                #endregion

            }

            else
            {

                ocppWebSocketServer.ServerCertificateChainSelector = null;

                if (wantsTLS)
                    Log.Warning(
                        "HTTP authentication over TLS or a TLS client certificate (security profile 2 or 3) is configured, but there is no server " +
                        "certificate - so the charging station port is unencrypted and only HTTP authentication without TLS (security profile 1) " +
                        "can be used on it. Make a TLS server identity for the listener " +
                        $"'{StationListener}' on the Server identities page, or a server key of it for the stations to pin; the port " +
                        "encrypts from the next start.",
                        "ocpp", "station", "tls"
                    );

                else if (OCPPServerHasCertificate)
                    Log.Notice(
                        "Only HTTP authentication without TLS (security profile 1) is allowed, so the charging station port is unencrypted and the server certificate is not presented.",
                        "ocpp", "station", "tls"
                    );

            }

            #endregion

            // A TCP port and nothing else, unless the file asks for HTTP/3:
            // it opens a UDP port of the same number, which nobody is given
            // who did not ask. And never without TLS.
            TakeHTTP3OrNot(settings);

            // The socket layer's own words for a port somebody else has name
            // neither the port nor what it was for, and both are known here.
            try
            {
                await ocppWebSocketServer.Start();
            }
            catch (SocketException problem)
            {
                throw new PortUnavailableException(
                          settings.TCPPort ?? WebSocketServerConfiguration.DefaultTCPPort,
                          problem,
                          StationServerPort
                      );
            }

            ocppServerStarted = true;

            Log.Notice(
                $"The charging stations connect to {OCPPServerURL} " +
                $"({(ocppServerTLS ? "TLS" : "unencrypted")}, {AuthenticationText(settings)}, " +
                $"{WebSocketLogins.EnabledCount} station login(s), {ClientTrust.EnabledCount} accepted chain(s))" +
                (OCPPServerKeyPin     is KeyPin keyPin  ? $", key pin {keyPin}"           : "") +
                (OCPPServerNextKeyPin is KeyPin nextPin ? $", next key pin {nextPin}"     : "") +
                (ocppWebSocketServer.AcceptHTTP3        ? $" - over HTTP/3 as well, on UDP port {settings.TCPPort ?? WebSocketServerConfiguration.DefaultTCPPort}" : "") +
                ".",
                "ocpp", "station"
            );

            if (settings.SSH?.Enabled == true)
                ServeOCPPOverSSH();

            ServeTheBackEndsThatConnect();

            #region What is about to run out, now and every day after this

            CheckCertificates();

            certificateCheckTimer?.Dispose();
            certificateCheckTimer = TimeProvider.CreateTimer(
                                        _ => CheckCertificates(),
                                        null,
                                        CertificateCheckEvery,
                                        CertificateCheckEvery
                                    );

            #endregion

            #region The two ways to have a port nobody can come through

            if (!ocppServerTLS && WebSocketLogins.EnabledCount == 0)
                Log.Warning(
                    "No charging station has a login here, so none can sign in. Add them under Configuration.",
                    "ocpp", "station", "auth"
                );

            if ((settings.ClientAuthentication ?? WebSocketServerConfiguration.DefaultClientAuthentication).Contains(WebSocketClientAuthentication.TLSClientCertificate) &&
                ClientTrust.EnabledCount == 0 &&
                !WebSocketLogins.Logins.Any(login => login.HasKeyPin))
                Log.Warning(
                    "A TLS client certificate (security profile 3) is allowed, but no certificate authority has been named to accept charging stations from, " +
                    "and no station has a key pin - so no station can connect with a certificate. An empty list is 'nobody', not 'everybody'.",
                    "ocpp", "station", "tls"
                );

            #endregion

        }

        /// <summary>
        /// Take WebSockets over HTTP/3 as well where the settings ask for it
        /// and the port is encrypted, and none otherwise; said where it was
        /// asked for and cannot be.
        /// </summary>
        private void TakeHTTP3OrNot(WebSocketServerConfiguration Settings)
        {

            if (ocppWebSocketServer is null)
                return;

            ocppWebSocketServer.AcceptHTTP3 = Settings.TakesHTTP3 && ocppServerTLS;

            if (Settings.TakesHTTP3 && !ocppServerTLS)
                Log.Warning(
                    $"The charging station port takes no WebSockets over HTTP/3, although the configuration says it shall: {NoHTTP3WithoutTLS}",
                    "ocpp", "station", "tls"
                );

            // A back end whose one-time password is bound to its TLS session
            // signs in over HTTP/3 only: what is said of its connection follows.
            FollowInboundConnections();

        }

        /// <summary>
        /// Whether the settings ask for TLS: a way in over TLS among those
        /// allowed - security profile 2 or 3.
        /// </summary>
        private static Boolean WantsTLS(WebSocketServerConfiguration Settings)

            => (Settings.ClientAuthentication ?? WebSocketServerConfiguration.DefaultClientAuthentication).Any(authentication => authentication.IsOverTLS());

        /// <summary>
        /// Whether the running station port is encrypted otherwise than the
        /// settings now ask: TLS allowed where there was none, with a server
        /// certificate to present, or only security profile 1 left where the
        /// port speaks TLS.
        /// </summary>
        /// <remarks>
        /// Whether the port speaks TLS is decided when it is opened. A port left
        /// as it was after such a change takes no charging station at all: it
        /// reads a TLS handshake as HTTP, or waits for one from a station that
        /// dials ws://.
        /// </remarks>
        private Boolean EncryptsOtherwiseThan(WebSocketServerConfiguration Settings)

            => ocppServerStarted &&
               ocppServerTLS != (WantsTLS(Settings) && OCPPServerHasCertificate);

        /// <summary>
        /// Close the station port and open it again, encrypted or not as the
        /// settings now say. Whoever was connected connects again.
        /// </summary>
        private async Task OpenTheStationPortAnew()
        {

            var encrypted = !ocppServerTLS;

            Log.Notice($"The charging station port is opened anew, {(encrypted ? "encrypted" : "unencrypted")}, as the ways in now ask: " +
                       $"the stations connected connect again, over {(encrypted ? "wss" : "ws")}://.",
                       "ocpp", "station", "config");

            await StopOCPPServer(). ConfigureAwait(false);
            await StartOCPPServer().ConfigureAwait(false);

        }

        /// <summary>
        /// Stop listening for charging stations.
        /// </summary>
        protected async Task StopOCPPServer()
        {

            if (ocppWebSocketServer is null || !ocppServerStarted)
                return;

            ocppServerStarted = false;

            certificateCheckTimer?.Dispose();
            certificateCheckTimer = null;

            StopServingOCPPOverSSH("the station server stopped");

            StopServingTheBackEndsThatConnect($"The station server of this {NodeNoun} stopped.");

            await ocppWebSocketServer.Shutdown($"The {NodeNoun} is shutting down.");

            // Its WebSockets came in on the port that stopped: they end with it.
            if (ocppWebSocketServer16 is not null)
                await ocppWebSocketServer16.Shutdown($"The {NodeNoun} is shutting down.");

            Log.Info("The charging station server has stopped listening.", "ocpp", "station");

        }

        #endregion

        #region (private) ServeTheBackEndsThatConnect() / StopServingTheBackEndsThatConnect(Why)

        /// <summary>
        /// Let in, over the station port, the back ends that connect to this
        /// controller: the connections the other end makes, where this
        /// controller reports to a CSMS at all.
        /// </summary>
        /// <remarks>
        /// The port is the charging stations' and stays theirs: who signs in
        /// there is decided in <see cref="StationSignIns"/>, which asks first
        /// whether somebody comes under the name of such a connection. Nothing
        /// of it is the server's to know.
        /// </remarks>
        private void ServeTheBackEndsThatConnect()
        {

            if (ocppWebSocketServer is null || !ocppServerStarted || !LetsBackEndsConnect)
                return;

            ServeInbound(OCPPVersion.OCPP2_1, ocppWebSocketServer);

            if (ocppWebSocketServer16 is not null)
                ServeInbound(OCPPVersion.OCPP1_6, ocppWebSocketServer16);

            ServeTheBackEndsThatConnectOverSSH();

            var written = Connections.Connections.Count(connection => connection.Inbound);

            if (written > 0)
                Log.Notice($"{written} connection{(written == 1 ? "" : "s")} of this {NodeNoun} {(written == 1 ? "is" : "are")} made by the other end: " +
                           $"a CSMS connects to {OCPPServerURL}/<its name>, the port of the charging stations.",
                           "ocpp", "csms");

        }

        /// <summary>
        /// Let no back end in any more, and let go of those that are.
        /// </summary>
        private void StopServingTheBackEndsThatConnect(String Why)
        {
            StopServingInbound       (OCPPVersion.OCPP2_1, Why);
            StopServingInbound       (OCPPVersion.OCPP1_6, Why);
            StopServingInboundOverSSH(OCPPVersion.OCPP2_1, Why);
            StopServingInboundOverSSH(OCPPVersion.OCPP1_6, Why);
        }

        /// <summary>
        /// Let in over the OCPP subsystem of the node's SSH server the back
        /// ends that connect to this node over SSH: where OCPP-over-SSH is served,
        /// the station server runs, and this node reports to a CSMS at all.
        /// </summary>
        /// <remarks>
        /// Who signs in is the SSH server's to decide, by the WebSocket login
        /// of the name an ssh:// connection the other end makes comes under -
        /// with its password, its one-time password or its SSH key.
        /// </remarks>
        private void ServeTheBackEndsThatConnectOverSSH()
        {

            if (ocppSshServer is null || !ocppOverSSHServed || !ocppServerStarted || !LetsBackEndsConnect)
                return;

            ServeInbound(OCPPVersion.OCPP2_1, ocppSshServer);

            if (ocppSshServer16 is not null)
                ServeInbound(OCPPVersion.OCPP1_6, ocppSshServer16);

        }

        #endregion

        #region TryAddStationSSHKey(Id, Line, Label, GroupId, Description, CreatedBy, out Key, out Error, out NotSaved)

        /// <summary>
        /// Give a charging station an SSH key for OCPP-over-SSH - adding the
        /// station where it is not listed yet.
        /// </summary>
        /// <remarks>
        /// Refused where an account of this controller has the station's name:
        /// both would sign in to the same SSH server under it, and a name that
        /// is both is let in as neither. Asked here rather than in the store,
        /// which knows the stations and not the accounts.
        /// </remarks>
        /// <param name="Id">What the charging station calls itself in OCPP, and signs in to SSH as.</param>
        /// <param name="Line">Its public key, as one line of an authorized_keys file.</param>
        /// <param name="Label">What the key is called, if anything.</param>
        /// <param name="GroupId">The group it should belong to, or null for the one it is in already.</param>
        /// <param name="Description">What this station is, for whoever reads the list later - in as many languages as it is said in.</param>
        /// <param name="CreatedBy">Who added the key, if anybody said.</param>
        /// <param name="Key">The key, as it was added.</param>
        /// <param name="Error">Why it was refused.</param>
        /// <param name="NotSaved">True where the file could not be written.</param>
        public Boolean TryAddStationSSHKey(User_Id                               Login,
                                           String?                               Line,
                                           String?                               Label,
                                           String?                               GroupId,
                                           I18NString?                           Description,
                                           String?                               CreatedBy,
                                           [NotNullWhen(true)]  out UserSSHKey?  Key,
                                           [NotNullWhen(false)] out String?      Error,
                                           out Boolean                           NotSaved)
        {

            Key       = null;
            NotSaved  = false;

            if (ExtAPI.TryGetUser(Login, out var user))
            {
                Error = $"'{Login}' is an account of this {NodeNoun}. A charging station of the same name could not sign in over SSH: " +
                         "both would sign in under one name, and a name that is both is let in as neither.";
                return false;
            }

            return WebSocketLogins.TryAddSSHKey(
                       Login,
                       Line,
                       Label,
                       GroupId,
                       Description,
                       CreatedBy,
                       out Key,
                       out Error,
                       out NotSaved
                   );

        }

        #endregion

        #region (private) ServeOCPPOverSSH() / StopServingOCPPOverSSH(Why)

        /// <summary>
        /// Serve OCPP-over-SSH from now on: the subsystem on the node's SSH
        /// server, and the stations with an SSH key let in to it.
        /// </summary>
        /// <remarks>
        /// Said out loud where it cannot work: OCPP-over-SSH switched on with
        /// the node's SSH server off is a door painted on a wall.
        /// </remarks>
        private void ServeOCPPOverSSH()
        {

            if (ocppSshServer is null || ocppOverSSHServed)
                return;

            // Before the subsystem is there, so that a station asking for it
            // the moment it is finds itself let in.
            ocppOverSSHServed = true;

            // The one subsystem, which commissioning may serve already - see
            // OCPPServingNode.Commissioning.cs.
            if (ServeTheOCPPSubsystem() is String problem)
            {

                ocppOverSSHServed = false;

                Log.Critical($"OCPP-over-SSH could not be served: {problem}", "ocpp", "station", "ssh");
                return;

            }

            // The back ends that connect over SSH, where the station server runs
            // already - otherwise when it starts.
            ServeTheBackEndsThatConnectOverSSH();

            var stations = WebSocketLogins.Logins.Count(login => login.Enabled && login.HasSSHKey);
            var withTotp = WebSocketLogins.Logins.Count(login => login.Enabled && login.HasTOTP);

            if (!SSHEnabled)
                Log.Warning(
                    $"OCPP-over-SSH is switched on, but the SSH server of this {NodeNoun} is off - so no charging station can reach it " +
                    "over SSH. Switch SSH on under Configuration.",
                    "ocpp", "station", "ssh"
                );

            else
                Log.Notice(
                    $"The charging stations may connect over SSH as well: {OCPPServerSSHURL}, subsystem {OCPPSshServer.OCPPSubsystem}, " +
                    $"host key {SSHHostKey ?? "not known yet"} ({stations} station(s) with an SSH key, {withTotp} with a TOTP secret).",
                    "ocpp", "station", "ssh"
                );

        }

        /// <summary>
        /// Serve OCPP-over-SSH no longer: every station connected over it
        /// ends, and the subsystem goes where it serves no commissioning tool
        /// either.
        /// </summary>
        /// <param name="Why">Why, for the log.</param>
        private void StopServingOCPPOverSSH(String Why)
        {

            if (!ocppOverSSHServed)
                return;

            ocppOverSSHServed = false;

            StopServingInboundOverSSH(OCPPVersion.OCPP2_1, Why);
            StopServingInboundOverSSH(OCPPVersion.OCPP1_6, Why);

            ServeTheOCPPSubsystem();

            Log.Info($"OCPP-over-SSH is no longer served: {Why}.", "ocpp", "station", "ssh");

        }

        #endregion

        #region (private) CheckCertificates()

        /// <summary>
        /// Look at every accepted chain, and say what is about to run out.
        /// </summary>
        /// <remarks>
        /// An expired trust anchor stops this controller believing the
        /// charging stations, which takes a site down as surely as its own
        /// certificate running out would - which the node looks at, see
        /// ListenerCertificates.
        ///
        /// Whatever goes wrong in here is caught: this runs on a timer, and a
        /// timer callback that throws takes the process with it. A controller
        /// that stopped charging cars because it could not read a certificate
        /// file would be the worst possible answer to "your certificate expires
        /// next month".
        /// </remarks>
        private void CheckCertificates()
        {

            try
            {
                ClientTrust.CheckExpiry();
            }
            catch (Exception e)
            {
                Log.Exception(e, "The certificates of the charging station server could not be checked.", "ocpp", "tls");
            }

            try
            {
                CertificatesChecked();
            }
            catch (Exception e)
            {
                Log.Exception(e, $"What this {NodeNoun} does once the certificates are checked failed.", "ocpp", "tls");
            }

        }

        /// <summary>
        /// What a kind of node does once the certificates of the charging
        /// station server were checked, on the same timer - a CSMS asks the
        /// connected charging stations for new certificates of their own.
        /// Nothing by default. Whatever it throws is caught and logged.
        /// </summary>
        protected virtual void CertificatesChecked()
        { }

        #endregion

        #region (private static) IsOCPP16(Subprotocol) / AuthenticationText(Settings)

        /// <summary>
        /// Whether the given subprotocol is OCPP 1.6, as JSON or as CBOR: the
        /// node of OCPP 1.6 speaks it.
        /// </summary>
        private static Boolean IsOCPP16(String Subprotocol)

            => OCPPWebSockets.OCPPSubprotocols.JSONOf(Subprotocol) == OCPPv1_6.Version.WebSocketSubProtocolId;

        /// <summary>
        /// How a client may authenticate at the station port, in words: what
        /// the log says it starts with.
        /// </summary>
        private static String AuthenticationText(WebSocketServerConfiguration Settings)

            => String.Join(", ", (Settings.ClientAuthentication ?? WebSocketServerConfiguration.DefaultClientAuthentication).
                                     Select(authentication => $"{authentication.ToText()} (security profile {authentication.SecurityProfile()})"));

        #endregion

        #region (protected virtual) SSHSubsystemOfLogin(Login)

        /// <summary>
        /// The SSH subsystem a WebSocket login may open: OCPP's - unless the
        /// kind says that the login is a connection of another protocol, which
        /// then gets the subsystem of that one, and not OCPP's.
        /// </summary>
        /// <param name="Login">A WebSocket login that signed in to the SSH server.</param>
        protected virtual String SSHSubsystemOfLogin(User_Id Login)

            => OCPPSshServer.OCPPSubsystem;

        #endregion

        #region (private) LetInAsStation(Connection, Login, How, Detail, ByCertificate)

        /// <summary>
        /// Let in as a charging station what signed in at the station port
        /// under a WebSocket login that is no back end that connects to this
        /// node: said, kept on the connection, and - where it signed in with a
        /// certificate - told to the kind of node.
        /// </summary>
        /// <remarks>
        /// Who signs in is decided by <see cref="StationSignIns"/>, for a
        /// charging station and a back end alike; the name decides
        /// afterwards which of the two it is, and this is the station.
        /// </remarks>
        private String? LetInAsStation(WebSocketServerConnection  Connection,
                                       String                     Login,
                                       String                     How,
                                       String                     Detail,
                                       X509Certificate2?          ByCertificate)
        {

            // The name of a connection that carries another protocol is that
            // connection's, and no charging station's: an EMSP that connects to a
            // CSMS over OCPI does not sign in as a station over OCPP.
            if (Connections?.Connections.FirstOrDefault(entry => entry.Inbound &&
                                                                 entry.Use != ConnectionUse.OCPP &&
                                                                 entry.InboundName == Login) is ConnectionEntry other)
            {
                return $"'{Login}' is the name '{other.Description.FirstText()}' comes under, which connects over {other.Protocol}, and no charging station";
            }

            if (ocppServerSettings.Logging?.Authentication != false)
                Log.Info($"The charging station '{Login}' signed in from {Connection.RemoteSocket} with {How} ({Detail}).",
                         "ocpp", "station", "auth");

            Connection.TryAddCustomData(StationIdKey, Login);

            // What it signed in with is what runs out - and what a kind of
            // node may ask it to renew before it does.
            if (ByCertificate is not null &&
                ByCertificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, false) is String named &&
                !named.IsNullOrEmpty())
            {
                StationSignedInWithCertificate(named, ByCertificate);
            }

            return null;

        }

        #endregion

        #region (protected override) InboundLoginLetsIn(Connection) / WhyInboundCanNotSignIn(Connection, Server)

        /// <summary>
        /// Whether the WebSocket login of the name the connection comes under
        /// is switched on - or there is none: then a certificate issued to
        /// that name, of a chain the station port accepts, is what lets the
        /// other end in, as it lets in a charging station without a login.
        /// </summary>
        protected override Boolean InboundLoginLetsIn(ConnectionEntry Connection)

            => Connection.InboundName is String name &&
               StationSignIns.LoginLetsIn(name);

        /// <summary>
        /// Why the other end of a connection it makes can not sign in, as its
        /// WebSocket login and the station port are now - or null.
        /// </summary>
        protected override String? WhyInboundCanNotSignIn(ConnectionEntry                                                Connection,
                                                          cloud.charging.open.protocols.WWCP.NetworkingNode.IWWCPServer  Server)
        {

            var name = Connection.InboundName ?? "";

            // Over SSH: the SSH server lets in what signs in with the WebSocket
            // login of its name - with its password, its one-time password or
            // its SSH key - and nothing without one.
            if (Connection.IsSSH)
            {

                if (!SSHEnabled)
                    return $"the SSH server of this {NodeNoun} is off. Switch SSH on under Configuration.";

                if (!WebSocketLogins.TryGet(WebSocketLogins.LoginOf(name), out var sshLogin))
                    return $"there is no WebSocket login '{name}' it could sign in to the SSH server with. Add one under WebSocket logins, " +
                            "with its password, its one-time password or its SSH key.";

                if (!sshLogin.Enabled)
                    return $"its WebSocket login '{name}' is switched off.";

                if (WebSocketLogins.GroupOf(sshLogin) is not { } sshGroup)
                    return $"its WebSocket login '{name}' belongs to the login group '{sshLogin.GroupId}', which this {NodeNoun} does not have.";

                if (!sshGroup.Enabled)
                    return $"its WebSocket login '{name}' is in the login group '{sshGroup.Id}', which is switched off.";

                return null;

            }

            return StationSignIns.WhyCanNotSignIn(name, ocppServerStarted, Server);

        }

        #endregion

        #region (private) WireOCPPServerLogging(Server)

        /// <summary>
        /// What of this server ends up in the event log, and what does not.
        /// </summary>
        /// <remarks>
        /// The switches are read inside the handlers rather than used to attach
        /// and detach them, so that changing one takes effect on the next
        /// message instead of the next start - and so that there is one place
        /// to look for what is logged rather than a set of subscriptions to
        /// reason about.
        /// </remarks>
        private void WireOCPPServerLogging(OCPPWebSockets.OCPPWebSocketServer Server)
        {

            #region Charging stations coming and going

            Server.OnNewWebSocketConnection += (timestamp, server, connection, sharedSubprotocols, selectedSubprotocol, eventTrackingId, cancellationToken) => {

                if (ocppServerSettings.Logging?.Connections != false)
                    Log.Notice(
                        $"{WhoIsOn(connection)} is connected from {connection.RemoteSocket}" +
                        $" ({selectedSubprotocol ?? "no subprotocol"}).",
                        "ocpp", "station"
                    );

                return Task.CompletedTask;

            };

            Server.OnCloseMessageReceived += (timestamp, server, connection, frame, eventTrackingId, statusCode, reason, cancellationToken) => {

                if (ocppServerSettings.Logging?.Connections != false)
                    Log.Notice(
                        $"{WhoIsOn(connection)} went away ({statusCode}{(reason.IsNullOrEmpty() ? "" : $": {reason}")}).",
                        "ocpp", "station"
                    );

                return Task.CompletedTask;

            };

            // A connection that never became one. Not behind the logging
            // switches: what arrives here is a charging station being dropped
            // before it could say anything, and that is never noise. The
            // commonest cause is a TLS handshake that could not be started at
            // all, which from the other end is a bare reset with nothing to
            // explain it.
            Server.OnTCPConnectionFailed += (server, timestamp, eventTrackingId, remoteSocket, connectionId, exception) => {

                Log.Warning(
                    $"A charging station at {remoteSocket} was dropped before it could sign in: {exception.Message}",
                    "ocpp", "station", "tls"
                );

                return Task.CompletedTask;

            };

            #endregion

            #region The OCPP messages themselves

            Server.OnJSONMessageReceived += (timestamp, server, connection, messageTimestamp, eventTrackingId, sourceNodeId, message, cancellationToken) => {

                LogOCPPMessage("<-", StationOn(connection), message);
                return Task.CompletedTask;

            };

            Server.OnJSONMessageSent += (timestamp, server, connection, messageTimestamp, eventTrackingId, message, sentStatus, cancellationToken) => {

                LogOCPPMessage("->", StationOn(connection), message);
                return Task.CompletedTask;

            };

            #endregion

            #region The pings that keep a connection open

            Server.OnPingMessageReceived += (timestamp, server, connection, frame, eventTrackingId, pingMessage, cancellationToken) => {

                if (ocppServerSettings.Logging?.Pings == true)
                    Log.Debug($"A ping came from the charging station '{StationOn(connection)}'.", "ocpp", "station", "ping");

                return Task.CompletedTask;

            };

            #endregion

        }

        #endregion

        #region (private) WireOCPPOverSSHLogging(Server)

        /// <summary>
        /// What of OCPP-over-SSH ends up in the event log: the same switches as
        /// the WebSocket port's, read inside the handlers for the same reason.
        /// </summary>
        /// <remarks>
        /// Who signed in, and who was refused at the door, the node's SSH
        /// server says itself; what is said here is the station connecting,
        /// which is a step later - after the HELLO that says which OCPP version
        /// it speaks - and going away.
        /// </remarks>
        private void WireOCPPOverSSHLogging(OCPPSshServer Server)
        {

            Server.OnNetworkingNodeConnectionAccepted += (timestamp, transport, connection, networkingNodeId, networkingMode, eventTrackingId, cancellationToken) => {

                if (ocppServerSettings.Logging?.Connections != false)
                    Log.Notice(
                        $"The charging station '{networkingNodeId?.ToString() ?? connection.RemoteSocket.ToString()}' is connected over SSH from {connection.RemoteSocket}" +
                        $" ({connection.Subprotocol ?? "no subprotocol"}).",
                        "ocpp", "station", "ssh"
                    );

                return Task.CompletedTask;

            };

            Server.OnNetworkingNodeConnectionClosed += (timestamp, transport, connection, networkingNodeId, close, cancellationToken) => {

                if (ocppServerSettings.Logging?.Connections != false)
                    Log.Notice(
                        $"The charging station '{networkingNodeId?.ToString() ?? connection.RemoteSocket.ToString()}' went away over SSH" +
                        $" ({(close.ByPeer ? "it closed" : "closed here")}{(close.Code.HasValue ? $", {close.Code}" : "")}{(close.Reason.IsNullOrEmpty() ? "" : $": {close.Reason}")}).",
                        "ocpp", "station", "ssh"
                    );

                return Task.CompletedTask;

            };

            Server.OnJSONMessageReceived += (timestamp, transport, connection, messageTimestamp, eventTrackingId, sourceNodeId, message, cancellationToken) => {

                LogOCPPMessage("<-", StationOn(connection), message);
                return Task.CompletedTask;

            };

            Server.OnJSONMessageSent += (timestamp, transport, connection, messageTimestamp, eventTrackingId, message, sentStatus, cancellationToken) => {

                LogOCPPMessage("->", StationOn(connection), message);
                return Task.CompletedTask;

            };

        }

        #endregion

        #region (private static) StationOn(Connection)

        /// <summary>
        /// Which charging station a connection is, as this controller let it
        /// in - or, for one it has not let in, where it comes from.
        /// </summary>
        protected static String StationOn(WebSocketServerConnection Connection)

            => Connection.TryGetCustomData(StationIdKey) as String ??
               (IsABackEnd(Connection)
                    ? cloud.charging.open.protocols.WWCP.NetworkingNode.WWCPConnectionExtensions.NetworkingNodeIdOf(Connection)?.ToString()
                    : null) ??
               Connection.RemoteSocket.ToString();

        /// <summary>
        /// Whether a connection of the station port was let in as a back end
        /// that connects to this controller, and not as a charging station.
        /// </summary>
        private static Boolean IsABackEnd(WebSocketServerConnection Connection)

            => Connection.TryGetCustomData(InboundConnectionKey) is String;

        /// <summary>
        /// What a connection of the station port is called in the log: the
        /// charging station, or the back end that connected.
        /// </summary>
        private static String WhoIsOn(WebSocketServerConnection Connection)

            => $"The {(IsABackEnd(Connection) ? "back end" : "charging station")} '{StationOn(Connection)}'";

        /// <summary>
        /// Which charging station a connection over SSH is: the networking
        /// node its HELLO was welcomed as - or where it comes from.
        /// </summary>
        protected static String StationOn(org.GraphDefined.Vanaheimr.Hermod.Messaging.IMessageConnection Connection)

            => Connection is WebSocketServerConnection webSocket
                   ? StationOn(webSocket)
                   : cloud.charging.open.protocols.WWCP.NetworkingNode.WWCPConnectionExtensions.NetworkingNodeIdOf(Connection)?.ToString() ?? Connection.RemoteSocket.ToString();

        #endregion

        #region (private) LogOCPPMessage(Direction, Connection, Message)

        /// <summary>
        /// One OCPP message in the log - by name only, or with what it carries.
        /// </summary>
        /// <remarks>
        /// <b>The contents are a separate switch, and a window rather than a
        /// setting.</b> An OCPP message carries identification tokens, RFID
        /// card numbers and meter readings: who charged, where and how much.
        /// The window running out is what switches it off again, and it is
        /// checked here, on every message, so that it goes off whether or not
        /// anybody is watching.
        ///
        /// An OCPP message is an array: the kind, the identification, the
        /// action, and then what it carries. So the first three can be logged
        /// without the fourth, which is the whole point of having two switches.
        /// </remarks>
        protected void LogOCPPMessage(String  Direction,
                                    String  Station,
                                    JArray  Message)
        {

            var logging = ocppServerSettings.Logging;

            if (logging?.Messages != true)
                return;

            var action  = Message.Count > 2 ? Message[2]?.Value<String>() : null;
            var id      = Message.Count > 1 ? Message[1]?.Value<String>() : null;
            var station = Station;

            if (logging.PayloadsAt(TimeProvider.GetUtcNow()))
                Log.Log(
                    LogLevel.Debug,
                    $"{Direction} {station}: {action ?? "?"} [{id ?? "?"}]",
                    new JObject(new JProperty("message", Message)),
                    "ocpp", "station", "message", "payload"
                );

            else
                Log.Debug(
                    $"{Direction} {station}: {action ?? "?"} [{id ?? "?"}]",
                    "ocpp", "station", "message"
                );

        }

        #endregion


        #region OCPPServerConfigurationJSON()

        /// <summary>
        /// The charging station server, as its Configuration page reads it.
        /// </summary>
        public JObject OCPPServerConfigurationJSON()
        {

            var settings = ocppServerSettings;
            var waiting  = ocppServerAsBuilt.NeedsARestartFor(settings);

            var json     = new JObject(

                new JProperty("enabled",                     settings.Enabled              == true),
                new JProperty("address",                     settings.Address?.ToString()),
                new JProperty("port",                        (settings.TCPPort ?? WebSocketServerConfiguration.DefaultTCPPort).ToUInt16()),
                new JProperty("clientAuthentication",        new JArray((settings.ClientAuthentication ?? []).Select(authentication => authentication.ToText()))),
                new JProperty("subprotocols",                new JArray(settings.Subprotocols ?? [])),
                new JProperty("reachableAs",                 new JArray(settings.ReachableAs  ?? [])),
                new JProperty("minTLSVersion",               WebSocketServerConfiguration.TLSVersionText(settings.MinimumTLSVersion ?? WebSocketServerConfiguration.DefaultMinimumTLSVersion)),
                new JProperty("checkCertificateRevocation",  settings.CheckCertificateRevocation == true),
                new JProperty("maxConnections",              settings.MaxConnections ?? WebSocketServerConfiguration.DefaultMaxConnections),
                new JProperty("pingEverySeconds",            (settings.PingEvery ?? WebSocketServerConfiguration.DefaultPingEvery).TotalSeconds),
                new JProperty("http3",                       settings.TakesHTTP3),

                new JProperty("ssh",                         new JObject(
                    new JProperty("enabled",                 settings.SSH?.Enabled == true),
                    new JProperty("subprotocols",            settings.SSH?.Subprotocols is IReadOnlyList<String> sshSubprotocols
                                                                 ? new JArray(sshSubprotocols)
                                                                 : null),
                    new JProperty("maxMessageSize",          settings.SSH?.MaxMessageSize ?? WebSocketServerSSH.DefaultMaxMessageSize),
                    new JProperty("pingEverySeconds",        (settings.SSH?.PingEvery ?? WebSocketServerSSH.DefaultPingEvery).TotalSeconds),
                    new JProperty("subsystem",               OCPPSshServer.OCPPSubsystem),
                    new JProperty("state",                   new JObject(
                        new JProperty("served",              ocppOverSSHServed),
                        new JProperty("sshServer",           SSHEnabled),
                        new JProperty("url",                 OCPPServerSSHURL),
                        new JProperty("hostKey",             SSHHostKey),
                        new JProperty("connections",         (ocppSshServer?.  ConnectedNetworkingNodeIds.Count() ?? 0) +
                                                             (ocppSshServer16?.ConnectedNetworkingNodeIds.Count() ?? 0)),
                        new JProperty("stationsWithAKey",    WebSocketLogins.Logins.Count(login => login.Enabled && login.HasSSHKey))
                    ))
                )),

                new JProperty("logging",                     new JObject(
                    new JProperty("connections",             settings.Logging?.Connections    != false),
                    new JProperty("authentication",          settings.Logging?.Authentication != false),
                    new JProperty("messages",                settings.Logging?.Messages       == true),
                    new JProperty("payloads",                settings.Logging?.Payloads       == true),
                    new JProperty("payloadsUntil",           settings.Logging?.PayloadsUntil?.ToString("o")),
                    new JProperty("payloadsNow",             settings.Logging?.PayloadsAt(TimeProvider.GetUtcNow()) == true),
                    new JProperty("pings",                   settings.Logging?.Pings          == true)
                )),

                new JProperty("state",                       new JObject(
                    new JProperty("running",                 ocppServerStarted),
                    new JProperty("tls",                     ocppServerTLS),
                    new JProperty("url",                     OCPPServerURL),
                    new JProperty("http3Port",               ocppWebSocketServer?.HTTP3Port?.ToUInt16()),
                    new JProperty("http3Problem",            WhyNoHTTP3(settings)),
                    new JProperty("connections",             StationServers.Sum(server => server.WebSocketConnections.Count())),
                    new JProperty("webSocketLogins",           WebSocketLogins.EnabledCount),
                    new JProperty("trustedChains",           ClientTrust.EnabledCount),
                    new JProperty("hasCertificate",          OCPPServerHasCertificate),

                    // What a charging station pins to reach this port, and what
                    // to hand it before a switch (EVQI Transport, key pins, 7.4).
                    new JProperty("keyPin",                  OCPPServerKeyPin?.    ToHex()),
                    new JProperty("nextKeyPin",              OCPPServerNextKeyPin?.ToHex()),

                    // What a change to the socket is waiting for, said rather
                    // than left to be discovered.
                    new JProperty("waitingForARestart",      new JArray(waiting))
                )),

                new JProperty("limits",                      new JObject(
                    new JProperty("subprotocols",            new JArray(WebSocketServerConfiguration.KnownSubprotocols)),
                    new JProperty("clientAuthentication",    new JArray(WebSocketClientAuthentications.Names)),
                    new JProperty("tlsVersions",             new JArray("1.2", "1.3")),
                    new JProperty("maxConnections",          WebSocketServerConfiguration.MaxMaxConnections),
                    new JProperty("maxReachableAs",          WebSocketServerConfiguration.MaxReachableAs),
                    new JProperty("minMaxMessageSize",       WebSocketServerSSH.MinMaxMessageSize),
                    new JProperty("maxMaxMessageSize",       WebSocketServerSSH.MaxMaxMessageSize),
                    new JProperty("suggestedPayloadWindowSeconds", WebSocketServerLogging.SuggestedPayloadWindow.TotalSeconds)
                )),

                new JProperty("file",                        ConfigFile.Path)

            );

            // What a kind of node says of its server beside this - a CSMS how
            // many of the nodes below it it connects to.
            OCPPServerStateJSON((JObject) json["state"]!);

            return json;

        }

        /// <summary>
        /// Add what a kind of node says of the state of its charging station
        /// server to what <see cref="OCPPServerConfigurationJSON"/> says. Nothing
        /// by default.
        /// </summary>
        /// <param name="State">The "state" object, to add to.</param>
        protected virtual void OCPPServerStateJSON(JObject State)
        { }

        /// <summary>
        /// A charging station that signed in with a client certificate naming
        /// it: what a kind of node remembers of it - a CSMS, what runs out, to
        /// ask for a new one before it does. Nothing by default.
        /// </summary>
        /// <param name="StationId">What the certificate names it.</param>
        /// <param name="Certificate">The certificate it signed in with.</param>
        protected virtual void StationSignedInWithCertificate(String            StationId,
                                                              X509Certificate2  Certificate)
        { }

        /// <summary>
        /// A charging station whose login was removed: what a kind of node
        /// forgets of it beside - a CSMS, the certificates it knows of it.
        /// </summary>
        /// <param name="Login">The login of the charging station.</param>
        /// <returns>What was forgotten beside, said as the end of a sentence (", and ..."), or nothing.</returns>
        protected internal virtual String StationLoginRemoved(User_Id Login)
            => "";

        #endregion

        #region (private) TakeEffectAfter(Answered, SwitchedOnOrOff, SSHOnOrOff, HTTP3OnOrOff)

        /// <summary>
        /// Switch the server, OCPP-over-SSH and HTTP/3 on or off, and open the
        /// port anew where its encryption changed, as the settings say by
        /// then, once the given task is done: what a change that came
        /// over a connection of this server waits for, so that its answer is
        /// out before the connection may go.
        /// </summary>
        /// <remarks>
        /// By the settings at that time rather than those of the change: a
        /// second change in between is what holds, and each step does nothing
        /// where it is done already.
        /// </remarks>
        /// <param name="Answered">What it waits for.</param>
        /// <param name="SwitchedOnOrOff">Whether the change switched the server on or off.</param>
        /// <param name="SSHOnOrOff">Whether it switched OCPP-over-SSH on or off.</param>
        /// <param name="HTTP3OnOrOff">Whether it switched HTTP/3 on or off.</param>
        private async Task TakeEffectAfter(Task     Answered,
                                           Boolean  SwitchedOnOrOff,
                                           Boolean  SSHOnOrOff,
                                           Boolean  HTTP3OnOrOff)
        {

            try
            {

                await Answered.ConfigureAwait(false);

                await reconfigureLock.WaitAsync().ConfigureAwait(false);

                try
                {

                    var settings = ocppServerSettings;

                    if (SwitchedOnOrOff && ocppWebSocketServer is not null)
                    {

                        if (settings.Enabled == true)
                            await StartOCPPServer().ConfigureAwait(false);

                        else
                            await StopOCPPServer(). ConfigureAwait(false);

                    }

                    // Opened anew, it serves OCPP-over-SSH and HTTP/3 as the
                    // settings say as well.
                    else if (EncryptsOtherwiseThan(settings))
                        await OpenTheStationPortAnew().ConfigureAwait(false);

                    else if (SSHOnOrOff && ocppServerStarted)
                    {

                        if (settings.SSH?.Enabled == true)
                            ServeOCPPOverSSH();

                        else
                            StopServingOCPPOverSSH("it was switched off");

                    }

                    if (HTTP3OnOrOff && ocppServerStarted)
                        TakeHTTP3OrNot(settings);

                }
                finally
                {
                    reconfigureLock.Release();
                }

            }
            catch (Exception e)
            {
                Log.Warning($"The charging station server could not be switched as it was reconfigured: {e.Message}", "ocpp", "station", "config");
            }

        }

        #endregion

        #region (private) WhyNoHTTP3(Settings)

        /// <summary>
        /// Why the station port takes no WebSockets over HTTP/3 although the
        /// settings ask for it, or null: while it takes them, while it was not
        /// asked to, and while it does not run.
        /// </summary>
        private String? WhyNoHTTP3(WebSocketServerConfiguration Settings)

            => ocppServerStarted                       &&
               Settings.TakesHTTP3                     &&
               ocppWebSocketServer is not null         &&
               ocppWebSocketServer.HTTP3Port is null
                   ? ocppServerTLS
                         ? ocppWebSocketServer.WhyNoHTTP3
                         : NoHTTP3WithoutTLS
                   : null;

        #endregion

        #region TryUpdateOCPPServerConfiguration(JSON, out Error)

        /// <summary>
        /// Change the charging station server.
        /// </summary>
        /// <remarks>
        /// Written to the file first and applied afterwards, as everywhere
        /// else here: a change that was applied but not written down is a
        /// change that disappears at the next start without anybody noticing.
        ///
        /// What can be put into effect now is; what cannot is reported through
        /// <c>waitingForARestart</c> rather than quietly doing nothing.
        /// </remarks>
        public Boolean TryUpdateOCPPServerConfiguration(JObject                           JSON,
                                                        [NotNullWhen(false)] out String?  Error)

            => TryUpdateOCPPServerConfiguration(JSON, out Error, out _);

        /// <summary>
        /// Change the charging station server - and say whether a refusal was
        /// the configuration file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the configuration file could not be read or written: nothing about the change was wrong, and nothing was changed.</param>
        /// <param name="TakesEffectAfter">What switching the server, HTTP/3 or OCPP-over-SSH on or off waits for - the response to the request that asked for it, which a connection dropped before would take with it; at once where null. Everything else is written and in effect when this returns.</param>
        public Boolean TryUpdateOCPPServerConfiguration(JObject                           JSON,
                                                        [NotNullWhen(false)] out String?  Error,
                                                        out Boolean                       NotSaved,
                                                        Task?                             TakesEffectAfter   = null)
        {

            NotSaved = false;

            if (!WebSocketServerConfiguration.TryParse(JSON, out var update, out Error, NodeNoun))
                return false;

            #region What cannot be saved because it would be a port nobody can use

            if (update.ClientAuthentication is { Count: > 0 } authentications &&
                authentications.All(authentication => authentication.IsOverTLS()) &&
                !OCPPServerHasCertificate)
            {
                Error = "HTTP authentication over TLS and a TLS client certificate (security profiles 2 and 3) both need TLS, and there is no server certificate yet. " +
                        $"Make a TLS server identity for the listener '{StationListener}' on the Server identities page, or a server " +
                        "key of it for the charging stations to pin, or allow HTTP authentication without TLS as well.";
                return false;
            }

            if (update.Logging?.Payloads == true &&
                update.Logging.PayloadsUntil <= TimeProvider.GetUtcNow())
            {
                Error = $"The window for logging message contents ends at {update.Logging.PayloadsUntil:yyyy-MM-dd HH:mm}'Z', " +
                        $"which by this {NodeNoun}'s clock has already passed - it is now {TimeProvider.GetUtcNow():yyyy-MM-dd HH:mm}'Z'.";
                return false;
            }

            #endregion

            reconfigureLock.Wait();

            try
            {

                // Everything this refuses is the file's: read, merged, written.
                if (!ConfigFile.TryMergeSection(WebSocketServerConfiguration.SectionName, update.ToJSON(), out Error))
                {
                    NotSaved = true;
                    return false;
                }

                var before    = ocppServerSettings;
                var reopened  = false;

                ocppServerSettings = before.Merge(update).Effective();

                #region What takes effect once the request that asked for it is answered

                if (TakesEffectAfter is not null)
                {

                    var switchedOnOrOff  = before.Enabled      != ocppServerSettings.Enabled;
                    var sshOnOrOff       = before.SSH?.Enabled != ocppServerSettings.SSH?.Enabled;
                    var http3OnOrOff     = before.TakesHTTP3   != ocppServerSettings.TakesHTTP3;

                    if (switchedOnOrOff || sshOnOrOff || http3OnOrOff || EncryptsOtherwiseThan(ocppServerSettings))
                        _ = TakeEffectAfter(TakesEffectAfter, switchedOnOrOff, sshOnOrOff, http3OnOrOff);

                }

                #endregion

                #region What takes effect at once

                else if (ocppWebSocketServer is not null &&
                    before.Enabled != ocppServerSettings.Enabled)
                {

                    if (ocppServerSettings.Enabled == true)
                        StartOCPPServer().GetAwaiter().GetResult();

                    else
                        StopOCPPServer(). GetAwaiter().GetResult();

                }

                // A way in over TLS allowed where the port was unencrypted, or
                // only security profile 1 left where it spoke TLS: the port is
                // opened anew, and with it OCPP-over-SSH and HTTP/3 as they
                // are now.
                else if (EncryptsOtherwiseThan(ocppServerSettings))
                {
                    OpenTheStationPortAnew().GetAwaiter().GetResult();
                    reopened = true;
                }

                // OCPP-over-SSH on and off while the station server runs: the
                // subsystem comes and goes, and its sessions end with it.
                else if (ocppServerStarted &&
                         before.SSH?.Enabled != ocppServerSettings.SSH?.Enabled)
                {

                    if (ocppServerSettings.SSH?.Enabled == true)
                        ServeOCPPOverSSH();

                    else
                        StopServingOCPPOverSSH("it was switched off");

                }

                // HTTP/3 on and off while the station server runs: the UDP
                // port opens and closes, and what is on it goes with it.
                if (TakesEffectAfter is null &&
                    !reopened                &&
                    ocppServerStarted        &&
                    before.Enabled    == ocppServerSettings.Enabled &&
                    before.TakesHTTP3 != ocppServerSettings.TakesHTTP3)
                {
                    TakeHTTP3OrNot(ocppServerSettings);
                }

                #endregion

                var waiting = ocppServerAsBuilt.NeedsARestartFor(ocppServerSettings);

                Log.Notice(
                    $"The charging station server was reconfigured: {ocppServerSettings}." +
                    (waiting.Count > 0
                         ? $" {String.Join(", ", waiting)} take effect at the next start."
                         : ""),
                    "ocpp", "station", "config"
                );

                return true;

            }
            finally
            {
                reconfigureLock.Release();
            }

        }

        #endregion

    }

}
