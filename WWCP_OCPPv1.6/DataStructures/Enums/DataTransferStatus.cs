/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for data transfer status.
    /// </summary>
    public static class DataTransferStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a data transfer status.</param>
        public static DataTransferStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return DataTransferStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a data transfer status.</param>
        public static DataTransferStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out DataTransferStatus)

        /// <summary>
        /// Try to parse the given text as a data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a data transfer status.</param>
        /// <param name="DataTransferStatus">The parsed data transfer status.</param>
        public static Boolean TryParse(String Text, out DataTransferStatus DataTransferStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    DataTransferStatus = DataTransferStatus.Accepted;
                    return true;

                case "Rejected":
                    DataTransferStatus = DataTransferStatus.Rejected;
                    return true;

                case "UnknownMessageId":
                    DataTransferStatus = DataTransferStatus.UnknownMessageId;
                    return true;

                case "UnknownVendorId":
                    DataTransferStatus = DataTransferStatus.UnknownVendorId;
                    return true;

                default:
                    DataTransferStatus = DataTransferStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this DataTransferStatus)

        /// <summary>
        /// Return a string representation of the given data transfer status.
        /// </summary>
        /// <param name="DataTransferStatus">A data transfer status.</param>
        public static String AsText(this DataTransferStatus DataTransferStatus)

            => DataTransferStatus switch {
                   DataTransferStatus.Accepted          => "Accepted",
                   DataTransferStatus.Rejected          => "Rejected",
                   DataTransferStatus.UnknownMessageId  => "UnknownMessageId",
                   DataTransferStatus.UnknownVendorId   => "UnknownVendorId",
                   _                                    => "Unknown"
               };

        #endregion  

    }


    /// <summary>
    /// Data transfer status.
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
        UnknownVendorId,


        Error,
        SignatureError


    }

}
