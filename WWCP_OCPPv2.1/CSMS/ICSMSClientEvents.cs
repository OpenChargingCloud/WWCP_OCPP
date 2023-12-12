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
    public interface ICSMSClientEvents : OCPP.ICSMSClientEvents
    {

        #region Reset                          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to a charging station.
        /// </summary>
        event OnResetRequestDelegate?                           OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseDelegate?                          OnResetResponse;

        #endregion

        #region UpdateFirmware                 (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to a charging station.
        /// </summary>
        event OnUpdateFirmwareRequestDelegate?                  OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseDelegate?                 OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnPublishFirmwareRequestDelegate?                 OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseDelegate?                OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware              (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to a charging station.
        /// </summary>
        event OnUnpublishFirmwareRequestDelegate?               OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseDelegate?              OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to a charging station.
        /// </summary>
        event OnGetBaseReportRequestDelegate?                   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseDelegate?                  OnGetBaseReportResponse;

        #endregion

        #region GetReport                      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to a charging station.
        /// </summary>
        event OnGetReportRequestDelegate?                       OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseDelegate?                      OnGetReportResponse;

        #endregion

        #region GetLog                         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to a charging station.
        /// </summary>
        event OnGetLogRequestDelegate?                          OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseDelegate?                         OnGetLogResponse;

        #endregion

        #region SetVariables                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to a charging station.
        /// </summary>
        event OnSetVariablesRequestDelegate?                    OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseDelegate?                   OnSetVariablesResponse;

        #endregion

        #region GetVariables                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to a charging station.
        /// </summary>
        event OnGetVariablesRequestDelegate?                    OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseDelegate?                   OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringBaseRequestDelegate?               OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseDelegate?              OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to a charging station.
        /// </summary>
        event OnGetMonitoringReportRequestDelegate?             OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseDelegate?            OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel             (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to a charging station.
        /// </summary>
        event OnSetMonitoringLevelRequestDelegate?              OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseDelegate?             OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnSetVariableMonitoringRequestDelegate?           OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseDelegate?          OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring        (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to a charging station.
        /// </summary>
        event OnClearVariableMonitoringRequestDelegate?         OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseDelegate?        OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to a charging station.
        /// </summary>
        event OnSetNetworkProfileRequestDelegate?               OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseDelegate?              OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability             (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to a charging station.
        /// </summary>
        event OnChangeAvailabilityRequestDelegate?              OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseDelegate?             OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage                 (-Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to a charging station.
        /// </summary>
        event OnTriggerMessageRequestDelegate?                  OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseDelegate?                 OnTriggerMessageResponse;

        #endregion

        #region DataTransfer                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to a charging station.
        /// </summary>
        event OnDataTransferRequestDelegate?                    OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate?                   OnDataTransferResponse;

        #endregion


        #region CertificateSigned              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to a charging station.
        /// </summary>
        event OnCertificateSignedRequestDelegate?               OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseDelegate?              OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate             (-Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to a charging station.
        /// </summary>
        event OnInstallCertificateRequestDelegate?              OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseDelegate?             OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to a charging station.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestDelegate?      OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseDelegate?     OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to a charging station.
        /// </summary>
        event OnDeleteCertificateRequestDelegate?               OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseDelegate?              OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL                      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to a charging station.
        /// </summary>
        event OnNotifyCRLRequestDelegate?                       OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseDelegate?                      OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to a charging station.
        /// </summary>
        event OnGetLocalListVersionRequestDelegate?             OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseDelegate?            OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to a charging station.
        /// </summary>
        event OnSendLocalListRequestDelegate?                   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseDelegate?                  OnSendLocalListResponse;

        #endregion

        #region ClearCache                     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to a charging station.
        /// </summary>
        event OnClearCacheRequestDelegate?                      OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseDelegate?                     OnClearCacheResponse;

        #endregion


        #region ReserveNow                     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to a charging station.
        /// </summary>
        event OnReserveNowRequestDelegate?                      OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseDelegate?                     OnReserveNowResponse;

        #endregion

        #region CancelReservation              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to a charging station.
        /// </summary>
        event OnCancelReservationRequestDelegate?               OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseDelegate?              OnCancelReservationResponse;

        #endregion

        #region RequestStartTransaction        (-Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStartTransactionRequestDelegate?         OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseDelegate?        OnRequestStartTransactionResponse;

        #endregion

        #region RequestStopTransaction         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to a charging station.
        /// </summary>
        event OnRequestStopTransactionRequestDelegate?          OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseDelegate?         OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to a charging station.
        /// </summary>
        event OnGetTransactionStatusRequestDelegate?            OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseDelegate?           OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile             (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnSetChargingProfileRequestDelegate?              OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseDelegate?             OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to a charging station.
        /// </summary>
        event OnGetChargingProfilesRequestDelegate?             OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseDelegate?            OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to a charging station.
        /// </summary>
        event OnClearChargingProfileRequestDelegate?            OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseDelegate?           OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to a charging station.
        /// </summary>
        event OnGetCompositeScheduleRequestDelegate?            OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseDelegate?           OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to a charging station.
        /// </summary>
        event OnUpdateDynamicScheduleRequestDelegate?           OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleResponseDelegate?          OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer    (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to a charging station.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseDelegate?    OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to a charging station.
        /// </summary>
        event OnUsePriorityChargingRequestDelegate?             OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingResponseDelegate?            OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector                (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to a charging station.
        /// </summary>
        event OnUnlockConnectorRequestDelegate?                 OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseDelegate?                OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal                     (-Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to a charging station.
        /// </summary>
        event OnAFRRSignalRequestDelegate?                      OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseDelegate?                     OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnSetDisplayMessageRequestDelegate?               OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseDelegate?              OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages             (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to a charging station.
        /// </summary>
        event OnGetDisplayMessagesRequestDelegate?              OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseDelegate?             OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to a charging station.
        /// </summary>
        event OnClearDisplayMessageRequestDelegate?             OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseDelegate?            OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to a charging station.
        /// </summary>
        event OnCostUpdatedRequestDelegate?                     OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseDelegate?                    OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to a charging station.
        /// </summary>
        event OnCustomerInformationRequestDelegate?             OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseDelegate?            OnCustomerInformationResponse;

        #endregion


    }

}
