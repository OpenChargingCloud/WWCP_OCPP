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

#region Usings

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPP.CS
{

    /// <summary>
    /// The common interface of all events for outgoing OCPP messages from a charging station.
    /// </summary>
    public interface ICSOutgoingMessagesEvents : IEventSender
    {

        // Binary Data Streams Extensions

        #region BinaryDataTransfer                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        event OnBinaryDataTransferRequestSentDelegate?   OnBinaryDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        event OnBinaryDataTransferResponseReceivedDelegate?  OnBinaryDataTransferResponseReceived;

        #endregion


    }

}
