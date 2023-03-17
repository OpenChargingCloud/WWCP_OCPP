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
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
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

        #region Outgoing messages to the CSMS

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnBootNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                         OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                        OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?     OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                               OnPublishFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?    OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnHeartbeatWSResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        #endregion

        #region OnNotifyEventRequest/-Response

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEventRequestDelegate?     OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?          OnNotifyEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnNotifyEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event OnNotifyEventResponseDelegate?    OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?     OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnSecurityEventNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?    OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyReportRequestDelegate?     OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnNotifyReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnNotifyReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?    OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?     OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnNotifyMonitoringReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?    OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a log status notification request will be sent to the CSMS.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?     OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a log status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnLogStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?    OnLogStatusNotificationResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestDelegate?     OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?    OnDataTransferResponse;

        #endregion


        #region OnSignCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event OnSignCertificateRequestDelegate?     OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?              OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnSignCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?    OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate request will be sent to the CSMS.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?     OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnGet15118EVCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?    OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatusRequest/-Response

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?     OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnGetCertificateStatusWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?    OnGetCertificateStatusResponse;

        #endregion


        #region OnReservationStatusUpdateRequest/-Response

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                      OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event ClientResponseLogHandler?                     OnReservationStatusUpdateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeedsRequest/-Response

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?     OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnNotifyEVChargingNeedsWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?    OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEventRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnTransactionEventRequestDelegate?     OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnTransactionEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnTransactionEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnTransactionEventResponseDelegate?    OnTransactionEventResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?     OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?    OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the CSMS.
        /// </summary>
        public event OnMeterValuesRequestDelegate?     OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?          OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnMeterValuesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?    OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?     OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                  OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                 OnNotifyChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?    OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?     OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnClearedChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?    OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfilesRequest/-Response

        /// <summary>
        /// An event fired whenever a report charging profiles request will be sent to the CSMS.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?     OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a report charging profiles request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a report charging profiles request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnReportChargingProfilesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a report charging profiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?    OnReportChargingProfilesResponse;

        #endregion


        #region OnNotifyDisplayMessagesRequest/-Response

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?     OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnNotifyDisplayMessagesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?    OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformationRequest/-Response

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?     OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnNotifyCustomerInformationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?    OnNotifyCustomerInformationResponse;

        #endregion

        #endregion

        #region Incoming messages from the CSMS

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

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?            OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an update firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnUpdateFirmwareWSResponse;

        #endregion

        #region OnPublishFirmware

        /// <summary>
        /// An event sent whenever a publish firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnPublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?     OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        public event OnPublishFirmwareDelegate?            OnPublishFirmware;

        /// <summary>
        /// An event sent whenever a response to a publish firmware request was sent.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?    OnPublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a publish firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnPublishFirmwareWSResponse;

        #endregion

        #region OnUnpublishFirmware

        /// <summary>
        /// An event sent whenever an unpublish firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnUnpublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?     OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareDelegate?            OnUnpublishFirmware;

        /// <summary>
        /// An event sent whenever a response to an unpublish firmware request was sent.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?    OnUnpublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an unpublish firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnUnpublishFirmwareWSResponse;

        #endregion

        #region OnGetBaseReport

        /// <summary>
        /// An event sent whenever a get base report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnGetBaseReportWSRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportDelegate?            OnGetBaseReport;

        /// <summary>
        /// An event sent whenever a response to a get base report request was sent.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get base report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnGetBaseReportWSResponse;

        #endregion

        #region OnGetReport

        /// <summary>
        /// An event sent whenever a get report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?      OnGetReportWSRequest;

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        public event OnGetReportRequestDelegate?     OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        public event OnGetReportDelegate?            OnGetReport;

        /// <summary>
        /// An event sent whenever a response to a get report request was sent.
        /// </summary>
        public event OnGetReportResponseDelegate?    OnGetReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?     OnGetReportWSResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnGetLogWSRequest;

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        public event OnGetLogRequestDelegate?       OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        public event OnGetLogDelegate?              OnGetLog;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        public event OnGetLogResponseDelegate?      OnGetLogResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get log request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnGetLogWSResponse;

        #endregion

        #region OnSetVariables

        /// <summary>
        /// An event sent whenever a set variables websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnSetVariablesWSRequest;

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        public event OnSetVariablesRequestDelegate?     OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        public event OnSetVariablesDelegate?            OnSetVariables;

        /// <summary>
        /// An event sent whenever a response to a set variables request was sent.
        /// </summary>
        public event OnSetVariablesResponseDelegate?    OnSetVariablesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variables request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnSetVariablesWSResponse;

        #endregion

        #region OnGetVariables

        /// <summary>
        /// An event sent whenever a get variables websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnGetVariablesWSRequest;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OnGetVariablesRequestDelegate?     OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OnGetVariablesDelegate?            OnGetVariables;

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        public event OnGetVariablesResponseDelegate?    OnGetVariablesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get variables request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnGetVariablesWSResponse;

        #endregion

        #region OnSetMonitoringBase

        /// <summary>
        /// An event sent whenever a set monitoring base websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetMonitoringBaseWSRequest;

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?     OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        public event OnSetMonitoringBaseDelegate?            OnSetMonitoringBase;

        /// <summary>
        /// An event sent whenever a response to a set monitoring base request was sent.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?    OnSetMonitoringBaseResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set monitoring base request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetMonitoringBaseWSResponse;

        #endregion

        #region OnGetMonitoringReport

        /// <summary>
        /// An event sent whenever a get monitoring report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetMonitoringReportWSRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?     OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportDelegate?            OnGetMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a get monitoring report request was sent.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?    OnGetMonitoringReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get monitoring report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetMonitoringReportWSResponse;

        #endregion

        #region OnSetMonitoringLevel

        /// <summary>
        /// An event sent whenever a set monitoring level websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnSetMonitoringLevelWSRequest;

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?     OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        public event OnSetMonitoringLevelDelegate?            OnSetMonitoringLevel;

        /// <summary>
        /// An event sent whenever a response to a set monitoring level request was sent.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?    OnSetMonitoringLevelResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set monitoring level request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnSetMonitoringLevelWSResponse;

        #endregion

        #region OnSetVariableMonitoring

        /// <summary>
        /// An event sent whenever a set variable monitoring websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnSetVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?     OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringDelegate?            OnSetVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?    OnSetVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variable monitoring request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnSetVariableMonitoringWSResponse;

        #endregion

        #region OnClearVariableMonitoring

        /// <summary>
        /// An event sent whenever a clear variable monitoring websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                    OnClearVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?     OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringDelegate?            OnClearVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a clear variable monitoring request was sent.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?    OnClearVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear variable monitoring request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                   OnClearVariableMonitoringWSResponse;

        #endregion

        #region OnSetNetworkProfile

        /// <summary>
        /// An event sent whenever a set network profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetNetworkProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?     OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OnSetNetworkProfileDelegate?            OnSetNetworkProfile;

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?    OnSetNetworkProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set network profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetNetworkProfileWSResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate?            OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a change availability request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnChangeAvailabilityWSResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageDelegate?            OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a trigger message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnTriggerMessageWSResponse;

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


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnCertificateSignedWSRequest;

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?     OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        public event OnCertificateSignedDelegate?            OnCertificateSigned;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?    OnCertificateSignedResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a certificate signed request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnCertificateSignedWSResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnInstallCertificateWSRequest;

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?        OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        public event OnInstallCertificateDelegate?               OnInstallCertificate;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?       OnInstallCertificateResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an install certificate request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnInstallCertificateWSResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                       OnGetInstalledCertificateIdsWSRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsDelegate?            OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?    OnGetInstalledCertificateIdsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get installed certificate ids request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                      OnGetInstalledCertificateIdsWSResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnDeleteCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?     OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateDelegate?            OnDeleteCertificate;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?    OnDeleteCertificateResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a delete certificate request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnDeleteCertificateWSResponse;

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

        #region OnRequestStartTransaction

        /// <summary>
        /// An event sent whenever a request start transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                    OnRequestStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OnRequestStartTransactionDelegate?            OnRequestStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a request start transaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request start transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                   OnRequestStartTransactionWSResponse;

        #endregion

        #region OnRequestStopTransaction

        /// <summary>
        /// An event sent whenever a request stop transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                   OnRequestStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?     OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionDelegate?            OnRequestStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a request stop transaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?    OnRequestStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request stop transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                  OnRequestStopTransactionWSResponse;

        #endregion

        #region OnGetTransactionStatus

        /// <summary>
        /// An event sent whenever a get transaction status websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnGetTransactionStatusWSRequest;

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?     OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        public event OnGetTransactionStatusDelegate?            OnGetTransactionStatus;

        /// <summary>
        /// An event sent whenever a response to a get transaction status request was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?    OnGetTransactionStatusResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get transaction status request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnGetTransactionStatusWSResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate?            OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set charging profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnSetChargingProfileWSResponse;

        #endregion

        #region OnGetChargingProfiles

        /// <summary>
        /// An event sent whenever a get charging profiles websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?     OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event OnGetChargingProfilesDelegate?            OnGetChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a get charging profiles request was sent.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?    OnGetChargingProfilesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get charging profiles request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetChargingProfilesWSResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnClearChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate?            OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear charging profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnClearChargingProfileWSResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a get composite schedule websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a get composite schedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?     OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a get composite schedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?            OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a get composite schedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?    OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get composite schedule request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnGetCompositeScheduleWSResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an unlock connector websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnUnlockConnectorWSRequest;

        /// <summary>
        /// An event sent whenever an unlock connector request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?     OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever an unlock connector request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate?            OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to an unlock connector request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?    OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an unlock connector request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnUnlockConnectorWSResponse;

        #endregion


        #region OnSetDisplayMessage

        /// <summary>
        /// An event sent whenever a set display message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?     OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageDelegate?            OnSetDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?    OnSetDisplayMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set display message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetDisplayMessageWSResponse;

        #endregion

        #region OnGetDisplayMessages

        /// <summary>
        /// An event sent whenever a get display messages websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnGetDisplayMessagesWSRequest;

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?     OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        public event OnGetDisplayMessagesDelegate?            OnGetDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a get display messages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?    OnGetDisplayMessagesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get display messages request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnGetDisplayMessagesWSResponse;

        #endregion

        #region OnClearDisplayMessage

        /// <summary>
        /// An event sent whenever a clear display message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnClearDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?     OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        public event OnClearDisplayMessageDelegate?            OnClearDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a clear display message request was sent.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?    OnClearDisplayMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear display message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnClearDisplayMessageWSResponse;

        #endregion

        #region OnCostUpdated

        /// <summary>
        /// An event sent whenever a cost updated websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?        OnCostUpdatedWSRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?     OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedDelegate?            OnCostUpdated;

        /// <summary>
        /// An event sent whenever a response to a cost updated request was sent.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?    OnCostUpdatedResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cost updated request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?       OnCostUpdatedWSResponse;

        #endregion

        #region OnCustomerInformation

        /// <summary>
        /// An event sent whenever a customer information websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnCustomerInformationWSRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?     OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationDelegate?            OnCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a customer information request was sent.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?    OnCustomerInformationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a customer information request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnCustomerInformationWSResponse;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station websocket client running on a charging station
        /// and connecting to a CSMS to invoke methods.
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

                        DebugX.Log("Invalid WebSocket connection!");

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



        #region SendBootNotification                 (Request)

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

        #region SendFirmwareStatusNotification       (Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request)

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

        #region SendPublishFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A publish firmware status notification request.</param>
        public async Task<PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request)

        {

            #region Send OnPublishFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                   this,
                                                                   Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!PublishFirmwareStatusNotificationResponse.TryParse(Request,
                                                                   (await WaitForResponse(requestId)) ?? new JObject(),
                                                                    out var response,
                                                                    out var errorResponse))
            {
                response = new PublishFirmwareStatusNotificationResponse(Request,
                                                                         Result.Format(errorResponse));
            }

            response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                       Result.GenericError());


            #region Send OnPublishFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                    this,
                                                                    Request,
                                                                    response,
                                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                        (Request)

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

        #region NotifyEvent                          (Request)

        /// <summary>
        /// Send a notify event.
        /// </summary>
        /// <param name="Request">A notify event request.</param>
        public async Task<NotifyEventResponse>

            NotifyEvent(NotifyEventRequest  Request)

        {

            #region Send OnNotifyEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEventRequest?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyEventResponse.TryParse(Request,
                                             (await WaitForResponse(requestId)) ?? new JObject(),
                                              out var response,
                                              out var errorResponse))
            {
                response = new NotifyEventResponse(Request,
                                                   Result.Format(errorResponse));
            }

            response ??= new NotifyEventResponse(Request,
                                                 Result.GenericError());


            #region Send OnNotifyEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEventResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendSecurityEventNotification        (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
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

        #region NotifyReport                         (Request)

        /// <summary>
        /// Send a notify report.
        /// </summary>
        /// <param name="Request">A notify report request.</param>
        public async Task<NotifyReportResponse>

            NotifyReport(NotifyReportRequest  Request)

        {

            #region Send OnNotifyReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyReportRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyReportResponse.TryParse(Request,
                                              (await WaitForResponse(requestId)) ?? new JObject(),
                                               out var response,
                                               out var errorResponse))
            {
                response = new NotifyReportResponse(Request,
                                                    Result.Format(errorResponse));
            }

            response ??= new NotifyReportResponse(Request,
                                                  Result.GenericError());


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyMonitoringReport               (Request)

        /// <summary>
        /// Send a notify monitoring report.
        /// </summary>
        /// <param name="Request">A notify monitoring report request.</param>
        public async Task<NotifyMonitoringReportResponse>

            NotifyMonitoringReport(NotifyMonitoringReportRequest  Request)

        {

            #region Send OnNotifyMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyMonitoringReportResponse.TryParse(Request,
                                                        (await WaitForResponse(requestId)) ?? new JObject(),
                                                         out var response,
                                                         out var errorResponse))
            {
                response = new NotifyMonitoringReportResponse(Request,
                                                              Result.Format(errorResponse));
            }

            response ??= new NotifyMonitoringReportResponse(Request,
                                                            Result.GenericError());


            #region Send OnNotifyMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region LogStatusNotification                (Request)

        /// <summary>
        /// Send a log status notification to the CSMS.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
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

        #region TransferData                         (Request)

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
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


        #region SignCertificate                      (Request)

        /// <summary>
        /// Send a sign certificate request to the CSMS.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
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

        #region Get15118EVCertificate                (Request)

        /// <summary>
        /// Send a get 15118 EV certificate.
        /// </summary>
        /// <param name="Request">A get 15118 EV certificate request.</param>
        public async Task<Get15118EVCertificateResponse>

            Get15118EVCertificate(Get15118EVCertificateRequest  Request)

        {

            #region Send OnGet15118EVCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGet15118EVCertificateRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!Get15118EVCertificateResponse.TryParse(Request,
                                                       (await WaitForResponse(requestId)) ?? new JObject(),
                                                        out var response,
                                                        out var errorResponse))
            {
                response = new Get15118EVCertificateResponse(Request,
                                                             Result.Format(errorResponse));
            }

            response ??= new Get15118EVCertificateResponse(Request,
                                                           Result.GenericError());


            #region Send OnGet15118EVCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGet15118EVCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCertificateStatus                 (Request)

        /// <summary>
        /// Send a get certificate status request.
        /// </summary>
        /// <param name="Request">A get certificate status request.</param>
        public async Task<GetCertificateStatusResponse>

            GetCertificateStatus(GetCertificateStatusRequest  Request)

        {

            #region Send OnGetCertificateStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCertificateStatusRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!GetCertificateStatusResponse.TryParse(Request,
                                                      (await WaitForResponse(requestId)) ?? new JObject(),
                                                       out var response,
                                                       out var errorResponse))
            {
                response = new GetCertificateStatusResponse(Request,
                                                            Result.Format(errorResponse));
            }

            response ??= new GetCertificateStatusResponse(Request,
                                                          Result.GenericError());


            #region Send OnGetCertificateStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCertificateStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendReservationStatusUpdate          (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public async Task<ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(ReservationStatusUpdateRequest  Request)

        {

            #region Send OnReservationStatusUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                         this,
                                                         Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!ReservationStatusUpdateResponse.TryParse(Request,
                                                         (await WaitForResponse(requestId)) ?? new JObject(),
                                                          out var response,
                                                          out var errorResponse))
            {
                response = new ReservationStatusUpdateResponse(Request,
                                                               Result.Format(errorResponse));
            }

            response ??= new ReservationStatusUpdateResponse(Request,
                                                             Result.GenericError());


            #region Send OnReservationStatusUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Authorize                            (Request)

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

        #region NotifyEVChargingNeeds                (Request)

        /// <summary>
        /// Send a notify EV charging needs request.
        /// </summary>
        /// <param name="Request">A notify EV charging needs request.</param>
        public async Task<NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest  Request)

        {

            #region Send OnNotifyEVChargingNeedsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyEVChargingNeedsResponse.TryParse(Request,
                                                       (await WaitForResponse(requestId)) ?? new JObject(),
                                                        out var response,
                                                        out var errorResponse))
            {
                response = new NotifyEVChargingNeedsResponse(Request,
                                                             Result.Format(errorResponse));
            }

            response ??= new NotifyEVChargingNeedsResponse(Request,
                                                           Result.GenericError());


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendTransactionEvent                 (Request)

        /// <summary>
        /// Sned a transaction event.
        /// </summary>
        /// <param name="Request">A transaction event request.</param>
        public async Task<TransactionEventResponse>

            SendTransactionEvent(TransactionEventRequest  Request)

        {

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!TransactionEventResponse.TryParse(Request,
                                                   (await WaitForResponse(requestId)) ?? new JObject(),
                                                   out var response,
                                                   out var errorResponse))
            {
                response = new TransactionEventResponse(Request,
                                                        Result.Format(errorResponse));
            }

            response ??= new TransactionEventResponse(Request,
                                                      Result.GenericError());


            #region Send OnTransactionEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTransactionEventResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification               (Request)

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

        #region SendMeterValues                      (Request)

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

        #region NotifyChargingLimit                  (Request)

        /// <summary>
        /// Send a notify charging limit request.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public async Task<NotifyChargingLimitResponse>

            NotifyChargingLimit(NotifyChargingLimitRequest  Request)

        {

            #region Send OnNotifyChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyChargingLimitResponse.TryParse(Request,
                                                     (await WaitForResponse(requestId)) ?? new JObject(),
                                                      out var response,
                                                      out var errorResponse))
            {
                response = new NotifyChargingLimitResponse(Request,
                                                           Result.Format(errorResponse));
            }

            response ??= new NotifyChargingLimitResponse(Request,
                                                         Result.GenericError());


            #region Send OnNotifyChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendClearedChargingLimit             (Request)

        /// <summary>
        /// Send a cleared charging limit request.
        /// </summary>
        /// <param name="Request">A cleared charging limit request.</param>
        public async Task<ClearedChargingLimitResponse>

            SendClearedChargingLimit(ClearedChargingLimitRequest  Request)

        {

            #region Send OnClearedChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!ClearedChargingLimitResponse.TryParse(Request,
                                                      (await WaitForResponse(requestId)) ?? new JObject(),
                                                       out var response,
                                                       out var errorResponse))
            {
                response = new ClearedChargingLimitResponse(Request,
                                                            Result.Format(errorResponse));
            }

            response ??= new ClearedChargingLimitResponse(Request,
                                                          Result.GenericError());


            #region Send OnClearedChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ReportChargingProfiles               (Request)

        /// <summary>
        /// Send a report charging profiles.
        /// </summary>
        /// <param name="Request">A report charging profiles request.</param>
        public async Task<ReportChargingProfilesResponse>

            ReportChargingProfiles(ReportChargingProfilesRequest  Request)

        {

            #region Send OnReportChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!ReportChargingProfilesResponse.TryParse(Request,
                                                        (await WaitForResponse(requestId)) ?? new JObject(),
                                                         out var response,
                                                         out var errorResponse))
            {
                response = new ReportChargingProfilesResponse(Request,
                                                              Result.Format(errorResponse));
            }

            response ??= new ReportChargingProfilesResponse(Request,
                                                            Result.GenericError());


            #region Send OnReportChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region NotifyDisplayMessages                (Request)

        /// <summary>
        /// Send a notify display messages request.
        /// </summary>
        /// <param name="Request">A notify display messages request.</param>
        public async Task<NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(NotifyDisplayMessagesRequest  Request)

        {

            #region Send OnNotifyDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyDisplayMessagesRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyDisplayMessagesResponse.TryParse(Request,
                                                       (await WaitForResponse(requestId)) ?? new JObject(),
                                                        out var response,
                                                        out var errorResponse))
            {
                response = new NotifyDisplayMessagesResponse(Request,
                                                             Result.Format(errorResponse));
            }

            response ??= new NotifyDisplayMessagesResponse(Request,
                                                           Result.GenericError());


            #region Send OnNotifyDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCustomerInformation            (Request)

        /// <summary>
        /// Send a notify customer information request.
        /// </summary>
        /// <param name="Request">A notify customer information request.</param>
        public async Task<NotifyCustomerInformationResponse>

            NotifyCustomerInformation(NotifyCustomerInformationRequest  Request)

        {

            #region Send OnNotifyCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            var requestId = await SendRequest(Request.Action,
                                              Request.RequestId,
                                              Request.ToJSON());

            if (!NotifyCustomerInformationResponse.TryParse(Request,
                                                           (await WaitForResponse(requestId)) ?? new JObject(),
                                                            out var response,
                                                            out var errorResponse))
            {
                response = new NotifyCustomerInformationResponse(Request,
                                                                 Result.Format(errorResponse));
            }

            response ??= new NotifyCustomerInformationResponse(Request,
                                                               Result.GenericError());


            #region Send OnNotifyCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
