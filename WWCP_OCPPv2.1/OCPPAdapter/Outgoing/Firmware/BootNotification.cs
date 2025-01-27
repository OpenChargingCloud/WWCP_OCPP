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
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a BootNotification request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnBootNotificationRequestSentDelegate(DateTime                  Timestamp,
                                                               IEventSender              Sender,
                                                               IWebSocketConnection?     Connection,
                                                               BootNotificationRequest   Request,
                                                               SentMessageResults        SentMessageResult,
                                                               CancellationToken         CancellationToken);


    /// <summary>
    /// A BootNotification response.
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

        OnBootNotificationResponseSentDelegate(DateTime                   Timestamp,
                                               IEventSender               Sender,
                                               IWebSocketConnection?      Connection,
                                               BootNotificationRequest    Request,
                                               BootNotificationResponse   Response,
                                               TimeSpan                   Runtime,
                                               SentMessageResults         SentMessageResult,
                                               CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BootNotification request error was sent.
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

        OnBootNotificationRequestErrorSentDelegate(DateTime                       Timestamp,
                                                   IEventSender                   Sender,
                                                   IWebSocketConnection?          Connection,
                                                   BootNotificationRequest?       Request,
                                                   OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                   TimeSpan?                      Runtime,
                                                   SentMessageResults             SentMessageResult,
                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BootNotification response error was sent.
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

        OnBootNotificationResponseErrorSentDelegate(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    IWebSocketConnection?           Connection,
                                                    BootNotificationRequest?        Request,
                                                    BootNotificationResponse?       Response,
                                                    OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                    TimeSpan?                       Runtime,
                                                    SentMessageResults              SentMessageResult,
                                                    CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send BootNotification request

        /// <summary>
        /// An event fired whenever a BootNotification request was sent.
        /// </summary>
        public event OnBootNotificationRequestSentDelegate?  OnBootNotificationRequestSent;


        /// <summary>
        /// Send a BootNotification request.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        public async Task<BootNotificationResponse>

            BootNotification(BootNotificationRequest Request)

        {

            BootNotificationResponse? response = null;

            try
            {

                switch (Request.SerializationFormat)
                {

                    case SerializationFormats.JSON: {

                            #region Sign request message

                            if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                                    Request,
                                    Request.ToJSON(
                                        true,
                                        parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                        parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                    ),
                                    out var signingErrors
                                ))
                            {

                                response = BootNotificationResponse.SignatureError(
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
                                                                         parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                                                         parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                                     )
                                                                 ),

                                                                 sentMessageResult => LogEvent(
                                                                     OnBootNotificationRequestSent,
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationResponse(
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationRequestError(
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

                                response ??= new BootNotificationResponse(
                                                 Request,
                                                 RegistrationStatus.Rejected,
                                                 Timestamp.Now,
                                                 BootNotificationResponse.DefaultInterval,
                                                 Result: Result.FromSendRequestState(sendRequestState)
                                             );

                            }

                        }
                        break;


                    case SerializationFormats.JSON_UTF8_Binary: {

                            #region Sign request message

                            if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                                    Request,
                                    Request.ToJSON(
                                        true,
                                        parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                        parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                    ),
                                    out var signingErrors
                                ))
                            {

                                response = BootNotificationResponse.SignatureError(
                                               Request,
                                               signingErrors
                                           );

                            }

                            #endregion

                            else
                            {

                                #region Send request message

                                var sendRequestState = await SendBinaryRequestAndWait(

                                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                                     Request,
                                                                     Request.ToJSON(
                                                                         false,
                                                                         parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                                                         parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                                     ).ToUTF8Bytes()
                                                                 ),

                                                                 sentMessageResult => LogEvent(
                                                                     OnBootNotificationRequestSent,
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

                                if (sendRequestState.IsValidBinaryResponse(Request, out var binaryResponse))
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationResponse(
                                                         Request,
                                                         binaryResponse,
                                                         sendRequestState.WebSocketConnectionReceived,
                                                         sendRequestState.DestinationReceived,
                                                         sendRequestState.NetworkPathReceived,
                                                         Request.         EventTrackingId,
                                                         Request.         RequestId,
                                                         sendRequestState.ResponseTimestamp,
                                                         Request.         CancellationToken
                                                     );

                                if (sendRequestState.IsValidBinaryRequestError(Request, out var binaryRequestError))
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationRequestError(
                                                         Request,
                                                         binaryRequestError,
                                                         sendRequestState.WebSocketConnectionReceived,
                                                         sendRequestState.DestinationReceived,
                                                         sendRequestState.NetworkPathReceived,
                                                         Request.EventTrackingId,
                                                         Request.RequestId,
                                                         sendRequestState.ResponseTimestamp,
                                                         Request.CancellationToken
                                                     );

                                response ??= new BootNotificationResponse(
                                                 Request,
                                                 RegistrationStatus.Rejected,
                                                 Timestamp.Now,
                                                 BootNotificationResponse.DefaultInterval,
                                                 Result: Result.FromSendRequestState(sendRequestState)
                                             );

                            }

                        }
                        break;


                    case SerializationFormats.BinaryCompact: {

                            #region Sign request message

                            if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                                    Request,
                                    Request.ToBinary(
                                        //parentNetworkingNode.OCPP.CustomBinaryBootNotificationRequestSerializer,
                                        //parentNetworkingNode.OCPP.CustomBinaryChargingStationSerializer,
                                        //parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                        IncludeSignatures: false
                                    ),
                                    out var signingErrors
                                ))
                            {

                                response = BootNotificationResponse.SignatureError(
                                               Request,
                                               signingErrors
                                           );

                            }

                            #endregion

                            else
                            {

                                #region Send request message

                                var sendRequestState = await SendBinaryRequestAndWait(

                                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                                     Request,
                                                                     Request.ToBinary(
                                                                         //parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                                                         //parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                                                         //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                                         //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                                         IncludeSignatures: true
                                                                     )
                                                                 ),

                                                                 sentMessageResult => LogEvent(
                                                                     OnBootNotificationRequestSent,
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationResponse(
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_BootNotificationRequestError(
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

                                response ??= new BootNotificationResponse(
                                                 Request,
                                                 RegistrationStatus.Rejected,
                                                 Timestamp.Now,
                                                 BootNotificationResponse.DefaultInterval,
                                                 Result: Result.FromSendRequestState(sendRequestState)
                                             );

                            }

                        }
                        break;


                    default:
                        response ??= new BootNotificationResponse(
                                         Request,
                                         RegistrationStatus.Rejected,
                                         Timestamp.Now,
                                         BootNotificationResponse.DefaultInterval
                                     );
                        break;

                }

            }
            catch (Exception e)
            {

                response = BootNotificationResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnBootNotificationResponseSent event

        /// <summary>
        /// An event sent whenever a BootNotification response was sent.
        /// </summary>
        public event OnBootNotificationResponseSentDelegate?  OnBootNotificationResponseSent;

        public Task SendOnBootNotificationResponseSent(DateTime                   Timestamp,
                                                       IEventSender               Sender,
                                                       IWebSocketConnection?      Connection,
                                                       BootNotificationRequest    Request,
                                                       BootNotificationResponse   Response,
                                                       TimeSpan                   Runtime,
                                                       SentMessageResults         SentMessageResult,
                                                       CancellationToken          CancellationToken = default)

            => LogEvent(
                   OnBootNotificationResponseSent,
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

        #region Send OnBootNotificationRequestErrorSent event

        /// <summary>
        /// An event sent whenever a BootNotification request error was sent.
        /// </summary>
        public event OnBootNotificationRequestErrorSentDelegate? OnBootNotificationRequestErrorSent;


        public Task SendOnBootNotificationRequestErrorSent(DateTime                      Timestamp,
                                                           IEventSender                  Sender,
                                                           IWebSocketConnection?         Connection,
                                                           BootNotificationRequest?      Request,
                                                           OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                           TimeSpan                      Runtime,
                                                           SentMessageResults            SentMessageResult,
                                                           CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnBootNotificationRequestErrorSent,
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

        #region Send OnBootNotificationResponseErrorSent event

        /// <summary>
        /// An event sent whenever a BootNotification response error was sent.
        /// </summary>
        public event OnBootNotificationResponseErrorSentDelegate? OnBootNotificationResponseErrorSent;


        public Task SendOnBootNotificationResponseErrorSent(DateTime                       Timestamp,
                                                            IEventSender                   Sender,
                                                            IWebSocketConnection?          Connection,
                                                            BootNotificationRequest?       Request,
                                                            BootNotificationResponse?      Response,
                                                            OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                            TimeSpan                       Runtime,
                                                            SentMessageResults             SentMessageResult,
                                                            CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnBootNotificationResponseErrorSent,
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
