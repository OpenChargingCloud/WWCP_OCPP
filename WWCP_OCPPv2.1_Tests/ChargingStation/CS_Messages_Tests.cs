/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP;

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

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null &&

                chargingStation2                                     is not null &&
                chargingStation2WebSocketJSONRequestsSent            is not null &&
                chargingStation2WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation2WebSocketJSONResponsesReceived       is not null &&
                chargingStation2WebSocketJSONRequestsReceived        is not null &&
                chargingStation2WebSocketJSONResponsesSent           is not null &&
                chargingStation2WebSocketJSONResponseErrorsReceived  is not null &&

                chargingStation3                                     is not null &&
                chargingStation3WebSocketJSONRequestsSent            is not null &&
                chargingStation3WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation3WebSocketJSONResponsesReceived       is not null &&
                chargingStation3WebSocketJSONRequestsReceived        is not null &&
                chargingStation3WebSocketJSONResponsesSent           is not null &&
                chargingStation3WebSocketJSONResponseErrorsReceived  is not null)
            {

                Assert.Multiple(() => {

                    Assert.That(chargingStation1.VendorName,  Is.EqualTo("GraphDefined OEM #1"));
                    Assert.That(chargingStation2.VendorName,  Is.EqualTo("GraphDefined OEM #2"));
                    Assert.That(chargingStation3.VendorName,  Is.EqualTo("GraphDefined OEM #3"));

                });

            }

            else
                Assert.Fail($"{nameof(ChargingStation_Init_Test)} preconditions failed!");

        }

        #endregion


        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var bootNotificationRequests = new ConcurrentList<BootNotificationRequest>();

                testCSMS1.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection,  bootNotificationRequest, ct) => {
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

                    Assert.Multiple(() => {
                        Assert.That(chargingStation.Model,                              Is.EqualTo(chargingStation1.Model));
                        Assert.That(chargingStation.VendorName,                         Is.EqualTo(chargingStation1.VendorName));
                        Assert.That(chargingStation.SerialNumber,                       Is.EqualTo(chargingStation1.SerialNumber));
                        Assert.That(chargingStation.FirmwareVersion,                    Is.EqualTo(chargingStation1.FirmwareVersion));
                        Assert.That(chargingStation.Modem,                              Is.Not.Null);
                    });

                    if (chargingStation. Modem is not null &&
                        chargingStation1.Modem is not null)
                    {

                        Assert.Multiple(() => {
                            Assert.That(chargingStation.Modem.ICCID,                    Is.EqualTo(chargingStation1.Modem.ICCID));
                            Assert.That(chargingStation.Modem.IMSI,                     Is.EqualTo(chargingStation1.Modem.IMSI));
                        });
                    }

                }

            }

            else
                Assert.Fail($"{nameof(SendBootNotifications_Test)} preconditions failed!");

        }

        #endregion

        #region SendFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending firmware status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendFirmwareStatusNotification_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var firmwareStatusNotificationRequests = new ConcurrentList<FirmwareStatusNotificationRequest>();

                testCSMS1.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, firmwareStatusNotificationRequest, ct) => {
                    firmwareStatusNotificationRequests.TryAdd(firmwareStatusNotificationRequest);
                    return Task.CompletedTask;
                };

                var status    = FirmwareStatus.Installed;
                var response  = await chargingStation1.SendFirmwareStatusNotification(
                                          Status:       status,
                                          CustomData:   null
                                      );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(firmwareStatusNotificationRequests.Count,                    Is.EqualTo(1));
                    Assert.That(firmwareStatusNotificationRequests.First().Status,           Is.EqualTo(status));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendFirmwareStatusNotification_Test)} preconditions failed!");

        }

        #endregion

        #region SendPublishFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending piblish firmware status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendPublishFirmwareStatusNotification_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var publishFirmwareStatusNotificationRequests = new ConcurrentList<PublishFirmwareStatusNotificationRequest>();

                testCSMS1.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp, sender, connection, publishFirmwareStatusNotificationRequest, ct) => {
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(publishFirmwareStatusNotificationRequests.Count,             Is.EqualTo(1));
                    Assert.That(publishFirmwareStatusNotificationRequests.First().Status,    Is.EqualTo(status));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendPublishFirmwareStatusNotification_Test)} preconditions failed!");

        }

        #endregion

        #region SendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the CSMS.
        /// </summary>
        [Test]
        public async Task SendHeartbeats_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var heartbeatRequests = new ConcurrentList<HeartbeatRequest>();

                testCSMS1.OCPP.IN.OnHeartbeatRequestReceived += (timestamp, sender, connection, heartbeatRequest, ct) => {
                    heartbeatRequests.TryAdd(heartbeatRequest);
                    return Task.CompletedTask;
                };


                var response = await chargingStation1.SendHeartbeat(
                                         CustomData:   null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                        Is.EqualTo(ResultCode.OK));

                    Assert.That(Timestamp.Now - response.CurrentTime < TimeSpan.FromSeconds(10),   Is.True);

                    Assert.That(heartbeatRequests.Count,                                           Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,         Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,         Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,         Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,         Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,         Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,         Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,         Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,         Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,         Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,         Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,         Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,         Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendHeartbeats_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyEvent_Test()

        /// <summary>
        /// A test for notifying the CSMS about events.
        /// </summary>
        [Test]
        public async Task NotifyEvent_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyEventRequests = new ConcurrentList<NotifyEventRequest>();

                testCSMS1.OCPP.IN.OnNotifyEventRequestReceived += (timestamp, sender, connection, notifyEventRequest, ct) => {
                    notifyEventRequests.TryAdd(notifyEventRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEvent(
                                         GeneratedAt:      Timestamp.Now,
                                         SequenceNumber:   1,
                                         EventData:        [
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
                                                           ],
                                         ToBeContinued:    false,
                                         CustomData:       null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyEventRequests.Count,                                   Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyEvent_Test)} preconditions failed!");

        }

        #endregion

        #region SendSecurityEventNotification_Test()

        /// <summary>
        /// A test for sending security event notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendSecurityEventNotification_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var securityEventNotificationRequests = new ConcurrentList<SecurityEventNotificationRequest>();

                testCSMS1.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, securityEventNotificationRequest, ct) => {
                    securityEventNotificationRequests.TryAdd(securityEventNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendSecurityEventNotification(
                                         Type:         SecurityEventType.MemoryExhaustion,
                                         Timestamp:    Timestamp.Now,
                                         TechInfo:     "Too many open TCP sockets!",
                                         CustomData:   null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(securityEventNotificationRequests.Count,                     Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendSecurityEventNotification_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyReport_Test()

        /// <summary>
        /// A test for notifying the CSMS about reports.
        /// </summary>
        [Test]
        public async Task NotifyReport_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyReportRequests = new ConcurrentList<NotifyReportRequest>();

                testCSMS1.OCPP.IN.OnNotifyReportRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyReport(
                                         NotifyReportRequestId:   1,
                                         SequenceNumber:          1,
                                         GeneratedAt:             Timestamp.Now,
                                         ReportData:              [
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
                                                                          VariableAttributes:        [
                                                                                                         new VariableAttribute(
                                                                                                             Type:             AttributeTypes.Actual,
                                                                                                             Value:            "123",
                                                                                                             Mutability:       MutabilityTypes.ReadWrite,
                                                                                                             Persistent:       true,
                                                                                                             Constant:         false,
                                                                                                             CustomData:       null
                                                                                                         )
                                                                                                     ],
                                                                          VariableCharacteristics:   new VariableCharacteristics(
                                                                                                         DataType:             DataTypes.Decimal,
                                                                                                         SupportsMonitoring:   true,
                                                                                                         Unit:                 UnitsOfMeasure.Celsius(
                                                                                                                                   Multiplier:   1,
                                                                                                                                   CustomData:   null
                                                                                                                               ),
                                                                                                         MinLimit:             0.1M,
                                                                                                         MaxLimit:             9.9M,
                                                                                                         ValuesList:           [ "" ],
                                                                                                         CustomData:           null
                                                                                                     ),
                                                                          CustomData:                null
                                                                      )
                                                                  ],
                                         CustomData:              null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyReportRequests.Count,                                  Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyReport_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyMonitoringReport_Test()

        /// <summary>
        /// A test for notifying the CSMS about monitoring reports.
        /// </summary>
        [Test]
        public async Task NotifyMonitoringReport_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyMonitoringReportRequests = new ConcurrentList<NotifyMonitoringReportRequest>();

                testCSMS1.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp, sender, connection, notifyMonitoringReportRequest, ct) => {
                    notifyMonitoringReportRequests.TryAdd(notifyMonitoringReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyMonitoringReport(
                                         NotifyMonitoringReportRequestId:   1,
                                         SequenceNumber:                    1,
                                         GeneratedAt:                       Timestamp.Now,
                                         MonitoringData:                    [
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
                                                                            ],
                                         ToBeContinued:                     false,
                                         CustomData:                        null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyMonitoringReportRequests.Count,                        Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyMonitoringReport_Test)} preconditions failed!");

        }

        #endregion

        #region SendLogStatusNotification_Test()

        /// <summary>
        /// A test for sending log status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendLogStatusNotification_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var logStatusNotificationRequests = new ConcurrentList<LogStatusNotificationRequest>();

                testCSMS1.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp, sender, connection, logStatusNotificationRequest, ct) => {
                    logStatusNotificationRequests.TryAdd(logStatusNotificationRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendLogStatusNotification(
                                         Status:         UploadLogStatus.Uploaded,
                                         LogRequestId:   1,
                                         CustomData:     null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(logStatusNotificationRequests.Count,                         Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendLogStatusNotification_Test)} preconditions failed!");

        }

        #endregion


        #region TransferTextData_Test()

        /// <summary>
        /// A test for transfering text data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferTextData_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                testCSMS1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       GraphDefined;
                var messageId  = Message_Id.      Parse       (RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await chargingStation1.TransferData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Data?.ToString(),                                   Is.EqualTo(data.Reverse()));

                    Assert.That(dataTransferRequests.Count,                                  Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().VendorId,                       Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                      Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.ToString(),               Is.EqualTo(data));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(TransferTextData_Test)} preconditions failed!");

        }

        #endregion

        #region TransferJObjectData_Test()

        /// <summary>
        /// A test for transfering JObject data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJObjectData_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                testCSMS1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
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
                                           Data:        data
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Data?.Type,                                         Is.EqualTo(JTokenType.Object));

                    Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),           Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                                  Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().VendorId,                       Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                      Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                     Is.EqualTo(JTokenType.Object));
                    Assert.That(dataTransferRequests.First().Data?["key"]?.Value<String>(),  Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(TransferJObjectData_Test)} preconditions failed!");

        }

        #endregion

        #region TransferJArrayData_Test()

        /// <summary>
        /// A test for transfering JArray data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJArrayData_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                testCSMS1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
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
                                           Data:        data
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Data?.Type,                                         Is.EqualTo(JTokenType.Array));

                    Assert.That(response.Data?[0]?.Value<String>()?.Reverse(),               Is.EqualTo(data[0]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                                  Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().VendorId,                       Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                      Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                     Is.EqualTo(JTokenType.Array));
                    Assert.That(dataTransferRequests.First().Data?[0]?.Value<String>(),      Is.EqualTo(data[0]?.Value<String>()));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(TransferJArrayData_Test)} preconditions failed!");

        }

        #endregion


        #region SendCertificateSigningRequest_Test()

        /// <summary>
        /// A test for sending a certificate signing request to the CSMS.
        /// </summary>
        [Test]
        public async Task SendCertificateSigningRequest_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var signCertificateRequests = new ConcurrentList<SignCertificateRequest>();

                testCSMS1.OCPP.IN.OnSignCertificateRequestReceived += (timestamp, sender, connection, signCertificateRequest, ct) => {
                    signCertificateRequests.TryAdd(signCertificateRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendCertificateSigningRequest(
                                         CSR:                        "0x1234",
                                         SignCertificateRequestId:   1,
                                         CertificateType:            CertificateSigningUse.ChargingStationCertificate,
                                         CustomData:                 null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(signCertificateRequests.Count,                               Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendCertificateSigningRequest_Test)} preconditions failed!");

        }

        #endregion

        #region Get15118EVCertificate_Test()

        /// <summary>
        /// A test for receiving a 15118 EV contract certificate from the CSMS.
        /// </summary>
        [Test]
        public async Task Get15118EVCertificate_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyReportRequests = new ConcurrentList<Get15118EVCertificateRequest>();

                testCSMS1.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {
                    notifyReportRequests.TryAdd(notifyReportRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.Get15118EVCertificate(
                                         ISO15118SchemaVersion:   ISO15118SchemaVersion.Parse("15118-20:BastelBrothers"),
                                         CertificateAction:       CertificateAction.Install,
                                         EXIRequest:              EXIData.Parse("0x1234"),
                                         CustomData:              null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyReportRequests.Count,                                  Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(Get15118EVCertificate_Test)} preconditions failed!");

        }

        #endregion

        #region GetCertificateStatus_Test()

        /// <summary>
        /// A test for notifying the CSMS about reports.
        /// </summary>
        [Test]
        public async Task GetCertificateStatus_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyReportRequests = new ConcurrentList<GetCertificateStatusRequest>();

                testCSMS1.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {
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

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyReportRequests.Count,                                  Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(GetCertificateStatus_Test)} preconditions failed!");

        }

        #endregion

        #region GetCRLRequest_Test()

        /// <summary>
        /// A test for fetching a certificate revocation list from the CSMS for the specified certificate.
        /// </summary>
        [Test]
        public async Task GetCRLRequest_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var getCRLRequests = new ConcurrentList<GetCRLRequest>();

                testCSMS1.OCPP.IN.OnGetCRLRequestReceived += (timestamp, sender, connection, getCRLRequest, ct) => {
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(getCRLRequests.Count,                                        Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(GetCRLRequest_Test)} preconditions failed!");

        }

        #endregion


        #region SendReservationStatusUpdate_Test()

        /// <summary>
        /// A test for sending reservation status updates to the CSMS.
        /// </summary>
        [Test]
        public async Task SendReservationStatusUpdate_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var reservationStatusUpdateRequests = new ConcurrentList<ReservationStatusUpdateRequest>();

                testCSMS1.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp, sender, connection, reservationStatusUpdateRequest, ct) => {
                    reservationStatusUpdateRequests.TryAdd(reservationStatusUpdateRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendReservationStatusUpdate(
                                         ReservationId:             Reservation_Id.NewRandom,
                                         ReservationUpdateStatus:   ReservationUpdateStatus.Expired,
                                         CustomData:                null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(reservationStatusUpdateRequests.Count,                       Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendReservationStatusUpdate_Test)} preconditions failed!");

        }

        #endregion

        #region Authorize_Test()

        /// <summary>
        /// A test for authorizing id tokens against the CSMS.
        /// </summary>
        [Test]
        public async Task Authorize_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var authorizeRequests = new ConcurrentList<AuthorizeRequest>();

                testCSMS1.OCPP.IN.OnAuthorizeRequestReceived += (timestamp, sender, connection, authorizeRequest, ct) => {
                    authorizeRequests.TryAdd(authorizeRequest);
                    return Task.CompletedTask;
                };


                var idToken   = IdToken.NewRandomRFID7();
                var response  = await chargingStation1.Authorize(
                                          IdToken:                       idToken,
                                          Certificate:                   null,
                                          ISO15118CertificateHashData:   null,
                                          CustomData:                    null
                                      );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.IdTokenInfo.Status,                                 Is.EqualTo(AuthorizationStatus.Accepted));

                    Assert.That(authorizeRequests.Count,                                     Is.EqualTo(1));
                    Assert.That(authorizeRequests.First().NetworkPath.Source,                Is.EqualTo(chargingStation1.Id));
                    Assert.That(authorizeRequests.First().IdToken,                           Is.EqualTo(idToken));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(Authorize_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyEVChargingNeeds_Test()

        /// <summary>
        /// A test for notifying the CSMS about EV charging needs.
        /// </summary>
        [Test]
        public async Task NotifyEVChargingNeeds_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyEVChargingNeedsRequests = new ConcurrentList<NotifyEVChargingNeedsRequest>();

                testCSMS1.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp, sender, connection, notifyEVChargingNeedsRequest, ct) => {
                    notifyEVChargingNeedsRequests.TryAdd(notifyEVChargingNeedsRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEVChargingNeeds(
                                         EVSEId:              EVSE_Id.Parse(1),
                                         ChargingNeeds:       new ChargingNeeds(
                                                                  RequestedEnergyTransferMode:   EnergyTransferMode.AC_ThreePhases,
                                                                  DepartureTime:                 Timestamp.Now + TimeSpan.FromHours(3),
                                                                  ACChargingParameters:          new ACChargingParameters(
                                                                                                     EnergyAmount:       WattHour.      ParseKWh( 20),
                                                                                                     EVMinCurrent:       Ampere.        ParseA  (  6),
                                                                                                     EVMaxCurrent:       Ampere.        ParseA  ( 32),
                                                                                                     EVMaxVoltage:       Volt.          ParseV  (230),
                                                                                                     CustomData:         null
                                                                                                 ),
                                                                  DCChargingParameters:          new DCChargingParameters(
                                                                                                     EVMaxCurrent:       Ampere.        ParseA  ( 20),
                                                                                                     EVMaxVoltage:       Volt.          ParseV  (900),
                                                                                                     EnergyAmount:       WattHour.      ParseKWh(300),
                                                                                                     EVMaxPower:         Watt.          ParseKW ( 60),
                                                                                                     StateOfCharge:      PercentageByte.Parse   ( 23),
                                                                                                     EVEnergyCapacity:   WattHour.      ParseKWh(250),
                                                                                                     FullSoC:            PercentageByte.Parse   ( 95),
                                                                                                     BulkSoC:            PercentageByte.Parse   ( 80),
                                                                                                     CustomData:         null
                                                                                                 ),
                                                                  CustomData:                    null
                                                              ),
                                         MaxScheduleTuples:   16,
                                         CustomData:          null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyEVChargingNeedsRequests.Count,                         Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyEVChargingNeeds_Test)} preconditions failed!");

        }

        #endregion

        #region SendTransactionEvent_Test()

        /// <summary>
        /// A test for sending a transaction event to the CSMS.
        /// </summary>
        [Test]
        public async Task SendTransactionEvent_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var transactionEventRequests = new ConcurrentList<TransactionEventRequest>();

                testCSMS1.OCPP.IN.OnTransactionEventRequestReceived += (timestamp, sender, connection, transactionEventRequest, ct) => {
                    transactionEventRequests.TryAdd(transactionEventRequest);
                    return Task.CompletedTask;
                };

                var evseId          = EVSE_Id.     Parse(1);
                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.     NewRandomRFID7();
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
                                                MeterValues:             [
                                                                             new MeterValue(
                                                                                 SampledValues:   [

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

                                                                                                  ],
                                                                                 Timestamp:       startTimestamp,
                                                                                 CustomData:      null
                                                                             )
                                                                         ],
                                                PreconditioningStatus:   PreconditioningStatus.Ready,
                                                CustomData:              null

                                            );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(transactionEventRequests.Count,                              Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendTransactionEvent_Test)} preconditions failed!");

        }

        #endregion

        #region SendStatusNotification_Test()

        /// <summary>
        /// A test for sending status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendStatusNotification_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var statusNotificationRequests = new ConcurrentList<StatusNotificationRequest>();

                testCSMS1.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp, sender, connection, statusNotificationRequest, ct) => {
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(statusNotificationRequests.Count,                            Is.EqualTo(1));
                    Assert.That(statusNotificationRequests.First().EVSEId,                   Is.EqualTo(evseId));
                    Assert.That(statusNotificationRequests.First().ConnectorId,              Is.EqualTo(connectorId));
                    Assert.That(statusNotificationRequests.First().ConnectorStatus,          Is.EqualTo(connectorStatus));
                    Assert.That(statusNotificationRequests.First().Timestamp.ToIso8601(),    Is.EqualTo(statusTimestamp.ToIso8601()));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendStatusNotification_Test)} preconditions failed!");

        }

        #endregion

        #region SendMeterValues_Test()

        /// <summary>
        /// A test for sending meter values to the CSMS.
        /// </summary>
        [Test]
        public async Task SendMeterValues_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var meterValuesRequests = new ConcurrentList<MeterValuesRequest>();

                testCSMS1.OCPP.IN.OnMeterValuesRequestReceived += (timestamp, sender, connection, meterValuesRequest, ct) => {
                    meterValuesRequests.TryAdd(meterValuesRequest);
                    return Task.CompletedTask;
                };

                var evseId       = EVSE_Id.Parse(1);
                var meterValues  = new[] {
                                       new MeterValue(
                                           Timestamp.Now - TimeSpan.FromMinutes(5),
                                           [
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
                                           ]
                                       ),
                                       new MeterValue(
                                           Timestamp.Now,
                                           [
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
                                           ]
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));



                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendMeterValues_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyChargingLimit_Test()

        /// <summary>
        /// A test for notifying the CSMS about charging limits.
        /// </summary>
        [Test]
        public async Task NotifyChargingLimit_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyChargingLimitRequests = new ConcurrentList<NotifyChargingLimitRequest>();

                testCSMS1.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp, sender, connection, notifyChargingLimitRequest, ct) => {
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
                                         ChargingSchedules:   [
                                                                  new ChargingSchedule(
                                                                      Id:                        ChargingSchedule_Id.NewRandom(),
                                                                      ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                      ChargingSchedulePeriods:   [
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
                                                                                                 ],
                                                                      StartSchedule:             Timestamp.Now,
                                                                      Duration:                  TimeSpan.FromMinutes(30),
                                                                      MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                      SalesTariff:               new SalesTariff(
                                                                                                     Id:                   SalesTariff_Id.NewRandom,
                                                                                                     SalesTariffEntries:   [
                                                                                                                               new SalesTariffEntry(
                                                                                                                                   RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                               Start:        TimeSpan.Zero,
                                                                                                                                                               Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                               CustomData:   null
                                                                                                                                                           ),
                                                                                                                                   EPriceLevel:            1,
                                                                                                                                   ConsumptionCosts:       [
                                                                                                                                                               new ConsumptionCost(
                                                                                                                                                                   StartValue:   1,
                                                                                                                                                                   Costs:        [
                                                                                                                                                                                     new Cost(
                                                                                                                                                                                         CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                         Amount:             200,
                                                                                                                                                                                         AmountMultiplier:   23,
                                                                                                                                                                                         CustomData:         null
                                                                                                                                                                                     )
                                                                                                                                                                                 ],
                                                                                                                                                                   CustomData:   null
                                                                                                                                                               )
                                                                                                                                                           ],
                                                                                                                                   CustomData:             null
                                                                                                                               )
                                                                                                                           ],
                                                                                                     Description:          "Green Charging ++",
                                                                                                     NumEPriceLevels:      1,
                                                                                                     CustomData:           null
                                                                                                 ),
                                                                      CustomData:                null
                                                                  )
                                                              ],
                                         EVSEId:              EVSE_Id.Parse("1"),
                                         CustomData:          null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyChargingLimitRequests.Count,                           Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyChargingLimit_Test)} preconditions failed!");

        }

        #endregion

        #region SendClearedChargingLimit_Test()

        /// <summary>
        /// A test for indicating a cleared charging limit to the CSMS.
        /// </summary>
        [Test]
        public async Task SendClearedChargingLimit_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var clearedChargingLimitRequests = new ConcurrentList<ClearedChargingLimitRequest>();

                testCSMS1.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp, sender, connection, clearedChargingLimitRequest, ct) => {
                    clearedChargingLimitRequests.TryAdd(clearedChargingLimitRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.SendClearedChargingLimit(
                                         ChargingLimitSource:   ChargingLimitSource.SO,
                                         EVSEId:                EVSE_Id.Parse("1"),
                                         CustomData:            null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(clearedChargingLimitRequests.Count,                          Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(SendClearedChargingLimit_Test)} preconditions failed!");

        }

        #endregion

        #region ReportChargingProfiles_Test()

        /// <summary>
        /// A test for reporting charging profiles to the CSMS.
        /// </summary>
        [Test]
        public async Task ReportChargingProfiles_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var reportChargingProfilesRequests = new ConcurrentList<ReportChargingProfilesRequest>();

                testCSMS1.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp, sender, connection, reportChargingProfilesRequest, ct) => {
                    reportChargingProfilesRequests.TryAdd(reportChargingProfilesRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.ReportChargingProfiles(
                                         ReportChargingProfilesRequestId:   1,
                                         ChargingLimitSource:               ChargingLimitSource.SO,
                                         EVSEId:                            EVSE_Id.Parse("1"),
                                         ChargingProfiles:                  [
                                                                                new ChargingProfile(
                                                                                    ChargingProfileId:        ChargingProfile_Id.NewRandom,
                                                                                    StackLevel:               1,
                                                                                    ChargingProfilePurpose:   ChargingProfilePurpose.TxDefaultProfile,
                                                                                    ChargingProfileKind:      ChargingProfileKinds.   Absolute,
                                                                                    ChargingSchedules:        [
                                                                                                                  new ChargingSchedule(
                                                                                                                      Id:                        ChargingSchedule_Id.NewRandom(),
                                                                                                                      ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                                                                      ChargingSchedulePeriods:   [
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
                                                                                                                                                 ],
                                                                                                                      StartSchedule:             Timestamp.Now,
                                                                                                                      Duration:                  TimeSpan.FromMinutes(30),
                                                                                                                      MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                                                                      SalesTariff:               new SalesTariff(
                                                                                                                                                     Id:                   SalesTariff_Id.NewRandom,
                                                                                                                                                     SalesTariffEntries:   [
                                                                                                                                                                               new SalesTariffEntry(
                                                                                                                                                                                   RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                                                                               Start:        TimeSpan.Zero,
                                                                                                                                                                                                               Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                                                                               CustomData:   null
                                                                                                                                                                                                           ),
                                                                                                                                                                                   EPriceLevel:            1,
                                                                                                                                                                                   ConsumptionCosts:       [
                                                                                                                                                                                                               new ConsumptionCost(
                                                                                                                                                                                                                   StartValue:   1,
                                                                                                                                                                                                                   Costs:        [
                                                                                                                                                                                                                                     new Cost(
                                                                                                                                                                                                                                         CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                                                                         Amount:             200,
                                                                                                                                                                                                                                         AmountMultiplier:   23,
                                                                                                                                                                                                                                         CustomData:         null
                                                                                                                                                                                                                                     )
                                                                                                                                                                                                                                 ],
                                                                                                                                                                                                                   CustomData:   null
                                                                                                                                                                                                               )
                                                                                                                                                                                                           ],
                                                                                                                                                                                   CustomData:             null
                                                                                                                                                                               )
                                                                                                                                                                           ],
                                                                                                                                                     Description:          "Green Charging ++",
                                                                                                                                                     NumEPriceLevels:      1,
                                                                                                                                                     CustomData:           null
                                                                                                                                                 ),
                                                                                                                      CustomData:                null
                                                                                                                  )
                                                                                                              ],
                                                                                    CustomData:               null
                                                                                )
                                                                            ],
                                         ToBeContinued:                     false,
                                         CustomData:                        null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(reportChargingProfilesRequests.Count,                        Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(ReportChargingProfiles_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyEVChargingSchedule_Test()

        /// <summary>
        /// A test for reporting charging profiles to the CSMS.
        /// </summary>
        [Test]
        public async Task NotifyEVChargingSchedule_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyEVChargingScheduleRequests = new ConcurrentList<NotifyEVChargingScheduleRequest>();

                testCSMS1.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp, sender, connection, notifyEVChargingScheduleRequest, ct) => {
                    notifyEVChargingScheduleRequests.TryAdd(notifyEVChargingScheduleRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyEVChargingSchedule(

                                         TimeBase:                            Timestamp.Now,
                                         EVSEId:                              EVSE_Id.Parse("1"),
                                         ChargingSchedule:                    new ChargingSchedule(
                                                                                  Id:                        ChargingSchedule_Id.NewRandom(),
                                                                                  ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                                  ChargingSchedulePeriods:   [
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
                                                                                                             ],
                                                                                  StartSchedule:             Timestamp.Now,
                                                                                  Duration:                  TimeSpan.FromMinutes(30),
                                                                                  MinChargingRate:           ChargingRateValue.Parse(6, ChargingRateUnits.Watts),
                                                                                  SalesTariff:               new SalesTariff(
                                                                                                                 Id:                   SalesTariff_Id.NewRandom,
                                                                                                                 SalesTariffEntries:   [
                                                                                                                                           new SalesTariffEntry(
                                                                                                                                               RelativeTimeInterval:   new RelativeTimeInterval(
                                                                                                                                                                           Start:        TimeSpan.Zero,
                                                                                                                                                                           Duration:     TimeSpan.FromMinutes(30),
                                                                                                                                                                           CustomData:   null
                                                                                                                                                                       ),
                                                                                                                                               EPriceLevel:            1,
                                                                                                                                               ConsumptionCosts:       [
                                                                                                                                                                           new ConsumptionCost(
                                                                                                                                                                               StartValue:   1,
                                                                                                                                                                               Costs:        [
                                                                                                                                                                                                 new Cost(
                                                                                                                                                                                                     CostKind:           CostKinds.CarbonDioxideEmission,
                                                                                                                                                                                                     Amount:             200,
                                                                                                                                                                                                     AmountMultiplier:   23,
                                                                                                                                                                                                     CustomData:         null
                                                                                                                                                                                                 )
                                                                                                                                                                                             ],
                                                                                                                                                                               CustomData:   null
                                                                                                                                                                           )
                                                                                                                                                                       ],
                                                                                                                                               CustomData:             null
                                                                                                                                           )
                                                                                                                                       ],
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                             Is.EqualTo(GenericStatus.Accepted));

                    Assert.That(notifyEVChargingScheduleRequests.Count,                      Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyEVChargingSchedule_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyPriorityCharging_Test()

        /// <summary>
        /// A test for notifying the CMS about priority charging at a charging station.
        /// </summary>
        [Test]
        public async Task NotifyPriorityCharging_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyPriorityChargingRequests = new ConcurrentList<NotifyPriorityChargingRequest>();

                testCSMS1.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp, sender, connection, notifyPriorityChargingRequest, ct) => {
                    notifyPriorityChargingRequests.TryAdd(notifyPriorityChargingRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyPriorityCharging(
                                         TransactionId:                     Transaction_Id.Parse("1234"),
                                         Activated:                         true,
                                         CustomData:                        null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyPriorityChargingRequests.Count,                        Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyPriorityCharging_Test)} preconditions failed!");

        }

        #endregion

        #region NotifySettlement_Test()

        /// <summary>
        /// A test for notifying the CSMS about payment settlements at a charging station.
        /// </summary>
        [Test]
        public async Task NotifySettlement_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifySettlementRequests = new ConcurrentList<NotifySettlementRequest>();

                testCSMS1.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp, sender, connection, notifyPriorityChargingRequest, ct) => {
                    notifySettlementRequests.TryAdd(notifyPriorityChargingRequest);
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifySettlementRequests.Count,                              Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifySettlement_Test)} preconditions failed!");

        }

        #endregion

        #region PullDynamicScheduleUpdate_Test()

        /// <summary>
        /// A test for pulling dynamic schedule updates from the CSMS.
        /// </summary>
        [Test]
        public async Task PullDynamicScheduleUpdate_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var pullDynamicScheduleUpdateRequests = new ConcurrentList<PullDynamicScheduleUpdateRequest>();

                testCSMS1.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp, sender, connection, pullDynamicScheduleUpdateRequest, ct) => {
                    pullDynamicScheduleUpdateRequests.TryAdd(pullDynamicScheduleUpdateRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.PullDynamicScheduleUpdate(
                                         ChargingProfileId:   ChargingProfile_Id.Parse(235),
                                         CustomData:          null
                                     );


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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(pullDynamicScheduleUpdateRequests.Count,                     Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(PullDynamicScheduleUpdate_Test)} preconditions failed!");

        }

        #endregion


        #region NotifyDisplayMessages_Test()

        /// <summary>
        /// A test for notifying the CSMS about display messages.
        /// </summary>
        [Test]
        public async Task NotifyDisplayMessages_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyDisplayMessagesRequests = new ConcurrentList<NotifyDisplayMessagesRequest>();

                testCSMS1.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp, sender, connection, notifyDisplayMessagesRequest, ct) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
                };

                var response = await chargingStation1.NotifyDisplayMessages(
                                         NotifyDisplayMessagesRequestId:   1,
                                         MessageInfos:                     [
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
                                                                           ],
                                         CustomData:                       null
                                     );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyDisplayMessagesRequests.Count,                         Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyDisplayMessages_Test)} preconditions failed!");

        }

        #endregion

        #region NotifyCustomerInformation_Test()

        /// <summary>
        /// A test for notifying the CSMS about customer information.
        /// </summary>
        [Test]
        public async Task NotifyCustomerInformation_Test()
        {

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyCustomerInformationRequests = new ConcurrentList<NotifyCustomerInformationRequest>();

                testCSMS1.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp, sender, connection, notifyCustomerInformationRequest, ct) => {
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


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));

                    Assert.That(notifyCustomerInformationRequests.Count,                     Is.EqualTo(1));

                    Assert.That(csms1WebSocketJSONRequestsSent.                     Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONRequestErrorsReceived.            Count,   Is.EqualTo(0));
                    Assert.That(csms1WebSocketJSONResponsesReceived.                Count,   Is.EqualTo(0));

                    Assert.That(csms1WebSocketJSONRequestsReceived.                 Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponsesSent.                    Count,   Is.EqualTo(1));
                    Assert.That(csms1WebSocketJSONResponseErrorsReceived.           Count,   Is.EqualTo(0));

                    Assert.That(chargingStation1WebSocketJSONRequestsSent.          Count,   Is.EqualTo(1));
                    Assert.That(chargingStation1WebSocketJSONRequestErrorsReceived. Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesReceived.     Count,   Is.EqualTo(1));

                    Assert.That(chargingStation1WebSocketJSONRequestsReceived.      Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponsesSent.         Count,   Is.EqualTo(0));
                    Assert.That(chargingStation1WebSocketJSONResponseErrorsReceived.Count,   Is.EqualTo(0));

                });

            }

            else
                Assert.Fail($"{nameof(NotifyCustomerInformation_Test)} preconditions failed!");

        }

        #endregion


    }

}
