﻿/*
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
    /// Extensions methods for the update types.
    /// </summary>
    public static class UpdateTypesExtensions
    {

        #region Parse(Text)

        public static UpdateTypes Parse(String Text)

            => Text.Trim() switch {
                   "Differential"  => UpdateTypes.Differential,
                   "Full"          => UpdateTypes.Full,
                   _               => UpdateTypes.Unknown
               };

        #endregion

        #region AsText(this UpdateType)

        public static String AsText(this UpdateTypes UpdateType)

            => UpdateType switch {
                   UpdateTypes.Differential  => "Differential",
                   UpdateTypes.Full          => "Full",
                   _                         => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the update-type-values.
    /// </summary>
    public enum UpdateTypes
    {

        /// <summary>
        /// Unknown update type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the current local authorization List
        /// must be updated with the values in this message.
        /// </summary>
        Differential,

        /// <summary>
        /// Indicates that the current local authorization list
        /// must be replaced by the values in this message.
        /// </summary>
        Full

    }

}
