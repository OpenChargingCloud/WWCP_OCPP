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
    /// A PullDynamicScheduleUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>>

        OnPullDynamicScheduleUpdateRequestFilterDelegate(DateTime                           Timestamp,
                                                         IEventSender                       Sender,
                                                         IWebSocketConnection               Connection,
                                                         PullDynamicScheduleUpdateRequest   Request,
                                                         CancellationToken                  CancellationToken);


    /// <summary>
    /// A filtered PullDynamicScheduleUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPullDynamicScheduleUpdateRequestFilteredDelegate(DateTime                                                                                  Timestamp,
                                                           IEventSender                                                                              Sender,
                                                           IWebSocketConnection                                                                      Connection,
                                                           PullDynamicScheduleUpdateRequest                                                          Request,
                                                           ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnPullDynamicScheduleUpdateRequestReceivedDelegate?    OnPullDynamicScheduleUpdateRequestReceived;
        public event OnPullDynamicScheduleUpdateRequestFilterDelegate?      OnPullDynamicScheduleUpdateRequestFilter;
        public event OnPullDynamicScheduleUpdateRequestFilteredDelegate?    OnPullDynamicScheduleUpdateRequestFiltered;
        public event OnPullDynamicScheduleUpdateRequestSentDelegate?        OnPullDynamicScheduleUpdateRequestSent;

        public event OnPullDynamicScheduleUpdateResponseReceivedDelegate?   OnPullDynamicScheduleUpdateResponseReceived;
        public event OnPullDynamicScheduleUpdateResponseSentDelegate?       OnPullDynamicScheduleUpdateResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_PullDynamicScheduleUpdate(OCPP_JSONRequestMessage  JSONRequestMessage,
                                              IWebSocketConnection     Connection,
                                              CancellationToken        CancellationToken   = default)

        {

            if (!PullDynamicScheduleUpdateRequest.TryParse(JSONRequestMessage.Payload,
                                                           JSONRequestMessage.RequestId,
                                                           JSONRequestMessage.DestinationId,
                                                           JSONRequestMessage.NetworkPath,
                                                           out var request,
                                                           out var errorResponse,
                                                           parentNetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>? forwardingDecision = null;


            #region Send OnPullDynamicScheduleUpdateRequestReceived event

            var receivedLogging = OnPullDynamicScheduleUpdateRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnPullDynamicScheduleUpdateRequestReceivedDelegate>().
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
                                nameof(OnPullDynamicScheduleUpdateRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnPullDynamicScheduleUpdateRequestFilter event

            var requestFilter = OnPullDynamicScheduleUpdateRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnPullDynamicScheduleUpdateRequestFilterDelegate>().
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
                              nameof(OnPullDynamicScheduleUpdateRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new PullDynamicScheduleUpdateResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomPullDynamicScheduleUpdateResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnPullDynamicScheduleUpdateRequestFiltered event

            var logger = OnPullDynamicScheduleUpdateRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnPullDynamicScheduleUpdateRequestFilteredDelegate>().
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
                              nameof(OnPullDynamicScheduleUpdateRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnPullDynamicScheduleUpdateRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnPullDynamicScheduleUpdateRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnPullDynamicScheduleUpdateRequestSentDelegate>().
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
                                    nameof(OnPullDynamicScheduleUpdateRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
