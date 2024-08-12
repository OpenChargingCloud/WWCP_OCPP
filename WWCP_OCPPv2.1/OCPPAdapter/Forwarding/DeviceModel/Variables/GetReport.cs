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
    /// A GetReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetReportRequest, GetReportResponse>>

        OnGetReportRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         GetReportRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetReportRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           GetReportRequest                                          Request,
                                           ForwardingDecision<GetReportRequest, GetReportResponse>   ForwardingDecision,
                                           CancellationToken                                         CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetReportRequestReceivedDelegate?    OnGetReportRequestReceived;
        public event OnGetReportRequestFilterDelegate?      OnGetReportRequestFilter;
        public event OnGetReportRequestFilteredDelegate?    OnGetReportRequestFiltered;
        public event OnGetReportRequestSentDelegate?        OnGetReportRequestSent;

        public event OnGetReportResponseReceivedDelegate?   OnGetReportResponseReceived;
        public event OnGetReportResponseSentDelegate?       OnGetReportResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetReport(OCPP_JSONRequestMessage  JSONRequestMessage,
                              IWebSocketConnection     WebSocketConnection,
                              CancellationToken        CancellationToken   = default)

        {

            #region Parse the GetReport request

            if (!GetReportRequest.TryParse(JSONRequestMessage.Payload,
                                           JSONRequestMessage.RequestId,
                                           JSONRequestMessage.DestinationId,
                                           JSONRequestMessage.NetworkPath,
                                           out var request,
                                           out var errorResponse,
                                           JSONRequestMessage.RequestTimestamp,
                                           JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                           JSONRequestMessage.EventTrackingId,
                                           parentNetworkingNode.OCPP.CustomGetReportRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetReportRequestReceived event

            await LogEvent(
                      OnGetReportRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetReportRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnGetReportRequestFilter,
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
                forwardingDecision = new ForwardingDecision<GetReportRequest, GetReportResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetReportResponse(
                                       request,
                                       GenericDeviceModelStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetReportRequest, GetReportResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetReportRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetReportRequestFiltered event

            await LogEvent(
                      OnGetReportRequestFiltered,
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


            #region Attach OnGetReportRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGetReportRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetReportRequestSent,
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
