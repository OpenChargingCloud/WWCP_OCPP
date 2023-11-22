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
using System.Collections.Concurrent;
using System.Security.Authentication;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region (enum)  SendJSONResults

        public enum SendJSONResults
        {
            Success,
            UnknownClient,
            TransmissionFailed
        }

        #endregion

        #region (class) SendRequestState

        public class SendRequestState
        {

            public DateTime                       Timestamp            { get; }
            public ChargingStation_Id             ChargingStationId    { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage     { get; }
            public DateTime                       Timeout              { get; }

            public DateTime?                      ResponseTimestamp    { get; set; }
            public JObject?                       Response             { get; set; }

            public ResultCodes?                   ErrorCode            { get; set; }
            public String?                        ErrorDescription     { get; set; }
            public JObject?                       ErrorDetails         { get; set; }


            public Boolean                        NoErrors
                 => !ErrorCode.HasValue;

            public Boolean                        HasErrors
                 =>  ErrorCode.HasValue;


            public SendRequestState(DateTime                       Timestamp,
                                    ChargingStation_Id             ChargingStationId,
                                    OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                    DateTime                       Timeout,

                                    DateTime?                      ResponseTimestamp   = null,
                                    JObject?                       Response            = null,

                                    ResultCodes?                   ErrorCode           = null,
                                    String?                        ErrorDescription    = null,
                                    JObject?                       ErrorDetails        = null)
            {

                this.Timestamp          = Timestamp;
                this.ChargingStationId  = ChargingStationId;
                this.WSRequestMessage   = WSRequestMessage;
                this.Timeout            = Timeout;

                this.ResponseTimestamp  = ResponseTimestamp;
                this.Response           = Response;

                this.ErrorCode          = ErrorCode;
                this.ErrorDescription   = ErrorDescription;
                this.ErrorDetails       = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const            String                                                                                 DefaultHTTPServiceName            = $"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly  IPPort                                                                                 DefaultHTTPServerPort             = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public static readonly  HTTPPath                                                                               DefaultURLPrefix                  = HTTPPath.Parse("/" + Version.String);

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly  TimeSpan                                                                               DefaultRequestTimeout             = TimeSpan.FromSeconds(30);


        public  const           String                                                                                 chargingStationId_WebSocketKey    = "chargingStationId";

        private readonly        ConcurrentDictionary<ChargingStation_Id, Tuple<WebSocketServerConnection, DateTime>>   connectedChargingStations         = new();

        private readonly        ConcurrentDictionary<Request_Id, SendRequestState>                                     requests                          = new();


        private const           String                                                                                 LogfileName                       = "CSMSWSServer.log";

        private readonly        Dictionary<String, MethodInfo>                                                         incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => HTTPServiceName;

        /// <summary>
        /// The enumeration of all connected charging stations.
        /// </summary>
        public IEnumerable<ChargingStation_Id> ChargingStationIds
            => connectedChargingStations.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean                                      RequireAuthentication    { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public ConcurrentDictionary<ChargingStation_Id, String?>  ChargingBoxLogins        { get; }
            = new();

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                                   JSONFormatting           { get; set; }
            = Formatting.None;

        /// <summary>
        /// The request timeout for messages sent by this HTTP WebSocket server.
        /// </summary>
        public TimeSpan?                                    RequestTimeout           { get; set; }

        #endregion

        #region Events

        public event OnNewCSMSWSConnectionDelegate?  OnNewCSMSWSConnection;


        #region Generic Text Messages

        /// <summary>
        /// An event sent whenever a text message request was received.
        /// </summary>
        public event OnWebSocketTextMessageRequestDelegate?     OnTextMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextMessageResponseDelegate?    OnTextMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnTextErrorResponseSent;


        /// <summary>
        /// An event sent whenever a text message request was sent.
        /// </summary>
        public event OnWebSocketTextMessageRequestDelegate?     OnTextMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextMessageResponseDelegate?    OnTextMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnTextErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion


        #region Custom JSON serializer delegates

        #region Charging Station Response Messages
        public CustomJObjectSerializerDelegate<CS.ResetResponse>?                                    CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateFirmwareResponse>?                           CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.PublishFirmwareResponse>?                          CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnpublishFirmwareResponse>?                        CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetBaseReportResponse>?                            CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetReportResponse>?                                CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetLogResponse>?                                   CustomGetLogResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<CS.SetVariablesResponse>?                             CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetVariablesResponse>?                             CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringBaseResponse>?                        CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetMonitoringReportResponse>?                      CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringLevelResponse>?                       CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetVariableMonitoringResponse>?                    CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearVariableMonitoringResponse>?                  CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetNetworkProfileResponse>?                        CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.ChangeAvailabilityResponse>?                       CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.TriggerMessageResponse>?                           CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.DataTransferResponse>?                             CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<CS.CertificateSignedResponse>?                        CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.InstallCertificateResponse>?                       CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetInstalledCertificateIdsResponse>?               CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CS.DeleteCertificateResponse>?                        CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyCRLResponse>?                                CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CS.GetLocalListVersionResponse>?                      CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SendLocalListResponse>?                            CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearCacheResponse>?                               CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.ReserveNowResponse>?                               CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.CancelReservationResponse>?                        CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStartTransactionResponse>?                  CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStopTransactionResponse>?                   CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetTransactionStatusResponse>?                     CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetChargingProfileResponse>?                       CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetChargingProfilesResponse>?                      CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearChargingProfileResponse>?                     CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCompositeScheduleResponse>?                     CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateDynamicScheduleResponse>?                    CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyAllowedEnergyTransferResponse>?              CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<CS.UsePriorityChargingResponse>?                      CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnlockConnectorResponse>?                          CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CS.AFRRSignalResponse>?                               CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.SetDisplayMessageResponse>?                        CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDisplayMessagesResponse>?                       CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearDisplayMessageResponse>?                      CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.CostUpdatedResponse>?                              CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.CustomerInformationResponse>?                      CustomCustomerInformationResponseSerializer                  { get; set; }

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


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }


        // E2E Charging Tariffs Extensions
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

        #region Constructor(s)

        /// <summary>
        /// Initialize a new HTTP server for the CSMS HTTP/WebSocket/JSON API.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSMSWSServer(String                               HTTPServiceName              = DefaultHTTPServiceName,
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

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   HTTPServiceName,

                   new[] {
                      "ocpp2.0.1",
                       Version.WebSocketSubProtocolId
                   },
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

                   DNSClient,
                   false)

        {

            this.RequireAuthentication           = RequireAuthentication;

            base.OnValidateTCPConnection        += ValidateTCPConnection;
            base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            base.OnCloseMessageReceived         += ProcessCloseMessage;


            foreach (var method in typeof(CSMSWSServer).GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                                        Where(method            => method.Name.StartsWith("Receive_") &&
                                                             (method.ReturnType == typeof(Task<Tuple<OCPP_WebSocket_ResponseMessage?,       OCPP_WebSocket_ErrorMessage?>>) ||
                                                              method.ReturnType == typeof(Task<Tuple<OCPP_WebSocket_BinaryResponseMessage?, OCPP_WebSocket_ErrorMessage?>>))))
            {

                // Create a delegate for the method and add it to the dictionary
                //  var del = (Func<JArray,
                //                  JObject,
                //                  Request_Id,
                //                  ChargingStation_Id,
                //                  WebSocketServerConnection,
                //                  String,
                //                  CancellationToken,
                //
                //                  Task <Tuple<OCPP_WebSocket_ResponseMessage?,
                //                              OCPP_WebSocket_ErrorMessage?>>>)
                //
                //                      Delegate.CreateDelegate(typeof(Func<JArray,
                //                                                          JObject,
                //                                                          Request_Id,
                //                                                          ChargingStation_Id,
                //                                                          WebSocketServerConnection,
                //                                                          String,
                //                                                          CancellationToken,
                //
                //                                                          Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                //                                                                     OCPP_WebSocket_ErrorMessage?>>>), method);

                var processorName = method.Name[8..];

                if (incomingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                incomingMessageProcessorsLookup.Add(processorName,
                                                    method);

            }

            if (AutoStart)
                Start();

        }

        #endregion


        #region AddOrUpdateHTTPBasicAuth(ChargingStationId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given charging station.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of the charging station.</param>
        /// <param name="Password">The password of the charging station.</param>
        public void AddOrUpdateHTTPBasicAuth(ChargingStation_Id  ChargingStationId,
                                             String              Password)
        {

            ChargingBoxLogins.AddOrUpdate(ChargingStationId,
                                          Password,
                                          (chargingStationId, password) => Password);

        }

        #endregion

        #region RemoveHTTPBasicAuth     (ChargingStationId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given charging station.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of the charging station.</param>
        public Boolean RemoveHTTPBasicAuth(ChargingStation_Id ChargingStationId)
        {

            if (ChargingBoxLogins.ContainsKey(ChargingStationId))
                return ChargingBoxLogins.TryRemove(ChargingStationId, out _);

            return true;

        }

        #endregion


        #region (protected) ValidateTCPConnection        (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<ConnectionFilterResponse> ValidateTCPConnection(DateTime                      LogTimestamp,
                                                                     AWebSocketServer              Server,
                                                                     System.Net.Sockets.TcpClient  Connection,
                                                                     EventTracking_Id              EventTrackingId,
                                                                     CancellationToken             CancellationToken)
        {

            return Task.FromResult(ConnectionFilterResponse.Accepted());

        }

        #endregion

        #region (protected) ValidateWebSocketConnection  (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<HTTPResponse?> ValidateWebSocketConnection(DateTime                   LogTimestamp,
                                                                AWebSocketServer           Server,
                                                                WebSocketServerConnection  Connection,
                                                                EventTracking_Id           EventTrackingId,
                                                                CancellationToken          CancellationToken)
        {

            #region Verify 'Sec-WebSocket-Protocol'...

            if (Connection.HTTPRequest?.SecWebSocketProtocol is null ||
                Connection.HTTPRequest?.SecWebSocketProtocol.Any() == false)
            {

                DebugX.Log("Missing 'Sec-WebSocket-Protocol' HTTP header!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                     JSONObject.Create(
                                                         new JProperty("en", "Missing 'Sec-WebSocket-Protocol' HTTP header!")
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }
            else if (!SecWebSocketProtocols.Overlaps(Connection.HTTPRequest?.SecWebSocketProtocol ?? Array.Empty<String>()))
            {

                var error = $"This WebSocket service only supports {(SecWebSocketProtocols.Select(id => $"'{id}'").AggregateWith(", "))}!";

                DebugX.Log(error);

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                         JSONObject.Create(
                                                             new JProperty("en", error)
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            #region Verify HTTP Authentication

            if (RequireAuthentication)
            {

                if (Connection.HTTPRequest?.Authorization is HTTPBasicAuthentication basicAuthentication)
                {

                    if (ChargingBoxLogins.TryGetValue(ChargingStation_Id.Parse(basicAuthentication.Username), out var password) &&
                        basicAuthentication.Password == password)
                    {
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " using authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            return Task.FromResult<HTTPResponse?>(null);

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        protected Task ProcessNewWebSocketConnection(DateTime                   LogTimestamp,
                                                     AWebSocketServer           Server,
                                                     WebSocketServerConnection  Connection,
                                                     EventTracking_Id           EventTrackingId,
                                                     CancellationToken          CancellationToken)
        {

            if (!Connection.HasCustomData(chargingStationId_WebSocketKey) &&
                Connection.HTTPRequest is not null &&
                ChargingStation_Id.TryParse(Connection.HTTPRequest.Path.ToString()[(Connection.HTTPRequest.Path.ToString().LastIndexOf("/") + 1)..], out var chargingStationId))
            {

                // Add the charging station identification to the WebSocket connection
                Connection.TryAddCustomData(chargingStationId_WebSocketKey, chargingStationId);

                if (!connectedChargingStations.ContainsKey(chargingStationId))
                     connectedChargingStations.TryAdd(chargingStationId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                else
                {

                    DebugX.Log($"{nameof(CSMSWSServer)} Duplicate charging station '{chargingStationId}' detected!");

                    var oldChargingStation_WebSocketConnection = connectedChargingStations[chargingStationId].Item1;

                    connectedChargingStations.TryRemove(chargingStationId, out _);
                    connectedChargingStations.TryAdd   (chargingStationId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                    try
                    {
                        oldChargingStation_WebSocketConnection.Close();
                    }
                    catch (Exception e)
                    {
                        DebugX.Log($"{nameof(CSMSWSServer)} Closing old HTTP WebSocket connection failed: {e.Message}");
                    }

                }

            }

            #region Send OnNewCSMSWSConnection event

            var OnNewCSMSWSConnectionLocal = OnNewCSMSWSConnection;
            if (OnNewCSMSWSConnectionLocal is not null)
            {

                OnNewCSMSWSConnection?.Invoke(LogTimestamp,
                                              this,
                                              Connection,
                                              EventTrackingId,
                                              CancellationToken);

            }

            #endregion

            return Task.CompletedTask;

        }

        #endregion

        #region (protected) ProcessCloseMessage          (LogTimestamp, Server, Connection, EventTrackingId, StatusCode, Reason)

        protected Task ProcessCloseMessage(DateTime                          LogTimestamp,
                                           AWebSocketServer                  Server,
                                           WebSocketServerConnection         Connection,
                                           EventTracking_Id                  EventTrackingId,
                                           WebSocketFrame.ClosingStatusCode  StatusCode,
                                           String?                           Reason)
        {

            if (Connection.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey, out var chargingStationId))
            {
                //DebugX.Log(nameof(CSMSWSServer), " Charging station " + chargingStationId + " disconnected!");
                connectedChargingStations.TryRemove(chargingStationId, out _);
            }

            return Task.CompletedTask;

        }

        #endregion


        #region (protected) ProcessTextMessage           (RequestTimestamp, Connection, OCPPTextMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="OCPPTextMessage">The received OCPP message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime                   RequestTimestamp,
                                                                                    WebSocketServerConnection  Connection,
                                                                                    String                     OCPPTextMessage,
                                                                                    EventTracking_Id           EventTrackingId,
                                                                                    CancellationToken          CancellationToken)
        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            var eventTrackingId = EventTracking_Id.New;

            try
            {

                var json = JArray.Parse(OCPPTextMessage);

                #region MessageType 2: CALL        (A request  from a charging station)

                // [
                //     2,                  // MessageType: CALL
                //    "19223201",          // A unique request identification
                //    "BootNotification",  // The OCPP action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (json.Count             == 4                   &&
                    json[0].Type           == JTokenType.Integer  &&
                    json[0].Value<Byte>()  == 2                   &&
                    json[1].Type == JTokenType.String             &&
                    json[2].Type == JTokenType.String             &&
                    json[3].Type == JTokenType.Object)
                {

                    #region OnTextMessageRequestReceived

                    var requestLogger = OnTextMessageRequestReceived;
                    if (requestLogger is not null)
                    {

                        var loggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnWebSocketTextMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                          this,
                                                                                                          Connection,
                                                                                                          eventTrackingId,
                                                                                                          Timestamp.Now,
                                                                                                          OCPPTextMessage)).
                                                        ToArray();

                        try
                        {
                            await Task.WhenAll(loggerTasks);
                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageRequestReceived));
                        }

                    }

                    #endregion

                    #region Initial checks

                    var chargingStationId  = Connection.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey);
                    var requestId          = Request_Id.TryParse(json[1]?.Value<String>() ?? "");
                    var action             = json[2]?.Value<String>()?.Trim();
                    var requestData        = json[3]?.Value<JObject>();

                    if (!chargingStationId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId ?? Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'charging station identity' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (!requestId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'request identification' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (action is null || action.IsNullOrEmpty())
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given 'action' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (requestData is null)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given request JSON payload must not be null!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    #endregion

                    #region Try to call the matching 'incoming message processor'

                    else if (incomingMessageProcessorsLookup.TryGetValue(action, out var methodInfo) &&
                             methodInfo is not null)
                    {

                        var result = methodInfo.Invoke(this,
                                                       [ json,
                                                         requestData,
                                                         requestId.Value,
                                                         chargingStationId.Value,
                                                         Connection,
                                                         OCPPTextMessage,
                                                         CancellationToken ]);

                        if (result is Task<Tuple<OCPP_WebSocket_ResponseMessage?, OCPP_WebSocket_ErrorMessage?>> textProcessor) {
                            (OCPPResponse, OCPPErrorResponse) = await textProcessor;
                        }

                    }

                    #endregion

                    else
                    {

                        DebugX.Log($"{nameof(CSMSWSServer)}: The OCPP message '{action}' is unkown!");

                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 $"The OCPP message '{action}' is unkown!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    }

                    #region OnTextMessageResponseSent

                    var now = Timestamp.Now;

                    if (OCPPResponse is not null || OCPPErrorResponse is not null)
                    {

                        var logger = OnTextMessageResponseSent;
                        if (logger is not null)
                        {

                            var loggerTasks = logger.GetInvocationList().
                                                     OfType <OnWebSocketTextMessageResponseDelegate>().
                                                     Select (loggingDelegate => loggingDelegate.Invoke(now,
                                                                                                       this,
                                                                                                       Connection,
                                                                                                       eventTrackingId,
                                                                                                       RequestTimestamp,
                                                                                                       json.ToString(JSONFormatting),
                                                                                                       now,
                                                                                                       (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting))).

                                                                                                       //ToDo: For some use cases returning an error to a charging station might be useless!

                                                                                                       //OCPPResponse?.ToJSON()?.ToString(JSONFormatting));
                                                     ToArray();

                            try
                            {
                                await Task.WhenAll(loggerTasks);
                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageResponseSent));
                            }

                        }

                    }

                    #endregion

                }

                #endregion

                #region MessageType 3: CALLRESULT  (A response from a charging station)

                // [
                //     3,                         // MessageType: CALLRESULT
                //    "19223201",                 // The request identification copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                else if (json.Count             == 3         &&
                         json[0].Type == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 3         &&
                         json[1].Type == JTokenType.String   &&
                         json[2].Type == JTokenType.Object)
                {

                    if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                        requests.TryGetValue(requestId, out var request))
                    {

                        request.ResponseTimestamp  = Timestamp.Now;
                        request.Response           = json[2] as JObject;

                        #region OnTextMessageResponseReceived

                        try
                        {

                            OnTextMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  Connection,
                                                                  EventTracking_Id.New,
                                                                  request.Timestamp,
                                                                  request.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                  Timestamp.Now,
                                                                  request.Response?.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageResponseReceived));
                        }

                        #endregion

                    }

                    // No response to the charging station!

                }

                #endregion

                #region MessageType 4: CALLERROR   (A charging station reports an error for a received request)

                // [
                //     4,                         // MessageType: CALLERROR
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                // Error Code                    Description
                // -----------------------------------------------------------------------------------------------
                // NotImplemented                Requested Action is not known by receiver
                // NotSupported                  Requested Action is recognized but not supported by the receiver
                // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
                // ProtocolError                 Payload for Action is incomplete
                // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
                // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
                // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
                // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
                // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
                // GenericError                  Any other error not covered by the previous ones

                else if (json.Count             == 5                   &&
                         json[0].Type           == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 4                   &&
                         json[1].Type == JTokenType.String             &&
                         json[2].Type == JTokenType.String             &&
                         json[3].Type == JTokenType.String             &&
                         json[4].Type == JTokenType.Object)
                {

                    if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                        requests.TryGetValue(requestId, out var request))
                    {

                        // ToDo: Refactor 
                        if (ResultCodes.TryParse(json[2]?.Value<String>() ?? "", out var errorCode))
                            request.ErrorCode = errorCode;
                        else
                            request.ErrorCode = ResultCodes.GenericError;

                        request.Response          = null;
                        request.ErrorDescription  = json[3]?.Value<String>();
                        request.ErrorDetails      = json[4] as JObject;

                        #region OnTextErrorResponseReceived

                        try
                        {

                            OnTextErrorResponseReceived?.Invoke(Timestamp.Now,
                                                                this,
                                                                Connection,
                                                                EventTracking_Id.New,
                                                                request.Timestamp,
                                                                request.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                Timestamp.Now,
                                                                request.Response?.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextErrorResponseReceived));
                        }

                        #endregion

                    }

                    // No response to the charging station!

                }

                #endregion

                #region Unknown message structure

                else
                {

                    OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                            Request_Id.Zero,
                                            ResultCodes.InternalError,
                                            $"{nameof(CSMSWSServer)} The OCPP message '{OCPPTextMessage}' is invalid!",
                                            new JObject(
                                                new JProperty("request", OCPPTextMessage)
                                            )
                                        );

                    DebugX.Log(OCPPErrorResponse.ErrorDescription);

                }

                #endregion

            }
            catch (Exception e)
            {
                #region Handle exception

                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                        Request_Id.Zero,
                                        ResultCodes.InternalError,
                                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(CSMSWSServer) + " led to an exception!",
                                        new JObject(
                                            new JProperty("request",      OCPPTextMessage),
                                            new JProperty("exception",    e.Message),
                                            new JProperty("stacktrace",   e.StackTrace)
                                        )
                                    );

                DebugX.LogException(e, OCPPErrorResponse.ErrorDescription);

                #endregion
            }


            #region OnTextErrorResponseSent

            if (OCPPErrorResponse is not null)
            {

                var now    = Timestamp.Now;
                var logger = OnTextErrorResponseSent;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketTextErrorResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(now,
                                                                                               this,
                                                                                               Connection,
                                                                                               eventTrackingId,
                                                                                               RequestTimestamp,
                                                                                               OCPPTextMessage,
                                                                                               now,
                                                                                               (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting))).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextErrorResponseSent));
                    }

                }

            }

            #endregion


            // The response to the charging station... might be empty!
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       OCPPTextMessage,
                       Timestamp.Now,
                       (OCPPResponse?.     ToJSON() ??
                        OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting) ?? String.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region (protected) ProcessBinaryMessage         (RequestTimestamp, Connection, OCPPBinaryMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="OCPPBinaryMessage">The received OCPP binary message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime                   RequestTimestamp,
                                                                                        WebSocketServerConnection  Connection,
                                                                                        Byte[]                     OCPPBinaryMessage,
                                                                                        EventTracking_Id           EventTrackingId,
                                                                                        CancellationToken          CancellationToken)
        {

            var requestBinaryData  = OCPPBinaryMessage;
            var requestId          = new Request_Id?        (Request_Id.        Parse("1"));
            var chargingStationId  = new ChargingStation_Id?(ChargingStation_Id.Parse("2"));

            OCPP_WebSocket_BinaryResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?          OCPPErrorResponse   = null;


            var action       = "BinaryDataTransfer";
            var json         = new JArray();
            var requestData  = OCPPBinaryMessage;

            #region Try to call the matching 'incoming message processor'

            if (incomingMessageProcessorsLookup.TryGetValue(action, out var methodInfo) &&
                             methodInfo is not null)
            {

                var result = methodInfo.Invoke(this,
                                               [ json,
                                                 requestData,
                                                 requestId.Value,
                                                 chargingStationId.Value,
                                                 Connection,
                                                 OCPPBinaryMessage,
                                                 CancellationToken ]);

                if (result is Task<Tuple<OCPP_WebSocket_BinaryResponseMessage?, OCPP_WebSocket_ErrorMessage?>> binaryProcessor)
                {
                    (OCPPResponse, OCPPErrorResponse) = await binaryProcessor;
                }

            }

            #endregion


            return new WebSocketBinaryMessageResponse(
                       RequestTimestamp,
                       OCPPBinaryMessage,
                       Timestamp.Now,
                       OCPPResponse.ToByteArray(),
                       EventTrackingId
                   );

        }

        #endregion


        #region SendRequest(EventTrackingId, RequestId, ChargingStationId, OCPPAction, JSONPayload, RequestTimeout = null)

        public async Task<SendRequestState> SendRequest(EventTracking_Id  EventTrackingId,
                                                        Request_Id        RequestId,
                                                        ChargingStation_Id      ChargingStationId,
                                                        String            OCPPAction,
                                                        JObject           JSONPayload,
                                                        TimeSpan?         RequestTimeout   = null)
        {

            var endTime         = Timestamp.Now + (RequestTimeout ?? this.RequestTimeout ?? DefaultRequestTimeout);

            var sendJSONResult  = await SendJSON(
                                      EventTrackingId,
                                      RequestId,
                                      ChargingStationId,
                                      OCPPAction,
                                      JSONPayload,
                                      endTime
                                  );

            if (sendJSONResult == SendJSONResults.Success) {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25);

                        if (requests.TryGetValue(RequestId, out var sendRequestState1) &&
                            sendRequestState1?.Response is not null ||
                            sendRequestState1?.ErrorCode.HasValue == true)
                        {

                            requests.TryRemove(RequestId, out _);

                            return sendRequestState1;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(CSMSWSServer), ".", nameof(SendRequest), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                if (requests.TryGetValue(RequestId, out var sendRequestState2) && sendRequestState2 is not null)
                {
                    sendRequestState2.ErrorCode = ResultCodes.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                if (requests.TryGetValue(RequestId, out var sendRequestState3) && sendRequestState3 is not null)
                {
                    sendRequestState3.ErrorCode = ResultCodes.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState3;
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return new SendRequestState(
                       Timestamp:           now,
                       ChargingStationId:         ChargingStationId,
                       WSRequestMessage:    new OCPP_WebSocket_RequestMessage(
                                                RequestId,
                                                OCPPAction,
                                                JSONPayload
                                            ),
                       Timeout:             now,
                       ResponseTimestamp:   now,
                       Response:            new JObject(),
                       ErrorCode:           ResultCodes.InternalError,
                       ErrorDescription:    null,
                       ErrorDetails:        null
                   );

        }

        #endregion

        #region SendJSON   (EventTrackingId, RequestId, ChargingStationId, Action, JSON, RequestTimeout)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="ChargingStationId">A charging station identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="JSON">The JSON payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public async Task<SendJSONResults> SendJSON(EventTracking_Id  EventTrackingId,
                                                    Request_Id        RequestId,
                                                    ChargingStation_Id      ChargingStationId,
                                                    String            Action,
                                                    JObject           JSON,
                                                    DateTime          RequestTimeout)
        {

            var wsRequestMessage  = new OCPP_WebSocket_RequestMessage(
                                        RequestId,
                                        Action,
                                        JSON
                                    );

            var ocppTextMessage   = wsRequestMessage.ToJSON().ToString(Formatting.None);

            try
            {

                var webSocketConnections  = WebSocketConnections.Where  (ws => ws.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey) == ChargingStationId).
                                                                 ToArray();

                if (webSocketConnections.Any())
                {

                    requests.TryAdd(RequestId,
                                    new SendRequestState(
                                        Timestamp.Now,
                                        ChargingStationId,
                                        wsRequestMessage,
                                        RequestTimeout
                                    ));

                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        #region OnTextMessageRequestSent

                        var requestLogger = OnTextMessageRequestSent;
                        if (requestLogger is not null)
                        {

                            var loggerTasks = requestLogger.GetInvocationList().
                                                            OfType <OnWebSocketTextMessageDelegate>().
                                                            Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                              this,
                                                                                                              webSocketConnection,
                                                                                                              EventTrackingId,
                                                                                                              ocppTextMessage)).
                                                            ToArray();

                            try
                            {
                                await Task.WhenAll(loggerTasks);
                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageRequestSent));
                            }

                        }

                        #endregion

                        var success = await SendTextMessage(webSocketConnection,
                                                            ocppTextMessage,
                                                            EventTrackingId);

                        if (success == SendStatus.Success)
                            break;

                        else
                            RemoveConnection(webSocketConnection);

                    }

                    return SendJSONResults.Success;

                }
                else
                    return SendJSONResults.UnknownClient;

            }
            catch (Exception)
            {
                return SendJSONResults.TransmissionFailed;
            }

        }

        #endregion


    }

}
