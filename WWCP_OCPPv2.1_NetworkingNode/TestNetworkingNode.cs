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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Concurrent;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Properties

        public Connector_Id    Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region ChargingStationConnector(Id, ConnectorType)

        public ChargingStationConnector(Connector_Id    Id,
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
    /// A networking node for testing.
    /// </summary>
    public partial class TestNetworkingNode : INetworkingNode,
                                              IEventSender
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public  const     String                                                       DefaultHTTPServiceName          = $"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API";

        private readonly  HashSet<OCPPWebSocketServer>                                 OCPPWebSocketServers            = [];
        private readonly  ConcurrentDictionary<NetworkingNode_Id, List<Reachability>>  reachableNetworkingNodes        = [];
        private readonly  ConcurrentDictionary<Request_Id, SendRequestState>           requests                        = [];

        public  const     String                                                       NetworkingNodeId_WebSocketKey   = "networkingNodeId";
        public  const     String                                                       NetworkingMode_WebSocketKey     = "networkingMode";

        private readonly  HashSet<SignaturePolicy>                                     signaturePolicies               = [];
        private readonly  HashSet<SignaturePolicy>                                     forwardingSignaturePolicies     = [];

        private           Int64                                                        internalRequestId               = 800000;

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => this.Id.ToString();

        /// <summary>
        /// The unique identification of this networking node.
        /// </summary>
        public NetworkingNode_Id        Id                          { get; }

        /// <summary>
        /// The networking node vendor identification.
        /// </summary>
        [Mandatory]
        public String                   VendorName                  { get; }

        /// <summary>
        ///  The networking node model identification.
        /// </summary>
        [Mandatory]
        public String                   Model                       { get; }


        /// <summary>
        /// The optional multi-language networking node description.
        /// </summary>
        [Optional]
        public I18NString?              Description                 { get; }

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


        public CustomData?              CustomData                  { get; set; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; set; }

        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                CSMSTime                    { get; set; }

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

        public DNSClient                DNSClient                   { get; }


        #region ToDo's

        public URL RemoteURL => throw new NotImplementedException();

        public HTTPHostname? VirtualHostname => throw new NotImplementedException();

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
            => signaturePolicies.First();


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


        public  ActingAsCS               AsCS                        { get; }

        public  ActingAsCSMS             AsCSMS                      { get; }


        public  INetworkingNodeIN        IN                          { get; }

        public  INetworkingNodeOUT       OUT                         { get; }

        public  FORWARD                  FORWARD                     { get; }

        public OCPPWebSocketAdapterIN    ocppIN                      { get; }
        public OCPPWebSocketAdapterOUT   ocppOUT                     { get; }


        public CS.INetworkingNodeOutgoingMessages? CSClient
            => AsCS.CSClient;

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

        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?   CustomNotifyNetworkTopologyRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer          { get; set; }

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


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer             { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public TestNetworkingNode(NetworkingNode_Id     Id,
                                  String                VendorName,
                                  String                Model,

                                  I18NString?           Description               = null,
                                  String?               SerialNumber              = null,
                                  String?               FirmwareVersion           = null,
                                  Modem?                Modem                     = null,

                                  Boolean               DisableSendHeartbeats     = false,
                                  TimeSpan?             SendHeartbeatEvery        = null,

                                  Boolean               DisableMaintenanceTasks   = false,
                                  TimeSpan?             MaintenanceEvery          = null,

                                  TimeSpan?             DefaultRequestTimeout     = null,
                                  IHTTPAuthentication?  HTTPAuthentication        = null,
                                  DNSClient?            DNSClient                 = null,

                                  SignaturePolicy?      SignaturePolicy           = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id),          "The given networking node identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given networking node vendor must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given networking node model must not be null or empty!");

            this.Id                       = Id;
            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.Description              = Description;
            this.SerialNumber             = SerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Modem                    = Modem;


            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            this.DNSClient                = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.signaturePolicies.          Add(SignaturePolicy ?? new SignaturePolicy());
            this.forwardingSignaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

         //   this.EnqueuedRequests         = [];


            this.AsCSMS                   = new ActingAsCSMS(

                                                NetworkingNode:            this,
                                                RequireAuthentication:     false,
                                                DefaultRequestTimeout:     this.DefaultRequestTimeout,
                                                HTTPUploadPort:            null,
                                                DNSClient:                 this.DNSClient,

                                                SignaturePolicy:           this.SignaturePolicy

                                            );

            this.AsCS                     = new ActingAsCS(

                                                NetworkingNode:            this,

                                                DisableSendHeartbeats:     this.DisableSendHeartbeats,
                                                SendHeartbeatEvery:        this.SendHeartbeatEvery,

                                                DisableMaintenanceTasks:   this.DisableMaintenanceTasks,
                                                MaintenanceEvery:          this.MaintenanceEvery,

                                                DefaultRequestTimeout:     this.DefaultRequestTimeout,
                                                HTTPAuthentication:        null,
                                                DNSClient:                 this.DNSClient,

                                                SignaturePolicy:           this.SignaturePolicy

                                            );

            this.IN       = new INPUT (this);
            this.OUT      = new OUTPUT(this);
            this.FORWARD  = new FORWARD (this);

            this.ocppIN   = new OCPPWebSocketAdapterIN (this);
            this.ocppOUT  = new OCPPWebSocketAdapterOUT(this);

            Wire();

        }

        #endregion


        #region ConnectWebSocket(...)

        public Task<HTTPResponse?> ConnectWebSocket(URL                                  RemoteURL,
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

            => AsCS.ConnectWebSocket(RemoteURL,
                                     VirtualHostname,
                                     Description,
                                     RemoteCertificateValidator,
                                     ClientCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     PreferIPv4,
                                     HTTPUserAgent,
                                     HTTPAuthentication,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,

                                     SecWebSocketProtocols,
                                     NetworkingMode,

                                     DisableMaintenanceTasks,
                                     MaintenanceEvery,
                                     DisableWebSocketPings,
                                     WebSocketPingEvery,
                                     SlowNetworkSimulationDelay,

                                     LoggingPath,
                                     LoggingContext,
                                     LogfileCreator,
                                     HTTPLogger,
                                     DNSClient);

        #endregion


        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer AttachWebSocketServer(String                               HTTPServiceName              = DefaultHTTPServiceName,
                                                         IIPAddress?                          IPAddress                    = null,
                                                         IPPort?                              TCPPort                      = null,

                                                         Boolean                              RequireAuthentication        = true,
                                                         Boolean                              DisableWebSocketPings        = false,
                                                         TimeSpan?                            WebSocketPingEvery           = null,
                                                         TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                         ServerCertificateSelectorDelegate?   ServerCertificateSelector    = null,
                                                         RemoteCertificateValidationHandler?  ClientCertificateValidator   = null,
                                                         LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                         SslProtocols?                        AllowedTLSProtocols          = null,
                                                         Boolean?                             ClientCertificateRequired    = null,
                                                         Boolean?                             CheckCertificateRevocation   = null,

                                                         ServerThreadNameCreatorDelegate?     ServerThreadNameCreator      = null,
                                                         ServerThreadPriorityDelegate?        ServerThreadPrioritySetter   = null,
                                                         Boolean?                             ServerThreadIsBackground     = null,
                                                         ConnectionIdBuilder?                 ConnectionIdBuilder          = null,
                                                         TimeSpan?                            ConnectionTimeout            = null,
                                                         UInt32?                              MaxClientConnections         = null,

                                                         DNSClient?                           DNSClient                    = null,
                                                         Boolean                              AutoStart                    = false)
        {

            var ocppWebSocketServer = new OCPPWebSocketServer(
                                          this.ocppIN,
                                          this.ocppOUT,

                                          HTTPServiceName,
                                          IPAddress,
                                          TCPPort,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          ServerCertificateSelector,
                                          ClientCertificateValidator,
                                          ClientCertificateSelector,
                                          AllowedTLSProtocols,
                                          ClientCertificateRequired,
                                          CheckCertificateRevocation,

                                          ServerThreadNameCreator,
                                          ServerThreadPrioritySetter,
                                          ServerThreadIsBackground,
                                          ConnectionIdBuilder,
                                          ConnectionTimeout,
                                          MaxClientConnections,

                                          DNSClient ?? this.DNSClient,
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

            OCPPWebSocketServers.Add(WebSocketServer);


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
                                  nameof(TestNetworkingNode),
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
                                  nameof(TestNetworkingNode),
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
                AddStaticRouting(DestinationNodeId:  networkingNodeId,
                                 WebSocketServer:    ocppWebSocketServer,
                                 Priority:           0,
                                 Timestamp:          timestamp);

                #region Send OnNewWebSocketConnection

                var logger = OnNewWebSocketConnection;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType <OnNetworkingNodeNewWebSocketConnectionDelegate>().
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
                                  nameof(TestNetworkingNode),
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
                                               OfType <OnNetworkingNodeCloseMessageReceivedDelegate>().
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
                                  nameof(TestNetworkingNode),
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
                                               OfType <OnNetworkingNodeTCPConnectionClosedDelegate>().
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
                                  nameof(TestNetworkingNode),
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
                                  nameof(TestNetworkingNode),
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


        #region NextRequestId

        public Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        public string? ClientCloseMessage => throw new NotImplementedException();

        #endregion


        #region SendJSONRequest         (JSONRequestMessage)

        public async Task<SendOCPPMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            if (LookupNetworkingNode(JSONRequestMessage.DestinationNodeId, out var reachability) &&
                reachability is not null)
            {

                //if (reachability.OCPPWebSocketClient is not null)
                //    return await reachability.OCPPWebSocketClient.SendJSONRequest(JSONRequestMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    return await reachability.OCPPWebSocketServer.SendJSONRequest(JSONRequestMessage);

            }

            return SendOCPPMessageResult.UnknownClient;

        }

        #endregion

        #region SendJSONRequestAndWait  (JSONRequestMessage)

        public async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var sendOCPPMessageResult = await SendJSONRequest(JSONRequestMessage);

            if (sendOCPPMessageResult == SendOCPPMessageResult.Success)
            {

                #region 1. Store 'in-flight' request...

                requests.TryAdd(JSONRequestMessage.RequestId,
                                SendRequestState.FromJSONRequest(
                                    Timestamp.Now,
                                    JSONRequestMessage.DestinationNodeId,
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
                            sendRequestState?.ErrorCode.HasValue == true))
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
                    sendRequestState2.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(JSONRequestMessage.RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            // Just in case...
            return SendRequestState.FromJSONRequest(
                       RequestTimestamp:   JSONRequestMessage.RequestTimestamp,
                       NetworkingNodeId:   JSONRequestMessage.DestinationNodeId,
                       Timeout:            JSONRequestMessage.RequestTimeout,
                       JSONRequest:        JSONRequestMessage,
                       ResponseTimestamp:  Timestamp.Now,
                       ErrorCode:          ResultCode.InternalError
                   );

        }

        #endregion

        #region SendBinaryRequest       (BinaryRequestMessage)

        public async Task<SendOCPPMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            if (LookupNetworkingNode(BinaryRequestMessage.DestinationNodeId, out var reachability) &&
                reachability is not null)
            {

                //if (reachability.OCPPWebSocketClient is not null)
                //    return await reachability.OCPPWebSocketClient.SendBinaryRequest(BinaryRequestMessage);

                if (reachability.OCPPWebSocketServer is not null)
                    return await reachability.OCPPWebSocketServer.SendBinaryRequest(BinaryRequestMessage);

            }

            return SendOCPPMessageResult.UnknownClient;

        }

        #endregion

        #region SendBinaryRequestAndWait(BinaryRequestMessage)

        public async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var sendOCPPMessageResult = await SendBinaryRequest(BinaryRequestMessage);

            if (sendOCPPMessageResult == SendOCPPMessageResult.Success)
            {

                #region 1. Store 'in-flight' request...

                requests.TryAdd(BinaryRequestMessage.RequestId,
                                SendRequestState.FromBinaryRequest(
                                    Timestamp.Now,
                                    BinaryRequestMessage.DestinationNodeId,
                                    BinaryRequestMessage.RequestTimeout,
                                    BinaryRequestMessage
                                ));

                #endregion

                #region 2. Wait for response... or timeout!

                do
                {

                    try
                    {

                        await Task.Delay(25, BinaryRequestMessage.CancellationToken);

                        if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var sendRequestState) &&
                           (sendRequestState?.JSONResponse   is not null ||
                            sendRequestState?.BinaryResponse is not null ||
                            sendRequestState?.ErrorCode.HasValue == true))
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

                #region 3. When timed out...

                if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var sendRequestState2) &&
                    sendRequestState2 is not null)
                {
                    sendRequestState2.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(BinaryRequestMessage.RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            // Just in case...
            return SendRequestState.FromBinaryRequest(
                       RequestTimestamp:   BinaryRequestMessage.RequestTimestamp,
                       NetworkingNodeId:   BinaryRequestMessage.DestinationNodeId,
                       Timeout:            BinaryRequestMessage.RequestTimeout,
                       BinaryRequest:      BinaryRequestMessage,
                       ResponseTimestamp:  Timestamp.Now,
                       ErrorCode:          ResultCode.InternalError
                   );

        }

        #endregion


        #region ReceiveResponseMessage  (JSONResponseMessage)

        public Boolean ReceiveResponseMessage(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            if (requests.TryGetValue(JSONResponseMessage.RequestId, out var sendRequestState) &&
                sendRequestState is not null)
            {

                sendRequestState.ResponseTimestamp  = Timestamp.Now;
                sendRequestState.JSONResponse       = JSONResponseMessage;

                #region OnJSONMessageResponseReceived

                //var onJSONMessageResponseReceived = OnJSONMessageResponseReceived;
                //if (onJSONMessageResponseReceived is not null)
                //{
                //    try
                //    {

                //        await Task.WhenAll(onJSONMessageResponseReceived.GetInvocationList().
                //                               OfType <OnWebSocketJSONMessageResponseDelegate>().
                //                               Select (loggingDelegate => loggingDelegate.Invoke(
                //                                                              Timestamp.Now,
                //                                                              this,
                //                                                              Connection,
                //                                                              jsonResponse.DestinationNodeId,
                //                                                              jsonResponse.NetworkPath,
                //                                                              EventTrackingId,
                //                                                              sendRequestState.RequestTimestamp,
                //                                                              sendRequestState.JSONRequest?.  ToJSON()      ?? [],
                //                                                              sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                //                                                              Timestamp.Now,
                //                                                              sendRequestState.JSONResponse.  ToJSON(),
                //                                                              CancellationToken
                //                                                          )).
                //                               ToArray());

                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONMessageResponseReceived));
                //    }
                //}

                #endregion

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{JSONResponseMessage.RequestId}' within {nameof(TestNetworkingNode)}:{Environment.NewLine}'{JSONResponseMessage.Payload.ToString(Formatting.None)}'!");
            return false;

        }

        #endregion

        #region ReceiveResponseMessage  (BinaryResponseMessage)

        public Boolean ReceiveResponseMessage(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            if (requests.TryGetValue(BinaryResponseMessage.RequestId, out var sendRequestState) &&
                sendRequestState is not null)
            {

                sendRequestState.ResponseTimestamp  = Timestamp.Now;
                sendRequestState.BinaryResponse     = BinaryResponseMessage;

                #region OnBinaryMessageResponseReceived

                //var onBinaryMessageResponseReceived = OnBinaryMessageResponseReceived;
                //if (onBinaryMessageResponseReceived is not null)
                //{
                //    try
                //    {

                //        await Task.WhenAll(onBinaryMessageResponseReceived.GetInvocationList().
                //                               OfType <OnWebSocketBinaryMessageResponseDelegate>().
                //                               Select (loggingDelegate => loggingDelegate.Invoke(
                //                                                              Timestamp.Now,
                //                                                              this,
                //                                                              Connection,
                //                                                              jsonResponse.DestinationNodeId,
                //                                                              jsonResponse.NetworkPath,
                //                                                              EventTrackingId,
                //                                                              sendRequestState.RequestTimestamp,
                //                                                              sendRequestState.BinaryRequest?.  ToBinary()      ?? [],
                //                                                              sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                //                                                              Timestamp.Now,
                //                                                              sendRequestState.BinaryResponse.  ToBinary(),
                //                                                              CancellationToken
                //                                                          )).
                //                               ToArray());

                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryMessageResponseReceived));
                //    }
                //}

                #endregion

                return true;

            }

            DebugX.Log($"Received an unknown OCPP response with identificaiton '{BinaryResponseMessage.RequestId}' within {nameof(TestNetworkingNode)}:{Environment.NewLine}'{BinaryResponseMessage.Payload.ToBase64()}'!");
            return false;

        }

        #endregion


        #region ReceiveErrorMessage     (JSONErrorMessage)

        public Boolean ReceiveErrorMessage(OCPP_JSONErrorMessage JSONErrorMessage)
        {

            if (requests.TryGetValue(JSONErrorMessage.RequestId, out var sendRequestState) &&
                sendRequestState is not null)
            {

                sendRequestState.JSONResponse      = null;
                sendRequestState.ErrorCode         = JSONErrorMessage.ErrorCode;
                sendRequestState.ErrorDescription  = JSONErrorMessage.ErrorDescription;
                sendRequestState.ErrorDetails      = JSONErrorMessage.ErrorDetails;

                #region OnJSONErrorResponseReceived

                //var onJSONErrorResponseReceived = OnJSONErrorResponseReceived;
                //if (onJSONErrorResponseReceived is not null)
                //{
                //    try
                //    {

                //        await Task.WhenAll(onJSONErrorResponseReceived.GetInvocationList().
                //                               OfType <OnWebSocketTextErrorResponseDelegate>().
                //                               Select (loggingDelegate => loggingDelegate.Invoke(
                //                                                              Timestamp.Now,
                //                                                              this,
                //                                                              Connection,
                //                                                              EventTrackingId,
                //                                                              sendRequestState.RequestTimestamp,
                //                                                              sendRequestState.JSONRequest?.  ToJSON().ToString(JSONFormatting) ?? "",
                //                                                              sendRequestState.BinaryRequest?.ToByteArray()                     ?? [],
                //                                                              Timestamp.Now,
                //                                                              sendRequestState.JSONResponse?. ToString() ?? "",
                //                                                              CancellationToken
                //                                                          )).
                //                               ToArray());

                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONErrorResponseReceived));
                //    }
                //}

                #endregion

                return true;

            }

            DebugX.Log($"Received an unknown OCPP error response with identificaiton '{JSONErrorMessage.RequestId}' within {nameof(TestNetworkingNode)}:{Environment.NewLine}'{JSONErrorMessage.ToJSON().ToString(Formatting.None)}'!");
            return false;

        }

        #endregion



        #region LookupNetworkingNode (DestinationNodeId, out Reachability)

        public Boolean LookupNetworkingNode(NetworkingNode_Id DestinationNodeId, out Reachability? Reachability)
        {

            if (reachableNetworkingNodes.TryGetValue(DestinationNodeId, out var reachabilityList) &&
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

                    } while (reachability.OCPPWebSocketClient is not null ||
                             reachability.OCPPWebSocketServer is not null);

                }

                Reachability = reachability;
                return true;

            }

            Reachability = null;
            return false;

        }

        #endregion

        #region AddStaticRouting     (DestinationNodeId, WebSocketClient,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id    DestinationNodeId,
                                     OCPPWebSocketClient  WebSocketClient,
                                     Byte?                Priority    = 0,
                                     DateTime?            Timestamp   = null,
                                     DateTime?            Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationNodeId,
                                   WebSocketClient,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationNodeId,

                (id)                   => [reachability],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [reachability];

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

            //csmsChannel.Item1.AddStaticRouting(DestinationNodeId,
            //                                   NetworkingHubId);

        }

        #endregion

        #region AddStaticRouting     (DestinationNodeId, WebSocketServer,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id    DestinationNodeId,
                                     OCPPWebSocketServer  WebSocketServer,
                                     Byte?                Priority    = 0,
                                     DateTime?            Timestamp   = null,
                                     DateTime?            Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationNodeId,
                                   WebSocketServer,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationNodeId,

                (id)                   => [reachability],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [reachability];

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

            //csmsChannel.Item1.AddStaticRouting(DestinationNodeId,
            //                                   NetworkingHubId);

        }

        #endregion

        #region AddStaticRouting     (DestinationNodeId, NetworkingHubId,        Priority = 0, Timestamp = null, Timeout = null)

        public void AddStaticRouting(NetworkingNode_Id  DestinationNodeId,
                                     NetworkingNode_Id  NetworkingHubId,
                                     Byte?              Priority    = 0,
                                     DateTime?          Timestamp   = null,
                                     DateTime?          Timeout     = null)
        {

            var reachability = new Reachability(
                                   DestinationNodeId,
                                   NetworkingHubId,
                                   Priority,
                                   Timeout
                               );

            reachableNetworkingNodes.AddOrUpdate(

                DestinationNodeId,

                (id)                   => [reachability],

                (id, reachabilityList) => {

                    if (reachabilityList is null)
                        return [reachability];

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

            //csmsChannel.Item1.AddStaticRouting(DestinationNodeId,
            //                                   NetworkingHubId);

        }

        #endregion

        #region RemoveStaticRouting  (DestinationNodeId, NetworkingHubId = null, Priority = 0)

        public void RemoveStaticRouting(NetworkingNode_Id   DestinationNodeId,
                                        NetworkingNode_Id?  NetworkingHubId   = null,
                                        Byte?               Priority          = 0)
        {

            if (!NetworkingHubId.HasValue)
            {
                reachableNetworkingNodes.TryRemove(DestinationNodeId, out _);
                return;
            }

            if (reachableNetworkingNodes.TryGetValue(DestinationNodeId, out var reachabilityList) &&
                reachabilityList is not null &&
                reachabilityList.Count > 0)
            {

                // For thread-safety!
                var updatedReachabilityList = new List<Reachability>(reachabilityList.Where(entry => entry.NetworkingHub == NetworkingHubId && (!Priority.HasValue || entry.Priority != (Priority ?? 0))));

                if (updatedReachabilityList.Count > 0)
                    reachableNetworkingNodes.TryUpdate(
                        DestinationNodeId,
                        updatedReachabilityList,
                        reachabilityList
                    );

                else
                    reachableNetworkingNodes.TryRemove(DestinationNodeId, out _);

            }

            //csmsChannel.Item1.RemoveStaticRouting(DestinationNodeId,
            //                                      NetworkingHubId);

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




        public void Wire()
        {

            // Bidirectional

            #region OnIncomingDataTransfer

            IN.OnIncomingDataTransfer += async (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                // VendorId
                // MessageId
                // Data

                DebugX.Log("OnIncomingDataTransfer: " + request.VendorId  + ", " +
                                                        request.MessageId + ", " +
                                                        request.Data);


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


                var response =  request.VendorId == Vendor_Id.GraphDefined

                                            ? new DataTransferResponse(
                                                Request:      request,
                                                Status:       DataTransferStatus.Accepted,
                                                Data:         responseData,
                                                StatusInfo:   null,
                                                CustomData:   null
                                            )

                                            : new DataTransferResponse(
                                                Request:      request,
                                                Status:       DataTransferStatus.Rejected,
                                                Data:         null,
                                                StatusInfo:   null,
                                                CustomData:   null
                                            );


                return response;

            };

            #endregion

            #region OnIncomingBinaryDataTransfer

            IN.OnIncomingBinaryDataTransfer += async (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      cancellationToken) => {

                BinaryDataTransferResponse? response = null;

                DebugX.Log($"Charging Station '{Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

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

                return response;

            };

            #endregion


            FORWARD.OnBinaryDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );



            // CS

            #region OnReset

            IN.OnReset += async (timestamp,
                                 sender,
                                 connection,
                                 request,
                                 cancellationToken) => {

                OCPPv2_1.CS.ResetResponse? response = null;

                DebugX.Log($"Charging Station '{Id}': Incoming '{request.ResetType}' reset request{(request.EVSEId.HasValue ? $" for EVSE '{request.EVSEId}" : "")}'!");

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

                return response;

            };

            #endregion

            FORWARD.OnReset += (timestamp,
                                sender,
                                connection,
                                request,
                                cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<OCPPv2_1.CSMS.ResetRequest, OCPPv2_1.CS.ResetResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );







            // CSMS

            FORWARD.OnBootNotification += (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<OCPPv2_1.CS.BootNotificationRequest, OCPPv2_1.CSMS.BootNotificationResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );



        }


    }

}
