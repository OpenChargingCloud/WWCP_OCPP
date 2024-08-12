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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A Heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<HeartbeatRequest, HeartbeatResponse>>

        OnHeartbeatRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         HeartbeatRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered Heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnHeartbeatRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           HeartbeatRequest                                          Request,
                                           ForwardingDecision<HeartbeatRequest, HeartbeatResponse>   ForwardingDecision,
                                           CancellationToken                                         CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnHeartbeatRequestReceivedDelegate?    OnHeartbeatRequestReceived;
        public event OnHeartbeatRequestFilterDelegate?      OnHeartbeatRequestFilter;
        public event OnHeartbeatRequestFilteredDelegate?    OnHeartbeatRequestFiltered;
        public event OnHeartbeatRequestSentDelegate?        OnHeartbeatRequestSent;

        public event OnHeartbeatResponseReceivedDelegate?   OnHeartbeatResponseReceived;
        public event OnHeartbeatResponseSentDelegate?       OnHeartbeatResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_Heartbeat(OCPP_JSONRequestMessage  JSONRequestMessage,
                              IWebSocketConnection     WebSocketConnection,
                              CancellationToken        CancellationToken   = default)

        {

            #region Parse the Heartbeat request

            if (!HeartbeatRequest.TryParse(JSONRequestMessage.Payload,
                                           JSONRequestMessage.RequestId,
                                           JSONRequestMessage.DestinationId,
                                           JSONRequestMessage.NetworkPath,
                                           out var request,
                                           out var errorResponse,
                                           JSONRequestMessage.RequestTimestamp,
                                           JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                           JSONRequestMessage.EventTrackingId,
                                           parentNetworkingNode.OCPP.CustomHeartbeatRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnHeartbeatRequestReceived event

            await LogEvent(
                      OnHeartbeatRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnHeartbeatRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnHeartbeatRequestFilter,
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

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<HeartbeatRequest, HeartbeatResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new HeartbeatResponse(
                                       request,
                                       Timestamp.Now,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<HeartbeatRequest, HeartbeatResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnHeartbeatRequestFiltered event

            await LogEvent(
                      OnHeartbeatRequestFiltered,
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


            #region Attach OnHeartbeatRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnHeartbeatRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnHeartbeatRequestSent,
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
