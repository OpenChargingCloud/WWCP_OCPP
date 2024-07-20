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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all forwarding OCPP messages processors.
    /// </summary>
    public interface IOCPPWebSocketAdapterFORWARD
    {

        #region Properties

        ForwardingResults           DefaultForwardingResult    { get; set; }

        HashSet<NetworkingNode_Id>  AnycastIdsAllowed          { get; }

        HashSet<NetworkingNode_Id>  AnycastIdsDenied           { get; }

        #endregion

        #region Events

        #region BinaryDataStreamsExtensions

        event OnDeleteFileRequestFilterDelegate?                            OnDeleteFileRequestFilter;
        event OnDeleteFileRequestFilteredDelegate?                          OnDeleteFileRequestFiltered;

        event OnGetFileRequestFilterDelegate?                               OnGetFileRequestFilter;
        event OnGetFileRequestFilteredDelegate?                             OnGetFileRequestFiltered;

        event OnListDirectoryRequestFilterDelegate?                         OnListDirectoryRequestFilter;
        event OnListDirectoryRequestFilteredDelegate?                       OnListDirectoryRequestFiltered;

        event OnSendFileRequestFilterDelegate?                              OnSendFileRequestFilter;
        event OnSendFileRequestFilteredDelegate?                            OnSendFileRequestFiltered;

        #endregion

        #region E2ESecurityExtensions

        event OnAddSignaturePolicyRequestFilterDelegate?                    OnAddSignaturePolicyRequestFilter;
        event OnAddSignaturePolicyRequestFilteredDelegate?                  OnAddSignaturePolicyRequestFiltered;

        event OnAddUserRoleRequestFilterDelegate?                           OnAddUserRoleRequestFilter;
        event OnAddUserRoleRequestFilteredDelegate?                         OnAddUserRoleRequestFiltered;

        event OnDeleteSignaturePolicyRequestFilterDelegate?                 OnDeleteSignaturePolicyRequestFilter;
        event OnDeleteSignaturePolicyRequestFilteredDelegate?               OnDeleteSignaturePolicyRequestFiltered;

        event OnDeleteUserRoleRequestFilterDelegate?                        OnDeleteUserRoleRequestFilter;
        event OnDeleteUserRoleRequestFilteredDelegate?                      OnDeleteUserRoleRequestFiltered;

        event OnUpdateSignaturePolicyRequestFilterDelegate?                 OnUpdateSignaturePolicyRequestFilter;
        event OnUpdateSignaturePolicyRequestFilteredDelegate?               OnUpdateSignaturePolicyRequestFiltered;

        event OnUpdateUserRoleRequestFilterDelegate?                        OnUpdateUserRoleRequestFilter;
        event OnUpdateUserRoleRequestFilteredDelegate?                      OnUpdateUserRoleRequestFiltered;

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


        #region CS

        #region Certificates

        event OnGet15118EVCertificateRequestReceivedDelegate                 OnGet15118EVCertificateRequestReceived;
        event OnGet15118EVCertificateRequestFilterDelegate?                  OnGet15118EVCertificateRequestFilter;
        event OnGet15118EVCertificateRequestFilteredDelegate?                OnGet15118EVCertificateRequestFiltered;
        event OnGet15118EVCertificateRequestSentDelegate?                    OnGet15118EVCertificateRequestSent;
        event OnGet15118EVCertificateResponseReceivedDelegate?               OnGet15118EVCertificateResponseReceived;
        event OnGet15118EVCertificateResponseSentDelegate?                   OnGet15118EVCertificateResponseSent;

        event OnGetCertificateStatusRequestReceivedDelegate                  OnGetCertificateStatusRequestReceived;
        event OnGetCertificateStatusRequestFilterDelegate?                   OnGetCertificateStatusRequestFilter;
        event OnGetCertificateStatusRequestFilteredDelegate?                 OnGetCertificateStatusRequestFiltered;
        event OnGetCertificateStatusRequestSentDelegate?                     OnGetCertificateStatusRequestSent;
        event OnGetCertificateStatusResponseReceivedDelegate?                OnGetCertificateStatusResponseReceived;
        event OnGetCertificateStatusResponseSentDelegate?                    OnGetCertificateStatusResponseSent;

        event OnGetCRLRequestReceivedDelegate                                OnGetCRLRequestReceived;
        event OnGetCRLRequestFilterDelegate?                                 OnGetCRLRequestFilter;
        event OnGetCRLRequestFilteredDelegate?                               OnGetCRLRequestFiltered;
        event OnGetCRLRequestSentDelegate?                                   OnGetCRLRequestSent;
        event OnGetCRLResponseReceivedDelegate?                              OnGetCRLResponseReceived;
        event OnGetCRLResponseSentDelegate?                                  OnGetCRLResponseSent;

        event OnSignCertificateRequestReceivedDelegate                       OnSignCertificateRequestReceived;
        event OnSignCertificateRequestFilterDelegate?                        OnSignCertificateRequestFilter;
        event OnSignCertificateRequestFilteredDelegate?                      OnSignCertificateRequestFiltered;
        event OnSignCertificateRequestSentDelegate?                          OnSignCertificateRequestSent;
        event OnSignCertificateResponseReceivedDelegate?                     OnSignCertificateResponseReceived;
        event OnSignCertificateResponseSentDelegate?                         OnSignCertificateResponseSent;

        #endregion

        #region Charging

        event OnAuthorizeRequestReceivedDelegate                             OnAuthorizeRequestReceived;
        event OnAuthorizeRequestFilterDelegate?                              OnAuthorizeRequestFilter;
        event OnAuthorizeRequestFilteredDelegate?                            OnAuthorizeRequestFiltered;
        event OnAuthorizeRequestSentDelegate?                                OnAuthorizeRequestSent;
        event OnAuthorizeResponseReceivedDelegate?                           OnAuthorizeResponseReceived;
        event OnAuthorizeResponseSentDelegate?                               OnAuthorizeResponseSent;

        event OnClearedChargingLimitRequestReceivedDelegate                  OnClearedChargingLimitRequestReceived;
        event OnClearedChargingLimitRequestFilterDelegate?                   OnClearedChargingLimitRequestFilter;
        event OnClearedChargingLimitRequestFilteredDelegate?                 OnClearedChargingLimitRequestFiltered;
        event OnClearedChargingLimitRequestSentDelegate?                     OnClearedChargingLimitRequestSent;
        event OnClearedChargingLimitResponseReceivedDelegate?                OnClearedChargingLimitResponseReceived;
        event OnClearedChargingLimitResponseSentDelegate?                    OnClearedChargingLimitResponseSent;

        event OnMeterValuesRequestReceivedDelegate                           OnMeterValuesRequestReceived;
        event OnMeterValuesRequestFilterDelegate?                            OnMeterValuesRequestFilter;
        event OnMeterValuesRequestFilteredDelegate?                          OnMeterValuesRequestFiltered;
        event OnMeterValuesRequestSentDelegate?                              OnMeterValuesRequestSent;
        event OnMeterValuesResponseReceivedDelegate?                         OnMeterValuesResponseReceived;
        event OnMeterValuesResponseSentDelegate?                             OnMeterValuesResponseSent;

        event OnNotifyChargingLimitRequestReceivedDelegate                   OnNotifyChargingLimitRequestReceived;
        event OnNotifyChargingLimitRequestFilterDelegate?                    OnNotifyChargingLimitRequestFilter;
        event OnNotifyChargingLimitRequestFilteredDelegate?                  OnNotifyChargingLimitRequestFiltered;
        event OnNotifyChargingLimitRequestSentDelegate?                      OnNotifyChargingLimitRequestSent;
        event OnNotifyChargingLimitResponseReceivedDelegate?                 OnNotifyChargingLimitResponseReceived;
        event OnNotifyChargingLimitResponseSentDelegate?                     OnNotifyChargingLimitResponseSent;

        event OnNotifyEVChargingNeedsRequestReceivedDelegate                 OnNotifyEVChargingNeedsRequestReceived;
        event OnNotifyEVChargingNeedsRequestFilterDelegate?                  OnNotifyEVChargingNeedsRequestFilter;
        event OnNotifyEVChargingNeedsRequestFilteredDelegate?                OnNotifyEVChargingNeedsRequestFiltered;
        event OnNotifyEVChargingNeedsRequestSentDelegate?                    OnNotifyEVChargingNeedsRequestSent;
        event OnNotifyEVChargingNeedsResponseReceivedDelegate?               OnNotifyEVChargingNeedsResponseReceived;
        event OnNotifyEVChargingNeedsResponseSentDelegate?                   OnNotifyEVChargingNeedsResponseSent;

        event OnNotifyEVChargingScheduleRequestReceivedDelegate              OnNotifyEVChargingScheduleRequestReceived;
        event OnNotifyEVChargingScheduleRequestFilterDelegate?               OnNotifyEVChargingScheduleRequestFilter;
        event OnNotifyEVChargingScheduleRequestFilteredDelegate?             OnNotifyEVChargingScheduleRequestFiltered;
        event OnNotifyEVChargingScheduleRequestSentDelegate?                 OnNotifyEVChargingScheduleRequestSent;
        event OnNotifyEVChargingScheduleResponseReceivedDelegate?            OnNotifyEVChargingScheduleResponseReceived;
        event OnNotifyEVChargingScheduleResponseSentDelegate?                OnNotifyEVChargingScheduleResponseSent;

        event OnNotifyPriorityChargingRequestReceivedDelegate                OnNotifyPriorityChargingRequestReceived;
        event OnNotifyPriorityChargingRequestFilterDelegate?                 OnNotifyPriorityChargingRequestFilter;
        event OnNotifyPriorityChargingRequestFilteredDelegate?               OnNotifyPriorityChargingRequestFiltered;
        event OnNotifyPriorityChargingRequestSentDelegate?                   OnNotifyPriorityChargingRequestSent;
        event OnNotifyPriorityChargingResponseReceivedDelegate?              OnNotifyPriorityChargingResponseReceived;
        event OnNotifyPriorityChargingResponseSentDelegate?                  OnNotifyPriorityChargingResponseSent;

        event OnPullDynamicScheduleUpdateRequestReceivedDelegate             OnPullDynamicScheduleUpdateRequestReceived;
        event OnPullDynamicScheduleUpdateRequestFilterDelegate?              OnPullDynamicScheduleUpdateRequestFilter;
        event OnPullDynamicScheduleUpdateRequestFilteredDelegate?            OnPullDynamicScheduleUpdateRequestFiltered;
        event OnPullDynamicScheduleUpdateRequestSentDelegate?                OnPullDynamicScheduleUpdateRequestSent;
        event OnPullDynamicScheduleUpdateResponseReceivedDelegate?           OnPullDynamicScheduleUpdateResponseReceived;
        event OnPullDynamicScheduleUpdateResponseSentDelegate?               OnPullDynamicScheduleUpdateResponseSent;

        event OnReportChargingProfilesRequestReceivedDelegate                OnReportChargingProfilesRequestReceived;
        event OnReportChargingProfilesRequestFilterDelegate?                 OnReportChargingProfilesRequestFilter;
        event OnReportChargingProfilesRequestFilteredDelegate?               OnReportChargingProfilesRequestFiltered;
        event OnReportChargingProfilesRequestSentDelegate?                   OnReportChargingProfilesRequestSent;
        event OnReportChargingProfilesResponseReceivedDelegate?              OnReportChargingProfilesResponseReceived;
        event OnReportChargingProfilesResponseSentDelegate?                  OnReportChargingProfilesResponseSent;

        event OnReservationStatusUpdateRequestReceivedDelegate               OnReservationStatusUpdateRequestReceived;
        event OnReservationStatusUpdateRequestFilterDelegate?                OnReservationStatusUpdateRequestFilter;
        event OnReservationStatusUpdateRequestFilteredDelegate?              OnReservationStatusUpdateRequestFiltered;
        event OnReservationStatusUpdateRequestSentDelegate?                  OnReservationStatusUpdateRequestSent;
        event OnReservationStatusUpdateResponseReceivedDelegate?             OnReservationStatusUpdateResponseReceived;
        event OnReservationStatusUpdateResponseSentDelegate?                 OnReservationStatusUpdateResponseSent;

        event OnStatusNotificationRequestReceivedDelegate                    OnStatusNotificationRequestReceived;
        event OnStatusNotificationRequestFilterDelegate?                     OnStatusNotificationRequestFilter;
        event OnStatusNotificationRequestFilteredDelegate?                   OnStatusNotificationRequestFiltered;
        event OnStatusNotificationRequestSentDelegate?                       OnStatusNotificationRequestSent;
        event OnStatusNotificationResponseReceivedDelegate?                  OnStatusNotificationResponseReceived;
        event OnStatusNotificationResponseSentDelegate?                      OnStatusNotificationResponseSent;

        event OnTransactionEventRequestReceivedDelegate                      OnTransactionEventRequestReceived;
        event OnTransactionEventRequestFilterDelegate?                       OnTransactionEventRequestFilter;
        event OnTransactionEventRequestFilteredDelegate?                     OnTransactionEventRequestFiltered;
        event OnTransactionEventRequestSentDelegate?                         OnTransactionEventRequestSent;
        event OnTransactionEventResponseReceivedDelegate?                    OnTransactionEventResponseReceived;
        event OnTransactionEventResponseSentDelegate?                        OnTransactionEventResponseSent;

        #endregion

        #region Customer

        event OnNotifyCustomerInformationRequestReceivedDelegate             OnNotifyCustomerInformationRequestReceived;
        event OnNotifyCustomerInformationRequestFilterDelegate?              OnNotifyCustomerInformationRequestFilter;
        event OnNotifyCustomerInformationRequestFilteredDelegate?            OnNotifyCustomerInformationRequestFiltered;
        event OnNotifyCustomerInformationRequestSentDelegate?                OnNotifyCustomerInformationRequestSent;
        event OnNotifyCustomerInformationResponseReceivedDelegate?           OnNotifyCustomerInformationResponseReceived;
        event OnNotifyCustomerInformationResponseSentDelegate?               OnNotifyCustomerInformationResponseSent;

        event OnNotifyDisplayMessagesRequestReceivedDelegate                 OnNotifyDisplayMessagesRequestReceived;
        event OnNotifyDisplayMessagesRequestFilterDelegate?                  OnNotifyDisplayMessagesRequestFilter;
        event OnNotifyDisplayMessagesRequestFilteredDelegate?                OnNotifyDisplayMessagesRequestFiltered;
        event OnNotifyDisplayMessagesRequestSentDelegate?                    OnNotifyDisplayMessagesRequestSent;
        event OnNotifyDisplayMessagesResponseReceivedDelegate?               OnNotifyDisplayMessagesResponseReceived;
        event OnNotifyDisplayMessagesResponseSentDelegate?                   OnNotifyDisplayMessagesResponseSent;

        #endregion

        #region DeviceModel

        event OnLogStatusNotificationRequestReceivedDelegate                 OnLogStatusNotificationRequestReceived;
        event OnLogStatusNotificationRequestFilterDelegate?                  OnLogStatusNotificationRequestFilter;
        event OnLogStatusNotificationRequestFilteredDelegate?                OnLogStatusNotificationRequestFiltered;
        event OnLogStatusNotificationRequestSentDelegate?                    OnLogStatusNotificationRequestSent;
        event OnLogStatusNotificationResponseReceivedDelegate?               OnLogStatusNotificationResponseReceived;
        event OnLogStatusNotificationResponseSentDelegate?                   OnLogStatusNotificationResponseSent;

        event OnNotifyEventRequestReceivedDelegate                           OnNotifyEventRequestReceived;
        event OnNotifyEventRequestFilterDelegate?                            OnNotifyEventRequestFilter;
        event OnNotifyEventRequestFilteredDelegate?                          OnNotifyEventRequestFiltered;
        event OnNotifyEventRequestSentDelegate?                              OnNotifyEventRequestSent;
        event OnNotifyEventResponseReceivedDelegate?                         OnNotifyEventResponseReceived;
        event OnNotifyEventResponseSentDelegate?                             OnNotifyEventResponseSent;

        event OnNotifyMonitoringReportRequestReceivedDelegate                OnNotifyMonitoringReportRequestReceived;
        event OnNotifyMonitoringReportRequestFilterDelegate?                 OnNotifyMonitoringReportRequestFilter;
        event OnNotifyMonitoringReportRequestFilteredDelegate?               OnNotifyMonitoringReportRequestFiltered;
        event OnNotifyMonitoringReportRequestSentDelegate?                   OnNotifyMonitoringReportRequestSent;
        event OnNotifyMonitoringReportResponseReceivedDelegate?              OnNotifyMonitoringReportResponseReceived;
        event OnNotifyMonitoringReportResponseSentDelegate?                  OnNotifyMonitoringReportResponseSent;

        event OnNotifyReportRequestReceivedDelegate                          OnNotifyReportRequestReceived;
        event OnNotifyReportRequestFilterDelegate?                           OnNotifyReportRequestFilter;
        event OnNotifyReportRequestFilteredDelegate?                         OnNotifyReportRequestFiltered;
        event OnNotifyReportRequestSentDelegate?                             OnNotifyReportRequestSent;
        event OnNotifyReportResponseReceivedDelegate?                        OnNotifyReportResponseReceived;
        event OnNotifyReportResponseSentDelegate?                            OnNotifyReportResponseSent;

        event OnSecurityEventNotificationRequestReceivedDelegate             OnSecurityEventNotificationRequestReceived;
        event OnSecurityEventNotificationRequestFilterDelegate?              OnSecurityEventNotificationRequestFilter;
        event OnSecurityEventNotificationRequestFilteredDelegate?            OnSecurityEventNotificationRequestFiltered;
        event OnSecurityEventNotificationRequestSentDelegate?                OnSecurityEventNotificationRequestSent;
        event OnSecurityEventNotificationResponseReceivedDelegate?           OnSecurityEventNotificationResponseReceived;
        event OnSecurityEventNotificationResponseSentDelegate?               OnSecurityEventNotificationResponseSent;

        #endregion

        #region Firmware

        event OnBootNotificationRequestReceivedDelegate                      OnBootNotificationRequestReceived;
        event OnBootNotificationRequestFilterDelegate?                       OnBootNotificationRequestFilter;
        event OnBootNotificationRequestFilteredDelegate?                     OnBootNotificationRequestFiltered;
        event OnBootNotificationRequestSentDelegate?                         OnBootNotificationRequestSent;
        event OnBootNotificationResponseReceivedDelegate?                    OnBootNotificationResponseReceived;
        event OnBootNotificationResponseSentDelegate?                        OnBootNotificationResponseSent;

        event OnFirmwareStatusNotificationRequestReceivedDelegate            OnFirmwareStatusNotificationRequestReceived;
        event OnFirmwareStatusNotificationRequestFilterDelegate?             OnFirmwareStatusNotificationRequestFilter;
        event OnFirmwareStatusNotificationRequestFilteredDelegate?           OnFirmwareStatusNotificationRequestFiltered;
        event OnFirmwareStatusNotificationRequestSentDelegate?               OnFirmwareStatusNotificationRequestSent;
        event OnFirmwareStatusNotificationResponseReceivedDelegate?          OnFirmwareStatusNotificationResponseReceived;
        event OnFirmwareStatusNotificationResponseSentDelegate?              OnFirmwareStatusNotificationResponseSent;

        event OnHeartbeatRequestReceivedDelegate                             OnHeartbeatRequestReceived;
        event OnHeartbeatRequestFilterDelegate?                              OnHeartbeatRequestFilter;
        event OnHeartbeatRequestFilteredDelegate?                            OnHeartbeatRequestFiltered;
        event OnHeartbeatRequestSentDelegate?                                OnHeartbeatRequestSent;
        event OnHeartbeatResponseReceivedDelegate?                           OnHeartbeatResponseReceived;
        event OnHeartbeatResponseSentDelegate?                               OnHeartbeatResponseSent;

        event OnPublishFirmwareStatusNotificationRequestReceivedDelegate     OnPublishFirmwareStatusNotificationRequestReceived;
        event OnPublishFirmwareStatusNotificationRequestFilterDelegate?      OnPublishFirmwareStatusNotificationRequestFilter;
        event OnPublishFirmwareStatusNotificationRequestFilteredDelegate?    OnPublishFirmwareStatusNotificationRequestFiltered;
        event OnPublishFirmwareStatusNotificationRequestSentDelegate?        OnPublishFirmwareStatusNotificationRequestSent;
        event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?   OnPublishFirmwareStatusNotificationResponseReceived;
        event OnPublishFirmwareStatusNotificationResponseSentDelegate?       OnPublishFirmwareStatusNotificationResponseSent;

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


        Task ProcessJSONRequestMessage         (OCPP_JSONRequestMessage          JSONRequestMessage,          IWebSocketConnection WebSocketConnection);
        Task ProcessJSONResponseMessage        (OCPP_JSONResponseMessage         JSONResponseMessage,         IWebSocketConnection WebSocketConnection);
        Task ProcessJSONRequestErrorMessage    (OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,     IWebSocketConnection WebSocketConnection);
        Task ProcessJSONResponseErrorMessage   (OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage,    IWebSocketConnection WebSocketConnection);
        Task ProcessJSONSendMessage            (OCPP_JSONSendMessage             JSONSendMessage,             IWebSocketConnection WebSocketConnection);


        Task ProcessBinaryRequestMessage       (OCPP_BinaryRequestMessage        BinaryRequestMessage,        IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryResponseMessage      (OCPP_BinaryResponseMessage       BinaryResponseMessage,       IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryRequestErrorMessage  (OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,   IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryResponseErrorMessage (OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,  IWebSocketConnection WebSocketConnection);
        Task ProcessBinarySendMessage          (OCPP_BinarySendMessage           BinarySendMessage,           IWebSocketConnection WebSocketConnection);


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
