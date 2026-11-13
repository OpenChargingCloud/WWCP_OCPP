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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SSH;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The ssh:// connections this charging station is told about: what it
    /// takes, what it refuses, and the host key it pins.
    /// </summary>
    /// <remarks>
    /// The host keys are measured hardest, because they are what is written
    /// by the station rather than by somebody: the first connection of one
    /// that trusts on first use learns the key it was shown, on probation, and
    /// a signed pin set replaces the pins - and nothing a person does on the
    /// page afterwards may undo that by accident - a save of the description
    /// from a page drawn before it.
    /// </remarks>
    [TestFixture]
    public class SshConnectionStoreTests
    {

        #region Data

        private String           directory  = "";
        private ConnectionStore  store      = default!;

        /// <summary>
        /// The host key of a server that is never started.
        /// </summary>
        private static readonly ISshHostKey  hostKey      = SshHostKey.GenerateEd25519();

        private static String Fingerprint
            => SshFingerprint.Sha256(hostKey.PublicKeyBlob);

        private const String  AnSshURL = "ssh://cs001@csms.example.org:2222";

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory  = TestNodes.TemporaryDirectory("ssh-connections");
            store      = new ConnectionStore(Path.Combine(directory, "conn"));
        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion


        #region AnSSHURLIsTakenAndSaysSo()

        /// <summary>
        /// An ssh:// URL is a connection like any other, and says that it goes
        /// over SSH - with a host key pinned or without one.
        /// </summary>
        [Test]
        public void AnSSHURLIsTakenAndSaysSo()
        {

            Assert.That(store.TryAddConnection("Over SSH, pinned", AnSshURL, "CSMS", true, null, null,
                                               out var pinned, out var error, SshHostKey: $"  {Fingerprint} "),
                        Is.True, error);

            Assert.That(store.TryAddConnection("Over SSH, not pinned", AnSshURL, "CSMS", true, null, null,
                                               out var notPinned, out error),
                        Is.True, error);

            var withPin     = store.Connections.Single(entry => entry.Id == pinned);
            var withoutPin  = store.Connections.Single(entry => entry.Id == notPinned);

            Assert.Multiple(() => {

                Assert.That(withPin.IsSSH,                                  Is.True);
                Assert.That(withPin.SshHostKey,                             Is.EqualTo(Fingerprint), "The pin was not taken as written, trimmed.");
                Assert.That(withPin.ToJSON().Value<String>("transport"),    Is.EqualTo("ssh"));
                Assert.That(withPin.Warnings,                               Is.Empty,
                            "An ssh:// connection proves itself with this station's key, and a pinned one has nothing to warn of.");

                Assert.That(withoutPin.SshHostKey,                          Is.Null);
                Assert.That(withoutPin.Warnings,                            Has.Some.Contains("not dialled"),
                            "That a connection without a pin and without trust on first use is not dialled was not said out loud.");

            });

            // And read back as it was written.
            var again = new ConnectionStore(store.Path);

            Assert.That(again.Connections.Single(entry => entry.Id == pinned).SshHostKey, Is.EqualTo(Fingerprint));

        }

        #endregion

        #region WhatCannotBeAHostKeyIsRefused()

        /// <summary>
        /// A pin that is neither a fingerprint nor a public-key line is
        /// refused, and so is one beside a WebSocket, whose server is known by
        /// its certificate.
        /// </summary>
        [Test]
        public void WhatCannotBeAHostKeyIsRefused()
        {

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, null,
                                                   out _, out var notAKey, SshHostKey: "the one from last week"),
                            Is.False);
                Assert.That(notAKey, Does.Contain("neither a fingerprint"));

                Assert.That(store.TryAddConnection("Over a WebSocket", "wss://csms.example.org/cs001", "CSMS", true, null, null,
                                                   out _, out var notSSH, SshHostKey: Fingerprint),
                            Is.False);
                Assert.That(notSSH, Does.Contain("TLS certificate"));

                // A public-key line is a pin as well.
                Assert.That(store.TryAddConnection("Over SSH, by its key", AnSshURL, "CSMS", true, null, null,
                                                   out _, out var error,
                                                   SshHostKey: SshPublicKey.FromHostKey(hostKey).ToAuthorizedKeyLine()),
                            Is.True, error);

            });

            Assert.That(store.Connections, Has.Count.EqualTo(1));

        }

        #endregion

        #region ACertificateOrATOTPPasswordIsRefusedOverSSH()

        /// <summary>
        /// Over SSH there is no TLS handshake to show a certificate in, and no
        /// HTTP to answer a TOTP password in. A Basic login is taken: its
        /// password is what an SSH server asks for.
        /// </summary>
        [Test]
        public void ACertificateOrATOTPPasswordIsRefusedOverSSH()
        {

            Assert.That(store.TryAddAuthentication("A TOTP login", "totp", "cs001", "shared-secret-not-the-real-one", out var totp, out var error),
                        Is.True, error);

            Assert.That(store.TryAddAuthentication("A Basic login", "basic", "cs001", "not-the-real-one", out var basic, out error),
                        Is.True, error);

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, totp, null, out _, out var refusedTOTP),
                            Is.False);
                Assert.That(refusedTOTP, Does.Contain("TOTP"));

                Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, "some-certificate", out _, out var refusedCertificate),
                            Is.False);
                Assert.That(refusedCertificate, Does.Contain("TLS handshake"));

                Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, basic, null, out var withPassword, out error),
                            Is.True, error);

                Assert.That(store.Connections.Single(entry => entry.Id == withPassword).Warnings,
                            Has.None.Contains("in the clear"),
                            "A password over SSH is not sent in the clear.");

            });

        }

        #endregion

        #region WhenAKeyWasLearned_IsReadWhateverTheDayAndTheCulture(Culture, Day)

        /// <summary>
        /// When a host key was learned is read as it was written, on a machine
        /// that writes the day first as on one that writes the month first, and
        /// on every day of a month: a file is read with its times recognized,
        /// and a time asked for as a text is written the invariant way.
        /// </summary>
        [TestCase("de-DE",  7)]
        [TestCase("de-DE", 25)]
        [TestCase("en-US",  7)]
        [TestCase("en-US", 25)]
        public void WhenAKeyWasLearned_IsReadWhateverTheDayAndTheCulture(String Culture, Int32 Day)
        {

            var before = System.Globalization.CultureInfo.CurrentCulture;

            try
            {

                System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Culture);

                Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, null, out var id, out var error,
                                                   SshTrust: new SshTrustSettings("HostKey", null, 120)),
                            Is.True, error);

                Assert.That(store.TryLearnHostKey(id!, URL.Parse(AnSshURL), Fingerprint, out error), Is.True, error);

                var written = store.Connections.Single(entry => entry.Id == id).ToJSON();
                var learned = $"2026-11-{Day:00}T10:11:12.000Z";

                written["sshLearnedAt"] = learned;

                // As a file is read: what looks like a time is one.
                var asRead  = Newtonsoft.Json.Linq.JObject.Parse(written.ToString());

                Assert.That(asRead["sshLearnedAt"]!.Type,  Is.EqualTo(Newtonsoft.Json.Linq.JTokenType.Date));

                Assert.That(ConnectionEntry.TryParse(asRead, out var entry, out error), Is.True, error);

                Assert.That(entry!.SshLearnedAt,
                            Is.EqualTo(new DateTimeOffset(2026, 11, Day, 10, 11, 12, TimeSpan.Zero)));

                // And as a text that was left a text.
                Assert.That(ConnectionEntry.TryParse(written, out var fromText, out error), Is.True, error);

                Assert.That(fromText!.SshLearnedAt,
                            Is.EqualTo(new DateTimeOffset(2026, 11, Day, 10, 11, 12, TimeSpan.Zero)));

            }
            finally
            {
                System.Globalization.CultureInfo.CurrentCulture = before;
            }

        }

        #endregion

        #region ALearnedHostKeyIsWrittenDownOnlyWhereItTrustsOnFirstUse()

        /// <summary>
        /// The host key the first connection was shown is learned - on
        /// probation - only where the connection trusts on first use, pins
        /// nothing and learned nothing yet, and still goes where it went.
        /// </summary>
        [Test]
        public void ALearnedHostKeyIsWrittenDownOnlyWhereItTrustsOnFirstUse()
        {

            Assert.That(store.TryAddConnection("Over SSH, no trust on first use", AnSshURL, "CSMS", true, null, null, out var withoutTOFU, out var error),
                        Is.True, error);

            Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, null, out var id, out error,
                                               SshTrust: new SshTrustSettings("HostKey", null, 120)),
                        Is.True, error);

            var dialled = URL.Parse(AnSshURL);

            Assert.Multiple(() => {

                Assert.That(store.TryLearnHostKey(withoutTOFU!, dialled, Fingerprint, out _), Is.False,
                            "A connection that does not trust on first use learned a key.");

                Assert.That(store.TryLearnHostKey(id!, URL.Parse("ssh://cs001@elsewhere.example.org"), Fingerprint, out _), Is.False,
                            "A key shown by where the connection went before was learned for where it goes now.");

                Assert.That(store.TryLearnHostKey(id!, dialled, Fingerprint, out error), Is.True, error);

                Assert.That(store.TryLearnHostKey(id!, dialled, "SHA256:another", out _), Is.False,
                            "A learned key was overwritten.");

            });

            var again = new ConnectionStore(store.Path).Connections.Single(entry => entry.Id == id);

            Assert.Multiple(() => {
                Assert.That(again.SshLearnedHostKey,   Is.EqualTo(Fingerprint), "The key learned did not reach the file.");
                Assert.That(again.SshLearnedAt,        Is.Not.Null);
                Assert.That(again.SshHostKey,          Is.Null, "A learned key is no pin: it is on probation.");
                Assert.That(again.SshTrustOnFirstUse,  Is.EqualTo(SshTrustOnFirstUse.HostKey));
                Assert.That(again.SshProbationTime,    Is.EqualTo(TimeSpan.FromSeconds(120)));
                Assert.That(store.Connections.Single(entry => entry.Id == id).Warnings, Has.Some.Contains("on probation"));
                Assert.That(store.Connections.Single(entry => entry.Id == withoutTOFU).Warnings, Has.Some.Contains("not dialled"));
            });

            // A probation that ran out forgets it - only that one.
            Assert.That(store.TryForgetLearnedHostKey(id!, "SHA256:another"), Is.False);
            Assert.That(store.TryForgetLearnedHostKey(id!, Fingerprint),       Is.True);
            Assert.That(new ConnectionStore(store.Path).Connections.Single(entry => entry.Id == id).SshLearnedHostKey, Is.Null);

        }

        #endregion

        #region ASignedPinSetIsTakenOnlyWithAHigherVersion()

        /// <summary>
        /// A signed pin set replaces the pins, ends a probation - confirming the
        /// learned key, or not - and is taken only with a higher version than
        /// the one there is, so that an older one cannot be brought back.
        /// </summary>
        [Test]
        public void ASignedPinSetIsTakenOnlyWithAHigherVersion()
        {

            var other = SshFingerprint.Sha256(SshHostKey.GenerateEd25519().PublicKeyBlob);

            Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, null, out var id, out var error,
                                               SshTrust: new SshTrustSettings("HostKey", null, null)),
                        Is.True, error);

            Assert.That(store.TryLearnHostKey(id!, URL.Parse(AnSshURL), Fingerprint, out error), Is.True, error);

            Assert.That(store.TryTakeSignedHostKeys(id!, $"{other}, {Fingerprint}", 3, out var learned, out var confirmed, out error), Is.True, error);

            var entry = store.Connections.Single();

            Assert.Multiple(() => {
                Assert.That(learned,                   Is.EqualTo(Fingerprint));
                Assert.That(confirmed,                 Is.True, "The learned key was among the signed pins, and was not taken as confirmed.");
                Assert.That(entry.SshHostKeys,         Is.EqualTo(new[] { other, Fingerprint }));
                Assert.That(entry.SshHostKeysVersion,  Is.EqualTo(3));
                Assert.That(entry.SshLearnedHostKey,   Is.Null, "The probation did not end.");
            });

            Assert.Multiple(() => {

                Assert.That(store.TryTakeSignedHostKeys(id!, other, 3, out _, out _, out var sameVersion), Is.False);
                Assert.That(sameVersion, Does.Contain("only a higher one is taken"));

                Assert.That(store.TryTakeSignedHostKeys(id!, other, 2, out _, out _, out _), Is.False, "An older pin set was brought back.");

                Assert.That(store.TryTakeSignedHostKeys(id!, "", 4, out _, out _, out var none), Is.False);
                Assert.That(none, Does.Contain("without a host key"));

                Assert.That(store.TryTakeSignedHostKeys(id!, other, 4, out learned, out confirmed, out error), Is.True, error);
                Assert.That(learned,   Is.Null);
                Assert.That(confirmed, Is.False);

            });

            Assert.That(new ConnectionStore(store.Path).Connections.Single().SshHostKeysVersion, Is.EqualTo(4));

        }

        #endregion

        #region ASaveAsShownKeepsThePinSetTakenMeanwhile()

        /// <summary>
        /// A page drawn before a signed pin set arrived shows no host key. Saved
        /// with the field as it was, the pins taken since stay; typed into, the
        /// field is what is taken - emptied, too - and the version goes with
        /// what was typed.
        /// </summary>
        [Test]
        public void ASaveAsShownKeepsThePinSetTakenMeanwhile()
        {

            Assert.That(store.TryAddConnection("Over SSH", AnSshURL, "CSMS", true, null, null, out var id, out var error,
                                               SshTrust: new SshTrustSettings("HostKey", null, null)),
                        Is.True, error);

            Assert.That(store.TryTakeSignedHostKeys(id!, Fingerprint, 1, out _, out _, out error), Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "Over SSH, renamed", AnSshURL, "CSMS", true, null, null, out error,
                                                  SshHostKey: "", SshHostKeyAsShown: ""),
                        Is.True, error);

            Assert.That(store.Connections.Single().SshHostKey, Is.EqualTo(Fingerprint),
                        "A save of the description took away the pins taken meanwhile.");

            // Left out altogether, as by somebody using the API without it.
            Assert.That(store.TryUpdateConnection(id, "Over SSH, renamed again", AnSshURL, "CSMS", true, null, null, out error),
                        Is.True, error);

            Assert.That(store.Connections.Single().SshHostKey,          Is.EqualTo(Fingerprint));
            Assert.That(store.Connections.Single().SshTrustOnFirstUse,  Is.EqualTo(SshTrustOnFirstUse.HostKey),
                        "Left out, how it trusts on first use was changed.");

            // Emptied on purpose: the version goes with the pins.
            Assert.That(store.TryUpdateConnection(id, "Over SSH", AnSshURL, "CSMS", true, null, null, out error,
                                                  SshHostKey: "", SshHostKeyAsShown: Fingerprint),
                        Is.True, error);

            Assert.That(store.Connections.Single().SshHostKey,          Is.Null);
            Assert.That(store.Connections.Single().SshHostKeysVersion,  Is.EqualTo(0));

            // And a connection that is no longer ssh:// keeps none.
            Assert.That(store.TryTakeSignedHostKeys(id!, Fingerprint, 1, out _, out _, out error), Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "Over a WebSocket now", "wss://csms.example.org/cs001", "CSMS", true, null, null, out error),
                        Is.True, error);

            Assert.That(store.Connections.Single().SshHostKey,          Is.Null,
                        "A host key stayed beside a WebSocket, where nothing checks it.");
            Assert.That(store.Connections.Single().SshTrustOnFirstUse,  Is.EqualTo(SshTrustOnFirstUse.None));

        }

        #endregion

        #region PinsAreASetOfSHA256FingerprintsOrKeyLines()

        /// <summary>
        /// Up to 16 pins, separated by commas: SHA-256 fingerprints or
        /// public-key lines. An MD5 fingerprint is refused with the way to its
        /// SHA-256 one; one already in the file is kept, and warned of.
        /// </summary>
        [Test]
        public void PinsAreASetOfSHA256FingerprintsOrKeyLines()
        {

            var keyLine = SshPublicKey.FromHostKey(hostKey).ToAuthorizedKeyLine();
            var many    = String.Join(",", Enumerable.Range(0, 17).Select(_ => SshFingerprint.Sha256(SshHostKey.GenerateEd25519().PublicKeyBlob)));

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("Two pins", AnSshURL, "CSMS", true, null, null, out var two, out var error,
                                                   SshHostKey: $"{Fingerprint} ,\n {keyLine}"),
                            Is.True, error);
                Assert.That(store.Connections.Single(entry => entry.Id == two).SshHostKeys, Is.EqualTo(new[] { Fingerprint, keyLine }));

                Assert.That(store.TryAddConnection("MD5", AnSshURL, "CSMS", true, null, null, out _, out var md5,
                                                   SshHostKey: "MD5:16:27:ac:a5:76:28:2d:36:63:1b:56:4d:eb:df:a6:48"),
                            Is.False);
                Assert.That(md5, Does.Contain("MD5 fingerprints are no longer taken").And.Contain("SHA-256"));

                Assert.That(store.TryAddConnection("Too many", AnSshURL, "CSMS", true, null, null, out _, out var tooMany, SshHostKey: many),
                            Is.False);
                Assert.That(tooMany, Does.Contain("at most 16"));

                Assert.That(store.TryAddConnection("Mismatch", AnSshURL, "CSMS", true, null, null, out var tolerant, out error,
                                                   SshHostKey: Fingerprint, SshTrust: new SshTrustSettings(null, "record", null)),
                            Is.True, error);
                Assert.That(store.Connections.Single(entry => entry.Id == tolerant).SshOnPinMismatch, Is.EqualTo(SshOnPinMismatch.Record));
                Assert.That(store.Connections.Single(entry => entry.Id == tolerant).Warnings,         Has.Some.Contains("believed all the same"));

                Assert.That(store.TryAddConnection("Nonsense", AnSshURL, "CSMS", true, null, null, out _, out var nonsense,
                                                   SshTrust: new SshTrustSettings("Always", null, null)),
                            Is.False);
                Assert.That(nonsense, Does.Contain("'None' or 'HostKey'"));

                Assert.That(store.TryAddConnection("Short", AnSshURL, "CSMS", true, null, null, out _, out var tooShort,
                                                   SshTrust: new SshTrustSettings("HostKey", null, 0)),
                            Is.False);
                Assert.That(tooShort, Does.Contain("from 1 s"));

            });

            // An MD5 pin from before is kept, so that the connection is not lost, and said not to be dialled.
            var md5Store = TestNodes.TemporaryDirectory("ssh-connections-md5");

            try
            {

                Directory.CreateDirectory(md5Store);

                File.WriteAllText(Path.Combine(md5Store, ConnectionStore.ConnectionsFileName),
                                  $$"""[ { "id": "old", "description": "From before", "url": "{{AnSshURL}}", "connectionType": "CSMS", "sshHostKey": "MD5:16:27:ac:a5:76:28:2d:36:63:1b:56:4d:eb:df:a6:48" } ]""");

                var old = new ConnectionStore(md5Store).Connections.Single();

                Assert.Multiple(() => {
                    Assert.That(old.PinsMD5,   Is.True);
                    Assert.That(old.Warnings,  Has.Some.Contains("MD5 fingerprints are no longer taken"));
                });

            }
            finally
            {
                TestNodes.Remove(md5Store);
            }

        }

        #endregion

    }

}
