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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

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

                var bootNotificationRequests = new List<CP.BootNotificationRequest>();

                testCentralSystem01.OnBootNotificationRequest += (timestamp, sender, connection, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargePoint1.SendBootNotification();

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                 Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                            Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(bootNotificationRequests.Count,                             Is.EqualTo(1));
                    Assert.That(bootNotificationRequests.First().NetworkingNodeId,          Is.EqualTo(chargePoint1.Id));
                    Assert.That(bootNotificationRequests.First().ChargePointVendor,         Is.EqualTo(chargePoint1.ChargePointVendor));
                    Assert.That(bootNotificationRequests.First().ChargePointSerialNumber,   Is.EqualTo(chargePoint1.ChargePointSerialNumber));
                    Assert.That(bootNotificationRequests.First().ChargeBoxSerialNumber,     Is.EqualTo(chargePoint1.ChargeBoxSerialNumber));
                    Assert.That(bootNotificationRequests.First().Iccid,                     Is.EqualTo(chargePoint1.Iccid));
                    Assert.That(bootNotificationRequests.First().IMSI,                      Is.EqualTo(chargePoint1.IMSI));
                    Assert.That(bootNotificationRequests.First().MeterType,                 Is.EqualTo(chargePoint1.MeterType));
                    Assert.That(bootNotificationRequests.First().MeterSerialNumber,         Is.EqualTo(chargePoint1.MeterSerialNumber));

                });

            }

        }

        #endregion

        #region ChargePoint_SendHeartbeat_Test()

        /// <summary>
        /// A test for sending heartbeats to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendHeartbeat_Test()
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

                var heartbeatRequests = new List<CP.HeartbeatRequest>();

                testCentralSystem01.OnHeartbeatRequest += (timestamp, sender, connection, heartbeatRequest) => {
                    heartbeatRequests.Add(heartbeatRequest);
                    return Task.CompletedTask;
                };

                var response = await chargePoint1.SendHeartbeat();

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                   Is.EqualTo(ResultCode.OK));
                    Assert.That(Timestamp.Now - response.CurrentTime < TimeSpan.FromSeconds(10));

                    Assert.That(heartbeatRequests.Count,                      Is.EqualTo(1));
                    Assert.That(heartbeatRequests.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));

                });

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

                var diagnosticsStatusNotifications = new List<CP.DiagnosticsStatusNotificationRequest>();

                testCentralSystem01.OnDiagnosticsStatusNotificationRequest += (timestamp, sender, connection, diagnosticsStatusNotification) => {
                    diagnosticsStatusNotifications.Add(diagnosticsStatusNotification);
                    return Task.CompletedTask;
                };

                var status    = DiagnosticsStatus.Uploaded;

                var response  = await chargePoint1.SendDiagnosticsStatusNotification(
                                          Status:   status
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                Is.EqualTo(ResultCode.OK));

                    Assert.That(diagnosticsStatusNotifications.Count,                      Is.EqualTo(1));
                    Assert.That(diagnosticsStatusNotifications.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(diagnosticsStatusNotifications.First().Status,             Is.EqualTo(status));

                });

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

                var firmwareStatusNotifications = new List<CP.FirmwareStatusNotificationRequest>();

                testCentralSystem01.OnFirmwareStatusNotificationRequest += (timestamp, sender, connection, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                    return Task.CompletedTask;
                };

                var status    = FirmwareStatus.Installed;

                var response  = await chargePoint1.SendFirmwareStatusNotification(
                                          Status:   status
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                             Is.EqualTo(ResultCode.OK));

                    Assert.That(firmwareStatusNotifications.Count,                      Is.EqualTo(1));
                    Assert.That(firmwareStatusNotifications.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(firmwareStatusNotifications.First().Status,             Is.EqualTo(status));

                });

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

                var authorizeRequests = new List<CP.AuthorizeRequest>();

                testCentralSystem01.OnAuthorizeRequest += (timestamp, sender, connection, authorizeRequest) => {
                    authorizeRequests.Add(authorizeRequest);
                    return Task.CompletedTask;
                };

                var idToken   = IdToken.NewRandom();
                var response  = await chargePoint1.Authorize(idToken);

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo.Status,                    Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(authorizeRequests.Count,                      Is.EqualTo(1));
                    Assert.That(authorizeRequests.First().NetworkingNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(authorizeRequests.First().IdTag,              Is.EqualTo(idToken));

                });

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

                var startTransactionRequests = new List<CP.StartTransactionRequest>();

                testCentralSystem01.OnStartTransactionRequest += (timestamp, sender, connection, startTransactionRequest) => {
                    startTransactionRequests.Add(startTransactionRequest);
                    return Task.CompletedTask;
                };

                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.NewRandom();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response        = await chargePoint1.SendStartTransactionNotification(
                                                connectorId,
                                                idToken,
                                                startTimestamp,
                                                meterStart,
                                                reservationId
                                            );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                    Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo.Status,                                     Is.EqualTo(AuthorizationStatus.Accepted));
                    Assert.That(response.TransactionId,                                        Is.Not.Null);

                    Assert.That(startTransactionRequests.Count,                                Is.EqualTo(1));
                    Assert.That(startTransactionRequests.First().NetworkingNodeId,             Is.EqualTo(chargePoint1.Id));
                    Assert.That(startTransactionRequests.First().IdTag,                        Is.EqualTo(idToken));
                    Assert.That(startTransactionRequests.First().ConnectorId,                  Is.EqualTo(connectorId));
                    Assert.That(startTransactionRequests.First().IdTag,                        Is.EqualTo(idToken));
                    Assert.That(startTransactionRequests.First().StartTimestamp.ToIso8601(),   Is.EqualTo(startTimestamp.ToIso8601()));
                    Assert.That(startTransactionRequests.First().MeterStart,                   Is.EqualTo(meterStart));
                    Assert.That(startTransactionRequests.First().ReservationId,                Is.EqualTo(reservationId));

                });

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

                var statusNotificationRequests = new List<CP.StatusNotificationRequest>();

                testCentralSystem01.OnStatusNotificationRequest += (timestamp, sender, connection, statusNotificationRequest) => {
                    statusNotificationRequests.Add(statusNotificationRequest);
                    return Task.CompletedTask;
                };

                var connectorId      = Connector_Id.Parse(1);
                var status           = ChargePointStatus.Available;
                var errorCode        = ChargePointErrorCodes.NoError;
                var info             = RandomExtensions.RandomString(20);
                var statusTimestamp  = Timestamp.Now;
                var vendorId         = "GraphDefined OEM";
                var vendorErrorCode  = "E0001";

                var response         = await chargePoint1.SendStatusNotification(
                                                 connectorId,
                                                 status,
                                                 errorCode,
                                                 info,
                                                 statusTimestamp,
                                                 vendorId,
                                                 vendorErrorCode
                                             );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                        Is.EqualTo(ResultCode.OK));

                    Assert.That(statusNotificationRequests.Count,                                  Is.EqualTo(1));
                    Assert.That(statusNotificationRequests.First().NetworkingNodeId,               Is.EqualTo(chargePoint1.Id));
                    Assert.That(statusNotificationRequests.First().ConnectorId,                    Is.EqualTo(connectorId));
                    Assert.That(statusNotificationRequests.First().Status,                         Is.EqualTo(status));
                    Assert.That(statusNotificationRequests.First().ErrorCode,                      Is.EqualTo(errorCode));
                    Assert.That(statusNotificationRequests.First().Info,                           Is.EqualTo(info));
                    Assert.That(statusNotificationRequests.First().StatusTimestamp?.ToIso8601(),   Is.EqualTo(statusTimestamp.ToIso8601()));
                    Assert.That(statusNotificationRequests.First().VendorId,                       Is.EqualTo(vendorId));
                    Assert.That(statusNotificationRequests.First().VendorErrorCode,                Is.EqualTo(vendorErrorCode));

                });

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

                var meterValuesRequests = new List<CP.MeterValuesRequest>();

                testCentralSystem01.OnMeterValuesRequest += (timestamp, sender, connection, meterValuesRequest) => {
                    meterValuesRequests.Add(meterValuesRequest);
                    return Task.CompletedTask;
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

                var response        = await chargePoint1.SendMeterValues(
                                                ConnectorId:     connectorId,
                                                MeterValues:     meterValues,
                                                TransactionId:   transactionId
                                            );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(meterValuesRequests.Count,                                                                   Is.EqualTo(1));
                    Assert.That(meterValuesRequests.First().NetworkingNodeId,                                                Is.EqualTo(chargePoint1.Id));
                    Assert.That(meterValuesRequests.First().ConnectorId,                                                     Is.EqualTo(connectorId));
                    Assert.That(meterValuesRequests.First().TransactionId,                                                   Is.EqualTo(transactionId));

                    Assert.That(meterValuesRequests.First().MeterValues.Count(),                                             Is.EqualTo(meterValues.Length));
                    Assert.That(meterValues.ElementAt(0).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                    Assert.That(meterValues.ElementAt(1).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.Count(),                  Is.EqualTo(meterValues.ElementAt(0).SampledValues.Count()));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.Count(),                  Is.EqualTo(meterValues.ElementAt(1).SampledValues.Count()));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Value,       Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Value));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Value,       Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Value));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Value,       Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Value));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Value,       Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Value));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Context,     Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Context));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Context,     Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Context));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Context,     Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Context));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Context,     Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Context));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Format));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Format));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Format));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Format));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Phase,       Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Phase));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Phase,       Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Phase));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Phase,       Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Phase));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Phase,       Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Phase));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Location,    Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Location));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Location,    Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Location));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Location,    Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Location));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Location,    Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Location));

                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        Is.EqualTo(meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit));
                    Assert.That(meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        Is.EqualTo(meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit));


                });

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

                var stopTransactionRequests = new List<CP.StopTransactionRequest>();

                testCentralSystem01.OnStopTransactionRequest += (timestamp, sender, connection, stopTransactionRequest) => {
                    stopTransactionRequests.Add(stopTransactionRequest);
                    return Task.CompletedTask;
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

                var response         = await chargePoint1.SendStopTransactionNotification(
                                                 TransactionId:     transactionId,
                                                 StopTimestamp:     stopTimestamp,
                                                 MeterStop:         meterStop,
                                                 IdTag:             idToken,
                                                 Reason:            reason,
                                                 TransactionData:   transactionData
                                             );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                                                          Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo,                                                                                  Is.Not.Null);
                    Assert.That(response.IdTagInfo?.Status,                                                                          Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(stopTransactionRequests.Count,                                                                       Is.EqualTo(1));
                    Assert.That(stopTransactionRequests.First().NetworkingNodeId,                                                    Is.EqualTo(chargePoint1.Id));

                    Assert.That(stopTransactionRequests.First().TransactionId,                                                       Is.EqualTo(transactionId));
                    Assert.That(stopTransactionRequests.First().StopTimestamp.ToIso8601(),                                           Is.EqualTo(stopTimestamp.ToIso8601()));
                    Assert.That(stopTransactionRequests.First().MeterStop,                                                           Is.EqualTo(meterStop));
                    Assert.That(stopTransactionRequests.First().IdTag,                                                               Is.EqualTo(idToken));
                    Assert.That(stopTransactionRequests.First().Reason,                                                              Is.EqualTo(reason));

                    Assert.That(stopTransactionRequests.First().TransactionData.Count(),                                             Is.EqualTo(transactionData.Length));
                    Assert.That(transactionData.ElementAt(0).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                    Assert.That(transactionData.ElementAt(1).Timestamp - stopTransactionRequests.First().TransactionData.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.Count(),                  Is.EqualTo(transactionData.ElementAt(0).SampledValues.Count()));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.Count(),                  Is.EqualTo(transactionData.ElementAt(1).SampledValues.Count()));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Value,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Value));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Value,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Value));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Value,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Value));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Value,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Value));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Context,     Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Context));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Context,     Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Context));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Context,     Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Context));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Context,     Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Context));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Format,      Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Format));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Format,      Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Format));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Format,      Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Format));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Format,      Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Format));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Phase,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Phase));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Phase,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Phase));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Phase,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Phase));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Phase,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Phase));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Location,    Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Location));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Location,    Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Location));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Location,    Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Location));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Location,    Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Location));

                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(0).Unit,        Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Unit));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(0).SampledValues.ElementAt(1).Unit,        Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Unit));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(0).Unit,        Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Unit));
                    Assert.That(stopTransactionRequests.First().TransactionData.ElementAt(1).SampledValues.ElementAt(1).Unit,        Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Unit));

                });

            }

        }

        #endregion


        #region ChargePoint_TransferTextData_Test()

        /// <summary>
        /// A test for sending custom text data to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_TransferTextData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                testCentralSystem01.OnIncomingDataTransferRequest += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await chargePoint1.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
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
                    Assert.That(dataTransferRequests.First().Data?.ToString(),   Is.EqualTo(data));

                });

            }

        }

        #endregion

        #region ChargePoint_TransferJObjectData_Test()

        /// <summary>
        /// A test for sending custom JObject data to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_TransferJObjectData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                testCentralSystem01.OnIncomingDataTransferRequest += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JObject(
                                     new JProperty(
                                         "key",
                                         RandomExtensions.RandomString(40)
                                     )
                                 );

                var response   = await chargePoint1.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
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

        #region ChargePoint_TransferJArrayData_Test()

        /// <summary>
        /// A test for sending custom JArray data to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_TransferJArrayData_Test()
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

                var dataTransferRequests = new List<DataTransferRequest>();

                testCentralSystem01.OnIncomingDataTransferRequest += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await chargePoint1.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
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


    }

}
