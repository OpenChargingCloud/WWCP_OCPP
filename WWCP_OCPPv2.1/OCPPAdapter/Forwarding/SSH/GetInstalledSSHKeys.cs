/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnGetInstalledSSHKeysRequestFilter(ed)Delegate

    /// <summary>
    /// A GetInstalledSSHKeys request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<GetInstalledSSHKeysRequest, GetInstalledSSHKeysResponse>>

        OnGetInstalledSSHKeysRequestFilterDelegate(DateTimeOffset             Timestamp,
                                                 IEventSender               Sender,
                                                 IMessageConnection         Connection,
                                                 GetInstalledSSHKeysRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered GetInstalledSSHKeys request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetInstalledSSHKeysRequestFilteredDelegate(DateTimeOffset                                                            Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IMessageConnection                                                        Connection,
                                                   GetInstalledSSHKeysRequest                                                  Request,
                                                   RequestForwardingDecision<GetInstalledSSHKeysRequest, GetInstalledSSHKeysResponse>   ForwardingDecision,
                                                   CancellationToken                                                         CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetInstalledSSHKeysRequestReceivedDelegate?    OnGetInstalledSSHKeysRequestReceived;
        public event OnGetInstalledSSHKeysRequestFilterDelegate?      OnGetInstalledSSHKeysRequestFilter;
        public event OnGetInstalledSSHKeysRequestFilteredDelegate?    OnGetInstalledSSHKeysRequestFiltered;
        public event OnGetInstalledSSHKeysRequestSentDelegate?        OnGetInstalledSSHKeysRequestSent;

        public event OnGetInstalledSSHKeysResponseReceivedDelegate?   OnGetInstalledSSHKeysResponseReceived;
        public event OnGetInstalledSSHKeysResponseSentDelegate?       OnGetInstalledSSHKeysResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_GetInstalledSSHKeys(OCPP_JSONRequestMessage    JSONRequestMessage,
                                      OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                      IMessageConnection         WebSocketConnection,
                                      CancellationToken          CancellationToken   = default)

        {

            #region Parse the GetInstalledSSHKeys request

            if (!GetInstalledSSHKeysRequest.TryParse(JSONRequestMessage.Payload,
                                                   JSONRequestMessage.RequestId,
                                                   JSONRequestMessage.Destination,
                                                   JSONRequestMessage.NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   JSONRequestMessage.RequestTimestamp,
                                                   JSONRequestMessage.RequestTimeout - Now,
                                                   JSONRequestMessage.EventTrackingId,
                                                   parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetInstalledSSHKeysRequestReceived event

            await LogEvent(
                      OnGetInstalledSSHKeysRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetInstalledSSHKeysRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetInstalledSSHKeysRequestFilter,
                                               filter => filter.Invoke(
                                                             Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request,
                                                             CancellationToken
                                                         )
                                           );

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = new RequestForwardingDecision<GetInstalledSSHKeysRequest, GetInstalledSSHKeysResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetInstalledSSHKeysResponse(
                                       request,
                                       GenericStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<GetInstalledSSHKeysRequest, GetInstalledSSHKeysResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
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
                                                        parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetInstalledSSHKeysRequestFiltered event

            await LogEvent(
                      OnGetInstalledSSHKeysRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          forwardingDecision,
                          CancellationToken
                      )
                  );

            #endregion


            #region Attach OnGetInstalledSSHKeysRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnGetInstalledSSHKeysRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetInstalledSSHKeysRequestSent,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Now,
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
