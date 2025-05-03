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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER FrequencyDroop
    /// </summary>
    public class DERFrequencyDroop : ACustomData,
                                     IEquatable<DERFrequencyDroop>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte       Priority          { get; }

        /// <summary>
        /// Over-frequency start of droop.
        /// </summary>
        [Mandatory]
        public Hertz      OverFrequency     { get; }

        /// <summary>
        /// Under-frequency start of droop.
        /// </summary>
        [Mandatory]
        public Hertz      UnderFrequency    { get; }

        /// <summary>
        /// Over-frequency droop per unit, oFDroop.
        /// </summary>
        [Mandatory]
        public Decimal    OverDroop         { get; }

        /// <summary>
        /// Under-frequency droop per unit, uFDroop.
        /// </summary>
        [Mandatory]
        public Decimal    UnderDroop        { get; }

        /// <summary>
        /// The open loop response time.
        /// </summary>
        [Mandatory]
        public TimeSpan   ResponseTime      { get; }

        /// <summary>
        /// The timestamp when this setting becomes active.
        /// </summary>
        [Optional]
        public DateTime?  StartTime         { get; }

        /// <summary>
        /// The duration that this setting is active.
        /// </summary>
        [Optional]
        public TimeSpan?  Duration          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new FrequencyDroop
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="OverFrequency">Over-frequency start of droop.</param>
        /// <param name="UnderFrequency">Under-frequency start of droop.</param>
        /// <param name="OverDroop">Over-frequency droop per unit, oFDroop.</param>
        /// <param name="UnderDroop">Under-frequency droop per unit, uFDroop.</param>
        /// <param name="ResponseTime">The open loop response time.</param>
        /// <param name="StartTime">The timestamp when this setting becomes active.</param>
        /// <param name="Duration">The duration that this setting is active.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERFrequencyDroop(Byte         Priority,
                                 Hertz        OverFrequency,
                                 Hertz        UnderFrequency,
                                 Decimal      OverDroop,
                                 Decimal      UnderDroop,
                                 TimeSpan     ResponseTime,
                                 DateTime?    StartTime    = null,
                                 TimeSpan?    Duration     = null,
                                 CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Priority        = Priority;
            this.OverFrequency   = OverFrequency;
            this.UnderFrequency  = UnderFrequency;
            this.OverDroop       = OverDroop;
            this.UnderDroop      = UnderDroop;
            this.ResponseTime    = ResponseTime;
            this.StartTime       = StartTime;
            this.Duration        = Duration;

            unchecked
            {

                hashCode = this.Priority.      GetHashCode()       * 23 ^
                           this.OverFrequency. GetHashCode()       * 19 ^
                           this.UnderFrequency.GetHashCode()       * 17 ^
                           this.OverDroop.     GetHashCode()       * 13 ^
                           this.UnderDroop.    GetHashCode()       * 11 ^
                           this.ResponseTime.  GetHashCode()       *  7 ^
                          (this.StartTime?.    GetHashCode() ?? 0) *  5 ^
                          (this.Duration?.     GetHashCode() ?? 0) *  3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "FreqDroop",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Priority of setting (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "overFreq": {
        //             "description": "Over-frequency start of droop",
        //             "type": "number"
        //         },
        //         "underFreq": {
        //             "description": "Under-frequency start of droop",
        //             "type": "number"
        //         },
        //         "overDroop": {
        //             "description": "Over-frequency droop per unit, oFDroop",
        //             "type": "number"
        //         },
        //         "underDroop": {
        //             "description": "Under-frequency droop per unit, uFDroop",
        //             "type": "number"
        //         },
        //         "responseTime": {
        //             "description": "Open loop response time in seconds",
        //             "type": "number"
        //         },
        //         "startTime": {
        //             "description": "Time when this setting becomes active",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "duration": {
        //             "description": "Duration in seconds that this setting is active",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priority",
        //         "overFreq",
        //         "underFreq",
        //         "overDroop",
        //         "underDroop",
        //         "responseTime"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomFrequencyDroopParser = null)

        /// <summary>
        /// Parse the given JSON representation of FrequencyDroop.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFrequencyDroopParser">A delegate to parse custom FrequencyDroop JSON objects.</param>
        public static DERFrequencyDroop Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<DERFrequencyDroop>?  CustomFrequencyDroopParser   = null)
        {

            if (TryParse(JSON,
                         out var derFrequencyDroop,
                         out var errorResponse,
                         CustomFrequencyDroopParser))
            {
                return derFrequencyDroop;
            }

            throw new ArgumentException("The given JSON representation of a FrequencyDroop is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out FrequencyDroop, out ErrorResponse, CustomFrequencyDroopParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of FrequencyDroop.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FrequencyDroop">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out DERFrequencyDroop?  FrequencyDroop,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out FrequencyDroop,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of FrequencyDroop.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FrequencyDroop">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFrequencyDroopParser">A delegate to parse custom FrequencyDroop JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out DERFrequencyDroop?      FrequencyDroop,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<DERFrequencyDroop>?  CustomFrequencyDroopParser)
        {

            try
            {

                FrequencyDroop = default;

                #region Priority          [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OverFrequency     [mandatory]

                if (!JSON.ParseMandatory("overFreq",
                                         "over frequency",
                                         out Hertz OverFrequency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UnderFrequency    [mandatory]

                if (!JSON.ParseMandatory("underFreq",
                                         "under frequency",
                                         out Hertz UnderFrequency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OverDroop         [mandatory]

                if (!JSON.ParseMandatory("overDroop",
                                         "over droop",
                                         out Decimal OverDroop,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UnderDroop        [mandatory]

                if (!JSON.ParseMandatory("underDroop",
                                         "under droop",
                                         out Decimal UnderDroop,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ResponseTime      [mandatory]

                if (!JSON.ParseMandatory("responseTime",
                                         "response time",
                                         out TimeSpan ResponseTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StartTime         [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Duration          [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData        [optional]

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


                FrequencyDroop = new DERFrequencyDroop(
                                     Priority,
                                     OverFrequency,
                                     UnderFrequency,
                                     OverDroop,
                                     UnderDroop,
                                     ResponseTime,
                                     StartTime,
                                     Duration,
                                     CustomData
                                 );

                if (CustomFrequencyDroopParser is not null)
                    FrequencyDroop = CustomFrequencyDroopParser(JSON,
                                                                FrequencyDroop);

                return true;

            }
            catch (Exception e)
            {
                FrequencyDroop  = default;
                ErrorResponse   = "The given JSON representation of FrequencyDroop is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomFrequencyDroopSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFrequencyDroopSerializer">A delegate to serialize custom FrequencyDroop.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERFrequencyDroop>?  CustomFrequencyDroopSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",       Priority),
                                 new JProperty("overFreq",       OverFrequency. Value),
                                 new JProperty("underFreq",      UnderFrequency.Value),
                                 new JProperty("overDroop",      OverDroop),
                                 new JProperty("underDroop",     UnderDroop),
                                 new JProperty("responseTime",   ResponseTime.        TotalSeconds),

                           StartTime.HasValue
                               ? new JProperty("startTime",      StartTime.     Value.ToISO8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",       Duration.      Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",     CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFrequencyDroopSerializer is not null
                       ? CustomFrequencyDroopSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FrequencyDroop1, FrequencyDroop2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FrequencyDroop1">FrequencyDroop.</param>
        /// <param name="FrequencyDroop2">Another FrequencyDroop.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERFrequencyDroop? FrequencyDroop1,
                                           DERFrequencyDroop? FrequencyDroop2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FrequencyDroop1, FrequencyDroop2))
                return true;

            // If one is null, but not both, return false.
            if (FrequencyDroop1 is null || FrequencyDroop2 is null)
                return false;

            return FrequencyDroop1.Equals(FrequencyDroop2);

        }

        #endregion

        #region Operator != (FrequencyDroop1, FrequencyDroop2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FrequencyDroop1">FrequencyDroop.</param>
        /// <param name="FrequencyDroop2">Another FrequencyDroop.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERFrequencyDroop? FrequencyDroop1,
                                           DERFrequencyDroop? FrequencyDroop2)

            => !(FrequencyDroop1 == FrequencyDroop2);

        #endregion

        #endregion

        #region IEquatable<FrequencyDroop> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two FrequencyDroops for equality.
        /// </summary>
        /// <param name="Object">FrequencyDroop to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERFrequencyDroop derFrequencyDroop &&
                   Equals(derFrequencyDroop);

        #endregion

        #region Equals(FrequencyDroop)

        /// <summary>
        /// Compares two FrequencyDroops for equality.
        /// </summary>
        /// <param name="FrequencyDroop">FrequencyDroop to compare with.</param>
        public Boolean Equals(DERFrequencyDroop? FrequencyDroop)

            => FrequencyDroop is not null &&

               Priority.      Equals(FrequencyDroop.Priority)       &&
               OverFrequency. Equals(FrequencyDroop.OverFrequency)  &&
               UnderFrequency.Equals(FrequencyDroop.UnderFrequency) &&
               OverDroop.     Equals(FrequencyDroop.OverDroop)      &&
               UnderDroop.    Equals(FrequencyDroop.UnderDroop)     &&
               ResponseTime.  Equals(FrequencyDroop.ResponseTime)   &&

            ((!StartTime.HasValue && !FrequencyDroop.StartTime.HasValue) ||
               StartTime.HasValue &&  FrequencyDroop.StartTime.HasValue && StartTime.Value.Equals(FrequencyDroop.StartTime.Value)) &&

            ((!Duration. HasValue && !FrequencyDroop.Duration. HasValue) ||
               Duration. HasValue &&  FrequencyDroop.Duration. HasValue && Duration. Value.Equals(FrequencyDroop.Duration. Value)) &&

               base.Equals(FrequencyDroop);

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

            => String.Concat(

                   $"Priority '{Priority}': {OverFrequency} / {UnderFrequency}, droop: {OverDroop} / {UnderDroop}, response time: {Math.Round(ResponseTime.TotalSeconds, 2)} seconds",

                   StartTime.HasValue
                       ? $", start: {StartTime.Value}"
                       : "",

                   Duration. HasValue
                       ? $", duration: {Math.Round(Duration.Value.TotalSeconds, 2)} seconds"
                       : ""

               );

        #endregion

    }

}
