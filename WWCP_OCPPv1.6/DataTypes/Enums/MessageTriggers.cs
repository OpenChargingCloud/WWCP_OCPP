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

namespace cloud.charging.adapters.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the message triggers.
    /// </summary>
    public static class MessageTriggersExtentions
    {

        #region Parse(Text)

        public static MessageTriggers Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "BootNotification":
                    return MessageTriggers.BootNotification;

                case "DiagnosticsStatusNotification":
                    return MessageTriggers.DiagnosticsStatusNotification;

                case "FirmwareStatusNotification":
                    return MessageTriggers.FirmwareStatusNotification;

                case "Heartbeat":
                    return MessageTriggers.Heartbeat;

                case "MeterValues":
                    return MessageTriggers.MeterValues;

                case "StatusNotification":
                    return MessageTriggers.StatusNotification;


                default:
                    return MessageTriggers.Unknown;

            }

        }

        #endregion

        #region AsText(this MessageTrigger)

        public static String AsText(this MessageTriggers MessageTrigger)
        {

            switch (MessageTrigger)
            {

                case MessageTriggers.BootNotification:
                    return "BootNotification";

                case MessageTriggers.DiagnosticsStatusNotification:
                    return "DiagnosticsStatusNotification";

                case MessageTriggers.FirmwareStatusNotification:
                    return "FirmwareStatusNotification";

                case MessageTriggers.Heartbeat:
                    return "Heartbeat";

                case MessageTriggers.MeterValues:
                    return "MeterValues";

                case MessageTriggers.StatusNotification:
                    return "StatusNotification";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the message-trigger-values.
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
        /// To trigger a StatusNotification request.
        /// </summary>
        StatusNotification

    }

}
