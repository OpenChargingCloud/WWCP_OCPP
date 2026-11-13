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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Server;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.TestKit;

using cloud.charging.open.protocols.OCPP.Node.TestKit;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// Chapter 7 of the white paper "DNS over SSH", from the CSMS to the name:
    /// it reads the key the station signs in with at name servers, has it
    /// entered at the resolver, gives the station that resolver over SSH with
    /// the host key it is pinned to in one SetVariables request - and the next
    /// name is resolved over SSH, what the attempt came to and the host key the
    /// server showed readable as variables. And what goes wrong is told as the
    /// security events of section 7.4, once per change of the verdict (T5, T6,
    /// T9 of chapter 11).
    /// </summary>
    /// <remarks>
    /// Against an SSH server on the loopback interface that hands the
    /// subsystem "dns@open.charging.cloud" to a DNS server without a listener
    /// of its own, addressed by its address: no name to resolve before the
    /// first one. The node is a bare one with its bridges, and a connection of
    /// OCPP 2.1 that is configured and never dialled: what it tells its CSMS
    /// waits in the queue.
    /// </remarks>
    [TestFixture]
    [NonParallelizable]
    public class DNSOverSSHEndToEndTests
    {

        #region Data

        private static readonly ISshHostKey  hostKey1  = SshHostKey.GenerateEd25519();
        private static readonly ISshHostKey  hostKey2  = SshHostKey.GenerateEd25519();
        private static readonly String       print1    = SshFingerprint.Sha256(hostKey1.PublicKeyBlob);
        private static readonly String       print2    = SshFingerprint.Sha256(hostKey2.PublicKeyBlob);
        private static readonly String       notShown  = "SHA256:" + new String('A', 43);

        private String          directory  = "";
        private IBridgedNode?   station;
        private DNSServer?      dns;

        private readonly List<SshServer>            sshServers  = [];
        private readonly ConcurrentQueue<DNSPacket> requests    = new ();

        /// <summary>
        /// The keys the name server lets in, under any login.
        /// </summary>
        private readonly List<Byte[]>               letIn       = [];

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void ANodeWithANameServerThatIsNotThere()
        {

            directory  = TestNodes.TemporaryDirectory("dns-ssh-end-to-end");

            var configuration = TestNodes.Offline;

            // One name server, over UDP, at an address of the documentation
            // range: until the CSMS says otherwise, nothing resolves.
            configuration["dns"] = JObject.Parse("""
                                       { "servers": [ { "id": "plain", "address": "192.0.2.1", "queryTimeoutSeconds": 0.2 } ], "queryTimeoutSeconds": 5, "maxRetries": 0 }
                                       """);

            station    = new BareNodes().New(directory, configuration);

            Assert.That(station.TryDial("ws://127.0.0.1:9/cs", "2.1", out var error), Is.True, error);

            dns        = new DNSServer(
                             new AuthoritativeDNSRequestHandler(
                                 new InMemoryDNSZone().
                                     AddZoneFileString("example.test. 300 IN SOA ns.example.test. hostmaster.example.test. 1 3600 600 86400 300", Origin: DomainName.Parse("example.test.")).
                                     AddZoneFileString("one.example.test.   300 IN A 192.0.2.101",                                                Origin: DomainName.Parse("example.test.")).
                                     AddZoneFileString("two.example.test.   300 IN A 192.0.2.102",                                                Origin: DomainName.Parse("example.test.")).
                                     AddZoneFileString("three.example.test. 300 IN A 192.0.2.103",                                                Origin: DomainName.Parse("example.test."))
                             ),
                             new DNSServerOptions {
                                 EnableUDPUnicast    = false,
                                 EnableUDPMulticast  = false,
                                 EnableTCPUnicast    = false
                             }
                         );

            dns.OnDNSRequestReceived += (timestamp, server, serverType, request, ct) => {
                requests.Enqueue(request);
                return Task.CompletedTask;
            };

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            foreach (var ssh in sshServers)
                await ssh.DisposeAsync();

            sshServers.Clear();
            requests.  Clear();
            letIn.     Clear();

            if (dns is not null)
                await dns.Stop();

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            TestNodes.Remove(directory);

        }

        #endregion

        #region (helpers) NameServer(HostKey, Port = null) / Set(Model, Entries) / Value(Server, Variable) / Ask(Name) / Told()

        /// <summary>
        /// The resolver behind an SSH server that shows the given host key, and
        /// lets in whoever holds one of the keys entered there so far.
        /// </summary>
        private async Task<(SshServer SSH, UInt16 Port, CollectingAuditSink Audit)> NameServer(ISshHostKey  HostKey,
                                                                                                UInt16?      Port   = null)
        {

            var audit = new CollectingAuditSink();

            var ssh   = new SshServer(new SshServerOptions {
                            HostKeys       = [ HostKey ],
                            Authenticator  = new SshUserAuthenticator((request, _) => {
                                                 lock (letIn)
                                                     return ValueTask.FromResult(letIn.Any(blob => blob.AsSpan().SequenceEqual(request.PublicKeyBlob)));
                                             }),
                            AuditSink      = audit,
                            Subsystems     = (session, name, ct) => ValueTask.FromResult<SshSubsystemHandler?>(
                                                 name == DNSServer.SSHSubsystemName
                                                     ? async (context, token) => {
                                                           try   { await dns!.ServeSSHSubsystemAsync(context, token); }
                                                           catch (OperationCanceledException) { }
                                                       }
                                                     : null
                                             )
                        });

            await ssh.StartAsync(new IPSocket(IPv4Address.Localhost, Port.HasValue ? IPPort.Parse(Port.Value) : IPPort.Auto));

            sshServers.Add(ssh);

            return (ssh, (UInt16) ssh.LocalEndPoint.Port.ToInt32(), audit);

        }

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
        /// What a CSMS reads of a variable of a name server through GetVariables.
        /// </summary>
        private String? Value(String  Server,
                              String  Variable)
        {

            var node = station!.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component(DNSDeviceModel.ServerComponent, Server), new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single().AttributeValue;

        }

        /// <summary>
        /// The addresses the node resolves a name to: none where nobody answered.
        /// </summary>
        private async Task<String[]> Ask(String Name)
        {

            var answer = await station!.Node.ResolveAsync(Name, [ DNSResourceRecordTypes.A ]);

            return [.. (answer["answers"] as JArray ?? []).Select(record => record["value"]!.Value<String>()!)];

        }

        /// <summary>
        /// The security events waiting for the CSMS, with their techInfo.
        /// </summary>
        private (String Type, String? TechInfo)[] Told()

            => [.. station!.OCPPv2_1Node.OfflineQueue.Pending.Where (message => message.Kind == "SecurityEventNotification").
                                                              Select(message => (message.Payload.Value<String>("type")!, message.Payload.Value<String>("techInfo")))];

        private static String Said(IEnumerable<OCPP21.SetVariableResult> Results)

            => String.Join(" | ", Results.Select(result => $"{result.Component.Instance ?? "client"}.{result.Variable.Name}: {result.AttributeStatus} {result.AttributeStatusInfo?.ReasonCode} {result.AttributeStatusInfo?.AdditionalInfo}"));

        #endregion


        #region ACSMSGivesTheStationANameServerOverSSH()

        /// <summary>
        /// The example of section 7.6, as far as a resolver with a connection
        /// of its own goes: the CSMS adds the server over SSH, pinned, with
        /// ACID; the station shows its DNS key, which the CSMS enrolls at the
        /// resolver; and the next name is resolved there, signed in under the
        /// node's own name - Accepted, with the host key it showed, and since
        /// when it is believed with it. Nothing of it is told as a security
        /// event the CSMS did not cause.
        /// </summary>
        [Test]
        public async Task ACSMSGivesTheStationANameServerOverSSH()
        {

            var (_, port, audit) = await NameServer(hostKey1);

            var added    = await Set(OCPP21.DataConsistencyModel.ACID,
                                     (null,    "Servers",         "cpo-1"),
                                     ("cpo-1", "Address",         "127.0.0.1"),
                                     ("cpo-1", "Port",            port.ToString()),
                                     ("cpo-1", "Transport",       "SSH"),
                                     ("cpo-1", "SSHHostKeyPins",  $"{print1},{print2}"),
                                     (null,    "EncryptedOnly",   "true"));

            // Not enrolled yet: the station's key is not taken.
            var key      = Value("cpo-1", "SSHClientPublicKey");
            var early    = await Ask("one.example.test");
            var refused  = Value("cpo-1", "SSHStatus");
            var problem  = Value("cpo-1", "Problem");

            // The CSMS enrolls it at the resolver.
            lock (letIn)
                letIn.Add(SshPublicKey.Parse(key!).Blob);

            var answer   = await Ask("two.example.test");
            var told     = Told();

            Assert.Multiple(() => {

                Assert.That(added.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(added));
                Assert.That(key,                                             Does.StartWith("ssh-ed25519 "));
                Assert.That(key,                                             Is.EqualTo(station!.Node.DNSSSHClientPublicKey));

                Assert.That(early,                                           Is.Empty);
                Assert.That(refused,                                         Is.EqualTo("AuthenticationFailed"));
                Assert.That(problem,                                         Is.EqualTo("true"));

                Assert.That(answer,                                          Is.EqualTo(new[] { "192.0.2.102" }));
                Assert.That(requests,                                        Has.Count.EqualTo(1), "the resolver was not asked over SSH");
                Assert.That(audit.Events.OfType<AuthenticationSucceededEvent>().Select(login => login.Username),
                                                                             Is.EqualTo(new[] { station.Node.Kind.Tag }), "the login is the node's own name");

                Assert.That(Value("cpo-1", "SSHStatus"),                     Is.EqualTo("Accepted"));
                Assert.That(Value("cpo-1", "Problem"),                       Is.EqualTo("false"));
                Assert.That(Value("cpo-1", "SSHHostKeyFingerprint"),         Is.EqualTo(print1));
                Assert.That(Value("cpo-1", "KnownSince"),                    Is.Not.Empty);
                Assert.That(Value("cpo-1", "CertificateStatus"),             Is.EqualTo(""));
                Assert.That(Value("cpo-1", "Port"),                          Is.EqualTo(port.ToString()));

                // The key that was not taken is told, once; the change itself is the CSMS's own.
                Assert.That(told.Select(one => one.Type),                    Is.EqualTo(new[] { "NameServerAuthenticationFailed" }));
                Assert.That(told[0].TechInfo,                                Does.StartWith($"ssh://127.0.0.1:{port}: it did not take the key"));

            });

        }

        #endregion

        #region AHostKeyThatIsNotPinnedIsToldOnce_AndAChangedOneAsChanged()

        /// <summary>
        /// T5 and T6, as the CSMS hears of them: a host key that is not pinned
        /// is not used, a Problem, and InvalidNameServerHostKey - once, however
        /// often a name is asked for. Pinned by the CSMS, the server is used;
        /// and when it changes to the second key of its pin set, it is used
        /// still, and told as NameServerHostKeyChanged with both keys.
        /// </summary>
        [Test]
        public async Task AHostKeyThatIsNotPinnedIsToldOnce_AndAChangedOneAsChanged()
        {

            lock (letIn)
                letIn.Add(SshPublicKey.Parse(station!.Node.DNSSSHClientPublicKey!).Blob);

            var (first, port, audit) = await NameServer(hostKey1);

            var added    = await Set(OCPP21.DataConsistencyModel.ACID,
                                     (null,    "Servers",         "cpo-1"),
                                     ("cpo-1", "Address",         "127.0.0.1"),
                                     ("cpo-1", "Port",            port.ToString()),
                                     ("cpo-1", "Transport",       "SSH"),
                                     ("cpo-1", "SSHHostKeyPins",  notShown));

            var notUsed1 = await Ask("one.example.test");
            var notUsed2 = await Ask("two.example.test");
            var status   = Value("cpo-1", "SSHStatus");
            var problem  = Value("cpo-1", "Problem");
            var shown    = Value("cpo-1", "SSHHostKeyFingerprint");
            var told1    = Told();
            var reached  = audit.Events.OfType<ConnectionOpenedEvent>().Count();

            // The CSMS pins the key it showed, and the next one.
            var pinned   = await Set(null, ("cpo-1", "SSHHostKeyPins", $"{print1},{print2}"));
            var used     = await Ask("one.example.test");
            var since1   = Value("cpo-1", "KnownSince");

            // The server changes to its next key.
            sshServers.Remove(first);
            await first.DisposeAsync();
            await Task.Delay(50);
            await NameServer(hostKey2, port);

            var still    = await Ask("three.example.test");
            var told2    = Told();

            Assert.Multiple(() => {

                Assert.That(added.Select(result => result.AttributeStatus),  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(added));

                Assert.That(notUsed1,                                        Is.Empty);
                Assert.That(notUsed2,                                        Is.Empty);
                Assert.That(reached,                                         Is.EqualTo(2), "the server was not reached once per question");
                Assert.That(status,                                          Is.EqualTo("PinMismatch"));
                Assert.That(problem,                                         Is.EqualTo("true"));
                Assert.That(shown,                                           Is.EqualTo(print1));
                Assert.That(told1.Select(one => one.Type),                   Is.EqualTo(new[] { "InvalidNameServerHostKey" }), "once per change of the verdict");
                Assert.That(told1[0].TechInfo,                               Does.StartWith($"ssh://127.0.0.1:{port}: PinMismatch, host key {print1}, refused"));

                Assert.That(pinned.Single().AttributeStatus,                 Is.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(pinned));
                Assert.That(used,                                            Is.EqualTo(new[] { "192.0.2.101" }));
                Assert.That(since1,                                          Is.Not.Empty);

                Assert.That(still,                                           Is.EqualTo(new[] { "192.0.2.103" }), "the second key of its pin set was not used");
                Assert.That(Value("cpo-1", "SSHStatus"),                     Is.EqualTo("Accepted"));
                Assert.That(Value("cpo-1", "SSHHostKeyFingerprint"),         Is.EqualTo(print2));
                Assert.That(told2.Select(one => one.Type),                   Is.EqualTo(new[] { "InvalidNameServerHostKey", "NameServerHostKeyChanged" }));
                Assert.That(told2[1].TechInfo,                               Is.EqualTo($"ssh://127.0.0.1:{port}: host key {print1} -> {print2}"));
                Assert.That(requests,                                        Has.Count.EqualTo(2));

            });

        }

        #endregion

    }

}
