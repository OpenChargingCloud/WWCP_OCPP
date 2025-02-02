﻿/*
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
    /// The EV absolute price schedule.
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
        public PriceAlgorithm                             PriceAlgorithm                    { get; }

        /// <summary>
        /// The enumeration of EV absolute price schedule entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<EVAbsolutePriceScheduleEntry>  EVAbsolutePriceScheduleEntries    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ev absolute price schedule.
        /// </summary>
        /// <param name="TimeAnchor">A time anchor.</param>
        /// <param name="Currency">A currency (ISO 4217).</param>
        /// <param name="PriceAlgorithm">A price algorithm.</param>
        /// <param name="EVAbsolutePriceScheduleEntries">An enumeration of EV absolute price schedule entries (max 1024).</param>
        public EVAbsolutePriceSchedule(DateTime                                   TimeAnchor,
                                       Currency                                   Currency,
                                       PriceAlgorithm                             PriceAlgorithm,
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

        // {
        //     "description": "Price schedule of EV energy offer.",
        //     "javaType": "EVAbsolutePriceSchedule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "timeAnchor": {
        //             "description": "Starting point in time of the EVEnergyOffer.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "currency": {
        //             "description": "Currency code according to ISO 4217.",
        //             "type": "string",
        //             "maxLength": 3
        //         },
        //         "evAbsolutePriceScheduleEntries": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/EVAbsolutePriceScheduleEntryType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 1024
        //         },
        //         "priceAlgorithm": {
        //             "description": "ISO 15118-20 URN of price algorithm: Power, PeakPower, StackedEnergy.",
        //             "type": "string",
        //             "maxLength": 2000
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "timeAnchor",
        //         "currency",
        //         "priceAlgorithm",
        //         "evAbsolutePriceScheduleEntries"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEVAbsolutePriceScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ev absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVAbsolutePriceScheduleParser">A delegate to parse custom ev absolute price schedule JSON objects.</param>
        public static EVAbsolutePriceSchedule Parse(JObject                                                JSON,
                                                    CustomJObjectParserDelegate<EVAbsolutePriceSchedule>?  CustomEVAbsolutePriceScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var evAbsolutePriceSchedule,
                         out var errorResponse,
                         CustomEVAbsolutePriceScheduleParser))
            {
                return evAbsolutePriceSchedule;
            }

            throw new ArgumentException("The given JSON representation of a ev absolute price schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVAbsolutePriceSchedule, out ErrorResponse, CustomEVAbsolutePriceScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ev absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out EVAbsolutePriceSchedule?  EVAbsolutePriceSchedule,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)

            => TryParse(JSON,
                        out EVAbsolutePriceSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ev absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVAbsolutePriceScheduleParser">A delegate to parse custom ev absolute price schedule JSON objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out EVAbsolutePriceSchedule?      EVAbsolutePriceSchedule,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
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
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceAlgorithm                    [mandatory]

                if (!JSON.ParseMandatory("priceAlgorithm",
                                         "price lgorithm",
                                         OCPPv2_1.PriceAlgorithm.TryParse,
                                         out PriceAlgorithm PriceAlgorithm,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
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
                ErrorResponse            = "The given JSON representation of a ev absolute price schedule is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVAbsolutePriceScheduleSerializer = null, CustomEVAbsolutePriceScheduleEntrySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           new JProperty("timeAnchor",                       TimeAnchor.ToIso8601()),
                           new JProperty("currency",                         this.Currency.ISOCode),
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
        /// <param name="EVAbsolutePriceSchedule1">A ev absolute price schedule.</param>
        /// <param name="EVAbsolutePriceSchedule2">Another ev absolute price schedule.</param>
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
        /// <param name="EVAbsolutePriceSchedule1">A ev absolute price schedule.</param>
        /// <param name="EVAbsolutePriceSchedule2">Another ev absolute price schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVAbsolutePriceSchedule? EVAbsolutePriceSchedule1,
                                           EVAbsolutePriceSchedule? EVAbsolutePriceSchedule2)

            => !(EVAbsolutePriceSchedule1 == EVAbsolutePriceSchedule2);

        #endregion

        #endregion

        #region IEquatable<EVAbsolutePriceSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ev absolute price schedules for equality..
        /// </summary>
        /// <param name="Object">A ev absolute price schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVAbsolutePriceSchedule evAbsolutePriceSchedule &&
                   Equals(evAbsolutePriceSchedule);

        #endregion

        #region Equals(EVAbsolutePriceSchedule)

        /// <summary>
        /// Compares two ev absolute price schedules for equality.
        /// </summary>
        /// <param name="EVAbsolutePriceSchedule">A ev absolute price schedule to compare with.</param>
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
