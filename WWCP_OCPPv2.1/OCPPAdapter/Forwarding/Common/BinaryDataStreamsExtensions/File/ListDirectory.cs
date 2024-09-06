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

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>>

        OnListDirectoryRequestFilterDelegate(DateTime               Timestamp,
                                             IEventSender           Sender,
                                             IWebSocketConnection   Connection,
                                             ListDirectoryRequest   Request,
                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnListDirectoryRequestFilteredDelegate(DateTime                                                          Timestamp,
                                               IEventSender                                                      Sender,
                                               IWebSocketConnection                                              Connection,
                                               ListDirectoryRequest                                              Request,
                                               RequestForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>   ForwardingDecision,
                                               CancellationToken                                                 CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnListDirectoryRequestReceivedDelegate?    OnListDirectoryRequestReceived;
        public event OnListDirectoryRequestFilterDelegate?      OnListDirectoryRequestFilter;
        public event OnListDirectoryRequestFilteredDelegate?    OnListDirectoryRequestFiltered;
        public event OnListDirectoryRequestSentDelegate?        OnListDirectoryRequestSent;

        public event OnListDirectoryResponseReceivedDelegate?   OnListDirectoryResponseReceived;
        public event OnListDirectoryResponseSentDelegate?       OnListDirectoryResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_ListDirectory(OCPP_JSONRequestMessage    JSONRequestMessage,
                                  OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                  IWebSocketConnection       WebSocketConnection,
                                  CancellationToken          CancellationToken   = default)

        {

            #region Parse the ListDirectory request

            if (!ListDirectoryRequest.TryParse(JSONRequestMessage.Payload,
                                               JSONRequestMessage.RequestId,
                                               JSONRequestMessage.Destination,
                                               JSONRequestMessage.NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               JSONRequestMessage.RequestTimestamp,
                                               JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                               JSONRequestMessage.EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomListDirectoryRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnListDirectoryRequestReceived event

            await LogEvent(
                      OnListDirectoryRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnListDirectoryRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnListDirectoryRequestFilter,
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
                forwardingDecision = RequestForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>.FORWARD(request);

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ListDirectoryResponse(
                                       request,
                                       request.DirectoryPath,
                                       ListDirectoryStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = RequestForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>.REJECT(
                                         request,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomListDirectoryRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnListDirectoryRequestFiltered event

            await LogEvent(
                      OnListDirectoryRequestFiltered,
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


            #region Attach OnListDirectoryRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnListDirectoryRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnListDirectoryRequestSent,
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
