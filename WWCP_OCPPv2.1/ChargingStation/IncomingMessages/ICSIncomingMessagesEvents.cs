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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The common interface of all incoming messages at charging station servers.
    /// </summary>
    public interface ICSIncomingMessagesEvents : OCPP.CS.ICSIncomingMessagesEvents
    {

        #region OnReset                         (Request/-Response)

        /// <summary>
        /// An event sent whenever a Reset request was received.
        /// </summary>
        event OnResetRequestReceivedDelegate                         OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        event OnResetResponseSentDelegate                            OnResetResponse;

        #endregion

        #region OnUpdateFirmware                (Request/-Response)

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareRequestReceivedDelegate                OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        event OnUpdateFirmwareResponseSentDelegate                   OnUpdateFirmwareResponse;

        #endregion

        #region OnPublishFirmware               (Request/-Response)

        /// <summary>
        /// An event sent whenever a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareRequestReceivedDelegate               OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        event OnPublishFirmwareResponseSentDelegate                  OnPublishFirmwareResponse;

        #endregion

        #region OnUnpublishFirmware             (Request/-Response)

        /// <summary>
        /// An event sent whenever an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareRequestReceivedDelegate             OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UnpublishFirmware request was sent.
        /// </summary>
        event OnUnpublishFirmwareResponseSentDelegate                OnUnpublishFirmwareResponse;

        #endregion

        #region OnGetBaseReport                 (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportRequestReceivedDelegate                 OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport was sent.
        /// </summary>
        event OnGetBaseReportResponseSentDelegate                    OnGetBaseReportResponse;

        #endregion

        #region OnGetReport                     (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetReport request was received.
        /// </summary>
        event OnGetReportRequestReceivedDelegate                     OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        event OnGetReportResponseSentDelegate                        OnGetReportResponse;

        #endregion

        #region OnGetLog                        (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetLog request was received.
        /// </summary>
        event OnGetLogRequestReceivedDelegate                        OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLog request was sent.
        /// </summary>
        event OnGetLogResponseSentDelegate                           OnGetLogResponse;

        #endregion

        #region OnSetVariables                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetVariables request was received.
        /// </summary>
        event OnSetVariablesRequestReceivedDelegate                  OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        event OnSetVariablesResponseSentDelegate                     OnSetVariablesResponse;

        #endregion

        #region OnGetVariables                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetVariables request was received.
        /// </summary>
        event OnGetVariablesRequestReceivedDelegate                  OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetVariables request was sent.
        /// </summary>
        event OnGetVariablesResponseSentDelegate                     OnGetVariablesResponse;

        #endregion

        #region OnSetMonitoringBase             (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseRequestReceivedDelegate             OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringBase request was sent.
        /// </summary>
        event OnSetMonitoringBaseResponseSentDelegate                OnSetMonitoringBaseResponse;

        #endregion

        #region OnGetMonitoringReport           (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportRequestReceivedDelegate           OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetMonitoringReport request was sent.
        /// </summary>
        event OnGetMonitoringReportResponseSentDelegate              OnGetMonitoringReportResponse;

        #endregion

        #region OnSetMonitoringLevel            (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelRequestReceivedDelegate            OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringLevel request was sent.
        /// </summary>
        event OnSetMonitoringLevelResponseSentDelegate               OnSetMonitoringLevelResponse;

        #endregion

        #region OnSetVariableMonitoring         (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringRequestReceivedDelegate         OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a SetVariableMonitoring request was sent.
        /// </summary>
        event OnSetVariableMonitoringResponseSentDelegate            OnSetVariableMonitoringResponse;

        #endregion

        #region OnClearVariableMonitoring       (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringRequestReceivedDelegate       OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearVariableMonitoring request was sent.
        /// </summary>
        event OnClearVariableMonitoringResponseSentDelegate          OnClearVariableMonitoringResponse;

        #endregion

        #region OnSetNetworkProfile             (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileRequestReceivedDelegate             OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetNetworkProfile request was sent.
        /// </summary>
        event OnSetNetworkProfileResponseSentDelegate                OnSetNetworkProfileResponse;

        #endregion

        #region OnChangeAvailability            (Request/-Response)

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityRequestReceivedDelegate            OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a ChangeAvailability request was sent.
        /// </summary>
        event OnChangeAvailabilityResponseSentDelegate               OnChangeAvailabilityResponse;

        #endregion

        #region OnTriggerMessage                (Request/-Response)

        /// <summary>
        /// An event sent whenever a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageRequestReceivedDelegate                OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a TriggerMessage request was sent.
        /// </summary>
        event OnTriggerMessageResponseSentDelegate                   OnTriggerMessageResponse;

        #endregion

        #region OnIncomingDataTransfer          (Request/-Response)

        /// <summary>
        /// An event sent whenever an incoming DataTransfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate                  OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an incoming DataTransfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate                 OnIncomingDataTransferResponse;

        #endregion


        #region OnCertificateSigned             (Request/-Response)

        /// <summary>
        /// An event sent whenever a CertificateSigned request was received.
        /// </summary>
        event OnCertificateSignedRequestReceivedDelegate             OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        event OnCertificateSignedResponseSentDelegate                OnCertificateSignedResponse;

        #endregion

        #region OnInstallCertificate            (Request/-Response)

        /// <summary>
        /// An event sent whenever an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateRequestReceivedDelegate            OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an InstallCertificate request was sent.
        /// </summary>
        event OnInstallCertificateResponseSentDelegate               OnInstallCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds    (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestReceivedDelegate    OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseSentDelegate       OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnDeleteCertificate             (Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateRequestReceivedDelegate             OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteCertificate request was sent.
        /// </summary>
        event OnDeleteCertificateResponseSentDelegate                OnDeleteCertificateResponse;

        #endregion

        #region OnNotifyCRL                     (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLRequestReceivedDelegate                     OnNotifyCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        event OnNotifyCRLResponseSentDelegate                        OnNotifyCRLResponse;

        #endregion


        #region OnGetLocalListVersion           (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionRequestReceivedDelegate           OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        event OnGetLocalListVersionResponseSentDelegate              OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList                 (Request/-Response)

        /// <summary>
        /// An event sent whenever a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListRequestReceivedDelegate                 OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        event OnSendLocalListResponseSentDelegate                    OnSendLocalListResponse;

        #endregion

        #region OnClearCache                    (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearCache request was received.
        /// </summary>
        event OnClearCacheRequestReceivedDelegate                    OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        event OnClearCacheResponseSentDelegate                       OnClearCacheResponse;

        #endregion


        #region OnReserveNow                    (Request/-Response)

        /// <summary>
        /// An event sent whenever a ReserveNow request was received.
        /// </summary>
        event OnReserveNowRequestReceivedDelegate                    OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a ReserveNow request was sent.
        /// </summary>
        event OnReserveNowResponseSentDelegate                       OnReserveNowResponse;

        #endregion

        #region OnCancelReservation             (Request/-Response)

        /// <summary>
        /// An event sent whenever a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationRequestReceivedDelegate             OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a CancelReservation request was sent.
        /// </summary>
        event OnCancelReservationResponseSentDelegate                OnCancelReservationResponse;

        #endregion

        #region OnRequestStartTransaction       (Request/-Response)

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionRequestReceivedDelegate       OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        event OnRequestStartTransactionResponseSentDelegate          OnRequestStartTransactionResponse;

        #endregion

        #region OnRequestStopTransaction        (Request/-Response)

        /// <summary>
        /// An event sent whenever a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionRequestReceivedDelegate        OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStopTransaction request was sent.
        /// </summary>
        event OnRequestStopTransactionResponseSentDelegate           OnRequestStopTransactionResponse;

        #endregion

        #region OnGetTransactionStatus          (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusRequestReceivedDelegate          OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        event OnGetTransactionStatusResponseSentDelegate             OnGetTransactionStatusResponse;

        #endregion

        #region OnSetChargingProfile            (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileRequestReceivedDelegate            OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        event OnSetChargingProfileResponseSentDelegate               OnSetChargingProfileResponse;

        #endregion

        #region OnGetChargingProfiles           (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesRequestReceivedDelegate           OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetChargingProfiles request was sent.
        /// </summary>
        event OnGetChargingProfilesResponseSentDelegate              OnGetChargingProfilesResponse;

        #endregion

        #region OnClearChargingProfile          (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileRequestReceivedDelegate          OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        event OnClearChargingProfileResponseSentDelegate             OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule          (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleRequestReceivedDelegate          OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        event OnGetCompositeScheduleResponseSentDelegate             OnGetCompositeScheduleResponse;

        #endregion

        #region OnUpdateDynamicSchedule         (Request/-Response)

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleRequestReceivedDelegate         OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever an response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        event OnUpdateDynamicScheduleResponseSentDelegate            OnUpdateDynamicScheduleResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer   (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestReceivedDelegate   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseSentDelegate      OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region OnUsePriorityCharging           (Request/-Response)

        /// <summary>
        /// An event sent whenever an UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingRequestReceivedDelegate           OnUsePriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to an UsePriorityCharging request was sent.
        /// </summary>
        event OnUsePriorityChargingResponseSentDelegate              OnUsePriorityChargingResponse;

        #endregion

        #region OnUnlockConnector               (Request/-Response)

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorRequestReceivedDelegate               OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        event OnUnlockConnectorResponseSentDelegate                  OnUnlockConnectorResponse;

        #endregion


        #region OnAFRRSignal                    (Request/-Response)

        /// <summary>
        /// An event sent whenever an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalRequestReceivedDelegate                    OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever a response to an AFRR signal request was sent.
        /// </summary>
        event OnAFRRSignalResponseSentDelegate                       OnAFRRSignalResponse;

        #endregion


        #region OnSetDisplayMessage             (Request/-Response)

        /// <summary>
        /// An event sent whenever a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageRequestReceivedDelegate             OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDisplayMessage request was sent.
        /// </summary>
        event OnSetDisplayMessageResponseSentDelegate                OnSetDisplayMessageResponse;

        #endregion

        #region OnGetDisplayMessages            (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesRequestReceivedDelegate            OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        event OnGetDisplayMessagesResponseSentDelegate               OnGetDisplayMessagesResponse;

        #endregion

        #region OnClearDisplayMessage           (Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageRequestReceivedDelegate           OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearDisplayMessage request was sent.
        /// </summary>
        event OnClearDisplayMessageResponseSentDelegate              OnClearDisplayMessageResponse;

        #endregion

        #region OnCostUpdated                   (Request/-Response)

        /// <summary>
        /// An event sent whenever a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedRequestReceivedDelegate                   OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a response to a CostUpdated request was sent.
        /// </summary>
        event OnCostUpdatedResponseSentDelegate                      OnCostUpdatedResponse;

        #endregion

        #region OnCustomerInformation           (Request/-Response)

        /// <summary>
        /// An event sent whenever a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationRequestReceivedDelegate           OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a CustomerInformation request was sent.
        /// </summary>
        event OnCustomerInformationResponseSentDelegate              OnCustomerInformationResponse;

        #endregion


    }

}
