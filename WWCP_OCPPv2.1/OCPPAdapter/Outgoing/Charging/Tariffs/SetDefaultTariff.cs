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
    /// A logging delegate called whenever an SetDefaultTariff request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultTariffRequestSentDelegate(DateTime                  Timestamp,
                                                               IEventSender              Sender,
                                                               IWebSocketConnection?     Connection,
                                                               SetDefaultTariffRequest   Request,
                                                               SentMessageResults        SentMessageResult,
                                                               CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff response was sent.
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

        OnSetDefaultTariffResponseSentDelegate(DateTime                   Timestamp,
                                               IEventSender               Sender,
                                               IWebSocketConnection?      Connection,
                                               SetDefaultTariffRequest?   Request,
                                               SetDefaultTariffResponse   Response,
                                               TimeSpan                   Runtime,
                                               SentMessageResults         SentMessageResult,
                                               CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff request error was sent.
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

        OnSetDefaultTariffRequestErrorSentDelegate(DateTime                       Timestamp,
                                                   IEventSender                   Sender,
                                                   IWebSocketConnection?          Connection,
                                                   SetDefaultTariffRequest?       Request,
                                                   OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                   TimeSpan?                      Runtime,
                                                   SentMessageResults             SentMessageResult,
                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff response error was sent.
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

        OnSetDefaultTariffResponseErrorSentDelegate(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    IWebSocketConnection?           Connection,
                                                    SetDefaultTariffRequest?        Request,
                                                    SetDefaultTariffResponse?       Response,
                                                    OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                    TimeSpan?                       Runtime,
                                                    SentMessageResults              SentMessageResult,
                                                    CancellationToken               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SetDefaultTariff request

        /// <summary>
        /// An event fired whenever an SetDefaultTariff request was sent.
        /// </summary>
        public event OnSetDefaultTariffRequestSentDelegate?  OnSetDefaultTariffRequestSent;


        /// <summary>
        /// SetDefaultTariff the given identification token.
        /// </summary>
        /// <param name="Request">An SetDefaultTariff request.</param>
        public async Task<SetDefaultTariffResponse> SetDefaultTariff(SetDefaultTariffRequest Request)
        {

            SetDefaultTariffResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestSerializer,
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

                    response = SetDefaultTariffResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestSerializer,
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
                                                         OnSetDefaultTariffRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDefaultTariffResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDefaultTariffRequestError(
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

                    response ??= new SetDefaultTariffResponse(
                                     Request,
                                     TariffStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SetDefaultTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSetDefaultTariffResponseSent event

        /// <summary>
        /// An event sent whenever an SetDefaultTariff response was sent.
        /// </summary>
        public event OnSetDefaultTariffResponseSentDelegate?  OnSetDefaultTariffResponseSent;


        public Task SendOnSetDefaultTariffResponseSent(DateTime                  Timestamp,
                                                       IEventSender              Sender,
                                                       IWebSocketConnection?     Connection,
                                                       SetDefaultTariffRequest   Request,
                                                       SetDefaultTariffResponse  Response,
                                                       TimeSpan                  Runtime,
                                                       SentMessageResults        SentMessageResult,
                                                       CancellationToken         CancellationToken = default)

            => LogEvent(
                   OnSetDefaultTariffResponseSent,
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

        #region Send OnSetDefaultTariffRequestErrorSent event

        /// <summary>
        /// An event sent whenever an SetDefaultTariff request error was sent.
        /// </summary>
        public event OnSetDefaultTariffRequestErrorSentDelegate? OnSetDefaultTariffRequestErrorSent;


        public Task SendOnSetDefaultTariffRequestErrorSent(DateTime                      Timestamp,
                                                           IEventSender                  Sender,
                                                           IWebSocketConnection?         Connection,
                                                           SetDefaultTariffRequest?      Request,
                                                           OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                           TimeSpan                      Runtime,
                                                           SentMessageResults            SentMessageResult,
                                                           CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSetDefaultTariffRequestErrorSent,
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

        #region Send OnSetDefaultTariffResponseErrorSent event

        /// <summary>
        /// An event sent whenever an SetDefaultTariff response error was sent.
        /// </summary>
        public event OnSetDefaultTariffResponseErrorSentDelegate? OnSetDefaultTariffResponseErrorSent;


        public Task SendOnSetDefaultTariffResponseErrorSent(DateTime                       Timestamp,
                                                            IEventSender                   Sender,
                                                            IWebSocketConnection?          Connection,
                                                            SetDefaultTariffRequest?       Request,
                                                            SetDefaultTariffResponse?      Response,
                                                            OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                            TimeSpan                       Runtime,
                                                            SentMessageResults             SentMessageResult,
                                                            CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSetDefaultTariffResponseErrorSent,
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
