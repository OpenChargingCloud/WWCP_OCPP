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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>>

        OnNotifyEVChargingScheduleRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        NotifyEVChargingScheduleRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEVChargingScheduleRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          NotifyEVChargingScheduleRequest                                                         Request,
                                                          ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyEVChargingScheduleRequestReceivedDelegate?    OnNotifyEVChargingScheduleRequestReceived;
        public event OnNotifyEVChargingScheduleRequestFilterDelegate?      OnNotifyEVChargingScheduleRequestFilter;
        public event OnNotifyEVChargingScheduleRequestFilteredDelegate?    OnNotifyEVChargingScheduleRequestFiltered;
        public event OnNotifyEVChargingScheduleRequestSentDelegate?        OnNotifyEVChargingScheduleRequestSent;

        public event OnNotifyEVChargingScheduleResponseReceivedDelegate?   OnNotifyEVChargingScheduleResponseReceived;
        public event OnNotifyEVChargingScheduleResponseSentDelegate?       OnNotifyEVChargingScheduleResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_NotifyEVChargingSchedule(OCPP_JSONRequestMessage  JSONRequestMessage,
                                             IWebSocketConnection     Connection,
                                             CancellationToken        CancellationToken   = default)

        {

            if (!NotifyEVChargingScheduleRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.DestinationId,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>? forwardingDecision = null;


            #region Send OnNotifyEVChargingScheduleRequestReceived event

            var receivedLogging = OnNotifyEVChargingScheduleRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnNotifyEVChargingScheduleRequestReceivedDelegate>().
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
                                nameof(OnNotifyEVChargingScheduleRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnNotifyEVChargingScheduleRequestFilter event

            var requestFilter = OnNotifyEVChargingScheduleRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnNotifyEVChargingScheduleRequestFilterDelegate>().
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
                              nameof(OnNotifyEVChargingScheduleRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyEVChargingScheduleResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnNotifyEVChargingScheduleRequestFiltered event

            var logger = OnNotifyEVChargingScheduleRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnNotifyEVChargingScheduleRequestFilteredDelegate>().
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
                              nameof(OnNotifyEVChargingScheduleRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnNotifyEVChargingScheduleRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifyEVChargingScheduleRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnNotifyEVChargingScheduleRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnNotifyEVChargingScheduleRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(

                                                        parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,

                                                        parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                                                        parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                                                        parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                                                        parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                                                        parentNetworkingNode.OCPP.CustomCostSerializer,

                                                        parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                                                        parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                                                        parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer

                                                    );

            return forwardingDecision;

        }

    }

}
