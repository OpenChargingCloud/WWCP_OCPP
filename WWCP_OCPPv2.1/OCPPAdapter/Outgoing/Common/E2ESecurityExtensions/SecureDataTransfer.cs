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
    /// A delegate called whenever a SecureDataTransfer request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSecureDataTransferRequestSentDelegate(DateTime                    Timestamp,
                                                                 IEventSender                Sender,
                                                                 IWebSocketConnection?       Connection,
                                                                 SecureDataTransferRequest   Request,
                                                                 SentMessageResults          SentMessageResult,
                                                                 CancellationToken           CancellationToken);


    /// <summary>
    /// A SecureDataTransfer response.
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

        OnSecureDataTransferResponseSentDelegate(DateTime                     Timestamp,
                                                 IEventSender                 Sender,
                                                 IWebSocketConnection?        Connection,
                                                 SecureDataTransferRequest    Request,
                                                 SecureDataTransferResponse   Response,
                                                 TimeSpan                     Runtime,
                                                 SentMessageResults           SentMessageResult,
                                                 CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SecureDataTransfer request error was sent.
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

        OnSecureDataTransferRequestErrorSentDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection?          Connection,
                                                     SecureDataTransferRequest?     Request,
                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                     TimeSpan?                      Runtime,
                                                     SentMessageResults             SentMessageResult,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SecureDataTransfer response error was sent.
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

        OnSecureDataTransferResponseErrorSentDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection?           Connection,
                                                      SecureDataTransferRequest?      Request,
                                                      SecureDataTransferResponse?     Response,
                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                      TimeSpan?                       Runtime,
                                                      SentMessageResults              SentMessageResult,
                                                      CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SecureDataTransfer request

        /// <summary>
        /// An event fired whenever a SecureDataTransfer request was sent.
        /// </summary>
        public event OnSecureDataTransferRequestSentDelegate?  OnSecureDataTransferRequestSent;


        /// <summary>
        /// Send a SecureDataTransfer request.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        public async Task<SecureDataTransferResponse>

            SecureDataTransfer(SecureDataTransferRequest Request)

        {

            SecureDataTransferResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToBinary(
                            parentNetworkingNode.OCPP.CustomSecureDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var signingErrors
                    ))
                {

                    response = SecureDataTransferResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomSecureDataTransferRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                                             IncludeSignatures: true
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnSecureDataTransferRequestSent,
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

                    if (sendRequestState.IsValidBinaryResponse(Request, out var binaryResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_SecureDataTransferResponse(
                                             Request,
                                             binaryResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_SecureDataTransferRequestError(
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

                    response ??= new SecureDataTransferResponse(
                                     Request,
                                     SecureDataTransferStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SecureDataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSecureDataTransferResponseSent event

        /// <summary>
        /// An event sent whenever a SecureDataTransfer response was sent.
        /// </summary>
        public event OnSecureDataTransferResponseSentDelegate?  OnSecureDataTransferResponseSent;

        public Task SendOnSecureDataTransferResponseSent(DateTime                    Timestamp,
                                                         IEventSender                Sender,
                                                         IWebSocketConnection?       Connection,
                                                         SecureDataTransferRequest   Request,
                                                         SecureDataTransferResponse  Response,
                                                         TimeSpan                    Runtime,
                                                         SentMessageResults          SentMessageResult,
                                                         CancellationToken           CancellationToken = default)

            => LogEvent(
                   OnSecureDataTransferResponseSent,
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

        #region Send OnSecureDataTransferRequestErrorSent event

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request error was sent.
        /// </summary>
        public event OnSecureDataTransferRequestErrorSentDelegate? OnSecureDataTransferRequestErrorSent;


        public Task SendOnSecureDataTransferRequestErrorSent(DateTime                      Timestamp,
                                                             IEventSender                  Sender,
                                                             IWebSocketConnection?         Connection,
                                                             SecureDataTransferRequest?    Request,
                                                             OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                             TimeSpan                      Runtime,
                                                             SentMessageResults            SentMessageResult,
                                                             CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSecureDataTransferRequestErrorSent,
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

        #region Send OnSecureDataTransferResponseErrorSent event

        /// <summary>
        /// An event sent whenever a SecureDataTransfer response error was sent.
        /// </summary>
        public event OnSecureDataTransferResponseErrorSentDelegate? OnSecureDataTransferResponseErrorSent;


        public Task SendOnSecureDataTransferResponseErrorSent(DateTime                       Timestamp,
                                                              IEventSender                   Sender,
                                                              IWebSocketConnection?          Connection,
                                                              SecureDataTransferRequest?     Request,
                                                              SecureDataTransferResponse?    Response,
                                                              OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                              TimeSpan                       Runtime,
                                                              SentMessageResults             SentMessageResult,
                                                              CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSecureDataTransferResponseErrorSent,
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
