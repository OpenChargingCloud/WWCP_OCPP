﻿/*
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
    /// A SetChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>>

        OnSetChargingProfileRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  SetChargingProfileRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered SetChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetChargingProfileRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    SetChargingProfileRequest                                                   Request,
                                                    ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSetChargingProfileRequestReceivedDelegate?    OnSetChargingProfileRequestReceived;
        public event OnSetChargingProfileRequestFilterDelegate?      OnSetChargingProfileRequestFilter;
        public event OnSetChargingProfileRequestFilteredDelegate?    OnSetChargingProfileRequestFiltered;
        public event OnSetChargingProfileRequestSentDelegate?        OnSetChargingProfileRequestSent;

        public event OnSetChargingProfileResponseReceivedDelegate?   OnSetChargingProfileResponseReceived;
        public event OnSetChargingProfileResponseSentDelegate?       OnSetChargingProfileResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SetChargingProfile(OCPP_JSONRequestMessage  JSONRequestMessage,
                                       IWebSocketConnection     Connection,
                                       CancellationToken        CancellationToken   = default)

        {

            if (!SetChargingProfileRequest.TryParse(JSONRequestMessage.Payload,
                                                    JSONRequestMessage.RequestId,
                                                    JSONRequestMessage.DestinationId,
                                                    JSONRequestMessage.NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    JSONRequestMessage.RequestTimestamp,
                                                    JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                    JSONRequestMessage.EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomSetChargingProfileRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>? forwardingDecision = null;

            #region Send OnSetChargingProfileRequestReceived event

            var receivedLogging = OnSetChargingProfileRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnSetChargingProfileRequestReceivedDelegate>().
                                  Select(filterDelegate => filterDelegate.Invoke(
                                                               Timestamp.Now,
                                                               parentNetworkingNode,
                                                               Connection,
                                                               request
                                                           )).
                                  ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetChargingProfileRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnSetChargingProfileRequestFilter event

            var requestFilter = OnSetChargingProfileRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnSetChargingProfileRequestFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             Connection,
                                                                             request,
                                                                             CancellationToken
                                                                         )).
                                                ToArray()
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetChargingProfileRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SetChargingProfileResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSetChargingProfileResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(

                                                        parentNetworkingNode.OCPP.CustomSetChargingProfileRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                                                        parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
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

            #region Send OnSetChargingProfileRequestFiltered event

            var logger = OnSetChargingProfileRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnSetChargingProfileRequestFilteredDelegate>().
                                  Select(loggingDelegate => loggingDelegate.Invoke(
                                                                Timestamp.Now,
                                                                parentNetworkingNode,
                                                                Connection,
                                                                request,
                                                                forwardingDecision
                                                            )).
                                  ToArray()
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetChargingProfileRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnSetChargingProfileRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnSetChargingProfileRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnSetChargingProfileRequestSentDelegate>().
                                          Select(filterDelegate => filterDelegate.Invoke(
                                                                       Timestamp.Now,
                                                                       parentNetworkingNode,
                                                                       sentMessageResult.Connection,
                                                                       request,
                                                                       sentMessageResult.Result
                                                                   )).
                                          ToArray()
                                  );

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(NetworkingNode),
                                      nameof(OnSetChargingProfileRequestSent),
                                      e
                                  );
                        }

                    };

            }

            #endregion

            return forwardingDecision;

        }

    }

}
