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
using System.Security.Authentication;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public static class BatterySwapStationSettings
    {

        /// <summary>
        /// Well-known User Roles for Battery Swap Stations.
        /// </summary>
        public static class UserRoles
        {

            /// <summary>
            /// Vendor User Role.
            /// </summary>
            public static readonly UserRole_Id Vendor  = UserRole_Id.Parse("vendor");

            /// <summary>
            /// Admin User Role.
            /// </summary>
            public static readonly UserRole_Id Admin   = UserRole_Id.Parse("admin");

            /// <summary>
            /// User User Role.
            /// </summary>
            public static readonly UserRole_Id User    = UserRole_Id.Parse("user");

        }

    }

}

namespace cloud.charging.open.protocols.OCPPv2_1.BSS
{

    public delegate Task OnQRCodeChangedDelegate (DateTime                 Timestamp,
                                                  ABatterySwapStationNode  ChargingStation,
                                                  EVSE_Id                  EVSEId,
                                                  String                   QRCodeURL,
                                                  TimeSpan                 RemainingTime,
                                                  DateTimeOffset           EndTime,
                                                  CancellationToken        CancellationToken);


    /// <summary>
    /// An abstract battery swap station node.
    /// </summary>
    public abstract class ABatterySwapStationNode : AOCPPNetworkingNode,
                                                    IBatterySwapStationNode
    {

        #region Data

        protected readonly ConcurrentDictionary<EVSE_Id,               BatterySwapStationEVSE> evses             = [];
        protected readonly ConcurrentDictionary<DisplayMessage_Id,     MessageInfo>         displayMessages   = [];
        protected readonly ConcurrentDictionary<Reservation_Id,        Reservation_Id>      reservations      = [];
        protected readonly ConcurrentDictionary<Transaction_Id,        Transaction>         transactions      = [];
        protected readonly ConcurrentDictionary<Transaction_Id,        Decimal>             totalCosts        = [];
        protected readonly ConcurrentDictionary<InstallCertificateUse, OCPP.Certificate>    certificates      = [];

        #endregion

        #region Properties

        /// <summary>
        /// The battery swap station vendor identification.
        /// </summary>
        [Mandatory]
        public String                   VendorName                  { get; }      = "";

        /// <summary>
        ///  The battery swap station model identification.
        /// </summary>
        [Mandatory]
        public String                   Model                       { get; }      = "";

        /// <summary>
        /// The optional serial number of the battery swap station.
        /// </summary>
        [Optional]
        public String?                  SerialNumber                { get; }

        /// <summary>
        /// The optional firmware version of the battery swap station.
        /// </summary>
        [Optional]
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The modem of the battery swap station.
        /// </summary>
        [Optional]
        public Modem?                   Modem                       { get; }


        /// <summary>
        /// An optional energy meter for the entire battery swap station.
        /// </summary>
        public OCPP.IEnergyMeter?       UplinkEnergyMeter           { get; }




        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                CSMSTime                    { get; private set; } = Timestamp.Now;


        public HTTPAPI?                 HTTPAPI                     { get; }

        public WebAPI?                  WebAPI                      { get; }
        public HTTPPath?                WebAPI_Path                 { get; }

        #endregion

        #region Controllers

        #region Generics

        public IEnumerable<T> GetComponentConfigs<T>(String Name)
            where T : ComponentConfig

        {

            var componentConfigs = OCPP.TryGetComponentConfig(Name, out var controllerList)
                                       ? controllerList.Cast<T>().ToList()
                                       : [];

            foreach (var evse in evses.Values)
                foreach (var componentConfig in evse.GetComponentConfigs<T>(Name))
                    componentConfigs.Add(componentConfig);

            return componentConfigs;

        }


        public IEnumerable<T> GetComponentConfigs<T>(String Name, EVSE_Id EVSEId)
            where T : ComponentConfig

            => evses.TryGetValue(EVSEId, out var evse)
                   ? evse.GetComponentConfigs<T>(Name)
                   : [] ;


        public IEnumerable<T> GetComponentConfigs<T>(String Name, EVSE_Id EVSEId, Connector_Id ConnectorId)
            where T : ComponentConfig

            => OCPP.TryGetComponentConfig(Name, out var controllerList)
                   ? controllerList.Where  (componentConfig => componentConfig.EVSE?.Id          == EVSEId &&
                                                               componentConfig.EVSE?.ConnectorId == ConnectorId).
                                    Cast<T>()
                   : [];

        #endregion


        #region WebPaymentsController

        /// <summary>
        /// All Web Payments Controllers
        /// </summary>
        public IEnumerable<WebPaymentsCtrlr>  WebPaymentsController()
            => GetComponentConfigs<WebPaymentsCtrlr>(nameof(WebPaymentsCtrlr));

        /// <summary>
        /// The Web Payments Controller for the given EVSE.
        /// </summary>
        /// <param name="EVSEId">An EVSE identification.</param>
        public WebPaymentsCtrlr?              WebPaymentsController(EVSE_Id EVSEId)
            => GetComponentConfigs<WebPaymentsCtrlr>(nameof(WebPaymentsCtrlr), EVSEId).FirstOrDefault();

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region CSMS Request  Messages
        //public CustomJObjectSerializerDelegate<CSMS.ResetRequest>?                                   CustomResetRequestSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.UpdateFirmwareRequest>?                          CustomUpdateFirmwareRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.PublishFirmwareRequest>?                         CustomPublishFirmwareRequestSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.UnpublishFirmwareRequest>?                       CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetBaseReportRequest>?                           CustomGetBaseReportRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetReportRequest>?                               CustomGetReportRequestSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetLogRequest>?                                  CustomGetLogRequestSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetVariablesRequest>?                            CustomSetVariablesRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetVariablesRequest>?                            CustomGetVariablesRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetMonitoringBaseRequest>?                       CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetMonitoringReportRequest>?                     CustomGetMonitoringReportRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetMonitoringLevelRequest>?                      CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetVariableMonitoringRequest>?                   CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ClearVariableMonitoringRequest>?                 CustomClearVariableMonitoringRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetNetworkProfileRequest>?                       CustomSetNetworkProfileRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ChangeAvailabilityRequest>?                      CustomChangeAvailabilityRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.TriggerMessageRequest>?                          CustomTriggerMessageRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<     DataTransferRequest>?                            CustomIncomingDataTransferRequestSerializer                  { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.CertificateSignedRequest>?                       CustomCertificateSignedRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.InstallCertificateRequest>?                      CustomInstallCertificateRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetInstalledCertificateIdsRequest>?              CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.DeleteCertificateRequest>?                       CustomDeleteCertificateRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyCRLRequest>?                               CustomNotifyCRLRequestSerializer                             { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.GetLocalListVersionRequest>?                     CustomGetLocalListVersionRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SendLocalListRequest>?                           CustomSendLocalListRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ClearCacheRequest>?                              CustomClearCacheRequestSerializer                            { get; set; }


        //public CustomJObjectSerializerDelegate<CSMS.QRCodeScannedRequest>?                           CustomQRCodeScannedRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ReserveNowRequest>?                              CustomReserveNowRequestSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.CancelReservationRequest>?                       CustomCancelReservationRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.RequestStartTransactionRequest>?                 CustomRequestStartTransactionRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.RequestStopTransactionRequest>?                  CustomRequestStopTransactionRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetTransactionStatusRequest>?                    CustomGetTransactionStatusRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SetChargingProfileRequest>?                      CustomSetChargingProfileRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetChargingProfilesRequest>?                     CustomGetChargingProfilesRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ClearChargingProfileRequest>?                    CustomClearChargingProfileRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetCompositeScheduleRequest>?                    CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.UpdateDynamicScheduleRequest>?                   CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyAllowedEnergyTransferRequest>?             CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<GetFileRequest>?                                      CustomGetFileRequestSerializer                               { get; set; }
        //public CustomBinarySerializerDelegate <SendFileRequest>?                                     CustomSendFileRequestSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                   CustomDeleteFileRequestSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                CustomListDirectoryRequestSerializer                         { get; set; }


        //// E2E Security Extensions
        //public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }


        //// E2E Charging Tariffs Extensions
        //public CustomJObjectSerializerDelegate<CSMS.SetDefaultChargingTariffRequest>?                CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetDefaultChargingTariffRequest>?                CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.RemoveDefaultChargingTariffRequest>?             CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages
        //public CustomJObjectSerializerDelegate<CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<     DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifySettlementResponse>?                       CustomNotifySettlementResponseSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        //public CustomJObjectSerializerDelegate<CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }

        #endregion


        #region Charging Station Request  Messages
        //public CustomJObjectSerializerDelegate<BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        //public CustomJObjectSerializerDelegate<HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        //public CustomJObjectSerializerDelegate<SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        //public CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        //public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }

        #endregion

        #region Charging Station Response Messages
        //public CustomJObjectSerializerDelegate<ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        //public CustomJObjectSerializerDelegate<CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        //public CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }


        //public CustomJObjectSerializerDelegate<QRCodeScannedResponse>?                               CustomQRCodeScannedResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        //public CustomJObjectSerializerDelegate<RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        //public CustomJObjectSerializerDelegate<UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        //public CustomJObjectSerializerDelegate<AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        //public CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        //public CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomIncomingBinaryDataTransferResponseSerializer           { get; set; }
        //public CustomBinarySerializerDelegate <GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<ListDirectoryResponse>?                               CustomListDirectoryResponseSerializer                        { get; set; }


        //// E2E Security Extensions
        //public CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }


        //// E2E Charging Tariff Extensions
        //public CustomJObjectSerializerDelegate<SetDefaultChargingTariffResponse>?                    CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        //public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region Data Structures

        //public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
        //public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        //public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        //public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        //public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        //public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        //public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        //public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        //public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitAtSoCSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        //public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        //public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

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



        //public CustomJObjectSerializerDelegate<OCPPv2_1.ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
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
        //public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        //public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        //public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }

        //public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        //public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        //public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <Signature>?                                           CustomBinarySignatureSerializer                              { get; set; }


        //// E2E Security Extensions



        //// E2E Charging Tariff Extensions

        //public CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        //public CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                                { get; set; }
        //public CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                               { get; set; }
        //public CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                      { get; set; }
        //public CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                           { get; set; }
        //public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        //public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        //public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }

        #endregion

        #endregion

        #region Events

        public event OnQRCodeChangedDelegate? OnQRCodeChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new battery swap station for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this battery swap station.</param>
        public ABatterySwapStationNode(NetworkingNode_Id                  Id,
                                    String                             VendorName,
                                    String                             Model,
                                    I18NString?                        Description                    = null,
                                    String?                            SerialNumber                   = null,
                                    String?                            FirmwareVersion                = null,
                                    Modem?                             Modem                          = null,

                                    IEnumerable<EVSESpec>?             EVSEs                          = null,
                                    OCPP.IEnergyMeter?                 UplinkEnergyMeter              = null,

                                    TimeSpan?                          DefaultRequestTimeout          = null,

                                    SignaturePolicy?                   SignaturePolicy                = null,
                                    SignaturePolicy?                   ForwardingSignaturePolicy      = null,

                                    Boolean                            HTTPAPI_Disabled               = false,
                                    IPPort?                            HTTPAPI_Port                   = null,
                                    String?                            HTTPAPI_ServerName             = null,
                                    String?                            HTTPAPI_ServiceName            = null,
                                    EMailAddress?                      HTTPAPI_RobotEMailAddress      = null,
                                    String?                            HTTPAPI_RobotGPGPassphrase     = null,
                                    Boolean                            HTTPAPI_EventLoggingDisabled   = false,

                                    WebAPI?                            WebAPI                         = null,
                                    Boolean                            WebAPI_Disabled                = false,
                                    HTTPPath?                          WebAPI_Path                    = null,

                                    WebSocketServer?                   ControlWebSocketServer         = null,

                                    Boolean                            DisableSendHeartbeats          = false,
                                    TimeSpan?                          SendHeartbeatsEvery            = null,

                                    Boolean                            DisableMaintenanceTasks        = false,
                                    TimeSpan?                          MaintenanceEvery               = null,

                                    CustomData?                        CustomData                     = null,
                                    DNSClient?                         DNSClient                      = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   !HTTPAPI_Disabled
                       ? new HTTPExtAPI(
                             HTTPServerPort:          HTTPAPI_Port ?? IPPort.Auto,
                             HTTPServerName:          "GraphDefined OCPP Test Charging Station",
                             HTTPServiceName:         "GraphDefined OCPP Test Charging Station Service",
                             APIRobotEMailAddress:    EMailAddress.Parse("GraphDefined OCPP Test Charging Station Robot <robot@charging.cloud>"),
                             APIRobotGPGPassphrase:   "test123",
                             SMTPClient:              new NullMailer(),
                             DNSClient:               DNSClient,
                             AutoStart:               true
                         )
                       : null,
                   ControlWebSocketServer,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DefaultRequestTimeout ?? TimeSpan.FromMinutes(1),

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   DNSClient)

        {

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given vendor name must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given model must not be null or empty!");

            this.VendorName             = VendorName;
            this.Model                  = Model;
            this.SerialNumber           = SerialNumber;
            this.FirmwareVersion        = FirmwareVersion;
            this.Modem                  = Modem;
            this.UplinkEnergyMeter      = UplinkEnergyMeter;

            foreach (var evseSpec in EVSEs ?? [])
            {

                var evse = AddEVSE(evseSpec.AdminStatus,
                                   evseSpec.ConnectorTypes,
                                   evseSpec.MeterType,
                                   evseSpec.MeterSerialNumber,
                                   evseSpec.MeterPublicKey);

                if (evse is not null)
                {
                    evse.OnWebPaymentURLChanged += (timestamp, evseId, qrCodeURL, remainingTime, endTime, ct) =>
                        LogEvent(OnQRCodeChanged, logger => logger.Invoke(timestamp, this, evseId, qrCodeURL, remainingTime, endTime, ct));
                }

            }


            #region Setup HTTP- and WebAPI

            this.WebAPI_Path            = WebAPI_Path ?? HTTPPath.Parse("webapi");

            if (HTTPExtAPI is not null)
            {

                this.HTTPAPI            = HTTPAPI ?? new HTTPAPI(
                                                         this,
                                                         HTTPExtAPI,
                                                         EventLoggingDisabled: HTTPAPI_EventLoggingDisabled
                                                     );

                #region HTTP API Security Settings

                this.HTTPExtAPI.HTTPServer.AddAuth(request => {

                    // Allow some URLs for anonymous access...
                    if (request.Path.StartsWith(HTTPExtAPI.URLPathPrefix + this.WebAPI_Path))
                    {
                        return HTTPExtAPI.Anonymous;
                    }

                    return null;

                });

                #endregion

                if (!WebAPI_Disabled)
                {

                    this.WebAPI  = WebAPI ?? new WebAPI(
                                                 this,
                                                 HTTPExtAPI.HTTPServer,
                                                 URLPathPrefix:   this.WebAPI_Path
                                             );

                }

            }

            #endregion


            #region Register Component Configurations

            AddComponent(new OCPPCommCtrlr(
                             DefaultMessageTimeout:              TimeSpan.FromSeconds(30),
                             FileTransferProtocols:              [ FileTransferProtocol.HTTPS ],
                             NetworkConfigurationPriority:       [ "1" ],
                             NetworkProfileConnectionAttempts:   3,
                             OfflineThreshold:                   TimeSpan.FromSeconds(30),
                             MessageAttempts:                    5,
                             MessageAttemptInterval:             TimeSpan.FromSeconds(30),
                             UnlockOnEVSideDisconnect:           true,
                             ResetRetries:                       3
                         ));

            AddComponent(new SecurityCtrlr(
                             OrganizationName:                   "GraphDefined CSO",
                             CertificateEntries:                 128,
                             SecurityProfile:                    SecurityProfiles.SecurityProfile2,

                             Identity:                           "CS001",
                             BasicAuthPassword:                  "s3cur3!",
                             AdditionalRootCertificateCheck:     false,
                             MaxCertificateChainSize:            128
                         ));

            // A CSMS can request a report of the CustomizationCtrlr component to get a list of all customizations that are supported by the battery swap station.

            #endregion


            OCPP.IN.OnBootNotificationResponseReceived += (timestamp, sender, connection, request, response, runtime, ct) => {

                if (response.Status == RegistrationStatus.Accepted)
                {
                    this.DisableSendHeartbeats  = false;
                    this.SendHeartbeatsEvery    = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                    this.SendHeartbeatsTimer.Change(this.SendHeartbeatsEvery, this.SendHeartbeatsEvery);
                    this.CSMSTime               = response.CurrentTime;
                }

                else if (response.Status == RegistrationStatus.Pending)
                {
                    this.DisableSendHeartbeats  = false;
                    this.SendHeartbeatsEvery    = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                    this.SendHeartbeatsTimer.Change(this.SendHeartbeatsEvery, this.SendHeartbeatsEvery);
                    this.CSMSTime               = response.CurrentTime;
                }

                else
                {
                    this.DisableSendHeartbeats  = true;
                    this.SendHeartbeatsTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }

                return Task.CompletedTask;

            };

        }

        #endregion


        #region ConnectWebSocketClient(...)

        public override Task<HTTPResponse>

            ConnectOCPPWebSocketClient(URL                                                             RemoteURL,
                                       HTTPHostname?                                                   VirtualHostname              = null,
                                       I18NString?                                                     Description                  = null,
                                       Boolean?                                                        PreferIPv4                   = null,
                                       RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                       X509Certificate?                                                ClientCert                   = null,
                                       SslProtocols?                                                   TLSProtocol                  = null,
                                       String?                                                         HTTPUserAgent                = null,
                                       IHTTPAuthentication?                                            HTTPAuthentication           = null,
                                       TimeSpan?                                                       RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate?                                 TransmissionRetryDelay       = null,
                                       UInt16?                                                         MaxNumberOfRetries           = 3,
                                       UInt32?                                                         InternalBufferSize           = null,

                                       IEnumerable<String>?                                            SecWebSocketProtocols        = null,
                                       NetworkingMode?                                                 NetworkingMode               = null,
                                       NetworkingNode_Id?                                              NextHopNetworkingNodeId      = null,
                                       IEnumerable<NetworkingNode_Id>?                                 RoutingNetworkingNodeIds     = null,

                                       Boolean                                                         DisableWebSocketPings        = false,
                                       TimeSpan?                                                       WebSocketPingEvery           = null,
                                       TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                       Boolean                                                         DisableMaintenanceTasks      = false,
                                       TimeSpan?                                                       MaintenanceEvery             = null,

                                       String?                                                         LoggingPath                  = null,
                                       String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
                                       LogfileCreatorDelegate?                                         LogfileCreator               = null,
                                       HTTPClientLogger?                                               HTTPLogger                   = null,
                                       DNSClient?                                                      DNSClient                    = null,

                                       EventTracking_Id?                                               EventTrackingId              = null,
                                       CancellationToken                                               CancellationToken            = default)

            => base.ConnectOCPPWebSocketClient(
                   RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,

                   SecWebSocketProtocols,
                   NetworkingMode,
                   NextHopNetworkingNodeId ??= NetworkingNode_Id.CSMS,
                   RoutingNetworkingNodeIds,

                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   HTTPLogger,
                   DNSClient,

                   EventTrackingId,
                   CancellationToken
               );

        #endregion


        #region (Timer) DoSendHeartbeatSync(State)

        protected override void DoSendHeartbeatsSync(Object? State)
        {
            if (!DisableSendHeartbeats)
            {
                try
                {

                    OCPP.OUT.Heartbeat(
                        new HeartbeatRequest(

                           Destination:        SourceRouting.CSMS,

                           SignKeys:           null, //SignKeys
                           SignInfos:          null, //SignInfos
                           Signatures:         null, //Signatures

                           CustomData:         null, //CustomData

                           RequestId:          null, //RequestId        ?? ChargingStation.NextRequestId
                           RequestTimestamp:   null, //RequestTimestamp ?? Timestamp.Now
                           RequestTimeout:     null  //RequestTimeout   ?? ChargingStation.DefaultRequestTimeout

                        )
                    ).Wait();

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(DoSendHeartbeatsSync));
                }
            }
        }

        #endregion



        #region EVSEs

        public IEnumerable<BatterySwapStationEVSE> EVSEs
            => evses.Values;

        public Boolean TryGetEVSE(EVSE_Id EVSEId, [NotNullWhen(true)] out BatterySwapStationEVSE? EVSE)
            => evses.TryGetValue(EVSEId, out EVSE);



        private BatterySwapStationEVSE? AddEVSE(OperationalStatus                       AdminStatus,
                                             IEnumerable<ConnectorType>?             ConnectorTypes      = null,
                                             String?                                 MeterType           = null,
                                             String?                                 MeterSerialNumber   = null,
                                             String?                                 MeterPublicKey      = null
                                             //IEnumerable<ChargingStationConnector>?  Connectors          = null
                                             )
        {

            lock (evses)
            {

                var evse = new BatterySwapStationEVSE(
                               this,
                               EVSE_Id.Parse((Byte) (evses.Count + 1)),
                               AdminStatus,
                               ConnectorTypes,
                               MeterType,
                               MeterSerialNumber,
                               MeterPublicKey
                           );

                return evses.TryAdd(evse.Id, evse)
                           ? evse
                           : null;

            }

        }

        #endregion


        public List<ComponentConfig> AddComponent(ComponentConfig Component)

            => OCPP.AddOrUpdateComponentConfig(
                   Component.Name,
                   name         => [ Component ],
                   (name, list) => list.AddAndReturnList(Component)
               );


        public IEnumerable<ComponentConfig> GetComponentConfigs(String Name, EVSE? EVSE = null)
        {

            List<ComponentConfig>? componentConfigList = null;

            if (EVSE is null)
                OCPP.TryGetComponentConfig(Name, out componentConfigList);

            else if (evses.TryGetValue(EVSE.Id, out var evse))
                evse.ComponentConfigs.TryGetValue(Name, out componentConfigList);

            return componentConfigList ?? [];

        }


        #region (Timer) DoMaintenance(State)

        protected override async Task DoMaintenanceAsync(Object? State)
        {

            //await base.DoMaintenanceAsync(State);

            foreach (var evse in EVSEs)
                await evse.DoMaintenanceAsync(this, State);

        }

        #endregion


        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("id",  Id.ToString())

                       );

            return json;

        }


        #region (private)  LogEvent    (Logger, LogHandler,    ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(BatterySwapStationEVSE),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPPCommand
               );

        #endregion

        #region (override) HandleErrors(Module, Caller, ErrorResponse)

        public override Task HandleErrors(String  Module,
                                          String  Caller,
                                          String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ErrorResponse
                   );

        }

        #endregion

        #region (override) HandleErrors(Module, Caller, ExceptionOccured)

        public override Task HandleErrors(String     Module,
                                          String     Caller,
                                          Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ExceptionOccured
                   );

        }

        #endregion


    }

}
