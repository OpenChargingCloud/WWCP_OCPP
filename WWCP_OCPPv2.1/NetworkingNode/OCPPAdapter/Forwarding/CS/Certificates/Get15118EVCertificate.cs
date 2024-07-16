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
    /// A Get15118EVCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>>

        OnGet15118EVCertificateRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     Get15118EVCertificateRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered Get15118EVCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGet15118EVCertificateRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       Get15118EVCertificateRequest                                                      Request,
                                                       ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGet15118EVCertificateRequestReceivedDelegate?    OnGet15118EVCertificateRequestReceived;
        public event OnGet15118EVCertificateRequestFilterDelegate?      OnGet15118EVCertificateRequestFilter;
        public event OnGet15118EVCertificateRequestFilteredDelegate?    OnGet15118EVCertificateRequestFiltered;
        public event OnGet15118EVCertificateRequestSentDelegate?        OnGet15118EVCertificateRequestSent;

        public event OnGet15118EVCertificateResponseReceivedDelegate?   OnGet15118EVCertificateResponseReceived;
        public event OnGet15118EVCertificateResponseSentDelegate?       OnGet15118EVCertificateResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_Get15118EVCertificate(OCPP_JSONRequestMessage  JSONRequestMessage,
                                          IWebSocketConnection     Connection,
                                          CancellationToken        CancellationToken   = default)

        {

            if (!Get15118EVCertificateRequest.TryParse(JSONRequestMessage.Payload,
                                                       JSONRequestMessage.RequestId,
                                                       JSONRequestMessage.DestinationId,
                                                       JSONRequestMessage.NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       parentNetworkingNode.OCPP.CustomGet15118EVCertificateRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>? forwardingDecision = null;


            #region Send OnGet15118EVCertificateRequestReceived event

            var receivedLogging = OnGet15118EVCertificateRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnGet15118EVCertificateRequestReceivedDelegate>().
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
                                nameof(OnGet15118EVCertificateRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnGet15118EVCertificateRequestFilter event

            var requestFilter = OnGet15118EVCertificateRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnGet15118EVCertificateRequestFilterDelegate>().
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
                              nameof(OnGet15118EVCertificateRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new Get15118EVCertificateResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGet15118EVCertificateResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnGet15118EVCertificateRequestFiltered event

            var logger = OnGet15118EVCertificateRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnGet15118EVCertificateRequestFilteredDelegate>().
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
                              nameof(OnGet15118EVCertificateRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnGet15118EVCertificateRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGet15118EVCertificateRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnGet15118EVCertificateRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnGet15118EVCertificateRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGet15118EVCertificateRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
