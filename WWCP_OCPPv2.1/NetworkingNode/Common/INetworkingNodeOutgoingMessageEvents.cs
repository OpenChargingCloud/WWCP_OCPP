﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all networking node outgoing message events.
    /// </summary>
    public interface INetworkingNodeOutgoingMessageEvents
    {

        #region DataTransfer

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent.
        /// </summary>
        event OnDataTransferRequestSentDelegate?              OnDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was sent.
        /// </summary>
        event OnDataTransferResponseSentDelegate?             OnDataTransferResponseSent;

        #endregion


        // Binary Data Streams Extensions

        #region BinaryDataTransfer      (Request/-Response)

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to a charging station.
        /// </summary>
        event OnBinaryDataTransferRequestSentDelegate?        OnBinaryDataTransferRequestSent;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        event OnBinaryDataTransferResponseSentDelegate?       OnBinaryDataTransferResponseSent;

        #endregion


        // E2E Security Extensions

        #region SecureDataTransfer      (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecureDataTransfer request will be sent to a charging station.
        /// </summary>
        event OnSecureDataTransferRequestSentDelegate?        OnSecureDataTransferRequestSent;

        /// <summary>
        /// An event sent whenever a response to a SecureDataTransfer request was sent.
        /// </summary>
        event OnSecureDataTransferResponseSentDelegate?       OnSecureDataTransferResponseSent;

        #endregion


        // Overlay Networking Extensions

        #region NotifyNetworkTopology   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology request will be sent to a charging station.
        /// </summary>
        event OnNotifyNetworkTopologyRequestSentDelegate?     OnNotifyNetworkTopologyRequestSent;

        /// <summary>
        /// An event fired whenever a response to a NotifyNetworkTopology request was received.
        /// </summary>
        event OnNotifyNetworkTopologyResponseSentDelegate?    OnNotifyNetworkTopologyResponseSent;

        #endregion

    }

}
