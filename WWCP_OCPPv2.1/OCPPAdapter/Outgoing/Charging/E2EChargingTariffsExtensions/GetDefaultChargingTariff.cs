/*
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
    /// A logging delegate called whenever an GetDefaultChargingTariff request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDefaultChargingTariffRequestSentDelegate(DateTime                          Timestamp,
                                                                       IEventSender                      Sender,
                                                                       IWebSocketConnection?             Connection,
                                                                       GetDefaultChargingTariffRequest   Request,
                                                                       SentMessageResults                SentMessageResult,
                                                                       CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff response was sent.
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

        OnGetDefaultChargingTariffResponseSentDelegate(DateTime                           Timestamp,
                                                       IEventSender                       Sender,
                                                       IWebSocketConnection?              Connection,
                                                       GetDefaultChargingTariffRequest?   Request,
                                                       GetDefaultChargingTariffResponse   Response,
                                                       TimeSpan                           Runtime,
                                                       SentMessageResults                 SentMessageResult,
                                                       CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff request error was sent.
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

        OnGetDefaultChargingTariffRequestErrorSentDelegate(DateTime                           Timestamp,
                                                           IEventSender                       Sender,
                                                           IWebSocketConnection?              Connection,
                                                           GetDefaultChargingTariffRequest?   Request,
                                                           OCPP_JSONRequestErrorMessage       RequestErrorMessage,
                                                           TimeSpan?                          Runtime,
                                                           SentMessageResults                 SentMessageResult,
                                                           CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff response error was sent.
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

        OnGetDefaultChargingTariffResponseErrorSentDelegate(DateTime                            Timestamp,
                                                            IEventSender                        Sender,
                                                            IWebSocketConnection?               Connection,
                                                            GetDefaultChargingTariffRequest?    Request,
                                                            GetDefaultChargingTariffResponse?   Response,
                                                            OCPP_JSONResponseErrorMessage       ResponseErrorMessage,
                                                            TimeSpan?                           Runtime,
                                                            SentMessageResults                  SentMessageResult,
                                                            CancellationToken                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send GetDefaultChargingTariff request

        /// <summary>
        /// An event fired whenever an GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestSentDelegate?  OnGetDefaultChargingTariffRequestSent;


        /// <summary>
        /// GetDefaultChargingTariff the given identification token.
        /// </summary>
        /// <param name="Request">An GetDefaultChargingTariff request.</param>
        public async Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)
        {

            GetDefaultChargingTariffResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = GetDefaultChargingTariffResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnGetDefaultChargingTariffRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetDefaultChargingTariffResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetDefaultChargingTariffRequestError(
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

                    response ??= new GetDefaultChargingTariffResponse(
                                     Request,
                                     GenericStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = GetDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnGetDefaultChargingTariffResponseSent event

        /// <summary>
        /// An event sent whenever an GetDefaultChargingTariff response was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseSentDelegate?  OnGetDefaultChargingTariffResponseSent;


        public Task SendOnGetDefaultChargingTariffResponseSent(DateTime                          Timestamp,
                                                               IEventSender                      Sender,
                                                               IWebSocketConnection?             Connection,
                                                               GetDefaultChargingTariffRequest   Request,
                                                               GetDefaultChargingTariffResponse  Response,
                                                               TimeSpan                          Runtime,
                                                               SentMessageResults                SentMessageResult,
                                                               CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnGetDefaultChargingTariffResponseSent,
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

        #region Send OnGetDefaultChargingTariffRequestErrorSent event

        /// <summary>
        /// An event sent whenever an GetDefaultChargingTariff request error was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestErrorSentDelegate? OnGetDefaultChargingTariffRequestErrorSent;


        public Task SendOnGetDefaultChargingTariffRequestErrorSent(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   IWebSocketConnection?             Connection,
                                                                   GetDefaultChargingTariffRequest?  Request,
                                                                   OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                   TimeSpan                          Runtime,
                                                                   SentMessageResults                SentMessageResult,
                                                                   CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnGetDefaultChargingTariffRequestErrorSent,
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

        #region Send OnGetDefaultChargingTariffResponseErrorSent event

        /// <summary>
        /// An event sent whenever an GetDefaultChargingTariff response error was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseErrorSentDelegate? OnGetDefaultChargingTariffResponseErrorSent;


        public Task SendOnGetDefaultChargingTariffResponseErrorSent(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    IWebSocketConnection?              Connection,
                                                                    GetDefaultChargingTariffRequest?   Request,
                                                                    GetDefaultChargingTariffResponse?  Response,
                                                                    OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                    TimeSpan                           Runtime,
                                                                    SentMessageResults                 SentMessageResult,
                                                                    CancellationToken                  CancellationToken = default)

            => LogEvent(
                   OnGetDefaultChargingTariffResponseErrorSent,
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
