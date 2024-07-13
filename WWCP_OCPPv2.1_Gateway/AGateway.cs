/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.Gateway
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Properties

        public Connector_Id    Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region ChargingStationConnector(Id, ConnectorType)

        public ChargingStationConnector(Connector_Id    Id,
                                        ConnectorType  ConnectorType)
        {

            this.Id             = Id;
            this.ConnectorType  = ConnectorType;

        }

        #endregion

    }

    #endregion

    #region (class) ChargingStationEVSE

    /// <summary>
    /// An EVSE at a charging station.
    /// </summary>
    public class ChargingStationEVSE
    {

        #region Properties

        public EVSE_Id            Id                       { get; }

        public Reservation_Id?    ReservationId            { get; set; }

        public OperationalStatus  AdminStatus              { get; set; }

        public ConnectorStatus    Status                   { get; set; }


        public String?            MeterType                { get; set; }
        public String?            MeterSerialNumber        { get; set; }
        public String?            MeterPublicKey           { get; set; }


        public Boolean            IsReserved               { get; set; }

        public Boolean            IsCharging               { get; set; }

        public IdToken?           IdToken                  { get; set; }

        public IdToken?           GroupIdToken             { get; set; }

        public Transaction_Id?    TransactionId            { get; set; }

        public RemoteStart_Id?    RemoteStartId            { get; set; }

        public ChargingProfile?   ChargingProfile          { get; set; }


        public DateTime?          StartTimestamp           { get; set; }

        public Decimal?           MeterStartValue          { get; set; }

        public String?            SignedStartMeterValue    { get; set; }

        public DateTime?          StopTimestamp            { get; set; }

        public Decimal?           MeterStopValue           { get; set; }

        public String?            SignedStopMeterValue     { get; set; }


        public ChargingTariff?    DefaultChargingTariff    { get; set; }

        #endregion

        #region ChargingStationEVSE(Id, AdminStatus, ...)

        public ChargingStationEVSE(EVSE_Id                                 Id,
                                   OperationalStatus                       AdminStatus,
                                   String?                                 MeterType           = null,
                                   String?                                 MeterSerialNumber   = null,
                                   String?                                 MeterPublicKey      = null,
                                   IEnumerable<ChargingStationConnector>?  Connectors          = null)
        {

            this.Id                 = Id;
            this.AdminStatus        = AdminStatus;
            this.MeterType          = MeterType;
            this.MeterSerialNumber  = MeterSerialNumber;
            this.MeterPublicKey     = MeterPublicKey;
            this.connectors         = Connectors is not null && Connectors.Any()
                                          ? new HashSet<ChargingStationConnector>(Connectors)
                                          : new HashSet<ChargingStationConnector>();

        }

        #endregion


        #region Connectors

        private readonly HashSet<ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }

        #endregion


    }

    #endregion


    /// <summary>
    /// An abstract gateway.
    /// </summary>
    public abstract class AGateway : ANetworkingNode,
                                     GW.IGateway
    {

        #region Data

        private readonly HTTPExtAPI  HTTPAPI;

        #endregion

        #region Properties

        /// <summary>
        /// The networking node vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                 { get; }      = "";

        /// <summary>
        ///  The networking node model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                      { get; }      = "";

        /// <summary>
        /// The optional serial number of the networking node.
        /// </summary>
        [Optional]
        public String?                     SerialNumber               { get; }

        /// <summary>
        /// The optional firmware version of the networking node.
        /// </summary>
        [Optional]
        public String?                     SoftwareVersion            { get; }


        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                   CSMSTime                   { get; set; } = Timestamp.Now;



        public WebAPI                      WebAPI                     { get; }

        private readonly HashSet<WebAPI> webAPIs = [];

        /// <summary>
        /// An enumeration of all WebAPIs.
        /// </summary>
        public IEnumerable<WebAPI> WebAPIs
            => webAPIs;

        #endregion

        #region Events

        #region WebSocket connections

        ///// <summary>
        ///// An event sent whenever the HTTP web socket server started.
        ///// </summary>
        //public event OnServerStartedDelegate?                 OnServerStarted;

        ///// <summary>
        ///// An event sent whenever a new TCP connection was accepted.
        ///// </summary>
        //public event OnValidateTCPConnectionDelegate?         OnValidateTCPConnection;

        ///// <summary>
        ///// An event sent whenever a new TCP connection was accepted.
        ///// </summary>
        //public event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        ///// <summary>
        ///// An event sent whenever a HTTP request was received.
        ///// </summary>
        //public event HTTPRequestLogDelegate?                  OnHTTPRequest;

        ///// <summary>
        ///// An event sent whenever the HTTP headers of a new web socket connection
        ///// need to be validated or filtered by an upper layer application logic.
        ///// </summary>
        //public event OnValidateWebSocketConnectionDelegate?   OnValidateWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever the HTTP connection switched successfully to web socket.
        ///// </summary>
        //public event OnCSMSNewWebSocketConnectionDelegate?    OnNewWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever a reponse to a HTTP request was sent.
        ///// </summary>
        //public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        ///// <summary>
        ///// An event sent whenever a web socket close frame was received.
        ///// </summary>
        //public event OnCSMSCloseMessageReceivedDelegate?      OnCloseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a TCP connection was closed.
        ///// </summary>
        //public event OnCSMSTCPConnectionClosedDelegate?       OnTCPConnectionClosed;

        ///// <summary>
        ///// An event sent whenever the HTTP web socket server stopped.
        ///// </summary>
        //public event OnServerStoppedDelegate?                 OnServerStopped;

        #region WebSocket connections

        /// <summary>
        /// An event sent whenever the HTTP web socket server started.
        /// </summary>
        public event OnServerStartedDelegate?                 OnServerStarted;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnValidateTCPConnectionDelegate?         OnValidateTCPConnection;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        /// <summary>
        /// An event sent whenever a HTTP request was received.
        /// </summary>
        public event HTTPRequestLogDelegate?                  OnHTTPRequest;

        /// <summary>
        /// An event sent whenever the HTTP headers of a new web socket connection
        /// need to be validated or filtered by an upper layer application logic.
        /// </summary>
        public event OnValidateWebSocketConnectionDelegate?   OnValidateWebSocketConnection;

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnCSMSNewWebSocketConnectionDelegate?    OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCSMSCloseMessageReceivedDelegate?      OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnCSMSTCPConnectionClosedDelegate?       OnTCPConnectionClosed;

        /// <summary>
        /// An event sent whenever the HTTP web socket server stopped.
        /// </summary>
        public event OnServerStoppedDelegate?                 OnServerStopped;

        #endregion

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a JSON message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a JSON message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract gateway.
        /// </summary>
        /// <param name="Id">The unique identification of this gateway.</param>
        public AGateway(NetworkingNode_Id  Id,
                        String             VendorName,
                        String             Model,
                        String?            SerialNumber                = null,
                        String?            SoftwareVersion             = null,
                        I18NString?        Description                 = null,

                        SignaturePolicy?   SignaturePolicy             = null,
                        SignaturePolicy?   ForwardingSignaturePolicy   = null,

                        Boolean            DisableSendHeartbeats       = false,
                        TimeSpan?          SendHeartbeatsEvery         = null,
                        TimeSpan?          DefaultRequestTimeout       = null,

                        Boolean            DisableMaintenanceTasks     = false,
                        TimeSpan?          MaintenanceEvery            = null,
                        DNSClient?         DNSClient                   = null)

            : base(Id,
                   Description,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   DNSClient)

        {

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given vendor name must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given model must not be null or empty!");

            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.SerialNumber             = SerialNumber;
            this.SoftwareVersion          = SoftwareVersion;

            #region Setup generic HTTP API

            this.HTTPAPI  = new HTTPExtAPI(
                                HTTPServerPort:         IPPort.Parse(3532),
                                HTTPServerName:         "GraphDefined OCPP Test Gateway",
                                HTTPServiceName:        "GraphDefined OCPP Test Gateway Service",
                                APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Gateway Robot <robot@charging.cloud>"),
                                APIRobotGPGPassphrase:  "test123",
                                SMTPClient:             new NullMailer(),
                                DNSClient:              DNSClient,
                                AutoStart:              true
                            );

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            #endregion

            #region HTTP API Security Settings

            var webAPIPrefix = "webapi";

            this.HTTPAPI.HTTPServer.AddAuth(request => {

                // Allow some URLs for anonymous access...
                if (request.Path.StartsWith(HTTPAPI.URLPathPrefix + webAPIPrefix))
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });

            #endregion

            #region Setup WebAPIs

            this.WebAPI             = new WebAPI(
                                          this,
                                          HTTPAPI,

                                          URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

                                      );

            #endregion

        }

        #endregion


        #region ConnectWebSocket(...)

        //public Task<HTTPResponse?> ConnectWebSocket(URL                                  RemoteURL,
        //                                            HTTPHostname?                        VirtualHostname              = null,
        //                                            String?                              Description                  = null,
        //                                            RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
        //                                            LocalCertificateSelectionHandler?    LocalCertificateSelector     = null,
        //                                            X509Certificate?                     ClientCert                   = null,
        //                                            SslProtocols?                        TLSProtocol                  = null,
        //                                            Boolean?                             PreferIPv4                   = null,
        //                                            String?                              HTTPUserAgent                = null,
        //                                            IHTTPAuthentication?                 HTTPAuthentication           = null,
        //                                            TimeSpan?                            RequestTimeout               = null,
        //                                            TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
        //                                            UInt16?                              MaxNumberOfRetries           = null,
        //                                            UInt32?                              InternalBufferSize           = null,

        //                                            IEnumerable<String>?                 SecWebSocketProtocols        = null,
        //                                            NetworkingMode?                      NetworkingMode               = null,

        //                                            Boolean                              DisableMaintenanceTasks      = false,
        //                                            TimeSpan?                            MaintenanceEvery             = null,
        //                                            Boolean                              DisableWebSocketPings        = false,
        //                                            TimeSpan?                            WebSocketPingEvery           = null,
        //                                            TimeSpan?                            SlowNetworkSimulationDelay   = null,

        //                                            String?                              LoggingPath                  = null,
        //                                            String?                              LoggingContext               = null,
        //                                            LogfileCreatorDelegate?              LogfileCreator               = null,
        //                                            HTTPClientLogger?                    HTTPLogger                   = null,
        //                                            DNSClient?                           DNSClient                    = null)

        //    => AsCS.ConnectWebSocket(RemoteURL,
        //                             VirtualHostname,
        //                             Description,
        //                             RemoteCertificateValidator,
        //                             LocalCertificateSelector,
        //                             ClientCert,
        //                             TLSProtocol,
        //                             PreferIPv4,
        //                             HTTPUserAgent,
        //                             HTTPAuthentication,
        //                             RequestTimeout,
        //                             TransmissionRetryDelay,
        //                             MaxNumberOfRetries,
        //                             InternalBufferSize,

        //                             SecWebSocketProtocols,
        //                             NetworkingMode,

        //                             DisableMaintenanceTasks,
        //                             MaintenanceEvery,
        //                             DisableWebSocketPings,
        //                             WebSocketPingEvery,
        //                             SlowNetworkSimulationDelay,

        //                             LoggingPath,
        //                             LoggingContext,
        //                             LogfileCreator,
        //                             HTTPLogger,
        //                             DNSClient);

        #endregion


        #region ConnectWebSocketClient(...)

        public async Task<HTTPResponse> ConnectWebSocketClient(NetworkingNode_Id                                               NetworkingNodeId,
                                                               URL                                                             RemoteURL,
                                                               HTTPHostname?                                                   VirtualHostname              = null,
                                                               String?                                                         Description                  = null,
                                                               Boolean?                                                        PreferIPv4                   = null,
                                                               RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
                                                               LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                                               X509Certificate?                                                ClientCert                   = null,
                                                               SslProtocols?                                                   TLSProtocol                  = null,
                                                               String?                                                         HTTPUserAgent                = null,
                                                               IHTTPAuthentication?                                            HTTPAuthentication           = null,
                                                               TimeSpan?                                                       RequestTimeout               = null,
                                                               TransmissionRetryDelayDelegate?                                 TransmissionRetryDelay       = null,
                                                               UInt16?                                                         MaxNumberOfRetries           = 3,
                                                               UInt32?                                                         InternalBufferSize           = null,

                                                               IEnumerable<String>?                                            SecWebSocketProtocols        = null,
                                                               NetworkingMode?                                                 NetworkingMode               = null,

                                                               Boolean                                                         DisableWebSocketPings        = false,
                                                               TimeSpan?                                                       WebSocketPingEvery           = null,
                                                               TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                               Boolean                                                         DisableMaintenanceTasks      = false,
                                                               TimeSpan?                                                       MaintenanceEvery             = null,

                                                               String?                                                         LoggingPath                  = null,
                                                               String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
                                                               LogfileCreatorDelegate?                                         LogfileCreator               = null,
                                                               HTTPClientLogger?                                               HTTPLogger                   = null,
                                                               DNSClient?                                                      DNSClient                    = null)
        {

            var ocppWebSocketClient = new OCPPWebSocketClient(

                                          OCPP,

                                          RemoteURL,
                                          VirtualHostname,
                                          Description,
                                          PreferIPv4,
                                          RemoteCertificateValidator,
                                          LocalCertificateSelector,
                                          ClientCert,
                                          TLSProtocol,
                                          HTTPUserAgent,
                                          HTTPAuthentication,
                                          RequestTimeout,
                                          TransmissionRetryDelay,
                                          MaxNumberOfRetries,
                                          InternalBufferSize,

                                          SecWebSocketProtocols ?? [
                                                                      "ocpp2.0.1",
                                                                       Version.WebSocketSubProtocolId
                                                                   ],
                                          NetworkingMode,

                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DisableMaintenanceTasks,
                                          MaintenanceEvery,

                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator,
                                          HTTPLogger,
                                          DNSClient

                                      );

            ocppWebSocketClients.Add(ocppWebSocketClient);

            var connectResponse = await ocppWebSocketClient.Connect();

            connectResponse.Item1.TryAddCustomData(OCPPAdapter.NetworkingNodeId_WebSocketKey,
                                                   NetworkingNodeId);

            OCPP.AddStaticRouting(NetworkingNodeId,
                                  ocppWebSocketClient,
                                  0,
                                  Timestamp.Now);

            return connectResponse.Item2;

        }

        #endregion


        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        /// 
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer AttachWebSocketServer(String?                                                         HTTPServiceName              = null,
                                                         IIPAddress?                                                     IPAddress                    = null,
                                                         IPPort?                                                         TCPPort                      = null,
                                                         I18NString?                                                     Description                  = null,

                                                         Boolean                                                         RequireAuthentication        = true,
                                                         Boolean                                                         DisableWebSocketPings        = false,
                                                         TimeSpan?                                                       WebSocketPingEvery           = null,
                                                         TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                         Func<X509Certificate2>?                                         ServerCertificateSelector    = null,
                                                         RemoteTLSClientCertificateValidationHandler<IWebSocketServer>?  ClientCertificateValidator   = null,
                                                         LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                                         SslProtocols?                                                   AllowedTLSProtocols          = null,
                                                         Boolean?                                                        ClientCertificateRequired    = null,
                                                         Boolean?                                                        CheckCertificateRevocation   = null,

                                                         ServerThreadNameCreatorDelegate?                                ServerThreadNameCreator      = null,
                                                         ServerThreadPriorityDelegate?                                   ServerThreadPrioritySetter   = null,
                                                         Boolean?                                                        ServerThreadIsBackground     = null,
                                                         ConnectionIdBuilder?                                            ConnectionIdBuilder          = null,
                                                         TimeSpan?                                                       ConnectionTimeout            = null,
                                                         UInt32?                                                         MaxClientConnections         = null,

                                                         Boolean                                                         AutoStart                    = false)
        {

            var ocppWebSocketServer = new OCPPWebSocketServer(

                                          OCPP,

                                          HTTPServiceName,
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          ServerCertificateSelector,
                                          ClientCertificateValidator,
                                          LocalCertificateSelector,
                                          AllowedTLSProtocols,
                                          ClientCertificateRequired,
                                          CheckCertificateRevocation,

                                          ServerThreadNameCreator,
                                          ServerThreadPrioritySetter,
                                          ServerThreadIsBackground,
                                          ConnectionIdBuilder,
                                          ConnectionTimeout,
                                          MaxClientConnections,

                                          DNSClient,
                                          AutoStart: false

                                      );

            WireWebSocketServer(ocppWebSocketServer);

            if (AutoStart)
                ocppWebSocketServer.Start();

            return ocppWebSocketServer;

        }

        #endregion

        #region (private) WireWebSocketServer(WebSocketServer)

        private void WireWebSocketServer(OCPPWebSocketServer WebSocketServer)
        {

            ocppWebSocketServers.Add(WebSocketServer);

            #region WebSocket related

            #region OnServerStarted

            WebSocketServer.OnServerStarted += async (timestamp,
                                                      server,
                                                      eventTrackingId,
                                                      cancellationToken) => {

                var onServerStarted = OnServerStarted;
                if (onServerStarted is not null)
                {
                    try
                    {

                        await Task.WhenAll(onServerStarted.GetInvocationList().
                                               OfType <OnServerStartedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnServerStarted),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnNewTCPConnection

            WebSocketServer.OnNewTCPConnection += async (timestamp,
                                                         webSocketServer,
                                                         newTCPConnection,
                                                         eventTrackingId,
                                                         cancellationToken) => {

                var onNewTCPConnection = OnNewTCPConnection;
                if (onNewTCPConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNewTCPConnection.GetInvocationList().
                                               OfType <OnNewTCPConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              newTCPConnection,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnNewTCPConnection),
                                  e
                              );
                    }
                }

            };

            #endregion

            // Failed (Charging Station) Authentication

            #region OnNetworkingNodeNewWebSocketConnection

            WebSocketServer.OnNetworkingNodeNewWebSocketConnection += async (timestamp,
                                                                             ocppWebSocketServer,
                                                                             newConnection,
                                                                             networkingNodeId,
                                                                             eventTrackingId,
                                                                             sharedSubprotocols,
                                                                             cancellationToken) => {

                // A new connection from the same networking node/charging station will replace the older one!
                OCPP.AddStaticRouting(DestinationNodeId:  networkingNodeId,
                                      WebSocketServer:    ocppWebSocketServer,
                                      Priority:           0,
                                      Timestamp:          timestamp);

                #region Send OnNewWebSocketConnection

                var logger = OnNewWebSocketConnection;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeNewWebSocketConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              ocppWebSocketServer,
                                                                              newConnection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              sharedSubprotocols,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnNewWebSocketConnection),
                                  e
                              );
                    }
                }

                #endregion

            };

            #endregion

            #region OnNetworkingNodeCloseMessageReceived

            WebSocketServer.OnNetworkingNodeCloseMessageReceived += async (timestamp,
                                                             server,
                                                             connection,
                                                             networkingNodeId,
                                                             eventTrackingId,
                                                             statusCode,
                                                             reason,
                                                             cancellationToken) => {

                var logger = OnCloseMessageReceived;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeCloseMessageReceivedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              connection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              statusCode,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnCloseMessageReceived),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnNetworkingNodeTCPConnectionClosed

            WebSocketServer.OnNetworkingNodeTCPConnectionClosed += async (timestamp,
                                                            server,
                                                            connection,
                                                            networkingNodeId,
                                                            eventTrackingId,
                                                            reason,
                                                            cancellationToken) => {

                var logger = OnTCPConnectionClosed;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeTCPConnectionClosedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              connection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnTCPConnectionClosed),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnServerStopped

            WebSocketServer.OnServerStopped += async (timestamp,
                                                  server,
                                                  eventTrackingId,
                                                  reason,
                                                  cancellationToken) => {

                var logger = OnServerStopped;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                                 OfType <OnServerStoppedDelegate>().
                                                 Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                timestamp,
                                                                                server,
                                                                                eventTrackingId,
                                                                                reason,
                                                                                cancellationToken
                                                                            )).
                                                 ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestGateway),
                                  nameof(OnServerStopped),
                                  e
                              );
                    }
                }

            };

            #endregion

            // (Generic) Error Handling

            #endregion

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            await Task.WhenAll(ocppWebSocketServers.
                                   Select (ocppWebSocketServer => ocppWebSocketServer.Shutdown(Message, Wait)).
                                   ToArray());

        }

        #endregion


        #region AttachWebAPI(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public WebAPI AttachWebAPI(HTTPHostname?                               HTTPHostname            = null,
                                                 String?                                     ExternalDNSName         = null,
                                                 IPPort?                                     HTTPServerPort          = null,
                                                 HTTPPath?                                   BasePath                = null,
                                                 String?                                     HTTPServerName          = null,

                                                 HTTPPath?                                   URLPathPrefix           = null,
                                                 String?                                     HTTPServiceName         = null,

                                                 String                                      HTTPRealm               = "...",
                                                 Boolean                                     RequireAuthentication   = true,
                                                 IEnumerable<KeyValuePair<String, String>>?  HTTPLogins              = null,

                                                 String?                                     HTMLTemplate            = null,
                                                 Boolean                                     AutoStart               = false)
        {

            var httpAPI  = new HTTPExtAPI(

                               HTTPHostname,
                               ExternalDNSName,
                               HTTPServerPort,
                               BasePath,
                               HTTPServerName,

                               URLPathPrefix,
                               HTTPServiceName,

                               DNSClient: DNSClient,
                               AutoStart: false

                           );

            var webAPI   = new WebAPI(

                               this,
                               httpAPI,

                               HTTPServerName,
                               URLPathPrefix,
                               BasePath,
                               HTTPRealm,
                               HTTPLogins,
                               HTMLTemplate

                           );

            if (AutoStart)
                httpAPI.Start();

            return webAPI;

        }

        #endregion


        #region (override) HandleErrors(Module, Caller, ErrorResponse)

        public override Task HandleErrors(String  Module,
                                          String  Caller,
                                          String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ErrorResponse
                   );

        }

        #endregion

        #region (override) HandleErrors(Module, Caller, ExceptionOccured)

        public override Task HandleErrors(String     Module,
                                          String     Caller,
                                          Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ExceptionOccured
                   );

        }

        #endregion


    }

}
