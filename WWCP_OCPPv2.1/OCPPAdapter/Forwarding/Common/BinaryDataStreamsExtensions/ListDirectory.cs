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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>>

        OnListDirectoryRequestFilterDelegate(DateTime               Timestamp,
                                             IEventSender           Sender,
                                             IWebSocketConnection   Connection,
                                             ListDirectoryRequest   Request,
                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnListDirectoryRequestFilteredDelegate(DateTime                                                          Timestamp,
                                               IEventSender                                                      Sender,
                                               IWebSocketConnection                                              Connection,
                                               ListDirectoryRequest                                              Request,
                                               ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnListDirectoryRequestReceivedDelegate?    OnListDirectoryRequestReceived;
        public event OnListDirectoryRequestFilterDelegate?      OnListDirectoryRequestFilter;
        public event OnListDirectoryRequestFilteredDelegate?    OnListDirectoryRequestFiltered;
        public event OnListDirectoryRequestSentDelegate?        OnListDirectoryRequestSent;

        public event OnListDirectoryResponseReceivedDelegate?   OnListDirectoryResponseReceived;
        public event OnListDirectoryResponseSentDelegate?       OnListDirectoryResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_ListDirectory(OCPP_JSONRequestMessage  JSONRequestMessage,
                                  IWebSocketConnection     Connection,
                                  CancellationToken        CancellationToken   = default)

        {

            if (!ListDirectoryRequest.TryParse(JSONRequestMessage.Payload,
                                               JSONRequestMessage.RequestId,
                                               JSONRequestMessage.DestinationId,
                                               JSONRequestMessage.NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               JSONRequestMessage.RequestTimestamp,
                                               JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                               JSONRequestMessage.EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomListDirectoryRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>? forwardingDecision = null;

            #region Send OnListDirectoryRequestReceived event

            var receivedLogging = OnListDirectoryRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnListDirectoryRequestReceivedDelegate>().
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
                              nameof(OnListDirectoryRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnListDirectoryRequestFilter event

            var requestFilter = OnListDirectoryRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnListDirectoryRequestFilterDelegate>().
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
                              nameof(OnListDirectoryRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>.FORWARD(request);

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ListDirectoryResponse(
                                       request,
                                       request.DirectoryPath,
                                       ListDirectoryStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>.REJECT(
                                         request,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomListDirectoryRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnListDirectoryRequestFiltered event

            var logger = OnListDirectoryRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnListDirectoryRequestFilteredDelegate>().
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
                              nameof(OnListDirectoryRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnListDirectoryRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnListDirectoryRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnListDirectoryRequestSentDelegate>().
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
                                      nameof(OnListDirectoryRequestSent),
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
