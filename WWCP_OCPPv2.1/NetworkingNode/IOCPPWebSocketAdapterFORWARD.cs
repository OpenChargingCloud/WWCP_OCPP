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

        #region Common

        #region BinaryDataStreamsExtensions

        event OnBinaryDataTransferRequestReceivedDelegate                   OnBinaryDataTransferRequestReceived;
        event OnBinaryDataTransferRequestFilterDelegate?                    OnBinaryDataTransferRequestFilter;
        event OnBinaryDataTransferRequestFilteredDelegate?                  OnBinaryDataTransferRequestFiltered;
        event OnBinaryDataTransferRequestSentDelegate?                      OnBinaryDataTransferRequestSent;
        event OnBinaryDataTransferResponseReceivedDelegate?                 OnBinaryDataTransferResponseReceived;
        event OnBinaryDataTransferResponseSentDelegate?                     OnBinaryDataTransferResponseSent;

        event OnDeleteFileRequestReceivedDelegate                           OnDeleteFileRequestReceived;
        event OnDeleteFileRequestFilterDelegate?                            OnDeleteFileRequestFilter;
        event OnDeleteFileRequestFilteredDelegate?                          OnDeleteFileRequestFiltered;
        event OnDeleteFileRequestSentDelegate?                              OnDeleteFileRequestSent;
        event OnDeleteFileResponseReceivedDelegate?                         OnDeleteFileResponseReceived;
        event OnDeleteFileResponseSentDelegate?                             OnDeleteFileResponseSent;

        event OnGetFileRequestReceivedDelegate                              OnGetFileRequestReceived;
        event OnGetFileRequestFilterDelegate?                               OnGetFileRequestFilter;
        event OnGetFileRequestFilteredDelegate?                             OnGetFileRequestFiltered;
        event OnGetFileRequestSentDelegate?                                 OnGetFileRequestSent;
        event OnGetFileResponseReceivedDelegate?                            OnGetFileResponseReceived;
        event OnGetFileResponseSentDelegate?                                OnGetFileResponseSent;

        event OnListDirectoryRequestReceivedDelegate                        OnListDirectoryRequestReceived;
        event OnListDirectoryRequestFilterDelegate?                         OnListDirectoryRequestFilter;
        event OnListDirectoryRequestFilteredDelegate?                       OnListDirectoryRequestFiltered;
        event OnListDirectoryRequestSentDelegate?                           OnListDirectoryRequestSent;
        event OnListDirectoryResponseReceivedDelegate?                      OnListDirectoryResponseReceived;
        event OnListDirectoryResponseSentDelegate?                          OnListDirectoryResponseSent;

        event OnSendFileRequestReceivedDelegate                             OnSendFileRequestReceived;
        event OnSendFileRequestFilterDelegate?                              OnSendFileRequestFilter;
        event OnSendFileRequestFilteredDelegate?                            OnSendFileRequestFiltered;
        event OnSendFileRequestSentDelegate?                                OnSendFileRequestSent;
        event OnSendFileResponseReceivedDelegate?                           OnSendFileResponseReceived;
        event OnSendFileResponseSentDelegate?                               OnSendFileResponseSent;

        #endregion

        #region E2ESecurityExtensions

        event OnAddSignaturePolicyRequestReceivedDelegate                   OnAddSignaturePolicyRequestReceived;
        event OnAddSignaturePolicyRequestFilterDelegate?                    OnAddSignaturePolicyRequestFilter;
        event OnAddSignaturePolicyRequestFilteredDelegate?                  OnAddSignaturePolicyRequestFiltered;
        event OnAddSignaturePolicyRequestSentDelegate?                      OnAddSignaturePolicyRequestSent;
        event OnAddSignaturePolicyResponseReceivedDelegate?                 OnAddSignaturePolicyResponseReceived;
        event OnAddSignaturePolicyResponseSentDelegate?                     OnAddSignaturePolicyResponseSent;

        event OnAddUserRoleRequestReceivedDelegate                          OnAddUserRoleRequestReceived;
        event OnAddUserRoleRequestFilterDelegate?                           OnAddUserRoleRequestFilter;
        event OnAddUserRoleRequestFilteredDelegate?                         OnAddUserRoleRequestFiltered;
        event OnAddUserRoleRequestSentDelegate?                             OnAddUserRoleRequestSent;
        event OnAddUserRoleResponseReceivedDelegate?                        OnAddUserRoleResponseReceived;
        event OnAddUserRoleResponseSentDelegate?                            OnAddUserRoleResponseSent;

        event OnDeleteSignaturePolicyRequestReceivedDelegate                OnDeleteSignaturePolicyRequestReceived;
        event OnDeleteSignaturePolicyRequestFilterDelegate?                 OnDeleteSignaturePolicyRequestFilter;
        event OnDeleteSignaturePolicyRequestFilteredDelegate?               OnDeleteSignaturePolicyRequestFiltered;
        event OnDeleteSignaturePolicyRequestSentDelegate?                   OnDeleteSignaturePolicyRequestSent;
        event OnDeleteSignaturePolicyResponseReceivedDelegate?              OnDeleteSignaturePolicyResponseReceived;
        event OnDeleteSignaturePolicyResponseSentDelegate?                  OnDeleteSignaturePolicyResponseSent;

        event OnDeleteUserRoleRequestReceivedDelegate                       OnDeleteUserRoleRequestReceived;
        event OnDeleteUserRoleRequestFilterDelegate?                        OnDeleteUserRoleRequestFilter;
        event OnDeleteUserRoleRequestFilteredDelegate?                      OnDeleteUserRoleRequestFiltered;
        event OnDeleteUserRoleRequestSentDelegate?                          OnDeleteUserRoleRequestSent;
        event OnDeleteUserRoleResponseReceivedDelegate?                     OnDeleteUserRoleResponseReceived;
        event OnDeleteUserRoleResponseSentDelegate?                         OnDeleteUserRoleResponseSent;

        event OnSecureDataTransferRequestReceivedDelegate                   OnSecureDataTransferRequestReceived;
        event OnSecureDataTransferRequestFilterDelegate?                    OnSecureDataTransferRequestFilter;
        event OnSecureDataTransferRequestFilteredDelegate?                  OnSecureDataTransferRequestFiltered;
        event OnSecureDataTransferRequestSentDelegate?                      OnSecureDataTransferRequestSent;
        event OnSecureDataTransferResponseReceivedDelegate?                 OnSecureDataTransferResponseReceived;
        event OnSecureDataTransferResponseSentDelegate?                     OnSecureDataTransferResponseSent;

        event OnUpdateSignaturePolicyRequestReceivedDelegate                OnUpdateSignaturePolicyRequestReceived;
        event OnUpdateSignaturePolicyRequestFilterDelegate?                 OnUpdateSignaturePolicyRequestFilter;
        event OnUpdateSignaturePolicyRequestFilteredDelegate?               OnUpdateSignaturePolicyRequestFiltered;
        event OnUpdateSignaturePolicyRequestSentDelegate?                   OnUpdateSignaturePolicyRequestSent;
        event OnUpdateSignaturePolicyResponseReceivedDelegate?              OnUpdateSignaturePolicyResponseReceived;
        event OnUpdateSignaturePolicyResponseSentDelegate?                  OnUpdateSignaturePolicyResponseSent;

        event OnUpdateUserRoleRequestReceivedDelegate                       OnUpdateUserRoleRequestReceived;
        event OnUpdateUserRoleRequestFilterDelegate?                        OnUpdateUserRoleRequestFilter;
        event OnUpdateUserRoleRequestFilteredDelegate?                      OnUpdateUserRoleRequestFiltered;
        event OnUpdateUserRoleRequestSentDelegate?                          OnUpdateUserRoleRequestSent;
        event OnUpdateUserRoleResponseReceivedDelegate?                     OnUpdateUserRoleResponseReceived;
        event OnUpdateUserRoleResponseSentDelegate?                         OnUpdateUserRoleResponseSent;

        #endregion

        event OnDataTransferRequestReceivedDelegate                         OnDataTransferRequestReceived;
        event OnDataTransferRequestFilterDelegate?                          OnDataTransferRequestFilter;
        event OnDataTransferRequestFilteredDelegate?                        OnDataTransferRequestFiltered;
        event OnDataTransferRequestSentDelegate?                            OnDataTransferRequestSent;
        event OnDataTransferResponseReceivedDelegate?                       OnDataTransferResponseReceived;
        event OnDataTransferResponseSentDelegate?                           OnDataTransferResponseSent;

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

        event OnCertificateSignedRequestReceivedDelegate                     OnCertificateSignedRequestReceived;
        event OnCertificateSignedRequestFilterDelegate?                      OnCertificateSignedRequestFilter;
        event OnCertificateSignedRequestFilteredDelegate?                    OnCertificateSignedRequestFiltered;
        event OnCertificateSignedRequestSentDelegate?                        OnCertificateSignedRequestSent;
        event OnCertificateSignedResponseReceivedDelegate?                   OnCertificateSignedResponseReceived;
        event OnCertificateSignedResponseSentDelegate?                       OnCertificateSignedResponseSent;

        event OnDeleteCertificateRequestReceivedDelegate                     OnDeleteCertificateRequestReceived;
        event OnDeleteCertificateRequestFilterDelegate?                      OnDeleteCertificateRequestFilter;
        event OnDeleteCertificateRequestFilteredDelegate?                    OnDeleteCertificateRequestFiltered;
        event OnDeleteCertificateRequestSentDelegate?                        OnDeleteCertificateRequestSent;
        event OnDeleteCertificateResponseReceivedDelegate?                   OnDeleteCertificateResponseReceived;
        event OnDeleteCertificateResponseSentDelegate?                       OnDeleteCertificateResponseSent;

        event OnGetInstalledCertificateIdsRequestReceivedDelegate            OnGetInstalledCertificateIdsRequestReceived;
        event OnGetInstalledCertificateIdsRequestFilterDelegate?             OnGetInstalledCertificateIdsRequestFilter;
        event OnGetInstalledCertificateIdsRequestFilteredDelegate?           OnGetInstalledCertificateIdsRequestFiltered;
        event OnGetInstalledCertificateIdsRequestSentDelegate?               OnGetInstalledCertificateIdsRequestSent;
        event OnGetInstalledCertificateIdsResponseReceivedDelegate?          OnGetInstalledCertificateIdsResponseReceived;
        event OnGetInstalledCertificateIdsResponseSentDelegate?              OnGetInstalledCertificateIdsResponseSent;

        event OnInstallCertificateRequestReceivedDelegate                    OnInstallCertificateRequestReceived;
        event OnInstallCertificateRequestFilterDelegate?                     OnInstallCertificateRequestFilter;
        event OnInstallCertificateRequestFilteredDelegate?                   OnInstallCertificateRequestFiltered;
        event OnInstallCertificateRequestSentDelegate?                       OnInstallCertificateRequestSent;
        event OnInstallCertificateResponseReceivedDelegate?                  OnInstallCertificateResponseReceived;
        event OnInstallCertificateResponseSentDelegate?                      OnInstallCertificateResponseSent;

        event OnNotifyCRLRequestReceivedDelegate                             OnNotifyCRLRequestReceived;
        event OnNotifyCRLRequestFilterDelegate?                              OnNotifyCRLRequestFilter;
        event OnNotifyCRLRequestFilteredDelegate?                            OnNotifyCRLRequestFiltered;
        event OnNotifyCRLRequestSentDelegate?                                OnNotifyCRLRequestSent;
        event OnNotifyCRLResponseReceivedDelegate?                           OnNotifyCRLResponseReceived;
        event OnNotifyCRLResponseSentDelegate?                               OnNotifyCRLResponseSent;

        #endregion

        #region Charging

        event OnCancelReservationRequestReceivedDelegate                     OnCancelReservationRequestReceived;
        event OnCancelReservationRequestFilterDelegate?                      OnCancelReservationRequestFilter;
        event OnCancelReservationRequestFilteredDelegate?                    OnCancelReservationRequestFiltered;
        event OnCancelReservationRequestSentDelegate?                        OnCancelReservationRequestSent;
        event OnCancelReservationResponseReceivedDelegate?                   OnCancelReservationResponseReceived;
        event OnCancelReservationResponseSentDelegate?                       OnCancelReservationResponseSent;

        event OnClearChargingProfileRequestReceivedDelegate                  OnClearChargingProfileRequestReceived;
        event OnClearChargingProfileRequestFilterDelegate?                   OnClearChargingProfileRequestFilter;
        event OnClearChargingProfileRequestFilteredDelegate?                 OnClearChargingProfileRequestFiltered;
        event OnClearChargingProfileRequestSentDelegate?                     OnClearChargingProfileRequestSent;
        event OnClearChargingProfileResponseReceivedDelegate?                OnClearChargingProfileResponseReceived;
        event OnClearChargingProfileResponseSentDelegate?                    OnClearChargingProfileResponseSent;

        event OnGetChargingProfilesRequestReceivedDelegate                   OnGetChargingProfilesRequestReceived;
        event OnGetChargingProfilesRequestFilterDelegate?                    OnGetChargingProfilesRequestFilter;
        event OnGetChargingProfilesRequestFilteredDelegate?                  OnGetChargingProfilesRequestFiltered;
        event OnGetChargingProfilesRequestSentDelegate?                      OnGetChargingProfilesRequestSent;
        event OnGetChargingProfilesResponseReceivedDelegate?                 OnGetChargingProfilesResponseReceived;
        event OnGetChargingProfilesResponseSentDelegate?                     OnGetChargingProfilesResponseSent;

        event OnGetCompositeScheduleRequestReceivedDelegate                  OnGetCompositeScheduleRequestReceived;
        event OnGetCompositeScheduleRequestFilterDelegate?                   OnGetCompositeScheduleRequestFilter;
        event OnGetCompositeScheduleRequestFilteredDelegate?                 OnGetCompositeScheduleRequestFiltered;
        event OnGetCompositeScheduleRequestSentDelegate?                     OnGetCompositeScheduleRequestSent;
        event OnGetCompositeScheduleResponseReceivedDelegate?                OnGetCompositeScheduleResponseReceived;
        event OnGetCompositeScheduleResponseSentDelegate?                    OnGetCompositeScheduleResponseSent;

        event OnGetTransactionStatusRequestReceivedDelegate                  OnGetTransactionStatusRequestReceived;
        event OnGetTransactionStatusRequestFilterDelegate?                   OnGetTransactionStatusRequestFilter;
        event OnGetTransactionStatusRequestFilteredDelegate?                 OnGetTransactionStatusRequestFiltered;
        event OnGetTransactionStatusRequestSentDelegate?                     OnGetTransactionStatusRequestSent;
        event OnGetTransactionStatusResponseReceivedDelegate?                OnGetTransactionStatusResponseReceived;
        event OnGetTransactionStatusResponseSentDelegate?                    OnGetTransactionStatusResponseSent;

        event OnNotifyAllowedEnergyTransferRequestReceivedDelegate           OnNotifyAllowedEnergyTransferRequestReceived;
        event OnNotifyAllowedEnergyTransferRequestFilterDelegate?            OnNotifyAllowedEnergyTransferRequestFilter;
        event OnNotifyAllowedEnergyTransferRequestFilteredDelegate?          OnNotifyAllowedEnergyTransferRequestFiltered;
        event OnNotifyAllowedEnergyTransferRequestSentDelegate?              OnNotifyAllowedEnergyTransferRequestSent;
        event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?         OnNotifyAllowedEnergyTransferResponseReceived;
        event OnNotifyAllowedEnergyTransferResponseSentDelegate?             OnNotifyAllowedEnergyTransferResponseSent;

        event OnRequestStartTransactionRequestReceivedDelegate               OnRequestStartTransactionRequestReceived;
        event OnRequestStartTransactionRequestFilterDelegate?                OnRequestStartTransactionRequestFilter;
        event OnRequestStartTransactionRequestFilteredDelegate?              OnRequestStartTransactionRequestFiltered;
        event OnRequestStartTransactionRequestSentDelegate?                  OnRequestStartTransactionRequestSent;
        event OnRequestStartTransactionResponseReceivedDelegate?             OnRequestStartTransactionResponseReceived;
        event OnRequestStartTransactionResponseSentDelegate?                 OnRequestStartTransactionResponseSent;

        event OnRequestStopTransactionRequestReceivedDelegate                OnRequestStopTransactionRequestReceived;
        event OnRequestStopTransactionRequestFilterDelegate?                 OnRequestStopTransactionRequestFilter;
        event OnRequestStopTransactionRequestFilteredDelegate?               OnRequestStopTransactionRequestFiltered;
        event OnRequestStopTransactionRequestSentDelegate?                   OnRequestStopTransactionRequestSent;
        event OnRequestStopTransactionResponseReceivedDelegate?              OnRequestStopTransactionResponseReceived;
        event OnRequestStopTransactionResponseSentDelegate?                  OnRequestStopTransactionResponseSent;

        event OnReserveNowRequestReceivedDelegate                            OnReserveNowRequestReceived;
        event OnReserveNowRequestFilterDelegate?                             OnReserveNowRequestFilter;
        event OnReserveNowRequestFilteredDelegate?                           OnReserveNowRequestFiltered;
        event OnReserveNowRequestSentDelegate?                               OnReserveNowRequestSent;
        event OnReserveNowResponseReceivedDelegate?                          OnReserveNowResponseReceived;
        event OnReserveNowResponseSentDelegate?                              OnReserveNowResponseSent;

        event OnSetChargingProfileRequestReceivedDelegate                    OnSetChargingProfileRequestReceived;
        event OnSetChargingProfileRequestFilterDelegate?                     OnSetChargingProfileRequestFilter;
        event OnSetChargingProfileRequestFilteredDelegate?                   OnSetChargingProfileRequestFiltered;
        event OnSetChargingProfileRequestSentDelegate?                       OnSetChargingProfileRequestSent;
        event OnSetChargingProfileResponseReceivedDelegate?                  OnSetChargingProfileResponseReceived;
        event OnSetChargingProfileResponseSentDelegate?                      OnSetChargingProfileResponseSent;

        event OnUnlockConnectorRequestReceivedDelegate                       OnUnlockConnectorRequestReceived;
        event OnUnlockConnectorRequestFilterDelegate?                        OnUnlockConnectorRequestFilter;
        event OnUnlockConnectorRequestFilteredDelegate?                      OnUnlockConnectorRequestFiltered;
        event OnUnlockConnectorRequestSentDelegate?                          OnUnlockConnectorRequestSent;
        event OnUnlockConnectorResponseReceivedDelegate?                     OnUnlockConnectorResponseReceived;
        event OnUnlockConnectorResponseSentDelegate?                         OnUnlockConnectorResponseSent;

        event OnUpdateDynamicScheduleRequestReceivedDelegate                 OnUpdateDynamicScheduleRequestReceived;
        event OnUpdateDynamicScheduleRequestFilterDelegate?                  OnUpdateDynamicScheduleRequestFilter;
        event OnUpdateDynamicScheduleRequestFilteredDelegate?                OnUpdateDynamicScheduleRequestFiltered;
        event OnUpdateDynamicScheduleRequestSentDelegate?                    OnUpdateDynamicScheduleRequestSent;
        event OnUpdateDynamicScheduleResponseReceivedDelegate?               OnUpdateDynamicScheduleResponseReceived;
        event OnUpdateDynamicScheduleResponseSentDelegate?                   OnUpdateDynamicScheduleResponseSent;

        event OnUsePriorityChargingRequestReceivedDelegate                   OnUsePriorityChargingRequestReceived;
        event OnUsePriorityChargingRequestFilterDelegate?                    OnUsePriorityChargingRequestFilter;
        event OnUsePriorityChargingRequestFilteredDelegate?                  OnUsePriorityChargingRequestFiltered;
        event OnUsePriorityChargingRequestSentDelegate?                      OnUsePriorityChargingRequestSent;
        event OnUsePriorityChargingResponseReceivedDelegate?                 OnUsePriorityChargingResponseReceived;
        event OnUsePriorityChargingResponseSentDelegate?                     OnUsePriorityChargingResponseSent;

        #endregion

        #region Customer

        event OnClearDisplayMessageRequestReceivedDelegate                   OnClearDisplayMessageRequestReceived;
        event OnClearDisplayMessageRequestFilterDelegate?                    OnClearDisplayMessageRequestFilter;
        event OnClearDisplayMessageRequestFilteredDelegate?                  OnClearDisplayMessageRequestFiltered;
        event OnClearDisplayMessageRequestSentDelegate?                      OnClearDisplayMessageRequestSent;
        event OnClearDisplayMessageResponseReceivedDelegate?                 OnClearDisplayMessageResponseReceived;
        event OnClearDisplayMessageResponseSentDelegate?                     OnClearDisplayMessageResponseSent;

        event OnCostUpdatedRequestReceivedDelegate                           OnCostUpdatedRequestReceived;
        event OnCostUpdatedRequestFilterDelegate?                            OnCostUpdatedRequestFilter;
        event OnCostUpdatedRequestFilteredDelegate?                          OnCostUpdatedRequestFiltered;
        event OnCostUpdatedRequestSentDelegate?                              OnCostUpdatedRequestSent;
        event OnCostUpdatedResponseReceivedDelegate?                         OnCostUpdatedResponseReceived;
        event OnCostUpdatedResponseSentDelegate?                             OnCostUpdatedResponseSent;

        event OnCustomerInformationRequestReceivedDelegate                   OnCustomerInformationRequestReceived;
        event OnCustomerInformationRequestFilterDelegate?                    OnCustomerInformationRequestFilter;
        event OnCustomerInformationRequestFilteredDelegate?                  OnCustomerInformationRequestFiltered;
        event OnCustomerInformationRequestSentDelegate?                      OnCustomerInformationRequestSent;
        event OnCustomerInformationResponseReceivedDelegate?                 OnCustomerInformationResponseReceived;
        event OnCustomerInformationResponseSentDelegate?                     OnCustomerInformationResponseSent;

        event OnGetDisplayMessagesRequestReceivedDelegate                    OnGetDisplayMessagesRequestReceived;
        event OnGetDisplayMessagesRequestFilterDelegate?                     OnGetDisplayMessagesRequestFilter;
        event OnGetDisplayMessagesRequestFilteredDelegate?                   OnGetDisplayMessagesRequestFiltered;
        event OnGetDisplayMessagesRequestSentDelegate?                       OnGetDisplayMessagesRequestSent;
        event OnGetDisplayMessagesResponseReceivedDelegate?                  OnGetDisplayMessagesResponseReceived;
        event OnGetDisplayMessagesResponseSentDelegate?                      OnGetDisplayMessagesResponseSent;

        event OnSetDisplayMessageRequestReceivedDelegate                     OnSetDisplayMessageRequestReceived;
        event OnSetDisplayMessageRequestFilterDelegate?                      OnSetDisplayMessageRequestFilter;
        event OnSetDisplayMessageRequestFilteredDelegate?                    OnSetDisplayMessageRequestFiltered;
        event OnSetDisplayMessageRequestSentDelegate?                        OnSetDisplayMessageRequestSent;
        event OnSetDisplayMessageResponseReceivedDelegate?                   OnSetDisplayMessageResponseReceived;
        event OnSetDisplayMessageResponseSentDelegate?                       OnSetDisplayMessageResponseSent;

        #endregion

        #region DeviceModel

        event OnChangeAvailabilityRequestReceivedDelegate                    OnChangeAvailabilityRequestReceived;
        event OnChangeAvailabilityRequestFilterDelegate?                     OnChangeAvailabilityRequestFilter;
        event OnChangeAvailabilityRequestFilteredDelegate?                   OnChangeAvailabilityRequestFiltered;
        event OnChangeAvailabilityRequestSentDelegate?                       OnChangeAvailabilityRequestSent;
        event OnChangeAvailabilityResponseReceivedDelegate?                  OnChangeAvailabilityResponseReceived;
        event OnChangeAvailabilityResponseSentDelegate?                      OnChangeAvailabilityResponseSent;

        event OnClearVariableMonitoringRequestReceivedDelegate               OnClearVariableMonitoringRequestReceived;
        event OnClearVariableMonitoringRequestFilterDelegate?                OnClearVariableMonitoringRequestFilter;
        event OnClearVariableMonitoringRequestFilteredDelegate?              OnClearVariableMonitoringRequestFiltered;
        event OnClearVariableMonitoringRequestSentDelegate?                  OnClearVariableMonitoringRequestSent;
        event OnClearVariableMonitoringResponseReceivedDelegate?             OnClearVariableMonitoringResponseReceived;
        event OnClearVariableMonitoringResponseSentDelegate?                 OnClearVariableMonitoringResponseSent;

        event OnGetBaseReportRequestReceivedDelegate                         OnGetBaseReportRequestReceived;
        event OnGetBaseReportRequestFilterDelegate?                          OnGetBaseReportRequestFilter;
        event OnGetBaseReportRequestFilteredDelegate?                        OnGetBaseReportRequestFiltered;
        event OnGetBaseReportRequestSentDelegate?                            OnGetBaseReportRequestSent;
        event OnGetBaseReportResponseReceivedDelegate?                       OnGetBaseReportResponseReceived;
        event OnGetBaseReportResponseSentDelegate?                           OnGetBaseReportResponseSent;

        event OnGetLogRequestReceivedDelegate                                OnGetLogRequestReceived;
        event OnGetLogRequestFilterDelegate?                                 OnGetLogRequestFilter;
        event OnGetLogRequestFilteredDelegate?                               OnGetLogRequestFiltered;
        event OnGetLogRequestSentDelegate?                                   OnGetLogRequestSent;
        event OnGetLogResponseReceivedDelegate?                              OnGetLogResponseReceived;
        event OnGetLogResponseSentDelegate?                                  OnGetLogResponseSent;

        event OnGetMonitoringReportRequestReceivedDelegate                   OnGetMonitoringReportRequestReceived;
        event OnGetMonitoringReportRequestFilterDelegate?                    OnGetMonitoringReportRequestFilter;
        event OnGetMonitoringReportRequestFilteredDelegate?                  OnGetMonitoringReportRequestFiltered;
        event OnGetMonitoringReportRequestSentDelegate?                      OnGetMonitoringReportRequestSent;
        event OnGetMonitoringReportResponseReceivedDelegate?                 OnGetMonitoringReportResponseReceived;
        event OnGetMonitoringReportResponseSentDelegate?                     OnGetMonitoringReportResponseSent;

        event OnGetReportRequestReceivedDelegate                             OnGetReportRequestReceived;
        event OnGetReportRequestFilterDelegate?                              OnGetReportRequestFilter;
        event OnGetReportRequestFilteredDelegate?                            OnGetReportRequestFiltered;
        event OnGetReportRequestSentDelegate?                                OnGetReportRequestSent;
        event OnGetReportResponseReceivedDelegate?                           OnGetReportResponseReceived;
        event OnGetReportResponseSentDelegate?                               OnGetReportResponseSent;

        event OnGetVariablesRequestReceivedDelegate                          OnGetVariablesRequestReceived;
        event OnGetVariablesRequestFilterDelegate?                           OnGetVariablesRequestFilter;
        event OnGetVariablesRequestFilteredDelegate?                         OnGetVariablesRequestFiltered;
        event OnGetVariablesRequestSentDelegate?                             OnGetVariablesRequestSent;
        event OnGetVariablesResponseReceivedDelegate?                        OnGetVariablesResponseReceived;
        event OnGetVariablesResponseSentDelegate?                            OnGetVariablesResponseSent;

        event OnSetMonitoringBaseRequestReceivedDelegate                     OnSetMonitoringBaseRequestReceived;
        event OnSetMonitoringBaseRequestFilterDelegate?                      OnSetMonitoringBaseRequestFilter;
        event OnSetMonitoringBaseRequestFilteredDelegate?                    OnSetMonitoringBaseRequestFiltered;
        event OnSetMonitoringBaseRequestSentDelegate?                        OnSetMonitoringBaseRequestSent;
        event OnSetMonitoringBaseResponseReceivedDelegate?                   OnSetMonitoringBaseResponseReceived;
        event OnSetMonitoringBaseResponseSentDelegate?                       OnSetMonitoringBaseResponseSent;

        event OnSetMonitoringLevelRequestReceivedDelegate                    OnSetMonitoringLevelRequestReceived;
        event OnSetMonitoringLevelRequestFilterDelegate?                     OnSetMonitoringLevelRequestFilter;
        event OnSetMonitoringLevelRequestFilteredDelegate?                   OnSetMonitoringLevelRequestFiltered;
        event OnSetMonitoringLevelRequestSentDelegate?                       OnSetMonitoringLevelRequestSent;
        event OnSetMonitoringLevelResponseReceivedDelegate?                  OnSetMonitoringLevelResponseReceived;
        event OnSetMonitoringLevelResponseSentDelegate?                      OnSetMonitoringLevelResponseSent;

        event OnSetNetworkProfileRequestReceivedDelegate                     OnSetNetworkProfileRequestReceived;
        event OnSetNetworkProfileRequestFilterDelegate?                      OnSetNetworkProfileRequestFilter;
        event OnSetNetworkProfileRequestFilteredDelegate?                    OnSetNetworkProfileRequestFiltered;
        event OnSetNetworkProfileRequestSentDelegate?                        OnSetNetworkProfileRequestSent;
        event OnSetNetworkProfileResponseReceivedDelegate?                   OnSetNetworkProfileResponseReceived;
        event OnSetNetworkProfileResponseSentDelegate?                       OnSetNetworkProfileResponseSent;

        event OnSetVariableMonitoringRequestReceivedDelegate                 OnSetVariableMonitoringRequestReceived;
        event OnSetVariableMonitoringRequestFilterDelegate?                  OnSetVariableMonitoringRequestFilter;
        event OnSetVariableMonitoringRequestFilteredDelegate?                OnSetVariableMonitoringRequestFiltered;
        event OnSetVariableMonitoringRequestSentDelegate?                    OnSetVariableMonitoringRequestSent;
        event OnSetVariableMonitoringResponseReceivedDelegate?               OnSetVariableMonitoringResponseReceived;
        event OnSetVariableMonitoringResponseSentDelegate?                   OnSetVariableMonitoringResponseSent;

        event OnSetVariablesRequestReceivedDelegate                          OnSetVariablesRequestReceived;
        event OnSetVariablesRequestFilterDelegate?                           OnSetVariablesRequestFilter;
        event OnSetVariablesRequestFilteredDelegate?                         OnSetVariablesRequestFiltered;
        event OnSetVariablesRequestSentDelegate?                             OnSetVariablesRequestSent;
        event OnSetVariablesResponseReceivedDelegate?                        OnSetVariablesResponseReceived;
        event OnSetVariablesResponseSentDelegate?                            OnSetVariablesResponseSent;

        event OnTriggerMessageRequestReceivedDelegate                        OnTriggerMessageRequestReceived;
        event OnTriggerMessageRequestFilterDelegate?                         OnTriggerMessageRequestFilter;
        event OnTriggerMessageRequestFilteredDelegate?                       OnTriggerMessageRequestFiltered;
        event OnTriggerMessageRequestSentDelegate?                           OnTriggerMessageRequestSent;
        event OnTriggerMessageResponseReceivedDelegate?                      OnTriggerMessageResponseReceived;
        event OnTriggerMessageResponseSentDelegate?                          OnTriggerMessageResponseSent;

        #endregion

        #region E2EChargingTariffsExtensions

        event OnGetDefaultChargingTariffRequestReceivedDelegate              OnGetDefaultChargingTariffRequestReceived;
        event OnGetDefaultChargingTariffRequestFilterDelegate?               OnGetDefaultChargingTariffRequestFilter;
        event OnGetDefaultChargingTariffRequestFilteredDelegate?             OnGetDefaultChargingTariffRequestFiltered;
        event OnGetDefaultChargingTariffRequestSentDelegate?                 OnGetDefaultChargingTariffRequestSent;
        event OnGetDefaultChargingTariffResponseReceivedDelegate?            OnGetDefaultChargingTariffResponseReceived;
        event OnGetDefaultChargingTariffResponseSentDelegate?                OnGetDefaultChargingTariffResponseSent;

        event OnRemoveDefaultChargingTariffRequestReceivedDelegate           OnRemoveDefaultChargingTariffRequestReceived;
        event OnRemoveDefaultChargingTariffRequestFilterDelegate?            OnRemoveDefaultChargingTariffRequestFilter;
        event OnRemoveDefaultChargingTariffRequestFilteredDelegate?          OnRemoveDefaultChargingTariffRequestFiltered;
        event OnRemoveDefaultChargingTariffRequestSentDelegate?              OnRemoveDefaultChargingTariffRequestSent;
        event OnRemoveDefaultChargingTariffResponseReceivedDelegate?         OnRemoveDefaultChargingTariffResponseReceived;
        event OnRemoveDefaultChargingTariffResponseSentDelegate?             OnRemoveDefaultChargingTariffResponseSent;

        event OnSetDefaultChargingTariffRequestReceivedDelegate              OnSetDefaultChargingTariffRequestReceived;
        event OnSetDefaultChargingTariffRequestFilterDelegate?               OnSetDefaultChargingTariffRequestFilter;
        event OnSetDefaultChargingTariffRequestFilteredDelegate?             OnSetDefaultChargingTariffRequestFiltered;
        event OnSetDefaultChargingTariffRequestSentDelegate?                 OnSetDefaultChargingTariffRequestSent;
        event OnSetDefaultChargingTariffResponseReceivedDelegate?            OnSetDefaultChargingTariffResponseReceived;
        event OnSetDefaultChargingTariffResponseSentDelegate?                OnSetDefaultChargingTariffResponseSent;

        #endregion

        #region Firmware

        event OnPublishFirmwareRequestReceivedDelegate                       OnPublishFirmwareRequestReceived;
        event OnPublishFirmwareRequestFilterDelegate?                        OnPublishFirmwareRequestFilter;
        event OnPublishFirmwareRequestFilteredDelegate?                      OnPublishFirmwareRequestFiltered;
        event OnPublishFirmwareRequestSentDelegate?                          OnPublishFirmwareRequestSent;
        event OnPublishFirmwareResponseReceivedDelegate?                     OnPublishFirmwareResponseReceived;
        event OnPublishFirmwareResponseSentDelegate?                         OnPublishFirmwareResponseSent;

        event OnResetRequestReceivedDelegate                                 OnResetRequestReceived;
        event OnResetRequestFilterDelegate?                                  OnResetRequestFilter;
        event OnResetRequestFilteredDelegate?                                OnResetRequestFiltered;
        event OnResetRequestSentDelegate?                                    OnResetRequestSent;
        event OnResetResponseReceivedDelegate?                               OnResetResponseReceived;
        event OnResetResponseSentDelegate?                                   OnResetResponseSent;

        event OnUnpublishFirmwareRequestReceivedDelegate                     OnUnpublishFirmwareRequestReceived;
        event OnUnpublishFirmwareRequestFilterDelegate?                      OnUnpublishFirmwareRequestFilter;
        event OnUnpublishFirmwareRequestFilteredDelegate?                    OnUnpublishFirmwareRequestFiltered;
        event OnUnpublishFirmwareRequestSentDelegate?                        OnUnpublishFirmwareRequestSent;
        event OnUnpublishFirmwareResponseReceivedDelegate?                   OnUnpublishFirmwareResponseReceived;
        event OnUnpublishFirmwareResponseSentDelegate?                       OnUnpublishFirmwareResponseSent;

        event OnUpdateFirmwareRequestReceivedDelegate                        OnUpdateFirmwareRequestReceived;
        event OnUpdateFirmwareRequestFilterDelegate?                         OnUpdateFirmwareRequestFilter;
        event OnUpdateFirmwareRequestFilteredDelegate?                       OnUpdateFirmwareRequestFiltered;
        event OnUpdateFirmwareRequestSentDelegate?                           OnUpdateFirmwareRequestSent;
        event OnUpdateFirmwareResponseReceivedDelegate?                      OnUpdateFirmwareResponseReceived;
        event OnUpdateFirmwareResponseSentDelegate?                          OnUpdateFirmwareResponseSent;

        #endregion

        #region Grid

        event OnAFRRSignalRequestReceivedDelegate                           OnAFRRSignalRequestReceived;
        event OnAFRRSignalRequestFilterDelegate?                            OnAFRRSignalRequestFilter;
        event OnAFRRSignalRequestFilteredDelegate?                          OnAFRRSignalRequestFiltered;
        event OnAFRRSignalRequestSentDelegate?                              OnAFRRSignalRequestSent;
        event OnAFRRSignalResponseReceivedDelegate?                         OnAFRRSignalResponseReceived;
        event OnAFRRSignalResponseSentDelegate?                             OnAFRRSignalResponseSent;

        #endregion

        #region LocalList

        event OnClearCacheRequestReceivedDelegate                            OnClearCacheRequestReceived;
        event OnClearCacheRequestFilterDelegate?                             OnClearCacheRequestFilter;
        event OnClearCacheRequestFilteredDelegate?                           OnClearCacheRequestFiltered;
        event OnClearCacheRequestSentDelegate?                               OnClearCacheRequestSent;
        event OnClearCacheResponseReceivedDelegate?                          OnClearCacheResponseReceived;
        event OnClearCacheResponseSentDelegate?                              OnClearCacheResponseSent;

        event OnGetLocalListVersionRequestReceivedDelegate                   OnGetLocalListVersionRequestReceived;
        event OnGetLocalListVersionRequestFilterDelegate?                    OnGetLocalListVersionRequestFilter;
        event OnGetLocalListVersionRequestFilteredDelegate?                  OnGetLocalListVersionRequestFiltered;
        event OnGetLocalListVersionRequestSentDelegate?                      OnGetLocalListVersionRequestSent;
        event OnGetLocalListVersionResponseReceivedDelegate?                 OnGetLocalListVersionResponseReceived;
        event OnGetLocalListVersionResponseSentDelegate?                     OnGetLocalListVersionResponseSent;

        event OnSendLocalListRequestReceivedDelegate                         OnSendLocalListRequestReceived;
        event OnSendLocalListRequestFilterDelegate?                          OnSendLocalListRequestFilter;
        event OnSendLocalListRequestFilteredDelegate?                        OnSendLocalListRequestFiltered;
        event OnSendLocalListRequestSentDelegate?                            OnSendLocalListRequestSent;
        event OnSendLocalListResponseReceivedDelegate?                       OnSendLocalListResponseReceived;
        event OnSendLocalListResponseSentDelegate?                           OnSendLocalListResponseSent;

        #endregion

        #endregion

        #endregion


        #region JSON Message Processing

        Task ProcessJSONRequestMessage         (OCPP_JSONRequestMessage          JSONRequestMessage,          IWebSocketConnection WebSocketConnection);
        Task ProcessJSONResponseMessage        (OCPP_JSONResponseMessage         JSONResponseMessage,         IWebSocketConnection WebSocketConnection);
        Task ProcessJSONRequestErrorMessage    (OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,     IWebSocketConnection WebSocketConnection);
        Task ProcessJSONResponseErrorMessage   (OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage,    IWebSocketConnection WebSocketConnection);
        Task ProcessJSONSendMessage            (OCPP_JSONSendMessage             JSONSendMessage,             IWebSocketConnection WebSocketConnection);

        #endregion

        #region Binary Message Processing

        Task ProcessBinaryRequestMessage       (OCPP_BinaryRequestMessage        BinaryRequestMessage,        IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryResponseMessage      (OCPP_BinaryResponseMessage       BinaryResponseMessage,       IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryRequestErrorMessage  (OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,   IWebSocketConnection WebSocketConnection);
        Task ProcessBinaryResponseErrorMessage (OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,  IWebSocketConnection WebSocketConnection);
        Task ProcessBinarySendMessage          (OCPP_BinarySendMessage           BinarySendMessage,           IWebSocketConnection WebSocketConnection);

        #endregion

        NetworkingNode_Id? GetForwardedNodeId(Request_Id RequestId);


        #region Commmon

        #region BinaryDataStreamsExtensions
        Task<ForwardingDecision> Forward_BinaryDataTransfer                (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteFile                        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetFile                           (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ListDirectory                     (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SendFile                          (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region E2ESecurityExtensions

        Task<ForwardingDecision> Forward_AddSignaturePolicy                (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_AddUserRole                       (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SecureDataTransfer                (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        Task<ForwardingDecision> Forward_DataTransfer                      (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

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
