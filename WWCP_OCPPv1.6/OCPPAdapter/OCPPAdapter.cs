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
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    /// <summary>
    /// The OCPP Adapter.
    /// </summary>
    public class OCPPAdapter
    {

        #region Data

        //private readonly        ConcurrentDictionary<String, List<ComponentConfig>>  componentConfigs                = [];
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

        public RegistrationStatus DefaultRegistrationStatus { get; set; } = RegistrationStatus.Rejected;


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
        public CustomJObjectSerializerDelegate<CP.BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CP.FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<CP.PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<CP.HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CP.SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CP.LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<CP.SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<CP.Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        //public CustomJObjectSerializerDelegate<CP.ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CP.TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CP.StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CP.ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CP.ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<CP.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        //public CustomJObjectSerializerDelegate<CP.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }

        #endregion

        #region Charging Station Response Messages

        public CustomJObjectSerializerDelegate<CP.ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CP.UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<CP.PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<CP.UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CP.ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CP.TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<   DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<CP.CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CP.DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CP.GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CP.ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CP.ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CP.CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CP.RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CP.RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CP.UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CP.NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        //public CustomJObjectSerializerDelegate<CP.QRCodeScannedResponse>?                               CustomQRCodeScannedResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CP.UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        //public CustomJObjectSerializerDelegate<CP.AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        //public CustomJObjectSerializerDelegate<CP.SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CP.ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CP.CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CP.CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }

        //public CustomJObjectSerializerDelegate<CP.ChangeTransactionTariffResponse>?                     CustomChangeTransactionTariffResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CP.ClearTariffsResponse>?                                CustomClearTariffsResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CP.SetDefaultTariffResponse>?                            CustomSetDefaultTariffResponseSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetTariffsResponse>?                                  CustomGetTariffsResponseSerializer                           { get; set; }


        // E2E Charging Tariff Extensions
        //public CustomJObjectSerializerDelegate<CP.SetDefaultE2EChargingTariffResponse>?                 CustomSetDefaultE2EChargingTariffResponseSerializer          { get; set; }
        //public CustomJObjectSerializerDelegate<CP.GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<CP.RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region CSMS Request  Messages
        public CustomJObjectSerializerDelegate<CS.ResetRequest>?                                   CustomResetRequestSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateFirmwareRequest>?                          CustomUpdateFirmwareRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CS.PublishFirmwareRequest>?                         CustomPublishFirmwareRequestSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<CS.UnpublishFirmwareRequest>?                       CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetBaseReportRequest>?                           CustomGetBaseReportRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetReportRequest>?                               CustomGetReportRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetLogRequest>?                                  CustomGetLogRequestSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetVariablesRequest>?                            CustomSetVariablesRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetVariablesRequest>?                            CustomGetVariablesRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetMonitoringBaseRequest>?                       CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetMonitoringReportRequest>?                     CustomGetMonitoringReportRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetMonitoringLevelRequest>?                      CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetVariableMonitoringRequest>?                   CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CS.ClearVariableMonitoringRequest>?                 CustomClearVariableMonitoringRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetNetworkProfileRequest>?                       CustomSetNetworkProfileRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CS.ChangeAvailabilityRequest>?                      CustomChangeAvailabilityRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.TriggerMessageRequest>?                          CustomTriggerMessageRequestSerializer                        { get; set; }
        

        public CustomJObjectSerializerDelegate<CS.CertificateSignedRequest>?                       CustomCertificateSignedRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CS.InstallCertificateRequest>?                      CustomInstallCertificateRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetInstalledCertificateIdsRequest>?              CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CS.DeleteCertificateRequest>?                       CustomDeleteCertificateRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyCRLRequest>?                               CustomNotifyCRLRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<CS.GetLocalListVersionRequest>?                     CustomGetLocalListVersionRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.SendLocalListRequest>?                           CustomSendLocalListRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearCacheRequest>?                              CustomClearCacheRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CS.ReserveNowRequest>?                              CustomReserveNowRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CS.CancelReservationRequest>?                       CustomCancelReservationRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.RequestStartTransactionRequest>?                 CustomRequestStartTransactionRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CS.RequestStopTransactionRequest>?                  CustomRequestStopTransactionRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetTransactionStatusRequest>?                    CustomGetTransactionStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetChargingProfileRequest>?                      CustomSetChargingProfileRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetChargingProfilesRequest>?                     CustomGetChargingProfilesRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearChargingProfileRequest>?                    CustomClearChargingProfileRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCompositeScheduleRequest>?                    CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CS.UpdateDynamicScheduleRequest>?                   CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyAllowedEnergyTransferRequest>?             CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<CS.QRCodeScannedRequest>?                           CustomQRCodeScannedRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        //public CustomJObjectSerializerDelegate<CS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        //public CustomJObjectSerializerDelegate<CS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<CS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }

        //public CustomJObjectSerializerDelegate<CS.ChangeTransactionTariffRequest>?                 CustomChangeTransactionTariffRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CS.ClearTariffsRequest>?                            CustomClearTariffsRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CS.SetDefaultTariffRequest>?                        CustomSetDefaultTariffRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetTariffsRequest>?                              CustomGetTariffsRequestSerializer                            { get; set; }


        // E2E Charging Tariffs Extensions
        //public CustomJObjectSerializerDelegate<CS.SetDefaultTariffRequest>?                        CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetDefaultChargingTariffRequest>?                CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CS.RemoveDefaultChargingTariffRequest>?             CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages

        public CustomJObjectSerializerDelegate<CS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<CS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<CS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<     DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<CS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<CS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        //public CustomJObjectSerializerDelegate<CS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifySettlementResponse>?                       CustomNotifySettlementResponseSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        //public CustomJObjectSerializerDelegate<CS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion



        public CustomJObjectParserDelegate<DataTransferRequest>?                                     CustomDataTransferRequestParser                              { get; set; }
        public CustomJObjectParserDelegate<DataTransferResponse>?                                    CustomDataTransferResponseParser                             { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomDataTransferResponseSerializer                         { get; set; }


        public CustomBinaryParserDelegate<BinaryDataTransferRequest>?                                CustomBinaryDataTransferRequestParser                        { get; set; }
        public CustomBinaryParserDelegate<BinaryDataTransferResponse>?                               CustomBinaryDataTransferResponseParser                       { get; set; }
        public CustomBinarySerializerDelegate<BinaryDataTransferRequest>?                            CustomBinaryDataTransferRequestSerializer                    { get; set; }
        public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?                           CustomBinaryDataTransferResponseSerializer                   { get; set; }


        public CustomJObjectParserDelegate<FirmwareImage>?                                           CustomFirmwareImageParser                                    { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareImage>?                                       CustomFirmwareImageSerializer                                { get; set; }


        public CustomJObjectParserDelegate<SignedUpdateFirmwareRequest>?                           CustomSignedUpdateFirmwareRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?                       CustomSignedUpdateFirmwareRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?                          CustomSignedUpdateFirmwareResponseParser                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedUpdateFirmwareResponse>?                      CustomSignedUpdateFirmwareResponseSerializer               { get; set; }



        public CustomJObjectParserDelegate<RemoteStartTransactionRequest>?                           CustomRemoteStartTransactionRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?                       CustomRemoteStartTransactionRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<RemoteStartTransactionResponse>?                          CustomRemoteStartTransactionResponseParser                   { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStartTransactionResponse>?                      CustomRemoteStartTransactionResponseSerializer               { get; set; }


        public CustomJObjectParserDelegate<IdTagInfo>?                                               CustomIdTagInfoParser                                        { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                                           CustomIdTagInfoSerializer                                    { get; set; }



        public CustomJObjectParserDelegate<RemoteStopTransactionRequest>?                            CustomRemoteStopTransactionRequestParser                     { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>?                        CustomRemoteStopTransactionRequestSerializer                 { get; set; }

        public CustomJObjectParserDelegate<RemoteStopTransactionResponse>?                           CustomRemoteStopTransactionResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStopTransactionResponse>?                       CustomRemoteStopTransactionResponseSerializer                { get; set; }


        public CustomJObjectParserDelegate<StartTransactionRequest>?                           CustomStartTransactionRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<StartTransactionRequest>?                       CustomStartTransactionRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<StartTransactionResponse>?                           CustomStartTransactionResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<StartTransactionResponse>?                       CustomStartTransactionResponseSerializer                { get; set; }



        public CustomJObjectParserDelegate<StopTransactionRequest>?                           CustomStopTransactionRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<StopTransactionRequest>?                       CustomStopTransactionRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<StopTransactionResponse>?                           CustomStopTransactionResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<StopTransactionResponse>?                       CustomStopTransactionResponseSerializer                { get; set; }



        public CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?                           CustomSignedFirmwareStatusNotificationRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationRequest>?                       CustomSignedFirmwareStatusNotificationRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?                           CustomSignedFirmwareStatusNotificationResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationResponse>?                       CustomSignedFirmwareStatusNotificationResponseSerializer                { get; set; }



        public CustomJObjectParserDelegate<GetDiagnosticsRequest>?                           CustomGetDiagnosticsRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDiagnosticsRequest>?                       CustomGetDiagnosticsRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<GetDiagnosticsResponse>?                           CustomGetDiagnosticsResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDiagnosticsResponse>?                       CustomGetDiagnosticsResponseSerializer                { get; set; }




        public CustomJObjectParserDelegate<ChangeConfigurationRequest>?                           CustomChangeConfigurationRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?                       CustomChangeConfigurationRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<ChangeConfigurationResponse>?                           CustomChangeConfigurationResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<ChangeConfigurationResponse>?                       CustomChangeConfigurationResponseSerializer                { get; set; }



        public CustomJObjectParserDelegate<GetConfigurationRequest>?                           CustomGetConfigurationRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<GetConfigurationRequest>?                       CustomGetConfigurationRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<GetConfigurationResponse>?                           CustomGetConfigurationResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<GetConfigurationResponse>?                       CustomGetConfigurationResponseSerializer                { get; set; }


        public CustomJObjectParserDelegate<ConfigurationKey>? CustomConfigurationKeyParser { get; set; }
        public CustomJObjectSerializerDelegate<ConfigurationKey>? CustomConfigurationKeySerializer { get; set; }



        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?                           CustomDiagnosticsStatusNotificationRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationRequest>?                       CustomDiagnosticsStatusNotificationRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?                           CustomDiagnosticsStatusNotificationResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationResponse>?                       CustomDiagnosticsStatusNotificationResponseSerializer                { get; set; }



        public CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?                           CustomExtendedTriggerMessageRequestParser                    { get; set; }
        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?                       CustomExtendedTriggerMessageRequestSerializer                { get; set; }

        public CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?                           CustomExtendedTriggerMessageResponseParser                    { get; set; }
        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageResponse>?                       CustomExtendedTriggerMessageResponseSerializer                { get; set; }








        //public CustomJObjectSerializerDelegate<MessageTransferMessage>?                              CustomMessageTransferMessageSerializer                       { get; set; }


        // Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                   CustomDeleteFileRequestSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<GetFileRequest>?                                      CustomGetFileRequestSerializer                               { get; set; }
        //public CustomBinarySerializerDelegate <GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
        //public CustomBinarySerializerDelegate <SendFileRequest>?                                     CustomSendFileRequestSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                CustomListDirectoryRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<ListDirectoryResponse>?                               CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        //public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        //public CustomBinarySerializerDelegate <SecureDataTransferRequest>?                           CustomSecureDataTransferRequestSerializer                    { get; set; }
        //public CustomBinarySerializerDelegate <SecureDataTransferResponse>?                          CustomSecureDataTransferResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }


        // Overlay Network Extensions
        //public CustomJObjectSerializerDelegate<NotifyNetworkTopologyMessage>?                        CustomNotifyNetworkTopologyMessageSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?                        CustomNotifyNetworkTopologyRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?                       CustomNotifyNetworkTopologyResponseSerializer                { get; set; }


        // DiagnosticControlExtensions


        public CustomJObjectSerializerDelegate<AdjustTimeScaleRequest>?                              CustomAdjustTimeScaleRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<AdjustTimeScaleResponse>?                             CustomAdjustTimeScaleResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<AttachCableRequest>?                                  CustomAttachCableRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<AttachCableResponse>?                                 CustomAttachCableResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<GetExecutingEnvironmentRequest>?                      CustomGetExecutingEnvironmentRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<GetExecutingEnvironmentResponse>?                     CustomGetExecutingEnvironmentResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<GetPWMValueRequest>?                                  CustomGetPWMValueRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<GetPWMValueResponse>?                                 CustomGetPWMValueResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SetCPVoltageRequest>?                                 CustomSetCPVoltageRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SetCPVoltageResponse>?                                CustomSetCPVoltageResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SetErrorStateRequest>?                                CustomSetErrorStateRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SetErrorStateResponse>?                               CustomSetErrorStateResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<SwipeRFIDCardRequest>?                                CustomSwipeRFIDCardRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SwipeRFIDCardResponse>?                               CustomSwipeRFIDCardResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<TimeTravelRequest>?                                   CustomTimeTravelRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<TimeTravelResponse>?                                  CustomTimeTravelResponseSerializer                           { get; set; }


        #region Data Structures

        //public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
        public CustomJObjectSerializerDelegate<SignaturePolicy>?                                     CustomSignaturePolicySerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<UserRole>?                                            CustomUserRoleSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        //public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        //public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        //public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        //public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        //public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitAtSoCSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        //public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

        //public CustomJObjectSerializerDelegate<Tariff>?                                              CustomTariffSerializer                                       { get; set; }
        //public CustomJObjectSerializerDelegate<TariffEnergy>?                                        CustomTariffEnergySerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<TariffEnergyPrice>?                                   CustomTariffEnergyPriceSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<TariffTime>?                                          CustomTariffTimeSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<TariffTimePrice>?                                     CustomTariffTimePriceSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<TariffFixed>?                                         CustomTariffFixedSerializer                                  { get; set; }
        //public CustomJObjectSerializerDelegate<TariffFixedPrice>?                                    CustomTariffFixedPriceSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<TariffAssignment>?                                    CustomTariffAssignmentSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<ClearTariffsResult>?                                  CustomClearTariffsResultSerializer                           { get; set; }

        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }

        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        //public CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                            { get; set; }

        //public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }



        //public CustomJObjectSerializerDelegate<OCPPv1_6.ChargingStation>?                            CustomChargingStationSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                              { get; set; }

        //public CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                                  { get; set; }

        //public CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }

        //public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<Signature>?                                            CustomBinarySignatureSerializer                              { get; set; }


        // E2E Security Extensions



        // E2E Charging Tariff Extensions
        //public CustomJObjectSerializerDelegate<Tariff>?                                              CustomChargingTariffSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        //public CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                      { get; set; }
        //public CustomJObjectSerializerDelegate<TariffConditions>?                                    CustomTariffConditionsSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer                   { get; set; }

        #endregion

        #endregion

        #region Custom JSON parser delegates

        #region CS

        #region Certificates
        //public CustomJObjectParserDelegate<CP.  Get15118EVCertificateRequest>?                CustomGet15118EVCertificateRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.Get15118EVCertificateResponse>?               CustomGet15118EVCertificateResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetCertificateStatusRequest>?                 CustomGetCertificateStatusRequestParser                  { get; set; }
        //public CustomJObjectParserDelegate<CS.GetCertificateStatusResponse>?                CustomGetCertificateStatusResponseParser                 { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetCRLRequest>?                               CustomGetCRLRequestParser                                { get; set; }
        //public CustomJObjectParserDelegate<CS.GetCRLResponse>?                              CustomGetCRLResponseParser                               { get; set; }
        public CustomJObjectParserDelegate<CP.  SignCertificateRequest>?                      CustomSignCertificateRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<CS.SignCertificateResponse>?                     CustomSignCertificateResponseParser                      { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<CP.  AuthorizeRequest>?                            CustomAuthorizeRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CS.AuthorizeResponse>?                           CustomAuthorizeResponseParser                            { get; set; }
        //public CustomJObjectParserDelegate<CP.  ClearedChargingLimitRequest>?                 CustomClearedChargingLimitRequestParser                  { get; set; }
        //public CustomJObjectParserDelegate<CS.ClearedChargingLimitResponse>?                CustomClearedChargingLimitResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<CP.  MeterValuesRequest>?                          CustomMeterValuesRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<CS.MeterValuesResponse>?                         CustomMeterValuesResponseParser                          { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyChargingLimitRequest>?                  CustomNotifyChargingLimitRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyChargingLimitResponse>?                 CustomNotifyChargingLimitResponseParser                  { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyEVChargingNeedsRequest>?                CustomNotifyEVChargingNeedsRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyEVChargingNeedsResponse>?               CustomNotifyEVChargingNeedsResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyEVChargingScheduleRequest>?             CustomNotifyEVChargingScheduleRequestParser              { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyEVChargingScheduleResponse>?            CustomNotifyEVChargingScheduleResponseParser             { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyPriorityChargingRequest>?               CustomNotifyPriorityChargingRequestParser                { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyPriorityChargingResponse>?              CustomNotifyPriorityChargingResponseParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifySettlementRequest>?                     CustomNotifySettlementRequestParser                      { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifySettlementResponse>?                    CustomNotifySettlementResponseParser                     { get; set; }
        //public CustomJObjectParserDelegate<CP.  PullDynamicScheduleUpdateRequest>?            CustomPullDynamicScheduleUpdateRequestParser             { get; set; }
        //public CustomJObjectParserDelegate<CS.PullDynamicScheduleUpdateResponse>?           CustomPullDynamicScheduleUpdateResponseParser            { get; set; }
        //public CustomJObjectParserDelegate<CP.  ReportChargingProfilesRequest>?               CustomReportChargingProfilesRequestParser                { get; set; }
        //public CustomJObjectParserDelegate<CS.ReportChargingProfilesResponse>?              CustomReportChargingProfilesResponseParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  ReservationStatusUpdateRequest>?              CustomReservationStatusUpdateRequestParser               { get; set; }
        //public CustomJObjectParserDelegate<CS.ReservationStatusUpdateResponse>?             CustomReservationStatusUpdateResponseParser              { get; set; }
        public CustomJObjectParserDelegate<CP.  StatusNotificationRequest>?                   CustomStatusNotificationRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.StatusNotificationResponse>?                  CustomStatusNotificationResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  TransactionEventRequest>?                     CustomTransactionEventRequestParser                      { get; set; }
        //public CustomJObjectParserDelegate<CS.TransactionEventResponse>?                    CustomTransactionEventResponseParser                     { get; set; }

        #endregion

        #region Customer
        //public CustomJObjectParserDelegate<CP.  NotifyCustomerInformationRequest>?            CustomNotifyCustomerInformationRequestParser             { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyCustomerInformationResponse>?           CustomNotifyCustomerInformationResponseParser            { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyDisplayMessagesRequest>?                CustomNotifyDisplayMessagesRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyDisplayMessagesResponse>?               CustomNotifyDisplayMessagesResponseParser                { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<CP.  LogStatusNotificationRequest>?                CustomLogStatusNotificationRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<CS.LogStatusNotificationResponse>?               CustomLogStatusNotificationResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyEventRequest>?                          CustomNotifyEventRequestParser                           { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyEventResponse>?                         CustomNotifyEventResponseParser                          { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyMonitoringReportRequest>?               CustomNotifyMonitoringReportRequestParser                { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyMonitoringReportResponse>?              CustomNotifyMonitoringReportResponseParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyReportRequest>?                         CustomNotifyReportRequestParser                          { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyReportResponse>?                        CustomNotifyReportResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CP.  SecurityEventNotificationRequest>?            CustomSecurityEventNotificationRequestParser             { get; set; }
        public CustomJObjectParserDelegate<CS.SecurityEventNotificationResponse>?           CustomSecurityEventNotificationResponseParser            { get; set; }

        #endregion

        #region Firmware

        public CustomJObjectParserDelegate<CP.  BootNotificationRequest>?                     CustomBootNotificationRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<CS.BootNotificationResponse>?                    CustomBootNotificationResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<CP.  FirmwareStatusNotificationRequest>?           CustomFirmwareStatusNotificationRequestParser            { get; set; }
        public CustomJObjectParserDelegate<CS.FirmwareStatusNotificationResponse>?          CustomFirmwareStatusNotificationResponseParser           { get; set; }
        public CustomJObjectParserDelegate<CP.  HeartbeatRequest>?                            CustomHeartbeatRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<CS.HeartbeatResponse>?                           CustomHeartbeatResponseParser                            { get; set; }
        //public CustomJObjectParserDelegate<CP.  PublishFirmwareStatusNotificationRequest>?    CustomPublishFirmwareStatusNotificationRequestParser     { get; set; }
        //public CustomJObjectParserDelegate<CS.PublishFirmwareStatusNotificationResponse>?   CustomPublishFirmwareStatusNotificationResponseParser    { get; set; }

        #endregion

        #endregion

        #region CSMS

        #region Certificates

        public CustomJObjectParserDelegate<CS.CertificateSignedRequest>?                    CustomCertificateSignedRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CP.  CertificateSignedResponse>?                   CustomCertificateSignedResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.DeleteCertificateRequest>?                    CustomDeleteCertificateRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CP.  DeleteCertificateResponse>?                   CustomDeleteCertificateResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.GetInstalledCertificateIdsRequest>?           CustomGetInstalledCertificateIdsRequestParser            { get; set; }
        public CustomJObjectParserDelegate<CP.  GetInstalledCertificateIdsResponse>?          CustomGetInstalledCertificateIdsResponseParser           { get; set; }
        public CustomJObjectParserDelegate<CS.InstallCertificateRequest>?                   CustomInstallCertificateRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CP.  InstallCertificateResponse>?                  CustomInstallCertificateResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyCRLRequest>?                            CustomNotifyCRLRequestParser                             { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyCRLResponse>?                           CustomNotifyCRLResponseParser                            { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<CS.CancelReservationRequest>?                    CustomCancelReservationRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<CP.  CancelReservationResponse>?                   CustomCancelReservationResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.ClearChargingProfileRequest>?                 CustomClearChargingProfileRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CP.  ClearChargingProfileResponse>?                CustomClearChargingProfileResponseParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.GetChargingProfilesRequest>?                  CustomGetChargingProfilesRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetChargingProfilesResponse>?                 CustomGetChargingProfilesResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.GetCompositeScheduleRequest>?                 CustomGetCompositeScheduleRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<CP.  GetCompositeScheduleResponse>?                CustomGetCompositeScheduleResponseParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.GetTransactionStatusRequest>?                 CustomGetTransactionStatusRequestParser                  { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetTransactionStatusResponse>?                CustomGetTransactionStatusResponseParser                 { get; set; }
        //public CustomJObjectParserDelegate<CS.NotifyAllowedEnergyTransferRequest>?          CustomNotifyAllowedEnergyTransferRequestParser           { get; set; }
        //public CustomJObjectParserDelegate<CP.  NotifyAllowedEnergyTransferResponse>?         CustomNotifyAllowedEnergyTransferResponseParser          { get; set; }
        //public CustomJObjectParserDelegate<CS.QRCodeScannedRequest>?                        CustomQRCodeScannedRequestParser                         { get; set; }
        //public CustomJObjectParserDelegate<CP.  QRCodeScannedResponse>?                       CustomQRCodeScannedResponseParser                        { get; set; }
        //public CustomJObjectParserDelegate<CS.RequestStartTransactionRequest>?              CustomRequestStartTransactionRequestParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  RequestStartTransactionResponse>?             CustomRequestStartTransactionResponseParser              { get; set; }
        //public CustomJObjectParserDelegate<CS.RequestStopTransactionRequest>?               CustomRequestStopTransactionRequestParser                { get; set; }
        //public CustomJObjectParserDelegate<CP.  RequestStopTransactionResponse>?              CustomRequestStopTransactionResponseParser               { get; set; }
        public CustomJObjectParserDelegate<CS.ReserveNowRequest>?                           CustomReserveNowRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CP.  ReserveNowResponse>?                          CustomReserveNowResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<CS.SetChargingProfileRequest>?                   CustomSetChargingProfileRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CP.  SetChargingProfileResponse>?                  CustomSetChargingProfileResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<CS.UnlockConnectorRequest>?                      CustomUnlockConnectorRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<CP.  UnlockConnectorResponse>?                     CustomUnlockConnectorResponseParser                      { get; set; }
        //public CustomJObjectParserDelegate<CS.UpdateDynamicScheduleRequest>?                CustomUpdateDynamicScheduleRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<CP.  UpdateDynamicScheduleResponse>?               CustomUpdateDynamicScheduleResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<CS.UsePriorityChargingRequest>?                  CustomUsePriorityChargingRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  UsePriorityChargingResponse>?                 CustomUsePriorityChargingResponseParser                  { get; set; }


        //public CustomJObjectParserDelegate<CS.ChangeTransactionTariffRequest>?              CustomChangeTransactionTariffRequestParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  ChangeTransactionTariffResponse>?             CustomChangeTransactionTariffResponseParser              { get; set; }
        //public CustomJObjectParserDelegate<CS.ClearTariffsRequest>?                         CustomClearTariffsRequestParser                          { get; set; }
        //public CustomJObjectParserDelegate<CP.  ClearTariffsResponse>?                        CustomClearTariffsResponseParser                         { get; set; }
        //public CustomJObjectParserDelegate<CS.SetDefaultTariffRequest>?                     CustomSetDefaultTariffRequestParser                      { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetDefaultTariffResponse>?                    CustomSetDefaultTariffResponseParser                     { get; set; }
        //public CustomJObjectParserDelegate<CS.GetTariffsRequest>?                           CustomGetTariffsRequestParser                            { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetTariffsResponse>?                          CustomGetTariffsResponseParser                           { get; set; }

        #endregion

        #region Customer

        //public CustomJObjectParserDelegate<CS.ClearDisplayMessageRequest>?                  CustomClearDisplayMessageRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  ClearDisplayMessageResponse>?                 CustomClearDisplayMessageResponseParser                  { get; set; }
        //public CustomJObjectParserDelegate<CS.CostUpdatedRequest>?                          CustomCostUpdatedRequestParser                           { get; set; }
        //public CustomJObjectParserDelegate<CP.  CostUpdatedResponse>?                         CustomCostUpdatedResponseParser                          { get; set; }
        //public CustomJObjectParserDelegate<CS.CustomerInformationRequest>?                  CustomCustomerInformationRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  CustomerInformationResponse>?                 CustomCustomerInformationResponseParser                  { get; set; }
        //public CustomJObjectParserDelegate<CS.GetDisplayMessagesRequest>?                   CustomGetDisplayMessagesRequestParser                    { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetDisplayMessagesResponse>?                  CustomGetDisplayMessagesResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<CS.SetDisplayMessageRequest>?                    CustomSetDisplayMessageRequestParser                     { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetDisplayMessageResponse>?                   CustomSetDisplayMessageResponseParser                    { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<CS.ChangeAvailabilityRequest>?                   CustomChangeAvailabilityRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CP.  ChangeAvailabilityResponse>?                  CustomChangeAvailabilityResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<CS.ClearVariableMonitoringRequest>?              CustomClearVariableMonitoringRequestParser               { get; set; }
        //public CustomJObjectParserDelegate<CP.  ClearVariableMonitoringResponse>?             CustomClearVariableMonitoringResponseParser              { get; set; }
        //public CustomJObjectParserDelegate<CS.GetBaseReportRequest>?                        CustomGetBaseReportRequestParser                         { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetBaseReportResponse>?                       CustomGetBaseReportResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<CS.GetLogRequest>?                               CustomGetLogRequestParser                                { get; set; }
        public CustomJObjectParserDelegate<CP.  GetLogResponse>?                              CustomGetLogResponseParser                               { get; set; }
        //public CustomJObjectParserDelegate<CS.GetMonitoringReportRequest>?                  CustomGetMonitoringReportRequestParser                   { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetMonitoringReportResponse>?                 CustomGetMonitoringReportResponseParser                  { get; set; }
        //public CustomJObjectParserDelegate<CS.GetReportRequest>?                            CustomGetReportRequestParser                             { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetReportResponse>?                           CustomGetReportResponseParser                            { get; set; }
        //public CustomJObjectParserDelegate<CS.GetVariablesRequest>?                         CustomGetVariablesRequestParser                          { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetVariablesResponse>?                        CustomGetVariablesResponseParser                         { get; set; }
        //public CustomJObjectParserDelegate<CS.SetMonitoringBaseRequest>?                    CustomSetMonitoringBaseRequestParser                     { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetMonitoringBaseResponse>?                   CustomSetMonitoringBaseResponseParser                    { get; set; }
        //public CustomJObjectParserDelegate<CS.SetMonitoringLevelRequest>?                   CustomSetMonitoringLevelRequestParser                    { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetMonitoringLevelResponse>?                  CustomSetMonitoringLevelResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<CS.SetNetworkProfileRequest>?                    CustomSetNetworkProfileRequestParser                     { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetNetworkProfileResponse>?                   CustomSetNetworkProfileResponseParser                    { get; set; }
        //public CustomJObjectParserDelegate<CS.SetVariableMonitoringRequest>?                CustomSetVariableMonitoringRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetVariableMonitoringResponse>?               CustomSetVariableMonitoringResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<CS.SetVariablesRequest>?                         CustomSetVariablesRequestParser                          { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetVariablesResponse>?                        CustomSetVariablesResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<CS.TriggerMessageRequest>?                       CustomTriggerMessageRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<CP.  TriggerMessageResponse>?                      CustomTriggerMessageResponseParser                       { get; set; }

        #endregion

        #region E2EChargingTariffsExtensions

        //public CustomJObjectParserDelegate<CS.GetDefaultChargingTariffRequest>?             CustomGetDefaultChargingTariffRequestParser              { get; set; }
        //public CustomJObjectParserDelegate<CP.  GetDefaultChargingTariffResponse>?            CustomGetDefaultChargingTariffResponseParser             { get; set; }
        //public CustomJObjectParserDelegate<CS.RemoveDefaultChargingTariffRequest>?          CustomRemoveDefaultChargingTariffRequestParser           { get; set; }
        //public CustomJObjectParserDelegate<CP.  RemoveDefaultChargingTariffResponse>?         CustomRemoveDefaultChargingTariffResponseParser          { get; set; }
        //public CustomJObjectParserDelegate<CS.SetDefaultE2EChargingTariffRequest>?          CustomSetDefaultE2EChargingTariffRequestParser           { get; set; }
        //public CustomJObjectParserDelegate<CP.  SetDefaultE2EChargingTariffResponse>?         CustomSetDefaultE2EChargingTariffResponseParser          { get; set; }

        #endregion

        #region Firmware

        //public CustomJObjectParserDelegate<CS.PublishFirmwareRequest>?                      CustomPublishFirmwareRequestParser                       { get; set; }
        //public CustomJObjectParserDelegate<CP.  PublishFirmwareResponse>?                     CustomPublishFirmwareResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<CS.ResetRequest>?                                CustomResetRequestParser                                 { get; set; }
        public CustomJObjectParserDelegate<CP.  ResetResponse>?                               CustomResetResponseParser                                { get; set; }
        //public CustomJObjectParserDelegate<CS.UnpublishFirmwareRequest>?                    CustomUnpublishFirmwareRequestParser                     { get; set; }
        //public CustomJObjectParserDelegate<CP.  UnpublishFirmwareResponse>?                   CustomUnpublishFirmwareResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<CS.UpdateFirmwareRequest>?                       CustomUpdateFirmwareRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<CP.  UpdateFirmwareResponse>?                      CustomUpdateFirmwareResponseParser                       { get; set; }

        #endregion

        #region Grid

        //public CustomJObjectParserDelegate<CS.AFRRSignalRequest>?                           CustomAFRRSignalRequestParser                            { get; set; }
        //public CustomJObjectParserDelegate<CP.  AFRRSignalResponse>?                          CustomAFRRSignalResponseParser                           { get; set; }

        #endregion

        #region LocalList

        public CustomJObjectParserDelegate<CS.ClearCacheRequest>?                           CustomClearCacheRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<CP.  ClearCacheResponse>?                          CustomClearCacheResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<CS.GetLocalListVersionRequest>?                  CustomGetLocalListVersionRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CP.  GetLocalListVersionResponse>?                 CustomGetLocalListVersionResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<CS.SendLocalListRequest>?                        CustomSendLocalListRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<CP.  SendLocalListResponse>?                       CustomSendLocalListResponseParser                        { get; set; }

        #endregion

        #endregion



        //public CustomJObjectParserDelegate<MessageTransferMessage>?                                    CustomMessageTransferMessageParser                       { get; set; }



        // BinaryDataStreamsExtensions
        //public CustomBinaryParserDelegate<BinaryDataTransferRequest>?                                  CustomBinaryDataTransferRequestParser                    { get; set; }
        //public CustomBinaryParserDelegate<BinaryDataTransferResponse>?                                 CustomBinaryDataTransferResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<DeleteFileRequest>?                                         CustomDeleteFileRequestParser                            { get; set; }
        //public CustomJObjectParserDelegate<DeleteFileResponse>?                                        CustomDeleteFileResponseParser                           { get; set; }
        //public CustomJObjectParserDelegate<GetFileRequest>?                                            CustomGetFileRequestParser                               { get; set; }
        //public CustomBinaryParserDelegate <GetFileResponse>?                                           CustomGetFileResponseParser                              { get; set; }
        //public CustomJObjectParserDelegate<ListDirectoryRequest>?                                      CustomListDirectoryRequestParser                         { get; set; }
        //public CustomJObjectParserDelegate<ListDirectoryResponse>?                                     CustomListDirectoryResponseParser                        { get; set; }
        //public CustomBinaryParserDelegate <SendFileRequest>?                                           CustomSendFileRequestParser                              { get; set; }
        //public CustomJObjectParserDelegate<SendFileResponse>?                                          CustomSendFileResponseParser                             { get; set; }


        // E2ESecurityExtensions
        //public CustomBinaryParserDelegate<SecureDataTransferRequest>?                                  CustomSecureDataTransferRequestParser                    { get; set; }
        //public CustomBinaryParserDelegate<SecureDataTransferResponse>?                                 CustomSecureDataTransferResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<AddSignaturePolicyRequest>?                                 CustomAddSignaturePolicyRequestParser                    { get; set; }
        //public CustomJObjectParserDelegate<AddSignaturePolicyResponse>?                                CustomAddSignaturePolicyResponseParser                   { get; set; }
        //public CustomJObjectParserDelegate<AddUserRoleRequest>?                                        CustomAddUserRoleRequestParser                           { get; set; }
        //public CustomJObjectParserDelegate<AddUserRoleResponse>?                                       CustomAddUserRoleResponseParser                          { get; set; }
        //public CustomJObjectParserDelegate<DeleteSignaturePolicyRequest>?                              CustomDeleteSignaturePolicyRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?                             CustomDeleteSignaturePolicyResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<DeleteUserRoleRequest>?                                     CustomDeleteUserRoleRequestParser                        { get; set; }
        //public CustomJObjectParserDelegate<DeleteUserRoleResponse>?                                    CustomDeleteUserRoleResponseParser                       { get; set; }
        //public CustomJObjectParserDelegate<UpdateSignaturePolicyRequest>?                              CustomUpdateSignaturePolicyRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?                             CustomUpdateSignaturePolicyResponseParser                { get; set; }
        //public CustomJObjectParserDelegate<UpdateUserRoleRequest>?                                     CustomUpdateUserRoleRequestParser                        { get; set; }
        //public CustomJObjectParserDelegate<UpdateUserRoleResponse>?                                    CustomUpdateUserRoleResponseParser                       { get; set; }


        // DiagnosticControlExtensions
        public CustomJObjectParserDelegate<AdjustTimeScaleRequest>?                                    CustomAdjustTimeScaleRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<AdjustTimeScaleResponse>?                                   CustomAdjustTimeScaleResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<AttachCableRequest>?                                        CustomAttachCableRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<AttachCableResponse>?                                       CustomAttachCableResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<GetExecutingEnvironmentRequest>?                            CustomGetExecutingEnvironmentRequestParser               { get; set; }
        public CustomJObjectParserDelegate<GetExecutingEnvironmentResponse>?                           CustomGetExecutingEnvironmentResponseParser              { get; set; }
        public CustomJObjectParserDelegate<GetPWMValueRequest>?                                        CustomGetPWMValueRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<GetPWMValueResponse>?                                       CustomGetPWMValueResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<SetCPVoltageRequest>?                                       CustomSetCPVoltageRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<SetCPVoltageResponse>?                                      CustomSetCPVoltageResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<SetErrorStateRequest>?                                      CustomSetErrorStateRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<SetErrorStateResponse>?                                     CustomSetErrorStateResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<SwipeRFIDCardRequest>?                                      CustomSwipeRFIDCardRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<SwipeRFIDCardResponse>?                                     CustomSwipeRFIDCardResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<TimeTravelRequest>?                                         CustomTimeTravelRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<TimeTravelResponse>?                                        CustomTimeTravelResponseParser                           { get; set; }





        //public CustomJObjectParserDelegate<ChargingStation>?                                           CustomChargingStationParser                              { get; set; }
        public CustomJObjectParserDelegate<Signature>?                                                 CustomSignatureParser                                    { get; set; }
        public CustomBinaryParserDelegate<Signature>?                                                  CustomBinarySignatureParser                              { get; set; }
        public CustomJObjectParserDelegate<CustomData>?                                                CustomCustomDataParser                                   { get; set; }
        //public CustomJObjectParserDelegate<StatusInfo>?                                                CustomStatusInfoParser                                   { get; set; }
        //public CustomJObjectParserDelegate<CompositeSchedule>?                                         CustomCompositeScheduleParser                            { get; set; }
        public CustomJObjectParserDelegate<ChargingSchedulePeriod>?                                    CustomChargingSchedulePeriodParser                       { get; set; }
        //public CustomJObjectParserDelegate<ClearMonitoringResult>?                                     CustomClearMonitoringResultParser                        { get; set; }


        //public CustomJObjectParserDelegate<GetVariableResult>?                                         CustomGetVariableResultParser                            { get; set; }
        //public CustomJObjectParserDelegate<Component>?                                                 CustomComponentParser                                    { get; set; }
        //public CustomJObjectParserDelegate<EVSE>?                                                      CustomEVSEParser                                         { get; set; }
        //public CustomJObjectParserDelegate<Variable>?                                                  CustomVariableParser                                     { get; set; }
        //public CustomJObjectParserDelegate<SetMonitoringResult>?                                       CustomSetMonitoringResultParser                          { get; set; }

        public CustomJObjectParserDelegate<CertificateHashData>?                                       CustomCertificateHashDataParser                          { get; set; }

        //public CustomJObjectParserDelegate<IdTokenInfo>?                                               CustomIdTokenInfoParser                                  { get; set; }
        public CustomJObjectParserDelegate<IdToken>?                                                   CustomIdTokenParser                                      { get; set; }
        //public CustomJObjectParserDelegate<AdditionalInfo>?                                            CustomAdditionalInfoParser                               { get; set; }
        //public CustomJObjectParserDelegate<MessageContent>?                                            CustomMessageContentParser                               { get; set; }


        //public CustomJObjectParserDelegate<TariffAssignment>?                                          CustomTariffAssignmentParser                             { get; set; }
        //public CustomJObjectParserDelegate<ClearTariffsResult>?                                        CustomClearTariffsResultParser                           { get; set; }


        // Overlay Network Extensions
        //public CustomJObjectParserDelegate<NotifyNetworkTopologyMessage>?                              CustomNotifyNetworkTopologyMessageParser                 { get; set; }
        //public CustomJObjectParserDelegate<NotifyNetworkTopologyRequest>?                              CustomNotifyNetworkTopologyRequestParser                 { get; set; }
        //public CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?                             CustomNotifyNetworkTopologyResponseParser                { get; set; }

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


        //public Boolean TryGetComponentConfig(String Name, [NotNullWhen(true)] out List<ComponentConfig>? Components)
        //{

        //    if (componentConfigs.TryGetValue(Name, out Components))
        //        return true;

        //    return false;

        //}

        //public List<ComponentConfig> AddOrUpdateComponentConfig(String                                                      Name,
        //                                                        Func<String, List<ComponentConfig>>                         AddValueFactory,
        //                                                        Func<String, List<ComponentConfig>, List<ComponentConfig>>  UpdateValueFactory)

        //    => componentConfigs.AddOrUpdate(
        //           Name,
        //           AddValueFactory,
        //           UpdateValueFactory
        //       );



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

                    sendRequestState2.JSONRequestErrorMessage =  new OCPP_JSONRequestErrorMessage(

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
