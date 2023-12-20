/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPP.CSMS
{

    /// <summary>
    /// The common interface of all incoming OCPP messages at a charging station management system.
    /// </summary>
    public interface ICSMSIncomingMessages
    {

        #region HTTP Web Socket connection management

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        event OnCSMSNewWebSocketConnectionDelegate?    OnCSMSNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        event OnCSMSCloseMessageReceivedDelegate?      OnCSMSCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        event OnCSMSTCPConnectionClosedDelegate?       OnCSMSTCPConnectionClosed;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever an incoming BinaryDataTransfer request was received.
        /// </summary>
        event OnIncomingBinaryDataTransferDelegate    OnIncomingBinaryDataTransfer;

        #endregion


    }

}
