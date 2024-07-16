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
    /// A NotifyEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEventRequest, NotifyEventResponse>>

        OnNotifyEventRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           NotifyEventRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered NotifyEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEventRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             NotifyEventRequest                                            Request,
                                             ForwardingDecision<NotifyEventRequest, NotifyEventResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyEventRequestReceivedDelegate?    OnNotifyEventRequestReceived;
        public event OnNotifyEventRequestFilterDelegate?      OnNotifyEventRequestFilter;
        public event OnNotifyEventRequestFilteredDelegate?    OnNotifyEventRequestFiltered;
        public event OnNotifyEventRequestSentDelegate?        OnNotifyEventRequestSent;

        public event OnNotifyEventResponseReceivedDelegate?   OnNotifyEventResponseReceived;
        public event OnNotifyEventResponseSentDelegate?       OnNotifyEventResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_NotifyEvent(OCPP_JSONRequestMessage  JSONRequestMessage,
                                IWebSocketConnection     Connection,
                                CancellationToken        CancellationToken   = default)

        {

            if (!NotifyEventRequest.TryParse(JSONRequestMessage.Payload,
                                             JSONRequestMessage.RequestId,
                                             JSONRequestMessage.DestinationId,
                                             JSONRequestMessage.NetworkPath,
                                             out var request,
                                             out var errorResponse,
                                             parentNetworkingNode.OCPP.CustomNotifyEventRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<NotifyEventRequest, NotifyEventResponse>? forwardingDecision = null;


            #region Send OnNotifyEventRequestReceived event

            var receivedLogging = OnNotifyEventRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnNotifyEventRequestReceivedDelegate>().
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
                                nameof(OnNotifyEventRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnNotifyEventRequestFilter event

            var requestFilter = OnNotifyEventRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnNotifyEventRequestFilterDelegate>().
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
                              nameof(OnNotifyEventRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<NotifyEventRequest, NotifyEventResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyEventResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<NotifyEventRequest, NotifyEventResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyEventResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnNotifyEventRequestFiltered event

            var logger = OnNotifyEventRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnNotifyEventRequestFilteredDelegate>().
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
                              nameof(OnNotifyEventRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnNotifyEventRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifyEventRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnNotifyEventRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnNotifyEventRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomEventDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
