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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSIncomingMessagesEvents : OCPP.CSMS.ICSMSIncomingMessagesEvents
    {

        #region OnBootNotification                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event OnBootNotificationRequestReceivedDelegate    OnBootNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        event OnBootNotificationResponseSentDelegate   OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification        (Request/-Response)

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestReceivedDelegate    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseSentDelegate   OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestReceivedDelegate    OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a publish firmware to a firmware status notification request was sent.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseSentDelegate   OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat                         (Request/-Response)

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestReceivedDelegate    OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        event OnHeartbeatResponseSentDelegate   OnHeartbeatResponse;

        #endregion

        #region OnNotifyEvent                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify event request was received.
        /// </summary>
        event OnNotifyEventRequestReceivedDelegate    OnNotifyEventRequest;

        /// <summary>
        /// An event sent whenever a response to a notify event was sent.
        /// </summary>
        event OnNotifyEventResponseSentDelegate   OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotification         (Request/-Response)

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestReceivedDelegate    OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a security event notification to a heartbeat was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseSentDelegate   OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReport                      (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify report request was received.
        /// </summary>
        event OnNotifyReportRequestReceivedDelegate    OnNotifyReportRequest;

        /// <summary>
        /// An event sent whenever a notify report to a heartbeat was sent.
        /// </summary>
        event OnNotifyReportResponseSentDelegate   OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReport            (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify monitoring report request was received.
        /// </summary>
        event OnNotifyMonitoringReportRequestReceivedDelegate    OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a notify monitoring report was sent.
        /// </summary>
        event OnNotifyMonitoringReportResponseSentDelegate   OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotification             (Request/-Response)

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestReceivedDelegate    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a log status notification was sent.
        /// </summary>
        event OnLogStatusNotificationResponseSentDelegate   OnLogStatusNotificationResponse;

        #endregion

        #region OnDataTransfer                      (Request/-Response)

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion


        #region OnSignCertificate                   (Request/-Response)

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        event OnSignCertificateRequestReceivedDelegate    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a sign certificate to a heartbeat was sent.
        /// </summary>
        event OnSignCertificateResponseSentDelegate   OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificate             (Request/-Response)

        /// <summary>
        /// An event sent whenever a get 15118 EV certificate request was received.
        /// </summary>
        event OnGet15118EVCertificateRequestReceivedDelegate    OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a get 15118 EV certificate was sent.
        /// </summary>
        event OnGet15118EVCertificateResponseSentDelegate   OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus              (Request/-Response)

        /// <summary>
        /// An event sent whenever a get certificate status request was received.
        /// </summary>
        event OnGetCertificateStatusRequestReceivedDelegate    OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate status was sent.
        /// </summary>
        event OnGetCertificateStatusResponseSentDelegate   OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL                            (Request/-Response)

        /// <summary>
        /// An event sent whenever a get certificate revocation list request was received.
        /// </summary>
        event OnGetCRLRequestReceivedDelegate    OnGetCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a get certificate revocation list was sent.
        /// </summary>
        event OnGetCRLResponseSentDelegate   OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdate           (Request/-Response)

        /// <summary>
        /// An event sent whenever a reservation status update request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestReceivedDelegate    OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a reservation status update was sent.
        /// </summary>
        event OnReservationStatusUpdateResponseSentDelegate   OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorize                         (Request/-Response)

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeRequestReceivedDelegate    OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        event OnAuthorizeResponseSentDelegate   OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeeds             (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify EV charging needs request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestReceivedDelegate    OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a response to a notify EV charging needs was sent.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseSentDelegate   OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEvent                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event OnTransactionEventRequestReceivedDelegate    OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a transaction event response was sent.
        /// </summary>
        event OnTransactionEventResponseSentDelegate   OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification                (Request/-Response)

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationRequestReceivedDelegate    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        event OnStatusNotificationResponseSentDelegate   OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesRequestReceivedDelegate    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        event OnMeterValuesResponseSentDelegate   OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit               (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify charging limit request was received.
        /// </summary>
        event OnNotifyChargingLimitRequestReceivedDelegate    OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a notify charging limit was sent.
        /// </summary>
        event OnNotifyChargingLimitResponseSentDelegate   OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimit              (Request/-Response)

        /// <summary>
        /// An event sent whenever a cleared charging limit request was received.
        /// </summary>
        event OnClearedChargingLimitRequestReceivedDelegate    OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a cleared charging limit was sent.
        /// </summary>
        event OnClearedChargingLimitResponseSentDelegate   OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles            (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestReceivedDelegate    OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        event OnReportChargingProfilesResponseSentDelegate   OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule          (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestReceivedDelegate    OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseSentDelegate   OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging            (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingRequestReceivedDelegate    OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging was sent.
        /// </summary>
        event OnNotifyPriorityChargingResponseSentDelegate   OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate         (Request/-Response)

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestReceivedDelegate    OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseSentDelegate   OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessages             (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestReceivedDelegate    OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a notify display messages was sent.
        /// </summary>
        event OnNotifyDisplayMessagesResponseSentDelegate   OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify customer information request was received.
        /// </summary>
        event OnNotifyCustomerInformationRequestReceivedDelegate    OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a notify customer information was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseSentDelegate   OnNotifyCustomerInformationResponse;

        #endregion


    }

}
