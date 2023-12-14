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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// Unit tests for charge points sending messages to the central system.
    /// </summary>
    [TestFixture]
    public class CP_Messages_Tests : AChargePointTests
    {

        #region ChargePoint_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendBootNotifications_Test()
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

                var bootNotificationRequests = new List<CP.BootNotificationRequest>();

                testCentralSystem01.OnBootNotificationRequest += async (timestamp, sender, connection, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                };

                var response1 = await chargePoint1.SendBootNotification();

                ClassicAssert.AreEqual(OCPP.ResultCode.OK,                        response1.Result.ResultCode);
                ClassicAssert.AreEqual(RegistrationStatus.Accepted,               response1.Status);

                ClassicAssert.AreEqual(1,                                         bootNotificationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,              bootNotificationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(chargePoint1.ChargePointVendor,        bootNotificationRequests.First().ChargePointVendor);
                ClassicAssert.AreEqual(chargePoint1.ChargePointSerialNumber,  bootNotificationRequests.First().ChargePointSerialNumber);
                ClassicAssert.AreEqual(chargePoint1.ChargeBoxSerialNumber,    bootNotificationRequests.First().ChargeBoxSerialNumber);
                ClassicAssert.AreEqual(chargePoint1.Iccid,                    bootNotificationRequests.First().Iccid);
                ClassicAssert.AreEqual(chargePoint1.IMSI,                     bootNotificationRequests.First().IMSI);
                ClassicAssert.AreEqual(chargePoint1.MeterType,                bootNotificationRequests.First().MeterType);
                ClassicAssert.AreEqual(chargePoint1.MeterSerialNumber,        bootNotificationRequests.First().MeterSerialNumber);

            }

        }

        #endregion

        #region ChargePoint_SendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendHeartbeats_Test()
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

                var heartbeatRequests = new List<CP.HeartbeatRequest>();

                testCentralSystem01.OnHeartbeatRequest += async (timestamp, sender, connection, heartbeatRequest) => {
                    heartbeatRequests.Add(heartbeatRequest);
                };


                var response1 = await chargePoint1.SendHeartbeat();


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.IsTrue  (Timestamp.Now - response1.CurrentTime < TimeSpan.FromSeconds(10));

                ClassicAssert.AreEqual(1,                              heartbeatRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   heartbeatRequests.First().NetworkingNodeId);

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

                var authorizeRequests = new List<CP.AuthorizeRequest>();

                testCentralSystem01.OnAuthorizeRequest += async (timestamp, sender, connection, authorizeRequest) => {
                    authorizeRequests.Add(authorizeRequest);
                };

                var idToken   = IdToken.NewRandom();
                var response1 = await chargePoint1.Authorize(idToken);


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTagInfo.Status);

                ClassicAssert.AreEqual(1,                              authorizeRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   authorizeRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(idToken,                        authorizeRequests.First().IdTag);

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

                var startTransactionRequests = new List<CP.StartTransactionRequest>();

                testCentralSystem01.OnStartTransactionRequest += async (timestamp, sender, connection, startTransactionRequest) => {
                    startTransactionRequests.Add(startTransactionRequest);
                };

                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.NewRandom();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response1       = await chargePoint1.StartTransaction(
                                          connectorId,
                                          idToken,
                                          startTimestamp,
                                          meterStart,
                                          reservationId
                                      );


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTagInfo.Status);
                ClassicAssert.IsTrue  (response1.TransactionId.IsNotNullOrEmpty);

                ClassicAssert.AreEqual(1,                              startTransactionRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   startTransactionRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(connectorId,                    startTransactionRequests.First().ConnectorId);
                ClassicAssert.AreEqual(idToken,                        startTransactionRequests.First().IdTag);
                ClassicAssert.AreEqual(startTimestamp.ToIso8601(),     startTransactionRequests.First().StartTimestamp.ToIso8601());
                ClassicAssert.AreEqual(meterStart,                     startTransactionRequests.First().MeterStart);
                ClassicAssert.AreEqual(reservationId,                  startTransactionRequests.First().ReservationId);

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

                var statusNotificationRequests = new List<CP.StatusNotificationRequest>();

                testCentralSystem01.OnStatusNotificationRequest += async (timestamp, sender, connection, statusNotificationRequest) => {
                    statusNotificationRequests.Add(statusNotificationRequest);
                };

                var connectorId      = Connector_Id.Parse(1);
                var status           = ChargePointStatus.Available;
                var errorCode        = ChargePointErrorCodes.NoError;
                var info             = RandomExtensions.RandomString(20);
                var statusTimestamp  = Timestamp.Now;
                var vendorId         = "GraphDefined OEM";
                var vendorErrorCode  = "E0001";

                var response1        = await chargePoint1.SendStatusNotification(
                                           connectorId,
                                           status,
                                           errorCode,
                                           info,
                                           statusTimestamp,
                                           vendorId,
                                           vendorErrorCode
                                       );


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              statusNotificationRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   statusNotificationRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(connectorId,                    statusNotificationRequests.First().ConnectorId);
                ClassicAssert.AreEqual(status,                         statusNotificationRequests.First().Status);
                ClassicAssert.AreEqual(errorCode,                      statusNotificationRequests.First().ErrorCode);
                ClassicAssert.AreEqual(info,                           statusNotificationRequests.First().Info);
                ClassicAssert.AreEqual(statusTimestamp.ToIso8601(),    statusNotificationRequests.First().StatusTimestamp!.Value.ToIso8601());
                ClassicAssert.AreEqual(vendorId,                       statusNotificationRequests.First().VendorId);
                ClassicAssert.AreEqual(vendorErrorCode,                statusNotificationRequests.First().VendorErrorCode);

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

                var meterValuesRequests = new List<CP.MeterValuesRequest>();

                testCentralSystem01.OnMeterValuesRequest += async (timestamp, sender, connection, meterValuesRequest) => {
                    meterValuesRequests.Add(meterValuesRequest);
                };

                var connectorId     = Connector_Id.Parse(1);
                var meterValues     = new[] {
                                          new MeterValue(
                                              Timestamp.Now - TimeSpan.FromMinutes(5),
                                              new[] {
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
                                              new[] {
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

                var response1       = await chargePoint1.SendMeterValues(
                                          connectorId,
                                          meterValues,
                                          transactionId
                                      );


                ClassicAssert.AreEqual (OCPP.ResultCode.OK,                                              response1.Result.ResultCode);

                ClassicAssert.AreEqual (1,                                                               meterValuesRequests.Count);
                ClassicAssert.AreEqual (chargePoint1.Id,                                    meterValuesRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual (connectorId,                                                     meterValuesRequests.First().ConnectorId);
                ClassicAssert.AreEqual (transactionId,                                                   meterValuesRequests.First().TransactionId);

                ClassicAssert.AreEqual (meterValues.Length,                                              meterValuesRequests.First().MeterValues.Count());
                ClassicAssert.IsTrue   (meterValues.ElementAt(0).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                ClassicAssert.IsTrue   (meterValues.ElementAt(1).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.Count());
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.Count());

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Value);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Value);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Value);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Value);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Context);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Context);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Context);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Context);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Phase);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Location);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Location);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Location);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Location);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit);

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

                var stopTransactionRequests = new List<CP.StopTransactionRequest>();

                testCentralSystem01.OnStopTransactionRequest += async (timestamp, sender, connection, stopTransactionRequest) => {
                    stopTransactionRequests.Add(stopTransactionRequest);
                };

                var transactionId    = Transaction_Id.NewRandom;
                var stopTimestamp    = Timestamp.Now;
                var meterStop        = RandomExtensions.RandomUInt64();
                var idToken          = IdToken.NewRandom();
                var reason           = Reasons.EVDisconnected;
                var transactionData  = new[] {
                                           new MeterValue(
                                               Timestamp.Now - TimeSpan.FromMinutes(5),
                                               new[] {
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
                                               new[] {
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

                var response1        = await chargePoint1.StopTransaction(
                                           transactionId,
                                           stopTimestamp,
                                           meterStop,
                                           idToken,
                                           reason,
                                           transactionData
                                       );


                ClassicAssert.AreEqual (OCPP.ResultCode.OK,                                                  response1.Result.ResultCode);
                ClassicAssert.IsNotNull(response1.IdTagInfo);
                ClassicAssert.AreEqual (AuthorizationStatus.Accepted,                                        response1.IdTagInfo!.Value.Status);

                ClassicAssert.AreEqual (1,                                                                   stopTransactionRequests.Count);
                ClassicAssert.AreEqual (chargePoint1.Id,                                        stopTransactionRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual (transactionId,                                                       stopTransactionRequests.First().TransactionId);
                ClassicAssert.AreEqual (stopTimestamp.ToIso8601(),                                           stopTransactionRequests.First().StopTimestamp.ToIso8601());
                ClassicAssert.AreEqual (meterStop,                                                           stopTransactionRequests.First().MeterStop);
                ClassicAssert.AreEqual (idToken,                                                             stopTransactionRequests.First().IdTag);
                ClassicAssert.AreEqual (reason,                                                              stopTransactionRequests.First().Reason);

                ClassicAssert.AreEqual (transactionData.Length,                                              stopTransactionRequests.First().TransactionData.Count());
                ClassicAssert.IsTrue   (transactionData.ElementAt(0).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                ClassicAssert.IsTrue   (transactionData.ElementAt(1).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.Count(),                  stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.Count());
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.Count(),                  stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.Count());

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Value,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Value);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Value,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Value);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Value,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Value);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Value,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Value);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Context,     stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Context);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Context,     stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Context);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Context,     stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Context);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Context,     stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Context);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Format,      stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Format);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Format,      stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Format);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Format,      stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Format);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Format,      stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Format);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand,   stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Phase);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Phase,       stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Phase);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Location,    stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Location);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Location,    stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Location);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Location,    stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Location);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Location,    stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Location);

                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(0).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Unit);
                ClassicAssert.AreEqual (transactionData.ElementAt(0).SampledValues.ElementAt(1).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Unit);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(0).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Unit);
                ClassicAssert.AreEqual (transactionData.ElementAt(1).SampledValues.ElementAt(1).Unit,        stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Unit);

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

                var dataTransferRequests = new List<OCPP.CS.DataTransferRequest>();

                testCentralSystem01.OnIncomingDataTransferRequest += async (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id. Parse("GraphDefined OEM");
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response1  = await chargePoint1.TransferData(
                                     vendorId,
                                     messageId,
                                     data
                                 );


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);
                ClassicAssert.AreEqual(data.Reverse(),                 response1.Data);

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   dataTransferRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(data,                           dataTransferRequests.First().Data);

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

                var diagnosticsStatusNotifications = new List<CP.DiagnosticsStatusNotificationRequest>();

                testCentralSystem01.OnDiagnosticsStatusNotificationRequest += async (timestamp, sender, connection, diagnosticsStatusNotification) => {
                    diagnosticsStatusNotifications.Add(diagnosticsStatusNotification);
                };

                var status     = DiagnosticsStatus.Uploaded;

                var response1  = await chargePoint1.SendDiagnosticsStatusNotification(
                                     status
                                 );


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              diagnosticsStatusNotifications.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   diagnosticsStatusNotifications.First().NetworkingNodeId);
                ClassicAssert.AreEqual(status,                         diagnosticsStatusNotifications.First().Status);

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

                var firmwareStatusNotifications = new List<CP.FirmwareStatusNotificationRequest>();

                testCentralSystem01.OnFirmwareStatusNotificationRequest += async (timestamp, sender, connection, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                };

                var status     = FirmwareStatus.Installed;

                var response1  = await chargePoint1.SendFirmwareStatusNotification(
                                     status
                                 );


                ClassicAssert.AreEqual(OCPP.ResultCode.OK,             response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              firmwareStatusNotifications.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   firmwareStatusNotifications.First().NetworkingNodeId);
                ClassicAssert.AreEqual(status,                         firmwareStatusNotifications.First().Status);

            }

        }

        #endregion


    }

}
