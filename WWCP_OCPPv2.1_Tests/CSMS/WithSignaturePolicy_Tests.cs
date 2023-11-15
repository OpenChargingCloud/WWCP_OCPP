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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a central system sending signed messages
    /// based on a message signature policy to charging stations.
    /// </summary>
    [TestFixture]
    public class WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region AddSignaturePolicy_Test()

        /// <summary>
        /// A test for setting a SignaturePolicy at the charging station.
        /// </summary>
        [Test]
        public async Task AddSignaturePolicy_Test()
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

                var setSignaturePolicyRequests = new ConcurrentList<AddSignaturePolicyRequest>();

                ////testCSMS01.OnAddSignaturePolicyRequest += (timestamp, sender, connection, setSignaturePolicyRequest) => {
                ////    setSignaturePolicyRequests.TryAdd(setSignaturePolicyRequest);
                ////    return Task.CompletedTask;
                ////};

                //var now1                           = Timestamp.Now;
                //var keyPair                        = KeyPair.GenerateKeys()!;
                //chargingStation1.SignaturePolicy.AddSigningRule     (BootNotificationRequest. DefaultJSONLDContext,
                //                                                     KeyPair:                 keyPair,
                //                                                     UserIdGenerator:         (signableMessage) => "cs001",
                //                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test!"),
                //                                                     TimestampGenerator:      (signableMessage) => now1);
                //chargingStation1.SignaturePolicy.AddVerificationRule(BootNotificationResponse.DefaultJSONLDContext,
                //                                                     VerificationRuleAction.VerifyAll);

                //var now2                           = Timestamp.Now;
                //var keyPair2                       = KeyPair.GenerateKeys()!;
                //testCSMS01.SignaturePolicy.      AddVerificationRule(BootNotificationRequest. DefaultJSONLDContext,
                //                                                     VerificationRuleAction.VerifyAll);
                //testCSMS01.SignaturePolicy.      AddSigningRule     (BootNotificationResponse.DefaultJSONLDContext,
                //                                                     keyPair2,
                //                                                     UserIdGenerator:         (signableMessage) => "csms001",
                //                                                     DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test!"),
                //                                                     TimestampGenerator:      (signableMessage) => now2);

                var signaturePolicy                = new SignaturePolicy();
                ////var response                       = await chargingStation1.AddSignaturePolicy(
                ////                                         SignaturePolicy:   signaturePolicy,
                ////                                         CustomData:        null
                ////                                     );

                ////Assert.AreEqual(ResultCodes.OK,                          response.Result.ResultCode);
                ////Assert.AreEqual(RegistrationStatus.Accepted,             response.Status);
                //Assert.AreEqual(1,                                       response.Signatures.Count());
                //Assert.AreEqual(VerificationStatus.ValidSignature,       response.Signatures.First().Status);
                //Assert.AreEqual("csms001",                               response.Signatures.First().Name);
                //Assert.AreEqual("Just a backend test!",                  response.Signatures.First().Description?.FirstText());
                //Assert.AreEqual(now2.ToIso8601(),                        response.Signatures.First().Timestamp?.  ToIso8601());

                //Assert.AreEqual(1,                                       setSignaturePolicyRequests.Count);
                //Assert.AreEqual(chargingStation1.Id,                     setSignaturePolicyRequests.First().ChargingStationId);
                //Assert.AreEqual(reason,                                  bootNotificationRequests.First().Reason);
                //Assert.AreEqual(1,                                       bootNotificationRequests.First().Signatures.Count());
                //Assert.AreEqual(VerificationStatus.ValidSignature,             bootNotificationRequests.First().Signatures.First().Status);
                //Assert.AreEqual("cs001",                                 bootNotificationRequests.First().Signatures.First().Name);
                //Assert.AreEqual("Just a charging station test!",         bootNotificationRequests.First().Signatures.First().Description?.FirstText());
                //Assert.AreEqual(now1.ToIso8601(),                        bootNotificationRequests.First().Signatures.First().Timestamp?.  ToIso8601());

            }

        }

        #endregion




        #region Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OnResetRequest += (timestamp, sender, resetRequest) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.SignaturePolicy.AddVerificationRule(ResetRequest.DefaultJSONLDContext,
                                                                     VerificationRuleActions.VerifyAll);


                var requestKeyPair  = KeyPair.GenerateKeys()!;
                testCSMS01.SignaturePolicy.AddSigningRule     (ResetRequest. DefaultJSONLDContext, requestKeyPair!);
                testCSMS01.SignaturePolicy.AddVerificationRule(ResetResponse.DefaultJSONLDContext);


                var resetType       = ResetType.Immediate;
                var now             = Timestamp.Now;
                var response        = await testCSMS01.Reset(
                                          ChargingStationId:   chargingStation1.Id,
                                          ResetType:           resetType,
                                          CustomData:          null
                                      );

                Assert.AreEqual(ResultCodes.OK,                response.Result.ResultCode);
                Assert.AreEqual(ResetStatus.Accepted,          response.Status);


                Assert.AreEqual(1,                             resetRequests.Count);
                Assert.AreEqual(chargingStation1.Id,           resetRequests.First().ChargingStationId);
                Assert.AreEqual(resetType,                     resetRequests.First().ResetType);
                Assert.AreEqual(1,                             resetRequests.First().Signatures.Count());
                Assert.AreEqual(VerificationStatus.ValidSignature,   resetRequests.First().Signatures.First().Status);
                //Assert.AreEqual("ahzf",                        resetRequests.First().Signatures.First().Name);
                //Assert.AreEqual("Just a test!",                resetRequests.First().Signatures.First().Description?.FirstText());
                //Assert.AreEqual(now.ToIso8601(),               resetRequests.First().Signatures.First().Timestamp?.  ToIso8601());

            }

        }

        #endregion


    }

}
