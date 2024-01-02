/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for set variable status.
    /// </summary>
    public static class SetVariableStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a set variable status.
        /// </summary>
        /// <param name="Text">A text representation of a set variable status.</param>
        public static SetVariableStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return SetVariableStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a set variable status.
        /// </summary>
        /// <param name="Text">A text representation of a set variable status.</param>
        public static SetVariableStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out SetVariableStatus)

        /// <summary>
        /// Try to parse the given text as a set variable status.
        /// </summary>
        /// <param name="Text">A text representation of a set variable status.</param>
        /// <param name="SetVariableStatus">The parsed set variable status.</param>
        public static Boolean TryParse(String Text, out SetVariableStatus SetVariableStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SetVariableStatus = SetVariableStatus.Accepted;
                    return true;

                case "UnknownComponent":
                    SetVariableStatus = SetVariableStatus.UnknownComponent;
                    return true;

                case "UnknownVariable":
                    SetVariableStatus = SetVariableStatus.UnknownVariable;
                    return true;

                case "NotSupportedAttributeTypes":
                    SetVariableStatus = SetVariableStatus.NotSupportedAttributeTypes;
                    return true;

                case "Rejected":
                    SetVariableStatus = SetVariableStatus.Rejected;
                    return true;

                default:
                    SetVariableStatus = SetVariableStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this SetVariableStatus)

        public static String AsText(this SetVariableStatus SetVariableStatus)

            => SetVariableStatus switch {
                   SetVariableStatus.Accepted                    => "Accepted",
                   SetVariableStatus.UnknownComponent            => "UnknownComponent",
                   SetVariableStatus.UnknownVariable             => "UnknownVariable",
                   SetVariableStatus.NotSupportedAttributeTypes  => "NotSupportedAttributeTypes",
                   SetVariableStatus.Rejected                    => "Rejected",
                   _                                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Set variable status.
    /// </summary>
    public enum SetVariableStatus
    {

        /// <summary>
        /// Unknown set variable status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The variable was successfully set.
        /// </summary>
        Accepted,

        /// <summary>
        /// The requested component of the attribute is unknown.
        /// </summary>
        UnknownComponent,

        /// <summary>
        /// The requested variable of the attribute is unknown.
        /// </summary>
        UnknownVariable,

        /// <summary>
        /// The requested attribute type is not supported.
        /// </summary>
        NotSupportedAttributeTypes,

        /// <summary>
        /// The request was rejected.
        /// </summary>
        Rejected

    }

}
