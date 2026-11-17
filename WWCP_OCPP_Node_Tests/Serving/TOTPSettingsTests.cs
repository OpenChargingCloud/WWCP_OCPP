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

using org.GraphDefined.Vanaheimr.Hermod;


using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// What a charging station needs in order to be let in with a one-time
    /// token, and what this controller refuses to store.
    /// </summary>
    public class TOTPSettingsTests
    {

        #region Data

        private static readonly DateTimeOffset Noon = new (2026, 6, 1, 12, 0, 0, TimeSpan.Zero);

        private const String TheSecret = "a-shared-secret-long-enough";

        #endregion

        #region (private) Settings(...)

        private static TOTPSettings Settings(String?             SharedSecret    = TheSecret,
                                             TimeSpan?           ValidityTime    = null,
                                             UInt32?             Length          = null,
                                             String?             Alphabet        = null,
                                             TOTPHashAlgorithm?  HashAlgorithm   = null)
        {

            if (!TOTPSettings.TryCreate(SharedSecret, ValidityTime, Length, Alphabet, HashAlgorithm,
                                        out var settings, out _, out var error))
            {
                throw new InvalidOperationException($"The test's own TOTP settings were refused: {error}");
            }

            return settings;

        }

        /// <summary>
        /// The token a charging station would be showing at that moment, made
        /// the way the station makes it rather than the way the controller
        /// checks it.
        /// </summary>
        private static String TokenAt(TOTPSettings Settings, DateTimeOffset When)

            => TOTPGenerator.GenerateTOTP(
                   Settings.SharedSecret,
                   Settings.ValidityTime,
                   Settings.Length,
                   Settings.Alphabet,
                   When,
                   null,
                   Settings.HashAlgorithm
               ).Current;

        #endregion


        #region TheTokenOfTheMomentIsAccepted()

        [Test]
        public void TheTokenOfTheMomentIsAccepted()
        {

            var settings = Settings();

            Assert.That(settings.Matches(TokenAt(settings, Noon), Noon), Is.True);

        }

        #endregion

        #region TheTokenBeforeAndTheTokenAfterAreAcceptedToo()

        /// <summary>
        /// Two clocks are never quite the same, and a token that was right when
        /// it was sent must still be right when it arrives.
        /// </summary>
        [Test]
        public void TheTokenBeforeAndTheTokenAfterAreAcceptedToo()
        {

            var settings = Settings(ValidityTime: TimeSpan.FromSeconds(30));

            Assert.Multiple(() => {

                Assert.That(settings.Matches(TokenAt(settings, Noon.AddSeconds(-30)), Noon), Is.True,
                            "The token of the slot before was refused, so a station whose clock is a moment behind cannot sign in.");

                Assert.That(settings.Matches(TokenAt(settings, Noon.AddSeconds(30)), Noon), Is.True,
                            "The token of the slot after was refused, so a station whose clock is a moment ahead cannot sign in.");

            });

        }

        #endregion

        #region ATokenFromWellOutsideTheWindowIsRefused()

        /// <summary>
        /// The tolerance is three slots wide and no wider. A token from an hour
        /// ago being accepted would make the whole exercise pointless.
        /// </summary>
        [Test]
        public void ATokenFromWellOutsideTheWindowIsRefused()
        {

            var settings = Settings(ValidityTime: TimeSpan.FromSeconds(30));

            Assert.Multiple(() => {
                Assert.That(settings.Matches(TokenAt(settings, Noon.AddMinutes(-60)), Noon), Is.False);
                Assert.That(settings.Matches(TokenAt(settings, Noon.AddSeconds(-90)), Noon), Is.False);
                Assert.That(settings.Matches(TokenAt(settings, Noon.AddSeconds( 90)), Noon), Is.False);
            });

        }

        #endregion

        #region ATokenOfAnotherSecretIsRefused()

        [Test]
        public void ATokenOfAnotherSecretIsRefused()
        {

            var ours    = Settings();
            var theirs  = Settings(SharedSecret: "a-completely-different-secret");

            Assert.That(ours.Matches(TokenAt(theirs, Noon), Noon), Is.False);

        }

        #endregion

        #region NothingAtAllIsRefused()

        [Test]
        public void NothingAtAllIsRefused()
        {

            var settings = Settings();

            Assert.Multiple(() => {
                Assert.That(settings.Matches(null, Noon), Is.False);
                Assert.That(settings.Matches("",   Noon), Is.False);
            });

        }

        #endregion


        #region ASecretThatIsLeftOutIsMadeUp()

        [Test]
        public void ASecretThatIsLeftOutIsMadeUp()
        {

            Assert.That(TOTPSettings.TryCreate(null, null, null, null, null,
                                               out var settings, out var generated, out var error),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(generated,                Is.Not.Null);
                Assert.That(generated!.Length,        Is.GreaterThanOrEqualTo(TOTPSettings.MinSecretLength));
                Assert.That(settings!.SharedSecret,   Is.EqualTo(generated));
            });

        }

        #endregion

        #region ASecretHermodWouldThrowOnIsRefusedHereFirst()

        /// <summary>
        /// Hermod trims the secret and throws on anything short or spaced. A
        /// controller that stored one anyway would turn that into an exception
        /// at the moment a charging station tries to sign in, which is the
        /// worst possible moment to find out.
        /// </summary>
        [Test]
        public void ASecretHermodWouldThrowOnIsRefusedHereFirst()
        {

            Assert.Multiple(() => {

                Assert.That(TOTPSettings.TryCreate("short", null, null, null, null, out _, out _, out var tooShort),
                            Is.False);
                Assert.That(tooShort, Does.Contain("16"));

                Assert.That(TOTPSettings.TryCreate("a secret with spaces in it", null, null, null, null,
                                                   out _, out _, out var spaced),
                            Is.False);
                Assert.That(spaced, Does.Contain("whitespace"));

                Assert.That(TOTPSettings.TryCreate(new String('x', TOTPSettings.MaxSecretLength + 1), null, null, null, null,
                                                   out _, out _, out var tooLong),
                            Is.False);
                Assert.That(tooLong, Is.Not.Null);

            });

        }

        #endregion

        #region AnAlphabetThatWouldMakeEveryTokenTheSameIsRefused()

        [Test]
        public void AnAlphabetThatWouldMakeEveryTokenTheSameIsRefused()
        {

            Assert.Multiple(() => {

                Assert.That(TOTPSettings.TryCreate(TheSecret, null, null, "x", null, out _, out _, out var oneCharacter),
                            Is.False);
                Assert.That(oneCharacter, Does.Contain("two different"));

                Assert.That(TOTPSettings.TryCreate(TheSecret, null, null, "aabb", null, out _, out _, out var repeated),
                            Is.False);
                Assert.That(repeated, Does.Contain("twice"));

            });

        }

        #endregion

        #region ATokenLongerThanTheHashIsRefused()

        /// <summary>
        /// The token is read out of the HMAC as a ring buffer, so asking for
        /// more characters than the hash has bytes buys nothing and only looks
        /// stronger.
        /// </summary>
        [Test]
        public void ATokenLongerThanTheHashIsRefused()
        {

            Assert.Multiple(() => {

                Assert.That(TOTPSettings.TryCreate(TheSecret, null, 33, null, TOTPHashAlgorithm.SHA256,
                                                   out _, out _, out var tooLong),
                            Is.False);
                Assert.That(tooLong, Does.Contain("32"));

                // The very same length is fine on a longer hash.
                Assert.That(TOTPSettings.TryCreate(TheSecret, null, 33, null, TOTPHashAlgorithm.SHA384,
                                                   out _, out _, out var fine),
                            Is.True, fine);

            });

        }

        #endregion

        #region AValidityNobodyCouldUseIsRefused()

        [Test]
        public void AValidityNobodyCouldUseIsRefused()
        {

            Assert.Multiple(() => {

                Assert.That(TOTPSettings.TryCreate(TheSecret, TimeSpan.FromSeconds(1), null, null, null,
                                                   out _, out _, out var tooShort),
                            Is.False);
                Assert.That(tooShort, Is.Not.Null);

                Assert.That(TOTPSettings.TryCreate(TheSecret, TimeSpan.FromHours(1), null, null, null,
                                                   out _, out _, out var tooLong),
                            Is.False);
                Assert.That(tooLong, Is.Not.Null);

            });

        }

        #endregion


        #region TheSharedSecretIsNotInWhatThePageReads()

        /// <summary>
        /// The one credential here that can be used as it stands, so the one
        /// that must not travel to a page anybody who may read the
        /// configuration can open.
        /// </summary>
        [Test]
        public void TheSharedSecretIsNotInWhatThePageReads()
        {

            var settings = Settings();

            var forThePage = settings.ToJSON();
            var forTheFile = settings.ToJSON(WithSecret: true);

            Assert.Multiple(() => {

                Assert.That(forThePage["sharedSecret"],          Is.Null,
                            "The shared secret was handed to the web interface.");
                Assert.That(forThePage.ToString(),               Does.Not.Contain(TheSecret));

                Assert.That(forTheFile.Value<String>("sharedSecret"), Is.EqualTo(TheSecret),
                            "The file has to carry it: the controller computes tokens from it.");

            });

        }

        #endregion

        #region WhatWasWrittenIsWhatIsReadBack()

        [Test]
        public void WhatWasWrittenIsWhatIsReadBack()
        {

            var settings = Settings(ValidityTime:   TimeSpan.FromSeconds(45),
                                    Length:         20,
                                    Alphabet:       "0123456789",
                                    HashAlgorithm:  TOTPHashAlgorithm.SHA512);

            Assert.That(TOTPSettings.TryParse(settings.ToJSON(WithSecret: true), out var read, out var error),
                        Is.True, error);

            Assert.That(read, Is.EqualTo(settings));

        }

        #endregion

        #region AConfigurationWithoutItsSecretIsRefusedRatherThanMadeUp()

        /// <summary>
        /// A file that lost its secret must say so. Making one up would mean
        /// the controller quietly starts expecting tokens no charging station
        /// can produce.
        /// </summary>
        [Test]
        public void AConfigurationWithoutItsSecretIsRefusedRatherThanMadeUp()
        {

            var json = new JObject(
                           new JProperty("validitySeconds", 30),
                           new JProperty("length",          12)
                       );

            Assert.Multiple(() => {
                Assert.That(TOTPSettings.TryParse(json, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("sharedSecret"));
            });

        }

        #endregion

        #region TheConfigHandedToHermodIsNotTheBoundKind()

        /// <summary>
        /// The binding is asked for or it is not, and the config handed to
        /// Hermod says which rather than leaving it to a default - Hermod's
        /// is the bound kind.
        /// </summary>
        [Test]
        public void TheConfigHandedToHermodIsOfTheKindThatWasAskedFor()
        {

            var raw    = Settings().ToTOTPConfig();
            var bound  = (Settings() with { TLSChannelBinding = true }).ToTOTPConfig();

            Assert.Multiple(() => {
                Assert.That(raw.  UseTLSExporterMaterial,  Is.False);
                Assert.That(bound.UseTLSExporterMaterial,  Is.True);
                Assert.That(raw.  SharedSecret,            Is.EqualTo(TheSecret));
            });

        }

        #endregion

        #region ATokenBoundToItsConnectionIsCheckedWithWhatThatConnectionExports()

        /// <summary>
        /// A token sent as bound is checked with the keying material of the
        /// connection it arrived on: right with that, wrong with another
        /// connection's, and no token at all where there is none - never the
        /// raw one, which is what a token read off another connection and
        /// called bound would be.
        /// </summary>
        [Test]
        public void ATokenBoundToItsConnectionIsCheckedWithWhatThatConnectionExports()
        {

            var settings  = Settings();
            var here      = Enumerable.Range(0, 32).Select(i => (Byte) i).      ToArray();
            var elsewhere = Enumerable.Range(0, 32).Select(i => (Byte) (i + 1)).ToArray();

            var bound     = TOTPGenerator.GenerateTOTP(settings.SharedSecret, settings.ValidityTime, settings.Length, settings.Alphabet,
                                                       Noon, here, settings.HashAlgorithm).Current;
            var raw       = TokenAt(settings, Noon);

            Assert.Multiple(() => {
                Assert.That(bound,                                           Is.Not.EqualTo(raw));
                Assert.That(settings.Matches(bound, Noon, true,  here),      Is.True,   "the token bound to this connection");
                Assert.That(settings.Matches(bound, Noon, true,  elsewhere), Is.False,  "a token bound to another connection");
                Assert.That(settings.Matches(bound, Noon, true,  null),      Is.False,  "a bound token where the connection exports nothing");
                Assert.That(settings.Matches(bound, Noon, true,  []),        Is.False,  "a bound token where the connection exports nothing");
                Assert.That(settings.Matches(bound, Noon, false, here),      Is.False,  "a bound token shown as a raw one");
                Assert.That(settings.Matches(raw,   Noon, true,  here),      Is.False,  "a raw token shown as a bound one");
                Assert.That(settings.Matches(raw,   Noon, true,  null),      Is.False,  "a raw token shown as a bound one, where nothing is exported");
                Assert.That(settings.Matches(raw,   Noon),                   Is.True,   "the raw token, where the binding is not asked for");
            });

        }

        #endregion

        #region ALoginThatAsksForTheBindingTakesNoRawToken()

        [Test]
        public void ALoginThatAsksForTheBindingTakesNoRawToken()
        {

            var settings  = Settings() with { TLSChannelBinding = true };
            var here      = Enumerable.Range(0, 32).Select(i => (Byte) i).ToArray();

            var bound     = TOTPGenerator.GenerateTOTP(settings.SharedSecret, settings.ValidityTime, settings.Length, settings.Alphabet,
                                                       Noon, here, settings.HashAlgorithm).Current;

            Assert.Multiple(() => {
                Assert.That(settings.Matches(TokenAt(settings, Noon), Noon),              Is.False,  "a raw token");
                Assert.That(settings.Matches(TokenAt(settings, Noon), Noon, false, here), Is.False,  "a raw token, over a connection that could have bound it");
                Assert.That(settings.Matches(bound, Noon, true, here),                    Is.True,   "the bound one");
                Assert.That(settings.ToString(),                                          Does.Contain("bound to the TLS session"));
            });

        }

        #endregion

        #region TheBindingIsWrittenWhereItIsAskedFor_AndReadBack()

        /// <summary>
        /// A file without the binding reads as it always did, one with it
        /// reads back bound - and one that says something else than true or
        /// false is refused: read as false, it would let raw tokens in where
        /// bound ones were asked for.
        /// </summary>
        [Test]
        public void TheBindingIsWrittenWhereItIsAskedFor_AndReadBack()
        {

            var raw      = Settings();
            var bound    = raw with { TLSChannelBinding = true };

            var mistyped = bound.ToJSON(WithSecret: true);
            mistyped["tlsChannelBinding"] = "yes";

            Assert.Multiple(() => {

                Assert.That(raw.  ToJSON()["tlsChannelBinding"],                         Is.Null);
                Assert.That(bound.ToJSON()["tlsChannelBinding"]?.Value<Boolean>(),       Is.True);

                Assert.That(TOTPSettings.TryParse(raw.  ToJSON(WithSecret: true), out var readRaw,   out _),  Is.True);
                Assert.That(TOTPSettings.TryParse(bound.ToJSON(WithSecret: true), out var readBound, out _),  Is.True);
                Assert.That(readRaw?.  TLSChannelBinding,                                Is.False);
                Assert.That(readBound?.TLSChannelBinding,                                Is.True);
                Assert.That(readBound,                                                   Is.EqualTo(bound));

                Assert.That(TOTPSettings.TryParse(mistyped, out var none, out var error), Is.False);
                Assert.That(none,                                                         Is.Null);
                Assert.That(error,                                                        Does.Contain("tlsChannelBinding"));

            });

        }

        #endregion

    }

}
