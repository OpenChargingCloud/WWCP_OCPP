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
    /// A delegate called whenever a NotifyEvent request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyEventRequestSentDelegate(DateTimeOffset          Timestamp,
                                                          IEventSender            Sender,
                                                          IWebSocketConnection?   Connection,
                                                          NotifyEventRequest      Request,
                                                          SentMessageResults      SentMessageResult,
                                                          CancellationToken       CancellationToken);


    /// <summary>
    /// A NotifyEvent response.
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

        OnNotifyEventResponseSentDelegate(DateTimeOffset         Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection?  Connection,
                                          NotifyEventRequest     Request,
                                          NotifyEventResponse    Response,
                                          TimeSpan               Runtime,
                                          SentMessageResults     SentMessageResult,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyEvent request error was sent.
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

        OnNotifyEventRequestErrorSentDelegate(DateTimeOffset                 Timestamp,
                                              IEventSender                   Sender,
                                              IWebSocketConnection?          Connection,
                                              NotifyEventRequest?            Request,
                                              OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                              TimeSpan?                      Runtime,
                                              SentMessageResults             SentMessageResult,
                                              CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyEvent response error was sent.
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

        OnNotifyEventResponseErrorSentDelegate(DateTimeOffset                  Timestamp,
                                               IEventSender                    Sender,
                                               IWebSocketConnection?           Connection,
                                               NotifyEventRequest?             Request,
                                               NotifyEventResponse?            Response,
                                               OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                               TimeSpan?                       Runtime,
                                               SentMessageResults              SentMessageResult,
                                               CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NotifyEvent request

        /// <summary>
        /// An event fired whenever a NotifyEvent request was sent.
        /// </summary>
        public event OnNotifyEventRequestSentDelegate?  OnNotifyEventRequestSent;


        /// <summary>
        /// Send a NotifyEvent request.
        /// </summary>
        /// <param name="Request">A NotifyEvent request.</param>
        public async Task<NotifyEventResponse>

            NotifyEvent(NotifyEventRequest Request)

        {

            NotifyEventResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                            parentNetworkingNode.OCPP.CustomEventDataSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = NotifyEventResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomEventDataSerializer,
                                                             parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                             parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnNotifyEventRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyEventResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyEventRequestError(
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

                    response ??= new NotifyEventResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = NotifyEventResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnNotifyEventResponseSent event

        /// <summary>
        /// An event sent whenever a NotifyEvent response was sent.
        /// </summary>
        public event OnNotifyEventResponseSentDelegate?  OnNotifyEventResponseSent;

        public Task SendOnNotifyEventResponseSent(DateTimeOffset        Timestamp,
                                                  IEventSender          Sender,
                                                  IWebSocketConnection? Connection,
                                                  NotifyEventRequest    Request,
                                                  NotifyEventResponse   Response,
                                                  TimeSpan              Runtime,
                                                  SentMessageResults    SentMessageResult,
                                                  CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnNotifyEventResponseSent,
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

        #region Send OnNotifyEventRequestErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyEvent request error was sent.
        /// </summary>
        public event OnNotifyEventRequestErrorSentDelegate? OnNotifyEventRequestErrorSent;


        public Task SendOnNotifyEventRequestErrorSent(DateTimeOffset                Timestamp,
                                                      IEventSender                  Sender,
                                                      IWebSocketConnection?         Connection,
                                                      NotifyEventRequest?           Request,
                                                      OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                      TimeSpan                      Runtime,
                                                      SentMessageResults            SentMessageResult,
                                                      CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnNotifyEventRequestErrorSent,
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

        #region Send OnNotifyEventResponseErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyEvent response error was sent.
        /// </summary>
        public event OnNotifyEventResponseErrorSentDelegate? OnNotifyEventResponseErrorSent;


        public Task SendOnNotifyEventResponseErrorSent(DateTimeOffset                 Timestamp,
                                                       IEventSender                   Sender,
                                                       IWebSocketConnection?          Connection,
                                                       NotifyEventRequest?            Request,
                                                       NotifyEventResponse?           Response,
                                                       OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                       TimeSpan                       Runtime,
                                                       SentMessageResults             SentMessageResult,
                                                       CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnNotifyEventResponseErrorSent,
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
