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

using cloud.charging.open.protocols.WWCP.WebSockets;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a DiagnosticsStatusNotification request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDiagnosticsStatusNotificationRequestSentDelegate(DateTime                  Timestamp,
                                                               IEventSender              Sender,
                                                               IWebSocketConnection?     Connection,
                                                               DiagnosticsStatusNotificationRequest   Request,
                                                               SentMessageResults        SentMessageResult,
                                                               CancellationToken         CancellationToken);


    /// <summary>
    /// A DiagnosticsStatusNotification response.
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

        OnDiagnosticsStatusNotificationResponseSentDelegate(DateTime                   Timestamp,
                                               IEventSender               Sender,
                                               IWebSocketConnection?      Connection,
                                               DiagnosticsStatusNotificationRequest    Request,
                                               DiagnosticsStatusNotificationResponse   Response,
                                               TimeSpan                   Runtime,
                                               SentMessageResults         SentMessageResult,
                                               CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DiagnosticsStatusNotification request error was sent.
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

        OnDiagnosticsStatusNotificationRequestErrorSentDelegate(DateTime                       Timestamp,
                                                   IEventSender                   Sender,
                                                   IWebSocketConnection?          Connection,
                                                   DiagnosticsStatusNotificationRequest?       Request,
                                                   OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                   TimeSpan?                      Runtime,
                                                   SentMessageResults             SentMessageResult,
                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DiagnosticsStatusNotification response error was sent.
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

        OnDiagnosticsStatusNotificationResponseErrorSentDelegate(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    IWebSocketConnection?           Connection,
                                                    DiagnosticsStatusNotificationRequest?        Request,
                                                    DiagnosticsStatusNotificationResponse?       Response,
                                                    OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                    TimeSpan?                       Runtime,
                                                    SentMessageResults              SentMessageResult,
                                                    CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send DiagnosticsStatusNotification request

        /// <summary>
        /// An event fired whenever a DiagnosticsStatusNotification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestSentDelegate?  OnDiagnosticsStatusNotificationRequestSent;


        /// <summary>
        /// Send a DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="Request">A DiagnosticsStatusNotification request.</param>
        public async Task<DiagnosticsStatusNotificationResponse>

            DiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest Request)

        {

            DiagnosticsStatusNotificationResponse? response = null;

            try
            {

                switch (Request.SerializationFormat)
                {

                    case SerializationFormats.JSON: {

                            #region Sign request message

                            if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                                    Request,
                                    Request.ToJSON(
                                        parentNetworkingNode.OCPP.CustomDiagnosticsStatusNotificationRequestSerializer,
                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                    ),
                                    out var signingErrors
                                ))
                            {

                                response = DiagnosticsStatusNotificationResponse.SignatureError(
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
                                                                         parentNetworkingNode.OCPP.CustomDiagnosticsStatusNotificationRequestSerializer,
                                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                                     )
                                                                 ),

                                                                 sentMessageResult => LogEvent(
                                                                     OnDiagnosticsStatusNotificationRequestSent,
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationResponse(
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationRequestError(
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

                                response ??= new DiagnosticsStatusNotificationResponse(
                                                 Request,
                                                 Result: Result.FromSendRequestState(sendRequestState)
                                             );

                            }

                        }
                        break;


                    case SerializationFormats.JSON_UTF8_Binary: {

                        //    #region Sign request message

                        //    if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        //            Request,
                        //            Request.ToJSON(
                        //                parentNetworkingNode.OCPP.CustomDiagnosticsStatusNotificationRequestSerializer,
                        //                parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                        //                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                        //                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        //            ),
                        //            out var signingErrors
                        //        ))
                        //    {

                        //        response = DiagnosticsStatusNotificationResponse.SignatureError(
                        //                       Request,
                        //                       signingErrors
                        //                   );

                        //    }

                        //    #endregion

                        //    else
                        //    {

                        //        #region Send request message

                        //        var sendRequestState = await SendBinaryRequestAndWait(

                        //                                         OCPP_BinaryRequestMessage.FromRequest(
                        //                                             Request,
                        //                                             Request.ToJSON(
                        //                                                 parentNetworkingNode.OCPP.CustomDiagnosticsStatusNotificationRequestSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomSignatureSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        //                                             ).ToUTF8Bytes()
                        //                                         ),

                        //                                         sentMessageResult => LogEvent(
                        //                                             OnDiagnosticsStatusNotificationRequestSent,
                        //                                             loggingDelegate => loggingDelegate.Invoke(
                        //                                                 Timestamp.Now,
                        //                                                 parentNetworkingNode,
                        //                                                 sentMessageResult.Connection,
                        //                                                 Request,
                        //                                                 sentMessageResult.Result,
                        //                                                 Request.CancellationToken
                        //                                             )
                        //                                         )

                        //                                     );

                        //        #endregion

                        //        if (sendRequestState.IsValidBinaryResponse(Request, out var binaryResponse))
                        //            response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationResponse(
                        //                                 Request,
                        //                                 binaryResponse,
                        //                                 sendRequestState.WebSocketConnectionReceived,
                        //                                 sendRequestState.DestinationReceived,
                        //                                 sendRequestState.NetworkPathReceived,
                        //                                 Request.         EventTrackingId,
                        //                                 Request.         RequestId,
                        //                                 sendRequestState.ResponseTimestamp,
                        //                                 Request.         CancellationToken
                        //                             );

                        //        if (sendRequestState.IsValidBinaryRequestError(Request, out var binaryRequestError))
                        //            response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationRequestError(
                        //                                 Request,
                        //                                 binaryRequestError,
                        //                                 sendRequestState.WebSocketConnectionReceived,
                        //                                 sendRequestState.DestinationReceived,
                        //                                 sendRequestState.NetworkPathReceived,
                        //                                 Request.EventTrackingId,
                        //                                 Request.RequestId,
                        //                                 sendRequestState.ResponseTimestamp,
                        //                                 Request.CancellationToken
                        //                             );

                        //        response ??= new DiagnosticsStatusNotificationResponse(
                        //                         Request,
                        //                         RegistrationStatus.Rejected,
                        //                         Timestamp.Now,
                        //                         DiagnosticsStatusNotificationResponse.DefaultInterval,
                        //                         Result: Result.FromSendRequestState(sendRequestState)
                        //                     );

                        //    }

                        }
                        break;


                    case SerializationFormats.BinaryCompact: {

                            //#region Sign request message

                            //if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                            //        Request,
                            //        Request.ToBinary(
                            //            //parentNetworkingNode.OCPP.CustomBinaryDiagnosticsStatusNotificationRequestSerializer,
                            //            //parentNetworkingNode.OCPP.CustomBinaryChargingStationSerializer,
                            //            //parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            //            IncludeSignatures: false
                            //        ),
                            //        out var signingErrors
                            //    ))
                            //{

                            //    response = DiagnosticsStatusNotificationResponse.SignatureError(
                            //                   Request,
                            //                   signingErrors
                            //               );

                            //}

                            //#endregion

                            //else
                            //{

                            //    #region Send request message

                            //    var sendRequestState = await SendBinaryRequestAndWait(

                            //                                     OCPP_BinaryRequestMessage.FromRequest(
                            //                                         Request,
                            //                                         Request.ToBinary(
                            //                                             //parentNetworkingNode.OCPP.CustomDiagnosticsStatusNotificationRequestSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            //                                             IncludeSignatures: true
                            //                                         )
                            //                                     ),

                            //                                     sentMessageResult => LogEvent(
                            //                                         OnDiagnosticsStatusNotificationRequestSent,
                            //                                         loggingDelegate => loggingDelegate.Invoke(
                            //                                             Timestamp.Now,
                            //                                             parentNetworkingNode,
                            //                                             sentMessageResult.Connection,
                            //                                             Request,
                            //                                             sentMessageResult.Result,
                            //                                             Request.CancellationToken
                            //                                         )
                            //                                     )

                            //                                 );

                            //    #endregion

                            //    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                            //        response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationResponse(
                            //                             Request,
                            //                             jsonResponse,
                            //                             sendRequestState.WebSocketConnectionReceived,
                            //                             sendRequestState.DestinationReceived,
                            //                             sendRequestState.NetworkPathReceived,
                            //                             Request.         EventTrackingId,
                            //                             Request.         RequestId,
                            //                             sendRequestState.ResponseTimestamp,
                            //                             Request.         CancellationToken
                            //                         );

                            //    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                            //        response = await parentNetworkingNode.OCPP.IN.Receive_DiagnosticsStatusNotificationRequestError(
                            //                             Request,
                            //                             jsonRequestError,
                            //                             sendRequestState.WebSocketConnectionReceived,
                            //                             sendRequestState.DestinationReceived,
                            //                             sendRequestState.NetworkPathReceived,
                            //                             Request.EventTrackingId,
                            //                             Request.RequestId,
                            //                             sendRequestState.ResponseTimestamp,
                            //                             Request.CancellationToken
                            //                         );

                            //    response ??= new DiagnosticsStatusNotificationResponse(
                            //                     Request,
                            //                     RegistrationStatus.Rejected,
                            //                     Timestamp.Now,
                            //                     DiagnosticsStatusNotificationResponse.DefaultInterval,
                            //                     Result: Result.FromSendRequestState(sendRequestState)
                            //                 );

                            //}

                        }
                        break;

                }

                response ??= new DiagnosticsStatusNotificationResponse(
                                 Request
                             );

            }
            catch (Exception e)
            {

                response = DiagnosticsStatusNotificationResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnDiagnosticsStatusNotificationResponseSent event

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification response was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseSentDelegate?  OnDiagnosticsStatusNotificationResponseSent;

        public Task SendOnDiagnosticsStatusNotificationResponseSent(DateTime                               Timestamp,
                                                                    IEventSender                           Sender,
                                                                    IWebSocketConnection?                  Connection,
                                                                    DiagnosticsStatusNotificationRequest   Request,
                                                                    DiagnosticsStatusNotificationResponse  Response,
                                                                    TimeSpan                               Runtime,
                                                                    SentMessageResults                     SentMessageResult,
                                                                    CancellationToken                      CancellationToken = default)

            => LogEvent(
                   OnDiagnosticsStatusNotificationResponseSent,
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

        #region Send OnDiagnosticsStatusNotificationRequestErrorSent event

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification request error was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestErrorSentDelegate? OnDiagnosticsStatusNotificationRequestErrorSent;


        public Task SendOnDiagnosticsStatusNotificationRequestErrorSent(DateTime                               Timestamp,
                                                                        IEventSender                           Sender,
                                                                        IWebSocketConnection?                  Connection,
                                                                        DiagnosticsStatusNotificationRequest?  Request,
                                                                        OCPP_JSONRequestErrorMessage           RequestErrorMessage,
                                                                        TimeSpan                               Runtime,
                                                                        SentMessageResults                     SentMessageResult,
                                                                        CancellationToken                      CancellationToken = default)

            => LogEvent(
                   OnDiagnosticsStatusNotificationRequestErrorSent,
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

        #region Send OnDiagnosticsStatusNotificationResponseErrorSent event

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification response error was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseErrorSentDelegate? OnDiagnosticsStatusNotificationResponseErrorSent;


        public Task SendOnDiagnosticsStatusNotificationResponseErrorSent(DateTime                                Timestamp,
                                                                         IEventSender                            Sender,
                                                                         IWebSocketConnection?                   Connection,
                                                                         DiagnosticsStatusNotificationRequest?   Request,
                                                                         DiagnosticsStatusNotificationResponse?  Response,
                                                                         OCPP_JSONResponseErrorMessage           ResponseErrorMessage,
                                                                         TimeSpan                                Runtime,
                                                                         SentMessageResults                      SentMessageResult,
                                                                         CancellationToken                       CancellationToken = default)

            => LogEvent(
                   OnDiagnosticsStatusNotificationResponseErrorSent,
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
