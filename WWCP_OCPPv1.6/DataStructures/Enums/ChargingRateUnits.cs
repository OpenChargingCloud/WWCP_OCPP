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
    /// Extensions methods for charging rate units.
    /// </summary>
    public static class ChargingRateUnitsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging rate unit.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate unit.</param>
        public static ChargingRateUnits Parse(String Text)
        {

            if (TryParse(Text, out var unit))
                return unit;

            return ChargingRateUnits.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging rate unit.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate unit.</param>
        public static ChargingRateUnits? TryParse(String Text)
        {

            if (TryParse(Text, out var unit))
                return unit;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingRateUnit)

        /// <summary>
        /// Try to parse the given text as a charging rate unit.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate unit.</param>
        /// <param name="ChargingRateUnit">The parsed charging rate unit.</param>
        public static Boolean TryParse(String Text, out ChargingRateUnits ChargingRateUnit)
        {
            switch (Text.Trim())
            {

                case "A":
                    ChargingRateUnit = ChargingRateUnits.Amperes;
                    return true;

                case "W":
                    ChargingRateUnit = ChargingRateUnits.Watts;
                    return true;

                default:
                    ChargingRateUnit = ChargingRateUnits.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this ChargingRateUnitType)

        public static String AsText(this ChargingRateUnits ChargingRateUnitType)

            => ChargingRateUnitType switch {
                   ChargingRateUnits.Amperes  => "A",
                   ChargingRateUnits.Watts    => "W",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the charging-rate-unit-type-values.
    /// </summary>
    public enum ChargingRateUnits
    {

        /// <summary>
        /// Unknown charging-rate-unit type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Watts (power).
        /// </summary>
        Watts,

        /// <summary>
        /// Amperes (current).
        /// </summary>
        Amperes

    }

}
