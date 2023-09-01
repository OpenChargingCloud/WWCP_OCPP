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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// Extention methods for all charging station servers.
    /// </summary>
    public static class IChargingStationServerExtensions
    {



    }


    /// <summary>
    /// The common interface of all charging station servers.
    /// </summary>
    public interface IChargingStationServer
    {

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        event OnResetRequestDelegate    OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset was received.
        /// </summary>
        event OnResetDelegate           OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        event OnResetResponseDelegate   OnResetResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        event OnUpdateFirmwareRequestDelegate    OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an update firmware was received.
        /// </summary>
        event OnUpdateFirmwareDelegate           OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        event OnUpdateFirmwareResponseDelegate   OnUpdateFirmwareResponse;

        #endregion

        #region OnPublishFirmware

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        event OnPublishFirmwareRequestDelegate    OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a publish firmware was received.
        /// </summary>
        event OnPublishFirmwareDelegate           OnPublishFirmware;

        /// <summary>
        /// An event sent whenever a response to a publish firmware request was sent.
        /// </summary>
        event OnPublishFirmwareResponseDelegate   OnPublishFirmwareResponse;

        #endregion

        #region OnUnpublishFirmware

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        event OnUnpublishFirmwareRequestDelegate    OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware was received.
        /// </summary>
        event OnUnpublishFirmwareDelegate           OnUnpublishFirmware;

        /// <summary>
        /// An event sent whenever a response to an unpublish firmware request was sent.
        /// </summary>
        event OnUnpublishFirmwareResponseDelegate   OnUnpublishFirmwareResponse;

        #endregion

        #region OnGetBaseReport

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        event OnGetBaseReportRequestDelegate    OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a get base report was received.
        /// </summary>
        event OnGetBaseReportDelegate           OnGetBaseReport;

        /// <summary>
        /// An event sent whenever a response to a get base request was sent.
        /// </summary>
        event OnGetBaseReportResponseDelegate   OnGetBaseReportResponse;

        #endregion

        #region OnGetReport

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        event OnGetReportRequestDelegate    OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a get report was received.
        /// </summary>
        event OnGetReportDelegate           OnGetReport;

        /// <summary>
        /// An event sent whenever a response to a get report request was sent.
        /// </summary>
        event OnGetReportResponseDelegate   OnGetReportResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        event OnGetLogRequestDelegate    OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a get log was received.
        /// </summary>
        event OnGetLogDelegate           OnGetLog;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        event OnGetLogResponseDelegate   OnGetLogResponse;

        #endregion

        #region OnSetVariables

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        event OnSetVariablesRequestDelegate    OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a set variables was received.
        /// </summary>
        event OnSetVariablesDelegate           OnSetVariables;

        /// <summary>
        /// An event sent whenever a response to a set variables request was sent.
        /// </summary>
        event OnSetVariablesResponseDelegate   OnSetVariablesResponse;

        #endregion

        #region OnGetVariables

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        event OnGetVariablesRequestDelegate    OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a get variables was received.
        /// </summary>
        event OnGetVariablesDelegate           OnGetVariables;

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        event OnGetVariablesResponseDelegate   OnGetVariablesResponse;

        #endregion

        #region OnSetMonitoringBase

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        event OnSetMonitoringBaseRequestDelegate    OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a set monitoring base was received.
        /// </summary>
        event OnSetMonitoringBaseDelegate           OnSetMonitoringBase;

        /// <summary>
        /// An event sent whenever a response to a set monitoring base request was sent.
        /// </summary>
        event OnSetMonitoringBaseResponseDelegate   OnSetMonitoringBaseResponse;

        #endregion

        #region OnGetMonitoringReport

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        event OnGetMonitoringReportRequestDelegate    OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report was received.
        /// </summary>
        event OnGetMonitoringReportDelegate           OnGetMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a get monitoring report request was sent.
        /// </summary>
        event OnGetMonitoringReportResponseDelegate   OnGetMonitoringReportResponse;

        #endregion

        #region OnSetMonitoringLevel

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        event OnSetMonitoringLevelRequestDelegate    OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a set monitoring level was received.
        /// </summary>
        event OnSetMonitoringLevelDelegate           OnSetMonitoringLevel;

        /// <summary>
        /// An event sent whenever a response to a set monitoring level request was sent.
        /// </summary>
        event OnSetMonitoringLevelResponseDelegate   OnSetMonitoringLevelResponse;

        #endregion

        #region OnSetVariableMonitoring

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringRequestDelegate    OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring was received.
        /// </summary>
        event OnSetVariableMonitoringDelegate           OnSetVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        event OnSetVariableMonitoringResponseDelegate   OnSetVariableMonitoringResponse;

        #endregion

        #region OnClearVariableMonitoring

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringRequestDelegate    OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring was received.
        /// </summary>
        event OnClearVariableMonitoringDelegate           OnClearVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a clear variable monitoring request was sent.
        /// </summary>
        event OnClearVariableMonitoringResponseDelegate   OnClearVariableMonitoringResponse;

        #endregion

        #region OnSetNetworkProfile

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        event OnSetNetworkProfileRequestDelegate    OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a set network profile was received.
        /// </summary>
        event OnSetNetworkProfileDelegate           OnSetNetworkProfile;

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        event OnSetNetworkProfileResponseDelegate   OnSetNetworkProfileResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        event OnChangeAvailabilityRequestDelegate    OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a change availability was received.
        /// </summary>
        event OnChangeAvailabilityDelegate           OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        event OnChangeAvailabilityResponseDelegate   OnChangeAvailabilityResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        event OnTriggerMessageRequestDelegate    OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a trigger message was received.
        /// </summary>
        event OnTriggerMessageDelegate           OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        event OnTriggerMessageResponseDelegate   OnTriggerMessageResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer was received.
        /// </summary>
        event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        event OnCertificateSignedRequestDelegate    OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a certificate signed was received.
        /// </summary>
        event OnCertificateSignedDelegate           OnCertificateSigned;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        event OnCertificateSignedResponseDelegate   OnCertificateSignedResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        event OnInstallCertificateRequestDelegate    OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever an install certificate was received.
        /// </summary>
        event OnInstallCertificateDelegate           OnInstallCertificate;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        event OnInstallCertificateResponseDelegate   OnInstallCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestDelegate    OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids was received.
        /// </summary>
        event OnGetInstalledCertificateIdsDelegate           OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseDelegate   OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        event OnDeleteCertificateRequestDelegate    OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a delete certificate was received.
        /// </summary>
        event OnDeleteCertificateDelegate           OnDeleteCertificate;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        event OnDeleteCertificateResponseDelegate   OnDeleteCertificateResponse;

        #endregion

        #region OnNotifyCRL

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        event OnNotifyCRLRequestDelegate    OnNotifyCRLRequest;

        /// <summary>
        /// An event sent whenever a delete certificate was received.
        /// </summary>
        event OnNotifyCRLDelegate           OnNotifyCRL;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        event OnNotifyCRLResponseDelegate   OnNotifyCRLResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a get local list version request was received.
        /// </summary>
        event OnGetLocalListVersionRequestDelegate    OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a get local list version was received.
        /// </summary>
        event OnGetLocalListVersionDelegate           OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a get local list version request was sent.
        /// </summary>
        event OnGetLocalListVersionResponseDelegate   OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a send local list request was received.
        /// </summary>
        event OnSendLocalListRequestDelegate    OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a send local list was received.
        /// </summary>
        event OnSendLocalListDelegate           OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a send local list request was sent.
        /// </summary>
        event OnSendLocalListResponseDelegate   OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a clear cache request was received.
        /// </summary>
        event OnClearCacheRequestDelegate    OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a clear cache was received.
        /// </summary>
        event OnClearCacheDelegate           OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a clear cache request was sent.
        /// </summary>
        event OnClearCacheResponseDelegate   OnClearCacheResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        event OnReserveNowRequestDelegate    OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now was received.
        /// </summary>
        event OnReserveNowDelegate           OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        event OnReserveNowResponseDelegate   OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        event OnCancelReservationRequestDelegate    OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation was received.
        /// </summary>
        event OnCancelReservationDelegate           OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        event OnCancelReservationResponseDelegate   OnCancelReservationResponse;

        #endregion

        #region OnRequestStartTransaction

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        event OnRequestStartTransactionRequestDelegate    OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a request start transaction was received.
        /// </summary>
        event OnRequestStartTransactionDelegate           OnRequestStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a request start transaction request was sent.
        /// </summary>
        event OnRequestStartTransactionResponseDelegate   OnRequestStartTransactionResponse;

        #endregion

        #region OnRequestStopTransaction

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        event OnRequestStopTransactionRequestDelegate    OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction was received.
        /// </summary>
        event OnRequestStopTransactionDelegate           OnRequestStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a request stop transaction request was sent.
        /// </summary>
        event OnRequestStopTransactionResponseDelegate   OnRequestStopTransactionResponse;

        #endregion

        #region OnGetTransactionStatus

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        event OnGetTransactionStatusRequestDelegate    OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a get transaction status was received.
        /// </summary>
        event OnGetTransactionStatusDelegate           OnGetTransactionStatus;

        /// <summary>
        /// An event sent whenever a response to a get transaction status request was sent.
        /// </summary>
        event OnGetTransactionStatusResponseDelegate   OnGetTransactionStatusResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        event OnSetChargingProfileRequestDelegate    OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a set charging profile was received.
        /// </summary>
        event OnSetChargingProfileDelegate           OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        event OnSetChargingProfileResponseDelegate   OnSetChargingProfileResponse;

        #endregion

        #region OnGetChargingProfiles

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        event OnGetChargingProfilesRequestDelegate    OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles was received.
        /// </summary>
        event OnGetChargingProfilesDelegate           OnGetChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a get charging profiles request was sent.
        /// </summary>
        event OnGetChargingProfilesResponseDelegate   OnGetChargingProfilesResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        event OnClearChargingProfileRequestDelegate    OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a clear charging profile was received.
        /// </summary>
        event OnClearChargingProfileDelegate           OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        event OnClearChargingProfileResponseDelegate   OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleRequestDelegate    OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule was received.
        /// </summary>
        event OnGetCompositeScheduleDelegate           OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        event OnGetCompositeScheduleResponseDelegate   OnGetCompositeScheduleResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestDelegate    OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferDelegate           OnNotifyAllowedEnergyTransfer;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseDelegate   OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorRequestDelegate    OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever an UnlockConnector was received.
        /// </summary>
        event OnUnlockConnectorDelegate           OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        event OnUnlockConnectorResponseDelegate   OnUnlockConnectorResponse;

        #endregion


        #region OnAFRRSignal

        /// <summary>
        /// An event sent whenever an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalRequestDelegate    OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever an AFRR signal was received.
        /// </summary>
        event OnAFRRSignalDelegate           OnAFRRSignal;

        /// <summary>
        /// An event sent whenever a response to an AFRR signal request was sent.
        /// </summary>
        event OnAFRRSignalResponseDelegate   OnAFRRSignalResponse;

        #endregion


        #region OnSetDisplayMessage

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        event OnSetDisplayMessageRequestDelegate    OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a set display message was received.
        /// </summary>
        event OnSetDisplayMessageDelegate           OnSetDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        event OnSetDisplayMessageResponseDelegate   OnSetDisplayMessageResponse;

        #endregion

        #region OnGetDisplayMessages

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        event OnGetDisplayMessagesRequestDelegate    OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a get display messages was received.
        /// </summary>
        event OnGetDisplayMessagesDelegate           OnGetDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a get display messages request was sent.
        /// </summary>
        event OnGetDisplayMessagesResponseDelegate   OnGetDisplayMessagesResponse;

        #endregion

        #region OnClearDisplayMessage

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        event OnClearDisplayMessageRequestDelegate    OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a clear display message was received.
        /// </summary>
        event OnClearDisplayMessageDelegate           OnClearDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a clear display message request was sent.
        /// </summary>
        event OnClearDisplayMessageResponseDelegate   OnClearDisplayMessageResponse;

        #endregion

        #region OnCostUpdated

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        event OnCostUpdatedRequestDelegate    OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a cost updated was received.
        /// </summary>
        event OnCostUpdatedDelegate           OnCostUpdated;

        /// <summary>
        /// An event sent whenever a response to a cost updated request was sent.
        /// </summary>
        event OnCostUpdatedResponseDelegate   OnCostUpdatedResponse;

        #endregion

        #region OnCustomerInformation

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        event OnCustomerInformationRequestDelegate    OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a customer information was received.
        /// </summary>
        event OnCustomerInformationDelegate           OnCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a customer information request was sent.
        /// </summary>
        event OnCustomerInformationResponseDelegate   OnCustomerInformationResponse;

        #endregion


    }

}
