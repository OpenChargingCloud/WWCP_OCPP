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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SendLocalList request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SendLocalListRequest, SendLocalListResponse>>

        OnSendLocalListRequestFilterDelegate(DateTime               Timestamp,
                                             IEventSender           Sender,
                                             IWebSocketConnection   Connection,
                                             SendLocalListRequest   Request,
                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered SendLocalList request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSendLocalListRequestFilteredDelegate(DateTime                                                          Timestamp,
                                               IEventSender                                                      Sender,
                                               IWebSocketConnection                                              Connection,
                                               SendLocalListRequest                                              Request,
                                               ForwardingDecision<SendLocalListRequest, SendLocalListResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSendLocalListRequestReceivedDelegate?    OnSendLocalListRequestReceived;
        public event OnSendLocalListRequestFilterDelegate?      OnSendLocalListRequestFilter;
        public event OnSendLocalListRequestFilteredDelegate?    OnSendLocalListRequestFiltered;
        public event OnSendLocalListRequestSentDelegate?        OnSendLocalListRequestSent;

        public event OnSendLocalListResponseReceivedDelegate?   OnSendLocalListResponseReceived;
        public event OnSendLocalListResponseSentDelegate?       OnSendLocalListResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SendLocalList(OCPP_JSONRequestMessage  JSONRequestMessage,
                                  IWebSocketConnection     Connection,
                                  CancellationToken        CancellationToken   = default)

        {

            if (!SendLocalListRequest.TryParse(JSONRequestMessage.Payload,
                                               JSONRequestMessage.RequestId,
                                               JSONRequestMessage.DestinationId,
                                               JSONRequestMessage.NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               JSONRequestMessage.RequestTimestamp,
                                               JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                               JSONRequestMessage.EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomSendLocalListRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<SendLocalListRequest, SendLocalListResponse>? forwardingDecision = null;

            #region Send OnSendLocalListRequestReceived event

            var receivedLogging = OnSendLocalListRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnSendLocalListRequestReceivedDelegate>().
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
                              nameof(OnSendLocalListRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnSendLocalListRequestFilter event

            var requestFilter = OnSendLocalListRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnSendLocalListRequestFilterDelegate>().
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
                              nameof(OnSendLocalListRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<SendLocalListRequest, SendLocalListResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SendLocalListResponse(
                                       request,
                                       SendLocalListStatus.Failed,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SendLocalListRequest, SendLocalListResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSendLocalListResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                                        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnSendLocalListRequestFiltered event

            var logger = OnSendLocalListRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnSendLocalListRequestFilteredDelegate>().
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
                              nameof(OnSendLocalListRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnSendLocalListRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnSendLocalListRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnSendLocalListRequestSentDelegate>().
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
                                      nameof(OnSendLocalListRequestSent),
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
