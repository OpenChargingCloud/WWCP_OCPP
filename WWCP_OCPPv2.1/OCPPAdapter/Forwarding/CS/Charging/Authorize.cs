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
    /// A Authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<AuthorizeRequest, AuthorizeResponse>>

        OnAuthorizeRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         AuthorizeRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered Authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnAuthorizeRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           AuthorizeRequest                                          Request,
                                           ForwardingDecision<AuthorizeRequest, AuthorizeResponse>   ForwardingDecision,
                                           CancellationToken                                         CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnAuthorizeRequestReceivedDelegate?    OnAuthorizeRequestReceived;
        public event OnAuthorizeRequestFilterDelegate?      OnAuthorizeRequestFilter;
        public event OnAuthorizeRequestFilteredDelegate?    OnAuthorizeRequestFiltered;
        public event OnAuthorizeRequestSentDelegate?        OnAuthorizeRequestSent;

        public event OnAuthorizeResponseReceivedDelegate?   OnAuthorizeResponseReceived;
        public event OnAuthorizeResponseSentDelegate?       OnAuthorizeResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_Authorize(OCPP_JSONRequestMessage  JSONRequestMessage,
                              IWebSocketConnection     WebSocketConnection,
                              CancellationToken        CancellationToken   = default)

        {

            #region Parse Authorize request

            if (!AuthorizeRequest.TryParse(JSONRequestMessage.Payload,
                                           JSONRequestMessage.RequestId,
                                           JSONRequestMessage.DestinationId,
                                           JSONRequestMessage.NetworkPath,
                                           out var request,
                                           out var errorResponse,
                                           JSONRequestMessage.RequestTimestamp,
                                           JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                           JSONRequestMessage.EventTrackingId,
                                           parentNetworkingNode.OCPP.CustomAuthorizeRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnAuthorizeRequestReceived event

            await LogEvent(
                      OnAuthorizeRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Call OnAuthorizeRequest filter

            ForwardingDecision<AuthorizeRequest, AuthorizeResponse>? forwardingDecision = null;

            forwardingDecision = await CallFilter(
                                           OnAuthorizeRequestFilter,
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

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<AuthorizeRequest, AuthorizeResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var rejectResponse  = forwardingDecision?.RejectResponse ??
                                          new AuthorizeResponse(
                                              request,
                                              IdTokenInfo.Error(AuthorizationStatus.Filtered),
                                              Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                          );

                forwardingDecision  = new ForwardingDecision<AuthorizeRequest, AuthorizeResponse>(
                                          request,
                                          ForwardingResults.REJECT,
                                          rejectResponse,
                                          rejectResponse.ToJSON(
                                              parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                                              parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                              parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                              parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                              parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                              parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                                              parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                              parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                          ),
                                          forwardingDecision?.RejectMessage,
                                          forwardingDecision?.RejectDetails,
                                          forwardingDecision?.LogMessage
                                      );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                        parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnAuthorizeRequestFiltered event

            await LogEvent(
                      OnAuthorizeRequestFiltered,
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


            #region Attach OnAuthorizeRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnAuthorizeRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                   OnAuthorizeRequestSent,
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
