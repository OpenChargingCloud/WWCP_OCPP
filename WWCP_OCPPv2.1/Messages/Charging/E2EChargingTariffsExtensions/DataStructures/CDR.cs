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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A read-only signable charge detail record.
    /// </summary>
    public class CDR : ACustomSignableData,
                       IHasId<CDR_Id>,
                       IEquatable<CDR>,
                       IComparable<CDR>,
                       IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cdr");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the charge detail record.
        /// </summary>
        [Mandatory]
        public CDR_Id                            Id                        { get; }

        /// <summary>
        /// The timestamp when this tariff was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public DateTimeOffset                    Created                   { get; }

        /// <summary>
        /// Optional references to other tariffs, which will be replaced by this charge detail record.
        /// </summary>
        [Optional]
        public IEnumerable<CDR_Id>               Replaces                  { get; }

        /// <summary>
        /// Optional references to other tariffs, e.g. because some local adaption of a charge detail record was required.
        /// </summary>
        [Optional]
        public IEnumerable<CDR_Id>               References                { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public Provider_Id                       ProviderId                { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public DisplayTexts                      ProviderName              { get; }

        /// <summary>
        /// The charging station operator identification.
        /// </summary>
        [Mandatory]
        public CSOOperator_Id                    CSOOperatorId             { get; }

        /// <summary>
        /// The EVSE identification.
        /// </summary>
        [Mandatory]
        public GlobalEVSE_Id                     EVSEId                    { get; }

        /// <summary>
        /// An optional enumeration of charging station identifications, this tariff is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>?  ChargingStationIds        { get; }

        /// <summary>
        /// An optional enumeration of charging pool identifications, this tariff is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>?     ChargingPoolIds           { get; }

        /// <summary>
        /// The optional charge detail record.
        /// </summary>
        [Optional]
        public Tariff?                           ChargingTariff            { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public Price?                            Price                     { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Optional]
        public Currency                          Currency                  { get; }


        public IEnumerable<MeteringValue>        MeteringValues            { get; }

        public IEnumerable<ChargingPeriod>       ChargingPeriods           { get; }




        public Price                             TotalFixedCost            { get; }
        public Price                             TotalReservationCost      { get; }

        public TimeSpan                          TotalTime                 { get; }
        public TimeSpan                          BilledTime                { get; }
        public Price                             TotalTimeCost             { get; }


        public TimeSpan                          TotalChargingTime         { get; }
        public TimeSpan                          BilledChargingTime        { get; }
        public Price                             BilledChargingTimeCost    { get; }


        public WattHour                          TotalEnergy               { get; }
        public WattHour                          BilledEnergy              { get; }
        public Price                             BilledEnergyCost          { get; }


        public TimeSpan                          TotalParkingTime          { get; }
        public TimeSpan                          BilledParkingTime         { get; }
        public Price                             TotalParkingCost          { get; }


        public Price                             TotalCost                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record.
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charge detail record.</param>
        /// <param name="ProviderId">An unique identification of the e-mobility provider responsible for this tariff.</param>
        /// <param name="ProviderName">An multi-language name of the e-mobility provider responsible for this tariff.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="MeteringValues">An enumeration of metering values.</param>
        /// 
        /// <param name="Created">An optional timestamp when this tariff was created.</param>
        /// <param name="Replaces">Optional references to other tariffs, which will be replaced by this charge detail record.</param>
        /// <param name="References">Optional references to other tariffs, e.g. because some local adaption of a charge detail record was required.</param>
        /// 
        /// <param name="Description">An optional multi-language tariff description.</param>
        /// <param name="URL">An optional informative (not legally binding) URL to a web page that contains an explanation of the tariff information in human readable form.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this charge detail record.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this charge detail record.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CDR(CDR_Id                            Id,

                   Provider_Id                       ProviderId,
                   DisplayTexts                      ProviderName,

                   CSOOperator_Id                    CSOOperatorId,
                   GlobalEVSE_Id                     EVSEId,
                   IEnumerable<ChargingStation_Id>?  ChargingStationIds,
                   IEnumerable<ChargingPool_Id>?     ChargingPoolIds,
                   IEnumerable<MeteringValue>        MeteringValues,

                   Price                             TotalFixedCost,
                   Price                             TotalReservationCost,

                   TimeSpan                          TotalTime,
                   TimeSpan                          BilledTime,
                   Price                             TotalTimeCost,

                   TimeSpan                          TotalChargingTime,
                   TimeSpan                          BilledChargingTime,
                   Price                             TotalChargingTimeCost,

                   WattHour                          TotalEnergy,
                   WattHour                          BilledEnergy,
                   Price                             TotalEnergyCost,

                   TimeSpan                          TotalParkingTime,
                   TimeSpan                          BilledParkingTime,
                   Price                             TotalParkingCost,

                   Price                             TotalCost,
                   Currency                          Currency,

                   DateTimeOffset?                   Created              = null,
                   IEnumerable<CDR_Id>?              Replaces             = null,
                   IEnumerable<CDR_Id>?              References           = null,
                   Tariff?                           ChargingTariff       = null,
                   IEnumerable<ChargingPeriod>?      ChargingPeriods      = null,

                   DisplayTexts?                     Description          = null,
                   URL?                              URL                  = null,

                   IEnumerable<KeyPair>?             SignKeys             = null,
                   IEnumerable<SignInfo>?            SignInfos            = null,
                   IEnumerable<Signature>?           Signatures           = null,

                   CustomData?                       CustomData           = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            if (!MeteringValues.Any())
                throw new ArgumentNullException(nameof(MeteringValues), "The given enumeration of tariff elements must not be null or empty!");

            this.Id                     = Id;
            this.ProviderId             = ProviderId;
            this.ProviderName           = ProviderName;
            this.Currency               = Currency;

            this.CSOOperatorId          = CSOOperatorId;
            this.EVSEId                 = EVSEId;
            this.ChargingStationIds     = ChargingStationIds?.Distinct();
            this.ChargingPoolIds        = ChargingPoolIds?.   Distinct();
            this.MeteringValues         = MeteringValues.     Distinct();
            this.ChargingPeriods        = ChargingPeriods?.   Distinct() ?? [];

            this.TotalFixedCost         = TotalFixedCost;
            this.TotalReservationCost   = TotalReservationCost;

            this.TotalTime              = TotalTime;
            this.BilledTime             = BilledTime;
            this.TotalTimeCost          = TotalTimeCost;

            this.TotalChargingTime      = TotalChargingTime;
            this.BilledChargingTime     = BilledChargingTime;
            this.BilledChargingTimeCost  = TotalChargingTimeCost;

            this.TotalEnergy            = TotalEnergy;
            this.BilledEnergy           = BilledEnergy;
            this.BilledEnergyCost        = TotalEnergyCost;

            this.TotalParkingTime       = TotalParkingTime;
            this.BilledParkingTime      = BilledParkingTime;
            this.TotalParkingCost       = TotalParkingCost;

            this.TotalCost              = TotalCost;

            this.Created                = Created     ?? Timestamp.Now;
            this.Replaces               = Replaces?.  Distinct() ?? [];
            this.References             = References?.Distinct() ?? [];
            this.ChargingTariff         = ChargingTariff;


            unchecked
            {

                hashCode = this.Id.                GetHashCode()       * 71 ^
                           this.ProviderId.        GetHashCode()       * 67 ^
                           this.ProviderName.      GetHashCode()       * 61 ^
                           this.Currency.          GetHashCode()       * 59 ^

                           this.Created.           GetHashCode()       * 47 ^
                           this.Replaces.          CalcHashCode()      * 44 ^
                           this.References.        CalcHashCode()      * 41 ^

                           base.                   GetHashCode();

            }

        }

        #endregion


        public static Boolean CalculateCosts(Provider_Id                       ProviderId,
                                             DisplayTexts                      ProviderName,
                                             CSOOperator_Id                    CSOOperatorId,
                                             GlobalEVSE_Id                     EVSEId,
                                             IEnumerable<MeterValue>           MeterValues,
                                             Tariff                            ChargingTariff,
                                             out CDR?                          CDR,
                                             out String?                       ErrorResponse,

                                             IEnumerable<ChargingStation_Id>?  ChargingStationIds    = null,
                                             IEnumerable<ChargingPool_Id>?     ChargingPoolIds       = null,
                                             Measurand?                        Measurand             = null,
                                             MeasurementLocation?              MeasurementLocation   = null)
        {

            CDR                     = null;
            ErrorResponse           = null;
            Measurand             ??= OCPPv2_1.Measurand.Current_Import_Offered;
            MeasurementLocation   ??= OCPPv2_1.MeasurementLocation.Outlet;

            var meterValues         = MeterValues.OrderBy(meterValue => meterValue.Timestamp).ToArray();

            #region Get Start Metering Value

            var startMeterValues    = meterValues.Where(meterValue => meterValue.SampledValues.Any(sampledValue => sampledValue.Measurand           == Measurand           &&
                                                                                                                   sampledValue.MeasurementLocation == MeasurementLocation &&
                                                                                                                   sampledValue.Context             == ReadingContext.TransactionBegin)).ToArray();
            if (startMeterValues.Length != 1)
            {
                ErrorResponse = startMeterValues.Length == 0
                                    ? "No 'TransactionBegin' meter value found!"
                                    : "More than one 'TransactionBegin' meter value found!";
                return false;
            }

            var startSampledValues  = startMeterValues.First().SampledValues.Where(sampledValue => sampledValue.Measurand           == Measurand           &&
                                                                                                   sampledValue.MeasurementLocation == MeasurementLocation &&
                                                                                                   sampledValue.Context             == ReadingContext.TransactionBegin).ToArray();
            if (startSampledValues.Length != 1)
            {
                ErrorResponse = startMeterValues.Length == 0
                                    ? "No 'TransactionBegin' sampled value found!"
                                    : "More than one 'TransactionBegin' sampled value found!";
                return false;
            }

            var startMeteringValue  = new MeteringValue(
                                          startMeterValues.  First().Timestamp,
                                          startSampledValues.First().Value,
                                          startSampledValues.First().Context,
                                          startSampledValues.First().Measurand,
                                          startSampledValues.First().Phase,
                                          startSampledValues.First().MeasurementLocation,
                                          startSampledValues.First().SignedMeterValue,
                                          startSampledValues.First().UnitOfMeasure,
                                          startSampledValues.First().CustomData
                                      );

            #endregion

            #region Get Stop  Metering Value

            var stopMeterValues     = meterValues.Where(meterValue => meterValue.SampledValues.Any(sampledValue => sampledValue.Measurand           == Measurand           &&
                                                                                                                   sampledValue.MeasurementLocation == MeasurementLocation &&
                                                                                                                   sampledValue.Context             == ReadingContext.TransactionEnd)).ToArray();
            if (stopMeterValues.Length != 1)
            {
                ErrorResponse = stopMeterValues.Length == 0
                                    ? "No 'TransactionEnd' meter value found!"
                                    : "More than one 'TransactionEnd' meter value found!";
                return false;
            }

            var stopSampledValues   = stopMeterValues.First().SampledValues.Where(sampledValue => sampledValue.Measurand           == Measurand           &&
                                                                                                  sampledValue.MeasurementLocation == MeasurementLocation &&
                                                                                                  sampledValue.Context             == ReadingContext.TransactionEnd).ToArray();
            if (stopSampledValues.Length != 1)
            {
                ErrorResponse = stopMeterValues.Length == 0
                                    ? "No 'TransactionEnd' sampled value found!"
                                    : "More than one 'TransactionEnd' sampled value found!";
                return false;
            }


            var stopMeteringValue   = new MeteringValue(
                                          stopMeterValues.  First().Timestamp,
                                          stopSampledValues.First().Value,
                                          stopSampledValues.First().Context,
                                          stopSampledValues.First().Measurand,
                                          stopSampledValues.First().Phase,
                                          stopSampledValues.First().MeasurementLocation,
                                          stopSampledValues.First().SignedMeterValue,
                                          stopSampledValues.First().UnitOfMeasure,
                                          stopSampledValues.First().CustomData
                                      );

            #endregion

            #region Calculate TotalChargingTime

            var totalChargingTime = stopMeteringValue.Timestamp - startMeteringValue.Timestamp;
            if (totalChargingTime.TotalSeconds < 0)
            {
                ErrorResponse = $"Transaction total charging time is invalid: {totalChargingTime}!";
                return false;
            }

            #endregion

            #region Calculate TotalIdleTime / TotalTime

            // Without charging periods or charging state information only the overall duration
            // of the transaction is known, thus all time is considered to be charging time!
            var totalIdleTime  = TimeSpan.Zero;
            var totalTime      = totalChargingTime + totalIdleTime;

            #endregion

            #region Calculate TotalEnergy

            if (!TryConvertToWattHours(startMeteringValue, out var startWattHours, out ErrorResponse) ||
                !TryConvertToWattHours(stopMeteringValue,  out var stopWattHours,  out ErrorResponse))
            {
                return false;
            }

            var totalEnergy = WattHour.FromWh(stopWattHours - startWattHours);

            if (totalEnergy.Value < 0)
            {
                ErrorResponse = $"Transaction total energy is invalid: {totalEnergy}!";
                return false;
            }

            #endregion


            if (ChargingTariff.Energy       is null &&
                ChargingTariff.ChargingTime is null &&
                ChargingTariff.IdleTime     is null &&
                ChargingTariff.FixedFee     is null)
            {
                ErrorResponse = "The given charging tariff does not define any price components!";
                return false;
            }


            #region Tariff condition matching

            // Tariff conditions are evaluated at the start of the charging session and
            // against the totals of the entire session. Splitting a charging session
            // into multiple charging periods with different prices, e.g. when a tariff
            // price changes during the session, is not yet supported!
            var sessionStart      = startMeteringValue.Timestamp;
            var sessionStartDate  = DateOnly.FromDateTime(sessionStart.DateTime);
            var sessionStartTime  = TimeOnly.FromDateTime(sessionStart.DateTime);

            Boolean ConditionsMatch(TariffConditions? Conditions)
            {

                if (Conditions is null)
                    return true;

                // Conditions that cannot be evaluated on start/stop metering values alone
                // (EVSE kind, current and power limits) exclude the price component,
                // as we cannot proof that the condition was met!
                if (Conditions.EVSEKind.  HasValue ||
                    Conditions.MinCurrent.HasValue ||
                    Conditions.MaxCurrent.HasValue ||
                    Conditions.MinPower.  HasValue ||
                    Conditions.MaxPower.  HasValue)
                {
                    return false;
                }

                if (Conditions.ValidFrom.HasValue && sessionStartDate <  Conditions.ValidFrom.Value)
                    return false;

                if (Conditions.ValidTo.  HasValue && sessionStartDate >= Conditions.ValidTo.  Value)
                    return false;

                if (Conditions.DaysOfWeek.Any() && !Conditions.DaysOfWeek.Contains(sessionStart.DayOfWeek))
                    return false;

                if (Conditions.StartTimeOfDay.HasValue || Conditions.EndTimeOfDay.HasValue)
                {

                    // TimeOnly.IsBetween(...) treats the start as inclusive, the end as exclusive
                    // and supports time ranges that span midnight!
                    if (!sessionStartTime.IsBetween(
                             Conditions.StartTimeOfDay ?? TimeOnly.MinValue,
                             Conditions.EndTimeOfDay   ?? TimeOnly.MinValue
                         ))
                    {
                        return false;
                    }

                }

                if (Conditions.MinEnergy.      HasValue && totalEnergy.Value  <  Conditions.MinEnergy.Value.Value)
                    return false;

                if (Conditions.MaxEnergy.      HasValue && totalEnergy.Value  >= Conditions.MaxEnergy.Value.Value)
                    return false;

                if (Conditions.MinTime.        HasValue && totalTime          <  Conditions.MinTime.        Value)
                    return false;

                if (Conditions.MaxTime.        HasValue && totalTime          >= Conditions.MaxTime.        Value)
                    return false;

                if (Conditions.MinChargingTime.HasValue && totalChargingTime  <  Conditions.MinChargingTime.Value)
                    return false;

                if (Conditions.MaxChargingTime.HasValue && totalChargingTime  >= Conditions.MaxChargingTime.Value)
                    return false;

                if (Conditions.MinIdleTime.    HasValue && totalIdleTime      <  Conditions.MinIdleTime.    Value)
                    return false;

                if (Conditions.MaxIdleTime.    HasValue && totalIdleTime      >= Conditions.MaxIdleTime.    Value)
                    return false;

                return true;

            }

            #endregion

            #region Price/StepSize helper methods

            // Tax rates having the same stack level are applied to the same base amount,
            // higher stack levels are applied on top of all lower stack levels.
            Price ApplyTaxRates(Decimal NetAmount, IEnumerable<TaxRate> TaxRates)
            {

                var includingTaxes = NetAmount;

                foreach (var stackLevel in TaxRates.GroupBy  (taxRate => taxRate.Stack ?? 0).
                                                    OrderBy  (group   => group.Key))
                {

                    var taxBase = includingTaxes;

                    foreach (var taxRate in stackLevel)
                        includingTaxes += taxBase * taxRate.Tax.Value / 100;

                }

                return new Price(
                           NetAmount,
                           includingTaxes,
                           TaxRates
                       );

            }

            // "When absent, the exact amount is billed. When present, this type is billed
            //  in blocks of stepSize of the base unit. Amounts are rounded up to a multiple
            //  of stepSize."
            static TimeSpan RoundUpToStepSize(TimeSpan Total, TimeSpan? StepSize)

                => !StepSize.HasValue || StepSize.Value <= TimeSpan.Zero || Total <= TimeSpan.Zero
                       ? Total
                       : TimeSpan.FromTicks(
                             StepSize.Value.Ticks * (Int64) Math.Ceiling(Total.Ticks / (Double) StepSize.Value.Ticks)
                         );

            static WattHour RoundUpEnergyToStepSize(WattHour Total, WattHour? StepSize)

                => !StepSize.HasValue || StepSize.Value.Value <= 0 || Total.Value <= 0
                       ? Total
                       : WattHour.FromWh(
                             StepSize.Value.Value * Math.Ceiling(Total.Value / StepSize.Value.Value)
                         );

            #endregion


            #region Calculate TotalFixedCost          (FixedFee)

            var fixedFeePrice          = ChargingTariff.FixedFee?.Prices.FirstOrDefault(price => ConditionsMatch(price.Conditions));

            var totalFixedCost         = fixedFeePrice is not null
                                             ? ApplyTaxRates(fixedFeePrice.PriceFixed, ChargingTariff.FixedFee!.TaxRates)
                                             : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate BilledChargingTime/Cost (ChargingTime)

            var chargingTimePrice      = ChargingTariff.ChargingTime?.Prices.FirstOrDefault(price => ConditionsMatch(price.Conditions));

            var billedChargingTime     = chargingTimePrice is not null
                                             ? RoundUpToStepSize(totalChargingTime, chargingTimePrice.StepSize)
                                             : TimeSpan.Zero;

            var totalChargingTimeCost  = chargingTimePrice is not null
                                             ? ApplyTaxRates(
                                                   ((Decimal) billedChargingTime.TotalMinutes) * chargingTimePrice.PriceMinute,
                                                   ChargingTariff.ChargingTime!.TaxRates
                                               )
                                             : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate BilledParkingTime/Cost  (IdleTime)

            var idleTimePrice          = ChargingTariff.IdleTime?.Prices.FirstOrDefault(price => ConditionsMatch(price.Conditions));

            var billedParkingTime      = idleTimePrice is not null
                                             ? RoundUpToStepSize(totalIdleTime, idleTimePrice.StepSize)
                                             : TimeSpan.Zero;

            var totalParkingCost       = idleTimePrice is not null
                                             ? ApplyTaxRates(
                                                   ((Decimal) billedParkingTime.TotalMinutes) * idleTimePrice.PriceMinute,
                                                   ChargingTariff.IdleTime!.TaxRates
                                               )
                                             : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate BilledEnergy/Cost       (Energy)

            var energyPrice            = ChargingTariff.Energy?.Prices.FirstOrDefault(price => ConditionsMatch(price.Conditions));

            var billedEnergy           = energyPrice is not null
                                             ? RoundUpEnergyToStepSize(totalEnergy, energyPrice.StepSize)
                                             : WattHour.Zero;

            var totalEnergyCost        = energyPrice is not null
                                             ? ApplyTaxRates(
                                                   billedEnergy.kWh * energyPrice.PriceKWh,
                                                   ChargingTariff.Energy!.TaxRates
                                               )
                                             : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate TotalCost               (incl. MinCost/MaxCost clamping)

            // OCPP v2.1 tariffs no longer define an overall-time price component,
            // only charging time and idle time, thus the overall billed time is
            // just the sum of both and has no own costs.
            var billedTime            = billedChargingTime + billedParkingTime;
            var totalTimeCost         = OCPPv2_1.Price.Zero;

            // Reservation costs are not yet supported!
            var totalReservationCost  = OCPPv2_1.Price.Zero;

            var totalCost             = totalFixedCost + totalReservationCost + totalChargingTimeCost + totalEnergyCost + totalParkingCost;

            if (ChargingTariff.MinCost.HasValue)
                totalCost = new Price(
                                Math.Max(totalCost.ExcludingTaxes, ChargingTariff.MinCost.Value.ExcludingTaxes),
                                Math.Max(totalCost.IncludingTaxes, ChargingTariff.MinCost.Value.IncludingTaxes)
                            );

            if (ChargingTariff.MaxCost.HasValue)
                totalCost = new Price(
                                Math.Min(totalCost.ExcludingTaxes, ChargingTariff.MaxCost.Value.ExcludingTaxes),
                                Math.Min(totalCost.IncludingTaxes, ChargingTariff.MaxCost.Value.IncludingTaxes)
                            );

            #endregion


            CDR = new CDR(

                      CDR_Id.NewRandom(ProviderId),

                      ProviderId,
                      ProviderName,

                      CSOOperatorId,
                      EVSEId,
                      ChargingStationIds,
                      ChargingPoolIds,
                      [
                          startMeteringValue,
                          stopMeteringValue
                      ],

                      totalFixedCost,
                      totalReservationCost,

                      totalTime,
                      billedTime,
                      totalTimeCost,

                      totalChargingTime,
                      billedChargingTime,
                      totalChargingTimeCost,

                      totalEnergy,
                      billedEnergy,
                      totalEnergyCost,

                      totalIdleTime,
                      billedParkingTime,
                      totalParkingCost,

                      totalCost,
                      ChargingTariff.Currency,

                      ChargingTariff: ChargingTariff

                  );

            return true;

        }


        #region (private static) TryConvertToWattHours(MeteringValue, out WattHours, out ErrorResponse)

        /// <summary>
        /// Convert the given metering value into WattHours honoring
        /// its unit of measure and multiplier (exponent to base 10).
        /// </summary>
        /// <param name="MeteringValue">A metering value.</param>
        /// <param name="WattHours">The converted WattHours.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        private static Boolean TryConvertToWattHours(MeteringValue  MeteringValue,
                                                     out Decimal    WattHours,
                                                     out String?    ErrorResponse)
        {

            WattHours      = 0;
            ErrorResponse  = null;

            // When no unit of measure is given, the default is "Wh" with multiplier 0 (10^0 == *1)!
            var unit        = MeteringValue.UnitOfMeasure?.Unit       ?? UnitOfMeasure.Wh;
            var multiplier  = MeteringValue.UnitOfMeasure?.Multiplier ?? 0;

            var scale       = (Decimal) Math.Pow(10, multiplier);

            if      (unit == UnitOfMeasure.Wh)
                WattHours = MeteringValue.Value * scale;

            else if (unit == UnitOfMeasure.kWh)
                WattHours = MeteringValue.Value * scale * 1000;

            else
            {
                ErrorResponse = $"The unit of measure '{unit}' of the metering value is not an energy unit!";
                return false;
            }

            return true;

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charge detail record.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRIdURL">An optional charge detail record identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom charge detail record JSON objects.</param>
        public static CDR Parse(JObject                                       JSON,
                                           CDR_Id?                            CDRIdURL   = null,
                                           CustomJObjectParserDelegate<CDR>?  CustomCDRParser    = null)
        {

            if (TryParse(JSON,
                         out var cdr,
                         out var errorResponse,
                         CDRIdURL,
                         CustomCDRParser) &&
                cdr is not null)
            {
                return cdr;
            }

            throw new ArgumentException("The given JSON representation of a charge detail record is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CDR, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed charge detail record.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out CDR?     CDR,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out CDR,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed charge detail record.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CDRIdURL">An optional charge detail record identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom charge detail record JSON objects.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       out CDR?                           CDR,
                                       out String?                        ErrorResponse,
                                       CDR_Id?                            CDRIdURL          = null,
                                       CustomJObjectParserDelegate<CDR>?  CustomCDRParser   = null)
        {

            try
            {

                CDR = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                    [optional]

                if (JSON.ParseOptional("id",
                                       "tariff identification",
                                       CDR_Id.TryParse,
                                       out CDR_Id? CDRIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CDRIdURL.HasValue && !CDRIdBody.HasValue)
                {
                    ErrorResponse = "The tariff identification is missing!";
                    return false;
                }

                if (CDRIdURL.HasValue && CDRIdBody.HasValue && CDRIdURL.Value != CDRIdBody.Value)
                {
                    ErrorResponse = "The optional tariff identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse ProviderId            [mandatory]

                if (!JSON.ParseMandatory("providerId",
                                         "provider identification",
                                         Provider_Id.TryParse,
                                         out Provider_Id ProviderId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ProviderName          [mandatory]

                if (!JSON.ParseMandatoryJSON("provider_name",
                                             "provider name",
                                             DisplayTexts.TryParse,
                                             out DisplayTexts? ProviderName,
                                             out ErrorResponse) ||
                     ProviderName is null)
                {
                    return false;
                }

                #endregion

                #region Parse Currency              [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MeteringValues        [mandatory]

                if (!JSON.ParseMandatoryHashSet("meterValues",
                                                "meter values",
                                                MeteringValue.TryParse,
                                                out HashSet<MeteringValue> MeteringValues,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalTime             [mandatory]

                if (!JSON.ParseMandatory("totalTime",
                                         "total time",
                                         out TimeSpan TotalTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BilledTime            [mandatory]

                if (!JSON.ParseMandatory("BilledTime",
                                         "billed time",
                                         out TimeSpan BilledTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalTimeCost         [mandatory]

                if (!JSON.ParseMandatoryJSON("totalTimeCost",
                                             "total time cost",
                                             OCPPv2_1.Price.TryParse,
                                             out Price TotalTimeCost,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion



                #region Parse Consumption           [mandatory]

                if (!JSON.ParseMandatory("consumption",
                                         "consumption",
                                         out WattHour Consumption,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created               [mandatory]

                if (!JSON.ParseMandatory("created",
                                         "created",
                                         out DateTime Created,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Replaces              [optional]

                if (JSON.ParseOptionalHashSet("replaces",
                                              "replaces tariff",
                                              CDR_Id.TryParse,
                                              out HashSet<CDR_Id> Replaces,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse References            [optional]

                if (JSON.ParseOptionalHashSet("references",
                                              "references tariff",
                                              CDR_Id.TryParse,
                                              out HashSet<CDR_Id> References,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingTariff        [optional]

                if (JSON.ParseOptionalJSON("chargingTariff",
                                           "tariff type",
                                           OCPPv2_1.Tariff.TryParse,
                                           out Tariff? Tariff,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingPeriods       [optional]

                if (JSON.ParseOptionalHashSet("chargingPeriods",
                                              "charging periods",
                                              ChargingPeriod.TryParse,
                                              out HashSet<ChargingPeriod> ChargingPeriods,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description           [optional]

                if (JSON.ParseOptionalJSON("description",
                                           "tariff description",
                                           DisplayTexts.TryParse,
                                           out DisplayTexts? Description,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse URL                   [optional]

                if (JSON.ParseOptional("url",
                                       "tariff URL",
                                       org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                       out URL? URL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEIds               [optional]

                if (JSON.ParseOptionalHashSet("evseIds",
                                              "EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> EVSEIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingStationIds    [optional]

                if (JSON.ParseOptionalHashSet("chargingStationIds",
                                              "charging station identifications",
                                              ChargingStation_Id.TryParse,
                                              out HashSet<ChargingStation_Id> ChargingStationIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingPoolIds       [optional]

                if (JSON.ParseOptionalHashSet("chargingPoolIds",
                                              "charging pool identifications",
                                              ChargingPool_Id.TryParse,
                                              out HashSet<ChargingPool_Id> ChargingPoolIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Price                 [optional]

                if (JSON.ParseOptionalJSON("price",
                                           "price",
                                           OCPPv2_1.Price.TryParse,
                                           out Price? Price,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotBefore             [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter              [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPPv2_1.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                  [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                  [optional]

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


            var TotalFixedCost         = new Price(0, 0);
            var TotalReservationCost   = new Price(0, 0);

            var TotalChargingTime      = TimeSpan.Zero;
            var BilledChargingTime     = TimeSpan.Zero;
            var TotalChargingTimeCost  = new Price(0, 0);

            var TotalEnergy            = WattHour.FromKWh(0);
            var BilledEnergy           = WattHour.FromKWh(0);
            var TotalEnergyCost        = new Price(0, 0);

            var TotalParkingTime       = TimeSpan.Zero;
            var BilledParkingTime      = TimeSpan.Zero;
            var TotalParkingCost       = new Price(0, 0);

            var TotalCost              = new Price(0, 0);


                CDR = new CDR(

                          CDRIdBody ?? CDRIdURL!.Value,
                          ProviderId,
                          ProviderName,

                          CSOOperator_Id.Parse("DE*GEF"),
                          GlobalEVSE_Id. Parse("DE*GEF*E12345678*1"),
                          null,
                          null,
                          MeteringValues,

                          TotalFixedCost,
                          TotalReservationCost,

                          TotalTime,
                          BilledTime,
                          TotalTimeCost,

                          TotalChargingTime,
                          BilledChargingTime,
                          TotalChargingTimeCost,

                          TotalEnergy,
                          BilledEnergy,
                          TotalEnergyCost,

                          TotalParkingTime,
                          BilledParkingTime,
                          TotalParkingCost,

                          TotalCost,
                          Currency,

                          Created,
                          Replaces,
                          References,
                          Tariff,
                          ChargingPeriods,

                          null,
                          null,

                          null,
                          null,
                          Signatures,

                          CustomData

                      );

                if (CustomCDRParser is not null)
                    CDR = CustomCDRParser(JSON,
                                          CDR);

                return true;

            }
            catch (Exception e)
            {
                CDR            = default;
                ErrorResponse  = "The given JSON representation of a charge detail record is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDR>?                  CustomTariffSerializer                = null,
                              //CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              //CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              //CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffRestrictionsSerializer    = null,
                              //CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              //CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              //CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                   Id.              ToString()),
                                 //new JProperty("providerId",           ProviderId.      ToString()),
                                 //new JProperty("providerName",         new JArray(ProviderName.     Select(providerName       => providerName.     ToJSON(CustomDisplayTextSerializer)))),
                                 new JProperty("currency",             Currency.        ToString()),

                           Replaces.          Any()
                               ? new JProperty("replaces",             new JArray(Replaces.          Select(chargingTariffId  => chargingTariffId. ToString())))
                               : null,

                           References.        Any()
                               ? new JProperty("references",           new JArray(References.        Select(chargingTariffId  => chargingTariffId. ToString())))
                               : null,



                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.        Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                                                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charge detail record.
        /// </summary>
        public CDR Clone()

            => new (

                   Id.                   Clone(),
                   ProviderId.           Clone(),
                   ProviderName.         Clone(),

                   CSOOperatorId,
                   EVSEId,
                   ChargingStationIds?.Select(chargingStationId => chargingStationId.Clone()).   ToArray(),
                   ChargingPoolIds?.   Select(chargingPoolId    => chargingPoolId.   Clone()).   ToArray(),
                   MeteringValues.     Select(meteringValue     => meteringValue.    Clone()). ToArray(),

                   TotalFixedCost.       Clone(),
                   TotalReservationCost. Clone(),

                   TotalTime,
                   BilledTime,
                   TotalTimeCost.        Clone(),

                   TotalChargingTime,
                   BilledChargingTime,
                   BilledChargingTimeCost.Clone(),

                   TotalEnergy,
                   BilledEnergy,
                   BilledEnergyCost.      Clone(),

                   TotalParkingTime,
                   BilledParkingTime,
                   TotalParkingCost.     Clone(),

                   TotalCost.            Clone(),
                   Currency,

                   Created,
                   Replaces.       Select(cdrId          => cdrId.         Clone()).  ToArray(),
                   References.     Select(cdrId          => cdrId.         Clone()).  ToArray(),
                   ChargingTariff,
                   ChargingPeriods.Select(chargingPeriod => chargingPeriod.Clone()).ToArray(),

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures.    Select(signature     => signature.    Clone()).ToArray(),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR? CDR1,
                                           CDR? CDR2)
        {

            if (Object.ReferenceEquals(CDR1, CDR2))
                return true;

            if (CDR1 is null || CDR2 is null)
                return false;

            return CDR1.Equals(CDR2);

        }

        #endregion

        #region Operator != (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 == CDR2);

        #endregion

        #region Operator <  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR? CDR1,
                                          CDR? CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) < 0;

        #endregion

        #region Operator <= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 > CDR2);

        #endregion

        #region Operator >  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR? CDR1,
                                          CDR? CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) > 0;

        #endregion

        #region Operator >= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 < CDR2);

        #endregion

        #endregion

        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDR chargingTariff
                   ? CompareTo(chargingTariff)
                   : throw new ArgumentException("The given object is not a charge detail record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDR)

        /// <summary>
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="CDR">A charge detail record to compare with.</param>
        public Int32 CompareTo(CDR? CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            var c = Id.         CompareTo(CDR.Id);

            if (c == 0)
                c = Currency.   CompareTo(CDR.Currency);

            //if (c == 0)
            //    c = Created.    CompareTo(Tariff.Created);

            //if (c == 0)
            //    c = LastUpdated.CompareTo(Tariff.LastUpdated);

            // TariffElements
            // 
            // TariffType
            // TariffAltText
            // TariffAltURL
            // MinPrice
            // MaxPrice
            // Start
            // End
            // EnergyMix

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CDR> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDR chargingTariff &&
                   Equals(chargingTariff);

        #endregion

        #region Equals(CDR)

        /// <summary>
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="CDR">A charge detail record to compare with.</param>
        public Boolean Equals(CDR? CDR)

            => CDR is not null &&

               Id.                     Equals(CDR.Id)          &&
               Currency.               Equals(CDR.Currency);

            //((!TariffType.HasValue    && !CDR.TariffType.HasValue) ||
            //  (TariffType.HasValue    &&  CDR.TariffType.HasValue    && TariffType.Value.Equals(CDR.TariffType.Value))) &&

            //((!TariffType.HasValue    && !CDR.TariffType.HasValue) ||
            //  (TariffType.HasValue    &&  CDR.TariffType.HasValue    && TariffType.Value.Equals(CDR.TariffType.Value))) &&

            //((!MinPrice.  HasValue    && !CDR.MinPrice.  HasValue) ||
            //  (MinPrice.  HasValue    &&  CDR.MinPrice.  HasValue    && MinPrice.  Value.Equals(CDR.MinPrice.  Value))) &&

            //((!MaxPrice.  HasValue    && !CDR.MaxPrice.  HasValue) ||
            //  (MaxPrice.  HasValue    &&  CDR.MaxPrice.  HasValue    && MaxPrice.  Value.Equals(CDR.MaxPrice.  Value))) &&

            //NotBefore.     Equals(CDR.NotBefore) &&

            //((!NotAfter.       HasValue    && !CDR.NotAfter.       HasValue) ||
            //  (NotAfter.       HasValue    &&  CDR.NotAfter.       HasValue    && NotAfter.       Value.Equals(CDR.NotAfter.       Value))) &&

            // ((EnergyMix  is     null &&  CDR.EnergyMix  is null)  ||
            //  (EnergyMix  is not null &&  CDR.EnergyMix  is not null && EnergyMix.       Equals(CDR.EnergyMix)))        &&

            //   TariffElements.Count().Equals(CDR.TariffElements.Count())     &&
            //   TariffElements.All(tariffElement => CDR.TariffElements.Contains(tariffElement)) &&

            //   Description.Count().Equals(CDR.Description.Count())     &&
            //   Description.All(displayText => CDR.Description.Contains(displayText));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => hashCode ^
               Signatures.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id

               );

        #endregion


    }

}
