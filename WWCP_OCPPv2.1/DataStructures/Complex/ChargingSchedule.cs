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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
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
        /// The unit of measure the charging limit is expressed in.
        /// </summary>
        [Mandatory]
        public ChargingRateUnits                    ChargingRateUnit           { get; }

        /// <summary>
        /// The optional minimal charging rate supported by the EV.
        /// The unit of measure is defined by the chargingRateUnit.
        /// This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates.
        /// Accepts at most one digit fraction (e.g. 8.1)
        /// </summary>
        [Optional]
        public ChargingRateValue?                   MinChargingRate            { get; }

        /// <summary>
        /// Optional indication whether to ignore the time zone offset in the dateTime fields
        /// of the charging schedule and to use the unqualified local time at the charging
        /// station instead.
        /// This allows the same absolute or recurring charging profile to be used in both summer and winter time.
        /// </summary>
        public Boolean?                             UseLocalTime               { get; }

        /// <summary>
        /// Optional indication whether to delay the start of each charging schedule period
        /// by a randomly chosen number of seconds between 0 and randomizedDelay.
        /// Only allowed for TxProfile and TxDefaultProfile.
        /// </summary>
        public TimeSpan?                            RandomizedDelay            { get; }

        /// <summary>
        /// When defined, any setpoint/limit in the charging schedule must be capped by this
        /// charging rate limit when state-of-charge measurements are greater than or equal
        /// to the state-of-charge limit.
        /// When absent or if state-of-charge measurements are unavailable, the EVSE
        /// shall apply the charging schedule without additional limits.
        /// </summary>
        [Optional]
        public LimitBeyondSoC?                      LimitBeyondSoC             { get; }

        /// <summary>
        /// Optional sales tariff associated with this charging schedule.
        /// </summary>
        [Optional]
        public SalesTariff?                         SalesTariff                { get; }

        /// <summary>
        /// The ISO 15118-20 absolute price schedule.
        /// </summary>
        public AbsolutePriceSchedule?               AbsolutePriceSchedule      { get; }

        /// <summary>
        /// The ISO 15118-20 price level schedule.
        /// </summary>
        public PriceLevelSchedule?                  PriceLevelSchedule         { get; }

        /// <summary>
        /// The identification of this element for referencing in a digital signature.
        /// </summary>
        public UInt32?                              SignatureId                { get; }

        /// <summary>
        /// The Base64-encoded cryptographical hash value (SHA256 for 15118-2,
        /// SHA512 for 15118-20) of the EXI price schedule element.
        /// </summary>
        public String?                              DigestValue                { get; }

        /// <summary>
        /// The power tolerance when following the EV power profile.
        /// </summary>
        public Decimal?                             PowerTolerance             { get; }

        /// <summary>
        /// The enumeration of charging schedule periods defining the maximum power or current usage over time.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods    { get; }

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
        /// <param name="UseLocalTime">Optional indication whether to ignore the time zone offset in the dateTime fields of the charging schedule and to use the unqualified local time at the charging station instead.</param>
        /// <param name="RandomizedDelay">Optional indication whether to delay the start of each charging schedule period by a randomly chosen number of seconds between 0 and randomizedDelay.</param>
        /// <param name="LimitBeyondSoC">When defined, any setpoint/limit in the charging schedule must be capped by this charging rate limit when state-of-charge measurements are greater than or equal to the state-of-charge limit.</param>
        /// <param name="SalesTariff">Optional sales tariff associated with this charging schedule.</param>
        /// <param name="AbsolutePriceSchedule">An ISO 15118-20 absolute price schedule.</param>
        /// <param name="PriceLevelSchedule">An ISO 15118-20 price level schedule.</param>
        /// <param name="SignatureId">An identification of this element for referencing in a digital signature.</param>
        /// <param name="DigestValue">An Base64-encoded cryptographical hash value (SHA256 for 15118-2, SHA512 for 15118-20) of the EXI price schedule element.</param>
        /// <param name="PowerTolerance">The power tolerance when following the EV power profile.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingSchedule(ChargingSchedule_Id                  Id,
                                ChargingRateUnits                    ChargingRateUnit,
                                IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods,
                                DateTime?                            StartSchedule           = null,
                                TimeSpan?                            Duration                = null,
                                ChargingRateValue?                   MinChargingRate         = null,
                                Boolean?                             UseLocalTime            = null,
                                TimeSpan?                            RandomizedDelay         = null,
                                LimitBeyondSoC?                      LimitBeyondSoC          = null,
                                SalesTariff?                         SalesTariff             = null,
                                AbsolutePriceSchedule?               AbsolutePriceSchedule   = null,
                                PriceLevelSchedule?                  PriceLevelSchedule      = null,
                                UInt32?                              SignatureId             = null,
                                String?                              DigestValue             = null,
                                Decimal?                             PowerTolerance          = null,
                                CustomData?                          CustomData              = null)

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
            this.UseLocalTime             = UseLocalTime;
            this.RandomizedDelay          = RandomizedDelay;
            this.LimitBeyondSoC           = LimitBeyondSoC;
            this.SalesTariff              = SalesTariff;
            this.AbsolutePriceSchedule    = AbsolutePriceSchedule;
            this.PriceLevelSchedule       = PriceLevelSchedule;
            this.SignatureId              = SignatureId;
            this.DigestValue              = DigestValue;
            this.PowerTolerance           = PowerTolerance;

            unchecked
            {

                hashCode = Id.                     GetHashCode()       * 47 ^
                           ChargingRateUnit.       GetHashCode()       * 43 ^
                           ChargingSchedulePeriods.CalcHashCode()      * 41 ^
                          (StartSchedule?.         GetHashCode() ?? 0) * 17 ^
                          (Duration?.              GetHashCode() ?? 0) * 31 ^
                          (MinChargingRate?.       GetHashCode() ?? 0) * 29 ^

                          (UseLocalTime?.          GetHashCode() ?? 0) * 31 ^
                          (RandomizedDelay?.       GetHashCode() ?? 0) * 29 ^
                          (LimitBeyondSoC?.        GetHashCode() ?? 0) * 23 ^
                          (SalesTariff?.           GetHashCode() ?? 0) * 19 ^
                          (AbsolutePriceSchedule?. GetHashCode() ?? 0) * 13 ^
                          (PriceLevelSchedule?.    GetHashCode() ?? 0) * 11 ^
                          (SignatureId?.           GetHashCode() ?? 0) *  7 ^
                          (DigestValue?.           GetHashCode() ?? 0) *  5 ^
                          (PowerTolerance?.        GetHashCode() ?? 0) *  3 ^

                           base.                   GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


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
                                       out ChargingRateValue? MinChargingRate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UseLocalTime               [optional]

                if (JSON.ParseOptional("useLocalTime",
                                       "use local time",
                                       out Boolean? UseLocalTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RandomizedDelay            [optional]

                if (JSON.ParseOptional("randomizedDelay",
                                       "randomized delay",
                                       out TimeSpan? RandomizedDelay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region LimitBeyondSoC             [optional]

                if (JSON.ParseOptionalJSON("limitBeyondSoC",
                                           "limit beyond state-of-charge",
                                           OCPPv2_1.LimitBeyondSoC.TryParse,
                                           out LimitBeyondSoC? LimitBeyondSoC,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SalesTariff                [optional]

                if (JSON.ParseOptionalJSON("salesTariff",
                                           "sales tariff",
                                           OCPPv2_1.SalesTariff.TryParse,
                                           out SalesTariff? SalesTariff,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AbsolutePriceSchedule      [optional]

                if (JSON.ParseOptionalJSON("absolutePriceSchedule",
                                           "absolute price schedule",
                                           ISO15118_20.CommonMessages.AbsolutePriceSchedule.TryParse,
                                           out AbsolutePriceSchedule? AbsolutePriceSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PriceLevelSchedule         [optional]

                if (JSON.ParseOptionalJSON("priceLevelSchedule",
                                           "price level schedule",
                                           ISO15118_20.CommonMessages.PriceLevelSchedule.TryParse,
                                           out PriceLevelSchedule? PriceLevelSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SignatureId                [optional]

                if (JSON.ParseOptional("signatureId",
                                       "signature identification",
                                       out UInt32? SignatureId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DigestValue                [optional]

                if (JSON.ParseOptional("digestValue",
                                       "price level schedule",
                                       out String? DigestValue,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PowerTolerance             [optional]

                if (JSON.ParseOptional("powerTolerance",
                                       "price level schedule",
                                       out Decimal? PowerTolerance,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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


                ChargingSchedule = new ChargingSchedule(
                                       Id,
                                       ChargingRateUnit,
                                       ChargingSchedulePeriods,
                                       StartSchedule,
                                       Duration,
                                       MinChargingRate,
                                       UseLocalTime,
                                       RandomizedDelay,
                                       LimitBeyondSoC,
                                       SalesTariff,
                                       AbsolutePriceSchedule,
                                       PriceLevelSchedule,
                                       SignatureId,
                                       DigestValue,
                                       PowerTolerance,
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

        #region ToJSON(CustomChargingScheduleSerializer = null, CustomLimitBeyondSoCSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomLimitBeyondSoCSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomV2XFreqWattEntrySerializer">A delegate to serialize custom V2X Frequency-Watt entrys.</param>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X Signal-Watt entrys.</param>
        /// 
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// 
        /// <param name="CustomAbsolutePriceScheduleSerializer">A delegate to serialize custom absolute price schedules.</param>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        /// 
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        /// 
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedule>?         CustomChargingScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<LimitBeyondSoC>?           CustomLimitBeyondSoCSerializer            = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?   CustomChargingSchedulePeriodSerializer    = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?         CustomV2XFreqWattEntrySerializer          = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?       CustomV2XSignalWattEntrySerializer        = null,

                              CustomJObjectSerializerDelegate<SalesTariff>?              CustomSalesTariffSerializer               = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?         CustomSalesTariffEntrySerializer          = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?     CustomRelativeTimeIntervalSerializer      = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?          CustomConsumptionCostSerializer           = null,
                              CustomJObjectSerializerDelegate<Cost>?                     CustomCostSerializer                      = null,

                              CustomJObjectSerializerDelegate<AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer     = null,
                              CustomJObjectSerializerDelegate<PriceRuleStack>?           CustomPriceRuleStackSerializer            = null,
                              CustomJObjectSerializerDelegate<PriceRule>?                CustomPriceRuleSerializer                 = null,
                              CustomJObjectSerializerDelegate<TaxRule>?                  CustomTaxRuleSerializer                   = null,
                              CustomJObjectSerializerDelegate<OverstayRuleList>?         CustomOverstayRuleListSerializer          = null,
                              CustomJObjectSerializerDelegate<OverstayRule>?             CustomOverstayRuleSerializer              = null,
                              CustomJObjectSerializerDelegate<AdditionalService>?        CustomAdditionalServiceSerializer         = null,

                              CustomJObjectSerializerDelegate<PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer   = null,

                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                       Id.                 ToString()),

                           StartSchedule.HasValue
                               ? new JProperty("startSchedule",            StartSchedule.Value.ToIso8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",                 (UInt64) Math.Round(Duration.Value.TotalSeconds, 0))
                               : null,

                                 new JProperty("chargingRateUnit",         ChargingRateUnit.   AsText()),

                           MinChargingRate.HasValue
                               ? new JProperty("minChargingRate",          MinChargingRate.Value.Value)
                               : null,

                           UseLocalTime.HasValue
                               ? new JProperty("useLocalTime",             UseLocalTime.Value)
                               : null,

                           RandomizedDelay.HasValue
                               ? new JProperty("randomizedDelay",          (Int32) RandomizedDelay.Value.TotalSeconds)
                               : null,

                           LimitBeyondSoC is not null
                               ? new JProperty("limitBeyondSoC",           LimitBeyondSoC.       ToJSON(CustomLimitBeyondSoCSerializer,
                                                                                                        CustomCustomDataSerializer))
                               : null,

                           SalesTariff is not null
                               ? new JProperty("salesTariff",              SalesTariff.          ToJSON(CustomSalesTariffSerializer,
                                                                                                        CustomSalesTariffEntrySerializer,
                                                                                                        CustomRelativeTimeIntervalSerializer,
                                                                                                        CustomConsumptionCostSerializer,
                                                                                                        CustomCostSerializer,
                                                                                                        CustomCustomDataSerializer))
                               : null,

                           AbsolutePriceSchedule is not null
                               ? new JProperty("absolutePriceSchedule",    AbsolutePriceSchedule.ToJSON(CustomAbsolutePriceScheduleSerializer,
                                                                                                        CustomPriceRuleStackSerializer,
                                                                                                        CustomPriceRuleSerializer,
                                                                                                        CustomTaxRuleSerializer,
                                                                                                        CustomOverstayRuleListSerializer,
                                                                                                        CustomOverstayRuleSerializer,
                                                                                                        CustomAdditionalServiceSerializer))
                               : null,

                           PriceLevelSchedule is not null
                               ? new JProperty("priceLevelSchedule",       PriceLevelSchedule.   ToJSON(CustomPriceLevelScheduleSerializer,
                                                                                                        CustomPriceLevelScheduleEntrySerializer))
                               : null,

                           SignatureId.HasValue
                               ? new JProperty("signatureId",              SignatureId.Value)
                               : null,

                           DigestValue is not null
                               ? new JProperty("digestValue",              DigestValue)
                               : null,

                           PowerTolerance.HasValue
                               ? new JProperty("powerTolerance",           PowerTolerance.Value)
                               : null,

                                 new JProperty("chargingSchedulePeriod",   new JArray(ChargingSchedulePeriods.Select(chargingSchedulePeriod => chargingSchedulePeriod.ToJSON(CustomChargingSchedulePeriodSerializer,
                                                                                                                                                                             CustomV2XFreqWattEntrySerializer,
                                                                                                                                                                             CustomV2XSignalWattEntrySerializer,
                                                                                                                                                                             CustomCustomDataSerializer)))),

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

             ((LimitBeyondSoC        is     null &&  ChargingSchedule.LimitBeyondSoC        is     null) ||
              (LimitBeyondSoC        is not null &&  ChargingSchedule.LimitBeyondSoC        is not null && LimitBeyondSoC.       Equals(ChargingSchedule.LimitBeyondSoC)))           &&

               ChargingSchedulePeriods.Count().Equals(ChargingSchedule.ChargingSchedulePeriods.Count())     &&
               ChargingSchedulePeriods.All(chargingSchedulePeriod => ChargingSchedule.ChargingSchedulePeriods.Contains(chargingSchedulePeriod)) &&

            ((!StartSchedule.        HasValue    && !ChargingSchedule.StartSchedule.        HasValue)    ||
              (StartSchedule.        HasValue    &&  ChargingSchedule.StartSchedule.        HasValue    && StartSchedule.  Value.Equals(ChargingSchedule.StartSchedule.  Value))) &&

            ((!Duration.             HasValue    && !ChargingSchedule.Duration.             HasValue)    ||
              (Duration.             HasValue    &&  ChargingSchedule.Duration.             HasValue    && Duration.       Value.Equals(ChargingSchedule.Duration.       Value))) &&

            ((!MinChargingRate.      HasValue    && !ChargingSchedule.MinChargingRate.      HasValue)    ||
              (MinChargingRate.      HasValue    &&  ChargingSchedule.MinChargingRate.      HasValue    && MinChargingRate.Value.Equals(ChargingSchedule.MinChargingRate.Value))) &&

             ((SalesTariff           is     null &&  ChargingSchedule.SalesTariff           is     null) ||
              (SalesTariff           is not null &&  ChargingSchedule.SalesTariff           is not null && SalesTariff.          Equals(ChargingSchedule.SalesTariff)))           &&

             ((AbsolutePriceSchedule is     null &&  ChargingSchedule.AbsolutePriceSchedule is     null) ||
              (AbsolutePriceSchedule is not null &&  ChargingSchedule.AbsolutePriceSchedule is not null && AbsolutePriceSchedule.Equals(ChargingSchedule.AbsolutePriceSchedule))) &&

             ((PriceLevelSchedule    is     null &&  ChargingSchedule.PriceLevelSchedule    is     null) ||
              (PriceLevelSchedule    is not null &&  ChargingSchedule.PriceLevelSchedule    is not null && PriceLevelSchedule.   Equals(ChargingSchedule.PriceLevelSchedule)))    &&

            ((!SignatureId.          HasValue    && !ChargingSchedule.SignatureId.          HasValue)    ||
              (SignatureId.          HasValue    &&  ChargingSchedule.SignatureId.          HasValue    && SignatureId.    Value.Equals(ChargingSchedule.SignatureId.    Value))) &&

             ((DigestValue           is     null &&  ChargingSchedule.DigestValue           is     null) ||
              (DigestValue           is not null &&  ChargingSchedule.DigestValue           is not null && DigestValue.          Equals(ChargingSchedule.DigestValue)))           &&

            ((!PowerTolerance.       HasValue    && !ChargingSchedule.PowerTolerance.       HasValue)    ||
              (PowerTolerance.       HasValue    &&  ChargingSchedule.PowerTolerance.       HasValue    && PowerTolerance. Value.Equals(ChargingSchedule.PowerTolerance. Value))) &&

               base.Equals(ChargingSchedule);

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

                   Id,
                   " / ",
                   ChargingRateUnit.AsText(),

                   " with ", ChargingSchedulePeriods.Count(), " charging schedule periods"

               );

        #endregion

    }

}
