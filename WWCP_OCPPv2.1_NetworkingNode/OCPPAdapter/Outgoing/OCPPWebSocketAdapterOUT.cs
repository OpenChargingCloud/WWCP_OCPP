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
using cloud.charging.open.protocols.OCPPv2_1.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for sending messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Data

        private readonly INetworkingNode parentNetworkingNode;

        #endregion

        #region Events

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON request was sent.
        /// </summary>
        public event OnJSONMessageRequestSentDelegate?          OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever a JSON response was sent.
        /// </summary>
        public event OnJSONMessageResponseSentDelegate?         OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever a JSON error response was sent.
        /// </summary>
        public event OnJSONRequestErrorMessageSentDelegate?            OnJSONErrorResponseSent;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary request was sent.
        /// </summary>
        public event OnBinaryMessageRequestSentDelegate?        OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever a binary response was sent.
        /// </summary>
        public event OnBinaryMessageResponseSentDelegate?       OnBinaryMessageResponseSent;

        ///// <summary>
        ///// An event sent whenever a binary error response was sent.
        ///// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?    OnBinaryErrorResponseSent;

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


        // Send requests...

        #region SendJSONRequest         (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="JSONRequestMessage">A OCPP JSON request.</param>
        public async Task<SendOCPPMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            #region OnJSONMessageRequestSent

            var logger = OnJSONMessageRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONMessageRequestSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONRequestMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONMessageRequestSent));
                }
            }

            #endregion

            return await parentNetworkingNode.OCPP.SendJSONRequest(JSONRequestMessage);

        }

        #endregion

        #region SendJSONRequestAndWait  (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="JSONRequestMessage">A OCPP JSON request.</param>
        public async Task<SendRequestState> SendJSONRequestAndWait(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            #region OnJSONMessageRequestSent

            var logger = OnJSONMessageRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONMessageRequestSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONRequestMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONMessageRequestSent));
                }
            }

            #endregion

            return await parentNetworkingNode.OCPP.SendJSONRequestAndWait(JSONRequestMessage);

        }

        #endregion


        #region SendBinaryRequest       (BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given Binary.
        /// </summary>
        /// <param name="BinaryRequestMessage">A OCPP Binary request.</param>
        public async Task<SendOCPPMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            #region OnBinaryMessageRequestSent

            var logger = OnBinaryMessageRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryMessageRequestSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryRequestMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryMessageRequestSent));
                }
            }

            #endregion

            return await parentNetworkingNode.OCPP.SendBinaryRequest(BinaryRequestMessage);

        }

        #endregion

        #region SendBinaryRequestAndWait(BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given Binary.
        /// </summary>
        /// <param name="BinaryRequestMessage">A OCPP Binary request.</param>
        public async Task<SendRequestState> SendBinaryRequestAndWait(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            #region OnBinaryMessageRequestSent

            var logger = OnBinaryMessageRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryMessageRequestSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryRequestMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryMessageRequestSent));
                }
            }

            #endregion

            return await parentNetworkingNode.OCPP.SendBinaryRequestAndWait(BinaryRequestMessage);

        }

        #endregion


        // Response events...

        #region NotifyJSONMessageResponseSent  (JSONResponseMessage)

        public async Task NotifyJSONMessageResponseSent(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            var logger = OnJSONMessageResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnJSONMessageResponseSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         JSONResponseMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONMessageRequestSent));
                }
            }

        }

        #endregion

        #region NotifyJSONErrorResponseSent    (JSONErrorMessage)

        public async Task NotifyJSONErrorResponseSent(OCPP_JSONRequestErrorMessage JSONErrorMessage)
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
                                                                         JSONErrorMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONMessageRequestSent));
                }
            }

        }

        #endregion


        #region NotifyBinaryMessageResponseSent(BinaryResponseMessage)


        public async Task NotifyBinaryMessageResponseSent(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            var logger = OnBinaryMessageResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnBinaryMessageResponseSentDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         this,
                                                                         BinaryResponseMessage
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryMessageRequestSent));
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
