/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Consumption cost(s).
    /// </summary>
    public class ConsumptionCost : ACustomData,
                                   IEquatable<ConsumptionCost>
    {

        #region Properties

        /// <summary>
        /// The lowest level of consumption that defines the starting point of this consumption block.
        /// The block interval extends to the start of the next interval.
        /// </summary>
        [Mandatory]
        public Decimal            StartValue    { get; }

        /// <summary>
        /// Detailed information about the costs.
        /// [max 3]
        /// </summary>
        [Mandatory]
        public IEnumerable<Cost>  Costs         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new consumption cost.
        /// </summary>
        /// <param name="StartValue">The lowest level of consumption that defines the starting point of this consumption block. The block interval extends to the start of the next interval.</param>
        /// <param name="Costs">Detailed information about the costs.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ConsumptionCost(Decimal            StartValue,
                               IEnumerable<Cost>  Costs,
                               CustomData?        CustomData   = null)

            : base(CustomData)

        {

            this.StartValue  = StartValue;
            this.Costs       = Costs;

        }

        #endregion


        #region Documentation

        // "ConsumptionCostType": {
        //   "description": "Consumption_ Cost\r\nurn:x-oca:ocpp:uid:2:233259\r\n",
        //   "javaType": "ConsumptionCost",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "startValue": {
        //       "description": "Consumption_ Cost. Start_ Value. Numeric\r\nurn:x-oca:ocpp:uid:1:569246\r\nThe lowest level of consumption that defines the starting point of this consumption block. The block interval extends to the start of the next interval.\r\n",
        //       "type": "number"
        //     },
        //     "cost": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/CostType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 3
        //     }
        //   },
        //   "required": [
        //     "startValue",
        //     "cost"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomConsumptionCostParser = null)

        /// <summary>
        /// Parse the given JSON representation of a consumption cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomConsumptionCostParser">A delegate to parse custom consumption costs.</param>
        public static ConsumptionCost Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<ConsumptionCost>?  CustomConsumptionCostParser   = null)
        {

            if (TryParse(JSON,
                         out var consumptionCost,
                         out var errorResponse,
                         CustomConsumptionCostParser))
            {
                return consumptionCost!;
            }

            throw new ArgumentException("The given JSON representation of a consumption cost is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(ConsumptionCostJSON, out ConsumptionCost, out ErrorResponse, CustomConsumptionCostParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a consumption cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ConsumptionCost">The parsed consumption cost.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out ConsumptionCost?  ConsumptionCost,
                                       out String?           ErrorResponse)

            => TryParse(JSON,
                        out ConsumptionCost,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a consumption cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ConsumptionCost">The parsed consumption cost.</param>
        /// <param name="CustomConsumptionCostParser">A delegate to parse custom consumption costs.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out ConsumptionCost?                           ConsumptionCost,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<ConsumptionCost>?  CustomConsumptionCostParser   = null)
        {

            try
            {

                ConsumptionCost = default;

                #region StartValue    [mandatory]

                if (!JSON.ParseMandatory("startValue",
                                         "start value",
                                         out Decimal StartValue,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Costs         [mandatory]

                if (!JSON.ParseMandatoryJSON("cost",
                                             "cost(s)",
                                             Cost.TryParse,
                                             out IEnumerable<Cost> Costs,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ConsumptionCost = new ConsumptionCost(StartValue,
                                                      Costs,
                                                      CustomData);

                if (CustomConsumptionCostParser is not null)
                    ConsumptionCost = CustomConsumptionCostParser(JSON,
                                                                  ConsumptionCost);

                return true;

            }
            catch (Exception e)
            {
                ConsumptionCost  = default;
                ErrorResponse    = "The given JSON representation of a consumption cost is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConsumptionCostResponseSerializer = null, CustomCostResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConsumptionCostResponseSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostResponseSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConsumptionCost>?  CustomConsumptionCostResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Cost>?             CustomCostResponseSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataResponseSerializer        = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("startValue",  StartValue),
                                 new JProperty("cost",        new JArray(Costs.Select(cost => cost.ToJSON(CustomCostResponseSerializer,
                                                                                                          CustomCustomDataResponseSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomConsumptionCostResponseSerializer is not null
                       ? CustomConsumptionCostResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ConsumptionCost1, ConsumptionCost2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConsumptionCost1">A consumption cost.</param>
        /// <param name="ConsumptionCost2">Another consumption cost.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConsumptionCost? ConsumptionCost1,
                                           ConsumptionCost? ConsumptionCost2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ConsumptionCost1, ConsumptionCost2))
                return true;

            // If one is null, but not both, return false.
            if (ConsumptionCost1 is null || ConsumptionCost2 is null)
                return false;

            return ConsumptionCost1.Equals(ConsumptionCost2);

        }

        #endregion

        #region Operator != (ConsumptionCost1, ConsumptionCost2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConsumptionCost1">A consumption cost.</param>
        /// <param name="ConsumptionCost2">Another consumption cost.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConsumptionCost? ConsumptionCost1,
                                           ConsumptionCost? ConsumptionCost2)

            => !(ConsumptionCost1 == ConsumptionCost2);

        #endregion

        #endregion

        #region IEquatable<ConsumptionCost> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two consumption costs for equality.
        /// </summary>
        /// <param name="Object">A consumption cost to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConsumptionCost consumptionCost &&
                   Equals(consumptionCost);

        #endregion

        #region Equals(ConsumptionCost)

        /// <summary>
        /// Compares two consumption costs for equality.
        /// </summary>
        /// <param name="ConsumptionCost">A consumption cost to compare with.</param>
        public Boolean Equals(ConsumptionCost? ConsumptionCost)

            => ConsumptionCost is not null &&

               StartValue.Equals(ConsumptionCost.StartValue) &&

               Costs.Count().Equals(ConsumptionCost.Costs.Count())     &&
               Costs.All(cost => ConsumptionCost.Costs.Contains(cost)) &&

               base.Equals(ConsumptionCost);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return StartValue.GetHashCode() * 5 ^
                       //ToDo: Add Costs!

                       base.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => StartValue.ToString();

        #endregion

    }

}
