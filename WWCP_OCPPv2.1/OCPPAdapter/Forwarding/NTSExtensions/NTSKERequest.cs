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
    /// An NTSKE request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<NTSKERequest, NTSKEResponse>>

        OnNTSKERequestFilterDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     NTSKERequest           Request,
                                     CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered NTSKE request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnNTSKERequestFilteredDelegate(DateTime                                                 Timestamp,
                                       IEventSender                                             Sender,
                                       IWebSocketConnection                                     Connection,
                                       NTSKERequest                                             Request,
                                       RequestForwardingDecision<NTSKERequest, NTSKEResponse>   ForwardingDecision,
                                       CancellationToken                                        CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNTSKERequestReceivedDelegate?    OnNTSKERequestReceived;
        public event OnNTSKERequestFilterDelegate?      OnNTSKERequestFilter;
        public event OnNTSKERequestFilteredDelegate?    OnNTSKERequestFiltered;
        public event OnNTSKERequestSentDelegate?        OnNTSKERequestSent;

        public event OnNTSKEResponseReceivedDelegate?   OnNTSKEResponseReceived;
        public event OnNTSKEResponseSentDelegate?       OnNTSKEResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_NTSKE(OCPP_JSONRequestMessage    JSONRequestMessage,
                          OCPP_BinaryRequestMessage  BinaryRequestMessage,
                          IWebSocketConnection       WebSocketConnection,
                          CancellationToken          CancellationToken   = default)

        {

            #region Parse the NTSKE request

            if (!NTSKERequest.TryParse(JSONRequestMessage.Payload,
                                       JSONRequestMessage.RequestId,
                                       JSONRequestMessage.Destination,
                                       JSONRequestMessage.NetworkPath,
                                       out var request,
                                       out var errorResponse,
                                       JSONRequestMessage.RequestTimestamp,
                                       JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                       JSONRequestMessage.EventTrackingId,
                                       parentNetworkingNode.OCPP.CustomNTSKERequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnNTSKERequestReceived event

            await LogEvent(
                      OnNTSKERequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnNTSKERequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnNTSKERequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<NTSKERequest, NTSKEResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NTSKEResponse(
                                       request,
                                       [],
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<NTSKERequest, NTSKEResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomNTSKEServerInfoSerializer,
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
                                                        parentNetworkingNode.OCPP.CustomNTSKERequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnNTSKERequestFiltered event

            await LogEvent(
                      OnNTSKERequestFiltered,
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


            #region Attach OnNTSKERequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnNTSKERequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnNTSKERequestSent,
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
