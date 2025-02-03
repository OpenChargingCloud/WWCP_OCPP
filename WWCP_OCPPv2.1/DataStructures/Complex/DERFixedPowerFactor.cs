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
    /// DER Fixed Power Factor
    /// </summary>
    public class DERFixedPowerFactor : ACustomData,
                                       IEquatable<DERFixedPowerFactor>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte       Priority        { get; }

        /// <summary>
        /// Power factor, cos(phi), as value between 0..1.
        /// </summary>
        [Mandatory]
        public Decimal    Displacement    { get; }

        /// <summary>
        /// True when absorbing reactive power (under-excited),
        /// false when injecting reactive power (over-excited).
        /// </summary>
        [Mandatory]
        public Boolean    Excitation      { get; }

        /// <summary>
        /// The timestamp when this setting becomes active.
        /// </summary>
        [Optional]
        public DateTime?  StartTime       { get; }

        /// <summary>
        /// The duration that this setting is active.
        /// </summary>
        [Optional]
        public TimeSpan?  Duration        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new FixedPowerFactor
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="Displacement">Power factor, cos(phi), as value between 0..1.</param>
        /// <param name="Excitation">True when absorbing reactive power (under-excited), false when injecting reactive power (over-excited).</param>
        /// <param name="StartTime">The timestamp when this setting becomes active.</param>
        /// <param name="Duration">The duration that this setting is active.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERFixedPowerFactor(Byte         Priority,
                                   Decimal      Displacement,
                                   Boolean      Excitation,
                                   DateTime?    StartTime    = null,
                                   TimeSpan?    Duration     = null,
                                   CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Priority      = Priority;
            this.Displacement  = Displacement;
            this.Excitation    = Excitation;
            this.StartTime     = StartTime;
            this.Duration      = Duration;

            unchecked
            {

                hashCode = this.Priority.    GetHashCode()       * 13 ^
                           this.Displacement.GetHashCode()       * 11 ^
                           this.Excitation.  GetHashCode()       *  7 ^
                          (this.StartTime?.  GetHashCode() ?? 0) *  5 ^
                          (this.Duration?.   GetHashCode() ?? 0) *  3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "FixedPF",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Priority of setting (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "displacement": {
        //             "description": "Power factor, cos(phi), as value between 0..1.",
        //             "type": "number"
        //         },
        //         "excitation": {
        //             "description": "True when absorbing reactive power (under-excited), false when injecting reactive power (over-excited).",
        //             "type": "boolean"
        //         },
        //         "startTime": {
        //             "description": "Time when this setting becomes active",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "duration": {
        //             "description": "Duration in seconds that this setting is active.",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priority",
        //         "displacement",
        //         "excitation"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomFixedPowerFactorParser = null)

        /// <summary>
        /// Parse the given JSON representation of FixedPowerFactor.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFixedPowerFactorParser">A delegate to parse custom FixedPowerFactor JSON objects.</param>
        public static DERFixedPowerFactor Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<DERFixedPowerFactor>?  CustomFixedPowerFactorParser   = null)
        {

            if (TryParse(JSON,
                         out var derFixedPowerFactor,
                         out var errorResponse,
                         CustomFixedPowerFactorParser))
            {
                return derFixedPowerFactor;
            }

            throw new ArgumentException("The given JSON representation of a FixedPowerFactor is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out FixedPowerFactor, out ErrorResponse, CustomFixedPowerFactorParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of FixedPowerFactor.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FixedPowerFactor">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out DERFixedPowerFactor?  FixedPowerFactor,
                                       [NotNullWhen(false)] out String?               ErrorResponse)

            => TryParse(JSON,
                        out FixedPowerFactor,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of FixedPowerFactor.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FixedPowerFactor">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFixedPowerFactorParser">A delegate to parse custom FixedPowerFactor JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out DERFixedPowerFactor?      FixedPowerFactor,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       CustomJObjectParserDelegate<DERFixedPowerFactor>?  CustomFixedPowerFactorParser)
        {

            try
            {

                FixedPowerFactor = default;

                #region Priority        [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Displacement    [mandatory]

                if (!JSON.ParseMandatory("displacement",
                                         "displacement",
                                         out Decimal Displacement,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Excitation      [mandatory]

                if (!JSON.ParseMandatory("excitation",
                                         "excitation",
                                         out Boolean Excitation,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StartTime       [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Duration        [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                FixedPowerFactor = new DERFixedPowerFactor(
                                       Priority,
                                       Displacement,
                                       Excitation,
                                       StartTime,
                                       Duration,
                                       CustomData
                                   );

                if (CustomFixedPowerFactorParser is not null)
                    FixedPowerFactor = CustomFixedPowerFactorParser(JSON,
                                                                    FixedPowerFactor);

                return true;

            }
            catch (Exception e)
            {
                FixedPowerFactor  = default;
                ErrorResponse     = "The given JSON representation of FixedPowerFactor is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomFixedPowerFactorSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFixedPowerFactorSerializer">A delegate to serialize custom FixedPowerFactor.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERFixedPowerFactor>?  CustomFixedPowerFactorSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",      Priority),
                                 new JProperty("displacement",  Displacement),
                                 new JProperty("excitation",    Excitation),

                           StartTime.HasValue
                               ? new JProperty("startTime",     StartTime.Value.ToIso8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",      Duration. Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFixedPowerFactorSerializer is not null
                       ? CustomFixedPowerFactorSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FixedPowerFactor1, FixedPowerFactor2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FixedPowerFactor1">FixedPowerFactor.</param>
        /// <param name="FixedPowerFactor2">Another FixedPowerFactor.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERFixedPowerFactor? FixedPowerFactor1,
                                           DERFixedPowerFactor? FixedPowerFactor2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FixedPowerFactor1, FixedPowerFactor2))
                return true;

            // If one is null, but not both, return false.
            if (FixedPowerFactor1 is null || FixedPowerFactor2 is null)
                return false;

            return FixedPowerFactor1.Equals(FixedPowerFactor2);

        }

        #endregion

        #region Operator != (FixedPowerFactor1, FixedPowerFactor2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FixedPowerFactor1">FixedPowerFactor.</param>
        /// <param name="FixedPowerFactor2">Another FixedPowerFactor.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERFixedPowerFactor? FixedPowerFactor1,
                                           DERFixedPowerFactor? FixedPowerFactor2)

            => !(FixedPowerFactor1 == FixedPowerFactor2);

        #endregion

        #endregion

        #region IEquatable<FixedPowerFactor> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two FixedPowerFactors for equality.
        /// </summary>
        /// <param name="Object">A FixedPowerFactor to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERFixedPowerFactor derFixedPowerFactor &&
                   Equals(derFixedPowerFactor);

        #endregion

        #region Equals(FixedPowerFactor)

        /// <summary>
        /// Compares two FixedPowerFactor for equality.
        /// </summary>
        /// <param name="FixedPowerFactor">FixedPowerFactor to compare with.</param>
        public Boolean Equals(DERFixedPowerFactor? FixedPowerFactor)

            => FixedPowerFactor is not null &&

               Priority.    Equals(FixedPowerFactor.Priority)     &&
               Displacement.Equals(FixedPowerFactor.Displacement) &&
               Excitation.  Equals(FixedPowerFactor.Excitation)   &&

            ((!StartTime.HasValue && !FixedPowerFactor.StartTime.HasValue) ||
               StartTime.HasValue &&  FixedPowerFactor.StartTime.HasValue && StartTime.Value.Equals(FixedPowerFactor.StartTime.Value)) &&

            ((!Duration. HasValue && !FixedPowerFactor.Duration. HasValue) ||
               Duration. HasValue &&  FixedPowerFactor.Duration. HasValue && Duration. Value.Equals(FixedPowerFactor.Duration. Value)) &&

               base.Equals(FixedPowerFactor);

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

                   $"Priority '{Priority}': cos(phi): {Displacement}, {(Excitation ? "absorbing reactive power (under-excited)" : "injecting reactive power (over-excited)")}",

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
