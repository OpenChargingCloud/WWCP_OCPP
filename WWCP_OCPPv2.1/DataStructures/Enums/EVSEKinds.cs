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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for EVSE kinds.
    /// </summary>
    public static class EVSEKindsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an EVSE kind.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE kind.</param>
        public static EVSEKinds Parse(String Text)
        {

            if (TryParse(Text, out var evseKind))
                return evseKind;

            throw new ArgumentException("The given text representation of an EVSE kind is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EVSE kind.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE kind.</param>
        public static EVSEKinds? TryParse(String Text)
        {

            if (TryParse(Text, out var evseKind))
                return evseKind;

            return null;

        }

        #endregion

        #region TryParse(Text, out EVSEKinde)

        /// <summary>
        /// Try to parse the given text as an EVSE kind.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE kind.</param>
        /// <param name="EVSEKinde">The parsed EVSE kind.</param>
        public static Boolean TryParse(String                             Text,
                                       [NotNullWhen(true)] out EVSEKinds  EVSEKinde)
        {
            switch (Text.Trim())
            {

                case "AC":
                    EVSEKinde = EVSEKinds.AC;
                    return true;

                case "DC":
                    EVSEKinde = EVSEKinds.DC;
                    return true;

                default:
                    EVSEKinde = EVSEKinds.AC;
                    return false;

            }
        }

        #endregion


        #region AsText(this EVSEKinde)

        public static String AsText(this EVSEKinds EVSEKinde)

            => EVSEKinde switch {
                   EVSEKinds.AC  => "AC",
                   _             => "DC"
               };

        #endregion

    }


    /// <summary>
    /// Allowed currents for charging tickets.
    /// </summary>
    public enum EVSEKinds
    {

        /// <summary>
        /// Only AC charging allowed
        /// </summary>
        AC,

        /// <summary>
        /// Only DC charging allowed
        /// </summary>
        DC

    }

}
