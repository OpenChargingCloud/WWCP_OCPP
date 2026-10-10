/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for the types of a total cost.
    /// </summary>
    public static class TariffCostsExtensions
    {

        #region TryParse(Text, out TariffCost)

        /// <summary>
        /// Try to parse the given text as a type of a total cost.
        /// </summary>
        /// <param name="Text">A text representation of a type of a total cost.</param>
        /// <param name="TariffCost">The parsed type of a total cost.</param>
        public static Boolean TryParse(String Text, out TariffCosts TariffCost)
        {

            switch (Text.Trim())
            {

                case "NormalCost":  TariffCost = TariffCosts.NormalCost;  return true;
                case "MinCost":     TariffCost = TariffCosts.MinCost;     return true;
                case "MaxCost":     TariffCost = TariffCosts.MaxCost;     return true;

                default:
                    TariffCost = default;
                    return false;

            }

        }

        #endregion

        #region AsText(this TariffCost)

        /// <summary>
        /// The text of the given type of a total cost.
        /// </summary>
        /// <param name="TariffCost">A type of a total cost.</param>
        public static String AsText(this TariffCosts TariffCost)

            => TariffCost switch {
                   TariffCosts.MinCost  => "MinCost",
                   TariffCosts.MaxCost  => "MaxCost",
                   _                    => "NormalCost"
               };

        #endregion

    }


    /// <summary>
    /// The types of a total cost: normal, or the minimum or the maximum cost.
    /// </summary>
    public enum TariffCosts
    {

        /// <summary>
        /// The cost as calculated.
        /// </summary>
        NormalCost,

        /// <summary>
        /// The minimum cost of a tariff with a minimum.
        /// </summary>
        MinCost,

        /// <summary>
        /// The maximum cost of a tariff with a maximum.
        /// </summary>
        MaxCost

    }

}
