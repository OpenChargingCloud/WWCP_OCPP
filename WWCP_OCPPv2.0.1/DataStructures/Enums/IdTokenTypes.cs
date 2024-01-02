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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extension methods for identification token types.
    /// </summary>
    public static class IdTokenTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        public static IdTokenTypes Parse(String Text)
        {

            if (TryParse(Text, out var tokenType))
                return tokenType;

            return IdTokenTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        public static IdTokenTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var tokenType))
                return tokenType;

            return null;

        }

        #endregion

        #region TryParse(Text, out IdTokenType)

        /// <summary>
        /// Try to parse the given text as an identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        /// <param name="IdTokenType">The parsed identification token type.</param>
        public static Boolean TryParse(String Text, out IdTokenTypes IdTokenType)
        {
            switch (Text.Trim())
            {

                case "Central":
                    IdTokenType = IdTokenTypes.Central;
                    return true;

                case "eMAID":
                    IdTokenType = IdTokenTypes.eMAID;
                    return true;

                case "ISO14443":
                    IdTokenType = IdTokenTypes.ISO14443;
                    return true;

                case "ISO15693":
                    IdTokenType = IdTokenTypes.ISO15693;
                    return true;

                case "KeyCode":
                    IdTokenType = IdTokenTypes.KeyCode;
                    return true;

                case "Local":
                    IdTokenType = IdTokenTypes.Local;
                    return true;

                case "MacAddress":
                    IdTokenType = IdTokenTypes.MACAddress;
                    return true;

                case "NoAuthorization":
                    IdTokenType = IdTokenTypes.NoAuthorization;
                    return true;

                default:
                    IdTokenType = IdTokenTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this IdTokenType)

        public static String AsText(this IdTokenTypes IdTokenType)

            => IdTokenType switch {
                   IdTokenTypes.Central          => "Central",
                   IdTokenTypes.eMAID            => "eMAID",
                   IdTokenTypes.ISO14443         => "ISO14443",
                   IdTokenTypes.ISO15693         => "ISO15693",
                   IdTokenTypes.KeyCode          => "KeyCode",
                   IdTokenTypes.Local            => "Local",
                   IdTokenTypes.MACAddress       => "MacAddress",
                   IdTokenTypes.NoAuthorization  => "NoAuthorization",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Identification token types.
    /// </summary>
    public enum IdTokenTypes
    {

        /// <summary>
        /// Unknown id token type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Central
        /// </summary>
        Central,

        /// <summary>
        /// eMAID
        /// </summary>
        eMAID,

        /// <summary>
        /// ISO14443
        /// </summary>
        ISO14443,

        /// <summary>
        /// ISO15693
        /// </summary>
        ISO15693,

        /// <summary>
        /// KeyCode
        /// </summary>
        KeyCode,

        /// <summary>
        /// Local
        /// </summary>
        Local,

        /// <summary>
        /// MAC Address
        /// </summary>
        MACAddress,

        /// <summary>
        /// NoAuthorization
        /// </summary>
        NoAuthorization

    }

}
