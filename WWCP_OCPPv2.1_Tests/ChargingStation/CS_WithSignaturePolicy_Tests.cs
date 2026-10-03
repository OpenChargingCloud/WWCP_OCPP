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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Unit tests for charging stations sending sending signed messages
    /// based on a message signature policy to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_WithSignaturePolicy_Tests : AChargingStationTests
    {

        #region Init_Test()

        /// <summary>
        /// A test for creating charging stations.
        /// </summary>
        [Test]
        public void ChargingStation_Init_Test()
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

                Assert.That(chargingStation1.VendorName, Is.EqualTo("GraphDefined OEM #1"));
                Assert.That(chargingStation2.VendorName, Is.EqualTo("GraphDefined OEM #2"));
                Assert.That(chargingStation3.VendorName, Is.EqualTo("GraphDefined OEM #3"));

            }

        }

        #endregion

        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
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

                var bootNotificationRequests = new ConcurrentList<CS.BootNotificationRequest>();

                testCSMS1.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    bootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                var now1                           = Timestamp.Now;
                var keyPair                        = ECCKeyPair.GenerateKeys()!;
                chargingStation1.OCPP.SignaturePolicy.AddSigningRule     (BootNotificationRequest. DefaultJSONLDContext,
                                                                          KeyPair:                 keyPair,
                                                                          UserIdGenerator:         (signableMessage) => "cs001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a charging station test!"),
                                                                          TimestampGenerator:      (signableMessage) => now1);
                chargingStation1.OCPP.SignaturePolicy.AddVerificationRule(BootNotificationResponse.DefaultJSONLDContext,
                                                                          VerificationRuleActions.VerifyAll);

                var now2                           = Timestamp.Now;
                var keyPair2                       = ECCKeyPair.GenerateKeys()!;
                testCSMS1.OCPP.SignaturePolicy.      AddVerificationRule(BootNotificationRequest. DefaultJSONLDContext,
                                                                          VerificationRuleActions.VerifyAll);
                testCSMS1.OCPP.SignaturePolicy.      AddSigningRule     (BootNotificationResponse.DefaultJSONLDContext,
                                                                          keyPair2,
                                                                          UserIdGenerator:         (signableMessage) => "csms001",
                                                                          DescriptionGenerator:    (signableMessage) => I18NString.Create("Just a backend test!"),
                                                                          TimestampGenerator:      (signableMessage) => now2);


                var reason                         = BootReason.PowerUp;
                var response                       = await chargingStation1.SendBootNotification(
                                                         BootReason:   reason,
                                                         CustomData:   null
                                                     );

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status, Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That(response.Signatures.Count(), Is.EqualTo(1));
                Assert.That(response.Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(response.Signatures.First().Name, Is.EqualTo("csms001"));
                Assert.That(response.Signatures.First().Description?.FirstText(), Is.EqualTo("Just a backend test!"));
                Assert.That(response.Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now2.ToISO8601()));

                var chargingStation = bootNotificationRequests.First().ChargingStation;

                Assert.That(chargingStation, Is.Not.Null);
                if (chargingStation is not null)
                {

                    Assert.That(chargingStation.Model, Is.EqualTo(chargingStation1.Model));
                    Assert.That(chargingStation.VendorName, Is.EqualTo(chargingStation1.VendorName));
                    Assert.That(chargingStation.SerialNumber, Is.EqualTo(chargingStation1.SerialNumber));
                    Assert.That(chargingStation.FirmwareVersion, Is.EqualTo(chargingStation1.FirmwareVersion));

                    var modem = chargingStation.Modem;

                    Assert.That(modem, Is.Not.Null);
                    if (modem is not null)
                    {
                        Assert.That(modem.ICCID, Is.EqualTo(chargingStation1.Modem!.ICCID));
                        Assert.That(modem.IMSI, Is.EqualTo(chargingStation1.Modem!.IMSI));
                    }

                }


                Assert.That(bootNotificationRequests.Count, Is.EqualTo(1));
                Assert.That(bootNotificationRequests.First().NetworkPath.Source, Is.EqualTo(chargingStation1.Id));
                Assert.That(bootNotificationRequests.First().Reason, Is.EqualTo(reason));
                Assert.That(bootNotificationRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(bootNotificationRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(bootNotificationRequests.First().Signatures.First().Name, Is.EqualTo("cs001"));
                Assert.That(bootNotificationRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a charging station test!"));
                Assert.That(bootNotificationRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now1.ToISO8601()));

            }

        }

        #endregion


    }

}
