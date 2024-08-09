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

#region

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The common interface of all charging station clients.
    /// </summary>
    public interface INetworkingNodeOutgoingMessageEvents
    {

        // Outgoing requests

        #region Certificates

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request be sent (to the CSMS).
        /// </summary>
        event OnGet15118EVCertificateRequestSentDelegate?                OnGet15118EVCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request be sent (to the CSMS).
        /// </summary>
        event OnGetCertificateStatusRequestSentDelegate?                 OnGetCertificateStatusRequestSent;

        /// <summary>
        /// An event fired whenever a get certificate revocation list request be sent (to the CSMS).
        /// </summary>
        event OnGetCRLRequestSentDelegate?                               OnGetCRLRequestSent;

        /// <summary>
        /// An event fired whenever a SignCertificate request be sent (to the CSMS).
        /// </summary>
        event OnSignCertificateRequestSentDelegate?                      OnSignCertificateRequestSent;

        #endregion

        #region Charging

        /// <summary>
        /// An event fired whenever an Authorize request be sent (to the CSMS).
        /// </summary>
        event OnAuthorizeRequestSentDelegate?                            OnAuthorizeRequestSent;

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request be sent (to the CSMS).
        /// </summary>
        event OnClearedChargingLimitRequestSentDelegate?                 OnClearedChargingLimitRequestSent;

        /// <summary>
        /// An event fired whenever a MeterValues request be sent (to the CSMS).
        /// </summary>
        event OnMeterValuesRequestSentDelegate?                          OnMeterValuesRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request be sent (to the CSMS).
        /// </summary>
        event OnNotifyChargingLimitRequestSentDelegate?                  OnNotifyChargingLimitRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request be sent (to the CSMS).
        /// </summary>
        event OnNotifyEVChargingNeedsRequestSentDelegate?                OnNotifyEVChargingNeedsRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request be sent (to the CSMS).
        /// </summary>
        event OnNotifyEVChargingScheduleRequestSentDelegate?             OnNotifyEVChargingScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request be sent (to the CSMS).
        /// </summary>
        event OnNotifyPriorityChargingRequestSentDelegate?               OnNotifyPriorityChargingRequestSent;

        /// <summary>
        /// An event fired whenever a NotifySettlement request be sent (to the CSMS).
        /// </summary>
        event OnNotifySettlementRequestSentDelegate?                     OnNotifySettlementRequestSent;

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request be sent (to the CSMS).
        /// </summary>
        event OnPullDynamicScheduleUpdateRequestSentDelegate?            OnPullDynamicScheduleUpdateRequestSent;

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request be sent (to the CSMS).
        /// </summary>
        event OnReportChargingProfilesRequestSentDelegate?               OnReportChargingProfilesRequestSent;

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request be sent (to the CSMS).
        /// </summary>
        event OnReservationStatusUpdateRequestSentDelegate?              OnReservationStatusUpdateRequestSent;

        /// <summary>
        /// An event fired whenever a StatusNotification request be sent (to the CSMS).
        /// </summary>
        event OnStatusNotificationRequestSentDelegate?                   OnStatusNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a TransactionEvent be sent (to the CSMS).
        /// </summary>
        event OnTransactionEventRequestSentDelegate?                     OnTransactionEventRequestSent;

        #endregion

        #region Customer

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request be sent (to the CSMS).
        /// </summary>
        event OnNotifyCustomerInformationRequestSentDelegate?            OnNotifyCustomerInformationRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request be sent (to the CSMS).
        /// </summary>
        event OnNotifyDisplayMessagesRequestSentDelegate?                OnNotifyDisplayMessagesRequestSent;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event fired whenever a LogStatusNotification request be sent (to the CSMS).
        /// </summary>
        event OnLogStatusNotificationRequestSentDelegate?                OnLogStatusNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyEvent request be sent (to the CSMS).
        /// </summary>
        event OnNotifyEventRequestSentDelegate?                          OnNotifyEventRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request be sent (to the CSMS).
        /// </summary>
        event OnNotifyMonitoringReportRequestSentDelegate?               OnNotifyMonitoringReportRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyReport request be sent (to the CSMS).
        /// </summary>
        event OnNotifyReportRequestSentDelegate?                         OnNotifyReportRequestSent;

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request be sent (to the CSMS).
        /// </summary>
        event OnSecurityEventNotificationRequestSentDelegate?            OnSecurityEventNotificationRequestSent;

        #endregion

        #region Firmware

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent (to the CSMS).
        /// </summary>
        event OnBootNotificationRequestSentDelegate?                     OnBootNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent (to the CSMS).
        /// </summary>
        event OnFirmwareStatusNotificationRequestSentDelegate?           OnFirmwareStatusNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent (to the CSMS).
        /// </summary>
        event OnHeartbeatRequestSentDelegate?                            OnHeartbeatRequestSent;

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request will be sent (to the CSMS).
        /// </summary>
        event OnPublishFirmwareStatusNotificationRequestSentDelegate?    OnPublishFirmwareStatusNotificationRequestSent;

        #endregion


        // Outgoing responses

        #region Certificates

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        event OnCertificateSignedResponseSentDelegate              OnCertificateSignedResponseSent;

        /// <summary>
        /// An event sent whenever a response to a DeleteCertificate request was sent.
        /// </summary>
        event OnDeleteCertificateResponseSentDelegate              OnDeleteCertificateResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseSentDelegate     OnGetInstalledCertificateIdsResponseSent;

        /// <summary>
        /// An event sent whenever a response to an InstallCertificate request was sent.
        /// </summary>
        event OnInstallCertificateResponseSentDelegate             OnInstallCertificateResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        event OnNotifyCRLResponseSentDelegate                      OnNotifyCRLResponseSent;

        #endregion

        #region Charging

        /// <summary>
        /// An event sent whenever a response to a CancelReservation request was sent.
        /// </summary>
        event OnCancelReservationResponseSentDelegate              OnCancelReservationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        event OnClearChargingProfileResponseSentDelegate           OnClearChargingProfileResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetChargingProfiles request was sent.
        /// </summary>
        event OnGetChargingProfilesResponseSentDelegate            OnGetChargingProfilesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        event OnGetCompositeScheduleResponseSentDelegate           OnGetCompositeScheduleResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        event OnGetTransactionStatusResponseSentDelegate           OnGetTransactionStatusResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseSentDelegate    OnNotifyAllowedEnergyTransferResponseSent;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        event OnRequestStartTransactionResponseSentDelegate        OnRequestStartTransactionResponseSent;

        /// <summary>
        /// An event sent whenever a response to a RequestStopTransaction request was sent.
        /// </summary>
        event OnRequestStopTransactionResponseSentDelegate         OnRequestStopTransactionResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ReserveNow request was sent.
        /// </summary>
        event OnReserveNowResponseSentDelegate                     OnReserveNowResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        event OnSetChargingProfileResponseSentDelegate             OnSetChargingProfileResponseSent;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        event OnUnlockConnectorResponseSentDelegate                OnUnlockConnectorResponseSent;

        /// <summary>
        /// An event sent whenever an response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        event OnUpdateDynamicScheduleResponseSentDelegate          OnUpdateDynamicScheduleResponseSent;

        /// <summary>
        /// An event sent whenever a response to an UsePriorityCharging request was sent.
        /// </summary>
        event OnUsePriorityChargingResponseSentDelegate            OnUsePriorityChargingResponseSent;

        #endregion

        #region Customer

        /// <summary>
        /// An event sent whenever a response to a ClearDisplayMessage request was sent.
        /// </summary>
        event OnClearDisplayMessageResponseSentDelegate            OnClearDisplayMessageResponseSent;

        /// <summary>
        /// An event sent whenever a response to a CostUpdated request was sent.
        /// </summary>
        event OnCostUpdatedResponseSentDelegate                    OnCostUpdatedResponseSent;

        /// <summary>
        /// An event sent whenever a response to a CustomerInformation request was sent.
        /// </summary>
        event OnCustomerInformationResponseSentDelegate            OnCustomerInformationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        event OnGetDisplayMessagesResponseSentDelegate             OnGetDisplayMessagesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetDisplayMessage request was sent.
        /// </summary>
        event OnSetDisplayMessageResponseSentDelegate              OnSetDisplayMessageResponseSent;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event sent whenever a response to a ChangeAvailability request was sent.
        /// </summary>
        event OnChangeAvailabilityResponseSentDelegate             OnChangeAvailabilityResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ClearVariableMonitoring request was sent.
        /// </summary>
        event OnClearVariableMonitoringResponseSentDelegate        OnClearVariableMonitoringResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport request was sent.
        /// </summary>
        event OnGetBaseReportResponseSentDelegate                  OnGetBaseReportResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetLog request was sent.
        /// </summary>
        event OnGetLogResponseSentDelegate                         OnGetLogResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetMonitoringReport request was sent.
        /// </summary>
        event OnGetMonitoringReportResponseSentDelegate            OnGetMonitoringReportResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        event OnGetReportResponseSentDelegate                      OnGetReportResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetVariables request was sent.
        /// </summary>
        event OnGetVariablesResponseSentDelegate                   OnGetVariablesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringBase request was sent.
        /// </summary>
        event OnSetMonitoringBaseResponseSentDelegate              OnSetMonitoringBaseResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringLevel request was sent.
        /// </summary>
        event OnSetMonitoringLevelResponseSentDelegate             OnSetMonitoringLevelResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetNetworkProfile request was sent.
        /// </summary>
        event OnSetNetworkProfileResponseSentDelegate              OnSetNetworkProfileResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetVariableMonitoring request was sent.
        /// </summary>
        event OnSetVariableMonitoringResponseSentDelegate          OnSetVariableMonitoringResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        event OnSetVariablesResponseSentDelegate                   OnSetVariablesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a TriggerMessage request was sent.
        /// </summary>
        event OnTriggerMessageResponseSentDelegate                 OnTriggerMessageResponseSent;

        #endregion

        #region Firmware

        /// <summary>
        /// An event sent whenever a response to an UnpublishFirmware request was sent.
        /// </summary>
        event OnUnpublishFirmwareResponseSentDelegate              OnUnpublishFirmwareResponseSent;

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        event OnResetResponseSentDelegate                          OnResetResponseSent;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        event OnPublishFirmwareResponseSentDelegate                OnPublishFirmwareResponseSent;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        event OnUpdateFirmwareResponseSentDelegate                 OnUpdateFirmwareResponseSent;

        #endregion

        #region Grid

        /// <summary>
        /// An event sent whenever a response to an AFRRSignal request was sent.
        /// </summary>
        event OnAFRRSignalResponseSentDelegate                     OnAFRRSignalResponseSent;

        #endregion

        #region Local List

        /// <summary>
        /// An event sent whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        event OnGetLocalListVersionResponseSentDelegate            OnGetLocalListVersionResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        event OnSendLocalListResponseSentDelegate                  OnSendLocalListResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        event OnClearCacheResponseSentDelegate                     OnClearCacheResponseSent;

        #endregion


    }

}
