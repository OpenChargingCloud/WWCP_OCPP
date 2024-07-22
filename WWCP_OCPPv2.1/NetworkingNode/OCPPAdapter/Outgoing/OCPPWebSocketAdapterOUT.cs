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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for sending outgoing messages.
    /// </remarks>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
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
        /// <param name="NetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterOUT(INetworkingNode NetworkingNode)
        {

            this.parentNetworkingNode = NetworkingNode;

        }

        #endregion


        // Send requests/responses...

        #region SendJSONRequest          (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON request message.</param>
        public async Task<SendMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONRequest(JSONRequestMessage);

            #region OnJSONRequestMessageSent

            var logger = OnJSONRequestMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONRequestMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONRequestMessage,
                                                                         sendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONRequestMessageSent));
                }
            }

            #endregion

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
                                                                   Action<SendMessageResult>?  SentAction   = null)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendJSONRequestAndWait(JSONRequestMessage,
                                                                                          async sendMessageResult => {

                SentAction?.Invoke(sendMessageResult);

                var logger = OnJSONRequestMessageSent;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType<OnJSONRequestMessageSentDelegate>().
                                               Select(loggingDelegate => loggingDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             this,
                                                                             JSONRequestMessage,
                                                                             sendMessageResult
                                                                         )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONRequestMessageSent));
                    }
                }

            });

            return sendRequestState;

        }

        #endregion

        #region SendJSONResponse         (JSONResponseMessage)

        public async Task<SendMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONResponse(JSONResponseMessage);

            #region Send OnJSONResponseMessageSent event

            var logger = OnJSONResponseMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                        OfType<OnJSONResponseMessageSentDelegate>().
                                        Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         //Connection,
                                                                                         JSONResponseMessage,
                                                                                         sendMessageResult)).
                                        ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                "NetworkingNode",
                                nameof(OnJSONResponseMessageSent),
                                e
                            );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendJSONRequestError     (JSONRequestErrorMessage)

        public async Task<SendMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            var sendMessageResult = SendMessageResult.TransmissionFailed;

            #region Send OnJSONRequestErrorMessageSent event

            var logger = OnJSONRequestErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnJSONRequestErrorMessageSentDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        //Connection,
                                                                                        JSONRequestErrorMessage,
                                                                                        sendMessageResult)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnJSONRequestErrorMessageSent),
                              e
                          );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendJSONResponseError    (JSONResponseErrorMessage)

        public async Task<SendMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            var sendMessageResult = SendMessageResult.TransmissionFailed;

            #region Send OnJSONResponseErrorMessageSent event

            var logger = OnJSONResponseErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnJSONResponseErrorMessageSentDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        //Connection,
                                                                                        JSONResponseErrorMessage,
                                                                                        sendMessageResult)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnJSONResponseErrorMessageSent),
                              e
                          );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendJSONSendMessage      (JSONSendMessage)

        /// <summary>
        /// Send (and forget) the given JSON send message.
        /// </summary>
        /// <param name="JSONSendMessage">A JSON send message.</param>
        public async Task<SendMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONSendMessage(JSONSendMessage);

            #region OnJSONSendMessageSent

            var logger = OnJSONSendMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONSendMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONSendMessage,
                                                                         sendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONSendMessageSent));
                }
            }

            #endregion

            return sendMessageResult;

        }

        #endregion


        #region SendBinaryRequest        (BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given binary request message.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary request message.</param>
        public async Task<SendMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

            #region OnBinaryRequestMessageSent

            var logger = OnBinaryRequestMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryRequestMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryRequestMessage,
                                                                         sendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryRequestMessageSent));
                }
            }

            #endregion

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
                                                                     Action<SendMessageResult>?  SentAction   = null)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendBinaryRequestAndWait(BinaryRequestMessage,
                                                                                            async sendMessageResult => {

                SentAction?.Invoke(sendMessageResult);

                var logger = OnBinaryRequestMessageSent;
                if (logger is not null)
                {
                    try
                    {

                        await Task.WhenAll(logger.GetInvocationList().
                                               OfType<OnBinaryRequestMessageSentDelegate>().
                                               Select(loggingDelegate => loggingDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             this,
                                                                             BinaryRequestMessage,
                                                                             sendMessageResult
                                                                         )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryRequestMessageSent));
                    }
                }

            });

            return sendRequestState;

        }

        #endregion

        #region SendBinaryResponse       (BinaryResponseMessage)

        public async Task<SendMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryResponse(BinaryResponseMessage);

            #region Send OnBinaryResponseMessageSent event

            var logger = OnBinaryResponseMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                        OfType<OnBinaryResponseMessageSentDelegate>().
                                        Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         //Connection,
                                                                                         BinaryResponseMessage,
                                                                                         sendMessageResult)).
                                        ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                "NetworkingNode",
                                nameof(OnBinaryResponseMessageSent),
                                e
                            );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendBinaryRequestError   (BinaryRequestErrorMessage)

        public async Task<SendMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            var sendMessageResult = SendMessageResult.TransmissionFailed;

            #region Send OnBinaryRequestErrorMessageSent event

            var logger = OnBinaryRequestErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnBinaryRequestErrorMessageSentDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        //Connection,
                                                                                        BinaryRequestErrorMessage,
                                                                                        sendMessageResult)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnBinaryRequestErrorMessageSent),
                              e
                          );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendBinaryResponseError  (BinaryResponseErrorMessage)

        public async Task<SendMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            var sendMessageResult = SendMessageResult.TransmissionFailed;

            #region Send OnBinaryResponseErrorMessageSent event

            var logger = OnBinaryResponseErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnBinaryResponseErrorMessageSentDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        //Connection,
                                                                                        BinaryResponseErrorMessage,
                                                                                        sendMessageResult)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnBinaryResponseErrorMessageSent),
                              e
                          );
                }

            }

            #endregion

            return sendMessageResult;

        }

        #endregion

        #region SendBinarySendMessage    (BinarySendMessage)

        /// <summary>
        /// Send (and forget) the given binary send message.
        /// </summary>
        /// <param name="BinarySendMessage">A binary send message.</param>
        public async Task<SendMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinarySendMessage(BinarySendMessage);

            #region OnBinarySendMessageSent

            var logger = OnBinarySendMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinarySendMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinarySendMessage,
                                                                         sendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinarySendMessageSent));
                }
            }

            #endregion

            return sendMessageResult;

        }

        #endregion



        // Request/response events...

        #region NotifyJSONMessageResponseSent   (JSONResponseMessage,        SendMessageResult)

        public async Task NotifyJSONMessageResponseSent(OCPP_JSONResponseMessage  JSONResponseMessage,
                                                        SendMessageResult         SendMessageResult)
        {

            var logger = OnJSONResponseMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONResponseMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONResponseMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONRequestMessageSent));
                }
            }

        }

        #endregion

        #region NotifyJSONRequestErrorSent      (JSONRequestErrorMessage,    SendMessageResult)

        public async Task NotifyJSONRequestErrorSent(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                     SendMessageResult             SendMessageResult)
        {

            var logger = OnJSONRequestErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONRequestErrorMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONRequestErrorMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONRequestErrorMessageSent));
                }
            }

        }

        #endregion

        #region NotifyJSONResponseErrorSent     (JSONResponseErrorMessage,   SendMessageResult)

        public async Task NotifyJSONResponseErrorSent(OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage,
                                                      SendMessageResult              SendMessageResult)
        {

            var logger = OnJSONResponseErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONResponseErrorMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONResponseErrorMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONResponseErrorMessageSent));
                }
            }

        }

        #endregion

        #region NotifyJSONSendMessageSent       (JSONResponseErrorMessage,   SendMessageResult)

        public async Task NotifyJSONSendMessageSent(OCPP_JSONSendMessage  JSONSendMessage,
                                                    SendMessageResult     SendMessageResult)
        {

            var logger = OnJSONSendMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONSendMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONSendMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONSendMessageSent));
                }
            }

        }

        #endregion


        #region NotifyBinaryMessageResponseSent (BinaryResponseMessage,      SendMessageResult)


        public async Task NotifyBinaryMessageResponseSent(OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                                          SendMessageResult       SendMessageResult)
        {

            var logger = OnBinaryResponseMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryResponseMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryResponseMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryRequestMessageSent));
                }
            }

        }

        #endregion

        #region NotifyBinaryRequestErrorSent    (BinaryRequestErrorMessage,  SendMessageResult)

        public async Task NotifyBinaryRequestErrorSent(OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                       SendMessageResult               SendMessageResult)
        {

            var logger = OnBinaryRequestErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryRequestErrorMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryRequestErrorMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryRequestErrorMessageSent));
                }
            }

        }

        #endregion

        #region NotifyBinaryResponseErrorSent   (BinaryResponseErrorMessage, SendMessageResult)

        public async Task NotifyBinaryResponseErrorSent(OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage,
                                                        SendMessageResult                SendMessageResult)
        {

            var logger = OnBinaryResponseErrorMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryResponseErrorMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryResponseErrorMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryResponseErrorMessageSent));
                }
            }

        }

        #endregion

        #region NotifyBinarySendMessageSent     (BinaryResponseErrorMessage, SendMessageResult)

        public async Task NotifyBinarySendMessageSent(OCPP_BinarySendMessage  BinarySendMessage,
                                                      SendMessageResult       SendMessageResult)
        {

            var logger = OnBinarySendMessageSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinarySendMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinarySendMessage,
                                                                         SendMessageResult
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinarySendMessageSent));
                }
            }

        }

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccured)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
