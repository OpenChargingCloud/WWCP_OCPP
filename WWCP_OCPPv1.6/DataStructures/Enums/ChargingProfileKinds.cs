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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the charging profile kinds.
    /// </summary>
    public static class ChargingProfileKindsExtensions
    {

        #region Parse(Text)

        public static ChargingProfileKinds Parse(String Text)

            => Text.Trim() switch {
                   "Absolute"   => ChargingProfileKinds.Absolute,
                   "Recurring"  => ChargingProfileKinds.Recurring,
                   "Relative"   => ChargingProfileKinds.Relative,
                   _            => ChargingProfileKinds.Unknown
               };

        #endregion

        #region AsText(this ChargingProfileKindType)

        public static String AsText(this ChargingProfileKinds ChargingProfileKindType)

            => ChargingProfileKindType switch {
                   ChargingProfileKinds.Absolute   => "Absolute",
                   ChargingProfileKinds.Recurring  => "Recurring",
                   ChargingProfileKinds.Relative   => "Relative",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the charging-profile-kind-type-values.
    /// </summary>
    public enum ChargingProfileKinds
    {

        /// <summary>
        /// Unknown charging profile kind type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Schedule periods are relative to a fixed point in
        /// time defined in the schedule.
        /// </summary>
        Absolute,

        /// <summary>
        /// The schedule restarts periodically at the first schedule period.
        /// </summary>
        Recurring,

        /// <summary>
        /// Schedule periods are relative to a situationspecific start
        /// point (such as the start of a session) that is determined
        /// by the charge point.
        /// </summary>
        Relative

    }

}
