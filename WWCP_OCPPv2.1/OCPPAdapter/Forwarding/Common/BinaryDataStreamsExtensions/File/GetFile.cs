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
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetFileRequest, GetFileResponse>>

        OnGetFileRequestFilterDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       GetFileRequest         Request,
                                       CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnGetFileRequestFilteredDelegate(DateTime                                              Timestamp,
                                         IEventSender                                          Sender,
                                         IWebSocketConnection                                  Connection,
                                         GetFileRequest                                        Request,
                                         ForwardingDecision<GetFileRequest, GetFileResponse>   ForwardingDecision,
                                         CancellationToken                                     CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetFileRequestReceivedDelegate?    OnGetFileRequestReceived;
        public event OnGetFileRequestFilterDelegate?      OnGetFileRequestFilter;
        public event OnGetFileRequestFilteredDelegate?    OnGetFileRequestFiltered;
        public event OnGetFileRequestSentDelegate?        OnGetFileRequestSent;

        public event OnGetFileResponseReceivedDelegate?   OnGetFileResponseReceived;
        public event OnGetFileResponseSentDelegate?       OnGetFileResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetFile(OCPP_JSONRequestMessage  JSONRequestMessage,
                            IWebSocketConnection     WebSocketConnection,
                            CancellationToken        CancellationToken   = default)

        {

            #region Parse the GetFile request

            if (!GetFileRequest.TryParse(JSONRequestMessage.Payload,
                                         JSONRequestMessage.RequestId,
                                         JSONRequestMessage.DestinationId,
                                         JSONRequestMessage.NetworkPath,
                                         out var request,
                                         out var errorResponse,
                                         JSONRequestMessage.RequestTimestamp,
                                         JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                         JSONRequestMessage.EventTrackingId,
                                         parentNetworkingNode.OCPP.CustomGetFileRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnGetFileRequestReceived event

            await LogEvent(
                      OnGetFileRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnGetFileRequestFilter event

            ForwardingDecision<GetFileRequest, GetFileResponse>? forwardingDecision = null;

            forwardingDecision = await CallFilter(
                                           OnGetFileRequestFilter,
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
                forwardingDecision = new ForwardingDecision<GetFileRequest, GetFileResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetFileResponse(
                                       request,
                                       request.FileName,
                                       GetFileStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetFileRequest, GetFileResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToBinary(
                                             parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomGetFileRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnGetFileRequestFiltered event

            await LogEvent(
                      OnGetFileRequestFiltered,
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


            #region Attach OnGetFileRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnGetFileRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnGetFileRequestSent,
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
