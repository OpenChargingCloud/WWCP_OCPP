/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A delegate called whenever a DeleteSSHKey request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDeleteSSHKeyRequestSentDelegate(DateTimeOffset             Timestamp,
                                                                IEventSender               Sender,
                                                                IMessageConnection?        Connection,
                                                                DeleteSSHKeyRequest   Request,
                                                                SentMessageResults         SentMessageResult,
                                                                CancellationToken          CancellationToken);


    /// <summary>
    /// A DeleteSSHKey response.
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

        OnDeleteSSHKeyResponseSentDelegate(DateTimeOffset              Timestamp,
                                                IEventSender                Sender,
                                                IMessageConnection?         Connection,
                                                DeleteSSHKeyRequest    Request,
                                                DeleteSSHKeyResponse   Response,
                                                TimeSpan                    Runtime,
                                                SentMessageResults          SentMessageResult,
                                                CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DeleteSSHKey request error was sent.
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

        OnDeleteSSHKeyRequestErrorSentDelegate(DateTimeOffset                 Timestamp,
                                                    IEventSender                   Sender,
                                                    IMessageConnection?            Connection,
                                                    DeleteSSHKeyRequest?      Request,
                                                    OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                    TimeSpan?                      Runtime,
                                                    SentMessageResults             SentMessageResult,
                                                    CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DeleteSSHKey response error was sent.
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

        OnDeleteSSHKeyResponseErrorSentDelegate(DateTimeOffset                  Timestamp,
                                                     IEventSender                    Sender,
                                                     IMessageConnection?             Connection,
                                                     DeleteSSHKeyRequest?       Request,
                                                     DeleteSSHKeyResponse?      Response,
                                                     OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                     TimeSpan?                       Runtime,
                                                     SentMessageResults              SentMessageResult,
                                                     CancellationToken               CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send DeleteSSHKey request

        /// <summary>
        /// An event fired whenever a DeleteSSHKey request was sent.
        /// </summary>
        public event OnDeleteSSHKeyRequestSentDelegate?  OnDeleteSSHKeyRequestSent;


        /// <summary>
        /// Send a DeleteSSHKey request.
        /// </summary>
        /// <param name="Request">A DeleteSSHKey request.</param>
        public async Task<DeleteSSHKeyResponse>

            DeleteSSHKey(DeleteSSHKeyRequest Request)

        {

            DeleteSSHKeyResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomDeleteSSHKeyRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = DeleteSSHKeyResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomDeleteSSHKeyRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnDeleteSSHKeyRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_DeleteSSHKeyResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_DeleteSSHKeyRequestError(
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

                    response ??= new DeleteSSHKeyResponse(
                                     Request,
                                     GenericStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = DeleteSSHKeyResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnDeleteSSHKeyResponseSent event

        /// <summary>
        /// An event sent whenever a DeleteSSHKey response was sent.
        /// </summary>
        public event OnDeleteSSHKeyResponseSentDelegate?  OnDeleteSSHKeyResponseSent;

        public Task SendOnDeleteSSHKeyResponseSent(DateTimeOffset             Timestamp,
                                                        IEventSender               Sender,
                                                        IMessageConnection?        Connection,
                                                        DeleteSSHKeyRequest   Request,
                                                        DeleteSSHKeyResponse  Response,
                                                        TimeSpan                   Runtime,
                                                        SentMessageResults         SentMessageResult,
                                                        CancellationToken          CancellationToken = default)

            => LogEvent(
                   OnDeleteSSHKeyResponseSent,
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

        #region Send OnDeleteSSHKeyRequestErrorSent event

        /// <summary>
        /// An event sent whenever a DeleteSSHKey request error was sent.
        /// </summary>
        public event OnDeleteSSHKeyRequestErrorSentDelegate? OnDeleteSSHKeyRequestErrorSent;


        public Task SendOnDeleteSSHKeyRequestErrorSent(DateTimeOffset                Timestamp,
                                                            IEventSender                  Sender,
                                                            IMessageConnection?           Connection,
                                                            DeleteSSHKeyRequest?     Request,
                                                            OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                            TimeSpan                      Runtime,
                                                            SentMessageResults            SentMessageResult,
                                                            CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnDeleteSSHKeyRequestErrorSent,
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

        #region Send OnDeleteSSHKeyResponseErrorSent event

        /// <summary>
        /// An event sent whenever a DeleteSSHKey response error was sent.
        /// </summary>
        public event OnDeleteSSHKeyResponseErrorSentDelegate? OnDeleteSSHKeyResponseErrorSent;


        public Task SendOnDeleteSSHKeyResponseErrorSent(DateTimeOffset                 Timestamp,
                                                             IEventSender                   Sender,
                                                             IMessageConnection?            Connection,
                                                             DeleteSSHKeyRequest?      Request,
                                                             DeleteSSHKeyResponse?     Response,
                                                             OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                             TimeSpan                       Runtime,
                                                             SentMessageResults             SentMessageResult,
                                                             CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnDeleteSSHKeyResponseErrorSent,
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
