﻿/*
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
    /// A DeleteCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>>

        OnDeleteCertificateRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 DeleteCertificateRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered DeleteCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDeleteCertificateRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   DeleteCertificateRequest                                                  Request,
                                                   ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnDeleteCertificateRequestReceivedDelegate?    OnDeleteCertificateRequestReceived;
        public event OnDeleteCertificateRequestFilterDelegate?      OnDeleteCertificateRequestFilter;
        public event OnDeleteCertificateRequestFilteredDelegate?    OnDeleteCertificateRequestFiltered;
        public event OnDeleteCertificateRequestSentDelegate?        OnDeleteCertificateRequestSent;

        public event OnDeleteCertificateResponseReceivedDelegate?   OnDeleteCertificateResponseReceived;
        public event OnDeleteCertificateResponseSentDelegate?       OnDeleteCertificateResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_DeleteCertificate(OCPP_JSONRequestMessage  JSONRequestMessage,
                                      IWebSocketConnection     Connection,
                                      CancellationToken        CancellationToken   = default)

        {

            if (!DeleteCertificateRequest.TryParse(JSONRequestMessage.Payload,
                                                   JSONRequestMessage.RequestId,
                                                   JSONRequestMessage.DestinationId,
                                                   JSONRequestMessage.NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   JSONRequestMessage.RequestTimestamp,
                                                   JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                   JSONRequestMessage.EventTrackingId,
                                                   parentNetworkingNode.OCPP.CustomDeleteCertificateRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>? forwardingDecision = null;

            #region Send OnDeleteCertificateRequestReceived event

            var receivedLogging = OnDeleteCertificateRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnDeleteCertificateRequestReceivedDelegate>().
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
                              nameof(OnDeleteCertificateRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnDeleteCertificateRequestFilter event

            var requestFilter = OnDeleteCertificateRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnDeleteCertificateRequestFilterDelegate>().
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
                              nameof(OnDeleteCertificateRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new DeleteCertificateResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomDeleteCertificateResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomDeleteCertificateRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnDeleteCertificateRequestFiltered event

            var logger = OnDeleteCertificateRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnDeleteCertificateRequestFilteredDelegate>().
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
                              nameof(OnDeleteCertificateRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnDeleteCertificateRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnDeleteCertificateRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnDeleteCertificateRequestSentDelegate>().
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
                                      nameof(OnDeleteCertificateRequestSent),
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
