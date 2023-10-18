/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// Extension methods for event triggers.
    /// </summary>
    public static class EventTriggersExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an event trigger.
        /// </summary>
        /// <param name="Text">A text representation of an event trigger.</param>
        public static EventTriggers Parse(String Text)
        {

            if (TryParse(Text, out var trigger))
                return trigger;

            return EventTriggers.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an event trigger.
        /// </summary>
        /// <param name="Text">A text representation of an event trigger.</param>
        public static EventTriggers? TryParse(String Text)
        {

            if (TryParse(Text, out var trigger))
                return trigger;

            return null;

        }

        #endregion

        #region TryParse(Text, out EventTrigger)

        /// <summary>
        /// Try to parse the given text as an event trigger.
        /// </summary>
        /// <param name="Text">A text representation of an event trigger.</param>
        /// <param name="EventTrigger">The parsed event trigger.</param>
        public static Boolean TryParse(String Text, out EventTriggers EventTrigger)
        {
            switch (Text.Trim())
            {

                case "Alerting":
                    EventTrigger = EventTriggers.Alerting;
                    return true;

                case "Delta":
                    EventTrigger = EventTriggers.Delta;
                    return true;

                case "Periodic":
                    EventTrigger = EventTriggers.Periodic;
                    return true;

                default:
                    EventTrigger = EventTriggers.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this EventTrigger)

        public static String AsText(this EventTriggers EventTrigger)

            => EventTrigger switch {
                   EventTriggers.Alerting  => "Alerting",
                   EventTriggers.Delta     => "Delta",
                   EventTriggers.Periodic  => "Periodic",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Event triggers.
    /// </summary>
    public enum EventTriggers
    {

        /// <summary>
        /// Unknown event trigger.
        /// </summary>
        Unknown,

        /// <summary>
        /// A monitored variable has passed an alert or critical threshold.
        /// </summary>
        Alerting,

        /// <summary>
        /// A delta monitored Variable value has changed by more than specified amount.
        /// </summary>
        Delta,

        /// <summary>
        /// A periodic monitored variable has been sampled for reporting at the specified interval.
        /// </summary>
        Periodic

    }

}
