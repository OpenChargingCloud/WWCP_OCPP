﻿/*
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
    /// A ClearDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>>

        OnClearDisplayMessageRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   ClearDisplayMessageRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered ClearDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnClearDisplayMessageRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     ClearDisplayMessageRequest                                                    Request,
                                                     RequestForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>   ForwardingDecision,
                                                     CancellationToken                                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnClearDisplayMessageRequestReceivedDelegate?    OnClearDisplayMessageRequestReceived;
        public event OnClearDisplayMessageRequestFilterDelegate?      OnClearDisplayMessageRequestFilter;
        public event OnClearDisplayMessageRequestFilteredDelegate?    OnClearDisplayMessageRequestFiltered;
        public event OnClearDisplayMessageRequestSentDelegate?        OnClearDisplayMessageRequestSent;

        public event OnClearDisplayMessageResponseReceivedDelegate?   OnClearDisplayMessageResponseReceived;
        public event OnClearDisplayMessageResponseSentDelegate?       OnClearDisplayMessageResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_ClearDisplayMessage(OCPP_JSONRequestMessage    JSONRequestMessage,
                                        OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                        IWebSocketConnection       WebSocketConnection,
                                        CancellationToken          CancellationToken   = default)

        {

            #region Parse the ClearDisplayMessage request

            if (!ClearDisplayMessageRequest.TryParse(JSONRequestMessage.Payload,
                                                     JSONRequestMessage.RequestId,
                                                     JSONRequestMessage.Destination,
                                                     JSONRequestMessage.NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     JSONRequestMessage.RequestTimestamp,
                                                     JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     parentNetworkingNode.OCPP.CustomClearDisplayMessageRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnClearDisplayMessageRequestReceived event

            await LogEvent(
                      OnClearDisplayMessageRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnClearDisplayMessageRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnClearDisplayMessageRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ClearDisplayMessageResponse(
                                       request,
                                       ClearMessageStatus.Unknown,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomClearDisplayMessageResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomClearDisplayMessageRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnClearDisplayMessageRequestFiltered event

            await LogEvent(
                      OnClearDisplayMessageRequestFiltered,
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


            #region Attach OnClearDisplayMessageRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnClearDisplayMessageRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnClearDisplayMessageRequestSent,
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
