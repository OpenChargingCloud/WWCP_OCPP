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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD : IOCPPWebSocketAdapterFORWARD
    {

        #region Data

        private readonly INetworkingNode parentNetworkingNode;

        #endregion

        #region Properties

        public ForwardingResult                DefaultResult        { get; set; } = ForwardingResult.DROP;

        public IEnumerable<NetworkingNode_Id>  AnycastIdsAllowed    { get; }      = [];

        public IEnumerable<NetworkingNode_Id>  AnycastIdsDenied     { get; }      = [];

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

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for forwarding messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// <param name="DefaultResult">The default forwarding result.</param>
        public OCPPWebSocketAdapterFORWARD(INetworkingNode   NetworkingNode,
                                           ForwardingResult  DefaultResult   = ForwardingResult.DROP)
        {

            this.parentNetworkingNode  = NetworkingNode;
            this.DefaultResult         = DefaultResult;

        }

        #endregion



        public async Task ProcessJSONRequestMessage(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            if (AnycastIdsAllowed.Any() && !AnycastIdsAllowed.Contains(JSONRequestMessage.DestinationNodeId))
                return;

            if (AnycastIdsDenied. Any() &&  AnycastIdsDenied. Contains(JSONRequestMessage.DestinationNodeId))
                return;



        }

        public async Task ProcessJSONResponseMessage(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            

        }

        public async Task ProcessJSONErrorMessage(OCPP_JSONErrorMessage JSONErrorMessage)
        {

            

        }



        public async Task ProcessBinaryRequestMessage(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            if (AnycastIdsAllowed.Any() && !AnycastIdsAllowed.Contains(BinaryRequestMessage.DestinationNodeId))
                return;

            if (AnycastIdsDenied. Any() &&  AnycastIdsDenied. Contains(BinaryRequestMessage.DestinationNodeId))
                return;

        }

        public async Task ProcessBinaryResponseMessage(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            

        }





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
