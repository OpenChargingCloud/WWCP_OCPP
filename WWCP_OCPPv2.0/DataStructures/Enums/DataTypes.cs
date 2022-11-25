/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for data types.
    /// </summary>
    public static class DataTypesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a data type.
        /// </summary>
        /// <param name="Text">A text representation of a data type.</param>
        public static DataTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return DataTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a data type.
        /// </summary>
        /// <param name="Text">A text representation of a data type.</param>
        public static DataTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out DataType)

        /// <summary>
        /// Try to parse the given text as a data type.
        /// </summary>
        /// <param name="Text">A text representation of a data type.</param>
        /// <param name="DataType">The parsed data type.</param>
        public static Boolean TryParse(String Text, out DataTypes DataType)
        {
            switch (Text.Trim())
            {

                case "string":
                    DataType = DataTypes.String;
                    return true;

                case "decimal":
                    DataType = DataTypes.Decimal;
                    return true;

                case "integer":
                    DataType = DataTypes.Integer;
                    return true;

                case "dateTime":
                    DataType = DataTypes.DateTime;
                    return true;

                case "boolean":
                    DataType = DataTypes.Boolean;
                    return true;

                case "OptionList":
                    DataType = DataTypes.OptionList;
                    return true;

                case "SequenceList":
                    DataType = DataTypes.SequenceList;
                    return true;

                case "MemberList":
                    DataType = DataTypes.MemberList;
                    return true;

                default:
                    DataType = DataTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this DataTypes DataType)

            => DataType switch {
                   DataTypes.String        => "string",
                   DataTypes.Decimal       => "decimal",
                   DataTypes.Integer       => "integer",
                   DataTypes.DateTime      => "dateTime",
                   DataTypes.Boolean       => "boolean",
                   DataTypes.OptionList    => "OptionList",
                   DataTypes.SequenceList  => "SequenceList",
                   DataTypes.MemberList    => "MemberList",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Cost kinds.
    /// </summary>
    public enum DataTypes
    {

        /// <summary>
        /// Unknown data type.
        /// </summary>
        Unknown,

        /// <summary>
        /// This variable is of the type string.
        /// </summary>
        String,

        /// <summary>
        /// This variable is of the type decimal.
        /// </summary>
        Decimal,

        /// <summary>
        /// This variable is of the type integer.
        /// </summary>
        Integer,

        /// <summary>
        /// DateTime following the [RFC3339] specification.
        /// </summary>
        DateTime,

        /// <summary>
        /// This variable is of the type boolean.
        /// </summary>
        Boolean,

        /// <summary>
        /// Supported/allowed values for a single choice, enumerated, text variable.
        /// </summary>
        OptionList,

        /// <summary>
        /// Supported/allowed values for an ordered sequence variable.
        /// </summary>
        SequenceList,

        /// <summary>
        /// Supported/allowed values for a mathematical set variable.
        /// </summary>
        MemberList

    }

}
