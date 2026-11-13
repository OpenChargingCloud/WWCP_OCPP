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

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The places this charging station dials, and the credentials it proves
    /// itself with when it gets there.
    /// </summary>
    /// <remarks>
    /// Two things are measured hardest here, because they are the two that go
    /// wrong quietly.
    ///
    /// The first is that a secret goes in and does not come back out. A
    /// password readable from a page is a password that belongs to everybody
    /// who ever borrowed that page, and the failure looks exactly like success
    /// from the outside - so the test looks for the secret in the answer
    /// itself rather than trusting a flag that says it was left out.
    ///
    /// The second is the reference from a connection to its credentials. It is
    /// the reason these two lists live in one store, and the way it breaks is
    /// that somebody deletes a login three connections were using and finds
    /// out at the next restart.
    /// </remarks>
    [TestFixture]
    public class ConnectionStoreTests
    {

        #region Data

        private const String  APassword      = "not-the-real-one";
        /// <summary>
        /// Deliberately not a run of base62 characters: the default alphabet a
        /// time-based password is drawn from is handed to the page, and a
        /// secret that happened to be a piece of it would let the test below
        /// find itself and pass for the wrong reason.
        /// </summary>
        private const String  ASharedSecret  = "shared-secret-not-the-real-one";

        private String            directory  = "";
        private ConnectionStore   store      = default!;
        private MovableClock clock = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {

            directory  = TestNodes.TemporaryDirectory("connections");
            clock      = new MovableClock(DateTimeOffset.UtcNow);
            store      = new ConnectionStore(Path.Combine(directory, "conn"), clock);

        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion

        #region (private) ABasicLogin(Description) / ATOTPLogin(Description)

        private String ABasicLogin(String Description = "CSMS login")
        {

            Assert.That(store.TryAddAuthentication(Description, "basic", "cs001", APassword, out var id, out var error),
                        Is.True, error);

            return id!;

        }

        private String ATOTPLogin(String Description = "Local controller login", Boolean ChannelBinding = true)
        {

            Assert.That(store.TryAddAuthentication(Description, "totp", "cs001", ASharedSecret,
                                                   out var id, out var error,
                                                   TLSChannelBinding: ChannelBinding),
                        Is.True, error);

            return id!;

        }

        #endregion


        #region ASecretGoesInAndDoesNotComeBackOut()

        /// <summary>
        /// Neither the password nor the shared secret is anywhere in what the
        /// web interface is handed.
        /// </summary>
        /// <remarks>
        /// Looked for in the text of the whole answer rather than in the field
        /// it would sit in, because the way this breaks is a secret turning up
        /// somewhere nobody thought to check - in a warning that quotes it, in
        /// a record echoed back after a change, in a copy of the entry
        /// somewhere further down.
        /// </remarks>
        [Test]
        public void ASecretGoesInAndDoesNotComeBackOut()
        {

            ABasicLogin();
            ATOTPLogin();

            var handedOut = store.ToJSON().ToString();

            Assert.Multiple(() => {

                Assert.That(handedOut, Does.Not.Contain(APassword),
                            "The password is in what the web interface is handed.");

                Assert.That(handedOut, Does.Not.Contain(ASharedSecret),
                            "The shared secret is in what the web interface is handed.");

                Assert.That(handedOut, Does.Contain("\"hasSecret\": true"),
                            "Nothing says whether a secret is set at all.");

            });

        }

        #endregion

        #region TheSecretIsOnDiskWhereOnlyItsOwnerCanReadIt()

        /// <summary>
        /// It is left out of the answer, not thrown away.
        /// </summary>
        /// <remarks>
        /// The other half of the test above, and worth having: a store that
        /// dropped the secret entirely would pass that one perfectly.
        /// </remarks>
        [Test]
        public void TheSecretIsOnDiskWhereOnlyItsOwnerCanReadIt()
        {

            var id   = ABasicLogin();

            var file = Path.Combine(store.Path, ConnectionStore.AuthenticationsFileName);

            Assert.That(File.Exists(file), Is.True, "Nothing was written down.");
            Assert.That(File.ReadAllText(file), Does.Contain(APassword),
                        "The password was not kept, so the station cannot use it either.");

            // And it comes back after a restart.
            var again = new ConnectionStore(store.Path, clock);

            Assert.That(again.Authentications.Single(entry => entry.Id == id).Password,
                        Is.EqualTo(APassword),
                        "The password did not survive a restart.");

        }

        #endregion

        #region ChangingSomethingElseKeepsTheSecret()

        /// <summary>
        /// A description corrected with the secret field left empty keeps the
        /// secret.
        /// </summary>
        /// <remarks>
        /// The reason this is an update rather than a delete and an add: being
        /// asked for the password again in order to fix a typo is how a
        /// password ends up on a sticky note. And this station cannot show it
        /// back, so asking would mean asking somebody to find it elsewhere.
        /// </remarks>
        [Test]
        public void ChangingSomethingElseKeepsTheSecret()
        {

            var id = ABasicLogin("CSMS lgoin");

            Assert.That(store.TryUpdateAuthentication(id, "CSMS login", "basic", "cs001", "", out var error),
                        Is.True, error);

            var entry = store.Authentications.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry.Description, Is.EqualTo("CSMS login"), "The correction did not take.");
                Assert.That(entry.Password,    Is.EqualTo(APassword),    "The password was thrown away by a change that was not about it.");
            });

        }

        #endregion

        #region ChangingWhatKindItIsNeedsTheRightKindOfSecret()

        /// <summary>
        /// A password is not a shared secret, so the change that turns one into
        /// the other has to bring one.
        /// </summary>
        [Test]
        public void ChangingWhatKindItIsNeedsTheRightKindOfSecret()
        {

            var id = ABasicLogin();

            Assert.That(store.TryUpdateAuthentication(id, "CSMS login", "totp", "cs001", "", out var refused),
                        Is.False,
                        "A password was quietly kept on as a shared secret.");

            Assert.That(refused, Does.Contain("shared secret"));

            // With one, it goes through - and the password is gone, because
            // nothing can use it any more.
            Assert.That(store.TryUpdateAuthentication(id, "CSMS login", "totp", "cs001", ASharedSecret, out var error),
                        Is.True, error);

            var entry = store.Authentications.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry.Kind,         Is.EqualTo(AuthenticationKind.TOTP));
                Assert.That(entry.SharedSecret, Is.EqualTo(ASharedSecret));
                Assert.That(entry.Password,     Is.Null, "A password nothing can use was left lying on disk.");
            });

        }

        #endregion

        #region CredentialsInUseCannotBeRemoved()

        /// <summary>
        /// Removing a login a connection is using is refused, and the refusal
        /// names the connection.
        /// </summary>
        [Test]
        public void CredentialsInUseCannotBeRemoved()
        {

            var login = ABasicLogin();

            Assert.That(store.TryAddConnection("CSMS, main", "wss://csms.example.org/cs001", "CSMS",
                                               true, login, null, out _, out var error),
                        Is.True, error);

            Assert.That(store.TryRemoveAuthentication(login, out var refused), Is.False,
                        "A login three connections could be using was removed without a word.");

            Assert.That(refused, Does.Contain("CSMS, main"),
                        "The refusal does not say which connection is in the way.");

            // Point the connection elsewhere, and it goes.
            var other = store.Connections.Single();

            Assert.That(store.TryUpdateConnection(other.Id, other.Description, other.URL.ToString(),
                                                  "CSMS", true, null, null, out error),
                        Is.True, error);

            Assert.That(store.TryRemoveAuthentication(login, out error), Is.True, error);

        }

        #endregion

        #region AConnectionCannotNameCredentialsThatAreNotHere()

        /// <summary>
        /// A reference to something that does not exist is refused on the way
        /// in.
        /// </summary>
        [Test]
        public void AConnectionCannotNameCredentialsThatAreNotHere()
        {

            Assert.That(store.TryAddConnection("CSMS", "wss://csms.example.org/cs001", "CSMS",
                                               false, "no-such-login", null, out _, out var refused),
                        Is.False,
                        "A connection was written down pointing at credentials that do not exist.");

            Assert.That(refused, Does.Contain("not configured"));

        }

        #endregion

        #region AConnectionProvesItselfOneWayAndNotTwo()

        /// <summary>
        /// Credentials or a certificate, not both.
        /// </summary>
        [Test]
        public void AConnectionProvesItselfOneWayAndNotTwo()
        {

            var login = ABasicLogin();

            store.KnownCertificateIds = () => [ "abc123" ];

            Assert.That(store.TryAddConnection("CSMS", "wss://csms.example.org/cs001", "CSMS",
                                               false, login, "abc123", out _, out var refused),
                        Is.False,
                        "A connection was given two ways of proving itself and nobody can say which is used.");

            Assert.That(refused, Does.Contain("one way"));

        }

        #endregion

        #region WhatIsWrongWithAURLIsSaidWhileSomebodyIsLooking(URL, Says)

        /// <summary>
        /// A URL that cannot work is refused with a sentence, not at the next
        /// connection attempt.
        /// </summary>
        /// <remarks>
        /// The schemeless cases are the ones worth having. Hermod's parser
        /// puts "https://" in front of anything without a scheme, which
        /// everywhere else is a convenience and here would have the station
        /// deciding for somebody whether the connection is encrypted.
        /// </remarks>
        [Test]
        [TestCase("",                        "somewhere to go")]
        [TestCase("not a url at all",        "scheme")]
        [TestCase("csms.example.org/cs001",  "scheme")]
        [TestCase("wsss://csms.example.org", "'wsss'")]
        [TestCase("ftp://csms.example.org",  "not something this station dials")]
        public void WhatIsWrongWithAURLIsSaidWhileSomebodyIsLooking(String URL, String Says)
        {

            Assert.That(store.TryAddConnection("CSMS", URL, "CSMS", false, null, null, out _, out var refused),
                        Is.False,
                        $"'{URL}' was accepted as somewhere to dial.");

            Assert.That(refused, Does.Contain(Says).IgnoreCase);

        }

        #endregion

        #region EveryKindOfConnectionCanBeWrittenDown(Type)

        /// <summary>
        /// The three ends a station dials, each one accepted and remembered.
        /// </summary>
        [Test]
        [TestCase("CSMS")]
        [TestCase("CSMSBackup")]
        [TestCase("LocalController")]
        public void EveryKindOfConnectionCanBeWrittenDown(String Type)
        {

            Assert.That(store.TryAddConnection($"A {Type}", "wss://somewhere.example.org/cs001", Type,
                                               true, null, null, out var id, out var error),
                        Is.True, error);

            var entry = store.Connections.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry.ConnectionType.ToString(), Is.EqualTo(Type));
                Assert.That(entry.AutoConnect,        Is.True, "Automatic reconnect was not remembered.");
                Assert.That(entry.IsSecure,                  Is.True);
            });

        }

        #endregion

        #region APasswordSentInTheClearIsSaidOutLoud()

        /// <summary>
        /// HTTP Basic over a connection without TLS is a password anybody on
        /// the way can read, and the station says so.
        /// </summary>
        /// <remarks>
        /// Not refused: a station on a bench talking to a test back end over
        /// ws:// is a real thing to want, and refusing it would push somebody
        /// towards turning the warning off altogether. Said, every time the
        /// page is opened.
        /// </remarks>
        [Test]
        public void APasswordSentInTheClearIsSaidOutLoud()
        {

            var login = ABasicLogin();

            Assert.That(store.TryAddConnection("Test back end", "ws://127.0.0.1:9000/cs001", "CSMS",
                                               false, login, null, out var id, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.Some.Contains("in the clear"),
                        "A password going out unencrypted was not worth mentioning.");

        }

        #endregion

        #region NewOneTimePasswordsAreUnboundByDefault()

        /// <summary>
        /// Written down without saying, a one-time password is unbound: bound,
        /// it could not be sent at all, since this station's TLS exports no
        /// keying material to bind it with.
        /// </summary>
        [Test]
        public void NewOneTimePasswordsAreUnboundByDefault()
        {

            Assert.That(store.TryAddAuthentication("Said nothing", "totp", "cs001", ASharedSecret,
                                                   out var id, out var error),
                        Is.True, error);

            Assert.That(store.Authentications.Single(one => one.Id == id).TLSChannelBinding, Is.False,
                        "A one-time password written down without saying is bound, and so is never sent.");

        }

        #endregion

        #region AChangeThatSaysNothingOfTheBindingKeepsIt(Bound)

        /// <summary>
        /// A change of a one-time password that leaves the binding out leaves it
        /// as it was, as it does the secret.
        /// </summary>
        [Test]
        public void AChangeThatSaysNothingOfTheBindingKeepsIt([Values] Boolean Bound)
        {

            var id = ATOTPLogin("Before", ChannelBinding: Bound);

            Assert.That(store.TryUpdateAuthentication(id, "After", "totp", "cs001", null, out var error),
                        Is.True, error);

            Assert.That(store.Authentications.Single(one => one.Id == id).TLSChannelBinding, Is.EqualTo(Bound),
                        "A change that said nothing of the binding changed it.");

        }

        #endregion

        #region ABoundPasswordIsNotDialledOverTLSEither()

        /// <summary>
        /// Bound to the TLS session, a one-time password cannot be made over
        /// wss:// either, and the connection says so before anybody wonders why
        /// it never connects.
        /// </summary>
        [Test]
        public void ABoundPasswordIsNotDialledOverTLSEither()
        {

            var bound = ATOTPLogin("Bound", ChannelBinding: true);

            Assert.That(store.TryAddConnection("Over wss", "wss://csms.example.org/cs001", "CSMS",
                                               false, bound, null, out var id, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.Some.Contains("not dialled"),
                        "A one-time password that cannot be sent as configured was not worth mentioning.");

        }

        #endregion

        #region AChannelBoundPasswordNeedsAChannel()

        /// <summary>
        /// A TOTP bound to a TLS session, over a connection that has none.
        /// </summary>
        [Test]
        public void AChannelBoundPasswordNeedsAChannel()
        {

            var bound    = ATOTPLogin("Bound",   ChannelBinding: true);
            var unbound  = ATOTPLogin("Unbound", ChannelBinding: false);

            Assert.That(store.TryAddConnection("Over ws", "ws://127.0.0.1:9000/cs001", "CSMS",
                                               false, bound, null, out var withBinding, out var error),
                        Is.True, error);

            Assert.That(store.TryAddConnection("Over ws too", "ws://127.0.0.1:9001/cs001", "CSMS",
                                               false, unbound, null, out var withoutBinding, out error),
                        Is.True, error);

            var connections = store.Connections;

            Assert.Multiple(() => {

                Assert.That(connections.Single(one => one.Id == withBinding).Warnings,
                            Has.Some.Contains("TLS session"),
                            "A password bound to a session that does not exist was not worth mentioning.");

                Assert.That(connections.Single(one => one.Id == withoutBinding).Warnings,
                            Has.None.Contains("TLS session"),
                            "A password that says it is unbound was complained about anyway.");

            });

        }

        #endregion

        #region AConnectionThatProvesNothingSaysSo()

        /// <summary>
        /// No credentials and no certificate is allowed, and mentioned.
        /// </summary>
        [Test]
        public void AConnectionThatProvesNothingSaysSo()
        {

            Assert.That(store.TryAddConnection("Bench", "ws://127.0.0.1:9000/cs001", "CSMS",
                                               false, null, null, out var id, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.Some.Contains("proves nothing"),
                        "A connection that identifies itself to nobody was not worth mentioning.");

        }

        #endregion

        #region ACertificateThatWentAwayIsNoticed()

        /// <summary>
        /// A connection naming a certificate that has since been removed.
        /// </summary>
        /// <remarks>
        /// The certificates live in their own store and can be removed there,
        /// which is why this is a warning worked out each time rather than a
        /// refusal at the moment of writing: at the moment of writing it was
        /// perfectly true.
        /// </remarks>
        [Test]
        public void ACertificateThatWentAwayIsNoticed()
        {

            var here = new List<String> { "abc123" };

            store.KnownCertificateIds = () => here;

            Assert.That(store.TryAddConnection("CSMS", "wss://csms.example.org/cs001", "CSMS",
                                               false, null, "abc123", out var id, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.None.Contains("not on this station"),
                        "A certificate that is right here was reported missing.");

            here.Clear();

            Assert.That(store.Connections.Single(one => one.Id == id).Warnings,
                        Has.Some.Contains("not on this station"),
                        "A certificate that was removed next door is still being counted on.");

        }

        #endregion

        #region CredentialsWithoutASecretAreRefused(Kind, Secret, Says)

        /// <summary>
        /// What cannot work is said while somebody is still typing it.
        /// </summary>
        [Test]
        [TestCase("basic", "",                 "password is needed")]
        [TestCase("totp",  "",                 "shared secret is needed")]
        [TestCase("totp",  "too-short",        "at least 16")]
        [TestCase("totp",  "has a space in it","spaces")]
        public void CredentialsWithoutASecretAreRefused(String Kind, String Secret, String Says)
        {

            Assert.That(store.TryAddAuthentication("Somewhere", Kind, "cs001", Secret, out _, out var refused),
                        Is.False,
                        $"A {Kind} login with '{Secret}' was written down.");

            Assert.That(refused, Does.Contain(Says).IgnoreCase);

        }

        #endregion

        #region ADescriptionIsWhatTellsThemApart()

        /// <summary>
        /// Both kinds of record insist on being described.
        /// </summary>
        /// <remarks>
        /// The one field somebody is tempted to leave empty, and the one that
        /// makes a page of six logins readable a year later.
        /// </remarks>
        [Test]
        public void ADescriptionIsWhatTellsThemApart()
        {

            Assert.Multiple(() => {

                Assert.That(store.TryAddAuthentication("  ", "basic", "cs001", APassword, out _, out var noLogin),
                            Is.False, "A login without a description was written down.");
                Assert.That(noLogin, Does.Contain("what these credentials are for"));

                Assert.That(store.TryAddConnection("", "wss://csms.example.org/cs001", "CSMS",
                                                   false, null, null, out _, out var noConnection),
                            Is.False, "A connection without a description was written down.");
                Assert.That(noConnection, Does.Contain("what this connection is"));

            });

        }

        #endregion

        #region EverythingSurvivesARestart()

        /// <summary>
        /// Both files read back, with the references between them intact.
        /// </summary>
        [Test]
        public void EverythingSurvivesARestart()
        {

            var login = ATOTPLogin("Local controller", ChannelBinding: false);

            Assert.That(store.TryAddAuthentication("CSMS login", "basic", "cs001", APassword, out var basic, out var error),
                        Is.True, error);

            Assert.That(store.TryAddConnection("Local controller", "wss://lc.example.org/cs001", "LocalController",
                                               true, login, null, out var connection, out error),
                        Is.True, error);

            Assert.That(store.TryAddConnection("CSMS, spare", "wss://spare.example.org/cs001", "CSMSBackup",
                                               false, basic, null, out _, out error),
                        Is.True, error);

            var again = new ConnectionStore(store.Path, clock);

            var back  = again.Connections.Single(one => one.Id == connection);
            var totp  = again.Authentications.Single(one => one.Id == login);

            Assert.Multiple(() => {

                Assert.That(again.Authentications.Count,  Is.EqualTo(2));
                Assert.That(again.Connections.Count,      Is.EqualTo(2));

                Assert.That(back.ConnectionType,          Is.EqualTo(ConnectionType.LocalController));
                Assert.That(back.AutoConnect,      Is.True);
                Assert.That(back.AuthenticationId,        Is.EqualTo(login),
                            "The connection lost track of which credentials it uses.");

                Assert.That(totp.SharedSecret,            Is.EqualTo(ASharedSecret));
                Assert.That(totp.TLSChannelBinding,       Is.False,
                            "Whether the password is bound to the session was not remembered.");

                Assert.That(back.Warnings,                Is.Empty,
                            $"Something is being complained about after a restart: {String.Join(" ", back.Warnings)}");

            });

        }

        #endregion

        #region TheCredentialsBecomeWhatAnHTTPClientNeeds()

        /// <summary>
        /// Both kinds turn into what Hermod's HTTP client actually takes.
        /// </summary>
        /// <remarks>
        /// The point of all of it. A configuration that could be typed in and
        /// stored and never became a credential an HTTP client would accept
        /// would be a page that looks like it works.
        /// </remarks>
        [Test]
        public void TheCredentialsBecomeWhatAnHTTPClientNeeds()
        {

            // Written down first: the predicate of a Single() runs once per
            // element, so calling these in there adds a login per comparison.
            var basicId = ABasicLogin();
            var totpId  = ATOTPLogin();

            var basic   = store.Authentications.Single(one => one.Id == basicId);
            var totp    = store.Authentications.Single(one => one.Id == totpId);

            Assert.Multiple(() => {

                var asBasic = basic.ToBasicAuthentication();

                Assert.That(asBasic,           Is.Not.Null, "HTTP Basic credentials did not become an HTTP Basic authentication.");
                Assert.That(asBasic!.Username, Is.EqualTo("cs001"));
                Assert.That(asBasic.Password,  Is.EqualTo(APassword));

                var asTOTP = totp.ToTOTPConfig();

                Assert.That(asTOTP,               Is.Not.Null, "TOTP credentials did not become a TOTP config.");
                Assert.That(asTOTP!.SharedSecret, Is.EqualTo(ASharedSecret));
                Assert.That(asTOTP.UseTLSExporterMaterial, Is.True);

                // And neither pretends to be the other.
                Assert.That(basic.ToTOTPConfig(),          Is.Null);
                Assert.That(totp. ToBasicAuthentication(), Is.Null);

            });

        }

        #endregion

    }

}
