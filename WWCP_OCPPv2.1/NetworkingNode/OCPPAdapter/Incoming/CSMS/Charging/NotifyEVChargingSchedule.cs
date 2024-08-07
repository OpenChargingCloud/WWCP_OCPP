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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestReceivedDelegate?  OnNotifyEVChargingScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received for processing.
        /// </summary>
        public event OnNotifyEVChargingScheduleDelegate?                 OnNotifyEVChargingSchedule;

        #endregion

        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_NotifyEVChargingSchedule(DateTime              RequestTimestamp,
                                             IWebSocketConnection  WebSocketConnection,
                                             NetworkingNode_Id     DestinationId,
                                             NetworkPath           NetworkPath,
                                             EventTracking_Id      EventTrackingId,
                                             Request_Id            RequestId,
                                             JObject               JSONRequest,
                                             CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (NotifyEVChargingScheduleRequest.TryParse(JSONRequest,
                                                             RequestId,
                                                             DestinationId,
                                                             NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             RequestTimestamp,
                                                             parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                             EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestParser)) {

                    NotifyEVChargingScheduleResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(

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

                        ),
                        out errorResponse))
                    {

                        response = NotifyEVChargingScheduleResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyEVChargingScheduleRequestReceived event

                    var logger = OnNotifyEVChargingScheduleRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnNotifyEVChargingScheduleRequestReceivedDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request
                                                                             )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnNotifyEVChargingScheduleRequestReceived),
                                      e
                                  );
                        }
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnNotifyEVChargingSchedule?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnNotifyEVChargingScheduleDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : NotifyEVChargingScheduleResponse.Failed(request, $"Undefined {nameof(OnNotifyEVChargingSchedule)}!");

                        }
                        catch (Exception e)
                        {

                            response = NotifyEVChargingScheduleResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnNotifyEVChargingSchedule),
                                      e
                                  );

                        }
                    }

                    response ??= NotifyEVChargingScheduleResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyEVChargingScheduleResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnNotifyEVChargingScheduleResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomNotifyEVChargingScheduleResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_NotifyEVChargingSchedule)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyEVChargingSchedule)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseSentDelegate?  OnNotifyEVChargingScheduleResponseSent;

        #endregion

        #region Send OnNotifyEVChargingScheduleResponse event

        public async Task SendOnNotifyEVChargingScheduleResponseSent(DateTime                 Timestamp,
                                                            IEventSender             Sender,
                                                            IWebSocketConnection     Connection,
                                                            NotifyEVChargingScheduleRequest   Request,
                                                            NotifyEVChargingScheduleResponse  Response,
                                                            TimeSpan                 Runtime)
        {

            var logger = OnNotifyEVChargingScheduleResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnNotifyEVChargingScheduleResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnNotifyEVChargingScheduleResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
