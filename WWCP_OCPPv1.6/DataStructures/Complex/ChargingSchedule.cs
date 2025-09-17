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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A charging schedule.
    /// </summary>
    public class ChargingSchedule : IEquatable<ChargingSchedule>
    {

        #region Properties

        /// <summary>
        /// The unit of measure the limit is expressed in.
        /// </summary>
        public ChargingRateUnits                    ChargingRateUnit           { get; }

        /// <summary>
        /// An enumeration of ChargingSchedulePeriods defining maximum power or current
        /// usage over time.
        /// </summary>
        public IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods    { get; }

        /// <summary>
        /// The optional duration of the charging schedule. If the duration is
        /// no defined, the last period will continue indefinitely or until end of the
        /// transaction in case startSchedule is also undefined.
        /// </summary>
        public TimeSpan?                            Duration                   { get; }

        /// <summary>
        /// The optional starting point of an absolute schedule. If absent the schedule
        /// will be relative to start of charging.
        /// </summary>
        public DateTimeOffset?                      StartSchedule              { get; }

        /// <summary>
        /// An optional minimum charging rate supported by the electric vehicle. The unit
        /// of measure is defined by the chargingRateUnit. This parameter is intended to
        /// be used by a local smart charging algorithm to optimize the power allocation
        /// for in the case a charging process is inefficient at lower charging rates.
        /// </summary>
        public Decimal?                             MinChargingRate            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a charging schedule.
        /// </summary>
        /// <param name="ChargingRateUnit">The unit of measure the limit is expressed in.</param>
        /// <param name="ChargingSchedulePeriods">An enumeration of ChargingSchedulePeriods defining maximum power or current usage over time.</param>
        /// <param name="Duration">The optional duration of the charging schedule. If the duration is no defined, the last period will continue indefinitely or until end of the transaction in case startSchedule is also undefined.</param>
        /// <param name="StartSchedule">The optional starting point of an absolute schedule. If absent the schedule will be relative to start of charging.</param>
        /// <param name="MinChargingRate">An optional minimum charging rate supported by the electric vehicle. The unit of measure is defined by the chargingRateUnit. This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates.</param>
        public ChargingSchedule(ChargingRateUnits                    ChargingRateUnit,
                                IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods,
                                TimeSpan?                            Duration          = null,
                                DateTimeOffset?                      StartSchedule     = null,
                                Decimal?                             MinChargingRate   = null)
        {

            #region Initial checks

            if (ChargingSchedulePeriods.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargingSchedulePeriods),  "The given enumeration of charging schedule periods must not be null or empty!");

            #endregion

            this.ChargingRateUnit         = ChargingRateUnit;
            this.ChargingSchedulePeriods  = ChargingSchedulePeriods;
            this.Duration                 = Duration;
            this.StartSchedule            = StartSchedule;
            this.MinChargingRate          = MinChargingRate;

        }

        #endregion


        #region Documentation

        // <ns:chargingSchedule>
        //
        //    <!--Optional:-->
        //    <ns:duration>?</ns:duration>
        //
        //    <!--Optional:-->
        //    <ns:startSchedule>?</ns:startSchedule>
        //    <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //    <!--1 or more repetitions:-->
        //    <ns:chargingSchedulePeriod>
        //
        //       <ns:startPeriod>?</ns:startPeriod>
        //       <ns:limit>?</ns:limit>
        //
        //       <!--Optional:-->
        //       <ns:numberPhases>?</ns:numberPhases>
        //
        //    </ns:chargingSchedulePeriod>
        //
        //    <!--Optional:-->
        //    <ns:minChargingRate>?</ns:minChargingRate>
        //
        // </ns:chargingSchedule>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionRequest",
        //     "title":   "chargingSchedule",
        //     "type":    "object",
        //     "properties": {
        //         "duration": {
        //             "type": "integer"
        //         },
        //         "startSchedule": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "chargingRateUnit": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "A",
        //                 "W"
        //             ]
        //         },
        //         "chargingSchedulePeriod": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "startPeriod": {
        //                         "type": "integer"
        //                     },
        //                     "limit": {
        //                         "type": "number",
        //                         "multipleOf" : 0.1
        //                     },
        //                     "numberPhases": {
        //                         "type": "integer"
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "startPeriod",
        //                     "limit"
        //                 ]
        //             }
        //         },
        //         "minChargingRate": {
        //             "type": "number",
        //             "multipleOf" : 0.1
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "chargingRateUnit",
        //         "chargingSchedulePeriod"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML)

        /// <summary>
        /// Parse the given XML representation of a charging schedule.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedule Parse(XElement XML)
        {

            if (TryParse(XML,
                         out var chargingSchedule,
                         out var errorResponse))
            {
                return chargingSchedule;
            }

            throw new ArgumentException("The given XML representation of an authorize request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomChargingScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingScheduleParser">An optional delegate to parse custom charging schedules.</param>
        public static ChargingSchedule Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<ChargingSchedule>?  CustomChargingScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingSchedule,
                         out var errorResponse,
                         CustomChargingScheduleParser))
            {
                return chargingSchedule;
            }

            throw new ArgumentException("The given JSON representation of a charging schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out ChargingSchedule, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a charging schedule.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                    XML,
                                       [NotNullWhen(true)]  out ChargingSchedule?  ChargingSchedule,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                ChargingSchedule = new ChargingSchedule(

                                       XML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "chargingRateUnit",
                                                               ChargingRateUnitsExtensions.Parse),

                                       XML.MapElementsOrFail2 (OCPPNS.OCPPv1_6_CP + "chargingSchedulePeriod",
                                                               ChargingSchedulePeriod.Parse),

                                       XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "duration",
                                                               s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                       XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "startSchedule",
                                                               DateTime.Parse),

                                       XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "minChargingRate",
                                                               Decimal.Parse)

                                   );

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

        #region (static) TryParse(JSON, out ChargingSchedule, out ErrorResponse, CustomChargingScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out ChargingSchedule?  ChargingSchedule,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

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
        /// <param name="CustomChargingScheduleParser">An optional delegate to parse custom Authorize requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out ChargingSchedule?      ChargingSchedule,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingSchedule>?  CustomChargingScheduleParser)
        {

            try
            {

                ChargingSchedule = null;

                #region ChargingRateUnit

                if (!JSON.MapMandatory("chargingRateUnit",
                                       "charging rate unit",
                                       ChargingRateUnitsExtensions.Parse,
                                       out ChargingRateUnits ChargingRateUnit,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedulePeriods

                if (!JSON.ParseMandatoryList("chargingSchedulePeriod",
                                             "charging schedule period",
                                             ChargingSchedulePeriod.TryParse,
                                             out List<ChargingSchedulePeriod> ChargingSchedulePeriods,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Duration

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StartSchedule

                if (JSON.ParseOptional("startSchedule",
                                       "start schedule",
                                       out DateTime? StartSchedule,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargingRate

                if (JSON.ParseOptional("minChargingRate",
                                       "min charging rate",
                                       out Decimal? MinChargingRate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingSchedule = new ChargingSchedule(
                                       ChargingRateUnit,
                                       ChargingSchedulePeriods,
                                       Duration,
                                       StartSchedule,
                                       MinChargingRate
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

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingSchedule"]</param>
        public XElement ToXML(XName? XName = null)

            => new (XName ?? OCPPNS.OCPPv1_6_CP + "chargingSchedule",

                   Duration.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "duration",         (Int32) Math.Round(Duration.Value.TotalSeconds, 0))
                       : null,

                   StartSchedule.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "startSchedule",    StartSchedule.Value.ToISO8601())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",       ChargingRateUnit.AsText()),

                   ChargingSchedulePeriods.Select(value => value.ToXML()),

                   MinChargingRate.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "minChargingRate",  MinChargingRate.Value.ToString("0.#"))
                       : null

               );

        #endregion

        #region ToJSON(CustomChargingScheduleSerializer = null, CustomChargingSchedulePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedule>?        CustomChargingScheduleSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer   = null)
        {

            var json = JSONObject.Create(

                           Duration.HasValue
                               ? new JProperty("duration",                 (Int32) Math.Round(Duration.Value.TotalSeconds, 0))
                               : null,

                           StartSchedule.HasValue
                               ? new JProperty("startSchedule",            StartSchedule.Value.ToISO8601())
                               : null,

                                 new JProperty("chargingRateUnit",         ChargingRateUnit.AsText()),

                                 new JProperty("chargingSchedulePeriod",   ChargingSchedulePeriods.Select(chargingSchedulePeriod => chargingSchedulePeriod.ToJSON(CustomChargingSchedulePeriodSerializer))),

                           MinChargingRate.HasValue
                               ? new JProperty("minChargingRate",          MinChargingRate.Value.ToString("0.#"))
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
        public static Boolean operator == (ChargingSchedule? ChargingSchedule1,
                                           ChargingSchedule? ChargingSchedule2)
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
        public static Boolean operator != (ChargingSchedule? ChargingSchedule1,
                                           ChargingSchedule? ChargingSchedule2)

            => !(ChargingSchedule1 == ChargingSchedule2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging schedules for equality.
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

               ChargingRateUnit.Equals(ChargingSchedule.ChargingRateUnit) &&

               ChargingSchedulePeriods.Count().Equals(ChargingSchedule.ChargingSchedulePeriods.Count()) &&
               ChargingSchedulePeriods.All(chargingSchedulePeriod => ChargingSchedule.ChargingSchedulePeriods.Contains(chargingSchedulePeriod)) &&

               ((!Duration.       HasValue && !ChargingSchedule.Duration.       HasValue) ||
                 (Duration.       HasValue &&  ChargingSchedule.Duration.       HasValue && Duration.       Value.Equals(ChargingSchedule.Duration.       Value))) &&

               ((!StartSchedule.  HasValue && !ChargingSchedule.StartSchedule.  HasValue) ||
                 (StartSchedule.  HasValue &&  ChargingSchedule.StartSchedule.  HasValue && StartSchedule.  Value.Equals(ChargingSchedule.StartSchedule.  Value))) &&

               ((!MinChargingRate.HasValue && !ChargingSchedule.MinChargingRate.HasValue) ||
                 (MinChargingRate.HasValue &&  ChargingSchedule.MinChargingRate.HasValue && MinChargingRate.Value.Equals(ChargingSchedule.MinChargingRate.Value)));

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

                return ChargingRateUnit.       GetHashCode()       * 13 ^
                       ChargingSchedulePeriods.CalcHashCode()      * 11 ^

                       (Duration?.             GetHashCode() ?? 0) *  7 ^
                       (StartSchedule?.        GetHashCode() ?? 0) *  5 ^
                       (MinChargingRate?.      GetHashCode() ?? 0) *  3;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingRateUnit,
                             " / ",
                             ChargingSchedulePeriods.Count(),
                             " charging schedule period(s)");

        #endregion

    }

}
