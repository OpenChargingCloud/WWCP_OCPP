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

    #region OnGetCompositeScheduleRequestFilter(ed)Delegate

    /// <summary>
    /// A GetCompositeSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>>

        OnGetCompositeScheduleRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    GetCompositeScheduleRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered GetCompositeSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetCompositeScheduleRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      GetCompositeScheduleRequest                                                     Request,
                                                      ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetCompositeScheduleRequestReceivedDelegate?    OnGetCompositeScheduleRequestReceived;
        public event OnGetCompositeScheduleRequestFilterDelegate?      OnGetCompositeScheduleRequestFilter;
        public event OnGetCompositeScheduleRequestFilteredDelegate?    OnGetCompositeScheduleRequestFiltered;
        public event OnGetCompositeScheduleRequestSentDelegate?        OnGetCompositeScheduleRequestSent;

        public event OnGetCompositeScheduleResponseReceivedDelegate?   OnGetCompositeScheduleResponseReceived;
        public event OnGetCompositeScheduleResponseSentDelegate?       OnGetCompositeScheduleResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetCompositeSchedule(OCPP_JSONRequestMessage  JSONRequestMessage,
                                         IWebSocketConnection     Connection,
                                         CancellationToken        CancellationToken   = default)

        {

            if (!GetCompositeScheduleRequest.TryParse(JSONRequestMessage.Payload,
                                                      JSONRequestMessage.RequestId,
                                                      JSONRequestMessage.DestinationId,
                                                      JSONRequestMessage.NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                      JSONRequestMessage.EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomGetCompositeScheduleRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>? forwardingDecision = null;

            #region Send OnGetCompositeScheduleRequestReceived event

            var receivedLogging = OnGetCompositeScheduleRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnGetCompositeScheduleRequestReceivedDelegate>().
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
                              nameof(OnGetCompositeScheduleRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnGetCompositeScheduleRequestFilter event

            var requestFilter = OnGetCompositeScheduleRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnGetCompositeScheduleRequestFilterDelegate>().
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
                              nameof(OnGetCompositeScheduleRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetCompositeScheduleResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomCompositeScheduleSerializer,
                                             parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetCompositeScheduleRequestFiltered event

            var logger = OnGetCompositeScheduleRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnGetCompositeScheduleRequestFilteredDelegate>().
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
                              nameof(OnGetCompositeScheduleRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnGetCompositeScheduleRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGetCompositeScheduleRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnGetCompositeScheduleRequestSentDelegate>().
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
                                      nameof(OnGetCompositeScheduleRequestSent),
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
