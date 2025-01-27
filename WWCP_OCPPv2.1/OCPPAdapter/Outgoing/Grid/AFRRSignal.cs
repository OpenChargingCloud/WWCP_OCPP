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
    /// A delegate called whenever an AFRRSignal request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAFRRSignalRequestSentDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         IWebSocketConnection?   Connection,
                                                         AFRRSignalRequest       Request,
                                                         SentMessageResults      SentMessageResult,
                                                         CancellationToken       CancellationToken);


    /// <summary>
    /// A AFRRSignal response.
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

        OnAFRRSignalResponseSentDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection?  Connection,
                                         AFRRSignalRequest      Request,
                                         AFRRSignalResponse     Response,
                                         TimeSpan               Runtime,
                                         SentMessageResults     SentMessageResult,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AFRRSignal request error was sent.
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

        OnAFRRSignalRequestErrorSentDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             IWebSocketConnection?          Connection,
                                             AFRRSignalRequest?             Request,
                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                             TimeSpan?                      Runtime,
                                             SentMessageResults             SentMessageResult,
                                             CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AFRRSignal response error was sent.
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

        OnAFRRSignalResponseErrorSentDelegate(DateTime                        Timestamp,
                                              IEventSender                    Sender,
                                              IWebSocketConnection?           Connection,
                                              AFRRSignalRequest?              Request,
                                              AFRRSignalResponse?             Response,
                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                              TimeSpan?                       Runtime,
                                              SentMessageResults              SentMessageResult,
                                              CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send AFRRSignal request

        /// <summary>
        /// An event fired whenever an AFRRSignal request was sent.
        /// </summary>
        public event OnAFRRSignalRequestSentDelegate?  OnAFRRSignalRequestSent;


        /// <summary>
        /// Send an AFRRSignal request.
        /// </summary>
        /// <param name="Request">A AFRRSignal request.</param>
        public async Task<AFRRSignalResponse>

            AFRRSignal(AFRRSignalRequest Request)

        {

            AFRRSignalResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = AFRRSignalResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnAFRRSignalRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_AFRRSignalResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_AFRRSignalRequestError(
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

                    response ??= new AFRRSignalResponse(
                                     Request,
                                     GenericStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = AFRRSignalResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnAFRRSignalResponseSent event

        /// <summary>
        /// An event sent whenever an AFRRSignal response was sent.
        /// </summary>
        public event OnAFRRSignalResponseSentDelegate?  OnAFRRSignalResponseSent;

        public Task SendOnAFRRSignalResponseSent(DateTime              Timestamp,
                                                 IEventSender          Sender,
                                                 IWebSocketConnection? Connection,
                                                 AFRRSignalRequest     Request,
                                                 AFRRSignalResponse    Response,
                                                 TimeSpan              Runtime,
                                                 SentMessageResults    SentMessageResult,
                                                 CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnAFRRSignalResponseSent,
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

        #region Send OnAFRRSignalRequestErrorSent event

        /// <summary>
        /// An event sent whenever an AFRRSignal request error was sent.
        /// </summary>
        public event OnAFRRSignalRequestErrorSentDelegate? OnAFRRSignalRequestErrorSent;


        public Task SendOnAFRRSignalRequestErrorSent(DateTime                      Timestamp,
                                                     IEventSender                  Sender,
                                                     IWebSocketConnection?         Connection,
                                                     AFRRSignalRequest?            Request,
                                                     OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                     TimeSpan                      Runtime,
                                                     SentMessageResults            SentMessageResult,
                                                     CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnAFRRSignalRequestErrorSent,
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

        #region Send OnAFRRSignalResponseErrorSent event

        /// <summary>
        /// An event sent whenever an AFRRSignal response error was sent.
        /// </summary>
        public event OnAFRRSignalResponseErrorSentDelegate? OnAFRRSignalResponseErrorSent;


        public Task SendOnAFRRSignalResponseErrorSent(DateTime                       Timestamp,
                                                      IEventSender                   Sender,
                                                      IWebSocketConnection?          Connection,
                                                      AFRRSignalRequest?             Request,
                                                      AFRRSignalResponse?            Response,
                                                      OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                      TimeSpan                       Runtime,
                                                      SentMessageResults             SentMessageResult,
                                                      CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnAFRRSignalResponseErrorSent,
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