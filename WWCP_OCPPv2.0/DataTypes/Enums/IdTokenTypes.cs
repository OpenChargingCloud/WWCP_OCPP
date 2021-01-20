/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
    /// Extention methods for id token types.
    /// </summary>
    public static class IdTokenTypesExtentions
    {

        #region Parse(Text)

        public static IdTokenTypes Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Central":
                    return IdTokenTypes.Central;

                case "eMAID":
                    return IdTokenTypes.eMAID;

                case "ISO14443":
                    return IdTokenTypes.ISO14443;

                case "ISO15693":
                    return IdTokenTypes.ISO15693;

                case "KeyCode":
                    return IdTokenTypes.KeyCode;

                case "Local":
                    return IdTokenTypes.Local;

                case "MacAddress":
                    return IdTokenTypes.MacAddress;

                case "NoAuthorization":
                    return IdTokenTypes.NoAuthorization;


                default:
                    return IdTokenTypes.Unknown;

            }

        }

        #endregion

        #region AsText(this IdTokenTypes)

        public static String AsText(this IdTokenTypes IdTokenTypes)
        {

            switch (IdTokenTypes)
            {

                case IdTokenTypes.Central:
                    return "Central";

                case IdTokenTypes.eMAID:
                    return "eMAID";

                case IdTokenTypes.ISO14443:
                    return "ISO14443";

                case IdTokenTypes.ISO15693:
                    return "ISO15693";

                case IdTokenTypes.KeyCode:
                    return "KeyCode";

                case IdTokenTypes.Local:
                    return "Local";

                case IdTokenTypes.MacAddress:
                    return "MacAddress";

                case IdTokenTypes.NoAuthorization:
                    return "NoAuthorization";


                default:
                    return "unknown";

            }

        }

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
