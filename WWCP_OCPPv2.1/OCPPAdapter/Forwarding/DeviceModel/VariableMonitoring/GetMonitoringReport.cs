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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A GetMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>>

        OnGetMonitoringReportRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetMonitoringReportRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetMonitoringReportRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetMonitoringReportRequest                                                    Request,
                                                     ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>   ForwardingDecision,
                                                     CancellationToken                                                             CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetMonitoringReportRequestReceivedDelegate?    OnGetMonitoringReportRequestReceived;
        public event OnGetMonitoringReportRequestFilterDelegate?      OnGetMonitoringReportRequestFilter;
        public event OnGetMonitoringReportRequestFilteredDelegate?    OnGetMonitoringReportRequestFiltered;
        public event OnGetMonitoringReportRequestSentDelegate?        OnGetMonitoringReportRequestSent;

        public event OnGetMonitoringReportResponseReceivedDelegate?   OnGetMonitoringReportResponseReceived;
        public event OnGetMonitoringReportResponseSentDelegate?       OnGetMonitoringReportResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetMonitoringReport(OCPP_JSONRequestMessage  JSONRequestMessage,
                                        IWebSocketConnection     WebSocketConnection,
                                        CancellationToken        CancellationToken   = default)

        {

            #region Parse the GetMonitoringReport request

            if (!GetMonitoringReportRequest.TryParse(JSONRequestMessage.Payload,
                                                     JSONRequestMessage.RequestId,
                                                     JSONRequestMessage.DestinationId,
                                                     JSONRequestMessage.NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     JSONRequestMessage.RequestTimestamp,
                                                     JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                     JSONRequestMessage.EventTrackingId,
                                                     parentNetworkingNode.OCPP.CustomGetMonitoringReportRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetMonitoringReportRequestReceived event

            await LogEvent(
                      OnGetMonitoringReportRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetMonitoringReportRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetMonitoringReportRequestFilter,
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
                forwardingDecision = new ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetMonitoringReportResponse(
                                       request,
                                       GenericDeviceModelStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetMonitoringReportRequestFiltered event

            await LogEvent(
                      OnGetMonitoringReportRequestFiltered,
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


            #region Attach OnGetMonitoringReportRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGetMonitoringReportRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetMonitoringReportRequestSent,
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
