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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using Org.BouncyCastle.Asn1.Ocsp;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A PublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>>

        OnPublishFirmwareRequestFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               PublishFirmwareRequest   Request,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A filtered PublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPublishFirmwareRequestFilteredDelegate(DateTime                                                              Timestamp,
                                                 IEventSender                                                          Sender,
                                                 IWebSocketConnection                                                  Connection,
                                                 PublishFirmwareRequest                                                Request,
                                                 ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnPublishFirmwareRequestReceivedDelegate?    OnPublishFirmwareRequestReceived;
        public event OnPublishFirmwareRequestFilterDelegate?      OnPublishFirmwareRequestFilter;
        public event OnPublishFirmwareRequestFilteredDelegate?    OnPublishFirmwareRequestFiltered;
        public event OnPublishFirmwareRequestSentDelegate?        OnPublishFirmwareRequestSent;

        public event OnPublishFirmwareResponseReceivedDelegate?   OnPublishFirmwareResponseReceived;
        public event OnPublishFirmwareResponseSentDelegate?       OnPublishFirmwareResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_PublishFirmware(OCPP_JSONRequestMessage  JSONRequestMessage,
                                    IWebSocketConnection     Connection,
                                    CancellationToken        CancellationToken   = default)

        {

            if (!PublishFirmwareRequest.TryParse(JSONRequestMessage.Payload,
                                                 JSONRequestMessage.RequestId,
                                                 JSONRequestMessage.DestinationId,
                                                 JSONRequestMessage.NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 JSONRequestMessage.RequestTimestamp,
                                                 JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                 JSONRequestMessage.EventTrackingId,
                                                 parentNetworkingNode.OCPP.CustomPublishFirmwareRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>? forwardingDecision = null;

            #region Send OnPublishFirmwareRequestReceived event

            var receivedLogging = OnPublishFirmwareRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnPublishFirmwareRequestReceivedDelegate>().
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
                              nameof(OnPublishFirmwareRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnPublishFirmwareRequestFilter event

            var requestFilter = OnPublishFirmwareRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnPublishFirmwareRequestFilterDelegate>().
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
                              nameof(OnPublishFirmwareRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new PublishFirmwareResponse(
                                       request,
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomPublishFirmwareResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomPublishFirmwareRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnPublishFirmwareRequestFiltered event

            var logger = OnPublishFirmwareRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnPublishFirmwareRequestFilteredDelegate>().
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
                              nameof(OnPublishFirmwareRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnPublishFirmwareRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnPublishFirmwareRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnPublishFirmwareRequestSentDelegate>().
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
                                      nameof(OnPublishFirmwareRequestSent),
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
