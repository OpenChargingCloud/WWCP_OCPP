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
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONRequestMessage          JSONRequestMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseMessageSentDelegate        (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONResponseMessage         JSONResponseMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnJSONRequestErrorMessageSentDelegate    (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnJSONResponseErrorMessageSentDelegate   (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONResponseErrorMessage    JSONRequestErrorMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnJSONSendMessageSentDelegate            (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_JSONSendMessage             JSONSendMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);



    public delegate Task OnBinaryRequestMessageSentDelegate       (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryRequestMessage        BinaryRequestMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseMessageSentDelegate      (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryResponseMessage       BinaryResponseMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnBinaryRequestErrorMessageSentDelegate  (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnBinaryResponseErrorMessageSentDelegate (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinaryResponseErrorMessage  BinaryRequestErrorMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

    public delegate Task OnBinarySendMessageSentDelegate          (DateTime                         Timestamp,
                                                                   OCPPWebSocketAdapterOUT          Sender,
                                                                   IWebSocketConnection?            WebSocketConnection,
                                                                   OCPP_BinarySendMessage           BinarySendMessage,
                                                                   SentMessageResults               SentMessageResult,
                                                                   CancellationToken                CancellationToken);

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

        #region NotifyJSONRequestMessageSent    (JSONRequestMessage,         SentMessageResult)

        public Task NotifyJSONRequestMessageSent(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                 SentMessageResult        SentMessageResult)

            => LogEvent(
                   OnJSONRequestMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       JSONRequestMessage,
                       SentMessageResult.Result,
                       JSONRequestMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyJSONResponseMessageSent   (JSONResponseMessage,        SentMessageResult)

        public Task NotifyJSONResponseMessageSent(OCPP_JSONResponseMessage  JSONResponseMessage,
                                                  SentMessageResult         SentMessageResult)

            => LogEvent(
                   OnJSONResponseMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       JSONResponseMessage,
                       SentMessageResult.Result,
                       JSONResponseMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyJSONRequestErrorSent      (JSONRequestErrorMessage,    SentMessageResult)

        public Task NotifyJSONRequestErrorSent(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                               SentMessageResult             SentMessageResult)

            => LogEvent(
                   OnJSONRequestErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       JSONRequestErrorMessage,
                       SentMessageResult.Result,
                       JSONRequestErrorMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyJSONResponseErrorSent     (JSONResponseErrorMessage,   SentMessageResult)

        public Task NotifyJSONResponseErrorSent(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                SentMessageResult              SentMessageResult)

            => LogEvent(
                   OnJSONResponseErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       JSONResponseErrorMessage,
                       SentMessageResult.Result,
                       JSONResponseErrorMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyJSONSendMessageSent       (JSONResponseErrorMessage,   SentMessageResult)

        public Task NotifyJSONSendMessageSent(OCPP_JSONSendMessage  JSONSendMessage,
                                              SentMessageResult     SentMessageResult)

            => LogEvent(
                   OnJSONSendMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       JSONSendMessage,
                       SentMessageResult.Result,
                       JSONSendMessage.CancellationToken
                   )
               );

        #endregion


        #region NotifyBinaryRequestMessageSent  (BinaryRequestMessage,       SentMessageResult)

        public Task NotifyBinaryRequestMessageSent(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                                   SentMessageResult          SentMessageResult)

            => LogEvent(
                   OnBinaryRequestMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       BinaryRequestMessage,
                       SentMessageResult.Result,
                       BinaryRequestMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyBinaryResponseMessageSent (BinaryResponseMessage,      SentMessageResult)


        public Task NotifyBinaryResponseMessageSent(OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                                    SentMessageResult           SentMessageResult)

            => LogEvent(
                   OnBinaryResponseMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       BinaryResponseMessage,
                       SentMessageResult.Result,
                       BinaryResponseMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyBinaryRequestErrorSent    (BinaryRequestErrorMessage,  SentMessageResult)

        public Task NotifyBinaryRequestErrorSent(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                 SentMessageResult               SentMessageResult)

            => LogEvent(
                   OnBinaryRequestErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       BinaryRequestErrorMessage,
                       SentMessageResult.Result,
                       BinaryRequestErrorMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyBinaryResponseErrorSent   (BinaryResponseErrorMessage, SentMessageResult)

        public Task NotifyBinaryResponseErrorSent(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                  SentMessageResult                SentMessageResult)

            => LogEvent(
                   OnBinaryResponseErrorMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       BinaryResponseErrorMessage,
                       SentMessageResult.Result,
                       BinaryResponseErrorMessage.CancellationToken
                   )
               );

        #endregion

        #region NotifyBinarySendMessageSent     (BinaryResponseErrorMessage, SentMessageResult)

        public Task NotifyBinarySendMessageSent(OCPP_BinarySendMessage  BinarySendMessage,
                                                SentMessageResult       SentMessageResult)

            => LogEvent(
                   OnBinarySendMessageSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp.Now,
                       this,
                       SentMessageResult.Connection,
                       BinarySendMessage,
                       SentMessageResult.Result,
                       BinarySendMessage.CancellationToken
                   )
               );

        #endregion



        #region (private) LogEvent(Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentNetworkingNode.LogEvent(
                   nameof(OCPPWebSocketAdapterOUT),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPPCommand
               );

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


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => parentNetworkingNode.Id.ToString();

        #endregion

    }

}
