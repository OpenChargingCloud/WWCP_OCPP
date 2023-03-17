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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_0.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// Extention methods for all charging station clients
    /// </summary>
    public static class IChargingStationClientExtensions
    {



    }


    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface IChargingStationClient : IHTTPClient,
                                              IEventSender
    {

        #region SendBootNotification

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        event OnBootNotificationResponseDelegate  OnBootNotificationResponse;


        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public Task<BootNotificationResponse> SendBootNotification(BootNotificationRequest Request);

        #endregion

        #region SendFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;


        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public Task<FirmwareStatusNotificationResponse> SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request);

        #endregion

        #region SendPublishFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestDelegate   OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseDelegate  OnPublishFirmwareStatusNotificationResponse;


        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A publish firmware status notification request.</param>
        public Task<PublishFirmwareStatusNotificationResponse> SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request);

        #endregion

        #region SendHeartbeat

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseDelegate  OnHeartbeatResponse;


        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public Task<HeartbeatResponse> SendHeartbeat(HeartbeatRequest Request);

        #endregion

        #region NotifyEvent

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        event OnNotifyEventRequestDelegate   OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        event OnNotifyEventResponseDelegate  OnNotifyEventResponse;


        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="Request">A notify event request.</param>
        public Task<NotifyEventResponse> NotifyEvent(NotifyEventRequest Request);

        #endregion

        #region SendSecurityEventNotification

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        event OnSecurityEventNotificationRequestDelegate   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        event OnSecurityEventNotificationResponseDelegate  OnSecurityEventNotificationResponse;


        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
        public Task<SecurityEventNotificationResponse> SendSecurityEventNotification(SecurityEventNotificationRequest Request);

        #endregion

        #region NotifyReport

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        event OnNotifyReportRequestDelegate   OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        event OnNotifyReportResponseDelegate  OnNotifyReportResponse;


        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="Request">A notify report request.</param>
        public Task<NotifyReportResponse> NotifyReport(NotifyReportRequest Request);

        #endregion

        #region NotifyMonitoringReport

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        event OnNotifyMonitoringReportRequestDelegate   OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        event OnNotifyMonitoringReportResponseDelegate  OnNotifyMonitoringReportResponse;


        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="Request">A notify monitoring report request.</param>
        public Task<NotifyMonitoringReportResponse> NotifyMonitoringReport(NotifyMonitoringReportRequest Request);

        #endregion

        #region SendLogStatusNotification

        /// <summary>
        /// An event fired whenever a log status notification request will be sent to the CSMS.
        /// </summary>
        event OnLogStatusNotificationRequestDelegate   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        event OnLogStatusNotificationResponseDelegate  OnLogStatusNotificationResponse;


        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
        public Task<LogStatusNotificationResponse> SendLogStatusNotification(LogStatusNotificationRequest Request);

        #endregion

        #region TransferData

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate  OnDataTransferResponse;


        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public Task<CSMS.DataTransferResponse> TransferData(DataTransferRequest Request);

        #endregion


        #region SignCertificate

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        event OnSignCertificateRequestDelegate   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        event OnSignCertificateResponseDelegate  OnSignCertificateResponse;


        /// <summary>
        /// Send certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<SignCertificateResponse> SignCertificate(SignCertificateRequest Request);

        #endregion

        #region Get15118EVCertificate

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate request will be sent to the CSMS.
        /// </summary>
        event OnGet15118EVCertificateRequestDelegate   OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate request was received.
        /// </summary>
        event OnGet15118EVCertificateResponseDelegate  OnGet15118EVCertificateResponse;


        /// <summary>
        /// Send a certificate signing request.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<Get15118EVCertificateResponse> Get15118EVCertificate(Get15118EVCertificateRequest Request);

        #endregion

        #region GetCertificateStatus

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        event OnGetCertificateStatusRequestDelegate   OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        event OnGetCertificateStatusResponseDelegate  OnGetCertificateStatusResponse;


        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="Request">A get certificate status request.</param>
        public Task<GetCertificateStatusResponse> GetCertificateStatus(GetCertificateStatusRequest Request);

        #endregion


        #region SendReservationStatusUpdate

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        event OnReservationStatusUpdateRequestDelegate   OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        event OnReservationStatusUpdateResponseDelegate  OnReservationStatusUpdateResponse;


        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="Request">A reservation status update request.</param>
        public Task<ReservationStatusUpdateResponse> SendReservationStatusUpdate(ReservationStatusUpdateRequest Request);

        #endregion

        #region Authorize

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        event OnAuthorizeResponseDelegate  OnAuthorizeResponse;


        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public Task<AuthorizeResponse> Authorize(AuthorizeRequest Request);

        #endregion

        #region NotifyEVChargingNeeds

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestDelegate   OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseDelegate  OnNotifyEVChargingNeedsResponse;


        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="Request">A notify EV charging needs request.</param>
        public Task<NotifyEVChargingNeedsResponse> NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest Request);

        #endregion

        #region SendTransactionEvent

        /// <summary>
        /// An event fired whenever a transaction event will be sent to the CSMS.
        /// </summary>
        event OnTransactionEventRequestDelegate   OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a response to a transaction event request was received.
        /// </summary>
        event OnTransactionEventResponseDelegate  OnTransactionEventResponse;


        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A transaction event request.</param>
        public Task<TransactionEventResponse> SendTransactionEvent(TransactionEventRequest Request);

        #endregion

        #region SendStatusNotification

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;


        /// <summary>
        /// Send a status notification for the given evse and connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public Task<StatusNotificationResponse> SendStatusNotification(StatusNotificationRequest Request);

        #endregion

        #region SendMeterValues

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the CSMS.
        /// </summary>
        event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        event OnMeterValuesResponseDelegate  OnMeterValuesResponse;


        /// <summary>
        /// Send a meter values for the given evse.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public Task<MeterValuesResponse> SendMeterValues(MeterValuesRequest Request);

        #endregion

        #region NotifyChargingLimit

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        event OnNotifyChargingLimitRequestDelegate   OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        event OnNotifyChargingLimitResponseDelegate  OnNotifyChargingLimitResponse;


        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<NotifyChargingLimitResponse> NotifyChargingLimit(NotifyChargingLimitRequest Request);

        #endregion

        #region SendClearedChargingLimit

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        event OnClearedChargingLimitRequestDelegate   OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        event OnClearedChargingLimitResponseDelegate  OnClearedChargingLimitResponse;


        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A notify charging limit request.</param>
        public Task<ClearedChargingLimitResponse> SendClearedChargingLimit(ClearedChargingLimitRequest Request);

        #endregion

        #region ReportChargingProfiles

        /// <summary>
        /// An event fired whenever a report charging profiles request will be sent to the CSMS.
        /// </summary>
        event OnReportChargingProfilesRequestDelegate   OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a report charging profiles request was received.
        /// </summary>
        event OnReportChargingProfilesResponseDelegate  OnReportChargingProfilesResponse;


        /// <summary>
        /// Report about charging profiles.
        /// </summary>
        /// <param name="Request">A report charging profiles request.</param>
        public Task<ReportChargingProfilesResponse> ReportChargingProfiles(ReportChargingProfilesRequest Request);

        #endregion


        #region NotifyDisplayMessages

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        event OnNotifyDisplayMessagesRequestDelegate   OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesResponseDelegate  OnNotifyDisplayMessagesResponse;


        /// <summary>
        /// Notify about display messages.
        /// </summary>
        /// <param name="Request">A notify display messages request.</param>
        public Task<NotifyDisplayMessagesResponse> NotifyDisplayMessages(NotifyDisplayMessagesRequest Request);

        #endregion

        #region NotifyCustomerInformation

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        event OnNotifyCustomerInformationRequestDelegate   OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        event OnNotifyCustomerInformationResponseDelegate  OnNotifyCustomerInformationResponse;


        /// <summary>
        /// Notify about customer information.
        /// </summary>
        /// <param name="Request">A notify customer information request.</param>
        public Task<NotifyCustomerInformationResponse> NotifyCustomerInformation(NotifyCustomerInformationRequest Request);

        #endregion


    }

}
