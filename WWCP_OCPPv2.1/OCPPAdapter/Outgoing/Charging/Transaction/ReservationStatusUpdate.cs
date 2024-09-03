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
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a ReservationStatusUpdate request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnReservationStatusUpdateRequestSentDelegate(DateTime                         Timestamp,
                                                                      IEventSender                     Sender,
                                                                      IWebSocketConnection?            Connection,
                                                                      ReservationStatusUpdateRequest   Request,
                                                                      SentMessageResults               SentMessageResult,
                                                                      CancellationToken                CancellationToken);


    /// <summary>
    /// A ReservationStatusUpdate response.
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

        OnReservationStatusUpdateResponseSentDelegate(DateTime                          Timestamp,
                                                      IEventSender                      Sender,
                                                      IWebSocketConnection?             Connection,
                                                      ReservationStatusUpdateRequest    Request,
                                                      ReservationStatusUpdateResponse   Response,
                                                      TimeSpan                          Runtime,
                                                      SentMessageResults                SentMessageResult,
                                                      CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReservationStatusUpdate request error was sent.
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

        OnReservationStatusUpdateRequestErrorSentDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection?             Connection,
                                                          ReservationStatusUpdateRequest?   Request,
                                                          OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                          TimeSpan?                         Runtime,
                                                          SentMessageResults                SentMessageResult,
                                                          CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReservationStatusUpdate response error was sent.
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

        OnReservationStatusUpdateResponseErrorSentDelegate(DateTime                           Timestamp,
                                                           IEventSender                       Sender,
                                                           IWebSocketConnection?              Connection,
                                                           ReservationStatusUpdateRequest?    Request,
                                                           ReservationStatusUpdateResponse?   Response,
                                                           OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                           TimeSpan?                          Runtime,
                                                           SentMessageResults                 SentMessageResult,
                                                           CancellationToken                  CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send ReservationStatusUpdate request

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request was sent.
        /// </summary>
        public event OnReservationStatusUpdateRequestSentDelegate?  OnReservationStatusUpdateRequestSent;


        /// <summary>
        /// Send a ReservationStatusUpdate request.
        /// </summary>
        /// <param name="Request">A ReservationStatusUpdate request.</param>
        public async Task<ReservationStatusUpdateResponse>

            ReservationStatusUpdate(ReservationStatusUpdateRequest Request)

        {

            ReservationStatusUpdateResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = ReservationStatusUpdateResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnReservationStatusUpdateRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReservationStatusUpdateResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReservationStatusUpdateRequestError(
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

                    response ??= new ReservationStatusUpdateResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = ReservationStatusUpdateResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnReservationStatusUpdateResponseSent event

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate response was sent.
        /// </summary>
        public event OnReservationStatusUpdateResponseSentDelegate?  OnReservationStatusUpdateResponseSent;

        public Task SendOnReservationStatusUpdateResponseSent(DateTime                         Timestamp,
                                                              IEventSender                     Sender,
                                                              IWebSocketConnection?            Connection,
                                                              ReservationStatusUpdateRequest   Request,
                                                              ReservationStatusUpdateResponse  Response,
                                                              TimeSpan                         Runtime,
                                                              SentMessageResults               SentMessageResult,
                                                              CancellationToken                CancellationToken = default)

            => LogEvent(
                   OnReservationStatusUpdateResponseSent,
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

        #region Send OnReservationStatusUpdateRequestErrorSent event

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request error was sent.
        /// </summary>
        public event OnReservationStatusUpdateRequestErrorSentDelegate? OnReservationStatusUpdateRequestErrorSent;


        public Task SendOnReservationStatusUpdateRequestErrorSent(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  IWebSocketConnection?            Connection,
                                                                  ReservationStatusUpdateRequest?  Request,
                                                                  OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                                  TimeSpan                         Runtime,
                                                                  SentMessageResults               SentMessageResult,
                                                                  CancellationToken                CancellationToken = default)

            => LogEvent(
                   OnReservationStatusUpdateRequestErrorSent,
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

        #region Send OnReservationStatusUpdateResponseErrorSent event

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate response error was sent.
        /// </summary>
        public event OnReservationStatusUpdateResponseErrorSentDelegate? OnReservationStatusUpdateResponseErrorSent;


        public Task SendOnReservationStatusUpdateResponseErrorSent(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   IWebSocketConnection?             Connection,
                                                                   ReservationStatusUpdateRequest?   Request,
                                                                   ReservationStatusUpdateResponse?  Response,
                                                                   OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                                   TimeSpan                          Runtime,
                                                                   SentMessageResults                SentMessageResult,
                                                                   CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnReservationStatusUpdateResponseErrorSent,
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
