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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using System.Collections.Concurrent;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Properties

        public Connector_Id   Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region ChargingStationConnector(Id, ConnectorType)

        public ChargingStationConnector(Connector_Id   Id,
                                        ConnectorType  ConnectorType)
        {

            this.Id             = Id;
            this.ConnectorType  = ConnectorType;

        }

        #endregion

    }

    #endregion

    #region (class) ChargingStationEVSE

    /// <summary>
    /// An EVSE at a charging station.
    /// </summary>
    public class ChargingStationEVSE
    {

        #region Properties

        public EVSE_Id            Id                       { get; }

        public Reservation_Id?    ReservationId            { get; set; }

        public OperationalStatus  AdminStatus              { get; set; }

        public ConnectorStatus    Status                   { get; set; }


        public String?            MeterType                { get; set; }
        public String?            MeterSerialNumber        { get; set; }
        public String?            MeterPublicKey           { get; set; }


        public Boolean            IsReserved               { get; set; }

        public Boolean            IsCharging               { get; set; }

        public IdToken?           IdToken                  { get; set; }

        public IdToken?           GroupIdToken             { get; set; }

        public Transaction_Id?    TransactionId            { get; set; }

        public RemoteStart_Id?    RemoteStartId            { get; set; }

        public ChargingProfile?   ChargingProfile          { get; set; }


        public DateTime?          StartTimestamp           { get; set; }

        public Decimal?           MeterStartValue          { get; set; }

        public String?            SignedStartMeterValue    { get; set; }

        public DateTime?          StopTimestamp            { get; set; }

        public Decimal?           MeterStopValue           { get; set; }

        public String?            SignedStopMeterValue     { get; set; }


        public ChargingTariff?    DefaultChargingTariff    { get; set; }

        #endregion

        #region ChargingStationEVSE(Id, AdminStatus, ...)

        public ChargingStationEVSE(EVSE_Id                                 Id,
                                   OperationalStatus                       AdminStatus,
                                   String?                                 MeterType           = null,
                                   String?                                 MeterSerialNumber   = null,
                                   String?                                 MeterPublicKey      = null,
                                   IEnumerable<ChargingStationConnector>?  Connectors          = null)
        {

            this.Id                 = Id;
            this.AdminStatus        = AdminStatus;
            this.MeterType          = MeterType;
            this.MeterSerialNumber  = MeterSerialNumber;
            this.MeterPublicKey     = MeterPublicKey;
            this.connectors         = Connectors is not null && Connectors.Any()
                                          ? new HashSet<ChargingStationConnector>(Connectors)
                                          : new HashSet<ChargingStationConnector>();

        }

        #endregion


        #region Connectors

        private readonly HashSet<ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }

        #endregion


    }

    #endregion


    /// <summary>
    /// An abstract charging station.
    /// </summary>
    public abstract class AChargingStation : ANetworkingNode
    {

        #region Data

        private readonly  HTTPExtAPI  NetworkingNodeAPI;

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly   TimeSpan    DefaultSendHeartbeatsEvery   = TimeSpan.FromSeconds(30);

        private readonly  Timer       SendHeartbeatsTimer;

        public ConcurrentDictionary<String, List<ComponentConfig>> ComponentConfigs = [];

        public List<UserRole> UserRoles = [];

        #endregion

        #region Properties

        /// <summary>
        /// The networking node vendor identification.
        /// </summary>
        [Mandatory]
        public String                   VendorName                  { get; }      = "";

        /// <summary>
        ///  The networking node model identification.
        /// </summary>
        [Mandatory]
        public String                   Model                       { get; }      = "";

        /// <summary>
        /// The optional serial number of the networking node.
        /// </summary>
        [Optional]
        public String?                  SerialNumber                { get; }

        /// <summary>
        /// The optional firmware version of the networking node.
        /// </summary>
        [Optional]
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The modem of the networking node.
        /// </summary>
        [Optional]
        public Modem?                   Modem                       { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterPublicKey              { get; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatsEvery         { get; set; }

        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                CSMSTime                    { get; private set; } = Timestamp.Now;

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                 DefaultRequestTimeout       { get; }


        /// <summary>
        /// Disable all heartbeats.
        /// </summary>
        public Boolean                  DisableSendHeartbeats       { get; set; }





        protected readonly Dictionary<EVSE_Id, ChargingStationEVSE> evses;

        public IEnumerable<ChargingStationEVSE> EVSEs
            => evses.Values;




        public WebAPI                      WebAPI                     { get; }



        private readonly HashSet<WebAPI> webAPIs = [];

        /// <summary>
        /// An enumeration of all WebAPIs.
        /// </summary>
        public IEnumerable<WebAPI> WebAPIs
            => webAPIs;

        #endregion

        #region Custom JSON serializer delegates

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


        public CustomJObjectSerializerDelegate<CSMS.QRCodeScannedRequest>?                           CustomQRCodeScannedRequestSerializer                         { get; set; }
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
        public CustomJObjectSerializerDelegate<CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<GetFileRequest>?                                      CustomGetFileRequestSerializer                               { get; set; }
        public CustomBinarySerializerDelegate <SendFileRequest>?                                     CustomSendFileRequestSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                   CustomDeleteFileRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                CustomListDirectoryRequestSerializer                         { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<CSMS.SetDefaultChargingTariffRequest>?                CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
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
        public CustomJObjectSerializerDelegate<CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

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


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }

        #endregion


        #region Charging Station Request  Messages
        public CustomJObjectSerializerDelegate<BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }

        #endregion

        #region Charging Station Response Messages
        public CustomJObjectSerializerDelegate<ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }


        public CustomJObjectSerializerDelegate<QRCodeScannedResponse>?                               CustomQRCodeScannedResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomIncomingBinaryDataTransferResponseSerializer           { get; set; }
        public CustomBinarySerializerDelegate <GetFileResponse>?                                     CustomGetFileResponseSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SendFileResponse>?                                    CustomSendFileResponseSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileResponse>?                                  CustomDeleteFileResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryResponse>?                               CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?                          CustomAddSignaturePolicyResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?                       CustomUpdateSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?                       CustomDeleteSignaturePolicyResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleResponse>?                                 CustomAddUserRoleResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?                              CustomUpdateUserRoleResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?                              CustomDeleteUserRoleResponseSerializer                       { get; set; }


        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<SetDefaultChargingTariffResponse>?                    CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<GetDefaultChargingTariffResponse>?                    CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffResponse>?                 CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region Data Structures

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
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



        public CustomJObjectSerializerDelegate<OCPPv2_1.ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
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
        public CustomBinarySerializerDelegate <Signature>?                                           CustomBinarySignatureSerializer                              { get; set; }


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

        #endregion

        #region Events

        #region WebSocket connections

        /// <summary>
        /// An event sent whenever the HTTP web socket server started.
        /// </summary>
        public event OnServerStartedDelegate?                 OnServerStarted;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnValidateTCPConnectionDelegate?         OnValidateTCPConnection;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        /// <summary>
        /// An event sent whenever a HTTP request was received.
        /// </summary>
        public event HTTPRequestLogDelegate?                  OnHTTPRequest;

        /// <summary>
        /// An event sent whenever the HTTP headers of a new web socket connection
        /// need to be validated or filtered by an upper layer application logic.
        /// </summary>
        public event OnValidateWebSocketConnectionDelegate?   OnValidateWebSocketConnection;

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnCSMSNewWebSocketConnectionDelegate?    OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCSMSCloseMessageReceivedDelegate?      OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnCSMSTCPConnectionClosedDelegate?       OnTCPConnectionClosed;

        /// <summary>
        /// An event sent whenever the HTTP web socket server stopped.
        /// </summary>
        public event OnServerStoppedDelegate?                 OnServerStopped;

        #endregion

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a JSON message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a JSON message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this charging station.</param>
        public AChargingStation(NetworkingNode_Id                  Id,
                                String                             VendorName,
                                String                             Model,
                                I18NString?                        Description                 = null,
                                String?                            SerialNumber                = null,
                                String?                            FirmwareVersion             = null,
                                Modem?                             Modem                       = null,

                                IEnumerable<ChargingStationEVSE>?  EVSEs                       = null,

                                String?                            MeterType                   = null,
                                String?                            MeterSerialNumber           = null,
                                String?                            MeterPublicKey              = null,

                                CustomData?                        CustomData                  = null,

                                SignaturePolicy?                   SignaturePolicy             = null,
                                SignaturePolicy?                   ForwardingSignaturePolicy   = null,

                                Boolean                            DisableSendHeartbeats       = false,
                                TimeSpan?                          SendHeartbeatsEvery         = null,
                                TimeSpan?                          DefaultRequestTimeout       = null,

                                IPPort?                            HTTPUploadPort              = null,
                                IPPort?                            HTTPDownloadPort            = null,

                                Boolean                            DisableMaintenanceTasks     = false,
                                TimeSpan?                          MaintenanceEvery            = null,
                                DNSClient?                         DNSClient                   = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DefaultRequestTimeout,

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

            this.evses                  = EVSEs?.ToDictionary(evse => evse.Id) ?? [];

            this.MeterType              = MeterType;
            this.MeterSerialNumber      = MeterSerialNumber;
            this.MeterPublicKey         = MeterPublicKey;

            this.DefaultRequestTimeout  = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

            this.DisableSendHeartbeats  = DisableSendHeartbeats;
            this.SendHeartbeatsEvery    = SendHeartbeatsEvery   ?? DefaultSendHeartbeatsEvery;
            this.SendHeartbeatsTimer    = new Timer(
                                              DoSendHeartbeatsSync,
                                              null,
                                              this.SendHeartbeatsEvery,
                                              this.SendHeartbeatsEvery
                                          );


            #region Setup generic HTTP API

            this.NetworkingNodeAPI  = new HTTPExtAPI(
                                          HTTPServerPort:         IPPort.Parse(3532),
                                          HTTPServerName:         "GraphDefined OCPP Test Charging Station",
                                          HTTPServiceName:        "GraphDefined OCPP Test Charging Station Service",
                                          APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Charging Station Robot <robot@charging.cloud>"),
                                          APIRobotGPGPassphrase:  "test123",
                                          SMTPClient:             new NullMailer(),
                                          DNSClient:              DNSClient,
                                          AutoStart:              true
                                      );

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            #endregion

            #region HTTP API Security Settings

            var webAPIPrefix        = "webapi";

            this.NetworkingNodeAPI.HTTPServer.AddAuth(request => {

                // Allow some URLs for anonymous access...
                if (request.Path.StartsWith(NetworkingNodeAPI.URLPathPrefix + webAPIPrefix))
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });

            #endregion

            #region Setup WebAPI

            this.WebAPI                 = new WebAPI(
                                              this,
                                              NetworkingNodeAPI,

                                              URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

                                          );

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

            // A CSMS can request a report of the CustomizationCtrlr component to get a list of all customizations that are supported by the charging station.

            #endregion

            #region Register User Roles

            var ur1 = KeyPair.GenerateKeys()!;

            UserRoles.Add(new UserRole(

                              KeyPair:                 ur1,
                              ComponentAccessRights:   null,

                              //SignerName:              null,
                              //Description:             null,
                              //Timestamp:               null,

                              CustomData:              null

                          ));

            #endregion



            OCPP.IN.OnBootNotificationResponseReceived += (timestamp, sender, request, response, runtime) => {

                switch (response.Status)
                {

                    case RegistrationStatus.Accepted:
                        this.CSMSTime               = response.CurrentTime;
                        this.SendHeartbeatsEvery    = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                        this.SendHeartbeatsTimer.Change(this.SendHeartbeatsEvery, this.SendHeartbeatsEvery);
                        this.DisableSendHeartbeats  = false;
                        break;

                    case RegistrationStatus.Pending:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                    case RegistrationStatus.Rejected:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                }

                return Task.CompletedTask;

            };

        }

        #endregion


        #region ConnectWebSocket(...)

        //public Task<HTTPResponse?> ConnectWebSocket(URL                                  RemoteURL,
        //                                            HTTPHostname?                        VirtualHostname              = null,
        //                                            String?                              Description                  = null,
        //                                            RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
        //                                            LocalCertificateSelectionHandler?    LocalCertificateSelector     = null,
        //                                            X509Certificate?                     ClientCert                   = null,
        //                                            SslProtocols?                        TLSProtocol                  = null,
        //                                            Boolean?                             PreferIPv4                   = null,
        //                                            String?                              HTTPUserAgent                = null,
        //                                            IHTTPAuthentication?                 HTTPAuthentication           = null,
        //                                            TimeSpan?                            RequestTimeout               = null,
        //                                            TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
        //                                            UInt16?                              MaxNumberOfRetries           = null,
        //                                            UInt32?                              InternalBufferSize           = null,

        //                                            IEnumerable<String>?                 SecWebSocketProtocols        = null,
        //                                            NetworkingMode?                      NetworkingMode               = null,

        //                                            Boolean                              DisableMaintenanceTasks      = false,
        //                                            TimeSpan?                            MaintenanceEvery             = null,
        //                                            Boolean                              DisableWebSocketPings        = false,
        //                                            TimeSpan?                            WebSocketPingEvery           = null,
        //                                            TimeSpan?                            SlowNetworkSimulationDelay   = null,

        //                                            String?                              LoggingPath                  = null,
        //                                            String?                              LoggingContext               = null,
        //                                            LogfileCreatorDelegate?              LogfileCreator               = null,
        //                                            HTTPClientLogger?                    HTTPLogger                   = null,
        //                                            DNSClient?                           DNSClient                    = null)

        //    => AsCS.ConnectWebSocket(RemoteURL,
        //                             VirtualHostname,
        //                             Description,
        //                             RemoteCertificateValidator,
        //                             LocalCertificateSelector,
        //                             ClientCert,
        //                             TLSProtocol,
        //                             PreferIPv4,
        //                             HTTPUserAgent,
        //                             HTTPAuthentication,
        //                             RequestTimeout,
        //                             TransmissionRetryDelay,
        //                             MaxNumberOfRetries,
        //                             InternalBufferSize,

        //                             SecWebSocketProtocols,
        //                             NetworkingMode,

        //                             DisableMaintenanceTasks,
        //                             MaintenanceEvery,
        //                             DisableWebSocketPings,
        //                             WebSocketPingEvery,
        //                             SlowNetworkSimulationDelay,

        //                             LoggingPath,
        //                             LoggingContext,
        //                             LogfileCreator,
        //                             HTTPLogger,
        //                             DNSClient);

        #endregion


        #region ConnectWebSocketClient(...)

        public async Task<HTTPResponse> ConnectWebSocketClient(NetworkingNode_Id                                               NetworkingNodeId,
                                                               URL                                                             RemoteURL,
                                                               HTTPHostname?                                                   VirtualHostname              = null,
                                                               String?                                                         Description                  = null,
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

                                                               Boolean                                                         DisableWebSocketPings        = false,
                                                               TimeSpan?                                                       WebSocketPingEvery           = null,
                                                               TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                               Boolean                                                         DisableMaintenanceTasks      = false,
                                                               TimeSpan?                                                       MaintenanceEvery             = null,

                                                               String?                                                         LoggingPath                  = null,
                                                               String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
                                                               LogfileCreatorDelegate?                                         LogfileCreator               = null,
                                                               HTTPClientLogger?                                               HTTPLogger                   = null,
                                                               DNSClient?                                                      DNSClient                    = null)
        {

            var ocppWebSocketClient = new OCPPWebSocketClient(

                                          OCPP,

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

                                          SecWebSocketProtocols ?? [
                                                                      "ocpp2.0.1",
                                                                       Version.WebSocketSubProtocolId
                                                                   ],
                                          NetworkingMode,

                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DisableMaintenanceTasks,
                                          MaintenanceEvery,

                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator,
                                          HTTPLogger,
                                          DNSClient

                                      );

            ocppWebSocketClients.Add(ocppWebSocketClient);

            var connectResponse = await ocppWebSocketClient.Connect();

            connectResponse.Item1.TryAddCustomData(OCPPAdapter.NetworkingNodeId_WebSocketKey,
                                                   NetworkingNodeId);

            OCPP.AddStaticRouting(NetworkingNodeId,
                                  ocppWebSocketClient,
                                  0,
                                  Timestamp.Now);

            return connectResponse.Item2;

        }

        #endregion


        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        /// 
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer AttachWebSocketServer(String?                                                         HTTPServiceName              = null,
                                                         IIPAddress?                                                     IPAddress                    = null,
                                                         IPPort?                                                         TCPPort                      = null,
                                                         I18NString?                                                     Description                  = null,

                                                         Boolean                                                         RequireAuthentication        = true,
                                                         Boolean                                                         DisableWebSocketPings        = false,
                                                         TimeSpan?                                                       WebSocketPingEvery           = null,
                                                         TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                         Func<X509Certificate2>?                                         ServerCertificateSelector    = null,
                                                         RemoteTLSClientCertificateValidationHandler<IWebSocketServer>?  ClientCertificateValidator   = null,
                                                         LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                                         SslProtocols?                                                   AllowedTLSProtocols          = null,
                                                         Boolean?                                                        ClientCertificateRequired    = null,
                                                         Boolean?                                                        CheckCertificateRevocation   = null,

                                                         ServerThreadNameCreatorDelegate?                                ServerThreadNameCreator      = null,
                                                         ServerThreadPriorityDelegate?                                   ServerThreadPrioritySetter   = null,
                                                         Boolean?                                                        ServerThreadIsBackground     = null,
                                                         ConnectionIdBuilder?                                            ConnectionIdBuilder          = null,
                                                         TimeSpan?                                                       ConnectionTimeout            = null,
                                                         UInt32?                                                         MaxClientConnections         = null,

                                                         Boolean                                                         AutoStart                    = false)
        {

            var ocppWebSocketServer = new OCPPWebSocketServer(

                                          OCPP,

                                          HTTPServiceName,
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          ServerCertificateSelector,
                                          ClientCertificateValidator,
                                          LocalCertificateSelector,
                                          AllowedTLSProtocols,
                                          ClientCertificateRequired,
                                          CheckCertificateRevocation,

                                          ServerThreadNameCreator,
                                          ServerThreadPrioritySetter,
                                          ServerThreadIsBackground,
                                          ConnectionIdBuilder,
                                          ConnectionTimeout,
                                          MaxClientConnections,

                                          DNSClient,
                                          AutoStart: false

                                      );

            WireWebSocketServer(ocppWebSocketServer);

            if (AutoStart)
                ocppWebSocketServer.Start();

            return ocppWebSocketServer;

        }

        #endregion

        #region (private) WireWebSocketServer(WebSocketServer)

        private void WireWebSocketServer(OCPPWebSocketServer WebSocketServer)
        {

            ocppWebSocketServers.Add(WebSocketServer);

            #region WebSocket related

            #region OnServerStarted

            WebSocketServer.OnServerStarted += async (timestamp,
                                                      server,
                                                      eventTrackingId,
                                                      cancellationToken) => {

                var onServerStarted = OnServerStarted;
                if (onServerStarted is not null)
                {
                    try
                    {

                        await Task.WhenAll(onServerStarted.GetInvocationList().
                                               OfType <OnServerStartedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnServerStarted),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnNewTCPConnection

            WebSocketServer.OnNewTCPConnection += async (timestamp,
                                                         webSocketServer,
                                                         newTCPConnection,
                                                         eventTrackingId,
                                                         cancellationToken) => {

                var onNewTCPConnection = OnNewTCPConnection;
                if (onNewTCPConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNewTCPConnection.GetInvocationList().
                                               OfType <OnNewTCPConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              newTCPConnection,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNewTCPConnection),
                                  e
                              );
                    }
                }

            };

            #endregion

            // Failed (Charging Station) Authentication

            #region OnNetworkingNodeNewWebSocketConnection

            WebSocketServer.OnNetworkingNodeNewWebSocketConnection += async (timestamp,
                                                                             ocppWebSocketServer,
                                                                             newConnection,
                                                                             networkingNodeId,
                                                                             eventTrackingId,
                                                                             sharedSubprotocols,
                                                                             cancellationToken) => {

                // A new connection from the same networking node/charging station will replace the older one!
                OCPP.AddStaticRouting(DestinationId:    networkingNodeId,
                                      WebSocketServer:  ocppWebSocketServer,
                                      Priority:         0,
                                      Timestamp:        timestamp);

                #region Send OnNewWebSocketConnection

                var logger = OnNewWebSocketConnection;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeNewWebSocketConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              ocppWebSocketServer,
                                                                              newConnection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              sharedSubprotocols,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(AChargingStation),
                                  nameof(OnNewWebSocketConnection),
                                  e
                              );
                    }
                }

                #endregion

            };

            #endregion

            #region OnNetworkingNodeCloseMessageReceived

            WebSocketServer.OnNetworkingNodeCloseMessageReceived += async (timestamp,
                                                             server,
                                                             connection,
                                                             networkingNodeId,
                                                             eventTrackingId,
                                                             statusCode,
                                                             reason,
                                                             cancellationToken) => {

                var logger = OnCloseMessageReceived;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeCloseMessageReceivedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              connection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              statusCode,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(AChargingStation),
                                  nameof(OnCloseMessageReceived),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnNetworkingNodeTCPConnectionClosed

            WebSocketServer.OnNetworkingNodeTCPConnectionClosed += async (timestamp,
                                                            server,
                                                            connection,
                                                            networkingNodeId,
                                                            eventTrackingId,
                                                            reason,
                                                            cancellationToken) => {

                var logger = OnTCPConnectionClosed;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <NetworkingNode.OnNetworkingNodeTCPConnectionClosedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              connection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(AChargingStation),
                                  nameof(OnTCPConnectionClosed),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnServerStopped

            WebSocketServer.OnServerStopped += async (timestamp,
                                                  server,
                                                  eventTrackingId,
                                                  reason,
                                                  cancellationToken) => {

                var logger = OnServerStopped;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                                 OfType <OnServerStoppedDelegate>().
                                                 Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                timestamp,
                                                                                server,
                                                                                eventTrackingId,
                                                                                reason,
                                                                                cancellationToken
                                                                            )).
                                                 ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnServerStopped),
                                  e
                              );
                    }
                }

            };

            #endregion

            // (Generic) Error Handling

            #endregion

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            await Task.WhenAll(ocppWebSocketServers.
                                   Select (ocppWebSocketServer => ocppWebSocketServer.Shutdown(Message, Wait)).
                                   ToArray());

        }

        #endregion


        #region AttachWebAPI(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public WebAPI AttachWebAPI(HTTPHostname?                               HTTPHostname            = null,
                                                 String?                                     ExternalDNSName         = null,
                                                 IPPort?                                     HTTPServerPort          = null,
                                                 HTTPPath?                                   BasePath                = null,
                                                 String?                                     HTTPServerName          = null,

                                                 HTTPPath?                                   URLPathPrefix           = null,
                                                 String?                                     HTTPServiceName         = null,

                                                 String                                      HTTPRealm               = "...",
                                                 Boolean                                     RequireAuthentication   = true,
                                                 IEnumerable<KeyValuePair<String, String>>?  HTTPLogins              = null,

                                                 String?                                     HTMLTemplate            = null,
                                                 Boolean                                     AutoStart               = false)
        {

            var httpAPI  = new HTTPExtAPI(

                               HTTPHostname,
                               ExternalDNSName,
                               HTTPServerPort,
                               BasePath,
                               HTTPServerName,

                               URLPathPrefix,
                               HTTPServiceName,

                               DNSClient: DNSClient,
                               AutoStart: false

                           );

            var webAPI   = new WebAPI(

                               this,
                               httpAPI,

                               HTTPServerName,
                               URLPathPrefix,
                               BasePath,
                               HTTPRealm,
                               HTTPLogins,
                               HTMLTemplate

                           );

            if (AutoStart)
                httpAPI.Start();

            return webAPI;

        }

        #endregion



        #region (Timer) DoSendHeartbeatSync(State)

        private void DoSendHeartbeatsSync(Object? State)
        {
            if (!DisableSendHeartbeats)
            {
                try
                {

                    OCPP.OUT.Heartbeat(
                        new HeartbeatRequest(

                           DestinationId:      NetworkingNode_Id.CSMS,

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




        public void AddComponent(ComponentConfig Component)
        {

            ComponentConfigs.AddOrUpdate(
                                 Component.Name,
                                 name => [Component],
                                 (name, list) => list.AddAndReturnList(Component)
                             );

        }







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
