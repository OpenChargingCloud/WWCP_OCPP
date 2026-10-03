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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2EChargingTariffsExtensions
{

    /// <summary>
    /// Unit tests for a CSMS sending signed messages to charging stations.
    /// </summary>
    [TestFixture]
    public class WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region SetupEachTest()

        [SetUp]
        public override async Task SetupEachTest()
        {

            await base.SetupEachTest();

            // The CSMS and the charging stations are shared between all tests of this fixture,
            // but every test registers its own signing/verification rules with fresh key pairs.
            // Without a reset the signing rules accumulate and later tests sign every message
            // multiple times, breaking their "exactly one signature" assertions!
            foreach (var signaturePolicy in new[] {
                                                testCSMS1?.       OCPP.SignaturePolicy,
                                                chargingStation1?.OCPP.SignaturePolicy,
                                                chargingStation2?.OCPP.SignaturePolicy,
                                                chargingStation3?.OCPP.SignaturePolicy
                                            })
            {
                signaturePolicy?.ClearSigningRules().
                                 ClearVerificationRules();
            }

        }

        #endregion


        #region SetDefaultE2EChargingTariffRequest_Test1()

        /// <summary>
        /// A test for sending a signed default charging tariff to a charging station.
        /// </summary>
        [Test]
        public async Task SetDefaultE2EChargingTariffRequest_Test1()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1            = Timestamp.Now;
                var requestKeyPair  = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffRequest. DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2            = Timestamp.Now;
                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffResponse.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests = new ConcurrentList<SetDefaultE2EChargingTariffRequest>();

                chargingStation1.OCPP.IN.OnSetDefaultE2EChargingTariffRequestReceived += (timestamp, sender, connection, setDefaultChargingTariffRequest, ct) => {
                    setDefaultChargingTariffRequests.TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair    = ECCKeyPair.GenerateKeys()!;

                var chargingTariff     = new Tariff(

                                             Id:               Tariff_Id.Parse("DE-GDF-T12345678"),
                                             //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                             //ProviderName:     new DisplayTexts(
                                             //                      Languages.en,
                                             //                      "GraphDefined EMP"
                                             //                  ),
                                             Currency:         Currency.EUR,
                                             Energy:           new TariffEnergy(
                                                                   [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                   [ TaxRate.VAT(15)]
                                                               ),

                                             //TariffElements:   [
                                             //                      new TariffElement(
                                             //                          [
                                             //                              PriceComponent.Energy(
                                             //                                  Price:      0.51M,
                                             //                                  VAT:        0.02M,
                                             //                                  StepSize:   WattHour.ParseKWh(1)
                                             //                              )
                                             //                          ]
                                             //                      )
                                             //                  ],

                                             //Created:          timeReference,
                                             //Replaces:         null,
                                             //References:       null,
                                             //TariffType:       TariffType.REGULAR,
                                             Description:      new MessageContents(
                                                                   "0.53 / kWh",
                                                                   Language_Id.EN
                                                               ),
                                             //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                             //EnergyMix:        null,

                                             MinCost:          null,
                                             MaxCost:          new Price(
                                                                   ExcludingTaxes:  0.51M,
                                                                   IncludingTaxes:  0.53M
                                                               ),
                                             //NotBefore:        timeReference,
                                             //NotAfter:         null,

                                             SignKeys:         null,
                                             SignInfos:        null,
                                             Signatures:       null,

                                             CustomData:       null

                                         );

                ClassicAssert.IsNotNull(chargingTariff);


                ClassicAssert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS1.OCPP.CustomChargingTariffSerializer
                                                     //testCSMS01.OCPP.CustomPriceSerializer,
                                                     //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                     //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                     //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                     //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                     //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                     //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                     //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                     //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                     //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                     //testCSMS01.OCPP.CustomSignatureSerializer,
                                                     //testCSMS01.OCPP.CustomCustomDataSerializer
                                                     ));

                ClassicAssert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response        = await testCSMS1.SetDefaultE2EChargingTariff(
                                          Destination: SourceRouting.To(chargingStation1.Id),
                                          ChargingTariff:     (Tariff)chargingTariff,
                                          CustomData:         null
                                      );

                #region Verify the response

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation1.Id));

                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Id, Is.EqualTo((object)chargingTariff.Id));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                           setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                Assert.That(setDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a backend test request!"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion

            }

        }

        #endregion

        #region GetDefaultChargingTariffRequest_Test1()

        /// <summary>
        /// A test for requesting the default charging tariffs
        /// of an unconfigured charging station.
        /// </summary>
        [Test]
        public async Task GetDefaultChargingTariffRequest_Test1()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference   = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1            = Timestamp.Now;
                var requestKeyPair  = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2            = Timestamp.Now;
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                #endregion

                #region Setup charging station incoming request monitoring

                var getDefaultChargingTariffRequests = new ConcurrentList<GetDefaultChargingTariffRequest>();

                chargingStation2.OCPP.IN.OnGetDefaultChargingTariffRequestReceived += (timestamp, sender, connection, getDefaultChargingTariffRequest, ct) => {
                    getDefaultChargingTariffRequests.TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion


                var response        = await testCSMS1.GetDefaultChargingTariff(
                                          Destination:    SourceRouting.To(chargingStation2.Id),
                                          CustomData:         null
                                      );

                #region Verify the response

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response.ChargingTariffs.Count(), Is.EqualTo(0));
                Assert.That(response.ChargingTariffMap.Count(), Is.EqualTo(0));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                Assert.That(getDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a backend test request!"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion

            }

        }

        #endregion


        #region SetGetRemoveGet_DefaultChargingTariffRequest_1EVSE_Test()

        /// <summary>
        /// A test for sending a signed default charging tariff to a charging station
        /// having a single EVSE, verify it via GetDefaultChargingTariff, remove it
        /// and verify it again.
        /// </summary>
        [Test]
        public async Task SetGetRemoveGet_DefaultChargingTariffRequest_1EVSE_Test()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultE2EChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = ECCKeyPair.GenerateKeys()!;
                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultE2EChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                chargingStation1.OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation1.OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultE2EChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation1.OCPP.IN.OnSetDefaultE2EChargingTariffRequestReceived += (timestamp, sender, connection, setDefaultChargingTariffRequest, ct) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OCPP.IN.OnGetDefaultChargingTariffRequestReceived    += (timestamp, sender, connection, getDefaultChargingTariffRequest, ct) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OCPP.IN.OnRemoveDefaultChargingTariffRequestReceived += (timestamp, sender, connection, removeDefaultChargingTariffRequest, ct) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = ECCKeyPair.GenerateKeys()!;

                var chargingTariff   = new Tariff(

                                           Id:               Tariff_Id.Parse("DE-GDF-T12345678"),
                                           //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           //ProviderName:     new DisplayTexts(
                                           //                      Languages.en,
                                           //                      "GraphDefined EMP"
                                           //                  ),
                                           Currency:         Currency.EUR,
                                           Energy:           new TariffEnergy(
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                 [ TaxRate.VAT(15)]
                                                             ),
                                           //TariffElements:   [
                                           //                      new TariffElement(
                                           //                          [
                                           //                              PriceComponent.Energy(
                                           //                                  Price:      0.51M,
                                           //                                  VAT:        0.02M,
                                           //                                  StepSize:   WattHour.ParseKWh(1)
                                           //                              )
                                           //                          ]
                                           //                      )
                                           //                  ],

                                           //Created:          timeReference,
                                           //Replaces:         null,
                                           //References:       null,
                                           //TariffType:       TariffType.REGULAR,
                                           Description:      new MessageContents(
                                                                 "0.53 / kWh",
                                                                 Language_Id.EN
                                                             ),
                                           //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           //EnergyMix:        null,

                                           MinCost:          null,
                                           MaxCost:          new Price(
                                                                 ExcludingTaxes:  0.51M,
                                                                 IncludingTaxes:  0.53M
                                                             ),
                                           //NotBefore:        timeReference,
                                           //NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                ClassicAssert.IsNotNull(chargingTariff);


                ClassicAssert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS1.OCPP.CustomChargingTariffSerializer
                                                     //testCSMS01.OCPP.CustomPriceSerializer,
                                                     //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                     //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                     //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                     //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                     //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                     //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                     //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                     //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                     //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                     //testCSMS01.OCPP.CustomSignatureSerializer,
                                                     //testCSMS01.OCPP.CustomCustomDataSerializer
                                                     ));

                ClassicAssert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation1.Id),
                                           ChargingTariff:     (Tariff)chargingTariff,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response1.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));
                Assert.That(response1.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response1.Signatures.First().Name, Is.EqualTo("cs001"));
                Assert.That(response1.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station SetDefaultE2EChargingTariff response!"));
                Assert.That(response1.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation1.Id));

                // Verify the signature of the charging tariff
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Id, Is.EqualTo((object)chargingTariff.Id));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                // Verify the signature of the request
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS SetDefaultE2EChargingTariff request!"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response2.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response2.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response2.ChargingTariffs.Count(), Is.EqualTo(1));
                Assert.That(response2.ChargingTariffMap.Count(), Is.EqualTo(1));                // 1 Charging tariff...
                Assert.That(response2.ChargingTariffMap.First().Value.Count(), Is.EqualTo(1));  // ...at 1 EVSE!
                Assert.That(response2.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response2.Signatures.First().Name, Is.EqualTo("cs001"));
                Assert.That(response2.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response2.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation1.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response3.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response3.Status, Is.EqualTo(RemoveDefaultChargingTariffStatus.Accepted));
                Assert.That(response3.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response3.Signatures.First().Name, Is.EqualTo("cs001"));
                Assert.That(response3.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station RemoveDefaultChargingTariff response!"));
                Assert.That(response3.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(removeDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation1.Id));

                // Verify the signature of the request
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS RemoveDefaultChargingTariff request!"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response4.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response4.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response4.ChargingTariffs.Count(), Is.EqualTo(0));
                Assert.That(response4.ChargingTariffMap.Count(), Is.EqualTo(0));
                Assert.That(response4.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response4.Signatures.First().Name, Is.EqualTo("cs001"));
                Assert.That(response4.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response4.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(2));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).DestinationId, Is.EqualTo(chargingStation1.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion


            }

        }

        #endregion

        #region SetGetRemoveGet_DefaultChargingTariffRequest_2EVSEs_Test()

        /// <summary>
        /// A test for sending a signed default charging tariff to a charging station
        /// having two EVSEs, verify it via GetDefaultChargingTariff, remove it
        /// and verify it again.
        /// </summary>
        [Test]
        public async Task SetGetRemoveGet_DefaultChargingTariffRequest_2EVSEs_Test()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultE2EChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = ECCKeyPair.GenerateKeys()!;
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultE2EChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultE2EChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OCPP.IN.OnSetDefaultE2EChargingTariffRequestReceived += (timestamp, sender, connection, setDefaultChargingTariffRequest, ct) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnGetDefaultChargingTariffRequestReceived    += (timestamp, sender, connection, getDefaultChargingTariffRequest, ct) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnRemoveDefaultChargingTariffRequestReceived += (timestamp, sender, connection, removeDefaultChargingTariffRequest, ct) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = ECCKeyPair.GenerateKeys()!;

                var chargingTariff   = new Tariff(

                                           Id:               Tariff_Id.Parse("DE-GDF-T12345678"),
                                           //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           //ProviderName:     new DisplayTexts(
                                           //                      Languages.en,
                                           //                      "GraphDefined EMP"
                                           //                  ),
                                           Currency:         Currency.EUR,
                                           Energy:           new TariffEnergy(
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                 [ TaxRate.VAT(15)]
                                                             ),
                                           //TariffElements:   [
                                           //                      new TariffElement(
                                           //                          [
                                           //                              PriceComponent.Energy(
                                           //                                  Price:      0.51M,
                                           //                                  VAT:        0.02M,
                                           //                                  StepSize:   WattHour.ParseKWh(1)
                                           //                              )
                                           //                          ]
                                           //                      )
                                           //                  ],

                                           //Created:          timeReference,
                                           //Replaces:         null,
                                           //References:       null,
                                           //TariffType:       TariffType.REGULAR,
                                           Description:      new MessageContents(
                                                                 "0.53 / kWh",
                                                                 Language_Id.EN
                                                             ),
                                           //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           //EnergyMix:        null,

                                           MinCost:          null,
                                           MaxCost:          new Price(
                                                                 ExcludingTaxes:  0.51M,
                                                                 IncludingTaxes:  0.53M
                                                             ),
                                           //NotBefore:        timeReference,
                                           //NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                ClassicAssert.IsNotNull(chargingTariff);


                ClassicAssert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS1.OCPP.CustomChargingTariffSerializer
                                                     //testCSMS01.OCPP.CustomPriceSerializer,
                                                     //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                     //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                     //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                     //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                     //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                     //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                     //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                     //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                     //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                     //testCSMS01.OCPP.CustomSignatureSerializer,
                                                     //testCSMS01.OCPP.CustomCustomDataSerializer
                                                     ));

                ClassicAssert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation2.Id),
                                           ChargingTariff:     (Tariff)chargingTariff,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response1.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));
                Assert.That(response1.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response1.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response1.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station SetDefaultE2EChargingTariff response!"));
                Assert.That(response1.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the charging tariff
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Id, Is.EqualTo((object)chargingTariff.Id));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                // Verify the signature of the request
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS SetDefaultE2EChargingTariff request!"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response2.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response2.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response2.ChargingTariffs.Count(), Is.EqualTo(1));
                Assert.That(response2.ChargingTariffMap.Count(), Is.EqualTo(1));                // 1 Charging tariff...
                Assert.That(response2.ChargingTariffMap.First().Value.Count(), Is.EqualTo(2));  // ...at 2 EVSEs!
                Assert.That(response2.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response2.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response2.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response2.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response3.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response3.Status, Is.EqualTo(RemoveDefaultChargingTariffStatus.Accepted));
                Assert.That(response3.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response3.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response3.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station RemoveDefaultChargingTariff response!"));
                Assert.That(response3.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(removeDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS RemoveDefaultChargingTariff request!"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response4.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response4.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response4.ChargingTariffs.Count(), Is.EqualTo(0));
                Assert.That(response4.ChargingTariffMap.Count(), Is.EqualTo(0));
                Assert.That(response4.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response4.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response4.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response4.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(2));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion


            }

        }

        #endregion

        #region SetGetRemoveGet_DefaultChargingTariffRequestForEVSE_2EVSEs_Test()

        /// <summary>
        /// A test for sending a signed default charging tariff to an EVSE of a
        /// charging station having two EVSEs, verify it via GetDefaultChargingTariff,
        /// remove it and verify it again.
        /// </summary>
        [Test]
        public async Task SetGetRemoveGet_DefaultChargingTariffRequestForEVSE_2EVSEs_Test()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultE2EChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffResponse.DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = ECCKeyPair.GenerateKeys()!;
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultE2EChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultE2EChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OCPP.IN.OnSetDefaultE2EChargingTariffRequestReceived += (timestamp, sender, connection, setDefaultChargingTariffRequest, ct) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnGetDefaultChargingTariffRequestReceived    += (timestamp, sender, connection, getDefaultChargingTariffRequest, ct) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnRemoveDefaultChargingTariffRequestReceived += (timestamp, sender, connection, removeDefaultChargingTariffRequest, ct) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = ECCKeyPair.GenerateKeys()!;

                var chargingTariff   = new Tariff(

                                           Id:               Tariff_Id.Parse("DE-GDF-T12345678"),
                                           //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           //ProviderName:     new DisplayTexts(
                                           //                      Languages.en,
                                           //                      "GraphDefined EMP"
                                           //                  ),
                                           Currency:         Currency.EUR,
                                           Energy:           new TariffEnergy(
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                 [ TaxRate.VAT(15)]
                                                             ),
                                           //TariffElements:   [
                                           //                      new TariffElement(
                                           //                          [
                                           //                              PriceComponent.Energy(
                                           //                                  Price:      0.51M,
                                           //                                  VAT:        0.02M,
                                           //                                  StepSize:   WattHour.ParseKWh(1)
                                           //                              )
                                           //                          ]
                                           //                      )
                                           //                  ],

                                           //Created:          timeReference,
                                           //Replaces:         null,
                                           //References:       null,
                                           //TariffType:       TariffType.REGULAR,
                                           Description:      new MessageContents(
                                                                 "0.53 / kWh",
                                                                 Language_Id.EN
                                                             ),
                                           //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           //EnergyMix:        null,

                                           MinCost:          null,
                                           MaxCost:          new Price(
                                                                 ExcludingTaxes:  0.51M,
                                                                 IncludingTaxes:  0.53M
                                                             ),
                                           //NotBefore:        timeReference,
                                           //NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                ClassicAssert.IsNotNull(chargingTariff);


                ClassicAssert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS1.OCPP.CustomChargingTariffSerializer
                                                     //testCSMS01.OCPP.CustomPriceSerializer,
                                                     //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                     //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                     //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                     //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                     //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                     //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                     //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                     //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                     //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                     //testCSMS01.OCPP.CustomSignatureSerializer,
                                                     //testCSMS01.OCPP.CustomCustomDataSerializer
                                                     ));

                ClassicAssert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation2.Id),
                                           ChargingTariff:     (Tariff)chargingTariff,
                                           EVSEIds:            new[] {
                                                                   EVSE_Id.Parse(1)
                                                               },
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response1.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));
                Assert.That(response1.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response1.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response1.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station SetDefaultE2EChargingTariff response!"));
                Assert.That(response1.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the charging tariff
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Id, Is.EqualTo((object)chargingTariff.Id));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                // Verify the signature of the request
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS SetDefaultE2EChargingTariff request!"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response2.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response2.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response2.ChargingTariffs.Count(), Is.EqualTo(1));
                Assert.That(response2.ChargingTariffMap.Count(), Is.EqualTo(1));                // 1 Charging tariff...
                Assert.That(response2.ChargingTariffMap.First().Value.Count(), Is.EqualTo(1));  // ...at 1 EVSEs!
                Assert.That(response2.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response2.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response2.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response2.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response3.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response3.Status, Is.EqualTo(RemoveDefaultChargingTariffStatus.Accepted));
                Assert.That(response3.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response3.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response3.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station RemoveDefaultChargingTariff response!"));
                Assert.That(response3.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(removeDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS RemoveDefaultChargingTariff request!"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.That(response4.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response4.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response4.ChargingTariffs.Count(), Is.EqualTo(0));
                Assert.That(response4.ChargingTariffMap.Count(), Is.EqualTo(0));
                Assert.That(response4.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response4.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response4.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response4.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(2));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion


            }

        }

        #endregion

        #region SetGetRemoveGet_TwoDefaultChargingTariffRequestsForTwoEVSEs_Test()

        /// <summary>
        /// A test for sending a signed default charging tariff to an EVSE of a
        /// charging station having two EVSEs, verify it via GetDefaultChargingTariff,
        /// remove it and verify it again.
        /// </summary>
        [Test]
        public async Task SetGetRemoveGet_TwoDefaultChargingTariffRequestsForTwoEVSEs_Test()
        {

            ClassicAssert.IsNotNull(testCSMS1);
            ClassicAssert.IsNotNull(testBackendWebSockets1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = ECCKeyPair.GenerateKeys()!;
                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultE2EChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS1.      OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                          requestKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                          TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS1.      OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = ECCKeyPair.GenerateKeys()!;
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(SetDefaultE2EChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.OCPP.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (SetDefaultE2EChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultE2EChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.OCPP.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                          responseKeyPair!,
                                                                          UserIdGenerator:         (signableMessage) => "cs002",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                          TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultE2EChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OCPP.IN.OnSetDefaultE2EChargingTariffRequestReceived += (timestamp, sender, connection, setDefaultChargingTariffRequest, ct) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnGetDefaultChargingTariffRequestReceived    += (timestamp, sender, connection, getDefaultChargingTariffRequest, ct) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OCPP.IN.OnRemoveDefaultChargingTariffRequestReceived += (timestamp, sender, connection, removeDefaultChargingTariffRequest, ct) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define 1. signed charging tariff

                var providerKeyPair  = ECCKeyPair.GenerateKeys()!;

                var chargingTariff1  = new Tariff(

                                           Id:               Tariff_Id.Parse("DE-GDF-T12345678-1"),
                                           //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           //ProviderName:     new DisplayTexts(
                                           //                      Languages.en,
                                           //                      "GraphDefined EMP"
                                           //                  ),
                                           Currency:         Currency.EUR,
                                           Energy:           new TariffEnergy(
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                 [ TaxRate.VAT(15)]
                                                             ),
                                           //TariffElements:   [
                                           //                      new TariffElement(
                                           //                          [
                                           //                              PriceComponent.Energy(
                                           //                                  Price:      0.51M,
                                           //                                  VAT:        0.02M,
                                           //                                  StepSize:   WattHour.ParseKWh(1)
                                           //                              )
                                           //                          ]
                                           //                      )
                                           //                  ],

                                           //Created:          timeReference,
                                           //Replaces:         null,
                                           //References:       null,
                                           //TariffType:       TariffType.REGULAR,
                                           Description:      new MessageContents(
                                                                 "0.53 / kWh",
                                                                 Language_Id.EN
                                                             ),
                                           //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           //EnergyMix:        null,

                                           MinCost:          null,
                                           MaxCost:          new Price(
                                                                 ExcludingTaxes:  0.51M,
                                                                 IncludingTaxes:  0.53M
                                                             ),
                                           //NotBefore:        timeReference,
                                           //NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                ClassicAssert.IsNotNull(chargingTariff1);


                ClassicAssert.IsTrue   (chargingTariff1.Sign(providerKeyPair,
                                                             out var eerr,
                                                             "emp1",
                                                             I18NString.Create("Just a signed charging tariff!"),
                                                             timeReference,
                                                             testCSMS1.OCPP.CustomChargingTariffSerializer
                                                             //testCSMS01.OCPP.CustomPriceSerializer,
                                                             //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                             //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                             //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                             //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                             //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                             //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                             //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                             //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                             //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                             //testCSMS01.OCPP.CustomSignatureSerializer,
                                                             //testCSMS01.OCPP.CustomCustomDataSerializer
                                                             ));

                ClassicAssert.IsTrue   (chargingTariff1.Signatures.Any());

                #endregion

                #region Define 2. signed charging tariff

                var chargingTariff2  = new Tariff(

                                           Id:               Tariff_Id.Parse("DE-GDF-T12345678-2"),
                                           //ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           //ProviderName:     new DisplayTexts(
                                           //                      Languages.en,
                                           //                      "GraphDefined EMP"
                                           //                  ),
                                           Currency:         Currency.EUR,
                                           Energy:           new TariffEnergy(
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryFromKWh(1)) ],
                                                                 [ TaxRate.VAT(15)]
                                                             ),

                                           //TariffElements:   [
                                           //                      new TariffElement(
                                           //                          [
                                           //                              PriceComponent.Energy(
                                           //                                  Price:      0.51M,
                                           //                                  VAT:        0.02M,
                                           //                                  StepSize:   WattHour.ParseKWh(1)
                                           //                              )
                                           //                          ]
                                           //                      )
                                           //                  ],

                                           //Created:          timeReference,
                                           //Replaces:         null,
                                           //References:       null,
                                           //TariffType:       TariffType.REGULAR,
                                           Description:      new MessageContents(
                                                                 "0.53 / kWh",
                                                                 Language_Id.EN
                                                             ),
                                           //URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           //EnergyMix:        null,

                                           MinCost:          null,
                                           MaxCost:          new Price(
                                                                 ExcludingTaxes:  0.51M,
                                                                 IncludingTaxes:  0.53M
                                                             ),
                                           //NotBefore:        timeReference,
                                           //NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                ClassicAssert.IsNotNull(chargingTariff2);


                ClassicAssert.IsTrue   (chargingTariff2.Sign(providerKeyPair,
                                                             out var eerr2,
                                                             "emp1",
                                                             I18NString.Create("Just a signed charging tariff!"),
                                                             timeReference,
                                                             testCSMS1.OCPP.CustomChargingTariffSerializer
                                                             //testCSMS01.OCPP.CustomPriceSerializer,
                                                             //testCSMS01.OCPP.CustomTaxRateSerializer,
                                                             //testCSMS01.OCPP.CustomTariffElementSerializer,
                                                             //testCSMS01.OCPP.CustomPriceComponentSerializer,
                                                             //testCSMS01.OCPP.CustomTariffRestrictionsSerializer,
                                                             //testCSMS01.OCPP.CustomEnergyMixSerializer,
                                                             //testCSMS01.OCPP.CustomEnergySourceSerializer,
                                                             //testCSMS01.OCPP.CustomEnvironmentalImpactSerializer,
                                                             //testCSMS01.OCPP.CustomIdTokenSerializer,
                                                             //testCSMS01.OCPP.CustomAdditionalInfoSerializer,
                                                             //testCSMS01.OCPP.CustomSignatureSerializer,
                                                             //testCSMS01.OCPP.CustomCustomDataSerializer
                                                             ));

                ClassicAssert.IsTrue   (chargingTariff2.Signatures.Any());

                #endregion


                var response1a       = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation2.Id),
                                           ChargingTariff:   (Tariff)chargingTariff1,
                                           EVSEIds:          [EVSE_Id.Parse(1) ],
                                           CustomData:       null
                                       );

                #region Verify the response

                Assert.That(response1a.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1a.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));
                Assert.That(response1a.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response1a.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response1a.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station SetDefaultE2EChargingTariff response!"));
                Assert.That(response1a.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the charging tariff
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Id, Is.EqualTo((object)chargingTariff1.Id));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                // Verify the signature of the request
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS SetDefaultE2EChargingTariff request!"));
                Assert.That(setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion



                var response1b       = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation2.Id),
                                           ChargingTariff:   (Tariff)chargingTariff2,
                                           EVSEIds:          [EVSE_Id.Parse(2) ],
                                           CustomData:       null
                                       );

                #region Verify the response

                Assert.That(response1b.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1b.Status, Is.EqualTo(SetDefaultE2EChargingTariffStatus.Accepted));
                Assert.That(response1b.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response1b.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response1b.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station SetDefaultE2EChargingTariff response!"));
                Assert.That(response1b.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(setDefaultChargingTariffRequests.Count, Is.EqualTo(2));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the charging tariff
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Id, Is.EqualTo((object)chargingTariff2.Id));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.Count(), Is.EqualTo(1));
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Verify(out var errr2));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Name, Is.EqualTo("emp1"));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a signed charging tariff!"));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(timeReference.ToISO8601()));

                // Verify the signature of the request
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).Signatures.Count(), Is.EqualTo(1));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS SetDefaultE2EChargingTariff request!"));
                Assert.That(setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                Assert.That(response2.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response2.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response2.ChargingTariffs.Count(), Is.EqualTo(2));
                Assert.That(response2.ChargingTariffMap.Count(), Is.EqualTo(2));                     // 2 Charging tariffs...
                Assert.That(response2.ChargingTariffMap.ElementAt(0).Value.Count(), Is.EqualTo(1));  // ...at 1 EVSEs!
                Assert.That(response2.ChargingTariffMap.ElementAt(1).Value.Count(), Is.EqualTo(1));
                Assert.That(response2.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response2.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response2.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response2.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                Assert.That(response3.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response3.Status, Is.EqualTo(RemoveDefaultChargingTariffStatus.Accepted));
                Assert.That(response3.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response3.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response3.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station RemoveDefaultChargingTariff response!"));
                Assert.That(response3.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(removeDefaultChargingTariffRequests.Count, Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS RemoveDefaultChargingTariff request!"));
                Assert.That(removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(8)).ToISO8601()));

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                Assert.That(response4.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response4.Status, Is.EqualTo(GenericStatus.Accepted));
                Assert.That(response4.ChargingTariffs.Count(), Is.EqualTo(0));
                Assert.That(response4.ChargingTariffMap.Count(), Is.EqualTo(0));
                Assert.That(response4.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response4.Signatures.First().Name, Is.EqualTo("cs002"));
                Assert.That(response4.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station GetDefaultChargingTariff response!"));
                Assert.That(response4.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now2 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion

                #region Verify the request at the charging station

                Assert.That(getDefaultChargingTariffRequests.Count, Is.EqualTo(2));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).DestinationId, Is.EqualTo(chargingStation2.Id));

                // Verify the signature of the request
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count(), Is.EqualTo(1));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText(), Is.EqualTo("Just a CSMS GetDefaultChargingTariff request!"));
                Assert.That(getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo((now1 + TimeSpan.FromSeconds(4)).ToISO8601()));

                #endregion


            }

        }

        #endregion


        //ToDo: Set an AC-only default charging tariff on an entire charging station having an AC and a DC EVSE.
        //      Will be accepted at the AC EVSE, but MUST fail at the DC EVSE!

        //ToDo: Set a charging tariff on an entire charging station having a charging station id restriction
        //      for another charging station. MUST fail!

        //ToDo: Set a charging tariff on an entire charging station having an EVSE id restriction for one of
        //      its EVSEs. Will be accepted at one EVSE, but MUST fail at the other EVSE!


    }

}
