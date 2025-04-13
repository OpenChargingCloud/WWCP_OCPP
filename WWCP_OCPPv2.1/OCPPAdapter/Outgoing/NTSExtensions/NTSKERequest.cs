﻿/*
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
    /// A delegate called whenever an NTSKE request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNTSKERequestSentDelegate(DateTime                Timestamp,
                                                    IEventSender            Sender,
                                                    IWebSocketConnection?   Connection,
                                                    NTSKERequest            Request,
                                                    SentMessageResults      SentMessageResult,
                                                    CancellationToken       CancellationToken);


    /// <summary>
    /// A NTSKE response.
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

        OnNTSKEResponseSentDelegate(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    IWebSocketConnection?  Connection,
                                    NTSKERequest           Request,
                                    NTSKEResponse          Response,
                                    TimeSpan               Runtime,
                                    SentMessageResults     SentMessageResult,
                                    CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an NTSKE request error was sent.
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

        OnNTSKERequestErrorSentDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        IWebSocketConnection?          Connection,
                                        NTSKERequest?                  Request,
                                        OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                        TimeSpan?                      Runtime,
                                        SentMessageResults             SentMessageResult,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an NTSKE response error was sent.
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

        OnNTSKEResponseErrorSentDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         IWebSocketConnection?           Connection,
                                         NTSKERequest?                   Request,
                                         NTSKEResponse?                  Response,
                                         OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                         TimeSpan?                       Runtime,
                                         SentMessageResults              SentMessageResult,
                                         CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NTSKE request

        /// <summary>
        /// An event fired whenever an NTSKE request was sent.
        /// </summary>
        public event OnNTSKERequestSentDelegate?  OnNTSKERequestSent;


        /// <summary>
        /// Send an NTSKE request.
        /// </summary>
        /// <param name="Request">A NTSKE request.</param>
        public async Task<NTSKEResponse>

            NTSKE(NTSKERequest Request)

        {

            NTSKEResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNTSKERequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = NTSKEResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomNTSKERequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnNTSKERequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NTSKEResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NTSKERequestError(
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

                    response ??= new NTSKEResponse(
                                     Request,
                                     [],
                                     GenericStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = NTSKEResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnNTSKEResponseSent event

        /// <summary>
        /// An event sent whenever an NTSKE response was sent.
        /// </summary>
        public event OnNTSKEResponseSentDelegate?  OnNTSKEResponseSent;

        public Task SendOnNTSKEResponseSent(DateTime              Timestamp,
                                            IEventSender          Sender,
                                            IWebSocketConnection? Connection,
                                            NTSKERequest          Request,
                                            NTSKEResponse         Response,
                                            TimeSpan              Runtime,
                                            SentMessageResults    SentMessageResult,
                                            CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnNTSKEResponseSent,
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

        #region Send OnNTSKERequestErrorSent event

        /// <summary>
        /// An event sent whenever an NTSKE request error was sent.
        /// </summary>
        public event OnNTSKERequestErrorSentDelegate? OnNTSKERequestErrorSent;


        public Task SendOnNTSKERequestErrorSent(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection?         Connection,
                                                NTSKERequest?                 Request,
                                                OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                TimeSpan                      Runtime,
                                                SentMessageResults            SentMessageResult,
                                                CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnNTSKERequestErrorSent,
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

        #region Send OnNTSKEResponseErrorSent event

        /// <summary>
        /// An event sent whenever an NTSKE response error was sent.
        /// </summary>
        public event OnNTSKEResponseErrorSentDelegate? OnNTSKEResponseErrorSent;


        public Task SendOnNTSKEResponseErrorSent(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection?          Connection,
                                                 NTSKERequest?                  Request,
                                                 NTSKEResponse?                 Response,
                                                 OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                 TimeSpan                       Runtime,
                                                 SentMessageResults             SentMessageResult,
                                                 CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnNTSKEResponseErrorSent,
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