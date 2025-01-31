﻿/*
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for a CSMS sending signed binary data messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region SendBinaryData_Test1()

        /// <summary>
        /// A test for sending a signed binary data to a charging station.
        /// </summary>
        [Test]
        public async Task SendBinaryData_Test1()
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
                                                Destination:      SourceRouting.To(chargingStation1.Id),
                                                ChargingTariff:   (Tariff)chargingTariff,
                                                CustomData:       null
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


    }

}
