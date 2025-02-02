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
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVPriceRuleStack)
    /// </summary>
    public class EVAbsolutePriceScheduleEntry : ACustomData,
                                                IEquatable<EVAbsolutePriceScheduleEntry>
    {

        #region Properties

        /// <summary>
        /// The duration.
        /// </summary>
        [Mandatory]
        public TimeSpan                  Duration        { get; }

        /// <summary>
        /// The enumeration of EV price rules.
        /// [max 8]
        /// </summary>
        [Mandatory]
        public IEnumerable<EVPriceRule>  EVPriceRules    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging limit.
        /// </summary>
        /// <param name="Duration">The duration.</param>
        /// <param name="EVPriceRules">An enumeration of EV price rules (max 8).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVAbsolutePriceScheduleEntry(TimeSpan                  Duration,
                                            IEnumerable<EVPriceRule>  EVPriceRules,
                                            CustomData?               CustomData   = null)

            : base(CustomData)

        {

            this.Duration      = Duration;
            this.EVPriceRules  = EVPriceRules;

            unchecked
            {

                hashCode = Duration.    GetHashCode()  * 5 ^
                           EVPriceRules.CalcHashCode() * 3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "An entry in price schedule over time for which EV is willing to discharge.",
        //     "javaType": "EVAbsolutePriceScheduleEntry",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "duration": {
        //             "description": "The amount of seconds of this entry.",
        //             "type": "integer"
        //         },
        //         "evPriceRule": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/EVPriceRuleType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 8
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "duration",
        //         "evPriceRule"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEVAbsolutePriceScheduleEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntryParser">A delegate to parse custom charging limit JSON objects.</param>
        public static EVAbsolutePriceScheduleEntry Parse(JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var evAbsolutePriceScheduleEntry,
                         out var errorResponse,
                         CustomEVAbsolutePriceScheduleEntryParser))
            {
                return evAbsolutePriceScheduleEntry;
            }

            throw new ArgumentException("The given JSON representation of a charging limit is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVAbsolutePriceScheduleEntry, out ErrorResponse, CustomEVAbsolutePriceScheduleEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceScheduleEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       [NotNullWhen(true)]  out EVAbsolutePriceScheduleEntry?  EVAbsolutePriceScheduleEntry,
                                       [NotNullWhen(false)] out String?                        ErrorResponse)

            => TryParse(JSON,
                        out EVAbsolutePriceScheduleEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVAbsolutePriceScheduleEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntryParser">A delegate to parse custom charging limit JSON objects.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       [NotNullWhen(true)]  out EVAbsolutePriceScheduleEntry?      EVAbsolutePriceScheduleEntry,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntryParser)
        {

            try
            {

                EVAbsolutePriceScheduleEntry = default;

                #region Duration        [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out TimeSpan Duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVPriceRules    [optional]

                if (!JSON.ParseMandatoryHashSet("evPriceRules",
                                                "EV price rules",
                                                EVPriceRule.TryParse,
                                                out HashSet<EVPriceRule> EVPriceRules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

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


                EVAbsolutePriceScheduleEntry = new EVAbsolutePriceScheduleEntry(
                                                   Duration,
                                                   EVPriceRules,
                                                   CustomData
                                               );

                if (CustomEVAbsolutePriceScheduleEntryParser is not null)
                    EVAbsolutePriceScheduleEntry = CustomEVAbsolutePriceScheduleEntryParser(JSON,
                                                                                            EVAbsolutePriceScheduleEntry);

                return true;

            }
            catch (Exception e)
            {
                EVAbsolutePriceScheduleEntry  = default;
                ErrorResponse                 = "The given JSON representation of a charging limit is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVAbsolutePriceScheduleEntrySerializer = null, CustomEVPriceRuleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("duration",       (UInt64) Math.Round(Duration.TotalSeconds, 0)),
                                 new JProperty("evPriceRules",   new JArray(EVPriceRules.Select(evPriceRule => evPriceRule.ToJSON(CustomEVPriceRuleSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVAbsolutePriceScheduleEntrySerializer is not null
                       ? CustomEVAbsolutePriceScheduleEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVAbsolutePriceScheduleEntry1, EVAbsolutePriceScheduleEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVAbsolutePriceScheduleEntry1">A charging limit.</param>
        /// <param name="EVAbsolutePriceScheduleEntry2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVAbsolutePriceScheduleEntry? EVAbsolutePriceScheduleEntry1,
                                           EVAbsolutePriceScheduleEntry? EVAbsolutePriceScheduleEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVAbsolutePriceScheduleEntry1, EVAbsolutePriceScheduleEntry2))
                return true;

            // If one is null, but not both, return false.
            if (EVAbsolutePriceScheduleEntry1 is null || EVAbsolutePriceScheduleEntry2 is null)
                return false;

            return EVAbsolutePriceScheduleEntry1.Equals(EVAbsolutePriceScheduleEntry2);

        }

        #endregion

        #region Operator != (EVAbsolutePriceScheduleEntry1, EVAbsolutePriceScheduleEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVAbsolutePriceScheduleEntry1">A charging limit.</param>
        /// <param name="EVAbsolutePriceScheduleEntry2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVAbsolutePriceScheduleEntry? EVAbsolutePriceScheduleEntry1,
                                           EVAbsolutePriceScheduleEntry? EVAbsolutePriceScheduleEntry2)

            => !(EVAbsolutePriceScheduleEntry1 == EVAbsolutePriceScheduleEntry2);

        #endregion

        #endregion

        #region IEquatable<EVAbsolutePriceScheduleEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging limits for equality..
        /// </summary>
        /// <param name="Object">A charging limit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVAbsolutePriceScheduleEntry evAbsolutePriceScheduleEntry &&
                   Equals(evAbsolutePriceScheduleEntry);

        #endregion

        #region Equals(EVAbsolutePriceScheduleEntry)

        /// <summary>
        /// Compares two charging limits for equality.
        /// </summary>
        /// <param name="EVAbsolutePriceScheduleEntry">A charging limit to compare with.</param>
        public Boolean Equals(EVAbsolutePriceScheduleEntry? EVAbsolutePriceScheduleEntry)

            => EVAbsolutePriceScheduleEntry is not null &&

               Duration.Equals(EVAbsolutePriceScheduleEntry.Duration) &&

               EVPriceRules.Count().Equals(EVAbsolutePriceScheduleEntry.EVPriceRules.Count()) &&
               EVPriceRules.All(evPriceRule => EVAbsolutePriceScheduleEntry.EVPriceRules.Contains(evPriceRule)) &&

               base.Equals(EVAbsolutePriceScheduleEntry);

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

            => $"{Duration.TotalSeconds} second(s), {EVPriceRules.Count()} EV price rule(s)";

        #endregion

    }

}
