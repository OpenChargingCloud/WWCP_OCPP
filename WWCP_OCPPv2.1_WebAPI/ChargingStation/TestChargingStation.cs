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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_1.CS;
//using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{


    public class ChargingStationConnector
    {

        public Connector_Id  Id    { get; }


        public ChargingStationConnector(Connector_Id Id)
        {
            this.Id = Id;
        }

    }


    /// <summary>
    /// A charging station connector.
    /// </summary>
    public class ChargingStationEVSE
    {

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


        private HashSet<ChargingStationConnector>  connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }


    }



    /// <summary>
    /// A charging station for testing.
    /// </summary>
    public class TestChargingStation : IChargingStation,
                                    //  IChargingStationClientEvents,
                                       IEventSender
    {

        public class EnqueuedRequest
        {

            public enum EnqueuedStatus
            {
                New,
                Processing,
                Finished
            }

            public String          Command           { get; }

            public IRequest        Request           { get; }

            public JObject         RequestJSON       { get; }

            public DateTime        EnqueTimestamp    { get; }

            public EnqueuedStatus  Status            { get; set; }

            public Action<Object>  ResponseAction    { get; }

            public EnqueuedRequest(String          Command,
                                   IRequest        Request,
                                   JObject         RequestJSON,
                                   DateTime        EnqueTimestamp,
                                   EnqueuedStatus  Status,
                                   Action<Object>  ResponseAction)
            {

                this.Command         = Command;
                this.Request         = Request;
                this.RequestJSON     = RequestJSON;
                this.EnqueTimestamp  = EnqueTimestamp;
                this.Status          = Status;
                this.ResponseAction  = ResponseAction;

            }

        }



        #region Data

        private readonly            HashSet<SignaturePolicy>    signaturePolicies           = new();


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
        public                      DNSClient?                  DNSClient                   { get; }

        private                     Int64                       internalRequestId           = 100000;

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a CSMS.
        /// </summary>
        public IChargingStationClient?   CSClient                    { get; private set; }


        public String? ClientCloseMessage
            => CSClient?.ClientCloseMessage;


        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => Id.ToString();



        /// <summary>
        /// The charging station identification.
        /// </summary>
        public ChargingStation_Id       Id                          { get; }

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

        string? IHTTPClient.Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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


        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               SignaturePolicy
            => SignaturePolicies.First();



        // Controlled by the CSMS!

        private readonly Dictionary<EVSE_Id, ChargingStationEVSE> evses;

        public IEnumerable<ChargingStationEVSE> EVSEs
            => evses.Values;

        #endregion

        #region Custom JSON serializer delegates

        #region CSMS Messages

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
        public CustomJObjectSerializerDelegate<CSMS.DataTransferRequest>?                            CustomIncomingDataTransferRequestSerializer                  { get; set; }

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
        public CustomJObjectSerializerDelegate<CSMS.UsePriorityChargingRequest>?                     CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.UnlockConnectorRequest>?                         CustomUnlockConnectorRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.AFRRSignalRequest>?                              CustomAFRRSignalRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.SetDisplayMessageRequest>?                       CustomSetDisplayMessageRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.GetDisplayMessagesRequest>?                      CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.ClearDisplayMessageRequest>?                     CustomClearDisplayMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CostUpdatedRequest>?                             CustomCostUpdatedRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.CustomerInformationRequest>?                     CustomCustomerInformationRequestSerializer                   { get; set; }


        // Responses
        public CustomJObjectSerializerDelegate<CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

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
        public CustomJObjectSerializerDelegate<CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion

        #region Charging Station Messages

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
        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }


        // Responses
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

        #endregion

        #region Data Structures

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
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

        #endregion

        #endregion

        #region Events

        #region Charging Station -> CSMS

        #region SendBootNotification

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the CSMS.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region SendFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region SendPublishFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region SendHeartbeat

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion

        #region NotifyEvent

        /// <summary>
        /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        #endregion

        #region SendSecurityEventNotification

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region NotifyReport

        /// <summary>
        /// An event fired whenever a NotifyReport request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        #endregion

        #region NotifyMonitoringReport

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        #endregion

        #region SendLogStatusNotification

        /// <summary>
        /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region TransferData

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region SignCertificate

        /// <summary>
        /// An event fired whenever a SignCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion

        #region Get15118EVCertificate

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        #endregion

        #region GetCertificateStatus

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        #endregion

        #region GetCRL

        /// <summary>
        /// An event fired whenever a GetCRL request will be sent to the CSMS.
        /// </summary>
        public event OnGetCRLRequestDelegate?   OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OnGetCRLResponseDelegate?  OnGetCRLResponse;

        #endregion


        #region SendReservationStatusUpdate

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        #endregion

        #region Authorize

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region NotifyEVChargingNeeds

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        #endregion

        #region SendTransactionEvent

        /// <summary>
        /// An event fired whenever a TransactionEvent will be sent to the CSMS.
        /// </summary>
        public event OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        #endregion

        #region SendStatusNotification

        /// <summary>
        /// An event fired whenever a StatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region SendMeterValues

        /// <summary>
        /// An event fired whenever a MeterValues request will be sent to the CSMS.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region NotifyChargingLimit

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        #endregion

        #region SendClearedChargingLimit

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        #endregion

        #region ReportChargingProfiles

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        #endregion

        #region NotifyEVChargingSchedule

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        #endregion

        #region NotifyPriorityCharging

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        #endregion

        #region PullDynamicScheduleUpdate

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region NotifyDisplayMessages

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        #endregion

        #region NotifyCustomerInformation

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        #endregion

        #endregion

        #region Charging Station <- CSMS

        public event OnResetDelegate? OnReset;
        public event OnUpdateFirmwareDelegate? OnUpdateFirmware;
        public event OnPublishFirmwareDelegate? OnPublishFirmware;
        public event OnUnpublishFirmwareDelegate? OnUnpublishFirmware;
        public event OnGetBaseReportDelegate? OnGetBaseReport;
        public event OnGetReportDelegate? OnGetReport;
        public event OnGetLogDelegate? OnGetLog;
        public event OnSetVariablesDelegate? OnSetVariables;
        public event OnGetVariablesDelegate? OnGetVariables;
        public event OnSetMonitoringBaseDelegate? OnSetMonitoringBase;
        public event OnGetMonitoringReportDelegate? OnGetMonitoringReport;
        public event OnSetMonitoringLevelDelegate? OnSetMonitoringLevel;
        public event OnSetVariableMonitoringDelegate? OnSetVariableMonitoring;
        public event OnClearVariableMonitoringDelegate? OnClearVariableMonitoring;
        public event OnSetNetworkProfileDelegate? OnSetNetworkProfile;
        public event OnChangeAvailabilityDelegate? OnChangeAvailability;
        public event OnTriggerMessageDelegate? OnTriggerMessage;
        public event OnIncomingDataTransferDelegate? OnIncomingDataTransfer;
        public event OnCertificateSignedDelegate? OnCertificateSigned;
        public event OnInstallCertificateDelegate? OnInstallCertificate;
        public event OnGetInstalledCertificateIdsDelegate? OnGetInstalledCertificateIds;
        public event OnDeleteCertificateDelegate? OnDeleteCertificate;
        public event OnNotifyCRLDelegate? OnNotifyCRL;
        public event OnGetLocalListVersionDelegate? OnGetLocalListVersion;
        public event OnSendLocalListDelegate? OnSendLocalList;
        public event OnClearCacheDelegate? OnClearCache;
        public event OnReserveNowDelegate? OnReserveNow;
        public event OnCancelReservationDelegate? OnCancelReservation;
        public event OnRequestStartTransactionDelegate? OnRequestStartTransaction;
        public event OnRequestStopTransactionDelegate? OnRequestStopTransaction;
        public event OnGetTransactionStatusDelegate? OnGetTransactionStatus;
        public event OnSetChargingProfileDelegate? OnSetChargingProfile;
        public event OnGetChargingProfilesDelegate? OnGetChargingProfiles;
        public event OnClearChargingProfileDelegate? OnClearChargingProfile;
        public event OnGetCompositeScheduleDelegate? OnGetCompositeSchedule;
        public event OnUpdateDynamicScheduleDelegate? OnUpdateDynamicSchedule;
        public event OnNotifyAllowedEnergyTransferDelegate? OnNotifyAllowedEnergyTransfer;
        public event OnUsePriorityChargingDelegate? OnUsePriorityCharging;
        public event OnUnlockConnectorDelegate? OnUnlockConnector;
        public event OnAFRRSignalDelegate? OnAFRRSignal;
        public event OnSetDisplayMessageDelegate? OnSetDisplayMessage;
        public event OnGetDisplayMessagesDelegate? OnGetDisplayMessages;
        public event OnClearDisplayMessageDelegate? OnClearDisplayMessage;
        public event OnCostUpdatedDelegate? OnCostUpdated;
        public event OnCustomerInformationDelegate? OnCustomerInformation;



        #region Reset

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the charging station.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the charging station.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the charging station.
        /// </summary>
        public event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the charging station.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the charging station.
        /// </summary>
        public event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the charging station.
        /// </summary>
        public event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the charging station.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the charging station.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the charging station.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the charging station.
        /// </summary>
        public event OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the charging station.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the charging station.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the charging station.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the charging station.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule

        /// <summary>
        /// An event fired whenever an UpdateDynamicSchedule request will be sent to the charging station.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the charging station.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to the charging station.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the charging station.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the charging station.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

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
        public TestChargingStation(ChargingStation_Id                 Id,
                                   String                             VendorName,
                                   String                             Model,

                                   I18NString?                        Description               = null,
                                   String?                            SerialNumber              = null,
                                   String?                            FirmwareVersion           = null,
                                   Modem?                             Modem                     = null,

                                   IEnumerable<ChargingStationEVSE>?  EVSEs                     = null,

                                   String?                            MeterType                 = null,
                                   String?                            MeterSerialNumber         = null,
                                   String?                            MeterPublicKey            = null,

                                   Boolean                            DisableSendHeartbeats     = false,
                                   TimeSpan?                          SendHeartbeatEvery        = null,

                                   Boolean                            DisableMaintenanceTasks   = false,
                                   TimeSpan?                          MaintenanceEvery          = null,

                                   TimeSpan?                          DefaultRequestTimeout     = null,
                                   IHTTPAuthentication?               HTTPAuthentication        = null,
                                   DNSClient?                         DNSClient                 = null,

                                   SignaturePolicy?                   SignaturePolicy           = null)

        {

            if (Id.        IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id),          "The given charging station identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given charging station vendor must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given charging station model must not be null or empty!");


            this.Id                       = Id;

            this.evses                    = EVSEs is not null && EVSEs.Any()
                                                ? EVSEs.ToDictionary(evse => evse.Id, evse => evse)
                                                : new Dictionary<EVSE_Id, ChargingStationEVSE>();

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
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

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
            this.MaintenanceTimer         = new Timer(
                                                DoMaintenanceSync,
                                                null,
                                                this.MaintenanceEvery,
                                                this.MaintenanceEvery
                                            );

            this.HTTPAuthentication       = HTTPAuthentication;
            this.DNSClient                = DNSClient;

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

        }

        #endregion


        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }


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

            var chargingStationWSClient = new ChargingStationWSClient(
                                              Id,
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

                                              DisableWebSocketPings,
                                              WebSocketPingEvery,
                                              SlowNetworkSimulationDelay,

                                              DisableMaintenanceTasks,
                                              MaintenanceEvery,

                                              LoggingPath,
                                              LoggingContext,
                                              LogfileCreator,
                                              HTTPLogger,
                                              DNSClient ?? this.DNSClient
                                          );

            this.CSClient  = chargingStationWSClient;

            WireEvents(chargingStationWSClient);

            var response = await chargingStationWSClient.Connect();

            return response;

        }

        #endregion

        #region WireEvents(ChargingStationServer)


        private readonly ConcurrentDictionary<DisplayMessage_Id, MessageInfo>     displayMessages   = new ();
        private readonly ConcurrentDictionary<Reservation_Id,    Reservation_Id>  reservations      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Transaction>     transactions      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Decimal>         totalCosts        = new ();
        private readonly ConcurrentDictionary<CertificateUse,    Certificate>     certificates      = new ();

        public void WireEvents(IChargingStationServer ChargingStationServer)
        {

            #region OnReset

            ChargingStationServer.OnReset += async (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                #region Send OnResetRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnResetRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnResetRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnResetRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ResetResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ResetResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid reset request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomResetRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new ResetResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming '{request.ResetType}' reset request{(request.EVSEId.HasValue ? $" for EVSE '{request.EVSEId}" : "")}'!");

                        // ResetType

                        // Reset entire charging station
                        if (!request.EVSEId.HasValue)
                        {

                            response = new ResetResponse(
                                           Request:      request,
                                           Status:       ResetStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                        // Only reset the given EVSE
                        else if (EVSEs.Any(evse => evse.Id == request.EVSEId))
                        {

                            response = new ResetResponse(
                                           Request:      request,
                                           Status:       ResetStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                        // Unknown EVSE
                        else
                        {

                            response = new ResetResponse(
                                           Request:      request,
                                           Status:       ResetStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnResetResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnResetResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnResetResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateFirmware

            ChargingStationServer.OnUpdateFirmware += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                #region Send OnUpdateFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUpdateFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUpdateFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                UpdateFirmwareResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new UpdateFirmwareResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid UpdateFirmware request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new UpdateFirmwareResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming UpdateFirmware request ({request.UpdateFirmwareRequestId}) for '" + request.Firmware.FirmwareURL + "'.");

                        // Firmware,
                        // UpdateFirmwareRequestId
                        // Retries
                        // RetryIntervals

                        response = new UpdateFirmwareResponse(
                                       Request:      request,
                                       Status:       UpdateFirmwareStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnUpdateFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUpdateFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnPublishFirmware

            ChargingStationServer.OnPublishFirmware += async (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnPublishFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPublishFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPublishFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnPublishFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                PublishFirmwareResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new PublishFirmwareResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid PublishFirmware request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomPublishFirmwareRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new PublishFirmwareResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming PublishFirmware request ({request.PublishFirmwareRequestId}) for '" + request.DownloadLocation + "'.");

                        // PublishFirmwareRequestId
                        // DownloadLocation
                        // MD5Checksum
                        // Retries
                        // RetryInterval

                        response = new PublishFirmwareResponse(
                                       Request:      request,
                                       Status:       GenericStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnPublishFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPublishFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnPublishFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnpublishFirmware

            ChargingStationServer.OnUnpublishFirmware += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnUnpublishFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUnpublishFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUnpublishFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnpublishFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                UnpublishFirmwareResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new UnpublishFirmwareResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid UnpublishFirmware request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomUnpublishFirmwareRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new UnpublishFirmwareResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming UnpublishFirmware request for '" + request.MD5Checksum + "'.");

                        // MD5Checksum

                        response = new UnpublishFirmwareResponse(
                                       Request:      request,
                                       Status:       UnpublishFirmwareStatus.Unpublished,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomUnpublishFirmwareResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnUnpublishFirmwareResponse event

                var responseLogger = OnUnpublishFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUnpublishFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnpublishFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetBaseReport

            ChargingStationServer.OnGetBaseReport += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                #region Send OnGetBaseReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetBaseReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetBaseReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetBaseReportRequest),
                                  e
                              );
                    }

                }

                                                                #endregion


                #region Check charging station identification

                GetBaseReportResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetBaseReportResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetBaseReport request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetBaseReportRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetBaseReportResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetBaseReport request ({request.GetBaseReportRequestId}) accepted.");

                        // GetBaseReportRequestId
                        // ReportBase

                        response = new GetBaseReportResponse(
                                       Request:      request,
                                       Status:       GenericDeviceModelStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetBaseReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetBaseReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetBaseReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetReport

            ChargingStationServer.OnGetReport += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                #region Send OnGetReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetReportRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetReportResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetReportResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetReport request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new GetReportResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetReport request ({request.GetReportRequestId}) accepted.");

                        // GetReportRequestId
                        // ComponentCriteria
                        // ComponentVariables

                        response = new GetReportResponse(
                                       Request:      request,
                                       Status:       GenericDeviceModelStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetLog

            ChargingStationServer.OnGetLog += async (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                #region Send OnGetLogRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetLogRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetLogRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLogRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetLogResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetLogResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetLog request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new GetLogResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetLog request ({request.LogRequestId}) accepted.");

                        // LogType
                        // LogRequestId
                        // Log
                        // Retries
                        // RetryInterval

                        response = new GetLogResponse(
                                       Request:      request,
                                       Status:       LogStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetLogResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetLogResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLogResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariables

            ChargingStationServer.OnSetVariables += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnSetVariablesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetVariablesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetVariablesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariablesRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetVariablesResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetVariablesResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetVariables request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new SetVariablesResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetVariables request accepted.");

                        // VariableData

                        response = new SetVariablesResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetVariablesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetVariablesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariablesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetVariables

            ChargingStationServer.OnGetVariables += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnGetVariablesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetVariablesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetVariablesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetVariablesRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetVariablesResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetVariablesResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetVariables request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new GetVariablesResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetVariables request accepted.");

                        // VariableData

                        response = new GetVariablesResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetVariablesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetVariablesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetVariablesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringBase

            ChargingStationServer.OnSetMonitoringBase += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnSetMonitoringBaseRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetMonitoringBaseRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetMonitoringBaseRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringBaseRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetMonitoringBaseResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetMonitoringBaseResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetMonitoringBase request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomSetMonitoringBaseRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new SetMonitoringBaseResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringBase request accepted.");

                        // MonitoringBase

                        response = new SetMonitoringBaseResponse(
                                       Request:      request,
                                       Status:       GenericDeviceModelStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetMonitoringBaseResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetMonitoringBaseResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringBaseResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetMonitoringReport

            ChargingStationServer.OnGetMonitoringReport += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnGetMonitoringReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetMonitoringReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetMonitoringReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetMonitoringReportRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetMonitoringReportResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetMonitoringReportResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetMonitoringReport request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new GetMonitoringReportResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetMonitoringReport request ({request.GetMonitoringReportRequestId}) accepted.");

                        // GetMonitoringReportRequestId
                        // MonitoringCriteria
                        // ComponentVariables

                        response = new GetMonitoringReportResponse(
                                       Request:      request,
                                       Status:       GenericDeviceModelStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetMonitoringReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetMonitoringReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetMonitoringReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringLevel

            ChargingStationServer.OnSetMonitoringLevel += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                #region Send OnSetMonitoringLevelRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetMonitoringLevelRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetMonitoringLevelRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringLevelRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetMonitoringLevelResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetMonitoringLevelResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetMonitoringLevel request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomSetMonitoringLevelRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new SetMonitoringLevelResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringLevel request accepted.");

                        // Severity

                        response = new SetMonitoringLevelResponse(
                                       Request:      request,
                                       Status:       GenericStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetMonitoringLevelResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetMonitoringLevelResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringLevelResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariableMonitoring

            ChargingStationServer.OnSetVariableMonitoring += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    cancellationToken) => {

                #region Send OnSetVariableMonitoringRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetVariableMonitoringRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetVariableMonitoringRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariableMonitoringRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetVariableMonitoringResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetVariableMonitoringResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomSetVariableMonitoringRequestSerializer,
                                 CustomSetMonitoringDataSerializer,
                                 CustomComponentSerializer,
                                 CustomEVSESerializer,
                                 CustomVariableSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new SetVariableMonitoringResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringLevel request accepted.");

                        // MonitoringData

                        response = new SetVariableMonitoringResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetVariableMonitoringResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetVariableMonitoringResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariableMonitoringResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearVariableMonitoring

            ChargingStationServer.OnClearVariableMonitoring += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                #region Send OnClearVariableMonitoringRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearVariableMonitoringRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearVariableMonitoringRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearVariableMonitoringRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ClearVariableMonitoringResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ClearVariableMonitoringResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ClearVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomClearVariableMonitoringRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new ClearVariableMonitoringResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ClearVariableMonitoring request (VariableMonitoringIds: {request.VariableMonitoringIds.AggregateWith(", ")})");

                        // VariableMonitoringIds

                        response = new ClearVariableMonitoringResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnClearVariableMonitoringResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearVariableMonitoringResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearVariableMonitoringResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetNetworkProfile

            ChargingStationServer.OnSetNetworkProfile += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnSetNetworkProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetNetworkProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetNetworkProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetNetworkProfileRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetNetworkProfileResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetNetworkProfileResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetNetworkProfile request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new SetNetworkProfileResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetNetworkProfile request for configuration slot {request.ConfigurationSlot}!");

                        // ConfigurationSlot
                        // NetworkConnectionProfile

                        response = new SetNetworkProfileResponse(
                                       Request:      request,
                                       Status:       SetNetworkProfileStatus.Accepted,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetNetworkProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetNetworkProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetNetworkProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnChangeAvailability

            ChargingStationServer.OnChangeAvailability += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                #region Send OnChangeAvailabilityRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnChangeAvailabilityRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnChangeAvailabilityRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnChangeAvailabilityRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ChangeAvailabilityResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ChangeAvailabilityResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ChangeAvailability request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new ChangeAvailabilityResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ChangeAvailability request {request.OperationalStatus.AsText()}{(request.EVSE is not null ? $" for EVSE '{request.EVSE.Id}'{(request.EVSE.ConnectorId.HasValue ? $"/{request.EVSE.ConnectorId}" : "")}" : "")}!");

                        // OperationalStatus
                        // EVSE

                        // Operational status of the entire charging station
                        if (request.EVSE is null)
                        {

                            response = new ChangeAvailabilityResponse(
                                           Request:      request,
                                           Status:       ChangeAvailabilityStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                        // Operational status for an EVSE and maybe a connector
                        else
                        {

                            var evse = EVSEs.FirstOrDefault(evse => evse.Id == request.EVSE.Id);

                            if (evse is null)
                            {

                                // Unknown EVSE identification
                                response = new ChangeAvailabilityResponse(
                                               Request:      request,
                                               Status:       ChangeAvailabilityStatus.Rejected,
                                               CustomData:   null
                                           );

                            }
                            else
                            {

                                if (request.EVSE.ConnectorId.HasValue &&
                                   !evse.Connectors.Any(connector => connector.Id == request.EVSE.ConnectorId.Value))
                                {

                                    // Unknown connector identification
                                    response = new ChangeAvailabilityResponse(
                                                   Request:      request,
                                                   Status:       ChangeAvailabilityStatus.Rejected,
                                                   CustomData:   null
                                               );

                                }
                                else
                                {

                                    response = new ChangeAvailabilityResponse(
                                                   Request:      request,
                                                   Status:       ChangeAvailabilityStatus.Accepted,
                                                   CustomData:   null
                                               );

                                }

                            }

                        }

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnChangeAvailabilityResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnChangeAvailabilityResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnChangeAvailabilityResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnTriggerMessage

            ChargingStationServer.OnTriggerMessage += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                #region Send OnTriggerMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnTriggerMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnTriggerMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnTriggerMessageRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                TriggerMessageResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new TriggerMessageResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid TriggerMessage request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new TriggerMessageResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming TriggerMessage request for '{request.RequestedMessage.AsText()}'{(request.EVSE is not null ? $" at EVSE '{request.EVSE.Id}'" : "")}!");

                        // RequestedMessage
                        // EVSE

                        _ = Task.Run(async () => {

                            switch (request.RequestedMessage) {

                                case MessageTriggers.BootNotification:
                                    await this.SendBootNotification(
                                              BootReason:   BootReason.Triggered,
                                              CustomData:   null
                                          );
                                    break;

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

                                case MessageTriggers.StatusNotification:
                                    if (request.EVSE is not null)
                                        await this.SendStatusNotification(
                                                  EVSEId:        request.EVSE.Id,
                                                  ConnectorId:   Connector_Id.Parse(1),
                                                  Timestamp:     Timestamp.Now,
                                                  Status:        evses[request.EVSE.Id].Status,
                                                  CustomData:    null
                                              );
                                    break;

                            }

                        },
                        CancellationToken.None);


                        response = request.RequestedMessage switch {

                                       MessageTriggers.BootNotification              or
                                       MessageTriggers.LogStatusNotification         or
                                       MessageTriggers.DiagnosticsStatusNotification or
                                       MessageTriggers.FirmwareStatusNotification    or
                                       //MessageTriggers.Heartbeat                     or
                                       MessageTriggers.SignChargingStationCertificate

                                           => new TriggerMessageResponse(
                                                  request,
                                                  TriggerMessageStatus.Accepted
                                              ),

                                       MessageTriggers.MeterValues or
                                       MessageTriggers.StatusNotification

                                           => request.EVSE is not null
                                                  ? new TriggerMessageResponse(
                                                        request,
                                                        TriggerMessageStatus.Accepted
                                                    )
                                                  : new TriggerMessageResponse(
                                                        request,
                                                        TriggerMessageStatus.Rejected
                                                    ),

                                       _   => new TriggerMessageResponse(
                                                  request,
                                                  TriggerMessageStatus.Rejected
                                              ),

                                   };

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnTriggerMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnTriggerMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnTriggerMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnIncomingDataTransfer

            ChargingStationServer.OnIncomingDataTransfer += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                #region Send OnDataTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnIncomingDataTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnIncomingDataTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnIncomingDataTransferRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                DataTransferResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new DataTransferResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid DataTransfer request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        DebugX.Log($"Charging Station '{Id}': Incoming data transfer request: {request.VendorId}.{request.MessageId ?? "-"}: {request.Data ?? "-"}!");

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

                        if (request.VendorId.ToString() == "GraphDefined OEM")
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

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomIncomingDataTransferResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnDataTransferResponse event

                var responseLogger = OnIncomingDataTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnIncomingDataTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnIncomingDataTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnCertificateSigned

            ChargingStationServer.OnCertificateSigned += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnCertificateSignedRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCertificateSignedRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCertificateSignedRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCertificateSignedRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                CertificateSignedResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new CertificateSignedResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid CertificateSigned request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomCertificateSignedRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new CertificateSignedResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming CertificateSigned request{(request.CertificateType.HasValue ? $"(certificate type: {request.CertificateType.Value.AsText()})" : "")}!");

                        // CertificateChain
                        // CertificateType

                        response = new CertificateSignedResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnCertificateSignedResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCertificateSignedResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCertificateSignedResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnInstallCertificate

            ChargingStationServer.OnInstallCertificate += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                #region Send OnInstallCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnInstallCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnInstallCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnInstallCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                InstallCertificateResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new InstallCertificateResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid InstallCertificate request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomInstallCertificateRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new InstallCertificateResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming InstallCertificate request (certificate type: {request.CertificateType.AsText()}!");

                        // CertificateType
                        // Certificate

                        var success = certificates.AddOrUpdate(request.CertificateType,
                                                                   a    => request.Certificate,
                                                                  (b,c) => request.Certificate);

                        response = new InstallCertificateResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnInstallCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnInstallCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnInstallCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetInstalledCertificateIds

            ChargingStationServer.OnGetInstalledCertificateIds += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         cancellationToken) => {

                #region Send OnGetInstalledCertificateIdsRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetInstalledCertificateIdsRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetInstalledCertificateIdsRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetInstalledCertificateIdsRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetInstalledCertificateIdsResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetInstalledCertificateIdsResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetInstalledCertificateIds request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetInstalledCertificateIdsRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetInstalledCertificateIdsResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetInstalledCertificateIds request for certificate types: {request.CertificateTypes.Select(certificateType => certificateType.AsText()).AggregateWith(", ")}!");

                        // CertificateTypes

                        var certs = new List<CertificateHashData>();

                        foreach (var certificateType in request.CertificateTypes)
                        {

                            if (certificates.TryGetValue(certificateType, out var cert))
                                certs.Add(new CertificateHashData(
                                              HashAlgorithm:         HashAlgorithms.SHA256,
                                              IssuerNameHash:        cert.Parsed?.Issuer               ?? "-",
                                              IssuerPublicKeyHash:   cert.Parsed?.GetPublicKeyString() ?? "-",
                                              SerialNumber:          cert.Parsed?.SerialNumber         ?? "-",
                                              CustomData:            null
                                          ));

                        }

                        response = new GetInstalledCertificateIdsResponse(
                                       Request:                    request,
                                       Status:                     GetInstalledCertificateStatus.Accepted,
                                       CertificateHashDataChain:   certs,
                                       StatusInfo:                 null,
                                       CustomData:                 null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetInstalledCertificateIdsResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetInstalledCertificateIdsResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetInstalledCertificateIdsResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnDeleteCertificate

            ChargingStationServer.OnDeleteCertificate += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnDeleteCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnDeleteCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnDeleteCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnDeleteCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                DeleteCertificateResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new DeleteCertificateResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid DeleteCertificate request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new DeleteCertificateResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming DeleteCertificate request!");

                        // CertificateHashData

                        var certKV  = certificates.FirstOrDefault(certificateKV => request.CertificateHashData.SerialNumber == certificateKV.Value.Parsed?.SerialNumber);

                        var success = certificates.TryRemove(certKV);

                        response = new DeleteCertificateResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnDeleteCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnDeleteCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnDeleteCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyCRL

            ChargingStationServer.OnNotifyCRL += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                #region Send OnNotifyCRLRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyCRLRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyCRLRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyCRLRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                NotifyCRLResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new NotifyCRLResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid NotifyCRL request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomNotifyCRLRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new NotifyCRLResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming NotifyCRL request!");

                        // NotifyCRLRequestId
                        // Availability
                        // Location

                        response = new NotifyCRLResponse(
                                       Request:      request,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyCRLResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnNotifyCRLResponse event

                var responseLogger = OnNotifyCRLResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyCRLResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyCRLResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnGetLocalListVersion

            ChargingStationServer.OnGetLocalListVersion += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnGetLocalListVersionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetLocalListVersionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetLocalListVersionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLocalListVersionRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetLocalListVersionResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetLocalListVersionResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetLocalListVersion request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetLocalListVersionRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetLocalListVersionResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetLocalListVersion request!");

                        // none

                        response = new GetLocalListVersionResponse(
                                       Request:         request,
                                       VersionNumber:   0,
                                       CustomData:      null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomGetLocalListVersionResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnGetLocalListVersionResponse event

                var responseLogger = OnGetLocalListVersionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetLocalListVersionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLocalListVersionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSendLocalList

            ChargingStationServer.OnSendLocalList += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                #region Send OnSendLocalListRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSendLocalListRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSendLocalListRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSendLocalListRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SendLocalListResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SendLocalListResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SendLocalList request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new SendLocalListResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SendLocalList request: '{request.UpdateType.AsText()}' version '{request.VersionNumber}'!");

                        // VersionNumber
                        // UpdateType
                        // LocalAuthorizationList

                        response = new SendLocalListResponse(
                                       Request:      request,
                                       Status:       SendLocalListStatus.Accepted,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSendLocalListResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSendLocalListResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSendLocalListResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearCache

            ChargingStationServer.OnClearCache += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnClearCacheRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearCacheRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearCacheRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearCacheRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ClearCacheResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ClearCacheResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ClearCache request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomClearCacheRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new ClearCacheResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ClearCache request!");

                        // none

                        response = new ClearCacheResponse(
                                       Request:      request,
                                       Status:       ClearCacheStatus.Accepted,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnClearCacheResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearCacheResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearCacheResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnReserveNow

            ChargingStationServer.OnReserveNow += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnReserveNowRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReserveNowRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReserveNowRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnReserveNowRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ReserveNowResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ReserveNowResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ReserveNow request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new ReserveNowResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ReserveNow request (reservation id: {request.Id}, idToken: '{request.IdToken.Value}'{(request.EVSEId.HasValue ? $", evseId: '{request.EVSEId.Value}'" : "")})!");

                        // ReservationId
                        // ExpiryDate
                        // IdToken
                        // ConnectorType
                        // EVSEId
                        // GroupIdToken

                        var success = reservations.TryAdd(request.Id,
                                                          request.Id);

                        response = new ReserveNowResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnReserveNowResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReserveNowResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnReserveNowResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCancelReservation

            ChargingStationServer.OnCancelReservation += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnCancelReservationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCancelReservationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCancelReservationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCancelReservationRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                CancelReservationResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new CancelReservationResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid CancelReservation request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomCancelReservationRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new CancelReservationResponse(
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

                        DebugX.Log($"Charging station '{Id}': Incoming CancelReservation request for reservation id '{request.ReservationId}': {(success ? "accepted" : "rejected")}!");

                        // ReservationId

                        response = new CancelReservationResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnCancelReservationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCancelReservationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCancelReservationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStartTransaction

            ChargingStationServer.OnRequestStartTransaction += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                #region Send OnRequestStartTransactionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnRequestStartTransactionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnRequestStartTransactionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStartTransactionRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                RequestStartTransactionResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new RequestStartTransactionResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid RequestStartTransaction request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer

                             ),
                             out var errorResponse
                         ))
                    {

                        response = new RequestStartTransactionResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming RequestStartTransaction for '{(request.EVSEId?.ToString() ?? "-")}'!");

                        // ToDo: lock(evses)

                        if (request.EVSEId.HasValue &&
                            evses.TryGetValue(request.EVSEId.Value, out var evse) &&
                            !evse.IsCharging)
                        {

                            evse.IsCharging              = true;
                            evse.TransactionId           = Transaction_Id.NewRandom;
                            evse.RemoteStartId           = request.RequestStartTransactionRequestId;

                            evse.StartTimestamp          = Timestamp.Now;
                            evse.MeterStartValue         = 0;
                            evse.SignedStartMeterValue   = "0";

                            evse.StopTimestamp           = null;
                            evse.MeterStopValue          = null;
                            evse.SignedStopMeterValue    = null;

                            evse.IdToken                 = request.IdToken;
                            evse.GroupIdToken            = request.GroupIdToken;
                            evse.ChargingProfile         = request.ChargingProfile;

                            _ = Task.Run(async () => {

                                await Task.Delay(500);

                                await this.SendTransactionEvent(

                                          EventType:            TransactionEvents.Started,
                                          Timestamp:            evse.StartTimestamp.Value,
                                          TriggerReason:        TriggerReasons.RemoteStart,
                                          SequenceNumber:       1,
                                          TransactionInfo:      new Transaction(
                                                                    TransactionId:       evse.TransactionId.Value,
                                                                    ChargingState:       ChargingStates.Charging,
                                                                    TimeSpentCharging:   TimeSpan.Zero,
                                                                    StoppedReason:       null,
                                                                    RemoteStartId:       request.RequestStartTransactionRequestId,
                                                                    CustomData:          null
                                                                ),

                                          Offline:              false,
                                          NumberOfPhasesUsed:   3,
                                          CableMaxCurrent:      Ampere.Parse(32),
                                          ReservationId:        evse.ReservationId,
                                          IdToken:              evse.IdToken,
                                          EVSE:                 new EVSE(
                                                                    Id:            evse.Id,
                                                                    ConnectorId:   evse.Connectors.First().Id,
                                                                    CustomData:    null
                                                                ),
                                          MeterValues:          new[] {
                                                                    new MeterValue(
                                                                        Timestamp:       evse.StartTimestamp.Value,
                                                                        SampledValues:   new[] {
                                                                                             new SampledValue(
                                                                                                 Value:              evse.MeterStartValue.Value,
                                                                                                 Context:            ReadingContexts.TransactionBegin,
                                                                                                 Measurand:          Measurands.Current_Export,
                                                                                                 Phase:              null,
                                                                                                 Location:           MeasurementLocations.Outlet,
                                                                                                 SignedMeterValue:   new SignedMeterValue(
                                                                                                                         SignedMeterData:   evse.SignedStartMeterValue,
                                                                                                                         SigningMethod:     "secp256r1",
                                                                                                                         EncodingMethod:    "base64",
                                                                                                                         PublicKey:         "04cafebabe",
                                                                                                                         CustomData:        null
                                                                                                                     ),
                                                                                                 UnitOfMeasure:      null,
                                                                                                 CustomData:         null
                                                                                             )
                                                                                         }
                                                                    )
                                                                },
                                          CustomData:           null

                                      );

                            },
                            CancellationToken.None);

                            response = new RequestStartTransactionResponse(
                                           Request:         request,
                                           Status:          RequestStartStopStatus.Accepted,
                                           TransactionId:   evse.TransactionId,
                                           StatusInfo:      null,
                                           CustomData:      null
                                       );

                        }
                        else
                            response = new RequestStartTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnRequestStartTransactionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnRequestStartTransactionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStartTransactionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStopTransaction

            ChargingStationServer.OnRequestStopTransaction += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) => {

                #region Send OnRequestStopTransactionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnRequestStopTransactionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnRequestStopTransactionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStopTransactionRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                RequestStopTransactionResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new RequestStopTransactionResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid RequestStopTransaction request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomRequestStopTransactionRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new RequestStopTransactionResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming RequestStopTransaction for '{request.TransactionId}'!");

                        // TransactionId

                        // ToDo: lock(evses)

                        var evse = evses.Values.FirstOrDefault(evse => request.TransactionId == evse.TransactionId);

                        if (evse is not null)
                        {

                            evse.IsCharging             = false;

                            evse.StopTimestamp          = Timestamp.Now;
                            evse.MeterStopValue         = 123;
                            evse.SignedStopMeterValue   = "123";

                            _ = Task.Run(async () => {

                                await this.SendTransactionEvent(

                                          EventType:            TransactionEvents.Ended,
                                          Timestamp:            evse.StopTimestamp.Value,
                                          TriggerReason:        TriggerReasons.RemoteStop,
                                          SequenceNumber:       2,
                                          TransactionInfo:      new Transaction(
                                                                    TransactionId:       evse.TransactionId!.Value,
                                                                    ChargingState:       ChargingStates.Idle,
                                                                    TimeSpentCharging:   evse.StopTimestamp - evse.StartTimestamp,
                                                                    StoppedReason:       StopTransactionReasons.Remote,
                                                                    RemoteStartId:       evse.RemoteStartId,
                                                                    CustomData:          null
                                                                ),

                                          Offline:              false,
                                          NumberOfPhasesUsed:   3,
                                          CableMaxCurrent:      Ampere.Parse(32),
                                          ReservationId:        evse.ReservationId,
                                          IdToken:              evse.IdToken,
                                          EVSE:                 new EVSE(
                                                                    Id:            evse.Id,
                                                                    ConnectorId:   evse.Connectors.First().Id,
                                                                    CustomData:    null
                                                                ),
                                          MeterValues:          new[] {
                                                                    new MeterValue(
                                                                        Timestamp:       evse.StopTimestamp.Value,
                                                                        SampledValues:   new[] {
                                                                                             new SampledValue(
                                                                                                 Value:              evse.MeterStopValue.Value,
                                                                                                 Context:            ReadingContexts.TransactionEnd,
                                                                                                 Measurand:          Measurands.Current_Export,
                                                                                                 Phase:              null,
                                                                                                 Location:           MeasurementLocations.Outlet,
                                                                                                 SignedMeterValue:   new SignedMeterValue(
                                                                                                                         SignedMeterData:   evse.SignedStopMeterValue,
                                                                                                                         SigningMethod:     "secp256r1",
                                                                                                                         EncodingMethod:    "base64",
                                                                                                                         PublicKey:         "04cafebabe",
                                                                                                                         CustomData:        null
                                                                                                                     ),
                                                                                                 UnitOfMeasure:      null,
                                                                                                 CustomData:         null
                                                                                             )
                                                                                         }
                                                                    )
                                                                },
                                          CustomData:           null

                                      );

                            },
                            CancellationToken.None);

                            response = new RequestStopTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Accepted
                                       );

                        }
                        else
                            response = new RequestStopTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnRequestStopTransactionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnRequestStopTransactionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStopTransactionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetTransactionStatus

            ChargingStationServer.OnGetTransactionStatus += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                #region Send OnGetTransactionStatusRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetTransactionStatusRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetTransactionStatusRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetTransactionStatusRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetTransactionStatusResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetTransactionStatusResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetTransactionStatus request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetTransactionStatusRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetTransactionStatusResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetTransactionStatus for '{request.TransactionId}'!");

                        // TransactionId

                        if (request.TransactionId.HasValue)
                        {

                            var foundEVSE =  evses.Values.FirstOrDefault(evse => request.TransactionId == evse.TransactionId);

                            if (foundEVSE is not null)
                            {

                                response = new GetTransactionStatusResponse(
                                               request,
                                               MessagesInQueue:    false,
                                               OngoingIndicator:   true
                                           );

                            }
                            else
                            {

                                response = new GetTransactionStatusResponse(
                                               request,
                                               MessagesInQueue:    false,
                                               OngoingIndicator:   true
                                           );

                            }

                        }
                        else
                        {

                            response = new GetTransactionStatusResponse(
                                           request,
                                           MessagesInQueue:    false,
                                           OngoingIndicator:   true
                                       );

                        }

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomGetTransactionStatusResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnGetTransactionStatusResponse event

                var responseLogger = OnGetTransactionStatusResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetTransactionStatusResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetTransactionStatusResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetChargingProfile

            ChargingStationServer.OnSetChargingProfile += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                #region Send OnSetChargingProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetChargingProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetChargingProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetChargingProfileRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetChargingProfileResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetChargingProfileResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetChargingProfile request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new SetChargingProfileResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetChargingProfile for '{request.EVSEId}'!");

                        // EVSEId
                        // ChargingProfile

                        // ToDo: lock(connectors)

                        if (request.EVSEId.Value == 0)
                        {

                            foreach (var evse in evses.Values)
                            {

                                if (!request.ChargingProfile.TransactionId.HasValue)
                                    evse.ChargingProfile = request.ChargingProfile;

                                else if (evse.TransactionId == request.ChargingProfile.TransactionId.Value)
                                    evse.ChargingProfile = request.ChargingProfile;

                            }

                            response = new SetChargingProfileResponse(
                                           request,
                                           ChargingProfileStatus.Accepted
                                       );

                        }
                        else if (evses.ContainsKey(request.EVSEId))
                        {

                            evses[request.EVSEId].ChargingProfile = request.ChargingProfile;

                            response = new SetChargingProfileResponse(
                                           request,
                                           ChargingProfileStatus.Accepted
                                       );

                        }
                        else
                            response = new SetChargingProfileResponse(
                                           request,
                                           ChargingProfileStatus.Rejected
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetChargingProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetChargingProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetChargingProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetChargingProfiles

            ChargingStationServer.OnGetChargingProfiles += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnGetChargingProfilesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetChargingProfilesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetChargingProfilesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetChargingProfilesRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetChargingProfilesResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetChargingProfilesResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetChargingProfiles request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new GetChargingProfilesResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetChargingProfiles request ({request.GetChargingProfilesRequestId}) for '{request.EVSEId}'!");

                        // GetChargingProfilesRequestId
                        // ChargingProfile
                        // EVSEId

                        if (request.EVSEId.HasValue && evses.ContainsKey(request.EVSEId.Value))
                        {

                            //evses[Request.EVSEId.Value].ChargingProfile = Request.ChargingProfile;

                            response = new GetChargingProfilesResponse(
                                           request,
                                           GetChargingProfileStatus.Accepted
                                       );

                        }
                        else
                           response = new GetChargingProfilesResponse(
                                          request,
                                          GetChargingProfileStatus.Unknown
                                      );

                            }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetChargingProfilesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetChargingProfilesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetChargingProfilesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearChargingProfile

            ChargingStationServer.OnClearChargingProfile += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                #region Send OnClearChargingProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearChargingProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearChargingProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearChargingProfileRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ClearChargingProfileResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ClearChargingProfileResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ClearChargingProfile request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new ClearChargingProfileResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ClearChargingProfile request for charging profile identification '{request.ChargingProfileId}'!");

                        // ChargingProfileId
                        // ChargingProfileCriteria

                        response = new ClearChargingProfileResponse(
                                       Request:      request,
                                       Status:       ClearChargingProfileStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnClearChargingProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearChargingProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearChargingProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCompositeSchedule

            ChargingStationServer.OnGetCompositeSchedule += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                #region Send OnGetCompositeScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCompositeScheduleRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCompositeScheduleRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetCompositeScheduleRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetCompositeScheduleResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetCompositeScheduleResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetCompositeSchedule request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetCompositeScheduleRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetCompositeScheduleResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetCompositeSchedule request for the next {request.Duration.TotalMinutes} minutes of EVSE '{request.EVSEId}'!");

                        // Duration,
                        // EVSEId,
                        // ChargingRateUnit

                        response = new GetCompositeScheduleResponse(
                                       Request:      request,
                                       Status:       GenericStatus.Accepted,
                                       Schedule:     null,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetCompositeScheduleResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCompositeScheduleResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetCompositeScheduleResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateDynamicSchedule

            ChargingStationServer.OnUpdateDynamicSchedule += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    cancellationToken) => {

                #region Send OnUpdateDynamicScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUpdateDynamicScheduleRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUpdateDynamicScheduleRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateDynamicScheduleRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                UpdateDynamicScheduleResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new UpdateDynamicScheduleResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid UpdateDynamicSchedule request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomUpdateDynamicScheduleRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new UpdateDynamicScheduleResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming UpdateDynamicSchedule request for charging profile '{request.ChargingProfileId}'!");

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

                        response = new UpdateDynamicScheduleResponse(
                                       Request:      request,
                                       Status:       ChargingProfileStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnUpdateDynamicScheduleResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUpdateDynamicScheduleResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateDynamicScheduleResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            ChargingStationServer.OnNotifyAllowedEnergyTransfer += async (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          cancellationToken) => {

                #region Send OnNotifyAllowedEnergyTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyAllowedEnergyTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyAllowedEnergyTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyAllowedEnergyTransferRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                NotifyAllowedEnergyTransferResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new NotifyAllowedEnergyTransferResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid NotifyAllowedEnergyTransfer request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomNotifyAllowedEnergyTransferRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new NotifyAllowedEnergyTransferResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming NotifyAllowedEnergyTransfer request allowing energy transfer modes: '{request.AllowedEnergyTransferModes.Select(mode => mode.AsText()).AggregateWith(", ")}'!");

                        // AllowedEnergyTransferModes

                        response = new NotifyAllowedEnergyTransferResponse(
                                       Request:      request,
                                       Status:       NotifyAllowedEnergyTransferStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnNotifyAllowedEnergyTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyAllowedEnergyTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyAllowedEnergyTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUsePriorityCharging

            ChargingStationServer.OnUsePriorityCharging += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnUsePriorityChargingRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUsePriorityChargingRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUsePriorityChargingRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUsePriorityChargingRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                UsePriorityChargingResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new UsePriorityChargingResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid UsePriorityCharging request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomUsePriorityChargingRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new UsePriorityChargingResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming UsePriorityCharging request for transaction '{request.TransactionId}': {(request.Activate ? "active" : "disabled")}!");

                        // TransactionId
                        // Activate

                        response = new UsePriorityChargingResponse(
                                       Request:      request,
                                       Status:       GenericStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnUsePriorityChargingResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUsePriorityChargingResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUsePriorityChargingResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnlockConnector

            ChargingStationServer.OnUnlockConnector += async (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnUnlockConnectorRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUnlockConnectorRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUnlockConnectorRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnlockConnectorRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                UnlockConnectorResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new UnlockConnectorResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid UnlockConnector request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomUnlockConnectorRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new UnlockConnectorResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming UnlockConnector request for EVSE '{request.EVSEId}' and connector '{request.ConnectorId}'!");

                        // EVSEId
                        // ConnectorId

                        // ToDo: lock(connectors)

                        if (evses.TryGetValue    (request.EVSEId,      out var evse) &&
                            evse. TryGetConnector(request.ConnectorId, out var connector))
                        {

                            // What to do here?!

                            response = new UnlockConnectorResponse(
                                           Request:      request,
                                           Status:       UnlockStatus.Unlocked,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }
                        else
                            response = new UnlockConnectorResponse(
                                           Request:      request,
                                           Status:       UnlockStatus.UnlockFailed,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnUnlockConnectorResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUnlockConnectorResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnlockConnectorResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnAFRRSignal

            ChargingStationServer.OnAFRRSignal += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnAFRRSignalRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnAFRRSignalRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnAFRRSignalRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnAFRRSignalRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                AFRRSignalResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new AFRRSignalResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid AFRRSignal request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomAFRRSignalRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new AFRRSignalResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming AFRRSignal '{request.Signal}' for timestamp '{request.ActivationTimestamp}'!");

                        // ActivationTimestamp
                        // Signal

                        response = new AFRRSignalResponse(
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

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnAFRRSignalResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnAFRRSignalResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnAFRRSignalResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnSetDisplayMessage

            ChargingStationServer.OnSetDisplayMessage += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                #region Send OnSetDisplayMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetDisplayMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetDisplayMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                SetDisplayMessageResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new SetDisplayMessageResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid SetDisplayMessage request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new SetDisplayMessageResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming SetDisplayMessage '{request.Message.Message.Content}'!");

                        // Message

                        if (displayMessages.TryAdd(request.Message.Id,
                                                   request.Message)) {

                            response = new SetDisplayMessageResponse(
                                           Request:      request,
                                           Status:       DisplayMessageStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                        else
                            response = new SetDisplayMessageResponse(
                                           Request:      request,
                                           Status:       DisplayMessageStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnSetDisplayMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetDisplayMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetDisplayMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetDisplayMessages

            ChargingStationServer.OnGetDisplayMessages += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                #region Send OnGetDisplayMessagesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetDisplayMessagesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetDisplayMessagesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetDisplayMessagesRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                GetDisplayMessagesResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new GetDisplayMessagesResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid GetDisplayMessages request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomGetDisplayMessagesRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new GetDisplayMessagesResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming GetDisplayMessages request ({request.GetDisplayMessagesRequestId})!");

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

                            await this.NotifyDisplayMessages(
                                      NotifyDisplayMessagesRequestId:   request.GetDisplayMessagesRequestId,
                                      MessageInfos:                     filteredDisplayMessages,
                                      ToBeContinued:                    false,
                                      CustomData:                       null
                                  );

                        },
                        CancellationToken.None);

                        response = new GetDisplayMessagesResponse(
                                       request,
                                       GetDisplayMessagesStatus.Accepted
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnGetDisplayMessagesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetDisplayMessagesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetDisplayMessagesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearDisplayMessage

            ChargingStationServer.OnClearDisplayMessage += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnClearDisplayMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearDisplayMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearDisplayMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                ClearDisplayMessageResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new ClearDisplayMessageResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid ClearDisplayMessage request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomClearDisplayMessageRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new ClearDisplayMessageResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming ClearDisplayMessage request ({request.DisplayMessageId})!");

                        // DisplayMessageId

                        if (displayMessages.TryGetValue(request.DisplayMessageId, out var messageInfo) &&
                            displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(request.DisplayMessageId, messageInfo))) {

                            response = new ClearDisplayMessageResponse(
                                           request,
                                           ClearMessageStatus.Accepted
                                       );

                        }

                        else
                            response = new ClearDisplayMessageResponse(
                                           request,
                                           ClearMessageStatus.Unknown
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnClearDisplayMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearDisplayMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearDisplayMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCostUpdated

            ChargingStationServer.OnCostUpdated += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnCostUpdatedRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCostUpdatedRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCostUpdatedRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCostUpdatedRequest),
                                  e
                              );
                    }

                }

                #endregion

                #region Check charging station identification

                CostUpdatedResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new CostUpdatedResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid CostUpdated request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
                             request,
                             request.ToJSON(
                                 CustomCostUpdatedRequestSerializer,
                                 CustomSignatureSerializer,
                                 CustomCustomDataSerializer
                             ),
                             out var errorResponse
                         ))
                    {

                        response = new CostUpdatedResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature: {errorResponse}"
                                                 )
                                   );

                    }

                #endregion

                    else
                    {

                        DebugX.Log($"Charging station '{Id}': Incoming CostUpdated request '{request.TotalCost}' for transaction '{request.TransactionId}'!");

                        // TotalCost
                        // TransactionId

                        if (transactions.ContainsKey(request.TransactionId)) {

                            totalCosts.AddOrUpdate(request.TransactionId,
                                                   request.TotalCost,
                                                   (transactionId, totalCost) => request.TotalCost);

                            response = new CostUpdatedResponse(
                                           request
                                       );

                        }

                        else
                            response = new CostUpdatedResponse(
                                           request,
                                           Result.GenericError($"Unknown transaction identification '{request.TransactionId}'!")
                                       );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomCostUpdatedResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnCostUpdatedResponse event

                var responseLogger = OnCostUpdatedResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCostUpdatedResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCostUpdatedResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCustomerInformation

            ChargingStationServer.OnCustomerInformation += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnCustomerInformationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCustomerInformationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCustomerInformationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCustomerInformationRequest),
                                  e
                              );
                    }

                }

                #endregion


                #region Check charging station identification

                CustomerInformationResponse? response = null;

                if (request.ChargingStationId != Id)
                {
                    response = new CustomerInformationResponse(
                                   Request:  request,
                                   Result:   Result.GenericError(
                                                 $"Charging station '{Id}': Invalid CustomerInformation request for charging station '{request.ChargingStationId}'!"
                                             )
                               );
                }

                #endregion

                #region Check request signature(s)

                else
                {

                    if (!SignaturePolicy.VerifyRequestMessage(
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

                        response = new CustomerInformationResponse(
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


                        DebugX.Log($"Charging station '{Id}': Incoming CustomerInformation request ({request.CustomerInformationRequestId}) to {command} for customer '{customer}'!");

                        // CustomerInformationRequestId
                        // Report
                        // Clear
                        // CustomerIdentifier
                        // IdToken
                        // CustomerCertificate

                        _ = Task.Run(async () => {

                            await this.NotifyCustomerInformation(
                                      NotifyCustomerInformationRequestId:   request.CustomerInformationRequestId,
                                      Data:                                 customer,
                                      SequenceNumber:                       1,
                                      GeneratedAt:                          Timestamp.Now,
                                      ToBeContinued:                        false,
                                      CustomData:                           null
                                  );

                        },
                        CancellationToken.None);

                        response = new CustomerInformationResponse(
                                       request,
                                       CustomerInformationStatus.Accepted
                                   );

                    }

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
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

                var responseLogger = OnCustomerInformationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCustomerInformationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCustomerInformationResponse),
                                  e
                              );
                    }

                }

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

            foreach (var enquedRequest in EnqueuedRequests.ToArray())
            {
                if (CSClient is ChargingStationWSClient wsClient)
                {

                    var response = await wsClient.SendRequest(
                                             enquedRequest.Command,
                                             enquedRequest.Request.RequestId,
                                             enquedRequest.RequestJSON
                                         );

                    enquedRequest.ResponseAction(response);

                    EnqueuedRequests.Remove(enquedRequest);

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
                    this.SendHeartbeat().Wait();
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
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion


        #region SendBootNotification                  (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public async Task<CSMS.BootNotificationResponse>
            SendBootNotification(BootNotificationRequest Request)

        {

            #region Send OnBootNotificationRequest event

            var startTime  = Timestamp.Now;

            try
            {

                OnBootNotificationRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomBootNotificationRequestSerializer,
                                         CustomChargingStationSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendBootNotification(Request)

                                     : new CSMS.BootNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.BootNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomBootNotificationResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            switch (response.Status)
            {

                case RegistrationStatus.Accepted:
                    this.CSMSTime               = response.CurrentTime;
                    this.SendHeartbeatEvery     = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                    this.SendHeartbeatTimer.Change(this.SendHeartbeatEvery, this.SendHeartbeatEvery);
                    this.DisableSendHeartbeats  = false;
                    break;

                case RegistrationStatus.Pending:
                    // Do not reconnect before: response.HeartbeatInterval
                    break;

                case RegistrationStatus.Rejected:
                    // Do not reconnect before: response.HeartbeatInterval
                    break;

            }


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBootNotificationResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFirmwareStatusNotification        (Request)

        /// <summary>
        /// Send a firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.FirmwareStatusNotificationResponse>
            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request)

        {

            #region Send OnFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomFirmwareStatusNotificationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendFirmwareStatusNotification(Request)

                                     : new CSMS.FirmwareStatusNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.FirmwareStatusNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomFirmwareStatusNotificationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendPublishFirmwareStatusNotification (Request)

        /// <summary>
        /// Send a publish firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The progress status of the publish firmware request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.PublishFirmwareStatusNotificationResponse>
            SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request)

        {

            #region Send OnPublishFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                   this,
                                                                   Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomPublishFirmwareStatusNotificationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendPublishFirmwareStatusNotification(Request)

                                     : new CSMS.PublishFirmwareStatusNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.PublishFirmwareStatusNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomPublishFirmwareStatusNotificationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnPublishFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                    this,
                                                                    Request,
                                                                    response,
                                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                         (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.HeartbeatResponse>
            SendHeartbeat(HeartbeatRequest Request)

        {

            #region Send OnHeartbeatRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnHeartbeatRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomHeartbeatRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendHeartbeat(Request)

                                     : new CSMS.HeartbeatResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.HeartbeatResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomHeartbeatResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEvent                           (Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="EventData">The enumeration of event data.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyEventResponse>
            NotifyEvent(NotifyEventRequest Request)

        {

            #region Send OnNotifyEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEventRequest?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyEventRequestSerializer,
                                         CustomEventDataSerializer,
                                         CustomComponentSerializer,
                                         CustomEVSESerializer,
                                         CustomVariableSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyEvent(Request)

                                     : new CSMS.NotifyEventResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyEventResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyEventResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEventResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendSecurityEventNotification         (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.SecurityEventNotificationResponse>
            SendSecurityEventNotification(SecurityEventNotificationRequest Request)

        {

            #region Send OnSecurityEventNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSecurityEventNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomSecurityEventNotificationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendSecurityEventNotification(Request)

                                     : new CSMS.SecurityEventNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.SecurityEventNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSecurityEventNotificationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSecurityEventNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnSecurityEventNotificationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSecurityEventNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyReport                          (Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyReportResponse>
            NotifyReport(NotifyReportRequest Request)

        {

            #region Send OnNotifyReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyReportRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyReportRequestSerializer,
                                         CustomReportDataSerializer,
                                         CustomComponentSerializer,
                                         CustomEVSESerializer,
                                         CustomVariableSerializer,
                                         CustomVariableAttributeSerializer,
                                         CustomVariableCharacteristicsSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyReport(Request)

                                     : new CSMS.NotifyReportResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyReportResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyReportResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyMonitoringReport                (Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyMonitoringReportResponse>
            NotifyMonitoringReport(NotifyMonitoringReportRequest Request)

        {

            #region Send OnNotifyMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyMonitoringReportRequestSerializer,
                                         CustomMonitoringDataSerializer,
                                         CustomComponentSerializer,
                                         CustomEVSESerializer,
                                         CustomVariableSerializer,
                                         CustomVariableMonitoringSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyMonitoringReport(Request)

                                     : new CSMS.NotifyMonitoringReportResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyMonitoringReportResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyMonitoringReportResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLogStatusNotification             (Request)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Status">The status of the log upload.</param>
        /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.LogStatusNotificationResponse>
            SendLogStatusNotification(LogStatusNotificationRequest Request)

        {

            #region Send OnLogStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnLogStatusNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomLogStatusNotificationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendLogStatusNotification(Request)

                                     : new CSMS.LogStatusNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.LogStatusNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomLogStatusNotificationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnLogStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnLogStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData                          (Request)

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.DataTransferResponse>
            TransferData(DataTransferRequest Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomDataTransferRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.TransferData(Request)

                                     : new CSMS.DataTransferResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.DataTransferResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDataTransferResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendCertificateSigningRequest         (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.SignCertificateResponse>
            SendCertificateSigningRequest(SignCertificateRequest Request)

        {

            #region Send OnSignCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignCertificateRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomSignCertificateRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendCertificateSigningRequest(Request)

                                     : new CSMS.SignCertificateResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.SignCertificateResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSignCertificateResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSignCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignCertificateResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Get15118EVCertificate                 (Request)

        /// <summary>
        /// Get an ISO 15118 contract certificate.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
        /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
        /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
        /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
        /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.Get15118EVCertificateResponse>
            Get15118EVCertificate(Get15118EVCertificateRequest Request)

        {

            #region Send OnGet15118EVCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGet15118EVCertificateRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomGet15118EVCertificateRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.Get15118EVCertificate(Request)

                                     : new CSMS.Get15118EVCertificateResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.Get15118EVCertificateResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGet15118EVCertificateResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGet15118EVCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGet15118EVCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCertificateStatus                  (Request)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.GetCertificateStatusResponse>
            GetCertificateStatus(GetCertificateStatusRequest Request)

        {

            #region Send OnGetCertificateStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCertificateStatusRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomGetCertificateStatusRequestSerializer,
                                         CustomOCSPRequestDataSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.GetCertificateStatus(Request)

                                     : new CSMS.GetCertificateStatusResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.GetCertificateStatusResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetCertificateStatusResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCertificateStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCertificateStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCRL                                (Request)

        /// <summary>
        /// Get a certificate revocation list from CSMS for the specified certificate.
        /// </summary>
        /// 
        /// <param name="GetCRLRequestId">The identification of this request.</param>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.GetCRLResponse>
            GetCRL(GetCRLRequest Request)

        {

            #region Send OnGetCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCRLRequest?.Invoke(startTime,
                                        this,
                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCRLRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomGetCRLRequestSerializer,
                                         CustomCertificateHashDataSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.GetCRL(Request)

                                     : new CSMS.GetCRLResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.GetCRLResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetCRLResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCRLResponse?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendReservationStatusUpdate           (Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.ReservationStatusUpdateResponse>
            SendReservationStatusUpdate(ReservationStatusUpdateRequest Request)

        {

            #region Send OnReservationStatusUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                         this,
                                                         Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomReservationStatusUpdateRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendReservationStatusUpdate(Request)

                                     : new CSMS.ReservationStatusUpdateResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.ReservationStatusUpdateResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomReservationStatusUpdateResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReservationStatusUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Authorize                             (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.AuthorizeResponse>
            Authorize(AuthorizeRequest Request)

        {

            #region Send OnAuthorizeRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomAuthorizeRequestSerializer,
                                         CustomIdTokenSerializer,
                                         CustomAdditionalInfoSerializer,
                                         CustomOCSPRequestDataSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.Authorize(Request)

                                     : new CSMS.AuthorizeResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.AuthorizeResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomAuthorizeResponseSerializer,
                    CustomIdTokenInfoSerializer,
                    CustomIdTokenSerializer,
                    CustomAdditionalInfoSerializer,
                    CustomMessageContentSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingNeeds                 (Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
        /// <param name="MaxScheduleTuples">The optional maximum number of schedule tuples per schedule the car supports.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyEVChargingNeedsResponse>
            NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest Request)

        {

            #region Send OnNotifyEVChargingNeedsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyEVChargingNeedsRequestSerializer,
                                         CustomChargingNeedsSerializer,
                                         CustomACChargingParametersSerializer,
                                         CustomDCChargingParametersSerializer,
                                         CustomV2XChargingParametersSerializer,
                                         CustomEVEnergyOfferSerializer,
                                         CustomEVPowerScheduleSerializer,
                                         CustomEVPowerScheduleEntrySerializer,
                                         CustomEVAbsolutePriceScheduleSerializer,
                                         CustomEVAbsolutePriceScheduleEntrySerializer,
                                         CustomEVPriceRuleSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyEVChargingNeeds(Request)

                                     : new CSMS.NotifyEVChargingNeedsResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyEVChargingNeedsResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyEVChargingNeedsResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendTransactionEvent                  (Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
        /// <param name="TransactionInfo">Transaction related information.</param>
        /// 
        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
        /// <param name="MeterValues">An optional enumeration of meter values.</param>
        /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.TransactionEventResponse>
            SendTransactionEvent(TransactionEventRequest Request)

        {

            #region Send OnTransactionEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTransactionEventRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomTransactionEventRequestSerializer,
                                         CustomTransactionSerializer,
                                         CustomIdTokenSerializer,
                                         CustomAdditionalInfoSerializer,
                                         CustomEVSESerializer,
                                         CustomMeterValueSerializer,
                                         CustomSampledValueSerializer,
                                         CustomSignedMeterValueSerializer,
                                         CustomUnitsOfMeasureSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendTransactionEvent(Request)

                                     : new CSMS.TransactionEventResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.TransactionEventResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomTransactionEventResponseSerializer,
                    CustomIdTokenInfoSerializer,
                    CustomIdTokenSerializer,
                    CustomAdditionalInfoSerializer,
                    CustomMessageContentSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnTransactionEventResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnTransactionEventResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification                (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="Status">The current status of the connector.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.StatusNotificationResponse>
            SendStatusNotification(StatusNotificationRequest Request)

        {

            #region Send OnStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStatusNotificationRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomStatusNotificationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendStatusNotification(Request)

                                     : new CSMS.StatusNotificationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.StatusNotificationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomStatusNotificationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnStatusNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendMeterValues                       (Request)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="EVSEId">The EVSE identification at the charging station.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.MeterValuesResponse>
            SendMeterValues(MeterValuesRequest Request)

        {

            #region Send OnMeterValuesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnMeterValuesRequest?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomMeterValuesRequestSerializer,
                                         CustomMeterValueSerializer,
                                         CustomSampledValueSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendMeterValues(Request)

                                     : new CSMS.MeterValuesResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.MeterValuesResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomMeterValuesResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnMeterValuesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnMeterValuesResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyChargingLimit                   (Request)

        /// <summary>
        /// Notify about a charging limit.
        /// </summary>
        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
        /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyChargingLimitResponse>
            NotifyChargingLimit(NotifyChargingLimitRequest Request)

        {

            #region Send OnNotifyChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(

                                         CustomNotifyChargingLimitRequestSerializer,
                                         CustomChargingScheduleSerializer,
                                         CustomLimitBeyondSoCSerializer,
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
                                 )

                                     ? await CSClient.NotifyChargingLimit(Request)

                                     : new CSMS.NotifyChargingLimitResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyChargingLimitResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyChargingLimitResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendClearedChargingLimit              (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.ClearedChargingLimitResponse>
            SendClearedChargingLimit(ClearedChargingLimitRequest Request)

        {

            #region Send OnClearedChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomClearedChargingLimitRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.SendClearedChargingLimit(Request)

                                     : new CSMS.ClearedChargingLimitResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.ClearedChargingLimitResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearedChargingLimitResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearedChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ReportChargingProfiles                (Request)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.ReportChargingProfilesResponse>
            ReportChargingProfiles(ReportChargingProfilesRequest Request)

        {

            #region Send OnReportChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(

                                         CustomReportChargingProfilesRequestSerializer,
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
                                 )

                                     ? await CSClient.ReportChargingProfiles(Request)

                                     : new CSMS.ReportChargingProfilesResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.ReportChargingProfilesResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomReportChargingProfilesResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReportChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingSchedule              (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
        /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
        /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyEVChargingScheduleResponse>
            NotifyEVChargingSchedule(NotifyEVChargingScheduleRequest Request)

        {

            #region Send OnNotifyEVChargingScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleRequest?.Invoke(startTime,
                                                          this,
                                                          Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingScheduleRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(

                                         CustomNotifyEVChargingScheduleRequestSerializer,
                                         CustomChargingScheduleSerializer,
                                         CustomLimitBeyondSoCSerializer,
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
                                 )

                                     ? await CSClient.NotifyEVChargingSchedule(Request)

                                     : new CSMS.NotifyEVChargingScheduleResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyEVChargingScheduleResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyEVChargingScheduleResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEVChargingScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyPriorityCharging                (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyPriorityChargingRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyPriorityChargingResponse>
            NotifyPriorityCharging(NotifyPriorityChargingRequest Request)

        {

            #region Send OnNotifyPriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyPriorityChargingRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyPriorityChargingRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyPriorityCharging(Request)

                                     : new CSMS.NotifyPriorityChargingResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyPriorityChargingResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyPriorityChargingResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyPriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyPriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PullDynamicScheduleUpdate             (Request)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting PullDynamicScheduleUpdateRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.PullDynamicScheduleUpdateResponse>
            PullDynamicScheduleUpdate(PullDynamicScheduleUpdateRequest Request)

        {

            #region Send OnPullDynamicScheduleUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomPullDynamicScheduleUpdateRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.PullDynamicScheduleUpdate(Request)

                                     : new CSMS.PullDynamicScheduleUpdateResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.PullDynamicScheduleUpdateResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomPullDynamicScheduleUpdateResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnPullDynamicScheduleUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region NotifyDisplayMessages                 (Request)

        /// <summary>
        /// NotifyDisplayMessages the given token.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the notify display messages request.</param>
        /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyDisplayMessagesResponse>
            NotifyDisplayMessages(NotifyDisplayMessagesRequest Request)

        {

            #region Send OnNotifyDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyDisplayMessagesRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyDisplayMessagesRequestSerializer,
                                         CustomMessageInfoSerializer,
                                         CustomMessageContentSerializer,
                                         CustomComponentSerializer,
                                         CustomEVSESerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyDisplayMessages(Request)

                                     : new CSMS.NotifyDisplayMessagesResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyDisplayMessagesResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyDisplayMessagesResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCustomerInformation             (Request)

        /// <summary>
        /// NotifyCustomerInformation the given token.
        /// </summary>
        /// <param name="NotifyCustomerInformationRequestId">The unique identification of the notify customer information request.</param>
        /// <param name="Data">The requested data or a part of the requested data. No format specified in which the data is returned.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.NotifyCustomerInformationResponse>
            NotifyCustomerInformation(NotifyCustomerInformationRequest Request)

        {

            #region Send OnNotifyCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            var response = CSClient is not null

                               ? SignaturePolicy.SignRequestMessage(
                                     Request,
                                     Request.ToJSON(
                                         CustomNotifyCustomerInformationRequestSerializer,
                                         CustomSignatureSerializer,
                                         CustomCustomDataSerializer
                                     ),
                                     out var errorResponse
                                 )

                                     ? await CSClient.NotifyCustomerInformation(Request)

                                     : new CSMS.NotifyCustomerInformationResponse(
                                           Request,
                                           Result.SignatureError(errorResponse)
                                       )

                               : new CSMS.NotifyCustomerInformationResponse(
                                     Request,
                                     Result.Server("Unknown or unreachable charging station!")
                                 );

            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyCustomerInformationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Dispose()

        public void Dispose()
        { }

        #endregion


    }

}
