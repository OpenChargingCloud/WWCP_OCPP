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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER Curve
    /// </summary>
    public class DERCurve : ACustomData,
                            IEquatable<DERCurve>
    {

        #region Properties

        /// <summary>
        /// The enumeration of DER curve points.
        /// </summary>
        [Mandatory]
        public IEnumerable<DERCurvePoint>  CurveData                  { get; }

        /// <summary>
        /// The priority of curve (0=highest)
        /// </summary>
        [Mandatory]
        public Byte                        Priority                   { get; }

        /// <summary>
        /// The DER unit of the y-axis values of the curve
        /// </summary>
        [Mandatory]
        public DERUnit                     YUnit                      { get; }

        /// <summary>
        /// The optional hysteresis of the curve.
        /// </summary>
        [Optional]
        public Hysteresis?                 Hysteresis                 { get; }

        /// <summary>
        /// The optional reactive power parameters of the curve.
        /// </summary>
        [Optional]
        public ReactivePowerParameters?    ReactivePowerParameters    { get; }

        /// <summary>
        /// The optional voltage parameters of the curve.
        /// </summary>
        [Optional]
        public VoltageParameters?          VoltageParameters          { get; }

        /// <summary>
        /// The optional open loop response time, the time to ramp up to 90% of the new target in response to the change in voltage.
        /// A value of 0 is used to mean no limit. When not present, the device should follow its default behavior.
        /// </summary>
        [Optional]
        public TimeSpan?                   ResponseTime               { get; }

        /// <summary>
        /// The optional timestamp when this curve will become activated.
        /// Only absent when this is a _default_ DER control curve.
        /// </summary>
        [Optional]
        public DateTime?                   StartTime                  { get; }

        /// <summary>
        /// The optional duration in seconds that this curve will be active.
        /// Only absent when this is a _default_ DER control curve.
        /// </summary>
        [Optional]
        public TimeSpan?                   Duration                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DER curve
        /// </summary>
        /// <param name="CurveData">The enumeration of DER curve points.</param>
        /// <param name="Priority">The priority of curve (0=highest)</param>
        /// <param name="YUnit">The DER unit of the y-axis values of the curve</param>
        /// <param name="Hysteresis">The optional hysteresis of the curve.</param>
        /// <param name="ReactivePowerParameters">The optional reactive power parameters of the curve.</param>
        /// <param name="VoltageParameters">The optional voltage parameters of the curve.</param>
        /// <param name="ResponseTime">The optional open loop response time, the time to ramp up to 90% of the new target in response to the change in voltage.</param>
        /// <param name="StartTime">The optional timestamp when this curve will become activated.</param>
        /// <param name="Duration">The optional duration in seconds that this curve will be active.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERCurve(IEnumerable<DERCurvePoint>  CurveData,
                        Byte                        Priority,
                        DERUnit                     YUnit,
                        Hysteresis?                 Hysteresis                = null,
                        ReactivePowerParameters?    ReactivePowerParameters   = null,
                        VoltageParameters?          VoltageParameters         = null,
                        TimeSpan?                   ResponseTime              = null,
                        DateTime?                   StartTime                 = null,
                        TimeSpan?                   Duration                  = null,
                        CustomData?                 CustomData                = null)

            : base(CustomData)

        {

            this.CurveData                = CurveData.Distinct();
            this.Priority                 = Priority;
            this.YUnit                    = YUnit;
            this.Hysteresis               = Hysteresis;
            this.ReactivePowerParameters  = ReactivePowerParameters;
            this.VoltageParameters        = VoltageParameters;
            this.ResponseTime             = ResponseTime;
            this.StartTime                = StartTime;
            this.Duration                 = Duration;

            unchecked
            {

                hashCode = this.CurveData.               GetHashCode()       * 29 ^
                           this.Priority.                GetHashCode()       * 23 ^
                           this.YUnit.                   GetHashCode()       * 19 ^
                          (this.Hysteresis?.             GetHashCode() ?? 0) * 17 ^
                          (this.ReactivePowerParameters?.GetHashCode() ?? 0) * 13 ^
                          (this.VoltageParameters?.      GetHashCode() ?? 0) * 11 ^
                          (this.ResponseTime?.           GetHashCode() ?? 0) *  7 ^
                          (this.StartTime?.              GetHashCode() ?? 0) *  5 ^
                          (this.Duration?.               GetHashCode() ?? 0) *  3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "DERCurve",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "curveData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/DERCurvePointsType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 10
        //         },
        //         "hysteresis": {
        //             "$ref": "#/definitions/HysteresisType"
        //         },
        //         "priority": {
        //             "description": "Priority of curve (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "reactivePowerParams": {
        //             "$ref": "#/definitions/ReactivePowerParamsType"
        //         },
        //         "voltageParams": {
        //             "$ref": "#/definitions/VoltageParamsType"
        //         },
        //         "yUnit": {
        //             "$ref": "#/definitions/DERUnitEnumType"
        //         },
        //         "responseTime": {
        //             "description": "Open loop response time, the time to ramp up to 90% of the new target in response to the change in voltage, in seconds. A value of 0 is used to mean no limit. When not present, the device should follow its default behavior.",
        //             "type": "number"
        //         },
        //         "startTime": {
        //             "description": "Point in time when this curve will become activated. Only absent when _default_ is true.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "duration": {
        //             "description": "Duration in seconds that this curve will be active. Only absent when _default_ is true.",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priority",
        //         "yUnit",
        //         "curveData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDERCurveParser = null)

        /// <summary>
        /// Parse the given JSON representation of DER curve.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDERCurveParser">A delegate to parse custom DER curve JSON objects.</param>
        public static DERCurve Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<DERCurve>?  CustomDERCurveParser   = null)
        {

            if (TryParse(JSON,
                         out var derCurve,
                         out var errorResponse,
                         CustomDERCurveParser))
            {
                return derCurve;
            }

            throw new ArgumentException("The given JSON representation of DER curve is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DERCurve, out ErrorResponse, CustomDERCurveParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DER curve.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERCurve">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out DERCurve?  DERCurve,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out DERCurve,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DER curve.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERCurve">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDERCurveParser">A delegate to parse custom DER curve JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out DERCurve?      DERCurve,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<DERCurve>?  CustomDERCurveParser)
        {

            try
            {

                DERCurve = default;

                #region CurveData                  [mandatory]

                if (!JSON.ParseMandatoryList("curveData",
                                             "DER curve point data",
                                             DERCurvePoint.TryParse,
                                             out List<DERCurvePoint> CurveData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Priority                   [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region YUnit                      [mandatory]

                if (!JSON.ParseMandatory("yUnit",
                                         "curve y-axis DER unit",
                                         DERUnit.TryParse,
                                         out DERUnit YUnit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Hysteresis                 [optional]

                if (JSON.ParseOptionalJSON("hysteresis",
                                           "hysteresis",
                                           OCPPv2_1.Hysteresis.TryParse,
                                           out Hysteresis? Hysteresis,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ReactivePowerParameters    [optional]

                if (JSON.ParseOptionalJSON("reactivePowerParams",
                                           "reactive power parameters",
                                           OCPPv2_1.ReactivePowerParameters.TryParse,
                                           out ReactivePowerParameters? ReactivePowerParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VoltageParameters          [optional]

                if (JSON.ParseOptionalJSON("voltageParams",
                                           "voltage parameters",
                                           OCPPv2_1.VoltageParameters.TryParse,
                                           out VoltageParameters? VoltageParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ResponseTime               [optional]

                if (JSON.ParseOptional("responseTime",
                                       "response time",
                                       out TimeSpan? ResponseTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StartTime                  [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Duration                   [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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


                DERCurve = new DERCurve(
                               CurveData,
                               Priority,
                               YUnit,
                               Hysteresis,
                               ReactivePowerParameters,
                               VoltageParameters,
                               ResponseTime,
                               StartTime,
                               Duration,
                               CustomData
                           );

                if (CustomDERCurveParser is not null)
                    DERCurve = CustomDERCurveParser(JSON,
                                                    DERCurve);

                return true;

            }
            catch (Exception e)
            {
                DERCurve       = default;
                ErrorResponse  = "The given JSON representation of DER curve is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDERCurveSerializer = null, CustomDERCurvePointSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDERCurveSerializer">A delegate to serialize custom DER curve.</param>
        /// <param name="CustomDERCurvePointSerializer">A delegate to serialize custom DER curve point.</param>
        /// <param name="CustomHysteresisSerializer">A delegate to serialize custom hysteresis.</param>
        /// <param name="CustomReactivePowerParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomVoltageParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERCurve>?                 CustomDERCurveSerializer                  = null,
                              CustomJObjectSerializerDelegate<DERCurvePoint>?            CustomDERCurvePointSerializer             = null,
                              CustomJObjectSerializerDelegate<Hysteresis>?               CustomHysteresisSerializer                = null,
                              CustomJObjectSerializerDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<VoltageParameters>?        CustomVoltageParametersSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("curveData",             new JArray(CurveData.Select(derCurvePoint => derCurvePoint.ToJSON(CustomDERCurvePointSerializer,
                                                                                                                                          CustomCustomDataSerializer)))),
                                 new JProperty("priority",              Priority),
                                 new JProperty("yUnit",                 YUnit.                  ToString()),

                           Hysteresis              is not null
                               ? new JProperty("hysteresis",            Hysteresis.             ToJSON(CustomHysteresisSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           ReactivePowerParameters is not null
                               ? new JProperty("reactivePowerParams",   ReactivePowerParameters.ToJSON(CustomReactivePowerParametersSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           VoltageParameters       is not null
                               ? new JProperty("voltageParams",         VoltageParameters.      ToJSON(CustomVoltageParametersSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           ResponseTime.HasValue
                               ? new JProperty("responseTime",          ResponseTime.Value.TotalSeconds)
                               : null,

                           StartTime.   HasValue
                               ? new JProperty("startTime",             StartTime.Value.        ToISO8601())
                               : null,

                           Duration.    HasValue
                               ? new JProperty("duration",              Duration.    Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",            CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDERCurveSerializer is not null
                       ? CustomDERCurveSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DERCurve1, DERCurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERCurve1">DER curve.</param>
        /// <param name="DERCurve2">Another DER curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERCurve? DERCurve1,
                                           DERCurve? DERCurve2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DERCurve1, DERCurve2))
                return true;

            // If one is null, but not both, return false.
            if (DERCurve1 is null || DERCurve2 is null)
                return false;

            return DERCurve1.Equals(DERCurve2);

        }

        #endregion

        #region Operator != (DERCurve1, DERCurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERCurve1">DER curve.</param>
        /// <param name="DERCurve2">Another DER curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERCurve? DERCurve1,
                                           DERCurve? DERCurve2)

            => !(DERCurve1 == DERCurve2);

        #endregion

        #endregion

        #region IEquatable<DERCurve> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER curve for equality..
        /// </summary>
        /// <param name="Object">DER curve to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERCurve derCurve &&
                   Equals(derCurve);

        #endregion

        #region Equals(DERCurve)

        /// <summary>
        /// Compares two DER curve for equality.
        /// </summary>
        /// <param name="DERCurve">DER curve to compare with.</param>
        public Boolean Equals(DERCurve? DERCurve)

            => DERCurve is not null &&

               CurveData.ToHashSet().SetEquals(DERCurve.CurveData) &&
               Priority.Equals(DERCurve.Priority) &&
               YUnit.Equals(DERCurve.YUnit) &&

             ((Hysteresis              is     null && DERCurve.Hysteresis              is     null) ||
              (Hysteresis              is not null && DERCurve.Hysteresis              is not null && Hysteresis.             Equals(DERCurve.Hysteresis)))              &&

             ((ReactivePowerParameters is     null && DERCurve.ReactivePowerParameters is     null) ||
              (ReactivePowerParameters is not null && DERCurve.ReactivePowerParameters is not null && ReactivePowerParameters.Equals(DERCurve.ReactivePowerParameters))) &&

             ((VoltageParameters       is     null && DERCurve.VoltageParameters       is     null) ||
              (VoltageParameters       is not null && DERCurve.VoltageParameters       is not null && VoltageParameters.      Equals(DERCurve.VoltageParameters)))       &&

            ((!ResponseTime.HasValue && !DERCurve.ResponseTime.HasValue) ||
               ResponseTime.HasValue &&  DERCurve.ResponseTime.HasValue && ResponseTime.Value.Equals(DERCurve.ResponseTime.Value)) &&

            ((!StartTime.   HasValue && !DERCurve.StartTime.   HasValue) ||
               StartTime.   HasValue &&  DERCurve.StartTime.   HasValue && StartTime.   Value.Equals(DERCurve.StartTime.   Value)) &&

            ((!Duration.    HasValue && !DERCurve.Duration.    HasValue) ||
               Duration.    HasValue &&  DERCurve.Duration.    HasValue && Duration.    Value.Equals(DERCurve.Duration.    Value)) &&

               base.Equals(DERCurve);

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

            => $"{CurveData.Count()} {YUnit} curve points, with priority '{Priority}'";

        #endregion

    }

}
