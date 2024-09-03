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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SetDefaultE2EChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<SetDefaultE2EChargingTariffRequest, SetDefaultE2EChargingTariffResponse>>

        OnSetDefaultE2EChargingTariffRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        SetDefaultE2EChargingTariffRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered SetDefaultE2EChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnSetDefaultE2EChargingTariffRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          SetDefaultE2EChargingTariffRequest                                                         Request,
                                                          RequestForwardingDecision<SetDefaultE2EChargingTariffRequest, SetDefaultE2EChargingTariffResponse>   ForwardingDecision,
                                                          CancellationToken                                                                       CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSetDefaultE2EChargingTariffRequestReceivedDelegate?    OnSetDefaultE2EChargingTariffRequestReceived;
        public event OnSetDefaultE2EChargingTariffRequestFilterDelegate?      OnSetDefaultE2EChargingTariffRequestFilter;
        public event OnSetDefaultE2EChargingTariffRequestFilteredDelegate?    OnSetDefaultE2EChargingTariffRequestFiltered;
        public event OnSetDefaultE2EChargingTariffRequestSentDelegate?        OnSetDefaultE2EChargingTariffRequestSent;

        public event OnSetDefaultE2EChargingTariffResponseReceivedDelegate?   OnSetDefaultE2EChargingTariffResponseReceived;
        public event OnSetDefaultE2EChargingTariffResponseSentDelegate?       OnSetDefaultE2EChargingTariffResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_SetDefaultE2EChargingTariff(OCPP_JSONRequestMessage    JSONRequestMessage,
                                                OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                IWebSocketConnection       WebSocketConnection,
                                                CancellationToken          CancellationToken   = default)

        {

            #region Parse the SetDefaultE2EChargingTariff request

            if (!SetDefaultE2EChargingTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.Destination,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          JSONRequestMessage.RequestTimestamp,
                                                          JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                          JSONRequestMessage.EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnSetDefaultE2EChargingTariffRequestReceived event

            await LogEvent(
                      OnSetDefaultE2EChargingTariffRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnSetDefaultE2EChargingTariffRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnSetDefaultE2EChargingTariffRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<SetDefaultE2EChargingTariffRequest, SetDefaultE2EChargingTariffResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SetDefaultE2EChargingTariffResponse(
                                       request,
                                       SetDefaultE2EChargingTariffStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<SetDefaultE2EChargingTariffRequest, SetDefaultE2EChargingTariffResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        //parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffRequestSerializer,
                                                        //parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
                                                        //parentNetworkingNode.OCPP.CustomPriceSerializer,
                                                        //parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                                                        //parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                                                        //parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                                                        //parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                                                        //parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                                                        //parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                                                        //parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                                                        //parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                        //parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                        //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnSetDefaultE2EChargingTariffRequestFiltered event

            await LogEvent(
                      OnSetDefaultE2EChargingTariffRequestFiltered,
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


            #region Attach OnSetDefaultE2EChargingTariffRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnSetDefaultE2EChargingTariffRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnSetDefaultE2EChargingTariffRequestSent,
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
