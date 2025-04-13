/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP Adapter.
    /// </summary>
    public class OCPPAdapter
    {

        #region Data

        private readonly        ConcurrentDictionary<String, List<ComponentConfig>>  componentConfigs                = [];
        private readonly        ConcurrentDictionary<Request_Id, SendRequestState>   requests                        = [];
        private readonly        HashSet<SignaturePolicy>                             signaturePolicies               = [];
        private                 Int64                                                internalRequestId               = 800000;

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public static readonly  TimeSpan                                             DefaultSendHeartbeatsEvery      = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The default request timeout default.
        /// </summary>
        public static readonly  TimeSpan                                             DefaultRequestTimeoutDefault    = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the networking node hosting this OCPP adapter.
        /// </summary>
        public INetworkingNode              NetworkingNode           { get; }

        /// <summary>
        /// Incoming OCPP messages.
        /// </summary>
        public OCPPWebSocketAdapterIN       IN                       { get; }

        /// <summary>
        /// Outgoing OCPP messages.
        /// </summary>
        public OCPPWebSocketAdapterOUT      OUT                      { get; }

        /// <summary>
        /// Forwarded OCPP messages.
        /// </summary>
        public OCPPWebSocketAdapterFORWARD  FORWARD                  { get; }

        public RegistrationStatus           DefaultRegistrationStatus { get; set; } = RegistrationStatus.Rejected;


        /// <summary>
        /// Disable the sending of heartbeats.
        /// </summary>
        public Boolean                      DisableSendHeartbeats    { get; set; }

        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                     SendHeartbeatsEvery      { get; set; } = DefaultSendHeartbeatsEvery;

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                     DefaultRequestTimeout    { get; }      = DefaultRequestTimeoutDefault;


        #region NextRequestId

        /// <summary>
        /// Return a new unique request identification.
        /// </summary>
        public Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion

        #region SignaturePolicy/-ies

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               SignaturePolicy
            => signaturePolicies.First();

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region Charging Station Request  Messages

        public CustomJObjectSerializerDelegate<CS.BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CS.PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<CS.HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<   DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<CS.SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<CS.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }

        #endregion

        #region Charging Station Response Messages

        public CustomJObjectSerializerDelegate<CS.ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<   DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<CS.CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CS.DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CS.GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyWebPaymentStartedResponse>?                     CustomNotifyWebPaymentStartedResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CS.AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<CS.ChangeTransactionTariffResponse>?                     CustomChangeTransactionTariffResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearTariffsResponse>?                                CustomClearTariffsResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetDefaultTariffResponse>?                            CustomSetDefaultTariffResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetTariffsResponse>?                                  CustomGetTariffsResponseSerializer                           { get; set; }


        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<CS.SetDefaultE2EChargingTariffResponse>?                 CustomSetDefaultE2EChargingTariffResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region CSMS Request  Messages
        public CustomJObjectSerializerDelegate<CSMS.ResetRequest>?                                   CustomResetRequestSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UpdateFirmwareRequest>?                          CustomUpdateFirmwareRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.PublishFirmwareRequest>?                         CustomPublishFirmwareRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UnpublishFirmwareRequest>?                       CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetBaseReportRequest>?                           CustomGetBaseReportRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetReportRequest>?                               CustomGetReportRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetLogRequest>?                                  CustomGetLogRequestSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetVariablesRequest>?                            CustomSetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetVariablesRequest>?                            CustomGetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetMonitoringBaseRequest>?                       CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetMonitoringReportRequest>?                     CustomGetMonitoringReportRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetMonitoringLevelRequest>?                      CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetVariableMonitoringRequest>?                   CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearVariableMonitoringRequest>?                 CustomClearVariableMonitoringRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetNetworkProfileRequest>?                       CustomSetNetworkProfileRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ChangeAvailabilityRequest>?                      CustomChangeAvailabilityRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.TriggerMessageRequest>?                          CustomTriggerMessageRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<     DataTransferRequest>?                            CustomIncomingDataTransferRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.CertificateSignedRequest>?                       CustomCertificateSignedRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.InstallCertificateRequest>?                      CustomInstallCertificateRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetInstalledCertificateIdsRequest>?              CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.DeleteCertificateRequest>?                       CustomDeleteCertificateRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyCRLRequest>?                               CustomNotifyCRLRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.GetLocalListVersionRequest>?                     CustomGetLocalListVersionRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SendLocalListRequest>?                           CustomSendLocalListRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearCacheRequest>?                              CustomClearCacheRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.ReserveNowRequest>?                              CustomReserveNowRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CancelReservationRequest>?                       CustomCancelReservationRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.RequestStartTransactionRequest>?                 CustomRequestStartTransactionRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.RequestStopTransactionRequest>?                  CustomRequestStopTransactionRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetTransactionStatusRequest>?                    CustomGetTransactionStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetChargingProfileRequest>?                      CustomSetChargingProfileRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetChargingProfilesRequest>?                     CustomGetChargingProfilesRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearChargingProfileRequest>?                    CustomClearChargingProfileRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetCompositeScheduleRequest>?                    CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UpdateDynamicScheduleRequest>?                   CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyAllowedEnergyTransferRequest>?             CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyWebPaymentStartedRequest>?                 CustomNotifyWebPaymentStartedRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.ChangeTransactionTariffRequest>?                 CustomChangeTransactionTariffRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearTariffsRequest>?                            CustomClearTariffsRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SetDefaultTariffRequest>?                        CustomSetDefaultTariffRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetTariffsRequest>?                              CustomGetTariffsRequestSerializer                            { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<CSMS.SetDefaultTariffRequest>?                        CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetDefaultChargingTariffRequest>?                CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.RemoveDefaultChargingTariffRequest>?             CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages

        public CustomJObjectSerializerDelegate<CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<     DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifySettlementResponse>?                       CustomNotifySettlementResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion


        public CustomJObjectSerializerDelegate<MessageTransferMessage>?                                CustomMessageTransferMessageSerializer                       { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                             CustomBinaryDataTransferRequestSerializer                    { get; set; }
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                            CustomBinaryDataTransferResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                     CustomDeleteFileRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileResponse>?                                    CustomDeleteFileResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<GetFileRequest>?                                        CustomGetFileRequestSerializer                               { get; set; }
        public CustomBinarySerializerDelegate <GetFileResponse>?                                       CustomGetFileResponseSerializer                              { get; set; }
        public CustomBinarySerializerDelegate <SendFileRequest>?                                       CustomSendFileRequestSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SendFileResponse>?                                      CustomSendFileResponseSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                  CustomListDirectoryRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryResponse>?                                 CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                             CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?                            CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                    CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleResponse>?                                   CustomAddUserRoleResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?                         CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                                 CustomDeleteUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?                                CustomDeleteUserRoleResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                          CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomBinarySerializerDelegate <SecureDataTransferRequest>?                             CustomSecureDataTransferRequestSerializer                    { get; set; }
        public CustomBinarySerializerDelegate <SecureDataTransferResponse>?                            CustomSecureDataTransferResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                          CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?                         CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                                 CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?                                CustomUpdateUserRoleResponseSerializer                       { get; set; }


        // Overlay Network Extensions
        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyMessage>?                          CustomNotifyNetworkTopologyMessageSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?                          CustomNotifyNetworkTopologyRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?                         CustomNotifyNetworkTopologyResponseSerializer                { get; set; }


        // NTS Extensions
        public CustomJObjectSerializerDelegate<NTSKERequest>?                                          CustomNTSKERequestSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<NTSKEResponse>?                                         CustomNTSKEResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<NTSKEServerInfo>?                                       CustomNTSKEServerInfoSerializer                              { get; set; }


        #region Data Structures

        public CustomJObjectSerializerDelegate<StatusInfo>?                                            CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?     CustomEVSEStatusInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?     CustomEVSEStatusInfoSerializer2                              { get; set; }
        public CustomJObjectSerializerDelegate<SignaturePolicy>?                                       CustomSignaturePolicySerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                             CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<UserRole>?                                              CustomUserRoleSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                            CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                              CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                     CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                             CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                  CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                              CustomVariableSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                         CustomPeriodicEventStreamParametersSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                         CustomLogParametersSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                       CustomSetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                       CustomGetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                     CustomSetMonitoringDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                              CustomNetworkConnectionProfileSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                      CustomVPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                      CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashDataChain>?                              CustomCertificateHashDataChainSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                   CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                     CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                               CustomIdTokenSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                        CustomAdditionalInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                           CustomIdTokenInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                        CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                       CustomChargingProfileSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingLimit>?                                         CustomChargingLimitSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<LimitAtSoC>?                                            CustomLimitAtSoCSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                      CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                                CustomChargingSchedulePeriodSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                      CustomV2XFreqWattEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                    CustomV2XSignalWattEntrySerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                           CustomSalesTariffSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                      CustomSalesTariffEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                  CustomRelativeTimeIntervalSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                       CustomConsumptionCostSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                  CustomCostSerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Contact>?                                               CustomContactSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ChargingScheduleUpdate>?                                CustomChargingScheduleUpdateSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<Tariff>?                                                CustomTariffSerializer                                       { get; set; }
        public CustomJObjectSerializerDelegate<TariffEnergy>?                                          CustomTariffEnergySerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<TariffEnergyPrice>?                                     CustomTariffEnergyPriceSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<TariffTime>?                                            CustomTariffTimeSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<TariffTimePrice>?                                       CustomTariffTimePriceSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<TariffFixed>?                                           CustomTariffFixedSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<TariffFixedPrice>?                                      CustomTariffFixedPriceSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<TariffAssignment>?                                      CustomTariffAssignmentSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ClearTariffsResult>?                                    CustomClearTariffsResultSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?      CustomAbsolutePriceScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?             CustomPriceRuleStackSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                  CustomPriceRuleSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                    CustomTaxRuleSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?           CustomOverstayRuleListSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?               CustomOverstayRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalSelectedService>?  CustomAdditionalServiceSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?         CustomPriceLevelScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?    CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<TransactionLimits>?                                     CustomTransactionLimitsSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                              CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                  CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                           CustomMessageInfoSerializer                                  { get; set; }



        public CustomJObjectSerializerDelegate<OCPPv2_1.ChargingStation>?                              CustomChargingStationSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                             CustomEventDataSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                            CustomReportDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                                     CustomVariableAttributeSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                               CustomVariableCharacteristicsSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                                        CustomMonitoringDataSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                                    CustomVariableMonitoringSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                                       CustomOCSPRequestDataSerializer                              { get; set; }

        public CustomJObjectSerializerDelegate<ChargingNeeds>?                                         CustomChargingNeedsSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                                  CustomACChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                                  CustomDCChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                                 CustomV2XChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                         CustomEVEnergyOfferSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                       CustomEVPowerScheduleSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                  CustomEVPowerScheduleEntrySerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                               CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                          CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                                           CustomEVPriceRuleSerializer                                  { get; set; }

        public CustomJObjectSerializerDelegate<Transaction>?                                           CustomTransactionSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                            CustomMeterValueSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                          CustomSampledValueSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                                      CustomSignedMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                        CustomUnitsOfMeasureSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableResult>?                                     CustomSetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                                     CustomGetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                   CustomSetMonitoringResultSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                                 CustomClearMonitoringResultSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                                     CustomCompositeScheduleSerializer                            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<Signature>?                                              CustomBinarySignatureSerializer                              { get; set; }


        // E2E Security Extensions



        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<Tariff>?                                                CustomChargingTariffSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                                                 CustomPriceSerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<TaxRate>?                                               CustomTaxRateSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<TariffConditions>?                                      CustomTariffConditionsSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                                             CustomEnergyMixSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                                          CustomEnergySourceSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                   CustomEnvironmentalImpactSerializer                          { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                            CustomNetworkTopologyInformationSerializer                   { get; set; }

        #endregion

        #endregion

        #region Custom JSON parser delegates

        #region CS

        #region Certificates
        public CustomJObjectParserDelegate<CS.  Get15118EVCertificateRequest>?                CustomGet15118EVCertificateRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.Get15118EVCertificateResponse>?               CustomGet15118EVCertificateResponseParser                { get; set; }
        public CustomJObjectParserDelegate<CS.  GetCertificateStatusRequest>?                 CustomGetCertificateStatusRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetCertificateStatusResponse>?                CustomGetCertificateStatusResponseParser                 { get; set; }
        //spublic CustomJObjectParserDelegate<CS.  GetCRLRequest>?                               CustomGetCRLRequestParser                                { get; set; }
        //spublic CustomJObjectParserDelegate<CSMS.GetCRLResponse>?                              CustomGetCRLResponseParser                               { get; set; }
        public CustomJObjectParserDelegate<CS.  SignCertificateRequest>?                      CustomSignCertificateRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<CSMS.SignCertificateResponse>?                     CustomSignCertificateResponseParser                      { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<CS.  AuthorizeRequest>?                            CustomAuthorizeRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CSMS.AuthorizeResponse>?                           CustomAuthorizeResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearedChargingLimitRequest>?                 CustomClearedChargingLimitRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.ClearedChargingLimitResponse>?                CustomClearedChargingLimitResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<CS.  MeterValuesRequest>?                          CustomMeterValuesRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<CSMS.MeterValuesResponse>?                         CustomMeterValuesResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyChargingLimitRequest>?                  CustomNotifyChargingLimitRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyChargingLimitResponse>?                 CustomNotifyChargingLimitResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyEVChargingNeedsRequest>?                CustomNotifyEVChargingNeedsRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyEVChargingNeedsResponse>?               CustomNotifyEVChargingNeedsResponseParser                { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyEVChargingScheduleRequest>?             CustomNotifyEVChargingScheduleRequestParser              { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyEVChargingScheduleResponse>?            CustomNotifyEVChargingScheduleResponseParser             { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyPriorityChargingRequest>?               CustomNotifyPriorityChargingRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyPriorityChargingResponse>?              CustomNotifyPriorityChargingResponseParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifySettlementRequest>?                     CustomNotifySettlementRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifySettlementResponse>?                    CustomNotifySettlementResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  PullDynamicScheduleUpdateRequest>?            CustomPullDynamicScheduleUpdateRequestParser             { get; set; }
        public CustomJObjectParserDelegate<CSMS.PullDynamicScheduleUpdateResponse>?           CustomPullDynamicScheduleUpdateResponseParser            { get; set; }
        public CustomJObjectParserDelegate<CS.  ReportChargingProfilesRequest>?               CustomReportChargingProfilesRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.ReportChargingProfilesResponse>?              CustomReportChargingProfilesResponseParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  ReservationStatusUpdateRequest>?              CustomReservationStatusUpdateRequestParser               { get; set; }
        public CustomJObjectParserDelegate<CSMS.ReservationStatusUpdateResponse>?             CustomReservationStatusUpdateResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CS.  StatusNotificationRequest>?                   CustomStatusNotificationRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.StatusNotificationResponse>?                  CustomStatusNotificationResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  TransactionEventRequest>?                     CustomTransactionEventRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<CSMS.TransactionEventResponse>?                    CustomTransactionEventResponseParser                     { get; set; }

        #endregion

        #region Customer
        public CustomJObjectParserDelegate<CS.  NotifyCustomerInformationRequest>?            CustomNotifyCustomerInformationRequestParser             { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyCustomerInformationResponse>?           CustomNotifyCustomerInformationResponseParser            { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyDisplayMessagesRequest>?                CustomNotifyDisplayMessagesRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyDisplayMessagesResponse>?               CustomNotifyDisplayMessagesResponseParser                { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<CS.  LogStatusNotificationRequest>?                CustomLogStatusNotificationRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.LogStatusNotificationResponse>?               CustomLogStatusNotificationResponseParser                { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyEventRequest>?                          CustomNotifyEventRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyEventResponse>?                         CustomNotifyEventResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyMonitoringReportRequest>?               CustomNotifyMonitoringReportRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyMonitoringReportResponse>?              CustomNotifyMonitoringReportResponseParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyReportRequest>?                         CustomNotifyReportRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyReportResponse>?                        CustomNotifyReportResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CS.  SecurityEventNotificationRequest>?            CustomSecurityEventNotificationRequestParser             { get; set; }
        public CustomJObjectParserDelegate<CSMS.SecurityEventNotificationResponse>?           CustomSecurityEventNotificationResponseParser            { get; set; }

        #endregion

        #region Firmware

        public CustomJObjectParserDelegate<CS.  BootNotificationRequest>?                     CustomBootNotificationRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<CSMS.BootNotificationResponse>?                    CustomBootNotificationResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  FirmwareStatusNotificationRequest>?           CustomFirmwareStatusNotificationRequestParser            { get; set; }
        public CustomJObjectParserDelegate<CSMS.FirmwareStatusNotificationResponse>?          CustomFirmwareStatusNotificationResponseParser           { get; set; }
        public CustomJObjectParserDelegate<CS.  HeartbeatRequest>?                            CustomHeartbeatRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CSMS.HeartbeatResponse>?                           CustomHeartbeatResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  PublishFirmwareStatusNotificationRequest>?    CustomPublishFirmwareStatusNotificationRequestParser     { get; set; }
        public CustomJObjectParserDelegate<CSMS.PublishFirmwareStatusNotificationResponse>?   CustomPublishFirmwareStatusNotificationResponseParser    { get; set; }

        #endregion

        #endregion

        #region CSMS

        #region Certificates

        public CustomJObjectParserDelegate<CSMS.CertificateSignedRequest>?                    CustomCertificateSignedRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  CertificateSignedResponse>?                   CustomCertificateSignedResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.DeleteCertificateRequest>?                    CustomDeleteCertificateRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  DeleteCertificateResponse>?                   CustomDeleteCertificateResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetInstalledCertificateIdsRequest>?           CustomGetInstalledCertificateIdsRequestParser            { get; set; }
        public CustomJObjectParserDelegate<CS.  GetInstalledCertificateIdsResponse>?          CustomGetInstalledCertificateIdsResponseParser           { get; set; }
        public CustomJObjectParserDelegate<CSMS.InstallCertificateRequest>?                   CustomInstallCertificateRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.  InstallCertificateResponse>?                  CustomInstallCertificateResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyCRLRequest>?                            CustomNotifyCRLRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyCRLResponse>?                           CustomNotifyCRLResponseParser                            { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<CSMS.CancelReservationRequest>?                    CustomCancelReservationRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  CancelReservationResponse>?                   CustomCancelReservationResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.ClearChargingProfileRequest>?                 CustomClearChargingProfileRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearChargingProfileResponse>?                CustomClearChargingProfileResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetChargingProfilesRequest>?                  CustomGetChargingProfilesRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  GetChargingProfilesResponse>?                 CustomGetChargingProfilesResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetCompositeScheduleRequest>?                 CustomGetCompositeScheduleRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.  GetCompositeScheduleResponse>?                CustomGetCompositeScheduleResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetTransactionStatusRequest>?                 CustomGetTransactionStatusRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.  GetTransactionStatusResponse>?                CustomGetTransactionStatusResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyAllowedEnergyTransferRequest>?          CustomNotifyAllowedEnergyTransferRequestParser           { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyAllowedEnergyTransferResponse>?         CustomNotifyAllowedEnergyTransferResponseParser          { get; set; }
        public CustomJObjectParserDelegate<CSMS.NotifyWebPaymentStartedRequest>?              CustomNotifyWebPaymentStartedRequestParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  NotifyWebPaymentStartedResponse>?             CustomNotifyWebPaymentStartedResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CSMS.RequestStartTransactionRequest>?              CustomRequestStartTransactionRequestParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  RequestStartTransactionResponse>?             CustomRequestStartTransactionResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CSMS.RequestStopTransactionRequest>?               CustomRequestStopTransactionRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CS.  RequestStopTransactionResponse>?              CustomRequestStopTransactionResponseParser               { get; set; }
        public CustomJObjectParserDelegate<CSMS.ReserveNowRequest>?                           CustomReserveNowRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  ReserveNowResponse>?                          CustomReserveNowResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetChargingProfileRequest>?                   CustomSetChargingProfileRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.  SetChargingProfileResponse>?                  CustomSetChargingProfileResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.UnlockConnectorRequest>?                      CustomUnlockConnectorRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<CS.  UnlockConnectorResponse>?                     CustomUnlockConnectorResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<CSMS.UpdateDynamicScheduleRequest>?                CustomUpdateDynamicScheduleRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CS.  UpdateDynamicScheduleResponse>?               CustomUpdateDynamicScheduleResponseParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.UsePriorityChargingRequest>?                  CustomUsePriorityChargingRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  UsePriorityChargingResponse>?                 CustomUsePriorityChargingResponseParser                  { get; set; }


        public CustomJObjectParserDelegate<CSMS.ChangeTransactionTariffRequest>?              CustomChangeTransactionTariffRequestParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  ChangeTransactionTariffResponse>?             CustomChangeTransactionTariffResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CSMS.ClearTariffsRequest>?                         CustomClearTariffsRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearTariffsResponse>?                        CustomClearTariffsResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetDefaultTariffRequest>?                     CustomSetDefaultTariffRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<CS.  SetDefaultTariffResponse>?                    CustomSetDefaultTariffResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetTariffsRequest>?                           CustomGetTariffsRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  GetTariffsResponse>?                          CustomGetTariffsResponseParser                           { get; set; }

        #endregion

        #region Customer

        public CustomJObjectParserDelegate<CSMS.ClearDisplayMessageRequest>?                  CustomClearDisplayMessageRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearDisplayMessageResponse>?                 CustomClearDisplayMessageResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.CostUpdatedRequest>?                          CustomCostUpdatedRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<CS.  CostUpdatedResponse>?                         CustomCostUpdatedResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<CSMS.CustomerInformationRequest>?                  CustomCustomerInformationRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  CustomerInformationResponse>?                 CustomCustomerInformationResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetDisplayMessagesRequest>?                   CustomGetDisplayMessagesRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.  GetDisplayMessagesResponse>?                  CustomGetDisplayMessagesResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetDisplayMessageRequest>?                    CustomSetDisplayMessageRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  SetDisplayMessageResponse>?                   CustomSetDisplayMessageResponseParser                    { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<CSMS.ChangeAvailabilityRequest>?                   CustomChangeAvailabilityRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.  ChangeAvailabilityResponse>?                  CustomChangeAvailabilityResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.ClearVariableMonitoringRequest>?              CustomClearVariableMonitoringRequestParser               { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearVariableMonitoringResponse>?             CustomClearVariableMonitoringResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetBaseReportRequest>?                        CustomGetBaseReportRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<CS.  GetBaseReportResponse>?                       CustomGetBaseReportResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetLogRequest>?                               CustomGetLogRequestParser                                { get; set; }
        public CustomJObjectParserDelegate<CS.  GetLogResponse>?                              CustomGetLogResponseParser                               { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetMonitoringReportRequest>?                  CustomGetMonitoringReportRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  GetMonitoringReportResponse>?                 CustomGetMonitoringReportResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetReportRequest>?                            CustomGetReportRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CS.  GetReportResponse>?                           CustomGetReportResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetVariablesRequest>?                         CustomGetVariablesRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<CS.  GetVariablesResponse>?                        CustomGetVariablesResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetMonitoringBaseRequest>?                    CustomSetMonitoringBaseRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  SetMonitoringBaseResponse>?                   CustomSetMonitoringBaseResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetMonitoringLevelRequest>?                   CustomSetMonitoringLevelRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.  SetMonitoringLevelResponse>?                  CustomSetMonitoringLevelResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetNetworkProfileRequest>?                    CustomSetNetworkProfileRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  SetNetworkProfileResponse>?                   CustomSetNetworkProfileResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetVariableMonitoringRequest>?                CustomSetVariableMonitoringRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CS.  SetVariableMonitoringResponse>?               CustomSetVariableMonitoringResponseParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetVariablesRequest>?                         CustomSetVariablesRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<CS.  SetVariablesResponse>?                        CustomSetVariablesResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CSMS.TriggerMessageRequest>?                       CustomTriggerMessageRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<CS.  TriggerMessageResponse>?                      CustomTriggerMessageResponseParser                       { get; set; }

        #endregion

        #region E2EChargingTariffsExtensions

        public CustomJObjectParserDelegate<CSMS.GetDefaultChargingTariffRequest>?             CustomGetDefaultChargingTariffRequestParser              { get; set; }
        public CustomJObjectParserDelegate<CS.  GetDefaultChargingTariffResponse>?            CustomGetDefaultChargingTariffResponseParser             { get; set; }
        public CustomJObjectParserDelegate<CSMS.RemoveDefaultChargingTariffRequest>?          CustomRemoveDefaultChargingTariffRequestParser           { get; set; }
        public CustomJObjectParserDelegate<CS.  RemoveDefaultChargingTariffResponse>?         CustomRemoveDefaultChargingTariffResponseParser          { get; set; }
        public CustomJObjectParserDelegate<CSMS.SetDefaultE2EChargingTariffRequest>?          CustomSetDefaultE2EChargingTariffRequestParser           { get; set; }
        public CustomJObjectParserDelegate<CS.  SetDefaultE2EChargingTariffResponse>?         CustomSetDefaultE2EChargingTariffResponseParser          { get; set; }

        #endregion

        #region Firmware

        public CustomJObjectParserDelegate<CSMS.PublishFirmwareRequest>?                      CustomPublishFirmwareRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<CS.  PublishFirmwareResponse>?                     CustomPublishFirmwareResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<CSMS.ResetRequest>?                                CustomResetRequestParser                                 { get; set; }
        public CustomJObjectParserDelegate<CS.  ResetResponse>?                               CustomResetResponseParser                                { get; set; }
        public CustomJObjectParserDelegate<CSMS.UnpublishFirmwareRequest>?                    CustomUnpublishFirmwareRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CS.  UnpublishFirmwareResponse>?                   CustomUnpublishFirmwareResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CSMS.UpdateFirmwareRequest>?                       CustomUpdateFirmwareRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<CS.  UpdateFirmwareResponse>?                      CustomUpdateFirmwareResponseParser                       { get; set; }

        #endregion

        #region Grid

        public CustomJObjectParserDelegate<CSMS.AFRRSignalRequest>?                           CustomAFRRSignalRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  AFRRSignalResponse>?                          CustomAFRRSignalResponseParser                           { get; set; }

        #endregion

        #region LocalList

        public CustomJObjectParserDelegate<CSMS.ClearCacheRequest>?                           CustomClearCacheRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CS.  ClearCacheResponse>?                          CustomClearCacheResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<CSMS.GetLocalListVersionRequest>?                  CustomGetLocalListVersionRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.  GetLocalListVersionResponse>?                 CustomGetLocalListVersionResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CSMS.SendLocalListRequest>?                        CustomSendLocalListRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<CS.  SendLocalListResponse>?                       CustomSendLocalListResponseParser                        { get; set; }

        #endregion

        #endregion

        public CustomJObjectParserDelegate<DataTransferRequest>?                                       CustomDataTransferRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<DataTransferResponse>?                                      CustomDataTransferResponseParser                         { get; set; }


        public CustomJObjectParserDelegate<MessageTransferMessage>?                                    CustomMessageTransferMessageParser                       { get; set; }



        // BinaryDataStreamsExtensions
        public CustomBinaryParserDelegate<BinaryDataTransferRequest>?                                  CustomBinaryDataTransferRequestParser                    { get; set; }
        public CustomBinaryParserDelegate<BinaryDataTransferResponse>?                                 CustomBinaryDataTransferResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<DeleteFileRequest>?                                         CustomDeleteFileRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<DeleteFileResponse>?                                        CustomDeleteFileResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<GetFileRequest>?                                            CustomGetFileRequestParser                               { get; set; }
        public CustomBinaryParserDelegate <GetFileResponse>?                                           CustomGetFileResponseParser                              { get; set; }
        public CustomJObjectParserDelegate<ListDirectoryRequest>?                                      CustomListDirectoryRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<ListDirectoryResponse>?                                     CustomListDirectoryResponseParser                        { get; set; }
        public CustomBinaryParserDelegate <SendFileRequest>?                                           CustomSendFileRequestParser                              { get; set; }
        public CustomJObjectParserDelegate<SendFileResponse>?                                          CustomSendFileResponseParser                             { get; set; }


        // E2ESecurityExtensions
        public CustomBinaryParserDelegate<SecureDataTransferRequest>?                                  CustomSecureDataTransferRequestParser                    { get; set; }
        public CustomBinaryParserDelegate<SecureDataTransferResponse>?                                 CustomSecureDataTransferResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<AddSignaturePolicyRequest>?                                 CustomAddSignaturePolicyRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<AddSignaturePolicyResponse>?                                CustomAddSignaturePolicyResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<AddUserRoleRequest>?                                        CustomAddUserRoleRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<AddUserRoleResponse>?                                       CustomAddUserRoleResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<DeleteSignaturePolicyRequest>?                              CustomDeleteSignaturePolicyRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?                             CustomDeleteSignaturePolicyResponseParser                { get; set; }
        public CustomJObjectParserDelegate<DeleteUserRoleRequest>?                                     CustomDeleteUserRoleRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<DeleteUserRoleResponse>?                                    CustomDeleteUserRoleResponseParser                       { get; set; }
        public CustomJObjectParserDelegate<UpdateSignaturePolicyRequest>?                              CustomUpdateSignaturePolicyRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?                             CustomUpdateSignaturePolicyResponseParser                { get; set; }
        public CustomJObjectParserDelegate<UpdateUserRoleRequest>?                                     CustomUpdateUserRoleRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<UpdateUserRoleResponse>?                                    CustomUpdateUserRoleResponseParser                       { get; set; }


        public CustomJObjectParserDelegate<ChargingStation>?                                           CustomChargingStationParser                              { get; set; }
        public CustomJObjectParserDelegate<Signature>?                                                 CustomSignatureParser                                    { get; set; }
        public CustomBinaryParserDelegate<Signature>?                                                  CustomBinarySignatureParser                              { get; set; }
        public CustomJObjectParserDelegate<CustomData>?                                                CustomCustomDataParser                                   { get; set; }
        public CustomJObjectParserDelegate<StatusInfo>?                                                CustomStatusInfoParser                                   { get; set; }
        public CustomJObjectParserDelegate<CompositeSchedule>?                                         CustomCompositeScheduleParser                            { get; set; }
        public CustomJObjectParserDelegate<ChargingSchedulePeriod>?                                    CustomChargingSchedulePeriodParser                       { get; set; }
        public CustomJObjectParserDelegate<ClearMonitoringResult>?                                     CustomClearMonitoringResultParser                        { get; set; }


        public CustomJObjectParserDelegate<GetVariableResult>?                                         CustomGetVariableResultParser                            { get; set; }
        public CustomJObjectParserDelegate<Component>?                                                 CustomComponentParser                                    { get; set; }
        public CustomJObjectParserDelegate<EVSE>?                                                      CustomEVSEParser                                         { get; set; }
        public CustomJObjectParserDelegate<Variable>?                                                  CustomVariableParser                                     { get; set; }
        public CustomJObjectParserDelegate<SetMonitoringResult>?                                       CustomSetMonitoringResultParser                          { get; set; }

        public CustomJObjectParserDelegate<CertificateHashData>?                                       CustomCertificateHashDataParser                          { get; set; }

        public CustomJObjectParserDelegate<IdTokenInfo>?                                               CustomIdTokenInfoParser                                  { get; set; }
        public CustomJObjectParserDelegate<IdToken>?                                                   CustomIdTokenParser                                      { get; set; }
        public CustomJObjectParserDelegate<AdditionalInfo>?                                            CustomAdditionalInfoParser                               { get; set; }
        public CustomJObjectParserDelegate<MessageContent>?                                            CustomMessageContentParser                               { get; set; }


        public CustomJObjectParserDelegate<TariffAssignment>?                                          CustomTariffAssignmentParser                             { get; set; }
        public CustomJObjectParserDelegate<ClearTariffsResult>?                                        CustomClearTariffsResultParser                           { get; set; }


        // Overlay Network Extensions
        public CustomJObjectParserDelegate<NotifyNetworkTopologyMessage>?                              CustomNotifyNetworkTopologyMessageParser                 { get; set; }
        //public CustomJObjectParserDelegate<NotifyNetworkTopologyRequest>?                              CustomNotifyNetworkTopologyRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?                             CustomNotifyNetworkTopologyResponseParser                { get; set; }


        // NTS Extensions
        public CustomJObjectParserDelegate<NTSKERequest>?                                              CustomNTSKERequestParser                                 { get; set; }
        public CustomJObjectParserDelegate<NTSKEResponse>?                                             CustomNTSKEResponseParser                                { get; set; }
        public CustomJObjectParserDelegate<NTSKEServerInfo>?                                           CustomNTSKEServerInfoParser                              { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP Adapter.
        /// </summary>
        /// <param name="NetworkingNode">The attached networking node.</param>
        /// 
        /// <param name="DisableSendHeartbeats">Whether to send heartbeats or not.</param>
        /// <param name="SendHeartbeatsEvery">The optional time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The optional default request time out.</param>
        /// 
        /// <param name="SignaturePolicy">An optional signature policy.</param>
        /// 
        /// <param name="ForwardingSignaturePolicy">An optional signature policy when forwarding OCPP messages.</param>
        /// <param name="DefaultForwardingDecision">The default forwarding decision.</param>
        public OCPPAdapter(INetworkingNode       NetworkingNode,

                           Boolean               DisableSendHeartbeats       = false,
                           TimeSpan?             SendHeartbeatsEvery         = null,

                           TimeSpan?             DefaultRequestTimeout       = null,

                           SignaturePolicy?      SignaturePolicy             = null,

                           SignaturePolicy?      ForwardingSignaturePolicy   = null,
                           ForwardingDecisions?  DefaultForwardingDecision   = null)

        {

            this.NetworkingNode         = NetworkingNode;
            this.DisableSendHeartbeats  = DisableSendHeartbeats;
            this.SendHeartbeatsEvery    = SendHeartbeatsEvery   ?? DefaultSendHeartbeatsEvery;

            this.DefaultRequestTimeout  = DefaultRequestTimeout ?? DefaultRequestTimeoutDefault;

            this.IN                     = new OCPPWebSocketAdapterIN(
                                              NetworkingNode
                                          );

            this.OUT                    = new OCPPWebSocketAdapterOUT(
                                              NetworkingNode
                                          );

            this.FORWARD                = new OCPPWebSocketAdapterFORWARD(
                                              NetworkingNode,
                                              ForwardingSignaturePolicy,
                                              DefaultForwardingDecision
                                          );

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

        }

        #endregion


        public Boolean TryGetComponentConfig(String Name, [NotNullWhen(true)] out List<ComponentConfig>? Components)
        {

            if (componentConfigs.TryGetValue(Name, out Components))
                return true;

            return false;

        }

        public List<ComponentConfig> AddOrUpdateComponentConfig(String                                                      Name,
                                                                Func<String, List<ComponentConfig>>                         AddValueFactory,
                                                                Func<String, List<ComponentConfig>, List<ComponentConfig>>  UpdateValueFactory)

            => componentConfigs.AddOrUpdate(
                   Name,
                   AddValueFactory,
                   UpdateValueFactory
               );



        #region Send    JSON   messages

        #region (internal) SendJSONRequest          (JSONRequestMessage, SentMessageResultDelegate = null)

        internal async Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage     JSONRequestMessage,
                                                               Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(JSONRequestMessage.Destination.Next, out var reachability))
            {

                if      (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendJSONRequest(JSONRequestMessage);

                else if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendJSONRequest(JSONRequestMessage);

            }

            if (SentMessageResultDelegate is not null)
                SentMessageResultDelegate(sentMessageResult);

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendJSONRequestAndWait   (JSONRequestMessage, SentMessageResultDelegate = null)

        internal async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage     JSONRequestMessage,
                                                                     Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = await SendJSONRequest(JSONRequestMessage, SentMessageResultDelegate);

            if (sentMessageResult.Result == SentMessageResults.Success)
            {

                #region 1. Store 'in-flight' request...

                requests.TryAdd(JSONRequestMessage.RequestId,
                                SendRequestState.FromJSONRequest(
                                    Timestamp.Now,
                                    JSONRequestMessage.Destination,
                                    JSONRequestMessage.RequestTimeout,
                                    JSONRequestMessage
                                ));

                #endregion

                #region 2. Wait for response... or timeout!

                do
                {

                    try
                    {

                        await Task.Delay(25, JSONRequestMessage.CancellationToken);

                        if (requests.TryGetValue(JSONRequestMessage.RequestId, out var sendRequestState) &&
                           (sendRequestState?.JSONResponse   is not null ||
                            sendRequestState?.BinaryResponse is not null ||
                            sendRequestState?.HasErrors == true))
                        {

                            requests.TryRemove(JSONRequestMessage.RequestId, out _);

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(OCPPWebSocketAdapterIN), ".", nameof(SendJSONRequestAndWait), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < JSONRequestMessage.RequestTimeout);

                #endregion

                #region 3. When timed out...

                if (requests.TryGetValue(JSONRequestMessage.RequestId, out var sendRequestState2) &&
                    sendRequestState2 is not null)
                {

                    sendRequestState2.JSONRequestErrorMessage = new OCPP_JSONRequestErrorMessage(

                                                                     Timestamp.Now,
                                                                     JSONRequestMessage.EventTrackingId,
                                                                     NetworkingMode.Unknown,
                                                                     SourceRouting.To(JSONRequestMessage.NetworkPath.Source),
                                                                     NetworkPath.From(NetworkingNode.Id),
                                                                     JSONRequestMessage.RequestId,

                                                                     ErrorCode: ResultCode.Timeout

                                                                 );

                    requests.TryRemove(JSONRequestMessage.RequestId, out _);

                    return sendRequestState2;

                }

                #endregion

            }

            else if (sentMessageResult.Result == SentMessageResults.UnknownClient)
            {

                return SendRequestState.FromJSONRequest(

                       RequestTimestamp:         JSONRequestMessage.RequestTimestamp,
                       Destination:              JSONRequestMessage.Destination,
                       Timeout:                  JSONRequestMessage.RequestTimeout,
                       JSONRequest:              JSONRequestMessage,
                       SentMessageResult:        sentMessageResult,
                       ResponseTimestamp:        Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     SourceRouting.To(JSONRequestMessage.NetworkPath.Source),
                                                     NetworkPath.From(NetworkingNode.Id),
                                                     JSONRequestMessage.RequestId,

                                                     ErrorCode:          ResultCode.UnknownClient,
                                                     ErrorDescription:   $"The given networking node '{JSONRequestMessage.Destination.Last()}' is unknown or unreachable!"

                                                 )

                   );

            }


            // Just in case...
            return SendRequestState.FromJSONRequest(

                       RequestTimestamp:         JSONRequestMessage.RequestTimestamp,
                       Destination:              JSONRequestMessage.Destination,
                       Timeout:                  JSONRequestMessage.RequestTimeout,
                       JSONRequest:              JSONRequestMessage,
                       SentMessageResult:        sentMessageResult,
                       ResponseTimestamp:        Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     SourceRouting.To(JSONRequestMessage.NetworkPath.Source),
                                                     NetworkPath.From(NetworkingNode.Id),
                                                     JSONRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.InternalError

                                                 )

                   );

        }

        #endregion

        #region (internal) SendJSONResponse         (JSONRequestMessage)

        internal async Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(JSONResponseMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendJSONResponse(JSONResponseMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendJSONResponse(JSONResponseMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendJSONRequestError     (JSONRequestErrorMessage)

        internal async Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(JSONRequestErrorMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendJSONRequestError(JSONRequestErrorMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendJSONRequestError(JSONRequestErrorMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendJSONResponseError    (JSONResponseErrorMessage)

        internal async Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(JSONResponseErrorMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendJSONResponseError(JSONResponseErrorMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendJSONResponseError(JSONResponseErrorMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendJSONSendMessage      (JSONRequestMessage, SentMessageResultDelegate = null)

        internal async Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage        JSONSendMessage,
                                                                   Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            #region Broadcast

            if (JSONSendMessage.Destination.Next == NetworkingNode_Id.Broadcast)
            {

                foreach (var webSocketClient in NetworkingNode.Routing.AllWebSocketClients)
                    if (webSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                        sentMessageResult = await ocppWebSocketClient.SendJSONSendMessage(JSONSendMessage);

                foreach (var webSocketServer in NetworkingNode.Routing.AllWebSocketServers)
                    if (webSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                        sentMessageResult = await ocppWebSocketServer.SendJSONSendMessage(JSONSendMessage);

                sentMessageResult = SentMessageResult.Broadcast();

            }

            #endregion

            else if (NetworkingNode.Routing.LookupNetworkingNode(JSONSendMessage.Destination.Next, out var reachability))
            {

                if      (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendJSONSendMessage(JSONSendMessage);

                else if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendJSONSendMessage(JSONSendMessage);

            }

            if (SentMessageResultDelegate is not null)
                SentMessageResultDelegate(sentMessageResult);

            return sentMessageResult;

        }

        #endregion

        #endregion

        #region Send    binary messages

        #region (internal) SendBinaryRequest        (BinaryRequestMessage, SentMessageResultDelegate = null)

        internal async Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage   BinaryRequestMessage,
                                                                 Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(BinaryRequestMessage.Destination.Next, out var reachability))
            {

                if      (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendBinaryRequest(BinaryRequestMessage);

                else if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendBinaryRequest(BinaryRequestMessage);

            }

            if (SentMessageResultDelegate is not null)
                SentMessageResultDelegate(sentMessageResult);

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendBinaryRequestAndWait (BinaryRequestMessage, SentMessageResultDelegate = null)

        internal async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage   BinaryRequestMessage,
                                                                       Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = await SendBinaryRequest(BinaryRequestMessage, SentMessageResultDelegate);

            if (sentMessageResult.Result == SentMessageResults.Success)
            {

                #region (internal) 1. Store 'in-flight' request...

                requests.TryAdd(BinaryRequestMessage.RequestId,
                                SendRequestState.FromBinaryRequest(
                                    Timestamp.Now,
                                    BinaryRequestMessage.Destination,
                                    BinaryRequestMessage.RequestTimeout,
                                    BinaryRequestMessage
                                ));

                #endregion

                #region (internal) 2. Wait for response... or timeout!

                do
                {

                    try
                    {

                        await Task.Delay(25, BinaryRequestMessage.CancellationToken);

                        if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var sendRequestState) &&
                           (sendRequestState?.JSONResponse   is not null ||
                            sendRequestState?.BinaryResponse is not null ||
                            sendRequestState?.HasErrors == true))
                        {

                            requests.TryRemove(BinaryRequestMessage.RequestId, out _);

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(OCPPWebSocketAdapterIN), ".", nameof(SendJSONRequestAndWait), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < BinaryRequestMessage.RequestTimeout);

                #endregion

                #region (internal) 3. When timed out...

                if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var sendRequestState2) &&
                    sendRequestState2 is not null)
                {

                    sendRequestState2.JSONRequestErrorMessage =  new OCPP_JSONRequestErrorMessage(

                                                                     Timestamp.Now,
                                                                     BinaryRequestMessage.EventTrackingId,
                                                                     NetworkingMode.Unknown,
                                                                     SourceRouting.To(BinaryRequestMessage.NetworkPath.Source),
                                                                     NetworkPath.From(NetworkingNode.Id),
                                                                     BinaryRequestMessage.RequestId,

                                                                     ErrorCode: ResultCode.Timeout

                                                                 );

                    requests.TryRemove(BinaryRequestMessage.RequestId, out _);

                    return sendRequestState2;

                }

                #endregion

            }

            // Just in case...
            return SendRequestState.FromBinaryRequest(

                       RequestTimestamp:         BinaryRequestMessage.RequestTimestamp,
                       Destination:              BinaryRequestMessage.Destination,
                       Timeout:                  BinaryRequestMessage.RequestTimeout,
                       BinaryRequest:            BinaryRequestMessage,
                       SentMessageResult:        sentMessageResult,
                       ResponseTimestamp:        Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     BinaryRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     SourceRouting.To(BinaryRequestMessage.NetworkPath.Source),
                                                     NetworkPath.From(NetworkingNode.Id),
                                                     BinaryRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.InternalError

                                                 )

                   );

        }

        #endregion

        #region (internal) SendBinaryResponse       (BinaryResponseMessage)

        internal async Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(BinaryResponseMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendBinaryResponse(BinaryResponseMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendBinaryResponse(BinaryResponseMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendBinaryRequestError   (BinaryRequestErrorMessage)

        internal async Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(BinaryRequestErrorMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendBinaryRequestError(BinaryRequestErrorMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendBinaryRequestError(BinaryRequestErrorMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendBinaryResponseError  (BinaryResponseErrorMessage)

        internal async Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(BinaryResponseErrorMessage.Destination.Next, out var reachability))
            {

                if (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendBinaryResponseError(BinaryResponseErrorMessage);

                if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendBinaryResponseError(BinaryResponseErrorMessage);

            }

            return sentMessageResult;

        }

        #endregion

        #region (internal) SendBinarySendMessage    (BinarySendMessage, SentMessageResultDelegate = null)

        internal async Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage      BinarySendMessage,
                                                                     Action<SentMessageResult>?  SentMessageResultDelegate   = null)
        {

            var sentMessageResult = SentMessageResult.UnknownClient();

            if (NetworkingNode.Routing.LookupNetworkingNode(BinarySendMessage.Destination.Next, out var reachability))
            {

                if      (reachability.WWCPWebSocketClient is OCPPWebSocketClient ocppWebSocketClient)
                    sentMessageResult = await ocppWebSocketClient.SendBinarySendMessage(BinarySendMessage);

                else if (reachability.WWCPWebSocketServer is OCPPWebSocketServer ocppWebSocketServer)
                    sentMessageResult = await ocppWebSocketServer.SendBinarySendMessage(BinarySendMessage);

            }

            if (SentMessageResultDelegate is not null)
                SentMessageResultDelegate(sentMessageResult);

            return sentMessageResult;

        }

        #endregion

        #endregion

        #region Receive JSON   messages

        #region (internal) ReceiveJSONResponse        (JSONResponseMessag,         WebSocketConnection)

        internal Boolean ReceiveJSONResponse(OCPP_JSONResponseMessage  JSONResponseMessage,
                                             IWebSocketConnection      WebSocketConnection)
        {

            if (requests.TryGetValue(JSONResponseMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.JSONResponse                 = JSONResponseMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = JSONResponseMessage.Destination;
                sendRequestState.NetworkPathReceived          = JSONResponseMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{JSONResponseMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{JSONResponseMessage.Payload.ToString(Formatting.None)}'!");
            return false;

        }

        #endregion

        #region (internal) ReceiveJSONRequestError    (JSONRequestErrorMessage,    WebSocketConnection)

        internal Boolean ReceiveJSONRequestError(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                 IWebSocketConnection          WebSocketConnection)
        {

            if (requests.TryGetValue(JSONRequestErrorMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.JSONRequestErrorMessage      = JSONRequestErrorMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = JSONRequestErrorMessage.Destination;
                sendRequestState.NetworkPathReceived          = JSONRequestErrorMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP JSON request error message with identificaiton '{JSONRequestErrorMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{JSONRequestErrorMessage.ToJSON().ToString(Formatting.None)}'!");
            return false;

        }

        #endregion

        #region (internal) ReceiveJSONResponseError   (JSONResponseErrorMessage,   WebSocketConnection)

        internal Boolean ReceiveJSONResponseError(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                  IWebSocketConnection           WebSocketConnection)
        {

            if (requests.TryGetValue(JSONResponseErrorMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.JSONResponseErrorMessage     = JSONResponseErrorMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = JSONResponseErrorMessage.Destination;
                sendRequestState.NetworkPathReceived          = JSONResponseErrorMessage.NetworkPath;

                //ToDo: This has to be forwarded actively, as it is not expected (async)!

                return true;

            }

            DebugX.Log($"Received an unknown OCPP JSON response error message with identificaiton '{JSONResponseErrorMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{JSONResponseErrorMessage.ToJSON().ToString(Formatting.None)}'!");
            return false;

        }

        #endregion

        #endregion

        #region Receive binary messages

        #region (internal) ReceiveBinaryResponse      (BinaryResponseMessage,      WebSocketConnection)

        internal Boolean ReceiveBinaryResponse(OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                               IWebSocketConnection        WebSocketConnection)
        {

            if (requests.TryGetValue(BinaryResponseMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.BinaryResponse               = BinaryResponseMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = BinaryResponseMessage.Destination;
                sendRequestState.NetworkPathReceived          = BinaryResponseMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{BinaryResponseMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{BinaryResponseMessage.Payload.ToBase64()}'!");
            return false;

        }

        #endregion

        #region (internal) ReceiveBinaryRequestError  (BinaryRequestErrorMessage,  WebSocketConnection)

        internal Boolean ReceiveBinaryRequestError(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                   IWebSocketConnection            WebSocketConnection)
        {

            if (requests.TryGetValue(BinaryRequestErrorMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.BinaryRequestErrorMessage    = BinaryRequestErrorMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = BinaryRequestErrorMessage.Destination;
                sendRequestState.NetworkPathReceived          = BinaryRequestErrorMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP Binary request error message with identificaiton '{BinaryRequestErrorMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{BinaryRequestErrorMessage.ToByteArray().ToBase64()}'!");
            return false;

        }

        #endregion

        #region (internal) ReceiveBinaryResponseError (BinaryResponseErrorMessage, WebSocketConnection)

        internal Boolean ReceiveBinaryResponseError(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                    IWebSocketConnection             WebSocketConnection)
        {

            if (requests.TryGetValue(BinaryResponseErrorMessage.RequestId, out var sendRequestState))
            {

                sendRequestState.ResponseTimestamp            = Timestamp.Now;
                sendRequestState.BinaryResponseErrorMessage   = BinaryResponseErrorMessage;
                sendRequestState.WebSocketConnectionReceived  = WebSocketConnection;
                sendRequestState.DestinationReceived          = BinaryResponseErrorMessage.Destination;
                sendRequestState.NetworkPathReceived          = BinaryResponseErrorMessage.NetworkPath;

                //ToDo: This has to be forwarded actively, as it is not expected (async)!

                return true;

            }

            DebugX.Log($"Received an unknown OCPP Binary response error message with identificaiton '{BinaryResponseErrorMessage.RequestId}' within {NetworkingNode.Id}:{Environment.NewLine}'{BinaryResponseErrorMessage.ToByteArray().ToBase64()}'!");
            return false;

        }

        #endregion

        #endregion


        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("id",                            NetworkingNode.Id.ToString()),

                           new JProperty("IN",                            IN.     ToJSON()),
                           new JProperty("OUT",                           OUT.    ToJSON()),
                           new JProperty("FORWARD",                       FORWARD.ToJSON()),

                           new JProperty("routing",                       NetworkingNode.Routing.ToJSON()),

                           new JProperty("disableSendHeartbeats",         DisableSendHeartbeats),
                           new JProperty("sendHeartbeatsEvery",           SendHeartbeatsEvery.TotalSeconds),
                          //new JProperty("disableSendHeartbeats",         DisableSendHeartbeats),

                           new JProperty("defaultRequestTimeout",         DefaultRequestTimeout.TotalSeconds),

                           new JProperty("signaturePolicies",             new JArray(SignaturePolicies.          Select(signaturePolicy => signaturePolicy.ToJSON())))

                       );

            return json;

        }

    }

}
