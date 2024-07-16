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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a CSMS sending signed messages
    /// based on a message signature policy to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region AddSignaturePolicy_Test()

        /// <summary>
        /// A test for setting a SignaturePolicy at the charging station.
        /// </summary>
        [Test]
        public async Task AddSignaturePolicy_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

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

                ////ClassicAssert.AreEqual(ResultCodes.OK,                          response.Result.ResultCode);
                ////ClassicAssert.AreEqual(RegistrationStatus.Accepted,             response.Status);
                //ClassicAssert.AreEqual(1,                                       response.Signatures.Count());
                //ClassicAssert.AreEqual(VerificationStatus.ValidSignature,       response.Signatures.First().Status);
                //ClassicAssert.AreEqual("csms001",                               response.Signatures.First().Name);
                //ClassicAssert.AreEqual("Just a backend test!",                  response.Signatures.First().Description?.FirstText());
                //ClassicAssert.AreEqual(now2.ToIso8601(),                        response.Signatures.First().Timestamp?.  ToIso8601());

                //ClassicAssert.AreEqual(1,                                       setSignaturePolicyRequests.Count);
                //ClassicAssert.AreEqual(chargingStation1.Id,                     setSignaturePolicyRequests.First().NetworkingNodeId);
                //ClassicAssert.AreEqual(reason,                                  bootNotificationRequests.First().Reason);
                //ClassicAssert.AreEqual(1,                                       bootNotificationRequests.First().Signatures.Count());
                //ClassicAssert.AreEqual(VerificationStatus.ValidSignature,             bootNotificationRequests.First().Signatures.First().Status);
                //ClassicAssert.AreEqual("cs001",                                 bootNotificationRequests.First().Signatures.First().Name);
                //ClassicAssert.AreEqual("Just a charging station test!",         bootNotificationRequests.First().Signatures.First().Description?.FirstText());
                //ClassicAssert.AreEqual(now1.ToIso8601(),                        bootNotificationRequests.First().Signatures.First().Timestamp?.  ToIso8601());

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

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest) => {
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
                                                DestinationId:   chargingStation1.Id,
                                                ResetType:       resetType,
                                                CustomData:      null
                                            );

                ClassicAssert.AreEqual(ResultCode.OK,                 response.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Accepted,          response.Status);


                ClassicAssert.AreEqual(1,                             resetRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,           resetRequests.First().DestinationId);
                ClassicAssert.AreEqual(resetType,                     resetRequests.First().ResetType);
                ClassicAssert.AreEqual(1,                             resetRequests.First().Signatures.Count());
                ClassicAssert.AreEqual(VerificationStatus.ValidSignature,   resetRequests.First().Signatures.First().Status);
                //ClassicAssert.AreEqual("ahzf",                        resetRequests.First().Signatures.First().Name);
                //ClassicAssert.AreEqual("Just a test!",                resetRequests.First().Signatures.First().Description?.FirstText());
                //ClassicAssert.AreEqual(now.ToIso8601(),               resetRequests.First().Signatures.First().Timestamp?.  ToIso8601());

            }

        }

        #endregion


    }

}
