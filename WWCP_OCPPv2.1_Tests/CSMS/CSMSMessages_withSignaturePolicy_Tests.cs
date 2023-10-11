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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests
{

    /// <summary>
    /// Unit tests for a central system sending signed messages
    /// based on a message signature policy to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMSMessages_withSignaturePolicy_Tests : AChargingStationTests
    {

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

                var keyPair     = KeyPair.GenerateKeys()!;

                testCSMS01.SignaturePolicy = new SignaturePolicy(
                                                 //SignaturePolicyAction.sign,
                                                 //KeyPair.GenerateKeys()
                                             );

                testCSMS01.SignaturePolicy.AddSigningRule     (ResetRequest. DefaultJSONLDContext, keyPair!);
                testCSMS01.SignaturePolicy.AddVerificationRule(ResetResponse.DefaultJSONLDContext);


                var resetType  = ResetTypes.Immediate;
                var now        = Timestamp.Now;
                var response   = await testCSMS01.Reset(
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
                Assert.AreEqual(VerificationStatus.Verified,   resetRequests.First().Signatures.First().Status);
                //Assert.AreEqual("ahzf",                        resetRequests.First().Signatures.First().Name);
                //Assert.AreEqual("Just a test!",                resetRequests.First().Signatures.First().Description?.FirstText());
                //Assert.AreEqual(now.ToIso8601(),               resetRequests.First().Signatures.First().Timestamp?.  ToIso8601());

            }

        }

        #endregion


    }

}
