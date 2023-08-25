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
    /// Extensions methods for the notify allowed energy transfer status.
    /// </summary>
    public static class NotifyAllowedEnergyTransferStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a notify allowed energy transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a notify allowed energy transfer status.</param>
        public static NotifyAllowedEnergyTransferStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return NotifyAllowedEnergyTransferStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a notify allowed energy transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a notify allowed energy transfer status.</param>
        public static NotifyAllowedEnergyTransferStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out NotifyAllowedEnergyTransferStatus)

        /// <summary>
        /// Try to parse the given text as a notify allowed energy transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a notify allowed energy transfer status.</param>
        /// <param name="NotifyAllowedEnergyTransferStatus">The parsed notify allowed energy transfer status.</param>
        public static Boolean TryParse(String Text, out NotifyAllowedEnergyTransferStatus NotifyAllowedEnergyTransferStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    NotifyAllowedEnergyTransferStatus = NotifyAllowedEnergyTransferStatus.Accepted;
                    return true;

                case "Rejected":
                    NotifyAllowedEnergyTransferStatus = NotifyAllowedEnergyTransferStatus.Rejected;
                    return true;

                default:
                    NotifyAllowedEnergyTransferStatus = NotifyAllowedEnergyTransferStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this NotifyAllowedEnergyTransferStatus)

        public static String AsText(this NotifyAllowedEnergyTransferStatus NotifyAllowedEnergyTransferStatus)

            => NotifyAllowedEnergyTransferStatus switch {
                   NotifyAllowedEnergyTransferStatus.Accepted  => "Accepted",
                   NotifyAllowedEnergyTransferStatus.Rejected  => "Rejected",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Notify allowed energy transfer status.
    /// </summary>
    public enum NotifyAllowedEnergyTransferStatus
    {

        /// <summary>
        /// Unknown notify allowed energy transfer status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has been rejected. Should not occur, unless there are
        /// some technical problems.
        /// </summary>
        Rejected

    }

}
