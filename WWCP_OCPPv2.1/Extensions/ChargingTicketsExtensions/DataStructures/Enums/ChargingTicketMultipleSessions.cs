/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for charging ticket multiple sessions.
    /// </summary>
    public static class ChargingTicketMultipleSessionsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket multiple sessions.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multiple sessions.</param>
        public static ChargingTicketMultipleSessions Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMultipleSessions))
                return chargingTicketMultipleSessions;

            return ChargingTicketMultipleSessions.NotAllowed;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket multiple sessions.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multiple sessions.</param>
        public static ChargingTicketMultipleSessions? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMultipleSessions))
                return chargingTicketMultipleSessions;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketMultipleSessions)

        /// <summary>
        /// Try to parse the given text as a charging ticket multiple sessions.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multiple sessions.</param>
        /// <param name="ChargingTicketMultipleSessions">The parsed charging ticket multiple sessions.</param>
        public static Boolean TryParse(String Text, out ChargingTicketMultipleSessions ChargingTicketMultipleSessions)
        {
            switch (Text.Trim())
            {

                case "Allowed":
                    ChargingTicketMultipleSessions = ChargingTicketMultipleSessions.Allowed;
                    return true;

                case "OnlyAtTheSameEVSE":
                    ChargingTicketMultipleSessions = ChargingTicketMultipleSessions.OnlyAtTheSameEVSE;
                    return true;

                case "AfterOnlineValidation":
                    ChargingTicketMultipleSessions = ChargingTicketMultipleSessions.AfterOnlineValidation;
                    return true;

                default:
                    ChargingTicketMultipleSessions = ChargingTicketMultipleSessions.NotAllowed;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketMultipleSessions)

        public static String AsText(this ChargingTicketMultipleSessions ChargingTicketMultipleSessions)

            => ChargingTicketMultipleSessions switch {
                   ChargingTicketMultipleSessions.Allowed                => "Allowed",
                   ChargingTicketMultipleSessions.OnlyAtTheSameEVSE      => "OnlyAtTheSameEVSE",
                   ChargingTicketMultipleSessions.AfterOnlineValidation  => "AfterOnlineValidation",
                   _                                                     => "NotAllowed"
               };

        #endregion

    }


    /// <summary>
    /// Charging ticket multiple sessions.
    /// </summary>
    public enum ChargingTicketMultipleSessions
    {

        /// <summary>
        /// The charging ticket must not be used for multiple charging sessions.
        /// (This might imply to use an online charging ticket validation method!)
        /// </summary>
        NotAllowed,

        /// <summary>
        /// The charging ticket can be used for multiple charging sessions,
        /// but only at the same EVSE.
        /// (This might imply to use an online charging ticket validation method!)
        /// </summary>
        OnlyAtTheSameEVSE,

        /// <summary>
        /// The charging ticket should always be validated online when used for more
        /// than a single charging session.
        /// </summary>
        AfterOnlineValidation,

        /// <summary>
        /// The charging ticket can be used for multiple charging sessions during its life time.
        /// </summary>
        Allowed

    }

}
