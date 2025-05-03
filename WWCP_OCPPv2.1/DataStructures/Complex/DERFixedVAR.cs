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
    /// DER FixedVAR
    /// </summary>
    public class DERFixedVAR : ACustomData,
                               IEquatable<DERFixedVAR>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte              Priority     { get; }

        /// <summary>
        /// The value specifies a target var output interpreted as a signed percentage (-100 to 100).
        /// A negative value refers to charging, whereas a positive one refers to discharging.
        /// The value type is determined by the unit field.
        /// </summary>
        [Mandatory]
        public SignedPercentage  Setpoint     { get; }

        /// <summary>
        /// The DER unit.
        /// </summary>
        [Mandatory]
        public DERUnit           Unit         { get; }

        /// <summary>
        /// The timestamp when this setting becomes active.
        /// </summary>
        [Optional]
        public DateTime?         StartTime    { get; }

        /// <summary>
        /// The duration that this setting is active.
        /// </summary>
        [Optional]
        public TimeSpan?         Duration     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new FixedVAR
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="Setpoint">The value specifies a target var output interpreted as a signed percentage (-100 to 100).</param>
        /// <param name="Unit">The DER unit.</param>
        /// <param name="StartTime">The timestamp when this setting becomes active.</param>
        /// <param name="Duration">The duration that this setting is active.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERFixedVAR(Byte              Priority,
                           SignedPercentage  Setpoint,
                           DERUnit           Unit,
                           DateTime?         StartTime    = null,
                           TimeSpan?         Duration     = null,
                           CustomData?       CustomData   = null)

            : base(CustomData)

        {

            this.Priority   = Priority;
            this.Setpoint   = Setpoint;
            this.Unit       = Unit;
            this.StartTime  = StartTime;
            this.Duration   = Duration;

            unchecked
            {

                hashCode = this.Priority.  GetHashCode()       * 13 ^
                           this.Setpoint.  GetHashCode()       * 11 ^
                           this.Unit.      GetHashCode()       *  7 ^
                          (this.StartTime?.GetHashCode() ?? 0) *  5 ^
                          (this.Duration?. GetHashCode() ?? 0) *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "FixedVar",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Priority of setting (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "setpoint": {
        //             "description": "The value specifies a target var output interpreted as a signed percentage (-100 to 100). \r\n    A negative value refers to charging, whereas a positive one refers to discharging.\r\n    The value type is determined by the unit field.",
        //             "type": "number"
        //         },
        //         "unit": {
        //             "$ref": "#/definitions/DERUnitEnumType"
        //         },
        //         "startTime": {
        //             "description": "Time when this setting becomes active.",
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
        //         "setpoint",
        //         "unit"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomFixedVARParser = null)

        /// <summary>
        /// Parse the given JSON representation of FixedVAR.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFixedVARParser">A delegate to parse custom FixedVAR JSON objects.</param>
        public static DERFixedVAR Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<DERFixedVAR>?  CustomFixedVARParser   = null)
        {

            if (TryParse(JSON,
                         out var derFixedVAR,
                         out var errorResponse,
                         CustomFixedVARParser))
            {
                return derFixedVAR;
            }

            throw new ArgumentException("The given JSON representation of a FixedVAR is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out FixedVAR, out ErrorResponse, CustomFixedVARParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of FixedVAR.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FixedVAR">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out DERFixedVAR?  FixedVAR,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out FixedVAR,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of FixedVAR.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FixedVAR">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFixedVARParser">A delegate to parse custom FixedVAR JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out DERFixedVAR?      FixedVAR,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<DERFixedVAR>?  CustomFixedVARParser)
        {

            try
            {

                FixedVAR = default;

                #region Priority      [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Setpoint      [mandatory]

                if (!JSON.ParseMandatory("setpoint",
                                         "setpoint",
                                         SignedPercentage.TryParse,
                                         out SignedPercentage Setpoint,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Unit          [mandatory]

                if (!JSON.ParseMandatory("unit",
                                         "DER unit",
                                         DERUnit.TryParse,
                                         out DERUnit Unit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StartTime     [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                FixedVAR = new DERFixedVAR(
                               Priority,
                               Setpoint,
                               Unit,
                               StartTime,
                               Duration,
                               CustomData
                           );

                if (CustomFixedVARParser is not null)
                    FixedVAR = CustomFixedVARParser(JSON,
                                                    FixedVAR);

                return true;

            }
            catch (Exception e)
            {
                FixedVAR       = default;
                ErrorResponse  = "The given JSON representation of FixedVAR is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomFixedVARSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFixedVARSerializer">A delegate to serialize custom FixedVAR.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERFixedVAR>?  CustomFixedVARSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",     Priority),
                                 new JProperty("setpoint",     Setpoint. Value),
                                 new JProperty("unit",         Unit.           ToString()),

                           StartTime.HasValue
                               ? new JProperty("startTime",    StartTime.Value.ToISO8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",     Duration. Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",   CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFixedVARSerializer is not null
                       ? CustomFixedVARSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FixedVAR1, FixedVAR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FixedVAR1">FixedVAR.</param>
        /// <param name="FixedVAR2">Another FixedVAR.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERFixedVAR? FixedVAR1,
                                           DERFixedVAR? FixedVAR2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FixedVAR1, FixedVAR2))
                return true;

            // If one is null, but not both, return false.
            if (FixedVAR1 is null || FixedVAR2 is null)
                return false;

            return FixedVAR1.Equals(FixedVAR2);

        }

        #endregion

        #region Operator != (FixedVAR1, FixedVAR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FixedVAR1">FixedVAR.</param>
        /// <param name="FixedVAR2">Another FixedVAR.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERFixedVAR? FixedVAR1,
                                           DERFixedVAR? FixedVAR2)

            => !(FixedVAR1 == FixedVAR2);

        #endregion

        #endregion

        #region IEquatable<FixedVAR> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two FixedVARs for equality.
        /// </summary>
        /// <param name="Object">FixedVAR to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERFixedVAR derFixedVAR &&
                   Equals(derFixedVAR);

        #endregion

        #region Equals(FixedVAR)

        /// <summary>
        /// Compares two FixedVARs for equality.
        /// </summary>
        /// <param name="FixedVAR">A FixedVAR to compare with.</param>
        public Boolean Equals(DERFixedVAR? FixedVAR)

            => FixedVAR is not null &&

               Priority.Equals(FixedVAR.Priority) &&
               Setpoint.Equals(FixedVAR.Setpoint) &&
               Unit.    Equals(FixedVAR.Unit)     &&

            ((!StartTime.HasValue && !FixedVAR.StartTime.HasValue) ||
               StartTime.HasValue &&  FixedVAR.StartTime.HasValue && StartTime.Value.Equals(FixedVAR.StartTime.Value)) &&

            ((!Duration. HasValue && !FixedVAR.Duration. HasValue) ||
               Duration. HasValue &&  FixedVAR.Duration. HasValue && Duration. Value.Equals(FixedVAR.Duration. Value)) &&

               base.Equals(FixedVAR);

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

                   $"Priority '{Priority}': {Setpoint} {Unit}",

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
