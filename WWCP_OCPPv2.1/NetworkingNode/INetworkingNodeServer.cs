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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all NetworkingNode servers.
    /// </summary>
    public interface INetworkingNodeServer : INetworkingNodeServerLogger
    {

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        event CSMS.OnBootNotificationDelegate           OnBootNotification;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event CSMS.OnFirmwareStatusNotificationDelegate           OnFirmwareStatusNotification;

        #endregion

        #region OnPublishFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event CSMS.OnPublishFirmwareStatusNotificationDelegate           OnPublishFirmwareStatusNotification;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        event CSMS.OnHeartbeatDelegate           OnHeartbeat;

        #endregion

        #region OnNotifyEvent

        /// <summary>
        /// An event sent whenever a notify event was received.
        /// </summary>
        event CSMS.OnNotifyEventDelegate           OnNotifyEvent;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a security event notification was received.
        /// </summary>
        event CSMS.OnSecurityEventNotificationDelegate           OnSecurityEventNotification;

        #endregion

        #region OnNotifyReport

        /// <summary>
        /// An event sent whenever a notify report was received.
        /// </summary>
        event CSMS.OnNotifyReportDelegate           OnNotifyReport;

        #endregion

        #region OnNotifyMonitoringReport

        /// <summary>
        /// An event sent whenever a notify monitoring report was received.
        /// </summary>
        event CSMS.OnNotifyMonitoringReportDelegate           OnNotifyMonitoringReport;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a log status notification was received.
        /// </summary>
        event CSMS.OnLogStatusNotificationDelegate           OnLogStatusNotification;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event CSMS.OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        #endregion


        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a sign certificate was received.
        /// </summary>
        event CSMS.OnSignCertificateDelegate           OnSignCertificate;

        #endregion

        #region OnGet15118EVCertificate

        /// <summary>
        /// An event sent whenever a get 15118 EV certificate was received.
        /// </summary>
        event CSMS.OnGet15118EVCertificateDelegate           OnGet15118EVCertificate;

        #endregion

        #region OnGetCertificateStatus

        /// <summary>
        /// An event sent whenever a get certificate status was received.
        /// </summary>
        event CSMS.OnGetCertificateStatusDelegate           OnGetCertificateStatus;

        #endregion

        #region OnGetCRL

        /// <summary>
        /// An event sent whenever a get certificate revocation list was received.
        /// </summary>
        event CSMS.OnGetCRLDelegate           OnGetCRL;

        #endregion


        #region OnReservationStatusUpdate

        /// <summary>
        /// An event sent whenever a reservation status update was received.
        /// </summary>
        event CSMS.OnReservationStatusUpdateDelegate           OnReservationStatusUpdate;

        #endregion

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event CSMS.OnAuthorizeDelegate           OnAuthorize;

        #endregion

        #region OnNotifyEVChargingNeeds

        /// <summary>
        /// An event sent whenever a notify EV charging needs was received.
        /// </summary>
        event CSMS.OnNotifyEVChargingNeedsDelegate           OnNotifyEVChargingNeeds;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event CSMS.OnTransactionEventDelegate           OnTransactionEvent;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event CSMS.OnStatusNotificationDelegate           OnStatusNotification;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event CSMS.OnMeterValuesDelegate           OnMeterValues;

        #endregion

        #region OnNotifyChargingLimit

        /// <summary>
        /// An event sent whenever a notify charging limit was received.
        /// </summary>
        event CSMS.OnNotifyChargingLimitDelegate           OnNotifyChargingLimit;

        #endregion

        #region OnClearedChargingLimit

        /// <summary>
        /// An event sent whenever a cleared charging limit was received.
        /// </summary>
        event CSMS.OnClearedChargingLimitDelegate           OnClearedChargingLimit;

        #endregion

        #region OnReportChargingProfiles

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        event CSMS.OnReportChargingProfilesDelegate           OnReportChargingProfiles;

        #endregion

        #region OnNotifyEVChargingSchedule

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        event CSMS.OnNotifyEVChargingScheduleDelegate           OnNotifyEVChargingSchedule;

        #endregion

        #region OnNotifyPriorityCharging

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging was received.
        /// </summary>
        event CSMS.OnNotifyPriorityChargingDelegate           OnNotifyPriorityCharging;

        #endregion

        #region OnPullDynamicScheduleUpdate

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate was received.
        /// </summary>
        event CSMS.OnPullDynamicScheduleUpdateDelegate           OnPullDynamicScheduleUpdate;

        #endregion


        #region OnNotifyDisplayMessages

        /// <summary>
        /// An event sent whenever a notify display messages was received.
        /// </summary>
        event CSMS.OnNotifyDisplayMessagesDelegate           OnNotifyDisplayMessages;

        #endregion

        #region OnNotifyCustomerInformation

        /// <summary>
        /// An event sent whenever a notify customer information was received.
        /// </summary>
        event CSMS.OnNotifyCustomerInformationDelegate           OnNotifyCustomerInformation;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever a binary data transfer request was received.
        /// </summary>
        event CSMS.OnIncomingBinaryDataTransferDelegate OnIncomingBinaryDataTransfer;

        #endregion


    }

}
