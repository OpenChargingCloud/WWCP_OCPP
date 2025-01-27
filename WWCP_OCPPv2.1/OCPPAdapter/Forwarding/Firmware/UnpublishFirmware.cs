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
    /// A filtered UnpublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>>

        OnUnpublishFirmwareRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 UnpublishFirmwareRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A UnpublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnUnpublishFirmwareRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   UnpublishFirmwareRequest                                                  Request,
                                                   RequestForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>   ForwardingDecision,
                                                   CancellationToken                                                         CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnUnpublishFirmwareRequestReceivedDelegate?    OnUnpublishFirmwareRequestReceived;
        public event OnUnpublishFirmwareRequestFilterDelegate?      OnUnpublishFirmwareRequestFilter;
        public event OnUnpublishFirmwareRequestFilteredDelegate?    OnUnpublishFirmwareRequestFiltered;
        public event OnUnpublishFirmwareRequestSentDelegate?        OnUnpublishFirmwareRequestSent;

        public event OnUnpublishFirmwareResponseReceivedDelegate?   OnUnpublishFirmwareResponseReceived;
        public event OnUnpublishFirmwareResponseSentDelegate?       OnUnpublishFirmwareResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_UnpublishFirmware(OCPP_JSONRequestMessage    JSONRequestMessage,
                                      OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                      IWebSocketConnection       WebSocketConnection,
                                      CancellationToken          CancellationToken   = default)

        {

            #region Parse the UnpublishFirmware request

            if (!UnpublishFirmwareRequest.TryParse(JSONRequestMessage.Payload,
                                                   JSONRequestMessage.RequestId,
                                                   JSONRequestMessage.Destination,
                                                   JSONRequestMessage.NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   JSONRequestMessage.RequestTimestamp,
                                                   JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                   JSONRequestMessage.EventTrackingId,
                                                   parentNetworkingNode.OCPP.CustomUnpublishFirmwareRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnUnpublishFirmwareRequestReceived event

            await LogEvent(
                      OnUnpublishFirmwareRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnUnpublishFirmwareRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnUnpublishFirmwareRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new UnpublishFirmwareResponse(
                                       request,
                                       UnpublishFirmwareStatus.Error,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnUnpublishFirmwareRequestFiltered event

            await LogEvent(
                      OnUnpublishFirmwareRequestFiltered,
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


            #region Attach OnUnpublishFirmwareRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnUnpublishFirmwareRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnUnpublishFirmwareRequestSent,
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
