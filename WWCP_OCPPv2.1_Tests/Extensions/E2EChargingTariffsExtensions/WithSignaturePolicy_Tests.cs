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
                                                                   [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                             MinPrice:         null,
                                             MaxPrice:         new Price(
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

                ClassicAssert.AreEqual(ResultCode.OK,                            response.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,   response.Status);

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                         setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,                       setDefaultChargingTariffRequests.First().DestinationId);

                ClassicAssert.AreEqual((object)chargingTariff.Id,                         setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                           setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                    setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",          setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                ClassicAssert.AreEqual(1,                                         setDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,         setDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                 setDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a backend test request!",            setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                          setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

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

                ClassicAssert.AreEqual(ResultCode.OK,                       response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,              response.Status);
                ClassicAssert.AreEqual(0,                                   response.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(0,                                   response.ChargingTariffMap.Count());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                   getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                 getDefaultChargingTariffRequests.First().DestinationId);

                ClassicAssert.AreEqual(1,                                   getDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,   getDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                           getDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a backend test request!",      getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                    getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

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
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
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

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response1.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,                           response1.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                ClassicAssert.AreEqual("cs001",                                                           response1.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station SetDefaultE2EChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,                               setDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the charging tariff
                ClassicAssert.AreEqual((object)chargingTariff.Id,                                 setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS SetDefaultE2EChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response2.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 1 EVSE!
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                ClassicAssert.AreEqual("cs001",                                                           response2.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,                                               getDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response3.Result.ResultCode);
                ClassicAssert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                ClassicAssert.AreEqual("cs001",                                                           response3.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,                                               removeDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response4.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                ClassicAssert.AreEqual("cs001",                                                           response4.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
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

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response1.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,                           response1.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response1.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station SetDefaultE2EChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the charging tariff
                ClassicAssert.AreEqual((object)chargingTariff.Id,                                 setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS SetDefaultE2EChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response2.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                ClassicAssert.AreEqual(2,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 2 EVSEs!
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response3.Result.ResultCode);
                ClassicAssert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response4.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
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

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response1.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,                           response1.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response1.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station SetDefaultE2EChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the charging tariff
                ClassicAssert.AreEqual((object)chargingTariff.Id,                                setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS SetDefaultE2EChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response2.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 1 EVSEs!
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response3.Result.ResultCode);
                ClassicAssert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To(chargingStation2.Id),
                                           CustomData:         null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                    response4.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
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
                                                                 [ new TariffEnergyPrice(0.51M, StepSize: WattHour.TryParseKWh(1)) ],
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

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
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

                ClassicAssert.AreEqual(ResultCode.OK,                                                     response1a.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,                           response1a.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response1a.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response1a.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station SetDefaultE2EChargingTariff response!",      response1a.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now2.ToIso8601(),                                                  response1a.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the charging tariff
                ClassicAssert.AreEqual((object)chargingTariff1.Id,                                setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS SetDefaultE2EChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response1b       = await testCSMS1.SetDefaultE2EChargingTariff(
                                           Destination: SourceRouting.To(chargingStation2.Id),
                                           ChargingTariff:   (Tariff)chargingTariff2,
                                           EVSEIds:          [EVSE_Id.Parse(2) ],
                                           CustomData:       null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                     response1b.Result.ResultCode);
                ClassicAssert.AreEqual(SetDefaultE2EChargingTariffStatus.Accepted,                        response1b.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response1b.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response1b.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station SetDefaultE2EChargingTariff response!",      response1b.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now2.ToIso8601(),                                                  response1b.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(2,                                                 setDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.ElementAt(1).DestinationId);

                // Verify the signature of the charging tariff
                ClassicAssert.AreEqual((object)chargingTariff2.Id,                                setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Id);
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.Count());
                ClassicAssert.IsTrue  (                                                   setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Verify(out var errr2));
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Status);
                ClassicAssert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                 setDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS SetDefaultE2EChargingTariff request!",   setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                     response2.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                ClassicAssert.AreEqual(2,                                                                 response2.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(2,                                                                 response2.ChargingTariffMap.Count());                     // 2 Charging tariffs...
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.ElementAt(0).Value.Count());  // ...at 1 EVSEs!
                ClassicAssert.AreEqual(1,                                                                 response2.ChargingTariffMap.ElementAt(1).Value.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS1.RemoveDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                     response3.Result.ResultCode);
                ClassicAssert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS1.GetDefaultChargingTariff(
                                           Destination:    SourceRouting.To( chargingStation2.Id),
                                           CustomData:      null
                                       );

                #region Verify the response

                ClassicAssert.AreEqual(ResultCode.OK,                                                     response4.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                ClassicAssert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                ClassicAssert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                ClassicAssert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                ClassicAssert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                ClassicAssert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).DestinationId);

                // Verify the signature of the request
                ClassicAssert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                ClassicAssert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                ClassicAssert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                ClassicAssert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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
