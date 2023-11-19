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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for binary data transfer status.
    /// </summary>
    public static class BinaryDataTransferStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        public static BinaryDataTransferStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return BinaryDataTransferStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        public static BinaryDataTransferStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out BinaryDataTransferStatus)

        /// <summary>
        /// Try to parse the given text as a binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus">The parsed binary data transfer status.</param>
        public static Boolean TryParse(String Text, out BinaryDataTransferStatus BinaryDataTransferStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    BinaryDataTransferStatus = BinaryDataTransferStatus.Accepted;
                    return true;

                case "Rejected":
                    BinaryDataTransferStatus = BinaryDataTransferStatus.Rejected;
                    return true;

                case "UnknownMessageId":
                    BinaryDataTransferStatus = BinaryDataTransferStatus.UnknownMessageId;
                    return true;

                case "UnknownVendorId":
                    BinaryDataTransferStatus = BinaryDataTransferStatus.UnknownVendorId;
                    return true;

                default:
                    BinaryDataTransferStatus = BinaryDataTransferStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this BinaryDataTransferStatus)

        /// <summary>
        /// Return a string representation of the given binary data transfer status.
        /// </summary>
        /// <param name="BinaryDataTransferStatus">A binary data transfer status.</param>
        public static String AsText(this BinaryDataTransferStatus BinaryDataTransferStatus)

            => BinaryDataTransferStatus switch {
                   BinaryDataTransferStatus.Accepted          => "Accepted",
                   BinaryDataTransferStatus.Rejected          => "Rejected",
                   BinaryDataTransferStatus.UnknownMessageId  => "UnknownMessageId",
                   BinaryDataTransferStatus.UnknownVendorId   => "UnknownVendorId",
                   _                                    => "Unknown"
               };

        #endregion  

    }


    /// <summary>
    /// BinaryData transfer status.
    /// </summary>
    public enum BinaryDataTransferStatus
    {

        /// <summary>
        /// Unknown binary data transfer status.
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
