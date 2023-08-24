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
    /// Extensions methods for trigger reasons.
    /// </summary>
    public static class TriggerReasonsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        public static TriggerReasons Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return TriggerReasons.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        public static TriggerReasons? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out TriggerReason)

        /// <summary>
        /// Try to parse the given text as a trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        /// <param name="TriggerReason">The parsed trigger reason.</param>
        public static Boolean TryParse(String Text, out TriggerReasons TriggerReason)
        {
            switch (Text.Trim())
            {

                case "Authorized":
                    TriggerReason = TriggerReasons.Authorized;
                    return true;

                case "CablePluggedIn":
                    TriggerReason = TriggerReasons.CablePluggedIn;
                    return true;

                case "ChargingRateChanged":
                    TriggerReason = TriggerReasons.ChargingRateChanged;
                    return true;

                case "ChargingStateChanged":
                    TriggerReason = TriggerReasons.ChargingStateChanged;
                    return true;

                case "Deauthorized":
                    TriggerReason = TriggerReasons.Deauthorized;
                    return true;

                case "EnergyLimitReached":
                    TriggerReason = TriggerReasons.EnergyLimitReached;
                    return true;

                case "EVCommunicationLost":
                    TriggerReason = TriggerReasons.EVCommunicationLost;
                    return true;

                case "EVConnectTimeout":
                    TriggerReason = TriggerReasons.EVConnectTimeout;
                    return true;

                case "MeterValueClock":
                    TriggerReason = TriggerReasons.MeterValueClock;
                    return true;

                case "MeterValuePeriodic":
                    TriggerReason = TriggerReasons.MeterValuePeriodic;
                    return true;

                case "TimeLimitReached":
                    TriggerReason = TriggerReasons.TimeLimitReached;
                    return true;

                case "Trigger":
                    TriggerReason = TriggerReasons.Trigger;
                    return true;

                case "UnlockCommand":
                    TriggerReason = TriggerReasons.UnlockCommand;
                    return true;

                case "StopAuthorized":
                    TriggerReason = TriggerReasons.StopAuthorized;
                    return true;

                case "EVDeparted":
                    TriggerReason = TriggerReasons.EVDeparted;
                    return true;

                case "EVDetected":
                    TriggerReason = TriggerReasons.EVDetected;
                    return true;

                case "RemoteStop":
                    TriggerReason = TriggerReasons.RemoteStop;
                    return true;

                case "RemoteStart":
                    TriggerReason = TriggerReasons.RemoteStart;
                    return true;

                case "AbnormalCondition":
                    TriggerReason = TriggerReasons.AbnormalCondition;
                    return true;

                case "SignedDataReceived":
                    TriggerReason = TriggerReasons.SignedDataReceived;
                    return true;

                case "ResetCommand":
                    TriggerReason = TriggerReasons.ResetCommand;
                    return true;


                default:
                    TriggerReason = TriggerReasons.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this TriggerReason)

        public static String AsText(this TriggerReasons TriggerReason)

            => TriggerReason switch {
                   TriggerReasons.Authorized            => "Authorized",
                   TriggerReasons.CablePluggedIn        => "CablePluggedIn",
                   TriggerReasons.ChargingRateChanged   => "ChargingRateChanged",
                   TriggerReasons.ChargingStateChanged  => "ChargingStateChanged",
                   TriggerReasons.Deauthorized          => "Deauthorized",
                   TriggerReasons.EnergyLimitReached    => "EnergyLimitReached",
                   TriggerReasons.EVCommunicationLost   => "EVCommunicationLost",
                   TriggerReasons.EVConnectTimeout      => "EVConnectTimeout",
                   TriggerReasons.MeterValueClock       => "MeterValueClock",
                   TriggerReasons.MeterValuePeriodic    => "MeterValuePeriodic",
                   TriggerReasons.TimeLimitReached      => "TimeLimitReached",
                   TriggerReasons.Trigger               => "Trigger",
                   TriggerReasons.UnlockCommand         => "UnlockCommand",
                   TriggerReasons.StopAuthorized        => "StopAuthorized",
                   TriggerReasons.EVDeparted            => "EVDeparted",
                   TriggerReasons.EVDetected            => "EVDetected",
                   TriggerReasons.RemoteStop            => "RemoteStop",
                   TriggerReasons.RemoteStart           => "RemoteStart",
                   TriggerReasons.AbnormalCondition     => "AbnormalCondition",
                   TriggerReasons.SignedDataReceived    => "SignedDataReceived",
                   TriggerReasons.ResetCommand          => "ResetCommand",
                   _                                    => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Trigger reasons.
    /// </summary>
    public enum TriggerReasons
    {

        /// <summary>
        /// Unknown trigger reason.
        /// </summary>
        Unknown,

        Authorized,
        CablePluggedIn,
        ChargingRateChanged,
        ChargingStateChanged,
        Deauthorized,
        EnergyLimitReached,
        EVCommunicationLost,
        EVConnectTimeout,
        MeterValueClock,
        MeterValuePeriodic,
        TimeLimitReached,
        Trigger,
        UnlockCommand,
        StopAuthorized,
        EVDeparted,
        EVDetected,
        RemoteStop,
        RemoteStart,
        AbnormalCondition,
        SignedDataReceived,
        ResetCommand

    }

}
