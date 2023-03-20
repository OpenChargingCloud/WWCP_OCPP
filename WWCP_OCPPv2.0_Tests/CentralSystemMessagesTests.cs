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

using cloud.charging.open.protocols.OCPPv2_0.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CSMSMessagesTests : AChargingStationTests
    {

        #region CSMS_Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CSMS_Reset_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var resetRequests = new List<ResetRequest>();

                chargingStation1.OnResetRequest += async (timestamp, sender, resetRequest) => {
                    resetRequests.Add(resetRequest);
                };

                var resetType  = ResetTypes.Immediate;
                var response1  = await testCSMS01.Reset(chargingStation1.ChargeBoxId, resetType);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ResetStatus.Accepted,           response1.Status);

                Assert.AreEqual(1,                              resetRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   resetRequests.First().ChargeBoxId);
                Assert.AreEqual(resetType,                      resetRequests.First().ResetType);

            }

        }

        #endregion

        #region CSMS_Reset_UnknownChargeBox_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CSMS_Reset_UnknownChargeBox_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var resetRequests = new List<ResetRequest>();

                chargingStation2.OnResetRequest += async (timestamp, sender, resetRequest) => {
                    resetRequests.Add(resetRequest);
                };

                var resetType  = ResetTypes.Immediate;
                var response1  = await testCSMS01.Reset(chargingStation2.ChargeBoxId, resetType);

                Assert.AreEqual  (ResultCodes.NetworkError,  response1.Result.ResultCode);
                Assert.IsNotEmpty(                           response1.Result.Description);
                Assert.AreEqual  (ResetStatus.Unknown,       response1.Status);

                Assert.AreEqual  (0,                         resetRequests.Count);

            }

        }

        #endregion


        #region CSMS_ChangeAvailability_Test()

        ///// <summary>
        ///// A test for sending a change availability message to a charge point.
        ///// </summary>
        //[Test]
        //public async Task CSMS_ChangeAvailability_Test()
        //{

        //    Assert.IsNotNull(testCSMS01);
        //    Assert.IsNotNull(testBackendWebSockets01);
        //    Assert.IsNotNull(chargingStation1);
        //    Assert.IsNotNull(chargingStation2);
        //    Assert.IsNotNull(chargingStation3);

        //    if (testCSMS01     is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

        //        chargingStation1.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
        //            changeAvailabilityRequests.Add(changeAvailabilityRequest);
        //        };

        //        var connectorId   = Connector_Id.Parse(1);
        //        var availability  = OperationalStatus.Operative;
        //        var response1     = await testCSMS01.ChangeAvailability(chargingStation1.ChargeBoxId,
        //                                                                         connectorId,
        //                                                                         availability);

        //        Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
        //        Assert.AreEqual(AvailabilityStatus.Accepted,    response1.Status);

        //        Assert.AreEqual(1,                              changeAvailabilityRequests.Count);
        //        Assert.AreEqual(chargingStation1.ChargeBoxId,   changeAvailabilityRequests.First().ChargeBoxId);
        //        Assert.AreEqual(connectorId,                    changeAvailabilityRequests.First().ConnectorId);
        //        Assert.AreEqual(availability,                   changeAvailabilityRequests.First().Availability);

        //    }

        //}

        #endregion

        #region CSMS_ChangeAvailability_UnknownChargeBox_Test()

        ///// <summary>
        ///// A test for sending a change availability message to a charge point.
        ///// </summary>
        //[Test]
        //public async Task CSMS_ChangeAvailability_UnknownChargeBox_Test()
        //{

        //    Assert.IsNotNull(testCSMS01);
        //    Assert.IsNotNull(testBackendWebSockets01);
        //    Assert.IsNotNull(chargingStation1);
        //    Assert.IsNotNull(chargingStation2);
        //    Assert.IsNotNull(chargingStation3);

        //    if (testCSMS01     is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

        //        chargingStation2.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
        //            changeAvailabilityRequests.Add(changeAvailabilityRequest);
        //        };

        //        var connectorId   = Connector_Id.Parse(1);
        //        var availability  = Availabilities.Operative;
        //        var response1     = await testCSMS01.ChangeAvailability(chargingStation2.ChargeBoxId,
        //                                                                         connectorId,
        //                                                                         availability);

        //        Assert.AreEqual  (ResultCodes.NetworkError,     response1.Result.ResultCode);
        //        Assert.IsNotEmpty(                              response1.Result.Description);
        //        Assert.AreEqual  (AvailabilityStatus.Unknown,   response1.Status);

        //        Assert.AreEqual  (0,                            changeAvailabilityRequests.Count);

        //    }

        //}

        #endregion


        #region CSMS_TransferTextData_Test()

        /// <summary>
        /// A test for transfering text data to the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_TransferTextData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "GraphDefined OEM";
                var messageId  = RandomExtensions.RandomString(10);
                var data       = RandomExtensions.RandomString(40);

                var response1  = await testCSMS01.TransferData(
                                     ChargeBoxId:  chargingStation1.ChargeBoxId,
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data,
                                     CustomData:   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(data.Reverse(),                 response1.Data?.ToString());

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion

        #region CSMS_TransferJObjectData_Test()

        /// <summary>
        /// A test for transfering JObject data to the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_TransferJObjectData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "GraphDefined OEM";
                var messageId  = RandomExtensions.RandomString(10);
                var data       = new JObject(
                                     new JProperty(
                                         "key",
                                         RandomExtensions.RandomString(40)
                                     )
                                 );

                var response1  = await testCSMS01.TransferData(
                                     ChargeBoxId:  chargingStation1.ChargeBoxId,
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data,
                                     CustomData:   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(JTokenType.Object,              response1.Data?.Type);
                Assert.AreEqual(data["key"]?.Value<String>(),   response1.Data?["key"]?.Value<String>()?.Reverse());

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(JTokenType.Object,              dataTransferRequests.First().Data?.Type);
                Assert.AreEqual(data["key"]?.Value<String>(),   dataTransferRequests.First().Data?["key"]?.Value<String>());

            }

        }

        #endregion

        #region CSMS_TransferJArrayData_Test()

        /// <summary>
        /// A test for transfering JArray data to the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_TransferJArrayData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "GraphDefined OEM";
                var messageId  = RandomExtensions.RandomString(10);
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response1  = await testCSMS01.TransferData(
                                     ChargeBoxId:  chargingStation1.ChargeBoxId,
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data,
                                     CustomData:   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(JTokenType.Array,               response1.Data?.Type);
                Assert.AreEqual(data[0]?.Value<String>(),       response1.Data?[0]?.Value<String>()?.Reverse());

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(JTokenType.Array,               dataTransferRequests.First().Data?.Type);
                Assert.AreEqual(data[0]?.Value<String>(),       dataTransferRequests.First().Data?[0]?.Value<String>());

            }

        }

        #endregion

        #region CSMS_TransferTextData_Rejected_Test()

        /// <summary>
        /// A test for sending data to a charge point.
        /// </summary>
        [Test]
        public async Task CSMS_TransferTextData_Rejected_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "ACME Inc.";
                var messageId  = "hello";
                var data       = "world!";
                var response1  = await testCSMS01.TransferData(chargingStation1.ChargeBoxId,
                                                               vendorId,
                                                               messageId,
                                                               data);

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(DataTransferStatus.Rejected,    response1.Status);

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion


        #region CSMS_SetDisplayMessage_Test()

        /// <summary>
        /// A test settingn the display message at the charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetDisplayMessage_Test()
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

                var setDisplayMessageRequests = new List<SetDisplayMessageRequest>();

                chargingStation1.OnSetDisplayMessageRequest += async (timestamp, sender, setDisplayMessageRequest) => {
                    setDisplayMessageRequests.Add(setDisplayMessageRequest);
                };

                var message    = RandomExtensions.RandomString(10);

                var response1  = await testCSMS01.SetDisplayMessage(
                                     ChargeBoxId:  chargingStation1.ChargeBoxId,
                                     Message:      new MessageInfo(
                                                       Id:               DisplayMessage_Id.NewRandom,
                                                       Priority:         MessagePriorities.AlwaysFront,
                                                       Message:          new MessageContent(
                                                                             Content:  message,
                                                                             Format:   MessageFormats.UTF8,
                                                                             Language: Language_Id.Parse("de"),
                                                                             CustomData:  null
                                                                         ),
                                                       State:            MessageStates.Charging,
                                                       StartTimestamp:   Timestamp.Now,
                                                       EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                       TransactionId:    null,
                                                       CustomData:       null
                                                   ),
                                     CustomData:   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                //Assert.AreEqual(data.Reverse(),                 response1.Data?.ToString());

                Assert.AreEqual(1,                              setDisplayMessageRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   setDisplayMessageRequests.First().ChargeBoxId);
                //Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                //Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                //Assert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion

    }

}
