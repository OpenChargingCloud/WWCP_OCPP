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
    /// A PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>>

        OnPublishFirmwareStatusNotificationRequestFilterDelegate(DateTime                                   Timestamp,
                                                                 IEventSender                               Sender,
                                                                 IWebSocketConnection                       Connection,
                                                                 PublishFirmwareStatusNotificationRequest   Request,
                                                                 CancellationToken                          CancellationToken);


    /// <summary>
    /// A filtered PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                                  Timestamp,
                                                                   IEventSender                                                                                              Sender,
                                                                   IWebSocketConnection                                                                                      Connection,
                                                                   PublishFirmwareStatusNotificationRequest                                                                  Request,
                                                                   ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnPublishFirmwareStatusNotificationRequestReceivedDelegate?    OnPublishFirmwareStatusNotificationRequestReceived;
        public event OnPublishFirmwareStatusNotificationRequestFilterDelegate?      OnPublishFirmwareStatusNotificationRequestFilter;
        public event OnPublishFirmwareStatusNotificationRequestFilteredDelegate?    OnPublishFirmwareStatusNotificationRequestFiltered;
        public event OnPublishFirmwareStatusNotificationRequestSentDelegate?        OnPublishFirmwareStatusNotificationRequestSent;

        public event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?   OnPublishFirmwareStatusNotificationResponseReceived;
        public event OnPublishFirmwareStatusNotificationResponseSentDelegate?       OnPublishFirmwareStatusNotificationResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_PublishFirmwareStatusNotification(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                      IWebSocketConnection     Connection,
                                                      CancellationToken        CancellationToken   = default)

        {

            if (!PublishFirmwareStatusNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                                   JSONRequestMessage.RequestId,
                                                                   JSONRequestMessage.DestinationId,
                                                                   JSONRequestMessage.NetworkPath,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   JSONRequestMessage.RequestTimestamp,
                                                                   JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                                   JSONRequestMessage.EventTrackingId,
                                                                   parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>? forwardingDecision = null;

            #region Send OnPublishFirmwareStatusNotificationRequestReceived event

            var receivedLogging = OnPublishFirmwareStatusNotificationRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnPublishFirmwareStatusNotificationRequestReceivedDelegate>().
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
                              nameof(OnPublishFirmwareStatusNotificationRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnPublishFirmwareStatusNotificationRequestFilter event

            var requestFilter = OnPublishFirmwareStatusNotificationRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnPublishFirmwareStatusNotificationRequestFilterDelegate>().
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
                              nameof(OnPublishFirmwareStatusNotificationRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new PublishFirmwareStatusNotificationResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnPublishFirmwareStatusNotificationRequestFiltered event

            var logger = OnPublishFirmwareStatusNotificationRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnPublishFirmwareStatusNotificationRequestFilteredDelegate>().
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
                              nameof(OnPublishFirmwareStatusNotificationRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnPublishFirmwareStatusNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnPublishFirmwareStatusNotificationRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnPublishFirmwareStatusNotificationRequestSentDelegate>().
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
                                      nameof(OnPublishFirmwareStatusNotificationRequestSent),
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
