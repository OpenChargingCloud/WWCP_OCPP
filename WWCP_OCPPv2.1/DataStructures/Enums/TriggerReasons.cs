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

                case "AbnormalCondition":
                    TriggerReason = TriggerReasons.AbnormalCondition;
                    return true;

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

                case "EVDeparted":
                    TriggerReason = TriggerReasons.EVDeparted;
                    return true;

                case "EVDetected":
                    TriggerReason = TriggerReasons.EVDetected;
                    return true;

                case "MeterValueClock":
                    TriggerReason = TriggerReasons.MeterValueClock;
                    return true;

                case "MeterValuePeriodic":
                    TriggerReason = TriggerReasons.MeterValuePeriodic;
                    return true;

                case "RemoteStop":
                    TriggerReason = TriggerReasons.RemoteStop;
                    return true;

                case "RemoteStart":
                    TriggerReason = TriggerReasons.RemoteStart;
                    return true;

                case "ResetCommand":
                    TriggerReason = TriggerReasons.ResetCommand;
                    return true;

                case "SignedDataReceived":
                    TriggerReason = TriggerReasons.SignedDataReceived;
                    return true;

                case "StopAuthorized":
                    TriggerReason = TriggerReasons.StopAuthorized;
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

                case "OperationModeChanged":
                    TriggerReason = TriggerReasons.OperationModeChanged;
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
                   TriggerReasons.AbnormalCondition     => "AbnormalCondition",
                   TriggerReasons.Authorized            => "Authorized",
                   TriggerReasons.CablePluggedIn        => "CablePluggedIn",
                   TriggerReasons.ChargingRateChanged   => "ChargingRateChanged",
                   TriggerReasons.ChargingStateChanged  => "ChargingStateChanged",
                   TriggerReasons.Deauthorized          => "Deauthorized",
                   TriggerReasons.EnergyLimitReached    => "EnergyLimitReached",
                   TriggerReasons.EVCommunicationLost   => "EVCommunicationLost",
                   TriggerReasons.EVConnectTimeout      => "EVConnectTimeout",
                   TriggerReasons.EVDeparted            => "EVDeparted",
                   TriggerReasons.EVDetected            => "EVDetected",
                   TriggerReasons.MeterValueClock       => "MeterValueClock",
                   TriggerReasons.MeterValuePeriodic    => "MeterValuePeriodic",
                   TriggerReasons.RemoteStop            => "RemoteStop",
                   TriggerReasons.RemoteStart           => "RemoteStart",
                   TriggerReasons.ResetCommand          => "ResetCommand",
                   TriggerReasons.SignedDataReceived    => "SignedDataReceived",
                   TriggerReasons.StopAuthorized        => "StopAuthorized",
                   TriggerReasons.TimeLimitReached      => "TimeLimitReached",
                   TriggerReasons.Trigger               => "Trigger",
                   TriggerReasons.UnlockCommand         => "UnlockCommand",
                   TriggerReasons.OperationModeChanged  => "OperationModeChanged",
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


        /// <summary>
        /// An abnormal error or fault condition has occurred.
        /// </summary>
        AbnormalCondition,

        /// <summary>
        /// Charging is authorized, by any means. Might be an RFID, or other authorization means
        /// </summary>
        Authorized,

        /// <summary>
        /// Cable is plugged in and an EV is detected.
        /// </summary>
        CablePluggedIn,

        /// <summary>
        /// Rate of charging changed by more than LimitChangeSignificance.
        /// </summary>
        ChargingRateChanged,

        /// <summary>
        /// Charging State changed.
        /// </summary>
        ChargingStateChanged,

        /// <summary>
        /// The transaction was stopped because of the authorization status in the response to a transactionEventRequest.
        /// </summary>
        Deauthorized,

        /// <summary>
        /// Maximum energy of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        EnergyLimitReached,

        /// <summary>
        /// Communication with EV lost, for example: cable disconnected.
        /// </summary>
        EVCommunicationLost,

        /// <summary>
        /// EV not connected before the connection is timed out.
        /// </summary>
        EVConnectTimeout,

        /// <summary>
        /// EV departed. For example: When a departing EV triggers a parking bay detector.
        /// </summary>
        EVDeparted,

        /// <summary>
        /// EV detected. For example: When an arriving EV triggers a parking bay detector.
        /// </summary>
        EVDetected,

        /// <summary>
        /// Needed to send a clock aligned meter value.
        /// </summary>
        MeterValueClock,

        /// <summary>
        /// Needed to send a periodic meter value.
        /// </summary>
        MeterValuePeriodic,

        /// <summary>
        /// A RequestStopTransactionRequest has been sent.
        /// </summary>
        RemoteStop,

        /// <summary>
        /// A RequestStartTransactionRequest has been sent.
        /// </summary>
        RemoteStart,

        /// <summary>
        /// CSMS sent a Reset Charging Station command.
        /// </summary>
        ResetCommand,

        /// <summary>
        /// Signed data is received from the energy meter.
        /// </summary>
        SignedDataReceived,

        /// <summary>
        /// An EV Driver has been authorized to stop charging. For example: By swiping an RFID card.
        /// </summary>
        StopAuthorized,

        /// <summary>
        /// Maximum time of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        TimeLimitReached,

        /// <summary>
        /// Requested by the CSMS via a TriggerMessageRequest.
        /// </summary>
        Trigger,

        /// <summary>
        /// CSMS sent an Unlock Connector command.
        /// </summary>
        UnlockCommand,

        /// <summary>
        /// The (V2X) operation mode in charging schedule period has changed.
        /// </summary>
        OperationModeChanged

    }

}
