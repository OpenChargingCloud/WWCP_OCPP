/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.E2EChargingTariffsExtensions
{

    /// <summary>
    /// Calculation of costs based on a transaction and a charging tariff tests.
    /// </summary>
    [TestFixture]
    public class CostsBasedOnATransactionAndATariff_Tests
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


        #region SimpleChargingSessionWithKWhTariff()

        /// <summary>
        /// A test for a charging tariff with a simple KWh price.
        /// </summary>
        [Test]
        public void SimpleChargingSessionWithKWhTariff()
        {

            var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

            #region Define a  charging tariff

            var chargingTariff     = new ChargingTariff(

                                         Id:                        ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                         ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         ProviderName:              new DisplayTexts(
                                                                        Languages.en,
                                                                        "GraphDefined EMP"
                                                                    ),
                                         Currency:                  Currency.EUR,
                                         TariffElements:            new[] {
                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.Energy(
                                                                                    Price:  0.51M,
                                                                                    VAT:    19
                                                                                )
                                                                            }
                                                                        )
                                                                    },

                                         Created:                   timeReference,
                                         Replaces:                  null,
                                         References:                null,
                                         TariffType:                TariffType.REGULAR,
                                         Description:               new DisplayTexts(
                                                                        Languages.en,
                                                                        "0.53 / kWh"
                                                                    ),
                                         URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingVAT:  0.51M,
                                                                        IncludingVAT:  0.53M
                                                                    ),
                                         NotBefore:                 timeReference,
                                         NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            Assert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = ChargingStation_Id.Parse("cp001");

            Assert.IsNotNull(chargingStationId);
            Assert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            Assert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenTypes.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            Assert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              1,
                                                                                                         Context:            ReadingContexts.TransactionBegin,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReasons.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              10000,
                                                                                                         Context:            ReadingContexts.TransactionEnd,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

            Assert.IsNotNull(transactionEvents);
            Assert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                Assert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurands.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocations.Outlet

                              ),
                              errorString);
                Assert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    Assert.AreEqual(39M,      cdr.TotalTime.        TotalMinutes,   "Total time");
                    Assert.AreEqual(39M,      cdr.BilledTime.       TotalMinutes,   "Billed time");

                    Assert.AreEqual(39M,      cdr.TotalChargingTime.TotalMinutes,   "Total charging time");

                    Assert.AreEqual(9999M,    cdr.TotalEnergy.      Value,          "Total energy");
                    Assert.AreEqual(10000M,   cdr.BilledEnergy.     Value,          "Billed energy");

                    Assert.AreEqual(5.1M,     cdr.TotalCost.ExcludingVAT,           "Total cost excl. VAT");
                    Assert.AreEqual(6.069M,   cdr.TotalCost.IncludingVAT,           "Total cost incl. VAT");

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

            var chargingTariff     = new ChargingTariff(

                                         Id:                        ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                         ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         ProviderName:              new DisplayTexts(
                                                                        Languages.en,
                                                                        "GraphDefined EMP"
                                                                    ),
                                         Currency:                  Currency.EUR,
                                         TariffElements:            new[] {

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.Energy(
                                                                                    Price:       0.40M,
                                                                                    TaxRates:   TaxRates.VAT(10)
                                                                                ),
                                                                                PriceComponent.IdleHours(
                                                                                    Price:      10.00M,
                                                                                    TaxRates:   TaxRates.VAT(10)
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                StartTimeOfDay:   Time.FromHourMin( 8, 0),
                                                                                EndTimeOfDay:     Time.FromHourMin(18, 0)
                                                                            )
                                                                        ),

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.Energy(
                                                                                    Price:      0.25M,
                                                                                    TaxRates:   TaxRates.VAT(10)
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                StartTimeOfDay:   Time.FromHourMin(18, 0),
                                                                                EndTimeOfDay:     Time.FromHourMin( 8, 0)
                                                                            )
                                                                        )

                                                                    },

                                         Created:                   timeReference,
                                         Replaces:                  null,
                                         References:                null,
                                         TariffType:                TariffType.REGULAR,
                                         Description:               new DisplayTexts(
                                                                        Languages.en,
                                                                        "08:00h - 18:00h: 0.44 ct/kWh (idle fee 11 EUR/hr), 18.00h - 08.00h: 0.275 ct/kWh. Price incl. VAT"
                                                                    ),
                                         URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  null,
                                         NotBefore:                 timeReference,
                                         NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            Assert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = ChargingStation_Id.Parse("cp001");

            Assert.IsNotNull(chargingStationId);
            Assert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            Assert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenTypes.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            Assert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              1,
                                                                                                         Context:            ReadingContexts.TransactionBegin,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReasons.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              10000,
                                                                                                         Context:            ReadingContexts.TransactionEnd,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

            Assert.IsNotNull(transactionEvents);
            Assert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                Assert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurands.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocations.Outlet

                              ),
                              errorString);
                Assert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    Assert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    Assert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    Assert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    Assert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    Assert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    Assert.AreEqual(5.1,     cdr.TotalCost.ExcludingVAT);
                    Assert.AreEqual(5.3,     cdr.TotalCost.IncludingVAT);

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

            var chargingTariff     = new ChargingTariff(

                                         Id:                        ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                         ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         ProviderName:              new DisplayTexts(
                                                                        Languages.en,
                                                                        "GraphDefined EMP"
                                                                    ),
                                         Currency:                  Currency.EUR,
                                         TariffElements:            new[] {

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.FlatRate(
                                                                                    Price:      2.50M,
                                                                                    TaxRates:   TaxRates.VAT(15)
                                                                                )
                                                                            }
                                                                        ),

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.ChargeHours(
                                                                                    Price:      1.00M,
                                                                                    TaxRates:   TaxRates.VAT(20),
                                                                                    StepSize:   TimeSpan.FromSeconds(900)
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                MinEnergy:   WattHour.Parse(11)
                                                                            )
                                                                        ),

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.ChargeHours(
                                                                                    Price:      2.00M,
                                                                                    TaxRates:   TaxRates.VAT(20),
                                                                                    StepSize:   TimeSpan.FromSeconds(600)
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                MinEnergy:   WattHour.Parse(11)
                                                                            )
                                                                        ),

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.IdleHours(
                                                                                    Price:      5.00M,
                                                                                    TaxRates:   TaxRates.VAT(10),
                                                                                    StepSize:   TimeSpan.FromSeconds(300)
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                StartTimeOfDay:   Time.FromHourMin( 9,00),
                                                                                EndTimeOfDay:     Time.FromHourMin(18,00),
                                                                                DaysOfWeek:       new[] {
                                                                                                      DayOfWeek.Monday,
                                                                                                      DayOfWeek.Tuesday,
                                                                                                      DayOfWeek.Wednesday,
                                                                                                      DayOfWeek.Thursday,
                                                                                                      DayOfWeek.Friday
                                                                                                  }
                                                                            )
                                                                        )

                                                                    },

                                         Created:                   timeReference,
                                         Replaces:                  null,
                                         References:                null,
                                         TariffType:                TariffType.REGULAR,
                                         Description:               new DisplayTexts(
                                                                        Languages.en,
                                                                        "Complex tariff"
                                                                    ),
                                         URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  null,
                                         NotBefore:                 timeReference,
                                         NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            Assert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = ChargingStation_Id.Parse("cp001");

            Assert.IsNotNull(chargingStationId);
            Assert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            Assert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenTypes.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            Assert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              1,
                                                                                                         Context:            ReadingContexts.TransactionBegin,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReasons.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              10000,
                                                                                                         Context:            ReadingContexts.TransactionEnd,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

            Assert.IsNotNull(transactionEvents);
            Assert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                Assert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurands.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocations.Outlet

                              ),
                              errorString);
                Assert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    Assert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    Assert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    Assert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    Assert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    Assert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    Assert.AreEqual(5.1,     cdr.TotalCost.ExcludingVAT);
                    Assert.AreEqual(5.3,     cdr.TotalCost.IncludingVAT);

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

            var chargingTariff     = new ChargingTariff(

                                         Id:                        ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                         ProviderId:                Provider_Id.      Parse("DE-GDF"),
                                         ProviderName:              new DisplayTexts(
                                                                        Languages.en,
                                                                        "GraphDefined EMP"
                                                                    ),
                                         Currency:                  Currency.EUR,
                                         TariffElements:            new[] {

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.Energy(
                                                                                    Price:  0.40M,
                                                                                    VAT:    10
                                                                                )
                                                                            }
                                                                        ),

                                                                        new TariffElement(
                                                                            new[] {
                                                                                PriceComponent.IdleHours(
                                                                                    Price:  10.00M,
                                                                                    VAT:    10
                                                                                )
                                                                            },
                                                                            new TariffRestrictions(
                                                                                MinIdleHours:   TimeSpan.FromHours(2)
                                                                            )
                                                                        )

                                                                    },

                                         Created:                   timeReference,
                                         Replaces:                  null,
                                         References:                null,
                                         TariffType:                TariffType.REGULAR,
                                         Description:               new DisplayTexts(
                                                                        Languages.en,
                                                                        "0.44 ct/kWh incl. VAT and an idle fee of 11 EUR/hr after 2 hours."
                                                                    ),
                                         URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                         EnergyMix:                 null,

                                         MinPrice:                  null,
                                         MaxPrice:                  new Price(
                                                                        ExcludingVAT:  0.51M,
                                                                        IncludingVAT:  0.53M
                                                                    ),
                                         NotBefore:                 timeReference,
                                         NotAfter:                  null,

                                         SignKeys:                  null,
                                         SignInfos:                 null,
                                         Signatures:                null,

                                         CustomData:                null

                                     );

            Assert.IsNotNull(chargingTariff);

            #endregion

            #region Define a  charging station identification

            var chargingStationId  = ChargingStation_Id.Parse("cp001");

            Assert.IsNotNull(chargingStationId);
            Assert.IsFalse  (chargingStationId.IsNullOrEmpty);

            #endregion

            #region Define an EVSE

            var evse               = new EVSE(
                                         Id:                        EVSE_Id.     Parse("1"),
                                         ConnectorId:               Connector_Id.Parse("1"),
                                         CustomData:                null
                                     );

            Assert.IsNotNull(evse);

            #endregion

            #region Define an IdToken

            var idToken            = new IdToken(
                                         Value:                     "",
                                         Type:                      IdTokenTypes.ISO14443,
                                         AdditionalInfos:           null,
                                         CustomData:                null
                                     );

            Assert.IsNotNull(idToken);

            #endregion

            #region Define    transaction events

            var transactionId      = Transaction_Id.Parse("DEGEFE12345678");

            var transactionEvents  = new[] {

                                         new CS.TransactionEventRequest(

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Started,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(10),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            1,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.EVConnected,
                                                                            TimeSpentCharging:         TimeSpan.Zero,
                                                                            StoppedReason:             null,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              1,
                                                                                                         Context:            ReadingContexts.TransactionBegin,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

                                             ChargingStationId:         chargingStationId,

                                             EventType:                 TransactionEvents.Ended,
                                             Timestamp:                 timeReference + TimeSpan.FromMinutes(49),
                                             TriggerReason:             TriggerReasons.ChargingStateChanged,
                                             SequenceNumber:            2,
                                             TransactionInfo:           new Transaction(
                                                                            TransactionId:             transactionId,
                                                                            ChargingState:             ChargingStates.SuspendedEVSE,
                                                                            TimeSpentCharging:         TimeSpan.FromHours(2),
                                                                            StoppedReason:             StopTransactionReasons.Local,
                                                                            RemoteStartId:             null,
                                                                            OperationMode:             OperationModes.ChargingOnly,
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
                                                                                                         Value:              10000,
                                                                                                         Context:            ReadingContexts.TransactionEnd,
                                                                                                         Measurand:          Measurands.Current_Import_Offered,
                                                                                                         Phase:              null,
                                                                                                         Location:           MeasurementLocations.Outlet,
                                                                                                         SignedMeterValue:   new SignedMeterValue(
                                                                                                                                 SignedMeterData:   "",
                                                                                                                                 SigningMethod:     "",
                                                                                                                                 EncodingMethod:    "",
                                                                                                                                 PublicKey:         "",
                                                                                                                                 CustomData:        null
                                                                                                                             ),
                                                                                                         UnitOfMeasure:      UnitsOfMeasure.Wh(1),
                                                                                                         CustomData:         null
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

            Assert.IsNotNull(transactionEvents);
            Assert.AreEqual (2, transactionEvents.Length);

            #endregion


            if (chargingTariff    is not null &&
                transactionEvents is not null)
            {

                Assert.IsTrue(CDR.CalculateCosts(

                                  ProviderId:            Provider_Id.   Parse ("DE-GDF"),
                                  ProviderName:          DisplayTexts.  Create("GraphDefined EMP"),
                                  CSOOperatorId:         CSOOperator_Id.Parse ("DE*GEF"),
                                  EVSEId:                GlobalEVSE_Id. Parse ("DE*GEF*E12345678*1"),
                                  MeterValues:           transactionEvents.SelectMany(transactionEvent => transactionEvent.MeterValues),
                                  ChargingTariff:        chargingTariff,

                                  CDR:                   out var cdr,
                                  ErrorResponse:         out var errorString,

                                  Measurand:             Measurands.Current_Import_Offered,
                                  MeasurementLocation:   MeasurementLocations.Outlet

                              ),
                              errorString);
                Assert.IsNotNull(cdr);

                if (cdr is not null)
                {

                    Assert.AreEqual(39,      cdr.TotalTime.        TotalMinutes);
                    Assert.AreEqual(39,      cdr.BilledTime.       TotalMinutes);

                    Assert.AreEqual(39,      cdr.TotalChargingTime.TotalMinutes);

                    Assert.AreEqual(9999,    cdr.TotalEnergy.      Value);
                    Assert.AreEqual(10000,   cdr.BilledEnergy.     Value);

                    Assert.AreEqual(5.1,     cdr.TotalCost.ExcludingVAT);
                    Assert.AreEqual(5.3,     cdr.TotalCost.IncludingVAT);

                }

            }

        }

        #endregion




    }

}
