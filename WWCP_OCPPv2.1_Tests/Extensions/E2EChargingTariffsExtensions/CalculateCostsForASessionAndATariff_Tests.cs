/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2EChargingTariffsExtensions
{

    /// <summary>
    /// Calculation of costs for a charging session and a charging tariff tests.
    /// </summary>
    [TestFixture]
    public class CalculateCostsForASessionAndATariff_Tests
    {

        #region SetupOnce()

        [OneTimeSetUp]
        public virtual void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual void ShutdownOnce()
        {

        }

        #endregion


        #region SimpleChargingSessionWithFlatRateTariff()

        /// <summary>
        /// A test for a charging tariff with a flat rate price.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithFlatRateTariff()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         FixedFee:                  new TariffFixed(
                                                                        [ new TariffFixedPrice(42.0M) ],
                                                                        [ TaxRate.VAT(15)]
                                                                    ),
                                         //TariffElements:            new[] {
                                         //                               new TariffElement(
                                         //                                   new[] {
                                         //                                       PriceComponent.FlatRate(
                                         //                                           Price:  42.0M,
                                         //                                           VAT:    23.5M
                                         //                                       )
                                         //                                   }
                                         //                               )
                                         //                           },

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "0.53 / kWh",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingTaxes:  0.51M,
                                                                        IncludingTaxes:  0.53M
                                                                    ),
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               new[] {
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   new[] {
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 },
                                                                                CustomData:      null
                                                                            )
                                                                        },
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:               SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               new[] {
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   new[] {
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 },
                                                                                CustomData:      null
                                                                            )
                                                                        },
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39M,      cdr.TotalTime.        TotalMinutes,   "Total time");
                    ClassicAssert.AreEqual(0M,       cdr.BilledTime.       TotalMinutes,   "Billed time");

                    ClassicAssert.AreEqual(39M,      cdr.TotalChargingTime.TotalMinutes,   "Total charging time");

                    ClassicAssert.AreEqual(9999M,    cdr.TotalEnergy.      Value,          "Total energy");
                    ClassicAssert.AreEqual(0M,       cdr.BilledEnergy.     Value,          "Billed energy");

                    ClassicAssert.AreEqual(42.00M,   cdr.TotalCost.ExcludingTaxes,         "Total cost excl. VAT");
                    ClassicAssert.AreEqual(51.87M,   cdr.TotalCost.IncludingTaxes,         "Total cost incl. VAT");

                }

            }

        }

        #endregion

        #region SimpleChargingSessionWith15MinutesTariff()

        /// <summary>
        /// A test for a charging tariff with a price per 15 minutes.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWith15MinutesTariff()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         ChargingTime:              new TariffTime(
                                                                        [ new TariffTimePrice(6.50M, StepSize: TimeSpan.FromMinutes(15)) ],
                                                                        [ TaxRate.VAT(19)]
                                                                    ),
                                         //TariffElements:            [
                                         //                               new TariffElement(
                                         //                                   [
                                         //                                       PriceComponent.ChargeHours(
                                         //                                           Price:      6.50M,
                                         //                                           VAT:        19,
                                         //                                           StepSize:   TimeSpan.FromMinutes(15)
                                         //                                       )
                                         //                                   ]
                                         //                               )
                                         //                           ],

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "0.53 / kWh",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingTaxes:  0.51M,
                                                                        IncludingTaxes:  0.53M
                                                                    ),
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   new[] {
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 },
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39M,        cdr.TotalTime.             TotalMinutes,   "Total time");
                    ClassicAssert.AreEqual(45M,        cdr.BilledTime.            TotalMinutes,   "Billed time");

                    ClassicAssert.AreEqual(39M,        cdr.TotalChargingTime.     TotalMinutes,   "Total charging time");
                    ClassicAssert.AreEqual(45M,        cdr.BilledChargingTime.    TotalMinutes,   "Billed charging time");
                    ClassicAssert.AreEqual(4.875M,     cdr.BilledChargingTimeCost.ExcludingTaxes,   "Billed charging time cost excl. VAT");
                    ClassicAssert.AreEqual(5.80125M,   cdr.BilledChargingTimeCost.IncludingTaxes,   "Billed charging time cost incl. VAT");

                    ClassicAssert.AreEqual(9999M,      cdr.TotalEnergy.           Value,          "Total energy");
                    ClassicAssert.AreEqual(0M,         cdr.BilledEnergy.          Value,          "Billed energy");
                    ClassicAssert.AreEqual(4.875M,     cdr.BilledEnergyCost.      ExcludingTaxes,   "Billed energy cost excl. VAT");
                    ClassicAssert.AreEqual(5.80125M,   cdr.BilledEnergyCost.      IncludingTaxes,   "Billed energy cost incl. VAT");

                    ClassicAssert.AreEqual(4.875M,     cdr.TotalCost.             ExcludingTaxes,   "Total cost excl. VAT");
                    ClassicAssert.AreEqual(5.80125M,   cdr.TotalCost.             IncludingTaxes,   "Total cost incl. VAT");

                }

            }

        }

        #endregion

        #region SimpleChargingSessionWithKWhTariff()

        /// <summary>
        /// A test for a charging tariff with a simple KWh price.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithKWhTariff()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         Energy:                    new TariffEnergy(
                                                                        [ new TariffEnergyPrice(0.51M) ],
                                                                        [ TaxRate.VAT(19)]
                                                                    ),
                                         //TariffElements:            [
                                         //                               new TariffElement(
                                         //                                   [
                                         //                                       PriceComponent.Energy(
                                         //                                           Price:  0.51M,
                                         //                                           VAT:    19
                                         //                                       )
                                         //                                   ]
                                         //                               )
                                         //                           ],

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "0.53 / kWh",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingTaxes:  0.51M,
                                                                        IncludingTaxes:  0.53M
                                                                    ),
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39M,      cdr.TotalTime.        TotalMinutes,   "Total time");
                    ClassicAssert.AreEqual(0M,       cdr.BilledTime.       TotalMinutes,   "Billed time");

                    ClassicAssert.AreEqual(39M,      cdr.TotalChargingTime.TotalMinutes,   "Total charging time");

                    ClassicAssert.AreEqual(9999M,    cdr.TotalEnergy.      Value,          "Total energy");
                    ClassicAssert.AreEqual(10000M,   cdr.BilledEnergy.     Value,          "Billed energy");

                    ClassicAssert.AreEqual(5.1M,     cdr.TotalCost.ExcludingTaxes,           "Total cost excl. VAT");
                    ClassicAssert.AreEqual(6.069M,   cdr.TotalCost.IncludingTaxes,           "Total cost incl. VAT");

                }

            }

        }

        #endregion

        #region SimpleChargingSessionWithKWhTariffWithIdleFee()

        /// <summary>
        /// A test for a charging tariff with a higher price during office hours (8-18h) and
        /// an idle fee for staying connected without charging.
        /// During evening and night there is a lower price without an idle fee.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithKWhTariffWithIdleFee()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         Energy:                    new TariffEnergy(
                                                                        [
                                                                            new TariffEnergyPrice(
                                                                                PriceKWh:     0.40M,
                                                                                Conditions:   new TariffConditions(
                                                                                                  StartTimeOfDay:   Time.FromHourMin( 8, 0),
                                                                                                  EndTimeOfDay:     Time.FromHourMin(18, 0)
                                                                                              )
                                                                            ),
                                                                            new TariffEnergyPrice(
                                                                                PriceKWh:     0.25M,
                                                                                Conditions:   new TariffConditions(
                                                                                                  StartTimeOfDay:   Time.FromHourMin(18, 0),
                                                                                                  EndTimeOfDay:     Time.FromHourMin( 8, 0)
                                                                                              )
                                                                            )
                                                                        ],
                                                                        [ TaxRate.VAT(10)]
                                                                    ),
                                         IdleTime:                  new TariffTime(
                                                                        [
                                                                            new TariffTimePrice(
                                                                                PriceMinute:  10.00M,
                                                                                Conditions:   new TariffConditions(
                                                                                                  StartTimeOfDay:   Time.FromHourMin( 8, 0),
                                                                                                  EndTimeOfDay:     Time.FromHourMin(18, 0)
                                                                                              )
                                                                            )
                                                                        ],
                                                                        [ TaxRate.VAT(10)]
                                                                    ),


                                         //TariffElements:            [

                                                                        //new TariffElement(
                                                                        //    [
                                                                        //        PriceComponent.Energy(
                                                                        //            Price:       0.40M,
                                                                        //            TaxRates:   TaxRates.VAT(10)
                                                                        //        ),
                                                                        //        PriceComponent.IdleHours(
                                                                        //            Price:      10.00M,
                                                                        //            TaxRates:   TaxRates.VAT(10)
                                                                        //        )
                                                                        //    ],
                                                                        //    new TariffConditions(
                                                                        //        StartTimeOfDay:   Time.FromHourMin( 8, 0),
                                                                        //        EndTimeOfDay:     Time.FromHourMin(18, 0)
                                                                        //    )
                                                                        //),

                                                                    //    new TariffElement(
                                                                    //        [
                                                                    //            PriceComponent.Energy(
                                                                    //                Price:      0.25M,
                                                                    //                TaxRates:   TaxRates.VAT(10)
                                                                    //            )
                                                                    //        ],
                                                                    //        new TariffConditions(
                                                                    //            StartTimeOfDay:   Time.FromHourMin(18, 0),
                                                                    //            EndTimeOfDay:     Time.FromHourMin( 8, 0)
                                                                    //        )
                                                                    //    )

                                                                    //],

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "08:00h - 18:00h: 0.44 ct/kWh (idle fee 11 EUR/hr), 18.00h - 08.00h: 0.275 ct/kWh. Price incl. VAT",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  null,
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    ClassicAssert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    ClassicAssert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    ClassicAssert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    ClassicAssert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    ClassicAssert.AreEqual(5.1,     cdr.TotalCost.ExcludingTaxes);
                    ClassicAssert.AreEqual(5.3,     cdr.TotalCost.IncludingTaxes);

                }

            }

        }

        #endregion

        #region SimpleChargingSessionWithComplexHourlyTariff()

        /// <summary>
        /// A test for a charging tariff with a time-based tariff, that includes
        /// a start fee and an hourly fee that depends time of the day and day of the week.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithComplexHourlyTariff()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         FixedFee:                  new TariffFixed(
                                                                        [ new TariffFixedPrice(2.50M) ],
                                                                        [ TaxRate.VAT(15) ]
                                                                    ),
                                         ChargingTime:              new TariffTime(
                                                                        [
                                                                            new TariffTimePrice(
                                                                                PriceMinute:  1.00M,
                                                                                StepSize:     TimeSpan.FromSeconds(900),
                                                                                Conditions:   new TariffConditions(
                                                                                                  MinEnergy:  WattHour.ParseKWh(11)
                                                                                              )
                                                                            ),
                                                                            new TariffTimePrice(
                                                                                PriceMinute:  2.00M,
                                                                                StepSize:     TimeSpan.FromSeconds(600),
                                                                                Conditions:   new TariffConditions(
                                                                                                  MinEnergy:  WattHour.ParseKWh(11)
                                                                                              )
                                                                            )
                                                                        ],
                                                                        [ TaxRate.VAT(20) ]
                                                                    ),
                                         IdleTime:                  new TariffTime(
                                                                        [
                                                                            new TariffTimePrice(
                                                                                PriceMinute:  5.00M,
                                                                                StepSize:     TimeSpan.FromSeconds(300),
                                                                                Conditions:   new TariffConditions(
                                                                                                  StartTimeOfDay:   Time.FromHourMin( 9,00),
                                                                                                  EndTimeOfDay:     Time.FromHourMin(18,00),
                                                                                                  DaysOfWeek:       [
                                                                                                                        DayOfWeek.Monday,
                                                                                                                        DayOfWeek.Tuesday,
                                                                                                                        DayOfWeek.Wednesday,
                                                                                                                        DayOfWeek.Thursday,
                                                                                                                        DayOfWeek.Friday
                                                                                                                    ]
                                                                                              )
                                                                            )
                                                                        ],
                                                                        [ TaxRate.VAT(10) ]
                                                                    ),

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "Complex tariff",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  null,
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    ClassicAssert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    ClassicAssert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    ClassicAssert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    ClassicAssert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    ClassicAssert.AreEqual(5.1,     cdr.TotalCost.ExcludingTaxes);
                    ClassicAssert.AreEqual(5.3,     cdr.TotalCost.IncludingTaxes);

                }

            }

        }

        #endregion

        #region SimpleChargingSessionWithKWhTariffAndDelayedIdleFee()

        /// <summary>
        /// A test for a charging tariff with a simple KWh price and a
        /// delayed idle fee that starts after 2 hours of idle time.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithKWhTariffAndDelayedIdleFee()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new Tariff(

                                         Id:                        Tariff_Id.Parse("DE-GDF-T12345678"),
                                         //ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         //ProviderName:              new DisplayTexts(
                                         //                               Languages.en,
                                         //                               "GraphDefined EMP"
                                         //                           ),
                                         Currency:                  Currency.EUR,
                                         Energy:                    new TariffEnergy(
                                                                        [ new TariffEnergyPrice(0.40M) ],
                                                                        [ TaxRate.VAT(10) ]
                                                                    ),
                                         IdleTime:                  new TariffTime(
                                                                        [
                                                                            new TariffTimePrice(
                                                                                PriceMinute:  10.00M,
                                                                                Conditions:   new TariffConditions(
                                                                                                  MinIdleTime: TimeSpan.FromHours(2)
                                                                                              )
                                                                            )
                                                                        ],
                                                                        [ TaxRate.VAT(10) ]
                                                                    ),
                                         //TariffElements:            [

                                         //                               new TariffElement(
                                         //                                   [
                                         //                                       PriceComponent.Energy(
                                         //                                           Price:  0.40M,
                                         //                                           VAT:    10
                                         //                                       )
                                         //                                   ]
                                         //                               ),

                                         //                               new TariffElement(
                                         //                                   [
                                         //                                       PriceComponent.IdleHours(
                                         //                                           Price:  10.00M,
                                         //                                           VAT:    10
                                         //                                       )
                                         //                                   ],
                                         //                                   new TariffConditions(
                                         //                                       MinIdleHours:   TimeSpan.FromHours(2)
                                         //                                   )
                                         //                               )

                                         //                           ],

                                         //Created:                   timeReference,
                                         //Replaces:                  null,
                                         //References:                null,
                                         //TariffType:                TariffType.REGULAR,
                                         Description:               new MessageContents(
                                                                        "0.44 ct/kWh incl. VAT and an idle fee of 11 EUR/hr after 2 hours.",
                                                                        Language_Id.EN
                                                                    ),
                                         //URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         //EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingTaxes:  0.51M,
                                                                        IncludingTaxes:  0.53M
                                                                    ),
                                         //NotBefore:                 timeReference,
                                         //NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            ClassicAssert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = NetworkingNode_Id.Parse("cp001");

            ClassicAssert.IsNotNull(chargingStationId);
            ClassicAssert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenType.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            ClassicAssert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(10),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 1,
                                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         ),

                                         new CS.TransactionEventRequest(

                                             Destination:    SourceRouting.To(        chargingStationId),

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReason.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReason.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationMode.ChargingOnly,
                                                                            CustomData:                null
                                                                        ),

                                             Offline:                   null,
                                             NumberOfPhasesUsed:        null,
                                             CableMaxCurrent:           null,
                                             ReservationId:             null,
                                             IdToken:                   idToken,
                                             EVSE:                      evse,
                                             MeterValues:               [
                                                                            new MeterValue(
                                                                                Timestamp:       timeReference + TimeSpan.FromMinutes(49),
                                                                                SampledValues:   [
                                                                                                     new SampledValue(
                                                                                                         Value:                 10000,
                                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                                         Measurand:             Measurand.Current_Import_Offered,
                                                                                                         Phase:                 null,
                                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                                    SignedMeterData:   "",
                                                                                                                                    SigningMethod:     "",
                                                                                                                                    EncodingMethod:    "",
                                                                                                                                    PublicKey:         "",
                                                                                                                                    CustomData:        null
                                                                                                                                ),
                                                                                                         UnitOfMeasure:         UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:            null
                                                                                                     )
                                                                                                 ],
                                                                                CustomData:      null
                                                                            )
                                                                        ],
                                             PreconditioningStatus:     null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null,

                                             RequestId:                 null,
                                             RequestTimestamp:          null,
                                             RequestTimeout:            null,
                                             EventTrackingId:           null

                                         )

                                     };

            ClassicAssert.IsNotNull(transactionEvents);
            ClassicAssert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                ClassicAssert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurand.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocation.Outlet

                              ),
                              errorString);
                ClassicAssert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    ClassicAssert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    ClassicAssert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    ClassicAssert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    ClassicAssert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    ClassicAssert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    ClassicAssert.AreEqual(5.1,     cdr.TotalCost.ExcludingTaxes);
                    ClassicAssert.AreEqual(5.3,     cdr.TotalCost.IncludingTaxes);

                }

            }

        }

        #endregion




    }

}
