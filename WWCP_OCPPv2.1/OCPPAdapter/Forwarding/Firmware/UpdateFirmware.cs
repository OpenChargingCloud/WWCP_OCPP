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
    /// A filtered UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>>

        OnUpdateFirmwareRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              UpdateFirmwareRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnUpdateFirmwareRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                UpdateFirmwareRequest                                               Request,
                                                RequestForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>   ForwardingDecision,
                                                CancellationToken                                                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnUpdateFirmwareRequestReceivedDelegate?    OnUpdateFirmwareRequestReceived;
        public event OnUpdateFirmwareRequestFilterDelegate?      OnUpdateFirmwareRequestFilter;
        public event OnUpdateFirmwareRequestFilteredDelegate?    OnUpdateFirmwareRequestFiltered;
        public event OnUpdateFirmwareRequestSentDelegate?        OnUpdateFirmwareRequestSent;

        public event OnUpdateFirmwareResponseReceivedDelegate?   OnUpdateFirmwareResponseReceived;
        public event OnUpdateFirmwareResponseSentDelegate?       OnUpdateFirmwareResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_UpdateFirmware(OCPP_JSONRequestMessage    JSONRequestMessage,
                                   OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                   IWebSocketConnection       WebSocketConnection,
                                   CancellationToken          CancellationToken   = default)

        {

            #region Parse the UpdateFirmware request

            if (!UpdateFirmwareRequest.TryParse(JSONRequestMessage.Payload,
                                                JSONRequestMessage.RequestId,
                                                JSONRequestMessage.Destination,
                                                JSONRequestMessage.NetworkPath,
                                                out var request,
                                                out var errorResponse,
                                                JSONRequestMessage.RequestTimestamp,
                                                JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                JSONRequestMessage.EventTrackingId,
                                                parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnUpdateFirmwareRequestReceived event

            await LogEvent(
                      OnUpdateFirmwareRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnUpdateFirmwareRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnUpdateFirmwareRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new UpdateFirmwareResponse(
                                       request,
                                       UpdateFirmwareStatus.Rejected,
                                       Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomUpdateFirmwareResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomFirmwareSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnUpdateFirmwareRequestFiltered event

            await LogEvent(
                      OnUpdateFirmwareRequestFiltered,
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


            #region Attach OnUpdateFirmwareRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnUpdateFirmwareRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnUpdateFirmwareRequestSent,
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
