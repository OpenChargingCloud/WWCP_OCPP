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
    /// A delegate called whenever an UpdateFirmware request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUpdateFirmwareRequestSentDelegate(DateTime                Timestamp,
                                                             IEventSender            Sender,
                                                             IWebSocketConnection?   Connection,
                                                             UpdateFirmwareRequest   Request,
                                                             SentMessageResults      SentMessageResult,
                                                             CancellationToken       CancellationToken);


    /// <summary>
    /// A UpdateFirmware response.
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

        OnUpdateFirmwareResponseSentDelegate(DateTime                 Timestamp,
                                             IEventSender             Sender,
                                             IWebSocketConnection?    Connection,
                                             UpdateFirmwareRequest    Request,
                                             UpdateFirmwareResponse   Response,
                                             TimeSpan                 Runtime,
                                             SentMessageResults       SentMessageResult,
                                             CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an UpdateFirmware request error was sent.
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

        OnUpdateFirmwareRequestErrorSentDelegate(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection?          Connection,
                                                 UpdateFirmwareRequest?         Request,
                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                 TimeSpan?                      Runtime,
                                                 SentMessageResults             SentMessageResult,
                                                 CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an UpdateFirmware response error was sent.
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

        OnUpdateFirmwareResponseErrorSentDelegate(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection?           Connection,
                                                  UpdateFirmwareRequest?          Request,
                                                  UpdateFirmwareResponse?         Response,
                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                  TimeSpan?                       Runtime,
                                                  SentMessageResults              SentMessageResult,
                                                  CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send UpdateFirmware request

        /// <summary>
        /// An event fired whenever an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestSentDelegate?  OnUpdateFirmwareRequestSent;


        /// <summary>
        /// Send an UpdateFirmware request.
        /// </summary>
        /// <param name="Request">A UpdateFirmware request.</param>
        public async Task<UpdateFirmwareResponse>

            UpdateFirmware(UpdateFirmwareRequest Request)

        {

            UpdateFirmwareResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                            parentNetworkingNode.OCPP.CustomFirmwareSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = UpdateFirmwareResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomFirmwareSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnUpdateFirmwareRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_UpdateFirmwareResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_UpdateFirmwareRequestError(
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

                    response ??= new UpdateFirmwareResponse(
                                     Request,
                                     UpdateFirmwareStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = UpdateFirmwareResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnUpdateFirmwareResponseSent event

        /// <summary>
        /// An event sent whenever an UpdateFirmware response was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseSentDelegate?  OnUpdateFirmwareResponseSent;

        public Task SendOnUpdateFirmwareResponseSent(DateTime                Timestamp,
                                                     IEventSender            Sender,
                                                     IWebSocketConnection?   Connection,
                                                     UpdateFirmwareRequest   Request,
                                                     UpdateFirmwareResponse  Response,
                                                     TimeSpan                Runtime,
                                                     SentMessageResults      SentMessageResult,
                                                     CancellationToken       CancellationToken = default)

            => LogEvent(
                   OnUpdateFirmwareResponseSent,
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

        #region Send OnUpdateFirmwareRequestErrorSent event

        /// <summary>
        /// An event sent whenever an UpdateFirmware request error was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestErrorSentDelegate? OnUpdateFirmwareRequestErrorSent;


        public Task SendOnUpdateFirmwareRequestErrorSent(DateTime                      Timestamp,
                                                         IEventSender                  Sender,
                                                         IWebSocketConnection?         Connection,
                                                         UpdateFirmwareRequest?        Request,
                                                         OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                         TimeSpan                      Runtime,
                                                         SentMessageResults            SentMessageResult,
                                                         CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnUpdateFirmwareRequestErrorSent,
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

        #region Send OnUpdateFirmwareResponseErrorSent event

        /// <summary>
        /// An event sent whenever an UpdateFirmware response error was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseErrorSentDelegate? OnUpdateFirmwareResponseErrorSent;


        public Task SendOnUpdateFirmwareResponseErrorSent(DateTime                       Timestamp,
                                                          IEventSender                   Sender,
                                                          IWebSocketConnection?          Connection,
                                                          UpdateFirmwareRequest?         Request,
                                                          UpdateFirmwareResponse?        Response,
                                                          OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                          TimeSpan                       Runtime,
                                                          SentMessageResults             SentMessageResult,
                                                          CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnUpdateFirmwareResponseErrorSent,
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
