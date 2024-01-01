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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface ICSOutgoingMessagesEvents : OCPP.CS.ICSOutgoingMessagesEvents
    {

        #region SendBootNotification                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the CSMS.
        /// </summary>
        event OnBootNotificationRequestSentDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        event OnBootNotificationResponseReceivedDelegate?  OnBootNotificationResponse;

        #endregion

        #region SendFirmwareStatusNotification        (Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        event OnFirmwareStatusNotificationRequestSentDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseReceivedDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region SendPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestSentDelegate?   OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?  OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region SendHeartbeat                         (Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        event OnHeartbeatRequestSentDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseReceivedDelegate?  OnHeartbeatResponse;

        #endregion

        #region NotifyEvent                           (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        /// </summary>
        event OnNotifyEventRequestSentDelegate?   OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        event OnNotifyEventResponseReceivedDelegate?  OnNotifyEventResponse;

        #endregion

        #region SendSecurityEventNotification         (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        /// </summary>
        event OnSecurityEventNotificationRequestSentDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        event OnSecurityEventNotificationResponseReceivedDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region NotifyReport                          (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyReport request will be sent to the CSMS.
        /// </summary>
        event OnNotifyReportRequestSentDelegate?   OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        event OnNotifyReportResponseReceivedDelegate?  OnNotifyReportResponse;

        #endregion

        #region NotifyMonitoringReport                (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        /// </summary>
        event OnNotifyMonitoringReportRequestSentDelegate?   OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        event OnNotifyMonitoringReportResponseReceivedDelegate?  OnNotifyMonitoringReportResponse;

        #endregion

        #region SendLogStatusNotification             (Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        /// </summary>
        event OnLogStatusNotificationRequestSentDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        event OnLogStatusNotificationResponseReceivedDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region TransferData                          (Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        event OnDataTransferRequestSentDelegate?     OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseReceivedDelegate?    OnDataTransferResponse;

        #endregion


        #region SignCertificate                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request will be sent to the CSMS.
        /// </summary>
        event OnSignCertificateRequestSentDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateResponseReceivedDelegate?  OnSignCertificateResponse;

        #endregion

        #region Get15118EVCertificate                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        /// </summary>
        event OnGet15118EVCertificateRequestSentDelegate?   OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        event OnGet15118EVCertificateResponseReceivedDelegate?  OnGet15118EVCertificateResponse;

        #endregion

        #region GetCertificateStatus                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        /// </summary>
        event OnGetCertificateStatusRequestSentDelegate?   OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        event OnGetCertificateStatusResponseReceivedDelegate?  OnGetCertificateStatusResponse;

        #endregion

        #region GetCRL                                (Request/-Response)

        /// <summary>
        /// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        /// </summary>
        event OnGetCRLRequestSentDelegate?   OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        event OnGetCRLResponseReceivedDelegate?  OnGetCRLResponse;

        #endregion


        #region SendReservationStatusUpdate           (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        /// </summary>
        event OnReservationStatusUpdateRequestSentDelegate?   OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        event OnReservationStatusUpdateResponseReceivedDelegate?  OnReservationStatusUpdateResponse;

        #endregion

        #region Authorize                             (Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        event OnAuthorizeRequestSentDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        event OnAuthorizeResponseReceivedDelegate?  OnAuthorizeResponse;

        #endregion

        #region NotifyEVChargingNeeds                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestSentDelegate?   OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseReceivedDelegate?  OnNotifyEVChargingNeedsResponse;

        #endregion

        #region SendTransactionEvent                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a TransactionEvent will be sent to the CSMS.
        /// </summary>
        event OnTransactionEventRequestSentDelegate?   OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        event OnTransactionEventResponseReceivedDelegate?  OnTransactionEventResponse;

        #endregion

        #region SendStatusNotification                (Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request will be sent to the CSMS.
        /// </summary>
        event OnStatusNotificationRequestSentDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationResponseReceivedDelegate?  OnStatusNotificationResponse;

        #endregion

        #region SendMeterValues                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request will be sent to the CSMS.
        /// </summary>
        event OnMeterValuesRequestSentDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        event OnMeterValuesResponseReceivedDelegate?  OnMeterValuesResponse;

        #endregion

        #region NotifyChargingLimit                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        /// </summary>
        event OnNotifyChargingLimitRequestSentDelegate?   OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        event OnNotifyChargingLimitResponseReceivedDelegate?  OnNotifyChargingLimitResponse;

        #endregion

        #region SendClearedChargingLimit              (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        /// </summary>
        event OnClearedChargingLimitRequestSentDelegate?   OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        event OnClearedChargingLimitResponseReceivedDelegate?  OnClearedChargingLimitResponse;

        #endregion

        #region ReportChargingProfiles                (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        event OnReportChargingProfilesRequestSentDelegate?   OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesResponseReceivedDelegate?  OnReportChargingProfilesResponse;

        #endregion

        #region NotifyEVChargingSchedule              (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestSentDelegate?   OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseReceivedDelegate?  OnNotifyEVChargingScheduleResponse;

        #endregion

        #region NotifyPriorityCharging                (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        event OnNotifyPriorityChargingRequestSentDelegate?   OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingResponseReceivedDelegate?  OnNotifyPriorityChargingResponse;

        #endregion

        #region PullDynamicScheduleUpdate             (Request/-Response)

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestSentDelegate?   OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseReceivedDelegate?  OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region NotifyDisplayMessages                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        /// </summary>
        event OnNotifyDisplayMessagesRequestSentDelegate?   OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesResponseReceivedDelegate?  OnNotifyDisplayMessagesResponse;

        #endregion

        #region NotifyCustomerInformation             (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        /// </summary>
        event OnNotifyCustomerInformationRequestSentDelegate?   OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        event OnNotifyCustomerInformationResponseReceivedDelegate?  OnNotifyCustomerInformationResponse;

        #endregion


    }

}
