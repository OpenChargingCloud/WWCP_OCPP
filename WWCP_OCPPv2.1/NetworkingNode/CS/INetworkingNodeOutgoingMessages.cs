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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface INetworkingNodeOutgoingMessages : OCPP.NN.CS.INetworkingNodeOutgoingMessages
    {

        #region BootNotification                  (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public Task<BootNotificationResponse> BootNotification(BootNotificationRequest Request);

        #endregion

        #region FirmwareStatusNotification        (Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public Task<FirmwareStatusNotificationResponse> FirmwareStatusNotification(FirmwareStatusNotificationRequest Request);

        #endregion

        #region PublishFirmwareStatusNotification (Request)

        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A publish firmware status notification request.</param>
        public Task<PublishFirmwareStatusNotificationResponse> PublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request);

        #endregion

        #region Heartbeat                         (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public Task<HeartbeatResponse> Heartbeat(HeartbeatRequest Request);

        #endregion

        #region NotifyEvent                       (Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="Request">A notify event request.</param>
        public Task<NotifyEventResponse> NotifyEvent(NotifyEventRequest Request);

        #endregion

        #region SecurityEventNotification         (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
        public Task<SecurityEventNotificationResponse> SecurityEventNotification(SecurityEventNotificationRequest Request);

        #endregion

        #region NotifyReport                      (Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="Request">A notify report request.</param>
        public Task<NotifyReportResponse> NotifyReport(NotifyReportRequest Request);

        #endregion

        #region NotifyMonitoringReport            (Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="Request">A notify monitoring report request.</param>
        public Task<NotifyMonitoringReportResponse> NotifyMonitoringReport(NotifyMonitoringReportRequest Request);

        #endregion

        #region LogStatusNotification             (Request)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
        public Task<LogStatusNotificationResponse> LogStatusNotification(LogStatusNotificationRequest Request);

        #endregion


        #region SignCertificate                   (Request)

        /// <summary>
        /// Send a certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<SignCertificateResponse> SignCertificate(SignCertificateRequest Request);

        #endregion

        #region Get15118EVCertificate             (Request)

        /// <summary>
        /// Send a certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<Get15118EVCertificateResponse> Get15118EVCertificate(Get15118EVCertificateRequest Request);

        #endregion

        #region GetCertificateStatus              (Request)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="Request">A get certificate status request.</param>
        public Task<GetCertificateStatusResponse> GetCertificateStatus(GetCertificateStatusRequest Request);

        #endregion

        #region GetCRL                            (Request)

        /// <summary>
        /// Get a certificate revocation list from CSMS for the specified certificate.
        /// </summary>
        /// <param name="Request">A get certificate revocation list request.</param>
        public Task<GetCRLResponse> GetCRL(GetCRLRequest Request);

        #endregion


        #region ReservationStatusUpdate           (Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="Request">A reservation status update request.</param>
        public Task<ReservationStatusUpdateResponse> ReservationStatusUpdate(ReservationStatusUpdateRequest Request);

        #endregion

        #region Authorize                         (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public Task<AuthorizeResponse> Authorize(AuthorizeRequest Request);

        #endregion

        #region NotifyEVChargingNeeds             (Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="Request">A notify EV charging needs request.</param>
        public Task<NotifyEVChargingNeedsResponse> NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest Request);

        #endregion

        #region TransactionEvent                  (Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A transaction event request.</param>
        public Task<TransactionEventResponse> TransactionEvent(TransactionEventRequest Request);

        #endregion

        #region StatusNotification                (Request)

        /// <summary>
        /// Send a status notification for the given evse and connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public Task<StatusNotificationResponse> StatusNotification(StatusNotificationRequest Request);

        #endregion

        #region MeterValues                       (Request)

        /// <summary>
        /// Send a meter values for the given evse.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public Task<MeterValuesResponse> MeterValues(MeterValuesRequest Request);

        #endregion

        #region NotifyChargingLimit               (Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<NotifyChargingLimitResponse> NotifyChargingLimit(NotifyChargingLimitRequest Request);

        #endregion

        #region ClearedChargingLimit              (Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<ClearedChargingLimitResponse> ClearedChargingLimit(ClearedChargingLimitRequest Request);

        #endregion

        #region ReportChargingProfiles            (Request)

        /// <summary>
        /// Report about charging profiles.
        /// </summary>
        /// <param name="Request">A report charging profiles request.</param>
        public Task<ReportChargingProfilesResponse> ReportChargingProfiles(ReportChargingProfilesRequest Request);

        #endregion

        #region NotifyEVChargingSchedule          (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingSchedule request.</param>
        public Task<NotifyEVChargingScheduleResponse> NotifyEVChargingSchedule(NotifyEVChargingScheduleRequest Request);

        #endregion

        #region NotifyPriorityCharging            (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="Request">A NotifyPriorityCharging request.</param>
        public Task<NotifyPriorityChargingResponse> NotifyPriorityCharging(NotifyPriorityChargingRequest Request);

        #endregion

        #region NotifySettlement                  (Request)

        /// <summary>
        /// Notify about payment settlements.
        /// </summary>
        /// <param name="Request">A NotifySettlement request.</param>
        public Task<NotifySettlementResponse> NotifySettlement(NotifySettlementRequest Request);

        #endregion

        #region PullDynamicScheduleUpdate         (Request)

        /// <summary>
        /// Pull a dynamic schedule update.
        /// </summary>
        /// <param name="Request">A PullDynamicScheduleUpdate request.</param>
        public Task<PullDynamicScheduleUpdateResponse> PullDynamicScheduleUpdate(PullDynamicScheduleUpdateRequest Request);

        #endregion


        #region NotifyDisplayMessages             (Request)

        /// <summary>
        /// Notify about display messages.
        /// </summary>
        /// <param name="Request">A notify display messages request.</param>
        public Task<NotifyDisplayMessagesResponse> NotifyDisplayMessages(NotifyDisplayMessagesRequest Request);

        #endregion

        #region NotifyCustomerInformation         (Request)

        /// <summary>
        /// Notify about customer information.
        /// </summary>
        /// <param name="Request">A notify customer information request.</param>
        public Task<NotifyCustomerInformationResponse> NotifyCustomerInformation(NotifyCustomerInformationRequest Request);

        #endregion


        // Overlay Networking Extensions

        #region NotifyNetworkTopology             (Request)

        /// <summary>
        /// Notify about the current network topology or a current change within the topology.
        /// </summary>
        /// <param name="Request">A NotifyNetworkTopology request.</param>
        public Task<OCPP.NN.NotifyNetworkTopologyResponse> NotifyNetworkTopology(OCPP.NN.NotifyNetworkTopologyRequest Request);

        #endregion


    }

}
