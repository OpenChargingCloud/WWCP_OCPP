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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.CS
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_Messages_Tests : AChargingStationWithNetworkingNodeTests
    {

        #region Init_Test()

        ///// <summary>
        ///// A test for creating charging stations.
        ///// </summary>
        //[Test]
        //public void ChargingStation_Init_Test()
        //{

        //    ClassicAssert.IsNotNull(testCSMS01);
        //    ClassicAssert.IsNotNull(testBackendWebSockets01);
        //    ClassicAssert.IsNotNull(chargingStation1);
        //    ClassicAssert.IsNotNull(chargingStation2);
        //    ClassicAssert.IsNotNull(chargingStation3);

        //    if (testCSMS01              is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        ClassicAssert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
        //        ClassicAssert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
        //        ClassicAssert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

        //    }

        //}

        #endregion


        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS01,                       Is.Not.Null);
                Assert.That(testBackendWebSockets01,          Is.Not.Null);
                Assert.That(networkingNode1,                  Is.Not.Null);
                Assert.That(testNetworkingNodeWebSockets01,   Is.Not.Null);
                Assert.That(chargingStation1,                 Is.Not.Null);
                Assert.That(chargingStation2,                 Is.Not.Null);
                Assert.That(chargingStation3,                 Is.Not.Null);
            });

            if (testCSMS01                     is not null &&
                testBackendWebSockets01        is not null &&
                networkingNode1                is not null &&
                testNetworkingNodeWebSockets01 is not null &&
                chargingStation1               is not null &&
                chargingStation2               is not null &&
                chargingStation3               is not null)
            {

                var nnBootNotificationRequests    = new ConcurrentList<BootNotificationRequest>();
                var csmsBootNotificationRequests  = new ConcurrentList<BootNotificationRequest>();

                networkingNode1.AsCSMS.OnBootNotificationRequest += (timestamp, sender, connection, bootNotificationRequest) => {
                    nnBootNotificationRequests.  TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                testCSMS01.            OnBootNotificationRequest += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.NetworkingMode = OCPP.WebSockets.NetworkingMode.NetworkingExtensions;


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation1.SendBootNotification(
                                    BootReason:   reason,
                                    CustomData:   null
                                );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                           Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(nnBootNotificationRequests.  Count,                        Is.EqualTo(1), "The BootNotification did not reach the networking node!");
                    Assert.That(nnBootNotificationRequests.  First().DestinationNodeId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequests.  First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequests.  First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBootNotificationRequests.  First().NetworkPath.Last,     Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBootNotificationRequests.  First().Reason,               Is.EqualTo(reason));

                    Assert.That(csmsBootNotificationRequests.Count,                        Is.EqualTo(1), "The BootNotification did not reach the CSMS!");
                    Assert.That(csmsBootNotificationRequests.First().DestinationNodeId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequests.First().NetworkPath.Length,   Is.EqualTo(2));
                    Assert.That(csmsBootNotificationRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(csmsBootNotificationRequests.First().NetworkPath.Last,     Is.EqualTo(networkingNode1. Id));
                    Assert.That(csmsBootNotificationRequests.First().Reason,               Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequests.  First().ChargingStation,      Is.Not.Null);

                });

                var chargingStation = csmsBootNotificationRequests.First().ChargingStation;
                if (chargingStation is not null)
                {

                    Assert.That(chargingStation.Model,             Is.EqualTo(chargingStation1.Model));
                    Assert.That(chargingStation.VendorName,        Is.EqualTo(chargingStation1.VendorName));
                    Assert.That(chargingStation.SerialNumber,      Is.EqualTo(chargingStation1.SerialNumber));
                    Assert.That(chargingStation.FirmwareVersion,   Is.EqualTo(chargingStation1.FirmwareVersion));
                    Assert.That(chargingStation.Modem,             Is.Not.Null);

                    if (chargingStation. Modem is not null &&
                        chargingStation1.Modem is not null)
                    {
                        Assert.That(chargingStation.Modem.ICCID,   Is.EqualTo(chargingStation1.Modem.ICCID));
                        Assert.That(chargingStation.Modem.IMSI,    Is.EqualTo(chargingStation1.Modem.IMSI));
                    }

                }

            }

        }

        #endregion



    }

}
