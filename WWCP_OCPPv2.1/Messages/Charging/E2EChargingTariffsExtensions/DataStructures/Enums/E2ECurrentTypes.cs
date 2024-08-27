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

#region Usings

using System.Diagnostics.CodeAnalysis;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for current types.
    /// </summary>
    public static class E2ECurrentTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        public static E2ECurrentTypes Parse(String Text)
        {

            if (TryParse(Text, out var currentType))
                return currentType;

            return E2ECurrentTypes.Any;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        public static E2ECurrentTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var currentType))
                return currentType;

            return null;

        }

        #endregion

        #region TryParse(Text, out E2ECurrentType)

        /// <summary>
        /// Try to parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        /// <param name="E2ECurrentType">The parsed current type.</param>
        public static Boolean TryParse(String                                   Text,
                                       [NotNullWhen(true)] out E2ECurrentTypes  E2ECurrentType)
        {
            switch (Text.Trim())
            {

                case "AC":
                    E2ECurrentType = E2ECurrentTypes.AC;
                    return true;

                case "DC":
                    E2ECurrentType = E2ECurrentTypes.DC;
                    return true;

                case "HPC":
                    E2ECurrentType = E2ECurrentTypes.HPC;
                    return true;

                default:
                    E2ECurrentType = E2ECurrentTypes.Any;
                    return false;

            }
        }

        #endregion


        #region AsText(this E2ECurrentType)

        public static String AsText(this E2ECurrentTypes E2ECurrentType)

            => E2ECurrentType switch {
                   E2ECurrentTypes.AC   => "AC",
                   E2ECurrentTypes.DC   => "DC",
                   E2ECurrentTypes.HPC  => "HPC",
                   _                 => "any"
               };

        #endregion

    }


    /// <summary>
    /// Allowed currents for charging tickets.
    /// </summary>
    [ChargingTicketsExtensions]
    public enum E2ECurrentTypes
    {

        /// <summary>
        /// Any type of current allowed
        /// </summary>
        Any,

        /// <summary>
        /// Only AC charging allowed
        /// </summary>
        AC,

        /// <summary>
        /// Only DC charging allowed
        /// </summary>
        DC,

        /// <summary>
        /// High power DC charging allowed
        /// </summary>
        HPC

    }

}
