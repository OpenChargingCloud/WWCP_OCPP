﻿/*
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
    /// A TransactionEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<TransactionEventRequest, TransactionEventResponse>>

        OnTransactionEventRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                TransactionEventRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered TransactionEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnTransactionEventRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  TransactionEventRequest                                                 Request,
                                                  RequestForwardingDecision<TransactionEventRequest, TransactionEventResponse>   ForwardingDecision,
                                                  CancellationToken                                                       CancellationToken);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnTransactionEventRequestReceivedDelegate?    OnTransactionEventRequestReceived;
        public event OnTransactionEventRequestFilterDelegate?      OnTransactionEventRequestFilter;
        public event OnTransactionEventRequestFilteredDelegate?    OnTransactionEventRequestFiltered;
        public event OnTransactionEventRequestSentDelegate?        OnTransactionEventRequestSent;

        public event OnTransactionEventResponseReceivedDelegate?   OnTransactionEventResponseReceived;
        public event OnTransactionEventResponseSentDelegate?       OnTransactionEventResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_TransactionEvent(OCPP_JSONRequestMessage    JSONRequestMessage,
                                     OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                     IWebSocketConnection       WebSocketConnection,
                                     CancellationToken          CancellationToken   = default)

        {

            #region Parse the TransactionEvent request

            if (!TransactionEventRequest.TryParse(JSONRequestMessage.Payload,
                                                  JSONRequestMessage.RequestId,
                                                  JSONRequestMessage.Destination,
                                                  JSONRequestMessage.NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  JSONRequestMessage.RequestTimestamp,
                                                  JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                  JSONRequestMessage.EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomTransactionEventRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnTransactionEventRequestReceived event

            await LogEvent(
                      OnTransactionEventRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnTransactionEventRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnTransactionEventRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<TransactionEventRequest, TransactionEventResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new TransactionEventResponse(
                                       request,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = RequestForwardingDecision<TransactionEventRequest, TransactionEventResponse>.REJECT(
                                         request,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomTransactionEventResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(

                                                        false,

                                                        parentNetworkingNode.OCPP.CustomTransactionEventRequestSerializer,

                                                        parentNetworkingNode.OCPP.CustomTransactionSerializer,
                                                        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomMeterValueSerializer,
                                                        parentNetworkingNode.OCPP.CustomSampledValueSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignedMeterValueSerializer,
                                                        parentNetworkingNode.OCPP.CustomUnitsOfMeasureSerializer,

                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer

                                                    );

            #region Send OnTransactionEventRequestFiltered event

            await LogEvent(
                      OnTransactionEventRequestFiltered,
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


            #region Attach OnTransactionEventRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnTransactionEventRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnTransactionEventRequestSent,
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
