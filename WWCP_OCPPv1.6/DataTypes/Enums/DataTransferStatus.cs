/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the data transfer status.
    /// </summary>
    public static class DataTransferStatusExtentions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a data transfer status.
        /// </summary>
        /// <param name="Text">A string representation of a data transfer status.</param>
        public static DataTransferStatus Parse(String Text)
        {

            switch (Text?.ToLower())
            {

                case "accepted":
                    return DataTransferStatus.Accepted;

                case "rejected":
                    return DataTransferStatus.Rejected;

                case "unknownmessageid":
                    return DataTransferStatus.UnknownMessageId;

                case "unknownvendorid":
                    return DataTransferStatus.UnknownVendorId;


                default:
                    return DataTransferStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this DataTransferStatus)

        /// <summary>
        /// Return a string representation of the given data transfer status.
        /// </summary>
        /// <param name="DataTransferStatus">A data transfer status.</param>
        public static String AsText(this DataTransferStatus DataTransferStatus)
        {

            switch (DataTransferStatus)
            {

                case DataTransferStatus.Accepted:
                    return "Accepted";

                case DataTransferStatus.Rejected:
                    return "Rejected";

                case DataTransferStatus.UnknownMessageId:
                    return "UnknownMessageId";

                case DataTransferStatus.UnknownVendorId:
                    return "UnknownVendorId";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


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
