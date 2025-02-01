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
    /// A composite schedule.
    /// </summary>
    public class CompositeSchedule : ACustomData,
                                     IEquatable<CompositeSchedule>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the EVSE for which the schedule is requested.
        /// When evseid is 0, the charging station calculated the expected consumption for the grid connection.
        /// </summary>
        public EVSE_Id                              EVSEId                     { get; }

        /// <summary>
        /// The duration of the schedule.
        /// </summary>
        public TimeSpan                             Duration                   { get; }

        /// <summary>
        /// The starting point of schedule.
        /// All time measurements within the schedule are relative to this timestamp.
        /// </summary>
        public DateTime                             ScheduleStart              { get; }

        /// <summary>
        /// The unit of measure the limit is expressed in.
        /// </summary>
        public ChargingRateUnits                    ChargingRateUnit           { get; }

        /// <summary>
        /// An enumeration of composite schedule periods defining maximum power or
        /// current usage over time.
        /// </summary>
        public IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a composite schedule.
        /// </summary>
        /// <param name="EVSEId">The unique identification of the EVSE for which the schedule is requested. When evseid is 0, the charging station calculated the expected consumption for the grid connection.</param>
        /// <param name="Duration">The duration of the schedule.</param>
        /// <param name="ScheduleStart">The starting point of schedule. All time measurements within the schedule are relative to this timestamp.</param>
        /// <param name="ChargingRateUnit">The unit of measure the limit is expressed in.</param>
        /// <param name="ChargingSchedulePeriods">An enumeration of composite schedule periods defining maximum power or current usage over time.</param>
        public CompositeSchedule(EVSE_Id                              EVSEId,
                                 TimeSpan                             Duration,
                                 DateTime                             ScheduleStart,
                                 ChargingRateUnits                    ChargingRateUnit,
                                 IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods,

                                 CustomData?                          CustomData   = null)

            : base(CustomData)

        {

            if (!ChargingSchedulePeriods.Any())
                throw new ArgumentException("The given enumeration of charging schedules must not be empty!",
                                            nameof(ChargingSchedulePeriods));

            this.EVSEId                   = EVSEId;
            this.Duration                 = Duration;
            this.ScheduleStart            = ScheduleStart;
            this.ChargingRateUnit         = ChargingRateUnit;
            this.ChargingSchedulePeriods  = ChargingSchedulePeriods.Distinct();

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "CompositeSchedule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evseId": {
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "duration": {
        //             "type": "integer"
        //         },
        //         "scheduleStart": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "chargingRateUnit": {
        //             "$ref": "#/definitions/ChargingRateUnitEnumType"
        //         },
        //         "chargingSchedulePeriod": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ChargingSchedulePeriodType"
        //             },
        //             "minItems": 1
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "evseId",
        //         "duration",
        //         "scheduleStart",
        //         "chargingRateUnit",
        //         "chargingSchedulePeriod"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCompositeScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a composite schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCompositeScheduleParser">A delegate to parse custom composite schedules.</param>
        public static CompositeSchedule Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<CompositeSchedule>?  CustomCompositeScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var compositeSchedule,
                         out var errorResponse,
                         CustomCompositeScheduleParser) &&
                compositeSchedule is not null)
            {
                return compositeSchedule;
            }

            throw new ArgumentException("The given JSON representation of a composite schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CompositeSchedule, out ErrorResponse, CustomCompositeScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a composite schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CompositeSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out CompositeSchedule?  CompositeSchedule,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out CompositeSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a composite schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CompositeSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCompositeScheduleParser">A delegate to parse custom Authorize requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out CompositeSchedule?      CompositeSchedule,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<CompositeSchedule>?  CustomCompositeScheduleParser)
        {

            try
            {

                CompositeSchedule = null;

                #region EVSEId                     [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Duration                   [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out TimeSpan Duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ScheduleStart              [mandatory]

                if (!JSON.ParseMandatory("scheduleStart",
                                         "schedule start timestamp",
                                         out DateTime ScheduleStart,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingRateUnit           [mandatory]

                if (!JSON.ParseMandatory("chargingRateUnit",
                                         "charging rate unit",
                                         ChargingRateUnitsExtensions.TryParse,
                                         out ChargingRateUnits ChargingRateUnit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedulePeriods    [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingSchedulePeriod",
                                                "charging schedule periods",
                                                ChargingSchedulePeriod.TryParse,
                                                out HashSet<ChargingSchedulePeriod> ChargingSchedulePeriods,
                                                out ErrorResponse))
                {
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


                CompositeSchedule = new CompositeSchedule(
                                        EVSEId,
                                        Duration,
                                        ScheduleStart,
                                        ChargingRateUnit,
                                        ChargingSchedulePeriods,
                                        CustomData
                                    );

                if (CustomCompositeScheduleParser is not null)
                    CompositeSchedule = CustomCompositeScheduleParser(JSON,
                                                                      CompositeSchedule);

                return true;

            }
            catch (Exception e)
            {
                CompositeSchedule  = default;
                ErrorResponse      = "The given JSON representation of a composite schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCompositeScheduleSerializer = null, CustomChargingSchedulePeriodSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCompositeScheduleSerializer">A delegate to serialize custom composite schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CompositeSchedule>?       CustomCompositeScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",                    EVSEId.Value),
                                 new JProperty("duration",                  (UInt32) Math.Round(Duration.TotalSeconds, 0)),
                                 new JProperty("scheduleStart",             ScheduleStart.   ToIso8601()),
                                 new JProperty("chargingRateUnit",          ChargingRateUnit.AsText()),
                                 new JProperty("compositeSchedulePeriod",   new JArray(ChargingSchedulePeriods.Select(chargingSchedulePeriod => chargingSchedulePeriod.ToJSON(CustomChargingSchedulePeriodSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCompositeScheduleSerializer is not null
                       ? CustomCompositeScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CompositeSchedule1, CompositeSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CompositeSchedule1">A composite schedule.</param>
        /// <param name="CompositeSchedule2">Another composite schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CompositeSchedule? CompositeSchedule1,
                                           CompositeSchedule? CompositeSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CompositeSchedule1, CompositeSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (CompositeSchedule1 is null || CompositeSchedule2 is null)
                return false;

            return CompositeSchedule1.Equals(CompositeSchedule2);

        }

        #endregion

        #region Operator != (CompositeSchedule1, CompositeSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CompositeSchedule1">A composite schedule.</param>
        /// <param name="CompositeSchedule2">Another composite schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CompositeSchedule? CompositeSchedule1,
                                           CompositeSchedule? CompositeSchedule2)

            => !(CompositeSchedule1 == CompositeSchedule2);

        #endregion

        #endregion

        #region IEquatable<CompositeSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two composite schedules for equality.
        /// </summary>
        /// <param name="Object">A composite schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CompositeSchedule compositeSchedule &&
                   Equals(compositeSchedule);

        #endregion

        #region Equals(CompositeSchedule)

        /// <summary>
        /// Compares two composite schedules for equality.
        /// </summary>
        /// <param name="CompositeSchedule">A composite schedule to compare with.</param>
        public Boolean Equals(CompositeSchedule? CompositeSchedule)

            => CompositeSchedule is not null&&

               EVSEId.          Equals(CompositeSchedule.EVSEId)           &&
               Duration.        Equals(CompositeSchedule.Duration)         &&
               ScheduleStart.   Equals(CompositeSchedule.ScheduleStart)    &&
               ChargingRateUnit.Equals(CompositeSchedule.ChargingRateUnit) &&

               ChargingSchedulePeriods.Count().Equals(CompositeSchedule.ChargingSchedulePeriods.Count()) &&
               ChargingSchedulePeriods.All(chargingSchedulePeriod => CompositeSchedule.ChargingSchedulePeriods.Contains(chargingSchedulePeriod));

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

                return EVSEId.          GetHashCode() * 13 ^
                       Duration.        GetHashCode() * 11 ^
                       ScheduleStart.   GetHashCode() *  7 ^
                       ChargingRateUnit.GetHashCode() *  5 ^
                       //ToDo: ChargingSchedulePeriods

                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   "EVSE ", EVSEId,
                   ", for ", Duration.TotalSeconds, " seconds ",
                   "starting at " , ScheduleStart.ToIso8601(),
                   ", with limit '", ChargingRateUnit,
                   "' and having ", ChargingSchedulePeriods.Count(), " charging schedule period(s)"
               );

        #endregion

    }

}
