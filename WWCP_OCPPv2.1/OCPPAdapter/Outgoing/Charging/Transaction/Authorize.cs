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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an Authorize request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAuthorizeRequestSentDelegate(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection?   Connection,
                                                        AuthorizeRequest        Request,
                                                        SentMessageResults      SentMessageResult,
                                                        CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an Authorize response was sent.
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

        OnAuthorizeResponseSentDelegate(DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection?  Connection,
                                        AuthorizeRequest?      Request,
                                        AuthorizeResponse      Response,
                                        TimeSpan               Runtime,
                                        SentMessageResults     SentMessageResult,
                                        CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an Authorize request error was sent.
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

        OnAuthorizeRequestErrorSentDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            IWebSocketConnection?          Connection,
                                            AuthorizeRequest?              Request,
                                            OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                            TimeSpan?                      Runtime,
                                            SentMessageResults             SentMessageResult,
                                            CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an Authorize response error was sent.
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

        OnAuthorizeResponseErrorSentDelegate(DateTime                        Timestamp,
                                             IEventSender                    Sender,
                                             IWebSocketConnection?           Connection,
                                             AuthorizeRequest?               Request,
                                             AuthorizeResponse?              Response,
                                             OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                             TimeSpan?                       Runtime,
                                             SentMessageResults              SentMessageResult,
                                             CancellationToken               CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send Authorize request

        /// <summary>
        /// An event fired whenever an Authorize request was sent.
        /// </summary>
        public event OnAuthorizeRequestSentDelegate?  OnAuthorizeRequestSent;


        /// <summary>
        /// Authorize the given identification token.
        /// </summary>
        /// <param name="Request">An Authorize request.</param>
        public async Task<AuthorizeResponse> Authorize(AuthorizeRequest Request)
        {

            AuthorizeResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = AuthorizeResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnAuthorizeRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_AuthorizeResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_AuthorizeRequestError(
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

                    response ??= new AuthorizeResponse(
                                     Request,
                                     IdTokenInfo.Error(),
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = AuthorizeResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnAuthorizeResponseSent event

        /// <summary>
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseSentDelegate?  OnAuthorizeResponseSent;


        public Task SendOnAuthorizeResponseSent(DateTime               Timestamp,
                                                IEventSender           Sender,
                                                IWebSocketConnection?  Connection,
                                                AuthorizeRequest       Request,
                                                AuthorizeResponse      Response,
                                                TimeSpan               Runtime,
                                                SentMessageResults     SentMessageResult,
                                                CancellationToken      CancellationToken = default)

            => LogEvent(
                   OnAuthorizeResponseSent,
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

        #region Send OnAuthorizeRequestErrorSent event

        /// <summary>
        /// An event sent whenever an Authorize request error was sent.
        /// </summary>
        public event OnAuthorizeRequestErrorSentDelegate? OnAuthorizeRequestErrorSent;


        public Task SendOnAuthorizeRequestErrorSent(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection?         Connection,
                                                    AuthorizeRequest?             Request,
                                                    OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                    TimeSpan                      Runtime,
                                                    SentMessageResults            SentMessageResult,
                                                    CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnAuthorizeRequestErrorSent,
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

        #region Send OnAuthorizeResponseErrorSent event

        /// <summary>
        /// An event sent whenever an Authorize response error was sent.
        /// </summary>
        public event OnAuthorizeResponseErrorSentDelegate? OnAuthorizeResponseErrorSent;


        public Task SendOnAuthorizeResponseErrorSent(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection?          Connection,
                                                     AuthorizeRequest?              Request,
                                                     AuthorizeResponse?             Response,
                                                     OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                     TimeSpan                       Runtime,
                                                     SentMessageResults             SentMessageResult,
                                                     CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnAuthorizeResponseErrorSent,
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
