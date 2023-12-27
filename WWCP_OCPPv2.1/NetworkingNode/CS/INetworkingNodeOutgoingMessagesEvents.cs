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

#region

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface INetworkingNodeOutgoingMessagesEvents
    {

        #region SendBootNotification                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the CSMS.
        /// </summary>
        event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        #endregion

        //#region SendFirmwareStatusNotification           (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a FirmwareStatusNotification request was received.
        ///// </summary>
        //event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        //#endregion

        //#region SendPublishFirmwareStatusNotification    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //event OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        ///// </summary>
        //event OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        //#endregion

        //#region SendHeartbeat                            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Heartbeat request will be sent to the CSMS.
        ///// </summary>
        //event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Heartbeat request was received.
        ///// </summary>
        //event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        //#endregion

        //#region NotifyEvent                              (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEvent request was received.
        ///// </summary>
        //event OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        //#endregion

        //#region SendSecurityEventNotification            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        ///// </summary>
        //event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SecurityEventNotification request was received.
        ///// </summary>
        //event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        //#endregion

        //#region NotifyReport                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyReport request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyReport request was received.
        ///// </summary>
        //event OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        //#endregion

        //#region NotifyMonitoringReport                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyMonitoringReport request was received.
        ///// </summary>
        //event OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        //#endregion

        //#region SendLogStatusNotification                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a LogStatusNotification request was received.
        ///// </summary>
        //event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        //#endregion

        //#region TransferData                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DataTransfer request will be sent to the CSMS.
        ///// </summary>
        //event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DataTransfer request was received.
        ///// </summary>
        //event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        //#endregion


        //#region SignCertificate                          (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SignCertificate request will be sent to the CSMS.
        ///// </summary>
        //event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SignCertificate request was received.
        ///// </summary>
        //event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        //#endregion

        //#region Get15118EVCertificate                    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        ///// </summary>
        //event OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Get15118EVCertificate request was received.
        ///// </summary>
        //event OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        //#endregion

        //#region GetCertificateStatus                     (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        ///// </summary>
        //event OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetCertificateStatus request was received.
        ///// </summary>
        //event OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        //#endregion

        //#region GetCRL                                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        ///// </summary>
        //event OnGetCRLRequestDelegate?   OnGetCRLRequest;

        ///// <summary>
        ///// An event fired whenever a response to a get certificate revocation list request was received.
        ///// </summary>
        //event OnGetCRLResponseDelegate?  OnGetCRLResponse;

        //#endregion


        //#region SendReservationStatusUpdate              (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        ///// </summary>
        //event OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ReservationStatusUpdate request was received.
        ///// </summary>
        //event OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        //#endregion

        //#region Authorize                                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever an Authorize request will be sent to the CSMS.
        ///// </summary>
        //event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        ///// <summary>
        ///// An event fired whenever a response to an Authorize request was received.
        ///// </summary>
        //event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        //#endregion

        //#region NotifyEVChargingNeeds                    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        ///// </summary>
        //event OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        //#endregion

        //#region SendTransactionEvent                     (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a TransactionEvent will be sent to the CSMS.
        ///// </summary>
        //event OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        ///// <summary>
        ///// An event fired whenever a response to a TransactionEvent request was received.
        ///// </summary>
        //event OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        //#endregion

        //#region SendStatusNotification                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a StatusNotification request will be sent to the CSMS.
        ///// </summary>
        //event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a StatusNotification request was received.
        ///// </summary>
        //event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        //#endregion

        //#region SendMeterValues                          (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a MeterValues request will be sent to the CSMS.
        ///// </summary>
        //event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a MeterValues request was received.
        ///// </summary>
        //event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        //#endregion

        //#region NotifyChargingLimit                      (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyChargingLimit request was received.
        ///// </summary>
        //event OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        //#endregion

        //#region SendClearedChargingLimit                 (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        ///// </summary>
        //event OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearedChargingLimit request was received.
        ///// </summary>
        //event OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        //#endregion

        //#region ReportChargingProfiles                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        ///// </summary>
        //event OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ReportChargingProfiles request was received.
        ///// </summary>
        //event OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        //#endregion

        //#region NotifyEVChargingSchedule                 (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        ///// </summary>
        //event OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        //#endregion

        //#region NotifyPriorityCharging                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyPriorityCharging request was received.
        ///// </summary>
        //event OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        //#endregion

        //#region PullDynamicScheduleUpdate                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        ///// </summary>
        //event OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        ///// </summary>
        //event OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        //#endregion


        //#region NotifyDisplayMessages                    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyDisplayMessages request was received.
        ///// </summary>
        //event OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        //#endregion

        //#region NotifyCustomerInformation                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        ///// </summary>
        //event OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyCustomerInformation request was received.
        ///// </summary>
        //event OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        //#endregion


        //// Binary Data Streams Extensions

        //#region TransferBinaryData                       (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        ///// </summary>
        //event OnBinaryDataTransferRequestDelegate?   OnBinaryDataTransferRequest;

        ///// <summary>
        ///// An event fired whenever a response to a BinaryDataTransfer request was received.
        ///// </summary>
        //event OnBinaryDataTransferResponseDelegate?  OnBinaryDataTransferResponse;

        //#endregion


    }

}
