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
    /// A relative time interval.
    /// </summary>
    public class RelativeTimeInterval : ACustomData,
                                        IEquatable<RelativeTimeInterval>
    {

        #region Properties

        /// <summary>
        /// The start of the relative time interval.
        /// </summary>
        [Mandatory]
        public TimeSpan   Start       { get; }

        /// <summary>
        /// The optional duration of the time interval.
        /// </summary>
        [Optional]
        public TimeSpan?  Duration    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new relative time interval.
        /// </summary>
        /// <param name="Start">The start of the relative time interval.</param>
        /// <param name="Duration">An optional duration of the time interval.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public RelativeTimeInterval(TimeSpan     Start,
                                    TimeSpan?    Duration     = null,
                                    CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Start     = Start;
            this.Duration  = Duration;

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "RelativeTimeInterval",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "start": {
        //             "description": "Start of the interval, in seconds from NOW.",
        //             "type": "integer"
        //         },
        //         "duration": {
        //             "description": "Duration of the interval, in seconds.",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "start"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomRelativeTimeIntervalParser = null)

        /// <summary>
        /// Parse the given JSON representation of a relative time interval.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRelativeTimeIntervalParser">An optional delegate to parse custom relative time intervals.</param>
        public static RelativeTimeInterval Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<RelativeTimeInterval>?  CustomRelativeTimeIntervalParser   = null)
        {

            if (TryParse(JSON,
                         out var relativeTimeInterval,
                         out var errorResponse,
                         CustomRelativeTimeIntervalParser))
            {
                return relativeTimeInterval;
            }

            throw new ArgumentException("The given JSON representation of a relative time interval is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(RelativeTimeIntervalJSON, out RelativeTimeInterval, out ErrorResponse, CustomRelativeTimeIntervalParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a relative time interval.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RelativeTimeInterval">The parsed relative time interval.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out RelativeTimeInterval?  RelativeTimeInterval,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out RelativeTimeInterval,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a relative time interval.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RelativeTimeInterval">The parsed relative time interval.</param>
        /// <param name="CustomRelativeTimeIntervalParser">An optional delegate to parse custom relative time intervals.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out RelativeTimeInterval?      RelativeTimeInterval,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<RelativeTimeInterval>?  CustomRelativeTimeIntervalParser   = null)
        {

            try
            {

                RelativeTimeInterval = default;

                #region Start         [mandatory]

                if (!JSON.ParseMandatory("start",
                                         "start",
                                         out TimeSpan Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Duration      [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
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


                RelativeTimeInterval = new RelativeTimeInterval(
                                           Start,
                                           Duration,
                                           CustomData
                                       );

                if (CustomRelativeTimeIntervalParser is not null)
                    RelativeTimeInterval = CustomRelativeTimeIntervalParser(JSON,
                                                                            RelativeTimeInterval);

                return true;

            }
            catch (Exception e)
            {
                RelativeTimeInterval  = default;
                ErrorResponse         = "The given JSON representation of a relative time interval is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRelativeTimeIntervalSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RelativeTimeInterval>?  CustomRelativeTimeIntervalSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("start",        (UInt64) Math.Round(Start.         TotalSeconds, 0)),

                           Duration.HasValue
                               ? new JProperty("duration",     (UInt64) Math.Round(Duration.Value.TotalSeconds, 0))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRelativeTimeIntervalSerializer is not null
                       ? CustomRelativeTimeIntervalSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RelativeTimeInterval1, RelativeTimeInterval2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RelativeTimeInterval1">A relative time interval.</param>
        /// <param name="RelativeTimeInterval2">Another relative time interval.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RelativeTimeInterval? RelativeTimeInterval1,
                                           RelativeTimeInterval? RelativeTimeInterval2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RelativeTimeInterval1, RelativeTimeInterval2))
                return true;

            // If one is null, but not both, return false.
            if (RelativeTimeInterval1 is null || RelativeTimeInterval2 is null)
                return false;

            return RelativeTimeInterval1.Equals(RelativeTimeInterval2);

        }

        #endregion

        #region Operator != (RelativeTimeInterval1, RelativeTimeInterval2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RelativeTimeInterval1">A relative time interval.</param>
        /// <param name="RelativeTimeInterval2">Another relative time interval.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RelativeTimeInterval? RelativeTimeInterval1,
                                           RelativeTimeInterval? RelativeTimeInterval2)

            => !(RelativeTimeInterval1 == RelativeTimeInterval2);

        #endregion

        #endregion

        #region IEquatable<RelativeTimeInterval> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two relative time intervals for equality.
        /// </summary>
        /// <param name="Object">A relative time interval to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RelativeTimeInterval relativeTimeInterval &&
                   Equals(relativeTimeInterval);

        #endregion

        #region Equals(RelativeTimeInterval)

        /// <summary>
        /// Compares two relative time intervals for equality.
        /// </summary>
        /// <param name="RelativeTimeInterval">A relative time interval to compare with.</param>
        public Boolean Equals(RelativeTimeInterval? RelativeTimeInterval)

            => RelativeTimeInterval is not null &&

               Start.Equals(RelativeTimeInterval.Start) &&

            ((!Duration.HasValue && !RelativeTimeInterval.Duration.HasValue) ||
               Duration.HasValue &&  RelativeTimeInterval.Duration.HasValue && Duration.Value.Equals(RelativeTimeInterval.Duration.Value)) &&

               base.Equals(RelativeTimeInterval);

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

                return Start.    GetHashCode()       * 5 ^
                      (Duration?.GetHashCode() ?? 0) * 3 ^

                       base.     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   "Starting in ",
                   Math.Round(Start.TotalSeconds, 0),
                   " second(s)",

                   Duration.HasValue
                       ? ", duration: " + Math.Round(Duration.Value.TotalSeconds, 0) + " second(s)"
                       : ""

               );

        #endregion

    }

}
