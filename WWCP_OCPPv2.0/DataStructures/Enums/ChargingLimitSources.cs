/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
    public static class ChargingLimitSourcesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        public static ChargingLimitSources Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

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

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        /// <param name="Status">The parsed charging limit source.</param>
        public static Boolean TryParse(String Text, out ChargingLimitSources Status)
        {
            switch (Text.Trim())
            {

                case "EMS":
                    Status = ChargingLimitSources.EMS;
                    return true;

                case "Other":
                    Status = ChargingLimitSources.Other;
                    return true;

                case "SO":
                    Status = ChargingLimitSources.SO;
                    return true;

                case "CSO":
                    Status = ChargingLimitSources.CSO;
                    return true;

                default:
                    Status = ChargingLimitSources.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this ChargingLimitSources BootReason)

            => BootReason switch {
                   ChargingLimitSources.EMS    => "EMS",
                   ChargingLimitSources.Other  => "Other",
                   ChargingLimitSources.SO     => "SO",
                   ChargingLimitSources.CSO    => "CSO",
                   _                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// ...
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
