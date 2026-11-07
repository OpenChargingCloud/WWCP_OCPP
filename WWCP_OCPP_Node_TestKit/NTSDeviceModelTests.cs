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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.Node;

using cloud.charging.open.protocols.OCPP.Node;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What a CSMS reads of the node's time servers in the Device Model,
    /// against what the node's file and the node itself say.
    /// </summary>
    /// <remarks>
    /// Read from a node that was built and not started, so that nothing is
    /// ever asked of a time server: the values come from the configuration
    /// file, through the same JSON the web page reads. The servers are named
    /// under ".invalid", which no name server answers for.
    /// </remarks>
        public abstract class NTSDeviceModelTests : ABridgeTests
    {

        #region Data

        /// <summary>
        /// A root fingerprint as somebody might type it, in capitals and with
        /// colons, which the node keeps in small letters and without them.
        /// </summary>
        private const String  ARootAsTyped  = "9D:2E:4F:6A:8B:0C:1D:3E:5F:7A:9B:1C:3D:5E:7F:9A:0B:2C:4D:6E:8F:0A:1B:3C:5D:7E:9F:1A:2B:4C:6D:8E";
        private const String  ARoot         = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";

        private static readonly DateTimeOffset  Now  = new (2026, 11, 2, 12, 0, 0, 125, TimeSpan.FromHours(1));

        private String            directory  = "";
        private IBridgedNode?  station;
        private NTSDeviceModel    model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithALegalAndALocalGroup()
        {

            directory  = TestNodes.TemporaryDirectory("nts-device-model");
            station    = Nodes.New(directory, Configuration(LegalEnabled: false), Clock: new MovableClock(Now));
            model      = new NTSDeviceModel(station.Node);

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            TestNodes.Remove(directory);

        }

        #endregion


        #region TheGroupsAndTheirServersAreTheNodes()

        /// <summary>
        /// The groups are the node's, in the order of its file, and the servers
        /// of a group are named by their ids: the one the file gives, and the one
        /// the node gives by place to every other.
        /// </summary>
        [Test]
        public void TheGroupsAndTheirServersAreTheNodes()
        {

            Assert.Multiple(() => {
                Assert.That(model.Groups,               Is.EqualTo(new[] { "legal", "local" }));
                Assert.That(model.ClientsOf("legal"),   Is.EqualTo(new[] { "ptb-a", "legal-2", "legal-3", "legal-4" }));
                Assert.That(model.ClientsOf("LOCAL"),   Is.EqualTo(new[] { "local-1" }), "in any case");
                Assert.That(model.ClientsOf("grid"),    Is.Empty);
            });

        }

        #endregion

        #region AGroupReadsAsTheFileSaysIt()

        /// <summary>
        /// NTPClientGroupController says what the file says, with both intervals
        /// read from the one the node has, and nothing about a check that has
        /// not happened yet.
        /// </summary>
        [Test]
        public void AGroupReadsAsTheFileSaysIt()
        {

            Assert.Multiple(() => {

                Assert.That(model.GroupValue("legal", "Enabled"),             Is.EqualTo("false"));
                Assert.That(model.GroupValue("legal", "MinServers"),          Is.EqualTo("2"));
                Assert.That(model.GroupValue("legal", "MaxDeviation"),        Is.EqualTo("0.5"));
                Assert.That(model.GroupValue("legal", "MinInterval"),         Is.EqualTo("900"));
                Assert.That(model.GroupValue("legal", "MaxInterval"),         Is.EqualTo("900"));
                Assert.That(model.GroupValue("legal", "Timeout"),             Is.EqualTo("2.5"));
                Assert.That(model.GroupValue("legal", "ErrorLogging"),        Is.EqualTo("3"));
                Assert.That(model.GroupValue("legal", "MaxErrors"),           Is.EqualTo("10"));
                Assert.That(model.GroupValue("legal", "LegalTimeAuthority"),  Is.EqualTo("PTB"));
                Assert.That(model.GroupValue("legal", "LegalTimeTolerance"),  Is.EqualTo("1"));
                Assert.That(model.GroupValue("legal", "LegalTimeMaxAge"),     Is.EqualTo("3600"));

                // Never checked.
                Assert.That(model.GroupValue("legal", "Limited"),             Is.EqualTo("false"));
                Assert.That(model.GroupValue("legal", "ErrorsInARow"),        Is.EqualTo("0"));
                Assert.That(model.GroupValue("legal", "LastCheck"),           Is.EqualTo(""));
                Assert.That(model.GroupValue("legal", "LastOffset"),          Is.EqualTo(""));

                Assert.That(model.GroupValue("local", "LegalTimeAuthority"),  Is.EqualTo(""), "the controller leaves it out of every group but the legal one");

            });

        }

        #endregion

        #region AServerReadsAsTheFileSaysIt()

        /// <summary>
        /// NTPClientController says what the file says of a server, with the
        /// node's ports where the file names none, its pins as the node keeps
        /// them, and nothing about a key exchange that has not happened yet.
        /// </summary>
        [Test]
        public void AServerReadsAsTheFileSaysIt()
        {

            Assert.Multiple(() => {

                Assert.That(model.ClientValue("ptb-a",    "Enabled"),                 Is.EqualTo("true"));
                Assert.That(model.ClientValue("ptb-a",    "ServerURL"),               Is.EqualTo("ptbtime1.example.invalid"));
                Assert.That(model.ClientValue("ptb-a",    "ServerPort"),              Is.EqualTo("123"));
                Assert.That(model.ClientValue("ptb-a",    "NTSKEServerPort"),         Is.EqualTo("4460"));
                Assert.That(model.ClientValue("ptb-a",    "Priority"),                Is.EqualTo("1"));
                Assert.That(model.ClientValue("ptb-a",    "Mode"),                    Is.EqualTo("NTSv4"));
                Assert.That(model.ClientValue("ptb-a",    "RootPins"),                Is.EqualTo(ARoot));
                Assert.That(model.ClientValue("ptb-a",    "CertificatePins"),         Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a",    "OnPinMismatch"),           Is.EqualTo("Record"));
                Assert.That(model.ClientValue("ptb-a",    "TrustOnFirstUse"),         Is.EqualTo("Certificate"));

                Assert.That(model.ClientValue("legal-3",  "NTSKEServerPort"),         Is.EqualTo("4461"));
                Assert.That(model.ClientValue("legal-3",  "Priority"),                Is.EqualTo("0"));
                Assert.That(model.ClientValue("legal-4",  "Enabled"),                 Is.EqualTo("false"));
                Assert.That(model.ClientValue("local-1",  "ServerURL"),               Is.EqualTo("lc1.example.invalid"));

                // Held to nothing.
                Assert.That(model.ClientValue("local-1",  "RootPins"),                Is.EqualTo(""));
                Assert.That(model.ClientValue("local-1",  "OnPinMismatch"),           Is.EqualTo("Refuse"));
                Assert.That(model.ClientValue("local-1",  "TrustOnFirstUse"),         Is.EqualTo("None"));

                // No key exchange yet.
                Assert.That(model.ClientValue("ptb-a",    "Problem"),                 Is.EqualTo("false"));
                Assert.That(model.ClientValue("ptb-a",    "CertificateFingerprint"),  Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a",    "LastKeyExchange"),         Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a",    "Cookies"),                 Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a",    "AEADAlgorithm"),           Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a",    "RootCAs"),                 Is.EqualTo(""),  "held to no group of roots");

                // A new key exchange after 12 to 24 hours, as nothing sets another.
                Assert.That(model.ClientValue("ptb-a",    "MinRefresh"),              Is.EqualTo("43200"));
                Assert.That(model.ClientValue("ptb-a",    "MaxRefresh"),              Is.EqualTo("86400"));

                // Its offsets not corrected.
                Assert.That(model.ClientValue("ptb-a",    "DelayAsymmetry"),          Is.EqualTo("0"));
                Assert.That(model.ClientValue("ptb-a",    "DelayVarAsymmetry"),       Is.EqualTo("0"));

            });

        }

        #endregion

        #region ACSMSIsOfferedOnlyTheModeTheNodeHas()

        /// <summary>
        /// The node asks its time servers by NTS only, so the valuesList of Mode
        /// a CSMS reads is NTSv4 alone, for every server of every group (STS
        /// 3.2.4: "The valuesList shall list only what the charging station
        /// supports"). The other values lists stay the paper's.
        /// </summary>
        [Test]
        public void ACSMSIsOfferedOnlyTheModeTheNodeHas()
        {

            static IEnumerable<String>? ValuesListOf(protocols.OCPPv2_1.IDeviceModelNode  Node,
                                                     String                               Client,
                                                     String                               Variable)

                => Node.GetComponentConfigs("NTPClientController").
                        Single(component => String.Equals(component.Instance, Client, StringComparison.OrdinalIgnoreCase)).
                        VariableConfigs.
                        First (variable  => variable.Name == Variable).
                        Characteristics?.ValuesList;

            var node = station!.OCPPv2_1Node;

            Assert.Multiple(() => {
                Assert.That(ValuesListOf(node, "ptb-a",    "Mode"),           Is.EqualTo(new[] { "NTSv4" }));
                Assert.That(ValuesListOf(node, "local-1",  "Mode"),           Is.EqualTo(new[] { "NTSv4" }));
                Assert.That(ValuesListOf(node, "ptb-a",    "AEADAlgorithm"),  Is.EqualTo(new[] { "AES-SIV-CMAC-256", "AES-128-GCM-SIV" }));
                Assert.That(ValuesListOf(node, "ptb-a",    "OnPinMismatch"),  Is.EqualTo(new[] { "Refuse", "Record", "Accept" }));
            });

        }

        #endregion

        #region WhatTheNodeCannotAnswerIsNull()

        /// <summary>
        /// A group or server the node does not have, and a variable it cannot
        /// answer, are null - which the controllers turn into UnknownComponent
        /// and UnknownVariable - and not an empty value.
        /// </summary>
        [Test]
        public void WhatTheNodeCannotAnswerIsNull()
        {

            Assert.Multiple(() => {
                Assert.That(model.ClientValue    ("nobody",  "Enabled"),         Is.Null);
                Assert.That(model.ClientValue    ("ptb-a",   "SignedResponses"), Is.Null);
                Assert.That(model.ClientValue    ("ptb-a",   "NTSKEServerURL"),  Is.Null);
                Assert.That(model.GroupValue     ("grid",    "Enabled"),         Is.Null);
                Assert.That(model.GroupValue     ("legal",   "Preflight"),       Is.Null);
                Assert.That(model.ControllerValue("ClientGroups"),               Is.Null, "the controller's own, from the groups");
                Assert.That(model.ClockValue     ("TimeZone"),                   Is.Null);
                Assert.That(model.ClockValue     ("NtpServerUri", "first"),      Is.Null);
            });

        }

        #endregion

        #region NamesAreReadInAnyCase()

        [Test]
        public void NamesAreReadInAnyCase()
        {

            Assert.Multiple(() => {
                Assert.That(model.GroupValue ("LEGAL",  "minservers"),  Is.EqualTo("2"));
                Assert.That(model.ClientValue("PTB-A",  "serverurl"),   Is.EqualTo("ptbtime1.example.invalid"));
                Assert.That(model.ClockValue ("ntpsource"),             Is.EqualTo("manual"));
            });

        }

        #endregion

        #region TheControllerSaysWhyTheTimeIsNotLegal()

        /// <summary>
        /// NTPController says whether the time is legal time, why not, and which
        /// groups the node knows a meaning for and does not have.
        /// </summary>
        [Test]
        public void TheControllerSaysWhyTheTimeIsNotLegal()
        {

            Assert.Multiple(() => {
                Assert.That(model.ControllerValue("LegalTime"),        Is.EqualTo("false"));
                Assert.That(model.ControllerValue("LegalTimeReason"),  Is.EqualTo("Disabled"), "the legal group is switched off");
                Assert.That(model.ControllerValue("SuggestedGroups"),  Is.EqualTo("grid"));
            });

        }

        #endregion

        #region TheClockIsTheLegalGroups()

        /// <summary>
        /// ClockCtrlr reads the node's clock, and the servers the legal group
        /// asks, in the order it asks them - by priority, then as its list names
        /// them - leaving out one switched off; where the time comes from is
        /// the clock alone while the legal group is switched off.
        /// </summary>
        [Test]
        public void TheClockIsTheLegalGroups()
        {

            Assert.Multiple(() => {

                Assert.That(model.ClockValue("DateTime"),           Is.EqualTo("2026-11-02T11:00:00.125Z"));
                Assert.That(model.ClockValue("NtpSource"),          Is.EqualTo("manual"));
                Assert.That(model.ClockValue("TimeSource"),         Is.EqualTo("RealTimeClock"));

                Assert.That(model.ClockValue("NtpServerUri", "1"),  Is.EqualTo("nts://ptbtime2.example.invalid"));
                Assert.That(model.ClockValue("NtpServerUri", "2"),  Is.EqualTo("nts://ptbtime3.example.invalid:4461"));
                Assert.That(model.ClockValue("NtpServerUri", "3"),  Is.EqualTo("nts://ptbtime1.example.invalid"));
                Assert.That(model.ClockValue("NtpServerUri", "4"),  Is.EqualTo(""));
                Assert.That(model.ClockValue("NtpServerUri", "9"),  Is.EqualTo(""));

            });

        }

        #endregion

        #region TheReportingThresholdIsTheLegalGroups()

        /// <summary>
        /// ClockCtrlr's TimeAdjustmentReportingThreshold is the legal group's
        /// threshold of an offset: the white paper's five seconds where the
        /// file names none, and empty where it says never.
        /// </summary>
        [Test]
        public void TheReportingThresholdIsTheLegalGroups()
        {

            var five   = model.ClockValue("TimeAdjustmentReportingThreshold");

            Assert.That(station!.Node.TryUpdateNTSConfiguration(station.Node.LegalTime, JObject.Parse("""{ "offsetReportingThresholdSeconds": 9 }"""), out var error, out _),  Is.True,  error);

            var nine   = model.ClockValue("TimeAdjustmentReportingThreshold");

            Assert.That(station.Node.TryUpdateNTSConfiguration(station.Node.LegalTime, JObject.Parse("""{ "offsetReportingThresholdSeconds": false }"""), out error, out _),  Is.True,  error);

            var never  = model.ClockValue("TimeAdjustmentReportingThreshold");

            Assert.Multiple(() => {
                Assert.That(five,   Is.EqualTo("5"));
                Assert.That(nine,   Is.EqualTo("9"));
                Assert.That(never,  Is.EqualTo(""),  "never is an empty value, which OCPP lets the variable have");
            });

        }

        #endregion

        #region WithTheLegalGroupOnTheTimeComesFromNTP()

        /// <summary>
        /// While the legal group is switched on, the time comes from NTP, and
        /// from the clock where that has nothing to say.
        /// </summary>
        /// <remarks>
        /// A node of its own, built with the legal group switched on and
        /// never started: it is the start that schedules the first check.
        /// </remarks>
        [Test]
        public async Task WithTheLegalGroupOnTheTimeComesFromNTP()
        {

            var otherDirectory = TestNodes.TemporaryDirectory("nts-device-model-on");

            try
            {

                await using var on = Nodes.New(otherDirectory, Configuration(LegalEnabled: true));

                Assert.Multiple(() => {
                    Assert.That(new NTSDeviceModel(on.Node).ClockValue("TimeSource"),    Is.EqualTo("NTP,RealTimeClock"));
                    Assert.That(new NTSDeviceModel(on.Node).GroupValue("legal", "Enabled"), Is.EqualTo("true"));
                });

            }
            finally
            {
                TestNodes.Remove(otherDirectory);
            }

        }

        #endregion

        #region AChangeElsewhereIsReadAtTheNextAsk()

        /// <summary>
        /// A change the page makes is what the next read sees, the servers of a
        /// group among it: nothing was kept.
        /// </summary>
        [Test]
        public void AChangeElsewhereIsReadAtTheNextAsk()
        {

            var legal = station!.Node.NTSGroupOf("legal")!;

            // Read once before, so that a value kept from then would show.
            Assert.Multiple(() => {
                Assert.That(model.GroupValue ("legal", "MinServers"),      Is.EqualTo("2"));
                Assert.That(model.ClientValue("ptb-a", "Priority"),        Is.EqualTo("1"));
            });

            Assert.That(station.Node.TryUpdateNTSConfiguration(
                            legal,
                            new JObject(
                                new JProperty("minServers",  3),
                                new JProperty("servers",     new JArray(
                                    Server("ptb-a",    "ptbtime1.example.invalid", Priority: 2),
                                    Server("legal-2",  "ptbtime2.example.invalid"),
                                    Server("legal-3",  "ptbtime3.example.invalid", NTSKEPort: 4461)
                                ))
                            ),
                            out var error,
                            out _
                        ),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(model.GroupValue("legal", "MinServers"),       Is.EqualTo("3"));
                Assert.That(model.ClientValue("ptb-a", "Priority"),        Is.EqualTo("2"));
                Assert.That(model.ClientsOf ("legal"),                     Is.EqualTo(new[] { "ptb-a", "legal-2", "legal-3" }));
                Assert.That(model.ClientValue("legal-4", "Enabled"),       Is.Null, "taken out of the list");
            });

        }

        #endregion

        #region TheOCPPNodeFollowsTheGroups()

        /// <summary>
        /// The node's OCPP 2.1 node answers GetVariables for ClockCtrlr and
        /// the NTP controllers from the node, and has a component for
        /// every group and server there is: one made, taken away or changed on
        /// the page is one the node has or no longer has at the next ask - the
        /// node rebuilt for changed EVSEs as well, from then on.
        /// </summary>
        [Test]
        public async Task TheOCPPNodeFollowsTheGroups()
        {

            static protocols.OCPPv2_1.GetVariableResult Ask(protocols.OCPPv2_1.IDeviceModelNode  Node,
                                                            String                                         Component,
                                                            String?                                        Instance,
                                                            String                                         Variable,
                                                            String?                                        VariableInstance = null)

                => protocols.OCPPv2_1.DeviceModel.GetVariables(
                       [
                           new protocols.OCPPv2_1.GetVariableData(
                               Component:   new protocols.OCPPv2_1.Component(Component, Instance),
                               Variable:    new protocols.OCPPv2_1.Variable (Variable,  VariableInstance)
                           )
                       ],
                       (name, evse) => Node.GetComponentConfigs(name, evse)
                   ).Single();

            var before = station!.OCPPv2_1Node;

            Assert.Multiple(() => {
                Assert.That(Ask(before, "ClockCtrlr",                null,     "NtpServerUri", "2").AttributeValue,   Is.EqualTo("nts://ptbtime3.example.invalid:4461"));
                Assert.That(Ask(before, "NTPController",             null,     "ClientGroups").AttributeValue,        Is.EqualTo("legal,local"));
                Assert.That(Ask(before, "NTPClientGroupController",  "legal",  "MinServers").AttributeValue,          Is.EqualTo("2"));
                Assert.That(Ask(before, "NTPClientController",       "ptb-a",  "RootPins").AttributeValue,            Is.EqualTo(ARoot));
                Assert.That(Ask(before, "NTPClientGroupController",  "local",  "LegalTimeAuthority").AttributeStatus, Is.EqualTo(protocols.OCPPv2_1.GetVariableStatus.UnknownVariable));
            });

            // A group made on the page.
            Assert.That(station.Node.TryAddNTSGroup("grid", null, out _, out var error, out _), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Ask(before, "NTPController",             null,     "ClientGroups").AttributeValue,        Is.EqualTo("legal,local,grid"));
                Assert.That(Ask(before, "NTPClientGroupController",  "grid",   "Enabled").AttributeValue,             Is.EqualTo("false"));
            });

            // A server put into a group, and one taken out of it.
            var local = station.Node.NTSGroupOf("local")!;

            Assert.That(station.Node.TryUpdateNTSConfiguration(local, new JObject(new JProperty("servers", new JArray(Server("local-1", "lc1.example.invalid"), Server("lc2", "lc2.example.invalid")))), out error, out _), Is.True, error);
            Assert.That(Ask(before, "NTPClientController", "lc2", "ServerURL").AttributeValue, Is.EqualTo("lc2.example.invalid"));

            Assert.That(station.Node.TryUpdateNTSConfiguration(local, new JObject(new JProperty("servers", new JArray(Server("lc2", "lc2.example.invalid")))), out error, out _), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Ask(before, "NTPClientController",       "local-1", "ServerURL").AttributeStatus,         Is.EqualTo(protocols.OCPPv2_1.GetVariableStatus.UnknownComponent));
                Assert.That(Ask(before, "NTPClientGroupController",  "local",   "Clients").AttributeValue,            Is.EqualTo("lc2"));
            });

            // A group taken away.
            var (removed, removeError, _) = await station.Node.TryRemoveNTSGroupAsync("grid");

            Assert.That(removed, Is.True, removeError);
            Assert.That(Ask(before, "NTPClientGroupController", "grid", "Enabled").AttributeStatus, Is.EqualTo(protocols.OCPPv2_1.GetVariableStatus.UnknownComponent));

            // OCPP nodes built anew, as a charging station builds them when
            // its EVSEs changed: the new 2.1 node has them all, and is the one
            // followed from then on.
            station.NewOCPPNodes();

            var after = station.OCPPv2_1Node;

            Assert.That(after, Is.Not.SameAs(before), "the node was rebuilt");

            Assert.That(station.Node.TryAddNTSGroup("grid", null, out _, out error, out _), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Ask(after,  "ClockCtrlr",                null,     "NtpServerUri", "1").AttributeValue,   Is.EqualTo("nts://ptbtime2.example.invalid"));
                Assert.That(Ask(after,  "NTPClientController",       "lc2",    "ServerURL").AttributeValue,           Is.EqualTo("lc2.example.invalid"));
                Assert.That(Ask(after,  "NTPClientGroupController",  "grid",   "Enabled").AttributeStatus,            Is.EqualTo(protocols.OCPPv2_1.GetVariableStatus.Accepted));
                Assert.That(Ask(before, "NTPClientGroupController",  "grid",   "Enabled").AttributeStatus,            Is.EqualTo(protocols.OCPPv2_1.GetVariableStatus.UnknownComponent), "the old node is left as it was");
            });

        }

        #endregion


        #region (private static) Configuration(LegalEnabled)

        /// <summary>
        /// A legal group of four servers - one given an id and pinned to a root,
        /// asked after the others; one on a port of its own; one switched off -
        /// and a local group of one.
        /// </summary>
        private static JObject Configuration(Boolean LegalEnabled)
        {

            var configuration = TestNodes.Offline;

            configuration["nts"] = new JObject(
                                       new JProperty("groups", new JObject(
                                           new JProperty("legal", new JObject(
                                               new JProperty("enabled",                    LegalEnabled),
                                               new JProperty("servers",                    new JArray(
                                                   new JObject(
                                                       new JProperty("id",                 "ptb-a"),
                                                       new JProperty("hostname",           "ptbtime1.example.invalid"),
                                                       new JProperty("priority",           1),
                                                       new JProperty("rootFingerprint",    ARootAsTyped),
                                                       new JProperty("onMismatch",         "record"),
                                                       new JProperty("trustOnFirstUse",    "certificate")
                                                   ),
                                                   new JObject(
                                                       new JProperty("hostname",           "ptbtime2.example.invalid")
                                                   ),
                                                   new JObject(
                                                       new JProperty("hostname",           "ptbtime3.example.invalid"),
                                                       new JProperty("ntsKEPort",          4461)
                                                   ),
                                                   new JObject(
                                                       new JProperty("hostname",           "ptbtime4.example.invalid"),
                                                       new JProperty("enabled",            false)
                                                   )
                                               )),
                                               new JProperty("minServers",                 2),
                                               new JProperty("maxDeviationSeconds",        0.5),
                                               new JProperty("checkEverySeconds",          900),
                                               new JProperty("timeoutSeconds",             2.5),
                                               new JProperty("legalTimeAuthority",         "PTB"),
                                               new JProperty("legalTimeToleranceSeconds",  1),
                                               new JProperty("legalTimeMaxAgeSeconds",     3600),
                                               new JProperty("errorLogging",               3),
                                               new JProperty("maxErrors",                  10)
                                           )),
                                           new JProperty("local", new JObject(
                                               new JProperty("enabled",                    false),
                                               new JProperty("servers",                    new JArray(
                                                   new JObject(
                                                       new JProperty("hostname",           "lc1.example.invalid")
                                                   )
                                               ))
                                           ))
                                       ))
                                   );

            return configuration;

        }

        #endregion

        #region (private static) Server(Id, Hostname, Priority = 0, NTSKEPort = null)

        /// <summary>
        /// A server as the page sends it back: with its id, so that it keeps it.
        /// </summary>
        private static JObject Server(String   Id,
                                      String   Hostname,
                                      Byte     Priority    = 0,
                                      UInt16?  NTSKEPort   = null)
        {

            var server = new JObject(
                             new JProperty("id",        Id),
                             new JProperty("hostname",  Hostname),
                             new JProperty("priority",  Priority)
                         );

            if (NTSKEPort.HasValue)
                server["ntsKEPort"] = NTSKEPort.Value;

            return server;

        }

        #endregion

    }

}
