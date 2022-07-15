/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for message formats.
    /// </summary>
    public static class MessageFormatsExtentions
    {

        #region Parse(Text)

        public static MessageFormats Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "ASCII":
                    return MessageFormats.ASCII;

                case "HTML":
                    return MessageFormats.HTML;

                case "URI":
                    return MessageFormats.URI;

                case "UTF8":
                    return MessageFormats.UTF8;


                default:
                    return MessageFormats.Unknown;

            }

        }

        #endregion

        #region AsText(this MessageFormats)

        public static String AsText(this MessageFormats MessageFormats)
        {

            switch (MessageFormats)
            {

                case MessageFormats.ASCII:
                    return "ASCII";

                case MessageFormats.HTML:
                    return "HTML";

                case MessageFormats.URI:
                    return "URI";

                case MessageFormats.UTF8:
                    return "UTF8";


                default:
                    return "Unknown";

            }

        }

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

        ASCII,
        HTML,
        URI,
        UTF8

    }

}
