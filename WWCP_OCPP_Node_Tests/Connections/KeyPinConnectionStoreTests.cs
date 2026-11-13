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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The key pins of a wss:// connection ("Key Pins and Raw Public Keys",
    /// section 7.1): what is taken, what is refused, and what is written down
    /// by the station rather than by somebody - a key pin learned on first use,
    /// a signed pin set - and read back from peers.json.
    /// </summary>
    [TestFixture]
    public class KeyPinConnectionStoreTests
    {

        #region Data

        private String           directory  = "";
        private ConnectionStore  store      = default!;

        private static readonly KeyPin  pinA  = KeyPin.FromKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        private static readonly KeyPin  pinB  = KeyPin.FromKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));

        private const String  AWssURL  = "wss://192.168.10.5/ocpp/cs001";

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory  = TestNodes.TemporaryDirectory("keypin-connections");
            store      = new ConnectionStore(Path.Combine(directory, "conn"));
        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion

        #region (private) Helpers

        private ConnectionStore Reloaded()
            => new (Path.Combine(directory, "conn"));

        private String Added(String URL, String? Pins = null, ServerKeyPinSettings? Trust = null)
        {
            Assert.That(store.TryAddConnection("The local controller", URL, "LocalController", true, null, null,
                                               out var id, out var error, ServerKeyPins: Pins, KeyPinTrust: Trust),
                        Is.True, error);
            return id!;
        }

        private ConnectionEntry Entry
            => store.Connections.Single();

        #endregion


        #region KeyPinsAreTakenInAnyNotation_AndWrittenAsLowercaseHex()

        /// <summary>
        /// Key pins are taken as 64 hexadecimal digits with or without colons and
        /// in curl's notation, kept once each, written as 64 lowercase digits, and
        /// read back as they were written, with what goes with them.
        /// </summary>
        [Test]
        public void KeyPinsAreTakenInAnyNotation_AndWrittenAsLowercaseHex()
        {

            Added(AWssURL, $"{pinA.ToHexWithColons()}, {pinB.ToCurl()}\n{pinA.ToHex().ToUpperInvariant()}",
                  new ServerKeyPinSettings("Prefer", "Key", 120));

            var json = Entry.ToJSON();

            Assert.Multiple(() => {

                Assert.That(Entry.ServerKeyPins,                 Is.EqualTo(new[] { pinA, pinB }));
                Assert.That(Entry.IsKeyPinned,                   Is.True);
                Assert.That(json["serverKeyPins"]!.Values<String>(), Is.EqualTo(new[] { pinA.ToHex(), pinB.ToHex() }));
                Assert.That(json.Value<String>("serverKeyPinState"),          Is.EqualTo("Configured"));
                Assert.That(json.Value<String>("rawPublicKeys"),              Is.EqualTo("Prefer"));
                Assert.That(json.Value<String>("trustOnFirstUse"),            Is.EqualTo("Key"));
                Assert.That(json.Value<Int64>("serverKeyPinProbationTime"),   Is.EqualTo(120));
                Assert.That(json.Value<Boolean>("keyPinned"),                 Is.True);

                Assert.That(ConnectionEntry.TryParse(json, out var parsed, out var error), Is.True, error);
                Assert.That(parsed!.ServerKeyPins,               Is.EqualTo(new[] { pinA, pinB }));
                Assert.That(parsed.RawPublicKeys,                Is.EqualTo(RawPublicKeyMode.Prefer));
                Assert.That(parsed.TrustOnFirstUse,              Is.EqualTo(KeyTrustOnFirstUse.Key));
                Assert.That(parsed.ServerKeyPinProbationTime,    Is.EqualTo(TimeSpan.FromSeconds(120)));

                Assert.That(Reloaded().Connections.Single().ServerKeyPins, Is.EqualTo(new[] { pinA, pinB }), "not read back from peers.json");

            });

        }

        #endregion

        #region KeyPinsAreRefusedWhereThereIsNoTLSServerToCheck()

        /// <summary>
        /// Key pins, trust on first use of a key and raw public keys are refused
        /// on a ws://, http:// or ssh:// URL, as a host key is on a non-ssh one;
        /// their defaults are taken of every URL, and written of none of those.
        /// </summary>
        [TestCase("ws://192.168.10.5/ocpp/cs001")]
        [TestCase("http://192.168.10.5/ocpp/cs001")]
        [TestCase("ssh://cs001@192.168.10.5:2222")]
        public void KeyPinsAreRefusedWhereThereIsNoTLSServerToCheck(String URL)
        {

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("Pinned", URL, "LocalController", true, null, null, out _, out var error,
                                                   ServerKeyPins: pinA.ToHex()),
                            Is.False);
                Assert.That(error, Does.Contain("key pin"));

                Assert.That(store.TryAddConnection("Learning", URL, "LocalController", true, null, null, out _, out error,
                                                   KeyPinTrust: new ServerKeyPinSettings(null, "Key", null)),
                            Is.False);
                Assert.That(error, Does.Contain("Trust on first use of a key pin"));

                Assert.That(store.TryAddConnection("Raw", URL, "LocalController", true, null, null, out _, out error,
                                                   KeyPinTrust: new ServerKeyPinSettings("Prefer", null, null)),
                            Is.False);
                Assert.That(error, Does.Contain("Raw public keys are a way of a TLS handshake"));

                Assert.That(store.Connections, Is.Empty);

            });

            // As every form sends them: none, Off, None.
            var id   = Added(URL, "", new ServerKeyPinSettings("Off", "None", 600));
            var json = Entry.ToJSON();

            Assert.Multiple(() => {
                Assert.That(Entry.IsKeyPinned, Is.False);
                foreach (var field in new[] { "serverKeyPins", "rawPublicKeys", "trustOnFirstUse", "serverKeyPinState", "keyPinned" })
                    Assert.That(json[field], Is.Null, $"'{field}' was written of {URL}");
            });

            // Nor are they taken from a file.
            json["serverKeyPins"] = new JArray(pinA.ToHex());
            Assert.That(ConnectionEntry.TryParse(json, out _, out var refused), Is.False, $"key pins of {URL} were read from the file");
            Assert.That(refused, Does.Contain("key pin"));

        }

        #endregion

        #region WhatIsNoKeyPin_OrTooMany_OrRawWithoutAPin_IsRefused()

        [Test]
        public void WhatIsNoKeyPin_OrTooMany_OrRawWithoutAPin_IsRefused()
        {

            var five = String.Join(",", Enumerable.Range(0, 5).Select(_ => KeyPin.FromKey(ECDsa.Create(ECCurve.NamedCurves.nistP256)).ToHex()));

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("x", AWssURL, "CSMS", true, null, null, out _, out var error, ServerKeyPins: "SHA256:notapin"), Is.False);
                Assert.That(error, Does.Contain("is not a key pin"));

                Assert.That(store.TryAddConnection("x", AWssURL, "CSMS", true, null, null, out _, out error, ServerKeyPins: pinA.ToHex()[..62]), Is.False);
                Assert.That(error, Does.Contain("is not a key pin"));

                Assert.That(store.TryAddConnection("x", AWssURL, "CSMS", true, null, null, out _, out error, ServerKeyPins: five), Is.False);
                Assert.That(error, Does.Contain("at most 4"));

                // A CA-checked connection never accepts a raw key.
                Assert.That(store.TryAddConnection("x", AWssURL, "CSMS", true, null, null, out _, out error,
                                                   KeyPinTrust: new ServerKeyPinSettings("Require", "None", null)), Is.False);
                Assert.That(error, Does.Contain("only where the server is known by its key"));

                Assert.That(store.TryAddConnection("x", AWssURL, "CSMS", true, null, null, out _, out error,
                                                   KeyPinTrust: new ServerKeyPinSettings("Off", "Key", 0)), Is.False);
                Assert.That(error, Does.Contain("is not one"));

                Assert.That(store.Connections, Is.Empty);

            });

        }

        #endregion

        #region APeersFileWithoutKeyPins_GetsTheDefaults()

        /// <summary>
        /// A wss:// endpoint written down before there were key pins is checked
        /// by its CA, as it always was.
        /// </summary>
        [Test]
        public void APeersFileWithoutKeyPins_GetsTheDefaults()
        {

            var json = new JObject(
                           new JProperty("id",              "old"),
                           new JProperty("description",     "The CSMS"),
                           new JProperty("url",             "wss://csms.example.org/cs001"),
                           new JProperty("connectionType",  "CSMS"),
                           new JProperty("createdAt",       "2026-01-01T00:00:00.000Z")
                       );

            Assert.That(ConnectionEntry.TryParse(json, out var entry, out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(entry!.ServerKeyPins,              Is.Empty);
                Assert.That(entry.ServerKeyPinsVersion,        Is.Zero);
                Assert.That(entry.RawPublicKeys,               Is.EqualTo(RawPublicKeyMode.Off));
                Assert.That(entry.TrustOnFirstUse,             Is.EqualTo(KeyTrustOnFirstUse.None));
                Assert.That(entry.ServerKeyPinProbationTime,   Is.EqualTo(TimeSpan.FromMinutes(10)));
                Assert.That(entry.ServerKeyPinState,           Is.EqualTo(KeyPinState.Configured));
                Assert.That(entry.IsKeyPinned,                 Is.False, "an old endpoint is no longer checked by its CA");
            });

        }

        #endregion

        #region AKeyPinIsLearnedOnlyWhereThatIsConfigured()

        /// <summary>
        /// A key pin is learned only by a wss:// connection that trusts on first
        /// use, pins nothing and learned nothing yet, and still goes where it
        /// was dialled.
        /// </summary>
        [Test]
        public void AKeyPinIsLearnedOnlyWhereThatIsConfigured()
        {

            var caChecked  = Added(AWssURL);
            var pinned     = Added(AWssURL, pinB.ToHex(), new ServerKeyPinSettings(null, "Key", null));
            var learning   = Added(AWssURL, null,         new ServerKeyPinSettings(null, "Key", null));
            var url        = store.Connections.First().URL;

            Assert.Multiple(() => {

                Assert.That(store.TryLearnServerKeyPin(caChecked, url, pinA, out var error), Is.False, "learned where trust on first use is off");
                Assert.That(error, Is.Null);
                Assert.That(store.TryLearnServerKeyPin(pinned,    url, pinA, out error),     Is.False, "learned where a key is pinned");

                Assert.That(store.TryLearnServerKeyPin(learning,  org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse("wss://elsewhere.example.org/cs001"), pinA, out error),
                            Is.False, "learned for a URL the connection no longer goes to");

                Assert.That(store.TryLearnServerKeyPin(learning,  url, pinA, out error),     Is.True,  error);
                Assert.That(store.TryLearnServerKeyPin(learning,  url, pinB, out error),     Is.False, "learned twice");

            });

            var reloaded = Reloaded().Connections.ToDictionary(entry => entry.Id);

            Assert.Multiple(() => {
                Assert.That(reloaded[caChecked].ServerKeyPinLearned,  Is.Null);
                Assert.That(reloaded[pinned].   ServerKeyPinLearned,  Is.Null);
                Assert.That(reloaded[pinned].   ServerKeyPins,        Is.EqualTo(new[] { pinB }));
                Assert.That(reloaded[learning]. ServerKeyPinLearned,  Is.EqualTo(pinA), "the key pin learned was not read back from peers.json");
                Assert.That(reloaded[learning]. ServerKeyPinLearnedAt, Is.Not.Null);
                Assert.That(reloaded[learning]. ServerKeyPinState,    Is.EqualTo(KeyPinState.Learned));
                Assert.That(reloaded[learning]. ServerKeyPins,        Is.Empty, "a learned key pin was taken for a pin");
            });

        }

        #endregion

        #region ASignedPinSetIsTakenOnlyWithAHigherVersion()

        /// <summary>
        /// A signed pin set is taken only with a higher version than the one held,
        /// so that an older one cannot be brought back; one that has the key pin
        /// learned confirms it, one that has not replaces it.
        /// </summary>
        [Test]
        public void ASignedPinSetIsTakenOnlyWithAHigherVersion()
        {

            var id  = Added(AWssURL, null, new ServerKeyPinSettings(null, "Key", null));

            Assert.That(store.TryLearnServerKeyPin(id, Entry.URL, pinA, out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(store.TryTakeSignedServerKeyPins(id, pinA.ToHex(), 0, out _, out _, out error), Is.False, "a pin set of version 0 was taken");
                Assert.That(error, Does.Contain("only a higher one is taken"));
                Assert.That(store.TryTakeSignedServerKeyPins(id, "",           1, out _, out _, out error), Is.False, "an empty pin set was taken");
            });

            Assert.That(store.TryTakeSignedServerKeyPins(id, $"{pinA}, {pinB}", 3, out var learned, out var confirmed, out error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(learned,                     Is.EqualTo(pinA));
                Assert.That(confirmed,                   Is.True);
                Assert.That(Entry.ServerKeyPins,         Is.EqualTo(new[] { pinA, pinB }));
                Assert.That(Entry.ServerKeyPinsVersion,  Is.EqualTo(3));
                Assert.That(Entry.ServerKeyPinState,     Is.EqualTo(KeyPinState.Confirmed));
                Assert.That(Entry.ServerKeyPinLearned,   Is.Null);
                Assert.That(Reloaded().Connections.Single().ServerKeyPinState, Is.EqualTo(KeyPinState.Confirmed), "not read back as confirmed");
            });

            // A replay of it, or of an older one, is refused.
            Assert.Multiple(() => {
                Assert.That(store.TryTakeSignedServerKeyPins(id, pinB.ToHex(), 3, out _, out _, out error), Is.False, "the same version was taken again");
                Assert.That(store.TryTakeSignedServerKeyPins(id, pinB.ToHex(), 2, out _, out _, out error), Is.False, "an older version was taken");
                Assert.That(Entry.ServerKeyPins, Is.EqualTo(new[] { pinA, pinB }));
            });

            Assert.That(store.TryTakeSignedServerKeyPins(id, pinB.ToHex(), 4, out learned, out confirmed, out error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(learned,                     Is.Null);
                Assert.That(Entry.ServerKeyPins,         Is.EqualTo(new[] { pinB }));
                Assert.That(Entry.ServerKeyPinState,     Is.EqualTo(KeyPinState.Configured));
            });

        }

        /// <summary>
        /// A signed pin set without the key pin learned replaces it: the server
        /// that answered first is not the one the CSMS knows.
        /// </summary>
        [Test]
        public void ASignedPinSetWithoutTheLearnedKeyPin_ReplacesIt()
        {

            var id  = Added(AWssURL, null, new ServerKeyPinSettings(null, "Key", null));

            Assert.That(store.TryLearnServerKeyPin(id, Entry.URL, pinA, out var error), Is.True, error);
            Assert.That(store.TryTakeSignedServerKeyPins(id, pinB.ToHex(), 1, out var learned, out var confirmed, out error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(learned,                     Is.EqualTo(pinA));
                Assert.That(confirmed,                   Is.False);
                Assert.That(Entry.ServerKeyPins,         Is.EqualTo(new[] { pinB }));
                Assert.That(Entry.ServerKeyPinLearned,   Is.Null);
                Assert.That(Entry.ServerKeyPinState,     Is.EqualTo(KeyPinState.Configured));
            });

        }

        #endregion

        #region ASaveOfSomethingElse_KeepsThePinsOfASignedPinSet()

        /// <summary>
        /// A save of the description from a page drawn before a signed pin set was
        /// taken keeps the signed pins; key pins typed anew replace them, and the
        /// version and what was learned go with them. A URL that is no longer
        /// wss:// takes them away.
        /// </summary>
        [Test]
        public void ASaveOfSomethingElse_KeepsThePinsOfASignedPinSet()
        {

            var id  = Added(AWssURL, pinA.ToHex());

            Assert.That(store.TryTakeSignedServerKeyPins(id, pinB.ToHex(), 7, out _, out _, out var error), Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "Renamed", AWssURL, "LocalController", true, null, null, out error,
                                                  ServerKeyPins: pinA.ToHex(), ServerKeyPinsAsShown: pinA.ToHexWithColons(),
                                                  KeyPinTrust: new ServerKeyPinSettings("Off", "None", 600)),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Entry.Description,           Is.EqualTo("Renamed"));
                Assert.That(Entry.ServerKeyPins,         Is.EqualTo(new[] { pinB }), "a signed pin set was undone by a save of something else");
                Assert.That(Entry.ServerKeyPinsVersion,  Is.EqualTo(7));
            });

            Assert.That(store.TryUpdateConnection(id, "Renamed", AWssURL, "LocalController", true, null, null, out error,
                                                  ServerKeyPins: pinA.ToHex(), ServerKeyPinsAsShown: pinB.ToHex()),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Entry.ServerKeyPins,         Is.EqualTo(new[] { pinA }));
                Assert.That(Entry.ServerKeyPinsVersion,  Is.Zero, "the version of a signed pin set stayed with pins somebody typed");
            });

            Assert.That(store.TryUpdateConnection(id, "Renamed", "ws://192.168.10.5/ocpp/cs001", "LocalController", true, null, null, out error),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(Entry.ServerKeyPins,         Is.Empty, "key pins stayed with a connection without TLS");
                Assert.That(Entry.IsKeyPinned,           Is.False);
            });

        }

        #endregion

        #region TheWarningsSayWhatIsLearnedAndWhatCannotBeDone()

        [Test]
        public void TheWarningsSayWhatIsLearnedAndWhatCannotBeDone()
        {

            var requires  = Added(AWssURL, pinA.ToHex(), new ServerKeyPinSettings("Require", null, null));
            var learning  = Added(AWssURL, null,         new ServerKeyPinSettings(null, "Key", null));

            var warnings  = store.Connections.ToDictionary(entry => entry.Id, entry => String.Join("\n", entry.Warnings));

            Assert.Multiple(() => {
                Assert.That(warnings[requires], Does.Contain("requires raw public keys").And.Contain("a server that shows a certificate is turned away"));
                Assert.That(warnings[learning], Does.Contain("No key is pinned, so the first server that answers is believed"));
            });

            Assert.That(store.TryLearnServerKeyPin(learning, store.Connections.First().URL, pinA, out var error), Is.True, error);

            Assert.That(String.Join("\n", store.Connections.Single(entry => entry.Id == learning).Warnings),
                        Does.Contain($"The key pin {pinA} was learned on the first connection and is on probation"));

        }

        #endregion

    }

}
