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
    /// A logging delegate called whenever a DataTransfer request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task OnDataTransferRequestSentDelegate(DateTime                Timestamp,
                                                           IEventSender            Sender,
                                                           IWebSocketConnection?   Connection,
                                                           DataTransferRequest     Request,
                                                           SentMessageResults      SentMessageResult,
                                                           CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer response was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnDataTransferResponseSentDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection?  Connection,
                                           DataTransferRequest?   Request,
                                           DataTransferResponse   Response,
                                           TimeSpan?              Runtime,
                                           SentMessageResults     SentMessageResult,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnDataTransferRequestErrorSentDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               IWebSocketConnection?          Connection,
                                               DataTransferRequest?           Request,
                                               OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                               TimeSpan?                      Runtime,
                                               SentMessageResults             SentMessageResult,
                                               CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer response error was sent.
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

        OnDataTransferResponseErrorSentDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                IWebSocketConnection?           Connection,
                                                DataTransferRequest?            Request,
                                                DataTransferResponse?           Response,
                                                OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                TimeSpan?                       Runtime,
                                                SentMessageResults              SentMessageResult,
                                                CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send DataTransfer request

        /// <summary>
        /// An event fired whenever a DataTransfer request was sent.
        /// </summary>
        public event OnDataTransferRequestSentDelegate?  OnDataTransferRequestSent;


        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<DataTransferResponse> DataTransfer(DataTransferRequest Request)
        {

            DataTransferResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = DataTransferResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnDataTransferRequestSent,
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


                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_DataTransferResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_DataTransferRequestError(
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

                    response ??= new DataTransferResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = DataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnDataTransferResponseSent event

        /// <summary>
        /// An event sent whenever a DataTransfer response was sent.
        /// </summary>
        public event OnDataTransferResponseSentDelegate?  OnDataTransferResponseSent;


        public Task SendOnDataTransferResponseSent(DateTime              Timestamp,
                                                   IEventSender          Sender,
                                                   IWebSocketConnection? Connection,
                                                   DataTransferRequest   Request,
                                                   DataTransferResponse  Response,
                                                   TimeSpan              Runtime,
                                                   SentMessageResults    SentMessageResult,
                                                   CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnDataTransferResponseSent,
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

        #region Send OnDataTransferRequestErrorSent event

        /// <summary>
        /// An event sent whenever a DataTransfer request error was sent.
        /// </summary>
        public event OnDataTransferRequestErrorSentDelegate? OnDataTransferRequestErrorSent;


        public Task SendOnDataTransferRequestErrorSent(DateTime                      Timestamp,
                                                       IEventSender                  Sender,
                                                       IWebSocketConnection?         Connection,
                                                       DataTransferRequest?          Request,
                                                       OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                       TimeSpan                      Runtime,
                                                       SentMessageResults            SentMessageResult,
                                                       CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnDataTransferRequestErrorSent,
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

        #region Send OnDataTransferResponseErrorSent event

        /// <summary>
        /// An event sent whenever a DataTransfer response error was sent.
        /// </summary>
        public event OnDataTransferResponseErrorSentDelegate? OnDataTransferResponseErrorSent;


        public Task SendOnDataTransferResponseErrorSent(DateTime                       Timestamp,
                                                        IEventSender                   Sender,
                                                        IWebSocketConnection?          Connection,
                                                        DataTransferRequest?           Request,
                                                        DataTransferResponse?          Response,
                                                        OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                        TimeSpan                       Runtime,
                                                        SentMessageResults             SentMessageResult,
                                                        CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnDataTransferResponseErrorSent,
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
