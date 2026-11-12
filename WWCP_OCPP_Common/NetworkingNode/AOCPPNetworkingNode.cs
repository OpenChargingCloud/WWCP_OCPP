/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.TCP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.WebSockets;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

    public abstract class AOCPPNetworkingNode : AWWCPNetworkingNode,
                                                INetworkingNode
    {

        #region Data

        private DateTimeOffset lastRoutesBroadcast;

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => Id.ToString();

        //public OCPPAdapter  OCPP    { get; }

        public IEnumerable<IOCPPWebSocketClient> OCPPWebSocketClients
            => wwcpWebSocketClients.
                   Where(webSocketClient => webSocketClient is IOCPPWebSocketClient).
                   Cast<IOCPPWebSocketClient>();

        public IEnumerable<IOCPPWebSocketServer> OCPPWebSocketServers
            => wwcpWebSocketServers.
                   Where(webSocketServer => webSocketServer is IOCPPWebSocketServer).
                   Cast<IOCPPWebSocketServer>();

        /// <summary>
        /// The SSH subsystems this node serves OCPP on.
        /// </summary>
        public IEnumerable<OCPPSshServer> OCPPSshServers
            => WWCPSshServers.OfType<OCPPSshServer>();

        /// <summary>
        /// The SSH connections this node speaks OCPP over.
        /// </summary>
        public IEnumerable<OCPPSshClient> OCPPSshClients
            => WWCPSshClients.OfType<OCPPSshClient>();

        /// <summary>
        /// The OCPP versions this node offers and serves over SSH, by preference - where a caller names none.
        /// </summary>
        protected virtual IEnumerable<String> DefaultOCPPSubprotocols
            => [];

        /// <summary>
        /// The subprotocols a server of this node takes over SSH - where a caller names none. More than
        /// it offers as a client: whoever calls decides how it wants to be spoken to.
        /// </summary>
        protected virtual IEnumerable<String> DefaultOCPPServerSubprotocols
            => DefaultOCPPSubprotocols;

        /// <summary>
        /// Whom a connection to a CSMS reaches, where a caller names nobody: "CSMS" in OCPP 2.0.1 and 2.1,
        /// "CentralSystem" in OCPP 1.6 - by that name a node sends to its CSMS.
        /// </summary>
        protected virtual NetworkingNode_Id DefaultNextHopNetworkingNodeId
            => NetworkingNode_Id.CSMS;

        #endregion

        #region (protected) ComesFromItsCSMS(Connection)

        /// <summary>
        /// Whether what came over the given connection came from the CSMS this node
        /// sends its own messages to, by <see cref="DefaultNextHopNetworkingNodeId"/>.
        /// </summary>
        /// <remarks>
        /// A station connected to a second back end as well - a successor, during a
        /// migration - registers with that one too, and is answered by it. Such an
        /// answer is about that connection: the heartbeat interval it gives, or its
        /// refusal, set on the node, changed the heartbeat to the node's CSMS. Where
        /// nothing tells the connections apart - a connection this node accepted, one
        /// without a client, no route by that name - it is taken to come from the
        /// CSMS, as everything did before.
        /// </remarks>
        /// <param name="Connection">The connection something came over.</param>
        protected Boolean ComesFromItsCSMS(IMessageConnection? Connection)

            => Connection?.WWCPClientOf() is not IWWCPClient client                      ||
               !Routing.LookupNetworkingNode(DefaultNextHopNetworkingNodeId, out var csms) ||
               csms.Client is null                                                        ||
               ReferenceEquals(csms.Client, client);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public AOCPPNetworkingNode(NetworkingNode_Id  Id,
                                   I18NString?        Description                 = null,
                                   CustomData?        CustomData                  = null,

                                   SignaturePolicy?   SignaturePolicy             = null,
                                   SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                   HTTPExtAPI?        HTTPExtAPI                  = null,
                                   WebSocketServer?   ControlWebSocketServer      = null,

                                   Boolean            DisableSendHeartbeats       = false,
                                   TimeSpan?          SendHeartbeatsEvery         = null,
                                   TimeSpan?          DefaultRequestTimeout       = null,

                                   Boolean            DisableMaintenanceTasks     = false,
                                   TimeSpan?          MaintenanceEvery            = null,
                                   IDNSClient?        DNSClient                   = null,

                                   TimeProvider?      Clock                       = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPExtAPI,
                   ControlWebSocketServer,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,
                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient,
                   Clock)

        {

            // Not a field initialiser: those run before the constructor and so
            // cannot reach this node's clock.
            lastRoutesBroadcast = Now;

        }

        #endregion


        #region ConnectWebSocketClient(...)

        public virtual async Task<HTTPResponse> ConnectOCPPWebSocketClient(URL                                                             RemoteURL,
                                                                           I18NString?                                                     Description                      = null,

                                                                           NetworkingMode?                                                 NetworkingMode                   = null,

                                                                           HTTPHostname?                                                   VirtualHostname                  = null,
                                                                           String?                                                         HTTPUserAgent                    = null,
                                                                           IHTTPAuthentication?                                            HTTPAuthentication               = null,
                                                                           IEnumerable<String>?                                            SecWebSocketProtocols            = null,
                                                                           TimeSpan?                                                       RequestTimeout                   = null,

                                                                           NetworkingNode_Id?                                              NextHopNetworkingNodeId          = null,
                                                                           IEnumerable<NetworkingNode_Id>?                                 RoutingNetworkingNodeIds         = null,

                                                                           Boolean                                                         DisableWebSocketPings            = false,
                                                                           TimeSpan?                                                       WebSocketPingEvery               = null,
                                                                           TimeSpan?                                                       SlowNetworkSimulationDelay       = null,

                                                                           Boolean                                                         DisableMaintenanceTasks          = false,
                                                                           TimeSpan?                                                       MaintenanceEvery                 = null,

                                                                           String?                                                         TLSHostname                      = null,
                                                                           RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator       = null,
                                                                           LocalCertificateSelectionHandler?                               LocalCertificateSelector         = null,
                                                                           IEnumerable<X509Certificate2>?                                  ClientCertificates               = null,
                                                                           SslStreamCertificateContext?                                    ClientCertificateContext         = null,
                                                                           IEnumerable<X509Certificate2>?                                  ClientCertificateChain           = null,
                                                                           SslProtocols?                                                   TLSProtocols                     = null,
                                                                           CipherSuitesPolicy?                                             CipherSuitesPolicy               = null,
                                                                           X509ChainPolicy?                                                CertificateChainPolicy           = null,
                                                                           X509RevocationMode?                                             CertificateRevocationCheckMode   = null,
                                                                           Boolean?                                                        EnforceTLS                       = null,
                                                                           IEnumerable<SslApplicationProtocol>?                            ApplicationProtocols             = null,
                                                                           WebSocketHTTPVersion?                                           WebSocketOver                    = null,
                                                                           Boolean?                                                        AllowRenegotiation               = null,
                                                                           Boolean?                                                        AllowTLSResume                   = null,
                                                                           TOTPConfig?                                                     TOTPConfig                       = null,
                                                                           String?                                                         TOTPLogin                        = null,

                                                                           IPVersionPreference?                                            IPVersionPreference              = null,
                                                                           TimeSpan?                                                       ConnectTimeout                   = null,
                                                                           TimeSpan?                                                       ReceiveTimeout                   = null,
                                                                           TimeSpan?                                                       SendTimeout                      = null,
                                                                           TransmissionRetryDelayDelegate?                                 TransmissionRetryDelay           = null,
                                                                           UInt16?                                                         MaxNumberOfRetries               = null,
                                                                           UInt32?                                                         InternalBufferSize               = null,

                                                                           Boolean?                                                        DisableLogging                   = null,
                                                                           String?                                                         LoggingPath                      = null,
                                                                           String                                                          LoggingContext                   = "logcontext", //CPClientLogger.DefaultContext,
                                                                           LogfileCreatorDelegate?                                         LogfileCreator                   = null,
                                                                           HTTPClientLogger?                                               HTTPLogger                       = null,

                                                                           IDNSClient?                                                     DNSClient                        = null,
                                                                           ILogger<ATLSClient>?                                            Logger                           = null,
                                                                           ILoggerFactory?                                                 LoggerFactory                    = null,

                                                                           EventTracking_Id?                                               EventTrackingId                  = null,
                                                                           CancellationToken                                               CancellationToken                = default)


        {

            NextHopNetworkingNodeId ??= NetworkingNode_Id.CSMS;

            var ocppWebSocketClient = new OCPPWebSocketClient(

                                          this,

                                          RemoteURL,
                                          Description,

                                          NetworkingMode,

                                          VirtualHostname,
                                          HTTPUserAgent,
                                          HTTPAuthentication,
                                          SecWebSocketProtocols,
                                          RequestTimeout,

                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DisableMaintenanceTasks,
                                          MaintenanceEvery,

                                          TLSHostname,
                                          RemoteCertificateValidator,
                                          LocalCertificateSelector,
                                          ClientCertificates,
                                          ClientCertificateContext,
                                          ClientCertificateChain,
                                          TLSProtocols,
                                          CipherSuitesPolicy,
                                          CertificateChainPolicy,
                                          CertificateRevocationCheckMode,
                                          EnforceTLS,
                                          ApplicationProtocols,
                                          AllowRenegotiation,
                                          AllowTLSResume,
                                          TOTPConfig,
                                          TOTPLogin,

                                          IPVersionPreference,
                                          ConnectTimeout,
                                          ReceiveTimeout,
                                          SendTimeout,
                                          TransmissionRetryDelay,
                                          MaxNumberOfRetries,
                                          InternalBufferSize,

                                          DisableLogging,
                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator,
                                          HTTPLogger,

                                          DNSClient,
                                          Logger,
                                          LoggerFactory

                                      );

            // Over HTTP/1.1 and its Upgrade unless said otherwise: over HTTP/2 and its
            // extended CONNECT (RFC 8441), or over whichever of the two the server chooses.
            if (WebSocketOver.HasValue)
                ocppWebSocketClient.WebSocketOver = WebSocketOver.Value;

            return await ConnectWebSocketClient(

                             ocppWebSocketClient,

                             NextHopNetworkingNodeId,
                             RoutingNetworkingNodeIds,
                             EventTrackingId,
                             CancellationToken

                         );

        }

        #endregion

        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP WebSocket service.</param>
        /// 
        /// <param name="AutoStart">Start the server immediately.</param>
        public virtual OCPPWebSocketServer AttachWebSocketServer(String?                                                         HTTPServiceName              = null,
                                                                 IIPAddress?                                                     IPAddress                    = null,
                                                                 IPPort?                                                         TCPPort                      = null,
                                                                 I18NString?                                                     Description                  = null,

                                                                 Boolean                                                         RequireAuthentication        = true,
                                                                 IEnumerable<String>?                                            SecWebSocketProtocols        = null,
                                                                 SubprotocolSelectorDelegate?                                    SubprotocolSelector          = null,
                                                                 Boolean                                                         DisableWebSocketPings        = false,
                                                                 TimeSpan?                                                       WebSocketPingEvery           = null,
                                                                 TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                                 ServerCertificateSelectorDelegate?                              ServerCertificateSelector    = null,
                                                                 RemoteTLSClientCertificateValidationHandler<ITCPServer>?        ClientCertificateValidator   = null,
                                                                 LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                                                 SslProtocols?                                                   AllowedTLSProtocols          = null,
                                                                 Boolean?                                                        ClientCertificateRequired    = null,
                                                                 Boolean?                                                        CheckCertificateRevocation   = null,

                                                           //      ServerThreadNameCreatorDelegate?                                ServerThreadNameCreator      = null,
                                                           //      ServerThreadPriorityDelegate?                                   ServerThreadPrioritySetter   = null,
                                                                 Boolean?                                                        ServerThreadIsBackground     = null,
                                                           //      ConnectionIdBuilder?                                            ConnectionIdBuilder          = null,
                                                                 TimeSpan?                                                       ConnectionTimeout            = null,
                                                                 UInt32?                                                         MaxClientConnections         = null,

                                                                 Boolean                                                         AutoStart                    = true)
        {

            var ocppWebSocketServer = new OCPPWebSocketServer(

                                          this,

                                          HTTPServiceName,
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
                                          SecWebSocketProtocols,
                                          SubprotocolSelector,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          ServerCertificateSelector,
                                          ClientCertificateValidator,
                                          LocalCertificateSelector,
                                          AllowedTLSProtocols,
                                          ClientCertificateRequired,
                                          CheckCertificateRevocation,

                                          //ServerThreadNameCreator,
                                          //ServerThreadPrioritySetter,
                                          //ServerThreadIsBackground,
                                          //ConnectionIdBuilder,
                                          //ConnectionTimeout,
                                          //MaxClientConnections,

                                          DNSClient:  DNSClient,
                                          AutoStart:  false

                                      );

            WireWebSocketServer(ocppWebSocketServer);

            if (AutoStart)
                ocppWebSocketServer.Start();

            return ocppWebSocketServer;

        }

        #endregion


        #region ConnectOCPPClient       (RemoteURL, ...)

        /// <summary>
        /// Connect to another networking node - a CSMS, a local controller - by its URL: over an HTTP WebSocket
        /// for "ws://" and "wss://" (and "http://" and "https://"), over the SSH subsystem
        /// "ocpp@open.charging.cloud" for "ssh://". The parameters of the other transport are not used.
        /// </summary>
        /// <param name="RemoteURL">Where to connect to: "wss://csms.example.org/ocpp/cs001", "ssh://cs001@lc.example.org:2222".</param>
        /// <param name="NextHopNetworkingNodeId">The networking node at the other end; by default the node's CSMS, see <see cref="DefaultNextHopNetworkingNodeId"/>.</param>
        /// <param name="RoutingNetworkingNodeIds">The networking nodes reached through it.</param>
        /// <param name="NetworkingMode">Whether messages carry their network path; standard by default.</param>
        /// <param name="Subprotocols">The OCPP versions to offer, by preference; the node's own by default.</param>
        /// <param name="RequestTimeout">How long to wait for an answer to a request (HTTP WebSocket).</param>
        /// <param name="PingEvery">Every how long to send a ping.</param>
        /// <param name="Description">An optional description.</param>
        /// <param name="HTTPAuthentication">HTTP Basic credentials (HTTP WebSocket).</param>
        /// <param name="TOTPConfig">A one-time password configuration (HTTP WebSocket).</param>
        /// <param name="TOTPLogin">The optional login under which the one-time password of the TOTP configuration goes in "Authorization: TOTP".</param>
        /// <param name="ClientCertificates">TLS client certificates (HTTP WebSocket).</param>
        /// <param name="ClientCertificateContext">A TLS client certificate context (HTTP WebSocket).</param>
        /// <param name="RemoteCertificateValidator">Whether to believe the server's TLS certificate (HTTP WebSocket).</param>
        /// <param name="TLSProtocols">The TLS versions allowed (HTTP WebSocket).</param>
        /// <param name="Ssh">The SSH login: user name, keys, how the host key is checked (SSH) - its user name must be the URL's, where the URL has one.</param>
        /// <param name="DNSClient">The DNS client the URL's host name is resolved with - over either transport; the node's by default. Never the resolver of the operating system.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentException">The URL is neither a WebSocket URL nor "ssh://"; or it is "ssh://" and no SSH login was given.</exception>
        public async Task<OCPPClientConnectResult> ConnectOCPPClient(URL                                                             RemoteURL,
                                                                     NetworkingNode_Id?                                              NextHopNetworkingNodeId      = null,
                                                                     IEnumerable<NetworkingNode_Id>?                                 RoutingNetworkingNodeIds     = null,
                                                                     NetworkingMode?                                                 NetworkingMode               = null,
                                                                     IEnumerable<String>?                                            Subprotocols                 = null,
                                                                     TimeSpan?                                                       RequestTimeout               = null,
                                                                     TimeSpan?                                                       PingEvery                    = null,
                                                                     I18NString?                                                     Description                  = null,

                                                                     IHTTPAuthentication?                                            HTTPAuthentication           = null,
                                                                     TOTPConfig?                                                     TOTPConfig                   = null,
                                                                     String?                                                         TOTPLogin                    = null,
                                                                     IEnumerable<X509Certificate2>?                                  ClientCertificates           = null,
                                                                     SslStreamCertificateContext?                                    ClientCertificateContext     = null,
                                                                     RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
                                                                     SslProtocols?                                                   TLSProtocols                 = null,

                                                                     SshClientOptions?                                               Ssh                          = null,

                                                                     IDNSClient?                                                     DNSClient                    = null,
                                                                     EventTracking_Id?                                               EventTrackingId              = null,
                                                                     CancellationToken                                               CancellationToken            = default)
        {

            if (RemoteURL.Scheme == URIScheme.ssh)
            {

                if (Ssh is null)
                    throw new ArgumentException($"'{RemoteURL}' is an ssh:// URL, and no SSH login was given.", nameof(Ssh));

                var sshClient = NewOCPPSshClient(
                                    WWCPSshClient.OptionsFrom(RemoteURL, Ssh, OCPPSshServer.OCPPSubsystem, Subprotocols, ReconnectPolicy, PingEvery, DNSClient ?? this.DNSClient),
                                    NetworkingMode,
                                    WWCPSshClient.ConnectAsFrom(RemoteURL),
                                    Description
                                );

                try
                {

                    var connection = await ConnectSshClient(
                                               sshClient,
                                               NextHopNetworkingNodeId ?? DefaultNextHopNetworkingNodeId,
                                               RoutingNetworkingNodeIds,
                                               CancellationToken
                                           );

                    return new OCPPClientConnectResult(RemoteURL, MessageTransport.SSH, sshClient, connection, null, null);

                }
                catch (Exception e) when (!CancellationToken.IsCancellationRequested)
                {
                    return new OCPPClientConnectResult(RemoteURL, MessageTransport.SSH, sshClient, null, null, e);
                }

            }

            // http:// and https:// as well: many a configuration names its WebSocket that way.
            if (RemoteURL.Scheme != URIScheme.ws   &&
                RemoteURL.Scheme != URIScheme.wss  &&
                RemoteURL.Scheme != URIScheme.http &&
                RemoteURL.Scheme != URIScheme.https)
            {
                throw new ArgumentException($"'{RemoteURL}' is neither a WebSocket URL (ws://, wss://, http://, https://) nor an ssh:// URL.", nameof(RemoteURL));
            }

            var response = await ConnectOCPPWebSocketClient(

                                     RemoteURL:                   RemoteURL,
                                     Description:                 Description,
                                     NetworkingMode:              NetworkingMode,
                                     HTTPAuthentication:          HTTPAuthentication,
                                     SecWebSocketProtocols:       Subprotocols,
                                     RequestTimeout:              RequestTimeout,
                                     NextHopNetworkingNodeId:     NextHopNetworkingNodeId,
                                     RoutingNetworkingNodeIds:    RoutingNetworkingNodeIds,
                                     WebSocketPingEvery:          PingEvery,
                                     RemoteCertificateValidator:  RemoteCertificateValidator,
                                     ClientCertificates:          ClientCertificates,
                                     ClientCertificateContext:    ClientCertificateContext,
                                     TLSProtocols:                TLSProtocols,
                                     TOTPConfig:                  TOTPConfig,
                                     TOTPLogin:                   TOTPLogin,
                                     DNSClient:                   DNSClient,
                                     EventTrackingId:             EventTrackingId,
                                     CancellationToken:           CancellationToken

                                 );

            // The client was made for this URL just now: the last one for it.
            var webSocketClient = WWCPWebSocketClients.LastOrDefault(client => client.RemoteURL == RemoteURL);

            return new OCPPClientConnectResult(
                       RemoteURL,
                       MessageTransport.WebSocket,
                       webSocketClient,
                       response.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols
                           ? (webSocketClient as WWCP.WebSockets.WWCPWebSocketClient)?.WebSocketClientConnection
                           : null,
                       response,
                       null
                   );

        }

        #endregion

        #region ConnectOCPPSshClient    (RemoteURL, Ssh, ...)

        /// <summary>
        /// Connect to another networking node over the SSH subsystem "ocpp@open.charging.cloud", by its
        /// "ssh://" URL: who signs in is the URL's user name, or the SSH login's; the URL's last path segment,
        /// where it has one, names the networking node to connect as.
        /// </summary>
        /// <param name="RemoteURL">An "ssh://" URL: "ssh://cs001@lc.example.org:2222", "ssh://lc01@csms.example.org/cs001".</param>
        /// <param name="Ssh">The SSH login: user name, keys, how the host key is checked.</param>
        /// <param name="NextHopNetworkingNodeId">The networking node at the other end; by default the node's CSMS, see <see cref="DefaultNextHopNetworkingNodeId"/>.</param>
        /// <param name="RoutingNetworkingNodeIds">The networking nodes reached through it.</param>
        /// <param name="NetworkingMode">Whether messages carry their network path; standard by default.</param>
        /// <param name="Subprotocols">The OCPP versions to offer, by preference; the node's own by default.</param>
        /// <param name="PingEvery">Every how long to send a PING; null for none.</param>
        /// <param name="Description">An optional description.</param>
        /// <param name="DNSClient">The DNS client the URL's host name is resolved with; the node's by default. Never the resolver of the operating system.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <returns>The connection. A failure is thrown - and, with the node's reconnect policy, tried again in the background.</returns>
        public Task<SshMessageConnection> ConnectOCPPSshClient(URL                              RemoteURL,
                                                               SshClientOptions                 Ssh,
                                                               NetworkingNode_Id?               NextHopNetworkingNodeId    = null,
                                                               IEnumerable<NetworkingNode_Id>?  RoutingNetworkingNodeIds   = null,
                                                               NetworkingMode?                  NetworkingMode             = null,
                                                               IEnumerable<String>?             Subprotocols               = null,
                                                               TimeSpan?                        PingEvery                  = null,
                                                               I18NString?                      Description                = null,
                                                               IDNSClient?                      DNSClient                  = null,
                                                               CancellationToken                CancellationToken          = default)

            => ConnectOCPPSshClient(
                   WWCPSshClient.OptionsFrom(RemoteURL, Ssh, OCPPSshServer.OCPPSubsystem, Subprotocols, ReconnectPolicy, PingEvery, DNSClient ?? this.DNSClient),
                   NextHopNetworkingNodeId,
                   RoutingNetworkingNodeIds,
                   NetworkingMode,
                   WWCPSshClient.ConnectAsFrom(RemoteURL),
                   Description,
                   CancellationToken
               );

        #endregion

        #region ConnectOCPPSshClient    (Options,        ...)

        /// <summary>
        /// Connect to another networking node over an SSH subsystem, as the given options say. OCPP versions
        /// it names none of are the node's own; a reconnect policy it has none of is the node's.
        /// </summary>
        /// <param name="Options">Where and how to connect.</param>
        /// <param name="NextHopNetworkingNodeId">The networking node at the other end; by default the node's CSMS, see <see cref="DefaultNextHopNetworkingNodeId"/>.</param>
        /// <param name="RoutingNetworkingNodeIds">The networking nodes reached through it.</param>
        /// <param name="NetworkingMode">Whether messages carry their network path; standard by default.</param>
        /// <param name="ConnectAs">The networking node to connect as, where that is not the SSH login.</param>
        /// <param name="Description">An optional description.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <returns>The connection. A failure is thrown - and, with a reconnect policy, tried again in the background.</returns>
        public Task<SshMessageConnection> ConnectOCPPSshClient(SshMessageClientOptions          Options,
                                                               NetworkingNode_Id?               NextHopNetworkingNodeId    = null,
                                                               IEnumerable<NetworkingNode_Id>?  RoutingNetworkingNodeIds   = null,
                                                               NetworkingMode?                  NetworkingMode             = null,
                                                               NetworkingNode_Id?               ConnectAs                  = null,
                                                               I18NString?                      Description                = null,
                                                               CancellationToken                CancellationToken          = default)

            => ConnectSshClient(
                   NewOCPPSshClient(Options, NetworkingMode, ConnectAs, Description),
                   NextHopNetworkingNodeId ?? DefaultNextHopNetworkingNodeId,
                   RoutingNetworkingNodeIds,
                   CancellationToken
               );

        #endregion

        #region AttachOCPPSshSubsystem  (Subsystem = "ocpp@open.charging.cloud", Subprotocols = null, ...)

        /// <summary>
        /// Serve OCPP over an SSH subsystem. The SSH server is not this node's: hand the returned server's
        /// <see cref="WWCPSshServer.Handler"/> to its SshServerOptions.Subsystems. Who signs in is who
        /// connects; <see cref="WWCPSshServer.AllowToActFor"/> lets a login connect as others.
        /// </summary>
        /// <param name="Subsystem">The subsystem served.</param>
        /// <param name="Subprotocols">The OCPP versions served; the node's own by default.</param>
        /// <param name="MaxMessageSize">The largest message taken and sent.</param>
        /// <param name="PingEvery">Every how long a PING is sent on every connection; null for none.</param>
        public OCPPSshServer AttachOCPPSshSubsystem(String                Subsystem        = OCPPSshServer.OCPPSubsystem,
                                                    IEnumerable<String>?  Subprotocols     = null,
                                                    UInt64?               MaxMessageSize   = null,
                                                    TimeSpan?             PingEvery        = null)
        {

            var sshServer = new OCPPSshServer(
                                this,
                                Subsystem,
                                Subprotocols ?? DefaultOCPPServerSubprotocols,
                                MaxMessageSize,
                                PingEvery
                            );

            sshServer.OnJSONMessageReceived   += (timestamp, transport, connection, messageTimestamp, eventTrackingId, sourceNodeId, jsonMessage,   cancellationToken) =>
                                                     ProcessIncomingJSONMessage  (connection, messageTimestamp, sourceNodeId, jsonMessage,   eventTrackingId, cancellationToken);

            sshServer.OnBinaryMessageReceived += (timestamp, transport, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) =>
                                                     ProcessIncomingBinaryMessage(connection, messageTimestamp, sourceNodeId, binaryMessage, eventTrackingId, cancellationToken);

            AttachSshServer(sshServer);

            return sshServer;

        }

        #endregion

        #region (private)   NewOCPPSshClient(Options, NetworkingMode, ConnectAs, Description)

        /// <summary>
        /// A client whose messages go to this node's OCPP adapter, offering the node's OCPP versions where the
        /// options name none, and coming back as the node's reconnect policy says where they have none.
        /// </summary>
        private OCPPSshClient NewOCPPSshClient(SshMessageClientOptions  Options,
                                               NetworkingMode?          NetworkingMode,
                                               NetworkingNode_Id?       ConnectAs,
                                               I18NString?              Description)
        {

            var sshClient = new OCPPSshClient(
                                this,
                                Options with {
                                    Subprotocols  = Options.Subprotocols.Count > 0 ? Options.Subprotocols : [.. DefaultOCPPSubprotocols],
                                    Reconnect     = Options.Reconnect ?? ReconnectPolicy
                                },
                                NetworkingMode,
                                ConnectAs,
                                Description
                            );

            sshClient.OnJSONMessageReceived   += (timestamp, transport, connection, messageTimestamp, eventTrackingId, sourceNodeId, jsonMessage,   cancellationToken) =>
                                                     ProcessIncomingJSONMessage  (connection, messageTimestamp, sourceNodeId, jsonMessage,   eventTrackingId, cancellationToken);

            sshClient.OnBinaryMessageReceived += (timestamp, transport, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) =>
                                                     ProcessIncomingBinaryMessage(connection, messageTimestamp, sourceNodeId, binaryMessage, eventTrackingId, cancellationToken);

            return sshClient;

        }

        #endregion

        #region (protected) ProcessIncomingJSONMessage  (Connection, MessageTimestamp, SourceNodeId, JSONMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// A JSON message that came over SSH, for this node's OCPP adapter: each OCPP version hands it to its own.
        /// </summary>
        protected virtual Task ProcessIncomingJSONMessage(IMessageConnection  Connection,
                                                          DateTimeOffset      MessageTimestamp,
                                                          NetworkingNode_Id   SourceNodeId,
                                                          JArray              JSONMessage,
                                                          EventTracking_Id    EventTrackingId,
                                                          CancellationToken   CancellationToken)

            => Task.CompletedTask;

        #endregion

        #region (protected) ProcessIncomingBinaryMessage(Connection, MessageTimestamp, SourceNodeId, BinaryMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// A binary message that came over SSH, for this node's OCPP adapter: each OCPP version hands it to its own.
        /// </summary>
        protected virtual Task ProcessIncomingBinaryMessage(IMessageConnection  Connection,
                                                            DateTimeOffset      MessageTimestamp,
                                                            NetworkingNode_Id   SourceNodeId,
                                                            Byte[]              BinaryMessage,
                                                            EventTracking_Id    EventTrackingId,
                                                            CancellationToken   CancellationToken)

            => Task.CompletedTask;

        #endregion



        #region (protected) WireWebSocketServer(WebSocketServer)

        //protected void WireWebSocketServer(OCPPWebSocketServer WebSocketServer)
        //{

        //    base.WireWebSocketServer(WebSocketServer);

        //    // (Generic) Error Handling

        //}

        #endregion



        #region (Timer) DoMaintenance(State)

        //protected override async Task DoMaintenanceAsync(Object? State)
        //{

        //    await Task.Delay(1);

        //    if (Timestamp.Now > lastRoutesBroadcast + TimeSpan.FromSeconds(10))
        //    {

        //        lastRoutesBroadcast = Timestamp.Now;

        //        var routes = Routing.GetNetworkRoutingInformation();
        //        if (routes.Any())
        //        {

        //        //    var notifyNetworkTopologyMessage = new NotifyNetworkTopologyMessage(
        //        //                                           Destination:                  SourceRouting.Broadcast,
        //        //                                           NetworkTopologyInformation:   new NetworkTopologyInformation(
        //        //                                                                             RoutingNode:  Id,
        //        //                                                                             Routes:       routes,
        //        //                                                                             NotBefore:    null,
        //        //                                                                             NotAfter:     null,
        //        //                                                                             Priority:     null
        //        //                                                                         )
        //        //                                       );

        //        //    var rr = OCPP.OUT.NotifyNetworkTopology(notifyNetworkTopologyMessage);

        //        }

        //    }

        //}

        #endregion



        #region LogEvent(OCPPIO, Logger, LogHandler, ...)

        //public async Task LogEvent<TDelegate>(String                                             OCPPIO,
        //                                      TDelegate?                                         Logger,
        //                                      Func<TDelegate, Task>                              LogHandler,
        //                                      [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
        //                                      [CallerMemberName()]                       String  OCPPCommand   = "")

        //    where TDelegate : Delegate

        //{
        //    if (Logger is not null)
        //    {
        //        try
        //        {

        //            await Task.WhenAll(
        //                      Logger.GetInvocationList().
        //                             OfType<TDelegate>().
        //                             Select(LogHandler)
        //                  );

        //        }
        //        catch (Exception e)
        //        {
        //            await HandleErrors(OCPPIO, $"{OCPPCommand}.{EventName}", e);
        //        }
        //    }
        //}

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ErrorResponse)

        //public virtual Task HandleErrors(String  Module,
        //                                 String  Caller,
        //                                 String  ErrorResponse)
        //{

        //    DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

        //    return Task.CompletedTask;

        //}

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccurred)

        //public virtual Task HandleErrors(String     Module,
        //                                 String     Caller,
        //                                 Exception  ExceptionOccurred)
        //{

        //    DebugX.LogException(ExceptionOccurred, Caller);

        //    return Task.CompletedTask;

        //}

        #endregion


    }

}
