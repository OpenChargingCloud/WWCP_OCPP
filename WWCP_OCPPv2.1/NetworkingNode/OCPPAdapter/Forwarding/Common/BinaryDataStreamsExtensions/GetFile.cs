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
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetFileRequest, GetFileResponse>>

        OnGetFileRequestFilterDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       GetFileRequest         Request,
                                       CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetFileRequestFilteredDelegate(DateTime                                              Timestamp,
                                         IEventSender                                          Sender,
                                         IWebSocketConnection                                  Connection,
                                         GetFileRequest                                        Request,
                                         ForwardingDecision<GetFileRequest, GetFileResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetFileRequestReceivedDelegate?    OnGetFileRequestReceived;
        public event OnGetFileRequestFilterDelegate?      OnGetFileRequestFilter;
        public event OnGetFileRequestFilteredDelegate?    OnGetFileRequestFiltered;
        public event OnGetFileRequestSentDelegate?        OnGetFileRequestSent;

        public event OnGetFileResponseReceivedDelegate?   OnGetFileResponseReceived;
        public event OnGetFileResponseSentDelegate?       OnGetFileResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetFile(OCPP_JSONRequestMessage  JSONRequestMessage,
                            IWebSocketConnection     Connection,
                            CancellationToken        CancellationToken   = default)

        {

            if (!GetFileRequest.TryParse(JSONRequestMessage.Payload,
                                         JSONRequestMessage.RequestId,
                                         JSONRequestMessage.DestinationId,
                                         JSONRequestMessage.NetworkPath,
                                         out var request,
                                         out var errorResponse,
                                         parentNetworkingNode.OCPP.CustomGetFileRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<GetFileRequest, GetFileResponse>? forwardingDecision = null;


            #region Send OnGetFileRequestReceived event

            var receivedLogging = OnGetFileRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnGetFileRequestReceivedDelegate>().
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
                                nameof(OnGetFileRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnGetFileRequestFilter event

            var requestFilter = OnGetFileRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnGetFileRequestFilterDelegate>().
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
                              nameof(OnGetFileRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<GetFileRequest, GetFileResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetFileResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetFileRequest, GetFileResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToBinary(
                                             parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         )
                                     );

            }

            #endregion


            #region Send OnGetFileRequestFiltered event

            var logger = OnGetFileRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnGetFileRequestFilteredDelegate>().
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
                              nameof(OnGetFileRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnGetFileRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGetFileRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnGetFileRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnGetFileRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetFileRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
