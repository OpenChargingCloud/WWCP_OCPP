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
    /// A delegate called whenever a GetTransactionStatus request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetTransactionStatusRequestSentDelegate(DateTime                      Timestamp,
                                                                   IEventSender                  Sender,
                                                                   IWebSocketConnection?         Connection,
                                                                   GetTransactionStatusRequest   Request,
                                                                   SentMessageResults            SentMessageResult,
                                                                   CancellationToken             CancellationToken);


    /// <summary>
    /// A GetTransactionStatus response.
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

        OnGetTransactionStatusResponseSentDelegate(DateTime                       Timestamp,
                                                   IEventSender                   Sender,
                                                   IWebSocketConnection?          Connection,
                                                   GetTransactionStatusRequest    Request,
                                                   GetTransactionStatusResponse   Response,
                                                   TimeSpan                       Runtime,
                                                   SentMessageResults             SentMessageResult,
                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetTransactionStatus request error was sent.
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

        OnGetTransactionStatusRequestErrorSentDelegate(DateTime                       Timestamp,
                                                       IEventSender                   Sender,
                                                       IWebSocketConnection?          Connection,
                                                       GetTransactionStatusRequest?   Request,
                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                       TimeSpan?                      Runtime,
                                                       SentMessageResults             SentMessageResult,
                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetTransactionStatus response error was sent.
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

        OnGetTransactionStatusResponseErrorSentDelegate(DateTime                        Timestamp,
                                                        IEventSender                    Sender,
                                                        IWebSocketConnection?           Connection,
                                                        GetTransactionStatusRequest?    Request,
                                                        GetTransactionStatusResponse?   Response,
                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                        TimeSpan?                       Runtime,
                                                        SentMessageResults              SentMessageResult,
                                                        CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send GetTransactionStatus request

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusRequestSentDelegate?  OnGetTransactionStatusRequestSent;


        /// <summary>
        /// Send a GetTransactionStatus request.
        /// </summary>
        /// <param name="Request">A GetTransactionStatus request.</param>
        public async Task<GetTransactionStatusResponse>

            GetTransactionStatus(GetTransactionStatusRequest Request)

        {

            GetTransactionStatusResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = GetTransactionStatusResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnGetTransactionStatusRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetTransactionStatusResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetTransactionStatusRequestError(
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

                    response ??= new GetTransactionStatusResponse(
                                     Request,
                                     false,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = GetTransactionStatusResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnGetTransactionStatusResponseSent event

        /// <summary>
        /// An event sent whenever a GetTransactionStatus response was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseSentDelegate?  OnGetTransactionStatusResponseSent;

        public Task SendOnGetTransactionStatusResponseSent(DateTime                      Timestamp,
                                                           IEventSender                  Sender,
                                                           IWebSocketConnection?         Connection,
                                                           GetTransactionStatusRequest   Request,
                                                           GetTransactionStatusResponse  Response,
                                                           TimeSpan                      Runtime,
                                                           SentMessageResults            SentMessageResult,
                                                           CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnGetTransactionStatusResponseSent,
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

        #region Send OnGetTransactionStatusRequestErrorSent event

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request error was sent.
        /// </summary>
        public event OnGetTransactionStatusRequestErrorSentDelegate? OnGetTransactionStatusRequestErrorSent;


        public Task SendOnGetTransactionStatusRequestErrorSent(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               IWebSocketConnection?         Connection,
                                                               GetTransactionStatusRequest?  Request,
                                                               OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                               TimeSpan                      Runtime,
                                                               SentMessageResults            SentMessageResult,
                                                               CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnGetTransactionStatusRequestErrorSent,
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

        #region Send OnGetTransactionStatusResponseErrorSent event

        /// <summary>
        /// An event sent whenever a GetTransactionStatus response error was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseErrorSentDelegate? OnGetTransactionStatusResponseErrorSent;


        public Task SendOnGetTransactionStatusResponseErrorSent(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection?          Connection,
                                                                GetTransactionStatusRequest?   Request,
                                                                GetTransactionStatusResponse?  Response,
                                                                OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                                TimeSpan                       Runtime,
                                                                SentMessageResults             SentMessageResult,
                                                                CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnGetTransactionStatusResponseErrorSent,
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
