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
using cloud.charging.open.protocols.OCPPv2_1.CS2;

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
    public abstract class AChargingStation : ANetworkingNode,
                                             IChargingStation2
    {

        #region Data

        private readonly HTTPExtAPI  NetworkingNodeAPI;

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly            TimeSpan                    DefaultSendHeartbeatsEvery   = TimeSpan.FromSeconds(30);

        private readonly           Timer                       SendHeartbeatsTimer;

        #endregion

        #region Properties

        /// <summary>
        /// The networking node vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                 { get; }      = "";

        /// <summary>
        ///  The networking node model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                      { get; }      = "";

        /// <summary>
        /// The optional serial number of the networking node.
        /// </summary>
        [Optional]
        public String?                     SerialNumber               { get; }

        /// <summary>
        /// The optional firmware version of the networking node.
        /// </summary>
        [Optional]
        public String?                     FirmwareVersion            { get; }

        /// <summary>
        /// The modem of the networking node.
        /// </summary>
        [Optional]
        public Modem?                      Modem                      { get; }

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
        public TimeSpan                 SendHeartbeatsEvery          { get; set; }

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





        private readonly Dictionary<EVSE_Id, ChargingStationEVSE> evses;

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

        ///// <summary>
        ///// An event sent whenever the HTTP web socket server started.
        ///// </summary>
        //public event OnServerStartedDelegate?                 OnServerStarted;

        ///// <summary>
        ///// An event sent whenever a new TCP connection was accepted.
        ///// </summary>
        //public event OnValidateTCPConnectionDelegate?         OnValidateTCPConnection;

        ///// <summary>
        ///// An event sent whenever a new TCP connection was accepted.
        ///// </summary>
        //public event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        ///// <summary>
        ///// An event sent whenever a HTTP request was received.
        ///// </summary>
        //public event HTTPRequestLogDelegate?                  OnHTTPRequest;

        ///// <summary>
        ///// An event sent whenever the HTTP headers of a new web socket connection
        ///// need to be validated or filtered by an upper layer application logic.
        ///// </summary>
        //public event OnValidateWebSocketConnectionDelegate?   OnValidateWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever the HTTP connection switched successfully to web socket.
        ///// </summary>
        //public event OnCSMSNewWebSocketConnectionDelegate?    OnNewWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever a reponse to a HTTP request was sent.
        ///// </summary>
        //public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        ///// <summary>
        ///// An event sent whenever a web socket close frame was received.
        ///// </summary>
        //public event OnCSMSCloseMessageReceivedDelegate?      OnCloseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a TCP connection was closed.
        ///// </summary>
        //public event OnCSMSTCPConnectionClosedDelegate?       OnTCPConnectionClosed;

        ///// <summary>
        ///// An event sent whenever the HTTP web socket server stopped.
        ///// </summary>
        //public event OnServerStoppedDelegate?                 OnServerStopped;

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


        //#region Outgoing Messages: Charging Station --(NN)-> CSMS

        //#region OnBootNotification                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a BootNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnBootNotificationRequestSentDelegate?                      OnBootNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a BootNotification request was received.
        ///// </summary>
        //public event OnBootNotificationResponseReceivedDelegate?                     OnBootNotificationResponse;

        //#endregion

        //#region OnFirmwareStatusNotification        (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnFirmwareStatusNotificationRequestSentDelegate?            OnFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a FirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnFirmwareStatusNotificationResponseReceivedDelegate?           OnFirmwareStatusNotificationResponse;

        //#endregion

        //#region OnPublishFirmwareStatusNotification (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnPublishFirmwareStatusNotificationRequestSentDelegate?     OnPublishFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?    OnPublishFirmwareStatusNotificationResponse;

        //#endregion

        //#region OnHeartbeat                         (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Heartbeat request will be sent to the CSMS.
        ///// </summary>
        //public event OnHeartbeatRequestSentDelegate?                             OnHeartbeatRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Heartbeat request was received.
        ///// </summary>
        //public event OnHeartbeatResponseReceivedDelegate?                            OnHeartbeatResponse;

        //#endregion

        //#region OnNotifyEvent                       (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyEventRequestSentDelegate?                           OnNotifyEventRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEvent request was received.
        ///// </summary>
        //public event OnNotifyEventResponseReceivedDelegate?                          OnNotifyEventResponse;

        //#endregion

        //#region OnSecurityEventNotification         (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnSecurityEventNotificationRequestSentDelegate?             OnSecurityEventNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SecurityEventNotification request was received.
        ///// </summary>
        //public event OnSecurityEventNotificationResponseReceivedDelegate?            OnSecurityEventNotificationResponse;

        //#endregion

        //#region OnNotifyReport                      (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyReport request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyReportRequestSentDelegate?                          OnNotifyReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyReport request was received.
        ///// </summary>
        //public event OnNotifyReportResponseReceivedDelegate?                         OnNotifyReportResponse;

        //#endregion

        //#region OnNotifyMonitoringReport            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyMonitoringReportRequestSentDelegate?                OnNotifyMonitoringReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyMonitoringReport request was received.
        ///// </summary>
        //public event OnNotifyMonitoringReportResponseReceivedDelegate?               OnNotifyMonitoringReportResponse;

        //#endregion

        //#region OnLogStatusNotification             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnLogStatusNotificationRequestSentDelegate?                 OnLogStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a LogStatusNotification request was received.
        ///// </summary>
        //public event OnLogStatusNotificationResponseReceivedDelegate?                OnLogStatusNotificationResponse;

        //#endregion

        //#region OnDataTransfer                      (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DataTransfer request will be sent to the CSMS.
        ///// </summary>
        //public event OnDataTransferRequestSentDelegate?                  OnDataTransferRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a DataTransfer request was received.
        ///// </summary>
        //public event OnDataTransferResponseReceivedDelegate?             OnDataTransferResponseReceived;

        //#endregion


        //#region OnSignCertificate                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SignCertificate request will be sent to the CSMS.
        ///// </summary>
        //public event OnSignCertificateRequestSentDelegate?                       OnSignCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SignCertificate request was received.
        ///// </summary>
        //public event OnSignCertificateResponseReceivedDelegate?                      OnSignCertificateResponse;

        //#endregion

        //#region OnGet15118EVCertificate             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        ///// </summary>
        //public event OnGet15118EVCertificateRequestSentDelegate?                 OnGet15118EVCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Get15118EVCertificate request was received.
        ///// </summary>
        //public event OnGet15118EVCertificateResponseReceivedDelegate?                OnGet15118EVCertificateResponse;

        //#endregion

        //#region OnGetCertificateStatus              (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        ///// </summary>
        //public event OnGetCertificateStatusRequestSentDelegate?                  OnGetCertificateStatusRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetCertificateStatus request was received.
        ///// </summary>
        //public event OnGetCertificateStatusResponseReceivedDelegate?                 OnGetCertificateStatusResponse;

        //#endregion

        //#region OnGetCRL                            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a GetCRL request will be sent to the CSMS.
        ///// </summary>
        //public event OnGetCRLRequestSentDelegate?                                OnGetCRLRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetCRL request was received.
        ///// </summary>
        //public event OnGetCRLResponseReceivedDelegate?                               OnGetCRLResponse;

        //#endregion


        //#region OnReservationStatusUpdate           (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        ///// </summary>
        //public event OnReservationStatusUpdateRequestSentDelegate?               OnReservationStatusUpdateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ReservationStatusUpdate request was received.
        ///// </summary>
        //public event OnReservationStatusUpdateResponseReceivedDelegate?              OnReservationStatusUpdateResponse;

        //#endregion

        //#region OnAuthorize                         (Request/-Response)

        ///// <summary>
        ///// An event fired whenever an Authorize request will be sent to the CSMS.
        ///// </summary>
        //public event OnAuthorizeRequestSentDelegate?                             OnAuthorizeRequest;

        ///// <summary>
        ///// An event fired whenever a response to an Authorize request was received.
        ///// </summary>
        //public event OnAuthorizeResponseReceivedDelegate?                            OnAuthorizeResponse;

        //#endregion

        //#region OnNotifyEVChargingNeeds             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyEVChargingNeedsRequestSentDelegate?                 OnNotifyEVChargingNeedsRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        ///// </summary>
        //public event OnNotifyEVChargingNeedsResponseReceivedDelegate?                OnNotifyEVChargingNeedsResponse;

        //#endregion

        //#region OnTransactionEvent                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a TransactionEvent will be sent to the CSMS.
        ///// </summary>
        //public event OnTransactionEventRequestSentDelegate?                      OnTransactionEventRequest;

        ///// <summary>
        ///// An event fired whenever a response to a TransactionEvent request was received.
        ///// </summary>
        //public event OnTransactionEventResponseReceivedDelegate?                     OnTransactionEventResponse;

        //#endregion

        //#region OnStatusNotification                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a StatusNotification request will be sent to the CSMS.
        ///// </summary>
        //public event OnStatusNotificationRequestSentDelegate?                    OnStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a StatusNotification request was received.
        ///// </summary>
        //public event OnStatusNotificationResponseReceivedDelegate?                   OnStatusNotificationResponse;

        //#endregion

        //#region OnMeterValues                       (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a MeterValues request will be sent to the CSMS.
        ///// </summary>
        //public event OnMeterValuesRequestSentDelegate?                           OnMeterValuesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a MeterValues request was received.
        ///// </summary>
        //public event OnMeterValuesResponseReceivedDelegate?                          OnMeterValuesResponse;

        //#endregion

        //#region OnNotifyChargingLimit               (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyChargingLimitRequestSentDelegate?                   OnNotifyChargingLimitRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyChargingLimit request was received.
        ///// </summary>
        //public event OnNotifyChargingLimitResponseReceivedDelegate?                  OnNotifyChargingLimitResponse;

        //#endregion

        //#region OnClearedChargingLimit              (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        ///// </summary>
        //public event OnClearedChargingLimitRequestSentDelegate?                  OnClearedChargingLimitRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearedChargingLimit request was received.
        ///// </summary>
        //public event OnClearedChargingLimitResponseReceivedDelegate?                 OnClearedChargingLimitResponse;

        //#endregion

        //#region OnReportChargingProfiles            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        ///// </summary>
        //public event OnReportChargingProfilesRequestSentDelegate?                OnReportChargingProfilesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ReportChargingProfiles request was received.
        ///// </summary>
        //public event OnReportChargingProfilesResponseReceivedDelegate?               OnReportChargingProfilesResponse;

        //#endregion

        //#region OnNotifyEVChargingSchedule          (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyEVChargingScheduleRequestSentDelegate?              OnNotifyEVChargingScheduleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        ///// </summary>
        //public event OnNotifyEVChargingScheduleResponseReceivedDelegate?             OnNotifyEVChargingScheduleResponse;

        //#endregion

        //#region NotifyPriorityCharging              (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyPriorityChargingRequestSentDelegate?                OnNotifyPriorityChargingRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyPriorityCharging request was received.
        ///// </summary>
        //public event OnNotifyPriorityChargingResponseReceivedDelegate?               OnNotifyPriorityChargingResponse;

        //#endregion

        //#region NotifySettlement                    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifySettlement request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifySettlementRequestSentDelegate?                    OnNotifySettlementRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifySettlement request was received.
        ///// </summary>
        //public event OnNotifySettlementResponseReceivedDelegate?               OnNotifySettlementResponse;

        //#endregion

        //#region PullDynamicScheduleUpdate           (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        ///// </summary>
        //public event OnPullDynamicScheduleUpdateRequestSentDelegate?             OnPullDynamicScheduleUpdateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        ///// </summary>
        //public event OnPullDynamicScheduleUpdateResponseReceivedDelegate?            OnPullDynamicScheduleUpdateResponse;

        //#endregion


        //#region NotifyDisplayMessages               (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyDisplayMessagesRequestSentDelegate?                 OnNotifyDisplayMessagesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyDisplayMessages request was received.
        ///// </summary>
        //public event OnNotifyDisplayMessagesResponseReceivedDelegate?                OnNotifyDisplayMessagesResponse;

        //#endregion

        //#region NotifyCustomerInformation           (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        ///// </summary>
        //public event OnNotifyCustomerInformationRequestSentDelegate?             OnNotifyCustomerInformationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyCustomerInformation request was received.
        ///// </summary>
        //public event OnNotifyCustomerInformationResponseReceivedDelegate?            OnNotifyCustomerInformationResponse;

        //#endregion


        //// Binary Data Streams Extensions

        //#region TransferBinaryData                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        ///// </summary>
        //public event OnBinaryDataTransferRequestSentDelegate?            OnBinaryDataTransferRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a BinaryDataTransfer request was received.
        ///// </summary>
        //public event OnBinaryDataTransferResponseReceivedDelegate?           OnBinaryDataTransferResponseReceived;

        //#endregion

        //#region GetFile                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a GetFile request will be sent to the CSMS.
        ///// </summary>
        //public event OnGetFileRequestSentDelegate?         OnGetFileRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a GetFile request was received.
        ///// </summary>
        //public event OnGetFileResponseReceivedDelegate?    OnGetFileResponseReceived;

        //#endregion

        //#region SendFile                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SendFile request will be sent to the CSMS.
        ///// </summary>
        //public event OnSendFileRequestSentDelegate?         OnSendFileRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a SendFile request was received.
        ///// </summary>
        //public event OnSendFileResponseReceivedDelegate?    OnSendFileResponseReceived;

        //#endregion

        //#region DeleteFile                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DeleteFile request will be sent to the CSMS.
        ///// </summary>
        //public event OnDeleteFileRequestSentDelegate? OnDeleteFileRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteFile request was received.
        ///// </summary>
        //public event OnDeleteFileResponseReceivedDelegate? OnDeleteFileResponseReceived;

        //#endregion

        //#region ListDirectory                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a ListDirectory request will be sent to the CSMS.
        ///// </summary>
        //public event OnListDirectoryRequestSentDelegate? OnListDirectoryRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a ListDirectory request was received.
        ///// </summary>
        //public event OnListDirectoryResponseReceivedDelegate? OnListDirectoryResponseReceived;

        //#endregion


        //// E2E Security Extensions

        //#region AddSignaturePolicy                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a AddSignaturePolicy request will be sent to the CSMS.
        ///// </summary>
        //public event OnAddSignaturePolicyRequestSentDelegate?         OnAddSignaturePolicyRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a AddSignaturePolicy request was received.
        ///// </summary>
        //public event OnAddSignaturePolicyResponseReceivedDelegate?    OnAddSignaturePolicyResponseReceived;

        //#endregion

        //#region UpdateSignaturePolicy                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a UpdateSignaturePolicy request will be sent to the CSMS.
        ///// </summary>
        //public event OnUpdateSignaturePolicyRequestSentDelegate? OnUpdateSignaturePolicyRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        ///// </summary>
        //public event OnUpdateSignaturePolicyResponseReceivedDelegate? OnUpdateSignaturePolicyResponseReceived;

        //#endregion

        //#region DeleteSignaturePolicy                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DeleteSignaturePolicy request will be sent to the CSMS.
        ///// </summary>
        //public event OnDeleteSignaturePolicyRequestSentDelegate? OnDeleteSignaturePolicyRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        ///// </summary>
        //public event OnDeleteSignaturePolicyResponseReceivedDelegate? OnDeleteSignaturePolicyResponseReceived;

        //#endregion

        //#region AddUserRole                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a AddUserRole request will be sent to the CSMS.
        ///// </summary>
        //public event OnAddUserRoleRequestSentDelegate? OnAddUserRoleRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a AddUserRole request was received.
        ///// </summary>
        //public event OnAddUserRoleResponseReceivedDelegate? OnAddUserRoleResponseReceived;

        //#endregion

        //#region UpdateUserRole                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a UpdateUserRole request will be sent to the CSMS.
        ///// </summary>
        //public event OnUpdateUserRoleRequestSentDelegate? OnUpdateUserRoleRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateUserRole request was received.
        ///// </summary>
        //public event OnUpdateUserRoleResponseReceivedDelegate? OnUpdateUserRoleResponseReceived;

        //#endregion

        //#region DeleteUserRole                             (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DeleteUserRole request will be sent to the CSMS.
        ///// </summary>
        //public event OnDeleteUserRoleRequestSentDelegate? OnDeleteUserRoleRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteUserRole request was received.
        ///// </summary>
        //public event OnDeleteUserRoleResponseReceivedDelegate? OnDeleteUserRoleResponseReceived;

        //#endregion


        //#region TransferSecureData                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SecureDataTransfer request will be sent to the CSMS.
        ///// </summary>
        //public event OnSecureDataTransferRequestSentDelegate? OnSecureDataTransferRequestSent;

        ///// <summary>
        ///// An event fired whenever a response to a SecureDataTransfer request was received.
        ///// </summary>
        //public event OnSecureDataTransferResponseReceivedDelegate? OnSecureDataTransferResponseReceived;

        //#endregion

        //#endregion

        //#region Incoming Messages: Charging Station <-(NN)-- CSMS

        //#region Reset  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Reset request was received from the CSMS.
        ///// </summary>
        //public event OnResetRequestReceivedDelegate?   OnResetRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Reset request was sent.
        ///// </summary>
        //public event OnResetResponseSentDelegate?  OnResetResponse;

        //#endregion

        //#region UpdateFirmware

        ///// <summary>
        ///// An event fired whenever an UpdateFirmware request was received from the CSMS.
        ///// </summary>
        //public event OnUpdateFirmwareRequestReceivedDelegate?   OnUpdateFirmwareRequest;

        ///// <summary>
        ///// An event fired whenever a response to an UpdateFirmware request was sent.
        ///// </summary>
        //public event OnUpdateFirmwareResponseSentDelegate?  OnUpdateFirmwareResponse;

        //#endregion

        //#region PublishFirmware

        ///// <summary>
        ///// An event fired whenever a PublishFirmware request was received from the CSMS.
        ///// </summary>
        //public event OnPublishFirmwareRequestReceivedDelegate?   OnPublishFirmwareRequest;

        ///// <summary>
        ///// An event fired whenever a response to a PublishFirmware request was sent.
        ///// </summary>
        //public event OnPublishFirmwareResponseSentDelegate?  OnPublishFirmwareResponse;

        //#endregion

        //#region UnpublishFirmware

        ///// <summary>
        ///// An event fired whenever an UnpublishFirmware request was received from the CSMS.
        ///// </summary>
        //public event OnUnpublishFirmwareRequestReceivedDelegate?   OnUnpublishFirmwareRequest;

        ///// <summary>
        ///// An event fired whenever a response to an UnpublishFirmware request was sent.
        ///// </summary>
        //public event OnUnpublishFirmwareResponseSentDelegate?  OnUnpublishFirmwareResponse;

        //#endregion

        //#region GetBaseReport

        ///// <summary>
        ///// An event fired whenever a GetBaseReport request was received from the CSMS.
        ///// </summary>
        //public event OnGetBaseReportRequestReceivedDelegate?   OnGetBaseReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetBaseReport request was sent.
        ///// </summary>
        //public event OnGetBaseReportResponseSentDelegate?  OnGetBaseReportResponse;

        //#endregion

        //#region GetReport

        ///// <summary>
        ///// An event fired whenever a GetReport request was received from the CSMS.
        ///// </summary>
        //public event OnGetReportRequestReceivedDelegate?   OnGetReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetReport request was sent.
        ///// </summary>
        //public event OnGetReportResponseSentDelegate?  OnGetReportResponse;

        //#endregion

        //#region GetLog

        ///// <summary>
        ///// An event fired whenever a GetLog request was received from the CSMS.
        ///// </summary>
        //public event OnGetLogRequestReceivedDelegate?   OnGetLogRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetLog request was sent.
        ///// </summary>
        //public event OnGetLogResponseSentDelegate?  OnGetLogResponse;

        //#endregion

        //#region SetVariables

        ///// <summary>
        ///// An event fired whenever a SetVariables request was received from the CSMS.
        ///// </summary>
        //public event OnSetVariablesRequestReceivedDelegate?   OnSetVariablesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetVariables request was sent.
        ///// </summary>
        //public event OnSetVariablesResponseSentDelegate?  OnSetVariablesResponse;

        //#endregion

        //#region GetVariables

        ///// <summary>
        ///// An event fired whenever a GetVariables request was received from the CSMS.
        ///// </summary>
        //public event OnGetVariablesRequestReceivedDelegate?   OnGetVariablesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetVariables request was sent.
        ///// </summary>
        //public event OnGetVariablesResponseSentDelegate?  OnGetVariablesResponse;

        //#endregion

        //#region SetMonitoringBase

        ///// <summary>
        ///// An event fired whenever a SetMonitoringBase request was received from the CSMS.
        ///// </summary>
        //public event OnSetMonitoringBaseRequestReceivedDelegate?   OnSetMonitoringBaseRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetMonitoringBase request was sent.
        ///// </summary>
        //public event OnSetMonitoringBaseResponseSentDelegate?  OnSetMonitoringBaseResponse;

        //#endregion

        //#region GetMonitoringReport

        ///// <summary>
        ///// An event fired whenever a GetMonitoringReport request was received from the CSMS.
        ///// </summary>
        //public event OnGetMonitoringReportRequestReceivedDelegate?   OnGetMonitoringReportRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetMonitoringReport request was sent.
        ///// </summary>
        //public event OnGetMonitoringReportResponseSentDelegate?  OnGetMonitoringReportResponse;

        //#endregion

        //#region SetMonitoringLevel

        ///// <summary>
        ///// An event fired whenever a SetMonitoringLevel request was received from the CSMS.
        ///// </summary>
        //public event OnSetMonitoringLevelRequestReceivedDelegate?   OnSetMonitoringLevelRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetMonitoringLevel request was sent.
        ///// </summary>
        //public event OnSetMonitoringLevelResponseSentDelegate?  OnSetMonitoringLevelResponse;

        //#endregion

        //#region SetVariableMonitoring

        ///// <summary>
        ///// An event fired whenever a SetVariableMonitoring request was received from the CSMS.
        ///// </summary>
        //public event OnSetVariableMonitoringRequestReceivedDelegate?   OnSetVariableMonitoringRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetVariableMonitoring request was sent.
        ///// </summary>
        //public event OnSetVariableMonitoringResponseSentDelegate?  OnSetVariableMonitoringResponse;

        //#endregion

        //#region ClearVariableMonitoring

        ///// <summary>
        ///// An event fired whenever a ClearVariableMonitoring request was received from the CSMS.
        ///// </summary>
        //public event OnClearVariableMonitoringRequestReceivedDelegate?   OnClearVariableMonitoringRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearVariableMonitoring request was sent.
        ///// </summary>
        //public event OnClearVariableMonitoringResponseSentDelegate?  OnClearVariableMonitoringResponse;

        //#endregion

        //#region SetNetworkProfile

        ///// <summary>
        ///// An event fired whenever a SetNetworkProfile request was received from the CSMS.
        ///// </summary>
        //public event OnSetNetworkProfileRequestReceivedDelegate?   OnSetNetworkProfileRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetNetworkProfile request was sent.
        ///// </summary>
        //public event OnSetNetworkProfileResponseSentDelegate?  OnSetNetworkProfileResponse;

        //#endregion

        //#region ChangeAvailability

        ///// <summary>
        ///// An event fired whenever a ChangeAvailability request was received from the CSMS.
        ///// </summary>
        //public event OnChangeAvailabilityRequestReceivedDelegate?   OnChangeAvailabilityRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ChangeAvailability request was sent.
        ///// </summary>
        //public event OnChangeAvailabilityResponseSentDelegate?  OnChangeAvailabilityResponse;

        //#endregion

        //#region TriggerMessage

        ///// <summary>
        ///// An event fired whenever a TriggerMessage request was received from the CSMS.
        ///// </summary>
        //public event OnTriggerMessageRequestReceivedDelegate?   OnTriggerMessageRequest;

        ///// <summary>
        ///// An event fired whenever a response to a TriggerMessage request was sent.
        ///// </summary>
        //public event OnTriggerMessageResponseSentDelegate?  OnTriggerMessageResponse;

        //#endregion

        //#region OnIncomingDataTransferRequest/-Response

        ///// <summary>
        ///// An event sent whenever a data transfer request was sent.
        ///// </summary>
        //public event OnDataTransferRequestReceivedDelegate?   OnDataTransferRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a data transfer request was sent.
        ///// </summary>
        //public event OnDataTransferResponseSentDelegate?      OnDataTransferResponseSent;

        //#endregion


        //#region SendSignedCertificate

        ///// <summary>
        ///// An event fired whenever a SignedCertificate request was received from the CSMS.
        ///// </summary>
        //public event OnCertificateSignedRequestReceivedDelegate?   OnCertificateSignedRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SignedCertificate request was sent.
        ///// </summary>
        //public event OnCertificateSignedResponseSentDelegate?  OnCertificateSignedResponse;

        //#endregion

        //#region InstallCertificate

        ///// <summary>
        ///// An event fired whenever an InstallCertificate request was received from the CSMS.
        ///// </summary>
        //public event OnInstallCertificateRequestReceivedDelegate?   OnInstallCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to an InstallCertificate request was sent.
        ///// </summary>
        //public event OnInstallCertificateResponseSentDelegate?  OnInstallCertificateResponse;

        //#endregion

        //#region GetInstalledCertificateIds

        ///// <summary>
        ///// An event fired whenever a GetInstalledCertificateIds request was received from the CSMS.
        ///// </summary>
        //public event OnGetInstalledCertificateIdsRequestReceivedDelegate?   OnGetInstalledCertificateIdsRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetInstalledCertificateIds request was sent.
        ///// </summary>
        //public event OnGetInstalledCertificateIdsResponseSentDelegate?  OnGetInstalledCertificateIdsResponse;

        //#endregion

        //#region DeleteCertificate

        ///// <summary>
        ///// An event fired whenever a DeleteCertificate request was received from the CSMS.
        ///// </summary>
        //public event OnDeleteCertificateRequestReceivedDelegate?   OnDeleteCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteCertificate request was sent.
        ///// </summary>
        //public event OnDeleteCertificateResponseSentDelegate?  OnDeleteCertificateResponse;

        //#endregion

        //#region NotifyCRL

        ///// <summary>
        ///// An event fired whenever a NotifyCRL request was received from the CSMS.
        ///// </summary>
        //public event OnNotifyCRLRequestReceivedDelegate?   OnNotifyCRLRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyCRL request was sent.
        ///// </summary>
        //public event OnNotifyCRLResponseSentDelegate?  OnNotifyCRLResponse;

        //#endregion


        //#region GetLocalListVersion

        ///// <summary>
        ///// An event fired whenever a GetLocalListVersion request was received from the CSMS.
        ///// </summary>
        //public event OnGetLocalListVersionRequestReceivedDelegate?   OnGetLocalListVersionRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetLocalListVersion request was sent.
        ///// </summary>
        //public event OnGetLocalListVersionResponseSentDelegate?  OnGetLocalListVersionResponse;

        //#endregion

        //#region SendLocalList

        ///// <summary>
        ///// An event fired whenever a SendLocalList request was received from the CSMS.
        ///// </summary>
        //public event OnSendLocalListRequestReceivedDelegate?   OnSendLocalListRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SendLocalList request was sent.
        ///// </summary>
        //public event OnSendLocalListResponseSentDelegate?  OnSendLocalListResponse;

        //#endregion

        //#region ClearCache

        ///// <summary>
        ///// An event fired whenever a ClearCache request was received from the CSMS.
        ///// </summary>
        //public event OnClearCacheRequestReceivedDelegate?   OnClearCacheRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearCache request was sent.
        ///// </summary>
        //public event OnClearCacheResponseSentDelegate?  OnClearCacheResponse;

        //#endregion


        //#region QRCodeScanned

        ///// <summary>
        ///// An event fired whenever a QRCodeScanned request was received from the CSMS.
        ///// </summary>
        //public event OnQRCodeScannedRequestReceivedDelegate?   OnQRCodeScannedRequest;

        ///// <summary>
        ///// An event fired whenever a response to a QRCodeScanned request was sent.
        ///// </summary>
        //public event OnQRCodeScannedResponseSentDelegate?      OnQRCodeScannedResponse;

        //#endregion

        //#region ReserveNow

        ///// <summary>
        ///// An event fired whenever a ReserveNow request was received from the CSMS.
        ///// </summary>
        //public event OnReserveNowRequestReceivedDelegate?   OnReserveNowRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ReserveNow request was sent.
        ///// </summary>
        //public event OnReserveNowResponseSentDelegate?      OnReserveNowResponse;

        //#endregion

        //#region CancelReservation

        ///// <summary>
        ///// An event fired whenever a CancelReservation request was received from the CSMS.
        ///// </summary>
        //public event OnCancelReservationRequestReceivedDelegate?   OnCancelReservationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a CancelReservation request was sent.
        ///// </summary>
        //public event OnCancelReservationResponseSentDelegate?  OnCancelReservationResponse;

        //#endregion

        //#region StartCharging

        ///// <summary>
        ///// An event fired whenever a RequestStartTransaction request was received from the CSMS.
        ///// </summary>
        //public event OnRequestStartTransactionRequestReceivedDelegate?   OnRequestStartTransactionRequest;

        ///// <summary>
        ///// An event fired whenever a response to a RequestStartTransaction request was sent.
        ///// </summary>
        //public event OnRequestStartTransactionResponseSentDelegate?  OnRequestStartTransactionResponse;

        //#endregion

        //#region StopCharging

        ///// <summary>
        ///// An event fired whenever a RequestStopTransaction request was received from the CSMS.
        ///// </summary>
        //public event OnRequestStopTransactionRequestReceivedDelegate?   OnRequestStopTransactionRequest;

        ///// <summary>
        ///// An event fired whenever a response to a RequestStopTransaction request was sent.
        ///// </summary>
        //public event OnRequestStopTransactionResponseSentDelegate?  OnRequestStopTransactionResponse;

        //#endregion

        //#region GetTransactionStatus

        ///// <summary>
        ///// An event fired whenever a GetTransactionStatus request was received from the CSMS.
        ///// </summary>
        //public event OnGetTransactionStatusRequestReceivedDelegate?   OnGetTransactionStatusRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetTransactionStatus request was sent.
        ///// </summary>
        //public event OnGetTransactionStatusResponseSentDelegate?  OnGetTransactionStatusResponse;

        //#endregion

        //#region SetChargingProfile

        ///// <summary>
        ///// An event fired whenever a SetChargingProfile request was received from the CSMS.
        ///// </summary>
        //public event OnSetChargingProfileRequestReceivedDelegate?   OnSetChargingProfileRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetChargingProfile request was sent.
        ///// </summary>
        //public event OnSetChargingProfileResponseSentDelegate?  OnSetChargingProfileResponse;

        //#endregion

        //#region GetChargingProfiles

        ///// <summary>
        ///// An event fired whenever a GetChargingProfiles request was received from the CSMS.
        ///// </summary>
        //public event OnGetChargingProfilesRequestReceivedDelegate?   OnGetChargingProfilesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetChargingProfiles request was sent.
        ///// </summary>
        //public event OnGetChargingProfilesResponseSentDelegate?  OnGetChargingProfilesResponse;

        //#endregion

        //#region ClearChargingProfile

        ///// <summary>
        ///// An event fired whenever a ClearChargingProfile request was received from the CSMS.
        ///// </summary>
        //public event OnClearChargingProfileRequestReceivedDelegate?   OnClearChargingProfileRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearChargingProfile request was sent.
        ///// </summary>
        //public event OnClearChargingProfileResponseSentDelegate?  OnClearChargingProfileResponse;

        //#endregion

        //#region GetCompositeSchedule

        ///// <summary>
        ///// An event fired whenever a GetCompositeSchedule request was received from the CSMS.
        ///// </summary>
        //public event OnGetCompositeScheduleRequestReceivedDelegate?   OnGetCompositeScheduleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetCompositeSchedule request was sent.
        ///// </summary>
        //public event OnGetCompositeScheduleResponseSentDelegate?  OnGetCompositeScheduleResponse;

        //#endregion

        //#region UpdateDynamicSchedule

        ///// <summary>
        ///// An event fired whenever an UpdateDynamicSchedule request was received from the CSMS.
        ///// </summary>
        //public event OnUpdateDynamicScheduleRequestReceivedDelegate?   OnUpdateDynamicScheduleRequest;

        ///// <summary>
        ///// An event fired whenever a response to an UpdateDynamicSchedule request was sent.
        ///// </summary>
        //public event OnUpdateDynamicScheduleResponseSentDelegate?  OnUpdateDynamicScheduleResponse;

        //#endregion

        //#region NotifyAllowedEnergyTransfer

        ///// <summary>
        ///// An event fired whenever a NotifyAllowedEnergyTransfer request was received from the CSMS.
        ///// </summary>
        //public event OnNotifyAllowedEnergyTransferRequestReceivedDelegate?   OnNotifyAllowedEnergyTransferRequest;

        ///// <summary>
        ///// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        ///// </summary>
        //public event OnNotifyAllowedEnergyTransferResponseSentDelegate?  OnNotifyAllowedEnergyTransferResponse;

        //#endregion

        //#region UsePriorityCharging

        ///// <summary>
        ///// An event fired whenever a UsePriorityCharging request was received from the CSMS.
        ///// </summary>
        //public event OnUsePriorityChargingRequestReceivedDelegate?   OnUsePriorityChargingRequest;

        ///// <summary>
        ///// An event fired whenever a response to a UsePriorityCharging request was sent.
        ///// </summary>
        //public event OnUsePriorityChargingResponseSentDelegate?  OnUsePriorityChargingResponse;

        //#endregion

        //#region UnlockConnector

        ///// <summary>
        ///// An event fired whenever an UnlockConnector request was received from the CSMS.
        ///// </summary>
        //public event OnUnlockConnectorRequestReceivedDelegate?   OnUnlockConnectorRequest;

        ///// <summary>
        ///// An event fired whenever a response to an UnlockConnector request was sent.
        ///// </summary>
        //public event OnUnlockConnectorResponseSentDelegate?  OnUnlockConnectorResponse;

        //#endregion


        //#region AFRRSignal

        ///// <summary>
        ///// An event fired whenever an AFRR signal request was received from the CSMS.
        ///// </summary>
        //public event OnAFRRSignalRequestReceivedDelegate?   OnAFRRSignalRequest;

        ///// <summary>
        ///// An event fired whenever a response to an AFRR signal request was sent.
        ///// </summary>
        //public event OnAFRRSignalResponseSentDelegate?  OnAFRRSignalResponse;

        //#endregion


        //#region SetDisplayMessage

        ///// <summary>
        ///// An event fired whenever a SetDisplayMessage request was received from the CSMS.
        ///// </summary>
        //public event OnSetDisplayMessageRequestReceivedDelegate?   OnSetDisplayMessageRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetDisplayMessage request was sent.
        ///// </summary>
        //public event OnSetDisplayMessageResponseSentDelegate?  OnSetDisplayMessageResponse;

        //#endregion

        //#region GetDisplayMessages

        ///// <summary>
        ///// An event fired whenever a GetDisplayMessages request was received from the CSMS.
        ///// </summary>
        //public event OnGetDisplayMessagesRequestReceivedDelegate?   OnGetDisplayMessagesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetDisplayMessages request was sent.
        ///// </summary>
        //public event OnGetDisplayMessagesResponseSentDelegate?  OnGetDisplayMessagesResponse;

        //#endregion

        //#region ClearDisplayMessage

        ///// <summary>
        ///// An event fired whenever a ClearDisplayMessage request was received from the CSMS.
        ///// </summary>
        //public event OnClearDisplayMessageRequestReceivedDelegate?   OnClearDisplayMessageRequest;

        ///// <summary>
        ///// An event fired whenever a response to a ClearDisplayMessage request was sent.
        ///// </summary>
        //public event OnClearDisplayMessageResponseSentDelegate?  OnClearDisplayMessageResponse;

        //#endregion

        //#region SendCostUpdated

        ///// <summary>
        ///// An event fired whenever a CostUpdated request was received from the CSMS.
        ///// </summary>
        //public event OnCostUpdatedRequestReceivedDelegate?   OnCostUpdatedRequest;

        ///// <summary>
        ///// An event fired whenever a response to a CostUpdated request was sent.
        ///// </summary>
        //public event OnCostUpdatedResponseSentDelegate?  OnCostUpdatedResponse;

        //#endregion

        //#region RequestCustomerInformation

        ///// <summary>
        ///// An event fired whenever a CustomerInformation request was received from the CSMS.
        ///// </summary>
        //public event OnCustomerInformationRequestReceivedDelegate?   OnCustomerInformationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a CustomerInformation request was sent.
        ///// </summary>
        //public event OnCustomerInformationResponseSentDelegate?  OnCustomerInformationResponse;

        //#endregion


        //// Binary Data Streams Extensions

        //#region OnIncomingBinaryDataTransferRequest/-Response

        ///// <summary>
        ///// An event sent whenever a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnBinaryDataTransferRequestReceivedDelegate?   OnIncomingBinaryDataTransferRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnBinaryDataTransferResponseSentDelegate?  OnIncomingBinaryDataTransferResponseSent;

        //#endregion

        //#region OnGetFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a GetFile request was sent.
        ///// </summary>
        //public event OnGetFileRequestReceivedDelegate?   OnGetFileRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a GetFile request was sent.
        ///// </summary>
        //public event OnGetFileResponseSentDelegate?  OnGetFileResponseSent;

        //#endregion

        //#region OnSendFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a SendFile request was sent.
        ///// </summary>
        //public event OnSendFileRequestReceivedDelegate?   OnSendFileRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a SendFile request was sent.
        ///// </summary>
        //public event OnSendFileResponseSentDelegate?  OnSendFileResponseSent;

        //#endregion

        //#region OnDeleteFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a DeleteFile request was sent.
        ///// </summary>
        //public event OnDeleteFileRequestReceivedDelegate?   OnDeleteFileRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a DeleteFile request was sent.
        ///// </summary>
        //public event OnDeleteFileResponseSentDelegate?  OnDeleteFileResponseSent;

        //#endregion

        //#region OnListDirectoryRequest/-Response

        ///// <summary>
        ///// An event sent whenever a ListDirectory request was sent.
        ///// </summary>
        //public event OnListDirectoryRequestReceivedDelegate?   OnListDirectoryRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to a ListDirectory request was sent.
        ///// </summary>
        //public event OnListDirectoryResponseSentDelegate?  OnListDirectoryResponseSent;

        //#endregion


        //// E2E Security Extensions

        //#region AddSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a AddSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OnAddSignaturePolicyRequestReceivedDelegate?   OnAddSignaturePolicyRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a AddSignaturePolicy request was sent.
        ///// </summary>
        //public event OnAddSignaturePolicyResponseSentDelegate?  OnAddSignaturePolicyResponseSent;

        //#endregion

        //#region UpdateSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a UpdateSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OnUpdateSignaturePolicyRequestReceivedDelegate?   OnUpdateSignaturePolicyRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateSignaturePolicy request was sent.
        ///// </summary>
        //public event OnUpdateSignaturePolicyResponseSentDelegate?  OnUpdateSignaturePolicyResponseSent;

        //#endregion

        //#region DeleteSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a DeleteSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OnDeleteSignaturePolicyRequestReceivedDelegate?   OnDeleteSignaturePolicyRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteSignaturePolicy request was sent.
        ///// </summary>
        //public event OnDeleteSignaturePolicyResponseSentDelegate?  OnDeleteSignaturePolicyResponseSent;

        //#endregion

        //#region AddUserRole

        ///// <summary>
        ///// An event fired whenever a AddUserRole request was received from the CSMS.
        ///// </summary>
        //public event OnAddUserRoleRequestReceivedDelegate?   OnAddUserRoleRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a AddUserRole request was sent.
        ///// </summary>
        //public event OnAddUserRoleResponseSentDelegate?  OnAddUserRoleResponseSent;

        //#endregion

        //#region UpdateUserRole

        ///// <summary>
        ///// An event fired whenever a UpdateUserRole request was received from the CSMS.
        ///// </summary>
        //public event OnUpdateUserRoleRequestReceivedDelegate?   OnUpdateUserRoleRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateUserRole request was sent.
        ///// </summary>
        //public event OnUpdateUserRoleResponseSentDelegate?  OnUpdateUserRoleResponseSent;

        //#endregion

        //#region DeleteUserRole

        ///// <summary>
        ///// An event fired whenever a DeleteUserRole request was received from the CSMS.
        ///// </summary>
        //public event OnDeleteUserRoleRequestReceivedDelegate?   OnDeleteUserRoleRequestReceived;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteUserRole request was sent.
        ///// </summary>
        //public event OnDeleteUserRoleResponseSentDelegate?  OnDeleteUserRoleResponseSent;

        //#endregion


        //// E2E Charging Tariffs Extensions

        //#region SetDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a SetDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OnSetDefaultChargingTariffRequestReceivedDelegate?   OnSetDefaultChargingTariffRequest;

        //public event OnSetDefaultChargingTariffDelegate?          OnSetDefaultChargingTariff;

        ///// <summary>
        ///// An event fired whenever a response to a SetDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OnSetDefaultChargingTariffResponseSentDelegate?  OnSetDefaultChargingTariffResponse;

        //#endregion

        //#region GetDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a GetDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OnGetDefaultChargingTariffRequestReceivedDelegate?   OnGetDefaultChargingTariffRequest;

        //public event OnGetDefaultChargingTariffDelegate?          OnGetDefaultChargingTariff;

        ///// <summary>
        ///// An event fired whenever a response to a GetDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OnGetDefaultChargingTariffResponseSentDelegate?  OnGetDefaultChargingTariffResponse;

        //#endregion

        //#region RemoveDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a RemoveDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OnRemoveDefaultChargingTariffRequestReceivedDelegate?   OnRemoveDefaultChargingTariffRequest;

        //public event OnRemoveDefaultChargingTariffDelegate?          OnRemoveDefaultChargingTariff;

        ///// <summary>
        ///// An event fired whenever a response to a RemoveDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OnRemoveDefaultChargingTariffResponseSentDelegate?  OnRemoveDefaultChargingTariffResponse;

        //#endregion

        //#endregion

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

            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.SerialNumber             = SerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Modem                    = Modem;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

            this.DisableSendHeartbeats    = DisableSendHeartbeats;
            this.SendHeartbeatsEvery      = SendHeartbeatsEvery   ?? DefaultSendHeartbeatsEvery;
            this.SendHeartbeatsTimer      = new Timer(
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

            this.WebAPI             = new WebAPI(
                                          this,
                                          NetworkingNodeAPI,

                                          URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

                                      );

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
