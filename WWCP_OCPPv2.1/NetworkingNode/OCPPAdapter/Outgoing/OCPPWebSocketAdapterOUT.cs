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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for sending messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Data

        private readonly NN.IBaseNetworkingNode parentNetworkingNode;

        #endregion

        #region Events

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON request was sent.
        /// </summary>
        public event OnJSONRequestMessageSentDelegate?        OnJSONRequestMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response was sent.
        /// </summary>
        public event OnJSONResponseMessageSentDelegate?       OnJSONResponseMessageSent;

        /// <summary>
        /// An event sent whenever a JSON error response was sent.
        /// </summary>
        public event OnJSONRequestErrorMessageSentDelegate?   OnJSONErrorResponseSent;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary request was sent.
        /// </summary>
        public event OnBinaryRequestMessageSentDelegate?       OnBinaryRequestMessageSent;

        /// <summary>
        /// An event sent whenever a binary response was sent.
        /// </summary>
        public event OnBinaryResponseMessageSentDelegate?      OnBinaryResponseMessageSent;

        ///// <summary>
        ///// An event sent whenever a binary error response was sent.
        ///// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?   OnBinaryErrorResponseSent;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP adapter for sending outgoing messages.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        public OCPPWebSocketAdapterOUT(NN.IBaseNetworkingNode NetworkingNode)
        {

            this.parentNetworkingNode = NetworkingNode;

        }

        #endregion


        // Send requests...

        #region SendJSONRequest          (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON request message.</param>
        public async Task<SendMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendJSONRequest(JSONRequestMessage);

            #region OnJSONMessageRequestSent

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

        #region SendJSONRequestAndWait   (JSONRequestMessage)

        /// <summary>
        /// Send the given JSON request message and wait for the result/timeout.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON request message.</param>
        public async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendJSONRequestAndWait(JSONRequestMessage,
                                                                                          async sendMessageResult => {

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


        #region SendBinaryRequest        (BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given binary request message.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary request message.</param>
        public async Task<SendMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var sendMessageResult = await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

            #region OnBinaryMessageRequestSent

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

        #region SendBinaryRequestAndWait (BinaryRequestMessage)

        /// <summary>
        /// Send the given binary request message and wait for the result/timeout.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary request message.</param>
        public async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var sendRequestState = await parentNetworkingNode.OCPP.SendBinaryRequestAndWait(BinaryRequestMessage,
                                                                                            async sendMessageResult => {

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


        // Response events...

        #region NotifyJSONMessageResponseSent   (JSONResponseMessage,   SendMessageResult)

        public async Task NotifyJSONMessageResponseSent(OCPP_JSONResponseMessage  JSONResponseMessage,
                                                        SendMessageResult     SendMessageResult)
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

        #region NotifyJSONErrorResponseSent     (JSONErrorMessage,      SendMessageResult)

        public async Task NotifyJSONErrorResponseSent(OCPP_JSONRequestErrorMessage  JSONErrorMessage,
                                                      SendMessageResult         SendMessageResult)
        {

            var logger = OnJSONErrorResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONRequestErrorMessageSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONErrorMessage,
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

        #region NotifyBinaryMessageResponseSent (BinaryResponseMessage, SendMessageResult)


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
