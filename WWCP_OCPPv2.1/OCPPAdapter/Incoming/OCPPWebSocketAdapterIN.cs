/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageReceivedDelegate         (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONRequestMessage          JSONRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseMessageReceivedDelegate        (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONResponseMessage         JSONResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONRequestErrorMessageReceivedDelegate    (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseErrorMessageReceivedDelegate   (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONSendMessageReceivedDelegate            (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_JSONSendMessage             JSONSendMessage,
                                                                       CancellationToken                CancellationToken);



    public delegate Task OnBinaryRequestMessageReceivedDelegate       (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryRequestMessage        BinaryRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseMessageReceivedDelegate      (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryResponseMessage       BinaryResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryRequestErrorMessageReceivedDelegate  (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseErrorMessageReceivedDelegate (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IWebSocketConnection?            WebSocketConnection,
                                                                       OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinarySendMessageReceivedDelegate          (DateTimeOffset                   Timestamp,
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


        #region (private) LearnRouteBack (SenderId, WebSocketConnection)

        /// <summary>
        /// Remember that the given node can be reached over the connection one of its
        /// requests just arrived on, unless we already know a way there.
        /// </summary>
        /// <param name="SenderId">The node that sent us a request.</param>
        /// <param name="WebSocketConnection">The connection it arrived on.</param>
        private void LearnRouteBack(NetworkingNode_Id     SenderId,
                                    IWebSocketConnection  WebSocketConnection)
        {

            if (SenderId == NetworkingNode_Id.Zero ||
                SenderId == parentNetworkingNode.Id)
                return;

            if (parentNetworkingNode.Routing.LookupNetworkingNode(SenderId, out _))
                return;

            // Only for a client connection. It has exactly one far end, so "reachable via this
            // client" is unambiguous. A server carries many connections, and a route pointing at
            // the server as a whole would claim we can reach this node while the server still has
            // to find a connection registered under that very identification - which is precisely
            // what it could not do. Incoming server connections already register themselves.
            if (WebSocketConnection is WebSocketClientConnection clientConnection &&
                clientConnection.WebSocketClient is IWWCPWebSocketClient wwcpWebSocketClient)
            {
                parentNetworkingNode.Routing.AddOrUpdateStaticRouting(SenderId, wwcpWebSocketClient);
            }

        }

        #endregion

        #region (private) AddressToSender (RequestError, RequestNetworkPath)

        /// <summary>
        /// Return the given request error addressed at the node whose request caused it.
        /// </summary>
        /// <remarks>
        /// A request error is built where the parsing failed. That place knows the request
        /// identification and the action, but not who sent them, so every factory on
        /// OCPP_JSONRequestErrorMessage fills in SourceRouting.Zero - and a message addressed
        /// to nobody is dropped as "UnknownClient". The sender then waits out its whole
        /// request timeout for an answer that was written and thrown away, and cannot tell a
        /// rejected message from a lost one.
        ///
        /// The successful path two lines further up already does this: it answers with
        /// SourceRouting.To(NetworkPath.Source). This gives the error path the same address.
        /// </remarks>
        /// <param name="RequestError">The request error to address.</param>
        /// <param name="RequestNetworkPath">The network path of the request that caused it.</param>
        private OCPP_JSONRequestErrorMessage AddressToSender(OCPP_JSONRequestErrorMessage  RequestError,
                                                             NetworkPath                   RequestNetworkPath)
        {

            // Whoever built it knew where it should go: leave it alone.
            if (RequestError.Destination.Next != NetworkingNode_Id.Zero)
                return RequestError;

            // An empty network path leaves us no better idea than the factory had.
            if (RequestNetworkPath.Source == NetworkingNode_Id.Zero)
                return RequestError;

            return RequestError.ChangeNetworking(
                       SourceRouting.To(RequestNetworkPath.Source),
                       NetworkPath.From(parentNetworkingNode.Id)
                   );

        }

        /// <summary>
        /// Return the given binary request error addressed at the node whose request caused it.
        /// </summary>
        /// <param name="RequestError">The request error to address.</param>
        /// <param name="RequestNetworkPath">The network path of the request that caused it.</param>
        private OCPP_BinaryRequestErrorMessage AddressToSender(OCPP_BinaryRequestErrorMessage  RequestError,
                                                               NetworkPath                     RequestNetworkPath)
        {

            if (RequestError.Destination.Next != NetworkingNode_Id.Zero)
                return RequestError;

            if (RequestNetworkPath.Source == NetworkingNode_Id.Zero)
                return RequestError;

            return RequestError.ChangeNetworking(
                       SourceRouting.To(RequestNetworkPath.Source),
                       NetworkPath.From(parentNetworkingNode.Id)
                   );

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
        public async Task ProcessJSONMessage(DateTimeOffset        MessageTimestamp,
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

                    // Most likely the first request message of a new connection (e.g. a BootNotificationRequest) will set the networking mode
                    WebSocketConnection.TryAddCustomData(WebSocketKeys.X_WWCP_NetworkingMode, jsonRequestMessage.NetworkingMode);

                    #region Learn the way back to whoever sent this

                    // The response to this request will be addressed to the node that sent it. If we
                    // do not know how to reach that node, the response is dropped as "UnknownClient"
                    // and the sender waits out its whole request timeout for an answer that was
                    // produced, accepted and then thrown away.
                    //
                    // That happens whenever a connection is registered under a well-known alias
                    // rather than under the identification its far end actually puts into the
                    // network path - a local controller dialling NetworkingNode_Id.CSMS, for
                    // instance, while the CSMS signs its requests as "csms01".
                    //
                    // An incoming request is proof that its sender is reachable this way, so
                    // remember it. Existing routes win: this only fills in what we do not know.
                    LearnRouteBack(jsonRequestMessage.NetworkPath.Source, WebSocketConnection);

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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (AddressToSender(ocppResponse.JSONRequestErrorMessage, jsonRequestMessage.NetworkPath));

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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, jsonRequestMessage.NetworkPath));

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
                            DebugX.LogException(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONRequestErrorMessageReceived));
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
                            DebugX.LogException(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnJSONResponseErrorMessageReceived));
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

                    // Most likely the first send message of a new connection (e.g. a RoutingInformation) will set the networking mode
                    WebSocketConnection.TryAddCustomData(WebSocketKeys.X_WWCP_NetworkingMode, jsonSendMessage.NetworkingMode);


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
                                    sentMessageResult  = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(AddressToSender(ocppResponse.JSONRequestErrorMessage, jsonSendMessage.NetworkPath));


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

                // A frame that is none of the five kinds ends up here. Report every reason we
                // have, and name the kind, so a dropped message does not look like a different
                // one that failed - or like nothing at all.
                else if (sendParsingError     is not null)
                    DebugX.Log($"Failed to parse a JSON send message within {nameof(OCPPWebSocketAdapterIN)}: '{sendParsingError}'{Environment.NewLine}'{JSONMessage.ToString(Formatting.None)}'!");

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

        public async Task ProcessBinaryMessage(DateTimeOffset        MessageTimestamp,
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


                    #region Learn the way back to whoever sent this

                    // The response to this request will be addressed to the node that sent it. If we
                    // do not know how to reach that node, the response is dropped as "UnknownClient"
                    // and the sender waits out its whole request timeout for an answer that was
                    // produced, accepted and then thrown away.
                    //
                    // That happens whenever a connection is registered under a well-known alias
                    // rather than under the identification its far end actually puts into the
                    // network path - a local controller dialling NetworkingNode_Id.CSMS, for
                    // instance, while the CSMS signs its requests as "csms01".
                    //
                    // An incoming request is proof that its sender is reachable this way, so
                    // remember it. Existing routes win: this only fills in what we do not know.
                    LearnRouteBack(binaryRequestMessage.NetworkPath.Source, WebSocketConnection);

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
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (AddressToSender(ocppResponse.JSONRequestErrorMessage, binaryRequestMessage.NetworkPath));

                                if (ocppResponse.JSONResponseMessage       is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse      (ocppResponse.JSONResponseMessage);

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, binaryRequestMessage.NetworkPath));

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
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, binarySendMessage.NetworkPath));


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

                // A frame that is none of the five kinds ends up here. Report every reason we
                // have, and name the kind, so a dropped message does not look like a different
                // one that failed - or like nothing at all.
                else if (sendParsingError     is not null)
                    DebugX.Log($"Failed to parse a binary send message within {nameof(OCPPWebSocketAdapterIN)}: '{sendParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

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

        #region (private) HandleErrors  (Caller, ExceptionOccurred)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccurred)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterIN),
                   Caller,
                   ExceptionOccurred
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
