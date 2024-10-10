/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS
{

    /// <summary>
    /// Unit tests for charge points sending messages to the central system.
    /// </summary>
    [TestFixture]
    public class ChargePoint_Messages_Tests : ACPwithCSTests
    {

        #region ChargePoint_Authorize_Test()

        /// <summary>
        /// A test for authorizing id tokens against the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_Authorize_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var authorizeRequests = new List<AuthorizeRequest>();

                centralSystem.OCPP.IN.OnAuthorizeRequestReceived += (timestamp, sender, connection, authorizeRequest, ct) => {
                    authorizeRequests.Add(authorizeRequest);
                    return Task.CompletedTask;
                };

                var idTag     = IdToken.NewRandomRFID7();
                await centralSystem.RegisterToken(idTag, IdTagInfo.Accepted);

                var response  = await chargePoint.Authorize(idTag);

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo.Status,        Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(authorizeRequests.Count,          Is.EqualTo(1));
                    var authorizeRequest = authorizeRequests.First();

                    Assert.That(authorizeRequest.DestinationId,   Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(authorizeRequest.IdTag,           Is.EqualTo(idTag));

                });

            }

        }

        #endregion

        #region ChargePoint_LogStatusNotification_Test()

        /// <summary>
        /// A test for sending log status notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_LogStatusNotification_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var logStatusNotificationRequests = new List<LogStatusNotificationRequest>();

                centralSystem.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp, sender, connection, logStatusNotificationRequest, ct) => {
                    logStatusNotificationRequests.Add(logStatusNotificationRequest);
                    return Task.CompletedTask;
                };

                var uploadLogStatus  = UploadLogStatus.UploadFailure;
                var logRequestId     = 1;

                var response         = await chargePoint.SendLogStatusNotification(
                                                 Status:        uploadLogStatus,
                                                 LogRequestId:  logRequestId
                                             );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));

                    Assert.That(logStatusNotificationRequests.Count,                      Is.EqualTo(1));
                    var statusNotificationRequest = logStatusNotificationRequests.First();

                    Assert.That(statusNotificationRequest.NetworkPath.Source,             Is.EqualTo(chargePoint.Id));
                    Assert.That(statusNotificationRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(statusNotificationRequest.Status,                         Is.EqualTo(uploadLogStatus));
                    Assert.That(statusNotificationRequest.LogRequestId,                   Is.EqualTo(logRequestId));

                });

            }

        }

        #endregion


        #region ChargePoint_SendBootNotifications_Test1()

        /// <summary>
        /// A test for sending boot notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendBootNotifications_Test1()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var bootNotificationRequests = new List<BootNotificationRequest>();

                centralSystem.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                    return Task.CompletedTask;
                };


                var response = await chargePoint.SendBootNotification();


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                        Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                   Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(bootNotificationRequests.Count,                    Is.EqualTo(1));
                    var bootNotificationRequest = bootNotificationRequests.First();

                    Assert.That(bootNotificationRequest.NetworkPath.Source,        Is.EqualTo(chargePoint.Id));
                    Assert.That(bootNotificationRequest.DestinationId,             Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(bootNotificationRequest.ChargePointVendor,         Is.EqualTo(chargePoint.ChargePointVendor));
                    Assert.That(bootNotificationRequest.ChargePointModel,          Is.EqualTo(chargePoint.ChargePointModel));
                    Assert.That(bootNotificationRequest.ChargePointSerialNumber,   Is.EqualTo(chargePoint.ChargePointSerialNumber));
                    Assert.That(bootNotificationRequest.ChargeBoxSerialNumber,     Is.EqualTo(chargePoint.ChargeBoxSerialNumber));
                    Assert.That(bootNotificationRequest.Iccid,                     Is.EqualTo(chargePoint.Iccid));
                    Assert.That(bootNotificationRequest.IMSI,                      Is.EqualTo(chargePoint.IMSI));
                    Assert.That(bootNotificationRequest.MeterType,                 Is.EqualTo(chargePoint.UplinkEnergyMeter?.Model));
                    Assert.That(bootNotificationRequest.MeterSerialNumber,         Is.EqualTo(chargePoint.UplinkEnergyMeter?.SerialNumber));

                });

            }

        }

        #endregion

        #region ChargePoint_SendBootNotifications_Test2()

        /// <summary>
        /// A test for sending boot notifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendBootNotifications_Test2()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var bootNotificationRequests = new List<BootNotificationRequest>();

                centralSystem.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                    return Task.CompletedTask;
                };


                var chargePointVendor        = "GraphDefined OEM #2";
                var chargePointModel         = "VCP.2";
                var chargePointSerialNumber  = "SN-CP0002";
                var chargeBoxSerialNumber    = "SN-CB0002";
                var firmwareVersion          = "v0.2";
                var iccid                    = "0002";
                var imsi                     = "2222";
                var meterType                = "eMeter Two";
                var meterSerialNumber        = "SN-EN0002";

                var response = await chargePoint.SendBootNotification(
                                         ChargePointVendor:        chargePointVendor,
                                         ChargePointModel:         chargePointModel,
                                         ChargePointSerialNumber:  chargePointSerialNumber,
                                         ChargeBoxSerialNumber:    chargeBoxSerialNumber,
                                         FirmwareVersion:          firmwareVersion,
                                         Iccid:                    iccid,
                                         IMSI:                     imsi,
                                         MeterType:                meterType,
                                         MeterSerialNumber:        meterSerialNumber
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                        Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                   Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(bootNotificationRequests.Count,                    Is.EqualTo(1));
                    var bootNotificationRequest = bootNotificationRequests.First();

                    //Assert.That(bootNotificationRequests.First().DestinationId,             Is.EqualTo(chargePoint.Id));
                    Assert.That(bootNotificationRequest.ChargePointVendor,         Is.EqualTo(chargePointVendor));
                    Assert.That(bootNotificationRequest.ChargePointModel,          Is.EqualTo(chargePointModel));
                    Assert.That(bootNotificationRequest.ChargePointSerialNumber,   Is.EqualTo(chargePointSerialNumber));
                    Assert.That(bootNotificationRequest.ChargeBoxSerialNumber,     Is.EqualTo(chargeBoxSerialNumber));
                    Assert.That(bootNotificationRequest.Iccid,                     Is.EqualTo(iccid));
                    Assert.That(bootNotificationRequest.IMSI,                      Is.EqualTo(imsi));
                    Assert.That(bootNotificationRequest.MeterType,                 Is.EqualTo(meterType));
                    Assert.That(bootNotificationRequest.MeterSerialNumber,         Is.EqualTo(meterSerialNumber));

                });

            }

        }

        #endregion


        #region ChargePoint_SendDiagnosticsStatusNotification_Test()

        /// <summary>
        /// A test for sending DiagnosticsStatusNotifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendDiagnosticsStatusNotification_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var diagnosticsStatusNotifications = new List<DiagnosticsStatusNotificationRequest>();

                centralSystem.OCPP.IN.OnDiagnosticsStatusNotificationRequestReceived += (timestamp, sender, connection, diagnosticsStatusNotification, ct) => {
                    diagnosticsStatusNotifications.Add(diagnosticsStatusNotification);
                    return Task.CompletedTask;
                };

                var status    = DiagnosticsStatus.Uploaded;

                var response  = await chargePoint.SendDiagnosticsStatusNotification(
                                          DiagnosticsStatus:  status
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));

                    Assert.That(diagnosticsStatusNotifications.Count,               Is.EqualTo(1));
                    var diagnosticsStatusNotification = diagnosticsStatusNotifications.First();

                    Assert.That(diagnosticsStatusNotification.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(diagnosticsStatusNotification.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(diagnosticsStatusNotification.Status,               Is.EqualTo(status));

                });

            }

        }

        #endregion

        #region ChargePoint_SendFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending FirmwareStatusNotifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendFirmwareStatusNotification_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var firmwareStatusNotifications = new List<FirmwareStatusNotificationRequest>();

                centralSystem.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, firmwareStatusNotification, ct) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                    return Task.CompletedTask;
                };

                var status    = FirmwareStatus.Installed;

                var response  = await chargePoint.SendFirmwareStatusNotification(
                                          FirmwareStatus:  status
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));

                    Assert.That(firmwareStatusNotifications.Count,               Is.EqualTo(1));
                    var firmwareStatusNotification = firmwareStatusNotifications.First();

                    Assert.That(firmwareStatusNotification.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(firmwareStatusNotification.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(firmwareStatusNotification.Status,               Is.EqualTo(status));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var heartbeatRequests = new List<HeartbeatRequest>();

                centralSystem.OCPP.IN.OnHeartbeatRequestReceived += (timestamp, sender, connection, heartbeatRequest, ct) => {
                    heartbeatRequests.Add(heartbeatRequest);
                    return Task.CompletedTask;
                };

                var response = await chargePoint.SendHeartbeat();

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,            Is.EqualTo(ResultCode.OK));
                    Assert.That(Timestamp.Now - response.CurrentTime < TimeSpan.FromSeconds(10));

                    Assert.That(heartbeatRequests.Count,               Is.EqualTo(1));
                    var heartbeatRequest = heartbeatRequests.First();

                    Assert.That(heartbeatRequest.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(heartbeatRequest.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var meterValuesRequests = new List<MeterValuesRequest>();

                centralSystem.OCPP.IN.OnMeterValuesRequestReceived += (timestamp, sender, connection, meterValuesRequest, ct) => {
                    meterValuesRequests.Add(meterValuesRequest);
                    return Task.CompletedTask;
                };

                var connectorId     = Connector_Id.Parse(1);
                var meterValues     = new[] {
                                          new MeterValue(
                                              Timestamp.Now - TimeSpan.FromMinutes(5),
                                              [
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
                                              ]
                                          ),
                                          new MeterValue(
                                              Timestamp.Now,
                                              [
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
                                              ]
                                          )
                                      };
                var transactionId   = Transaction_Id.NewRandom;

                var response        = await chargePoint.SendMeterValues(
                                                ConnectorId:     connectorId,
                                                MeterValues:     meterValues,
                                                TransactionId:   transactionId
                                            );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(meterValuesRequests.Count,                                                                   Is.EqualTo(1));
                    Assert.That(meterValuesRequests.First().NetworkPath.Source,                                              Is.EqualTo(chargePoint.Id));
                    Assert.That(meterValuesRequests.First().DestinationId,                                                   Is.EqualTo(NetworkingNode_Id.CentralSystem));
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

        #region ChargePoint_SendSecurityEventNotification_Test()

        /// <summary>
        /// A test for sending SecurityEventNotifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendSecurityEventNotification_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var securityEventNotifications = new List<SecurityEventNotificationRequest>();

                centralSystem.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, securityEventNotification, ct) => {
                    securityEventNotifications.Add(securityEventNotification);
                    return Task.CompletedTask;
                };

                var securityEventType  = SecurityEvent.AttemptedReplayAttacks;
                var timestamp          = Timestamp.Now;
                var techInfo           = "test123";

                var response  = await chargePoint.SendSecurityEventNotification(
                                          Type:       securityEventType,
                                          Timestamp:  timestamp,
                                          TechInfo:   techInfo
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                     Is.EqualTo(ResultCode.OK));

                    Assert.That(securityEventNotifications.Count,               Is.EqualTo(1));
                    var securityEventNotification = securityEventNotifications.First();

                    Assert.That(securityEventNotification.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(securityEventNotification.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(securityEventNotification.Type,                 Is.EqualTo(securityEventType));
                    Assert.That(securityEventNotification.Timestamp,            Is.EqualTo(timestamp));
                    Assert.That(securityEventNotification.TechInfo,             Is.EqualTo(techInfo));

                });

            }

        }

        #endregion

        #region ChargePoint_SendSignedFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending SignedFirmwareStatusNotifications to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SendSignedFirmwareStatusNotification_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var signedFirmwareStatusNotifications = new List<SignedFirmwareStatusNotificationRequest>();

                centralSystem.OCPP.IN.OnSignedFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, signedFirmwareStatusNotification, ct) => {
                    signedFirmwareStatusNotifications.Add(signedFirmwareStatusNotification);
                    return Task.CompletedTask;
                };

                var status    = FirmwareStatus.Installed;

                var response  = await chargePoint.SendSignedFirmwareStatusNotification(
                                          FirmwareStatus:  status
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                            Is.EqualTo(ResultCode.OK));

                    Assert.That(signedFirmwareStatusNotifications.Count,               Is.EqualTo(1));
                    var signedFirmwareStatusNotification = signedFirmwareStatusNotifications.First();

                    Assert.That(signedFirmwareStatusNotification.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(signedFirmwareStatusNotification.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(signedFirmwareStatusNotification.Status,               Is.EqualTo(status));

                });

            }

        }

        #endregion

        #region ChargePoint_SignCertificate_Test1()

        /// <summary>
        /// A test for sending a SignCertificate request to the central system.
        /// </summary>
        [Test]
        public async Task ChargePoint_SignCertificate_Test1()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var signCertificateRequests = new List<SignCertificateRequest>();

                centralSystem.OCPP.IN.OnSignCertificateRequestReceived += (timestamp, sender, connection, signCertificateRequest, ct) => {
                    signCertificateRequests.Add(signCertificateRequest);
                    return Task.CompletedTask;
                };


                var CSR       = "";

                var response  = await chargePoint.SignCertificate(CSR);


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                             Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(signCertificateRequests.Count,               Is.EqualTo(1));
                    var signCertificateRequest = signCertificateRequests.First();

                    Assert.That(signCertificateRequest.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(signCertificateRequest.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(signCertificateRequest.CSR,                  Is.EqualTo(CSR));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var startTransactionRequests = new List<StartTransactionRequest>();

                centralSystem.OCPP.IN.OnStartTransactionRequestReceived += (timestamp, sender, connection, startTransactionRequest, ct) => {
                    startTransactionRequests.Add(startTransactionRequest);
                    return Task.CompletedTask;
                };

                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.NewRandom();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response        = await chargePoint.SendStartTransactionNotification(
                                                connectorId,
                                                idToken,
                                                startTimestamp,
                                                meterStart,
                                                reservationId
                                            );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                           Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo.Status,                            Is.EqualTo(AuthorizationStatus.Accepted));
                    Assert.That(response.TransactionId,                               Is.Not.Null);

                    Assert.That(startTransactionRequests.Count,                       Is.EqualTo(1));
                    var startTransactionRequest = startTransactionRequests.First();

                    Assert.That(startTransactionRequest.DestinationId,                Is.EqualTo(chargePoint.Id));
                    Assert.That(startTransactionRequest.IdTag,                        Is.EqualTo(idToken));
                    Assert.That(startTransactionRequest.ConnectorId,                  Is.EqualTo(connectorId));
                    Assert.That(startTransactionRequest.IdTag,                        Is.EqualTo(idToken));
                    Assert.That(startTransactionRequest.StartTimestamp.ToIso8601(),   Is.EqualTo(startTimestamp.ToIso8601()));
                    Assert.That(startTransactionRequest.MeterStart,                   Is.EqualTo(meterStart));
                    Assert.That(startTransactionRequest.ReservationId,                Is.EqualTo(reservationId));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var statusNotificationRequests = new List<StatusNotificationRequest>();

                centralSystem.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp, sender, connection, statusNotificationRequest, ct) => {
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

                var response         = await chargePoint.SendStatusNotification(
                                                 connectorId,
                                                 status,
                                                 errorCode,
                                                 info,
                                                 statusTimestamp,
                                                 vendorId,
                                                 vendorErrorCode
                                             );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));

                    Assert.That(statusNotificationRequests.Count,                         Is.EqualTo(1));
                    var statusNotificationRequest = statusNotificationRequests.First();

                    Assert.That(statusNotificationRequest.NetworkPath.Source,             Is.EqualTo(chargePoint.Id));
                    Assert.That(statusNotificationRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(statusNotificationRequest.ConnectorId,                    Is.EqualTo(connectorId));
                    Assert.That(statusNotificationRequest.Status,                         Is.EqualTo(status));
                    Assert.That(statusNotificationRequest.ErrorCode,                      Is.EqualTo(errorCode));
                    Assert.That(statusNotificationRequest.Info,                           Is.EqualTo(info));
                    Assert.That(statusNotificationRequest.StatusTimestamp?.ToIso8601(),   Is.EqualTo(statusTimestamp.ToIso8601()));
                    Assert.That(statusNotificationRequest.VendorId,                       Is.EqualTo(vendorId));
                    Assert.That(statusNotificationRequest.VendorErrorCode,                Is.EqualTo(vendorErrorCode));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var stopTransactionRequests = new List<StopTransactionRequest>();

                centralSystem.OCPP.IN.OnStopTransactionRequestReceived += (timestamp, sender, connection, stopTransactionRequest, ct) => {
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
                                               [
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
                                               ]
                                           ),
                                           new MeterValue(
                                               Timestamp.Now,
                                               [
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
                                               ]
                                           )
                                       };

                var response         = await chargePoint.SendStopTransactionNotification(
                                                 TransactionId:     transactionId,
                                                 StopTimestamp:     stopTimestamp,
                                                 MeterStop:         meterStop,
                                                 IdTag:             idToken,
                                                 Reason:            reason,
                                                 TransactionData:   transactionData
                                             );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                                                 Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTagInfo,                                                                         Is.Not.Null);
                    Assert.That(response.IdTagInfo?.Status,                                                                 Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(stopTransactionRequests.Count,                                                              Is.EqualTo(1));
                    var stopTransactionRequest = stopTransactionRequests.First();

                    Assert.That(stopTransactionRequest.NetworkPath.Source,                                                  Is.EqualTo(chargePoint.Id));
                    Assert.That(stopTransactionRequest.DestinationId,                                                       Is.EqualTo(NetworkingNode_Id.CentralSystem));

                    Assert.That(stopTransactionRequest.TransactionId,                                                       Is.EqualTo(transactionId));
                    Assert.That(stopTransactionRequest.StopTimestamp.ToIso8601(),                                           Is.EqualTo(stopTimestamp.ToIso8601()));
                    Assert.That(stopTransactionRequest.MeterStop,                                                           Is.EqualTo(meterStop));
                    Assert.That(stopTransactionRequest.IdTag,                                                               Is.EqualTo(idToken));
                    Assert.That(stopTransactionRequest.Reason,                                                              Is.EqualTo(reason));

                    Assert.That(stopTransactionRequest.TransactionData.Count(),                                             Is.EqualTo(transactionData.Length));
                    Assert.That(transactionData.ElementAt(0).Timestamp - stopTransactionRequest.TransactionData.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                    Assert.That(transactionData.ElementAt(1).Timestamp - stopTransactionRequest.TransactionData.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.Count(),                  Is.EqualTo(transactionData.ElementAt(0).SampledValues.Count()));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.Count(),                  Is.EqualTo(transactionData.ElementAt(1).SampledValues.Count()));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Value,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Value));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Value,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Value));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Value,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Value));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Value,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Value));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Context,     Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Context));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Context,     Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Context));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Context,     Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Context));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Context,     Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Context));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Format,      Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Format));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Format,      Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Format));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Format,      Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Format));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Format,      Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Format));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Measurand));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Measurand));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand,   Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Measurand));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand,   Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Measurand));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Phase,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Phase));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Phase,       Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Phase));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Phase,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Phase));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Phase,       Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Phase));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Location,    Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Location));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Location,    Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Location));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Location,    Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Location));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Location,    Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Location));

                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(0).Unit,        Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(0).Unit));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(0).SampledValues.ElementAt(1).Unit,        Is.EqualTo(transactionData.ElementAt(0).SampledValues.ElementAt(1).Unit));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(0).Unit,        Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(0).Unit));
                    Assert.That(stopTransactionRequest.TransactionData.ElementAt(1).SampledValues.ElementAt(1).Unit,        Is.EqualTo(transactionData.ElementAt(1).SampledValues.ElementAt(1).Unit));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                centralSystem.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await chargePoint.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                   Is.Not.Null);
                    Assert.That(response.Data?.Type,                             Is.EqualTo(JTokenType.Array));
                    Assert.That(response.Data?[0]?.Value<String>()?.Reverse(),   Is.EqualTo(data[0]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                      Is.EqualTo(1));
                    var dataTransferRequest = dataTransferRequests.First();

                    Assert.That(dataTransferRequest.NetworkPath.Source,                                              Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequest.DestinationId,                                                   Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(dataTransferRequest.VendorId,                    Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequest.MessageId,                   Is.EqualTo(messageId));
                    Assert.That(dataTransferRequest.Data?.Type,                  Is.EqualTo(JTokenType.Array));
                    Assert.That(dataTransferRequest.Data?[0]?.Value<String>(),   Is.EqualTo(data[0]?.Value<String>()));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                centralSystem.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
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

                var response   = await chargePoint.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                          Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                     Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                       Is.Not.Null);
                    Assert.That(response.Data?.Type,                                 Is.EqualTo(JTokenType.Object));
                    Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),   Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                          Is.EqualTo(1));
                    var dataTransferRequest = dataTransferRequests.First();

                    Assert.That(dataTransferRequest.NetworkPath.Source,              Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequest.DestinationId,                   Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(dataTransferRequest.VendorId,                        Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequest.MessageId,                       Is.EqualTo(messageId));
                    Assert.That(dataTransferRequest.Data?.Type,                      Is.EqualTo(JTokenType.Object));
                    Assert.That(dataTransferRequest.Data?["key"]?.Value<String>(),   Is.EqualTo(data["key"]?.Value<String>()));

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
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                centralSystem.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await chargePoint.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                            Is.Not.Null);
                    Assert.That(response.Data?.Type,                      Is.EqualTo(JTokenType.String));
                    Assert.That(response.Data?.ToString(),                Is.EqualTo(data.Reverse()));

                    Assert.That(dataTransferRequests.Count,               Is.EqualTo(1));
                    var dataTransferRequest = dataTransferRequests.First();

                    Assert.That(dataTransferRequest.NetworkPath.Source,   Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequest.DestinationId,        Is.EqualTo(NetworkingNode_Id.CentralSystem));
                    Assert.That(dataTransferRequest.VendorId,             Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequest.MessageId,            Is.EqualTo(messageId));
                    Assert.That(dataTransferRequest.Data?.ToString(),     Is.EqualTo(data));

                });

            }

        }

        #endregion


    }

}
