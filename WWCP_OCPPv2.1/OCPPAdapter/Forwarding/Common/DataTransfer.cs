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
    /// A delegate called whenever a DataTransfer request should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<DataTransferRequest, DataTransferResponse>>

        OnDataTransferRequestFilterDelegate(DateTimeOffset         Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            DataTransferRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A delegate called whenever a DataTransfer request was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnDataTransferRequestFilteredDelegate(DateTimeOffset                                                  Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              DataTransferRequest                                             Request,
                                              RequestForwardingDecision<DataTransferRequest, DataTransferResponse>   ForwardingDecision,
                                              CancellationToken                                               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnDataTransferRequestReceivedDelegate?    OnDataTransferRequestReceived;
        public event OnDataTransferRequestFilterDelegate?      OnDataTransferRequestFilter;
        public event OnDataTransferRequestFilteredDelegate?    OnDataTransferRequestFiltered;
        public event OnDataTransferRequestSentDelegate?        OnDataTransferRequestSent;

        public event OnDataTransferResponseReceivedDelegate?   OnDataTransferResponseReceived;
        public event OnDataTransferResponseSentDelegate?       OnDataTransferResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_DataTransfer(OCPP_JSONRequestMessage    JSONRequestMessage,
                                 OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                 IWebSocketConnection       WebSocketConnection,
                                 CancellationToken          CancellationToken   = default)

        {

            #region Parse the DataTransfer request

            if (!DataTransferRequest.TryParse(JSONRequestMessage.Payload,
                                              JSONRequestMessage.RequestId,
                                              JSONRequestMessage.Destination,
                                              JSONRequestMessage.NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              JSONRequestMessage.RequestTimestamp,
                                              JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                              JSONRequestMessage.EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomDataTransferRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnDataTransferRequestReceived event

            await LogEvent(
                      OnDataTransferRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnDataTransferRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnDataTransferRequestFilter,
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
                forwardingDecision = RequestForwardingDecision<DataTransferRequest, DataTransferResponse>.FORWARD(request);

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new DataTransferResponse(
                                       request,
                                       DataTransferStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = RequestForwardingDecision<DataTransferRequest, DataTransferResponse>.REJECT(
                                         request,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
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
                                                        null,//parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnDataTransferRequestFiltered event

            await LogEvent(
                      OnDataTransferRequestFiltered,
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


            #region Attach OnDataTransferRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnDataTransferRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnDataTransferRequestSent,
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
