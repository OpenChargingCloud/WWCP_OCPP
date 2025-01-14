﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a CSMS sending messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_Messages_Tests : AChargingStationTests
    {

        #region Reset_ChargingStation_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_ChargingStation_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                           Destination:   SourceRouting.To(chargingStation1.Id),
                                           ResetType:     resetType,
                                           CustomData:    null
                                       );

                ClassicAssert.AreEqual(ResultCode.OK,          response.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                      resetRequests.Count);
                ClassicAssert.AreEqual(resetType,              resetRequests.First().ResetType);

            }

        }

        #endregion

        #region Reset_UnknownChargingStation_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_UnknownChargingStation_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation2.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                           Destination:   SourceRouting.To( chargingStation3.Id),
                                           ResetType:     resetType,
                                           CustomData:    null
                                       );

                ClassicAssert.AreEqual  (ResultCode.NetworkError,   response.Result.ResultCode);
                ClassicAssert.IsNotEmpty(                           response.Result.Description);
                ClassicAssert.AreEqual  (ResetStatus.Unknown,       response.Status);

                ClassicAssert.AreEqual  (0,                         resetRequests.Count);

            }

        }

        #endregion

        #region Reset_EVSE_Test()

        /// <summary>
        /// A test for sending a reset message to an EVSE of a charging station.
        /// </summary>
        [Test]
        public async Task Reset_EVSE_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                           Destination:   SourceRouting.To( chargingStation1.Id),
                                           ResetType:     resetType,
                                           EVSEId:        EVSE_Id.Parse(1),
                                           CustomData:    null
                                       );

                ClassicAssert.AreEqual(ResultCode.OK,          response.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                      resetRequests.Count);
                ClassicAssert.AreEqual(resetType,              resetRequests.First().ResetType);
                ClassicAssert.IsTrue  (                        resetRequests.First().EVSEId.HasValue);
                ClassicAssert.AreEqual(EVSE_Id.Parse(1),       resetRequests.First().EVSEId!.Value);

            }

        }

        #endregion

        #region Reset_UnknownEVSE_Test()

        /// <summary>
        /// A test for sending a reset message to an unknown EVSE of a charging station.
        /// </summary>
        [Test]
        public async Task Reset_UnknownEVSE_Test()
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

                var resetRequests = new ConcurrentList<ResetRequest>();

                chargingStation1.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                           Destination:   SourceRouting.To(chargingStation1.Id),
                                           ResetType:     resetType,
                                           EVSEId:        EVSE_Id.Parse(5),
                                           CustomData:    null
                                       );

                ClassicAssert.AreEqual(ResultCode.OK,          response.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Rejected,   response.Status);

                ClassicAssert.AreEqual(1,                      resetRequests.Count);
                ClassicAssert.AreEqual(resetType,              resetRequests.First().ResetType);
                ClassicAssert.IsTrue  (                        resetRequests.First().EVSEId.HasValue);
                ClassicAssert.AreEqual(EVSE_Id.Parse(5),       resetRequests.First().EVSEId!.Value);

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

                var updateFirmwareRequests = new ConcurrentList<UpdateFirmwareRequest>();

                chargingStation1.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp, sender, connection, updateFirmwareRequest, ct) => {
                    updateFirmwareRequests.TryAdd(updateFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.UpdateFirmware(
                                         Destination:               SourceRouting.To(chargingStation1.Id),
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

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     updateFirmwareRequests.Count);

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

                var publishFirmwareRequests = new ConcurrentList<PublishFirmwareRequest>();

                chargingStation1.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp, sender, connection, publishFirmwareRequest, ct) => {
                    publishFirmwareRequests.TryAdd(publishFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.PublishFirmware(
                                         Destination:                SourceRouting.To(chargingStation1.Id),
                                         PublishFirmwareRequestId:   1,
                                         DownloadLocation:           URL.Parse("https://example.org/fw0001.bin"),
                                         MD5Checksum:                "0x1234",
                                         Retries:                    5,
                                         RetryInterval:              TimeSpan.FromMinutes(5),
                                         CustomData:                 null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    publishFirmwareRequests.Count);

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

                var unpublishFirmwareRequests = new ConcurrentList<UnpublishFirmwareRequest>();

                chargingStation1.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp, sender, connection, unpublishFirmwareRequest, ct) => {
                    unpublishFirmwareRequests.TryAdd(unpublishFirmwareRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.UnpublishFirmware(
                                         Destination:         SourceRouting.To(chargingStation1.Id),
                                         MD5Checksum:         "0x1234",
                                         CustomData:          null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    unpublishFirmwareRequests.Count);

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

                var getBaseReportRequests = new ConcurrentList<GetBaseReportRequest>();

                chargingStation1.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp, sender, connection, getBaseReportRequest, ct) => {
                    getBaseReportRequests.TryAdd(getBaseReportRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetBaseReport(
                                         Destination:              SourceRouting.To(chargingStation1.Id),
                                         GetBaseReportRequestId:   1,
                                         ReportBase:               ReportBase.FullInventory,
                                         CustomData:               null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    getBaseReportRequests.Count);

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

                var getReportRequests = new ConcurrentList<GetReportRequest>();

                chargingStation1.OCPP.IN.OnGetReportRequestReceived += (timestamp, sender, connection, getReportRequest, ct) => {
                    getReportRequests.TryAdd(getReportRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetReport(
                                         Destination:          SourceRouting.To(chargingStation1.Id),
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

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    getReportRequests.Count);

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

                var setMonitoringBaseRequests = new ConcurrentList<GetLogRequest>();

                chargingStation1.OCPP.IN.OnGetLogRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetLog(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         LogType:        LogType.DiagnosticsLog,
                                         LogRequestId:   1,
                                         Log:            new LogParameters(
                                                             RemoteLocation:    URL.Parse("https://example.org/log0001.log"),
                                                             OldestTimestamp:   Timestamp.Now - TimeSpan.FromDays(2),
                                                             LatestTimestamp:   Timestamp.Now,
                                                             CustomData:        null
                                                         ),
                                         CustomData:     null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               setMonitoringBaseRequests.Count);

            }

        }

        #endregion


        // Notes: - When no variableInstance is provided, then the default or only instance of a variable is referenced.
        //
        //        - Each variable, in addition to its primary ("Actual") value, can have a set of associated secondary data
        //          that is linked to the same primary variable name and variableInstance.
        //
        //            • Variable characteristics meta-data (read-only)
        //                ◦ Unit of measure(V, W, kW, kWh, etc.)
        //                ◦ Data type(Integer, Decimal, String, Date, OptionList, etc.)
        //                ◦ Lower limit
        //                ◦ Upper limit
        //                ◦ List of allowed values for enumerated variables

        //            • Variable attributes(read-write):
        //                ◦ Actual value
        //                ◦ Target value
        //                ◦ Configured lower limit
        //                ◦ Configured upper limit
        //                ◦ Mutability  (whether the value can be altered or not, e.g.ReadOnly or ReadWrite)
        //                ◦ Persistence (whether the value is preserved in case of a reboot or power loss)
        //
        //        - The CSMS SHALL NOT send more SetVariableData elements in a SetVariablesRequest than reported by the Charging Station via ItemsPerMessageSetVariables.
        //        - When the Charging Station receives a SetVariablesRequest without an attributeType => Actual
        //        - The CSMS SHALL NOT include multiple SetVariableData elements, in a single SetVariablesRequest, with the same
        //          Component, Variable and AttributeType combination. Note that an omitted AttributeType counts as the value Actual.
        //

        #region SetVariables_Tests()

        /// <summary>
        /// Tests for setting variables of a charging station.
        /// </summary>
        [Test]
        public async Task SetVariables_Tests()
        {

            Assert.That(testCSMS01,                Is.Not.Null);
            Assert.That(testBackendWebSockets01,   Is.Not.Null);
            Assert.That(chargingStation1,          Is.Not.Null);
            Assert.That(chargingStation2,          Is.Not.Null);
            Assert.That(chargingStation3,          Is.Not.Null);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var setVariablesRequests = new ConcurrentList<SetVariablesRequest>();

                chargingStation1.OCPP.IN.OnSetVariablesRequestReceived += (timestamp, sender, connection, setLogRequest, ct) => {
                    setVariablesRequests.TryAdd(setLogRequest);
                    return Task.CompletedTask;
                };

                var setVariablesResponse = await testCSMS01.SetVariables(
                                                     Destination:         SourceRouting.To( chargingStation1.Id),
                                                     VariableData:        [

                                                                              #region 1. Known component & component instance...                  [must pass!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud by GraphDefined GmbH",
                                                                                  OldAttributeValue:   null,
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region 2. Unknown component...                                     [must fail!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlXXXX",
                                                                                                           Instance:    "Alert System!",
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud by GraphDefined GmbH",
                                                                                  OldAttributeValue:   null,
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region 3. Known component, unkown component instance...            [must fail!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:    "Alert System!",
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud by GraphDefined GmbH",
                                                                                  OldAttributeValue:   null,
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region 4. Known component, unknown variable...                     [must fail!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "Temperature Sensors",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud by GraphDefined GmbH",
                                                                                  OldAttributeValue:   null,
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region 5. Known component & variable, unknown variable instance... [must fail!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:    "Section31CodeName",
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud by GraphDefined GmbH",
                                                                                  OldAttributeValue:   null,
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              )

                                                                              #endregion

                                                                          ],
                                                     CustomData:          null
                                                 );

                Assert.That(setVariablesResponse.Result.ResultCode,            Is.EqualTo(ResultCode.OK));
                Assert.That(setVariablesRequests.Count,                        Is.EqualTo(1));

                Assert.That(setVariablesResponse.SetVariableResults.Count,     Is.EqualTo(5));

                var firstResult  = setVariablesResponse.SetVariableResults.First();
                Assert.That(firstResult. AttributeStatus,                               Is.EqualTo(SetVariableStatus.Accepted));

                var secondResult = setVariablesResponse.SetVariableResults.Skip(1).First();
                Assert.That(secondResult.AttributeStatus,                               Is.EqualTo(SetVariableStatus.UnknownComponent));

                var thirdResult  = setVariablesResponse.SetVariableResults.Skip(2).First();
                Assert.That(thirdResult. AttributeStatus,                               Is.EqualTo(SetVariableStatus.UnknownComponent));

                var fourthResult = setVariablesResponse.SetVariableResults.Skip(3).First();
                Assert.That(fourthResult.AttributeStatus,                               Is.EqualTo(SetVariableStatus.UnknownVariable));

                var fifthResult  = setVariablesResponse.SetVariableResults.Skip(4).First();
                Assert.That(fifthResult. AttributeStatus,                               Is.EqualTo(SetVariableStatus.UnknownVariable));


            }

        }

        #endregion

        #region SetVariables_Conditional_Tests()

        /// <summary>
        /// Tests for setting variables of a charging station via safe conditional requests.
        /// </summary>
        [Test]
        public async Task SetVariables_Conditional_Test()
        {

            Assert.That(testCSMS01,                Is.Not.Null);
            Assert.That(testBackendWebSockets01,   Is.Not.Null);
            Assert.That(chargingStation1,          Is.Not.Null);
            Assert.That(chargingStation2,          Is.Not.Null);
            Assert.That(chargingStation3,          Is.Not.Null);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var setVariablesRequests = new ConcurrentList<SetVariablesRequest>();

                chargingStation1.OCPP.IN.OnSetVariablesRequestReceived += (timestamp, sender, connection, setLogRequest, ct) => {
                    setVariablesRequests.TryAdd(setLogRequest);
                    return Task.CompletedTask;
                };

                var setVariablesResponse = await testCSMS01.SetVariables(
                                                     Destination:    SourceRouting.To( chargingStation1.Id),
                                                     VariableData:        [

                                                                              #region Correct old value...     [must pass!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud #1 by GraphDefined GmbH",
                                                                                  OldAttributeValue:   "GraphDefined CSO",
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region Outdated old value...    [must fail!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud #2 by GraphDefined GmbH",
                                                                                  OldAttributeValue:   "GraphDefined CSO",
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                              #region Correct updated value... [must pass!]

                                                                              new SetVariableData(
                                                                                  Component:           new Component(
                                                                                                           Name:        "SecurityCtrlr",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  Variable:            new Variable(
                                                                                                           Name:        "OrganizationName",
                                                                                                           Instance:     null,
                                                                                                           CustomData:   null
                                                                                                       ),
                                                                                  AttributeValue:      "Open Charging Cloud #2 by GraphDefined GmbH",
                                                                                  OldAttributeValue:   "Open Charging Cloud #1 by GraphDefined GmbH",
                                                                                  AttributeType:       null,
                                                                                  CustomData:          null
                                                                              ),

                                                                              #endregion

                                                                          ],
                                                     CustomData:          null
                                                 );

                Assert.That(setVariablesResponse.Result.ResultCode,            Is.EqualTo(ResultCode.OK));
                Assert.That(setVariablesRequests.Count,                        Is.EqualTo(1));


                Assert.That(setVariablesResponse.SetVariableResults.Count(),   Is.EqualTo(3));

                var firstResult  = setVariablesResponse.SetVariableResults.First();
                Assert.That(firstResult. AttributeStatus,                               Is.EqualTo(SetVariableStatus.Accepted));

                var secondResult = setVariablesResponse.SetVariableResults.Skip(1).First();
                Assert.That(secondResult.AttributeStatus,                               Is.EqualTo(SetVariableStatus.Rejected));

                var thirdResult  = setVariablesResponse.SetVariableResults.Skip(2).First();
                Assert.That(thirdResult. AttributeStatus,                               Is.EqualTo(SetVariableStatus.Accepted));

            }

        }

        #endregion

        #region SetVariables_Conditional_WithinSameRequest_Passes_Test()

        /// <summary>
        /// A test for setting variables of a charging station.
        /// </summary>
        [Test]
        public async Task SetVariables_Conditional_WithinSameRequest_Test()
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

                var setVariablesRequests = new ConcurrentList<SetVariablesRequest>();

                chargingStation1.OCPP.IN.OnSetVariablesRequestReceived += (timestamp, sender, connection, setVariablesRequest, ct) => {
                    setVariablesRequests.TryAdd(setVariablesRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetVariables(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         VariableData:    [
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
                                                          ],
                                         CustomData:      null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setVariablesRequests.Count);

            }

        }

        #endregion

        #region SetVariables_Conditional_WithinSameRequest_Fails_Test()

        /// <summary>
        /// A test for setting variables of a charging station.
        /// </summary>
        [Test]
        public async Task SetVariables_Conditional_WithinSameRequest_Fails_Test()
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

                var setVariablesRequests = new ConcurrentList<SetVariablesRequest>();

                chargingStation1.OCPP.IN.OnSetVariablesRequestReceived += (timestamp, sender, connection, setVariablesRequest, ct) => {
                    setVariablesRequests.TryAdd(setVariablesRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetVariables(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         VariableData:        [
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
                                                              ],
                                         CustomData:          null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setVariablesRequests.Count);

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

                var getVariablesRequests = new ConcurrentList<GetVariablesRequest>();

                chargingStation1.OCPP.IN.OnGetVariablesRequestReceived += (timestamp, sender, connection, getVariablesRequest, ct) => {
                    getVariablesRequests.TryAdd(getVariablesRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetVariables(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         VariableData:        [

                                                                  new GetVariableData(
                                                                      Component:       new Component(
                                                                                           Name:         "OCPPCommCtrlr"
                                                                                       ),
                                                                      Variable:        new Variable(
                                                                                           Name:         "FileTransferProtocols"
                                                                                       )
                                                                  ),

                                                                  new GetVariableData(
                                                                      Component:       new Component(
                                                                                           Name:         "SecurityCtrlr"
                                                                                       ),
                                                                      Variable:        new Variable(
                                                                                           Name:         "OrganizationName"
                                                                                       )
                                                                  )

                                                                  //new GetVariableData(
                                                                  //    Component:       new Component(
                                                                  //                         Name:         "Alert System!",
                                                                  //                         Instance:     "Alert System #1",
                                                                  //                         EVSE:         new EVSE(
                                                                  //                                           Id:            EVSE_Id.     Parse(1),
                                                                  //                                           ConnectorId:   Connector_Id.Parse(1),
                                                                  //                                           CustomData:    null
                                                                  //                                       ),
                                                                  //                         CustomData:   null
                                                                  //                     ),
                                                                  //    Variable:        new Variable(
                                                                  //                         Name:         "Temperature Sensors",
                                                                  //                         Instance:     "Temperature Sensor #1",
                                                                  //                         CustomData:   null
                                                                  //                     ),
                                                                  //    AttributeType:   AttributeTypes.Actual,
                                                                  //    CustomData:      null
                                                                  //)

                                                              ],
                                         CustomData:          null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    getVariablesRequests.Count);


                Assert.That(response.Results.Count, Is.EqualTo(2));

                var firstResult = response.Results.First();

                Assert.That(firstResult.AttributeValue, Is.EqualTo("HTTPS"));



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

                var setMonitoringBaseRequests = new ConcurrentList<SetMonitoringBaseRequest>();

                chargingStation1.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetMonitoringBase(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         MonitoringBase:  MonitoringBase.All,
                                         CustomData:      null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setMonitoringBaseRequests.Count);

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

                var setMonitoringBaseRequests = new ConcurrentList<GetMonitoringReportRequest>();

                chargingStation1.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetMonitoringReport(
                                         Destination:                    SourceRouting.To(            chargingStation1.Id),
                                         GetMonitoringReportRequestId:   1,
                                         MonitoringCriteria:             new[] {
                                                                             MonitoringCriterion.PeriodicMonitoring
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

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     setMonitoringBaseRequests.Count);

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

                var setMonitoringBaseRequests = new ConcurrentList<SetMonitoringLevelRequest>();

                chargingStation1.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetMonitoringLevel(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         Severity:            Severities.Informational,
                                         CustomData:          null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setMonitoringBaseRequests.Count);

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

                var setMonitoringBaseRequests = new ConcurrentList<SetVariableMonitoringRequest>();

                chargingStation1.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetVariableMonitoring(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         MonitoringData:      new[] {
                                                                  new SetMonitoringData(
                                                                      Value:                  23.2M,
                                                                      MonitorType:            MonitorType.Delta,
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
                                         CustomData:          null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setMonitoringBaseRequests.Count);

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

                var setMonitoringBaseRequests = new ConcurrentList<ClearVariableMonitoringRequest>();

                chargingStation1.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp, sender, connection, setMonitoringBaseRequest, ct) => {
                    setMonitoringBaseRequests.TryAdd(setMonitoringBaseRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.ClearVariableMonitoring(
                                         Destination:    SourceRouting.To(     chargingStation1.Id),
                                         VariableMonitoringIds:   new[] {
                                                                      VariableMonitoring_Id.NewRandom
                                                                  },
                                         CustomData:              null
                                     );

                ClassicAssert.AreEqual(ResultCode.OK,        response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                    setMonitoringBaseRequests.Count);

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

                var setNetworkProfileRequests = new ConcurrentList<SetNetworkProfileRequest>();

                chargingStation1.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp, sender, connection, setNetworkProfileRequest, ct) => {
                    setNetworkProfileRequests.TryAdd(setNetworkProfileRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetNetworkProfile(
                                         Destination:    SourceRouting.To(            chargingStation1.Id),
                                         ConfigurationSlot:          1,
                                         NetworkConnectionProfile:   new NetworkConnectionProfile(
                                                                         Version:             OCPPVersion.OCPP201,
                                                                         Transport:           TransportProtocols.JSON,
                                                                         CentralServiceURL:   URL.Parse("https://example.com/OCPPv2.0/"),
                                                                         MessageTimeout:      TimeSpan.FromSeconds(30),
                                                                         SecurityProfile:     SecurityProfiles.SecurityProfile3,
                                                                         NetworkInterface:    NetworkInterface.Wireless1,
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

                ClassicAssert.AreEqual(ResultCode.OK,                      response.Result.ResultCode);
                ClassicAssert.AreEqual(SetNetworkProfileStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                  setNetworkProfileRequests.Count);

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
                csms1WebSocketJSONMessagesSent                        is not null &&
                csms1WebSocketJSONMessageResponsesReceived            is not null &&
                csms1WebSocketJSONMessagesReceived                    is not null &&
                csms1WebSocketJSONMessageResponsesSent                is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketJSONMessagesReceived         is not null &&
                chargingStation1WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation1WebSocketJSONMessagesSent             is not null &&
                chargingStation1WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketJSONMessagesReceived         is not null &&
                chargingStation2WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation2WebSocketJSONMessagesSent             is not null &&
                chargingStation2WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketJSONMessagesReceived         is not null &&
                chargingStation3WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation3WebSocketJSONMessagesSent             is not null &&
                chargingStation3WebSocketJSONMessageResponsesReceived is not null)

            {

                var changeAvailabilityRequests = new ConcurrentList<ChangeAvailabilityRequest>();

                chargingStation1.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp, sender, connection, changeAvailabilityRequest, ct) => {
                    changeAvailabilityRequests.TryAdd(changeAvailabilityRequest);
                    return Task.CompletedTask;
                };

                var evseId             = EVSE_Id.     Parse(1);
                var connectorId        = Connector_Id.Parse(1);
                var operationalStatus  = OperationalStatus.Operative;

                var response           = await testCSMS01.ChangeAvailability(
                                                   Destination:         SourceRouting.To(chargingStation1.Id),
                                                   OperationalStatus:   operationalStatus,
                                                   EVSE:                new EVSE(
                                                                            Id:            evseId,
                                                                            ConnectorId:   connectorId,
                                                                            CustomData:    null
                                                                        ),
                                                   CustomData:          null
                                               );

                ClassicAssert.AreEqual(ResultCode.OK,                       response.Result.ResultCode);
                ClassicAssert.AreEqual(ChangeAvailabilityStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                   changeAvailabilityRequests.Count);
                ClassicAssert.AreEqual(evseId,                              changeAvailabilityRequests.First().EVSE?.Id);
                ClassicAssert.AreEqual(connectorId,                         changeAvailabilityRequests.First().EVSE?.ConnectorId);
                ClassicAssert.AreEqual(operationalStatus,                   changeAvailabilityRequests.First().OperationalStatus);

                ClassicAssert.AreEqual(1,                                   csms1WebSocketJSONMessagesSent.                        Count);
                ClassicAssert.AreEqual(1,                                   csms1WebSocketJSONMessageResponsesReceived.            Count);
                ClassicAssert.AreEqual(1,                                   csms1WebSocketJSONMessagesReceived.                    Count);
                ClassicAssert.AreEqual(0,                                   csms1WebSocketJSONMessageResponsesSent.                Count);

                ClassicAssert.AreEqual(1,                                   chargingStation1WebSocketJSONMessagesReceived.        Count);
                ClassicAssert.AreEqual(1,                                   chargingStation1WebSocketJSONMessageResponsesSent.    Count);
                ClassicAssert.AreEqual(1,                                   chargingStation1WebSocketJSONMessagesSent.            Count);
                ClassicAssert.AreEqual(0,                                   chargingStation1WebSocketJSONMessageResponsesReceived.Count);

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

                var triggerMessageRequests = new ConcurrentList<TriggerMessageRequest>();

                chargingStation1.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp, sender, connection, triggerMessageRequest, ct) => {
                    triggerMessageRequests.TryAdd(triggerMessageRequest);
                    return Task.CompletedTask;
                };

                var evseId          = EVSE_Id.Parse(1);
                var messageTrigger  = MessageTrigger.StatusNotification;

                var response        = await testCSMS01.TriggerMessage(
                                                Destination:        SourceRouting.To(chargingStation1.Id),
                                                RequestedMessage:   messageTrigger,
                                                EVSE:               new EVSE(
                                                                        evseId
                                                                    ),
                                                CustomData:         null
                                            );

                ClassicAssert.AreEqual(ResultCode.OK,                   response.Result.ResultCode);
                ClassicAssert.AreEqual(TriggerMessageStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                               triggerMessageRequests.Count);

            }

        }

        #endregion


        #region TransferTextData_Test()

        /// <summary>
        /// A test for transfering text data to charging stations.
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       GraphDefined;
                var messageId  = Message_Id.      Parse       (RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await testCSMS01.TransferData(
                                           Destination:   SourceRouting.To( chargingStation1.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
                                       );


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);
                ClassicAssert.AreEqual(data.Reverse(),        response.Data?.ToString());

                ClassicAssert.AreEqual(1,                     dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,              dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,             dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(data,                  dataTransferRequests.First().Data?.ToString());

            }

        }

        #endregion

        #region TransferJObjectData_Test()

        /// <summary>
        /// A test for transfering JObject data to charging stations.
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
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

                var response   = await testCSMS01.TransferData(
                                           Destination:   SourceRouting.To( chargingStation1.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
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
        /// A test for transfering JArray data to charging stations.
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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await testCSMS01.TransferData(
                                           Destination:   SourceRouting.To( chargingStation1.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
                                       );


                ClassicAssert.AreEqual(ResultCode.OK,                  response.Result.ResultCode);
                ClassicAssert.AreEqual(JTokenType.Array,               response.Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),       response.Data?[0]?.Value<String>()?.Reverse());

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(JTokenType.Array,               dataTransferRequests.First().Data?.Type);
                ClassicAssert.AreEqual(data[0]?.Value<String>(),       dataTransferRequests.First().Data?[0]?.Value<String>());

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

                var dataTransferRequests = new ConcurrentList<DataTransferRequest>();

                chargingStation1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. Parse("ACME Inc.");
                var messageId  = Message_Id.Parse("hello");
                var data       = "world!";
                var response   = await testCSMS01.TransferData(
                                           Destination:   SourceRouting.To( chargingStation1.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
                                       );

                ClassicAssert.AreEqual(ResultCode.OK,                  response.Result.ResultCode);
                ClassicAssert.AreEqual(DataTransferStatus.Rejected,    response.Status);

                ClassicAssert.AreEqual(1,                              dataTransferRequests.Count);
                ClassicAssert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                ClassicAssert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                ClassicAssert.AreEqual(data,                           dataTransferRequests.First().Data?.ToString());

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

                var certificateSignedRequests = new ConcurrentList<CertificateSignedRequest>();

                chargingStation1.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp, sender, connection, certificateSignedRequest, ct) => {
                    certificateSignedRequests.TryAdd(certificateSignedRequest);
                    return Task.CompletedTask;
                };


                var response = await testCSMS01.SendSignedCertificate(
                                         Destination:        SourceRouting.To(chargingStation1.Id),
                                         CertificateChain:   new OCPP.CertificateChain(
                                                                 Certificates:   [
                                                                                     OCPP.Certificate.Parse(
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
                                                                                 ]
                                                             ),
                                         CertificateType:    CertificateSigningUse.ChargingStationCertificate,
                                         CustomData:         null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,                      response.Result.ResultCode);
                ClassicAssert.AreEqual(CertificateSignedStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                  certificateSignedRequests.Count);

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

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp, sender, connection, installCertificateRequest, ct) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };


                var response = await testCSMS01.InstallCertificate(
                                         Destination:       SourceRouting.To(chargingStation1.Id),
                                         CertificateType:   InstallCertificateUse.V2GRootCertificate,
                                         Certificate:       OCPP.Certificate.Parse(
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


                ClassicAssert.AreEqual(ResultCode.OK,                  response.Result.ResultCode);
                ClassicAssert.AreEqual(CertificateStatus.Accepted,     response.Status);

                ClassicAssert.AreEqual(1,                              installCertificateRequests.Count);

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

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp, sender, connection, installCertificateRequest, ct) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };

                var response1 = await testCSMS01.InstallCertificate(
                                          Destination:       SourceRouting.To(chargingStation1.Id),
                                          CertificateType:   InstallCertificateUse.V2GRootCertificate,
                                          Certificate:       OCPP.Certificate.Parse(
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


                ClassicAssert.AreEqual(ResultCode.OK,                  response1.Result.ResultCode);
                ClassicAssert.AreEqual(CertificateStatus.Accepted,     response1.Status);

                ClassicAssert.AreEqual(1,                              installCertificateRequests.Count);


                await Task.Delay(500);


                var getInstalledCertificateIdsRequests = new ConcurrentList<GetInstalledCertificateIdsRequest>();

                chargingStation1.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp, sender, connection, getInstalledCertificateIdsRequest, ct) => {
                    getInstalledCertificateIdsRequests.TryAdd(getInstalledCertificateIdsRequest);
                    return Task.CompletedTask;
                };

                var response2  = await testCSMS01.GetInstalledCertificateIds(
                                           Destination:        SourceRouting.To(chargingStation1.Id),
                                           CertificateTypes:   [ GetCertificateIdUse.V2GRootCertificate ],
                                           CustomData:         null
                                       );


                ClassicAssert.AreEqual(ResultCode.OK,                            response2.Result.ResultCode);
                ClassicAssert.AreEqual(GetInstalledCertificateStatus.Accepted,   response2.Status);

                ClassicAssert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);

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

                #region 1. Install Certificate

                var installCertificateRequests = new ConcurrentList<InstallCertificateRequest>();

                chargingStation1.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp, sender, connection, installCertificateRequest, ct) => {
                    installCertificateRequests.TryAdd(installCertificateRequest);
                    return Task.CompletedTask;
                };

                var response1 = await testCSMS01.InstallCertificate(
                                          Destination:       SourceRouting.To(chargingStation1.Id),
                                          CertificateType:   InstallCertificateUse.V2GRootCertificate,
                                          Certificate:       OCPP.Certificate.Parse(
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


                ClassicAssert.AreEqual(ResultCode.OK,                  response1.Result.ResultCode);
                ClassicAssert.AreEqual(CertificateStatus.Accepted,     response1.Status);

                ClassicAssert.AreEqual(1,                              installCertificateRequests.Count);


                await Task.Delay(500);

                #endregion

                #region 2. Get installed certificate identifications

                var getInstalledCertificateIdsRequests = new ConcurrentList<GetInstalledCertificateIdsRequest>();

                chargingStation1.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp, sender, connection, getInstalledCertificateIdsRequest, ct) => {
                    getInstalledCertificateIdsRequests.TryAdd(getInstalledCertificateIdsRequest);
                    return Task.CompletedTask;
                };

                var response2 = await testCSMS01.GetInstalledCertificateIds(
                                          Destination:        SourceRouting.To( chargingStation1.Id),
                                          CertificateTypes:   [ GetCertificateIdUse.V2GRootCertificate ],
                                          CustomData:         null
                                      );


                ClassicAssert.AreEqual(ResultCode.OK,                            response2.Result.ResultCode);
                ClassicAssert.AreEqual(GetInstalledCertificateStatus.Accepted,   response2.Status);
                ClassicAssert.AreEqual(1,                                        response2.CertificateHashDataChain.Count());

                ClassicAssert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);


                await Task.Delay(500);

                #endregion

                #region 3. Delete certificate

                var deleteCertificateRequests = new ConcurrentList<DeleteCertificateRequest>();

                chargingStation1.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp, sender, connection, deleteCertificateRequest, ct) => {
                    deleteCertificateRequests.TryAdd(deleteCertificateRequest);
                    return Task.CompletedTask;
                };

                var response3 = await testCSMS01.DeleteCertificate(
                                          Destination:           SourceRouting.To(chargingStation1.Id),
                                          CertificateHashData:   response2.CertificateHashDataChain.First().CertificateHashData,
                                          CustomData:            null
                                      );


                ClassicAssert.AreEqual(ResultCode.OK,                      response3.Result.ResultCode);
                ClassicAssert.AreEqual(DeleteCertificateStatus.Accepted,   response3.Status);

                ClassicAssert.AreEqual(1,                                  deleteCertificateRequests.Count);


                // Verification
                getInstalledCertificateIdsRequests.Clear();

                var response4  = await testCSMS01.GetInstalledCertificateIds(
                                           Destination:        SourceRouting.To( chargingStation1.Id),
                                           CertificateTypes:   [ GetCertificateIdUse.V2GRootCertificate ],
                                           CustomData:         null
                                       );


                ClassicAssert.AreEqual(ResultCode.OK,                            response4.Result.ResultCode);
                ClassicAssert.AreEqual(GetInstalledCertificateStatus.Accepted,   response4.Status);
                ClassicAssert.AreEqual(0,                                        response4.CertificateHashDataChain.Count());

                ClassicAssert.AreEqual(1,                                        getInstalledCertificateIdsRequests.Count);


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

                var notifyCRLRequests = new ConcurrentList<NotifyCRLRequest>();

                chargingStation1.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp, sender, connection, notifyCRLRequest, ct) => {
                    notifyCRLRequests.TryAdd(notifyCRLRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.NotifyCRLAvailability(
                                         Destination:          SourceRouting.To(chargingStation1.Id),
                                         NotifyCRLRequestId:   1,
                                         Availability:         NotifyCRLStatus.Available,
                                         Location:             URL.Parse("https://localhost/clr.json"),
                                         CustomData:           null
                                     );;


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               notifyCRLRequests.Count);

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

                var getLocalListVersionRequests = new ConcurrentList<GetLocalListVersionRequest>();

                chargingStation1.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp, sender, connection, getLocalListVersionRequest, ct) => {
                    getLocalListVersionRequests.TryAdd(getLocalListVersionRequest);
                    return Task.CompletedTask;
                };


                var response = await testCSMS01.GetLocalListVersion(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         CustomData:      null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     getLocalListVersionRequests.Count);

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

                var sendLocalListRequests = new ConcurrentList<SendLocalListRequest>();

                chargingStation1.OCPP.IN.OnSendLocalListRequestReceived += (timestamp, sender, connection, sendLocalListRequest, ct) => {
                    sendLocalListRequests.TryAdd(sendLocalListRequest);
                    return Task.CompletedTask;
                };


                var response  = await testCSMS01.SendLocalList(
                                          Destination:    SourceRouting.To(      chargingStation1.Id),
                                          ListVersion:              1,
                                          UpdateType:               UpdateTypes.Full,
                                          LocalAuthorizationList:   new[] {
                                                                        new AuthorizationData(
                                                                            IdToken:       new IdToken(
                                                                                               Value:                 "aabbccdd",
                                                                                               Type:                  IdTokenType.ISO14443,
                                                                                               AdditionalInfos:       [
                                                                                                                          new AdditionalInfo(
                                                                                                                              AdditionalIdToken:   "1234",
                                                                                                                              Type:                "PIN",
                                                                                                                              CustomData:          null
                                                                                                                          )
                                                                                                                      ],
                                                                                               CustomData:            null
                                                                                           ),
                                                                            IdTokenInfo:   new IdTokenInfo(
                                                                                               Status:                AuthorizationStatus.Accepted,
                                                                                               ChargingPriority:      8,
                                                                                               CacheExpiryDateTime:   Timestamp.Now + TimeSpan.FromDays(3),
                                                                                               ValidEVSEIds:          [
                                                                                                                          EVSE_Id.Parse(1)
                                                                                                                      ],
                                                                                               GroupIdToken:          new IdToken(
                                                                                                                          Value:                 "55667788",
                                                                                                                          Type:                  IdTokenType.ISO14443,
                                                                                                                          AdditionalInfos:       [
                                                                                                                                                     new AdditionalInfo(
                                                                                                                                                         AdditionalIdToken:   "1234",
                                                                                                                                                         Type:                "PIN",
                                                                                                                                                         CustomData:          null
                                                                                                                                                     )
                                                                                                                                                 ],
                                                                                                                          CustomData:            null
                                                                                                                      ),
                                                                                               Language1:             Language_Id.Parse("de"),
                                                                                               Language2:             Language_Id.Parse("en"),
                                                                                               PersonalMessage:       new MessageContents(
                                                                                                                          Content:      "Hello world!",
                                                                                                                          Format:       MessageFormat.UTF8,
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


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     sendLocalListRequests.Count);

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

                var clearCacheRequests = new ConcurrentList<ClearCacheRequest>();

                chargingStation1.OCPP.IN.OnClearCacheRequestReceived += (timestamp, sender, connection, clearCacheRequest, ct) => {
                    clearCacheRequests.TryAdd(clearCacheRequest);
                    return Task.CompletedTask;
                };


                var response = await testCSMS01.ClearCache(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         CustomData:      null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     clearCacheRequests.Count);

            }

        }

        #endregion


        #region QRCodeScanned_Test()

        /// <summary>
        /// A test for creating a reservation at a charging station.
        /// </summary>
        [Test]
        public async Task QRCodeScanned_Test()
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

                var qrCodeScannedRequests = new ConcurrentList<QRCodeScannedRequest>();

                chargingStation1.OCPP.IN.OnQRCodeScannedRequestReceived += (timestamp, sender, connection, qrCodeScannedRequest, ct) => {
                    qrCodeScannedRequests.TryAdd(qrCodeScannedRequest);
                    return Task.CompletedTask;
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);
                var timeout         = TimeSpan.      FromMinutes(1);

                var response        = await testCSMS01.QRCodeScanned(
                                                Destination:    SourceRouting.To( chargingStation1.Id),
                                                EVSEId:          evseId,
                                                Timeout:         timeout,
                                                CustomData:      null
                                            );


                ClassicAssert.AreEqual(ResultCode.OK,   response.Result.ResultCode);

                ClassicAssert.AreEqual(1,               qrCodeScannedRequests.Count);

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

                var reserveNowRequests = new ConcurrentList<ReserveNowRequest>();

                chargingStation1.OCPP.IN.OnReserveNowRequestReceived += (timestamp, sender, connection, reserveNowRequest, ct) => {
                    reserveNowRequests.TryAdd(reserveNowRequest);
                    return Task.CompletedTask;
                };


                var reservationId   = Reservation_Id.NewRandom;
                var evseId          = EVSE_Id.       Parse(1);
                var connectorType   = ConnectorType.sType2;

                var response        = await testCSMS01.ReserveNow(
                                                Destination:    SourceRouting.To( chargingStation1.Id),
                                                ReservationId:   reservationId,
                                                ExpiryDate:      Timestamp.Now + TimeSpan.FromHours(2),
                                                IdToken:         new IdToken(
                                                                     Value:             "22334455",
                                                                     Type:              IdTokenType.ISO14443,
                                                                     AdditionalInfos:   [
                                                                                            new AdditionalInfo(
                                                                                                AdditionalIdToken:   "123",
                                                                                                Type:                "typetype",
                                                                                                CustomData:          null
                                                                                            )
                                                                                        ],
                                                                     CustomData:        null
                                                                 ),
                                                ConnectorType:   connectorType,
                                                EVSEId:          evseId,
                                                GroupIdToken:    new IdToken(
                                                                     Value:             "55667788",
                                                                     Type:              IdTokenType.ISO14443,
                                                                     AdditionalInfos:   [
                                                                                            new AdditionalInfo(
                                                                                                AdditionalIdToken:   "567",
                                                                                                Type:                "type2type2",
                                                                                                CustomData:          null
                                                                                            )
                                                                                        ],
                                                                     CustomData:        null
                                                                 ),
                                                CustomData:      null
                                            );


                ClassicAssert.AreEqual(ResultCode.OK,                response.Result.ResultCode);
                ClassicAssert.AreEqual(ReservationStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                            reserveNowRequests.Count);

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

                var cancelReservationRequests = new ConcurrentList<CancelReservationRequest>();

                chargingStation1.OCPP.IN.OnCancelReservationRequestReceived += (timestamp, sender, connection, cancelReservationRequest, ct) => {
                    cancelReservationRequests.TryAdd(cancelReservationRequest);
                    return Task.CompletedTask;
                };

                var reservationId  = Reservation_Id.NewRandom;
                var response       = await testCSMS01.CancelReservation(
                                               Destination:     SourceRouting.To(chargingStation1.Id),
                                               ReservationId:   reservationId,
                                               CustomData:      null
                                           );


                ClassicAssert.AreEqual(ResultCode.OK,                      response.Result.ResultCode);
                ClassicAssert.AreEqual(CancelReservationStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                  cancelReservationRequests.Count);

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

                var requestStartTransactionRequests  = new ConcurrentList<RequestStartTransactionRequest>();
                var requestStopTransactionRequests   = new ConcurrentList<RequestStopTransactionRequest>();

                chargingStation1.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp, sender, connection, requestStartTransactionRequest, ct) => {
                    requestStartTransactionRequests.TryAdd(requestStartTransactionRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OCPP.IN.OnRequestStopTransactionRequestReceived  += (timestamp, sender, connection, requestStopTransactionRequest, ct) => {
                    requestStopTransactionRequests. TryAdd(requestStopTransactionRequest);
                    return Task.CompletedTask;
                };

                var startResponse = await testCSMS01.StartCharging(
                                              Destination:                        SourceRouting.To(chargingStation1.Id),
                                              RequestStartTransactionRequestId:   RemoteStart_Id.NewRandom,
                                              IdToken:                            new IdToken(
                                                                                      Value:             "aabbccdd",
                                                                                      Type:              IdTokenType.ISO14443,
                                                                                      AdditionalInfos:   null,
                                                                                      CustomData:        null
                                                                                  ),
                                              EVSEId:                             EVSE_Id.Parse(1),
                                              ChargingProfile:                    null,
                                              GroupIdToken:                       new IdToken(
                                                                                      Value:             "cafebabe",
                                                                                      Type:              IdTokenType.ISO14443,
                                                                                      AdditionalInfos:   null,
                                                                                      CustomData:        null
                                                                                  ),
                                              CustomData:                         null
                                          );


                ClassicAssert.AreEqual(ResultCode.OK,         startResponse.Result.ResultCode);
                ClassicAssert.IsTrue  (startResponse.TransactionId.HasValue);

                ClassicAssert.AreEqual(1,                     requestStartTransactionRequests.Count);

                await Task.Delay(500);


                if (startResponse.TransactionId.HasValue)
                {

                    var stopResponse = await testCSMS01.StopCharging(
                                           Destination:     SourceRouting.To(chargingStation1.Id),
                                           TransactionId:   startResponse.TransactionId.Value,
                                           CustomData:      null
                                       );


                    ClassicAssert.AreEqual(ResultCode.OK,           stopResponse.Result.ResultCode);
                    //ClassicAssert.AreEqual(UnlockStatus.Unlocked,   response1.Status);

                    ClassicAssert.AreEqual(1,                       requestStopTransactionRequests.Count);

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

                var unlockConnectorRequests = new ConcurrentList<GetTransactionStatusRequest>();

                chargingStation1.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp, sender, connection, unlockConnectorRequest, ct) => {
                    unlockConnectorRequests.TryAdd(unlockConnectorRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetTransactionStatus(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         TransactionId:   null,
                                         CustomData:      null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,           response.Result.ResultCode);
                //ClassicAssert.AreEqual(UnlockStatus.Unlocked,   response1.Status);

                ClassicAssert.AreEqual(1,                       unlockConnectorRequests.Count);

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

                var setChargingProfileRequests = new ConcurrentList<SetChargingProfileRequest>();

                chargingStation1.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp, sender, connection, setChargingProfileRequest, ct) => {
                    setChargingProfileRequests.TryAdd(setChargingProfileRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SetChargingProfile(
                                   Destination:    SourceRouting.To(   chargingStation1.Id),
                                   EVSEId:            chargingStation1.EVSEs.First().Id,
                                   ChargingProfile:   new ChargingProfile(
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
                                                      ),
                                   CustomData:        null
                               );


                ClassicAssert.AreEqual(ResultCode.OK,                    response.Result.ResultCode);
                ClassicAssert.AreEqual(ChargingProfileStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                setChargingProfileRequests.Count);

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

                var getChargingProfilesRequests = new ConcurrentList<GetChargingProfilesRequest>();

                chargingStation1.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp, sender, connection, getChargingProfilesRequest, ct) => {
                    getChargingProfilesRequests.TryAdd(getChargingProfilesRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetChargingProfiles(
                                         Destination:    SourceRouting.To(                chargingStation1.Id),
                                         GetChargingProfilesRequestId:   1,
                                         ChargingProfile:                new ChargingProfileCriterion(
                                                                             ChargingProfilePurpose:   ChargingProfilePurpose.TxDefaultProfile,
                                                                             StackLevel:               1,
                                                                             ChargingProfileIds:       [
                                                                                                           ChargingProfile_Id.Parse(123)
                                                                                                       ],
                                                                             ChargingLimitSources:     [
                                                                                                           ChargingLimitSource.SO
                                                                                                       ],
                                                                             CustomData:               null
                                                                         ),
                                         EVSEId:                         EVSE_Id.Parse(1),
                                         CustomData:                     null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,                       response.Result.ResultCode);
                ClassicAssert.AreEqual(GetChargingProfileStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                   getChargingProfilesRequests.Count);

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

                var getChargingProfilesRequests = new ConcurrentList<ClearChargingProfileRequest>();

                chargingStation1.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp, sender, connection, getChargingProfilesRequest, ct) => {
                    getChargingProfilesRequests.TryAdd(getChargingProfilesRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.ClearChargingProfile(
                                         Destination:    SourceRouting.To(           chargingStation1.Id),
                                         ChargingProfileId:         ChargingProfile_Id.Parse(123),
                                         ChargingProfileCriteria:   new ClearChargingProfile(
                                                                        EVSEId:                   EVSE_Id.Parse(1),
                                                                        ChargingProfilePurpose:   ChargingProfilePurpose.TxDefaultProfile,
                                                                        StackLevel:               1,
                                                                        CustomData:               null
                                                                    ),
                                         CustomData:                null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,                         response.Result.ResultCode);
                ClassicAssert.AreEqual(ClearChargingProfileStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                     getChargingProfilesRequests.Count);

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

                var getCompositeScheduleRequests = new ConcurrentList<GetCompositeScheduleRequest>();

                chargingStation1.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp, sender, connection, getCompositeScheduleRequest, ct) => {
                    getCompositeScheduleRequests.TryAdd(getCompositeScheduleRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.GetCompositeSchedule(
                                         Destination:    SourceRouting.To(    chargingStation1.Id),
                                         Duration:           TimeSpan.FromSeconds(1),
                                         EVSEId:             EVSE_Id.Parse(1),
                                         ChargingRateUnit:   ChargingRateUnits.Watts,
                                         CustomData:         null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,            response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                        getCompositeScheduleRequests.Count);

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

                var updateDynamicScheduleRequests = new ConcurrentList<UpdateDynamicScheduleRequest>();

                chargingStation1.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp, sender, connection, updateDynamicScheduleRequest, ct) => {
                    updateDynamicScheduleRequests.TryAdd(updateDynamicScheduleRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.UpdateDynamicSchedule(

                                         Destination:    SourceRouting.To(       chargingStation1.Id),

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


                ClassicAssert.AreEqual(ResultCode.OK,                    response.Result.ResultCode);
                ClassicAssert.AreEqual(ChargingProfileStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                updateDynamicScheduleRequests.Count);

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

                var notifyAllowedEnergyTransferRequests = new ConcurrentList<NotifyAllowedEnergyTransferRequest>();

                chargingStation1.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp, sender, connection, notifyAllowedEnergyTransferRequest, ct) => {
                    notifyAllowedEnergyTransferRequests.TryAdd(notifyAllowedEnergyTransferRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.NotifyAllowedEnergyTransfer(
                                         Destination:    SourceRouting.To(              chargingStation1.Id),
                                         AllowedEnergyTransferModes:   [
                                                                           EnergyTransferMode.AC_SinglePhase,
                                                                           EnergyTransferMode.AC_ThreePhases
                                                                       ],
                                         CustomData:                   null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,                                response.Result.ResultCode);
                ClassicAssert.AreEqual(NotifyAllowedEnergyTransferStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                                            notifyAllowedEnergyTransferRequests.Count);

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

                var usePriorityChargingRequests = new ConcurrentList<UsePriorityChargingRequest>();

                chargingStation1.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp, sender, connection, usePriorityChargingRequest, ct) => {
                    usePriorityChargingRequests.TryAdd(usePriorityChargingRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.UsePriorityCharging(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         TransactionId:   Transaction_Id.Parse("1234"),
                                         Activate:        true,
                                         CustomData:      null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,            response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                        usePriorityChargingRequests.Count);

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

                var unlockConnectorRequests = new ConcurrentList<UnlockConnectorRequest>();

                chargingStation1.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp, sender, connection, unlockConnectorRequest, ct) => {
                    unlockConnectorRequests.TryAdd(unlockConnectorRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.UnlockConnector(
                                         Destination:    SourceRouting.To( chargingStation1.Id),
                                         EVSEId:          chargingStation1.EVSEs.First().Id,
                                         ConnectorId:     chargingStation1.EVSEs.First().Connectors.First().Id,
                                         CustomData:      null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,           response.Result.ResultCode);
                ClassicAssert.AreEqual(UnlockStatus.Unlocked,   response.Status);

                ClassicAssert.AreEqual(1,                       unlockConnectorRequests.Count);

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

                var afrrSignalRequestRequests = new ConcurrentList<AFRRSignalRequest>();

                chargingStation1.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp, sender, connection, afrrSignalRequestRequest, ct) => {
                    afrrSignalRequestRequests.TryAdd(afrrSignalRequestRequest);
                    return Task.CompletedTask;
                };

                var response = await testCSMS01.SendAFRRSignal(
                                         Destination:    SourceRouting.To(       chargingStation1.Id),
                                         ActivationTimestamp:   Timestamp.Now,
                                         Signal:                AFRR_Signal.Parse(-1),
                                         CustomData:            null
                                     );


                ClassicAssert.AreEqual(ResultCode.OK,            response.Result.ResultCode);
                ClassicAssert.AreEqual(GenericStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                        afrrSignalRequestRequests.Count);

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

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp, sender, connection, setDisplayMessageRequest, ct) => {
                    setDisplayMessageRequests.TryAdd(setDisplayMessageRequest);
                    return Task.CompletedTask;
                };

                var message   = RandomExtensions.RandomString(10);

                var response  = await testCSMS01.SetDisplayMessage(
                                          Destination:    SourceRouting.To( chargingStation1.Id),
                                          Message:         new MessageInfo(
                                                               Id:               DisplayMessage_Id.NewRandom,
                                                               Priority:         MessagePriority.AlwaysFront,
                                                               Message:          new MessageContent(
                                                                                     Content:      message,
                                                                                     Format:       MessageFormat.UTF8,
                                                                                     Language:     Language_Id.Parse("de"),
                                                                                     CustomData:   null
                                                                                 ),
                                                               State:            MessageState.Charging,
                                                               StartTimestamp:   Timestamp.Now,
                                                               EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                               TransactionId:    null,
                                                               CustomData:       null
                                                           ),
                                          CustomData:      null
                                      );


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);
                //ClassicAssert.AreEqual(data.Reverse(),        response1.Data?.ToString());

                ClassicAssert.AreEqual(1,                     setDisplayMessageRequests.Count);
                //ClassicAssert.AreEqual(vendorId,               dataTransferRequests.First().VendorId);
                //ClassicAssert.AreEqual(messageId,              dataTransferRequests.First().MessageId);
                //ClassicAssert.AreEqual(data,                   dataTransferRequests.First().Data?.ToString());

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
                csms1WebSocketJSONMessagesSent                         is not null &&
                csms1WebSocketJSONMessageResponsesReceived             is not null &&
                csms1WebSocketJSONMessagesReceived                     is not null &&
                csms1WebSocketJSONMessageResponsesSent                 is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketJSONMessagesReceived         is not null &&
                chargingStation1WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation1WebSocketJSONMessagesSent             is not null &&
                chargingStation1WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketJSONMessagesReceived         is not null &&
                chargingStation2WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation2WebSocketJSONMessagesSent             is not null &&
                chargingStation2WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketJSONMessagesReceived         is not null &&
                chargingStation3WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation3WebSocketJSONMessagesSent             is not null &&
                chargingStation3WebSocketJSONMessageResponsesReceived is not null)

            {

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp, sender, connection, setDisplayMessageRequest, ct) => {
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
                                                 Destination:    SourceRouting.To( chargingStation1.Id),
                                                 Message:         new MessageInfo(
                                                                      Id:               messageIds[i-1],
                                                                      Priority:         i > 7 ? MessagePriority.AlwaysFront : MessagePriority.NormalCycle,
                                                                      Message:          new MessageContent(
                                                                                            Content:      $"{i}:{setMessage}",
                                                                                            Format:       MessageFormat.UTF8,
                                                                                            Language:     Language_Id.Parse("de"),
                                                                                            CustomData:   null
                                                                                        ),
                                                                      State:            i > 5 ? MessageState.Charging : MessageState.Idle,
                                                                      StartTimestamp:   Timestamp.Now,
                                                                      EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                                      TransactionId:    null,
                                                                      CustomData:       null
                                                                  ),
                                                 CustomData:      null
                                             );

                    ClassicAssert.AreEqual(ResultCode.OK,    setResponse.Result.ResultCode);
                    ClassicAssert.AreEqual(i,                setDisplayMessageRequests.Count);

                }


                ClassicAssert.AreEqual(10, csms1WebSocketJSONMessagesSent.                        Count);
                ClassicAssert.AreEqual(10, csms1WebSocketJSONMessageResponsesReceived.            Count);
                ClassicAssert.AreEqual(10, csms1WebSocketJSONMessagesReceived.                    Count);
                ClassicAssert.AreEqual( 0, csms1WebSocketJSONMessageResponsesSent.                Count);

                ClassicAssert.AreEqual(10, chargingStation1WebSocketJSONMessagesReceived.        Count);
                ClassicAssert.AreEqual(10, chargingStation1WebSocketJSONMessageResponsesSent.    Count);
                ClassicAssert.AreEqual(10, chargingStation1WebSocketJSONMessagesSent.            Count);
                ClassicAssert.AreEqual( 0, chargingStation1WebSocketJSONMessageResponsesReceived.Count);

                await Task.Delay(500);


                var getDisplayMessagesRequests     = new ConcurrentList<GetDisplayMessagesRequest>();
                var notifyDisplayMessagesRequests  = new ConcurrentList<CS.NotifyDisplayMessagesRequest>();

                chargingStation1.OCPP.IN. OnGetDisplayMessagesRequestReceived    += (timestamp, sender, connection, getDisplayMessagesRequest, ct) => {
                    getDisplayMessagesRequests.   TryAdd(getDisplayMessagesRequest);
                    return Task.CompletedTask;
                };

                testCSMS01.      OCPP.IN. OnNotifyDisplayMessagesRequestReceived += (timestamp, sender, connection, notifyDisplayMessagesRequest, ct) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
                };


                var getResponse1  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   1,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK, getResponse1.Result.ResultCode);
                ClassicAssert.AreEqual(1, getDisplayMessagesRequests.Count);

                await Task.Delay(500);


                ClassicAssert.AreEqual(12, csms1WebSocketJSONMessagesSent.                       Count);
                ClassicAssert.AreEqual(11, csms1WebSocketJSONMessageResponsesReceived.           Count);
                ClassicAssert.AreEqual(12, csms1WebSocketJSONMessagesReceived.                   Count);
                ClassicAssert.AreEqual( 1, csms1WebSocketJSONMessageResponsesSent.               Count);

                ClassicAssert.AreEqual(12, chargingStation1WebSocketJSONMessagesReceived.        Count);
                ClassicAssert.AreEqual(11, chargingStation1WebSocketJSONMessageResponsesSent.    Count);
                ClassicAssert.AreEqual(12, chargingStation1WebSocketJSONMessagesSent.            Count);
                ClassicAssert.AreEqual( 1, chargingStation1WebSocketJSONMessageResponsesReceived.Count);


                var getResponse2  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   2,
                                              Ids:                           [
                                                                                 messageIds[0],
                                                                                 messageIds[2],
                                                                                 messageIds[4]
                                                                             ],
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK, getResponse2.Result.ResultCode);
                ClassicAssert.AreEqual(2, getDisplayMessagesRequests.Count);

                await Task.Delay(500);


                var getResponse3  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   3,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         MessageState.Charging,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK, getResponse3.Result.ResultCode);
                ClassicAssert.AreEqual(3, getDisplayMessagesRequests.Count);

                await Task.Delay(500);


                var getResponse4  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   4,
                                              Ids:                           null,
                                              Priority:                      MessagePriority.AlwaysFront,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK, getResponse4.Result.ResultCode);
                ClassicAssert.AreEqual(4, getDisplayMessagesRequests.Count);

                await Task.Delay(500);


                ClassicAssert.AreEqual(4, notifyDisplayMessagesRequests.Count);

                ClassicAssert.AreEqual(10, notifyDisplayMessagesRequests[0].MessageInfos.Count());
                ClassicAssert.AreEqual( 3, notifyDisplayMessagesRequests[1].MessageInfos.Count());
                ClassicAssert.AreEqual( 5, notifyDisplayMessagesRequests[2].MessageInfos.Count());
                ClassicAssert.AreEqual( 3, notifyDisplayMessagesRequests[3].MessageInfos.Count());

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
                csms1WebSocketJSONMessagesSent                        is not null &&
                csms1WebSocketJSONMessageResponsesReceived            is not null &&
                csms1WebSocketJSONMessagesReceived                    is not null &&
                csms1WebSocketJSONMessageResponsesSent                is not null &&

                chargingStation1                                      is not null &&
                chargingStation1WebSocketJSONMessagesReceived         is not null &&
                chargingStation1WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation1WebSocketJSONMessagesSent             is not null &&
                chargingStation1WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation2                                      is not null &&
                chargingStation2WebSocketJSONMessagesReceived         is not null &&
                chargingStation2WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation2WebSocketJSONMessagesSent             is not null &&
                chargingStation2WebSocketJSONMessageResponsesReceived is not null &&

                chargingStation3                                      is not null &&
                chargingStation3WebSocketJSONMessagesReceived         is not null &&
                chargingStation3WebSocketJSONMessageResponsesSent     is not null &&
                chargingStation3WebSocketJSONMessagesSent             is not null &&
                chargingStation3WebSocketJSONMessageResponsesReceived is not null)

            {

                var setDisplayMessageRequests = new ConcurrentList<SetDisplayMessageRequest>();

                chargingStation1.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp, sender, connection, setDisplayMessageRequest, ct) => {
                    setDisplayMessageRequests.TryAdd(setDisplayMessageRequest);
                    return Task.CompletedTask;
                };

                var messageId1    = DisplayMessage_Id.NewRandom;
                var message1      = RandomExtensions.RandomString(10);

                var setResponse1  = await testCSMS01.SetDisplayMessage(
                                              Destination:    SourceRouting.To( chargingStation1.Id),
                                              Message:         new MessageInfo(
                                                                   Id:               messageId1,
                                                                   Priority:         MessagePriority.AlwaysFront,
                                                                   Message:          new MessageContent(
                                                                                         Content:      message1,
                                                                                         Format:       MessageFormat.UTF8,
                                                                                         Language:     Language_Id.Parse("de"),
                                                                                         CustomData:   null
                                                                                     ),
                                                                   State:            MessageState.Charging,
                                                                   StartTimestamp:   Timestamp.Now,
                                                                   EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                                   TransactionId:    null,
                                                                   CustomData:       null
                                                               ),
                                              CustomData:      null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK,   setResponse1.Result.ResultCode);
                ClassicAssert.AreEqual(1,               setDisplayMessageRequests.Count);


                var messageId2    = DisplayMessage_Id.NewRandom;
                var message2      = RandomExtensions.RandomString(10);

                var setResponse2  = await testCSMS01.SetDisplayMessage(
                                              Destination:    SourceRouting.To( chargingStation1.Id),
                                              Message:         new MessageInfo(
                                                                   Id:               messageId2,
                                                                   Priority:         MessagePriority.AlwaysFront,
                                                                   Message:          new MessageContent(
                                                                                         Content:      message2,
                                                                                         Format:       MessageFormat.UTF8,
                                                                                         Language:     Language_Id.Parse("de"),
                                                                                         CustomData:   null
                                                                                     ),
                                                                   State:            MessageState.Charging,
                                                                   StartTimestamp:   Timestamp.Now,
                                                                   EndTimestamp:     Timestamp.Now + TimeSpan.FromDays(1),
                                                                   TransactionId:    null,
                                                                   CustomData:       null
                                                               ),
                                              CustomData:      null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK,   setResponse2.Result.ResultCode);
                ClassicAssert.AreEqual(2,               setDisplayMessageRequests.Count);


                // Get Messages BEFORE
                var getDisplayMessagesRequests = new ConcurrentList<GetDisplayMessagesRequest>();

                chargingStation1.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp, sender, connection, getDisplayMessagesRequest, ct) => {
                    getDisplayMessagesRequests.TryAdd(getDisplayMessagesRequest);
                    return Task.CompletedTask;
                };


                var notifyDisplayMessagesRequests = new ConcurrentList<CS.NotifyDisplayMessagesRequest>();

                testCSMS01.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp, sender, connection, notifyDisplayMessagesRequest, ct) => {
                    notifyDisplayMessagesRequests.TryAdd(notifyDisplayMessagesRequest);
                    return Task.CompletedTask;
                };


                var getResponse1  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   1,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK,   getResponse1.Result.ResultCode);
                ClassicAssert.AreEqual(1,               getDisplayMessagesRequests.Count);


                await Task.Delay(500);


                // Delete message #1
                var clearDisplayMessageRequests = new ConcurrentList<ClearDisplayMessageRequest>();

                chargingStation1.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp, sender, connection, clearDisplayMessageRequest, ct) => {
                    clearDisplayMessageRequests.TryAdd(clearDisplayMessageRequest);
                    return Task.CompletedTask;
                };

                var clearResponse  = await testCSMS01.ClearDisplayMessage(
                                               Destination:    SourceRouting.To(     chargingStation1.Id),
                                               DisplayMessageId:    messageId1,
                                               CustomData:          null
                                           );

                ClassicAssert.AreEqual(ResultCode.OK,   clearResponse.Result.ResultCode);
                ClassicAssert.AreEqual(1,               clearDisplayMessageRequests.Count);


                await Task.Delay(500);


                // Get Messages AFTER
                var getResponse2  = await testCSMS01.GetDisplayMessages(
                                              Destination:    SourceRouting.To(               chargingStation1.Id),
                                              GetDisplayMessagesRequestId:   2,
                                              Ids:                           null,
                                              Priority:                      null,
                                              State:                         null,
                                              CustomData:                    null
                                          );

                ClassicAssert.AreEqual(ResultCode.OK,   getResponse2.Result.ResultCode);
                ClassicAssert.AreEqual(2,               getDisplayMessagesRequests.Count);


                await Task.Delay(500);


                ClassicAssert.AreEqual(2,                notifyDisplayMessagesRequests[0].MessageInfos.Count());
                ClassicAssert.AreEqual(1,                notifyDisplayMessagesRequests[1].MessageInfos.Count());


                ClassicAssert.AreEqual(7, csms1WebSocketJSONMessagesSent.                        Count);
                ClassicAssert.AreEqual(5, csms1WebSocketJSONMessageResponsesReceived.            Count);
                ClassicAssert.AreEqual(7, csms1WebSocketJSONMessagesReceived.                    Count);
                ClassicAssert.AreEqual(2, csms1WebSocketJSONMessageResponsesSent.                Count);

                ClassicAssert.AreEqual(7, chargingStation1WebSocketJSONMessagesReceived.        Count);
                ClassicAssert.AreEqual(5, chargingStation1WebSocketJSONMessageResponsesSent.    Count);
                ClassicAssert.AreEqual(7, chargingStation1WebSocketJSONMessagesSent.            Count);
                ClassicAssert.AreEqual(2, chargingStation1WebSocketJSONMessageResponsesReceived.Count);

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

                var costUpdatedRequests = new ConcurrentList<CostUpdatedRequest>();

                chargingStation1.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp, sender, connection, costUpdatedRequest, ct) => {
                    costUpdatedRequests.TryAdd(costUpdatedRequest);
                    return Task.CompletedTask;
                };

                var message   = RandomExtensions.RandomString(10);

                var response  = await testCSMS01.SendCostUpdated(
                                          Destination:    SourceRouting.To( chargingStation1.Id),
                                          TotalCost:       1.02M,
                                          TransactionId:   Transaction_Id.NewRandom,
                                          CustomData:      null
                                      );


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);
                //ClassicAssert.AreEqual(data.Reverse(),        response.Data?.ToString());

                ClassicAssert.AreEqual(1,                     costUpdatedRequests.Count);
                //ClassicAssert.AreEqual(vendorId,              dataTransferRequests.First().VendorId);
                //ClassicAssert.AreEqual(messageId,             dataTransferRequests.First().MessageId);
                //ClassicAssert.AreEqual(data,                  dataTransferRequests.First().Data?.ToString());

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

                var customerInformationRequests        = new ConcurrentList<CustomerInformationRequest>();
                var notifyCustomerInformationRequests  = new ConcurrentList<CS.NotifyCustomerInformationRequest>();

                chargingStation1.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp, sender, connection, customerInformationRequest, ct) => {
                    customerInformationRequests.      TryAdd(customerInformationRequest);
                    return Task.CompletedTask;
                };

                testCSMS01.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp, sender, connection, notifyCustomerInformationRequest, ct) => {
                    notifyCustomerInformationRequests.TryAdd(notifyCustomerInformationRequest);
                    return Task.CompletedTask;
                };


                var response = await testCSMS01.RequestCustomerInformation(
                                         Destination:    SourceRouting.To(                chargingStation1.Id),
                                         CustomerInformationRequestId:   1,
                                         Report:                         true,
                                         Clear:                          false,
                                         CustomerIdentifier:             CustomerIdentifier.Parse("123"),
                                         IdToken:                        new IdToken(
                                                                             Value:                 "aabbccdd",
                                                                             Type:                  IdTokenType.ISO14443,
                                                                             AdditionalInfos:       [
                                                                                                        new AdditionalInfo(
                                                                                                            AdditionalIdToken:   "1234",
                                                                                                            Type:                "PIN",
                                                                                                            CustomData:          null
                                                                                                        )
                                                                                                    ],
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


                ClassicAssert.AreEqual(ResultCode.OK,         response.Result.ResultCode);

                ClassicAssert.AreEqual(1,                     customerInformationRequests.Count);


                await Task.Delay(500);


                ClassicAssert.AreEqual(1,                     notifyCustomerInformationRequests.Count);

            }

        }

        #endregion


    }

}
