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
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketConnection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WSClientRequestLogHandler(DateTime                   Timestamp,
                                                   WebSocketClientConnection  WebSocketConnection,
                                                   ChargingStation_Id         chargingStationId,
                                                   EventTracking_Id           EventTrackingId,
                                                   JObject                    Request);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketConnection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    /// <param name="ErrorResponse">The outgoing WebSocket error response.</param>
    public delegate Task WSClientResponseLogHandler(DateTime                   Timestamp,
                                                    WebSocketClientConnection  WebSocketConnection,
                                                    JObject                    Request,
                                                    JObject?                   Response,
                                                    JArray?                    ErrorResponse);


    public delegate Task  OnWebSocketClientTextMessageResponseDelegate  (DateTime                  Timestamp,
                                                                         ChargingStationWSClient   Client,
                                                                         EventTracking_Id          EventTrackingId,
                                                                         DateTime                  RequestTimestamp,
                                                                         String                    RequestMessage,
                                                                         DateTime                  ResponseTimestamp,
                                                                         String                    ResponseMessage);


    public delegate Task  OnWebSocketClientBinaryMessageResponseDelegate(DateTime                  Timestamp,
                                                                         ChargingStationWSClient   Client,
                                                                         EventTracking_Id          EventTrackingId,
                                                                         DateTime                  RequestTimestamp,
                                                                         Byte[]                    RequestMessage,
                                                                         DateTime                  ResponseTimestamp,
                                                                         Byte[]                    ResponseMessage);


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region (class) SendRequestState2

        public class SendRequestState2
        {

            public DateTime                              Timestamp                  { get; }
            public OCPP_WebSocket_RequestMessage?        WSRequestMessage           { get; }
            public OCPP_WebSocket_BinaryRequestMessage?  WSBinaryRequestMessage     { get; }
            public DateTime                              Timeout                    { get; }

            public DateTime?                             ResponseTimestamp          { get; set; }
            public JObject?                              Response                   { get; set; }
            public Byte[]?                               BinaryResponse             { get; set; }

            public ResultCodes?                          ErrorCode                  { get; set; }
            public String?                               ErrorDescription           { get; set; }
            public JObject?                              ErrorDetails               { get; set; }


            public Boolean                              NoErrors
                 => !ErrorCode.HasValue;

            public Boolean                              HasErrors
                 =>  ErrorCode.HasValue;


            public SendRequestState2(DateTime                             Timestamp,
                                     OCPP_WebSocket_RequestMessage        WSRequestMessage,
                                     DateTime                             Timeout,

                                     DateTime?                            ResponseTimestamp   = null,
                                     JObject?                             Response            = null,

                                     ResultCodes?                         ErrorCode           = null,
                                     String?                              ErrorDescription    = null,
                                     JObject?                             ErrorDetails        = null)
            {

                this.Timestamp          = Timestamp;
                this.WSRequestMessage   = WSRequestMessage;
                this.Timeout            = Timeout;

                this.ResponseTimestamp  = ResponseTimestamp;
                this.Response           = Response;

                this.ErrorCode          = ErrorCode;
                this.ErrorDescription   = ErrorDescription;
                this.ErrorDetails       = ErrorDetails;

            }

            public SendRequestState2(DateTime                             Timestamp,
                                     OCPP_WebSocket_BinaryRequestMessage  WSBinaryRequestMessage,
                                     DateTime                             Timeout,

                                     DateTime?                            ResponseTimestamp   = null,
                                     JObject?                             Response            = null,

                                     ResultCodes?                         ErrorCode           = null,
                                     String?                              ErrorDescription    = null,
                                     JObject?                             ErrorDetails        = null)
            {

                this.Timestamp               = Timestamp;
                this.WSBinaryRequestMessage  = WSBinaryRequestMessage;
                this.Timeout                 = Timeout;

                this.ResponseTimestamp       = ResponseTimestamp;
                this.Response                = Response;

                this.ErrorCode               = ErrorCode;
                this.ErrorDescription        = ErrorDescription;
                this.ErrorDetails            = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const        String                                               DefaultHTTPUserAgent              = $"GraphDefined OCPP {Version.String} CP WebSocket Client";

        private const           String                                               LogfileName                       = "ChargePointWSClient.log";

        public static readonly  TimeSpan                                             DefaultRequestTimeout             = TimeSpan.FromSeconds(30);

        public readonly         ConcurrentDictionary<Request_Id, SendRequestState2>  requests                          = [];

        private readonly        Dictionary<String, MethodInfo>                       incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charging station.
        /// </summary>
        public ChargingStation_Id                    ChargingStationIdentity         { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargingStationIdentity.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From                            { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                              { get; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                            JSONFormatting                  { get; set; } = Formatting.None;

        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        //public ChargePointWSClient.CPClientLogger    Logger                          { get; }

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<StatusInfo>?                                CustomStatusInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                     CustomClearMonitoringResultSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ChargingStation>?                           CustomChargingStationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                 CustomSignatureSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                 CustomEventDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                 CustomComponentSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableResult>?                         CustomSetVariableResultSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                         CustomGetVariableResultSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                       CustomSetMonitoringResultSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                      CustomEVSESerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                  CustomVariableSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                        CustomVariableMonitoringSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                CustomReportDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                         CustomVariableAttributeSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                   CustomVariableCharacteristicsSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                            CustomMonitoringDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                           CustomOCSPRequestDataSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                   CustomIdTokenSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                            CustomAdditionalInfoSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingNeeds>?                             CustomChargingNeedsSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                      CustomACChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                      CustomDCChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                     CustomV2XChargingParametersSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                             CustomEVEnergyOfferSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                           CustomEVPowerScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                      CustomEVPowerScheduleEntrySerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                   CustomEVAbsolutePriceScheduleSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?              CustomEVAbsolutePriceScheduleEntrySerializer                { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                               CustomEVPriceRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<Transaction>?                               CustomTransactionSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                CustomMeterValueSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                              CustomSampledValueSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                          CustomSignedMeterValueSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                            CustomUnitsOfMeasureSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                          CustomChargingScheduleSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                            CustomLimitBeyondSoCSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                         CustomCompositeScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                    CustomChargingSchedulePeriodSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                          CustomV2XFreqWattEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                        CustomV2XSignalWattEntrySerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                               CustomSalesTariffSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                          CustomSalesTariffEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                      CustomRelativeTimeIntervalSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                           CustomConsumptionCostSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                      CustomCostSerializer                                        { get; set; }

        public CustomJObjectSerializerDelegate<AbsolutePriceSchedule>?                     CustomAbsolutePriceScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PriceRuleStack>?                            CustomPriceRuleStackSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<PriceRule>?                                 CustomPriceRuleSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<TaxRule>?                                   CustomTaxRuleSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRuleList>?                          CustomOverstayRuleListSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRule>?                              CustomOverstayRuleSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalService>?                         CustomAdditionalServiceSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<PriceLevelSchedule>?                        CustomPriceLevelScheduleSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>?                   CustomPriceLevelScheduleEntrySerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfile>?                           CustomChargingProfileSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                               CustomMessageInfoSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                            CustomMessageContentSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                       CustomCertificateHashDataSerializer                         { get; set; }

        #endregion

        #region Events

        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseReceived;
        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseSent;

        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station websocket client running on a charging station
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="ChargingStationIdentity">The unique identification of this charging station.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargingStationWSClient(ChargingStation_Id                   ChargingStationIdentity,
                                       String                               From,
                                       String                               To,

                                       URL                                  RemoteURL,
                                       HTTPHostname?                        VirtualHostname              = null,
                                       String?                              Description                  = null,
                                       Boolean?                             PreferIPv4                   = null,
                                       RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                       X509Certificate?                     ClientCert                   = null,
                                       SslProtocols?                        TLSProtocol                  = null,
                                       String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                       IHTTPAuthentication?                 HTTPAuthentication           = null,
                                       TimeSpan?                            RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                       UInt16?                              MaxNumberOfRetries           = 3,
                                       UInt32?                              InternalBufferSize           = null,

                                       IEnumerable<String>?                 SecWebSocketProtocols        = null,

                                       Boolean                              DisableWebSocketPings        = false,
                                       TimeSpan?                            WebSocketPingEvery           = null,
                                       TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                       Boolean                              DisableMaintenanceTasks      = false,
                                       TimeSpan?                            MaintenanceEvery             = null,

                                       String?                              LoggingPath                  = null,
                                       String                               LoggingContext               = null, //CPClientLogger.DefaultContext,
                                       LogfileCreatorDelegate?              LogfileCreator               = null,
                                       HTTPClientLogger?                    HTTPLogger                   = null,
                                       DNSClient?                           DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout ?? DefaultRequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,

                   SecWebSocketProtocols,

                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (ChargingStationIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargingStationIdentity),  "The given charging station identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

            #endregion

            this.ChargingStationIdentity                  = ChargingStationIdentity;
            this.From                               = From;
            this.To                                 = To;

            //this.Logger                             = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                                   LoggingPath,
            //                                                                                   LoggingContext,
            //                                                                                   LogfileCreator);

            foreach (var method in typeof(ChargingStationWSClient).GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                                                   Where(method            => method.Name.StartsWith("Receive_") &&
                                                                        (method.ReturnType == typeof(Task<Tuple<OCPP_WebSocket_ResponseMessage?,       OCPP_WebSocket_ErrorMessage?>>) ||
                                                                         method.ReturnType == typeof(Task<Tuple<OCPP_WebSocket_BinaryResponseMessage?, OCPP_WebSocket_ErrorMessage?>>))))
            {

                var processorName = method.Name[8..];

                if (incomingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                incomingMessageProcessorsLookup.Add(processorName,
                                                    method);

            }

        }

        #endregion


        #region ProcessWebSocketTextFrame  (RequestTimestamp, Connection, TextMessage,   EventTrackingId, CancellationToken)

        public override async Task ProcessWebSocketTextFrame(DateTime                   RequestTimestamp,
                                                             WebSocketClientConnection  Connection,
                                                             EventTracking_Id           EventTrackingId,
                                                             String                     TextMessage,
                                                             CancellationToken          CancellationToken)
        {

            if (TextMessage == "[]")
            {
                DebugX.Log(nameof(ChargingStationWSClient), " [] received!");
                return;
            }

            try
            {

                var jsonArray = JArray.Parse(TextMessage);

                if      (OCPP_WebSocket_RequestMessage. TryParse(jsonArray, out var ocppRequest)       && ocppRequest       is not null)
                {

                    OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
                    OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(ocppRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ RequestTimestamp,
                                                         Connection,
                                                         ChargingStationIdentity,
                                                         EventTrackingId,
                                                         TextMessage,
                                                         ocppRequest.RequestId,
                                                         ocppRequest.Message,
                                                         CancellationToken ]);

                        #endregion

                        if (result is Task<Tuple<OCPP_WebSocket_ResponseMessage?, OCPP_WebSocket_ErrorMessage?>> textProcessor)
                        {

                            (OCPPResponse, OCPPErrorResponse) = await textProcessor;

                            #region Send response...

                            if (OCPPResponse is not null)
                                await SendText(
                                          OCPPResponse.ToJSON().
                                                       ToString(JSONFormatting)
                                      );

                            #endregion

                            #region ..., or send error response!

                            if (OCPPErrorResponse is not null)
                            {
                                // CALL RESULT ERROR: New in OCPP v2.1++
                            }

                            #endregion


                            #region OnTextMessageResponseSent

                            try
                            {

                                OnTextMessageResponseSent?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTracking_Id.New,
                                                                  RequestTimestamp,
                                                                  TextMessage,
                                                                  Timestamp.Now,
                                                                  OCPPResponse?.ToJSON().ToString(JSONFormatting) ?? OCPPErrorResponse?.ToJSON().ToString(JSONFormatting) ?? "");

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTextMessageResponseSent));
                            }

                            #endregion

                        }

                    }

                }

                else if (OCPP_WebSocket_ResponseMessage.TryParse(jsonArray, out var ocppResponse)      && ocppResponse      is not null)
                {

                    if (requests.TryGetValue(ocppResponse.RequestId, out var sendRequestState))
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.Response           = ocppResponse.Message;

                        #region OnTextMessageResponseReceived

                        try
                        {

                            OnTextMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTracking_Id.New,
                                                                  sendRequestState.Timestamp,
                                                                  sendRequestState.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                  sendRequestState.ResponseTimestamp.Value,
                                                                  sendRequestState.Response.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTextMessageResponseReceived));
                        }

                        #endregion

                    }

                    else
                        DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP response message: " + TextMessage);

                }

                else if (OCPP_WebSocket_ErrorMessage.   TryParse(jsonArray, out var ocppErrorResponse) && ocppErrorResponse is not null)
                {
                    DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP error message: " + TextMessage);
                }

                else
                    DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP request/response message: " + TextMessage);

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(ChargingStationWSClient) + "." + nameof(ProcessWebSocketTextFrame));

                //OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                //                        Request_Id.Zero,
                //                        ResultCodes.InternalError,
                //                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(ChargingStationWSClient) + " led to an exception!",
                //                        new JObject(
                //                            new JProperty("request",      OCPPTextMessage),
                //                            new JProperty("exception",    e.Message),
                //                            new JProperty("stacktrace",   e.StackTrace)
                //                        )
                //                    );

            }

        }

        #endregion

        #region ProcessWebSocketBinaryFrame(RequestTimestamp, Connection, BinaryMessage, EventTrackingId, CancellationToken)

        public override async Task ProcessWebSocketBinaryFrame(DateTime                   RequestTimestamp,
                                                               WebSocketClientConnection  Connection,
                                                               EventTracking_Id           EventTrackingId,
                                                               Byte[]                     BinaryMessage,
                                                               CancellationToken          CancellationToken)
        {

            if (BinaryMessage.Length == 0)
            {
                DebugX.Log(nameof(ChargingStationWSClient), " [] received!");
                return;
            }

            try
            {

                     if (OCPP_WebSocket_BinaryRequestMessage. TryParse(BinaryMessage, out var ocppRequest)  && ocppRequest  is not null)
                {

                    OCPP_WebSocket_BinaryResponseMessage? OCPPResponse        = null;
                    OCPP_WebSocket_ErrorMessage?          OCPPErrorResponse   = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(ocppRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                            #region Call 'incoming message' processor

                            var result = methodInfo.Invoke(this,
                                                           [ RequestTimestamp,
                                                             Connection,
                                                             ChargingStationIdentity,
                                                             EventTrackingId,
                                                             BinaryMessage,
                                                             ocppRequest.RequestId,
                                                             ocppRequest.BinaryMessage,
                                                             CancellationToken ]);

                            #endregion

                            if (result is Task<Tuple<OCPP_WebSocket_BinaryResponseMessage?, OCPP_WebSocket_ErrorMessage?>> binaryProcessor)
                            {

                                (OCPPResponse, OCPPErrorResponse) = await binaryProcessor;

                                #region Send response...

                                if (OCPPResponse is not null)
                                    await SendBinary(
                                              OCPPResponse.ToByteArray()
                                          );

                                #endregion

                                #region ..., or send error response!

                                if (OCPPErrorResponse is not null)
                                {
                                    // CALL RESULT ERROR: New in OCPP v2.1++
                                }

                                #endregion


                                #region OnBinaryMessageResponseSent

                                try
                                {

                                    OnBinaryMessageResponseSent?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        EventTracking_Id.New,
                                                                        RequestTimestamp,
                                                                        BinaryMessage,
                                                                        Timestamp.Now,
                                                                        OCPPResponse?.ToByteArray() ?? OCPPErrorResponse?.ToByteArray() ?? []);

                                }
                                catch (Exception e)
                                {
                                    DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBinaryMessageResponseSent));
                                }

                                #endregion

                            }
                
                
                
                    }



                    Byte[]?                      OCPPResponseJSON   = null;
                    OCPP_WebSocket_ErrorMessage? ErrorMessage       = null;

                    switch (ocppRequest.Action)
                    {

                        case "BinaryDataTransfer":
                            {



                            }
                            break;

                    }

                    if (OCPPResponseJSON is not null)
                    {

                        await SendBinary(
                                  new OCPP_WebSocket_BinaryResponseMessage(
                                      ocppRequest.RequestId,
                                      OCPPResponseJSON
                                  ).ToByteArray()
                              );

                        #region OnBinaryMessageResponseSent

                        try
                        {

                            OnBinaryMessageResponseSent?.Invoke(Timestamp.Now,
                                                                this,
                                                                EventTracking_Id.New,
                                                                RequestTimestamp,
                                                                ocppRequest.BinaryMessage,
                                                                Timestamp.Now,
                                                                OCPPResponseJSON);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTextMessageResponseSent));
                        }

                        #endregion

                    }

                }

                else if (OCPP_WebSocket_BinaryResponseMessage.TryParse(BinaryMessage, out var ocppResponse) && ocppResponse is not null)
                {

                    if (requests.TryGetValue(ocppResponse.RequestId, out var sendRequestState))
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.BinaryResponse     = ocppResponse.BinaryMessage;

                        #region OnBinaryMessageResponseReceived

                        try
                        {

                            OnBinaryMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                    this,
                                                      //              OCPPBinaryMessage,
                                                                    EventTracking_Id.New,
                                                                    sendRequestState.Timestamp,
                                                                    sendRequestState.WSBinaryRequestMessage?.ToByteArray() ?? [],
                                                                    sendRequestState.ResponseTimestamp.Value,
                                                                    sendRequestState.BinaryResponse);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBinaryMessageResponseReceived));
                        }

                        #endregion

                    }

                    else
                        DebugX.Log(nameof(ChargingStationWSClient), " Received unknown binary OCPP response message!");

                }

                else
                    DebugX.Log(nameof(ChargingStationWSClient), " Received unknown binary OCPP request/response message!");

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(ChargingStationWSClient) + "." + nameof(ProcessWebSocketBinaryFrame));

                //OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                //                        Request_Id.Zero,
                //                        ResultCodes.InternalError,
                //                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(ChargingStationWSClient) + " led to an exception!",
                //                        new JObject(
                //                            new JProperty("request",      OCPPTextMessage),
                //                            new JProperty("exception",    e.Message),
                //                            new JProperty("stacktrace",   e.StackTrace)
                //                        )
                //                    );

            }

        }

        #endregion


        #region SendRequest(Action, RequestId, Message)

        public async Task<OCPP_WebSocket_RequestMessage> SendRequest(String      Action,
                                                                     Request_Id  RequestId,
                                                                     JObject     Message)
        {

            OCPP_WebSocket_RequestMessage? wsRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                               RequestId,
                                               Action,
                                               Message
                                           );

                        await SendText(wsRequestMessage.
                                       ToJSON().
                                       ToString(JSONFormatting));

                        requests.TryAdd(RequestId,
                                        new SendRequestState2(
                                            Timestamp.Now,
                                            wsRequestMessage,
                                            Timestamp.Now + RequestTimeout
                                        ));

                    }
                    else
                    {

                        wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                               RequestId,
                                               Action,
                                               Message,
                                               ErrorMessage: "Invalid WebSocket connection!"
                                           );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                           RequestId,
                                           Action,
                                           Message,
                                           ErrorMessage: e.Message
                                       );

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }

            else
                wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                       RequestId,
                                       Action,
                                       Message,
                                       ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                   );

            return wsRequestMessage;

        }

        #endregion

        #region SendRequest(Action, RequestId, BinaryMessage)

        public async Task<OCPP_WebSocket_BinaryRequestMessage> SendRequest(String      Action,
                                                                           Request_Id  RequestId,
                                                                           Byte[]      BinaryMessage)
        {

            OCPP_WebSocket_BinaryRequestMessage? wsRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        wsRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                               RequestId,
                                               Action,
                                               BinaryMessage
                                           );

                        await SendBinary(wsRequestMessage.BinaryMessage); //ToDo: Fix me!

                        requests.TryAdd(RequestId,
                                        new SendRequestState2(
                                            Timestamp.Now,
                                            wsRequestMessage,
                                            Timestamp.Now + RequestTimeout
                                        ));

                    }
                    else
                    {

                        wsRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                               RequestId,
                                               Action,
                                               BinaryMessage,
                                               ErrorMessage: "Invalid WebSocket connection!"
                                           );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    wsRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                           RequestId,
                                           Action,
                                           BinaryMessage,
                                           ErrorMessage: e.Message
                                       );

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }

            else
                wsRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                       RequestId,
                                       Action,
                                       BinaryMessage,
                                       ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                   );

            return wsRequestMessage;

        }

        #endregion


        #region (private) WaitForResponse(RequestMessage)

        private async Task<SendRequestState2> WaitForResponse(OCPP_WebSocket_RequestMessage RequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(RequestMessage.RequestId, out var sendRequestState2) &&
                       (sendRequestState2?.Response is not null ||
                        sendRequestState2?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(RequestMessage.RequestId, out _);

                        return sendRequestState2;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargingStationWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return new SendRequestState2(
                       Timestamp:          Timestamp.Now,
                       WSRequestMessage:   RequestMessage,
                       Timeout:            endTime,

                       Response:           null,
                       ErrorCode:          ResultCodes.Timeout,
                       ErrorDescription:   null,
                       ErrorDetails:       null
                   );

        }

        #endregion

        #region (private) WaitForResponse(BinaryRequestMessage)

        private async Task<SendRequestState2> WaitForResponse(OCPP_WebSocket_BinaryRequestMessage BinaryRequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var sendRequestState2) &&
                       (sendRequestState2?.Response is not null ||
                        sendRequestState2?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(BinaryRequestMessage.RequestId, out _);

                        return sendRequestState2;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargingStationWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return new SendRequestState2(
                       Timestamp:                Timestamp.Now,
                       WSBinaryRequestMessage:   BinaryRequestMessage,
                       Timeout:                  endTime,

                       Response:                 null,
                       ErrorCode:                ResultCodes.Timeout,
                       ErrorDescription:         null,
                       ErrorDetails:             null
                   );

        }

        #endregion


    }

}
