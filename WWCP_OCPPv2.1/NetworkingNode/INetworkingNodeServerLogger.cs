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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all NetworkingNode servers.
    /// </summary>
    public interface INetworkingNodeServerLogger : IEventSender
    {

        #region Properties

        /// <summary>
        /// The unique identifications of all connected charging stations.
        /// </summary>
        IEnumerable<ChargingStation_Id>  ChargingStationIds    { get; }

        #endregion


        #region WebSocket connection

        ///// <summary>
        ///// An event sent whenever the HTTP web socket server started.
        ///// </summary>
        //event CSMS.OnServerStartedDelegate?                 OnServerStarted;

        ///// <summary>
        ///// An event sent whenever a new TCP connection was accepted.
        ///// </summary>
        //event CSMS.OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        ///// <summary>
        ///// An event sent whenever a HTTP request was received.
        ///// </summary>
        //event HTTPRequestLogDelegate?                  OnHTTPRequest;

        ///// <summary>
        ///// An event sent whenever the HTTP connection switched successfully to web socket.
        ///// </summary>
        //event CSMS.OnNewWebSocketConnectionDelegate?        OnNewWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever a reponse to a HTTP request was sent.
        ///// </summary>
        //event HTTPResponseLogDelegate?                 OnHTTPResponse;

        ///// <summary>
        ///// An event sent whenever a web socket close frame was received.
        ///// </summary>
        //event CSMS.OnCloseMessageDelegate?                  OnCloseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a TCP connection was closed.
        ///// </summary>
        //event CSMS.OnTCPConnectionClosedDelegate?           OnTCPConnectionClosed;

        #endregion


        #region OnJSONMessage   (-Received/-ResponseSent/-ErrorResponseSent)

        event CSMS.OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestReceived;

        event CSMS.OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseSent;

        event CSMS.OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseSent;


        event CSMS.OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestSent;

        event CSMS.OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseReceived;

        event CSMS.OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseReceived;

        #endregion

        #region OnBinaryMessage (-Received/-ResponseSent/-ErrorResponseSent)

        event CSMS.OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestReceived;

        event CSMS.OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseSent;

        //event CSMS.OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseSent;


        event CSMS.OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestSent;

        event CSMS.OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseReceived;

        //event CSMS.OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseReceived;

        #endregion


        #region OnBootNotification                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event CSMS.OnBootNotificationRequestDelegate    OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        event CSMS.OnBootNotificationResponseDelegate   OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event CSMS.OnFirmwareStatusNotificationRequestDelegate    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event CSMS.OnFirmwareStatusNotificationResponseDelegate   OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event CSMS.OnPublishFirmwareStatusNotificationRequestDelegate    OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a publish firmware to a firmware status notification request was sent.
        /// </summary>
        event CSMS.OnPublishFirmwareStatusNotificationResponseDelegate   OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat                            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        event CSMS.OnHeartbeatRequestDelegate    OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        event CSMS.OnHeartbeatResponseDelegate   OnHeartbeatResponse;

        #endregion

        #region OnNotifyEvent                          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify event request was received.
        /// </summary>
        event CSMS.OnNotifyEventRequestDelegate    OnNotifyEventRequest;

        /// <summary>
        /// An event sent whenever a response to a notify event was sent.
        /// </summary>
        event CSMS.OnNotifyEventResponseDelegate   OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotification            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        event CSMS.OnSecurityEventNotificationRequestDelegate    OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a security event notification to a heartbeat was sent.
        /// </summary>
        event CSMS.OnSecurityEventNotificationResponseDelegate   OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReport                         (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify report request was received.
        /// </summary>
        event CSMS.OnNotifyReportRequestDelegate    OnNotifyReportRequest;

        /// <summary>
        /// An event sent whenever a notify report to a heartbeat was sent.
        /// </summary>
        event CSMS.OnNotifyReportResponseDelegate   OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReport               (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify monitoring report request was received.
        /// </summary>
        event CSMS.OnNotifyMonitoringReportRequestDelegate    OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a notify monitoring report was sent.
        /// </summary>
        event CSMS.OnNotifyMonitoringReportResponseDelegate   OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotification                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        event CSMS.OnLogStatusNotificationRequestDelegate    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a log status notification was sent.
        /// </summary>
        event CSMS.OnLogStatusNotificationResponseDelegate   OnLogStatusNotificationResponse;

        #endregion

        #region OnDataTransfer                         (-Request/-Response)

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event CSMS.OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event CSMS.OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion


        #region OnSignCertificate                      (-Request/-Response)

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        event CSMS.OnSignCertificateRequestDelegate    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a sign certificate to a heartbeat was sent.
        /// </summary>
        event CSMS.OnSignCertificateResponseDelegate   OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificate                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a get 15118 EV certificate request was received.
        /// </summary>
        event CSMS.OnGet15118EVCertificateRequestDelegate    OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a get 15118 EV certificate was sent.
        /// </summary>
        event CSMS.OnGet15118EVCertificateResponseDelegate   OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever a get certificate status request was received.
        /// </summary>
        event CSMS.OnGetCertificateStatusRequestDelegate    OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate status was sent.
        /// </summary>
        event CSMS.OnGetCertificateStatusResponseDelegate   OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL                               (-Request/-Response)

        /// <summary>
        /// An event sent whenever a get certificate revocation list request was received.
        /// </summary>
        event CSMS.OnGetCRLRequestDelegate    OnGetCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate revocation list was sent.
        /// </summary>
        event CSMS.OnGetCRLResponseDelegate   OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdate              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a reservation status update request was received.
        /// </summary>
        event CSMS.OnReservationStatusUpdateRequestDelegate    OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a reservation status update was sent.
        /// </summary>
        event CSMS.OnReservationStatusUpdateResponseDelegate   OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorize                            (-Request/-Response)

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event CSMS.OnAuthorizeRequestDelegate    OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        event CSMS.OnAuthorizeResponseDelegate   OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeeds                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify EV charging needs request was received.
        /// </summary>
        event CSMS.OnNotifyEVChargingNeedsRequestDelegate    OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a response to a notify EV charging needs was sent.
        /// </summary>
        event CSMS.OnNotifyEVChargingNeedsResponseDelegate   OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEvent                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event CSMS.OnTransactionEventRequestDelegate    OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a transaction event response was sent.
        /// </summary>
        event CSMS.OnTransactionEventResponseDelegate   OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification                   (-Request/-Response)

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event CSMS.OnStatusNotificationRequestDelegate    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        event CSMS.OnStatusNotificationResponseDelegate   OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues                          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event CSMS.OnMeterValuesRequestDelegate    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        event CSMS.OnMeterValuesResponseDelegate   OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit                  (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify charging limit request was received.
        /// </summary>
        event CSMS.OnNotifyChargingLimitRequestDelegate    OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a notify charging limit was sent.
        /// </summary>
        event CSMS.OnNotifyChargingLimitResponseDelegate   OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimit                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever a cleared charging limit request was received.
        /// </summary>
        event CSMS.OnClearedChargingLimitRequestDelegate    OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a cleared charging limit was sent.
        /// </summary>
        event CSMS.OnClearedChargingLimitResponseDelegate   OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles               (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event CSMS.OnReportChargingProfilesRequestDelegate    OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        event CSMS.OnReportChargingProfilesResponseDelegate   OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event CSMS.OnNotifyEVChargingScheduleRequestDelegate    OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        event CSMS.OnNotifyEVChargingScheduleResponseDelegate   OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging               (-Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event CSMS.OnNotifyPriorityChargingRequestDelegate    OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging was sent.
        /// </summary>
        event CSMS.OnNotifyPriorityChargingResponseDelegate   OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event CSMS.OnPullDynamicScheduleUpdateRequestDelegate    OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        event CSMS.OnPullDynamicScheduleUpdateResponseDelegate   OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessages                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event CSMS.OnNotifyDisplayMessagesRequestDelegate    OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a notify display messages was sent.
        /// </summary>
        event CSMS.OnNotifyDisplayMessagesResponseDelegate   OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformation            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a notify customer information request was received.
        /// </summary>
        event CSMS.OnNotifyCustomerInformationRequestDelegate    OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a notify customer information was sent.
        /// </summary>
        event CSMS.OnNotifyCustomerInformationResponseDelegate   OnNotifyCustomerInformationResponse;

        #endregion


    }

}
