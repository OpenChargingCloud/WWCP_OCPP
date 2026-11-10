/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a NotifyNetworkTimeResult request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTimeResultRequestSentDelegate(DateTimeOffset          Timestamp,
                                                        IEventSender            Sender,
                                                        IMessageConnection?     Connection,
                                                        NotifyNetworkTimeResultRequest        Request,
                                                        SentMessageResults      SentMessageResult,
                                                        CancellationToken       CancellationToken);


    /// <summary>
    /// A NotifyNetworkTimeResult response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The NotifyNetworkTimeResult request.</param>
    /// <param name="Response">The NotifyNetworkTimeResult response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnNotifyNetworkTimeResultResponseSentDelegate(DateTimeOffset         Timestamp,
                                        IEventSender           Sender,
                                        IMessageConnection?    Connection,
                                        NotifyNetworkTimeResultRequest       Request,
                                        NotifyNetworkTimeResultResponse      Response,
                                        TimeSpan               Runtime,
                                        SentMessageResults     SentMessageResult,
                                        CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTimeResult request error was sent.
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

        OnNotifyNetworkTimeResultRequestErrorSentDelegate(DateTimeOffset                 Timestamp,
                                            IEventSender                   Sender,
                                            IMessageConnection?            Connection,
                                            NotifyNetworkTimeResultRequest?              Request,
                                            OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                            TimeSpan?                      Runtime,
                                            SentMessageResults             SentMessageResult,
                                            CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTimeResult response error was sent.
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

        OnNotifyNetworkTimeResultResponseErrorSentDelegate(DateTimeOffset                  Timestamp,
                                             IEventSender                    Sender,
                                             IMessageConnection?             Connection,
                                             NotifyNetworkTimeResultRequest?               Request,
                                             NotifyNetworkTimeResultResponse?              Response,
                                             OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                             TimeSpan?                       Runtime,
                                             SentMessageResults              SentMessageResult,
                                             CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NotifyNetworkTimeResult request

        /// <summary>
        /// An event fired whenever a NotifyNetworkTimeResult request was sent.
        /// </summary>
        public event OnNotifyNetworkTimeResultRequestSentDelegate?  OnNotifyNetworkTimeResultRequestSent;


        /// <summary>
        /// Send a NotifyNetworkTimeResult request.
        /// </summary>
        /// <param name="Request">A NotifyNetworkTimeResult request.</param>
        public async Task<NotifyNetworkTimeResultResponse>

            NotifyNetworkTimeResult(NotifyNetworkTimeResultRequest Request)

        {

            NotifyNetworkTimeResultResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = NotifyNetworkTimeResultResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnNotifyNetworkTimeResultRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Now,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyNetworkTimeResultResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyNetworkTimeResultRequestError(
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

                    response ??= new NotifyNetworkTimeResultResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = NotifyNetworkTimeResultResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnNotifyNetworkTimeResultResponseSent event

        /// <summary>
        /// An event sent whenever a NotifyNetworkTimeResult response was sent.
        /// </summary>
        public event OnNotifyNetworkTimeResultResponseSentDelegate?  OnNotifyNetworkTimeResultResponseSent;

        public Task SendOnNotifyNetworkTimeResultResponseSent(DateTimeOffset        Timestamp,
                                                IEventSender          Sender,
                                                IMessageConnection?   Connection,
                                                NotifyNetworkTimeResultRequest      Request,
                                                NotifyNetworkTimeResultResponse     Response,
                                                TimeSpan              Runtime,
                                                SentMessageResults    SentMessageResult,
                                                CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnNotifyNetworkTimeResultResponseSent,
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

        #region Send OnNotifyNetworkTimeResultRequestErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyNetworkTimeResult request error was sent.
        /// </summary>
        public event OnNotifyNetworkTimeResultRequestErrorSentDelegate? OnNotifyNetworkTimeResultRequestErrorSent;


        public Task SendOnNotifyNetworkTimeResultRequestErrorSent(DateTimeOffset                Timestamp,
                                                    IEventSender                  Sender,
                                                    IMessageConnection?           Connection,
                                                    NotifyNetworkTimeResultRequest?             Request,
                                                    OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                    TimeSpan                      Runtime,
                                                    SentMessageResults            SentMessageResult,
                                                    CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnNotifyNetworkTimeResultRequestErrorSent,
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

        #region Send OnNotifyNetworkTimeResultResponseErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyNetworkTimeResult response error was sent.
        /// </summary>
        public event OnNotifyNetworkTimeResultResponseErrorSentDelegate? OnNotifyNetworkTimeResultResponseErrorSent;


        public Task SendOnNotifyNetworkTimeResultResponseErrorSent(DateTimeOffset                 Timestamp,
                                                     IEventSender                   Sender,
                                                     IMessageConnection?            Connection,
                                                     NotifyNetworkTimeResultRequest?              Request,
                                                     NotifyNetworkTimeResultResponse?             Response,
                                                     OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                     TimeSpan                       Runtime,
                                                     SentMessageResults             SentMessageResult,
                                                     CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnNotifyNetworkTimeResultResponseErrorSent,
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
