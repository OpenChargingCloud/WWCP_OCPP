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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a SetSignaturePolicy request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetSignaturePolicyRequestSentDelegate(DateTimeOffset           Timestamp,
                                                              IEventSender             Sender,
                                                              IMessageConnection?      Connection,
                                                              SetSignaturePolicyRequest   Request,
                                                              SentMessageResults       SentMessageResult,
                                                              CancellationToken        CancellationToken);


    /// <summary>
    /// A SetSignaturePolicy response.
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

        OnSetSignaturePolicyResponseSentDelegate(DateTimeOffset            Timestamp,
                                              IEventSender              Sender,
                                              IMessageConnection?       Connection,
                                              SetSignaturePolicyRequest    Request,
                                              SetSignaturePolicyResponse   Response,
                                              TimeSpan                  Runtime,
                                              SentMessageResults        SentMessageResult,
                                              CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetSignaturePolicy request error was sent.
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

        OnSetSignaturePolicyRequestErrorSentDelegate(DateTimeOffset                 Timestamp,
                                                  IEventSender                   Sender,
                                                  IMessageConnection?            Connection,
                                                  SetSignaturePolicyRequest?        Request,
                                                  OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                  TimeSpan?                      Runtime,
                                                  SentMessageResults             SentMessageResult,
                                                  CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetSignaturePolicy response error was sent.
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

        OnSetSignaturePolicyResponseErrorSentDelegate(DateTimeOffset                  Timestamp,
                                                   IEventSender                    Sender,
                                                   IMessageConnection?             Connection,
                                                   SetSignaturePolicyRequest?         Request,
                                                   SetSignaturePolicyResponse?        Response,
                                                   OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                   TimeSpan?                       Runtime,
                                                   SentMessageResults              SentMessageResult,
                                                   CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SetSignaturePolicy request

        /// <summary>
        /// An event fired whenever a SetSignaturePolicy request was sent.
        /// </summary>
        public event OnSetSignaturePolicyRequestSentDelegate?  OnSetSignaturePolicyRequestSent;


        /// <summary>
        /// Send a SetSignaturePolicy request.
        /// </summary>
        /// <param name="Request">A SetSignaturePolicy request.</param>
        public async Task<SetSignaturePolicyResponse>

            SetSignaturePolicy(SetSignaturePolicyRequest Request)

        {

            SetSignaturePolicyResponse? response = null;

            try
            {

                switch (Request.SerializationFormat)
                {

                    case SerializationFormats.JSON: {

                            #region Sign request message

                            if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                                    Request,
                                    Request.ToJSON(
                                        parentNetworkingNode.OCPP.CustomSetSignaturePolicyRequestSerializer,
                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                    ),
                                    out var signingErrors
                                ))
                            {

                                response = SetSignaturePolicyResponse.SignatureError(
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
                                                                         parentNetworkingNode.OCPP.CustomSetSignaturePolicyRequestSerializer,
                                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                                     )
                                                                 ),

                                                                 sentMessageResult => LogEvent(
                                                                     OnSetSignaturePolicyRequestSent,
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyResponse(
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
                                    response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyRequestError(
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

                                response ??= new SetSignaturePolicyResponse(
                                                 Request,
                                                 GenericStatus.Rejected,
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
                        //                parentNetworkingNode.OCPP.CustomSetSignaturePolicyRequestSerializer,
                        //                parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                        //                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                        //                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        //            ),
                        //            out var signingErrors
                        //        ))
                        //    {

                        //        response = SetSignaturePolicyResponse.SignatureError(
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
                        //                                                 parentNetworkingNode.OCPP.CustomSetSignaturePolicyRequestSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomSignatureSerializer,
                        //                                                 parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        //                                             ).ToUTF8Bytes()
                        //                                         ),

                        //                                         sentMessageResult => LogEvent(
                        //                                             OnSetSignaturePolicyRequestSent,
                        //                                             loggingDelegate => loggingDelegate.Invoke(
                        //                                                 Now,
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
                        //            response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyResponse(
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
                        //            response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyRequestError(
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

                        //        response ??= new SetSignaturePolicyResponse(
                        //                         Request,
                        //                         RegistrationStatus.Rejected,
                        //                         Now,
                        //                         SetSignaturePolicyResponse.DefaultInterval,
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
                            //            //parentNetworkingNode.OCPP.CustomBinarySetSignaturePolicyRequestSerializer,
                            //            //parentNetworkingNode.OCPP.CustomBinaryChargingStationSerializer,
                            //            //parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            //            IncludeSignatures: false
                            //        ),
                            //        out var signingErrors
                            //    ))
                            //{

                            //    response = SetSignaturePolicyResponse.SignatureError(
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
                            //                                             //parentNetworkingNode.OCPP.CustomSetSignaturePolicyRequestSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            //                                             //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            //                                             IncludeSignatures: true
                            //                                         )
                            //                                     ),

                            //                                     sentMessageResult => LogEvent(
                            //                                         OnSetSignaturePolicyRequestSent,
                            //                                         loggingDelegate => loggingDelegate.Invoke(
                            //                                             Now,
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
                            //        response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyResponse(
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
                            //        response = await parentNetworkingNode.OCPP.IN.Receive_SetSignaturePolicyRequestError(
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

                            //    response ??= new SetSignaturePolicyResponse(
                            //                     Request,
                            //                     RegistrationStatus.Rejected,
                            //                     Now,
                            //                     SetSignaturePolicyResponse.DefaultInterval,
                            //                     Result: Result.FromSendRequestState(sendRequestState)
                            //                 );

                            //}

                        }
                        break;


                    default:
                        response ??= new SetSignaturePolicyResponse(
                                         Request,
                                         GenericStatus.Rejected
                                     );
                        break;

                }

            }
            catch (Exception e)
            {

                response = SetSignaturePolicyResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSetSignaturePolicyResponseSent event

        /// <summary>
        /// An event sent whenever a SetSignaturePolicy response was sent.
        /// </summary>
        public event OnSetSignaturePolicyResponseSentDelegate?  OnSetSignaturePolicyResponseSent;

        public Task SendOnSetSignaturePolicyResponseSent(DateTimeOffset            Timestamp,
                                                      IEventSender              Sender,
                                                      IMessageConnection?       Connection,
                                                      SetSignaturePolicyRequest    Request,
                                                      SetSignaturePolicyResponse   Response,
                                                      TimeSpan                  Runtime,
                                                      SentMessageResults        SentMessageResult,
                                                      CancellationToken         CancellationToken = default)

            => LogEvent(
                   OnSetSignaturePolicyResponseSent,
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

        #region Send OnSetSignaturePolicyRequestErrorSent event

        /// <summary>
        /// An event sent whenever a SetSignaturePolicy request error was sent.
        /// </summary>
        public event OnSetSignaturePolicyRequestErrorSentDelegate? OnSetSignaturePolicyRequestErrorSent;


        public Task SendOnSetSignaturePolicyRequestErrorSent(DateTimeOffset                Timestamp,
                                                          IEventSender                  Sender,
                                                          IMessageConnection?           Connection,
                                                          SetSignaturePolicyRequest?       Request,
                                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                          TimeSpan                      Runtime,
                                                          SentMessageResults            SentMessageResult,
                                                          CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSetSignaturePolicyRequestErrorSent,
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

        #region Send OnSetSignaturePolicyResponseErrorSent event

        /// <summary>
        /// An event sent whenever a SetSignaturePolicy response error was sent.
        /// </summary>
        public event OnSetSignaturePolicyResponseErrorSentDelegate? OnSetSignaturePolicyResponseErrorSent;


        public Task SendOnSetSignaturePolicyResponseErrorSent(DateTimeOffset                 Timestamp,
                                                           IEventSender                   Sender,
                                                           IMessageConnection?            Connection,
                                                           SetSignaturePolicyRequest?        Request,
                                                           SetSignaturePolicyResponse?       Response,
                                                           OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                           TimeSpan                       Runtime,
                                                           SentMessageResults             SentMessageResult,
                                                           CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSetSignaturePolicyResponseErrorSent,
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
