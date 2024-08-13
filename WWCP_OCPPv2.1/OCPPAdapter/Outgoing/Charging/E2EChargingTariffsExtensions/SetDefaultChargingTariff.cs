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
    /// A logging delegate called whenever an SetDefaultChargingTariff request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultChargingTariffRequestSentDelegate(DateTime                          Timestamp,
                                                                       IEventSender                      Sender,
                                                                       IWebSocketConnection?             Connection,
                                                                       SetDefaultChargingTariffRequest   Request,
                                                                       SentMessageResults                SentMessageResult,
                                                                       CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff response was sent.
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

        OnSetDefaultChargingTariffResponseSentDelegate(DateTime                           Timestamp,
                                                       IEventSender                       Sender,
                                                       IWebSocketConnection?              Connection,
                                                       SetDefaultChargingTariffRequest?   Request,
                                                       SetDefaultChargingTariffResponse   Response,
                                                       TimeSpan                           Runtime,
                                                       SentMessageResults                 SentMessageResult,
                                                       CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff request error was sent.
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

        OnSetDefaultChargingTariffRequestErrorSentDelegate(DateTime                           Timestamp,
                                                           IEventSender                       Sender,
                                                           IWebSocketConnection?              Connection,
                                                           SetDefaultChargingTariffRequest?   Request,
                                                           OCPP_JSONRequestErrorMessage       RequestErrorMessage,
                                                           TimeSpan?                          Runtime,
                                                           SentMessageResults                 SentMessageResult,
                                                           CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff response error was sent.
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

        OnSetDefaultChargingTariffResponseErrorSentDelegate(DateTime                            Timestamp,
                                                            IEventSender                        Sender,
                                                            IWebSocketConnection?               Connection,
                                                            SetDefaultChargingTariffRequest?    Request,
                                                            SetDefaultChargingTariffResponse?   Response,
                                                            OCPP_JSONResponseErrorMessage       ResponseErrorMessage,
                                                            TimeSpan?                           Runtime,
                                                            SentMessageResults                  SentMessageResult,
                                                            CancellationToken                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SetDefaultChargingTariff request

        /// <summary>
        /// An event fired whenever an SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestSentDelegate?  OnSetDefaultChargingTariffRequestSent;


        /// <summary>
        /// SetDefaultChargingTariff the given identification token.
        /// </summary>
        /// <param name="Request">An SetDefaultChargingTariff request.</param>
        public async Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)
        {

            SetDefaultChargingTariffResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
                            parentNetworkingNode.OCPP.CustomPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                            parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                            parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                            parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                            parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = SetDefaultChargingTariffResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                                                             parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                                                             parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                                                             parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                                                             parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                                                             parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                                                             parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnSetDefaultChargingTariffRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDefaultChargingTariffResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDefaultChargingTariffRequestError(
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

                    response ??= new SetDefaultChargingTariffResponse(
                                     Request,
                                     SetDefaultChargingTariffStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SetDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSetDefaultChargingTariffResponseSent event

        /// <summary>
        /// An event sent whenever an SetDefaultChargingTariff response was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseSentDelegate?  OnSetDefaultChargingTariffResponseSent;


        public Task SendOnSetDefaultChargingTariffResponseSent(DateTime                          Timestamp,
                                                               IEventSender                      Sender,
                                                               IWebSocketConnection?             Connection,
                                                               SetDefaultChargingTariffRequest   Request,
                                                               SetDefaultChargingTariffResponse  Response,
                                                               TimeSpan                          Runtime,
                                                               SentMessageResults                SentMessageResult,
                                                               CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnSetDefaultChargingTariffResponseSent,
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

        #region Send OnSetDefaultChargingTariffRequestErrorSent event

        /// <summary>
        /// An event sent whenever an SetDefaultChargingTariff request error was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestErrorSentDelegate? OnSetDefaultChargingTariffRequestErrorSent;


        public Task SendOnSetDefaultChargingTariffRequestErrorSent(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   IWebSocketConnection?             Connection,
                                                                   SetDefaultChargingTariffRequest?  Request,
                                                                   OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                   TimeSpan                          Runtime,
                                                                   SentMessageResults                SentMessageResult,
                                                                   CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnSetDefaultChargingTariffRequestErrorSent,
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

        #region Send OnSetDefaultChargingTariffResponseErrorSent event

        /// <summary>
        /// An event sent whenever an SetDefaultChargingTariff response error was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseErrorSentDelegate? OnSetDefaultChargingTariffResponseErrorSent;


        public Task SendOnSetDefaultChargingTariffResponseErrorSent(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    IWebSocketConnection?              Connection,
                                                                    SetDefaultChargingTariffRequest?   Request,
                                                                    SetDefaultChargingTariffResponse?  Response,
                                                                    OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                    TimeSpan                           Runtime,
                                                                    SentMessageResults                 SentMessageResult,
                                                                    CancellationToken                  CancellationToken = default)

            => LogEvent(
                   OnSetDefaultChargingTariffResponseErrorSent,
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
