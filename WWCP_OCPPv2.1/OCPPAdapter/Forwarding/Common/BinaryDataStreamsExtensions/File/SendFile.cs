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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SendFileRequest, SendFileResponse>>

        OnSendFileRequestFilterDelegate(DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        SendFileRequest        Request,
                                        CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnSendFileRequestFilteredDelegate(DateTime                                                Timestamp,
                                          IEventSender                                            Sender,
                                          IWebSocketConnection                                    Connection,
                                          SendFileRequest                                         Request,
                                          ForwardingDecision<SendFileRequest, SendFileResponse>   ForwardingDecision,
                                          CancellationToken                                       CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSendFileRequestReceivedDelegate?    OnSendFileRequestReceived;
        public event OnSendFileRequestFilterDelegate?      OnSendFileRequestFilter;
        public event OnSendFileRequestFilteredDelegate?    OnSendFileRequestFiltered;
        public event OnSendFileRequestSentDelegate?        OnSendFileRequestSent;

        public event OnSendFileResponseReceivedDelegate?   OnSendFileResponseReceived;
        public event OnSendFileResponseSentDelegate?       OnSendFileResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SendFile(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                             IWebSocketConnection       WebSocketConnection,
                             CancellationToken          CancellationToken   = default)

        {

            #region Parse the SendFile request

            if (!SendFileRequest.TryParse(BinaryRequestMessage.Payload,
                                          BinaryRequestMessage.RequestId,
                                          BinaryRequestMessage.DestinationId,
                                          BinaryRequestMessage.NetworkPath,
                                          out var request,
                                          out var errorResponse,
                                          BinaryRequestMessage.RequestTimestamp,
                                          BinaryRequestMessage.RequestTimeout - Timestamp.Now,
                                          BinaryRequestMessage.EventTrackingId,
                                          parentNetworkingNode.OCPP.CustomSendFileRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnSendFileRequestReceived event

            await LogEvent(
                      OnSendFileRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnSendFileRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnSendFileRequestFilter,
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
                forwardingDecision = new ForwardingDecision<SendFileRequest, SendFileResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SendFileResponse(
                                       request,
                                       request.FileName,
                                       SendFileStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SendFileRequest, SendFileResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSendFileResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewBinaryRequest = forwardingDecision.NewRequest.ToBinary(
                                                          parentNetworkingNode.OCPP.CustomSendFileRequestSerializer,
                                                          parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                      );

            #region Send OnSendFileRequestFiltered event

            await LogEvent(
                      OnSendFileRequestFiltered,
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


            #region Attach OnSendFileRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnSendFileRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnSendFileRequestSent,
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
