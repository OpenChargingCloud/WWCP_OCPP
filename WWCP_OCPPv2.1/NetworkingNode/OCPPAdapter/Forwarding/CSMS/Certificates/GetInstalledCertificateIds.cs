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
    /// A GetInstalledCertificateIds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>>

        OnGetInstalledCertificateIdsRequestFilterDelegate(DateTime                            Timestamp,
                                                          IEventSender                        Sender,
                                                          IWebSocketConnection                Connection,
                                                          GetInstalledCertificateIdsRequest   Request,
                                                          CancellationToken                   CancellationToken);


    /// <summary>
    /// A filtered GetInstalledCertificateIds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            GetInstalledCertificateIdsRequest                                                           Request,
                                                            ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetInstalledCertificateIdsRequestFilterDelegate?      OnGetInstalledCertificateIdsRequest;

        public event OnGetInstalledCertificateIdsRequestFilteredDelegate?    OnGetInstalledCertificateIdsRequestLogging;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetInstalledCertificateIds(OCPP_JSONRequestMessage  JSONRequestMessage,
                                               IWebSocketConnection     Connection,
                                               CancellationToken        CancellationToken   = default)

        {

            if (!GetInstalledCertificateIdsRequest.TryParse(JSONRequestMessage.Payload,
                                                            JSONRequestMessage.RequestId,
                                                            JSONRequestMessage.DestinationId,
                                                            JSONRequestMessage.NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            JSONRequestMessage.RequestTimestamp,
                                                            JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                            JSONRequestMessage.EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>? forwardingDecision = null;

            #region Send OnGetInstalledCertificateIdsRequest event

            var requestFilter = OnGetInstalledCertificateIdsRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnGetInstalledCertificateIdsRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
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
                              nameof(OnGetInstalledCertificateIdsRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetInstalledCertificateIdsResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetInstalledCertificateIdsResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnGetInstalledCertificateIdsRequestLogging event

            var logger = OnGetInstalledCertificateIdsRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnGetInstalledCertificateIdsRequestFilteredDelegate>().
                                       Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
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
                              nameof(OnGetInstalledCertificateIdsRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
