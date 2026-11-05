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

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What the CSMS is told of the name servers without asking: the security
    /// events of the Secure DNS white paper (section 3.1.4) and the events of
    /// its monitors, queued at the OCPP node of each version a connection is
    /// configured for.
    /// </summary>
    /// <remarks>
    /// The node has one name server over TLS and is never started, so its
    /// connections are never dialled and what is told stays in the queues,
    /// where the tests read it. A handshake is the node's judgement of the
    /// certificate a server showed, said to it directly.
    /// </remarks>
        public abstract class DNSEventsTests : ABridgeTests
    {

        #region Data

        private const String      Server       = "tls://192.0.2.53:853";

        private String            directory    = "";
        private IBridgedNode?  station;
        private X509Certificate2  certificate  = default!;
        private X509Certificate2  another      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithANameServerOverTLS()
        {

            directory    = TestNodes.TemporaryDirectory("dns-events");
            certificate  = SelfSigned();
            another      = SelfSigned();

            var configuration = TestNodes.Offline;

            configuration["dns"] = JObject.Parse("""
                                       { "servers": [ { "address": "192.0.2.53", "transport": "TLS" } ], "maxRetries": 2 }
                                       """);

            station      = Nodes.New(directory, configuration);

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            certificate.Dispose();
            another.    Dispose();

            TestNodes.Remove(directory);

        }

        #endregion


        #region AChangeOnThePageIsToldToEachVersionThatIsDialled()

        /// <summary>
        /// A change of the name resolution made on the page is a
        /// ReconfigurationOfSecurityParameters for the CSMS, naming what changed
        /// and who changed it - queued at both nodes where both versions are
        /// dialled, and written into the log among the name servers' lines.
        /// </summary>
        [Test]
        public void AChangeOnThePageIsToldToEachVersionThatIsDialled()
        {

            Dial("2.1");
            Dial("1.6");

            var before = station!.Node.Log.LastId;

            ChangeOnThePage("""{ "maxRetries": 3 }""");

            var told21 = Security(station.OCPPv2_1Node.OfflineQueue);
            var told16 = Security(station.OCPPv1_6Node.OfflineQueue);

            Assert.Multiple(() => {

                Assert.That(told21,  Is.EqualTo(new[] { ("ReconfigurationOfSecurityParameters", (String?) "dns: max retries = 3 (by web:root)") }));
                Assert.That(told16,  Is.EqualTo(told21));

                Assert.That(station.Node.Log.Recent(100, before, "dns").Select(entry => entry.Message),
                            Has.Some.StartWith("Security event ReconfigurationOfSecurityParameters for the CSMS: dns: "));

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
                                        VariableData:  [ new OCPP21.SetVariableData(new OCPP21.Component(DNSDeviceModel.ClientComponent),
                                                                                    new OCPP21.Variable("MaxRetries"),
                                                                                    "3") ]
                                    ),
                                    (name, evse) => node.GetComponentConfigs(name, evse));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,  Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(station.Node.DNSConfigurationJSON()["settings"]?["maxRetries"]?.Value<Int32>(),  Is.EqualTo(3));
                Assert.That(Security(node.OfflineQueue),        Is.Empty);
            });

        }

        #endregion

        #region ARefusedNameServerCertificateIsToldOnce()

        /// <summary>
        /// A name server whose certificate chains to no root anybody trusts is
        /// refused, and the CSMS is told InvalidNameServerCertificate, naming
        /// the server and the certificate - once, and not again for the same
        /// verdict (T10).
        /// </summary>
        [Test]
        public void ARefusedNameServerCertificateIsToldOnce()
        {

            Dial("2.1");

            Judge();
            Judge();

            var told = Security(station!.OCPPv2_1Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told.Select(one => one.Type),  Is.EqualTo(new[] { "InvalidNameServerCertificate" }));
                Assert.That(told.Single().TechInfo,        Is.EqualTo($"{Server}: untrusted, certificate {Fingerprint(certificate)}, root none"));
            });

        }

        #endregion

        #region AFingerprintThatDoesNotMatchIsTold(OnMismatch, Type, Outcome)

        /// <summary>
        /// A server whose certificate is not the one it is held to is told as
        /// refused where it is refused for it, and as used all the same where its
        /// configuration says so - recorded or not (T6).
        /// </summary>
        [TestCase("refuse",  "InvalidNameServerCertificate",    "pinMismatch")]
        [TestCase("accept",  "NameServerPinMismatchTolerated",  "tolerated")]
        [TestCase("record",  "NameServerPinMismatchTolerated",  "recorded")]
        public void AFingerprintThatDoesNotMatchIsTold(String  OnMismatch,
                                                       String  Type,
                                                       String  Outcome)
        {

            ChangeOnThePage($$"""
                            { "servers": [ { "address": "192.0.2.53", "transport": "TLS",
                                             "certificateFingerprint": "{{Fingerprint(another)}}", "onMismatch": "{{OnMismatch}}" } ] }
                            """);

            TrustForNameServers(certificate);
            Dial("2.1");

            Judge();

            var told = Security(station!.OCPPv2_1Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told.Select(one => one.Type),  Is.EqualTo(new[] { Type }));
                Assert.That(told.Single().TechInfo,        Does.StartWith($"{Server}: {Outcome}, certificate {Fingerprint(certificate)}, root {Fingerprint(certificate)}"));
            });

        }

        #endregion

        #region AnotherCertificateThanBeforeIsTold()

        /// <summary>
        /// A server believed with another certificate than it was before is
        /// NameServerCertificateChanged, with the one before and the one now
        /// (T9).
        /// </summary>
        [Test]
        public void AnotherCertificateThanBeforeIsTold()
        {

            TrustForNameServers(certificate);
            station!.Node.KnownServers.Observe(CertificateUsages.DNS, Server, Fingerprint(another), null, DateTimeOffset.UtcNow.AddDays(-3));

            Dial("2.1");

            Judge();

            var told = Security(station.OCPPv2_1Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told.Select(one => one.Type),  Is.EqualTo(new[] { "NameServerCertificateChanged" }));
                Assert.That(told.Single().TechInfo,        Is.EqualTo($"{Server}: certificate {Fingerprint(another)} -> {Fingerprint(certificate)}, " +
                                                                      $"root unknown -> {Fingerprint(certificate)}"));
            });

        }

        #endregion

        #region APinLearnedOnFirstUseIsToldOnce()

        /// <summary>
        /// A root learned on first use is a change of the configuration by
        /// "tofu", told as ReconfigurationOfSecurityParameters (T7) - once, and
        /// not again as the news of the handshake it was learned at.
        /// </summary>
        [Test]
        public void APinLearnedOnFirstUseIsToldOnce()
        {

            ChangeOnThePage("""
                            { "servers": [ { "address": "192.0.2.53", "transport": "TLS", "trustOnFirstUse": "root" } ] }
                            """);

            TrustForNameServers(certificate);
            Dial("2.1");

            Judge();
            Judge();

            var told = Security(station!.OCPPv2_1Node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(told.Select(one => one.Type),  Is.EqualTo(new[] { "ReconfigurationOfSecurityParameters" }));
                Assert.That(told.Single().TechInfo,        Does.StartWith($"dns: {Server} held to ").And.EndWith("(by tofu)"));
                Assert.That(station.Node.DNSConfigurationJSON()["servers"]?[0]?["rootFingerprint"]?.Value<String>(),  Is.EqualTo(Fingerprint(certificate)).IgnoreCase);
            });

        }

        #endregion

        #region AMonitorIsToldWhatChangedWithoutTheCSMS()

        /// <summary>
        /// Delta monitors the CSMS set on DNSClientController and on a server are
        /// told a change made on the page and a server refused at a handshake:
        /// NotifyEvents in the queue, beside the security events, with the
        /// values as they read now.
        /// </summary>
        [Test]
        public void AMonitorIsToldWhatChangedWithoutTheCSMS()
        {

            Dial("2.1");

            var node = station!.OCPPv2_1Node;
            var set  = node.VariableMonitors.Set([ new OCPP21.SetMonitoringData(0, OCPP21.MonitorType.Delta, OCPP21.Severities.Alert,
                                                                                new OCPP21.Component(DNSDeviceModel.ClientComponent),
                                                                                new OCPP21.Variable("MaxRetries")),
                                                   new OCPP21.SetMonitoringData(0, OCPP21.MonitorType.Delta, OCPP21.Severities.Alert,
                                                                                new OCPP21.Component(DNSDeviceModel.ServerComponent, "dns-1"),
                                                                                new OCPP21.Variable("Problem")) ],
                                                 (name, evse) => node.GetComponentConfigs(name, evse));

            ChangeOnThePage("""{ "maxRetries": 3 }""");
            var afterTheChange = Events(node.OfflineQueue);

            Judge();
            var afterTheHandshake = Events(node.OfflineQueue);

            Assert.Multiple(() => {
                Assert.That(set.Select(one => one.Status),  Is.All.EqualTo(OCPP21.SetMonitoringStatus.Accepted));
                Assert.That(afterTheChange,                 Is.EqualTo(new[] { ("MaxRetries", "3") }));
                Assert.That(afterTheHandshake,              Is.EqualTo(new[] { ("MaxRetries", "3"), ("Problem", "true") }));
            });

        }

        #endregion


        #region (private) Dial(Version) / ChangeOnThePage(Settings) / TrustForNameServers(Root) / Judge()

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
        /// The name resolution changed as the page changes it, by root.
        /// </summary>
        private void ChangeOnThePage(String Settings)
        {
            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse(Settings), out var error, out _, ChangeOrigin.Web("root")),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The given certificate as a root of the node's store for name servers.
        /// </summary>
        private void TrustForNameServers(X509Certificate2 Root)
        {
            Assert.That(station!.Node.Certificates.Import(System.Text.Encoding.ASCII.GetBytes(Root.ExportCertificatePem()), CertificateKind.TLSRoot, null, null,
                                                     [ CertificateUsages.DNS ], out _, out var error),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The name server showed the test's certificate, which the machine
        /// trusts no root of.
        /// </summary>
        private void Judge()
        {

            using var chain = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.Build(certificate);

            station!.Node.JudgeNameServer(station.Node.DNSClient.DNSServers.Single(), certificate, chain, SslPolicyErrors.RemoteCertificateChainErrors);

        }

        #endregion

        #region (private static) Security(Queue) / Events(Queue)

        /// <summary>
        /// The security events waiting in a queue, with their techInfo.
        /// </summary>
        private static (String Type, String? TechInfo)[] Security(OCPPC.OfflineQueue Queue)

            => [.. Queue.Pending.Where (message => message.Kind == "SecurityEventNotification").
                                 Select(message => (message.Payload.Value<String>("type")!, message.Payload.Value<String>("techInfo")))];

        /// <summary>
        /// The monitor events waiting in a queue: which variable, and what it
        /// reads now.
        /// </summary>
        private static (String Variable, String Value)[] Events(OCPPC.OfflineQueue Queue)

            => [.. Queue.Pending.Where     (message => message.Kind == "NotifyEvent").
                                 SelectMany(message => message.Payload["eventData"]!.Children<JObject>()).
                                 Select    (data    => (data["variable"]?.Value<String>("name") ?? "", data.Value<String>("actualValue") ?? ""))];

        #endregion

        #region (private static) SelfSigned() / Fingerprint(Certificate)

        /// <summary>
        /// A certificate for the name server's address that signed itself.
        /// </summary>
        private static X509Certificate2 SelfSigned()
        {

            using var key  = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var request    = new CertificateRequest("CN=192.0.2.53", key, HashAlgorithmName.SHA256);
            var names      = new SubjectAlternativeNameBuilder();

            names.AddIpAddress(System.Net.IPAddress.Parse("192.0.2.53"));

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(names.Build());

            return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddDays(1));

        }

        private static String Fingerprint(X509Certificate2 Certificate)
            => CertificateEntry.ThumbprintOf(Certificate);

        #endregion

    }

}
