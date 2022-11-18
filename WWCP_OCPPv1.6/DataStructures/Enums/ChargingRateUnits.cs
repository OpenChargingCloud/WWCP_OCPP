/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the charging rate units.
    /// </summary>
    public static class ChargingRateUnitsExtentions
    {

        #region Parse(Text)

        public static ChargingRateUnits Parse(String Text)

            => Text.Trim() switch {
                   "A"  => ChargingRateUnits.Amperes,
                   "W"  => ChargingRateUnits.Watts,
                   _    => ChargingRateUnits.Unknown
               };

        #endregion

        #region AsText(this ChargingRateUnitType)

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
