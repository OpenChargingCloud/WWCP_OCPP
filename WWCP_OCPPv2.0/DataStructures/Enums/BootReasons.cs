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
    /// Extention methods for boot reasons.
    /// </summary>
    public static class BootReasonsExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        public static BootReasons Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return BootReasons.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        public static BootReasons? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        /// <param name="Status">The parsed boot reason.</param>
        public static Boolean TryParse(String Text, out BootReasons Status)
        {
            switch (Text.Trim())
            {

                case "ApplicationReset":
                    Status = BootReasons.ApplicationReset;
                    return true;

                case "FirmwareUpdate":
                    Status = BootReasons.FirmwareUpdate;
                    return true;

                case "LocalReset":
                    Status = BootReasons.LocalReset;
                    return true;

                case "PowerUp":
                    Status = BootReasons.PowerUp;
                    return true;

                case "RemoteReset":
                    Status = BootReasons.RemoteReset;
                    return true;

                case "ScheduledReset":
                    Status = BootReasons.ScheduledReset;
                    return true;

                case "Triggered":
                    Status = BootReasons.Triggered;
                    return true;

                case "Unknown":
                    Status = BootReasons.Unknown;
                    return true;

                case "Watchdog":
                    Status = BootReasons.Watchdog;
                    return true;

                default:
                    Status = BootReasons.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this BootReasons BootReason)

            => BootReason switch {
                   BootReasons.ApplicationReset  => "ApplicationReset",
                   BootReasons.FirmwareUpdate    => "FirmwareUpdate",
                   BootReasons.LocalReset        => "LocalReset",
                   BootReasons.PowerUp           => "PowerUp",
                   BootReasons.RemoteReset       => "RemoteReset",
                   BootReasons.ScheduledReset    => "ScheduledReset",
                   BootReasons.Triggered         => "Triggered",
                   BootReasons.Watchdog          => "Watchdog",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Boot reasons.
    /// </summary>
    public enum BootReasons
    {

        ApplicationReset,
        FirmwareUpdate,
        LocalReset,
        PowerUp,
        RemoteReset,
        ScheduledReset,
        Triggered,
        Unknown,
        Watchdog

    }

}
