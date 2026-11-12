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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

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
                                       IMessageConnection        Connection,
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
                                         IMessageConnection        Connection,
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
                                         IMessageConnection          Connection,
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
                                           IMessageConnection          Connection,
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
                                        IMessageConnection         Connection,
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
                                         IMessageConnection        Connection,
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
                                         IMessageConnection          Connection,
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
                                           IMessageConnection          Connection,
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
                                           IMessageConnection     Connection,
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
                                             IMessageConnection          Connection,
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
                                             IMessageConnection       Connection,
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
                                               IMessageConnection          Connection,
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

        /// <summary>
        /// An answer this node forwarded: the request id it came under, the
        /// networking node that sent it, and until when it is remembered.
        /// </summary>
        protected readonly record struct ForwardedResponse(Request_Id         RequestId,
                                                           NetworkingNode_Id  AnsweredBy,
                                                           DateTimeOffset     KeptUntil);

        /// <summary>
        /// The answers this node forwarded, by the networking node each went to and
        /// the request id it went under: where a response error about one goes on to.
        /// </summary>
        protected readonly  ConcurrentDictionary<(NetworkingNode_Id SentTo, Request_Id SentUnder), ForwardedResponse>  forwardedResponses  = [];

        private             DateTimeOffset                                  forwardedResponsesCleanedUp;

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

            this.forwardingSignaturePolicies.Add((ForwardingSignaturePolicy ?? new SignaturePolicy()).BindTo(NetworkingNode.Id, cloud.charging.open.protocols.OCPPv1_6.Version.WebSocketSubProtocolId));

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


        #region (private) ForwardUnderAnIdOfItsOwn(RequestId, RequestContext, SourceNodeId, Timeout)

        /// <summary>
        /// The request identification a request this node forwards is sent on under:
        /// one of this node's own, remembered with the identification the request came
        /// with and the networking node that sent it.
        /// </summary>
        /// <remarks>
        /// Every networking node numbers its requests from the same start, so the
        /// stations behind a local controller ask with the same identifications as
        /// each other, and as the controller itself, as often as not. Sent on under
        /// theirs, two answers with one identification would come back over the same
        /// connection, and in standard networking mode nothing in them says whose
        /// each is. A random identification, which is never one of the numbers a node
        /// gives its own requests, is unique here; the answer goes back under the
        /// identification the request came with.
        /// </remarks>
        /// <param name="RequestId">The request identification the request came with.</param>
        /// <param name="RequestContext">The JSON-LD context of the request, where the forwarding decision says one.</param>
        /// <param name="SourceNodeId">The networking node that sent the request.</param>
        /// <param name="Timeout">When an answer is no longer waited for.</param>
        private Request_Id ForwardUnderAnIdOfItsOwn(Request_Id         RequestId,
                                                    JSONLDContext?     RequestContext,
                                                    NetworkingNode_Id  SourceNodeId,
                                                    DateTimeOffset     Timeout)
        {

            var responseInfo = new ResponseInfo(
                                   RequestId,
                                   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                                   SourceNodeId,
                                   Timeout
                               );

            while (true)
            {

                var forwardedRequestId = Request_Id.NewRandom();

                if (expectedResponses.TryAdd(forwardedRequestId, responseInfo))
                    return forwardedRequestId;

            }

        }

        #endregion

        public NetworkingNode_Id? GetForwardedNodeId(Request_Id RequestId)
        {

            if (expectedResponses.TryGetValue(RequestId, out var responseInfo))
                return responseInfo.SourceNodeId;

            return null;

        }

        #region GetAnsweringNodeId(SentTo, RequestId)

        /// <summary>
        /// Whoever sent the answer this node forwarded to the given networking node
        /// under the given request id, while it is remembered: where a response error
        /// about that answer goes on to.
        /// </summary>
        /// <param name="SentTo">The networking node this node sent the answer on to.</param>
        /// <param name="RequestId">The request id this node sent the answer on under.</param>
        public NetworkingNode_Id? GetAnsweringNodeId(NetworkingNode_Id  SentTo,
                                                     Request_Id         RequestId)

            => forwardedResponses.TryGetValue((SentTo, RequestId), out var forwardedResponse) &&
               forwardedResponse.KeptUntil >= Now

                   ? forwardedResponse.AnsweredBy
                   : null;

        #endregion

        #region (private) RememberForwardedResponse(SentTo, SentUnder, AnsweredBy, AnsweredUnder)

        /// <summary>
        /// Remember an answer this node forwards, for as long as this node waits for an
        /// answer to a request of its own: a response error about it, which comes from
        /// whoever asked under the request id this node sends the answer on under, then
        /// finds its way to whoever answered.
        /// </summary>
        /// <param name="SentTo">The networking node this node sends the answer on to.</param>
        /// <param name="SentUnder">The request id this node sends the answer on under.</param>
        /// <param name="AnsweredBy">The networking node the answer came from.</param>
        /// <param name="AnsweredUnder">The request id the answer came under.</param>
        private void RememberForwardedResponse(NetworkingNode_Id  SentTo,
                                               Request_Id         SentUnder,
                                               NetworkingNode_Id  AnsweredBy,
                                               Request_Id         AnsweredUnder)
        {

            if (AnsweredBy == NetworkingNode_Id.Zero)
                return;

            var now      = Now;
            var keepFor  = parentNetworkingNode.OCPP.DefaultRequestTimeout;

            // Most answers draw no response error: what is no longer
            // remembered goes now and then, rather than piling up.
            if (now >= forwardedResponsesCleanedUp + keepFor)
            {

                foreach (var remembered in forwardedResponses)
                {
                    if (remembered.Value.KeptUntil < now)
                        forwardedResponses.TryRemove(remembered);
                }

                forwardedResponsesCleanedUp = now;

            }

            forwardedResponses[(SentTo, SentUnder)] = new ForwardedResponse(
                                                          AnsweredUnder,
                                                          AnsweredBy,
                                                          now + keepFor
                                                      );

        }

        #endregion

        #region (private) TryTakeForwardedResponse(SentTo, SentUnder, out Forwarded)

        /// <summary>
        /// Take the answer this node forwarded to the given networking node under the
        /// given request id: one response error about it is all there is.
        /// </summary>
        /// <param name="SentTo">The networking node this node sent the answer on to.</param>
        /// <param name="SentUnder">The request id this node sent the answer on under.</param>
        /// <param name="Forwarded">The answer, while it was still remembered.</param>
        private Boolean TryTakeForwardedResponse(NetworkingNode_Id      SentTo,
                                                 Request_Id             SentUnder,
                                                 out ForwardedResponse  Forwarded)

            => forwardedResponses.TryRemove((SentTo, SentUnder), out Forwarded) &&
               Forwarded.KeptUntil >= Now;

        #endregion


        #region ProcessJSONRequestMessage         (JSONRequestMessage,         WebSocketConnection)

        public async Task ProcessJSONRequestMessage(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                    IMessageConnection       WebSocketConnection)
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
                                                                             Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             JSONRequestMessage,
                                                                             JSONRequestMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = WithItsRejectResponseGiven(results.First());

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
                                  Now,
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
                          Now,
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

                // Sent on under a request identification of its own: see
                // ForwardUnderAnIdOfItsOwn.
                newJSONRequestMessage = newJSONRequestMessage.ChangeRequestId(
                                            ForwardUnderAnIdOfItsOwn(
                                                newJSONRequestMessage.RequestId,
                                                forwardingDecision.RequestContext,
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

            // One decision, one branch: FORWARD, REPLACE, REJECT and DROP exclude each other.
            else if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
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

                // Sent on under a request identification of its own: see
                // ForwardUnderAnIdOfItsOwn.
                newJSONRequestMessage = newJSONRequestMessage.ChangeRequestId(
                                            ForwardUnderAnIdOfItsOwn(
                                                newJSONRequestMessage.RequestId,
                                                forwardingDecision.RequestContext,
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

            // With a reject response or without one: the asker is told. Without one - a
            // filter's plain REJECT, or a request this node could not read - it used to be
            // dropped without a word, and the asker waited out its whole timeout.
            else if (forwardingDecision.Result == ForwardingDecisions.REJECT)
            {

                // What a filter gave as the answer is the answer, under the request's
                // identification. It was never sent: every REJECT went as a request error,
                // and the response the filter had made was thrown away.
                if (forwardingDecision.RejectResponseGiven &&
                    forwardingDecision.JSONRejectResponse is JObject rejectResponse)
                    await parentNetworkingNode.OCPP.OUT.SendJSONResponse(
                              new OCPP_JSONResponseMessage(
                                  Now,
                                  JSONRequestMessage.EventTrackingId,
                                  NetworkingMode.Unknown,
                                  SourceRouting.To  (JSONRequestMessage.NetworkPath.Source),
                                  NetworkPath.  From(parentNetworkingNode.Id),
                                  JSONRequestMessage.RequestId,
                                  rejectResponse,
                                  JSONRequestMessage.CancellationToken
                              )
                          );

                else
                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                              new OCPP_JSONRequestErrorMessage(
                                  Now,
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
                                                     IMessageConnection        WebSocketConnection)
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
                                                                             Now,
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

                    if (responseInfo.Timeout >= Now)
                    {

                        // Before it goes: whoever asked may answer at once that it
                        // cannot read it.
                        RememberForwardedResponse(
                            responseInfo.SourceNodeId,
                            responseInfo.RequestId,
                            JSONResponseMessage.NetworkPath.Source,
                            JSONResponseMessage.RequestId
                        );

                        await parentNetworkingNode.OCPP.OUT.SendJSONResponse(
                                  JSONResponseMessage.ChangeRequestId(responseInfo.RequestId).ChangeNetworking(
                                      // To whoever asked, whatever the answer names: it is not the answerer's to choose.
                                      SourceRouting.To(responseInfo.SourceNodeId),
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
                                                         IMessageConnection            WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONRequestErrorMessage.RequestId, out var responseInfo))
            {

                // In time: sent on to whoever asked, under the request
                // identification it asked with - as an answer is.
                if (responseInfo.Timeout >= Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                              JSONRequestErrorMessage.ChangeRequestId(responseInfo.RequestId).ChangeNetworking(
                                  // To whoever asked, whatever the answer names: it is not the answerer's to choose.
                                  SourceRouting.To(responseInfo.SourceNodeId),
                                  JSONRequestErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                              )
                          );

                }
                else
                    DebugX.Log($"Received a too late JSON request error message for request id '{JSONRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a JSON request error message for an unknown request id '{JSONRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessJSONResponseErrorMessage   (JSONResponseErrorMessage,   WebSocketConnection)

        /// <summary>
        /// A response error about an answer this node forwarded goes on to whoever
        /// answered, under the request id it answered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A response error - a CALLRESULTERROR in OCPP 2.1 - goes from whoever asked to
        /// whoever answered: it is "sent back on receipt of a CALLRESULT that contains
        /// errors", under the id of that CALLRESULT (OCPP 2.1 Edition 2, Part 4, 4.2.3 and
        /// 4.1.4). Here that id is the one this node sent the answer on under, which is the
        /// asker's own; whoever answered knows the request by the id this node forwarded
        /// it under.
        /// </para>
        /// <para>
        /// It used to go the other way, to whoever had asked a request this node still
        /// waited for an answer to, as if it were that answer.
        /// </para>
        /// </remarks>
        public async Task ProcessJSONResponseErrorMessage(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                          IMessageConnection             WebSocketConnection)
        {

            if (TryTakeForwardedResponse(JSONResponseErrorMessage.NetworkPath.Source,
                                         JSONResponseErrorMessage.RequestId,
                                         out var forwardedResponse))
            {

                await parentNetworkingNode.OCPP.OUT.SendJSONResponseError(
                          JSONResponseErrorMessage.ChangeRequestId(forwardedResponse.RequestId).ChangeNetworking(
                              // To whoever answered, whatever the error names: it is not the asker's to choose.
                              SourceRouting.To(forwardedResponse.AnsweredBy),
                              JSONResponseErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                          )
                      );

            }

            else if (ForwardUnknownResponses)
            {

                await parentNetworkingNode.OCPP.OUT.SendJSONResponseError(
                          JSONResponseErrorMessage.ChangeNetworking(
                              JSONResponseErrorMessage.Destination,
                              JSONResponseErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                          )
                      );

            }

            else
                DebugX.Log($"Received a JSON response error message for request id '{JSONResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}', about no answer this node forwarded, or about one forwarded too long ago!");

        }

        #endregion

        #region ProcessJSONSendMessage            (JSONSendMessage,            WebSocketConnection)

        public async Task ProcessJSONSendMessage(OCPP_JSONSendMessage  JSONSendMessage,
                                                 IMessageConnection    WebSocketConnection)
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
                                                                             Now,
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
                                  Now,
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
                          Now,
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
                              Now,
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
                                                      IMessageConnection         WebSocketConnection)
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
                                                                             Now,
                                                                             parentNetworkingNode,
                                                                             WebSocketConnection,
                                                                             BinaryRequestMessage,
                                                                             BinaryRequestMessage.CancellationToken
                                                                         ))
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = WithItsRejectResponseGiven(results.First());

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
                                  Now,
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
                          Now,
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

                // Sent on under a request identification of its own: see
                // ForwardUnderAnIdOfItsOwn.
                newBinaryRequestMessage = newBinaryRequestMessage.ChangeRequestId(
                                              ForwardUnderAnIdOfItsOwn(
                                                  newBinaryRequestMessage.RequestId,
                                                  forwardingDecision.RequestContext,
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

            // One decision, one branch, as for JSON requests.
            else if (forwardingDecision.Result == ForwardingDecisions.REPLACE)
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

                // Sent on under a request identification of its own: see
                // ForwardUnderAnIdOfItsOwn.
                newBinaryRequestMessage = newBinaryRequestMessage.ChangeRequestId(
                                              ForwardUnderAnIdOfItsOwn(
                                                  newBinaryRequestMessage.RequestId,
                                                  forwardingDecision.RequestContext,
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

            // With a reject response or without one: the asker is told, as for JSON requests.
            else if (forwardingDecision.Result == ForwardingDecisions.REJECT)
            {

                // What a filter gave as the answer is the answer, under the request's
                // identification. It was never sent: every REJECT went as a request error,
                // and the response the filter had made was thrown away.
                if (forwardingDecision.RejectResponseGiven &&
                    forwardingDecision.BinaryRejectResponse is Byte[] rejectResponse)
                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponse(
                              new OCPP_BinaryResponseMessage(
                                  Now,
                                  BinaryRequestMessage.EventTrackingId,
                                  NetworkingMode.Unknown,
                                  SourceRouting.To  (BinaryRequestMessage.NetworkPath.Source),
                                  NetworkPath.  From(parentNetworkingNode.Id),
                                  BinaryRequestMessage.RequestId,
                                  rejectResponse,
                                  BinaryRequestMessage.CancellationToken
                              )
                          );

                else
                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                              new OCPP_BinaryRequestErrorMessage(
                                  Now,
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
                                                       IMessageConnection          WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryResponseMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout >= Now)
                {

                    // Before it goes: whoever asked may answer at once that it
                    // cannot read it.
                    RememberForwardedResponse(
                        responseInfo.SourceNodeId,
                        responseInfo.RequestId,
                        BinaryResponseMessage.NetworkPath.Source,
                        BinaryResponseMessage.RequestId
                    );

                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponse(
                              BinaryResponseMessage.ChangeRequestId(responseInfo.RequestId).ChangeNetworking(
                                  // To whoever asked, whatever the answer names: it is not the answerer's to choose.
                                  SourceRouting.To(responseInfo.SourceNodeId),
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
                                                           IMessageConnection              WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryRequestErrorMessage.RequestId, out var responseInfo))
            {

                // In time: sent on to whoever asked, under the request
                // identification it asked with - as an answer is.
                if (responseInfo.Timeout >= Now)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                              BinaryRequestErrorMessage.ChangeRequestId(responseInfo.RequestId).ChangeNetworking(
                                  // To whoever asked, whatever the answer names: it is not the answerer's to choose.
                                  SourceRouting.To(responseInfo.SourceNodeId),
                                  BinaryRequestErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                              )
                          );

                }
                else
                    DebugX.Log($"Received a too late binary request error message for request id '{BinaryRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

            }
            else
                DebugX.Log($"Received a binary request error message for an unknown request id '{BinaryRequestErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}'!");

        }

        #endregion

        #region ProcessBinaryResponseErrorMessage (BinaryResponseErrorMessage, WebSocketConnection)

        /// <summary>
        /// A binary response error about an answer this node forwarded goes on to whoever
        /// answered, under the request id it answered - see ProcessJSONResponseErrorMessage.
        /// </summary>
        public async Task ProcessBinaryResponseErrorMessage(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                            IMessageConnection               WebSocketConnection)
        {

            if (TryTakeForwardedResponse(BinaryResponseErrorMessage.NetworkPath.Source,
                                         BinaryResponseErrorMessage.RequestId,
                                         out var forwardedResponse))
            {

                await parentNetworkingNode.OCPP.OUT.SendBinaryResponseError(
                          BinaryResponseErrorMessage.ChangeRequestId(forwardedResponse.RequestId).ChangeNetworking(
                              // To whoever answered, whatever the error names: it is not the asker's to choose.
                              SourceRouting.To(forwardedResponse.AnsweredBy),
                              BinaryResponseErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                          )
                      );

            }

            else if (ForwardUnknownResponses)
            {

                await parentNetworkingNode.OCPP.OUT.SendBinaryResponseError(
                          BinaryResponseErrorMessage.ChangeNetworking(
                              BinaryResponseErrorMessage.Destination,
                              BinaryResponseErrorMessage.NetworkPath.Append(parentNetworkingNode.Id)
                          )
                      );

            }

            else
                DebugX.Log($"Received a binary response error message for request id '{BinaryResponseErrorMessage.RequestId}' from '{WebSocketConnection.RemoteSocket}', about no answer this node forwarded, or about one forwarded too long ago!");

        }

        #endregion

        #region ProcessBinarySendMessage          (BinarySendMessage,          WebSocketConnection)

        public async Task ProcessBinarySendMessage(OCPP_BinarySendMessage  BinarySendMessage,
                                                   IMessageConnection      WebSocketConnection)
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
                                                                             Now,
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
                                  Now,
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
                          Now,
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
                              Now,
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

        #region (private static) WithItsRejectResponseGiven(Decision)

        /// <summary>
        /// The decision of a filter, marked where it REJECTs with a response of
        /// its own: that response is what the asker gets. A decision a
        /// forwarding processor makes afterwards is not marked, whatever
        /// response it makes up.
        /// </summary>
        private static T WithItsRejectResponseGiven<T>(T Decision)
        {

            if (Decision is RequestForwardingDecision { Result: ForwardingDecisions.REJECT } decision &&
               (decision.JSONRejectResponse is not null || decision.BinaryRejectResponse is not null))
                decision.RejectResponseGiven = true;

            return Decision;

        }

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
                        return WithItsRejectResponseGiven(await FilterHandler(handler));

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
