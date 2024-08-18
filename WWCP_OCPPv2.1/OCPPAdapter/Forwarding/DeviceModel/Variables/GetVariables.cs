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
using Org.BouncyCastle.Asn1.Ocsp;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A GetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetVariablesRequest, GetVariablesResponse>>

        OnGetVariablesRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            GetVariablesRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetVariablesRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              GetVariablesRequest                                             Request,
                                              ForwardingDecision<GetVariablesRequest, GetVariablesResponse>   ForwardingDecision,
                                              CancellationToken                                               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetVariablesRequestReceivedDelegate?    OnGetVariablesRequestReceived;
        public event OnGetVariablesRequestFilterDelegate?      OnGetVariablesRequestFilter;
        public event OnGetVariablesRequestFilteredDelegate?    OnGetVariablesRequestFiltered;
        public event OnGetVariablesRequestSentDelegate?        OnGetVariablesRequestSent;

        public event OnGetVariablesResponseReceivedDelegate?   OnGetVariablesResponseReceived;
        public event OnGetVariablesResponseSentDelegate?       OnGetVariablesResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetVariables(OCPP_JSONRequestMessage    JSONRequestMessage,
                                 OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                 IWebSocketConnection       WebSocketConnection,
                                 CancellationToken          CancellationToken   = default)

        {

            #region Parse the Authorize request

            if (!GetVariablesRequest.TryParse(JSONRequestMessage.Payload,
                                              JSONRequestMessage.RequestId,
                                              JSONRequestMessage.Destination,
                                              JSONRequestMessage.NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              JSONRequestMessage.RequestTimestamp,
                                              JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                              JSONRequestMessage.EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomGetVariablesRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetVariablesRequestReceived event

            await LogEvent(
                      OnGetVariablesRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetVariablesRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetVariablesRequestFilter,
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
                forwardingDecision = new ForwardingDecision<GetVariablesRequest, GetVariablesResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetVariablesResponse(
                                       request,
                                       [],
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetVariablesRequest, GetVariablesResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetVariablesResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomGetVariableResultSerializer,
                                             parentNetworkingNode.OCPP.CustomComponentSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSESerializer,
                                             parentNetworkingNode.OCPP.CustomVariableSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetVariablesRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomGetVariableDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetVariablesRequestFiltered event

            await LogEvent(
                      OnGetVariablesRequestFiltered,
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


            #region Attach OnGetVariablesRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetVariablesRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetVariablesRequestSent,
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
