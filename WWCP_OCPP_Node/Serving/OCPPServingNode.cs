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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;

using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

using OCPPWebSockets = cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// A node that dials a CSMS and is dialled itself, by the charging stations
    /// - and the local controllers - below it: a local controller, and a
    /// gateway. What it dials is WWCPNode's, over OCPP (OCPPDialling); the server those below it
    /// connect to - its section "webSocketServer", the charging stations that may
    /// sign in and their login groups, the chains a station's certificate may
    /// lead to, the keys and certificates it presents, OCPP over SSH and a CSMS
    /// that dials it on the same port - is here, the same for both.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A class of its own between WWCPNode and the kinds, and not a part of
    /// OCPPDialling: a charging station dials and has an OCPP
    /// server of its own for a CSMS that cannot be dialled, with members of the
    /// same names that mean something else, and it has no stations below it.
    /// </para>
    /// <para>
    /// What a kind does with it: it builds the server in its constructor once
    /// the OCPP node exists (<see cref="BuildOCPPServer"/>, with the section
    /// it read), starts it once the node listens and stops it when the node
    /// stops (<see cref="StartOCPPServer"/>, <see cref="StopOCPPServer"/>), and
    /// says the few things that are its own here: whether a CSMS may dial it
    /// (<see cref="LetsBackEndsDialIn"/>), what it does with a station it has
    /// seen (FollowStationsSeen) and with a switch of its server key
    /// (<see cref="ServerKeySwitched"/>).
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode : WWCPNode
    {

        #region Data

        /// <summary>
        /// The port the charging stations connect to, as a sentence names it
        /// when it cannot be had.
        /// </summary>
        public static readonly NodePort  StationServerPort  = new ("The charging station server");

        #endregion

        #region Properties

        /// <summary>
        /// The node that speaks OCPP 1.6 for this one, where it has one.
        /// </summary>
        protected abstract OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Node16    { get; }

        /// <summary>
        /// The node that speaks OCPP 2.1 for this one.
        /// </summary>
        protected abstract OCPPv2_1.NetworkingNode.AOCPPNetworkingNode   Node21    { get; }

        /// <summary>
        /// What OCPP says of the connections of this node.
        /// </summary>
        public OCPPDialling  OCPPDialling
            => (OCPPDialling) Dialling;

        /// <summary>
        /// The OCPP nodes this one has: the one for OCPP 2.1, and before it
        /// the one for OCPP 1.6 where it speaks that.
        /// </summary>
        protected IEnumerable<NetworkingNode.AOCPPNetworkingNode> Nodes
            => OCPPDialling.Nodes;

        /// <summary>
        /// What OCPP says of the connections of this node - asked for once,
        /// before anything of them is. A kind of node that says more of them
        /// than every serving node gives itself one of its own.
        /// </summary>
        protected virtual OCPPDialling NewOCPPDialling()

            => new (this, () => Node16, () => Node21);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A node that dials a CSMS and serves the nodes below it; everything
        /// it is given is the node's.
        /// </summary>
        protected OCPPServingNode(NodeKind?                                  Kind                            = null,
                                  String?                                    Version                         = null,
                                  IPPort?                                    HTTPPort                        = null,
                                  IIPAddress?                                HTTPHostname                    = null,
                                  HTTPServer?                                HTTPServer                      = null,
                                  ServerCertificateSelectorDelegate?         ServerCertificateSelector       = null,
                                  ServerCertificateChainSelectorDelegate?    ServerCertificateChainSelector  = null,
                                  HTTPPath?                                  BasePath                        = null,
                                  HTTPPath?                                  HTTPRootPath                    = null,
                                  HTTPExtAPI?                                ExtAPI                          = null,
                                  String?                                    AccountsPath                    = null,
                                  IEnumerable<String>?                       Roles                           = null,
                                  IEnumerable<String>?                       Resources                       = null,
                                  IEnumerable<Role>?                         RoleDefinitions                 = null,
                                  WWCPConfigFile?                            ConfigFile                      = null,
                                  DNSClient?                                 DNSClient                       = null,
                                  NTSClient?                                 NTSClient                       = null,
                                  IStaticContentSource?                      Frontend                        = null,
                                  String?                                    CertificatesPath                = null,
                                  IEnumerable<CertificateKind>?              CertificateKinds                = null,
                                  IEnumerable<String>?                       CertificateUsages               = null,
                                  IEnumerable<String>?                       CertificateListeners            = null,
                                  EventLog?                                  Log                             = null,
                                  Boolean                                    LogToConsole                    = true,
                                  LogLevel                                   ConsoleLogLevel                 = LogLevel.Info,
                                  String?                                    LogPath                         = null,
                                  String?                                    MetrologicalLogPath             = null,
                                  Boolean                                    BridgeDebugLog                  = true,
                                  IEnumerable<(String Needle, String Tag)>?  TraceTags                       = null,
                                  TimeProvider?                              TimeProvider                    = null,
                                  SSHSettings?                               SSH                             = null,
                                  SecurityHeaderOptions?                     FrontendSecurityHeaders         = null,
                                  WebConfiguration?                          Web                             = null,
                                  IEnumerable<CertificateKind>?              CertificateRequestKinds         = null)

            : base(Kind,
                   Version,
                   HTTPPort,
                   HTTPHostname,
                   HTTPServer,
                   ServerCertificateSelector,
                   ServerCertificateChainSelector,
                   BasePath,
                   HTTPRootPath,
                   ExtAPI,
                   AccountsPath,
                   Roles,
                   Resources,
                   RoleDefinitions,
                   ConfigFile,
                   DNSClient,
                   NTSClient,
                   Frontend,
                   CertificatesPath,
                   WithStationPortKinds    (CertificateKinds),
                   CertificateUsages,
                   WithStationPortListener (CertificateListeners),
                   Log,
                   LogToConsole,
                   ConsoleLogLevel,
                   LogPath,
                   MetrologicalLogPath,
                   BridgeDebugLog,
                   TraceTags,
                   TimeProvider,
                   SSH,
                   FrontendSecurityHeaders,
                   Web,
                   WithStationPortRequests (CertificateRequestKinds))

        {

            // Its connections speak OCPP, dialled by the OCPP nodes it has
            // at the time - asked for whenever they are needed.
            Dialling = NewOCPPDialling();

            http3Restarts = new RestartsOneAtATime(
                                RestartHTTP3ToShowAgainAsync,
                                e => this.Log.Exception(e, "HTTP/3 of the charging station port could not be started again with its new certificate.", "ocpp", "station", "tls")
                            );

        }

        #endregion


        #region (private static) WithStationPortKinds(Kinds) / WithStationPortListener(Listeners) / WithStationPortRequests(RequestKinds)

        /// <summary>
        /// The kinds the node's store keeps, with the TLS server identity among
        /// them: what the station port presents is one - every kind, where the
        /// kind of node says nothing.
        /// </summary>
        private static IEnumerable<CertificateKind>? WithStationPortKinds(IEnumerable<CertificateKind>? Kinds)

            => Kinds is null
                   ? null
                   : Kinds.Contains(CertificateKind.TLSServerIdentity)
                         ? Kinds
                         : [ .. Kinds, CertificateKind.TLSServerIdentity ];

        /// <summary>
        /// The listeners a TLS server identity of the store may be told, with
        /// the station port's, "ocpp", among them - see <see cref="StationListener"/>.
        /// </summary>
        private static IEnumerable<String> WithStationPortListener(IEnumerable<String>? Listeners)

            => (Listeners ?? []).Contains(StationListener, StringComparer.OrdinalIgnoreCase)
                   ? Listeners!
                   : [ .. Listeners ?? [], StationListener ];

        /// <summary>
        /// The kinds the node makes keys and signing requests for, with the TLS
        /// server identity among them: a key the station port presents is best
        /// made here, and never carried to the node.
        /// </summary>
        private static IEnumerable<CertificateKind> WithStationPortRequests(IEnumerable<CertificateKind>? RequestKinds)

            => (RequestKinds ?? []).Contains(CertificateKind.TLSServerIdentity)
                   ? RequestKinds!
                   : [ .. RequestKinds ?? [], CertificateKind.TLSServerIdentity ];

        #endregion

        #region (protected virtual) LetsBackEndsDialIn

        /// <summary>
        /// Whether a CSMS may dial this node over the port of its charging
        /// stations, as a connection the other end makes. Yes, unless the kind
        /// says otherwise - a local controller only while it reports to a CSMS
        /// at all.
        /// </summary>
        protected virtual Boolean LetsBackEndsDialIn
            => true;

        #endregion

        #region (protected virtual) ServesStations / ServesOCPPOverSSH / MaxFailedSignInsPerMinute

        /// <summary>
        /// Whether charging stations sign in at the station port. Yes, unless
        /// the kind says otherwise: a charging station serves no stations, and
        /// what it lets in is a back end that dials it - a connection the other
        /// end makes - and nothing else.
        /// </summary>
        protected virtual Boolean ServesStations
            => true;

        /// <summary>
        /// Whether OCPP over SSH is offered beside the station port, as the
        /// subsystem "ocpp@open.charging.cloud" of the node's SSH server. Yes,
        /// unless the kind says otherwise - a charging station, whose tools
        /// commission it over that subsystem, and which nothing dials over SSH.
        /// </summary>
        protected virtual Boolean ServesOCPPOverSSH
            => true;

        /// <summary>
        /// How many sign-ins that were wrong one address is given within a
        /// minute before the station port tells it to wait - or null, where
        /// the kind sets no limit.
        /// </summary>
        /// <remarks>
        /// A sign-in with a client certificate is no guess, and is neither
        /// counted nor held back.
        /// </remarks>
        protected virtual UInt32? MaxFailedSignInsPerMinute
            => null;

        #endregion

        #region (protected virtual) FollowStationsSeen(Server)

        /// <summary>
        /// Follow the charging stations that come and go on the WebSocket
        /// server of this node; nothing unless the kind says otherwise.
        /// </summary>
        protected virtual void FollowStationsSeen(OCPPWebSockets.OCPPWebSocketServer Server)
        { }

        /// <summary>
        /// Follow the charging stations that come and go over SSH; nothing
        /// unless the kind says otherwise.
        /// </summary>
        protected virtual void FollowStationsSeen(OCPPSshServer Server)
        { }

        #endregion

        #region (protected virtual) ServerKeySwitched(Before, After)

        /// <summary>
        /// The key the station port presents was switched to the next one - by
        /// somebody on the page, at the command line or over OCPP. Nothing
        /// unless the kind says otherwise; a local controller tells its CSMS.
        /// </summary>
        /// <param name="Before">The key pin of the key presented before, if any.</param>
        /// <param name="After">The key pin of the key presented now.</param>
        protected virtual void ServerKeySwitched(KeyPin?  Before,
                                                 KeyPin   After)
        { }

        #endregion


        #region StationServerJSON()

        /// <summary>
        /// The station server in a few words, as the Configuration page shows
        /// it on a card of its own.
        /// </summary>
        public JObject StationServerJSON()

            => new (
                   new JProperty("enabled",          OCPPServerEnabled),
                   new JProperty("running",          ocppServerStarted),
                   new JProperty("tls",              ocppServerTLS),
                   new JProperty("url",              OCPPServerURL),
                   new JProperty("sshUrl",           OCPPOverSSHServed ? OCPPServerSSHURL : null),
                   new JProperty("clientAuthentication", new JArray((ocppServerSettings.ClientAuthentication ?? []).Select(authentication => authentication.ToText()))),
                   new JProperty("webSocketLogins",    WebSocketLogins.EnabledCount),
                   new JProperty("trustedChains",    ClientTrust.EnabledCount),
                   new JProperty("certificates",     Certificates.ByKind(CertificateKind.TLSServerIdentity).Count(entry => entry.IsFor(StationListener)))
               );

        #endregion

        #region DisposeAsync()

        /// <summary>
        /// Stop listening, let go of the stores of the station server, and then
        /// of the rest.
        /// </summary>
        /// <remarks>
        /// Let go of even where stopping fails, which is thrown on afterwards:
        /// a node whose stop threw used to keep its certificate stores, and
        /// everything of the node below, its log file among it.
        /// </remarks>
        public override async ValueTask DisposeAsync()
        {

            try
            {

                // Stopped here as well as below: the stores may not go before
                // the station server that reads them has.
                await Stop();

            }
            finally
            {

                try
                {
                    ClientTrust?.Dispose();
                }
                finally
                {
                    await base.DisposeAsync();
                }

            }

        }

        #endregion

    }

}
