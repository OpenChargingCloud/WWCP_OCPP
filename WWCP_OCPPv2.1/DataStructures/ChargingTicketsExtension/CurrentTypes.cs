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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for current types.
    /// </summary>
    public static class CurrentTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        public static CurrentTypes Parse(String Text)
        {

            if (TryParse(Text, out var currentType))
                return currentType;

            return CurrentTypes.ACDC;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        public static CurrentTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var currentType))
                return currentType;

            return null;

        }

        #endregion

        #region TryParse(Text, out CurrentType)

        /// <summary>
        /// Try to parse the given text as a current type.
        /// </summary>
        /// <param name="Text">A text representation of a current type.</param>
        /// <param name="CurrentType">The parsed current type.</param>
        public static Boolean TryParse(String Text, out CurrentTypes CurrentType)
        {
            switch (Text.Trim())
            {

                case "AC":
                    CurrentType = CurrentTypes.AC;
                    return true;

                case "DC":
                    CurrentType = CurrentTypes.DC;
                    return true;

                default:
                    CurrentType = CurrentTypes.ACDC;
                    return false;

            }
        }

        #endregion


        #region AsText(this CurrentType)

        public static String AsText(this CurrentTypes CurrentType)

            => CurrentType switch {
                   CurrentTypes.AC  => "AC",
                   CurrentTypes.DC  => "DC",
                   _                => "ACDC"
               };

        #endregion

    }


    /// <summary>
    /// Allowed currents for charging tickets.
    /// </summary>
    public enum CurrentTypes
    {

        /// <summary>
        /// Only AC charging allowed
        /// </summary>
        AC,

        /// <summary>
        /// Only DC charging allowed
        /// </summary>
        DC,

        /// <summary>
        /// AC and DC charging allowed
        /// </summary>
        ACDC,

    }

}
