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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all NetworkingNode clients.
    /// </summary>
    public interface INetworkingNodeClientEvents
    {

        #region Reset                          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event CSMS.OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware                 (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event CSMS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event CSMS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware              (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event CSMS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event CSMS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event CSMS.OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event CSMS.OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event CSMS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event CSMS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event CSMS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event CSMS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event CSMS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event CSMS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event CSMS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event CSMS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event CSMS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event CSMS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region DataTransfer

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event CSMS.OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event CSMS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event CSMS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event CSMS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event CSMS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event CSMS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event CSMS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event CSMS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event CSMS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event CSMS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event CSMS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event CSMS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event CSMS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event CSMS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event CSMS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event CSMS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event CSMS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event CSMS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        event CSMS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer    (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event CSMS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        event CSMS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event CSMS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event CSMS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event CSMS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event CSMS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event CSMS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event CSMS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the NetworkingNode.
        /// </summary>
        event CSMS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event CSMS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        #endregion


    }

}
