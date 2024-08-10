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
    /// Extensions methods for charging ticket EV driver communications.
    /// </summary>
    public static class ChargingTicketEVDriverCommunicationsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket EV driver communication.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket EV driver communication.</param>
        public static ChargingTicketE2ECommunicationSecurity Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketValidationMethod))
                return chargingTicketValidationMethod;

            return ChargingTicketE2ECommunicationSecurity.Unencrypted;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket EV driver communication.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket EV driver communication.</param>
        public static ChargingTicketE2ECommunicationSecurity? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketValidationMethod))
                return chargingTicketValidationMethod;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketEVDriverCommunication)

        /// <summary>
        /// Try to parse the given text as a charging ticket EV driver communication.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket EV driver communication.</param>
        /// <param name="ChargingTicketEVDriverCommunication">The parsed charging ticket EV driver communication.</param>
        public static Boolean TryParse(String Text, out ChargingTicketE2ECommunicationSecurity ChargingTicketEVDriverCommunication)
        {
            switch (Text.Trim())
            {

                case "EphemeralKeyAgreement":
                    ChargingTicketEVDriverCommunication = ChargingTicketE2ECommunicationSecurity.EphemeralKeyAgreement;
                    return true;

                default:
                    ChargingTicketEVDriverCommunication = ChargingTicketE2ECommunicationSecurity.Unencrypted;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketEVDriverCommunication)

        public static String AsText(this ChargingTicketE2ECommunicationSecurity ChargingTicketEVDriverCommunication)

            => ChargingTicketEVDriverCommunication switch {
                   ChargingTicketE2ECommunicationSecurity.EphemeralKeyAgreement  => "EphemeralKeyAgreement",
                   _                                                           => "Unencrypted"
            };

        #endregion

    }


    /// <summary>
    /// Charging ticket EV driver communications.
    /// </summary>
    public enum ChargingTicketE2ECommunicationSecurity
    {

        /// <summary>
        /// The communication between the charging station and the EV driver is unencrypted.
        /// </summary>
        Unencrypted,

        /// <summary>
        /// The communication between the charging station and the EV driver uses
        /// an ephemeral key based on the private and public keys.
        /// </summary>
        EphemeralKeyAgreement

    }

}
