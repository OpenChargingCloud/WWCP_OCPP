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

using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The common interface of all charging station servers.
    /// </summary>
    public interface ICSIncomingMessagesEvents : OCPP.CS.ICSIncomingMessagesEvents
    {

        #region OnReset                         (Request/-Response)

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        event OnResetRequestReceivedDelegate    OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        event OnResetResponseSentDelegate   OnResetResponse;

        #endregion

        #region OnUpdateFirmware                (Request/-Response)

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        event OnUpdateFirmwareRequestReceivedDelegate    OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        event OnUpdateFirmwareResponseSentDelegate   OnUpdateFirmwareResponse;

        #endregion

        #region OnPublishFirmware               (Request/-Response)

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        event OnPublishFirmwareRequestReceivedDelegate    OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a publish firmware request was sent.
        /// </summary>
        event OnPublishFirmwareResponseSentDelegate   OnPublishFirmwareResponse;

        #endregion

        #region OnUnpublishFirmware

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        event OnUnpublishFirmwareRequestReceivedDelegate    OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an unpublish firmware request was sent.
        /// </summary>
        event OnUnpublishFirmwareResponseSentDelegate   OnUnpublishFirmwareResponse;

        #endregion

        #region OnGetBaseReport

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        event OnGetBaseReportRequestReceivedDelegate    OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a response to a get base request was sent.
        /// </summary>
        event OnGetBaseReportResponseSentDelegate   OnGetBaseReportResponse;

        #endregion

        #region OnGetReport

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        event OnGetReportRequestReceivedDelegate    OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a response to a get report request was sent.
        /// </summary>
        event OnGetReportResponseSentDelegate   OnGetReportResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        event OnGetLogRequestReceivedDelegate    OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        event OnGetLogResponseSentDelegate   OnGetLogResponse;

        #endregion

        #region OnSetVariables

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        event OnSetVariablesRequestReceivedDelegate    OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a set variables request was sent.
        /// </summary>
        event OnSetVariablesResponseSentDelegate   OnSetVariablesResponse;

        #endregion

        #region OnGetVariables

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        event OnGetVariablesRequestReceivedDelegate    OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        event OnGetVariablesResponseSentDelegate   OnGetVariablesResponse;

        #endregion

        #region OnSetMonitoringBase

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        event OnSetMonitoringBaseRequestReceivedDelegate    OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a response to a set monitoring base request was sent.
        /// </summary>
        event OnSetMonitoringBaseResponseSentDelegate   OnSetMonitoringBaseResponse;

        #endregion

        #region OnGetMonitoringReport

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        event OnGetMonitoringReportRequestReceivedDelegate    OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a get monitoring report request was sent.
        /// </summary>
        event OnGetMonitoringReportResponseSentDelegate   OnGetMonitoringReportResponse;

        #endregion

        #region OnSetMonitoringLevel

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        event OnSetMonitoringLevelRequestReceivedDelegate    OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a response to a set monitoring level request was sent.
        /// </summary>
        event OnSetMonitoringLevelResponseSentDelegate   OnSetMonitoringLevelResponse;

        #endregion

        #region OnSetVariableMonitoring

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringRequestReceivedDelegate    OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        event OnSetVariableMonitoringResponseSentDelegate   OnSetVariableMonitoringResponse;

        #endregion

        #region OnClearVariableMonitoring

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringRequestReceivedDelegate    OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a clear variable monitoring request was sent.
        /// </summary>
        event OnClearVariableMonitoringResponseSentDelegate   OnClearVariableMonitoringResponse;

        #endregion

        #region OnSetNetworkProfile

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        event OnSetNetworkProfileRequestReceivedDelegate    OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        event OnSetNetworkProfileResponseSentDelegate   OnSetNetworkProfileResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        event OnChangeAvailabilityRequestReceivedDelegate    OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        event OnChangeAvailabilityResponseSentDelegate   OnChangeAvailabilityResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        event OnTriggerMessageRequestReceivedDelegate    OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        event OnTriggerMessageResponseSentDelegate   OnTriggerMessageResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        event OnCertificateSignedRequestReceivedDelegate    OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        event OnCertificateSignedResponseSentDelegate   OnCertificateSignedResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        event OnInstallCertificateRequestReceivedDelegate    OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        event OnInstallCertificateResponseSentDelegate   OnInstallCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds    (Request/-Response)

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestReceivedDelegate    OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseSentDelegate   OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        event OnDeleteCertificateRequestReceivedDelegate    OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        event OnDeleteCertificateResponseSentDelegate   OnDeleteCertificateResponse;

        #endregion

        #region OnNotifyCRL

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        event OnNotifyCRLRequestReceivedDelegate    OnNotifyCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        event OnNotifyCRLResponseSentDelegate   OnNotifyCRLResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a get local list version request was received.
        /// </summary>
        event OnGetLocalListVersionRequestReceivedDelegate    OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a get local list version request was sent.
        /// </summary>
        event OnGetLocalListVersionResponseSentDelegate   OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a send local list request was received.
        /// </summary>
        event OnSendLocalListRequestReceivedDelegate    OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a send local list request was sent.
        /// </summary>
        event OnSendLocalListResponseSentDelegate   OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a clear cache request was received.
        /// </summary>
        event OnClearCacheRequestReceivedDelegate    OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a clear cache request was sent.
        /// </summary>
        event OnClearCacheResponseSentDelegate   OnClearCacheResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        event OnReserveNowRequestReceivedDelegate    OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        event OnReserveNowResponseSentDelegate   OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        event OnCancelReservationRequestReceivedDelegate    OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        event OnCancelReservationResponseSentDelegate   OnCancelReservationResponse;

        #endregion

        #region OnRequestStartTransaction

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        event OnRequestStartTransactionRequestReceivedDelegate    OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a request start transaction request was sent.
        /// </summary>
        event OnRequestStartTransactionResponseSentDelegate   OnRequestStartTransactionResponse;

        #endregion

        #region OnRequestStopTransaction

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        event OnRequestStopTransactionRequestReceivedDelegate    OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a request stop transaction request was sent.
        /// </summary>
        event OnRequestStopTransactionResponseSentDelegate   OnRequestStopTransactionResponse;

        #endregion

        #region OnGetTransactionStatus

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        event OnGetTransactionStatusRequestReceivedDelegate    OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a get transaction status request was sent.
        /// </summary>
        event OnGetTransactionStatusResponseSentDelegate   OnGetTransactionStatusResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        event OnSetChargingProfileRequestReceivedDelegate    OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        event OnSetChargingProfileResponseSentDelegate   OnSetChargingProfileResponse;

        #endregion

        #region OnGetChargingProfiles

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        event OnGetChargingProfilesRequestReceivedDelegate    OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a get charging profiles request was sent.
        /// </summary>
        event OnGetChargingProfilesResponseSentDelegate   OnGetChargingProfilesResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        event OnClearChargingProfileRequestReceivedDelegate    OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        event OnClearChargingProfileResponseSentDelegate   OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleRequestReceivedDelegate    OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        event OnGetCompositeScheduleResponseSentDelegate   OnGetCompositeScheduleResponse;

        #endregion

        #region OnUpdateDynamicSchedule

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        event OnUpdateDynamicScheduleRequestReceivedDelegate    OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever an response to a UpdateDynamicSchedule request was sent.
        /// </summary>
        event OnUpdateDynamicScheduleResponseSentDelegate   OnUpdateDynamicScheduleResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestReceivedDelegate    OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseSentDelegate   OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region OnUsePriorityCharging

        /// <summary>
        /// An event sent whenever an UsePriorityCharging request was received.
        /// </summary>
        event OnUsePriorityChargingRequestReceivedDelegate    OnUsePriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to an UsePriorityCharging request was sent.
        /// </summary>
        event OnUsePriorityChargingResponseSentDelegate   OnUsePriorityChargingResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorRequestReceivedDelegate    OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        event OnUnlockConnectorResponseSentDelegate   OnUnlockConnectorResponse;

        #endregion


        #region OnAFRRSignal

        /// <summary>
        /// An event sent whenever an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalRequestReceivedDelegate    OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever a response to an AFRR signal request was sent.
        /// </summary>
        event OnAFRRSignalResponseSentDelegate   OnAFRRSignalResponse;

        #endregion


        #region OnSetDisplayMessage

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        event OnSetDisplayMessageRequestReceivedDelegate    OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        event OnSetDisplayMessageResponseSentDelegate   OnSetDisplayMessageResponse;

        #endregion

        #region OnGetDisplayMessages

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        event OnGetDisplayMessagesRequestReceivedDelegate    OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a get display messages request was sent.
        /// </summary>
        event OnGetDisplayMessagesResponseSentDelegate   OnGetDisplayMessagesResponse;

        #endregion

        #region OnClearDisplayMessage

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        event OnClearDisplayMessageRequestReceivedDelegate    OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a clear display message request was sent.
        /// </summary>
        event OnClearDisplayMessageResponseSentDelegate   OnClearDisplayMessageResponse;

        #endregion

        #region OnCostUpdated

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        event OnCostUpdatedRequestReceivedDelegate    OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a response to a cost updated request was sent.
        /// </summary>
        event OnCostUpdatedResponseSentDelegate   OnCostUpdatedResponse;

        #endregion

        #region OnCustomerInformation

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        event OnCustomerInformationRequestReceivedDelegate    OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a customer information request was sent.
        /// </summary>
        event OnCustomerInformationResponseSentDelegate   OnCustomerInformationResponse;

        #endregion


    }

}
