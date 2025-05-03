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
    /// DER Limit Max Discharge
    /// </summary>
    public class DERLimitMaxDischarge : ACustomData,
                                        IEquatable<DERLimitMaxDischarge>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte         Priority                       { get; }

        /// <summary>
        /// Only for PowerMonitoring.
        /// The value specifies a percentage (0 to 100) of the rated maximum discharge power of EV.
        /// The PowerMonitoring curve becomes active when power exceeds this percentage.
        /// </summary>
        [Optional]
        public Percentage?  PercentageMaxDischargePower    { get; }

        /// <summary>
        /// The optional DER Curve.
        /// </summary>
        [Optional]
        public DERCurve?    PowerMonitoringMustTrip        { get; }

        /// <summary>
        /// The timestamp when this setting becomes active.
        /// </summary>
        [Optional]
        public DateTime?    StartTime                      { get; }

        /// <summary>
        /// The duration that this setting is active.
        /// </summary>
        [Optional]
        public TimeSpan?    Duration                       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DER LimitMaxDischarge
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="PercentageMaxDischargePower">Only for PowerMonitoring. The value specifies a percentage (0 to 100) of the rated maximum discharge power of EV. The PowerMonitoring curve becomes active when power exceeds this percentage.</param>
        /// <param name="PowerMonitoringMustTrip">An optional DER Curve.</param>
        /// <param name="StartTime">The timestamp when this setting becomes active.</param>
        /// <param name="Duration">The duration that this setting is active.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERLimitMaxDischarge(Byte         Priority,
                                    Percentage?  PercentageMaxDischargePower   = null,
                                    DERCurve?    PowerMonitoringMustTrip       = null,
                                    DateTime?    StartTime                     = null,
                                    TimeSpan?    Duration                      = null,
                                    CustomData?  CustomData                    = null)

            : base(CustomData)

        {

            this.Priority                     = Priority;
            this.PercentageMaxDischargePower  = PercentageMaxDischargePower;
            this.PowerMonitoringMustTrip      = PowerMonitoringMustTrip;
            this.StartTime                    = StartTime;
            this.Duration                     = Duration;

            unchecked
            {

                hashCode = this.Priority.                    GetHashCode()       * 13 ^
                          (this.PercentageMaxDischargePower?.GetHashCode() ?? 0) * 11 ^
                          (this.PowerMonitoringMustTrip?.    GetHashCode() ?? 0) *  7 ^
                          (this.StartTime?.                  GetHashCode() ?? 0) *  5 ^
                          (this.Duration?.                   GetHashCode() ?? 0) *  3 ^
                           base.                             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "LimitMaxDischarge",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Priority of setting (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "pctMaxDischargePower": {
        //             "description": "Only for PowerMonitoring.
        //             The value specifies a percentage (0 to 100) of the rated maximum discharge power of EV.
        //             The PowerMonitoring curve becomes active when power exceeds this percentage.",
        //             "type": "number"
        //         },
        //         "powerMonitoringMustTrip": {
        //             "$ref": "#/definitions/DERCurveType"
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
        //         "priority"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomFixedPowerFactorParser = null)

        /// <summary>
        /// Parse the given JSON representation of FixedPowerFactor.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFixedPowerFactorParser">A delegate to parse custom FixedPowerFactor JSON objects.</param>
        public static DERLimitMaxDischarge Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<DERLimitMaxDischarge>?  CustomFixedPowerFactorParser   = null)
        {

            if (TryParse(JSON,
                         out var derLimitMaxDischarge,
                         out var errorResponse,
                         CustomFixedPowerFactorParser))
            {
                return derLimitMaxDischarge;
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
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out DERLimitMaxDischarge?  FixedPowerFactor,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

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
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out DERLimitMaxDischarge?      FixedPowerFactor,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<DERLimitMaxDischarge>?  CustomFixedPowerFactorParser)
        {

            try
            {

                FixedPowerFactor = default;

                #region Priority                       [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PercentageMaxDischargePower    [optional]

                if (JSON.ParseOptional("pctMaxDischargePower",
                                       "percentage max discharge power",
                                       out Percentage? PercentageMaxDischargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PowerMonitoringMustTrip        [optional]

                if (JSON.ParseOptionalJSON("powerMonitoringMustTrip",
                                           "power monitoring must trip",
                                           DERCurve.TryParse,
                                           out DERCurve? PowerMonitoringMustTrip,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StartTime                      [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Duration                       [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                     [optional]

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


                FixedPowerFactor = new DERLimitMaxDischarge(
                                       Priority,
                                       PercentageMaxDischargePower,
                                       PowerMonitoringMustTrip,
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

        #region ToJSON(CustomDERLimitMaxDischargeSerializer = null, CustomDERCurveSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDERLimitMaxDischargeSerializer">A delegate to serialize custom DERLimitMaxDischarge JSON objects.</param>
        /// <param name="CustomDERCurveSerializer">A delegate to serialize custom DER curve.</param>
        /// <param name="CustomDERCurvePointSerializer">A delegate to serialize custom DER curve point.</param>
        /// <param name="CustomHysteresisSerializer">A delegate to serialize custom hysteresis.</param>
        /// <param name="CustomReactivePowerParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomVoltageParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERLimitMaxDischarge>?     CustomDERLimitMaxDischargeSerializer      = null,
                              CustomJObjectSerializerDelegate<DERCurve>?                 CustomDERCurveSerializer                  = null,
                              CustomJObjectSerializerDelegate<DERCurvePoint>?            CustomDERCurvePointSerializer             = null,
                              CustomJObjectSerializerDelegate<Hysteresis>?               CustomHysteresisSerializer                = null,
                              CustomJObjectSerializerDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<VoltageParameters>?        CustomVoltageParametersSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",                  Priority),

                           PercentageMaxDischargePower.HasValue
                               ? new JProperty("pctMaxDischargePower",      PercentageMaxDischargePower.Value.Value)
                               : null,

                           PowerMonitoringMustTrip is not null
                               ? new JProperty("powerMonitoringMustTrip",   PowerMonitoringMustTrip.          ToJSON(CustomDERCurveSerializer,
                                                                                                                     CustomDERCurvePointSerializer,
                                                                                                                     CustomHysteresisSerializer,
                                                                                                                     CustomReactivePowerParametersSerializer,
                                                                                                                     CustomVoltageParametersSerializer,
                                                                                                                     CustomCustomDataSerializer))
                               : null,

                           StartTime.HasValue
                               ? new JProperty("startTime",                 StartTime.                  Value.ToISO8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",                  Duration.                   Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",                CustomData.                       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDERLimitMaxDischargeSerializer is not null
                       ? CustomDERLimitMaxDischargeSerializer(this, json)
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
        public static Boolean operator == (DERLimitMaxDischarge? FixedPowerFactor1,
                                           DERLimitMaxDischarge? FixedPowerFactor2)
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
        public static Boolean operator != (DERLimitMaxDischarge? FixedPowerFactor1,
                                           DERLimitMaxDischarge? FixedPowerFactor2)

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

            => Object is DERLimitMaxDischarge derLimitMaxDischarge &&
                   Equals(derLimitMaxDischarge);

        #endregion

        #region Equals(FixedPowerFactor)

        /// <summary>
        /// Compares two FixedPowerFactor for equality.
        /// </summary>
        /// <param name="FixedPowerFactor">FixedPowerFactor to compare with.</param>
        public Boolean Equals(DERLimitMaxDischarge? FixedPowerFactor)

            => FixedPowerFactor is not null &&

               Priority.Equals(FixedPowerFactor.Priority) &&

            ((!PercentageMaxDischargePower.HasValue    && !FixedPowerFactor.PercentageMaxDischargePower.HasValue) ||
               PercentageMaxDischargePower.HasValue    &&  FixedPowerFactor.PercentageMaxDischargePower.HasValue    && PercentageMaxDischargePower.Value.Equals(FixedPowerFactor.PercentageMaxDischargePower.Value)) &&

             ((PowerMonitoringMustTrip     is null     &&  FixedPowerFactor.PowerMonitoringMustTrip     is null)  ||
              (PowerMonitoringMustTrip     is not null &&  FixedPowerFactor.PowerMonitoringMustTrip     is not null && PowerMonitoringMustTrip.          Equals(FixedPowerFactor.PowerMonitoringMustTrip)))          &&

            ((!StartTime.                  HasValue    && !FixedPowerFactor.StartTime.                  HasValue) ||
               StartTime.                  HasValue    &&  FixedPowerFactor.StartTime.                  HasValue    && StartTime.                  Value.Equals(FixedPowerFactor.StartTime.                  Value)) &&

            ((!Duration.                   HasValue    && !FixedPowerFactor.Duration.                   HasValue) ||
               Duration.                   HasValue    &&  FixedPowerFactor.Duration.                   HasValue    && Duration.                   Value.Equals(FixedPowerFactor.Duration.                   Value)) &&

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

                   $"Priority '{Priority}'",

                   PercentageMaxDischargePower.HasValue
                       ? $", % max discharge power: {PercentageMaxDischargePower.Value}"
                       : "",

                   PowerMonitoringMustTrip is not null
                       ? $", curve: '{PowerMonitoringMustTrip}'"
                       : "",

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
