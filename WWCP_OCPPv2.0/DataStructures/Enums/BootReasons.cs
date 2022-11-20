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

            if (TryParse(Text, out var reason))
                return reason;

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

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out Reason)

        /// <summary>
        /// Try to parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        /// <param name="Reason">The parsed boot reason.</param>
        public static Boolean TryParse(String Text, out BootReasons Reason)
        {
            switch (Text.Trim())
            {

                case "ApplicationReset":
                    Reason = BootReasons.ApplicationReset;
                    return true;

                case "FirmwareUpdate":
                    Reason = BootReasons.FirmwareUpdate;
                    return true;

                case "LocalReset":
                    Reason = BootReasons.LocalReset;
                    return true;

                case "PowerUp":
                    Reason = BootReasons.PowerUp;
                    return true;

                case "RemoteReset":
                    Reason = BootReasons.RemoteReset;
                    return true;

                case "ScheduledReset":
                    Reason = BootReasons.ScheduledReset;
                    return true;

                case "Triggered":
                    Reason = BootReasons.Triggered;
                    return true;

                case "Unknown":
                    Reason = BootReasons.Unknown;
                    return true;

                case "Watchdog":
                    Reason = BootReasons.Watchdog;
                    return true;

                default:
                    Reason = BootReasons.Unknown;
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
