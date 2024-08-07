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

using System.Runtime.CompilerServices;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageSentDelegate         (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONRequestMessage          JSONRequestMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnJSONResponseMessageSentDelegate        (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONResponseMessage         JSONResponseMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnJSONRequestErrorMessageSentDelegate    (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnJSONResponseErrorMessageSentDelegate   (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONResponseErrorMessage    JSONRequestErrorMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnJSONSendMessageSentDelegate            (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONSendMessage             JSONSendMessage,
                                                                   SentMessageResults               SendMessageResult);



    public delegate Task OnBinaryRequestMessageSentDelegate       (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryRequestMessage        BinaryRequestMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnBinaryResponseMessageSentDelegate      (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryResponseMessage       BinaryResponseMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnBinaryRequestErrorMessageSentDelegate  (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnBinaryResponseErrorMessageSentDelegate (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryResponseErrorMessage  BinaryRequestErrorMessage,
                                                                   SentMessageResults               SendMessageResult);

    public delegate Task OnBinarySendMessageSentDelegate          (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Server,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinarySendMessage           BinarySendMessage,
                                                                   SentMessageResults               SendMessageResult);

    #endregion


    /// <summary>
    /// The OCPP adapter for sending outgoing messages.
    /// </remarks>
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Data

        private readonly INetworkingNode parentNetworkingNode;

        #endregion

        #region Events

        #region JSON   messages sent

        /// <summary>
        /// An event sent whenever a JSON request was sent.
        /// </summary>
        public event OnJSONRequestMessageSentDelegate?          OnJSONRequestMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response was sent.
        /// </summary>
        public event OnJSONResponseMessageSentDelegate?         OnJSONResponseMessageSent;

        /// <summary>
        /// An event sent whenever a JSON request error was sent.
        /// </summary>
        public event OnJSONRequestErrorMessageSentDelegate?     OnJSONRequestErrorMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response error was sent.
        /// </summary>
        public event OnJSONResponseErrorMessageSentDelegate?    OnJSONResponseErrorMessageSent;

        /// <summary>
        /// An event sent whenever a JSON send message was sent.
        /// </summary>
        public event OnJSONSendMessageSentDelegate?             OnJSONSendMessageSent;

        #endregion

        #region Binary messages sent

        /// <summary>
        /// An event sent whenever a binary request was sent.
        /// </summary>
        public event OnBinaryRequestMessageSentDelegate?        OnBinaryRequestMessageSent;

        /// <summary>
        /// An event sent whenever a binary response was sent.
        /// </summary>
        public event OnBinaryResponseMessageSentDelegate?       OnBinaryResponseMessageSent;

        /// <summary>
        /// An event sent whenever a JSON request error was sent.
        /// </summary>
        public event OnBinaryRequestErrorMessageSentDelegate?   OnBinaryRequestErrorMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response error was sent.
        /// </summary>
        public event OnBinaryResponseErrorMessageSentDelegate?  OnBinaryResponseErrorMessageSent;

        /// <summary>
        /// An event sent whenever a binary send message was sent.
        /// </summary>
        public event OnBinarySendMessageSentDelegate?           OnBinarySendMessageSent;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for sending outgoing messages.
        /// </summary>
        /// <param name="ParentNetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterOUT(INetworkingNode ParentNetworkingNode)
        {

            this.parentNetworkingNode = ParentNetworkingNode;

        }

        #endregion



        // Send requests/responses...

        #region SendJSONRequest          (JSONRequestMessage, SentAction = null)

        /// <summary>
        /// Send (and forget) the given JSON request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON request message.</param>
        /// <param name="SentAction">An action called after trying to send the request.</param>
        public async Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage     JSONRequestMessage,
                                                             Action<SentMessageResult>?  SentAction   = null)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONRequest(JSONRequestMessage);

            SentAction?.Invoke(sendMessageResult);

            await NotifyJSONRequestMessageSent(
                      JSONRequestMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendJSONRequestAndWait   (JSONRequestMessage, SentAction = null)

        /// <summary>
        /// Send the given JSON request message and wait for the result/timeout.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON request message.</param>
        /// <param name="SentAction">An action called after trying to send the request.</param>
        public async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage     JSONRequestMessage,
                                                                   Action<SentMessageResult>?  SentAction   = null)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendJSONRequestAndWait(JSONRequestMessage,
                                                                                          async sendMessageResult => {

                SentAction?.Invoke(sendMessageResult);

                await NotifyJSONRequestMessageSent(
                          JSONRequestMessage,
                          sendMessageResult
                      );

            });

            return sendRequestState;

        }

        #endregion

        #region SendJSONResponse         (JSONResponseMessage)

        public async Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONResponse(JSONResponseMessage);

            await NotifyJSONResponseMessageSent(
                      JSONResponseMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendJSONRequestError     (JSONRequestErrorMessage)

        public async Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONRequestError(JSONRequestErrorMessage);

            await NotifyJSONRequestErrorSent(
                      JSONRequestErrorMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendJSONResponseError    (JSONResponseErrorMessage)

        public async Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONResponseError(JSONResponseErrorMessage);

            await NotifyJSONResponseErrorSent(
                      JSONResponseErrorMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendJSONSendMessage      (JSONSendMessage)

        /// <summary>
        /// Send (and forget) the given JSON send message.
        /// </summary>
        /// <param name="JSONSendMessage">A JSON send message.</param>
        public async Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONSendMessage(JSONSendMessage);

            await NotifyJSONSendMessageSent(
                      JSONSendMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion


        #region SendBinaryRequest        (BinaryRequestMessage, SentAction = null)

        /// <summary>
        /// Send (and forget) the given binary request message.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary request message.</param>
        /// <param name="SentAction">An action called after trying to send the request.</param>
        public async Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage   BinaryRequestMessage,
                                                               Action<SentMessageResult>?  SentAction   = null)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

            SentAction?.Invoke(sendMessageResult);

            await NotifyBinaryRequestMessageSent(
                      BinaryRequestMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendBinaryRequestAndWait (BinaryRequestMessage, SentAction = null)

        /// <summary>
        /// Send the given binary request message and wait for the result/timeout.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary request message.</param>
        /// <param name="SentAction">An action called after trying to send the request.</param>
        public async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage   BinaryRequestMessage,
                                                                     Action<SentMessageResult>?  SentAction   = null)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendBinaryRequestAndWait(BinaryRequestMessage,
                                                                                            async sendMessageResult => {

                SentAction?.Invoke(sendMessageResult);

                await NotifyBinaryRequestMessageSent(
                          BinaryRequestMessage,
                          sendMessageResult
                      );

            });

            return sendRequestState;

        }

        #endregion

        #region SendBinaryResponse       (BinaryResponseMessage)

        public async Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

            await NotifyBinaryResponseMessageSent(
                      BinaryResponseMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendBinaryRequestError   (BinaryRequestErrorMessage)

        public async Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryRequestError(BinaryRequestErrorMessage);

            await NotifyBinaryRequestErrorSent(
                      BinaryRequestErrorMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendBinaryResponseError  (BinaryResponseErrorMessage)

        public async Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryResponseError(BinaryResponseErrorMessage);

            await NotifyBinaryResponseErrorSent(
                      BinaryResponseErrorMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion

        #region SendBinarySendMessage    (BinarySendMessage)

        /// <summary>
        /// Send (and forget) the given binary send message.
        /// </summary>
        /// <param name="BinarySendMessage">A binary send message.</param>
        public async Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinarySendMessage(BinarySendMessage);

            await NotifyBinarySendMessageSent(
                      BinarySendMessage,
                      sendMessageResult
                  );

            return sendMessageResult;

        }

        #endregion



        // Send request/response events...

        #region NotifyJSONRequestMessageSent    (JSONRequestMessage,         SendMessageResult)

        public Task NotifyJSONRequestMessageSent(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                 SentMessageResult        SendMessageResult)

            => LogEvent(
                   OnJSONRequestMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       JSONRequestMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyJSONResponseMessageSent   (JSONResponseMessage,        SendMessageResult)

        public Task NotifyJSONResponseMessageSent(OCPP_JSONResponseMessage  JSONResponseMessage,
                                                  SentMessageResult         SendMessageResult)

            => LogEvent(
                   OnJSONResponseMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       JSONResponseMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyJSONRequestErrorSent      (JSONRequestErrorMessage,    SendMessageResult)

        public Task NotifyJSONRequestErrorSent(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                               SentMessageResult             SendMessageResult)

            => LogEvent(
                   OnJSONRequestErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       JSONRequestErrorMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyJSONResponseErrorSent     (JSONResponseErrorMessage,   SendMessageResult)

        public Task NotifyJSONResponseErrorSent(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                SentMessageResult              SendMessageResult)

            => LogEvent(
                   OnJSONResponseErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       JSONResponseErrorMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyJSONSendMessageSent       (JSONResponseErrorMessage,   SendMessageResult)

        public Task NotifyJSONSendMessageSent(OCPP_JSONSendMessage  JSONSendMessage,
                                              SentMessageResult     SendMessageResult)

            => LogEvent(
                   OnJSONSendMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       JSONSendMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion


        #region NotifyBinaryRequestMessageSent  (BinaryRequestMessage,       SendMessageResult)

        public Task NotifyBinaryRequestMessageSent(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                   SentMessageResult          SendMessageResult)

            => LogEvent(
                   OnBinaryRequestMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       BinaryRequestMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyBinaryResponseMessageSent (BinaryResponseMessage,      SendMessageResult)


        public Task NotifyBinaryResponseMessageSent(OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                                    SentMessageResult           SendMessageResult)

            => LogEvent(
                   OnBinaryResponseMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       BinaryResponseMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyBinaryRequestErrorSent    (BinaryRequestErrorMessage,  SendMessageResult)

        public Task NotifyBinaryRequestErrorSent(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                 SentMessageResult               SendMessageResult)

            => LogEvent(
                   OnBinaryRequestErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       BinaryRequestErrorMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyBinaryResponseErrorSent   (BinaryResponseErrorMessage, SendMessageResult)

        public Task NotifyBinaryResponseErrorSent(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                  SentMessageResult                SendMessageResult)

            => LogEvent(
                   OnBinaryResponseErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       BinaryResponseErrorMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion

        #region NotifyBinarySendMessageSent     (BinaryResponseErrorMessage, SendMessageResult)

        public Task NotifyBinarySendMessageSent(OCPP_BinarySendMessage  BinarySendMessage,
                                                SentMessageResult       SendMessageResult)

            => LogEvent(
                   OnBinarySendMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SendMessageResult.Connection,
                       BinarySendMessage,
                       SendMessageResult.Result
                   )
               );

        #endregion



        #region (private) LogEvent(Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentNetworkingNode.LogEvent(nameof(OCPPWebSocketAdapterOUT), Logger, LogHandler, EventName, OCPPCommand);

        #endregion

        #region (private) HandleErrors(Caller, ExceptionOccured)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterOUT),
                   Caller,
                   ExceptionOccured
               );

        #endregion




        [Obsolete]
        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)

            => parentNetworkingNode.HandleErrors(
                   nameof(OCPPWebSocketAdapterOUT),
                   Caller,
                   ExceptionOccured
               );


    }

}
