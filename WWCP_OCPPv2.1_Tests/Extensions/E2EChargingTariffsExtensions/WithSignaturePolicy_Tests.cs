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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.E2EChargingTariffsExtensions
{

    /// <summary>
    /// Unit tests for a central system sending signed messages to charging stations.
    /// </summary>
    [TestFixture]
    public class WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region SetDefaultChargingTariffRequest_Test1()

        /// <summary>
        /// A test for sending a signed default charging tariff to a charging station.
        /// </summary>
        [Test]
        public async Task SetDefaultChargingTariffRequest_Test1()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference      = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1            = Timestamp.Now;
                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2            = Timestamp.Now;
                chargingStation1.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests = new ConcurrentList<SetDefaultChargingTariffRequest>();

                chargingStation1.OnSetDefaultChargingTariffRequest += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair    = KeyPair.GenerateKeys()!;

                var chargingTariff     = new ChargingTariff(

                                             Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                             ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                             ProviderName:     new DisplayTexts(
                                                                   Languages.en,
                                                                   "GraphDefined EMP"
                                                               ),
                                             Currency:         Currency.EUR,
                                             TariffElements:   new[] {
                                                                   new TariffElement(
                                                                       new[] {
                                                                           PriceComponent.Energy(
                                                                               Price:      0.51M,
                                                                               VAT:        0.02M,
                                                                               StepSize:   1000
                                                                           )
                                                                       }
                                                                       //new TariffRestrictions(
                                                                       //    MaxCurrent:   Ampere.Parse(10)
                                                                       //)
                                                                   )
                                                               },

                                             Created:          timeReference,
                                             Replaces:         null,
                                             References:       null,
                                             TariffType:       TariffType.REGULAR,
                                             Description:      new DisplayTexts(
                                                                   Languages.en,
                                                                   "0.53 / kWh"
                                                               ),
                                             URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                             EnergyMix:        null,

                                             MinPrice:         null,
                                             MaxPrice:         new Price(
                                                                   ExcludingVAT:  0.51M,
                                                                   IncludingVAT:  0.53M
                                                               ),
                                             NotBefore:        timeReference,
                                             NotAfter:         null,

                                             SignKeys:         null,
                                             SignInfos:        null,
                                             Signatures:       null,

                                             CustomData:       null

                                         );

                Assert.IsNotNull(chargingTariff);


                Assert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response        = await testCSMS01.SetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          ChargingTariff:     chargingTariff,
                                          CustomData:         null
                                      );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                            response.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,   response.Status);

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                         setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                       setDefaultChargingTariffRequests.First().ChargingStationId);

                Assert.AreEqual(chargingTariff.Id,                         setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                Assert.AreEqual(1,                                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                           setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.AreEqual(VerificationStatus.ValidSignature,         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                    setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",          setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                Assert.AreEqual(1,                                         setDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,         setDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                 setDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a backend test request!",            setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                          setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference   = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1            = Timestamp.Now;
                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2            = Timestamp.Now;
                chargingStation2.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                #endregion

                #region Setup charging station incoming request monitoring

                var getDefaultChargingTariffRequests = new ConcurrentList<GetDefaultChargingTariffRequest>();

                chargingStation2.OnGetDefaultChargingTariffRequest += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion


                var response        = await testCSMS01.GetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation2.Id,
                                          CustomData:         null
                                      );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                      response.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,              response.Status);
                Assert.AreEqual(0,                                   response.ChargingTariffs.  Count());
                Assert.AreEqual(0,                                   response.ChargingTariffMap.Count());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                   getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                 getDefaultChargingTariffRequests.First().ChargingStationId);

                Assert.AreEqual(1,                                   getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,   getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                           getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a backend test request!",      getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                    getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS01.      SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = KeyPair.GenerateKeys()!;
                chargingStation1.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation1.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation1.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                chargingStation1.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation1.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation1.OnSetDefaultChargingTariffRequest    += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OnGetDefaultChargingTariffRequest    += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OnRemoveDefaultChargingTariffRequest += (timestamp, sender, removeDefaultChargingTariffRequest) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = KeyPair.GenerateKeys()!;

                var chargingTariff   = new ChargingTariff(

                                           Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                           ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           ProviderName:     new DisplayTexts(
                                                                 Languages.en,
                                                                 "GraphDefined EMP"
                                                             ),
                                           Currency:         Currency.EUR,
                                           TariffElements:   new[] {
                                                                 new TariffElement(
                                                                     new[] {
                                                                         PriceComponent.Energy(
                                                                             Price:      0.51M,
                                                                             VAT:        0.02M,
                                                                             StepSize:   1000
                                                                         )
                                                                     }
                                                                     //new TariffRestrictions(
                                                                     //    MaxCurrent:   Ampere.Parse(10)
                                                                     //)
                                                                 )
                                                             },

                                           Created:          timeReference,
                                           Replaces:         null,
                                           References:       null,
                                           TariffType:       TariffType.REGULAR,
                                           Description:      new DisplayTexts(
                                                                 Languages.en,
                                                                 "0.53 / kWh"
                                                             ),
                                           URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           EnergyMix:        null,

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
                                                                 ExcludingVAT:  0.51M,
                                                                 IncludingVAT:  0.53M
                                                             ),
                                           NotBefore:        timeReference,
                                           NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                Assert.IsNotNull(chargingTariff);


                Assert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS01.SetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation1.Id,
                                           ChargingTariff:     chargingTariff,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response1.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,                           response1.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                Assert.AreEqual("cs001",                                                           response1.Signatures.First().Name);
                Assert.AreEqual("Just a charging station SetDefaultChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                Assert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                               setDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the charging tariff
                Assert.AreEqual(chargingTariff.Id,                                 setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS SetDefaultChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation1.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response2.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                Assert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 1 EVSE!
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                Assert.AreEqual("cs001",                                                           response2.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                                               getDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS01.RemoveDefaultChargingTariff(
                                           ChargingStationId:  chargingStation1.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response3.Result.ResultCode);
                Assert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                Assert.AreEqual("cs001",                                                           response3.Signatures.First().Name);
                Assert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                                               removeDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation1.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response4.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                Assert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                Assert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                Assert.AreEqual("cs001",                                                           response4.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS01.      SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = KeyPair.GenerateKeys()!;
                chargingStation2.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OnSetDefaultChargingTariffRequest    += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnGetDefaultChargingTariffRequest    += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnRemoveDefaultChargingTariffRequest += (timestamp, sender, removeDefaultChargingTariffRequest) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = KeyPair.GenerateKeys()!;

                var chargingTariff   = new ChargingTariff(

                                           Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                           ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           ProviderName:     new DisplayTexts(
                                                                 Languages.en,
                                                                 "GraphDefined EMP"
                                                             ),
                                           Currency:         Currency.EUR,
                                           TariffElements:   new[] {
                                                                 new TariffElement(
                                                                     new[] {
                                                                         PriceComponent.Energy(
                                                                             Price:      0.51M,
                                                                             VAT:        0.02M,
                                                                             StepSize:   1000
                                                                         )
                                                                     }
                                                                     //new TariffRestrictions(
                                                                     //    MaxCurrent:   Ampere.Parse(10)
                                                                     //)
                                                                 )
                                                             },

                                           Created:          timeReference,
                                           Replaces:         null,
                                           References:       null,
                                           TariffType:       TariffType.REGULAR,
                                           Description:      new DisplayTexts(
                                                                 Languages.en,
                                                                 "0.53 / kWh"
                                                             ),
                                           URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           EnergyMix:        null,

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
                                                                 ExcludingVAT:  0.51M,
                                                                 IncludingVAT:  0.53M
                                                             ),
                                           NotBefore:        timeReference,
                                           NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                Assert.IsNotNull(chargingTariff);


                Assert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS01.SetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           ChargingTariff:     chargingTariff,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response1.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,                           response1.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response1.Signatures.First().Name);
                Assert.AreEqual("Just a charging station SetDefaultChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                Assert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the charging tariff
                Assert.AreEqual(chargingTariff.Id,                                 setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS SetDefaultChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response2.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                Assert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                Assert.AreEqual(2,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 2 EVSEs!
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS01.RemoveDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response3.Result.ResultCode);
                Assert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                Assert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response4.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                Assert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                Assert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS01.      SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = KeyPair.GenerateKeys()!;
                chargingStation2.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OnSetDefaultChargingTariffRequest    += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnGetDefaultChargingTariffRequest    += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnRemoveDefaultChargingTariffRequest += (timestamp, sender, removeDefaultChargingTariffRequest) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define a signed charging tariff

                var providerKeyPair  = KeyPair.GenerateKeys()!;

                var chargingTariff   = new ChargingTariff(

                                           Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678"),
                                           ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           ProviderName:     new DisplayTexts(
                                                                 Languages.en,
                                                                 "GraphDefined EMP"
                                                             ),
                                           Currency:         Currency.EUR,
                                           TariffElements:   new[] {
                                                                 new TariffElement(
                                                                     new[] {
                                                                         PriceComponent.Energy(
                                                                             Price:      0.51M,
                                                                             VAT:        0.02M,
                                                                             StepSize:   1000
                                                                         )
                                                                     }
                                                                     //new TariffRestrictions(
                                                                     //    MaxCurrent:   Ampere.Parse(10)
                                                                     //)
                                                                 )
                                                             },

                                           Created:          timeReference,
                                           Replaces:         null,
                                           References:       null,
                                           TariffType:       TariffType.REGULAR,
                                           Description:      new DisplayTexts(
                                                                 Languages.en,
                                                                 "0.53 / kWh"
                                                             ),
                                           URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           EnergyMix:        null,

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
                                                                 ExcludingVAT:  0.51M,
                                                                 IncludingVAT:  0.53M
                                                             ),
                                           NotBefore:        timeReference,
                                           NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                Assert.IsNotNull(chargingTariff);


                Assert.IsTrue   (chargingTariff.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff.Signatures.Any());

                #endregion


                var response1        = await testCSMS01.SetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           ChargingTariff:     chargingTariff,
                                           EVSEIds:            new[] {
                                                                   EVSE_Id.Parse(1)
                                                               },
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response1.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,                           response1.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response1.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response1.Signatures.First().Name);
                Assert.AreEqual("Just a charging station SetDefaultChargingTariff response!",      response1.Signatures.First().Description?.FirstText());
                Assert.AreEqual(now2.ToIso8601(),                                                  response1.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the charging tariff
                Assert.AreEqual(chargingTariff.Id,                                setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS SetDefaultChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response2.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                Assert.AreEqual(1,                                                                 response2.ChargingTariffs.  Count());
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.Count());                // 1 Charging tariff...
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.First().Value.Count());  // ...at 1 EVSEs!
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS01.RemoveDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response3.Result.ResultCode);
                Assert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                Assert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response4.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                Assert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                Assert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var timeReference    = Timestamp.Now - TimeSpan.FromHours(1);

                #region Set the CSMS             signature policy

                var now1             = timeReference;
                var requestKeyPair   = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS SetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.   DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS GetDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(4));

                testCSMS01.      SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a CSMS RemoveDefaultChargingTariff request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1 + TimeSpan.FromSeconds(8));

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.   DefaultJSONLDContext);
                testCSMS01.      SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffResponse.DefaultJSONLDContext);

                #endregion

                #region Set the charging station signature policy

                var now2             = now1 + TimeSpan.FromSeconds(2);
                var responseKeyPair  = KeyPair.GenerateKeys()!;
                chargingStation2.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.    DefaultJSONLDContext);
                chargingStation2.SignaturePolicy.AddVerificationRule(RemoveDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation2.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station SetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);

                chargingStation2.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffResponse.   DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station GetDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(4));

                chargingStation2.SignaturePolicy.AddSigningRule     (RemoveDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     responseKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs002",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station RemoveDefaultChargingTariff response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2 + TimeSpan.FromSeconds(8));

                #endregion

                #region Setup charging station incoming request monitoring

                var setDefaultChargingTariffRequests     = new ConcurrentList<SetDefaultChargingTariffRequest>();
                var getDefaultChargingTariffRequests     = new ConcurrentList<GetDefaultChargingTariffRequest>();
                var removeDefaultChargingTariffRequests  = new ConcurrentList<RemoveDefaultChargingTariffRequest>();

                chargingStation2.OnSetDefaultChargingTariffRequest    += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.   TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnGetDefaultChargingTariffRequest    += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.   TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                chargingStation2.OnRemoveDefaultChargingTariffRequest += (timestamp, sender, removeDefaultChargingTariffRequest) => {
                    removeDefaultChargingTariffRequests.TryAdd(removeDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };

                #endregion

                #region Define 1. signed charging tariff

                var providerKeyPair  = KeyPair.GenerateKeys()!;

                var chargingTariff1  = new ChargingTariff(

                                           Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678-1"),
                                           ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           ProviderName:     new DisplayTexts(
                                                                 Languages.en,
                                                                 "GraphDefined EMP"
                                                             ),
                                           Currency:         Currency.EUR,
                                           TariffElements:   new[] {
                                                                 new TariffElement(
                                                                     new[] {
                                                                         PriceComponent.Energy(
                                                                             Price:      0.51M,
                                                                             VAT:        0.02M,
                                                                             StepSize:   1000
                                                                         )
                                                                     }
                                                                     //new TariffRestrictions(
                                                                     //    MaxCurrent:   Ampere.Parse(10)
                                                                     //)
                                                                 )
                                                             },

                                           Created:          timeReference,
                                           Replaces:         null,
                                           References:       null,
                                           TariffType:       TariffType.REGULAR,
                                           Description:      new DisplayTexts(
                                                                 Languages.en,
                                                                 "0.53 / kWh"
                                                             ),
                                           URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           EnergyMix:        null,

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
                                                                 ExcludingVAT:  0.51M,
                                                                 IncludingVAT:  0.53M
                                                             ),
                                           NotBefore:        timeReference,
                                           NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                Assert.IsNotNull(chargingTariff1);


                Assert.IsTrue   (chargingTariff1.Sign(providerKeyPair,
                                                     out var eerr,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff1.Signatures.Any());

                #endregion

                #region Define 2. signed charging tariff

                var chargingTariff2  = new ChargingTariff(

                                           Id:               ChargingTariff_Id.Parse("DE-GDF-T12345678-2"),
                                           ProviderId:       Provider_Id.      Parse("DE-GDF"),
                                           ProviderName:     new DisplayTexts(
                                                                 Languages.en,
                                                                 "GraphDefined EMP"
                                                             ),
                                           Currency:         Currency.EUR,
                                           TariffElements:   new[] {
                                                                 new TariffElement(
                                                                     new[] {
                                                                         PriceComponent.Energy(
                                                                             Price:      0.51M,
                                                                             VAT:        0.02M,
                                                                             StepSize:   1000
                                                                         )
                                                                     }
                                                                     //new TariffRestrictions(
                                                                     //    MaxCurrent:   Ampere.Parse(10)
                                                                     //)
                                                                 )
                                                             },

                                           Created:          timeReference,
                                           Replaces:         null,
                                           References:       null,
                                           TariffType:       TariffType.REGULAR,
                                           Description:      new DisplayTexts(
                                                                 Languages.en,
                                                                 "0.53 / kWh"
                                                             ),
                                           URL:              URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),
                                           EnergyMix:        null,

                                           MinPrice:         null,
                                           MaxPrice:         new Price(
                                                                 ExcludingVAT:  0.51M,
                                                                 IncludingVAT:  0.53M
                                                             ),
                                           NotBefore:        timeReference,
                                           NotAfter:         null,

                                           SignKeys:         null,
                                           SignInfos:        null,
                                           Signatures:       null,

                                           CustomData:       null

                                       );

                Assert.IsNotNull(chargingTariff2);


                Assert.IsTrue   (chargingTariff2.Sign(providerKeyPair,
                                                     out var eerr2,
                                                     "emp1",
                                                     I18NString.Create("Just a signed charging tariff!"),
                                                     timeReference,
                                                     testCSMS01.CustomChargingTariffSerializer,
                                                     testCSMS01.CustomPriceSerializer,
                                                     testCSMS01.CustomTariffElementSerializer,
                                                     testCSMS01.CustomPriceComponentSerializer,
                                                     testCSMS01.CustomTariffRestrictionsSerializer,
                                                     testCSMS01.CustomEnergyMixSerializer,
                                                     testCSMS01.CustomEnergySourceSerializer,
                                                     testCSMS01.CustomEnvironmentalImpactSerializer,
                                                     testCSMS01.CustomSignatureSerializer,
                                                     testCSMS01.CustomCustomDataSerializer));

                Assert.IsTrue   (chargingTariff2.Signatures.Any());

                #endregion


                var response1a       = await testCSMS01.SetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           ChargingTariff:     chargingTariff1,
                                           EVSEIds:            new[] {
                                                                   EVSE_Id.Parse(1)
                                                               },
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response1a.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,                           response1a.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response1a.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response1a.Signatures.First().Name);
                Assert.AreEqual("Just a charging station SetDefaultChargingTariff response!",      response1a.Signatures.First().Description?.FirstText());
                Assert.AreEqual(now2.ToIso8601(),                                                  response1a.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the charging tariff
                Assert.AreEqual(chargingTariff1.Id,                                setDefaultChargingTariffRequests.First().ChargingTariff.Id);
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                                   setDefaultChargingTariffRequests.First().ChargingTariff.Verify(out var errr));
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.First().ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS SetDefaultChargingTariff request!",   setDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response1b       = await testCSMS01.SetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           ChargingTariff:     chargingTariff2,
                                           EVSEIds:            new[] {
                                                                   EVSE_Id.Parse(2)
                                                               },
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response1b.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,                           response1b.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response1b.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response1b.Signatures.First().Name);
                Assert.AreEqual("Just a charging station SetDefaultChargingTariff response!",      response1b.Signatures.First().Description?.FirstText());
                Assert.AreEqual(now2.ToIso8601(),                                                  response1b.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(2,                                                 setDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                               setDefaultChargingTariffRequests.ElementAt(1).ChargingStationId);

                // Verify the signature of the charging tariff
                Assert.AreEqual(chargingTariff2.Id,                                setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Id);
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.Count());
                Assert.IsTrue  (                                                   setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Verify(out var errr2));
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Status);
                Assert.AreEqual("emp1",                                            setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Name);
                Assert.AreEqual("Just a signed charging tariff!",                  setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Description?.FirstText());
                Assert.AreEqual(timeReference.ToIso8601(),                         setDefaultChargingTariffRequests.ElementAt(1).ChargingTariff.Signatures.First().Timestamp?.  ToIso8601());

                // Verify the signature of the request
                Assert.AreEqual(1,                                                 setDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                 setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                Assert.AreEqual("csms001",                                         setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                Assert.AreEqual("Just a CSMS SetDefaultChargingTariff request!",   setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                Assert.AreEqual(now1.ToIso8601(),                                  setDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response2        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response2.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response2.Status);
                Assert.AreEqual(2,                                                                 response2.ChargingTariffs.  Count());
                Assert.AreEqual(2,                                                                 response2.ChargingTariffMap.Count());                     // 2 Charging tariffs...
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.ElementAt(0).Value.Count());  // ...at 1 EVSEs!
                Assert.AreEqual(1,                                                                 response2.ChargingTariffMap.ElementAt(1).Value.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response2.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response2.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response2.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response2.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response3        = await testCSMS01.RemoveDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response3.Result.ResultCode);
                Assert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted,                        response3.Status);
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response3.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response3.Signatures.First().Name);
                Assert.AreEqual("Just a charging station RemoveDefaultChargingTariff response!",   response3.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(8)).ToIso8601(),                      response3.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               removeDefaultChargingTariffRequests.First().ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 removeDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 removeDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         removeDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a CSMS RemoveDefaultChargingTariff request!",                removeDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(8)).ToIso8601(),                      removeDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

                #endregion



                var response4        = await testCSMS01.GetDefaultChargingTariff(
                                           ChargingStationId:  chargingStation2.Id,
                                           CustomData:         null
                                       );

                #region Verify the response

                Assert.AreEqual(ResultCodes.OK,                                                    response4.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,                                            response4.Status);
                Assert.AreEqual(0,                                                                 response4.ChargingTariffs.  Count());
                Assert.AreEqual(0,                                                                 response4.ChargingTariffMap.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 response4.Signatures.First().Status);
                Assert.AreEqual("cs002",                                                           response4.Signatures.First().Name);
                Assert.AreEqual("Just a charging station GetDefaultChargingTariff response!",      response4.Signatures.First().Description?.FirstText());
                Assert.AreEqual((now2 + TimeSpan.FromSeconds(4)).ToIso8601(),                      response4.Signatures.First().Timestamp?.  ToIso8601());

                #endregion

                #region Verify the request at the charging station

                Assert.AreEqual(2,                                                                 getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation2.Id,                                               getDefaultChargingTariffRequests.ElementAt(1).ChargingStationId);

                // Verify the signature of the request
                Assert.AreEqual(1,                                                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,                                 getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Status);
                Assert.AreEqual("csms001",                                                         getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Name);
                Assert.AreEqual("Just a CSMS GetDefaultChargingTariff request!",                   getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Description?.FirstText());
                Assert.AreEqual((now1 + TimeSpan.FromSeconds(4)).ToIso8601(),                      getDefaultChargingTariffRequests.ElementAt(1).Signatures.First().Timestamp?.  ToIso8601());

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
