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

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SetDefaultTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<SetDefaultTariffRequest, SetDefaultTariffResponse>>

        OnSetDefaultTariffRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                SetDefaultTariffRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered SetDefaultTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnSetDefaultTariffRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  SetDefaultTariffRequest                                                 Request,
                                                  RequestForwardingDecision<SetDefaultTariffRequest, SetDefaultTariffResponse>   ForwardingDecision,
                                                  CancellationToken                                                       CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSetDefaultTariffRequestReceivedDelegate?    OnSetDefaultTariffRequestReceived;
        public event OnSetDefaultTariffRequestFilterDelegate?      OnSetDefaultTariffRequestFilter;
        public event OnSetDefaultTariffRequestFilteredDelegate?    OnSetDefaultTariffRequestFiltered;
        public event OnSetDefaultTariffRequestSentDelegate?        OnSetDefaultTariffRequestSent;

        public event OnSetDefaultTariffResponseReceivedDelegate?   OnSetDefaultTariffResponseReceived;
        public event OnSetDefaultTariffResponseSentDelegate?       OnSetDefaultTariffResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_SetDefaultTariff(OCPP_JSONRequestMessage    JSONRequestMessage,
                                     OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                     IWebSocketConnection       WebSocketConnection,
                                     CancellationToken          CancellationToken   = default)

        {

            #region Parse the SetDefaultTariff request

            if (!SetDefaultTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                  JSONRequestMessage.RequestId,
                                                  JSONRequestMessage.Destination,
                                                  JSONRequestMessage.NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  JSONRequestMessage.RequestTimestamp,
                                                  JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                  JSONRequestMessage.EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnSetDefaultTariffRequestReceived event

            await LogEvent(
                      OnSetDefaultTariffRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnSetDefaultTariffRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnSetDefaultTariffRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<SetDefaultTariffRequest, SetDefaultTariffResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SetDefaultTariffResponse(
                                       request,
                                       TariffStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<SetDefaultTariffRequest, SetDefaultTariffResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffSerializer,
                                                        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceSerializer,
                                                        parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffConditionsSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffEnergySerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffEnergyPriceSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffTimeSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffTimePriceSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffFixedSerializer,
                                                        parentNetworkingNode.OCPP.CustomTariffFixedPriceSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnSetDefaultTariffRequestFiltered event

            await LogEvent(
                      OnSetDefaultTariffRequestFiltered,
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


            #region Attach OnSetDefaultTariffRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnSetDefaultTariffRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnSetDefaultTariffRequestSent,
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
