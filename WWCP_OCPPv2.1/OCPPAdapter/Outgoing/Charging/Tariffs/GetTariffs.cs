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
    /// A logging delegate called whenever an GetTariffs request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetTariffsRequestSentDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         IWebSocketConnection?   Connection,
                                                         GetTariffsRequest       Request,
                                                         SentMessageResults      SentMessageResult,
                                                         CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs response was sent.
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

        OnGetTariffsResponseSentDelegate(DateTime                Timestamp,
                                         IEventSender            Sender,
                                         IWebSocketConnection?   Connection,
                                         GetTariffsRequest?      Request,
                                         GetTariffsResponse      Response,
                                         TimeSpan                Runtime,
                                         SentMessageResults      SentMessageResult,
                                         CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs request error was sent.
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

        OnGetTariffsRequestErrorSentDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             IWebSocketConnection?          Connection,
                                             GetTariffsRequest?             Request,
                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                             TimeSpan?                      Runtime,
                                             SentMessageResults             SentMessageResult,
                                             CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs response error was sent.
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

        OnGetTariffsResponseErrorSentDelegate(DateTime                        Timestamp,
                                              IEventSender                    Sender,
                                              IWebSocketConnection?           Connection,
                                              GetTariffsRequest?              Request,
                                              GetTariffsResponse?             Response,
                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                              TimeSpan?                       Runtime,
                                              SentMessageResults              SentMessageResult,
                                              CancellationToken               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send GetTariffs request

        /// <summary>
        /// An event fired whenever an GetTariffs request was sent.
        /// </summary>
        public event OnGetTariffsRequestSentDelegate?  OnGetTariffsRequestSent;


        /// <summary>
        /// GetTariffs the given identification token.
        /// </summary>
        /// <param name="Request">An GetTariffs request.</param>
        public async Task<GetTariffsResponse> GetTariffs(GetTariffsRequest Request)
        {

            GetTariffsResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetTariffsRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = GetTariffsResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomGetTariffsRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnGetTariffsRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetTariffsResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_GetTariffsRequestError(
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

                    response ??= new GetTariffsResponse(
                                     Request,
                                     TariffStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = GetTariffsResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnGetTariffsResponseSent event

        /// <summary>
        /// An event sent whenever an GetTariffs response was sent.
        /// </summary>
        public event OnGetTariffsResponseSentDelegate?  OnGetTariffsResponseSent;


        public Task SendOnGetTariffsResponseSent(DateTime               Timestamp,
                                                 IEventSender           Sender,
                                                 IWebSocketConnection?  Connection,
                                                 GetTariffsRequest      Request,
                                                 GetTariffsResponse     Response,
                                                 TimeSpan               Runtime,
                                                 SentMessageResults     SentMessageResult,
                                                 CancellationToken      CancellationToken = default)

            => LogEvent(
                   OnGetTariffsResponseSent,
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

        #region Send OnGetTariffsRequestErrorSent event

        /// <summary>
        /// An event sent whenever an GetTariffs request error was sent.
        /// </summary>
        public event OnGetTariffsRequestErrorSentDelegate? OnGetTariffsRequestErrorSent;


        public Task SendOnGetTariffsRequestErrorSent(DateTime                      Timestamp,
                                                     IEventSender                  Sender,
                                                     IWebSocketConnection?         Connection,
                                                     GetTariffsRequest?            Request,
                                                     OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                     TimeSpan                      Runtime,
                                                     SentMessageResults            SentMessageResult,
                                                     CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnGetTariffsRequestErrorSent,
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

        #region Send OnGetTariffsResponseErrorSent event

        /// <summary>
        /// An event sent whenever an GetTariffs response error was sent.
        /// </summary>
        public event OnGetTariffsResponseErrorSentDelegate? OnGetTariffsResponseErrorSent;


        public Task SendOnGetTariffsResponseErrorSent(DateTime                       Timestamp,
                                                      IEventSender                   Sender,
                                                      IWebSocketConnection?          Connection,
                                                      GetTariffsRequest?             Request,
                                                      GetTariffsResponse?            Response,
                                                      OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                      TimeSpan                       Runtime,
                                                      SentMessageResults             SentMessageResult,
                                                      CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnGetTariffsResponseErrorSent,
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
