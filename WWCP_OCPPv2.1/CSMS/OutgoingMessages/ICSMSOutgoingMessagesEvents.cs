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

using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all OCPP v2.1 CSMS clients.
    /// </summary>
    public interface ICSMSOutgoingMessagesEvents : OCPP.ICSMSOutgoingMessagesEvents
    {

        #region Reset                          (Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to a charging station.
        /// </summary>
        event OnResetRequestSentDelegate?                           OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseReceivedDelegate?                          OnResetResponse;

        #endregion

        #region UpdateFirmware                 (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to a charging station.
        /// </summary>
        event OnUpdateFirmwareRequestSentDelegate?                  OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseReceivedDelegate?                 OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware                (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnPublishFirmwareRequestSentDelegate?                 OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseReceivedDelegate?                OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware              (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnUnpublishFirmwareRequestSentDelegate?               OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseReceivedDelegate?              OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to a charging station.
        /// </summary>
        event OnGetBaseReportRequestSentDelegate?                   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseReceivedDelegate?                  OnGetBaseReportResponse;

        #endregion

        #region GetReport                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to a charging station.
        /// </summary>
        event OnGetReportRequestSentDelegate?                       OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseReceivedDelegate?                      OnGetReportResponse;

        #endregion

        #region GetLog                         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to a charging station.
        /// </summary>
        event OnGetLogRequestSentDelegate?                          OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseReceivedDelegate?                         OnGetLogResponse;

        #endregion

        #region SetVariables                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to a charging station.
        /// </summary>
        event OnSetVariablesRequestSentDelegate?                    OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseReceivedDelegate?                   OnSetVariablesResponse;

        #endregion

        #region GetVariables                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to a charging station.
        /// </summary>
        event OnGetVariablesRequestSentDelegate?                    OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseReceivedDelegate?                   OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase              (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringBaseRequestSentDelegate?               OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseReceivedDelegate?              OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport            (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to a charging station.
        /// </summary>
        event OnGetMonitoringReportRequestSentDelegate?             OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseReceivedDelegate?            OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel             (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringLevelRequestSentDelegate?              OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseReceivedDelegate?             OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnSetVariableMonitoringRequestSentDelegate?           OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseReceivedDelegate?          OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring        (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnClearVariableMonitoringRequestSentDelegate?         OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseReceivedDelegate?        OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile              (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to a charging station.
        /// </summary>
        event OnSetNetworkProfileRequestSentDelegate?               OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseReceivedDelegate?              OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability             (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to a charging station.
        /// </summary>
        event OnChangeAvailabilityRequestSentDelegate?              OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseReceivedDelegate?             OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to a charging station.
        /// </summary>
        event OnTriggerMessageRequestSentDelegate?                  OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseReceivedDelegate?                 OnTriggerMessageResponse;

        #endregion

        #region DataTransfer                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to a charging station.
        /// </summary>
        event OnDataTransferRequestSentDelegate?                    OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseReceivedDelegate?                   OnDataTransferResponse;

        #endregion


        #region CertificateSigned              (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to a charging station.
        /// </summary>
        event OnCertificateSignedRequestSentDelegate?               OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseReceivedDelegate?              OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate             (Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to a charging station.
        /// </summary>
        event OnInstallCertificateRequestSentDelegate?              OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseReceivedDelegate?             OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds     (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to a charging station.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestSentDelegate?      OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseReceivedDelegate?     OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate              (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to a charging station.
        /// </summary>
        event OnDeleteCertificateRequestSentDelegate?               OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseReceivedDelegate?              OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to a charging station.
        /// </summary>
        event OnNotifyCRLRequestSentDelegate?                       OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseReceivedDelegate?                      OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion            (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to a charging station.
        /// </summary>
        event OnGetLocalListVersionRequestSentDelegate?             OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseReceivedDelegate?            OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to a charging station.
        /// </summary>
        event OnSendLocalListRequestSentDelegate?                   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseReceivedDelegate?                  OnSendLocalListResponse;

        #endregion

        #region ClearCache                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to a charging station.
        /// </summary>
        event OnClearCacheRequestSentDelegate?                      OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseReceivedDelegate?                     OnClearCacheResponse;

        #endregion


        #region ReserveNow                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to a charging station.
        /// </summary>
        event OnReserveNowRequestSentDelegate?                      OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseReceivedDelegate?                     OnReserveNowResponse;

        #endregion

        #region CancelReservation              (Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to a charging station.
        /// </summary>
        event OnCancelReservationRequestSentDelegate?               OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseReceivedDelegate?              OnCancelReservationResponse;

        #endregion

        #region RequestStartTransaction        (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStartTransactionRequestSentDelegate?         OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseReceivedDelegate?        OnRequestStartTransactionResponse;

        #endregion

        #region RequestStopTransaction         (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStopTransactionRequestSentDelegate?          OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseReceivedDelegate?         OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus           (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to a charging station.
        /// </summary>
        event OnGetTransactionStatusRequestSentDelegate?            OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseReceivedDelegate?           OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile             (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnSetChargingProfileRequestSentDelegate?              OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseReceivedDelegate?             OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles            (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to a charging station.
        /// </summary>
        event OnGetChargingProfilesRequestSentDelegate?             OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseReceivedDelegate?            OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile           (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnClearChargingProfileRequestSentDelegate?            OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseReceivedDelegate?           OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule           (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to a charging station.
        /// </summary>
        event OnGetCompositeScheduleRequestSentDelegate?            OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseReceivedDelegate?           OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule          (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to a charging station.
        /// </summary>
        event OnUpdateDynamicScheduleRequestSentDelegate?           OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleResponseReceivedDelegate?          OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer    (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to a charging station.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestSentDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?    OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging            (Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to a charging station.
        /// </summary>
        event OnUsePriorityChargingRequestSentDelegate?             OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingResponseReceivedDelegate?            OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector                (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to a charging station.
        /// </summary>
        event OnUnlockConnectorRequestSentDelegate?                 OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseReceivedDelegate?                OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal                     (Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to a charging station.
        /// </summary>
        event OnAFRRSignalRequestSentDelegate?                      OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseReceivedDelegate?                     OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage              (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnSetDisplayMessageRequestSentDelegate?               OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseReceivedDelegate?              OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages             (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to a charging station.
        /// </summary>
        event OnGetDisplayMessagesRequestSentDelegate?              OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseReceivedDelegate?             OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage            (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnClearDisplayMessageRequestSentDelegate?             OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseReceivedDelegate?            OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated                (Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to a charging station.
        /// </summary>
        event OnCostUpdatedRequestSentDelegate?                     OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseReceivedDelegate?                    OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation     (Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to a charging station.
        /// </summary>
        event OnCustomerInformationRequestSentDelegate?             OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseReceivedDelegate?            OnCustomerInformationResponse;

        #endregion


    }

}
