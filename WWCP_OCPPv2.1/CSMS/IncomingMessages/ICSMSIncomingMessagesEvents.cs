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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSIncomingMessagesEvents : OCPP.CSMS.ICSMSIncomingMessagesEvents
    {

        // Certificates

        #region OnGet15118EVCertificate             (Request/-Response)

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate request was received.
        /// </summary>
        event OnGet15118EVCertificateRequestReceivedDelegate                OnGet15118EVCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate request was sent.
        /// </summary>
        event OnGet15118EVCertificateResponseSentDelegate                   OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus              (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        event OnGetCertificateStatusRequestReceivedDelegate                 OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus request was sent.
        /// </summary>
        event OnGetCertificateStatusResponseSentDelegate                    OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL                            (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCRL request was received.
        /// </summary>
        event OnGetCRLRequestReceivedDelegate                               OnGetCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCRL request was sent.
        /// </summary>
        event OnGetCRLResponseSentDelegate                                  OnGetCRLResponse;

        #endregion

        #region OnSignCertificate                   (Request/-Response)

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateRequestReceivedDelegate                      OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        event OnSignCertificateResponseSentDelegate                         OnSignCertificateResponse;

        #endregion


        // Charging

        #region OnAuthorize                         (Request/-Response)

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        event OnAuthorizeRequestReceivedDelegate                            OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a response to an Authorize request was sent.
        /// </summary>
        event OnAuthorizeResponseSentDelegate                               OnAuthorizeResponse;

        #endregion

        #region OnClearedChargingLimit              (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit request was received.
        /// </summary>
        event OnClearedChargingLimitRequestReceivedDelegate                 OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearedChargingLimit request was sent.
        /// </summary>
        event OnClearedChargingLimitResponseSentDelegate                    OnClearedChargingLimitResponse;

        #endregion

        #region OnMeterValues                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        event OnMeterValuesRequestReceivedDelegate                          OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        event OnMeterValuesResponseSentDelegate                             OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit               (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        event OnNotifyChargingLimitRequestReceivedDelegate                  OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit request was sent.
        /// </summary>
        event OnNotifyChargingLimitResponseSentDelegate                     OnNotifyChargingLimitResponse;

        #endregion

        #region OnNotifyEVChargingNeeds             (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestReceivedDelegate                OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingNeeds request was sent.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseSentDelegate                   OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnNotifyEVChargingSchedule          (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestReceivedDelegate             OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule request was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseSentDelegate                OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging            (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingRequestReceivedDelegate               OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging request was sent.
        /// </summary>
        event OnNotifyPriorityChargingResponseSentDelegate                  OnNotifyPriorityChargingResponse;

        #endregion

        #region OnNotifySettlement                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifySettlement request was received.
        /// </summary>
        event OnNotifySettlementRequestReceivedDelegate               OnNotifySettlementRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifySettlement request was sent.
        /// </summary>
        event OnNotifySettlementResponseSentDelegate                  OnNotifySettlementResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate         (Request/-Response)

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestReceivedDelegate            OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate request was sent.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseSentDelegate               OnPullDynamicScheduleUpdateResponse;

        #endregion

        #region OnReportChargingProfiles            (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestReceivedDelegate OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles request was sent.
        /// </summary>
        event OnReportChargingProfilesResponseSentDelegate OnReportChargingProfilesResponse;

        #endregion

        #region OnReservationStatusUpdate           (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestReceivedDelegate              OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate request was sent.
        /// </summary>
        event OnReservationStatusUpdateResponseSentDelegate                 OnReservationStatusUpdateResponse;

        #endregion

        #region OnStatusNotification                (Request/-Response)

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationRequestReceivedDelegate OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        event OnStatusNotificationResponseSentDelegate OnStatusNotificationResponse;

        #endregion

        #region OnTransactionEvent                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        event OnTransactionEventRequestReceivedDelegate                     OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a response to a TransactionEvent request was sent.
        /// </summary>
        event OnTransactionEventResponseSentDelegate                        OnTransactionEventResponse;

        #endregion


        // Customer

        #region OnNotifyCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify customer information request was received.
        /// </summary>
        event OnNotifyCustomerInformationRequestReceivedDelegate            OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a notify customer information request was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseSentDelegate               OnNotifyCustomerInformationResponse;

        #endregion

        #region OnNotifyDisplayMessages             (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestReceivedDelegate                OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a notify display messages request was sent.
        /// </summary>
        event OnNotifyDisplayMessagesResponseSentDelegate                   OnNotifyDisplayMessagesResponse;

        #endregion


        // Firmware

        #region OnBootNotification                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        event OnBootNotificationRequestReceivedDelegate                     OnBootNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a BootNotification request was sent.
        /// </summary>
        event OnBootNotificationResponseSentDelegate                        OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification        (Request/-Response)

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestReceivedDelegate           OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseSentDelegate              OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat                         (Request/-Response)

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestReceivedDelegate                            OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat request was sent.
        /// </summary>
        event OnHeartbeatResponseSentDelegate                               OnHeartbeatResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestReceivedDelegate    OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseSentDelegate       OnPublishFirmwareStatusNotificationResponse;

        #endregion


        // Monitoring

        #region OnLogStatusNotification             (Request/-Response)

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestReceivedDelegate                OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        event OnLogStatusNotificationResponseSentDelegate                   OnLogStatusNotificationResponse;

        #endregion

        #region OnNotifyEvent                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEvent request was received.
        /// </summary>
        event OnNotifyEventRequestReceivedDelegate                          OnNotifyEventRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyEvent request was sent.
        /// </summary>
        event OnNotifyEventResponseSentDelegate                             OnNotifyEventResponse;

        #endregion

        #region OnNotifyMonitoringReport            (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport request was received.
        /// </summary>
        event OnNotifyMonitoringReportRequestReceivedDelegate               OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyMonitoringReport request was sent.
        /// </summary>
        event OnNotifyMonitoringReportResponseSentDelegate                  OnNotifyMonitoringReportResponse;

        #endregion

        #region OnNotifyReport                      (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyReport request was received.
        /// </summary>
        event OnNotifyReportRequestReceivedDelegate                         OnNotifyReportRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyReport request was sent.
        /// </summary>
        event OnNotifyReportResponseSentDelegate                            OnNotifyReportResponse;

        #endregion

        #region OnSecurityEventNotification         (Request/-Response)

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestReceivedDelegate            OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseSentDelegate               OnSecurityEventNotificationResponse;

        #endregion



        #region OnDataTransfer                      (Request/-Response)

        /// <summary>
        /// An event sent whenever an incoming DataTransfer request was received.
        /// </summary>
        event OnDataTransferRequestReceivedDelegate                         OnDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an incoming DataTransfer request was sent.
        /// </summary>
        event OnDataTransferResponseSentDelegate                            OnDataTransferResponseSent;

        #endregion


    }

}
