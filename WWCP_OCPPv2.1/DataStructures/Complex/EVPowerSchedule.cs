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
    /// The EV power schedule.
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVPowerSchedule)
    /// </summary>
    public class EVPowerSchedule : ACustomData,
                                   IEquatable<EVPowerSchedule>
    {

        #region Properties

        /// <summary>
        /// The starting time of the energy offer.
        /// </summary>
        [Mandatory]
        public DateTimeOffset                     TimeAnchor                { get; }

        /// <summary>
        /// The enumeration of power schedule entries offered for discharging.
        /// </summary>
        [Mandatory]
        public IEnumerable<EVPowerScheduleEntry>  EVPowerScheduleEntries    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new power schedule.
        /// </summary>
        /// <param name="TimeAnchor">An starting time of the energy offer.</param>
        /// <param name="EVPowerScheduleEntries">An enumeration of power schedule entries offered for discharging (max 1024).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVPowerSchedule(DateTimeOffset                     TimeAnchor,
                               IEnumerable<EVPowerScheduleEntry>  EVPowerScheduleEntries,
                               CustomData?                        CustomData   = null)

            : base(CustomData)

        {

            if (!EVPowerScheduleEntries.Any())
                throw new ArgumentException("The given enumeration of power schedule entries must not be empty!",
                                            nameof(EVPowerScheduleEntries));

            this.TimeAnchor              = TimeAnchor;
            this.EVPowerScheduleEntries  = EVPowerScheduleEntries;

            unchecked
            {

                hashCode = TimeAnchor.            GetHashCode()  * 5 ^
                           EVPowerScheduleEntries.CalcHashCode() * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Schedule of EV energy offer.",
        //     "javaType": "EVPowerSchedule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evPowerScheduleEntries": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/EVPowerScheduleEntryType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 1024
        //         },
        //         "timeAnchor": {
        //             "description": "The time that defines the starting point for the EVEnergyOffer.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "timeAnchor",
        //         "evPowerScheduleEntries"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEVPowerScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of an ev power schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVPowerScheduleParser">A delegate to parse custom power schedule JSON objects.</param>
        public static EVPowerSchedule Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<EVPowerSchedule>?  CustomEVPowerScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var evPowerSchedule,
                         out var errorResponse,
                         CustomEVPowerScheduleParser))
            {
                return evPowerSchedule;
            }

            throw new ArgumentException("The given JSON representation of an ev power schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVPowerSchedule, out ErrorResponse, CustomEVPowerScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an ev power schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPowerSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out EVPowerSchedule?  EVPowerSchedule,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out EVPowerSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an ev power schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPowerSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVPowerScheduleParser">A delegate to parse custom power schedule JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out EVPowerSchedule?      EVPowerSchedule,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<EVPowerSchedule>?  CustomEVPowerScheduleParser)
        {

            try
            {

                EVPowerSchedule = default;

                #region TimeAnchor                [mandatory]

                if (!JSON.ParseMandatory("timeAnchor",
                                         "time anchor",
                                         out DateTime TimeAnchor,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVPowerScheduleEntries    [mandatory]

                if (!JSON.ParseMandatoryHashSet("evPowerScheduleEntries",
                                                "ev power schedule entries",
                                                EVPowerScheduleEntry.TryParse,
                                                out HashSet<EVPowerScheduleEntry> EVPowerScheduleEntries,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                [optional]

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


                EVPowerSchedule = new EVPowerSchedule(
                                      TimeAnchor,
                                      EVPowerScheduleEntries,
                                      CustomData
                                  );

                if (CustomEVPowerScheduleParser is not null)
                    EVPowerSchedule = CustomEVPowerScheduleParser(JSON,
                                                                  EVPowerSchedule);

                return true;

            }
            catch (Exception e)
            {
                EVPowerSchedule  = default;
                ErrorResponse    = "The given JSON representation of an ev power schedule is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVPowerScheduleSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVPowerSchedule>?       CustomEVPowerScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?  CustomEVPowerScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timeAnchor",             TimeAnchor.ToISO8601()),
                                 new JProperty("powerScheduleEntries",   new JArray(EVPowerScheduleEntries.Select(evPowerScheduleEntry => evPowerScheduleEntry.ToJSON(CustomEVPowerScheduleEntrySerializer,
                                                                                                                                                                      CustomCustomDataSerializer)))),
 
                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVPowerScheduleSerializer is not null
                       ? CustomEVPowerScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVPowerSchedule1, EVPowerSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPowerSchedule1">An ev power schedule.</param>
        /// <param name="EVPowerSchedule2">Another ev power schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVPowerSchedule? EVPowerSchedule1,
                                           EVPowerSchedule? EVPowerSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVPowerSchedule1, EVPowerSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (EVPowerSchedule1 is null || EVPowerSchedule2 is null)
                return false;

            return EVPowerSchedule1.Equals(EVPowerSchedule2);

        }

        #endregion

        #region Operator != (EVPowerSchedule1, EVPowerSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPowerSchedule1">An ev power schedule.</param>
        /// <param name="EVPowerSchedule2">Another ev power schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVPowerSchedule? EVPowerSchedule1,
                                           EVPowerSchedule? EVPowerSchedule2)

            => !(EVPowerSchedule1 == EVPowerSchedule2);

        #endregion

        #endregion

        #region IEquatable<EVPowerSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two power schedules for equality..
        /// </summary>
        /// <param name="Object">An ev power schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVPowerSchedule evPowerSchedule &&
                   Equals(evPowerSchedule);

        #endregion

        #region Equals(EVPowerSchedule)

        /// <summary>
        /// Compares two power schedules for equality.
        /// </summary>
        /// <param name="EVPowerSchedule">An ev power schedule to compare with.</param>
        public Boolean Equals(EVPowerSchedule? EVPowerSchedule)

            => EVPowerSchedule is not null &&

               TimeAnchor.Equals(EVPowerSchedule.TimeAnchor) &&

               EVPowerScheduleEntries.Count().Equals(EVPowerSchedule.EVPowerScheduleEntries.Count()) &&
               EVPowerScheduleEntries.All(powerScheduleEntry => EVPowerSchedule.EVPowerScheduleEntries.Contains(powerScheduleEntry)) &&

               base.Equals(EVPowerSchedule);

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

            => $"{TimeAnchor.ToISO8601()}, {EVPowerScheduleEntries.Count()} power schedule entry/entries";

        #endregion

    }

}
