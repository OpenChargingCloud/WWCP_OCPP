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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface INetworkingNodeClient : INetworkingNodeClientEvents,
                                             IHTTPClient,
                                             IEventSender
    {

        String?  ClientCloseMessage    { get; }


        #region Custom JSON serializer delegates

        #region Charging Station Messages

        CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer    { get; set; }


        #endregion

        #region Data Structures

        CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer            { get; set; }
        CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                 { get; set; }

        #endregion

        #endregion


        #region SendBootNotification                  (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public Task<BootNotificationResponse> SendBootNotification(BootNotificationRequest Request);

        #endregion

        #region SendFirmwareStatusNotification        (Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public Task<FirmwareStatusNotificationResponse> SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request);

        #endregion

        #region SendPublishFirmwareStatusNotification (Request)

        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A publish firmware status notification request.</param>
        public Task<PublishFirmwareStatusNotificationResponse> SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request);

        #endregion

        #region SendHeartbeat                         (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public Task<HeartbeatResponse> SendHeartbeat(HeartbeatRequest Request);

        #endregion

        #region NotifyEvent                           (Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="Request">A notify event request.</param>
        public Task<NotifyEventResponse> NotifyEvent(NotifyEventRequest Request);

        #endregion

        #region SendSecurityEventNotification         (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
        public Task<SecurityEventNotificationResponse> SendSecurityEventNotification(SecurityEventNotificationRequest Request);

        #endregion

        #region NotifyReport                          (Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="Request">A notify report request.</param>
        public Task<NotifyReportResponse> NotifyReport(NotifyReportRequest Request);

        #endregion

        #region NotifyMonitoringReport                (Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="Request">A notify monitoring report request.</param>
        public Task<NotifyMonitoringReportResponse> NotifyMonitoringReport(NotifyMonitoringReportRequest Request);

        #endregion

        #region SendLogStatusNotification             (Request)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
        public Task<LogStatusNotificationResponse> SendLogStatusNotification(LogStatusNotificationRequest Request);

        #endregion

        #region TransferData                          (Request)

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public Task<OCPPv2_1.CSMS.DataTransferResponse> TransferData(OCPPv2_1.CS.DataTransferRequest Request);

        #endregion


        #region SendCertificateSigningRequest         (Request)

        /// <summary>
        /// Send a certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<SignCertificateResponse> SendCertificateSigningRequest(SignCertificateRequest Request);

        #endregion

        #region Get15118EVCertificate                 (Request)

        /// <summary>
        /// Send a certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<Get15118EVCertificateResponse> Get15118EVCertificate(Get15118EVCertificateRequest Request);

        #endregion

        #region GetCertificateStatus                  (Request)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="Request">A get certificate status request.</param>
        public Task<GetCertificateStatusResponse> GetCertificateStatus(GetCertificateStatusRequest Request);

        #endregion

        #region GetCRL                                (Request)

        /// <summary>
        /// Get a certificate revocation list from CSMS for the specified certificate.
        /// </summary>
        /// <param name="Request">A get certificate revocation list request.</param>
        public Task<GetCRLResponse> GetCRL(GetCRLRequest Request);

        #endregion


        #region SendReservationStatusUpdate           (Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="Request">A reservation status update request.</param>
        public Task<ReservationStatusUpdateResponse> SendReservationStatusUpdate(ReservationStatusUpdateRequest Request);

        #endregion

        #region Authorize                             (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public Task<AuthorizeResponse> Authorize(AuthorizeRequest Request);

        #endregion

        #region NotifyEVChargingNeeds                 (Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="Request">A notify EV charging needs request.</param>
        public Task<NotifyEVChargingNeedsResponse> NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest Request);

        #endregion

        #region SendTransactionEvent                  (Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A transaction event request.</param>
        public Task<TransactionEventResponse> SendTransactionEvent(TransactionEventRequest Request);

        #endregion

        #region SendStatusNotification                (Request)

        /// <summary>
        /// Send a status notification for the given evse and connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public Task<StatusNotificationResponse> SendStatusNotification(StatusNotificationRequest Request);

        #endregion

        #region SendMeterValues                       (Request)

        /// <summary>
        /// Send a meter values for the given evse.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public Task<MeterValuesResponse> SendMeterValues(MeterValuesRequest Request);

        #endregion

        #region NotifyChargingLimit                   (Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<NotifyChargingLimitResponse> NotifyChargingLimit(NotifyChargingLimitRequest Request);

        #endregion

        #region SendClearedChargingLimit              (Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<ClearedChargingLimitResponse> SendClearedChargingLimit(ClearedChargingLimitRequest Request);

        #endregion

        #region ReportChargingProfiles                (Request)

        /// <summary>
        /// Report about charging profiles.
        /// </summary>
        /// <param name="Request">A report charging profiles request.</param>
        public Task<ReportChargingProfilesResponse> ReportChargingProfiles(ReportChargingProfilesRequest Request);

        #endregion

        #region NotifyEVChargingSchedule              (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingSchedule request.</param>
        public Task<NotifyEVChargingScheduleResponse> NotifyEVChargingSchedule(NotifyEVChargingScheduleRequest Request);

        #endregion

        #region NotifyPriorityCharging                (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="Request">A NotifyPriorityCharging request.</param>
        public Task<NotifyPriorityChargingResponse> NotifyPriorityCharging(NotifyPriorityChargingRequest Request);

        #endregion

        #region PullDynamicScheduleUpdate             (Request)

        /// <summary>
        /// Pull a dynamic schedule update.
        /// </summary>
        /// <param name="Request">A PullDynamicScheduleUpdate request.</param>
        public Task<PullDynamicScheduleUpdateResponse> PullDynamicScheduleUpdate(PullDynamicScheduleUpdateRequest Request);

        #endregion


        #region NotifyDisplayMessages                 (Request)

        /// <summary>
        /// Notify about display messages.
        /// </summary>
        /// <param name="Request">A notify display messages request.</param>
        public Task<NotifyDisplayMessagesResponse> NotifyDisplayMessages(NotifyDisplayMessagesRequest Request);

        #endregion

        #region NotifyCustomerInformation             (Request)

        /// <summary>
        /// Notify about customer information.
        /// </summary>
        /// <param name="Request">A notify customer information request.</param>
        public Task<NotifyCustomerInformationResponse> NotifyCustomerInformation(NotifyCustomerInformationRequest Request);

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData                    (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A binary data transfer request.</param>
        public Task<OCPPv2_1.CSMS.BinaryDataTransferResponse> TransferBinaryData(OCPPv2_1.CS.BinaryDataTransferRequest Request);

        #endregion


    }

}
