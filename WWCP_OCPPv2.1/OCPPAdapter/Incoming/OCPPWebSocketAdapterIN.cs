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
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageReceivedDelegate         (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONRequestMessage          JSONRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseMessageReceivedDelegate        (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONResponseMessage         JSONResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONRequestErrorMessageReceivedDelegate    (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseErrorMessageReceivedDelegate   (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONSendMessageReceivedDelegate            (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONSendMessage             JSONSendMessage,
                                                                       CancellationToken                CancellationToken);



    public delegate Task OnBinaryRequestMessageReceivedDelegate       (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryRequestMessage        BinaryRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseMessageReceivedDelegate      (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryResponseMessage       BinaryResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryRequestErrorMessageReceivedDelegate  (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseErrorMessageReceivedDelegate (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinarySendMessageReceivedDelegate          (DateTime                         Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinarySendMessage           BinarySendMessage,
                                                                       CancellationToken                CancellationToken);

    #endregion


    /// <summary>
    /// The OCPP adapter for receiving incoming messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN
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

        /// <summary>
        /// An event sent whenever a binary request error was received.
        /// </summary>
        public event OnBinaryRequestErrorMessageReceivedDelegate?    OnBinaryRequestErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a binary response error was received.
        /// </summary>
        public event OnBinaryResponseErrorMessageReceivedDelegate?   OnBinaryResponseErrorMessageReceived;

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
        /// <param name="ParentNetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterIN(INetworkingNode ParentNetworkingNode)
        {

            this.parentNetworkingNode = ParentNetworkingNode;

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

                    await LogEvent(
                              OnJSONRequestMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  jsonRequestMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    SentMessageResult? sendMessageResult  = null;

                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonRequestMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonRequestMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONRequestMessage(jsonRequestMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonRequestMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(jsonRequestMessage.Action, out var methodInfo))
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

                            OCPP_JSONRequestErrorMessage? JSONRequestErrorMessage   = null;
                            OCPP_JSONResponseMessage?     JSONResponseMessage       = null;
                            OCPP_BinaryResponseMessage?   BinaryResponseMessage     = null;

                            // Obsolete!
                            if      (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                                (JSONResponseMessage, JSONRequestErrorMessage) = await textProcessor;

                                if (JSONResponseMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse(JSONResponseMessage);

                                if (JSONRequestErrorMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(JSONRequestErrorMessage);

                            }

                            // Obsolete!
                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (BinaryResponseMessage, JSONRequestErrorMessage) = await binaryProcessor;

                                if (BinaryResponseMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse(BinaryResponseMessage);

                                if (JSONRequestErrorMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(JSONRequestErrorMessage);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppResponse        = await ocppProcessor;

                                JSONResponseMessage     = ocppResponse.JSONResponseMessage;
                                JSONRequestErrorMessage = ocppResponse.JSONRequestErrorMessage;
                                BinaryResponseMessage   = ocppResponse.BinaryResponseMessage;

                                if (ocppResponse.JSONRequestErrorMessage   is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (ocppResponse.JSONRequestErrorMessage);

                                if (ocppResponse.JSONResponseMessage       is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse      (ocppResponse.JSONResponseMessage);

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppResponse.BinaryRequestErrorMessage);

                                if (ocppResponse.BinaryResponseMessage     is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage);

                                if (ocppResponse.SentMessageLogger         is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sendMessageResult ?? SentMessageResult.Unknown());

                            }

                            else
                                DebugX.Log($"Received undefined '{jsonRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                                          new OCPP_JSONRequestErrorMessage(
                                                              Timestamp.Now,
                                                              EventTracking_Id.New,
                                                              NetworkingMode.Unknown,
                                                              NetworkingNode_Id.Zero,
                                                              NetworkPath.Empty,
                                                              jsonRequestMessage.RequestId,
                                                              ResultCode.ProtocolError,
                                                              $"Received unknown OCPP '{jsonRequestMessage.Action}' JSON request message!",
                                                              new JObject(
                                                                  new JProperty("request", JSONMessage)
                                                              )
                                                          )
                                                      );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    //if (JSONRequestErrorMessage is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent     (JSONRequestErrorMessage, sendMessageResult);

                    //if (JSONResponseMessage     is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyJSONResponseMessageSent  (JSONResponseMessage,     sendMessageResult);

                    //if (BinaryResponseMessage   is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyBinaryResponseMessageSent(BinaryResponseMessage,   sendMessageResult);

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

                    await LogEvent(
                              OnJSONResponseMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  jsonResponseMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonResponseMessage.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONResponseMessage(jsonResponseMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonResponseMessage.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonResponseMessage.DestinationId))
                    {
                        parentNetworkingNode.OCPP.ReceiveJSONResponse(jsonResponseMessage, WebSocketConnection);
                    }

                    // No response!

                }

                else if (OCPP_JSONRequestErrorMessage. TryParse(JSONMessage, out var jsonRequestErrorMessage,  out var requestErrorError,                                             sourceNodeId))
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
                                                                                  WebSocketConnection,
                                                                                  jsonRequestErrorMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONRequestErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveJSONRequestError(jsonRequestErrorMessage, WebSocketConnection);

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
                                                                                  WebSocketConnection,
                                                                                  jsonResponseErrorMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONResponseErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveJSONResponseError(jsonResponseErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_JSONSendMessage.         TryParse(JSONMessage, out var jsonSendMessage,          out var sendParsingError,     MessageTimestamp,       EventTrackingId, sourceNodeId, CancellationToken))
                {

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

                    await LogEvent(
                              OnJSONSendMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  jsonSendMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    var sendMessageResult  = SentMessageResult.UnknownClient();
                    var acceptAsAnycast    = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonSendMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonSendMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONSendMessage(jsonSendMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonSendMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(jsonSendMessage.Action, out var methodInfo))
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

                            if (result is Task<OCPP_Response> ocppResponseTask)
                            {

                                var ocppReply          = await ocppResponseTask;

                                if (ocppReply.JSONRequestErrorMessage is not null)
                                    sendMessageResult  = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(ocppReply.JSONRequestErrorMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{jsonSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {
                            OCPP_JSONRequestErrorMessage? JSONRequestErrorMessage = null;
                            JSONRequestErrorMessage = new OCPP_JSONRequestErrorMessage(
                                                          Timestamp.Now,
                                                          EventTracking_Id.New,
                                                          NetworkingMode.Unknown,
                                                          NetworkingNode_Id.Zero,
                                                          NetworkPath.Empty,
                                                          jsonSendMessage.MessageId,
                                                          ResultCode.ProtocolError,
                                                          $"Received unknown '{jsonSendMessage.Action}' JSONSendMessage!",
                                                          new JObject(
                                                              new JProperty("request", JSONMessage)
                                                          )
                                                      );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                 //   if (JSONRequestErrorMessage is not null)
                 //       await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent(JSONRequestErrorMessage, sendMessageResult);

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
                await HandleErrors(
                          nameof(ProcessJSONMessage),
                          e
                          //EventTrackingId,
                          //JSONMessage
                      );
            }

            // The response is empty!
            return new WebSocketTextMessageResponse(
                       MessageTimestamp,
                       JSONMessage.ToString(),
                       Timestamp.Now,
                       String.Empty,
                       EventTrackingId,
                       CancellationToken
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

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_BinaryRequestMessage.      TryParse(BinaryMessage, out var binaryRequestMessage,       out var requestParsingError,  MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequestMessage  is not null)
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryRequestMessage.NetworkingMode == NetworkingMode.Standard &&
                        binaryRequestMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryRequestMessage = binaryRequestMessage.ChangeNetworking(
                                                                  parentNetworkingNode.Id,
                                                                  binaryRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                              );
                                break;

                            case WebSocketServerConnection:
                                binaryRequestMessage = binaryRequestMessage.ChangeNetworking(
                                                                  NetworkingNode_Id.CSMS,
                                                                  binaryRequestMessage.NetworkPath.Append(sourceNodeId.Value)
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
                                                                                  WebSocketConnection,
                                                                                  binaryRequestMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryRequestMessageReceived));
                        }
                    }

                    #endregion

                    SentMessageResult? sendMessageResult = null;
                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryRequestMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (binaryRequestMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryRequestMessage(binaryRequestMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binaryRequestMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'

                        if (incomingMessageProcessorsLookup.TryGetValue(binaryRequestMessage.Action, out var methodInfo))
                        {

                            var result = methodInfo.Invoke(this,
                                                           [ binaryRequestMessage.RequestTimestamp,
                                                             WebSocketConnection,
                                                             binaryRequestMessage.DestinationId,
                                                             binaryRequestMessage.NetworkPath,
                                                             binaryRequestMessage.EventTrackingId,
                                                             binaryRequestMessage.RequestId,
                                                             binaryRequestMessage.Payload,
                                                             binaryRequestMessage.CancellationToken ]);

                            OCPP_JSONResponseMessage?     OCPPResponse         = null;
                            OCPP_BinaryResponseMessage?   OCPPBinaryResponse   = null;
                            OCPP_JSONRequestErrorMessage? OCPPErrorResponse    = null;

                            // Obsolete!
                            if      (result is Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>> textProcessor) {
                                (OCPPResponse, OCPPErrorResponse) = await textProcessor;

                                if (OCPPResponse is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse    (OCPPResponse);

                                if (OCPPErrorResponse is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(OCPPErrorResponse);

                            }

                            // Obsolete!
                            else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>> binaryProcessor) {

                                (OCPPBinaryResponse, OCPPErrorResponse) = await binaryProcessor;

                                if (OCPPBinaryResponse is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse  (OCPPBinaryResponse);

                                if (OCPPErrorResponse is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(OCPPErrorResponse);

                            }

                            else if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppResponse = await ocppProcessor;

                                OCPPErrorResponse    = ocppResponse.JSONRequestErrorMessage;
                                OCPPResponse         = ocppResponse.JSONResponseMessage;
                                OCPPBinaryResponse   = ocppResponse.BinaryResponseMessage;

                                if (ocppResponse.JSONRequestErrorMessage   is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (ocppResponse.JSONRequestErrorMessage);

                                if (ocppResponse.JSONResponseMessage       is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse      (ocppResponse.JSONResponseMessage);

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppResponse.BinaryRequestErrorMessage);

                                if (ocppResponse.BinaryResponseMessage     is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage);

                                if (ocppResponse.SentMessageLogger         is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sendMessageResult ?? SentMessageResult.Unknown());

                            }

                            else
                                DebugX.Log($"Received undefined '{binaryRequestMessage.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                                          new OCPP_JSONRequestErrorMessage(
                                                              Timestamp.Now,
                                                              EventTracking_Id.New,
                                                              NetworkingMode.Unknown,
                                                              NetworkingNode_Id.Zero,
                                                              NetworkPath.Empty,
                                                              binaryRequestMessage.RequestId,
                                                              ResultCode.ProtocolError,
                                                              $"The OCPP message '{binaryRequestMessage.Action}' is unkown!",
                                                              new JObject(
                                                                  new JProperty("request", BinaryMessage.ToBase64())
                                                              )
                                                          )
                                                      );

                        }

                        #endregion

                    }

                    #region NotifyJSON(Message/Error)ResponseSent

                    //if (OCPPResponse       is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyJSONResponseMessageSent  (OCPPResponse,       sendMessageResult);

                    //if (OCPPErrorResponse  is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyJSONRequestErrorSent     (OCPPErrorResponse,  sendMessageResult);

                    //if (OCPPBinaryResponse is not null)
                    //    await parentNetworkingNode.OCPP.OUT.NotifyBinaryResponseMessageSent(OCPPBinaryResponse, sendMessageResult);

                    #endregion

                }

                else if (OCPP_BinaryResponseMessage.     TryParse(BinaryMessage, out var binaryResponseMessage,      out var responseParsingError,                                    sourceNodeId)                    && binaryResponseMessage is not null)
                {


                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryResponseMessage.NetworkingMode == NetworkingMode.Standard &&
                        binaryResponseMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryResponseMessage = binaryResponseMessage.ChangeNetworking(
                                                            parentNetworkingNode.Id, //sourceNodeId.Value,
                                                            binaryResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                        );
                                break;

                            case WebSocketServerConnection:
                                binaryResponseMessage = binaryResponseMessage.ChangeNetworking(
                                                            parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(binaryResponseMessage.RequestId) ?? parentNetworkingNode.Id,
                                                            binaryResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                        );
                                break;

                        }
                    }

                    #endregion


                    #region OnBinaryResponseMessageReceived

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
                                                                                  WebSocketConnection,
                                                                                  binaryResponseMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryResponseMessageReceived));
                        }
                    }

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (binaryResponseMessage.DestinationId != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryResponseMessage(binaryResponseMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binaryResponseMessage.DestinationId == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryResponseMessage.DestinationId))
                    {
                        parentNetworkingNode.OCPP.ReceiveBinaryResponse(binaryResponseMessage, WebSocketConnection);
                    }

                    // No response!

                }

                else if (OCPP_BinaryRequestErrorMessage. TryParse(BinaryMessage, out var binaryRequestErrorMessage,                                                                   sourceNodeId))
                {

                    #region OnBinaryRequestErrorMessageReceived

                    var logger = OnBinaryRequestErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryRequestErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  WebSocketConnection,
                                                                                  binaryRequestErrorMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryRequestErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryRequestError(binaryRequestErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_BinaryResponseErrorMessage.TryParse(BinaryMessage, out var binaryResponseErrorMessage,                                                                  sourceNodeId))
                {

                    #region OnBinaryResponseErrorMessageReceived

                    var logger = OnBinaryResponseErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinaryResponseErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  WebSocketConnection,
                                                                                  binaryResponseErrorMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryResponseErrorMessageReceived));
                        }
                    }

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryResponseError(binaryResponseErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_BinarySendMessage.         TryParse(BinaryMessage, out var binarySendMessage,          out var sendParsingError,     MessageTimestamp,       EventTrackingId, sourceNodeId, CancellationToken))
                {

                    OCPP_BinaryRequestErrorMessage? BinaryRequestErrorMessage   = null;

                    #region Fix DestinationId and network path for standard networking connections

                    if (binarySendMessage.NetworkingMode == NetworkingMode.Standard &&
                        binarySendMessage.DestinationId  == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binarySendMessage = binarySendMessage.ChangeNetworking(
                                                        parentNetworkingNode.Id,
                                                        binarySendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                                break;

                            case WebSocketServerConnection:
                                binarySendMessage = binarySendMessage.ChangeNetworking(
                                                        NetworkingNode_Id.CSMS,
                                                        binarySendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                                break;

                        }
                    }

                    #endregion


                    #region OnBinarySendMessageReceived

                    var logger = OnBinarySendMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnBinarySendMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  WebSocketConnection,
                                                                                  binarySendMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinarySendMessageReceived));
                        }
                    }

                    #endregion

                    var sendMessageResult  = SentMessageResult.UnknownClient();
                    var acceptAsAnycast    = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binarySendMessage.DestinationId);

                    // When not for this node, send it to the FORWARD processor...
                    if (binarySendMessage.DestinationId != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinarySendMessage(binarySendMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binarySendMessage.DestinationId == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingMessageProcessorsLookup.TryGetValue(binarySendMessage.Action, out var methodInfo) &&
                            methodInfo is not null)
                        {

                            //ToDo: Maybe this could be done via code generation!
                            var result = methodInfo.Invoke(this,
                                                           [ binarySendMessage.MessageTimestamp,
                                                             WebSocketConnection,
                                                             binarySendMessage.DestinationId,
                                                             binarySendMessage.NetworkPath,
                                                             binarySendMessage.EventTrackingId,
                                                             binarySendMessage.MessageId,
                                                             binarySendMessage.Payload,
                                                             binarySendMessage.CancellationToken ]);

                            //if      (result is Task<Tuple<OCPP_BinaryResponseMessage?,   OCPP_BinaryRequestErrorMessage?>> textProcessor) {
                            //    (BinaryResponseMessage, BinaryRequestErrorMessage) = await textProcessor;

                            //    if (BinaryResponseMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

                            //    if (BinaryRequestErrorMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendBinaryRequestError(BinaryRequestErrorMessage);

                            //}

                            //else if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_BinaryRequestErrorMessage?>> binaryProcessor) {

                            //    (BinaryResponseMessage, BinaryRequestErrorMessage) = await binaryProcessor;

                            //    if (BinaryResponseMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

                            //    if (BinaryRequestErrorMessage is not null)
                            //        sendresult = await parentNetworkingNode.OCPP.SendBinaryRequestError(BinaryRequestErrorMessage);

                            //}

                            if (result is Task<OCPP_Response> ocppProcessor)
                            {

                                var ocppReply        = await ocppProcessor;

                                BinaryRequestErrorMessage = ocppReply.BinaryRequestErrorMessage;

                                if (ocppReply.BinaryRequestErrorMessage is not null)
                                    sendMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppReply.BinaryRequestErrorMessage);

                            }

                            else
                                DebugX.Log($"Received undefined '{binarySendMessage.Action}' Binary send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Received unknown '{binarySendMessage.Action}' Binary send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            BinaryRequestErrorMessage = new OCPP_BinaryRequestErrorMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    NetworkingMode.Unknown,
                                                    NetworkingNode_Id.Zero,
                                                    NetworkPath.Empty,
                                                    binarySendMessage.MessageId,
                                                    ResultCode.ProtocolError,
                                                    $"The OCPP message '{binarySendMessage.Action}' is unkown!",
                                                    new JObject(
                                                        new JProperty("request", BinaryMessage)
                                                    )
                                                );

                        }

                        #endregion

                    }

                    #region NotifyBinary(Message/Error)ResponseSent

                    if (BinaryRequestErrorMessage is not null)
                        await parentNetworkingNode.OCPP.OUT.NotifyBinaryRequestErrorSent(BinaryRequestErrorMessage, sendMessageResult);

                    #endregion

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
                await HandleErrors(
                          nameof(ProcessBinaryMessage),
                          e
                          //EventTrackingId,
                          //BinaryMessage
                      );
            }


            // The response is empty!
            return new WebSocketBinaryMessageResponse(
                       MessageTimestamp,
                       BinaryMessage,
                       Timestamp.Now,
                       [],
                       EventTrackingId,
                       CancellationToken
                   );

        }

        #endregion



        #region (private) LogEvent(Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentNetworkingNode.LogEvent(
                   nameof(OCPPWebSocketAdapterIN),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPPCommand
               );

        #endregion

        #region (private) HandleErrors(Caller, ExceptionOccured)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterIN),
                   Caller,
                   ExceptionOccured
               );

        #endregion




        [Obsolete]
        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterIN),
                   Caller,
                   ExceptionOccured
               );

    }

}
