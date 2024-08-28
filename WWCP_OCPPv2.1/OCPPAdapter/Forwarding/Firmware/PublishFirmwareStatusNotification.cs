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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>>

        OnPublishFirmwareStatusNotificationRequestFilterDelegate(DateTime                                   Timestamp,
                                                                 IEventSender                               Sender,
                                                                 IWebSocketConnection                       Connection,
                                                                 PublishFirmwareStatusNotificationRequest   Request,
                                                                 CancellationToken                          CancellationToken);


    /// <summary>
    /// A filtered PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                                  Timestamp,
                                                                   IEventSender                                                                                              Sender,
                                                                   IWebSocketConnection                                                                                      Connection,
                                                                   PublishFirmwareStatusNotificationRequest                                                                  Request,
                                                                   RequestForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>   ForwardingDecision,
                                                                   CancellationToken                                                                                         CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnPublishFirmwareStatusNotificationRequestReceivedDelegate?    OnPublishFirmwareStatusNotificationRequestReceived;
        public event OnPublishFirmwareStatusNotificationRequestFilterDelegate?      OnPublishFirmwareStatusNotificationRequestFilter;
        public event OnPublishFirmwareStatusNotificationRequestFilteredDelegate?    OnPublishFirmwareStatusNotificationRequestFiltered;
        public event OnPublishFirmwareStatusNotificationRequestSentDelegate?        OnPublishFirmwareStatusNotificationRequestSent;

        public event OnPublishFirmwareStatusNotificationResponseReceivedDelegate?   OnPublishFirmwareStatusNotificationResponseReceived;
        public event OnPublishFirmwareStatusNotificationResponseSentDelegate?       OnPublishFirmwareStatusNotificationResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_PublishFirmwareStatusNotification(OCPP_JSONRequestMessage    JSONRequestMessage,
                                                      OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                      IWebSocketConnection       WebSocketConnection,
                                                      CancellationToken          CancellationToken   = default)

        {

            #region Parse the PublishFirmwareStatusNotification request

            if (!PublishFirmwareStatusNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                                   JSONRequestMessage.RequestId,
                                                                   JSONRequestMessage.Destination,
                                                                   JSONRequestMessage.NetworkPath,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   JSONRequestMessage.RequestTimestamp,
                                                                   JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                                   JSONRequestMessage.EventTrackingId,
                                                                   parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnPublishFirmwareStatusNotificationRequestReceived event

            await LogEvent(
                      OnPublishFirmwareStatusNotificationRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnPublishFirmwareStatusNotificationRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnPublishFirmwareStatusNotificationRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new PublishFirmwareStatusNotificationResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnPublishFirmwareStatusNotificationRequestFiltered event

            await LogEvent(
                      OnPublishFirmwareStatusNotificationRequestFiltered,
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


            #region Attach OnPublishFirmwareStatusNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnPublishFirmwareStatusNotificationRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnPublishFirmwareStatusNotificationRequestSent,
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
