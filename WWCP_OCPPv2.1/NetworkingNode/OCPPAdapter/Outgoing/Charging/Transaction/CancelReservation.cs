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
    /// A delegate called whenever a CancelReservation request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnCancelReservationRequestSentDelegate(DateTime                   Timestamp,
                                                                IEventSender               Sender,
                                                                IWebSocketConnection       Connection,
                                                                CancelReservationRequest   Request,
                                                                SentMessageResults         SendMessageResult,
                                                                CancellationToken          CancellationToken = default);


    /// <summary>
    /// A CancelReservation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnCancelReservationResponseSentDelegate(DateTime                    Timestamp,
                                                IEventSender                Sender,
                                                IWebSocketConnection        Connection,
                                                CancelReservationRequest    Request,
                                                CancelReservationResponse   Response,
                                                TimeSpan                    Runtime,
                                                SentMessageResults          SendMessageResult,
                                                CancellationToken           CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a CancelReservation request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnCancelReservationRequestErrorSentDelegate(DateTime                       Timestamp,
                                                    IEventSender                   Sender,
                                                    IWebSocketConnection           Connection,
                                                    CancelReservationRequest?      Request,
                                                    OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                    TimeSpan?                      Runtime,
                                                    SentMessageResults             SendMessageResult,
                                                    CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a CancelReservation response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnCancelReservationResponseErrorSentDelegate(DateTime                        Timestamp,
                                                     IEventSender                    Sender,
                                                     IWebSocketConnection            Connection,
                                                     CancelReservationRequest?       Request,
                                                     CancelReservationResponse?      Response,
                                                     OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                     TimeSpan?                       Runtime,
                                                     SentMessageResults              SendMessageResult,
                                                     CancellationToken               CancellationToken = default);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send CancelReservation request

        /// <summary>
        /// An event fired whenever a CancelReservation request was sent.
        /// </summary>
        public event OnCancelReservationRequestSentDelegate?  OnCancelReservationRequestSent;


        /// <summary>
        /// Send a CancelReservation request.
        /// </summary>
        /// <param name="Request">A CancelReservation request.</param>
        public async Task<CancelReservationResponse>

            CancelReservation(CancelReservationRequest Request)

        {

            CancelReservationResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = CancelReservationResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnCancelReservationRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sendMessageResult.Connection,
                                                             Request,
                                                             sendMessageResult.Result
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_CancelReservationResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_CancelReservationRequestError(
                                             Request,
                                             jsonRequestError,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    response ??= new CancelReservationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = CancelReservationResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnCancelReservationResponseSent event

        /// <summary>
        /// An event sent whenever a CancelReservation response was sent.
        /// </summary>
        public event OnCancelReservationResponseSentDelegate?  OnCancelReservationResponseSent;

        public Task SendOnCancelReservationResponseSent(DateTime                   Timestamp,
                                                        IEventSender               Sender,
                                                        IWebSocketConnection       Connection,
                                                        CancelReservationRequest   Request,
                                                        CancelReservationResponse  Response,
                                                        TimeSpan                   Runtime,
                                                        SentMessageResults         SendMessageResult,
                                                        CancellationToken          CancellationToken = default)

            => LogEvent(
                   OnCancelReservationResponseSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnCancelReservationRequestErrorSent event

        /// <summary>
        /// An event sent whenever a CancelReservation request error was sent.
        /// </summary>
        public event OnCancelReservationRequestErrorSentDelegate? OnCancelReservationRequestErrorSent;


        public Task SendOnCancelReservationRequestErrorSent(DateTime                      Timestamp,
                                                            IEventSender                  Sender,
                                                            IWebSocketConnection          Connection,
                                                            CancelReservationRequest?     Request,
                                                            OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                            TimeSpan                      Runtime,
                                                            SentMessageResults            SendMessageResult,
                                                            CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnCancelReservationRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnCancelReservationResponseErrorSent event

        /// <summary>
        /// An event sent whenever a CancelReservation response error was sent.
        /// </summary>
        public event OnCancelReservationResponseErrorSentDelegate? OnCancelReservationResponseErrorSent;


        public Task SendOnCancelReservationResponseErrorSent(DateTime                       Timestamp,
                                                             IEventSender                   Sender,
                                                             IWebSocketConnection           Connection,
                                                             CancelReservationRequest?      Request,
                                                             CancelReservationResponse?     Response,
                                                             OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                             TimeSpan                       Runtime,
                                                             SentMessageResults             SendMessageResult,
                                                             CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnCancelReservationResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

    }

}
