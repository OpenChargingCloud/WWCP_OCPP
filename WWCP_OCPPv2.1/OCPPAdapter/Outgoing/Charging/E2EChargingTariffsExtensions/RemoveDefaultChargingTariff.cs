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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRemoveDefaultChargingTariffRequestSentDelegate(DateTime                             Timestamp,
                                                                          IEventSender                         Sender,
                                                                          IWebSocketConnection?                Connection,
                                                                          RemoveDefaultChargingTariffRequest   Request,
                                                                          SentMessageResults                   SentMessageResult,
                                                                          CancellationToken                    CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff response was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffResponseSentDelegate(DateTime                              Timestamp,
                                                          IEventSender                          Sender,
                                                          IWebSocketConnection?                 Connection,
                                                          RemoveDefaultChargingTariffRequest?   Request,
                                                          RemoveDefaultChargingTariffResponse   Response,
                                                          TimeSpan                              Runtime,
                                                          SentMessageResults                    SentMessageResult,
                                                          CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffRequestErrorSentDelegate(DateTime                              Timestamp,
                                                              IEventSender                          Sender,
                                                              IWebSocketConnection?                 Connection,
                                                              RemoveDefaultChargingTariffRequest?   Request,
                                                              OCPP_JSONRequestErrorMessage          RequestErrorMessage,
                                                              TimeSpan?                             Runtime,
                                                              SentMessageResults                    SentMessageResult,
                                                              CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffResponseErrorSentDelegate(DateTime                               Timestamp,
                                                               IEventSender                           Sender,
                                                               IWebSocketConnection?                  Connection,
                                                               RemoveDefaultChargingTariffRequest?    Request,
                                                               RemoveDefaultChargingTariffResponse?   Response,
                                                               OCPP_JSONResponseErrorMessage          ResponseErrorMessage,
                                                               TimeSpan?                              Runtime,
                                                               SentMessageResults                     SentMessageResult,
                                                               CancellationToken                      CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send RemoveDefaultChargingTariff request

        /// <summary>
        /// An event fired whenever an RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestSentDelegate?  OnRemoveDefaultChargingTariffRequestSent;


        /// <summary>
        /// RemoveDefaultChargingTariff the given identification token.
        /// </summary>
        /// <param name="Request">An RemoveDefaultChargingTariff request.</param>
        public async Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)
        {

            RemoveDefaultChargingTariffResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = RemoveDefaultChargingTariffResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnRemoveDefaultChargingTariffRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_RemoveDefaultChargingTariffResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_RemoveDefaultChargingTariffRequestError(
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

                    response ??= new RemoveDefaultChargingTariffResponse(
                                     Request,
                                     RemoveDefaultChargingTariffStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = RemoveDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnRemoveDefaultChargingTariffResponseSent event

        /// <summary>
        /// An event sent whenever an RemoveDefaultChargingTariff response was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseSentDelegate?  OnRemoveDefaultChargingTariffResponseSent;


        public Task SendOnRemoveDefaultChargingTariffResponseSent(DateTime                             Timestamp,
                                                                  IEventSender                         Sender,
                                                                  IWebSocketConnection?                Connection,
                                                                  RemoveDefaultChargingTariffRequest   Request,
                                                                  RemoveDefaultChargingTariffResponse  Response,
                                                                  TimeSpan                             Runtime,
                                                                  SentMessageResults                   SentMessageResult,
                                                                  CancellationToken                    CancellationToken = default)

            => LogEvent(
                   OnRemoveDefaultChargingTariffResponseSent,
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

        #region Send OnRemoveDefaultChargingTariffRequestErrorSent event

        /// <summary>
        /// An event sent whenever an RemoveDefaultChargingTariff request error was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestErrorSentDelegate? OnRemoveDefaultChargingTariffRequestErrorSent;


        public Task SendOnRemoveDefaultChargingTariffRequestErrorSent(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      IWebSocketConnection?                Connection,
                                                                      RemoveDefaultChargingTariffRequest?  Request,
                                                                      OCPP_JSONRequestErrorMessage         RequestErrorMessage,
                                                                      TimeSpan                             Runtime,
                                                                      SentMessageResults                   SentMessageResult,
                                                                      CancellationToken                    CancellationToken = default)

            => LogEvent(
                   OnRemoveDefaultChargingTariffRequestErrorSent,
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

        #region Send OnRemoveDefaultChargingTariffResponseErrorSent event

        /// <summary>
        /// An event sent whenever an RemoveDefaultChargingTariff response error was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseErrorSentDelegate? OnRemoveDefaultChargingTariffResponseErrorSent;


        public Task SendOnRemoveDefaultChargingTariffResponseErrorSent(DateTime                              Timestamp,
                                                                       IEventSender                          Sender,
                                                                       IWebSocketConnection?                 Connection,
                                                                       RemoveDefaultChargingTariffRequest?   Request,
                                                                       RemoveDefaultChargingTariffResponse?  Response,
                                                                       OCPP_JSONResponseErrorMessage         ResponseErrorMessage,
                                                                       TimeSpan                              Runtime,
                                                                       SentMessageResults                    SentMessageResult,
                                                                       CancellationToken                     CancellationToken = default)

            => LogEvent(
                   OnRemoveDefaultChargingTariffResponseErrorSent,
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
