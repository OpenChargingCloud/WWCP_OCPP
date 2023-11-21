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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSServer : ICSMSServerLogger
    {

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        event OnBootNotificationDelegate           OnBootNotification;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationDelegate           OnFirmwareStatusNotification;

        #endregion

        #region OnPublishFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a publish firmware status notification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationDelegate           OnPublishFirmwareStatusNotification;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        event OnHeartbeatDelegate           OnHeartbeat;

        #endregion

        #region OnNotifyEvent

        /// <summary>
        /// An event sent whenever a notify event was received.
        /// </summary>
        event OnNotifyEventDelegate           OnNotifyEvent;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a security event notification was received.
        /// </summary>
        event OnSecurityEventNotificationDelegate           OnSecurityEventNotification;

        #endregion

        #region OnNotifyReport

        /// <summary>
        /// An event sent whenever a notify report was received.
        /// </summary>
        event OnNotifyReportDelegate           OnNotifyReport;

        #endregion

        #region OnNotifyMonitoringReport

        /// <summary>
        /// An event sent whenever a notify monitoring report was received.
        /// </summary>
        event OnNotifyMonitoringReportDelegate           OnNotifyMonitoringReport;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a log status notification was received.
        /// </summary>
        event OnLogStatusNotificationDelegate           OnLogStatusNotification;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        #endregion


        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a sign certificate was received.
        /// </summary>
        event OnSignCertificateDelegate           OnSignCertificate;

        #endregion

        #region OnGet15118EVCertificate

        /// <summary>
        /// An event sent whenever a get 15118 EV certificate was received.
        /// </summary>
        event OnGet15118EVCertificateDelegate           OnGet15118EVCertificate;

        #endregion

        #region OnGetCertificateStatus

        /// <summary>
        /// An event sent whenever a get certificate status was received.
        /// </summary>
        event OnGetCertificateStatusDelegate           OnGetCertificateStatus;

        #endregion

        #region OnGetCRL

        /// <summary>
        /// An event sent whenever a get certificate revocation list was received.
        /// </summary>
        event OnGetCRLDelegate           OnGetCRL;

        #endregion


        #region OnReservationStatusUpdate

        /// <summary>
        /// An event sent whenever a reservation status update was received.
        /// </summary>
        event OnReservationStatusUpdateDelegate           OnReservationStatusUpdate;

        #endregion

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeDelegate           OnAuthorize;

        #endregion

        #region OnNotifyEVChargingNeeds

        /// <summary>
        /// An event sent whenever a notify EV charging needs was received.
        /// </summary>
        event OnNotifyEVChargingNeedsDelegate           OnNotifyEVChargingNeeds;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a transaction event request was received.
        /// </summary>
        event OnTransactionEventDelegate           OnTransactionEvent;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationDelegate           OnStatusNotification;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesDelegate           OnMeterValues;

        #endregion

        #region OnNotifyChargingLimit

        /// <summary>
        /// An event sent whenever a notify charging limit was received.
        /// </summary>
        event OnNotifyChargingLimitDelegate           OnNotifyChargingLimit;

        #endregion

        #region OnClearedChargingLimit

        /// <summary>
        /// An event sent whenever a cleared charging limit was received.
        /// </summary>
        event OnClearedChargingLimitDelegate           OnClearedChargingLimit;

        #endregion

        #region OnReportChargingProfiles

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        event OnReportChargingProfilesDelegate           OnReportChargingProfiles;

        #endregion

        #region OnNotifyEVChargingSchedule

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        event OnNotifyEVChargingScheduleDelegate           OnNotifyEVChargingSchedule;

        #endregion

        #region OnNotifyPriorityCharging

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging was received.
        /// </summary>
        event OnNotifyPriorityChargingDelegate           OnNotifyPriorityCharging;

        #endregion

        #region OnPullDynamicScheduleUpdate

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateDelegate           OnPullDynamicScheduleUpdate;

        #endregion


        #region OnNotifyDisplayMessages

        /// <summary>
        /// An event sent whenever a notify display messages was received.
        /// </summary>
        event OnNotifyDisplayMessagesDelegate           OnNotifyDisplayMessages;

        #endregion

        #region OnNotifyCustomerInformation

        /// <summary>
        /// An event sent whenever a notify customer information was received.
        /// </summary>
        event OnNotifyCustomerInformationDelegate           OnNotifyCustomerInformation;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever a binary data transfer request was received.
        /// </summary>
        event OnIncomingBinaryDataTransferDelegate   OnIncomingBinaryDataTransfer;

        #endregion


    }

}
