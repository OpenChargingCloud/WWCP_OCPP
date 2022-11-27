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

    // ToDo: Should REALLY not be an enum, but a readonly struct to allow extensibility!

    /// <summary>
    /// Extentions methods for message triggers.
    /// </summary>
    public static class MessageTriggersExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        public static MessageTriggers Parse(String Text)
        {

            if (TryParse(Text, out var trigger))
                return trigger;

            return MessageTriggers.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        public static MessageTriggers? TryParse(String Text)
        {

            if (TryParse(Text, out var trigger))
                return trigger;

            return null;

        }

        #endregion

        #region TryParse(Text, out MessageTrigger)

        /// <summary>
        /// Try to parse the given text as a message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        /// <param name="MessageTrigger">The parsed message trigger.</param>
        public static Boolean TryParse(String Text, out MessageTriggers MessageTrigger)
        {
            switch (Text.Trim())
            {

                case "BootNotification":
                    MessageTrigger = MessageTriggers.BootNotification;
                    return true;

                case "LogStatusNotification":
                    MessageTrigger = MessageTriggers.LogStatusNotification;
                    return true;

                case "DiagnosticsStatusNotification":
                    MessageTrigger = MessageTriggers.DiagnosticsStatusNotification;
                    return true;

                case "FirmwareStatusNotification":
                    MessageTrigger = MessageTriggers.FirmwareStatusNotification;
                    return true;

                case "MeterValues":
                    MessageTrigger = MessageTriggers.MeterValues;
                    return true;

                case "SignChargePointCertificate":
                    MessageTrigger = MessageTriggers.SignChargePointCertificate;
                    return true;

                case "StatusNotification":
                    MessageTrigger = MessageTriggers.StatusNotification;
                    return true;

                default:
                    MessageTrigger = MessageTriggers.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MessageTrigger)

        public static String AsText(this MessageTriggers MessageTrigger)

            => MessageTrigger switch {
                   MessageTriggers.BootNotification               => "BootNotification",
                   MessageTriggers.LogStatusNotification          => "LogStatusNotification",
                   MessageTriggers.DiagnosticsStatusNotification  => "DiagnosticsStatusNotification",
                   MessageTriggers.FirmwareStatusNotification     => "FirmwareStatusNotification",
                   MessageTriggers.Heartbeat                      => "Heartbeat",
                   MessageTriggers.MeterValues                    => "MeterValues",
                   MessageTriggers.SignChargePointCertificate     => "SignChargePointCertificate",
                   MessageTriggers.StatusNotification             => "StatusNotification",
                   _                                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Message triggers.
    /// </summary>
    public enum MessageTriggers
    {

        /// <summary>
        /// Unknown message-trigger status.
        /// </summary>
        Unknown,

        /// <summary>
        /// To trigger a BootNotification request.
        /// </summary>
        BootNotification,

        /// <summary>
        /// To trigger LogStatusNotification.req.
        /// </summary>
        LogStatusNotification,

        /// <summary>
        /// To trigger a DiagnosticsStatusNotification request.
        /// </summary>
        DiagnosticsStatusNotification,

        /// <summary>
        /// To trigger a FirmwareStatusNotification request.
        /// </summary>
        FirmwareStatusNotification,

        /// <summary>
        /// To trigger a Heartbeat request.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// To trigger a MeterValues request.
        /// </summary>
        MeterValues,

        /// <summary>
        /// To trigger a SignCertificate.req with certificateType: ChargePointCertificate.
        /// </summary>
        SignChargePointCertificate,

        /// <summary>
        /// To trigger a StatusNotification request.
        /// </summary>
        StatusNotification

    }

}
