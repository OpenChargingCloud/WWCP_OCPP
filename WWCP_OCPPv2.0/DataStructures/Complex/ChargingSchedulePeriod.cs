/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A charging schedule period.
    /// </summary>
    public class ChargingSchedulePeriod : ACustomData,
                                          IEquatable<ChargingSchedulePeriod>
    {

        #region Properties

        /// <summary>
        /// The start of the period relative to the start of the charging schedule.
        /// This value also defines the stop time of the previous period.
        /// </summary>
        [Mandatory]
        public TimeSpan      StartPeriod     { get; }

        /// <summary>
        /// Power limit during the schedule period in Amperes.
        /// </summary>
        [Mandatory]
        public Decimal       Limit           { get; }

        /// <summary>
        /// The number of phases that can be used for charging.
        /// </summary>
        [Optional]
        public Byte?         NumberPhases    { get; }

        /// <summary>
        /// Optional electrical phase to use for charging.
        /// Only allowed when numberPhases is 1 and when the EVSE is capable of switching the phase connected to the EV,
        /// i.e. ACPhaseSwitchingSupported is defined and true.
        /// It’s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted,
        /// the charging station / EVSE will make the selection on its own.
        /// </summary>
        [Optional]
        public PhasesToUse?  PhaseToUse      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging schedule period.
        /// </summary>
        /// <param name="StartPeriod">The start of the period relative to the start of the charging schedule. This value also defines the stop time of the previous period.</param>
        /// <param name="Limit">Power limit during the schedule period in Amperes.</param>
        /// <param name="NumberPhases">The number of phases that can be used for charging.</param>
        /// <param name="PhaseToUse">Optional electrical phase to use for charging.</param>
        public ChargingSchedulePeriod(TimeSpan      StartPeriod,
                                      Decimal       Limit,
                                      Byte?         NumberPhases   = null,
                                      PhasesToUse?  PhaseToUse     = null,
                                      CustomData?   CustomData     = null)

            : base(CustomData)

        {

            this.StartPeriod   = StartPeriod;
            this.Limit         = Limit;
            this.NumberPhases  = NumberPhases;
            this.PhaseToUse    = PhaseToUse;

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
                                         out UInt32 startPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                var StartPeriod = TimeSpan.FromSeconds(startPeriod);

                #endregion

                #region Limit           [mandatory]

                if (!JSON.ParseMandatory("limit",
                                         "power limit",
                                         out Decimal Limit,
                                         out ErrorResponse))
                {
                    return false;
                }

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
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingSchedulePeriod = new ChargingSchedulePeriod(StartPeriod,
                                                                    Limit,
                                                                    NumberPhases,
                                                                    PhaseToUse,
                                                                    CustomData);

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

                           new JProperty("startPeriod",         (UInt32) Math.Round(StartPeriod.TotalSeconds, 0)),
                           new JProperty("limit",               Math.Round(Limit, 1)),

                           NumberPhases.HasValue
                               ? new JProperty("numberPhases",  NumberPhases)
                               : null,

                           PhaseToUse.HasValue
                               ? new JProperty("phaseToUse",    PhaseToUse.Value.AsNumber())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
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

            ((!NumberPhases.HasValue && !ChargingSchedulePeriod.NumberPhases.HasValue) ||
              (NumberPhases.HasValue &&  ChargingSchedulePeriod.NumberPhases.HasValue && NumberPhases.Value.Equals(ChargingSchedulePeriod.NumberPhases.Value))) &&

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
                      (NumberPhases?.GetHashCode() ?? 0) *  5 ^
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

                             NumberPhases.HasValue
                                 ? ", " + NumberPhases + " phases"
                                 : "",

                             PhaseToUse.HasValue
                                 ? ", using phase: " + PhaseToUse.Value.AsText()
                                 : "");

        #endregion

    }

}
