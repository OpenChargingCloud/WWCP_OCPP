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

using Newtonsoft.Json.Linq;

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
    /// Serves OCPP to networking nodes over the SSH subsystem "ocpp@open.charging.cloud" - as
    /// <see cref="OCPPWebSocketServer"/> does over HTTP WebSockets. A charging station is who signed in.
    /// </summary>
    public class OCPPSshServer : WWCPSshServer,
                                 IOCPPMessageTransport
    {

        #region Data

        /// <summary>
        /// The subsystem of OCPP.
        /// </summary>
        public const String OCPPSubsystem = "ocpp@open.charging.cloud";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A server of OCPP over an SSH subsystem.
        /// </summary>
        /// <param name="NetworkingNode">The networking node serving.</param>
        /// <param name="Subsystem">The subsystem served.</param>
        /// <param name="Subprotocols">The OCPP versions served, e.g. "ocpp2.1".</param>
        /// <param name="MaxMessageSize">The largest message taken and sent.</param>
        /// <param name="PingEvery">Every how long a PING is sent on every connection; null for none.</param>
        public OCPPSshServer(INetworkingNode       NetworkingNode,
                             String                Subsystem        = OCPPSubsystem,
                             IEnumerable<String>?  Subprotocols     = null,
                             UInt64?               MaxMessageSize   = null,
                             TimeSpan?             PingEvery        = null)

            : base(NetworkingNode,
                   Subsystem,
                   Subprotocols,
                   MaxMessageSize,
                   PingEvery)

        { }

        #endregion


        #region (private) SendJSON  (Destination, Build, MessageTimestamp, EventTrackingId, CancellationToken)

        /// <summary>
        /// Send a JSON message to the given networking node over the first of its connections that takes it -
        /// written for the networking mode of that connection.
        /// </summary>
        private async Task<SentMessageResult> SendJSON(NetworkingNode_Id             Destination,
                                                       Func<NetworkingMode, JArray>  Build,
                                                       DateTimeOffset                MessageTimestamp,
                                                       EventTracking_Id              EventTrackingId,
                                                       CancellationToken             CancellationToken)
        {

            try
            {

                foreach (var connection in GetConnectionsFor(Destination))
                {

                    var sentMessageResult = await SendJSONMessage(
                                                      connection,
                                                      Build(NetworkingModeOf(connection)),
                                                      MessageTimestamp,
                                                      EventTrackingId,
                                                      CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region (private) SendBinary(Destination, Build, MessageTimestamp, EventTrackingId, CancellationToken)

        /// <summary>
        /// Send a binary message to the given networking node over the first of its connections that takes
        /// it - written for the networking mode of that connection.
        /// </summary>
        private async Task<SentMessageResult> SendBinary(NetworkingNode_Id             Destination,
                                                         Func<NetworkingMode, Byte[]>  Build,
                                                         DateTimeOffset                MessageTimestamp,
                                                         EventTracking_Id              EventTrackingId,
                                                         CancellationToken             CancellationToken)
        {

            try
            {

                foreach (var connection in GetConnectionsFor(Destination))
                {

                    var sentMessageResult = await SendBinaryMessage(
                                                      connection,
                                                      Build(NetworkingModeOf(connection)),
                                                      MessageTimestamp,
                                                      EventTrackingId,
                                                      CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region (private static) NetworkingModeOf(Connection)

        private static NetworkingMode NetworkingModeOf(SshMessageConnection Connection)

            => Connection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode) ?? NetworkingMode.Standard;

        #endregion


        #region Send JSON   messages

        public Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)

            => SendJSON(JSONRequestMessage.Destination.Next,
                        networkingMode => {
                            JSONRequestMessage.NetworkingMode = networkingMode;
                            return JSONRequestMessage.ToJSON();
                        },
                        JSONRequestMessage.RequestTimestamp,
                        JSONRequestMessage.EventTrackingId,
                        JSONRequestMessage.CancellationToken);

        public Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)

            => SendJSON(JSONResponseMessage.Destination.Next,
                        networkingMode => {
                            JSONResponseMessage.NetworkingMode = networkingMode;
                            return JSONResponseMessage.ToJSON();
                        },
                        JSONResponseMessage.ResponseTimestamp,
                        JSONResponseMessage.EventTrackingId,
                        JSONResponseMessage.CancellationToken);

        public Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)

            => SendJSON(JSONRequestErrorMessage.Destination.Next,
                        networkingMode => {
                            JSONRequestErrorMessage.NetworkingMode = networkingMode;
                            return JSONRequestErrorMessage.ToJSON();
                        },
                        JSONRequestErrorMessage.ResponseTimestamp,
                        JSONRequestErrorMessage.EventTrackingId,
                        JSONRequestErrorMessage.CancellationToken);

        public Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)

            => SendJSON(JSONResponseErrorMessage.Destination.Next,
                        networkingMode => {
                            JSONResponseErrorMessage.NetworkingMode = networkingMode;
                            return JSONResponseErrorMessage.ToJSON();
                        },
                        JSONResponseErrorMessage.ResponseTimestamp,
                        JSONResponseErrorMessage.EventTrackingId,
                        JSONResponseErrorMessage.CancellationToken);

        public Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)

            => SendJSON(JSONSendMessage.Destination.Next,
                        networkingMode => {
                            JSONSendMessage.NetworkingMode = networkingMode;
                            return JSONSendMessage.ToJSON();
                        },
                        JSONSendMessage.MessageTimestamp,
                        JSONSendMessage.EventTrackingId,
                        JSONSendMessage.CancellationToken);

        #endregion

        #region Send binary messages

        public Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)

            => SendBinary(BinaryRequestMessage.Destination.Next,
                          networkingMode => {
                              BinaryRequestMessage.NetworkingMode = networkingMode;
                              return BinaryRequestMessage.ToByteArray();
                          },
                          BinaryRequestMessage.RequestTimestamp,
                          BinaryRequestMessage.EventTrackingId,
                          BinaryRequestMessage.CancellationToken);

        public Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)

            => SendBinary(BinaryResponseMessage.Destination.Next,
                          networkingMode => {
                              BinaryResponseMessage.NetworkingMode = networkingMode;
                              return BinaryResponseMessage.ToByteArray();
                          },
                          BinaryResponseMessage.ResponseTimestamp,
                          BinaryResponseMessage.EventTrackingId,
                          BinaryResponseMessage.CancellationToken);

        public Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)

            => SendBinary(BinaryRequestErrorMessage.Destination.Next,
                          networkingMode => {
                              BinaryRequestErrorMessage.NetworkingMode = networkingMode;
                              return BinaryRequestErrorMessage.ToByteArray();
                          },
                          BinaryRequestErrorMessage.ResponseTimestamp,
                          BinaryRequestErrorMessage.EventTrackingId,
                          BinaryRequestErrorMessage.CancellationToken);

        public Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)

            => SendBinary(BinaryResponseErrorMessage.Destination.Next,
                          networkingMode => {
                              BinaryResponseErrorMessage.NetworkingMode = networkingMode;
                              return BinaryResponseErrorMessage.ToByteArray();
                          },
                          BinaryResponseErrorMessage.ResponseTimestamp,
                          BinaryResponseErrorMessage.EventTrackingId,
                          BinaryResponseErrorMessage.CancellationToken);

        public Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)

            => SendBinary(BinarySendMessage.Destination.Next,
                          networkingMode => {
                              BinarySendMessage.NetworkingMode = networkingMode;
                              return BinarySendMessage.ToByteArray();
                          },
                          BinarySendMessage.MessageTimestamp,
                          BinarySendMessage.EventTrackingId,
                          BinarySendMessage.CancellationToken);

        #endregion

        #region Send CBOR messages

        /// <summary>
        /// Send a CBOR message to the given networking node over the first of its connections
        /// that agreed on a CBOR subprotocol and takes it - written for the networking mode of
        /// that connection. A connection that speaks JSON is no way for a CBOR message.
        /// </summary>
        private async Task<SentMessageResult> SendCBOR(NetworkingNode_Id             Destination,
                                                       Func<NetworkingMode, Byte[]>  Build,
                                                       DateTimeOffset                MessageTimestamp,
                                                       EventTracking_Id              EventTrackingId,
                                                       CancellationToken             CancellationToken)
        {

            try
            {

                foreach (var connection in GetConnectionsFor(Destination))
                {

                    if (!connection.SpeaksCBOR())
                        continue;

                    var sentMessageResult = await SendBinaryMessage(
                                                      connection,
                                                      Build(NetworkingModeOf(connection)),
                                                      MessageTimestamp,
                                                      EventTrackingId,
                                                      CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        /// <summary>
        /// Send (and forget) the given CBOR OCPP request message.
        /// </summary>
        /// <param name="CBORRequestMessage">A CBOR OCPP request message.</param>
        public Task<SentMessageResult> SendCBORRequest(OCPP_CBORRequestMessage CBORRequestMessage)

            => SendCBOR(CBORRequestMessage.Destination.Next,
                        networkingMode => {
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

            => SendCBOR(CBORResponseMessage.Destination.Next,
                        networkingMode => {
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

            => SendCBOR(CBORRequestErrorMessage.Destination.Next,
                        networkingMode => {
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

            => SendCBOR(CBORResponseErrorMessage.Destination.Next,
                        networkingMode => {
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

            => SendCBOR(CBORSendMessage.Destination.Next,
                        networkingMode => {
                            CBORSendMessage.NetworkingMode = networkingMode;
                            return CBORSendMessage.ToByteArray();
                        },
                        CBORSendMessage.MessageTimestamp,
                        CBORSendMessage.EventTrackingId,
                        CBORSendMessage.CancellationToken);

        #endregion

    }

}
