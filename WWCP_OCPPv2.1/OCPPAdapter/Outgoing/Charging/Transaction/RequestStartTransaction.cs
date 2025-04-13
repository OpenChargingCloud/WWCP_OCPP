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
    /// A delegate called whenever a RequestStartTransaction request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRequestStartTransactionRequestSentDelegate(DateTime                         Timestamp,
                                                                      IEventSender                     Sender,
                                                                      IWebSocketConnection?            Connection,
                                                                      RequestStartTransactionRequest   Request,
                                                                      SentMessageResults               SentMessageResult,
                                                                      CancellationToken                CancellationToken);


    /// <summary>
    /// A RequestStartTransaction response.
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

        OnRequestStartTransactionResponseSentDelegate(DateTime                          Timestamp,
                                                      IEventSender                      Sender,
                                                      IWebSocketConnection?             Connection,
                                                      RequestStartTransactionRequest    Request,
                                                      RequestStartTransactionResponse   Response,
                                                      TimeSpan                          Runtime,
                                                      SentMessageResults                SentMessageResult,
                                                      CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a RequestStartTransaction request error was sent.
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

        OnRequestStartTransactionRequestErrorSentDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection?             Connection,
                                                          RequestStartTransactionRequest?   Request,
                                                          OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                          TimeSpan?                         Runtime,
                                                          SentMessageResults                SentMessageResult,
                                                          CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a RequestStartTransaction response error was sent.
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

        OnRequestStartTransactionResponseErrorSentDelegate(DateTime                           Timestamp,
                                                           IEventSender                       Sender,
                                                           IWebSocketConnection?              Connection,
                                                           RequestStartTransactionRequest?    Request,
                                                           RequestStartTransactionResponse?   Response,
                                                           OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                           TimeSpan?                          Runtime,
                                                           SentMessageResults                 SentMessageResult,
                                                           CancellationToken                  CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send RequestStartTransaction request

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionRequestSentDelegate?  OnRequestStartTransactionRequestSent;


        /// <summary>
        /// Send a RequestStartTransaction request.
        /// </summary>
        /// <param name="Request">A RequestStartTransaction request.</param>
        public async Task<RequestStartTransactionResponse>

            RequestStartTransaction(RequestStartTransactionRequest Request)

        {

            RequestStartTransactionResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(

                            true,

                            parentNetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                            parentNetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                            parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                            parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                            parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                            parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                            parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                            parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                            parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                            parentNetworkingNode.OCPP.CustomCostSerializer,

                            parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                            parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                            parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                            parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                            parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                            parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer

                        ),
                        out var signingErrors
                    ))
                {

                    response = RequestStartTransactionResponse.SignatureError(
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

                                                             parentNetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                                                             parentNetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                                                             parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                                                             parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                                                             parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                                                             parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                                                             parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                                                             parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                                                             parentNetworkingNode.OCPP.CustomCostSerializer,

                                                             parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                                                             parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                                                             parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                                                             parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                                                             parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                                                             parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer

                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnRequestStartTransactionRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_RequestStartTransactionResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_RequestStartTransactionRequestError(
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

                    response ??= new RequestStartTransactionResponse(
                                     Request,
                                     RequestStartStopStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = RequestStartTransactionResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnRequestStartTransactionResponseSent event

        /// <summary>
        /// An event sent whenever a RequestStartTransaction response was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseSentDelegate?  OnRequestStartTransactionResponseSent;

        public Task SendOnRequestStartTransactionResponseSent(DateTime                          Timestamp,
                                                              IEventSender                      Sender,
                                                              IWebSocketConnection?             Connection,
                                                              RequestStartTransactionRequest    Request,
                                                              RequestStartTransactionResponse   Response,
                                                              TimeSpan                          Runtime,
                                                              SentMessageResults                SentMessageResult,
                                                              CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnRequestStartTransactionResponseSent,
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

        #region Send OnRequestStartTransactionRequestErrorSent event

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request error was sent.
        /// </summary>
        public event OnRequestStartTransactionRequestErrorSentDelegate? OnRequestStartTransactionRequestErrorSent;


        public Task SendOnRequestStartTransactionRequestErrorSent(DateTime                          Timestamp,
                                                                  IEventSender                      Sender,
                                                                  IWebSocketConnection?             Connection,
                                                                  RequestStartTransactionRequest?   Request,
                                                                  OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                  TimeSpan                          Runtime,
                                                                  SentMessageResults                SentMessageResult,
                                                                  CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnRequestStartTransactionRequestErrorSent,
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

        #region Send OnRequestStartTransactionResponseErrorSent event

        /// <summary>
        /// An event sent whenever a RequestStartTransaction response error was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseErrorSentDelegate? OnRequestStartTransactionResponseErrorSent;


        public Task SendOnRequestStartTransactionResponseErrorSent(DateTime                           Timestamp,
                                                                   IEventSender                       Sender,
                                                                   IWebSocketConnection?              Connection,
                                                                   RequestStartTransactionRequest?    Request,
                                                                   RequestStartTransactionResponse?   Response,
                                                                   OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                   TimeSpan                           Runtime,
                                                                   SentMessageResults                 SentMessageResult,
                                                                   CancellationToken                  CancellationToken = default)

            => LogEvent(
                   OnRequestStartTransactionResponseErrorSent,
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
