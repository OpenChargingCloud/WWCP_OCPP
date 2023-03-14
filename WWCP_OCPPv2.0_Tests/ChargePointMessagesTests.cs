/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{

    /// <summary>
    /// Unit tests for charge points sending messages to the central system.
    /// </summary>
    [TestFixture]
    public class ChargePointMessagesTests : AChargePointTests
    {

        #region ChargePoint_Init_Test()

        /// <summary>
        /// A test for creating charge points.
        /// </summary>
        [Test]
        public void ChargePoint_Init_Test()
        {

            Assert.IsNotNull(testCentralSystem01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.ChargingStationVendor);
                Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.ChargingStationVendor);
                Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.ChargingStationVendor);

            }

        }

        #endregion

        #region ChargePoint_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendBootNotifications_Test()
        {

            Assert.IsNotNull(testCentralSystem01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var bootNotificationRequests = new List<CP.BootNotificationRequest>();

                testCentralSystem01.OnBootNotificationRequest += async (timestamp, sender, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                };

                var response1 = await chargingStation1.SendBootNotification(BootReasons.PowerUp);

                Assert.AreEqual(ResultCodes.OK,                            response1.Result.ResultCode);
                Assert.AreEqual(RegistrationStatus.Accepted,               response1.Status);

                Assert.AreEqual(1,                                         bootNotificationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,              bootNotificationRequests.First().ChargeBoxId);
                //Assert.AreEqual(chargingStation1.ChargePointVendor,        bootNotificationRequests.First().ChargePointVendor);
                //Assert.AreEqual(chargingStation1.ChargePointSerialNumber,  bootNotificationRequests.First().ChargePointSerialNumber);
                //Assert.AreEqual(chargingStation1.ChargeBoxSerialNumber,    bootNotificationRequests.First().ChargeBoxSerialNumber);
                //Assert.AreEqual(chargingStation1.Iccid,                    bootNotificationRequests.First().Iccid);
                //Assert.AreEqual(chargingStation1.IMSI,                     bootNotificationRequests.First().IMSI);
                //Assert.AreEqual(chargingStation1.MeterType,                bootNotificationRequests.First().MeterType);
                //Assert.AreEqual(chargingStation1.MeterSerialNumber,        bootNotificationRequests.First().MeterSerialNumber);

            }

        }

        #endregion

        #region ChargePoint_SendSendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendSendHeartbeats_Test()
        {

            Assert.IsNotNull(testCentralSystem01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var heartbeatRequests = new List<CP.HeartbeatRequest>();

                testCentralSystem01.OnHeartbeatRequest += async (timestamp, sender, heartbeatRequest) => {
                    heartbeatRequests.Add(heartbeatRequest);
                };


                var response1 = await chargingStation1.SendHeartbeat();


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.IsTrue  (Timestamp.Now - response1.CurrentTime < TimeSpan.FromSeconds(10));

                Assert.AreEqual(1,                              heartbeatRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   heartbeatRequests.First().ChargeBoxId);

            }

        }

        #endregion


    }

}
