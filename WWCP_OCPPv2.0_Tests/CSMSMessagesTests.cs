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

            if (testCSMS01              is not null &&
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

            if (testCSMS01              is not null &&
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




        #region CSMS_RequestStartStopTransaction_Test()

        /// <summary>
        /// A test starting and stopping a charging session/transaction.
        /// </summary>
        [Test]
        public async Task CSMS_RequestStartStopTransaction_Test()
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

                var requestStartTransactionRequests  = new List<RequestStartTransactionRequest>();
                var requestStopTransactionRequests   = new List<RequestStopTransactionRequest>();

                chargingStation1.OnRequestStartTransactionRequest += async (timestamp, sender, requestStartTransactionRequest) => {
                    requestStartTransactionRequests.Add(requestStartTransactionRequest);
                };

                chargingStation1.OnRequestStopTransactionRequest  += async (timestamp, sender, requestStopTransactionRequest) => {
                    requestStopTransactionRequests. Add(requestStopTransactionRequest);
                };

                var startResponse  = await testCSMS01.StartCharging(
                                           ChargeBoxId:                        chargingStation1.ChargeBoxId,
                                           RequestStartTransactionRequestId:   RemoteStart_Id.NewRandom,
                                           IdToken:                            new IdToken(
                                                                                   Value:             "aabbccdd",
                                                                                   Type:              IdTokenTypes.ISO14443,
                                                                                   AdditionalInfos:   null,
                                                                                   CustomData:        null
                                                                               ),
                                           EVSEId:                             EVSE_Id.Parse(1),
                                           ChargingProfile:                    null,
                                           GroupIdToken:                       null,
                                           CustomData:                         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 startResponse.Result.ResultCode);
                Assert.IsTrue  (startResponse.TransactionId.HasValue);

                Assert.AreEqual(1,                              requestStartTransactionRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   requestStartTransactionRequests.First().ChargeBoxId);


                if (startResponse.TransactionId.HasValue)
                {

                    var stopResponse  = await testCSMS01.StopCharging(
                                                  ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                  TransactionId:   startResponse.TransactionId.Value,
                                                  CustomData:      null
                                              );


                    Assert.AreEqual(ResultCodes.OK,                 stopResponse.Result.ResultCode);
                    //Assert.AreEqual(UnlockStatus.Unlocked,          response1.Status);

                    Assert.AreEqual(1,                              requestStopTransactionRequests.Count);
                    Assert.AreEqual(chargingStation1.ChargeBoxId,   requestStopTransactionRequests.First().ChargeBoxId);

                }

            }

        }

        #endregion



        #region CSMS_GetTransactionStatus_Test()

        /// <summary>
        /// A test gettig the current status of a charging session/transaction.
        /// </summary>
        [Test]
        public async Task CSMS_GetTransactionStatus_Test()
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

                var unlockConnectorRequests = new List<GetTransactionStatusRequest>();

                chargingStation1.OnGetTransactionStatusRequest += async (timestamp, sender, unlockConnectorRequest) => {
                    unlockConnectorRequests.Add(unlockConnectorRequest);
                };

                var response1  = await testCSMS01.GetTransactionStatus(
                                     ChargeBoxId:     chargingStation1.ChargeBoxId,
                                     TransactionId:   null,
                                     CustomData:      null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                //Assert.AreEqual(UnlockStatus.Unlocked,          response1.Status);

                Assert.AreEqual(1,                              unlockConnectorRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   unlockConnectorRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region CSMS_UnlockConnector_Test()

        /// <summary>
        /// A test unlocking an EVSE/connector at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_UnlockConnector_Test()
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

                var unlockConnectorRequests = new List<UnlockConnectorRequest>();

                chargingStation1.OnUnlockConnectorRequest += async (timestamp, sender, unlockConnectorRequest) => {
                    unlockConnectorRequests.Add(unlockConnectorRequest);
                };

                var response1  = await testCSMS01.UnlockConnector(
                                     ChargeBoxId:   chargingStation1.ChargeBoxId,
                                     EVSEId:        chargingStation1.EVSEs.First().Id,
                                     ConnectorId:   chargingStation1.EVSEs.First().Connectors.First().Id,
                                     CustomData:    null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(UnlockStatus.Unlocked,          response1.Status);

                Assert.AreEqual(1,                              unlockConnectorRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   unlockConnectorRequests.First().ChargeBoxId);

            }

        }

        #endregion



        #region CSMS_SetDisplayMessage_Test()

        /// <summary>
        /// A test setting the display message at a charging station.
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
                                     ChargeBoxId:   chargingStation1.ChargeBoxId,
                                     Message:       new MessageInfo(
                                                        Id:               DisplayMessage_Id.NewRandom,
                                                        Priority:         MessagePriorities.AlwaysFront,
                                                        Message:          new MessageContent(
                                                                              Content:      message,
                                                                              Format:       MessageFormats.UTF8,
                                                                              Language:     Language_Id.Parse("de"),
                                                                              CustomData:   null
                                                                          ),
                                                        State:            MessageStates.Charging,
                                                        StartTimestamp:   Timestamp.Now,
                                                        EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                        TransactionId:    null,
                                                        CustomData:       null
                                                    ),
                                     CustomData:    null
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

        #region CSMS_GetDisplayMessages_Test()

        /// <summary>
        /// A test getting the display messages from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetDisplayMessages_Test()
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

                var messageIds = new DisplayMessage_Id[] {
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom,
                                     DisplayMessage_Id.NewRandom
                                 };

                for (int i = 1; i <= 10; i++) {

                    var setMessage   = RandomExtensions.RandomString(10);

                    var setResponse  = await testCSMS01.SetDisplayMessage(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           Message:       new MessageInfo(
                                                              Id:               messageIds[i-1],
                                                              Priority:         i > 7 ? MessagePriorities.AlwaysFront : MessagePriorities.NormalCycle,
                                                              Message:          new MessageContent(
                                                                                    Content:      $"{i}:{setMessage}",
                                                                                    Format:       MessageFormats.UTF8,
                                                                                    Language:     Language_Id.Parse("de"),
                                                                                    CustomData:   null
                                                                                ),
                                                              State:            i > 5 ? MessageStates.Charging : MessageStates.Idle,
                                                              StartTimestamp:   Timestamp.Now,
                                                              EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                              TransactionId:    null,
                                                              CustomData:       null
                                                          ),
                                           CustomData:    null
                                       );

                    Assert.AreEqual(ResultCodes.OK,   setResponse.Result.ResultCode);
                    Assert.AreEqual(i,                setDisplayMessageRequests.Count);

                }




                var getDisplayMessagesRequests = new List<GetDisplayMessagesRequest>();

                chargingStation1.OnGetDisplayMessagesRequest += async (timestamp, sender, getDisplayMessagesRequest) => {
                    getDisplayMessagesRequests.Add(getDisplayMessagesRequest);
                };


                var notifyDisplayMessagesRequests = new List<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OnNotifyDisplayMessagesRequest += async (timestamp, sender, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.Add(notifyDisplayMessagesRequest);
                };


                var getResponse1  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   1,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK, getResponse1.Result.ResultCode);
                Assert.AreEqual(1, getDisplayMessagesRequests.Count);


                var getResponse2  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   2,
                                              Ids:                           new DisplayMessage_Id[] {
                                                                                 messageIds[0],
                                                                                 messageIds[2],
                                                                                 messageIds[4]
                                                                             },
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK, getResponse2.Result.ResultCode);
                Assert.AreEqual(2, getDisplayMessagesRequests.Count);


                var getResponse3  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   3,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         MessageStates.Charging,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK, getResponse3.Result.ResultCode);
                Assert.AreEqual(3, getDisplayMessagesRequests.Count);


                var getResponse4  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   4,
                                              Ids:                           null,
                                              Priority:                      MessagePriorities.AlwaysFront,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK, getResponse4.Result.ResultCode);
                Assert.AreEqual(4, getDisplayMessagesRequests.Count);


                await Task.Delay(500);


                Assert.AreEqual(4, notifyDisplayMessagesRequests.Count);

                Assert.AreEqual(10, notifyDisplayMessagesRequests.ElementAt(0).MessageInfos.Count());
                Assert.AreEqual( 3, notifyDisplayMessagesRequests.ElementAt(1).MessageInfos.Count());
                Assert.AreEqual( 5, notifyDisplayMessagesRequests.ElementAt(2).MessageInfos.Count());
                Assert.AreEqual( 3, notifyDisplayMessagesRequests.ElementAt(3).MessageInfos.Count());

            }

        }

        #endregion

        #region CSMS_ClearDisplayMessage_Test()

        /// <summary>
        /// A test removing a display message from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ClearDisplayMessage_Test()
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

                var messageId1    = DisplayMessage_Id.NewRandom;
                var message1      = RandomExtensions.RandomString(10);

                var setResponse1  = await testCSMS01.SetDisplayMessage(
                                        ChargeBoxId:   chargingStation1.ChargeBoxId,
                                        Message:       new MessageInfo(
                                                           Id:               messageId1,
                                                           Priority:         MessagePriorities.AlwaysFront,
                                                           Message:          new MessageContent(
                                                                                 Content:      message1,
                                                                                 Format:       MessageFormats.UTF8,
                                                                                 Language:     Language_Id.Parse("de"),
                                                                                 CustomData:   null
                                                                             ),
                                                           State:            MessageStates.Charging,
                                                           StartTimestamp:   Timestamp.Now,
                                                           EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                           TransactionId:    null,
                                                           CustomData:       null
                                                       ),
                                        CustomData:    null
                                    );

                Assert.AreEqual(ResultCodes.OK,   setResponse1.Result.ResultCode);
                Assert.AreEqual(1,                setDisplayMessageRequests.Count);


                var messageId2    = DisplayMessage_Id.NewRandom;
                var message2      = RandomExtensions.RandomString(10);

                var setResponse2  = await testCSMS01.SetDisplayMessage(
                                        ChargeBoxId:   chargingStation1.ChargeBoxId,
                                        Message:       new MessageInfo(
                                                           Id:               messageId2,
                                                           Priority:         MessagePriorities.AlwaysFront,
                                                           Message:          new MessageContent(
                                                                                 Content:      message2,
                                                                                 Format:       MessageFormats.UTF8,
                                                                                 Language:     Language_Id.Parse("de"),
                                                                                 CustomData:   null
                                                                             ),
                                                           State:            MessageStates.Charging,
                                                           StartTimestamp:   Timestamp.Now,
                                                           EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                           TransactionId:    null,
                                                           CustomData:       null
                                                       ),
                                        CustomData:    null
                                    );

                Assert.AreEqual(ResultCodes.OK,   setResponse2.Result.ResultCode);
                Assert.AreEqual(2,                setDisplayMessageRequests.Count);


                // Get Messages BEFORE
                var getDisplayMessagesRequests = new List<GetDisplayMessagesRequest>();

                chargingStation1.OnGetDisplayMessagesRequest += async (timestamp, sender, getDisplayMessagesRequest) => {
                    getDisplayMessagesRequests.Add(getDisplayMessagesRequest);
                };


                var notifyDisplayMessagesRequests = new List<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OnNotifyDisplayMessagesRequest += async (timestamp, sender, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.Add(notifyDisplayMessagesRequest);
                };


                var getResponse1  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   1,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK,   getResponse1.Result.ResultCode);
                Assert.AreEqual(1,                getDisplayMessagesRequests.Count);



                // Delete message #1
                var clearDisplayMessageRequests = new List<ClearDisplayMessageRequest>();

                chargingStation1.OnClearDisplayMessageRequest += async (timestamp, sender, clearDisplayMessageRequest) => {
                    clearDisplayMessageRequests.Add(clearDisplayMessageRequest);
                };

                var clearResponse  = await testCSMS01.ClearDisplayMessage(
                                         ChargeBoxId:        chargingStation1.ChargeBoxId,
                                         DisplayMessageId:   messageId1,
                                         CustomData:         null
                                     );

                Assert.AreEqual(ResultCodes.OK,   clearResponse.Result.ResultCode);
                Assert.AreEqual(1,                clearDisplayMessageRequests.Count);



                // Get Messages AFTER
                var getResponse2  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   2,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                Assert.AreEqual(ResultCodes.OK,   getResponse2.Result.ResultCode);
                Assert.AreEqual(2,                getDisplayMessagesRequests.Count);


                await Task.Delay(500);


                Assert.AreEqual(2,                notifyDisplayMessagesRequests.ElementAt(0).MessageInfos.Count());
                Assert.AreEqual(1,                notifyDisplayMessagesRequests.ElementAt(1).MessageInfos.Count());

            }

        }

        #endregion


        #region CSMS_SendCostUpdate_Test()

        /// <summary>
        /// A test sending updated total costs.
        /// </summary>
        [Test]
        public async Task CSMS_SendCostUpdate_Test()
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

                var costUpdatedRequests = new List<CostUpdatedRequest>();

                chargingStation1.OnCostUpdatedRequest += async (timestamp, sender, costUpdatedRequest) => {
                    costUpdatedRequests.Add(costUpdatedRequest);
                };

                var message    = RandomExtensions.RandomString(10);

                var response1  = await testCSMS01.SendCostUpdated(
                                     ChargeBoxId:     chargingStation1.ChargeBoxId,
                                     TotalCost:       1.02M,
                                     TransactionId:   Transaction_Id.Parse(123),
                                     CustomData:      null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                //Assert.AreEqual(data.Reverse(),                 response1.Data?.ToString());

                Assert.AreEqual(1,                              costUpdatedRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   costUpdatedRequests.First().ChargeBoxId);
                //Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                //Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                //Assert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion


    }

}
