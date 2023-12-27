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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class FORWARD(TestNetworkingNode  NetworkingNode,
                                 ForwardingResult    DefaultResult   = ForwardingResult.DROP)
    {

        #region Data

        private readonly TestNetworkingNode parentNetworkingNode = NetworkingNode;

        #endregion

        #region Properties

        public ForwardingResult DefaultResult { get; set; } = DefaultResult;

        #endregion

        #region Events

        #region Charging Station <- CSMS

        public event OnUpdateFirmwareDelegate? OnUpdateFirmware;
        public event OnPublishFirmwareDelegate? OnPublishFirmware;
        public event OnUnpublishFirmwareDelegate? OnUnpublishFirmware;
        public event OnGetBaseReportDelegate? OnGetBaseReport;
        public event OnGetReportDelegate? OnGetReport;
        public event OnGetLogDelegate? OnGetLog;
        public event OnSetVariablesDelegate? OnSetVariables;
        public event OnGetVariablesDelegate? OnGetVariables;
        public event OnSetMonitoringBaseDelegate? OnSetMonitoringBase;
        public event OnGetMonitoringReportDelegate? OnGetMonitoringReport;
        public event OnSetMonitoringLevelDelegate? OnSetMonitoringLevel;
        public event OnSetVariableMonitoringDelegate? OnSetVariableMonitoring;
        public event OnClearVariableMonitoringDelegate? OnClearVariableMonitoring;
        public event OnSetNetworkProfileDelegate? OnSetNetworkProfile;
        public event OnChangeAvailabilityDelegate? OnChangeAvailability;
        public event OnTriggerMessageDelegate? OnTriggerMessage;
        public event OnIncomingDataTransferDelegate? OnIncomingDataTransfer;
        public event OnCertificateSignedDelegate? OnCertificateSigned;
        public event OnInstallCertificateDelegate? OnInstallCertificate;
        public event OnGetInstalledCertificateIdsDelegate? OnGetInstalledCertificateIds;
        public event OnDeleteCertificateDelegate? OnDeleteCertificate;
        public event OnNotifyCRLDelegate? OnNotifyCRL;
        public event OnGetLocalListVersionDelegate? OnGetLocalListVersion;
        public event OnSendLocalListDelegate? OnSendLocalList;
        public event OnClearCacheDelegate? OnClearCache;
        public event OnReserveNowDelegate? OnReserveNow;
        public event OnCancelReservationDelegate? OnCancelReservation;
        public event OnRequestStartTransactionDelegate? OnRequestStartTransaction;
        public event OnRequestStopTransactionDelegate? OnRequestStopTransaction;
        public event OnGetTransactionStatusDelegate? OnGetTransactionStatus;
        public event OnSetChargingProfileDelegate? OnSetChargingProfile;
        public event OnGetChargingProfilesDelegate? OnGetChargingProfiles;
        public event OnClearChargingProfileDelegate? OnClearChargingProfile;
        public event OnGetCompositeScheduleDelegate? OnGetCompositeSchedule;
        public event OnUpdateDynamicScheduleDelegate? OnUpdateDynamicSchedule;
        public event OnNotifyAllowedEnergyTransferDelegate? OnNotifyAllowedEnergyTransfer;
        public event OnUsePriorityChargingDelegate? OnUsePriorityCharging;
        public event OnUnlockConnectorDelegate? OnUnlockConnector;
        public event OnAFRRSignalDelegate? OnAFRRSignal;
        public event OnSetDisplayMessageDelegate? OnSetDisplayMessage;
        public event OnGetDisplayMessagesDelegate? OnGetDisplayMessages;
        public event OnClearDisplayMessageDelegate? OnClearDisplayMessage;
        public event OnCostUpdatedDelegate? OnCostUpdated;
        public event OnCustomerInformationDelegate? OnCustomerInformation;


        // Binary Data Streams Extensions

        public event OnGetFileDelegate?                      OnGetFile;
        public event OnSendFileDelegate?                     OnSendFile;
        public event OnDeleteFileDelegate?                   OnDeleteFile;


        // E2E Security Extensions

        public event OnAddSignaturePolicyDelegate?           OnAddSignaturePolicy;
        public event OnUpdateSignaturePolicyDelegate?        OnUpdateSignaturePolicy;
        public event OnDeleteSignaturePolicyDelegate?        OnDeleteSignaturePolicy;
        public event OnAddUserRoleDelegate?                  OnAddUserRole;
        public event OnUpdateUserRoleDelegate?               OnUpdateUserRole;
        public event OnDeleteUserRoleDelegate?               OnDeleteUserRole;


        // E2E Charging Tariffs Extensions

        public event OnSetDefaultChargingTariffDelegate?     OnSetDefaultChargingTariff;
        public event OnGetDefaultChargingTariffDelegate?     OnGetDefaultChargingTariff;
        public event OnRemoveDefaultChargingTariffDelegate?  OnRemoveDefaultChargingTariff;

        #endregion

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccured)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
