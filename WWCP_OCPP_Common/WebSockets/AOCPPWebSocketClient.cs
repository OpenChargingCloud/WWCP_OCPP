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

#endregion

namespace cloud.charging.open.protocols.OCPP.CS
{

    /// <summary>
    /// An OCPP HTTP Web Socket client runs on a charging station or networking node
    /// and connects to a CSMS or another networking node to invoke OCPP commands.
    /// </summary>
    public abstract class AOCPPWebSocketClient : WebSocketClient,
                                                 IEventSender
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const        String                                              DefaultHTTPUserAgent              = $"GraphDefined OCPP {Version.String} Charging Station HTTP Web Socket Client";

        public static readonly  TimeSpan                                            DefaultRequestTimeout             = TimeSpan.FromSeconds(30);

        protected readonly      ConcurrentDictionary<Request_Id, SendRequestState>  requests                          = [];

        protected readonly      Dictionary<String, MethodInfo>                      incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charging station.
        /// </summary>
        public NetworkingNode_Id                     Id                 { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => Id.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From               { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                 { get; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                            JSONFormatting     { get; set; } = Formatting.None;


        public NetworkingMode                        NetworkingMode     { get; set; } = NetworkingMode.Standard;

        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        //public ChargePointWSClient.CPClientLogger    Logger             { get; }

        #endregion

        #region Events

        public event OnWebSocketClientJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;
        public event OnWebSocketClientJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

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
        public AOCPPWebSocketClient(NetworkingNode_Id                    ChargingStationIdentity,
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
                                        NetworkingMode?                      NetworkingMode               = null,

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
                throw new ArgumentNullException(nameof(From),                     "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                       "The given websocket message destination must not be null or empty!");

            #endregion

            this.Id                       = ChargingStationIdentity;
            this.From                     = From;
            this.To                       = To;

            this.NetworkingMode           = NetworkingMode ?? WebSockets.NetworkingMode.Standard;

            //this.Logger                   = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                         LoggingPath,
            //                                                                         LoggingContext,
            //                                                                         LogfileCreator);

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
                DebugX.Log($"Received an empty JSON message within {nameof(AOCPPWebSocketClient)}!");
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
                                                         Id,
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
                                          OCPPJSONResponse.ToJSON().ToString(JSONFormatting),
                                          CancellationToken
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
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseSent));
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
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnBinaryMessageResponseSent));
                            }

                            #endregion

                        }

                        else
                            DebugX.Log($"Received undefined '{jsonRequest.Action}' JSON request message handler within {nameof(AOCPPWebSocketClient)}!");

                    }
                    else
                        DebugX.Log($"Received unknown '{jsonRequest.Action}' JSON request message handler within {nameof(AOCPPWebSocketClient)}!");

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
                            DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseReceived));
                        }

                        #endregion

                    }
                    else
                        DebugX.Log($"Received an OCPP JSON response message having an unknown request identification within {nameof(AOCPPWebSocketClient)}: '{jsonResponse}'!");

                }

                else if (OCPP_JSONErrorMessage.   TryParse(jsonArray, out var jsonErrorResponse)                          && jsonErrorResponse is not null)
                {
                    DebugX.Log(nameof(AOCPPWebSocketClient), " Received unknown OCPP error message: " + TextMessage);
                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a JSON request message within {nameof(AOCPPWebSocketClient)}: '{requestParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a JSON response message within {nameof(AOCPPWebSocketClient)}: '{responseParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else
                    DebugX.Log($"Received unknown text message within {nameof(AOCPPWebSocketClient)}: '{TextMessage}'!");

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(AOCPPWebSocketClient) + "." + nameof(ProcessWebSocketTextFrame));

                //OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                //                        Request_Id.Zero,
                //                        ResultCodes.InternalError,
                //                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(AChargingStationWSClient) + " led to an exception!",
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
                DebugX.Log($"Received an empty binary message within {nameof(AOCPPWebSocketClient)}!");
                return;
            }

            try
            {

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError)  && binaryRequest  is not null)
                {

                    OCPP_JSONResponseMessage?     OCPPJSONResponse     = null;
                    OCPP_BinaryResponseMessage?   OCPPBinaryResponse   = null;
                    OCPP_JSONErrorMessage?  OCPPErrorResponse    = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) && methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ RequestTimestamp,
                                                         Connection,
                                                         Id,
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
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseSent));
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
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnBinaryMessageResponseSent));
                            }

                            #endregion

                        }
                        else
                            DebugX.Log($"Undefined '{binaryRequest.Action}' binary request message handler within {nameof(AOCPPWebSocketClient)}!");

                    }
                    else
                        DebugX.Log($"Unknown '{binaryRequest.Action}' binary request message handler within {nameof(AOCPPWebSocketClient)}!");

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
                            DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnBinaryMessageResponseReceived));
                        }

                        #endregion

                    }
                    else
                        DebugX.Log($"Received a binary OCPP response message having an unknown request identification within {nameof(AOCPPWebSocketClient)}: '{binaryResponse}'!");

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a binary request message within {nameof(AOCPPWebSocketClient)}: '{requestParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a binary response message within {nameof(AOCPPWebSocketClient)}: '{responseParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else
                    DebugX.Log($"Received unknown binary message within {nameof(AOCPPWebSocketClient)}: '{BinaryMessage.ToBase64()}'!");

            }
            catch (Exception e)
            {

                DebugX.LogException(e, nameof(AOCPPWebSocketClient) + "." + nameof(ProcessWebSocketBinaryFrame));

                //OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                //                        Request_Id.Zero,
                //                        ResultCodes.InternalError,
                //                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(AChargingStationWSClient) + " led to an exception!",
                //                        new JObject(
                //                            new JProperty("request",      OCPPTextMessage),
                //                            new JProperty("exception",    e.Message),
                //                            new JProperty("stacktrace",   e.StackTrace)
                //                        )
                //                    );

            }

        }

        #endregion


        #region SendRequest(DestinationNodeId, Action, RequestId, JSONMessage,   NetworkPath = null)

        public Task<OCPP_JSONRequestMessage> SendRequest(NetworkingNode_Id  DestinationNodeId,
                                                         String             Action,
                                                         Request_Id         RequestId,
                                                         JObject            JSONMessage)

            => SendRequest(DestinationNodeId,
                           NetworkPath.Empty,
                           Action,
                           RequestId,
                           JSONMessage);

        public async Task<OCPP_JSONRequestMessage> SendRequest(NetworkingNode_Id  DestinationNodeId,
                                                               NetworkPath        NetworkPath,
                                                               String             Action,
                                                               Request_Id         RequestId,
                                                               JObject            JSONMessage)
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
                                                 DestinationNodeId,
                                                (NetworkPath ?? NetworkPath.Empty).Append(Id),
                                                 RequestId,
                                                 Action,
                                                 JSONMessage
                                             );

                        await SendText(jsonRequestMessage.
                                           ToJSON  (NetworkingMode).
                                           ToString(JSONFormatting));

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromJSONRequest(
                                            Timestamp.Now,
                                            Id,
                                            Timestamp.Now + RequestTimeout,
                                            jsonRequestMessage
                                        ));

                    }
                    else
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 DestinationNodeId,
                                                 NetworkPath ?? NetworkPath.Empty,
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
                                             DestinationNodeId,
                                             NetworkPath ?? NetworkPath.Empty,
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
                                         DestinationNodeId,
                                         NetworkPath ?? NetworkPath.Empty,
                                         RequestId,
                                         Action,
                                         JSONMessage,
                                         ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                     );

            return jsonRequestMessage;

        }

        #endregion

        #region SendRequest(DestinationNodeId, Action, RequestId, BinaryMessage, NetworkPath = null)

        public Task<OCPP_BinaryRequestMessage> SendRequest(NetworkingNode_Id  DestinationNodeId,
                                                           String             Action,
                                                           Request_Id         RequestId,
                                                           Byte[]             BinaryMessage)

            => SendRequest(DestinationNodeId,
                           NetworkPath.Empty,
                           Action,
                           RequestId,
                           BinaryMessage);

        public async Task<OCPP_BinaryRequestMessage> SendRequest(NetworkingNode_Id  DestinationNodeId,
                                                                 NetworkPath        NetworkPath,
                                                                 String             Action,
                                                                 Request_Id         RequestId,
                                                                 Byte[]             BinaryMessage)
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
                                                   DestinationNodeId,
                                                  (NetworkPath ?? NetworkPath.Empty).Append(Id),
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage
                                               );

                        await SendBinary(binaryRequestMessage.ToByteArray());

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromBinaryRequest(
                                            Timestamp.Now,
                                            Id,
                                            Timestamp.Now + RequestTimeout,
                                            binaryRequestMessage
                                        ));

                    }
                    else
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   DestinationNodeId,
                                                   NetworkPath ?? NetworkPath.Empty,
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
                                               DestinationNodeId,
                                               NetworkPath ?? NetworkPath.Empty,
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
                                           DestinationNodeId,
                                           NetworkPath ?? NetworkPath.Empty,
                                           RequestId,
                                           Action,
                                           BinaryMessage,
                                           ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                       );

            return binaryRequestMessage;

        }

        #endregion


        #region (protected) WaitForResponse(JSONRequestMessage)

        protected async Task<SendRequestState> WaitForResponse(OCPP_JSONRequestMessage JSONRequestMessage)
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
                    DebugX.Log(String.Concat(nameof(AOCPPWebSocketClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromJSONRequest(

                       Timestamp.Now,
                       Id,
                       endTime,
                       JSONRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion

        #region (protected) WaitForResponse(BinaryRequestMessage)

        protected async Task<SendRequestState> WaitForResponse(OCPP_BinaryRequestMessage BinaryRequestMessage)
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
                    DebugX.Log(String.Concat(nameof(AOCPPWebSocketClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromBinaryRequest(

                       Timestamp.Now,
                       Id,
                       endTime,
                       BinaryRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion


    }

}
