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

using org.GraphDefined.Vanaheimr.Hermod.DNS;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The SSH peers of a node as OCPP 1.6 reads and changes them: the
    /// configuration keys that stand for SSHPeerController, against what the
    /// node then holds.
    /// </summary>
    public abstract class SSHPeerKeys16Tests : ABridgeTests
    {

        #region Data

        private static readonly String  PinA  = "SHA256:" + new String('A', 43);
        private static readonly String  PinB  = "SHA256:" + new String('B', 43);
        private static readonly String  PinC  = "SHA256:" + new String('C', 43);

        private String         directory  = "";
        private IBridgedNode?  station;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void ANodeWithOneSSHPeer()
        {

            directory  = TestNodes.TemporaryDirectory("ssh-peer-keys16");

            var configuration = TestNodes.Offline;

            configuration["ssh"] = JObject.Parse($$"""{ "peers": [ { "id": "lc", "address": "lc.example", "user": "cs001", "hostKeyPins": [ "{{PinA}}" ] } ] }""");

            station    = Nodes.New(directory, configuration);

            // Whoever writes the peers over OCPP has signed it: here, everybody has.
            station.Bridge.SSHPeers.SignedBy = (OCPP21.CSMS.SetVariablesRequest? request, out String? signer, out String? why) => {
                signer  = "the test";
                why     = null;
                return true;
            };

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


        private OCPP16.CP.IConfigurationKeySource Keys
            => station!.OCPPv1_6Node.ConfigurationKeySources.OfType<SSHPeerDeviceModel>().Single();

        private String? Value(String Key)
            => Keys.Get(Key)?.Value;

        private Task<OCPP16.ConfigurationStatus?> Change(String Key, String Value)
            => Keys.ChangeAsync(Key, Value, "CS-16");

        private JArray OnDisk()
            => (JArray) JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path))["ssh"]!["peers"]!;


        #region TheKeysAreReadFromTheNode()

        /// <summary>
        /// "sshPeers" and the key the node signs in with, and under each peer's
        /// name where it is, what it is held to and what it showed. A key is
        /// found whatever its case and named as it is written; one of a peer
        /// the node does not have is none of these keys.
        /// </summary>
        [Test]
        public void TheKeysAreReadFromTheNode()
        {

            Assert.Multiple(() => {

                Assert.That(Keys.Keys, Is.EqualTo(new[] {
                                           "sshPeers", "sshPeers.clientPublicKey",
                                           "sshPeer.lc.url", "sshPeer.lc.hostKeyPins", "sshPeer.lc.hostCAs", "sshPeer.lc.onPinMismatch",
                                           "sshPeer.lc.hostKeyFingerprint", "sshPeer.lc.connected", "sshPeer.lc.problem",
                                           "sshPeer.lc.fileTransfers", "sshPeer.lc.fileTransferUsers", "sshPeer.lc.uploadPaths", "sshPeer.lc.downloadPaths",
                                           "sshPeer.lc.fileTransferConnection", "sshPeer.lc.fileTransferPublicKeys", "sshPeer.lc.fileTransferKeyStates"
                                       }));

                Assert.That(Value("sshPeers"),                         Is.EqualTo("lc"));
                Assert.That(Value("sshPeers.clientPublicKey"),         Does.StartWith("ssh-ed25519 "));
                Assert.That(Value("sshPeer.lc.url"),                   Is.EqualTo("ssh://cs001@lc.example"));
                Assert.That(Value("sshPeer.lc.hostKeyPins"),           Is.EqualTo(PinA));
                Assert.That(Value("sshPeer.lc.hostCAs"),               Is.EqualTo(""));
                Assert.That(Value("sshPeer.lc.onPinMismatch"),         Is.EqualTo("Refuse"));
                Assert.That(Value("sshPeer.lc.hostKeyFingerprint"),    Is.EqualTo(""));
                Assert.That(Value("sshPeer.lc.connected"),             Is.EqualTo("false"));
                Assert.That(Value("sshPeer.lc.problem"),               Is.EqualTo("false"));

                Assert.That(Keys.Get("SSHPEER.LC.URL")?.Key,           Is.EqualTo("sshPeer.lc.url"));
                Assert.That(Keys.Get("sshPeer.lc.url")?.AccessRights,  Is.EqualTo(OCPP16.AccessRights.ReadWrite));
                Assert.That(Keys.Get("sshPeer.lc.connected")?.AccessRights,       Is.EqualTo(OCPP16.AccessRights.ReadOnly));
                Assert.That(Keys.Get("sshPeers.clientPublicKey")?.AccessRights,   Is.EqualTo(OCPP16.AccessRights.ReadOnly));

                Assert.That(Keys.Get("sshPeer.time.url"),              Is.Null);
                Assert.That(Keys.Get("sshPeer.lc.colour"),             Is.Null);
                Assert.That(Keys.Get("dns.servers"),                   Is.Null);

            });

        }

        #endregion

        #region APeerIsAddedByItsURL()

        /// <summary>
        /// ChangeConfiguration changes one key, and a peer nobody could believe
        /// is never stored: so a peer is added by its "url", which says what it
        /// is held to after a '#'. Without that it is refused, and so is its
        /// name in "sshPeers" alone.
        /// </summary>
        [Test]
        public async Task APeerIsAddedByItsURL()
        {

            var nameAlone  = await Change("sshPeers",          "lc,time");
            var noPins     = await Change("sshPeer.time.url",  "ssh://10.10.0.7:2222");
            var noURL      = await Change("sshPeer.time.url",  "10.10.0.7");
            var pinsFirst  = await Change("sshPeer.time.hostKeyPins", PinB);

            var added      = await Change("sshPeer.time.url",  $"ssh://station-7@10.10.0.7:2222#{PinB}, {PinC}");

            Assert.Multiple(() => {

                Assert.That(nameAlone,                                 Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "a name alone became a peer");
                Assert.That(noPins,                                    Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "a peer held to nothing was added");
                Assert.That(noURL,                                     Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(pinsFirst,                                 Is.Null, "a key of a peer the node does not have");

                Assert.That(added,                                     Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("sshPeers"),                         Is.EqualTo("lc,time"));
                Assert.That(Value("sshPeer.time.url"),                 Is.EqualTo("ssh://station-7@10.10.0.7:2222"));
                Assert.That(Value("sshPeer.time.hostKeyPins"),         Is.EqualTo($"{PinB},{PinC}"));
                Assert.That(Value("sshPeer.lc.url"),                   Is.EqualTo("ssh://cs001@lc.example"), "the peer that was there changed");

                Assert.That(JToken.DeepEquals(OnDisk()[1], JObject.Parse($$"""{ "id": "time", "address": "10.10.0.7", "user": "station-7", "port": 2222, "hostKeyPins": [ "{{PinB}}", "{{PinC}}" ] }""")),
                            Is.True, OnDisk().ToString());

                Assert.That(station!.Node.SSHPeerTo(DomainName.Parse("10.10.0.7"))?.Port, Is.EqualTo(2222));
                Assert.That(Keys.Keys,                                 Does.Contain("sshPeer.time.url"));

            });

        }

        #endregion

        #region APeerIsChangedKeyByKey()

        /// <summary>
        /// A peer that is there takes its keys one by one. Its "url" without
        /// pins leaves its pins alone, and with them they are its pins; what
        /// only reads is refused, and so is leaving it with nothing to hold it
        /// to.
        /// </summary>
        [Test]
        public async Task APeerIsChangedKeyByKey()
        {

            var moved      = await Change("sshPeer.lc.url",            "ssh://lc2.example:2022");
            var urlNow     = Value("sshPeer.lc.url");
            var pinsKept   = Value("sshPeer.lc.hostKeyPins");

            var withPins   = await Change("sshPeer.lc.url",            $"ssh://cs001@lc2.example#{PinB}");
            var pinsNow    = Value("sshPeer.lc.hostKeyPins");

            var cas        = await Change("sshPeer.lc.hostCAs",        PinC);
            var mismatch   = await Change("SSHPeer.LC.OnPinMismatch",  "Record");
            var noPins     = await Change("sshPeer.lc.hostKeyPins",    "");

            var nothing    = await Change("sshPeer.lc.hostCAs",        "");
            var readOnly   = await Change("sshPeer.lc.connected",      "true");
            var itsKey     = await Change("sshPeers.clientPublicKey",  "ssh-ed25519 AAAA");
            var md5        = await Change("sshPeer.lc.hostKeyPins",    "ab:cd:ef");
            var notOurs    = await Change("dns.servers",               "192.0.2.1");

            Assert.Multiple(() => {

                Assert.That(moved,                                     Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(urlNow,                                    Is.EqualTo("ssh://lc2.example:2022"), "the login it had stayed, though the URL names none");
                Assert.That(pinsKept,                                  Is.EqualTo(PinA), "a URL without pins took the pins away");

                Assert.That(withPins,                                  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(pinsNow,                                   Is.EqualTo(PinB));
                Assert.That(Value("sshPeer.lc.url"),                   Is.EqualTo("ssh://cs001@lc2.example"));

                Assert.That((cas, mismatch, noPins),                   Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted)));
                Assert.That(Value("sshPeer.lc.hostCAs"),               Is.EqualTo(PinC));
                Assert.That(Value("sshPeer.lc.onPinMismatch"),         Is.EqualTo("Record"));
                Assert.That(Value("sshPeer.lc.hostKeyPins"),           Is.EqualTo(""));

                Assert.That(nothing,                                   Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "a peer was left with nothing to hold it to");
                Assert.That(readOnly,                                  Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(itsKey,                                    Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(md5,                                       Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(notOurs,                                   Is.Null);

                Assert.That(station!.Node.SSHPeerTo(DomainName.Parse("lc2.example")), Is.Not.Null);
                Assert.That(station.Node.SSHPeerTo(DomainName.Parse("lc.example")),   Is.Null);

            });

        }

        #endregion

        #region APeerIsTakenOutByTheList()

        /// <summary>
        /// A name "sshPeers" leaves out is a peer no longer, and its keys are
        /// no keys any more.
        /// </summary>
        [Test]
        public async Task APeerIsTakenOutByTheList()
        {

            await Change("sshPeer.time.url", $"ssh://10.10.0.7#{PinB}");

            var one   = await Change("sshPeers", "time");
            var left  = Value("sshPeers");
            var none  = await Change("sshPeers", "");

            Assert.Multiple(() => {
                Assert.That(one,                                       Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(left,                                      Is.EqualTo("time"));
                Assert.That(none,                                      Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("sshPeers"),                         Is.EqualTo(""));
                Assert.That(Keys.Get("sshPeer.lc.url"),                Is.Null);
                Assert.That(Keys.Keys,                                 Is.EqualTo(new[] { "sshPeers", "sshPeers.clientPublicKey" }));
                Assert.That(OnDisk().Count,                            Is.Zero);
                Assert.That(station!.Node.SSHPeers,                    Is.Empty);
            });

        }

        #endregion

        #region File servers, and who may write the peers

        /// <summary>
        /// What a peer may be used for as a file server has keys of its own,
        /// read and written like the variables of OCPP 2.1.
        /// </summary>
        [Test]
        public async Task APeerIsAFileServerByItsKeys()
        {

            Assert.That(Keys.Keys, Is.SupersetOf(new[] {
                                       "sshPeer.lc.fileTransfers", "sshPeer.lc.fileTransferUsers", "sshPeer.lc.uploadPaths", "sshPeer.lc.downloadPaths",
                                       "sshPeer.lc.fileTransferConnection", "sshPeer.lc.fileTransferPublicKeys", "sshPeer.lc.fileTransferKeyStates"
                                   }));

            Assert.That(Value("sshPeer.lc.fileTransfers"),                               Is.EqualTo(""));
            Assert.That(Keys.Get("sshPeer.lc.fileTransfers")?.AccessRights,              Is.EqualTo(OCPP16.AccessRights.ReadWrite));
            Assert.That(Keys.Get("sshPeer.lc.fileTransferPublicKeys")?.AccessRights,     Is.EqualTo(OCPP16.AccessRights.ReadOnly));

            Assert.That(await Change("sshPeer.lc.fileTransfers",  "Upload"),      Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
            Assert.That(await Change("sshPeer.lc.uploadPaths",    "/upload/{id}"), Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));

            Assert.That(Value("sshPeer.lc.fileTransfers"),           Is.EqualTo("Upload"));
            Assert.That(Value("sshPeer.lc.uploadPaths"),             Is.EqualTo("/upload/{id}/"));
            Assert.That(Value("sshPeer.lc.fileTransferConnection"),  Is.EqualTo("Dedicated"));

            Assert.That(OnDisk()[0]["fileTransfers"]!.ToString(Newtonsoft.Json.Formatting.None),
                        Is.EqualTo("""{"directions":["upload"],"uploadPaths":["/upload/{id}/"]}"""));

            // The key of the peer's login, and whether it is used.
            Assert.That(Value("sshPeer.lc.fileTransferPublicKeys"),  Does.StartWith("ssh-ed25519 ").And.EndWith(" cs001@lc"));
            Assert.That(Value("sshPeer.lc.fileTransferKeyStates"),   Is.EqualTo("cs001=Unconfirmed"));

            Assert.That(await Change("sshPeer.lc.fileTransferPublicKeys", "ssh-ed25519 AAAA mine"), Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
            Assert.That(await Change("sshPeer.lc.uploadPaths",            "upload/"),               Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));

            // Another key of the peer changed: what it may be used for is still there.
            Assert.That(await Change("sshPeer.lc.hostKeyPins", $"{PinA},{PinB}"), Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
            Assert.That(Value("sshPeer.lc.uploadPaths"),             Is.EqualTo("/upload/{id}/"));

        }

        /// <summary>
        /// A ChangeConfiguration of OCPP 1.6 brings no signature here, so with a
        /// signer that checks one, no key of the peers is written over OCPP 1.6.
        /// </summary>
        [Test]
        public async Task WithoutASignatureNoKeyOfThePeersIsWritten()
        {

            var before = OnDisk().ToString();

            foreach (var check in new SSHPeerSignatureCheck?[] {
                         null,
                         (OCPP21.CSMS.SetVariablesRequest? request, out String? signer, out String? why) => {
                             signer  = null;
                             why     = request?.Signatures.Any() == true ? "not believed" : "not signed";
                             return false;
                         }
                     })
            {

                ((SSHPeerDeviceModel) Keys).SignedBy = check;

                Assert.That(await Change("sshPeer.lc.hostKeyPins",    PinB),                                Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(await Change("sshPeer.lc.url",            "ssh://cs001@elsewhere.example"),     Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(await Change("sshPeer.lc.fileTransfers",  "Upload"),                            Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(await Change("sshPeer.new.url",           $"ssh://new.example#{PinB}"),         Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(await Change("sshPeers",                  ""),                                  Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));

            }

            Assert.That(OnDisk().ToString(),            Is.EqualTo(before), "nothing was written");
            Assert.That(Value("sshPeer.lc.hostKeyPins"), Is.EqualTo(PinA),  "and everything is still read");

        }

        #endregion

    }

}
