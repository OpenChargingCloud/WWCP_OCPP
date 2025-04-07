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
    /// Cost(s).
    /// </summary>
    public class Cost : ACustomData,
                        IEquatable<Cost>
    {

        #region Properties

        /// <summary>
        /// The kind of cost referred to in the message element amount.
        /// </summary>
        [Mandatory]
        public CostKinds  CostKind            { get; }

        /// <summary>
        /// The estimated or actual cost per kWh.
        /// </summary>
        [Mandatory]
        public UInt32     Amount              { get; }

        /// <summary>
        /// The optional amount multiplier defines the exponent to base 10 (dec): finalAmount = amount * 10 ^ amountMultiplier.
        /// [min -3, max +3]
        /// </summary>
        [Optional]
        public Int16?     AmountMultiplier    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cost.
        /// </summary>
        /// <param name="CostKind">The kind of cost referred to in the message element amount.</param>
        /// <param name="Amount">The estimated or actual cost per kWh.</param>
        /// <param name="AmountMultiplier">The optional amount multiplier defines the exponent to base 10 (dec): finalAmount = amount * 10 ^ amountMultiplier. [min -3, max +3]</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Cost(CostKinds    CostKind,
                    UInt32       Amount,
                    Int16?       AmountMultiplier   = null,
                    CustomData?  CustomData         = null)

            : base(CustomData)

        {

            this.CostKind          = CostKind;
            this.Amount            = Amount;
            this.AmountMultiplier  = AmountMultiplier;

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "Cost",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "costKind": {
        //             "$ref": "#/definitions/CostKindEnumType"
        //         },
        //         "amount": {
        //             "description": "The estimated or actual cost per kWh",
        //             "type": "integer"
        //         },
        //         "amountMultiplier": {
        //             "description": "Values: -3..3, The amountMultiplier defines the exponent to base 10 (dec). The final value is determined by: amount * 10 ^ amountMultiplier",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "costKind",
        //         "amount"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCostParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCostParser">An optional delegate to parse custom costs.</param>
        public static Cost Parse(JObject                             JSON,
                                 CustomJObjectParserDelegate<Cost>?  CustomCostParser   = null)
        {

            if (TryParse(JSON,
                         out var cost,
                         out var errorResponse,
                         CustomCostParser))
            {
                return cost;
            }

            throw new ArgumentException("The given JSON representation of a cost is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(CostJSON, out Cost, out ErrorResponse, CustomCostParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Cost">The parsed cost.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Cost?    Cost,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out Cost,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cost.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Cost">The parsed cost.</param>
        /// <param name="CustomCostParser">An optional delegate to parse custom costs.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Cost?      Cost,
                                       [NotNullWhen(false)] out String?    ErrorResponse,
                                       CustomJObjectParserDelegate<Cost>?  CustomCostParser   = null)
        {

            try
            {

                Cost = default;

                #region CostKind            [mandatory]

                if (!JSON.ParseMandatory("costKind",
                                         "cost kind",
                                         CostKindsExtensions.TryParse,
                                         out CostKinds CostKind,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Amount              [mandatory]

                if (!JSON.ParseMandatory("amount",
                                         "amount",
                                         out UInt32 Amount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AmountMultiplier    [optional]

                if (JSON.ParseOptional("amountMultiplier",
                                       "amount multiplier",
                                       out Int16? AmountMultiplier,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData          [optional]

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


                Cost = new Cost(
                           CostKind,
                           Amount,
                           AmountMultiplier,
                           CustomData
                       );

                if (CustomCostParser is not null)
                    Cost = CustomCostParser(JSON,
                                            Cost);

                return true;

            }
            catch (Exception e)
            {
                Cost           = default;
                ErrorResponse  = "The given JSON representation of a cost is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Cost>?        CustomCostSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("costKind",           CostKind.AsText()),
                                 new JProperty("amount",             Amount),

                           AmountMultiplier.HasValue
                               ? new JProperty("amountMultiplier",   AmountMultiplier.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCostSerializer is not null
                       ? CustomCostSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Cost1, Cost2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cost1">A cost.</param>
        /// <param name="Cost2">Another cost.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Cost? Cost1,
                                           Cost? Cost2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Cost1, Cost2))
                return true;

            // If one is null, but not both, return false.
            if (Cost1 is null || Cost2 is null)
                return false;

            return Cost1.Equals(Cost2);

        }

        #endregion

        #region Operator != (Cost1, Cost2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cost1">A cost.</param>
        /// <param name="Cost2">Another cost.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Cost? Cost1,
                                           Cost? Cost2)

            => !(Cost1 == Cost2);

        #endregion

        #endregion

        #region IEquatable<Cost> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two costs for equality.
        /// </summary>
        /// <param name="Object">A cost to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Cost cost &&
                   Equals(cost);

        #endregion

        #region Equals(Cost)

        /// <summary>
        /// Compares two costs for equality.
        /// </summary>
        /// <param name="Cost">A cost to compare with.</param>
        public Boolean Equals(Cost? Cost)

            => Cost is not null &&

               CostKind.Equals(Cost.CostKind) &&
               Amount.  Equals(Cost.Amount)   &&

            ((!AmountMultiplier.HasValue && !Cost.AmountMultiplier.HasValue) ||
               AmountMultiplier.HasValue &&  Cost.AmountMultiplier.HasValue && AmountMultiplier.Value.Equals(Cost.AmountMultiplier.Value)) &&

               base.Equals(Cost);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return CostKind.         GetHashCode()       * 7 ^
                       Amount.           GetHashCode()       * 5 ^
                      (AmountMultiplier?.GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   AmountMultiplier.HasValue
                       ? (Amount * 10 ^ AmountMultiplier).ToString()
                       :  Amount.                         ToString(),

                   " per kWh for ",

                   CostKind.AsText()

               );

        #endregion

    }

}
