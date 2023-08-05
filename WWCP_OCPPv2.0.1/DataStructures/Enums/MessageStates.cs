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
    /// Extention methods for display message states.
    /// </summary>
    public static class MessageStatesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a display message state.
        /// </summary>
        /// <param name="Text">A text representation of a display message state.</param>
        public static MessageStates Parse(String Text)
        {

            if (TryParse(Text, out var state))
                return state;

            return MessageStates.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a display message state.
        /// </summary>
        /// <param name="Text">A text representation of a display message state.</param>
        public static MessageStates? TryParse(String Text)
        {

            if (TryParse(Text, out var state))
                return state;

            return null;

        }

        #endregion

        #region TryParse(Text, out MessageState)

        /// <summary>
        /// Try to parse the given text as a display message state.
        /// </summary>
        /// <param name="Text">A text representation of a display message state.</param>
        /// <param name="MessageState">The parsed display message state.</param>
        public static Boolean TryParse(String Text, out MessageStates MessageState)
        {
            switch (Text.Trim())
            {

                case "Charging":
                    MessageState = MessageStates.Charging;
                    return true;

                case "Faulted":
                    MessageState = MessageStates.Faulted;
                    return true;

                case "Idle":
                    MessageState = MessageStates.Idle;
                    return true;

                case "Unavailable":
                    MessageState = MessageStates.Unavailable;
                    return true;

                default:
                    MessageState = MessageStates.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MessageState)

        public static String AsText(this MessageStates MessageState)

            => MessageState switch {
                   MessageStates.Charging     => "Charging",
                   MessageStates.Faulted      => "Faulted",
                   MessageStates.Idle         => "Idle",
                   MessageStates.Unavailable  => "Unavailable",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Display message states.
    /// </summary>
    public enum MessageStates
    {

        /// <summary>
        /// 
        /// </summary>
        Unknown,

        /// <summary>
        /// Message only to be shown while the charging station is charging.
        /// </summary>
        Charging,

        /// <summary>
        /// Message only to be shown while the charging station is in faulted state.
        /// </summary>
        Faulted,

        /// <summary>
        /// Message only to be shown while the charging station is idle (not charging).
        /// </summary>
        Idle,

        /// <summary>
        /// Message only to be shown while the charging station is in unavailable state.
        /// </summary>
        Unavailable

    }

}
