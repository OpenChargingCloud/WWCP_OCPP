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
    /// Extensions methods for charging profile kinds.
    /// </summary>
    public static class ChargingProfileKindsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging profile kind.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile kind.</param>
        public static ChargingProfileKinds Parse(String Text)
        {

            if (TryParse(Text, out var kind))
                return kind;

            return ChargingProfileKinds.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile kind.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile kind.</param>
        public static ChargingProfileKinds? TryParse(String Text)
        {

            if (TryParse(Text, out var kind))
                return kind;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingProfileKind)

        /// <summary>
        /// Try to parse the given text as a charging profile kind.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile kind.</param>
        /// <param name="ChargingProfileKind">The parsed charging profile kind.</param>
        public static Boolean TryParse(String Text, out ChargingProfileKinds ChargingProfileKind)
        {
            switch (Text.Trim())
            {

                case "Absolute":
                    ChargingProfileKind = ChargingProfileKinds.Absolute;
                    return true;

                case "Recurring":
                    ChargingProfileKind = ChargingProfileKinds.Recurring;
                    return true;

                case "Relative":
                    ChargingProfileKind = ChargingProfileKinds.Relative;
                    return true;

                default:
                    ChargingProfileKind = ChargingProfileKinds.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingProfileKind)

        public static String AsText(this ChargingProfileKinds ChargingProfileKind)

            => ChargingProfileKind switch {
                   ChargingProfileKinds.Absolute   => "Absolute",
                   ChargingProfileKinds.Recurring  => "Recurring",
                   ChargingProfileKinds.Relative   => "Relative",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Charging profile kinds
    /// </summary>
    public enum ChargingProfileKinds
    {

        /// <summary>
        /// Unknown charging profile kind type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Schedule periods are relative to a fixed point in time defined in the schedule.
        ///  This requires that startSchedule is set to a starting point in time.
        /// </summary>
        Absolute,

        /// <summary>
        /// The schedule restarts periodically at the first schedule period.
        /// To be most useful, this requires that startSchedule is set to a starting point in time.
        /// </summary>
        Recurring,

        /// <summary>
        /// The charging schedule periods start when charging profile is activated.
        /// In most cases this will be at start of the power delivery.
        /// When a charging profile is received for a transaction in progress, then it should activate immediately.
        /// No value for startSchedule should be supplied.
        /// </summary>
        Relative

    }

}
