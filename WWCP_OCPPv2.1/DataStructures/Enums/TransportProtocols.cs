/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for OCPP transport protocols.
    /// </summary>
    public static class TransportProtocolsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an OCPP transport protocol.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP transport protocol.</param>
        public static TransportProtocols Parse(String Text)
        {

            if (TryParse(Text, out var transportProtocol))
                return transportProtocol;

            return TransportProtocols.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCPP transport protocol.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP transport protocol.</param>
        public static TransportProtocols? TryParse(String Text)
        {

            if (TryParse(Text, out var transportProtocol))
                return transportProtocol;

            return null;

        }

        #endregion

        #region TryParse(Text, out TransportProtocol)

        /// <summary>
        /// Try to parse the given text as an OCPP transport protocol.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP transport protocol.</param>
        /// <param name="TransportProtocol">The parsed OCPP transport protocol.</param>
        public static Boolean TryParse(String Text, out TransportProtocols TransportProtocol)
        {
            switch (Text.Trim())
            {

                case "JSON":
                    TransportProtocol = TransportProtocols.JSON;
                    return true;

                case "SOAP":
                    TransportProtocol = TransportProtocols.SOAP;
                    return true;

                default:
                    TransportProtocol = TransportProtocols.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this TransportProtocol)

        public static String AsText(this TransportProtocols TransportProtocol)

            => TransportProtocol switch {
                   TransportProtocols.JSON  => "JSON",
                   TransportProtocols.SOAP  => "SOAP",
                   _                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// OCPP transport protocols.
    /// </summary>
    public enum TransportProtocols
    {

        /// <summary>
        /// Unknown OCPP transport protocol.
        /// </summary>
        Unknown,

        /// <summary>
        /// JSON via HTTP WebSockets.
        /// </summary>
        JSON,

        /// <summary>
        /// HTTP/SOAP/XML for transport.
        /// </summary>
        SOAP

    }

}
