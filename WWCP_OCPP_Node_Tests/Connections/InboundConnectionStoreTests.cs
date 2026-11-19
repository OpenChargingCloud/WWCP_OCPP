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
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// What a connection the other end dials says of what the other end signs
    /// in with: nothing. The other end signs in with the WebSocket login of the
    /// name it comes under - its password, one-time password, key pin, or a
    /// certificate issued to that name -, as whoever else signs in at the
    /// station port. Credentials of this node, the key pins and the CA of a
    /// client certificate on the connection itself are refused, in the file as
    /// on a page, with what to do instead.
    /// </summary>
    [TestFixture]
    public class InboundConnectionStoreTests
    {

        #region Data

        private const String  Name           = "csms-01";
        private const String  InboundURL     = "wss://this-controller.example.org:9443/" + Name;
        private const String  APassword      = "what-the-csms-signs-in-with-here";

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

        private String ALogin()
        {

            Assert.That(store.TryAddBasicAuthentication(Name, APassword, "What this node signs in with", out var id, out var error),
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


        #region NamesNothingOfItsOwn_TheOtherEndSignsInWithItsLogin()

        /// <summary>
        /// Written down with nothing it signs in with, and read back so: the
        /// name it comes under is the name of its WebSocket login.
        /// </summary>
        [Test]
        public void NamesNothingOfItsOwn_TheOtherEndSignsInWithItsLogin()
        {

            Assert.That(TryAddInbound(null, out var id, out var error), Is.True, error);

            var entry  = Entry(id!);
            var json   = entry.ToJSON();

            Assert.Multiple(() => {
                Assert.That(entry.Inbound,                                  Is.True);
                Assert.That(entry.InboundName,                              Is.EqualTo(Name));
                Assert.That(entry.AuthenticationId,                         Is.Null);
                Assert.That(entry.ServerKeyPins,                            Is.Empty);
                Assert.That(entry.IsKeyPinned,                              Is.False);
                Assert.That(json["authenticationId"],                       Is.Null);
                Assert.That(json[ConnectionEntry.ClientKeyPinsKey],         Is.Null);
                Assert.That(json[ConnectionEntry.ClientPinsKey],            Is.Null);
                Assert.That(json["serverKeyPins"],                          Is.Null);
                Assert.That(entry.Warnings,                                 Has.None.Contains("names no credentials"));
            });

            var again = new ConnectionStore(path, clock).Connections.Single(one => one.Id == id);

            Assert.Multiple(() => {
                Assert.That(again.Inbound,      Is.True,  "not read back from the file");
                Assert.That(again.InboundName,  Is.EqualTo(Name));
            });

        }

        #endregion

        #region CredentialsOfThisNode_AreNotWhatTheOtherEndSignsInWith()

        /// <summary>
        /// Credentials of this node are what it proves itself with where it
        /// dials: refused on a connection the other end makes, written down and
        /// changed into, with the login to say it on instead.
        /// </summary>
        [Test]
        public void CredentialsOfThisNode_AreNotWhatTheOtherEndSignsInWith()
        {

            var login = ALogin();

            Assert.That(TryAddInbound(login, out _, out var error), Is.False);
            Assert.That(error, Does.Contain("signs in with its WebSocket login"));

            Assert.That(TryAddInbound(null, out var id, out error), Is.True, error);

            Assert.That(store.TryUpdateConnection(id, "The CSMS, dialling in", InboundURL, "CSMS", true, login, null, out var changed),
                        Is.False, "credentials given to it afterwards");

            Assert.That(changed, Does.Contain("signs in with its WebSocket login"));

        }

        #endregion

        #region KeyPinsAndCAs_AreSaidOnTheLoginAndTheAcceptedChains()

        /// <summary>
        /// The key pins of the client certificate of the other end, and the CAs
        /// it may lead to: said on its WebSocket login and under the accepted
        /// chains of the station port, and refused on the connection.
        /// </summary>
        [Test]
        public void KeyPinsAndCAs_AreSaidOnTheLoginAndTheAcceptedChains()
        {

            Assert.Multiple(() => {

                Assert.That(TryAddInbound(null, out _, out var byKey, KeyPins: csmsPin),            Is.False, "a key pin");
                Assert.That(byKey,                                                                   Does.Contain("said there"));

                Assert.That(TryAddInbound(null, out _, out var byCA,  Pins: OfGroup("operator")),   Is.False, "a group of roots");
                Assert.That(byCA,                                                                    Does.Contain("Accepted chains"));

            });

        }

        #endregion

        #region TurnedAround_WhatWasKeptOfTheServer_IsNotTaken()

        /// <summary>
        /// A connection this node dialled, its server known by a key pin and
        /// signed in to with credentials of this node, is turned into one the
        /// other end dials: refused while it keeps either, and taken once both
        /// are left out - with nothing of them kept.
        /// </summary>
        [Test]
        public void TurnedAround_WhatWasKeptOfTheServer_IsNotTaken()
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
                        Is.False, "turned around with its credentials and the key pin of its server");

            Assert.That(store.TryUpdateConnection(id, "The CSMS", InboundURL, "CSMS", true, null, null, out error, out _,
                                                  Inbound: true),
                        Is.True, error);

            var entry = Entry(id!);

            Assert.Multiple(() => {
                Assert.That(entry.Inbound,           Is.True);
                Assert.That(entry.ServerKeyPins,     Is.Empty, "the key pin of its server kept");
                Assert.That(entry.AuthenticationId,  Is.Null,  "its credentials kept");
            });

        }

        #endregion

        #region SignedPinSets_AreNotForAConnectionTheOtherEndDials()

        /// <summary>
        /// A signed pin set pins the server of a connection this node dials.
        /// This node is the server of one the other end dials: the pin set
        /// does not reach it.
        /// </summary>
        [Test]
        public void SignedPinSets_AreNotForAConnectionTheOtherEndDials()
        {

            Assert.That(TryAddInbound(null, out var id, out var error), Is.True, error);

            using var another = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            Assert.Multiple(() => {

                Assert.That(store.TryTakeSignedServerKeyPins(id!, KeyPin.FromKey(another).ToHex(), 1, out _, out _, out var keyPins),  Is.False, "key pins taken");
                Assert.That(keyPins,                                                                                                   Does.Contain("dialled by the other end"));

                Assert.That(store.TryTakeSignedServerCAKeyPins(id!, KeyPin.FromKey(another).ToHex(), 1, out var roots),               Is.False, "a root taken");
                Assert.That(roots,                                                                                                     Does.Contain("dialled by the other end"));

                Assert.That(Entry(id!).ServerKeyPins,                                                                                  Is.Empty);

            });

        }

        #endregion

        #region AFileOfBefore_IsRefused_WithWhatToDo()

        /// <summary>
        /// A file of before, whose connection the other end makes named what
        /// the other end signs in with - credentials, the key pins or the CA of
        /// its client certificate -, is refused rather than read as if it were
        /// obeyed, with the WebSocket login to say it on. A connection this node
        /// dials keeps the names of a server.
        /// </summary>
        [Test]
        public void AFileOfBefore_IsRefused_WithWhatToDo()
        {

            JObject Inbound(String Says)
                => JObject.Parse($$"""{ "id": "c1", "description": "in", "url": "{{InboundURL}}", "connectionType": "CSMS", "inbound": true, {{Says}} }""");

            var outbound = JObject.Parse($$"""{ "id": "c2", "description": "out", "url": "wss://csms.example.org/x", "connectionType": "CSMS", "clientKeyPins": [ "{{csmsPin}}" ] }""");

            Assert.Multiple(() => {

                Assert.That(ConnectionEntry.TryParse(Inbound("\"authenticationId\": \"a1\""),                                     out _, out var credentials),  Is.False);
                Assert.That(credentials,                                                                                          Does.Contain("'authenticationId'").And.Contain("WebSocket login"));

                Assert.That(ConnectionEntry.TryParse(Inbound($$"""{{"\""}}clientKeyPins": [ "{{csmsPin}}" ]"""),                  out _, out var keyPins),      Is.False);
                Assert.That(keyPins,                                                                                              Does.Contain($"'{ConnectionEntry.ClientKeyPinsKey}'").And.Contain("WebSocket login"));

                Assert.That(ConnectionEntry.TryParse(Inbound("\"clientPins\": { \"rootGroup\": \"operator\" }"),                  out _, out var roots),        Is.False);
                Assert.That(roots,                                                                                                Does.Contain($"'{ConnectionEntry.ClientPinsKey}'").And.Contain("Accepted chains"));

                Assert.That(ConnectionEntry.TryParse(outbound,                                                                     out _, out var wrongOut),     Is.False);
                Assert.That(wrongOut,                                                                                             Does.Contain("serverKeyPins"));

                Assert.That(ConnectionEntry.TryParse(Inbound("\"autoConnect\": true"),                                            out var plain, out var none), Is.True, none);
                Assert.That(plain!.InboundName,                                                                                   Is.EqualTo(Name));

            });

        }

        #endregion

    }

}
