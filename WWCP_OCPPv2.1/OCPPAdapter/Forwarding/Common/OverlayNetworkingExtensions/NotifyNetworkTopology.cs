///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod;
//using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

//using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
//{

//    #region Delegates

//    /// <summary>
//    /// A NotifyNetworkTopology request.
//    /// </summary>
//    /// <param name="Timestamp">The timestamp of the request.</param>
//    /// <param name="Sender">The sender of the request.</param>
//    /// <param name="Connection">The HTTP Web Socket connection.</param>
//    /// <param name="Request">The request.</param>
//    /// <param name="CancellationToken">A token to cancel this request.</param>
//    public delegate Task<ForwardingDecision<NotifyNetworkTopologyMessage>>

//        OnNotifyNetworkTopologyMessageFilterDelegate(DateTime                       Timestamp,
//                                                     IEventSender                   Sender,
//                                                     IWebSocketConnection           Connection,
//                                                     NotifyNetworkTopologyMessage   Request,
//                                                     CancellationToken              CancellationToken);


//    /// <summary>
//    /// A filtered NotifyNetworkTopology request.
//    /// </summary>
//    /// <param name="Timestamp">The timestamp of the request.</param>
//    /// <param name="Sender">The sender of the request.</param>
//    /// <param name="Connection">The HTTP Web Socket connection.</param>
//    /// <param name="Request">The request.</param>
//    /// <param name="ForwardingDecision">The forwarding decision.</param>
//    /// <param name="CancellationToken">A token to cancel this request.</param>
//    public delegate Task

//        OnNotifyNetworkTopologyMessageFilteredDelegate(DateTime                                           Timestamp,
//                                                       IEventSender                                       Sender,
//                                                       IWebSocketConnection                               Connection,
//                                                       NotifyNetworkTopologyMessage                       Request,
//                                                       ForwardingDecision<NotifyNetworkTopologyMessage>   ForwardingDecision,
//                                                       CancellationToken                                  CancellationToken);

//    #endregion

//    public partial class OCPPWebSocketAdapterFORWARD
//    {

//        #region Events

//        public event OnNotifyNetworkTopologyMessageReceivedDelegate?    OnNotifyNetworkTopologyMessageReceived;
//        public event OnNotifyNetworkTopologyMessageFilterDelegate?      OnNotifyNetworkTopologyMessageFilter;
//        public event OnNotifyNetworkTopologyMessageFilteredDelegate?    OnNotifyNetworkTopologyMessageFiltered;
//        public event OnNotifyNetworkTopologyMessageSentDelegate?        OnNotifyNetworkTopologyMessageSent;

//        //public event OnNotifyNetworkTopologyResponseReceivedDelegate?   OnNotifyNetworkTopologyResponseReceived;
//        //public event OnNotifyNetworkTopologyResponseSentDelegate?       OnNotifyNetworkTopologyResponseSent;

//        #endregion

//        public async Task<ForwardingDecision>

//            Forward_NotifyNetworkTopology(OCPP_JSONRequestMessage    JSONRequestMessage,
//                                          OCPP_BinaryRequestMessage  BinaryRequestMessage,
//                                          IWebSocketConnection       WebSocketConnection,
//                                          CancellationToken          CancellationToken   = default)

//        {

//            #region Parse the NotifyNetworkTopology request

//            if (!NotifyNetworkTopologyMessage.TryParse(JSONRequestMessage.Payload,
//                                                       JSONRequestMessage.RequestId,
//                                                       JSONRequestMessage.Destination,
//                                                       JSONRequestMessage.NetworkPath,
//                                                       out var request,
//                                                       out var errorResponse,
//                                                       JSONRequestMessage.RequestTimestamp,
//                                                       JSONRequestMessage.RequestTimeout - Timestamp.Now,
//                                                       JSONRequestMessage.EventTrackingId,
//                                                       parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageParser))
//            {
//                return ForwardingDecision.REJECT(errorResponse);
//            }

//            #endregion

//            #region Send OnNotifyNetworkTopologyMessageReceived event

//            await LogEvent(
//                      OnNotifyNetworkTopologyMessageReceived,
//                      loggingDelegate => loggingDelegate.Invoke(
//                          Timestamp.Now,
//                          parentNetworkingNode,
//                          WebSocketConnection,
//                          request,
//                          CancellationToken
//                      )
//                  );

//            #endregion


//            #region Send OnNotifyNetworkTopologyMessageFilter event

//            var forwardingDecision = await CallFilter(
//                                               OnNotifyNetworkTopologyMessageFilter,
//                                               filter => filter.Invoke(
//                                                             Timestamp.Now,
//                                                             parentNetworkingNode,
//                                                             WebSocketConnection,
//                                                             request,
//                                                             CancellationToken
//                                                         )
//                                           );

//            #endregion

//            #region Default result

//            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
//                forwardingDecision = new ForwardingDecision<NotifyNetworkTopologyMessage>(
//                                         request,
//                                         ForwardingDecisions.FORWARD
//                                     );

//            if (forwardingDecision is null ||
//               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
//            {

//                var response = forwardingDecision?.RejectResponse ??
//                                   new NotifyNetworkTopologyResponse(
//                                       request,
//                                       NetworkTopologyStatus.Error,
//                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
//                                   );

//                forwardingDecision = new ForwardingDecision<NotifyNetworkTopologyMessage>(
//                                         request,
//                                         ForwardingDecisions.REJECT,
//                                         response,
//                                         response.ToJSON(
//                                             parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
//                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
//                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
//                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
//                                         )
//                                     );

//            }

//            #endregion

//            if (forwardingDecision.NewRequest is not null)
//                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
//                                                        parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageSerializer,
//                                                        parentNetworkingNode.OCPP.CustomNetworkTopologyInformationSerializer,
//                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
//                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
//                                                    );

//            #region Send OnNotifyNetworkTopologyMessageFiltered event

//            await LogEvent(
//                      OnNotifyNetworkTopologyMessageFiltered,
//                      loggingDelegate => loggingDelegate.Invoke(
//                          Timestamp.Now,
//                          parentNetworkingNode,
//                          WebSocketConnection,
//                          request,
//                          forwardingDecision,
//                          CancellationToken
//                      )
//                  );

//            #endregion


//            #region Attach OnNotifyNetworkTopologyMessageSent event

//            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
//            {

//                var sentLogging = OnNotifyNetworkTopologyMessageSent;
//                if (sentLogging is not null)
//                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
//                        await LogEvent(
//                                  OnNotifyNetworkTopologyMessageSent,
//                                  loggingDelegate => loggingDelegate.Invoke(
//                                      Timestamp.Now,
//                                      parentNetworkingNode,
//                                      sentMessageResult.Connection,
//                                      request,
//                                      sentMessageResult.Result,
//                                      CancellationToken
//                                  )
//                              );

//            }

//            #endregion

//            return forwardingDecision;

//        }

//    }

//}
