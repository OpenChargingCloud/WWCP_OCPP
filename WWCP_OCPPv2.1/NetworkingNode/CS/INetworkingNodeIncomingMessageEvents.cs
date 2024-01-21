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

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The common interface of all charging station servers.
    /// </summary>
    public interface INetworkingNodeIncomingMessageEvents : OCPP.NN.CS.INetworkingNodeIncomingMessageEvents
    {

        // Incoming requests

        #region Certificates

        /// <summary>
        /// An event sent whenever a CertificateSigned request was received.
        /// </summary>
        event OnCertificateSignedRequestReceivedDelegate              OnCertificateSignedRequestReceived;

        /// <summary>
        /// An event sent whenever a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateRequestReceivedDelegate              OnDeleteCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestReceivedDelegate     OnGetInstalledCertificateIdsRequestReceived;

        /// <summary>
        /// An event sent whenever an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateRequestReceivedDelegate             OnInstallCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyCRLRequest request was received.
        /// </summary>
        event OnNotifyCRLRequestReceivedDelegate                      OnNotifyCRLRequestReceived;

        #endregion

        #region Charging

        /// <summary>
        /// An event sent whenever a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationRequestReceivedDelegate              OnCancelReservationRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileRequestReceivedDelegate           OnClearChargingProfileRequestReceived;

        /// <summary>
        /// An event sent whenever a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesRequestReceivedDelegate            OnGetChargingProfilesRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleRequestReceivedDelegate           OnGetCompositeScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusRequestReceivedDelegate           OnGetTransactionStatusRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestReceivedDelegate    OnNotifyAllowedEnergyTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionRequestReceivedDelegate        OnRequestStartTransactionRequestReceived;

        /// <summary>
        /// An event sent whenever a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionRequestReceivedDelegate         OnRequestStopTransactionRequestReceived;

        /// <summary>
        /// An event sent whenever a ReserveNowRequest was received.
        /// </summary>
        event OnReserveNowRequestReceivedDelegate                     OnReserveNowRequestReceived;

        /// <summary>
        /// An event sent whenever a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileRequestReceivedDelegate             OnSetChargingProfileRequestReceived;

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorRequestReceivedDelegate                OnUnlockConnectorRequestReceived;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleRequestReceivedDelegate          OnUpdateDynamicScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever an UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingRequestReceivedDelegate            OnUsePriorityChargingRequestReceived;

        #endregion

        #region Customer

        /// <summary>
        /// An event sent whenever a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageRequestReceivedDelegate            OnClearDisplayMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedRequestReceivedDelegate                    OnCostUpdatedRequestReceived;

        /// <summary>
        /// An event sent whenever a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationRequestReceivedDelegate            OnCustomerInformationRequestReceived;

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesRequestReceivedDelegate             OnGetDisplayMessagesRequestReceived;

        /// <summary>
        /// An event sent whenever a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageRequestReceivedDelegate              OnSetDisplayMessageRequestReceived;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityRequestReceivedDelegate             OnChangeAvailabilityRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringRequestReceivedDelegate        OnClearVariableMonitoringRequestReceived;

        /// <summary>
        /// An event sent whenever a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportRequestReceivedDelegate                  OnGetBaseReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetLog request was received.
        /// </summary>
        event OnGetLogRequestReceivedDelegate                         OnGetLogRequestReceived;

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportRequestReceivedDelegate            OnGetMonitoringReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetReport request was received.
        /// </summary>
        event OnGetReportRequestReceivedDelegate                      OnGetReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetVariables request was received.
        /// </summary>
        event OnGetVariablesRequestReceivedDelegate                   OnGetVariablesRequestReceived;

        /// <summary>
        /// An event sent whenever a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseRequestReceivedDelegate              OnSetMonitoringBaseRequestReceived;

        /// <summary>
        /// An event sent whenever a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelRequestReceivedDelegate             OnSetMonitoringLevelRequestReceived;

        /// <summary>
        /// An event sent whenever a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileRequestReceivedDelegate              OnSetNetworkProfileRequestReceived;

        /// <summary>
        /// An event sent whenever a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringRequestReceivedDelegate          OnSetVariableMonitoringRequestReceived;

        /// <summary>
        /// An event sent whenever a SetVariables request was received.
        /// </summary>
        event OnSetVariablesRequestReceivedDelegate                   OnSetVariablesRequestReceived;

        /// <summary>
        /// An event sent whenever a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageRequestReceivedDelegate                 OnTriggerMessageRequestReceived;

        #endregion

        #region Firmware

        /// <summary>
        /// An event sent whenever an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareRequestReceivedDelegate              OnUnpublishFirmwareRequestReceived;

        /// <summary>
        /// An event sent whenever a Reset request was received.
        /// </summary>
        event OnResetRequestReceivedDelegate                          OnResetRequestReceived;

        /// <summary>
        /// An event sent whenever a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareRequestReceivedDelegate                OnPublishFirmwareRequestReceived;

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareRequestReceivedDelegate                 OnUpdateFirmwareRequestReceived;

        #endregion

        #region Grid

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        event OnAFRRSignalRequestReceivedDelegate                     OnAFRRSignalRequestReceived;

        #endregion

        #region Local List

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionRequestReceivedDelegate            OnGetLocalListVersionRequestReceived;

        /// <summary>
        /// An event sent whenever a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListRequestReceivedDelegate                  OnSendLocalListRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearCache request was received.
        /// </summary>
        event OnClearCacheRequestReceivedDelegate                     OnClearCacheRequestReceived;

        #endregion


        // Incoming responses

        #region Certificates

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        event OnGet15118EVCertificateResponseReceivedDelegate?                OnGet15118EVCertificateResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        event OnGetCertificateStatusResponseReceivedDelegate?                 OnGetCertificateStatusResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        event OnGetCRLResponseReceivedDelegate?                               OnGetCRLResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateResponseReceivedDelegate?                      OnSignCertificateResponseReceived;

        #endregion

        #region Charging

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        event OnAuthorizeResponseReceivedDelegate?                            OnAuthorizeResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        event OnClearedChargingLimitResponseReceivedDelegate?                 OnClearedChargingLimitResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        event OnMeterValuesResponseReceivedDelegate?                          OnMeterValuesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        event OnNotifyChargingLimitResponseReceivedDelegate?                  OnNotifyChargingLimitResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseReceivedDelegate?                OnNotifyEVChargingNeedsResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseReceivedDelegate?             OnNotifyEVChargingScheduleResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        event OnNotifyPriorityChargingResponseReceivedDelegate?               OnNotifyPriorityChargingResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifySettlement request was received.
        /// </summary>
        event OnNotifySettlementResponseReceivedDelegate?                     OnNotifySettlementResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseReceivedDelegate?            OnPullDynamicScheduleUpdateResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        event OnReportChargingProfilesResponseReceivedDelegate?               OnReportChargingProfilesResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        event OnReservationStatusUpdateResponseReceivedDelegate?              OnReservationStatusUpdateResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationResponseReceivedDelegate?                   OnStatusNotificationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        event OnTransactionEventResponseReceivedDelegate?                     OnTransactionEventResponseReceived;

        #endregion

        #region Customer

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        event OnNotifyCustomerInformationResponseReceivedDelegate?            OnNotifyCustomerInformationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        event OnNotifyDisplayMessagesResponseReceivedDelegate?                OnNotifyDisplayMessagesResponseReceived;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        event OnLogStatusNotificationResponseReceivedDelegate?                OnLogStatusNotificationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        event OnNotifyEventResponseReceivedDelegate?                          OnNotifyEventResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        event OnNotifyMonitoringReportResponseReceivedDelegate?               OnNotifyMonitoringReportResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        event OnNotifyReportResponseReceivedDelegate?                         OnNotifyReportResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        event OnSecurityEventNotificationResponseReceivedDelegate?            OnSecurityEventNotificationResponseReceived;

        #endregion

        #region Firmware

        event OnBootNotificationResponseReceivedDelegate?                     OnBootNotificationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseReceivedDelegate?           OnFirmwareStatusNotificationResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseReceivedDelegate?                            OnHeartbeatResponseReceived;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?    OnPublishFirmwareStatusNotificationResponseReceived;

        #endregion


        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseReceivedDelegate?                         OnDataTransferResponseReceived;


    }

}
