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

using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What the CSMS is told of the time servers without asking: security
    /// events and the events of its monitors, queued at the OCPP node of each
    /// version a connection is configured for.
    /// </summary>
    /// <remarks>
    /// The node is built and never started, so its connections are never
    /// dialled and what is told stays in the queues, where the tests read it.
    /// </remarks>
        public abstract class NTSEventsTests : ABridgeTests
    {

        #region Data

        private String            directory  = "";
        private IBridgedNode?  station;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithALegalGroup()
        {

            directory  = TestNodes.TemporaryDirectory("nts-events");
            station    = Nodes.New(directory, Configuration());

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


        #region AChangeOnThePageIsToldToEachVersionThatIsDialled()

        /// <summary>
        /// A change of the legal group made on the page is a
        /// ReconfigurationOfSecurityParameters for the CSMS, naming what changed
        /// and who changed it - queued at both nodes where both versions are
        /// dialled.
        /// </summary>
        [Test]
        public void AChangeOnThePageIsToldToEachVersionThatIsDialled()
        {

            Dial("2.1");
            Dial("1.6");

            ChangeOnThePage("""{ "minServers": 1 }""");

            var told21 = Security(station!.OCPPv2_1Node.OfflineQueue);
            var told16 = Security(station.OCPPv1_6Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told21.Select(told => told.Type),  Is.EqualTo(new[] { "ReconfigurationOfSecurityParameters" }));
                Assert.That(told21.Single().TechInfo,          Does.StartWith("nts.groups.legal: ").And.EndWith("(by web:root)"));
                Assert.That(told16.Select(told => told.Type),  Is.EqualTo(new[] { "ReconfigurationOfSecurityParameters" }));
            });

        }

        #endregion

        #region NothingIsToldWhereNoCSMSIsDialled()

        /// <summary>
        /// A version no connection is configured for is told nothing, and a
        /// node that dials no CSMS at all queues nothing.
        /// </summary>
        [Test]
        public void NothingIsToldWhereNoCSMSIsDialled()
        {

            ChangeOnThePage("""{ "minServers": 1 }""");

            var nowhere = (station!.OCPPv2_1Node.OfflineQueue.Pending.Count, station.OCPPv1_6Node.OfflineQueue.Pending.Count);

            Dial("2.1");

            ChangeOnThePage("""{ "minServers": 2 }""");

            Assert.Multiple(() => {
                Assert.That(nowhere,                                       Is.EqualTo((0, 0)));
                Assert.That(station.OCPPv2_1Node.OfflineQueue.Pending,    Has.Count.EqualTo(1));
                Assert.That(station.OCPPv1_6Node.OfflineQueue.Pending,    Is.Empty,  "1.6 is dialled by no connection");
            });

        }

        #endregion

        #region AChangeThroughOCPPIsNotToldBack()

        /// <summary>
        /// A change the CSMS made itself through OCPP is not told back to it.
        /// </summary>
        [Test]
        public async Task AChangeThroughOCPPIsNotToldBack()
        {

            Dial("2.1");

            var node    = station!.OCPPv2_1Node;
            var results = await OCPP21.DeviceModel.SetVariablesAsync(
                                    new OCPP21.CSMS.SetVariablesRequest(
                                        Destination:   protocols.WWCP.NetworkingNode.SourceRouting.To(node.Id),
                                        VariableData:  [ new OCPP21.SetVariableData(new OCPP21.Component("NTPClientGroupController", "legal"),
                                                                                    new OCPP21.Variable("MinServers"),
                                                                                    "1") ]
                                    ),
                                    (name, evse) => node.GetComponentConfigs(name, evse),
                                    default,
                                    node.NewInstanceOf);

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,  Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(node.OfflineQueue.Pending,          Is.Empty);
            });

        }

        #endregion

        #region TheLegalTimeLimitedAndRestoredIsTold()

        /// <summary>
        /// The legal group reaching MaxErrors is LegalTimeLimited, and its time
        /// back LegalTimeRestored; another group's is nobody's security event.
        /// </summary>
        [Test]
        public void TheLegalTimeLimitedAndRestoredIsTold()
        {

            Dial("2.1");

            var legal = station!.Node.NTSGroups.Single(group => group.IsLegal);
            var local = station.Node.NTSGroups.Single(group => !group.IsLegal);

            station.Bridge.WhenNTSGroupLimited(legal, true);
            station.Bridge.WhenNTSGroupLimited(local, true);
            station.Bridge.WhenNTSGroupLimited(legal, false);

            Assert.That(Security(station.OCPPv2_1Node.OfflineQueue).Select(told => told.Type),
                        Is.EqualTo(new[] { "LegalTimeLimited", "LegalTimeRestored" }));

        }

        #endregion

        #region AnOffsetReachedIsSettingSystemTime()

        /// <summary>
        /// A check that found the clock at least the legal group's threshold
        /// off is SettingSystemTime for the CSMS of each version, with the
        /// offset and the threshold.
        /// </summary>
        [Test]
        public void AnOffsetReachedIsSettingSystemTime()
        {

            Dial("2.1");
            Dial("1.6");

            station!.Bridge.WhenNTSOffsetReached(station.Node.LegalTime, TimeSpan.FromSeconds(-7.25));

            var told21 = Security(station.OCPPv2_1Node.OfflineQueue);
            var told16 = Security(station.OCPPv1_6Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told21.Select(told => told.Type),  Is.EqualTo(new[] { "SettingSystemTime" }));
                Assert.That(told21.Single().TechInfo,          Is.EqualTo("nts.groups.legal: the clock is -7.250 s off, the threshold is 5 s"));
                Assert.That(told16.Select(told => told.Type),  Is.EqualTo(new[] { "SettingSystemTime" }));
            });

        }

        #endregion

        #region ARefusedTimeServerCertificateIsTold()

        /// <summary>
        /// A time server whose certificate chains to no root anybody trusts is
        /// refused, and the CSMS is told InvalidTimeServerCertificate, naming
        /// the server - once, and not again for the same verdict.
        /// </summary>
        [Test]
        public void ARefusedTimeServerCertificateIsTold()
        {

            Dial("2.1");

            using var key    = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var request      = new System.Security.Cryptography.X509Certificates.CertificateRequest("CN=ptbtime1.example.invalid", key, HashAlgorithmName.SHA256);
            using var leaf   = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
            using var chain  = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.Build(leaf);

            var server = DomainName.Parse("ptbtime1.example.invalid");

            station!.Node.JudgeTimeServer(server, leaf, chain, SslPolicyErrors.RemoteCertificateChainErrors);
            station.Node.JudgeTimeServer(server, leaf, chain, SslPolicyErrors.RemoteCertificateChainErrors);

            var told = Security(station.OCPPv2_1Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told.Select(one => one.Type),  Is.EqualTo(new[] { "InvalidTimeServerCertificate" }));
                Assert.That(told.Single().TechInfo,        Does.StartWith("ptbtime1.example.invalid: untrusted"));
            });

        }

        #endregion

        #region AMonitorOnAGroupTellsAChangeMadeOnThePage()

        /// <summary>
        /// A Delta monitor the CSMS set on the legal group's MinServers is told a
        /// change made on the page: a NotifyEvent in the queue, beside the
        /// security event, with the value as it reads now.
        /// </summary>
        [Test]
        public void AMonitorOnAGroupTellsAChangeMadeOnThePage()
        {

            Dial("2.1");

            var node = station!.OCPPv2_1Node;
            var set  = node.VariableMonitors.Set([ new OCPP21.SetMonitoringData(0, OCPP21.MonitorType.Delta, OCPP21.Severities.Alert,
                                                                                new OCPP21.Component("NTPClientGroupController", "legal"),
                                                                                new OCPP21.Variable("MinServers")) ],
                                                 (name, evse) => node.GetComponentConfigs(name, evse));

            ChangeOnThePage("""{ "minServers": 1 }""");

            var events = node.OfflineQueue.Pending.Where(message => message.Kind == "NotifyEvent").
                                                   SelectMany(message => message.Payload["eventData"]!.Children<JObject>()).
                                                   ToArray();

            Assert.Multiple(() => {
                Assert.That(set.Single().Status,                       Is.EqualTo(OCPP21.SetMonitoringStatus.Accepted));
                Assert.That(events.Single().Value<String>("actualValue"),  Is.EqualTo("1"));
                Assert.That(events.Single().Value<String>("trigger"),      Is.EqualTo("Delta"));
            });

        }

        #endregion


        #region (private) Dial(Version) / ChangeOnThePage(Settings) / Security(Queue)

        /// <summary>
        /// A connection of the given version, configured and never dialled.
        /// </summary>
        private void Dial(String Version)
        {
            Assert.That(station!.TryDial("ws://127.0.0.1:9/cs", Version, out var error),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The legal group changed as the page changes it, by root.
        /// </summary>
        private void ChangeOnThePage(String Settings)
        {
            Assert.That(station!.Node.TryUpdateNTSConfiguration(station.Node.NTSGroups.Single(group => group.IsLegal), JObject.Parse(Settings),
                                                           out var error, out _, ChangeOrigin.Web("root")),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The security events waiting in a queue, with their techInfo.
        /// </summary>
        private static (String Type, String? TechInfo)[] Security(OCPPC.OfflineQueue Queue)

            => [.. Queue.Pending.Where (message => message.Kind == "SecurityEventNotification").
                                 Select(message => (message.Payload.Value<String>("type")!, message.Payload.Value<String>("techInfo")))];

        #endregion

        #region (private static) Configuration()

        /// <summary>
        /// A legal group of two servers and a local group of one, under
        /// ".invalid", none of them ever asked.
        /// </summary>
        private static JObject Configuration()
        {

            var configuration = TestNodes.Offline;

            configuration["nts"] = JObject.Parse("""
                                       { "groups": {
                                           "legal": { "enabled": false, "minServers": 2, "servers": [ "ptbtime1.example.invalid", "ptbtime2.example.invalid" ] },
                                           "local": { "enabled": false, "servers": [ "lc1.example.invalid" ] } } }
                                       """);

            return configuration;

        }

        #endregion

    }

}
