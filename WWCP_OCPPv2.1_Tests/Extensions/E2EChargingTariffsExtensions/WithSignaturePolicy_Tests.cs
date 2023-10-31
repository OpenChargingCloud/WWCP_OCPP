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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.CS;

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
        /// A test for sending a reset message to a charging station.
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

                #region Define a signed charging tariff

                var providerKeyPair    = KeyPair.GenerateKeys()!;

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

                                             Created:                   timeReference,
                                             Replaces:                  null,
                                             References:                null,
                                             TariffType:                TariffType.REGULAR,
                                             Description:               new DisplayTexts(
                                                                            Languages.en,
                                                                            "0.53 / kWh"
                                                                        ),
                                             URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),

                                             EVSEIds:                   null,
                                             ChargingStationIds:        null,
                                             ChargingPoolIds:           null,

                                             MinPrice:                  null,
                                             MaxPrice:                  new Price(
                                                                            ExcludingVAT:  0.51M,
                                                                            IncludingVAT:  0.53M
                                                                        ),
                                             NotBefore:                 timeReference,
                                             NotAfter:                  null,
                                             EnergyMix:                 null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null

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


                var setDefaultChargingTariffRequests = new ConcurrentList<SetDefaultChargingTariffRequest>();

                chargingStation1.OnSetDefaultChargingTariffRequest += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };


                var now1            = Timestamp.Now;
                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.DefaultJSONLDContext);


                var now2            = Timestamp.Now;
                chargingStation1.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);


                var response        = await testCSMS01.SetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          ChargingTariff:     chargingTariff,
                                          CustomData:         null
                                      );

                Assert.AreEqual(ResultCodes.OK,                            response.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,   response.Status);

                //Assert.IsTrue  (testCSMS01.SignaturePolicy.VerifyResponseMessage(
                //                    response,
                //                    response.ToJSON(
                //                        testCSMS01.CustomSetDefaultChargingTariffResponseSerializer,
                //                        testCSMS01.CustomStatusInfoSerializer,
                //                        testCSMS01.CustomSignatureSerializer,
                //                        testCSMS01.CustomCustomDataSerializer
                //                    ),
                //                    out var errorResponse
                //                ));


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

            }

        }

        #endregion

        #region GetDefaultChargingTariffRequest_Test1()

        /// <summary>
        /// A test for sending a reset message to a charging station.
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

                var getDefaultChargingTariffRequests = new ConcurrentList<GetDefaultChargingTariffRequest>();

                chargingStation1.OnGetDefaultChargingTariffRequest += (timestamp, sender, getDefaultChargingTariffRequest) => {
                    getDefaultChargingTariffRequests.TryAdd(getDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };


                var now1            = Timestamp.Now;
                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffResponse.DefaultJSONLDContext);


                var now2            = Timestamp.Now;
                chargingStation1.SignaturePolicy.AddVerificationRule(GetDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     VerificationRuleActions.VerifyAll);

                chargingStation1.SignaturePolicy.AddSigningRule     (GetDefaultChargingTariffRequest.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);


                var response   = await testCSMS01.GetDefaultChargingTariff(
                                     ChargingStationId:   chargingStation1.Id,
                                     CustomData:          null
                                 );

                Assert.AreEqual(ResultCodes.OK,                response.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,        response.Status);

                Assert.IsTrue  (testCSMS01.SignaturePolicy.VerifyResponseMessage(
                                    response,
                                    response.ToJSON(
                                        testCSMS01.CustomGetDefaultChargingTariffResponseSerializer,
                                        testCSMS01.CustomStatusInfoSerializer,
                                        testCSMS01.CustomChargingTariffSerializer,
                                        testCSMS01.CustomPriceSerializer,
                                        testCSMS01.CustomTariffElementSerializer,
                                        testCSMS01.CustomPriceComponentSerializer,
                                        testCSMS01.CustomTariffRestrictionsSerializer,
                                        testCSMS01.CustomEnergyMixSerializer,
                                        testCSMS01.CustomEnergySourceSerializer,
                                        testCSMS01.CustomEnvironmentalImpactSerializer,
                                        testCSMS01.CustomSignatureSerializer,
                                        testCSMS01.CustomCustomDataSerializer
                                    ),
                                    out var errorResponse
                                ));


                Assert.AreEqual(1,                             getDefaultChargingTariffRequests.Count);
                Assert.AreEqual(chargingStation1.Id,           getDefaultChargingTariffRequests.First().ChargingStationId);
                //Assert.AreEqual(resetType,                     resetRequests.First().GetDefaultChargingTariffType);
                Assert.AreEqual(1,                             getDefaultChargingTariffRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,   getDefaultChargingTariffRequests.First().Signatures.First().Status);
                Assert.AreEqual("ahzf",                        getDefaultChargingTariffRequests.First().Signatures.First().Name);
                Assert.AreEqual("Just a test!",                getDefaultChargingTariffRequests.First().Signatures.First().Description?.FirstText());
                //Assert.AreEqual(now.ToIso8601(),               getDefaultChargingTariffRequests.First().Signatures.First().Timestamp?.  ToIso8601());

            }

        }

        #endregion


        #region SetGetRemoveGet_DefaultChargingTariffRequest_Test1()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task SetGetRemoveGet_DefaultChargingTariffRequest_Test1()
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

                #region Define a signed charging tariff

                var providerKeyPair    = KeyPair.GenerateKeys()!;

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

                                             Created:                   timeReference,
                                             Replaces:                  null,
                                             References:                null,
                                             TariffType:                TariffType.REGULAR,
                                             Description:               new DisplayTexts(
                                                                            Languages.en,
                                                                            "0.53 / kWh"
                                                                        ),
                                             URL:                       URL.Parse("https://open.charging.cloud/emp/tariffs/DE-GDF-T12345678"),

                                             EVSEIds:                   null,
                                             ChargingStationIds:        null,
                                             ChargingPoolIds:           null,

                                             MinPrice:                  null,
                                             MaxPrice:                  new Price(
                                                                            ExcludingVAT:  0.51M,
                                                                            IncludingVAT:  0.53M
                                                                        ),
                                             NotBefore:                 timeReference,
                                             NotAfter:                  null,
                                             EnergyMix:                 null,

                                             SignKeys:                  null,
                                             SignInfos:                 null,
                                             Signatures:                null,

                                             CustomData:                null

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


                var setDefaultChargingTariffRequests = new ConcurrentList<SetDefaultChargingTariffRequest>();

                chargingStation1.OnSetDefaultChargingTariffRequest += (timestamp, sender, setDefaultChargingTariffRequest) => {
                    setDefaultChargingTariffRequests.TryAdd(setDefaultChargingTariffRequest);
                    return Task.CompletedTask;
                };


                var now1            = Timestamp.Now;
                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.      SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffRequest. DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "csms001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test request!"),
                                                                     TimestampGenerator:      (signableMessage) => now1);

                testCSMS01.      SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffResponse.DefaultJSONLDContext);


                var now2            = Timestamp.Now;
                chargingStation1.SignaturePolicy.AddVerificationRule(SetDefaultChargingTariffRequest. DefaultJSONLDContext);

                chargingStation1.SignaturePolicy.AddSigningRule     (SetDefaultChargingTariffResponse.DefaultJSONLDContext,
                                                                     requestKeyPair!,
                                                                     UserIdGenerator:         (signableMessage) => "cs001",
                                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test response!"),
                                                                     TimestampGenerator:      (signableMessage) => now2);


                var response        = await testCSMS01.SetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          ChargingTariff:     chargingTariff,
                                          CustomData:         null
                                      );

                Assert.AreEqual(ResultCodes.OK,                            response.Result.ResultCode);
                Assert.AreEqual(SetDefaultChargingTariffStatus.Accepted,   response.Status);


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



                var response2       = await testCSMS01.GetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          CustomData:         null
                                      );

                Assert.AreEqual(GenericStatus.Accepted, response2.Status);
                Assert.AreEqual(1,                      response2.ChargingTariffs.  Count());
                Assert.AreEqual(1,                      response2.ChargingTariffMap.Count());



                var response3       = await testCSMS01.RemoveDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          CustomData:         null
                                      );

                Assert.AreEqual(RemoveDefaultChargingTariffStatus.Accepted, response3.Status);



                var response4       = await testCSMS01.GetDefaultChargingTariff(
                                          ChargingStationId:  chargingStation1.Id,
                                          CustomData:         null
                                      );

                Assert.AreEqual(GenericStatus.Accepted, response4.Status);
                Assert.AreEqual(0,                      response4.ChargingTariffs.  Count());
                Assert.AreEqual(0,                      response4.ChargingTariffMap.Count());

            }

        }

        #endregion



    }

}
