/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The EV energy offer.
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVAbsolutePriceSchedule)
    /// </summary>
    public class EVAbsolutePriceSchedule : ACustomData,
                                           IEquatable<EVAbsolutePriceSchedule>
    {

        #region Properties

        /// <summary>
        /// The time anchor.
        /// </summary>
        [Mandatory]
        public DateTime                                   TimeAnchor                        { get; }

        /// <summary>
        /// The currency (ISO 4217).
        /// </summary>
        [Mandatory]
        public Currency                                   Currency                          { get; }

        /// <summary>
        /// The price algorithm.
        /// </summary>
        [Mandatory]
        public PriceAlgorithm_Id                          PriceAlgorithm                    { get; }

        /// <summary>
        /// The enumeration of EV price rule stacks.
        /// </summary>
        [Mandatory]
        public IEnumerable<EVAbsolutePriceScheduleEntry>  EVAbsolutePriceScheduleEntries    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging limit.
        /// </summary>
        /// <param name="TimeAnchor">A time anchor.</param>
        /// <param name="Currency">A currency (ISO 4217).</param>
        /// <param name="PriceAlgorithm">A price algorithm.</param>
        /// <param name="EVAbsolutePriceScheduleEntries">An enumeration of EV price rule stacks (max 1024).</param>
        public EVAbsolutePriceSchedule(DateTime                                   TimeAnchor,
                                       Currency                                   Currency,
                                       PriceAlgorithm_Id                          PriceAlgorithm,
                                       IEnumerable<EVAbsolutePriceScheduleEntry>  EVAbsolutePriceScheduleEntries,
                                       CustomData?                                CustomData   = null)

            : base(CustomData)

        {

            this.TimeAnchor                      = TimeAnchor;
            this.Currency                        = Currency;
            this.PriceAlgorithm                  = PriceAlgorithm;
            this.EVAbsolutePriceScheduleEntries  = EVAbsolutePriceScheduleEntries;

            unchecked
            {

                hashCode = TimeAnchor.                    GetHashCode()  * 11 ^
                           Currency.                      GetHashCode()  *  7 ^
                           PriceAlgorithm.                GetHashCode()  *  5 ^
                           EVAbsolutePriceScheduleEntries.CalcHashCode() *  3 ^

                           base.                          GetHashCode();

            }

        }

        #endregion


        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomEVAbsolutePriceScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVAbsolutePriceScheduleParser">A delegate to parse custom charging limit JSON objects.</param>
        public static EVAbsolutePriceSchedule Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<EVAbsolutePriceSchedule>?  CustomEVAbsolutePriceScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var evAbsolutePriceSchedule,
                         out var errorResponse,
                         CustomEVAbsolutePriceScheduleParser))
            {
                return evAbsolutePriceSchedule!;
            }

            throw new ArgumentException("The given JSON representation of a charging limit is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVAbsolutePriceSchedule, out ErrorResponse, CustomEVAbsolutePriceScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out EVAbsolutePriceSchedule?  EVAbsolutePriceSchedule,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out EVAbsolutePriceSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVAbsolutePriceScheduleParser">A delegate to parse custom charging limit JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out EVAbsolutePriceSchedule?                           EVAbsolutePriceSchedule,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<EVAbsolutePriceSchedule>?  CustomEVAbsolutePriceScheduleParser)
        {

            try
            {

                EVAbsolutePriceSchedule = default;

                #region TimeAnchor                        [mandatory]

                if (!JSON.ParseMandatory("timeAnchor",
                                         "time anchor",
                                         out DateTime TimeAnchor,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Currency                          [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency? Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (Currency is null)
                    return false;

                #endregion

                #region PriceAlgorithm                    [mandatory]

                if (!JSON.ParseMandatory("priceAlgorithm",
                                         "price lgorithm",
                                         PriceAlgorithm_Id.TryParse,
                                         out PriceAlgorithm_Id PriceAlgorithm,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVAbsolutePriceScheduleEntries    [mandatory]

                if (!JSON.ParseMandatoryJSON("evPriceRuleStack",
                                             "EV price rule stack",
                                             EVAbsolutePriceScheduleEntry.TryParse,
                                             out IEnumerable<EVAbsolutePriceScheduleEntry>? EVAbsolutePriceScheduleEntries,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                EVAbsolutePriceSchedule = new EVAbsolutePriceSchedule(
                                              TimeAnchor,
                                              Currency,
                                              PriceAlgorithm,
                                              EVAbsolutePriceScheduleEntries,
                                              CustomData
                                          );

                if (CustomEVAbsolutePriceScheduleParser is not null)
                    EVAbsolutePriceSchedule = CustomEVAbsolutePriceScheduleParser(JSON,
                                                                                  EVAbsolutePriceSchedule);

                return true;

            }
            catch (Exception e)
            {
                EVAbsolutePriceSchedule  = default;
                ErrorResponse            = "The given JSON representation of a charging limit is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVAbsolutePriceScheduleSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           new JProperty("timeAnchor",                       TimeAnchor.ToIso8601()),
                           new JProperty("currency",                         Currency.ISOCode),
                           new JProperty("priceAlgorithm",                   PriceAlgorithm.ToString()),
                           new JProperty("evAbsolutePriceScheduleEntries",   new JArray(EVAbsolutePriceScheduleEntries.Select(evAbsolutePriceScheduleEntry => evAbsolutePriceScheduleEntry.ToJSON(CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                                                                                                                                                  CustomEVPriceRuleSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",                 CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVAbsolutePriceScheduleSerializer is not null
                       ? CustomEVAbsolutePriceScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVAbsolutePriceSchedule1, EVAbsolutePriceSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVAbsolutePriceSchedule1">A charging limit.</param>
        /// <param name="EVAbsolutePriceSchedule2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVAbsolutePriceSchedule? EVAbsolutePriceSchedule1,
                                           EVAbsolutePriceSchedule? EVAbsolutePriceSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVAbsolutePriceSchedule1, EVAbsolutePriceSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (EVAbsolutePriceSchedule1 is null || EVAbsolutePriceSchedule2 is null)
                return false;

            return EVAbsolutePriceSchedule1.Equals(EVAbsolutePriceSchedule2);

        }

        #endregion

        #region Operator != (EVAbsolutePriceSchedule1, EVAbsolutePriceSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVAbsolutePriceSchedule1">A charging limit.</param>
        /// <param name="EVAbsolutePriceSchedule2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVAbsolutePriceSchedule? EVAbsolutePriceSchedule1,
                                           EVAbsolutePriceSchedule? EVAbsolutePriceSchedule2)

            => !(EVAbsolutePriceSchedule1 == EVAbsolutePriceSchedule2);

        #endregion

        #endregion

        #region IEquatable<EVAbsolutePriceSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging limits for equality..
        /// </summary>
        /// <param name="Object">A charging limit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVAbsolutePriceSchedule evAbsolutePriceSchedule &&
                   Equals(evAbsolutePriceSchedule);

        #endregion

        #region Equals(EVAbsolutePriceSchedule)

        /// <summary>
        /// Compares two charging limits for equality.
        /// </summary>
        /// <param name="EVAbsolutePriceSchedule">A charging limit to compare with.</param>
        public Boolean Equals(EVAbsolutePriceSchedule? EVAbsolutePriceSchedule)

            => EVAbsolutePriceSchedule is not null &&

               TimeAnchor.    Equals(EVAbsolutePriceSchedule.TimeAnchor)     &&
               Currency.      Equals(EVAbsolutePriceSchedule.Currency)       &&
               PriceAlgorithm.Equals(EVAbsolutePriceSchedule.PriceAlgorithm) &&

               EVAbsolutePriceScheduleEntries.Count().Equals(EVAbsolutePriceSchedule.EVAbsolutePriceScheduleEntries.Count()) &&
               EVAbsolutePriceScheduleEntries.All(evPriceRuleStack => EVAbsolutePriceSchedule.EVAbsolutePriceScheduleEntries.Contains(evPriceRuleStack)) &&

               base.Equals(EVAbsolutePriceSchedule);

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

            => $"{TimeAnchor}, {Currency.ISOCode}, {PriceAlgorithm}, {EVAbsolutePriceScheduleEntries.Count()} EV absolute price schedule entry/entries";

        #endregion

    }

}
