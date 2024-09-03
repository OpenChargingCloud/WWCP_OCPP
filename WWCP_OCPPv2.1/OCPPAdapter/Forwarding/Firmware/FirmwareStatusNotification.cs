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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>>

        OnFirmwareStatusNotificationRequestFilterDelegate(DateTime                            Timestamp,
                                                          IEventSender                        Sender,
                                                          IWebSocketConnection                Connection,
                                                          FirmwareStatusNotificationRequest   Request,
                                                          CancellationToken                   CancellationToken);


    /// <summary>
    /// A filtered FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            FirmwareStatusNotificationRequest                                                           Request,
                                                            RequestForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>   ForwardingDecision,
                                                            CancellationToken                                                                           CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnFirmwareStatusNotificationRequestReceivedDelegate?    OnFirmwareStatusNotificationRequestReceived;
        public event OnFirmwareStatusNotificationRequestFilterDelegate?      OnFirmwareStatusNotificationRequestFilter;
        public event OnFirmwareStatusNotificationRequestFilteredDelegate?    OnFirmwareStatusNotificationRequestFiltered;
        public event OnFirmwareStatusNotificationRequestSentDelegate?        OnFirmwareStatusNotificationRequestSent;

        public event OnFirmwareStatusNotificationResponseReceivedDelegate?   OnFirmwareStatusNotificationResponseReceived;
        public event OnFirmwareStatusNotificationResponseSentDelegate?       OnFirmwareStatusNotificationResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_FirmwareStatusNotification(OCPP_JSONRequestMessage    JSONRequestMessage,
                                               OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                               IWebSocketConnection       WebSocketConnection,
                                               CancellationToken          CancellationToken   = default)

        {

            #region Parse the FirmwareStatusNotification request

            if (!FirmwareStatusNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                            JSONRequestMessage.RequestId,
                                                            JSONRequestMessage.Destination,
                                                            JSONRequestMessage.NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            JSONRequestMessage.RequestTimestamp,
                                                            JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                            JSONRequestMessage.EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnFirmwareStatusNotificationRequestReceived event

            await LogEvent(
                      OnFirmwareStatusNotificationRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnFirmwareStatusNotificationRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnFirmwareStatusNotificationRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new FirmwareStatusNotificationResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnFirmwareStatusNotificationRequestFiltered event

            await LogEvent(
                      OnFirmwareStatusNotificationRequestFiltered,
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


            #region Attach OnFirmwareStatusNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnFirmwareStatusNotificationRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnFirmwareStatusNotificationRequestSent,
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
