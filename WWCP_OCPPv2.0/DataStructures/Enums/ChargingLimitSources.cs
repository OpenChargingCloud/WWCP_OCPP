/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for charging limit sources.
    /// </summary>
    public static class ChargingLimitSourcesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        public static ChargingLimitSources Parse(String Text)
        {

            if (TryParse(Text, out var source))
                return source;

            return ChargingLimitSources.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        public static ChargingLimitSources? TryParse(String Text)
        {

            if (TryParse(Text, out var source))
                return source;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingLimitSources)

        /// <summary>
        /// Try to parse the given text as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        /// <param name="ChargingLimitSources">The parsed charging limit source.</param>
        public static Boolean TryParse(String Text, out ChargingLimitSources ChargingLimitSources)
        {
            switch (Text.Trim())
            {

                case "EMS":
                    ChargingLimitSources = ChargingLimitSources.EMS;
                    return true;

                case "Other":
                    ChargingLimitSources = ChargingLimitSources.Other;
                    return true;

                case "SO":
                    ChargingLimitSources = ChargingLimitSources.SO;
                    return true;

                case "CSO":
                    ChargingLimitSources = ChargingLimitSources.CSO;
                    return true;

                default:
                    ChargingLimitSources = ChargingLimitSources.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingLimitSources)

        public static String AsText(this ChargingLimitSources ChargingLimitSources)

            => ChargingLimitSources switch {
                   ChargingLimitSources.EMS    => "EMS",
                   ChargingLimitSources.Other  => "Other",
                   ChargingLimitSources.SO     => "SO",
                   ChargingLimitSources.CSO    => "CSO",
                   _                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Charging limit sources.
    /// </summary>
    public enum ChargingLimitSources
    {

        /// <summary>
        /// Unknown source.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that an Energy Management System has sent a charging limit.
        /// </summary>
        EMS,

        /// <summary>
        /// Indicates that an external source, not being an EMS or system operator, has sent a charging limit.
        /// </summary>
        Other,

        /// <summary>
        /// Indicates that a System Operator (DSO or TSO) has sent a charging limit.
        /// </summary>
        SO,

        /// <summary>
        /// Indicates that the CSO has set this charging profile.
        /// </summary>
        CSO

    }

}
