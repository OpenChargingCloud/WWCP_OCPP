﻿/*
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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP Adapter.
    /// </summary>
    public class OCPPAdapter
    {

        #region Data

        private readonly        ConcurrentDictionary<NetworkingNode_Id, List<Reachability>>  reachableNetworkingNodes        = [];
        private readonly        ConcurrentDictionary<Request_Id, SendRequestState>           requests                        = [];
        private readonly        HashSet<SignaturePolicy>                                     signaturePolicies               = [];
        private readonly        HashSet<SignaturePolicy>                                     forwardingSignaturePolicies     = [];
        private                 Int64                                                        internalRequestId               = 800000;

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public static readonly  TimeSpan                                                     DefaultSendHeartbeatsEvery      = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The default request timeout default.
        /// </summary>
        public static readonly  TimeSpan                                                     DefaultRequestTimeoutDefault    = TimeSpan.FromSeconds(30);

        public const            String                                                       X_OCPP_NetworkingMode           = "X-OCPP-NetworkingMode";
        public const            String                                                       NetworkingNodeId_WebSocketKey   = "NetworkingNodeId";
        public const            String                                                       NetworkingMode_WebSocketKey     = "NetworkingMode";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the networking node hosting this OCPP adapter.
        /// </summary>
        public NetworkingNode_Id            Id                       { get; }

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

        #region ForwardingSignaturePolicy/-ies

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  ForwardingSignaturePolicies
            => forwardingSignaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               ForwardingSignaturePolicy
            => forwardingSignaturePolicies.First();

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region Charging Station Request  Messages
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<            DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }

        #endregion

        #region Charging Station Response Messages

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<            DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.QRCodeScannedResponse>?                               CustomQRCodeScannedResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }


        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDefaultChargingTariffResponse>?                    CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region CSMS Request  Messages
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ResetRequest>?                                   CustomResetRequestSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UpdateFirmwareRequest>?                          CustomUpdateFirmwareRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PublishFirmwareRequest>?                         CustomPublishFirmwareRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UnpublishFirmwareRequest>?                       CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetBaseReportRequest>?                           CustomGetBaseReportRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetReportRequest>?                               CustomGetReportRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetLogRequest>?                                  CustomGetLogRequestSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetVariablesRequest>?                            CustomSetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetVariablesRequest>?                            CustomGetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetMonitoringBaseRequest>?                       CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetMonitoringReportRequest>?                     CustomGetMonitoringReportRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetMonitoringLevelRequest>?                      CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetVariableMonitoringRequest>?                   CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearVariableMonitoringRequest>?                 CustomClearVariableMonitoringRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetNetworkProfileRequest>?                       CustomSetNetworkProfileRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ChangeAvailabilityRequest>?                      CustomChangeAvailabilityRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.TriggerMessageRequest>?                          CustomTriggerMessageRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<              DataTransferRequest>?                            CustomIncomingDataTransferRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CertificateSignedRequest>?                       CustomCertificateSignedRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.InstallCertificateRequest>?                      CustomInstallCertificateRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest>?              CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.DeleteCertificateRequest>?                       CustomDeleteCertificateRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCRLRequest>?                               CustomNotifyCRLRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetLocalListVersionRequest>?                     CustomGetLocalListVersionRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SendLocalListRequest>?                           CustomSendLocalListRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearCacheRequest>?                              CustomClearCacheRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReserveNowRequest>?                              CustomReserveNowRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CancelReservationRequest>?                       CustomCancelReservationRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RequestStartTransactionRequest>?                 CustomRequestStartTransactionRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RequestStopTransactionRequest>?                  CustomRequestStopTransactionRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetTransactionStatusRequest>?                    CustomGetTransactionStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetChargingProfileRequest>?                      CustomSetChargingProfileRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetChargingProfilesRequest>?                     CustomGetChargingProfilesRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearChargingProfileRequest>?                    CustomClearChargingProfileRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCompositeScheduleRequest>?                    CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UpdateDynamicScheduleRequest>?                   CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest>?             CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.QRCodeScannedRequest>?                           CustomQRCodeScannedRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetDefaultChargingTariffRequest>?                CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetDefaultChargingTariffRequest>?                CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest>?             CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<              DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifySettlementResponse>?                       CustomNotifySettlementResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion



        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                   CustomDeleteFileRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<GetFileRequest>?                                      CustomGetFileRequestSerializer                               { get; set; }
        public CustomBinarySerializerDelegate <GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
        public CustomBinarySerializerDelegate <SendFileRequest>?                                     CustomSendFileRequestSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                CustomListDirectoryRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryResponse>?                               CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomBinarySerializerDelegate <SecureDataTransferRequest>?                           CustomSecureDataTransferRequestSerializer                    { get; set; }
        public CustomBinarySerializerDelegate <SecureDataTransferResponse>?                          CustomSecureDataTransferResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }


        // Overlay Network Extensions
        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?                        CustomNotifyNetworkTopologyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?                       CustomNotifyNetworkTopologyResponseSerializer                { get; set; }


        #region Data Structures

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
        public CustomJObjectSerializerDelegate<SignaturePolicy>?                                     CustomSignaturePolicySerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }



        public CustomJObjectSerializerDelegate<OCPPv2_1.ChargingStation>?                            CustomChargingStationSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                              { get; set; }

        public CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                                  { get; set; }

        public CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<Signature>?                                            CustomBinarySignatureSerializer                              { get; set; }


        // E2E Security Extensions



        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer                   { get; set; }

        #endregion

        #endregion

        #region Custom JSON parser delegates

        #region CS

        #region Certificates
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  Get15118EVCertificateRequest>?                CustomGet15118EVCertificateRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.Get15118EVCertificateResponse>?               CustomGet15118EVCertificateResponseParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCertificateStatusRequest>?                 CustomGetCertificateStatusRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCertificateStatusResponse>?                CustomGetCertificateStatusResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCRLRequest>?                               CustomGetCRLRequestParser                                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCRLResponse>?                              CustomGetCRLResponseParser                               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SignCertificateRequest>?                      CustomSignCertificateRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SignCertificateResponse>?                     CustomSignCertificateResponseParser                      { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  AuthorizeRequest>?                            CustomAuthorizeRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.AuthorizeResponse>?                           CustomAuthorizeResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearedChargingLimitRequest>?                 CustomClearedChargingLimitRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearedChargingLimitResponse>?                CustomClearedChargingLimitResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  MeterValuesRequest>?                          CustomMeterValuesRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.MeterValuesResponse>?                         CustomMeterValuesResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyChargingLimitRequest>?                  CustomNotifyChargingLimitRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyChargingLimitResponse>?                 CustomNotifyChargingLimitResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEVChargingNeedsRequest>?                CustomNotifyEVChargingNeedsRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>?               CustomNotifyEVChargingNeedsResponseParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEVChargingScheduleRequest>?             CustomNotifyEVChargingScheduleRequestParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>?            CustomNotifyEVChargingScheduleResponseParser             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyPriorityChargingRequest>?               CustomNotifyPriorityChargingRequestParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>?              CustomNotifyPriorityChargingResponseParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifySettlementRequest>?                     CustomNotifySettlementRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifySettlementResponse>?                    CustomNotifySettlementResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  PullDynamicScheduleUpdateRequest>?            CustomPullDynamicScheduleUpdateRequestParser             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?           CustomPullDynamicScheduleUpdateResponseParser            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ReportChargingProfilesRequest>?               CustomReportChargingProfilesRequestParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReportChargingProfilesResponse>?              CustomReportChargingProfilesResponseParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ReservationStatusUpdateRequest>?              CustomReservationStatusUpdateRequestParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>?             CustomReservationStatusUpdateResponseParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  StatusNotificationRequest>?                   CustomStatusNotificationRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.StatusNotificationResponse>?                  CustomStatusNotificationResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  TransactionEventRequest>?                     CustomTransactionEventRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.TransactionEventResponse>?                    CustomTransactionEventResponseParser                     { get; set; }

        #endregion

        #region Customer
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyCustomerInformationRequest>?            CustomNotifyCustomerInformationRequestParser             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?           CustomNotifyCustomerInformationResponseParser            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyDisplayMessagesRequest>?                CustomNotifyDisplayMessagesRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?               CustomNotifyDisplayMessagesResponseParser                { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  LogStatusNotificationRequest>?                CustomLogStatusNotificationRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.LogStatusNotificationResponse>?               CustomLogStatusNotificationResponseParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyEventRequest>?                          CustomNotifyEventRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyEventResponse>?                         CustomNotifyEventResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyMonitoringReportRequest>?               CustomNotifyMonitoringReportRequestParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>?              CustomNotifyMonitoringReportResponseParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyReportRequest>?                         CustomNotifyReportRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyReportResponse>?                        CustomNotifyReportResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SecurityEventNotificationRequest>?            CustomSecurityEventNotificationRequestParser             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SecurityEventNotificationResponse>?           CustomSecurityEventNotificationResponseParser            { get; set; }

        #endregion

        #region Firmware

        public CustomJObjectParserDelegate<OCPPv2_1.CS.  BootNotificationRequest>?                     CustomBootNotificationRequestParser                      { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.BootNotificationResponse>?                    CustomBootNotificationResponseParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  FirmwareStatusNotificationRequest>?           CustomFirmwareStatusNotificationRequestParser            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>?          CustomFirmwareStatusNotificationResponseParser           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  HeartbeatRequest>?                            CustomHeartbeatRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.HeartbeatResponse>?                           CustomHeartbeatResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  PublishFirmwareStatusNotificationRequest>?    CustomPublishFirmwareStatusNotificationRequestParser     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>?   CustomPublishFirmwareStatusNotificationResponseParser    { get; set; }

        #endregion

        #endregion

        #region CSMS

        #region Certificates

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.CertificateSignedRequest>?                    CustomCertificateSignedRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  CertificateSignedResponse>?                   CustomCertificateSignedResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.DeleteCertificateRequest>?                    CustomDeleteCertificateRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  DeleteCertificateResponse>?                   CustomDeleteCertificateResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest>?           CustomGetInstalledCertificateIdsRequestParser            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetInstalledCertificateIdsResponse>?          CustomGetInstalledCertificateIdsResponseParser           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.InstallCertificateRequest>?                   CustomInstallCertificateRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  InstallCertificateResponse>?                  CustomInstallCertificateResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyCRLRequest>?                            CustomNotifyCRLRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyCRLResponse>?                           CustomNotifyCRLResponseParser                            { get; set; }

        #endregion

        #region Charging
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.CancelReservationRequest>?                    CustomCancelReservationRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  CancelReservationResponse>?                   CustomCancelReservationResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearChargingProfileRequest>?                 CustomClearChargingProfileRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearChargingProfileResponse>?                CustomClearChargingProfileResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetChargingProfilesRequest>?                  CustomGetChargingProfilesRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetChargingProfilesResponse>?                 CustomGetChargingProfilesResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetCompositeScheduleRequest>?                 CustomGetCompositeScheduleRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetCompositeScheduleResponse>?                CustomGetCompositeScheduleResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetTransactionStatusRequest>?                 CustomGetTransactionStatusRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetTransactionStatusResponse>?                CustomGetTransactionStatusResponseParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest>?          CustomNotifyAllowedEnergyTransferRequestParser           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  NotifyAllowedEnergyTransferResponse>?         CustomNotifyAllowedEnergyTransferResponseParser          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.QRCodeScannedRequest>?                        CustomQRCodeScannedRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  QRCodeScannedResponse>?                       CustomQRCodeScannedResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.RequestStartTransactionRequest>?              CustomRequestStartTransactionRequestParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  RequestStartTransactionResponse>?             CustomRequestStartTransactionResponseParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.RequestStopTransactionRequest>?               CustomRequestStopTransactionRequestParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  RequestStopTransactionResponse>?              CustomRequestStopTransactionResponseParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ReserveNowRequest>?                           CustomReserveNowRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ReserveNowResponse>?                          CustomReserveNowResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetChargingProfileRequest>?                   CustomSetChargingProfileRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetChargingProfileResponse>?                  CustomSetChargingProfileResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.UnlockConnectorRequest>?                      CustomUnlockConnectorRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  UnlockConnectorResponse>?                     CustomUnlockConnectorResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.UpdateDynamicScheduleRequest>?                CustomUpdateDynamicScheduleRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  UpdateDynamicScheduleResponse>?               CustomUpdateDynamicScheduleResponseParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.UsePriorityChargingRequest>?                  CustomUsePriorityChargingRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  UsePriorityChargingResponse>?                 CustomUsePriorityChargingResponseParser                  { get; set; }

        #endregion

        #region Customer

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearDisplayMessageRequest>?                  CustomClearDisplayMessageRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearDisplayMessageResponse>?                 CustomClearDisplayMessageResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.CostUpdatedRequest>?                          CustomCostUpdatedRequestParser                           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  CostUpdatedResponse>?                         CustomCostUpdatedResponseParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.CustomerInformationRequest>?                  CustomCustomerInformationRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  CustomerInformationResponse>?                 CustomCustomerInformationResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetDisplayMessagesRequest>?                   CustomGetDisplayMessagesRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetDisplayMessagesResponse>?                  CustomGetDisplayMessagesResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetDisplayMessageRequest>?                    CustomSetDisplayMessageRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetDisplayMessageResponse>?                   CustomSetDisplayMessageResponseParser                    { get; set; }

        #endregion

        #region DeviceModel
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ChangeAvailabilityRequest>?                   CustomChangeAvailabilityRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ChangeAvailabilityResponse>?                  CustomChangeAvailabilityResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearVariableMonitoringRequest>?              CustomClearVariableMonitoringRequestParser               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearVariableMonitoringResponse>?             CustomClearVariableMonitoringResponseParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetBaseReportRequest>?                        CustomGetBaseReportRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetBaseReportResponse>?                       CustomGetBaseReportResponseParser                        { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetLogRequest>?                               CustomGetLogRequestParser                                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetLogResponse>?                              CustomGetLogResponseParser                               { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetMonitoringReportRequest>?                  CustomGetMonitoringReportRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetMonitoringReportResponse>?                 CustomGetMonitoringReportResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetReportRequest>?                            CustomGetReportRequestParser                             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetReportResponse>?                           CustomGetReportResponseParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetVariablesRequest>?                         CustomGetVariablesRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetVariablesResponse>?                        CustomGetVariablesResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetMonitoringBaseRequest>?                    CustomSetMonitoringBaseRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetMonitoringBaseResponse>?                   CustomSetMonitoringBaseResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetMonitoringLevelRequest>?                   CustomSetMonitoringLevelRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetMonitoringLevelResponse>?                  CustomSetMonitoringLevelResponseParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetNetworkProfileRequest>?                    CustomSetNetworkProfileRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetNetworkProfileResponse>?                   CustomSetNetworkProfileResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetVariableMonitoringRequest>?                CustomSetVariableMonitoringRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetVariableMonitoringResponse>?               CustomSetVariableMonitoringResponseParser                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetVariablesRequest>?                         CustomSetVariablesRequestParser                          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetVariablesResponse>?                        CustomSetVariablesResponseParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.TriggerMessageRequest>?                       CustomTriggerMessageRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  TriggerMessageResponse>?                      CustomTriggerMessageResponseParser                       { get; set; }

        #endregion

        #region E2EChargingTariffsExtensions

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetDefaultChargingTariffRequest>?             CustomGetDefaultChargingTariffRequestParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetDefaultChargingTariffResponse>?            CustomGetDefaultChargingTariffResponseParser             { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest>?          CustomRemoveDefaultChargingTariffRequestParser           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  RemoveDefaultChargingTariffResponse>?         CustomRemoveDefaultChargingTariffResponseParser          { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SetDefaultChargingTariffRequest>?             CustomSetDefaultChargingTariffRequestParser              { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SetDefaultChargingTariffResponse>?            CustomSetDefaultChargingTariffResponseParser             { get; set; }

        #endregion

        #region Firmware

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.PublishFirmwareRequest>?                      CustomPublishFirmwareRequestParser                       { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  PublishFirmwareResponse>?                     CustomPublishFirmwareResponseParser                      { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ResetRequest>?                                CustomResetRequestParser                                 { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ResetResponse>?                               CustomResetResponseParser                                { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.UnpublishFirmwareRequest>?                    CustomUnpublishFirmwareRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  UnpublishFirmwareResponse>?                   CustomUnpublishFirmwareResponseParser                    { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.UpdateFirmwareRequest>?                       CustomUpdateFirmwareRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  UpdateFirmwareResponse>?                      CustomUpdateFirmwareResponseParser                       { get; set; }

        #endregion

        #region Grid

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.AFRRSignalRequest>?                           CustomAFRRSignalRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  AFRRSignalResponse>?                          CustomAFRRSignalResponseParser                           { get; set; }

        #endregion

        #region LocalList

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.ClearCacheRequest>?                           CustomClearCacheRequestParser                            { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  ClearCacheResponse>?                          CustomClearCacheResponseParser                           { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.GetLocalListVersionRequest>?                  CustomGetLocalListVersionRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  GetLocalListVersionResponse>?                 CustomGetLocalListVersionResponseParser                  { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.SendLocalListRequest>?                        CustomSendLocalListRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<OCPPv2_1.CS.  SendLocalListResponse>?                       CustomSendLocalListResponseParser                        { get; set; }

        #endregion

        #endregion

        public CustomJObjectParserDelegate<DataTransferRequest>?                                       CustomDataTransferRequestParser                              { get; set; }
        public CustomJObjectParserDelegate<DataTransferResponse>?                                      CustomDataTransferResponseParser                             { get; set; }


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


        public CustomJObjectParserDelegate<OCPPv2_1.ChargingStation>?                                  CustomChargingStationParser                              { get; set; }
        public CustomJObjectParserDelegate<Signature>?                                                 CustomSignatureParser                                    { get; set; }
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


        // Overlay Network Extensions
        public CustomJObjectParserDelegate<NotifyNetworkTopologyRequest>?                              CustomNotifyNetworkTopologyRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?                             CustomNotifyNetworkTopologyResponseParser                { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter.
        /// </summary>
        /// <param name="NetworkingNode">The attached networking node.</param>
        /// 
        /// <param name="DisableSendHeartbeats">Whether to send heartbeats or not.</param>
        /// <param name="SendHeartbeatsEvery">The optional time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The optional default request time out.</param>
        /// 
        /// <param name="SignaturePolicy">An optional signature policy.</param>
        /// <param name="ForwardingSignaturePolicy">An optional signature policy when forwarding OCPP messages.</param>
        public OCPPAdapter(INetworkingNode   NetworkingNode,

                           Boolean           DisableSendHeartbeats       = false,
                           TimeSpan?         SendHeartbeatsEvery         = null,

                           TimeSpan?         DefaultRequestTimeout       = null,

                           SignaturePolicy?  SignaturePolicy             = null,
                           SignaturePolicy?  ForwardingSignaturePolicy   = null)

        {

            this.Id                     = NetworkingNode.Id;
            this.DisableSendHeartbeats  = DisableSendHeartbeats;
            this.SendHeartbeatsEvery    = SendHeartbeatsEvery   ?? DefaultSendHeartbeatsEvery;

            this.DefaultRequestTimeout  = DefaultRequestTimeout ?? DefaultRequestTimeoutDefault;

            this.signaturePolicies.          Add(SignaturePolicy           ?? new SignaturePolicy());
            this.forwardingSignaturePolicies.Add(ForwardingSignaturePolicy ?? new SignaturePolicy());

            this.IN       = new OCPPWebSocketAdapterIN     (NetworkingNode);
            this.OUT      = new OCPPWebSocketAdapterOUT    (NetworkingNode);
            this.FORWARD  = new OCPPWebSocketAdapterFORWARD(NetworkingNode);

        }

        #endregion


        #region Send    JSON   messages

        #region (internal) SendJSONRequest          (JSONRequestMessage)

        internal async Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(JSONRequestMessage.DestinationId, out var reachability))
            {

                if      (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendJSONRequest(JSONRequestMessage);

                else if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendJSONRequest(JSONRequestMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendJSONRequestAndWait   (JSONRequestMessage, SendMessageResultDelegate = null)

        internal async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage     JSONRequestMessage,
                                                                     Action<SentMessageResult>?  SendMessageResultDelegate   = null)
        {

            var sendMessageResult = await SendJSONRequest(JSONRequestMessage);

            if (SendMessageResultDelegate is not null)
                SendMessageResultDelegate(sendMessageResult);


            if (sendMessageResult.Result == SentMessageResults.Success)
            {

                #region 1. Store 'in-flight' request...

                requests.TryAdd(JSONRequestMessage.RequestId,
                                SendRequestState.FromJSONRequest(
                                    Timestamp.Now,
                                    JSONRequestMessage.DestinationId,
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
                                                                     JSONRequestMessage.NetworkPath.Source,
                                                                     NetworkPath.From(Id),
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
                       DestinationId:            JSONRequestMessage.DestinationId,
                       Timeout:                  JSONRequestMessage.RequestTimeout,
                       JSONRequest:              JSONRequestMessage,
                       SendMessageResult:        sendMessageResult,
                       ResponseTimestamp:        Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     JSONRequestMessage.NetworkPath.Source,
                                                     NetworkPath.From(Id),
                                                     JSONRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.InternalError

                                                 )

                   );

        }

        #endregion

        #region (internal) SendJSONResponse         (JSONRequestMessage)

        internal async Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(JSONResponseMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendJSONResponse(JSONResponseMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendJSONResponse(JSONResponseMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendJSONRequestError     (JSONRequestErrorMessage)

        internal async Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(JSONRequestErrorMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendJSONRequestError(JSONRequestErrorMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendJSONRequestError(JSONRequestErrorMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendJSONResponseError    (JSONResponseErrorMessage)

        internal async Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(JSONResponseErrorMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendJSONResponseError(JSONResponseErrorMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendJSONResponseError(JSONResponseErrorMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendJSONSendMessage      (JSONRequestMessage)

        internal async Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(JSONSendMessage.DestinationId, out var reachability))
            {

                if      (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendJSONSendMessage(JSONSendMessage);

                else if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendJSONSendMessage(JSONSendMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #endregion

        #region Send    binary messages

        #region (internal) SendBinaryRequest        (BinaryRequestMessage)

        internal async Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(BinaryRequestMessage.DestinationId, out var reachability))
            {

                if      (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendBinaryRequest(BinaryRequestMessage);

                else if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendBinaryRequest(BinaryRequestMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendBinaryRequestAndWait (BinaryRequestMessage, SendMessageResultDelegate = null)

        internal async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage   BinaryRequestMessage,
                                                                       Action<SentMessageResult>?  SendMessageResultDelegate   = null)
        {

            var sendMessageResult = await SendBinaryRequest(BinaryRequestMessage);

            if (SendMessageResultDelegate is not null)
                SendMessageResultDelegate(sendMessageResult);

            if (sendMessageResult.Result == SentMessageResults.Success)
            {

                #region (internal) 1. Store 'in-flight' request...

                requests.TryAdd(BinaryRequestMessage.RequestId,
                                SendRequestState.FromBinaryRequest(
                                    Timestamp.Now,
                                    BinaryRequestMessage.DestinationId,
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
                                                                     BinaryRequestMessage.NetworkPath.Source,
                                                                     NetworkPath.From(Id),
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
                       DestinationId:            BinaryRequestMessage.DestinationId,
                       Timeout:                  BinaryRequestMessage.RequestTimeout,
                       BinaryRequest:            BinaryRequestMessage,
                       SendMessageResult:        sendMessageResult,
                       ResponseTimestamp:        Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     BinaryRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     BinaryRequestMessage.NetworkPath.Source,
                                                     NetworkPath.From(Id),
                                                     BinaryRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.InternalError

                                                 )

                   );

        }

        #endregion

        #region (internal) SendBinaryResponse       (BinaryResponseMessage)

        internal async Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(BinaryResponseMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendBinaryResponse(BinaryResponseMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendBinaryResponse(BinaryResponseMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendBinaryRequestError   (BinaryRequestErrorMessage)

        internal async Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(BinaryRequestErrorMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendBinaryRequestError(BinaryRequestErrorMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendBinaryRequestError(BinaryRequestErrorMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendBinaryResponseError  (BinaryResponseErrorMessage)

        internal async Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(BinaryResponseErrorMessage.DestinationId, out var reachability))
            {

                if (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendBinaryResponseError(BinaryResponseErrorMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendBinaryResponseError(BinaryResponseErrorMessage);

            }

            return sendMessageResult;

        }

        #endregion

        #region (internal) SendBinarySendMessage    (BinarySendMessage)

        internal async Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)
        {

            var sendMessageResult = SentMessageResult.UnknownClient();

            if (LookupNetworkingNode(BinarySendMessage.DestinationId, out var reachability))
            {

                if      (reachability.OCPPWebSocketClient is not null)
                    sendMessageResult = await reachability.OCPPWebSocketClient.SendBinarySendMessage(BinarySendMessage);

                else if (reachability.OCPPWebSocketServer is not null)
                    sendMessageResult = await reachability.OCPPWebSocketServer.SendBinarySendMessage(BinarySendMessage);

            }

            return sendMessageResult;

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
                sendRequestState.DestinationIdReceived        = JSONResponseMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = JSONResponseMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{JSONResponseMessage.RequestId}' within {Id}:{Environment.NewLine}'{JSONResponseMessage.Payload.ToString(Formatting.None)}'!");
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
                sendRequestState.DestinationIdReceived        = JSONRequestErrorMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = JSONRequestErrorMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP JSON request error message with identificaiton '{JSONRequestErrorMessage.RequestId}' within {Id}:{Environment.NewLine}'{JSONRequestErrorMessage.ToJSON().ToString(Formatting.None)}'!");
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
                sendRequestState.DestinationIdReceived        = JSONResponseErrorMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = JSONResponseErrorMessage.NetworkPath;

                //ToDo: This has to be forwarded actively, as it is not expected (async)!

                return true;

            }

            DebugX.Log($"Received an unknown OCPP JSON response error message with identificaiton '{JSONResponseErrorMessage.RequestId}' within {Id}:{Environment.NewLine}'{JSONResponseErrorMessage.ToJSON().ToString(Formatting.None)}'!");
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
                sendRequestState.DestinationIdReceived        = BinaryResponseMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = BinaryResponseMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{BinaryResponseMessage.RequestId}' within {Id}:{Environment.NewLine}'{BinaryResponseMessage.Payload.ToBase64()}'!");
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
                sendRequestState.DestinationIdReceived        = BinaryRequestErrorMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = BinaryRequestErrorMessage.NetworkPath;

                return true;

            }

            DebugX.Log($"Received an unknown OCPP Binary request error message with identificaiton '{BinaryRequestErrorMessage.RequestId}' within {Id}:{Environment.NewLine}'{BinaryRequestErrorMessage.ToByteArray().ToBase64()}'!");
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
                sendRequestState.DestinationIdReceived        = BinaryResponseErrorMessage.DestinationId;
                sendRequestState.NetworkPathReceived          = BinaryResponseErrorMessage.NetworkPath;

                //ToDo: This has to be forwarded actively, as it is not expected (async)!

                return true;

            }

            DebugX.Log($"Received an unknown OCPP Binary response error message with identificaiton '{BinaryResponseErrorMessage.RequestId}' within {Id}:{Environment.NewLine}'{BinaryResponseErrorMessage.ToByteArray().ToBase64()}'!");
            return false;

        }

        #endregion

        #endregion


        #region LookupNetworkingNode (DestinationId, out Reachability)

        public Boolean LookupNetworkingNode(NetworkingNode_Id                      DestinationId,
                                            [NotNullWhen(true)] out Reachability?  Reachability)
        {

            if (reachableNetworkingNodes.TryGetValue(DestinationId, out var reachabilityList) &&
                reachabilityList is not null &&
                reachabilityList.Count > 0)
            {

                var reachability = reachabilityList.OrderBy(entry => entry.Priority).First();

                if (reachability.NetworkingHub.HasValue)
                {

                    var visitedIds = new HashSet<NetworkingNode_Id>();

                    do
                    {

                        if (reachability.NetworkingHub.HasValue)
                        {

                            visitedIds.Add(reachability.NetworkingHub.Value);

                            if (reachableNetworkingNodes.TryGetValue(reachability.NetworkingHub.Value, out var reachability2List) &&
                                reachability2List is not null &&
                                reachability2List.Count > 0)
                            {
                                reachability = reachability2List.OrderBy(entry => entry.Priority).First();
                            }

                            // Loop detected!
                            if (reachability.NetworkingHub.HasValue && visitedIds.Contains(reachability.NetworkingHub.Value))
                                break;

                        }

                    } while (reachability.OCPPWebSocketClient is null &&
                             reachability.OCPPWebSocketServer is null);

                }

                Reachability = reachability;
                return true;

            }

            Reachability = null;
            return false;

        }

        #endregion


        #region AddStaticRouting     (DestinationId,  WebSocketClient,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id     DestinationId,
                                     IOCPPWebSocketClient  WebSocketClient,
                                     Byte?                 Priority    = 0,
                                     DateTime?             Timestamp   = null,
                                     DateTime?             Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationId,
                                   WebSocketClient,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationId,

                (id)                   => [ reachability ],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [ reachability ];

                    else
                    {

                        // For thread-safety!
                        var updatedReachabilityList = new List<Reachability>();
                        updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                        updatedReachabilityList.Add     (reachability);

                        return updatedReachabilityList;

                    }

                }

            );

        }

        #endregion

        #region AddStaticRouting     (DestinationIds, WebSocketClient,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                     IOCPPWebSocketClient            WebSocketClient,
                                     Byte?                           Priority    = 0,
                                     DateTime?                       Timestamp   = null,
                                     DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddStaticRouting(
                    destinationId,
                    WebSocketClient,
                    Priority,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region AddStaticRouting     (DestinationId,  WebSocketServer,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id     DestinationId,
                                     IOCPPWebSocketServer  WebSocketServer,
                                     Byte?                 Priority    = 0,
                                     DateTime?             Timestamp   = null,
                                     DateTime?             Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationId,
                                   WebSocketServer,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationId,

                (id)                   => [ reachability ],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [ reachability ];

                    else
                    {

                        // For thread-safety!
                        var updatedReachabilityList = new List<Reachability>();
                        updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                        updatedReachabilityList.Add     (reachability);

                        return updatedReachabilityList;

                    }

                }

            );

        }

        #endregion

        #region AddStaticRouting     (DestinationIds, WebSocketClient,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                     IOCPPWebSocketServer            WebSocketServer,
                                     Byte?                           Priority    = 0,
                                     DateTime?                       Timestamp   = null,
                                     DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddStaticRouting(
                    destinationId,
                    WebSocketServer,
                    Priority,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region AddStaticRouting     (DestinationId,  NetworkingHubId,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id  DestinationId,
                                     NetworkingNode_Id  NetworkingHubId,
                                     Byte?              Priority    = 0,
                                     DateTime?          Timestamp   = null,
                                     DateTime?          Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationId,
                                   NetworkingHubId,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationId,

                (id)                   => [ reachability ],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [ reachability ];

                    else
                    {

                        // For thread-safety!
                        var updatedReachabilityList = new List<Reachability>();
                        updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                        updatedReachabilityList.Add     (reachability);

                        return updatedReachabilityList;

                    }

                }

            );

        }

        #endregion

        #region AddStaticRouting     (DestinationIds, NetworkingHubId,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                     NetworkingNode_Id               NetworkingHubId,
                                     Byte?                           Priority    = 0,
                                     DateTime?                       Timestamp   = null,
                                     DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddStaticRouting(
                    destinationId,
                    NetworkingHubId,
                    Priority,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region RemoveStaticRouting  (DestinationId,  NetworkingHubId = null, Priority = 0)

        public void RemoveStaticRouting(NetworkingNode_Id   DestinationId,
                                        NetworkingNode_Id?  NetworkingHubId   = null,
                                        Byte?               Priority          = 0)
        {

            if (!NetworkingHubId.HasValue)
            {
                reachableNetworkingNodes.TryRemove(DestinationId, out _);
                return;
            }

            if (reachableNetworkingNodes.TryGetValue(DestinationId, out var reachabilityList) &&
                reachabilityList is not null &&
                reachabilityList.Count > 0)
            {

                // For thread-safety!
                var updatedReachabilityList = new List<Reachability>(reachabilityList.Where(entry => entry.NetworkingHub == NetworkingHubId && (!Priority.HasValue || entry.Priority != (Priority ?? 0))));

                if (updatedReachabilityList.Count > 0)
                    reachableNetworkingNodes.TryUpdate(
                        DestinationId,
                        updatedReachabilityList,
                        reachabilityList
                    );

                else
                    reachableNetworkingNodes.TryRemove(DestinationId, out _);

            }

            //csmsChannel.Item1.RemoveStaticRouting(DestinationId,
            //                                      NetworkingHubId);

        }

        #endregion


    }

}
