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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An entry in price schedule over time for which EV is willing to discharge.
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVPriceRule)
    /// </summary>
    public class EVPriceRule : ACustomData,
                               IEquatable<EVPriceRule>
    {

        #region Properties

        /// <summary>
        /// The energy fee/cost per kWh.
        /// </summary>
        [Mandatory]
        public Decimal  EnergyFee          { get; }

        /// <summary>
        /// The EnergyFee applies between this value and the value of the
        /// PowerRangeStart of the subsequent EVPriceRule.If the power is below this
        /// value, the EnergyFee of the previous EVPriceRule applies.
        /// </summary>
        [Mandatory]
        public Watt     PowerRangeStart    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EV price rule.
        /// </summary>
        /// <param name="EnergyFee">The energy fee/cost per kWh.</param>
        /// <param name="PowerRangeStart">The EnergyFee applies between this value and the value of the PowerRangeStart of the subsequent EVPriceRule.If the power is below this value, the EnergyFee of the previous EVPriceRule applies.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVPriceRule(Decimal      EnergyFee,
                           Watt         PowerRangeStart,
                           CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.EnergyFee        = EnergyFee;
            this.PowerRangeStart  = PowerRangeStart;

            unchecked
            {

                hashCode = EnergyFee.      GetHashCode() * 5 ^
                           PowerRangeStart.GetHashCode() * 3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "An entry in price schedule over time for which EV is willing to discharge.",
        //     "javaType": "EVPriceRule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "energyFee": {
        //             "description": "Cost per kWh.",
        //             "type": "number"
        //         },
        //         "powerRangeStart": {
        //             "description": "The EnergyFee applies between this value and the value of the PowerRangeStart of the subsequent EVPriceRule.
        //                             If the power is below this value, the EnergyFee of the previous EVPriceRule applies. Negative values are used for discharging.",
        //             "type": "number"
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

        #region (static) Parse   (JSON, CustomEVPriceRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EV price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVPriceRuleParser">A delegate to parse custom EV price rule JSON objects.</param>
        public static EVPriceRule Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<EVPriceRule>?  CustomEVPriceRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var evPriceRule,
                         out var errorResponse,
                         CustomEVPriceRuleParser))
            {
                return evPriceRule;
            }

            throw new ArgumentException("The given JSON representation of an EV price rule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVPriceRule, out ErrorResponse, CustomEVPriceRuleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an EV price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPriceRule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out EVPriceRule?  EVPriceRule,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out EVPriceRule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an EV price rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPriceRule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVPriceRuleParser">A delegate to parse custom EV price rule JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out EVPriceRule?      EVPriceRule,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<EVPriceRule>?  CustomEVPriceRuleParser)
        {

            try
            {

                EVPriceRule = default;

                #region EnergyFee          [mandatory]

                if (!JSON.ParseMandatory("EnergyFee",
                                         "energy fee",
                                         out Decimal EnergyFee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PowerRangeStart    [mandatory]

                if (!JSON.ParseMandatory("PowerRangeStart",
                                         "power range start",
                                         Watt.TryParseKW,
                                         out Watt PowerRangeStart,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                EVPriceRule = new EVPriceRule(
                                  EnergyFee,
                                  PowerRangeStart,
                                  CustomData
                              );

                if (CustomEVPriceRuleParser is not null)
                    EVPriceRule = CustomEVPriceRuleParser(JSON,
                                                          EVPriceRule);

                return true;

            }
            catch (Exception e)
            {
                EVPriceRule    = default;
                ErrorResponse  = "The given JSON representation of an EV price rule is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVPriceRuleSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom EV price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVPriceRule>?  CustomEVPriceRuleSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("energyFee",         EnergyFee),
                                 new JProperty("powerRangeStart",   PowerRangeStart.kW),

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVPriceRuleSerializer is not null
                       ? CustomEVPriceRuleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVPriceRule1, EVPriceRule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPriceRule1">An EV price rule.</param>
        /// <param name="EVPriceRule2">Another EV price rule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVPriceRule? EVPriceRule1,
                                           EVPriceRule? EVPriceRule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVPriceRule1, EVPriceRule2))
                return true;

            // If one is null, but not both, return false.
            if (EVPriceRule1 is null || EVPriceRule2 is null)
                return false;

            return EVPriceRule1.Equals(EVPriceRule2);

        }

        #endregion

        #region Operator != (EVPriceRule1, EVPriceRule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPriceRule1">An EV price rule.</param>
        /// <param name="EVPriceRule2">Another EV price rule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVPriceRule? EVPriceRule1,
                                           EVPriceRule? EVPriceRule2)

            => !(EVPriceRule1 == EVPriceRule2);

        #endregion

        #endregion

        #region IEquatable<EVPriceRule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EV price rules for equality..
        /// </summary>
        /// <param name="Object">An EV price rule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVPriceRule evPriceRule &&
                   Equals(evPriceRule);

        #endregion

        #region Equals(EVPriceRule)

        /// <summary>
        /// Compares two EV price rules for equality.
        /// </summary>
        /// <param name="EVPriceRule">An EV price rule to compare with.</param>
        public Boolean Equals(EVPriceRule? EVPriceRule)

            => EVPriceRule is not null &&

               EnergyFee.      Equals(EVPriceRule.EnergyFee)       &&
               PowerRangeStart.Equals(EVPriceRule.PowerRangeStart) &&

               base.           Equals(EVPriceRule);

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

            => $"{EnergyFee} >= {PowerRangeStart.kW} kW";

        #endregion

    }

}
