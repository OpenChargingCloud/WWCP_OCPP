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

using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// What the other end of a connection it dials signs in with: a client
    /// certificate - known by the key pin of its key, or by its CA - or
    /// credentials, and never both. Written down under names of its own, and
    /// never mistaken for what a server this node dials is held to.
    /// </summary>
    [TestFixture]
    public class InboundCertificateStoreTests
    {

        #region Data

        private const String  Name           = "csms-01";
        private const String  InboundURL     = "wss://this-controller.example.org:9443/" + Name;
        private const String  APassword      = "what-the-csms-signs-in-with-here";
        private const String  ASharedSecret  = "shared-secret-not-the-real-one";

        private String            directory  = "";
        private String            path       = "";
        private ConnectionStore   store      = default!;
        private MovableClock      clock      = default!;
        private ECDsa             csmsKey    = default!;
        private String            csmsPin    = "";

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory  = TestNodes.TemporaryDirectory("connections");
            path       = Path.Combine(directory, "conn");
            clock      = new MovableClock(DateTimeOffset.UtcNow);
            store      = new ConnectionStore(path, clock);
            csmsKey    = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            csmsPin    = KeyPin.FromKey(csmsKey).ToHex();
        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            csmsKey.Dispose();
            TestNodes.Remove(directory);
        }

        #endregion

        #region (private) helpers

        private String ALogin(String Kind = "basic", Boolean Bound = false)
        {

            Assert.That(store.TryAddAuthentication("What the CSMS signs in with", Kind, Name, Kind == "basic" ? APassword : ASharedSecret,
                                                   out var id, out var error,
                                                   TLSChannelBinding: Kind == "totp" ? Bound : null),
                        Is.True, error);

            return id!;

        }

        private Boolean TryAddInbound(String?            AuthenticationId,
                                      out String?        Id,
                                      out String?        Error,
                                      String?            KeyPins  = null,
                                      ServerPins?        Pins     = null,
                                      String             URL      = InboundURL)

            => store.TryAddConnection("The CSMS, dialling in", URL, "CSMS", true, AuthenticationId, null,
                                      out Id, out Error,
                                      ServerKeyPins:  KeyPins,
                                      Pins:           Pins is null ? null : new ServerPinsChange(Pins),
                                      Inbound:        true);

        private static ServerPins OfGroup(String Group)
            => new ([], [], RootGroup: Group);

        private ConnectionEntry Entry(String Id)
            => store.Connections.Single(one => one.Id == Id);

        #endregion


        #region ByItsKeyPin_WithoutCredentials_AndWrittenAsTheClientsKeyPins()

        /// <summary>
        /// A CSMS that signs in with a certificate known by its key: no
        /// credentials, and the key pins are those of its client certificate -
        /// under "clientKeyPins" in the file and to the page, never as the key
        /// pins of a server, and read back as such.
        /// </summary>
        [Test]
        public void ByItsKeyPin_WithoutCredentials_AndWrittenAsTheClientsKeyPins()
        {

            Assert.That(TryAddInbound(null, out var id, out var error, KeyPins: csmsPin), Is.True, error);

            var entry  = Entry(id!);
            var json   = entry.ToJSON();

            Assert.Multiple(() => {
                Assert.That(entry.SignsInWithACertificate,                     Is.True);
                Assert.That(entry.ClientKeyPins.Select(pin => pin.ToHex()),    Is.EqualTo(new[] { csmsPin }));
                Assert.That(entry.IsKeyPinned,                                 Is.False, "the key of the other end's client certificate taken for one of a server");
                Assert.That(json[ConnectionEntry.ClientKeyPinsKey]?.Values<String>(),  Is.EqualTo(new[] { csmsPin }));
                Assert.That(json["serverKeyPins"],                             Is.Null, "written down as the key pins of a server");
                Assert.That(json["keyPinned"],                                 Is.Null);
                Assert.That(entry.Warnings,                                    Has.None.Contains("names no credentials"));
            });

            var again = new ConnectionStore(path, clock).Connections.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(again.Inbound,                                     Is.True);
                Assert.That(again.ClientKeyPins.Select(pin => pin.ToHex()),    Is.EqualTo(new[] { csmsPin }), "not read back from the file");
            });

        }

        #endregion

        #region ByItsCA_HeldToAGroupOfRoots()

        /// <summary>
        /// A CSMS that signs in with a certificate checked by its CA: held to
        /// a group of roots of the store, under "clientPins".
        /// </summary>
        [Test]
        public void ByItsCA_HeldToAGroupOfRoots()
        {

            Assert.That(TryAddInbound(null, out var id, out var error, Pins: OfGroup("operator")), Is.True, error);

            var entry = Entry(id!);

            Assert.Multiple(() => {
                Assert.That(entry.SignsInWithACertificate,                                  Is.True);
                Assert.That(entry.ClientPins?.RootGroup,                                    Is.EqualTo("operator"));
                Assert.That(entry.ToJSON()[ConnectionEntry.ClientPinsKey]?["rootGroup"]?.Value<String>(),  Is.EqualTo("operator"));
                Assert.That(entry.ToJSON()[ConnectionEntry.PinsKey],                        Is.Null, "written down as what a server is held to");
            });

            Assert.That(new ConnectionStore(path, clock).Connections.Single(one => one.Id == id).ClientPins?.RootGroup,
                        Is.EqualTo("operator"), "not read back from the file");

        }

        #endregion

        #region ACertificateAndCredentials_AreNeverBoth()

        /// <summary>
        /// One or the other: credentials beside a certificate would be the
        /// easier way in - refused when written down, and when changed into.
        /// </summary>
        [Test]
        public void ACertificateAndCredentials_AreNeverBoth()
        {

            var login = ALogin();

            Assert.Multiple(() => {

                Assert.That(TryAddInbound(login, out _, out var byKey, KeyPins: csmsPin),            Is.False, "a key pin and a password");
                Assert.That(byKey,                                                                    Does.Contain("does not sign in with credentials as well"));

                Assert.That(TryAddInbound(login, out _, out var byCA,  Pins: OfGroup("operator")),   Is.False, "a group of roots and a password");
                Assert.That(byCA,                                                                     Does.Contain("does not sign in with credentials as well"));

            });

            Assert.That(TryAddInbound(login, out var id, out var error), Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "The CSMS, dialling in", InboundURL, "CSMS", true, login, null, out var changed,
                                                  ServerKeyPins: csmsPin),
                        Is.False, "a password, and then a key pin beside it");

            Assert.That(changed, Does.Contain("does not sign in with credentials as well"));

        }

        #endregion

        #region NeitherACertificateNorCredentials_LetsNothingIn()

        [Test]
        public void NeitherACertificateNorCredentials_LetsNothingIn()
        {

            Assert.That(TryAddInbound(null, out _, out var error), Is.False);
            Assert.That(error, Does.Contain("Nothing gets in without"));

        }

        #endregion

        #region ByItsCA_IsHeldToNamedRoots_AndLearnsNothing()

        /// <summary>
        /// A client certificate checked by its CA is held to roots the
        /// connection names - never to whatever root the machine trusts -,
        /// learns nothing on first use, and lets nothing in that does not
        /// match.
        /// </summary>
        [Test]
        public void ByItsCA_IsHeldToNamedRoots_AndLearnsNothing()
        {

            var someCertificate = new String('a', 64);

            Assert.Multiple(() => {

                Assert.That(TryAddInbound(null, out _, out var anyRoot, Pins: new ServerPins([ someCertificate ], [])),                  Is.False, "no root named");
                Assert.That(anyRoot,                                                                                                    Does.Contain("name the roots its chain may end at"));

                Assert.That(TryAddInbound(null, out _, out var learns,  Pins: new ServerPins([], [], TrustOnFirstUse: TrustOnFirstUse.Root, RootGroup: "operator")), Is.False, "learning on first use");
                Assert.That(learns,                                                                                                     Does.Contain("Nothing is learned"));

                Assert.That(TryAddInbound(null, out _, out var accepts, Pins: new ServerPins([], [], OnMismatch: PinMismatch.Accept, RootGroup: "operator")),     Is.False, "a mismatch let in");
                Assert.That(accepts,                                                                                                    Does.Contain("is refused"));

            });

        }

        #endregion

        #region ACertificateNeedsTLS()

        [Test]
        public void ACertificateNeedsTLS()
        {

            Assert.That(TryAddInbound(null, out _, out var error, KeyPins: csmsPin, URL: "ws://this-controller.example.org:9000/" + Name), Is.False);
            Assert.That(error, Does.Contain("makes no TLS handshake"));

        }

        #endregion

        #region AOneTimePasswordBoundToTheTLSSession_IsTaken_ForHTTP3()

        /// <summary>
        /// Refused before: the server did not check one. Taken now, and the
        /// connection says that it lets in over HTTP/3 only.
        /// </summary>
        [Test]
        public void AOneTimePasswordBoundToTheTLSSession_IsTaken_ForHTTP3()
        {

            var login = ALogin("totp", Bound: true);

            Assert.That(TryAddInbound(login, out var id, out var error), Is.True, error);

            Assert.That(Entry(id!).Warnings, Has.Some.Contains("over HTTP/3").And.None.Contains("does not check yet"));

        }

        #endregion

        #region TurnedAround_WhatWasKeptOfTheServer_IsNotTheClients()

        /// <summary>
        /// A connection this node dialled, its server known by a key pin, is
        /// turned into one the other end dials, with the form sent back as it
        /// showed it: the key pin of the server is not taken for the key of
        /// the other end's client certificate.
        /// </summary>
        [Test]
        public void TurnedAround_WhatWasKeptOfTheServer_IsNotTheClients()
        {

            var login = ALogin();

            Assert.That(store.TryAddConnection("The CSMS", "wss://csms.example.org/" + Name, "CSMS", true, login, null,
                                               out var id, out var error,
                                               ServerKeyPins: csmsPin),
                        Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "The CSMS", InboundURL, "CSMS", true, login, null, out error, out _,
                                                  ServerKeyPins:         csmsPin,
                                                  ServerKeyPinsAsShown:  csmsPin,
                                                  Inbound:               true),
                        Is.True, error);

            var entry = Entry(id!);

            Assert.Multiple(() => {
                Assert.That(entry.Inbound,                  Is.True);
                Assert.That(entry.ClientKeyPins,            Is.Empty, "the key pin of its server became the key of its client certificate");
                Assert.That(entry.SignsInWithACertificate,  Is.False);
            });

        }

        #endregion

        #region SignedPinSets_AreNotForAConnectionTheOtherEndDials()

        /// <summary>
        /// A signed pin set pins the server of a connection this node dials.
        /// This node is the server of one the other end dials: the pin set
        /// does not reach the keys the other end signs in with.
        /// </summary>
        [Test]
        public void SignedPinSets_AreNotForAConnectionTheOtherEndDials()
        {

            Assert.That(TryAddInbound(null, out var id, out var error, KeyPins: csmsPin), Is.True, error);

            using var another = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            Assert.Multiple(() => {

                Assert.That(store.TryTakeSignedServerKeyPins(id!, KeyPin.FromKey(another).ToHex(), 1, out _, out _, out var keyPins),  Is.False, "the key pins of its client certificate replaced");
                Assert.That(keyPins,                                                                                                   Does.Contain("dialled by the other end"));

                Assert.That(store.TryTakeSignedServerCAKeyPins(id!, KeyPin.FromKey(another).ToHex(), 1, out var roots),               Is.False, "a root taken");
                Assert.That(roots,                                                                                                     Does.Contain("dialled by the other end"));

                Assert.That(Entry(id!).ClientKeyPins.Select(pin => pin.ToHex()),                                                       Is.EqualTo(new[] { csmsPin }));

            });

        }

        #endregion

        #region AFile_KeepsEachDirectionsNames()

        /// <summary>
        /// A file that writes the key pins of a server beside a connection the
        /// other end dials - or those of a client beside one this node dials -
        /// says something nobody checks: refused with the names to use.
        /// </summary>
        [Test]
        public void AFile_KeepsEachDirectionsNames()
        {

            var inbound   = JObject.Parse($$"""{ "id": "c1", "description": "in",  "url": "{{InboundURL}}",          "connectionType": "CSMS", "inbound": true, "serverKeyPins": [ "{{csmsPin}}" ] }""");
            var outbound  = JObject.Parse($$"""{ "id": "c2", "description": "out", "url": "wss://csms.example.org/x", "connectionType": "CSMS",                  "clientKeyPins": [ "{{csmsPin}}" ] }""");

            Assert.Multiple(() => {

                Assert.That(ConnectionEntry.TryParse(inbound,  out _, out var wrongIn),   Is.False);
                Assert.That(wrongIn,                                                      Does.Contain(ConnectionEntry.ClientKeyPinsKey));

                Assert.That(ConnectionEntry.TryParse(outbound, out _, out var wrongOut),  Is.False);
                Assert.That(wrongOut,                                                     Does.Contain("serverKeyPins"));

            });

        }

        #endregion

    }

}
