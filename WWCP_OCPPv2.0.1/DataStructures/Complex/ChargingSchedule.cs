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
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// A charging schedule.
    /// </summary>
    public class ChargingSchedule : ACustomData,
                                    IEquatable<ChargingSchedule>
    {

        #region Properties

        /// <summary>
        /// Unique identification of the charging schedule.
        /// </summary>
        [Mandatory]
        public ChargingSchedule_Id                  Id                         { get; }

        /// <summary>
        /// The unit of measure the charging limit is expressed in.
        /// </summary>
        [Mandatory]
        public ChargingRateUnits                    ChargingRateUnit           { get; }

        /// <summary>
        /// The enumeration of charging schedule periods defining the maximum power or current usage over time.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods    { get; }

        /// <summary>
        /// The optional starting timestamp of an absolute schedule.
        /// If absent the schedule will be relative to start of charging.
        /// </summary>
        [Optional]
        public DateTime?                            StartSchedule              { get; }

        /// <summary>
        /// Optional duration of the charging schedule.
        /// If the duration is left empty, the last period will continue indefinitely or until end of the transaction if chargingProfilePurpose = TxProfile.
        /// </summary>
        [Optional]
        public TimeSpan?                            Duration                   { get; }

        /// <summary>
        /// The optional minimal charging rate supported by the EV.
        /// The unit of measure is defined by the chargingRateUnit.
        /// This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates.
        /// Accepts at most one digit fraction (e.g. 8.1)
        /// </summary>
        [Optional]
        public Decimal?                             MinChargingRate            { get; }

        /// <summary>
        /// Optional sales tariff associated with this charging schedule.
        /// </summary>
        [Optional]
        public SalesTariff?                         SalesTariff                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging schedule.
        /// </summary>
        /// <param name="Id">Unique identification of the charging schedule.</param>
        /// <param name="ChargingRateUnit">The unit of measure the charging limit is expressed in.</param>
        /// <param name="ChargingSchedulePeriods">The enumeration of charging schedule periods defining the maximum power or current usage over time.</param>
        /// <param name="StartSchedule">The optional starting timestamp of an absolute schedule. If absent the schedule will be relative to start of charging.</param>
        /// <param name="Duration">Optional duration of the charging schedule. If the duration is left empty, the last period will continue indefinitely or until end of the transaction if chargingProfilePurpose = TxProfile.</param>
        /// <param name="MinChargingRate">The optional minimal charging rate supported by the EV.</param>
        /// <param name="SalesTariff">Optional sales tariff associated with this charging schedule.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingSchedule(ChargingSchedule_Id                  Id,
                                ChargingRateUnits                    ChargingRateUnit,
                                IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods,
                                DateTime?                            StartSchedule     = null,
                                TimeSpan?                            Duration          = null,
                                Decimal?                             MinChargingRate   = null,
                                SalesTariff?                         SalesTariff       = null,
                                CustomData?                          CustomData        = null)

            : base(CustomData)

        {

            if (!ChargingSchedulePeriods.Any())
                throw new ArgumentException("The given enumeration of charging schedules must not be empty!",
                                            nameof(ChargingSchedulePeriods));

            this.Id                       = Id;
            this.ChargingRateUnit         = ChargingRateUnit;
            this.ChargingSchedulePeriods  = ChargingSchedulePeriods.Distinct();
            this.StartSchedule            = StartSchedule;
            this.Duration                 = Duration;
            this.MinChargingRate          = MinChargingRate;
            this.SalesTariff              = SalesTariff;

        }

        #endregion


        #region Documentation

        // "ChargingScheduleType": {
        //   "description": "Charging_ Schedule\r\nurn:x-oca:ocpp:uid:2:233256\r\nCharging schedule structure defines a list of charging periods, as used in: GetCompositeSchedule.conf and ChargingProfile. \r\n",
        //   "javaType": "ChargingSchedule",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "Identifies the ChargingSchedule.\r\n",
        //       "type": "integer"
        //     },
        //     "startSchedule": {
        //       "description": "Charging_ Schedule. Start_ Schedule. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569237\r\nStarting point of an absolute schedule. If absent the schedule will be relative to start of charging.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "duration": {
        //       "description": "Charging_ Schedule. Duration. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569236\r\nDuration of the charging schedule in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction if chargingProfilePurpose = TxProfile.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingRateUnit": {
        //       "$ref": "#/definitions/ChargingRateUnitEnumType"
        //     },
        //     "chargingSchedulePeriod": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ChargingSchedulePeriodType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 1024
        //     },
        //     "minChargingRate": {
        //       "description": "Charging_ Schedule. Min_ Charging_ Rate. Numeric\r\nurn:x-oca:ocpp:uid:1:569239\r\nMinimum charging rate supported by the EV. The unit of measure is defined by the chargingRateUnit. This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates. Accepts at most one digit fraction (e.g. 8.1)\r\n",
        //       "type": "number"
        //     },
        //     "salesTariff": {
        //       "$ref": "#/definitions/SalesTariffType"
        //     }
        //   },
        //   "required": [
        //     "id",
        //     "chargingRateUnit",
        //     "chargingSchedulePeriod"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingScheduleParser">A delegate to parse custom charging schedules.</param>
        public static ChargingSchedule Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<ChargingSchedule>?  CustomChargingScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingSchedule,
                         out var errorResponse,
                         CustomChargingScheduleParser))
            {
                return chargingSchedule!;
            }

            throw new ArgumentException("The given JSON representation of a charging schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingSchedule, out ErrorResponse, CustomChargingScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out ChargingSchedule?  ChargingSchedule,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out ChargingSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingScheduleParser">A delegate to parse custom charging schedules.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out ChargingSchedule?                           ChargingSchedule,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingSchedule>?  CustomChargingScheduleParser)
        {

            try
            {

                ChargingSchedule = default;

                #region Id                         [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "charging schedule identification",
                                         ChargingSchedule_Id.TryParse,
                                         out ChargingSchedule_Id Id,
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

                #region StartSchedule              [optional]

                if (JSON.ParseOptional("startSchedule",
                                       "start schedule",
                                       out DateTime? StartSchedule,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region Duration                   [optional]

                if (JSON.ParseOptional("duration",
                                       "electrical phase to use",
                                       out UInt32? duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var Duration = duration.HasValue
                                   ? new TimeSpan?(TimeSpan.FromSeconds(duration.Value))
                                   : null;

                #endregion

                #region MinChargingRate            [optional]

                if (JSON.ParseOptional("minChargingRate",
                                       "minimal charging rate",
                                       out Decimal? MinChargingRate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SalesTariff                [optional]

                if (JSON.ParseOptionalJSON("salesTariff",
                                           "sales tariff",
                                           OCPPv2_0_1.SalesTariff.TryParse,
                                           out SalesTariff? SalesTariff,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingSchedule = new ChargingSchedule(
                                       Id,
                                       ChargingRateUnit,
                                       ChargingSchedulePeriods,
                                       StartSchedule,
                                       Duration,
                                       MinChargingRate,
                                       SalesTariff,
                                       CustomData
                                   );

                if (CustomChargingScheduleParser is not null)
                    ChargingSchedule = CustomChargingScheduleParser(JSON,
                                                                    ChargingSchedule);

                return true;

            }
            catch (Exception e)
            {
                ChargingSchedule  = default;
                ErrorResponse     = "The given JSON representation of a charging schedule is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomChargingScheduleSerializer = null, CustomChargingSchedulePeriodSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedule>?        CustomChargingScheduleSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?             CustomSalesTariffSerializer              = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?        CustomSalesTariffEntrySerializer         = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?    CustomRelativeTimeIntervalSerializer     = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?         CustomConsumptionCostSerializer          = null,
                              CustomJObjectSerializerDelegate<Cost>?                    CustomCostSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                       Id.                 ToString()),
                                 new JProperty("chargingRateUnit",         ChargingRateUnit.   AsText()),
                                 new JProperty("chargingSchedulePeriod",   new JArray(ChargingSchedulePeriods.Select(chargingSchedulePeriod => chargingSchedulePeriod.ToJSON(CustomChargingSchedulePeriodSerializer,
                                                                                                                                                                             CustomCustomDataSerializer)))),

                           StartSchedule.HasValue
                               ? new JProperty("startSchedule",            StartSchedule.Value.ToIso8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",                 (UInt64) Math.Round(Duration.Value.TotalSeconds, 0))
                               : null,

                           MinChargingRate.HasValue
                               ? new JProperty("minChargingRate",          MinChargingRate.Value)
                               : null,

                           SalesTariff is not null
                               ? new JProperty("salesTariff",              SalesTariff.        ToJSON(CustomSalesTariffSerializer,
                                                                                                      CustomSalesTariffEntrySerializer,
                                                                                                      CustomRelativeTimeIntervalSerializer,
                                                                                                      CustomConsumptionCostSerializer,
                                                                                                      CustomCostSerializer,
                                                                                                      CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingScheduleSerializer is not null
                       ? CustomChargingScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingSchedule1, ChargingSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedule1">A charging schedule.</param>
        /// <param name="ChargingSchedule2">Another charging schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingSchedule ChargingSchedule1,
                                           ChargingSchedule ChargingSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingSchedule1, ChargingSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingSchedule1 is null || ChargingSchedule2 is null)
                return false;

            return ChargingSchedule1.Equals(ChargingSchedule2);

        }

        #endregion

        #region Operator != (ChargingSchedule1, ChargingSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedule1">A charging schedule.</param>
        /// <param name="ChargingSchedule2">Another charging schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedule ChargingSchedule1,
                                           ChargingSchedule ChargingSchedule2)

            => !(ChargingSchedule1 == ChargingSchedule2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging schedules for equality..
        /// </summary>
        /// <param name="Object">A charging schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingSchedule chargingSchedule &&
                   Equals(chargingSchedule);

        #endregion

        #region Equals(ChargingSchedule)

        /// <summary>
        /// Compares two charging schedules for equality.
        /// </summary>
        /// <param name="ChargingSchedule">A charging schedule to compare with.</param>
        public Boolean Equals(ChargingSchedule? ChargingSchedule)

            => ChargingSchedule is not null &&

               Id.              Equals(ChargingSchedule.Id)               &&
               ChargingRateUnit.Equals(ChargingSchedule.ChargingRateUnit) &&

               ChargingSchedulePeriods.Count().Equals(ChargingSchedule.ChargingSchedulePeriods.Count())     &&
               ChargingSchedulePeriods.All(chargingSchedulePeriod => ChargingSchedule.ChargingSchedulePeriods.Contains(chargingSchedulePeriod)) &&

            ((!StartSchedule.  HasValue    && !ChargingSchedule.StartSchedule.  HasValue)    ||
              (StartSchedule.  HasValue    &&  ChargingSchedule.StartSchedule.  HasValue && StartSchedule.  Value.Equals(ChargingSchedule.StartSchedule.  Value))) &&

            ((!Duration.       HasValue    && !ChargingSchedule.Duration.       HasValue)    ||
              (Duration.       HasValue    &&  ChargingSchedule.Duration.       HasValue && Duration.       Value.Equals(ChargingSchedule.Duration.       Value))) &&

            ((!MinChargingRate.HasValue    && !ChargingSchedule.MinChargingRate.HasValue)    ||
              (MinChargingRate.HasValue    &&  ChargingSchedule.MinChargingRate.HasValue && MinChargingRate.Value.Equals(ChargingSchedule.MinChargingRate.Value))) &&

             ((SalesTariff     is     null &&  ChargingSchedule.SalesTariff     is     null) ||
              (SalesTariff     is not null &&  ChargingSchedule.SalesTariff     is not null && SalesTariff.       Equals(ChargingSchedule.SalesTariff)))           &&

               base.            Equals(ChargingSchedule);

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

                return Id.                     GetHashCode()       * 19 ^
                       ChargingRateUnit.       GetHashCode()       * 17 ^
                       ChargingSchedulePeriods.CalcHashCode()      * 13 ^
                      (StartSchedule?.         GetHashCode() ?? 0) * 11 ^
                      (Duration?.              GetHashCode() ?? 0) *  7 ^
                      (MinChargingRate?.       GetHashCode() ?? 0) *  5 ^
                      (SalesTariff?.           GetHashCode() ?? 0) *  3 ^

                       base.                   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,
                   " / ",
                   ChargingRateUnit.AsText(),

                   " with ", ChargingSchedulePeriods.Count(), " charging schedule periods"

               );

        #endregion

    }

}
