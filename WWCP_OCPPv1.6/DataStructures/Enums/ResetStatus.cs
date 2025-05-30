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
    /// Extensions methods for the reset status.
    /// </summary>
    public static class ResetStatusExtensions
    {

        #region Parse(Text)

        public static ResetStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => ResetStatus.Accepted,
                   "Rejected"  => ResetStatus.Rejected,
                   _           => ResetStatus.Unknown
               };

        #endregion

        #region AsText(this ResetStatus)

        public static String AsText(this ResetStatus ResetStatus)

            => ResetStatus switch {
                   ResetStatus.Accepted  => "Accepted",
                   ResetStatus.Rejected  => "Rejected",
                   _                     => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the reset-status-values.
    /// </summary>
    public enum ResetStatus
    {

        /// <summary>
        /// Unknown reset status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        Rejected

    }

}
