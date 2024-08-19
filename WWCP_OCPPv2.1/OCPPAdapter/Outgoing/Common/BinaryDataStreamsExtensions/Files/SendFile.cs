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

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a SendFile request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSendFileRequestSentDelegate(DateTime                Timestamp,
                                                       IEventSender            Sender,
                                                       IWebSocketConnection?   Connection,
                                                       SendFileRequest         Request,
                                                       SentMessageResults      SentMessageResult,
                                                       CancellationToken       CancellationToken);


    /// <summary>
    /// A SendFile response.
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

        OnSendFileResponseSentDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection?  Connection,
                                       SendFileRequest        Request,
                                       SendFileResponse       Response,
                                       TimeSpan               Runtime,
                                       SentMessageResults     SentMessageResult,
                                       CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SendFile request error was sent.
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

        OnSendFileRequestErrorSentDelegate(DateTime                       Timestamp,
                                           IEventSender                   Sender,
                                           IWebSocketConnection?          Connection,
                                           SendFileRequest?               Request,
                                           OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                           TimeSpan?                      Runtime,
                                           SentMessageResults             SentMessageResult,
                                           CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SendFile response error was sent.
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

        OnSendFileResponseErrorSentDelegate(DateTime                        Timestamp,
                                            IEventSender                    Sender,
                                            IWebSocketConnection?           Connection,
                                            SendFileRequest?                Request,
                                            SendFileResponse?               Response,
                                            OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                            TimeSpan?                       Runtime,
                                            SentMessageResults              SentMessageResult,
                                            CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SendFile request

        /// <summary>
        /// An event fired whenever a SendFile request was sent.
        /// </summary>
        public event OnSendFileRequestSentDelegate?  OnSendFileRequestSent;


        /// <summary>
        /// Send a SendFile request.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        public async Task<SendFileResponse>

            SendFile(SendFileRequest Request)

        {

            SendFileResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToBinary(
                            parentNetworkingNode.OCPP.CustomSendFileRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var signingErrors
                    ))
                {

                    response = SendFileResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );

                }

                #endregion

                else
                {

                    #region Send request message

                    var sendRequestState = await SendBinaryRequestAndWait(

                                                     OCPP_BinaryRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToBinary(
                                                             parentNetworkingNode.OCPP.CustomSendFileRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                                             IncludeSignatures: true
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnSendFileRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SendFileResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SendFileRequestError(
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

                    response ??= new SendFileResponse(
                                     Request,
                                     Request.FileName,
                                     SendFileStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SendFileResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSendFileResponseSent event

        /// <summary>
        /// An event sent whenever a SendFile response was sent.
        /// </summary>
        public event OnSendFileResponseSentDelegate?  OnSendFileResponseSent;

        public Task SendOnSendFileResponseSent(DateTime              Timestamp,
                                               IEventSender          Sender,
                                               IWebSocketConnection? Connection,
                                               SendFileRequest       Request,
                                               SendFileResponse      Response,
                                               TimeSpan              Runtime,
                                               SentMessageResults    SentMessageResult,
                                               CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnSendFileResponseSent,
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

        #region Send OnSendFileRequestErrorSent event

        /// <summary>
        /// An event sent whenever a SendFile request error was sent.
        /// </summary>
        public event OnSendFileRequestErrorSentDelegate? OnSendFileRequestErrorSent;


        public Task SendOnSendFileRequestErrorSent(DateTime                      Timestamp,
                                                   IEventSender                  Sender,
                                                   IWebSocketConnection?         Connection,
                                                   SendFileRequest?              Request,
                                                   OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                   TimeSpan                      Runtime,
                                                   SentMessageResults            SentMessageResult,
                                                   CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSendFileRequestErrorSent,
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

        #region Send OnSendFileResponseErrorSent event

        /// <summary>
        /// An event sent whenever a SendFile response error was sent.
        /// </summary>
        public event OnSendFileResponseErrorSentDelegate? OnSendFileResponseErrorSent;


        public Task SendOnSendFileResponseErrorSent(DateTime                       Timestamp,
                                                    IEventSender                   Sender,
                                                    IWebSocketConnection?          Connection,
                                                    SendFileRequest?               Request,
                                                    SendFileResponse?              Response,
                                                    OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                    TimeSpan                       Runtime,
                                                    SentMessageResults             SentMessageResult,
                                                    CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSendFileResponseErrorSent,
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
