﻿/*
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

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a PublishFirmwareStatusNotification request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnPublishFirmwareStatusNotificationRequestSentDelegate(DateTime                                   Timestamp,
                                                                                IEventSender                               Sender,
                                                                                IWebSocketConnection?                      Connection,
                                                                                PublishFirmwareStatusNotificationRequest   Request,
                                                                                SentMessageResults                         SentMessageResult,
                                                                                CancellationToken                          CancellationToken);


    /// <summary>
    /// A PublishFirmwareStatusNotification response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationResponseSentDelegate(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                IWebSocketConnection?                       Connection,
                                                                PublishFirmwareStatusNotificationRequest    Request,
                                                                PublishFirmwareStatusNotificationResponse   Response,
                                                                TimeSpan                                    Runtime,
                                                                SentMessageResults                          SentMessageResult,
                                                                CancellationToken                           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a PublishFirmwareStatusNotification request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error message.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationRequestErrorSentDelegate(DateTime                                    Timestamp,
                                                                    IEventSender                                Sender,
                                                                    IWebSocketConnection?                       Connection,
                                                                    PublishFirmwareStatusNotificationRequest?   Request,
                                                                    OCPP_JSONRequestErrorMessage                RequestErrorMessage,
                                                                    TimeSpan?                                   Runtime,
                                                                    SentMessageResults                          SentMessageResult,
                                                                    CancellationToken                           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a PublishFirmwareStatusNotification response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationResponseErrorSentDelegate(DateTime                                     Timestamp,
                                                                     IEventSender                                 Sender,
                                                                     IWebSocketConnection?                        Connection,
                                                                     PublishFirmwareStatusNotificationRequest?    Request,
                                                                     PublishFirmwareStatusNotificationResponse?   Response,
                                                                     OCPP_JSONResponseErrorMessage                ResponseErrorMessage,
                                                                     TimeSpan?                                    Runtime,
                                                                     SentMessageResults                           SentMessageResult,
                                                                     CancellationToken                            CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send PublishFirmwareStatusNotification request

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestSentDelegate?  OnPublishFirmwareStatusNotificationRequestSent;


        /// <summary>
        /// Send a PublishFirmwareStatusNotification request.
        /// </summary>
        /// <param name="Request">A PublishFirmwareStatusNotification request.</param>
        public async Task<PublishFirmwareStatusNotificationResponse>

            PublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request)

        {

            PublishFirmwareStatusNotificationResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = PublishFirmwareStatusNotificationResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );

                }

                #endregion

                else
                {

                    #region Send request message

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnPublishFirmwareStatusNotificationRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sendMessageResult.Connection,
                                                             Request,
                                                             sendMessageResult.Result,
                                                             Request.CancellationToken
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_PublishFirmwareStatusNotificationResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_PublishFirmwareStatusNotificationRequestError(
                                             Request,
                                             jsonRequestError,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    response ??= new PublishFirmwareStatusNotificationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = PublishFirmwareStatusNotificationResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnPublishFirmwareStatusNotificationResponseSent event

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification response was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseSentDelegate?  OnPublishFirmwareStatusNotificationResponseSent;

        public Task SendOnPublishFirmwareStatusNotificationResponseSent(DateTime                                   Timestamp,
                                                                        IEventSender                               Sender,
                                                                        IWebSocketConnection?                      Connection,
                                                                        PublishFirmwareStatusNotificationRequest   Request,
                                                                        PublishFirmwareStatusNotificationResponse  Response,
                                                                        TimeSpan                                   Runtime,
                                                                        SentMessageResults                         SentMessageResult,
                                                                        CancellationToken                          CancellationToken = default)

            => LogEvent(
                   OnPublishFirmwareStatusNotificationResponseSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnPublishFirmwareStatusNotificationRequestErrorSent event

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request error was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestErrorSentDelegate? OnPublishFirmwareStatusNotificationRequestErrorSent;


        public Task SendOnPublishFirmwareStatusNotificationRequestErrorSent(DateTime                                   Timestamp,
                                                                            IEventSender                               Sender,
                                                                            IWebSocketConnection?                      Connection,
                                                                            PublishFirmwareStatusNotificationRequest?  Request,
                                                                            OCPP_JSONRequestErrorMessage               RequestErrorMessage,
                                                                            TimeSpan                                   Runtime,
                                                                            SentMessageResults                         SentMessageResult,
                                                                            CancellationToken                          CancellationToken = default)

            => LogEvent(
                   OnPublishFirmwareStatusNotificationRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnPublishFirmwareStatusNotificationResponseErrorSent event

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification response error was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseErrorSentDelegate? OnPublishFirmwareStatusNotificationResponseErrorSent;


        public Task SendOnPublishFirmwareStatusNotificationResponseErrorSent(DateTime                                    Timestamp,
                                                                             IEventSender                                Sender,
                                                                             IWebSocketConnection?                       Connection,
                                                                             PublishFirmwareStatusNotificationRequest?   Request,
                                                                             PublishFirmwareStatusNotificationResponse?  Response,
                                                                             OCPP_JSONResponseErrorMessage               ResponseErrorMessage,
                                                                             TimeSpan                                    Runtime,
                                                                             SentMessageResults                          SentMessageResult,
                                                                             CancellationToken                           CancellationToken = default)

            => LogEvent(
                   OnPublishFirmwareStatusNotificationResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

    }

}