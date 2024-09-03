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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A GetCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<GetCRLRequest, GetCRLResponse>>

        OnGetCRLRequestFilterDelegate(DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      GetCRLRequest          Request,
                                      CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetCRLRequestFilteredDelegate(DateTime                                            Timestamp,
                                        IEventSender                                        Sender,
                                        IWebSocketConnection                                Connection,
                                        GetCRLRequest                                       Request,
                                        RequestForwardingDecision<GetCRLRequest, GetCRLResponse>   ForwardingDecision,
                                        CancellationToken                                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetCRLRequestReceivedDelegate?    OnGetCRLRequestReceived;
        public event OnGetCRLRequestFilterDelegate?      OnGetCRLRequestFilter;
        public event OnGetCRLRequestFilteredDelegate?    OnGetCRLRequestFiltered;
        public event OnGetCRLRequestSentDelegate?        OnGetCRLRequestSent;

        public event OnGetCRLResponseReceivedDelegate?   OnGetCRLResponseReceived;
        public event OnGetCRLResponseSentDelegate?       OnGetCRLResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_GetCRL(OCPP_JSONRequestMessage    JSONRequestMessage,
                           OCPP_BinaryRequestMessage  BinaryRequestMessage,
                           IWebSocketConnection       WebSocketConnection,
                           CancellationToken          CancellationToken   = default)

        {

            #region Parse the GetCRL request

            if (!GetCRLRequest.TryParse(JSONRequestMessage.Payload,
                                        JSONRequestMessage.RequestId,
                                        JSONRequestMessage.Destination,
                                        JSONRequestMessage.NetworkPath,
                                        out var request,
                                        out var errorResponse,
                                        JSONRequestMessage.RequestTimestamp,
                                        JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                        JSONRequestMessage.EventTrackingId,
                                        parentNetworkingNode.OCPP.CustomGetCRLRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetCRLRequestReceived event

            await LogEvent(
                      OnGetCRLRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetCRLRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetCRLRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<GetCRLRequest, GetCRLResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetCRLResponse(
                                       request,
                                       request.GetCRLRequestId,
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<GetCRLRequest, GetCRLResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetCRLResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetCRLRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetCRLRequestFiltered event

            await LogEvent(
                      OnGetCRLRequestFiltered,
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


            #region Attach OnGetCRLRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetCRLRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetCRLRequestSent,
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
