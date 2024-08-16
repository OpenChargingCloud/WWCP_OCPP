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
    /// A RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>>

        OnRemoveDefaultChargingTariffRequestFilterDelegate(DateTime                             Timestamp,
                                                           IEventSender                         Sender,
                                                           IWebSocketConnection                 Connection,
                                                           RemoveDefaultChargingTariffRequest   Request,
                                                           CancellationToken                    CancellationToken);


    /// <summary>
    /// A filtered RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                      Timestamp,
                                                             IEventSender                                                                                  Sender,
                                                             IWebSocketConnection                                                                          Connection,
                                                             RemoveDefaultChargingTariffRequest                                                            Request,
                                                             ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>   ForwardingDecision,
                                                             CancellationToken                                                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnRemoveDefaultChargingTariffRequestReceivedDelegate?    OnRemoveDefaultChargingTariffRequestReceived;
        public event OnRemoveDefaultChargingTariffRequestFilterDelegate?      OnRemoveDefaultChargingTariffRequestFilter;
        public event OnRemoveDefaultChargingTariffRequestFilteredDelegate?    OnRemoveDefaultChargingTariffRequestFiltered;
        public event OnRemoveDefaultChargingTariffRequestSentDelegate?        OnRemoveDefaultChargingTariffRequestSent;

        public event OnRemoveDefaultChargingTariffResponseReceivedDelegate?   OnRemoveDefaultChargingTariffResponseReceived;
        public event OnRemoveDefaultChargingTariffResponseSentDelegate?       OnRemoveDefaultChargingTariffResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_RemoveDefaultChargingTariff(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                IWebSocketConnection     WebSocketConnection,
                                                CancellationToken        CancellationToken   = default)

        {

            #region Parse the RemoveDefaultChargingTariff request

            if (!RemoveDefaultChargingTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                             JSONRequestMessage.RequestId,
                                                             JSONRequestMessage.Destination,
                                                             JSONRequestMessage.NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             JSONRequestMessage.RequestTimestamp,
                                                             JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                             JSONRequestMessage.EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnRemoveDefaultChargingTariffRequestReceived event

            await LogEvent(
                      OnRemoveDefaultChargingTariffRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnRemoveDefaultChargingTariffRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnRemoveDefaultChargingTariffRequestFilter,
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
                forwardingDecision = new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new RemoveDefaultChargingTariffResponse(
                                       request,
                                       RemoveDefaultChargingTariffStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer2,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnRemoveDefaultChargingTariffRequestFiltered event

            await LogEvent(
                      OnRemoveDefaultChargingTariffRequestFiltered,
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


            #region Attach OnRemoveDefaultChargingTariffRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnRemoveDefaultChargingTariffRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnRemoveDefaultChargingTariffRequestSent,
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
