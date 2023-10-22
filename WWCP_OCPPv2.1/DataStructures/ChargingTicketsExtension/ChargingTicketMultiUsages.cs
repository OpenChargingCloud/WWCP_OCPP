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
    /// Extensions methods for charging ticket multi-usages.
    /// </summary>
    public static class ChargingTicketMultiUsagesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket multi-usage.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multi-usage.</param>
        public static ChargingTicketMultiUsages Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMultiUsage))
                return chargingTicketMultiUsage;

            return ChargingTicketMultiUsages.NotAllowed;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket multi-usage.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multi-usage.</param>
        public static ChargingTicketMultiUsages? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMultiUsage))
                return chargingTicketMultiUsage;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketMultiUsage)

        /// <summary>
        /// Try to parse the given text as a charging ticket multi-usage.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket multi-usage.</param>
        /// <param name="ChargingTicketMultiUsage">The parsed charging ticket multi-usage.</param>
        public static Boolean TryParse(String Text, out ChargingTicketMultiUsages ChargingTicketMultiUsage)
        {
            switch (Text.Trim())
            {

                case "Allowed":
                    ChargingTicketMultiUsage = ChargingTicketMultiUsages.Allowed;
                    return true;

                case "AfterOnlineValidation":
                    ChargingTicketMultiUsage = ChargingTicketMultiUsages.AfterOnlineValidation;
                    return true;

                default:
                    ChargingTicketMultiUsage = ChargingTicketMultiUsages.NotAllowed;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketMultiUsage)

        public static String AsText(this ChargingTicketMultiUsages ChargingTicketMultiUsage)

            => ChargingTicketMultiUsage switch {
                   ChargingTicketMultiUsages.Allowed                => "Allowed",
                   ChargingTicketMultiUsages.AfterOnlineValidation  => "AfterOnlineValidation",
                   _                                                => "NotAllowed"
               };

        #endregion

    }


    /// <summary>
    /// Charging ticket multi-usages.
    /// </summary>
    public enum ChargingTicketMultiUsages
    {

        /// <summary>
        /// The charging ticket must not be used for multiple charging sessions.
        /// (This might imply to use an online charging ticket validation method!)
        /// </summary>
        NotAllowed,

        /// <summary>
        /// The charging ticket should be validated online when used more than once.
        /// </summary>
        AfterOnlineValidation,

        /// <summary>
        /// The charging ticket can be used for multiple charging sessions during its life time.
        /// </summary>
        Allowed

    }

}
