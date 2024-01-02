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

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// Extension methods for all CSMS servers.
    /// </summary>
    public static class ICSMSServerExtensions
    {



    }


    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSServer : IEventSender
    {

        #region Properties

        /// <summary>
        /// The unique identifications of all connected charge boxes.
        /// </summary>
        IEnumerable<ChargeBox_Id>  ChargeBoxIds    { get; }

        #endregion


        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event OnBootNotificationRequestDelegate    OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        event OnBootNotificationDelegate           OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification request was sent.
        /// </summary>
        event OnBootNotificationResponseDelegate   OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationDelegate           OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate   OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestDelegate    OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationDelegate           OnPublishFirmwareStatusNotification;

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
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        event OnHeartbeatDelegate           OnHeartbeat;

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
        /// An event sent whenever a notify event was received.
        /// </summary>
        event OnNotifyEventDelegate           OnNotifyEvent;

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
        /// An event sent whenever a security event notification was received.
        /// </summary>
        event OnSecurityEventNotificationDelegate           OnSecurityEventNotification;

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
        /// An event sent whenever a notify report was received.
        /// </summary>
        event OnNotifyReportDelegate           OnNotifyReport;

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
        /// An event sent whenever a notify monitoring report was received.
        /// </summary>
        event OnNotifyMonitoringReportDelegate           OnNotifyMonitoringReport;

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
        /// An event sent whenever a log status notification was received.
        /// </summary>
        event OnLogStatusNotificationDelegate           OnLogStatusNotification;

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
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

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
        /// An event sent whenever a sign certificate was received.
        /// </summary>
        event OnSignCertificateDelegate           OnSignCertificate;

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
        /// An event sent whenever a get 15118 EV certificate was received.
        /// </summary>
        event OnGet15118EVCertificateDelegate           OnGet15118EVCertificate;

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
        /// An event sent whenever a get certificate status was received.
        /// </summary>
        event OnGetCertificateStatusDelegate           OnGetCertificateStatus;

        /// <summary>
        /// An event sent whenever a response to a get certificate status was sent.
        /// </summary>
        event OnGetCertificateStatusResponseDelegate   OnGetCertificateStatusResponse;

        #endregion


        #region OnReservationStatusUpdate

        /// <summary>
        /// An event sent whenever a reservation status update request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestDelegate    OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a reservation status update was received.
        /// </summary>
        event OnReservationStatusUpdateDelegate           OnReservationStatusUpdate;

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
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeDelegate           OnAuthorize;

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
        /// An event sent whenever a notify EV charging needs was received.
        /// </summary>
        event OnNotifyEVChargingNeedsDelegate           OnNotifyEVChargingNeeds;

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
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event OnTransactionEventDelegate           OnTransactionEvent;

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
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationDelegate           OnStatusNotification;

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
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesDelegate           OnMeterValues;

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
        /// An event sent whenever a notify charging limit was received.
        /// </summary>
        event OnNotifyChargingLimitDelegate           OnNotifyChargingLimit;

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
        /// An event sent whenever a cleared charging limit was received.
        /// </summary>
        event OnClearedChargingLimitDelegate           OnClearedChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a cleared charging limit was sent.
        /// </summary>
        event OnClearedChargingLimitResponseDelegate   OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles

        /// <summary>
        /// An event sent whenever a report charging profiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestDelegate    OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a report charging profiles was received.
        /// </summary>
        event OnReportChargingProfilesDelegate           OnReportChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a report charging profiles was sent.
        /// </summary>
        event OnReportChargingProfilesResponseDelegate   OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestDelegate    OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        event OnNotifyEVChargingScheduleDelegate           OnNotifyEVChargingSchedule;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseDelegate   OnNotifyEVChargingScheduleResponse;

        #endregion


        #region OnNotifyDisplayMessages

        /// <summary>
        /// An event sent whenever a notify display messages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestDelegate    OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a notify display messages was received.
        /// </summary>
        event OnNotifyDisplayMessagesDelegate           OnNotifyDisplayMessages;

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
        /// An event sent whenever a notify customer information was received.
        /// </summary>
        event OnNotifyCustomerInformationDelegate           OnNotifyCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a notify customer information was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseDelegate   OnNotifyCustomerInformationResponse;

        #endregion


    }

}
