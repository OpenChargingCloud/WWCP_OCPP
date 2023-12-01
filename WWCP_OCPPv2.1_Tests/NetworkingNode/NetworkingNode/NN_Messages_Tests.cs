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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.NN
{

    /// <summary>
    /// Unit tests for networking nodes sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class NN_Messages_Tests : AChargingStationWithNetworkingNodeTests
    {

        #region Init_Test()

        ///// <summary>
        ///// A test for creating charging stations.
        ///// </summary>
        //[Test]
        //public void ChargingStation_Init_Test()
        //{

        //    Assert.IsNotNull(testCSMS01);
        //    Assert.IsNotNull(testBackendWebSockets01);
        //    Assert.IsNotNull(chargingStation1);
        //    Assert.IsNotNull(chargingStation2);
        //    Assert.IsNotNull(chargingStation3);

        //    if (testCSMS01              is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
        //        Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
        //        Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(networkingNode1);
            Assert.IsNotNull(testNetworkingNodeWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01                     is not null &&
                testBackendWebSockets01        is not null &&
                networkingNode1                is not null &&
                testNetworkingNodeWebSockets01 is not null &&
                chargingStation1               is not null &&
                chargingStation2               is not null &&
                chargingStation3               is not null)
            {

                var csmsBootNotificationRequests  = new ConcurrentList<BootNotificationRequest>();

                testCSMS01.OnBootNotificationRequest += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await networkingNode1.AsCS.SendBootNotification(
                                    new BootNotificationRequest(
                                        networkingNode1.Id,
                                        new OCPPv2_1.ChargingStation(
                                            networkingNode1.Model,
                                            networkingNode1.VendorName,
                                            networkingNode1.SerialNumber,
                                            networkingNode1.Modem,
                                            networkingNode1.FirmwareVersion,
                                            networkingNode1.CustomData
                                        ),
                                        BootReason.PowerUp
                                    )
                                );

                Assert.AreEqual (ResultCode.OK,                          response.Result.ResultCode);
                Assert.AreEqual (RegistrationStatus.Accepted,            response.Status);

                Assert.AreEqual (1,                                      csmsBootNotificationRequests.Count, "The BootNotification did not reach the CSMS!");
                Assert.AreEqual (networkingNode1.Id.ToString(),          csmsBootNotificationRequests.First().NetworkPath.Origin.ToString());
                Assert.AreEqual (reason,                                 csmsBootNotificationRequests.First().Reason);

                var chargingStation = csmsBootNotificationRequests.First().ChargingStation;

                Assert.IsNotNull(chargingStation);
                if (chargingStation is not null)
                {

                    Assert.AreEqual(chargingStation1.Model,              chargingStation.Model);
                    Assert.AreEqual(chargingStation1.VendorName,         chargingStation.VendorName);
                    Assert.AreEqual(chargingStation1.SerialNumber,       chargingStation.SerialNumber);
                    Assert.AreEqual(chargingStation1.FirmwareVersion,    chargingStation.FirmwareVersion);

                    var modem = chargingStation.Modem;

                    Assert.IsNotNull(modem);
                    if (modem is not null)
                    {
                        Assert.AreEqual(chargingStation1.Modem!.ICCID,   modem.ICCID);
                        Assert.AreEqual(chargingStation1.Modem!.IMSI,    modem.IMSI);
                    }

                }

            }

        }

        #endregion



    }

}
