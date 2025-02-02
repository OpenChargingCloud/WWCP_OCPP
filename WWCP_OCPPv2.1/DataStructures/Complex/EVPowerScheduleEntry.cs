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
    /// The EV power schedule entry.
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVPowerScheduleEntry)
    /// </summary>
    public class EVPowerScheduleEntry : ACustomData,
                                        IEquatable<EVPowerScheduleEntry>
    {

        #region Properties

        /// <summary>
        /// The duration of the given power schedule entry.
        /// </summary>
        [Mandatory]
        public TimeSpan  Duration    { get; }

        /// <summary>
        /// The maximum amount of power for this power schedule entry to be discharged
        /// from the EV battery through EVSE power outlet.
        /// Negative values are used for discharging.
        /// A positive value indicates that the EV currently is not able to offer energy to discharge.
        /// </summary>
        [Mandatory]
        public Watt      Power       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new power schedule entry.
        /// </summary>
        /// <param name="Duration">The duration of the given power schedule entry.</param>
        /// <param name="Power">The maximum amount of power for this power schedule entry to be discharged from the EV battery through EVSE power outlet. Negative values are used for discharging.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVPowerScheduleEntry(TimeSpan     Duration,
                                    Watt         Power,
                                    CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Duration  = Duration;
            this.Power     = Power;

            unchecked
            {

                hashCode = Duration.GetHashCode() * 5 ^
                           Power.   GetHashCode() * 3 ^
                           base.    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "An entry in schedule of the energy amount over time that EV is willing to discharge.
        //                     A negative value indicates the willingness to discharge under specific conditions,
        //                     a positive value indicates that the EV currently is not able to offer energy to discharge.",
        //     "javaType": "EVPowerScheduleEntry",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "duration": {
        //             "description": "The duration of this entry.",
        //             "type": "integer"
        //         },
        //         "power": {
        //             "description": "Defines maximum amount of power for the duration of this EVPowerScheduleEntry to be discharged
        //                             from the EV battery through EVSE power outlet. Negative values are used for discharging.",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "duration",
        //         "power"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEVPowerScheduleEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of an ev power schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVPowerScheduleEntryParser">A delegate to parse custom power schedule entry JSON objects.</param>
        public static EVPowerScheduleEntry Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<EVPowerScheduleEntry>?  CustomEVPowerScheduleEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var evPowerScheduleEntry,
                         out var errorResponse,
                         CustomEVPowerScheduleEntryParser))
            {
                return evPowerScheduleEntry;
            }

            throw new ArgumentException("The given JSON representation of an ev power schedule entry is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVPowerScheduleEntry, out ErrorResponse, CustomEVPowerScheduleEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an ev power schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPowerScheduleEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out EVPowerScheduleEntry?  EVPowerScheduleEntry,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out EVPowerScheduleEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an ev power schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVPowerScheduleEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVPowerScheduleEntryParser">A delegate to parse custom power schedule entry JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out EVPowerScheduleEntry?      EVPowerScheduleEntry,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<EVPowerScheduleEntry>?  CustomEVPowerScheduleEntryParser)
        {

            try
            {

                EVPowerScheduleEntry = default;

                #region Duration      [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out TimeSpan Duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Power         [mandatory]

                if (!JSON.ParseMandatory("power",
                                         "common power or power on phase L1",
                                         Watt.TryParseKW,
                                         out Watt Power,
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


                EVPowerScheduleEntry = new EVPowerScheduleEntry(
                                           Duration,
                                           Power,
                                           CustomData
                                       );

                if (CustomEVPowerScheduleEntryParser is not null)
                    EVPowerScheduleEntry = CustomEVPowerScheduleEntryParser(JSON,
                                                                            EVPowerScheduleEntry);

                return true;

            }
            catch (Exception e)
            {
                EVPowerScheduleEntry  = default;
                ErrorResponse         = "The given JSON representation of an ev power schedule entry is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVPowerScheduleEntrySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?  CustomEVPowerScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("duration",     (UInt64) Math.Round(Duration.TotalSeconds, 0)),

                                 new JProperty("power",        Power.kW),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVPowerScheduleEntrySerializer is not null
                       ? CustomEVPowerScheduleEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVPowerScheduleEntry1, EVPowerScheduleEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPowerScheduleEntry1">An ev power schedule entry.</param>
        /// <param name="EVPowerScheduleEntry2">Another ev power schedule entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVPowerScheduleEntry? EVPowerScheduleEntry1,
                                           EVPowerScheduleEntry? EVPowerScheduleEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVPowerScheduleEntry1, EVPowerScheduleEntry2))
                return true;

            // If one is null, but not both, return false.
            if (EVPowerScheduleEntry1 is null || EVPowerScheduleEntry2 is null)
                return false;

            return EVPowerScheduleEntry1.Equals(EVPowerScheduleEntry2);

        }

        #endregion

        #region Operator != (EVPowerScheduleEntry1, EVPowerScheduleEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVPowerScheduleEntry1">An ev power schedule entry.</param>
        /// <param name="EVPowerScheduleEntry2">Another ev power schedule entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVPowerScheduleEntry? EVPowerScheduleEntry1,
                                           EVPowerScheduleEntry? EVPowerScheduleEntry2)

            => !(EVPowerScheduleEntry1 == EVPowerScheduleEntry2);

        #endregion

        #endregion

        #region IEquatable<EVPowerScheduleEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two power schedule entries for equality..
        /// </summary>
        /// <param name="Object">An ev power schedule entry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVPowerScheduleEntry evPowerScheduleEntry &&
                   Equals(evPowerScheduleEntry);

        #endregion

        #region Equals(EVPowerScheduleEntry)

        /// <summary>
        /// Compares two power schedule entries for equality.
        /// </summary>
        /// <param name="EVPowerScheduleEntry">An ev power schedule entry to compare with.</param>
        public Boolean Equals(EVPowerScheduleEntry? EVPowerScheduleEntry)

            => EVPowerScheduleEntry is not null &&

               Duration.Equals(EVPowerScheduleEntry.Duration) &&
               Power.   Equals(EVPowerScheduleEntry.Power)    &&

               base.    Equals(EVPowerScheduleEntry);

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

            => $"{Power.kW} kW for {Duration.TotalSeconds} second(s)";

        #endregion

    }

}
