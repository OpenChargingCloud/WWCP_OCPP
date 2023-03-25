﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Net.NetworkInformation;
using org.GraphDefined.Vanaheimr.Hermod.Modbus;
using org.GraphDefined.Vanaheimr.Illias.ConsoleLog;
using social.OpenData.UsersAPI;

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
                Assert.AreEqual(evseId,                              changeAvailabilityRequests.First().EVSE!.Id);
                Assert.AreEqual(connectorId,                         changeAvailabilityRequests.First().EVSE!.ConnectorId);
                Assert.AreEqual(operationalStatus,                   changeAvailabilityRequests.First().OperationalStatus);

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


    }

}
