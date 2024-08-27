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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeTransactionTariffRequestSentDelegate(DateTime                         Timestamp,
                                                                      IEventSender                     Sender,
                                                                      IWebSocketConnection?            Connection,
                                                                      ChangeTransactionTariffRequest   Request,
                                                                      SentMessageResults               SentMessageResult,
                                                                      CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff response was sent.
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

        OnChangeTransactionTariffResponseSentDelegate(DateTime                          Timestamp,
                                                      IEventSender                      Sender,
                                                      IWebSocketConnection?             Connection,
                                                      ChangeTransactionTariffRequest?   Request,
                                                      ChangeTransactionTariffResponse   Response,
                                                      TimeSpan                          Runtime,
                                                      SentMessageResults                SentMessageResult,
                                                      CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff request error was sent.
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

        OnChangeTransactionTariffRequestErrorSentDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection?             Connection,
                                                          ChangeTransactionTariffRequest?   Request,
                                                          OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                          TimeSpan?                         Runtime,
                                                          SentMessageResults                SentMessageResult,
                                                          CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff response error was sent.
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

        OnChangeTransactionTariffResponseErrorSentDelegate(DateTime                           Timestamp,
                                                           IEventSender                       Sender,
                                                           IWebSocketConnection?              Connection,
                                                           ChangeTransactionTariffRequest?    Request,
                                                           ChangeTransactionTariffResponse?   Response,
                                                           OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                           TimeSpan?                          Runtime,
                                                           SentMessageResults                 SentMessageResult,
                                                           CancellationToken                  CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send ChangeTransactionTariff request

        /// <summary>
        /// An event fired whenever an ChangeTransactionTariff request was sent.
        /// </summary>
        public event OnChangeTransactionTariffRequestSentDelegate?  OnChangeTransactionTariffRequestSent;


        /// <summary>
        /// ChangeTransactionTariff the given identification token.
        /// </summary>
        /// <param name="Request">An ChangeTransactionTariff request.</param>
        public async Task<ChangeTransactionTariffResponse> ChangeTransactionTariff(ChangeTransactionTariffRequest Request)
        {

            ChangeTransactionTariffResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomChangeTransactionTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomTariffSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            parentNetworkingNode.OCPP.CustomTariffConditionsSerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergySerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergyPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimeSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimePriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedPriceSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = ChangeTransactionTariffResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomChangeTransactionTariffRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffSerializer,
                                                             parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceSerializer,
                                                             parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffConditionsSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffEnergySerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffEnergyPriceSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffTimeSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffTimePriceSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffFixedSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffFixedPriceSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnChangeTransactionTariffRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ChangeTransactionTariffResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ChangeTransactionTariffRequestError(
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

                    response ??= new ChangeTransactionTariffResponse(
                                     Request,
                                     TariffStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = ChangeTransactionTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnChangeTransactionTariffResponseSent event

        /// <summary>
        /// An event sent whenever an ChangeTransactionTariff response was sent.
        /// </summary>
        public event OnChangeTransactionTariffResponseSentDelegate?  OnChangeTransactionTariffResponseSent;


        public Task SendOnChangeTransactionTariffResponseSent(DateTime                         Timestamp,
                                                              IEventSender                     Sender,
                                                              IWebSocketConnection?            Connection,
                                                              ChangeTransactionTariffRequest   Request,
                                                              ChangeTransactionTariffResponse  Response,
                                                              TimeSpan                         Runtime,
                                                              SentMessageResults               SentMessageResult,
                                                              CancellationToken                CancellationToken = default)

            => LogEvent(
                   OnChangeTransactionTariffResponseSent,
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

        #region Send OnChangeTransactionTariffRequestErrorSent event

        /// <summary>
        /// An event sent whenever an ChangeTransactionTariff request error was sent.
        /// </summary>
        public event OnChangeTransactionTariffRequestErrorSentDelegate? OnChangeTransactionTariffRequestErrorSent;


        public Task SendOnChangeTransactionTariffRequestErrorSent(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  IWebSocketConnection?            Connection,
                                                                  ChangeTransactionTariffRequest?  Request,
                                                                  OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                                  TimeSpan                         Runtime,
                                                                  SentMessageResults               SentMessageResult,
                                                                  CancellationToken                CancellationToken = default)

            => LogEvent(
                   OnChangeTransactionTariffRequestErrorSent,
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

        #region Send OnChangeTransactionTariffResponseErrorSent event

        /// <summary>
        /// An event sent whenever an ChangeTransactionTariff response error was sent.
        /// </summary>
        public event OnChangeTransactionTariffResponseErrorSentDelegate? OnChangeTransactionTariffResponseErrorSent;


        public Task SendOnChangeTransactionTariffResponseErrorSent(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   IWebSocketConnection?             Connection,
                                                                   ChangeTransactionTariffRequest?   Request,
                                                                   ChangeTransactionTariffResponse?  Response,
                                                                   OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                                   TimeSpan                          Runtime,
                                                                   SentMessageResults                SentMessageResult,
                                                                   CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnChangeTransactionTariffResponseErrorSent,
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
