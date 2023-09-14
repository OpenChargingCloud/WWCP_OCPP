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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSServerLogger : IEventSender
    {

        #region Properties

        /// <summary>
        /// The unique identifications of all connected charge boxes.
        /// </summary>
        IEnumerable<ChargeBox_Id>  ChargeBoxIds    { get; }

        #endregion


        #region WebSocket related

        /// <summary>
        /// An event sent whenever the HTTP web socket server started.
        /// </summary>
        event OnServerStartedDelegate?                 OnServerStarted;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        /// <summary>
        /// An event sent whenever a HTTP request was received.
        /// </summary>
        event HTTPRequestLogDelegate?                  OnHTTPRequest;

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        event OnNewWebSocketConnectionDelegate?        OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        event HTTPResponseLogDelegate?                 OnHTTPResponse;


        ///// <summary>
        ///// An event sent whenever a web socket frame was received.
        ///// </summary>
        //event OnWebSocketFrameDelegate?                OnWebSocketFrameReceived;

        ///// <summary>
        ///// An event sent whenever a web socket frame was sent.
        ///// </summary>
        //event OnWebSocketFrameDelegate?                OnWebSocketFrameSent;


        #region OnTextMessage   (-Received/-ResponseSent/-ErrorResponseSent)

        event OnWebSocketTextMessageRequestDelegate?      OnTextMessageRequestReceived;

        event OnWebSocketTextMessageResponseDelegate?     OnTextMessageResponseSent;

        event OnWebSocketTextErrorResponseDelegate?       OnTextErrorResponseSent;


        event OnWebSocketTextMessageRequestDelegate?      OnTextMessageRequestSent;

        event OnWebSocketTextMessageResponseDelegate?     OnTextMessageResponseReceived;

        event OnWebSocketTextErrorResponseDelegate?       OnTextErrorResponseReceived;

        #endregion


        ///// <summary>
        ///// An event sent whenever a web socket frame was sent.
        ///// </summary>
        //event OnWebSocketTextMessageDelegate?          OnTextMessageSent;


        ///// <summary>
        ///// An event sent whenever a binary message was received.
        ///// </summary>
        //event OnWebSocketBinaryMessageDelegate?        OnBinaryMessageReceived;

        ///// <summary>
        ///// An event sent whenever a web socket frame was sent.
        ///// </summary>
        //event OnWebSocketBinaryMessageDelegate?        OnBinaryMessageSent;


        ///// <summary>
        ///// An event sent whenever a web socket ping frame was received.
        ///// </summary>
        //event OnWebSocketFrameDelegate?                OnPingMessageReceived;

        ///// <summary>
        ///// An event sent whenever a web socket ping frame was sent.
        ///// </summary>
        //event OnWebSocketFrameDelegate?                OnPingMessageSent;

        ///// <summary>
        ///// An event sent whenever a web socket pong frame was received.
        ///// </summary>
        //event OnWebSocketFrameDelegate?                OnPongMessageReceived;


        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        event OnCloseMessageDelegate?                  OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        event OnTCPConnectionClosedDelegate?           OnTCPConnectionClosed;

        #endregion


        #region OnBootNotification                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event OnBootNotificationRequestDelegate    OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        event OnBootNotificationResponseDelegate   OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate   OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestDelegate    OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a publish firmware to a firmware status notification request was sent.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseDelegate   OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestDelegate    OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        event OnHeartbeatResponseDelegate   OnHeartbeatResponse;

        #endregion

        #region OnNotifyEvent

        /// <summary>
        /// An event sent whenever a notify event request was received.
        /// </summary>
        event OnNotifyEventRequestDelegate    OnNotifyEventRequest;

        /// <summary>
        /// An event sent whenever a response to a notify event was sent.
        /// </summary>
        event OnNotifyEventResponseDelegate   OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestDelegate    OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a security event notification to a heartbeat was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseDelegate   OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReport

        /// <summary>
        /// An event sent whenever a notify report request was received.
        /// </summary>
        event OnNotifyReportRequestDelegate    OnNotifyReportRequest;

        /// <summary>
        /// An event sent whenever a notify report to a heartbeat was sent.
        /// </summary>
        event OnNotifyReportResponseDelegate   OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReport

        /// <summary>
        /// An event sent whenever a notify monitoring report request was received.
        /// </summary>
        event OnNotifyMonitoringReportRequestDelegate    OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a notify monitoring report was sent.
        /// </summary>
        event OnNotifyMonitoringReportResponseDelegate   OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestDelegate    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a log status notification was sent.
        /// </summary>
        event OnLogStatusNotificationResponseDelegate   OnLogStatusNotificationResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion


        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        event OnSignCertificateRequestDelegate    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a sign certificate to a heartbeat was sent.
        /// </summary>
        event OnSignCertificateResponseDelegate   OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificate

        /// <summary>
        /// An event sent whenever a get 15118 EV certificate request was received.
        /// </summary>
        event OnGet15118EVCertificateRequestDelegate    OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a get 15118 EV certificate was sent.
        /// </summary>
        event OnGet15118EVCertificateResponseDelegate   OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus

        /// <summary>
        /// An event sent whenever a get certificate status request was received.
        /// </summary>
        event OnGetCertificateStatusRequestDelegate    OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate status was sent.
        /// </summary>
        event OnGetCertificateStatusResponseDelegate   OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL

        /// <summary>
        /// An event sent whenever a get certificate revocation list request was received.
        /// </summary>
        event OnGetCRLRequestDelegate    OnGetCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate revocation list was sent.
        /// </summary>
        event OnGetCRLResponseDelegate   OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdate

        /// <summary>
        /// An event sent whenever a reservation status update request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestDelegate    OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a reservation status update was sent.
        /// </summary>
        event OnReservationStatusUpdateResponseDelegate   OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeRequestDelegate    OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        event OnAuthorizeResponseDelegate   OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeeds

        /// <summary>
        /// An event sent whenever a notify EV charging needs request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestDelegate    OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a response to a notify EV charging needs was sent.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseDelegate   OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event OnTransactionEventRequestDelegate    OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a transaction event response was sent.
        /// </summary>
        event OnTransactionEventResponseDelegate   OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationRequestDelegate    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        event OnStatusNotificationResponseDelegate   OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesRequestDelegate    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        event OnMeterValuesResponseDelegate   OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit

        /// <summary>
        /// An event sent whenever a notify charging limit request was received.
        /// </summary>
        event OnNotifyChargingLimitRequestDelegate    OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a notify charging limit was sent.
        /// </summary>
        event OnNotifyChargingLimitResponseDelegate   OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimit

        /// <summary>
        /// An event sent whenever a cleared charging limit request was received.
        /// </summary>
        event OnClearedChargingLimitRequestDelegate    OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a cleared charging limit was sent.
        /// </summary>
        event OnClearedChargingLimitResponseDelegate   OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestDelegate    OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        event OnReportChargingProfilesResponseDelegate   OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestDelegate    OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseDelegate   OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingRequestDelegate    OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging was sent.
        /// </summary>
        event OnNotifyPriorityChargingResponseDelegate   OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestDelegate    OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseDelegate   OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessages

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestDelegate    OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a notify display messages was sent.
        /// </summary>
        event OnNotifyDisplayMessagesResponseDelegate   OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformation

        /// <summary>
        /// An event sent whenever a notify customer information request was received.
        /// </summary>
        event OnNotifyCustomerInformationRequestDelegate    OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a notify customer information was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseDelegate   OnNotifyCustomerInformationResponse;

        #endregion


    }

}
