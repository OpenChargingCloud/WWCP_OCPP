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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The networking node HTTP WebSocket client runs on a networking node
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeServer,
                                                  INetworkingNodeClientEvents
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const        String                                              DefaultHTTPUserAgent              = $"GraphDefined OCPP {Version.String} CP WebSocket Client";

        private const           String                                              LogfileName                       = "ChargePointWSClient.log";

        public static readonly  TimeSpan                                            DefaultRequestTimeout             = TimeSpan.FromSeconds(30);

        public readonly         ConcurrentDictionary<Request_Id, SendRequestState>  requests                          = [];

        private readonly        Dictionary<String, MethodInfo>                      incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this networking node.
        /// </summary>
        public NetworkingNode_Id                     NetworkingNodeIdentity         { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => NetworkingNodeIdentity.ToString();

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
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                            CustomSignatureSerializer                                   { get; set; }
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


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<OCPP.Signature>?                             CustomBinarySignatureSerializer                             { get; set; }

        #endregion

        #region Events

        public event OnWebSocketClientJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;
        public event OnWebSocketClientJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node websocket client running on a networking node
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="NetworkingNodeIdentity">The unique identification of this networking node.</param>
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
        public NetworkingNodeWSClient(NetworkingNode_Id                    NetworkingNodeIdentity,
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

            if (NetworkingNodeIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NetworkingNodeIdentity),  "The given networking node identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),                     "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                       "The given websocket message destination must not be null or empty!");

            #endregion

            this.NetworkingNodeIdentity  = NetworkingNodeIdentity;
            this.From                    = From;
            this.To                      = To;

            //this.Logger                   = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                         LoggingPath,
            //                                                                         LoggingContext,
            //                                                                         LogfileCreator);

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var method in typeof(NetworkingNodeWSClient).
                                       GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                            Where(method            => method.Name.StartsWith("Receive_") &&
                                                 (method.ReturnType == typeof(Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONErrorMessage?>>) ||
                                                  method.ReturnType == typeof(Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>>))))
            {

                var processorName = method.Name[8..];

                if (incomingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                incomingMessageProcessorsLookup.Add(processorName,
                                                    method);

            }

            #endregion

        }

        #endregion


        #region ProcessWebSocketTextFrame  (RequestTimestamp, Connection, TextMessage,   EventTrackingId, CancellationToken)

        public override async Task ProcessWebSocketTextFrame(DateTime                   RequestTimestamp,
                                                             WebSocketClientConnection  Connection,
                                                             EventTracking_Id           EventTrackingId,
                                                             String                     TextMessage,
                                                             CancellationToken          CancellationToken)
        {

            if (TextMessage == "[]" ||
                TextMessage.IsNullOrEmpty())
            {
                DebugX.Log($"Received an empty JSON message within {nameof(NetworkingNodeWSClient)}!");
                return;
            }

            try
            {

                var jsonArray = JArray.Parse(TextMessage);

                if      (OCPP_JSONRequestMessage. TryParse(jsonArray, out var jsonRequest,  out var requestParsingError)  && jsonRequest       is not null)
                {

                    OCPP_JSONResponseMessage?    OCPPJSONResponse     = null;
                    OCPP_BinaryResponseMessage?  OCPPBinaryResponse   = null;
                    OCPP_JSONErrorMessage?       OCPPErrorResponse    = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(jsonRequest.Action, out var methodInfo) && methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ RequestTimestamp,
                                                         Connection,
                                                         NetworkingNodeIdentity,
                                                         NetworkPath.Empty,
                                                         EventTrackingId,
                                                         jsonRequest.RequestId,
                                                         jsonRequest.Payload,
                                                         CancellationToken ]);

                        #endregion

                             if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONErrorMessage?>> jsonProcessor)
                        {

                            (OCPPJSONResponse,   OCPPErrorResponse) = await jsonProcessor;

                            #region Send response...

                            if (OCPPJSONResponse is not null)
                                await SendText(
                                          OCPPJSONResponse.ToJSON().ToString(JSONFormatting)
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

                                OnJSONMessageResponseSent?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  RequestTimestamp,
                                                                  jsonArray,
                                                                  null,
                                                                  Timestamp.Now,
                                                                  OCPPJSONResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnJSONMessageResponseSent));
                            }

                            #endregion

                        }

                        else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>> binaryProcessor)
                        {

                            (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                            #region Send response...

                            if (OCPPBinaryResponse is not null)
                                await SendBinary(
                                          OCPPBinaryResponse.ToByteArray()
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
                                                                    EventTrackingId,
                                                                    RequestTimestamp,
                                                                    jsonArray,
                                                                    null,
                                                                    Timestamp.Now,
                                                                    OCPPBinaryResponse?.ToByteArray() ?? OCPPErrorResponse?.ToByteArray() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnBinaryMessageResponseSent));
                            }

                            #endregion

                        }

                        else
                            DebugX.Log($"Received undefined '{jsonRequest.Action}' JSON request message handler within {nameof(NetworkingNodeWSClient)}!");

                    }
                    else
                        DebugX.Log($"Received unknown '{jsonRequest.Action}' JSON request message handler within {nameof(NetworkingNodeWSClient)}!");

                }

                else if (OCPP_JSONResponseMessage.TryParse(jsonArray, out var jsonResponse, out var responseParsingError) && jsonResponse      is not null)
                {

                    if (requests.TryGetValue(jsonResponse.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.JSONResponse       = jsonResponse;

                        #region OnTextMessageResponseReceived

                        try
                        {

                            OnJSONMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  sendRequestState.RequestTimestamp,
                                                                  sendRequestState.JSONRequest?.  ToJSON()      ?? [],
                                                                  sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                                                                  sendRequestState.ResponseTimestamp.Value,
                                                                  sendRequestState.JSONResponse?. ToJSON()      ?? []);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnJSONMessageResponseReceived));
                        }

                        #endregion

                    }
                    else
                        DebugX.Log($"Received an OCPP JSON response message having an unknown request identification within {nameof(NetworkingNodeWSClient)}: '{jsonResponse}'!");

                }

                else if (OCPP_JSONErrorMessage.   TryParse(jsonArray, out var jsonErrorResponse)                          && jsonErrorResponse is not null)
                {
                    DebugX.Log(nameof(NetworkingNodeWSClient), " Received unknown OCPP error message: " + TextMessage);
                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a JSON request message within {nameof(NetworkingNodeWSClient)}: '{requestParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a JSON response message within {nameof(NetworkingNodeWSClient)}: '{responseParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else
                    DebugX.Log($"Received unknown text message within {nameof(NetworkingNodeWSClient)}: '{TextMessage}'!");

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(NetworkingNodeWSClient) + "." + nameof(ProcessWebSocketTextFrame));

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
                DebugX.Log($"Received an empty binary message within {nameof(NetworkingNodeWSClient)}!");
                return;
            }

            try
            {

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError)  && binaryRequest  is not null)
                {

                    OCPP_JSONResponseMessage?    OCPPJSONResponse     = null;
                    OCPP_BinaryResponseMessage?  OCPPBinaryResponse   = null;
                    OCPP_JSONErrorMessage?       OCPPErrorResponse    = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) && methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ RequestTimestamp,
                                                         Connection,
                                                         NetworkingNodeIdentity,
                                                         NetworkPath.Empty,
                                                         EventTrackingId,
                                                         binaryRequest.RequestId,
                                                         binaryRequest.Payload,
                                                         CancellationToken ]);

                        #endregion

                             if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONErrorMessage?>> jsonProcessor)
                        {

                            (OCPPJSONResponse,   OCPPErrorResponse) = await jsonProcessor;

                            #region Send response...

                            if (OCPPJSONResponse is not null)
                                await SendText(
                                          OCPPJSONResponse.ToJSON().ToString(JSONFormatting)
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

                                OnJSONMessageResponseSent?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  RequestTimestamp,
                                                                  null,
                                                                  BinaryMessage,
                                                                  Timestamp.Now,
                                                                  OCPPJSONResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnJSONMessageResponseSent));
                            }

                            #endregion

                        }

                        else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>> binaryProcessor)
                        {

                            (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                            #region Send response...

                            if (OCPPBinaryResponse is not null)
                                await SendBinary(
                                          OCPPBinaryResponse.ToByteArray()
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
                                                                    EventTrackingId,
                                                                    RequestTimestamp,
                                                                    null,
                                                                    BinaryMessage,
                                                                    Timestamp.Now,
                                                                    OCPPBinaryResponse?.ToByteArray() ?? OCPPErrorResponse?.ToByteArray() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnBinaryMessageResponseSent));
                            }

                            #endregion

                        }
                        else
                            DebugX.Log($"Undefined '{binaryRequest.Action}' binary request message handler within {nameof(NetworkingNodeWSClient)}!");

                    }
                    else
                        DebugX.Log($"Unknown '{binaryRequest.Action}' binary request message handler within {nameof(NetworkingNodeWSClient)}!");

                }

                else if (OCPP_BinaryResponseMessage.TryParse(BinaryMessage, out var binaryResponse, out var responseParsingError) && binaryResponse is not null)
                {

                    if (requests.TryGetValue(binaryResponse.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.BinaryResponse     = binaryResponse;

                        #region OnBinaryMessageResponseReceived

                        try
                        {

                            OnBinaryMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    EventTrackingId,
                                                                    sendRequestState.RequestTimestamp,
                                                                    sendRequestState.JSONRequest?.  ToJSON()      ?? [],
                                                                    sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                                                                    sendRequestState.ResponseTimestamp.Value,
                                                                    sendRequestState.BinaryResponse.ToByteArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnBinaryMessageResponseReceived));
                        }

                        #endregion

                    }
                    else
                        DebugX.Log($"Received a binary OCPP response message having an unknown request identification within {nameof(NetworkingNodeWSClient)}: '{binaryResponse}'!");

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a binary request message within {nameof(NetworkingNodeWSClient)}: '{requestParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a binary response message within {nameof(NetworkingNodeWSClient)}: '{responseParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else
                    DebugX.Log($"Received unknown binary message within {nameof(NetworkingNodeWSClient)}: '{BinaryMessage.ToBase64()}'!");

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(NetworkingNodeWSClient) + "." + nameof(ProcessWebSocketBinaryFrame));

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


        #region SendRequest(Action, RequestId, JSONMessage)

        public async Task<OCPP_JSONRequestMessage> SendRequest(NetworkingNode_Id?  NetworkingNodeId,
                                                               NetworkPath?        NetworkPath,
                                                               String              Action,
                                                               Request_Id          RequestId,
                                                               JObject             JSONMessage)
        {

            OCPP_JSONRequestMessage? jsonRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 NetworkingNodeId,
                                                 NetworkPath,
                                                 RequestId,
                                                 Action,
                                                 JSONMessage
                                             );

                        await SendText(jsonRequestMessage.
                                           ToJSON().
                                           ToString(JSONFormatting));

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromJSONRequest(
                                            Timestamp.Now,
                                            NetworkingNodeIdentity,
                                            Timestamp.Now + RequestTimeout,
                                            jsonRequestMessage
                                        ));

                    }
                    else
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 NetworkingNodeId,
                                                 NetworkPath,
                                                 RequestId,
                                                 Action,
                                                 JSONMessage,
                                                 ErrorMessage: "Invalid HTTP Web Socket connection!"
                                             );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    jsonRequestMessage = new OCPP_JSONRequestMessage(
                                             NetworkingNodeId,
                                             NetworkPath,
                                             RequestId,
                                             Action,
                                             JSONMessage,
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
                jsonRequestMessage = new OCPP_JSONRequestMessage(
                                         NetworkingNodeId,
                                         NetworkPath,
                                         RequestId,
                                         Action,
                                         JSONMessage,
                                         ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                     );

            return jsonRequestMessage;

        }

        #endregion

        #region SendRequest(Action, RequestId, BinaryMessage)

        public async Task<OCPP_BinaryRequestMessage> SendRequest(NetworkingNode_Id?  NetworkingNodeId,
                                                                 NetworkPath?        NetworkPath,
                                                                 String              Action,
                                                                 Request_Id          RequestId,
                                                                 Byte[]              BinaryMessage)
        {

            OCPP_BinaryRequestMessage? binaryRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   NetworkingNodeId,
                                                   NetworkPath,
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage
                                               );

                        await SendBinary(binaryRequestMessage.ToByteArray());

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromBinaryRequest(
                                            Timestamp.Now,
                                            NetworkingNodeIdentity,
                                            Timestamp.Now + RequestTimeout,
                                            binaryRequestMessage
                                        ));

                    }
                    else
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   NetworkingNodeId,
                                                   NetworkPath,
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage,
                                                   ErrorMessage: "Invalid HTTP Web Socket connection!"
                                               );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                               NetworkingNodeId,
                                               NetworkPath,
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
                binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                           NetworkingNodeId,
                                           NetworkPath,
                                           RequestId,
                                           Action,
                                           BinaryMessage,
                                           ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                       );

            return binaryRequestMessage;

        }

        #endregion


        #region (private) WaitForResponse(JSONRequestMessage)

        private async Task<SendRequestState> WaitForResponse(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(JSONRequestMessage.RequestId, out var aSendRequestState) &&
                        aSendRequestState is SendRequestState sendRequestState &&
                       (sendRequestState?.JSONResponse is not null ||
                        sendRequestState?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(JSONRequestMessage.RequestId, out _);

                        return sendRequestState;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(NetworkingNodeWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromJSONRequest(

                       Timestamp.Now,
                       NetworkingNodeIdentity,
                       endTime,
                       JSONRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion

        #region (private) WaitForResponse(BinaryRequestMessage)

        private async Task<SendRequestState> WaitForResponse(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var aSendRequestState) &&
                        aSendRequestState is SendRequestState sendRequestState &&
                       (sendRequestState?.BinaryResponse is not null ||
                        sendRequestState?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(BinaryRequestMessage.RequestId, out _);

                        return sendRequestState;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(NetworkingNodeWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromBinaryRequest(

                       Timestamp.Now,
                       NetworkingNodeIdentity,
                       endTime,
                       BinaryRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion


    }

}
