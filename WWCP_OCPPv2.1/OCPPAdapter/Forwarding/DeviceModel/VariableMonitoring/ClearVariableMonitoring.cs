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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A ClearVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>>

        OnClearVariableMonitoringRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       ClearVariableMonitoringRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered ClearVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnClearVariableMonitoringRequestFilteredDelegate(DateTime                                                                              Timestamp,
                                                         IEventSender                                                                          Sender,
                                                         IWebSocketConnection                                                                  Connection,
                                                         ClearVariableMonitoringRequest                                                        Request,
                                                         RequestForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>   ForwardingDecision,
                                                         CancellationToken                                                                     CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnClearVariableMonitoringRequestReceivedDelegate?    OnClearVariableMonitoringRequestReceived;
        public event OnClearVariableMonitoringRequestFilterDelegate?      OnClearVariableMonitoringRequestFilter;
        public event OnClearVariableMonitoringRequestFilteredDelegate?    OnClearVariableMonitoringRequestFiltered;
        public event OnClearVariableMonitoringRequestSentDelegate?        OnClearVariableMonitoringRequestSent;

        public event OnClearVariableMonitoringResponseReceivedDelegate?   OnClearVariableMonitoringResponseReceived;
        public event OnClearVariableMonitoringResponseSentDelegate?       OnClearVariableMonitoringResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_ClearVariableMonitoring(OCPP_JSONRequestMessage    JSONRequestMessage,
                                            OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                            IWebSocketConnection       WebSocketConnection,
                                            CancellationToken          CancellationToken   = default)

        {

            #region Parse the ClearVariableMonitoring request

            if (!ClearVariableMonitoringRequest.TryParse(JSONRequestMessage.Payload,
                                                         JSONRequestMessage.RequestId,
                                                         JSONRequestMessage.Destination,
                                                         JSONRequestMessage.NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         JSONRequestMessage.RequestTimestamp,
                                                         JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                         JSONRequestMessage.EventTrackingId,
                                                         parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnClearVariableMonitoringRequestReceived event

            await LogEvent(
                      OnClearVariableMonitoringRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnClearVariableMonitoringRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnClearVariableMonitoringRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ClearVariableMonitoringResponse(
                                       request,
                                       [],
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnClearVariableMonitoringRequestFiltered event

            await LogEvent(
                      OnClearVariableMonitoringRequestFiltered,
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


            #region Attach OnClearVariableMonitoringRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnClearVariableMonitoringRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnClearVariableMonitoringRequestSent,
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
