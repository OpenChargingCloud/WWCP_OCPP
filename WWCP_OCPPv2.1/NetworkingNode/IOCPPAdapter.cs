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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all OCPP adapters.
    /// </summary>
    public interface IOCPPAdapter
    {

        #region Properties

        /// <summary>
        /// The unique identification of the networking node hosting this OCPP adapter.
        /// </summary>
        NetworkingNode_Id             Id                             { get; }

        /// <summary>
        /// Incoming OCPP messages.
        /// </summary>
        IOCPPWebSocketAdapterIN       IN                             { get; }

        /// <summary>
        /// Outgoing OCPP messages.
        /// </summary>
        IOCPPWebSocketAdapterOUT      OUT                            { get; }

        /// <summary>
        /// Forwarded OCPP messages.
        /// </summary>
        IOCPPWebSocketAdapterFORWARD  FORWARD                        { get; }

        /// <summary>
        /// Disable all heartbeats.
        /// </summary>
        Boolean                       DisableSendHeartbeats          { get; set; }

        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        TimeSpan                      SendHeartbeatsEvery            { get; set; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        TimeSpan                      DefaultRequestTimeout          { get; }


        /// <summary>
        /// Return a new unique request identification.
        /// </summary>
        Request_Id                    NextRequestId                  { get; }


        #region SignaturePolicy/-ies

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        IEnumerable<SignaturePolicy>  SignaturePolicies              { get; }

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        SignaturePolicy               SignaturePolicy                { get; }

        #endregion

        #region ForwardingSignaturePolicy/-ies

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        IEnumerable<SignaturePolicy>  ForwardingSignaturePolicies    { get; }

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        SignaturePolicy               ForwardingSignaturePolicy      { get; }

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region Charging Station Request  Messages

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<            DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }

        #endregion

        #region Charging Station Response Messages

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<            DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }


        // Binary Data Streams Extensions
        CustomBinarySerializerDelegate <            GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<            SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<            DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<            ListDirectoryResponse>?                               CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        CustomJObjectSerializerDelegate<            AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<            UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<            DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<            AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<            UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<            DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }


        // E2E Charging Tariff Extensions
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDefaultChargingTariffResponse>?                    CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CS.RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region CSMS Request  Messages

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ResetRequest>?                                   CustomResetRequestSerializer                                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UpdateFirmwareRequest>?                          CustomUpdateFirmwareRequestSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PublishFirmwareRequest>?                         CustomPublishFirmwareRequestSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UnpublishFirmwareRequest>?                       CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetBaseReportRequest>?                           CustomGetBaseReportRequestSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetReportRequest>?                               CustomGetReportRequestSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetLogRequest>?                                  CustomGetLogRequestSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetVariablesRequest>?                            CustomSetVariablesRequestSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetVariablesRequest>?                            CustomGetVariablesRequestSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetMonitoringBaseRequest>?                       CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetMonitoringReportRequest>?                     CustomGetMonitoringReportRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetMonitoringLevelRequest>?                      CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetVariableMonitoringRequest>?                   CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearVariableMonitoringRequest>?                 CustomClearVariableMonitoringRequestSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetNetworkProfileRequest>?                       CustomSetNetworkProfileRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ChangeAvailabilityRequest>?                      CustomChangeAvailabilityRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.TriggerMessageRequest>?                          CustomTriggerMessageRequestSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<              DataTransferRequest>?                            CustomIncomingDataTransferRequestSerializer                  { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CertificateSignedRequest>?                       CustomCertificateSignedRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.InstallCertificateRequest>?                      CustomInstallCertificateRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest>?              CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.DeleteCertificateRequest>?                       CustomDeleteCertificateRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCRLRequest>?                               CustomNotifyCRLRequestSerializer                             { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetLocalListVersionRequest>?                     CustomGetLocalListVersionRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SendLocalListRequest>?                           CustomSendLocalListRequestSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearCacheRequest>?                              CustomClearCacheRequestSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReserveNowRequest>?                              CustomReserveNowRequestSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CancelReservationRequest>?                       CustomCancelReservationRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RequestStartTransactionRequest>?                 CustomRequestStartTransactionRequestSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RequestStopTransactionRequest>?                  CustomRequestStopTransactionRequestSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetTransactionStatusRequest>?                    CustomGetTransactionStatusRequestSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetChargingProfileRequest>?                      CustomSetChargingProfileRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetChargingProfilesRequest>?                     CustomGetChargingProfilesRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearChargingProfileRequest>?                    CustomClearChargingProfileRequestSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCompositeScheduleRequest>?                    CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UpdateDynamicScheduleRequest>?                   CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest>?             CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


        // Binary Data Streams Extensions
        CustomJObjectSerializerDelegate<              GetFileRequest>?                                 CustomGetFileRequestSerializer                               { get; set; }
        CustomBinarySerializerDelegate <              SendFileRequest>?                                CustomSendFileRequestSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<              DeleteFileRequest>?                              CustomDeleteFileRequestSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<              ListDirectoryRequest>?                           CustomListDirectoryRequestSerializer                         { get; set; }


        // E2E Security Extensions
        CustomJObjectSerializerDelegate<              AddSignaturePolicyRequest>?                      CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<              UpdateSignaturePolicyRequest>?                   CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<              DeleteSignaturePolicyRequest>?                   CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<              AddUserRoleRequest>?                             CustomAddUserRoleRequestSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<              UpdateUserRoleRequest>?                          CustomUpdateUserRoleRequestSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<              DeleteUserRoleRequest>?                          CustomDeleteUserRoleRequestSerializer                        { get; set; }


        // E2E Charging Tariffs Extensions
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetDefaultChargingTariffRequest>?                CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetDefaultChargingTariffRequest>?                CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest>?             CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<              DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifySettlementResponse>?                       CustomNotifySettlementResponseSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion



        // Binary Data Streams Extensions
        CustomBinarySerializerDelegate <              BinaryDataTransferRequest>?                      CustomBinaryDataTransferRequestSerializer                    { get; set; }
        CustomBinarySerializerDelegate <              BinaryDataTransferResponse>?                     CustomIncomingBinaryDataTransferResponseSerializer           { get; set; }
        CustomBinarySerializerDelegate <              BinaryDataTransferRequest>?                      CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }
        CustomBinarySerializerDelegate <              BinaryDataTransferResponse>?                     CustomBinaryDataTransferResponseSerializer                   { get; set; }


        // E2E Security Extensions
        CustomBinarySerializerDelegate <              SecureDataTransferRequest>?                      CustomSecureDataTransferRequestSerializer                    { get; set; }
        CustomBinarySerializerDelegate <              SecureDataTransferResponse>?                     CustomIncomingSecureDataTransferResponseSerializer           { get; set; }
        CustomBinarySerializerDelegate <              SecureDataTransferRequest>?                      CustomIncomingSecureDataTransferRequestSerializer            { get; set; }
        CustomBinarySerializerDelegate <              SecureDataTransferResponse>?                     CustomSecureDataTransferResponseSerializer                   { get; set; }


        CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?   CustomNotifyNetworkTopologyRequestSerializer           { get; set; }
        CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer          { get; set; }

        #region Data Structures

        CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
        CustomJObjectSerializerDelegate<SignaturePolicy>?                                     CustomSignaturePolicySerializer                              { get; set; }
        CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }



        CustomJObjectSerializerDelegate<ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                              { get; set; }

        CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                              { get; set; }
        CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                         { get; set; }
        CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                                  { get; set; }

        CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                                  { get; set; }
        CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }

        CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }


        // Binary Data Streams Extensions
        CustomBinarySerializerDelegate<Signature>?                                            CustomBinarySignatureSerializer                              { get; set; }


        // E2E Security Extensions



        // E2E Charging Tariff Extensions
        CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                                { get; set; }
        CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                               { get; set; }
        CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                      { get; set; }
        CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                           { get; set; }
        CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }


        // Overlay Networking Extensions
        CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer             { get; set; }

        #endregion

        #endregion

        #region Custom JSON parser delegates

        #region CS

        #region BinaryDataStreamsExtensions

        #endregion

        #region Certificates
        CustomJObjectParserDelegate<OCPPv2_1.CS.  Get15118EVCertificateRequest>?                CustomGet15118EVCertificateRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.Get15118EVCertificateResponse>?               CustomGet15118EVCertificateResponseParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCertificateStatusRequest>?                 CustomGetCertificateStatusRequestParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCertificateStatusResponse>?                CustomGetCertificateStatusResponseParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCRLRequest>?                               CustomGetCRLRequestParser                                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCRLResponse>?                              CustomGetCRLResponseParser                               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SignCertificateRequest>?                      CustomSignCertificateRequestParser                       { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SignCertificateResponse>?                     CustomSignCertificateResponseParser                      { get; set; }

        #endregion

        #region Charging
        CustomJObjectParserDelegate<OCPPv2_1.CS.  AuthorizeRequest>?                            CustomAuthorizeRequestParser                             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.AuthorizeResponse>?                           CustomAuthorizeResponseParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearedChargingLimitRequest>?                 CustomClearedChargingLimitRequestParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearedChargingLimitResponse>?                CustomClearedChargingLimitResponseParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  MeterValuesRequest>?                          CustomMeterValuesRequestParser                           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.MeterValuesResponse>?                         CustomMeterValuesResponseParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyChargingLimitRequest>?                  CustomNotifyChargingLimitRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyChargingLimitResponse>?                 CustomNotifyChargingLimitResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEVChargingNeedsRequest>?                CustomNotifyEVChargingNeedsRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>?               CustomNotifyEVChargingNeedsResponseParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEVChargingScheduleRequest>?             CustomNotifyEVChargingScheduleRequestParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>?            CustomNotifyEVChargingScheduleResponseParser             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyPriorityChargingRequest>?               CustomNotifyPriorityChargingRequestParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>?              CustomNotifyPriorityChargingResponseParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifySettlementRequest>?                     CustomNotifySettlementRequestParser                      { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifySettlementResponse>?                    CustomNotifySettlementResponseParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  PullDynamicScheduleUpdateRequest>?            CustomPullDynamicScheduleUpdateRequestParser             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?           CustomPullDynamicScheduleUpdateResponseParser            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ReportChargingProfilesRequest>?               CustomReportChargingProfilesRequestParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReportChargingProfilesResponse>?              CustomReportChargingProfilesResponseParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ReservationStatusUpdateRequest>?              CustomReservationStatusUpdateRequestParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>?             CustomReservationStatusUpdateResponseParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  StatusNotificationRequest>?                   CustomStatusNotificationRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.StatusNotificationResponse>?                  CustomStatusNotificationResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  TransactionEventRequest>?                     CustomTransactionEventRequestParser                      { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.TransactionEventResponse>?                    CustomTransactionEventResponseParser                     { get; set; }

        #endregion

        #region Customer
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyCustomerInformationRequest>?            CustomNotifyCustomerInformationRequestParser             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?           CustomNotifyCustomerInformationResponseParser            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyDisplayMessagesRequest>?                CustomNotifyDisplayMessagesRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?               CustomNotifyDisplayMessagesResponseParser                { get; set; }

        #endregion

        #region DeviceModel
        CustomJObjectParserDelegate<OCPPv2_1.CS.  LogStatusNotificationRequest>?                CustomLogStatusNotificationRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.LogStatusNotificationResponse>?               CustomLogStatusNotificationResponseParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEventRequest>?                          CustomNotifyEventRequestParser                           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEventResponse>?                         CustomNotifyEventResponseParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyMonitoringReportRequest>?               CustomNotifyMonitoringReportRequestParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>?              CustomNotifyMonitoringReportResponseParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyReportRequest>?                         CustomNotifyReportRequestParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyReportResponse>?                        CustomNotifyReportResponseParser                         { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SecurityEventNotificationRequest>?            CustomSecurityEventNotificationRequestParser             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SecurityEventNotificationResponse>?           CustomSecurityEventNotificationResponseParser            { get; set; }

        #endregion

        #region Firmware
        CustomJObjectParserDelegate<OCPPv2_1.CS.  BootNotificationRequest>?                     CustomBootNotificationRequestParser                      { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.BootNotificationResponse>?                    CustomBootNotificationResponseParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  FirmwareStatusNotificationRequest>?           CustomFirmwareStatusNotificationRequestParser            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>?          CustomFirmwareStatusNotificationResponseParser           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  HeartbeatRequest>?                            CustomHeartbeatRequestParser                             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.HeartbeatResponse>?                           CustomHeartbeatResponseParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  PublishFirmwareStatusNotificationRequest>?    CustomPublishFirmwareStatusNotificationRequestParser     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>?   CustomPublishFirmwareStatusNotificationResponseParser    { get; set; }

        #endregion

        #endregion

        #region CSMS

        #region BinaryDataStreamsExtensions

        CustomJObjectParserDelegate<DeleteFileRequest>?                                         CustomDeleteFileRequestParser                            { get; set; }
        CustomJObjectParserDelegate<DeleteFileResponse>?                                        CustomDeleteFileResponseParser                           { get; set; }
        CustomJObjectParserDelegate<GetFileRequest>?                                            CustomGetFileRequestParser                               { get; set; }
        CustomJObjectParserDelegate<GetFileResponse>?                                           CustomGetFileResponseParser                              { get; set; }
        CustomJObjectParserDelegate<ListDirectoryRequest>?                                      CustomListDirectoryRequestParser                         { get; set; }
        CustomJObjectParserDelegate<ListDirectoryResponse>?                                     CustomListDirectoryResponseParser                        { get; set; }
        CustomBinaryParserDelegate <SendFileRequest>?                                           CustomSendFileRequestParser                              { get; set; }
        CustomJObjectParserDelegate<SendFileResponse>?                                          CustomSendFileResponseParser                             { get; set; }

        #endregion

        #region Certificates

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.CertificateSignedRequest>?                    CustomCertificateSignedRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  CertificateSignedResponse>?                   CustomCertificateSignedResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.DeleteCertificateRequest>?                    CustomDeleteCertificateRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  DeleteCertificateResponse>?                   CustomDeleteCertificateResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest>?           CustomGetInstalledCertificateIdsRequestParser            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetInstalledCertificateIdsResponse>?          CustomGetInstalledCertificateIdsResponseParser           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.InstallCertificateRequest>?                   CustomInstallCertificateRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  InstallCertificateResponse>?                  CustomInstallCertificateResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyCRLRequest>?                            CustomNotifyCRLRequestParser                             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyCRLResponse>?                           CustomNotifyCRLResponseParser                            { get; set; }

        #endregion

        #region Charging
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.CancelReservationRequest>?                    CustomCancelReservationRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  CancelReservationResponse>?                   CustomCancelReservationResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearChargingProfileRequest>?                 CustomClearChargingProfileRequestParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearChargingProfileResponse>?                CustomClearChargingProfileResponseParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetChargingProfilesRequest>?                  CustomGetChargingProfilesRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetChargingProfilesResponse>?                 CustomGetChargingProfilesResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCompositeScheduleRequest>?                 CustomGetCompositeScheduleRequestParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCompositeScheduleResponse>?                CustomGetCompositeScheduleResponseParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetTransactionStatusRequest>?                 CustomGetTransactionStatusRequestParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetTransactionStatusResponse>?                CustomGetTransactionStatusResponseParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest>?          CustomNotifyAllowedEnergyTransferRequestParser           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyAllowedEnergyTransferResponse>?         CustomNotifyAllowedEnergyTransferResponseParser          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.RequestStartTransactionRequest>?              CustomRequestStartTransactionRequestParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  RequestStartTransactionResponse>?             CustomRequestStartTransactionResponseParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.RequestStopTransactionRequest>?               CustomRequestStopTransactionRequestParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  RequestStopTransactionResponse>?              CustomRequestStopTransactionResponseParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReserveNowRequest>?                           CustomReserveNowRequestParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ReserveNowResponse>?                          CustomReserveNowResponseParser                           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetChargingProfileRequest>?                   CustomSetChargingProfileRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetChargingProfileResponse>?                  CustomSetChargingProfileResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.UnlockConnectorRequest>?                      CustomUnlockConnectorRequestParser                       { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  UnlockConnectorResponse>?                     CustomUnlockConnectorResponseParser                      { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.UpdateDynamicScheduleRequest>?                CustomUpdateDynamicScheduleRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  UpdateDynamicScheduleResponse>?               CustomUpdateDynamicScheduleResponseParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.UsePriorityChargingRequest>?                  CustomUsePriorityChargingRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  UsePriorityChargingResponse>?                 CustomUsePriorityChargingResponseParser                  { get; set; }

        #endregion

        #region Customer

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearDisplayMessageRequest>?                  CustomClearDisplayMessageRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearDisplayMessageResponse>?                 CustomClearDisplayMessageResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.CostUpdatedRequest>?                          CustomCostUpdatedRequestParser                           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  CostUpdatedResponse>?                         CustomCostUpdatedResponseParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.CustomerInformationRequest>?                  CustomCustomerInformationRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  CustomerInformationResponse>?                 CustomCustomerInformationResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetDisplayMessagesRequest>?                   CustomGetDisplayMessagesRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetDisplayMessagesResponse>?                  CustomGetDisplayMessagesResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetDisplayMessageRequest>?                    CustomSetDisplayMessageRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetDisplayMessageResponse>?                   CustomSetDisplayMessageResponseParser                    { get; set; }

        #endregion

        #region DeviceModel
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ChangeAvailabilityRequest>?                   CustomChangeAvailabilityRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ChangeAvailabilityResponse>?                  CustomChangeAvailabilityResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearVariableMonitoringRequest>?              CustomClearVariableMonitoringRequestParser               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearVariableMonitoringResponse>?             CustomClearVariableMonitoringResponseParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetBaseReportRequest>?                        CustomGetBaseReportRequestParser                         { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetBaseReportResponse>?                       CustomGetBaseReportResponseParser                        { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetLogRequest>?                               CustomGetLogRequestParser                                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetLogResponse>?                              CustomGetLogResponseParser                               { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetMonitoringReportRequest>?                  CustomGetMonitoringReportRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetMonitoringReportResponse>?                 CustomGetMonitoringReportResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetReportRequest>?                            CustomGetReportRequestParser                             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetReportResponse>?                           CustomGetReportResponseParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetVariablesRequest>?                         CustomGetVariablesRequestParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetVariablesResponse>?                        CustomGetVariablesResponseParser                         { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetMonitoringBaseRequest>?                    CustomSetMonitoringBaseRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetMonitoringBaseResponse>?                   CustomSetMonitoringBaseResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetMonitoringLevelRequest>?                   CustomSetMonitoringLevelRequestParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetMonitoringLevelResponse>?                  CustomSetMonitoringLevelResponseParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetNetworkProfileRequest>?                    CustomSetNetworkProfileRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetNetworkProfileResponse>?                   CustomSetNetworkProfileResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetVariableMonitoringRequest>?                CustomSetVariableMonitoringRequestParser                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetVariableMonitoringResponse>?               CustomSetVariableMonitoringResponseParser                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetVariablesRequest>?                         CustomSetVariablesRequestParser                          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetVariablesResponse>?                        CustomSetVariablesResponseParser                         { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.TriggerMessageRequest>?                       CustomTriggerMessageRequestParser                        { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  TriggerMessageResponse>?                      CustomTriggerMessageResponseParser                       { get; set; }

        #endregion

        #region E2EChargingTariffsExtensions

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetDefaultChargingTariffRequest>?             CustomGetDefaultChargingTariffRequestParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetDefaultChargingTariffResponse>?            CustomGetDefaultChargingTariffResponseParser             { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest>?          CustomRemoveDefaultChargingTariffRequestParser           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  RemoveDefaultChargingTariffResponse>?         CustomRemoveDefaultChargingTariffResponseParser          { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetDefaultChargingTariffRequest>?             CustomSetDefaultChargingTariffRequestParser              { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SetDefaultChargingTariffResponse>?            CustomSetDefaultChargingTariffResponseParser             { get; set; }

        #endregion

        #region E2ESecurityExtensions

        CustomJObjectParserDelegate<AddSignaturePolicyRequest>?                                 CustomAddSignaturePolicyRequestParser                    { get; set; }
        CustomJObjectParserDelegate<AddSignaturePolicyResponse>?                                CustomAddSignaturePolicyResponseParser                   { get; set; }
        CustomJObjectParserDelegate<AddUserRoleRequest>?                                        CustomAddUserRoleRequestParser                           { get; set; }
        CustomJObjectParserDelegate<AddUserRoleResponse>?                                       CustomAddUserRoleResponseParser                          { get; set; }
        CustomJObjectParserDelegate<DeleteSignaturePolicyRequest>?                              CustomDeleteSignaturePolicyRequestParser                 { get; set; }
        CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?                             CustomDeleteSignaturePolicyResponseParser                { get; set; }
        CustomJObjectParserDelegate<DeleteUserRoleRequest>?                                     CustomDeleteUserRoleRequestParser                        { get; set; }
        CustomJObjectParserDelegate<DeleteUserRoleResponse>?                                    CustomDeleteUserRoleResponseParser                       { get; set; }
        CustomJObjectParserDelegate<UpdateSignaturePolicyRequest>?                              CustomUpdateSignaturePolicyRequestParser                 { get; set; }
        CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?                             CustomUpdateSignaturePolicyResponseParser                { get; set; }
        CustomJObjectParserDelegate<UpdateUserRoleRequest>?                                     CustomUpdateUserRoleRequestParser                        { get; set; }
        CustomJObjectParserDelegate<UpdateUserRoleResponse>?                                    CustomUpdateUserRoleResponseParser                       { get; set; }

        #endregion

        #region Firmware

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.PublishFirmwareRequest>?                      CustomPublishFirmwareRequestParser                       { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  PublishFirmwareResponse>?                     CustomPublishFirmwareResponseParser                      { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ResetRequest>?                                CustomResetRequestParser                                 { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ResetResponse>?                               CustomResetResponseParser                                { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.UnpublishFirmwareRequest>?                    CustomUnpublishFirmwareRequestParser                     { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  UnpublishFirmwareResponse>?                   CustomUnpublishFirmwareResponseParser                    { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.UpdateFirmwareRequest>?                       CustomUpdateFirmwareRequestParser                        { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  UpdateFirmwareResponse>?                      CustomUpdateFirmwareResponseParser                       { get; set; }

        #endregion

        #region Grid

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.AFRRSignalRequest>?                           CustomAFRRSignalRequestParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  AFRRSignalResponse>?                          CustomAFRRSignalResponseParser                           { get; set; }

        #endregion

        #region LocalList

        CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearCacheRequest>?                           CustomClearCacheRequestParser                            { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearCacheResponse>?                          CustomClearCacheResponseParser                           { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetLocalListVersionRequest>?                  CustomGetLocalListVersionRequestParser                   { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  GetLocalListVersionResponse>?                 CustomGetLocalListVersionResponseParser                  { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CSMS.SendLocalListRequest>?                        CustomSendLocalListRequestParser                         { get; set; }
        CustomJObjectParserDelegate<OCPPv2_1.CS.  SendLocalListResponse>?                       CustomSendLocalListResponseParser                        { get; set; }

        #endregion

        #endregion

        CustomBinaryParserDelegate<BinaryDataTransferRequest>?                                  CustomBinaryDataTransferRequestParser                    { get; set; }
        CustomBinaryParserDelegate<BinaryDataTransferResponse>?                                 CustomBinaryDataTransferResponseParser                   { get; set; }
        CustomJObjectParserDelegate<DataTransferRequest>?                                       CustomDataTransferRequestParser                          { get; set; }
        CustomJObjectParserDelegate<DataTransferResponse>?                                      CustomDataTransferResponseParser                         { get; set; }
        CustomBinaryParserDelegate<SecureDataTransferRequest>?                                  CustomSecureDataTransferRequestParser                    { get; set; }
        CustomBinaryParserDelegate<SecureDataTransferResponse>?                                 CustomSecureDataTransferResponseParser                   { get; set; }



        CustomJObjectParserDelegate<ChargingStation>?                                           CustomChargingStationParser                              { get; set; }
        CustomJObjectParserDelegate<Signature>?                                                 CustomSignatureParser                                    { get; set; }
        CustomJObjectParserDelegate<CustomData>?                                                CustomCustomDataParser                                   { get; set; }
        CustomJObjectParserDelegate<StatusInfo>?                                                CustomStatusInfoParser                                   { get; set; }

        #endregion



        Task<SendMessageResult> SendJSONRequest          (OCPP_JSONRequestMessage          JSONRequestMessage);
        Task<SendRequestState>  SendJSONRequestAndWait   (OCPP_JSONRequestMessage          JSONRequestMessage,   Action<SendMessageResult>? SendMessageResultDelegate = null);
        Task<SendMessageResult> SendJSONResponse         (OCPP_JSONResponseMessage         JSONResponseMessage);
        Task<SendMessageResult> SendJSONRequestError     (OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage);
        Task<SendMessageResult> SendJSONResponseError    (OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage);
        Task<SendMessageResult> SendJSONSendMessage      (OCPP_JSONSendMessage             JSONSendMessage);

        Task<SendMessageResult> SendBinaryRequest        (OCPP_BinaryRequestMessage        BinaryRequestMessage);
        Task<SendRequestState>  SendBinaryRequestAndWait (OCPP_BinaryRequestMessage        BinaryRequestMessage, Action<SendMessageResult>? SendMessageResultDelegate = null);
        Task<SendMessageResult> SendBinaryResponse       (OCPP_BinaryResponseMessage       BinaryResponseMessage);
        Task<SendMessageResult> SendBinaryRequestError   (OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage);
        Task<SendMessageResult> SendBinaryResponseError  (OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage);
        Task<SendMessageResult> SendBinarySendMessage    (OCPP_BinarySendMessage           BinarySendMessage);


        Boolean ReceiveJSONResponse        (OCPP_JSONResponseMessage         JSONResponseMessage);
        Boolean ReceiveJSONRequestError    (OCPP_JSONRequestErrorMessage     JSONErrorMessage);
        Boolean ReceiveJSONResponseError   (OCPP_JSONResponseErrorMessage    JSONErrorMessage);

        Boolean ReceiveBinaryResponse      (OCPP_BinaryResponseMessage       BinaryResponseMessage);
        Boolean ReceiveBinaryRequestError  (OCPP_BinaryRequestErrorMessage   BinaryErrorMessage);
        Boolean ReceiveBinaryResponseError (OCPP_BinaryResponseErrorMessage  BinaryErrorMessage);



        Boolean LookupNetworkingNode(NetworkingNode_Id DestinationId, out Reachability? Reachability);

        void AddStaticRouting   (NetworkingNode_Id     DestinationId,
                                 IOCPPWebSocketClient  WebSocketClient,
                                 Byte?                 Priority    = 0,
                                 DateTime?             Timestamp   = null,
                                 DateTime?             Timeout     = null);

        void AddStaticRouting   (NetworkingNode_Id     DestinationId,
                                 IOCPPWebSocketServer  WebSocketServer,
                                 Byte?                 Priority    = 0,
                                 DateTime?             Timestamp   = null,
                                 DateTime?             Timeout     = null);

        void AddStaticRouting   (NetworkingNode_Id     DestinationId,
                                 NetworkingNode_Id     NetworkingHubId,
                                 Byte?                 Priority    = 0,
                                 DateTime?             Timestamp   = null,
                                 DateTime?             Timeout     = null);

        void RemoveStaticRouting(NetworkingNode_Id     DestinationId,
                                 NetworkingNode_Id?    NetworkingHubId   = null,
                                 Byte?                 Priority          = 0);


    }

}
