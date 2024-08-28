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
    /// A delegate called whenever a BinaryDataTransfer request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

        OnBinaryDataTransferRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  BinaryDataTransferRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A delegate called whenever a BinaryDataTransfer request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnBinaryDataTransferRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    BinaryDataTransferRequest                                                   Request,
                                                    RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>   ForwardingDecision,
                                                    CancellationToken                                                           CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnBinaryDataTransferRequestReceivedDelegate?    OnBinaryDataTransferRequestReceived;
        public event OnBinaryDataTransferRequestFilterDelegate?      OnBinaryDataTransferRequestFilter;
        public event OnBinaryDataTransferRequestFilteredDelegate?    OnBinaryDataTransferRequestFiltered;
        public event OnBinaryDataTransferRequestSentDelegate?        OnBinaryDataTransferRequestSent;

        public event OnBinaryDataTransferResponseReceivedDelegate?   OnBinaryDataTransferResponseReceived;
        public event OnBinaryDataTransferResponseSentDelegate?       OnBinaryDataTransferResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_BinaryDataTransfer(OCPP_JSONRequestMessage    JSONRequestMessage,
                                       OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                       IWebSocketConnection       WebSocketConnection,
                                       CancellationToken          CancellationToken   = default)

        {

            #region Parse the BinaryDataTransfer request

            if (!BinaryDataTransferRequest.TryParse(BinaryRequestMessage.Payload,
                                                    BinaryRequestMessage.RequestId,
                                                    BinaryRequestMessage.Destination,
                                                    BinaryRequestMessage.NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    BinaryRequestMessage.RequestTimestamp,
                                                    BinaryRequestMessage.RequestTimeout - Timestamp.Now,
                                                    BinaryRequestMessage.EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnBinaryDataTransferRequestReceived event

            await LogEvent(
                      OnBinaryDataTransferRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnBinaryDataTransferRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnBinaryDataTransferRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.BinaryRejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new BinaryDataTransferResponse(
                                       request,
                                       BinaryDataTransferStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToBinary(
                                             parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewBinaryRequest = forwardingDecision.NewRequest.ToBinary(
                                                          parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                                                          parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                      );

            #region Send OnBinaryDataTransferRequestFiltered event

            await LogEvent(
                      OnBinaryDataTransferRequestFiltered,
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


            #region Attach OnBinaryDataTransferRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnBinaryDataTransferRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnBinaryDataTransferRequestSent,
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
