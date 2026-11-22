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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;


using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The key pins charging stations are let in by: listed per station, one
    /// pin for one station across all of them, read in every notation, and
    /// read back as the next start reads them.
    /// </summary>
    public class StationKeyPinTests
    {

        #region Data

        private String                 directory  = default!;
        private TestClock              clock      = default!;
        private WebSocketLogins  logins     = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAFile()
        {

            directory = ServingTestDirectories.TemporaryDirectory("stationpins");
            Directory.CreateDirectory(directory);

            clock     = TestClock.At(2026, 6, 1);
            logins    = new WebSocketLogins(Path.Combine(directory, "websocket-logins.json"), clock);

            Assert.That(logins.TryLoad(out var error), Is.True, error);

        }

        [TearDown]
        public void RemoveTheDirectory()
            => ServingTestDirectories.Remove(directory);

        #endregion

        #region (private static) APin()

        /// <summary>
        /// The key pin of a key of its own.
        /// </summary>
        private static KeyPin APin()
        {
            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            return KeyPin.FromKey(key);
        }

        #endregion


        #region AKeyPinIsListedAndReadBack()

        /// <summary>
        /// A pin given to a station that is not listed yet lists it, in the
        /// default group, and is there after a restart with what it is called
        /// and who gave it.
        /// </summary>
        [Test]
        public void AKeyPinIsListedAndReadBack()
        {

            var pin = APin();

            Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.ToHex(), "bay 1", null, I18NString.Create("Ladepunkt 1"), "admin", out var added, out var error),
                        Is.True, error);

            var restarted = new WebSocketLogins(Path.Combine(directory, "websocket-logins.json"), clock);

            Assert.That(restarted.TryLoad(out var loadError), Is.True, loadError);

            Assert.That(restarted.TryGet(User_Id.Parse("cs001"), out var login), Is.True, "the station was not listed");

            Assert.Multiple(() => {
                Assert.That(added!.Pin,                               Is.EqualTo(pin));
                Assert.That(login!.GroupId,                           Is.EqualTo(WebSocketLoginGroup.DefaultId));
                Assert.That(login.Description?.FirstText(),                               Is.EqualTo("Ladepunkt 1"));
                Assert.That(login.Pins.Select(one => one.Pin),        Is.EqualTo(new[] { pin }));
                Assert.That(login.Pins.Single().Label,                Is.EqualTo("bay 1"));
                Assert.That(login.Pins.Single().AddedBy,              Is.EqualTo("admin"));
                Assert.That(login.Pins.Single().Added,                Is.EqualTo(clock.Now));
                Assert.That(login.Credentials,                        Does.Contain(AuthMethod.KeyPin));
                Assert.That(restarted.FindKeyPin(pin)?.Login,            Is.EqualTo(User_Id.Parse("cs001")));
                Assert.That(restarted.WasEverListed(pin),             Is.True);
                Assert.That(restarted.GroupOf(login)!.Allows(AuthMethod.KeyPin), Is.True, "the default group takes no key pins");
            });

        }

        #endregion

        #region EveryNotationOfAPinIsThePin()

        /// <summary>
        /// Hexadecimal with and without colons, and curl's "sha256//" with
        /// base64, are the same pin: given in one, it is known in the others -
        /// and refused a second time in any of them.
        /// </summary>
        [Test]
        public void EveryNotationOfAPinIsThePin()
        {

            var pin = APin();

            Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.ToCurl(), null, null, null, null, out _, out var error), Is.True, error);

            var withColons  = logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.ToHexWithColons(),        null, null, null, null, out _, out var twiceColons);
            var upperCase   = logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.ToHex().ToUpperInvariant(), null, null, null, null, out _, out var twiceUpper);

            Assert.Multiple(() => {
                Assert.That(logins.FindKeyPin(pin)?.Login,  Is.EqualTo(User_Id.Parse("cs001")));
                Assert.That(withColons,                  Is.False);
                Assert.That(twiceColons,                 Does.Contain("has the key pin"));
                Assert.That(upperCase,                   Is.False);
                Assert.That(twiceUpper,                  Does.Contain("has the key pin"));
                Assert.That(logins.ToJSON()["logins"]?[0]?["keyPins"]?[0]?.Value<String>("pin"), Is.EqualTo(pin.ToHex()));
            });

        }

        #endregion

        #region OnePinIsOneStationAcrossAll()

        /// <summary>
        /// A pin that lets one station in is refused for another, in whatever
        /// notation: a certificate accepted by its pin is the station the pin is
        /// listed for, so a shared pin would let either be the other.
        /// </summary>
        [Test]
        public void OnePinIsOneStationAcrossAll()
        {

            var pin = APin();

            Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.ToHex(), null, null, null, null, out _, out var error), Is.True, error);

            var forAnother = logins.TryAddKeyPin(User_Id.Parse("cs002"), pin.ToHexWithColons(), null, null, null, null, out _, out var refused);

            Assert.Multiple(() => {
                Assert.That(forAnother,                     Is.False);
                Assert.That(refused,                        Does.Contain("already lets the charging station 'cs001' in"));
                Assert.That(logins.TryGet(User_Id.Parse("cs002"), out _),  Is.False, "the station was listed although its pin was refused");
                Assert.That(logins.FindKeyPin(pin)?.Login,     Is.EqualTo(User_Id.Parse("cs001")));
            });

        }

        /// <summary>
        /// The same for a file written by hand: two stations with one pin is a
        /// file that is refused, not one of the two quietly winning.
        /// </summary>
        [Test]
        public void AFileWithOnePinForTwoStationsIsRefused()
        {

            var pin  = APin().ToHex();
            var path = Path.Combine(directory, "websocket-logins.json");

            File.WriteAllText(path, new JObject(
                new JProperty("logins", new JArray(
                    new JObject(new JProperty("login", "cs001"), new JProperty("keyPins", new JArray(new JObject(new JProperty("pin", pin))))),
                    new JObject(new JProperty("login", "cs002"), new JProperty("keyPins", new JArray(new JObject(new JProperty("pin", pin)))))
                ))
            ).ToString());

            var shared = new WebSocketLogins(path, clock);

            Assert.Multiple(() => {
                Assert.That(shared.TryLoad(out var error), Is.False);
                Assert.That(error,                         Does.Contain("listed for the charging stations 'cs001' and 'cs002'"));
            });

        }

        #endregion

        #region AtMostFourPinsAStation()

        [Test]
        public void AtMostFourPinsAStation()
        {

            for (var i = 0; i < WebSocketLogin.MaxKeyPins; i++)
                Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), APin().ToHex(), null, null, null, null, out _, out var error), Is.True, error);

            var fifth = logins.TryAddKeyPin(User_Id.Parse("cs001"), APin().ToHex(), null, null, null, null, out _, out var refused);

            Assert.Multiple(() => {
                Assert.That(fifth,                                      Is.False);
                Assert.That(refused,                                    Does.Contain("at most 4 key pins"));
                Assert.That(logins.Logins.Single().Pins.Count,          Is.EqualTo(4));
            });

        }

        #endregion

        #region APinIsTakenAwayAndRemembered()

        /// <summary>
        /// A pin taken away is nobody's any more - and remembered as having
        /// been listed, for the moment between a handshake it let through and
        /// the upgrade after it.
        /// </summary>
        [Test]
        public void APinIsTakenAwayAndRemembered()
        {

            var pin   = APin();
            var other = APin();

            Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), pin.  ToHex(), null, null, null, null, out _, out var error),      Is.True, error);
            Assert.That(logins.TryAddKeyPin(User_Id.Parse("cs001"), other.ToHex(), null, null, null, null, out _, out var otherError), Is.True, otherError);

            Assert.That(logins.TryRemoveKeyPin(User_Id.Parse("cs001"), pin.ToHexWithColons(), out var removeError), Is.True, removeError);

            var again = logins.TryRemoveKeyPin(User_Id.Parse("cs001"), pin.ToHex(), out var notThere);

            Assert.Multiple(() => {
                Assert.That(logins.FindKeyPin(pin),                      Is.Null);
                Assert.That(logins.FindKeyPin(other)?.Login,                Is.EqualTo(User_Id.Parse("cs001")));
                Assert.That(logins.WasEverListed(pin),                   Is.True);
                Assert.That(logins.WasEverListed(APin()),                Is.False);
                Assert.That(again,                                       Is.False);
                Assert.That(notThere,                                    Does.Contain("has no key pin"));
            });

        }

        #endregion

        #region WhatIsNoPinIsRefused()

        /// <summary>
        /// A pin is SHA-256 over a key's SubjectPublicKeyInfo: too short, not
        /// hexadecimal, or an SSH fingerprint, which hashes something else
        /// entirely, is not one.
        /// </summary>
        [TestCase("")]
        [TestCase("abcdef")]
        [TestCase("zz112233445566778899aabbccddeeff00112233445566778899aabbccddeeff")]
        [TestCase("SHA256:47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU")]
        public void WhatIsNoPinIsRefused(String NoPin)
        {

            var added = logins.TryAddKeyPin(User_Id.Parse("cs001"), NoPin, null, null, null, null, out _, out var error);

            Assert.Multiple(() => {
                Assert.That(added,                          Is.False);
                Assert.That(error,                          Does.Contain("key pin"));
                Assert.That(logins.TryGet(User_Id.Parse("cs001"), out _),  Is.False);
            });

        }

        #endregion

        #region ADefaultGroupOfAnOlderFileKeepsWhatItLists()

        /// <summary>
        /// As for SSH keys: a default group written before key pins existed
        /// accepts none until somebody adds them - the file said what it takes.
        /// </summary>
        [Test]
        public void ADefaultGroupOfAnOlderFileKeepsWhatItLists()
        {

            var path = Path.Combine(directory, "websocket-logins.json");

            File.WriteAllText(path, new JObject(
                new JProperty("groups", new JArray(
                    new JObject(new JProperty("id", "default"), new JProperty("name", "Default"),
                                new JProperty("authMethods", new JArray("basic", "certificate")),
                                new JProperty("clientAuthentication", new JArray("httpAuthWithoutTLS", "httpAuthOverTLS", "tlsClientCertificate")))
                )),
                new JProperty("logins", new JArray())
            ).ToString());

            var older = new WebSocketLogins(path, clock);

            Assert.That(older.TryLoad(out var error), Is.True, error);

            Assert.That(older.GetGroup("default")!.Allows(AuthMethod.KeyPin), Is.False);

        }

        #endregion

    }

}
