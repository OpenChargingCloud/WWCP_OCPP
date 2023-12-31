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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The networking node HTTP WebSocket client runs on a networking node
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Data

        private   readonly  TestNetworkingNode              parentNetworkingNode;

        protected readonly  Dictionary<String, MethodInfo>  incomingMessageProcessorsLookup   = [];

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
        /// Create a new OCPP HTTP Web Socket adapter.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterIN(TestNetworkingNode NetworkingNode)
        {

            this.parentNetworkingNode = NetworkingNode;

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var method in typeof(OCPPWebSocketAdapterIN).
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


        #region ProcessJSONMessage  (RequestTimestamp, WebSocketConnection, JSONMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="WebSocketConnection">The WebSocket connection.</param>
        /// <param name="JSONMessage">The received JSON message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public async Task<WebSocketTextMessageResponse> ProcessJSONMessage(DateTime              RequestTimestamp,
                                                                           IWebSocketConnection  WebSocketConnection,
                                                                           JArray                JSONMessage,
                                                                           EventTracking_Id      EventTrackingId,
                                                                           CancellationToken     CancellationToken)
        {

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                var sourceNodeId  = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(TestNetworkingNode.NetworkingNodeId_WebSocketKey);

                if      (OCPP_JSONRequestMessage. TryParse(JSONMessage, out var jsonRequest,  out var requestParsingError,  RequestTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && jsonRequest       is not null)
                {

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

                    #region Try to call the matching 'forwarding message processor'...

                    // ToDo: ...

                    #endregion

                    #region Try to call the matching 'incoming message processor'...

                    if (incomingMessageProcessorsLookup.TryGetValue(jsonRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                        //ToDo: Maybe this could be done via code generation!
                        var result = methodInfo.Invoke(this,
                                                       [ jsonRequest.RequestTimestamp,
                                                         WebSocketConnection,
                                                         jsonRequest.DestinationNodeId,
                                                         jsonRequest.NetworkPath,
                                                         jsonRequest.EventTrackingId,
                                                         jsonRequest.RequestId,
                                                         jsonRequest.Payload,
                                                         jsonRequest.CancellationToken ]);

                        if (result is Task<Tuple<OCPP_JSONResponseMessage?, OCPP_JSONErrorMessage?>> textProcessor) {
                            (OCPPResponse, OCPPErrorResponse) = await textProcessor;
                        }

                        else
                            DebugX.Log($"Received undefined '{jsonRequest.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                    }

                    #endregion

                    #region ...or error!

                    else
                    {

                        DebugX.Log($"Received unknown '{jsonRequest.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        OCPPErrorResponse = new OCPP_JSONErrorMessage(
                                                Timestamp.Now,
                                                EventTracking_Id.New,
                                                jsonRequest.RequestId,
                                                ResultCode.ProtocolError,
                                                $"The OCPP message '{jsonRequest.Action}' is unkown!",
                                                new JObject(
                                                    new JProperty("request", JSONMessage)
                                                )
                                            );

                    }

                    #endregion


                    #region NotifyJSON(Message/Error)ResponseSent

                    var now = Timestamp.Now;

                    if (OCPPResponse is not null)
                        await parentNetworkingNode.ocppOUT.NotifyJSONMessageResponseSent(OCPPResponse);

                    if (OCPPErrorResponse is not null)
                        await parentNetworkingNode.ocppOUT.NotifyJSONErrorResponseSent(OCPPErrorResponse);

                    #endregion

                }

                else if (OCPP_JSONResponseMessage.TryParse(JSONMessage, out var jsonResponse, out var responseParsingError,                                    sourceNodeId)                    && jsonResponse      is not null)
                {

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

                    parentNetworkingNode.ReceiveResponseMessage(jsonResponse);

                    // No response to the charging station!

                }

                else if (OCPP_JSONErrorMessage.   TryParse(JSONMessage, out var jsonErrorResponse,                                                             sourceNodeId)                    && jsonErrorResponse is not null)
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

                    parentNetworkingNode.ReceiveErrorMessage(jsonErrorResponse);

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

                OCPPErrorResponse = OCPP_JSONErrorMessage.InternalError(
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


            // The response to the charging station... might be empty!
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       JSONMessage.ToString(),
                       Timestamp.Now,
                       (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(Formatting.None) ?? String.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region ProcessBinaryMessage(RequestTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime              RequestTimestamp,
                                                                               IWebSocketConnection  WebSocketConnection,
                                                                               Byte[]                BinaryMessage,
                                                                               EventTracking_Id      EventTrackingId,
                                                                               CancellationToken     CancellationToken)
        {

            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(TestNetworkingNode.NetworkingNodeId_WebSocketKey);

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError,  RequestTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequest  is not null)
                {

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

                    #region Try to call the matching 'incoming message processor'

                    if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                        var result = methodInfo.Invoke(this,
                                                       [ binaryRequest.RequestTimestamp,
                                                         WebSocketConnection,
                                                         binaryRequest.DestinationNodeId,
                                                         binaryRequest.NetworkPath,
                                                         binaryRequest.EventTrackingId,
                                                         binaryRequest.RequestId,
                                                         binaryRequest.Payload,
                                                         binaryRequest.CancellationToken ]);

                        if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>> binaryProcessor)
                        {
                            (OCPPResponse, OCPPErrorResponse) = await binaryProcessor;
                        }

                        else
                            DebugX.Log($"Received undefined '{binaryRequest.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                    }

                    #endregion

                    #region ...or error!

                    else
                    {

                        DebugX.Log($"Received unknown '{binaryRequest.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        OCPPErrorResponse = new OCPP_JSONErrorMessage(
                                                Timestamp.Now,
                                                EventTracking_Id.New,
                                                binaryRequest.RequestId,
                                                ResultCode.ProtocolError,
                                                $"The OCPP message '{binaryRequest.Action}' is unkown!",
                                                new JObject(
                                                    new JProperty("request", BinaryMessage.ToBase64())
                                                )
                                            );

                    }

                    #endregion


                    #region NotifyJSON(Message/Error)ResponseSent

                    if (OCPPResponse is not null)
                        await parentNetworkingNode.ocppOUT.NotifyBinaryMessageResponseSent(OCPPResponse);

                    if (OCPPErrorResponse is not null)
                        await parentNetworkingNode.ocppOUT.NotifyJSONErrorResponseSent(OCPPErrorResponse);

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

                    parentNetworkingNode.ReceiveResponseMessage(binaryResponse);

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

                OCPPErrorResponse = OCPP_JSONErrorMessage.InternalError(
                                        nameof(OCPPWebSocketAdapterIN),
                                        EventTrackingId,
                                        BinaryMessage,
                                        e
                                    );

            }


            return new WebSocketBinaryMessageResponse(
                       RequestTimestamp,
                       BinaryMessage,
                       Timestamp.Now,
                       OCPPResponse?.ToByteArray() ?? [],
                       EventTrackingId
                   );

        }

        #endregion


    }

}
