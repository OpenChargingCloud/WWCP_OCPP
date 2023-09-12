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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests
{

    /// <summary>
    /// Unit tests for a central system sending messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMSMessagesTests : AChargingStationTests
    {

        #region Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OnResetRequest += (timestamp, sender, resetRequest) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetTypes.Immediate;
                var response1  = await testCSMS01.Reset(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           ResetType:     resetType,
                                           CustomData:    null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ResetStatus.Accepted,           response1.Status);

                Assert.AreEqual(1,                              resetRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   resetRequests.First().ChargeBoxId);
                Assert.AreEqual(resetType,                      resetRequests.First().ResetType);

            }

        }

        #endregion

        #region Reset_UnknownChargeBox_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_UnknownChargeBox_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation2.OnResetRequest += (timestamp, sender, resetRequest) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
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


        #region UpdateFirmware_Test()

        /// <summary>
        /// A test for updating the firmware of a charging station.
        /// </summary>
        [Test]
        public async Task UpdateFirmware_Test()
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

                var updateFirmwareRequests = new ConcurrentList<UpdateFirmwareRequest>();

                chargingStation1.OnUpdateFirmwareRequest += (timestamp, sender, updateFirmwareRequest) => {
                    updateFirmwareRequests.TryAdd(updateFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.UpdateFirmware(
                                           ChargeBoxId:               chargingStation1.ChargeBoxId,
                                           Firmware:                  new Firmware(
                                                                          FirmwareURL:          URL.Parse("https://example.org/fw0001.bin"),
                                                                          RetrieveTimestamp:    Timestamp.Now,
                                                                          InstallTimestamp:     Timestamp.Now,
                                                                          SigningCertificate:   "0x1234",
                                                                          Signature:            "0x5678",
                                                                          CustomData:           null
                                                                      ),
                                           UpdateFirmwareRequestId:   1,
                                           Retries:                   5,
                                           RetryInterval:             TimeSpan.FromMinutes(5),
                                           CustomData:                null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              updateFirmwareRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   updateFirmwareRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region PublishFirmware_Test()

        /// <summary>
        /// A test for publishing a firmware update onto a charging station/local controller.
        /// </summary>
        [Test]
        public async Task PublishFirmware_Test()
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

                var publishFirmwareRequests = new ConcurrentList<PublishFirmwareRequest>();

                chargingStation1.OnPublishFirmwareRequest += (timestamp, sender, publishFirmwareRequest) => {
                    publishFirmwareRequests.TryAdd(publishFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.PublishFirmware(
                                           ChargeBoxId:                chargingStation1.ChargeBoxId,
                                           PublishFirmwareRequestId:   1,
                                           DownloadLocation:           URL.Parse("https://example.org/fw0001.bin"),
                                           MD5Checksum:                "0x1234",
                                           Retries:                    5,
                                           RetryInterval:              TimeSpan.FromMinutes(5),
                                           CustomData:                 null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              publishFirmwareRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   publishFirmwareRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region UnpublishFirmware_Test()

        /// <summary>
        /// A test for unpublishing a firmware update from a charging station/local controller.
        /// </summary>
        [Test]
        public async Task UnpublishFirmware_Test()
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

                var unpublishFirmwareRequests = new ConcurrentList<UnpublishFirmwareRequest>();

                chargingStation1.OnUnpublishFirmwareRequest += (timestamp, sender, unpublishFirmwareRequest) => {
                    unpublishFirmwareRequests.TryAdd(unpublishFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.UnpublishFirmware(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           MD5Checksum:   "0x1234",
                                           CustomData:    null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              unpublishFirmwareRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   unpublishFirmwareRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetBaseReport_Test()

        /// <summary>
        /// A test for getting a base report from a charging station.
        /// </summary>
        [Test]
        public async Task GetBaseReport_Test()
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

                var getBaseReportRequests = new ConcurrentList<GetBaseReportRequest>();

                chargingStation1.OnGetBaseReportRequest += (timestamp, sender, getBaseReportRequest) => {
                    getBaseReportRequests.TryAdd(getBaseReportRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetBaseReport(
                                           ChargeBoxId:              chargingStation1.ChargeBoxId,
                                           GetBaseReportRequestId:   1,
                                           ReportBase:               ReportBases.FullInventory,
                                           CustomData:               null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getBaseReportRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getBaseReportRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetReport_Test()

        /// <summary>
        /// A test for getting a report from a charging station.
        /// </summary>
        [Test]
        public async Task GetReport_Test()
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

                var getReportRequests = new ConcurrentList<GetReportRequest>();

                chargingStation1.OnGetReportRequest += (timestamp, sender, getReportRequest) => {
                    getReportRequests.TryAdd(getReportRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetReport(
                                           ChargeBoxId:          chargingStation1.ChargeBoxId,
                                           GetReportRequestId:   1,
                                           ComponentCriteria:    new[] {
                                                                     ComponentCriteria.Available
                                                                 },
                                           ComponentVariables:   new[] {
                                                                     new ComponentVariable(
                                                                         Component:    new Component(
                                                                                           Name:         "Alert System!",
                                                                                           Instance:     "Alert System #1",
                                                                                           EVSE:         new EVSE(
                                                                                                             Id:            EVSE_Id.     Parse(1),
                                                                                                             ConnectorId:   Connector_Id.Parse(1),
                                                                                                             CustomData:    null
                                                                                                         ),
                                                                                           CustomData:   null
                                                                                       ),
                                                                         Variable:     new Variable(
                                                                                           Name:         "Temperature Sensors",
                                                                                           Instance:     "Temperature Sensor #1",
                                                                                           CustomData:   null
                                                                                       ),
                                                                         CustomData:   null
                                                                     )
                                                                 },
                                           CustomData:           null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getReportRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getReportRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetLog_Test()

        /// <summary>
        /// A test for getting a log file from a charging station.
        /// </summary>
        [Test]
        public async Task GetLog_Test()
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

                var getLogRequests = new ConcurrentList<GetLogRequest>();

                chargingStation1.OnGetLogRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetLog(
                                           ChargeBoxId:    chargingStation1.ChargeBoxId,
                                           LogType:        LogTypes.DiagnosticsLog,
                                           LogRequestId:   1,
                                           Log:            new LogParameters(
                                                               RemoteLocation:    URL.Parse("https://example.org/log0001.log"),
                                                               OldestTimestamp:   Timestamp.Now - TimeSpan.FromDays(2),
                                                               LatestTimestamp:   Timestamp.Now,
                                                               CustomData:        null
                                                           ),
                                           CustomData:     null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SetVariables_Test()

        /// <summary>
        /// A test for setting variables of a charging station.
        /// </summary>
        [Test]
        public async Task SetVariables_Test()
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

                var getLogRequests = new ConcurrentList<SetVariablesRequest>();

                chargingStation1.OnSetVariablesRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetVariables(
                                           ChargeBoxId:    chargingStation1.ChargeBoxId,
                                           VariableData:   new[] {
                                                               new SetVariableData(
                                                                   AttributeValue:   "123",
                                                                   Component:        new Component(
                                                                                         Name:         "Alert System!",
                                                                                         Instance:     "Alert System #1",
                                                                                         EVSE:         new EVSE(
                                                                                                           Id:            EVSE_Id.     Parse(1),
                                                                                                           ConnectorId:   Connector_Id.Parse(1),
                                                                                                           CustomData:    null
                                                                                                       ),
                                                                                         CustomData:   null
                                                                                     ),
                                                                   Variable:         new Variable(
                                                                                         Name:         "Temperature Sensors",
                                                                                         Instance:     "Temperature Sensor #1",
                                                                                         CustomData:   null
                                                                                     ),
                                                                   AttributeType:    AttributeTypes.Actual,
                                                                   CustomData:       null
                                                               )
                                                           },
                                           CustomData:     null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetVariables_Test()

        /// <summary>
        /// A test for getting variables of a charging station.
        /// </summary>
        [Test]
        public async Task GetVariables_Test()
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

                var getLogRequests = new ConcurrentList<GetVariablesRequest>();

                chargingStation1.OnGetVariablesRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetVariables(
                                           ChargeBoxId:    chargingStation1.ChargeBoxId,
                                           VariableData:   new[] {
                                                               new GetVariableData(
                                                                   Component:       new Component(
                                                                                        Name:         "Alert System!",
                                                                                        Instance:     "Alert System #1",
                                                                                        EVSE:         new EVSE(
                                                                                                          Id:            EVSE_Id.     Parse(1),
                                                                                                          ConnectorId:   Connector_Id.Parse(1),
                                                                                                          CustomData:    null
                                                                                                      ),
                                                                                        CustomData:   null
                                                                                    ),
                                                                   Variable:        new Variable(
                                                                                        Name:         "Temperature Sensors",
                                                                                        Instance:     "Temperature Sensor #1",
                                                                                        CustomData:   null
                                                                                    ),
                                                                   AttributeType:   AttributeTypes.Actual,
                                                                   CustomData:      null
                                                               )
                                                           },
                                           CustomData:     null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SetMonitoringBase_Test()

        /// <summary>
        /// A test for setting the monitoring base of a charging station.
        /// </summary>
        [Test]
        public async Task SetMonitoringBase_Test()
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

                var getLogRequests = new ConcurrentList<SetMonitoringBaseRequest>();

                chargingStation1.OnSetMonitoringBaseRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetMonitoringBase(
                                           ChargeBoxId:      chargingStation1.ChargeBoxId,
                                           MonitoringBase:   MonitoringBases.All,
                                           CustomData:       null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetMonitoringReport_Test()

        /// <summary>
        /// A test for setting the monitoring base of a charging station.
        /// </summary>
        [Test]
        public async Task GetMonitoringReport_Test()
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

                var getLogRequests = new ConcurrentList<GetMonitoringReportRequest>();

                chargingStation1.OnGetMonitoringReportRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetMonitoringReport(
                                           ChargeBoxId:                    chargingStation1.ChargeBoxId,
                                           GetMonitoringReportRequestId:   1,
                                           MonitoringCriteria:             new[] {
                                                                               MonitoringCriteria.PeriodicMonitoring
                                                                           },
                                           ComponentVariables:             new[] {
                                                                               new ComponentVariable(
                                                                                   Component:        new Component(
                                                                                                         Name:         "Alert System!",
                                                                                                         Instance:     "Alert System #1",
                                                                                                         EVSE:         new EVSE(
                                                                                                                           Id:            EVSE_Id.     Parse(1),
                                                                                                                           ConnectorId:   Connector_Id.Parse(1),
                                                                                                                           CustomData:    null
                                                                                                                       ),
                                                                                                         CustomData:   null
                                                                                                     ),
                                                                                   Variable:         new Variable(
                                                                                                         Name:         "Temperature Sensors",
                                                                                                         Instance:     "Temperature Sensor #1",
                                                                                                         CustomData:   null
                                                                                                     )
                                                                               )
                                                                           },
                                           CustomData:                     null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SetMonitoringLevel_Test()

        /// <summary>
        /// A test for setting the monitoring level of a charging station.
        /// </summary>
        [Test]
        public async Task SetMonitoringLevel_Test()
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

                var getLogRequests = new ConcurrentList<SetMonitoringLevelRequest>();

                chargingStation1.OnSetMonitoringLevelRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetMonitoringLevel(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           Severity:      Severities.Informational,
                                           CustomData:    null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SetVariableMonitoring_Test()

        /// <summary>
        /// A test for creating a variable monitoring at a charging station.
        /// </summary>
        [Test]
        public async Task SetVariableMonitoring_Test()
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

                var getLogRequests = new ConcurrentList<SetVariableMonitoringRequest>();

                chargingStation1.OnSetVariableMonitoringRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetVariableMonitoring(
                                           ChargeBoxId:      chargingStation1.ChargeBoxId,
                                           MonitoringData:   new[] {
                                                                 new SetMonitoringData(
                                                                     Value:                  23.2M,
                                                                     MonitorType:            MonitorTypes.Delta,
                                                                     Severity:               Severities.Critical,
                                                                     Component:              new Component(
                                                                                                 Name:         "Alert System!",
                                                                                                 Instance:     "Alert System #1",
                                                                                                 EVSE:         new EVSE(
                                                                                                                   Id:            EVSE_Id.     Parse(1),
                                                                                                                   ConnectorId:   Connector_Id.Parse(1),
                                                                                                                   CustomData:    null
                                                                                                               ),
                                                                                                 CustomData:   null
                                                                                             ),
                                                                     Variable:               new Variable(
                                                                                                 Name:         "Temperature Sensors",
                                                                                                 Instance:     "Temperature Sensor #1",
                                                                                                 CustomData:   null
                                                                                             ),
                                                                     VariableMonitoringId:   VariableMonitoring_Id.NewRandom,
                                                                     Transaction:            true,
                                                                     CustomData:             null
                                                                 )
                                                             },
                                           CustomData:       null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region ClearVariableMonitoring_Test()

        /// <summary>
        /// A test for deleting a variable monitoring from a charging station.
        /// </summary>
        [Test]
        public async Task ClearVariableMonitoring_Test()
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

                var getLogRequests = new ConcurrentList<ClearVariableMonitoringRequest>();

                chargingStation1.OnClearVariableMonitoringRequest += (timestamp, sender, getLogRequest) => {
                    getLogRequests.TryAdd(getLogRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.ClearVariableMonitoring(
                                           ChargeBoxId:             chargingStation1.ChargeBoxId,
                                           VariableMonitoringIds:   new[] {
                                                                        VariableMonitoring_Id.NewRandom
                                                                    },
                                           CustomData:              null
                                       );

                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLogRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLogRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SetNetworkProfile_Test()

        /// <summary>
        /// A test for setting the network profile of a charging station.
        /// </summary>
        [Test]
        public async Task SetNetworkProfile_Test()
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

                var setNetworkProfileRequests = new ConcurrentList<SetNetworkProfileRequest>();

                chargingStation1.OnSetNetworkProfileRequest += (timestamp, sender, setNetworkProfileRequest) => {
                    setNetworkProfileRequests.TryAdd(setNetworkProfileRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetNetworkProfile(
                                           ChargeBoxId:                chargingStation1.ChargeBoxId,
                                           ConfigurationSlot:          1,
                                           NetworkConnectionProfile:   new NetworkConnectionProfile(
                                                                           Version:             OCPPVersions.OCPP20,
                                                                           Transport:           TransportProtocols.JSON,
                                                                           CentralServiceURL:   URL.Parse("https://example.com/OCPPv2.0/"),
                                                                           MessageTimeout:      TimeSpan.FromSeconds(30),
                                                                           SecurityProfile:     SecurityProfiles.SecurityProfile3,
                                                                           NetworkInterface:    NetworkInterfaces.Wireless1,
                                                                           VPNConfiguration:    new VPNConfiguration(
                                                                                                    ServerURL:              URL.Parse("https://example.com/OCPPv2.0/"),
                                                                                                    Login:                  "vpn",
                                                                                                    Password:               "pw123",
                                                                                                    SharedSecret:           "secret123",
                                                                                                    Protocol:               VPNProtocols.IPSec,
                                                                                                    AccessGroup:            "group1",
                                                                                                    CustomData:             null
                                                                                                ),
                                                                           APNConfiguration:    new APNConfiguration(
                                                                                                    AccessPointName:        "apn1",
                                                                                                    AuthenticationMethod:   APNAuthenticationMethods.PAP,
                                                                                                    Username:               "root",
                                                                                                    Password:               "pw234",
                                                                                                    SIMPINCode:             "7873",
                                                                                                    PreferredNetwork:       "Vanaheimr Wireless",
                                                                                                    OnlyPreferredNetwork:   false,
                                                                                                    CustomData:             null
                                                                                                ),
                                                                           CustomData:          null
                                                                       ),
                                           CustomData:                 null
                                       );

                Assert.AreEqual(ResultCodes.OK,                     response1.Result.ResultCode);
                Assert.AreEqual(SetNetworkProfileStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                  setNetworkProfileRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,       setNetworkProfileRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region ChangeAvailability_Test()

        /// <summary>
        /// A test for sending a change availability message to a charging station.
        /// </summary>
        [Test]
        public async Task ChangeAvailability_Test()
        {

            if (testCSMS01                                            is not null &&
                testBackendWebSockets01                               is not null &&
                csmsWebSocketTextMessagesSent                         is not null &&
                csmsWebSocketTextMessageResponsesReceived             is not null &&
                csmsWebSocketTextMessagesReceived                     is not null &&
                csmsWebSocketTextMessageResponsesSent                 is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketTextMessagesReceived         is not null &&
                chargingStation1WebSocketTextMessageResponsesSent     is not null &&
                chargingStation1WebSocketTextMessagesSent             is not null &&
                chargingStation1WebSocketTextMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketTextMessagesReceived         is not null &&
                chargingStation2WebSocketTextMessageResponsesSent     is not null &&
                chargingStation2WebSocketTextMessagesSent             is not null &&
                chargingStation2WebSocketTextMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketTextMessagesReceived         is not null &&
                chargingStation3WebSocketTextMessageResponsesSent     is not null &&
                chargingStation3WebSocketTextMessagesSent             is not null &&
                chargingStation3WebSocketTextMessageResponsesReceived is not null)

            {

                var changeAvailabilityRequests = new ConcurrentList<ChangeAvailabilityRequest>();

                chargingStation1.OnChangeAvailabilityRequest += (timestamp, sender, changeAvailabilityRequest) => {
                    changeAvailabilityRequests.TryAdd(changeAvailabilityRequest);
                    return Task.CompletedTask;
                };

                var evseId             = EVSE_Id.     Parse(1);
                var connectorId        = Connector_Id.Parse(1);
                var operationalStatus  = OperationalStatus.Operative;

                var response1          = await testCSMS01.ChangeAvailability(
                                                   ChargeBoxId:         chargingStation1.ChargeBoxId,
                                                   OperationalStatus:   operationalStatus,
                                                   EVSE:                new EVSE(
                                                                            Id:            evseId,
                                                                            ConnectorId:   connectorId,
                                                                            CustomData:    null
                                                                        ),
                                                   CustomData:          null
                                               );

                Assert.AreEqual(ResultCodes.OK,                      response1.Result.ResultCode);
                Assert.AreEqual(ChangeAvailabilityStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                   changeAvailabilityRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,        changeAvailabilityRequests.First().ChargeBoxId);
                Assert.AreEqual(evseId,                              changeAvailabilityRequests.First().EVSE?.Id);
                Assert.AreEqual(connectorId,                         changeAvailabilityRequests.First().EVSE?.ConnectorId);
                Assert.AreEqual(operationalStatus,                   changeAvailabilityRequests.First().OperationalStatus);

                Assert.AreEqual(1, csmsWebSocketTextMessagesSent.                        Count);
                Assert.AreEqual(1, csmsWebSocketTextMessageResponsesReceived.            Count);
                Assert.AreEqual(1, csmsWebSocketTextMessagesReceived.                    Count);
                Assert.AreEqual(0, csmsWebSocketTextMessageResponsesSent.                Count);

                Assert.AreEqual(1, chargingStation1WebSocketTextMessagesReceived.        Count);
                Assert.AreEqual(1, chargingStation1WebSocketTextMessageResponsesSent.    Count);
                Assert.AreEqual(1, chargingStation1WebSocketTextMessagesSent.            Count);
                Assert.AreEqual(0, chargingStation1WebSocketTextMessageResponsesReceived.Count);

            }

        }

        #endregion

        #region TriggerMessage_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task TriggerMessage_Test()
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

                var triggerMessageRequests = new ConcurrentList<TriggerMessageRequest>();

                chargingStation1.OnTriggerMessageRequest += (timestamp, sender, triggerMessageRequest) => {
                    triggerMessageRequests.TryAdd(triggerMessageRequest);
                    return Task.CompletedTask;
                };

                var evseId          = EVSE_Id.Parse(1);
                var messageTrigger  = MessageTriggers.StatusNotification;

                var response1       = await testCSMS01.TriggerMessage(
                                                ChargeBoxId:        chargingStation1.ChargeBoxId,
                                                RequestedMessage:   messageTrigger,
                                                EVSEId:             evseId,
                                                CustomData:         null
                                            );

                Assert.AreEqual(ResultCodes.OK,                  response1.Result.ResultCode);
                Assert.AreEqual(TriggerMessageStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                               triggerMessageRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,    triggerMessageRequests.First().ChargeBoxId);

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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
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

        #region TransferJObjectData_Test()

        /// <summary>
        /// A test for transfering JObject data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJObjectData_Test()
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
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

        #region TransferJArrayData_Test()

        /// <summary>
        /// A test for transfering JArray data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferJArrayData_Test()
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       Parse       ("GraphDefined OEM");
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

        #region TransferTextData_Rejected_Test()

        /// <summary>
        /// A test for sending data to a charging station.
        /// </summary>
        [Test]
        public async Task TransferTextData_Rejected_Test()
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OnIncomingDataTransferRequest += (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.Parse("ACME Inc.");
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


        #region CertificateSigned_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CertificateSigned_Test()
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

                var certificateSignedRequests = new ConcurrentList<CertificateSignedRequest>();

                chargingStation1.OnCertificateSignedRequest += (timestamp, sender, certificateSignedRequest) => {
                    certificateSignedRequests.TryAdd(certificateSignedRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.SendSignedCertificate(
                                           ChargeBoxId:        chargingStation1.ChargeBoxId,
                                           CertificateChain:   new CertificateChain(
                                                                   Certificates:   new[] {
                                                                                       Certificate.Parse(
                                                                                           String.Concat(
                                                                                               "-----BEGIN CERTIFICATE-----\n",
                                                                                               "MIIFfDCCBGSgAwIBAgISAxm1F16JrzgdEDxpDfnyG2xaMA0GCSqGSIb3DQEBCwUA\n",
                                                                                               "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                                                                               "EwJSMzAeFw0yMzAxMDYwNDAwMjZaFw0yMzA0MDYwNDAwMjVaMCIxIDAeBgNVBAMT\n",
                                                                                               "F2phYmJlci5ncmFwaGRlZmluZWQuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\n",
                                                                                               "MIIBCgKCAQEAtucIqzk30QB90mZxCNO+XP2kiY9QMFIsTfupU5IYrqGcQ1Zn+mYa\n",
                                                                                               "7yMW9UDZdJeMpi0Ls3bOOY6HbktNTglIETUD3/hUxtLlSIQXgPV/r7qPmx5+rNgT\n",
                                                                                               "H1uoCJ81Mk/vtGr0hWj/bbEv/FGRLo8KKr10ZZ/PNOs5JA/2SKolGGqst6Xd3Eh5\n",
                                                                                               "JPqSwOeCPv/2D6rWvdEJwsbHBBgXBvdtb4NzGibz/y4VyiPcDZbw1P+F4MucvVEg\n",
                                                                                               "cvFxCoupsolLcX/f49uq3FRgYGloPOAjCkHbbi8HCt0VfL0OKL4ooLtzAtm2VOJA\n",
                                                                                               "ZueprlXzEVES9RR9jfkB5OpE1PMFc4oSEQIDAQABo4ICmjCCApYwDgYDVR0PAQH/\n",
                                                                                               "BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAMBgNVHRMBAf8E\n",
                                                                                               "AjAAMB0GA1UdDgQWBBTRSR2BPdSRXb+ifMhxcHkS+Dn9uTAfBgNVHSMEGDAWgBQU\n",
                                                                                               "LrMXt1hWy65QCUDmH6+dixTCxjBVBggrBgEFBQcBAQRJMEcwIQYIKwYBBQUHMAGG\n",
                                                                                               "FWh0dHA6Ly9yMy5vLmxlbmNyLm9yZzAiBggrBgEFBQcwAoYWaHR0cDovL3IzLmku\n",
                                                                                               "bGVuY3Iub3JnLzBqBgNVHREEYzBhghtjb25mZXJlbmNlLmdyYXBoZGVmaW5lZC5j\n",
                                                                                               "b22CEGdyYXBoZGVmaW5lZC5jb22CF2phYmJlci5ncmFwaGRlZmluZWQuY29tghdw\n",
                                                                                               "dWJzdWIuZ3JhcGhkZWZpbmVkLmNvbTBMBgNVHSAERTBDMAgGBmeBDAECATA3Bgsr\n",
                                                                                               "BgEEAYLfEwEBATAoMCYGCCsGAQUFBwIBFhpodHRwOi8vY3BzLmxldHNlbmNyeXB0\n",
                                                                                               "Lm9yZzCCAQQGCisGAQQB1nkCBAIEgfUEgfIA8AB2AHoyjFTYty22IOo44FIe6YQW\n",
                                                                                               "cDIThU070ivBOlejUutSAAABhYVzpcAAAAQDAEcwRQIhAJCxbUKgpq153bfWcnMv\n",
                                                                                               "4yrKTyqtYBttKHxtw+nWMPQ5AiAmwa2yn/7794mQS3dh2hI79p/hC8p8XKn4jx6j\n",
                                                                                               "ZscOngB2AOg+0No+9QY1MudXKLyJa8kD08vREWvs62nhd31tBr1uAAABhYVzpaAA\n",
                                                                                               "AAQDAEcwRQIhAORY8NM3uxbxTSECXlWNazCywl3Q0G7iAHBOXIqTzJ2iAiAgEkJ4\n",
                                                                                               "14UlG3TnHRgITx3wRXQsY0A95z7wa7YR3nkdWTANBgkqhkiG9w0BAQsFAAOCAQEA\n",
                                                                                               "bwnRFC0EiAs/32J48Ifnt6/hDjqmd5ATo1pCdhy4YIf72EKoPAnZ/kOtaNP5hD8U\n",
                                                                                               "CHVPQqYTaPE6bAPKs4JJOVIRdUJOTBHeYEHSD6iJHL93zWEKP3nB4ZYx5zOibtS0\n",
                                                                                               "dN/EqKU7djyvnwM6fTO5gs07cDu1uToV8lBjhH9EHJu8KJJ4vPXFNgyK30XPx1Fd\n",
                                                                                               "itTVGQId1kGwkuBmBBwbTd5uJiLFBwiJs5Vl/sUj1OHB6fp0pqzJ1M+WlNR3sYM2\n",
                                                                                               "i68/S4sQsqy8ui74d60lNkuFrZzYpB7NRVVKesHOSdGQeYqchGn6c33kI67fvF5a\n",
                                                                                               "Ra0DThYgIhij18nkpwaYHg==\n",
                                                                                               "-----END CERTIFICATE-----\n\n"
                                                                                           )
                                                                                       )
                                                                                   }
                                                               ),
                                           CertificateType:    CertificateSigningUse.ChargingStationCertificate,
                                           CustomData:         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                     response1.Result.ResultCode);
                Assert.AreEqual(CertificateSignedStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                  certificateSignedRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,       certificateSignedRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region InstallCertificate_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task InstallCertificate_Test()
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

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.InstallCertificate(
                                           ChargeBoxId:       chargingStation1.ChargeBoxId,
                                           CertificateType:   CertificateUse.V2GRootCertificate,
                                           Certificate:       Certificate.Parse(
                                                                  String.Concat(
                                                                      "-----BEGIN CERTIFICATE-----\n",
                                                                      "MIIFfDCCBGSgAwIBAgISAxm1F16JrzgdEDxpDfnyG2xaMA0GCSqGSIb3DQEBCwUA\n",
                                                                      "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                                                      "EwJSMzAeFw0yMzAxMDYwNDAwMjZaFw0yMzA0MDYwNDAwMjVaMCIxIDAeBgNVBAMT\n",
                                                                      "F2phYmJlci5ncmFwaGRlZmluZWQuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\n",
                                                                      "MIIBCgKCAQEAtucIqzk30QB90mZxCNO+XP2kiY9QMFIsTfupU5IYrqGcQ1Zn+mYa\n",
                                                                      "7yMW9UDZdJeMpi0Ls3bOOY6HbktNTglIETUD3/hUxtLlSIQXgPV/r7qPmx5+rNgT\n",
                                                                      "H1uoCJ81Mk/vtGr0hWj/bbEv/FGRLo8KKr10ZZ/PNOs5JA/2SKolGGqst6Xd3Eh5\n",
                                                                      "JPqSwOeCPv/2D6rWvdEJwsbHBBgXBvdtb4NzGibz/y4VyiPcDZbw1P+F4MucvVEg\n",
                                                                      "cvFxCoupsolLcX/f49uq3FRgYGloPOAjCkHbbi8HCt0VfL0OKL4ooLtzAtm2VOJA\n",
                                                                      "ZueprlXzEVES9RR9jfkB5OpE1PMFc4oSEQIDAQABo4ICmjCCApYwDgYDVR0PAQH/\n",
                                                                      "BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAMBgNVHRMBAf8E\n",
                                                                      "AjAAMB0GA1UdDgQWBBTRSR2BPdSRXb+ifMhxcHkS+Dn9uTAfBgNVHSMEGDAWgBQU\n",
                                                                      "LrMXt1hWy65QCUDmH6+dixTCxjBVBggrBgEFBQcBAQRJMEcwIQYIKwYBBQUHMAGG\n",
                                                                      "FWh0dHA6Ly9yMy5vLmxlbmNyLm9yZzAiBggrBgEFBQcwAoYWaHR0cDovL3IzLmku\n",
                                                                      "bGVuY3Iub3JnLzBqBgNVHREEYzBhghtjb25mZXJlbmNlLmdyYXBoZGVmaW5lZC5j\n",
                                                                      "b22CEGdyYXBoZGVmaW5lZC5jb22CF2phYmJlci5ncmFwaGRlZmluZWQuY29tghdw\n",
                                                                      "dWJzdWIuZ3JhcGhkZWZpbmVkLmNvbTBMBgNVHSAERTBDMAgGBmeBDAECATA3Bgsr\n",
                                                                      "BgEEAYLfEwEBATAoMCYGCCsGAQUFBwIBFhpodHRwOi8vY3BzLmxldHNlbmNyeXB0\n",
                                                                      "Lm9yZzCCAQQGCisGAQQB1nkCBAIEgfUEgfIA8AB2AHoyjFTYty22IOo44FIe6YQW\n",
                                                                      "cDIThU070ivBOlejUutSAAABhYVzpcAAAAQDAEcwRQIhAJCxbUKgpq153bfWcnMv\n",
                                                                      "4yrKTyqtYBttKHxtw+nWMPQ5AiAmwa2yn/7794mQS3dh2hI79p/hC8p8XKn4jx6j\n",
                                                                      "ZscOngB2AOg+0No+9QY1MudXKLyJa8kD08vREWvs62nhd31tBr1uAAABhYVzpaAA\n",
                                                                      "AAQDAEcwRQIhAORY8NM3uxbxTSECXlWNazCywl3Q0G7iAHBOXIqTzJ2iAiAgEkJ4\n",
                                                                      "14UlG3TnHRgITx3wRXQsY0A95z7wa7YR3nkdWTANBgkqhkiG9w0BAQsFAAOCAQEA\n",
                                                                      "bwnRFC0EiAs/32J48Ifnt6/hDjqmd5ATo1pCdhy4YIf72EKoPAnZ/kOtaNP5hD8U\n",
                                                                      "CHVPQqYTaPE6bAPKs4JJOVIRdUJOTBHeYEHSD6iJHL93zWEKP3nB4ZYx5zOibtS0\n",
                                                                      "dN/EqKU7djyvnwM6fTO5gs07cDu1uToV8lBjhH9EHJu8KJJ4vPXFNgyK30XPx1Fd\n",
                                                                      "itTVGQId1kGwkuBmBBwbTd5uJiLFBwiJs5Vl/sUj1OHB6fp0pqzJ1M+WlNR3sYM2\n",
                                                                      "i68/S4sQsqy8ui74d60lNkuFrZzYpB7NRVVKesHOSdGQeYqchGn6c33kI67fvF5a\n",
                                                                      "Ra0DThYgIhij18nkpwaYHg==\n",
                                                                      "-----END CERTIFICATE-----\n\n"
                                                                  )
                                                              ),
                                           CustomData:        null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(CertificateStatus.Accepted,     response1.Status);

                Assert.AreEqual(1,                              installCertificateRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   installCertificateRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetInstalledCertificateIds_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task GetInstalledCertificateIds_Test()
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

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.InstallCertificate(
                                           ChargeBoxId:       chargingStation1.ChargeBoxId,
                                           CertificateType:   CertificateUse.V2GRootCertificate,
                                           Certificate:       Certificate.Parse(
                                                                  String.Concat(
                                                                      "-----BEGIN CERTIFICATE-----\n",
                                                                      "MIIFfDCCBGSgAwIBAgISAxm1F16JrzgdEDxpDfnyG2xaMA0GCSqGSIb3DQEBCwUA\n",
                                                                      "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                                                      "EwJSMzAeFw0yMzAxMDYwNDAwMjZaFw0yMzA0MDYwNDAwMjVaMCIxIDAeBgNVBAMT\n",
                                                                      "F2phYmJlci5ncmFwaGRlZmluZWQuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\n",
                                                                      "MIIBCgKCAQEAtucIqzk30QB90mZxCNO+XP2kiY9QMFIsTfupU5IYrqGcQ1Zn+mYa\n",
                                                                      "7yMW9UDZdJeMpi0Ls3bOOY6HbktNTglIETUD3/hUxtLlSIQXgPV/r7qPmx5+rNgT\n",
                                                                      "H1uoCJ81Mk/vtGr0hWj/bbEv/FGRLo8KKr10ZZ/PNOs5JA/2SKolGGqst6Xd3Eh5\n",
                                                                      "JPqSwOeCPv/2D6rWvdEJwsbHBBgXBvdtb4NzGibz/y4VyiPcDZbw1P+F4MucvVEg\n",
                                                                      "cvFxCoupsolLcX/f49uq3FRgYGloPOAjCkHbbi8HCt0VfL0OKL4ooLtzAtm2VOJA\n",
                                                                      "ZueprlXzEVES9RR9jfkB5OpE1PMFc4oSEQIDAQABo4ICmjCCApYwDgYDVR0PAQH/\n",
                                                                      "BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAMBgNVHRMBAf8E\n",
                                                                      "AjAAMB0GA1UdDgQWBBTRSR2BPdSRXb+ifMhxcHkS+Dn9uTAfBgNVHSMEGDAWgBQU\n",
                                                                      "LrMXt1hWy65QCUDmH6+dixTCxjBVBggrBgEFBQcBAQRJMEcwIQYIKwYBBQUHMAGG\n",
                                                                      "FWh0dHA6Ly9yMy5vLmxlbmNyLm9yZzAiBggrBgEFBQcwAoYWaHR0cDovL3IzLmku\n",
                                                                      "bGVuY3Iub3JnLzBqBgNVHREEYzBhghtjb25mZXJlbmNlLmdyYXBoZGVmaW5lZC5j\n",
                                                                      "b22CEGdyYXBoZGVmaW5lZC5jb22CF2phYmJlci5ncmFwaGRlZmluZWQuY29tghdw\n",
                                                                      "dWJzdWIuZ3JhcGhkZWZpbmVkLmNvbTBMBgNVHSAERTBDMAgGBmeBDAECATA3Bgsr\n",
                                                                      "BgEEAYLfEwEBATAoMCYGCCsGAQUFBwIBFhpodHRwOi8vY3BzLmxldHNlbmNyeXB0\n",
                                                                      "Lm9yZzCCAQQGCisGAQQB1nkCBAIEgfUEgfIA8AB2AHoyjFTYty22IOo44FIe6YQW\n",
                                                                      "cDIThU070ivBOlejUutSAAABhYVzpcAAAAQDAEcwRQIhAJCxbUKgpq153bfWcnMv\n",
                                                                      "4yrKTyqtYBttKHxtw+nWMPQ5AiAmwa2yn/7794mQS3dh2hI79p/hC8p8XKn4jx6j\n",
                                                                      "ZscOngB2AOg+0No+9QY1MudXKLyJa8kD08vREWvs62nhd31tBr1uAAABhYVzpaAA\n",
                                                                      "AAQDAEcwRQIhAORY8NM3uxbxTSECXlWNazCywl3Q0G7iAHBOXIqTzJ2iAiAgEkJ4\n",
                                                                      "14UlG3TnHRgITx3wRXQsY0A95z7wa7YR3nkdWTANBgkqhkiG9w0BAQsFAAOCAQEA\n",
                                                                      "bwnRFC0EiAs/32J48Ifnt6/hDjqmd5ATo1pCdhy4YIf72EKoPAnZ/kOtaNP5hD8U\n",
                                                                      "CHVPQqYTaPE6bAPKs4JJOVIRdUJOTBHeYEHSD6iJHL93zWEKP3nB4ZYx5zOibtS0\n",
                                                                      "dN/EqKU7djyvnwM6fTO5gs07cDu1uToV8lBjhH9EHJu8KJJ4vPXFNgyK30XPx1Fd\n",
                                                                      "itTVGQId1kGwkuBmBBwbTd5uJiLFBwiJs5Vl/sUj1OHB6fp0pqzJ1M+WlNR3sYM2\n",
                                                                      "i68/S4sQsqy8ui74d60lNkuFrZzYpB7NRVVKesHOSdGQeYqchGn6c33kI67fvF5a\n",
                                                                      "Ra0DThYgIhij18nkpwaYHg==\n",
                                                                      "-----END CERTIFICATE-----\n\n"
                                                                  )
                                                              ),
                                           CustomData:        null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(CertificateStatus.Accepted,     response1.Status);

                Assert.AreEqual(1,                              installCertificateRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   installCertificateRequests.First().ChargeBoxId);


                await Task.Delay(500);


                var getInstalledCertificateIdsRequests = new ConcurrentList<GetInstalledCertificateIdsRequest>();

                chargingStation1.OnGetInstalledCertificateIdsRequest += (timestamp, sender, getInstalledCertificateIdsRequest) => {
                    getInstalledCertificateIdsRequests.TryAdd(getInstalledCertificateIdsRequest);
                    return Task.CompletedTask;
                };

                var response2  = await testCSMS01.GetInstalledCertificateIds(
                                           ChargeBoxId:        chargingStation1.ChargeBoxId,
                                           CertificateTypes:   new[] {
                                                                   CertificateUse.V2GRootCertificate
                                                               },
                                           CustomData:         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                           response2.Result.ResultCode);
                Assert.AreEqual(GetInstalledCertificateStatus.Accepted,   response2.Status);

                Assert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,             getInstalledCertificateIdsRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region DeleteCertificate_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task DeleteCertificate_Test()
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

                #region 1. Install Certificate

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.InstallCertificate(
                                           ChargeBoxId:       chargingStation1.ChargeBoxId,
                                           CertificateType:   CertificateUse.V2GRootCertificate,
                                           Certificate:       Certificate.Parse(
                                                                  String.Concat(
                                                                      "-----BEGIN CERTIFICATE-----\n",
                                                                      "MIIFfDCCBGSgAwIBAgISAxm1F16JrzgdEDxpDfnyG2xaMA0GCSqGSIb3DQEBCwUA\n",
                                                                      "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                                                      "EwJSMzAeFw0yMzAxMDYwNDAwMjZaFw0yMzA0MDYwNDAwMjVaMCIxIDAeBgNVBAMT\n",
                                                                      "F2phYmJlci5ncmFwaGRlZmluZWQuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\n",
                                                                      "MIIBCgKCAQEAtucIqzk30QB90mZxCNO+XP2kiY9QMFIsTfupU5IYrqGcQ1Zn+mYa\n",
                                                                      "7yMW9UDZdJeMpi0Ls3bOOY6HbktNTglIETUD3/hUxtLlSIQXgPV/r7qPmx5+rNgT\n",
                                                                      "H1uoCJ81Mk/vtGr0hWj/bbEv/FGRLo8KKr10ZZ/PNOs5JA/2SKolGGqst6Xd3Eh5\n",
                                                                      "JPqSwOeCPv/2D6rWvdEJwsbHBBgXBvdtb4NzGibz/y4VyiPcDZbw1P+F4MucvVEg\n",
                                                                      "cvFxCoupsolLcX/f49uq3FRgYGloPOAjCkHbbi8HCt0VfL0OKL4ooLtzAtm2VOJA\n",
                                                                      "ZueprlXzEVES9RR9jfkB5OpE1PMFc4oSEQIDAQABo4ICmjCCApYwDgYDVR0PAQH/\n",
                                                                      "BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAMBgNVHRMBAf8E\n",
                                                                      "AjAAMB0GA1UdDgQWBBTRSR2BPdSRXb+ifMhxcHkS+Dn9uTAfBgNVHSMEGDAWgBQU\n",
                                                                      "LrMXt1hWy65QCUDmH6+dixTCxjBVBggrBgEFBQcBAQRJMEcwIQYIKwYBBQUHMAGG\n",
                                                                      "FWh0dHA6Ly9yMy5vLmxlbmNyLm9yZzAiBggrBgEFBQcwAoYWaHR0cDovL3IzLmku\n",
                                                                      "bGVuY3Iub3JnLzBqBgNVHREEYzBhghtjb25mZXJlbmNlLmdyYXBoZGVmaW5lZC5j\n",
                                                                      "b22CEGdyYXBoZGVmaW5lZC5jb22CF2phYmJlci5ncmFwaGRlZmluZWQuY29tghdw\n",
                                                                      "dWJzdWIuZ3JhcGhkZWZpbmVkLmNvbTBMBgNVHSAERTBDMAgGBmeBDAECATA3Bgsr\n",
                                                                      "BgEEAYLfEwEBATAoMCYGCCsGAQUFBwIBFhpodHRwOi8vY3BzLmxldHNlbmNyeXB0\n",
                                                                      "Lm9yZzCCAQQGCisGAQQB1nkCBAIEgfUEgfIA8AB2AHoyjFTYty22IOo44FIe6YQW\n",
                                                                      "cDIThU070ivBOlejUutSAAABhYVzpcAAAAQDAEcwRQIhAJCxbUKgpq153bfWcnMv\n",
                                                                      "4yrKTyqtYBttKHxtw+nWMPQ5AiAmwa2yn/7794mQS3dh2hI79p/hC8p8XKn4jx6j\n",
                                                                      "ZscOngB2AOg+0No+9QY1MudXKLyJa8kD08vREWvs62nhd31tBr1uAAABhYVzpaAA\n",
                                                                      "AAQDAEcwRQIhAORY8NM3uxbxTSECXlWNazCywl3Q0G7iAHBOXIqTzJ2iAiAgEkJ4\n",
                                                                      "14UlG3TnHRgITx3wRXQsY0A95z7wa7YR3nkdWTANBgkqhkiG9w0BAQsFAAOCAQEA\n",
                                                                      "bwnRFC0EiAs/32J48Ifnt6/hDjqmd5ATo1pCdhy4YIf72EKoPAnZ/kOtaNP5hD8U\n",
                                                                      "CHVPQqYTaPE6bAPKs4JJOVIRdUJOTBHeYEHSD6iJHL93zWEKP3nB4ZYx5zOibtS0\n",
                                                                      "dN/EqKU7djyvnwM6fTO5gs07cDu1uToV8lBjhH9EHJu8KJJ4vPXFNgyK30XPx1Fd\n",
                                                                      "itTVGQId1kGwkuBmBBwbTd5uJiLFBwiJs5Vl/sUj1OHB6fp0pqzJ1M+WlNR3sYM2\n",
                                                                      "i68/S4sQsqy8ui74d60lNkuFrZzYpB7NRVVKesHOSdGQeYqchGn6c33kI67fvF5a\n",
                                                                      "Ra0DThYgIhij18nkpwaYHg==\n",
                                                                      "-----END CERTIFICATE-----\n\n"
                                                                  )
                                                              ),
                                           CustomData:        null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(CertificateStatus.Accepted,     response1.Status);

                Assert.AreEqual(1,                              installCertificateRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   installCertificateRequests.First().ChargeBoxId);


                await Task.Delay(500);

                #endregion

                #region 2. Get installed certificate identifications

                var getInstalledCertificateIdsRequests = new ConcurrentList<GetInstalledCertificateIdsRequest>();

                chargingStation1.OnGetInstalledCertificateIdsRequest += (timestamp, sender, getInstalledCertificateIdsRequest) => {
                    getInstalledCertificateIdsRequests.TryAdd(getInstalledCertificateIdsRequest);
                    return Task.CompletedTask;
                };

                var response2  = await testCSMS01.GetInstalledCertificateIds(
                                           ChargeBoxId:        chargingStation1.ChargeBoxId,
                                           CertificateTypes:   new[] {
                                                                   CertificateUse.V2GRootCertificate
                                                               },
                                           CustomData:         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                           response2.Result.ResultCode);
                Assert.AreEqual(GetInstalledCertificateStatus.Accepted,   response2.Status);
                Assert.AreEqual(1,                                        response2.CertificateHashDataChain.Count());

                Assert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,             getInstalledCertificateIdsRequests.First().ChargeBoxId);


                await Task.Delay(500);

                #endregion

                #region 3. Delete certificate

                var deleteCertificateRequests = new ConcurrentList<DeleteCertificateRequest>();

                chargingStation1.OnDeleteCertificateRequest += (timestamp, sender, deleteCertificateRequest) => {
                    deleteCertificateRequests.TryAdd(deleteCertificateRequest);
                    return Task.CompletedTask;
                };

                var response3  = await testCSMS01.DeleteCertificate(
                                           ChargeBoxId:           chargingStation1.ChargeBoxId,
                                           CertificateHashData:   response2.CertificateHashDataChain.First(),
                                           CustomData:            null
                                       );


                Assert.AreEqual(ResultCodes.OK,                     response3.Result.ResultCode);
                Assert.AreEqual(DeleteCertificateStatus.Accepted,   response3.Status);

                Assert.AreEqual(1,                                  deleteCertificateRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,       deleteCertificateRequests.First().ChargeBoxId);


                // Verification
                getInstalledCertificateIdsRequests.Clear();

                var response4  = await testCSMS01.GetInstalledCertificateIds(
                                           ChargeBoxId:        chargingStation1.ChargeBoxId,
                                           CertificateTypes:   new[] {
                                                                   CertificateUse.V2GRootCertificate
                                                               },
                                           CustomData:         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                           response4.Result.ResultCode);
                Assert.AreEqual(GetInstalledCertificateStatus.Accepted,   response4.Status);
                Assert.AreEqual(0,                                        response4.CertificateHashDataChain.Count());

                Assert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,             getInstalledCertificateIdsRequests.First().ChargeBoxId);


                await Task.Delay(500);

                #endregion

            }

        }

        #endregion

        #region NotifyCRL_Test()

        /// <summary>
        /// A test for notifing a charging station, that a certificate revocation list
        /// for a given certificate is available for download.
        /// </summary>
        [Test]
        public async Task NotifyCRL_Test()
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

                var notifyCRLRequests = new ConcurrentList<NotifyCRLRequest>();

                chargingStation1.OnNotifyCRLRequest += (timestamp, sender, notifyCRLRequest) => {
                    notifyCRLRequests.TryAdd(notifyCRLRequest);
                    return Task.CompletedTask;
                };

                var response3  = await testCSMS01.NotifyCRLAvailability(
                                           ChargeBoxId:          chargingStation1.ChargeBoxId,
                                           NotifyCRLRequestId:   1,
                                           Availability:         NotifyCRLStatus.Available,
                                           Location:             URL.Parse("https://localhost/clr.json"),
                                           CustomData:           null
                                       );;


                Assert.AreEqual(ResultCodes.OK,                 response3.Result.ResultCode);

                Assert.AreEqual(1,                              notifyCRLRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   notifyCRLRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region GetLocalListVersion_Test()

        /// <summary>
        /// A test for getting the local list of a charging station.
        /// </summary>
        [Test]
        public async Task GetLocalListVersion_Test()
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

                var getLocalListVersionRequests = new ConcurrentList<GetLocalListVersionRequest>();

                chargingStation1.OnGetLocalListVersionRequest += (timestamp, sender, getLocalListVersionRequest) => {
                    getLocalListVersionRequests.TryAdd(getLocalListVersionRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.GetLocalListVersion(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           CustomData:    null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              getLocalListVersionRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getLocalListVersionRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region SendLocalList_Test()

        /// <summary>
        /// A test for sending a local list to a charging station.
        /// </summary>
        [Test]
        public async Task SendLocalList_Test()
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

                var sendLocalListRequests = new ConcurrentList<SendLocalListRequest>();

                chargingStation1.OnSendLocalListRequest += (timestamp, sender, sendLocalListRequest) => {
                    sendLocalListRequests.TryAdd(sendLocalListRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.SendLocalList(
                                           ChargeBoxId:              chargingStation1.ChargeBoxId,
                                           ListVersion:              1,
                                           UpdateType:               UpdateTypes.Full,
                                           LocalAuthorizationList:   new[] {
                                                                         new AuthorizationData(
                                                                             IdToken:       new IdToken(
                                                                                                Value:                 "aabbccdd",
                                                                                                Type:                  IdTokenTypes.ISO14443,
                                                                                                AdditionalInfos:       new[] {
                                                                                                                           new AdditionalInfo(
                                                                                                                               AdditionalIdToken:   "1234",
                                                                                                                               Type:                "PIN",
                                                                                                                               CustomData:          null
                                                                                                                           )
                                                                                                                       },
                                                                                                CustomData:            null
                                                                                            ),
                                                                             IdTokenInfo:   new IdTokenInfo(
                                                                                                Status:                AuthorizationStatus.Accepted,
                                                                                                ChargingPriority:      8,
                                                                                                CacheExpiryDateTime:   Timestamp.Now + TimeSpan.FromDays(3),
                                                                                                ValidEVSEIds:          new[] {
                                                                                                                           EVSE_Id.Parse(1)
                                                                                                                       },
                                                                                                GroupIdToken:          new IdToken(
                                                                                                                           Value:                 "55667788",
                                                                                                                           Type:                  IdTokenTypes.ISO14443,
                                                                                                                           AdditionalInfos:       new[] {
                                                                                                                                                      new AdditionalInfo(
                                                                                                                                                          AdditionalIdToken:   "1234",
                                                                                                                                                          Type:                "PIN",
                                                                                                                                                          CustomData:          null
                                                                                                                                                      )
                                                                                                                                                  },
                                                                                                                           CustomData:            null
                                                                                                                       ),
                                                                                                Language1:             Language_Id.Parse("de"),
                                                                                                Language2:             Language_Id.Parse("en"),
                                                                                                PersonalMessage:       new MessageContent(
                                                                                                                           Content:      "Hello world!",
                                                                                                                           Format:       MessageFormats.UTF8,
                                                                                                                           Language:     Language_Id.Parse("en"),
                                                                                                                           CustomData:   null
                                                                                                                       ),
                                                                                                CustomData:            null
                                                                                            ),
                                                                             CustomData:    null
                                                                         )
                                                                     },
                                           CustomData:               null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              sendLocalListRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   sendLocalListRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region ClearCache_Test()

        /// <summary>
        /// A test for clearing the authorization cache of a charging station.
        /// </summary>
        [Test]
        public async Task ClearCache_Test()
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

                var clearCacheRequests = new ConcurrentList<ClearCacheRequest>();

                chargingStation1.OnClearCacheRequest += (timestamp, sender, clearCacheRequest) => {
                    clearCacheRequests.TryAdd(clearCacheRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.ClearCache(
                                           ChargeBoxId:   chargingStation1.ChargeBoxId,
                                           CustomData:    null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              clearCacheRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   clearCacheRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region ReserveNow_Test()

        /// <summary>
        /// A test for creating a reservation at a charging station.
        /// </summary>
        [Test]
        public async Task ReserveNow_Test()
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

                var reserveNowRequests = new ConcurrentList<ReserveNowRequest>();

                chargingStation1.OnReserveNowRequest += (timestamp, sender, reserveNowRequest) => {
                    reserveNowRequests.TryAdd(reserveNowRequest);
                    return Task.CompletedTask;
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);
                //var connectorId     = Connector_Id.  Parse(1);
                var connectorType   = ConnectorTypes.sType2;

                var response1       = await testCSMS01.ReserveNow(
                                                ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                //ConnectorId:     connectorId,
                                                ReservationId:   reservationId,
                                                ExpiryDate:      Timestamp.Now + TimeSpan.FromHours(2),
                                                IdToken:         new IdToken(
                                                                     Value:             "22334455",
                                                                     Type:              IdTokenTypes.ISO14443,
                                                                     AdditionalInfos:   new[] {
                                                                                            new AdditionalInfo(
                                                                                                AdditionalIdToken:   "123",
                                                                                                Type:                "typetype",
                                                                                                CustomData:          null
                                                                                            )
                                                                                        },
                                                                     CustomData:        null
                                                                 ),
                                                ConnectorType:   connectorType,
                                                EVSEId:          evseId,
                                                GroupIdToken:    new IdToken(
                                                                     Value:             "55667788",
                                                                     Type:              IdTokenTypes.ISO14443,
                                                                     AdditionalInfos:   new[] {
                                                                                            new AdditionalInfo(
                                                                                                AdditionalIdToken:   "567",
                                                                                                Type:                "type2type2",
                                                                                                CustomData:          null
                                                                                            )
                                                                                        },
                                                                     CustomData:        null
                                                                 ),
                                                CustomData:      null
                                            );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ReservationStatus.Accepted,     response1.Status);

                Assert.AreEqual(1,                              reserveNowRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   reserveNowRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region CancelReservation_Test()

        /// <summary>
        /// A test for creating and deleting a reservation at a charging station.
        /// </summary>
        [Test]
        public async Task CancelReservation_Test()
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

                var reserveNowRequests = new ConcurrentList<ReserveNowRequest>();

                chargingStation1.OnReserveNowRequest += (timestamp, sender, reserveNowRequest) => {
                    reserveNowRequests.TryAdd(reserveNowRequest);
                    return Task.CompletedTask;
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);

                var response1       = await testCSMS01.CancelReservation(
                                                ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                ReservationId:   reservationId,
                                                CustomData:      null
                                            );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(ReservationStatus.Accepted,     response1.Status);

                Assert.AreEqual(1,                              reserveNowRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   reserveNowRequests.First().ChargeBoxId);


                await Task.Delay(500);


                var cancelReservationRequests = new ConcurrentList<CancelReservationRequest>();

                chargingStation1.OnCancelReservationRequest += (timestamp, sender, cancelReservationRequest) => {
                    cancelReservationRequests.TryAdd(cancelReservationRequest);
                    return Task.CompletedTask;
                };

                var response2       = await testCSMS01.CancelReservation(
                                                ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                ReservationId:   reservationId,
                                                CustomData:      null
                                            );


                Assert.AreEqual(ResultCodes.OK,                     response2.Result.ResultCode);
                Assert.AreEqual(CancelReservationStatus.Accepted,   response2.Status);

                Assert.AreEqual(1,                                  cancelReservationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,       cancelReservationRequests.First().ChargeBoxId);


            }

        }

        #endregion

        #region RequestStartStopTransaction_Test()

        /// <summary>
        /// A test starting and stopping a charging session/transaction.
        /// </summary>
        [Test]
        public async Task RequestStartStopTransaction_Test()
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

                var requestStartTransactionRequests  = new ConcurrentList<RequestStartTransactionRequest>();
                var requestStopTransactionRequests   = new ConcurrentList<RequestStopTransactionRequest>();

                chargingStation1.OnRequestStartTransactionRequest += (timestamp, sender, requestStartTransactionRequest) => {
                    requestStartTransactionRequests.TryAdd(requestStartTransactionRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OnRequestStopTransactionRequest  += (timestamp, sender, requestStopTransactionRequest) => {
                    requestStopTransactionRequests. TryAdd(requestStopTransactionRequest);
                    return Task.CompletedTask;
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
                                           GroupIdToken:                       new IdToken(
                                                                                   Value:             "cafebabe",
                                                                                   Type:              IdTokenTypes.ISO14443,
                                                                                   AdditionalInfos:   null,
                                                                                   CustomData:        null
                                                                               ),
                                           CustomData:                         null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 startResponse.Result.ResultCode);
                Assert.IsTrue  (startResponse.TransactionId.HasValue);

                Assert.AreEqual(1,                              requestStartTransactionRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   requestStartTransactionRequests.First().ChargeBoxId);

                await Task.Delay(500);


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

        #region GetTransactionStatus_Test()

        /// <summary>
        /// A test gettig the current status of a charging session/transaction.
        /// </summary>
        [Test]
        public async Task GetTransactionStatus_Test()
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

                var unlockConnectorRequests = new ConcurrentList<GetTransactionStatusRequest>();

                chargingStation1.OnGetTransactionStatusRequest += (timestamp, sender, unlockConnectorRequest) => {
                    unlockConnectorRequests.TryAdd(unlockConnectorRequest);
                    return Task.CompletedTask;
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

        #region SetChargingProfile_Test()

        /// <summary>
        /// A test setting a charging profile at a charging station.
        /// </summary>
        [Test]
        public async Task SetChargingProfile_Test()
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

                var setChargingProfileRequests = new ConcurrentList<SetChargingProfileRequest>();

                chargingStation1.OnSetChargingProfileRequest += (timestamp, sender, setChargingProfileRequest) => {
                    setChargingProfileRequests.TryAdd(setChargingProfileRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SetChargingProfile(
                                     ChargeBoxId:       chargingStation1.ChargeBoxId,
                                     EVSEId:            chargingStation1.EVSEs.First().Id,
                                     ChargingProfile:   new ChargingProfile(
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
                                                        ),
                                     CustomData:        null
                                 );


                Assert.AreEqual(ResultCodes.OK,                   response1.Result.ResultCode);
                Assert.AreEqual(ChargingProfileStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                setChargingProfileRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,     setChargingProfileRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetChargingProfiles_Test()

        /// <summary>
        /// A test requesting charging profiles from a charging station.
        /// </summary>
        [Test]
        public async Task GetChargingProfiles_Test()
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

                var getChargingProfilesRequests = new ConcurrentList<GetChargingProfilesRequest>();

                chargingStation1.OnGetChargingProfilesRequest += (timestamp, sender, getChargingProfilesRequest) => {
                    getChargingProfilesRequests.TryAdd(getChargingProfilesRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetChargingProfiles(
                                     ChargeBoxId:                    chargingStation1.ChargeBoxId,
                                     GetChargingProfilesRequestId:   1,
                                     ChargingProfile:                new ChargingProfileCriterion(
                                                                         ChargingProfilePurpose:   ChargingProfilePurposes.TxDefaultProfile,
                                                                         StackLevel:               1,
                                                                         ChargingProfileIds:       new[] {
                                                                                                       ChargingProfile_Id.Parse(123)
                                                                                                   },
                                                                         ChargingLimitSources:     new[] {
                                                                                                       ChargingLimitSources.SO
                                                                                                   },
                                                                         CustomData:               null
                                                                     ),
                                     EVSEId:                         EVSE_Id.Parse(1),
                                     CustomData:                     null
                                 );


                Assert.AreEqual(ResultCodes.OK,                      response1.Result.ResultCode);
                Assert.AreEqual(GetChargingProfileStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                   getChargingProfilesRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,        getChargingProfilesRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region ClearChargingProfile_Test()

        /// <summary>
        /// A test deleting a charging profile from a charging station.
        /// </summary>
        [Test]
        public async Task ClearChargingProfile_Test()
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

                var getChargingProfilesRequests = new ConcurrentList<ClearChargingProfileRequest>();

                chargingStation1.OnClearChargingProfileRequest += (timestamp, sender, getChargingProfilesRequest) => {
                    getChargingProfilesRequests.TryAdd(getChargingProfilesRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.ClearChargingProfile(
                                     ChargeBoxId:               chargingStation1.ChargeBoxId,
                                     ChargingProfileId:         ChargingProfile_Id.Parse(123),
                                     ChargingProfileCriteria:   new ClearChargingProfile(
                                                                    EVSEId:                   EVSE_Id.Parse(1),
                                                                    ChargingProfilePurpose:   ChargingProfilePurposes.TxDefaultProfile,
                                                                    StackLevel:               1,
                                                                    CustomData:               null
                                                                ),
                                     CustomData:                null
                                 );


                Assert.AreEqual(ResultCodes.OK,                        response1.Result.ResultCode);
                Assert.AreEqual(ClearChargingProfileStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                     getChargingProfilesRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,          getChargingProfilesRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region GetCompositeSchedule_Test()

        /// <summary>
        /// A test requesting the composite schedule from a charging station.
        /// </summary>
        [Test]
        public async Task GetCompositeSchedule_Test()
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

                var getCompositeScheduleRequests = new ConcurrentList<GetCompositeScheduleRequest>();

                chargingStation1.OnGetCompositeScheduleRequest += (timestamp, sender, getCompositeScheduleRequest) => {
                    getCompositeScheduleRequests.TryAdd(getCompositeScheduleRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.GetCompositeSchedule(
                                     ChargeBoxId:        chargingStation1.ChargeBoxId,
                                     Duration:           TimeSpan.FromSeconds(1),
                                     EVSEId:             EVSE_Id.Parse(1),
                                     ChargingRateUnit:   ChargingRateUnits.Watts,
                                     CustomData:         null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,         response1.Status);

                Assert.AreEqual(1,                              getCompositeScheduleRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   getCompositeScheduleRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region UpdateDynamicSchedule_Test()

        /// <summary>
        /// A test updating the dynamic charging schedule for the given charging profile.
        /// </summary>
        [Test]
        public async Task UpdateDynamicSchedule_Test()
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

                var updateDynamicScheduleRequests = new ConcurrentList<UpdateDynamicScheduleRequest>();

                chargingStation1.OnUpdateDynamicScheduleRequest += (timestamp, sender, updateDynamicScheduleRequest) => {
                    updateDynamicScheduleRequests.TryAdd(updateDynamicScheduleRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.UpdateDynamicSchedule(

                                     ChargeBoxId:           chargingStation1.ChargeBoxId,

                                     ChargingProfileId:     ChargingProfile_Id.Parse(1),

                                     Limit:                 ChargingRateValue. Parse( 1, ChargingRateUnits.Watts),
                                     Limit_L2:              ChargingRateValue. Parse( 2, ChargingRateUnits.Watts),
                                     Limit_L3:              ChargingRateValue. Parse( 3, ChargingRateUnits.Watts),

                                     DischargeLimit:        ChargingRateValue. Parse(-4, ChargingRateUnits.Watts),
                                     DischargeLimit_L2:     ChargingRateValue. Parse(-5, ChargingRateUnits.Watts),
                                     DischargeLimit_L3:     ChargingRateValue. Parse(-6, ChargingRateUnits.Watts),

                                     Setpoint:              ChargingRateValue. Parse( 7, ChargingRateUnits.Watts),
                                     Setpoint_L2:           ChargingRateValue. Parse( 8, ChargingRateUnits.Watts),
                                     Setpoint_L3:           ChargingRateValue. Parse( 9, ChargingRateUnits.Watts),

                                     SetpointReactive:      ChargingRateValue. Parse(10, ChargingRateUnits.Watts),
                                     SetpointReactive_L2:   ChargingRateValue. Parse(11, ChargingRateUnits.Watts),
                                     SetpointReactive_L3:   ChargingRateValue. Parse(12, ChargingRateUnits.Watts),

                                     CustomData:            null

                                 );


                Assert.AreEqual(ResultCodes.OK,                   response1.Result.ResultCode);
                Assert.AreEqual(ChargingProfileStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                updateDynamicScheduleRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,     updateDynamicScheduleRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region NotifyAllowedEnergyTransfer_Test()

        /// <summary>
        /// A test updating the list of authorized energy services.
        /// </summary>
        [Test]
        public async Task NotifyAllowedEnergyTransfer_Test()
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

                var unlockConnectorRequests = new ConcurrentList<NotifyAllowedEnergyTransferRequest>();

                chargingStation1.OnNotifyAllowedEnergyTransferRequest += (timestamp, sender, unlockConnectorRequest) => {
                    unlockConnectorRequests.TryAdd(unlockConnectorRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.NotifyAllowedEnergyTransfer(
                                     ChargeBoxId:                  chargingStation1.ChargeBoxId,
                                     AllowedEnergyTransferModes:   new[] {
                                                                       EnergyTransferModes.AC_SinglePhase,
                                                                       EnergyTransferModes.AC_ThreePhases
                                                                   },
                                     CustomData:                   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                               response1.Result.ResultCode);
                Assert.AreEqual(NotifyAllowedEnergyTransferStatus.Accepted,   response1.Status);

                Assert.AreEqual(1,                                            unlockConnectorRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,                 unlockConnectorRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region UsePriorityCharging_Test()

        /// <summary>
        /// A test switching to the priority charging profile.
        /// </summary>
        [Test]
        public async Task UsePriorityCharging_Test()
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

                var usePriorityChargingRequests = new ConcurrentList<UsePriorityChargingRequest>();

                chargingStation1.OnUsePriorityChargingRequest += (timestamp, sender, usePriorityChargingRequest) => {
                    usePriorityChargingRequests.TryAdd(usePriorityChargingRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.UsePriorityCharging(
                                     ChargeBoxId:     chargingStation1.ChargeBoxId,
                                     TransactionId:   Transaction_Id.Parse("1234"),
                                     Activate:        true,
                                     CustomData:      null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,         response1.Status);

                Assert.AreEqual(1,                              usePriorityChargingRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   usePriorityChargingRequests.First().ChargeBoxId);

            }

        }

        #endregion

        #region UnlockConnector_Test()

        /// <summary>
        /// A test unlocking an EVSE/connector at a charging station.
        /// </summary>
        [Test]
        public async Task UnlockConnector_Test()
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

                var unlockConnectorRequests = new ConcurrentList<UnlockConnectorRequest>();

                chargingStation1.OnUnlockConnectorRequest += (timestamp, sender, unlockConnectorRequest) => {
                    unlockConnectorRequests.TryAdd(unlockConnectorRequest);
                    return Task.CompletedTask;
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


        #region SendAFRRSignal_Test()

        /// <summary>
        /// A test sending an AFRR signal to a charging station.
        /// </summary>
        [Test]
        public async Task SendAFRRSignal_Test()
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

                var afrrSignalRequestRequests = new ConcurrentList<AFRRSignalRequest>();

                chargingStation1.OnAFRRSignalRequest += (timestamp, sender, afrrSignalRequestRequest) => {
                    afrrSignalRequestRequests.TryAdd(afrrSignalRequestRequest);
                    return Task.CompletedTask;
                };

                var response1  = await testCSMS01.SendAFRRSignal(
                                     ChargeBoxId:           chargingStation1.ChargeBoxId,
                                     ActivationTimestamp:   Timestamp.Now,
                                     Signal:                AFRR_Signal.Parse(-1),
                                     CustomData:            null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(GenericStatus.Accepted,         response1.Status);

                Assert.AreEqual(1,                              afrrSignalRequestRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   afrrSignalRequestRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region SetDisplayMessage_Test()

        /// <summary>
        /// A test setting the display message at a charging station.
        /// </summary>
        [Test]
        public async Task SetDisplayMessage_Test()
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

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OnSetDisplayMessageRequest += (timestamp, sender, setDisplayMessageRequest) => {
                    setDisplayMessageRequests.TryAdd(setDisplayMessageRequest);
                    return Task.CompletedTask;
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

        #region GetDisplayMessages_Test()

        /// <summary>
        /// A test getting the display messages from a charging station.
        /// </summary>
        [Test]
        public async Task GetDisplayMessages_Test()
        {

            if (testCSMS01                                            is not null &&
                testBackendWebSockets01                               is not null &&
                csmsWebSocketTextMessagesSent                         is not null &&
                csmsWebSocketTextMessageResponsesReceived             is not null &&
                csmsWebSocketTextMessagesReceived                     is not null &&
                csmsWebSocketTextMessageResponsesSent                 is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketTextMessagesReceived         is not null &&
                chargingStation1WebSocketTextMessageResponsesSent     is not null &&
                chargingStation1WebSocketTextMessagesSent             is not null &&
                chargingStation1WebSocketTextMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketTextMessagesReceived         is not null &&
                chargingStation2WebSocketTextMessageResponsesSent     is not null &&
                chargingStation2WebSocketTextMessagesSent             is not null &&
                chargingStation2WebSocketTextMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketTextMessagesReceived         is not null &&
                chargingStation3WebSocketTextMessageResponsesSent     is not null &&
                chargingStation3WebSocketTextMessagesSent             is not null &&
                chargingStation3WebSocketTextMessageResponsesReceived is not null)

            {

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OnSetDisplayMessageRequest += (timestamp, sender, setDisplayMessageRequest) => {
                    setDisplayMessageRequests.TryAdd(setDisplayMessageRequest);
                    return Task.CompletedTask;
                };

                var messageIds = new[] {
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

                for (var i = 1; i <= 10; i++) {

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


                Assert.AreEqual(10, csmsWebSocketTextMessagesSent.                        Count);
                Assert.AreEqual(10, csmsWebSocketTextMessageResponsesReceived.            Count);
                Assert.AreEqual(10, csmsWebSocketTextMessagesReceived.                    Count);
                Assert.AreEqual( 0, csmsWebSocketTextMessageResponsesSent.                Count);

                Assert.AreEqual(10, chargingStation1WebSocketTextMessagesReceived.        Count);
                Assert.AreEqual(10, chargingStation1WebSocketTextMessageResponsesSent.    Count);
                Assert.AreEqual(10, chargingStation1WebSocketTextMessagesSent.            Count);
                Assert.AreEqual( 0, chargingStation1WebSocketTextMessageResponsesReceived.Count);

                await Task.Delay(500);


                var getDisplayMessagesRequests     = new ConcurrentList<GetDisplayMessagesRequest>();
                var notifyDisplayMessagesRequests  = new ConcurrentList<CS.NotifyDisplayMessagesRequest>();

                chargingStation1.OnGetDisplayMessagesRequest += (timestamp, sender, getDisplayMessagesRequest) => {
                    getDisplayMessagesRequests.   TryAdd(getDisplayMessagesRequest);
                    return Task.CompletedTask;
                };

                testCSMS01.OnNotifyDisplayMessagesRequest    += (timestamp, sender, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
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

                await Task.Delay(500);


                Assert.AreEqual(12, csmsWebSocketTextMessagesSent.                        Count);
                Assert.AreEqual(11, csmsWebSocketTextMessageResponsesReceived.            Count);
                Assert.AreEqual(12, csmsWebSocketTextMessagesReceived.                    Count);
                Assert.AreEqual( 1, csmsWebSocketTextMessageResponsesSent.                Count);

                Assert.AreEqual(12, chargingStation1WebSocketTextMessagesReceived.        Count);
                Assert.AreEqual(11, chargingStation1WebSocketTextMessageResponsesSent.    Count);
                Assert.AreEqual(12, chargingStation1WebSocketTextMessagesSent.            Count);
                Assert.AreEqual( 1, chargingStation1WebSocketTextMessageResponsesReceived.Count);


                var getResponse2  = await testCSMS01.GetDisplayMessages(
                                              ChargeBoxId:                   chargingStation1.ChargeBoxId,
                                              GetDisplayMessagesRequestId:   2,
                                              Ids:                           new[] {
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

                await Task.Delay(500);


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

                await Task.Delay(500);


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

                Assert.AreEqual(10, notifyDisplayMessagesRequests[0].MessageInfos.Count());
                Assert.AreEqual( 3, notifyDisplayMessagesRequests[1].MessageInfos.Count());
                Assert.AreEqual( 5, notifyDisplayMessagesRequests[2].MessageInfos.Count());
                Assert.AreEqual( 3, notifyDisplayMessagesRequests[3].MessageInfos.Count());

            }

        }

        #endregion

        #region ClearDisplayMessage_Test()

        /// <summary>
        /// A test removing a display message from a charging station.
        /// </summary>
        [Test]
        public async Task ClearDisplayMessage_Test()
        {

            if (testCSMS01                                            is not null &&
                testBackendWebSockets01                               is not null &&
                csmsWebSocketTextMessagesSent                         is not null &&
                csmsWebSocketTextMessageResponsesReceived             is not null &&
                csmsWebSocketTextMessagesReceived                     is not null &&
                csmsWebSocketTextMessageResponsesSent                 is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketTextMessagesReceived         is not null &&
                chargingStation1WebSocketTextMessageResponsesSent     is not null &&
                chargingStation1WebSocketTextMessagesSent             is not null &&
                chargingStation1WebSocketTextMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketTextMessagesReceived         is not null &&
                chargingStation2WebSocketTextMessageResponsesSent     is not null &&
                chargingStation2WebSocketTextMessagesSent             is not null &&
                chargingStation2WebSocketTextMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketTextMessagesReceived         is not null &&
                chargingStation3WebSocketTextMessageResponsesSent     is not null &&
                chargingStation3WebSocketTextMessagesSent             is not null &&
                chargingStation3WebSocketTextMessageResponsesReceived is not null)

            {

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OnSetDisplayMessageRequest += (timestamp, sender, setDisplayMessageRequest) => {
                    setDisplayMessageRequests.TryAdd(setDisplayMessageRequest);
                    return Task.CompletedTask;
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
                var getDisplayMessagesRequests = new ConcurrentList<GetDisplayMessagesRequest>();

                chargingStation1.OnGetDisplayMessagesRequest += (timestamp, sender, getDisplayMessagesRequest) => {
                    getDisplayMessagesRequests.TryAdd(getDisplayMessagesRequest);
                    return Task.CompletedTask;
                };


                var notifyDisplayMessagesRequests = new ConcurrentList<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OnNotifyDisplayMessagesRequest += (timestamp, sender, notifyDisplayMessagesRequest) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
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


                await Task.Delay(500);


                // Delete message #1
                var clearDisplayMessageRequests = new ConcurrentList<ClearDisplayMessageRequest>();

                chargingStation1.OnClearDisplayMessageRequest += (timestamp, sender, clearDisplayMessageRequest) => {
                    clearDisplayMessageRequests.TryAdd(clearDisplayMessageRequest);
                    return Task.CompletedTask;
                };

                var clearResponse  = await testCSMS01.ClearDisplayMessage(
                                         ChargeBoxId:        chargingStation1.ChargeBoxId,
                                         DisplayMessageId:   messageId1,
                                         CustomData:         null
                                     );

                Assert.AreEqual(ResultCodes.OK,   clearResponse.Result.ResultCode);
                Assert.AreEqual(1,                clearDisplayMessageRequests.Count);


                await Task.Delay(500);


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


                Assert.AreEqual(2,                notifyDisplayMessagesRequests[0].MessageInfos.Count());
                Assert.AreEqual(1,                notifyDisplayMessagesRequests[1].MessageInfos.Count());


                Assert.AreEqual(7, csmsWebSocketTextMessagesSent.                        Count);
                Assert.AreEqual(5, csmsWebSocketTextMessageResponsesReceived.            Count);
                Assert.AreEqual(7, csmsWebSocketTextMessagesReceived.                    Count);
                Assert.AreEqual(2, csmsWebSocketTextMessageResponsesSent.                Count);

                Assert.AreEqual(7, chargingStation1WebSocketTextMessagesReceived.        Count);
                Assert.AreEqual(5, chargingStation1WebSocketTextMessageResponsesSent.    Count);
                Assert.AreEqual(7, chargingStation1WebSocketTextMessagesSent.            Count);
                Assert.AreEqual(2, chargingStation1WebSocketTextMessageResponsesReceived.Count);

            }

        }

        #endregion


        #region SendCostUpdate_Test()

        /// <summary>
        /// A test sending updated total costs.
        /// </summary>
        [Test]
        public async Task SendCostUpdate_Test()
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

                var costUpdatedRequests = new ConcurrentList<CostUpdatedRequest>();

                chargingStation1.OnCostUpdatedRequest += (timestamp, sender, costUpdatedRequest) => {
                    costUpdatedRequests.TryAdd(costUpdatedRequest);
                    return Task.CompletedTask;
                };

                var message    = RandomExtensions.RandomString(10);

                var response1  = await testCSMS01.SendCostUpdated(
                                     ChargeBoxId:     chargingStation1.ChargeBoxId,
                                     TotalCost:       1.02M,
                                     TransactionId:   Transaction_Id.NewRandom,
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

        #region RequestCustomerInformation_Test()

        /// <summary>
        /// A test for requesting customer information from a charging station.
        /// </summary>
        [Test]
        public async Task RequestCustomerInformation_Test()
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

                var customerInformationRequests        = new ConcurrentList<CustomerInformationRequest>();
                var notifyCustomerInformationRequests  = new ConcurrentList<CS.NotifyCustomerInformationRequest>();

                chargingStation1.OnCustomerInformationRequest += (timestamp, sender, customerInformationRequest) => {
                    customerInformationRequests.      TryAdd(customerInformationRequest);
                    return Task.CompletedTask;
                };

                testCSMS01.OnNotifyCustomerInformationRequest += (timestamp, sender, notifyCustomerInformationRequest) => {
                    notifyCustomerInformationRequests.TryAdd(notifyCustomerInformationRequest);
                    return Task.CompletedTask;
                };


                var response1  = await testCSMS01.RequestCustomerInformation(
                                           ChargeBoxId:                    chargingStation1.ChargeBoxId,
                                           CustomerInformationRequestId:   1,
                                           Report:                         true,
                                           Clear:                          false,
                                           CustomerIdentifier:             CustomerIdentifier.Parse("123"),
                                           IdToken:                        new IdToken(
                                                                               Value:                 "aabbccdd",
                                                                               Type:                  IdTokenTypes.ISO14443,
                                                                               AdditionalInfos:       new[] {
                                                                                                          new AdditionalInfo(
                                                                                                              AdditionalIdToken:   "1234",
                                                                                                              Type:                "PIN",
                                                                                                              CustomData:          null
                                                                                                          )
                                                                                                      },
                                                                               CustomData:            null
                                                                           ),
                                           CustomerCertificate:            new CertificateHashData(
                                                                               HashAlgorithm:         HashAlgorithms.SHA256,
                                                                               IssuerNameHash:        "f2311e9a995dfbd006bfc909e480987dc2d440ae6eaf1746efdadc638a295f65",
                                                                               IssuerPublicKeyHash:   "99084534bbe5f6ceaffa2e65ff1ad5301c4c359b599d6edd486a475071f715fb",
                                                                               SerialNumber:          "23",
                                                                               CustomData:            null
                                                                           ),
                                           CustomData:                     null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              customerInformationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   customerInformationRequests.First().ChargeBoxId);


                await Task.Delay(500);


                Assert.AreEqual(1,                              notifyCustomerInformationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   notifyCustomerInformationRequests.First().ChargeBoxId);

            }

        }

        #endregion


    }

}
