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
    /// A FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>>

        OnFirmwareStatusNotificationRequestFilterDelegate(DateTime                            Timestamp,
                                                          IEventSender                        Sender,
                                                          IWebSocketConnection                Connection,
                                                          FirmwareStatusNotificationRequest   Request,
                                                          CancellationToken                   CancellationToken);


    /// <summary>
    /// A filtered FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            FirmwareStatusNotificationRequest                                                           Request,
                                                            ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnFirmwareStatusNotificationRequestReceivedDelegate?    OnFirmwareStatusNotificationRequestReceived;
        public event OnFirmwareStatusNotificationRequestFilterDelegate?      OnFirmwareStatusNotificationRequestFilter;
        public event OnFirmwareStatusNotificationRequestFilteredDelegate?    OnFirmwareStatusNotificationRequestFiltered;
        public event OnFirmwareStatusNotificationRequestSentDelegate?        OnFirmwareStatusNotificationRequestSent;

        public event OnFirmwareStatusNotificationResponseReceivedDelegate?   OnFirmwareStatusNotificationResponseReceived;
        public event OnFirmwareStatusNotificationResponseSentDelegate?       OnFirmwareStatusNotificationResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_FirmwareStatusNotification(OCPP_JSONRequestMessage  JSONRequestMessage,
                                               IWebSocketConnection     Connection,
                                               CancellationToken        CancellationToken   = default)

        {

            if (!FirmwareStatusNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                            JSONRequestMessage.RequestId,
                                                            JSONRequestMessage.DestinationId,
                                                            JSONRequestMessage.NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>? forwardingDecision = null;


            #region Send OnFirmwareStatusNotificationRequestReceived event

            var receivedLogging = OnFirmwareStatusNotificationRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnFirmwareStatusNotificationRequestReceivedDelegate>().
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
                                nameof(OnFirmwareStatusNotificationRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            var requestFilter = OnFirmwareStatusNotificationRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnFirmwareStatusNotificationRequestFilterDelegate>().
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
                              nameof(OnFirmwareStatusNotificationRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new FirmwareStatusNotificationResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnFirmwareStatusNotificationRequestFiltered event

            var logger = OnFirmwareStatusNotificationRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnFirmwareStatusNotificationRequestFilteredDelegate>().
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
                              nameof(OnFirmwareStatusNotificationRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnFirmwareStatusNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnFirmwareStatusNotificationRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnFirmwareStatusNotificationRequestSentDelegate>().
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
                                    nameof(OnFirmwareStatusNotificationRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;


        }

    }

}
