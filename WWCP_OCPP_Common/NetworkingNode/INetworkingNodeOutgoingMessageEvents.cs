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

#region Usings

using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPP.NN
{

    /// <summary>
    /// The common interface of all networking node outgoing message events.
    /// </summary>
    public interface INetworkingNodeOutgoingMessageEvents
    {

        // Binary Data Streams Extensions

        #region BinaryDataTransfer      (Request/-Response)

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to a charging station.
        /// </summary>
        event OnBinaryDataTransferRequestDelegate?              OnBinaryDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        event OnBinaryDataTransferResponseDelegate?             OnBinaryDataTransferResponse;

        #endregion


        // Overlay Networking Extensions

        #region NotifyNetworkTopology   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology request will be sent to a charging station.
        /// </summary>
        event OnNotifyNetworkTopologyRequestDelegate?              OnNotifyNetworkTopologyRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyNetworkTopology request was received.
        /// </summary>
        event OnNotifyNetworkTopologyResponseDelegate?             OnNotifyNetworkTopologyResponse;

        #endregion


    }

}
