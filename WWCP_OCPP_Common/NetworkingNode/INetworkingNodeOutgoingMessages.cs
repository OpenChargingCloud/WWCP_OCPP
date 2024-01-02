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

namespace cloud.charging.open.protocols.OCPP.NN
{

    /// <summary>
    /// The common interface of all networking node outgoing messages.
    /// </summary>
    public interface INetworkingNodeOutgoingMessages
    {

        // Binary Data Streams Extensions

        #region BinaryDataTransfer (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request);

        #endregion


        // Networking Overlay Extensions

        #region NotifyNetworkTopology        (Request)

        /// <summary>
        /// Notify about the current network topology or a current change within the topology.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<NotifyNetworkTopologyResponse> NotifyNetworkTopology(NotifyNetworkTopologyRequest Request);

        #endregion


    }

}
