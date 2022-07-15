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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for boot reasons.
    /// </summary>
    public static class BootReasonsExtentions
    {

        #region Parse(Text)

        public static BootReasons Parse(this String Text)
        {

            switch (Text?.Trim())
            {

                case "ApplicationReset":
                    return BootReasons.ApplicationReset;

                case "FirmwareUpdate":
                    return BootReasons.FirmwareUpdate;

                case "LocalReset":
                    return BootReasons.LocalReset;

                case "PowerUp":
                    return BootReasons.PowerUp;

                case "RemoteReset":
                    return BootReasons.RemoteReset;

                case "ScheduledReset":
                    return BootReasons.ScheduledReset;

                case "Triggered":
                    return BootReasons.Triggered;

                case "Unknown":
                    return BootReasons.Unknown;

                case "Watchdog":
                    return BootReasons.Watchdog;


                default:
                    return BootReasons.Unknown;

            }

        }

        #endregion

        #region AsText(this Phase)

        public static String AsText(this BootReasons BootReason)
        {

            switch (BootReason)
            {

                case BootReasons.ApplicationReset:
                    return "ApplicationReset";

                case BootReasons.FirmwareUpdate:
                    return "FirmwareUpdate";

                case BootReasons.LocalReset:
                    return "LocalReset";

                case BootReasons.PowerUp:
                    return "PowerUp";

                case BootReasons.RemoteReset:
                    return "RemoteReset";

                case BootReasons.ScheduledReset:
                    return "ScheduledReset";

                case BootReasons.Triggered:
                    return "Triggered";

                case BootReasons.Unknown:
                    return "Unknown";

                case BootReasons.Watchdog:
                    return "Watchdog";


                default:
                    return "unknown";

            }

        }

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
