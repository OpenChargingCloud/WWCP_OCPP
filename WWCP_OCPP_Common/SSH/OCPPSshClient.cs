/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.SSH;

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.SSH
{

    /// <summary>
    /// A connection of a networking node - a charging station, a local controller - to another one over the
    /// SSH subsystem "ocpp@open.charging.cloud", as <see cref="OCPPWebSocketClient"/> is over an HTTP
    /// WebSocket.
    /// </summary>
    /// <param name="NetworkingNode">The networking node connecting.</param>
    /// <param name="Options">Where and how to connect: host, port, login, subsystem, OCPP versions, reconnects.</param>
    /// <param name="NetworkingMode">Whether messages carry their network path; standard by default.</param>
    /// <param name="ConnectAs">The networking node to connect as, where that is not the SSH login.</param>
    /// <param name="Description">An optional description.</param>
    public class OCPPSshClient(INetworkingNode          NetworkingNode,
                               SshMessageClientOptions  Options,
                               NetworkingMode?          NetworkingMode   = null,
                               NetworkingNode_Id?       ConnectAs        = null,
                               I18NString?              Description      = null)

        : WWCPSshClient(NetworkingNode,
                        Options,
                        NetworkingMode,
                        ConnectAs,
                        Description),

          IOCPPMessageTransport

    {

        #region Send JSON   messages

        public Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {
            JSONRequestMessage.NetworkingMode = this.NetworkingMode;
            return SendJSONMessage(JSONRequestMessage.ToJSON(), JSONRequestMessage.RequestTimestamp, JSONRequestMessage.EventTrackingId, JSONRequestMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {
            JSONResponseMessage.NetworkingMode = this.NetworkingMode;
            return SendJSONMessage(JSONResponseMessage.ToJSON(), JSONResponseMessage.ResponseTimestamp, JSONResponseMessage.EventTrackingId, JSONResponseMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {
            JSONRequestErrorMessage.NetworkingMode = this.NetworkingMode;
            return SendJSONMessage(JSONRequestErrorMessage.ToJSON(), JSONRequestErrorMessage.ResponseTimestamp, JSONRequestErrorMessage.EventTrackingId, JSONRequestErrorMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {
            JSONResponseErrorMessage.NetworkingMode = this.NetworkingMode;
            return SendJSONMessage(JSONResponseErrorMessage.ToJSON(), JSONResponseErrorMessage.ResponseTimestamp, JSONResponseErrorMessage.EventTrackingId, JSONResponseErrorMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)
        {
            JSONSendMessage.NetworkingMode = this.NetworkingMode;
            return SendJSONMessage(JSONSendMessage.ToJSON(), JSONSendMessage.MessageTimestamp, JSONSendMessage.EventTrackingId, JSONSendMessage.CancellationToken);
        }

        #endregion

        #region Send binary messages

        public Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {
            BinaryRequestMessage.NetworkingMode = this.NetworkingMode;
            return SendBinaryMessage(BinaryRequestMessage.ToByteArray(), BinaryRequestMessage.RequestTimestamp, BinaryRequestMessage.EventTrackingId, BinaryRequestMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {
            BinaryResponseMessage.NetworkingMode = this.NetworkingMode;
            return SendBinaryMessage(BinaryResponseMessage.ToByteArray(), BinaryResponseMessage.ResponseTimestamp, BinaryResponseMessage.EventTrackingId, BinaryResponseMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {
            BinaryRequestErrorMessage.NetworkingMode = this.NetworkingMode;
            return SendBinaryMessage(BinaryRequestErrorMessage.ToByteArray(), BinaryRequestErrorMessage.ResponseTimestamp, BinaryRequestErrorMessage.EventTrackingId, BinaryRequestErrorMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {
            BinaryResponseErrorMessage.NetworkingMode = this.NetworkingMode;
            return SendBinaryMessage(BinaryResponseErrorMessage.ToByteArray(), BinaryResponseErrorMessage.ResponseTimestamp, BinaryResponseErrorMessage.EventTrackingId, BinaryResponseErrorMessage.CancellationToken);
        }

        public Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)
        {
            BinarySendMessage.NetworkingMode = this.NetworkingMode;
            return SendBinaryMessage(BinarySendMessage.ToByteArray(), BinarySendMessage.MessageTimestamp, BinarySendMessage.EventTrackingId, BinarySendMessage.CancellationToken);
        }

        #endregion

        #region Send CBOR messages

        /// <summary>
        /// Send a CBOR message over the connection of this client - where it agreed on a
        /// CBOR subprotocol. A connection that speaks JSON is no way for a CBOR message.
        /// </summary>
        private Task<SentMessageResult> SendCBOR(Func<NetworkingMode, Byte[]>  Build,
                                                 DateTimeOffset                MessageTimestamp,
                                                 EventTracking_Id              EventTrackingId,
                                                 CancellationToken             CancellationToken)

            => Connection.SpeaksCBOR()

                   ? SendBinaryMessage(
                         Build(this.NetworkingMode),
                         MessageTimestamp,
                         EventTrackingId,
                         CancellationToken
                     )

                   : Task.FromResult(
                         SentMessageResult.TransmissionFailed(
                             new InvalidOperationException($"The connection agreed on '{Connection?.Subprotocol ?? "no subprotocol"}', which is not CBOR!")
                         )
                     );

        /// <summary>
        /// Send (and forget) the given CBOR OCPP request message.
        /// </summary>
        /// <param name="CBORRequestMessage">A CBOR OCPP request message.</param>
        public Task<SentMessageResult> SendCBORRequest(OCPP_CBORRequestMessage CBORRequestMessage)

            => SendCBOR(networkingMode => {
                            CBORRequestMessage.NetworkingMode = networkingMode;
                            return CBORRequestMessage.ToByteArray();
                        },
                        CBORRequestMessage.RequestTimestamp,
                        CBORRequestMessage.EventTrackingId,
                        CBORRequestMessage.CancellationToken);

        /// <summary>
        /// Send (and forget) the given CBOR OCPP response message.
        /// </summary>
        /// <param name="CBORResponseMessage">A CBOR OCPP response message.</param>
        public Task<SentMessageResult> SendCBORResponse(OCPP_CBORResponseMessage CBORResponseMessage)

            => SendCBOR(networkingMode => {
                            CBORResponseMessage.NetworkingMode = networkingMode;
                            return CBORResponseMessage.ToByteArray();
                        },
                        CBORResponseMessage.ResponseTimestamp,
                        CBORResponseMessage.EventTrackingId,
                        CBORResponseMessage.CancellationToken);

        /// <summary>
        /// Send (and forget) the given CBOR OCPP request error message.
        /// </summary>
        /// <param name="CBORRequestErrorMessage">A CBOR OCPP request error message.</param>
        public Task<SentMessageResult> SendCBORRequestError(OCPP_CBORRequestErrorMessage CBORRequestErrorMessage)

            => SendCBOR(networkingMode => {
                            CBORRequestErrorMessage.NetworkingMode = networkingMode;
                            return CBORRequestErrorMessage.ToByteArray();
                        },
                        CBORRequestErrorMessage.ResponseTimestamp,
                        CBORRequestErrorMessage.EventTrackingId,
                        CBORRequestErrorMessage.CancellationToken);

        /// <summary>
        /// Send (and forget) the given CBOR OCPP response error message.
        /// </summary>
        /// <param name="CBORResponseErrorMessage">A CBOR OCPP response error message.</param>
        public Task<SentMessageResult> SendCBORResponseError(OCPP_CBORResponseErrorMessage CBORResponseErrorMessage)

            => SendCBOR(networkingMode => {
                            CBORResponseErrorMessage.NetworkingMode = networkingMode;
                            return CBORResponseErrorMessage.ToByteArray();
                        },
                        CBORResponseErrorMessage.ResponseTimestamp,
                        CBORResponseErrorMessage.EventTrackingId,
                        CBORResponseErrorMessage.CancellationToken);

        /// <summary>
        /// Send (and forget) the given CBOR OCPP send message.
        /// </summary>
        /// <param name="CBORSendMessage">A CBOR OCPP send message.</param>
        public Task<SentMessageResult> SendCBORSendMessage(OCPP_CBORSendMessage CBORSendMessage)

            => SendCBOR(networkingMode => {
                            CBORSendMessage.NetworkingMode = networkingMode;
                            return CBORSendMessage.ToByteArray();
                        },
                        CBORSendMessage.MessageTimestamp,
                        CBORSendMessage.EventTrackingId,
                        CBORSendMessage.CancellationToken);

        #endregion

    }

}
