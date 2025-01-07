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
    /// Extensions methods for the charging profile purposes.
    /// </summary>
    public static class ChargingProfilePurposesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        public static ChargingProfilePurposes Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ChargingProfilePurposes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        public static ChargingProfilePurposes? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingProfilePurpose)

        /// <summary>
        /// Try to parse the given text as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose">The parsed charging profile purpose.</param>
        public static Boolean TryParse(String Text, out ChargingProfilePurposes ChargingProfilePurpose)
        {
            switch (Text.Trim())
            {

                case "ChargePointMaxProfile":
                    ChargingProfilePurpose = ChargingProfilePurposes.ChargePointMaxProfile;
                    return true;

                case "TxProfile":
                    ChargingProfilePurpose = ChargingProfilePurposes.TxProfile;
                    return true;

                case "TxDefaultProfile":
                    ChargingProfilePurpose = ChargingProfilePurposes.TxDefaultProfile;
                    return true;

                default:
                    ChargingProfilePurpose = ChargingProfilePurposes.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this ChargingProfilePurpose)

        public static String AsText(this ChargingProfilePurposes ChargingProfilePurpose)

            => ChargingProfilePurpose switch {
                   ChargingProfilePurposes.ChargePointMaxProfile  => "ChargePointMaxProfile",
                   ChargingProfilePurposes.TxProfile              => "TxProfile",
                   _                                              => "TxDefaultProfile"
               };

        #endregion

    }


    /// <summary>
    /// Defines the charging-profile-purpose-values.
    /// </summary>
    public enum ChargingProfilePurposes
    {

        /// <summary>
        /// Unknown charging profile purpose.
        /// </summary>
        Unknown,

        /// <summary>
        /// Configuration for the maximum power or current available
        /// for an entire charge point.
        /// </summary>
        ChargePointMaxProfile,

        /// <summary>
        /// Default profile to be used for new transactions.
        /// </summary>
        TxDefaultProfile,

        /// <summary>
        /// Profile with constraints to be imposed by the charge point
        /// on the current transaction. A profile with this purpose
        /// SHALL cease to be valid when the transaction terminates.
        /// </summary>
        TxProfile

    }

}
