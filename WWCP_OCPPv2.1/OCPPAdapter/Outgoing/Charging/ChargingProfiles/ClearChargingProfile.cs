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
    /// A delegate called whenever a ClearChargingProfile request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnClearChargingProfileRequestSentDelegate(DateTime                      Timestamp,
                                                                   IEventSender                  Sender,
                                                                   IWebSocketConnection?         Connection,
                                                                   ClearChargingProfileRequest   Request,
                                                                   SentMessageResults            SentMessageResult,
                                                                   CancellationToken             CancellationToken);


    /// <summary>
    /// A ClearChargingProfile response.
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

        OnClearChargingProfileResponseSentDelegate(DateTime                       Timestamp,
                                                   IEventSender                   Sender,
                                                   IWebSocketConnection?          Connection,
                                                   ClearChargingProfileRequest    Request,
                                                   ClearChargingProfileResponse   Response,
                                                   TimeSpan                       Runtime,
                                                   SentMessageResults             SentMessageResult,
                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ClearChargingProfile request error was sent.
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

        OnClearChargingProfileRequestErrorSentDelegate(DateTime                       Timestamp,
                                                       IEventSender                   Sender,
                                                       IWebSocketConnection?          Connection,
                                                       ClearChargingProfileRequest?   Request,
                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                       TimeSpan?                      Runtime,
                                                       SentMessageResults             SentMessageResult,
                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ClearChargingProfile response error was sent.
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

        OnClearChargingProfileResponseErrorSentDelegate(DateTime                        Timestamp,
                                                        IEventSender                    Sender,
                                                        IWebSocketConnection?           Connection,
                                                        ClearChargingProfileRequest?    Request,
                                                        ClearChargingProfileResponse?   Response,
                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                        TimeSpan?                       Runtime,
                                                        SentMessageResults              SentMessageResult,
                                                        CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send ClearChargingProfile request

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request was sent.
        /// </summary>
        public event OnClearChargingProfileRequestSentDelegate?  OnClearChargingProfileRequestSent;


        /// <summary>
        /// Send a ClearChargingProfile request.
        /// </summary>
        /// <param name="Request">A ClearChargingProfile request.</param>
        public async Task<ClearChargingProfileResponse>

            ClearChargingProfile(ClearChargingProfileRequest Request)

        {

            ClearChargingProfileResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                            parentNetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = ClearChargingProfileResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnClearChargingProfileRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ClearChargingProfileResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_ClearChargingProfileRequestError(
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

                    response ??= new ClearChargingProfileResponse(
                                     Request,
                                     ClearChargingProfileStatus.Unknown,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = ClearChargingProfileResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnClearChargingProfileResponseSent event

        /// <summary>
        /// An event sent whenever a ClearChargingProfile response was sent.
        /// </summary>
        public event OnClearChargingProfileResponseSentDelegate?  OnClearChargingProfileResponseSent;

        public Task SendOnClearChargingProfileResponseSent(DateTime                      Timestamp,
                                                           IEventSender                  Sender,
                                                           IWebSocketConnection?         Connection,
                                                           ClearChargingProfileRequest   Request,
                                                           ClearChargingProfileResponse  Response,
                                                           TimeSpan                      Runtime,
                                                           SentMessageResults            SentMessageResult,
                                                           CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnClearChargingProfileResponseSent,
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

        #region Send OnClearChargingProfileRequestErrorSent event

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request error was sent.
        /// </summary>
        public event OnClearChargingProfileRequestErrorSentDelegate? OnClearChargingProfileRequestErrorSent;


        public Task SendOnClearChargingProfileRequestErrorSent(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               IWebSocketConnection?         Connection,
                                                               ClearChargingProfileRequest?  Request,
                                                               OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                               TimeSpan                      Runtime,
                                                               SentMessageResults            SentMessageResult,
                                                               CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnClearChargingProfileRequestErrorSent,
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

        #region Send OnClearChargingProfileResponseErrorSent event

        /// <summary>
        /// An event sent whenever a ClearChargingProfile response error was sent.
        /// </summary>
        public event OnClearChargingProfileResponseErrorSentDelegate? OnClearChargingProfileResponseErrorSent;


        public Task SendOnClearChargingProfileResponseErrorSent(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection?          Connection,
                                                                ClearChargingProfileRequest?   Request,
                                                                ClearChargingProfileResponse?  Response,
                                                                OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                                TimeSpan                       Runtime,
                                                                SentMessageResults             SentMessageResult,
                                                                CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnClearChargingProfileResponseErrorSent,
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
