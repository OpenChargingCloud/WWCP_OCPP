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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A NotifyPriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>>

        OnNotifyPriorityChargingRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      NotifyPriorityChargingRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered NotifyPriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnNotifyPriorityChargingRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        NotifyPriorityChargingRequest                                                       Request,
                                                        RequestForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>   ForwardingDecision,
                                                        CancellationToken                                                                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyPriorityChargingRequestReceivedDelegate?    OnNotifyPriorityChargingRequestReceived;
        public event OnNotifyPriorityChargingRequestFilterDelegate?      OnNotifyPriorityChargingRequestFilter;
        public event OnNotifyPriorityChargingRequestFilteredDelegate?    OnNotifyPriorityChargingRequestFiltered;
        public event OnNotifyPriorityChargingRequestSentDelegate?        OnNotifyPriorityChargingRequestSent;

        public event OnNotifyPriorityChargingResponseReceivedDelegate?   OnNotifyPriorityChargingResponseReceived;
        public event OnNotifyPriorityChargingResponseSentDelegate?       OnNotifyPriorityChargingResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_NotifyPriorityCharging(OCPP_JSONRequestMessage    JSONRequestMessage,
                                           OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                           IWebSocketConnection       WebSocketConnection,
                                           CancellationToken          CancellationToken   = default)

        {

            #region Parse the NotifyPriorityCharging request

            if (!NotifyPriorityChargingRequest.TryParse(JSONRequestMessage.Payload,
                                                        JSONRequestMessage.RequestId,
                                                        JSONRequestMessage.Destination,
                                                        JSONRequestMessage.NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        JSONRequestMessage.RequestTimestamp,
                                                        JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                        JSONRequestMessage.EventTrackingId,
                                                        parentNetworkingNode.OCPP.CustomNotifyPriorityChargingRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnNotifyPriorityChargingRequestReceived event

            await LogEvent(
                      OnNotifyPriorityChargingRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnNotifyPriorityChargingRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnNotifyPriorityChargingRequestFilter,
                                               filter => filter.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request,
                                                             CancellationToken
                                                         )
                                           );

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = new RequestForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyPriorityChargingResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyPriorityChargingResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnNotifyPriorityChargingRequestFiltered event

            await LogEvent(
                      OnNotifyPriorityChargingRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          forwardingDecision,
                          CancellationToken
                      )
                  );

            #endregion


            #region Attach OnNotifyPriorityChargingRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnNotifyPriorityChargingRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnNotifyPriorityChargingRequestSent,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Timestamp.Now,
                                      parentNetworkingNode,
                                      sentMessageResult.Connection,
                                      request,
                                      sentMessageResult.Result,
                                      CancellationToken
                                  )
                              );

            }

            #endregion

            return forwardingDecision;

        }

    }

}
