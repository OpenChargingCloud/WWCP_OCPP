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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for charging ticket validation methods.
    /// </summary>
    public static class ChargingTicketValidationMethodsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket validation method.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket validation method.</param>
        public static ChargingTicketValidationMethods Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketValidationMethod))
                return chargingTicketValidationMethod;

            return ChargingTicketValidationMethods.MustBeValidated;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket validation method.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket validation method.</param>
        public static ChargingTicketValidationMethods? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketValidationMethod))
                return chargingTicketValidationMethod;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketValidationMethod)

        /// <summary>
        /// Try to parse the given text as a charging ticket validation method.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket validation method.</param>
        /// <param name="ChargingTicketValidationMethod">The parsed charging ticket validation method.</param>
        public static Boolean TryParse(String Text, out ChargingTicketValidationMethods ChargingTicketValidationMethod)
        {
            switch (Text.Trim())
            {

                case "OfflineChargingAllowed":
                    ChargingTicketValidationMethod = ChargingTicketValidationMethods.OfflineChargingAllowed;
                    return true;

                case "ShouldBeValidated":
                    ChargingTicketValidationMethod = ChargingTicketValidationMethods.ShouldBeValidated;
                    return true;

                default:
                    ChargingTicketValidationMethod = ChargingTicketValidationMethods.MustBeValidated;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketValidationMethod)

        public static String AsText(this ChargingTicketValidationMethods ChargingTicketValidationMethod)

            => ChargingTicketValidationMethod switch {
                   ChargingTicketValidationMethods.OfflineChargingAllowed  => "OfflineChargingAllowed",
                   ChargingTicketValidationMethods.ShouldBeValidated       => "ShouldBeValidated",
                   _                                                       => "MustBeValidated"
            };

        #endregion

    }


    /// <summary>
    /// Charging ticket validation methods.
    /// </summary>
    public enum ChargingTicketValidationMethods
    {

        /// <summary>
        /// The charging ticket can be used at offline charging stations.
        /// </summary>
        OfflineChargingAllowed,

        /// <summary>
        /// The charging ticket should be validated online.
        /// </summary>
        ShouldBeValidated,

        /// <summary>
        /// The charging ticket must be validated online.
        /// </summary>
        MustBeValidated

    }

}
