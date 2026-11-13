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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// A name server asked over SSH in the OCPP Device Model (white paper "DNS
    /// over SSH", chapter 7): the transport SSH and the SSH variables of
    /// DNSServerController as a CSMS reads them, what it may set of them and
    /// what it is refused (section 7.2; T14 to T17 of chapter 11), the
    /// configuration keys of OCPP 1.6 (section 7.5), and the security events
    /// (section 7.4).
    /// </summary>
    /// <remarks>
    /// The node has a name server over TLS, one over UDP and one over SSH held
    /// to a host key, and one SSH peer, "lc.example". It is built and never
    /// started, and no test here has an SSH server: what an attempt comes to
    /// is the node's to say, and tried against one where the node is - the
    /// status is read here from a node of words. Every refusal stands beside
    /// the same request, changed in the one thing that was refused, being
    /// taken.
    /// </remarks>
    public abstract class DNSOverSSHDeviceModelTests : ABridgeTests
    {

        #region Data

        private const String  ARoot  = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  APin   = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private static readonly String  PinA  = "SHA256:" + new String('A', 43);
        private static readonly String  PinB  = "SHA256:" + new String('B', 43);

        private String          directory  = "";
        private IBridgedNode?   station;
        private DNSDeviceModel  model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithANameServerOverSSHAndAnSSHPeer()
        {

            directory  = TestNodes.TemporaryDirectory("dns-ssh-device-model");

            var configuration = TestNodes.Offline;

            configuration["dns"] = JObject.Parse($$"""
                                       { "servers": [ { "address": "192.0.2.53",   "transport": "TLS", "rootFingerprint": "{{ARoot}}" },
                                                      { "address": "2001:db8::53", "transport": "UDP" },
                                                      { "id": "cpo-1", "address": "192.0.2.54", "transport": "SSH", "ssh": { "hostKeyPins": [ "{{PinA}}" ] } } ],
                                         "maxRetries": 2 }
                                       """);

            configuration["ssh"] = JObject.Parse($$"""
                                       { "peers": [ { "id": "lc", "address": "lc.example", "hostKeyPins": [ "{{PinB}}" ] } ] }
                                       """);

            station    = Nodes.New(directory, configuration);
            model      = new DNSDeviceModel(station.Node.DNSConfigurationJSON);

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


        #region AServerOverSSHIsReadAsTheNodeSaysIt()

        /// <summary>
        /// The transport SSH and its variables (section 7.1): the port of SSH,
        /// the pins, a connection of its own, the key the node signs in with -
        /// made at this first read - and nothing of a certificate. Before the
        /// first attempt there is no status, and no problem. A server that is
        /// not asked over SSH has the variables, empty.
        /// </summary>
        [Test]
        public void AServerOverSSHIsReadAsTheNodeSaysIt()
        {

            var controller = station!.OCPPv2_1Node.GetComponentConfigs(DNSDeviceModel.ServerComponent, null).
                                                   OfType<OCPP21.DNSServerController>().
                                                   Single(server => server.Server == "cpo-1");

            var variables  = controller.VariableConfigs.ToDictionary(variable => variable.Name);

            Assert.Multiple(() => {

                Assert.That(DNSDeviceModel.ServerVariables,                   Is.SupersetOf(new[] { "SSHUserName", "SSHHostKeyPins", "SSHHostCAs", "SSHConnection", "SSHClientPublicKey", "SSHHostKeyFingerprint", "SSHStatus" }));
                Assert.That(variables.Keys,                                   Is.SupersetOf(new[] { "SSHUserName", "SSHHostKeyPins", "SSHHostCAs", "SSHConnection", "SSHClientPublicKey", "SSHHostKeyFingerprint", "SSHStatus" }));
                Assert.That(variables["Transport"].Characteristics.ValuesList, Has.Member("SSH"));
                Assert.That(variables["SSHStatus"].Characteristics.ValuesList, Is.EqualTo(new[] { "Accepted", "PinMismatch", "Untrusted", "WeakAlgorithms", "AuthenticationFailed", "SubsystemRefused", "NoSSHPeer" }));

                Assert.That(model.ServerValue("cpo-1", "Address"),                Is.EqualTo("192.0.2.54"));
                Assert.That(model.ServerValue("cpo-1", "Transport"),              Is.EqualTo("SSH"));
                Assert.That(model.ServerValue("cpo-1", "Port"),                   Is.EqualTo("22"));
                Assert.That(model.ServerValue("cpo-1", "SSHUserName"),            Is.EqualTo(""),  "the station's own identity");
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),         Is.EqualTo(PinA));
                Assert.That(model.ServerValue("cpo-1", "SSHHostCAs"),             Is.EqualTo(""));
                Assert.That(model.ServerValue("cpo-1", "SSHConnection"),          Is.EqualTo("Dedicated"));
                Assert.That(model.ServerValue("cpo-1", "SSHClientPublicKey"),     Does.StartWith("ssh-ed25519 "));
                Assert.That(model.ServerValue("cpo-1", "SSHClientPublicKey"),     Is.EqualTo(station.Node.DNSSSHClientPublicKey));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyFingerprint"),  Is.EqualTo(""));
                Assert.That(model.ServerValue("cpo-1", "SSHStatus"),              Is.EqualTo(""),  "a verdict before the first attempt");
                Assert.That(model.ServerValue("cpo-1", "Problem"),                Is.EqualTo("false"));
                Assert.That(model.ServerValue("cpo-1", "OnPinMismatch"),          Is.EqualTo("Refuse"));
                Assert.That(model.ServerValue("cpo-1", "TrustOnFirstUse"),        Is.EqualTo("None"));
                Assert.That(model.ServerValue("cpo-1", "CertificatePins"),        Is.EqualTo(""));
                Assert.That(model.ServerValue("cpo-1", "CertificateStatus"),      Is.EqualTo(""));
                Assert.That(model.ServerValue("cpo-1", "CertificateFingerprint"), Is.EqualTo(""));
                Assert.That(model.ServerValue("cpo-1", "KnownSince"),             Is.EqualTo(""));

                Assert.That(model.ServerValue("dns-1", "SSHUserName"),            Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "SSHHostKeyPins"),         Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "SSHConnection"),          Is.EqualTo("Dedicated"));
                Assert.That(model.ServerValue("dns-1", "SSHClientPublicKey"),     Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "SSHStatus"),              Is.EqualTo(""));

                // The key of the purpose DNS, beside those of the others.
                Assert.That(station.Node.DNSSSHClientPublicKey,                   Is.Not.EqualTo(station.Node.NTSKEClientPublicKey));

            });

        }

        #endregion

        #region WhatTheLastAttemptCameToIsRead(Status, Problem)

        /// <summary>
        /// SSHStatus and SSHHostKeyFingerprint take the place of
        /// CertificateStatus and its fingerprints; Problem is true while
        /// SSHStatus is anything but Accepted; KnownSince is since when the
        /// server is believed with its host key; and OnPinMismatch is what the
        /// server's "ssh" says.
        /// </summary>
        [TestCase("Accepted",              "false")]
        [TestCase("PinMismatch",           "true")]
        [TestCase("Untrusted",             "true")]
        [TestCase("WeakAlgorithms",        "true")]
        [TestCase("AuthenticationFailed",  "true")]
        [TestCase("SubsystemRefused",      "true")]
        [TestCase("NoSSHPeer",             "true")]
        public void WhatTheLastAttemptCameToIsRead(String  Status,
                                                   String  Problem)
        {

            var section = JObject.Parse($$"""
                              { "servers": [ { "id": "cpo-1", "address": "192.0.2.54", "port": 22, "transport": "SSH",
                                               "ssh":        { "hostKeyPins": [ "{{PinA}}" ], "onMismatch": "record" },
                                               "sshStatus":  "{{Status}}",
                                               "sshHostKey": "{{PinB}}",
                                               "judgement":  null,
                                               "known":      { "certificate": "{{PinB}}", "root": null } } ] }
                              """);

            // As the node writes it: a text, with seven digits of the second.
            section["servers"]![0]!["known"]!["since"] = new JValue("2026-11-13T08:09:10.1234567+00:00");

            Assert.Multiple(() => {
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "SSHStatus"),              Is.EqualTo(Status));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "Problem"),                Is.EqualTo(Problem));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "SSHHostKeyFingerprint"),  Is.EqualTo(PinB));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "KnownSince"),             Is.EqualTo("2026-11-13T08:09:10.123Z"));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "OnPinMismatch"),          Is.EqualTo("Record"));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "CertificateStatus"),      Is.EqualTo(""));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "CertificateFingerprint"), Is.EqualTo(""));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "cpo-1", "RootFingerprint"),        Is.EqualTo(""));
            });

        }

        #endregion

        #region ANewServerOverSSHComesWithItsPins()

        /// <summary>
        /// A server over SSH named anew in Servers, with ACID, as the example
        /// of section 7.6 has it: pinned with its current and its next host
        /// key, signed in under a login of its own - written into the file in
        /// "ssh", on the port of SSH.
        /// </summary>
        [Test]
        public async Task ANewServerOverSSHComesWithItsPins()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null,    "Servers",         "dns-1,dns-2,cpo-1,cpo-2"),
                                    ("cpo-2", "Address",         "192.0.2.55"),
                                    ("cpo-2", "Transport",       "SSH"),
                                    ("cpo-2", "SSHUserName",     "resolver"),
                                    ("cpo-2", "SSHHostKeyPins",  $"{PinA}=, {PinB},{PinA}"));

            var onDisk  = (OnDisk()["dns"]?["servers"] as JArray)?.OfType<JObject>().FirstOrDefault(server => server.Value<String>("id") == "cpo-2");

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                                Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(results));

                Assert.That(model.ServerValue("cpo-2", "Transport"),            Is.EqualTo("SSH"));
                Assert.That(model.ServerValue("cpo-2", "Port"),                 Is.EqualTo("22"));
                Assert.That(model.ServerValue("cpo-2", "SSHUserName"),          Is.EqualTo("resolver"));
                Assert.That(model.ServerValue("cpo-2", "SSHHostKeyPins"),       Is.EqualTo($"{PinA},{PinB}"), "a fingerprint with its padding, and twice");
                Assert.That(model.ServerValue("cpo-2", "SSHConnection"),        Is.EqualTo("Dedicated"));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(PinA), "the server beside it");

                Assert.That(JToken.DeepEquals(onDisk?["ssh"], JToken.Parse($$"""{ "user": "resolver", "hostKeyPins": [ "{{PinA}}", "{{PinB}}" ] }""")),
                            Is.True, onDisk?.ToString());

            });

        }

        #endregion

        #region T14_AServerOverSSHThatNothingAuthenticatesIsNeverStored()

        /// <summary>
        /// T14: Transport SSH with no pins, no CAs and SSHConnection Dedicated,
        /// with ACID. Every entry of that server is Rejected with
        /// InvalidSSHHostKeyConfig, the others rolled back, and nothing is
        /// changed. With a pin beside it, the same request is taken. And a
        /// server over SSH does not lose its last pin either.
        /// </summary>
        [Test]
        public async Task T14_AServerOverSSHThatNothingAuthenticatesIsNeverStored()
        {

            var before   = station!.Node.DNSConfigurationJSON()["servers"]!.ToString();
            var file     = File.ReadAllText(station.Node.ConfigFile.Path);

            var refused  = await Set(OCPP21.DataConsistencyModel.ACID,
                                     ("dns-2", "Transport",       "SSH"),
                                     ("dns-2", "QueryTimeout",    "3"),
                                     ("dns-1", "QueryTimeout",    "4"),
                                     (null,    "MaxRetries",      "3"));

            var after    = station.Node.DNSConfigurationJSON()["servers"]!.ToString();
            var fileNow  = File.ReadAllText(station.Node.ConfigFile.Path);

            var alone    = await Set(null, ("dns-2", "Transport", "SSH"));
            var unpinned = await Set(null, ("cpo-1", "SSHHostKeyPins", ""));

            // The control: the same request, with a pin.
            var taken    = await Set(OCPP21.DataConsistencyModel.ACID,
                                     ("dns-2", "Transport",       "SSH"),
                                     ("dns-2", "QueryTimeout",    "3"),
                                     ("dns-1", "QueryTimeout",    "4"),
                                     (null,    "MaxRetries",      "3"),
                                     ("dns-2", "SSHHostKeyPins",  PinB));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(refused),                                Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback, OCPP21.SetVariableStatus.Rolledback }), Said(refused));
                Assert.That(refused[0].AttributeStatusInfo?.ReasonCode,         Is.EqualTo("InvalidSSHHostKeyConfig"));
                Assert.That(refused[1].AttributeStatusInfo?.ReasonCode,         Is.EqualTo("InvalidSSHHostKeyConfig"));
                Assert.That(refused[0].AttributeStatusInfo?.AdditionalInfo,     Does.Contain("'DNSServerController[dns-2]'").And.Contain("never stored"));
                Assert.That(after,                                              Is.EqualTo(before),  "something was changed");
                Assert.That(fileNow,                                            Is.EqualTo(file),    "something was saved");

                Assert.That(StatusesOf(alone),                                  Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected }));
                Assert.That(alone[0].AttributeStatusInfo?.ReasonCode,           Is.EqualTo(DNSDeviceModel.InvalidSSHHostKeyConfig));
                Assert.That(StatusesOf(unpinned),                               Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected }));
                Assert.That(unpinned[0].AttributeStatusInfo?.ReasonCode,        Is.EqualTo(DNSDeviceModel.InvalidSSHHostKeyConfig));

                Assert.That(StatusesOf(taken),                                  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(taken));
                Assert.That(model.ServerValue("dns-2", "Transport"),            Is.EqualTo("SSH"));
                Assert.That(model.ServerValue("dns-2", "Port"),                 Is.EqualTo("22"), "the port of UDP, kept over SSH");
                Assert.That(model.ServerValue("dns-2", "SSHHostKeyPins"),       Is.EqualTo(PinB));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(PinA), "the last pin of a server over SSH was taken away");
                Assert.That(model.ClientValue("MaxRetries"),                    Is.EqualTo("3"));

            });

        }

        #endregion

        #region T15_WhatBelongsToACertificateOrToSSHIsRefusedOnTheOther()

        /// <summary>
        /// T15: CertificatePins for an SSH server, and SSHHostKeyPins for a TLS
        /// server: Rejected, InvalidValue - as is everything else of a
        /// certificate over SSH, trust on first use, an MD5 fingerprint, and
        /// every SSH variable on a server that is not asked over SSH. The empty
        /// value and the default say nothing, and are taken; a CA is.
        /// </summary>
        [Test]
        public async Task T15_WhatBelongsToACertificateOrToSSHIsRefusedOnTheOther()
        {

            var ofACertificate = new List<OCPP21.SetVariableResult>();
            var ofSSH          = new List<OCPP21.SetVariableResult>();

            foreach (var (variable, value) in new[] { ("CertificatePins", APin), ("RootPins", ARoot), ("RootCAs", "office"), ("TrustOnFirstUse", "Root") })
                ofACertificate.AddRange(await Set(null, ("cpo-1", variable, value)));

            foreach (var (variable, value) in new[] { ("SSHHostKeyPins", PinA), ("SSHHostCAs", PinA), ("SSHUserName", "cs001"), ("SSHConnection", "Shared") })
                ofSSH.AddRange(await Set(null, ("dns-1", variable, value)));

            var md5     = await Set(null, ("cpo-1", "SSHHostKeyPins", "MD5:16:27:ac:a5:76:28:2d:36:63:1b:56:4d:eb:df:a6:48"));

            // The controls: what says nothing, and what does apply.
            var nothing = await Set(null, ("dns-1", "SSHHostKeyPins", ""));
            var usual   = await Set(null, ("dns-1", "SSHConnection",  "Dedicated"));
            var cleared = await Set(null, ("cpo-1", "CertificatePins", ""));
            var none    = await Set(null, ("cpo-1", "TrustOnFirstUse", "None"));
            var aCA     = await Set(null, ("cpo-1", "SSHHostCAs",     PinB));
            var aPin    = await Set(null, ("dns-1", "CertificatePins", APin));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(ofACertificate),                         Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), Said(ofACertificate));
                Assert.That(ofACertificate.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InvalidValue"));
                Assert.That(ofACertificate[0].AttributeStatusInfo?.AdditionalInfo, Does.Contain("'DNSServerController[cpo-1].CertificatePins' does not apply to a name server asked over SSH"));
                Assert.That(ofACertificate[3].AttributeStatusInfo?.AdditionalInfo, Does.Contain("not learned on first use"));

                Assert.That(StatusesOf(ofSSH),                                  Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), Said(ofSSH));
                Assert.That(ofSSH.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InvalidValue"));
                Assert.That(ofSSH[0].AttributeStatusInfo?.AdditionalInfo,       Does.Contain("'DNSServerController[dns-1].SSHHostKeyPins' applies to a name server whose Transport is SSH only"));

                Assert.That(md5.Single().AttributeStatus,                       Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(md5.Single().AttributeStatusInfo?.ReasonCode,       Is.EqualTo("InvalidValue"));
                Assert.That(md5.Single().AttributeStatusInfo?.AdditionalInfo,   Does.Contain("is no SHA-256 fingerprint"));

                foreach (var control in new[] { nothing, usual, cleared, none, aCA, aPin })
                    Assert.That(control.Single().AttributeStatus,               Is.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(control));

                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(PinA));
                Assert.That(model.ServerValue("cpo-1", "SSHHostCAs"),           Is.EqualTo(PinB));
                Assert.That(model.ServerValue("cpo-1", "CertificatePins"),      Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "SSHHostKeyPins"),       Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "CertificatePins"),      Is.EqualTo(APin));

            });

        }

        #endregion

        #region TransportDoesNotChangeAwayFromSSHWhileAnSSHVariableIsSet()

        /// <summary>
        /// A change of Transport away from SSH while an SSH variable is set is
        /// Rejected with InvalidValue (section 7.2); with what the server had
        /// over SSH cleared in the same request it is taken, and the server is
        /// one over TLS on the port of TLS, with no "ssh" left in the file.
        /// </summary>
        [Test]
        public async Task TransportDoesNotChangeAwayFromSSHWhileAnSSHVariableIsSet()
        {

            var refused = await Set(null, ("cpo-1", "Transport", "TLS"));
            var still   = model.ServerValue("cpo-1", "Transport");

            var taken   = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("cpo-1", "SSHHostKeyPins",  ""),
                                    ("cpo-1", "Transport",       "TLS"));

            var onDisk  = (OnDisk()["dns"]?["servers"] as JArray)?.OfType<JObject>().FirstOrDefault(server => server.Value<String>("id") == "cpo-1");

            Assert.Multiple(() => {

                Assert.That(refused.Single().AttributeStatus,                   Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(refused.Single().AttributeStatusInfo?.ReasonCode,   Is.EqualTo("InvalidValue"));
                Assert.That(refused.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("cannot change away from SSH"));
                Assert.That(still,                                              Is.EqualTo("SSH"));

                Assert.That(StatusesOf(taken),                                  Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(taken));
                Assert.That(model.ServerValue("cpo-1", "Transport"),            Is.EqualTo("TLS"));
                Assert.That(model.ServerValue("cpo-1", "Port"),                 Is.EqualTo("853"));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(""));
                Assert.That(onDisk?["ssh"],                                     Is.Null, onDisk?.ToString());

            });

        }

        #endregion

        #region OnASharedConnectionNothingOfItsOwnApplies()

        /// <summary>
        /// SSHConnection Shared (section 7.2), for a server whose Address is an
        /// SSH peer of the station: taken, and what only a connection of its
        /// own has goes - its pins and its port. The key it shows is the one
        /// of the connection to that peer. After that Port, SSHUserName,
        /// SSHHostKeyPins, SSHHostCAs and OnPinMismatch do not apply; and it
        /// becomes a connection of its own again together with its pins.
        /// </summary>
        [Test]
        public async Task OnASharedConnectionNothingOfItsOwnApplies()
        {

            var shared  = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("cpo-1", "SSHConnection",  "Shared"),
                                    ("cpo-1", "Address",        "lc.example"));

            var pins    = model.ServerValue("cpo-1", "SSHHostKeyPins");
            var port    = model.ServerValue("cpo-1", "Port");
            var key     = model.ServerValue("cpo-1", "SSHClientPublicKey");
            var onDisk  = (OnDisk()["dns"]?["servers"] as JArray)?.OfType<JObject>().FirstOrDefault(server => server.Value<String>("id") == "cpo-1");

            var refused = new List<OCPP21.SetVariableResult>();

            foreach (var (variable, value) in new[] { ("Port", "2222"), ("SSHUserName", "cs001"), ("SSHHostKeyPins", PinA), ("SSHHostCAs", PinA), ("OnPinMismatch", "Record") })
                refused.AddRange(await Set(null, ("cpo-1", variable, value)));

            // The controls: what says nothing is taken, and so is a pin on a connection of its own.
            var usual   = await Set(null, ("cpo-1", "OnPinMismatch", "Refuse"));
            var own     = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("cpo-1", "SSHHostKeyPins",  PinA),
                                    ("cpo-1", "SSHUserName",     "cs001"),
                                    ("cpo-1", "SSHConnection",   "Dedicated"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(shared),                                 Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(shared));
                Assert.That(pins,                                               Is.EqualTo(""),  "the pins of its own connection stayed");
                Assert.That(port,                                               Is.EqualTo(""),  "a port of its own, on a shared connection");
                Assert.That(key,                                                Is.EqualTo(station!.Node.SSHPeerTo(DomainName.Parse("lc.example"))?.PublicKey?.Invoke()), "the key of the connection to that SSH peer");
                Assert.That(key,                                                Does.StartWith("ssh-"));
                Assert.That(JToken.DeepEquals(onDisk?["ssh"], JToken.Parse("""{ "connection": "shared" }""")), Is.True, onDisk?.ToString());
                Assert.That(onDisk?["port"],                                    Is.Null, onDisk?.ToString());

                Assert.That(StatusesOf(refused),                                Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), Said(refused));
                Assert.That(refused.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InvalidValue"));
                Assert.That(refused.Select(result => result.AttributeStatusInfo?.AdditionalInfo), Is.All.Contains("does not apply with SSHConnection Shared"));

                Assert.That(usual.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(usual));
                Assert.That(StatusesOf(own),                                    Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(own));
                Assert.That(model.ServerValue("cpo-1", "SSHConnection"),        Is.EqualTo("Dedicated"));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(PinA));
                Assert.That(model.ServerValue("cpo-1", "SSHUserName"),          Is.EqualTo("cs001"));
                Assert.That(model.ServerValue("cpo-1", "Port"),                 Is.EqualTo("22"));

            });

        }

        #endregion

        #region SSHConnectionIsAppliedAfterTransportAndBeforeTheOtherVariables()

        /// <summary>
        /// In a request, SSHConnection is applied after Transport and before
        /// the other variables, whatever its place (section 7.2). Named before
        /// Transport, it is still set on the server over SSH the request makes;
        /// named after a pin, it still decides that the pin does not apply.
        /// </summary>
        [Test]
        public async Task SSHConnectionIsAppliedAfterTransportAndBeforeTheOtherVariables()
        {

            var afterTransport = await Set(OCPP21.DataConsistencyModel.ACID,
                                           ("dns-2", "SSHConnection",  "Shared"),
                                           ("dns-2", "Address",        "lc.example"),
                                           ("dns-2", "Transport",      "SSH"));

            var beforeTheRest  = await Set(OCPP21.DataConsistencyModel.ACID,
                                           ("cpo-1", "SSHHostKeyPins", PinB),
                                           ("cpo-1", "Address",        "lc.example"),
                                           ("cpo-1", "SSHConnection",  "Shared"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(afterTransport),                         Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(afterTransport));
                Assert.That(model.ServerValue("dns-2", "Transport"),            Is.EqualTo("SSH"));
                Assert.That(model.ServerValue("dns-2", "SSHConnection"),        Is.EqualTo("Shared"));

                Assert.That(StatusesOf(beforeTheRest),                          Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback, OCPP21.SetVariableStatus.Rolledback }), Said(beforeTheRest));
                Assert.That(beforeTheRest[0].AttributeStatusInfo?.ReasonCode,   Is.EqualTo("InvalidValue"));
                Assert.That(beforeTheRest[0].AttributeStatusInfo?.AdditionalInfo, Does.Contain("'DNSServerController[cpo-1].SSHHostKeyPins' does not apply with SSHConnection Shared"));
                Assert.That(model.ServerValue("cpo-1", "SSHConnection"),        Is.EqualTo("Dedicated"));
                Assert.That(model.ServerValue("cpo-1", "SSHHostKeyPins"),       Is.EqualTo(PinA));
                Assert.That(model.ServerValue("cpo-1", "Address"),              Is.EqualTo("192.0.2.54"));

            });

        }

        #endregion

        #region T16_ASharedConnectionNeedsAnAddressThatIsAnSSHPeer()

        /// <summary>
        /// T16: SSHConnection Shared with an Address that is no SSH peer of
        /// the station: Rejected, InvalidValue, and nothing changed - where the
        /// address is another one in the same request as well. The same
        /// request with the address of the SSH peer is taken.
        /// </summary>
        [Test]
        public async Task T16_ASharedConnectionNeedsAnAddressThatIsAnSSHPeer()
        {

            var before    = station!.Node.DNSConfigurationJSON()["servers"]!.ToString();

            var noPeer    = await Set(null, ("cpo-1", "SSHConnection", "Shared"));

            var elsewhere = await Set(OCPP21.DataConsistencyModel.ACID,
                                      ("cpo-1", "SSHConnection",  "Shared"),
                                      ("cpo-1", "Address",        "elsewhere.example"),
                                      (null,    "MaxRetries",     "3"));

            var after     = station.Node.DNSConfigurationJSON()["servers"]!.ToString();

            // The control: the same request, to the SSH peer.
            var peer      = await Set(OCPP21.DataConsistencyModel.ACID,
                                      ("cpo-1", "SSHConnection",  "Shared"),
                                      ("cpo-1", "Address",        "LC.example."),
                                      (null,    "MaxRetries",     "3"));

            Assert.Multiple(() => {

                Assert.That(station.Node.DNSConfigurationJSON()["sshPeers"]?.Values<String>(), Has.Member("lc.example"));

                Assert.That(noPeer.Single().AttributeStatus,                    Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(noPeer.Single().AttributeStatusInfo?.ReasonCode,    Is.EqualTo("InvalidValue"));
                Assert.That(noPeer.Single().AttributeStatusInfo?.AdditionalInfo, Does.Contain("'192.0.2.54' is no SSH peer of this charging station"));

                Assert.That(StatusesOf(elsewhere),                              Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }), Said(elsewhere));
                Assert.That(elsewhere[0].AttributeStatusInfo?.ReasonCode,       Is.EqualTo("InvalidValue"));
                Assert.That(elsewhere[1].AttributeStatusInfo?.ReasonCode,       Is.EqualTo("InvalidValue"));
                Assert.That(after,                                              Is.EqualTo(before), "something was changed");

                Assert.That(StatusesOf(peer),                                   Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(peer));
                Assert.That(model.ServerValue("cpo-1", "SSHConnection"),        Is.EqualTo("Shared"));

            });

        }

        #endregion

        #region T17_ACSMSDoesNotResolveItsOwnNameThroughItsOwnConnection()

        /// <summary>
        /// T17: the CSMS is dialled at ssh://lc.example:22, and the only name
        /// server would be SSHConnection Shared to lc.example. Rejected,
        /// CSMSUnreachable, although lc.example resolves now: it would not
        /// after the next reconnect, with no connection yet to resolve it
        /// over. With a second name server beside it, the same change is
        /// taken.
        /// </summary>
        [Test]
        public async Task T17_ACSMSDoesNotResolveItsOwnNameThroughItsOwnConnection()
        {

            var origin      = DomainName.Parse("example.");

            var nameServer  = new DNSServer(
                                  new AuthoritativeDNSRequestHandler(
                                      new InMemoryDNSZone().
                                          AddZoneFileString("example. 300 IN SOA ns.example. hostmaster.example. 1 3600 600 86400 300", Origin: origin).
                                          AddZoneFileString("lc.example. 300 IN A 127.0.0.42",                                          Origin: origin)
                                  ),
                                  new DNSServerOptions {
                                      EnableUDPUnicast    = false,
                                      EnableUDPMulticast  = false,
                                      EnableTCPUnicast    = true,
                                      TCPUnicastSocket    = new IPSocket(IPv4Address.Localhost, IPPort.Parse(0))
                                  }
                              );

            await nameServer.Start();

            try
            {

                var port = nameServer.ActiveTCPUnicastSocket!.Value.Port.ToUInt16();

                Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse($$"""
                                { "servers": [ { "id": "plain", "address": "127.0.0.1", "port": {{port}}, "transport": "TCP", "queryTimeoutSeconds": 3 } ], "maxRetries": 0, "useCache": false }
                                """), out var error),
                            Is.True, error);

                Assert.That(station.TryDial("ssh://lc.example:22", "2.1", out error), Is.True, error);

                // It resolves now.
                var now      = await station.Node.ResolveWithAsync(new JObject(), [ "lc.example" ], TimeSpan.FromSeconds(10));

                var cutOff   = await Set(OCPP21.DataConsistencyModel.ACID,
                                         (null, "Servers",        "lc"),
                                         ("lc", "Address",        "lc.example"),
                                         ("lc", "Transport",      "SSH"),
                                         ("lc", "SSHConnection",  "Shared"));

                var servers  = model.ClientValue("Servers");

                // The control: the same server, beside the one that resolves the name.
                var beside   = await Set(OCPP21.DataConsistencyModel.ACID,
                                         (null, "Servers",        "plain,lc"),
                                         ("lc", "Address",        "lc.example"),
                                         ("lc", "Transport",      "SSH"),
                                         ("lc", "SSHConnection",  "Shared"));

                Assert.Multiple(() => {

                    Assert.That(now.Resolves("lc.example"),                     Is.True, "the name of the CSMS does not resolve now");

                    Assert.That(StatusesOf(cutOff),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), Said(cutOff));
                    Assert.That(cutOff.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("CSMSUnreachable"));
                    Assert.That(cutOff[0].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("'lc.example' would not resolve"));
                    Assert.That(servers,                                        Is.EqualTo("plain"));

                    Assert.That(StatusesOf(beside),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted), Said(beside));
                    Assert.That(model.ClientValue("Servers"),                   Is.EqualTo("plain,lc"));
                    Assert.That(model.ServerValue("lc", "SSHConnection"),       Is.EqualTo("Shared"));

                });

            }
            finally
            {
                await nameServer.Stop();
            }

        }

        #endregion

        #region TheKeysOfOCPP16AreReadFromTheNode()

        /// <summary>
        /// The keys of section 7.5: "dns.transports" says SSH, the logins and
        /// connections are lists per server, the pins and CAs are keys of the
        /// n-th server, and the host keys shown are read and not written.
        /// </summary>
        [Test]
        public async Task TheKeysOfOCPP16AreReadFromTheNode()
        {

            var keys     = Keys.Keys.ToArray();
            var readOnly = await Change("dns.sshHostKeyFingerprints", $"{PinA},,");

            Assert.Multiple(() => {

                Assert.That(keys,  Is.SupersetOf(new[] { "dns.sshUserNames", "dns.sshConnection", "dns.sshHostKeyFingerprints", "dns.sshHostKeyPins.1", "dns.sshHostKeyPins.3", "dns.sshHostCAs.3" }));
                Assert.That(keys,  Has.None.EqualTo("dns.sshHostKeyPins.4"));

                Assert.That(Value("dns.transports"),                Is.EqualTo("TLS,UDP,SSH"));
                Assert.That(Value("dns.ports"),                     Is.EqualTo("853,53,22"));
                Assert.That(Value("dns.sshUserNames"),              Is.EqualTo(",,"));
                Assert.That(Value("dns.sshConnection"),             Is.EqualTo("Dedicated,Dedicated,Dedicated"));
                Assert.That(Value("dns.sshHostKeyPins.3"),          Is.EqualTo(PinA));
                Assert.That(Value("dns.sshHostKeyPins.1"),          Is.EqualTo(""));
                Assert.That(Value("dns.sshHostCAs.3"),              Is.EqualTo(""));
                Assert.That(Value("dns.sshHostKeyFingerprints"),    Is.EqualTo(",,"));

                Assert.That(Keys.Get("dns.sshHostKeyFingerprints")?.AccessRights, Is.EqualTo(OCPP16.AccessRights.ReadOnly));
                Assert.That(Keys.Get("dns.sshUserNames")?.AccessRights,           Is.EqualTo(OCPP16.AccessRights.ReadWrite));
                Assert.That(Keys.Get("DNS.SSHHOSTKEYPINS.3")?.Key,                Is.EqualTo("dns.sshHostKeyPins.3"));
                Assert.That(readOnly,                                             Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));

            });

        }

        #endregion

        #region AKeyOfOCPP16SaysWhatGoesWithIt()

        /// <summary>
        /// A key of OCPP 1.6 is one change, and a server over SSH is made by
        /// several variables at once: so "dns.transports" alone cannot make one
        /// - it would be one nothing authenticates - a host key pinned makes
        /// its server one over SSH, "Shared" does where the address is an SSH
        /// peer, a pin makes a shared connection one of its own again, and
        /// "dns.transports" taking a server away from SSH takes its pins with
        /// it. An empty login is the station's identity, and no login repeats
        /// onto the next server.
        /// </summary>
        [Test]
        public async Task AKeyOfOCPP16SaysWhatGoesWithIt()
        {

            var byTransport  = await Change("dns.transports",       "TLS,SSH,SSH");
            var transports1  = Value("dns.transports");

            var byPin        = await Change("dns.sshHostKeyPins.2", PinB);
            var transports2  = Value("dns.transports");
            var ports2       = Value("dns.ports");

            var login        = await Change("dns.sshUserNames",     ",resolver");
            var logins       = Value("dns.sshUserNames");

            var noPeer       = await Change("dns.sshConnection",    ",,Shared");
            var moved        = await Change("dns.servers",          "192.0.2.53,2001:db8::53,lc.example");
            var shared       = await Change("dns.sshConnection",    ",,Shared");
            var connections  = Value("dns.sshConnection");
            var pins3        = Value("dns.sshHostKeyPins.3");

            var ownAgain     = await Change("dns.sshHostKeyPins.3", PinA);
            var connections2 = Value("dns.sshConnection");

            var away         = await Change("dns.transports",       "TLS,UDP,SSH");

            Assert.Multiple(() => {

                Assert.That(byTransport,                            Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "a server over SSH that nothing authenticates");
                Assert.That(transports1,                            Is.EqualTo("TLS,UDP,SSH"));

                Assert.That(byPin,                                  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(transports2,                            Is.EqualTo("TLS,SSH,SSH"));
                Assert.That(ports2,                                 Is.EqualTo("853,22,22"));

                Assert.That(login,                                  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(logins,                                 Is.EqualTo(",resolver,"), "the login of the server before it repeated");

                Assert.That(noPeer,                                 Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "192.0.2.54 is no SSH peer");
                Assert.That(moved,                                  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(shared,                                 Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(connections,                            Is.EqualTo("Dedicated,Dedicated,Shared"));
                Assert.That(pins3,                                  Is.EqualTo(""));

                Assert.That(ownAgain,                               Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(connections2,                           Is.EqualTo("Dedicated,Dedicated,Dedicated"));

                Assert.That(away,                                   Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("dns.transports"),                Is.EqualTo("TLS,UDP,SSH"));
                Assert.That(Value("dns.sshHostKeyPins.2"),          Is.EqualTo(""));
                Assert.That(Value("dns.sshUserNames"),              Is.EqualTo(",,"));
                Assert.That(Value("dns.sshHostKeyPins.3"),          Is.EqualTo(PinA));

            });

        }

        #endregion

        #region WhatAnAttemptOverSSHCameToIsToldAsItsSecurityEvent()

        /// <summary>
        /// The security events of section 7.4, to each version that is dialled:
        /// a host key nothing vouches for is InvalidNameServerHostKey whether
        /// it was refused or used all the same, a host key that changed is
        /// NameServerHostKeyChanged, a key that was not taken
        /// NameServerAuthenticationFailed, and a server without strict KEX
        /// InvalidSSHAlgorithms - each naming the server, and the key it showed.
        /// </summary>
        [Test]
        public void WhatAnAttemptOverSSHCameToIsToldAsItsSecurityEvent()
        {

            Assert.That(station!.TryDial("ws://127.0.0.1:9/cs", "2.1", out var error), Is.True, error);
            Assert.That(station .TryDial("ws://127.0.0.1:9/cs", "1.6", out error),     Is.True, error);

            var server  = new DNSServerConfig(IPv4Address.Parse("192.0.2.54"), IPPort.SSH, DNSTransport.SSH);
            var at      = DateTimeOffset.UtcNow;

            NameServerSSHState State(NameServerSSHStatus Status, Boolean Matched, String Why, String? Previously = null)
                => new (at, Status, PinB, Matched, Why, Previously);

            station.Bridge.WhenNameServerSSHNews(server, State(NameServerSSHStatus.PinMismatch,          false, "its host key is none it is held to"),  NameServerSSHNews.HostKeyRefused);
            station.Bridge.WhenNameServerSSHNews(server, State(NameServerSSHStatus.Accepted,             false, "its host key is none it is held to"),  NameServerSSHNews.HostKeyTolerated);
            station.Bridge.WhenNameServerSSHNews(server, State(NameServerSSHStatus.Accepted,             true,  "its host key is pinned", PinA),         NameServerSSHNews.HostKeyChanged);
            station.Bridge.WhenNameServerSSHNews(server, State(NameServerSSHStatus.AuthenticationFailed, true,  "it did not take the key"),              NameServerSSHNews.AuthenticationFailed);
            station.Bridge.WhenNameServerSSHNews(server, State(NameServerSSHStatus.WeakAlgorithms,       true,  "it does not signal strict KEX"),        NameServerSSHNews.WeakAlgorithms);

            var told21 = Security(station.OCPPv2_1Node.OfflineQueue);
            var told16 = Security(station.OCPPv1_6Node.OfflineQueue);

            Assert.Multiple(() => {

                Assert.That(told21.Select(one => one.Type),  Is.EqualTo(new[] { "InvalidNameServerHostKey", "InvalidNameServerHostKey", "NameServerHostKeyChanged", "NameServerAuthenticationFailed", "InvalidSSHAlgorithms" }));
                Assert.That(told16,                          Is.EqualTo(told21));

                Assert.That(told21[0].TechInfo,              Is.EqualTo($"ssh://192.0.2.54:22: PinMismatch, host key {PinB}, refused: its host key is none it is held to"));
                Assert.That(told21[1].TechInfo,              Does.StartWith($"ssh://192.0.2.54:22: host key {PinB}, used although nothing vouches for it"));
                Assert.That(told21[2].TechInfo,              Is.EqualTo($"ssh://192.0.2.54:22: host key {PinA} -> {PinB}"));
                Assert.That(told21[3].TechInfo,              Is.EqualTo("ssh://192.0.2.54:22: it did not take the key"));
                Assert.That(told21[4].TechInfo,              Is.EqualTo("ssh://192.0.2.54:22: it does not signal strict KEX"));

                Assert.That(OCPP21.SecurityEventType.InvalidNameServerHostKey.      IsCritical,  Is.True);
                Assert.That(OCPP21.SecurityEventType.NameServerHostKeyChanged.      IsCritical,  Is.False);
                Assert.That(OCPP21.SecurityEventType.NameServerAuthenticationFailed.IsCritical,  Is.False);

            });

        }

        #endregion


        #region (private) Keys / Value(Key) / Change(Key, Value)

        private OCPP16.CP.IConfigurationKeySource Keys
            => station!.OCPPv1_6Node.ConfigurationKeySources.OfType<DNSDeviceModel>().Single();

        private String? Value(String Key)
            => Keys.Get(Key)?.Value;

        private Task<OCPP16.ConfigurationStatus?> Change(String Key, String Value)
            => Keys.ChangeAsync(Key, Value, "CS-16");

        #endregion

        #region (private) OnDisk()

        /// <summary>
        /// The node's file as it is now.
        /// </summary>
        private JObject OnDisk()

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path));

        #endregion

        #region (private) Set(Model, params Entries)

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

        #endregion

        #region (private static) StatusesOf(Results) / Said(Results) / Security(Queue)

        private static OCPP21.SetVariableStatus[] StatusesOf(IEnumerable<OCPP21.SetVariableResult> Results)

            => [.. Results.Select(result => result.AttributeStatus)];

        /// <summary>
        /// What each result says, for the line of a failed assertion.
        /// </summary>
        private static String Said(IEnumerable<OCPP21.SetVariableResult> Results)

            => String.Join(" | ", Results.Select(result => $"{result.Component.Instance ?? "client"}.{result.Variable.Name}: {result.AttributeStatus} {result.AttributeStatusInfo?.ReasonCode} {result.AttributeStatusInfo?.AdditionalInfo}"));

        /// <summary>
        /// The security events waiting in a queue, with their techInfo.
        /// </summary>
        private static (String Type, String? TechInfo)[] Security(OCPPC.OfflineQueue Queue)

            => [.. Queue.Pending.Where (message => message.Kind == "SecurityEventNotification").
                                 Select(message => (message.Payload.Value<String>("type")!, message.Payload.Value<String>("techInfo")))];

        #endregion

    }

}
