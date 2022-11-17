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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for trigger reasons.
    /// </summary>
    public static class TriggerReasonsExtentions
    {

        #region Parse(Text)

        public static TriggerReasons Parse(String Text)

            => Text.Trim() switch {
                   "Authorized"            => TriggerReasons.Authorized,
                   "CablePluggedIn"        => TriggerReasons.CablePluggedIn,
                   "ChargingRateChanged"   => TriggerReasons.ChargingRateChanged,
                   "ChargingStateChanged"  => TriggerReasons.ChargingStateChanged,
                   "Deauthorized"          => TriggerReasons.Deauthorized,
                   "EnergyLimitReached"    => TriggerReasons.EnergyLimitReached,
                   "EVCommunicationLost"   => TriggerReasons.EVCommunicationLost,
                   "EVConnectTimeout"      => TriggerReasons.EVConnectTimeout,
                   "MeterValueClock"       => TriggerReasons.MeterValueClock,
                   "MeterValuePeriodic"    => TriggerReasons.MeterValuePeriodic,
                   "TimeLimitReached"      => TriggerReasons.TimeLimitReached,
                   "Trigger"               => TriggerReasons.Trigger,
                   "UnlockCommand"         => TriggerReasons.UnlockCommand,
                   "StopAuthorized"        => TriggerReasons.StopAuthorized,
                   "EVDeparted"            => TriggerReasons.EVDeparted,
                   "EVDetected"            => TriggerReasons.EVDetected,
                   "RemoteStop"            => TriggerReasons.RemoteStop,
                   "RemoteStart"           => TriggerReasons.RemoteStart,
                   "AbnormalCondition"     => TriggerReasons.AbnormalCondition,
                   "SignedDataReceived"    => TriggerReasons.SignedDataReceived,
                   "ResetCommand"          => TriggerReasons.ResetCommand,
                   _                       => TriggerReasons.Unknown
               };

        #endregion

        #region AsText(this TriggerReasons)

        public static String AsText(this TriggerReasons TriggerReasons)

            => TriggerReasons switch {
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
