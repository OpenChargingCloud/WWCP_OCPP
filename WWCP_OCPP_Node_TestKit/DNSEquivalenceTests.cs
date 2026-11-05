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
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Certificates;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// One name resolution, three ways to change it: the web page's PUT
    /// /api/v1/configuration/dns, a CSMS of OCPP 2.1 through SetVariables, and
    /// a central system of OCPP 1.6 through its configuration keys. The same
    /// change leaves a file the next start reads the same and the same state,
    /// and says so in the same words but for who made it; what one way changed,
    /// the others read; neither overwrites what the other changed meanwhile;
    /// and what the file cannot take, none of them changes (Secure DNS concept
    /// B.14 step 9).
    /// </summary>
    /// <remarks>
    /// The node the page saves to is started and signed in to; the twins changed
    /// through OCPP are built from the same configuration and never started.
    /// Nothing asks the name servers of the configuration, which are in the
    /// documentation's ranges; where a name is resolved, it is by name servers
    /// of Hermod's own on the loopback, over TLS.
    /// </remarks>
        public abstract class DNSEquivalenceTests : AStartedBridgeTests
    {

        #region Data

        private const String  ARoot    = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  APin     = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private const String  DNSPath  = "api/v1/configuration/dns";

        private const String  CSMS     = "csms.example.test";

        private static readonly Regex origin = new (@" \(by [^)]*\)", RegexOptions.CultureInvariant);

        private readonly List<(IBridgedNode Station, String Directory)> twins = [];

        #endregion

        #region Configuration / TearDown

        /// <summary>
        /// Three name servers: one over TLS, held to a root and recording a
        /// fingerprint that does not match; one over UDP; one over TCP on a
        /// port and with a timeout of its own.
        /// </summary>
        protected override JObject Configuration
        {
            get
            {

                var configuration = TestNodes.Offline;

                configuration["dns"] = JObject.Parse($$"""
                                           { "servers": [
                                               { "address": "192.0.2.53",    "transport": "TLS", "rootFingerprint": "{{ARoot}}", "onMismatch": "record" },
                                               { "address": "2001:db8::53",  "transport": "UDP" },
                                               { "address": "198.51.100.53", "transport": "TCP", "port": 5353, "queryTimeoutSeconds": 3 } ],
                                             "maxRetries": 2 }
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
        /// The client's settings changed on the page, through SetVariables and
        /// through the configuration keys of OCPP 1.6 leave a file that the next
        /// start reads the same and the same name resolution in each node - and,
        /// for the page and SetVariables, which change them at once, the same
        /// log line but for who made the change.
        /// </summary>
        /// <remarks>
        /// The same file as the next start reads it, not as it is written: the
        /// page sends every setting of its form and every server with its port,
        /// and the node writes what it was sent, while OCPP sends only what it
        /// changes. A setting written out at its default is the default.
        /// </remarks>
        [Test]
        public async Task TheSameSettingsLeaveTheSameFileWhicheverWayTheyCame()
        {

            using var http  = await SignedIn();

            var pageLog     = Station.Node.Log.LastId;

            Assert.That(await SaveOnThePage(http, AsLoaded(), settings => {
                                                                settings["maxRetries"]           = 4;
                                                                settings["useCache"]             = false;
                                                                settings["dnssecOK"]             = true;
                                                                settings["recursionDesired"]     = false;
                                                                settings["queryTimeoutSeconds"]  = 5;
                                                                settings["maxCNAMEFollows"]      = 4;
                                                                settings["encryptedOnly"]        = true;
                                                            }),
                        Is.EqualTo(HttpStatusCode.OK));

            var viaOCPP21   = Twin();
            var ocppLog     = viaOCPP21.Node.Log.LastId;

            var set         = await Set(viaOCPP21, OCPP21.DataConsistencyModel.ACID,
                                        Client("MaxRetries",        "4"),
                                        Client("UseCache",          "false"),
                                        Client("DNSSECOK",          "true"),
                                        Client("RecursionDesired",  "Unset"),
                                        Client("QueryTimeout",      "5"),
                                        Client("MaxCNAMEFollows",   "4"),
                                        Client("EncryptedOnly",     "true"));

            var viaOCPP16   = Twin();
            var keys        = KeysOf(viaOCPP16);
            var changed     = new[] {
                                  await keys.ChangeAsync("dns.maxRetries",        "4",      "CS-16"),
                                  await keys.ChangeAsync("dns.useCache",          "false",  "CS-16"),
                                  await keys.ChangeAsync("dns.dnssecOK",          "true",   "CS-16"),
                                  await keys.ChangeAsync("dns.recursionDesired",  "Unset",  "CS-16"),
                                  await keys.ChangeAsync("dns.queryTimeout",      "5",      "CS-16"),
                                  await keys.ChangeAsync("dns.maxCNAMEFollows",   "4",      "CS-16"),
                                  await keys.ChangeAsync("dns.encryptedOnly",     "true",   "CS-16")
                              };

            var pageLine          = ChangeLine(Station,   pageLog);
            var ocppLine          = ChangeLine(viaOCPP21, ocppLog);

            var fromThePagesFile  = AsTheNextStartReadsIt(Station);
            var fromTheFile21     = AsTheNextStartReadsIt(viaOCPP21);
            var fromTheFile16     = AsTheNextStartReadsIt(viaOCPP16);

            Assert.Multiple(() => {

                Assert.That(set.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(changed,                                       Is.All.EqualTo(OCPP16.ConfigurationStatus.Accepted));

                Assert.That(fromTheFile21.ToString(),         Is.EqualTo(fromThePagesFile.ToString()),    "SetVariables left another file than the page");
                Assert.That(fromTheFile16.ToString(),         Is.EqualTo(fromThePagesFile.ToString()),    "the 1.6 keys left another file than the page");

                Assert.That(Held(viaOCPP21).ToString(),       Is.EqualTo(Held(Station).ToString()),       "the node resolves otherwise after SetVariables");
                Assert.That(Held(viaOCPP16).ToString(),       Is.EqualTo(Held(Station).ToString()),       "the node resolves otherwise after the 1.6 keys");
                Assert.That(fromThePagesFile.ToString(),      Is.EqualTo(Held(Station).ToString()),       "the next start reads otherwise than the page's save made");

                Assert.That(pageLine,                         Does.EndWith("(by web:root)."));
                Assert.That(ocppLine,                         Does.EndWith("(by ocpp:CSMS)."));
                Assert.That(origin.Replace(ocppLine, ""),     Is.EqualTo(origin.Replace(pageLine, "")));

            });

        }

        #endregion

        #region TheSameServersLeaveTheSameFileWhicheverWayTheyCame()

        /// <summary>
        /// The first server taken away, the one behind it moved to TLS to learn
        /// its root on first use, and the last given another timeout: on the
        /// page, which sends what is left of the list with the ids it was shown,
        /// and through SetVariables, which leaves the first out of Servers and
        /// changes the two behind it by their ids in the same request, with the
        /// port of TLS left to the transport. The same file as the next start
        /// reads it, the same servers with the same ids, the same log line but
        /// for who.
        /// </summary>
        [Test]
        public async Task TheSameServersLeaveTheSameFileWhicheverWayTheyCame()
        {

            using var http  = await SignedIn();

            var pageLog     = Station.Node.Log.LastId;

            Assert.That(await SaveOnThePage(http, """
                                                  [ { "id": "dns-2", "address": "2001:db8::53",  "port": 853,  "transport": "TLS", "queryTimeoutSeconds": null,
                                                      "trustOnFirstUse": "root", "pinsAsShown": { } },
                                                    { "id": "dns-3", "address": "198.51.100.53", "port": 5353, "transport": "TCP", "queryTimeoutSeconds": 4, "pinsAsShown": { } } ]
                                                  """),
                        Is.EqualTo(HttpStatusCode.OK));

            var viaOCPP21   = Twin();
            var ocppLog     = viaOCPP21.Node.Log.LastId;

            var set         = await Set(viaOCPP21, OCPP21.DataConsistencyModel.ACID,
                                        Client("Servers",                   "dns-2,dns-3"),
                                        Server("dns-2", "Transport",        "TLS"),
                                        Server("dns-2", "TrustOnFirstUse",  "Root"),
                                        Server("dns-3", "QueryTimeout",     "4"));

            var pageLine          = ChangeLine(Station,   pageLog);
            var ocppLine          = ChangeLine(viaOCPP21, ocppLog);

            var fromThePagesFile  = AsTheNextStartReadsIt(Station);
            var fromTheFile21     = AsTheNextStartReadsIt(viaOCPP21);

            Assert.Multiple(() => {

                Assert.That(set.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(fromTheFile21.ToString(),         Is.EqualTo(fromThePagesFile.ToString()),    "SetVariables left another file than the page");
                Assert.That(Held(viaOCPP21).ToString(),       Is.EqualTo(Held(Station).ToString()),       "the node resolves otherwise after SetVariables");
                Assert.That(Held(viaOCPP21)["servers"]?[0]?.Value<Int32>("port"),  Is.EqualTo(853), "the port of TLS");

                Assert.That(origin.Replace(ocppLine, ""),     Is.EqualTo(origin.Replace(pageLine, "")));

            });

        }

        #endregion

        #region WhatThePageChangedIsReadByTheCSMSAndTheOtherWayRound()

        /// <summary>
        /// What the page changed, a CSMS reads at once through GetVariables and a
        /// central system through GetConfiguration; what a CSMS changed, the page
        /// reads at once through GET.
        /// </summary>
        [Test]
        public async Task WhatThePageChangedIsReadByTheCSMSAndTheOtherWayRound()
        {

            using var http  = await SignedIn();

            await SaveOnThePage(http, AsLoaded(), settings => settings["maxRetries"] = 3);

            var read21      = Get(Station, null, "MaxRetries");
            var read16      = KeysOf(Station).Get("dns.maxRetries")?.Value;

            var set         = await Set(Station, null, Server("dns-2", "Port", "5300"));

            var page        = JObject.Parse(await http.GetStringAsync(DNSPath));

            Assert.Multiple(() => {
                Assert.That(read21.AttributeValue,                         Is.EqualTo("3"));
                Assert.That(read16,                                        Is.EqualTo("3"));
                Assert.That(set.Single().AttributeStatus,                  Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(page["servers"]?[1]?.Value<Int32>("port"),     Is.EqualTo(5300));
            });

        }

        #endregion

        #region APageChangeBetweenTheCSMSsReadAndItsWriteIsNotOverwritten()

        /// <summary>
        /// A CSMS reads the port of the second server, the page changes it, and
        /// the CSMS's write that names the port it read is refused as
        /// ValueChanged: the page's port stays, in effect and in the file.
        /// </summary>
        [Test]
        public async Task APageChangeBetweenTheCSMSsReadAndItsWriteIsNotOverwritten()
        {

            using var http  = await SignedIn();

            var seen        = Get(Station, "dns-2", "Port").AttributeValue;

            await SaveOnThePage(http, AsLoaded(SecondPort: 5300));

            var set         = await Set(Station, null, Server("dns-2", "Port", "5353", Old: seen));

            Assert.Multiple(() => {
                Assert.That(seen,                                                     Is.EqualTo("53"));
                Assert.That(set.Single().AttributeStatus,                             Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(set.Single().AttributeStatusInfo?.ReasonCode,             Is.EqualTo("ValueChanged"));
                Assert.That(Held     (Station)["servers"]?[1]?.Value<Int32>("port"),  Is.EqualTo(5300));
                Assert.That(DNSOnDisk(Station)["servers"]?[1]?.Value<Int32>("port"),  Is.EqualTo(5300));
            });

        }

        #endregion

        #region APinTheCSMSSetWhileThePageWasOpenIsKept()

        /// <summary>
        /// The page loads the servers, a CSMS holds the first one to a
        /// certificate as well, and the page saves another server's port: the
        /// CSMS's pin is one the page never showed, and the save keeps it, as
        /// it keeps one learned on first use. A pin the page showed and does not
        /// send back is taken away.
        /// </summary>
        [Test]
        public async Task APinTheCSMSSetWhileThePageWasOpenIsKept()
        {

            using var http  = await SignedIn();

            var set         = await Set(Station, null, Server("dns-1", "CertificatePins", APin));

            await SaveOnThePage(http, AsLoaded(SecondPort: 5300));

            var kept        = Held(Station)["servers"]?[0];

            await SaveOnThePage(http, AsLoaded(SecondPort: 5300, FirstHeldToItsRoot: false));

            var taken       = Held(Station)["servers"]?[0];

            Assert.Multiple(() => {

                Assert.That(set.Single().AttributeStatus,                              Is.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(kept?["certificateFingerprint"]?.Value<String>(),          Is.EqualTo(APin), "the CSMS's pin");
                Assert.That(kept?["rootFingerprint"]?.Value<String>(),                 Is.EqualTo(ARoot));
                Assert.That(DNSOnDisk(Station)["servers"]?[1]?.Value<Int32>("port"),   Is.EqualTo(5300));

                Assert.That(taken?["rootFingerprint"],                                 Is.Null,  "shown and not sent back");
                Assert.That(taken?["certificateFingerprint"]?.Value<String>(),         Is.EqualTo(APin));

            });

        }

        #endregion

        #region APinLearnedBetweenTheCSMSsReadAndItsWriteIsKept()

        /// <summary>
        /// A CSMS reads the first server held to no certificate, the server's
        /// certificate is learned at a handshake, and the CSMS changes the
        /// server's port: the learned pin goes with the change, as the page's
        /// save keeps it. A write of the pins that names the empty list the
        /// CSMS read is refused as ValueChanged.
        /// </summary>
        [Test]
        public async Task APinLearnedBetweenTheCSMSsReadAndItsWriteIsKept()
        {

            using var certificate = SelfSigned();
            using var http        = await SignedIn();

            await SaveOnThePage(http, """
                                      [ { "id": "dns-1", "address": "192.0.2.53", "port": 853, "transport": "TLS", "queryTimeoutSeconds": null,
                                          "trustOnFirstUse": "certificate", "pinsAsShown": { "rootFingerprint": "{{ARoot}}", "onMismatch": "record" } } ]
                                      """.Replace("{{ARoot}}", ARoot));

            TrustForNameServers(certificate);

            var seen     = Get(Station, "dns-1", "CertificatePins").AttributeValue;

            Judge(certificate);

            var learned  = Get(Station, "dns-1", "CertificatePins").AttributeValue;

            var moved    = await Set(Station, null, Server("dns-1", "Port", "8853"));
            var unseen   = await Set(Station, null, Server("dns-1", "CertificatePins", "", Old: seen));

            var server   = DNSOnDisk(Station)["servers"]?[0];

            Assert.Multiple(() => {

                Assert.That(seen,                                                      Is.EqualTo(""));
                Assert.That(learned,                                                   Is.EqualTo(Fingerprint(certificate)).IgnoreCase);

                Assert.That(moved.Single().AttributeStatus,                            Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(server?.Value<Int32>("port"),                              Is.EqualTo(8853));
                Assert.That(server?["certificateFingerprint"]?.Value<String>(),        Is.EqualTo(Fingerprint(certificate)).IgnoreCase);

                Assert.That(unseen.Single().AttributeStatus,                           Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(unseen.Single().AttributeStatusInfo?.ReasonCode,           Is.EqualTo("ValueChanged"));

            });

        }

        #endregion

        #region ANameServerTheCSMSWouldNotResolveThroughIsRefusedByEitherVersion()

        /// <summary>
        /// The CSMS is dialled by a name that resolves through a name server
        /// over TLS. Moved to a name server whose certificate nobody vouches
        /// for, through SetVariables and through dns.ports, the node would
        /// lose its CSMS: both are refused, as CSMSUnreachable where OCPP 2.1
        /// can say so, and nothing is changed. Once the CSMS installed the
        /// second server's root as a NameServerRootCertificate, the same
        /// request is accepted (Secure DNS T20).
        /// </summary>
        [Test]
        public async Task ANameServerTheCSMSWouldNotResolveThroughIsRefusedByEitherVersion()
        {

            using var trustedCertificate    = ServerCertificate("A trusted name server");
            using var untrustedCertificate  = ServerCertificate("An untrusted name server");

            var trusted    = NameServer(trustedCertificate);
            var untrusted  = NameServer(untrustedCertificate);

            await trusted.  Start();
            await untrusted.Start();

            try
            {

                var station  = Twin();
                var port     = untrusted.ActiveTLSUnicastSocket!.Value.Port.ToUInt16().ToString();

                Assert.That(station.Node.TryUpdateDNSConfiguration(JObject.Parse($$"""
                                { "servers": [ { "address": "127.0.0.1", "port": {{trusted.ActiveTLSUnicastSocket!.Value.Port.ToUInt16()}}, "transport": "TLS" } ],
                                  "maxRetries": 0 }
                                """), out var error),
                            Is.True, error);

                Assert.That(station.Node.Certificates.Import(Encoding.ASCII.GetBytes(trustedCertificate.ExportCertificatePem()), CertificateKind.TLSRoot, null, null,
                                                        [ CertificateUsages.DNS ], out _, out error),
                            Is.True, error);

                Assert.That(station.TryDial($"ws://{CSMS}:9/cs", "2.1", out error),
                            Is.True, error);

                var before     = station.Node.DNSConfigurationJSON().ToString();

                var refused21  = await Set(station, OCPP21.DataConsistencyModel.ACID, Server("dns-1", "Port", port));
                var refused16  = await KeysOf(station).ChangeAsync("dns.ports", port, "CS-16");
                var after      = station.Node.DNSConfigurationJSON().ToString();

                var installed  = await station.OCPPv2_1Node.CertificateSources.InstallAsync(DNSRootCertificates.CertificateType, null,
                                                                                            OCPPC.Certificate.Parse(untrustedCertificate.ExportCertificatePem()), "CSMS");

                var accepted   = await Set(station, OCPP21.DataConsistencyModel.ACID, Server("dns-1", "Port", port));

                Assert.Multiple(() => {

                    Assert.That(refused21.Single().AttributeStatus,                    Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                    Assert.That(refused21.Single().AttributeStatusInfo?.ReasonCode,    Is.EqualTo("CSMSUnreachable"));
                    Assert.That(refused21.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain($"'{CSMS}'"));
                    Assert.That(refused16,                                             Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                    Assert.That(after,                                                 Is.EqualTo(before), "nothing was changed");

                    Assert.That(installed.Status,                                      Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                    Assert.That(accepted.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Accepted), accepted.Single().AttributeStatusInfo?.AdditionalInfo);
                    Assert.That(station.Node.DNSConfigurationJSON()["servers"]?[0]?.Value<String>("port"),  Is.EqualTo(port));

                });

            }
            finally
            {
                await trusted.  Stop();
                await untrusted.Stop();
            }

        }

        #endregion

        #region WhatTheFileCannotTakeIsChangedNowhere()

        /// <summary>
        /// Where the file cannot be written, the page is answered 500, OCPP 2.1
        /// InternalError and OCPP 1.6 Rejected - and none of them changed
        /// anything in effect (Secure DNS T17).
        /// </summary>
        [Test]
        public async Task WhatTheFileCannotTakeIsChangedNowhere()
        {

            using var http  = await SignedIn();

            var before      = Held(Station).ToString();

            // Where the node writes the file first, before it moves it over
            // the old one: a directory there, and the file cannot be written
            // on any system the node runs on.
            System.IO.Directory.CreateDirectory(Station.Node.ConfigFile.Path + ".tmp");

            var page        = await SaveOnThePage(http, AsLoaded(), settings => settings["maxRetries"] = 4);
            var set         = await Set(Station, OCPP21.DataConsistencyModel.ACID,
                                        Client("MaxRetries", "4"),
                                        Server("dns-2", "Port",  "5300"));
            var changed     = await KeysOf(Station).ChangeAsync("dns.maxRetries", "4", "CS-16");

            Assert.Multiple(() => {
                Assert.That(page,                                                      Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(set.Select(result => result.AttributeStatus),              Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(set.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InternalError"));
                Assert.That(changed,                                                   Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(Held(Station).ToString(),                                  Is.EqualTo(before));
            });

        }

        #endregion


        #region (private static) AsLoaded(SecondPort, FirstHeldToItsRoot)

        /// <summary>
        /// The servers of the configuration as the page sends them back, each
        /// entry with what the page showed it held to: unchanged, but for the
        /// port of the second, and the first's root, where it is taken away.
        /// </summary>
        private static String AsLoaded(Int32    SecondPort          = 53,
                                       Boolean  FirstHeldToItsRoot  = true)
        {

            var itsRoot = FirstHeldToItsRoot
                              ? $$""" "rootFingerprint": "{{ARoot}}", "onMismatch": "record", """
                              : "";

            return $$"""
                   [ { "id": "dns-1", "address": "192.0.2.53",    "port": 853,  "transport": "TLS", "queryTimeoutSeconds": null, {{itsRoot}}
                       "pinsAsShown": { "rootFingerprint": "{{ARoot}}", "onMismatch": "record" } },
                     { "id": "dns-2", "address": "2001:db8::53",  "port": {{SecondPort}}, "transport": "UDP", "queryTimeoutSeconds": null, "pinsAsShown": { } },
                     { "id": "dns-3", "address": "198.51.100.53", "port": 5353, "transport": "TCP", "queryTimeoutSeconds": 3,    "pinsAsShown": { } } ]
                   """;

        }

        #endregion

        #region (private) Twin() / SaveOnThePage(Http, Servers, Settings)

        /// <summary>
        /// A node built from the same configuration, never started.
        /// </summary>
        private IBridgedNode Twin()
        {

            var directory = TestNodes.TemporaryDirectory("dns-equivalence-twin");
            var station   = Nodes.New(directory, Configuration);

            twins.Add((station, directory));

            return station;

        }

        /// <summary>
        /// The name resolution saved as the page saves it: the given servers,
        /// and every setting of its form - as the node said them, but for what
        /// the given action changed.
        /// </summary>
        private static async Task<HttpStatusCode> SaveOnThePage(HttpClient        Http,
                                                                String            Servers,
                                                                Action<JObject>?  Settings = null)
        {

            var shown    = JObject.Parse(await Http.GetStringAsync(DNSPath))["settings"] as JObject ?? [];
            var settings = new JObject();

            foreach (var name in new[] { "encryptedOnly", "useCache", "dnssecOK", "followCNAMEs", "recursionDesired", "queryTimeoutSeconds", "maxCNAMEFollows", "maxRetries" })
                settings[name] = shown[name]?.DeepClone() ?? JValue.CreateNull();

            Settings?.Invoke(settings);

            settings.AddFirst(new JProperty("servers", JArray.Parse(Servers)));

            return (await Http.PutAsync(DNSPath, new StringContent(settings.ToString(), Encoding.UTF8, "application/json"))).StatusCode;

        }

        #endregion

        #region (private static) Set(Station, Model, Entries) / Get(Station, Server, Variable) / KeysOf(Station)

        private sealed record Entry(String? Server, String Variable, String Value, String? Old);

        private static Entry Client(String Variable, String Value, String? Old = null)
            => new (null, Variable, Value, Old);

        private static Entry Server(String Id, String Variable, String Value, String? Old = null)
            => new (Id, Variable, Value, Old);

        /// <summary>
        /// SetVariables of the DNS controllers as the node's OCPP 2.1 node
        /// answers it.
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
                                                                                            Component:          new OCPP21.Component(entry.Server is null ? DNSDeviceModel.ClientComponent : DNSDeviceModel.ServerComponent,
                                                                                                                                     entry.Server),
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
                                                    String?          Server,
                                                    String           Variable)
        {

            var node = Station.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component(Server is null ? DNSDeviceModel.ClientComponent : DNSDeviceModel.ServerComponent, Server),
                                                                                new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single();

        }

        /// <summary>
        /// The configuration keys of the name resolution on the node's OCPP
        /// 1.6 node.
        /// </summary>
        private static DNSDeviceModel KeysOf(IBridgedNode Station)

            => Station.OCPPv1_6Node.ConfigurationKeySources.OfType<DNSDeviceModel>().Single();

        #endregion

        #region (private) DNSOnDisk(Station) / Held(Station) / AsTheNextStartReadsIt(Station) / ChangeLine(Station, Since)

        /// <summary>
        /// The "dns" section as the node's file says it now.
        /// </summary>
        private static JObject DNSOnDisk(IBridgedNode Station)

            => JObject.Parse(File.ReadAllText(Station.Node.ConfigFile.Path))["dns"] as JObject ?? [];

        /// <summary>
        /// The name resolution as the node holds it, as the page reads it -
        /// but for where its file is, which is each node's own.
        /// </summary>
        private static JObject Held(IBridgedNode Station)
        {

            var held = Station.Node.DNSConfigurationJSON();

            held.Remove("file");

            return held;

        }

        /// <summary>
        /// The name resolution of a node built from the given node's
        /// file: what the file says, as the next start reads it.
        /// </summary>
        private JObject AsTheNextStartReadsIt(IBridgedNode Station)
        {

            var directory = TestNodes.TemporaryDirectory("dns-equivalence-start");
            var started   = Nodes.New(directory, JObject.Parse(File.ReadAllText(Station.Node.ConfigFile.Path)));

            twins.Add((started, directory));

            return Held(started);

        }

        /// <summary>
        /// The one line that says what a change of the name resolution changed.
        /// </summary>
        private static String ChangeLine(IBridgedNode  Station,
                                         UInt64           Since)

            => Station.Node.Log.Recent(100, Since, null).
                           Select(entry => entry.Message).
                           Single(message => message.StartsWith("DNS configuration changed: ", StringComparison.Ordinal));

        #endregion

        #region (private) TrustForNameServers(Root) / Judge(Certificate)

        /// <summary>
        /// The given certificate as a root of the node's store for name
        /// servers.
        /// </summary>
        private void TrustForNameServers(X509Certificate2 Root)
        {
            Assert.That(Station.Node.Certificates.Import(Encoding.ASCII.GetBytes(Root.ExportCertificatePem()), CertificateKind.TLSRoot, null, null,
                                                    [ CertificateUsages.DNS ], out _, out var error),
                        Is.True,
                        error);
        }

        /// <summary>
        /// The first name server showed the given certificate, which the
        /// machine trusts no root of.
        /// </summary>
        private void Judge(X509Certificate2 Certificate)
        {

            using var chain = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.Build(Certificate);

            Station.Node.JudgeNameServer(Station.Node.DNSClient.DNSServers.First(), Certificate, chain, SslPolicyErrors.RemoteCertificateChainErrors);

        }

        #endregion

        #region (private static) NameServer(Certificate) / ServerCertificate(Name) / SelfSigned() / Fingerprint(Certificate)

        /// <summary>
        /// A name server on the loopback, over TLS and nothing else, that knows
        /// the CSMS's name.
        /// </summary>
        private static DNSServer NameServer(X509Certificate2 Certificate)

            => new (
                   new AuthoritativeDNSRequestHandler(
                       new InMemoryDNSZone().
                           Add(new A(
                                   DomainName. Parse(CSMS + "."),
                                   DNSQueryClasses.IN,
                                   TimeSpan.   FromMinutes(5),
                                   IPv4Address.Parse("127.0.0.42")
                               ))
                   ),
                   new DNSServerOptions {
                       EnableUDPUnicast      = false,
                       EnableUDPMulticast    = false,
                       EnableTCPUnicast      = false,
                       EnableTLSUnicast      = true,
                       TLSUnicastSocket      = new IPSocket(IPv4Address.Localhost, IPPort.Parse(0)),
                       TLSServerCertificate  = Certificate
                   }
               );

        /// <summary>
        /// A certificate for the loopback that signed itself, with its key: a
        /// name server's own, and its root.
        /// </summary>
        /// <remarks>
        /// Each with a name of its own. Two that signed themselves under the same
        /// name, without key identifiers, are one issuer to the chain on Linux:
        /// it took the first root of that name in the store, whose signature does
        /// not fit, and the second server stayed untrusted with its root
        /// installed - red in the Debian job only.
        /// </remarks>
        private static X509Certificate2 ServerCertificate(String Name)
        {

            using var rsa  = RSA.Create(2048);

            var request    = new CertificateRequest($"CN={Name}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var names      = new SubjectAlternativeNameBuilder();

            names.AddDnsName  ("localhost");
            names.AddIpAddress(System.Net.IPAddress.Loopback);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension([ new Oid("1.3.6.1.5.5.7.3.1") ], false));
            request.CertificateExtensions.Add(names.Build());

            using var created = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddDays(1));

            return X509CertificateLoader.LoadPkcs12(created.Export(X509ContentType.Pfx), null, X509KeyStorageFlags.Exportable);

        }

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
