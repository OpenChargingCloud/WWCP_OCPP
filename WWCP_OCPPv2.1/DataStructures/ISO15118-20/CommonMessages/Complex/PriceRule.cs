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
        public Decimal         PowerRangeStart                  { get; }

        /// <summary>
        /// The energy fee.
        /// </summary>
        [Mandatory]
        public Decimal         EnergyFee                        { get; }

        /// <summary>
        /// The optional parking fee.
        /// </summary>
        [Optional]
        public Decimal?        ParkingFee                       { get; }

        /// <summary>
        /// The optional parking fee period.
        /// </summary>
        [Optional]
        public TimeSpan?       ParkingFeePeriod                 { get; }

        /// <summary>
        /// The optional carbon dioxide emission.
        /// </summary>
        [Optional]
        public UInt16?         CarbonDioxideEmission            { get; }

        /// <summary>
        /// The optional renewable generation percentage.
        /// </summary>
        [Optional]
        public PercentageByte?  RenewableGenerationPercentage    { get; }

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
        public PriceRule(Decimal         PowerRangeStart,
                         Decimal         EnergyFee,
                         Decimal?        ParkingFee                      = null,
                         TimeSpan?       ParkingFeePeriod                = null,
                         UInt16?         CarbonDioxideEmission           = null,
                         PercentageByte?  RenewableGenerationPercentage   = null)
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


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomPriceRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceRuleParser">A delegate to parse custom price rules.</param>
        public static PriceRule Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<PriceRule>?  CustomPriceRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var priceRule,
                         out var errorResponse,
                         CustomPriceRuleParser) &&
                priceRule is not null)
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
        public static Boolean TryParse(JObject         JSON,
                                       out PriceRule?  PriceRule,
                                       out String?     ErrorResponse)

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
        /// <param name="CustomPriceRuleParser">A delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out PriceRule?                           PriceRule,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<PriceRule>?  CustomPriceRuleParser)
        {

            try
            {

                PriceRule = null;

                #region PowerRangeStart                  [mandatory]

                if (!JSON.ParseMandatory("powerRangeStart",
                                         "power range start",
                                         out Decimal PowerRangeStart,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EnergyFee                        [mandatory]

                if (!JSON.ParseMandatory("energyFee",
                                         "energy fee",
                                         out Decimal EnergyFee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ParkingFee                       [optional]

                if (JSON.ParseOptional("parkingFee",
                                       "parking fee",
                                       out Decimal? ParkingFee,
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
                                       out PercentageByte? RenewableGenerationPercentage,
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

                                 new JProperty("powerRangeStart",                 PowerRangeStart),
                                 new JProperty("energyFee",                       EnergyFee),

                           ParkingFee is not null
                               ? new JProperty("parkingFee",                      ParkingFee.Value)
                               : null,

                           ParkingFeePeriod.HasValue
                               ? new JProperty("parkingFeePeriod",                (UInt64) Math.Round(ParkingFeePeriod.Value.TotalSeconds, 0))
                               : null,

                           CarbonDioxideEmission.HasValue
                               ? new JProperty("carbonDioxideEmission",           CarbonDioxideEmission.Value)
                               : null,

                           RenewableGenerationPercentage.HasValue
                               ? new JProperty("renewableGenerationPercentage",   RenewableGenerationPercentage.Value)
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
