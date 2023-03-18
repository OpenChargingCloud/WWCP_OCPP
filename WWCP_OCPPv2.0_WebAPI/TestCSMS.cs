/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using social.OpenData.UsersAPI;

using cloud.charging.open.protocols.OCPPv2_0.CSMS;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCSMS : IEventSender
    {

        #region Data

        private          readonly  HashSet<ICSMSServer>                               centralSystemServers;

        private          readonly  Dictionary<ChargeBox_Id, Tuple<ICSMS, DateTime>>   reachableChargingBoxes;

        private          readonly  UsersAPI                                                    TestAPI;

        private          readonly  OCPPWebAPI                                                  WebAPI;

        protected static readonly  SemaphoreSlim                                               ChargeBoxesSemaphore    = new (1, 1);

        protected static readonly  TimeSpan                                                    SemaphoreSlimTimeout    = TimeSpan.FromSeconds(5);

        public    static readonly  IPPort                                                      DefaultHTTPUploadPort   = IPPort.Parse(9901);

        private                    Int64                                                       internalRequestId       = 900000;

        private                    TimeSpan                                                    defaultRequestTimeout   = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public CSMS_Id  CSMSId    { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => CSMSId.ToString();


        public UploadAPI  HTTPUploadAPI             { get; }

        public IPPort     HTTPUploadPort            { get; }

        public DNSClient  DNSClient                 { get; }

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean    RequireAuthentication     { get; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan   DefaultRequestTimeout     { get; }


        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<ICSMSServer> CSMSServers
            => centralSystemServers;

        /// <summary>
        /// The unique identifications of all connected or reachable charge boxes.
        /// </summary>
        public IEnumerable<ChargeBox_Id> ChargeBoxIds
            => reachableChargingBoxes.Values.SelectMany(box => box.Item1.ChargeBoxIds);


        public Dictionary<String, Transaction_Id> TransactionIds = new ();

        #endregion

        #region Events

        // CP -> CS

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat request was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a response to an authorize request was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        public event OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a response to a transaction event request was sent.
        /// </summary>
        public event OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion


        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever an IncomingDataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingDataTransfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion


        //ToDo: Add security extensions

        // LogStatusNotification
        // SecurityEventNotification
        // SignCertificate
        // SignedFirmwareStatusNotification



        // CS -> CP

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnIncomingDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnIncomingDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        // Security extensions

        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed request was sent.
        /// </summary>
        public event CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        public event CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate request was sent.
        /// </summary>
        public event CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was sent.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log request was sent.
        /// </summary>
        public event CS.OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        public event CS.OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate request was sent.
        /// </summary>
        public event CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        public event CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

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
        /// <param name="CSMSId">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        public TestCSMS(CSMS_Id  CSMSId,
                                 Boolean           RequireAuthentication   = true,
                                 TimeSpan?         DefaultRequestTimeout   = null,
                                 IPPort?           HTTPUploadPort          = null,
                                 DNSClient?        DNSClient               = null)
        {

            if (CSMSId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(CSMSId), "The given central system identification must not be null or empty!");

            this.CSMSId         = CSMSId;
            this.RequireAuthentication   = RequireAuthentication;
            this.DefaultRequestTimeout   = DefaultRequestTimeout ?? defaultRequestTimeout;
            this.HTTPUploadPort          = HTTPUploadPort        ?? DefaultHTTPUploadPort;
            this.centralSystemServers    = new HashSet<ICSMSServer>();
            this.reachableChargingBoxes  = new Dictionary<ChargeBox_Id, Tuple<ICSMS, DateTime>>();
            this.chargeBoxes             = new Dictionary<ChargeBox_Id, ChargeBox>();

            Directory.CreateDirectory("HTTPSSEs");

            this.TestAPI                 = new UsersAPI(
                                               HTTPServerPort:        IPPort.Parse(3500),
                                               HTTPServerName:        "GraphDefined OCPP Test Central System",
                                               HTTPServiceName:       "GraphDefined OCPP Test Central System Service",
                                               APIRobotEMailAddress:  EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                               SMTPClient:            new NullMailer(),
                                               DNSClient:             DNSClient,
                                               Autostart:             true
                                           );

            this.TestAPI.HTTPServer.AddAuth(request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(TestAPI.URLPathPrefix + "/webapi"))
                {
                    return UsersAPI.Anonymous;
                }

                #endregion

                return null;

            });


            this.HTTPUploadAPI           = new UploadAPI(
                                               this,
                                               new HTTPServer(
                                                   this.HTTPUploadPort,
                                                   "Open Charging Cloud OCPP Upload Server",
                                                   "Open Charging Cloud OCPP Upload Service"
                                               )
                                           );

            this.WebAPI                  = new OCPPWebAPI(
                                               this,
                                               TestAPI.HTTPServer
                                           );

            this.DNSClient               = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

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
        /// <param name="Autostart">Start the server immediately.</param>
        public CSMSWSServer CreateWebSocketService(String       HTTPServerName               = CSMSWSServer.DefaultHTTPServiceName,
                                                            IIPAddress?  IPAddress                    = null,
                                                            IPPort?      TCPPort                      = null,

                                                            Boolean      DisableWebSocketPings        = false,
                                                            TimeSpan?    WebSocketPingEvery           = null,
                                                            TimeSpan?    SlowNetworkSimulationDelay   = null,

                                                            DNSClient?   DNSClient                    = null,
                                                            Boolean      Autostart                    = false)
        {

            var centralSystemServer = new CSMSWSServer(
                                          HTTPServerName,
                                          IPAddress,
                                          TCPPort,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DNSClient ?? this.DNSClient,
                                          false
                                      );

            Attach(centralSystemServer);


            #region OnServerStarted

            centralSystemServer.OnServerStarted += async (Timestamp,
                                                          server,
                                                          eventTrackingId) => {

                DebugX.Log("OCPP " + Version.Number + " web socket server has started on " + server.IPSocket);

            };

            #endregion


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


            #region OnTextMessageRequest

            centralSystemServer.OnTextMessageRequest += async (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               webSocketTextMessageRequest,
                                                               cancellationToken) => {

                if (OnTextMessageRequest is not null)
                    await OnTextMessageRequest.Invoke(timestamp,
                                                      webSocketServer,
                                                      webSocketConnection,
                                                      webSocketTextMessageRequest,
                                                      cancellationToken);

            };

            #endregion

            #region OnTextMessageResponse

            centralSystemServer.OnTextMessageResponse += async (timestamp,
                                                                webSocketServer,
                                                                webSocketConnection,
                                                                eventTrackingId,
                                                                requestTimestamp,
                                                                requestMessage,
                                                                responseTimestamp,
                                                                responseMessage) => {

                if (OnTextMessageResponse is not null)
                    await OnTextMessageResponse.Invoke(timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       requestMessage,
                                                       responseTimestamp,
                                                       responseMessage);

            };

            #endregion


            #region OnCloseMessageReceived

            //centralSystemServer.OnCloseMessageReceived += async (timestamp,
            //                                                     server,
            //                                                     connection,
            //                                                     message,
            //                                                     eventTrackingId) => {

            //    DebugX.Log(String.Concat("HTTP web socket server on ",
            //                             server.IPSocket,
            //                             " closed connection to ",
            //                             connection.RemoteSocket));

            //};

            #endregion


            if (Autostart)
                centralSystemServer.Start();

            return centralSystemServer;

        }

        #endregion

        #region Attach(CSMSServer)

        public void Attach(ICSMSServer CSMSServer)
        {

            #region Initial checks

            if (CSMSServer is null)
                throw new ArgumentNullException(nameof(CSMSServer), "The given central system must not be null!");

            #endregion


            centralSystemServers.Add(CSMSServer);


            if (CSMSServer is CSMSWSServer centralSystemWSServer)
            {
                centralSystemWSServer.OnNewCSMSWSConnection += async (LogTimestamp,
                                                                               CSMS,
                                                                               Connection,
                                                                               EventTrackingId,
                                                                               CancellationToken) =>
                {

                    if (Connection.TryGetCustomDataAs("chargeBoxId", out ChargeBox_Id chargeBoxId))
                    {
                        //ToDo: lock(...)
                        if (!reachableChargingBoxes.ContainsKey(chargeBoxId))
                            reachableChargingBoxes.Add(chargeBoxId, new Tuple<ICSMS, DateTime>(CSMS, Timestamp.Now));
                        else
                            reachableChargingBoxes[chargeBoxId]   = new Tuple<ICSMS, DateTime>(CSMS, Timestamp.Now);
                    }

                };
            }


            // Wire events...

            #region OnBootNotification

            CSMSServer.OnBootNotification += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnBootNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnBootNotificationRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBootNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnBootNotification: " + Request.ChargeBoxId   //          + ", " +
                                                           //Request.ChargePointVendor       + ", " +
                                                           //Request.ChargePointModel        + ", " +
                                                           //Request.ChargePointSerialNumber + ", " +
                                                           //Request.ChargeBoxSerialNumber
                                                           );


                //await AddChargeBoxIfNotExists(new ChargeBox(Request.ChargeBoxId,
                //                                            1,
                //                                            Request.ChargePointVendor,
                //                                            Request.ChargePointModel,
                //                                            null,
                //                                            Request.ChargePointSerialNumber,
                //                                            Request.ChargeBoxSerialNumber,
                //                                            Request.FirmwareVersion,
                //                                            Request.Iccid,
                //                                            Request.IMSI,
                //                                            Request.MeterType,
                //                                            Request.MeterSerialNumber));


                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }


                await Task.Delay(100, CancellationToken);


                var response = new BootNotificationResponse(Request:      Request,
                                                            Status:       RegistrationStatus.Accepted,
                                                            CurrentTime:  Timestamp.Now,
                                                            Interval:     TimeSpan.FromMinutes(5));


                #region Send OnBootNotificationResponse event

                try
                {

                    OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBootNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnHeartbeat

            CSMSServer.OnHeartbeat += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnHeartbeatRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnHeartbeatRequest?.Invoke(startTime,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnHeartbeatRequest));
                }

                #endregion


                Console.WriteLine("OnHeartbeat: " + Request.ChargeBoxId);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }


                await Task.Delay(100, CancellationToken);


                var response = new HeartbeatResponse(Request:      Request,
                                                     CurrentTime:  Timestamp.Now);


                #region Send OnHeartbeatResponse event

                try
                {

                    OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnHeartbeatResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnAuthorize

            CSMSServer.OnAuthorize += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnAuthorizeRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnAuthorizeRequest?.Invoke(startTime,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAuthorizeRequest));
                }

                #endregion


                Console.WriteLine("OnAuthorize: " + Request.ChargeBoxId + ", " +
                                                    Request.IdToken);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100, CancellationToken);

                var response = new AuthorizeResponse(
                                   Request:      Request,
                                   IdTokenInfo:  new IdTokenInfo(
                                                     Status:               AuthorizationStatus.Accepted,
                                                     CacheExpiryDateTime:  Timestamp.Now.AddDays(3)
                                                 )
                               );


                #region Send OnAuthorizeResponse event

                try
                {

                    OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAuthorizeResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnTransactionEvent

            CSMSServer.OnTransactionEvent += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnTransactionEventRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnTransactionEventRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTransactionEventRequest));
                }

                #endregion


                Console.WriteLine("OnTransactionEvent: " + Request.ChargeBoxId + ", " +
                                                           Request.IdToken);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100, CancellationToken);

                var response = new TransactionEventResponse(
                                   Request:                 Request,
                                   TotalCost:               null,
                                   ChargingPriority:        null,
                                   IdTokenInfo:             null,
                                   UpdatedPersonalMessage:  null,
                                   CustomData:              null
                               );


                #region Send OnTransactionEventResponse event

                try
                {

                    OnTransactionEventResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTransactionEventResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStatusNotification

            CSMSServer.OnStatusNotification += async (LogTimestamp,
                                                               Sender,
                                                               Request,
                                                               CancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnStatusNotificationRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnStatusNotification: " + Request.ConnectorId  //   + ", " +
                                                             //Request.Status          + ", " +
                                                             //Request.ErrorCode       + ", " +
                                                             //Request.Info            + ", " +
                                                             //Request.StatusTimestamp + ", " +
                                                             //Request.VendorId        + ", " +
                                                             //Request.VendorErrorCode
                                                             );

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100, CancellationToken);

                var response = new StatusNotificationResponse(Request);


                #region Send OnStatusNotificationResponse event

                try
                {

                    OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         Request,
                                                         response,
                                                         Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnMeterValues

            CSMSServer.OnMeterValues += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnMeterValuesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnMeterValuesRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnMeterValuesRequest));
                }

                                                            #endregion


                Console.WriteLine("OnMeterValues: " + Request.EVSEId);

                Console.WriteLine(Request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100, CancellationToken);

                var response = new MeterValuesResponse(Request);


                #region Send OnMeterValuesResponse event

                try
                {

                    OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnMeterValuesResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnIncomingDataTransfer

            CSMSServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnIncomingDataRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(startTime,
                                                          this,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnIncomingDataTransferRequest));
                }

                #endregion


                Console.WriteLine("OnIncomingDataTransfer: " + Request.VendorId  + ", " +
                                                               Request.MessageId + ", " +
                                                               Request.Data);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100, CancellationToken);

                var responseData = Request.Data;

                if (Request.Data is not null)
                {

                    if      (Request.Data.Type == JTokenType.String)
                        responseData = Request.Data.ToString().Reverse();

                    else if (Request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (Request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                   property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (Request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (Request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }

                var response      = new DataTransferResponse(
                                        Request:  Request,
                                        Status:   DataTransferStatus.Accepted,
                                        Data:     responseData
                                    );


                #region Send OnIncomingDataResponse event

                try
                {

                    OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           Request,
                                                           response,
                                                           Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnIncomingDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnFirmwareStatusNotification

            CSMSServer.OnFirmwareStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnFirmwareStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnFirmwareStatus: " + Request.Status);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICSMS, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100, CancellationToken);

                var response = new FirmwareStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

        }

        #endregion


        #region AddHTTPBasicAuth(ChargeBoxId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of the charge box.</param>
        /// <param name="Password">The password of the charge box.</param>
        public void AddHTTPBasicAuth(ChargeBox_Id  ChargeBoxId,
                                     String        Password)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CSMSWSServer centralSystemWSServer)
                {

                    centralSystemWSServer.AddHTTPBasicAuth(ChargeBoxId,
                                                           Password);

                }
            }

        }

        #endregion


        #region ChargeBoxes

        #region Data

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        protected internal readonly Dictionary<ChargeBox_Id, ChargeBox> chargeBoxes;

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

                        return chargeBoxes.Values.ToArray();

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

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
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

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal is not null)
                await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
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

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxIfNotExistsResult.Success(chargeBoxes[ChargeBox.Id],
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

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

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

            if (chargeBoxes.TryGetValue(ChargeBox.Id, out ChargeBox OldChargeBox))
            {
                chargeBoxes.Remove(OldChargeBox.Id);
                ChargeBox.CopyAllLinkedDataFrom(OldChargeBox);
            }

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

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
        public event OnChargeBoxUpdatedDelegate? OnChargeBoxUpdated;


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

            chargeBoxes.Remove(OldChargeBox.Id);
            ChargeBox.CopyAllLinkedDataFrom(OldChargeBox);
            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

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

            chargeBoxes.Remove(ChargeBox.Id);
            updatedChargeBox.CopyAllLinkedDataFrom(ChargeBox);
            chargeBoxes.Add(updatedChargeBox.Id, updatedChargeBox);

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

            => ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.ContainsKey(ChargeBoxId);

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal Boolean _ChargeBoxExists(ChargeBox_Id? ChargeBoxId)

            => ChargeBoxId.IsNotNullOrEmpty() && chargeBoxes.ContainsKey(ChargeBoxId.Value);


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

            if (ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(ChargeBoxId, out ChargeBox? chargeBox))
                return chargeBox;

            return default;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id? ChargeBoxId)
        {

            if (ChargeBoxId is not null && chargeBoxes.TryGetValue(ChargeBoxId.Value, out ChargeBox? chargeBox))
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

            if (ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(ChargeBoxId, out ChargeBox? chargeBox))
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

            if (ChargeBoxId is not null && chargeBoxes.TryGetValue(ChargeBoxId.Value, out ChargeBox? chargeBox))
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
        public event OnChargeBoxDeletedDelegate? OnChargeBoxDeleted;


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

            if (!chargeBoxes.TryGetValue(ChargeBox.Id, out ChargeBox? ChargeBoxToBeDeleted))
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

            chargeBoxes.Remove(ChargeBox.Id);

            OnDeleted?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxDeletedLocal = OnChargeBoxDeleted;
            if (OnChargeBoxDeletedLocal is not null)
                await OnChargeBoxDeletedLocal.Invoke(Timestamp.Now,
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


        #region (private) NextRequestId

        private Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion


        #region Reset                 (ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charging station should perform.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.ResetResponse>

            Reset(ChargeBox_Id        ChargeBoxId,
                  ResetTypes          ResetType,
                  CustomData?         CustomData          = null,

                  DateTime?           RequestTimestamp    = null,
                  TimeSpan?           RequestTimeout      = null,
                  EventTracking_Id?   EventTrackingId     = null,
                  CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ResetRequest(
                                 ChargeBoxId,
                                 ResetType,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnResetRequest event

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnResetRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.Reset(request)

                               : new CS.ResetResponse(request,
                                                      Result.Server("Unknown or unreachable charge box!"));


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability    (ChargeBoxId, ConnectorId, OperationalStatus, ...)

        /// <summary>
        /// Change the availability of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charging station and all its connectors should be changed.</param>
        /// <param name="OperationalStatus">The new availability of the charging station or charging station connector.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.ChangeAvailabilityResponse>

            ChangeAvailability(ChargeBox_Id        ChargeBoxId,
                               OperationalStatus   OperationalStatus,
                               EVSE?               EVSE,
                               CustomData?         CustomData          = null,

                               DateTime?           RequestTimestamp    = null,
                               TimeSpan?           RequestTimeout      = null,
                               EventTracking_Id?   EventTrackingId     = null,
                               CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ChangeAvailabilityRequest(
                                 ChargeBoxId,
                                 OperationalStatus,
                                 EVSE,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnChangeAvailabilityRequest event

            try
            {

                OnChangeAvailabilityRequest?.Invoke(startTime,
                                                    this,
                                                    request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.ChangeAvailability(request)

                               : new CS.ChangeAvailabilityResponse(request,
                                                                   Result.Server("Unknown or unreachable charge box!"));


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                                     this,
                                                     request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData          (ChargeBoxId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.DataTransferResponse>

            TransferData(ChargeBox_Id        ChargeBoxId,
                         String              VendorId,
                         String?             MessageId           = null,
                         String?             Data                = null,
                         CustomData?         CustomData          = null,

                         DateTime?           RequestTimestamp    = null,
                         TimeSpan?           RequestTimeout      = null,
                         EventTracking_Id?   EventTrackingId     = null,
                         CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new DataTransferRequest(
                                 ChargeBoxId,
                                 VendorId,
                                 MessageId,
                                 Data,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.TransferData(request)

                               : new CS.DataTransferResponse(request,
                                                             Result.Server("Unknown or unreachable charge box!"));


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage        (ChargeBoxId, RequestedMessage, ConnectorId = null,...)

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.TriggerMessageResponse>

            TriggerMessage(ChargeBox_Id        ChargeBoxId,
                           MessageTriggers     RequestedMessage,
                           EVSE_Id?            EVSEId              = null,
                           CustomData?         CustomData          = null,

                           DateTime?           RequestTimestamp    = null,
                           TimeSpan?           RequestTimeout      = null,
                           EventTracking_Id?   EventTrackingId     = null,
                           CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new TriggerMessageRequest(
                                 ChargeBoxId,
                                 RequestedMessage,
                                 EVSEId,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnTriggerMessageRequest event

            try
            {

                OnTriggerMessageRequest?.Invoke(startTime,
                                                this,
                                                request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.TriggerMessage(request)

                               : new CS.TriggerMessageResponse(request,
                                                               Result.Server("Unknown or unreachable charge box!"));


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                                 this,
                                                 request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware        (ChargeBoxId, FirmwareURL, RetrieveTimestamp, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareURL">The URL where to download the firmware.</param>
        /// <param name="RetrieveTimestamp">The timestamp after which the charging station must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charging station for trying to download the firmware before giving up. If this field is not present, it is left to the charging station to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charging station to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.UpdateFirmwareResponse>

            UpdateFirmware(ChargeBox_Id        ChargeBoxId,
                           Firmware            Firmware,
                           Byte?               Retries             = null,
                           TimeSpan?           RetryInterval       = null,
                           CustomData?         CustomData          = null,

                           DateTime?           RequestTimestamp    = null,
                           TimeSpan?           RequestTimeout      = null,
                           EventTracking_Id?   EventTrackingId     = null,
                           CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new UpdateFirmwareRequest(
                                 ChargeBoxId,
                                 Firmware,
                                 0,
                                 Retries,
                                 RetryInterval,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnUpdateFirmwareRequest event

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                                this,
                                                request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.UpdateFirmware(request)

                               : new CS.UpdateFirmwareResponse(request,
                                                               Result.Server("Unknown or unreachable charge box!"));


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow            (ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdToken">The identifier for which the charging station has to reserve a connector.</param>
        /// <param name="GroupIdToken">An optional ParentIdTag.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.ReserveNowResponse>

            ReserveNow(ChargeBox_Id        ChargeBoxId,
                       Connector_Id        ConnectorId,
                       Reservation_Id      ReservationId,
                       DateTime            ExpiryDate,
                       IdToken             IdToken,
                       ConnectorTypes?     ConnectorType       = null,
                       EVSE_Id?            EVSEId              = null,
                       IdToken?            GroupIdToken        = null,
                       CustomData?         CustomData          = null,

                       DateTime?           RequestTimestamp    = null,
                       TimeSpan?           RequestTimeout      = null,
                       EventTracking_Id?   EventTrackingId     = null,
                       CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ReserveNowRequest(
                                 ChargeBoxId,
                                 ReservationId,
                                 ExpiryDate,
                                 IdToken,
                                 ConnectorType,
                                 EVSEId,
                                 GroupIdToken,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnReserveNowRequest event

            try
            {

                OnReserveNowRequest?.Invoke(startTime,
                                            this,
                                            request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.ReserveNow(request)

                               : new CS.ReserveNowResponse(request,
                                                           Result.Server("Unknown or unreachable charge box!"));


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                             this,
                                             request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation     (ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.CancelReservationResponse>

            CancelReservation(ChargeBox_Id        ChargeBoxId,
                              Reservation_Id      ReservationId,
                              CustomData?         CustomData          = null,

                              DateTime?           RequestTimestamp    = null,
                              TimeSpan?           RequestTimeout      = null,
                              EventTracking_Id?   EventTrackingId     = null,
                              CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new CancelReservationRequest(
                                 ChargeBoxId,
                                 ReservationId,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnCancelReservationRequest event

            try
            {

                OnCancelReservationRequest?.Invoke(startTime,
                                                   this,
                                                   request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.CancelReservation(request)

                               : new CS.CancelReservationResponse(request,
                                                                  Result.Server("Unknown or unreachable charge box!"));


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                                    this,
                                                    request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile    (ChargeBoxId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.SetChargingProfileResponse>

            SetChargingProfile(ChargeBox_Id        ChargeBoxId,
                               EVSE_Id             EVSEId,
                               ChargingProfile     ChargingProfile,
                               CustomData?         CustomData          = null,

                               DateTime?           RequestTimestamp    = null,
                               TimeSpan?           RequestTimeout      = null,
                               EventTracking_Id?   EventTrackingId     = null,
                               CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new SetChargingProfileRequest(
                                 ChargeBoxId,
                                 EVSEId,
                                 ChargingProfile,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnSetChargingProfileRequest event

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                                    this,
                                                    request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.SetChargingProfile(request)

                               : new CS.SetChargingProfileResponse(request,
                                                                   Result.Server("Unknown or unreachable charge box!"));


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile  (ChargeBoxId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charging station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.ClearChargingProfileResponse>

            ClearChargingProfile(ChargeBox_Id           ChargeBoxId,
                                 ChargingProfile_Id?    ChargingProfileId         = null,
                                 ClearChargingProfile?  ChargingProfileCriteria   = null,
                                 CustomData?            CustomData                = null,

                                 DateTime?              RequestTimestamp          = null,
                                 TimeSpan?              RequestTimeout            = null,
                                 EventTracking_Id?      EventTrackingId           = null,
                                 CancellationToken?     CancellationToken         = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ClearChargingProfileRequest(
                                 ChargeBoxId,
                                 ChargingProfileId,
                                 ChargingProfileCriteria,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnClearChargingProfileRequest event

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                                      this,
                                                      request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.ClearChargingProfile(request)

                               : new CS.ClearChargingProfileResponse(request,
                                                                     Result.Server("Unknown or unreachable charge box!"));


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule  (ChargeBoxId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.GetCompositeScheduleResponse>

            GetCompositeSchedule(ChargeBox_Id        ChargeBoxId,
                                 TimeSpan            Duration,
                                 EVSE_Id             EVSEId,
                                 ChargingRateUnits?  ChargingRateUnit    = null,
                                 CustomData?         CustomData          = null,

                                 DateTime?           RequestTimestamp    = null,
                                 TimeSpan?           RequestTimeout      = null,
                                 EventTracking_Id?   EventTrackingId     = null,
                                 CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetCompositeScheduleRequest(
                                 ChargeBoxId,
                                 Duration,
                                 EVSEId,
                                 ChargingRateUnit,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetCompositeScheduleRequest event

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(startTime,
                                                      this,
                                                      request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.GetCompositeSchedule(request)

                               : new CS.GetCompositeScheduleResponse(request,
                                                                     Result.Server("Unknown or unreachable charge box!"));


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                                       this,
                                                       request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector       (ChargeBoxId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.UnlockConnectorResponse>

            UnlockConnector(ChargeBox_Id        ChargeBoxId,
                            EVSE_Id             EVSEId,
                            Connector_Id        ConnectorId,
                            CustomData?         CustomData          = null,

                            DateTime?           RequestTimestamp    = null,
                            TimeSpan?           RequestTimeout      = null,
                            EventTracking_Id?   EventTrackingId     = null,
                            CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new UnlockConnectorRequest(
                                 ChargeBoxId,
                                 EVSEId,
                                 ConnectorId,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnUnlockConnectorRequest event

            try
            {

                OnUnlockConnectorRequest?.Invoke(startTime,
                                                 this,
                                                 request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.UnlockConnector(request)

                               : new CS.UnlockConnectorResponse(request,
                                                                Result.Server("Unknown or unreachable charge box!"));


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                                  this,
                                                  request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion   (ChargeBoxId, ...)

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.GetLocalListVersionResponse>

            GetLocalListVersion(ChargeBox_Id        ChargeBoxId,
                                CustomData          CustomData          = null,

                                DateTime?           RequestTimestamp    = null,
                                TimeSpan?           RequestTimeout      = null,
                                EventTracking_Id?   EventTrackingId     = null,
                                CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetLocalListVersionRequest(
                                 ChargeBoxId,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetLocalListVersionRequest event

            try
            {

                OnGetLocalListVersionRequest?.Invoke(startTime,
                                                     this,
                                                     request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.GetLocalListVersion(request)

                               : new CS.GetLocalListVersionResponse(request,
                                                                    Result.Server("Unknown or unreachable charge box!"));


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                                      this,
                                                      request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList         (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charging station. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.SendLocalListResponse>

            SendLocalList(ChargeBox_Id                     ChargeBoxId,
                          UInt64                           ListVersion,
                          UpdateTypes                      UpdateType,
                          IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,
                          CustomData?                      CustomData               = null,

                          DateTime?                        RequestTimestamp         = null,
                          TimeSpan?                        RequestTimeout           = null,
                          EventTracking_Id?                EventTrackingId          = null,
                          CancellationToken?               CancellationToken        = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new SendLocalListRequest(
                                 ChargeBoxId,
                                 ListVersion,
                                 UpdateType,
                                 LocalAuthorizationList,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnSendLocalListRequest event

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                               this,
                                               request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.SendLocalList(request)

                               : new CS.SendLocalListResponse(request,
                                                              Result.Server("Unknown or unreachable charge box!"));


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache            (ChargeBoxId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.ClearCacheResponse>

            ClearCache(ChargeBox_Id        ChargeBoxId,
                       CustomData?         CustomData          = null,

                       DateTime?           RequestTimestamp    = null,
                       TimeSpan?           RequestTimeout      = null,
                       EventTracking_Id?   EventTrackingId     = null,
                       CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ClearCacheRequest(
                                 ChargeBoxId,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnClearCacheRequest event

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                            this,
                                            request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.ClearCache(request)

                               : new CS.ClearCacheResponse(request,
                                                           Result.Server("Unknown or unreachable charge box!"));


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             this,
                                             request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region CertificateSigned         (ChargeBoxId, CertificateChain, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.CertificateSignedResponse>

            CertificateSigned(ChargeBox_Id            ChargeBoxId,
                              CertificateChain        CertificateChain,
                              CertificateSigningUse?  CertificateType     = null,
                              CustomData?             CustomData          = null,

                              DateTime?               RequestTimestamp    = null,
                              TimeSpan?               RequestTimeout      = null,
                              EventTracking_Id?       EventTrackingId     = null,
                              CancellationToken?      CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new CertificateSignedRequest(
                                 ChargeBoxId,
                                 CertificateChain,
                                 CertificateType,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnCertificateSignedRequest event

            try
            {

                OnCertificateSignedRequest?.Invoke(startTime,
                                                   this,
                                                   request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.SendSignedCertificate(request)

                               : new CS.CertificateSignedResponse(request,
                                                           Result.Server("Unknown or unreachable charge box!"));


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponse?.Invoke(endTime,
                                                    this,
                                                    request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate         (ChargeBoxId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.DeleteCertificateResponse>

            DeleteCertificate(ChargeBox_Id         ChargeBoxId,
                              CertificateHashData  CertificateHashData,
                              CustomData?          CustomData          = null,

                              DateTime?            RequestTimestamp    = null,
                              TimeSpan?            RequestTimeout      = null,
                              EventTracking_Id?    EventTrackingId     = null,
                              CancellationToken?   CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new DeleteCertificateRequest(
                                 ChargeBoxId,
                                 CertificateHashData,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnDeleteCertificateRequest event

            try
            {

                OnDeleteCertificateRequest?.Invoke(startTime,
                                                   this,
                                                   request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.DeleteCertificate(request)

                               : new CS.DeleteCertificateResponse(request,
                                                                  Result.Server("Unknown or unreachable charge box!"));


            #region Send OnDeleteCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateResponse?.Invoke(endTime,
                                                    this,
                                                    request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds(ChargeBoxId, CertificateType, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateType">The type of the certificates requested.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.GetInstalledCertificateIdsResponse>

            GetInstalledCertificateIds(ChargeBox_Id        ChargeBoxId,
                                       CertificateUse      CertificateType,
                                       CustomData?         CustomData          = null,

                                       DateTime?           RequestTimestamp    = null,
                                       TimeSpan?           RequestTimeout      = null,
                                       EventTracking_Id?   EventTrackingId     = null,
                                       CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetInstalledCertificateIdsRequest(
                                 ChargeBoxId,
                                 CertificateType,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetInstalledCertificateIdsRequest event

            try
            {

                OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
                                                            this,
                                                            request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.GetInstalledCertificateIds(request)

                               : new CS.GetInstalledCertificateIdsResponse(request,
                                                                           Result.Server("Unknown or unreachable charge box!"));


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
                                                             this,
                                                             request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                    (ChargeBoxId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="LogType">The type of the certificates requested.</param>
        /// <param name="LogRequestId">The unique identification of this request.</param>
        /// <param name="Log">This field specifies the requested log and the location to which the log should be sent.</param>
        /// <param name="Retries">This specifies how many times the Charge Point must try to upload the log before giving up. If this field is not present, it is left to Charge Point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to Charge Point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.GetLogResponse>

            GetLog(ChargeBox_Id        ChargeBoxId,
                   LogTypes            LogType,
                   Int32               LogRequestId,
                   LogParameters       Log,
                   Byte?               Retries             = null,
                   TimeSpan?           RetryInterval       = null,
                   CustomData?         CustomData          = null,

                   DateTime?           RequestTimestamp    = null,
                   TimeSpan?           RequestTimeout      = null,
                   EventTracking_Id?   EventTrackingId     = null,
                   CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetLogRequest(
                                 ChargeBoxId,
                                 LogType,
                                 LogRequestId,
                                 Log,
                                 Retries,
                                 RetryInterval,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetLogRequest event

            try
            {

                OnGetLogRequest?.Invoke(startTime,
                                        this,
                                        request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.GetLog(request)

                               : new CS.GetLogResponse(request,
                                                       Result.Server("Unknown or unreachable charge box!"));


            #region Send OnGetLogResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLogResponse?.Invoke(endTime,
                                         this,
                                         request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate        (ChargeBoxId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.InstallCertificateResponse>

            InstallCertificate(ChargeBox_Id        ChargeBoxId,
                               CertificateUse      CertificateType,
                               Certificate         Certificate,
                               CustomData?         CustomData          = null,

                               DateTime?           RequestTimestamp    = null,
                               TimeSpan?           RequestTimeout      = null,
                               EventTracking_Id?   EventTrackingId     = null,
                               CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new InstallCertificateRequest(
                                 ChargeBoxId,
                                 CertificateType,
                                 Certificate,
                                 CustomData,

                                 NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnInstallCertificateRequest event

            try
            {

                OnInstallCertificateRequest?.Invoke(startTime,
                                                    this,
                                                    request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            var response = reachableChargingBoxes.TryGetValue(ChargeBoxId, out var centralSystem) && centralSystem is not null

                               ? await centralSystem.Item1.InstallCertificate(request)

                               : new CS.InstallCertificateResponse(request,
                                                                   Result.Server("Unknown or unreachable charge box!"));


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                                     this,
                                                     request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
