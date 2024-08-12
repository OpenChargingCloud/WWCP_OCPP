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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using Org.BouncyCastle.Asn1.Ocsp;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SetVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>>

        OnSetVariableMonitoringRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     SetVariableMonitoringRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered SetVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnSetVariableMonitoringRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       SetVariableMonitoringRequest                                                      Request,
                                                       ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>   ForwardingDecision,
                                                       CancellationToken                                                                 CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSetVariableMonitoringRequestReceivedDelegate?    OnSetVariableMonitoringRequestReceived;
        public event OnSetVariableMonitoringRequestFilterDelegate?      OnSetVariableMonitoringRequestFilter;
        public event OnSetVariableMonitoringRequestFilteredDelegate?    OnSetVariableMonitoringRequestFiltered;
        public event OnSetVariableMonitoringRequestSentDelegate?        OnSetVariableMonitoringRequestSent;

        public event OnSetVariableMonitoringResponseReceivedDelegate?   OnSetVariableMonitoringResponseReceived;
        public event OnSetVariableMonitoringResponseSentDelegate?       OnSetVariableMonitoringResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SetVariableMonitoring(OCPP_JSONRequestMessage  JSONRequestMessage,
                                          IWebSocketConnection     WebSocketConnection,
                                          CancellationToken        CancellationToken   = default)

        {

            #region Parse the Authorize request

            if (!SetVariableMonitoringRequest.TryParse(JSONRequestMessage.Payload,
                                                       JSONRequestMessage.RequestId,
                                                       JSONRequestMessage.DestinationId,
                                                       JSONRequestMessage.NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       JSONRequestMessage.RequestTimestamp,
                                                       JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                       JSONRequestMessage.EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomSetVariableMonitoringRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnSetVariableMonitoringRequestReceived event

            await LogEvent(
                      OnSetVariableMonitoringRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnSetVariableMonitoringRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnSetVariableMonitoringRequestFilter,
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
                forwardingDecision = new ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SetVariableMonitoringResponse(
                                       request,
                                       [],
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSetVariableMonitoringResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSetMonitoringResultSerializer,
                                             parentNetworkingNode.OCPP.CustomComponentSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSESerializer,
                                             parentNetworkingNode.OCPP.CustomVariableSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomSetVariableMonitoringRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSetMonitoringDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomPeriodicEventStreamParametersSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnSetVariableMonitoringRequestFiltered event

            await LogEvent(
                      OnSetVariableMonitoringRequestFiltered,
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


            #region Attach OnSetVariableMonitoringRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnSetVariableMonitoringRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnSetVariableMonitoringRequestSent,
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
