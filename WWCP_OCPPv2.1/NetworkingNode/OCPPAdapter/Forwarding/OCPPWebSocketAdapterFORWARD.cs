﻿/*
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
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A delegate called whenever any JSON request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<ForwardingDecision>

        OnAnyJSONRequestFilterDelegate(DateTime                  Timestamp,
                                       IEventSender              Sender,
                                       IWebSocketConnection      Connection,
                                       OCPP_JSONRequestMessage   Request,
                                       CancellationToken         CancellationToken = default);


    /// <summary>
    /// A delegate called whenever any JSON request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyJSONRequestFilteredDelegate(DateTime                  Timestamp,
                                         IEventSender              Sender,
                                         IWebSocketConnection      Connection,
                                         OCPP_JSONRequestMessage   Request,
                                         ForwardingDecision        ForwardingDecision,
                                         CancellationToken         CancellationToken = default);


    /// <summary>
    /// A delegate called whenever any Binary request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<ForwardingDecision>

        OnAnyBinaryRequestFilterDelegate(DateTime                    Timestamp,
                                         IEventSender                Sender,
                                         IWebSocketConnection        Connection,
                                         OCPP_BinaryRequestMessage   Request,
                                         CancellationToken           CancellationToken = default);


    /// <summary>
    /// A delegate called whenever any Binary request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAnyBinaryRequestFilteredDelegate(DateTime                    Timestamp,
                                           IEventSender                Sender,
                                           IWebSocketConnection        Connection,
                                           OCPP_BinaryRequestMessage   Request,
                                           ForwardingDecision          ForwardingDecision,
                                           CancellationToken           CancellationToken = default);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Data

        private   readonly  INetworkingNode                                 parentNetworkingNode;

        protected readonly  Dictionary<String, MethodInfo>                  forwardingMessageProcessorsLookup   = [];

        protected readonly  ConcurrentDictionary<Request_Id, ResponseInfo>  expectedResponses                   = [];

        #endregion

        #region Properties

        public ForwardingResults           DefaultForwardingResult    { get; set; } = ForwardingResults.DROP;

        public HashSet<NetworkingNode_Id>  AnycastIdsAllowed          { get; }      = [];

        public HashSet<NetworkingNode_Id>  AnycastIdsDenied           { get; }      = [];

        #endregion

        #region Events

        public event OnAnyJSONRequestFilterDelegate      OnAnyJSONRequestFilter;
        public event OnAnyJSONRequestFilteredDelegate    OnAnyJSONRequestFiltered;

        public event OnAnyBinaryRequestFilterDelegate    OnAnyBinaryRequestFilter;
        public event OnAnyBinaryRequestFilteredDelegate  OnAnyBinaryRequestFiltered;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for forwarding messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// <param name="DefaultForwardingResult">The default forwarding result.</param>
        public OCPPWebSocketAdapterFORWARD(INetworkingNode    NetworkingNode,
                                           ForwardingResults  DefaultForwardingResult = ForwardingResults.DROP)
        {

            this.parentNetworkingNode     = NetworkingNode;
            this.DefaultForwardingResult  = DefaultForwardingResult;

            #region Reflect "Forward_XXX" messages and wire them...

            foreach (var method in typeof(OCPPWebSocketAdapterFORWARD).
                                       GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                            Where(method => method.Name.StartsWith("Forward_")))
            {

                var processorName = method.Name[8..];

                if (forwardingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                forwardingMessageProcessorsLookup.Add(processorName,
                                                      method);

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

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(JSONRequestMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(JSONRequestMessage.DestinationId))
                return;


            #region Do we have a general filter rule for any JSON request?

            ForwardingDecision? forwardingDecision = null;

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

            #region In case: Try to call the matching 'incoming message processor'...

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingResults.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(JSONRequestMessage.Action, out var methodInfo) &&
                    methodInfo is not null)
                {

                    //ToDo: Maybe this could be done via code generation!
                    var result = methodInfo.Invoke(
                                     this,
                                     [
                                         JSONRequestMessage,
                                         WebSocketConnection,
                                         JSONRequestMessage.CancellationToken
                                     ]
                                 );

                    if (result is Task<ForwardingDecision> forwardingProcessor)
                        forwardingDecision = await forwardingProcessor;

                    else
                        DebugX.Log($"Received undefined '{JSONRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {


                    DebugX.Log($"Received unknown '{JSONRequestMessage.Action}' JSON request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    //OCPPErrorResponse = new OCPP_JSONErrorMessage(
                    //                        Timestamp.Now,
                    //                        EventTracking_Id.New,
                    //                        NetworkingMode.Unknown,
                    //                        NetworkingNode_Id.Zero,
                    //                        NetworkPath.Empty,
                    //                        jsonRequest.RequestId,
                    //                        ResultCode.ProtocolError,
                    //                        $"The OCPP message '{jsonRequest.Action}' is unkown!",
                    //                        new JObject(
                    //                            new JProperty("request", JSONMessage)
                    //                        )
                    //                    );

                }

                #endregion

            }

            #endregion


            forwardingDecision ??= new ForwardingDecision(DefaultForwardingResult);


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

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var newJSONRequestMessage = JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestinationId.HasValue)
                    newJSONRequestMessage = newJSONRequestMessage.ChangeNetworking(forwardingDecision.NewDestinationId.Value);

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

            if (forwardingDecision.Result == ForwardingResults.REPLACE)
            {

                var newJSONRequestMessage = forwardingDecision.NewJSONRequest is null
                                                ? JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                                : new OCPP_JSONRequestMessage(
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.EventTrackingId,
                                                      JSONRequestMessage.NetworkingMode,
                                                      forwardingDecision.NewDestinationId ?? JSONRequestMessage.DestinationId,
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
                          newJSONRequestMessage
                      );

            }

            #endregion

            #region REJECT

            else if (forwardingDecision.Result == ForwardingResults.REJECT &&
                     forwardingDecision.JSONRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                          new OCPP_JSONRequestErrorMessage(
                              Timestamp.Now,
                              JSONRequestMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              JSONRequestMessage.NetworkPath.Source,
                              NetworkPath.From(parentNetworkingNode.Id),
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

            if (expectedResponses.TryRemove(JSONResponseMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout >= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONResponse(
                              JSONResponseMessage.ChangeNetworking(
                                  JSONResponseMessage.DestinationId == NetworkingNode_Id.Zero
                                      ? responseInfo.       SourceNodeId
                                      : JSONResponseMessage.DestinationId,
                                  JSONResponseMessage.NetworkPath.Append(parentNetworkingNode.Id)
                              )
                          );

                }
                else
                    DebugX.Log($"Received a response message too late for request identification: {JSONResponseMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received a response message for an unknown request identification: {JSONResponseMessage.RequestId}!");

        }

        #endregion

        #region ProcessJSONRequestErrorMessage    (JSONRequestErrorMessage,    WebSocketConnection)

        public async Task ProcessJSONRequestErrorMessage(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                         IWebSocketConnection          WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONRequestErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(JSONRequestErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {JSONRequestErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {JSONRequestErrorMessage.RequestId}!");

        }

        #endregion

        #region ProcessJSONResponseErrorMessage   (JSONResponseErrorMessage,   WebSocketConnection)

        public async Task ProcessJSONResponseErrorMessage(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                          IWebSocketConnection           WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONResponseErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendJSONResponseError(JSONResponseErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {JSONResponseErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {JSONResponseErrorMessage.RequestId}!");

        }

        #endregion

        #region ProcessJSONSendMessage            (JSONSendMessage,            WebSocketConnection)

        public async Task ProcessJSONSendMessage(OCPP_JSONSendMessage  JSONSendMessage,
                                                 IWebSocketConnection  WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(JSONSendMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(JSONSendMessage.DestinationId))
                return;


            #region Try to call the matching 'incoming message processor'...

            if (forwardingMessageProcessorsLookup.TryGetValue(JSONSendMessage.Action, out var methodInfo) &&
                methodInfo is not null)
            {

                //ToDo: Maybe this could be done via code generation!
                var result = methodInfo.Invoke(
                                 this,
                                 [
                                     JSONSendMessage,
                                     WebSocketConnection,
                                     JSONSendMessage.CancellationToken
                                 ]
                             );

                if (result is Task<ForwardingDecision> forwardingProcessor)
                {

                    var forwardingDecision = await forwardingProcessor;

                    #region FORWARD

                    if (forwardingDecision.Result == ForwardingResults.FORWARD)
                    {

                        var newJSONSendMessage = JSONSendMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                        //expectedResponses.TryAdd(
                        //    newJSONRequestMessage.MessageId,
                        //    new ResponseInfo(
                        //        newJSONRequestMessage.MessageId,
                        //        forwardingDecision.   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        //        newJSONRequestMessage.NetworkPath.Source,
                        //        newJSONRequestMessage.RequestTimeout
                        //    )
                        //);

                        await parentNetworkingNode.OCPP.OUT.SendJSONSendMessage(newJSONSendMessage);

                    }

                    #endregion

                    #region REPLACE

                    if (forwardingDecision.Result == ForwardingResults.REPLACE)
                    {

                        var newJSONSendMessage = forwardingDecision.NewJSONRequest is null
                                                        ? JSONSendMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                                        : new OCPP_JSONSendMessage(
                                                              JSONSendMessage.MessageTimestamp,
                                                              JSONSendMessage.EventTrackingId,
                                                              JSONSendMessage.NetworkingMode,
                                                              forwardingDecision.NewDestinationId ?? JSONSendMessage.DestinationId,
                                                              JSONSendMessage.NetworkPath.Append(parentNetworkingNode.Id),
                                                              JSONSendMessage.MessageId,
                                                              forwardingDecision.NewAction        ?? JSONSendMessage.Action,
                                                              forwardingDecision.NewJSONRequest, // <-- !!!
                                                              JSONSendMessage.ErrorMessage,
                                                              JSONSendMessage.CancellationToken
                                                          );

                        //expectedResponses.TryAdd(
                        //    newJSONSendMessage.MessageId,
                        //    new ResponseInfo(
                        //        newJSONSendMessage.MessageId,
                        //        forwardingDecision.   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                        //        newJSONSendMessage.NetworkPath.Source,
                        //        newJSONSendMessage.RequestTimeout
                        //    )
                        //);

                        await parentNetworkingNode.OCPP.OUT.SendJSONSendMessage(newJSONSendMessage);

                    }

                    #endregion

                    #region REJECT

                    else if (forwardingDecision.Result == ForwardingResults.REJECT &&
                             forwardingDecision.JSONRejectResponse is not null)
                    {

                        await parentNetworkingNode.OCPP.OUT.SendJSONRequestError(
                                  new OCPP_JSONRequestErrorMessage(
                                      Timestamp.Now,
                                      JSONSendMessage.EventTrackingId,
                                      NetworkingMode.Unknown,
                                      JSONSendMessage.NetworkPath.Source,
                                      NetworkPath.From(parentNetworkingNode.Id),
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

                else
                    DebugX.Log($"Received undefined '{JSONSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

            }

            #endregion

            #region ...or error!

            else
            {

                DebugX.Log($"Received unknown '{JSONSendMessage.Action}' JSON send message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                //OCPPErrorResponse = new OCPP_JSONErrorMessage(
                //                        Timestamp.Now,
                //                        EventTracking_Id.New,
                //                        NetworkingMode.Unknown,
                //                        NetworkingNode_Id.Zero,
                //                        NetworkPath.Empty,
                //                        jsonRequest.RequestId,
                //                        ResultCode.ProtocolError,
                //                        $"The OCPP message '{jsonRequest.Action}' is unkown!",
                //                        new JObject(
                //                            new JProperty("request", JSONMessage)
                //                        )
                //                    );

            }

            #endregion

        }

        #endregion


        #region ProcessBinaryRequestMessage       (BinaryRequestMessage,       WebSocketConnection)

        public async Task ProcessBinaryRequestMessage(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                      IWebSocketConnection       WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(BinaryRequestMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(BinaryRequestMessage.DestinationId))
                return;

            #region Do we have a general filter rule for any Binary request?

            ForwardingDecision? forwardingDecision = null;

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

            #region In case: Try to call the matching 'incoming message processor'...

            if (forwardingDecision is null ||
                forwardingDecision.Result == ForwardingResults.NEXT)
            {

                #region A filter rule for the OCPP action was found...

                if (forwardingMessageProcessorsLookup.TryGetValue(BinaryRequestMessage.Action, out var methodInfo) &&
                    methodInfo is not null)
                {

                    //ToDo: Maybe this could be done via code generation!
                    var result = methodInfo.Invoke(
                                     this,
                                     [
                                         BinaryRequestMessage,
                                         WebSocketConnection,
                                         BinaryRequestMessage.CancellationToken
                                     ]
                                 );

                    if (result is Task<ForwardingDecision> forwardingProcessor)
                        forwardingDecision = await forwardingProcessor;

                    else
                        DebugX.Log($"Received undefined '{BinaryRequestMessage.Action}' Binary request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                }

                #endregion

                #region ...or error!

                else
                {


                    DebugX.Log($"Received unknown '{BinaryRequestMessage.Action}' Binary request message handler within {nameof(OCPPWebSocketAdapterFORWARD)}!");

                    //OCPPErrorResponse = new OCPP_BinaryErrorMessage(
                    //                        Timestamp.Now,
                    //                        EventTracking_Id.New,
                    //                        NetworkingMode.Unknown,
                    //                        NetworkingNode_Id.Zero,
                    //                        NetworkPath.Empty,
                    //                        jsonRequest.RequestId,
                    //                        ResultCode.ProtocolError,
                    //                        $"The OCPP message '{jsonRequest.Action}' is unkown!",
                    //                        new JObject(
                    //                            new JProperty("request", BinaryMessage)
                    //                        )
                    //                    );

                }

                #endregion

            }

            #endregion


            forwardingDecision ??= new ForwardingDecision(DefaultForwardingResult);


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

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var newBinaryRequestMessage = BinaryRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                if (forwardingDecision.NewDestinationId.HasValue)
                    newBinaryRequestMessage = newBinaryRequestMessage.ChangeNetworking(forwardingDecision.NewDestinationId.Value);

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

            if (forwardingDecision.Result == ForwardingResults.REPLACE)
            {

                var newBinaryRequestMessage = forwardingDecision.NewBinaryRequest is null
                                                ? BinaryRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id)
                                                : new OCPP_BinaryRequestMessage(
                                                      BinaryRequestMessage.RequestTimestamp,
                                                      BinaryRequestMessage.EventTrackingId,
                                                      BinaryRequestMessage.NetworkingMode,
                                                      forwardingDecision.NewDestinationId ?? BinaryRequestMessage.DestinationId,
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

            else if (forwardingDecision.Result == ForwardingResults.REJECT &&
                     forwardingDecision.BinaryRejectResponse is not null)
            {

                await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(
                          new OCPP_BinaryRequestErrorMessage(
                              Timestamp.Now,
                              BinaryRequestMessage.EventTrackingId,
                              NetworkingMode.Unknown,
                              BinaryRequestMessage.NetworkPath.Source,
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
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponse(
                              BinaryResponseMessage.ChangeNetworking(
                                  BinaryResponseMessage.DestinationId == NetworkingNode_Id.Zero
                                      ? responseInfo.       SourceNodeId
                                      : BinaryResponseMessage.DestinationId,
                                  BinaryResponseMessage.NetworkPath.Append(parentNetworkingNode.Id)
                              )
                          );

                }
                else
                    DebugX.Log($"Received a binary response message too late for request identification: {BinaryResponseMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received a binary response message for an unknown request identification: {BinaryResponseMessage.RequestId}!");

        }

        #endregion

        #region ProcessBinaryRequestErrorMessage  (BinaryRequestErrorMessage,  WebSocketConnection)

        public async Task ProcessBinaryRequestErrorMessage(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                           IWebSocketConnection            WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryRequestErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == BinaryResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryRequestError(BinaryRequestErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {BinaryRequestErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {BinaryRequestErrorMessage.RequestId}!");

        }

        #endregion

        #region ProcessBinaryResponseErrorMessage (BinaryResponseErrorMessage, WebSocketConnection)

        public async Task ProcessBinaryResponseErrorMessage(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                            IWebSocketConnection             WebSocketConnection)
        {

            if (expectedResponses.TryRemove(BinaryResponseErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == BinaryResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.OUT.SendBinaryResponseError(BinaryResponseErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {BinaryResponseErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {BinaryResponseErrorMessage.RequestId}!");

        }

        #endregion

        #region ProcessBinarySendMessage          (BinarySendMessage,          WebSocketConnection)

        public async Task ProcessBinarySendMessage(OCPP_BinarySendMessage  BinarySendMessage,
                                                   IWebSocketConnection    WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(BinarySendMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(BinarySendMessage.DestinationId))
                return;

            await parentNetworkingNode.OCPP.OUT.SendBinarySendMessage(BinarySendMessage);

        }

        #endregion



        #region (private) LogEvent(Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentNetworkingNode.LogEvent(nameof(OCPPWebSocketAdapterIN), Logger, LogHandler, EventName, OCPPCommand);

        #endregion

        #region (private) HandleErrors(Caller, ExceptionOccured)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterFORWARD),
                   Caller,
                   ExceptionOccured
               );

        #endregion




        [Obsolete]
        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterFORWARD),
                   Caller,
                   ExceptionOccured
               );


    }

}
