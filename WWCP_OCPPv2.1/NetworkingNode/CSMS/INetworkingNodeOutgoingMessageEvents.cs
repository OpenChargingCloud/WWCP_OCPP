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
    /// The common interface of all notworking nodes acting as a CSMS.
    /// </summary>
    public interface INetworkingNodeOutgoingMessageEvents
    {

        // Outgoing requests

        #region Certificates

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be be sent (to the charging station).
        /// </summary>
        event OnCertificateSignedRequestSentDelegate?              OnCertificateSignedRequestSent;

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be be sent (to the charging station).
        /// </summary>
        event OnDeleteCertificateRequestSentDelegate?              OnDeleteCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be be sent (to the charging station).
        /// </summary>
        event OnGetInstalledCertificateIdsRequestSentDelegate?     OnGetInstalledCertificateIdsRequestSent;

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be be sent (to the charging station).
        /// </summary>
        event OnInstallCertificateRequestSentDelegate?             OnInstallCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be be sent (to the charging station).
        /// </summary>
        event OnNotifyCRLRequestSentDelegate?                      OnNotifyCRLRequestSent;

        #endregion

        #region Charging

        /// <summary>
        /// An event fired whenever a CancelReservation request will be be sent (to the charging station).
        /// </summary>
        event OnCancelReservationRequestSentDelegate?              OnCancelReservationRequestSent;

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be be sent (to the charging station).
        /// </summary>
        event OnClearChargingProfileRequestSentDelegate?           OnClearChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be be sent (to the charging station).
        /// </summary>
        event OnGetChargingProfilesRequestSentDelegate?            OnGetChargingProfilesRequestSent;

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be be sent (to the charging station).
        /// </summary>
        event OnGetCompositeScheduleRequestSentDelegate?           OnGetCompositeScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be be sent (to the charging station).
        /// </summary>
        event OnGetTransactionStatusRequestSentDelegate?           OnGetTransactionStatusRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be be sent (to the charging station).
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestSentDelegate?    OnNotifyAllowedEnergyTransferRequestSent;

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be be sent (to the charging station).
        /// </summary>
        event OnRequestStartTransactionRequestSentDelegate?        OnRequestStartTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be be sent (to the charging station).
        /// </summary>
        event OnRequestStopTransactionRequestSentDelegate?         OnRequestStopTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a ReserveNow request will be be sent (to the charging station).
        /// </summary>
        event OnReserveNowRequestSentDelegate?                     OnReserveNowRequestSent;

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be be sent (to the charging station).
        /// </summary>
        event OnSetChargingProfileRequestSentDelegate?             OnSetChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be be sent (to the charging station).
        /// </summary>
        event OnUnlockConnectorRequestSentDelegate?                OnUnlockConnectorRequestSent;

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be be sent (to the charging station).
        /// </summary>
        event OnUpdateDynamicScheduleRequestSentDelegate?          OnUpdateDynamicScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be be sent (to the charging station).
        /// </summary>
        event OnUsePriorityChargingRequestSentDelegate?            OnUsePriorityChargingRequestSent;

        #endregion

        #region Customer

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be be sent (to the charging station).
        /// </summary>
        event OnClearDisplayMessageRequestSentDelegate?            OnClearDisplayMessageRequestSent;

        /// <summary>
        /// An event fired whenever a CostUpdated request will be be sent (to the charging station).
        /// </summary>
        event OnCostUpdatedRequestSentDelegate?                    OnCostUpdatedRequestSent;

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be be sent (to the charging station).
        /// </summary>
        event OnCustomerInformationRequestSentDelegate?            OnCustomerInformationRequestSent;

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be be sent (to the charging station).
        /// </summary>
        event OnGetDisplayMessagesRequestSentDelegate?             OnGetDisplayMessagesRequestSent;

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be be sent (to the charging station).
        /// </summary>
        event OnSetDisplayMessageRequestSentDelegate?              OnSetDisplayMessageRequestSent;

        #endregion

        #region DeviceModel

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be be sent (to the charging station).
        /// </summary>
        event OnChangeAvailabilityRequestSentDelegate?             OnChangeAvailabilityRequestSent;

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be be sent (to the charging station).
        /// </summary>
        event OnClearVariableMonitoringRequestSentDelegate?        OnClearVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be be sent (to the charging station).
        /// </summary>
        event OnGetBaseReportRequestSentDelegate?                  OnGetBaseReportRequestSent;

        /// <summary>
        /// An event fired whenever a GetLog request will be be sent (to the charging station).
        /// </summary>
        event OnGetLogRequestSentDelegate?                         OnGetLogRequestSent;

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be be sent (to the charging station).
        /// </summary>
        event OnGetMonitoringReportRequestSentDelegate?            OnGetMonitoringReportRequestSent;

        /// <summary>
        /// An event fired whenever a GetReport request will be be sent (to the charging station).
        /// </summary>
        event OnGetReportRequestSentDelegate?                      OnGetReportRequestSent;

        /// <summary>
        /// An event fired whenever a GetVariables request will be be sent (to the charging station).
        /// </summary>
        event OnGetVariablesRequestSentDelegate?                   OnGetVariablesRequestSent;

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be be sent (to the charging station).
        /// </summary>
        event OnSetMonitoringBaseRequestSentDelegate?              OnSetMonitoringBaseRequestSent;

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be be sent (to the charging station).
        /// </summary>
        event OnSetMonitoringLevelRequestSentDelegate?             OnSetMonitoringLevelRequestSent;

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be be sent (to the charging station).
        /// </summary>
        event OnSetNetworkProfileRequestSentDelegate?              OnSetNetworkProfileRequestSent;

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be be sent (to the charging station).
        /// </summary>
        event OnSetVariableMonitoringRequestSentDelegate?          OnSetVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a SetVariables request will be be sent (to the charging station).
        /// </summary>
        event OnSetVariablesRequestSentDelegate?                   OnSetVariablesRequestSent;

         /// <summary>
        /// An event fired whenever a TriggerMessage request will be be sent (to the charging station).
        /// </summary>
        event OnTriggerMessageRequestSentDelegate?                 OnTriggerMessageRequestSent;

        #endregion

        #region Firmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be be sent (to the charging station).
        /// </summary>
        event OnPublishFirmwareRequestSentDelegate?                OnPublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a Reset request will be be sent (to the charging station).
        /// </summary>
        event OnResetRequestSentDelegate?                          OnResetRequestSent;

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be be sent (to the charging station).
        /// </summary>
        event OnUnpublishFirmwareRequestSentDelegate?              OnUnpublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be be sent (to the charging station).
        /// </summary>
        event OnUpdateFirmwareRequestSentDelegate?                 OnUpdateFirmwareRequestSent;

        #endregion

        #region Grid

        /// <summary>
        /// An event fired whenever an AFRRSignal request will be be sent (to the charging station).
        /// </summary>
        event OnAFRRSignalRequestSentDelegate?                     OnAFRRSignalRequestSent;

        #endregion

        #region Local List

        /// <summary>
        /// An event fired whenever a ClearCache request will be be sent (to the charging station).
        /// </summary>
        event OnClearCacheRequestSentDelegate?                     OnClearCacheRequestSent;

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be be sent (to the charging station).
        /// </summary>
        event OnGetLocalListVersionRequestSentDelegate?            OnGetLocalListVersionRequestSent;

        /// <summary>
        /// An event fired whenever a SendLocalList request will be be sent (to the charging station).
        /// </summary>
        event OnSendLocalListRequestSentDelegate?                  OnSendLocalListRequestSent;

        #endregion



        // Outgoing responses

        #region Certificates

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate request was sent.
        /// </summary>
        event OnGet15118EVCertificateResponseSentDelegate                OnGet15118EVCertificateResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus request was sent.
        /// </summary>
        event OnGetCertificateStatusResponseSentDelegate                 OnGetCertificateStatusResponseSent;

        /// <summary>
        /// An event sent whenever a response to a GetCRL request was sent.
        /// </summary>
        event OnGetCRLResponseSentDelegate                               OnGetCRLResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        event OnSignCertificateResponseSentDelegate                      OnSignCertificateResponseSent;

        #endregion

        #region Charging

        /// <summary>
        /// An event sent whenever a response to an Authorize request was sent.
        /// </summary>
        event OnAuthorizeResponseSentDelegate                            OnAuthorizeResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ClearedChargingLimit request was sent.
        /// </summary>
        event OnClearedChargingLimitResponseSentDelegate                 OnClearedChargingLimitResponseSent;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        event OnMeterValuesResponseSentDelegate                          OnMeterValuesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit request was sent.
        /// </summary>
        event OnNotifyChargingLimitResponseSentDelegate                  OnNotifyChargingLimitResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingNeeds request was sent.
        /// </summary>
        event OnNotifyEVChargingNeedsResponseSentDelegate                OnNotifyEVChargingNeedsResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule request was sent.
        /// </summary>
        event OnNotifyEVChargingScheduleResponseSentDelegate             OnNotifyEVChargingScheduleResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging request was sent.
        /// </summary>
        event OnNotifyPriorityChargingResponseSentDelegate               OnNotifyPriorityChargingResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifySettlement request was sent.
        /// </summary>
        event OnNotifySettlementResponseSentDelegate                     OnNotifySettlementResponseSent;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate request was sent.
        /// </summary>
        event OnPullDynamicScheduleUpdateResponseSentDelegate            OnPullDynamicScheduleUpdateResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles request was sent.
        /// </summary>
        event OnReportChargingProfilesResponseSentDelegate               OnReportChargingProfilesResponseSent;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate request was sent.
        /// </summary>
        event OnReservationStatusUpdateResponseSentDelegate              OnReservationStatusUpdateResponseSent;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        event OnStatusNotificationResponseSentDelegate                   OnStatusNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a Transaction request was sent.
        /// </summary>
        event OnTransactionEventResponseSentDelegate                     OnTransactionEventResponseSent;

        #endregion

        #region Customer

        /// <summary>
        /// An event sent whenever a response to a NotifyCustomerInformation request was sent.
        /// </summary>
        event OnNotifyCustomerInformationResponseSentDelegate            OnNotifyCustomerInformationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyDisplayMessages request was sent.
        /// </summary>
        event OnNotifyDisplayMessagesResponseSentDelegate                OnNotifyDisplayMessagesResponseSent;

        #endregion

        #region Firmware

        /// <summary>
        /// An event sent whenever a response to a BootNotification request was sent.
        /// </summary>
        event OnBootNotificationResponseSentDelegate                     OnBootNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseSentDelegate           OnFirmwareStatusNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat request was sent.
        /// </summary>
        event OnHeartbeatResponseSentDelegate                            OnHeartbeatResponseSent;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        event OnPublishFirmwareStatusNotificationResponseSentDelegate    OnPublishFirmwareStatusNotificationResponseSent;

        #endregion

        #region Monitoring

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        event OnLogStatusNotificationResponseSentDelegate                OnLogStatusNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyEvent request was sent.
        /// </summary>
        event OnNotifyEventResponseSentDelegate                          OnNotifyEventResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyMonitoringReport request was sent.
        /// </summary>
        event OnNotifyMonitoringReportResponseSentDelegate               OnNotifyMonitoringReportResponseSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyReport request was sent.
        /// </summary>
        event OnNotifyReportResponseSentDelegate                         OnNotifyReportResponseSent;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseSentDelegate            OnSecurityEventNotificationResponseSent;

        #endregion


    }

}
