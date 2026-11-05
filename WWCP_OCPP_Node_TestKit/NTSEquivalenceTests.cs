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

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.Node;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// One configuration of the time servers, three ways to change it: the web
    /// page's PUT /api/v1/configuration/nts/groups/{id}, a CSMS of OCPP 2.1
    /// through SetVariables, and a central system of OCPP 1.6 through its
    /// configuration keys. The same change leaves the same file and the same
    /// state, and says so in the same words but for who made it (NTS concept
    /// B.15 step 9).
    /// </summary>
    /// <remarks>
    /// The node the page saves to is started and signed in to; the twins changed
    /// through OCPP are built from the same configuration and never started.
    /// All of them are offline: every group is switched off.
    /// </remarks>
        public abstract class NTSEquivalenceTests : AStartedBridgeTests
    {

        #region Data

        private const String  ARoot  = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  APin   = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private const String  LegalPath = "api/v1/configuration/nts/groups/legal";

        private static readonly Regex origin = new (@" \(by [^)]*\)", RegexOptions.CultureInvariant);

        private readonly List<(IBridgedNode Station, String Directory)> twins = [];

        #endregion

        #region Configuration / TearDown

        /// <summary>
        /// A legal group of four - one given an id, pinned to a root and asked
        /// after the others, one on a port of its own, one switched off - and a
        /// local group of one; every server named, so that the ids are the
        /// file's in every node.
        /// </summary>
        protected override JObject Configuration
        {
            get
            {

                var configuration = TestNodes.Offline;

                configuration["nts"] = JObject.Parse($$"""
                                           { "groups": {
                                               "legal": { "enabled": false, "minServers": 2, "checkEverySeconds": 900, "legalTimeAuthority": "PTB",
                                                          "errorLogging": 3, "maxErrors": 10,
                                                          "servers": [
                                                              { "id": "ptb-a", "hostname": "ptbtime1.example.invalid", "priority": 1,
                                                                "rootFingerprint": "{{ARoot}}", "onMismatch": "record", "trustOnFirstUse": "certificate" },
                                                              { "id": "legal-2", "hostname": "ptbtime2.example.invalid" },
                                                              { "id": "legal-3", "hostname": "ptbtime3.example.invalid", "ntsKEPort": 4461 },
                                                              { "id": "legal-4", "hostname": "ptbtime4.example.invalid", "enabled": false } ] },
                                               "local": { "enabled": false, "servers": [ "lc1.example.invalid" ] } } }
                                           """);

                return configuration;

            }
        }

        [TearDown]
        public async Task TakeTheTwinsAway()
        {

            foreach (var (station, directory) in twins)
            {
                await station.DisposeAsync();
                TestNodes.Remove(directory);
            }

            twins.Clear();

        }

        #endregion


        #region TheSameSettingsLeaveTheSameFileWhicheverWayTheyCame()

        /// <summary>
        /// A group's settings changed on the page, through SetVariables and
        /// through the configuration keys of OCPP 1.6 leave the same section in
        /// each file, the same group as each node holds it, and - for the page
        /// and SetVariables, which change it at once - the same log line but for
        /// who made the change.
        /// </summary>
        [Test]
        public async Task TheSameSettingsLeaveTheSameFileWhicheverWayTheyCame()
        {

            using var http  = await SignedIn();

            var pageLog     = Station.Node.Log.LastId;

            Assert.That(await Put(http, """{ "minServers": 3, "maxDeviationSeconds": 0.25, "errorLogging": 4, "maxErrors": 12, "checkEverySeconds": 600 }"""),
                        Is.EqualTo(HttpStatusCode.OK));

            var viaOCPP21   = Twin();
            var ocppLog     = viaOCPP21.Node.Log.LastId;

            var set         = await Set(viaOCPP21, OCPP21.DataConsistencyModel.ACID,
                                        Group("MinServers",   "3"),
                                        Group("MaxDeviation", "0.25"),
                                        Group("ErrorLogging", "4"),
                                        Group("MaxErrors",    "12"),
                                        Group("MinInterval",  "600"),
                                        Group("MaxInterval",  "600"));

            var viaOCPP16   = Twin();
            var keys        = viaOCPP16.OCPPv1_6Node.ConfigurationKeySources.OfType<NTSDeviceModel>().Single();
            var changed     = new[] {
                                  await keys.ChangeAsync("ntp.legal.minServers",   "3",    "CS-16"),
                                  await keys.ChangeAsync("ntp.legal.maxDeviation", "0.25", "CS-16"),
                                  await keys.ChangeAsync("ntp.legal.errorLogging", "4",    "CS-16"),
                                  await keys.ChangeAsync("ntp.legal.maxErrors",    "12",   "CS-16"),
                                  await keys.ChangeAsync("ntp.legal.maxInterval",  "600",  "CS-16")
                              };

            var pageLine    = ChangeLine(Station,   pageLog);
            var ocppLine    = ChangeLine(viaOCPP21, ocppLog);

            Assert.Multiple(() => {

                Assert.That(set.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(changed,                                       Is.All.EqualTo(OCPP16.ConfigurationStatus.Accepted));

                Assert.That(NTSOnDisk(viaOCPP21).ToString(),  Is.EqualTo(NTSOnDisk(Station).ToString()),  "SetVariables left another file than the page");
                Assert.That(NTSOnDisk(viaOCPP16).ToString(),  Is.EqualTo(NTSOnDisk(Station).ToString()),  "the 1.6 keys left another file than the page");

                Assert.That(Held(viaOCPP21).ToString(),       Is.EqualTo(Held(Station).ToString()),       "the node holds another group");

                Assert.That(pageLine,                         Does.EndWith("(by web:root)."));
                Assert.That(ocppLine,                         Does.Contain("(by ocpp:"));
                Assert.That(origin.Replace(ocppLine, ""),     Is.EqualTo(origin.Replace(pageLine, "")));

            });

        }

        #endregion

        #region TheSameServersLeaveTheSameFileWhicheverWayTheyCame()

        /// <summary>
        /// A server's priority, another's certificate pin and group of roots,
        /// and a third's AEAD algorithm, changed on the page - which sends the
        /// whole list, each server with what it showed it held to - and through
        /// SetVariables leave the same servers in the file.
        /// </summary>
        [Test]
        public async Task TheSameServersLeaveTheSameFileWhicheverWayTheyCame()
        {

            using var http = await SignedIn();

            Assert.That(await Put(http, $$"""
                            { "servers": [
                                { "id": "ptb-a", "hostname": "ptbtime1.example.invalid", "priority": 1,
                                  "rootFingerprint": "{{ARoot}}", "certificateFingerprint": "{{APin}}", "onMismatch": "record",
                                  "trustOnFirstUse": "certificate", "rootGroup": "legalCA",
                                  "pinsAsShown": { "rootFingerprint": "{{ARoot}}", "onMismatch": "record", "trustOnFirstUse": "certificate" } },
                                { "id": "legal-2", "hostname": "ptbtime2.example.invalid", "priority": 2, "pinsAsShown": { } },
                                { "id": "legal-3", "hostname": "ptbtime3.example.invalid", "ntsKEPort": 4461, "aead": "AES-SIV-CMAC-256", "pinsAsShown": { } },
                                { "id": "legal-4", "hostname": "ptbtime4.example.invalid", "enabled": false, "pinsAsShown": { } } ] }
                            """),
                        Is.EqualTo(HttpStatusCode.OK));

            var viaOCPP21 = Twin();

            var set       = await Set(viaOCPP21, OCPP21.DataConsistencyModel.ACID,
                                      Client("legal-2", "Priority",        "2"),
                                      Client("ptb-a",   "CertificatePins", APin),
                                      Client("ptb-a",   "RootCAs",         "legalCA"),
                                      Client("legal-3", "AEADAlgorithm",   "AES-SIV-CMAC-256"));

            Assert.Multiple(() => {
                Assert.That(set.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(NTSOnDisk(viaOCPP21).ToString(),               Is.EqualTo(NTSOnDisk(Station).ToString()));
            });

        }

        #endregion

        #region WhatThePageChangedIsReadByTheCSMSAndTheOtherWayRound()

        /// <summary>
        /// What the page changed, a CSMS reads at once through GetVariables; what
        /// a CSMS changed, the page reads at once through GET.
        /// </summary>
        [Test]
        public async Task WhatThePageChangedIsReadByTheCSMSAndTheOtherWayRound()
        {

            using var http  = await SignedIn();

            await Put(http, """{ "minServers": 3 }""");

            var read        = Get(Station, "NTPClientGroupController", "legal", "MinServers");

            var set         = await Set(Station, null, Client("legal-3", "NTSKEServerPort", "4470"));

            var page        = JObject.Parse(await http.GetStringAsync(LegalPath));
            var server3     = (page["timeSources"] as JArray)?.OfType<JObject>().FirstOrDefault(server => server.Value<String>("id") == "legal-3");

            Assert.Multiple(() => {
                Assert.That(read.AttributeValue,                       Is.EqualTo("3"));
                Assert.That(set.Single().AttributeStatus,              Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(server3?.Value<Int32>("ntsKEPort"),        Is.EqualTo(4470));
            });

        }

        #endregion

        #region APageChangeBetweenTheCSMSsReadAndItsWriteIsNotOverwritten()

        /// <summary>
        /// A CSMS reads MinServers, the page changes it, and the CSMS's write that
        /// names the value it read is refused as ValueChanged: the page's value
        /// stays, in effect and in the file.
        /// </summary>
        [Test]
        public async Task APageChangeBetweenTheCSMSsReadAndItsWriteIsNotOverwritten()
        {

            using var http  = await SignedIn();

            var seen        = Get(Station, "NTPClientGroupController", "legal", "MinServers").AttributeValue;

            await Put(http, """{ "minServers": 3 }""");

            var set         = await Set(Station, null, Group("MinServers", "4", Old: seen));

            Assert.Multiple(() => {
                Assert.That(seen,                                             Is.EqualTo("2"));
                Assert.That(set.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(set.Single().AttributeStatusInfo?.ReasonCode,     Is.EqualTo("ValueChanged"));
                Assert.That(NTSOnDisk(Station)["groups"]?["legal"]?.Value<Int32>("minServers"),  Is.EqualTo(3));
            });

        }

        #endregion


        #region (private) Twin() / Put(Http, JSON)

        /// <summary>
        /// A node built from the same configuration, never started.
        /// </summary>
        private IBridgedNode Twin()
        {

            var directory = TestNodes.TemporaryDirectory("nts-equivalence-twin");
            var station   = Nodes.New(directory, Configuration);

            twins.Add((station, directory));

            return station;

        }

        /// <summary>
        /// The legal group changed as the page changes it.
        /// </summary>
        private static async Task<HttpStatusCode> Put(HttpClient  Http,
                                                      String      JSON)

            => (await Http.PutAsync(LegalPath, new StringContent(JSON, Encoding.UTF8, "application/json"))).StatusCode;

        #endregion

        #region (private static) Set(Station, Model, Entries) / Get(Station, Component, Instance, Variable)

        private sealed record Entry(String Component, String Instance, String Variable, String Value, String? Old);

        private static Entry Group(String Variable, String Value, String? Old = null)
            => new ("NTPClientGroupController", "legal", Variable, Value, Old);

        private static Entry Client(String Client, String Variable, String Value)
            => new ("NTPClientController", Client, Variable, Value, null);

        /// <summary>
        /// SetVariables as the node's OCPP 2.1 node answers it.
        /// </summary>
        private static async Task<OCPP21.SetVariableResult[]> Set(IBridgedNode               Station,
                                                                  OCPP21.DataConsistencyModel?  Model,
                                                                  params Entry[]                Entries)
        {

            var node = Station.OCPPv2_1Node;

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(
                                 new OCPP21.CSMS.SetVariablesRequest(
                                     Destination:           SourceRouting.To(node.Id),
                                     VariableData:          [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                                            Component:          new OCPP21.Component(entry.Component, entry.Instance),
                                                                                            Variable:           new OCPP21.Variable(entry.Variable),
                                                                                            AttributeValue:     entry.Value,
                                                                                            OldAttributeValue:  entry.Old
                                                                                        ))],
                                     DataConsistencyModel:  Model
                                 ),
                                 (name, evse) => node.GetComponentConfigs(name, evse),
                                 default,
                                 node.NewInstanceOf)];

        }

        private static OCPP21.GetVariableResult Get(IBridgedNode  Station,
                                                    String           Component,
                                                    String           Instance,
                                                    String           Variable)
        {

            var node = Station.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component(Component, Instance), new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single();

        }

        #endregion

        #region (private static) NTSOnDisk(Station) / Held(Station) / ChangeLine(Station, Since)

        /// <summary>
        /// The "nts" section as the node's file says it now.
        /// </summary>
        private static JObject NTSOnDisk(IBridgedNode Station)

            => JObject.Parse(File.ReadAllText(Station.Node.ConfigFile.Path))["nts"] as JObject ?? [];

        /// <summary>
        /// The legal group as the node holds it, as the page reads it.
        /// </summary>
        private static JToken Held(IBridgedNode Station)

            => Station.Node.NTSConfigurationJSON(Station.Node.NTSGroups.Single(group => group.IsLegal))["settings"]!;

        /// <summary>
        /// The one line that says what a change of the legal group changed.
        /// </summary>
        private static String ChangeLine(IBridgedNode  Station,
                                         UInt64           Since)

            => Station.Node.Log.Recent(100, Since, null).
                           Select(entry => entry.Message).
                           Single(message => message.StartsWith("NTS configuration changed: ", StringComparison.Ordinal));

        #endregion

    }

}
