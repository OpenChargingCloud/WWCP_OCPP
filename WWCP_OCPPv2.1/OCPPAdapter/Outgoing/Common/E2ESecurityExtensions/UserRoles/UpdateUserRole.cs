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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever an UpdateUserRole request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUpdateUserRoleRequestSentDelegate(DateTime                Timestamp,
                                                             IEventSender            Sender,
                                                             IWebSocketConnection?   Connection,
                                                             UpdateUserRoleRequest   Request,
                                                             SentMessageResults      SendMessageResult,
                                                             CancellationToken       CancellationToken = default);


    /// <summary>
    /// A UpdateUserRole response.
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

        OnUpdateUserRoleResponseSentDelegate(DateTime                 Timestamp,
                                             IEventSender             Sender,
                                             IWebSocketConnection     Connection,
                                             UpdateUserRoleRequest    Request,
                                             UpdateUserRoleResponse   Response,
                                             TimeSpan                 Runtime,
                                             SentMessageResults       SendMessageResult,
                                             CancellationToken        CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an UpdateUserRole request error was sent.
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

        OnUpdateUserRoleRequestErrorSentDelegate(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 UpdateUserRoleRequest?         Request,
                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                 TimeSpan?                      Runtime,
                                                 SentMessageResults             SendMessageResult,
                                                 CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an UpdateUserRole response error was sent.
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

        OnUpdateUserRoleResponseErrorSentDelegate(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  UpdateUserRoleRequest?          Request,
                                                  UpdateUserRoleResponse?         Response,
                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                  TimeSpan?                       Runtime,
                                                  SentMessageResults              SendMessageResult,
                                                  CancellationToken               CancellationToken = default);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send UpdateUserRole request

        /// <summary>
        /// An event fired whenever an UpdateUserRole request was sent.
        /// </summary>
        public event OnUpdateUserRoleRequestSentDelegate?  OnUpdateUserRoleRequestSent;


        /// <summary>
        /// Send an UpdateUserRole request.
        /// </summary>
        /// <param name="Request">A UpdateUserRole request.</param>
        public async Task<UpdateUserRoleResponse>

            UpdateUserRole(UpdateUserRoleRequest Request)

        {

            UpdateUserRoleResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomUpdateUserRoleRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = UpdateUserRoleResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomUpdateUserRoleRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sendMessageResult => LogEvent(
                                                         OnUpdateUserRoleRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_UpdateUserRoleResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_UpdateUserRoleRequestError(
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

                    response ??= new UpdateUserRoleResponse(
                                     Request,
                                     GenericStatus.Rejected,
                                     null,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = UpdateUserRoleResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnUpdateUserRoleResponseSent event

        /// <summary>
        /// An event sent whenever an UpdateUserRole response was sent.
        /// </summary>
        public event OnUpdateUserRoleResponseSentDelegate?  OnUpdateUserRoleResponseSent;

        public Task SendOnUpdateUserRoleResponseSent(DateTime                Timestamp,
                                                     IEventSender            Sender,
                                                     IWebSocketConnection    Connection,
                                                     UpdateUserRoleRequest   Request,
                                                     UpdateUserRoleResponse  Response,
                                                     TimeSpan                Runtime,
                                                     SentMessageResults      SendMessageResult,
                                                     CancellationToken       CancellationToken = default)

            => LogEvent(
                   OnUpdateUserRoleResponseSent,
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

        #region Send OnUpdateUserRoleRequestErrorSent event

        /// <summary>
        /// An event sent whenever an UpdateUserRole request error was sent.
        /// </summary>
        public event OnUpdateUserRoleRequestErrorSentDelegate? OnUpdateUserRoleRequestErrorSent;


        public Task SendOnUpdateUserRoleRequestErrorSent(DateTime                      Timestamp,
                                                         IEventSender                  Sender,
                                                         IWebSocketConnection          Connection,
                                                         UpdateUserRoleRequest?        Request,
                                                         OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                         TimeSpan                      Runtime,
                                                         SentMessageResults            SendMessageResult,
                                                         CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnUpdateUserRoleRequestErrorSent,
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

        #region Send OnUpdateUserRoleResponseErrorSent event

        /// <summary>
        /// An event sent whenever an UpdateUserRole response error was sent.
        /// </summary>
        public event OnUpdateUserRoleResponseErrorSentDelegate? OnUpdateUserRoleResponseErrorSent;


        public Task SendOnUpdateUserRoleResponseErrorSent(DateTime                       Timestamp,
                                                          IEventSender                   Sender,
                                                          IWebSocketConnection           Connection,
                                                          UpdateUserRoleRequest?         Request,
                                                          UpdateUserRoleResponse?        Response,
                                                          OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                          TimeSpan                       Runtime,
                                                          SentMessageResults             SendMessageResult,
                                                          CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnUpdateUserRoleResponseErrorSent,
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
