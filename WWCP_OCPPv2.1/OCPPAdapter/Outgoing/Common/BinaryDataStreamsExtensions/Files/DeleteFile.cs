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
    /// A delegate called whenever a DeleteFile request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDeleteFileRequestSentDelegate(DateTime               Timestamp,
                                                         IEventSender           Sender,
                                                         IWebSocketConnection   Connection,
                                                         DeleteFileRequest      Request,
                                                         SentMessageResults     SendMessageResult,
                                                         CancellationToken      CancellationToken = default);


    /// <summary>
    /// A DeleteFile response.
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

        OnDeleteFileResponseSentDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         DeleteFileRequest      Request,
                                         DeleteFileResponse     Response,
                                         TimeSpan               Runtime,
                                         SentMessageResults     SendMessageResult,
                                         CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a DeleteFile request error was sent.
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

        OnDeleteFileRequestErrorSentDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             IWebSocketConnection           Connection,
                                             DeleteFileRequest?             Request,
                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                             TimeSpan?                      Runtime,
                                             SentMessageResults             SendMessageResult,
                                             CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a DeleteFile response error was sent.
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

        OnDeleteFileResponseErrorSentDelegate(DateTime                        Timestamp,
                                              IEventSender                    Sender,
                                              IWebSocketConnection            Connection,
                                              DeleteFileRequest?              Request,
                                              DeleteFileResponse?             Response,
                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                              TimeSpan?                       Runtime,
                                              SentMessageResults              SendMessageResult,
                                              CancellationToken               CancellationToken = default);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send DeleteFile request

        /// <summary>
        /// An event fired whenever a DeleteFile request was sent.
        /// </summary>
        public event OnDeleteFileRequestSentDelegate?  OnDeleteFileRequestSent;


        /// <summary>
        /// Send a DeleteFile request.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        public async Task<DeleteFileResponse>

            DeleteFile(DeleteFileRequest Request)

        {

            DeleteFileResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDeleteFileRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = DeleteFileResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomDeleteFileRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnDeleteFileRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_DeleteFileResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_DeleteFileRequestError(
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

                    response ??= new DeleteFileResponse(
                                     Request,
                                     Request.FileName,
                                     DeleteFileStatus.Rejected,
                                     null,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = DeleteFileResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnDeleteFileResponseSent event

        /// <summary>
        /// An event sent whenever a DeleteFile response was sent.
        /// </summary>
        public event OnDeleteFileResponseSentDelegate?  OnDeleteFileResponseSent;

        public Task SendOnDeleteFileResponseSent(DateTime              Timestamp,
                                                 IEventSender          Sender,
                                                 IWebSocketConnection  Connection,
                                                 DeleteFileRequest     Request,
                                                 DeleteFileResponse    Response,
                                                 TimeSpan              Runtime,
                                                 SentMessageResults    SendMessageResult,
                                                 CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnDeleteFileResponseSent,
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

        #region Send OnDeleteFileRequestErrorSent event

        /// <summary>
        /// An event sent whenever a DeleteFile request error was sent.
        /// </summary>
        public event OnDeleteFileRequestErrorSentDelegate? OnDeleteFileRequestErrorSent;


        public Task SendOnDeleteFileRequestErrorSent(DateTime                      Timestamp,
                                                     IEventSender                  Sender,
                                                     IWebSocketConnection          Connection,
                                                     DeleteFileRequest?            Request,
                                                     OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                     TimeSpan                      Runtime,
                                                     SentMessageResults            SendMessageResult,
                                                     CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnDeleteFileRequestErrorSent,
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

        #region Send OnDeleteFileResponseErrorSent event

        /// <summary>
        /// An event sent whenever a DeleteFile response error was sent.
        /// </summary>
        public event OnDeleteFileResponseErrorSentDelegate? OnDeleteFileResponseErrorSent;


        public Task SendOnDeleteFileResponseErrorSent(DateTime                       Timestamp,
                                                      IEventSender                   Sender,
                                                      IWebSocketConnection           Connection,
                                                      DeleteFileRequest?             Request,
                                                      DeleteFileResponse?            Response,
                                                      OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                      TimeSpan                       Runtime,
                                                      SentMessageResults             SendMessageResult,
                                                      CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnDeleteFileResponseErrorSent,
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
