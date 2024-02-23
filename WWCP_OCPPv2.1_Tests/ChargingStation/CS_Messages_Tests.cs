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
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_Messages_Tests : AChargingStationTests
    {

        #region Init_Test()

        /// <summary>
        /// A test for creating charging stations.
        /// </summary>
        [Test]
        public void ChargingStation_Init_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                ClassicAssert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
                ClassicAssert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
                ClassicAssert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

            }

        }

        #endregion


        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var bootNotificationRequests= new ConcurrentList<BootNotificationRequest>();

                testCSMS01.OnBootNotificationRequestReceived += (timestamp, sender, connection,  bootNotificationRequest) => {
                    bootNotificationRequests.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                var reason    = BootReason.PowerUp;
                var response  = await chargingStation1.SendBootNotification(
                                    BootReason:   reason,
                                    CustomData:   null
                                );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                            Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                       Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(bootNotificationRequests.Count,                        Is.EqualTo(1));
                    Assert.That(bootNotificationRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(bootNotificationRequests.First().Reason,               Is.EqualTo(reason));
                    Assert.That(bootNotificationRequests.First().ChargingStation,      Is.Not.Null);

                });

                var chargingStation = bootNotificationRequests.First().ChargingStation;
                if (chargingStation is not null)
                {

                    Assert.That(chargingStation.Model,                              Is.EqualTo(chargingStation1.Model));
                    Assert.That(chargingStation.VendorName,                         Is.EqualTo(chargingStation1.VendorName));
                    Assert.That(chargingStation.SerialNumber,                       Is.EqualTo(chargingStation1.SerialNumber));
                    Assert.That(chargingStation.FirmwareVersion,                    Is.EqualTo(chargingStation1.FirmwareVersion));
                    Assert.That(chargingStation.Modem,                              Is.Not.Null);

                    if (chargingStation. Modem is not null &&
                        chargingStation1.Modem is not null)
                    {
                        Assert.That(chargingStation.Modem.ICCID,                    Is.EqualTo(chargingStation1.Modem.ICCID));
                        Assert.That(chargingStation.Modem.IMSI,                     Is.EqualTo(chargingStation1.Modem.IMSI));
                    }

                }

            }

        }

        #endregion

        #region SendFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending firmware status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendFirmwareStatusNotification_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var firmwareStatusNotificationRequests= new ConcurrentList<CS.FirmwareStatusNotificationRequest>();

                testCSMS01.OnFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, firmwareStatusNotificationRequest) => {
                    firmwareStatusNotificationRequests.TryAdd(firmwareStatusNotificationRequest);
                    return Task.CompletedTask;
                };

                var status    = FirmwareStatus.Installed;
                var response  = await chargingStation1.SendFirmwareStatusNotification(
                                    Status:       status,
                                    CustomData:   null
                                );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               firmwareStatusNotificationRequests.Count);
                ClassicAssert.AreEqual(status,          firmwareStatusNotificationRequests.First().Status);

            }

        }

        #endregion

        #region SendPublishFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending piblish firmware status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendPublishFirmwareStatusNotification_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var publishFirmwareStatusNotificationRequests = new ConcurrentList<CS.PublishFirmwareStatusNotificationRequest>();

                testCSMS01.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, publishFirmwareStatusNotificationRequest) => {
                    publishFirmwareStatusNotificationRequests.TryAdd(publishFirmwareStatusNotificationRequest);
                    return Task.CompletedTask;
                };

                var status    = PublishFirmwareStatus.Published;
                var url1      = URL.Parse("https://example.org/firmware.bin");

                var response  = await chargingStation1.SendPublishFirmwareStatusNotification(
                                    Status:                                       status,
                                    PublishFirmwareStatusNotificationRequestId:   0,
                                    DownloadLocations:                            new[] { url1 },
                                    CustomData:                                   null
                                );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               publishFirmwareStatusNotificationRequests.Count);
                ClassicAssert.AreEqual(status,          publishFirmwareStatusNotificationRequests.First().Status);

            }

        }

        #endregion

        #region SendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the CSMS.
        /// </summary>
        [Test]
        public async Task SendHeartbeats_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var heartbeatRequests= new ConcurrentList<CS.HeartbeatRequest>();

                testCSMS01.OnHeartbeatRequestReceived += (timestamp, sender, connection, heartbeatRequest) => {
                    heartbeatRequests.TryAdd(heartbeatRequest);
                    return Task.CompletedTask;
                };


                var response = await chargingStation1.SendHeartbeat(
                                   CustomData:   null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);
                ClassicAssert.IsTrue  (Timestamp.Now - response.CurrentTime < TimeSpan.FromSeconds(10));

                ClassicAssert.AreEqual(1,               heartbeatRequests.Count);

            }

        }

        #endregion

        #region NotifyEvent_Test()

        /// <summary>
        /// A test for notifying the CSMS about events.
        /// </summary>
        [Test]
        public async Task NotifyEvent_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyEventRequests= new ConcurrentList<CS.NotifyEventRequest>();

                testCSMS01.OnNotifyEventRequestReceived += (timestamp, sender, connection, notifyEventRequest) => {
                    notifyEventRequests.TryAdd(notifyEventRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEvent(
                                   GeneratedAt:      Timestamp.Now,
                                   SequenceNumber:   1,
                                   EventData:        new[] {
                                                         new EventData(
                                                             EventId:                 Event_Id.NewRandom,
                                                             Timestamp:               Timestamp.Now,
                                                             Trigger:                 EventTriggers.Alerting,
                                                             ActualValue:             "ALERTA!",
                                                             EventNotificationType:   EventNotificationType.HardWiredMonitor,
                                                             Component:               new Component(
                                                                                          Name:         "Alert System!",
                                                                                          Instance:     "Alert System #1",
                                                                                          EVSE:         new EVSE(
                                                                                                            Id:            EVSE_Id.Parse(1),
                                                                                                            ConnectorId:   Connector_Id.Parse(1),
                                                                                                            CustomData:    null
                                                                                                        ),
                                                                                          CustomData:   null
                                                                                      ),
                                                             Variable:                new Variable(
                                                                                          Name:         "Temperature Sensors",
                                                                                          Instance:     "Temperature Sensor #1",
                                                                                          CustomData:   null
                                                                                      ),
                                                             Cause:                   Event_Id.NewRandom,
                                                             TechCode:                "Tech Code #1",
                                                             TechInfo:                "Tech Info #1",
                                                             Cleared:                 false,
                                                             TransactionId:           Transaction_Id.       NewRandom,
                                                             VariableMonitoringId:    VariableMonitoring_Id.NewRandom,
                                                             CustomData:              null
                                                         )
                                                     },
                                   ToBeContinued:    false,
                                   CustomData:       null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyEventRequests.Count);

            }

        }

        #endregion

        #region SendSecurityEventNotification_Test()

        /// <summary>
        /// A test for sending security event notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendSecurityEventNotification_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var securityEventNotificationRequests= new ConcurrentList<CS.SecurityEventNotificationRequest>();

                testCSMS01.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.TryAdd(securityEventNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendSecurityEventNotification(
                                   Type:         SecurityEventType.MemoryExhaustion,
                                   Timestamp:    Timestamp.Now,
                                   TechInfo:     "Too many open TCP sockets!",
                                   CustomData:   null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               securityEventNotificationRequests.Count);

            }

        }

        #endregion

        #region NotifyReport_Test()

        /// <summary>
        /// A test for notifying the CSMS about reports.
        /// </summary>
        [Test]
        public async Task NotifyReport_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyReportRequests= new ConcurrentList<CS.NotifyReportRequest>();

                testCSMS01.OnNotifyReportRequestReceived += (timestamp, sender, connection, notifyReportRequest) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyReport(
                                   NotifyReportRequestId:   1,
                                   SequenceNumber:          1,
                                   GeneratedAt:             Timestamp.Now,
                                   ReportData:              new[] {
                                                                new ReportData(
                                                                    Component:                 new Component(
                                                                                                   Name:                 "Alert System!",
                                                                                                   Instance:             "Alert System #1",
                                                                                                   EVSE:                 new EVSE(
                                                                                                                             Id:            EVSE_Id.Parse(1),
                                                                                                                             ConnectorId:   Connector_Id.Parse(1),
                                                                                                                             CustomData:    null
                                                                                                                         ),
                                                                                                   CustomData:           null
                                                                                               ),
                                                                    Variable:                  new Variable(
                                                                                                   Name:                 "Temperature Sensors",
                                                                                                   Instance:             "Temperature Sensor #1",
                                                                                                   CustomData:           null
                                                                                               ),
                                                                    VariableAttributes:        new[] {
                                                                                                   new VariableAttribute(
                                                                                                       Type:             AttributeTypes.Actual,
                                                                                                       Value:            "123",
                                                                                                       Mutability:       MutabilityTypes.ReadWrite,
                                                                                                       Persistent:       true,
                                                                                                       Constant:         false,
                                                                                                       CustomData:       null
                                                                                                   )
                                                                                               },
                                                                    VariableCharacteristics:   new VariableCharacteristics(
                                                                                                   DataType:             DataTypes.Decimal,
                                                                                                   SupportsMonitoring:   true,
                                                                                                   Unit:                 UnitsOfMeasure.Celsius(
                                                                                                                             Multiplier:   1,
                                                                                                                             CustomData:   null
                                                                                                                         ),
                                                                                                   MinLimit:             0.1M,
                                                                                                   MaxLimit:             9.9M,
                                                                                                   ValuesList:           new[] { "" },
                                                                                                   CustomData:           null
                                                                                               ),
                                                                    CustomData:                null
                                                                )
                                                            },
                                   CustomData:              null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyReportRequests.Count);

            }

        }

        #endregion

        #region NotifyMonitoringReport_Test()

        /// <summary>
        /// A test for notifying the CSMS about monitoring reports.
        /// </summary>
        [Test]
        public async Task NotifyMonitoringReport_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyMonitoringReportRequests= new ConcurrentList<CS.NotifyMonitoringReportRequest>();

                testCSMS01.OnNotifyMonitoringReportRequestReceived += (timestamp, sender, connection, notifyMonitoringReportRequest) => {
                    notifyMonitoringReportRequests.TryAdd(notifyMonitoringReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyMonitoringReport(
                                   NotifyMonitoringReportRequestId:   1,
                                   SequenceNumber:                    1,
                                   GeneratedAt:                       Timestamp.Now,
                                   MonitoringData:                    new[] {
                                                                          new MonitoringData(
                                                                              Component:             new Component(
                                                                                                         Name:             "Alert System!",
                                                                                                         Instance:         "Alert System #1",
                                                                                                         EVSE:             new EVSE(
                                                                                                                               Id:            EVSE_Id.Parse(1),
                                                                                                                               ConnectorId:   Connector_Id.Parse(1),
                                                                                                                               CustomData:    null
                                                                                                                           ),
                                                                                                         CustomData:       null
                                                                                                     ),
                                                                              Variable:              new Variable(
                                                                                                         Name:             "Temperature Sensors",
                                                                                                         Instance:         "Temperature Sensor #1",
                                                                                                         CustomData:       null
                                                                                                     ),
                                                                              VariableMonitorings:   new[] {
                                                                                                         new VariableMonitoring(
                                                                                                             Id:            VariableMonitoring_Id.NewRandom,
                                                                                                             Transaction:   true,
                                                                                                             Value:         1.01M,
                                                                                                             Type:          MonitorType.Periodic,
                                                                                                             Severity:      Severities.Warning,
                                                                                                             CustomData:    null
                                                                                                         )
                                                                                                     },
                                                                              CustomData:            null
                                                                          )
                                                                      },
                                   ToBeContinued:                     false,
                                   CustomData:                        null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyMonitoringReportRequests.Count);

            }

        }

        #endregion

        #region SendLogStatusNotification_Test()

        /// <summary>
        /// A test for sending log status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendLogStatusNotification_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var securityEventNotificationRequests= new ConcurrentList<CS.LogStatusNotificationRequest>();

                testCSMS01.OnLogStatusNotificationRequestReceived += (timestamp, sender, connection, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.TryAdd(securityEventNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendLogStatusNotification(
                                   Status:         UploadLogStatus.Uploaded,
                                   LogRequestId:   1,
                                   CustomData:     null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               securityEventNotificationRequests.Count);

            }

        }

        #endregion


        #region TransferTextData_Test()

        /// <summary>
        /// A test for transfering text data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferTextData_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var dataTransferRequests= new ConcurrentList<DataTransferRequest>();

                testCSMS01.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       GraphDefined;
                var messageId  = Message_Id.      Parse       (RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,    response.Result.ResultCode);
                ClassicAssert.AreEqual(data.Reverse(),   response.Data?.ToString());

                ClassicAssert.AreEqual(1,                dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,         dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,        dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(data,             dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion

        #region TransferJObjectData_Test()

        /// <summary>
        /// A test for transfering JObject data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJObjectData_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var dataTransferRequests= new ConcurrentList<DataTransferRequest>();

                testCSMS01.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
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

                var response   = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,                  response.Result.ResultCode);
                ClassicAssert.AreEqual(JTokenType.Object,              response.Data?.Type);
                ClassicAssert.AreEqual(data["key"]?.Value<String>(),   response.Data?["key"]?.Value<String>()?.Reverse());

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(JTokenType.Object,              dataTransferRequests.First().Data?.Type);
                ClassicAssert.AreEqual(data["key"]?.Value<String>(),   dataTransferRequests.First().Data?["key"]?.Value<String>());

            }

        }

        #endregion

        #region TransferJArrayData_Test()

        /// <summary>
        /// A test for transfering JArray data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJArrayData_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var dataTransferRequests= new ConcurrentList<DataTransferRequest>();

                testCSMS01.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,             response.Result.ResultCode);
                ClassicAssert.AreEqual(JTokenType.Array,           response.Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),   response.Data?[0]?.Value<String>()?.Reverse());

                ClassicAssert.AreEqual(1,                          dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,                   dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                  dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(JTokenType.Array,           dataTransferRequests.First().Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),   dataTransferRequests.First().Data?[0]?.Value<String>());

            }

        }

        #endregion


        #region SendCertificateSigningRequest_Test()

        /// <summary>
        /// A test for sending a certificate signing request to the CSMS.
        /// </summary>
        [Test]
        public async Task SendCertificateSigningRequest_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyReportRequests= new ConcurrentList<CS.SignCertificateRequest>();

                testCSMS01.OnSignCertificateRequestReceived += (timestamp, sender, connection, notifyReportRequest) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendCertificateSigningRequest(
                                   CSR:                        "0x1234",
                                   SignCertificateRequestId:   1,
                                   CertificateType:            CertificateSigningUse.ChargingStationCertificate,
                                   CustomData:                 null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyReportRequests.Count);

            }

        }

        #endregion

        #region Get15118EVCertificate_Test()

        /// <summary>
        /// A test for receiving a 15118 EV contract certificate from the CSMS.
        /// </summary>
        [Test]
        public async Task Get15118EVCertificate_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyReportRequests= new ConcurrentList<CS.Get15118EVCertificateRequest>();

                testCSMS01.OnGet15118EVCertificateRequestReceived += (timestamp, sender, connection, notifyReportRequest) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.Get15118EVCertificate(
                                   ISO15118SchemaVersion:   ISO15118SchemaVersion.Parse("15118-20:BastelBrothers"),
                                   CertificateAction:       CertificateAction.Install,
                                   EXIRequest:              EXIData.Parse("0x1234"),
                                   CustomData:              null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    notifyReportRequests.Count);

            }

        }

        #endregion

        #region GetCertificateStatus_Test()

        /// <summary>
        /// A test for notifying the CSMS about reports.
        /// </summary>
        [Test]
        public async Task GetCertificateStatus_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyReportRequests= new ConcurrentList<CS.GetCertificateStatusRequest>();

                testCSMS01.OnGetCertificateStatusRequestReceived += (timestamp, sender, connection, notifyReportRequest) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.GetCertificateStatus(
                                   OCSPRequestData:   new OCSPRequestData(
                                                          HashAlgorithm:    HashAlgorithms.SHA256,
                                                          IssuerNameHash:   "0x1234",
                                                          IssuerKeyHash:    "0x5678",
                                                          SerialNumber:     "12345678",
                                                          ResponderURL:     URL.Parse("https://example.org/12345678"),
                                                          CustomData:       null
                                                      ),
                                   CustomData:        null
                               );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,   Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyReportRequests.Count,   Is.EqualTo(1));

                });

            }

        }

        #endregion

        #region GetCRLRequest_Test()

        /// <summary>
        /// A test for fetching a certificate revocation list from the CSMS for the specified certificate.
        /// </summary>
        [Test]
        public async Task GetCRLRequest_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var getCRLRequests= new ConcurrentList<CS.GetCRLRequest>();

                testCSMS01.OnGetCRLRequestReceived += (timestamp, sender, connection, getCRLRequest) => {
                    getCRLRequests.TryAdd(getCRLRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.GetCRLRequest(
                                   GetCRLRequestId:       1,
                                   CertificateHashData:   new CertificateHashData(
                                                              HashAlgorithm:         HashAlgorithms.SHA256,
                                                              IssuerNameHash:       "f2311e9a995dfbd006bfc909e480987dc2d440ae6eaf1746efdadc638a295f65",
                                                              IssuerPublicKeyHash:  "99084534bbe5f6ceaffa2e65ff1ad5301c4c359b599d6edd486a475071f715fb",
                                                              SerialNumber:         "23"
                                                          ),
                                   CustomData:            null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               getCRLRequests.Count);

            }

        }

        #endregion


        #region SendReservationStatusUpdate_Test()

                /// <summary>
                /// A test for sending reservation status updates to the CSMS.
                /// </summary>
                [Test]
        public async Task SendReservationStatusUpdate_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var securityEventNotificationRequests= new ConcurrentList<CS.ReservationStatusUpdateRequest>();

                testCSMS01.OnReservationStatusUpdateRequestReceived += (timestamp, sender, connection, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.TryAdd(securityEventNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendReservationStatusUpdate(
                                   ReservationId:             Reservation_Id.NewRandom,
                                   ReservationUpdateStatus:   ReservationUpdateStatus.Expired,
                                   CustomData:                null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               securityEventNotificationRequests.Count);

            }

        }

        #endregion

        #region Authorize_Test()

        /// <summary>
        /// A test for authorizing id tokens against the CSMS.
        /// </summary>
        [Test]
        public async Task Authorize_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var authorizeRequests= new ConcurrentList<CS.AuthorizeRequest>();

                testCSMS01.OnAuthorizeRequestReceived += (timestamp, sender, connection, authorizeRequest) => {
                    authorizeRequests.TryAdd(authorizeRequest);
                    return Task.CompletedTask;
                };


                var idToken   = IdToken.NewRandomRFID();
                var response  = await chargingStation1.Authorize(
                                    IdToken:                       idToken,
                                    Certificate:                   null,
                                    ISO15118CertificateHashData:   null,
                                    CustomData:                    null
                                );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                     Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTokenInfo.Status,                    Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(authorizeRequests.Count,                        Is.EqualTo(1));
                    Assert.That(authorizeRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(authorizeRequests.First().IdToken,              Is.EqualTo(idToken));

                });

            }

        }

        #endregion

        #region NotifyEVChargingNeeds_Test()

        /// <summary>
        /// A test for notifying the CSMS about EV charging needs.
        /// </summary>
        [Test]
        public async Task NotifyEVChargingNeeds_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyEVChargingNeedsRequests= new ConcurrentList<CS.NotifyEVChargingNeedsRequest>();

                testCSMS01.OnNotifyEVChargingNeedsRequestReceived += (timestamp, sender, connection, notifyEVChargingNeedsRequest) => {
                    notifyEVChargingNeedsRequests.TryAdd(notifyEVChargingNeedsRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEVChargingNeeds(
                                         EVSEId:              EVSE_Id.Parse(1),
                                         ChargingNeeds:       new ChargingNeeds(
                                                                  RequestedEnergyTransferMode:   EnergyTransferMode.AC_ThreePhases,
                                                                  DepartureTime:                 Timestamp.Now + TimeSpan.FromHours(3),
                                                                  ACChargingParameters:          new ACChargingParameters(
                                                                                                     EnergyAmount:       WattHour.     Parse( 20),
                                                                                                     EVMinCurrent:       Ampere.       Parse(  6),
                                                                                                     EVMaxCurrent:       Ampere.       Parse( 32),
                                                                                                     EVMaxVoltage:       Volt.         Parse(230),
                                                                                                     CustomData:         null
                                                                                                 ),
                                                                  DCChargingParameters:          new DCChargingParameters(
                                                                                                     EVMaxCurrent:       Ampere.       Parse( 20),
                                                                                                     EVMaxVoltage:       Volt.         Parse(900),
                                                                                                     EnergyAmount:       WattHour.     Parse(300),
                                                                                                     EVMaxPower:         Watt.         Parse( 60),
                                                                                                     StateOfCharge:      PercentageByte.Parse( 23),
                                                                                                     EVEnergyCapacity:   WattHour.     Parse(250),
                                                                                                     FullSoC:            PercentageByte.Parse( 95),
                                                                                                     BulkSoC:            PercentageByte.Parse( 80),
                                                                                                     CustomData:         null
                                                                                                 ),
                                                                  CustomData:                    null
                                                              ),
                                         MaxScheduleTuples:   16,
                                         CustomData:          null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyEVChargingNeedsRequests.Count);

            }

        }

        #endregion

        #region SendTransactionEvent_Test()

        /// <summary>
        /// A test for sending a transaction event to the CSMS.
        /// </summary>
        [Test]
        public async Task SendTransactionEvent_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests= new ConcurrentList<CS.TransactionEventRequest>();

                testCSMS01.OnTransactionEventRequestReceived += (timestamp, sender, connection, transactionEventRequest) => {
                    transactionEventRequests.TryAdd(transactionEventRequest);
                    return Task.CompletedTask;
                };

                var evseId          = EVSE_Id.     Parse(1);
                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.     NewRandomRFID();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response        = await chargingStation1.SendTransactionEvent(

                                          EventType:               TransactionEvents.Started,
                                          Timestamp:               startTimestamp,
                                          TriggerReason:           TriggerReason.Authorized,
                                          SequenceNumber:          0,
                                          TransactionInfo:         new Transaction(
                                                                       TransactionId:       Transaction_Id.NewRandom,
                                                                       ChargingState:       ChargingStates.Charging,
                                                                       TimeSpentCharging:   TimeSpan.FromSeconds(3),
                                                                       StoppedReason:       null,
                                                                       RemoteStartId:       null,
                                                                       CustomData:          null
                                                                   ),

                                          Offline:                 null,
                                          NumberOfPhasesUsed:      null,
                                          CableMaxCurrent:         null,
                                          ReservationId:           reservationId,
                                          IdToken:                 idToken,
                                          EVSE:                    new EVSE(
                                                                       Id:                  evseId,
                                                                       ConnectorId:         connectorId,
                                                                       CustomData:          null
                                                                   ),
                                          MeterValues:             new[] {
                                                                       new MeterValue(
                                                                           SampledValues:   new[] {

                                                                                                new SampledValue(
                                                                                                    Value:                 meterStart,
                                                                                                    Context:               ReadingContext.TransactionBegin,
                                                                                                    Measurand:             Measurand.Energy_Active_Export_Interval,
                                                                                                    Phase:                 Phases.L1,
                                                                                                    MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                                    SignedMeterValue:      new SignedMeterValue(
                                                                                                                               SignedMeterData:   meterStart.ToString(),
                                                                                                                               SigningMethod:     "secp256r1",
                                                                                                                               EncodingMethod:    "base64",
                                                                                                                               PublicKey:         "0x1234",
                                                                                                                               CustomData:        null
                                                                                                                           ),
                                                                                                    UnitOfMeasure:         UnitsOfMeasure.kW(
                                                                                                                               Multiplier:   0,
                                                                                                                               CustomData:   null
                                                                                                                           ),
                                                                                                    CustomData:            null
                                                                                                )

                                                                                            },
                                                                           Timestamp:       startTimestamp,
                                                                           CustomData:      null
                                                                       )
                                                                   },
                                          PreconditioningStatus:   PreconditioningStatus.Ready,
                                          CustomData:              null

                                      );


                ClassicAssert.AreEqual(ResultCode.OK,                 response.Result.ResultCode);
                //ClassicAssert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTokenInfo.Status);
                //ClassicAssert.IsTrue  (response1.TransactionId.IsNotNullOrEmpty);

                ClassicAssert.AreEqual(1,                              transactionEventRequests.Count);
                //ClassicAssert.AreEqual(connectorId,                    transactionEventRequests.First().ConnectorId);
                //ClassicAssert.AreEqual(idToken,                        transactionEventRequests.First().IdTag);
                //ClassicAssert.AreEqual(startTimestamp.ToIso8601(),     transactionEventRequests.First().StartTimestamp.ToIso8601());
                //ClassicAssert.AreEqual(meterStart,                     transactionEventRequests.First().MeterStart);
                //ClassicAssert.AreEqual(reservationId,                  transactionEventRequests.First().ReservationId);

            }

        }

        #endregion

        #region SendStatusNotification_Test()

        /// <summary>
        /// A test for sending status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendStatusNotification_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var statusNotificationRequests= new ConcurrentList<CS.StatusNotificationRequest>();

                testCSMS01.OnStatusNotificationRequestReceived += (timestamp, sender, connection, statusNotificationRequest) => {
                    statusNotificationRequests.TryAdd(statusNotificationRequest);
                    return Task.CompletedTask;
                };

                var evseId           = EVSE_Id.     Parse(1);
                var connectorId      = Connector_Id.Parse(1);
                var connectorStatus  = ConnectorStatus.Available;
                var statusTimestamp  = Timestamp.Now;

                var response         = await chargingStation1.SendStatusNotification(
                                           EVSEId:        evseId,
                                           ConnectorId:   connectorId,
                                           Timestamp:     statusTimestamp,
                                           Status:        connectorStatus,
                                           CustomData:    null
                                       );


                ClassicAssert.AreEqual(ResultCode.OK,                response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                             statusNotificationRequests.Count);
                ClassicAssert.AreEqual(evseId,                        statusNotificationRequests.First().EVSEId);
                ClassicAssert.AreEqual(connectorId,                   statusNotificationRequests.First().ConnectorId);
                ClassicAssert.AreEqual(connectorStatus,               statusNotificationRequests.First().ConnectorStatus);
                ClassicAssert.AreEqual(statusTimestamp.ToIso8601(),   statusNotificationRequests.First().Timestamp.ToIso8601());

            }

        }

        #endregion

        #region SendMeterValues_Test()

        /// <summary>
        /// A test for sending meter values to the CSMS.
        /// </summary>
        [Test]
        public async Task SendMeterValues_Test()
        {

            if (testCSMS01                                    is not null &&
                testBackendWebSockets01                       is not null &&
                csms1WebSocketJSONMessagesReceived            is not null &&
                csms1WebSocketJSONMessageResponsesSent        is not null &&

                chargingStation1                              is not null &&
                chargingStation1WebSocketJSONMessagesReceived is not null &&
                chargingStation1WebSocketJSONMessagesSent     is not null &&

                chargingStation2                              is not null &&
                chargingStation2WebSocketJSONMessagesReceived is not null &&
                chargingStation2WebSocketJSONMessagesSent     is not null &&

                chargingStation3                              is not null &&
                chargingStation3WebSocketJSONMessagesReceived is not null &&
                chargingStation3WebSocketJSONMessagesSent     is not null)

            {

                var meterValuesRequests= new ConcurrentList<CS.MeterValuesRequest>();

                testCSMS01.OnMeterValuesRequestReceived += (timestamp, sender, connection, meterValuesRequest) => {
                    meterValuesRequests.TryAdd(meterValuesRequest);
                    return Task.CompletedTask;
                };

                var evseId       = EVSE_Id.Parse(1);
                var meterValues  = new[] {
                                       new MeterValue(
                                           Timestamp.Now - TimeSpan.FromMinutes(5),
                                           new[] {
                                               new SampledValue(
                                                   Value:                 1.01M,
                                                   Context:               ReadingContext.TransactionBegin,
                                                   Measurand:             Measurand.Current_Import,
                                                   Phase:                 Phases.L1,
                                                   MeasurementLocation:   MeasurementLocation.Outlet,
                                                   SignedMeterValue:      new SignedMeterValue(
                                                                              SignedMeterData:   "1.01",
                                                                              SigningMethod:     "secp256r1_1.01",
                                                                              EncodingMethod:    "base64_1.01",
                                                                              PublicKey:         "pubkey_1.01",
                                                                              CustomData:        null
                                                                          ),
                                                   UnitOfMeasure:         UnitsOfMeasure.kW(
                                                                              Multiplier:   1,
                                                                              CustomData:   null
                                                                          ),
                                                   CustomData:            null
                                               ),
                                               new SampledValue(
                                                   Value:                 1.02M,
                                                   Context:               ReadingContext.TransactionBegin,
                                                   Measurand:             Measurand.Voltage,
                                                   Phase:                 Phases.L2,
                                                   MeasurementLocation:   MeasurementLocation.Inlet,
                                                   SignedMeterValue:      new SignedMeterValue(
                                                                              SignedMeterData:   "1.02",
                                                                              SigningMethod:     "secp256r1_1.02",
                                                                              EncodingMethod:    "base64_1.02",
                                                                              PublicKey:         "pubkey_1.02",
                                                                              CustomData:        null
                                                                          ),
                                                   UnitOfMeasure:         UnitsOfMeasure.kW(
                                                                              Multiplier:   2,
                                                                              CustomData:   null
                                                                          ),
                                                   CustomData:            null
                                               )
                                           }
                                       ),
                                       new MeterValue(
                                           Timestamp.Now,
                                           new[] {
                                               new SampledValue(
                                                   Value:                 2.01M,
                                                   Context:               ReadingContext.TransactionEnd,
                                                   Measurand:             Measurand.Current_Import_Offered,
                                                   Phase:                 Phases.L3,
                                                   MeasurementLocation:   MeasurementLocation.Cable,
                                                   SignedMeterValue:      new SignedMeterValue(
                                                                              SignedMeterData:   "2.01",
                                                                              SigningMethod:     "secp256r1_2.01",
                                                                              EncodingMethod:    "base64_2.01",
                                                                              PublicKey:         "pubkey_2.01",
                                                                              CustomData:        null
                                                                          ),
                                                   UnitOfMeasure:         UnitsOfMeasure.kW(
                                                                              Multiplier:   3,
                                                                              CustomData:   null
                                                                          ),
                                                   CustomData:            null
                                               ),
                                               new SampledValue(
                                                   Value:                 2.02M,
                                                   Context:               ReadingContext.TransactionEnd,
                                                   Measurand:             Measurand.Frequency,
                                                   Phase:                 Phases.N,
                                                   MeasurementLocation:   MeasurementLocation.EV,
                                                   SignedMeterValue:      new SignedMeterValue(
                                                                              SignedMeterData:   "2.02",
                                                                              SigningMethod:     "secp256r1_2.02",
                                                                              EncodingMethod:    "base64_2.02",
                                                                              PublicKey:         "pubkey_2.02",
                                                                              CustomData:        null
                                                                          ),
                                                   UnitOfMeasure:         UnitsOfMeasure.kW(
                                                                              Multiplier:   4,
                                                                              CustomData:   null
                                                                          ),
                                                   CustomData:            null
                                               )
                                           }
                                       )
                                   };

                var response     = await chargingStation1.SendMeterValues(
                                       EVSEId:        evseId,
                                       MeterValues:   meterValues,
                                       CustomData:    null
                                   );


                var clientCloseMessage = chargingStation1.ClientCloseMessage;

                ClassicAssert.AreEqual (ResultCode.OK,                                                  response.Result.ResultCode);

                ClassicAssert.AreEqual (1,                                                               meterValuesRequests.Count);
                ClassicAssert.AreEqual (evseId,                                                          meterValuesRequests.First().EVSEId);

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

                //ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format);
                //ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format);
                //ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format);
                //ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Phase);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Phase);

                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).MeasurementLocation,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).MeasurementLocation);
                ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).MeasurementLocation,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).MeasurementLocation);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).MeasurementLocation,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).MeasurementLocation);
                ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).MeasurementLocation,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).MeasurementLocation);

                //ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit);
                //ClassicAssert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit);
                //ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit);
                //ClassicAssert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit);


                ClassicAssert.AreEqual(1, chargingStation1WebSocketJSONMessagesReceived.Count);
                ClassicAssert.AreEqual(1, csms1WebSocketJSONMessagesReceived.            Count);
                ClassicAssert.AreEqual(1, csms1WebSocketJSONMessageResponsesSent.        Count);
                ClassicAssert.AreEqual(1, chargingStation1WebSocketJSONMessagesSent.    Count);

            }

        }

        #endregion

        #region NotifyChargingLimit_Test()

        /// <summary>
        /// A test for notifying the CSMS about charging limits.
        /// </summary>
        [Test]
        public async Task NotifyChargingLimit_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyChargingLimitRequests= new ConcurrentList<CS.NotifyChargingLimitRequest>();

                testCSMS01.OnNotifyChargingLimitRequestReceived += (timestamp, sender, connection, notifyChargingLimitRequest) => {
                    notifyChargingLimitRequests.TryAdd(notifyChargingLimitRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyChargingLimit(
                                         ChargingLimit:       new ChargingLimit(
                                                                  ChargingLimitSource:   ChargingLimitSource.SO,
                                                                  IsGridCritical:        true,
                                                                  IsLocalGeneration:     false,
                                                                  CustomData:            null
                                                              ),
                                         ChargingSchedules:   new[] {
                                                                  new ChargingSchedule(
                                                                      Id:                        ChargingSchedule_Id.NewRandom(),
                                                                      ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                      ChargingSchedulePeriods:   new[] {
                                                                                                     new ChargingSchedulePeriod(
                                                                                                         StartPeriod:      TimeSpan.Zero,
                                                                                                         Limit:            ChargingRateValue.Parse(
                                                                                                                               20,
                                                                                                                               ChargingRateUnits.Watts
                                                                                                                           ),
                                                                                                         NumberOfPhases:   3,
                                                                                                         PhaseToUse:       PhasesToUse.Three,
                                                                                                         CustomData:       null
                                                                                                     )
                                                                                                 },
                                                                      StartSchedule:             Timestamp.Now,
                                                                      Duration:                  TimeSpan.FromMinutes(30),
                                                                      MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                      SalesTariff:               new SalesTariff(
                                                                                                     Id:                   SalesTariff_Id.NewRandom,
                                                                                                     SalesTariffEntries:   new[] {
                                                                                                                               new SalesTariffEntry(
                                                                                                                                   RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                               Start:        TimeSpan.Zero,
                                                                                                                                                               Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                               CustomData:   null
                                                                                                                                                           ),
                                                                                                                                   EPriceLevel:            1,
                                                                                                                                   ConsumptionCosts:       new[] {
                                                                                                                                                               new ConsumptionCost(
                                                                                                                                                                   StartValue:   1,
                                                                                                                                                                   Costs:        new[] {
                                                                                                                                                                                     new Cost(
                                                                                                                                                                                         CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                         Amount:             200,
                                                                                                                                                                                         AmountMultiplier:   23,
                                                                                                                                                                                         CustomData:         null
                                                                                                                                                                                     )
                                                                                                                                                                                 },
                                                                                                                                                                   CustomData:   null
                                                                                                                                                               )
                                                                                                                                                           },
                                                                                                                                   CustomData:             null
                                                                                                                               )
                                                                                                                           },
                                                                                                     Description:          "Green Charging ++",
                                                                                                     NumEPriceLevels:      1,
                                                                                                     CustomData:           null
                                                                                                 ),
                                                                      CustomData:                null
                                                                  )
                                                              },
                                         EVSEId:              EVSE_Id.Parse("1"),
                                         CustomData:          null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyChargingLimitRequests.Count);

            }

        }

        #endregion

        #region SendClearedChargingLimit_Test()

        /// <summary>
        /// A test for indicating a cleared charging limit to the CSMS.
        /// </summary>
        [Test]
        public async Task SendClearedChargingLimit_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests= new ConcurrentList<CS.ClearedChargingLimitRequest>();

                testCSMS01.OnClearedChargingLimitRequestReceived += (timestamp, sender, connection, transactionEventRequest) => {
                    transactionEventRequests.TryAdd(transactionEventRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendClearedChargingLimit(
                                   ChargingLimitSource:   ChargingLimitSource.SO,
                                   EVSEId:                EVSE_Id.Parse("1"),
                                   CustomData:            null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               transactionEventRequests.Count);

            }

        }

        #endregion

        #region ReportChargingProfiles_Test()

        /// <summary>
        /// A test for reporting charging profiles to the CSMS.
        /// </summary>
        [Test]
        public async Task ReportChargingProfiles_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests= new ConcurrentList<CS.ReportChargingProfilesRequest>();

                testCSMS01.OnReportChargingProfilesRequestReceived += (timestamp, sender, connection, transactionEventRequest) => {
                    transactionEventRequests.TryAdd(transactionEventRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.ReportChargingProfiles(
                                   ReportChargingProfilesRequestId:   1,
                                   ChargingLimitSource:               ChargingLimitSource.SO,
                                   EVSEId:                            EVSE_Id.Parse("1"),
                                   ChargingProfiles:                  new[] {
                                                                          new ChargingProfile(
                                                                              ChargingProfileId:        ChargingProfile_Id.NewRandom,
                                                                              StackLevel:               1,
                                                                              ChargingProfilePurpose:   ChargingProfilePurpose.TxDefaultProfile,
                                                                              ChargingProfileKind:      ChargingProfileKinds.   Absolute,
                                                                              ChargingSchedules:        new[] {
                                                                                                            new ChargingSchedule(
                                                                                                                Id:                        ChargingSchedule_Id.NewRandom(),
                                                                                                                ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                                                                ChargingSchedulePeriods:   new[] {
                                                                                                                                               new ChargingSchedulePeriod(
                                                                                                                                                   StartPeriod:      TimeSpan.Zero,
                                                                                                                                                   Limit:            ChargingRateValue.Parse(
                                                                                                                                                                         20,
                                                                                                                                                                         ChargingRateUnits.Watts
                                                                                                                                                                     ),
                                                                                                                                                   NumberOfPhases:   3,
                                                                                                                                                   PhaseToUse:       PhasesToUse.Three,
                                                                                                                                                   CustomData:       null
                                                                                                                                               )
                                                                                                                                           },
                                                                                                                StartSchedule:             Timestamp.Now,
                                                                                                                Duration:                  TimeSpan.FromMinutes(30),
                                                                                                                MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                                                                SalesTariff:               new SalesTariff(
                                                                                                                                               Id:                   SalesTariff_Id.NewRandom,
                                                                                                                                               SalesTariffEntries:   new[] {
                                                                                                                                                                         new SalesTariffEntry(
                                                                                                                                                                             RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                                                                         Start:        TimeSpan.Zero,
                                                                                                                                                                                                         Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                                                                         CustomData:   null
                                                                                                                                                                                                     ),
                                                                                                                                                                             EPriceLevel:            1,
                                                                                                                                                                             ConsumptionCosts:       new[] {
                                                                                                                                                                                                         new ConsumptionCost(
                                                                                                                                                                                                             StartValue:   1,
                                                                                                                                                                                                             Costs:        new[] {
                                                                                                                                                                                                                               new Cost(
                                                                                                                                                                                                                                   CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                                                                   Amount:             200,
                                                                                                                                                                                                                                   AmountMultiplier:   23,
                                                                                                                                                                                                                                   CustomData:         null
                                                                                                                                                                                                                               )
                                                                                                                                                                                                                           },
                                                                                                                                                                                                             CustomData:   null
                                                                                                                                                                                                         )
                                                                                                                                                                                                     },
                                                                                                                                                                             CustomData:             null
                                                                                                                                                                         )
                                                                                                                                                                     },
                                                                                                                                               Description:          "Green Charging ++",
                                                                                                                                               NumEPriceLevels:      1,
                                                                                                                                               CustomData:           null
                                                                                                                                           ),
                                                                                                                CustomData:                null
                                                                                                            )
                                                                                                        },
                                                                              CustomData:               null
                                                                          )
                                                                      },
                                   ToBeContinued:                     false,
                                   CustomData:                        null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               transactionEventRequests.Count);

            }

        }

        #endregion

        #region NotifyEVChargingSchedule_Test()

        /// <summary>
        /// A test for reporting charging profiles to the CSMS.
        /// </summary>
        [Test]
        public async Task NotifyEVChargingSchedule_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyEVChargingScheduleRequests= new ConcurrentList<CS.NotifyEVChargingScheduleRequest>();

                testCSMS01.OnNotifyEVChargingScheduleRequestReceived += (timestamp, sender, connection, notifyEVChargingScheduleRequest) => {
                    notifyEVChargingScheduleRequests.TryAdd(notifyEVChargingScheduleRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEVChargingSchedule(

                                   TimeBase:                            Timestamp.Now,
                                   EVSEId:                              EVSE_Id.Parse("1"),
                                   ChargingSchedule:                    new ChargingSchedule(
                                                                            Id:                        ChargingSchedule_Id.NewRandom(),
                                                                            ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                            ChargingSchedulePeriods:   new[] {
                                                                                                           new ChargingSchedulePeriod(
                                                                                                               StartPeriod:      TimeSpan.Zero,
                                                                                                               Limit:            ChargingRateValue.Parse(
                                                                                                                                     20,
                                                                                                                                     ChargingRateUnits.Watts
                                                                                                                                 ),
                                                                                                               NumberOfPhases:   3,
                                                                                                               PhaseToUse:       PhasesToUse.Three,
                                                                                                               CustomData:       null
                                                                                                           )
                                                                                                       },
                                                                            StartSchedule:             Timestamp.Now,
                                                                            Duration:                  TimeSpan.FromMinutes(30),
                                                                            MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                            SalesTariff:               new SalesTariff(
                                                                                                           Id:                   SalesTariff_Id.NewRandom,
                                                                                                           SalesTariffEntries:   new[] {
                                                                                                                                     new SalesTariffEntry(
                                                                                                                                         RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                                     Start:        TimeSpan.Zero,
                                                                                                                                                                     Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                                     CustomData:   null
                                                                                                                                                                 ),
                                                                                                                                         EPriceLevel:            1,
                                                                                                                                         ConsumptionCosts:       new[] {
                                                                                                                                                                     new ConsumptionCost(
                                                                                                                                                                         StartValue:   1,
                                                                                                                                                                         Costs:        new[] {
                                                                                                                                                                                           new Cost(
                                                                                                                                                                                               CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                               Amount:             200,
                                                                                                                                                                                               AmountMultiplier:   23,
                                                                                                                                                                                               CustomData:         null
                                                                                                                                                                                           )
                                                                                                                                                                                       },
                                                                                                                                                                         CustomData:   null
                                                                                                                                                                     )
                                                                                                                                                                 },
                                                                                                                                         CustomData:             null
                                                                                                                                     )
                                                                                                                                 },
                                                                                                           Description:          "Green Charging ++",
                                                                                                           NumEPriceLevels:      1,
                                                                                                           CustomData:           null
                                                                                                       ),
                                                                            CustomData:                null
                                                                        ),
                                   SelectedScheduleTupleId:             1,
                                   PowerToleranceAcceptance:            true,

                                   CustomData:                          null

                               );


                ClassicAssert.AreEqual(ResultCode.OK,            response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                        notifyEVChargingScheduleRequests.Count);

            }

        }

        #endregion

        #region NotifyPriorityCharging_Test()

        /// <summary>
        /// A test for notifying the CMS about priority charging at a charging station.
        /// </summary>
        [Test]
        public async Task NotifyPriorityCharging_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyPriorityChargingRequests= new ConcurrentList<CS.NotifyPriorityChargingRequest>();

                testCSMS01.OnNotifyPriorityChargingRequestReceived += (timestamp, sender, connection, notifyPriorityChargingRequest) => {
                    notifyPriorityChargingRequests.TryAdd(notifyPriorityChargingRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyPriorityCharging(
                                   TransactionId:                     Transaction_Id.Parse("1234"),
                                   Activated:                         true,
                                   CustomData:                        null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyPriorityChargingRequests.Count);

            }

        }

        #endregion

        #region NotifySettlement_Test()

        /// <summary>
        /// A test for notifying the CSMS about payment settlements at a charging station.
        /// </summary>
        [Test]
        public async Task NotifySettlement_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyPriorityChargingRequests= new ConcurrentList<CS.NotifySettlementRequest>();

                testCSMS01.OnNotifySettlementRequestReceived += (timestamp, sender, connection, notifyPriorityChargingRequest) => {
                    notifyPriorityChargingRequests.TryAdd(notifyPriorityChargingRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifySettlement(

                                   PaymentReference:      PaymentReference.Parse("pref_123"),
                                   PaymentStatus:         PaymentStatus.Settled,
                                   SettlementAmount:      23.5m,
                                   SettlementTimestamp:   Timestamp.Now,

                                   TransactionId:         Transaction_Id.Parse("1234"),
                                   StatusInfo:            "status...infoo!",
                                   ReceiptId:             ReceiptId.Parse("rid123"),
                                   ReceiptURL:            URL.Parse("https://example.org/payments/123"),
                                   InvoiceNumber:         InvoiceNumber.Parse("inum123"),

                                   CustomData:            null

                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyPriorityChargingRequests.Count);

            }

        }

        #endregion

        #region PullDynamicScheduleUpdate_Test()

        /// <summary>
        /// A test for pulling dynamic schedule updates from the CSMS.
        /// </summary>
        [Test]
        public async Task PullDynamicScheduleUpdate_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var pullDynamicScheduleUpdateRequests= new ConcurrentList<CS.PullDynamicScheduleUpdateRequest>();

                testCSMS01.OnPullDynamicScheduleUpdateRequestReceived += (timestamp, sender, connection, pullDynamicScheduleUpdateRequest) => {
                    pullDynamicScheduleUpdateRequests.TryAdd(pullDynamicScheduleUpdateRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.PullDynamicScheduleUpdate(
                                   ChargingProfileId:   ChargingProfile_Id.Parse(235),
                                   CustomData:          null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,                                            response.Result.ResultCode);

                //ToDo: Find a way to set the correct data type of the ChargingRateUnits!
                ClassicAssert.AreEqual(ChargingRateValue.Parse( 1, ChargingRateUnits.Unknown),   response.Limit);
                ClassicAssert.AreEqual(ChargingRateValue.Parse( 2, ChargingRateUnits.Unknown),   response.Limit_L2);
                ClassicAssert.AreEqual(ChargingRateValue.Parse( 3, ChargingRateUnits.Unknown),   response.Limit_L3);

                ClassicAssert.AreEqual(ChargingRateValue.Parse(-4, ChargingRateUnits.Unknown),   response.DischargeLimit);
                ClassicAssert.AreEqual(ChargingRateValue.Parse(-5, ChargingRateUnits.Unknown),   response.DischargeLimit_L2);
                ClassicAssert.AreEqual(ChargingRateValue.Parse(-6, ChargingRateUnits.Unknown),   response.DischargeLimit_L3);

                ClassicAssert.AreEqual(ChargingRateValue.Parse( 7, ChargingRateUnits.Unknown),   response.Setpoint);
                ClassicAssert.AreEqual(ChargingRateValue.Parse( 8, ChargingRateUnits.Unknown),   response.Setpoint_L2);
                ClassicAssert.AreEqual(ChargingRateValue.Parse( 9, ChargingRateUnits.Unknown),   response.Setpoint_L3);

                ClassicAssert.AreEqual(ChargingRateValue.Parse(10, ChargingRateUnits.Unknown),   response.SetpointReactive);
                ClassicAssert.AreEqual(ChargingRateValue.Parse(11, ChargingRateUnits.Unknown),   response.SetpointReactive_L2);
                ClassicAssert.AreEqual(ChargingRateValue.Parse(12, ChargingRateUnits.Unknown),   response.SetpointReactive_L3);


                ClassicAssert.AreEqual(1,                                                        pullDynamicScheduleUpdateRequests.Count);

            }

        }

        #endregion


        #region NotifyDisplayMessages_Test()

        /// <summary>
        /// A test for notifying the CSMS about display messages.
        /// </summary>
        [Test]
        public async Task NotifyDisplayMessages_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyDisplayMessagesRequests= new ConcurrentList<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OnNotifyDisplayMessagesRequestReceived += (timestamp, sender, connection, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyDisplayMessages(
                                         NotifyDisplayMessagesRequestId:   1,
                                         MessageInfos:                     new[] {
                                                                               new MessageInfo(
                                                                                   Id:               DisplayMessage_Id.NewRandom,
                                                                                   Priority:         MessagePriority.InFront,
                                                                                   Message:          new MessageContent(
                                                                                                         Content:      "Hello World!",
                                                                                                         Format:       MessageFormat.UTF8,
                                                                                                         Language:     Language_Id.Parse("EN"),
                                                                                                         CustomData:   null
                                                                                                     ),
                                                                                   State:            MessageState.Charging,
                                                                                   StartTimestamp:   Timestamp.Now,
                                                                                   EndTimestamp:     Timestamp.Now + TimeSpan.FromHours(3),
                                                                                   TransactionId:    Transaction_Id.NewRandom,
                                                                                   Display:          new Component(
                                                                                                         Name:         "Big Displays",
                                                                                                         Instance:     "Big Display #1",
                                                                                                         EVSE:         new EVSE(
                                                                                                                           Id:            EVSE_Id.     Parse(1),
                                                                                                                           ConnectorId:   Connector_Id.Parse(1),
                                                                                                                           CustomData:    null
                                                                                                                       ),
                                                                                                         CustomData:   null
                                                                                                     ),
                                                                                   CustomData:       null
                                                                               )
                                                                           },
                                         CustomData:                       null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyDisplayMessagesRequests.Count);

            }

        }

        #endregion

        #region NotifyCustomerInformation_Test()

        /// <summary>
        /// A test for notifying the CSMS about customer information.
        /// </summary>
        [Test]
        public async Task NotifyCustomerInformation_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var notifyCustomerInformationRequests= new ConcurrentList<CS.NotifyCustomerInformationRequest>();

                testCSMS01.OnNotifyCustomerInformationRequestReceived += (timestamp, sender, connection, notifyCustomerInformationRequest) => {
                    notifyCustomerInformationRequests.TryAdd(notifyCustomerInformationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyCustomerInformation(
                                   NotifyCustomerInformationRequestId:   1,
                                   Data:                                 "Hello World!",
                                   SequenceNumber:                       1,
                                   GeneratedAt:                          Timestamp.Now,
                                   ToBeContinued:                        false,
                                   CustomData:                           null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyCustomerInformationRequests.Count);

            }

        }

        #endregion


    }

}
