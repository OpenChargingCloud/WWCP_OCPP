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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a NotifyPriorityCharging request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyPriorityChargingRequestSentDelegate(DateTime                        Timestamp,
                                                                     IEventSender                    Sender,
                                                                     IWebSocketConnection?           Connection,
                                                                     NotifyPriorityChargingRequest   Request,
                                                                     SentMessageResults              SentMessageResult,
                                                                     CancellationToken               CancellationToken);


    /// <summary>
    /// A NotifyPriorityCharging response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnNotifyPriorityChargingResponseSentDelegate(DateTime                         Timestamp,
                                                     IEventSender                     Sender,
                                                     IWebSocketConnection?            Connection,
                                                     NotifyPriorityChargingRequest    Request,
                                                     NotifyPriorityChargingResponse   Response,
                                                     TimeSpan                         Runtime,
                                                     SentMessageResults               SentMessageResult,
                                                     CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyPriorityCharging request error was sent.
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

        OnNotifyPriorityChargingRequestErrorSentDelegate(DateTime                         Timestamp,
                                                         IEventSender                     Sender,
                                                         IWebSocketConnection?            Connection,
                                                         NotifyPriorityChargingRequest?   Request,
                                                         OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                         TimeSpan?                        Runtime,
                                                         SentMessageResults               SentMessageResult,
                                                         CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyPriorityCharging response error was sent.
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

        OnNotifyPriorityChargingResponseErrorSentDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection?             Connection,
                                                          NotifyPriorityChargingRequest?    Request,
                                                          NotifyPriorityChargingResponse?   Response,
                                                          OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                          TimeSpan?                         Runtime,
                                                          SentMessageResults                SentMessageResult,
                                                          CancellationToken                 CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NotifyPriorityCharging request

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request was sent.
        /// </summary>
        public event OnNotifyPriorityChargingRequestSentDelegate?  OnNotifyPriorityChargingRequestSent;


        /// <summary>
        /// Send a NotifyPriorityCharging request.
        /// </summary>
        /// <param name="Request">A NotifyPriorityCharging request.</param>
        public async Task<NotifyPriorityChargingResponse>

            NotifyPriorityCharging(NotifyPriorityChargingRequest Request)

        {

            NotifyPriorityChargingResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = NotifyPriorityChargingResponse.SignatureError(
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
                                                             false,
                                                             parentNetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnNotifyPriorityChargingRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sentMessageResult.Connection,
                                                             Request,
                                                             sentMessageResult.Result,
                                                             Request.CancellationToken
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyPriorityChargingResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyPriorityChargingRequestError(
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

                    response ??= new NotifyPriorityChargingResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = NotifyPriorityChargingResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnNotifyPriorityChargingResponseSent event

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging response was sent.
        /// </summary>
        public event OnNotifyPriorityChargingResponseSentDelegate?  OnNotifyPriorityChargingResponseSent;

        public Task SendOnNotifyPriorityChargingResponseSent(DateTime                        Timestamp,
                                                             IEventSender                    Sender,
                                                             IWebSocketConnection?           Connection,
                                                             NotifyPriorityChargingRequest   Request,
                                                             NotifyPriorityChargingResponse  Response,
                                                             TimeSpan                        Runtime,
                                                             SentMessageResults              SentMessageResult,
                                                             CancellationToken               CancellationToken = default)

            => LogEvent(
                   OnNotifyPriorityChargingResponseSent,
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

        #region Send OnNotifyPriorityChargingRequestErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request error was sent.
        /// </summary>
        public event OnNotifyPriorityChargingRequestErrorSentDelegate? OnNotifyPriorityChargingRequestErrorSent;


        public Task SendOnNotifyPriorityChargingRequestErrorSent(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 IWebSocketConnection?           Connection,
                                                                 NotifyPriorityChargingRequest?  Request,
                                                                 OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                                 TimeSpan                        Runtime,
                                                                 SentMessageResults              SentMessageResult,
                                                                 CancellationToken               CancellationToken = default)

            => LogEvent(
                   OnNotifyPriorityChargingRequestErrorSent,
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

        #region Send OnNotifyPriorityChargingResponseErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging response error was sent.
        /// </summary>
        public event OnNotifyPriorityChargingResponseErrorSentDelegate? OnNotifyPriorityChargingResponseErrorSent;


        public Task SendOnNotifyPriorityChargingResponseErrorSent(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  IWebSocketConnection?            Connection,
                                                                  NotifyPriorityChargingRequest?   Request,
                                                                  NotifyPriorityChargingResponse?  Response,
                                                                  OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
                                                                  TimeSpan                         Runtime,
                                                                  SentMessageResults               SentMessageResult,
                                                                  CancellationToken                CancellationToken = default)

            => LogEvent(
                   OnNotifyPriorityChargingResponseErrorSent,
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
