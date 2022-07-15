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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for trigger reasons.
    /// </summary>
    public static class TriggerReasonsExtentions
    {

        #region Parse(Text)

        public static TriggerReasons Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case  "Authorized":
                    return TriggerReasons.Authorized;

                case "CablePluggedIn":
                    return TriggerReasons.CablePluggedIn;

                case "ChargingRateChanged":
                    return TriggerReasons.ChargingRateChanged;

                case "ChargingStateChanged":
                    return TriggerReasons.ChargingStateChanged;

                case "Deauthorized":
                    return TriggerReasons.Deauthorized;

                case "EnergyLimitReached":
                    return TriggerReasons.EnergyLimitReached;

                case "EVCommunicationLost":
                    return TriggerReasons.EVCommunicationLost;

                case "EVConnectTimeout":
                    return TriggerReasons.EVConnectTimeout;

                case "MeterValueClock":
                    return TriggerReasons.MeterValueClock;

                case "MeterValuePeriodic":
                    return TriggerReasons.MeterValuePeriodic;

                case "TimeLimitReached":
                    return TriggerReasons.TimeLimitReached;

                case "Trigger":
                    return TriggerReasons.Trigger;

                case "UnlockCommand":
                    return TriggerReasons.UnlockCommand;

                case "StopAuthorized":
                    return TriggerReasons.StopAuthorized;

                case "EVDeparted":
                    return TriggerReasons.EVDeparted;

                case "EVDetected":
                    return TriggerReasons.EVDetected;

                case "RemoteStop":
                    return TriggerReasons.RemoteStop;

                case "RemoteStart":
                    return TriggerReasons.RemoteStart;

                case "AbnormalCondition":
                    return TriggerReasons.AbnormalCondition;

                case "SignedDataReceived":
                    return TriggerReasons.SignedDataReceived;

                case "ResetCommand":
                    return TriggerReasons.ResetCommand;


                default:
                    return TriggerReasons.Unknown;

            }

        }

        #endregion

        #region AsText(this TriggerReasons)

        public static String AsText(this TriggerReasons TriggerReasons)
        {

            switch (TriggerReasons)
            {

                case TriggerReasons.Authorized:
                    return "Authorized";

                case TriggerReasons.CablePluggedIn:
                    return "CablePluggedIn";

                case TriggerReasons.ChargingRateChanged:
                    return "ChargingRateChanged";

                case TriggerReasons.ChargingStateChanged:
                    return "ChargingStateChanged";

                case TriggerReasons.Deauthorized:
                    return "Deauthorized";

                case TriggerReasons.EnergyLimitReached:
                    return "EnergyLimitReached";

                case TriggerReasons.EVCommunicationLost:
                    return "EVCommunicationLost";

                case TriggerReasons.EVConnectTimeout:
                    return "EVConnectTimeout";

                case TriggerReasons.MeterValueClock:
                    return "MeterValueClock";

                case TriggerReasons.MeterValuePeriodic:
                    return "MeterValuePeriodic";

                case TriggerReasons.TimeLimitReached:
                    return "TimeLimitReached";

                case TriggerReasons.Trigger:
                    return "Trigger";

                case TriggerReasons.UnlockCommand:
                    return "UnlockCommand";

                case TriggerReasons.StopAuthorized:
                    return "StopAuthorized";

                case TriggerReasons.EVDeparted:
                    return "EVDeparted";

                case TriggerReasons.EVDetected:
                    return "EVDetected";

                case TriggerReasons.RemoteStop:
                    return "RemoteStop";

                case TriggerReasons.RemoteStart:
                    return "RemoteStart";

                case TriggerReasons.AbnormalCondition:
                    return "AbnormalCondition";

                case TriggerReasons.SignedDataReceived:
                    return "SignedDataReceived";

                case TriggerReasons.ResetCommand:
                    return "ResetCommand";


                default:
                    return "Unknown";

            }

        }

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
