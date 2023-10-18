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
    /// Extension methods for attribute types.
    /// </summary>
    public static class AttributeTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a attribute type.
        /// </summary>
        /// <param name="Text">A text representation of a attribute type.</param>
        public static AttributeTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return AttributeTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a attribute type.
        /// </summary>
        /// <param name="Text">A text representation of a attribute type.</param>
        public static AttributeTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out AttributeType)

        /// <summary>
        /// Try to parse the given text as a attribute type.
        /// </summary>
        /// <param name="Text">A text representation of a attribute type.</param>
        /// <param name="AttributeType">The parsed attribute type.</param>
        public static Boolean TryParse(String Text, out AttributeTypes AttributeType)
        {
            switch (Text.Trim())
            {

                case "Actual":
                    AttributeType = AttributeTypes.Actual;
                    return true;

                case "Target":
                    AttributeType = AttributeTypes.Target;
                    return true;

                case "MinSet":
                    AttributeType = AttributeTypes.MinSet;
                    return true;

                case "MaxSet":
                    AttributeType = AttributeTypes.MaxSet;
                    return true;

                default:
                    AttributeType = AttributeTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this AttributeType)

        public static String AsText(this AttributeTypes AttributeType)

            => AttributeType switch {
                   AttributeTypes.Actual  => "Actual",
                   AttributeTypes.Target  => "Target",
                   AttributeTypes.MinSet  => "MinSet",
                   AttributeTypes.MaxSet  => "MaxSet",
                   _                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Attribute types.
    /// </summary>
    public enum AttributeTypes
    {

        /// <summary>
        /// Unknown attribute type.
        /// </summary>
        Unknown,

        /// <summary>
        /// The actual value of the variable.
        /// </summary>
        Actual,

        /// <summary>
        /// The target value for this variable.
        /// </summary>
        Target,

        /// <summary>
        /// The minimal allowed value for this variable.
        /// </summary>
        MinSet,

        /// <summary>
        /// The maximum allowed value for this variable.
        /// </summary>
        MaxSet

    }

}
