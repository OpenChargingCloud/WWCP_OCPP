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
    /// A NotifySettlement request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifySettlementRequest, NotifySettlementResponse>>

        OnNotifySettlementRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                NotifySettlementRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered NotifySettlement request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifySettlementRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  NotifySettlementRequest                                                 Request,
                                                  ForwardingDecision<NotifySettlementRequest, NotifySettlementResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifySettlementRequestReceivedDelegate?    OnNotifySettlementRequestReceived;
        public event OnNotifySettlementRequestFilterDelegate?      OnNotifySettlementRequestFilter;
        public event OnNotifySettlementRequestFilteredDelegate?    OnNotifySettlementRequestFiltered;
        public event OnNotifySettlementRequestSentDelegate?        OnNotifySettlementRequestSent;

        public event OnNotifySettlementResponseReceivedDelegate?   OnNotifySettlementResponseReceived;
        public event OnNotifySettlementResponseSentDelegate?       OnNotifySettlementResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_NotifySettlement(OCPP_JSONRequestMessage  JSONRequestMessage,
                                     IWebSocketConnection     Connection,
                                     CancellationToken        CancellationToken   = default)

        {

            if (!NotifySettlementRequest.TryParse(JSONRequestMessage.Payload,
                                                  JSONRequestMessage.RequestId,
                                                  JSONRequestMessage.DestinationId,
                                                  JSONRequestMessage.NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  parentNetworkingNode.OCPP.CustomNotifySettlementRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<NotifySettlementRequest, NotifySettlementResponse>? forwardingDecision = null;


            #region Send OnNotifySettlementRequestReceived event

            var receivedLogging = OnNotifySettlementRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnNotifySettlementRequestReceivedDelegate>().
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
                                nameof(OnNotifySettlementRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnNotifySettlementRequestFilter event

            var requestFilter = OnNotifySettlementRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnNotifySettlementRequestFilterDelegate>().
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
                              nameof(OnNotifySettlementRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<NotifySettlementRequest, NotifySettlementResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifySettlementResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<NotifySettlementRequest, NotifySettlementResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifySettlementResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnNotifySettlementRequestFiltered event

            var logger = OnNotifySettlementRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnNotifySettlementRequestFilteredDelegate>().
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
                              nameof(OnNotifySettlementRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnNotifySettlementRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifySettlementRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnNotifySettlementRequestSentDelegate>().
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
                                    nameof(OnNotifySettlementRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifySettlementRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
