/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extensions methods for connector formats.
    /// </summary>
    public static class ConnectorFormatsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a connector format.
        /// </summary>
        /// <param name="Text">A text representation of a connector format.</param>
        public static ConnectorFormats Parse(String Text)
        {

            if (TryParse(Text, out var format))
                return format;

            return ConnectorFormats.UNKNOWN;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector format.
        /// </summary>
        /// <param name="Text">A text representation of a connector format.</param>
        public static ConnectorFormats? TryParse(String Text)
        {

            if (TryParse(Text, out var format))
                return format;

            return null;

        }

        #endregion

        #region TryParse(Text, out ConnectorFormat)

        /// <summary>
        /// Try to parse the given text as a connector format.
        /// </summary>
        /// <param name="Text">A text representation of a connector format.</param>
        /// <param name="ConnectorFormat">The parsed connector format.</param>
        public static Boolean TryParse(String Text, out ConnectorFormats ConnectorFormat)
        {
            switch (Text.Trim().ToUpper())
            {

                case "SOCKET":
                    ConnectorFormat = ConnectorFormats.SOCKET;
                    return true;

                case "CABLE":
                    ConnectorFormat = ConnectorFormats.CABLE;
                    return true;

                default:
                    ConnectorFormat = ConnectorFormats.UNKNOWN;
                    return true;

            }
        }

        #endregion

        #region AsText(this ConnectorFormat)

        public static String AsText(this ConnectorFormats ConnectorFormat)

            => ConnectorFormat switch {
                   ConnectorFormats.SOCKET  => "SOCKET",
                   ConnectorFormats.CABLE   => "CABLE",
                   _                        => "UNKNOWN"
               };

        #endregion

    }


    /// <summary>
    /// The format of the connector, whether it is a socket or a plug.
    /// </summary>
    public enum ConnectorFormats
    {

        /// <summary>
        /// Unknown connector format.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The connector is a socket; the EV user needs to bring a fitting plug.
        /// </summary>
        SOCKET,

        /// <summary>
        /// The connector is a attached cable; the EV users car needs to have a fitting inlet.
        /// </summary>
        CABLE

    }

}
