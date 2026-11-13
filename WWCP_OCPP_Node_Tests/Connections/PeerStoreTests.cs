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

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The connections made peers (work package P6 of EVQI Transport, section
    /// 11.7.1): a peers.json with the endpoints in it, made once of
    /// connections.json, and nothing dialled differently.
    /// </summary>
    [TestFixture]
    public class PeerStoreTests
    {

        #region Data

        private String                    directory  = "";
        private String                    path       = "";
        private MovableClock   clock      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeADirectory()
        {
            directory  = TestNodes.TemporaryDirectory("peers");
            path       = Path.Combine(directory, "conn");
            clock      = new MovableClock(DateTimeOffset.Parse("2026-11-11T12:00:00Z"));
            Directory.CreateDirectory(path);
        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion

        #region (private) Helpers

        /// <summary>
        /// A connections.json as a station before the peers wrote it.
        /// </summary>
        private void AnOldFile(params String[] Connections)

            => File.WriteAllText(Path.Combine(path, ConnectionStore.ConnectionsFileName),
                                 "[" + String.Join(",", Connections) + "]");

        private static String Old(String Id, String Type, String Version, Boolean AutoConnect, Int32 Minute, String? Url = null)

            => new JObject(
                   new JProperty("id",              Id),
                   new JProperty("description",     $"The {Id}"),
                   new JProperty("url",             Url ?? $"wss://{Id}.example.org/ocpp/CS001"),
                   new JProperty("connectionType",  Type),
                   new JProperty("ocppVersion",     Version),
                   new JProperty("autoConnect",     AutoConnect),
                   new JProperty("createdAt",       $"2026-11-0{1 + Minute / 60}T10:{Minute % 60:00}:00.000Z")
               ).ToString();

        private ConnectionStore AStore(List<String>? Notices = null)
        {
            var store = new ConnectionStore(path, clock);
            if (Notices is not null)
                store.OnNotice += (level, message) => Notices.Add(message);
            return store;
        }

        private static PeerEntry PeerOf(ConnectionStore Store, String ConnectionId)
            => Store.PeerOf(ConnectionId) ?? throw new AssertionException($"'{ConnectionId}' is the endpoint of no peer.");

        /// <summary>
        /// The connections as the dialling sees them: everything of each but
        /// its warnings, which are worked out when asked.
        /// </summary>
        private static Dictionary<String, String> AsDialled(IEnumerable<ConnectionEntry> Connections)

            => Connections.ToDictionary(entry => entry.Id,
                                        entry => { var json = entry.ToJSON(); json.Remove("warnings"); return json.ToString(Newtonsoft.Json.Formatting.None); });

        #endregion


        #region ANewStationHasNoPeersAndWritesNoFile()

        [Test]
        public void ANewStationHasNoPeersAndWritesNoFile()
        {

            var store = AStore();

            Assert.Multiple(() => {
                Assert.That(store.Peers,       Is.Empty);
                Assert.That(store.Migration,   Is.Null);
                Assert.That(File.Exists(Path.Combine(path, ConnectionStore.PeersFileName)), Is.False, "nothing to make peers of");
            });

        }

        #endregion

        #region EveryOldConnectionIsAnEndpointUnderItsOwnIdAndDialledAsBefore()

        /// <summary>
        /// The acceptance of P6: every existing connections.json loads, and
        /// nothing is dialled differently - every connection is there under its
        /// id, exactly as it was, and connections.json is left as it was.
        /// </summary>
        [Test]
        public void EveryOldConnectionIsAnEndpointUnderItsOwnIdAndDialledAsBefore()
        {

            AnOldFile(Old("main",   "CSMS",            "OCPP2.1", true,  1),
                      Old("spare",  "CSMSBackup",      "OCPP2.1", false, 2),
                      Old("lc",     "LocalController", "OCPP1.6", true,  3),
                      Old("test",   "CSMS",            "OCPP1.6", false, 4),
                      Old("ssh",    "CSMS",            "OCPP2.1", true,  5, "ssh://cs001@csms.example.org:2222"));

            var oldFile   = File.ReadAllText(Path.Combine(path, ConnectionStore.ConnectionsFileName));
            var expected  = AsDialled(JArray.Parse(oldFile).OfType<JObject>().Select(json => {
                                ConnectionEntry.TryParse(json, out var entry, out _);
                                return entry!;
                            }));

            var store     = AStore();

            Assert.Multiple(() => {
                Assert.That(AsDialled(store.Connections),  Is.EquivalentTo(expected));
                Assert.That(store.Connections.Where(entry => entry.AutoConnect).Select(entry => entry.Id), Is.EquivalentTo(new[] { "main", "lc", "ssh" }));
                Assert.That(File.ReadAllText(Path.Combine(path, ConnectionStore.ConnectionsFileName)), Is.EqualTo(oldFile), "connections.json is left as it was");
                Assert.That(File.Exists(Path.Combine(path, ConnectionStore.PeersFileName)), Is.True);
            });

            // And so after it was read back from peers.json alone.
            File.Delete(Path.Combine(path, ConnectionStore.ConnectionsFileName));

            Assert.That(AsDialled(AStore().Connections), Is.EquivalentTo(expected), "read back from peers.json");

        }

        #endregion

        #region ThePeersAreMadeAsSection11_7_1Says()

        /// <summary>
        /// The oldest connection that dials by itself makes its peer the
        /// primary, the oldest of the other OCPP version a successor; every
        /// other one dialled by itself an observer, one that is not a service;
        /// a CSMSBackup the next endpoint of the one CSMS of its version.
        /// </summary>
        [Test]
        public void ThePeersAreMadeAsSection11_7_1Says()
        {

            AnOldFile(Old("main",    "CSMS",            "OCPP2.1", true,  1),
                      Old("spare",   "CSMSBackup",      "OCPP2.1", false, 2),
                      Old("lc",      "LocalController", "OCPP1.6", true,  3),
                      Old("second",  "LocalController", "OCPP2.1", true,  4),
                      Old("test",    "CSMS",            "OCPP1.6", false, 5));

            var store = AStore();

            Assert.Multiple(() => {

                Assert.That(store.Peers.Select(peer => peer.Id),  Is.EqualTo(new[] { "main", "lc", "second", "test" }));

                Assert.That(PeerOf(store, "main").Role,           Is.EqualTo(PeerRole.Primary));
                Assert.That(PeerOf(store, "main").EndpointIds,    Is.EqualTo(new[] { "main", "spare" }), "the backup is the CSMS's next endpoint");
                Assert.That(PeerOf(store, "spare").Id,            Is.EqualTo("main"));
                Assert.That(PeerOf(store, "main").Kind,           Is.EqualTo(PeerKind.CSMS));

                Assert.That(PeerOf(store, "lc").Role,             Is.EqualTo(PeerRole.Successor), "in charge of the other OCPP node until now");
                Assert.That(PeerOf(store, "lc").Kind,             Is.EqualTo(PeerKind.LocalController));
                Assert.That(PeerOf(store, "lc").OCPPVersion,      Is.EqualTo(OCPPVersion.OCPP1_6));

                Assert.That(PeerOf(store, "second").Role,         Is.EqualTo(PeerRole.Observer));
                Assert.That(PeerOf(store, "test").Role,           Is.EqualTo(PeerRole.Service), "dialled by hand only");

            });

        }

        #endregion

        #region ABackupWithoutOrWithTwoCSMSIsAPeerOfItsOwn([Values(0, 2)] CSMS)

        [Test]
        public void ABackupWithoutOrWithTwoCSMSIsAPeerOfItsOwn([Values(0, 2)] Int32 CSMS)
        {

            var connections = new List<String> { Old("spare", "CSMSBackup", "OCPP2.1", true, 9) };

            for (var i = 1; i <= CSMS; i++)
                connections.Add(Old($"csms{i}", "CSMS", "OCPP2.1", true, i));

            AnOldFile([.. connections]);

            var store = AStore();

            Assert.Multiple(() => {
                Assert.That(PeerOf(store, "spare").Id,           Is.EqualTo("spare"));
                Assert.That(PeerOf(store, "spare").EndpointIds,  Is.EqualTo(new[] { "spare" }));
                Assert.That(store.Migration!.Notes,              Has.Some.Contains("a peer of its own"));
                Assert.That(store.Peers.Count(peer => peer.Role == PeerRole.Primary), Is.EqualTo(1));
            });

        }

        #endregion

        #region TheMigrationIsToldOnceAndKept()

        [Test]
        public void TheMigrationIsToldOnceAndKept()
        {

            AnOldFile(Old("main", "CSMS", "OCPP2.1", true, 1));

            var notices = new List<String>();
            var store   = AStore(notices);

            Assert.Multiple(() => {
                Assert.That(notices,                Has.Some.Contains("nothing is dialled differently"), "told to who listens after the store was made");
                Assert.That(store.Migration,        Is.Not.Null);
                Assert.That(store.ToJSON()["migration"]?["notes"], Is.Not.Null.And.Not.Empty);
            });

            var again    = new List<String>();
            var reloaded = AStore(again);

            Assert.Multiple(() => {
                Assert.That(again,                         Has.None.Contains("nothing is dialled differently"), "made once");
                Assert.That(reloaded.Migration?.Notes,     Is.EqualTo(store.Migration!.Notes), "kept in peers.json");
            });

        }

        #endregion

        #region ConnectionsJsonIsNotReadAgainOncePeersJsonIsThere()

        [Test]
        public void ConnectionsJsonIsNotReadAgainOncePeersJsonIsThere()
        {

            AnOldFile(Old("main", "CSMS", "OCPP2.1", true, 1));
            AStore();

            // An older station, started once more, wrote a connection down.
            AnOldFile(Old("main", "CSMS", "OCPP2.1", true, 1),
                      Old("later", "CSMS", "OCPP2.1", true, 2));

            Assert.That(AStore().Connections.Select(entry => entry.Id), Is.EqualTo(new[] { "main" }));

        }

        #endregion

        #region AnUnreadablePeersFileIsIgnoredAndTheOldOneNotTakenInstead()

        [Test]
        public void AnUnreadablePeersFileIsIgnoredAndTheOldOneNotTakenInstead()
        {

            AnOldFile(Old("main", "CSMS", "OCPP2.1", true, 1));
            File.WriteAllText(Path.Combine(path, ConnectionStore.PeersFileName), "{ this is not JSON");

            var notices  = new List<String>();
            var store    = AStore(notices);

            Assert.Multiple(() => {
                Assert.That(store.Connections,  Is.Empty, "nothing nobody wrote down any more");
                Assert.That(notices,            Has.Some.Contains("could not be read"));
            });

        }

        #endregion

        #region AHandEditedFileIsPutRightAndTold()

        /// <summary>
        /// Two primaries, a service whose endpoint dials by itself, rights on
        /// the primary: put right where read, and told, and not written back.
        /// </summary>
        [Test]
        public void AHandEditedFileIsPutRightAndTold()
        {

            var file = Path.Combine(path, ConnectionStore.PeersFileName);

            File.WriteAllText(file, """
                { "peers": [
                  { "id": "a", "description": "A", "kind": "CSMS", "role": "Primary", "ocppVersion": "OCPP2.1",
                    "rights": { "outbound": [ "MeterValues" ], "inbound": [] },
                    "createdAt": "2026-11-01T10:00:00.000Z",
                    "endpoints": [ { "id": "a1", "description": "A", "url": "wss://a.example.org/", "connectionType": "CSMS",
                                     "autoConnect": true, "createdAt": "2026-11-01T10:00:00.000Z" } ] },
                  { "id": "b", "description": "B", "kind": "CSMS", "role": "Primary", "ocppVersion": "OCPP2.1",
                    "createdAt": "2026-11-01T11:00:00.000Z",
                    "endpoints": [ { "id": "b1", "description": "B", "url": "wss://b.example.org/", "connectionType": "CSMS",
                                     "autoConnect": true, "createdAt": "2026-11-01T11:00:00.000Z" } ] },
                  { "id": "c", "description": "C", "kind": "EnergyManager", "role": "Service", "ocppVersion": "OCPP2.1",
                    "createdAt": "2026-11-01T12:00:00.000Z",
                    "endpoints": [ { "id": "c1", "description": "C", "url": "ws://c.example.org/", "connectionType": "CSMS",
                                     "autoConnect": true, "createdAt": "2026-11-01T12:00:00.000Z" } ] }
                ] }
                """);

            var written  = File.ReadAllText(file);
            var notices  = new List<String>();
            var store    = AStore(notices);

            Assert.Multiple(() => {
                Assert.That(PeerOf(store, "a1").Role,    Is.EqualTo(PeerRole.Primary));
                Assert.That(PeerOf(store, "a1").Rights,  Is.Null, "a primary has every right");
                Assert.That(PeerOf(store, "b1").Role,    Is.EqualTo(PeerRole.Successor));
                Assert.That(PeerOf(store, "c1").Role,    Is.EqualTo(PeerRole.Observer));
                Assert.That(PeerOf(store, "c1").Kind,    Is.EqualTo(PeerKind.EnergyManager));
                Assert.That(notices,                     Has.Some.Contains("second primary"));
                Assert.That(File.ReadAllText(file),      Is.EqualTo(written), "not written back");
            });

        }

        #endregion

        #region AConnectionWrittenDownIsAPeerOfItsOwn()

        [Test]
        public void AConnectionWrittenDownIsAPeerOfItsOwn()
        {

            var store = AStore();

            Assert.That(store.TryAddConnection("First",  "wss://one.example.org/",   "CSMS",            true,  null, null, out var first,  out var error), Is.True, error);
            Assert.That(store.TryAddConnection("Second", "wss://two.example.org/",   "LocalController", true,  null, null, out var second, out error),     Is.True, error);
            Assert.That(store.TryAddConnection("Spare",  "wss://three.example.org/", "CSMSBackup",      false, null, null, out var spare,  out error),     Is.True, error);

            var reloaded = AStore();

            Assert.Multiple(() => {
                Assert.That(PeerOf(reloaded, first).Role,    Is.EqualTo(PeerRole.Primary));
                Assert.That(PeerOf(reloaded, second).Role,   Is.EqualTo(PeerRole.Observer));
                Assert.That(PeerOf(reloaded, second).Kind,   Is.EqualTo(PeerKind.LocalController));
                Assert.That(PeerOf(reloaded, spare).Role,    Is.EqualTo(PeerRole.Service));
                Assert.That(reloaded.Peers,                  Has.Count.EqualTo(3));
                Assert.That(File.Exists(Path.Combine(path, ConnectionStore.ConnectionsFileName)), Is.False, "a new station writes peers.json only");
            });

        }

        #endregion

        #region ThePrimaryGoneTheOldestDialledTakesOver()

        [Test]
        public void ThePrimaryGoneTheOldestDialledTakesOver()
        {

            var store = AStore();

            store.TryAddConnection("First",  "wss://one.example.org/", "CSMS", true, null, null, out var first,  out _);
            clock.Now = clock.Now.AddMinutes(1);
            store.TryAddConnection("Second", "wss://two.example.org/", "CSMS", true, null, null, out var second, out _);

            var notices = new List<String>();
            store.OnNotice += (level, message) => notices.Add(message);

            Assert.That(store.TryRemoveConnection(first, out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(PeerOf(store, second).Role,  Is.EqualTo(PeerRole.Primary));
                Assert.That(notices,                     Has.Some.StartsWith("Switchover:"));
                Assert.That(store.Peers,                 Has.Count.EqualTo(1));
            });

        }

        #endregion

        #region WhetherItDialsByItselfMakesAPeerAServiceOrNot()

        [Test]
        public void WhetherItDialsByItselfMakesAPeerAServiceOrNot()
        {

            var store = AStore();

            store.TryAddConnection("First",  "wss://one.example.org/", "CSMS", true, null, null, out var first,  out _);
            clock.Now = clock.Now.AddMinutes(1);
            store.TryAddConnection("Second", "wss://two.example.org/", "CSMS", true, null, null, out var second, out _);

            Assert.That(store.TryUpdateConnection(first, "First", "wss://one.example.org/", "CSMS", false, null, null, out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(PeerOf(store, first).Role,   Is.EqualTo(PeerRole.Service));
                Assert.That(PeerOf(store, second).Role,  Is.EqualTo(PeerRole.Primary), "the oldest still dialled");
            });

            Assert.That(store.TryUpdateConnection(first, "First", "wss://one.example.org/", "CSMS", true, null, null, out error), Is.True, error);

            Assert.That(PeerOf(store, first).Role, Is.EqualTo(PeerRole.Observer), "dialled again, and the primary is another now");

        }

        #endregion

        #region APeerOfOneEndpointFollowsItsVersionAndKind()

        [Test]
        public void APeerOfOneEndpointFollowsItsVersionAndKind()
        {

            var store = AStore();

            store.TryAddConnection("First", "wss://one.example.org/", "CSMS", true, null, null, out var first, out _, OCPPVersion: "OCPP2.1");

            Assert.That(store.TryUpdateConnection(first, "Renamed", "wss://one.example.org/", "LocalController", true, null, null, out var error, OCPPVersion: "OCPP1.6"),
                        Is.True, error);

            var peer = PeerOf(AStore(), first);

            Assert.Multiple(() => {
                Assert.That(peer.OCPPVersion,  Is.EqualTo(OCPPVersion.OCPP1_6));
                Assert.That(peer.Kind,         Is.EqualTo(PeerKind.LocalController));
                Assert.That(peer.Description,  Is.EqualTo("Renamed"));
            });

        }

        #endregion

        #region EveryEndpointOfAPeerSpeaksOneVersion()

        [Test]
        public void EveryEndpointOfAPeerSpeaksOneVersion()
        {

            var store = AStore();

            store.TryAddConnection("Main",  "wss://one.example.org/",   "CSMS",       true,  null, null, out var main,  out _, OCPPVersion: "OCPP2.1");
            store.TryAddConnection("Spare", "wss://two.example.org/",   "CSMSBackup", false, null, null, out var spare, out _, OCPPVersion: "OCPP2.1");
            store.TryAddConnection("Old",   "wss://three.example.org/", "CSMS",       false, null, null, out var old,   out _, OCPPVersion: "OCPP1.6");

            Assert.That(store.TryMoveEndpoint(spare, PeerOf(store, main).Id, null, out var error), Is.True, error);
            Assert.That(store.TryMoveEndpoint(old,   PeerOf(store, main).Id, null, out error),     Is.False);

            Assert.That(error, Does.Contain("one OCPP version"));

            Assert.That(store.TryUpdateConnection(spare, "Spare", "wss://two.example.org/", "CSMSBackup", false, null, null, out error, OCPPVersion: "OCPP1.6"),
                        Is.False);

            Assert.That(error, Does.Contain("one OCPP version"));

        }

        #endregion

        #region AnEndpointMovedIsDialledAsBefore()

        [Test]
        public void AnEndpointMovedIsDialledAsBefore()
        {

            var store = AStore();

            store.TryAddConnection("Main",  "wss://one.example.org/", "CSMS",       true, null, null, out var main,  out _);
            store.TryAddConnection("Spare", "wss://two.example.org/", "CSMSBackup", true, null, null, out var spare, out _);

            var before = AsDialled(store.Connections);

            Assert.That(store.TryMoveEndpoint(spare, PeerOf(store, main).Id, 0, out var error), Is.True, error);

            var reloaded = AStore();

            Assert.Multiple(() => {
                Assert.That(PeerOf(reloaded, main).EndpointIds,  Is.EqualTo(new[] { spare, main }), "at the place asked for");
                Assert.That(reloaded.Peers,                      Has.Count.EqualTo(1), "the spare's own peer is gone");
                Assert.That(AsDialled(reloaded.Connections),     Is.EquivalentTo(before));
            });

        }

        #endregion

        #region APeerIsChangedAndTheRulesHold()

        [Test]
        public void APeerIsChangedAndTheRulesHold()
        {

            var store = AStore();

            store.TryAddConnection("Old CSMS",   "wss://old.example.org/",   "CSMS", true,  null, null, out var old,  out _);
            store.TryAddConnection("New CSMS",   "wss://new.example.org/",   "CSMS", true,  null, null, out var @new, out _);
            store.TryAddConnection("Diagnosis",  "wss://diag.example.org/",  "CSMS", false, null, null, out var diag, out _);

            var oldPeer   = PeerOf(store, old). Id;
            var newPeer   = PeerOf(store, @new).Id;
            var diagPeer  = PeerOf(store, diag).Id;

            Assert.Multiple(() => {

                Assert.That(store.TryUpdatePeer(oldPeer,  null, null, "Observer", null, null, out var error), Is.False, "the primary is not demoted directly");
                Assert.That(error, Does.Contain("make that one the primary"));

                Assert.That(store.TryUpdatePeer(diagPeer, null, null, "Observer", null, null, out error), Is.False, "dialled by hand only");
                Assert.That(error, Does.Contain("dials by itself"));

                Assert.That(store.TryUpdatePeer(oldPeer,  null, null, null, null, JObject.Parse("""{ "outbound": [ "MeterValues" ] }"""), out error), Is.False);
                Assert.That(error, Does.Contain("every right"));

                Assert.That(store.TryUpdatePeer(newPeer,  "The new operator", "CSMS", "Successor", "DE*NEW*E000123",
                                                JObject.Parse("""{ "outbound": [ "MeterValues", "StatusNotification" ], "inbound": [ "GetVariables" ] }"""),
                                                out error), Is.True, error);

            });

            var notices = new List<String>();
            store.OnNotice += (level, message) => notices.Add(message);

            Assert.That(store.TryUpdatePeer(newPeer, null, null, "Primary", null, JValue.CreateNull(), out var switchover), Is.True, switchover);

            var reloaded = AStore();

            Assert.Multiple(() => {
                Assert.That(reloaded.Peers.Single(peer => peer.Id == newPeer).Role,       Is.EqualTo(PeerRole.Primary));
                Assert.That(reloaded.Peers.Single(peer => peer.Id == newPeer).Identity,   Is.EqualTo("DE*NEW*E000123"));
                Assert.That(reloaded.Peers.Single(peer => peer.Id == newPeer).Rights,     Is.Null);
                Assert.That(reloaded.Peers.Single(peer => peer.Id == oldPeer).Role,       Is.EqualTo(PeerRole.Successor), "the one that was in charge");
                Assert.That(notices,                                                      Has.Some.StartsWith("Switchover:"));
            });

        }

        #endregion

        #region ASecondarysRightsAreKept()

        [Test]
        public void ASecondarysRightsAreKept()
        {

            var store = AStore();

            store.TryAddConnection("Main", "wss://one.example.org/",  "CSMS", true, null, null, out _,       out _);
            store.TryAddConnection("EMS",  "ws://ems.example.org/",   "CSMS", true, null, null, out var ems, out _);

            Assert.That(store.TryUpdatePeer(PeerOf(store, ems).Id, null, "EnergyManager", "EnergyManager", null,
                                            JObject.Parse("""{ "outbound": [ "MeterValues" ], "inbound": [ "SetChargingProfile:ChargingStationExternalConstraints", "GetCompositeSchedule" ] }"""),
                                            out var error), Is.True, error);

            var peer = PeerOf(AStore(), ems);

            Assert.Multiple(() => {
                Assert.That(peer.Role,             Is.EqualTo(PeerRole.EnergyManager));
                Assert.That(peer.Rights!.Outbound, Is.EqualTo(new[] { "MeterValues" }));
                Assert.That(peer.Rights!.Inbound,  Is.EqualTo(new[] { "SetChargingProfile:ChargingStationExternalConstraints", "GetCompositeSchedule" }));
            });

            Assert.That(store.TryUpdatePeer(PeerOf(store, ems).Id, null, null, null, null, JObject.Parse("""{ "inbound": [ "two words" ] }"""), out error), Is.False);

        }

        #endregion

        #region WhatTheConnectionsLearnIsWrittenToThePeers()

        /// <summary>
        /// The ways a connection is changed that know nothing of peers - a host
        /// key learned on first use - write peers.json too.
        /// </summary>
        [Test]
        public void WhatTheConnectionsLearnIsWrittenToThePeers()
        {

            var store = AStore();

            Assert.That(store.TryAddConnection("Over SSH", "ssh://cs001@csms.example.org:2222", "CSMS", true, null, null, out var id, out var error,
                                               SshTrust: new SshTrustSettings("HostKey", null, null)), Is.True, error);

            var key = "SHA256:" + Convert.ToBase64String(new Byte[32]).TrimEnd('=');

            Assert.That(store.TryLearnHostKey(id, store.Connections.Single().URL, key, out error), Is.True, error);

            Assert.That(AStore().Connections.Single().SshLearnedHostKey, Is.EqualTo(key));

        }

        #endregion

        #region TheWebInterfaceIsToldOfThePeers()

        [Test]
        public void TheWebInterfaceIsToldOfThePeers()
        {

            AnOldFile(Old("main", "CSMS", "OCPP2.1", true, 1), Old("spare", "CSMSBackup", "OCPP2.1", false, 2));

            var json = AStore().ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["peers"]![0]!["role"]!.Value<String>(),       Is.EqualTo("Primary"));
                Assert.That(json["peers"]![0]!["endpoints"]!.Values<String>(), Is.EqualTo(new[] { "main", "spare" }));
                Assert.That(json["peerRoles"]!.Values<String>(),               Does.Contain("Successor"));
                Assert.That(json["peerKinds"]!.Values<String>(),               Does.Contain("EnergyManager"));
                Assert.That(json["connections"]!.Count(),                      Is.EqualTo(2), "the connections as before");
            });

        }

        #endregion

    }

}
