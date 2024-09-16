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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;

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

        protected readonly  Dictionary<String, MethodInfo>  incomingJSONMessageProcessorsLookup     = [];
        protected readonly  Dictionary<String, MethodInfo>  incomingBinaryMessageProcessorsLookup   = [];

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

            this.AnycastIds.Add(NetworkingNode_Id.Broadcast);

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var methodInfo in typeof(OCPPWebSocketAdapterIN).
                                           GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                                Where(method            => method.Name.StartsWith("Receive_") &&
                                                     (method.ReturnType == typeof(Task<OCPP_Response>))))
            {

                var processorName = methodInfo.Name[8..];

                if (incomingJSONMessageProcessorsLookup.ContainsKey(processorName) &&
                    incomingBinaryMessageProcessorsLookup.ContainsKey(processorName))
                {
                    throw new ArgumentException($"Duplicate processor name: '{processorName}'!");
                }

                var parameterInfos = methodInfo.GetParameters();

                if (parameterInfos.Length > 7)
                {

                    if      (parameterInfos[6].ParameterType == typeof(JObject))
                        incomingJSONMessageProcessorsLookup.Add(
                            processorName,
                            methodInfo
                        );

                    else if (parameterInfos[6].ParameterType == typeof(Byte[]))
                        incomingBinaryMessageProcessorsLookup.Add(
                            processorName,
                            methodInfo
                        );

                    else
                        throw new ArgumentException($"Invalid method found: '{methodInfo.Name}'!");

                }
                else
                    throw new ArgumentException($"Invalid method found: '{methodInfo.Name}'!");

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
        public async Task ProcessJSONMessage(DateTime              MessageTimestamp,
                                             IWebSocketConnection  WebSocketConnection,
                                             NetworkingNode_Id?    sourceNodeId,
                                             JArray                JSONMessage,
                                             EventTracking_Id      EventTrackingId,
                                             CancellationToken     CancellationToken)
        {

            try
            {

                SentMessageResult? sentMessageResult = null;

                //var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_JSONRequestMessage.      TryParse(JSONMessage, out var jsonRequestMessage,       out var requestParsingError,  MessageTimestamp, null, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (jsonRequestMessage.NetworkingMode   == NetworkingMode.Standard &&
                        jsonRequestMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonRequestMessage = jsonRequestMessage.ChangeNetworking(
                                                         SourceRouting.To(parentNetworkingNode.Id),
                                                         jsonRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                     );
                                break;

                            case WebSocketServerConnection:
                                jsonRequestMessage = jsonRequestMessage.ChangeNetworking(
                                                         SourceRouting.To(NetworkingNode_Id.CSMS),
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

                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonRequestMessage.Destination.Next);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonRequestMessage.Destination.Next != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONRequestMessage(jsonRequestMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonRequestMessage.Destination.Next == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingJSONMessageProcessorsLookup.TryGetValue(jsonRequestMessage.Action, out var methodInfo))
                        {

                            var resultTask = methodInfo.Invoke(
                                                 this,
                                                 [
                                                     jsonRequestMessage.RequestTimestamp,
                                                     WebSocketConnection,
                                                     jsonRequestMessage.Destination,
                                                     jsonRequestMessage.NetworkPath,
                                                     jsonRequestMessage.EventTrackingId,
                                                     jsonRequestMessage.RequestId,
                                                     jsonRequestMessage.Payload,
                                                     jsonRequestMessage.CancellationToken
                                                 ]
                                             );

                            if (resultTask is Task<OCPP_Response> ocppResponseTask)
                            {

                                var ocppResponse = await ocppResponseTask;

                                if (ocppResponse.JSONResponseMessage       is not null)
                                {

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse(ocppResponse.JSONResponseMessage);

                                    if (sentMessageResult.Result != SentMessageResults.Success)
                                    {
                                        await HandleErrors(
                                                  nameof(ProcessJSONMessage),
                                                  $"Sent JSON Response Message: {ocppResponse.JSONResponseMessage.ToJSON().ToString(Formatting.None)} => '{sentMessageResult}'!"
                                              );
                                    }

                                }

                                if (ocppResponse.JSONRequestErrorMessage   is not null)
                                {

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (ocppResponse.JSONRequestErrorMessage);

                                    if (sentMessageResult.Result != SentMessageResults.Success)
                                    {
                                        await HandleErrors(
                                                  nameof(ProcessJSONMessage),
                                                  $"Sent JSON Request Error Message: {ocppResponse.JSONRequestErrorMessage.ToJSON().ToString(Formatting.None)} => '{sentMessageResult}'!"
                                              );
                                    }

                                }

                                if (ocppResponse.BinaryResponseMessage     is not null)
                                {

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage);

                                    if (sentMessageResult.Result != SentMessageResults.Success)
                                    {
                                        await HandleErrors(
                                                  nameof(ProcessJSONMessage),
                                                  $"Sent Binary Response Message: {ocppResponse.BinaryResponseMessage.ToByteArray().ToBase64()} => '{sentMessageResult}'!"
                                              );
                                    }

                                }

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                {

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppResponse.BinaryRequestErrorMessage);

                                    if (sentMessageResult.Result != SentMessageResults.Success)
                                    {
                                        await HandleErrors(
                                                  nameof(ProcessJSONMessage),
                                                  $"Sent Request Error Message: {ocppResponse.BinaryRequestErrorMessage.ToByteArray().ToBase64()} => '{sentMessageResult}'!"
                                              ); 
                                    }

                                }


                                // Notify about the result of the sent message
                                if (ocppResponse.SentMessageLogger         is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sentMessageResult ?? SentMessageResult.Unknown());


                                if (sentMessageResult is null ||
                                    sentMessageResult.Result != SentMessageResults.Success)
                                {
                                    await HandleErrors(
                                              "JSONRequestMessage",
                                              $"Sent message result: {sentMessageResult}"
                                          );
                                }

                            }

                            else
                                DebugX.Log($"Invalid result type for a '{jsonRequestMessage.Action}' JSON request message processor within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Undefined '{jsonRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.Zero,
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

                }

                else if (OCPP_JSONResponseMessage.     TryParse(JSONMessage, out var jsonResponseMessage,      out var responseParsingError,                                          sourceNodeId))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (jsonResponseMessage.NetworkingMode   == NetworkingMode.Standard &&
                        jsonResponseMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonResponseMessage = jsonResponseMessage.ChangeNetworking(
                                                          SourceRouting.To(parentNetworkingNode.Id),
                                                          jsonResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                      );
                                break;

                            case WebSocketServerConnection:
                                jsonResponseMessage = jsonResponseMessage.ChangeNetworking(
                                                          SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(jsonResponseMessage.RequestId) ?? parentNetworkingNode.Id),
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
                    if (jsonResponseMessage.Destination.Next != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONResponseMessage(jsonResponseMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonResponseMessage.Destination.Next == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonResponseMessage.Destination.Next))
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

                    if (jsonSendMessage.NetworkingMode   == NetworkingMode.Standard &&
                        jsonSendMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                jsonSendMessage = jsonSendMessage.ChangeNetworking(
                                                       SourceRouting.To(parentNetworkingNode.Id),
                                                       jsonSendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                   );
                                break;

                            case WebSocketServerConnection:
                                jsonSendMessage = jsonSendMessage.ChangeNetworking(
                                                      SourceRouting.To(NetworkingNode_Id.CSMS),
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

                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonSendMessage.Destination.Next);

                    // When not for this node, send it to the FORWARD processor...
                    if (jsonSendMessage.Destination.Next != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONSendMessage(jsonSendMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (jsonSendMessage.Destination.Next == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingJSONMessageProcessorsLookup.TryGetValue(jsonSendMessage.Action, out var methodInfo))
                        {

                            var resultTask = methodInfo.Invoke(
                                                 this,
                                                 [
                                                     jsonSendMessage.MessageTimestamp,
                                                     WebSocketConnection,
                                                     jsonSendMessage.Destination,
                                                     jsonSendMessage.NetworkPath,
                                                     jsonSendMessage.EventTrackingId,
                                                     jsonSendMessage.MessageId,
                                                     jsonSendMessage.Payload,
                                                     jsonSendMessage.CancellationToken
                                                 ]
                                             );

                            if (resultTask is Task<OCPP_Response> ocppResponseTask)
                            {

                                var ocppResponse = await ocppResponseTask;

                                if (ocppResponse.JSONRequestErrorMessage is not null)
                                    sentMessageResult  = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(ocppResponse.JSONRequestErrorMessage);


                                // Notify about the result of the sent message
                                if (ocppResponse.SentMessageLogger         is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sentMessageResult ?? SentMessageResult.Unknown());

                            }

                            else
                                DebugX.Log($"Invalid result type for a '{jsonSendMessage.Action}' JSON send message processor within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Undefined '{jsonSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.Zero,
                                          NetworkPath.Empty,
                                          jsonSendMessage.MessageId,
                                          ResultCode.ProtocolError,
                                          $"Received unknown OCPP '{jsonSendMessage.Action}' JSON send message!",
                                          new JObject(
                                              new JProperty("request", JSONMessage)
                                          )
                                      )
                                  );

                        }

                        #endregion

                    }

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

        }

        #endregion

        #region ProcessBinaryMessage (MessageTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public async Task ProcessBinaryMessage(DateTime              MessageTimestamp,
                                               IWebSocketConnection  WebSocketConnection,
                                               NetworkingNode_Id?    sourceNodeId,
                                               Byte[]                BinaryMessage,
                                               EventTracking_Id      EventTrackingId,
                                               CancellationToken     CancellationToken)
        {

            try
            {

                SentMessageResult? sentMessageResult = null;

                //var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                if      (OCPP_BinaryRequestMessage.      TryParse(BinaryMessage, out var binaryRequestMessage,       out var requestParsingError,  MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequestMessage  is not null)
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryRequestMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binaryRequestMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryRequestMessage = binaryRequestMessage.ChangeNetworking(
                                                           SourceRouting.To(parentNetworkingNode.Id),
                                                           binaryRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                       );
                                break;

                            case WebSocketServerConnection:
                                binaryRequestMessage = binaryRequestMessage.ChangeNetworking(
                                                           SourceRouting.To(NetworkingNode_Id.CSMS),
                                                           binaryRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                       );
                                break;

                        }
                    }

                    #endregion


                    #region OnBinaryRequestMessageReceived

                    await LogEvent(
                              OnBinaryRequestMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  binaryRequestMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryRequestMessage.Destination.Next);

                    // When not for this node, send it to the FORWARD processor...
                    if (binaryRequestMessage.Destination.Next != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryRequestMessage(binaryRequestMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binaryRequestMessage.Destination.Next == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'

                        if (incomingBinaryMessageProcessorsLookup.TryGetValue(binaryRequestMessage.Action, out var methodInfo))
                        {

                            var resultTask = methodInfo.Invoke(
                                                 this,
                                                 [
                                                     binaryRequestMessage.RequestTimestamp,
                                                     WebSocketConnection,
                                                     binaryRequestMessage.Destination,
                                                     binaryRequestMessage.NetworkPath,
                                                     binaryRequestMessage.EventTrackingId,
                                                     binaryRequestMessage.RequestId,
                                                     binaryRequestMessage.Payload,
                                                     binaryRequestMessage.CancellationToken
                                                 ]
                                             );

                            if (resultTask is Task<OCPP_Response> ocppResponseTask)
                            {

                                var ocppResponse = await ocppResponseTask;

                                if (ocppResponse.JSONRequestErrorMessage   is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (ocppResponse.JSONRequestErrorMessage);

                                if (ocppResponse.JSONResponseMessage       is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse      (ocppResponse.JSONResponseMessage);

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppResponse.BinaryRequestErrorMessage);

                                if (ocppResponse.BinaryResponseMessage     is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage);


                                // Notify about the result of the sent message
                                if (ocppResponse.SentMessageLogger         is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sentMessageResult ?? SentMessageResult.Unknown());

                            }

                            else
                                DebugX.Log($"Invalid result type for a '{binaryRequestMessage.Action}' binary request message processor within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Undefined '{binaryRequestMessage.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.Zero,
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

                }

                else if (OCPP_BinaryResponseMessage.     TryParse(BinaryMessage, out var binaryResponseMessage,      out var responseParsingError,                                    sourceNodeId)                    && binaryResponseMessage is not null)
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryResponseMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binaryResponseMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binaryResponseMessage = binaryResponseMessage.ChangeNetworking(
                                                            SourceRouting.To(parentNetworkingNode.Id),
                                                            binaryResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                        );
                                break;

                            case WebSocketServerConnection:
                                binaryResponseMessage = binaryResponseMessage.ChangeNetworking(
                                                            SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(binaryResponseMessage.RequestId) ?? parentNetworkingNode.Id),
                                                            binaryResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                        );
                                break;

                        }
                    }

                    #endregion


                    #region OnBinaryResponseMessageReceived

                    await LogEvent(
                              OnBinaryResponseMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  binaryResponseMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    // When not for this node, send it to the FORWARD processor...
                    if (binaryResponseMessage.Destination.Next != parentNetworkingNode.Id)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryResponseMessage(binaryResponseMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binaryResponseMessage.Destination.Next == parentNetworkingNode.Id ||
                        parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryResponseMessage.Destination.Next))
                    {
                        parentNetworkingNode.OCPP.ReceiveBinaryResponse(binaryResponseMessage, WebSocketConnection);
                    }

                    // No response!

                }

                else if (OCPP_BinaryRequestErrorMessage. TryParse(BinaryMessage, out var binaryRequestErrorMessage,                                                                   sourceNodeId))
                {

                    #region OnBinaryRequestErrorMessageReceived

                    await LogEvent(
                              OnBinaryRequestErrorMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  binaryRequestErrorMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryRequestError(binaryRequestErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_BinaryResponseErrorMessage.TryParse(BinaryMessage, out var binaryResponseErrorMessage,                                                                  sourceNodeId))
                {

                    #region OnBinaryResponseErrorMessageReceived

                    await LogEvent(
                              OnBinaryResponseErrorMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  binaryResponseErrorMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    parentNetworkingNode.OCPP.ReceiveBinaryResponseError(binaryResponseErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_BinarySendMessage.         TryParse(BinaryMessage, out var binarySendMessage,          out var sendParsingError,     MessageTimestamp,       EventTrackingId, sourceNodeId, CancellationToken))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binarySendMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binarySendMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection)
                        {

                            case WebSocketClientConnection:
                                binarySendMessage = binarySendMessage.ChangeNetworking(
                                                        SourceRouting.To(parentNetworkingNode.Id),
                                                        binarySendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                                break;

                            case WebSocketServerConnection:
                                binarySendMessage = binarySendMessage.ChangeNetworking(
                                                        SourceRouting.CSMS,
                                                        binarySendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                                break;

                        }
                    }

                    #endregion


                    #region OnBinarySendMessageReceived

                    await LogEvent(
                              OnBinarySendMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  WebSocketConnection,
                                  binarySendMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    var acceptAsAnycast = parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binarySendMessage.Destination.Next);

                    // When not for this node, send it to the FORWARD processor...
                    if (binarySendMessage.Destination.Next != parentNetworkingNode.Id && !acceptAsAnycast)
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinarySendMessage(binarySendMessage, WebSocketConnection);

                    // Directly for this node OR an anycast message for this node...
                    if (binarySendMessage.Destination.Next == parentNetworkingNode.Id ||  acceptAsAnycast)
                    {

                        #region Try to call the matching 'incoming message processor'...

                        if (incomingJSONMessageProcessorsLookup.TryGetValue(binarySendMessage.Action, out var methodInfo))
                        {

                            var resultTask = methodInfo.Invoke(
                                                 this,
                                                 [
                                                     binarySendMessage.MessageTimestamp,
                                                     WebSocketConnection,
                                                     binarySendMessage.Destination,
                                                     binarySendMessage.NetworkPath,
                                                     binarySendMessage.EventTrackingId,
                                                     binarySendMessage.MessageId,
                                                     binarySendMessage.Payload,
                                                     binarySendMessage.CancellationToken
                                                 ]
                                             );

                            if (resultTask is Task<OCPP_Response> ocppResponseTask)
                            {

                                var ocppResponse = await ocppResponseTask;

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(ocppResponse.BinaryRequestErrorMessage);


                                // Notify about the result of the sent message
                                if (ocppResponse.SentMessageLogger is not null)
                                    await ocppResponse.SentMessageLogger.Invoke(sentMessageResult ?? SentMessageResult.Unknown());

                            }

                            else
                                DebugX.Log($"Invalid result type for a '{binarySendMessage.Action}' binary send message processor within {nameof(OCPPWebSocketAdapterIN)}!");

                        }

                        #endregion

                        #region ...or error!

                        else
                        {

                            DebugX.Log($"Undefined '{binarySendMessage.Action}' binary send message handler within {nameof(OCPPWebSocketAdapterIN)}!");

                            await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                                      new OCPP_BinaryRequestErrorMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.Zero,
                                          NetworkPath.Empty,
                                          binarySendMessage.MessageId,
                                          ResultCode.ProtocolError,
                                          $"Received unknown OCPP '{binarySendMessage.Action}' binary send message!",
                                          new JObject(
                                              new JProperty("request", BinaryMessage.ToBase64())
                                          )
                                      )
                                  );

                        }

                        #endregion

                    }

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

        }

        #endregion


        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("anycastIds",   new JArray(AnycastIds.Select(networkingNodeId => networkingNodeId.ToString())))

                       );

            return json;

        }


        #region (private) LogEvent      (Logger, LogHandler, ...)

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

        #region (private) CallProcessor (Processor, ProcessorHandler, ...)

        private async Task<T?> CallProcessor<TDelegate, T>(TDelegate?                                            Processor,
                                                           Func<TDelegate, Task<T>>                              ProcessorHandler,
                                                           [CallerArgumentExpression(nameof(Processor))] String  EventName     = "",
                                                           [CallerMemberName()]                          String  OCPPCommand   = "")

            where TDelegate : Delegate

        {

            if (Processor is not null)
            {
                try
                {

                    var handler = Processor.GetInvocationList().OfType<TDelegate>().FirstOrDefault();

                    if (handler is not null)
                        return await ProcessorHandler(handler);

                }
                catch (Exception e)
                {
                    await HandleErrors($"{OCPPCommand}.{EventName}", e);
                }
            }

            return default;

        }

        #endregion

        #region (private) HandleErrors  (Caller, ErrorResponse)

        private Task HandleErrors(String  Caller,
                                  String  ErrorResponse)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterIN),
                   Caller,
                   ErrorResponse
               );

        #endregion

        #region (private) HandleErrors  (Caller, ExceptionOccured)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterIN),
                   Caller,
                   ExceptionOccured
               );

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => parentNetworkingNode.Id.ToString();

        #endregion

    }

}
