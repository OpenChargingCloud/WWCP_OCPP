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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Unit tests for charging stations sending sending signed messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_SignedMessages_Tests : AChargingStationTests
    {

        #region Init_Test()

        /// <summary>
        /// A test for creating charging stations.
        /// </summary>
        [Test]
        public void ChargingStation_Init_Test()
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

                var bootNotificationRequests = new ConcurrentList<CS.BootNotificationRequest>();

                testCSMS1.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    bootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };


                testCSMS1.OCPP.SignaturePolicy.AddVerificationRule(BootNotificationRequest.DefaultJSONLDContext,
                                                                    VerificationRuleActions.VerifyAll);

                var responseKeyPair          = ECCKeyPair.GenerateKeys()!;
                testCSMS1.OCPP.SignaturePolicy.AddSigningRule(BootNotificationResponse.DefaultJSONLDContext, responseKeyPair);


                var requestKeyPair           = ECCKeyPair.GenerateKeys()!;
                var reason                   = BootReason.PowerUp;
                var now                      = Timestamp.Now;
                var response1                = await chargingStation1.SendBootNotification(
                                                         BootReason:   reason,
                                                         SignInfos:    new[] {
                                                                           requestKeyPair.ToSignInfo1(
                                                                               "ahzf",
                                                                               I18NString.Create("Just a test!"),
                                                                               now
                                                                           )
                                                                       },
                                                         CustomData:   null
                                                     );

                Assert.That(response1.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response1.Status, Is.EqualTo(RegistrationStatus.Accepted));


                Assert.That(bootNotificationRequests.Count, Is.EqualTo(1));

                // A BootNotification travels from the station to the CSMS, so the station is
                // its source, not its destination. A standard-mode message carries no
                // destination at all, and the receiving server fills in NetworkingNode_Id.CSMS.
                Assert.That(bootNotificationRequests.First().DestinationId, Is.EqualTo(NetworkingNode_Id.CSMS));
                Assert.That(bootNotificationRequests.First().NetworkPath.Length, Is.EqualTo(1));
                Assert.That(bootNotificationRequests.First().NetworkPath.Source, Is.EqualTo(chargingStation1.Id));
                Assert.That(bootNotificationRequests.First().Reason, Is.EqualTo(reason));
                Assert.That(bootNotificationRequests.First().Signatures.Count(), Is.EqualTo(1));
                Assert.That(bootNotificationRequests.First().Signatures.First().Status, Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(bootNotificationRequests.First().Signatures.First().Name, Is.EqualTo("ahzf"));
                Assert.That(bootNotificationRequests.First().Signatures.First().Description?.FirstText(), Is.EqualTo("Just a test!"));
                Assert.That(bootNotificationRequests.First().Signatures.First().Timestamp?.ToISO8601(), Is.EqualTo(now.ToISO8601()));

                var chargingStation = bootNotificationRequests.First().ChargingStation;

                ClassicAssert.IsNotNull(chargingStation);
                if (chargingStation is not null)
                {

                    Assert.That(chargingStation.Model, Is.EqualTo(chargingStation1.Model));
                    Assert.That(chargingStation.VendorName, Is.EqualTo(chargingStation1.VendorName));
                    Assert.That(chargingStation.SerialNumber, Is.EqualTo(chargingStation1.SerialNumber));
                    Assert.That(chargingStation.FirmwareVersion, Is.EqualTo(chargingStation1.FirmwareVersion));

                    var modem = chargingStation.Modem;

                    ClassicAssert.IsNotNull(modem);
                    if (modem is not null)
                    {
                        Assert.That(modem.ICCID, Is.EqualTo(chargingStation1.Modem!.ICCID));
                        Assert.That(modem.IMSI, Is.EqualTo(chargingStation1.Modem!.IMSI));
                    }

                }

            }

        }

        #endregion


    }

}
