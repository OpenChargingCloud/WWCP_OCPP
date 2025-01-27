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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>>

        OnCostUpdatedRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           CostUpdatedRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnCostUpdatedRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             CostUpdatedRequest                                            Request,
                                             RequestForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>   ForwardingDecision,
                                             CancellationToken                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnCostUpdatedRequestReceivedDelegate?    OnCostUpdatedRequestReceived;
        public event OnCostUpdatedRequestFilterDelegate?      OnCostUpdatedRequestFilter;
        public event OnCostUpdatedRequestFilteredDelegate?    OnCostUpdatedRequestFiltered;
        public event OnCostUpdatedRequestSentDelegate?        OnCostUpdatedRequestSent;

        public event OnCostUpdatedResponseReceivedDelegate?   OnCostUpdatedResponseReceived;
        public event OnCostUpdatedResponseSentDelegate?       OnCostUpdatedResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_CostUpdated(OCPP_JSONRequestMessage    JSONRequestMessage,
                                OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                IWebSocketConnection       WebSocketConnection,
                                CancellationToken          CancellationToken   = default)

        {

            #region Parse the CostUpdated request

            if (!CostUpdatedRequest.TryParse(JSONRequestMessage.Payload,
                                             JSONRequestMessage.RequestId,
                                             JSONRequestMessage.Destination,
                                             JSONRequestMessage.NetworkPath,
                                             out var request,
                                             out var errorResponse,
                                             JSONRequestMessage.RequestTimestamp,
                                             JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                             JSONRequestMessage.EventTrackingId,
                                             parentNetworkingNode.OCPP.CustomCostUpdatedRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnCostUpdatedRequestReceived event

            await LogEvent(
                      OnCostUpdatedRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnCostUpdatedRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnCostUpdatedRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new CostUpdatedResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomCostUpdatedResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomCostUpdatedRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnCostUpdatedRequestFiltered event

            await LogEvent(
                      OnCostUpdatedRequestFiltered,
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


            #region Attach OnCostUpdatedRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnCostUpdatedRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnCostUpdatedRequestSent,
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
