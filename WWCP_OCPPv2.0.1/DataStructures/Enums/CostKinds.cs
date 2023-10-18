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
    /// Extension methods for cost kinds.
    /// </summary>
    public static class CostKindsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a cost kind.
        /// </summary>
        /// <param name="Text">A text representation of a cost kind.</param>
        public static CostKinds Parse(String Text)
        {

            if (TryParse(Text, out var costKinds))
                return costKinds;

            return CostKinds.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cost kind.
        /// </summary>
        /// <param name="Text">A text representation of a cost kind.</param>
        public static CostKinds? TryParse(String Text)
        {

            if (TryParse(Text, out var costKinds))
                return costKinds;

            return null;

        }

        #endregion

        #region TryParse(Text, out CostKind)

        /// <summary>
        /// Try to parse the given text as a cost kind.
        /// </summary>
        /// <param name="Text">A text representation of a cost kind.</param>
        /// <param name="CostKind">The parsed cost kind.</param>
        public static Boolean TryParse(String Text, out CostKinds CostKind)
        {
            switch (Text.Trim())
            {

                case "CarbonDioxideEmission":
                    CostKind = CostKinds.CarbonDioxideEmission;
                    return true;

                case "RelativePricePercentage":
                    CostKind = CostKinds.RelativePricePercentage;
                    return true;

                case "RenewableGenerationPercentage":
                    CostKind = CostKinds.RenewableGenerationPercentage;
                    return true;

                default:
                    CostKind = CostKinds.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this CostKind)

        public static String AsText(this CostKinds CostKind)

            => CostKind switch {
                   CostKinds.CarbonDioxideEmission          => "CarbonDioxideEmission",
                   CostKinds.RelativePricePercentage        => "RelativePricePercentage",
                   CostKinds.RenewableGenerationPercentage  => "RenewableGenerationPercentage",
                   _                                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Cost kinds.
    /// </summary>
    public enum CostKinds
    {

        /// <summary>
        /// Unknown cost kind.
        /// </summary>
        Unknown,

        /// <summary>
        /// An absolute value of carbon dioxide emissions, in grams per kWh.
        /// </summary>
        CarbonDioxideEmission,

        /// <summary>
        /// Price per kWh, as percentage relative to the maximum price stated in any of all tariffs indicated to the EV.
        /// </summary>
        RelativePricePercentage,

        /// <summary>
        /// Percentage of renewable generation within total generation.
        /// </summary>
        RenewableGenerationPercentage

    }

}
