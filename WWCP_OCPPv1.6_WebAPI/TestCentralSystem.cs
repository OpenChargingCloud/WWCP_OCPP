/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using social.OpenData.UsersAPI;

using cloud.charging.open.protocols.OCPPv1_6.CS;
using System.Threading;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCentralSystem : IEventSender
    {

        #region Data

        private readonly HashSet<ICentralSystemServer> centralSystemServers;

        private readonly Dictionary<ChargeBox_Id, Tuple<ICentralSystem, DateTime>> reachableChargingBoxes;

        private readonly UsersAPI    TestAPI;

        private readonly OCPPWebAPI  WebAPI;

        protected static readonly SemaphoreSlim ChargeBoxesSemaphore = new SemaphoreSlim(1, 1);

        protected static readonly TimeSpan SemaphoreSlimTimeout = TimeSpan.FromSeconds(5);

        public static readonly IPPort DefaultHTTPUploadPort = IPPort.Parse(9901);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public CentralSystem_Id  CentralSystemId    { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => CentralSystemId.ToString();


        public UploadAPI  HTTPUploadAPI     { get; }

        public IPPort     HTTPUploadPort    { get; }

        public DNSClient  DNSClient         { get; }


        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<ICentralSystemServer> CentralSystemServers
            => centralSystemServers;


        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean RequireAuthentication { get; }


        /// <summary>
        /// The unique identifications of all connected or reachable charge boxes.
        /// </summary>
        public IEnumerable<ChargeBox_Id> ChargeBoxIds
            => reachableChargingBoxes.Values.SelectMany(box => box.Item1.ChargeBoxIds);


        public Dictionary<String, Transaction_Id> TransactionIds = new Dictionary<String, Transaction_Id>();

        #endregion

        #region Events

        // CP -> CS

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event BootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        public event BootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event HeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat request was sent.
        /// </summary>
        public event HeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event AuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a response to an authorize request was sent.
        /// </summary>
        public event AuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a StartTransaction request was received.
        /// </summary>
        public event StartTransactionRequestDelegate?   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a StartTransaction request was sent.
        /// </summary>
        public event StartTransactionResponseDelegate?  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event StatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event StatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event MeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        public event MeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a StopTransaction request was received.
        /// </summary>
        public event StopTransactionRequestDelegate?   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a StopTransaction request was sent.
        /// </summary>
        public event StopTransactionResponseDelegate?  OnStopTransactionResponse;

        #endregion


        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever an IncomingDataTransfer request was received.
        /// </summary>
        public event IncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingDataTransfer request was sent.
        /// </summary>
        public event IncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event DiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a DiagnosticsStatusNotification request was sent.
        /// </summary>
        public event DiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event FirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event FirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion


        // CS -> CP

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.GetConfigurationRequestDelegate?   OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.GetConfigurationResponseDelegate?  OnGetConfigurationResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ChangeConfigurationRequestDelegate?   OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ChangeConfigurationResponseDelegate?  OnChangeConfigurationResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.IncomingDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.IncomingDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.GetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.GetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.TriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.TriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.UpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.UpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.CancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.CancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.RemoteStartTransactionRequestDelegate?   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.RemoteStartTransactionResponseDelegate?  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.RemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.RemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.SetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.SetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.GetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.GetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.UnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.UnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.GetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.GetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.SendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.SendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CP.ClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CP.ClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion



        // WebSocket events

        #region OnNewTCPConnection

        public event OnNewTCPConnectionDelegate?                 OnNewTCPConnection;

        public event OnNewWebSocketConnectionDelegate?           OnNewWebSocketConnection;

        #endregion

        #region OnMessage

        //public event OnWebSocketMessageDelegate                 OnMessage;

        #endregion

        #region OnTextMessage  (Request/Response)

        public event OnWebSocketTextMessageRequestDelegate?      OnTextMessageRequest;

        //public event OnWebSocketTextMessageDelegate             OnTextMessage;

        public event OnWebSocketTextMessageResponseDelegate?     OnTextMessageResponse;

        #endregion

        #region OnBinaryMessage(Request/Response)

        public event OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequest;

        //public event OnWebSocketBinaryMessageDelegate           OnBinaryMessage;

        public event OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponse;

        #endregion

        #region On(Ping/Pong)Message

        //public event OnWebSocketMessageDelegate                 OnPingMessage;

        //public event OnWebSocketMessageDelegate                 OnPongMessage;

        #endregion

        #region OnCloseMessage

        public event OnCloseMessageDelegate?                     OnCloseMessage;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="CentralSystemId">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        public TestCentralSystem(CentralSystem_Id  CentralSystemId,
                                 Boolean           RequireAuthentication   = true,
                                 IPPort?           HTTPUploadPort          = null,
                                 DNSClient         DNSClient               = null)
        {

            if (CentralSystemId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(CentralSystemId), "The given central system identification must not be null or empty!");

            this.CentralSystemId         = CentralSystemId;
            this.RequireAuthentication   = RequireAuthentication;
            this.HTTPUploadPort          = HTTPUploadPort ?? DefaultHTTPUploadPort;
            this.centralSystemServers    = new HashSet<ICentralSystemServer>();
            this.reachableChargingBoxes  = new Dictionary<ChargeBox_Id, Tuple<ICentralSystem, DateTime>>();
            this._ChargeBoxes            = new Dictionary<ChargeBox_Id, ChargeBox>();

            Directory.CreateDirectory("HTTPSSEs");

            this.TestAPI                 = new UsersAPI(HTTPServerPort:        IPPort.Parse(3500),
                                                        HTTPServerName:        "GraphDefined OCPP Test Central System",
                                                        HTTPServiceName:       "GraphDefined OCPP Test Central System Service",
                                                        APIRobotEMailAddress:  EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                                        SMTPClient:            new NullMailer(),
                                                        DNSClient:             DNSClient,
                                                        Autostart:             true);

            this.HTTPUploadAPI           = new UploadAPI(this,
                                                         new HTTPServer(this.HTTPUploadPort,
                                                                        "Open Charging Cloud OCPP Upload Server",
                                                                        "Open Charging Cloud OCPP Upload Service"));

            this.WebAPI                  = new OCPPWebAPI(this,
                                                          TestAPI.HTTPServer);

            this.DNSClient               = DNSClient;

        }

        #endregion


        #region CreateSOAPService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/SOAP.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemSOAPServer CreateSOAPService(String           HTTPServerName            = CentralSystemSOAPServer.DefaultHTTPServerName,
                                                         IPPort?          TCPPort                   = null,
                                                         String           ServiceName               = null,
                                                         HTTPPath?        URLPrefix                 = null,
                                                         HTTPContentType  ContentType               = null,
                                                         Boolean          RegisterHTTPRootService   = true,
                                                         DNSClient        DNSClient                 = null,
                                                         Boolean          AutoStart                 = false)
        {

            var centralSystemServer = new CentralSystemSOAPServer(HTTPServerName,
                                                                  TCPPort,
                                                                  ServiceName,
                                                                  URLPrefix,
                                                                  ContentType,
                                                                  RegisterHTTPRootService,
                                                                  DNSClient ?? this.DNSClient,
                                                                  AutoStart);

            Attach(centralSystemServer);

            return centralSystemServer;

        }

        #endregion

        #region CreateWebSocketService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer CreateWebSocketService(String      HTTPServerName   = CentralSystemWSServer.DefaultHTTPServerName,
                                                            IIPAddress  IPAddress        = null,
                                                            IPPort?     TCPPort          = null,
                                                            DNSClient   DNSClient        = null,
                                                            Boolean     AutoStart        = false)
        {

            var centralSystemServer = new CentralSystemWSServer(HTTPServerName,
                                                                IPAddress,
                                                                TCPPort,
                                                                RequireAuthentication,
                                                                DNSClient ?? this.DNSClient,
                                                                AutoStart);

            Attach(centralSystemServer);


            #region OnNewTCPConnection

            centralSystemServer.OnNewTCPConnection += async (Timestamp,
                                                             WebSocketServer,
                                                             NewWebSocketConnection,
                                                             EventTrackingId,
                                                             CancellationToken) => {

                OnNewTCPConnection?.Invoke(Timestamp,
                                           WebSocketServer,
                                           NewWebSocketConnection,
                                           EventTrackingId,
                                           CancellationToken);

            };

            #endregion

            #region OnNewWebSocketConnection

            centralSystemServer.OnNewWebSocketConnection += async (Timestamp,
                                                                   WebSocketServer,
                                                                   NewWebSocketConnection,
                                                                   EventTrackingId,
                                                                   CancellationToken) => {

                OnNewWebSocketConnection?.Invoke(Timestamp,
                                                 WebSocketServer,
                                                 NewWebSocketConnection,
                                                 EventTrackingId,
                                                 CancellationToken);

            };

            #endregion


            //OnMessage;


            #region OnTextMessageRequest


            //centralSystemServer.OnTextMessageRequest += (timestamp,
            //                                             webSocketServer,
            //                                             webSocketConnection,
            //                                             webSocketTextMessageRequest,
            //                                             cancellationToken) => {

            //    OnTextMessageRequest?.Invoke(Timestamp,
            //                                 WebSocketServer,
            //                                 Sender,
            //                                 TextRequestMessage,
            //                                 EventTrackingId,
            //                                 CancellationToken);

            //};


            #endregion

            #region OnTextMessageResponse

            //centralSystemServer.OnTextMessageResponse += async (Timestamp,
            //                                                    WebSocketServer,
            //                                                    Sender,
            //                                                    TextRequestMessage,
            //                                                    TextResponseMessage,
            //                                                    EventTrackingId,
            //                                                    CancellationToken) => {

            //    OnTextMessageResponse?.Invoke(Timestamp,
            //                                  WebSocketServer,
            //                                  Sender,
            //                                  TextRequestMessage,
            //                                  TextResponseMessage,
            //                                  EventTrackingId,
            //                                  CancellationToken);

            //};

            #endregion


            #region OnBinaryMessageRequest

            //centralSystemServer.OnBinaryMessageRequest += async (Timestamp,
            //                                                     WebSocketServer,
            //                                                     Sender,
            //                                                     BinaryRequestMessage,
            //                                                     EventTrackingId,
            //                                                     CancellationToken) => {

            //    OnBinaryMessageRequest?.Invoke(Timestamp,
            //                                   WebSocketServer,
            //                                   Sender,
            //                                   BinaryRequestMessage,
            //                                   EventTrackingId,
            //                                   CancellationToken);

            //};

            #endregion

            #region OnBinaryMessageResponse

            //centralSystemServer.OnBinaryMessageResponse += async (Timestamp,
            //                                                      WebSocketServer,
            //                                                      Sender,
            //                                                      BinaryRequestMessage,
            //                                                      BinaryResponseMessage,
            //                                                      EventTrackingId,
            //                                                      CancellationToken) => {

            //    OnBinaryMessageResponse?.Invoke(Timestamp,
            //                                    WebSocketServer,
            //                                    Sender,
            //                                    BinaryRequestMessage,
            //                                    BinaryResponseMessage,
            //                                    EventTrackingId,
            //                                    CancellationToken);

            //};

            #endregion


            //OnPingMessage;

            //OnPongMessage;


            //OnCloseMessage;


            return centralSystemServer;

        }

        #endregion

        #region Attach(CentralSystemServer)

        public void Attach(ICentralSystemServer CentralSystemServer)
        {

            #region Initial checks

            if (CentralSystemServer is null)
                throw new ArgumentNullException(nameof(CentralSystemServer), "The given central system must not be null!");

            #endregion


            centralSystemServers.Add(CentralSystemServer);


            if (CentralSystemServer is CentralSystemWSServer centralSystemWSServer)
            {
                centralSystemWSServer.OnNewCentralSystemWSConnection += async (LogTimestamp,
                                                                               CentralSystem,
                                                                               Connection,
                                                                               EventTrackingId,
                                                                               CancellationToken) =>
                {

                    if (Connection.TryGetCustomData("chargeBoxId", out ChargeBox_Id chargeBoxId))
                    {
                        //ToDo: lock(...)
                        if (!reachableChargingBoxes.ContainsKey(chargeBoxId))
                            reachableChargingBoxes.Add(chargeBoxId, new Tuple<ICentralSystem, DateTime>(CentralSystem, Timestamp.Now));
                        else
                            reachableChargingBoxes[chargeBoxId]   = new Tuple<ICentralSystem, DateTime>(CentralSystem, Timestamp.Now);
                    }

                };
            }


            // Wire events...

            #region OnBootNotification

            CentralSystemServer.OnBootNotification += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnBootNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnBootNotificationRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBootNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnBootNotification: " + Request.ChargeBoxId             + ", " +
                                                           Request.ChargePointVendor       + ", " +
                                                           Request.ChargePointModel        + ", " +
                                                           Request.ChargePointSerialNumber + ", " +
                                                           Request.ChargeBoxSerialNumber);


                await AddChargeBoxIfNotExists(new ChargeBox(Request.ChargeBoxId,
                                                            1,
                                                            Request.ChargePointVendor,
                                                            Request.ChargePointModel,
                                                            null,
                                                            Request.ChargePointSerialNumber,
                                                            Request.ChargeBoxSerialNumber,
                                                            Request.FirmwareVersion,
                                                            Request.Iccid,
                                                            Request.IMSI,
                                                            Request.MeterType,
                                                            Request.MeterSerialNumber));


                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }


                await Task.Delay(100);


                var response = new BootNotificationResponse(Request:            Request,
                                                            Status:             RegistrationStatus.Accepted,
                                                            CurrentTime:        Timestamp.Now,
                                                            HeartbeatInterval:  TimeSpan.FromMinutes(5));


                #region Send OnBootNotificationResponse event

                try
                {

                    OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBootNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnHeartbeat

            CentralSystemServer.OnHeartbeat += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnHeartbeatRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnHeartbeatRequest?.Invoke(requestTimestamp,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatRequest));
                }

                #endregion


                Console.WriteLine("OnHeartbeat: " + Request.ChargeBoxId);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }


                await Task.Delay(100);


                var response = new HeartbeatResponse(Request:      Request,
                                                     CurrentTime:  Timestamp.Now);


                #region Send OnHeartbeatResponse event

                try
                {

                    OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnAuthorize

            CentralSystemServer.OnAuthorize += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnAuthorizeRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnAuthorizeRequest?.Invoke(requestTimestamp,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeRequest));
                }

                #endregion


                Console.WriteLine("OnAuthorize: " + Request.ChargeBoxId + ", " +
                                                    Request.IdTag);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100);

                var response = new AuthorizeResponse(Request:    Request,
                                                     IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                               ExpiryDate:  Timestamp.Now.AddDays(3)));


                #region Send OnAuthorizeResponse event

                try
                {

                    OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStartTransaction

            CentralSystemServer.OnStartTransaction += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnStartTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStartTransactionRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStartTransaction: " + Request.ChargeBoxId + ", " +
                                                           Request.ConnectorId + ", " +
                                                           Request.IdTag + ", " +
                                                           Request.StartTimestamp + ", " +
                                                           Request.MeterStart + ", " +
                                                           Request.ReservationId ?? "-");

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100);

                var response = new StartTransactionResponse(Request:        Request,
                                                            TransactionId:  Transaction_Id.NewRandom,
                                                            IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                          ExpiryDate:  Timestamp.Now.AddDays(3)));

                var key = Request.ChargeBoxId + "*" + Request.ConnectorId;

                if (TransactionIds.ContainsKey(key))
                    TransactionIds[key] = response.TransactionId;
                else
                    TransactionIds.Add(key, response.TransactionId);


                #region Send OnStartTransactionResponse event

                try
                {

                    OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStatusNotification

            CentralSystemServer.OnStatusNotification += async (LogTimestamp,
                                                               Sender,
                                                               Request,
                                                               CancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStatusNotificationRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnStatusNotification: " + Request.ConnectorId     + ", " +
                                                             Request.Status          + ", " +
                                                             Request.ErrorCode       + ", " +
                                                             Request.Info            + ", " +
                                                             Request.StatusTimestamp + ", " +
                                                             Request.VendorId        + ", " +
                                                             Request.VendorErrorCode);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new StatusNotificationResponse(Request);


                #region Send OnStatusNotificationResponse event

                try
                {

                    OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         Request,
                                                         response,
                                                         Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnMeterValues

            CentralSystemServer.OnMeterValues += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnMeterValuesRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnMeterValuesRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesRequest));
                }

                                                            #endregion


                Console.WriteLine("OnMeterValues: " + Request.ConnectorId + ", " +
                                                      Request.TransactionId);

                Console.WriteLine(Request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new MeterValuesResponse(Request);


                #region Send OnMeterValuesResponse event

                try
                {

                    OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStopTransaction

            CentralSystemServer.OnStopTransaction += async (LogTimestamp,
                                                            Sender,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnStopTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStopTransactionRequest?.Invoke(requestTimestamp,
                                                     this,
                                                     Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStopTransaction: " + Request.TransactionId + ", " +
                                                          Request.IdTag + ", " +
                                                          Request.Timestamp.ToIso8601() + ", " +
                                                          Request.MeterStop + ", " +
                                                          Request.Reason);

                Console.WriteLine(Request.TransactionData.SafeSelect(transactionData => transactionData.Timestamp.ToIso8601() +
                                          transactionData.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                await Task.Delay(100);

                var response = new StopTransactionResponse(Request:    Request,
                                                           IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                     ExpiryDate:  Timestamp.Now.AddDays(3)));

                var kvp = TransactionIds.Where(trid => trid.Value == Request.TransactionId).ToArray();
                if (kvp.SafeAny())
                    TransactionIds.Remove(kvp.First().Key);


                #region Send OnStopTransactionResponse event

                try
                {

                    OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      Request,
                                                      response,
                                                      Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnIncomingDataTransfer

            CentralSystemServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnIncomingDataRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(requestTimestamp,
                                                          this,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferRequest));
                }

                #endregion


                Console.WriteLine("OnIncomingDataTransfer: " + Request.VendorId  + ", " +
                                                               Request.MessageId + ", " +
                                                               Request.Data);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new DataTransferResponse(Request:  Request,
                                                        Status:   DataTransferStatus.Accepted,
                                                        Data:     Request.Data.Reverse());


                #region Send OnIncomingDataResponse event

                try
                {

                    OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           Request,
                                                           response,
                                                           Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnDiagnosticsStatusNotification

            CentralSystemServer.OnDiagnosticsStatusNotification += async (LogTimestamp,
                                                                          Sender,
                                                                          Request,
                                                                          CancellationToken) => {

                #region Send OnDiagnosticsStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnDiagnosticsStatusNotificationRequest?.Invoke(requestTimestamp,
                                                                   this,
                                                                   Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnDiagnosticsStatusNotification: " + Request.Status);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new DiagnosticsStatusNotificationResponse(Request);


                #region Send OnDiagnosticsStatusResponse event

                try
                {

                    OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    Request,
                                                                    response,
                                                                    Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnFirmwareStatusNotification

            CentralSystemServer.OnFirmwareStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationRequest?.Invoke(requestTimestamp,
                                                                this,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnFirmwareStatus: " + Request.Status);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new FirmwareStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

        }

        #endregion


        public void AddBasicAuth(String Login, String Password)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CentralSystemWSServer centralSystemWSServer)
                {
                    centralSystemWSServer.ChargingBoxLogins.Add(Login, Password);
                }
            }

        }


        #region ChargeBoxes

        #region Data

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        protected internal readonly Dictionary<ChargeBox_Id, ChargeBox> _ChargeBoxes;

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        public IEnumerable<ChargeBox> ChargeBoxes
        {
            get
            {

                if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return _ChargeBoxes.Values.ToArray();

                    }
                    finally
                    {
                        try
                        {
                            ChargeBoxesSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }

                return new ChargeBox[0];

            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(ChargeBox,                      MessageType,    OldChargeBox = null, ...)

        ///// <summary>
        ///// Write the given chargeBox to the database and send out notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async Task WriteToDatabaseFileAndNotify(ChargeBox             ChargeBox,
        //                                                           NotificationMessageType  MessageType,
        //                                                           ChargeBox             OldChargeBox   = null,
        //                                                           EventTracking_Id         EventTrackingId   = null,
        //                                                           User_Id?                 CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    if (MessageType.IsNullOrEmpty)
        //        throw new ArgumentNullException(nameof(MessageType),   "The given message type must not be null or empty!");


        //    var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

        //    await WriteToDatabaseFile(MessageType,
        //                              ChargeBox.ToJSON(false, true),
        //                              eventTrackingId,
        //                              CurrentUserId);

        //    await SendNotifications(ChargeBox,
        //                            MessageType,
        //                            OldChargeBox,
        //                            eventTrackingId,
        //                            CurrentUserId);

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox,                      MessageType(s), OldChargeBox = null, ...)

        //protected virtual String ChargeBoxHTMLInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Name.FirstText(), "</a> ",
        //                                        "(<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>)")
        //                         : String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>"));

        //protected virtual String ChargeBoxTextInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("'", ChargeBox.Name.FirstText(), "' (", ChargeBox.Id, ")")
        //                         : String.Concat("'", ChargeBox.Id.ToString(), "'"));


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox             ChargeBox,
        //                                                  NotificationMessageType  MessageType,
        //                                                  ChargeBox             OldChargeBox   = null,
        //                                                  EventTracking_Id         EventTrackingId   = null,
        //                                                  User_Id?                 CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         new NotificationMessageType[] { MessageType },
        //                         OldChargeBox,
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageTypes">The chargeBox notifications.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        ChargeBox                          OldChargeBox   = null,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

        //    if (messageTypesHash.Contains(addChargeBoxIfNotExists_MessageType))
        //        messageTypesHash.Add(addChargeBox_MessageType);

        //    if (messageTypesHash.Contains(addOrUpdateChargeBox_MessageType))
        //        messageTypesHash.Add(OldChargeBox == null
        //                               ? addChargeBox_MessageType
        //                               : updateChargeBox_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    ComparizionResult? comparizionResult = null;

        //    if (messageTypes.Contains(updateChargeBox_MessageType))
        //        comparizionResult = ChargeBox.CompareWith(OldChargeBox);


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ChargeBox.GetNotificationsOf<TelegramNotification>(messageTypes).
        //                                                     ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.\n" + comparizionResult?.ToTelegram(),
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications  = ChargeBox.GetNotificationsOf<SMSNotification>(messageTypes).
        //                                                    ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' was successfully created. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' information had been successfully updated. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                                          // + {Updated information}
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications  = ChargeBox.GetNotificationsOf<HTTPSNotification>(messageTypes).
        //                                                      ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxCreated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxUpdated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications  = ChargeBox.GetNotificationsOf<EMailNotification>(messageTypes).
        //                                                          ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " was successfully created",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " was successfully created.\r\n",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.<br /><br />",
        //                                                                    comparizionResult?.ToHTML() ?? "",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated.\r\r\r\r",
        //                                                                    comparizionResult?.ToText() ?? "",
        //                                                                    "\r\r\r\r",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox, ParentChargeBoxes, MessageType(s), ...)

        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charge boxes.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox               ChargeBox,
        //                                                  IEnumerable<ChargeBox>  ParentChargeBoxes,
        //                                                  NotificationMessageType    MessageType,
        //                                                  EventTracking_Id           EventTrackingId   = null,
        //                                                  User_Id?                   CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         ParentChargeBoxes,
        //                         new NotificationMessageType[] { MessageType },
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charge boxes.</param>
        ///// <param name="MessageTypes">The user notifications.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<ChargeBox>             ParentChargeBoxes,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),         "The given chargeBox must not be null or empty!");

        //    if (ParentChargeBoxes is null)
        //        ParentChargeBoxes = new ChargeBox[0];

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),         "The given enumeration of message types must not be null or empty!");

        //    //if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
        //    //    messageTypesHash.Add(addUser_MessageType);

        //    //if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
        //    //    messageTypesHash.Add(OldChargeBox == null
        //    //                           ? addUser_MessageType
        //    //                           : updateUser_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ParentChargeBoxes.
        //                                                    SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                    SelectMany(edge   => edge.Source.GetNotificationsOf<TelegramNotification>(deleteChargeBox_MessageType)).
        //                                                    ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications = ParentChargeBoxes.
        //                                          SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                          SelectMany(edge   => edge.Source.GetNotificationsOf<SMSNotification>(deleteChargeBox_MessageType)).
        //                                          ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' has been deleted."),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications = ParentChargeBoxes.
        //                                            SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                            SelectMany(edge   => edge.Source.GetNotificationsOf<HTTPSNotification>(deleteChargeBox_MessageType)).
        //                                            ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxDeleted",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications = ParentChargeBoxes.
        //                                                SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                SelectMany(edge   => edge.Source.GetNotificationsOf<EMailNotification>(deleteChargeBox_MessageType)).
        //                                                ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                             new HTMLEMailBuilder() {

        //                                 From           = Robot.EMail,
        //                                 To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                 Passphrase     = APIPassphrase,
        //                                 Subject        = ChargeBoxTextInfo(ChargeBox) + " has been deleted",

        //                                 HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.<br />",
        //                                                                HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxTextInfo(ChargeBox) + " has been deleted.\r\n",
        //                                                                TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 SecurityLevel  = EMailSecurity.autosign

        //                             });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) GetChargeBoxSerializator (Request, ChargeBox)

        protected internal ChargeBoxToJSONDelegate GetChargeBoxSerializator(HTTPRequest  Request,
                                                                            User         User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (chargeBox,
                            embedded,
                            expandTags,
                            includeCryptoHash)

                            => chargeBox.ToJSON(embedded,
                                                expandTags,
                                                includeCryptoHash);

            }

        }

        #endregion


        #region AddChargeBox           (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was added.</param>
        /// <param name="ChargeBox">The added charge box.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxAddedDelegate(DateTime           Timestamp,
                                                      ChargeBox          ChargeBox,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                                      User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was added.
        /// </summary>
        public event OnChargeBoxAddedDelegate? OnChargeBoxAdded;


        #region (protected internal) _AddChargeBox(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxResult> _AddChargeBox(ChargeBox                             ChargeBox,
                                                                        Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                                                        EventTracking_Id?                     EventTrackingId   = null,
                                                                        User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return AddChargeBoxResult.ArgumentError(null,
                                                        eventTrackingId,
                                                        nameof(ChargeBox),
                                                        "The given chargeBox must not be null!");

            if (ChargeBox.API is not null && ChargeBox.API != this)
                return AddChargeBoxResult.ArgumentError(ChargeBox,
                                                        eventTrackingId,
                                                        nameof(ChargeBox),
                                                        "The given chargeBox is already attached to another API!");

            if (_ChargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.ArgumentError(ChargeBox,
                                                        eventTrackingId,
                                                        nameof(ChargeBox),
                                                        "ChargeBox identification '" + ChargeBox.Id + "' already exists!");

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                       nameof(ChargeBox),
            //                                       "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            _ChargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal is not null)
                await OnChargeBoxAddedLocal?.Invoke(Timestamp.Now,
                                                    ChargeBox,
                                                    eventTrackingId,
                                                    CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBox_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxResult.Success(ChargeBox,
                                              eventTrackingId);

        }

        #endregion

        #region AddChargeBox                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charge box.
        /// </summary>
        /// <param name="ChargeBox">A new charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxResult> AddChargeBox(ChargeBox                             ChargeBox,
                                                           Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                                           EventTracking_Id?                     EventTrackingId   = null,
                                                           User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBox(ChargeBox,
                                               OnAdded,
                                               eventTrackingId,
                                               CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return AddChargeBoxResult.Failed(ChargeBox,
                                                     eventTrackingId,
                                                     e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxResult.Failed(ChargeBox,
                                             eventTrackingId,
                                             "Internal locking failed!");

        }

        #endregion

        #endregion

        #region AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        #region (protected internal) _AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxIfNotExistsResult> _AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                                                                              Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                                                                              EventTracking_Id?                     EventTrackingId   = null,
                                                                                              User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return AddChargeBoxIfNotExistsResult.ArgumentError(ChargeBox,
                                                                   eventTrackingId,
                                                                   nameof(ChargeBox),
                                                                   "The given chargeBox must not be null!");

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddChargeBoxIfNotExistsResult.ArgumentError(ChargeBox,
                                                                   eventTrackingId,
                                                                   nameof(ChargeBox),
                                                                   "The given chargeBox is already attached to another API!");

            if (_ChargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxIfNotExistsResult.Success(_ChargeBoxes[ChargeBox.Id],
                                                             AddedOrIgnored.Ignored,
                                                             eventTrackingId);

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddChargeBoxIfNotExistsResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxIfNotExistsResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxIfNotExistsResult.ArgumentError(ChargeBox,
            //                                                  nameof(ChargeBox),
            //                                                  "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBoxIfNotExists_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            _ChargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal != null)
                await OnChargeBoxAddedLocal?.Invoke(Timestamp.Now,
                                                    ChargeBox,
                                                    eventTrackingId,
                                                    CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBoxIfNotExists_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxIfNotExistsResult.Success(ChargeBox,
                                                         AddedOrIgnored.Added,
                                                         eventTrackingId);

        }

        #endregion

        #region AddChargeBoxIfNotExists                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charge box.
        /// </summary>
        /// <param name="ChargeBox">A new charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxIfNotExistsResult> AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                                                                 Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                                                                 EventTracking_Id?                     EventTrackingId   = null,
                                                                                 User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBoxIfNotExists(ChargeBox,
                                                          OnAdded,
                                                          eventTrackingId,
                                                          CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return AddChargeBoxIfNotExistsResult.Failed(ChargeBox,
                                                                eventTrackingId,
                                                                e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxIfNotExistsResult.Failed(ChargeBox,
                                                        eventTrackingId,
                                                        "Internal locking failed!");

        }

        #endregion

        #endregion

        #region AddOrUpdateChargeBox   (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateChargeBox(ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddOrUpdateChargeBoxResult> _AddOrUpdateChargeBox(ChargeBox                            ChargeBox,
                                                                                        Action<ChargeBox, EventTracking_Id>  OnAdded           = null,
                                                                                        Action<ChargeBox, EventTracking_Id>  OnUpdated         = null,
                                                                                        EventTracking_Id                     EventTrackingId   = null,
                                                                                        User_Id?                             CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                                   eventTrackingId,
                                                                   nameof(ChargeBox),
                                                                   "The given chargeBox must not be null!");

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                                   eventTrackingId,
                                                                   nameof(ChargeBox.API),
                                                                   "The given chargeBox is already attached to another API!");

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addOrUpdateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            if (_ChargeBoxes.TryGetValue(ChargeBox.Id, out ChargeBox OldChargeBox))
            {
                _ChargeBoxes.Remove(OldChargeBox.Id);
                ChargeBox.CopyAllLinkedDataFrom(OldChargeBox);
            }

            _ChargeBoxes.Add(ChargeBox.Id, ChargeBox);

            if (OldChargeBox is null)
            {

                OnAdded?.Invoke(ChargeBox,
                                eventTrackingId);

                var OnChargeBoxAddedLocal = OnChargeBoxAdded;
                if (OnChargeBoxAddedLocal != null)
                    await OnChargeBoxAddedLocal?.Invoke(Timestamp.Now,
                                                           ChargeBox,
                                                           eventTrackingId,
                                                           CurrentUserId);

                //await SendNotifications(ChargeBox,
                //                        addChargeBox_MessageType,
                //                        null,
                //                        eventTrackingId,
                //                        CurrentUserId);

                return AddOrUpdateChargeBoxResult.Success(ChargeBox,
                                                             AddedOrUpdated.Add,
                                                             eventTrackingId);

            }
            else
            {

                OnAdded?.Invoke(ChargeBox,
                                eventTrackingId);

                var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
                if (OnChargeBoxUpdatedLocal != null)
                    await OnChargeBoxUpdatedLocal?.Invoke(Timestamp.Now,
                                                             ChargeBox,
                                                             OldChargeBox,
                                                             eventTrackingId,
                                                             CurrentUserId);

                //await SendNotifications(ChargeBox,
                //                        updateChargeBox_MessageType,
                //                        OldChargeBox,
                //                        eventTrackingId,
                //                        CurrentUserId);

                return AddOrUpdateChargeBoxResult.Success(ChargeBox,
                                                             AddedOrUpdated.Update,
                                                             eventTrackingId);

            }

        }

        #endregion

        #region AddOrUpdateChargeBox                      (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddOrUpdateChargeBoxResult> AddOrUpdateChargeBox(ChargeBox                            ChargeBox,
                                                                           Action<ChargeBox, EventTracking_Id>  OnAdded           = null,
                                                                           Action<ChargeBox, EventTracking_Id>  OnUpdated         = null,
                                                                           EventTracking_Id                     EventTrackingId   = null,
                                                                           User_Id?                             CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddOrUpdateChargeBox(ChargeBox,
                                                       OnAdded,
                                                       OnUpdated,
                                                       eventTrackingId,
                                                       CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return AddOrUpdateChargeBoxResult.Failed(ChargeBox,
                                                             eventTrackingId,
                                                             e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return AddOrUpdateChargeBoxResult.Failed(ChargeBox,
                                                     eventTrackingId,
                                                     "Internal locking failed!");

        }

        #endregion

        #endregion

        #region UpdateChargeBox        (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was updated.</param>
        /// <param name="ChargeBox">The updated charge box.</param>
        /// <param name="OldChargeBox">The old charge box.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxUpdatedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        ChargeBox          OldChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was updated.
        /// </summary>
        public event OnChargeBoxUpdatedDelegate OnChargeBoxUpdated;


        #region (protected internal) _UpdateChargeBox(ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult> _UpdateChargeBox(ChargeBox                             ChargeBox,
                                                                              Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                                                              EventTracking_Id?                     EventTrackingId   = null,
                                                                              User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox must not be null!");

            if (!_TryGetChargeBox(ChargeBox.Id, out ChargeBox OldChargeBox))
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox '" + ChargeBox.Id + "' does not exists in this API!");

            if (ChargeBox.API != null && ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox.API),
                                                           "The given chargeBox is already attached to another API!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            _ChargeBoxes.Remove(OldChargeBox.Id);
            ChargeBox.CopyAllLinkedDataFrom(OldChargeBox);
            _ChargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal?.Invoke(Timestamp.Now,
                                                         ChargeBox,
                                                         OldChargeBox,
                                                         eventTrackingId,
                                                         CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        updateChargeBox_MessageType,
            //                        OldChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(ChargeBox,
                                                 eventTrackingId);

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult> UpdateChargeBox(ChargeBox                             ChargeBox,
                                                                 Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                                                 EventTracking_Id?                     EventTrackingId   = null,
                                                                 User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return UpdateChargeBoxResult.Failed(ChargeBox,
                                                        eventTrackingId,
                                                        e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.Failed(ChargeBox,
                                                eventTrackingId,
                                                "Internal locking failed!");

        }

        #endregion


        #region (protected internal) _UpdateChargeBox(ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charge box.
        /// </summary>
        /// <param name="ChargeBox">An charge box.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult> _UpdateChargeBox(ChargeBox                             ChargeBox,
                                                                              Action<ChargeBox.Builder>             UpdateDelegate,
                                                                              Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                                                              EventTracking_Id?                     EventTrackingId   = null,
                                                                              User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox must not be null!");

            if (!_ChargeBoxExists(ChargeBox.Id))
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox '" + ChargeBox.Id + "' does not exists in this API!");

            if (ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox.API),
                                                           "The given chargeBox is not attached to this API!");

            if (UpdateDelegate is null)
                return UpdateChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(UpdateDelegate),
                                                           "The given update delegate must not be null!");


            var builder = ChargeBox.ToBuilder();
            UpdateDelegate(builder);
            var updatedChargeBox = builder.ToImmutable;

            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          updatedChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            _ChargeBoxes.Remove(ChargeBox.Id);
            updatedChargeBox.CopyAllLinkedDataFrom(ChargeBox);
            _ChargeBoxes.Add(updatedChargeBox.Id, updatedChargeBox);

            OnUpdated?.Invoke(updatedChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal?.Invoke(Timestamp.Now,
                                                      updatedChargeBox,
                                                      ChargeBox,
                                                      eventTrackingId,
                                                      CurrentUserId);

            //await SendNotifications(updatedChargeBox,
            //                        updateChargeBox_MessageType,
            //                        ChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(ChargeBox,
                                                 eventTrackingId);

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charge box.
        /// </summary>
        /// <param name="ChargeBox">An charge box.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult> UpdateChargeBox(ChargeBox                             ChargeBox,
                                                                 Action<ChargeBox.Builder>             UpdateDelegate,
                                                                 Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                                                 EventTracking_Id?                     EventTrackingId   = null,
                                                                 User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  UpdateDelegate,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return UpdateChargeBoxResult.Failed(ChargeBox,
                                                        eventTrackingId,
                                                        e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.Failed(ChargeBox,
                                                eventTrackingId,
                                                "Internal locking failed!");

        }

        #endregion

        #endregion


        #region ChargeBoxExists(ChargeBoxId)

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal Boolean _ChargeBoxExists(ChargeBox_Id ChargeBoxId)

            => ChargeBoxId.IsNotNullOrEmpty && _ChargeBoxes.ContainsKey(ChargeBoxId);

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal Boolean _ChargeBoxExists(ChargeBox_Id? ChargeBoxId)

            => ChargeBoxId.IsNotNullOrEmpty() && _ChargeBoxes.ContainsKey(ChargeBoxId.Value);


        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        public Boolean ChargeBoxExists(ChargeBox_Id ChargeBoxId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        public Boolean ChargeBoxExists(ChargeBox_Id? ChargeBoxId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        #endregion

        #region GetChargeBox   (ChargeBoxId)

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id ChargeBoxId)
        {

            if (ChargeBoxId.IsNotNullOrEmpty && _ChargeBoxes.TryGetValue(ChargeBoxId, out ChargeBox? chargeBox))
                return chargeBox;

            return default;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id? ChargeBoxId)
        {

            if (ChargeBoxId is not null && _ChargeBoxes.TryGetValue(ChargeBoxId.Value, out ChargeBox? chargeBox))
                return chargeBox;

            return default;

        }


        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        public ChargeBox? GetChargeBox(ChargeBox_Id ChargeBoxId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        public ChargeBox? GetChargeBox(ChargeBox_Id? ChargeBoxId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        #endregion

        #region TryGetChargeBox(ChargeBoxId, out ChargeBox)

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        protected internal Boolean _TryGetChargeBox(ChargeBox_Id    ChargeBoxId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxId.IsNotNullOrEmpty && _ChargeBoxes.TryGetValue(ChargeBoxId, out ChargeBox? chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        protected internal Boolean _TryGetChargeBox(ChargeBox_Id?   ChargeBoxId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxId is not null && _ChargeBoxes.TryGetValue(ChargeBoxId.Value, out ChargeBox? chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }


        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        public Boolean TryGetChargeBox(ChargeBox_Id    ChargeBoxId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(ChargeBoxId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        public Boolean TryGetChargeBox(ChargeBox_Id?   ChargeBoxId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(ChargeBoxId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        #endregion


        #region DeleteChargeBox(ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was deleted.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was deleted.</param>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnChargeBoxDeletedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was deleted.
        /// </summary>
        public event OnChargeBoxDeletedDelegate OnChargeBoxDeleted;


        #region (protected internal virtual) _CanDeleteChargeBox(ChargeBox)

        /// <summary>
        /// Determines whether the chargeBox can safely be deleted from the API.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        protected internal virtual I18NString? _CanDeleteChargeBox(ChargeBox ChargeBox)
        {

            //if (ChargeBox.Users.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has members!");

            //if (ChargeBox.SubChargeBoxes.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has sub chargeBoxs!");

            return null;

        }

        #endregion

        #region (protected internal) _DeleteChargeBox(ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charge box.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<DeleteChargeBoxResult> _DeleteChargeBox(ChargeBox                             ChargeBox,
                                                                              Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                                                                              EventTracking_Id?                     EventTrackingId   = null,
                                                                              User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox is null)
                return DeleteChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox must not be null!");

            if (ChargeBox.API != this)
                return DeleteChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox is not attached to this API!");

            if (!_ChargeBoxes.TryGetValue(ChargeBox.Id, out ChargeBox? ChargeBoxToBeDeleted))
                return DeleteChargeBoxResult.ArgumentError(ChargeBox,
                                                           eventTrackingId,
                                                           nameof(ChargeBox),
                                                           "The given chargeBox does not exists in this API!");


            var result = _CanDeleteChargeBox(ChargeBox);

            if (result is not null)
                return DeleteChargeBoxResult.Failed(ChargeBox,
                                                    eventTrackingId,
                                                    result);


            //// Get all parent chargeBoxs now, because later
            //// the --isChildOf--> edge will no longer be available!
            //var parentChargeBoxes = ChargeBox.GetAllParents(parent => parent != NoOwner).
            //                                       ToArray();


            //// Remove all: this --edge--> other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxOutEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Source,
            //                               edge.EdgeLabel,
            //                               edge.Target,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);

            //// Remove all: this <--edge-- other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxInEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Target,
            //                               edge.EdgeLabel,
            //                               edge.Source,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);


            //await WriteToDatabaseFile(deleteChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            _ChargeBoxes.Remove(ChargeBox.Id);

            OnDeleted?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxDeletedLocal = OnChargeBoxDeleted;
            if (OnChargeBoxDeletedLocal is not null)
                await OnChargeBoxDeletedLocal?.Invoke(Timestamp.Now,
                                                      ChargeBox,
                                                      eventTrackingId,
                                                      CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        parentChargeBoxes,
            //                        deleteChargeBox_MessageType,
            //                        eventTrackingId,
            //                        CurrentUserId);


            return DeleteChargeBoxResult.Success(ChargeBox,
                                                 eventTrackingId);

        }

        #endregion

        #region DeleteChargeBox                      (ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charge box.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<DeleteChargeBoxResult> DeleteChargeBox(ChargeBox                             ChargeBox,
                                                                 Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                                                                 EventTracking_Id?                     EventTrackingId   = null,
                                                                 User_Id?                              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _DeleteChargeBox(ChargeBox,
                                                  OnDeleted,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    DebugX.LogException(e);

                    return DeleteChargeBoxResult.Failed(ChargeBox,
                                                        eventTrackingId,
                                                        e);

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return DeleteChargeBoxResult.Failed(ChargeBox,
                                                eventTrackingId,
                                                "Internal locking failed!");

        }

        #endregion

        #endregion

        #endregion



        #region Reset                 (ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        public async Task<CP.ResetResponse> Reset(ChargeBox_Id       ChargeBoxId,
                                                  ResetTypes         ResetType,
                                                  EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new ResetRequest(ChargeBoxId,
                                           ResetType,
                                           Request_Id.NewRandom(),
                                           null,
                                           EventTrackingId);

            #region Send OnResetRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(requestTimestamp,
                                       this,
                                       request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetRequest));
            }

            #endregion


            CP.ResetResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.Reset(request);

            else
                response = new CP.ResetResponse(request, ResetStatus.Rejected);


            if (response is null)
                response = new CP.ResetResponse(request,
                                                Result.Server("Response is null!"));


            #region Send OnResetResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnResetResponse?.Invoke(responseTimestamp,
                                        this,
                                        request,
                                        response,
                                        responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability    (ChargeBoxId, ConnectorId, Availability, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        public async Task<CP.ChangeAvailabilityResponse> ChangeAvailability(ChargeBox_Id       ChargeBoxId,
                                                                            Connector_Id       ConnectorId,
                                                                            Availabilities     Availability,
                                                                            EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new ChangeAvailabilityRequest(ChargeBoxId,
                                                        ConnectorId,
                                                        Availability,
                                                        Request_Id.NewRandom(),
                                                        null,
                                                        EventTrackingId);

            #region Send OnChangeAvailabilityRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            CP.ChangeAvailabilityResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.ChangeAvailability(request);

            else
                response = new CP.ChangeAvailabilityResponse(request, AvailabilityStatus.Rejected);


            if (response is null)
                response = new CP.ChangeAvailabilityResponse(request,
                                                             Result.Server("Response is null!"));


            #region Send OnChangeAvailabilityResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnChangeAvailabilityResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     request,
                                                     response,
                                                     responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConfiguration      (ChargeBoxId, Keys = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        public async Task<CP.GetConfigurationResponse> GetConfiguration(ChargeBox_Id          ChargeBoxId,
                                                                        IEnumerable<String>?  Keys              = null,
                                                                        EventTracking_Id?     EventTrackingId   = null)
        {

            var request = new GetConfigurationRequest(ChargeBoxId,
                                                      Keys,
                                                      Request_Id.NewRandom(),
                                                      null,
                                                      EventTrackingId);

            #region Send OnGetConfigurationRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnGetConfigurationRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            CP.GetConfigurationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.GetConfiguration(request);

            else
                response = new CP.GetConfigurationResponse(request,
                                                           new ConfigurationKey[0],
                                                           Keys);


            if (response is null)
                response = new CP.GetConfigurationResponse(request,
                                                           Result.Server("Response is null!"));


            #region Send OnGetConfigurationResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnGetConfigurationResponse?.Invoke(responseTimestamp,
                                                   this,
                                                   request,
                                                   response,
                                                   responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeConfiguration   (ChargeBoxId, Key, Value, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        public async Task<CP.ChangeConfigurationResponse> ChangeConfiguration(ChargeBox_Id       ChargeBoxId,
                                                                              String             Key,
                                                                              String             Value,
                                                                              EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new ChangeConfigurationRequest(ChargeBoxId,
                                                         Key,
                                                         Value,
                                                         Request_Id.NewRandom(),
                                                         null,
                                                         EventTrackingId);

            #region Send OnChangeConfigurationRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnChangeConfigurationRequest?.Invoke(requestTimestamp,
                                                     this,
                                                     request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            CP.ChangeConfigurationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.ChangeConfiguration(request);

            else
                response = new CP.ChangeConfigurationResponse(request, ConfigurationStatus.Rejected);


            if (response is null)
                response = new CP.ChangeConfigurationResponse(request,
                                                              Result.Server("Response is null!"));


            #region Send OnChangeConfigurationResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnChangeConfigurationResponse?.Invoke(responseTimestamp,
                                                      this,
                                                      request,
                                                      response,
                                                      responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DataTransfer          (ChargeBoxId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        public async Task<CP.DataTransferResponse> DataTransfer(ChargeBox_Id       ChargeBoxId,
                                                                String             VendorId,
                                                                String?            MessageId         = null,
                                                                String?            Data              = null,
                                                                EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new DataTransferRequest(ChargeBoxId,
                                                  VendorId,
                                                  MessageId,
                                                  Data,
                                                  Request_Id.NewRandom(),
                                                  null,
                                                  EventTrackingId);

            #region Send OnDataTransferRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(requestTimestamp,
                                              this,
                                              request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CP.DataTransferResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.DataTransfer(request);

            else
                response = new CP.DataTransferResponse(request, DataTransferStatus.Rejected);


            if (response is null)
                response = new CP.DataTransferResponse(request,
                                                       Result.Server("Response is null!"));


            #region Send OnDataTransferResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnDataTransferResponse?.Invoke(responseTimestamp,
                                               this,
                                               request,
                                               response,
                                               responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDiagnostics        (ChargeBoxId, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null,...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        public async Task<CP.GetDiagnosticsResponse> GetDiagnostics(ChargeBox_Id       ChargeBoxId,
                                                                    String             Location,
                                                                    DateTime?          StartTime         = null,
                                                                    DateTime?          StopTime          = null,
                                                                    Byte?              Retries           = null,
                                                                    TimeSpan?          RetryInterval     = null,
                                                                    EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new GetDiagnosticsRequest(ChargeBoxId,
                                                    Location,
                                                    StartTime,
                                                    StopTime,
                                                    Retries,
                                                    RetryInterval,
                                                    Request_Id.NewRandom(),
                                                    null,
                                                    EventTrackingId);

            #region Send OnGetDiagnosticsRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnGetDiagnosticsRequest?.Invoke(requestTimestamp,
                                                this,
                                                request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            CP.GetDiagnosticsResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.GetDiagnostics(request);

            else
                response = new CP.GetDiagnosticsResponse(request);


            if (response is null)
                response = new CP.GetDiagnosticsResponse(request,
                                                         Result.Server("Response is null!"));


            #region Send OnGetDiagnosticsResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnGetDiagnosticsResponse?.Invoke(responseTimestamp,
                                                 this,
                                                 request,
                                                 response,
                                                 responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage        (ChargeBoxId, RequestedMessage, ConnectorId = null,...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        public async Task<CP.TriggerMessageResponse> TriggerMessage(ChargeBox_Id       ChargeBoxId,
                                                                    MessageTriggers    RequestedMessage,
                                                                    Connector_Id?      ConnectorId       = null,
                                                                    EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new TriggerMessageRequest(ChargeBoxId,
                                                    RequestedMessage,
                                                    ConnectorId,
                                                    Request_Id.NewRandom(),
                                                    null,
                                                    EventTrackingId);

            #region Send OnTriggerMessageRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnTriggerMessageRequest?.Invoke(requestTimestamp,
                                                this,
                                                request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            CP.TriggerMessageResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.TriggerMessage(request);

            else
                response = new CP.TriggerMessageResponse(request, TriggerMessageStatus.Rejected);


            if (response is null)
                response = new CP.TriggerMessageResponse(request,
                                                         Result.Server("Response is null!"));


            #region Send OnTriggerMessageResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnTriggerMessageResponse?.Invoke(responseTimestamp,
                                                 this,
                                                 request,
                                                 response,
                                                 responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware        (ChargeBoxId, Location, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where to download the firmware.</param>
        /// <param name="RetrieveDate">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        public async Task<CP.UpdateFirmwareResponse> UpdateFirmware(ChargeBox_Id       ChargeBoxId,
                                                                    String             Location,
                                                                    DateTime           RetrieveDate,
                                                                    Byte?              Retries           = null,
                                                                    TimeSpan?          RetryInterval     = null,
                                                                    EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new UpdateFirmwareRequest(ChargeBoxId,
                                                    Location,
                                                    RetrieveDate,
                                                    Retries,
                                                    RetryInterval,
                                                    Request_Id.NewRandom(),
                                                    null,
                                                    EventTrackingId);

            #region Send OnUpdateFirmwareRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(requestTimestamp,
                                                this,
                                                request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            CP.UpdateFirmwareResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.UpdateFirmware(request);

            else
                response = new CP.UpdateFirmwareResponse(request);


            if (response is null)
                response = new CP.UpdateFirmwareResponse(request,
                                                         Result.Server("Response is null!"));


            #region Send OnUpdateFirmwareResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnUpdateFirmwareResponse?.Invoke(responseTimestamp,
                                                 this,
                                                 request,
                                                 response,
                                                 responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow            (ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        public async Task<CP.ReserveNowResponse> ReserveNow(ChargeBox_Id       ChargeBoxId,
                                                            Connector_Id       ConnectorId,
                                                            Reservation_Id     ReservationId,
                                                            DateTime           ExpiryDate,
                                                            IdToken            IdTag,
                                                            IdToken?           ParentIdTag       = null,
                                                            EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new ReserveNowRequest(ChargeBoxId,
                                                ConnectorId,
                                                ReservationId,
                                                ExpiryDate,
                                                IdTag,
                                                ParentIdTag,
                                                Request_Id.NewRandom(),
                                                null,
                                                EventTrackingId);

            #region Send OnReserveNowRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnReserveNowRequest?.Invoke(requestTimestamp,
                                            this,
                                            request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            CP.ReserveNowResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.ReserveNow(request);

            else
                response = new CP.ReserveNowResponse(request, ReservationStatus.Rejected);


            if (response is null)
                response = new CP.ReserveNowResponse(request,
                                                     Result.Server("Response is null!"));


            #region Send OnReserveNowResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnReserveNowResponse?.Invoke(responseTimestamp,
                                             this,
                                             request,
                                             response,
                                             responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation     (ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        public async Task<CP.CancelReservationResponse> CancelReservation(ChargeBox_Id       ChargeBoxId,
                                                                          Reservation_Id     ReservationId,
                                                                          EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new CancelReservationRequest(ChargeBoxId,
                                                       ReservationId,
                                                       Request_Id.NewRandom(),
                                                       null,
                                                       EventTrackingId);

            #region Send OnCancelReservationRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnCancelReservationRequest?.Invoke(requestTimestamp,
                                                   this,
                                                   request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            CP.CancelReservationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.CancelReservation(request);

            else
                response = new CP.CancelReservationResponse(request, CancelReservationStatus.Rejected);


            if (response is null)
                response = new CP.CancelReservationResponse(request,
                                                            Result.Server("Response is null!"));


            #region Send OnCancelReservationResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnCancelReservationResponse?.Invoke(responseTimestamp,
                                                    this,
                                                    request,
                                                    response,
                                                    responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStartTransaction(ChargeBoxId, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        public async Task<CP.RemoteStartTransactionResponse> RemoteStartTransaction(ChargeBox_Id       ChargeBoxId,
                                                                                    IdToken            IdTag,
                                                                                    Connector_Id?      ConnectorId       = null,
                                                                                    ChargingProfile?   ChargingProfile   = null,
                                                                                    EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new RemoteStartTransactionRequest(ChargeBoxId,
                                                            IdTag,
                                                            ConnectorId,
                                                            ChargingProfile,
                                                            Request_Id.NewRandom(),
                                                            null,
                                                            EventTrackingId);

            #region Send OnRemoteStartTransactionRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(requestTimestamp,
                                                        this,
                                                        request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            CP.RemoteStartTransactionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.RemoteStartTransaction(request);

            else
                response = new CP.RemoteStartTransactionResponse(request, RemoteStartStopStatus.Rejected);


            if (response is null)
                response = new CP.RemoteStartTransactionResponse(request,
                                                                 Result.Server("Response is null!"));


            #region Send OnRemoteStartTransactionResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnRemoteStartTransactionResponse?.Invoke(responseTimestamp,
                                                         this,
                                                         request,
                                                         response,
                                                         responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStopTransaction (ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        public async Task<CP.RemoteStopTransactionResponse> RemoteStopTransaction(ChargeBox_Id       ChargeBoxId,
                                                                                  Transaction_Id     TransactionId,
                                                                                  EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new RemoteStopTransactionRequest(ChargeBoxId,
                                                           TransactionId,
                                                           Request_Id.NewRandom(),
                                                           null,
                                                           EventTrackingId);

            #region Send OnRemoteStopTransactionRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionRequest?.Invoke(requestTimestamp,
                                                       this,
                                                       request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            CP.RemoteStopTransactionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.RemoteStopTransaction(request);

            else
                response = new CP.RemoteStopTransactionResponse(request, RemoteStartStopStatus.Rejected);


            if (response is null)
                response = new CP.RemoteStopTransactionResponse(request,
                                                                Result.Server("Response is null!"));


            #region Send OnRemoteStopTransactionResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnRemoteStopTransactionResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        request,
                                                        response,
                                                        responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile    (ChargeBoxId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        public async Task<CP.SetChargingProfileResponse> SetChargingProfile(ChargeBox_Id       ChargeBoxId,
                                                                            Connector_Id       ConnectorId,
                                                                            ChargingProfile    ChargingProfile,
                                                                            EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new SetChargingProfileRequest(ChargeBoxId,
                                                        ConnectorId,
                                                        ChargingProfile,
                                                        Request_Id.NewRandom(),
                                                        null,
                                                        EventTrackingId);

            #region Send OnSetChargingProfileRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            CP.SetChargingProfileResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.SetChargingProfile(request);

            else
                response = new CP.SetChargingProfileResponse(request, ChargingProfileStatus.NotSupported);


            if (response is null)
                response = new CP.SetChargingProfileResponse(request,
                                                             Result.Server("Response is null!"));


            #region Send OnSetChargingProfileResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnSetChargingProfileResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     request,
                                                     response,
                                                     responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile  (ChargeBoxId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        public async Task<CP.ClearChargingProfileResponse> ClearChargingProfile(ChargeBox_Id              ChargeBoxId,
                                                                                ChargingProfile_Id?       ChargingProfileId        = null,
                                                                                Connector_Id?             ConnectorId              = null,
                                                                                ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                                                                UInt32?                   StackLevel               = null,
                                                                                EventTracking_Id?         EventTrackingId          = null)
        {

            var request = new ClearChargingProfileRequest(ChargeBoxId,
                                                          ChargingProfileId,
                                                          ConnectorId,
                                                          ChargingProfilePurpose,
                                                          StackLevel,
                                                          Request_Id.NewRandom(),
                                                          null,
                                                          EventTrackingId);

            #region Send OnClearChargingProfileRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            CP.ClearChargingProfileResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.ClearChargingProfile(request);

            else
                response = new CP.ClearChargingProfileResponse(request, ClearChargingProfileStatus.Unknown);


            if (response is null)
                response = new CP.ClearChargingProfileResponse(request,
                                                               Result.Server("Response is null!"));


            #region Send OnClearChargingProfileResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnClearChargingProfileResponse?.Invoke(responseTimestamp,
                                                       this,
                                                       request,
                                                       response,
                                                       responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule  (ChargeBoxId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        public async Task<CP.GetCompositeScheduleResponse> GetCompositeSchedule(ChargeBox_Id        ChargeBoxId,
                                                                                Connector_Id        ConnectorId,
                                                                                TimeSpan            Duration,
                                                                                ChargingRateUnits?  ChargingRateUnit   = null,
                                                                                EventTracking_Id?   EventTrackingId    = null)
        {

            var request = new GetCompositeScheduleRequest(ChargeBoxId,
                                                          ConnectorId,
                                                          Duration,
                                                          ChargingRateUnit,
                                                          Request_Id.NewRandom(),
                                                          null,
                                                          EventTrackingId);

            #region Send OnGetCompositeScheduleRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            CP.GetCompositeScheduleResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.GetCompositeSchedule(request);

            else
                response = new CP.GetCompositeScheduleResponse(request,
                                                               GetCompositeScheduleStatus.Rejected,
                                                               ConnectorId);


            if (response is null)
                response = new CP.GetCompositeScheduleResponse(request,
                                                               Result.Server("Response is null!"));


            #region Send OnGetCompositeScheduleResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnGetCompositeScheduleResponse?.Invoke(responseTimestamp,
                                                       this,
                                                       request,
                                                       response,
                                                       responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector       (ChargeBoxId, ConnectorId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        public async Task<CP.UnlockConnectorResponse> UnlockConnector(ChargeBox_Id       ChargeBoxId,
                                                                      Connector_Id       ConnectorId,
                                                                      EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new UnlockConnectorRequest(ChargeBoxId,
                                                     ConnectorId,
                                                     Request_Id.NewRandom(),
                                                     null,
                                                     EventTrackingId);

            #region Send OnUnlockConnectorRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequest?.Invoke(requestTimestamp,
                                                 this,
                                                 request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            CP.UnlockConnectorResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.UnlockConnector(request);

            else
                response = new CP.UnlockConnectorResponse(request, UnlockStatus.NotSupported);


            if (response is null)
                response = new CP.UnlockConnectorResponse(request,
                                                          Result.Server("Response is null!"));


            #region Send OnUnlockConnectorResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnUnlockConnectorResponse?.Invoke(responseTimestamp,
                                                  this,
                                                  request,
                                                  response,
                                                  responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion   (ChargeBoxId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public async Task<CP.GetLocalListVersionResponse> GetLocalListVersion(ChargeBox_Id       ChargeBoxId,
                                                                              EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new GetLocalListVersionRequest(ChargeBoxId,
                                                         Request_Id.NewRandom(),
                                                         null,
                                                         EventTrackingId);

            #region Send OnGetLocalListVersionRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequest?.Invoke(requestTimestamp,
                                                     this,
                                                     request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            CP.GetLocalListVersionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.GetLocalListVersion(request);

            else
                response = new CP.GetLocalListVersionResponse(request, 0);


            if (response is null)
                response = new CP.GetLocalListVersionResponse(request,
                                                              Result.Server("Response is null!"));


            #region Send OnGetLocalListVersionResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnGetLocalListVersionResponse?.Invoke(responseTimestamp,
                                                      this,
                                                      request,
                                                      response,
                                                      responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList         (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        public async Task<CP.SendLocalListResponse> SendLocalList(ChargeBox_Id                     ChargeBoxId,
                                                                  UInt64                           ListVersion,
                                                                  UpdateTypes                      UpdateType,
                                                                  IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,
                                                                  EventTracking_Id?                EventTrackingId          = null)
        {

            var request = new SendLocalListRequest(ChargeBoxId,
                                                   ListVersion,
                                                   UpdateType,
                                                   LocalAuthorizationList,
                                                   Request_Id.NewRandom(),
                                                   null,
                                                   EventTrackingId);

            #region Send OnSendLocalListRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(requestTimestamp,
                                               this,
                                               request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            CP.SendLocalListResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.SendLocalList(request);

            else
                response = new CP.SendLocalListResponse(request, UpdateStatus.NotSupported);


            if (response is null)
                response = new CP.SendLocalListResponse(request,
                                                        Result.Server("Response is null!"));


            #region Send OnSendLocalListResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnSendLocalListResponse?.Invoke(responseTimestamp,
                                                this,
                                                request,
                                                response,
                                                responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache            (ChargeBoxId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public async Task<CP.ClearCacheResponse> ClearCache(ChargeBox_Id       ChargeBoxId,
                                                            EventTracking_Id?  EventTrackingId   = null)
        {

            var request = new ClearCacheRequest(ChargeBoxId,
                                                Request_Id.NewRandom(),
                                                null,
                                                EventTrackingId);

            #region Send OnClearCacheRequest event

            var requestTimestamp = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(requestTimestamp,
                                            this,
                                            request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            CP.ClearCacheResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime>? webSocketServer))
                response = await webSocketServer.Item1.ClearCache(request);

            else
                response = new CP.ClearCacheResponse(request, ClearCacheStatus.Rejected);


            if (response is null)
                response = new CP.ClearCacheResponse(request,
                                                     Result.Server("Response is null!"));


            #region Send OnClearCacheResponse event

            try
            {

                var responseTimestamp = Timestamp.Now;

                OnClearCacheResponse?.Invoke(responseTimestamp,
                                             this,
                                             request,
                                             response,
                                             responseTimestamp - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion

    }

}
