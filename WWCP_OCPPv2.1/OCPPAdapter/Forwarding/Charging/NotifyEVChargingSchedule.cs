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
    /// A NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>>

        OnNotifyEVChargingScheduleRequestFilterDelegate(DateTimeOffset                    Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        NotifyEVChargingScheduleRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEVChargingScheduleRequestFilteredDelegate(DateTimeOffset                                                                          Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          NotifyEVChargingScheduleRequest                                                         Request,
                                                          RequestForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>   ForwardingDecision,
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

        public async Task<RequestForwardingDecision>

            Forward_NotifyEVChargingSchedule(OCPP_JSONRequestMessage    JSONRequestMessage,
                                             OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                             IWebSocketConnection       WebSocketConnection,
                                             CancellationToken          CancellationToken   = default)

        {

            #region Parse the NotifyEVChargingSchedule request

            if (!NotifyEVChargingScheduleRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.Destination,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          JSONRequestMessage.RequestTimestamp,
                                                          JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                          JSONRequestMessage.EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
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

            var forwardingDecision = await CallFilter(
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

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = new RequestForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyEVChargingScheduleResponse(
                                       request,
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
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

                                                        false,

                                                        parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,

                                                        parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomLimitAtSoCSerializer,
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

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
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
