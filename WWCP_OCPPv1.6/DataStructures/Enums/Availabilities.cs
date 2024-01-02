/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for the availability types.
    /// </summary>
    public static class AvailabilityTypesExtensions
    {

        #region Parse(Text)

        public static Availabilities Parse(String Text)

            => Text.Trim() switch {
                   "Inoperative"  => Availabilities.Inoperative,
                   "Operative"    => Availabilities.Operative,
                   _              => Availabilities.Unknown
               };

        #endregion

        #region AsText(this AvailabilityType)

        public static String AsText(this Availabilities AvailabilityType)

            => AvailabilityType switch {
                   Availabilities.Inoperative  => "Inoperative",
                   Availabilities.Operative    => "Operative",
                   _                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the availability-type-values.
    /// </summary>
    public enum Availabilities
    {

        /// <summary>
        /// Unknown availability type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Charge point is not available for charging.
        /// </summary>
        Inoperative,

        /// <summary>
        /// Charge point is available for charging.
        /// </summary>
        Operative

    }

}
