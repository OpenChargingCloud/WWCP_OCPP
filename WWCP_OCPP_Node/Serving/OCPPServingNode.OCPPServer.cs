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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.OCPP.SSH;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;

using OCPPWebSockets = cloud.charging.open.protocols.OCPP.WebSockets;
using OCPPVersion    = cloud.charging.open.protocols.OCPP.Node.Connections.OCPPVersion;

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
    /// <b>And the CSMS, where it is the one that dials.</b> A CSMS this node
    /// cannot dial dials the node, on this same port: written down as a
    /// connection the other end makes, let in under the name its URL ends with
    /// by the credentials it names - and from then on the line up of this
    /// node, not a charging station of it. See
    /// <see cref="ServeTheBackEndsThatDial"/> and <see cref="ValidateStation"/>.
    ///
    /// <b>And over SSH, where somebody switches it on.</b> The charging
    /// stations may speak OCPP over the SSH server of this node as well,
    /// as the subsystem "ocpp@open.charging.cloud", signing in with an SSH key
    /// of their own - see <see cref="OCPPServerSSH"/> and
    /// <see cref="StationSSHSignIns"/>. That is served while the station
    /// server is, and switched on and off without a restart.
    ///
    /// <b>Moved here from the LocalController library</b>, as the dialling
    /// moved into OCPPDiallingNode before it, so that a gateway has the same
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
        private OCPPServerConfiguration                    ocppServerSettings  = new OCPPServerConfiguration().Effective();

        /// <summary>
        /// What it was configured as when it was built, to tell a change that
        /// is in effect from one that is waiting for a restart.
        /// </summary>
        private OCPPServerConfiguration                    ocppServerAsBuilt   = new OCPPServerConfiguration().Effective();

        private OCPPWebSockets.OCPPWebSocketServer?        ocppWebSocketServer;

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
        private Boolean                                    ocppServerTLS;

        private Boolean                                    ocppServerStarted;

        /// <summary>
        /// What makes this controller look at its certificates without being
        /// asked.
        /// </summary>
        /// <remarks>
        /// The whole value of uploading a replacement weeks early is somebody
        /// being told weeks early. Checking only when a page is opened would
        /// mean the warning arrives when somebody was already looking - which
        /// is the one moment it is not needed.
        ///
        /// On this controller's own <see cref="TimeProvider"/> rather than a
        /// bare timer, so that a test which moves the clock moves this too.
        /// </remarks>
        private ITimer?                                    certificateCheckTimer;

        /// <summary>
        /// How often the certificates are looked at. Daily: what is being
        /// watched for moves at the speed of a calendar.
        /// </summary>
        public static readonly TimeSpan CertificateCheckEvery = TimeSpan.FromHours(24);

        /// <summary>
        /// Where a connection keeps which charging station this controller let
        /// in on it.
        /// </summary>
        /// <remarks>
        /// Kept by this controller rather than read from the connection's
        /// Login. The server leaves the checking to <see cref="ValidateStation"/>,
        /// and Hermod sets a Login only where it authenticated the upgrade
        /// itself - so to Hermod every one of these connections is anonymous,
        /// and the log said "'?' is connected" of a station it had just let in.
        /// </remarks>
        protected const String StationIdKey = "ocppServer.chargingStation";

        /// <summary>
        /// The server of OCPP over SSH: built with the station server, and
        /// handed to the node's SSH server while it is served.
        /// </summary>
        private OCPPSshServer?                             ocppSshServer;

        /// <summary>
        /// The charging stations, as logins of the node's SSH server.
        /// </summary>
        private StationSSHSignIns?                         stationSSHSignIns;

        /// <summary>
        /// Whether OCPP over SSH is served right now.
        /// </summary>
        private volatile Boolean                           ocppOverSSHServed;

        /// <summary>
        /// What OCPP over SSH is called in what the SSH server says: who, the
        /// SSH page, the log.
        /// </summary>
        public const String OCPPOverSSHLabel = "OCPP";

        #endregion

        #region Properties

        /// <summary>
        /// The keys and certificates this local controller presents to the
        /// charging stations.
        /// </summary>
        public ServerCertificateStore   ServerCertificates    { get; private set; } = default!;

        /// <summary>
        /// Which chains a charging station's own certificate may lead to.
        /// </summary>
        public ClientTrustStore         ClientTrust           { get; private set; } = default!;

        /// <summary>
        /// Which charging stations may sign in, and with what.
        /// </summary>
        public ChargingStationLogins    StationLogins         { get; private set; } = default!;

        /// <summary>
        /// The server itself, or null when this controller was built without
        /// one.
        /// </summary>
        public OCPPWebSockets.OCPPWebSocketServer?  StationServer
            => ocppWebSocketServer;

        /// <summary>
        /// What the charging station server is configured as.
        /// </summary>
        public OCPPServerConfiguration  OCPPServerSettings
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
                                                "it needs a server certificate and security profile 2 or 3.";

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
                   ? ServerCertificates.CurrentKey?.KeyPin
                   : null;

        /// <summary>
        /// The key pin of the announced next key, to be handed to the charging
        /// stations before the switch - or null where none was announced.
        /// </summary>
        public KeyPin? OCPPServerNextKeyPin
            => ServerCertificates.NextKey?.KeyPin;

        /// <summary>
        /// Where the charging stations are told to connect.
        /// </summary>
        public String OCPPServerURL

            => $"{(ocppServerTLS ? "wss" : "ws")}://" +
               $"{(ocppServerSettings.ReachableAs is { Count: > 0 } names ? names[0] : ocppServerSettings.Address?.ToString() ?? "0.0.0.0")}:" +
               $"{ocppServerSettings.TCPPort ?? OCPPServerConfiguration.DefaultTCPPort}";

        /// <summary>
        /// The server of OCPP over SSH, or null when this controller was built
        /// without a station server.
        /// </summary>
        public OCPPSshServer?  StationSSHServer
            => ocppSshServer;

        /// <summary>
        /// Whether the charging stations may connect over SSH right now: the
        /// station server is running, OCPP over SSH is switched on, and the
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
        protected void BuildOCPPServer(OCPPServerConfiguration? Configuration)
        {

            var directory = Path.GetDirectoryName(ConfigFile.Path) ?? ".";

            if (Configuration?.WhatOneServerCannotServe() is String cannotServe)
                throw new InvalidOperationException(cannotServe);

            ocppServerSettings  = (Configuration ?? new OCPPServerConfiguration()).Effective();
            ocppServerAsBuilt   = ocppServerSettings;

            #region The three stores, each saying what it finds

            ServerCertificates = new ServerCertificateStore(
                                     Path.Combine(directory, ServerCertificateStore.DefaultDirectoryName),
                                     TimeProvider
                                 );

            ServerCertificates.OnNotice += (level, message) => Log.Log(level, message, "ocpp", "tls");

            // Every switch, from wherever it was asked for, told to the CSMS.
            ServerCertificates.OnSwitched += ServerKeySwitched;

            ClientTrust        = new ClientTrustStore(
                                     Path.Combine(directory, ClientTrustStore.DefaultDirectoryName),
                                     TimeProvider
                                 );

            ClientTrust.OnNotice += (level, message) => Log.Log(level, message, "ocpp", "tls", "trust");

            // Their owner's alone, as the node keeps its accounts and its own
            // certificates: a key read by somebody else is somebody else's
            // server, and a chain added by somebody else lets their charging
            // stations in. What an older controller left open is taken back
            // here, once somebody listens to what cannot be done.
            ServerCertificates.KeepToItsOwner();
            ClientTrust.       KeepToItsOwner();

            StationLogins      = new ChargingStationLogins(
                                     Path.Combine(directory, ChargingStationLogins.DefaultFileName),
                                     TimeProvider
                                 );

            StationLogins.OnNotice += (level, message) => Log.Log(level, message, "ocpp", "station", "auth");

            // A file that is there but unreadable is not papered over with an
            // empty list: that would turn every charging station on the site
            // away while looking like nothing was wrong.
            if (!StationLogins.TryLoad(out var loginsError))
                throw new InvalidOperationException($"{loginsError} Repair or remove it and start again.");

            StationLogins.OnChanged += () => {

                if (ocppWebSocketServer is not null)
                    ApplyStationLogins(ocppWebSocketServer);

                // Over SSH, a station whose key was taken away or that was
                // switched off is signed out now rather than at the next
                // round of the node's own rechecks.
                RecheckSSHSessions();

            };

            #endregion

            #region The server

            var settings = ocppServerSettings;

            ocppWebSocketServer = Node21.AttachWebSocketServer(

                                      HTTPServiceName:              $"OpenChargingCloud {Kind.Product} v{Version}",
                                      IPAddress:                    settings.Address,
                                      TCPPort:                      settings.TCPPort,
                                      Description:                  I18NString.Create($"The charging stations of this {NodeWording.Noun}"),

                                      // Off, because this server does the
                                      // deciding itself: a station on security
                                      // profile 3 authenticates with its
                                      // certificate and has no password to send,
                                      // and the check built in here cannot tell
                                      // the two apart. See ValidateStation.
                                      RequireAuthentication:        false,
                                      SecWebSocketProtocols:        settings.Subprotocols,
                                      DisableWebSocketPings:        false,
                                      WebSocketPingEvery:           settings.PingEvery,

                                      ClientCertificateValidator:   ValidateStationCertificate,
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

            // A station's certificate let in by its key pin names nobody: the
            // station is the one the pin is listed for, and its path has to say
            // so - see ValidateStation. Told to the server as well, or it would
            // read the common name, register the connection under it, and turn
            // a station whose certificate names another away with a 403. The
            // same for a back end that dials and signs in with a certificate
            // its connection knows by its key.
            ocppWebSocketServer.AcceptedByKeyPin = certificate => StationLogins.FindKeyPin(KeyPin.FromCertificate(certificate)) is not null ||
                                                                  IsInboundKeyPinned(OCPPVersion.OCPP2_1, certificate);

            ocppWebSocketServer.OnValidateWebSocketConnection += ValidateStation;

            // A WebSocket server of WWCP takes HTTP/3 unless told otherwise:
            // here what the file says from the start, and whether the port is
            // encrypted, which HTTP/3 needs, once it starts.
            ocppWebSocketServer.AcceptHTTP3 = settings.TakesHTTP3;

            ApplyStationLogins(ocppWebSocketServer);
            WireOCPPServerLogging(ocppWebSocketServer);
            FollowStationsSeen(ocppWebSocketServer);

            #endregion

            #region OCPP over SSH, on the node's SSH server

            // Built here, as the WebSocket server is, with what it is
            // configured as at the start; served or not while running.
            ocppSshServer = Node21.AttachOCPPSshSubsystem(
                                Subsystem:       OCPPSshServer.OCPPSubsystem,
                                Subprotocols:    settings.SSH?.Subprotocols ?? settings.Subprotocols,
                                MaxMessageSize:  settings.SSH?.MaxMessageSize ?? OCPPServerSSH.DefaultMaxMessageSize,
                                PingEvery:       settings.SSH?.PingEvery      ?? OCPPServerSSH.DefaultPingEvery
                            );

            WireOCPPOverSSHLogging(ocppSshServer);
            FollowStationsSeen(ocppSshServer);

            stationSSHSignIns = new StationSSHSignIns(
                                    StationLogins,
                                    OCPPSshServer.OCPPSubsystem,
                                    () => ocppOverSSHServed,
                                    TimeProvider,
                                    () => SSHSubsystems.Select(subsystem => subsystem.Name)
                                );

            stationSSHSignIns.OnNotice += (level, message) => {
                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Log(level, message, "ocpp", "station", "auth", "ssh");
            };

            AddSSHSignIns(stationSSHSignIns);

            #endregion

            Log.Info(
                settings.Enabled == true
                    ? $"The charging station server will listen on {settings.Address}:{settings.TCPPort}, " +
                      $"speaking {String.Join(" and ", settings.Subprotocols ?? [])}, " +
                      $"security profile(s) {String.Join(", ", settings.SecurityProfiles ?? [])}."
                    : "The charging station server is switched off; no charging station can connect. " +
                      "Switch it on under Configuration to let them in.",
                "ocpp", "station"
            );

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
            var wantsTLS   = settings.SecurityProfiles?.Any(profile => profile > 1) == true;

            #region Encrypted or not, decided here and for good

            ocppServerTLS = wantsTLS && ServerCertificates.HasCertificate;

            if (ocppServerTLS)
            {

                ocppWebSocketServer.ServerCertificateChainSelector = (server, client) => ServerCertificates.Select();

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

                    var chain = ServerCertificates.Select();

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
                        "Security profile 2 or 3 is configured, but there is no server certificate - so the charging station port is " +
                        "unencrypted and only security profile 1 can be used on it. Make a signing request and upload the certificate " +
                        "that answers it, or make a server key for the stations to pin; the port encrypts from the next start.",
                        "ocpp", "station", "tls"
                    );

                else if (ServerCertificates.HasCertificate)
                    Log.Notice(
                        "Only security profile 1 is allowed, so the charging station port is unencrypted and the server certificate is not presented.",
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
                          settings.TCPPort ?? OCPPServerConfiguration.DefaultTCPPort,
                          problem,
                          StationServerPort
                      );
            }

            ocppServerStarted = true;

            Log.Notice(
                $"The charging stations connect to {OCPPServerURL} " +
                $"({(ocppServerTLS ? "TLS" : "unencrypted")}, security profile(s) {String.Join(", ", settings.SecurityProfiles ?? [])}, " +
                $"{StationLogins.EnabledCount} station login(s), {ClientTrust.EnabledCount} accepted chain(s))" +
                (OCPPServerKeyPin     is KeyPin keyPin  ? $", key pin {keyPin}"           : "") +
                (OCPPServerNextKeyPin is KeyPin nextPin ? $", next key pin {nextPin}"     : "") +
                (ocppWebSocketServer.AcceptHTTP3        ? $" - over HTTP/3 as well, on UDP port {settings.TCPPort ?? OCPPServerConfiguration.DefaultTCPPort}" : "") +
                ".",
                "ocpp", "station"
            );

            if (settings.SSH?.Enabled == true)
                ServeOCPPOverSSH();

            ServeTheBackEndsThatDial();

            #region What is about to run out, now and every day after this

            CheckCertificates();

            certificateCheckTimer?.Dispose();
            certificateCheckTimer = TimeProvider.CreateTimer(
                                        _ => CheckCertificates(),
                                        null,
                                        CertificateCheckEvery,
                                        CertificateCheckEvery
                                    );

            if (ocppServerTLS)
                ServerCertificates.Select();

            #endregion

            #region The two ways to have a port nobody can come through

            if (!ocppServerTLS && StationLogins.EnabledCount == 0)
                Log.Warning(
                    "No charging station has a login here, so none can sign in. Add them under Configuration.",
                    "ocpp", "station", "auth"
                );

            if (settings.SecurityProfiles?.Contains<Byte>(3) == true &&
                ClientTrust.EnabledCount == 0 &&
                !StationLogins.Logins.Any(login => login.HasKeyPin))
                Log.Warning(
                    "Security profile 3 is allowed, but no certificate authority has been named to accept charging stations from, " +
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
        private void TakeHTTP3OrNot(OCPPServerConfiguration Settings)
        {

            if (ocppWebSocketServer is null)
                return;

            ocppWebSocketServer.AcceptHTTP3 = Settings.TakesHTTP3 && ocppServerTLS;

            if (Settings.TakesHTTP3 && !ocppServerTLS)
                Log.Warning(
                    $"The charging station port takes no WebSockets over HTTP/3, although the configuration says it shall: {NoHTTP3WithoutTLS}",
                    "ocpp", "station", "tls"
                );

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

            StopServingTheBackEndsThatDial($"The station server of this {NodeWording.Noun} stopped.");

            await ocppWebSocketServer.Shutdown($"The {NodeWording.Noun} is shutting down.");

            Log.Info("The charging station server has stopped listening.", "ocpp", "station");

        }

        #endregion

        #region (private) ServeTheBackEndsThatDial() / StopServingTheBackEndsThatDial(Why)

        /// <summary>
        /// Let in, over the station port, the back ends that dial this
        /// controller: the connections the other end makes, where this
        /// controller reports to a CSMS at all.
        /// </summary>
        /// <remarks>
        /// The port is the charging stations' and stays theirs: who signs in
        /// there is decided in <see cref="ValidateStation"/>, which asks first
        /// whether somebody comes under the name of such a connection. Nothing
        /// of it is the server's to know.
        /// </remarks>
        private void ServeTheBackEndsThatDial()
        {

            if (ocppWebSocketServer is null || !ocppServerStarted || !LetsBackEndsDialIn)
                return;

            ServeInbound(OCPPVersion.OCPP2_1, ocppWebSocketServer, SharedWithOthers: true);

            var written = Connections.Connections.Count(connection => connection.Inbound);

            if (written > 0)
                Log.Notice($"{written} connection{(written == 1 ? "" : "s")} of this {NodeWording.Noun} {(written == 1 ? "is" : "are")} made by the other end: " +
                           $"a CSMS dials {OCPPServerURL}<its name>, the port of the charging stations.",
                           "ocpp", "csms");

        }

        /// <summary>
        /// Let no back end in any more, and let go of those that are.
        /// </summary>
        private void StopServingTheBackEndsThatDial(String Why)

            => StopServingInbound(OCPPVersion.OCPP2_1, Why);

        #endregion

        #region TryAddStationSSHKey(Id, Line, Label, GroupId, Note, CreatedBy, out Key, out Error, out NotSaved)

        /// <summary>
        /// Give a charging station an SSH key for OCPP over SSH - adding the
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
        /// <param name="Note">What this station is, for whoever reads the list later.</param>
        /// <param name="CreatedBy">Who added the key, if anybody said.</param>
        /// <param name="Key">The key, as it was added.</param>
        /// <param name="Error">Why it was refused.</param>
        /// <param name="NotSaved">True where the file could not be written.</param>
        public Boolean TryAddStationSSHKey(String                                Id,
                                           String?                               Line,
                                           String?                               Label,
                                           String?                               GroupId,
                                           String?                               Note,
                                           String?                               CreatedBy,
                                           [NotNullWhen(true)]  out UserSSHKey?  Key,
                                           [NotNullWhen(false)] out String?      Error,
                                           out Boolean                           NotSaved)
        {

            Key       = null;
            NotSaved  = false;

            var id = Id?.Trim() ?? "";

            if (User_Id.TryParse(id) is User_Id userId &&
                ExtAPI.TryGetUser(userId, out var user) &&
                user is not null)
            {
                Error = $"'{id}' is an account of this {NodeWording.Noun}. A charging station of the same name could not sign in over SSH: " +
                         "both would sign in under one name, and a name that is both is let in as neither.";
                return false;
            }

            return StationLogins.TryAddSSHKey(id, Line, Label, GroupId, Note, CreatedBy, out Key, out Error, out NotSaved);

        }

        #endregion

        #region (private) ServeOCPPOverSSH() / StopServingOCPPOverSSH(Why)

        /// <summary>
        /// Serve OCPP over SSH from now on: the subsystem on the node's SSH
        /// server, and the stations with an SSH key let in to it.
        /// </summary>
        /// <remarks>
        /// Said out loud where it cannot work: OCPP over SSH switched on with
        /// the node's SSH server off is a door painted on a wall.
        /// </remarks>
        private void ServeOCPPOverSSH()
        {

            if (ocppSshServer is null || ocppOverSSHServed)
                return;

            // Before the subsystem is there, so that a station asking for it
            // the moment it is finds itself let in.
            ocppOverSSHServed = true;

            try
            {
                AddSSHSubsystem(new SSHSubsystem(OCPPSshServer.OCPPSubsystem, OCPPOverSSHLabel, ocppSshServer.Handler));
            }
            catch (Exception e)
            {

                ocppOverSSHServed = false;

                Log.Critical($"OCPP over SSH could not be served: {e.Message}", "ocpp", "station", "ssh");
                return;

            }

            var stations = StationLogins.Logins.Count(login => login.Enabled && login.HasSSHKey);
            var withTotp = StationLogins.Logins.Count(login => login.Enabled && login.HasTOTP);

            if (!SSHEnabled)
                Log.Warning(
                    $"OCPP over SSH is switched on, but the SSH server of this {NodeWording.Noun} is off - so no charging station can reach it " +
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
        /// Serve OCPP over SSH no longer: the subsystem goes, and every
        /// session of it ends.
        /// </summary>
        /// <param name="Why">Why, for the log.</param>
        private void StopServingOCPPOverSSH(String Why)
        {

            if (!ocppOverSSHServed)
                return;

            ocppOverSSHServed = false;

            RemoveSSHSubsystem(OCPPSshServer.OCPPSubsystem);

            Log.Info($"OCPP over SSH is no longer served: {Why}.", "ocpp", "station", "ssh");

        }

        #endregion

        #region (private) CheckCertificates()

        /// <summary>
        /// Look at every certificate and every accepted chain, and say what is
        /// about to run out.
        /// </summary>
        /// <remarks>
        /// Both sides, because both sides take a site down when they expire:
        /// this controller's own certificate stops being believed by the
        /// charging stations, and an expired trust anchor stops this controller
        /// believing them.
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
                ServerCertificates.CheckExpiry(ocppServerSettings.ReachableAs ?? []);
                ClientTrust.       CheckExpiry();
            }
            catch (Exception e)
            {
                Log.Exception(e, "The certificates of the charging station server could not be checked.", "ocpp", "tls");
            }

        }

        #endregion

        #region (private) ValidateStation(...)

        /// <summary>
        /// Whether this charging station may come in - the OCPP security
        /// profiles and the login groups, decided in one place.
        /// </summary>
        /// <remarks>
        /// <b>Three ways in, and each is checked twice.</b> A certificate, a
        /// password or a one-time token; and whichever it is, the server as a
        /// whole has to allow the security profile it arrives on, and the group
        /// the login belongs to has to allow both the profile and the method.
        /// The group can only narrow what the server allows, never widen it.
        ///
        /// <b>Profile 3 first.</b> A station that authenticated with a
        /// certificate has already been checked against the accepted chains by
        /// the time this runs; it needs no password and is not asked for one.
        /// If it is also listed as a login, its group has its say - which is
        /// how a certificate station is put under the same rules as the rest.
        /// If it is not listed, the accepted chains are the whole of the
        /// decision, as they were before there were groups.
        ///
        /// <b>Then a password or a token</b>, which are the same question over
        /// the same transport: is this the credential this identification
        /// should be showing? What decides the profile is whether the port is
        /// encrypted, and that was settled when the server started. TOTP is not
        /// an OCPP security profile at all, so it is judged under the profile
        /// its transport would have had.
        ///
        /// A refusal says as little as it can to the station and as much as it
        /// can to the log. "Unauthorized" back over the wire; which
        /// identification it was, where it came from and what was wrong with it
        /// into the log, where somebody who is allowed to know may read it.
        /// </remarks>
        private Task<HTTPResponse?> ValidateStation(DateTimeOffset             Timestamp,
                                                    AWebSocketEndpoint         Server,
                                                    WebSocketServerConnection  Connection,
                                                    EventTracking_Id           EventTrackingId,
                                                    CancellationToken          CancellationToken)
        {

            var profiles  = ocppServerSettings.SecurityProfiles ?? OCPPServerConfiguration.DefaultSecurityProfiles;
            var from      = Connection.RemoteSocket.ToString();

            #region A back end that dials: a connection the other end makes

            // Whoever comes under the name of a connection the other end makes
            // is let in as that or not at all, by what that connection names -
            // before anything a charging station is asked, and whatever it
            // shows besides: its CSMS, or a controller above this one, is no
            // charging station of this controller. Held to the security
            // profiles of this port all the same: a password over a port that
            // may not take one is not taken from a CSMS either, and a client
            // certificate is security profile 3, as it is for a station.
            if (SignInInbound(OCPPVersion.OCPP2_1, Connection) is InboundSignIn backEnd)
            {

                var asProfile = (Byte) (backEnd.ByCertificate ? 3 : ocppServerTLS ? 2 : 1);

                if (!backEnd.LetIn)
                    return Refuse(Connection, backEnd.Why);

                if (!profiles.Contains(asProfile))
                    return Refuse(Connection, backEnd.ByCertificate
                                                  ? $"'{backEnd.Name}' is a back end that dials this {NodeWording.Noun}, and signed in with {backEnd.Why}, " +
                                                     "which is security profile 3, not allowed here"
                                                  : $"'{backEnd.Name}' is a back end that dials this {NodeWording.Noun}, and signed in with {backEnd.Why} on " +
                                                    $"{(ocppServerTLS ? "an encrypted" : "an unencrypted")} port, which is security profile {asProfile}, not allowed here");

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Info($"The back end '{backEnd.Name}' dialled this {NodeWording.Noun} from {from} and signed in with {backEnd.Why} (security profile {asProfile}" +
                             $"{(ocppServerTLS || backEnd.ByCertificate ? "" : ", on an unencrypted port, so what it signed in with crossed the network in the clear")}).",
                             "ocpp", "csms", "auth");

                return Task.FromResult<HTTPResponse?>(null);

            }

            #endregion

            #region Security profile 3: it brought a certificate

            if (Connection.ClientCertificate is not null)
            {

                if (!profiles.Contains<Byte>(3))
                    return Refuse(Connection, "it authenticated with a certificate, and security profile 3 is not allowed here");

                var keyPin = KeyPin.FromCertificate(Connection.ClientCertificate);

                #region By its key pin: the station the pin is listed for, on its own path, and nobody else

                // A certificate whose key pin is listed was let through the
                // handshake by the pin alone, and its chain, its name and its
                // dates were never looked at: anybody can wrap a key in a
                // certificate that says anything. So its common name names
                // nobody, and the station it is let in as is the one the pin
                // is listed for - only on that station's own path, the last
                // segment of the URL, which is what the CN check of OCPP's
                // security profile 3 is for a certificate (A00.FR.511).
                if (StationLogins.FindKeyPin(keyPin) is ChargingStationLogin pinned)
                {

                    var path = StationOfPath(Connection.HTTPRequest);

                    if (path != pinned.Id)
                        return Refuse(Connection, $"its key pin {keyPin} is the charging station '{pinned.Id}''s, and it asked to connect as " +
                                                  (path is null ? "nobody in particular" : $"'{path}'"));

                    if (!pinned.Enabled)
                        return Refuse(Connection, $"the charging station '{pinned.Id}' is switched off here");

                    if (!GroupAllows(pinned, AuthMethod.KeyPin, 3, out var whyNot))
                        return Refuse(Connection, $"the charging station '{pinned.Id}' {whyNot}");

                    if (ocppServerSettings.Logging?.Authentication != false)
                        Log.Info($"The charging station '{pinned.Id}' signed in from {from} with its key pin {keyPin} (security profile 3).",
                                 "ocpp", "station", "auth");

                    Connection.TryAddCustomData(StationIdKey, pinned.Id);

                    return Task.FromResult<HTTPResponse?>(null);

                }

                // Its pin was listed once and is no longer: the handshake may
                // have let it through by that pin, before the pin was taken
                // away - the client decides how long it waits between the two.
                // Such a certificate skipped the accepted chains, so it is held
                // against them now, rather than read as one that passed them.
                if (StationLogins.WasEverListed(keyPin) &&
                    !ClientTrust.Validate(Connection.ClientCertificate, null, ocppServerSettings.CheckCertificateRevocation == true).Accepted)
                {
                    return Refuse(Connection, $"its key pin {keyPin} is no charging station's any more, and its certificate does not lead to an accepted chain");
                }

                // Let through the handshake as the certificate of a back end that
                // dials - by a key pin, or by the CA, of a connection the other
                // end makes - and not under that connection's name: it skipped
                // the accepted chains of the stations, and is held against them
                // now, rather than read as one that passed them.
                if (IsAnInboundCertificate(OCPPVersion.OCPP2_1, Connection.ClientCertificate) &&
                    !ClientTrust.Validate(Connection.ClientCertificate, null, ocppServerSettings.CheckCertificateRevocation == true).Accepted)
                {
                    return Refuse(Connection, $"its certificate is the one a back end that dials signs in with, and it asked to connect as a charging station " +
                                               "under a name that is no such back end's; as a station's it does not lead to an accepted chain");
                }

                #endregion

                // The certificate itself was held against the accepted chains
                // during the handshake; a connection that got this far with one
                // has passed.
                var named  = Connection.ClientCertificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, false);
                var who    = named.IsNullOrEmpty() ? Connection.ClientCertificate.Subject : named;

                // Listed as a login as well? Then it is in a group, and the
                // group decides. Not listed is not an error: the chain was the
                // whole decision before groups existed and still is.
                if (!named.IsNullOrEmpty() &&
                    StationLogins.TryGet(named, out var certificateLogin))
                {

                    if (!certificateLogin.Enabled)
                        return Refuse(Connection, $"the charging station '{named}' is switched off here");

                    // Either by key pin or by certificate authority, never both
                    // (EVQI Transport, key pins, 5.4): a station listed with key
                    // pins is let in by them, and a certificate an authority
                    // issued to its name with another key is a key it was never
                    // given - the very thing a pin is there to refuse.
                    if (certificateLogin.HasKeyPin)
                        return Refuse(Connection, $"the charging station '{named}' is let in by its key pins, and the key of this certificate is none of them");

                    if (!GroupAllows(certificateLogin, AuthMethod.Certificate, 3, out var why))
                        return Refuse(Connection, $"the charging station '{named}' {why}");

                }

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Info($"The charging station '{who}' signed in from {from} with a certificate (security profile 3).",
                             "ocpp", "station", "auth");

                Connection.TryAddCustomData(StationIdKey, who);

                return Task.FromResult<HTTPResponse?>(null);

            }

            if (!profiles.Contains<Byte>(1) && !profiles.Contains<Byte>(2))
                return Refuse(Connection, "only security profile 3 is allowed here and it presented no certificate");

            #endregion

            #region Which profile a credential on this port would be

            var profile = (Byte) (ocppServerTLS ? 2 : 1);

            if (!profiles.Contains(profile))
                return Refuse(
                           Connection,
                           profile == 1
                               ? "the charging station port is unencrypted, so this would be security profile 1, which is not allowed here"
                               : "the charging station port is encrypted, so this would be security profile 2, which is not allowed here"
                       );

            #endregion

            #region A password

            if (Connection.HTTPRequest?.Authorization is HTTPBasicAuthentication basicAuthentication)
            {

                var id = basicAuthentication.Username;

                // Verified before the group is consulted, and always. The
                // store hashes a fixed word for an identification it does not
                // have, so that an unknown one does not answer faster than a
                // known one - and asking the group first would put that back:
                // a station in a group that forbids passwords would be refused
                // without the hashing, and the difference is measurable.
                var right = StationLogins.Verify(id, basicAuthentication.Password);

                if (!StationLogins.TryGet(id, out var login) || !login.Enabled || !right)
                    return Refuse(Connection, $"'{id}' is not a charging station that may sign in, or the password is wrong");

                if (!GroupAllows(login, AuthMethod.Basic, profile, out var why))
                    return Refuse(Connection, $"the charging station '{id}' {why}");

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Info($"The charging station '{id}' signed in from {from} with a password (security profile {profile}).",
                             "ocpp", "station", "auth");

                Connection.TryAddCustomData(StationIdKey, id);

                return Task.FromResult<HTTPResponse?>(null);

            }

            #endregion

            #region A one-time token

            if (Connection.HTTPRequest?.Authorization is HTTPTOTPAuthentication totpAuthentication)
            {

                var id = totpAuthentication.Login;

                // A token bound to the TLS session is checked with keying
                // material of the connection it came over, and only the TLS
                // of QUIC - a WebSocket over HTTP/3 - exports any: .NET does
                // not offer it on an SslStream. Over HTTP/1.1 and HTTP/2 such
                // a token cannot be checked at all. Said out loud, because the
                // alternative is a station whose token is computed correctly
                // and refused anyway, with nothing in the log to explain it.
                var bound     = totpAuthentication.Type != TOTPHTTPHeaderType.RAW;
                var material  = Connection.TOTPTLSExporterMaterial;

                if (bound && material is not { Length: > 0 })
                    return Refuse(Connection, $"'{id}' sent a TLS-bound one-time token over a connection whose TLS exports nothing to check it with: " +
                                               "that is checked over HTTP/3 only. The station has to open its WebSocket over HTTP/3, or send tlscb=false");

                // Derived before the group is consulted, and always, for the
                // same reason as the password above.
                var right = StationLogins.VerifyTOTP(id, totpAuthentication.TOTP, bound, material);

                if (!StationLogins.TryGet(id, out var login) || !login.Enabled || !right)
                    return Refuse(Connection, login is { Enabled: true, TOTP.TLSChannelBinding: true } && !bound
                                                  ? $"'{id}' sent a raw one-time token, and has to show one bound to the TLS session of its connection: " +
                                                     "the station has to open its WebSocket over HTTP/3 and bind its token"
                                                  : $"'{id}' is not a charging station that may sign in, or the one-time token is wrong");

                if (!GroupAllows(login, AuthMethod.TOTP, profile, out var why))
                    return Refuse(Connection, $"the charging station '{id}' {why}");

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Info($"The charging station '{id}' signed in from {from} with a one-time token{(bound ? " bound to its connection" : "")} (security profile {profile}" +
                             $"{(ocppServerTLS ? "" : ", on an unencrypted port, so the token is replayable while it stands")}).",
                             "ocpp", "station", "auth");

                Connection.TryAddCustomData(StationIdKey, id);

                return Task.FromResult<HTTPResponse?>(null);

            }

            #endregion

            return Refuse(Connection, "it sent no credentials");


            #region (local) GroupAllows(Login, Method, Profile, out Why)

            // Whether the group of this login lets it in this way, on this
            // security profile.
            Boolean GroupAllows(ChargingStationLogin  Login,
                                AuthMethod            Method,
                                Byte                  Profile,
                                out String            Why)
            {

                Why = "";

                var group = StationLogins.GroupOf(Login);

                // A login whose group has gone missing is refused rather than
                // waved through: the group is what decides about it, and a
                // decision that cannot be found is not a yes.
                if (group is null)
                {
                    Why = $"belongs to the login group '{Login.GroupId}', which this {NodeWording.Noun} does not have";
                    return false;
                }

                if (!group.Enabled)
                {
                    Why = $"is in the login group '{group.Id}', which is switched off";
                    return false;
                }

                if (!group.Allows(Method))
                {
                    Why = $"is in the login group '{group.Id}', which does not accept {Method.AsText()}";
                    return false;
                }

                if (!group.Allows(Profile))
                {
                    Why = $"is in the login group '{group.Id}', which does not accept security profile {Profile}";
                    return false;
                }

                return true;

            }

            #endregion


            Task<HTTPResponse?> Refuse(WebSocketServerConnection Connection, String Because)
            {

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Warning($"A charging station at {Connection.RemoteSocket} was turned away: {Because}.",
                                "ocpp", "station", "auth");

                // Hermod sets the request on the connection before it asks any
                // validator, so this is never null in practice. If it ever is,
                // the answer is still no: returning null here would mean "let
                // them in", and a refusal that cannot be written is a refusal
                // all the same.
                if (Connection.HTTPRequest is null)
                    throw new InvalidOperationException(
                              "A charging station connection reached the validator without an HTTP request; refusing it."
                          );

                var refusal = new HTTPResponse.Builder(Connection.HTTPRequest) {
                                  HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                  Server          = Server.HTTPServiceName,
                                  Connection      = org.GraphDefined.Vanaheimr.Hermod.HTTP.ConnectionType.Close
                              };

                // What this server takes on this port, and nothing about who
                // asked: the schemes its enabled login groups allow for the
                // security profile a credential here would be. A port that
                // takes certificates and nothing else challenges with nothing.
                var asProfile = (Byte) (ocppServerTLS ? 2 : 1);

                var taken     = profiles.Contains(asProfile)
                                    ? StationLogins.Groups.
                                          Where     (group => group.Enabled && group.SecurityProfiles.Contains(asProfile)).
                                          SelectMany(group => group.AuthMethods).
                                          ToHashSet ()
                                    : [];

                var basic     = taken.Contains(AuthMethod.Basic) ? WWWAuthenticate.Basic("OCPP") : null;
                var totp      = taken.Contains(AuthMethod.TOTP)  ? WWWAuthenticate.TOTP ("OCPP") : null;

                if ((basic is not null && totp is not null ? basic.And(totp) : basic ?? totp) is WWWAuthenticate challenge)
                    refusal.WWWAuthenticate = challenge;

                return Task.FromResult<HTTPResponse?>(refusal.AsImmutable);

            }

        }

        #endregion

        #region (private) ValidateStationCertificate(...)

        /// <summary>
        /// Whether the certificate a charging station presented during the TLS
        /// handshake leads to a chain this local controller accepts.
        /// </summary>
        /// <remarks>
        /// A station that presents no certificate is let through here and
        /// judged by <see cref="ValidateStation"/>, which knows whether a
        /// password would do instead.
        ///
        /// A station that presents one this controller does not accept is
        /// turned away here rather than allowed to fall back to a password: a
        /// station sends a certificate because it was configured for security
        /// profile 3, so it has no password to fall back to, and a certificate
        /// that does not check out is worth a line in the log rather than a
        /// quiet second attempt.
        ///
        /// <b>Or by its key pin.</b> A certificate whose key pin is listed for
        /// a charging station that is switched on is let through by that pin
        /// alone, and its chain, name and dates are not looked at - that is what
        /// a station without a certificate authority behind it relies on. Which
        /// station it is, and whether it may come in as the one it asks to be,
        /// is decided in <see cref="ValidateStation"/>. Every other certificate
        /// is held against the accepted chains as before; a key pin of a station
        /// that is switched off opens nothing here.
        /// </remarks>
        private TLSValidationResult ValidateStationCertificate(Object                                                             Sender,
                                                               System.Security.Cryptography.X509Certificates.X509Certificate2?    Certificate,
                                                               System.Security.Cryptography.X509Certificates.X509Chain?           Chain,
                                                               org.GraphDefined.Vanaheimr.Hermod.TCP.ITCPServer                   Server,
                                                               System.Net.Security.SslPolicyErrors                                PolicyErrors)
        {

            if (Certificate is null)
                return new TLSValidationResult(IsValid: true, Errors: null, Warnings: null);

            #region A back end that dials, by its client certificate

            // The certificate of a connection the other end makes: by its key
            // pin, or by its CA under the name it is issued to. One that claims
            // to be such a certificate passes as that or not at all; one that
            // claims nothing is a station's, and judged as before.
            if (InboundCertificateCheck(OCPPVersion.OCPP2_1, Certificate, Chain, PolicyErrors) is TLSValidationResult backEnd)
                return backEnd;

            #endregion

            #region By its key pin

            if (StationLogins.FindKeyPin(KeyPin.FromCertificate(Certificate)) is ChargingStationLogin pinned &&
                pinned.Enabled)
            {

                // Hermod's validator rather than a comparison here: it also
                // refuses a key that cannot be one - a malformed key, an
                // unknown algorithm, a point that is not on its curve - before
                // any pin is compared (EVQI Transport, key pins, 10.7).
                var validation = KeyPinValidator.Validate(
                                     new KeyPinPolicy(pinned.Pins.Select(keyPin => keyPin.Pin)),
                                     Certificate
                                 );

                if (ocppServerSettings.Logging?.Authentication != false)
                    Log.Log(
                        validation.IsAccepted ? LogLevel.Debug : LogLevel.Warning,
                        validation.IsAccepted
                            ? $"The certificate of '{Certificate.Subject}' was accepted by its key pin {validation.Pin}, which is the charging station '{pinned.Id}''s."
                            : $"The certificate of '{Certificate.Subject}' was refused: {validation.Reason}",
                        "ocpp", "station", "tls"
                    );

                return validation.IsAccepted
                           ? TLSValidationResult.Success()
                           : TLSValidationResult.Failed(validation.Reason ?? "The key was refused.");

            }

            #endregion

            var result = ClientTrust.Validate(
                             Certificate,
                             Chain,
                             ocppServerSettings.CheckCertificateRevocation == true
                         );

            if (ocppServerSettings.Logging?.Authentication != false)
                Log.Log(
                    result.Accepted ? LogLevel.Debug : LogLevel.Warning,
                    result.Accepted
                        ? $"The certificate of '{result.Subject}' was {result.Reason}."
                        : $"The certificate of '{result.Subject}' was refused: {result.Reason}." +
                          // A back end that dials with a certificate of its CA,
                          // issued to another name than its connection's: no
                          // station's, and said so where the refusal is looked for.
                          (InboundCertificateHint(OCPPVersion.OCPP2_1, Certificate, Chain, PolicyErrors) is String hint ? " " + hint : ""),
                    "ocpp", "station", "tls"
                );

            return new TLSValidationResult(
                       IsValid:   result.Accepted,
                       Errors:    result.Accepted ? null : [ Error.Create(result.Reason) ],
                       Warnings:  null
                   );

        }

        #endregion

        #region (private) ApplyStationLogins(Server) / WireOCPPServerLogging(Server)

        /// <summary>
        /// Put the charging station logins into the server, and take out the
        /// ones that are no longer there.
        /// </summary>
        /// <remarks>
        /// Removed and not only added: a station whose password was taken away
        /// must stop being able to use it now, and a dictionary that is only
        /// ever added to would let it in until the next start.
        /// </remarks>
        private void ApplyStationLogins(AWebSocketServer Server)
        {

            var wanted = StationLogins.SecurePasswords();

            foreach (var gone in Server.ClientLogins.Keys.Where(id => !wanted.ContainsKey(id)).ToArray())
                Server.ClientLogins.TryRemove(gone, out _);

            foreach (var login in wanted)
                Server.ClientLogins[login.Key] = login.Value;

        }

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
        /// What of OCPP over SSH ends up in the event log: the same switches as
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
        private static String StationOn(WebSocketServerConnection Connection)

            => Connection.TryGetCustomData(StationIdKey) as String ??
               (IsABackEnd(Connection)
                    ? cloud.charging.open.protocols.WWCP.NetworkingNode.WWCPConnectionExtensions.NetworkingNodeIdOf(Connection)?.ToString()
                    : null) ??
               Connection.RemoteSocket.ToString();

        /// <summary>
        /// Whether a connection of the station port was let in as a back end
        /// that dials this controller, and not as a charging station.
        /// </summary>
        private static Boolean IsABackEnd(WebSocketServerConnection Connection)

            => Connection.TryGetCustomData(InboundConnectionKey) is String;

        /// <summary>
        /// What a connection of the station port is called in the log: the
        /// charging station, or the back end that dialled.
        /// </summary>
        private static String WhoIsOn(WebSocketServerConnection Connection)

            => $"The {(IsABackEnd(Connection) ? "back end" : "charging station")} '{StationOn(Connection)}'";

        /// <summary>
        /// The charging station the URL of an upgrade names: its last segment,
        /// percent-decoded, as OCPP 2.1 Part 4, 3.1.1 has it - or null where it
        /// names none.
        /// </summary>
        /// <remarks>
        /// Read the way the WebSocket server reads it when it registers the
        /// connection, so that the station this controller let in and the one
        /// the connection is registered as cannot be two.
        /// </remarks>
        private static String? StationOfPath(HTTPRequest? Request)
        {

            if (Request is null)
                return null;

            var path     = Request.Path.ToString();
            var station  = path[(path.LastIndexOf('/') + 1)..];

            return station.Length > 0 ? station : null;

        }

        /// <summary>
        /// Which charging station a connection over SSH is: the networking
        /// node its HELLO was welcomed as - or where it comes from.
        /// </summary>
        private static String StationOn(org.GraphDefined.Vanaheimr.Hermod.Messaging.IMessageConnection Connection)

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
        private void LogOCPPMessage(String  Direction,
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

            return new JObject(

                new JProperty("enabled",                     settings.Enabled              == true),
                new JProperty("address",                     settings.Address?.ToString()),
                new JProperty("port",                        (settings.TCPPort ?? OCPPServerConfiguration.DefaultTCPPort).ToUInt16()),
                new JProperty("securityProfiles",            new JArray((settings.SecurityProfiles ?? []).Select(profile => (Int32) profile))),
                new JProperty("subprotocols",                new JArray(settings.Subprotocols ?? [])),
                new JProperty("reachableAs",                 new JArray(settings.ReachableAs  ?? [])),
                new JProperty("minTLSVersion",               OCPPServerConfiguration.TLSVersionText(settings.MinimumTLSVersion ?? OCPPServerConfiguration.DefaultMinimumTLSVersion)),
                new JProperty("checkCertificateRevocation",  settings.CheckCertificateRevocation == true),
                new JProperty("maxConnections",              settings.MaxConnections ?? OCPPServerConfiguration.DefaultMaxConnections),
                new JProperty("pingEverySeconds",            (settings.PingEvery ?? OCPPServerConfiguration.DefaultPingEvery).TotalSeconds),
                new JProperty("http3",                       settings.TakesHTTP3),

                new JProperty("ssh",                         new JObject(
                    new JProperty("enabled",                 settings.SSH?.Enabled == true),
                    new JProperty("subprotocols",            settings.SSH?.Subprotocols is IReadOnlyList<String> sshSubprotocols
                                                                 ? new JArray(sshSubprotocols)
                                                                 : null),
                    new JProperty("maxMessageSize",          settings.SSH?.MaxMessageSize ?? OCPPServerSSH.DefaultMaxMessageSize),
                    new JProperty("pingEverySeconds",        (settings.SSH?.PingEvery ?? OCPPServerSSH.DefaultPingEvery).TotalSeconds),
                    new JProperty("subsystem",               OCPPSshServer.OCPPSubsystem),
                    new JProperty("state",                   new JObject(
                        new JProperty("served",              ocppOverSSHServed),
                        new JProperty("sshServer",           SSHEnabled),
                        new JProperty("url",                 OCPPServerSSHURL),
                        new JProperty("hostKey",             SSHHostKey),
                        new JProperty("connections",         ocppSshServer?.ConnectedNetworkingNodeIds.Count() ?? 0),
                        new JProperty("stationsWithAKey",    StationLogins.Logins.Count(login => login.Enabled && login.HasSSHKey))
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
                    new JProperty("connections",             ocppWebSocketServer?.WebSocketConnections.Count() ?? 0),
                    new JProperty("stationLogins",           StationLogins.EnabledCount),
                    new JProperty("trustedChains",           ClientTrust.EnabledCount),
                    new JProperty("hasCertificate",          ServerCertificates.HasCertificate),

                    // What a charging station pins to reach this port, and what
                    // to hand it before a switch (EVQI Transport, key pins, 7.4).
                    new JProperty("keyPin",                  OCPPServerKeyPin?.    ToHex()),
                    new JProperty("nextKeyPin",              OCPPServerNextKeyPin?.ToHex()),

                    // What a change to the socket is waiting for, said rather
                    // than left to be discovered.
                    new JProperty("waitingForARestart",      new JArray(waiting))
                )),

                new JProperty("limits",                      new JObject(
                    new JProperty("subprotocols",            new JArray(OCPPServerConfiguration.KnownSubprotocols)),
                    new JProperty("securityProfiles",        new JArray(1, 2, 3)),
                    new JProperty("tlsVersions",             new JArray("1.2", "1.3")),
                    new JProperty("maxConnections",          OCPPServerConfiguration.MaxMaxConnections),
                    new JProperty("maxReachableAs",          OCPPServerConfiguration.MaxReachableAs),
                    new JProperty("minMaxMessageSize",       OCPPServerSSH.MinMaxMessageSize),
                    new JProperty("maxMaxMessageSize",       OCPPServerSSH.MaxMaxMessageSize),
                    new JProperty("suggestedPayloadWindowSeconds", OCPPServerLogging.SuggestedPayloadWindow.TotalSeconds)
                )),

                new JProperty("file",                        ConfigFile.Path)

            );

        }

        #endregion

        #region (private) WhyNoHTTP3(Settings)

        /// <summary>
        /// Why the station port takes no WebSockets over HTTP/3 although the
        /// settings ask for it, or null: while it takes them, while it was not
        /// asked to, and while it does not run.
        /// </summary>
        private String? WhyNoHTTP3(OCPPServerConfiguration Settings)

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
        public Boolean TryUpdateOCPPServerConfiguration(JObject                           JSON,
                                                        [NotNullWhen(false)] out String?  Error,
                                                        out Boolean                       NotSaved)
        {

            NotSaved = false;

            if (!OCPPServerConfiguration.TryParseForOneServer(JSON, out var update, out Error))
                return false;

            #region What cannot be saved because it would be a port nobody can use

            if (update.SecurityProfiles is { Count: > 0 } profiles &&
                profiles.All(profile => profile > 1) &&
                !ServerCertificates.HasCertificate)
            {
                Error = "Security profiles 2 and 3 both need TLS, and there is no server certificate yet. " +
                        "Make a signing request and upload the certificate that answers it, or make a server key for the charging " +
                        "stations to pin, or allow security profile 1 as well.";
                return false;
            }

            if (update.Logging?.Payloads == true &&
                update.Logging.PayloadsUntil <= TimeProvider.GetUtcNow())
            {
                Error = $"The window for logging message contents ends at {update.Logging.PayloadsUntil:yyyy-MM-dd HH:mm}'Z', " +
                        $"which by this {NodeWording.Noun}'s clock has already passed - it is now {TimeProvider.GetUtcNow():yyyy-MM-dd HH:mm}'Z'.";
                return false;
            }

            #endregion

            reconfigureLock.Wait();

            try
            {

                // Everything this refuses is the file's: read, merged, written.
                if (!ConfigFile.TryMergeSection(OCPPServerConfiguration.SectionName, update.ToJSON(), out Error))
                {
                    NotSaved = true;
                    return false;
                }

                var before = ocppServerSettings;

                ocppServerSettings = before.Merge(update).Effective();

                #region What takes effect at once

                if (ocppWebSocketServer is not null &&
                    before.Enabled != ocppServerSettings.Enabled)
                {

                    if (ocppServerSettings.Enabled == true)
                        StartOCPPServer().GetAwaiter().GetResult();

                    else
                        StopOCPPServer(). GetAwaiter().GetResult();

                }

                // OCPP over SSH on and off while the station server runs: the
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
                if (ocppServerStarted &&
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
