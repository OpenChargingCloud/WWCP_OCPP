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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CS_Messages_Tests : AChargePointTests
    {

        #region CentralSystem_Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_Reset_Test()
        {

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var resetRequests = new List<ResetRequest>();

                chargePoint1.OnResetRequest += async (timestamp, sender, resetRequest) => {
                    resetRequests.Add(resetRequest);
                };

                var resetType  = ResetTypes.Hard;
                var response1  = await testCentralSystem01.Reset(chargePoint1.Id, resetType);

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Accepted,           response1.Status);

                ClassicAssert.AreEqual(1,                              resetRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   resetRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(resetType,                      resetRequests.First().ResetType);

            }

        }

        #endregion

        #region CentralSystem_Reset_UnknownChargeBox_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_Reset_UnknownChargeBox_Test()
        {

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var resetRequests = new List<ResetRequest>();

                chargePoint2.OnResetRequest += async (timestamp, sender, resetRequest) => {
                    resetRequests.Add(resetRequest);
                };

                var resetType  = ResetTypes.Hard;
                var response1  = await testCentralSystem01.Reset(chargePoint2.Id, resetType);

                ClassicAssert.AreEqual  (OCPP.ResultCode.NetworkError,   response1.Result.ResultCode);
                ClassicAssert.IsNotEmpty(                                response1.Result.Description);
                ClassicAssert.AreEqual  (ResetStatus.Unknown,            response1.Status);

                ClassicAssert.AreEqual  (0,                              resetRequests.Count);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

                chargePoint1.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
                    changeAvailabilityRequests.Add(changeAvailabilityRequest);
                };

                var connectorId   = Connector_Id.Parse(1);
                var availability  = Availabilities.Operative;
                var response1     = await testCentralSystem01.ChangeAvailability(chargePoint1.Id,
                                                                                 connectorId,
                                                                                 availability);

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(AvailabilityStatus.Accepted,    response1.Status);

                ClassicAssert.AreEqual(1,                              changeAvailabilityRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   changeAvailabilityRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(connectorId,                    changeAvailabilityRequests.First().ConnectorId);
                ClassicAssert.AreEqual(availability,                   changeAvailabilityRequests.First().Availability);

            }

        }

        #endregion

        #region CentralSystem_ChangeAvailability_UnknownChargeBox_Test()

        /// <summary>
        /// A test for sending a change availability message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeAvailability_UnknownChargeBox_Test()
        {

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

                chargePoint2.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
                    changeAvailabilityRequests.Add(changeAvailabilityRequest);
                };

                var connectorId   = Connector_Id.Parse(1);
                var availability  = Availabilities.Operative;
                var response1     = await testCentralSystem01.ChangeAvailability(chargePoint2.Id,
                                                                                 connectorId,
                                                                                 availability);

                ClassicAssert.AreEqual  (OCPP.ResultCode.NetworkError,   response1.Result.ResultCode);
                ClassicAssert.IsNotEmpty(                                response1.Result.Description);
                ClassicAssert.AreEqual  (AvailabilityStatus.Unknown,     response1.Status);

                ClassicAssert.AreEqual  (0,                              changeAvailabilityRequests.Count);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargePoint1.Id);

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(4,                              response1.ConfigurationKeys.Count());
                ClassicAssert.AreEqual(0,                              response1.UnknownKeys.      Count());

                ClassicAssert.AreEqual(1,                              getConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   getConfigurationRequests.First().NetworkingNodeId);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargePoint1.Id,
                                                                           new String[] { "hello" });

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(1,                              response1.ConfigurationKeys.Count());
                ClassicAssert.AreEqual(0,                              response1.UnknownKeys.      Count());

                ClassicAssert.AreEqual(1,                              getConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   getConfigurationRequests.First().NetworkingNodeId);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response1 = await testCentralSystem01.GetConfiguration(chargePoint1.Id,
                                                                           new String[] { "ABCD", "BCDE" });

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(0,                              response1.ConfigurationKeys.Count());
                ClassicAssert.AreEqual(2,                              response1.UnknownKeys.      Count());

                ClassicAssert.AreEqual(1,                              getConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   getConfigurationRequests.First().NetworkingNodeId);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "hello";
                var value      = "world!!!";
                var response1  = await testCentralSystem01.ChangeConfiguration(chargePoint1.Id,
                                                                               key,
                                                                               value);

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(ConfigurationStatus.Rejected,   response1.Status);

                ClassicAssert.AreEqual(1,                              changeConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   changeConfigurationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                ClassicAssert.AreEqual(value,                          changeConfigurationRequests.First().Value);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "changeMe";
                var value      = "now!!!";
                var response1  = await testCentralSystem01.ChangeConfiguration(chargePoint1.Id,
                                                                               key,
                                                                               value);

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(ConfigurationStatus.Accepted,   response1.Status);

                ClassicAssert.AreEqual(1,                              changeConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   changeConfigurationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                ClassicAssert.AreEqual(value,                          changeConfigurationRequests.First().Value);


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response2 = await testCentralSystem01.GetConfiguration(chargePoint1.Id,
                                                                           new String[] { key });

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response2.Result.ResultCode);
                ClassicAssert.AreEqual(1,                              response2.ConfigurationKeys.Count());
                ClassicAssert.AreEqual(0,                              response2.UnknownKeys.      Count());
                ClassicAssert.AreEqual(value,                          response2.ConfigurationKeys.First().Value);

                ClassicAssert.AreEqual(1,                              getConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   getConfigurationRequests.First().NetworkingNodeId);

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

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint1.OnChangeConfigurationRequest += async (timestamp, sender, changeConfigurationRequest) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                };

                var key        = "hello";
                var value      = "hell";
                var response1  = await testCentralSystem01.ChangeConfiguration(
                                           NetworkingNodeId:   chargePoint1.Id,
                                           Key:                key,
                                           Value:              value
                                       );

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(ConfigurationStatus.Rejected,   response1.Status);

                ClassicAssert.AreEqual(1,                              changeConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   changeConfigurationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(key,                            changeConfigurationRequests.First().Key);
                ClassicAssert.AreEqual(value,                          changeConfigurationRequests.First().Value);


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint1.OnGetConfigurationRequest += async (timestamp, sender, getConfigurationRequest) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                };

                var response2 = await testCentralSystem01.GetConfiguration(chargePoint1.Id,
                                                                           new String[] { key });

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response2.Result.ResultCode);
                ClassicAssert.AreEqual(1,                              response2.ConfigurationKeys.Count());
                ClassicAssert.AreEqual(0,                              response2.UnknownKeys.      Count());
                ClassicAssert.AreEqual("world",                        response2.ConfigurationKeys.First().Value);

                ClassicAssert.AreEqual(1,                              getConfigurationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   getConfigurationRequests.First().NetworkingNodeId);

            }

        }

        #endregion


        #region CentralSystem_TransferTextData_OK_Test()

        /// <summary>
        /// A test for sending custom text data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferTextData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var dataTransferRequests = new List<OCPP.CSMS.DataTransferRequest>();

                chargePoint1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id.       GraphDefined;
                var messageId  = Message_Id.      Parse       (RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await testCentralSystem01.TransferData(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     VendorId:           vendorId,
                                     MessageId:          messageId,
                                     Data:               data,
                                     CustomData:         null
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                   Is.Not.Null);
                    Assert.That(response.Data?.Type,                             Is.EqualTo(JTokenType.String));
                    Assert.That(response.Data?.ToString(),                       Is.EqualTo(data.Reverse()));

                    Assert.That(dataTransferRequests.Count,                      Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(dataTransferRequests.First().VendorId,           Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,          Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,         Is.EqualTo(JTokenType.String));
                    Assert.That(dataTransferRequests.First().Data?.ToString(),   Is.EqualTo(data));

                });

            }

        }

        #endregion

        #region CentralSystem_TransferJObjectData_OK_Test()

        /// <summary>
        /// A test for sending custom JObject data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferJObjectData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var dataTransferRequests = new List<OCPP.CSMS.DataTransferRequest>();

                chargePoint1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JObject(
                                     new JProperty(
                                         "key",
                                         RandomExtensions.RandomString(40)
                                     )
                                 );

                var response   = await testCentralSystem01.TransferData(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     VendorId:           vendorId,
                                     MessageId:          messageId,
                                     Data:               data
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                                Is.Not.Null);
                    Assert.That(response.Data?.Type,                                          Is.EqualTo(JTokenType.Object));
                    Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),            Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                                   Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().NetworkingNodeId,                Is.EqualTo(chargePoint1.Id));
                    Assert.That(dataTransferRequests.First().VendorId,                        Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                       Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                      Is.EqualTo(JTokenType.Object));
                    Assert.That(dataTransferRequests.First().Data?["key"]?.Value<String>(),   Is.EqualTo(data["key"]?.Value<String>()));

                });

            }

        }

        #endregion

        #region CentralSystem_TransferJArrayData_OK_Test()

        /// <summary>
        /// A test for sending custom JArray data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferJArrayData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var dataTransferRequests = new List<OCPP.CSMS.DataTransferRequest>();

                chargePoint1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await testCentralSystem01.TransferData(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     VendorId:           vendorId,
                                     MessageId:          messageId,
                                     Data:               data,
                                     CustomData:         null
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                            Is.Not.Null);
                    Assert.That(response.Data?.Type,                                      Is.EqualTo(JTokenType.Array));
                    Assert.That(response.Data?[0]?.Value<String>()?.Reverse(),            Is.EqualTo(data[0]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                               Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().NetworkingNodeId,            Is.EqualTo(chargePoint1.Id));
                    Assert.That(dataTransferRequests.First().VendorId,                    Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                   Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                  Is.EqualTo(JTokenType.Array));
                    Assert.That(dataTransferRequests.First().Data?[0]?.Value<String>(),   Is.EqualTo(data[0]?.Value<String>()));

                });

            }

        }

        #endregion

        #region CentralSystem_DataTransfer_Rejected_Test()

        /// <summary>
        /// A test for sending custom text data to a charge point, but it is rejected.
        /// </summary>
        [Test]
        public async Task CentralSystem_DataTransfer_Rejected_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                var dataTransferRequests = new List<OCPP.CSMS.DataTransferRequest>();

                chargePoint1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id. Parse("ACME Inc.");
                var messageId  = Message_Id.Parse("hello");
                var data       = "world!";
                var response   = await testCentralSystem01.TransferData(
                                           NetworkingNodeId:   chargePoint1.Id,
                                           VendorId:           vendorId,
                                           MessageId:          messageId,
                                           Data:               data,
                                           CustomData:         null
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DataTransferStatus.Rejected));
                    Assert.That(response.Data,                                   Is.Null);

                    Assert.That(dataTransferRequests.Count,                      Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(dataTransferRequests.First().VendorId,           Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,          Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,         Is.EqualTo(JTokenType.String));
                    Assert.That(dataTransferRequests.First().Data?.ToString(),   Is.EqualTo(data));

                });

            }

        }

        #endregion


    }

}
