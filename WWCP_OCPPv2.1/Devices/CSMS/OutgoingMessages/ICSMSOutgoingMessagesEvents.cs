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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all OCPP v2.1 CSMS clients.
    /// </summary>
    public interface ICSMSOutgoingMessagesEvents : OCPP.ICSMSOutgoingMessagesEvents
    {

        #region Reset                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to a charging station.
        /// </summary>
        event OnResetRequestSentDelegate?                               OnResetRequestSent;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseReceivedDelegate?                          OnResetResponseReceived;

        #endregion

        #region UpdateFirmware              (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to a charging station.
        /// </summary>
        event OnUpdateFirmwareRequestSentDelegate?                      OnUpdateFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseReceivedDelegate?                 OnUpdateFirmwareResponseReceived;

        #endregion

        #region PublishFirmware             (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnPublishFirmwareRequestSentDelegate?                     OnPublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseReceivedDelegate?                OnPublishFirmwareResponseReceived;

        #endregion

        #region UnpublishFirmware           (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnUnpublishFirmwareRequestSentDelegate?                   OnUnpublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseReceivedDelegate?              OnUnpublishFirmwareResponseReceived;

        #endregion

        #region GetBaseReport               (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to a charging station.
        /// </summary>
        event OnGetBaseReportRequestSentDelegate?                       OnGetBaseReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseReceivedDelegate?                  OnGetBaseReportResponseReceived;

        #endregion

        #region GetReport                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to a charging station.
        /// </summary>
        event OnGetReportRequestSentDelegate?                           OnGetReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseReceivedDelegate?                      OnGetReportResponseReceived;

        #endregion

        #region GetLog                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to a charging station.
        /// </summary>
        event OnGetLogRequestSentDelegate?                              OnGetLogRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseReceivedDelegate?                         OnGetLogResponseReceived;

        #endregion

        #region SetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to a charging station.
        /// </summary>
        event OnSetVariablesRequestSentDelegate?                        OnSetVariablesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseReceivedDelegate?                   OnSetVariablesResponseReceived;

        #endregion

        #region GetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to a charging station.
        /// </summary>
        event OnGetVariablesRequestSentDelegate?                        OnGetVariablesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseReceivedDelegate?                   OnGetVariablesResponseReceived;

        #endregion

        #region SetMonitoringBase           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringBaseRequestSentDelegate?                   OnSetMonitoringBaseRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseReceivedDelegate?              OnSetMonitoringBaseResponseReceived;

        #endregion

        #region GetMonitoringReport         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to a charging station.
        /// </summary>
        event OnGetMonitoringReportRequestSentDelegate?                 OnGetMonitoringReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseReceivedDelegate?            OnGetMonitoringReportResponseReceived;

        #endregion

        #region SetMonitoringLevel          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringLevelRequestSentDelegate?                  OnSetMonitoringLevelRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseReceivedDelegate?             OnSetMonitoringLevelResponseReceived;

        #endregion

        #region SetVariableMonitoring       (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnSetVariableMonitoringRequestSentDelegate?               OnSetVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseReceivedDelegate?          OnSetVariableMonitoringResponseReceived;

        #endregion

        #region ClearVariableMonitoring     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnClearVariableMonitoringRequestSentDelegate?             OnClearVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseReceivedDelegate?        OnClearVariableMonitoringResponseReceived;

        #endregion

        #region SetNetworkProfile           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to a charging station.
        /// </summary>
        event OnSetNetworkProfileRequestSentDelegate?                   OnSetNetworkProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseReceivedDelegate?              OnSetNetworkProfileResponseReceived;

        #endregion

        #region ChangeAvailability          (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to a charging station.
        /// </summary>
        event OnChangeAvailabilityRequestSentDelegate?                  OnChangeAvailabilityRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseReceivedDelegate?             OnChangeAvailabilityResponseReceived;

        #endregion

        #region TriggerMessage              (Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to a charging station.
        /// </summary>
        event OnTriggerMessageRequestSentDelegate?                      OnTriggerMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseReceivedDelegate?                 OnTriggerMessageResponseReceived;

        #endregion

        #region DataTransfer                (Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to a charging station.
        /// </summary>
        event OnDataTransferRequestSentDelegate?                        OnDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseReceivedDelegate?                   OnDataTransferResponseReceived;

        #endregion


        #region CertificateSigned           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to a charging station.
        /// </summary>
        event OnCertificateSignedRequestSentDelegate?                   OnCertificateSignedRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseReceivedDelegate?              OnCertificateSignedResponseReceived;

        #endregion

        #region InstallCertificate          (Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to a charging station.
        /// </summary>
        event OnInstallCertificateRequestSentDelegate?                  OnInstallCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseReceivedDelegate?             OnInstallCertificateResponseReceived;

        #endregion

        #region GetInstalledCertificateIds  (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to a charging station.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestSentDelegate?          OnGetInstalledCertificateIdsRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseReceivedDelegate?     OnGetInstalledCertificateIdsResponseReceived;

        #endregion

        #region DeleteCertificate           (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to a charging station.
        /// </summary>
        event OnDeleteCertificateRequestSentDelegate?                   OnDeleteCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseReceivedDelegate?              OnDeleteCertificateResponseReceived;

        #endregion

        #region NotifyCRL                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to a charging station.
        /// </summary>
        event OnNotifyCRLRequestSentDelegate?                           OnNotifyCRLRequestSent;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseReceivedDelegate?                      OnNotifyCRLResponseReceived;

        #endregion


        #region GetLocalListVersion         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to a charging station.
        /// </summary>
        event OnGetLocalListVersionRequestSentDelegate?                 OnGetLocalListVersionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseReceivedDelegate?            OnGetLocalListVersionResponseReceived;

        #endregion

        #region SendLocalList               (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to a charging station.
        /// </summary>
        event OnSendLocalListRequestSentDelegate?                       OnSendLocalListRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseReceivedDelegate?                  OnSendLocalListResponseReceived;

        #endregion

        #region ClearCache                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to a charging station.
        /// </summary>
        event OnClearCacheRequestSentDelegate?                          OnClearCacheRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseReceivedDelegate?                     OnClearCacheResponseReceived;

        #endregion


        #region ReserveNow                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to a charging station.
        /// </summary>
        event OnReserveNowRequestSentDelegate?                          OnReserveNowRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseReceivedDelegate?                     OnReserveNowResponseReceived;

        #endregion

        #region CancelReservation           (Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to a charging station.
        /// </summary>
        event OnCancelReservationRequestSentDelegate?                   OnCancelReservationRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseReceivedDelegate?              OnCancelReservationResponseReceived;

        #endregion

        #region RequestStartTransaction     (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStartTransactionRequestSentDelegate?             OnRequestStartTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseReceivedDelegate?        OnRequestStartTransactionResponseReceived;

        #endregion

        #region RequestStopTransaction      (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStopTransactionRequestSentDelegate?              OnRequestStopTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseReceivedDelegate?         OnRequestStopTransactionResponseReceived;

        #endregion

        #region GetTransactionStatus        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to a charging station.
        /// </summary>
        event OnGetTransactionStatusRequestSentDelegate?                OnGetTransactionStatusRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseReceivedDelegate?           OnGetTransactionStatusResponseReceived;

        #endregion

        #region SetChargingProfile          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnSetChargingProfileRequestSentDelegate?                  OnSetChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseReceivedDelegate?             OnSetChargingProfileResponseReceived;

        #endregion

        #region GetChargingProfiles         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to a charging station.
        /// </summary>
        event OnGetChargingProfilesRequestSentDelegate?                 OnGetChargingProfilesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseReceivedDelegate?            OnGetChargingProfilesResponseReceived;

        #endregion

        #region ClearChargingProfile        (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnClearChargingProfileRequestSentDelegate?                OnClearChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseReceivedDelegate?           OnClearChargingProfileResponseReceived;

        #endregion

        #region GetCompositeSchedule        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to a charging station.
        /// </summary>
        event OnGetCompositeScheduleRequestSentDelegate?                OnGetCompositeScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseReceivedDelegate?           OnGetCompositeScheduleResponseReceived;

        #endregion

        #region UpdateDynamicSchedule       (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to a charging station.
        /// </summary>
        event OnUpdateDynamicScheduleRequestSentDelegate?               OnUpdateDynamicScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleResponseReceivedDelegate?          OnUpdateDynamicScheduleResponseReceived;

        #endregion

        #region NotifyAllowedEnergyTransfer (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to a charging station.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestSentDelegate?         OnNotifyAllowedEnergyTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?    OnNotifyAllowedEnergyTransferResponseReceived;

        #endregion

        #region UsePriorityCharging         (Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to a charging station.
        /// </summary>
        event OnUsePriorityChargingRequestSentDelegate?                 OnUsePriorityChargingRequestSent;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingResponseReceivedDelegate?            OnUsePriorityChargingResponseReceived;

        #endregion

        #region UnlockConnector             (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to a charging station.
        /// </summary>
        event OnUnlockConnectorRequestSentDelegate?                     OnUnlockConnectorRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseReceivedDelegate?                OnUnlockConnectorResponseReceived;

        #endregion


        #region AFRRSignal                  (Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to a charging station.
        /// </summary>
        event OnAFRRSignalRequestSentDelegate?                          OnAFRRSignalRequestSent;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseReceivedDelegate?                     OnAFRRSignalResponseReceived;

        #endregion


        #region SetDisplayMessage           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnSetDisplayMessageRequestSentDelegate?                   OnSetDisplayMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseReceivedDelegate?              OnSetDisplayMessageResponseReceived;

        #endregion

        #region GetDisplayMessages          (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to a charging station.
        /// </summary>
        event OnGetDisplayMessagesRequestSentDelegate?                  OnGetDisplayMessagesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseReceivedDelegate?             OnGetDisplayMessagesResponseReceived;

        #endregion

        #region ClearDisplayMessage         (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnClearDisplayMessageRequestSentDelegate?                 OnClearDisplayMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseReceivedDelegate?            OnClearDisplayMessageResponseReceived;

        #endregion

        #region SendCostUpdated             (Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to a charging station.
        /// </summary>
        event OnCostUpdatedRequestSentDelegate?                         OnCostUpdatedRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseReceivedDelegate?                    OnCostUpdatedResponseReceived;

        #endregion

        #region RequestCustomerInformation  (Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to a charging station.
        /// </summary>
        event OnCustomerInformationRequestSentDelegate?                 OnCustomerInformationRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseReceivedDelegate?            OnCustomerInformationResponseReceived;

        #endregion


    }

}
