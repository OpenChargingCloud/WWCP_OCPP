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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for message formats.
    /// </summary>
    public static class MessageFormatsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        public static MessageFormats Parse(String Text)
        {

            if (TryParse(Text, out var format))
                return format;

            return MessageFormats.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        public static MessageFormats? TryParse(String Text)
        {

            if (TryParse(Text, out var format))
                return format;

            return null;

        }

        #endregion

        #region TryParse(Text, out MessageFormat)

        /// <summary>
        /// Try to parse the given text as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        /// <param name="MessageFormats">The parsed message format.</param>
        public static Boolean TryParse(String Text, out MessageFormats MessageFormat)
        {
            switch (Text.Trim())
            {

                case "ASCII":
                    MessageFormat = MessageFormats.ASCII;
                    return true;

                case "HTML":
                    MessageFormat = MessageFormats.HTML;
                    return true;

                case "URI":
                    MessageFormat = MessageFormats.URI;
                    return true;

                case "UTF8":
                    MessageFormat = MessageFormats.UTF8;
                    return true;

                default:
                    MessageFormat = MessageFormats.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MessageFormat)

        public static String AsText(this MessageFormats MessageFormat)

            => MessageFormat switch {
                   MessageFormats.ASCII  => "ASCII",
                   MessageFormats.HTML   => "HTML",
                   MessageFormats.URI    => "URI",
                   MessageFormats.UTF8   => "UTF8",
                   _                     => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Message formats.
    /// </summary>
    public enum MessageFormats
    {

        /// <summary>
        /// Unknown message format.
        /// </summary>
        Unknown,

        /// <summary>
        /// ASCII
        /// </summary>
        ASCII,

        /// <summary>
        /// HTML
        /// </summary>
        HTML,

        /// <summary>
        /// URL/URI
        /// </summary>
        URI,

        /// <summary>
        /// UTF8
        /// </summary>
        UTF8

    }

}
