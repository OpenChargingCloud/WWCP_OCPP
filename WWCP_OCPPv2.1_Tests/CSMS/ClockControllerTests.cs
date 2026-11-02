/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The standard clock controller, as a CSMS reads it: under the name the
    /// standard gives it, NtpServerUri by priority, read from its source at
    /// every read, and nothing a CSMS may change yet.
    /// </summary>
    /// <remarks>
    /// The test station has the clock controller of an InMemoryClockSource:
    /// a clock set by NTS from two servers, with neither a time zone nor a
    /// time offset. The tests that ask only over the wire name the component
    /// by its name, so that they can run against a station of any version.
    /// </remarks>
    [TestFixture]
    public class ClockControllerTests : AChargingStationTests
    {

        #region GetVariables_TheStandardName_Test()

        /// <summary>
        /// A CSMS finds the clock under the name the standard gives it,
        /// ClockCtrlr, and nowhere else: its time, where that comes from, and
        /// the NTP servers by priority - an empty one where there is no server.
        /// </summary>
        [Test]
        public async Task GetVariables_TheStandardName_Test()
        {

            var results = await GetVariables(("ClockCtrlr",       "DateTime",      null),
                                             ("ClockCtrlr",       "TimeSource",    null),
                                             ("ClockCtrlr",       "NtpSource",     null),
                                             ("ClockCtrlr",       "NtpServerUri",  "1"),
                                             ("ClockCtrlr",       "NtpServerUri",  "2"),
                                             ("ClockCtrlr",       "NtpServerUri",  "3"),
                                             ("ClockController",  "DateTime",      null));

            // The value of DateTime is not compared: the JSON of a message is read
            // with Newtonsoft's defaults, which take a string that looks like a
            // time for a time, and the CSMS reads "2026-11-02T12:00:00.000Z"
            // back as "11/02/2026 12:00:00" - see NTS-DeviceModel-Concept.md B.16.
            Assert.Multiple(() => {
                Assert.That(results.Select(result => result.AttributeStatus),                   Is.EqualTo(new[] { GetVariableStatus.Accepted, GetVariableStatus.Accepted, GetVariableStatus.Accepted,
                                                                                                                   GetVariableStatus.Accepted, GetVariableStatus.Accepted, GetVariableStatus.Accepted,
                                                                                                                   GetVariableStatus.UnknownComponent }));
                Assert.That(results.Skip(1).Take(5).Select(result => result.AttributeValue),   Is.EqualTo(new[] { "NTP,RealTimeClock", "manual",
                                                                                                                   "nts://ptbtime1.ptb.de", "nts://ptbtime2.ptb.de:4461", "" }));
            });

        }

        #endregion

        #region GetVariables_WhatTheSourceCannotAnswerIsUnknown_Test()

        /// <summary>
        /// A variable of the standard that the source cannot answer is an
        /// unknown variable, not an empty value; so is NtpServerUri without a
        /// priority, or with one beyond 9.
        /// </summary>
        [Test]
        public async Task GetVariables_WhatTheSourceCannotAnswerIsUnknown_Test()
        {

            var results = await GetVariables(("ClockCtrlr",  "TimeZone",                          null),
                                             ("ClockCtrlr",  "TimeOffset",                        null),
                                             ("ClockCtrlr",  "TimeOffset",                        "NextTransition"),
                                             ("ClockCtrlr",  "TimeAdjustmentReportingThreshold",  null),
                                             ("ClockCtrlr",  "NtpServerUri",                      null),
                                             ("ClockCtrlr",  "NtpServerUri",                      "10"));

            Assert.That(results.Select(result => result.AttributeStatus),  Is.All.EqualTo(GetVariableStatus.UnknownVariable));

        }

        #endregion

        #region GetVariables_ReadsTheSourceAtEveryRead_Test()

        /// <summary>
        /// GetVariables answers what the source says at that moment: a server
        /// put in front of the others, and the clock no longer set by NTP.
        /// </summary>
        [Test]
        public async Task GetVariables_ReadsTheSourceAtEveryRead_Test()
        {

            clockSource1!.NtpServers.Insert(0, "nts://time.example.org");
            clockSource1. Values["TimeSource"] = "RealTimeClock";

            var results = await GetVariables(("ClockCtrlr",  "NtpServerUri",  "1"),
                                             ("ClockCtrlr",  "NtpServerUri",  "3"),
                                             ("ClockCtrlr",  "TimeSource",    null));

            Assert.That(results.Select(result => result.AttributeValue),  Is.EqualTo(new[] { "nts://time.example.org", "nts://ptbtime2.ptb.de:4461", "RealTimeClock" }));

        }

        #endregion

        #region SetVariables_IsReadOnlyForNow_Test()

        /// <summary>
        /// Until the setters exist, every variable is read-only: a CSMS is told
        /// so, and the value stays as it was.
        /// </summary>
        [Test]
        public async Task SetVariables_IsReadOnlyForNow_Test()
        {

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [
                                                         new SetVariableData(
                                                             Component:        new Component(Name: "ClockCtrlr"),
                                                             Variable:         new Variable (Name: "TimeSource"),
                                                             AttributeValue:   "Heartbeat"
                                                         ),
                                                         new SetVariableData(
                                                             Component:        new Component(Name: "ClockCtrlr"),
                                                             Variable:         new Variable (Name: "NtpServerUri", Instance: "1"),
                                                             AttributeValue:   "nts://time.example.org"
                                                         )
                                                     ]
                                 );

            var after    = await GetVariables(("ClockCtrlr",  "NtpServerUri",  "1"));

            Assert.Multiple(() => {
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatus),                Is.All.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("ReadOnly"));
                Assert.That(after.Single().AttributeValue,                                                         Is.EqualTo("nts://ptbtime1.ptb.de"));
            });

        }

        #endregion

        #region GetBaseReport_ListsClockCtrlrWithItsCharacteristics_Test()

        /// <summary>
        /// A full inventory lists ClockCtrlr with what its source answers: one
        /// NtpServerUri per priority from 1 to 9, the standard's characteristics,
        /// and every variable read-only for now.
        /// </summary>
        [Test]
        public async Task GetBaseReport_ListsClockCtrlrWithItsCharacteristics_Test()
        {

            var parts       = NotifyReportsOf(5811);
            var response    = await testCSMS1!.GetBaseReport(
                                        Destination:              SourceRouting.To(chargingStation1!.Id),
                                        GetBaseReportRequestId:   5811,
                                        ReportBase:               ReportBase.FullInventory
                                    );
            var report      = await CompleteReport(parts);

            var clock       = report.Where(reportData => reportData.Component.Name == "ClockCtrlr").ToList();
            var servers     = clock. Where(reportData => reportData.Variable. Name == "NtpServerUri").ToList();
            var timeSource  = clock. Single(reportData => reportData.Variable.Name == "TimeSource");
            var ntpSource   = clock. Single(reportData => reportData.Variable.Name == "NtpSource");

            Assert.Multiple(() => {

                Assert.That(response.Status,                                                                Is.EqualTo(GenericDeviceModelStatus.Accepted));

                Assert.That(clock.Select(reportData => reportData.Variable.Name).Distinct(),               Is.EquivalentTo(new[] { "DateTime", "NtpServerUri", "NtpSource", "TimeSource" }));
                Assert.That(servers.Select(reportData => reportData.Variable.Instance),                    Is.EqualTo(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }));
                Assert.That(servers.Select(reportData => reportData.VariableAttributes.Single().Value),    Is.EqualTo(new[] { "nts://ptbtime1.ptb.de", "nts://ptbtime2.ptb.de:4461", "", "", "", "", "", "", "" }));
                Assert.That(clock.Select(reportData => reportData.VariableAttributes.Single().Mutability), Is.All.EqualTo(MutabilityTypes.ReadOnly));

                Assert.That(timeSource.VariableCharacteristics?.DataType,                                  Is.EqualTo(DataTypes.SequenceList));
                Assert.That(timeSource.VariableCharacteristics?.ValuesList,                                Is.EqualTo(new[] { "Heartbeat", "NTP", "GPS", "RealTimeClock", "MobileNetwork", "RadioTimeTransmitter" }));
                Assert.That(ntpSource. VariableCharacteristics?.ValuesList,                                Is.EqualTo(new[] { "DHCP", "manual" }));

            });

        }

        #endregion


        // ---- With the in-memory source --------------------------------------

        #region TheStationFindsItsClockController_Test()

        [Test]
        public void TheStationFindsItsClockController_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation1!.ClockControllers().Count(),  Is.EqualTo(1));
                Assert.That(chargingStation1. ClockControllers().Single().Name,  Is.EqualTo(ClockController.ComponentName));
            });

        }

        #endregion


        #region (private) GetVariables(params Asked)

        private async Task<IReadOnlyList<GetVariableResult>> GetVariables(params (String Component, String Variable, String? Instance)[] Asked)
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [.. Asked.Select(asked => new GetVariableData(
                                                                                   Component:   new Component(Name: asked.Component),
                                                                                   Variable:    new Variable (Name: asked.Variable, Instance: asked.Instance)
                                                                               ))]
                                 );

            return [.. response.Results];

        }

        #endregion

        #region (private) NotifyReportsOf(RequestId)

        /// <summary>
        /// Collect the NotifyReport requests the CSMS receives for the given report.
        /// </summary>
        private ConcurrentList<NotifyReportRequest> NotifyReportsOf(Int32 RequestId)
        {

            var parts = new ConcurrentList<NotifyReportRequest>();

            testCSMS1!.OCPP.IN.OnNotifyReportRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {

                if (notifyReportRequest.NotifyReportRequestId == RequestId)
                    parts.TryAdd(notifyReportRequest);

                return Task.CompletedTask;

            };

            return parts;

        }

        #endregion

        #region (private static) CompleteReport(Parts)

        /// <summary>
        /// Wait for the last part of a report, and return all of its report data.
        /// </summary>
        private static async Task<List<ReportData>> CompleteReport(ConcurrentList<NotifyReportRequest> Parts)
        {

            var deadline = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);

            while (DateTimeOffset.UtcNow < deadline &&
                   !(Parts.Any(part => part.ToBeContinued != true) &&
                     Parts.Count == Parts.Max(part => part.SequenceNumber) + 1))
            {
                await Task.Delay(20);
            }

            Assert.That(Parts.Any(part => part.ToBeContinued != true), Is.True, "The last part of the report never came!");

            return [.. Parts.OrderBy(part => part.SequenceNumber).SelectMany(part => part.ReportData)];

        }

        #endregion

    }

}
