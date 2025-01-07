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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for NTP sources.
    /// </summary>
    public static class NTPSourcesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a NTP source.
        /// </summary>
        /// <param name="Text">A text representation of a NTP source.</param>
        public static NTPSources Parse(String Text)
        {

            if (TryParse(Text, out var ntpSource))
                return ntpSource;

            return NTPSources.Manual;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a NTP source.
        /// </summary>
        /// <param name="Text">A text representation of a NTP source.</param>
        public static NTPSources? TryParse(String Text)
        {

            if (TryParse(Text, out var ntpSource))
                return ntpSource;

            return null;

        }

        #endregion

        #region TryParse(Text, out NTPSource)

        /// <summary>
        /// Try to parse the given text as a NTP source.
        /// </summary>
        /// <param name="Text">A text representation of a NTP source.</param>
        /// <param name="NTPSource">The parsed NTP source.</param>
        public static Boolean TryParse(String Text, out NTPSources NTPSource)
        {
            switch (Text.Trim().ToLower())
            {

                case "dhcp":
                    NTPSource = NTPSources.DHCP;
                    return true;

                default:
                    NTPSource = NTPSources.Manual;
                    return false;

            }
        }

        #endregion


        #region AsText(this NTPSource)

        public static String AsText(this NTPSources NTPSource)

            => NTPSource switch {
                   NTPSources.DHCP  => "DHCP",
                   _                => "manual"
            };

        #endregion

    }


    /// <summary>
    /// NTP sources
    /// </summary>
    public enum NTPSources
    {

        /// <summary>
        /// Manual
        /// </summary>
        Manual,

        /// <summary>
        /// DHCP
        /// </summary>
        DHCP

    }

}
