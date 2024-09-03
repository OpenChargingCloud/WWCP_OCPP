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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SecureDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>>

        OnSecureDataTransferRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  SecureDataTransferRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered SecureDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnSecureDataTransferRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    SecureDataTransferRequest                                                   Request,
                                                    RequestForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>   ForwardingDecision,
                                                    CancellationToken                                                           CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSecureDataTransferRequestReceivedDelegate?    OnSecureDataTransferRequestReceived;
        public event OnSecureDataTransferRequestFilterDelegate?      OnSecureDataTransferRequestFilter;
        public event OnSecureDataTransferRequestFilteredDelegate?    OnSecureDataTransferRequestFiltered;
        public event OnSecureDataTransferRequestSentDelegate?        OnSecureDataTransferRequestSent;

        public event OnSecureDataTransferResponseReceivedDelegate?   OnSecureDataTransferResponseReceived;
        public event OnSecureDataTransferResponseSentDelegate?       OnSecureDataTransferResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_SecureDataTransfer(OCPP_JSONRequestMessage    JSONRequestMessage,
                                       OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                       IWebSocketConnection       WebSocketConnection,
                                       CancellationToken          CancellationToken   = default)

        {

            #region Parse the SecureDataTransfer request

            if (!SecureDataTransferRequest.TryParse(BinaryRequestMessage.Payload,
                                                    BinaryRequestMessage.RequestId,
                                                    BinaryRequestMessage.Destination,
                                                    BinaryRequestMessage.NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    BinaryRequestMessage.RequestTimestamp,
                                                    BinaryRequestMessage.RequestTimeout - Timestamp.Now,
                                                    BinaryRequestMessage.EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomSecureDataTransferRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnSecureDataTransferRequestReceived event

            await LogEvent(
                      OnSecureDataTransferRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnSecureDataTransferRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnSecureDataTransferRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.BinaryRejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SecureDataTransferResponse(
                                       request,
                                       SecureDataTransferStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToBinary(
                                             parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewBinaryRequest = forwardingDecision.NewRequest.ToBinary(
                                                          parentNetworkingNode.OCPP.CustomSecureDataTransferRequestSerializer,
                                                          parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                      );

            #region Send OnSecureDataTransferRequestFiltered event

            await LogEvent(
                      OnSecureDataTransferRequestFiltered,
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


            #region Attach OnSecureDataTransferRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnSecureDataTransferRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnSecureDataTransferRequestSent,
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
