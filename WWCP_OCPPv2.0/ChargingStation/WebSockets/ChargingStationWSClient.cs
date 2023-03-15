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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_0.CSMS;
using cloud.charging.open.protocols.OCPPv2_0.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The charge point HTTP web socket client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer
    {

        #region (class) SendRequestState

        public class SendRequestState2
        {

            public DateTime                       Timestamp           { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage    { get; }
            public DateTime                       Timeout             { get; }

            public JObject?                       Response            { get; set; }
            public ResultCodes?                   ErrorCode           { get; set; }
            public String?                        ErrorDescription    { get; set; }
            public JObject?                       ErrorDetails        { get; set; }

            public SendRequestState2(DateTime                       Timestamp,
                                     OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                     DateTime                       Timeout,

                                     JObject?                       Response           = null,
                                     ResultCodes?                   ErrorCode          = null,
                                     String?                        ErrorDescription   = null,
                                     JObject?                       ErrorDetails       = null)
            {

                this.Timestamp         = Timestamp;
                this.WSRequestMessage  = WSRequestMessage;
                this.Timeout           = Timeout;

                this.Response          = Response;
                this.ErrorCode         = ErrorCode;
                this.ErrorDescription  = ErrorDescription;
                this.ErrorDetails      = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public  new const  String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.Number + " CP WebSocket Client";

        private     const  String  LogfileName           = "ChargePointWSClient.log";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id                         ChargeBoxIdentity               { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From                            { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                              { get; }


        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        //public ChargePointWSClient.CPClientLogger    Logger                          { get; }

        #endregion

        #region CustomRequestParsers

        public CustomJObjectParserDelegate<ResetRequest>?                   CustomResetRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>?      CustomChangeAvailabilityRequestParser        { get; set; }
        public CustomJObjectParserDelegate<CSMS.DataTransferRequest>?         CustomIncomingDataTransferRequestParser      { get; set; }
        public CustomJObjectParserDelegate<TriggerMessageRequest>?          CustomTriggerMessageRequestParser            { get; set; }
        public CustomJObjectParserDelegate<UpdateFirmwareRequest>?          CustomUpdateFirmwareRequestParser            { get; set; }

        public CustomJObjectParserDelegate<ReserveNowRequest>?              CustomReserveNowRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CancelReservationRequest>?       CustomCancelReservationRequestParser         { get; set; }
        public CustomJObjectParserDelegate<SetChargingProfileRequest>?      CustomSetChargingProfileRequestParser        { get; set; }
        public CustomJObjectParserDelegate<ClearChargingProfileRequest>?    CustomClearChargingProfileRequestParser      { get; set; }
        public CustomJObjectParserDelegate<GetCompositeScheduleRequest>?    CustomGetCompositeScheduleRequestParser      { get; set; }
        public CustomJObjectParserDelegate<UnlockConnectorRequest>?         CustomUnlockConnectorRequestParser           { get; set; }

        public CustomJObjectParserDelegate<GetLocalListVersionRequest>?     CustomGetLocalListVersionRequestParser       { get; set; }
        public CustomJObjectParserDelegate<SendLocalListRequest>?           CustomSendLocalListRequestParser             { get; set; }
        public CustomJObjectParserDelegate<ClearCacheRequest>?              CustomClearCacheRequestParser                { get; set; }

        #endregion

        //ToDo: Add reponse serializers!!!

        #region Events

        // Outgoing messages (to central system)

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?             OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?            OnBootNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?      OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event ClientResponseLogHandler?     OnHeartbeatWSResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?      OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?     OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?               OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?        OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnMeterValuesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?         OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?        OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                       OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                      OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion


        // Security extensions

        #region OnLogStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a log status notification request will be send to the central system.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a log status notification request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                  OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                 OnLogStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region OnSecurityEventNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a security event notification request will be send to the central system.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a security event notification request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                      OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                     OnSecurityEventNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region OnSignCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a sign certificate request will be send to the central system.
        /// </summary>
        public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a sign certificate request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?            OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?           OnSignCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion


        // Incoming messages (from central system)

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate?        OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate?               OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?       OnResetResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnResetWSResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate?            OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnChangeAvailabilityWSResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnIncomingDataTransferWSResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageDelegate?            OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnTriggerMessageWSResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?            OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnUpdateFirmwareWSResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnReserveNowWSRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?     OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate?            OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?    OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reserve now request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnReserveNowWSResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnCancelReservationWSRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?     OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate?            OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?    OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cancel reservation request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnCancelReservationWSResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate?            OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnSetChargingProfileWSResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnClearChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate?            OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnClearChargingProfileWSResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?     OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?            OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?    OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnGetCompositeScheduleWSResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnUnlockConnectorWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?     OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate?            OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?    OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnUnlockConnectorWSResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?     OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate?            OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?    OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetLocalListVersionWSResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate?            OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?    OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnSendLocalListWSResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate?            OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?    OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnClearCacheWSResponse;

        #endregion


        // Security extensions


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point websocket client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPBasicAuth">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargingStationWSClient(ChargeBox_Id                          ChargeBoxIdentity,
                                   String                                From,
                                   String                                To,

                                   URL                                   RemoteURL,
                                   HTTPHostname?                         VirtualHostname              = null,
                                   String?                               Description                  = null,
                                   RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                   X509Certificate?                      ClientCert                   = null,
                                   String                                HTTPUserAgent                = DefaultHTTPUserAgent,
                                   HTTPPath?                             URLPathPrefix                = null,
                                   SslProtocols?                         TLSProtocol                  = null,
                                   Boolean?                              PreferIPv4                   = null,
                                   Tuple<String, String>?                HTTPBasicAuth                = null,
                                   TimeSpan?                             RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                   UInt16?                               MaxNumberOfRetries           = 3,
                                   Boolean                               UseHTTPPipelining            = false,

                                   Boolean                               DisableMaintenanceTasks      = false,
                                   TimeSpan?                             MaintenanceEvery             = null,
                                   Boolean                               DisableWebSocketPings        = false,
                                   TimeSpan?                             WebSocketPingEvery           = null,
                                   TimeSpan?                             SlowNetworkSimulationDelay   = null,

                                   String?                               LoggingPath                  = null,
                                   String                                LoggingContext               = null, //CPClientLogger.DefaultContext,
                                   LogfileCreatorDelegate?               LogfileCreator               = null,
                                   HTTPClientLogger?                     HTTPLogger                   = null,
                                   DNSClient?                            DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   HTTPUserAgent,
                   URLPathPrefix,
                   TLSProtocol,
                   PreferIPv4,
                   HTTPBasicAuth,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   UseHTTPPipelining,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity                  = ChargeBoxIdentity;
            this.From                               = From;
            this.To                                 = To;

            //this.Logger                             = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                                   LoggingPath,
            //                                                                                   LoggingContext,
            //                                                                                   LogfileCreator);

        }

        #endregion


        #region ProcessWebSocketTextFrame(frame)

        public override async Task ProcessWebSocketTextFrame(WebSocketFrame frame)
        {

            var textPayload = frame.Payload.ToUTF8String();

            if (textPayload == "[]")
                DebugX.Log(nameof(ChargingStationWSClient), " [] received!");

            else if (OCPP_WebSocket_RequestMessage. TryParse(textPayload, out var requestMessage)  && requestMessage  is not null)
            {

                File.AppendAllText(LogfileName,
                                   String.Concat("timestamp: ",         Timestamp.Now.ToIso8601(),                                               Environment.NewLine,
                                                 "ChargeBoxId: ",       ChargeBoxIdentity.ToString(),                                            Environment.NewLine,
                                                 "Message received: ",  requestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented),   Environment.NewLine,
                                                 "--------------------------------------------------------------------------------------------", Environment.NewLine));


                var requestJSON              = JArray.Parse(textPayload);
                var cancellationTokenSource  = new CancellationTokenSource();

                JObject?                     OCPPResponseJSON   = null;
                OCPP_WebSocket_ErrorMessage? ErrorMessage       = null;

                switch (requestMessage.Action)
                {

                    case "Reset":
                        {

                            #region Send OnResetWSRequest event

                            try
                            {

                                OnResetWSRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ResetRequest.TryParse(requestMessage.Message,
                                                          requestMessage.RequestId,
                                                          ChargeBoxIdentity,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomResetRequestParser) && request is not null) {

                                    #region Send OnResetRequest event

                                    try
                                    {

                                        OnResetRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ResetResponse? response = null;

                                    var results = OnReset?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnResetDelegate)?.Invoke(Timestamp.Now,
                                                                                                                       this,
                                                                                                                       request,
                                                                                                                       cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ResetResponse.Failed(request);

                                    #endregion

                                    #region Send OnResetResponse event

                                    try
                                    {

                                        OnResetResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnResetWSResponse event

                            try
                            {

                                OnResetWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          requestJSON,
                                                          new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                             OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ChangeAvailability":
                        {

                            #region Send OnChangeAvailabilityWSRequest event

                            try
                            {

                                OnChangeAvailabilityWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ChangeAvailabilityRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomChangeAvailabilityRequestParser) && request is not null) {

                                    #region Send OnChangeAvailabilityRequest event

                                    try
                                    {

                                        OnChangeAvailabilityRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ChangeAvailabilityResponse? response = null;

                                    var results = OnChangeAvailability?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ChangeAvailabilityResponse.Failed(request);

                                    #endregion

                                    #region Send OnChangeAvailabilityResponse event

                                    try
                                    {

                                        OnChangeAvailabilityResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnChangeAvailabilityWSResponse event

                            try
                            {

                                OnChangeAvailabilityWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "DataTransfer":
                        {

                            #region Send OnIncomingDataTransferWSRequest event

                            try
                            {

                                OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CSMS.DataTransferRequest.TryParse(requestMessage.Message,
                                                                    requestMessage.RequestId,
                                                                    ChargeBoxIdentity,
                                                                    out var request,
                                                                    out var errorResponse,
                                                                    CustomIncomingDataTransferRequestParser) && request is not null) {

                                    #region Send OnIncomingDataTransferRequest event

                                    try
                                    {

                                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    DataTransferResponse? response = null;

                                    var results = OnIncomingDataTransfer?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= DataTransferResponse.Failed(request);

                                    #endregion

                                    #region Send OnIncomingDataTransferResponse event

                                    try
                                    {

                                        OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnIncomingDataTransferWSResponse event

                            try
                            {

                                OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "TriggerMessage":
                        {

                            #region Send OnTriggerMessageWSRequest event

                            try
                            {

                                OnTriggerMessageWSRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (TriggerMessageRequest.TryParse(requestMessage.Message,
                                                                   requestMessage.RequestId,
                                                                   ChargeBoxIdentity,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomTriggerMessageRequestParser) && request is not null) {

                                    #region Send OnTriggerMessageRequest event

                                    try
                                    {

                                        OnTriggerMessageRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    TriggerMessageResponse? response = null;

                                    var results = OnTriggerMessage?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnTriggerMessageDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                request,
                                                                                                                                cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= TriggerMessageResponse.Failed(request);

                                    #endregion

                                    #region Send OnTriggerMessageResponse event

                                    try
                                    {

                                        OnTriggerMessageResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         response,
                                                                         response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnTriggerMessageWSResponse event

                            try
                            {

                                OnTriggerMessageWSResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON,
                                                                   new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                      OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UpdateFirmware":
                        {

                            #region Send OnUpdateFirmwareWSRequest event

                            try
                            {

                                OnUpdateFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UpdateFirmwareRequest.TryParse(requestMessage.Message,
                                                                   requestMessage.RequestId,
                                                                   ChargeBoxIdentity,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomUpdateFirmwareRequestParser) && request is not null) {

                                    #region Send OnUpdateFirmwareRequest event

                                    try
                                    {

                                        OnUpdateFirmwareRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UpdateFirmwareResponse? response = null;

                                    var results = OnUpdateFirmware?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUpdateFirmwareDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                request,
                                                                                                                                cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UpdateFirmwareResponse.Failed(request);

                                    #endregion

                                    #region Send OnUpdateFirmwareResponse event

                                    try
                                    {

                                        OnUpdateFirmwareResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         response,
                                                                         response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUpdateFirmwareWSResponse event

                            try
                            {

                                OnUpdateFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON,
                                                                   new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                      OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "ReserveNow":
                        {

                            #region Send OnReserveNowWSRequest event

                            try
                            {

                                OnReserveNowWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ReserveNowRequest.TryParse(requestMessage.Message,
                                                               requestMessage.RequestId,
                                                               ChargeBoxIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomReserveNowRequestParser) && request is not null) {

                                    #region Send OnReserveNowRequest event

                                    try
                                    {

                                        OnReserveNowRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ReserveNowResponse? response = null;

                                    var results = OnReserveNow?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ReserveNowResponse.Failed(request);

                                    #endregion

                                    #region Send OnReserveNowResponse event

                                    try
                                    {

                                        OnReserveNowResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnReserveNowWSResponse event

                            try
                            {

                                OnReserveNowWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON,
                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "CancelReservation":
                        {

                            #region Send OnCancelReservationWSRequest event

                            try
                            {

                                OnCancelReservationWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CancelReservationRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomCancelReservationRequestParser) && request is not null) {

                                    #region Send OnCancelReservationRequest event

                                    try
                                    {

                                        OnCancelReservationRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    CancelReservationResponse? response = null;

                                    var results = OnCancelReservation?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= CancelReservationResponse.Failed(request);

                                    #endregion

                                    #region Send OnCancelReservationResponse event

                                    try
                                    {

                                        OnCancelReservationResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnCancelReservationWSResponse event

                            try
                            {

                                OnCancelReservationWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetChargingProfile":
                        {

                            #region Send OnSetChargingProfileWSRequest event

                            try
                            {

                                OnSetChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetChargingProfileRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomSetChargingProfileRequestParser) && request is not null) {

                                    #region Send OnSetChargingProfileRequest event

                                    try
                                    {

                                        OnSetChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetChargingProfileResponse? response = null;

                                    var results = OnSetChargingProfile?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetChargingProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetChargingProfileResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetChargingProfileResponse event

                                    try
                                    {

                                        OnSetChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetChargingProfileWSResponse event

                            try
                            {

                                OnSetChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearChargingProfile":
                        {

                            #region Send OnClearChargingProfileWSRequest event

                            try
                            {

                                OnClearChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearChargingProfileRequest.TryParse(requestMessage.Message,
                                                                         requestMessage.RequestId,
                                                                         ChargeBoxIdentity,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomClearChargingProfileRequestParser) && request is not null) {

                                    #region Send OnClearChargingProfileRequest event

                                    try
                                    {

                                        OnClearChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearChargingProfileResponse? response = null;

                                    var results = OnClearChargingProfile?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearChargingProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearChargingProfileResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearChargingProfileResponse event

                                    try
                                    {

                                        OnClearChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearChargingProfileWSResponse event

                            try
                            {

                                OnClearChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetCompositeSchedule":
                        {

                            #region Send OnGetCompositeScheduleWSRequest event

                            try
                            {

                                OnGetCompositeScheduleWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetCompositeScheduleRequest.TryParse(requestMessage.Message,
                                                                         requestMessage.RequestId,
                                                                         ChargeBoxIdentity,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomGetCompositeScheduleRequestParser) && request is not null) {

                                    #region Send OnGetCompositeScheduleRequest event

                                    try
                                    {

                                        OnGetCompositeScheduleRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetCompositeScheduleResponse? response = null;

                                    var results = OnGetCompositeSchedule?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetCompositeScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetCompositeScheduleResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetCompositeScheduleResponse event

                                    try
                                    {

                                        OnGetCompositeScheduleResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetCompositeScheduleWSResponse event

                            try
                            {

                                OnGetCompositeScheduleWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UnlockConnector":
                        {

                            #region Send OnUnlockConnectorWSRequest event

                            try
                            {

                                OnUnlockConnectorWSRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UnlockConnectorRequest.TryParse(requestMessage.Message,
                                                                    requestMessage.RequestId,
                                                                    ChargeBoxIdentity,
                                                                    out var request,
                                                                    out var errorResponse,
                                                                    CustomUnlockConnectorRequestParser) && request is not null) {

                                    #region Send OnUnlockConnectorRequest event

                                    try
                                    {

                                        OnUnlockConnectorRequest?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UnlockConnectorResponse? response = null;

                                    var results = OnUnlockConnector?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUnlockConnectorDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UnlockConnectorResponse.Failed(request);

                                    #endregion

                                    #region Send OnUnlockConnectorResponse event

                                    try
                                    {

                                        OnUnlockConnectorResponse?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          request,
                                                                          response,
                                                                          response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUnlockConnectorWSResponse event

                            try
                            {

                                OnUnlockConnectorWSResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    requestJSON,
                                                                    new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                       OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "GetLocalListVersion":
                        {

                            #region Send OnGetLocalListVersionWSRequest event

                            try
                            {

                                OnGetLocalListVersionWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetLocalListVersionRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomGetLocalListVersionRequestParser) && request is not null) {

                                    #region Send OnGetLocalListVersionRequest event

                                    try
                                    {

                                        OnGetLocalListVersionRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetLocalListVersionResponse? response = null;

                                    var results = OnGetLocalListVersion?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetLocalListVersionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetLocalListVersionResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetLocalListVersionResponse event

                                    try
                                    {

                                        OnGetLocalListVersionResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetLocalListVersionWSResponse event

                            try
                            {

                                OnGetLocalListVersionWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SendLocalList":
                        {

                            #region Send OnSendLocalListWSRequest event

                            try
                            {

                                OnSendLocalListWSRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SendLocalListRequest.TryParse(requestMessage.Message,
                                                                  requestMessage.RequestId,
                                                                  ChargeBoxIdentity,
                                                                  out var request,
                                                                  out var errorResponse,
                                                                  CustomSendLocalListRequestParser) && request is not null) {

                                    #region Send OnSendLocalListRequest event

                                    try
                                    {

                                        OnSendLocalListRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SendLocalListResponse? response = null;

                                    var results = OnSendLocalList?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSendLocalListDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               request,
                                                                                                                               cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response = SendLocalListResponse.Failed(request);

                                    #endregion

                                    #region Send OnSendLocalListResponse event

                                    try
                                    {

                                        OnSendLocalListResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request,
                                                                        response,
                                                                        response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSendLocalListWSResponse event

                            try
                            {

                                OnSendLocalListWSResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON,
                                                                  new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                     OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearCache":
                        {

                            #region Send OnClearCacheWSRequest event

                            try
                            {

                                OnClearCacheWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearCacheRequest.TryParse(requestMessage.Message,
                                                               requestMessage.RequestId,
                                                               ChargeBoxIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomClearCacheRequestParser) && request is not null) {

                                    #region Send OnClearCacheRequest event

                                    try
                                    {

                                        OnClearCacheRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearCacheResponse? response = null;

                                    var results = OnClearCache?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearCacheDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearCacheResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearCacheResponse event

                                    try
                                    {

                                        OnClearCacheResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearCacheWSResponse event

                            try
                            {

                                OnClearCacheWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON,
                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSResponse));
                            }

                            #endregion

                        }
                        break;

                }

                if (OCPPResponseJSON is not null)
                {

                    var wsResponseMessage = new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                               OCPPResponseJSON);

                    SendWebSocketFrame(new WebSocketFrame(
                                           WebSocketFrame.Fin.Final,
                                           WebSocketFrame.MaskStatus.On,
                                           new Byte[] { 0xaa, 0xaa, 0xaa, 0xaa },
                                           WebSocketFrame.Opcodes.Text,
                                           wsResponseMessage.ToByteArray(),
                                           WebSocketFrame.Rsv.Off,
                                           WebSocketFrame.Rsv.Off,
                                           WebSocketFrame.Rsv.Off
                                       ));

                    File.AppendAllText(LogfileName,
                                       String.Concat("Timestamp: ",    Timestamp.Now.ToIso8601(),                                                     Environment.NewLine,
                                                     "ChargeBoxId: ",  ChargeBoxIdentity.ToString(),                                                  Environment.NewLine,
                                                     "Message sent: ", wsResponseMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented),      Environment.NewLine,
                                                     "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                }

            }

            else if (OCPP_WebSocket_ResponseMessage.TryParse(textPayload, out var responseMessage) && responseMessage is not null)
            {
                lock (requests)
                {

                    if (requests.TryGetValue(responseMessage.RequestId, out var resp))
                        resp.Response = responseMessage.Message;

                    else
                        DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP response message: " + textPayload);

                }
            }

            else if (OCPP_WebSocket_ErrorMessage.   TryParse(textPayload, out var wsErrorMessage))
            {
                DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP error message: " + textPayload);
            }

            else
                DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP request/response message: " + textPayload);

        }

        #endregion


        public readonly Dictionary<Request_Id, SendRequestState2> requests = new ();


        #region SendRequest(Action, RequestId, Message)

        public async Task<Request_Id?> SendRequest(String      Action,
                                                   Request_Id  RequestId,
                                                   JObject     Message)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        var wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                                   RequestId,
                                                   Action,
                                                   Message
                                               );

                        SendWebSocketFrame(new WebSocketFrame(
                                               WebSocketFrame.Fin.Final,
                                               WebSocketFrame.MaskStatus.On,
                                               new Byte[] { 0xaa, 0xbb, 0xcc, 0xdd },
                                               WebSocketFrame.Opcodes.Text,
                                               wsRequestMessage.ToByteArray(),
                                               WebSocketFrame.Rsv.Off,
                                               WebSocketFrame.Rsv.Off,
                                               WebSocketFrame.Rsv.Off
                                           ));

                        requests.Add(RequestId,
                                     new SendRequestState2(
                                         Timestamp.Now,
                                         wsRequestMessage,
                                         Timestamp.Now + TimeSpan.FromSeconds(10)
                                     ));

                        //File.AppendAllText(LogfileName,
                        //                   String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                               Environment.NewLine,
                        //                                 "ChargeBoxId: ",       ChargeBoxIdentity.ToString(),                                            Environment.NewLine,
                        //                                 "Message sent: ",      wsRequestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                        //                                 "--------------------------------------------------------------------------------------------", Environment.NewLine));

                    }
                    else
                    {

                        DebugX.Log("Invalid web socket connection!");

                        //DebugX.Log("Will try to reconnect!");
                        //await Connect();

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }
            else
                DebugX.LogT("Could not aquire the maintenance tasks lock!");

            return RequestId;

        }

        #endregion


        private async Task<JObject?> WaitForResponse(Request_Id? RequestId)
        {

            if (!RequestId.HasValue)
                return null;

            var endTime = Timestamp.Now + TimeSpan.FromSeconds(10);

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(RequestId.Value, out var sendRequestState) &&
                        sendRequestState?.Response is not null ||
                        sendRequestState?.ErrorCode.HasValue == true)
                    {

                        lock (requests)
                        {
                            requests.Remove(RequestId.Value);
                        }

                        return sendRequestState.Response;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargingStationWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return null;

        }



        #region SendBootNotification             (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public async Task<BootNotificationResponse>

            SendBootNotification(BootNotificationRequest Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!BootNotificationResponse.TryParse(Request,
                                                  (await WaitForResponse(requestId)) ?? new JObject(),
                                                   out var response,
                                                   out var errorResponse))
            {
                response = new BootNotificationResponse(Request,
                                                        Result.Format(errorResponse));
            }

            response ??= new BootNotificationResponse(Request,
                                                      Result.GenericError());


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBootNotificationResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                    (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public async Task<HeartbeatResponse>

            SendHeartbeat(HeartbeatRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!HeartbeatResponse.TryParse(Request,
                                           (await WaitForResponse(requestId)) ?? new JObject(),
                                            out var response,
                                            out var errorResponse))
            {
                response = new HeartbeatResponse(Request,
                                                 Result.Format(errorResponse));
            }

            response ??= new HeartbeatResponse(Request,
                                               Result.GenericError());


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Authorize                        (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public async Task<AuthorizeResponse>

            Authorize(AuthorizeRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!AuthorizeResponse.TryParse(Request,
                                           (await WaitForResponse(requestId)) ?? new JObject(),
                                            out var response,
                                            out var errorResponse))
            {
                response = new AuthorizeResponse(Request,
                                                 Result.Format(errorResponse));
            }

            response ??= new AuthorizeResponse(Request,
                                               Result.GenericError());


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification           (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public async Task<StatusNotificationResponse>

            SendStatusNotification(StatusNotificationRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!StatusNotificationResponse.TryParse(Request,
                                                    (await WaitForResponse(requestId)) ?? new JObject(),
                                                     out var response,
                                                     out var errorResponse))
            {
                response = new StatusNotificationResponse(Request,
                                                          Result.Format(errorResponse));
            }

            response ??= new StatusNotificationResponse(Request,
                                                        Result.GenericError());


            #region Send OnStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendMeterValues                  (Request)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public async Task<MeterValuesResponse>

            SendMeterValues(MeterValuesRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!MeterValuesResponse.TryParse(Request,
                                             (await WaitForResponse(requestId)) ?? new JObject(),
                                              out var response,
                                              out var errorResponse))
            {
                response = new MeterValuesResponse(Request,
                                                   Result.Format(errorResponse));
            }

            response ??= new MeterValuesResponse(Request,
                                                 Result.GenericError());


            #region Send OnMeterValuesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnMeterValuesResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData                     (Request)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public async Task<CSMS.DataTransferResponse>

            TransferData(DataTransferRequest  Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!CSMS.DataTransferResponse.TryParse(Request,
                                                 (await WaitForResponse(requestId)) ?? new JObject(),
                                                  out var response,
                                                  out var errorResponse))
            {
                response = new CSMS.DataTransferResponse(Request,
                                                       Result.Format(errorResponse));
            }

            response ??= new CSMS.DataTransferResponse(Request,
                                                     Result.GenericError());


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFirmwareStatusNotification   (Request)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!FirmwareStatusNotificationResponse.TryParse(Request,
                                                            (await WaitForResponse(requestId)) ?? new JObject(),
                                                             out var response,
                                                             out var errorResponse))
            {
                response = new FirmwareStatusNotificationResponse(Request,
                                                                  Result.Format(errorResponse));
            }

            response ??= new FirmwareStatusNotificationResponse(Request,
                                                                Result.GenericError());


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Security extensions

        #region LogStatusNotification            (Request)

        /// <summary>
        /// Send a log status notification to the central system.
        /// </summary>
        /// <param name="Request">A start transaction request.</param>
        public async Task<LogStatusNotificationResponse>

            SendLogStatusNotification(LogStatusNotificationRequest  Request)

        {

            #region Send OnLogStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnLogStatusNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!LogStatusNotificationResponse.TryParse(Request,
                                                       (await WaitForResponse(requestId)) ?? new JObject(),
                                                        out var response,
                                                        out var errorResponse))
            {
                response = new LogStatusNotificationResponse(Request,
                                                             Result.Format(errorResponse));
            }

            response ??= new LogStatusNotificationResponse(Request,
                                                           Result.GenericError());


            #region Send OnLogStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnLogStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SecurityEventNotification        (Request)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        public async Task<SecurityEventNotificationResponse>

            SendSecurityEventNotification(SecurityEventNotificationRequest  Request)

        {

            #region Send OnSecurityEventNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSecurityEventNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!SecurityEventNotificationResponse.TryParse(Request,
                                                           (await WaitForResponse(requestId)) ?? new JObject(),
                                                            out var response,
                                                            out var errorResponse))
            {
                response = new SecurityEventNotificationResponse(Request,
                                                                 Result.Format(errorResponse));
            }

            response ??= new SecurityEventNotificationResponse(Request,
                                                               Result.GenericError());


            #region Send OnSecurityEventNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSecurityEventNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SignCertificate                  (Request)

        /// <summary>
        /// Send certificate signing request to the central system.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        public async Task<SignCertificateResponse>

            SignCertificate(SignCertificateRequest  Request)

        {

            #region Send OnSignCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignCertificateRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!SignCertificateResponse.TryParse(Request,
                                                 (await WaitForResponse(requestId)) ?? new JObject(),
                                                  out var response,
                                                  out var errorResponse))
            {
                response = new SignCertificateResponse(Request,
                                                       Result.Format(errorResponse));
            }

            response ??= new SignCertificateResponse(Request,
                                                     Result.GenericError());


            #region Send OnSignCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignCertificateResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
