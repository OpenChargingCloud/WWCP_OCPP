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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS clients.
    /// </summary>
    public interface ICSMSClient : ICSMSClientEvents
    {

        #region Custom JSON serializer delegates

        #region CSMS Messages

        public CustomJObjectSerializerDelegate<ResetRequest>?                         CustomResetRequestSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                CustomUpdateFirmwareRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<PublishFirmwareRequest>?               CustomPublishFirmwareRequestSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<UnpublishFirmwareRequest>?             CustomUnpublishFirmwareRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<GetBaseReportRequest>?                 CustomGetBaseReportRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<GetReportRequest>?                     CustomGetReportRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<GetLogRequest>?                        CustomGetLogRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<SetVariablesRequest>?                  CustomSetVariablesRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<GetVariablesRequest>?                  CustomGetVariablesRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<SetMonitoringBaseRequest>?             CustomSetMonitoringBaseRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?           CustomGetMonitoringReportRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?            CustomSetMonitoringLevelRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?         CustomSetVariableMonitoringRequestSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<ClearVariableMonitoringRequest>?       CustomClearVariableMonitoringRequestSerializer         { get; set; }

        public CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?             CustomSetNetworkProfileRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?            CustomChangeAvailabilityRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                CustomTriggerMessageRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?                  CustomDataTransferRequestSerializer                    { get; set; }


        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?             CustomCertificateSignedRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?            CustomInstallCertificateRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?    CustomGetInstalledCertificateIdsRequestSerializer      { get; set; }

        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?             CustomDeleteCertificateRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<NotifyCRLRequest>?                     CustomNotifyCRLRequestSerializer                       { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?           CustomGetLocalListVersionRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                 CustomSendLocalListRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                    CustomClearCacheRequestSerializer                      { get; set; }


        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                    CustomReserveNowRequestSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>?             CustomCancelReservationRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?       CustomRequestStartTransactionRequestSerializer         { get; set; }

        public CustomJObjectSerializerDelegate<RequestStopTransactionRequest>?        CustomRequestStopTransactionRequestSerializer          { get; set; }

        public CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?          CustomGetTransactionStatusRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?            CustomSetChargingProfileRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?           CustomGetChargingProfilesRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?          CustomClearChargingProfileRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?          CustomGetCompositeScheduleRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?         CustomUpdateDynamicScheduleRequestSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?   CustomNotifyAllowedEnergyTransferRequestSerializer     { get; set; }

        public CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?           CustomUsePriorityChargingRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?               CustomUnlockConnectorRequestSerializer                 { get; set; }


        public CustomJObjectSerializerDelegate<AFRRSignalRequest>?                    CustomAFRRSignalRequestSerializer                      { get; set; }


        public CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?             CustomSetDisplayMessageRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?            CustomGetDisplayMessagesRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?           CustomClearDisplayMessageRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<CostUpdatedRequest>?                   CustomCostUpdatedRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<CustomerInformationRequest>?           CustomCustomerInformationRequestSerializer             { get; set; }

        #endregion

        #region Data Structures

        CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                       { get; set; }
        CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                { get; set; }
        CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                             { get; set; }
        CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                         { get; set; }
        CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                    { get; set; }
        CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                { get; set; }
        CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer         { get; set; }
        CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer              { get; set; }
        CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                { get; set; }
        CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer           { get; set; }
        CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                 { get; set; }
        CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer               { get; set; }
        CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                      { get; set; }
        CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                 { get; set; }
        CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer             { get; set; }
        CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                             { get; set; }
        
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer            { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                   { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                        { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                          { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                 { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                     { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                { get; set; }
        
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer               { get; set; }
        CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer          { get; set; }

        CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer         { get; set; }
        CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer             { get; set; }
        CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                      { get; set; }

        #endregion

        #endregion


        #region Reset                       (Request)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region UpdateFirmware              (Request)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
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
        Task<CS.DataTransferResponse> TransferData(DataTransferRequest Request);

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


    }

}
