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
    /// A delegate called whenever a SetDisplayMessage request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDisplayMessageRequestSentDelegate(DateTime                   Timestamp,
                                                                IEventSender               Sender,
                                                                IWebSocketConnection?      Connection,
                                                                SetDisplayMessageRequest   Request,
                                                                SentMessageResults         SentMessageResult,
                                                                CancellationToken          CancellationToken);


    /// <summary>
    /// A SetDisplayMessage response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetDisplayMessageResponseSentDelegate(DateTime                    Timestamp,
                                                IEventSender                Sender,
                                                IWebSocketConnection?       Connection,
                                                SetDisplayMessageRequest    Request,
                                                SetDisplayMessageResponse   Response,
                                                TimeSpan                    Runtime,
                                                SentMessageResults          SentMessageResult,
                                                CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetDisplayMessage request error was sent.
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

        OnSetDisplayMessageRequestErrorSentDelegate(DateTime                       Timestamp,
                                                    IEventSender                   Sender,
                                                    IWebSocketConnection?          Connection,
                                                    SetDisplayMessageRequest?      Request,
                                                    OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                    TimeSpan?                      Runtime,
                                                    SentMessageResults             SentMessageResult,
                                                    CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetDisplayMessage response error was sent.
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

        OnSetDisplayMessageResponseErrorSentDelegate(DateTime                        Timestamp,
                                                     IEventSender                    Sender,
                                                     IWebSocketConnection?           Connection,
                                                     SetDisplayMessageRequest?       Request,
                                                     SetDisplayMessageResponse?      Response,
                                                     OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                     TimeSpan?                       Runtime,
                                                     SentMessageResults              SentMessageResult,
                                                     CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SetDisplayMessage request

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request was sent.
        /// </summary>
        public event OnSetDisplayMessageRequestSentDelegate?  OnSetDisplayMessageRequestSent;


        /// <summary>
        /// Send a SetDisplayMessage request.
        /// </summary>
        /// <param name="Request">A SetDisplayMessage request.</param>
        public async Task<SetDisplayMessageResponse>

            SetDisplayMessage(SetDisplayMessageRequest Request)

        {

            SetDisplayMessageResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                            parentNetworkingNode.OCPP.CustomMessageInfoSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = SetDisplayMessageResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomMessageInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                             parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnSetDisplayMessageRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDisplayMessageResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetDisplayMessageRequestError(
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

                    response ??= new SetDisplayMessageResponse(
                                     Request,
                                     DisplayMessageStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SetDisplayMessageResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSetDisplayMessageResponseSent event

        /// <summary>
        /// An event sent whenever a SetDisplayMessage response was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseSentDelegate?  OnSetDisplayMessageResponseSent;

        public Task SendOnSetDisplayMessageResponseSent(DateTime                   Timestamp,
                                                        IEventSender               Sender,
                                                        IWebSocketConnection?      Connection,
                                                        SetDisplayMessageRequest   Request,
                                                        SetDisplayMessageResponse  Response,
                                                        TimeSpan                   Runtime,
                                                        SentMessageResults         SentMessageResult,
                                                        CancellationToken          CancellationToken = default)

            => LogEvent(
                   OnSetDisplayMessageResponseSent,
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

        #region Send OnSetDisplayMessageRequestErrorSent event

        /// <summary>
        /// An event sent whenever a SetDisplayMessage request error was sent.
        /// </summary>
        public event OnSetDisplayMessageRequestErrorSentDelegate? OnSetDisplayMessageRequestErrorSent;


        public Task SendOnSetDisplayMessageRequestErrorSent(DateTime                      Timestamp,
                                                            IEventSender                  Sender,
                                                            IWebSocketConnection?         Connection,
                                                            SetDisplayMessageRequest?     Request,
                                                            OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                            TimeSpan                      Runtime,
                                                            SentMessageResults            SentMessageResult,
                                                            CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSetDisplayMessageRequestErrorSent,
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

        #region Send OnSetDisplayMessageResponseErrorSent event

        /// <summary>
        /// An event sent whenever a SetDisplayMessage response error was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseErrorSentDelegate? OnSetDisplayMessageResponseErrorSent;


        public Task SendOnSetDisplayMessageResponseErrorSent(DateTime                       Timestamp,
                                                             IEventSender                   Sender,
                                                             IWebSocketConnection?          Connection,
                                                             SetDisplayMessageRequest?      Request,
                                                             SetDisplayMessageResponse?     Response,
                                                             OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                             TimeSpan                       Runtime,
                                                             SentMessageResults             SentMessageResult,
                                                             CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSetDisplayMessageResponseErrorSent,
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
