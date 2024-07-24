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
    /// A NotifyEVChargingNeeds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>>

        OnNotifyEVChargingNeedsRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     NotifyEVChargingNeedsRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered NotifyEVChargingNeeds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEVChargingNeedsRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       NotifyEVChargingNeedsRequest                                                      Request,
                                                       ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyEVChargingNeedsRequestReceivedDelegate?    OnNotifyEVChargingNeedsRequestReceived;
        public event OnNotifyEVChargingNeedsRequestFilterDelegate?      OnNotifyEVChargingNeedsRequestFilter;
        public event OnNotifyEVChargingNeedsRequestFilteredDelegate?    OnNotifyEVChargingNeedsRequestFiltered;
        public event OnNotifyEVChargingNeedsRequestSentDelegate?        OnNotifyEVChargingNeedsRequestSent;

        public event OnNotifyEVChargingNeedsResponseReceivedDelegate?   OnNotifyEVChargingNeedsResponseReceived;
        public event OnNotifyEVChargingNeedsResponseSentDelegate?       OnNotifyEVChargingNeedsResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_NotifyEVChargingNeeds(OCPP_JSONRequestMessage  JSONRequestMessage,
                                          IWebSocketConnection     Connection,
                                          CancellationToken        CancellationToken   = default)

        {

            if (!NotifyEVChargingNeedsRequest.TryParse(JSONRequestMessage.Payload,
                                                       JSONRequestMessage.RequestId,
                                                       JSONRequestMessage.DestinationId,
                                                       JSONRequestMessage.NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       JSONRequestMessage.RequestTimestamp,
                                                       JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                       JSONRequestMessage.EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>? forwardingDecision = null;


            #region Send OnNotifyEVChargingNeedsRequestReceived event

            var receivedLogging = OnNotifyEVChargingNeedsRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnNotifyEVChargingNeedsRequestReceivedDelegate>().
                                          Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                         parentNetworkingNode,
                                          Connection,
                                                                                         request)).
                                          ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                "NetworkingNode",
                                nameof(OnNotifyEVChargingNeedsRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnNotifyEVChargingNeedsRequestFilter event

            var requestFilter = OnNotifyEVChargingNeedsRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnNotifyEVChargingNeedsRequestFilterDelegate>().
                                                     Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                    parentNetworkingNode,
                                                                                                    Connection,
                                                                                                    request,
                                                                                                    CancellationToken)).
                                                     ToArray());

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnNotifyEVChargingNeedsRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyEVChargingNeedsResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyEVChargingNeedsResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnNotifyEVChargingNeedsRequestFiltered event

            var logger = OnNotifyEVChargingNeedsRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnNotifyEVChargingNeedsRequestFilteredDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        parentNetworkingNode,
                                                                                        Connection,
                                                                                        request,
                                                                                        forwardingDecision)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnNotifyEVChargingNeedsRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnNotifyEVChargingNeedsRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifyEVChargingNeedsRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnNotifyEVChargingNeedsRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request,
                                                SendMessageResult.Success)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnNotifyEVChargingNeedsRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingNeedsSerializer,
                                                        parentNetworkingNode.OCPP.CustomACChargingParametersSerializer,
                                                        parentNetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                                                        parentNetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
