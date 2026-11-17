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

using OCPPWebSockets = cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// A node that dials a CSMS and is dialled itself, by the charging stations
    /// - and the local controllers - below it: a local controller, and a
    /// gateway. What it dials is OCPPDiallingNode's; the server those below it
    /// connect to - its section "ocppServer", the charging stations that may
    /// sign in and their login groups, the chains a station's certificate may
    /// lead to, the keys and certificates it presents, OCPP over SSH and a CSMS
    /// that dials it on the same port - is here, the same for both.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A class of its own between OCPPDiallingNode and the kinds, and not a
    /// part of OCPPDiallingNode: a charging station dials and has an OCPP
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
    public abstract partial class OCPPServingNode : OCPPDiallingNode
    {

        #region Data

        /// <summary>
        /// The port the charging stations connect to, as a sentence names it
        /// when it cannot be had.
        /// </summary>
        public static readonly NodePort  StationServerPort  = new ("The charging station server");

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
                   CertificateKinds,
                   CertificateUsages,
                   CertificateListeners,
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
                   CertificateRequestKinds)

        { }

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
                   new JProperty("securityProfiles", new JArray((ocppServerSettings.SecurityProfiles ?? []).Select(profile => (Int32) profile))),
                   new JProperty("stationLogins",    StationLogins.EnabledCount),
                   new JProperty("trustedChains",    ClientTrust.EnabledCount),
                   new JProperty("certificates",     ServerCertificates.Entries.Count)
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
                    ServerCertificates?.Dispose();
                    ClientTrust?       .Dispose();
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
