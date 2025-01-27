﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A GetInstalledCertificateIds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>>

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
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            GetInstalledCertificateIdsRequest                                                           Request,
                                                            RequestForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>   ForwardingDecision,
                                                            CancellationToken                                                                           CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetInstalledCertificateIdsRequestReceivedDelegate?    OnGetInstalledCertificateIdsRequestReceived;
        public event OnGetInstalledCertificateIdsRequestFilterDelegate?      OnGetInstalledCertificateIdsRequestFilter;
        public event OnGetInstalledCertificateIdsRequestFilteredDelegate?    OnGetInstalledCertificateIdsRequestFiltered;
        public event OnGetInstalledCertificateIdsRequestSentDelegate?        OnGetInstalledCertificateIdsRequestSent;

        public event OnGetInstalledCertificateIdsResponseReceivedDelegate?   OnGetInstalledCertificateIdsResponseReceived;
        public event OnGetInstalledCertificateIdsResponseSentDelegate?       OnGetInstalledCertificateIdsResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_GetInstalledCertificateIds(OCPP_JSONRequestMessage    JSONRequestMessage,
                                               OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                               IWebSocketConnection       WebSocketConnection,
                                               CancellationToken          CancellationToken   = default)

        {

            #region Parse the GetInstalledCertificateIds request

            if (!GetInstalledCertificateIdsRequest.TryParse(JSONRequestMessage.Payload,
                                                            JSONRequestMessage.RequestId,
                                                            JSONRequestMessage.Destination,
                                                            JSONRequestMessage.NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            JSONRequestMessage.RequestTimestamp,
                                                            JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                            JSONRequestMessage.EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetInstalledCertificateIdsRequestReceived event

            await LogEvent(
                      OnGetInstalledCertificateIdsRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetInstalledCertificateIdsRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetInstalledCertificateIdsRequestFilter,
                                               filter => filter.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request,
                                                             CancellationToken
                                                         )
                                           );

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = new RequestForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetInstalledCertificateIdsResponse(
                                       request,
                                       GetInstalledCertificateStatus.NotFound,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomGetInstalledCertificateIdsResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomCertificateHashDataChainSerializer,
                                             parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetInstalledCertificateIdsRequestFiltered event

            await LogEvent(
                      OnGetInstalledCertificateIdsRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          forwardingDecision,
                          CancellationToken
                      )
                  );

            #endregion


            #region Attach OnGetInstalledCertificateIdsRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetInstalledCertificateIdsRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetInstalledCertificateIdsRequestSent,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Timestamp.Now,
                                      parentNetworkingNode,
                                      sentMessageResult.Connection,
                                      request,
                                      sentMessageResult.Result,
                                      CancellationToken
                                  )
                              );

            }

            #endregion

            return forwardingDecision;

        }

    }

}
