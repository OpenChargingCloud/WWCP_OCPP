/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// Extensions methods for get variable status.
    /// </summary>
    public static class GetVariableStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a get variable status.
        /// </summary>
        /// <param name="Text">A text representation of a get variable status.</param>
        public static GetVariableStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GetVariableStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get variable status.
        /// </summary>
        /// <param name="Text">A text representation of a get variable status.</param>
        public static GetVariableStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GetVariableStatus)

        /// <summary>
        /// Try to parse the given text as a get variable status.
        /// </summary>
        /// <param name="Text">A text representation of a get variable status.</param>
        /// <param name="GetVariableStatus">The parsed get variable status.</param>
        public static Boolean TryParse(String Text, out GetVariableStatus GetVariableStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GetVariableStatus = GetVariableStatus.Accepted;
                    return true;

                case "Rejected":
                    GetVariableStatus = GetVariableStatus.Rejected;
                    return true;

                case "UnknownComponent":
                    GetVariableStatus = GetVariableStatus.UnknownComponent;
                    return true;

                case "UnknownVariable":
                    GetVariableStatus = GetVariableStatus.UnknownVariable;
                    return true;

                case "NotSupportedAttributeType":
                    GetVariableStatus = GetVariableStatus.NotSupportedAttributeType;
                    return true;

                default:
                    GetVariableStatus = GetVariableStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GetVariableStatus)

        public static String AsText(this GetVariableStatus GetVariableStatus)

            => GetVariableStatus switch {
                   GetVariableStatus.Accepted                   => "UnknownComponent",
                   GetVariableStatus.Rejected                   => "Rejected",
                   GetVariableStatus.UnknownComponent           => "UnknownComponent",
                   GetVariableStatus.UnknownVariable            => "UnknownVariable",
                   GetVariableStatus.NotSupportedAttributeType  => "NotFound",
                   _                                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Get variable status.
    /// </summary>
    public enum GetVariableStatus
    {

        /// <summary>
        /// Unknown variable status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The variable was set successfully.
        /// </summary>
        Accepted,

        /// <summary>
        /// The request was rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// The requested component is not known.
        /// </summary>
        UnknownComponent,

        /// <summary>
        /// The requested variable is not known.
        /// </summary>
        UnknownVariable,

        /// <summary>
        /// The attribute type is not supported.
        /// </summary>
        NotSupportedAttributeType

    }

}
