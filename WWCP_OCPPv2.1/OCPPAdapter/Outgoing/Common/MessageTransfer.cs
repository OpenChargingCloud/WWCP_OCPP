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
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a Message request was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="MessageTransferMessage">The message.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task OnMessageTransferMessageSentDelegate(DateTime                 Timestamp,
                                                              IEventSender             Sender,
                                                              IWebSocketConnection?    Connection,
                                                              MessageTransferMessage   MessageTransferMessage,
                                                              SentMessageResults       SentMessageResult,
                                                              CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a Message request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="MessageTransferMessage">The message, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    public delegate Task

        OnMessageRequestErrorSentDelegate(DateTime                       Timestamp,
                                          IEventSender                   Sender,
                                          IWebSocketConnection?          Connection,
                                          MessageTransferMessage?        MessageTransferMessage,
                                          OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                          TimeSpan?                      Runtime,
                                          SentMessageResults             SentMessageResult,
                                          CancellationToken              CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send MessageTransfer message

        /// <summary>
        /// An event fired whenever a MessageTransfer was sent.
        /// </summary>
        public event OnMessageTransferMessageSentDelegate?  OnMessageTransferMessageSent;


        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Message">A message.</param>
        public async Task<SentMessageResult>

            MessageTransfer(MessageTransferMessage Message)

        {

            SentMessageResult? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignSendMessage(
                        Message,
                        Message.ToJSON(
                            parentNetworkingNode.OCPP.CustomMessageTransferMessageSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    //response = MessageResponse.SignatureError(
                    //               Message,
                    //               signingErrors
                    //           );

                }

                #endregion

                else
                {

                    response = await SendJSONSendMessage(

                                         OCPP_JSONSendMessage.FromDatagram(
                                             Message,
                                             Message.ToJSON(
                                                 parentNetworkingNode.OCPP.CustomMessageTransferMessageSerializer,
                                                 parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                 parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                             )
                                         ),

                                         sentMessageResult => LogEvent(
                                             OnMessageTransferMessageSent,
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

                }

            }
            catch (Exception e)
            {
                response = SentMessageResult.TransmissionFailed(e);
            }

            response ??= SentMessageResult.Unknown();

            return response;

        }

        #endregion


        #region Send OnMessageRequestErrorSent event

        /// <summary>
        /// An event sent whenever a Message request error was sent.
        /// </summary>
        public event OnMessageRequestErrorSentDelegate? OnMessageRequestErrorSent;


        public Task SendOnMessageRequestErrorSent(DateTime                      Timestamp,
                                                  IEventSender                  Sender,
                                                  IWebSocketConnection?         Connection,
                                                  MessageTransferMessage?       MessageTransferMessage,
                                                  OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                  TimeSpan                      Runtime,
                                                  SentMessageResults            SentMessageResult,
                                                  CancellationToken             CancellationToken = default)

            => LogEvent(
                   OnMessageRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       MessageTransferMessage,
                       RequestErrorMessage,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion


    }

}
