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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a MessageTransfer message was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the message.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Message">The MessageTransfer message.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnMessageTransferMessageReceivedDelegate(DateTime                 Timestamp,
                                                 IEventSender             Sender,
                                                 IWebSocketConnection     Connection,
                                                 MessageTransferMessage   Message,
                                                 CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a MessageTransfer request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Message">The message, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnMessageTransferMessageErrorReceivedDelegate(DateTime                       Timestamp,
                                                                       IEventSender                   Sender,
                                                                       IWebSocketConnection           Connection,
                                                                       MessageTransferMessage?        Message,
                                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                       TimeSpan?                      Runtime,
                                                                       CancellationToken              CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive MessageTransfer request

        /// <summary>
        /// An event sent whenever a MessageTransfer request was received.
        /// </summary>
        public event OnMessageTransferMessageReceivedDelegate?  OnMessageTransferMessageReceived;


        public async Task<OCPP_Response>

            Receive_MessageTransfer(DateTime              RequestTimestamp,
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

                if (MessageTransferMessage.TryParse(JSONRequest,
                                                    RequestId,
                                                    Destination,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    RequestTimestamp,
                                                    EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomMessageTransferMessageParser,
                                                    parentNetworkingNode.OCPP.CustomSignatureParser,
                                                    parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifySendMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomMessageTransferMessageSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        //response = MessageTransferResponse.SignatureError(
                        //               request,
                        //               errorResponse
                        //           );

                    }

                    #endregion

                    #region Send OnMessageTransferMessageReceived event

                    await LogEvent(
                              OnMessageTransferMessageReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  request,
                                  CancellationToken
                              )
                          );

                    #endregion

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_MessageTransfer)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_MessageTransfer)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            ocppResponse ??= OCPP_Response.Empty();

            return ocppResponse;

        }

        #endregion

        #region Receive MessageTransfer RequestError

        ///// <summary>
        ///// An event fired whenever a MessageTransfer request error was received.
        ///// </summary>
        //public event OnMessageTransferMessageErrorReceivedDelegate? MessageTransferMessageErrorReceived;


        //public async Task<MessageTransferR>

        //    Receive_MessageTransferMessageError(MessageTransferMessage        Request,
        //                                        OCPP_JSONRequestErrorMessage  RequestErrorMessage,
        //                                        IWebSocketConnection          Connection,
        //                                        SourceRouting                 Destination,
        //                                        NetworkPath                   NetworkPath,
        //                                        EventTracking_Id              EventTrackingId,
        //                                        Request_Id                    RequestId,
        //                                        DateTime?                     ResponseTimestamp   = null,
        //                                        CancellationToken             CancellationToken   = default)
        //{

        //    //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
        //    //    response,
        //    //    response.ToJSON(
        //    //        parentNetworkingNode.OCPP.CustomMessageTransferResponseSerializer,
        //    //        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
        //    //        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
        //    //        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
        //    //        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
        //    //        parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
        //    //        parentNetworkingNode.OCPP.CustomSignatureSerializer,
        //    //        parentNetworkingNode.OCPP.CustomCustomMessageSerializer
        //    //    ),
        //    //    out errorResponse
        //    //);

        //    #region Send MessageTransferMessageErrorReceived event

        //    await LogEvent(
        //              MessageTransferMessageErrorReceived,
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


        //    var response = MessageTransferResponse.RequestError(
        //                       Request,
        //                       RequestErrorMessage.EventTrackingId,
        //                       RequestErrorMessage.ErrorCode,
        //                       RequestErrorMessage.ErrorDescription,
        //                       RequestErrorMessage.ErrorDetails,
        //                       RequestErrorMessage.ResponseTimestamp,
        //                       RequestErrorMessage.Destination,
        //                       RequestErrorMessage.NetworkPath
        //                   );

        //    #region Send OnMessageTransferResponseReceived event

        //    await LogEvent(
        //              OnMessageTransferResponseReceived,
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
