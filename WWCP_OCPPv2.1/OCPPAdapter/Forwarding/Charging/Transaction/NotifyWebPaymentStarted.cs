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
    /// A NotifyWebPaymentStarted request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<NotifyWebPaymentStartedRequest, NotifyWebPaymentStartedResponse>>

        OnNotifyWebPaymentStartedRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       NotifyWebPaymentStartedRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered NotifyWebPaymentStarted request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnNotifyWebPaymentStartedRequestFilteredDelegate(DateTime                                                                                     Timestamp,
                                                         IEventSender                                                                                 Sender,
                                                         IWebSocketConnection                                                                         Connection,
                                                         NotifyWebPaymentStartedRequest                                                               Request,
                                                         RequestForwardingDecision<NotifyWebPaymentStartedRequest, NotifyWebPaymentStartedResponse>   ForwardingDecision,
                                                         CancellationToken                                                                            CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyWebPaymentStartedRequestReceivedDelegate?    OnNotifyWebPaymentStartedRequestReceived;
        public event OnNotifyWebPaymentStartedRequestFilterDelegate?      OnNotifyWebPaymentStartedRequestFilter;
        public event OnNotifyWebPaymentStartedRequestFilteredDelegate?    OnNotifyWebPaymentStartedRequestFiltered;
        public event OnNotifyWebPaymentStartedRequestSentDelegate?        OnNotifyWebPaymentStartedRequestSent;

        public event OnNotifyWebPaymentStartedResponseReceivedDelegate?   OnNotifyWebPaymentStartedResponseReceived;
        public event OnNotifyWebPaymentStartedResponseSentDelegate?       OnNotifyWebPaymentStartedResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_NotifyWebPaymentStarted(OCPP_JSONRequestMessage    JSONRequestMessage,
                                            OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                            IWebSocketConnection       WebSocketConnection,
                                            CancellationToken          CancellationToken   = default)

        {

            #region Parse the NotifyWebPaymentStarted request

            if (!NotifyWebPaymentStartedRequest.TryParse(JSONRequestMessage.Payload,
                                               JSONRequestMessage.RequestId,
                                               JSONRequestMessage.Destination,
                                               JSONRequestMessage.NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               JSONRequestMessage.RequestTimestamp,
                                               JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                               JSONRequestMessage.EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomNotifyWebPaymentStartedRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnNotifyWebPaymentStartedRequestReceived event

            await LogEvent(
                      OnNotifyWebPaymentStartedRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Call OnNotifyWebPaymentStartedRequest filter

            var forwardingDecision = await CallFilter(
                                               OnNotifyWebPaymentStartedRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<NotifyWebPaymentStartedRequest, NotifyWebPaymentStartedResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var rejectResponse  = forwardingDecision?.RejectResponse ??
                                          new NotifyWebPaymentStartedResponse(
                                              request,
                                              Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                          );

                forwardingDecision  = new RequestForwardingDecision<NotifyWebPaymentStartedRequest, NotifyWebPaymentStartedResponse>(
                                          request,
                                          ForwardingDecisions.REJECT,
                                          rejectResponse,
                                          rejectResponse.ToJSON(
                                              false,
                                              parentNetworkingNode.OCPP.CustomNotifyWebPaymentStartedResponseSerializer,
                                              parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                              parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                          ),
                                          forwardingDecision?.RejectMessage,
                                          forwardingDecision?.RejectDetails,
                                          forwardingDecision?.LogMessage
                                      );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomNotifyWebPaymentStartedRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnNotifyWebPaymentStartedRequestFiltered event

            await LogEvent(
                      OnNotifyWebPaymentStartedRequestFiltered,
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


            #region Attach OnNotifyWebPaymentStartedRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnNotifyWebPaymentStartedRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnNotifyWebPaymentStartedRequestSent,
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
