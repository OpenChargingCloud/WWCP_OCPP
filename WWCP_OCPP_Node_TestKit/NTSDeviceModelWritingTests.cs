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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;

using cloud.charging.open.protocols.OCPP.Node;

using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What a CSMS changes of the node's time servers through SetVariables,
    /// against what the node then holds.
    /// </summary>
    /// <remarks>
    /// Through the OCPP 2.1 node handed to the bridge into the node, built and
    /// never started, so that nothing is ever asked of a time server; the
    /// servers are named under ".invalid". The node is the one of
    /// NTSDeviceModelTests: a legal group of four, one of them given an id,
    /// pinned to a root and asked after the others, one on a port of its own,
    /// one switched off - and a local group of one.
    /// </remarks>
        public abstract class NTSDeviceModelWritingTests : ABridgeTests
    {

        #region Data

        private const String  ARoot       = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  AnotherPin  = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private String            directory  = "";
        private IBridgedNode?  station;
        private NTSDeviceModel    model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithALegalAndALocalGroup()
        {

            directory  = TestNodes.TemporaryDirectory("nts-device-model-writing");
            station    = Nodes.New(directory, Configuration(LocalEnabled: false));
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


        #region AGroupIsSetAsThePageWouldSetIt()

        /// <summary>
        /// The variables of a group are set one by one, as the page sets them:
        /// in effect at once, in the file, and the authority taken away by an
        /// empty value.
        /// </summary>
        [Test]
        public async Task AGroupIsSetAsThePageWouldSetIt()
        {

            var results = await Set(null,
                                    Group("legal", "MinServers",          "3"),
                                    Group("legal", "MaxDeviation",        "0.25"),
                                    Group("legal", "ErrorLogging",        "4"),
                                    Group("legal", "MaxErrors",           "12"),
                                    Group("legal", "LegalTimeAuthority",  ""));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                              Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.GroupValue("legal", "MinServers"),          Is.EqualTo("3"));
                Assert.That(model.GroupValue("legal", "MaxDeviation"),        Is.EqualTo("0.25"));
                Assert.That(model.GroupValue("legal", "ErrorLogging"),        Is.EqualTo("4"));
                Assert.That(model.GroupValue("legal", "MaxErrors"),           Is.EqualTo("12"));
                Assert.That(model.GroupValue("legal", "LegalTimeAuthority"),  Is.EqualTo(""));

                Assert.That(OnDisk("legal")?["minServers"]?.Value<Int32>(),   Is.EqualTo(3));
                Assert.That(OnDisk("legal")?["legalTimeAuthority"],           Is.Null, "taken out of the file");

            });

        }

        #endregion

        #region AServerIsOfferedOneAEADAlgorithmOrEveryOne()

        /// <summary>
        /// AEADAlgorithm sets the one algorithm a server's key exchange offers:
        /// in effect, written as the node names it whatever its case, read
        /// back - and kept by a change of anything else of the server, which
        /// writes its entry anew. "" offers every one again, and an algorithm
        /// the node does not offer is refused.
        /// </summary>
        [Test]
        public async Task AServerIsOfferedOneAEADAlgorithmOrEveryOne()
        {

            var a        = station!.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.ToString().StartsWith("ptbtime1."));

            var set      = await Set(null, Client("ptb-a", "AEADAlgorithm", "aes-siv-cmac-256"));
            var read     = model.ClientValue("ptb-a", "AEADAlgorithm");
            var written  = OnDisk("legal")?["servers"]?[0]?.Value<String>("aead");
            var offered  = station.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.Equals(a.Hostname)).OfferedAEADAlgorithms?.ToArray();

            var other    = await Set(null, Client("ptb-a", "Priority", "2"));
            var kept     = OnDisk("legal")?["servers"]?[0]?.Value<String>("aead");

            var refused  = await Set(null, Client("ptb-a", "AEADAlgorithm", "AES-256-GCM-SIV"));

            var unset    = await Set(null, Client("ptb-a", "AEADAlgorithm", ""));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(set),                               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(read,                                          Is.EqualTo("AES-SIV-CMAC-256"));
                Assert.That(written,                                       Is.EqualTo("AES-SIV-CMAC-256"));
                Assert.That(offered,                                       Is.EqualTo(new[] { org.GraphDefined.Vanaheimr.Norn.NTS.AEADAlgorithms.AES_SIV_CMAC_256 }));

                Assert.That(StatusesOf(other),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(kept,                                          Is.EqualTo("AES-SIV-CMAC-256"),  "a change of the server's priority took it away");

                Assert.That(StatusesOf(refused),                           Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));

                Assert.That(StatusesOf(unset),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(OnDisk("legal")?["servers"]?[0]?["aead"],     Is.Null);
                Assert.That(model.ClientValue("ptb-a", "AEADAlgorithm"),   Is.EqualTo(""),  "what the last key exchange agreed on, and there was none");

            });

        }

        #endregion

        #region AServerMakesItsKeyExchangeAgainAsMinAndMaxRefreshSay()

        /// <summary>
        /// MinRefresh and MaxRefresh set when a server's key exchange is made
        /// again: in effect, written down in seconds, read back - and kept by a
        /// change of anything else of the server. Less than a minute is refused,
        /// and so is a MaxRefresh before the MinRefresh, which the node refuses.
        /// </summary>
        [Test]
        public async Task AServerMakesItsKeyExchangeAgainAsMinAndMaxRefreshSay()
        {

            var a        = station!.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.ToString().StartsWith("ptbtime1."));

            var set      = await Set(null, Client("ptb-a", "MinRefresh", "3600"), Client("ptb-a", "MaxRefresh", "7200"));
            var min      = model.ClientValue("ptb-a", "MinRefresh");
            var max      = model.ClientValue("ptb-a", "MaxRefresh");
            var written  = OnDisk("legal")?["servers"]?[0];
            var source   = station.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.Equals(a.Hostname));

            var other    = await Set(null, Client("ptb-a", "Priority", "2"));
            var kept     = OnDisk("legal")?["servers"]?[0]?.Value<Double>("minRefreshSeconds");

            var short_   = await Set(null, Client("ptb-a", "MinRefresh", "59"));
            var crossed  = await Set(null, Client("ptb-a", "MaxRefresh", "1800"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(set),                               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(min,                                           Is.EqualTo("3600"));
                Assert.That(max,                                           Is.EqualTo("7200"));
                Assert.That(written?.Value<Double>("minRefreshSeconds"),   Is.EqualTo(3600));
                Assert.That(written?.Value<Double>("maxRefreshSeconds"),   Is.EqualTo(7200));
                Assert.That(source.MinRefresh,                             Is.EqualTo(TimeSpan.FromHours(1)));
                Assert.That(source.MaxRefresh,                             Is.EqualTo(TimeSpan.FromHours(2)));

                Assert.That(StatusesOf(other),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(kept,                                          Is.EqualTo(3600),  "a change of the server's priority took it away");

                Assert.That(StatusesOf(short_),                            Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(StatusesOf(crossed),                           Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(model.ClientValue("ptb-a", "MaxRefresh"),      Is.EqualTo("7200"),  "what was refused changed nothing");

            });

        }

        #endregion

        #region AServersOffsetsAreCorrectedAsDelayAsymmetryAndDelayVarAsymmetrySay()

        /// <summary>
        /// DelayAsymmetry and DelayVarAsymmetry correct the offsets measured
        /// with a server: in effect, written down, read back the same on every
        /// machine - and kept by a change of anything else of the server. "0"
        /// corrects nothing, and an asymmetry beyond a half is refused.
        /// </summary>
        [Test]
        public async Task AServersOffsetsAreCorrectedAsDelayAsymmetryAndDelayVarAsymmetrySay()
        {

            var a        = station!.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.ToString().StartsWith("ptbtime1."));

            var set      = await Set(null, Client("ptb-a", "DelayAsymmetry", "-0.05"), Client("ptb-a", "DelayVarAsymmetry", "-0.5"));
            var shift    = model.ClientValue("ptb-a", "DelayAsymmetry");
            var written  = OnDisk("legal")?["servers"]?[0];
            var source   = station.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.Equals(a.Hostname));

            var other    = await Set(null, Client("ptb-a", "Priority", "2"));
            var kept     = OnDisk("legal")?["servers"]?[0]?.Value<Double>("delayVarAsymmetry");

            var beyond   = await Set(null, Client("ptb-a", "DelayVarAsymmetry", "0.6"));
            var zero     = await Set(null, Client("ptb-a", "DelayAsymmetry", "0"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(set),                               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(shift,                                         Is.EqualTo("-0.05"));
                Assert.That(written?.Value<Double>("delayAsymmetryMs"),    Is.EqualTo(-0.05).Within(1e-9));
                Assert.That(written?.Value<Double>("delayVarAsymmetry"),   Is.EqualTo(-0.5));
                Assert.That(source.DelayAsymmetry,                         Is.EqualTo(TimeSpan.FromMilliseconds(-0.05)));
                Assert.That(source.DelayVarAsymmetry,                      Is.EqualTo(-0.5));

                Assert.That(StatusesOf(other),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(kept,                                          Is.EqualTo(-0.5),  "a change of the server's priority took it away");

                Assert.That(StatusesOf(beyond),                            Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(StatusesOf(zero),                              Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(OnDisk("legal")?["servers"]?[0]?["delayAsymmetryMs"],  Is.Null,  "zero corrects nothing, and is written as nothing");
                Assert.That(model.ClientValue("ptb-a", "DelayAsymmetry"),  Is.EqualTo("0"));

            });

        }

        #endregion

        #region APreflightIsSetAndTakenAwayAsAGroupsVariable()

        /// <summary>
        /// Preflight sets after how long without a round a group's servers are
        /// first sent a query whose answer is ignored: in effect, written down,
        /// read back; "" takes it away, which the node writes as false.
        /// </summary>
        [Test]
        public async Task APreflightIsSetAndTakenAwayAsAGroupsVariable()
        {

            var set      = await Set(null, Group("legal", "Preflight", "60"));
            var read     = model.GroupValue("legal", "Preflight");
            var written  = OnDisk("legal")?["preflightSeconds"]?.Value<Int32>();
            var inEffect = station!.Node.NTSGroupOf("legal")!.Preflight;

            var unset    = await Set(null, Group("legal", "Preflight", ""));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(set),                               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(read,                                          Is.EqualTo("60"));
                Assert.That(written,                                       Is.EqualTo(60));
                Assert.That(inEffect,                                      Is.EqualTo(TimeSpan.FromSeconds(60)));
                Assert.That(StatusesOf(unset),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(OnDisk("legal")?["preflightSeconds"]?.Value<Boolean>(),  Is.False,  "never, written down as false");
                Assert.That(model.GroupValue("legal", "Preflight"),        Is.EqualTo(""));
            });

        }

        #endregion

        #region AServersKeysComeFromTheKeyExchangeServerNTSKEServerURLNames()

        /// <summary>
        /// NTSKEServerURL names the key exchange server a server's keys come
        /// from: in effect, written down, read back - and kept by a change of
        /// anything else of the server. "" and the server itself are the server.
        /// </summary>
        [Test]
        public async Task AServersKeysComeFromTheKeyExchangeServerNTSKEServerURLNames()
        {

            var a        = station!.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.ToString().StartsWith("ptbtime1."));

            var set      = await Set(null, Client("ptb-a", "NTSKEServerURL", "ke.example.invalid"));
            var read     = model.ClientValue("ptb-a", "NTSKEServerURL");
            var written  = OnDisk("legal")?["servers"]?[0]?.Value<String>("ntsKEHostname");
            var source   = station.Node.NTSGroupOf("legal")!.Sources.Sources.Single(source => source.Hostname.Equals(a.Hostname));

            var other    = await Set(null, Client("ptb-a", "Priority", "2"));
            var kept     = OnDisk("legal")?["servers"]?[0]?.Value<String>("ntsKEHostname");

            var itself   = await Set(null, Client("ptb-a", "NTSKEServerURL", a.Hostname.Trimmed));
            var gone     = OnDisk("legal")?["servers"]?[0]?["ntsKEHostname"];

            Assert.Multiple(() => {
                Assert.That(StatusesOf(set),                               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(read,                                          Is.EqualTo("ke.example.invalid"));
                Assert.That(written?.TrimEnd('.'),                         Is.EqualTo("ke.example.invalid"));
                Assert.That(source.NTSKEHost?.Trimmed,                     Is.EqualTo("ke.example.invalid"));
                Assert.That(StatusesOf(other),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(kept?.TrimEnd('.'),                            Is.EqualTo("ke.example.invalid"),  "a change of the server's priority took it away");
                Assert.That(StatusesOf(itself),                            Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(gone,                                          Is.Null,  "the server itself is no other key exchange server");
                Assert.That(model.ClientValue("ptb-a", "NTSKEServerURL"),  Is.EqualTo(""));
            });

        }

        #endregion

        #region BothIntervalsAreTheOneTheNodeChecksAt()

        /// <summary>
        /// The node checks at one interval: MinInterval and MaxInterval are set
        /// together, to the same value, or not at all.
        /// </summary>
        [Test]
        public async Task BothIntervalsAreTheOneTheNodeChecksAt()
        {

            var alone   = await Set(null,
                                    Group("legal", "MinInterval", "300"));

            var apart   = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group("legal", "MinInterval", "300"),
                                    Group("legal", "MaxInterval", "600"));

            var both    = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group("legal", "MinInterval", "600"),
                                    Group("legal", "MaxInterval", "600"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(alone),                         Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(alone.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("same value"));
                Assert.That(StatusesOf(apart),                         Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(StatusesOf(both),                          Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.GroupValue("legal", "MinInterval"),  Is.EqualTo("600"));
                Assert.That(model.GroupValue("legal", "MaxInterval"),  Is.EqualTo("600"));

            });

        }

        #endregion

        #region AServerKeepsItsIdAndRootsWhenItIsRenamed()

        /// <summary>
        /// A server given another host and key exchange port keeps its id and
        /// what it is pinned to at its root, and is asked as the server it now
        /// names.
        /// </summary>
        [Test]
        public async Task AServerKeepsItsIdAndRootsWhenItIsRenamed()
        {

            var results = await Set(null,
                                    Client("ptb-a",   "ServerURL",  "nts://ptbtime9.example.invalid:4470"),
                                    Client("legal-2", "Priority",   "2"),
                                    Client("legal-4", "Enabled",    "true"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                              Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.ClientsOf("legal"),                         Is.EqualTo(new[] { "ptb-a", "legal-2", "legal-3", "legal-4" }));
                Assert.That(model.ClientValue("ptb-a",   "ServerURL"),        Is.EqualTo("ptbtime9.example.invalid"));
                Assert.That(model.ClientValue("ptb-a",   "NTSKEServerPort"),  Is.EqualTo("4470"));
                Assert.That(model.ClientValue("ptb-a",   "RootPins"),         Is.EqualTo(ARoot));
                Assert.That(model.ClientValue("ptb-a",   "OnPinMismatch"),    Is.EqualTo("Record"));
                Assert.That(model.ClientValue("legal-2", "Priority"),         Is.EqualTo("2"));
                Assert.That(model.ClientValue("legal-4", "Enabled"),          Is.EqualTo("true"));

            });

        }

        #endregion

        #region ARenamedServerKeepsNoPinOfTheOldCertificate()

        /// <summary>
        /// A pin of a certificate names the certificate of the server that
        /// showed it: a server given another host loses it, unless the same
        /// change says which it keeps, while what it is pinned to at its root
        /// stays.
        /// </summary>
        [Test]
        public async Task ARenamedServerKeepsNoPinOfTheOldCertificate()
        {

            var pinned  = await Set(null,
                                    Client("ptb-a", "CertificatePins", AnotherPin));

            var renamed = await Set(null,
                                    Client("ptb-a", "ServerURL", "ptbtime9.example.invalid"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf([.. pinned, .. renamed]),               Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.ClientValue("ptb-a", "CertificatePins"),     Is.EqualTo(""));
                Assert.That(model.ClientValue("ptb-a", "RootPins"),            Is.EqualTo(ARoot));
            });

        }

        #endregion

        #region AVariableSetTwiceIsRefused()

        /// <summary>
        /// Which of two values for one variable is meant cannot be told: both
        /// are refused, and with ACID the rest is rolled back.
        /// </summary>
        [Test]
        public async Task AVariableSetTwiceIsRefused()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group("legal", "MaxErrors",  "11"),
                                    Group("legal", "MaxErrors",  "12"),
                                    Group("local", "MaxErrors",  "7"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                     Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(model.GroupValue("legal", "MaxErrors"),  Is.EqualTo("10"));
            });

        }

        #endregion

        #region AServerIsAskedByNTSOnly()

        /// <summary>
        /// A server named by another scheme than nts:// is refused: this
        /// node asks its time servers by NTS only.
        /// </summary>
        [Test]
        public async Task AServerIsAskedByNTSOnly()
        {

            var results = await Set(null,
                                    Client("ptb-a", "ServerURL", "ntp://ptbtime9.example.invalid"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("NTS only"));
                Assert.That(model.ClientValue("ptb-a", "ServerURL"),              Is.EqualTo("ptbtime1.example.invalid"));
            });

        }

        #endregion

        #region ANewServerComesWithItsURLInOneChange()

        /// <summary>
        /// A server named anew in Clients is made by the same request, with the
        /// ServerURL given to its new NTPClientController - which needs ACID or
        /// BASE, since the two are one change. The OCPP node has its component
        /// right after.
        /// </summary>
        [Test]
        public async Task ANewServerComesWithItsURLInOneChange()
        {

            var alone   = await Set(null,
                                    Group ("local", "Clients",    "local-1,lc2"),
                                    Client("lc2",   "ServerURL",  "lc2.example.invalid"));

            var both    = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group ("local", "Clients",    "local-1,lc2"),
                                    Client("lc2",   "ServerURL",  "lc2.example.invalid"),
                                    Client("lc2",   "Priority",   "1"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(alone),                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), "with IndependentRequests");
                Assert.That(StatusesOf(both),                        Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.ClientsOf("local"),                Is.EqualTo(new[] { "local-1", "lc2" }));
                Assert.That(model.ClientValue("lc2", "ServerURL"),   Is.EqualTo("lc2.example.invalid"));
                Assert.That(model.ClientValue("lc2", "Priority"),    Is.EqualTo("1"));

                Assert.That(Get("NTPClientController", "lc2", "ServerURL").AttributeValue,  Is.EqualTo("lc2.example.invalid"));

            });

        }

        #endregion

        #region ANewServerWithoutItsURLIsNotMade()

        /// <summary>
        /// A server named anew without its ServerURL cannot be made: the entry
        /// naming it is refused, and the rest of the change rolled back.
        /// </summary>
        [Test]
        public async Task ANewServerWithoutItsURLIsNotMade()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group("local", "Clients",    "local-1,lc2"),
                                    Group("local", "MaxErrors",  "7"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                    Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo, Does.Contain("set its ServerURL"));
                Assert.That(model.ClientsOf("local"),               Is.EqualTo(new[] { "local-1" }));
                Assert.That(model.GroupValue("local", "MaxErrors"), Is.Not.EqualTo("7"));
            });

        }

        #endregion

        #region AnInstanceNotNamedAnewIsUnknown()

        /// <summary>
        /// A server no Clients of the request names anew is no new one: its
        /// entries are UnknownComponent, as for any component that is not there.
        /// </summary>
        [Test]
        public async Task AnInstanceNotNamedAnewIsUnknown()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Client("lc3", "ServerURL", "lc3.example.invalid"));

            Assert.That(results.Single().AttributeStatus, Is.EqualTo(OCPP21.SetVariableStatus.UnknownComponent));

        }

        #endregion

        #region AServerLeftOutOfClientsIsTakenAway()

        [Test]
        public async Task AServerLeftOutOfClientsIsTakenAway()
        {

            var results = await Set(null,
                                    Group("legal", "Clients", "ptb-a,legal-2,legal-3"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,   Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.ClientsOf("legal"),           Is.EqualTo(new[] { "ptb-a", "legal-2", "legal-3" }));
                Assert.That(Get("NTPClientController", "legal-4", "ServerURL").AttributeStatus,  Is.EqualTo(OCPP21.GetVariableStatus.UnknownComponent));
            });

        }

        #endregion

        #region AGroupIsMadeWithItsServersAndTakenAwayAgain()

        /// <summary>
        /// A group named anew in ClientGroups is made by the same request, with
        /// what is set of it and of a server it is given; a group no longer
        /// named, switched off, is taken away.
        /// </summary>
        [Test]
        public async Task AGroupIsMadeWithItsServersAndTakenAwayAgain()
        {

            var made    = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Controller("ClientGroups",         "legal,local,grid"),
                                    Group ("grid", "MaxErrors",        "9"),
                                    Group ("grid", "Clients",          "grid-a"),
                                    Client("grid-a", "ServerURL",      "grid-a.example.invalid"));

            var groupsMade  = model.Groups.ToArray();
            var gridServers = model.ClientsOf("grid").ToArray();
            var gridErrors  = model.GroupValue("grid", "MaxErrors");

            var taken   = await Set(null,
                                    Controller("ClientGroups",         "legal,local"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(made),         Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(groupsMade,               Is.EqualTo(new[] { "legal", "local", "grid" }));
                Assert.That(gridServers,              Is.EqualTo(new[] { "grid-a" }));
                Assert.That(gridErrors,               Is.EqualTo("9"));

                Assert.That(StatusesOf(taken),        Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.Groups,             Is.EqualTo(new[] { "legal", "local" }));

            });

        }

        #endregion

        #region AGroupMadeByARefusedChangeIsTakenAwayAgain()

        /// <summary>
        /// A change refused by the node after it made a group takes the group
        /// away again: the entry the node refused says why, the others are
        /// rolled back, and nothing of the change is left.
        /// </summary>
        [Test]
        public async Task AGroupMadeByARefusedChangeIsTakenAwayAgain()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Controller("ClientGroups",          "legal,local,grid"),
                                    Group ("legal", "MinServers",       "9"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                    Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rolledback, OCPP21.SetVariableStatus.Rejected }));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("NTPClientGroupController[legal]"));
                Assert.That(model.Groups,                           Is.EqualTo(new[] { "legal", "local" }));
                Assert.That(model.GroupValue("legal", "MinServers"), Is.EqualTo("2"));
            });

        }

        #endregion

        #region AGroupSwitchedOnIsNotTakenAway()

        /// <summary>
        /// A group switched on is not taken away: it is switched off first.
        /// </summary>
        /// <remarks>
        /// A node of its own, with the local group switched on, never
        /// started: it is the start that schedules the first check.
        /// </remarks>
        [Test]
        public async Task AGroupSwitchedOnIsNotTakenAway()
        {

            var otherDirectory = TestNodes.TemporaryDirectory("nts-device-model-writing-on");

            try
            {

                await using var on = Nodes.New(otherDirectory, Configuration(LocalEnabled: true));

                var results = await Set(on, null, Controller("ClientGroups", "legal"));

                Assert.Multiple(() => {
                    Assert.That(results.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                    Assert.That(results.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("switch it off"));
                    Assert.That(new NTSDeviceModel(on.Node).Groups,                        Is.EqualTo(new[] { "legal", "local" }));
                });

            }
            finally
            {
                TestNodes.Remove(otherDirectory);
            }

        }

        #endregion

        #region ARefusalOfTheNodeRollsTheRestBack()

        /// <summary>
        /// A quorum the legal group cannot reach is refused by the node: with
        /// ACID the change of the local group in the same request is rolled
        /// back, with BASE each group is a change of its own.
        /// </summary>
        [Test]
        public async Task ARefusalOfTheNodeRollsTheRestBack()
        {

            var acid    = await Set(OCPP21.DataConsistencyModel.ACID,
                                    Group("legal", "MinServers", "9"),
                                    Group("local", "MaxErrors",  "7"));

            var afterACID = model.GroupValue("local", "MaxErrors");

            var @base   = await Set(OCPP21.DataConsistencyModel.BASE,
                                    Group("legal", "MinServers", "9"),
                                    Group("local", "MaxErrors",  "7"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(acid),    Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(afterACID,           Is.Not.EqualTo("7"));
                Assert.That(StatusesOf(@base),   Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Accepted }));
                Assert.That(model.GroupValue("local", "MaxErrors"), Is.EqualTo("7"));
            });

        }

        #endregion

        #region NtpServerUriIsTheLegalGroupByPosition()

        /// <summary>
        /// ClockCtrlr.NtpServerUri[k] is the k-th server the legal group asks.
        /// Set, it renames that server; set at the next free position, it adds
        /// one in the band of the last; emptied, it takes one away. Each entry
        /// of independent requests finds what the one before left.
        /// </summary>
        [Test]
        public async Task NtpServerUriIsTheLegalGroupByPosition()
        {

            var results = await Set(null,
                                    Clock("1", "nts://other.example.invalid"),
                                    Clock("4", "new.example.invalid"),
                                    Clock("2", ""),
                                    Clock("9", "far.example.invalid"),
                                    Clock("3", "ntp://old.example.invalid"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                    Is.EqualTo(new[] { OCPP21.SetVariableStatus.Accepted, OCPP21.SetVariableStatus.Accepted, OCPP21.SetVariableStatus.Accepted,
                                                                                       OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rejected }));
                Assert.That(results[3].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("NtpServerUri[4]"), "the next free one");

                Assert.That(model.ClockValue("NtpServerUri", "1"),  Is.EqualTo("nts://other.example.invalid"));
                Assert.That(model.ClockValue("NtpServerUri", "2"),  Is.EqualTo("nts://ptbtime1.example.invalid"));
                Assert.That(model.ClockValue("NtpServerUri", "3"),  Is.EqualTo("nts://new.example.invalid"));
                Assert.That(model.ClockValue("NtpServerUri", "4"),  Is.EqualTo(""));

                Assert.That(model.ClientValue("legal-2", "ServerURL"),  Is.EqualTo("other.example.invalid"), "renamed, keeping its id");
                Assert.That(model.ClientValue("legal-3", "ServerURL"),  Is.Null, "taken away");
                Assert.That(model.ClientsOf("legal").Count(),           Is.EqualTo(4), "ptb-a, legal-2, legal-4 and the new one");

            });

        }

        #endregion

        #region TimeSourceIsWhatTheTimeServersSay()

        [Test]
        public async Task TimeSourceIsWhatTheTimeServersSay()
        {

            var asItIs  = await Set(null, Clock(null, "RealTimeClock",     "TimeSource"));
            var other   = await Set(null, Clock(null, "NTP,RealTimeClock", "TimeSource"));

            Assert.Multiple(() => {
                Assert.That(asItIs.Single().AttributeStatus,                    Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(other. Single().AttributeStatus,                    Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(other. Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("switched off"));
            });

        }

        #endregion

        #region TheReportingThresholdIsTheLegalGroups()

        /// <summary>
        /// ClockCtrlr's TimeAdjustmentReportingThreshold goes into the legal
        /// group, with its servers where a request names both; empty is never,
        /// written down as false, and below a second is out of range.
        /// </summary>
        [Test]
        public async Task TheReportingThresholdIsTheLegalGroups()
        {

            var seven        = await Set(OCPP21.DataConsistencyModel.ACID,
                                         Clock("1",  "other.example.invalid"),
                                         Clock(null, "7", "TimeAdjustmentReportingThreshold"));

            var sevenOnDisk  = OnDisk("legal")?["offsetReportingThresholdSeconds"];
            var sevenRead    = model.ClockValue("TimeAdjustmentReportingThreshold");
            var firstServer  = model.ClockValue("NtpServerUri", "1");

            var never        = await Set(null, Clock(null, "",  "TimeAdjustmentReportingThreshold"));

            var neverOnDisk  = OnDisk("legal")?["offsetReportingThresholdSeconds"];
            var neverRead    = model.ClockValue("TimeAdjustmentReportingThreshold");

            var zero         = await Set(null, Clock(null, "0", "TimeAdjustmentReportingThreshold"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(seven),                   Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(sevenOnDisk?.Type,                   Is.EqualTo(JTokenType.Integer));
                Assert.That(sevenOnDisk?.Value<Int32>(),         Is.EqualTo(7));
                Assert.That(sevenRead,                           Is.EqualTo("7"));
                Assert.That(firstServer,                         Is.EqualTo("nts://other.example.invalid"),  "the servers went in with it");

                Assert.That(never.Single().AttributeStatus,      Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(neverOnDisk?.Type,                   Is.EqualTo(JTokenType.Boolean),  "never, and not the five seconds of the key left out");
                Assert.That(neverOnDisk?.Value<Boolean>(),       Is.False);
                Assert.That(neverRead,                           Is.EqualTo(""));
                Assert.That(station!.Node.LegalTime.OffsetReportingThreshold,  Is.Null);

                Assert.That(zero.Single().AttributeStatus,                   Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(zero.Single().AttributeStatusInfo?.ReasonCode,   Is.EqualTo("ValueOutOfRange"));

            });

        }

        #endregion

        #region AChangeTheCSMSHasNotSeenIsNotMade()

        /// <summary>
        /// A value the CSMS did not read last - changed on the page meanwhile -
        /// is not overwritten: compare and set.
        /// </summary>
        [Test]
        public async Task AChangeTheCSMSHasNotSeenIsNotMade()
        {

            var results = await Set(null,
                                    Group("legal", "MaxErrors", "12", Old: "11"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                  Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,  Is.EqualTo("ValueChanged"));
                Assert.That(model.GroupValue("legal", "MaxErrors"),            Is.EqualTo("10"));
            });

        }

        #endregion

        #region AServerAddedOnThePageMeanwhileIsNotWrittenAway()

        /// <summary>
        /// A server added on the page while a change of the CSMS was made: the
        /// list the change would send back is not the one the node holds, so
        /// it is refused as ValueChanged, and the page's server is kept.
        /// </summary>
        [Test]
        public async Task AServerAddedOnThePageMeanwhileIsNotWrittenAway()
        {

            var local     = station!.Node.NTSGroupOf("local")!;

            // Said right before the change goes into the node: the page saves
            // a second server of the local group at that moment.
            var meanwhile = new NTSDeviceModel(station.Node, sentence => {
                                Assert.That(station.Node.TryUpdateNTSConfiguration(local,
                                                                              JObject.Parse("""{ "servers": [ { "id": "local-1", "hostname": "lc1.example.invalid" }, { "id": "lc9", "hostname": "lc9.example.invalid" } ] }"""),
                                                                              out var error,
                                                                              out _),
                                            Is.True, error);
                            });

            var results   = await SetIn(meanwhile, null, Client("local-1", "Priority", "3"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                  Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,  Is.EqualTo("ValueChanged"));
                Assert.That(model.ClientsOf("local"),                          Is.EqualTo(new[] { "local-1", "lc9" }), "the page's server is kept");
                Assert.That(model.ClientValue("local-1", "Priority"),          Is.EqualTo("0"));
            });

        }

        #endregion

        #region AChangeNamesTheCSMSThatMadeIt()

        /// <summary>
        /// The node writes a change of a CSMS down as the CSMS's, and tells it
        /// so: a bridge telling the CSMS of local changes leaves its own out.
        /// </summary>
        [Test]
        public async Task AChangeNamesTheCSMSThatMadeIt()
        {

            var told    = new List<String?>();

            station!.Node.OnNTSGroupChanged += (group, changes, origin) => {
                lock (told)
                    told.Add(origin);
            };

            var before  = station.Node.Log.LastId;
            var results = await Set(null, Group("local", "MaxErrors", "7"));

            Assert.Multiple(() => {

                Assert.That(results.Single().AttributeStatus,                Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(told,                                            Is.EqualTo(new[] { "ocpp:CSMS" }));
                Assert.That(told.All(ChangeOrigin.IsOCPP),                   Is.True);

                Assert.That(station.Node.Log.Recent(100, before, null).Select(entry => entry.Message),
                            Has.Some.EndsWith("(by ocpp:CSMS)."));

            });

        }

        #endregion

        #region WhatTheCSMSMaySetIsReadWrite()

        /// <summary>
        /// The variables the bridge writes are ReadWrite in the OCPP node, the
        /// others ReadOnly - and the components the bridge's own components.
        /// </summary>
        [Test]
        public void WhatTheCSMSMaySetIsReadWrite()
        {

            static OCPP21.MutabilityTypes? MutabilityOf(OCPP21.IDeviceModelNode  Node,
                                                        String                             Component,
                                                        String?                            Instance,
                                                        String                             Variable)

                => Node.GetComponentConfigs(Component).
                        Single(component => String.Equals(component.Instance, Instance, StringComparison.OrdinalIgnoreCase)).
                        VariableConfigs.
                        First(variable => variable.Name == Variable).
                        Attributes?.Mutability;

            var node = station!.OCPPv2_1Node;

            Assert.Multiple(() => {
                Assert.That(MutabilityOf(node, "NTPController",             null,     "ClientGroups"),     Is.EqualTo(OCPP21.MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(node, "NTPController",             null,     "LegalTime"),        Is.EqualTo(OCPP21.MutabilityTypes.ReadOnly));
                Assert.That(MutabilityOf(node, "NTPClientGroupController",  "legal",  "MinServers"),       Is.EqualTo(OCPP21.MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(node, "NTPClientGroupController",  "legal",  "LastCheck"),        Is.EqualTo(OCPP21.MutabilityTypes.ReadOnly));
                Assert.That(MutabilityOf(node, "NTPClientController",       "ptb-a",  "ServerURL"),        Is.EqualTo(OCPP21.MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(node, "NTPClientController",       "ptb-a",  "Mode"),             Is.EqualTo(OCPP21.MutabilityTypes.ReadOnly));
                Assert.That(MutabilityOf(node, "ClockCtrlr",                null,     "DateTime"),         Is.EqualTo(OCPP21.MutabilityTypes.ReadOnly));
                Assert.That(MutabilityOf(node, "ClockCtrlr",                null,     "TimeSource"),       Is.EqualTo(OCPP21.MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(node, "ClockCtrlr",                null,     "TimeAdjustmentReportingThreshold"),  Is.EqualTo(OCPP21.MutabilityTypes.ReadWrite));
            });

        }

        #endregion


        #region (private) Set([Station,] Model, params Entries)

        /// <summary>
        /// SetVariables as the node's OCPP 2.1 node answers it: its
        /// components, and the instances the request makes anew.
        /// </summary>
        private Task<OCPP21.SetVariableResult[]> Set(OCPP21.DataConsistencyModel?  Model,
                                                     params Entry[]                Entries)

            => Set(station!, Model, Entries);

        private static async Task<OCPP21.SetVariableResult[]> Set(IBridgedNode               Station,
                                                                  OCPP21.DataConsistencyModel?  Model,
                                                                  params Entry[]                Entries)
        {

            var node = Station.OCPPv2_1Node;

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(RequestOf(node.Id, Model, Entries),
                                                                  (name, evse) => node.GetComponentConfigs(name, evse),
                                                                  default,
                                                                  node.NewInstanceOf)];

        }

        #endregion

        #region (private static) SetIn(Source, Model, params Entries)

        /// <summary>
        /// The given entries handed to the given source, as the Device Model
        /// hands them: found in controllers of that source, or in those it
        /// makes anew.
        /// </summary>
        private static async Task<OCPP21.SetVariableResult[]> SetIn(NTSDeviceModel                Source,
                                                                    OCPP21.DataConsistencyModel?  Model,
                                                                    params Entry[]                Entries)
        {

            OCPP21.ComponentConfig[] components = [ new OCPP21.ClockController(Source), .. OCPP21.NTPController.AllOf(Source) ];

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(RequestOf(NetworkingNode_Id.Parse("cs"), Model, Entries),
                                                                  (name, evse) => components.Where(component => String.Equals(component.Name, name, StringComparison.OrdinalIgnoreCase)),
                                                                  default,
                                                                  Source.NewInstance)];

        }

        #endregion

        #region (private) Get(Component, Instance, Variable)

        private OCPP21.GetVariableResult Get(String   Component,
                                             String?  Instance,
                                             String   Variable)
        {

            var node = station!.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component(Component, Instance), new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single();

        }

        #endregion

        #region (private) OnDisk(Group)

        /// <summary>
        /// A group as the configuration file says it now.
        /// </summary>
        private JObject? OnDisk(String Group)

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path))["nts"]?["groups"]?[Group] as JObject;

        #endregion

        #region (private static) Entries: Controller, Group, Client, Clock

        /// <summary>
        /// One entry of a request: a variable of a component, to a value, and
        /// what the CSMS read of it last, where it says.
        /// </summary>
        private sealed record Entry(String   Component,
                                    String?  Instance,
                                    String   Variable,
                                    String?  VariableInstance,
                                    String   Value,
                                    String?  Old);

        private static Entry Controller(String Variable, String Value)
            => new ("NTPController", null, Variable, null, Value, null);

        private static Entry Group(String Group, String Variable, String Value, String? Old = null)
            => new ("NTPClientGroupController", Group, Variable, null, Value, Old);

        private static Entry Client(String Client, String Variable, String Value)
            => new ("NTPClientController", Client, Variable, null, Value, null);

        private static Entry Clock(String? Priority, String Value, String Variable = "NtpServerUri")
            => new (OCPP21.ClockController.ComponentName, null, Variable, Priority, Value, null);

        private static OCPP21.CSMS.SetVariablesRequest RequestOf(NetworkingNode_Id             Destination,
                                                                 OCPP21.DataConsistencyModel?  Model,
                                                                 IEnumerable<Entry>            Entries)

            => new (Destination:           SourceRouting.To(Destination),
                    VariableData:          [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                           Component:          new OCPP21.Component(Name: entry.Component, Instance: entry.Instance),
                                                                           Variable:           new OCPP21.Variable (Name: entry.Variable,  Instance: entry.VariableInstance),
                                                                           AttributeValue:     entry.Value,
                                                                           OldAttributeValue:  entry.Old
                                                                       ))],
                    DataConsistencyModel:  Model);

        private static OCPP21.SetVariableStatus[] StatusesOf(IEnumerable<OCPP21.SetVariableResult> Results)
            => [.. Results.Select(result => result.AttributeStatus)];

        #endregion

        #region (private static) Configuration(LocalEnabled)

        /// <summary>
        /// A legal group of four servers - one given an id and pinned to a root,
        /// asked after the others; one on a port of its own; one switched off -
        /// and a local group of one.
        /// </summary>
        private static JObject Configuration(Boolean LocalEnabled)
        {

            var configuration = TestNodes.Offline;

            configuration["nts"] = new JObject(
                                       new JProperty("groups", new JObject(
                                           new JProperty("legal", new JObject(
                                               new JProperty("enabled",                    false),
                                               new JProperty("servers",                    new JArray(
                                                   new JObject(
                                                       new JProperty("id",                 "ptb-a"),
                                                       new JProperty("hostname",           "ptbtime1.example.invalid"),
                                                       new JProperty("priority",           1),
                                                       new JProperty("rootFingerprint",    ARoot),
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
                                               new JProperty("enabled",                    LocalEnabled),
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

    }

}
