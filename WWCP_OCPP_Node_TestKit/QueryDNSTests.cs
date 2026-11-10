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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// QueryDNS of the white paper "Advanced Diagnostics for OCPP", answered by
    /// a node with its own name resolution: Accepted with what the name server
    /// said, and Rejected with the reason codes of the white paper where
    /// nothing was asked.
    /// </summary>
    /// <remarks>
    /// The name server is Hermod's, authoritative for example.test, on TCP at
    /// the loopback address: nothing here leaves this machine. A second name
    /// server is configured by its name, which a node cannot ask on its own.
    ///
    /// Asked in OCPP 2.1, with a name server named by its id, and in OCPP 1.6,
    /// with a name server named by its place in dns.servers, counted from 1:
    /// the answer is the node's either way.
    /// </remarks>
    public abstract class QueryDNSTests : ABridgeTests
    {

        #region Data

        private String         directory   = "";
        private DNSServer      nameServer  = default!;
        private UInt16         port;
        private IBridgedNode?  station;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task AStationWithANameServerOfItsOwn()
        {

            var origin  = DomainName.Parse("example.test.");

            nameServer  = new DNSServer(
                              new AuthoritativeDNSRequestHandler(
                                  new InMemoryDNSZone().
                                      AddZoneFileString("example.test. 300 IN SOA ns.example.test. hostmaster.example.test. 1 3600 600 86400 300", Origin: origin).
                                      AddZoneFileString("csms.example.test. 300 IN A 127.0.0.42",                                                    Origin: origin)
                              ),
                              new DNSServerOptions {
                                  EnableUDPUnicast    = false,
                                  EnableUDPMulticast  = false,
                                  EnableTCPUnicast    = true,
                                  TCPUnicastSocket    = new IPSocket(IPv4Address.Localhost, IPPort.Parse(0))
                              }
                          );

            await nameServer.Start();

            port               = nameServer.ActiveTCPUnicastSocket!.Value.Port.ToUInt16();
            directory          = TestNodes.TemporaryDirectory("query-dns");

            var configuration  = TestNodes.Offline;

            configuration["dns"] = JObject.Parse($$"""
                                       { "servers": [ { "id": "local",  "address": "127.0.0.1",        "port": {{port}}, "transport": "TCP", "queryTimeoutSeconds": 3 },
                                                      { "id": "byname", "address": "resolver.example", "port": {{port}}, "transport": "TCP", "queryTimeoutSeconds": 3 } ],
                                         "maxRetries": 0,
                                         "useCache": false }
                                       """);

            station            = Nodes.New(directory, configuration);

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            await nameServer.Stop();

            TestNodes.Remove(directory);

        }

        #endregion

        #region (helper) Ask(Name, RecordTypes = null, ServerId = null)

        private Task<OCPPv2_1.CS.QueryDNSResponse> Ask(String                Name,
                                                       IEnumerable<String>?  RecordTypes   = null,
                                                       String?               ServerId      = null)

            => station!.Bridge.QueryDNS(
                   new OCPPv2_1.CSMS.QueryDNSRequest(
                       SourceRouting.To(station.OCPPv2_1Node.Id),
                       Name,
                       RecordTypes,
                       ServerId
                   )
               );

        #endregion

        #region (helper) Ask16(Name, RecordTypes = null, Server = null)

        private Task<OCPPv1_6.CP.QueryDNSResponse> Ask16(String                Name,
                                                         IEnumerable<String>?  RecordTypes   = null,
                                                         UInt32?               Server        = null)

            => station!.Bridge.QueryDNS(
                   new OCPPv1_6.CS.QueryDNSRequest(
                       SourceRouting.To(((OCPPv1_6.CS.IChargePointNode) station.OCPPv1_6Node).Id),
                       Name,
                       RecordTypes,
                       Server
                   )
               );

        #endregion


        #region TheStationAnswersQueryDNSWithItsNameResolution()

        /// <summary>
        /// The OCPP node of the station is given the bridge to answer QueryDNS
        /// with, and not left to say it has nothing to ask.
        /// </summary>
        [Test]
        public void TheStationAnswersQueryDNSWithItsNameResolution()
        {

            Assert.That(station!.OCPPv2_1Node,                                          Is.InstanceOf<OCPPv2_1.IQueryDNSNode>());
            Assert.That(((OCPPv2_1.IQueryDNSNode) station.OCPPv2_1Node).QueryDNSProcessor,  Is.Not.Null);

        }

        #endregion

        #region OneServerAloneIsAccepted()

        /// <summary>
        /// One server, by the id of its DNSServerController: Accepted, with
        /// where the answer came from, its code in the registry's words and
        /// the record.
        /// </summary>
        [Test]
        public async Task OneServerAloneIsAccepted()
        {

            var response = await Ask("csms.example.test", [ "A" ], "LOCAL");

            Assert.Multiple(() => {
                Assert.That(response.Status,         Is.EqualTo(OCPPv2_1.GenericStatus.Accepted));
                Assert.That(response.StatusInfo,     Is.Null);
                Assert.That(response.QueriedName,    Is.EqualTo("csms.example.test."));
                Assert.That(response.RecordTypes,    Is.EqualTo(new[] { "A" }));
                Assert.That(response.ServerId,       Is.EqualTo("local"));
                Assert.That(response.AnsweredBy,     Is.EqualTo($"tcp://127.0.0.1:{port}"));
                Assert.That(response.ResponseCode,   Is.EqualTo("NoError"));
                Assert.That(response.Authoritative,  Is.True);
                Assert.That(response.QueryRuntime,   Is.Not.Null);
                Assert.That(response.Answers,        Is.EqualTo(new[] { new OCPPv2_1.DNSAnswer("csms.example.test.", "A", TimeSpan.FromSeconds(300), "127.0.0.42") }));
                Assert.That(response.Error,          Is.Null);
            });

        }

        #endregion

        #region ANameThatDoesNotExistIsAnAnswer()

        /// <summary>
        /// NXDomain is what the server answered: Accepted, not Rejected.
        /// </summary>
        [Test]
        public async Task ANameThatDoesNotExistIsAnAnswer()
        {

            var response = await Ask("missing.example.test", ServerId: "local");

            Assert.Multiple(() => {
                Assert.That(response.Status,        Is.EqualTo(OCPPv2_1.GenericStatus.Accepted));
                Assert.That(response.ResponseCode,  Is.EqualTo("NXDomain"));
                Assert.That(response.Answers,       Is.Empty);
            });

        }

        #endregion

        #region AnAddressIsAskedForItsReverseName()

        /// <summary>
        /// An address is asked for as its reverse name, for PTR, and the
        /// response keeps the address it was made of.
        /// </summary>
        [Test]
        public async Task AnAddressIsAskedForItsReverseName()
        {

            var response = await Ask("127.0.0.42", ServerId: "local");

            Assert.Multiple(() => {
                Assert.That(response.Status,           Is.EqualTo(OCPPv2_1.GenericStatus.Accepted));
                Assert.That(response.QueriedName,      Is.EqualTo("42.0.0.127.in-addr.arpa."));
                Assert.That(response.ReverseLookupOf,  Is.EqualTo("127.0.0.42"));
                Assert.That(response.RecordTypes,      Is.EqualTo(new[] { "PTR" }));
            });

        }

        #endregion

        #region WhereNothingIsAskedTheReasonCodeSaysWhy()

        /// <summary>
        /// An id that is none of the station's servers, a server configured by
        /// its name, a word that is no record type and too many of them: each
        /// Rejected with its reason code, and the reason in words.
        /// </summary>
        [Test]
        public async Task WhereNothingIsAskedTheReasonCodeSaysWhy()
        {

            var unknown   = await Ask("csms.example.test", ServerId: "192.0.2.53");
            var byName    = await Ask("csms.example.test", ServerId: "byname");
            var bogus     = await Ask("csms.example.test", [ "A", "SRVX" ]);
            var tooMany   = await Ask("csms.example.test", [ "A", "AAAA", "MX", "NS", "TXT", "SRV", "SOA", "PTR", "CAA" ]);
            var invalid   = await Ask("csms..example.test", ServerId: "local");

            Assert.Multiple(() => {

                foreach (var (response, reasonCode) in new[] { (unknown,  DNSQueryRefusals.UnknownServer),
                                                               (byName,   DNSQueryRefusals.ServerByName),
                                                               (bogus,    DNSQueryRefusals.InvalidRecordType),
                                                               (tooMany,  DNSQueryRefusals.TooManyRecordTypes),
                                                               (invalid,  DNSQueryRefusals.InvalidName) })
                {
                    Assert.That(response.Status,                      Is.EqualTo(OCPPv2_1.GenericStatus.Rejected), reasonCode);
                    Assert.That(response.StatusInfo?.ReasonCode,      Is.EqualTo(reasonCode));
                    Assert.That(response.StatusInfo?.AdditionalInfo,  Is.EqualTo(response.Error));
                    Assert.That(response.Error,                       Is.Not.Null.And.Not.Empty, reasonCode);
                    Assert.That(response.ResponseCode,                Is.Null, reasonCode);
                }

                Assert.That(unknown.Error, Is.EqualTo($"'192.0.2.53' is none of this {station!.Node.Kind.Name}'s name servers, which are local, byname."));

            });

        }

        #endregion

        #region NothingIsAskedWithNameResolutionOff()

        /// <summary>
        /// With name resolution switched off: Rejected, Disabled.
        /// </summary>
        [Test]
        public async Task NothingIsAskedWithNameResolutionOff()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse("""{ "enabled": false }"""), out var error), Is.True, error);

            var response = await Ask("csms.example.test");

            Assert.Multiple(() => {
                Assert.That(response.Status,                  Is.EqualTo(OCPPv2_1.GenericStatus.Rejected));
                Assert.That(response.StatusInfo?.ReasonCode,  Is.EqualTo(DNSQueryRefusals.Disabled));
            });

        }

        #endregion


        #region TheChargePointAnswersQueryDNSWithItsNameResolution()

        /// <summary>
        /// The OCPP 1.6 node of the station is given the bridge to answer
        /// QueryDNS with as well, and not left to say it has nothing to ask.
        /// </summary>
        [Test]
        public void TheChargePointAnswersQueryDNSWithItsNameResolution()
        {

            Assert.That(station!.OCPPv1_6Node,                                              Is.InstanceOf<OCPPv1_6.IQueryDNSNode>());
            Assert.That(((OCPPv1_6.IQueryDNSNode) station.OCPPv1_6Node).QueryDNSProcessor,  Is.Not.Null);

        }

        #endregion

        #region OneServerAloneIsAcceptedIn16()

        /// <summary>
        /// One server, by its place in dns.servers: Accepted, with the place it
        /// was asked by, where the answer came from, its code in the registry's
        /// words and the record - and no reason code.
        /// </summary>
        [Test]
        public async Task OneServerAloneIsAcceptedIn16()
        {

            var response = await Ask16("csms.example.test", [ "A" ], 1);

            Assert.Multiple(() => {
                Assert.That(response.Status,         Is.EqualTo(OCPPv1_6.GenericStatus.Accepted));
                Assert.That(response.ReasonCode,     Is.Null);
                Assert.That(response.QueriedName,    Is.EqualTo("csms.example.test."));
                Assert.That(response.RecordTypes,    Is.EqualTo(new[] { "A" }));
                Assert.That(response.Server,         Is.EqualTo(1));
                Assert.That(response.AnsweredBy,     Is.EqualTo($"tcp://127.0.0.1:{port}"));
                Assert.That(response.ResponseCode,   Is.EqualTo("NoError"));
                Assert.That(response.Authoritative,  Is.True);
                Assert.That(response.QueryRuntime,   Is.Not.Null);
                Assert.That(response.Answers,        Is.EqualTo(new[] { new OCPPv1_6.DNSAnswer("csms.example.test.", "A", TimeSpan.FromSeconds(300), "127.0.0.42") }));
                Assert.That(response.Error,          Is.Null);
            });

        }

        #endregion

        #region ANameThatDoesNotExistIsAnAnswerIn16()

        /// <summary>
        /// NXDomain is what the server answered: Accepted, not Rejected - and an
        /// address is asked for as its reverse name, for PTR.
        /// </summary>
        [Test]
        public async Task ANameThatDoesNotExistIsAnAnswerIn16()
        {

            var missing  = await Ask16("missing.example.test", Server: 1);
            var address  = await Ask16("127.0.0.42",           Server: 1);

            Assert.Multiple(() => {

                Assert.That(missing.Status,           Is.EqualTo(OCPPv1_6.GenericStatus.Accepted));
                Assert.That(missing.ResponseCode,     Is.EqualTo("NXDomain"));
                Assert.That(missing.Answers,          Is.Empty);

                Assert.That(address.Status,           Is.EqualTo(OCPPv1_6.GenericStatus.Accepted));
                Assert.That(address.QueriedName,      Is.EqualTo("42.0.0.127.in-addr.arpa."));
                Assert.That(address.ReverseLookupOf,  Is.EqualTo("127.0.0.42"));
                Assert.That(address.RecordTypes,      Is.EqualTo(new[] { "PTR" }));

            });

        }

        #endregion

        #region WhereNothingIsAskedTheReasonCodeSaysWhyIn16()

        /// <summary>
        /// A place behind the station's last server, a server configured by its
        /// name, a word that is no record type and too many of them: each
        /// Rejected with its reason code in reasonCode, as OCPP 1.6 has no
        /// StatusInfo, and the reason in words.
        /// </summary>
        [Test]
        public async Task WhereNothingIsAskedTheReasonCodeSaysWhyIn16()
        {

            var unknown   = await Ask16("csms.example.test", Server: 3);
            var byName    = await Ask16("csms.example.test", Server: 2);
            var bogus     = await Ask16("csms.example.test", [ "A", "SRVX" ]);
            var tooMany   = await Ask16("csms.example.test", [ "A", "AAAA", "MX", "NS", "TXT", "SRV", "SOA", "PTR", "CAA" ]);
            var invalid   = await Ask16("csms..example.test", Server: 1);

            Assert.Multiple(() => {

                foreach (var (response, reasonCode) in new[] { (unknown,  DNSQueryRefusals.UnknownServer),
                                                               (byName,   DNSQueryRefusals.ServerByName),
                                                               (bogus,    DNSQueryRefusals.InvalidRecordType),
                                                               (tooMany,  DNSQueryRefusals.TooManyRecordTypes),
                                                               (invalid,  DNSQueryRefusals.InvalidName) })
                {
                    Assert.That(response.Status,                                   Is.EqualTo(OCPPv1_6.GenericStatus.Rejected), reasonCode);
                    Assert.That(response.ReasonCode,                               Is.EqualTo(reasonCode));
                    Assert.That(response.ToJSON()["reasonCode"]?.Value<String>(),  Is.EqualTo(reasonCode));
                    Assert.That(response.Error,                                    Is.Not.Null.And.Not.Empty, reasonCode);
                    Assert.That(response.ResponseCode,                             Is.Null, reasonCode);
                }

                Assert.That(unknown.Error, Is.EqualTo($"This {station!.Node.Kind.Name} has no name server number 3, only 2."));

            });

        }

        #endregion

        #region NothingIsAskedWithNameResolutionOffIn16()

        /// <summary>
        /// With name resolution switched off: Rejected, Disabled.
        /// </summary>
        [Test]
        public async Task NothingIsAskedWithNameResolutionOffIn16()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse("""{ "enabled": false }"""), out var error), Is.True, error);

            var response = await Ask16("csms.example.test");

            Assert.Multiple(() => {
                Assert.That(response.Status,      Is.EqualTo(OCPPv1_6.GenericStatus.Rejected));
                Assert.That(response.ReasonCode,  Is.EqualTo(DNSQueryRefusals.Disabled));
            });

        }

        #endregion

    }

}
