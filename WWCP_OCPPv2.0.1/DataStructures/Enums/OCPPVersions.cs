/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extension methods for OCPP versions.
    /// </summary>
    public static class OCPPVersionsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        public static OCPPVersions Parse(String Text)
        {

            if (TryParse(Text, out var version))
                return version;

            return OCPPVersions.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        public static OCPPVersions? TryParse(String Text)
        {

            if (TryParse(Text, out var version))
                return version;

            return null;

        }

        #endregion

        #region TryParse(Text, out OCPPVersion)

        /// <summary>
        /// Try to parse the given text as an OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        /// <param name="OCPPVersion">The parsed OCPP version.</param>
        public static Boolean TryParse(String Text, out OCPPVersions OCPPVersion)
        {
            switch (Text.Trim())
            {

                case "OCPP12":
                    OCPPVersion = OCPPVersions.OCPP12;
                    return true;

                case "OCPP15":
                    OCPPVersion = OCPPVersions.OCPP15;
                    return true;

                case "OCPP16":
                    OCPPVersion = OCPPVersions.OCPP16;
                    return true;

                case "OCPP20":
                    OCPPVersion = OCPPVersions.OCPP20;
                    return true;

                default:
                    OCPPVersion = OCPPVersions.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this OCPPVersion)

        public static String AsText(this OCPPVersions OCPPVersion)

            => OCPPVersion switch {
                   OCPPVersions.OCPP12  => "OCPP12",
                   OCPPVersions.OCPP15  => "OCPP15",
                   OCPPVersions.OCPP16  => "OCPP16",
                   OCPPVersions.OCPP20  => "OCPP20",
                   _                    => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// OCPP versions.
    /// </summary>
    public enum OCPPVersions
    {

        /// <summary>
        /// Unknown OCPP version.
        /// </summary>
        Unknown,

        /// <summary>
        /// OCPP Version v1.2
        /// </summary>
        OCPP12,

        /// <summary>
        /// OCPP Version v1.5
        /// </summary>
        OCPP15,

        /// <summary>
        /// OCPP Version v1.6
        /// </summary>
        OCPP16,

        /// <summary>
        /// OCPP Version v2.0
        /// </summary>
        OCPP20

    }

}
