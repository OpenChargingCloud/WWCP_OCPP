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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSIncomingMessagesEvents
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
        event OnGet15118EVCertificateResponseSentDelegate                   OnGet15118EVCertificateResponseSent;

        #endregion

        #region OnGetCertificateStatus              (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        event OnGetCertificateStatusRequestReceivedDelegate                 OnGetCertificateStatusRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus request was sent.
        /// </summary>
        event OnGetCertificateStatusResponseSentDelegate                    OnGetCertificateStatusResponseSent;

        #endregion

        #region OnGetCRL                            (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCRL request was received.
        /// </summary>
        event OnGetCRLRequestReceivedDelegate                               OnGetCRLRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a GetCRL request was sent.
        /// </summary>
        event OnGetCRLResponseSentDelegate                                  OnGetCRLResponseSent;

        #endregion

        #region OnSignCertificate                   (Request/-Response)

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateRequestReceivedDelegate                      OnSignCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        event OnSignCertificateResponseSentDelegate                         OnSignCertificateResponseSent;

        #endregion


        // Charging

        #region OnAuthorize                         (Request/-Response)

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        event OnAuthorizeRequestReceivedDelegate                            OnAuthorizeRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an Authorize request was sent.
        /// </summary>
        event OnAuthorizeResponseSentDelegate                               OnAuthorizeResponseSent;

        #endregion

        #region OnClearedChargingLimit              (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit request was received.
        /// </summary>
        event OnClearedChargingLimitRequestReceivedDelegate                 OnClearedChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a ClearedChargingLimit request was sent.
        /// </summary>
        event OnClearedChargingLimitResponseSentDelegate                    OnClearedChargingLimitResponseSent;

        #endregion

        #region OnMeterValues                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        event OnMeterValuesRequestReceivedDelegate                          OnMeterValuesRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        event OnMeterValuesResponseSentDelegate                             OnMeterValuesResponseSent;

        #endregion

        #region OnNotifyChargingLimit               (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        event OnNotifyChargingLimitRequestReceivedDelegate                  OnNotifyChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit request was sent.
        /// </summary>
        event OnNotifyChargingLimitResponseSentDelegate                     OnNotifyChargingLimitResponseSent;

        #endregion

        #region OnNotifyEVChargingNeeds             (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestReceivedDelegate                OnNotifyEVChargingNeedsRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingNeeds request was sent.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseSentDelegate                   OnNotifyEVChargingNeedsResponseSent;

        #endregion

        #region OnNotifyEVChargingSchedule          (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestReceivedDelegate             OnNotifyEVChargingScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule request was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseSentDelegate                OnNotifyEVChargingScheduleResponseSent;

        #endregion

        #region OnNotifyPriorityCharging            (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingRequestReceivedDelegate               OnNotifyPriorityChargingRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging request was sent.
        /// </summary>
        event OnNotifyPriorityChargingResponseSentDelegate                  OnNotifyPriorityChargingResponseSent;

        #endregion

        #region OnNotifySettlement                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifySettlement request was received.
        /// </summary>
        event OnNotifySettlementRequestReceivedDelegate               OnNotifySettlementRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifySettlement request was sent.
        /// </summary>
        event OnNotifySettlementResponseSentDelegate                  OnNotifySettlementResponseSent;

        #endregion

        #region OnPullDynamicScheduleUpdate         (Request/-Response)

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestReceivedDelegate            OnPullDynamicScheduleUpdateRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate request was sent.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseSentDelegate               OnPullDynamicScheduleUpdateResponseSent;

        #endregion

        #region OnReportChargingProfiles            (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestReceivedDelegate OnReportChargingProfilesRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles request was sent.
        /// </summary>
        event OnReportChargingProfilesResponseSentDelegate OnReportChargingProfilesResponseSent;

        #endregion

        #region OnReservationStatusUpdate           (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestReceivedDelegate              OnReservationStatusUpdateRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate request was sent.
        /// </summary>
        event OnReservationStatusUpdateResponseSentDelegate                 OnReservationStatusUpdateResponseSent;

        #endregion

        #region OnStatusNotification                (Request/-Response)

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationRequestReceivedDelegate OnStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        event OnStatusNotificationResponseSentDelegate OnStatusNotificationResponseSent;

        #endregion

        #region OnTransactionEvent                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        event OnTransactionEventRequestReceivedDelegate                     OnTransactionEventRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a TransactionEvent request was sent.
        /// </summary>
        event OnTransactionEventResponseSentDelegate                        OnTransactionEventResponseSent;

        #endregion


        // Customer

        #region OnNotifyCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify customer information request was received.
        /// </summary>
        event OnNotifyCustomerInformationRequestReceivedDelegate            OnNotifyCustomerInformationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a notify customer information request was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseSentDelegate               OnNotifyCustomerInformationResponseSent;

        #endregion

        #region OnNotifyDisplayMessages             (Request/-Response)

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestReceivedDelegate                OnNotifyDisplayMessagesRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a notify display messages request was sent.
        /// </summary>
        event OnNotifyDisplayMessagesResponseSentDelegate                   OnNotifyDisplayMessagesResponseSent;

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
        event OnBootNotificationResponseSentDelegate                        OnBootNotificationResponseSent;

        #endregion

        #region OnFirmwareStatusNotification        (Request/-Response)

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestReceivedDelegate           OnFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseSentDelegate              OnFirmwareStatusNotificationResponseSent;

        #endregion

        #region OnHeartbeat                         (Request/-Response)

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestReceivedDelegate                            OnHeartbeatRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat request was sent.
        /// </summary>
        event OnHeartbeatResponseSentDelegate                               OnHeartbeatResponseSent;

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestReceivedDelegate    OnPublishFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseSentDelegate       OnPublishFirmwareStatusNotificationResponseSent;

        #endregion


        // Monitoring

        #region OnLogStatusNotification             (Request/-Response)

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestReceivedDelegate                OnLogStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        event OnLogStatusNotificationResponseSentDelegate                   OnLogStatusNotificationResponseSent;

        #endregion

        #region OnNotifyEvent                       (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyEvent request was received.
        /// </summary>
        event OnNotifyEventRequestReceivedDelegate                          OnNotifyEventRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyEvent request was sent.
        /// </summary>
        event OnNotifyEventResponseSentDelegate                             OnNotifyEventResponseSent;

        #endregion

        #region OnNotifyMonitoringReport            (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport request was received.
        /// </summary>
        event OnNotifyMonitoringReportRequestReceivedDelegate               OnNotifyMonitoringReportRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyMonitoringReport request was sent.
        /// </summary>
        event OnNotifyMonitoringReportResponseSentDelegate                  OnNotifyMonitoringReportResponseSent;

        #endregion

        #region OnNotifyReport                      (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyReport request was received.
        /// </summary>
        event OnNotifyReportRequestReceivedDelegate                         OnNotifyReportRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a NotifyReport request was sent.
        /// </summary>
        event OnNotifyReportResponseSentDelegate                            OnNotifyReportResponseSent;

        #endregion

        #region OnSecurityEventNotification         (Request/-Response)

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestReceivedDelegate            OnSecurityEventNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseSentDelegate               OnSecurityEventNotificationResponseSent;

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
