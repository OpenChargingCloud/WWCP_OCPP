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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_0.CS;
using cloud.charging.open.protocols.OCPPv2_0.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WebSocketRequestLogHandler(DateTime         Timestamp,
                                                    WebSocketServer  WebSocketServer,
                                                    JArray           Request);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    public delegate Task WebSocketResponseLogHandler(DateTime         Timestamp,
                                                     WebSocketServer  WebSocketServer,
                                                     JArray           Request,
                                                     JArray           Response);


    public delegate Task OnNewCSMSWSConnectionDelegate(DateTime             Timestamp,
                                                       ICSMS                CSMS,
                                                       WebSocketConnection  NewWebSocketConnection,
                                                       EventTracking_Id     EventTrackingId,
                                                       CancellationToken    CancellationToken);


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public class CSMSWSServer : WebSocketServer,
                                ICSMS
    {

        #region (enum)  SendJSONResults

        public enum SendJSONResults
        {
            Success,
            UnknownClient,
            TransmissionFailed
        }

        #endregion

        #region (class) SendRequestState

        public class SendRequestState
        {

            public DateTime                       Timestamp           { get; }
            public ChargeBox_Id                   ChargeBoxId         { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage    { get; }
            public DateTime                       Timeout             { get; }

            public JObject?                       Response            { get; set; }
            public ResultCodes?                 ErrorCode           { get; set; }
            public String?                        ErrorDescription    { get; set; }
            public JObject?                       ErrorDetails        { get; set; }

            public SendRequestState(DateTime                       Timestamp,
                                    ChargeBox_Id                   ChargeBoxId,
                                    OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                    DateTime                       Timeout,

                                    JObject?                       Response           = null,
                                    ResultCodes?                 ErrorCode          = null,
                                    String?                        ErrorDescription   = null,
                                    JObject?                       ErrorDetails       = null)
            {

                this.Timestamp         = Timestamp;
                this.ChargeBoxId       = ChargeBoxId;
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
        /// The default HTTP server name.
        /// </summary>
        public const            String                                                          DefaultHTTPServiceName      = "GraphDefined OCPP " + Version.Number + " HTTP/WebSocket/JSON Central System API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly  IPPort                                                          DefaultHTTPServerPort       = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public static readonly  HTTPPath                                                        DefaultURLPrefix            = HTTPPath.Parse("/" + Version.Number);

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly  TimeSpan                                                        DefaultRequestTimeout       = TimeSpan.FromMinutes(1);


        private readonly        Dictionary<ChargeBox_Id, Tuple<WebSocketConnection, DateTime>>  connectedChargingBoxes;

        private readonly        Dictionary<Request_Id, SendRequestState>                        requests;


        private const           String                                                          LogfileName                 = "CSMSWSServer.log";

        private const           Formatting                                                      JSONFormating               = Formatting.None;

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => HTTPServiceName;

        public IEnumerable<ChargeBox_Id> ChargeBoxIds
            => connectedChargingBoxes.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean                            RequireAuthentication    { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public Dictionary<ChargeBox_Id, String?>  ChargingBoxLogins        { get; }

        public ChargeBox_Id ChargeBoxIdentity
            => throw new NotImplementedException();

        public String From
            => "";

        //public CSMSSOAPClient.CSClientLogger Logger
        //    => throw new NotImplementedException();

        public String To
            => "";

        #endregion

        #region Events

        public event OnNewCSMSWSConnectionDelegate? OnNewCSMSWSConnection;


        // CP -> CS

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate?            OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a boot notification was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?            OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a heartbeat was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?            OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize web socket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnAuthorizeWSResponse;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a transaction event web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?           OnTransactionEventWSRequest;

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        public event OnTransactionEventRequestDelegate?    OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        public event OnTransactionEventDelegate?           OnTransactionEvent;

        /// <summary>
        /// An event sent whenever a transaction event response was sent.
        /// </summary>
        public event OnTransactionEventResponseDelegate?   OnTransactionEventResponse;

        /// <summary>
        /// An event sent whenever a transaction event web socket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?          OnTransactionEventWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?             OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?           OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?   OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?            OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?      OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate?           OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?   OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a meter values request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?     OnMeterValuesWSResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?               OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?           OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?   OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a data transfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?              OnIncomingDataTransferWSResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?           OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?   OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a firmware status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnFirmwareStatusNotificationWSResponse;

        #endregion


        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a log status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationDelegate?           OnLogStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a log status notification request was sent.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?   OnLogStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a log status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnLogStatusNotificationWSResponse;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a security event notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                    OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?    OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?           OnSecurityEventNotification;

        /// <summary>
        /// An event sent whenever a response to a security event notification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?   OnSecurityEventNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a security event notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                   OnSecurityEventNotificationWSResponse;

        #endregion

        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a sign certificate web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?          OnSignCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateRequestDelegate?    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateDelegate?           OnSignCertificate;

        /// <summary>
        /// An event sent whenever a response to a sign certificate request was sent.
        /// </summary>
        public event OnSignCertificateResponseDelegate?   OnSignCertificateResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a sign certificate request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?         OnSignCertificateWSResponse;

        #endregion



        // CS -> CP

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was sent.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now request was sent.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a get composite schedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a get composite schedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an unlock connector request was sent.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an unlock connector request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a get local list version request was sent.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a get local list version request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a send local list request was sent.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a send local list request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a clear cache request was sent.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a clear cache request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log request was sent.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #endregion

        #region Custom JSON parser/serializer delegates

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>?                  CustomBootNotificationRequestParser                    { get; set; }

        /// <summary>
        /// A delegate to parse custom Heartbeat requests.
        /// </summary>
        public CustomJObjectParserDelegate<HeartbeatRequest>?                         CustomHeartbeatRequestParser                           { get; set; }


        /// <summary>
        /// A delegate to parse custom Authorize requests.
        /// </summary>
        public CustomJObjectParserDelegate<AuthorizeRequest>?                         CustomAuthorizeRequestParser                           { get; set; }

        /// <summary>
        /// A delegate to parse custom TransactionEvent requests.
        /// </summary>
        public CustomJObjectParserDelegate<TransactionEventRequest>?                  CustomTransactionEventRequestParser                    { get; set; }

        /// <summary>
        /// A delegate to parse custom StatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<StatusNotificationRequest>?                CustomStatusNotificationRequestParser                  { get; set; }

        /// <summary>
        /// A delegate to parse custom MeterValues requests.
        /// </summary>
        public CustomJObjectParserDelegate<MeterValuesRequest>?                       CustomMeterValuesRequestParser                         { get; set; }


        /// <summary>
        /// A delegate to parse custom DataTransfer requests.
        /// </summary>
        public CustomJObjectParserDelegate<CS.DataTransferRequest>?                   CustomDataTransferRequestParser                        { get; set; }

        /// <summary>
        /// A delegate to parse custom FirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?        CustomFirmwareStatusNotificationRequestParser          { get; set; }

        /// <summary>
        /// A delegate to parse custom LogStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<LogStatusNotificationRequest>?             CustomLogStatusNotificationRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom SecurityEventNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<SecurityEventNotificationRequest>?         CustomSecurityEventNotificationRequestParser           { get; set; }

        /// <summary>
        /// A delegate to parse custom SignCertificate requests.
        /// </summary>
        public CustomJObjectParserDelegate<SignCertificateRequest>?                   CustomSignCertificateRequestParser                     { get; set; }


        public CustomJObjectSerializerDelegate<ResetRequest>?                         CustomResetRequestSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?            CustomChangeAvailabilityRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?                  CustomDataTransferRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                CustomTriggerMessageRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                CustomUpdateFirmwareRequestSerializer                  { get; set; }



        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                    CustomReserveNowRequestSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>?             CustomCancelReservationRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfile>?                      CustomChargingProfileSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                     CustomChargingScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?               CustomChargingSchedulePeriodSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?            CustomSetChargingProfileRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?          CustomClearChargingProfileRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?          CustomGetCompositeScheduleRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?               CustomUnlockConnectorRequestSerializer                 { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?           CustomGetLocalListVersionRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                 CustomSendLocalListRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                    CustomAuthorizationDataSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                          CustomIdTokenInfoResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                    CustomClearCacheRequestSerializer                      { get; set; }


        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?             CustomCertificateSignedRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?             CustomDeleteCertificateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?    CustomGetInstalledCertificateIdsRequestSerializer      { get; set; }
        public CustomJObjectSerializerDelegate<GetLogRequest>?                        CustomGetLogRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?            CustomInstallCertificateRequestSerializer              { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize a new HTTP server for the CSMS HTTP/WebSocket/JSON API.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSMSWSServer(String       HTTPServiceName              = DefaultHTTPServiceName,
                            IIPAddress?  IPAddress                    = null,
                            IPPort?      TCPPort                      = null,

                            Boolean      RequireAuthentication        = true,
                            Boolean      DisableWebSocketPings        = false,
                            TimeSpan?    WebSocketPingEvery           = null,
                            TimeSpan?    SlowNetworkSimulationDelay   = null,

                            DNSClient?   DNSClient                    = null,
                            Boolean      AutoStart                    = false)

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   HTTPServiceName,
                   null,
                   null,
                   null,

                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   DNSClient,
                   false)

        {

            this.RequireAuthentication           = RequireAuthentication;
            this.ChargingBoxLogins               = new Dictionary<ChargeBox_Id, String?>();
            this.connectedChargingBoxes          = new Dictionary<ChargeBox_Id, Tuple<WebSocketConnection, DateTime>>();
            this.requests                        = new Dictionary<Request_Id, SendRequestState>();

            base.OnValidateTCPConnection        += ValidateTCPConnection;
            base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            base.OnCloseMessageReceived         += ProcessCloseMessage;

            if (AutoStart)
                Start();

        }

        #endregion


        #region (protected) ValidateTCPConnection        (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<Boolean?> ValidateTCPConnection(DateTime                      LogTimestamp,
                                                     WebSocketServer               Server,
                                                     System.Net.Sockets.TcpClient  Connection,
                                                     EventTracking_Id              EventTrackingId,
                                                     CancellationToken             CancellationToken)
        {

            return Task.FromResult<Boolean?>(true);

        }

        #endregion

        #region (protected) ValidateWebSocketConnection  (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<HTTPResponse?> ValidateWebSocketConnection(DateTime             LogTimestamp,
                                                                WebSocketServer      Server,
                                                                WebSocketConnection  Connection,
                                                                EventTracking_Id     EventTrackingId,
                                                                CancellationToken    CancellationToken)
        {

            #region Verify 'Sec-WebSocket-Protocol'...

            var secWebSocketProtocols = Connection.Request?.SecWebSocketProtocol?.Split(',')?.Select(protocol => protocol?.Trim()).ToArray();

            if (secWebSocketProtocols is null)
            {

                DebugX.Log("Missing 'Sec-WebSocket-Protocol' HTTP header!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder(HTTPStatusCode.BadRequest) {
                               Server       = HTTPServiceName,
                               Date         = Timestamp.Now,
                               ContentType  = HTTPContentType.JSON_UTF8,
                               Content      = JSONObject.Create(
                                                  new JProperty("description",
                                                  JSONObject.Create(
                                                      new JProperty("en", "Missing 'Sec-WebSocket-Protocol' HTTP header!")
                                                  ))).ToUTF8Bytes(),
                               Connection   = "close"
                           }.AsImmutable);

            }
            else if (!secWebSocketProtocols.Contains("ocpp1.6"))
            {

                DebugX.Log("This web socket service only supports 'ocpp1.6'!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder(HTTPStatusCode.BadRequest) {
                           Server       = HTTPServiceName,
                           Date         = Timestamp.Now,
                           ContentType  = HTTPContentType.JSON_UTF8,
                           Content      = JSONObject.Create(
                                              new JProperty("description",
                                                  JSONObject.Create(
                                                      new JProperty("en", "This web socket service only supports 'ocpp1.6'!")
                                              ))).ToUTF8Bytes(),
                           Connection   = "close"
                       }.AsImmutable);

            }

            #endregion

            #region Verify HTTP Authentication

            if (RequireAuthentication)
            {

                if (Connection.Request?.Authorization is HTTPBasicAuthentication basicAuthentication)
                {

                    if (ChargingBoxLogins.TryGetValue(ChargeBox_Id.Parse(basicAuthentication.Username), out var password) &&
                        basicAuthentication.Password == password)
                    {
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " using authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder(HTTPStatusCode.Unauthorized) {
                               Server      = HTTPServiceName,
                               Date        = Timestamp.Now,
                               Connection  = "close"
                           }.AsImmutable);

            }

            #endregion

            return Task.FromResult<HTTPResponse?>(null);

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        protected Task ProcessNewWebSocketConnection(DateTime             LogTimestamp,
                                                     WebSocketServer      Server,
                                                     WebSocketConnection  Connection,
                                                     EventTracking_Id     EventTrackingId,
                                                     CancellationToken    CancellationToken)
        {

            if (!Connection.HasCustomData("chargeBoxId") &&
                Connection.Request is not null &&
                ChargeBox_Id.TryParse(Connection.Request.Path.ToString().Substring(Connection.Request.Path.ToString().LastIndexOf("/") + 1), out var chargeBoxId))
            {

                // Add the chargeBoxId to the web socket connection
                Connection.AddCustomData("chargeBoxId", chargeBoxId);

                lock (connectedChargingBoxes)
                {

                    if (!connectedChargingBoxes.ContainsKey(chargeBoxId))
                        connectedChargingBoxes.Add(chargeBoxId, new Tuple<WebSocketConnection, DateTime>(Connection, Timestamp.Now));

                    else
                    {

                        DebugX.Log(nameof(CSMSWSServer) + " Duplicate charge box '" + chargeBoxId + "' detected");

                        var oldChargingBox_WebSocketConnection = connectedChargingBoxes[chargeBoxId].Item1;

                        connectedChargingBoxes.Remove(chargeBoxId);
                        connectedChargingBoxes.Add(chargeBoxId, new Tuple<WebSocketConnection, DateTime>(Connection, Timestamp.Now));

                        try
                        {
                            oldChargingBox_WebSocketConnection.Close();
                        }
                        catch (Exception e)
                        {
                            DebugX.Log(nameof(CSMSWSServer) + " Closing old web socket connection failed: " + e.Message);
                        }

                    }

                }

            }

            #region Send OnNewCSMSWSConnection event

            var OnNewCSMSWSConnectionLocal = OnNewCSMSWSConnection;
            if (OnNewCSMSWSConnectionLocal is not null)
            {

                OnNewCSMSWSConnection?.Invoke(LogTimestamp,
                                                       this,
                                                       Connection,
                                                       EventTrackingId,
                                                       CancellationToken);

            }

            #endregion

            return Task.CompletedTask;

        }

        #endregion

        #region (protected) ProcessCloseMessage          (LogTimestamp, Server, Connection, Message, EventTrackingId)

        protected Task ProcessCloseMessage(DateTime             LogTimestamp,
                                           WebSocketServer      Server,
                                           WebSocketConnection  Connection,
                                           WebSocketFrame       Message,
                                           EventTracking_Id     EventTrackingId)
        {

            lock (connectedChargingBoxes)
            {
                if (Connection.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId", out var chargeBoxId))
                {
                    //DebugX.Log(nameof(CSMSWSServer), " Charge box " + chargeBoxId + " disconnected!");
                    connectedChargingBoxes.Remove(chargeBoxId);
                }
            }

            return Task.CompletedTask;

        }

        #endregion


        #region (protected) ProcessTextMessages          (RequestTimestamp, Connection, OCPPTextMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this web socket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The web socket connection.</param>
        /// <param name="OCPPTextMessage">The received OCPP message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime             RequestTimestamp,
                                                                                    WebSocketConnection  Connection,
                                                                                    String               OCPPTextMessage,
                                                                                    EventTracking_Id     EventTrackingId,
                                                                                    CancellationToken    CancellationToken)
        {

            if (OCPPTextMessage.Trim().IsNullOrEmpty())
            {

                DebugX.Log(nameof(CSMSWSServer) + " The given OCPP message must not be null or empty!");

                // "No response" to the charging station!
                return new WebSocketTextMessageResponse(
                           RequestTimestamp,
                           OCPPTextMessage,
                           Timestamp.Now,
                           new JArray().ToString(JSONFormating),
                           EventTrackingId
                       );

            }

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                var json = JArray.Parse(OCPPTextMessage);

                //File.AppendAllText(LogfileName,
                //                   String.Concat("Timestamp: ",        Timestamp.Now.ToIso8601(),                                                    Environment.NewLine,
                //                                 "ChargeBoxId: ",      Connection.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId")?.ToString() ?? "-",  Environment.NewLine,
                //                                 "Message received: ", JSON.ToString(Newtonsoft.Json.Formatting.Indented),                           Environment.NewLine,
                //                                 "--------------------------------------------------------------------------------------------",     Environment.NewLine));

                #region MessageType 2: CALL        (A request from a charging station)

                // [
                //     2,                  // MessageType: CALL
                //    "19223201",          // A unique request identification
                //    "BootNotification",  // The OCPP action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (json.Count             == 4                   &&
                    json[0].Type           == JTokenType.Integer  &&
                    json[0].Value<Byte>()  == 2                   &&
                    json[1].Type == JTokenType.String             &&
                    json[2].Type == JTokenType.String             &&
                    json[3].Type == JTokenType.Object)
                {

                    #region Initial checks

                    var chargeBoxId  = Connection.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId");
                    var requestId    = Request_Id.TryParse(json[1]?.Value<String>() ?? "");
                    var action       = json[2]?.Value<String>()?.Trim();
                    var requestData  = json[3]?.Value<JObject>();

                    if (!chargeBoxId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId ?? Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'charge box identity' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (!requestId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'request identification' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (action is null || action.IsNullOrEmpty())
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given 'action' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (requestData is null)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given request JSON payload must not be null!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    #endregion

                    else
                    {

                        switch (action)
                        {

                            #region BootNotification

                            case "BootNotification":
                                {

                                    #region Send OnBootNotificationWSRequest event

                                    try
                                    {

                                        OnBootNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (BootNotificationRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomBootNotificationRequestParser) && request is not null) {

                                            #region Send OnBootNotificationRequest event

                                            try
                                            {

                                                OnBootNotificationRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            BootNotificationResponse? response = null;

                                            var responseTasks = OnBootNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                this,
                                                                                                                                                request,
                                                                                                                                                CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= BootNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnBootNotificationResponse event

                                            try
                                            {

                                                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'BootNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'BootNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnBootNotificationWSResponse event

                                    try
                                    {

                                        OnBootNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             json,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region Heartbeat

                            case "Heartbeat":
                                {

                                    #region Send OnHeartbeatWSRequest event

                                    try
                                    {

                                        OnHeartbeatWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (HeartbeatRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargeBoxId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomHeartbeatRequestParser) && request is not null) {

                                            #region Send OnHeartbeatRequest event

                                            try
                                            {

                                                OnHeartbeatRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            HeartbeatResponse? response = null;

                                            var responseTasks = OnHeartbeat?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= HeartbeatResponse.Failed(request);

                                            #endregion

                                            #region Send OnHeartbeatResponse event

                                            try
                                            {

                                                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'Heartbeat' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'Heartbeat' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnHeartbeatWSResponse event

                                    try
                                    {

                                        OnHeartbeatWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      json,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region Authorize

                            case "Authorize":
                                {

                                    #region Send OnAuthorizeWSRequest event

                                    try
                                    {

                                        OnAuthorizeWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (AuthorizeRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargeBoxId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomAuthorizeRequestParser) && request is not null) {

                                            #region Send OnAuthorizeRequest event

                                            try
                                            {

                                                OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            AuthorizeResponse? response = null;

                                            var responseTasks = OnAuthorize?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= AuthorizeResponse.Failed(request);

                                            #endregion

                                            #region Send OnAuthorizeResponse event

                                            try
                                            {

                                                OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'Authorize' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'Authorize' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnAuthorizeWSResponse event

                                    try
                                    {

                                        OnAuthorizeWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      json,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region TransactionEvent

                            case "TransactionEvent":
                                {

                                    #region Send OnTransactionEventWSRequest event

                                    try
                                    {

                                        OnTransactionEventWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (TransactionEventRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomTransactionEventRequestParser) && request is not null) {

                                            #region Send OnTransactionEventRequest event

                                            try
                                            {

                                                OnTransactionEventRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            TransactionEventResponse? response = null;

                                            var responseTasks = OnTransactionEvent?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnTransactionEventDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                this,
                                                                                                                                                request,
                                                                                                                                                CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= TransactionEventResponse.Failed(request);

                                            #endregion

                                            #region Send OnTransactionEventResponse event

                                            try
                                            {

                                                OnTransactionEventResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'TransactionEvent' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'TransactionEvent' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnTransactionEventWSResponse event

                                    try
                                    {

                                        OnTransactionEventWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             json,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region StatusNotification

                            case "StatusNotification":
                                {

                                    #region Send OnStatusNotificationWSRequest event

                                    try
                                    {

                                        OnStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (StatusNotificationRequest.TryParse(requestData,
                                                                               requestId.Value,
                                                                               chargeBoxId.Value,
                                                                               out var request,
                                                                               out var errorResponse,
                                                                               CustomStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnStatusNotificationRequest event

                                            try
                                            {

                                                OnStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            StatusNotificationResponse? response = null;

                                            var responseTasks = OnStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                  this,
                                                                                                                                                  request,
                                                                                                                                                  CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= StatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnStatusNotificationResponse event

                                            try
                                            {

                                                OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request,
                                                                                     response,
                                                                                     response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'StatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'StatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnStatusNotificationWSResponse event

                                    try
                                    {

                                        OnStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               json,
                                                                               OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region MeterValues

                            case "MeterValues":
                                {

                                    #region Send OnMeterValuesWSRequest event

                                    try
                                    {

                                        OnMeterValuesWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (MeterValuesRequest.TryParse(requestData,
                                                                        requestId.Value,
                                                                        chargeBoxId.Value,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomMeterValuesRequestParser) && request is not null) {

                                            #region Send OnMeterValuesRequest event

                                            try
                                            {

                                                OnMeterValuesRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            MeterValuesResponse? response = null;

                                            var responseTasks = OnMeterValues?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                           this,
                                                                                                                                           request,
                                                                                                                                           CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= MeterValuesResponse.Failed(request);

                                            #endregion

                                            #region Send OnMeterValuesResponse event

                                            try
                                            {

                                                OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'MeterValues' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'MeterValues' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",     OCPPTextMessage),
                                                                    new JProperty("exception",   e.Message),
                                                                    new JProperty("stacktrace",  e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnMeterValuesWSResponse event

                                    try
                                    {

                                        OnMeterValuesWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        json,
                                                                        OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region DataTransfer

                            case "DataTransfer":
                                {

                                    #region Send OnIncomingDataTransferWSRequest event

                                    try
                                    {

                                        OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (CS.DataTransferRequest.TryParse(requestData,
                                                                            requestId.Value,
                                                                            chargeBoxId.Value,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomDataTransferRequestParser) && request is not null) {

                                            #region Send OnIncomingDataTransferRequest event

                                            try
                                            {

                                                OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            DataTransferResponse? response = null;

                                            var responseTasks = OnIncomingDataTransfer?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                    this,
                                                                                                                                                    request,
                                                                                                                                                    CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
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
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'IncomingDataTransfer' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'IncomingDataTransfer' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnIncomingDataTransferWSResponse event

                                    try
                                    {

                                        OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json,
                                                                                 OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region FirmwareStatusNotification

                            case "FirmwareStatusNotification":
                                {

                                    #region Send OnFirmwareStatusNotificationWSRequest event

                                    try
                                    {

                                        OnFirmwareStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (FirmwareStatusNotificationRequest.TryParse(requestData,
                                                                                       requestId.Value,
                                                                                       chargeBoxId.Value,
                                                                                       out var request,
                                                                                       out var errorResponse,
                                                                                       CustomFirmwareStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnFirmwareStatusNotificationRequest event

                                            try
                                            {

                                                OnFirmwareStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            FirmwareStatusNotificationResponse? response = null;

                                            var responseTasks = OnFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                          this,
                                                                                                                                                          request,
                                                                                                                                                          CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= FirmwareStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnFirmwareStatusNotificationResponse event

                                            try
                                            {

                                                OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             request,
                                                                                             response,
                                                                                             response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'FirmwareStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'FirmwareStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnFirmwareStatusNotificationWSResponse event

                                    try
                                    {

                                        OnFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       json,
                                                                                       OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region LogStatusNotification

                            case "LogStatusNotification":
                                {

                                    #region Send OnLogStatusNotificationWSRequest event

                                    try
                                    {

                                        OnLogStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (LogStatusNotificationRequest.TryParse(requestData,
                                                                                  requestId.Value,
                                                                                  chargeBoxId.Value,
                                                                                  out var request,
                                                                                  out var errorResponse,
                                                                                  CustomLogStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnLogStatusNotificationRequest event

                                            try
                                            {

                                                OnLogStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            LogStatusNotificationResponse? response = null;

                                            var responseTasks = OnLogStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnLogStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                     this,
                                                                                                                                                     request,
                                                                                                                                                     CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= LogStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnLogStatusNotificationResponse event

                                            try
                                            {

                                                OnLogStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'LogStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'LogStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnLogStatusNotificationWSResponse event

                                    try
                                    {

                                        OnLogStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json,
                                                                                  OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region SecurityEventNotification

                            case "SecurityEventNotification":
                                {

                                    #region Send OnSecurityEventNotificationWSRequest event

                                    try
                                    {

                                        OnSecurityEventNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (SecurityEventNotificationRequest.TryParse(requestData,
                                                                                      requestId.Value,
                                                                                      chargeBoxId.Value,
                                                                                      out var request,
                                                                                      out var errorResponse,
                                                                                      CustomSecurityEventNotificationRequestParser) && request is not null) {

                                            #region Send OnSecurityEventNotificationRequest event

                                            try
                                            {

                                                OnSecurityEventNotificationRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            SecurityEventNotificationResponse? response = null;

                                            var responseTasks = OnSecurityEventNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnSecurityEventNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                         this,
                                                                                                                                                         request,
                                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= SecurityEventNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnSecurityEventNotificationResponse event

                                            try
                                            {

                                                OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'SecurityEventNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'SecurityEventNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnSecurityEventNotificationWSResponse event

                                    try
                                    {

                                        OnSecurityEventNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json,
                                                                            OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region SignCertificate

                            case "SignCertificate":
                                {

                                    #region Send OnSignCertificateWSRequest event

                                    try
                                    {

                                        OnSignCertificateWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (SignCertificateRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomSignCertificateRequestParser) && request is not null) {

                                            #region Send OnSignCertificateRequest event

                                            try
                                            {

                                                OnSignCertificateRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            SignCertificateResponse? response = null;

                                            var responseTasks = OnSignCertificate?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnSignCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                               this,
                                                                                                                                               request,
                                                                                                                                               CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= SignCertificateResponse.Failed(request);

                                            #endregion

                                            #region Send OnSignCertificateResponse event

                                            try
                                            {

                                                OnSignCertificateResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'SignCertificate' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'SignCertificate' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnSignCertificateWSResponse event

                                    try
                                    {

                                        OnSignCertificateWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json,
                                                                            OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            default:

                                // It does not make much sense to send this error to a charging station as no one will read it there!
                                DebugX.Log(nameof(CSMSWSServer) + " The OCPP message action '" + action + "' is unkown!");

                                //OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                //                   requestId.Value,
                                //                   new JObject()
                                //               );

                                break;

                        }

                    }

                }

                #endregion

                #region MessageType 3: CALLRESULT  (A response from charging station)

                // [
                //     3,                         // MessageType: CALLRESULT
                //    "19223201",                 // The request identification copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                else if (json.Count             == 3         &&
                         json[0].Type == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 3         &&
                         json[1].Type == JTokenType.String   &&
                         json[2].Type == JTokenType.Object)
                {

                    lock (requests)
                    {
                        if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                            requests.TryGetValue(requestId, out var request))
                        {
                            request.Response = json[2] as JObject;
                        }
                    }

                    // No response to the charging station!

                }

                #endregion

                #region MessageType 4: CALLERROR   (A charging station reports an error on a received request)

                // [
                //     4,                         // MessageType: CALLERROR
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                // Error Code                    Description
                // -----------------------------------------------------------------------------------------------
                // NotImplemented                Requested Action is not known by receiver
                // NotSupported                  Requested Action is recognized but not supported by the receiver
                // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
                // ProtocolError                 Payload for Action is incomplete
                // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
                // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
                // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
                // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
                // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
                // GenericError                  Any other error not covered by the previous ones

                else if (json.Count             == 5                   &&
                         json[0].Type           == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 4                   &&
                         json[1].Type == JTokenType.String             &&
                         json[2].Type == JTokenType.String             &&
                         json[3].Type == JTokenType.String             &&
                         json[4].Type == JTokenType.Object)
                {

                    lock (requests)
                    {
                        if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                            requests.TryGetValue(requestId, out var request))
                        {

                            // ToDo: Refactor 
                            if (ResultCodes.TryParse(json[2]?.Value<String>() ?? "", out var errorCode))
                                request.ErrorCode = errorCode;
                            else
                                request.ErrorCode = ResultCodes.GenericError;

                            request.Response          = null;
                            request.ErrorDescription  = json[3]?.Value<String>();
                            request.ErrorDetails      = json[4] as JObject;

                        }
                    }

                    // No response to the charging station!

                }

                #endregion

                else
                {

                    // It does not make much sense to send this error to a charging station as no one will read it there!
                    DebugX.Log(nameof(CSMSWSServer) + " The OCPP message '" + OCPPTextMessage + "' is invalid!");

                    //new WSErrorMessage(Request_Id.Parse(JSON.Count >= 2 ? JSON[1]?.Value<String>()?.Trim() : "unknown"),
                    //                                  WSErrorCodes.FormationViolation,
                    //                                  "The given OCPP request message is invalid!",
                    //                                  new JObject(
                    //                                      new JProperty("request", TextMessage)
                    //                                 ));

                    //// No response to the charging station!
                    //return null;

                }

            }
            catch (Exception e)
            {

                // It does not make much sense to send this error to a charging station as no one will read it there!
                DebugX.LogException(e, "The OCPP message '" + OCPPTextMessage + "' received in " + nameof(CSMSWSServer) + " led to an exception!");

                //ErrorMessage = new WSErrorMessage(Request_Id.Parse(JSON != null && JSON.Count >= 2
                //                                                       ? JSON?[1].Value<String>()?.Trim()
                //                                                       : "Unknown request identification"),
                //                                  WSErrorCodes.FormationViolation,
                //                                  "Processing the given OCPP request message led to an exception!",
                //                                  new JObject(
                //                                      new JProperty("request",     TextMessage),
                //                                      new JProperty("exception",   e.Message),
                //                                      new JProperty("stacktrace",  e.StackTrace)
                //                                  ));

            }

            // The response to the charging station...
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       OCPPTextMessage,
                       Timestamp.Now,
                       (OCPPResponse?.     ToJSON() ??
                        OCPPErrorResponse?.ToJSON())?.ToString(JSONFormating)
                           ?? String.Empty,
                       EventTrackingId
                   );

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
            lock (ChargingBoxLogins)
            {

                if (ChargingBoxLogins.ContainsKey(ChargeBoxId))
                    ChargingBoxLogins.Remove(ChargeBoxId);

                ChargingBoxLogins.Add(ChargeBoxId,
                                      Password);

            }
        }

        #endregion


        #region SendRequest(RequestId, ChargeBoxId, OCPPAction, JSONPayload, Timeout = null)

        public async Task<SendRequestState> SendRequest(Request_Id    RequestId,
                                                        ChargeBox_Id  ChargeBoxId,
                                                        String        OCPPAction,
                                                        JObject       JSONPayload,
                                                        TimeSpan?     Timeout   = null)
        {

            var endTime         = Timestamp.Now + (Timeout ?? TimeSpan.FromSeconds(30));

            var sendJSONResult  = await SendJSON(RequestId,
                                                 ChargeBoxId,
                                                 OCPPAction,
                                                 JSONPayload,
                                                 endTime);

            if (sendJSONResult == SendJSONResults.Success) {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25);

                        if (requests.TryGetValue(RequestId, out var sendRequestState) &&
                            sendRequestState?.Response is not null ||
                            sendRequestState?.ErrorCode.HasValue == true)
                        {

                            lock (requests)
                            {
                                requests.Remove(RequestId);
                            }

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(CSMSWSServer), ".", nameof(SendRequest), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                lock (requests)
                {
                    if (requests.TryGetValue(RequestId, out var sendRequestState) && sendRequestState is not null)
                    {
                        sendRequestState.ErrorCode = ResultCodes.Timeout;
                        requests.Remove(RequestId);
                        return sendRequestState;
                    }
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                lock (requests)
                {
                    if (requests.TryGetValue(RequestId, out var sendRequestState) && sendRequestState is not null)
                    {
                        sendRequestState.ErrorCode = ResultCodes.Timeout;
                        requests.Remove(RequestId);
                        return sendRequestState;
                    }
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return new SendRequestState(
                       now,
                       ChargeBoxId,
                       new OCPP_WebSocket_RequestMessage(
                           RequestId,
                           OCPPAction,
                           JSONPayload
                       ),
                       now,
                       new JObject(),
                       ResultCodes.InternalError
                   );

        }

        #endregion

        #region SendJSON   (RequestId, ChargeBoxId, Action, JSON, RequestTimeout)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="ChargeBoxId">A charge box identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="JSON">The JSON payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public Task<SendJSONResults> SendJSON(Request_Id    RequestId,
                                              ChargeBox_Id  ChargeBoxId,
                                              String        Action,
                                              JObject       JSON,
                                              DateTime      RequestTimeout)
        {

            var wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                       RequestId,
                                       Action,
                                       JSON
                                   );

            try
            {

                var webSocketConnections  = WebSocketConnections.Where  (ws => ws.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId") == ChargeBoxId).
                                                                 ToArray();

                if (webSocketConnections.Any())
                {

                    requests.Add(RequestId,
                                 new SendRequestState(
                                     Timestamp.Now,
                                     ChargeBoxId,
                                     wsRequestMessage,
                                     RequestTimeout
                                 ));

                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        var success = SendFrame(webSocketConnection,
                                                new WebSocketFrame(
                                                    WebSocketFrame.Fin.Final,
                                                    WebSocketFrame.MaskStatus.Off,
                                                    new Byte[4],
                                                    WebSocketFrame.Opcodes.Text,
                                                    wsRequestMessage.ToJSON().ToString(Formatting.None).ToUTF8Bytes(),
                                                    WebSocketFrame.Rsv.Off,
                                                    WebSocketFrame.Rsv.Off,
                                                    WebSocketFrame.Rsv.Off
                                                ));

                        if (success == SendStatus.Success)
                            break;

                        else
                            RemoveConnection(webSocketConnection);

                    }

                    return Task.FromResult(SendJSONResults.Success);

                }
                else
                    return Task.FromResult(SendJSONResults.UnknownClient);

            }
            catch (Exception)
            {
                return Task.FromResult(SendJSONResults.TransmissionFailed);
            }

        }

        #endregion


        #region Reset                 (Request)

        public async Task<ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetRequest));
            }

            #endregion


            ResetResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomResetRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (ResetResponse.TryParse(Request,
                                           sendRequestState.Response,
                                           out var resetResponse,
                                           out var errorResponse))
                {
                    response = resetResponse!;
                }

                else
                    response = new ResetResponse(Request,
                                                 Result.Format(errorResponse));

            }
            else
                response = new ResetResponse(Request,
                                             Result.FromSendRequestState(sendRequestState));


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability    (Request)

        public async Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        {

            #region Send OnChangeAvailabilityRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            ChangeAvailabilityResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomChangeAvailabilityRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var changeAvailabilityResponse,
                                                        out var errorResponse))
                {
                    response = changeAvailabilityResponse!;
                }

                else
                    response = new ChangeAvailabilityResponse(Request,
                                                              Result.Format(errorResponse));

            }
            else
                response = new ChangeAvailabilityResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DataTransfer          (Request)

        public async Task<CS.DataTransferResponse> DataTransfer(DataTransferRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CS.DataTransferResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomDataTransferRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (CS.DataTransferResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var dataTransferResponse,
                                                     out var errorResponse))
                {
                    response = dataTransferResponse!;
                }

                else
                    response = new CS.DataTransferResponse(Request,
                                                           Result.Format(errorResponse));

            }
            else
                response = new CS.DataTransferResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage        (Request)

        public async Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        {

            #region Send OnTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            TriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomTriggerMessageRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (TriggerMessageResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var triggerMessageResponse,
                                                    out var errorResponse))
                {
                    response = triggerMessageResponse!;
                }

                else
                    response = new TriggerMessageResponse(Request,
                                                          Result.Format(errorResponse));

            }
            else
                response = new TriggerMessageResponse(Request,
                                                      Result.FromSendRequestState(sendRequestState));


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware        (Request)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomUpdateFirmwareRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var updateFirmwareResponse,
                                                    out var errorResponse))
                {
                    response = updateFirmwareResponse!;
                }

                else
                    response = new UpdateFirmwareResponse(Request,
                                                          Result.Format(errorResponse));

            }
            else
                response = new UpdateFirmwareResponse(Request,
                                                      Result.FromSendRequestState(sendRequestState));


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow            (Request)

        public async Task<ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        {

            #region Send OnReserveNowRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            ReserveNowResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomReserveNowRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (ReserveNowResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var reserveNowResponse,
                                                    out var errorResponse))
                {
                    response = reserveNowResponse!;
                }

                else
                    response = new ReserveNowResponse(Request,
                                                      Result.Format(errorResponse));

            }
            else
                response = new ReserveNowResponse(Request,
                                                  Result.FromSendRequestState(sendRequestState));


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation     (Request)

        public async Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        {

            #region Send OnCancelReservationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            CancelReservationResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomCancelReservationRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (CancelReservationResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var cancelReservationResponse,
                                                       out var errorResponse))
                {
                    response = cancelReservationResponse!;
                }

                else
                    response = new CancelReservationResponse(Request,
                                                             Result.Format(errorResponse));

            }
            else
                response = new CancelReservationResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile    (Request)

        public async Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            SetChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomSetChargingProfileRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (SetChargingProfileResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var setChargingProfileResponse,
                                                        out var errorResponse))
                {
                    response = setChargingProfileResponse!;
                }

                else
                    response = new SetChargingProfileResponse(Request,
                                                              Result.Format(errorResponse));

            }
            else
                response = new SetChargingProfileResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile  (Request)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomClearChargingProfileRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse))
                {
                    response = clearChargingProfileResponse!;
                }

                else
                    response = new ClearChargingProfileResponse(Request,
                                                                Result.Format(errorResponse));

            }
            else
                response = new ClearChargingProfileResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule  (Request)


        public async Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        {

            #region Send OnGetCompositeScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            GetCompositeScheduleResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetCompositeScheduleRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (GetCompositeScheduleResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var getCompositeScheduleResponse,
                                                          out var errorResponse))
                {
                    response = getCompositeScheduleResponse!;
                }

                else
                    response = new GetCompositeScheduleResponse(Request,
                                                                Result.Format(errorResponse));

            }
            else
                response = new GetCompositeScheduleResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector       (Request)

        public async Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        {

            #region Send OnUnlockConnectorRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequest?.Invoke(startTime,
                                                 this,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            UnlockConnectorResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomUnlockConnectorRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (UnlockConnectorResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var unlockConnectorResponse,
                                                     out var errorResponse))
                {
                    response = unlockConnectorResponse!;
                }

                else
                    response = new UnlockConnectorResponse(Request,
                                                           Result.Format(errorResponse));

            }
            else
                response = new UnlockConnectorResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion   (Request)

        public async Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        {

            #region Send OnGetLocalListVersionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            GetLocalListVersionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetLocalListVersionRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (GetLocalListVersionResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var getLocalListVersionResponse,
                                                         out var errorResponse))
                {
                    response = getLocalListVersionResponse!;
                }

                else
                    response = new GetLocalListVersionResponse(Request,
                                                               Result.Format(errorResponse));

            }
            else
                response = new GetLocalListVersionResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList         (Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            SendLocalListResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomSendLocalListRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (SendLocalListResponse.TryParse(Request,
                                                   sendRequestState.Response,
                                                   out var sendLocalListResponse,
                                                   out var errorResponse))
                {
                    response = sendLocalListResponse!;
                }

                else
                    response = new SendLocalListResponse(Request,
                                                         Result.Format(errorResponse));

            }
            else
                response = new SendLocalListResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache            (Request)

        public async Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            ClearCacheResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomClearCacheRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (ClearCacheResponse.TryParse(Request,
                                                sendRequestState.Response,
                                                out var clearCacheResponse,
                                                out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new ClearCacheResponse(Request,
                                                      Result.Format(errorResponse));

            }
            else
                response = new ClearCacheResponse(Request,
                                                  Result.FromSendRequestState(sendRequestState));


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region CertificateSigned         (Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        public async Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            CertificateSignedResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomCertificateSignedRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (CertificateSignedResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var clearCacheResponse,
                                                       out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new CertificateSignedResponse(Request,
                                                             Result.Format(errorResponse));

            }
            else
                response = new CertificateSignedResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate         (Request)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        public async Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {

            #region Send OnDeleteCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            DeleteCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomDeleteCertificateRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (DeleteCertificateResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var clearCacheResponse,
                                                       out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new DeleteCertificateResponse(Request,
                                                             Result.Format(errorResponse));

            }
            else
                response = new DeleteCertificateResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


            #region Send OnDeleteCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds(Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        public async Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {

            #region Send OnGetInstalledCertificateIdsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
                                                            this,
                                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            GetInstalledCertificateIdsResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetInstalledCertificateIdsRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (GetInstalledCertificateIdsResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var clearCacheResponse,
                                                                out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new GetInstalledCertificateIdsResponse(Request,
                                                                      Result.Format(errorResponse));

            }
            else
                response = new GetInstalledCertificateIdsResponse(Request,
                                                                  Result.FromSendRequestState(sendRequestState));


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                    (Request)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        public async Task<GetLogResponse> GetLog(GetLogRequest Request)
        {

            #region Send OnGetLogRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLogRequest?.Invoke(startTime,
                                        this,
                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            GetLogResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetLogRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (GetLogResponse.TryParse(Request,
                                            sendRequestState.Response,
                                            out var clearCacheResponse,
                                            out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new GetLogResponse(Request,
                                                  Result.Format(errorResponse));

            }
            else
                response = new GetLogResponse(Request,
                                              Result.FromSendRequestState(sendRequestState));


            #region Send OnGetLogResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLogResponse?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate        (Request)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        public async Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {

            #region Send OnInstallCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnInstallCertificateRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            InstallCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomInstallCertificateRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.Response is not null)
            {

                if (InstallCertificateResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var clearCacheResponse,
                                                        out var errorResponse))
                {
                    response = clearCacheResponse!;
                }

                else
                    response = new InstallCertificateResponse(Request,
                                                              Result.Format(errorResponse));

            }
            else
                response = new InstallCertificateResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
