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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for id token types.
    /// </summary>
    public static class IdTokenTypesExtentions
    {

        #region Parse(Text)

        public static IdTokenTypes Parse(String Text)

            => Text.Trim() switch {
                   "Central"          => IdTokenTypes.Central,
                   "eMAID"            => IdTokenTypes.eMAID,
                   "ISO14443"         => IdTokenTypes.ISO14443,
                   "ISO15693"         => IdTokenTypes.ISO15693,
                   "KeyCode"          => IdTokenTypes.KeyCode,
                   "Local"            => IdTokenTypes.Local,
                   "MacAddress"       => IdTokenTypes.MacAddress,
                   "NoAuthorization"  => IdTokenTypes.NoAuthorization,
                   _                  => IdTokenTypes.Unknown
               };

        #endregion

        #region AsText(this IdTokenTypes)

        public static String AsText(this IdTokenTypes IdTokenTypes)

            => IdTokenTypes switch {
                   IdTokenTypes.Central          => "Central",
                   IdTokenTypes.eMAID            => "eMAID",
                   IdTokenTypes.ISO14443         => "ISO14443",
                   IdTokenTypes.ISO15693         => "ISO15693",
                   IdTokenTypes.KeyCode          => "KeyCode",
                   IdTokenTypes.Local            => "Local",
                   IdTokenTypes.MacAddress       => "MacAddress",
                   IdTokenTypes.NoAuthorization  => "NoAuthorization",
                   _                             => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// Id token types.
    /// </summary>
    public enum IdTokenTypes
    {

        /// <summary>
        /// Unknown id token type.
        /// </summary>
        Unknown,

        Central,
        eMAID,
        ISO14443,
        ISO15693,
        KeyCode,
        Local,
        MacAddress,
        NoAuthorization

    }

}
