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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;

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
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cdr");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the charge detail record.
        /// </summary>
        [Mandatory]
        public   CDR_Id                         Id                       { get; }

        /// <summary>
        /// The timestamp when this tariff was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public   DateTime                       Created                  { get; }

        /// <summary>
        /// Optional references to other tariffs, which will be replaced by this charge detail record.
        /// </summary>
        [Optional]
        public  IEnumerable<CDR_Id>             Replaces                 { get; }

        /// <summary>
        /// Optional references to other tariffs, e.g. because some local adaption of a charge detail record was required.
        /// </summary>
        [Optional]
        public IEnumerable<CDR_Id>              References               { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   Provider_Id                    ProviderId               { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   DisplayTexts                   ProviderName             { get; }

        /// <summary>
        /// The charging station operator identification.
        /// </summary>
        [Mandatory]
        public CSOOperator_Id                   CSOOperatorId            { get; }

        /// <summary>
        /// The EVSE identification.
        /// </summary>
        [Mandatory]
        public GlobalEVSE_Id                    EVSEId                   { get; }

        /// <summary>
        /// An optional enumeration of charging station identifications, this tariff is valid for.
        /// </summary>
        [Optional]
        public ChargingStation_Id?              ChargingStationId        { get; }

        /// <summary>
        /// An optional enumeration of charging pool identifications, this tariff is valid for.
        /// </summary>
        [Optional]
        public ChargingPool_Id?                 ChargingPoolId           { get; }

        /// <summary>
        /// The optional charge detail record.
        /// </summary>
        [Optional]
        public   ChargingTariff?                ChargingTariff           { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public   Price?                         Price                    { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Optional]
        public   Currency                       Currency                 { get; }


        public   IEnumerable<MeteringValue>     MeteringValues           { get; }

        public   IEnumerable<ChargingPeriod>    ChargingPeriods          { get; }




        public   Price                          TotalFixedCost           { get; }
        public   Price                          TotalReservationCost     { get; }

        public   TimeSpan                       TotalTime                { get; }
        public   TimeSpan                       BilledTime               { get; }
        public   Price                          TotalTimeCost            { get; }


        public   TimeSpan                       TotalChargingTime        { get; }
        public   TimeSpan                       BilledChargingTime       { get; }
        public   Price                          BilledChargingTimeCost    { get; }


        public   WattHour                       TotalEnergy              { get; }
        public   WattHour                       BilledEnergy             { get; }
        public   Price                          BilledEnergyCost          { get; }


        public   TimeSpan                       TotalParkingTime         { get; }
        public   TimeSpan                       BilledParkingTime        { get; }
        public   Price                          TotalParkingCost         { get; }


        public   Price                          TotalCost                { get; }

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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CDR(CDR_Id                        Id,

                   Provider_Id                   ProviderId,
                   DisplayTexts                  ProviderName,

                   CSOOperator_Id                CSOOperatorId,
                   GlobalEVSE_Id                 EVSEId,
                   ChargingStation_Id?           ChargingStationId,
                   ChargingPool_Id?              ChargingPoolId,
                   IEnumerable<MeteringValue>    MeteringValues,

                   Price                         TotalFixedCost,
                   Price                         TotalReservationCost,

                   TimeSpan                      TotalTime,
                   TimeSpan                      BilledTime,
                   Price                         TotalTimeCost,

                   TimeSpan                      TotalChargingTime,
                   TimeSpan                      BilledChargingTime,
                   Price                         TotalChargingTimeCost,

                   WattHour                      TotalEnergy,
                   WattHour                      BilledEnergy,
                   Price                         TotalEnergyCost,

                   TimeSpan                      TotalParkingTime,
                   TimeSpan                      BilledParkingTime,
                   Price                         TotalParkingCost,

                   Price                         TotalCost,
                   Currency                      Currency,

                   DateTime?                     Created              = null,
                   IEnumerable<CDR_Id>?          Replaces             = null,
                   IEnumerable<CDR_Id>?          References           = null,
                   ChargingTariff?               ChargingTariff       = null,
                   IEnumerable<ChargingPeriod>?  ChargingPeriods      = null,

                   DisplayTexts?                 Description          = null,
                   URL?                          URL                  = null,

                   IEnumerable<KeyPair>?         SignKeys             = null,
                   IEnumerable<SignInfo>?        SignInfos            = null,
                   IEnumerable<Signature>?       Signatures           = null,

                   CustomData?                   CustomData           = null)

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
            this.ChargingStationId      = ChargingStationId;
            this.ChargingPoolId         = ChargingPoolId;
            this.MeteringValues         = MeteringValues.  Distinct();
            this.ChargingPeriods        = ChargingPeriods?.Distinct() ?? Array.Empty<ChargingPeriod>();

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
            this.Replaces               = Replaces?.  Distinct() ?? Array.Empty<CDR_Id>();
            this.References             = References?.Distinct() ?? Array.Empty<CDR_Id>();
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


        public static Boolean CalculateCosts(Provider_Id              ProviderId,
                                             DisplayTexts             ProviderName,
                                             CSOOperator_Id           CSOOperatorId,
                                             GlobalEVSE_Id            EVSEId,
                                             IEnumerable<MeterValue>  MeterValues,
                                             ChargingTariff           ChargingTariff,
                                             out CDR?                 CDR,
                                             out String?              ErrorResponse,

                                             ChargingStation_Id?      ChargingStationId     = null,
                                             ChargingPool_Id?         ChargingPoolId        = null,
                                             Measurand?               Measurand             = null,
                                             MeasurementLocation?     MeasurementLocation   = null)
        {

            CDR                     = null;
            ErrorResponse           = null;
            Measurand             ??= OCPPv2_1.Measurand.Current_Import_Offered;
            MeasurementLocation   ??= OCPPv2_1.MeasurementLocation.Outlet;

            var meterValues         = MeterValues.OrderBy(meterValue => meterValue.Timestamp).ToArray();

            #region Get Start Metering Value

            var startMeterValues    = meterValues.Where(meterValue => meterValue.SampledValues.Any(sampledValue => sampledValue.Measurand == Measurand           &&
                                                                                                                   sampledValue.MeasurementLocation  == MeasurementLocation &&
                                                                                                                   sampledValue.Context   == ReadingContexts.TransactionBegin)).ToArray();
            if (startMeterValues.Length != 1)
            {
                ErrorResponse = startMeterValues.Length == 0
                                    ? "No 'TransactionBegin' meter value found!"
                                    : "More than one 'TransactionBegin' meter value found!";
                return false;
            }

            var startSampledValues  = startMeterValues.First().SampledValues.Where(sampledValue => sampledValue.Measurand == Measurand           &&
                                                                                                   sampledValue.MeasurementLocation  == MeasurementLocation &&
                                                                                                   sampledValue.Context   == ReadingContexts.TransactionBegin).ToArray();
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

            var stopMeterValues     = meterValues.Where(meterValue => meterValue.SampledValues.Any(sampledValue => sampledValue.Measurand == Measurand           &&
                                                                                                                   sampledValue.MeasurementLocation  == MeasurementLocation &&
                                                                                                                   sampledValue.Context   == ReadingContexts.TransactionEnd)).ToArray();
            if (stopMeterValues.Length != 1)
            {
                ErrorResponse = stopMeterValues.Length == 0
                                    ? "No 'TransactionEnd' meter value found!"
                                    : "More than one 'TransactionEnd' meter value found!";
                return false;
            }

            var stopSampledValues   = stopMeterValues.First().SampledValues.Where(sampledValue => sampledValue.Measurand == Measurand           &&
                                                                                                  sampledValue.MeasurementLocation  == MeasurementLocation &&
                                                                                                  sampledValue.Context   == ReadingContexts.TransactionEnd).ToArray();
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

            #region Calculate TotalTime

            var totalTime = totalChargingTime;

            #endregion

            #region Calculate TotalTimeCost

            var totalTimeCost = new Price(0);

            #endregion

            #region Calculate TotalEnergy

            var totalEnergy = WattHour.Parse(stopMeteringValue.Value - startMeteringValue.Value);

            #endregion


            if (!ChargingTariff.TariffElements.Any())
            {
                ErrorResponse = "No charge detail record elements found!";
                return false;
            }

            #region Get first matching charge detail record element

            var tariffElement               = ChargingTariff.TariffElements.First();

            #endregion


            #region Calculate FlatPrice

            var flatPriceComponent          = tariffElement.PriceComponents.FirstOrDefault(priceComponent => priceComponent.Type == TariffDimension.FLAT);
            var flatPrice                   = flatPriceComponent?.Price;
            var flatVAT                     = flatPriceComponent?.TaxRates.Get("VAT", AppliesToMinimumMaximumCost: true)?.Tax;

            var totalFixedCost              = flatPrice.HasValue
                                                  ? new Price(
                                                        ExcludingVAT:  flatPrice.Value,
                                                        IncludingVAT:  flatPrice.Value + flatPrice.Value * (flatVAT ?? 0) / 100
                                                    )
                                                  : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate BilledChargingTime

            var chargingTimePriceComponent  = tariffElement.PriceComponents.FirstOrDefault(priceComponent => priceComponent.Type == TariffDimension.CHARGE_HOURS);
            var chargingTimeStepSize        = chargingTimePriceComponent?.StepSize ?? 1;
            var chargingTimePrice           = chargingTimePriceComponent?.Price;
            var chargingTimeVAT             = chargingTimePriceComponent?.TaxRates.Get("VAT", AppliesToEnergyFee: true)?.Tax;

            var billedChargingTimeSteps     = Math.Ceiling(totalChargingTime.TotalSeconds / chargingTimeStepSize);
            var billedChargingTime          = chargingTimePriceComponent is not null
                                                  ? TimeSpan.FromSeconds(billedChargingTimeSteps * chargingTimeStepSize)
                                                  : TimeSpan.Zero;
            var totalChargingTimeCost       = chargingTimePrice.HasValue
                                                  ? new Price(
                                                        ExcludingVAT:  ((Decimal) billedChargingTime.TotalSeconds) / 3600 *  chargingTimePrice.Value,
                                                        IncludingVAT:  ((Decimal) billedChargingTime.TotalSeconds) / 3600 * (chargingTimePrice.Value + chargingTimePrice.Value * (chargingTimeVAT ?? 0) / 100)
                                                    )
                                                  : OCPPv2_1.Price.Zero;

            #endregion

            #region Calculate BilledTime

            var timePriceComponent          = tariffElement.PriceComponents.FirstOrDefault(priceComponent => priceComponent.Type == TariffDimension.CHARGE_HOURS);
            var timeStepSize                = timePriceComponent?.StepSize ?? 1;
            var timePrice                   = timePriceComponent?.Price;
            var timeVAT                     = timePriceComponent?.TaxRates.Get("VAT", AppliesToMinimumMaximumCost: true)?.Tax;

            var billedTimeSteps             = Math.Ceiling(totalTime.TotalSeconds / timeStepSize);
            var billedTime                  = timePriceComponent is not null
                                                  ? TimeSpan.FromSeconds(billedTimeSteps * timeStepSize)
                                                  : TimeSpan.Zero;

            #endregion

            #region Calculate BilledEnergy

            var energyPriceComponent        = tariffElement.PriceComponents.FirstOrDefault(priceComponent => priceComponent.Type == TariffDimension.ENERGY);
            var energyStepSize              = energyPriceComponent?.StepSize ?? 1;
            var energyPrice                 = energyPriceComponent?.Price;
            var energyVAT                   = energyPriceComponent?.TaxRates.Get("VAT", AppliesToMinimumMaximumCost: true)?.Tax;

            var billedEnergySteps           = Math.Ceiling(totalEnergy.Value / energyStepSize);
            var billedEnergy                = energyPriceComponent is not null
                                                  ? WattHour.Parse(billedEnergySteps * energyStepSize)
                                                  : WattHour.Zero;
            var totalEnergyCost             = energyPrice.HasValue
                                                  ? new Price(
                                                        ExcludingVAT:  billedEnergy.Value / 1000 *  energyPrice.Value,
                                                        IncludingVAT:  billedEnergy.Value / 1000 * (energyPrice.Value + energyPrice.Value * (energyVAT ?? 0) / 100)
                                                    )
                                                  : OCPPv2_1.Price.Zero;

            #endregion


            var totalReservationCost        = new Price(0);

            var totalParkingTime            = TimeSpan.Zero;
            var billedParkingTime           = TimeSpan.Zero;
            var totalParkingCost            = new Price(0);

            var totalCost                   = totalFixedCost + totalReservationCost + totalChargingTimeCost + totalEnergyCost + totalParkingCost + totalTimeCost;
            var currency                    = ChargingTariff.Currency;


            CDR = new CDR(

                      CDR_Id.NewRandom(ProviderId),

                      ProviderId,
                      ProviderName,

                      CSOOperatorId,
                      EVSEId,
                      ChargingStationId,
                      ChargingPoolId,
                      new[] {
                          startMeteringValue,
                          stopMeteringValue
                      },

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

                      totalParkingTime,
                      billedParkingTime,
                      totalParkingCost,

                      totalCost,
                      currency

                  );

            return true;

        }


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
                         out var tariff,
                         out var errorResponse,
                         CDRIdURL,
                         CustomCDRParser))
            {
                return tariff!;
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
                                       CDR_Id?                            CDRIdURL       = null,
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
                                           OCPPv2_1.ChargingTariff.TryParse,
                                           out ChargingTariff? ChargingTariff,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


            var TotalFixedCost         = new Price(0);
            var TotalReservationCost   = new Price(0);

            var TotalChargingTime      = TimeSpan.Zero;
            var BilledChargingTime     = TimeSpan.Zero;
            var TotalChargingTimeCost  = new Price(0);

            var TotalEnergy            = WattHour.Parse(0);
            var BilledEnergy           = WattHour.Parse(0);
            var TotalEnergyCost        = new Price(0);

            var TotalParkingTime       = TimeSpan.Zero;
            var BilledParkingTime      = TimeSpan.Zero;
            var TotalParkingCost       = new Price(0);

            var TotalCost              = new Price(0);


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
                          ChargingTariff,
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
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                   Id.              ToString()),
                                 new JProperty("providerId",           ProviderId.      ToString()),
                                 new JProperty("providerName",         new JArray(ProviderName.     Select(providerName       => providerName.     ToJSON(CustomDisplayTextSerializer)))),
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

                   Id.                   Clone,
                   ProviderId.           Clone,
                   ProviderName.         Clone(),

                   CSOOperatorId,
                   EVSEId,
                   ChargingStationId,
                   ChargingPoolId,
                   MeteringValues. Select(meteringValue  => meteringValue.Clone()). ToArray(),

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
                   Replaces.       Select(cdrId          => cdrId.         Clone).  ToArray(),
                   References.     Select(cdrId          => cdrId.         Clone).  ToArray(),
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
