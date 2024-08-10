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
    /// Extensions methods for charging ticket meter value signature modes.
    /// </summary>
    public static class ChargingTicketMeterValueSignatureModesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging ticket meter value signature.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket meter value signature.</param>
        public static ChargingTicketMeterValueSignatureModes Parse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMeterValueSignature))
                return chargingTicketMeterValueSignature;

            return ChargingTicketMeterValueSignatureModes.WhenAvailable;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket meter value signature.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket meter value signature.</param>
        public static ChargingTicketMeterValueSignatureModes? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingTicketMeterValueSignature))
                return chargingTicketMeterValueSignature;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingTicketMeterValueSignature)

        /// <summary>
        /// Try to parse the given text as a charging ticket meter value signature.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket meter value signature.</param>
        /// <param name="ChargingTicketMeterValueSignature">The parsed charging ticket meter value signature.</param>
        public static Boolean TryParse(String Text, out ChargingTicketMeterValueSignatureModes ChargingTicketMeterValueSignature)
        {
            switch (Text.Trim())
            {

                case "Mandatory":
                    ChargingTicketMeterValueSignature = ChargingTicketMeterValueSignatureModes.Mandatory;
                    return true;

                case "Off":
                    ChargingTicketMeterValueSignature = ChargingTicketMeterValueSignatureModes.Off;
                    return true;

                default:
                    ChargingTicketMeterValueSignature = ChargingTicketMeterValueSignatureModes.WhenAvailable;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingTicketMeterValueSignature)

        public static String AsText(this ChargingTicketMeterValueSignatureModes ChargingTicketMeterValueSignature)

            => ChargingTicketMeterValueSignature switch {
                   ChargingTicketMeterValueSignatureModes.Mandatory  => "Mandatory",
                   ChargingTicketMeterValueSignatureModes.Off        => "Off",
                   _                                              => "WhenAvailable"
            };

        #endregion

    }


    /// <summary>
    /// Charging ticket meter value signature modes.
    /// </summary>
    public enum ChargingTicketMeterValueSignatureModes
    {

        /// <summary>
        /// Meter values can be be digitally signed, when available.
        /// </summary>
        WhenAvailable,

        /// <summary>
        /// All meter values must be digitally signed.
        /// </summary>
        Mandatory,

        /// <summary>
        /// Meter values should be send without digital signatures.
        /// </summary>
        Off

    }

}
