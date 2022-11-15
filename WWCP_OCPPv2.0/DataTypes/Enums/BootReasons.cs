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

        #region Parse(Text)

        public static BootReasons Parse(this String Text)

            => Text.Trim() switch {
                "ApplicationReset"  => BootReasons.ApplicationReset,
                "FirmwareUpdate"    => BootReasons.FirmwareUpdate,
                "LocalReset"        => BootReasons.LocalReset,
                "PowerUp"           => BootReasons.PowerUp,
                "RemoteReset"       => BootReasons.RemoteReset,
                "ScheduledReset"    => BootReasons.ScheduledReset,
                "Triggered"         => BootReasons.Triggered,
                "Unknown"           => BootReasons.Unknown,
                "Watchdog"          => BootReasons.Watchdog,
                _                   => BootReasons.Unknown
            };

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
                BootReasons.Unknown           => "Unknown",
                BootReasons.Watchdog          => "Watchdog",
                _                             => "unknown"
            };

        #endregion

    }


    /// <summary>
    /// ...
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
