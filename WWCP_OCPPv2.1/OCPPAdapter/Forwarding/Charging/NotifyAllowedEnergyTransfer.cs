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
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>>

        OnNotifyAllowedEnergyTransferRequestFilterDelegate(DateTime                             Timestamp,
                                                           IEventSender                         Sender,
                                                           IWebSocketConnection                 Connection,
                                                           NotifyAllowedEnergyTransferRequest   Request,
                                                           CancellationToken                    CancellationToken);


    /// <summary>
    /// A filtered NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferRequestFilteredDelegate(DateTime                                                                                      Timestamp,
                                                             IEventSender                                                                                  Sender,
                                                             IWebSocketConnection                                                                          Connection,
                                                             NotifyAllowedEnergyTransferRequest                                                            Request,
                                                             RequestForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>   ForwardingDecision,
                                                             CancellationToken                                                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyAllowedEnergyTransferRequestReceivedDelegate?    OnNotifyAllowedEnergyTransferRequestReceived;
        public event OnNotifyAllowedEnergyTransferRequestFilterDelegate?      OnNotifyAllowedEnergyTransferRequestFilter;
        public event OnNotifyAllowedEnergyTransferRequestFilteredDelegate?    OnNotifyAllowedEnergyTransferRequestFiltered;
        public event OnNotifyAllowedEnergyTransferRequestSentDelegate?        OnNotifyAllowedEnergyTransferRequestSent;

        public event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?   OnNotifyAllowedEnergyTransferResponseReceived;
        public event OnNotifyAllowedEnergyTransferResponseSentDelegate?       OnNotifyAllowedEnergyTransferResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_NotifyAllowedEnergyTransfer(OCPP_JSONRequestMessage    JSONRequestMessage,
                                                OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                IWebSocketConnection       WebSocketConnection,
                                                CancellationToken          CancellationToken   = default)

        {

            #region Parse the NotifyAllowedEnergyTransfer request

            if (!NotifyAllowedEnergyTransferRequest.TryParse(JSONRequestMessage.Payload,
                                                             JSONRequestMessage.RequestId,
                                                             JSONRequestMessage.Destination,
                                                             JSONRequestMessage.NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             JSONRequestMessage.RequestTimestamp,
                                                             JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                             JSONRequestMessage.EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnNotifyAllowedEnergyTransferRequestReceived event

            await LogEvent(
                      OnNotifyAllowedEnergyTransferRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnNotifyAllowedEnergyTransferRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnNotifyAllowedEnergyTransferRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyAllowedEnergyTransferResponse(
                                       request,
                                       NotifyAllowedEnergyTransferStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnNotifyAllowedEnergyTransferRequestFiltered event

            await LogEvent(
                      OnNotifyAllowedEnergyTransferRequestFiltered,
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


            #region Attach OnNotifyAllowedEnergyTransferRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnNotifyAllowedEnergyTransferRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnNotifyAllowedEnergyTransferRequestSent,
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
