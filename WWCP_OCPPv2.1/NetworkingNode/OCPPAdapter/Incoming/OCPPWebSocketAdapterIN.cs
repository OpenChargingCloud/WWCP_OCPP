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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for receiving incoming messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Data

        private   readonly  INetworkingNode                 parentNetworkingNode;

        protected readonly  Dictionary<String, MethodInfo>  incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        public HashSet<NetworkingNode_Id>  AnycastIds    { get; } = [];

        #endregion

        #region Events

        #region JSON   messages received

        /// <summary>
        /// An event sent whenever a JSON request was received.
        /// </summary>
        public event OnJSONRequestMessageReceivedDelegate?           OnJSONRequestMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON response was received.
        /// </summary>
        public event OnJSONResponseMessageReceivedDelegate?          OnJSONResponseMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON request error was received.
        /// </summary>
        public event OnJSONRequestErrorMessageReceivedDelegate?      OnJSONRequestErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON response error was received.
        /// </summary>
        public event OnJSONResponseErrorMessageReceivedDelegate?     OnJSONResponseErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON send message was received.
        /// </summary>
        public event OnJSONSendMessageReceivedDelegate?              OnJSONSendMessageReceived;

        #endregion

        #region Binary messages received

        /// <summary>
        /// An event sent whenever a binary request was received.
        /// </summary>
        public event OnBinaryRequestMessageReceivedDelegate?         OnBinaryRequestMessageReceived;

        /// <summary>
        /// An event sent whenever a binary response was received.
        /// </summary>
        public event OnBinaryResponseMessageReceivedDelegate?        OnBinaryResponseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a binary request error was received.
        ///// </summary>
        //public event OnBinaryRequestErrorMessageReceivedDelegate?    OnBinaryRequestErrorMessageReceived;

        ///// <summary>
        ///// An event sent whenever a binary response error was received.
        ///// </summary>
        //public event OnBinaryResponseErrorMessageReceivedDelegate?   OnBinaryResponseErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a binary send message was received.
        /// </summary>
        public event OnBinarySendMessageReceivedDelegate?            OnBinarySendMessageReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for accepting incoming messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterIN(INetworkingNode NetworkingNode)
        {

            this.parentNetworkingNode = NetworkingNode;

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var method in typeof(OCPPWebSocketAdapterIN).
                                       GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                            Where(method            => method.Name.StartsWith("Receive_") &&
                                                 (method.ReturnType == typeof(Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>>) ||
                                                  method.ReturnType == typeof(Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>>) ||
                                                  method.ReturnType == typeof(Task<OCPP_Response>))))
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


        #region ProcessJSONMessage   (MessageTimestamp, WebSocketConnection, JSONMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="MessageTimestamp">The receive timestamp of the JSON message.</param>
        /// <param name="WebSocketConnection">The WebSocket connection.</param>
        /// <param name="JSONMessage">The received JSON message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public async Task<WebSocketTextMessageResponse> ProcessJSONMessage(DateTime              MessageTimestamp,
                                                                           IWebSocketConnection  WebSocketConnection,
                                                                           JArray                JSONMessage,
                                                                           EventTracking_Id      EventTrackingId,
                                                                           CancellationToken     CancellationToken)
        {

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_JSONRequestMessage.      TryParse(JSONMessage, out var jsonRequestMessage,       out var requestParsingError,  MessageTimestamp, null, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    OCPP_JSONResponseMessage?     JSONResponseMessage       = null;
                    OCPP_BinaryResponseMessage?   BinaryResponseMessage     = null;
                    OCPP_JSONRequestErrorMessage? JSONRequestErrorMessage   = null;

                    #region Fix DestinationId and network path for standard networking connections

                    if (jsonRequestMessage.NetworkingMode == NetworkingMode.Standard &&
                        jsonRequestMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonRequestMessage = jsonRequestMessage.ChangeNetworking(
                                                              parentNetworkingNode.Id,
                                                              jsonRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                            case WebSocketServerConnection:
                                jsonRequestMessage = jsonRequestMessage.ChangeNetworking(
                                                              NetworkingNode_Id.CSMS,
                                                              jsonRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                        }
                    }

                    #endregion


                    #region OnJSONMessageRequestReceived

                    var logger = OnJSONRequestMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONRequestMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonRequestMessage
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONRequestMessageReceived));
                        }
                    }

                    #endregion

                    var sendresult      = SendMessageResult.TransmissionFailed;
                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonRequestMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonRequestMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONRequestMessage(jsonRequestMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonRequestMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(jsonRequestMessage.Action, out var methodInfo) &&
                            methodInfo is not null)
                        {

                            //ToDo: Maybe this could be done via code generation!
                            var result = methodInfo.Invoke(this,
                                                           [ jsonRequestMessage.RequestTimestamp,
                                                             WebSocketConnection,
                                                             jsonRequestMessage.DestinationId,
                                                             jsonRequestMessage.NetworkPath,
                                                             jsonRequestMessage.EventTrackingId,
                                                             jsonRequestMessage.RequestId,
                                                             jsonRequestMessage.Payload,
                                                             jsonRequestMessage.CancellationToken ]);

                            if      (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                                (JSONResponseMessage, JSONRequestErrorMessage) = await textProcessor;

                                if (JSONResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONResponse(JSONResponseMessage);

                                if (JSONRequestErrorMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

                            }

                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (BinaryResponseMessage, JSONRequestErrorMessage) = await binaryProcessor;

                                if (BinaryResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

                                if (JSONRequestErrorMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply        = await ocppProcessor;

                                JSONResponseMessage     = ocppReply.JSONResponseMessage;
                                JSONRequestErrorMessage = ocppReply.JSONRequestErrorMessage;
                                BinaryResponseMessage   = ocppReply.BinaryResponseMessage;

                                if (ocppReply.JSONResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONResponse    (ocppReply.JSONResponseMessage);

                                if (ocppReply.JSONRequestErrorMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(ocppReply.JSONRequestErrorMessage);

                                if (ocppReply.BinaryResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse  (ocppReply.BinaryResponseMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{jsonRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Received unknown '{jsonRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            JSONRequestErrorMessage = new OCPP_JSONRequestErrorMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    NetworkingMode.Unknown,
                                                    NetworkingNode_Id.Zero,
                                                    NetworkPath.Empty,
                                                    jsonRequestMessage.RequestId,
                                                    ResultCode.ProtocolError,
                                                    $"The OCPP message '{jsonRequestMessage.Action}' is unkown!",
                                                    new JObject(
                                                        new JProperty("request", JSONMessage)
                                                    )
                                                );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    if (JSONResponseMessage     is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONMessageResponseSent  (JSONResponseMessage,     sendresult);

                    if (JSONRequestErrorMessage is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent    (JSONRequestErrorMessage, sendresult);

                    if (BinaryResponseMessage   is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyBinaryMessageResponseSent(BinaryResponseMessage,   sendresult);

                    #endregion

                }

                else if (OCPP_JSONResponseMessage.     TryParse(JSONMessage, out var jsonResponseMessage,      out var responseParsingError,                                          sourceNodeId))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (jsonResponseMessage.NetworkingMode == NetworkingMode.Standard &&
                        jsonResponseMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonResponseMessage = jsonResponseMessage.ChangeNetworking(
                                                                parentNetworkingNode.Id, //sourceNodeId.Value,
                                                                jsonResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                            );
                                break;

                            case WebSocketServerConnection:
                                jsonResponseMessage = jsonResponseMessage.ChangeNetworking(
                                                                parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(jsonResponseMessage.RequestId) ?? parentNetworkingNode.Id,
                                                                jsonResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                            );
                                break;

                        }
                    }

                    #endregion


                    #region OnJSONMessageResponseReceived

                    var logger = OnJSONResponseMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONResponseMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonResponseMessage
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONResponseMessageReceived));
                        }
                    }

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonResponseMessage.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONResponseMessage(jsonResponseMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonResponseMessage.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonResponseMessage.DestinationId))
                    {
                        parentNetworkingNode.OCPP.ReceiveJSONResponse(jsonResponseMessage);
                    }

                    // No response!

                }

                else if (OCPP_JSONRequestErrorMessage. TryParse(JSONMessage, out var jsonRequestErrorMessage,                                                                         sourceNodeId))
                {

                    #region OnJSONRequestErrorMessageReceived

                    var logger = OnJSONRequestErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONRequestErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonRequestErrorMessage
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONRequestErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveJSONRequestError(jsonRequestErrorMessage);

                    // No response!

                }

                else if (OCPP_JSONResponseErrorMessage.TryParse(JSONMessage, out var jsonResponseErrorMessage,                                                                        sourceNodeId))
                {

                    #region OnJSONResponseErrorMessageReceived

                    var logger = OnJSONResponseErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONResponseErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonResponseErrorMessage
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONResponseErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveJSONResponseError(jsonResponseErrorMessage);

                    // No response!

                }

                else if (OCPP_JSONSendMessage.         TryParse(JSONMessage, out var jsonSendMessage,          out var sendParsingError,     MessageTimestamp,       EventTrackingId, sourceNodeId, CancellationToken))
                {

                    OCPP_JSONRequestErrorMessage? JSONRequestErrorMessage   = null;

                    #region Fix DestinationId and network path for standard networking connections

                    if (jsonSendMessage.NetworkingMode == NetworkingMode.Standard &&
                        jsonSendMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonSendMessage = jsonSendMessage.ChangeNetworking(
                                                              parentNetworkingNode.Id,
                                                              jsonSendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                            case WebSocketServerConnection:
                                jsonSendMessage = jsonSendMessage.ChangeNetworking(
                                                              NetworkingNode_Id.CSMS,
                                                              jsonSendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                        }
                    }

                    #endregion


                    #region OnJSONSendMessageReceived

                    var logger = OnJSONSendMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONSendMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonSendMessage
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONSendMessageReceived));
                        }
                    }

                    #endregion

                    var sendresult      = SendMessageResult.TransmissionFailed;
                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonSendMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonSendMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONSendMessage(jsonSendMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonSendMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(jsonSendMessage.Action, out var methodInfo) &&
                            methodInfo is not null)
                        {

                            //ToDo: Maybe this could be done via code generation!
                            var result = methodInfo.Invoke(this,
                                                           [ jsonSendMessage.MessageTimestamp,
                                                             WebSocketConnection,
                                                             jsonSendMessage.DestinationId,
                                                             jsonSendMessage.NetworkPath,
                                                             jsonSendMessage.EventTrackingId,
                                                             jsonSendMessage.MessageId,
                                                             jsonSendMessage.Payload,
                                                             jsonSendMessage.CancellationToken ]);

                            //if      (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                            //    (JSONResponseMessage, JSONRequestErrorMessage) = await textProcessor;

                            //    if (JSONResponseMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendJSONResponse(JSONResponseMessage);

                            //    if (JSONRequestErrorMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

                            //}

                            //else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                            //    (BinaryResponseMessage, JSONRequestErrorMessage) = await binaryProcessor;

                            //    if (BinaryResponseMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

                            //    if (JSONRequestErrorMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

                            //}

                            if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply        = await ocppProcessor;

                                JSONRequestErrorMessage = ocppReply.JSONRequestErrorMessage;

                                if (ocppReply.JSONRequestErrorMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(ocppReply.JSONRequestErrorMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{jsonSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Received unknown '{jsonSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            JSONRequestErrorMessage = new OCPP_JSONRequestErrorMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    NetworkingMode.Unknown,
                                                    NetworkingNode_Id.Zero,
                                                    NetworkPath.Empty,
                                                    jsonSendMessage.MessageId,
                                                    ResultCode.ProtocolError,
                                                    $"The OCPP message '{jsonSendMessage.Action}' is unkown!",
                                                    new JObject(
                                                        new JProperty("request", JSONMessage)
                                                    )
                                                );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    if (JSONRequestErrorMessage is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent(JSONRequestErrorMessage, sendresult);

                    #endregion

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a JSON request message within {nameof(OCPPWebSocketAdapterIN)}: '{requestParsingError}'{Environment.NewLine}'{JSONMessage.ToString(Formatting.None)}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a JSON response message within {nameof(OCPPWebSocketAdapterIN)}: '{responseParsingError}'{Environment.NewLine}'{JSONMessage.ToString(Formatting.None)}'!");

                else
                    DebugX.Log($"Received unknown text message within {nameof(OCPPWebSocketAdapterIN)}: '{JSONMessage.ToString(Formatting.None)}'!");


            }
            catch (Exception e)
            {

                //JSONRequestErrorMessage = OCPP_JSONRequestErrorMessage.InternalError(
                //                              nameof(OCPPWebSocketAdapterIN),
                //                              EventTrackingId,
                //                              JSONMessage,
                //                              e
                //                          );

            }

            // The response is empty!
            return new WebSocketTextMessageResponse(
                       MessageTimestamp,
                       JSONMessage.ToString(),
                       Timestamp.Now,
                       String.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region ProcessBinaryMessage (MessageTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime              MessageTimestamp,
                                                                               IWebSocketConnection  WebSocketConnection,
                                                                               Byte[]                BinaryMessage,
                                                                               EventTracking_Id      EventTrackingId,
                                                                               CancellationToken     CancellationToken)
        {

            OCPP_JSONResponseMessage?     OCPPResponse         = null;
            OCPP_BinaryResponseMessage?   OCPPBinaryResponse   = null;
            OCPP_JSONRequestErrorMessage? OCPPErrorResponse    = null;

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError,  MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequest  is not null)
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryRequest.NetworkingMode    == NetworkingMode.Standard &&
                        binaryRequest.DestinationId == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryRequest = binaryRequest.ChangeDestinationId(
                                                                  parentNetworkingNode.Id,
                                                                  binaryRequest.NetworkPath.Append(sourceNodeId.Value)
                                                              );
                                break;

                            case WebSocketServerConnection:
                                binaryRequest = binaryRequest.ChangeDestinationId(
                                                                  NetworkingNode_Id.CSMS,
                                                                  binaryRequest.NetworkPath.Append(sourceNodeId.Value)
                                                              );
                                break;

                        }
                    }

                    #endregion



                    #region OnBinaryMessageRequestReceived

                    var logger = OnBinaryRequestMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryRequestMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  binaryRequest
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryRequestMessageReceived));
                        }
                    }

                    #endregion


                    var sendresult = SendMessageResult.TransmissionFailed;

                    // When not for this node, send it to the FORWARD processor...
                    if (binaryRequest.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryRequestMessage(binaryRequest, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binaryRequest.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryRequest.DestinationId))
                    {

                        #region Try to call the matching 'incoming message processor'

                        if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) &&
                            methodInfo is not null)
                        {

                            var result = methodInfo.Invoke(this,
                                                           [ binaryRequest.RequestTimestamp,
                                                             WebSocketConnection,
                                                             binaryRequest.DestinationId,
                                                             binaryRequest.NetworkPath,
                                                             binaryRequest.EventTrackingId,
                                                             binaryRequest.RequestId,
                                                             binaryRequest.Payload,
                                                             binaryRequest.CancellationToken ]);

                                 if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                                (OCPPResponse, OCPPErrorResponse) = await textProcessor;

                                if (OCPPResponse is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONResponse    (OCPPResponse);

                                if (OCPPErrorResponse is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(OCPPErrorResponse);

                            }

                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                                if (OCPPBinaryResponse is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse  (OCPPBinaryResponse);

                                if (OCPPErrorResponse is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(OCPPErrorResponse);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply = await ocppProcessor;

                                OCPPResponse         = ocppReply.JSONResponseMessage;
                                OCPPErrorResponse    = ocppReply.JSONRequestErrorMessage;
                                OCPPBinaryResponse   = ocppReply.BinaryResponseMessage;

                                if (ocppReply.JSONResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONResponse    (ocppReply.JSONResponseMessage);

                                if (ocppReply.JSONRequestErrorMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendJSONRequestError(ocppReply.JSONRequestErrorMessage);

                                if (ocppReply.BinaryResponseMessage is not null)
                                    sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse  (ocppReply.BinaryResponseMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{binaryRequest.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Received unknown '{binaryRequest.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            OCPPErrorResponse = new OCPP_JSONRequestErrorMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    NetworkingMode.Unknown,
                                                    NetworkingNode_Id.Zero,
                                                    NetworkPath.Empty,
                                                    binaryRequest.RequestId,
                                                    ResultCode.ProtocolError,
                                                    $"The OCPP message '{binaryRequest.Action}' is unkown!",
                                                    new JObject(
                                                        new JProperty("request", BinaryMessage.ToBase64())
                                                    )
                                                );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    if (OCPPResponse       is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONMessageResponseSent  (OCPPResponse,       sendresult);

                    if (OCPPErrorResponse  is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent    (OCPPErrorResponse,  sendresult);

                    if (OCPPBinaryResponse is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyBinaryMessageResponseSent(OCPPBinaryResponse, sendresult);

                    #endregion

                }

                else if (OCPP_BinaryResponseMessage.TryParse(BinaryMessage, out var binaryResponse, out var responseParsingError,                                    sourceNodeId)                    && binaryResponse is not null)
                {

                    #region OnBinaryMessageResponseReceived

                    var logger = OnBinaryResponseMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryResponseMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  binaryResponse
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryResponseMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryResponse(binaryResponse);

                    // No response!

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a binary request message within {nameof(OCPPWebSocketAdapterIN)}: '{requestParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a binary response message within {nameof(OCPPWebSocketAdapterIN)}: '{responseParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else
                    DebugX.Log($"Received unknown binary message within {nameof(OCPPWebSocketAdapterIN)}: '{BinaryMessage.ToBase64()}'!");

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.InternalError(
                                        nameof(OCPPWebSocketAdapterIN),
                                        EventTrackingId,
                                        BinaryMessage,
                                        e
                                    );

            }


            // The response is empty!
            return new WebSocketBinaryMessageResponse(
                       MessageTimestamp,
                       BinaryMessage,
                       Timestamp.Now,
                       [],
                       EventTrackingId
                   );

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


    }

}
