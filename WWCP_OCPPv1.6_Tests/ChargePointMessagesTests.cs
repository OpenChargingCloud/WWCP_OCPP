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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

/////[assembly: LevelOfParallelism(1)]
namespace cloud.charging.open.protocols.OCPPv1_6.tests
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

                Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.ChargePointVendor);
                Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.ChargePointVendor);
                Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.ChargePointVendor);

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

                var response1 = await chargingStation1.SendBootNotification();

                Assert.AreEqual(ResultCodes.OK,                            response1.Result.ResultCode);
                Assert.AreEqual(RegistrationStatus.Accepted,               response1.Status);

                Assert.AreEqual(1,                                         bootNotificationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,              bootNotificationRequests.First().ChargeBoxId);
                Assert.AreEqual(chargingStation1.ChargePointVendor,        bootNotificationRequests.First().ChargePointVendor);
                Assert.AreEqual(chargingStation1.ChargePointSerialNumber,  bootNotificationRequests.First().ChargePointSerialNumber);
                Assert.AreEqual(chargingStation1.ChargeBoxSerialNumber,    bootNotificationRequests.First().ChargeBoxSerialNumber);
                Assert.AreEqual(chargingStation1.Iccid,                    bootNotificationRequests.First().Iccid);
                Assert.AreEqual(chargingStation1.IMSI,                     bootNotificationRequests.First().IMSI);
                Assert.AreEqual(chargingStation1.MeterType,                bootNotificationRequests.First().MeterType);
                Assert.AreEqual(chargingStation1.MeterSerialNumber,        bootNotificationRequests.First().MeterSerialNumber);

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


        #region ChargePoint_Authorize_Test()

        /// <summary>
        /// A test for authorizing id tokens against the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_Authorize_Test()
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

                var authorizeRequests = new List<CP.AuthorizeRequest>();

                testCentralSystem01.OnAuthorizeRequest += async (timestamp, sender, authorizeRequest) => {
                    authorizeRequests.Add(authorizeRequest);
                };

                var idToken   = IdToken.NewRandom();
                var response1 = await chargingStation1.Authorize(idToken);


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTagInfo.Status);

                Assert.AreEqual(1,                              authorizeRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   authorizeRequests.First().ChargeBoxId);
                Assert.AreEqual(idToken,                        authorizeRequests.First().IdTag);

            }

        }

        #endregion

        #region ChargePoint_StartTransaction_Test()

        /// <summary>
        /// A test for sending a start transaction (notification) to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_StartTransaction_Test()
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

                var startTransactionRequests = new List<CP.StartTransactionRequest>();

                testCentralSystem01.OnStartTransactionRequest += async (timestamp, sender, startTransactionRequest) => {
                    startTransactionRequests.Add(startTransactionRequest);
                };

                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.NewRandom();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response1       = await chargingStation1.StartTransaction(
                                          connectorId,
                                          idToken,
                                          startTimestamp,
                                          meterStart,
                                          reservationId
                                      );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTagInfo.Status);
                Assert.IsTrue  (response1.TransactionId.IsNotNullOrEmpty);

                Assert.AreEqual(1,                              startTransactionRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   startTransactionRequests.First().ChargeBoxId);
                Assert.AreEqual(connectorId,                    startTransactionRequests.First().ConnectorId);
                Assert.AreEqual(idToken,                        startTransactionRequests.First().IdTag);
                Assert.AreEqual(startTimestamp.ToIso8601(),     startTransactionRequests.First().StartTimestamp.ToIso8601());
                Assert.AreEqual(meterStart,                     startTransactionRequests.First().MeterStart);
                Assert.AreEqual(reservationId,                  startTransactionRequests.First().ReservationId);

            }

        }

        #endregion

        #region ChargePoint_StatusNotification_Test()

        /// <summary>
        /// A test for sending status notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_StatusNotification_Test()
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

                var statusNotificationRequests = new List<CP.StatusNotificationRequest>();

                testCentralSystem01.OnStatusNotificationRequest += async (timestamp, sender, statusNotificationRequest) => {
                    statusNotificationRequests.Add(statusNotificationRequest);
                };

                var connectorId      = Connector_Id.Parse(1);
                var status           = ChargePointStatus.Available;
                var errorCode        = ChargePointErrorCodes.NoError;
                var info             = RandomExtensions.RandomString(20);
                var statusTimestamp  = Timestamp.Now;
                var vendorId         = "GraphDefined OEM";
                var vendorErrorCode  = "E0001";

                var response1        = await chargingStation1.SendStatusNotification(
                                           connectorId,
                                           status,
                                           errorCode,
                                           info,
                                           statusTimestamp,
                                           vendorId,
                                           vendorErrorCode
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              statusNotificationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   statusNotificationRequests.First().ChargeBoxId);
                Assert.AreEqual(connectorId,                    statusNotificationRequests.First().ConnectorId);
                Assert.AreEqual(status,                         statusNotificationRequests.First().Status);
                Assert.AreEqual(errorCode,                      statusNotificationRequests.First().ErrorCode);
                Assert.AreEqual(info,                           statusNotificationRequests.First().Info);
                Assert.AreEqual(statusTimestamp.ToIso8601(),    statusNotificationRequests.First().StatusTimestamp!.Value.ToIso8601());
                Assert.AreEqual(vendorId,                       statusNotificationRequests.First().VendorId);
                Assert.AreEqual(vendorErrorCode,                statusNotificationRequests.First().VendorErrorCode);

            }

        }

        #endregion

        #region ChargePoint_SendMeterValues_Test()

        /// <summary>
        /// A test for sending meter values to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendMeterValues_Test()
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

                var meterValuesRequests = new List<CP.MeterValuesRequest>();

                testCentralSystem01.OnMeterValuesRequest += async (timestamp, sender, meterValuesRequest) => {
                    meterValuesRequests.Add(meterValuesRequest);
                };

                var connectorId     = Connector_Id.Parse(1);
                var meterValues     = new MeterValue[] {
                                          new MeterValue(
                                              Timestamp.Now - TimeSpan.FromMinutes(5),
                                              new SampledValue[] {
                                                  new SampledValue(
                                                      "001",
                                                      ReadingContexts.TransactionBegin,
                                                      ValueFormats.Raw,
                                                      Measurands.CurrentImport,
                                                      Phases.L1,
                                                      Locations.Outlet,
                                                      UnitsOfMeasure.kW
                                                  ),
                                                  new SampledValue(
                                                      "001cryptic",
                                                      ReadingContexts.TransactionBegin,
                                                      ValueFormats.SignedData,
                                                      Measurands.CurrentImport,
                                                      Phases.L1,
                                                      Locations.Outlet,
                                                      UnitsOfMeasure.kW
                                                  )
                                              }
                                          ),
                                          new MeterValue(
                                              Timestamp.Now,
                                              new SampledValue[] {
                                                  new SampledValue(
                                                      "002",
                                                      ReadingContexts.TransactionEnd,
                                                      ValueFormats.Raw,
                                                      Measurands.CurrentImport,
                                                      Phases.L1,
                                                      Locations.Outlet,
                                                      UnitsOfMeasure.kW
                                                  ),
                                                  new SampledValue(
                                                      "002cryptic",
                                                      ReadingContexts.TransactionEnd,
                                                      ValueFormats.SignedData,
                                                      Measurands.CurrentImport,
                                                      Phases.L1,
                                                      Locations.Outlet,
                                                      UnitsOfMeasure.kW
                                                  )
                                              }
                                          )
                                      };
                var transactionId   = Transaction_Id.NewRandom;

                var response1       = await chargingStation1.SendMeterValues(
                                          connectorId,
                                          meterValues,
                                          transactionId
                                      );


                Assert.AreEqual (ResultCodes.OK,                                                  response1.Result.ResultCode);

                Assert.AreEqual (1,                                                               meterValuesRequests.Count);
                Assert.AreEqual (chargingStation1.ChargeBoxId,                                    meterValuesRequests.First().ChargeBoxId);
                Assert.AreEqual (connectorId,                                                     meterValuesRequests.First().ConnectorId);
                Assert.AreEqual (transactionId,                                                   meterValuesRequests.First().TransactionId);

                Assert.AreEqual (meterValues.Length,                                              meterValuesRequests.First().MeterValues.Count());
                Assert.IsTrue   (meterValues.ElementAt(0).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                Assert.IsTrue   (meterValues.ElementAt(1).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.Count());
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.Count());

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Value);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Value);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Context);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Context);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Phase);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Phase);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Location);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Location);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit);

            }

        }

        #endregion

        #region ChargePoint_StopTransaction_Test()

        /// <summary>
        /// A test for sending a stop transaction (notification) to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_StopTransaction_Test()
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

                var stopTransactionRequests = new List<CP.StopTransactionRequest>();

                testCentralSystem01.OnStopTransactionRequest += async (timestamp, sender, stopTransactionRequest) => {
                    stopTransactionRequests.Add(stopTransactionRequest);
                };

                var transactionId    = Transaction_Id.NewRandom;
                var stopTimestamp    = Timestamp.Now;
                var meterStop        = RandomExtensions.RandomUInt64();
                var idToken          = IdToken.NewRandom();
                var reason           = Reasons.EVDisconnected;
                var transactionData  = new MeterValue[] {
                                           new MeterValue(
                                               Timestamp.Now - TimeSpan.FromMinutes(5),
                                               new SampledValue[] {
                                                   new SampledValue(
                                                       "001",
                                                       ReadingContexts.TransactionBegin,
                                                       ValueFormats.Raw,
                                                       Measurands.CurrentImport,
                                                       Phases.L1,
                                                       Locations.Outlet,
                                                       UnitsOfMeasure.kW
                                                   ),
                                                   new SampledValue(
                                                       "001cryptic",
                                                       ReadingContexts.TransactionBegin,
                                                       ValueFormats.SignedData,
                                                       Measurands.CurrentImport,
                                                       Phases.L1,
                                                       Locations.Outlet,
                                                       UnitsOfMeasure.kW
                                                   )
                                               }
                                           ),
                                           new MeterValue(
                                               Timestamp.Now,
                                               new SampledValue[] {
                                                   new SampledValue(
                                                       "002",
                                                       ReadingContexts.TransactionEnd,
                                                       ValueFormats.Raw,
                                                       Measurands.CurrentImport,
                                                       Phases.L1,
                                                       Locations.Outlet,
                                                       UnitsOfMeasure.kW
                                                   ),
                                                   new SampledValue(
                                                       "002cryptic",
                                                       ReadingContexts.TransactionEnd,
                                                       ValueFormats.SignedData,
                                                       Measurands.CurrentImport,
                                                       Phases.L1,
                                                       Locations.Outlet,
                                                       UnitsOfMeasure.kW
                                                   )
                                               }
                                           )
                                       };

                var response1        = await chargingStation1.StopTransaction(
                                           transactionId,
                                           stopTimestamp,
                                           meterStop,
                                           idToken,
                                           reason,
                                           transactionData
                                       );


                Assert.AreEqual (ResultCodes.OK,                                                      response1.Result.ResultCode);
                Assert.IsNotNull(response1.IdTagInfo);
                Assert.AreEqual (AuthorizationStatus.Accepted,                                        response1.IdTagInfo!.Value.Status);

                Assert.AreEqual (1,                                                                   stopTransactionRequests.Count);
                Assert.AreEqual (chargingStation1.ChargeBoxId,                                        stopTransactionRequests.First().ChargeBoxId);
                Assert.AreEqual (transactionId,                                                       stopTransactionRequests.First().TransactionId);
                Assert.AreEqual (stopTimestamp.ToIso8601(),                                           stopTransactionRequests.First().StopTimestamp.ToIso8601());
                Assert.AreEqual (meterStop,                                                           stopTransactionRequests.First().MeterStop);
                Assert.AreEqual (idToken,                                                             stopTransactionRequests.First().IdTag);
                Assert.AreEqual (reason,                                                              stopTransactionRequests.First().Reason);

                Assert.AreEqual (transactionData.Length,                                              stopTransactionRequests.First().TransactionData.Count());
                Assert.IsTrue   (transactionData.ElementAt(0).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                Assert.IsTrue   (transactionData.ElementAt(1).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.Count(),                  stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.Count());
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.Count(),                  stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.Count());

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Value,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Value,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Value);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Value,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Value,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Value);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Context,     stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Context,     stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Context);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Context,     stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Context,     stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Context);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Format,      stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Format);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Format,      stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Format);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Format,      stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Format);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Format,      stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Format);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Phase);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Phase);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Location,    stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Location,    stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Location);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Location,    stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Location,    stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Location);

                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Unit);
                Assert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Unit);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Unit);
                Assert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Unit);

            }

        }

        #endregion


        #region ChargePoint_TransferData_Test()

        /// <summary>
        /// A test for transfering data to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_TransferData_Test()
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

                var dataTransferRequests = new List<CP.DataTransferRequest>();

                testCentralSystem01.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "GraphDefined OEM";
                var messageId  = RandomExtensions.RandomString(10);
                var data       = RandomExtensions.RandomString(40);

                var response1  = await chargingStation1.TransferData(
                                     vendorId,
                                     messageId,
                                     data
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(data.Reverse(),                 response1.Data);

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(data,                           dataTransferRequests.First().Data);

            }

        }

        #endregion

        #region ChargePoint_SendDiagnosticsStatusNotification_Test()

        /// <summary>
        /// A test for sending diagnostics status notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendDiagnosticsStatusNotification_Test()
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

                var diagnosticsStatusNotifications = new List<CP.DiagnosticsStatusNotificationRequest>();

                testCentralSystem01.OnDiagnosticsStatusNotificationRequest += async (timestamp, sender, diagnosticsStatusNotification) => {
                    diagnosticsStatusNotifications.Add(diagnosticsStatusNotification);
                };

                var status     = DiagnosticsStatus.Uploaded;

                var response1  = await chargingStation1.SendDiagnosticsStatusNotification(
                                     status
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              diagnosticsStatusNotifications.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   diagnosticsStatusNotifications.First().ChargeBoxId);
                Assert.AreEqual(status,                         diagnosticsStatusNotifications.First().Status);

            }

        }

        #endregion

        #region ChargePoint_SendFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending firmware status notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendFirmwareStatusNotification_Test()
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

                var firmwareStatusNotifications = new List<CP.FirmwareStatusNotificationRequest>();

                testCentralSystem01.OnFirmwareStatusNotificationRequest += async (timestamp, sender, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                };

                var status     = FirmwareStatus.Installed;

                var response1  = await chargingStation1.SendFirmwareStatusNotification(
                                     status
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              firmwareStatusNotifications.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   firmwareStatusNotifications.First().ChargeBoxId);
                Assert.AreEqual(status,                         firmwareStatusNotifications.First().Status);

            }

        }

        #endregion


    }

}
