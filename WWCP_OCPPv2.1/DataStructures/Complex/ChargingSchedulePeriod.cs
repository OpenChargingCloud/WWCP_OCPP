/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using System.Runtime.ConstrainedExecution;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging schedule period.
    /// </summary>
    public class ChargingSchedulePeriod : ACustomData,
                                          IEquatable<ChargingSchedulePeriod>
    {

        //ToDo: Implement me!

        #region Properties

        /// <summary>
        /// The start of the period relative to the start of the charging schedule.
        /// This value also defines the stop time of the previous period.
        /// </summary>
        [Mandatory]
        public TimeSpan                         StartPeriod               { get; }

        /// <summary>
        /// The number of phases that can be used for charging.
        /// </summary>
        [Optional]
        public Byte?                            NumberOfPhases            { get; }

        /// <summary>
        /// Optional electrical phase to use for charging.
        /// Only allowed when numberPhases is 1 and when the EVSE is capable of switching the phase connected to the EV,
        /// i.e. ACPhaseSwitchingSupported is defined and true.
        /// It’s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted,
        /// the charging station / EVSE will make the selection on its own.
        /// </summary>
        [Optional]
        public PhasesToUse?                     PhaseToUse                { get; }


        /// <summary>
        /// Optional charging rate limit during the schedule period is given
        /// in the applicable chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?               Limit                     { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?               Limit_L2                  { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?               Limit_L3                  { get; }


        /// <summary>
        /// Optional discharging limit in chargingRateUnit that the EV is allowed to discharge
        /// with.Note, these are negative values in order to be consistent with setpoint, which
        /// can be positive and negative.
        /// For AC this field represents the sum of all phases, unless values are provided for
        /// L2 and L3, in which case this field represents phase L1.
        /// </summary>
        public ChargingRateValue?               DischargeLimit            { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L2.
        /// </summary>
        public ChargingRateValue?               DischargeLimit_L2         { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L3.
        /// </summary>
        public ChargingRateValue?               DischargeLimit_L3         { get; }


        /// <summary>
        /// Optional setpoint in chargingRateUnit that the EV should follow as close as possible.
        /// Use negative values for discharging.
        /// For AC this field represents the sum of all phases, unless values are provided for L2
        /// and L3, in which case this field represents phase L1.
        /// In LocalFrequency mode, the value of setpoint is calculated automatically based on
        /// v2xBaseline and the power value from v2xFreqWattCurve and/or v2xSignalWattCurve.
        /// </summary>
        public ChargingRateValue?               Setpoint                  { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L2.
        /// </summary>
        public ChargingRateValue?               Setpoint_L2               { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L3.
        /// </summary>
        public ChargingRateValue?               Setpoint_L3               { get; }


        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit that the EV
        /// should follow as close as possible. Positive values for inductive, negative for
        /// capacitive reactive power or current.
        /// For AC this field represents the sum of all phases, unless values are provided for
        /// L2 and L3, in which case this field represents phase L1.
        /// </summary>
        public ChargingRateValue?               SetpointReactive          { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.
        /// </summary>
        public ChargingRateValue?               SetpointReactive_L2       { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.
        /// </summary>
        public ChargingRateValue?               SetpointReactive_L3       { get; }


        /// <summary>
        /// The optional indication whether the EV should attempt to keep its battery management system
        /// preconditioned for this time interval, such that the EV can charge/discharge at requested power immediately.
        /// </summary>
        public Boolean?                         PreconditioningRequest    { get; }

        /// <summary>
        /// The optional V2X operation mode that should be used during this time interval.
        /// When absent it defaults to "charging only".
        /// </summary>
        public OperationModes?                  OperationMode             { get; }

        /// <summary>
        /// The optional power baseline value that is used on top of all values of
        /// the V2X Frequency-Watt and V2X Signal-Watt curve.
        /// </summary>
        public Decimal?                         V2XBaseline               { get; }

        /// <summary>
        /// The optional power frequency curve used, but not required, when operationMode = LocalFrequency.
        /// When used it must contain at least two coordinates to specify a power frequency curve to use during this period.
        /// The curve determines the value of setpoint power for a given frequency.
        /// The charging rate unit must be W for local frequency control.
        /// It allows for 20 points, instead of the normal maximum of 10 for DER curves.
        /// </summary>
        public IEnumerable<V2XFreqWattEntry>    V2XFreqWattCurve          { get; }

        /// <summary>
        /// The optional power frequency curve used, but not required, when operationMode = LocalFrequency.
        /// When used it must contain at least two coordinates to specify a power frequency curve to use during this period.
        /// The curve determines the value of setpoint power for a given signal.
        /// The charging rate unit must be W for local frequency control.
        /// </summary>
        public IEnumerable<V2XSignalWattEntry>  V2XSignalWattCurve        { get; }

        /// <summary>
        /// The optional timestamp when this charging schedule period had been updated last.
        /// Only relevant in a dynamic charging profile.
        /// </summary>
        public DateTime?                        DynUpdateTime             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging schedule period.
        /// </summary>
        /// <param name="StartPeriod">The start of the period relative to the start of the charging schedule. This value also defines the stop time of the previous period.</param>
        /// <param name="Limit">Power limit during the schedule period in Amperes.</param>
        /// <param name="NumberOfPhases">The number of phases that can be used for charging.</param>
        /// <param name="PhaseToUse">Optional electrical phase to use for charging.</param>
        /// 
        /// <param name="Limit">Optional charging rate limit in chargingRateUnit.</param>
        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2.</param>
        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit.</param>
        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2.</param>
        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="PreconditioningRequest">The optional indication whether the EV should attempt to keep its battery management system preconditioned for this time interval, such that the EV can charge/discharge at requested power immediately.</param>
        /// <param name="OperationMode">The optional V2X operation mode that should be used during this time interval. When absent it defaults to "charging only".</param>
        /// <param name="V2XBaseline">The optional power baseline value that is used on top of all values of the V2X Frequency-Watt and V2X Signal-Watt curve.</param>
        /// <param name="V2XFreqWattCurve">The optional power frequency curve used, but not required, when operationMode = LocalFrequency.</param>
        /// <param name="V2XSignalWattCurve">An optional power frequency curve used, but not required, when operationMode = LocalFrequency.</param>
        /// <param name="DynUpdateTime">An optional timestamp when this charging schedule period had been updated last. Only relevant in a dynamic charging profile.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingSchedulePeriod(TimeSpan                          StartPeriod,
                                      Byte?                             NumberOfPhases           = null,
                                      PhasesToUse?                      PhaseToUse               = null,

                                      ChargingRateValue?                Limit                    = null,
                                      ChargingRateValue?                Limit_L2                 = null,
                                      ChargingRateValue?                Limit_L3                 = null,

                                      ChargingRateValue?                DischargeLimit           = null,
                                      ChargingRateValue?                DischargeLimit_L2        = null,
                                      ChargingRateValue?                DischargeLimit_L3        = null,

                                      ChargingRateValue?                Setpoint                 = null,
                                      ChargingRateValue?                Setpoint_L2              = null,
                                      ChargingRateValue?                Setpoint_L3              = null,

                                      ChargingRateValue?                SetpointReactive         = null,
                                      ChargingRateValue?                SetpointReactive_L2      = null,
                                      ChargingRateValue?                SetpointReactive_L3      = null,

                                      Boolean?                          PreconditioningRequest   = null,
                                      OperationModes?                   OperationMode            = null,
                                      Decimal?                          V2XBaseline              = null,
                                      IEnumerable<V2XFreqWattEntry>?    V2XFreqWattCurve         = null,
                                      IEnumerable<V2XSignalWattEntry>?  V2XSignalWattCurve       = null,
                                      DateTime?                         DynUpdateTime            = null,

                                      CustomData?                       CustomData               = null)

            : base(CustomData)

        {

            this.StartPeriod          = StartPeriod;
            this.NumberOfPhases       = NumberOfPhases;
            this.PhaseToUse           = PhaseToUse;

            this.Limit                = Limit;
            this.Limit_L2             = Limit_L2;
            this.Limit_L3             = Limit_L3;

            this.DischargeLimit       = DischargeLimit;
            this.DischargeLimit_L2    = DischargeLimit_L2;
            this.DischargeLimit_L3    = DischargeLimit_L3;

            this.Setpoint             = Setpoint;
            this.Setpoint_L2          = Setpoint_L2;
            this.Setpoint_L3          = Setpoint_L3;

            this.SetpointReactive     = SetpointReactive;
            this.SetpointReactive_L2  = SetpointReactive_L2;
            this.SetpointReactive_L3  = SetpointReactive_L3;

        }

        #endregion


        #region Documentation

        // "ChargingSchedulePeriodType": {
        //   "description": "Charging_ Schedule_ Period\r\nurn:x-oca:ocpp:uid:2:233257\r\nCharging schedule period structure defines a time period in a charging schedule.\r\n",
        //   "javaType": "ChargingSchedulePeriod",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "startPeriod": {
        //       "description": "Charging_ Schedule_ Period. Start_ Period. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569240\r\nStart of the period, in seconds from the start of schedule. The value of StartPeriod also defines the stop time of the previous period.\r\n",
        //       "type": "integer"
        //     },
        //     "limit": {
        //       "description": "Charging_ Schedule_ Period. Limit. Measure\r\nurn:x-oca:ocpp:uid:1:569241\r\nCharging rate limit during the schedule period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).\r\n",
        //       "type": "number"
        //     },
        //     "numberPhases": {
        //       "description": "Charging_ Schedule_ Period. Number_ Phases. Counter\r\nurn:x-oca:ocpp:uid:1:569242\r\nThe number of phases that can be used for charging. If a number of phases is needed, numberPhases=3 will be assumed unless another number is given.\r\n",
        //       "type": "integer"
        //     },
        //     "phaseToUse": {
        //       "description": "Values: 1..3, Used if numberPhases=1 and if the EVSE is capable of switching the phase connected to the EV, i.e. ACPhaseSwitchingSupported is defined and true. It’s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted, the Charging Station / EVSE will make the selection on its own.\r\n\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "startPeriod",
        //     "limit"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingSchedulePeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingSchedulePeriodParser">A delegate to parse custom CustomChargingSchedulePeriod JSON objects.</param>
        public static ChargingSchedulePeriod Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingSchedulePeriod,
                         out var errorResponse,
                         CustomChargingSchedulePeriodParser))
            {
                return chargingSchedulePeriod!;
            }

            throw new ArgumentException("The given JSON representation of a charging schedule period is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingSchedulePeriod, out ErrorResponse, CustomChargingSchedulePeriodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       out ChargingSchedulePeriod?  ChargingSchedulePeriod,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        out ChargingSchedulePeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingSchedulePeriodParser">A delegate to parse custom CustomChargingSchedulePeriod JSON objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       out ChargingSchedulePeriod?                           ChargingSchedulePeriod,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodParser)
        {

            try
            {

                ChargingSchedulePeriod = default;

                #region StartPeriod     [mandatory]

                if (!JSON.ParseMandatory("startPeriod",
                                         "start period",
                                         out TimeSpan StartPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Limit           [mandatory]

                if (!JSON.ParseMandatory("limit",
                                         "power limit",
                                         out Decimal limit,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Limit = ChargingRateValue.Parse(limit, ChargingRateUnits.Unknown);

                #endregion

                #region NumberPhases    [optional]

                if (JSON.ParseOptional("numberPhases",
                                       "number of phases",
                                       out Byte? NumberPhases,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region PhaseToUse      [optional]

                if (JSON.ParseOptional("phaseToUse",
                                       "electrical phase to use",
                                       PhasesToUseExtensions.TryParse,
                                       out PhasesToUse? PhaseToUse,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData      [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingSchedulePeriod = new ChargingSchedulePeriod(
                                             StartPeriod,
                                             NumberPhases,
                                             PhaseToUse,
                                             CustomData: CustomData
                                         );

                if (CustomChargingSchedulePeriodParser is not null)
                    ChargingSchedulePeriod = CustomChargingSchedulePeriodParser(JSON,
                                                                                ChargingSchedulePeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingSchedulePeriod  = default;
                ErrorResponse           = "The given JSON representation of a charging schedule period is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomChargingSchedulePeriodSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("startPeriod",    (UInt32) Math.Round(StartPeriod.TotalSeconds, 0)),

                           Limit.HasValue
                               ? new JProperty("limit",          Math.Round(Limit.Value.Value, 1))
                               : null,

                           NumberOfPhases.HasValue
                               ? new JProperty("numberPhases",   NumberOfPhases)
                               : null,

                           PhaseToUse.HasValue
                               ? new JProperty("phaseToUse",     PhaseToUse.Value.AsNumber())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingSchedulePeriodSerializer is not null
                       ? CustomChargingSchedulePeriodSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingSchedulePeriod1, ChargingSchedulePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedulePeriod1">A charging schedule period.</param>
        /// <param name="ChargingSchedulePeriod2">Another charging schedule period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingSchedulePeriod ChargingSchedulePeriod1,
                                           ChargingSchedulePeriod ChargingSchedulePeriod2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingSchedulePeriod1, ChargingSchedulePeriod2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingSchedulePeriod1 is null || ChargingSchedulePeriod2 is null)
                return false;

            return ChargingSchedulePeriod1.Equals(ChargingSchedulePeriod2);

        }

        #endregion

        #region Operator != (ChargingSchedulePeriod1, ChargingSchedulePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedulePeriod1">A charging schedule period.</param>
        /// <param name="ChargingSchedulePeriod2">Another charging schedule period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedulePeriod ChargingSchedulePeriod1,
                                           ChargingSchedulePeriod ChargingSchedulePeriod2)

            => !(ChargingSchedulePeriod1 == ChargingSchedulePeriod2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedulePeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging schedule periods for equality..
        /// </summary>
        /// <param name="Object">A charging schedule period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingSchedulePeriod chargingSchedulePeriod &&
                   Equals(chargingSchedulePeriod);

        #endregion

        #region Equals(ChargingSchedulePeriod)

        /// <summary>
        /// Compares two charging schedule periods for equality.
        /// </summary>
        /// <param name="ChargingSchedulePeriod">A charging schedule period to compare with.</param>
        public Boolean Equals(ChargingSchedulePeriod? ChargingSchedulePeriod)

            => ChargingSchedulePeriod is not null &&

               StartPeriod.Equals(ChargingSchedulePeriod.StartPeriod) &&
               Limit.      Equals(ChargingSchedulePeriod.Limit)       &&

            ((!NumberOfPhases.HasValue && !ChargingSchedulePeriod.NumberOfPhases.HasValue) ||
              (NumberOfPhases.HasValue &&  ChargingSchedulePeriod.NumberOfPhases.HasValue && NumberOfPhases.Value.Equals(ChargingSchedulePeriod.NumberOfPhases.Value))) &&

            ((!PhaseToUse.  HasValue && !ChargingSchedulePeriod.PhaseToUse.  HasValue) ||
              (PhaseToUse.  HasValue &&  ChargingSchedulePeriod.PhaseToUse.  HasValue && PhaseToUse.  Value.Equals(ChargingSchedulePeriod.PhaseToUse.  Value))) &&

               base.       Equals(ChargingSchedulePeriod);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return StartPeriod.  GetHashCode()       * 11 ^
                       Limit.        GetHashCode()       *  7 ^
                      (NumberOfPhases?.GetHashCode() ?? 0) *  5 ^
                      (PhaseToUse?.  GetHashCode() ?? 0) *  3 ^

                       base.         GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartPeriod,
                             " / ",
                             " with ", Limit, " Ampere",

                             NumberOfPhases.HasValue
                                 ? ", " + NumberOfPhases + " phases"
                                 : "",

                             PhaseToUse.HasValue
                                 ? ", using phase: " + PhaseToUse.Value.AsText()
                                 : "");

        #endregion

    }

}
