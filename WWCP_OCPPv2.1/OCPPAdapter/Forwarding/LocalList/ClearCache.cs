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
    /// A ClearCache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearCacheRequest, ClearCacheResponse>>

        OnClearCacheRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          ClearCacheRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ClearCache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearCacheRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            ClearCacheRequest                                           Request,
                                            ForwardingDecision<ClearCacheRequest, ClearCacheResponse>   ForwardingDecision,
                                            CancellationToken                                           CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnClearCacheRequestReceivedDelegate?    OnClearCacheRequestReceived;
        public event OnClearCacheRequestFilterDelegate?      OnClearCacheRequestFilter;
        public event OnClearCacheRequestFilteredDelegate?    OnClearCacheRequestFiltered;
        public event OnClearCacheRequestSentDelegate?        OnClearCacheRequestSent;

        public event OnClearCacheResponseReceivedDelegate?   OnClearCacheResponseReceived;
        public event OnClearCacheResponseSentDelegate?       OnClearCacheResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_ClearCache(OCPP_JSONRequestMessage  JSONRequestMessage,
                               IWebSocketConnection     WebSocketConnection,
                               CancellationToken        CancellationToken   = default)

        {

            #region Parse the ClearCache request

            if (!ClearCacheRequest.TryParse(JSONRequestMessage.Payload,
                                            JSONRequestMessage.RequestId,
                                            JSONRequestMessage.DestinationId,
                                            JSONRequestMessage.NetworkPath,
                                            out var request,
                                            out var errorResponse,
                                            JSONRequestMessage.RequestTimestamp,
                                            JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                            JSONRequestMessage.EventTrackingId,
                                            parentNetworkingNode.OCPP.CustomClearCacheRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnClearCacheRequestReceived event

            await LogEvent(
                      OnClearCacheRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnClearCacheRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnClearCacheRequestFilter,
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
                forwardingDecision = new ForwardingDecision<ClearCacheRequest, ClearCacheResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ClearCacheResponse(
                                       request,
                                       ClearCacheStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<ClearCacheRequest, ClearCacheResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomClearCacheResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomClearCacheRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnClearCacheRequestFiltered event

            await LogEvent(
                      OnClearCacheRequestFiltered,
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


            #region Attach OnClearCacheRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnClearCacheRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnClearCacheRequestSent,
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
