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
    /// Extensions methods for charging ticket smart charging modes.
    /// </summary>
    public static class ChargingTicketSmartChargingModesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket smart charging mode.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket smart charging mode.</param>
        public static ChargingTicketSmartChargingModes Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketSmartChargingMode))
                return chargingTicketSmartChargingMode;

            return ChargingTicketSmartChargingModes.NotAllowed;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket smart charging mode.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket smart charging mode.</param>
        public static ChargingTicketSmartChargingModes? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketSmartChargingMode))
                return chargingTicketSmartChargingMode;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketSmartChargingMode)

        /// <summary>
        /// Try to parse the given text as a charging ticket smart charging mode.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket smart charging mode.</param>
        /// <param name="ChargingTicketSmartChargingMode">The parsed charging ticket smart charging mode.</param>
        public static Boolean TryParse(String Text, out ChargingTicketSmartChargingModes ChargingTicketSmartChargingMode)
        {
            switch (Text.Trim())
            {

                case "Smart":
                    ChargingTicketSmartChargingMode = ChargingTicketSmartChargingModes.Smart;
                    return true;

                case "Bidirectional":
                    ChargingTicketSmartChargingMode = ChargingTicketSmartChargingModes.Bidirectional;
                    return true;

                default:
                    ChargingTicketSmartChargingMode = ChargingTicketSmartChargingModes.NotAllowed;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketSmartChargingMode)

        public static String AsText(this ChargingTicketSmartChargingModes ChargingTicketSmartChargingMode)

            => ChargingTicketSmartChargingMode switch {
                   ChargingTicketSmartChargingModes.Smart          => "Smart",
                   ChargingTicketSmartChargingModes.Bidirectional  => "Bidirectional",
                   _                                               => "NotAllowed"
            };

        #endregion

    }


    /// <summary>
    /// Charging ticket smart charging modes.
    /// </summary>
    public enum ChargingTicketSmartChargingModes
    {

        /// <summary>
        /// Smart charging is not allowed.
        /// </summary>
        NotAllowed,

        /// <summary>
        /// Smart charging is allowed.
        /// </summary>
        Smart,

        /// <summary>
        /// Bidirectional smart charging is allowed.
        /// </summary>
        Bidirectional

    }

}
