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
                                                          ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>   ForwardingDecision,
                                                          CancellationToken                                                                       CancellationToken);

    #endregion

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
                                             IWebSocketConnection     WebSocketConnection,
                                             CancellationToken        CancellationToken   = default)

        {

            #region Parse the NotifyEVChargingSchedule request

            if (!NotifyEVChargingScheduleRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.DestinationId,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          JSONRequestMessage.RequestTimestamp,
                                                          JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                          JSONRequestMessage.EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnNotifyEVChargingScheduleRequestReceived event

            await LogEvent(
                      OnNotifyEVChargingScheduleRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnNotifyEVChargingScheduleRequestFilter event

            ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>? forwardingDecision = null;

            forwardingDecision = await CallFilter(
                                           OnNotifyEVChargingScheduleRequestFilter,
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
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
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

            #region Send OnNotifyEVChargingScheduleRequestFiltered event

            await LogEvent(
                      OnNotifyEVChargingScheduleRequestFiltered,
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


            #region Attach OnNotifyEVChargingScheduleRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifyEVChargingScheduleRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnNotifyEVChargingScheduleRequestSent,
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
