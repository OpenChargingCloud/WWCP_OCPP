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
    /// A delegate called whenever a SetVariables request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetVariablesRequestSentDelegate(DateTime                Timestamp,
                                                           IEventSender            Sender,
                                                           IWebSocketConnection?   Connection,
                                                           SetVariablesRequest     Request,
                                                           SentMessageResults      SendMessageResult,
                                                           CancellationToken       CancellationToken = default);


    /// <summary>
    /// A SetVariables response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetVariablesResponseSentDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           SetVariablesRequest    Request,
                                           SetVariablesResponse   Response,
                                           TimeSpan               Runtime,
                                           SentMessageResults     SendMessageResult,
                                           CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a SetVariables request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetVariablesRequestErrorSentDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               IWebSocketConnection           Connection,
                                               SetVariablesRequest?           Request,
                                               OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                               TimeSpan?                      Runtime,
                                               SentMessageResults             SendMessageResult,
                                               CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a SetVariables response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetVariablesResponseErrorSentDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                IWebSocketConnection            Connection,
                                                SetVariablesRequest?            Request,
                                                SetVariablesResponse?           Response,
                                                OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                TimeSpan?                       Runtime,
                                                SentMessageResults              SendMessageResult,
                                                CancellationToken               CancellationToken = default);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send SetVariables request

        /// <summary>
        /// An event fired whenever a SetVariables request was sent.
        /// </summary>
        public event OnSetVariablesRequestSentDelegate?  OnSetVariablesRequestSent;


        /// <summary>
        /// Send a SetVariables request.
        /// </summary>
        /// <param name="Request">A SetVariables request.</param>
        public async Task<SetVariablesResponse>

            SetVariables(SetVariablesRequest Request)

        {

            SetVariablesResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSetVariableDataSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = SetVariablesResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSetVariableDataSerializer,
                                                             parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                             parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnSetVariablesRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sendMessageResult.Connection,
                                                             Request,
                                                             sendMessageResult.Result
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetVariablesResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_SetVariablesRequestError(
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

                    response ??= new SetVariablesResponse(
                                     Request,
                                     [],
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = SetVariablesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnSetVariablesResponseSent event

        /// <summary>
        /// An event sent whenever a SetVariables response was sent.
        /// </summary>
        public event OnSetVariablesResponseSentDelegate?  OnSetVariablesResponseSent;

        public Task SendOnSetVariablesResponseSent(DateTime              Timestamp,
                                                   IEventSender          Sender,
                                                   IWebSocketConnection  Connection,
                                                   SetVariablesRequest   Request,
                                                   SetVariablesResponse  Response,
                                                   TimeSpan              Runtime,
                                                   SentMessageResults    SendMessageResult,
                                                   CancellationToken     CancellationToken = default)

            => LogEvent(
                   OnSetVariablesResponseSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnSetVariablesRequestErrorSent event

        /// <summary>
        /// An event sent whenever a SetVariables request error was sent.
        /// </summary>
        public event OnSetVariablesRequestErrorSentDelegate? OnSetVariablesRequestErrorSent;


        public Task SendOnSetVariablesRequestErrorSent(DateTime                      Timestamp,
                                                       IEventSender                  Sender,
                                                       IWebSocketConnection          Connection,
                                                       SetVariablesRequest?          Request,
                                                       OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                       TimeSpan                      Runtime,
                                                       SentMessageResults            SendMessageResult,
                                                       CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnSetVariablesRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnSetVariablesResponseErrorSent event

        /// <summary>
        /// An event sent whenever a SetVariables response error was sent.
        /// </summary>
        public event OnSetVariablesResponseErrorSentDelegate? OnSetVariablesResponseErrorSent;


        public Task SendOnSetVariablesResponseErrorSent(DateTime                       Timestamp,
                                                        IEventSender                   Sender,
                                                        IWebSocketConnection           Connection,
                                                        SetVariablesRequest?           Request,
                                                        SetVariablesResponse?          Response,
                                                        OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                        TimeSpan                       Runtime,
                                                        SentMessageResults             SendMessageResult,
                                                        CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnSetVariablesResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime,
                       SendMessageResult,
                       CancellationToken
                   )
               );

        #endregion

    }

}
