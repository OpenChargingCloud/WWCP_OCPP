/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extensions methods for pricing types.
    /// </summary>
    public static class PricingTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a pricing types.
        /// </summary>
        /// <param name="Text">A text representation of a pricing types.</param>
        public static PricingTypes Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return PricingTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a pricing types.
        /// </summary>
        /// <param name="Text">A text representation of a pricing types.</param>
        public static PricingTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out PricingTypes)

        /// <summary>
        /// Try to parse the given text as a pricing types.
        /// </summary>
        /// <param name="Text">A text representation of a pricing types.</param>
        /// <param name="PricingTypes">The parsed pricing types.</param>
        public static Boolean TryParse(String Text, out PricingTypes PricingTypes)
        {
            switch (Text.Trim())
            {

                case "NoPricing":
                    PricingTypes = PricingTypes.NoPricing;
                    return true;

                case "AbsolutePricing":
                    PricingTypes = PricingTypes.AbsolutePricing;
                    return true;

                case "PriceLevels":
                    PricingTypes = PricingTypes.PriceLevels;
                    return true;

                default:
                    PricingTypes = PricingTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this PricingTypes)

        /// <summary>
        /// Return a string representation of the given pricing types.
        /// </summary>
        /// <param name="PricingTypes">A pricing types.</param>
        public static String AsText(this PricingTypes PricingTypes)

            => PricingTypes switch {
                   PricingTypes.NoPricing        => "NoPricing",
                   PricingTypes.AbsolutePricing  => "AbsolutePricing",
                   PricingTypes.PriceLevels      => "PriceLevels",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Pricing types.
    /// </summary>
    public enum PricingTypes
    {

        /// <summary>
        /// Unknown pricing types.
        /// </summary>
        Unknown,

        /// <summary>
        /// No pricing information will be provided.
        /// </summary>
        NoPricing,

        /// <summary>
        /// An absolute pricing structure will be offered.
        /// </summary>
        AbsolutePricing,

        /// <summary>
        /// A price level structure will be offered.
        /// </summary>
        PriceLevels

    }

}
