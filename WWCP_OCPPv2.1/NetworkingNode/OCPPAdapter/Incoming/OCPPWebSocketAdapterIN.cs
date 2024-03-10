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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for receiving messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Data

        private   readonly  NN.IBaseNetworkingNode          parentNetworkingNode;

        protected readonly  Dictionary<String, MethodInfo>  incomingMessageProcessorsLookup   = [];

        #endregion

        #region Properties

        public HashSet<NetworkingNode_Id>  AnycastIds    { get; } = [];

        #endregion

        #region Events

        #region Generic Text Messages

        /// <summary>
        /// An event sent whenever a JSON request was received.
        /// </summary>
        public event OnJSONMessageRequestReceivedDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a JSON response was received.
        /// </summary>
        public event OnJSONMessageResponseReceivedDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever a JSON error response was received.
        /// </summary>
        public event OnJSONErrorResponseReceivedDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary request was received.
        /// </summary>
        public event OnBinaryMessageRequestReceivedDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a binary response was received.
        /// </summary>
        public event OnBinaryMessageResponseReceivedDelegate?    OnBinaryMessageResponseReceived;

        ///// <summary>
        ///// An event sent whenever a binary error response was received.
        ///// </summary>
        //public event OnBinaryErrorResponseReceivedDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for accepting incoming messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterIN(NN.IBaseNetworkingNode NetworkingNode)
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


        #region ProcessJSONMessage   (RequestTimestamp, WebSocketConnection, JSONMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="WebSocketConnection">The WebSocket connection.</param>
        /// <param name="JSONMessage">The received JSON message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public async Task<WebSocketTextMessageResponse> ProcessJSONMessage(DateTime              RequestTimestamp,
                                                                           IWebSocketConnection  WebSocketConnection,
                                                                           JArray                JSONMessage,
                                                                           EventTracking_Id      EventTrackingId,
                                                                           CancellationToken     CancellationToken)
        {

            OCPP_JSONResponseMessage?      OCPPResponse         = null;
            OCPP_BinaryResponseMessage?    OCPPBinaryResponse   = null;
            OCPP_JSONRequestErrorMessage?  OCPPErrorResponse    = null;

            try
            {

                var sourceNodeId  = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_JSONRequestMessage. TryParse(JSONMessage, out var jsonRequest,  out var requestParsingError,  RequestTimestamp, null, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    #region Fix DestinationNodeId and network path for standard networking connections

                    if (jsonRequest.NetworkingMode    == NetworkingMode.Standard &&
                        jsonRequest.DestinationId == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonRequest = jsonRequest.ChangeNetworking(
                                                              parentNetworkingNode.Id,
                                                              jsonRequest.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                            case WebSocketServerConnection:
                                jsonRequest = jsonRequest.ChangeNetworking(
                                                              NetworkingNode_Id.CSMS,
                                                              jsonRequest.NetworkPath.Append(sourceNodeId.Value)
                                                          );
                                break;

                        }
                    }

                    #endregion


                    #region OnJSONMessageRequestReceived

                    var logger = OnJSONMessageRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONMessageRequestReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonRequest
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONMessageRequestReceived));
                        }
                    }

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonRequest.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONRequestMessage(jsonRequest);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonRequest.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonRequest.DestinationId))
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(jsonRequest.Action, out var methodInfo) &&
                            methodInfo is not null)
                        {

                            //ToDo: Maybe this could be done via code generation!
                            var result = methodInfo.Invoke(this,
                                                           [ jsonRequest.RequestTimestamp,
                                                             WebSocketConnection,
                                                             jsonRequest.DestinationId,
                                                             jsonRequest.NetworkPath,
                                                             jsonRequest.EventTrackingId,
                                                             jsonRequest.RequestId,
                                                             jsonRequest.Payload,
                                                             jsonRequest.CancellationToken ]);

                                 if (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                                (OCPPResponse, OCPPErrorResponse) = await textProcessor;

                                if (OCPPResponse is not null)
                                    await parentNetworkingNode.OCPP.SendJSONResponse(OCPPResponse);

                                if (OCPPErrorResponse is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError   (OCPPErrorResponse);

                            }

                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                                if (OCPPBinaryResponse is not null)
                                    await parentNetworkingNode.OCPP.SendBinaryResponse(OCPPBinaryResponse);

                                if (OCPPErrorResponse is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError     (OCPPErrorResponse);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply = await ocppProcessor;

                                OCPPResponse         = ocppReply.JSONResponseMessage;
                                OCPPErrorResponse    = ocppReply.JSONErrorMessage;
                                OCPPBinaryResponse   = ocppReply.BinaryResponseMessage;

                                if (ocppReply.JSONResponseMessage is not null)
                                    await parentNetworkingNode.OCPP.SendJSONResponse  (ocppReply.JSONResponseMessage);

                                if (ocppReply.JSONErrorMessage is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError     (ocppReply.JSONErrorMessage);

                                if (ocppReply.BinaryResponseMessage is not null)
                                    await parentNetworkingNode.OCPP.SendBinaryResponse(ocppReply.BinaryResponseMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{jsonRequest.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Received unknown '{jsonRequest.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            OCPPErrorResponse = new OCPP_JSONRequestErrorMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    NetworkingMode.Unknown,
                                                    NetworkingNode_Id.Zero,
                                                    NetworkPath.Empty,
                                                    jsonRequest.RequestId,
                                                    ResultCode.ProtocolError,
                                                    $"The OCPP message '{jsonRequest.Action}' is unkown!",
                                                    new JObject(
                                                        new JProperty("request", JSONMessage)
                                                    )
                                                );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    if (OCPPResponse       is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONMessageResponseSent  (OCPPResponse);

                    if (OCPPErrorResponse  is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONErrorResponseSent    (OCPPErrorResponse);

                    if (OCPPBinaryResponse is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyBinaryMessageResponseSent(OCPPBinaryResponse);

                    #endregion

                }

                else if (OCPP_JSONResponseMessage.TryParse(JSONMessage, out var jsonResponse, out var responseParsingError,                                    sourceNodeId))
                {

                    #region Fix DestinationNodeId and network path for standard networking connections

                    if (jsonResponse.NetworkingMode == NetworkingMode.Standard &&
                        jsonResponse.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonResponse = jsonResponse.ChangeNetworking(
                                                                parentNetworkingNode.Id, //sourceNodeId.Value,
                                                                jsonResponse.NetworkPath.Append(sourceNodeId.Value)
                                                            );
                                break;

                            case WebSocketServerConnection:
                                jsonResponse = jsonResponse.ChangeNetworking(
                                                                parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(jsonResponse.RequestId) ?? parentNetworkingNode.Id,
                                                                jsonResponse.NetworkPath.Append(sourceNodeId.Value)
                                                            );
                                break;

                        }
                    }

                    #endregion


                    #region OnJSONMessageResponseReceived

                    var logger = OnJSONMessageResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONMessageResponseReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonResponse
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONMessageResponseReceived));
                        }
                    }

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonResponse.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONResponseMessage(jsonResponse);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonResponse.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonResponse.DestinationId))
                    {
                        parentNetworkingNode.OCPP.ReceiveJSONResponse(jsonResponse);
                    }

                    // No response to the charging station!

                }

                else if (OCPP_JSONRequestErrorMessage.   TryParse(JSONMessage, out var jsonErrorResponse,                                                             sourceNodeId))
                {

                    #region OnJSONErrorResponseReceived

                    var logger = OnJSONErrorResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONErrorResponseReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  jsonErrorResponse
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONErrorResponseReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveJSONRequestError(jsonErrorResponse);

                    // No response to the charging station!

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

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.InternalError(
                                        nameof(OCPPWebSocketAdapterIN),
                                        EventTrackingId,
                                        JSONMessage,
                                        e
                                    );

            }


            #region OnJSONErrorResponseSent

            //if (OCPPErrorResponse is not null)
            //{

            //    var now = Timestamp.Now;

            //    var onJSONErrorResponseSent = OnJSONErrorResponseSent;
            //    if (onJSONErrorResponseSent is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onJSONErrorResponseSent.GetInvocationList().
            //                                   OfType<OnWebSocketTextErrorResponseDelegate>().
            //                                   Select(loggingDelegate => loggingDelegate.Invoke(
            //                                                                  now,
            //                                                                  this,
            //                                                                  Connection,
            //                                                                  EventTrackingId,
            //                                                                  RequestTimestamp,
            //                                                                  TextMessage,
            //                                                                  [],
            //                                                                  now,
            //                                                                  (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting) ?? "",
            //                                                                  CancellationToken
            //                                                              )).
            //                                   ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONErrorResponseSent));
            //        }
            //    }

            //}

            #endregion


            // The response is empty!
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       JSONMessage.ToString(),
                       Timestamp.Now,
                       String.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region ProcessBinaryMessage (RequestTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime              RequestTimestamp,
                                                                               IWebSocketConnection  WebSocketConnection,
                                                                               Byte[]                BinaryMessage,
                                                                               EventTracking_Id      EventTrackingId,
                                                                               CancellationToken     CancellationToken)
        {

            OCPP_JSONResponseMessage?    OCPPResponse         = null;
            OCPP_BinaryResponseMessage?  OCPPBinaryResponse   = null;
            OCPP_JSONRequestErrorMessage?       OCPPErrorResponse    = null;

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError,  RequestTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequest  is not null)
                {

                    #region Fix DestinationNodeId and network path for standard networking connections

                    if (binaryRequest.NetworkingMode    == NetworkingMode.Standard &&
                        binaryRequest.DestinationId == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryRequest = binaryRequest.ChangeDestinationNodeId(
                                                                  parentNetworkingNode.Id,
                                                                  binaryRequest.NetworkPath.Append(sourceNodeId.Value)
                                                              );
                                break;

                            case WebSocketServerConnection:
                                binaryRequest = binaryRequest.ChangeDestinationNodeId(
                                                                  NetworkingNode_Id.CSMS,
                                                                  binaryRequest.NetworkPath.Append(sourceNodeId.Value)
                                                              );
                                break;

                        }
                    }

                    #endregion



                    #region OnBinaryMessageRequestReceived

                    var logger = OnBinaryMessageRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryMessageRequestReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  binaryRequest
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryMessageRequestReceived));
                        }
                    }

                    #endregion


                    // When not for this node, send it to the FORWARD processor...
                    if (binaryRequest.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryRequestMessage(binaryRequest);

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
                                    await parentNetworkingNode.OCPP.SendJSONResponse(OCPPResponse);

                                if (OCPPErrorResponse is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError   (OCPPErrorResponse);

                            }

                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                                if (OCPPBinaryResponse is not null)
                                    await parentNetworkingNode.OCPP.SendBinaryResponse(OCPPBinaryResponse);

                                if (OCPPErrorResponse is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError     (OCPPErrorResponse);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply = await ocppProcessor;

                                OCPPResponse         = ocppReply.JSONResponseMessage;
                                OCPPErrorResponse    = ocppReply.JSONErrorMessage;
                                OCPPBinaryResponse   = ocppReply.BinaryResponseMessage;

                                if (ocppReply.JSONResponseMessage is not null)
                                    await parentNetworkingNode.OCPP.SendJSONResponse  (ocppReply.JSONResponseMessage);

                                if (ocppReply.JSONErrorMessage is not null)
                                    await parentNetworkingNode.OCPP.SendJSONRequestError     (ocppReply.JSONErrorMessage);

                                if (ocppReply.BinaryResponseMessage is not null)
                                    await parentNetworkingNode.OCPP.SendBinaryResponse(ocppReply.BinaryResponseMessage);

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
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONMessageResponseSent  (OCPPResponse);

                    if (OCPPErrorResponse  is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyJSONErrorResponseSent    (OCPPErrorResponse);

                    if (OCPPBinaryResponse is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyBinaryMessageResponseSent(OCPPBinaryResponse);

                    #endregion

                }

                else if (OCPP_BinaryResponseMessage.TryParse(BinaryMessage, out var binaryResponse, out var responseParsingError,                                    sourceNodeId)                    && binaryResponse is not null)
                {

                    #region OnBinaryMessageResponseReceived

                    var logger = OnBinaryMessageResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryMessageResponseReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  binaryResponse
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryMessageResponseReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryResponse(binaryResponse);

                    // No response to the charging station!

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
                       RequestTimestamp,
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
