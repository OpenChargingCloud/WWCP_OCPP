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


using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// Which charging stations may sign in, and with what - OCPP security
    /// profiles 1 and 2.
    /// </summary>
    public class StationLoginTests
    {

        #region Data

        private String                 directory  = default!;
        private TestClock              clock      = default!;
        private WebSocketLogins  logins     = default!;

        private const String           APassword        = "a-password-long-enough-for-ocpp";
        private const String           AnotherPassword  = "another-password-long-enough";

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAFile()
        {

            directory = ServingTestDirectories.TemporaryDirectory("stations");
            Directory.CreateDirectory(directory);

            clock     = TestClock.At(2026, 6, 1);
            logins    = new WebSocketLogins(Path.Combine(directory, "websocket-logins.json"), clock);

            Assert.That(logins.TryLoad(out var error), Is.True, error);

        }

        [TearDown]
        public void RemoveTheDirectory()
            => ServingTestDirectories.Remove(directory);

        #endregion


        #region NoFileIsAControllerNoStationHasBeenAddedTo()

        [Test]
        public void NoFileIsAControllerNoStationHasBeenAddedTo()
        {

            Assert.Multiple(() => {
                Assert.That(logins.Exists,        Is.False);
                Assert.That(logins.Logins,        Is.Empty);
                Assert.That(logins.EnabledCount,  Is.EqualTo(0));
            });

        }

        #endregion

        #region AnUnreadableFileIsAFailureAndNotAnEmptyList()

        /// <summary>
        /// Carrying on with an empty list would turn every charging station on
        /// the site away while looking like nothing was wrong.
        /// </summary>
        [Test]
        public void AnUnreadableFileIsAFailureAndNotAnEmptyList()
        {

            var path = Path.Combine(directory, "websocket-logins.json");

            File.WriteAllText(path, "{ stations: [ unquoted");

            var broken = new WebSocketLogins(path, clock);

            Assert.Multiple(() => {
                Assert.That(broken.TryLoad(out var error), Is.False);
                Assert.That(error,                         Does.Contain("websocket-logins.json"));
            });

        }

        #endregion

        #region NoPasswordIsMadeUp()

        /// <summary>
        /// A password is given, and none is made up here: it is kept only as a
        /// hash, and whoever sets it has to know it to type it into the
        /// charging station.
        /// </summary>
        [Test]
        public void NoPasswordIsMadeUp()
        {

            Assert.Multiple(() => {

                Assert.That(logins.TrySetPassword("cs001", null, null, "Ladepunkt 1", out var none),  Is.False);
                Assert.That(none,                                                                    Does.Contain("16"));

                Assert.That(logins.TrySetPassword("cs001", "   ", null, "Ladepunkt 1", out var blank), Is.False);
                Assert.That(blank,                                                                   Does.Contain("16"));

                Assert.That(logins.Logins,                                                           Is.Empty,
                            "A charging station was added without a password.");

                Assert.That(logins.TrySetPassword("cs001", APassword, null, "Ladepunkt 1", out var error), Is.True, error);
                Assert.That(logins.Verify("cs001", APassword),                                       Is.True);

                // And nowhere else: not in what the page is shown.
                Assert.That(logins.ToJSON().ToString(),                                              Does.Not.Contain(APassword));

            });

        }

        #endregion

        #region TheFileNeverHoldsThePasswordInTheClear()

        [Test]
        public void TheFileNeverHoldsThePasswordInTheClear()
        {

            logins.TrySetPassword("cs001", APassword, null, null, out _);

            var written = File.ReadAllText(Path.Combine(directory, "websocket-logins.json"));

            Assert.Multiple(() => {
                Assert.That(written, Does.Not.Contain(APassword),
                            "The password of a charging station was written to its file in the clear.");
                Assert.That(written, Does.Contain("$pbkdf2"));
            });

        }

        #endregion

        #region AChosenPasswordHasToBeWorthChoosing()

        /// <summary>
        /// OCPP asks for at least 16 characters of entropy in the password of a
        /// charging station; a controller that accepted "1234" would be the
        /// weakest part of a system built around that rule.
        /// </summary>
        [Test]
        public void AChosenPasswordHasToBeWorthChoosing()
        {

            Assert.Multiple(() => {

                Assert.That(logins.TrySetPassword("cs001", "1234", null, null, out var tooShort), Is.False);
                Assert.That(tooShort, Does.Contain("16"));

                Assert.That(logins.TrySetPassword("cs001", new String('x', 65), null, null, out var tooLong), Is.False);
                Assert.That(tooLong,  Does.Contain("64"));

                Assert.That(logins.TrySetPassword("cs001", new String('x', 20), null, null, out var fine), Is.True, fine);

            });

        }

        #endregion

        #region AStationNeedsAnIdentificationToSignInWith()

        [Test]
        public void AStationNeedsAnIdentificationToSignInWith()
        {

            Assert.Multiple(() => {
                Assert.That(logins.TrySetPassword("",  null, null, null, out var empty), Is.False);
                Assert.That(empty, Does.Contain("identification"));

                Assert.That(logins.TrySetPassword(new String('c', 49), null, null, null, out var tooLong), Is.False);
                Assert.That(tooLong, Does.Contain("48"));
            });

        }

        #endregion

        #region AWrongPasswordOrAnUnknownStationIsRefused()

        [Test]
        public void AWrongPasswordOrAnUnknownStationIsRefused()
        {

            logins.TrySetPassword("cs001", APassword, null, null, out _);

            Assert.Multiple(() => {
                Assert.That(logins.Verify("cs001", APassword),        Is.True);
                Assert.That(logins.Verify("cs001", "something else"),  Is.False);
                Assert.That(logins.Verify("cs002", APassword),        Is.False);
                Assert.That(logins.Verify("",      ""),                Is.False);
            });

        }

        #endregion

        #region AStationSwitchedOffCannotSignInAndKeepsItsPassword()

        [Test]
        public void AStationSwitchedOffCannotSignInAndKeepsItsPassword()
        {

            logins.TrySetPassword("cs001", APassword, null, null, out _);

            Assert.That(logins.TrySetEnabled("cs001", false, out var error), Is.True, error);

            Assert.Multiple(() => {

                Assert.That(logins.Verify("cs001", APassword),  Is.False);
                Assert.That(logins.EnabledCount,                 Is.EqualTo(0));
                Assert.That(logins.Logins.Count,                 Is.EqualTo(1));

                // A switched-off station is not handed to the server at all.
                Assert.That(logins.SecurePasswords(),            Is.Empty);

                Assert.That(logins.TrySetEnabled("cs001", true, out _), Is.True);
                Assert.That(logins.Verify("cs001", APassword),  Is.True);

            });

        }

        #endregion

        #region ChangingAPasswordKeepsWhatWasKnownAboutTheStation()

        [Test]
        public void ChangingAPasswordKeepsWhatWasKnownAboutTheStation()
        {

            logins.TrySetPassword("cs001", APassword, null, "Ladepunkt 1", out _);

            logins.TrySetEnabled ("cs001", false, out _);
            logins.TrySetPassword("cs001", AnotherPassword, null, null, out var error);

            var station = logins.Logins.Single();

            Assert.Multiple(() => {
                Assert.That(error,                            Is.Null);
                Assert.That(station.Note,                     Is.EqualTo("Ladepunkt 1"), "The note was lost when the password changed.");
                Assert.That(station.Enabled,                  Is.False,                  "A switched-off station was switched on by a new password.");
                Assert.That(logins.TrySetEnabled("cs001", true, out _), Is.True);
                Assert.That(logins.Verify("cs001", APassword),        Is.False, "The password of before still lets it in.");
                Assert.That(logins.Verify("cs001", AnotherPassword),  Is.True);
            });

        }

        #endregion

        #region ARemovedStationIsGoneFromTheFileToo()

        [Test]
        public void ARemovedStationIsGoneFromTheFileToo()
        {

            logins.TrySetPassword("cs001", APassword, null, null, out _);

            Assert.That(logins.TryRemove("cs001", out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(logins.Verify("cs001", APassword), Is.False);
                Assert.That(File.ReadAllText(Path.Combine(directory, "websocket-logins.json")), Does.Not.Contain("cs001"));
                Assert.That(logins.TryRemove("cs001", out var again), Is.False);
                Assert.That(again, Does.Contain("never heard of"));
            });

        }

        #endregion

        #region EverythingIsStillThereAfterARestart()

        [Test]
        public void EverythingIsStillThereAfterARestart()
        {

            logins.TrySetPassword("cs001", APassword, null, "Ladepunkt 1", out _);
            logins.TrySetPassword("cs002", AnotherPassword, null, null,          out _);
            logins.TrySetEnabled ("cs002", false, out _);

            var restarted = new WebSocketLogins(Path.Combine(directory, "websocket-logins.json"), clock);

            Assert.That(restarted.TryLoad(out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(restarted.Verify("cs001", APassword), Is.True);
                Assert.That(restarted.Logins.Count,                Is.EqualTo(2));
                Assert.That(restarted.EnabledCount,                Is.EqualTo(1));
                Assert.That(restarted.Logins.First().Note,         Is.EqualTo("Ladepunkt 1"));
                Assert.That(restarted.Logins.First().AddedAt,      Is.EqualTo(clock.Now));
            });

        }

        #endregion

        #region AChangeIsAnnouncedSoTheServerCanBeTold()

        /// <summary>
        /// A station whose password was taken away must stop being able to use
        /// it now, and not at the next start.
        /// </summary>
        [Test]
        public void AChangeIsAnnouncedSoTheServerCanBeTold()
        {

            var changes = 0;

            logins.OnChanged += () => changes++;

            logins.TrySetPassword("cs001", APassword, null, null, out _);
            logins.TrySetEnabled ("cs001", false, out _);
            logins.TryRemove     ("cs001", out _);

            // And not for something that changed nothing.
            logins.TrySetEnabled("cs001", false, out _);

            Assert.That(changes, Is.EqualTo(3));

        }

        #endregion

        #region WhatThePageIsShownNamesNoHashes()

        /// <summary>
        /// A PBKDF2 hash on a screen is a PBKDF2 hash somebody can take away
        /// and attack offline.
        /// </summary>
        [Test]
        public void WhatThePageIsShownNamesNoHashes()
        {

            logins.TrySetPassword("cs001", APassword, null, "Ladepunkt 1", out _);

            var shown = logins.ToJSON();

            Assert.Multiple(() => {
                Assert.That(shown.ToString(),                             Does.Not.Contain("$pbkdf2"));
                Assert.That(shown["logins"]?[0]?.Value<String>("id"),    Is.EqualTo("cs001"));
                Assert.That(shown["logins"]?[0]?.Value<String>("note"),  Is.EqualTo("Ladepunkt 1"));
                Assert.That(shown["logins"]?[0]?["password"],            Is.Null);
            });

        }

        #endregion

        #region TheBindingOfATokenIsSetWithoutItsSecret_AndOutlivesANewOne()

        /// <summary>
        /// Whether a token has to be bound is said without touching its
        /// secret, which cannot be read back; and a new secret does not undo
        /// it, which would be less than was configured, quietly.
        /// </summary>
        [Test]
        public void TheBindingOfATokenIsSetWithoutItsSecret_AndOutlivesANewOne()
        {

            const String theSecret = "a-shared-secret-long-enough";

            var here = Enumerable.Range(0, 32).Select(i => (Byte) i).ToArray();

            String Token(String Secret, Byte[]? Material)
                => org.GraphDefined.Vanaheimr.Hermod.TOTPGenerator.GenerateTOTP(Secret, TOTPSettings.DefaultValidityTime, TOTPSettings.DefaultLength, TOTPSettings.DefaultAlphabet,
                                              clock.GetUtcNow(), Material, org.GraphDefined.Vanaheimr.Hermod.TOTPHashAlgorithm.SHA256).Current;

            Assert.That(logins.TrySetTOTPBinding("cs001", true, out var nothingToBind), Is.False, "bound what is not there");

            logins.TrySetTOTP("cs001", theSecret, null, null, null, null, null, null, out _, out var error);

            var rawBefore    = logins.VerifyTOTP("cs001", Token(theSecret, null));

            var boundNow     = logins.TrySetTOTPBinding("cs001", true, out var bindingError);

            var rawAfter     = logins.VerifyTOTP("cs001", Token(theSecret, null));
            var boundAfter   = logins.VerifyTOTP("cs001", Token(theSecret, here), true, here);
            var shown        = logins.ToJSON();

            logins.TrySetTOTP("cs001", theSecret + "-anew", null, null, null, null, null, null, out _, out var errorAnew);

            var stillBound   = logins.TryGet("cs001", out var anew) && anew.TOTP?.TLSChannelBinding == true;
            var rawAnew      = logins.VerifyTOTP("cs001", Token(theSecret + "-anew", null));

            var unbound      = logins.TrySetTOTPBinding("cs001", false, out _);
            var rawAtLast    = logins.VerifyTOTP("cs001", Token(theSecret + "-anew", null));

            Assert.Multiple(() => {
                Assert.That(nothingToBind,  Is.Not.Null);
                Assert.That(error,          Is.Null);
                Assert.That(rawBefore,      Is.True,   "a raw token, before the binding was asked for");
                Assert.That(boundNow,       Is.True,   bindingError);
                Assert.That(rawAfter,       Is.False,  "a raw token, where the binding is asked for");
                Assert.That(boundAfter,     Is.True,   "the token bound to its connection");
                Assert.That(shown["logins"]?[0]?["totp"]?.Value<Boolean>("tlsChannelBinding"), Is.True, "the page is not told");
                Assert.That(errorAnew,      Is.Null);
                Assert.That(stillBound,     Is.True,   "a new secret undid the binding");
                Assert.That(rawAnew,        Is.False,  "a raw token of the new secret");
                Assert.That(unbound,        Is.True);
                Assert.That(rawAtLast,      Is.True,   "a raw token, once the binding is no longer asked for");
            });

        }

        #endregion

        #region WhatThePageIsShownCarriesNoSharedSecretEither()

        /// <summary>
        /// The shared secret is the one credential here that can be used as it
        /// stands - not a hash of something, the thing itself. A page that
        /// anybody who may read the configuration can open must not carry it.
        /// </summary>
        [Test]
        public void WhatThePageIsShownCarriesNoSharedSecretEither()
        {

            const String theSecret = "a-shared-secret-long-enough";

            logins.TrySetTOTP("cs001", theSecret, null, null, null, null, null, null, out _, out var error);

            var shown = logins.ToJSON();

            Assert.Multiple(() => {

                Assert.That(error,                                                Is.Null);
                Assert.That(shown.ToString(),                                     Does.Not.Contain(theSecret));
                Assert.That(shown["logins"]?[0]?["totp"]?["sharedSecret"],      Is.Null);

                // What the page does need: that there is one, and what shape it
                // has, so somebody can see it is configured without seeing it.
                Assert.That(shown["logins"]?[0]?.Value<Boolean>("hasTOTP"),      Is.True);
                Assert.That(shown["logins"]?[0]?["totp"]?.Value<Int32>("length"), Is.EqualTo((Int32) TOTPSettings.DefaultLength));

                // And the file does carry it, or no token could ever be checked.
                Assert.That(File.ReadAllText(logins.Path),                         Does.Contain(theSecret));

            });

        }

        #endregion

        #region ALoginMayCarryNoCredentialsAtAll()

        /// <summary>
        /// A charging station that comes in on a client certificate proves who
        /// it is without either - and may still want to be listed, so that it
        /// belongs to a group that decides something about it.
        /// </summary>
        [Test]
        public void ALoginMayCarryNoCredentialsAtAll()
        {

            logins.TrySetPassword("cs001", APassword, null, "Ladepunkt 1", out _);

            Assert.That(logins.TryClearPassword("cs001", out var error), Is.True, error);

            var again = new WebSocketLogins(logins.Path, clock);

            Assert.That(again.TryLoad(out var reading), Is.True, reading);

            var login = again.Logins.Single();

            Assert.Multiple(() => {
                Assert.That(login.HasPassword,   Is.False);
                Assert.That(login.HasTOTP,       Is.False);
                Assert.That(login.Credentials,   Is.Empty);
                Assert.That(login.Note,          Is.EqualTo("Ladepunkt 1"));
                Assert.That(again.Verify("cs001", ""), Is.False);
            });

        }

        #endregion

        #region TakingAwayACredentialThatIsNotThereSaysSo()

        [Test]
        public void TakingAwayACredentialThatIsNotThereSaysSo()
        {

            logins.TrySetPassword("cs001", APassword, null, null, out _);

            Assert.Multiple(() => {

                Assert.That(logins.TryClearTOTP("cs001", out var noToken),  Is.False);
                Assert.That(noToken, Does.Contain("no TOTP configuration"));

                Assert.That(logins.TryClearTOTP("cs404", out var unknown),  Is.False);
                Assert.That(unknown, Does.Contain("never heard of"));

            });

        }

        #endregion

        #region ALoginIsAStationsALocalControllersOrAGatewaysAndSaysSo()

        /// <summary>
        /// A login marked as a local controller's or a gateway's may send in
        /// the name of the charging stations behind it; it is the one or the
        /// other, and what it is survives a restart.
        /// </summary>
        [Test]
        public void ALoginIsAStationsALocalControllersOrAGatewaysAndSaysSo()
        {

            logins.TrySetPassword("lc001", APassword,       null, null, out _);
            logins.TrySetPassword("gw001", AnotherPassword, null, null, out _);

            Assert.Multiple(() => {

                Assert.That(logins.SpeaksForOthers("lc001"),                          Is.False, "A login speaks for others before it is marked.");

                Assert.That(logins.TrySetLocalController("lc001", true, out var lcError), Is.True, lcError);
                Assert.That(logins.TrySetGateway        ("gw001", true, out var gwError), Is.True, gwError);

                Assert.That(logins.IsLocalController("lc001"),                        Is.True);
                Assert.That(logins.IsGateway        ("gw001"),                        Is.True);
                Assert.That(logins.SpeaksForOthers  ("lc001"),                        Is.True);
                Assert.That(logins.SpeaksForOthers  ("gw001"),                        Is.True);

                // The one or the other: marked as a gateway's, it is no local
                // controller's any more.
                Assert.That(logins.TrySetGateway("lc001", true, out var bothError),   Is.True, bothError);
                Assert.That(logins.IsLocalController("lc001"),                        Is.False);
                Assert.That(logins.IsGateway        ("lc001"),                        Is.True);

                Assert.That(logins.TrySetLocalController("cs404", true, out var unknown), Is.False);
                Assert.That(unknown,                                                  Does.Contain("never heard of"));

            });

            var restarted = new WebSocketLogins(Path.Combine(directory, "websocket-logins.json"), clock);

            Assert.That(restarted.TryLoad(out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(restarted.IsGateway        ("gw001"),  Is.True,  "Being a gateway's login was not written down.");
                Assert.That(restarted.IsGateway        ("lc001"),  Is.True);
                Assert.That(restarted.IsLocalController("lc001"),  Is.False);
                Assert.That(restarted.Logins.Single(login => login.Id == "gw001").Kind, Is.EqualTo("gateway"));
            });

        }

        #endregion

        #region TheFileOfBeforeIsNotReadAndSaysWhatToDo()

        /// <summary>
        /// The logins were kept in 'ocpp-stations.json' before, with OCPP's
        /// security profiles in their groups: where that file is all there is,
        /// it is not read - the store refuses to load and says what to do,
        /// rather than start without anybody and turn everybody away.
        /// </summary>
        [Test]
        public void TheFileOfBeforeIsNotReadAndSaysWhatToDo()
        {

            var former = Path.Combine(directory, WebSocketLogins.FormerFileName);

            Directory.CreateDirectory(directory);
            File.WriteAllText(former, """{ "groups": [ { "id": "default", "securityProfiles": [ 1, 2, 3 ] } ], "stations": [] }""");

            var store = new WebSocketLogins(Path.Combine(directory, WebSocketLogins.DefaultFileName), clock);

            Assert.Multiple(() => {
                Assert.That(store.TryLoad(out var error), Is.False, "The file of before was read as if nothing had changed.");
                Assert.That(error, Does.Contain("is no more read").And.Contain(WebSocketLogins.DefaultFileName).And.Contain("clientAuthentication"));
                Assert.That(File.Exists(former), Is.True, "The file of before was taken away.");
            });

        }

        #endregion

    }

}
