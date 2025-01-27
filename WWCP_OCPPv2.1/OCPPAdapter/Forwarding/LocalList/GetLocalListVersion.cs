/*
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
    /// A GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>>

        OnGetLocalListVersionRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetLocalListVersionRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetLocalListVersionRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetLocalListVersionRequest                                                    Request,
                                                     RequestForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>   ForwardingDecision,
                                                     CancellationToken                                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetLocalListVersionRequestReceivedDelegate?    OnGetLocalListVersionRequestReceived;
        public event OnGetLocalListVersionRequestFilterDelegate?      OnGetLocalListVersionRequestFilter;
        public event OnGetLocalListVersionRequestFilteredDelegate?    OnGetLocalListVersionRequestFiltered;
        public event OnGetLocalListVersionRequestSentDelegate?        OnGetLocalListVersionRequestSent;

        public event OnGetLocalListVersionResponseReceivedDelegate?   OnGetLocalListVersionResponseReceived;
        public event OnGetLocalListVersionResponseSentDelegate?       OnGetLocalListVersionResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_GetLocalListVersion(OCPP_JSONRequestMessage    JSONRequestMessage,
                                        OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                        IWebSocketConnection       WebSocketConnection,
                                        CancellationToken          CancellationToken   = default)

        {

            #region Parse the GetLocalListVersion request

            if (!GetLocalListVersionRequest.TryParse(JSONRequestMessage.Payload,
                                                     JSONRequestMessage.RequestId,
                                                     JSONRequestMessage.Destination,
                                                     JSONRequestMessage.NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     JSONRequestMessage.RequestTimestamp,
                                                     JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     parentNetworkingNode.OCPP.CustomGetLocalListVersionRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetLocalListVersionRequestReceived event

            await LogEvent(
                      OnGetLocalListVersionRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetLocalListVersionRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetLocalListVersionRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetLocalListVersionResponse(
                                       request,
                                       VersionNumber:  0,
                                       Result:         Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomGetLocalListVersionResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomGetLocalListVersionRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetLocalListVersionRequestFiltered event

            await LogEvent(
                      OnGetLocalListVersionRequestFiltered,
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


            #region Attach OnGetLocalListVersionRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetLocalListVersionRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetLocalListVersionRequestSent,
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
