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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface INetworkingNodeIncomingMessageEvents
    {

        // Incoming requests

        #region Certificates

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate request was received.
        /// </summary>
        event OnGet15118EVCertificateRequestReceivedDelegate                OnGet15118EVCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        event OnGetCertificateStatusRequestReceivedDelegate                 OnGetCertificateStatusRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCRL request was received.
        /// </summary>
        event OnGetCRLRequestReceivedDelegate                               OnGetCRLRequestReceived;

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateRequestReceivedDelegate                      OnSignCertificateRequestReceived;

        #endregion

        #region Charging

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        event OnAuthorizeRequestReceivedDelegate                            OnAuthorizeRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit request was received.
        /// </summary>
        event OnClearedChargingLimitRequestReceivedDelegate                 OnClearedChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        event OnMeterValuesRequestReceivedDelegate                          OnMeterValuesRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        event OnNotifyChargingLimitRequestReceivedDelegate                  OnNotifyChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsRequestReceivedDelegate                OnNotifyEVChargingNeedsRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleRequestReceivedDelegate             OnNotifyEVChargingScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingRequestReceivedDelegate               OnNotifyPriorityChargingRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifySettlement request was received.
        /// </summary>
        event OnNotifySettlementRequestReceivedDelegate                     OnNotifySettlementRequestReceived;

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestReceivedDelegate            OnPullDynamicScheduleUpdateRequestReceived;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesRequestReceivedDelegate               OnReportChargingProfilesRequestReceived;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        event OnReservationStatusUpdateRequestReceivedDelegate              OnReservationStatusUpdateRequestReceived;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationRequestReceivedDelegate                   OnStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        event OnTransactionEventRequestReceivedDelegate                     OnTransactionEventRequestReceived;

        #endregion

        #region Customer

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation request was received.
        /// </summary>
        event OnNotifyCustomerInformationRequestReceivedDelegate            OnNotifyCustomerInformationRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesRequestReceivedDelegate                OnNotifyDisplayMessagesRequestReceived;

        #endregion

        #region Firmware

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        event OnBootNotificationRequestReceivedDelegate                     OnBootNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestReceivedDelegate           OnFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestReceivedDelegate                            OnHeartbeatRequestReceived;

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestReceivedDelegate    OnPublishFirmwareStatusNotificationRequestReceived;

        #endregion

        #region Monitoring

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestReceivedDelegate                OnLogStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a notify event request was received.
        /// </summary>
        event OnNotifyEventRequestReceivedDelegate                          OnNotifyEventRequestReceived;

        /// <summary>
        /// An event sent whenever a notify monitoring report request was received.
        /// </summary>
        event OnNotifyMonitoringReportRequestReceivedDelegate               OnNotifyMonitoringReportRequestReceived;

        /// <summary>
        /// An event sent whenever a notify report request was received.
        /// </summary>
        event OnNotifyReportRequestReceivedDelegate                         OnNotifyReportRequestReceived;

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestReceivedDelegate            OnSecurityEventNotificationRequestReceived;

        #endregion

        ///// <summary>
        ///// An event sent whenever a data transfer request was received.
        ///// </summary>
        //event OnIncomingDataTransferRequestDelegate     OnIncomingDataTransferRequest;


        // Incoming responses

        #region Certificates

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseReceivedDelegate?              OnCertificateSignedResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseReceivedDelegate?              OnDeleteCertificateResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseReceivedDelegate?     OnGetInstalledCertificateIdsResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseReceivedDelegate?             OnInstallCertificateResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseReceivedDelegate?                      OnNotifyCRLResponseReceived;

        #endregion

        #region Charging

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseReceivedDelegate?              OnCancelReservationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseReceivedDelegate?           OnClearChargingProfileResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseReceivedDelegate?            OnGetChargingProfilesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseReceivedDelegate?           OnGetCompositeScheduleResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseReceivedDelegate?           OnGetTransactionStatusResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?    OnNotifyAllowedEnergyTransferResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseReceivedDelegate?        OnRequestStartTransactionResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseReceivedDelegate?         OnRequestStopTransactionResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseReceivedDelegate?                     OnReserveNowResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseReceivedDelegate?             OnSetChargingProfileResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseReceivedDelegate?                OnUnlockConnectorResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleResponseReceivedDelegate?          OnUpdateDynamicScheduleResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingResponseReceivedDelegate?            OnUsePriorityChargingResponseReceived;

        #endregion

        #region Customer

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseReceivedDelegate?            OnClearDisplayMessageResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseReceivedDelegate?                    OnCostUpdatedResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseReceivedDelegate?            OnCustomerInformationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseReceivedDelegate?             OnGetDisplayMessagesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseReceivedDelegate?              OnSetDisplayMessageResponseReceived;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseReceivedDelegate?             OnChangeAvailabilityResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseReceivedDelegate?        OnClearVariableMonitoringResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseReceivedDelegate?                  OnGetBaseReportResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseReceivedDelegate?                         OnGetLogResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseReceivedDelegate?            OnGetMonitoringReportResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseReceivedDelegate?                      OnGetReportResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseReceivedDelegate?                   OnGetVariablesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseReceivedDelegate?              OnSetMonitoringBaseResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseReceivedDelegate?             OnSetMonitoringLevelResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseReceivedDelegate?              OnSetNetworkProfileResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseReceivedDelegate?          OnSetVariableMonitoringResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseReceivedDelegate?                   OnSetVariablesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseReceivedDelegate?                 OnTriggerMessageResponseReceived;

        #endregion

        #region Firmware

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseReceivedDelegate?                OnPublishFirmwareResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseReceivedDelegate?                          OnResetResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseReceivedDelegate?              OnUnpublishFirmwareResponseReceived;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseReceivedDelegate?                 OnUpdateFirmwareResponseReceived;

        #endregion

        #region Grid

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseReceivedDelegate?                     OnAFRRSignalResponseReceived;

        #endregion

        #region Local List

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseReceivedDelegate?                     OnClearCacheResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseReceivedDelegate?            OnGetLocalListVersionResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseReceivedDelegate?                  OnSendLocalListResponseReceived;

        #endregion



        #region ChargingTariffs

        event OnSetDefaultChargingTariffResponseReceivedDelegate? OnSetDefaultChargingTariffResponseReceived;

        #endregion


    }

}
