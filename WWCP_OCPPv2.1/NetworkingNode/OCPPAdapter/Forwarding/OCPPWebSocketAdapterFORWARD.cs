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
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD : IOCPPWebSocketAdapterFORWARD
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


        #region ProcessJSONRequestMessage       (JSONRequestMessage,       WebSocketConnection)

        public async Task ProcessJSONRequestMessage(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                    IWebSocketConnection     WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(JSONRequestMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(JSONRequestMessage.DestinationId))
                return;


            #region Try to call the matching 'incoming message processor'...

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
                {

                    var forwardingDecision = await forwardingProcessor;

                    #region FORWARD

                    if (forwardingDecision.Result == ForwardingResults.FORWARD)
                    {

                        var newJSONRequestMessage = JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                        expectedResponses.TryAdd(
                            newJSONRequestMessage.RequestId,
                            new ResponseInfo(
                                newJSONRequestMessage.RequestId,
                                forwardingDecision.   RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                                newJSONRequestMessage.NetworkPath.Source,
                                newJSONRequestMessage.RequestTimeout
                            )
                        );

                        await parentNetworkingNode.OCPP.OUT.SendJSONRequest(newJSONRequestMessage);

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

                        await parentNetworkingNode.OCPP.OUT.SendJSONRequest(newJSONRequestMessage);

                    }

                    #endregion

                    #region REJECT

                    else if (forwardingDecision.Result == ForwardingResults.REJECT &&
                             forwardingDecision.JSONRejectResponse is not null)
                    {

                        await parentNetworkingNode.OCPP.SendJSONRequestError(
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

        #region ProcessJSONResponseMessage      (JSONResponseMessage,      WebSocketConnection)

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

        #region ProcessJSONRequestErrorMessage  (JSONRequestErrorMessage,  WebSocketConnection)

        public async Task ProcessJSONRequestErrorMessage(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                         IWebSocketConnection          WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONRequestErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {JSONRequestErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {JSONRequestErrorMessage.RequestId}!");

        }

        #endregion

        #region ProcessJSONResponseErrorMessage (JSONResponseErrorMessage, WebSocketConnection)

        public async Task ProcessJSONResponseErrorMessage(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                          IWebSocketConnection           WebSocketConnection)
        {

            if (expectedResponses.TryRemove(JSONResponseErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.SendJSONResponseError(JSONResponseErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {JSONResponseErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {JSONResponseErrorMessage.RequestId}!");

        }

        #endregion


        #region ProcessBinaryRequestMessage     (BinaryRequestMessage,     WebSocketConnection)

        public async Task ProcessBinaryRequestMessage(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                      IWebSocketConnection       WebSocketConnection)
        {

            if (AnycastIdsAllowed.Count > 0 && !AnycastIdsAllowed.Contains(BinaryRequestMessage.DestinationId))
                return;

            if (AnycastIdsDenied. Count > 0 &&  AnycastIdsDenied. Contains(BinaryRequestMessage.DestinationId))
                return;

            await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

        }

        #endregion

        #region ProcessBinaryResponseMessage    (BinaryResponseMessage,    WebSocketConnection)

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
