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
using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.tests
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class ChargingStationMessagesTests : AChargingStationTests
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

                var bootNotificationRequests = new List<CS.BootNotificationRequest>();

                testCSMS01.OnBootNotificationRequest += async (timestamp, sender, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                };

                var reason     = BootReason.PowerUp;
                var response1  = await chargingStation1.SendBootNotification(reason);

                ClassicAssert.AreEqual (ResultCodes.OK,                         response1.Result.ResultCode);
                ClassicAssert.AreEqual (RegistrationStatus.Accepted,            response1.Status);

                ClassicAssert.AreEqual (1,                                      bootNotificationRequests.Count);
                ClassicAssert.AreEqual (chargingStation1.ChargeBoxId,           bootNotificationRequests.First().ChargeBoxId);
                ClassicAssert.AreEqual (reason,                                 bootNotificationRequests.First().Reason);

                var chargingStation = bootNotificationRequests.First().ChargingStation;

                ClassicAssert.IsNotNull(chargingStation);
                if (chargingStation is not null)
                {

                    ClassicAssert.AreEqual(chargingStation1.Model,              chargingStation.Model);
                    ClassicAssert.AreEqual(chargingStation1.VendorName,         chargingStation.VendorName);
                    ClassicAssert.AreEqual(chargingStation1.SerialNumber,       chargingStation.SerialNumber);
                    ClassicAssert.AreEqual(chargingStation1.FirmwareVersion,    chargingStation.FirmwareVersion);

                    var modem = chargingStation.Modem;

                    ClassicAssert.IsNotNull(modem);
                    if (modem is not null)
                    {
                        ClassicAssert.AreEqual(chargingStation1.Modem!.ICCID,   modem.ICCID);
                        ClassicAssert.AreEqual(chargingStation1.Modem!.IMSI,    modem.IMSI);
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

                var firmwareStatusNotifications = new List<CS.FirmwareStatusNotificationRequest>();

                testCSMS01.OnFirmwareStatusNotificationRequest += async (timestamp, sender, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                };

                var status     = FirmwareStatus.Installed;

                var response1  = await chargingStation1.SendFirmwareStatusNotification(
                                           Status:       status,
                                           CustomData:   null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              firmwareStatusNotifications.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   firmwareStatusNotifications.First().ChargeBoxId);
                ClassicAssert.AreEqual(status,                         firmwareStatusNotifications.First().Status);

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

                var firmwareStatusNotifications = new List<CS.PublishFirmwareStatusNotificationRequest>();

                testCSMS01.OnPublishFirmwareStatusNotificationRequest += async (timestamp, sender, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                };

                var status     = PublishFirmwareStatus.Published;
                var url1       = URL.Parse("https://example.org/firmware.bin");

                var response1  = await chargingStation1.SendPublishFirmwareStatusNotification(
                                           Status:                                       status,
                                           PublishFirmwareStatusNotificationRequestId:   0,
                                           DownloadLocations:                            new[] { url1 },
                                           CustomData:                                   null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              firmwareStatusNotifications.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   firmwareStatusNotifications.First().ChargeBoxId);
                ClassicAssert.AreEqual(status,                         firmwareStatusNotifications.First().Status);

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

                var heartbeatRequests = new List<CS.HeartbeatRequest>();

                testCSMS01.OnHeartbeatRequest += async (timestamp, sender, heartbeatRequest) => {
                    heartbeatRequests.Add(heartbeatRequest);
                };


                var response1 = await chargingStation1.SendHeartbeat();


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                ClassicAssert.IsTrue  (Timestamp.Now - response1.CurrentTime < TimeSpan.FromSeconds(10));

                ClassicAssert.AreEqual(1,                              heartbeatRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   heartbeatRequests.First().ChargeBoxId);

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

                var notifyEventRequests = new List<CS.NotifyEventRequest>();

                testCSMS01.OnNotifyEventRequest += async (timestamp, sender, notifyEventRequest) => {
                    notifyEventRequests.Add(notifyEventRequest);
                };

                var response1  = await chargingStation1.NotifyEvent(
                                           GeneratedAt:      Timestamp.Now,
                                           SequenceNumber:   1,
                                           EventData:        new[] {
                                                                 new EventData(
                                                                     EventId:                 Event_Id.NewRandom,
                                                                     Timestamp:               Timestamp.Now,
                                                                     Trigger:                 EventTriggers.Alerting,
                                                                     ActualValue:             "ALERTA!",
                                                                     EventNotificationType:   EventNotificationTypes.HardWiredMonitor,
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyEventRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyEventRequests.First().ChargeBoxId);

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

                var securityEventNotificationRequests = new List<CS.SecurityEventNotificationRequest>();

                testCSMS01.OnSecurityEventNotificationRequest += async (timestamp, sender, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.Add(securityEventNotificationRequest);
                };

                var response1  = await chargingStation1.SendSecurityEventNotification(
                                           Type:         SecurityEventType.MemoryExhaustion,
                                           Timestamp:    Timestamp.Now,
                                           TechInfo:     "Too many open TCP sockets!",
                                           CustomData:   null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              securityEventNotificationRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   securityEventNotificationRequests.First().ChargeBoxId);

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

                var notifyReportRequests = new List<CS.NotifyReportRequest>();

                testCSMS01.OnNotifyReportRequest += async (timestamp, sender, notifyReportRequest) => {
                    notifyReportRequests.Add(notifyReportRequest);
                };

                var response1  = await chargingStation1.NotifyReport(
                                           NotifyReportRequestId:   1,
                                           SequenceNumber:          1,
                                           GeneratedAt:             Timestamp.Now,
                                           ReportData:              new[] {
                                                                        new ReportData(
                                                                            Component:                new Component(
                                                                                                           Name:                 "Alert System!",
                                                                                                           Instance:             "Alert System #1",
                                                                                                           EVSE:                 new EVSE(
                                                                                                                                     Id:            EVSE_Id.Parse(1),
                                                                                                                                     ConnectorId:   Connector_Id.Parse(1),
                                                                                                                                     CustomData:    null
                                                                                                                                 ),
                                                                                                           CustomData:           null
                                                                                                       ),
                                                                             Variable:                 new Variable(
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyReportRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyReportRequests.First().ChargeBoxId);

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

                var notifyMonitoringReportRequests = new List<CS.NotifyMonitoringReportRequest>();

                testCSMS01.OnNotifyMonitoringReportRequest += async (timestamp, sender, notifyMonitoringReportRequest) => {
                    notifyMonitoringReportRequests.Add(notifyMonitoringReportRequest);
                };

                var response1  = await chargingStation1.NotifyMonitoringReport(
                                           NotifyMonitoringReportRequestId:   1,
                                           SequenceNumber:                    1,
                                           GeneratedAt:                       Timestamp.Now,
                                           MonitoringData:                    new[] {
                                                                                  new MonitoringData(
                                                                                      Component:              new Component(
                                                                                                                  Name:             "Alert System!",
                                                                                                                  Instance:         "Alert System #1",
                                                                                                                  EVSE:             new EVSE(
                                                                                                                                        Id:            EVSE_Id.Parse(1),
                                                                                                                                        ConnectorId:   Connector_Id.Parse(1),
                                                                                                                                        CustomData:    null
                                                                                                                                    ),
                                                                                                                  CustomData:       null
                                                                                                              ),
                                                                                      Variable:               new Variable(
                                                                                                                  Name:             "Temperature Sensors",
                                                                                                                  Instance:         "Temperature Sensor #1",
                                                                                                                  CustomData:       null
                                                                                                              ),
                                                                                      VariableMonitorings:   new[] {
                                                                                                                 new VariableMonitoring(
                                                                                                                     Id:            VariableMonitoring_Id.NewRandom,
                                                                                                                     Transaction:   true,
                                                                                                                     Value:         1.01M,
                                                                                                                     Type:          MonitorTypes.Periodic,
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyMonitoringReportRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyMonitoringReportRequests.First().ChargeBoxId);

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

                var securityEventNotificationRequests = new List<CS.LogStatusNotificationRequest>();

                testCSMS01.OnLogStatusNotificationRequest += async (timestamp, sender, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.Add(securityEventNotificationRequest);
                };

                var response1  = await chargingStation1.SendLogStatusNotification(
                                            Status:         UploadLogStatus.Uploaded,
                                            LogRequestId:   1,
                                            CustomData:     null
                                        );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              securityEventNotificationRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   securityEventNotificationRequests.First().ChargeBoxId);

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

                var dataTransferRequests = new List<CS.DataTransferRequest>();

                testCSMS01.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
                var messageId  = RandomExtensions.RandomString(10);
                var data       = RandomExtensions.RandomString(40);

                var response1  = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                ClassicAssert.AreEqual(data.Reverse(),                 response1.Data?.ToString());

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

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

                var dataTransferRequests = new List<CS.DataTransferRequest>();

                testCSMS01.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
                var messageId  = RandomExtensions.RandomString(10);
                var data       = new JObject(
                                     new JProperty(
                                         "key",
                                         RandomExtensions.RandomString(40)
                                     )
                                 );

                var response1  = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                ClassicAssert.AreEqual(JTokenType.Object,              response1.Data?.Type);
                ClassicAssert.AreEqual(data["key"]?.Value<String>(),   response1.Data?["key"]?.Value<String>()?.Reverse());

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
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

                var dataTransferRequests = new List<CS.DataTransferRequest>();

                testCSMS01.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
                var messageId  = RandomExtensions.RandomString(10);
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response1  = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                ClassicAssert.AreEqual(JTokenType.Array,               response1.Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),       response1.Data?[0]?.Value<String>()?.Reverse());

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(JTokenType.Array,               dataTransferRequests.First().Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),       dataTransferRequests.First().Data?[0]?.Value<String>());

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

                var notifyReportRequests = new List<CS.SignCertificateRequest>();

                testCSMS01.OnSignCertificateRequest += async (timestamp, sender, notifyReportRequest) => {
                    notifyReportRequests.Add(notifyReportRequest);
                };

                var response1  = await chargingStation1.SendCertificateSigningRequest(
                                           CSR:               "0x1234",
                                           CertificateType:   CertificateSigningUse.ChargingStationCertificate,
                                           CustomData:        null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyReportRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyReportRequests.First().ChargeBoxId);

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

                var notifyReportRequests = new List<CS.Get15118EVCertificateRequest>();

                testCSMS01.OnGet15118EVCertificateRequest += async (timestamp, sender, notifyReportRequest) => {
                    notifyReportRequests.Add(notifyReportRequest);
                };

                var response1  = await chargingStation1.Get15118EVCertificate(
                                           ISO15118SchemaVersion:   ISO15118SchemaVersion.Parse("15118-20:BastelBrothers"),
                                           CertificateAction:       CertificateAction.Install,
                                           EXIRequest:              EXIData.Parse("0x1234"),
                                           CustomData:              null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyReportRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyReportRequests.First().ChargeBoxId);

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

                var notifyReportRequests = new List<CS.GetCertificateStatusRequest>();

                testCSMS01.OnGetCertificateStatusRequest += async (timestamp, sender, notifyReportRequest) => {
                    notifyReportRequests.Add(notifyReportRequest);
                };

                var response1  = await chargingStation1.GetCertificateStatus(
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyReportRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyReportRequests.First().ChargeBoxId);

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

                var securityEventNotificationRequests = new List<CS.ReservationStatusUpdateRequest>();

                testCSMS01.OnReservationStatusUpdateRequest += async (timestamp, sender, securityEventNotificationRequest) => {
                    securityEventNotificationRequests.Add(securityEventNotificationRequest);
                };

                var response1  = await chargingStation1.SendReservationStatusUpdate(
                                           ReservationId:             Reservation_Id.NewRandom,
                                           ReservationUpdateStatus:   ReservationUpdateStatus.Expired,
                                           CustomData:                null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              securityEventNotificationRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   securityEventNotificationRequests.First().ChargeBoxId);

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

                var authorizeRequests = new List<CS.AuthorizeRequest>();

                testCSMS01.OnAuthorizeRequest += async (timestamp, sender, authorizeRequest) => {
                    authorizeRequests.Add(authorizeRequest);
                };

                var idToken   = IdToken.NewRandomRFID();
                var response1 = await chargingStation1.Authorize(idToken);


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                ClassicAssert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTokenInfo.Status);

                ClassicAssert.AreEqual(1,                              authorizeRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   authorizeRequests.First().ChargeBoxId);
                ClassicAssert.AreEqual(idToken,                        authorizeRequests.First().IdToken);

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

                var notifyEVChargingNeedsRequests = new List<CS.NotifyEVChargingNeedsRequest>();

                testCSMS01.OnNotifyEVChargingNeedsRequest += async (timestamp, sender, notifyEVChargingNeedsRequest) => {
                    notifyEVChargingNeedsRequests.Add(notifyEVChargingNeedsRequest);
                };

                var response1  = await chargingStation1.NotifyEVChargingNeeds(
                                           EVSEId:              EVSE_Id.Parse(1),
                                           ChargingNeeds:       new ChargingNeeds(
                                                                    RequestedEnergyTransferMode:   EnergyTransferModes.AC_ThreePhases,
                                                                    DepartureTime:                 Timestamp.Now + TimeSpan.FromHours(3),
                                                                    ACChargingParameters:          new ACChargingParameters(
                                                                                                       EnergyAmount:       WattHour.      Parse( 20),
                                                                                                       EVMinCurrent:       Ampere.        Parse(  6),
                                                                                                       EVMaxCurrent:       Ampere.        Parse( 32),
                                                                                                       EVMaxVoltage:       Volt.          Parse(230),
                                                                                                       CustomData:         null
                                                                                                   ),
                                                                    DCChargingParameters:          new DCChargingParameters(
                                                                                                       EVMaxCurrent:       Ampere.        Parse( 20),
                                                                                                       EVMaxVoltage:       Volt.          Parse(900),
                                                                                                       EnergyAmount:       WattHour.      Parse(300),
                                                                                                       EVMaxPower:         Watt.          Parse( 60),
                                                                                                       StateOfCharge:      PercentageByte.Parse( 23),
                                                                                                       EVEnergyCapacity:   WattHour.      Parse(250),
                                                                                                       FullSoC:            PercentageByte.Parse( 95),
                                                                                                       BulkSoC:            PercentageByte.Parse( 80),
                                                                                                       CustomData:         null
                                                                                                   ),
                                                                    CustomData:                    null
                                                                ),
                                           MaxScheduleTuples:   16,
                                           CustomData:          null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyEVChargingNeedsRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyEVChargingNeedsRequests.First().ChargeBoxId);

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

                var transactionEventRequests = new List<CS.TransactionEventRequest>();

                testCSMS01.OnTransactionEventRequest += async (timestamp, sender, transactionEventRequest) => {
                    transactionEventRequests.Add(transactionEventRequest);
                };

                var evseId          = EVSE_Id.     Parse(1);
                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.     NewRandomRFID();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response1       = await chargingStation1.SendTransactionEvent(

                                          EventType:            TransactionEvents.Started,
                                          Timestamp:            startTimestamp,
                                          TriggerReason:        TriggerReasons.Authorized,
                                          SequenceNumber:       0,
                                          TransactionInfo:      new Transaction(
                                                                    TransactionId:       Transaction_Id.NewRandom,
                                                                    ChargingState:       ChargingStates.Charging,
                                                                    TimeSpentCharging:   TimeSpan.FromSeconds(3),
                                                                    StoppedReason:       null,
                                                                    RemoteStartId:       null,
                                                                    CustomData:          null
                                                                ),

                                          Offline:              null,
                                          NumberOfPhasesUsed:   null,
                                          CableMaxCurrent:      null,
                                          ReservationId:        reservationId,
                                          IdToken:              idToken,
                                          EVSE:                 new EVSE(
                                                                    Id:                  evseId,
                                                                    ConnectorId:         connectorId,
                                                                    CustomData:          null
                                                                ),
                                          MeterValues:          new[] {
                                                                    new MeterValue(
                                                                        SampledValues:   new[] {

                                                                                             new SampledValue(
                                                                                                 Value:              meterStart,
                                                                                                 Context:            ReadingContexts.TransactionBegin,
                                                                                                 Measurand:          Measurands.Energy_Active_Export_Interval,
                                                                                                 Phase:              Phases.L1,
                                                                                                 Location:           MeasurementLocations.Outlet,
                                                                                                 SignedMeterValue:   new SignedMeterValue(
                                                                                                                         SignedMeterData:   meterStart.ToString(),
                                                                                                                         SigningMethod:     "secp256r1",
                                                                                                                         EncodingMethod:    "base64",
                                                                                                                         PublicKey:         "0x1234",
                                                                                                                         CustomData:        null
                                                                                                                     ),
                                                                                                 UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                                                                         Multiplier:   0,
                                                                                                                         CustomData:   null
                                                                                                                     ),
                                                                                                 CustomData:         null
                                                                                             )

                                                                                         },
                                                                        Timestamp:       startTimestamp,
                                                                        CustomData:      null
                                                                    )
                                                                },
                                          CustomData:           null

                                      );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                //Assert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTokenInfo.Status);
                //Assert.IsTrue  (response1.TransactionId.IsNotNullOrEmpty);

                ClassicAssert.AreEqual(1,                              transactionEventRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   transactionEventRequests.First().ChargeBoxId);
                //Assert.AreEqual(connectorId,                    transactionEventRequests.First().ConnectorId);
                //Assert.AreEqual(idToken,                        transactionEventRequests.First().IdTag);
                //Assert.AreEqual(startTimestamp.ToIso8601(),     transactionEventRequests.First().StartTimestamp.ToIso8601());
                //Assert.AreEqual(meterStart,                     transactionEventRequests.First().MeterStart);
                //Assert.AreEqual(reservationId,                  transactionEventRequests.First().ReservationId);

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

                var statusNotificationRequests = new List<CS.StatusNotificationRequest>();

                testCSMS01.OnStatusNotificationRequest += async (timestamp, sender, statusNotificationRequest) => {
                    statusNotificationRequests.Add(statusNotificationRequest);
                };

                var evseId           = EVSE_Id.     Parse(1);
                var connectorId      = Connector_Id.Parse(1);
                var connectorStatus  = ConnectorStatus.Available;
                var statusTimestamp  = Timestamp.Now;

                var response1        = await chargingStation1.SendStatusNotification(
                                           EVSEId:        evseId,
                                           ConnectorId:   connectorId,
                                           Timestamp:     statusTimestamp,
                                           Status:        connectorStatus,
                                           CustomData:    null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              statusNotificationRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   statusNotificationRequests.First().ChargeBoxId);
                ClassicAssert.AreEqual(evseId,                         statusNotificationRequests.First().EVSEId);
                ClassicAssert.AreEqual(connectorId,                    statusNotificationRequests.First().ConnectorId);
                ClassicAssert.AreEqual(connectorStatus,                statusNotificationRequests.First().ConnectorStatus);
                ClassicAssert.AreEqual(statusTimestamp.ToIso8601(),    statusNotificationRequests.First().Timestamp.ToIso8601());

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
                csmsWebSocketTextMessagesReceived                  is not null &&
                csmsWebSocketTextMessageResponsesSent                 is not null &&

                chargingStation1                              is not null &&
                chargingStation1WebSocketTextMessagesReceived is not null &&
                chargingStation1WebSocketTextMessagesSent is not null &&

                chargingStation2                              is not null &&
                chargingStation2WebSocketTextMessagesReceived is not null &&
                chargingStation2WebSocketTextMessagesSent is not null &&

                chargingStation3                              is not null &&
                chargingStation3WebSocketTextMessagesReceived is not null &&
                chargingStation3WebSocketTextMessagesSent is not null)

            {

                var meterValuesRequests = new List<CS.MeterValuesRequest>();

                testCSMS01.OnMeterValuesRequest += async (timestamp, sender, meterValuesRequest) => {
                    meterValuesRequests.Add(meterValuesRequest);
                };

                var evseId       = EVSE_Id.Parse(1);
                var meterValues  = new[] {
                                       new MeterValue(
                                           Timestamp.Now - TimeSpan.FromMinutes(5),
                                           new[] {
                                               new SampledValue(
                                                   Value:              1.01M,
                                                   Context:            ReadingContexts.TransactionBegin,
                                                   Measurand:          Measurands.Current_Import,
                                                   Phase:              Phases.L1,
                                                   Location:           MeasurementLocations.Outlet,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "1.01",
                                                                           SigningMethod:     "secp256r1_1.01",
                                                                           EncodingMethod:    "base64_1.01",
                                                                           PublicKey:         "pubkey_1.01",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   1,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               ),
                                               new SampledValue(
                                                   Value:              1.02M,
                                                   Context:            ReadingContexts.TransactionBegin,
                                                   Measurand:          Measurands.Voltage,
                                                   Phase:              Phases.L2,
                                                   Location:           MeasurementLocations.Inlet,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "1.02",
                                                                           SigningMethod:     "secp256r1_1.02",
                                                                           EncodingMethod:    "base64_1.02",
                                                                           PublicKey:         "pubkey_1.02",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   2,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               )
                                           }
                                       ),
                                       new MeterValue(
                                           Timestamp.Now,
                                           new[] {
                                               new SampledValue(
                                                   Value:              2.01M,
                                                   Context:            ReadingContexts.TransactionEnd,
                                                   Measurand:          Measurands.Current_Offered,
                                                   Phase:              Phases.L3,
                                                   Location:           MeasurementLocations.Cable,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "2.01",
                                                                           SigningMethod:     "secp256r1_2.01",
                                                                           EncodingMethod:    "base64_2.01",
                                                                           PublicKey:         "pubkey_2.01",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   3,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               ),
                                               new SampledValue(
                                                   Value:              2.02M,
                                                   Context:            ReadingContexts.TransactionEnd,
                                                   Measurand:          Measurands.Frequency,
                                                   Phase:              Phases.N,
                                                   Location:           MeasurementLocations.EV,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "2.02",
                                                                           SigningMethod:     "secp256r1_2.02",
                                                                           EncodingMethod:    "base64_2.02",
                                                                           PublicKey:         "pubkey_2.02",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   4,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               )
                                           }
                                       )
                                   };

                var response1    = await chargingStation1.SendMeterValues(
                                       EVSEId:        evseId,
                                       MeterValues:   meterValues,
                                       CustomData:    null
                                   );


                var clientCloseMessage = chargingStation1.ClientCloseMessage;

                ClassicAssert.AreEqual (ResultCodes.OK,                                                  response1.Result.ResultCode);

                ClassicAssert.AreEqual (1,                                                               meterValuesRequests.Count);
                ClassicAssert.AreEqual (chargingStation1.ChargeBoxId,                                    meterValuesRequests.First().ChargeBoxId);
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

                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format);
                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format);

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

                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit);
                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit);


                ClassicAssert.AreEqual(1, chargingStation1WebSocketTextMessagesReceived.Count);
                ClassicAssert.AreEqual(1, csmsWebSocketTextMessagesReceived.                 Count);
                ClassicAssert.AreEqual(1, csmsWebSocketTextMessageResponsesSent.                Count);
                ClassicAssert.AreEqual(1, chargingStation1WebSocketTextMessagesSent.Count);

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

                var notifyChargingLimitRequests = new List<CS.NotifyChargingLimitRequest>();

                testCSMS01.OnNotifyChargingLimitRequest += async (timestamp, sender, notifyChargingLimitRequest) => {
                    notifyChargingLimitRequests.Add(notifyChargingLimitRequest);
                };

                var response1  = await chargingStation1.NotifyChargingLimit(
                                           ChargingLimit:       new ChargingLimit(
                                                                    ChargingLimitSource:   ChargingLimitSources.SO,
                                                                    IsGridCritical:        true,
                                                                    CustomData:            null
                                                                ),
                                           ChargingSchedules:   new[] {
                                                                    new ChargingSchedule(
                                                                        Id:                        ChargingSchedule_Id.NewRandom(),
                                                                        ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                        ChargingSchedulePeriods:   new[] {
                                                                                                       new ChargingSchedulePeriod(
                                                                                                           StartPeriod:      TimeSpan.Zero,
                                                                                                           Limit:            20,
                                                                                                           NumberOfPhases:   3,
                                                                                                           PhaseToUse:       PhasesToUse.Three,
                                                                                                           CustomData:       null
                                                                                                       )
                                                                                                   },
                                                                        StartSchedule:             Timestamp.Now,
                                                                        Duration:                  TimeSpan.FromMinutes(30),
                                                                        MinChargingRate:           6,
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyChargingLimitRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyChargingLimitRequests.First().ChargeBoxId);

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

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests = new List<CS.ClearedChargingLimitRequest>();

                testCSMS01.OnClearedChargingLimitRequest += async (timestamp, sender, transactionEventRequest) => {
                    transactionEventRequests.Add(transactionEventRequest);
                };

                var response  = await chargingStation1.SendClearedChargingLimit(
                                    ChargingLimitSource:   ChargingLimitSources.SO,
                                    EVSEId:                EVSE_Id.Parse("1"),
                                    CustomData:            null
                                );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              transactionEventRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   transactionEventRequests.First().ChargeBoxId);

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

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests = new List<CS.ReportChargingProfilesRequest>();

                testCSMS01.OnReportChargingProfilesRequest += async (timestamp, sender, transactionEventRequest) => {
                    transactionEventRequests.Add(transactionEventRequest);
                };

                var response  = await chargingStation1.ReportChargingProfiles(
                                    ReportChargingProfilesRequestId:   1,
                                    ChargingLimitSource:               ChargingLimitSources.SO,
                                    EVSEId:                            EVSE_Id.Parse("1"),
                                    ChargingProfiles:                  new[] {
                                                                           new ChargingProfile(
                                                                               ChargingProfileId:        ChargingProfile_Id.NewRandom,
                                                                               StackLevel:               1,
                                                                               ChargingProfilePurpose:   ChargingProfilePurposes.TxDefaultProfile,
                                                                               ChargingProfileKind:      ChargingProfileKinds.   Absolute,
                                                                               ChargingSchedules:        new[] {
                                                                                                             new ChargingSchedule(
                                                                                                                 Id:                        ChargingSchedule_Id.NewRandom(),
                                                                                                                 ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                                                                 ChargingSchedulePeriods:   new[] {
                                                                                                                                                new ChargingSchedulePeriod(
                                                                                                                                                    StartPeriod:      TimeSpan.Zero,
                                                                                                                                                    Limit:            20,
                                                                                                                                                    NumberOfPhases:   3,
                                                                                                                                                    PhaseToUse:       PhasesToUse.Three,
                                                                                                                                                    CustomData:       null
                                                                                                                                                )
                                                                                                                                            },
                                                                                                                 StartSchedule:             Timestamp.Now,
                                                                                                                 Duration:                  TimeSpan.FromMinutes(30),
                                                                                                                 MinChargingRate:           6,
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              transactionEventRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   transactionEventRequests.First().ChargeBoxId);

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

                var notifyEVChargingScheduleRequests = new List<CS.NotifyEVChargingScheduleRequest>();

                testCSMS01.OnNotifyEVChargingScheduleRequest += async (timestamp, sender, notifyEVChargingScheduleRequest) => {
                    notifyEVChargingScheduleRequests.Add(notifyEVChargingScheduleRequest);
                };

                var response  = await chargingStation1.NotifyEVChargingSchedule(

                                    NotifyEVChargingScheduleRequestId:   1,
                                    TimeBase:                            Timestamp.Now,
                                    EVSEId:                              EVSE_Id.Parse("1"),
                                    ChargingSchedule:                    new ChargingSchedule(
                                                                             Id:                        ChargingSchedule_Id.NewRandom(),
                                                                             ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                             ChargingSchedulePeriods:   new[] {
                                                                                                            new ChargingSchedulePeriod(
                                                                                                                StartPeriod:      TimeSpan.Zero,
                                                                                                                Limit:            20M,
                                                                                                                NumberOfPhases:   3,
                                                                                                                PhaseToUse:       PhasesToUse.Three,
                                                                                                                CustomData:       null
                                                                                                            )
                                                                                                        },
                                                                             StartSchedule:             Timestamp.Now,
                                                                             Duration:                  TimeSpan.FromMinutes(30),
                                                                             MinChargingRate:           6M,
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

                                    CustomData:                          null

                                );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,         response.Status);

                ClassicAssert.AreEqual(1,                              notifyEVChargingScheduleRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyEVChargingScheduleRequests.First().ChargeBoxId);

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

                var notifyDisplayMessagesRequests = new List<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OnNotifyDisplayMessagesRequest += async (timestamp, sender, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.Add(notifyDisplayMessagesRequest);
                };

                var response1  = await chargingStation1.NotifyDisplayMessages(
                                           NotifyDisplayMessagesRequestId:   1,
                                           MessageInfos:                     new[] {
                                                                                 new MessageInfo(
                                                                                     Id:               DisplayMessage_Id.NewRandom,
                                                                                     Priority:         MessagePriorities.InFront,
                                                                                     Message:          new MessageContent(
                                                                                                           Content:      "Hello World!",
                                                                                                           Format:       MessageFormats.UTF8,
                                                                                                           Language:     Language_Id.Parse("EN"),
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                     State:            MessageStates.Charging,
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


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyDisplayMessagesRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyDisplayMessagesRequests.First().ChargeBoxId);

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

                var notifyCustomerInformationRequests = new List<CS.NotifyCustomerInformationRequest>();

                testCSMS01.OnNotifyCustomerInformationRequest += async (timestamp, sender, notifyCustomerInformationRequest) => {
                    notifyCustomerInformationRequests.Add(notifyCustomerInformationRequest);
                };

                var response1  = await chargingStation1.NotifyCustomerInformation(
                                           NotifyCustomerInformationRequestId:   1,
                                           Data:                                 "Hello World!",
                                           SequenceNumber:                       1,
                                           GeneratedAt:                          Timestamp.Now,
                                           ToBeContinued:                        false,
                                           CustomData:                           null
                                       );


                ClassicAssert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                ClassicAssert.AreEqual(1,                              notifyCustomerInformationRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.ChargeBoxId,   notifyCustomerInformationRequests.First().ChargeBoxId);

            }

        }

        #endregion


    }

}
