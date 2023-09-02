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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS clients.
    /// </summary>
    public interface ICSMSClientEvents
    {

        #region Reset

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the CSMS.
        /// </summary>
        event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the CSMS.
        /// </summary>
        event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the CSMS.
        /// </summary>
        event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the CSMS.
        /// </summary>
        event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the CSMS.
        /// </summary>
        event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the CSMS.
        /// </summary>
        event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the CSMS.
        /// </summary>
        event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the CSMS.
        /// </summary>
        event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the CSMS.
        /// </summary>
        event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the CSMS.
        /// </summary>
        event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the CSMS.
        /// </summary>
        event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the CSMS.
        /// </summary>
        event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the CSMS.
        /// </summary>
        event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the CSMS.
        /// </summary>
        event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the CSMS.
        /// </summary>
        event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region TransferData

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the CSMS.
        /// </summary>
        event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the CSMS.
        /// </summary>
        event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the CSMS.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the CSMS.
        /// </summary>
        event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the CSMS.
        /// </summary>
        event OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the CSMS.
        /// </summary>
        event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the CSMS.
        /// </summary>
        event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the CSMS.
        /// </summary>
        event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the CSMS.
        /// </summary>
        event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the CSMS.
        /// </summary>
        event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the CSMS.
        /// </summary>
        event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the CSMS.
        /// </summary>
        event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the CSMS.
        /// </summary>
        event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the CSMS.
        /// </summary>
        event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the CSMS.
        /// </summary>
        event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the CSMS.
        /// </summary>
        event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the CSMS.
        /// </summary>
        event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to the CSMS.
        /// </summary>
        event OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the CSMS.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the CSMS.
        /// </summary>
        event OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the CSMS.
        /// </summary>
        event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to the CSMS.
        /// </summary>
        event OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the CSMS.
        /// </summary>
        event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the CSMS.
        /// </summary>
        event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the CSMS.
        /// </summary>
        event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the CSMS.
        /// </summary>
        event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the CSMS.
        /// </summary>
        event OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        #endregion


    }

}
