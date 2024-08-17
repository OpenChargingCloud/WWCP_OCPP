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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A UnlockConnector request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>>

        OnUnlockConnectorRequestFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               UnlockConnectorRequest   Request,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A filtered UnlockConnector request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnUnlockConnectorRequestFilteredDelegate(DateTime                                                              Timestamp,
                                                 IEventSender                                                          Sender,
                                                 IWebSocketConnection                                                  Connection,
                                                 UnlockConnectorRequest                                                Request,
                                                 ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>   ForwardingDecision,
                                                 CancellationToken                                                     CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnUnlockConnectorRequestReceivedDelegate?    OnUnlockConnectorRequestReceived;
        public event OnUnlockConnectorRequestFilterDelegate?      OnUnlockConnectorRequestFilter;
        public event OnUnlockConnectorRequestFilteredDelegate?    OnUnlockConnectorRequestFiltered;
        public event OnUnlockConnectorRequestSentDelegate?        OnUnlockConnectorRequestSent;

        public event OnUnlockConnectorResponseReceivedDelegate?   OnUnlockConnectorResponseReceived;
        public event OnUnlockConnectorResponseSentDelegate?       OnUnlockConnectorResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_UnlockConnector(OCPP_JSONRequestMessage    JSONRequestMessage,
                                    OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                    IWebSocketConnection       WebSocketConnection,
                                    CancellationToken          CancellationToken   = default)

        {

            #region Parse the UnlockConnector request

            if (!UnlockConnectorRequest.TryParse(JSONRequestMessage.Payload,
                                                 JSONRequestMessage.RequestId,
                                                 JSONRequestMessage.Destination,
                                                 JSONRequestMessage.NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 JSONRequestMessage.RequestTimestamp,
                                                 JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                 JSONRequestMessage.EventTrackingId,
                                                 parentNetworkingNode.OCPP.CustomUnlockConnectorRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnUnlockConnectorRequestReceived event

            await LogEvent(
                      OnUnlockConnectorRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnUnlockConnectorRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnUnlockConnectorRequestFilter,
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

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingDecisions.FORWARD)
                forwardingDecision = new ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new UnlockConnectorResponse(
                                       request,
                                       UnlockStatus.UnlockFailed,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomUnlockConnectorResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnUnlockConnectorRequestFiltered event

            await LogEvent(
                      OnUnlockConnectorRequestFiltered,
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


            #region Attach OnUnlockConnectorRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnUnlockConnectorRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnUnlockConnectorRequestSent,
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
