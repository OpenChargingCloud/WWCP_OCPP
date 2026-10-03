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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a CSMS sending signed messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_SignedMessages_Tests : AChargingStationTests
    {

        #region Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_Test()
        {

            Assert.That(testCSMS1, Is.Not.Null);
            Assert.That(testBackendWebSockets1, Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);
            Assert.That(chargingStation2, Is.Not.Null);
            Assert.That(chargingStation3, Is.Not.Null);

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(ResetRequest.DefaultJSONLDContext,
                                                                          VerificationRuleActions.VerifyAll);


                var keyPair    = ECCKeyPair.GenerateKeys()!;

                var resetType  = ResetType.Immediate;
                var now        = Timestamp.Now;
                var response   = await testCSMS1.Reset(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           ResetType:    resetType,
                                           SignInfos:    [
                                                             keyPair.ToSignInfo1(
                                                                         Name:         "ahzf",
                                                                         Description:   I18NString.Create("Just a test!"),
                                                                         Timestamp:     now
                                                                     )
                                                         ],
                                           CustomData:   null
                                       );

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status, Is.EqualTo(ResetStatus.Accepted));

                Assert.That(testCSMS1.OCPP.SignaturePolicy.VerifyResponseMessage(
                                           response,
                                           response.ToJSON(
                                               true,
                                               testCSMS1.OCPP.CustomResetResponseSerializer,
                                               testCSMS1.OCPP.CustomStatusInfoSerializer,
                                               testCSMS1.OCPP.CustomSignatureSerializer,
                                               testCSMS1.OCPP.CustomCustomDataSerializer
                                           ),
                                           out var errorResponse
                                       ), Is.True);


                Assert.That(resetRequests.Count, Is.EqualTo(1));
                Assert.That(resetRequests.First().DestinationId, Is.EqualTo(chargingStation1.Id));
                Assert.That(resetRequests.First().ResetType, Is.EqualTo(resetType));
                Assert.That(resetRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(resetRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(resetRequests.First().Signatures.First().Name, Is.EqualTo("ahzf"));
                Assert.That(resetRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a test!"));
                Assert.That(resetRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now.ToISO8601()));

            }

        }

        #endregion


    }

}
