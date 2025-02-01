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

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The price rule.
    /// </summary>
    public class PriceRule : IEquatable<PriceRule>
    {

        #region Properties

        /// <summary>
        /// The power range start.
        /// </summary>
        [Mandatory]
        public RationalNumber     PowerRangeStart                  { get; }

        /// <summary>
        /// The energy fee.
        /// </summary>
        [Mandatory]
        public RationalNumber     EnergyFee                        { get; }

        /// <summary>
        /// The optional parking fee.
        /// </summary>
        [Optional]
        public RationalNumber?    ParkingFee                       { get; }

        /// <summary>
        /// The optional parking fee period.
        /// </summary>
        [Optional]
        public TimeSpan?          ParkingFeePeriod                 { get; }

        /// <summary>
        /// The optional carbon dioxide emission.
        /// </summary>
        [Optional]
        public UInt16?            CarbonDioxideEmission            { get; }

        /// <summary>
        /// The optional renewable generation percentage.
        /// </summary>
        [Optional]
        public PercentageDouble?  RenewableGenerationPercentage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price rule.
        /// </summary>
        /// <param name="PowerRangeStart">A power range start.</param>
        /// <param name="EnergyFee">An energy fee.</param>
        /// <param name="ParkingFee">An optional parking fee.</param>
        /// <param name="ParkingFeePeriod">An optional parking fee period.</param>
        /// <param name="CarbonDioxideEmission">An optional carbon dioxide emission.</param>
        /// <param name="RenewableGenerationPercentage">An optional renewable generation percentage.</param>
        public PriceRule(RationalNumber     PowerRangeStart,
                         RationalNumber     EnergyFee,
                         RationalNumber?    ParkingFee                      = null,
                         TimeSpan?          ParkingFeePeriod                = null,
                         UInt16?            CarbonDioxideEmission           = null,
                         PercentageDouble?  RenewableGenerationPercentage   = null)
        {

            this.PowerRangeStart                = PowerRangeStart;
            this.EnergyFee                      = EnergyFee;
            this.ParkingFee                     = ParkingFee;
            this.ParkingFeePeriod               = ParkingFeePeriod;
            this.CarbonDioxideEmission          = CarbonDioxideEmission;
            this.RenewableGenerationPercentage  = RenewableGenerationPercentage;

            unchecked
            {

                hashCode = this.PowerRangeStart.               GetHashCode()       * 17 ^
                           this.EnergyFee.                     GetHashCode()       * 13 ^
                          (this.ParkingFee?.                   GetHashCode() ?? 0) * 11 ^
                          (this.ParkingFeePeriod?.             GetHashCode() ?? 0) *  7 ^
                          (this.CarbonDioxideEmission?.        GetHashCode() ?? 0) *  5 ^
                          (this.RenewableGenerationPercentage?.GetHashCode() ?? 0) *  3 ^

                           base.                               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.\r\n\r\n",
        //     "javaType": "PriceRule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "parkingFeePeriod": {
        //             "description": "The duration of the parking fee period (in seconds).\r\nWhen the time enters into a ParkingFeePeriod, the ParkingFee will apply to the session. .\r\n",
        //             "type": "integer"
        //         },
        //         "carbonDioxideEmission": {
        //             "description": "Number of grams of CO2 per kWh.\r\n",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "renewableGenerationPercentage": {
        //             "description": "Percentage of the power that is created by renewable resources.\r\n",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "energyFee": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "parkingFee": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "powerRangeStart": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "energyFee",
        //         "powerRangeStart"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomPriceRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceRuleParser">An optional delegate to parse custom price rules.</param>
        public static PriceRule Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<PriceRule>?  CustomPriceRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var priceRule,
                         out var errorResponse,
                         CustomPriceRuleParser))
            {
                return priceRule;
            }

            throw new ArgumentException("The given JSON representation of a price rule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PriceRule, out ErrorResponse, CustomPriceRuleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceRule">The parsed price rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out PriceRule?  PriceRule,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out PriceRule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceRule">The parsed price rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceRuleParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out PriceRule?      PriceRule,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<PriceRule>?  CustomPriceRuleParser)
        {

            try
            {

                PriceRule = null;

                #region PowerRangeStart                  [mandatory]

                if (!JSON.ParseMandatoryJSON("powerRangeStart",
                                             "power range start",
                                             RationalNumber.TryParse,
                                             out RationalNumber? PowerRangeStart,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EnergyFee                        [mandatory]

                if (!JSON.ParseMandatoryJSON("energyFee",
                                             "energy fee",
                                             RationalNumber.TryParse,
                                             out RationalNumber? EnergyFee,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ParkingFee                       [optional]

                if (JSON.ParseOptionalJSON("parkingFee",
                                           "parking fee",
                                           RationalNumber.TryParse,
                                           out RationalNumber? ParkingFee,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ParkingFeePeriod                 [optional]

                if (JSON.ParseOptional("ParkingFeePeriod",
                                       "parking fee period",
                                       out TimeSpan? ParkingFeePeriod,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CarbonDioxideEmission            [optional]

                if (JSON.ParseOptional("carbonDioxideEmission",
                                       "carbon dioxide emission",
                                       out UInt16? CarbonDioxideEmission,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RenewableGenerationPercentage    [optional]

                if (JSON.ParseOptional("renewableGenerationPercentage",
                                       "renewable generation percentage",
                                       out PercentageDouble? RenewableGenerationPercentage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PriceRule = new PriceRule(
                                PowerRangeStart,
                                EnergyFee,
                                ParkingFee,
                                ParkingFeePeriod,
                                CarbonDioxideEmission,
                                RenewableGenerationPercentage
                            );

                if (CustomPriceRuleParser is not null)
                    PriceRule = CustomPriceRuleParser(JSON,
                                                      PriceRule);

                return true;

            }
            catch (Exception e)
            {
                PriceRule      = null;
                ErrorResponse  = "The given JSON representation of a price rule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceRuleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceRule>? CustomPriceRuleSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("powerRangeStart",                 PowerRangeStart.ToJSON()),
                                 new JProperty("energyFee",                       EnergyFee.      ToJSON()),

                           ParkingFee is not null
                               ? new JProperty("parkingFee",                      ParkingFee.     ToJSON())
                               : null,

                           ParkingFeePeriod.HasValue
                               ? new JProperty("parkingFeePeriod",                (UInt32) Math.Round(ParkingFeePeriod.Value.TotalSeconds, 0))
                               : null,

                           CarbonDioxideEmission.HasValue
                               ? new JProperty("carbonDioxideEmission",           CarbonDioxideEmission.Value)
                               : null,

                           RenewableGenerationPercentage.HasValue
                               ? new JProperty("renewableGenerationPercentage",   RenewableGenerationPercentage.Value.Value)
                               : null

                       );

            return CustomPriceRuleSerializer is not null
                       ? CustomPriceRuleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceRule1, PriceRule2)

        /// <summary>
        /// Compares two price rules for equality.
        /// </summary>
        /// <param name="PriceRule1">A price rule.</param>
        /// <param name="PriceRule2">Another price rule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PriceRule? PriceRule1,
                                           PriceRule? PriceRule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PriceRule1, PriceRule2))
                return true;

            // If one is null, but not both, return false.
            if (PriceRule1 is null || PriceRule2 is null)
                return false;

            return PriceRule1.Equals(PriceRule2);

        }

        #endregion

        #region Operator != (PriceRule1, PriceRule2)

        /// <summary>
        /// Compares two price rules for inequality.
        /// </summary>
        /// <param name="PriceRule1">A price rule.</param>
        /// <param name="PriceRule2">Another price rule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PriceRule? PriceRule1,
                                           PriceRule? PriceRule2)

            => !(PriceRule1 == PriceRule2);

        #endregion

        #endregion

        #region IEquatable<PriceRule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price rules for equality.
        /// </summary>
        /// <param name="Object">A price rule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceRule priceRule &&
                   Equals(priceRule);

        #endregion

        #region Equals(PriceRule)

        /// <summary>
        /// Compares two price rules for equality.
        /// </summary>
        /// <param name="PriceRule">A price rule to compare with.</param>
        public Boolean Equals(PriceRule? PriceRule)

            => PriceRule is not null &&

               PowerRangeStart.Equals(PriceRule.PowerRangeStart) &&
               EnergyFee.      Equals(PriceRule.EnergyFee)       &&

             ((ParkingFee                 is     null &&  PriceRule.ParkingFee                 is     null) ||
              (ParkingFee                 is not null &&  PriceRule.ParkingFee                 is not null && ParkingFee.                         Equals(PriceRule.ParkingFee)))                  &&

            ((!ParkingFeePeriod.             HasValue && !PriceRule.ParkingFeePeriod.             HasValue) ||
              (ParkingFeePeriod.             HasValue &&  PriceRule.ParkingFeePeriod.             HasValue && ParkingFeePeriod.             Value.Equals(PriceRule.ParkingFeePeriod.Value)))      &&

            ((!CarbonDioxideEmission.        HasValue && !PriceRule.CarbonDioxideEmission.        HasValue) ||
              (CarbonDioxideEmission.        HasValue &&  PriceRule.CarbonDioxideEmission.        HasValue && CarbonDioxideEmission.        Value.Equals(PriceRule.CarbonDioxideEmission.Value))) &&

            ((!RenewableGenerationPercentage.HasValue && !PriceRule.RenewableGenerationPercentage.HasValue) ||
              (RenewableGenerationPercentage.HasValue &&  PriceRule.RenewableGenerationPercentage.HasValue && RenewableGenerationPercentage.Value.Equals(PriceRule.RenewableGenerationPercentage.Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   PowerRangeStart,
                   " kW, ",

                   EnergyFee,

                   ParkingFee is not null
                       ? ", parking fee: " + ParkingFee.ToString()
                       : "",

                   ParkingFeePeriod.HasValue
                       ? ", parking fee period: " + ParkingFeePeriod.Value.TotalSeconds + " second(s)"
                       : "",

                   CarbonDioxideEmission.HasValue
                       ? ", CO2: " + CarbonDioxideEmission.Value
                       : "",

                   RenewableGenerationPercentage.HasValue
                       ? ", Renewable: " + RenewableGenerationPercentage + "%"
                       : ""

               );

        #endregion

    }

}
