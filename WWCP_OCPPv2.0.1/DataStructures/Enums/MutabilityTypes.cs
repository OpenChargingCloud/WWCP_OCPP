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
    /// Extension methods for mutability types.
    /// </summary>
    public static class MutabilityTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a mutability type.
        /// </summary>
        /// <param name="Text">A text representation of a mutability type.</param>
        public static MutabilityTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return MutabilityTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a mutability type.
        /// </summary>
        /// <param name="Text">A text representation of a mutability type.</param>
        public static MutabilityTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out MutabilityType)

        /// <summary>
        /// Try to parse the given text as a mutability type.
        /// </summary>
        /// <param name="Text">A text representation of a mutability type.</param>
        /// <param name="MutabilityType">The parsed mutability type.</param>
        public static Boolean TryParse(String Text, out MutabilityTypes MutabilityType)
        {
            switch (Text.Trim())
            {

                case "ReadOnly":
                    MutabilityType = MutabilityTypes.ReadOnly;
                    return true;

                case "WriteOnly":
                    MutabilityType = MutabilityTypes.WriteOnly;
                    return true;

                case "ReadWrite":
                    MutabilityType = MutabilityTypes.ReadWrite;
                    return true;

                default:
                    MutabilityType = MutabilityTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MutabilityType)

        public static String AsText(this MutabilityTypes MutabilityType)

            => MutabilityType switch {
                   MutabilityTypes.ReadOnly   => "ReadOnly",
                   MutabilityTypes.WriteOnly  => "WriteOnly",
                   MutabilityTypes.ReadWrite  => "ReadWrite",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Mutability types.
    /// </summary>
    public enum MutabilityTypes
    {

        /// <summary>
        /// Unknown mutability type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Read-Only
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Write-Only
        /// </summary>
        WriteOnly,

        /// <summary>
        /// Local reset.
        /// </summary>
        ReadWrite

    }

}
