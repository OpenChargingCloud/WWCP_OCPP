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
    /// A GetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>>

        OnGetDefaultChargingTariffRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        GetDefaultChargingTariffRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered GetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          GetDefaultChargingTariffRequest                                                         Request,
                                                          RequestForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>   ForwardingDecision,
                                                          CancellationToken                                                                       CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetDefaultChargingTariffRequestReceivedDelegate?    OnGetDefaultChargingTariffRequestReceived;
        public event OnGetDefaultChargingTariffRequestFilterDelegate?      OnGetDefaultChargingTariffRequestFilter;
        public event OnGetDefaultChargingTariffRequestFilteredDelegate?    OnGetDefaultChargingTariffRequestFiltered;
        public event OnGetDefaultChargingTariffRequestSentDelegate?        OnGetDefaultChargingTariffRequestSent;

        public event OnGetDefaultChargingTariffResponseReceivedDelegate?   OnGetDefaultChargingTariffResponseReceived;
        public event OnGetDefaultChargingTariffResponseSentDelegate?       OnGetDefaultChargingTariffResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_GetDefaultChargingTariff(OCPP_JSONRequestMessage    JSONRequestMessage,
                                             OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                             IWebSocketConnection       WebSocketConnection,
                                             CancellationToken          CancellationToken   = default)

        {

            #region Parse the GetDefaultChargingTariff request

            if (!GetDefaultChargingTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.Destination,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          JSONRequestMessage.RequestTimestamp,
                                                          JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                          JSONRequestMessage.EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetDefaultChargingTariffRequestReceived event

            await LogEvent(
                      OnGetDefaultChargingTariffRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetDefaultChargingTariffRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetDefaultChargingTariffRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetDefaultChargingTariffResponse(
                                       request,
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
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
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetDefaultChargingTariffRequestFiltered event

            await LogEvent(
                      OnGetDefaultChargingTariffRequestFiltered,
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


            #region Attach OnGetDefaultChargingTariffRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetDefaultChargingTariffRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetDefaultChargingTariffRequestSent,
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
