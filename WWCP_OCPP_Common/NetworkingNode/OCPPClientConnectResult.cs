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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

    /// <summary>
    /// What came of <see cref="AOCPPNetworkingNode.ConnectOCPPClient"/>: over an HTTP WebSocket, the HTTP
    /// answer; over SSH, the connection or why there is none.
    /// </summary>
    /// <param name="RemoteURL">Where the client connects to.</param>
    /// <param name="Transport">An HTTP WebSocket, or an SSH channel.</param>
    /// <param name="Client">The client - which, with a reconnect policy, goes on trying where this attempt failed.</param>
    /// <param name="Connection">The connection made, if any.</param>
    /// <param name="HTTPResponse">Over an HTTP WebSocket: the answer to the upgrade - 101 where it was made.</param>
    /// <param name="Error">Over SSH: why the connection was not made - a refusal, a login the server did not take, a server not reached.</param>
    public sealed record OCPPClientConnectResult(URL                  RemoteURL,
                                                 MessageTransport     Transport,
                                                 IWWCPClient?         Client,
                                                 IMessageConnection?  Connection,
                                                 HTTPResponse?        HTTPResponse,
                                                 Exception?           Error)
    {

        /// <summary>
        /// Whether the connection was made.
        /// </summary>
        public Boolean IsConnected

            => Transport == MessageTransport.WebSocket
                   ? HTTPResponse?.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols
                   : Connection is not null && Error is null;

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IsConnected
                   ? $"Connected to {RemoteURL}"
                   : $"Not connected to {RemoteURL}: {(Transport == MessageTransport.WebSocket ? HTTPResponse?.HTTPStatusCode.ToString() : Error?.Message)}";

    }

}
