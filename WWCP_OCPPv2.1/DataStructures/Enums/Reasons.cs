/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extention methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static Reasons Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return Reasons.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static Reasons? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out Reason)

        /// <summary>
        /// Try to parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        /// <param name="Reason">The parsed stop transaction reason.</param>
        public static Boolean TryParse(String Text, out Reasons Reason)
        {
            switch (Text.Trim())
            {

                case "ChargingNeedsNotAccepted":
                    Reason = Reasons.ChargingNeedsNotAccepted;
                    return true;

                case "DeAuthorized":
                    Reason = Reasons.DeAuthorized;
                    return true;

                case "EmergencyStop":
                    Reason = Reasons.EmergencyStop;
                    return true;

                case "EnergyLimitReached":
                    Reason = Reasons.EnergyLimitReached;
                    return true;

                case "EVDisconnected":
                    Reason = Reasons.EVDisconnected;
                    return true;

                case "GroundFault":
                    Reason = Reasons.GroundFault;
                    return true;

                case "ImmediateReset":
                    Reason = Reasons.ImmediateReset;
                    return true;

                case "LocalOutOfCredit":
                    Reason = Reasons.LocalOutOfCredit;
                    return true;

                case "MasterPass":
                    Reason = Reasons.MasterPass;
                    return true;

                case "Other":
                    Reason = Reasons.Other;
                    return true;

                case "OvercurrentFault":
                    Reason = Reasons.OvercurrentFault;
                    return true;

                case "PowerLoss":
                    Reason = Reasons.PowerLoss;
                    return true;

                case "PowerQuality":
                    Reason = Reasons.PowerQuality;
                    return true;

                case "Reboot":
                    Reason = Reasons.Reboot;
                    return true;

                case "Remote":
                    Reason = Reasons.Remote;
                    return true;

                case "SOCLimitReached":
                    Reason = Reasons.SOCLimitReached;
                    return true;

                case "StoppedByEV":
                    Reason = Reasons.StoppedByEV;
                    return true;

                case "TimeLimitReached":
                    Reason = Reasons.TimeLimitReached;
                    return true;

                case "Timeout":
                    Reason = Reasons.Timeout;
                    return true;

                default:
                    Reason = Reasons.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this Reason)

        public static String AsText(this Reasons Reason)

            => Reason switch {
                   Reasons.ChargingNeedsNotAccepted  => "ChargingNeedsNotAccepted",
                   Reasons.DeAuthorized              => "DeAuthorized",
                   Reasons.EmergencyStop             => "EmergencyStop",
                   Reasons.EnergyLimitReached        => "EnergyLimitReached",
                   Reasons.EVDisconnected            => "EVDisconnected",
                   Reasons.GroundFault               => "GroundFault",
                   Reasons.ImmediateReset            => "ImmediateReset",
                   Reasons.Local                     => "Local",
                   Reasons.LocalOutOfCredit          => "LocalOutOfCredit",
                   Reasons.MasterPass                => "MasterPass",
                   Reasons.Other                     => "Other",
                   Reasons.OvercurrentFault          => "OvercurrentFault",
                   Reasons.PowerLoss                 => "PowerLoss",
                   Reasons.PowerQuality              => "PowerQuality",
                   Reasons.Reboot                    => "Reboot",
                   Reasons.Remote                    => "Remote",
                   Reasons.SOCLimitReached           => "SOCLimitReached",
                   Reasons.StoppedByEV               => "StoppedByEV",
                   Reasons.TimeLimitReached          => "TimeLimitReached",
                   Reasons.Timeout                   => "Timeout",
                   _                                 => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Stop transaction reasons.
    /// </summary>
    public enum Reasons
    {

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Unknown,


        /// <summary>
        /// CSMS cannot accept the requested energy transfer type or other part of the EV charging needs.
        /// </summary>
        ChargingNeedsNotAccepted,

        /// <summary>
        /// The transaction was stopped because of the authorization
        /// status in a StartTransaction response.
        /// </summary>
        DeAuthorized,

        /// <summary>
        /// The emergency stop button was used.
        /// </summary>
        EmergencyStop,

        /// <summary>
        /// EV charging session reached a locally enforced maximum energy transfer limit.
        /// </summary>
        EnergyLimitReached,

        /// <summary>
        /// Disconnection of the cable or vehicle moved away from inductive charge unit.
        /// </summary>
        EVDisconnected,

        /// <summary>
        /// A GroundFault has occurred.
        /// </summary>
        GroundFault,

        /// <summary>
        /// A Reset(Immediate) command was received.
        /// </summary>
        ImmediateReset,

        /// <summary>
        /// Stopped locally on request of the EV Driver at the charging station.
        /// This is a regular termination of a transaction.
        /// Examples: presenting an IdToken tag, pressing a button to stop.
        /// </summary>
        Local,

        /// <summary>
        /// A local credit limit enforced through the charging station has been exceeded.
        /// </summary>
        LocalOutOfCredit,

        /// <summary>
        /// The transaction was stopped using a token with a MasterPassGroupId.
        /// </summary>
        MasterPass,

        /// <summary>
        /// Any other reason.
        /// </summary>
        Other,

        /// <summary>
        /// A larger than intended electric current has occurred.
        /// </summary>
        OvercurrentFault,

        /// <summary>
        /// Complete loss of power.
        /// </summary>
        PowerLoss,

        /// <summary>
        /// Quality of power too low, e.g. voltage too low/high, phase imbalance, etc
        /// </summary>
        PowerQuality,

        /// <summary>
        /// A locally initiated reset/reboot occurred, e.g. the watchdog kicked in.
        /// </summary>
        Reboot,

        /// <summary>
        /// Stopped remotely on request of the CSMS. This is a regular termination of a transaction.
        /// Examples: termination using a smartphone app, exceeding a(non local) prepaid credit.
        /// </summary>
        Remote,

        /// <summary>
        /// Electric vehicle has reported reaching a locally enforced maximum battery state-of-charge.
        /// </summary>
        SOCLimitReached,

        /// <summary>
        /// The transaction was stopped by the EV.
        /// </summary>
        StoppedByEV,

        /// <summary>
        /// EV charging session reached a locally enforced time limit.
        /// </summary>
        TimeLimitReached,

        /// <summary>
        /// EV not connected within timeout.
        /// </summary>
        Timeout

    }

}
