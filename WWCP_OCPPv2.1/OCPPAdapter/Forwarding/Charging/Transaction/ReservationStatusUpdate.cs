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
    /// A ReservationStatusUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>>

        OnReservationStatusUpdateRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       ReservationStatusUpdateRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered ReservationStatusUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnReservationStatusUpdateRequestFilteredDelegate(DateTime                                                                              Timestamp,
                                                         IEventSender                                                                          Sender,
                                                         IWebSocketConnection                                                                  Connection,
                                                         ReservationStatusUpdateRequest                                                        Request,
                                                         ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>   ForwardingDecision,
                                                         CancellationToken                                                                     CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnReservationStatusUpdateRequestReceivedDelegate?    OnReservationStatusUpdateRequestReceived;
        public event OnReservationStatusUpdateRequestFilterDelegate?      OnReservationStatusUpdateRequestFilter;
        public event OnReservationStatusUpdateRequestFilteredDelegate?    OnReservationStatusUpdateRequestFiltered;
        public event OnReservationStatusUpdateRequestSentDelegate?        OnReservationStatusUpdateRequestSent;

        public event OnReservationStatusUpdateResponseReceivedDelegate?   OnReservationStatusUpdateResponseReceived;
        public event OnReservationStatusUpdateResponseSentDelegate?       OnReservationStatusUpdateResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_ReservationStatusUpdate(OCPP_JSONRequestMessage  JSONRequestMessage,
                                            IWebSocketConnection     WebSocketConnection,
                                            CancellationToken        CancellationToken   = default)

        {

            #region Parse the ReservationStatusUpdate request

            if (!ReservationStatusUpdateRequest.TryParse(JSONRequestMessage.Payload,
                                                         JSONRequestMessage.RequestId,
                                                         JSONRequestMessage.Destination,
                                                         JSONRequestMessage.NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         JSONRequestMessage.RequestTimestamp,
                                                         JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                         JSONRequestMessage.EventTrackingId,
                                                         parentNetworkingNode.OCPP.CustomReservationStatusUpdateRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnReservationStatusUpdateRequestReceived event

            await LogEvent(
                      OnReservationStatusUpdateRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnReservationStatusUpdateRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnReservationStatusUpdateRequestFilter,
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
                forwardingDecision = new ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ReservationStatusUpdateResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomReservationStatusUpdateResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnReservationStatusUpdateRequestFiltered event

            await LogEvent(
                      OnReservationStatusUpdateRequestFiltered,
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


            #region Attach OnReservationStatusUpdateRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnReservationStatusUpdateRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnReservationStatusUpdateRequestSent,
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
