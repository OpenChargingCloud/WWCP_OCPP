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
    /// A delegate called whenever an UnlockConnector request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUnlockConnectorRequestSentDelegate(DateTime                 Timestamp,
                                                              IEventSender             Sender,
                                                              IWebSocketConnection     Connection,
                                                              UnlockConnectorRequest   Request,
                                                              SentMessageResults       SendMessageResult,
                                                              CancellationToken        CancellationToken = default);


    /// <summary>
    /// A UnlockConnector response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnUnlockConnectorResponseSentDelegate(DateTime                  Timestamp,
                                              IEventSender              Sender,
                                              IWebSocketConnection      Connection,
                                              UnlockConnectorRequest    Request,
                                              UnlockConnectorResponse   Response,
                                              TimeSpan                  Runtime,
                                              SentMessageResults        SendMessageResult,
                                              CancellationToken         CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an UnlockConnector request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnUnlockConnectorRequestErrorSentDelegate(DateTime                       Timestamp,
                                                  IEventSender                   Sender,
                                                  IWebSocketConnection           Connection,
                                                  UnlockConnectorRequest?        Request,
                                                  OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                  TimeSpan?                      Runtime,
                                                  SentMessageResults             SendMessageResult,
                                                  CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an UnlockConnector response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnUnlockConnectorResponseErrorSentDelegate(DateTime                        Timestamp,
                                                   IEventSender                    Sender,
                                                   IWebSocketConnection            Connection,
                                                   UnlockConnectorRequest?         Request,
                                                   UnlockConnectorResponse?        Response,
                                                   OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                   TimeSpan?                       Runtime,
                                                   SentMessageResults              SendMessageResult,
                                                   CancellationToken               CancellationToken = default);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send UnlockConnector request

        /// <summary>
        /// An event fired whenever an UnlockConnector request was sent.
        /// </summary>
        public event OnUnlockConnectorRequestSentDelegate?  OnUnlockConnectorRequestSent;


        /// <summary>
        /// Send an UnlockConnector request.
        /// </summary>
        /// <param name="Request">A UnlockConnector request.</param>
        public async Task<UnlockConnectorResponse>

            UnlockConnector(UnlockConnectorRequest Request)

        {

            UnlockConnectorResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = UnlockConnectorResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnUnlockConnectorRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sendMessageResult.Connection,
                                                             Request,
                                                             sendMessageResult.Result
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_UnlockConnectorResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_UnlockConnectorRequestError(
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

                    response ??= new UnlockConnectorResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = UnlockConnectorResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnUnlockConnectorResponseSent event

        /// <summary>
        /// An event sent whenever an UnlockConnector response was sent.
        /// </summary>
        public event OnUnlockConnectorResponseSentDelegate?  OnUnlockConnectorResponseSent;

        public Task SendOnUnlockConnectorResponseSent(DateTime                 Timestamp,
                                                      IEventSender             Sender,
                                                      IWebSocketConnection     Connection,
                                                      UnlockConnectorRequest   Request,
                                                      UnlockConnectorResponse  Response,
                                                      TimeSpan                 Runtime,
                                                      SentMessageResults       SendMessageResult,
                                                      CancellationToken        CancellationToken = default)

            => LogEvent(
                   OnUnlockConnectorResponseSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnUnlockConnectorRequestErrorSent event

        /// <summary>
        /// An event sent whenever an UnlockConnector request error was sent.
        /// </summary>
        public event OnUnlockConnectorRequestErrorSentDelegate? OnUnlockConnectorRequestErrorSent;


        public Task SendOnUnlockConnectorRequestErrorSent(DateTime                      Timestamp,
                                                          IEventSender                  Sender,
                                                          IWebSocketConnection          Connection,
                                                          UnlockConnectorRequest?       Request,
                                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                          TimeSpan                      Runtime,
                                                          SentMessageResults            SendMessageResult,
                                                          CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnUnlockConnectorRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnUnlockConnectorResponseErrorSent event

        /// <summary>
        /// An event sent whenever an UnlockConnector response error was sent.
        /// </summary>
        public event OnUnlockConnectorResponseErrorSentDelegate? OnUnlockConnectorResponseErrorSent;


        public Task SendOnUnlockConnectorResponseErrorSent(DateTime                       Timestamp,
                                                           IEventSender                   Sender,
                                                           IWebSocketConnection           Connection,
                                                           UnlockConnectorRequest?        Request,
                                                           UnlockConnectorResponse?       Response,
                                                           OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                           TimeSpan                       Runtime,
                                                           SentMessageResults             SendMessageResult,
                                                           CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnUnlockConnectorResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

    }

}
