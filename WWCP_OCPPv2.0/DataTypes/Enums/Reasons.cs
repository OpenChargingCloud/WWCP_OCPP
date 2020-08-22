/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#region Usings

using System;

#endregion

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// Extention methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtentions
    {

        #region Parse(Text)

        public static Reasons AsReasons(String Text)
        {

            switch (Text?.Trim())
            {

                case "DeAuthorized":
                    return Reasons.DeAuthorized;

                case "EmergencyStop":
                    return Reasons.EmergencyStop;

                case "EnergyLimitReached":
                    return Reasons.EnergyLimitReached;

                case "EVDisconnected":
                    return Reasons.EVDisconnected;

                case "GroundFault":
                    return Reasons.GroundFault;

                case "ImmediateReset":
                    return Reasons.ImmediateReset;

                case "Local":
                    return Reasons.Local;

                case "LocalOutOfCredit":
                    return Reasons.LocalOutOfCredit;

                case "MasterPass":
                    return Reasons.MasterPass;

                case "Other":
                    return Reasons.Other;

                case "OvercurrentFault":
                    return Reasons.OvercurrentFault;

                case "PowerLoss":
                    return Reasons.PowerLoss;

                case "PowerQuality":
                    return Reasons.PowerQuality;

                case "Reboot":
                    return Reasons.Reboot;

                case "Remote":
                    return Reasons.Remote;

                case "SOCLimitReached":
                    return Reasons.SOCLimitReached;

                case "StoppedByEV":
                    return Reasons.StoppedByEV;

                case "TimeLimitReached":
                    return Reasons.TimeLimitReached;

                case "Timeout":
                    return Reasons.Timeout;


                default:
                    return Reasons.Unknown;

            }

        }

        #endregion

        #region AsText(this Reasons)

        public static String AsText(this Reasons Reasons)
        {

            switch (Reasons)
            {

                case Reasons.DeAuthorized:
                    return "DeAuthorized";

                case Reasons.EmergencyStop:
                    return "EmergencyStop";

                case Reasons.EnergyLimitReached:
                    return "EnergyLimitReached";

                case Reasons.EVDisconnected:
                    return "EVDisconnected";

                case Reasons.GroundFault:
                    return "GroundFault";

                case Reasons.ImmediateReset:
                    return "ImmediateReset";

                case Reasons.Local:
                    return "Local";

                case Reasons.LocalOutOfCredit:
                    return "LocalOutOfCredit";

                case Reasons.MasterPass:
                    return "MasterPass";

                case Reasons.Other:
                    return "Other";

                case Reasons.OvercurrentFault:
                    return "OvercurrentFault";

                case Reasons.PowerLoss:
                    return "PowerLoss";

                case Reasons.PowerQuality:
                    return "PowerQuality";

                case Reasons.Reboot:
                    return "Reboot";

                case Reasons.Remote:
                    return "Remote";

                case Reasons.SOCLimitReached:
                    return "SOCLimitReached";

                case Reasons.StoppedByEV:
                    return "StoppedByEV";

                case Reasons.TimeLimitReached:
                    return "TimeLimitReached";

                case Reasons.Timeout:
                    return "Timeout";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// The reasons for stopping a transaction.
    /// </summary>
    public enum Reasons
    {

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Unknown,


        /// <summary>
        /// The transaction was stopped because of the authorization
        /// status in a StartTransaction response.
        /// </summary>
        DeAuthorized,

        /// <summary>
        /// The emergency stop button was used.
        /// </summary>
        EmergencyStop,


        EnergyLimitReached,

        /// <summary>
        /// Disconnection of the cable or vehicle moved away
        /// from inductive charge unit.
        /// </summary>
        EVDisconnected,


        GroundFault,


        ImmediateReset,


        /// <summary>
        /// Stopped locally on request of the user at the Charge Point.
        /// This is a regular termination of a transaction.
        /// </summary>
        Local,


        LocalOutOfCredit,


        MasterPass,


        /// <summary>
        /// Any other reason.
        /// </summary>
        Other,


        OvercurrentFault,


        /// <summary>
        /// Complete loss of power.
        /// </summary>
        PowerLoss,

        PowerQuality,


        /// <summary>
        /// A locally initiated reset/reboot occurred,
        /// e.g. the watchdog kicked in.
        /// </summary>
        Reboot,

        /// <summary>
        /// Stopped remotely on request of the user, e.g. via using
        /// a smartphone app or exceeding a (non local) prepaid credit.
        /// This is a regular termination of a transaction.
        /// </summary>
        Remote,


        SOCLimitReached,


        StoppedByEV,


        TimeLimitReached,


        Timeout

    }

}
