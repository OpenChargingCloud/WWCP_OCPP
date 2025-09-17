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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Delegates

    #region OnAnyJSONRequestFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any JSON request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The JSON request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<RequestForwardingDecision>

        OnAnyJSONRequestFilterDelegate(DateTimeOffset            Timestamp,
                                       IEventSender              Sender,
                                       IWebSocketConnection      Connection,
                                       OCPP_JSONRequestMessage   Request,
                                       CancellationToken         CancellationToken);


    /// <summary>
    /// A delegate called whenever any JSON request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The JSON request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyJSONRequestFilteredDelegate(DateTimeOffset            Timestamp,
                                         IEventSender              Sender,
                                         IWebSocketConnection      Connection,
                                         OCPP_JSONRequestMessage   Request,
                                         RequestForwardingDecision        ForwardingDecision,
                                         CancellationToken         CancellationToken);

    #endregion

    #region OnAnyBinaryRequestFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any binary request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The binary request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<RequestForwardingDecision>

        OnAnyBinaryRequestFilterDelegate(DateTimeOffset              Timestamp,
                                         IEventSender                Sender,
                                         IWebSocketConnection        Connection,
                                         OCPP_BinaryRequestMessage   Request,
                                         CancellationToken           CancellationToken);


    /// <summary>
    /// A delegate called whenever any binary request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The binary request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyBinaryRequestFilteredDelegate(DateTimeOffset              Timestamp,
                                           IEventSender                Sender,
                                           IWebSocketConnection        Connection,
                                           OCPP_BinaryRequestMessage   Request,
                                           RequestForwardingDecision          ForwardingDecision,
                                           CancellationToken           CancellationToken);

    #endregion


    #region OnAnyJSONResponseFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any JSON response should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Response">The JSON response.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<RequestForwardingDecision>

        OnAnyJSONResponseFilterDelegate(DateTimeOffset             Timestamp,
                                        IEventSender               Sender,
                                        IWebSocketConnection       Connection,
                                        OCPP_JSONResponseMessage   Response,
                                        CancellationToken          CancellationToken);


    /// <summary>
    /// A delegate called whenever any JSON response was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Response">The JSON response.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyJSONResponseFilteredDelegate(DateTimeOffset            Timestamp,
                                         IEventSender              Sender,
                                         IWebSocketConnection      Connection,
                                         OCPP_JSONResponseMessage   Response,
                                         RequestForwardingDecision        ForwardingDecision,
                                         CancellationToken         CancellationToken);

    #endregion

    #region OnAnyBinaryResponseFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any binary response should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Response">The binary response.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<RequestForwardingDecision>

        OnAnyBinaryResponseFilterDelegate(DateTimeOffset              Timestamp,
                                         IEventSender                Sender,
                                         IWebSocketConnection        Connection,
                                         OCPP_BinaryResponseMessage   Response,
                                         CancellationToken           CancellationToken);


    /// <summary>
    /// A delegate called whenever any binary response was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Response">The binary response.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyBinaryResponseFilteredDelegate(DateTimeOffset              Timestamp,
                                           IEventSender                Sender,
                                           IWebSocketConnection        Connection,
                                           OCPP_BinaryResponseMessage   Response,
                                           RequestForwardingDecision          ForwardingDecision,
                                           CancellationToken           CancellationToken);

    #endregion



    #region OnAnyJSONSendMessageFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any JSON SendMessage should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="SendMessage">The JSON SendMessage.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<MessageForwardingDecision>

        OnAnyJSONSendMessageFilterDelegate(DateTimeOffset         Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           OCPP_JSONSendMessage   SendMessage,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A delegate called whenever any JSON SendMessage was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="SendMessage">The JSON SendMessage.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyJSONSendMessageFilteredDelegate(DateTimeOffset              Timestamp,
                                             IEventSender                Sender,
                                             IWebSocketConnection        Connection,
                                             OCPP_JSONSendMessage        SendMessage,
                                             MessageForwardingDecision   ForwardingDecision,
                                             CancellationToken           CancellationToken);

    #endregion

    #region OnAnyBinarySendMessageFilter(ed)Delegate

    /// <summary>
    /// A delegate called whenever any binary SendMessage should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="SendMessage">The binary SendMessage.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<MessageForwardingDecision>

        OnAnyBinarySendMessageFilterDelegate(DateTimeOffset           Timestamp,
                                             IEventSender             Sender,
                                             IWebSocketConnection     Connection,
                                             OCPP_BinarySendMessage   SendMessage,
                                             CancellationToken        CancellationToken);


    /// <summary>
    /// A delegate called whenever any binary SendMessage was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="SendMessage">The binary SendMessage.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyBinarySendMessageFilteredDelegate(DateTimeOffset              Timestamp,
                                               IEventSender                Sender,
                                               IWebSocketConnection        Connection,
                                               OCPP_BinarySendMessage      SendMessage,
                                               MessageForwardingDecision   ForwardingDecision,
                                               CancellationToken           CancellationToken);

    #endregion

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Data

        private   readonly  INetworkingNode                                 parentNetworkingNode;

        private   readonly  HashSet<SignaturePolicy>                        forwardingSignaturePolicies         = [];

        protected readonly  Dictionary<String, MethodInfo>                  forwardingMessageProcessorsLookup   = [];

        protected readonly  ConcurrentDictionary<Request_Id, ResponseInfo>  expectedResponses                   = [];

        #endregion

        #region Properties

        public ForwardingDecisions         DefaultForwardingDecision    { get; set; } = ForwardingDecisions.DROP;

        public HashSet<NetworkingNode_Id>  AnycastIdsAllowed            { get; }      = [];

        public HashSet<NetworkingNode_Id>  AnycastIdsDenied             { get; }      = [];

        /// <summary>
        /// Whether to forward responses, that have no 'known requests ids' entry.
        /// This might occur in high-available systems, when the request was not forwarded by this node.
        /// </summary>
        public Boolean                     ForwardUnknownResponses      { get; set; }


        #region ForwardingSignaturePolicy/-ies

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  ForwardingSignaturePolicies
            => forwardingSignaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               ForwardingSignaturePolicy
            => forwardingSignaturePolicies.First();

        #endregion

        #endregion

        #region Events

        public event OnAnyJSONRequestFilterDelegate?          OnAnyJSONRequestFilter;
        public event OnAnyJSONRequestFilteredDelegate?        OnAnyJSONRequestFiltered;

        public event OnAnyBinaryRequestFilterDelegate?        OnAnyBinaryRequestFilter;
        public event OnAnyBinaryRequestFilteredDelegate?      OnAnyBinaryRequestFiltered;


        public event OnAnyJSONResponseFilterDelegate?         OnAnyJSONResponseFilter;
        public event OnAnyJSONResponseFilteredDelegate?       OnAnyJSONResponseFiltered;

        public event OnAnyBinaryResponseFilterDelegate?       OnAnyBinaryResponseFilter;
        public event OnAnyBinaryResponseFilteredDelegate?     OnAnyBinaryResponseFiltered;


        public event OnAnyJSONSendMessageFilterDelegate?      OnAnyJSONSendMessageFilter;
        public event OnAnyJSONSendMessageFilteredDelegate?    OnAnyJSONSendMessageFiltered;

        public event OnAnyBinarySendMessageFilterDelegate?    OnAnyBinarySendMessageFilter;
        public event OnAnyBinarySendMessageFilteredDelegate?  OnAnyBinarySendMessageFiltered;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for forwarding messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// <param name="ForwardingSignaturePolicy">An optional signature policy when forwarding OCPP messages.</param>
        /// <param name="DefaultForwardingDecision">The default forwarding decision.</param>
        /// <param name="ForwardUnknownResponses">Whether to forward responses, that have no 'known requests ids' entry.</param>
        public OCPPWebSocketAdapterFORWARD(INetworkingNode       NetworkingNode,
                                           SignaturePolicy?      ForwardingSignaturePolicy   = null,
                                           ForwardingDecisions?  DefaultForwardingDecision   = ForwardingDecisions.DROP,
                                           Boolean               ForwardUnknownResponses     = false)
        {

            this.parentNetworkingNode       = NetworkingNode;
            this.DefaultForwardingDecision  = DefaultForwardingDecision    ?? ForwardingDecisions.DROP;
            this.ForwardUnknownResponses    = ForwardUnknownResponses;

            this.forwardingSignaturePolicies.Add(ForwardingSignaturePolicy ?? new SignaturePolicy());

            #region Reflect "Forward_XXX" messages and wire them...

            foreach (var methodInfo in typeof(OCPPWebSocketAdapterFORWARD).
                                           GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                                Where(method => method.Name.StartsWith("Forward_")))
            {

                var processorName = methodInfo.Name[8..];

                if (forwardingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                var parameterInfos = methodInfo.GetParameters();

                if (parameterInfos.Length == 4)
                    forwardingMessageProcessorsLookup.Add(
                        processorName,
                        methodInfo
                    );

            }

            #endregion

        }

        #endregion


        public NetworkingNode_Id? GetForwardedNodeId(Request_Id RequestId)
        {

            if (expectedResponses.TryGetValue(RequestId, out var responseInfo))
                return responseInfo.SourceNodeId;

            return null;

        }


        #region ProcessJSONRequestMessage         (JSONRequestMessage,         WebSocketConnection)

        public async Task ProcessJSONRequestMessage(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                    IWebSocketConnection     WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(JSONRequestMessage.Destination.Next))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(JSONRequestMessage.Destination.Next))
                return;


            #region Do we have a general filter rule for any JSON request?

            RequestForwardingDecision? forwardingDecision = null;

            var requestFilter = OnAnyJSONRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnAnyJSONRequestFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             JSONRequestMessage,
                                                                             JSONRequestMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OnAnyJSONRequestFilter),
                              e
                          );
                }
            }

            #endregion

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingDecisions.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(JSONRequestMessage.Action, out var methodInfo))
                {

                    var resultTask = methodInfo.Invoke(
                                         this,
                                         [
                                             JSONRequestMessage,
                                             null,
                                             WebSocketConnection,
                                             JSONRequestMessage.CancellationToken
                                         ]
                                     );

                    if (resultTask is Task<RequestForwardingDecision> forwardingDecisionTask)
                        forwardingDecision = await forwardingDecisionTask;

                    else
                        DebugX.Log($"Invalid result type for a '{JSONRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {

                    DebugX.Log($"Undefined '{JSONRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                              new OCPP_JSONRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Unknown,
                                  SourceRouting.Zero,
                                  NetworkPath.Empty,
                                  JSONRequestMessage.RequestId,
                                  ResultCode.ProtocolError,
                                  $"Received unknown OCPP '{JSONRequestMessage.Action}' JSON request message!",
                                  new JObject(
                                      new JProperty("request", JSONRequestMessage.ToJSON())
                                  )
                              )
                          );

                }

                #endregion

            }


            forwardingDecision ??= new RequestForwardingDecision(DefaultForwardingDecision);


            #region Send OnAnyJSONRequestFiltered event

            await LogEvent(
                      OnAnyJSONRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          JSONRequestMessage,
                          forwardingDecision,
                          JSONRequestMessage.CancellationToken
                      )
                  );

            #endregion


            #region FORWARD

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var newJSONRequestMessage = JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestination is not null)
                    newJSONRequestMessage = newJSONRequestMessage.ChangeNetworking(forwardingDecision.NewDestination);

                expectedResponses.TryAdd(
                    newJSONRequestMessage.RequestId,
                    new ResponseInfo(
                        newJSONRequestMessage.RequestId,
                        forwardingDecision.   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        newJSONRequestMessage.NetworkPath.Source,
                        newJSONRequestMessage.RequestTimeout
                    )
                );

                await parentNetworkingNode.OCPP.OUT.SendJSONRequest(
                          newJSONRequestMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REPLACE

            if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
            {

                var newJSONRequestMessage = forwardingDecision.NewJSONRequest is null
                                                ? JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                                : new OCPP_JSONRequestMessage(
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.EventTrackingId,
                                                      JSONRequestMessage.NetworkingMode,
                                                      forwardingDecision.NewDestination ?? JSONRequestMessage.Destination,
                                                      JSONRequestMessage.NetworkPath.Append(parentNetworkingNode.Id),
                                                      JSONRequestMessage.RequestId,
                                                      forwardingDecision.NewAction        ?? JSONRequestMessage.Action,
                                                      forwardingDecision.NewJSONRequest, // <-- !!!
                                                      JSONRequestMessage.RequestTimeout,
                                                      JSONRequestMessage.ErrorMessage,
                                                      JSONRequestMessage.CancellationToken
                                                  );

                expectedResponses.TryAdd(
                    newJSONRequestMessage.RequestId,
                    new ResponseInfo(
                        newJSONRequestMessage.RequestId,
                        forwardingDecision.   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        newJSONRequestMessage.NetworkPath.Source,
                        newJSONRequestMessage.RequestTimeout
                    )
                );

                await parentNetworkingNode.OCPP.OUT.SendJSONRequest(
                          newJSONRequestMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REJECT

            else if (forwardingDecision.Result == ForwardingDecisions.REJECT &&
                     forwardingDecision.JSONRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                          new OCPP_JSONRequestErrorMessage(
                              Timestamp.Now,
                              JSONRequestMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              SourceRouting.To  (JSONRequestMessage.NetworkPath.Source),
                              NetworkPath.  From(parentNetworkingNode.Id),
                              JSONRequestMessage.RequestId,
                              ResultCode.Filtered,
                              forwardingDecision.RejectMessage,
                              forwardingDecision.RejectDetails,
                              JSONRequestMessage.CancellationToken
                          )
                      );

            }

            #endregion

            #region DROP

            else
            {
                // Just ignore the request!
            }

            #endregion

        }

        #endregion

        #region ProcessJSONResponseMessage        (JSONResponseMessage,        WebSocketConnection)

        public async Task ProcessJSONResponseMessage(OCPP_JSONResponseMessage  JSONResponseMessage,
                                                     IWebSocketConnection      WebSocketConnection)
        {

            #region Do we have a general filter rule for any JSON response?

            RequestForwardingDecision? forwardingDecision = null;

            var responseFilter = OnAnyJSONResponseFilter;
            if (responseFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            responseFilter.GetInvocationList().
                                                OfType<OnAnyJSONResponseFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             JSONResponseMessage,
                                                                             JSONResponseMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OnAnyJSONResponseFilter),
                              e
                          );
                }
            }

            #endregion


            if (forwardingDecision is null)
            {

                if (expectedResponses.TryRemove(JSONResponseMessage.RequestId, out var responseInfo))
                {

                    if (responseInfo.Timeout >= Timestamp.Now)
                    {

                        await parentNetworkingNode.OCPP.OUT.SendJSONResponse(
                                  JSONResponseMessage.ChangeNetworking(
                                      JSONResponseMessage.Destination.Next == NetworkingNode_Id.Zero
                                          ? SourceRouting.To(responseInfo.SourceNodeId)
                                          : JSONResponseMessage.Destination,
                                      JSONResponseMessage.NetworkPath.Append(parentNetworkingNode.Id)
                                  )
                              );

                    }
                    else
                        await HandleErrors(
                            nameof(ProcessJSONResponseMessage),
                            $"Received a too late JSON response message for request id '{JSONResponseMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!"
                        );

                }
                else if (ForwardUnknownResponses)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONResponse(
                              JSONResponseMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                          );

                }
                else
                    await HandleErrors(
                        nameof(ProcessJSONResponseMessage),
                        $"Received a JSON response message for an unknown request id '{JSONResponseMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!"
                    );

            }

            #region FORWARD

            if (forwardingDecision?.Result == ForwardingDecisions.FORWARD)
            {



            }

            #endregion

        }

        #endregion

        #region ProcessJSONRequestErrorMessage    (JSONRequestErrorMessage,    WebSocketConnection)

        public async Task ProcessJSONRequestErrorMessage(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                         IWebSocketConnection          WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONRequestErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(JSONRequestErrorMessage);

                }
                else
                    DebugX.Log($"Received a too late JSON request error message for request id '{JSONRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a JSON request error message for an unknown request id '{JSONRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessJSONResponseErrorMessage   (JSONResponseErrorMessage,   WebSocketConnection)

        public async Task ProcessJSONResponseErrorMessage(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                          IWebSocketConnection           WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONResponseErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONResponseError(JSONResponseErrorMessage);

                }
                else
                    DebugX.Log($"Received a too late JSON response error message for request id '{JSONResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a JSON response error message for an unknown request id '{JSONResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessJSONSendMessage            (JSONSendMessage,            WebSocketConnection)

        public async Task ProcessJSONSendMessage(OCPP_JSONSendMessage  JSONSendMessage,
                                                 IWebSocketConnection  WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(JSONSendMessage.Destination.Next))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(JSONSendMessage.Destination.Next))
                return;


            #region Do we have a general filter rule for any JSON request?

            MessageForwardingDecision? forwardingDecision = null;

            var requestFilter = OnAnyJSONSendMessageFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnAnyJSONSendMessageFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             JSONSendMessage,
                                                                             JSONSendMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OnAnyJSONSendMessageFilter),
                              e
                          );
                }
            }

            #endregion

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingDecisions.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(JSONSendMessage.Action, out var methodInfo))
                {

                    var resultTask = methodInfo.Invoke(
                                         this,
                                         [
                                             JSONSendMessage,
                                             null,
                                             WebSocketConnection,
                                             JSONSendMessage.CancellationToken
                                         ]
                                     );

                    if (resultTask is Task<MessageForwardingDecision> forwardingDecisionTask)
                        forwardingDecision = await forwardingDecisionTask;

                    else
                        DebugX.Log($"Received undefined '{JSONSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {

                    DebugX.Log($"Undefined '{JSONSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                              new OCPP_JSONRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Unknown,
                                  SourceRouting.Zero,
                                  NetworkPath.Empty,
                                  JSONSendMessage.MessageId,
                                  ResultCode.ProtocolError,
                                  $"Received unknown OCPP '{JSONSendMessage.Action}' JSON send message!",
                                  new JObject(
                                      new JProperty("request", JSONSendMessage.ToJSON())
                                  )
                              )
                          );

                }

                #endregion

            }


            forwardingDecision ??= new MessageForwardingDecision(DefaultForwardingDecision);


            #region Send OnAnyJSONSendMessageFiltered event

            await LogEvent(
                      OnAnyJSONSendMessageFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          JSONSendMessage,
                          forwardingDecision,
                          JSONSendMessage.CancellationToken
                      )
                  );

            #endregion


            #region FORWARD

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var newJSONSendMessage = JSONSendMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestination is not null)
                    newJSONSendMessage = newJSONSendMessage.ChangeNetworking(forwardingDecision.NewDestination);

                await parentNetworkingNode.OCPP.OUT.SendJSONSendMessage(
                          newJSONSendMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REPLACE

            if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
            {

                var newJSONSendMessage = forwardingDecision.NewJSONRequest is null
                                             ? JSONSendMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                             : new OCPP_JSONSendMessage(
                                                   JSONSendMessage.MessageTimestamp,
                                                   JSONSendMessage.EventTrackingId,
                                                   JSONSendMessage.NetworkingMode,
                                                   forwardingDecision.NewDestination ?? JSONSendMessage.Destination,
                                                   JSONSendMessage.NetworkPath.Append(parentNetworkingNode.Id),
                                                   JSONSendMessage.MessageId,
                                                   forwardingDecision.NewAction        ?? JSONSendMessage.Action,
                                                   forwardingDecision.NewJSONRequest, // <-- !!!
                                                   //JSONSendMessage.SendTimeout,
                                                   JSONSendMessage.ErrorMessage,
                                                   JSONSendMessage.CancellationToken
                                               );

                await parentNetworkingNode.OCPP.OUT.SendJSONSendMessage(
                          newJSONSendMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REJECT

            else if (forwardingDecision.Result == ForwardingDecisions.REJECT &&
                     forwardingDecision.JSONRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                          new OCPP_JSONRequestErrorMessage(
                              Timestamp.Now,
                              JSONSendMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              SourceRouting.To  (JSONSendMessage.NetworkPath.Source),
                              NetworkPath.  From(parentNetworkingNode.Id),
                              JSONSendMessage.MessageId,
                              ResultCode.Filtered,
                              forwardingDecision.RejectMessage,
                              forwardingDecision.RejectDetails,
                              JSONSendMessage.CancellationToken
                          )
                      );

            }

            #endregion

            #region DROP

            else
            {
                // Just ignore the request!
            }

            #endregion

        }

        #endregion


        #region ProcessBinaryRequestMessage       (BinaryRequestMessage,       WebSocketConnection)

        public async Task ProcessBinaryRequestMessage(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                      IWebSocketConnection       WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(BinaryRequestMessage.Destination.Next))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(BinaryRequestMessage.Destination.Next))
                return;


            #region Do we have a general filter rule for any Binary request?

            RequestForwardingDecision? forwardingDecision = null;

            var requestFilter = OnAnyBinaryRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnAnyBinaryRequestFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             BinaryRequestMessage,
                                                                             BinaryRequestMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OnAnyBinaryRequestFilter),
                              e
                          );
                }
            }

            #endregion

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingDecisions.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(BinaryRequestMessage.Action, out var methodInfo))
                {

                    var resultTask = methodInfo.Invoke(
                                         this,
                                         [
                                             null,
                                             BinaryRequestMessage,
                                             WebSocketConnection,
                                             BinaryRequestMessage.CancellationToken
                                         ]
                                     );

                    if (resultTask is Task<RequestForwardingDecision> forwardingDecisionTask)
                        forwardingDecision = await forwardingDecisionTask;

                    else
                        DebugX.Log($"Invalid result type for a '{BinaryRequestMessage.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {

                    DebugX.Log($"Undefined '{BinaryRequestMessage.Action}' binary request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                              new OCPP_BinaryRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Unknown,
                                  SourceRouting.Zero,
                                  NetworkPath.Empty,
                                  BinaryRequestMessage.RequestId,
                                  ResultCode.ProtocolError,
                                  $"Received unknown OCPP '{BinaryRequestMessage.Action}' binary request message!",
                                  new JObject(
                                      new JProperty("request", BinaryRequestMessage.ToByteArray().ToBase64())
                                  )
                              )
                          );

                }

                #endregion

            }


            forwardingDecision ??= new RequestForwardingDecision(DefaultForwardingDecision);


            #region Send OnAnyBinaryRequestFiltered event

            await LogEvent(
                      OnAnyBinaryRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          BinaryRequestMessage,
                          forwardingDecision,
                          BinaryRequestMessage.CancellationToken
                      )
                  );

            #endregion


            #region FORWARD

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var newBinaryRequestMessage = BinaryRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestination is not null)
                    newBinaryRequestMessage = newBinaryRequestMessage.ChangeNetworking(forwardingDecision.NewDestination);

                expectedResponses.TryAdd(
                    newBinaryRequestMessage.RequestId,
                    new ResponseInfo(
                        newBinaryRequestMessage.RequestId,
                        forwardingDecision.     RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        newBinaryRequestMessage.NetworkPath.Source,
                        newBinaryRequestMessage.RequestTimeout
                    )
                );

                await parentNetworkingNode.OCPP.OUT.SendBinaryRequest(
                          newBinaryRequestMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REPLACE

            if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
            {

                var newBinaryRequestMessage = forwardingDecision.NewBinaryRequest is null
                                                ? BinaryRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                                : new OCPP_BinaryRequestMessage(
                                                      BinaryRequestMessage.RequestTimestamp,
                                                      BinaryRequestMessage.EventTrackingId,
                                                      BinaryRequestMessage.NetworkingMode,
                                                      forwardingDecision.NewDestination ?? BinaryRequestMessage.Destination,
                                                      BinaryRequestMessage.NetworkPath.Append(parentNetworkingNode.Id),
                                                      BinaryRequestMessage.RequestId,
                                                      forwardingDecision.NewAction        ?? BinaryRequestMessage.Action,
                                                      forwardingDecision.NewBinaryRequest, // <-- !!!
                                                      BinaryRequestMessage.RequestTimeout,
                                                      BinaryRequestMessage.ErrorMessage,
                                                      BinaryRequestMessage.CancellationToken
                                                  );

                expectedResponses.TryAdd(
                    newBinaryRequestMessage.RequestId,
                    new ResponseInfo(
                        newBinaryRequestMessage.RequestId,
                        forwardingDecision.     RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        newBinaryRequestMessage.NetworkPath.Source,
                        newBinaryRequestMessage.RequestTimeout
                    )
                );

                await parentNetworkingNode.OCPP.OUT.SendBinaryRequest(
                          newBinaryRequestMessage
                      );

            }

            #endregion

            #region REJECT

            else if (forwardingDecision.Result == ForwardingDecisions.REJECT &&
                     forwardingDecision.BinaryRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                          new OCPP_BinaryRequestErrorMessage(
                              Timestamp.Now,
                              BinaryRequestMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              SourceRouting.To(BinaryRequestMessage.NetworkPath.Source),
                              NetworkPath.From(parentNetworkingNode.Id),
                              BinaryRequestMessage.RequestId,
                              ResultCode.Filtered,
                              forwardingDecision.RejectMessage,
                              forwardingDecision.RejectDetails,
                              BinaryRequestMessage.CancellationToken
                          )
                      );

            }

            #endregion

            #region DROP

            else
            {
                // Just ignore the request!
            }

            #endregion

        }

        #endregion

        #region ProcessBinaryResponseMessage      (BinaryResponseMessage,      WebSocketConnection)

        public async Task ProcessBinaryResponseMessage(OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                                       IWebSocketConnection        WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryResponseMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout >= Timestamp.Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponse(
                              BinaryResponseMessage.ChangeNetworking(
                                  BinaryResponseMessage.Destination.Next == NetworkingNode_Id.Zero
                                      ? SourceRouting.To(responseInfo.SourceNodeId)
                                      : BinaryResponseMessage.Destination,
                                  BinaryResponseMessage.NetworkPath.Append(parentNetworkingNode.Id)
                              )
                          );

                }
                else
                    DebugX.Log($"Received a too late binary response message for request id '{BinaryResponseMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a binary response message for an unknown request id '{BinaryResponseMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessBinaryRequestErrorMessage  (BinaryRequestErrorMessage,  WebSocketConnection)

        public async Task ProcessBinaryRequestErrorMessage(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                           IWebSocketConnection            WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryRequestErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(BinaryRequestErrorMessage);

                }
                else
                    DebugX.Log($"Received a too late binary request error message for request id '{BinaryRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a binary request error message for an unknown request id '{BinaryRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessBinaryResponseErrorMessage (BinaryResponseErrorMessage, WebSocketConnection)

        public async Task ProcessBinaryResponseErrorMessage(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                            IWebSocketConnection             WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryResponseErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponseError(BinaryResponseErrorMessage);

                }
                else
                    DebugX.Log($"Received a too late binary response error message for request id '{BinaryResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a binary response error message for an unknown request id '{BinaryResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessBinarySendMessage          (BinarySendMessage,          WebSocketConnection)

        public async Task ProcessBinarySendMessage(OCPP_BinarySendMessage  BinarySendMessage,
                                                   IWebSocketConnection    WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(BinarySendMessage.Destination.Next))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(BinarySendMessage.Destination.Next))
                return;


            #region Do we have a general filter rule for any Binary request?

            MessageForwardingDecision? forwardingDecision = null;

            var requestFilter = OnAnyBinarySendMessageFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnAnyBinarySendMessageFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             BinarySendMessage,
                                                                             BinarySendMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OnAnyBinarySendMessageFilter),
                              e
                          );
                }
            }

            #endregion

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingDecisions.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(BinarySendMessage.Action, out var methodInfo))
                {

                    var resultTask = methodInfo.Invoke(
                                         this,
                                         [
                                             null,
                                             BinarySendMessage,
                                             WebSocketConnection,
                                             BinarySendMessage.CancellationToken
                                         ]
                                     );

                    if (resultTask is Task<MessageForwardingDecision> forwardingDecisionTask)
                        forwardingDecision = await forwardingDecisionTask;

                    else
                        DebugX.Log($"Invalid result type for a '{BinarySendMessage.Action}' binary send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {

                    DebugX.Log($"Undefined '{BinarySendMessage.Action}' binary send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                              new OCPP_BinaryRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Unknown,
                                  SourceRouting.Zero,
                                  NetworkPath.Empty,
                                  BinarySendMessage.MessageId,
                                  ResultCode.ProtocolError,
                                  $"Received unknown OCPP '{BinarySendMessage.Action}' binary send message!",
                                  new JObject(
                                      new JProperty("request", BinarySendMessage.ToByteArray().ToBase64())
                                  )
                              )
                          );

                }

                #endregion

            }


            forwardingDecision ??= new MessageForwardingDecision(DefaultForwardingDecision);


            #region Send OnAnyBinarySendMessageFiltered event

            await LogEvent(
                      OnAnyBinarySendMessageFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          BinarySendMessage,
                          forwardingDecision,
                          BinarySendMessage.CancellationToken
                      )
                  );

            #endregion


            #region FORWARD

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var newBinarySendMessage = BinarySendMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestination is not null)
                    newBinarySendMessage = newBinarySendMessage.ChangeNetworking(forwardingDecision.NewDestination);

                await parentNetworkingNode.OCPP.OUT.SendBinarySendMessage(
                          newBinarySendMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REPLACE

            if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
            {

                var newBinarySendMessage = forwardingDecision.NewJSONRequest is null
                                               ? BinarySendMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                               : new OCPP_BinarySendMessage(
                                                     BinarySendMessage.MessageTimestamp,
                                                     BinarySendMessage.EventTrackingId,
                                                     BinarySendMessage.NetworkingMode,
                                                     forwardingDecision.NewDestination ?? BinarySendMessage.Destination,
                                                     BinarySendMessage.NetworkPath.Append(parentNetworkingNode.Id),
                                                     BinarySendMessage.MessageId,
                                                     forwardingDecision.NewAction        ?? BinarySendMessage.Action,
                                                     forwardingDecision.NewBinaryRequest, // <-- !!!
                                                     //BinarySendMessage.SendTimeout,
                                                     BinarySendMessage.ErrorMessage,
                                                     BinarySendMessage.CancellationToken
                                                 );

                await parentNetworkingNode.OCPP.OUT.SendBinarySendMessage(
                          newBinarySendMessage,
                          forwardingDecision.SentMessageLogger
                      );

            }

            #endregion

            #region REJECT

            else if (forwardingDecision.Result == ForwardingDecisions.REJECT &&
                     forwardingDecision.BinaryRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                          new OCPP_BinaryRequestErrorMessage(
                              Timestamp.Now,
                              BinarySendMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              SourceRouting.To(BinarySendMessage.NetworkPath.Source),
                              NetworkPath.From(parentNetworkingNode.Id),
                              BinarySendMessage.MessageId,
                              ResultCode.Filtered,
                              forwardingDecision.RejectMessage,
                              forwardingDecision.RejectDetails,
                              BinarySendMessage.CancellationToken
                          )
                      );

            }

            #endregion

            #region DROP

            else
            {
                // Just ignore the request!
            }

            #endregion

        }

        #endregion



        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("DefaultForwardingDecision",   DefaultForwardingDecision.ToString()),
                           new JProperty("anycastIdsAllowed",           new JArray(AnycastIdsAllowed.          Select(networkingNodeId => networkingNodeId.ToString()))),
                           new JProperty("anycastIdsDenied",            new JArray(AnycastIdsDenied.           Select(networkingNodeId => networkingNodeId.ToString()))),

                           new JProperty("signaturePolicies",           new JArray(ForwardingSignaturePolicies.Select(signaturePolicy  => signaturePolicy. ToJSON())))

                       );

            return json;

        }



        #region (private) LogEvent     (Logger, LogHandler,    ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentNetworkingNode.LogEvent(nameof(OCPPWebSocketAdapterIN), Logger, LogHandler, EventName, OCPPCommand);

        #endregion

        #region (private) CallFilter   (Filter, FilterHandler, ...)

        private async Task<T?> CallFilter<TDelegate, T>(TDelegate?                                         Filter,
                                                        Func<TDelegate, Task<T>>                           FilterHandler,
                                                        [CallerArgumentExpression(nameof(Filter))] String  EventName     = "",
                                                        [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

        {

            if (Filter is not null)
            {
                try
                {

                    var handler = Filter.GetInvocationList().OfType<TDelegate>().FirstOrDefault();

                    if (handler is not null)
                        return await FilterHandler(handler);

                }
                catch (Exception e)
                {
                    await HandleErrors($"{OCPPCommand}.{EventName}", e);
                }
            }

            return default;

        }

        #endregion

        #region (private) HandleErrors (Caller, ErrorOccurred)

        private Task HandleErrors(String  Caller,
                                  String  ErrorOccurred)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterFORWARD),
                   Caller,
                   ErrorOccurred
               );

        #endregion

        #region (private) HandleErrors (Caller, ExceptionOccurred)
        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccurred)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterFORWARD),
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
