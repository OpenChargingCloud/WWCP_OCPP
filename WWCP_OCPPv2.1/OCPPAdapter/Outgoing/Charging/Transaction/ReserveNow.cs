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
    /// A delegate called whenever a ReserveNow request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnReserveNowRequestSentDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         IWebSocketConnection?   Connection,
                                                         ReserveNowRequest       Request,
                                                         SentMessageResults      SentMessageResult,
                                                         CancellationToken       CancellationToken);


    /// <summary>
    /// A ReserveNow response.
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

        OnReserveNowResponseSentDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection?  Connection,
                                         ReserveNowRequest      Request,
                                         ReserveNowResponse     Response,
                                         TimeSpan               Runtime,
                                         SentMessageResults     SentMessageResult,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReserveNow request error was sent.
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

        OnReserveNowRequestErrorSentDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             IWebSocketConnection?          Connection,
                                             ReserveNowRequest?             Request,
                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                             TimeSpan?                      Runtime,
                                             SentMessageResults             SentMessageResult,
                                             CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReserveNow response error was sent.
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

        OnReserveNowResponseErrorSentDelegate(DateTime                        Timestamp,
                                              IEventSender                    Sender,
                                              IWebSocketConnection?           Connection,
                                              ReserveNowRequest?              Request,
                                              ReserveNowResponse?             Response,
                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                              TimeSpan?                       Runtime,
                                              SentMessageResults              SentMessageResult,
                                              CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send ReserveNow request

        /// <summary>
        /// An event fired whenever a ReserveNow request was sent.
        /// </summary>
        public event OnReserveNowRequestSentDelegate?  OnReserveNowRequestSent;


        /// <summary>
        /// Send a ReserveNow request.
        /// </summary>
        /// <param name="Request">A ReserveNow request.</param>
        public async Task<ReserveNowResponse>

            ReserveNow(ReserveNowRequest Request)

        {

            ReserveNowResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = ReserveNowResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnReserveNowRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReserveNowResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReserveNowRequestError(
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

                    response ??= new ReserveNowResponse(
                                     Request,
                                     ReservationStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = ReserveNowResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnReserveNowResponseSent event

        /// <summary>
        /// An event sent whenever a ReserveNow response was sent.
        /// </summary>
        public event OnReserveNowResponseSentDelegate?  OnReserveNowResponseSent;

        public Task SendOnReserveNowResponseSent(DateTime              Timestamp,
                                                 IEventSender          Sender,
                                                 IWebSocketConnection? Connection,
                                                 ReserveNowRequest     Request,
                                                 ReserveNowResponse    Response,
                                                 TimeSpan              Runtime,
                                                 SentMessageResults    SentMessageResult,
                                                 CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnReserveNowResponseSent,
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

        #region Send OnReserveNowRequestErrorSent event

        /// <summary>
        /// An event sent whenever a ReserveNow request error was sent.
        /// </summary>
        public event OnReserveNowRequestErrorSentDelegate? OnReserveNowRequestErrorSent;


        public Task SendOnReserveNowRequestErrorSent(DateTime                      Timestamp,
                                                     IEventSender                  Sender,
                                                     IWebSocketConnection?         Connection,
                                                     ReserveNowRequest?            Request,
                                                     OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                     TimeSpan                      Runtime,
                                                     SentMessageResults            SentMessageResult,
                                                     CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnReserveNowRequestErrorSent,
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

        #region Send OnReserveNowResponseErrorSent event

        /// <summary>
        /// An event sent whenever a ReserveNow response error was sent.
        /// </summary>
        public event OnReserveNowResponseErrorSentDelegate? OnReserveNowResponseErrorSent;


        public Task SendOnReserveNowResponseErrorSent(DateTime                       Timestamp,
                                                      IEventSender                   Sender,
                                                      IWebSocketConnection?          Connection,
                                                      ReserveNowRequest?             Request,
                                                      ReserveNowResponse?            Response,
                                                      OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                      TimeSpan                       Runtime,
                                                      SentMessageResults             SentMessageResult,
                                                      CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnReserveNowResponseErrorSent,
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
