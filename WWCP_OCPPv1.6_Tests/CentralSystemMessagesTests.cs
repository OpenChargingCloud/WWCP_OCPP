/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CentralSystemMessagesTests : AChargePointTests
    {

        #region CentralSystem_Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_Reset_Test()
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

                var resetRequests = new List<ResetRequest>();

                chargingStation1.OnResetRequest += async (timestamp, sender, resetRequest) => {
                    resetRequests.Add(resetRequest);
                };

                var resetType  = ResetTypes.Hard;
                var response1  = await testCentralSystem01.Reset(chargingStation1.ChargeBoxId, resetType);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ResetStatus.Accepted,           response1.Status);

                Assert.AreEqual(1,                              resetRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   resetRequests.First().ChargeBoxId);
                Assert.AreEqual(resetType,                      resetRequests.First().ResetType);

            }

        }

        #endregion

        #region CentralSystem_ChangeAvailability_Test()

        /// <summary>
        /// A test for sending a change availability message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeAvailability_Test()
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

                var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

                chargingStation1.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
                    changeAvailabilityRequests.Add(changeAvailabilityRequest);
                };

                var connectorId   = Connector_Id.Parse(1);
                var availability  = Availabilities.Operative;
                var response1     = await testCentralSystem01.ChangeAvailability(chargingStation1.ChargeBoxId,
                                                                                 connectorId,
                                                                                 availability);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(AvailabilityStatus.Accepted,    response1.Status);

                Assert.AreEqual(1,                              changeAvailabilityRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   changeAvailabilityRequests.First().ChargeBoxId);
                Assert.AreEqual(connectorId,                    changeAvailabilityRequests.First().ConnectorId);
                Assert.AreEqual(availability,                   changeAvailabilityRequests.First().Availability);

            }

        }

        #endregion


        #region CentralSystem_GetConfiguration_All_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_All_Test()
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

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargingStation1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargingStation1.ChargeBoxId);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(3,                              response1.ConfigurationKeys.Count());
                Assert.AreEqual(0,                              response1.UnknownKeys.      Count());

                Assert.AreEqual(1,                              getConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getConfigurationRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region CentralSystem_GetConfiguration_ValidKey_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_ValidKey_Test()
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

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargingStation1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargingStation1.ChargeBoxId,
                                                                           new String[] { "hello" });

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(1,                              response1.ConfigurationKeys.Count());
                Assert.AreEqual(0,                              response1.UnknownKeys.      Count());

                Assert.AreEqual(1,                              getConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getConfigurationRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region CentralSystem_GetConfiguration_UnknownKeys_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_UnknownKeys_Test()
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

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargingStation1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargingStation1.ChargeBoxId,
                                                                           new String[] { "ABCD", "BCDE" });

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(0,                              response1.ConfigurationKeys.Count());
                Assert.AreEqual(2,                              response1.UnknownKeys.      Count());

                Assert.AreEqual(1,                              getConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getConfigurationRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region CentralSystem_ChangeConfiguration_NoOverwrite_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_NoOverwrite_Test()
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

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargingStation1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "hello";
                var value      = "world!!!";
                var response1  = await testCentralSystem01.ChangeConfiguration(chargingStation1.ChargeBoxId,
                                                                               key,
                                                                               value);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ConfigurationStatus.Rejected,   response1.Status);

                Assert.AreEqual(1,                              changeConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   changeConfigurationRequests.First().ChargeBoxId);
                Assert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                Assert.AreEqual(value,                          changeConfigurationRequests.First().Value);

            }

        }

        #endregion

        #region CentralSystem_ChangeConfiguration_Overwrite_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_Overwrite_Test()
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

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargingStation1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "changeMe";
                var value      = "now!!!";
                var response1  = await testCentralSystem01.ChangeConfiguration(chargingStation1.ChargeBoxId,
                                                                               key,
                                                                               value);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ConfigurationStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                              changeConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   changeConfigurationRequests.First().ChargeBoxId);
                Assert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                Assert.AreEqual(value,                          changeConfigurationRequests.First().Value);


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargingStation1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response2 = await testCentralSystem01.GetConfiguration(chargingStation1.ChargeBoxId,
                                                                           new String[] { key });

                Assert.AreEqual(ResultCodes.OK,                 response2.Result.ResultCode);
                Assert.AreEqual(1,                              response2.ConfigurationKeys.Count());
                Assert.AreEqual(0,                              response2.UnknownKeys.      Count());
                Assert.AreEqual(value,                          response2.ConfigurationKeys.First().Value);

                Assert.AreEqual(1,                              getConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getConfigurationRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region CentralSystem_ChangeConfiguration_ReadOnly_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_ReadOnly_Test()
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

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargingStation1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "hello";
                var value      = "hell";
                var response1  = await testCentralSystem01.ChangeConfiguration(chargingStation1.ChargeBoxId,
                                                                               key,
                                                                               value);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ConfigurationStatus.Rejected,   response1.Status);

                Assert.AreEqual(1,                              changeConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   changeConfigurationRequests.First().ChargeBoxId);
                Assert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                Assert.AreEqual(value,                          changeConfigurationRequests.First().Value);


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargingStation1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response2 = await testCentralSystem01.GetConfiguration(chargingStation1.ChargeBoxId,
                                                                           new String[] { key });

                Assert.AreEqual(ResultCodes.OK,                 response2.Result.ResultCode);
                Assert.AreEqual(1,                              response2.ConfigurationKeys.Count());
                Assert.AreEqual(0,                              response2.UnknownKeys.      Count());
                Assert.AreEqual("world",                        response2.ConfigurationKeys.First().Value);

                Assert.AreEqual(1,                              getConfigurationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getConfigurationRequests.First().ChargeBoxId);

            }

        }

        #endregion

    }

}
