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
using NUnit.Framework.Legacy;

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

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(networkingNode1);
            ClassicAssert.IsNotNull(testNetworkingNodeWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

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

                testCSMS01.OnBootNotificationRequest             += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation1.SendBootNotification(
                                    BootReason:   reason,
                                    CustomData:   null
                                );

                ClassicAssert.AreEqual (ResultCode.OK,                          response.Result.ResultCode);
                ClassicAssert.AreEqual (RegistrationStatus.Accepted,            response.Status);

                ClassicAssert.AreEqual (1,                                      nnBootNotificationRequests.  Count, "The BootNotification did not reach the networking node!");
                ClassicAssert.AreEqual (chargingStation1.Id,                    nnBootNotificationRequests.  First().NetworkPath.Origin);
                ClassicAssert.AreEqual (reason,                                 nnBootNotificationRequests.  First().Reason);

                ClassicAssert.AreEqual (1,                                      csmsBootNotificationRequests.Count, "The BootNotification did not reach the CSMS!");
                // Expected: GD001
                // But was:  GDNN001
//                ClassicAssert.AreEqual (chargingStation1.Id,                    csmsBootNotificationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual (reason,                                 csmsBootNotificationRequests.First().Reason);

                var chargingStation = csmsBootNotificationRequests.First().ChargingStation;

                ClassicAssert.IsNotNull(chargingStation);
                if (chargingStation is not null)
                {

                    ClassicAssert.AreEqual(chargingStation1.Model,              chargingStation.Model);
                    ClassicAssert.AreEqual(chargingStation1.VendorName,         chargingStation.VendorName);
                    ClassicAssert.AreEqual(chargingStation1.SerialNumber,       chargingStation.SerialNumber);
                    ClassicAssert.AreEqual(chargingStation1.FirmwareVersion,    chargingStation.FirmwareVersion);

                    var modem = chargingStation.Modem;

                    ClassicAssert.IsNotNull(modem);
                    if (modem is not null)
                    {
                        ClassicAssert.AreEqual(chargingStation1.Modem!.ICCID,   modem.ICCID);
                        ClassicAssert.AreEqual(chargingStation1.Modem!.IMSI,    modem.IMSI);
                    }

                }

            }

        }

        #endregion



    }

}
