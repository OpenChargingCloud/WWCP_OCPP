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

using cloud.charging.open.protocols.WWCP;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

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
        /// Maximum transactionLimits allowed transactionLimits(s) for this transaction in currency used for transaction.
        /// </summary>
        [Mandatory]
        public Decimal?   MaxCost      { get; }

        /// <summary>
        /// Maximum energy allowed to charge during this transaction.
        /// </summary>
        [Mandatory]
        public WattHour?  MaxEnergy    { get; }

        /// <summary>
        /// Maximum time allowed to charge during this transaction.
        /// </summary>
        [Optional]
        public TimeSpan?  MaxTime      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new transaction limits.
        /// </summary>
        /// <param name="MaxCost">Maximum transactionLimits allowed transactionLimits(s) for this transaction in currency used for transaction.</param>
        /// <param name="MaxEnergy">Maximum energy allowed to charge during this transaction.</param>
        /// <param name="MaxTime">Maximum time allowed to charge during this transaction.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public TransactionLimits(Decimal?     MaxCost      = null,
                                 WattHour?    MaxEnergy    = null,
                                 TimeSpan?    MaxTime      = null,
                                 CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.MaxCost    = MaxCost;
            this.MaxEnergy  = MaxEnergy;
            this.MaxTime    = MaxTime;


            unchecked
            {

                hashCode = (MaxCost?.  GetHashCode() ?? 0) * 7 ^
                           (MaxEnergy?.GetHashCode() ?? 0) * 5 ^
                           (MaxTime?.  GetHashCode() ?? 0) * 3 ^

                           base.       GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // tba.

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
                         CustomTransactionLimitsParser) &&
                transactionLimits is not null)
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
        public static Boolean TryParse(JObject                 JSON,
                                       out TransactionLimits?  TransactionLimits,
                                       out String?             ErrorResponse)

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
                                       out TransactionLimits?                           TransactionLimits,
                                       out String?                                      ErrorResponse,
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
                       : ""

               }.AggregateWith(", ");

        #endregion

    }

}
