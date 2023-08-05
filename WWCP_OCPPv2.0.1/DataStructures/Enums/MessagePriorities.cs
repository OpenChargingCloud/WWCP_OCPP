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
    /// Extention methods for display message prioritys.
    /// </summary>
    public static class MessagePrioritiesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a display message priority.
        /// </summary>
        /// <param name="Text">A text representation of a display message priority.</param>
        public static MessagePriorities Parse(String Text)
        {

            if (TryParse(Text, out var priority))
                return priority;

            return MessagePriorities.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a display message priority.
        /// </summary>
        /// <param name="Text">A text representation of a display message priority.</param>
        public static MessagePriorities? TryParse(String Text)
        {

            if (TryParse(Text, out var priority))
                return priority;

            return null;

        }

        #endregion

        #region TryParse(Text, out MessagePriority)

        /// <summary>
        /// Try to parse the given text as a display message priority.
        /// </summary>
        /// <param name="Text">A text representation of a display message priority.</param>
        /// <param name="MessagePriority">The parsed display message priority.</param>
        public static Boolean TryParse(String Text, out MessagePriorities MessagePriority)
        {
            switch (Text.Trim())
            {

                case "AlwaysFront":
                    MessagePriority = MessagePriorities.AlwaysFront;
                    return true;

                case "InFront":
                    MessagePriority = MessagePriorities.InFront;
                    return true;

                case "NormalCycle":
                    MessagePriority = MessagePriorities.NormalCycle;
                    return true;

                default:
                    MessagePriority = MessagePriorities.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MessagePriority)

        public static String AsText(this MessagePriorities MessagePriority)

            => MessagePriority switch {
                   MessagePriorities.AlwaysFront  => "AlwaysFront",
                   MessagePriorities.InFront      => "InFront",
                   MessagePriorities.NormalCycle  => "NormalCycle",
                   _                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Display message priorities.
    /// </summary>
    public enum MessagePriorities
    {

        /// <summary>
        /// Unknown message priority.
        /// </summary>
        Unknown,

        /// <summary>
        /// Show this message always in front. Highest priority, don’t cycle with other messages.
        /// When a newer message with this message priority is received, this message is replaced.
        /// No message of the charging station itself may override this message.
        /// </summary>
        AlwaysFront,

        /// <summary>
        /// Show this message in front of the normal cycle of messages.
        /// When multiple messages with this priority have to be shown, they SHALL be cycled.
        /// </summary>
        InFront,

        /// <summary>
        /// Show this message in the normal cycle of display messages.
        /// </summary>
        NormalCycle

    }

}
