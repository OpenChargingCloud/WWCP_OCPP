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

using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all forwarding OCPP messages processors.
    /// </summary>
    public interface IOCPPWebSocketAdapterFORWARD
    {

        #region Properties

        ForwardingResults           DefaultResult        { get; set; }

        HashSet<NetworkingNode_Id>  AnycastIdsAllowed    { get; }

        HashSet<NetworkingNode_Id>  AnycastIdsDenied     { get; }

        #endregion

        #region Events

        #region Common

        #region BinaryDataStreamsExtensions

        event OnDeleteFileRequestFilterDelegate?                            OnDeleteFileRequest;
        event OnDeleteFileRequestFilteredDelegate?                          OnDeleteFileRequestLogging;

        event OnGetFileRequestFilterDelegate?                               OnGetFileRequest;
        event OnGetFileRequestFilteredDelegate?                             OnGetFileRequestLogging;

        event OnListDirectoryRequestFilterDelegate?                         OnListDirectoryRequest;
        event OnListDirectoryRequestFilteredDelegate?                       OnListDirectoryRequestLogging;

        event OnSendFileRequestFilterDelegate?                              OnSendFileRequest;
        event OnSendFileRequestFilteredDelegate?                            OnSendFileRequestLogging;

        #endregion

        #region E2ESecurityExtensions

        event OnAddSignaturePolicyRequestFilterDelegate?                    OnAddSignaturePolicyRequest;
        event OnAddSignaturePolicyRequestFilteredDelegate?                  OnAddSignaturePolicyRequestLogging;

        event OnAddUserRoleRequestFilterDelegate?                           OnAddUserRoleRequest;
        event OnAddUserRoleRequestFilteredDelegate?                         OnAddUserRoleRequestLogging;

        event OnDeleteSignaturePolicyRequestFilterDelegate?                 OnDeleteSignaturePolicyRequest;
        event OnDeleteSignaturePolicyRequestFilteredDelegate?               OnDeleteSignaturePolicyRequestLogging;

        event OnDeleteUserRoleRequestFilterDelegate?                        OnDeleteUserRoleRequest;
        event OnDeleteUserRoleRequestFilteredDelegate?                      OnDeleteUserRoleRequestLogging;

        event OnUpdateSignaturePolicyRequestFilterDelegate?                 OnUpdateSignaturePolicyRequest;
        event OnUpdateSignaturePolicyRequestFilteredDelegate?               OnUpdateSignaturePolicyRequestLogging;

        event OnUpdateUserRoleRequestFilterDelegate?                        OnUpdateUserRoleRequest;
        event OnUpdateUserRoleRequestFilteredDelegate?                      OnUpdateUserRoleRequestLogging;

        #endregion

        #region DataTransfers

        event OnDataTransferRequestReceivedDelegate                         OnDataTransferRequestReceived;
        event OnDataTransferRequestFilterDelegate?                          OnDataTransferRequestFilter;
        event OnDataTransferRequestFilteredDelegate?                        OnDataTransferRequestFiltered;
        event OnDataTransferRequestSentDelegate?                            OnDataTransferRequestSent;
        event OnDataTransferResponseReceivedDelegate?                       OnDataTransferResponseReceived;
        event OnDataTransferResponseSentDelegate?                           OnDataTransferResponseSent;

        event OnBinaryDataTransferRequestReceivedDelegate                   OnBinaryDataTransferRequestReceived;
        event OnBinaryDataTransferRequestFilterDelegate?                    OnBinaryDataTransferRequestFilter;
        event OnBinaryDataTransferRequestFilteredDelegate?                  OnBinaryDataTransferRequestFiltered;
        event OnBinaryDataTransferRequestSentDelegate?                      OnBinaryDataTransferRequestSent;
        event OnBinaryDataTransferResponseReceivedDelegate?                 OnBinaryDataTransferResponseReceived;
        event OnBinaryDataTransferResponseSentDelegate?                     OnBinaryDataTransferResponseSent;

        event OnSecureDataTransferRequestReceivedDelegate                   OnSecureDataTransferRequestReceived;
        event OnSecureDataTransferRequestFilterDelegate?                    OnSecureDataTransferRequestFilter;
        event OnSecureDataTransferRequestFilteredDelegate?                  OnSecureDataTransferRequestFiltered;
        event OnSecureDataTransferRequestSentDelegate?                      OnSecureDataTransferRequestSent;
        event OnSecureDataTransferResponseReceivedDelegate?                 OnSecureDataTransferResponseReceived;
        event OnSecureDataTransferResponseSentDelegate?                     OnSecureDataTransferResponseSent;

        #endregion

        #endregion

        #region CS

        #region Certificates

        event OnGet15118EVCertificateRequestFilterDelegate?                 OnGet15118EVCertificateRequest;
        event OnGet15118EVCertificateRequestFilteredDelegate?               OnGet15118EVCertificateRequestLogging;

        event OnGetCertificateStatusRequestFilterDelegate?                  OnGetCertificateStatusRequest;
        event OnGetCertificateStatusRequestFilteredDelegate?                OnGetCertificateStatusRequestLogging;

        event OnGetCRLRequestFilterDelegate?                                OnGetCRLRequest;
        event OnGetCRLRequestFilteredDelegate?                              OnGetCRLRequestLogging;

        event OnSignCertificateRequestFilterDelegate?                       OnSignCertificateRequest;
        event OnSignCertificateRequestFilteredDelegate?                     OnSignCertificateRequestLogging;

        #endregion

        #region Charging

        event OnAuthorizeRequestFilterDelegate?                             OnAuthorizeRequest;
        event OnAuthorizeRequestFilteredDelegate?                           OnAuthorizeRequestLogging;

        event OnClearedChargingLimitRequestFilterDelegate?                  OnClearedChargingLimitRequest;
        event OnClearedChargingLimitRequestFilteredDelegate?                OnClearedChargingLimitRequestLogging;

        event OnMeterValuesRequestFilterDelegate?                           OnMeterValuesRequest;
        event OnMeterValuesRequestFilteredDelegate?                         OnMeterValuesRequestLogging;

        event OnNotifyChargingLimitRequestFilterDelegate?                   OnNotifyChargingLimitRequest;
        event OnNotifyChargingLimitRequestFilteredDelegate?                 OnNotifyChargingLimitRequestLogging;

        event OnNotifyEVChargingNeedsRequestFilterDelegate?                 OnNotifyEVChargingNeedsRequest;
        event OnNotifyEVChargingNeedsRequestFilteredDelegate?               OnNotifyEVChargingNeedsRequestLogging;

        event OnNotifyEVChargingScheduleRequestFilterDelegate?              OnNotifyEVChargingScheduleRequest;
        event OnNotifyEVChargingScheduleRequestFilteredDelegate?            OnNotifyEVChargingScheduleRequestLogging;

        event OnNotifyPriorityChargingRequestFilterDelegate?                OnNotifyPriorityChargingRequest;
        event OnNotifyPriorityChargingRequestFilteredDelegate?              OnNotifyPriorityChargingRequestLogging;

        event OnPullDynamicScheduleUpdateRequestFilterDelegate?             OnPullDynamicScheduleUpdateRequest;
        event OnPullDynamicScheduleUpdateRequestFilteredDelegate?           OnPullDynamicScheduleUpdateRequestLogging;

        event OnReportChargingProfilesRequestFilterDelegate?                OnReportChargingProfilesRequest;
        event OnReportChargingProfilesRequestFilteredDelegate?              OnReportChargingProfilesRequestLogging;

        event OnReservationStatusUpdateRequestFilterDelegate?               OnReservationStatusUpdateRequest;
        event OnReservationStatusUpdateRequestFilteredDelegate?             OnReservationStatusUpdateRequestLogging;

        event OnStatusNotificationRequestFilterDelegate?                    OnStatusNotificationRequest;
        event OnStatusNotificationRequestFilteredDelegate?                  OnStatusNotificationRequestLogging;

        event OnTransactionEventRequestFilterDelegate?                      OnTransactionEventRequest;
        event OnTransactionEventRequestFilteredDelegate?                    OnTransactionEventRequestLogging;

        #endregion

        #region Customer

        event OnNotifyCustomerInformationRequestFilterDelegate?             OnNotifyCustomerInformationRequest;
        event OnNotifyCustomerInformationRequestFilteredDelegate?           OnNotifyCustomerInformationRequestLogging;

        event OnNotifyDisplayMessagesRequestFilterDelegate?                 OnNotifyDisplayMessagesRequest;
        event OnNotifyDisplayMessagesRequestFilteredDelegate?               OnNotifyDisplayMessagesRequestLogging;

        #endregion

        #region DeviceModel

        event OnLogStatusNotificationRequestFilterDelegate?                 OnLogStatusNotificationRequest;
        event OnLogStatusNotificationRequestFilteredDelegate?               OnLogStatusNotificationRequestLogging;

        event OnNotifyEventRequestFilterDelegate?                           OnNotifyEventRequest;
        event OnNotifyEventRequestFilteredDelegate?                         OnNotifyEventRequestLogging;

        event OnNotifyMonitoringReportRequestFilterDelegate?                OnNotifyMonitoringReportRequest;
        event OnNotifyMonitoringReportRequestFilteredDelegate?              OnNotifyMonitoringReportRequestLogging;

        event OnNotifyReportRequestFilterDelegate?                          OnNotifyReportRequest;
        event OnNotifyReportRequestFilteredDelegate?                        OnNotifyReportRequestLogging;

        event OnSecurityEventNotificationRequestFilterDelegate?             OnSecurityEventNotificationRequest;
        event OnSecurityEventNotificationRequestFilteredDelegate?           OnSecurityEventNotificationRequestLogging;

        #endregion

        #region Firmware

        event OnBootNotificationRequestReceivedDelegate                     OnBootNotificationRequestReceived;
        event OnBootNotificationRequestFilterDelegate?                      OnBootNotificationRequestFilter;
        event OnBootNotificationRequestFilteredDelegate?                    OnBootNotificationRequestFiltered;
        event OnBootNotificationRequestSentDelegate?                        OnBootNotificationRequestSent;
        event OnBootNotificationResponseReceivedDelegate?                   OnBootNotificationResponseReceived;
        event OnBootNotificationResponseSentDelegate?                       OnBootNotificationResponseSent;

        event OnFirmwareStatusNotificationRequestFilterDelegate?            OnFirmwareStatusNotificationRequestFilter;
        event OnFirmwareStatusNotificationRequestFilteredDelegate?          OnFirmwareStatusNotificationRequestFiltered;

        event OnHeartbeatRequestFilterDelegate?                             OnHeartbeatRequestFilter;
        event OnHeartbeatRequestFilteredDelegate?                           OnHeartbeatRequestFiltered;

        event OnPublishFirmwareStatusNotificationRequestFilterDelegate?     OnPublishFirmwareStatusNotificationRequestFilter;
        event OnPublishFirmwareStatusNotificationRequestFilteredDelegate?   OnPublishFirmwareStatusNotificationRequestFiltered;

        #endregion

        #endregion

        #region CSMS

        #region Certificates

        event OnCertificateSignedRequestFilterDelegate?                     OnCertificateSignedRequest;
        event OnCertificateSignedRequestFilteredDelegate?                   OnCertificateSignedRequestLogging;

        event OnDeleteCertificateRequestFilterDelegate?                     OnDeleteCertificateRequest;
        event OnDeleteCertificateRequestFilteredDelegate?                   OnDeleteCertificateRequestLogging;

        event OnGetInstalledCertificateIdsRequestFilterDelegate?            OnGetInstalledCertificateIdsRequest;
        event OnGetInstalledCertificateIdsRequestFilteredDelegate?          OnGetInstalledCertificateIdsRequestLogging;

        event OnInstallCertificateRequestFilterDelegate?                    OnInstallCertificateRequest;
        event OnInstallCertificateRequestFilteredDelegate?                  OnInstallCertificateRequestLogging;

        event OnNotifyCRLRequestFilterDelegate?                             OnNotifyCRLRequest;
        event OnNotifyCRLRequestFilteredDelegate?                           OnNotifyCRLRequestLogging;

        #endregion

        #region Charging

        event OnCancelReservationRequestFilterDelegate?                     OnCancelReservationRequest;
        event OnCancelReservationRequestFilteredDelegate?                   OnCancelReservationRequestLogging;

        event OnClearChargingProfileRequestFilterDelegate?                  OnClearChargingProfileRequest;
        event OnClearChargingProfileRequestFilteredDelegate?                OnClearChargingProfileRequestLogging;

        event OnGetChargingProfilesRequestFilterDelegate?                   OnGetChargingProfilesRequest;
        event OnGetChargingProfilesRequestFilteredDelegate?                 OnGetChargingProfilesRequestLogging;

        event OnGetCompositeScheduleRequestFilterDelegate?                  OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleRequestFilteredDelegate?                OnGetCompositeScheduleRequestLogging;

        event OnGetTransactionStatusRequestFilterDelegate?                  OnGetTransactionStatusRequest;
        event OnGetTransactionStatusRequestFilteredDelegate?                OnGetTransactionStatusRequestLogging;

        event OnNotifyAllowedEnergyTransferRequestFilterDelegate?           OnNotifyAllowedEnergyTransferRequest;
        event OnNotifyAllowedEnergyTransferRequestFilteredDelegate?         OnNotifyAllowedEnergyTransferRequestLogging;

        event OnRequestStartTransactionRequestFilterDelegate?               OnRequestStartTransactionRequest;
        event OnRequestStartTransactionRequestFilteredDelegate?             OnRequestStartTransactionRequestLogging;

        event OnRequestStopTransactionRequestFilterDelegate?                OnRequestStopTransactionRequest;
        event OnRequestStopTransactionRequestFilteredDelegate?              OnRequestStopTransactionRequestLogging;

        event OnReserveNowRequestFilterDelegate?                            OnReserveNowRequest;
        event OnReserveNowRequestFilteredDelegate?                          OnReserveNowRequestLogging;

        event OnSetChargingProfileRequestFilterDelegate?                    OnSetChargingProfileRequest;
        event OnSetChargingProfileRequestFilteredDelegate?                  OnSetChargingProfileRequestLogging;

        event OnUnlockConnectorRequestFilterDelegate?                       OnUnlockConnectorRequest;
        event OnUnlockConnectorRequestFilteredDelegate?                     OnUnlockConnectorRequestLogging;

        event OnUpdateDynamicScheduleRequestFilterDelegate?                 OnUpdateDynamicScheduleRequest;
        event OnUpdateDynamicScheduleRequestFilteredDelegate?               OnUpdateDynamicScheduleRequestLogging;

        event OnUsePriorityChargingRequestFilterDelegate?                   OnUsePriorityChargingRequest;
        event OnUsePriorityChargingRequestFilteredDelegate?                 OnUsePriorityChargingRequestLogging;

        #endregion

        #region Customer

        event OnClearDisplayMessageRequestFilterDelegate?                   OnClearDisplayMessageRequest;
        event OnClearDisplayMessageRequestFilteredDelegate?                 OnClearDisplayMessageRequestLogging;

        event OnCostUpdatedRequestFilterDelegate?                           OnCostUpdatedRequest;
        event OnCostUpdatedRequestFilteredDelegate?                         OnCostUpdatedRequestLogging;

        event OnCustomerInformationRequestFilterDelegate?                   OnCustomerInformationRequest;
        event OnCustomerInformationRequestFilteredDelegate?                 OnCustomerInformationRequestLogging;

        event OnGetDisplayMessagesRequestFilterDelegate?                    OnGetDisplayMessagesRequest;
        event OnGetDisplayMessagesRequestFilteredDelegate?                  OnGetDisplayMessagesRequestLogging;

        event OnSetDisplayMessageRequestFilterDelegate?                     OnSetDisplayMessageRequest;
        event OnSetDisplayMessageRequestFilteredDelegate?                   OnSetDisplayMessageRequestLogging;

        #endregion

        #region DeviceModel

        event OnChangeAvailabilityRequestFilterDelegate?                    OnChangeAvailabilityRequest;
        event OnChangeAvailabilityRequestFilteredDelegate?                  OnChangeAvailabilityRequestLogging;

        event OnClearVariableMonitoringRequestFilterDelegate?               OnClearVariableMonitoringRequest;
        event OnClearVariableMonitoringRequestFilteredDelegate?             OnClearVariableMonitoringRequestLogging;

        event OnGetBaseReportRequestFilterDelegate?                         OnGetBaseReportRequest;
        event OnGetBaseReportRequestFilteredDelegate?                       OnGetBaseReportRequestLogging;

        event OnGetLogRequestFilterDelegate?                                OnGetLogRequest;
        event OnGetLogRequestFilteredDelegate?                              OnGetLogRequestLogging;

        event OnGetMonitoringReportRequestFilterDelegate?                   OnGetMonitoringReportRequest;
        event OnGetMonitoringReportRequestFilteredDelegate?                 OnGetMonitoringReportRequestLogging;

        event OnGetReportRequestFilterDelegate?                             OnGetReportRequest;
        event OnGetReportRequestFilteredDelegate?                           OnGetReportRequestLogging;

        event OnGetVariablesRequestFilterDelegate?                          OnGetVariablesRequest;
        event OnGetVariablesRequestFilteredDelegate?                        OnGetVariablesRequestLogging;

        event OnSetMonitoringBaseRequestFilterDelegate?                     OnSetMonitoringBaseRequest;
        event OnSetMonitoringBaseRequestFilteredDelegate?                   OnSetMonitoringBaseRequestLogging;

        event OnSetMonitoringLevelRequestFilterDelegate?                    OnSetMonitoringLevelRequest;
        event OnSetMonitoringLevelRequestFilteredDelegate?                  OnSetMonitoringLevelRequestLogging;

        event OnSetNetworkProfileRequestFilterDelegate?                     OnSetNetworkProfileRequest;
        event OnSetNetworkProfileRequestFilteredDelegate?                   OnSetNetworkProfileRequestLogging;

        event OnSetVariableMonitoringRequestFilterDelegate?                 OnSetVariableMonitoringRequest;
        event OnSetVariableMonitoringRequestFilteredDelegate?               OnSetVariableMonitoringRequestLogging;

        event OnSetVariablesRequestFilterDelegate?                          OnSetVariablesRequest;
        event OnSetVariablesRequestFilteredDelegate?                        OnSetVariablesRequestLogging;

        event OnTriggerMessageRequestFilterDelegate?                        OnTriggerMessageRequest;
        event OnTriggerMessageRequestFilteredDelegate?                      OnTriggerMessageRequestLogging;

        #endregion

        #region E2EChargingTariffsExtensions

        event OnGetDefaultChargingTariffRequestFilterDelegate?              OnGetDefaultChargingTariffRequest;
        event OnGetDefaultChargingTariffRequestFilteredDelegate?            OnGetDefaultChargingTariffRequestLogging;

        event OnRemoveDefaultChargingTariffRequestFilterDelegate?           OnRemoveDefaultChargingTariffRequest;
        event OnRemoveDefaultChargingTariffRequestFilteredDelegate?         OnRemoveDefaultChargingTariffRequestLogging;

        event OnSetDefaultChargingTariffRequestFilterDelegate?              OnSetDefaultChargingTariffRequest;
        event OnSetDefaultChargingTariffRequestFilteredDelegate?            OnSetDefaultChargingTariffRequestLogging;

        #endregion

        #region Firmware

        event OnPublishFirmwareRequestFilterDelegate?                       OnPublishFirmwareRequest;
        event OnPublishFirmwareRequestFilteredDelegate?                     OnPublishFirmwareRequestLogging;

        event OnResetRequestFilterDelegate?                                 OnResetRequest;
        event OnResetRequestFilteredDelegate?                               OnResetRequestLogging;

        event OnUnpublishFirmwareRequestFilterDelegate?                     OnUnpublishFirmwareRequest;
        event OnUnpublishFirmwareRequestFilteredDelegate?                   OnUnpublishFirmwareRequestLogging;

        event OnUpdateFirmwareRequestFilterDelegate?                        OnUpdateFirmwareRequest;
        event OnUpdateFirmwareRequestFilteredDelegate?                      OnUpdateFirmwareRequestLogging;

        #endregion

        #region Grid

        event OnAFRRSignalRequestFilterDelegate?                            OnAFRRSignalRequest;
        event OnAFRRSignalRequestFilteredDelegate?                          OnAFRRSignalRequestLogging;

        #endregion

        #region LocalList

        event OnClearCacheRequestFilterDelegate?                            OnClearCacheRequest;
        event OnClearCacheRequestFilteredDelegate?                          OnClearCacheRequestLogging;

        event OnGetLocalListVersionRequestFilterDelegate?                   OnGetLocalListVersionRequest;
        event OnGetLocalListVersionRequestFilteredDelegate?                 OnGetLocalListVersionRequestLogging;

        event OnSendLocalListRequestFilterDelegate?                         OnSendLocalListRequest;
        event OnSendLocalListRequestFilteredDelegate?                       OnSendLocalListRequestLogging;

        #endregion

        #endregion

        #endregion


        NetworkingNode_Id? GetForwardedNodeId(Request_Id RequestId);


        Task ProcessJSONRequestMessage       (OCPP_JSONRequestMessage        JSONRequestMessage);
        Task ProcessJSONResponseMessage      (OCPP_JSONResponseMessage       JSONResponseMessage);
        Task ProcessJSONRequestErrorMessage  (OCPP_JSONRequestErrorMessage   JSONRequestErrorMessage);
        Task ProcessJSONResponseErrorMessage (OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage);


        Task ProcessBinaryRequestMessage     (OCPP_BinaryRequestMessage      BinaryRequestMessage);
        Task ProcessBinaryResponseMessage    (OCPP_BinaryResponseMessage     BinaryResponseMessage);


        #region Common

        #region BinaryDataStreamsExtensions

        Task<ForwardingDecision> Forward_DeleteFile                        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetFile                           (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ListDirectory                     (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SendFile                          (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region E2ESecurityExtensions

        Task<ForwardingDecision> Forward_AddSignaturePolicy                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_AddUserRole                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region DataTransfers

        Task<ForwardingDecision>                 Forward_DataTransfer                      (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision>                 Forward_BinaryDataTransfer                (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision>                 Forward_SecureDataTransfer                (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #endregion

        #region CS

        #region Certificates
        Task<ForwardingDecision> Forward_Get15118EVCertificate             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCertificateStatus              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCRL                            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SignCertificate                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Charging
        Task<ForwardingDecision> Forward_Authorize                         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearedChargingLimit              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_MeterValues                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyChargingLimit               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEVChargingNeeds             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEVChargingSchedule          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyPriorityCharging            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_PullDynamicScheduleUpdate         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReportChargingProfiles            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReservationStatusUpdate           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_StatusNotification                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_TransactionEvent                  (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Customer
        Task<ForwardingDecision> Forward_NotifyCustomerInformation         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyDisplayMessages             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region DeviceModel
        Task<ForwardingDecision> Forward_LogStatusNotification             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEvent                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyMonitoringReport            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyReport                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SecurityEventNotification         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Firmware

        Task<ForwardingDecision> Forward_BootNotification                  (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_FirmwareStatusNotification        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_Heartbeat                         (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_PublishFirmwareStatusNotification (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #endregion

        #region CSMS

        #region Certificates
        Task<ForwardingDecision> Forward_CertificateSigned                 (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteCertificate                 (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetInstalledCertificateIds        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_InstallCertificate                (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyCRL                         (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Charging

        Task<ForwardingDecision> Forward_CancelReservation                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearChargingProfile              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetChargingProfiles               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCompositeSchedule              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetTransactionStatus              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyAllowedEnergyTransfer       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RequestStartTransaction           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RequestStopTransaction            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReserveNow                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetChargingProfile                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UnlockConnector                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateDynamicSchedule             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UsePriorityCharging               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Customer

        Task<ForwardingDecision> Forward_ClearDisplayMessage               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_CostUpdated                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_CustomerInformation               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetDisplayMessages                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetDisplayMessage                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region DeviceModel
        Task<ForwardingDecision> Forward_ChangeAvailability                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearVariableMonitoring           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetBaseReport                     (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetLog                            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetMonitoringReport               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetReport                         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetVariables                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetMonitoringBase                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetMonitoringLevel                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetNetworkProfile                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetVariableMonitoring             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetVariables                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_TriggerMessage                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region E2EChargingTariffsExtensions

        Task<ForwardingDecision> Forward_GetDefaultChargingTariff          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RemoveDefaultChargingTariff       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetDefaultChargingTariff          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Firmware

        Task<ForwardingDecision> Forward_PublishFirmware                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_Reset                             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UnpublishFirmware                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateFirmware                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Grid
        Task<ForwardingDecision> Forward_AFRRSignal                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region LocalList

        Task<ForwardingDecision> Forward_ClearCache                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetLocalListVersion               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SendLocalList                     (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #endregion


    }

}
