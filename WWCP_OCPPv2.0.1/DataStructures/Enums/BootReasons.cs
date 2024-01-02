///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License: Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing: software
// * distributed under the License is distributed on an "AS IS" BASIS:
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//namespace cloud.charging.open.protocols.OCPPv2_0_1
//{

//    /// <summary>
//    /// Extension methods for boot reasons.
//    /// </summary>
//    public static class BootReasonsExtensions
//    {

//        #region Parse   (Text)

//        /// <summary>
//        /// Parse the given text as a boot reason.
//        /// </summary>
//        /// <param name="Text">A text representation of a boot reason.</param>
//        public static BootReasons Parse(String Text)
//        {

//            if (TryParse(Text, out var reason))
//                return reason;

//            return BootReasons.Unknown;

//        }

//        #endregion

//        #region TryParse(Text)

//        /// <summary>
//        /// Try to parse the given text as a boot reason.
//        /// </summary>
//        /// <param name="Text">A text representation of a boot reason.</param>
//        public static BootReasons? TryParse(String Text)
//        {

//            if (TryParse(Text, out var reason))
//                return reason;

//            return null;

//        }

//        #endregion

//        #region TryParse(Text, out BootReason)

//        /// <summary>
//        /// Try to parse the given text as a boot reason.
//        /// </summary>
//        /// <param name="Text">A text representation of a boot reason.</param>
//        /// <param name="BootReason">The parsed boot reason.</param>
//        public static Boolean TryParse(String Text, out BootReasons BootReason)
//        {
//            switch (Text.Trim())
//            {

//                case "ApplicationReset":
//                    BootReason = BootReasons.ApplicationReset;
//                    return true;

//                case "FirmwareUpdate":
//                    BootReason = BootReasons.FirmwareUpdate;
//                    return true;

//                case "LocalReset":
//                    BootReason = BootReasons.LocalReset;
//                    return true;

//                case "PowerUp":
//                    BootReason = BootReasons.PowerUp;
//                    return true;

//                case "RemoteReset":
//                    BootReason = BootReasons.RemoteReset;
//                    return true;

//                case "ScheduledReset":
//                    BootReason = BootReasons.ScheduledReset;
//                    return true;

//                case "Triggered":
//                    BootReason = BootReasons.Triggered;
//                    return true;

//                case "Watchdog":
//                    BootReason = BootReasons.Watchdog;
//                    return true;

//                default:
//                    BootReason = BootReasons.Unknown;
//                    return false;

//            }
//        }

//        #endregion


//        #region AsText(this BootReason)

//        public static String AsText(this BootReasons BootReason)

//            => BootReason switch {
//                   BootReasons.ApplicationReset  => "ApplicationReset",
//                   BootReasons.FirmwareUpdate    => "FirmwareUpdate",
//                   BootReasons.LocalReset        => "LocalReset",
//                   BootReasons.PowerUp           => "PowerUp",
//                   BootReasons.RemoteReset       => "RemoteReset",
//                   BootReasons.ScheduledReset    => "ScheduledReset",
//                   BootReasons.Triggered         => "Triggered",
//                   BootReasons.Watchdog          => "Watchdog",
//                   _                             => "Unknown"
//               };

//        #endregion

//    }


//    /// <summary>
//    /// Boot reasons.
//    /// </summary>
//    public enum BootReasons
//    {

//        /// <summary>
//        /// Application reset.
//        /// </summary>
//        ApplicationReset,

//        /// <summary>
//        /// Firmware update
//        /// </summary>
//        FirmwareUpdate,

//        /// <summary>
//        /// Local reset.
//        /// </summary>
//        LocalReset,

//        /// <summary>
//        /// Power up.
//        /// </summary>
//        PowerUp,

//        /// <summary>
//        /// Remote reset.
//        /// </summary>
//        RemoteReset,

//        /// <summary>
//        /// Scheduled reset.
//        /// </summary>
//        ScheduledReset,

//        /// <summary>
//        /// Triggered.
//        /// </summary>
//        Triggered,

//        /// <summary>
//        /// Unknown boot reason.
//        /// </summary>
//        Unknown,

//        /// <summary>
//        /// Watchdog.
//        /// </summary>
//        Watchdog

//    }

//}
