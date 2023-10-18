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
    /// Extension methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static StopTransactionReasons Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return StopTransactionReasons.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static StopTransactionReasons? TryParse(String Text)
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
        public static Boolean TryParse(String Text, out StopTransactionReasons Reason)
        {
            switch (Text.Trim())
            {

                case "ChargingNeedsNotAccepted":
                    Reason = StopTransactionReasons.ChargingNeedsNotAccepted;
                    return true;

                case "DeAuthorized":
                    Reason = StopTransactionReasons.DeAuthorized;
                    return true;

                case "EmergencyStop":
                    Reason = StopTransactionReasons.EmergencyStop;
                    return true;

                case "EnergyLimitReached":
                    Reason = StopTransactionReasons.EnergyLimitReached;
                    return true;

                case "EVDisconnected":
                    Reason = StopTransactionReasons.EVDisconnected;
                    return true;

                case "GroundFault":
                    Reason = StopTransactionReasons.GroundFault;
                    return true;

                case "ImmediateReset":
                    Reason = StopTransactionReasons.ImmediateReset;
                    return true;

                case "LocalOutOfCredit":
                    Reason = StopTransactionReasons.LocalOutOfCredit;
                    return true;

                case "MasterPass":
                    Reason = StopTransactionReasons.MasterPass;
                    return true;

                case "Other":
                    Reason = StopTransactionReasons.Other;
                    return true;

                case "OvercurrentFault":
                    Reason = StopTransactionReasons.OvercurrentFault;
                    return true;

                case "PowerLoss":
                    Reason = StopTransactionReasons.PowerLoss;
                    return true;

                case "PowerQuality":
                    Reason = StopTransactionReasons.PowerQuality;
                    return true;

                case "Reboot":
                    Reason = StopTransactionReasons.Reboot;
                    return true;

                case "Remote":
                    Reason = StopTransactionReasons.Remote;
                    return true;

                case "SOCLimitReached":
                    Reason = StopTransactionReasons.SOCLimitReached;
                    return true;

                case "StoppedByEV":
                    Reason = StopTransactionReasons.StoppedByEV;
                    return true;

                case "TimeLimitReached":
                    Reason = StopTransactionReasons.TimeLimitReached;
                    return true;

                case "Timeout":
                    Reason = StopTransactionReasons.Timeout;
                    return true;

                default:
                    Reason = StopTransactionReasons.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this Reason)

        public static String AsText(this StopTransactionReasons Reason)

            => Reason switch {
                   StopTransactionReasons.ChargingNeedsNotAccepted  => "ChargingNeedsNotAccepted",
                   StopTransactionReasons.DeAuthorized              => "DeAuthorized",
                   StopTransactionReasons.EmergencyStop             => "EmergencyStop",
                   StopTransactionReasons.EnergyLimitReached        => "EnergyLimitReached",
                   StopTransactionReasons.EVDisconnected            => "EVDisconnected",
                   StopTransactionReasons.GroundFault               => "GroundFault",
                   StopTransactionReasons.ImmediateReset            => "ImmediateReset",
                   StopTransactionReasons.Local                     => "Local",
                   StopTransactionReasons.LocalOutOfCredit          => "LocalOutOfCredit",
                   StopTransactionReasons.MasterPass                => "MasterPass",
                   StopTransactionReasons.Other                     => "Other",
                   StopTransactionReasons.OvercurrentFault          => "OvercurrentFault",
                   StopTransactionReasons.PowerLoss                 => "PowerLoss",
                   StopTransactionReasons.PowerQuality              => "PowerQuality",
                   StopTransactionReasons.Reboot                    => "Reboot",
                   StopTransactionReasons.Remote                    => "Remote",
                   StopTransactionReasons.SOCLimitReached           => "SOCLimitReached",
                   StopTransactionReasons.StoppedByEV               => "StoppedByEV",
                   StopTransactionReasons.TimeLimitReached          => "TimeLimitReached",
                   StopTransactionReasons.Timeout                   => "Timeout",
                   _                                                => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Stop transaction reasons.
    /// </summary>
    public enum StopTransactionReasons
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
