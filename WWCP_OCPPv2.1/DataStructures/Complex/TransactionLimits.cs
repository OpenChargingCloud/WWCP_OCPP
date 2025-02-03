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
    /// Transaction limits.
    /// </summary>
    public class TransactionLimits : ACustomData,
                                     IEquatable<TransactionLimits>
    {

        #region Properties

        /// <summary>
        /// The maximum transactionLimits allowed transactionLimits(s) for this transaction in currency used for transaction.
        /// </summary>
        [Mandatory]
        public Decimal?     MaxCost      { get; }

        /// <summary>
        /// The maximum energy allowed to charge during this transaction.
        /// </summary>
        [Mandatory]
        public WattHour?    MaxEnergy    { get; }

        /// <summary>
        /// The maximum time allowed to charge during this transaction.
        /// </summary>
        [Optional]
        public TimeSpan?    MaxTime      { get; }

        /// <summary>
        /// The maximum state-of-charge allowed to charge during this transaction.
        /// </summary>
        [Optional]
        public Percentage?  MaxSoC       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new transaction limits.
        /// </summary>
        /// <param name="MaxCost">The maximum transactionLimits allowed transactionLimits(s) for this transaction in currency used for transaction.</param>
        /// <param name="MaxEnergy">The maximum energy allowed to charge during this transaction.</param>
        /// <param name="MaxTime">The maximum time allowed to charge during this transaction.</param>
        /// <param name="MaxSoC">The maximum state-of-charge allowed to charge during this transaction.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public TransactionLimits(Decimal?     MaxCost      = null,
                                 WattHour?    MaxEnergy    = null,
                                 TimeSpan?    MaxTime      = null,
                                 Percentage?  MaxSoC       = null,
                                 CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.MaxCost    = MaxCost;
            this.MaxEnergy  = MaxEnergy;
            this.MaxTime    = MaxTime;
            this.MaxSoC     = MaxSoC;

            unchecked
            {

                hashCode = (MaxCost?.  GetHashCode() ?? 0) * 11 ^
                           (MaxEnergy?.GetHashCode() ?? 0) *  7 ^
                           (MaxTime?.  GetHashCode() ?? 0) *  5 ^
                           (MaxSoC?.   GetHashCode() ?? 0) *  3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Cost, energy, time or SoC limit for a transaction.",
        //     "javaType": "TransactionLimit",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "maxCost": {
        //             "description": "Maximum allowed cost of transaction in currency of tariff.",
        //             "type": "number"
        //         },
        //         "maxEnergy": {
        //             "description": "Maximum allowed energy in Wh to charge in transaction.",
        //             "type": "number"
        //         },
        //         "maxTime": {
        //             "description": "Maximum duration of transaction in seconds from start to end.",
        //             "type": "integer"
        //         },
        //         "maxSoC": {
        //             "description": "Maximum State of Charge of EV in percentage.",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTransactionLimitsParser = null)

        /// <summary>
        /// Parse the given JSON representation of transaction limits.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTransactionLimitsParser">An optional delegate to parse custom transaction limits.</param>
        public static TransactionLimits Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null)
        {

            if (TryParse(JSON,
                         out var transactionLimits,
                         out var errorResponse,
                         CustomTransactionLimitsParser))
            {
                return transactionLimits;
            }

            throw new ArgumentException("The given JSON representation of transaction limits is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(TransactionLimitsJSON, out TransactionLimits, out ErrorResponse, CustomTransactionLimitsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of transaction limits.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TransactionLimits">The parsed transactionLimits.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out TransactionLimits?  TransactionLimits,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out TransactionLimits,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of transaction limits.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TransactionLimits">The parsed transactionLimits.</param>
        /// <param name="CustomTransactionLimitsParser">An optional delegate to parse custom transaction limits.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out TransactionLimits?      TransactionLimits,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null)
        {

            try
            {

                TransactionLimits = default;

                #region MaxCost       [optional]

                if (JSON.ParseOptional("maxCost",
                                       "maximum cost(s)",
                                       out Decimal? MaxCost,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                //var MaxCost = MaxCostNumber.HasValue
                //                  ? new Price?(new Price(MaxCostNumber.Value))
                //                  : null;

                #endregion

                #region MaxEnergy     [optional]

                if (JSON.ParseOptional("maxEnergy",
                                       "maximum energy",
                                       out Decimal? MaxEnergyNumber,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MaxEnergy = MaxEnergyNumber.HasValue
                                  ? WattHour.TryParseKWh(MaxEnergyNumber.Value)
                                  : null;

                #endregion

                #region MaxTime       [optional]

                if (JSON.ParseOptional("maxTime",
                                       "maximum energy",
                                       out TimeSpan? MaxTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxSoC        [optional]

                if (JSON.ParseOptional("maxSoC",
                                       "maximum state-of-charge",
                                       out Percentage? MaxSoC,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData    [optional]

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


                TransactionLimits = new TransactionLimits(

                                        MaxCost,
                                        MaxEnergy,
                                        MaxTime,
                                        MaxSoC,

                                        CustomData

                                    );

                if (CustomTransactionLimitsParser is not null)
                    TransactionLimits = CustomTransactionLimitsParser(JSON,
                                                                      TransactionLimits);

                return true;

            }
            catch (Exception e)
            {
                TransactionLimits  = default;
                ErrorResponse      = "The given JSON representation of transaction limits is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransactionLimitsSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransactionLimitsSerializer">A delegate to serialize custom transaction limits.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TransactionLimits>?  CustomTransactionLimitsSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           MaxCost.  HasValue
                               ? new JProperty("maxCost",      MaxCost.  Value)
                               : null,

                           MaxEnergy.HasValue
                               ? new JProperty("maxEnergy",    MaxEnergy.Value.Value)
                               : null,

                           MaxTime.  HasValue
                               ? new JProperty("maxTime",      MaxTime.  Value.TotalSeconds)
                               : null,

                           MaxSoC.   HasValue
                               ? new JProperty("maxSoC",       MaxSoC.   Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTransactionLimitsSerializer is not null
                       ? CustomTransactionLimitsSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TransactionLimits1, TransactionLimits2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionLimits1">Transaction Limits.</param>
        /// <param name="TransactionLimits2">Other transaction limits.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TransactionLimits? TransactionLimits1,
                                           TransactionLimits? TransactionLimits2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TransactionLimits1, TransactionLimits2))
                return true;

            // If one is null, but not both, return false.
            if (TransactionLimits1 is null || TransactionLimits2 is null)
                return false;

            return TransactionLimits1.Equals(TransactionLimits2);

        }

        #endregion

        #region Operator != (TransactionLimits1, TransactionLimits2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionLimits1">Transaction Limits.</param>
        /// <param name="TransactionLimits2">Other transaction limits.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TransactionLimits? TransactionLimits1,
                                           TransactionLimits? TransactionLimits2)

            => !(TransactionLimits1 == TransactionLimits2);

        #endregion

        #endregion

        #region IEquatable<TransactionLimits> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction limits for equality.
        /// </summary>
        /// <param name="Object">Transaction limits to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransactionLimits transactionLimits &&
                   Equals(transactionLimits);

        #endregion

        #region Equals(TransactionLimits)

        /// <summary>
        /// Compares two transaction limits for equality.
        /// </summary>
        /// <param name="TransactionLimits">Transaction limits to compare with.</param>
        public Boolean Equals(TransactionLimits? TransactionLimits)

            => TransactionLimits is not null &&

            ((!MaxCost.  HasValue && !TransactionLimits.MaxCost.  HasValue) ||
               MaxCost.  HasValue &&  TransactionLimits.MaxCost.  HasValue && MaxCost.  Value.Equals(TransactionLimits.MaxCost.  Value)) &&

            ((!MaxEnergy.HasValue && !TransactionLimits.MaxEnergy.HasValue) ||
               MaxEnergy.HasValue &&  TransactionLimits.MaxEnergy.HasValue && MaxEnergy.Value.Equals(TransactionLimits.MaxEnergy.Value)) &&

            ((!MaxTime.  HasValue && !TransactionLimits.MaxTime.  HasValue) ||
               MaxTime.  HasValue &&  TransactionLimits.MaxTime.  HasValue && MaxTime.  Value.Equals(TransactionLimits.MaxTime.  Value)) &&

            ((!MaxSoC.   HasValue && !TransactionLimits.MaxSoC.   HasValue) ||
               MaxSoC.   HasValue &&  TransactionLimits.MaxSoC.   HasValue && MaxSoC.   Value.Equals(TransactionLimits.MaxSoC.   Value)) &&

               base.Equals(TransactionLimits);

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

            => new String[] {

                   MaxCost.HasValue
                       ? $"max cost: {MaxCost.Value}"
                       : "",

                   MaxEnergy.HasValue
                       ? $"max. {MaxEnergy} Wh"
                       : "",

                   MaxTime.HasValue
                       ? $"max. {MaxTime.Value.TotalSeconds} seconds"
                       : "",

                   MaxSoC. HasValue
                       ? $"max. {MaxSoC.Value.Value}% SoC"
                       : ""

               }.AggregateWith(", ");

        #endregion

    }

}
