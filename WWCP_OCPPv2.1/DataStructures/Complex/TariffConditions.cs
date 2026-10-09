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

using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Tariff conditions.
    /// </summary>
    public class TariffConditions : ACustomData,
                                    ICBORSerializable<TariffConditions>,
                                    IEquatable<TariffConditions>
    {

        #region Properties

        /// <summary>
        /// The start date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateOnly?               ValidFrom          { get; }

        /// <summary>
        /// The end date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        [Optional]
        public DateOnly?               ValidTo            { get; }


        /// <summary>
        /// The day(s) of the week this tariff element is active.
        /// </summary>
        [Optional]
        public IEnumerable<DayOfWeek>  DaysOfWeek         { get; }

        /// <summary>
        /// The start time of day, for example "13:30", valid from this time of the day.
        /// </summary>
        [Optional]
        public TimeOnly?               StartTimeOfDay     { get; }

        /// <summary>
        /// The end time of day, for example "19:45", valid from this time of the day.
        /// </summary>
        [Optional]
        public TimeOnly?               EndTimeOfDay       { get; }


        /// <summary>
        /// The optional type of EVSE (AC, DC) this tariff applies to.
        /// </summary>
        [Optional]
        public EVSEKinds?              EVSEKind           { get; }

        /// <summary>
        /// The optional payment brand the (ad hoc) fixed price applies to, e.g. for
        /// a surcharge of a brand - the additional identification token of the
        /// type "PaymentBrand".
        /// </summary>
        [Optional]
        public String?                 PaymentBrand       { get; }

        /// <summary>
        /// The optional kind of (ad hoc) payment the fixed price applies to, e.g.
        /// CC or Debit - the additional identification token of the type
        /// "PaymentRecognition".
        /// </summary>
        [Optional]
        public String?                 PaymentRecognition { get; }


        /// <summary>
        /// The minimum consumed energy in kWh, for example 20, valid from this amount of energy
        /// (inclusive) being used.
        /// </summary>
        [Optional]
        public WattHour?               MinEnergy          { get; }

        /// <summary>
        /// The maximum consumed energy in kWh, for example 50, valid until this amount of energy
        /// (exclusive) being used.
        /// </summary>
        [Optional]
        public WattHour?               MaxEnergy          { get; }

        /// <summary>
        /// The sum of the minimum current (in Amperes) over all phases, for example 5. When the EV is
        /// charging with more than, or equal to, the defined amount of current, this tariff element
        /// is/becomes active. If the charging current is or becomes lower, this tariff element is
        /// not or no longer valid and becomes inactive. This describes NOT the minimum current over
        /// the entire charging session. This condition can make a tariff element become active
        /// when the charging current is above the defined value, but the tariff element MUST no
        /// longer be active when the charging current drops below the defined value.
        /// </summary>
        [Optional]
        public Ampere?                 MinCurrent         { get; }

        /// <summary>
        /// The sum of the maximum current (in Amperes) over all phases, for example 20. When the EV is
        /// charging with less than the defined amount of current, this tariff element becomes/is
        /// active. If the charging current is or becomes higher, this tariff element is not or no
        /// longer valid and becomes inactive. This describes NOT the maximum current over the
        /// entire charging session. This condition can make a tariff element become active when
        /// the charging current is below this value, but the tariff element MUST no longer be
        /// active when the charging current raises above the defined value.
        /// </summary>
        [Optional]
        public Ampere?                 MaxCurrent         { get; }

        /// <summary>
        /// The minimum power in kW, for example 5. When the EV is charging with more than, or equal to,
        /// the defined amount of power, this tariff element is/becomes active. If the charging power
        /// is or becomes lower, this tariff element is not or no longer valid and becomes inactive.
        /// This describes NOT the minimum power over the entire charging session. This condition
        /// can make a tariff element become active when the charging power is above this value, but
        /// the tariff element MUST no longer be active when the charging power drops below the
        /// defined value.
        /// </summary>
        [Optional]
        public Watt?                   MinPower           { get; }

        /// <summary>
        /// The maximum power in kW, for example 20. When the EV is charging with less than the defined
        /// amount of power, this tariff element becomes/is active. If the charging power is or
        /// becomes higher, this tariff element is not or no longer valid and becomes inactive. This
        /// describes NOT the maximum power over the entire Charging Session. This condition can
        /// make a tariff element become active when the charging power is below this value, but the
        /// tariff element MUST no longer be active when the charging power raises above the defined
        /// value.
        /// </summary>
        [Optional]
        public Watt?                   MaxPower           { get; }


        /// <summary>
        /// The minimum duration in seconds the charging session (charging & idle) MUST last (inclusive).
        /// When the duration of a charging session is longer than the defined value, this tariff element
        /// is or becomes active. Before that moment, this tariff element is not yet active.
        /// </summary>
        [Optional]
        public TimeSpan?               MinTime            { get; }

        /// <summary>
        /// The maximum duration in seconds the charging session (charging & idle) MUST last (exclusive).
        /// When the duration of a charging session is shorter than the defined value, this tariff element
        /// is or becomes active. After that moment, this tariff element is no longer active.
        /// </summary>
        [Optional]
        public TimeSpan?               MaxTime            { get; }

        /// <summary>
        /// The minimum duration in seconds the charging MUST last (inclusive). When the
        /// duration of a charging session is longer than the defined value, this tariff element is
        /// or becomes active. Before that moment, this tariff element is not yet active.
        /// </summary>
        [Optional]
        public TimeSpan?               MinChargingTime    { get; }

        /// <summary>
        /// The maximum duration in seconds the charging MUST last (exclusive). When the
        /// duration of a charging session is shorter than the defined value, this tariff element
        /// is or becomes active. After that moment, this tariff element is no longer active.
        /// </summary>
        [Optional]
        public TimeSpan?               MaxChargingTime    { get; }

        /// <summary>
        /// The minimum duration in seconds the charging session (i.e. not charging) MUST last (inclusive).
        /// When the duration of a charging session is longer than the defined value, this tariff element
        /// is or becomes active. Before that moment, this tariff element is not yet active.
        /// </summary>
        [Optional]
        public TimeSpan?               MinIdleTime        { get; }

        /// <summary>
        /// The maximum duration in seconds the charging session (i.e. not charging) MUST last (exclusive).
        /// When the duration of a Charging Session is shorter than the defined value, this tariff element
        /// is or becomes active. After that moment, this tariff element is no longer active.
        /// </summary>
        [Optional]
        public TimeSpan?               MaxIdleTime        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new tariff conditions.
        /// </summary>
        /// <param name="ValidFrom">A start date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="ValidTo">An end date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// 
        /// <param name="DaysOfWeek">All day(s) of the week this tariff element is active.</param>
        /// <param name="StartTimeOfDay">A start time of day, for example "13:30", valid from this time of the day.</param>
        /// <param name="EndTimeOfDay">An end time of day, for example "19:45", valid from this time of the day.</param>
        /// 
        /// <param name="EVSEKind">The allowed current type: AC, DC, or both.</param>
        /// 
        /// <param name="MinEnergy">A minimum consumed energy in kWh, for example 20, valid from this amount of energy (inclusive) being used.</param>
        /// <param name="MaxEnergy">A maximum consumed energy in kWh, for example 50, valid until this amount of energy (exclusive) being used.</param>
        /// <param name="MinCurrent">A sum of the minimum current (in Amperes) over all phases, for example 5.</param>
        /// <param name="MaxCurrent">A sum of the maximum current (in Amperes) over all phases, for example 20.</param>
        /// <param name="MinPower">A minimum power in kW, for example 5.</param>
        /// <param name="MaxPower">A maximum power in kW, for example 20.</param>
        /// 
        /// <param name="MinTime">A minimum duration in seconds the charging session (charging & idle) MUST last (inclusive).</param>
        /// <param name="MaxTime">A maximum duration in seconds the charging session (charging & idle) MUST last (exclusive).</param>
        /// <param name="MinChargingTime">A minimum duration in seconds the charging MUST last (inclusive).</param>
        /// <param name="MaxChargingTime">A maximum duration in seconds the charging MUST last (exclusive).</param>
        /// <param name="MinIdleTime">A minimum duration in seconds the idle period (i.e. not charging) MUST last (inclusive).</param>
        /// <param name="MaxIdleTime">A maximum duration in seconds the idle period (i.e. not charging) MUST last (exclusive).</param>
        /// <param name="PaymentBrand">The optional payment brand a fixed price applies to.</param>
        /// <param name="PaymentRecognition">The optional kind of payment a fixed price applies to.</param>
        public TariffConditions(DateOnly?                ValidFrom         = null,  // startDate, local time
                                DateOnly?                ValidTo           = null,  // endDate,   local time

                                IEnumerable<DayOfWeek>?  DaysOfWeek        = null,
                                TimeOnly?                StartTimeOfDay    = null,  // startTime, local time
                                TimeOnly?                EndTimeOfDay      = null,  // endTime,   local time

                                EVSEKinds?               EVSEKind          = null,

                                WattHour?                MinEnergy         = null,  // Inclusive
                                WattHour?                MaxEnergy         = null,  // Exclusive
                                Ampere?                  MinCurrent        = null,
                                Ampere?                  MaxCurrent        = null,
                                Watt?                    MinPower          = null,
                                Watt?                    MaxPower          = null,

                                TimeSpan?                MinTime           = null,
                                TimeSpan?                MaxTime           = null,
                                TimeSpan?                MinChargingTime   = null,
                                TimeSpan?                MaxChargingTime   = null,
                                TimeSpan?                MinIdleTime       = null,
                                TimeSpan?                MaxIdleTime       = null,

                                String?                  PaymentBrand         = null,
                                String?                  PaymentRecognition   = null,

                                CustomData?              CustomData        = null)

            : base(CustomData)

        {

            this.ValidFrom           = ValidFrom;
            this.ValidTo             = ValidTo;

            this.DaysOfWeek          = DaysOfWeek?.Distinct() ?? [];
            this.StartTimeOfDay      = StartTimeOfDay;
            this.EndTimeOfDay        = EndTimeOfDay;

            this.EVSEKind            = EVSEKind;
            this.MinEnergy           = MinEnergy;
            this.MaxEnergy           = MaxEnergy;
            this.MinCurrent          = MinCurrent;
            this.MaxCurrent          = MaxCurrent;
            this.MinPower            = MinPower;
            this.MaxPower            = MaxPower;

            this.MinTime             = MinTime;
            this.MaxTime             = MaxTime;
            this.MinChargingTime     = MinChargingTime;
            this.MaxChargingTime     = MaxChargingTime;
            this.MinIdleTime         = MinIdleTime;
            this.MaxIdleTime         = MaxIdleTime;

            this.PaymentBrand        = PaymentBrand;
            this.PaymentRecognition  = PaymentRecognition;

            unchecked
            {

                hashCode = (this.PaymentBrand?.      GetHashCode() ?? 0) * 71 ^
                           (this.PaymentRecognition?.GetHashCode() ?? 0) * 67 ^

                           (this.ValidFrom?.         GetHashCode() ?? 0) * 61 ^
                           (this.ValidTo?.           GetHashCode() ?? 0) * 59 ^

                            this.DaysOfWeek.         CalcHashCode()      * 53 ^
                           (this.StartTimeOfDay?.    GetHashCode() ?? 0) * 47 ^
                           (this.EndTimeOfDay?.      GetHashCode() ?? 0) * 43 ^

                           (this.EVSEKind?.          GetHashCode() ?? 0) * 41 ^

                           (this.MinEnergy?.         GetHashCode() ?? 0) * 37 ^
                           (this.MaxEnergy?.         GetHashCode() ?? 0) * 31 ^
                           (this.MinCurrent?.        GetHashCode() ?? 0) * 29 ^
                           (this.MaxCurrent?.        GetHashCode() ?? 0) * 23 ^
                           (this.MinPower?.          GetHashCode() ?? 0) * 19 ^
                           (this.MaxPower?.          GetHashCode() ?? 0) * 17 ^

                           (this.MinTime?.           GetHashCode() ?? 0) * 13 ^
                           (this.MaxTime?.           GetHashCode() ?? 0) * 11 ^
                           (this.MinChargingTime?.   GetHashCode() ?? 0) *  7 ^
                           (this.MaxChargingTime?.   GetHashCode() ?? 0) *  5 ^
                           (this.MinIdleTime?.       GetHashCode() ?? 0) *  3 ^
                           (this.MaxIdleTime?.       GetHashCode() ?? 0)      ^
                            base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "These conditions describe if and when a TariffEnergyType or TariffTimeType applies during a transaction.\r\n\r\nWhen more than one restriction is set, they are to be treated as a logical AND. All need to be valid before this price is active.\r\n\r\nFor reverse energy flow (discharging) negative values of energy, power and current are used.\r\n\r\nNOTE: _minXXX_ (where XXX = Kwh/A/Kw) must be read as \"closest to zero\", and _maxXXX_ as \"furthest from zero\". For example, a *charging* power range from 10 kW to 50 kWh is given by _minPower_ = 10000 and _maxPower_ = 50000, and a *discharging* power range from -10 kW to -50 kW is given by _minPower_ = -10 and _maxPower_ = -50.\r\n\r\nNOTE: _startTimeOfDay_ and _endTimeOfDay_ are in local time, because it is the time in the tariff as it is shown to the EV driver at the Charging Station.\r\nA Charging Station will convert this to the internal time zone that it uses (which is recommended to be UTC, see section Generic chapter 3.1) when performing cost calculation.",
        //     "javaType": "TariffConditions",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "startTimeOfDay": {
        //             "description": "Start time of day in local time. +\r\nFormat as per RFC 3339: time-hour \":\" time-minute.
        //                             Must be in 24h format with leading zeros.
        //                             Hour/Minute separator: \":\"
        //                             Regex: ([0-1][0-9]\\|2[0-3]):[0-5][0-9]",
        //             "type": "string"
        //         },
        //         "endTimeOfDay": {
        //             "description": "End time of day in local time. Same syntax as _startTimeOfDay_.
        //                             If end time &lt; start time then the period wraps around to the next day.#
        //                             To stop at end of the day use: 00:00.",
        //             "type": "string"
        //         },
        //         "dayOfWeek": {
        //             "description": "Day(s) of the week this is tariff applies.",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/DayOfWeekEnumType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 7
        //         },
        //         "validFromDate": {
        //             "description": "Start date in local time, for example: 2015-12-24.
        //                             Valid from this day (inclusive).
        //                             Format as per RFC 3339: full-date
        //                             Regex: ([12][0-9]{3})-(0[1-9]\\|1[0-2])-(0[1-9]\\|[12][0-9]\\|3[01])",
        //             "type": "string"
        //         },
        //         "validToDate": {
        //             "description": "End date in local time, for example: 2015-12-27.
        //                             Valid until this day (exclusive).
        //                             Same syntax as _validFromDate_.",
        //             "type": "string"
        //         },
        //         "evseKind": {
        //             "$ref": "#/definitions/EvseKindEnumType"
        //         },
        //         "minEnergy": {
        //             "description": "Minimum consumed energy in Wh, for example 20000 Wh.
        //                             Valid from this amount of energy (inclusive) being used.",
        //             "type": "number"
        //         },
        //         "maxEnergy": {
        //             "description": "Maximum consumed energy in Wh, for example 50000 Wh.
        //                             Valid until this amount of energy (exclusive) being used.",
        //             "type": "number"
        //         },
        //         "minCurrent": {
        //             "description": "Sum of the minimum current (in Amperes) over all phases, for example 5 A.
        //                             When the EV is charging with more than, or equal to, the defined amount of current, this price is/becomes active.
        //                             If the charging current is or becomes lower, this price is not or no longer valid and becomes inactive.
        //                             This is NOT about the minimum current over the entire transaction.",
        //             "type": "number"
        //         },
        //         "maxCurrent": {
        //             "description": "Sum of the maximum current (in Amperes) over all phases, for example 20 A.
        //                             When the EV is charging with less than the defined amount of current, this price becomes/is active.
        //                             If the charging current is or becomes higher, this price is not or no longer valid and becomes inactive.
        //                             This is NOT about the maximum current over the entire transaction.",
        //             "type": "number"
        //         },
        //         "minPower": {
        //             "description": "Minimum power in W, for example 5000 W.
        //                             When the EV is charging with more than, or equal to, the defined amount of power, this price is/becomes active.
        //                             If the charging power is or becomes lower, this price is not or no longer valid and becomes inactive.
        //                             This is NOT about the minimum power over the entire transaction.",
        //             "type": "number"
        //         },
        //         "maxPower": {
        //             "description": "Maximum power in W, for example 20000 W.
        //                             When the EV is charging with less than the defined amount of power, this price becomes/is active.
        //                             If the charging power is or becomes higher, this price is not or no longer valid and becomes inactive.
        //                             This is NOT about the maximum power over the entire transaction.",
        //             "type": "number"
        //         },
        //         "minTime": {
        //             "description": "Minimum duration in seconds the transaction (charging &amp; idle) MUST last (inclusive).
        //                             When the duration of a transaction is longer than the defined value, this price is or becomes active.
        //                             Before that moment, this price is not yet active.",
        //             "type": "integer"
        //         },
        //         "maxTime": {
        //             "description": "Maximum duration in seconds the transaction (charging &amp; idle) MUST last (exclusive).
        //                             When the duration of a transaction is shorter than the defined value, this price is or becomes active.
        //                             After that moment, this price is no longer active.",
        //             "type": "integer"
        //         },
        //         "minChargingTime": {
        //             "description": "Minimum duration in seconds the charging MUST last (inclusive).
        //                             When the duration of a charging is longer than the defined value, this price is or becomes active.
        //                             Before that moment, this price is not yet active.",
        //             "type": "integer"
        //         },
        //         "maxChargingTime": {
        //             "description": "Maximum duration in seconds the charging MUST last (exclusive).
        //                             When the duration of a charging is shorter than the defined value, this price is or becomes active.
        //                             After that moment, this price is no longer active.",
        //             "type": "integer"
        //         },
        //         "minIdleTime": {
        //             "description": "Minimum duration in seconds the idle period (i.e. not charging) MUST last (inclusive).
        //                             When the duration of the idle time is longer than the defined value, this price is or becomes active.
        //                             Before that moment, this price is not yet active.",
        //             "type": "integer"
        //         },
        //         "maxIdleTime": {
        //             "description": "Maximum duration in seconds the idle period (i.e. not charging) MUST last (exclusive).
        //                             When the duration of idle time is shorter than the defined value, this price is or becomes active.
        //                             After that moment, this price is no longer active.",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffConditionsParser = null)

        /// <summary>
        /// Parse the given JSON representation of TariffConditions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static TariffConditions? Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<TariffConditions?>?  CustomTariffConditionsParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffConditions,
                         out var errorResponse,
                         CustomTariffConditionsParser))
            {
                return tariffConditions;
            }

            throw new ArgumentException("The given JSON representation of TariffConditions is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffConditions, out ErrorResponse, CustomTariffConditionsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of TariffConditions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffConditions">The parsed TariffConditions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                                             out TariffConditions?  TariffConditions,
                                       [NotNullWhen(false)]  out String?            ErrorResponse)

            => TryParse(JSON,
                        out TariffConditions,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of TariffConditions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffConditions">The parsed TariffConditions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                                             out TariffConditions?      TariffConditions,
                                       [NotNullWhen(false)]  out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<TariffConditions?>?  CustomTariffConditionsParser   = null)
        {

            try
            {

                TariffConditions = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = null;
                    return true;
                }

                #region Parse ValidFrom          [optional]

                // RFC 3339 full-date, whatever culture this machine is in.
                if (JSON.ParseOptional("validFromDate",
                                       "not before",
                                       (s) => DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                       out DateOnly? ValidFrom,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidTo            [optional]

                if (JSON.ParseOptional("validToDate",
                                       "not after",
                                       (s) => DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                       out DateOnly? ValidTo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse DaysOfWeek         [optional]

                // "dayOfWeek": [ "Monday", "Tuesday", ... ] - it was read as "daysOfWeek".

                if (JSON.ParseOptionalEnums("dayOfWeek",
                                            "days of week",
                                            out HashSet<DayOfWeek> DaysOfWeek,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse StartTimeOfDay     [optional]

                // "startTimeOfDay" and "endTimeOfDay", as RFC 3339 hour and
                // minute - they were read as "startTime" and "endTime", in
                // whatever format this machine's culture takes.
                if (JSON.ParseOptional("startTimeOfDay",
                                       "start time of day",
                                       (s) => TimeOfDay(s),
                                       out TimeOnly? StartTimeOfDay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EndTimeOfDay       [optional]

                if (JSON.ParseOptional("endTimeOfDay",
                                       "end time of day",
                                       (s) => TimeOfDay(s),
                                       out TimeOnly? EndTimeOfDay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse EVSEKind           [optional]

                if (JSON.ParseOptional("evseKind",
                                       "EVSE kind",
                                       EVSEKindsExtensions.TryParse,
                                       out EVSEKinds? EVSEKind,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PaymentBrand       [optional]

                var PaymentBrand        = JSON["paymentBrand"]?.      Value<String>();

                #endregion

                #region Parse PaymentRecognition [optional]

                var PaymentRecognition  = JSON["paymentRecognition"]?.Value<String>();

                #endregion


                #region Parse MinEnergy          [optional]

                if (JSON.ParseOptional("minEnergy",
                                       "minimum energy",
                                       out WattHour? MinEnergy,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxEnergy          [optional]

                if (JSON.ParseOptional("maxEnergy",
                                       "maximum energy",
                                       out WattHour? MaxEnergy,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinCurrent         [optional]

                if (JSON.ParseOptional("minCurrent",
                                       "minimum current",
                                       out Ampere? MinCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxCurrent         [optional]

                if (JSON.ParseOptional("maxCurrent",
                                       "maximum current",
                                       out Ampere? MaxCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinPower           [optional]

                if (JSON.ParseOptional("minPower",
                                       "minimum power",
                                       out Watt? MinPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPower           [optional]

                if (JSON.ParseOptional("maxPower",
                                       "maximum power",
                                       out Watt? MaxPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse MinTime            [optional]

                if (JSON.ParseOptional("minTime",
                                       "minimum hours",
                                       out Double? minTimeSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MinTime = minTimeSec.HasValue
                                   ? new TimeSpan?(TimeSpan.FromSeconds(minTimeSec.Value))
                                   : null;

                #endregion

                #region Parse MaxTime            [optional]

                if (JSON.ParseOptional("maxTime",
                                       "maximum hours",
                                       out Double? maxTimeSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MaxTime = maxTimeSec.HasValue
                                   ? new TimeSpan?(TimeSpan.FromSeconds(maxTimeSec.Value))
                                   : null;

                #endregion

                #region Parse MinChargingTime    [optional]

                if (JSON.ParseOptional("minChargingTime",
                                       "minimum charge hours",
                                       out Double? minChargingTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MinChargingTime = minChargingTime.HasValue
                                         ? new TimeSpan?(TimeSpan.FromSeconds(minChargingTime.Value))
                                         : null;

                #endregion

                #region Parse MaxChargingTime    [optional]

                if (JSON.ParseOptional("maxChargingTime",
                                       "maximum duration",
                                       out Double? maxChargingTimeSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MaxChargingTime = maxChargingTimeSec.HasValue
                                         ? new TimeSpan?(TimeSpan.FromSeconds(maxChargingTimeSec.Value))
                                         : null;

                #endregion

                #region Parse MinIdleTime        [optional]

                if (JSON.ParseOptional("minIdleTime",
                                       "minimum idle hours",
                                       out Double? minIdleTimeSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MinIdleTime = minIdleTimeSec.HasValue
                                       ? new TimeSpan?(TimeSpan.FromSeconds(minIdleTimeSec.Value))
                                       : null;

                #endregion

                #region Parse MaxIdleTime        [optional]

                if (JSON.ParseOptional("maxIdleTime",
                                       "maximum idle hours",
                                       out Double? maxIdleTimeSec,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var MaxIdleTime = maxIdleTimeSec.HasValue
                                      ? new TimeSpan?(TimeSpan.FromSeconds(maxIdleTimeSec.Value))
                                      : null;

                #endregion

                #region Parse CustomData         [optional]

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


                TariffConditions  = (ValidFrom.         HasValue  ||
                                     ValidTo.           HasValue  ||

                                     DaysOfWeek.        Count > 0 ||
                                     StartTimeOfDay.    HasValue  ||
                                     EndTimeOfDay.      HasValue  ||

                                     EVSEKind.          HasValue  ||
                                     PaymentBrand       is not null ||
                                     PaymentRecognition is not null ||

                                     MinEnergy.         HasValue  ||
                                     MaxEnergy.         HasValue  ||
                                     MinCurrent.        HasValue  ||
                                     MaxCurrent.        HasValue  ||
                                     MinPower.          HasValue  ||
                                     MaxPower.          HasValue  ||

                                     MinTime.           HasValue  ||
                                     MaxTime.           HasValue  ||
                                     MinChargingTime.   HasValue  ||
                                     MaxChargingTime.   HasValue  ||
                                     MinIdleTime.       HasValue  ||
                                     MaxIdleTime.       HasValue  ||
                                     CustomData         is not null)

                                         ? new TariffConditions(

                                               ValidFrom,
                                               ValidTo,

                                               DaysOfWeek,
                                               StartTimeOfDay,
                                               EndTimeOfDay,

                                               EVSEKind,

                                               MinEnergy,
                                               MaxEnergy,
                                               MinCurrent,
                                               MaxCurrent,
                                               MinPower,
                                               MaxPower,

                                               MinTime,
                                               MaxTime,
                                               MinChargingTime,
                                               MaxChargingTime,
                                               MinIdleTime,
                                               MaxIdleTime,
                                               PaymentBrand,
                                               PaymentRecognition,
                                               CustomData
                                           )

                                         : null;


                if (CustomTariffConditionsParser is not null)
                    TariffConditions = CustomTariffConditionsParser(JSON,
                                                                    TariffConditions);

                return true;

            }
            catch (Exception e)
            {
                TariffConditions  = default;
                ErrorResponse     = "The given JSON representation of TariffConditions is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        public JObject? ToJSON(CustomJObjectSerializerDelegate<TariffConditions>? CustomTariffConditionsSerializer = null)
        {

            var json = JSONObject.Create(


                           ValidFrom.         HasValue
                               ? new JProperty("validFromDate",     ValidFrom.      Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                               : null,

                           ValidTo.           HasValue
                               ? new JProperty("validToDate",       ValidTo.        Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                               : null,


                           // "Monday" as the schema's enumeration has it, not "monday".
                           DaysOfWeek.Any()
                               ? new JProperty("dayOfWeek",         new JArray(DaysOfWeek.Select(day => day.ToString())))
                               : null,

                           StartTimeOfDay.    HasValue
                               ? new JProperty("startTimeOfDay",    StartTimeOfDay. Value.ToString("HH:mm", CultureInfo.InvariantCulture))
                               : null,

                           EndTimeOfDay.      HasValue
                               ? new JProperty("endTimeOfDay",      EndTimeOfDay.   Value.ToString("HH:mm", CultureInfo.InvariantCulture))
                               : null,


                           // As text - the enum itself was written as its number.
                           EVSEKind.          HasValue
                               ? new JProperty("evseKind",          EVSEKind.       Value.AsText())
                               : null,

                           PaymentBrand       is not null
                               ? new JProperty("paymentBrand",       PaymentBrand)
                               : null,

                           PaymentRecognition is not null
                               ? new JProperty("paymentRecognition", PaymentRecognition)
                               : null,


                           MinEnergy.         HasValue
                               ? new JProperty("minEnergy",         MinEnergy.      Value.Value)
                               : null,

                           MaxEnergy.         HasValue
                               ? new JProperty("maxEnergy",         MaxEnergy.      Value.Value)
                               : null,

                           MinCurrent.        HasValue
                               ? new JProperty("minCurrent",        MinCurrent.     Value.Value)
                               : null,

                           MaxCurrent.        HasValue
                               ? new JProperty("maxCurrent",        MaxCurrent.     Value.Value)
                               : null,

                           MinPower.          HasValue
                               ? new JProperty("minPower",          MinPower.       Value.Value)
                               : null,

                           MaxPower.          HasValue
                               ? new JProperty("maxPower",          MaxPower.       Value.Value)
                               : null,


                           // Whole seconds, as the schema's integers.
                           MinTime.           HasValue
                               ? new JProperty("minTime",           (Int64) Math.Round(MinTime.        Value.TotalSeconds))
                               : null,

                           MaxTime.           HasValue
                               ? new JProperty("maxTime",           (Int64) Math.Round(MaxTime.        Value.TotalSeconds))
                               : null,

                           MinChargingTime.   HasValue
                               ? new JProperty("minChargingTime",   (Int64) Math.Round(MinChargingTime.Value.TotalSeconds))
                               : null,

                           MaxChargingTime.   HasValue
                               ? new JProperty("maxChargingTime",   (Int64) Math.Round(MaxChargingTime.Value.TotalSeconds))
                               : null,

                           MinIdleTime.       HasValue
                               ? new JProperty("minIdleTime",       (Int64) Math.Round(MinIdleTime.    Value.TotalSeconds))
                               : null,

                           MaxIdleTime.       HasValue
                               ? new JProperty("maxIdleTime",       (Int64) Math.Round(MaxIdleTime.    Value.TotalSeconds))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON())
                               : null

                       );

            var json2 = CustomTariffConditionsSerializer is not null
                            ? CustomTariffConditionsSerializer(this, json)
                            : json;

            return json2.HasValues
                       ? json2
                       : null;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out TariffConditions, out ErrorResponse, CustomTariffConditionsParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a tariff condition.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TariffConditions">The tariff condition.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out TariffConditions?  TariffConditions,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out TariffConditions,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a tariff condition.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TariffConditions">The tariff condition.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to read custom tariff conditions.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out TariffConditions?           TariffConditions,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<TariffConditions>?  CustomTariffConditionsParser)
        {

            try
            {

                TariffConditions = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a tariff condition is not a map!";
                    return false;
                }

                DateOnly? ValidFrom = null;

                if (CBOR.ParseOptionalText("validFromDate", "valid from date", out var validFromText, out ErrorResponse))
                {
                    if (!DateOnly.TryParseExact(validFromText, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var validFrom))
                    {
                        ErrorResponse = $"Invalid valid from date '{validFromText}'!";
                        return false;
                    }
                    ValidFrom = validFrom;
                }
                else if (ErrorResponse is not null)
                    return false;

                DateOnly? ValidTo = null;

                if (CBOR.ParseOptionalText("validToDate", "valid to date", out var validToText, out ErrorResponse))
                {
                    if (!DateOnly.TryParseExact(validToText, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var validTo))
                    {
                        ErrorResponse = $"Invalid valid to date '{validToText}'!";
                        return false;
                    }
                    ValidTo = validTo;
                }
                else if (ErrorResponse is not null)
                    return false;

                var DaysOfWeek = new List<DayOfWeek>();

                CBOR.ParseOptionalList<String>("dayOfWeek",
                                               "days of the week",
                                               (CBORValue item, out String? text, out String? errorResponse) => {
                                                   text          = item.Kind == CBORValueKind.TextString ? item.AsText() : null;
                                                   errorResponse = text is null ? "A day of the week is a text!" : null;
                                                   return text is not null;
                                               },
                                               out var dayTexts,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                foreach (var dayText in dayTexts)
                {
                    if (!Enum.TryParse<DayOfWeek>(dayText, ignoreCase: true, out var day) || !Enum.IsDefined(day))
                    {
                        ErrorResponse = $"Invalid day of the week '{dayText}'!";
                        return false;
                    }
                    DaysOfWeek.Add(day);
                }

                TimeOnly? StartTimeOfDay = null;

                if (CBOR.ParseOptionalText("startTimeOfDay", "start time of day", out var startTimeText, out ErrorResponse))
                {
                    if (!TimeOnly.TryParseExact(startTimeText, [ "HH:mm", "HH:mm:ss" ], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var startTime))
                    {
                        ErrorResponse = $"Invalid start time of day '{startTimeText}'!";
                        return false;
                    }
                    StartTimeOfDay = startTime;
                }
                else if (ErrorResponse is not null)
                    return false;

                TimeOnly? EndTimeOfDay = null;

                if (CBOR.ParseOptionalText("endTimeOfDay", "end time of day", out var endTimeText, out ErrorResponse))
                {
                    if (!TimeOnly.TryParseExact(endTimeText, [ "HH:mm", "HH:mm:ss" ], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var endTime))
                    {
                        ErrorResponse = $"Invalid end time of day '{endTimeText}'!";
                        return false;
                    }
                    EndTimeOfDay = endTime;
                }
                else if (ErrorResponse is not null)
                    return false;

                EVSEKinds? EVSEKind = null;

                if (CBOR.ParseOptionalText("evseKind",
                                           "EVSE kind",
                                           out var EVSEKindText,
                                           out ErrorResponse))
                {

                    if (!EVSEKindsExtensions.TryParse(EVSEKindText!, out var EVSEKindValue))
                    {
                        ErrorResponse = $"Invalid EVSE kind '{EVSEKindText}'!";
                        return false;
                    }

                    EVSEKind = EVSEKindValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("paymentBrand",
                                       "payment brand",
                                       out var PaymentBrand,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("paymentRecognition",
                                       "payment recognition",
                                       out var PaymentRecognition,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minEnergy",
                                        "minimum energy",
                                        OCPPCBORExtensions.TryParseWattHour,
                                        out WattHour? MinEnergy,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxEnergy",
                                        "maximum energy",
                                        OCPPCBORExtensions.TryParseWattHour,
                                        out WattHour? MaxEnergy,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minCurrent",
                                        "minimum current",
                                        OCPPCBORExtensions.TryParseAmpere,
                                        out Ampere? MinCurrent,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxCurrent",
                                        "maximum current",
                                        OCPPCBORExtensions.TryParseAmpere,
                                        out Ampere? MaxCurrent,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minPower",
                                        "minimum power",
                                        OCPPCBORExtensions.TryParseWatt,
                                        out Watt? MinPower,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxPower",
                                        "maximum power",
                                        OCPPCBORExtensions.TryParseWatt,
                                        out Watt? MaxPower,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minTime",
                                        "minimum time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MinTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxTime",
                                        "maximum time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MaxTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minChargingTime",
                                        "minimum charging time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MinChargingTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxChargingTime",
                                        "maximum charging time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MaxChargingTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("minIdleTime",
                                        "minimum idle time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MinIdleTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("maxIdleTime",
                                        "maximum idle time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? MaxIdleTime,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                TariffConditions = new TariffConditions(
                                       ValidFrom,
                                       ValidTo,
                                       DaysOfWeek,
                                       StartTimeOfDay,
                                       EndTimeOfDay,
                                       EVSEKind,
                                       MinEnergy,
                                       MaxEnergy,
                                       MinCurrent,
                                       MaxCurrent,
                                       MinPower,
                                       MaxPower,
                                       MinTime,
                                       MaxTime,
                                       MinChargingTime,
                                       MaxChargingTime,
                                       MinIdleTime,
                                       MaxIdleTime,
                                       PaymentBrand,
                                       PaymentRecognition,
                                       CustomData
                                   );

                if (CustomTariffConditionsParser is not null)
                    TariffConditions = CustomTariffConditionsParser(CBOR,
                                                  TariffConditions);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TariffConditions  = default;
                ErrorResponse  = "The given CBOR representation of a tariff condition is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<TariffConditions>.TryParse(CBOR, out TariffConditions, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a tariff condition - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<TariffConditions>.TryParse(CBORValue                         CBOR,
                                                                  out TariffConditions                  Value,
                                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this tariff condition: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom tariff conditions.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TariffConditions>? CustomTariffConditionsSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("validFromDate",           OCPPCBORExtensions.Text(ValidFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))),
                           ("validToDate",             OCPPCBORExtensions.Text(ValidTo?.  ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))),
                           ("dayOfWeek",               OCPPCBORExtensions.Array(DaysOfWeek, day => CBORValue.FromText(day.ToString()))),
                           ("startTimeOfDay",          OCPPCBORExtensions.Text(StartTimeOfDay?.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture))),
                           ("endTimeOfDay",            OCPPCBORExtensions.Text(EndTimeOfDay?.  ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture))),
                           ("evseKind",                OCPPCBORExtensions.Text(EVSEKind?.AsText())),
                           ("paymentBrand",            OCPPCBORExtensions.Text(PaymentBrand)),
                           ("paymentRecognition",      OCPPCBORExtensions.Text(PaymentRecognition)),
                           ("minEnergy",               MinEnergy?.      ToCBOR()),
                           ("maxEnergy",               MaxEnergy?.      ToCBOR()),
                           ("minCurrent",              MinCurrent?.     ToCBOR()),
                           ("maxCurrent",              MaxCurrent?.     ToCBOR()),
                           ("minPower",                MinPower?.       ToCBOR()),
                           ("maxPower",                MaxPower?.       ToCBOR()),
                           ("minTime",                 MinTime?.        ToCBOR()),
                           ("maxTime",                 MaxTime?.        ToCBOR()),
                           ("minChargingTime",         MinChargingTime?.ToCBOR()),
                           ("maxChargingTime",         MaxChargingTime?.ToCBOR()),
                           ("minIdleTime",             MinIdleTime?.    ToCBOR()),
                           ("maxIdleTime",             MaxIdleTime?.    ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomTariffConditionsSerializer is not null
                       ? CustomTariffConditionsSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone these TariffConditions.
        /// </summary>
        public TariffConditions Clone()

            => new (

                   ValidFrom,
                   ValidTo,

                   [.. DaysOfWeek],
                   StartTimeOfDay,
                   EndTimeOfDay,

                   EVSEKind,
                   MinEnergy,
                   MaxEnergy,
                   MinCurrent,
                   MaxCurrent,
                   MinPower,
                   MaxPower,

                   MinTime,
                   MaxTime,
                   MinChargingTime,
                   MaxChargingTime,
                   MinIdleTime,
                   MaxIdleTime,

                   PaymentBrand,
                   PaymentRecognition,

                   CustomData

               );

        #endregion

        #region (private static) TimeOfDay(Text)

        /// <summary>
        /// A time of day as RFC 3339 writes one: "HH:mm", 24 hours, leading
        /// zeros - and, read as it was written here before, "HH:mm:ss".
        /// </summary>
        /// <param name="Text">The text of a time of day.</param>
        private static TimeOnly TimeOfDay(String Text)

            => TimeOnly.ParseExact(Text.Trim(),
                                   [ "HH:mm", "HH:mm:ss" ],
                                   CultureInfo.InvariantCulture);

        #endregion


        #region Operator overloading

        #region Operator == (TariffCondition1, TariffCondition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffCondition1">A charging tariff condition.</param>
        /// <param name="TariffCondition2">Another charging tariff condition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffConditions TariffCondition1,
                                           TariffConditions TariffCondition2)
        {

            if (Object.ReferenceEquals(TariffCondition1, TariffCondition2))
                return true;

            if (TariffCondition1 is null || TariffCondition2 is null)
                return false;

            return TariffCondition1.Equals(TariffCondition2);

        }

        #endregion

        #region Operator != (TariffCondition1, TariffCondition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffCondition1">A charging tariff condition.</param>
        /// <param name="TariffCondition2">Another charging tariff condition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffConditions TariffCondition1,
                                           TariffConditions TariffCondition2)

            => !(TariffCondition1 == TariffCondition2);

        #endregion

        #endregion

        #region IEquatable<TariffCondition> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffConditions for equality.
        /// </summary>
        /// <param name="Object">A charging tariff condition to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffConditions tariffConditions &&
                   Equals(tariffConditions);

        #endregion

        #region Equals(TariffCondition)

        /// <summary>
        /// Compares two TariffConditions for equality.
        /// </summary>
        /// <param name="TariffConditions">A charging tariff condition to compare with.</param>
        public Boolean Equals(TariffConditions? TariffConditions)

            => TariffConditions is not null &&

            ((!ValidFrom.         HasValue && !TariffConditions.ValidFrom.         HasValue) ||
             ( ValidFrom.         HasValue &&  TariffConditions.ValidFrom.         HasValue && ValidFrom.         Value.Equals(TariffConditions.ValidFrom.         Value))) &&

            ((!ValidTo.           HasValue && !TariffConditions.ValidTo.           HasValue) ||
             ( ValidTo.           HasValue &&  TariffConditions.ValidTo.           HasValue && ValidTo.           Value.Equals(TariffConditions.ValidTo.           Value))) &&


               DaysOfWeek.Count().Equals(TariffConditions.DaysOfWeek.Count())   &&
               DaysOfWeek.All(day => TariffConditions.DaysOfWeek.Contains(day)) &&

            ((!StartTimeOfDay.    HasValue && !TariffConditions.StartTimeOfDay.    HasValue) ||
              (StartTimeOfDay.    HasValue &&  TariffConditions.StartTimeOfDay.    HasValue && StartTimeOfDay.    Value.Equals(TariffConditions.StartTimeOfDay.    Value))) &&

            ((!EndTimeOfDay.      HasValue && !TariffConditions.EndTimeOfDay.      HasValue) ||
              (EndTimeOfDay.      HasValue &&  TariffConditions.EndTimeOfDay.      HasValue && EndTimeOfDay.      Value.Equals(TariffConditions.EndTimeOfDay.      Value))) &&


            ((!EVSEKind.          HasValue && !TariffConditions.EVSEKind.          HasValue) ||
              (EVSEKind.          HasValue &&  TariffConditions.EVSEKind.          HasValue && EVSEKind.          Value.Equals(TariffConditions.EVSEKind.          Value))) &&

              String.Equals(PaymentBrand,       TariffConditions.PaymentBrand,       StringComparison.Ordinal) &&
              String.Equals(PaymentRecognition, TariffConditions.PaymentRecognition, StringComparison.Ordinal) &&

            ((!MinEnergy.         HasValue && !TariffConditions.MinEnergy.         HasValue) ||
              (MinEnergy.         HasValue &&  TariffConditions.MinEnergy.         HasValue && MinEnergy.         Value.Equals(TariffConditions.MinEnergy.         Value))) &&

            ((!MaxEnergy.         HasValue && !TariffConditions.MaxEnergy.         HasValue) ||
              (MaxEnergy.         HasValue &&  TariffConditions.MaxEnergy.         HasValue && MaxEnergy.         Value.Equals(TariffConditions.MaxEnergy.         Value))) &&

            ((!MinCurrent.        HasValue && !TariffConditions.MinCurrent.        HasValue) ||
              (MinCurrent.        HasValue &&  TariffConditions.MinCurrent.        HasValue && MinCurrent.        Value.Equals(TariffConditions.MinCurrent.        Value))) &&

            ((!MaxCurrent.        HasValue && !TariffConditions.MaxCurrent.        HasValue) ||
              (MaxCurrent.        HasValue &&  TariffConditions.MaxCurrent.        HasValue && MaxCurrent.        Value.Equals(TariffConditions.MaxCurrent.        Value))) &&

            ((!MinPower.          HasValue && !TariffConditions.MinPower.          HasValue) ||
              (MinPower.          HasValue &&  TariffConditions.MinPower.          HasValue && MinPower.          Value.Equals(TariffConditions.MinPower.          Value))) &&

            ((!MaxPower.          HasValue && !TariffConditions.MaxPower.          HasValue) ||
              (MaxPower.          HasValue &&  TariffConditions.MaxPower.          HasValue && MaxPower.          Value.Equals(TariffConditions.MaxPower.          Value))) &&


            ((!MinTime.           HasValue && !TariffConditions.MinTime.           HasValue) ||
              (MinTime.           HasValue &&  TariffConditions.MinTime.           HasValue && MinTime.           Value.Equals(TariffConditions.MinTime.           Value))) &&

            ((!MaxTime.           HasValue && !TariffConditions.MaxTime.           HasValue) ||
              (MaxTime.           HasValue &&  TariffConditions.MaxTime.           HasValue && MaxTime.           Value.Equals(TariffConditions.MaxTime.           Value))) &&

            ((!MinChargingTime.   HasValue && !TariffConditions.MinChargingTime.   HasValue) ||
              (MinChargingTime.   HasValue &&  TariffConditions.MinChargingTime.   HasValue && MinChargingTime.   Value.Equals(TariffConditions.MinChargingTime.   Value))) &&

            ((!MaxChargingTime.   HasValue && !TariffConditions.MaxChargingTime.   HasValue) ||
              (MaxChargingTime.   HasValue &&  TariffConditions.MaxChargingTime.   HasValue && MaxChargingTime.   Value.Equals(TariffConditions.MaxChargingTime.   Value))) &&

            ((!MinIdleTime.       HasValue && !TariffConditions.MinIdleTime.       HasValue) ||
              (MinIdleTime.       HasValue &&  TariffConditions.MinIdleTime.       HasValue && MinIdleTime.       Value.Equals(TariffConditions.MinIdleTime.       Value))) &&

            ((!MaxIdleTime.       HasValue && !TariffConditions.MaxIdleTime.       HasValue) ||
              (MaxIdleTime.       HasValue &&  TariffConditions.MaxIdleTime.       HasValue && MaxIdleTime.       Value.Equals(TariffConditions.MaxIdleTime.       Value))) &&

               base.Equals(TariffConditions);

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

                   ValidFrom.  HasValue
                       ? " from " + ValidFrom.    Value.ToString("yyyy-MM-dd")
                       : "",

                   ValidTo.    HasValue
                       ? " to "   + ValidTo.      Value.ToString("yyyy-MM-dd")
                       : "",


                   DaysOfWeek.  SafeAny()
                       ? ", "     + DaysOfWeek.AggregateWith("|")
                       : "",

                   StartTimeOfDay.  HasValue
                       ?            StartTimeOfDay.Value.ToString()
                       : "",

                   EndTimeOfDay.    HasValue
                       ? " - "    + EndTimeOfDay.  Value.ToString()
                       : "",


                   MinEnergy.     HasValue
                       ? ", >= "  + MinEnergy.     Value.ToString() + " kWh"
                       : "",

                   MaxEnergy.     HasValue
                       ? ", <= "  + MaxEnergy.     Value.ToString() + " kWh"
                       : "",

                   MinCurrent. HasValue
                       ? ", >= "  + MinCurrent.    Value.ToString() + " A"
                       : "",

                   MaxCurrent. HasValue
                       ? ", <= "  + MaxCurrent.    Value.ToString() + " A"
                       : "",

                   MinPower.   HasValue
                       ? ", >= "  + MinPower.      Value.ToString() + " kW"
                       : "",

                   MaxPower.   HasValue
                       ? ", <= "  + MaxPower.      Value.ToString() + " kW"
                       : "",


                   MinTime.HasValue
                       ? ", > "   + MinTime.      Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MaxTime.HasValue
                       ? ", < "   + MaxTime.      Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MinChargingTime.HasValue
                       ? ", > "   + MinChargingTime.Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MaxChargingTime.HasValue
                       ? ", < "   + MaxChargingTime.Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MinIdleTime.HasValue
                       ? ", > "   + MinIdleTime.  Value.TotalMinutes.ToString("0.00") + " min"
                       : "",

                   MaxIdleTime.HasValue
                       ? ", < "   + MaxIdleTime.  Value.TotalMinutes.ToString("0.00") + " min"
                       : ""

               );

        #endregion

    }

}
