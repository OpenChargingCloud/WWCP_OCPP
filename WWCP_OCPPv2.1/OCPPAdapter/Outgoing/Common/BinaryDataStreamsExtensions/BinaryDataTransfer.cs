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
    /// A logging delegate called whenever a BinaryDataTransfer request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task OnBinaryDataTransferRequestSentDelegate(DateTime                    Timestamp,
                                                                 IEventSender                Sender,
                                                                 IWebSocketConnection?       Connection,
                                                                 BinaryDataTransferRequest   Request,
                                                                 SentMessageResults          SentMessageResult,
                                                                 CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer response was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnBinaryDataTransferResponseSentDelegate(DateTime                     Timestamp,
                                                 IEventSender                 Sender,
                                                 IWebSocketConnection?        Connection,
                                                 BinaryDataTransferRequest?   Request,
                                                 BinaryDataTransferResponse   Response,
                                                 TimeSpan?                    Runtime,
                                                 SentMessageResults           SentMessageResult,
                                                 CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnBinaryDataTransferRequestErrorSentDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection?          Connection,
                                                     BinaryDataTransferRequest?     Request,
                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                     TimeSpan?                      Runtime,
                                                     SentMessageResults             SentMessageResult,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnBinaryDataTransferResponseErrorSentDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection?           Connection,
                                                      BinaryDataTransferRequest?      Request,
                                                      BinaryDataTransferResponse?     Response,
                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                      TimeSpan?                       Runtime,
                                                      SentMessageResults              SentMessageResult,
                                                      CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send BinaryDataTransfer request

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnBinaryDataTransferRequestSentDelegate?  OnBinaryDataTransferRequestSent;


        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        {

            BinaryDataTransferResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var signingErrors
                    ))
                {

                    response = BinaryDataTransferResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                                             IncludeSignatures: true
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnBinaryDataTransferRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_BinaryDataTransferResponse(
                                             Request,
                                             binaryResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    if (sendRequestState.IsValidBinaryRequestError(Request, out var binaryRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_BinaryDataTransferRequestError(
                                             Request,
                                             binaryRequestError,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    response ??= new BinaryDataTransferResponse(
                                     Request,
                                     BinaryDataTransferStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = BinaryDataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnBinaryDataTransferResponseSent event

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer response was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseSentDelegate?  OnBinaryDataTransferResponseSent;


        public Task SendOnBinaryDataTransferResponseSent(DateTime                    Timestamp,
                                                         IEventSender                Sender,
                                                         IWebSocketConnection?       Connection,
                                                         BinaryDataTransferRequest   Request,
                                                         BinaryDataTransferResponse  Response,
                                                         TimeSpan                    Runtime,
                                                         SentMessageResults          SentMessageResult,
                                                         CancellationToken           CancellationToken = default)

            => LogEvent(
                   OnBinaryDataTransferResponseSent,
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

        #region Send OnBinaryDataTransferRequestErrorSent event

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request error was sent.
        /// </summary>
        public event OnBinaryDataTransferRequestErrorSentDelegate? OnBinaryDataTransferRequestErrorSent;


        public Task SendOnBinaryDataTransferRequestErrorSent(DateTime                      Timestamp,
                                                             IEventSender                  Sender,
                                                             IWebSocketConnection?         Connection,
                                                             BinaryDataTransferRequest?    Request,
                                                             OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                             TimeSpan                      Runtime,
                                                             SentMessageResults            SentMessageResult,
                                                             CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnBinaryDataTransferRequestErrorSent,
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

        #region Send OnBinaryDataTransferResponseErrorSent event

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer response error was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseErrorSentDelegate? OnBinaryDataTransferResponseErrorSent;


        public Task SendOnBinaryDataTransferResponseErrorSent(DateTime                       Timestamp,
                                                              IEventSender                   Sender,
                                                              IWebSocketConnection?          Connection,
                                                              BinaryDataTransferRequest?     Request,
                                                              BinaryDataTransferResponse?    Response,
                                                              OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                              TimeSpan                       Runtime,
                                                              SentMessageResults             SentMessageResult,
                                                              CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnBinaryDataTransferResponseErrorSent,
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
