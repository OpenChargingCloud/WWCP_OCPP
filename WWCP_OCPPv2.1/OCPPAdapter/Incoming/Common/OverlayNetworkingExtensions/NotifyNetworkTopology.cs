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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyMessageReceivedDelegate(DateTime                       Timestamp,
                                                                        IEventSender                   Sender,
                                                                        IWebSocketConnection           Connection,
                                                                        NotifyNetworkTopologyMessage   Request,
                                                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyResponseErrorReceivedDelegate(DateTime                         Timestamp,
                                                                              IEventSender                     Sender,
                                                                              IWebSocketConnection             Connection,
                                                                              NotifyNetworkTopologyMessage?    Request,
                                                                              OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
                                                                              TimeSpan?                        Runtime,
                                                                              CancellationToken                CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyNetworkTopology request

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyMessageReceivedDelegate?  OnNotifyNetworkTopologyMessageReceived;


        public async Task<OCPP_Response>

            Receive_NotifyNetworkTopology(DateTime              RequestTimestamp,
                                          IWebSocketConnection  WebSocketConnection,
                                          SourceRouting         Destination,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          JObject               JSONRequest,
                                          CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (NotifyNetworkTopologyMessage.TryParse(JSONRequest,
                                                          RequestId,
                                                          Destination,
                                                          NetworkPath,
                                                          out var message,
                                                          out var errorResponse,
                                                          RequestTimestamp,
                                                          EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageParser)) {

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifySendMessage(
                        message,
                        message.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageSerializer,
                            parentNetworkingNode.OCPP.CustomNetworkTopologyInformationSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        //response = NotifyNetworkTopologyResponse.SignatureError(
                        //               message,
                        //               errorResponse
                        //           );

                    }

                    #endregion

                    #region Send OnNotifyNetworkTopologyMessageReceived event

                    await LogEvent(
                              OnNotifyNetworkTopologyMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  message,
                                  CancellationToken
                              )
                          );

                    #endregion

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_NotifyNetworkTopology)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyNetworkTopology)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            ocppResponse ??= OCPP_Response.Empty();

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyNetworkTopology request error

        ///// <summary>
        ///// An event fired whenever a NotifyNetworkTopology request error was received.
        ///// </summary>
        //public event OnNotifyNetworkTopologyMessageErrorReceivedDelegate? NotifyNetworkTopologyMessageErrorReceived;


        //public async Task<NotifyNetworkTopologyResponse>

        //    Receive_NotifyNetworkTopologyMessageError(NotifyNetworkTopologyMessage  Request,
        //                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
        //                                              IWebSocketConnection          Connection,
        //                                              SourceRouting                 Destination,
        //                                              NetworkPath                   NetworkPath,
        //                                              EventTracking_Id              EventTrackingId,
        //                                              Request_Id                    RequestId,
        //                                              DateTime?                     ResponseTimestamp   = null,
        //                                              CancellationToken             CancellationToken   = default)
        //{

        //    //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
        //    //    response,
        //    //    response.ToJSON(
        //    //        parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
        //    //        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
        //    //        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
        //    //        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
        //    //        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
        //    //        parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
        //    //        parentNetworkingNode.OCPP.CustomSignatureSerializer,
        //    //        parentNetworkingNode.OCPP.CustomCustomDataSerializer
        //    //    ),
        //    //    out errorResponse
        //    //);

        //    #region Send NotifyNetworkTopologyMessageErrorReceived event

        //    await LogEvent(
        //              NotifyNetworkTopologyMessageErrorReceived,
        //              loggingDelegate => loggingDelegate.Invoke(
        //                  Timestamp.Now,
        //                  parentNetworkingNode,
        //                  Connection,
        //                  Request,
        //                  RequestErrorMessage,
        //                  RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
        //                  CancellationToken
        //              )
        //          );

        //    #endregion


        //    var response = NotifyNetworkTopologyResponse.RequestError(
        //                       Request,
        //                       RequestErrorMessage.EventTrackingId,
        //                       RequestErrorMessage.ErrorCode,
        //                       RequestErrorMessage.ErrorDescription,
        //                       RequestErrorMessage.ErrorDetails,
        //                       RequestErrorMessage.ResponseTimestamp,
        //                       RequestErrorMessage.Destination,
        //                       RequestErrorMessage.NetworkPath
        //                   );

        //    #region Send OnNotifyNetworkTopologyResponseReceived event

        //    await LogEvent(
        //              OnNotifyNetworkTopologyResponseReceived,
        //              loggingDelegate => loggingDelegate.Invoke(
        //                  Timestamp.Now,
        //                  parentNetworkingNode,
        //                  Connection,
        //                  Request,
        //                  response,
        //                  response.Runtime,
        //                  CancellationToken
        //              )
        //          );

        //    #endregion

        //    return response;

        //}

        #endregion


    }

}
