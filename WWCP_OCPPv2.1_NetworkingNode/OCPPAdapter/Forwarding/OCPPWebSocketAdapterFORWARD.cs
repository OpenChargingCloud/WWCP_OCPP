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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NN;

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

        public ForwardingResult                DefaultResult        { get; set; } = ForwardingResult.DROP;

        public IEnumerable<NetworkingNode_Id>  AnycastIdsAllowed    { get; }      = [];

        public IEnumerable<NetworkingNode_Id>  AnycastIdsDenied     { get; }      = [];

        #endregion

        #region Events

        public event OnJSONRequestMessageSentDelegate?   OnJSONRequestMessageSent;
        public event OnJSONResponseMessageSentDelegate?  OnJSONResponseMessageSent;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for forwarding messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// <param name="DefaultResult">The default forwarding result.</param>
        public OCPPWebSocketAdapterFORWARD(INetworkingNode   NetworkingNode,
                                           ForwardingResult  DefaultResult   = ForwardingResult.DROP)
        {

            this.parentNetworkingNode  = NetworkingNode;
            this.DefaultResult         = DefaultResult;

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


        #region ProcessJSONRequestMessage   (JSONRequestMessage)

        public async Task ProcessJSONRequestMessage(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            if (AnycastIdsAllowed.Any() && !AnycastIdsAllowed.Contains(JSONRequestMessage.DestinationNodeId))
                return;

            if (AnycastIdsDenied. Any() &&  AnycastIdsDenied. Contains(JSONRequestMessage.DestinationNodeId))
                return;


            #region Try to call the matching 'incoming message processor'...

            if (forwardingMessageProcessorsLookup.TryGetValue(JSONRequestMessage.Action, out var methodInfo) &&
                methodInfo is not null)
            {

                //ToDo: Maybe this could be done via code generation!
                var result = methodInfo.Invoke(this,
                                               [ JSONRequestMessage,
                                                 null, //WebSocketConnection,
                                                 JSONRequestMessage.CancellationToken ]);

                if (result is Task<ForwardingDecision> forwardingProcessor)
                {

                    var forwardingDecision = await forwardingProcessor;

                    if (forwardingDecision.Result == ForwardingResult.FORWARD)
                    {

                        var newJSONRequestMessage = JSONRequestMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                        expectedResponses.TryAdd(
                            newJSONRequestMessage.RequestId,
                            new ResponseInfo(
                                newJSONRequestMessage.RequestId,
                                forwardingDecision.RequestContext ?? JSONLDContext.Parse("willnothappen!"),
                                newJSONRequestMessage.RequestTimeout
                            )
                        );

                        var sendOCPPMessageResult = await parentNetworkingNode.OCPP.SendJSONRequest(newJSONRequestMessage);

                        #region Send OnJSONRequestMessageSent event

                        var logger = OnJSONRequestMessageSent;
                        if (logger is not null)
                        {
                            try
                            {

                                await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnJSONRequestMessageSentDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                    parentNetworkingNode,
                                                                                                    //Connection,
                                                                                                    newJSONRequestMessage,
                                                                                                    sendOCPPMessageResult)).
                                                   ToArray());

                            }
                            catch (Exception e)
                            {
                                await HandleErrors(
                                          nameof(TestNetworkingNode),
                                          nameof(OnBootNotificationRequestLogging),
                                          e
                                      );
                            }

                        }

                        #endregion

                    }
                    else if (forwardingDecision.Result == ForwardingResult.REJECT &&
                             forwardingDecision.JSONRejectResponse is not null)
                    {

                        //ToDo: Send the REJECT response back to the sender!

                    }
                    else // ForwardingResult.DROP
                    {
                        // Just ignore the request!
                    }

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

        #region ProcessJSONResponseMessage  (JSONResponseMessage)

        public async Task ProcessJSONResponseMessage(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            if (expectedResponses.TryRemove(JSONResponseMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout >= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    var newJSONResponseMessage  = JSONResponseMessage.AppendToNetworkPath(parentNetworkingNode.Id);

                    var sendOCPPMessageResult   = await parentNetworkingNode.OCPP.SendJSONResponse(newJSONResponseMessage);

                    #region Send OnJSONResponseMessageSent event

                    var logger = OnJSONResponseMessageSent;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnJSONResponseMessageSentDelegate>().
                                                Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                parentNetworkingNode,
                                                                                                //Connection,
                                                                                                newJSONResponseMessage,
                                                                                                sendOCPPMessageResult)).
                                                ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                        nameof(TestNetworkingNode),
                                        nameof(OnBootNotificationRequestLogging),
                                        e
                                    );
                        }

                    }

                    #endregion

                }
                else
                    DebugX.Log($"Received a response message too late for request identification: {JSONResponseMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received a response message for an unknown request identification: {JSONResponseMessage.RequestId}!");

        }

        #endregion

        #region ProcessJSONErrorMessage     (JSONErrorMessage)

        public async Task ProcessJSONErrorMessage(OCPP_JSONErrorMessage JSONErrorMessage)
        {

            if (expectedResponses.TryRemove(JSONErrorMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                    //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.SendJSONError(JSONErrorMessage);

                }
                else
                    DebugX.Log($"Received an error message too late for request identification: {JSONErrorMessage.RequestId}!");

            }
            else
                DebugX.Log($"Received an error message for an unknown request identification: {JSONErrorMessage.RequestId}!");

        }

        #endregion


        #region ProcessBinaryRequestMessage (BinaryRequestMessage)

        public async Task ProcessBinaryRequestMessage(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            if (AnycastIdsAllowed.Any() && !AnycastIdsAllowed.Contains(BinaryRequestMessage.DestinationNodeId))
                return;

            if (AnycastIdsDenied. Any() &&  AnycastIdsDenied. Contains(BinaryRequestMessage.DestinationNodeId))
                return;

            await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

        }

        #endregion

        #region ProcessBinaryResponseMessage(BinaryResponseMessage)

        public async Task ProcessBinaryResponseMessage(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            if (expectedResponses.TryRemove(BinaryResponseMessage.RequestId, out var responseInfo))
            {

                if (responseInfo.Timeout <= Timestamp.Now)
                //responseInfo.Context == JSONResponseMessage.Context)
                {

                    await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

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
