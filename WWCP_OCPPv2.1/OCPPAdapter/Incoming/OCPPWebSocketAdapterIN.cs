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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageReceivedDelegate         (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_JSONRequestMessage          JSONRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseMessageReceivedDelegate        (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_JSONResponseMessage         JSONResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONRequestErrorMessageReceivedDelegate    (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseErrorMessageReceivedDelegate   (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnJSONSendMessageReceivedDelegate            (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_JSONSendMessage             JSONSendMessage,
                                                                       CancellationToken                CancellationToken);



    public delegate Task OnBinaryRequestMessageReceivedDelegate       (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_BinaryRequestMessage        BinaryRequestMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseMessageReceivedDelegate      (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_BinaryResponseMessage       BinaryResponseMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryRequestErrorMessageReceivedDelegate  (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseErrorMessageReceivedDelegate (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
                                                                       OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                                       CancellationToken                CancellationToken);

    public delegate Task OnBinarySendMessageReceivedDelegate          (DateTimeOffset                   Timestamp,
                                                                       OCPPWebSocketAdapterIN           Sender,
                                                                       IMessageConnection?              WebSocketConnection,
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

        /// <summary>
        /// The current time, as the networking node this adapter belongs to sees it.
        /// </summary>
        /// <remarks>
        /// Every timestamp this adapter puts into a message comes from here and
        /// not from the global clock, so that a node moved through time takes
        /// its whole OCPP side along with it.
        /// </remarks>
        public DateTimeOffset Now
            => parentNetworkingNode.Now;

        public HashSet<NetworkingNode_Id>  AnycastIds    { get; } = [];

        /// <summary>
        /// Asked before a request for this node is processed, with the
        /// connection it came over, its action and its JSON payload - none for a
        /// binary one. A sentence, why it is not processed, keeps it from being
        /// processed, and the asker is answered with a SecurityError that says
        /// so; null lets it through. For a connection whose server is not
        /// believed yet: one whose host key is on probation (white paper "OCPP
        /// over SSH", section 10.4) executes nothing but a signed pin set.
        /// </summary>
        public Func<IMessageConnection, String, JObject?, String?>?  RequestGate  { get; set; }

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
                                    IMessageConnection    WebSocketConnection)
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
            if (WebSocketConnection.WWCPClientOf() is IWWCPClient wwcpClient)
            {
                parentNetworkingNode.Routing.AddOrUpdateStaticRouting(SenderId, wwcpClient);
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

        #region (private) TellTheAnswererItCannotBeRead(Connection, NetworkPath, RequestId, EventTrackingId, Binary, Why, ...)

        /// <summary>
        /// Tell whoever answered a request of this node that its answer cannot be read:
        /// a response error with FormationViolation, under the request id of the answer,
        /// in the format the answer came in.
        /// </summary>
        /// <remarks>
        /// <para>
        /// "A CALLRESULTERROR is sent back on receipt of a CALLRESULT that contains errors to
        /// notify the sender that the response could not be processed", under the id of that
        /// CALLRESULT (OCPP 2.1 Edition 2, Part 4, 4.2.3 and 4.1.4). This node never sent one,
        /// and whoever answered never heard that its answer was of no use.
        /// </para>
        /// <para>
        /// Only over a connection that speaks OCPP 2.1. OCPP 2.0.1, which this node speaks as
        /// well, has no CALLRESULTERROR: a system that gets a message type it does not know
        /// ignores it at best.
        /// </para>
        /// </remarks>
        /// <param name="Connection">The connection the answer came over.</param>
        /// <param name="ResponseNetworkPath">The network path of the answer.</param>
        /// <param name="RequestId">The request id of the answer.</param>
        /// <param name="EventTrackingId">The event tracking identification of the request.</param>
        /// <param name="Binary">Whether the answer was a binary one.</param>
        /// <param name="Why">Why the answer cannot be read.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        private async Task TellTheAnswererItCannotBeRead(IMessageConnection?  Connection,
                                                         NetworkPath          ResponseNetworkPath,
                                                         Request_Id           RequestId,
                                                         EventTracking_Id     EventTrackingId,
                                                         Boolean              Binary,
                                                         String?              Why,
                                                         CancellationToken    CancellationToken)
        {

            if (Connection?.Subprotocol != Version.WebSocketSubProtocolId)
                return;

            // An empty network path leaves no idea where to.
            if (ResponseNetworkPath.Source == NetworkingNode_Id.Zero)
                return;

            var description = $"The answer cannot be read: {(Why.IsNotNullOrEmpty() ? Why : "-")}";

            try
            {

                if (Binary)
                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponseError(
                              new OCPP_BinaryResponseErrorMessage(
                                  Now,
                                  EventTrackingId,
                                  NetworkingMode.Unknown,
                                  SourceRouting.To(ResponseNetworkPath.Source),
                                  NetworkPath.From(parentNetworkingNode.Id),
                                  RequestId,
                                  ResultCode.FormationViolation,
                                  description,
                                  CancellationToken: CancellationToken
                              )
                          );

                else
                    await parentNetworkingNode.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Now,
                                  EventTrackingId,
                                  NetworkingMode.Unknown,
                                  SourceRouting.To(ResponseNetworkPath.Source),
                                  NetworkPath.From(parentNetworkingNode.Id),
                                  RequestId,
                                  ResultCode.FormationViolation,
                                  description,
                                  CancellationToken: CancellationToken
                              )
                          );

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(TellTheAnswererItCannotBeRead));
            }

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
        #region (private) RefusalOf(Connection, Action, Payload)

        /// <summary>
        /// Why the <see cref="RequestGate"/> keeps a request from being
        /// processed - or null, where there is no gate or it lets it through. A
        /// gate that fails keeps it, rather than let through what it was there
        /// to stop.
        /// </summary>
        private String? RefusalOf(IMessageConnection  Connection,
                                  String              Action,
                                  JObject?            Payload)
        {

            if (RequestGate is not Func<IMessageConnection, String, JObject?, String?> gate)
                return null;

            try
            {
                return gate(Connection, Action, Payload);
            }
            catch (Exception exception)
            {
                return $"This node could not decide whether to process '{Action}': {exception.Message}";
            }

        }

        #endregion

        public async Task ProcessJSONMessage(DateTimeOffset        MessageTimestamp,
                                             IMessageConnection    WebSocketConnection,
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
                        switch (WebSocketConnection.Role)
                        {

                            case ConnectionRole.Client:
                                jsonRequestMessage = jsonRequestMessage.ChangeNetworking(
                                                         SourceRouting.To(parentNetworkingNode.Id),
                                                         jsonRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                     );
                                break;

                            case ConnectionRole.Server:
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
                                  Now,
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

                        if (RefusalOf(WebSocketConnection, jsonRequestMessage.Action, jsonRequestMessage.Payload) is String refusal)
                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.To(jsonRequestMessage.NetworkPath.Source),
                                          NetworkPath.From(parentNetworkingNode.Id),
                                          jsonRequestMessage.RequestId,
                                          ResultCode.SecurityError,
                                          refusal,
                                          new JObject()
                                      ),
                                      WebSocketConnection
                                  );

                        else if (incomingJSONMessageProcessorsLookup.TryGetValue(jsonRequestMessage.Action, out var methodInfo))
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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse(ocppResponse.JSONResponseMessage, WebSocketConnection);

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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (AddressToSender(ocppResponse.JSONRequestErrorMessage, jsonRequestMessage.NetworkPath), WebSocketConnection);

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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage, WebSocketConnection);

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

                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, jsonRequestMessage.NetworkPath), WebSocketConnection);

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

                            // Back to whoever asked. Addressed to nobody, the refusal found
                            // no way out over a connection this node accepted, and the asker
                            // waited out its whole timeout.
                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.To(jsonRequestMessage.NetworkPath.Source),
                                          NetworkPath.From(parentNetworkingNode.Id),
                                          jsonRequestMessage.RequestId,
                                          ResultCode.ProtocolError,
                                          $"Received unknown OCPP '{jsonRequestMessage.Action}' JSON request message!",
                                          new JObject(
                                              new JProperty("request", JSONMessage)
                                          )
                                      ),
                                      WebSocketConnection
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
                        // Whichever way the connection was made: an answer to a request
                        // this node forwarded, under a request identification of its
                        // own (see FORWARD), goes back to whoever asked, and every other
                        // answer is this node's. A local controller's CSMS answers over
                        // the controller's client connection, and there an answer to a
                        // station's request used to be taken for the controller's own.
                        jsonResponseMessage = jsonResponseMessage.ChangeNetworking(
                                                  SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(jsonResponseMessage.RequestId) ?? parentNetworkingNode.Id),
                                                  jsonResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                              );
                    }

                    #endregion


                    #region OnJSONMessageResponseReceived

                    await LogEvent(
                              OnJSONResponseMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
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

                    #region Fix DestinationId and network path for standard networking connections

                    // As for an answer: an error to a request this node forwarded,
                    // under a request identification of its own (see FORWARD), goes
                    // back to whoever asked, and every other error is this node's.
                    if (jsonRequestErrorMessage.NetworkingMode   == NetworkingMode.Standard &&
                        jsonRequestErrorMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        jsonRequestErrorMessage = jsonRequestErrorMessage.ChangeNetworking(
                                                      SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(jsonRequestErrorMessage.RequestId) ?? parentNetworkingNode.Id),
                                                      jsonRequestErrorMessage.NetworkPath.Append(sourceNodeId.Value)
                                                  );
                    }

                    #endregion


                    #region OnJSONRequestErrorMessageReceived

                    var logger = OnJSONRequestErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONRequestErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Now,
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

                    // An error to a request this node forwarded goes on to whoever
                    // asked, see FORWARD; every other one is this node's own.
                    if (jsonRequestErrorMessage.Destination.Next != parentNetworkingNode.Id &&
                        jsonRequestErrorMessage.Destination.Next != NetworkingNode_Id.Zero  &&
                       !parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonRequestErrorMessage.Destination.Next))
                    {
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONRequestErrorMessage(jsonRequestErrorMessage, WebSocketConnection);
                    }

                    else
                        parentNetworkingNode.OCPP.ReceiveJSONRequestError(jsonRequestErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_JSONResponseErrorMessage.TryParse(JSONMessage, out var jsonResponseErrorMessage,                                                                        sourceNodeId))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    // A response error about an answer this node forwarded goes on
                    // to whoever answered, under the id it answered (see FORWARD);
                    // every other one is about an answer of this node's own.
                    if (jsonResponseErrorMessage.NetworkingMode   == NetworkingMode.Standard &&
                        jsonResponseErrorMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        jsonResponseErrorMessage = jsonResponseErrorMessage.ChangeNetworking(
                                                       SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetAnsweringNodeId(sourceNodeId.Value, jsonResponseErrorMessage.RequestId) ?? parentNetworkingNode.Id),
                                                       jsonResponseErrorMessage.NetworkPath.Append(sourceNodeId.Value)
                                                   );
                    }

                    #endregion


                    #region OnJSONResponseErrorMessageReceived

                    var logger = OnJSONResponseErrorMessageReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType <OnJSONResponseErrorMessageReceivedDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Now,
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

                    // About an answer this node forwarded: on to whoever answered,
                    // see FORWARD; every other one is about an answer of this node's.
                    if (jsonResponseErrorMessage.Destination.Next != parentNetworkingNode.Id &&
                        jsonResponseErrorMessage.Destination.Next != NetworkingNode_Id.Zero  &&
                       !parentNetworkingNode.OCPP.IN.AnycastIds.Contains(jsonResponseErrorMessage.Destination.Next))
                    {
                        await parentNetworkingNode.OCPP.FORWARD.ProcessJSONResponseErrorMessage(jsonResponseErrorMessage, WebSocketConnection);
                    }

                    else
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
                        switch (WebSocketConnection.Role)
                        {

                            case ConnectionRole.Client:
                                jsonSendMessage = jsonSendMessage.ChangeNetworking(
                                                       SourceRouting.To(parentNetworkingNode.Id),
                                                       jsonSendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                   );
                                break;

                            case ConnectionRole.Server:
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
                                  Now,
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
                                    sentMessageResult  = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(AddressToSender(ocppResponse.JSONRequestErrorMessage, jsonSendMessage.NetworkPath), WebSocketConnection);


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
                                          Now,
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
                                      ),
                                      WebSocketConnection
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

        #region ProcessCBORMessage   (MessageTimestamp, WebSocketConnection, CBORMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process a binary frame of a connection that agreed on a CBOR subprotocol.
        ///
        /// The frame is read as the CBOR array of an OCPP message. Its payload is not
        /// handed to the message processors yet: a request is answered at once with a
        /// request error in CBOR, so that whoever asked learns it instead of waiting out
        /// its timeout, and everything else is logged and dropped.
        /// </summary>
        /// <param name="MessageTimestamp">When the frame arrived.</param>
        /// <param name="WebSocketConnection">The connection it arrived over.</param>
        /// <param name="sourceNodeId">The networking node at the far end of that connection.</param>
        /// <param name="CBORMessage">The bytes of the frame.</param>
        /// <param name="EventTrackingId">An event tracking identification.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task ProcessCBORMessage(DateTimeOffset        MessageTimestamp,
                                             IMessageConnection    WebSocketConnection,
                                             NetworkingNode_Id?    sourceNodeId,
                                             Byte[]                CBORMessage,
                                             EventTracking_Id      EventTrackingId,
                                             CancellationToken     CancellationToken)
        {

            try
            {

                if (!OCPP_CBORFrame.TryParse(CBORMessage, out var cborArray, out var frameError))
                {
                    // Without a request identification there is nobody to tell.
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A binary frame of a CBOR connection could not be read: {frameError}");
                    return;
                }

                if      (OCPP_CBORRequestMessage.      TryParse(cborArray, out var cborRequestMessage,       out var requestParsingError,  MessageTimestamp, null, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    await parentNetworkingNode.OCPP.SendCBORRequestError(
                              new OCPP_CBORRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Unknown,
                                  SourceRouting.To(cborRequestMessage.NetworkPath.Source),
                                  NetworkPath.From(parentNetworkingNode.Id),
                                  cborRequestMessage.RequestId,
                                  ResultCode.NotImplemented,
                                  $"The OCPP message '{cborRequestMessage.Action}' is not served over CBOR yet!"
                              ),
                              WebSocketConnection
                          );

                }

                else if (OCPP_CBORResponseMessage.     TryParse(cborArray, out var cborResponseMessage,      out var responseParsingError, sourceNodeId))
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A CBOR response to request '{cborResponseMessage.RequestId}' was received, and dropped: CBOR responses are not processed yet.");

                else if (OCPP_CBORRequestErrorMessage. TryParse(cborArray, out var cborRequestErrorMessage,  out var requestErrorError,    sourceNodeId))
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A CBOR request error for request '{cborRequestErrorMessage.RequestId}' was received, and dropped: {cborRequestErrorMessage.ErrorCode} {cborRequestErrorMessage.ErrorDescription}");

                else if (OCPP_CBORResponseErrorMessage.TryParse(cborArray, out var cborResponseErrorMessage, out var responseErrorError,   sourceNodeId))
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A CBOR response error for request '{cborResponseErrorMessage.RequestId}' was received, and dropped: {cborResponseErrorMessage.ErrorCode} {cborResponseErrorMessage.ErrorDescription}");

                else if (OCPP_CBORSendMessage.         TryParse(cborArray, out var cborSendMessage,          out var sendParsingError,     MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken))
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A CBOR send message '{cborSendMessage.Action}' was received, and dropped: CBOR send messages are not processed yet.");

                else
                    DebugX.Log($"{nameof(OCPPWebSocketAdapterIN)}: A CBOR array that is no OCPP message was received: {requestParsingError ?? responseParsingError ?? requestErrorError ?? responseErrorError ?? sendParsingError ?? cborArray.ToDiagnosticString()}");

            }
            catch (Exception e)
            {
                await HandleErrors(
                          nameof(ProcessCBORMessage),
                          e
                      );
            }

        }

        #endregion

        #region ProcessBinaryMessage (MessageTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public async Task ProcessBinaryMessage(DateTimeOffset        MessageTimestamp,
                                               IMessageConnection    WebSocketConnection,
                                               NetworkingNode_Id?    sourceNodeId,
                                               Byte[]                BinaryMessage,
                                               EventTracking_Id      EventTrackingId,
                                               CancellationToken     CancellationToken)
        {

            try
            {

                SentMessageResult? sentMessageResult = null;

                //var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(OCPPAdapter.NetworkingNodeId_WebSocketKey);

                // A connection that agreed on a CBOR subprotocol has CBOR in its binary frames,
                // and nothing else: the binary format of the BinaryDataStreams extension is not
                // spoken there, and is not guessed at.
                if (WebSocketConnection.SpeaksCBOR())
                {

                    await ProcessCBORMessage(
                              MessageTimestamp,
                              WebSocketConnection,
                              sourceNodeId,
                              BinaryMessage,
                              EventTrackingId,
                              CancellationToken
                          );

                    return;

                }

                if      (OCPP_BinaryRequestMessage.      TryParse(BinaryMessage, out var binaryRequestMessage,       out var requestParsingError,  MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && binaryRequestMessage  is not null)
                {

                    #region Fix DestinationId and network path for standard networking connections

                    if (binaryRequestMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binaryRequestMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        switch (WebSocketConnection.Role)
                        {

                            case ConnectionRole.Client:
                                binaryRequestMessage = binaryRequestMessage.ChangeNetworking(
                                                           SourceRouting.To(parentNetworkingNode.Id),
                                                           binaryRequestMessage.NetworkPath.Append(sourceNodeId.Value)
                                                       );
                                break;

                            case ConnectionRole.Server:
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
                                  Now,
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

                        if (RefusalOf(WebSocketConnection, binaryRequestMessage.Action, null) is String refusal)
                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.To(binaryRequestMessage.NetworkPath.Source),
                                          NetworkPath.From(parentNetworkingNode.Id),
                                          binaryRequestMessage.RequestId,
                                          ResultCode.SecurityError,
                                          refusal,
                                          new JObject()
                                      ),
                                      WebSocketConnection
                                  );

                        else if (incomingBinaryMessageProcessorsLookup.TryGetValue(binaryRequestMessage.Action, out var methodInfo))
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
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONRequestError  (AddressToSender(ocppResponse.JSONRequestErrorMessage, binaryRequestMessage.NetworkPath), WebSocketConnection);

                                if (ocppResponse.JSONResponseMessage       is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendJSONResponse      (ocppResponse.JSONResponseMessage, WebSocketConnection);

                                if (ocppResponse.BinaryRequestErrorMessage is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, binaryRequestMessage.NetworkPath), WebSocketConnection);

                                if (ocppResponse.BinaryResponseMessage     is not null)
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryResponse    (ocppResponse.BinaryResponseMessage, WebSocketConnection);


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

                            // Back to whoever asked. Addressed to nobody, the refusal found
                            // no way out over a connection this node accepted, and the asker
                            // waited out its whole timeout.
                            await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                      new OCPP_JSONRequestErrorMessage(
                                          Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Unknown,
                                          SourceRouting.To(binaryRequestMessage.NetworkPath.Source),
                                          NetworkPath.From(parentNetworkingNode.Id),
                                          binaryRequestMessage.RequestId,
                                          ResultCode.ProtocolError,
                                          $"The OCPP message '{binaryRequestMessage.Action}' is unkown!",
                                          new JObject(
                                              new JProperty("request", BinaryMessage.ToBase64())
                                          )
                                      ),
                                      WebSocketConnection
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
                        // Whichever way the connection was made: an answer to a request
                        // this node forwarded, under a request identification of its
                        // own (see FORWARD), goes back to whoever asked, and every other
                        // answer is this node's. A local controller's CSMS answers over
                        // the controller's client connection, and there an answer to a
                        // station's request used to be taken for the controller's own.
                        binaryResponseMessage = binaryResponseMessage.ChangeNetworking(
                                                    SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(binaryResponseMessage.RequestId) ?? parentNetworkingNode.Id),
                                                    binaryResponseMessage.NetworkPath.Append(sourceNodeId.Value)
                                                );
                    }

                    #endregion


                    #region OnBinaryResponseMessageReceived

                    await LogEvent(
                              OnBinaryResponseMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
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

                else if (OCPP_BinaryRequestErrorMessage. TryParse(BinaryMessage, out var binaryRequestErrorMessage,  out var requestErrorParsingError,                               sourceNodeId))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    // As for an answer: an error to a request this node forwarded,
                    // under a request identification of its own (see FORWARD), goes
                    // back to whoever asked, and every other error is this node's.
                    if (binaryRequestErrorMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binaryRequestErrorMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        binaryRequestErrorMessage = binaryRequestErrorMessage.ChangeNetworking(
                                                        SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetForwardedNodeId(binaryRequestErrorMessage.RequestId) ?? parentNetworkingNode.Id),
                                                        binaryRequestErrorMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                    }

                    #endregion


                    #region OnBinaryRequestErrorMessageReceived

                    await LogEvent(
                              OnBinaryRequestErrorMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
                                  this,
                                  WebSocketConnection,
                                  binaryRequestErrorMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    // An error to a request this node forwarded goes on to whoever
                    // asked, see FORWARD; every other one is this node's own.
                    if (binaryRequestErrorMessage.Destination.Next != parentNetworkingNode.Id &&
                        binaryRequestErrorMessage.Destination.Next != NetworkingNode_Id.Zero  &&
                       !parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryRequestErrorMessage.Destination.Next))
                    {
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryRequestErrorMessage(binaryRequestErrorMessage, WebSocketConnection);
                    }

                    else
                        parentNetworkingNode.OCPP.ReceiveBinaryRequestError(binaryRequestErrorMessage, WebSocketConnection);

                    // No response!

                }

                else if (OCPP_BinaryResponseErrorMessage.TryParse(BinaryMessage, out var binaryResponseErrorMessage, out var responseErrorParsingError,                              sourceNodeId))
                {

                    #region Fix DestinationId and network path for standard networking connections

                    // A response error about an answer this node forwarded goes on
                    // to whoever answered, under the id it answered (see FORWARD);
                    // every other one is about an answer of this node's own.
                    if (binaryResponseErrorMessage.NetworkingMode   == NetworkingMode.Standard &&
                        binaryResponseErrorMessage.Destination.Next == NetworkingNode_Id.Zero  &&
                        sourceNodeId.HasValue)
                    {
                        binaryResponseErrorMessage = binaryResponseErrorMessage.ChangeNetworking(
                                                         SourceRouting.To(parentNetworkingNode.OCPP.FORWARD.GetAnsweringNodeId(sourceNodeId.Value, binaryResponseErrorMessage.RequestId) ?? parentNetworkingNode.Id),
                                                         binaryResponseErrorMessage.NetworkPath.Append(sourceNodeId.Value)
                                                     );
                    }

                    #endregion


                    #region OnBinaryResponseErrorMessageReceived

                    await LogEvent(
                              OnBinaryResponseErrorMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
                                  this,
                                  WebSocketConnection,
                                  binaryResponseErrorMessage,
                                  CancellationToken
                              )
                          );

                    #endregion

                    // About an answer this node forwarded: on to whoever answered,
                    // see FORWARD; every other one is about an answer of this node's.
                    if (binaryResponseErrorMessage.Destination.Next != parentNetworkingNode.Id &&
                        binaryResponseErrorMessage.Destination.Next != NetworkingNode_Id.Zero  &&
                       !parentNetworkingNode.OCPP.IN.AnycastIds.Contains(binaryResponseErrorMessage.Destination.Next))
                    {
                        await parentNetworkingNode.OCPP.FORWARD.ProcessBinaryResponseErrorMessage(binaryResponseErrorMessage, WebSocketConnection);
                    }

                    else
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
                        switch (WebSocketConnection.Role)
                        {

                            case ConnectionRole.Client:
                                binarySendMessage = binarySendMessage.ChangeNetworking(
                                                        SourceRouting.To(parentNetworkingNode.Id),
                                                        binarySendMessage.NetworkPath.Append(sourceNodeId.Value)
                                                    );
                                break;

                            case ConnectionRole.Server:
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
                                  Now,
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
                                    sentMessageResult = await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(AddressToSender(ocppResponse.BinaryRequestErrorMessage, binarySendMessage.NetworkPath), WebSocketConnection);


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
                                          Now,
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
                                      ),
                                      WebSocketConnection
                                  );

                        }

                        #endregion

                    }

                }

                // A frame that is none of the five kinds ends up here. Report every reason we
                // have, and name the kind, so a dropped message does not look like a different
                // one that failed - or like nothing at all.
                // An error frame names its kind in its first byte: report why that kind
                // could not be read, rather than why it is no SEND or no request.
                else if (BinaryMessage.Length > 0 && BinaryMessage[0] == 4 && requestErrorParsingError  is not null)
                    DebugX.Log($"Failed to parse a binary request error message within {nameof(OCPPWebSocketAdapterIN)}: '{requestErrorParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else if (BinaryMessage.Length > 0 && BinaryMessage[0] == 5 && responseErrorParsingError is not null)
                    DebugX.Log($"Failed to parse a binary response error message within {nameof(OCPPWebSocketAdapterIN)}: '{responseErrorParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

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
