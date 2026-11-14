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

using cloud.charging.open.protocols.OCPP.Node.Connections;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// A node that dials a CSMS: a charging station, and a local controller
    /// on its line up. What it dials is written down in its connections; how
    /// each is dialled, followed, believed, let go of and dialled again, and
    /// which of several it turns to when one fails, is the same for both, and
    /// is here.
    /// </summary>
    /// <remarks>
    /// What differs is said by the node itself: which OCPP nodes it has, and
    /// what it does when a connection is up - a charging station tells its
    /// CSMS that it started, a local controller has nothing of the kind to
    /// say.
    /// </remarks>
    public abstract partial class OCPPDiallingNode : WWCPNode
    {

        #region Properties

        /// <summary>
        /// The places this node dials, and the credentials it proves itself
        /// with when it gets there.
        /// </summary>
        public ConnectionStore         Connections           { get; protected set; } = default!;

        /// <summary>
        /// The key this node signs in with when it dials an ssh://
        /// connection - made the first time one needs it.
        /// </summary>
        public SshClientKey            SshKey                { get; protected set; } = default!;

        /// <summary>
        /// The node that speaks OCPP 1.6 for this one, where it has one.
        /// </summary>
        protected abstract OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Node16    { get; }

        /// <summary>
        /// The node that speaks OCPP 2.1 for this one.
        /// </summary>
        protected abstract OCPPv2_1.NetworkingNode.AOCPPNetworkingNode   Node21    { get; }

        /// <summary>
        /// The OCPP nodes this one has: the one for OCPP 2.1, and before it
        /// the one for OCPP 1.6 where it speaks that.
        /// </summary>
        protected IEnumerable<OCPP.NetworkingNode.AOCPPNetworkingNode> Nodes

            => Node16 is OCPPv1_6.NetworkingNode.AOCPPNetworkingNode node16
                   ? [ node16, Node21 ]
                   : [ Node21 ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A node that dials a CSMS; everything it is given is the node's.
        /// </summary>
        protected OCPPDiallingNode(NodeKind?                                  Kind                            = null,
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

    }

}
