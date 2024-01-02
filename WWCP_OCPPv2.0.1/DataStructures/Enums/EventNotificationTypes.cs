/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extension methods for event notification types.
    /// </summary>
    public static class EventNotificationTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a event notification type.
        /// </summary>
        /// <param name="Text">A text representation of a event notification type.</param>
        public static EventNotificationTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return EventNotificationTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a event notification type.
        /// </summary>
        /// <param name="Text">A text representation of a event notification type.</param>
        public static EventNotificationTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out EventNotificationType)

        /// <summary>
        /// Try to parse the given text as a event notification type.
        /// </summary>
        /// <param name="Text">A text representation of a event notification type.</param>
        /// <param name="EventNotificationType">The parsed event notification type.</param>
        public static Boolean TryParse(String Text, out EventNotificationTypes EventNotificationType)
        {
            switch (Text.Trim())
            {

                case "HardWiredNotification":
                    EventNotificationType = EventNotificationTypes.HardWiredNotification;
                    return true;

                case "HardWiredMonitor":
                    EventNotificationType = EventNotificationTypes.HardWiredMonitor;
                    return true;

                case "PreconfiguredMonitor":
                    EventNotificationType = EventNotificationTypes.PreconfiguredMonitor;
                    return true;

                case "CustomMonitor":
                    EventNotificationType = EventNotificationTypes.CustomMonitor;
                    return true;

                default:
                    EventNotificationType = EventNotificationTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this EventNotificationType)

        public static String AsText(this EventNotificationTypes EventNotificationType)

            => EventNotificationType switch {
                   EventNotificationTypes.HardWiredNotification  => "HardWiredNotification",
                   EventNotificationTypes.HardWiredMonitor       => "HardWiredMonitor",
                   EventNotificationTypes.PreconfiguredMonitor   => "PreconfiguredMonitor",
                   EventNotificationTypes.CustomMonitor          => "CustomMonitor",
                   _                                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Event notification types.
    /// </summary>
    public enum EventNotificationTypes
    {

        /// <summary>
        /// Unknown event notification type.
        /// </summary>
        Unknown,

        /// <summary>
        /// The software implemented by the manufacturer triggered a hardwired notification.
        /// </summary>
        HardWiredNotification,

        /// <summary>
        /// Triggered by a monitor, which is hardwired by the manufacturer.
        /// </summary>
        HardWiredMonitor,

        /// <summary>
        /// Triggered by a monitor, which is preconfigured by the manufacturer.
        /// </summary>
        PreconfiguredMonitor,

        /// <summary>
        /// Triggered by a monitor, which is set with the set variable monitoring request message by the charging station operator.
        /// </summary>
        CustomMonitor

    }

}
