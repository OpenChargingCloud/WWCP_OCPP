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
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The identities a connection proves itself with - several, each from a
    /// time on - and the SSH keys a node signs in with beside its own.
    /// </summary>
    /// <remarks>
    /// What is measured is the one thing several identities are for: that the
    /// key which follows another is written down before its time, is not
    /// shown before it, and is shown from it - without anybody touching the
    /// connection at the moment its server changes over.
    /// </remarks>
    [TestFixture]
    public class ConnectionIdentityTests
    {

        #region Data

        private const String  AWssURL   = "wss://csms.example.org/cs001";
        private const String  AnSshURL  = "ssh://cs001@csms.example.org:2222";
        private const String  AHostKey  = "SHA256:nThbg6kXUpJWGl7E1IGOCspRomTxdCARLviKw6E5SY8";

        private static readonly DateTimeOffset Start = new (2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

        private String           directory  = "";
        private ConnectionStore  store      = default!;
        private MovableClock     clock      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {

            directory  = TestNodes.TemporaryDirectory("identities");
            clock      = new MovableClock(Start);
            store      = new ConnectionStore(Path.Combine(directory, "conn"), clock) {
                             KnownCertificateIds  = () => [ "aaaa", "bbbb", "cccc" ],
                             KnownSshKeyIds       = () => [ SshClientKeys.OwnId, "11112222" ]
                         };

        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion


        #region TheOneWhoseTimeCameLastGoesFirst()

        /// <summary>
        /// Of the identities whose time has come the one that came last goes
        /// first, and of those that say no time the one written first; one
        /// whose time has not come is not among them.
        /// </summary>
        [Test]
        public void TheOneWhoseTimeCameLastGoesFirst()
        {

            ConnectionIdentity[] written = [ new ("old"),
                                             new ("spare"),
                                             new ("next",  Start.AddDays(10)),
                                             new ("later", Start.AddDays(20)) ];

            String[] At(DateTimeOffset Now)
                => [ .. ConnectionIdentity.InOrder(written, Now).Select(identity => identity.Id) ];

            Assert.Multiple(() => {
                Assert.That(At(Start),                Is.EqualTo(new[] { "old", "spare" }));
                Assert.That(At(Start.AddDays(10)),    Is.EqualTo(new[] { "next", "old", "spare" }),          "the one that follows is not shown from its time on");
                Assert.That(At(Start.AddDays(30)),    Is.EqualTo(new[] { "later", "next", "old", "spare" }));
                Assert.That(ConnectionIdentity.InOrder([ new ("next", Start.AddDays(1)) ], Start), Is.Empty, "an identity is shown before its time");
            });

        }

        #endregion

        #region TheyAreReadAsAFormAndAFileWriteThem()

        [Test]
        public void TheyAreReadAsAFormAndAFileWriteThem()
        {

            Assert.Multiple(() => {

                Assert.That(ConnectionIdentity.TryParse(null, out var none, out _),                      Is.True);
                Assert.That(none,                                                                         Is.Empty);

                Assert.That(ConnectionIdentity.TryParse(JArray.Parse("""[ "aaaa", { "id": " bbbb ", "from": "2027-01-31T00:00:00Z" } ]"""),
                                                        out var two, out var error),                      Is.True, error);
                Assert.That(two.Select(identity => identity.Id),                                          Is.EqualTo(new[] { "aaaa", "bbbb" }));
                Assert.That(two[0].From,                                                                  Is.Null);
                Assert.That(two[1].From,                                                                  Is.EqualTo(new DateTimeOffset(2027, 1, 31, 0, 0, 0, TimeSpan.Zero)));

                Assert.That(ConnectionIdentity.ToJSON(two).ToString(Newtonsoft.Json.Formatting.None),
                            Is.EqualTo("""[{"id":"aaaa"},{"id":"bbbb","from":"2027-01-31T00:00:00Z"}]"""));

                Assert.That(ConnectionIdentity.TryParse(JArray.Parse("""[ "aaaa", "aaaa" ]"""), out _, out var twice),          Is.False);
                Assert.That(twice,                                                                        Does.Contain("twice"));

                Assert.That(ConnectionIdentity.TryParse(JArray.Parse("""[ { "id": "aaaa", "from": "next week" } ]"""), out _, out var noTime), Is.False);
                Assert.That(noTime,                                                                       Does.Contain("no time"));

                Assert.That(ConnectionIdentity.TryParse(JArray.Parse("""[ { "from": "2027-01-31T00:00:00Z" } ]"""), out _, out var unnamed), Is.False);
                Assert.That(unnamed,                                                                      Does.Contain("named"));

                Assert.That(ConnectionIdentity.TryParse(new JValue("aaaa"), out _, out var noList),       Is.False);
                Assert.That(noList,                                                                       Does.Contain("list"));

            });

        }

        #endregion

        #region AConnectionNamesSeveral_AndKeepsThemOverARestart()

        /// <summary>
        /// A wss:// connection names two certificates, the second from a time
        /// on; both are in its file, and a store that reads the file again
        /// has them. The first it names is what it was asked for before it
        /// could name several.
        /// </summary>
        [Test]
        public void AConnectionNamesSeveral_AndKeepsThemOverARestart()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, null, out var id, out var error,
                                               Identities: [ new ("aaaa"), new ("bbbb", Start.AddDays(30)) ]),
                        Is.True, error);

            var again  = new ConnectionStore(store.Path, clock);
            var entry  = again.Connections.Single(one => one.Id == id);

            Assert.Multiple(() => {

                Assert.That(entry.Identities,                                       Is.EqualTo(new[] { new ConnectionIdentity("aaaa"), new ConnectionIdentity("bbbb", Start.AddDays(30)) }));
                Assert.That(entry.CertificateId,                                    Is.EqualTo("aaaa"));
                Assert.That(entry.IdentitiesAt(Start).           Select(one => one.Id), Is.EqualTo(new[] { "aaaa" }));
                Assert.That(entry.IdentitiesAt(Start.AddDays(30)).Select(one => one.Id), Is.EqualTo(new[] { "bbbb", "aaaa" }));

                Assert.That(entry.ToJSON()["identities"]!.ToString(Newtonsoft.Json.Formatting.None),
                            Is.EqualTo($$"""[{"id":"aaaa"},{"id":"bbbb","from":"{{Start.AddDays(30):yyyy-MM-ddTHH:mm:ssZ}}"}]"""));
                Assert.That(entry.ToJSON().Value<String>("certificateId"),          Is.EqualTo("aaaa"));

            });

        }

        #endregion

        #region AFileOfBeforeNamesOneCertificate_AndIsReadAsOneIdentity()

        [Test]
        public void AFileOfBeforeNamesOneCertificate_AndIsReadAsOneIdentity()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, "aaaa", out var id, out var error), Is.True, error);

            // As a store of before wrote it: "certificateId", and no list.
            var written = store.Connections.Single(one => one.Id == id).ToJSON();

            written.Remove("identities");

            Assert.That(ConnectionEntry.TryParse(written, out var entry, out var notRead), Is.True, notRead);
            Assert.That(entry!.Identities, Is.EqualTo(new[] { new ConnectionIdentity("aaaa") }));

            // And a list goes before it, where a file says both.
            written["identities"] = JArray.Parse("""[ "bbbb" ]""");

            Assert.That(ConnectionEntry.TryParse(written, out var both, out notRead), Is.True, notRead);
            Assert.That(both!.Identities, Is.EqualTo(new[] { new ConnectionIdentity("bbbb") }));

        }

        #endregion

        #region WhatAConnectionMayNotName()

        /// <summary>
        /// A certificate that is not here, an SSH key that is not here, the
        /// same one twice, and certificates beside a login: none of them is
        /// written down. Over SSH a login beside the keys is the password
        /// the server may ask for, and stays.
        /// </summary>
        [Test]
        public void WhatAConnectionMayNotName()
        {

            Assert.That(store.TryAddPasswordAuthentication(User_Id.Parse("cs001"), "not-the-real-one", I18NString.Create("CSMS login"), out var login, out var noLogin), Is.True, noLogin);

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, null, out _, out var noCertificate,
                                                   Identities: [ new ("aaaa"), new ("zzzz") ]),                                  Is.False);
                Assert.That(noCertificate,                                                                                       Does.Contain("not on this"));

                Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, null, out _, out var twice,
                                                   Identities: [ new ("aaaa"), new ("aaaa", Start.AddDays(1)) ]),                Is.False);
                Assert.That(twice,                                                                                               Does.Contain("twice"));

                Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, login, null, out _, out var twoWays,
                                                   Identities: [ new ("aaaa") ]),                                                Is.False);
                Assert.That(twoWays,                                                                                             Does.Contain("one way"));

                Assert.That(store.TryAddConnection(I18NString.Create("Over SSH"), URL.Parse(AnSshURL), "CSMS", true, null, null, out _, out var noKey,
                                                   SshHostKey: AHostKey, Identities: [ new ("99990000") ]),                      Is.False);
                Assert.That(noKey,                                                                                               Does.Contain("no SSH key"));

                Assert.That(store.TryAddConnection(I18NString.Create("Over SSH"), URL.Parse(AnSshURL), "CSMS", true, login, null, out var overSSH, out var refused,
                                                   SshHostKey: AHostKey, Identities: [ new (SshClientKeys.OwnId), new ("11112222", Start.AddDays(7)) ]),
                            Is.True, refused);

                var entry = store.Connections.Single(one => one.Id == overSSH);

                Assert.That(entry.Identities.Select(one => one.Id),   Is.EqualTo(new[] { SshClientKeys.OwnId, "11112222" }));
                Assert.That(entry.CertificateId,                      Is.Null, "an ssh:// connection shows a certificate");
                Assert.That(entry.AuthenticationId,                   Is.EqualTo(login));

            });

        }

        #endregion

        #region ACertificateThatIsSetIsShownNow_AndWhatFollowsLaterStillFollows()

        /// <summary>
        /// What CertificateSigned does: the connection shows the new
        /// certificate from now on - and the one written down to follow at a
        /// time that has not come still follows.
        /// </summary>
        [Test]
        public void ACertificateThatIsSetIsShownNow_AndWhatFollowsLaterStillFollows()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, null, out var id, out var error,
                                               Identities: [ new ("aaaa"), new ("bbbb", Start.AddDays(30)) ]),
                        Is.True, error);

            Assert.That(store.TrySetCertificate(id, "cccc", out var notSet), Is.True, notSet);

            var entry = store.Connections.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry.Identities,     Is.EqualTo(new[] { new ConnectionIdentity("cccc"), new ConnectionIdentity("bbbb", Start.AddDays(30)) }));
                Assert.That(entry.CertificateId,  Is.EqualTo("cccc"));
            });

        }

        #endregion

        #region AConnectionWhoseIdentitiesAllFollowLaterSaysSo()

        [Test]
        public void AConnectionWhoseIdentitiesAllFollowLaterSaysSo()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("CSMS"), URL.Parse(AWssURL), "CSMS", true, null, null, out var id, out var error,
                                               Identities: [ new ("aaaa", Start.AddDays(3)) ]),
                        Is.True, error);

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.Some.Contains("None of the identities this connection names is shown yet"));

        }

        #endregion


        #region TheSSHKeysOfANode_AreItsOwnAndTheOnesSomebodyMade()

        /// <summary>
        /// The node's own key is always named, made or not, and stays; a key
        /// somebody makes is a file of its own that only its owner reads,
        /// survives a restart, and is gone with its files when it is removed.
        /// </summary>
        [Test]
        public async Task TheSSHKeysOfANode_AreItsOwnAndTheOnesSomebodyMade()
        {

            var own   = new SshClientKey(Path.Combine(directory, "ssh"), "test@here");
            var keys  = new SshClientKeys(own, "This node's own key", "test@here", clock);

            Assert.That(keys.Entries.Select(entry => entry.Id), Is.EqualTo(new[] { SshClientKeys.OwnId }));
            Assert.That(keys.Entries[0].Key.Key,                Is.Null, "the node's own key was made before anything needed it");

            var (unnamed, why) = await keys.MakeAsync("  ");

            Assert.That(unnamed, Is.Null);
            Assert.That(why,     Does.Contain("what the key is for"));

            var (made, error) = await keys.MakeAsync(" CSMS over SSH, from January ");

            Assert.That(made, Is.Not.Null, error);

            Assert.Multiple(() => {

                Assert.That(made!.Label,                            Is.EqualTo("CSMS over SSH, from January"));
                Assert.That(made.IsTheNodes,                        Is.False);
                Assert.That(made.Key.PublicKeyLine,                 Does.StartWith("ssh-ed25519 "));
                Assert.That(made.Key.Fingerprint,                   Does.StartWith("SHA256:"));
                Assert.That(File.Exists(made.Key.Path),             Is.True);
                Assert.That(made.ToJSON().ToString(),               Does.Not.Contain("PRIVATE KEY"));
                Assert.That(keys.Ids,                               Is.EqualTo(new[] { SshClientKeys.OwnId, made.Id }));

            });

            // A restart: read from what was written.
            var again = new SshClientKeys(new SshClientKey(Path.Combine(directory, "ssh"), "test@here"), "This node's own key", "test@here", clock);

            Assert.Multiple(() => {
                Assert.That(again.Get(made!.Id)?.Label,             Is.EqualTo("CSMS over SSH, from January"));
                Assert.That(again.Get(made.Id)?.Key.Fingerprint,    Is.EqualTo(made.Key.Fingerprint), "another key came back under its name");
                Assert.That(again.Get(made.Id)?.CreatedAt,          Is.EqualTo(Start));
            });

            Assert.Multiple(() => {

                Assert.That(again.TryRemove(SshClientKeys.OwnId, out var stays),   Is.False);
                Assert.That(stays,                                                  Does.Contain("it stays"));

                Assert.That(again.TryRemove("00000000", out var unknown),          Is.False);
                Assert.That(unknown,                                                Does.Contain("no SSH key"));

                Assert.That(again.TryRemove(made!.Id, out var notRemoved),         Is.True, notRemoved);
                Assert.That(again.Ids,                                              Is.EqualTo(new[] { SshClientKeys.OwnId }));
                Assert.That(File.Exists(made.Key.Path),                             Is.False, "the private half of a key that was removed is still there");
                Assert.That(Directory.EnumerateFiles(again.Directory).Select(Path.GetFileName),
                            Is.EqualTo(new[] { SshClientKeys.IndexFileName }));

            });

        }

        #endregion

    }

}
