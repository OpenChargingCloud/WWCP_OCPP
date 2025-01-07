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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a NotifyNetworkTopology request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyMessageSentDelegate(DateTime                       Timestamp,
                                                                    IEventSender                   Sender,
                                                                    IWebSocketConnection?          Connection,
                                                                    NotifyNetworkTopologyMessage   Request,
                                                                    SentMessageResults             SentMessageResult,
                                                                    CancellationToken              CancellationToken);


    ///// <summary>
    ///// A NotifyNetworkTopology response.
    ///// </summary>
    ///// <param name="Timestamp">The log timestamp of the response.</param>
    ///// <param name="Sender">The sender of the response.</param>
    ///// <param name="Connection">The HTTP WebSocket client connection.</param>
    ///// <param name="Request">The reserve now request.</param>
    ///// <param name="Response">The reserve now response.</param>
    ///// <param name="Runtime">The runtime of this request.</param>
    ///// <param name="SentMessageResult">The result of the send message process.</param>
    ///// <param name="CancellationToken">An optional cancellation token.</param>
    //public delegate Task

    //    OnNotifyNetworkTopologyResponseSentDelegate(DateTime                        Timestamp,
    //                                                IEventSender                    Sender,
    //                                                IWebSocketConnection?           Connection,
    //                                                NotifyNetworkTopologyMessage    Request,
    //                                                NotifyNetworkTopologyResponse   Response,
    //                                                TimeSpan                        Runtime,
    //                                                SentMessageResults              SentMessageResult,
    //                                                CancellationToken               CancellationToken);


    ///// <summary>
    ///// A logging delegate called whenever a NotifyNetworkTopology request error was sent.
    ///// </summary>
    ///// <param name="Timestamp">The logging timestamp.</param>
    ///// <param name="Sender">The sender of the request error.</param>
    ///// <param name="Connection">The connection of the request error.</param>
    ///// <param name="Request">The optional request (when parsable).</param>
    ///// <param name="RequestErrorMessage">The request error message.</param>
    ///// <param name="Runtime">The optional runtime of the request error message.</param>
    ///// <param name="SentMessageResult">The result of the send message process.</param>
    ///// <param name="CancellationToken">An optional cancellation token.</param>
    //public delegate Task

    //    OnNotifyNetworkTopologyMessageErrorSentDelegate(DateTime                        Timestamp,
    //                                                    IEventSender                    Sender,
    //                                                    IWebSocketConnection?           Connection,
    //                                                    NotifyNetworkTopologyMessage?   Request,
    //                                                    OCPP_JSONRequestErrorMessage    RequestErrorMessage,
    //                                                    TimeSpan?                       Runtime,
    //                                                    SentMessageResults              SentMessageResult,
    //                                                    CancellationToken               CancellationToken);


    ///// <summary>
    ///// A logging delegate called whenever a NotifyNetworkTopology response error was sent.
    ///// </summary>
    ///// <param name="Timestamp">The logging timestamp.</param>
    ///// <param name="Sender">The sender of the response error.</param>
    ///// <param name="Connection">The connection of the response error.</param>
    ///// <param name="Request">The optional request.</param>
    ///// <param name="Response">The optional response.</param>
    ///// <param name="ResponseErrorMessage">The response error message.</param>
    ///// <param name="Runtime">The optional runtime of the response error message.</param>
    ///// <param name="SentMessageResult">The result of the send message process.</param>
    ///// <param name="CancellationToken">An optional cancellation token.</param>
    //public delegate Task

    //    OnNotifyNetworkTopologyResponseErrorSentDelegate(DateTime                         Timestamp,
    //                                                     IEventSender                     Sender,
    //                                                     IWebSocketConnection?            Connection,
    //                                                     NotifyNetworkTopologyMessage?    Request,
    //                                                     NotifyNetworkTopologyResponse?   Response,
    //                                                     OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
    //                                                     TimeSpan?                        Runtime,
    //                                                     SentMessageResults               SentMessageResult,
    //                                                     CancellationToken                CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NotifyNetworkTopology request

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnNotifyNetworkTopologyMessageSentDelegate?  OnNotifyNetworkTopologyMessageSent;


        /// <summary>
        /// Send a NotifyNetworkTopology request.
        /// </summary>
        /// <param name="Message">A NotifyNetworkTopology request.</param>
        public async Task<SentMessageResult>

            NotifyNetworkTopology(NotifyNetworkTopologyMessage Message)

        {

            SentMessageResult? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignSendMessage(
                        Message,
                        Message.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageSerializer,
                            parentNetworkingNode.OCPP.CustomNetworkTopologyInformationSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    //response = NotifyNetworkTopologyResponse.SignatureError(
                    //               Request,
                    //               signingErrors
                    //           );

                }

                #endregion

                else
                {

                    #region Send request message

                    response = await SendJSONSendMessage(

                                         OCPP_JSONSendMessage.FromDatagram(
                                             Message,
                                             Message.ToJSON(
                                                 parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyMessageSerializer,
                                                 parentNetworkingNode.OCPP.CustomNetworkTopologyInformationSerializer,
                                                 parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                 parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                             )
                                         ),

                                         sentMessageResult => LogEvent(
                                             OnNotifyNetworkTopologyMessageSent,
                                             loggingDelegate => loggingDelegate.Invoke(
                                                 Timestamp.Now,
                                                 parentNetworkingNode,
                                                 sentMessageResult.Connection,
                                                 Message,
                                                 sentMessageResult.Result,
                                                 Message.CancellationToken
                                             )
                                         )

                                     );

                    #endregion

                    //if (sendRequestState.IsValidJSONResponse(Message, out var jsonResponse))
                    //    response = await parentNetworkingNode.OCPP.IN.Receive_NotifyNetworkTopologyResponse(
                    //                         Message,
                    //                         jsonResponse,
                    //                         sendRequestState.WebSocketConnectionReceived,
                    //                         sendRequestState.DestinationReceived,
                    //                         sendRequestState.NetworkPathReceived,
                    //                         Message.         EventTrackingId,
                    //                         Message.         RequestId,
                    //                         sendRequestState.ResponseTimestamp,
                    //                         Message.         CancellationToken
                    //                     );

                    //if (sendRequestState.IsValidJSONRequestError(Message, out var jsonRequestError))
                    //    response = await parentNetworkingNode.OCPP.IN.Receive_NotifyNetworkTopologyMessageError(
                    //                         Message,
                    //                         jsonRequestError,
                    //                         sendRequestState.WebSocketConnectionReceived,
                    //                         sendRequestState.DestinationReceived,
                    //                         sendRequestState.NetworkPathReceived,
                    //                         Message.EventTrackingId,
                    //                         Message.RequestId,
                    //                         sendRequestState.ResponseTimestamp,
                    //                         Message.CancellationToken
                    //                     );

                    //response ??= new NotifyNetworkTopologyResponse(
                    //                 Message,
                    //                 NetworkTopologyStatus.Error,
                    //                 Result: Result.FromSendRequestState(sendRequestState)
                    //             );

                }

            }
            catch (Exception e)
            {

                //response = NotifyNetworkTopologyResponse.ExceptionOccured(
                //               Message,
                //               e
                //           );

                response = SentMessageResult.TransmissionFailed(e);

            }

            response ??= SentMessageResult.Unknown();

            return response;

        }

        #endregion


        #region Send OnNotifyNetworkTopologyResponseSent event

        ///// <summary>
        ///// An event sent whenever a NotifyNetworkTopology response was sent.
        ///// </summary>
        //public event OnNotifyNetworkTopologyResponseSentDelegate?  OnNotifyNetworkTopologyResponseSent;

        //public Task SendOnNotifyNetworkTopologyResponseSent(DateTime                       Timestamp,
        //                                                    IEventSender                   Sender,
        //                                                    IWebSocketConnection?          Connection,
        //                                                    NotifyNetworkTopologyMessage   Request,
        //                                                    NotifyNetworkTopologyResponse  Response,
        //                                                    TimeSpan                       Runtime,
        //                                                    SentMessageResults             SentMessageResult,
        //                                                    CancellationToken              CancellationToken = default)

        //    => LogEvent(
        //           OnNotifyNetworkTopologyResponseSent,
        //           loggingDelegate => loggingDelegate.Invoke(
        //               Timestamp,
        //               Sender,
        //               Connection,
        //               Request,
        //               Response,
        //               Runtime,
        //               SentMessageResult,
        //               CancellationToken
        //           )
        //       );

        #endregion

        #region Send OnNotifyNetworkTopologyMessageErrorSent event

        ///// <summary>
        ///// An event sent whenever a NotifyNetworkTopology request error was sent.
        ///// </summary>
        //public event OnNotifyNetworkTopologyMessageErrorSentDelegate? OnNotifyNetworkTopologyMessageErrorSent;


        //public Task SendOnNotifyNetworkTopologyMessageErrorSent(DateTime                       Timestamp,
        //                                                        IEventSender                   Sender,
        //                                                        IWebSocketConnection?          Connection,
        //                                                        NotifyNetworkTopologyMessage?  Request,
        //                                                        OCPP_JSONRequestErrorMessage   RequestErrorMessage,
        //                                                        TimeSpan                       Runtime,
        //                                                        SentMessageResults             SentMessageResult,
        //                                                        CancellationToken              CancellationToken = default)

        //    => LogEvent(
        //           OnNotifyNetworkTopologyMessageErrorSent,
        //           loggingDelegate => loggingDelegate.Invoke(
        //               Timestamp,
        //               Sender,
        //               Connection,
        //               Request,
        //               RequestErrorMessage,
        //               Runtime,
        //               SentMessageResult,
        //               CancellationToken
        //           )
        //       );

        #endregion

        #region Send OnNotifyNetworkTopologyResponseErrorSent event

        ///// <summary>
        ///// An event sent whenever a NotifyNetworkTopology response error was sent.
        ///// </summary>
        //public event OnNotifyNetworkTopologyResponseErrorSentDelegate? OnNotifyNetworkTopologyResponseErrorSent;


        //public Task SendOnNotifyNetworkTopologyResponseErrorSent(DateTime                        Timestamp,
        //                                                         IEventSender                    Sender,
        //                                                         IWebSocketConnection?           Connection,
        //                                                         NotifyNetworkTopologyMessage?   Request,
        //                                                         NotifyNetworkTopologyResponse?  Response,
        //                                                         OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
        //                                                         TimeSpan                        Runtime,
        //                                                         SentMessageResults              SentMessageResult,
        //                                                         CancellationToken               CancellationToken = default)

        //    => LogEvent(
        //           OnNotifyNetworkTopologyResponseErrorSent,
        //           loggingDelegate => loggingDelegate.Invoke(
        //               Timestamp,
        //               Sender,
        //               Connection,
        //               Request,
        //               Response,
        //               ResponseErrorMessage,
        //               Runtime,
        //               SentMessageResult,
        //               CancellationToken
        //           )
        //       );

        #endregion

    }

}
