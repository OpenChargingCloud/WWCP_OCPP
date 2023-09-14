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
    /// The common interface of all charging station servers.
    /// </summary>
    public interface IChargingStationServer : IChargingStationServerLogger
    {

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset was received.
        /// </summary>
        event OnResetDelegate           OnReset;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware was received.
        /// </summary>
        event OnUpdateFirmwareDelegate           OnUpdateFirmware;

        #endregion

        #region OnPublishFirmware

        /// <summary>
        /// An event sent whenever a publish firmware was received.
        /// </summary>
        event OnPublishFirmwareDelegate           OnPublishFirmware;

        #endregion

        #region OnUnpublishFirmware

        /// <summary>
        /// An event sent whenever an unpublish firmware was received.
        /// </summary>
        event OnUnpublishFirmwareDelegate           OnUnpublishFirmware;

        #endregion

        #region OnGetBaseReport

        /// <summary>
        /// An event sent whenever a get base report was received.
        /// </summary>
        event OnGetBaseReportDelegate           OnGetBaseReport;

        #endregion

        #region OnGetReport

        /// <summary>
        /// An event sent whenever a get report was received.
        /// </summary>
        event OnGetReportDelegate           OnGetReport;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log was received.
        /// </summary>
        event OnGetLogDelegate           OnGetLog;

        #endregion

        #region OnSetVariables

        /// <summary>
        /// An event sent whenever a set variables was received.
        /// </summary>
        event OnSetVariablesDelegate           OnSetVariables;

        #endregion

        #region OnGetVariables

        /// <summary>
        /// An event sent whenever a get variables was received.
        /// </summary>
        event OnGetVariablesDelegate           OnGetVariables;

        #endregion

        #region OnSetMonitoringBase

        /// <summary>
        /// An event sent whenever a set monitoring base was received.
        /// </summary>
        event OnSetMonitoringBaseDelegate           OnSetMonitoringBase;

        #endregion

        #region OnGetMonitoringReport

        /// <summary>
        /// An event sent whenever a get monitoring report was received.
        /// </summary>
        event OnGetMonitoringReportDelegate           OnGetMonitoringReport;

        #endregion

        #region OnSetMonitoringLevel

        /// <summary>
        /// An event sent whenever a set monitoring level was received.
        /// </summary>
        event OnSetMonitoringLevelDelegate           OnSetMonitoringLevel;

        #endregion

        #region OnSetVariableMonitoring

        /// <summary>
        /// An event sent whenever a set variable monitoring was received.
        /// </summary>
        event OnSetVariableMonitoringDelegate           OnSetVariableMonitoring;

        #endregion

        #region OnClearVariableMonitoring

        /// <summary>
        /// An event sent whenever a clear variable monitoring was received.
        /// </summary>
        event OnClearVariableMonitoringDelegate           OnClearVariableMonitoring;

        #endregion

        #region OnSetNetworkProfile

        /// <summary>
        /// An event sent whenever a set network profile was received.
        /// </summary>
        event OnSetNetworkProfileDelegate           OnSetNetworkProfile;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability was received.
        /// </summary>
        event OnChangeAvailabilityDelegate           OnChangeAvailability;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message was received.
        /// </summary>
        event OnTriggerMessageDelegate           OnTriggerMessage;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer was received.
        /// </summary>
        event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        #endregion


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed was received.
        /// </summary>
        event OnCertificateSignedDelegate           OnCertificateSigned;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate was received.
        /// </summary>
        event OnInstallCertificateDelegate           OnInstallCertificate;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids was received.
        /// </summary>
        event OnGetInstalledCertificateIdsDelegate           OnGetInstalledCertificateIds;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate was received.
        /// </summary>
        event OnDeleteCertificateDelegate           OnDeleteCertificate;

        #endregion

        #region OnNotifyCRL

        /// <summary>
        /// An event sent whenever a delete certificate was received.
        /// </summary>
        event OnNotifyCRLDelegate           OnNotifyCRL;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a get local list version was received.
        /// </summary>
        event OnGetLocalListVersionDelegate           OnGetLocalListVersion;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a send local list was received.
        /// </summary>
        event OnSendLocalListDelegate           OnSendLocalList;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a clear cache was received.
        /// </summary>
        event OnClearCacheDelegate           OnClearCache;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now was received.
        /// </summary>
        event OnReserveNowDelegate           OnReserveNow;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation was received.
        /// </summary>
        event OnCancelReservationDelegate           OnCancelReservation;

        #endregion

        #region OnRequestStartTransaction

        /// <summary>
        /// An event sent whenever a request start transaction was received.
        /// </summary>
        event OnRequestStartTransactionDelegate           OnRequestStartTransaction;

        #endregion

        #region OnRequestStopTransaction

        /// <summary>
        /// An event sent whenever a request stop transaction was received.
        /// </summary>
        event OnRequestStopTransactionDelegate           OnRequestStopTransaction;

        #endregion

        #region OnGetTransactionStatus

        /// <summary>
        /// An event sent whenever a get transaction status was received.
        /// </summary>
        event OnGetTransactionStatusDelegate           OnGetTransactionStatus;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile was received.
        /// </summary>
        event OnSetChargingProfileDelegate           OnSetChargingProfile;

        #endregion

        #region OnGetChargingProfiles

        /// <summary>
        /// An event sent whenever a get charging profiles was received.
        /// </summary>
        event OnGetChargingProfilesDelegate           OnGetChargingProfiles;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile was received.
        /// </summary>
        event OnClearChargingProfileDelegate           OnClearChargingProfile;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule was received.
        /// </summary>
        event OnGetCompositeScheduleDelegate           OnGetCompositeSchedule;

        #endregion

        #region OnUpdateDynamicSchedule

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule was received.
        /// </summary>
        event OnUpdateDynamicScheduleDelegate           OnUpdateDynamicSchedule;

        #endregion

        #region OnNotifyAllowedEnergyTransfer

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferDelegate           OnNotifyAllowedEnergyTransfer;

        #endregion

        #region OnUsePriorityCharging

        /// <summary>
        /// An event sent whenever an UsePriorityCharging was received.
        /// </summary>
        event OnUsePriorityChargingDelegate           OnUsePriorityCharging;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an UnlockConnector was received.
        /// </summary>
        event OnUnlockConnectorDelegate           OnUnlockConnector;

        #endregion


        #region OnAFRRSignal

        /// <summary>
        /// An event sent whenever an AFRR signal was received.
        /// </summary>
        event OnAFRRSignalDelegate           OnAFRRSignal;

        #endregion


        #region OnSetDisplayMessage

        /// <summary>
        /// An event sent whenever a set display message was received.
        /// </summary>
        event OnSetDisplayMessageDelegate           OnSetDisplayMessage;

        #endregion

        #region OnGetDisplayMessages

        /// <summary>
        /// An event sent whenever a get display messages was received.
        /// </summary>
        event OnGetDisplayMessagesDelegate           OnGetDisplayMessages;

        #endregion

        #region OnClearDisplayMessage

        /// <summary>
        /// An event sent whenever a clear display message was received.
        /// </summary>
        event OnClearDisplayMessageDelegate           OnClearDisplayMessage;

        #endregion

        #region OnCostUpdated

        /// <summary>
        /// An event sent whenever a cost updated was received.
        /// </summary>
        event OnCostUpdatedDelegate           OnCostUpdated;

        #endregion

        #region OnCustomerInformation

        /// <summary>
        /// An event sent whenever a customer information was received.
        /// </summary>
        event OnCustomerInformationDelegate           OnCustomerInformation;

        #endregion


    }

}
