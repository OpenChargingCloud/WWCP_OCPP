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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Charging tariff restrictions.
    /// </summary>
    public class TariffRestrictions : IEquatable<TariffRestrictions>
    {

        #region Properties

        /// <summary>
        /// The start date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateTime?                        NotBefore                  { get; }

        /// <summary>
        /// The end date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateTime?                        NotAfter                   { get; }



        /// <summary>
        /// The day(s) of the week this tariff element is active.
        /// </summary>
        [Optional]
        public IEnumerable<DayOfWeek>           DaysOfWeek                 { get; }

        /// <summary>
        /// The start time of day, for example "13:30", valid from this time of the day.
        /// </summary>
        [Optional]
        public Time?                            StartTimeOfDay             { get; }

        /// <summary>
        /// The end time of day, for example "19:45", valid from this time of the day.
        /// </summary>
        [Optional]
        public Time?                            EndTimeOfDay               { get; }



        /// <summary>
        /// The optional allowed current type: AC, DC, or both.
        /// </summary>
        [Optional]
        public CurrentTypes?                    CurrentType                { get; }

        /// <summary>
        /// The minimum consumed energy in kWh, for example 20, valid from this amount of energy
        /// (inclusive) being used.
        /// </summary>
        [Optional]
        public WattHour?                        MinEnergy                  { get; }

        /// <summary>
        /// The maximum consumed energy in kWh, for example 50, valid until this amount of energy
        /// (exclusive) being used.
        /// </summary>
        [Optional]
        public WattHour?                        MaxEnergy                  { get; }

        /// <summary>
        /// The sum of the minimum current (in Amperes) over all phases, for example 5. When the EV is
        /// charging with more than, or equal to, the defined amount of current, this tariff element
        /// is/becomes active. If the charging current is or becomes lower, this tariff element is
        /// not or no longer valid and becomes inactive. This describes NOT the minimum current over
        /// the entire charging session. This restriction can make a tariff element become active
        /// when the charging current is above the defined value, but the tariff element MUST no
        /// longer be active when the charging current drops below the defined value.
        /// </summary>
        [Optional]
        public Ampere?                          MinCurrent                 { get; }

        /// <summary>
        /// The sum of the maximum current (in Amperes) over all phases, for example 20. When the EV is
        /// charging with less than the defined amount of current, this tariff element becomes/is
        /// active. If the charging current is or becomes higher, this tariff element is not or no
        /// longer valid and becomes inactive. This describes NOT the maximum current over the
        /// entire charging session. This restriction can make a tariff element become active when
        /// the charging current is below this value, but the tariff element MUST no longer be
        /// active when the charging current raises above the defined value.
        /// </summary>
        [Optional]
        public Ampere?                          MaxCurrent                 { get; }

        /// <summary>
        /// The minimum power in kW, for example 5. When the EV is charging with more than, or equal to,
        /// the defined amount of power, this tariff element is/becomes active. If the charging power
        /// is or becomes lower, this tariff element is not or no longer valid and becomes inactive.
        /// This describes NOT the minimum power over the entire charging session. This restriction
        /// can make a tariff element become active when the charging power is above this value, but
        /// the tariff element MUST no longer be active when the charging power drops below the
        /// defined value.
        /// </summary>
        [Optional]
        public Watt?                            MinPower                   { get; }

        /// <summary>
        /// The maximum power in kW, for example 20. When the EV is charging with less than the defined
        /// amount of power, this tariff element becomes/is active. If the charging power is or
        /// becomes higher, this tariff element is not or no longer valid and becomes inactive. This
        /// describes NOT the maximum power over the entire Charging Session. This restriction can
        /// make a tariff element become active when the charging power is below this value, but the
        /// tariff element MUST no longer be active when the charging power raises above the defined
        /// value.
        /// </summary>
        [Optional]
        public Watt?                            MaxPower                   { get; }

        /// <summary>
        /// The minimum duration in seconds the charging session MUST last (inclusive). When the
        /// duration of a charging session is longer than the defined value, this tariff element is
        /// or becomes active. Before that moment, this tariff element is not yet active.
        /// </summary>
        [Optional]
        public TimeSpan?                        MinDuration                { get; }

        /// <summary>
        /// The maximum duration in seconds the charging session MUST last (exclusive). When the
        /// duration of a Charging Session is shorter than the defined value, this tariff element
        /// is or becomes active. After that moment, this tariff element is no longer active.
        /// </summary>
        [Optional]
        public TimeSpan?                        MaxDuration                { get; }


        /// <summary>
        /// An optional enumeration of charging station operators, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<CSOOperator_Id>      ValidOperators             { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>     ValidChargingPools         { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>  ValidChargingStations      { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>       ValidEVSEs                 { get; }


        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>     InvalidChargingPools       { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>  InvalidChargingStations    { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>       InvalidEVSEs               { get; }


        /// <summary>
        /// When this field is present, the tariff element describes reservation costs.
        /// A reservation starts when the reservation is made, and ends when the driver
        /// starts charging on the reserved EVSE/charging location, or when the reservation
        /// expires. A reservation can only have: FLAT and TIME tariff dimensions, where TIME is
        /// for the duration of the reservation.
        /// </summary>
        [Optional]
        public ReservationRestrictions?         Reservation                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff restrictions.
        /// </summary>
        /// <param name="NotBefore">A start date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="NotAfter">An end date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// 
        /// <param name="DaysOfWeek">All day(s) of the week this tariff element is active.</param>
        /// <param name="StartTimeOfDay">A start time of day, for example "13:30", valid from this time of the day.</param>
        /// <param name="EndTimeOfDay">An end time of day, for example "19:45", valid from this time of the day.</param>
        /// 
        /// <param name="CurrentType">The allowed current type: AC, DC, or both.</param>
        /// <param name="MinEnergy">A minimum consumed energy in kWh, for example 20, valid from this amount of energy (inclusive) being used.</param>
        /// <param name="MaxEnergy">A maximum consumed energy in kWh, for example 50, valid until this amount of energy (exclusive) being used.</param>
        /// <param name="MinCurrent">A sum of the minimum current (in Amperes) over all phases, for example 5.</param>
        /// <param name="MaxCurrent">A sum of the maximum current (in Amperes) over all phases, for example 20.</param>
        /// <param name="MinPower">A minimum power in kW, for example 5.</param>
        /// <param name="MaxPower">A maximum power in kW, for example 20.</param>
        /// <param name="MinDuration">A minimum duration in seconds the charging session MUST last (inclusive).</param>
        /// <param name="MaxDuration">A maximum duration in seconds the charging session MUST last (exclusive).</param>
        /// 
        /// <param name="ValidOperators">An enumeration of charging station operators where this charging ticket can be used.</param>
        /// <param name="ValidChargingPools">An enumeration of charging pools where this charging ticket can be used.</param>
        /// <param name="ValidChargingStations">An enumeration of charging stations where this charging ticket can be used.</param>
        /// <param name="ValidEVSEs">An enumeration of EVSEs where this charging ticket can be used.</param>
        /// 
        /// <param name="InvalidChargingPools">An enumeration of charging pools where this charging ticket can NOT be used.</param>
        /// <param name="InvalidChargingStations">An enumeration of charging stations where this charging ticket can NOT be used.</param>
        /// <param name="InvalidEVSEs">An enumeration of EVSEs where this charging ticket can NOT be used.</param>
        /// 
        /// <param name="Reservation"> When this field is present, the tariff element describes reservation costs.</param>
        public TariffRestrictions(DateTime?                         NotBefore                 = null,
                                  DateTime?                         NotAfter                  = null,

                                  IEnumerable<DayOfWeek>?           DaysOfWeek                = null,
                                  Time?                             StartTimeOfDay            = null,
                                  Time?                             EndTimeOfDay              = null,

                                  CurrentTypes?                     CurrentType               = null,
                                  WattHour?                         MinEnergy                 = null,
                                  WattHour?                         MaxEnergy                 = null,
                                  Ampere?                           MinCurrent                = null,
                                  Ampere?                           MaxCurrent                = null,
                                  Watt?                             MinPower                  = null,
                                  Watt?                             MaxPower                  = null,
                                  TimeSpan?                         MinDuration               = null,
                                  TimeSpan?                         MaxDuration               = null,

                                  IEnumerable<CSOOperator_Id>?      ValidOperators            = null,
                                  IEnumerable<ChargingPool_Id>?     ValidChargingPools        = null,
                                  IEnumerable<ChargingStation_Id>?  ValidChargingStations     = null,
                                  IEnumerable<GlobalEVSE_Id>?       ValidEVSEs                = null,

                                  IEnumerable<ChargingPool_Id>?     InvalidChargingPools      = null,
                                  IEnumerable<ChargingStation_Id>?  InvalidChargingStations   = null,
                                  IEnumerable<GlobalEVSE_Id>?       InvalidEVSEs              = null,

                                  ReservationRestrictions?          Reservation               = null)

        {

            this.NotBefore                = NotBefore;
            this.NotAfter                 = NotAfter;

            this.DaysOfWeek               = DaysOfWeek?.             Distinct() ?? Array.Empty<DayOfWeek>();
            this.StartTimeOfDay           = StartTimeOfDay;
            this.EndTimeOfDay             = EndTimeOfDay;

            this.CurrentType              = CurrentType;
            this.MinEnergy                = MinEnergy;
            this.MaxEnergy                = MaxEnergy;
            this.MinCurrent               = MinCurrent;
            this.MaxCurrent               = MaxCurrent;
            this.MinPower                 = MinPower;
            this.MaxPower                 = MaxPower;
            this.MinDuration              = MinDuration;
            this.MaxDuration              = MaxDuration;

            this.ValidOperators           = ValidOperators?.         Distinct() ?? Array.Empty<CSOOperator_Id>();
            this.ValidChargingPools       = ValidChargingPools?.     Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.ValidChargingStations    = ValidChargingStations?.  Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.ValidEVSEs               = ValidEVSEs?.             Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.InvalidChargingPools     = InvalidChargingPools?.   Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.InvalidChargingStations  = InvalidChargingStations?.Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.InvalidEVSEs             = InvalidEVSEs?.           Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.Reservation              = Reservation;

            unchecked
            {

                hashCode = (this.NotBefore?.             GetHashCode() ?? 0) * 41 ^
                           (this.NotAfter?.              GetHashCode() ?? 0) * 37 ^

                            this.DaysOfWeek.             CalcHashCode()      * 47 ^
                           (this.StartTimeOfDay?.        GetHashCode() ?? 0) * 47 ^
                           (this.EndTimeOfDay?.          GetHashCode() ?? 0) * 43 ^

                           (this.CurrentType?.           GetHashCode() ?? 0) * 31 ^
                           (this.MinEnergy?.             GetHashCode() ?? 0) * 31 ^
                           (this.MaxEnergy?.             GetHashCode() ?? 0) * 29 ^
                           (this.MinCurrent?.            GetHashCode() ?? 0) * 23 ^
                           (this.MaxCurrent?.            GetHashCode() ?? 0) * 19 ^
                           (this.MinPower?.              GetHashCode() ?? 0) * 13^
                           (this.MaxPower?.              GetHashCode() ?? 0) * 11 ^
                           (this.MinDuration?.           GetHashCode() ?? 0) *  7 ^
                           (this.MaxDuration?.           GetHashCode() ?? 0) *  5 ^

                            this.ValidOperators.         CalcHashCode()      * 19 ^
                            this.ValidChargingPools.     CalcHashCode()      * 17 ^
                            this.ValidChargingStations.  CalcHashCode()      * 13 ^
                            this.ValidEVSEs.             CalcHashCode()      * 11 ^

                            this.InvalidChargingPools.   CalcHashCode()      *  7 ^
                            this.InvalidChargingStations.CalcHashCode()      *  5 ^
                            this.InvalidEVSEs.           CalcHashCode()      *  3 ^

                            this.Reservation?.           GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomTariffRestrictionsParser = null)

        /// <summary>
        /// Parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static TariffRestrictions Parse(JObject                                            JSON,
                                               CustomJObjectParserDelegate<TariffRestrictions?>?  CustomTariffRestrictionsParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffRestrictions,
                         out var errorResponse,
                         CustomTariffRestrictionsParser))
            {
                return tariffRestrictions!;
            }

            throw new ArgumentException("The given JSON representation of tariff restrictions is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffRestrictions, out ErrorResponse, CustomTariffRestrictionsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffRestrictions">The parsed tariff restrictions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out TariffRestrictions?  TariffRestrictions,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        out TariffRestrictions,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of tariff restrictions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffRestrictions">The parsed tariff restrictions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffRestrictionsParser">A delegate to parse custom tariff restrictions JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       out TariffRestrictions?                            TariffRestrictions,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<TariffRestrictions?>?  CustomTariffRestrictionsParser   = null)
        {

            try
            {

                TariffRestrictions = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = null;
                    return true;
                }

                #region Parse NotBefore                  [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter                   [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse DaysOfWeek                 [optional]

                // "day_of_week": ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"]

                if (JSON.ParseOptionalEnums("daysOfWeek",
                                            "days of week",
                                            out HashSet<DayOfWeek> DaysOfWeek,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse StartTimeOfDay             [optional]

                if (JSON.ParseOptional("startTimeOfDay",
                                       "start time",
                                       Time.TryParse,
                                       out Time? StartTimeOfDay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EndTimeOfDay               [optional]

                if (JSON.ParseOptional("endTimeOfDay",
                                       "end time",
                                       Time.TryParse,
                                       out Time? EndTimeOfDay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse CurrentType                [optional]

                if (JSON.ParseOptional("currentType",
                                       "current type",
                                       CurrentTypesExtensions.TryParse,
                                       out CurrentTypes? CurrentType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinEnergy                  [optional]

                if (JSON.ParseOptional("minEnergy",
                                       "minimum energy",
                                       out WattHour? MinEnergy,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxEnergy                  [optional]

                if (JSON.ParseOptional("maxEnergy",
                                       "maximum energy",
                                       out WattHour? MaxEnergy,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinCurrent                 [optional]

                if (JSON.ParseOptional("minCurrent",
                                       "minimum current",
                                       out Ampere? MinCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxCurrent                 [optional]

                if (JSON.ParseOptional("maxCurrent",
                                       "maximum current",
                                       out Ampere? MaxCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinPower                   [optional]

                if (JSON.ParseOptional("minPower",
                                       "minimum power",
                                       out Watt? MinPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPower                   [optional]

                if (JSON.ParseOptional("maxPower",
                                       "maximum power",
                                       out Watt? MaxPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinDuration                [optional]

                if (JSON.ParseOptional("minDuration",
                                       "minimum duration",
                                       out Double? minDurationSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MinDuration = minDurationSec.HasValue
                                      ? new TimeSpan?(TimeSpan.FromSeconds(minDurationSec.Value))
                                      : null;

                #endregion

                #region Parse MaxDuration                [optional]

                if (JSON.ParseOptional("maxDuration",
                                       "maximum duration",
                                       out Double? maxDurationSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MaxDuration = maxDurationSec.HasValue
                                      ? new TimeSpan?(TimeSpan.FromSeconds(maxDurationSec.Value))
                                      : null;

                #endregion


                #region Parse ValidOperators             [optional]

                if (JSON.ParseOptionalHashSet("validOperators",
                                              "valid operator identifications",
                                              CSOOperator_Id.TryParse,
                                              out HashSet<CSOOperator_Id> ValidOperators,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidChargingPools         [optional]

                if (JSON.ParseOptionalHashSet("validChargingPools",
                                              "valid charging pool identifications",
                                              ChargingPool_Id.TryParse,
                                              out HashSet<ChargingPool_Id> ValidChargingPools,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidChargingStations      [optional]

                if (JSON.ParseOptionalHashSet("validChargingStations",
                                              "valid charging station identifications",
                                              ChargingStation_Id.TryParse,
                                              out HashSet<ChargingStation_Id> ValidChargingStations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidEVSEs                 [optional]

                if (JSON.ParseOptionalHashSet("validEVSEs",
                                              "valid EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> ValidEVSEs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse InvalidChargingPools       [optional]

                if (JSON.ParseOptionalHashSet("invalidChargingPools",
                                              "invalid charging pool identifications",
                                              ChargingPool_Id.TryParse,
                                              out HashSet<ChargingPool_Id> InvalidChargingPools,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvalidChargingStations    [optional]

                if (JSON.ParseOptionalHashSet("invalidChargingStations",
                                              "invalid charging station identifications",
                                              ChargingStation_Id.TryParse,
                                              out HashSet<ChargingStation_Id> InvalidChargingStations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvalidEVSEs               [optional]

                if (JSON.ParseOptionalHashSet("invalidEVSEs",
                                              "invalid EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> InvalidEVSEs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Reservation                [optional]

                if (JSON.ParseOptional("reservation",
                                       "reservation restriction",
                                       ReservationRestrictionsExtensions.TryParse,
                                       out ReservationRestrictions? Reservation,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TariffRestrictions  = (NotBefore.              HasValue ||
                                       NotAfter.               HasValue ||

                                       DaysOfWeek.             Any()    ||
                                       StartTimeOfDay.         HasValue ||
                                       EndTimeOfDay.           HasValue ||

                                       CurrentType.            HasValue ||
                                       MinEnergy.              HasValue ||
                                       MaxEnergy.              HasValue ||
                                       MinCurrent.             HasValue ||
                                       MaxCurrent.             HasValue ||
                                       MinPower.               HasValue ||
                                       MaxPower.               HasValue ||
                                       MinDuration.            HasValue ||
                                       MaxDuration.            HasValue ||

                                       ValidOperators.         Any()    ||
                                       ValidChargingPools.     Any()    ||
                                       ValidChargingStations.  Any()    ||
                                       ValidEVSEs.             Any()    ||

                                       InvalidChargingPools.   Any()    ||
                                       InvalidChargingStations.Any()    ||
                                       InvalidEVSEs.           Any()    ||

                                       Reservation.            HasValue)

                                           ? new TariffRestrictions(

                                                 NotBefore,
                                                 NotAfter,

                                                 DaysOfWeek,
                                                 StartTimeOfDay,
                                                 EndTimeOfDay,

                                                 CurrentType,
                                                 MinEnergy,
                                                 MaxEnergy,
                                                 MinCurrent,
                                                 MaxCurrent,
                                                 MinPower,
                                                 MaxPower,
                                                 MinDuration,
                                                 MaxDuration,

                                                 ValidOperators,
                                                 ValidChargingPools,
                                                 ValidChargingStations,
                                                 ValidEVSEs,

                                                 InvalidChargingPools,
                                                 InvalidChargingStations,
                                                 InvalidEVSEs,

                                                 Reservation
                                             )

                                           : null;


                if (CustomTariffRestrictionsParser is not null)
                    TariffRestrictions = CustomTariffRestrictionsParser(JSON,
                                                                        TariffRestrictions);

                return true;

            }
            catch (Exception e)
            {
                TariffRestrictions  = default;
                ErrorResponse       = "The given JSON representation of tariff restrictions is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffRestrictionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject? ToJSON(CustomJObjectSerializerDelegate<TariffRestrictions>? CustomTariffRestrictionsSerializer = null)
        {

            var json = JSONObject.Create(


                           NotBefore.  HasValue
                               ? new JProperty("notBefore",                 NotBefore.     Value.ToIso8601())
                               : null,

                           NotAfter.    HasValue
                               ? new JProperty("notAfter",                  NotAfter.      Value.ToIso8601())
                               : null,


                           DaysOfWeek.SafeAny()
                               ? new JProperty("dayOfWeek",                 new JArray(DaysOfWeek.              Select(day               => day.              ToString().ToUpper())))
                               : null,

                           StartTimeOfDay.  HasValue
                               ? new JProperty("startTimeOfDay",            StartTimeOfDay.Value.ToString())
                               : null,

                           EndTimeOfDay.    HasValue
                               ? new JProperty("endTimeofDay",              EndTimeOfDay.  Value.ToString())
                               : null,


                           MinEnergy.     HasValue
                               ? new JProperty("minEnergy",                 MinEnergy.     Value)
                               : null,

                           MaxEnergy.     HasValue
                               ? new JProperty("maxEnergy",                 MaxEnergy.     Value)
                               : null,

                           MinCurrent. HasValue
                               ? new JProperty("minCurrent",                MinCurrent.    Value)
                               : null,

                           MaxCurrent. HasValue
                               ? new JProperty("maxCurrent",                MaxCurrent.    Value)
                               : null,

                           MinPower.   HasValue
                               ? new JProperty("minPower",                  MinPower.      Value)
                               : null,

                           MaxPower.   HasValue
                               ? new JProperty("maxPower",                  MaxPower.      Value)
                               : null,

                           MinDuration.HasValue
                               ? new JProperty("minDuration",               MinDuration.   Value.TotalSeconds)
                               : null,

                           MaxDuration.HasValue
                               ? new JProperty("maxDuration",               MaxDuration.   Value.TotalSeconds)
                               : null,


                           ValidOperators.         Any()
                               ? new JProperty("validOperators",            new JArray(ValidOperators.         Select(operatorId        => operatorId.       ToString())))
                               : null,

                           ValidChargingPools.     Any()
                               ? new JProperty("validChargingPools",        new JArray(ValidChargingPools.     Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           ValidChargingStations.  Any()
                               ? new JProperty("validChargingStations",     new JArray(ValidChargingStations.  Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           ValidEVSEs.           Any()
                               ? new JProperty("validEVSEIds",              new JArray(ValidEVSEs.             Select(evseId            => evseId.           ToString())))
                               : null,


                           InvalidChargingPools.   Any()
                               ? new JProperty("invalidChargingPools",      new JArray(InvalidChargingPools.   Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           InvalidChargingStations.Any()
                               ? new JProperty("invalidChargingStations",   new JArray(InvalidChargingStations.Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           InvalidEVSEs.         Any()
                               ? new JProperty("invalidEVSEIds",            new JArray(InvalidEVSEs.           Select(evseId            => evseId.           ToString())))
                               : null,


                           Reservation.HasValue
                               ? new JProperty("reservation",               Reservation.   Value.AsText())
                               : null

                       );

            var JSON2 = CustomTariffRestrictionsSerializer is not null
                            ? CustomTariffRestrictionsSerializer(this, json)
                            : json;

            return JSON2.HasValues
                       ? JSON2
                       : null;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TariffRestrictions Clone()

            => new (NotBefore,
                    NotAfter,

                    DaysOfWeek.ToArray(),
                    StartTimeOfDay,
                    EndTimeOfDay,

                    CurrentType,
                    MinEnergy,
                    MaxEnergy,
                    MinCurrent,
                    MaxCurrent,
                    MinPower,
                    MaxPower,
                    MinDuration,
                    MaxDuration,

                    ValidOperators,
                    ValidChargingPools,
                    ValidChargingStations,
                    ValidEVSEs,

                    InvalidChargingPools,
                    InvalidChargingStations,
                    InvalidEVSEs,

                    Reservation);

        #endregion


        #region Operator overloading

        #region Operator == (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A charging tariff restriction.</param>
        /// <param name="TariffRestriction2">Another charging tariff restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)
        {

            if (Object.ReferenceEquals(TariffRestriction1, TariffRestriction2))
                return true;

            if (TariffRestriction1 is null || TariffRestriction2 is null)
                return false;

            return TariffRestriction1.Equals(TariffRestriction2);

        }

        #endregion

        #region Operator != (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A charging tariff restriction.</param>
        /// <param name="TariffRestriction2">Another charging tariff restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)

            => !(TariffRestriction1 == TariffRestriction2);

        #endregion

        #endregion

        #region IEquatable<TariffRestriction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariff restrictions for equality.
        /// </summary>
        /// <param name="Object">A charging tariff restriction to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffRestrictions tariffRestrictions &&
                   Equals(tariffRestrictions);

        #endregion

        #region Equals(TariffRestriction)

        /// <summary>
        /// Compares two charging tariff restrictions for equality.
        /// </summary>
        /// <param name="TariffRestrictions">A charging tariff restriction to compare with.</param>
        public Boolean Equals(TariffRestrictions? TariffRestrictions)

            => TariffRestrictions is not null &&

            ((!NotBefore.     HasValue && !TariffRestrictions.NotBefore.     HasValue) ||
             ( NotBefore.     HasValue &&  TariffRestrictions.NotBefore.     HasValue && NotBefore.     Value.Equals(TariffRestrictions.NotBefore.  Value))) &&

            ((!NotAfter.      HasValue && !TariffRestrictions.NotAfter.      HasValue) ||
             ( NotAfter.      HasValue &&  TariffRestrictions.NotAfter.      HasValue && NotAfter.      Value.Equals(TariffRestrictions.NotAfter.    Value))) &&


               DaysOfWeek.Count().Equals(TariffRestrictions.DaysOfWeek.Count())   &&
               DaysOfWeek.All(day => TariffRestrictions.DaysOfWeek.Contains(day)) &&

            ((!StartTimeOfDay.HasValue && !TariffRestrictions.StartTimeOfDay.HasValue) ||
              (StartTimeOfDay.HasValue &&  TariffRestrictions.StartTimeOfDay.HasValue && StartTimeOfDay.Value.Equals(TariffRestrictions.StartTimeOfDay.Value))) &&

            ((!EndTimeOfDay.  HasValue && !TariffRestrictions.EndTimeOfDay.  HasValue) ||
              (EndTimeOfDay.  HasValue &&  TariffRestrictions.EndTimeOfDay.  HasValue && EndTimeOfDay.  Value.Equals(TariffRestrictions.EndTimeOfDay.  Value))) &&


            ((!CurrentType.   HasValue && !TariffRestrictions.CurrentType.   HasValue) ||
              (CurrentType.   HasValue &&  TariffRestrictions.CurrentType.   HasValue && CurrentType.   Value.Equals(TariffRestrictions.CurrentType.   Value))) &&

            ((!MinEnergy.     HasValue && !TariffRestrictions.MinEnergy.     HasValue) ||
              (MinEnergy.     HasValue &&  TariffRestrictions.MinEnergy.     HasValue && MinEnergy.     Value.Equals(TariffRestrictions.MinEnergy.     Value))) &&

            ((!MaxEnergy.     HasValue && !TariffRestrictions.MaxEnergy.     HasValue) ||
              (MaxEnergy.     HasValue &&  TariffRestrictions.MaxEnergy.     HasValue && MaxEnergy.     Value.Equals(TariffRestrictions.MaxEnergy.     Value))) &&

            ((!MinCurrent. HasValue && !TariffRestrictions.MinCurrent. HasValue) ||
              (MinCurrent. HasValue &&  TariffRestrictions.MinCurrent. HasValue && MinCurrent. Value.Equals(TariffRestrictions.MinCurrent. Value))) &&

            ((!MaxCurrent. HasValue && !TariffRestrictions.MaxCurrent. HasValue) ||
              (MaxCurrent. HasValue &&  TariffRestrictions.MaxCurrent. HasValue && MaxCurrent. Value.Equals(TariffRestrictions.MaxCurrent. Value))) &&

            ((!MinPower.   HasValue && !TariffRestrictions.MinPower.   HasValue) ||
              (MinPower.   HasValue &&  TariffRestrictions.MinPower.   HasValue && MinPower.   Value.Equals(TariffRestrictions.MinPower.   Value))) &&

            ((!MaxPower.   HasValue && !TariffRestrictions.MaxPower.   HasValue) ||
              (MaxPower.   HasValue &&  TariffRestrictions.MaxPower.   HasValue && MaxPower.   Value.Equals(TariffRestrictions.MaxPower.   Value))) &&

            ((!MinDuration.HasValue && !TariffRestrictions.MinDuration.HasValue) ||
              (MinDuration.HasValue &&  TariffRestrictions.MinDuration.HasValue && MinDuration.Value.Equals(TariffRestrictions.MinDuration.Value))) &&

            ((!MaxDuration.HasValue && !TariffRestrictions.MaxDuration.HasValue) ||
              (MaxDuration.HasValue &&  TariffRestrictions.MaxDuration.HasValue && MaxDuration.Value.Equals(TariffRestrictions.MaxDuration.Value))) &&


               ValidOperators.         Count().          Equals(TariffRestrictions.ValidOperators.         Count())                     &&
               ValidOperators.         All(operatorId        => TariffRestrictions.ValidOperators.         Contains(operatorId))        &&

               ValidChargingPools.     Count().          Equals(TariffRestrictions.ValidChargingPools.     Count())                     &&
               ValidChargingPools.     All(chargingPoolId    => TariffRestrictions.ValidChargingPools.     Contains(chargingPoolId))    &&

               ValidChargingStations.  Count().          Equals(TariffRestrictions.ValidChargingStations.  Count())                     &&
               ValidChargingStations.  All(chargingStationId => TariffRestrictions.ValidChargingStations.  Contains(chargingStationId)) &&

               ValidEVSEs.             Count().          Equals(TariffRestrictions.ValidEVSEs.             Count())                     &&
               ValidEVSEs.             All(evseId            => TariffRestrictions.ValidEVSEs.             Contains(evseId))            &&


               InvalidChargingPools.   Count().          Equals(TariffRestrictions.InvalidChargingPools.   Count())                     &&
               InvalidChargingPools.   All(chargingPoolId    => TariffRestrictions.InvalidChargingPools.   Contains(chargingPoolId))    &&

               InvalidChargingStations.Count().          Equals(TariffRestrictions.InvalidChargingStations.Count())                     &&
               InvalidChargingStations.All(chargingStationId => TariffRestrictions.InvalidChargingStations.Contains(chargingStationId)) &&

               InvalidEVSEs.           Count().          Equals(TariffRestrictions.InvalidEVSEs.           Count())                     &&
               InvalidEVSEs.           All(evseId            => TariffRestrictions.InvalidEVSEs.           Contains(evseId))            &&


            ((!Reservation.HasValue && !TariffRestrictions.Reservation.HasValue) ||
              (Reservation.HasValue &&  TariffRestrictions.Reservation.HasValue && Reservation.Value.Equals(TariffRestrictions.Reservation.Value)));

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

                   StartTimeOfDay.  HasValue
                       ?            StartTimeOfDay.  Value.ToString()
                       : "",

                   EndTimeOfDay.    HasValue
                       ? " - "    + EndTimeOfDay.    Value.ToString()
                       : "",

                   NotBefore.  HasValue
                       ? " from " + NotBefore.  Value.ToString()
                       : "",

                   NotAfter.    HasValue
                       ? " to "   + NotAfter.    Value.ToString()
                       : "",

                   MinEnergy.     HasValue
                       ? ", >= "  + MinEnergy.     Value.ToString() + " kWh"
                       : "",

                   MaxEnergy.     HasValue
                       ? ", <= "  + MaxEnergy.     Value.ToString() + " kWh"
                       : "",

                   MinCurrent. HasValue
                       ? ", >= "  + MinCurrent. Value.ToString() + " A"
                       : "",

                   MaxCurrent. HasValue
                       ? ", <= "  + MaxCurrent. Value.ToString() + " A"
                       : "",

                   MinPower.   HasValue
                       ? ", >= "  + MinPower.   Value.ToString() + " kW"
                       : "",

                   MaxPower.   HasValue
                       ? ", <= "  + MaxPower.   Value.ToString() + " kW"
                       : "",

                   MinDuration.HasValue
                       ? ", > "   + MinDuration.Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MaxDuration.HasValue
                       ? ", < "   + MaxDuration.Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   Reservation.HasValue
                       ? ", reservation: " + Reservation.Value.AsText()
                       : "",

                   DaysOfWeek.  SafeAny()
                       ? ", "     + DaysOfWeek.AggregateWith("|")
                       : ""

               );

        #endregion

    }

}
