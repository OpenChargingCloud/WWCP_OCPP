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
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Certificates;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The test criteria of the Secure DNS white paper (section 5) that no
    /// other fixture tries as the paper states them, against the node: the
    /// order of entries in one request (T1, T2, T2a), a certificate for
    /// another address (T3), which roots vouch for a name server (T4, T5),
    /// which logs a mismatch goes into (T6), what is learned on first use and
    /// when nothing is (T7, T8), a server in clear text beside an encrypted
    /// one (T12), and emptying every address while switched off (T14).
    /// </summary>
    /// <remarks>
    /// The node is built and never started: a handshake is the node's
    /// judgement of the certificate a server showed, said to it directly, and
    /// what is told to the CSMS waits in the queue of the version dialled.
    /// </remarks>
        public abstract class DNSTestCriteriaTests : ABridgeTests
    {

        #region Data

        private const String      ARoot        = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";

        private String            directory    = "";
        private String?           restarted;
        private IBridgedNode?  station;
        private IBridgedNode?  afterARestart;
        private X509Certificate2  certificate  = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithTwoNameServers()
        {

            directory    = TestNodes.TemporaryDirectory("dns-test-criteria");
            certificate  = SelfSigned();

            var configuration = TestNodes.Offline;

            configuration["dns"] = JObject.Parse("""
                                       { "servers": [ { "address": "192.0.2.53",   "transport": "TLS" },
                                                      { "address": "2001:db8::53", "transport": "UDP" } ],
                                         "maxRetries": 2 }
                                       """);

            station      = Nodes.New(directory, configuration);

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (afterARestart is not null)
                await afterARestart.DisposeAsync();

            if (station is not null)
                await station.DisposeAsync();

            afterARestart  = null;
            station        = null;

            certificate.Dispose();

            TestNodes.Remove(directory);

            if (restarted is not null)
                TestNodes.Remove(restarted);

            restarted = null;

        }

        #endregion


        #region APinForATransportThatCannotCarryItIsRefusedAsAWhole()

        /// <summary>
        /// With ACID, a server moved to UDP and held to a root in the same
        /// request, and the client changed beside it: the node refuses the
        /// server, and both of its entries are Rejected, naming it - it
        /// is the two together that cannot be, as neither alone would be
        /// refused. The client's entry is Rolledback, and all is as it was (T1).
        /// </summary>
        [Test]
        public async Task APinForATransportThatCannotCarryItIsRefusedAsAWhole()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("dns-1",  "Transport",  "UDP"),
                                    ("dns-1",  "RootPins",   ARoot),
                                    (null, "MaxRetries", "4"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                                       Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected,
                                                                                                          OCPP21.SetVariableStatus.Rejected,
                                                                                                          OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(results.Take(2).Select(result => result.AttributeStatusInfo?.AdditionalInfo),
                                                                                       Is.All.Contains("'DNSServerController[dns-1]'"));

                Assert.That(Value("dns-1",  "Transport"),                                  Is.EqualTo("TLS"));
                Assert.That(Value("dns-1",  "RootPins"),                                   Is.EqualTo(""));
                Assert.That(Value(null, "MaxRetries"),                                 Is.EqualTo("2"));

            });

        }

        #endregion

        #region APinBeforeItsTransportIsRefusedOnlyEntryByEntry(Atomic, PinsFirst, PinsRefused)

        /// <summary>
        /// A server over UDP given a root and TLS in one request: entry by entry,
        /// the root before the transport is refused, as UDP cannot carry it, and
        /// the transport still taken; the other way round both are. With ACID,
        /// the request is one change, in whichever order (T2).
        /// </summary>
        [TestCase(false, true,  true)]
        [TestCase(false, false, false)]
        [TestCase(true,  true,  false)]
        [TestCase(true,  false, false)]
        public async Task APinBeforeItsTransportIsRefusedOnlyEntryByEntry(Boolean  Atomic,
                                                                          Boolean  PinsFirst,
                                                                          Boolean  PinsRefused)
        {

            (String?, String, String) pins      = ("dns-2", "RootPins",  ARoot);
            (String?, String, String) transport = ("dns-2", "Transport", "TLS");

            var results = await Set(Atomic ? OCPP21.DataConsistencyModel.ACID : OCPP21.DataConsistencyModel.IndependentRequests,
                                    PinsFirst ? [ pins, transport ] : [ transport, pins ]);

            var forPins       = results[PinsFirst ? 0 : 1];
            var forTransport  = results[PinsFirst ? 1 : 0];

            Assert.Multiple(() => {

                Assert.That(forPins.AttributeStatus,       Is.EqualTo(PinsRefused ? OCPP21.SetVariableStatus.Rejected : OCPP21.SetVariableStatus.Accepted));
                Assert.That(forTransport.AttributeStatus,  Is.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(Value("dns-2", "Transport"),       Is.EqualTo("TLS"));
                Assert.That(Value("dns-2", "RootPins"),        Is.EqualTo(PinsRefused ? "" : ARoot));

            });

        }

        #endregion

        #region ANewServerIsNeverAskedInClearText()

        /// <summary>
        /// A new server named in Servers, and its root, TLS and address: entry
        /// by entry, all refused and nothing changed, as each needs the others;
        /// as one change with ACID, all accepted, and the node is changed once,
        /// to a server over TLS held to the root - never to one asked in clear
        /// text (T2a).
        /// </summary>
        [Test]
        public async Task ANewServerIsNeverAskedInClearText()
        {

            var told    = new List<String>();

            station!.Node.OnDNSConfigurationChanged += (changes, origin) => {
                lock (told)
                    told.AddRange(changes);
            };

            (String?, String, String)[] entries = [
                (null,     "Servers",   "dns-1,dns-2,office"),
                ("office", "RootPins",  ARoot),
                ("office", "Transport", "TLS"),
                ("office", "Address",   "203.0.113.53")
            ];

            var independent  = await Set(OCPP21.DataConsistencyModel.IndependentRequests, entries);
            var toldBefore   = told.Count;

            var atomic       = await Set(OCPP21.DataConsistencyModel.ACID, entries);

            Assert.Multiple(() => {

                Assert.That(StatusesOf(independent),                 Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(independent.Select(result => result.AttributeStatusInfo?.AdditionalInfo),
                                                                     Is.All.Contains("with the DataConsistencyModel ACID or BASE"));
                Assert.That(toldBefore,                              Is.Zero, "nothing was changed entry by entry");

                Assert.That(StatusesOf(atomic),                      Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(told.Where(change => change.StartsWith("servers = ", StringComparison.Ordinal)),
                            Has.Exactly(1).Contains("tls://203.0.113.53:853"));
                Assert.That(told,                                    Has.None.Contains("udp://203.0.113.53"));

                Assert.That(Value("office", "RootPins"),             Is.EqualTo(ARoot));

            });

        }

        #endregion

        #region ACertificateForAnotherAddressIsRefusedWhateverItIsHeldTo()

        /// <summary>
        /// A name server held to the very certificate it shows, whose root the
        /// store trusts for name servers - but issued for another address: it is
        /// refused, as WrongName, a Problem, and InvalidNameServerCertificate for
        /// the CSMS (T3).
        /// </summary>
        [Test]
        public void ACertificateForAnotherAddressIsRefusedWhateverItIsHeldTo()
        {

            Change($$"""
                   { "servers": [ { "address": "192.0.2.53", "transport": "TLS", "certificateFingerprint": "{{Fingerprint(certificate)}}" } ] }
                   """);

            TrustForNameServers(certificate);
            Dial("2.1");

            Judge(SslPolicyErrors.RemoteCertificateNameMismatch);

            var told = station!.OCPPv2_1Node.OfflineQueue.Pending.Where(message => message.Kind == "SecurityEventNotification").
                                                                  Select(message => message.Payload.Value<String>("type"));

            Assert.Multiple(() => {
                Assert.That(Value("dns-1", "CertificateStatus"),  Is.EqualTo("WrongName"));
                Assert.That(Value("dns-1", "Problem"),            Is.EqualTo("true"));
                Assert.That(told,                             Is.EqualTo(new[] { "InvalidNameServerCertificate" }));
            });

        }

        #endregion

        #region AFingerprintThatDoesNotMatchIsLoggedAsItsServerSays(OnMismatch, Outcome, Metrological)

        /// <summary>
        /// A server whose certificate is not the one it is held to: refused, or
        /// used and written into the security log - and, where its entry says
        /// "record", into the metrological log as well (T6).
        /// </summary>
        [TestCase("refuse",  "PinMismatch",  false)]
        [TestCase("accept",  "Tolerated",    false)]
        [TestCase("record",  "Recorded",     true)]
        public void AFingerprintThatDoesNotMatchIsLoggedAsItsServerSays(String   OnMismatch,
                                                                        String   Outcome,
                                                                        Boolean  Metrological)
        {

            Change($$"""
                   { "servers": [ { "address": "192.0.2.53", "transport": "TLS", "certificateFingerprint": "{{ARoot}}", "onMismatch": "{{OnMismatch}}" } ] }
                   """);

            TrustForNameServers(certificate);

            var before = station!.Node.Log.LastId;

            Judge(SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.Multiple(() => {
                Assert.That(Value("dns-1", "CertificateStatus"),                                              Is.EqualTo(Outcome));
                Assert.That(station.Node.Log.Recent(100, before, "security").Select(entry => entry.Message),   Has.Some.Contains(Fingerprint(certificate)).IgnoreCase);
                Assert.That(station.Node.Log.Recent(100, before, null).Any(entry => entry.Metrological),      Is.EqualTo(Metrological));
            });

        }

        #endregion

        #region OnlyARootForNameServersVouchesForANameServer(Type, Outcome)

        /// <summary>
        /// A name server's root installed by the CSMS as the root of another
        /// service vouches for no name server: untrusted (T4). Installed as a
        /// NameServerRootCertificate, it does (T5).
        /// </summary>
        [TestCase("CSMSRootCertificate",                    "Untrusted")]
        [TestCase(NTSRootCertificates.CertificateType,      "Untrusted")]
        [TestCase(DNSRootCertificates.CertificateType,      "Accepted")]
        public async Task OnlyARootForNameServersVouchesForANameServer(String  Type,
                                                                       String  Outcome)
        {

            var installed = await station!.OCPPv2_1Node.CertificateSources.InstallAsync(Type, null, OCPPC.Certificate.Parse(certificate.ExportCertificatePem()), "CSMS");

            Judge(SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.Multiple(() => {
                Assert.That(installed.Status,                 Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(Value("dns-1", "CertificateStatus"),  Is.EqualTo(Outcome));
            });

        }

        #endregion

        #region ARootLearnedOnFirstUseIsHeldAfterARestart()

        /// <summary>
        /// A root learned on first use is in the file, and a node started
        /// from it holds the server to it (T7).
        /// </summary>
        [Test]
        public void ARootLearnedOnFirstUseIsHeldAfterARestart()
        {

            Change("""
                   { "servers": [ { "address": "192.0.2.53", "transport": "TLS", "trustOnFirstUse": "root" } ] }
                   """);

            TrustForNameServers(certificate);

            Judge(SslPolicyErrors.RemoteCertificateChainErrors);

            restarted      = TestNodes.TemporaryDirectory("dns-test-criteria-restarted");
            afterARestart  = Nodes.New(restarted, JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path)));

            Assert.That(afterARestart.Node.DNSConfigurationJSON()["servers"]?[0]?["rootFingerprint"]?.Value<String>(),
                        Is.EqualTo(Fingerprint(certificate)).IgnoreCase);

        }

        #endregion

        #region AServerHeldToARootAlreadyLearnsNone()

        /// <summary>
        /// A server that learns its root on first use, and holds one already:
        /// believed through it, and nothing learned or changed (T8).
        /// </summary>
        [Test]
        public void AServerHeldToARootAlreadyLearnsNone()
        {

            Change($$"""
                   { "servers": [ { "address": "192.0.2.53", "transport": "TLS", "trustOnFirstUse": "root", "rootFingerprint": "{{Fingerprint(certificate)}}" } ] }
                   """);

            TrustForNameServers(certificate);

            var told = new List<String>();

            station!.Node.OnDNSConfigurationChanged += (changes, origin) => {
                lock (told)
                    told.AddRange(changes);
            };

            Judge(SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.Multiple(() => {
                Assert.That(Value("dns-1", "CertificateStatus"),  Is.EqualTo("Accepted"));
                Assert.That(told,                             Is.Empty);
                Assert.That(Value("dns-1", "RootPins"),           Is.EqualTo(Fingerprint(certificate)).IgnoreCase);
            });

        }

        #endregion

        #region OnlyEncryptedServersAreAskedAndNoneComesInClearText()

        /// <summary>
        /// One server over TLS and one over UDP, where whoever is on the path
        /// answers first: with EncryptedOnly, the one over UDP stays and is no
        /// longer asked, a new one over UDP and the one over TLS moved to TCP
        /// are Rejected, naming them, and nothing is changed; switched off, the
        /// one over UDP is asked again (T12).
        /// </summary>
        [Test]
        public async Task OnlyEncryptedServersAreAskedAndNoneComesInClearText()
        {

            var on       = await Set(null, (null, "EncryptedOnly", "true"));
            var asked    = station!.Node.DNSClient.DNSServers.Select(server => server.ToString()).ToArray();

            var added    = await Set(OCPP21.DataConsistencyModel.ACID,
                                     (null,     "Servers",   "dns-1,dns-2,office"),
                                     ("office", "Address",   "203.0.113.53"));

            var moved    = await Set(null, ("dns-1", "Transport", "TCP"));

            var off      = await Set(null, (null, "EncryptedOnly", "false"));

            Assert.Multiple(() => {

                Assert.That(on.Single().AttributeStatus,                    Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(asked,                                          Is.EqualTo(new[] { "tls://192.0.2.53:853" }));

                Assert.That(StatusesOf(added),                              Is.All.Not.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(added[1].AttributeStatusInfo?.AdditionalInfo,   Does.StartWith("'DNSServerController[office]' would be asked over UDP, in clear text"));

                Assert.That(moved.Single().AttributeStatus,                 Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(moved.Single().AttributeStatusInfo?.AdditionalInfo,  Does.StartWith("'DNSServerController[dns-1]' would be asked over TCP, in clear text"));

                Assert.That(Value(null,    "Servers"),                      Is.EqualTo("dns-1,dns-2"));
                Assert.That(Value("dns-1", "Transport"),                    Is.EqualTo("TLS"));
                Assert.That(Value("dns-2", "Address"),                      Is.EqualTo("2001:db8::53"));

                Assert.That(off.Single().AttributeStatus,                   Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(station.Node.DNSClient.DNSServers.Count(),           Is.EqualTo(2));

            });

        }

        #endregion

        #region NoServerAtAllIsRefusedSwitchedOffToo()

        /// <summary>
        /// Name resolution switched off, then Servers naming none: refused as it
        /// is while switched on, and the servers are kept (T14; the first half
        /// is DNSDeviceModelWritingTests.NoServerAtAllIsRefused).
        /// </summary>
        [Test]
        public async Task NoServerAtAllIsRefusedSwitchedOffToo()
        {

            var off      = await Set(null, (null, "Enabled", "false"));

            var emptied  = await Set(OCPP21.DataConsistencyModel.ACID,
                                     (null, "Servers", ""));

            Assert.Multiple(() => {
                Assert.That(off.Single().AttributeStatus,            Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(StatusesOf(emptied),                     Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(Value(null, "Enabled"),                  Is.EqualTo("false"));
                Assert.That(Value("dns-1",  "Address"),                  Is.EqualTo("192.0.2.53"));
                Assert.That(Value("dns-2",  "Address"),                  Is.EqualTo("2001:db8::53"));
            });

        }

        #endregion


        #region (private) Set(Model, params Entries) / Value(Server, Variable)

        /// <summary>
        /// SetVariables of the DNS controllers as the node's OCPP 2.1 node
        /// answers it: (null, variable, value) for the client, (id, ...) for a
        /// server.
        /// </summary>
        private async Task<OCPP21.SetVariableResult[]> Set(OCPP21.DataConsistencyModel?                             Model,
                                                           params (String? Server, String Variable, String Value)[]  Entries)
        {

            var node    = station!.OCPPv2_1Node;

            var request = new OCPP21.CSMS.SetVariablesRequest(
                              Destination:           SourceRouting.To(node.Id),
                              VariableData:          [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                                     Component:       new OCPP21.Component(Name:     entry.Server is null ? DNSDeviceModel.ClientComponent : DNSDeviceModel.ServerComponent,
                                                                                                                           Instance: entry.Server),
                                                                                     Variable:        new OCPP21.Variable(Name: entry.Variable),
                                                                                     AttributeValue:  entry.Value
                                                                                 ))],
                              DataConsistencyModel:  Model
                          );

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(request, (name, evse) => node.GetComponentConfigs(name, evse), default, node.NewInstanceOf)];

        }

        /// <summary>
        /// What a CSMS reads of a variable through GetVariables.
        /// </summary>
        private String? Value(String?  Server,
                              String   Variable)
        {

            var node = station!.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component(Server is null ? DNSDeviceModel.ClientComponent : DNSDeviceModel.ServerComponent, Server),
                                                                                new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single().AttributeValue;

        }

        #endregion

        #region (private) Change(Section) / Dial(Version) / TrustForNameServers(Root) / Judge(PolicyErrors)

        /// <summary>
        /// The name resolution changed as the page changes it.
        /// </summary>
        private void Change(String Section)
        {
            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse(Section), out var error),
                        Is.True,
                        error);
        }

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
        /// The given certificate as a root of the node's store for name servers.
        /// </summary>
        private void TrustForNameServers(X509Certificate2 Root)
        {
            Assert.That(station!.Node.Certificates.Import(Encoding.ASCII.GetBytes(Root.ExportCertificatePem()), CertificateKind.TLSRoot, null, null,
                                                     [ CertificateUsages.DNS ], out _, out var error),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The first name server showed the test's certificate, with what the
        /// handshake found wrong with it.
        /// </summary>
        private void Judge(SslPolicyErrors PolicyErrors)
        {

            using var chain = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.Build(certificate);

            station!.Node.JudgeNameServer(station.Node.DNSClient.DNSServers.First(), certificate, chain, PolicyErrors);

        }

        #endregion

        #region (private static) StatusesOf(Results) / SelfSigned() / Fingerprint(Certificate)

        private static OCPP21.SetVariableStatus[] StatusesOf(IEnumerable<OCPP21.SetVariableResult> Results)

            => [.. Results.Select(result => result.AttributeStatus)];

        /// <summary>
        /// A certificate for the first name server's address that signed itself.
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
