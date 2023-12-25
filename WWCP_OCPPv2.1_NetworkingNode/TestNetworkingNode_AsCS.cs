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

using System.Reflection;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A networking node for testing.
    /// </summary>
    public partial class TestNetworkingNode : INetworkingNode,
                                              IEventSender
    {

        public class ActingAsCS : IEventSender
        {

            #region Data

            private readonly            TestNetworkingNode          parentNetworkingNode;

            /// <summary>
            /// The default time span between heartbeat requests.
            /// </summary>
            public readonly             TimeSpan                    DefaultSendHeartbeatEvery   = TimeSpan.FromSeconds(30);

            protected static readonly   TimeSpan                    SemaphoreSlimTimeout        = TimeSpan.FromSeconds(5);

            /// <summary>
            /// The default maintenance interval.
            /// </summary>
            public readonly             TimeSpan                    DefaultMaintenanceEvery     = TimeSpan.FromSeconds(1);
            private static readonly     SemaphoreSlim               MaintenanceSemaphore        = new (1, 1);
            private readonly            Timer                       MaintenanceTimer;

            private readonly            Timer                       SendHeartbeatTimer;


            private readonly            List<EnqueuedRequest>       EnqueuedRequests;

            public                      IHTTPAuthentication?        HTTPAuthentication          { get; }

            #endregion

            #region Properties

            /// <summary>
            /// The client connected to a CSMS.
            /// </summary>
            public INetworkingNodeOutgoingMessages?   CSClient                    { get; private set; }


            public String? ClientCloseMessage
                => "-";


            /// <summary>
            /// The sender identification.
            /// </summary>
            String IEventSender.Id
                => parentNetworkingNode.Id.ToString();


            /// <summary>
            /// The charging station vendor identification.
            /// </summary>
            [Mandatory]
            public String                   VendorName                  { get; }

            /// <summary>
            ///  The charging station model identification.
            /// </summary>
            [Mandatory]
            public String                   Model                       { get; }


            /// <summary>
            /// The optional multi-language charging station description.
            /// </summary>
            [Optional]
            public I18NString?              Description                 { get; }

            /// <summary>
            /// The optional serial number of the charging station.
            /// </summary>
            [Optional]
            public String?                  SerialNumber                { get; }

            /// <summary>
            /// The optional firmware version of the charging station.
            /// </summary>
            [Optional]
            public String?                  FirmwareVersion             { get; }

            /// <summary>
            /// The modem of the charging station.
            /// </summary>
            [Optional]
            public Modem?                   Modem                       { get; }

            /// <summary>
            /// The optional meter type of the main power meter of the charging station.
            /// </summary>
            [Optional]


            public CustomData?              CustomData                  { get; set; }


            /// <summary>
            /// The time span between heartbeat requests.
            /// </summary>
            public TimeSpan                 SendHeartbeatEvery          { get; set; }

            /// <summary>
            /// The time at the CSMS.
            /// </summary>
            public DateTime?                CSMSTime                    { get; private set; }

            /// <summary>
            /// The default request timeout for all requests.
            /// </summary>
            public TimeSpan                 DefaultRequestTimeout       { get; }




            /// <summary>
            /// The maintenance interval.
            /// </summary>
            public TimeSpan                 MaintenanceEvery            { get; }

            /// <summary>
            /// Disable all maintenance tasks.
            /// </summary>
            public Boolean                  DisableMaintenanceTasks     { get; set; }

            /// <summary>
            /// Disable all heartbeats.
            /// </summary>
            public Boolean                  DisableSendHeartbeats       { get; set; }


            #region ToDo's

            public URL RemoteURL => throw new NotImplementedException();

            public HTTPHostname? VirtualHostname => throw new NotImplementedException();

   //         string? IHTTPClient.Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public RemoteCertificateValidationHandler? RemoteCertificateValidator => throw new NotImplementedException();

            public X509Certificate? ClientCert => throw new NotImplementedException();

            public SslProtocols TLSProtocol => throw new NotImplementedException();

            public bool PreferIPv4 => throw new NotImplementedException();

            public string HTTPUserAgent => throw new NotImplementedException();

            public TimeSpan RequestTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public TransmissionRetryDelayDelegate TransmissionRetryDelay => throw new NotImplementedException();

            public ushort MaxNumberOfRetries => throw new NotImplementedException();

            public bool UseHTTPPipelining => throw new NotImplementedException();

            public HTTPClientLogger? HTTPLogger => throw new NotImplementedException();

            #endregion

            #endregion

            #region Custom JSON serializer delegates

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
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


            // Binary Data Streams Extensions
            public CustomBinarySerializerDelegate <              BinaryDataTransferRequest>?                      CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }
            public CustomJObjectSerializerDelegate<              GetFileRequest>?                                 CustomGetFileRequestSerializer                               { get; set; }
            public CustomBinarySerializerDelegate <              SendFileRequest>?                                CustomSendFileRequestSerializer                              { get; set; }
            public CustomJObjectSerializerDelegate<              DeleteFileRequest>?                              CustomDeleteFileRequestSerializer                            { get; set; }


            // E2E Security Extensions
            public CustomJObjectSerializerDelegate<              AddSignaturePolicyRequest>?                      CustomAddSignaturePolicyRequestSerializer                    { get; set; }
            public CustomJObjectSerializerDelegate<              UpdateSignaturePolicyRequest>?                   CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
            public CustomJObjectSerializerDelegate<              DeleteSignaturePolicyRequest>?                   CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
            public CustomJObjectSerializerDelegate<              AddUserRoleRequest>?                             CustomAddUserRoleRequestSerializer                           { get; set; }
            public CustomJObjectSerializerDelegate<              UpdateUserRoleRequest>?                          CustomUpdateUserRoleRequestSerializer                        { get; set; }
            public CustomJObjectSerializerDelegate<              DeleteUserRoleRequest>?                          CustomDeleteUserRoleRequestSerializer                        { get; set; }


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
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }


            // Binary Data Streams Extensions
            public CustomBinarySerializerDelegate <              BinaryDataTransferResponse>?                     CustomBinaryDataTransferResponseSerializer                   { get; set; }

            #endregion


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
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }


            // Binary Data Streams Extensions
            public CustomBinarySerializerDelegate <            BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }

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
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }


            // Binary Data Streams Extensions
            public CustomBinarySerializerDelegate <            BinaryDataTransferResponse>?                          CustomIncomingBinaryDataTransferResponseSerializer           { get; set; }
            public CustomBinarySerializerDelegate <            GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
            public CustomJObjectSerializerDelegate<            SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
            public CustomJObjectSerializerDelegate<            DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }


            // E2E Security Extensions
            public CustomJObjectSerializerDelegate<            AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
            public CustomJObjectSerializerDelegate<            UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
            public CustomJObjectSerializerDelegate<            DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
            public CustomJObjectSerializerDelegate<            AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
            public CustomJObjectSerializerDelegate<            UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }
            public CustomJObjectSerializerDelegate<            DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }


            // E2E Charging Tariff Extensions
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SetDefaultChargingTariffResponse>?                    CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
            public CustomJObjectSerializerDelegate<OCPPv2_1.CS.RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

            #endregion

            public CustomJObjectSerializerDelegate<OCPP.NN.NotifyNetworkTopologyRequest>?                CustomNotifyNetworkTopologyRequestSerializer           { get; set; }
            public CustomJObjectSerializerDelegate<OCPP.NN.NotifyNetworkTopologyResponse>?               CustomNotifyNetworkTopologyResponseSerializer          { get; set; }


            #region Data Structures

            public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
            public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
            public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
            public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                                    { get; set; }
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



            public CustomJObjectSerializerDelegate<ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
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
            public CustomBinarySerializerDelegate<OCPP.Signature>?                                       CustomBinarySignatureSerializer                              { get; set; }


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

            #endregion

            public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer             { get; set; }

            #endregion

            #region Events

            #region Charging Station -> CSMS

            //#region SendBootNotification

            /// <summary>
            /// An event fired whenever a BootNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnBootNotificationRequestDelegate? OnBootNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a BootNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnBootNotificationResponseDelegate? OnBootNotificationResponse;

            //#endregion

            //#region SendFirmwareStatusNotification

            /// <summary>
            /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnFirmwareStatusNotificationRequestDelegate? OnFirmwareStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a FirmwareStatusNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnFirmwareStatusNotificationResponseDelegate? OnFirmwareStatusNotificationResponse;

            //#endregion

            //#region SendPublishFirmwareStatusNotification

            /// <summary>
            /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationRequestDelegate? OnPublishFirmwareStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationResponseDelegate? OnPublishFirmwareStatusNotificationResponse;

            //#endregion

            //#region SendHeartbeat

            /// <summary>
            /// An event fired whenever a Heartbeat request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnHeartbeatRequestDelegate? OnHeartbeatRequest;

            /// <summary>
            /// An event fired whenever a response to a Heartbeat request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnHeartbeatResponseDelegate? OnHeartbeatResponse;

            //#endregion

            //#region NotifyEvent

            /// <summary>
            /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEventRequestDelegate? OnNotifyEventRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEvent request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEventResponseDelegate? OnNotifyEventResponse;

            //#endregion

            //#region SendSecurityEventNotification

            /// <summary>
            /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnSecurityEventNotificationRequestDelegate? OnSecurityEventNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a SecurityEventNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnSecurityEventNotificationResponseDelegate? OnSecurityEventNotificationResponse;

            //#endregion

            //#region NotifyReport

            /// <summary>
            /// An event fired whenever a NotifyReport request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyReportRequestDelegate? OnNotifyReportRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyReport request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyReportResponseDelegate? OnNotifyReportResponse;

            //#endregion

            //#region NotifyMonitoringReport

            /// <summary>
            /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyMonitoringReportRequestDelegate? OnNotifyMonitoringReportRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyMonitoringReport request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyMonitoringReportResponseDelegate? OnNotifyMonitoringReportResponse;

            //#endregion

            //#region SendLogStatusNotification

            /// <summary>
            /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnLogStatusNotificationRequestDelegate? OnLogStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a LogStatusNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnLogStatusNotificationResponseDelegate? OnLogStatusNotificationResponse;

            //#endregion

            //#region TransferData

            /// <summary>
            /// An event fired whenever a DataTransfer request will be sent to the CSMS.
            /// </summary>
            //public event OnDataTransferRequestDelegate? OnDataTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a DataTransfer request was received.
            /// </summary>
            //public event OnDataTransferResponseDelegate? OnDataTransferResponse;

            //#endregion


            //#region SignCertificate

            /// <summary>
            /// An event fired whenever a SignCertificate request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnSignCertificateRequestDelegate? OnSignCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a SignCertificate request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnSignCertificateResponseDelegate? OnSignCertificateResponse;

            //#endregion

            //#region Get15118EVCertificate

            /// <summary>
            /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnGet15118EVCertificateRequestDelegate? OnGet15118EVCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a Get15118EVCertificate request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnGet15118EVCertificateResponseDelegate? OnGet15118EVCertificateResponse;

            //#endregion

            //#region GetCertificateStatus

            /// <summary>
            /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnGetCertificateStatusRequestDelegate? OnGetCertificateStatusRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCertificateStatus request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnGetCertificateStatusResponseDelegate? OnGetCertificateStatusResponse;

            //#endregion

            //#region GetCRL

            /// <summary>
            /// An event fired whenever a GetCRL request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnGetCRLRequestDelegate? OnGetCRLRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCRL request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnGetCRLResponseDelegate? OnGetCRLResponse;

            //#endregion


            //#region SendReservationStatusUpdate

            /// <summary>
            /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnReservationStatusUpdateRequestDelegate? OnReservationStatusUpdateRequest;

            /// <summary>
            /// An event fired whenever a response to a ReservationStatusUpdate request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnReservationStatusUpdateResponseDelegate? OnReservationStatusUpdateResponse;

            //#endregion

            //#region Authorize

            /// <summary>
            /// An event fired whenever an Authorize request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnAuthorizeRequestDelegate? OnAuthorizeRequest;

            /// <summary>
            /// An event fired whenever a response to an Authorize request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnAuthorizeResponseDelegate? OnAuthorizeResponse;

            //#endregion

            //#region NotifyEVChargingNeeds

            /// <summary>
            /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEVChargingNeedsRequestDelegate? OnNotifyEVChargingNeedsRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseDelegate? OnNotifyEVChargingNeedsResponse;

            //#endregion

            //#region SendTransactionEvent

            /// <summary>
            /// An event fired whenever a TransactionEvent will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnTransactionEventRequestDelegate? OnTransactionEventRequest;

            /// <summary>
            /// An event fired whenever a response to a TransactionEvent request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnTransactionEventResponseDelegate? OnTransactionEventResponse;

            //#endregion

            //#region SendStatusNotification

            /// <summary>
            /// An event fired whenever a StatusNotification request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnStatusNotificationRequestDelegate? OnStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a StatusNotification request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnStatusNotificationResponseDelegate? OnStatusNotificationResponse;

            //#endregion

            //#region SendMeterValues

            /// <summary>
            /// An event fired whenever a MeterValues request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnMeterValuesRequestDelegate? OnMeterValuesRequest;

            /// <summary>
            /// An event fired whenever a response to a MeterValues request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnMeterValuesResponseDelegate? OnMeterValuesResponse;

            //#endregion

            //#region NotifyChargingLimit

            /// <summary>
            /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyChargingLimitRequestDelegate? OnNotifyChargingLimitRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyChargingLimit request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyChargingLimitResponseDelegate? OnNotifyChargingLimitResponse;

            //#endregion

            //#region SendClearedChargingLimit

            /// <summary>
            /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnClearedChargingLimitRequestDelegate? OnClearedChargingLimitRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearedChargingLimit request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnClearedChargingLimitResponseDelegate? OnClearedChargingLimitResponse;

            //#endregion

            //#region ReportChargingProfiles

            /// <summary>
            /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnReportChargingProfilesRequestDelegate? OnReportChargingProfilesRequest;

            /// <summary>
            /// An event fired whenever a response to a ReportChargingProfiles request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnReportChargingProfilesResponseDelegate? OnReportChargingProfilesResponse;

            //#endregion

            //#region NotifyEVChargingSchedule

            /// <summary>
            /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEVChargingScheduleRequestDelegate? OnNotifyEVChargingScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyEVChargingScheduleResponseDelegate? OnNotifyEVChargingScheduleResponse;

            //#endregion

            //#region NotifyPriorityCharging

            /// <summary>
            /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyPriorityChargingRequestDelegate? OnNotifyPriorityChargingRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyPriorityCharging request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyPriorityChargingResponseDelegate? OnNotifyPriorityChargingResponse;

            //#endregion

            //#region PullDynamicScheduleUpdate

            /// <summary>
            /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateRequestDelegate? OnPullDynamicScheduleUpdateRequest;

            /// <summary>
            /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateResponseDelegate? OnPullDynamicScheduleUpdateResponse;

            //#endregion


            //#region NotifyDisplayMessages

            /// <summary>
            /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyDisplayMessagesRequestDelegate? OnNotifyDisplayMessagesRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyDisplayMessages request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseDelegate? OnNotifyDisplayMessagesResponse;

            //#endregion

            //#region NotifyCustomerInformation

            /// <summary>
            /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyCustomerInformationRequestDelegate? OnNotifyCustomerInformationRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyCustomerInformation request was received.
            /// </summary>
            //public event OCPPv2_1.CS.OnNotifyCustomerInformationResponseDelegate? OnNotifyCustomerInformationResponse;

            //#endregion


            //Binary Data Streams Extensions

            //#region TransferBinaryData

            /// <summary>
            /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
            /// </summary>
            //public event OnBinaryDataTransferRequestDelegate? OnBinaryDataTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a BinaryDataTransfer request was received.
            /// </summary>
            //public event OnBinaryDataTransferResponseDelegate? OnBinaryDataTransferResponse;

            //#endregion


            //Overlay Networking Extensions

            //#region OnNotifyNetworkTopology (Request/-Response)

            /// <summary>
            /// An event fired whenever a NotifyNetworkTopology request will be sent to another node.
            /// </summary>
            //public event OCPP.CS.OnNotifyNetworkTopologyRequestDelegate? OnNotifyNetworkTopologyRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyNetworkTopology request was received.
            /// </summary>
            //public event OCPP.CS.OnNotifyNetworkTopologyResponseDelegate? OnNotifyNetworkTopologyResponse;

            //#endregion

            #endregion

            #region Charging Station <- CSMS

            ////ToDo: Are those events really required here?
            //public event OCPPv2_1.CS.OnUpdateFirmwareDelegate?                OnUpdateFirmware;
            //public event OCPPv2_1.CS.OnPublishFirmwareDelegate?               OnPublishFirmware;
            //public event OCPPv2_1.CS.OnUnpublishFirmwareDelegate?             OnUnpublishFirmware;
            //public event OCPPv2_1.CS.OnGetBaseReportDelegate?                 OnGetBaseReport;
            //public event OCPPv2_1.CS.OnGetReportDelegate?                     OnGetReport;
            //public event OCPPv2_1.CS.OnGetLogDelegate?                        OnGetLog;
            //public event OCPPv2_1.CS.OnSetVariablesDelegate?                  OnSetVariables;
            //public event OCPPv2_1.CS.OnGetVariablesDelegate?                  OnGetVariables;
            //public event OCPPv2_1.CS.OnSetMonitoringBaseDelegate?             OnSetMonitoringBase;
            //public event OCPPv2_1.CS.OnGetMonitoringReportDelegate?           OnGetMonitoringReport;
            //public event OCPPv2_1.CS.OnSetMonitoringLevelDelegate?            OnSetMonitoringLevel;
            //public event OCPPv2_1.CS.OnSetVariableMonitoringDelegate?         OnSetVariableMonitoring;
            //public event OCPPv2_1.CS.OnClearVariableMonitoringDelegate?       OnClearVariableMonitoring;
            //public event OCPPv2_1.CS.OnSetNetworkProfileDelegate?             OnSetNetworkProfile;
            //public event OCPPv2_1.CS.OnChangeAvailabilityDelegate?            OnChangeAvailability;
            //public event OCPPv2_1.CS.OnTriggerMessageDelegate?                OnTriggerMessage;
            //public event             OnIncomingDataTransferDelegate?          OnIncomingDataTransfer;

            //public event OCPPv2_1.CS.OnCertificateSignedDelegate?             OnCertificateSigned;
            //public event OCPPv2_1.CS.OnInstallCertificateDelegate?            OnInstallCertificate;
            //public event OCPPv2_1.CS.OnGetInstalledCertificateIdsDelegate?    OnGetInstalledCertificateIds;
            //public event OCPPv2_1.CS.OnDeleteCertificateDelegate?             OnDeleteCertificate;
            //public event OCPPv2_1.CS.OnNotifyCRLDelegate?                     OnNotifyCRL;

            //public event OCPPv2_1.CS.OnGetLocalListVersionDelegate?           OnGetLocalListVersion;
            //public event OCPPv2_1.CS.OnSendLocalListDelegate?                 OnSendLocalList;
            //public event OCPPv2_1.CS.OnClearCacheDelegate?                    OnClearCache;

            //public event OCPPv2_1.CS.OnReserveNowDelegate?                    OnReserveNow;
            //public event OCPPv2_1.CS.OnCancelReservationDelegate?             OnCancelReservation;
            //public event OCPPv2_1.CS.OnRequestStartTransactionDelegate?       OnRequestStartTransaction;
            //public event OCPPv2_1.CS.OnRequestStopTransactionDelegate?        OnRequestStopTransaction;
            //public event OCPPv2_1.CS.OnGetTransactionStatusDelegate?          OnGetTransactionStatus;
            //public event OCPPv2_1.CS.OnSetChargingProfileDelegate?            OnSetChargingProfile;
            //public event OCPPv2_1.CS.OnGetChargingProfilesDelegate?           OnGetChargingProfiles;
            //public event OCPPv2_1.CS.OnClearChargingProfileDelegate?          OnClearChargingProfile;
            //public event OCPPv2_1.CS.OnGetCompositeScheduleDelegate?          OnGetCompositeSchedule;
            //public event OCPPv2_1.CS.OnUpdateDynamicScheduleDelegate?         OnUpdateDynamicSchedule;
            //public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferDelegate?   OnNotifyAllowedEnergyTransfer;
            //public event OCPPv2_1.CS.OnUsePriorityChargingDelegate?           OnUsePriorityCharging;
            //public event OCPPv2_1.CS.OnUnlockConnectorDelegate?               OnUnlockConnector;

            //public event OCPPv2_1.CS.OnAFRRSignalDelegate?                    OnAFRRSignal;

            //public event OCPPv2_1.CS.OnSetDisplayMessageDelegate?             OnSetDisplayMessage;
            //public event OCPPv2_1.CS.OnGetDisplayMessagesDelegate?            OnGetDisplayMessages;
            //public event OCPPv2_1.CS.OnClearDisplayMessageDelegate?           OnClearDisplayMessage;
            //public event OCPPv2_1.CS.OnCostUpdatedDelegate?                   OnCostUpdated;
            //public event OCPPv2_1.CS.OnCustomerInformationDelegate?           OnCustomerInformation;

            //// Binary Data Streams Extensions
            //public event             OnIncomingBinaryDataTransferDelegate?    OnIncomingBinaryDataTransfer;
            //public event             OnGetFileDelegate?                       OnGetFile;
            //public event             OnSendFileDelegate?                      OnSendFile;
            //public event             OnDeleteFileDelegate?                    OnDeleteFile;

            //// E2E Security Extensions
            //public event             OnAddSignaturePolicyDelegate?            OnAddSignaturePolicy;
            //public event             OnUpdateSignaturePolicyDelegate?         OnUpdateSignaturePolicy;
            //public event             OnDeleteSignaturePolicyDelegate?         OnDeleteSignaturePolicy;
            //public event             OnAddUserRoleDelegate?                   OnAddUserRole;
            //public event             OnUpdateUserRoleDelegate?                OnUpdateUserRole;
            //public event             OnDeleteUserRoleDelegate?                OnDeleteUserRole;


            #region UpdateFirmware

            ///// <summary>
            ///// An event fired whenever an UpdateFirmware request was received from the CSMS.
            ///// </summary>
            //public event OCPPv2_1.CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

            ///// <summary>
            ///// An event fired whenever a response to an UpdateFirmware request was sent.
            ///// </summary>
            //public event OCPPv2_1.CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

            #endregion

            #region PublishFirmware

            /// <summary>
            /// An event fired whenever a PublishFirmware request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to a PublishFirmware request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

            #endregion

            #region UnpublishFirmware

            /// <summary>
            /// An event fired whenever an UnpublishFirmware request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to an UnpublishFirmware request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

            #endregion

            #region GetBaseReport

            /// <summary>
            /// An event fired whenever a GetBaseReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetBaseReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

            #endregion

            #region GetReport

            /// <summary>
            /// An event fired whenever a GetReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetReportRequestDelegate?   OnGetReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetReportResponseDelegate?  OnGetReportResponse;

            #endregion

            #region GetLog

            /// <summary>
            /// An event fired whenever a GetLog request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLogRequestDelegate?   OnGetLogRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLog request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLogResponseDelegate?  OnGetLogResponse;

            #endregion

            #region SetVariables

            /// <summary>
            /// An event fired whenever a SetVariables request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariables request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

            #endregion

            #region GetVariables

            /// <summary>
            /// An event fired whenever a GetVariables request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetVariables request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

            #endregion

            #region SetMonitoringBase

            /// <summary>
            /// An event fired whenever a SetMonitoringBase request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringBase request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

            #endregion

            #region GetMonitoringReport

            /// <summary>
            /// An event fired whenever a GetMonitoringReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetMonitoringReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

            #endregion

            #region SetMonitoringLevel

            /// <summary>
            /// An event fired whenever a SetMonitoringLevel request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringLevel request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

            #endregion

            #region SetVariableMonitoring

            /// <summary>
            /// An event fired whenever a SetVariableMonitoring request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariableMonitoring request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

            #endregion

            #region ClearVariableMonitoring

            /// <summary>
            /// An event fired whenever a ClearVariableMonitoring request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearVariableMonitoring request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

            #endregion

            #region SetNetworkProfile

            /// <summary>
            /// An event fired whenever a SetNetworkProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetNetworkProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

            #endregion

            #region ChangeAvailability

            /// <summary>
            /// An event fired whenever a ChangeAvailability request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

            /// <summary>
            /// An event fired whenever a response to a ChangeAvailability request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

            #endregion

            #region TriggerMessage

            /// <summary>
            /// An event fired whenever a TriggerMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a TriggerMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

            #endregion

            #region OnIncomingDataTransferRequest/-Response

            /// <summary>
            /// An event sent whenever a data transfer request was sent.
            /// </summary>
            public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

            /// <summary>
            /// An event sent whenever a response to a data transfer request was sent.
            /// </summary>
            public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

            #endregion


            #region SendSignedCertificate

            /// <summary>
            /// An event fired whenever a SignedCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

            /// <summary>
            /// An event fired whenever a response to a SignedCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

            #endregion

            #region InstallCertificate

            /// <summary>
            /// An event fired whenever an InstallCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to an InstallCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

            #endregion

            #region GetInstalledCertificateIds

            /// <summary>
            /// An event fired whenever a GetInstalledCertificateIds request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

            /// <summary>
            /// An event fired whenever a response to a GetInstalledCertificateIds request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

            #endregion

            #region DeleteCertificate

            /// <summary>
            /// An event fired whenever a DeleteCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

            #endregion

            #region NotifyCRL

            /// <summary>
            /// An event fired whenever a NotifyCRL request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyCRL request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

            #endregion


            #region GetLocalListVersion

            /// <summary>
            /// An event fired whenever a GetLocalListVersion request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLocalListVersion request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

            #endregion

            #region SendLocalList

            /// <summary>
            /// An event fired whenever a SendLocalList request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

            /// <summary>
            /// An event fired whenever a response to a SendLocalList request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

            #endregion

            #region ClearCache

            /// <summary>
            /// An event fired whenever a ClearCache request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearCache request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

            #endregion


            #region ReserveNow

            /// <summary>
            /// An event fired whenever a ReserveNow request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

            /// <summary>
            /// An event fired whenever a response to a ReserveNow request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

            #endregion

            #region CancelReservation

            /// <summary>
            /// An event fired whenever a CancelReservation request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

            /// <summary>
            /// An event fired whenever a response to a CancelReservation request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

            #endregion

            #region StartCharging

            /// <summary>
            /// An event fired whenever a RequestStartTransaction request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStartTransaction request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

            #endregion

            #region StopCharging

            /// <summary>
            /// An event fired whenever a RequestStopTransaction request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStopTransaction request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

            #endregion

            #region GetTransactionStatus

            /// <summary>
            /// An event fired whenever a GetTransactionStatus request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

            /// <summary>
            /// An event fired whenever a response to a GetTransactionStatus request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

            #endregion

            #region SetChargingProfile

            /// <summary>
            /// An event fired whenever a SetChargingProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetChargingProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

            #endregion

            #region GetChargingProfiles

            /// <summary>
            /// An event fired whenever a GetChargingProfiles request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetChargingProfiles request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

            #endregion

            #region ClearChargingProfile

            /// <summary>
            /// An event fired whenever a ClearChargingProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearChargingProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

            #endregion

            #region GetCompositeSchedule

            /// <summary>
            /// An event fired whenever a GetCompositeSchedule request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCompositeSchedule request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

            #endregion

            #region UpdateDynamicSchedule

            /// <summary>
            /// An event fired whenever an UpdateDynamicSchedule request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to an UpdateDynamicSchedule request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

            #endregion

            #region NotifyAllowedEnergyTransfer

            /// <summary>
            /// An event fired whenever a NotifyAllowedEnergyTransfer request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

            #endregion

            #region UsePriorityCharging

            /// <summary>
            /// An event fired whenever a UsePriorityCharging request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

            /// <summary>
            /// An event fired whenever a response to a UsePriorityCharging request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

            #endregion

            #region UnlockConnector

            /// <summary>
            /// An event fired whenever an UnlockConnector request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

            /// <summary>
            /// An event fired whenever a response to an UnlockConnector request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

            #endregion


            #region AFRRSignal

            /// <summary>
            /// An event fired whenever an AFRR signal request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

            /// <summary>
            /// An event fired whenever a response to an AFRR signal request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

            #endregion


            #region SetDisplayMessage

            /// <summary>
            /// An event fired whenever a SetDisplayMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a SetDisplayMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

            #endregion

            #region GetDisplayMessages

            /// <summary>
            /// An event fired whenever a GetDisplayMessages request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetDisplayMessages request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

            #endregion

            #region ClearDisplayMessage

            /// <summary>
            /// An event fired whenever a ClearDisplayMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearDisplayMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

            #endregion

            #region SendCostUpdated

            /// <summary>
            /// An event fired whenever a CostUpdated request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

            /// <summary>
            /// An event fired whenever a response to a CostUpdated request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

            #endregion

            #region RequestCustomerInformation

            /// <summary>
            /// An event fired whenever a CustomerInformation request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

            /// <summary>
            /// An event fired whenever a response to a CustomerInformation request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

            #endregion


            // Binary Data Streams Extensions

            #region OnIncomingBinaryDataTransferRequest/-Response

            /// <summary>
            /// An event sent whenever a BinaryDataTransfer request was sent.
            /// </summary>
            public event OnIncomingBinaryDataTransferRequestDelegate?   OnIncomingBinaryDataTransferRequest;

            /// <summary>
            /// An event sent whenever a response to a BinaryDataTransfer request was sent.
            /// </summary>
            public event OnIncomingBinaryDataTransferResponseDelegate?  OnIncomingBinaryDataTransferResponse;

            #endregion

            #region OnGetFileRequest/-Response

            /// <summary>
            /// An event sent whenever a GetFile request was sent.
            /// </summary>
            public event OCPP.CS.OnGetFileRequestDelegate?   OnGetFileRequest;

            /// <summary>
            /// An event sent whenever a response to a GetFile request was sent.
            /// </summary>
            public event OCPP.CS.OnGetFileResponseDelegate?  OnGetFileResponse;

            #endregion

            #region OnSendFileRequest/-Response

            /// <summary>
            /// An event sent whenever a SendFile request was sent.
            /// </summary>
            public event OCPP.CS.OnSendFileRequestDelegate?   OnSendFileRequest;

            /// <summary>
            /// An event sent whenever a response to a SendFile request was sent.
            /// </summary>
            public event OCPP.CS.OnSendFileResponseDelegate?  OnSendFileResponse;

            #endregion

            #region OnDeleteFileRequest/-Response

            /// <summary>
            /// An event sent whenever a DeleteFile request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

            /// <summary>
            /// An event sent whenever a response to a DeleteFile request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

            #endregion


            // E2E Security Extensions

            #region AddSignaturePolicy

            /// <summary>
            /// An event fired whenever a AddSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a AddSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

            #endregion

            #region UpdateSignaturePolicy

            /// <summary>
            /// An event fired whenever a UpdateSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

            #endregion

            #region DeleteSignaturePolicy

            /// <summary>
            /// An event fired whenever a DeleteSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

            #endregion

            #region AddUserRole

            /// <summary>
            /// An event fired whenever a AddUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a AddUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

            #endregion

            #region UpdateUserRole

            /// <summary>
            /// An event fired whenever a UpdateUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

            #endregion

            #region DeleteUserRole

            /// <summary>
            /// An event fired whenever a DeleteUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

            #endregion


            // E2E Charging Tariffs Extensions

            #region SetDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a SetDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

            public event OCPPv2_1.CS.OnSetDefaultChargingTariffDelegate?          OnSetDefaultChargingTariff;

            /// <summary>
            /// An event fired whenever a response to a SetDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

            #endregion

            #region GetDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a GetDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

            public event OCPPv2_1.CS.OnGetDefaultChargingTariffDelegate?          OnGetDefaultChargingTariff;

            /// <summary>
            /// An event fired whenever a response to a GetDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

            #endregion

            #region RemoveDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a RemoveDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffDelegate?          OnRemoveDefaultChargingTariff;

            /// <summary>
            /// An event fired whenever a response to a RemoveDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

            #endregion

            #endregion

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new charging station for testing.
            /// </summary>
            /// <param name="Id">The charging station identification.</param>
            /// <param name="VendorName">The charging station vendor identification.</param>
            /// <param name="Model">The charging station model identification.</param>
            /// 
            /// <param name="Description">An optional multi-language charging station description.</param>
            /// <param name="SerialNumber">An optional serial number of the charging station.</param>
            /// <param name="FirmwareVersion">An optional firmware version of the charging station.</param>
            /// <param name="MeterType">An optional meter type of the main power meter of the charging station.</param>
            /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charging station.</param>
            /// <param name="MeterPublicKey">An optional public key of the main power meter of the charging station.</param>
            /// 
            /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
            /// 
            /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
            public ActingAsCS(TestNetworkingNode                 NetworkingNode,
                              String                             VendorName,
                              String                             Model,

                              I18NString?                        Description               = null,
                              String?                            SerialNumber              = null,
                              String?                            FirmwareVersion           = null,
                              Modem?                             Modem                     = null,

                              Boolean                            DisableSendHeartbeats     = false,
                              TimeSpan?                          SendHeartbeatEvery        = null,

                              Boolean                            DisableMaintenanceTasks   = false,
                              TimeSpan?                          MaintenanceEvery          = null,

                              TimeSpan?                          DefaultRequestTimeout     = null,
                              IHTTPAuthentication?               HTTPAuthentication        = null,
                              DNSClient?                         DNSClient                 = null,

                              SignaturePolicy?                   SignaturePolicy           = null)

            {

                if (VendorName.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(VendorName),  "The given charging station vendor must not be null or empty!");

                if (Model.     IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(Model),       "The given charging station model must not be null or empty!");


                this.parentNetworkingNode     = NetworkingNode;

                //this.Configuration = new Dictionary<String, ConfigurationData> {
                //    { "hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false) },
                //    { "changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false) },
                //    { "doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false) },
                //    { "password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false) }
                //};
                this.EnqueuedRequests         = new List<EnqueuedRequest>();

                this.VendorName               = VendorName;
                this.Model                    = Model;
                this.Description              = Description;
                this.SerialNumber             = SerialNumber;
                this.FirmwareVersion          = FirmwareVersion;
                this.Modem                    = Modem;

                this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

                this.DisableSendHeartbeats    = DisableSendHeartbeats;
                this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;
                this.SendHeartbeatTimer       = new Timer(
                                                    DoSendHeartbeatSync,
                                                    null,
                                                    this.SendHeartbeatEvery,
                                                    this.SendHeartbeatEvery
                                                );

                this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
                this.MaintenanceEvery         = MaintenanceEvery      ?? DefaultMaintenanceEvery;
                //this.MaintenanceTimer         = new Timer(
                //                                    DoMaintenanceSync,
                //                                    null,
                //                                    this.MaintenanceEvery,
                //                                    this.MaintenanceEvery
                //                                );

                this.HTTPAuthentication       = HTTPAuthentication;

            }

            #endregion


            #region HandleErrors(Module, Caller, ExceptionOccured)

            private Task HandleErrors(String     Module,
                                      String     Caller,
                                      Exception  ExceptionOccured)
            {

                DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

                return Task.CompletedTask;

            }

            #endregion


            #region ConnectWebSocket(...)

            public async Task<HTTPResponse?> ConnectWebSocket(String                               From,
                                                              String                               To,

                                                              URL                                  RemoteURL,
                                                              HTTPHostname?                        VirtualHostname              = null,
                                                              String?                              Description                  = null,
                                                              RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                                              LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                              X509Certificate?                     ClientCert                   = null,
                                                              SslProtocols?                        TLSProtocol                  = null,
                                                              Boolean?                             PreferIPv4                   = null,
                                                              String?                              HTTPUserAgent                = null,
                                                              IHTTPAuthentication?                 HTTPAuthentication           = null,
                                                              TimeSpan?                            RequestTimeout               = null,
                                                              TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                                              UInt16?                              MaxNumberOfRetries           = null,
                                                              UInt32?                              InternalBufferSize           = null,

                                                              IEnumerable<String>?                 SecWebSocketProtocols        = null,
                                                              NetworkingMode?                      NetworkingMode               = null,

                                                              Boolean                              DisableMaintenanceTasks      = false,
                                                              TimeSpan?                            MaintenanceEvery             = null,
                                                              Boolean                              DisableWebSocketPings        = false,
                                                              TimeSpan?                            WebSocketPingEvery           = null,
                                                              TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                              String?                              LoggingPath                  = null,
                                                              String?                              LoggingContext               = null,
                                                              LogfileCreatorDelegate?              LogfileCreator               = null,
                                                              HTTPClientLogger?                    HTTPLogger                   = null,
                                                              DNSClient?                           DNSClient                    = null)

            {

                var networkingNodeWSClient = new NetworkingNodeWSClient(
                                                 parentNetworkingNode.Id,
                                                 From,
                                                 To,

                                                 RemoteURL,
                                                 VirtualHostname,
                                                 Description,
                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 ClientCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 HTTPAuthentication ?? this.HTTPAuthentication,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,

                                                 SecWebSocketProtocols ?? new[] { Version.WebSocketSubProtocolId },
                                                 NetworkingMode        ?? OCPP.WebSockets.NetworkingMode.NetworkingExtensions,

                                                 DisableWebSocketPings,
                                                 WebSocketPingEvery,
                                                 SlowNetworkSimulationDelay,

                                                 DisableMaintenanceTasks,
                                                 MaintenanceEvery,

                                                 LoggingPath,
                                                 LoggingContext,
                                                 LogfileCreator,
                                                 HTTPLogger,
                                                 DNSClient ?? parentNetworkingNode.DNSClient
                                             );

                this.CSClient  = networkingNodeWSClient;

                WireEvents(networkingNodeWSClient);

                var response = await networkingNodeWSClient.Connect();

                return response;

            }

            #endregion

            #region WireEvents(IncomingMessages)


            private readonly ConcurrentDictionary<DisplayMessage_Id,     MessageInfo>     displayMessages   = new ();
            private readonly ConcurrentDictionary<Reservation_Id,        Reservation_Id>  reservations      = new ();
            private readonly ConcurrentDictionary<Transaction_Id,        Transaction>     transactions      = new ();
            private readonly ConcurrentDictionary<Transaction_Id,        Decimal>         totalCosts        = new ();
            private readonly ConcurrentDictionary<InstallCertificateUse, Certificate>     certificates      = new ();

            public void WireEvents(INetworkingNodeIncomingMessages IncomingMessages)
            {

                #region OnReset

                IncomingMessages.OnReset += async (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   cancellationToken) => {

                    #region Send OnResetRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnResetRequest(startTime,
                                                                      this,
                                                                      connection,
                                                                      request);

                    #endregion


                    #region Check request signature(s)

                    OCPPv2_1.CS.ResetResponse? response = null;

                    if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomResetRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new OCPPv2_1.CS.ResetResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                    #endregion

                    else if (request.DestinationNodeId != parentNetworkingNode.Id)
                    {

                        DebugX.Log($"Forwarding incoming '{request.ResetType}' reset request to '{request.DestinationNodeId}'!");

                        response = await parentNetworkingNode.AsCSMS.Reset(request);

                    }

                    else
                    {

                        var me_or_not_me = request.DestinationNodeId;

                        DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming '{request.ResetType}' reset request{(request.EVSEId.HasValue ? $" for EVSE '{request.EVSEId}" : "")}'!");

                        // ResetType

                        // Reset entire charging station
                        if (!request.EVSEId.HasValue)
                        {

                            response = new OCPPv2_1.CS.ResetResponse(
                                           Request:      request,
                                           Status:       ResetStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                        // Unknown EVSE
                        else
                        {

                            response = new OCPPv2_1.CS.ResetResponse(
                                           Request:      request,
                                           Status:       ResetStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomResetResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnResetResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnResetResponse(responseTime,
                                                                       this,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateFirmware

                IncomingMessages.OnUpdateFirmware += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnUpdateFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateFirmwareRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UpdateFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomUpdateFirmwareRequestSerializer,
                                     CustomFirmwareSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateFirmware request ({request.UpdateFirmwareRequestId}) for '" + request.Firmware.FirmwareURL + "'.");

                            // Firmware,
                            // UpdateFirmwareRequestId
                            // Retries
                            // RetryIntervals

                            response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                                           Request:      request,
                                           Status:       UpdateFirmwareStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUpdateFirmwareResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateFirmwareResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);


                    #endregion

                    return response;

                };

                #endregion

                #region OnPublishFirmware

                IncomingMessages.OnPublishFirmware += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                    #region Send OnPublishFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnPublishFirmwareRequest(startTime,
                                                                                this,
                                                                                connection,
                                                                                request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.PublishFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.PublishFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid PublishFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomPublishFirmwareRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.PublishFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming PublishFirmware request ({request.PublishFirmwareRequestId}) for '" + request.DownloadLocation + "'.");

                            // PublishFirmwareRequestId
                            // DownloadLocation
                            // MD5Checksum
                            // Retries
                            // RetryInterval

                            response = new OCPPv2_1.CS.PublishFirmwareResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomPublishFirmwareResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnPublishFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnPublishFirmwareResponse(responseTime,
                                                                                 this,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUnpublishFirmware

                IncomingMessages.OnUnpublishFirmware += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnUnpublishFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnpublishFirmwareRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UnpublishFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UnpublishFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomUnpublishFirmwareRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UnpublishFirmware request for '" + request.MD5Checksum + "'.");

                            // MD5Checksum

                            response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                                           Request:      request,
                                           Status:       UnpublishFirmwareStatus.Unpublished,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUnpublishFirmwareResponseSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUnpublishFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnpublishFirmwareResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetBaseReport

                IncomingMessages.OnGetBaseReport += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                    #region Send OnGetBaseReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetBaseReportRequest(startTime,
                                                                              this,
                                                                              connection,
                                                                              request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetBaseReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetBaseReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetBaseReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetBaseReportRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetBaseReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetBaseReport request ({request.GetBaseReportRequestId}) accepted.");

                            // GetBaseReportRequestId
                            // ReportBase

                            response = new OCPPv2_1.CS.GetBaseReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetBaseReportResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetBaseReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetBaseReportResponse(responseTime,
                                                                               this,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetReport

                IncomingMessages.OnGetReport += async (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) => {

                    #region Send OnGetReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetReportRequest(startTime,
                                                                          this,
                                                                          connection,
                                                                          request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetReportRequestSerializer,
                                     CustomComponentVariableSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomVariableSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetReport request ({request.GetReportRequestId}) accepted.");

                            // GetReportRequestId
                            // ComponentCriteria
                            // ComponentVariables

                            response = new OCPPv2_1.CS.GetReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetReportResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetReportResponse(responseTime,
                                                                           this,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetLog

                IncomingMessages.OnGetLog += async (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                    #region Send OnGetLogRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLogRequest(startTime,
                                                                       this,
                                                                       connection,
                                                                       request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetLogResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetLogResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetLog request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetLogRequestSerializer,
                                     CustomLogParametersSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetLogResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetLog request ({request.LogRequestId}) accepted.");

                            // LogType
                            // LogRequestId
                            // Log
                            // Retries
                            // RetryInterval

                            response = new OCPPv2_1.CS.GetLogResponse(
                                           Request:      request,
                                           Status:       LogStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetLogResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetLogResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLogResponse(responseTime,
                                                                        this,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetVariables

                IncomingMessages.OnSetVariables += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                    #region Send OnSetVariablesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariablesRequest(startTime,
                                                                             this,
                                                                             connection,
                                                                             request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetVariablesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetVariablesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetVariables request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetVariablesRequestSerializer,
                                     CustomSetVariableDataSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomVariableSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetVariablesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetVariables request accepted.");

                            // VariableData

                            response = new OCPPv2_1.CS.SetVariablesResponse(
                                           Request:              request,
                                           SetVariableResults:   request.VariableData.Select(variableData => new SetVariableResult(
                                                                                                                 Status:                SetVariableStatus.Accepted,
                                                                                                                 Component:             variableData.Component,
                                                                                                                 Variable:              variableData.Variable,
                                                                                                                 AttributeType:         variableData.AttributeType,
                                                                                                                 AttributeStatusInfo:   null,
                                                                                                                 CustomData:            null
                                                                                                             )),
                                           CustomData:           null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetVariablesResponseSerializer,
                            CustomSetVariableResultSerializer,
                            CustomComponentSerializer,
                            CustomEVSESerializer,
                            CustomVariableSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetVariablesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariablesResponse(responseTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetVariables

                IncomingMessages.OnGetVariables += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                    #region Send OnGetVariablesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetVariablesRequest(startTime,
                                                                             this,
                                                                             connection,
                                                                             request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetVariablesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetVariablesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetVariables request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetVariablesRequestSerializer,
                                     CustomGetVariableDataSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomVariableSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetVariablesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetVariables request accepted.");

                            // VariableData

                            response = new OCPPv2_1.CS.GetVariablesResponse(
                                           Request:      request,
                                           Results:      request.VariableData.Select(variableData => new GetVariableResult(
                                                                                                         AttributeStatus:       GetVariableStatus.Accepted,
                                                                                                         Component:             variableData.Component,
                                                                                                         Variable:              variableData.Variable,
                                                                                                         AttributeValue:        "",
                                                                                                         AttributeType:         variableData.AttributeType,
                                                                                                         AttributeStatusInfo:   null,
                                                                                                         CustomData:            null
                                                                                                     )),
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetVariablesResponseSerializer,
                            CustomGetVariableResultSerializer,
                            CustomComponentSerializer,
                            CustomEVSESerializer,
                            CustomVariableSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetVariablesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetVariablesResponse(responseTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetMonitoringBase

                IncomingMessages.OnSetMonitoringBase += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetMonitoringBaseRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringBaseRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetMonitoringBaseResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetMonitoringBase request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetMonitoringBaseRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringBase request accepted.");

                            // MonitoringBase

                            response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetMonitoringBaseResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetMonitoringBaseResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringBaseResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetMonitoringReport

                IncomingMessages.OnGetMonitoringReport += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetMonitoringReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetMonitoringReportRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetMonitoringReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetMonitoringReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetMonitoringReportRequestSerializer,
                                     CustomComponentVariableSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomVariableSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetMonitoringReport request ({request.GetMonitoringReportRequestId}) accepted.");

                            // GetMonitoringReportRequestId
                            // MonitoringCriteria
                            // ComponentVariables

                            response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetMonitoringReportResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetMonitoringReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetMonitoringReportResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetMonitoringLevel

                IncomingMessages.OnSetMonitoringLevel += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnSetMonitoringLevelRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringLevelRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetMonitoringLevelResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetMonitoringLevel request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetMonitoringLevelRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringLevel request accepted.");

                            // Severity

                            response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetMonitoringLevelResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetMonitoringLevelResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringLevelResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetVariableMonitoring

                IncomingMessages.OnSetVariableMonitoring += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnSetVariableMonitoringRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariableMonitoringRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetVariableMonitoringResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetVariableMonitoringRequestSerializer,
                                     CustomSetMonitoringDataSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomVariableSerializer,
                                     CustomPeriodicEventStreamParametersSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringLevel request accepted.");

                            // MonitoringData

                            response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                                           Request:                request,
                                           SetMonitoringResults:   request.MonitoringData.Select(setMonitoringData => new SetMonitoringResult(
                                                                                                                          Status:                 SetMonitoringStatus.Accepted,
                                                                                                                          MonitorType:            setMonitoringData.MonitorType,
                                                                                                                          Severity:               setMonitoringData.Severity,
                                                                                                                          Component:              setMonitoringData.Component,
                                                                                                                          Variable:               setMonitoringData.Variable,
                                                                                                                          VariableMonitoringId:   setMonitoringData.VariableMonitoringId,
                                                                                                                          StatusInfo:             null,
                                                                                                                          CustomData:             null
                                                                                                                      )),
                                           CustomData:             null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetVariableMonitoringResponseSerializer,
                            CustomSetMonitoringResultSerializer,
                            CustomComponentSerializer,
                            CustomEVSESerializer,
                            CustomVariableSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetVariableMonitoringResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariableMonitoringResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearVariableMonitoring

                IncomingMessages.OnClearVariableMonitoring += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) => {

                    #region Send OnClearVariableMonitoringRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearVariableMonitoringRequest(startTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearVariableMonitoringResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomClearVariableMonitoringRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearVariableMonitoring request (VariableMonitoringIds: {request.VariableMonitoringIds.AggregateWith(", ")})");

                            // VariableMonitoringIds

                            response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                                           Request:                  request,
                                           ClearMonitoringResults:   request.VariableMonitoringIds.Select(variableMonitoringId => new ClearMonitoringResult(
                                                                                                                                      Status:       ClearMonitoringStatus.Accepted,
                                                                                                                                      Id:           variableMonitoringId,
                                                                                                                                      StatusInfo:   null,
                                                                                                                                      CustomData:   null
                                                                                                                                  )),
                                           CustomData:               null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomClearVariableMonitoringResponseSerializer,
                            CustomClearMonitoringResultSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearVariableMonitoringResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearVariableMonitoringResponse(responseTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request,
                                                                                         response,
                                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetNetworkProfile

                IncomingMessages.OnSetNetworkProfile += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetNetworkProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetNetworkProfileRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetNetworkProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetNetworkProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetNetworkProfileRequestSerializer,
                                     CustomNetworkConnectionProfileSerializer,
                                     CustomVPNConfigurationSerializer,
                                     CustomAPNConfigurationSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetNetworkProfile request for configuration slot {request.ConfigurationSlot}!");

                            // ConfigurationSlot
                            // NetworkConnectionProfile

                            response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                                           Request:      request,
                                           Status:       SetNetworkProfileStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetNetworkProfileResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetNetworkProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetNetworkProfileResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnChangeAvailability

                IncomingMessages.OnChangeAvailability += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnChangeAvailabilityRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnChangeAvailabilityRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ChangeAvailabilityResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ChangeAvailabilityResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ChangeAvailability request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomChangeAvailabilityRequestSerializer,
                                     CustomEVSESerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ChangeAvailability request {request.OperationalStatus.AsText()}{(request.EVSE is not null ? $" for EVSE '{request.EVSE.Id}'{(request.EVSE.ConnectorId.HasValue ? $"/{request.EVSE.ConnectorId}" : "")}" : "")}!");

                            // OperationalStatus
                            // EVSE
 
                            response = request.EVSE is null

                                           ? new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                                 Request:      request,
                                                 Status:       ChangeAvailabilityStatus.Accepted,
                                                 CustomData:   null
                                             )

                                           : new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                                 Request:      request,
                                                 Status:       ChangeAvailabilityStatus.Rejected,
                                                 CustomData:   null
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomChangeAvailabilityResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnChangeAvailabilityResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnChangeAvailabilityResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnTriggerMessage

                IncomingMessages.OnTriggerMessage += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnTriggerMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnTriggerMessageRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.TriggerMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.TriggerMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid TriggerMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomTriggerMessageRequestSerializer,
                                     CustomEVSESerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.TriggerMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming TriggerMessage request for '{request.RequestedMessage}'{(request.EVSE is not null ? $" at EVSE '{request.EVSE.Id}'" : "")}!");

                            // RequestedMessage
                            // EVSE

                            _ = Task.Run(async () => {

                                if (request.RequestedMessage == MessageTrigger.BootNotification)
                                {
                                    await parentNetworkingNode.SendBootNotification(
                                              BootReason:  BootReason.Triggered,
                                              CustomData:  null
                                          );
                                }


                                    // LogStatusNotification
                                    // DiagnosticsStatusNotification
                                    // FirmwareStatusNotification

                                    // Seems not to be allowed any more!
                                    //case MessageTriggers.Heartbeat:
                                    //    await this.SendHeartbeat(
                                    //              CustomData:   null
                                    //          );
                                    //    break;

                                    // MeterValues
                                    // SignChargingStationCertificate

                                else if (request.RequestedMessage == MessageTrigger.StatusNotification &&
                                         request.EVSE is not null)
                                {
                                    await parentNetworkingNode.SendStatusNotification(
                                              EVSEId:        request.EVSE.Id,
                                              ConnectorId:   Connector_Id.Parse(1),
                                              Timestamp:     Timestamp.Now,
                                              Status:        ConnectorStatus.Unavailable,
                                              CustomData:    null
                                          );
                                }

                            },
                            CancellationToken.None);


                            if (request.RequestedMessage == MessageTrigger.BootNotification ||
                                request.RequestedMessage == MessageTrigger.LogStatusNotification ||
                                request.RequestedMessage == MessageTrigger.DiagnosticsStatusNotification ||
                                request.RequestedMessage == MessageTrigger.FirmwareStatusNotification ||
                              //MessageTriggers.Heartbeat
                                request.RequestedMessage == MessageTrigger.SignChargingStationCertificate)
                            {

                                response = new OCPPv2_1.CS.TriggerMessageResponse(
                                               request,
                                               TriggerMessageStatus.Accepted
                                           );

                            }



                            if (response == null &&
                               (request.RequestedMessage == MessageTrigger.MeterValues ||
                                request.RequestedMessage == MessageTrigger.StatusNotification))
                            {

                                response = request.EVSE is not null

                                               ? new OCPPv2_1.CS.TriggerMessageResponse(
                                                     request,
                                                     TriggerMessageStatus.Accepted
                                                 )

                                               : new OCPPv2_1.CS.TriggerMessageResponse(
                                                     request,
                                                     TriggerMessageStatus.Rejected
                                                 );

                            }

                        }

                    }

                    response ??= new OCPPv2_1.CS.TriggerMessageResponse(
                                     request,
                                     TriggerMessageStatus.Rejected
                                 );

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomTriggerMessageResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnTriggerMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnTriggerMessageResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnIncomingDataTransfer

                IncomingMessages.OnIncomingDataTransfer += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnIncomingDataTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingDataTransferRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    DataTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.DataTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DataTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomIncomingDataTransferRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new DataTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming data transfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data ?? "-"}!");

                            // VendorId
                            // MessageId
                            // Data

                            var responseData = request.Data;

                            if (request.Data is not null)
                            {

                                if      (request.Data.Type == JTokenType.String)
                                    responseData = request.Data.ToString().Reverse();

                                else if (request.Data.Type == JTokenType.Object) {

                                    var responseObject = new JObject();

                                    foreach (var property in (request.Data as JObject)!)
                                    {
                                        if (property.Value?.Type == JTokenType.String)
                                            responseObject.Add(property.Key,
                                                               property.Value.ToString().Reverse());
                                    }

                                    responseData = responseObject;

                                }

                                else if (request.Data.Type == JTokenType.Array) {

                                    var responseArray = new JArray();

                                    foreach (var element in (request.Data as JArray)!)
                                    {
                                        if (element?.Type == JTokenType.String)
                                            responseArray.Add(element.ToString().Reverse());
                                    }

                                    responseData = responseArray;

                                }

                            }

                            if (request.VendorId == Vendor_Id.GraphDefined)
                            {
                                response = new DataTransferResponse(
                                               request,
                                               DataTransferStatus.Accepted,
                                               responseData
                                           );
                            }
                            else
                                response = new DataTransferResponse(
                                               request,
                                               DataTransferStatus.Rejected
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomIncomingDataTransferResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnIncomingDataTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingDataTransferResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnCertificateSigned

                IncomingMessages.OnCertificateSigned += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnCertificateSignedRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCertificateSignedRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CertificateSignedResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CertificateSignedResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CertificateSigned request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomCertificateSignedRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CertificateSignedResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CertificateSigned request{(request.CertificateType.HasValue ? $"(certificate type: {request.CertificateType.Value})" : "")}!");

                            // CertificateChain
                            // CertificateType

                            response = new OCPPv2_1.CS.CertificateSignedResponse(
                                           Request:      request,
                                           Status:       request.CertificateChain.FirstOrDefault()?.Parsed is not null
                                                             ? CertificateSignedStatus.Accepted
                                                             : CertificateSignedStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomCertificateSignedResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCertificateSignedResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCertificateSignedResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnInstallCertificate

                IncomingMessages.OnInstallCertificate += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnInstallCertificateRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnInstallCertificateRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.InstallCertificateResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.InstallCertificateResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid InstallCertificate request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomInstallCertificateRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.InstallCertificateResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming InstallCertificate request (certificate type: {request.CertificateType}!");

                            // CertificateType
                            // Certificate

                            var success = certificates.AddOrUpdate(request.CertificateType,
                                                                       a    => request.Certificate,
                                                                      (b,c) => request.Certificate);

                            response = new OCPPv2_1.CS.InstallCertificateResponse(
                                           Request:      request,
                                           Status:       request.Certificate?.Parsed is not null
                                                             ? CertificateStatus.Accepted
                                                             : CertificateStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomInstallCertificateResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnInstallCertificateResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnInstallCertificateResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetInstalledCertificateIds

                IncomingMessages.OnGetInstalledCertificateIds += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        cancellationToken) => {

                    #region Send OnGetInstalledCertificateIdsRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetInstalledCertificateIdsRequest(startTime,
                                                                                           this,
                                                                                           connection,
                                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetInstalledCertificateIdsResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetInstalledCertificateIds request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetInstalledCertificateIdsRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetInstalledCertificateIds request for certificate types: {request.CertificateTypes.Select(certificateType => certificateType).AggregateWith(", ")}!");

                            // CertificateTypes

                            var certs = new List<CertificateHashData>();

                            foreach (var certificateType in request.CertificateTypes)
                            {

                                if (certificates.TryGetValue(InstallCertificateUse.Parse(certificateType.ToString()), out var cert))
                                    certs.Add(new CertificateHashData(
                                                  HashAlgorithm:         HashAlgorithms.SHA256,
                                                  IssuerNameHash:        cert.Parsed?.Issuer               ?? "-",
                                                  IssuerPublicKeyHash:   cert.Parsed?.GetPublicKeyString() ?? "-",
                                                  SerialNumber:          cert.Parsed?.SerialNumber         ?? "-",
                                                  CustomData:            null
                                              ));

                            }

                            response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                                           Request:                    request,
                                           Status:                     GetInstalledCertificateStatus.Accepted,
                                           CertificateHashDataChain:   certs,
                                           StatusInfo:                 null,
                                           CustomData:                 null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetInstalledCertificateIdsResponseSerializer,
                            CustomCertificateHashDataSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetInstalledCertificateIdsResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetInstalledCertificateIdsResponse(responseTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request,
                                                                                            response,
                                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteCertificate

                IncomingMessages.OnDeleteCertificate += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnDeleteCertificateRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteCertificateRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.DeleteCertificateResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.DeleteCertificateResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteCertificate request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomDeleteCertificateRequestSerializer,
                                     CustomCertificateHashDataSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.DeleteCertificateResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteCertificate request!");

                            // CertificateHashData

                            var certKV  = certificates.FirstOrDefault(certificateKV => request.CertificateHashData.SerialNumber == certificateKV.Value.Parsed?.SerialNumber);

                            var success = certificates.TryRemove(certKV);

                            response = new OCPPv2_1.CS.DeleteCertificateResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? DeleteCertificateStatus.Accepted
                                                             : DeleteCertificateStatus.NotFound,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomDeleteCertificateResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteCertificateResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteCertificateResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnNotifyCRL

                IncomingMessages.OnNotifyCRL += async (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) => {

                    #region Send OnNotifyCRLRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyCRLRequest(startTime,
                                                                          this,
                                                                          connection,
                                                                          request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.NotifyCRLResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.NotifyCRLResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid NotifyCRL request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomNotifyCRLRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.NotifyCRLResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming NotifyCRL request!");

                            // NotifyCRLRequestId
                            // Availability
                            // Location

                            response = new OCPPv2_1.CS.NotifyCRLResponse(
                                           Request:      request,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomNotifyCRLResponseSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyCRLResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyCRLResponse(responseTime,
                                                                           this,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnGetLocalListVersion

                IncomingMessages.OnGetLocalListVersion += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetLocalListVersionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLocalListVersionRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetLocalListVersionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetLocalListVersion request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetLocalListVersionRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetLocalListVersion request!");

                            // none

                            response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                                           Request:         request,
                                           VersionNumber:   0,
                                           CustomData:      null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetLocalListVersionResponseSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetLocalListVersionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLocalListVersionResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSendLocalList

                IncomingMessages.OnSendLocalList += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                    #region Send OnSendLocalListRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendLocalListRequest(startTime,
                                                                              this,
                                                                              connection,
                                                                              request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SendLocalListResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SendLocalListResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SendLocalList request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSendLocalListRequestSerializer,
                                     CustomAuthorizationDataSerializer,
                                     CustomIdTokenSerializer,
                                     CustomAdditionalInfoSerializer,
                                     CustomIdTokenInfoSerializer,
                                     CustomMessageContentSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SendLocalListResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SendLocalList request: '{request.UpdateType.AsText()}' version '{request.VersionNumber}'!");

                            // VersionNumber
                            // UpdateType
                            // LocalAuthorizationList

                            response = new OCPPv2_1.CS.SendLocalListResponse(
                                           Request:      request,
                                           Status:       SendLocalListStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSendLocalListResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSendLocalListResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendLocalListResponse(responseTime,
                                                                               this,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearCache

                IncomingMessages.OnClearCache += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnClearCacheRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearCacheRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearCacheResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearCacheResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearCache request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomClearCacheRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearCacheResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearCache request!");

                            // none

                            response = new OCPPv2_1.CS.ClearCacheResponse(
                                           Request:      request,
                                           Status:       ClearCacheStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomClearCacheResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearCacheResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearCacheResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnReserveNow

                IncomingMessages.OnReserveNow += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnReserveNowRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnReserveNowRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ReserveNowResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ReserveNowResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ReserveNow request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomReserveNowRequestSerializer,
                                     CustomIdTokenSerializer,
                                     CustomAdditionalInfoSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ReserveNowResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ReserveNow request (reservation id: {request.Id}, idToken: '{request.IdToken.Value}'{(request.EVSEId.HasValue ? $", evseId: '{request.EVSEId.Value}'" : "")})!");

                            // ReservationId
                            // ExpiryDate
                            // IdToken
                            // ConnectorType
                            // EVSEId
                            // GroupIdToken

                            var success = reservations.TryAdd(request.Id,
                                                              request.Id);

                            response = new OCPPv2_1.CS.ReserveNowResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? ReservationStatus.Accepted
                                                             : ReservationStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomReserveNowResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnReserveNowResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnReserveNowResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCancelReservation

                IncomingMessages.OnCancelReservation += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnCancelReservationRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCancelReservationRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CancelReservationResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CancelReservationResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CancelReservation request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomCancelReservationRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CancelReservationResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            var success = reservations.ContainsKey(request.ReservationId)
                                              ? reservations.TryRemove(request.ReservationId, out _)
                                              : true;

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CancelReservation request for reservation id '{request.ReservationId}': {(success ? "accepted" : "rejected")}!");

                            // ReservationId

                            response = new OCPPv2_1.CS.CancelReservationResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? CancelReservationStatus.Accepted
                                                             : CancelReservationStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomCancelReservationResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCancelReservationResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCancelReservationResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRequestStartTransaction

                IncomingMessages.OnRequestStartTransaction += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) => {

                    #region Send OnRequestStartTransactionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStartTransactionRequest(startTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RequestStartTransactionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RequestStartTransaction request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(

                                     CustomRequestStartTransactionRequestSerializer,
                                     CustomIdTokenSerializer,
                                     CustomAdditionalInfoSerializer,
                                     CustomChargingProfileSerializer,
                                     CustomLimitBeyondSoCSerializer,
                                     CustomChargingScheduleSerializer,
                                     CustomChargingSchedulePeriodSerializer,
                                     CustomV2XFreqWattEntrySerializer,
                                     CustomV2XSignalWattEntrySerializer,
                                     CustomSalesTariffSerializer,
                                     CustomSalesTariffEntrySerializer,
                                     CustomRelativeTimeIntervalSerializer,
                                     CustomConsumptionCostSerializer,
                                     CustomCostSerializer,

                                     CustomAbsolutePriceScheduleSerializer,
                                     CustomPriceRuleStackSerializer,
                                     CustomPriceRuleSerializer,
                                     CustomTaxRuleSerializer,
                                     CustomOverstayRuleListSerializer,
                                     CustomOverstayRuleSerializer,
                                     CustomAdditionalServiceSerializer,

                                     CustomPriceLevelScheduleSerializer,
                                     CustomPriceLevelScheduleEntrySerializer,

                                     CustomTransactionLimitsSerializer,

                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer

                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RequestStartTransaction for '{(request.EVSEId?.ToString() ?? "-")}'!");

                            // ToDo: lock(evses)

                            response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomRequestStartTransactionResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRequestStartTransactionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStartTransactionResponse(responseTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request,
                                                                                         response,
                                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRequestStopTransaction

                IncomingMessages.OnRequestStopTransaction += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    cancellationToken) => {

                    #region Send OnRequestStopTransactionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStopTransactionRequest(startTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RequestStopTransactionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RequestStopTransaction request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomRequestStopTransactionRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RequestStopTransaction for '{request.TransactionId}'!");

                            // TransactionId

                            response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomRequestStopTransactionResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRequestStopTransactionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStopTransactionResponse(responseTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request,
                                                                                        response,
                                                                                        responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetTransactionStatus

                IncomingMessages.OnGetTransactionStatus += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnGetTransactionStatusRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetTransactionStatusRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetTransactionStatusResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetTransactionStatus request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetTransactionStatusRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetTransactionStatus for '{request.TransactionId}'!");

                            // TransactionId

                            response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                                           request,
                                           MessagesInQueue:    false,
                                           OngoingIndicator:   false
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetTransactionStatusResponseSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetTransactionStatusResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetTransactionStatusResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetChargingProfile

                IncomingMessages.OnSetChargingProfile += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnSetChargingProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetChargingProfileRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetChargingProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetChargingProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetChargingProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(

                                     CustomSetChargingProfileRequestSerializer,
                                     CustomChargingProfileSerializer,
                                     CustomLimitBeyondSoCSerializer,
                                     CustomChargingScheduleSerializer,
                                     CustomChargingSchedulePeriodSerializer,
                                     CustomV2XFreqWattEntrySerializer,
                                     CustomV2XSignalWattEntrySerializer,
                                     CustomSalesTariffSerializer,
                                     CustomSalesTariffEntrySerializer,
                                     CustomRelativeTimeIntervalSerializer,
                                     CustomConsumptionCostSerializer,
                                     CustomCostSerializer,

                                     CustomAbsolutePriceScheduleSerializer,
                                     CustomPriceRuleStackSerializer,
                                     CustomPriceRuleSerializer,
                                     CustomTaxRuleSerializer,
                                     CustomOverstayRuleListSerializer,
                                     CustomOverstayRuleSerializer,
                                     CustomAdditionalServiceSerializer,

                                     CustomPriceLevelScheduleSerializer,
                                     CustomPriceLevelScheduleEntrySerializer,

                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer

                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetChargingProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetChargingProfile for '{request.EVSEId}'!");

                            // EVSEId
                            // ChargingProfile

                            response = new OCPPv2_1.CS.SetChargingProfileResponse(
                                           request,
                                           ChargingProfileStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetChargingProfileResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetChargingProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetChargingProfileResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetChargingProfiles

                IncomingMessages.OnGetChargingProfiles += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetChargingProfilesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetChargingProfilesRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetChargingProfilesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetChargingProfiles request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetChargingProfilesRequestSerializer,
                                     CustomChargingProfileCriterionSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetChargingProfiles request ({request.GetChargingProfilesRequestId}) for '{request.EVSEId}'!");

                            // GetChargingProfilesRequestId
                            // ChargingProfile
                            // EVSEId

                            response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                                           request,
                                           GetChargingProfileStatus.Unknown
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetChargingProfilesResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetChargingProfilesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetChargingProfilesResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearChargingProfile

                IncomingMessages.OnClearChargingProfile += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnClearChargingProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearChargingProfileRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearChargingProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearChargingProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomClearChargingProfileRequestSerializer,
                                     CustomClearChargingProfileSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearChargingProfile request for charging profile identification '{request.ChargingProfileId}'!");

                            // ChargingProfileId
                            // ChargingProfileCriteria

                            response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                                           Request:      request,
                                           Status:       ClearChargingProfileStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomClearChargingProfileResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearChargingProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearChargingProfileResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetCompositeSchedule

                IncomingMessages.OnGetCompositeSchedule += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnGetCompositeScheduleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetCompositeScheduleRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetCompositeScheduleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetCompositeSchedule request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetCompositeScheduleRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetCompositeSchedule request for the next {request.Duration.TotalMinutes} minutes of EVSE '{request.EVSEId}'!");

                            // Duration,
                            // EVSEId,
                            // ChargingRateUnit

                            response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           Schedule:     null,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetCompositeScheduleResponseSerializer,
                            CustomCompositeScheduleSerializer,
                            CustomChargingSchedulePeriodSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetCompositeScheduleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetCompositeScheduleResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateDynamicSchedule

                IncomingMessages.OnUpdateDynamicSchedule += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnUpdateDynamicScheduleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateDynamicScheduleRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UpdateDynamicScheduleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateDynamicSchedule request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomUpdateDynamicScheduleRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateDynamicSchedule request for charging profile '{request.ChargingProfileId}'!");

                            // ChargingProfileId

                            // Limit
                            // Limit_L2
                            // Limit_L3

                            // DischargeLimit
                            // DischargeLimit_L2
                            // DischargeLimit_L3

                            // Setpoint
                            // Setpoint_L2
                            // Setpoint_L3

                            // SetpointReactive
                            // SetpointReactive_L2
                            // SetpointReactive_L3

                            response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                                           Request:      request,
                                           Status:       ChargingProfileStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUpdateDynamicScheduleResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateDynamicScheduleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateDynamicScheduleResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnNotifyAllowedEnergyTransfer

                IncomingMessages.OnNotifyAllowedEnergyTransfer += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         cancellationToken) => {

                    #region Send OnNotifyAllowedEnergyTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyAllowedEnergyTransferRequest(startTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid NotifyAllowedEnergyTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomNotifyAllowedEnergyTransferRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming NotifyAllowedEnergyTransfer request allowing energy transfer modes: '{request.AllowedEnergyTransferModes.Select(mode => mode.ToString()).AggregateWith(", ")}'!");

                            // AllowedEnergyTransferModes

                            response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                                           Request:      request,
                                           Status:       NotifyAllowedEnergyTransferStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomNotifyAllowedEnergyTransferResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyAllowedEnergyTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyAllowedEnergyTransferResponse(responseTime,
                                                                                             this,
                                                                                             connection,
                                                                                             request,
                                                                                             response,
                                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUsePriorityCharging

                IncomingMessages.OnUsePriorityCharging += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnUsePriorityChargingRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUsePriorityChargingRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UsePriorityChargingResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UsePriorityCharging request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomUsePriorityChargingRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UsePriorityCharging request for transaction '{request.TransactionId}': {(request.Activate ? "active" : "disabled")}!");

                            // TransactionId
                            // Activate

                            response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUsePriorityChargingResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUsePriorityChargingResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUsePriorityChargingResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUnlockConnector

                IncomingMessages.OnUnlockConnector += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                    #region Send OnUnlockConnectorRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnlockConnectorRequest(startTime,
                                                                                this,
                                                                                connection,
                                                                                request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UnlockConnectorResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UnlockConnectorResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UnlockConnector request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomUnlockConnectorRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UnlockConnectorResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UnlockConnector request for EVSE '{request.EVSEId}' and connector '{request.ConnectorId}'!");

                            // EVSEId
                            // ConnectorId

                            response = new OCPPv2_1.CS.UnlockConnectorResponse(
                                           Request:      request,
                                           Status:       UnlockStatus.UnlockFailed,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUnlockConnectorResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUnlockConnectorResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnlockConnectorResponse(responseTime,
                                                                                 this,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnAFRRSignal

                IncomingMessages.OnAFRRSignal += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnAFRRSignalRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAFRRSignalRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.AFRRSignalResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.AFRRSignalResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AFRRSignal request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomAFRRSignalRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.AFRRSignalResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AFRRSignal '{request.Signal}' for timestamp '{request.ActivationTimestamp}'!");

                            // ActivationTimestamp
                            // Signal

                            response = new OCPPv2_1.CS.AFRRSignalResponse(
                                           Request:      request,
                                           Status:       request.ActivationTimestamp < Timestamp.Now - TimeSpan.FromDays(1)
                                                             ? GenericStatus.Rejected
                                                             : GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomAFRRSignalResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAFRRSignalResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAFRRSignalResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnSetDisplayMessage

                IncomingMessages.OnSetDisplayMessage += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetDisplayMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDisplayMessageRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetDisplayMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetDisplayMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetDisplayMessageRequestSerializer,
                                     CustomMessageInfoSerializer,
                                     CustomMessageContentSerializer,
                                     CustomComponentSerializer,
                                     CustomEVSESerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetDisplayMessage '{request.Message.Message.Content}'!");

                            // Message

                            if (displayMessages.TryAdd(request.Message.Id,
                                                       request.Message)) {

                                response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                               Request:      request,
                                               Status:       DisplayMessageStatus.Accepted,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                               Request:      request,
                                               Status:       DisplayMessageStatus.Rejected,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetDisplayMessageResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetDisplayMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDisplayMessageResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetDisplayMessages

                IncomingMessages.OnGetDisplayMessages += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnGetDisplayMessagesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDisplayMessagesRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetDisplayMessagesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetDisplayMessages request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetDisplayMessagesRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetDisplayMessages request ({request.GetDisplayMessagesRequestId})!");

                            // GetDisplayMessagesRequestId
                            // Ids
                            // Priority
                            // State

                            _ = Task.Run(async () => {

                                var filteredDisplayMessages = displayMessages.Values.
                                                                  Where(displayMessage =>  request.Ids is null || !request.Ids.Any() || request.Ids.Contains(displayMessage.Id)).
                                                                  Where(displayMessage => !request.State.   HasValue || (displayMessage.State.HasValue && displayMessage.State.Value == request.State.   Value)).
                                                                  Where(displayMessage => !request.Priority.HasValue ||  displayMessage.Priority                                     == request.Priority.Value).
                                                                  ToArray();

                                await parentNetworkingNode.NotifyDisplayMessages(
                                          NotifyDisplayMessagesRequestId:   1,
                                          MessageInfos:                     filteredDisplayMessages,
                                          ToBeContinued:                    false,
                                          CustomData:                       null
                                      );

                            },
                            CancellationToken.None);

                            response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                                           request,
                                           GetDisplayMessagesStatus.Accepted
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetDisplayMessagesResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetDisplayMessagesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDisplayMessagesResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearDisplayMessage

                IncomingMessages.OnClearDisplayMessage += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnClearDisplayMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearDisplayMessageRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearDisplayMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearDisplayMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomClearDisplayMessageRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearDisplayMessage request ({request.DisplayMessageId})!");

                            // DisplayMessageId

                            if (displayMessages.TryGetValue(request.DisplayMessageId, out var messageInfo) &&
                                displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(request.DisplayMessageId, messageInfo))) {

                                response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                               request,
                                               ClearMessageStatus.Accepted
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                               request,
                                               ClearMessageStatus.Unknown
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomClearDisplayMessageResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearDisplayMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearDisplayMessageResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCostUpdated

                IncomingMessages.OnCostUpdated += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                    #region Send OnCostUpdatedRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCostUpdatedRequest(startTime,
                                                                            this,
                                                                            connection,
                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CostUpdatedResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CostUpdatedResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CostUpdated request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomCostUpdatedRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CostUpdatedResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CostUpdated request '{request.TotalCost}' for transaction '{request.TransactionId}'!");

                            // TotalCost
                            // TransactionId

                            if (transactions.ContainsKey(request.TransactionId)) {

                                totalCosts.AddOrUpdate(request.TransactionId,
                                                       request.TotalCost,
                                                       (transactionId, totalCost) => request.TotalCost);

                                response = new OCPPv2_1.CS.CostUpdatedResponse(
                                               request
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.CostUpdatedResponse(
                                               request,
                                               Result.GenericError($"Unknown transaction identification '{request.TransactionId}'!")
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomCostUpdatedResponseSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCostUpdatedResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCostUpdatedResponse(responseTime,
                                                                             this,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCustomerInformation

                IncomingMessages.OnCustomerInformation += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnCustomerInformationRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCustomerInformationRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CustomerInformationResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CustomerInformationResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CustomerInformation request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomCustomerInformationRequestSerializer,
                                     CustomIdTokenSerializer,
                                     CustomAdditionalInfoSerializer,
                                     CustomCertificateHashDataSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CustomerInformationResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            var command   = new String[] {

                                                request.Report
                                                    ? "report"
                                                    : "",

                                                request.Clear
                                                    ? "clear"
                                                    : "",

                                            }.Where(text => text.IsNotNullOrEmpty()).
                                              AggregateWith(" and ");

                            var customer  = request.IdToken is not null
                                               ? $"IdToken: {request.IdToken.Value}"
                                               : request.CustomerCertificate is not null
                                                     ? $"certificate s/n: {request.CustomerCertificate.SerialNumber}"
                                                     : request.CustomerIdentifier.HasValue
                                                           ? $"customer identifier: {request.CustomerIdentifier.Value}"
                                                           : "-";


                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CustomerInformation request ({request.CustomerInformationRequestId}) to {command} for customer '{customer}'!");

                            // CustomerInformationRequestId
                            // Report
                            // Clear
                            // CustomerIdentifier
                            // IdToken
                            // CustomerCertificate



                            _ = Task.Run(async () => {

                                await parentNetworkingNode.NotifyCustomerInformation(
                                          NotifyCustomerInformationRequestId:   1,
                                          Data:                                 customer,
                                          SequenceNumber:                       1,
                                          GeneratedAt:                          Timestamp.Now,
                                          ToBeContinued:                        false,
                                          CustomData:                           null
                                      );

                            },
                            CancellationToken.None);



                            response = new OCPPv2_1.CS.CustomerInformationResponse(
                                           request,
                                           CustomerInformationStatus.Accepted
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomCustomerInformationResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCustomerInformationResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCustomerInformationResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // Binary Data Streams Extensions

                #region OnIncomingBinaryDataTransfer

                IncomingMessages.OnIncomingBinaryDataTransfer += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        cancellationToken) => {

                    #region Send OnBinaryDataTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingBinaryDataTransferRequest(startTime,
                                                                                           this,
                                                                                           connection,
                                                                                           request);

                    #endregion


                    #region Check charging station identification

                    BinaryDataTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.BinaryDataTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid BinaryDataTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToBinary(
                                     CustomIncomingBinaryDataTransferRequestSerializer,
                                     CustomBinarySignatureSerializer,
                                     IncludeSignatures: false
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new BinaryDataTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

                            // VendorId
                            // MessageId
                            // Data

                            var responseBinaryData = request.Data;

                            if (request.Data is not null)
                                responseBinaryData = request.Data.Reverse();

                            response = request.VendorId == Vendor_Id.GraphDefined

                                           ? new BinaryDataTransferResponse(
                                                 Request:                request,
                                                 Status:                 BinaryDataTransferStatus.Accepted,
                                                 AdditionalStatusInfo:   null,
                                                 Data:                   responseBinaryData
                                             )

                                           : new BinaryDataTransferResponse(
                                                 Request:                request,
                                                 Status:                 BinaryDataTransferStatus.Rejected,
                                                 AdditionalStatusInfo:   null,
                                                 Data:                   responseBinaryData
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            CustomIncomingBinaryDataTransferResponseSerializer,
                            null, //CustomStatusInfoSerializer,
                            CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnBinaryDataTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingBinaryDataTransferResponse(responseTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request,
                                                                                            response,
                                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetFile

                IncomingMessages.OnGetFile += async (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                    #region Send OnGetFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetFileRequest(startTime,
                                                                        this,
                                                                        connection,
                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.GetFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.GetFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetFileRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.GetFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.GetFileResponse(
                                                 Request:           request,
                                                 FileName:          request.FileName,
                                                 Status:            GetFileStatus.Success,
                                                 FileContent:       "Hello world!".ToUTF8Bytes(),
                                                 FileContentType:   ContentType.Text.Plain,
                                                 FileSHA256:        SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                                 FileSHA512:        SHA512.HashData("Hello world!".ToUTF8Bytes())
                                             )

                                           : new OCPP.CS.GetFileResponse(
                                                 Request:           request,
                                                 FileName:          request.FileName,
                                                 Status:            GetFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            CustomGetFileResponseSerializer,
                            null, //CustomStatusInfoSerializer,
                            CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetFileResponse(responseTime,
                                                                         this,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSendFile

                IncomingMessages.OnSendFile += async (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      cancellationToken) => {

                    #region Send OnSendFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendFileRequest(startTime,
                                                                         this,
                                                                         connection,
                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.SendFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.SendFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SendFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToBinary(
                                     CustomSendFileRequestSerializer,
                                     CustomBinarySignatureSerializer,
                                     IncludeSignatures: false
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.SendFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SendFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.SendFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    SendFileStatus.Success
                                             )

                                           : new OCPP.CS.SendFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    SendFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSendFileResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSendFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendFileResponse(responseTime,
                                                                          this,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteFile

                IncomingMessages.OnDeleteFile += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnDeleteFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteFileRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomDeleteFileRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.DeleteFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    DeleteFileStatus.Success
                                             )

                                           : new OCPP.CS.DeleteFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    DeleteFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomDeleteFileResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteFileResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // E2E Security Extensions

                #region OnAddSignaturePolicy

                IncomingMessages.OnAddSignaturePolicy += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnAddSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddSignaturePolicyRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.AddSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.AddSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AddSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomAddSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.AddSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AddSignaturePolicy!");

                            // Message


                                response = new OCPP.CS.AddSignaturePolicyResponse(
                                               Request:      request,
                                               Status:       GenericStatus.Accepted,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );


                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomAddSignaturePolicyResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAddSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddSignaturePolicyResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateSignaturePolicy

                IncomingMessages.OnUpdateSignaturePolicy += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnUpdateSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateSignaturePolicyRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.UpdateSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.UpdateSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomUpdateSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.UpdateSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateSignaturePolicy!");

                            // Message

                            response = new OCPP.CS.UpdateSignaturePolicyResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUpdateSignaturePolicyResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateSignaturePolicyResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteSignaturePolicy

                IncomingMessages.OnDeleteSignaturePolicy += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnDeleteSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteSignaturePolicyRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomDeleteSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteSignaturePolicy!");

                            // Message

                            response = new OCPP.CS.DeleteSignaturePolicyResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomDeleteSignaturePolicyResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteSignaturePolicyResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnAddUserRole

                IncomingMessages.OnAddUserRole += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                    #region Send OnAddUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddUserRoleRequest(startTime,
                                                                            this,
                                                                            connection,
                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.AddUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.AddUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AddUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomAddUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.AddUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AddUserRole!");

                            // Message

                            response = new OCPP.CS.AddUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomAddUserRoleResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAddUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddUserRoleResponse(responseTime,
                                                                             this,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateUserRole

                IncomingMessages.OnUpdateUserRole += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnUpdateUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateUserRoleRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.UpdateUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.UpdateUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomUpdateUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.UpdateUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateUserRole!");

                            // Message

                            response = new OCPP.CS.UpdateUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomUpdateUserRoleResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateUserRoleResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteUserRole

                IncomingMessages.OnDeleteUserRole += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnDeleteUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteUserRoleRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomDeleteUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteUserRole!");

                            // Message

                            response = new OCPP.CS.DeleteUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomDeleteUserRoleResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteUserRoleResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // E2E Charging Tariffs Extensions

                #region OnSetDefaultChargingTariff

                IncomingMessages.OnSetDefaultChargingTariff += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                    #region Send OnSetDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDefaultChargingTariffRequest(startTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomSetDefaultChargingTariffRequestSerializer,
                                     CustomChargingTariffSerializer,
                                     CustomPriceSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTaxRateSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomEnergyMixSerializer,
                                     CustomEnergySourceSerializer,
                                     CustomEnvironmentalImpactSerializer,
                                     CustomIdTokenSerializer,
                                     CustomAdditionalInfoSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                           Request:   request,
                                           Result:    Result.SignatureError(
                                                          $"Invalid signature: {errorResponse}"
                                                      )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetDefaultChargingTariff!");

                            List<EVSEStatusInfo<SetDefaultChargingTariffStatus>>? evseStatusInfos = null;

                            if (!request.ChargingTariff.Verify(out var err))
                            {
                                response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                               Request:      request,
                                               Status:       SetDefaultChargingTariffStatus.InvalidSignature,
                                               StatusInfo:   new StatusInfo(
                                                                 ReasonCode:       "Invalid charging tariff signature(s)!",
                                                                 AdditionalInfo:   err,
                                                                 CustomData:       null
                                                             ),
                                               CustomData:   null
                                           );
                            }

                            else if (!request.EVSEIds.Any())
                            {

                                response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                               Request:           request,
                                               Status:            SetDefaultChargingTariffStatus.Accepted,
                                               StatusInfo:        null,
                                               EVSEStatusInfos:   null,
                                               CustomData:        null
                                           );

                            }

                            else
                            {

                                foreach (var evseId in request.EVSEIds)
                                {
                                        response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                                       Request:   request,
                                                       Result:    Result.SignatureError(
                                                                      $"Invalid EVSE identification: {evseId}"
                                                                  )
                                                   );
                                }

                                if (response == null)
                                {

                                    evseStatusInfos = new List<EVSEStatusInfo<SetDefaultChargingTariffStatus>>();

                                    foreach (var evseId in request.EVSEIds)
                                    {

                                        evseStatusInfos.Add(new EVSEStatusInfo<SetDefaultChargingTariffStatus>(
                                                                EVSEId:           evseId,
                                                                Status:           SetDefaultChargingTariffStatus.Accepted,
                                                                ReasonCode:       null,
                                                                AdditionalInfo:   null,
                                                                CustomData:       null
                                                            ));

                                    }

                                    response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                                   Request:           request,
                                                   Status:            SetDefaultChargingTariffStatus.Accepted,
                                                   StatusInfo:        null,
                                                   EVSEStatusInfos:   evseStatusInfos,
                                                   CustomData:        null
                                               );

                                }

                            }

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomSetDefaultChargingTariffResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomEVSEStatusInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDefaultChargingTariffResponse(responseTime,
                                                                                          this,
                                                                                          connection,
                                                                                          request,
                                                                                          response,
                                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetDefaultChargingTariff

                IncomingMessages.OnGetDefaultChargingTariff += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                    #region Send OnGetDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDefaultChargingTariffRequest(startTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomGetDefaultChargingTariffRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetDefaultChargingTariff!");

                            response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                                           Request:             request,
                                           Status:              GenericStatus.Accepted,
                                           StatusInfo:          null,
                                           ChargingTariffs:     null,
                                           ChargingTariffMap:   null,
                                           CustomData:          null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomGetDefaultChargingTariffResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomChargingTariffSerializer,
                            CustomPriceSerializer,
                            CustomTariffElementSerializer,
                            CustomPriceComponentSerializer,
                            CustomTaxRateSerializer,
                            CustomTariffRestrictionsSerializer,
                            CustomEnergyMixSerializer,
                            CustomEnergySourceSerializer,
                            CustomEnvironmentalImpactSerializer,
                            CustomIdTokenSerializer,
                            CustomAdditionalInfoSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDefaultChargingTariffResponse(responseTime,
                                                                                          this,
                                                                                          connection,
                                                                                          request,
                                                                                          response,
                                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRemoveDefaultChargingTariff

                IncomingMessages.OnRemoveDefaultChargingTariff += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         cancellationToken) => {

                    #region Send OnRemoveDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRemoveDefaultChargingTariffRequest(startTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RemoveDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RemoveDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     CustomRemoveDefaultChargingTariffRequestSerializer,
                                     CustomSignatureSerializer,
                                     CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RemoveDefaultChargingTariff!");

                            response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                                           Request:           request,
                                           Status:            RemoveDefaultChargingTariffStatus.Accepted,
                                           StatusInfo:        null,
                                           EVSEStatusInfos:   null,
                                           CustomData:        null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            CustomRemoveDefaultChargingTariffResponseSerializer,
                            CustomStatusInfoSerializer,
                            CustomEVSEStatusInfoSerializer2,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRemoveDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRemoveDefaultChargingTariffResponse(responseTime,
                                                                                             this,
                                                                                             connection,
                                                                                             request,
                                                                                             response,
                                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

            }

            #endregion


            #region (Timer) DoMaintenance(State)

            private void DoMaintenanceSync(Object? State)
            {
                if (!DisableMaintenanceTasks)
                    DoMaintenance(State).Wait();
            }

            protected internal virtual async Task _DoMaintenance(Object State)
            {

                foreach (var enqueuedRequest in EnqueuedRequests.ToArray())
                {
                    if (CSClient is NetworkingNodeWSClient wsClient)
                    {

                        var response = await wsClient.SendRequest(
                                                 enqueuedRequest.NetworkingNodeId,
                                                 enqueuedRequest.NetworkPath,
                                                 enqueuedRequest.Command,
                                                 enqueuedRequest.Request.RequestId,
                                                 enqueuedRequest.RequestJSON
                                             );

                        enqueuedRequest.ResponseAction(response);

                        EnqueuedRequests.Remove(enqueuedRequest);

                    }
                }

            }

            private async Task DoMaintenance(Object State)
            {

                if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                               ConfigureAwait(false))
                {
                    try
                    {

                        await _DoMaintenance(State);

                    }
                    catch (Exception e)
                    {

                        while (e.InnerException is not null)
                            e = e.InnerException;

                        DebugX.LogException(e);

                    }
                    finally
                    {
                        MaintenanceSemaphore.Release();
                    }
                }
                else
                    DebugX.LogT("Could not aquire the maintenance tasks lock!");

            }

            #endregion

            #region (Timer) DoSendHeartbeatSync(State)

            private void DoSendHeartbeatSync(Object? State)
            {
                if (!DisableSendHeartbeats)
                {
                    try
                    {
                        this.parentNetworkingNode.SendHeartbeat().Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, nameof(DoSendHeartbeatSync));
                    }
                }
            }

            #endregion


            #region (private) NextRequestId

            public Request_Id NextRequestId
                => parentNetworkingNode.NextRequestId;

            #endregion


            public static void ShowAllRequests()
            {

                var interfaceType      = typeof(IRequest);
                var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                                  GetTypes().
                                                  Where(t => interfaceType.IsAssignableFrom(t) &&
                                                             !t.IsInterface &&
                                                              t.FullName is not null &&
                                                              t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CS.")).
                                                  ToArray() ?? [];

                foreach (var type in implementingTypes)
                {

                    var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                    var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                    Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

                }

            }

            public static void ShowAllResponses()
            {

                var interfaceType      = typeof(IResponse);
                var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                                  GetTypes().
                                                  Where(t => interfaceType.IsAssignableFrom(t) &&
                                                             !t.IsInterface &&
                                                              t.FullName is not null &&
                                                              t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CSMS.")).
                                                  ToArray() ?? [];

                foreach (var type in implementingTypes)
                {

                    var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                    var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                    Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

                }

            }


            #region Dispose()

            public void Dispose()
            { }

            #endregion

        }


    }

}
