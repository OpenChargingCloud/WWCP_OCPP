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
    /// The NTP controllers of the Secure Time Synchronization white paper, as a
    /// CSMS reads them: the variables of the paper, read from their source at
    /// every read, the variables of legal time in the legal group only, no
    /// clock variables, and nothing a CSMS may change yet.
    /// </summary>
    /// <remarks>
    /// The test station has the controllers of an InMemoryNTPSource: a "legal"
    /// group of ptb1 to ptb4 and a "local" group of lc1 and lc2. The tests that
    /// ask only over the wire name the components by their names, so that
    /// they can run against a station of any version.
    /// </remarks>
    [TestFixture]
    public class NTPControllerTests : AChargingStationTests
    {

        #region GetVariables_TheVariablesOfThePaper_Test()

        /// <summary>
        /// A CSMS reads the variables of the paper: the groups, the clients of a
        /// group, the quorum, the authority of legal time, the server of a
        /// client and its mode.
        /// </summary>
        [Test]
        public async Task GetVariables_TheVariablesOfThePaper_Test()
        {

            var results = await GetVariables(("NTPController",             null,     "ClientGroups"),
                                             ("NTPClientGroupController",  "legal",  "Clients"),
                                             ("NTPClientGroupController",  "legal",  "MinServers"),
                                             ("NTPClientGroupController",  "legal",  "MaxDeviation"),
                                             ("NTPClientGroupController",  "legal",  "LegalTimeAuthority"),
                                             ("NTPClientController",       "ptb1",   "ServerURL"),
                                             ("NTPClientController",       "ptb1",   "Mode"),
                                             ("NTPClientController",       "lc2",    "ServerURL"));

            Assert.Multiple(() => {
                Assert.That(results.Select(result => result.AttributeStatus),  Is.All.EqualTo(GetVariableStatus.Accepted));
                Assert.That(results.Select(result => result.AttributeValue),   Is.EqualTo(new[] { "legal,local", "ptb1,ptb2,ptb3,ptb4", "2", "60", "PTB", "ptbtime1.ptb.de", "NTSv4", "lc2.local" }));
            });

        }

        #endregion

        #region GetVariables_LegalTimeOnlyInTheLegalGroup_Test()

        /// <summary>
        /// The variables of legal time exist in the legal group only, even where
        /// the source would have a value for another group.
        /// </summary>
        [Test]
        public async Task GetVariables_LegalTimeOnlyInTheLegalGroup_Test()
        {

            var results = await GetVariables(("NTPClientGroupController",  "legal",  "LegalTimeTolerance"),
                                             ("NTPClientGroupController",  "legal",  "LegalTimeMaxAge"),
                                             ("NTPClientGroupController",  "local",  "MaxErrors"),
                                             ("NTPClientGroupController",  "local",  "LegalTimeAuthority"),
                                             ("NTPClientGroupController",  "local",  "LegalTimeTolerance"));

            Assert.Multiple(() => {
                Assert.That(results.Select(result => result.AttributeStatus),  Is.EqualTo(new[] { GetVariableStatus.Accepted,      GetVariableStatus.Accepted,      GetVariableStatus.Accepted,
                                                                                                  GetVariableStatus.UnknownVariable, GetVariableStatus.UnknownVariable }));
                Assert.That(results.Take(3).Select(result => result.AttributeValue),  Is.EqualTo(new[] { "1", "3600", "50" }));
            });

        }

        #endregion

        #region GetVariables_NoClockVariablesInTheNTPControllers_Test()

        /// <summary>
        /// The clock belongs to ClockCtrlr: a group or a client of time servers
        /// has no date, time zone or time offset of its own.
        /// </summary>
        [Test]
        public async Task GetVariables_NoClockVariablesInTheNTPControllers_Test()
        {

            var results = await GetVariables(("NTPClientGroupController",  "legal",  "DateTime"),
                                             ("NTPClientGroupController",  "legal",  "TimeZone"),
                                             ("NTPClientGroupController",  "legal",  "TimeOffset"),
                                             ("NTPClientGroupController",  "legal",  "TimeAdjustmentReportingThreshold"),
                                             ("NTPClientController",       "ptb1",   "DateTime"),
                                             ("NTPClientController",       "ptb1",   "TimeOffset"));

            Assert.That(results.Select(result => result.AttributeStatus),  Is.All.EqualTo(GetVariableStatus.UnknownVariable));

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
                                                             Component:        new Component(Name: "NTPClientGroupController", Instance: "legal"),
                                                             Variable:         new Variable (Name: "MinServers"),
                                                             AttributeValue:   "3"
                                                         ),
                                                         new SetVariableData(
                                                             Component:        new Component(Name: "NTPClientController",      Instance: "ptb1"),
                                                             Variable:         new Variable (Name: "ServerURL"),
                                                             AttributeValue:   "time.example.org"
                                                         ),
                                                         new SetVariableData(
                                                             Component:        new Component(Name: "NTPController"),
                                                             Variable:         new Variable (Name: "ClientGroups"),
                                                             AttributeValue:   "legal"
                                                         )
                                                     ]
                                 );

            var after    = await GetVariables(("NTPClientGroupController",  "legal",  "MinServers"));

            Assert.Multiple(() => {
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatus),                Is.All.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("ReadOnly"));
                Assert.That(after.Single().AttributeValue,                                                         Is.EqualTo("2"));
            });

        }

        #endregion

        #region GetBaseReport_ListsTheNTPComponentsWithTheirCharacteristics_Test()

        /// <summary>
        /// A full inventory lists the NTP controller, both groups and all six
        /// clients, the variables of legal time in the legal group only, each
        /// variable read-only and with the characteristics of the paper; a
        /// configuration inventory lists none of them while they are read-only.
        /// </summary>
        [Test]
        public async Task GetBaseReport_ListsTheNTPComponentsWithTheirCharacteristics_Test()
        {

            var fullParts     = NotifyReportsOf(5801);
            var full          = await testCSMS1!.GetBaseReport(
                                          Destination:              SourceRouting.To(chargingStation1!.Id),
                                          GetBaseReportRequestId:   5801,
                                          ReportBase:               ReportBase.FullInventory
                                      );
            var fullReport    = await CompleteReport(fullParts);

            var configParts   = NotifyReportsOf(5802);
            var config        = await testCSMS1!.GetBaseReport(
                                          Destination:              SourceRouting.To(chargingStation1!.Id),
                                          GetBaseReportRequestId:   5802,
                                          ReportBase:               ReportBase.ConfigurationInventory
                                      );
            var configReport  = await CompleteReport(configParts);

            var ntp           = fullReport.Where(reportData => reportData.Component.Name.StartsWith("NTP")).ToList();
            var controller    = ntp.Where(reportData => reportData.Component.Name == "NTPController").ToList();
            var legal         = ntp.Where(reportData => reportData.Component.Name == "NTPClientGroupController" && reportData.Component.Instance == "legal").ToList();
            var local         = ntp.Where(reportData => reportData.Component.Name == "NTPClientGroupController" && reportData.Component.Instance == "local").ToList();
            var clients       = ntp.Where(reportData => reportData.Component.Name == "NTPClientController").ToList();

            var maxDeviation  = legal.  Single(reportData => reportData.Variable.Name == "MaxDeviation");
            var mode          = clients.First (reportData => reportData.Variable.Name == "Mode");

            Assert.Multiple(() => {

                Assert.That(full.Status,                                                                    Is.EqualTo(GenericDeviceModelStatus.Accepted));

                Assert.That(controller.Select(reportData => reportData.Variable.Name),                      Is.EquivalentTo(new[] { "ClientGroups", "SuggestedGroups", "LegalTime", "LegalTimeReason" }));
                Assert.That(legal.Count,                                                                    Is.EqualTo(16));
                Assert.That(local.Count,                                                                    Is.EqualTo(13));
                Assert.That(local.Any(reportData => reportData.Variable.Name.StartsWith("LegalTime")),      Is.False);
                Assert.That(clients.Select(reportData => reportData.Component.Instance).Distinct(),         Is.EquivalentTo(new[] { "ptb1", "ptb2", "ptb3", "ptb4", "lc1", "lc2" }));
                Assert.That(clients.Count,                                                                  Is.EqualTo(6 * 17));

                Assert.That(ntp.Select(reportData => reportData.VariableAttributes.Single().Mutability),    Is.All.EqualTo(MutabilityTypes.ReadOnly));

                Assert.That(maxDeviation.VariableAttributes.Single().Value,                                 Is.EqualTo("60"));
                Assert.That(maxDeviation.VariableCharacteristics?.DataType,                                 Is.EqualTo(DataTypes.Decimal));
                Assert.That(maxDeviation.VariableCharacteristics?.MinLimit,                                 Is.EqualTo(0.001m));
                Assert.That(maxDeviation.VariableCharacteristics?.MaxLimit,                                 Is.EqualTo(3600m));

                Assert.That(mode.VariableCharacteristics?.DataType,                                         Is.EqualTo(DataTypes.OptionList));
                Assert.That(mode.VariableCharacteristics?.ValuesList,                                       Is.EqualTo(new[] { "NTPv4", "NTSv4", "NTPv4TLS" }));

                Assert.That(config.Status,                                                                  Is.Not.EqualTo(GenericDeviceModelStatus.Rejected));
                Assert.That(configReport.Any(reportData => reportData.Component.Name.StartsWith("NTP")),    Is.False);

            });

        }

        #endregion


        // ---- With the in-memory source --------------------------------------

        #region GetVariables_ReadsTheSourceAtEveryRead_Test()

        /// <summary>
        /// GetVariables answers what the source says at that moment: a changed
        /// quorum, a client taken out of a group, legal time lost.
        /// </summary>
        [Test]
        public async Task GetVariables_ReadsTheSourceAtEveryRead_Test()
        {

            ntpSource1!.Group  ["legal"]["MinServers"]  = "3";
            ntpSource1. Members["legal"].Remove("ptb4");
            ntpSource1. Controller["LegalTime"]          = "false";
            ntpSource1. Controller["LegalTimeReason"]    = "Stale";

            var results = await GetVariables(("NTPClientGroupController",  "legal",  "MinServers"),
                                             ("NTPClientGroupController",  "legal",  "Clients"),
                                             ("NTPController",             null,     "LegalTime"),
                                             ("NTPController",             null,     "LegalTimeReason"));

            Assert.That(results.Select(result => result.AttributeValue),  Is.EqualTo(new[] { "3", "ptb1,ptb2,ptb3", "false", "Stale" }));

        }

        #endregion

        #region GetVariables_WhatTheSourceCannotAnswerIsUnknown_Test()

        /// <summary>
        /// A variable of the paper that the source cannot answer is an unknown
        /// variable, not an empty value, and a group the source does not have is
        /// an unknown component.
        /// </summary>
        [Test]
        public async Task GetVariables_WhatTheSourceCannotAnswerIsUnknown_Test()
        {

            var results = await GetVariables(("NTPClientGroupController",  "legal",  "Preflight"),
                                             ("NTPClientController",       "ptb1",   "NTSKEServerURL"),
                                             ("NTPClientController",       "ptb1",   "SignedResponses"),
                                             ("NTPClientGroupController",  "grid",   "Enabled"));

            Assert.That(results.Select(result => result.AttributeStatus),
                        Is.EqualTo(new[] { GetVariableStatus.UnknownVariable, GetVariableStatus.UnknownVariable, GetVariableStatus.UnknownVariable, GetVariableStatus.UnknownComponent }));

        }

        #endregion

        #region AllOf_OneControllerPerGroupAndClient_Test()

        /// <summary>
        /// AllOf makes the NTP controller, one controller per group and one per
        /// client; a client named by two groups belongs to the first, and a
        /// group or client the source does not have is refused.
        /// </summary>
        [Test]
        public void AllOf_OneControllerPerGroupAndClient_Test()
        {

            var source = new InMemoryNTPSource();
            source.Members["local"].Add("PTB1");

            var all    = NTPController.AllOf(source).ToList();

            Assert.Multiple(() => {

                Assert.That(all.OfType<NTPController>().Count(),                                       Is.EqualTo(1));
                Assert.That(all.OfType<NTPClientGroupController>().Select(group => group.Instance),    Is.EqualTo(new[] { "legal", "local" }));
                Assert.That(all.OfType<NTPClientController>().Select(client => client.Instance),       Is.EqualTo(new[] { "ptb1", "ptb2", "ptb3", "ptb4", "lc1", "lc2" }));
                Assert.That(all.OfType<NTPClientController>().First().Group,                           Is.EqualTo("legal"));

                Assert.That(new NTPClientGroupController(source, "LEGAL").IsLegal,                     Is.True);
                Assert.That(new NTPClientGroupController(source, "local").IsLegal,                     Is.False);

                Assert.That(() => new NTPClientGroupController(source, "grid"),                        Throws.ArgumentException);
                Assert.That(() => new NTPClientController     (source, "local", "ptb9"),               Throws.ArgumentException);

            });

        }

        #endregion

        #region TheStationFindsItsNTPControllers_Test()

        [Test]
        public void TheStationFindsItsNTPControllers_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation1!.NTPController,                              Is.Not.Null);
                Assert.That(chargingStation1. LegalTimeClientController?.IsLegal,         Is.True);
                Assert.That(chargingStation1. NetworkTimeClientController("lc1")?.Group,  Is.EqualTo("local"));
            });

        }

        #endregion


        #region (private) GetVariables(params Asked)

        private async Task<IReadOnlyList<GetVariableResult>> GetVariables(params (String Component, String? Instance, String Variable)[] Asked)
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [.. Asked.Select(asked => new GetVariableData(
                                                                                   Component:   new Component(Name: asked.Component, Instance: asked.Instance),
                                                                                   Variable:    new Variable (Name: asked.Variable)
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
