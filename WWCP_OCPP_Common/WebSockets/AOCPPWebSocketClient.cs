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
        /// Create a new OCPP HTTP Web Socket client running, e.g on a charging station
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="NetworkingNodeIdentity">The unique identification of this charging station.</param>
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
        public AOCPPWebSocketClient(NetworkingNode_Id                    NetworkingNodeIdentity,

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

            this.Id              = NetworkingNodeIdentity;
            this.NetworkingMode  = NetworkingMode ?? WebSockets.NetworkingMode.Standard;

            //this.Logger          = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                LoggingPath,
            //                                                                LoggingContext,
            //                                                                LogfileCreator);

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
              //  var sourceNodeId = Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey) ?? NetworkingNode_Id.Zero;

                if      (OCPP_JSONRequestMessage.     TryParse(jsonArray, out var jsonRequestMessage,  out var requestParsingError, RequestTimestamp, null, EventTrackingId, null, CancellationToken))
                {

                    OCPP_JSONResponseMessage?      OCPPJSONResponse     = null;
                    OCPP_BinaryResponseMessage?    OCPPBinaryResponse   = null;
                    OCPP_JSONRequestErrorMessage?  OCPPRequestError     = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(jsonRequestMessage.Action, out var methodInfo) && methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ jsonRequestMessage.RequestTimestamp,
                                                         Connection,
                                                         jsonRequestMessage.DestinationId,
                                                         jsonRequestMessage.NetworkPath,
                                                         jsonRequestMessage.EventTrackingId,
                                                         jsonRequestMessage.RequestId,
                                                         jsonRequestMessage.Payload,
                                                         jsonRequestMessage.CancellationToken ]);

                        #endregion

                             if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> jsonProcessor)
                        {

                            (OCPPJSONResponse,   OCPPRequestError) = await jsonProcessor;

                            #region Send response...

                            if (OCPPJSONResponse is not null)
                            {

                                if (OCPPJSONResponse.NetworkingMode == NetworkingMode.Unknown)
                                    OCPPJSONResponse = OCPPJSONResponse.ChangeNetworkingMode(NetworkingMode);

                                var sendStatus = await SendTextMessage(
                                                           OCPPJSONResponse.ToJSON().ToString(JSONFormatting),
                                                           EventTrackingId,
                                                           CancellationToken
                                                       );

                            }

                            #endregion

                            #region ..., or send error response!

                            if (OCPPRequestError is not null)
                            {
                                // CALL RESULT ERROR: New in OCPP v2.1++
                            }

                            #endregion


                            #region OnJSONMessageResponseSent

                            try
                            {

                                OnJSONMessageResponseSent?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  RequestTimestamp,
                                                                  jsonArray,
                                                                  null,
                                                                  Timestamp.Now,
                                                                  OCPPJSONResponse?.ToJSON() ?? OCPPRequestError?.ToJSON() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseSent));
                            }

                            #endregion

                        }

                        else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor)
                        {

                            (OCPPBinaryResponse, OCPPRequestError) = await binaryProcessor;

                            #region Send response...

                            if (OCPPBinaryResponse is not null)
                            {

                                var sendStatus = await SendBinaryMessage(
                                                           OCPPBinaryResponse.ToByteArray(),
                                                           EventTrackingId,
                                                           CancellationToken
                                                       );

                            }

                            #endregion

                            #region ..., or send error response!

                            if (OCPPRequestError is not null)
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
                                                                    OCPPBinaryResponse?.ToByteArray() ?? OCPPRequestError?.ToByteArray() ?? []);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnBinaryMessageResponseSent));
                            }

                            #endregion

                        }

                        else
                            DebugX.Log($"Received undefined '{jsonRequestMessage.Action}' JSON request message handler within {nameof(AOCPPWebSocketClient)}!");

                    }
                    else
                        DebugX.Log($"Received unknown '{jsonRequestMessage.Action}' JSON request message handler within {nameof(AOCPPWebSocketClient)}!");

                }

                else if (OCPP_JSONResponseMessage.    TryParse(jsonArray, out var jsonResponseMessage, out var responseParsingError))
                {

                    if (requests.TryGetValue(jsonResponseMessage.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.JSONResponse       = jsonResponseMessage;

                        #region OnJSONMessageResponseReceived

                        try
                        {

                            OnJSONMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  sendRequestState.RequestTimestamp,
                                                                  sendRequestState.JSONRequest?.     ToJSON()      ?? [],
                                                                  sendRequestState.BinaryRequest?.   ToByteArray() ?? [],
                                                                  sendRequestState.ResponseTimestamp.Value,
                                                                  sendRequestState.JSONResponse?.    ToJSON()      ?? []);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseReceived));
                        }

                        #endregion

                    }
                    else
                        DebugX.Log($"Received an OCPP JSON response message having an unknown request identification within {nameof(AOCPPWebSocketClient)}: '{jsonResponseMessage}'!");

                }

                else if (OCPP_JSONRequestErrorMessage.TryParse(jsonArray, out var jsonRequestErrorMessage))
                {

                    if (requests.TryGetValue(jsonRequestErrorMessage.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        sendRequestState.ResponseTimestamp        = Timestamp.Now;
                        sendRequestState.JSONRequestErrorMessage  = jsonRequestErrorMessage;

                        #region OnJSONMessageResponseReceived

                        try
                        {

                            OnJSONMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  EventTrackingId,
                                                                  sendRequestState.RequestTimestamp,
                                                                  sendRequestState.JSONRequest?.            ToJSON()      ?? [],
                                                                  sendRequestState.BinaryRequest?.          ToByteArray() ?? [],
                                                                  sendRequestState.ResponseTimestamp.       Value,
                                                                  sendRequestState.JSONRequestErrorMessage?.ToJSON()      ?? []);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(AOCPPWebSocketClient) + "." + nameof(OnJSONMessageResponseReceived));
                        }

                        #endregion

                    }


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

              //  var sourceNodeId = Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey) ?? NetworkingNode_Id.Zero;

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError, RequestTimestamp, EventTrackingId, null, CancellationToken)  && binaryRequest  is not null)
                {

                    OCPP_JSONResponseMessage?    OCPPJSONResponse     = null;
                    OCPP_BinaryResponseMessage?  OCPPBinaryResponse   = null;
                    OCPP_JSONRequestErrorMessage?       OCPPErrorResponse    = null;

                    // Try to call the matching 'incoming message processor'
                    if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) && methodInfo is not null)
                    {

                        #region Call 'incoming message' processor

                        var result = methodInfo.Invoke(this,
                                                       [ binaryRequest.RequestTimestamp,
                                                         Connection,
                                                         binaryRequest.DestinationNodeId,
                                                         binaryRequest.NetworkPath,
                                                         binaryRequest.EventTrackingId,
                                                         binaryRequest.RequestId,
                                                         binaryRequest.Payload,
                                                         binaryRequest.CancellationToken ]);

                        #endregion

                             if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> jsonProcessor)
                        {

                            (OCPPJSONResponse,   OCPPErrorResponse) = await jsonProcessor;

                            #region Send response...

                            if (OCPPJSONResponse is not null)
                                await SendTextMessage(
                                          OCPPJSONResponse.ToJSON().ToString(JSONFormatting),
                                          EventTrackingId,
                                          CancellationToken
                                      );

                            #endregion

                            #region ..., or send error response!

                            if (OCPPErrorResponse is not null)
                            {
                                // CALL RESULT ERROR: New in OCPP v2.1++
                            }

                            #endregion


                            #region OnJSONMessageResponseSent

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

                        else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor)
                        {

                            (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                            #region Send response...

                            if (OCPPBinaryResponse is not null)
                                await SendBinaryMessage(
                                          OCPPBinaryResponse.ToByteArray(),
                                          EventTrackingId,
                                          CancellationToken
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
                                                 Timestamp.Now,
                                                 EventTracking_Id.New,
                                                 NetworkingMode,
                                                 DestinationNodeId,
                                                 NetworkPath.Append(Id),
                                                 RequestId,
                                                 Action,
                                                 JSONMessage
                                             );

                        var sendStatus = await SendTextMessage(jsonRequestMessage.
                                                                   ToJSON  ().
                                                                   ToString(JSONFormatting));

                        if (sendStatus == SendStatus.Success)
                            requests.TryAdd(RequestId,
                                            SendRequestState.FromJSONRequest(
                                                Timestamp.Now,
                                                Id,
                                                Timestamp.Now + RequestTimeout,
                                                jsonRequestMessage
                                            ));

                        else
                        {
                            //ToDo: Retry to send text message!
                        }

                    }
                    else
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 Timestamp.Now,
                                                 EventTracking_Id.New,
                                                 NetworkingMode,
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
                                             Timestamp.Now,
                                             EventTracking_Id.New,
                                             NetworkingMode,
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
                                         Timestamp.Now,
                                         EventTracking_Id.New,
                                         NetworkingMode,
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
                                                   Timestamp.Now,
                                                   EventTracking_Id.New,
                                                   NetworkingMode,
                                                   DestinationNodeId,
                                                   NetworkPath.Append(Id),
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage
                                               );

                        var sendStatus = await SendBinaryMessage(binaryRequestMessage.ToByteArray());

                        if (sendStatus == SendStatus.Success)
                            requests.TryAdd(RequestId,
                                            SendRequestState.FromBinaryRequest(
                                                Timestamp.Now,
                                                Id,
                                                Timestamp.Now + RequestTimeout,
                                                binaryRequestMessage
                                            ));

                        else
                        {
                            //ToDo: Retry to send binary message!
                        }

                    }
                    else
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   Timestamp.Now,
                                                   EventTracking_Id.New,
                                                   NetworkingMode,
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
                                               Timestamp.Now,
                                               EventTracking_Id.New,
                                               NetworkingMode,
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
                                           Timestamp.Now,
                                           EventTracking_Id.New,
                                           NetworkingMode,
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
                        sendRequestState?.HasErrors == true))
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

                       JSONRequestMessage.RequestTimestamp,
                       Id,
                       endTime,
                       JSONRequestMessage,
                       Timestamp.Now,

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     JSONRequestMessage.NetworkPath.Source,
                                                     NetworkPath.From(Id),
                                                     JSONRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.Timeout

                                                 )

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
                        sendRequestState?.HasErrors == true))
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

                       JSONRequestErrorMessage:  new OCPP_JSONRequestErrorMessage(

                                                     Timestamp.Now,
                                                     BinaryRequestMessage.EventTrackingId,
                                                     NetworkingMode.Unknown,
                                                     BinaryRequestMessage.NetworkPath.Source,
                                                     NetworkPath.From(Id),
                                                     BinaryRequestMessage.RequestId,

                                                     ErrorCode: ResultCode.Timeout

                                                 )

                   );

        }

        #endregion


    }

}
