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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// SSHPeerController: what a CSMS reads of the SSH peers of a node, and
    /// what it changes of them through SetVariables, against what the node
    /// then holds - in its file, among its peers, and in its Device Model.
    /// </summary>
    /// <remarks>
    /// The node is built and not started, and nothing is dialled: a peer's
    /// connection is made when something first needs it.
    /// </remarks>
    public abstract class SSHPeerDeviceModelTests : ABridgeTests
    {

        #region Data

        private static readonly String  PinA  = "SHA256:" + new String('A', 43);
        private static readonly String  PinB  = "SHA256:" + new String('B', 43);

        private String              directory  = "";
        private IBridgedNode?       station;
        private SSHPeerDeviceModel  model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void ANodeWithOneSSHPeer()
        {

            directory  = TestNodes.TemporaryDirectory("ssh-peer-device-model");

            var configuration = TestNodes.Offline;

            configuration["ssh"] = new JObject(
                                       new JProperty("peers", new JArray(
                                           new JObject(
                                               new JProperty("id",           "lc"),
                                               new JProperty("address",      "lc.example"),
                                               new JProperty("hostKeyPins",  new JArray(PinA))
                                           )
                                       ))
                                   );

            station    = Nodes.New(directory, configuration);
            model      = new SSHPeerDeviceModel(station.Node);

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


        #region ThePeersAreReadAsTheFileSaysThem()

        /// <summary>
        /// The peer of the file is an instance of SSHPeerController, with what
        /// its entry says and the usual where it says nothing; the instance
        /// without a name lists the peers and has the key the node signs in
        /// with at them.
        /// </summary>
        [Test]
        public void ThePeersAreReadAsTheFileSaysThem()
        {

            Assert.Multiple(() => {

                Assert.That(model.PeerIds,                                   Is.EqualTo(new[] { "lc" }));
                Assert.That(model.NodeValue("Peers"),                        Is.EqualTo("lc"));
                Assert.That(model.NodeValue("ClientPublicKey"),              Does.StartWith("ssh-ed25519 "));

                Assert.That(model.PeerValue("lc", "Address"),                Is.EqualTo("lc.example"));
                Assert.That(model.PeerValue("lc", "Port"),                   Is.EqualTo("22"));
                Assert.That(model.PeerValue("lc", "UserName"),               Is.EqualTo(""));
                Assert.That(model.PeerValue("lc", "HostKeyPins"),            Is.EqualTo(PinA));
                Assert.That(model.PeerValue("lc", "HostCAs"),                Is.EqualTo(""));
                Assert.That(model.PeerValue("lc", "OnPinMismatch"),          Is.EqualTo("Refuse"));
                Assert.That(model.PeerValue("lc", "HostKeyFingerprint"),     Is.EqualTo(""));
                Assert.That(model.PeerValue("lc", "Connected"),              Is.EqualTo("false"));
                Assert.That(model.PeerValue("lc", "Problem"),                Is.EqualTo("false"));
                Assert.That(model.PeerValue("no-one", "Address"),            Is.Null);

                Assert.That(ShownPeers(),                                    Is.EqualTo(new[] { "lc" }), "the instances the Device Model shows");
                Assert.That(station!.Node.SSHPeerTo(DomainName.Parse("lc.example")), Is.Not.Null, "the peer of the file is no SSH peer of the node");

            });

        }

        #endregion

        #region APeerIsAddedWithWhatItIsHeldTo()

        /// <summary>
        /// A peer is added by its name in Peers, with its Address and a host
        /// key to hold it to in the same request, as one: it is in the file, a
        /// peer of the node, and an instance of the Device Model.
        /// </summary>
        [Test]
        public async Task APeerIsAddedWithWhatItIsHeldTo()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null,   "Peers",          "lc,time"),
                                    ("time", "Address",        "10.10.0.7"),
                                    ("time", "Port",           "2222"),
                                    ("time", "UserName",       "cs001"),
                                    ("time", "HostKeyPins",    PinB),
                                    ("time", "OnPinMismatch",  "Record"));

            var onDisk  = (JArray) OnDisk()["ssh"]!["peers"]!;

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted),
                            String.Join(" | ", results.Select(result => result.AttributeStatusInfo?.AdditionalInfo)));

                Assert.That(model.NodeValue("Peers"),                        Is.EqualTo("lc,time"));
                Assert.That(model.PeerValue("time", "Address"),              Is.EqualTo("10.10.0.7"));
                Assert.That(model.PeerValue("time", "Port"),                 Is.EqualTo("2222"));
                Assert.That(model.PeerValue("time", "UserName"),             Is.EqualTo("cs001"));
                Assert.That(model.PeerValue("time", "HostKeyPins"),          Is.EqualTo(PinB));
                Assert.That(model.PeerValue("time", "OnPinMismatch"),        Is.EqualTo("Record"));
                Assert.That(model.PeerValue("lc",   "HostKeyPins"),          Is.EqualTo(PinA), "the peer that was there changed");

                Assert.That(JToken.DeepEquals(onDisk[1], JObject.Parse($$"""{ "id": "time", "address": "10.10.0.7", "user": "cs001", "port": 2222, "hostKeyPins": [ "{{PinB}}" ], "onMismatch": "record" }""")),
                            Is.True, onDisk.ToString());

                Assert.That(station!.Node.SSHPeerTo(DomainName.Parse("10.10.0.7"))?.Port, Is.EqualTo(2222));
                Assert.That(ShownPeers(),                                    Is.EqualTo(new[] { "lc", "time" }));

            });

        }

        #endregion

        #region APeerNobodyCouldBelieveIsNotAdded()

        /// <summary>
        /// A peer without a host key or a CA to hold it to is never stored: the
        /// request is refused as one, saying which peer, and nothing changes.
        /// So is one without an address.
        /// </summary>
        [Test]
        public async Task APeerNobodyCouldBelieveIsNotAdded()
        {

            var before     = OnDisk()["ssh"]!.ToString();

            var noPins     = await Set(OCPP21.DataConsistencyModel.ACID,
                                       (null,   "Peers",    "lc,time"),
                                       ("time", "Address",  "10.10.0.7"));

            var noAddress  = await Set(OCPP21.DataConsistencyModel.ACID,
                                       (null,   "Peers",        "lc,time"),
                                       ("time", "HostKeyPins",  PinB));

            var md5        = await Set(OCPP21.DataConsistencyModel.ACID,
                                       (null,   "Peers",        "lc,time"),
                                       ("time", "Address",      "10.10.0.7"),
                                       ("time", "HostKeyPins",  "ab:cd:ef"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(noPins),                              Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(noPins[0].AttributeStatusInfo?.AdditionalInfo,   Does.Contain("SSHPeerController[time]").And.Contain("held to no host key and no CA"));

                Assert.That(StatusesOf(noAddress),                           Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(noAddress[0].AttributeStatusInfo?.AdditionalInfo, Does.Contain("SSHPeerController[time].address"));

                Assert.That(StatusesOf(md5),                                 Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(md5[0].AttributeStatusInfo?.AdditionalInfo,      Does.Contain("no SHA-256 fingerprint"));

                Assert.That(OnDisk()["ssh"]!.ToString(),                     Is.EqualTo(before), "something was saved");
                Assert.That(model.NodeValue("Peers"),                        Is.EqualTo("lc"));
                Assert.That(ShownPeers(),                                    Is.EqualTo(new[] { "lc" }));

            });

        }

        #endregion

        #region APeerIsNotAddedEntryByEntry()

        /// <summary>
        /// Without ACID or BASE each entry is set by itself, and no single one
        /// says a peer: its name in Peers is refused, and so is a variable of a
        /// peer the node does not have.
        /// </summary>
        [Test]
        public async Task APeerIsNotAddedEntryByEntry()
        {

            var results = await Set(null,
                                    (null,   "Peers",        "lc,time"),
                                    ("time", "Address",      "10.10.0.7"),
                                    ("time", "HostKeyPins",  PinB));

            Assert.Multiple(() => {
                Assert.That(results[0].AttributeStatus,                      Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("ACID or BASE"));
                Assert.That(StatusesOf(results),                             Has.None.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.NodeValue("Peers"),                        Is.EqualTo("lc"));
            });

        }

        #endregion

        #region APeerIsChangedAndTakenOut()

        /// <summary>
        /// A variable of a peer the node has is set by itself: its port, its
        /// login, what its host key is held to. What only reads is refused. A
        /// peer that Peers leaves out is no peer any more, and no instance.
        /// </summary>
        [Test]
        public async Task APeerIsChangedAndTakenOut()
        {

            var was       = station!.Node.SSHPeerTo(DomainName.Parse("lc.example"));

            var changed   = await Set(null,
                                      ("lc", "Port",         "2222"),
                                      ("lc", "UserName",     "station-7"),
                                      ("lc", "HostCAs",      PinB),
                                      ("lc", "HostKeyPins",  ""));

            var port      = model.PeerValue("lc", "Port");
            var user      = model.PeerValue("lc", "UserName");
            var cas       = model.PeerValue("lc", "HostCAs");
            var pins      = model.PeerValue("lc", "HostKeyPins");
            var now       = station.Node.SSHPeerTo(DomainName.Parse("lc.example"));

            var readOnly  = await Set(null, ("lc", "HostKeyFingerprint", PinA));
            var nothing   = await Set(null, ("lc", "HostCAs", ""));
            var badPort   = await Set(null, ("lc", "Port", "0"));

            var takenOut  = await Set(null, (null, "Peers", ""));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(changed),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted),
                            String.Join(" | ", changed.Select(result => result.AttributeStatusInfo?.AdditionalInfo)));
                Assert.That((port, user, cas, pins),                         Is.EqualTo(("2222", "station-7", PinB, "")));
                Assert.That(now,                                             Is.Not.Null.And.Not.SameAs(was), "a peer that changed kept the connection of before");
                Assert.That(now?.Port,                                       Is.EqualTo(2222));

                Assert.That(StatusesOf(readOnly),                            Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(StatusesOf(nothing),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected), "a peer was left with nothing to hold it to");
                Assert.That(nothing[0].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("SSHPeerController[lc]").And.Contain("held to no host key and no CA"));
                Assert.That(StatusesOf(badPort),                             Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));

                Assert.That(StatusesOf(takenOut),                            Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.NodeValue("Peers"),                        Is.EqualTo(""));
                Assert.That(station.Node.SSHPeerTo(DomainName.Parse("lc.example")), Is.Null);
                Assert.That(ShownPeers(),                                    Is.Empty);
                Assert.That(((JArray) OnDisk()["ssh"]!["peers"]!).Count,     Is.Zero);

            });

        }

        #endregion


        #region (private) OnDisk(), ShownPeers(), Set(Model, params Entries), StatusesOf(Results)

        private JObject OnDisk()

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path));

        /// <summary>
        /// The peers the Device Model of the OCPP 2.1 node shows instances of.
        /// </summary>
        private String[] ShownPeers()

            => [.. station!.OCPPv2_1Node.GetComponentConfigs(SSHPeerDeviceModel.Component).
                            OfType<OCPP21.SSHPeerController>().
                            Where (controller => controller.Peer is not null).
                            Select(controller => controller.Peer!).
                            Order (StringComparer.Ordinal)];

        /// <summary>
        /// SetVariables of SSHPeerController as the node's OCPP 2.1 node
        /// answers it: (null, variable, value) for the instance without a name,
        /// (peer, ...) for a peer.
        /// </summary>
        private async Task<OCPP21.SetVariableResult[]> Set(OCPP21.DataConsistencyModel?                           Model,
                                                           params (String? Peer, String Variable, String Value)[]  Entries)
        {

            var node    = station!.OCPPv2_1Node;

            var request = new OCPP21.CSMS.SetVariablesRequest(
                              Destination:           SourceRouting.To(node.Id),
                              VariableData:          [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                                     Component:       new OCPP21.Component(Name:     SSHPeerDeviceModel.Component,
                                                                                                                           Instance: entry.Peer),
                                                                                     Variable:        new OCPP21.Variable(Name: entry.Variable),
                                                                                     AttributeValue:  entry.Value
                                                                                 ))],
                              DataConsistencyModel:  Model
                          );

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(request, (name, evse) => node.GetComponentConfigs(name, evse), default, node.NewInstanceOf)];

        }

        private static OCPP21.SetVariableStatus[] StatusesOf(IEnumerable<OCPP21.SetVariableResult> Results)

            => [.. Results.Select(result => result.AttributeStatus)];

        #endregion

        #region File servers, and who may write the peers

        private static readonly OCPP21.SetVariableStatus Accepted = OCPP21.SetVariableStatus.Accepted;
        private static readonly OCPP21.SetVariableStatus Rejected = OCPP21.SetVariableStatus.Rejected;

        private JObject? FileTransfersOnDisk(String Peer = "lc")
            => ((JArray) OnDisk()["ssh"]!["peers"]!).OfType<JObject>().
                   First(peer => peer.Value<String>("id") == Peer)["fileTransfers"] as JObject;

        private String? Shown(String Variable, String Peer = "lc")
            => station!.Bridge.SSHPeers.PeerValue(Peer, Variable);

        /// <summary>
        /// A peer becomes a file server by what its variables say, kept as the
        /// "fileTransfers" object of its entry; and the keys the node signs in
        /// with there are read from the same component.
        /// </summary>
        [Test]
        public async Task APeerIsAFileServerByWhatItsVariablesSay()
        {

            Assert.That(Shown("FileTransfers"),           Is.EqualTo(""),          "a peer is no file server by being a peer");
            Assert.That(Shown("FileTransferConnection"),  Is.EqualTo("Dedicated"));
            Assert.That(Shown("FileTransferPublicKeys"),  Is.EqualTo(""));
            Assert.That(station!.Node.FileServers,        Is.Empty);

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("lc", "FileTransfers",           "Upload,Download"),
                                    ("lc", "FileTransferUsers",       "cs001,cs001-fw"),
                                    ("lc", "UploadPaths",             "/upload/{id}"),
                                    ("lc", "DownloadPaths",           "/firmware/"),
                                    ("lc", "FileTransferConnection",  "Shared"));

            Assert.That(StatusesOf(results), Is.All.EqualTo(Accepted),
                        String.Join(" | ", results.Select(result => result.AttributeStatusInfo?.AdditionalInfo)));

            Assert.That(FileTransfersOnDisk()!.ToString(Newtonsoft.Json.Formatting.None),
                        Is.EqualTo("""{"directions":["upload","download"],"users":["cs001","cs001-fw"],"uploadPaths":["/upload/{id}/"],"downloadPaths":["/firmware/"],"connection":"shared"}"""));

            Assert.That(Shown("FileTransfers"),           Is.EqualTo("Upload,Download"));
            Assert.That(Shown("FileTransferUsers"),       Is.EqualTo("cs001,cs001-fw"));
            Assert.That(Shown("UploadPaths"),             Is.EqualTo("/upload/{id}/"));
            Assert.That(Shown("DownloadPaths"),           Is.EqualTo("/firmware/"));
            Assert.That(Shown("FileTransferConnection"),  Is.EqualTo("Shared"));

            Assert.That(station.Node.FileServers.Select(fileServer => fileServer.Id), Is.EqualTo(new[] { "lc" }));

            // A key per login, made by the node: the list a CSMS enters at the file server.
            var keys = Shown("FileTransferPublicKeys")!.Split(',');

            Assert.That(keys,                             Has.Length.EqualTo(2));
            Assert.That(keys,                             Is.All.StartsWith("ssh-ed25519 "));
            Assert.That(keys[0],                          Does.EndWith(" cs001@lc"));
            Assert.That(keys[1],                          Does.EndWith(" cs001-fw@lc"));
            Assert.That(keys[0].Split(' ')[1],            Is.Not.EqualTo(keys[1].Split(' ')[1]));

            Assert.That(Shown("FileTransferKeyStates"),   Is.EqualTo("cs001=Unconfirmed,cs001-fw=Unconfirmed"));

            Assert.That(station.Node.ConfirmFileTransferKey("lc", "cs001", keys[0]), Is.Null);

            Assert.That(Shown("FileTransferKeyStates"),   Is.EqualTo("cs001=Confirmed,cs001-fw=Unconfirmed"));

        }

        /// <summary>
        /// What a peer may be used for is carried through every change of the
        /// peers - of this one and of another - and is gone only when a request
        /// empties it.
        /// </summary>
        [Test]
        public async Task WhatAPeerMayBeUsedForOutlivesEveryOtherChange()
        {

            Assert.That(StatusesOf(await Set(OCPP21.DataConsistencyModel.ACID,
                                             ("lc", "FileTransfers",  "Upload"),
                                             ("lc", "UploadPaths",    "/upload/"))),
                        Is.All.EqualTo(Accepted));

            // Another variable of the same peer.
            Assert.That(StatusesOf(await Set(null, ("lc", "HostKeyPins", $"{PinA},{PinB}"))), Is.All.EqualTo(Accepted));
            Assert.That(FileTransfersOnDisk()?["uploadPaths"]?.Values<String>(), Is.EqualTo(new[] { "/upload/" }));

            // Another peer added.
            Assert.That(StatusesOf(await Set(OCPP21.DataConsistencyModel.ACID,
                                             (null,   "Peers",        "lc,time"),
                                             ("time", "Address",      "time.example"),
                                             ("time", "HostKeyPins",  PinB))),
                        Is.All.EqualTo(Accepted));

            Assert.That(FileTransfersOnDisk()?["directions"]?.Values<String>(), Is.EqualTo(new[] { "upload" }));
            Assert.That(FileTransfersOnDisk("time"),                             Is.Null);
            Assert.That(station!.Node.FileServers.Select(fileServer => fileServer.Id), Is.EqualTo(new[] { "lc" }));

            // Emptied, it is no file server, and its entry says nothing of it.
            Assert.That(StatusesOf(await Set(OCPP21.DataConsistencyModel.ACID,
                                             ("lc", "FileTransfers",  ""),
                                             ("lc", "UploadPaths",    ""))),
                        Is.All.EqualTo(Accepted));

            Assert.That(FileTransfersOnDisk(),     Is.Null);
            Assert.That(station.Node.FileServers,  Is.Empty);

        }

        /// <summary>
        /// What a peer may be used for is checked as its entry is read.
        /// </summary>
        [Test]
        public async Task WhatAFileServerCannotBeIsRefused()
        {

            foreach (var (variable, value) in new[] {
                         ("FileTransfers",           "Sideways"),
                         ("UploadPaths",             "upload/"),
                         ("DownloadPaths",           "/firmware/../etc/"),
                         ("FileTransferUsers",       "-rf"),
                         ("FileTransferConnection",  "Both"),
                         ("FileTransferPublicKeys",  "ssh-ed25519 AAAA mine"),
                         ("FileTransferKeyStates",   "cs001=Confirmed")
                     })
            {

                var results = await Set(null, ("lc", variable, value));

                Assert.That(StatusesOf(results), Is.All.EqualTo(Rejected), $"{variable} = '{value}'");

            }

            Assert.That(FileTransfersOnDisk(), Is.Null);

        }

        /// <summary>
        /// The peers say who a host is believed by and where the node's files
        /// may go: over OCPP they are written by a request a signer of pin sets
        /// signed, and by no other - neither a peer nor what it may be used for.
        /// </summary>
        [Test]
        public async Task OnlyASignedRequestWritesThePeers()
        {

            var before = OnDisk().ToString();

            (String? Peer, String Variable, String Value)[][] writes = [
                [ ("lc", "HostKeyPins", PinB) ],
                [ ("lc", "Address", "elsewhere.example") ],
                [ ("lc", "FileTransfers", "Upload"), ("lc", "UploadPaths", "/upload/") ],
                [ (null, "Peers", "") ],
                [ (null, "Peers", "lc,time"), ("time", "Address", "time.example"), ("time", "HostKeyPins", PinB) ]
            ];

            // A node that knows no signer writes nothing over OCPP: it is not the way around one.
            station!.Bridge.SSHPeers.SignedBy = null;

            foreach (var write in writes)
            {

                var results = await Set(OCPP21.DataConsistencyModel.ACID, write);

                Assert.That(StatusesOf(results),                                               Is.All.EqualTo(Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode),  Is.All.EqualTo("Unsigned"));

            }

            // A request no signer the node believes signed.
            OCPP21.CSMS.SetVariablesRequest? asked = null;

            station.Bridge.SSHPeers.SignedBy = (OCPP21.CSMS.SetVariablesRequest? request, out String? signer, out String? why) => {
                asked   = request;
                signer  = null;
                why     = "None of the request's signatures is a valid one of a signer this node believes.";
                return false;
            };

            foreach (var write in writes)
            {

                var results = await Set(null, write);

                Assert.That(StatusesOf(results),                                                   Is.All.EqualTo(Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode),      Is.All.EqualTo("Unsigned"));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.AdditionalInfo),  Is.All.Contains("None of the request's signatures"));

            }

            Assert.That(asked,                Is.Not.Null);
            Assert.That(OnDisk().ToString(),  Is.EqualTo(before), "nothing was written");

            // Reading is nobody's secret.
            Assert.That(Shown("HostKeyPins"), Is.EqualTo(PinA));

            // Signed, it is written.
            station.Bridge.SSHPeers.SignedBy = (OCPP21.CSMS.SetVariablesRequest? request, out String? signer, out String? why) => {
                signer  = "the security officer";
                why     = null;
                return true;
            };

            Assert.That(StatusesOf(await Set(null, ("lc", "HostKeyPins", PinB))), Is.All.EqualTo(Accepted));
            Assert.That(Shown("HostKeyPins"), Is.EqualTo(PinB));

        }

        #endregion

    }

}
