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
    /// Extentions methods for the reset types.
    /// </summary>
    public static class ResetTypesExtentions
    {

        #region Parse(Text)

        public static ResetTypes Parse(String Text)

            => Text.Trim() switch {
                   "Hard"  => ResetTypes.Hard,
                   "Soft"  => ResetTypes.Soft,
                   _       => ResetTypes.Unknown
               };

        #endregion

        #region AsText(this ResetType)

        public static String AsText(this ResetTypes ResetType)

            => ResetType switch {
                   ResetTypes.Hard  => "Hard",
                   ResetTypes.Soft  => "Soft",
                   _                => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the reset-type-values.
    /// </summary>
    public enum ResetTypes
    {

        /// <summary>
        /// Unknown reset type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Full reboot of Charge Point software.
        /// </summary>
        Hard,

        /// <summary>
        /// Return to initial status, gracefully terminating
        /// any transactions in progress.
        /// </summary>
        Soft

    }

}
