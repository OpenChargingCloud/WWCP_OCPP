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

using cloud.charging.open.protocols.OCPPv2_0.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{

    /// <summary>
    /// Unit tests for a central system sending messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMSMessagesTests : AChargingStationTests
    {

        #region CSMS_Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
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

        #region CSMS_Reset_UnknownChargeBox_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
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


        #region CSMS_UpdateFirmware_Test()

        /// <summary>
        /// A test for updating the firmware of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_UpdateFirmware_Test()
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

                var updateFirmwareRequests = new List<UpdateFirmwareRequest>();

                chargingStation1.OnUpdateFirmwareRequest += async (timestamp, sender, updateFirmwareRequest) => {
                    updateFirmwareRequests.Add(updateFirmwareRequest);
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

        #region CSMS_PublishFirmware_Test()

        /// <summary>
        /// A test for publishing a firmware update onto a charging station/local controller.
        /// </summary>
        [Test]
        public async Task CSMS_PublishFirmware_Test()
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

                var publishFirmwareRequests = new List<PublishFirmwareRequest>();

                chargingStation1.OnPublishFirmwareRequest += async (timestamp, sender, publishFirmwareRequest) => {
                    publishFirmwareRequests.Add(publishFirmwareRequest);
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

        #region CSMS_UnpublishFirmware_Test()

        /// <summary>
        /// A test for unpublishing a firmware update from a charging station/local controller.
        /// </summary>
        [Test]
        public async Task CSMS_UnpublishFirmware_Test()
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

                var unpublishFirmwareRequests = new List<UnpublishFirmwareRequest>();

                chargingStation1.OnUnpublishFirmwareRequest += async (timestamp, sender, unpublishFirmwareRequest) => {
                    unpublishFirmwareRequests.Add(unpublishFirmwareRequest);
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

        #region CSMS_GetBaseReport_Test()

        /// <summary>
        /// A test for getting a base report from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetBaseReport_Test()
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

                var getBaseReportRequests = new List<GetBaseReportRequest>();

                chargingStation1.OnGetBaseReportRequest += async (timestamp, sender, getBaseReportRequest) => {
                    getBaseReportRequests.Add(getBaseReportRequest);
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

        #region CSMS_GetReport_Test()

        /// <summary>
        /// A test for getting a report from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetReport_Test()
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

                var getReportRequests = new List<GetReportRequest>();

                chargingStation1.OnGetReportRequest += async (timestamp, sender, getReportRequest) => {
                    getReportRequests.Add(getReportRequest);
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

        #region CSMS_GetLog_Test()

        /// <summary>
        /// A test for getting a log file from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetLog_Test()
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

                var getLogRequests = new List<GetLogRequest>();

                chargingStation1.OnGetLogRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_SetVariables_Test()

        /// <summary>
        /// A test for setting variables of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetVariables_Test()
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

                var getLogRequests = new List<SetVariablesRequest>();

                chargingStation1.OnSetVariablesRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_GetVariables_Test()

        /// <summary>
        /// A test for getting variables of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetVariables_Test()
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

                var getLogRequests = new List<GetVariablesRequest>();

                chargingStation1.OnGetVariablesRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_SetMonitoringBase_Test()

        /// <summary>
        /// A test for setting the monitoring base of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetMonitoringBase_Test()
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

                var getLogRequests = new List<SetMonitoringBaseRequest>();

                chargingStation1.OnSetMonitoringBaseRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_GetMonitoringReport_Test()

        /// <summary>
        /// A test for setting the monitoring base of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetMonitoringReport_Test()
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

                var getLogRequests = new List<GetMonitoringReportRequest>();

                chargingStation1.OnGetMonitoringReportRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_SetMonitoringLevel_Test()

        /// <summary>
        /// A test for setting the monitoring level of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetMonitoringLevel_Test()
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

                var getLogRequests = new List<SetMonitoringLevelRequest>();

                chargingStation1.OnSetMonitoringLevelRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_SetVariableMonitoring_Test()

        /// <summary>
        /// A test for creating a variable monitoring at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetVariableMonitoring_Test()
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

                var getLogRequests = new List<SetVariableMonitoringRequest>();

                chargingStation1.OnSetVariableMonitoringRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_ClearVariableMonitoring_Test()

        /// <summary>
        /// A test for deleting a variable monitoring from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ClearVariableMonitoring_Test()
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

                var getLogRequests = new List<ClearVariableMonitoringRequest>();

                chargingStation1.OnClearVariableMonitoringRequest += async (timestamp, sender, getLogRequest) => {
                    getLogRequests.Add(getLogRequest);
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

        #region CSMS_SetNetworkProfile_Test()

        /// <summary>
        /// A test for setting the network profile of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetNetworkProfile_Test()
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

                var setNetworkProfileRequests = new List<SetNetworkProfileRequest>();

                chargingStation1.OnSetNetworkProfileRequest += async (timestamp, sender, setNetworkProfileRequest) => {
                    setNetworkProfileRequests.Add(setNetworkProfileRequest);
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

        #region CSMS_ChangeAvailability_Test()

        /// <summary>
        /// A test for sending a change availability message to a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ChangeAvailability_Test()
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

                var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

                chargingStation1.OnChangeAvailabilityRequest += async (timestamp, sender, changeAvailabilityRequest) => {
                    changeAvailabilityRequests.Add(changeAvailabilityRequest);
                };

                var evseId             = EVSE_Id.     Parse(1);
                var connectorId        = Connector_Id.Parse(1);
                var operationalStatus  = OperationalStatus.Operative;

                var response1     = await testCSMS01.ChangeAvailability(
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

        #region CSMS_TriggerMessage_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_TriggerMessage_Test()
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

                var triggerMessageRequests = new List<TriggerMessageRequest>();

                chargingStation1.OnTriggerMessageRequest += async (timestamp, sender, triggerMessageRequest) => {
                    triggerMessageRequests.Add(triggerMessageRequest);
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
        /// A test for sending data to a charging station.
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


        #region CSMS_CertificateSigned_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_CertificateSigned_Test()
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

                var certificateSignedRequests = new List<CertificateSignedRequest>();

                chargingStation1.OnCertificateSignedRequest += async (timestamp, sender, certificateSignedRequest) => {
                    certificateSignedRequests.Add(certificateSignedRequest);
                };


                var response1  = await testCSMS01.CertificateSigned(
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

        #region CSMS_InstallCertificate_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_InstallCertificate_Test()
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

                var installCertificateRequests = new List<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += async (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.Add(installCertificateRequest);
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

        #region CSMS_GetInstalledCertificateIds_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetInstalledCertificateIds_Test()
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

                var installCertificateRequests = new List<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += async (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.Add(installCertificateRequest);
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


                var getInstalledCertificateIdsRequests = new List<GetInstalledCertificateIdsRequest>();

                chargingStation1.OnGetInstalledCertificateIdsRequest += async (timestamp, sender, getInstalledCertificateIdsRequest) => {
                    getInstalledCertificateIdsRequests.Add(getInstalledCertificateIdsRequest);
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

        #region CSMS_DeleteCertificate_Test()

        /// <summary>
        /// A test for triggering a message at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_DeleteCertificate_Test()
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

                var installCertificateRequests = new List<InstallCertificateRequest>();

                chargingStation1.OnInstallCertificateRequest += async (timestamp, sender, installCertificateRequest) => {
                    installCertificateRequests.Add(installCertificateRequest);
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


                var getInstalledCertificateIdsRequests = new List<GetInstalledCertificateIdsRequest>();

                chargingStation1.OnGetInstalledCertificateIdsRequest += async (timestamp, sender, getInstalledCertificateIdsRequest) => {
                    getInstalledCertificateIdsRequests.Add(getInstalledCertificateIdsRequest);
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


                var deleteCertificateRequests = new List<DeleteCertificateRequest>();

                chargingStation1.OnDeleteCertificateRequest += async (timestamp, sender, deleteCertificateRequest) => {
                    deleteCertificateRequests.Add(deleteCertificateRequest);
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


            }

        }

        #endregion


        #region CSMS_GetLocalListVersion_Test()

        /// <summary>
        /// A test for getting the local list of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetLocalListVersion_Test()
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

                var getLocalListVersionRequests = new List<GetLocalListVersionRequest>();

                chargingStation1.OnGetLocalListVersionRequest += async (timestamp, sender, getLocalListVersionRequest) => {
                    getLocalListVersionRequests.Add(getLocalListVersionRequest);
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

        #region CSMS_SendLocalList_Test()

        /// <summary>
        /// A test for sending a local list to a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SendLocalList_Test()
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

                var sendLocalListRequests = new List<SendLocalListRequest>();

                chargingStation1.OnSendLocalListRequest += async (timestamp, sender, sendLocalListRequest) => {
                    sendLocalListRequests.Add(sendLocalListRequest);
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

        #region CSMS_ClearCache_Test()

        /// <summary>
        /// A test for clearing the authorization cache of a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ClearCache_Test()
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

                var clearCacheRequests = new List<ClearCacheRequest>();

                chargingStation1.OnClearCacheRequest += async (timestamp, sender, clearCacheRequest) => {
                    clearCacheRequests.Add(clearCacheRequest);
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


        #region CSMS_ReserveNow_Test()

        /// <summary>
        /// A test for creating a reservation at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ReserveNow_Test()
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

                var reserveNowRequests = new List<ReserveNowRequest>();

                chargingStation1.OnReserveNowRequest += async (timestamp, sender, reserveNowRequest) => {
                    reserveNowRequests.Add(reserveNowRequest);
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);
                var connectorId     = Connector_Id.  Parse(1);
                var connectorType   = ConnectorTypes.sType2;

                var response1       = await testCSMS01.ReserveNow(
                                                ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                ConnectorId:     connectorId,
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

        #region CSMS_CancelReservation_Test()

        /// <summary>
        /// A test for creating and deleting a reservation at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_CancelReservation_Test()
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

                var reserveNowRequests = new List<ReserveNowRequest>();

                chargingStation1.OnReserveNowRequest += async (timestamp, sender, reserveNowRequest) => {
                    reserveNowRequests.Add(reserveNowRequest);
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);
                var connectorId     = Connector_Id.  Parse(1);
                var connectorType   = ConnectorTypes.sType2;

                var response1       = await testCSMS01.ReserveNow(
                                                ChargeBoxId:     chargingStation1.ChargeBoxId,
                                                ConnectorId:     connectorId,
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


                await Task.Delay(500);


                var cancelReservationRequests = new List<CancelReservationRequest>();

                chargingStation1.OnCancelReservationRequest += async (timestamp, sender, cancelReservationRequest) => {
                    cancelReservationRequests.Add(cancelReservationRequest);
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

        #region CSMS_SetChargingProfile_Test()

        /// <summary>
        /// A test setting a charging profile at a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SetChargingProfile_Test()
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

                var setChargingProfileRequests = new List<SetChargingProfileRequest>();

                chargingStation1.OnSetChargingProfileRequest += async (timestamp, sender, setChargingProfileRequest) => {
                    setChargingProfileRequests.Add(setChargingProfileRequest);
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

        #region CSMS_GetChargingProfiles_Test()

        /// <summary>
        /// A test requesting charging profiles from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetChargingProfiles_Test()
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

                var getChargingProfilesRequests = new List<GetChargingProfilesRequest>();

                chargingStation1.OnGetChargingProfilesRequest += async (timestamp, sender, getChargingProfilesRequest) => {
                    getChargingProfilesRequests.Add(getChargingProfilesRequest);
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

        #region CSMS_ClearChargingProfile_Test()

        /// <summary>
        /// A test deleting a charging profile from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_ClearChargingProfile_Test()
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

                var getChargingProfilesRequests = new List<ClearChargingProfileRequest>();

                chargingStation1.OnClearChargingProfileRequest += async (timestamp, sender, getChargingProfilesRequest) => {
                    getChargingProfilesRequests.Add(getChargingProfilesRequest);
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

        #region CSMS_GetCompositeSchedule_Test()

        /// <summary>
        /// A test requesting the composite schedule from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_GetCompositeSchedule_Test()
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

                var getCompositeScheduleRequests = new List<GetCompositeScheduleRequest>();

                chargingStation1.OnGetCompositeScheduleRequest += async (timestamp, sender, getCompositeScheduleRequest) => {
                    getCompositeScheduleRequests.Add(getCompositeScheduleRequest);
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


                Assert.AreEqual(10, csmsWebSocketTextMessagesSent.                        Count);
                Assert.AreEqual(10, csmsWebSocketTextMessageResponsesReceived.            Count);
                Assert.AreEqual(10, csmsWebSocketTextMessagesReceived.                    Count);
                Assert.AreEqual( 0, csmsWebSocketTextMessageResponsesSent.                Count);

                Assert.AreEqual(10, chargingStation1WebSocketTextMessagesReceived.        Count);
                Assert.AreEqual(10, chargingStation1WebSocketTextMessageResponsesSent.    Count);
                Assert.AreEqual(10, chargingStation1WebSocketTextMessagesSent.            Count);
                Assert.AreEqual( 0, chargingStation1WebSocketTextMessageResponsesReceived.Count);


                await Task.Delay(500);


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


                Assert.AreEqual(7, csmsWebSocketTextMessagesReceived.                 Count);
                Assert.AreEqual(6, chargingStation1WebSocketTextMessagesReceived.Count);
                Assert.AreEqual(7, chargingStation1WebSocketTextMessagesSent.Count);
                Assert.AreEqual(1, csmsWebSocketTextMessageResponsesSent.                Count);

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

        #region CSMS_RequestCustomerInformation_Test()

        /// <summary>
        /// A test for requesting customer information from a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_RequestCustomerInformation_Test()
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

                var clearCacheRequests = new List<CustomerInformationRequest>();

                chargingStation1.OnCustomerInformationRequest += async (timestamp, sender, clearCacheRequest) => {
                    clearCacheRequests.Add(clearCacheRequest);
                };

                var notifyCustomerInformationRequests = new List<CS.NotifyCustomerInformationRequest>();

                testCSMS01.OnNotifyCustomerInformationRequest += async (timestamp, sender, notifyCustomerInformationRequest) => {
                    notifyCustomerInformationRequests.Add(notifyCustomerInformationRequest);
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
                                                                               IssuerNameHash:        "-",
                                                                               IssuerPublicKeyHash:   "-",
                                                                               SerialNumber:          "-",
                                                                               CustomData:            null
                                                                           ),
                                           CustomData:                     null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              clearCacheRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   clearCacheRequests.First().ChargeBoxId);


                await Task.Delay(500);


                Assert.AreEqual(1,                              notifyCustomerInformationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   notifyCustomerInformationRequests.First().ChargeBoxId);

            }

        }

        #endregion


    }

}
