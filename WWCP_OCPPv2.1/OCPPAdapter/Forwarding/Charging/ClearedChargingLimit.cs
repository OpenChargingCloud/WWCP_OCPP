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
    /// A ClearedChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>>

        OnClearedChargingLimitRequestFilterDelegate(DateTimeOffset                Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    ClearedChargingLimitRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered ClearedChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnClearedChargingLimitRequestFilteredDelegate(DateTimeOffset                                                                  Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      ClearedChargingLimitRequest                                                     Request,
                                                      RequestForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>   ForwardingDecision,
                                                      CancellationToken                                                               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnClearedChargingLimitRequestReceivedDelegate?    OnClearedChargingLimitRequestReceived;
        public event OnClearedChargingLimitRequestFilterDelegate?      OnClearedChargingLimitRequestFilter;
        public event OnClearedChargingLimitRequestFilteredDelegate?    OnClearedChargingLimitRequestFiltered;
        public event OnClearedChargingLimitRequestSentDelegate?        OnClearedChargingLimitRequestSent;

        public event OnClearedChargingLimitResponseReceivedDelegate?   OnClearedChargingLimitResponseReceived;
        public event OnClearedChargingLimitResponseSentDelegate?       OnClearedChargingLimitResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_ClearedChargingLimit(OCPP_JSONRequestMessage    JSONRequestMessage,
                                         OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                         IWebSocketConnection       WebSocketConnection,
                                         CancellationToken          CancellationToken   = default)

        {

            #region Parse the ClearedChargingLimit request

            if (!ClearedChargingLimitRequest.TryParse(JSONRequestMessage.Payload,
                                                      JSONRequestMessage.RequestId,
                                                      JSONRequestMessage.Destination,
                                                      JSONRequestMessage.NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                      JSONRequestMessage.EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomClearedChargingLimitRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnClearedChargingLimitRequestReceived event

            await LogEvent(
                      OnClearedChargingLimitRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnClearedChargingLimitRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnClearedChargingLimitRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ClearedChargingLimitResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomClearedChargingLimitResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomClearedChargingLimitRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnClearedChargingLimitRequestFiltered event

            await LogEvent(
                      OnClearedChargingLimitRequestFiltered,
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


            #region Attach OnClearedChargingLimitRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnClearedChargingLimitRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnClearedChargingLimitRequestSent,
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
