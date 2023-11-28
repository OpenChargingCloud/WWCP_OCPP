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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all NetworkingNode clients.
    /// </summary>
    public interface INetworkingNodeClient : INetworkingNodeClientEvents
    {

        #region Custom JSON serializer delegates

        #region NetworkingNode Request Messages

        CustomJObjectSerializerDelegate<ResetRequest>?                                        CustomResetRequestSerializer                                 { get; set; }

        CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                               CustomUpdateFirmwareRequestSerializer                        { get; set; }

        CustomJObjectSerializerDelegate<PublishFirmwareRequest>?                              CustomPublishFirmwareRequestSerializer                       { get; set; }

        CustomJObjectSerializerDelegate<UnpublishFirmwareRequest>?                            CustomUnpublishFirmwareRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<GetBaseReportRequest>?                                CustomGetBaseReportRequestSerializer                         { get; set; }

        CustomJObjectSerializerDelegate<GetReportRequest>?                                    CustomGetReportRequestSerializer                             { get; set; }

        CustomJObjectSerializerDelegate<GetLogRequest>?                                       CustomGetLogRequestSerializer                                { get; set; }

        CustomJObjectSerializerDelegate<SetVariablesRequest>?                                 CustomSetVariablesRequestSerializer                          { get; set; }

        CustomJObjectSerializerDelegate<GetVariablesRequest>?                                 CustomGetVariablesRequestSerializer                          { get; set; }

        CustomJObjectSerializerDelegate<SetMonitoringBaseRequest>?                            CustomSetMonitoringBaseRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?                          CustomGetMonitoringReportRequestSerializer                   { get; set; }

        CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?                           CustomSetMonitoringLevelRequestSerializer                    { get; set; }

        CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?                        CustomSetVariableMonitoringRequestSerializer                 { get; set; }

        CustomJObjectSerializerDelegate<ClearVariableMonitoringRequest>?                      CustomClearVariableMonitoringRequestSerializer               { get; set; }

        CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?                            CustomSetNetworkProfileRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?                           CustomChangeAvailabilityRequestSerializer                    { get; set; }

        CustomJObjectSerializerDelegate<TriggerMessageRequest>?                               CustomTriggerMessageRequestSerializer                        { get; set; }

        CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.DataTransferRequest>?                   CustomDataTransferRequestSerializer                          { get; set; }


        CustomJObjectSerializerDelegate<CertificateSignedRequest>?                            CustomCertificateSignedRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<InstallCertificateRequest>?                           CustomInstallCertificateRequestSerializer                    { get; set; }

        CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?                   CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }

        CustomJObjectSerializerDelegate<DeleteCertificateRequest>?                            CustomDeleteCertificateRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<NotifyCRLRequest>?                                    CustomNotifyCRLRequestSerializer                             { get; set; }


        CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?                          CustomGetLocalListVersionRequestSerializer                   { get; set; }

        CustomJObjectSerializerDelegate<SendLocalListRequest>?                                CustomSendLocalListRequestSerializer                         { get; set; }

        CustomJObjectSerializerDelegate<ClearCacheRequest>?                                   CustomClearCacheRequestSerializer                            { get; set; }


        CustomJObjectSerializerDelegate<ReserveNowRequest>?                                   CustomReserveNowRequestSerializer                            { get; set; }

        CustomJObjectSerializerDelegate<CancelReservationRequest>?                            CustomCancelReservationRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?                      CustomRequestStartTransactionRequestSerializer               { get; set; }

        CustomJObjectSerializerDelegate<RequestStopTransactionRequest>?                       CustomRequestStopTransactionRequestSerializer                { get; set; }

        CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?                         CustomGetTransactionStatusRequestSerializer                  { get; set; }

        CustomJObjectSerializerDelegate<SetChargingProfileRequest>?                           CustomSetChargingProfileRequestSerializer                    { get; set; }

        CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?                          CustomGetChargingProfilesRequestSerializer                   { get; set; }

        CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?                         CustomClearChargingProfileRequestSerializer                  { get; set; }

        CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?                         CustomGetCompositeScheduleRequestSerializer                  { get; set; }

        CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?                        CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }

        CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?                  CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }

        CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?                          CustomUsePriorityChargingRequestSerializer                   { get; set; }

        CustomJObjectSerializerDelegate<UnlockConnectorRequest>?                              CustomUnlockConnectorRequestSerializer                       { get; set; }


        CustomJObjectSerializerDelegate<AFRRSignalRequest>?                                   CustomAFRRSignalRequestSerializer                            { get; set; }


        CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?                            CustomSetDisplayMessageRequestSerializer                     { get; set; }

        CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?                           CustomGetDisplayMessagesRequestSerializer                    { get; set; }

        CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?                          CustomClearDisplayMessageRequestSerializer                   { get; set; }

        CustomJObjectSerializerDelegate<CostUpdatedRequest>?                                  CustomCostUpdatedRequestSerializer                           { get; set; }

        CustomJObjectSerializerDelegate<CustomerInformationRequest>?                          CustomCustomerInformationRequestSerializer                   { get; set; }

        #endregion

        #region Data Structures

        CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
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

        CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }

        #endregion

        #endregion


        #region Reset                       (Request)

        /// <summary>
        /// Reset the given charging station.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region UpdateFirmware              (Request)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charging station.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request);

        #endregion

        #region PublishFirmware             (Request)

        /// <summary>
        /// Publish a firmware.
        /// </summary>
        /// <param name="Request">A publish firmware request.</param>
        Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request);

        #endregion

        #region UnpublishFirmware           (Request)

        /// <summary>
        /// Unpublish a firmware.
        /// </summary>
        /// <param name="Request">An unpublish firmware request.</param>
        Task<UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request);

        #endregion

        #region GetBaseReport               (Request)

        /// <summary>
        /// Get a base report.
        /// </summary>
        /// <param name="Request">A get base report request.</param>
        Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request);

        #endregion

        #region GetReport                   (Request)

        /// <summary>
        /// Get a report.
        /// </summary>
        /// <param name="Request">A get report request.</param>
        Task<GetReportResponse> GetReport(GetReportRequest Request);

        #endregion

        #region GetLog                      (Request)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        Task<GetLogResponse> GetLog(GetLogRequest Request);

        #endregion

        #region SetVariables                (Request)

        /// <summary>
        /// Set variables.
        /// </summary>
        /// <param name="Request">A set variables request.</param>
        Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request);

        #endregion

        #region GetVariables                (Request)

        /// <summary>
        /// Get all variables.
        /// </summary>
        /// <param name="Request">A get variables request.</param>
        Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request);

        #endregion

        #region SetMonitoringBase           (Request)

        /// <summary>
        /// Set the monitoring base.
        /// </summary>
        /// <param name="Request">A set monitoring base request.</param>
        Task<SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request);

        #endregion

        #region GetMonitoringReport         (Request)

        /// <summary>
        /// Get a monitoring report.
        /// </summary>
        /// <param name="Request">A get monitoring report request.</param>
        Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request);

        #endregion

        #region SetMonitoringLevel          (Request)

        /// <summary>
        /// Set the monitoring level.
        /// </summary>
        /// <param name="Request">A set monitoring level request.</param>
        Task<SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request);

        #endregion

        #region SetVariableMonitoring       (Request)

        /// <summary>
        /// Set a variable monitoring.
        /// </summary>
        /// <param name="Request">A set variable monitoring request.</param>
        Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request);

        #endregion

        #region ClearVariableMonitoring     (Request)

        /// <summary>
        /// Remove the given variable monitoring.
        /// </summary>
        /// <param name="Request">A clear variable monitoring request.</param>
        Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request);

        #endregion

        #region SetNetworkProfile           (Request)

        /// <summary>
        /// Set the network profile.
        /// </summary>
        /// <param name="Request">A set network profile request.</param>
        Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request);

        #endregion

        #region ChangeAvailability          (Request)

        /// <summary>
        /// Change the availability of the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request);

        #endregion

        #region TriggerMessage              (Request)

        /// <summary>
        /// Create a trigger for the given message at the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request);

        #endregion

        #region TransferData                (Request)

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        Task<OCPPv2_1.CS.DataTransferResponse> TransferData(OCPPv2_1.CSMS.DataTransferRequest Request);

        #endregion


        #region SendSignedCertificate       (Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        Task<CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request);

        #endregion

        #region InstallCertificate          (Request)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request);

        #endregion

        #region GetInstalledCertificateIds  (Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request);

        #endregion

        #region DeleteCertificate           (Request)

        /// <summary>
        /// Remove the given certificate from the charging station.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request);

        #endregion

        #region NotifyCRLAvailability       (Request)

        /// <summary>
        /// Notify the charging station about the status of a certificate revocation list.
        /// </summary>
        /// <param name="Request">A NotifyCRL request.</param>
        Task<NotifyCRLResponse> NotifyCRLAvailability(NotifyCRLRequest Request);

        #endregion


        #region GetLocalListVersion         (Request)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request);

        #endregion

        #region SendLocalList               (Request)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request);

        #endregion

        #region ClearCache                  (Request)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request);

        #endregion


        #region ReserveNow                  (Request)

        /// <summary>
        /// Create a charging reservation at the given charging station.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request);

        #endregion

        #region CancelReservation           (Request)

        /// <summary>
        /// Cancel the given charging reservation.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request);

        #endregion

        #region StartCharging               (Request)

        /// <summary>
        /// Start a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request start transaction request.</param>
        Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request);

        #endregion

        #region StopCharging                (Request)

        /// <summary>
        /// Stop a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request stop transaction request.</param>
        Task<RequestStopTransactionResponse> StopCharging(RequestStopTransactionRequest Request);

        #endregion

        #region GetTransactionStatus        (Request)

        /// <summary>
        /// Get the status of a charging process (transaction).
        /// </summary>
        /// <param name="Request">A get transaction status request.</param>
        Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request);

        #endregion

        #region SetChargingProfile          (Request)

        /// <summary>
        /// Set the charging profile of the given EVSE at the given charging station.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request);

        #endregion

        #region GetChargingProfiles         (Request)

        /// <summary>
        /// Get all charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A get charging profiles request.</param>
        Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request);

        #endregion

        #region ClearChargingProfile        (Request)

        /// <summary>
        /// Remove matching charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request);

        #endregion

        #region GetCompositeSchedule        (Request)

        /// <summary>
        /// Return the charging schedule at the given charging station and EVSE.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request);

        #endregion

        #region UpdateDynamicSchedule       (Request)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="Request">An UpdateDynamicSchedule request.</param>
        Task<UpdateDynamicScheduleResponse> UpdateDynamicSchedule(UpdateDynamicScheduleRequest Request);

        #endregion

        #region NotifyAllowedEnergyTransfer (Request)

        /// <summary>
        /// Update the list of authorized energy services.
        /// </summary>
        /// <param name="Request">A notify allowed energy transfer request.</param>
        Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request);

        #endregion

        #region UsePriorityCharging         (Request)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="Request">An UsePriorityCharging request.</param>
        Task<UsePriorityChargingResponse> UsePriorityCharging(UsePriorityChargingRequest Request);

        #endregion

        #region UnlockConnector             (Request)

        /// <summary>
        /// Unlock the given EVSE/connector at the given charging station.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request);

        #endregion


        #region SendAFRRSignal              (Request)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<AFRRSignalResponse> SendAFRRSignal(AFRRSignalRequest Request);

        #endregion


        #region SetDisplayMessage           (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A set display message request.</param>
        Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request);

        #endregion

        #region GetDisplayMessages          (Request)

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="Request">A get display messages request.</param>
        Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request);

        #endregion

        #region ClearDisplayMessage         (Request)

        /// <summary>
        /// Remove the given display message.
        /// </summary>
        /// <param name="Request">A clear display message request.</param>
        Task<ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request);

        #endregion

        #region SendCostUpdated             (Request)

        /// <summary>
        /// Send updated cost(s).
        /// </summary>
        /// <param name="Request">A cost updated request.</param>
        Task<CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request);

        #endregion

        #region RequestCustomerInformation  (Request)

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="Request">A customer information request.</param>
        Task<CustomerInformationResponse> RequestCustomerInformation(CustomerInformationRequest Request);

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData          (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A binary data transfer request.</param>
        Task<OCPPv2_1.CS.BinaryDataTransferResponse> TransferBinaryData(OCPPv2_1.CSMS.BinaryDataTransferRequest Request);

        #endregion

        #region GetFile                     (Request)

        /// <summary>
        /// Request to download the specified file.
        /// </summary>
        /// <param name="Request">A GetFile request.</param>
        Task<OCPPv2_1.CS.GetFileResponse> GetFile(GetFileRequest Request);

        #endregion

        #region SendFile                    (Request)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        Task<OCPPv2_1.CS.SendFileResponse> SendFile(SendFileRequest Request);

        #endregion

        #region DeleteFile                  (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        Task<OCPPv2_1.CS.DeleteFileResponse> DeleteFile(DeleteFileRequest Request);

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy          (Request)

        /// <summary>
        /// Add a signature policy.
        /// </summary>
        /// <param name="Request">An AddSignaturePolicy request.</param>
        Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request);

        #endregion

        #region UpdateSignaturePolicy       (Request)

        /// <summary>
        /// Update a signature policy.
        /// </summary>
        /// <param name="Request">An UpdateSignaturePolicy request.</param>
        Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request);

        #endregion

        #region DeleteSignaturePolicy       (Request)

        /// <summary>
        /// Delete a signature policy.
        /// </summary>
        /// <param name="Request">A DeleteSignaturePolicy request.</param>
        Task<DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request);

        #endregion

        #region AddUserRole                 (Request)

        /// <summary>
        /// Add a user role.
        /// </summary>
        /// <param name="Request">An AddUserRole request.</param>
        Task<AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request);

        #endregion

        #region UpdateUserRole              (Request)

        /// <summary>
        /// Update a user role.
        /// </summary>
        /// <param name="Request">An UpdateUserRole request.</param>
        Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request);

        #endregion

        #region DeleteUserRole              (Request)

        /// <summary>
        /// Delete a user role.
        /// </summary>
        /// <param name="Request">An DeleteUserRole request.</param>
        Task<DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request);

        #endregion


        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff    (Request)

        /// <summary>
        /// Set a default charging tariff for the charging station,
        /// or for a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="Request">A SetDefaultChargingTariff request.</param>
        Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request);

        #endregion

        #region GetDefaultChargingTariff    (Request)

        /// <summary>
        /// Get the default charging tariff(s) for the charging station and its EVSEs.
        /// </summary>
        /// <param name="Request">A GetDefaultChargingTariff request.</param>
        Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request);

        #endregion

        #region RemoveDefaultChargingTariff (Request)

        /// <summary>
        /// Remove the default charging tariff of the charging station,
        /// or of a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="Request">A RemoveDefaultChargingTariff request.</param>
        Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request);

        #endregion


    }

}
