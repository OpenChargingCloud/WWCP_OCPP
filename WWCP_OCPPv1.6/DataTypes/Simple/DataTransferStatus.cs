/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Defines the result of a data transfer.
    /// </summary>
    public enum DataTransferStatus
    {

        /// <summary>
        /// Unknown data transfer status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Message has been accepted, and the contained request is accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// Message has been accepted, but the contained request is rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Message could not be interpreted due to unknown MessageId string.
        /// </summary>
        UnknownMessageId,

        /// <summary>
        /// Message could not be interpreted due to unknown VendorId string.
        /// </summary>
        UnknownVendorId

    }

}
