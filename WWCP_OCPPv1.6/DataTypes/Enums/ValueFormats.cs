/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention method for the value format.
    /// </summary>
    public static class ValueFormatExtentions
    {

        #region Parse(Text)

        public static ValueFormats Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "SignedData":
                    return ValueFormats.SignedData;


                default:
                    return ValueFormats.Raw;

            }

        }

        #endregion

        #region AsText(this ValueFormat)

        public static String AsText(this ValueFormats ValueFormat)
        {

            switch (ValueFormat)
            {

                case ValueFormats.SignedData:
                    return "SignedData";


                default:
                    return "Raw";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the value-format-values.
    /// </summary>
    public enum ValueFormats
    {

        /// <summary>
        ///  Data is to be interpreted as integer/decimal numeric data.
        /// </summary>
        Raw,

        /// <summary>
        /// Data is represented as a signed binary data block, encoded as hex data.
        /// </summary>
        SignedData

    }

}
