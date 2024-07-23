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
    /// A LogStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>>

        OnLogStatusNotificationRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     LogStatusNotificationRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered LogStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnLogStatusNotificationRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       LogStatusNotificationRequest                                                      Request,
                                                       ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnLogStatusNotificationRequestReceivedDelegate?    OnLogStatusNotificationRequestReceived;
        public event OnLogStatusNotificationRequestFilterDelegate?      OnLogStatusNotificationRequestFilter;
        public event OnLogStatusNotificationRequestFilteredDelegate?    OnLogStatusNotificationRequestFiltered;
        public event OnLogStatusNotificationRequestSentDelegate?        OnLogStatusNotificationRequestSent;

        public event OnLogStatusNotificationResponseReceivedDelegate?   OnLogStatusNotificationResponseReceived;
        public event OnLogStatusNotificationResponseSentDelegate?       OnLogStatusNotificationResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_LogStatusNotification(OCPP_JSONRequestMessage  JSONRequestMessage,
                                          IWebSocketConnection     Connection,
                                          CancellationToken        CancellationToken   = default)

        {

            if (!LogStatusNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                       JSONRequestMessage.RequestId,
                                                       JSONRequestMessage.DestinationId,
                                                       JSONRequestMessage.NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       parentNetworkingNode.OCPP.CustomLogStatusNotificationRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>? forwardingDecision = null;


            #region Send OnLogStatusNotificationRequestReceived event

            var receivedLogging = OnLogStatusNotificationRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnLogStatusNotificationRequestReceivedDelegate>().
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
                                nameof(OnLogStatusNotificationRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnLogStatusNotificationRequestFilter event

            var requestFilter = OnLogStatusNotificationRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnLogStatusNotificationRequestFilterDelegate>().
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
                              nameof(OnLogStatusNotificationRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new LogStatusNotificationResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomLogStatusNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnLogStatusNotificationRequestFiltered event

            var logger = OnLogStatusNotificationRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnLogStatusNotificationRequestFilteredDelegate>().
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
                              nameof(OnLogStatusNotificationRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnLogStatusNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnLogStatusNotificationRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnLogStatusNotificationRequestSentDelegate>().
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
                                    nameof(OnLogStatusNotificationRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomLogStatusNotificationRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
