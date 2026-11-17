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


using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The groups that decide what a login may do.
    /// </summary>
    public class LoginGroupTests
    {

        #region Data

        private String                 directory  = default!;
        private TestClock              clock      = default!;
        private ChargingStationLogins  logins     = default!;

        private const String ThePassword = "a-password-long-enough-for-ocpp";

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAFile()
        {

            directory = ServingTestDirectories.TemporaryDirectory("groups");
            Directory.CreateDirectory(directory);

            clock     = TestClock.At(2026, 6, 1);
            logins    = new ChargingStationLogins(Path.Combine(directory, "ocpp-stations.json"), clock);

            if (!logins.TryLoad(out var error))
                throw new InvalidOperationException($"The test's own login store was refused: {error}");

        }

        [TearDown]
        public void RemoveTheDirectory()
            => ServingTestDirectories.Remove(directory);

        #endregion


        #region ThereIsAlwaysADefaultGroupAndItBehavesAsBefore()

        /// <summary>
        /// A controller updated to a version that knows about groups must let
        /// in every charging station it let in yesterday, so the group they all
        /// land in has to allow what the server allows.
        /// </summary>
        [Test]
        public void ThereIsAlwaysADefaultGroupAndItBehavesAsBefore()
        {

            var group = logins.GetGroup(LoginGroup.DefaultId);

            Assert.That(group, Is.Not.Null);

            Assert.Multiple(() => {

                Assert.That(group!.IsBuiltIn,  Is.True);
                Assert.That(group.Enabled,     Is.True);

                Assert.That(group.Allows(AuthMethod.Basic),        Is.True);
                Assert.That(group.Allows(AuthMethod.TOTP),         Is.True);
                Assert.That(group.Allows(AuthMethod.Certificate),  Is.True);

                Assert.That(group.Allows((Byte) 1),  Is.True);
                Assert.That(group.Allows((Byte) 2),  Is.True);
                Assert.That(group.Allows((Byte) 3),  Is.True);

            });

        }

        #endregion

        #region AGroupWithNothingTickedLetsNobodyIn()

        /// <summary>
        /// The same choice the accepted certificate chains make: a list that
        /// means "everything" when it is empty is a list that opens the door
        /// when somebody deletes the last row.
        /// </summary>
        [Test]
        public void AGroupWithNothingTickedLetsNobodyIn()
        {

            Assert.That(logins.TryAddOrUpdateGroup("locked", "Locked", true, [], [], null, out var error),
                        Is.True, error);

            var group = logins.GetGroup("locked");

            Assert.Multiple(() => {
                Assert.That(group!.Allows(AuthMethod.Basic),        Is.False);
                Assert.That(group.Allows(AuthMethod.TOTP),          Is.False);
                Assert.That(group.Allows(AuthMethod.Certificate),   Is.False);
                Assert.That(group.Allows((Byte) 1),                 Is.False);
            });

        }

        #endregion

        #region AGroupThatIsSwitchedOffAllowsNothingWhateverIsTicked()

        [Test]
        public void AGroupThatIsSwitchedOffAllowsNothingWhateverIsTicked()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", false,
                                       [AuthMethod.Basic, AuthMethod.TOTP], [1, 2, 3], null, out _);

            var group = logins.GetGroup("field");

            Assert.Multiple(() => {
                Assert.That(group!.AuthMethods,               Has.Count.EqualTo(2),
                            "What it accepts is kept, so that switching it on again does not mean setting it up again.");
                Assert.That(group.Allows(AuthMethod.Basic),   Is.False);
                Assert.That(group.Allows((Byte) 2),           Is.False);
            });

        }

        #endregion

        #region AnIdentificationThatCouldNotBeInAURLIsRefused()

        [Test]
        public void AnIdentificationThatCouldNotBeInAURLIsRefused()
        {

            Assert.Multiple(() => {

                Assert.That(logins.TryAddOrUpdateGroup("",             null, true, [], [], null, out var empty),    Is.False);
                Assert.That(empty, Is.Not.Null);

                Assert.That(logins.TryAddOrUpdateGroup("a group",      null, true, [], [], null, out var spaced),   Is.False);
                Assert.That(spaced, Does.Contain("lower-case"));

                Assert.That(logins.TryAddOrUpdateGroup("a/b",          null, true, [], [], null, out var slashed),  Is.False);
                Assert.That(slashed, Does.Contain("lower-case"));

                Assert.That(logins.TryAddOrUpdateGroup(new String('g', 33), null, true, [], [], null, out var tooLong), Is.False);
                Assert.That(tooLong, Is.Not.Null);

            });

        }

        #endregion

        #region AnIdentificationIsTheSameGroupWhateverItsCase()

        /// <summary>
        /// Otherwise "Field-Test" would make a second group that shadows the
        /// first, and which of the two decides would depend on how somebody
        /// typed it.
        /// </summary>
        [Test]
        public void AnIdentificationIsTheSameGroupWhateverItsCase()
        {

            logins.TryAddOrUpdateGroup("field", "First",  true, [AuthMethod.Basic], [1], null, out _);
            logins.TryAddOrUpdateGroup("FIELD", "Second", true, [AuthMethod.TOTP],  [2], null, out _);

            Assert.Multiple(() => {
                Assert.That(logins.Groups.Count(group => group.Id == "field"),  Is.EqualTo(1));
                Assert.That(logins.GetGroup("field")!.Name,                     Is.EqualTo("Second"));
            });

        }

        #endregion

        #region ANotASecurityProfileIsRefused()

        [Test]
        public void ANotASecurityProfileIsRefused()
        {

            Assert.Multiple(() => {
                Assert.That(logins.TryAddOrUpdateGroup("odd", null, true, [], [4], null, out var error), Is.False);
                Assert.That(error, Does.Contain("1, 2 and 3"));
            });

        }

        #endregion


        #region TheDefaultGroupCannotBeRemoved()

        /// <summary>
        /// A login without a group would be a login nothing decides about.
        /// </summary>
        [Test]
        public void TheDefaultGroupCannotBeRemoved()
        {

            Assert.Multiple(() => {
                Assert.That(logins.TryRemoveGroup(LoginGroup.DefaultId, out var error),  Is.False);
                Assert.That(error, Is.Not.Null);
                Assert.That(logins.GetGroup(LoginGroup.DefaultId),                       Is.Not.Null);
            });

        }

        #endregion

        #region AGroupWithMembersIsNotRemovedAndItsMembersAreNotQuietlyMoved()

        /// <summary>
        /// The default group is the permissive one, so tidying a member into it
        /// would be a quiet widening of what that station may do.
        /// </summary>
        [Test]
        public void AGroupWithMembersIsNotRemovedAndItsMembersAreNotQuietlyMoved()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", true, [AuthMethod.Basic], [1], null, out _);
            logins.TrySetPassword("cs001", ThePassword, "field", null, out _, out _);

            Assert.Multiple(() => {

                Assert.That(logins.TryRemoveGroup("field", out var error),  Is.False);
                Assert.That(error, Does.Contain("1 charging station"));

                Assert.That(logins.GetGroup("field"),                       Is.Not.Null);
                Assert.That(logins.Logins.Single().GroupId,                 Is.EqualTo("field"));

            });

        }

        #endregion

        #region AGroupNobodyIsInGoesAway()

        [Test]
        public void AGroupNobodyIsInGoesAway()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", true, [AuthMethod.Basic], [1], null, out _);
            logins.TrySetPassword("cs001", ThePassword, "field", null, out _, out _);

            Assert.That(logins.TrySetGroup("cs001", LoginGroup.DefaultId, out var moved), Is.True, moved);
            Assert.That(logins.TryRemoveGroup("field", out var error),                    Is.True, error);

            Assert.That(logins.GetGroup("field"), Is.Null);

        }

        #endregion


        #region ALoginCannotBePutIntoAGroupThatIsNotThere()

        [Test]
        public void ALoginCannotBePutIntoAGroupThatIsNotThere()
        {

            Assert.Multiple(() => {

                Assert.That(logins.TrySetPassword("cs001", ThePassword, "nowhere", null, out _, out var onAdding),
                            Is.False);
                Assert.That(onAdding, Does.Contain("nowhere"));

                Assert.That(logins.Logins, Is.Empty,
                            "A charging station was added although its group was refused.");

            });

        }

        #endregion

        #region MovingALoginToAGroupThatIsNotThereIsRefused()

        [Test]
        public void MovingALoginToAGroupThatIsNotThereIsRefused()
        {

            logins.TrySetPassword("cs001", ThePassword, null, null, out _, out _);

            Assert.Multiple(() => {
                Assert.That(logins.TrySetGroup("cs001", "nowhere", out var error),  Is.False);
                Assert.That(error, Is.Not.Null);
                Assert.That(logins.Logins.Single().GroupId,                        Is.EqualTo(LoginGroup.DefaultId));
            });

        }

        #endregion

        #region ALoginOfAGroupThatIsSwitchedOffDoesNotCountAsAbleToSignIn()

        /// <summary>
        /// The number on the page is meant to be the number that could actually
        /// come in, so the group has its say in it.
        /// </summary>
        [Test]
        public void ALoginOfAGroupThatIsSwitchedOffDoesNotCountAsAbleToSignIn()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", true, [AuthMethod.Basic], [1], null, out _);
            logins.TrySetPassword("cs001", ThePassword, "field", null, out _, out _);

            Assert.That(logins.EnabledCount, Is.EqualTo(1));

            logins.TryAddOrUpdateGroup("field", "Field test", false, [AuthMethod.Basic], [1], null, out _);

            Assert.That(logins.EnabledCount, Is.EqualTo(0));

        }

        #endregion

        #region AGroupThatDoesNotAcceptAPasswordTakesItsMembersOutOfTheServersList()

        /// <summary>
        /// The WebSocket server keeps the passwords to answer with, so a
        /// station whose group has had Basic Authentication taken away must
        /// drop out of that list rather than linger in it.
        /// </summary>
        [Test]
        public void AGroupThatDoesNotAcceptAPasswordTakesItsMembersOutOfTheServersList()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", true, [AuthMethod.Basic], [1], null, out _);
            logins.TrySetPassword("cs001", ThePassword, "field", null, out _, out _);

            Assert.That(logins.SecurePasswords(), Does.ContainKey("cs001"));

            logins.TryAddOrUpdateGroup("field", "Field test", true, [AuthMethod.TOTP], [1], null, out _);

            Assert.That(logins.SecurePasswords(), Does.Not.ContainKey("cs001"));

        }

        #endregion


        #region WhatWasWrittenIsWhatIsReadBack()

        [Test]
        public void WhatWasWrittenIsWhatIsReadBack()
        {

            logins.TryAddOrUpdateGroup("field", "Field test", false,
                                       [AuthMethod.TOTP, AuthMethod.Certificate], [2, 3], "the yard", out _);

            logins.TrySetPassword("cs001", ThePassword, "field", "Ladepunkt 1", out _, out _);

            var again = new ChargingStationLogins(logins.Path, clock);

            Assert.That(again.TryLoad(out var error), Is.True, error);

            var group = again.GetGroup("field");

            Assert.Multiple(() => {

                Assert.That(group,                   Is.Not.Null);
                Assert.That(group!.Name,             Is.EqualTo("Field test"));
                Assert.That(group.Enabled,           Is.False);
                Assert.That(group.Note,              Is.EqualTo("the yard"));
                Assert.That(group.AuthMethods,       Is.EquivalentTo(new[] { AuthMethod.TOTP, AuthMethod.Certificate }));
                Assert.That(group.SecurityProfiles,  Is.EquivalentTo(new Byte[] { 2, 3 }));

                Assert.That(again.Logins.Single().GroupId,  Is.EqualTo("field"));

            });

        }

        #endregion

        #region AFileFromBeforeThereWereGroupsStillWorks()

        /// <summary>
        /// The charging stations of a controller that is updated must keep
        /// signing in, and the file of that controller says nothing about
        /// groups at all.
        /// </summary>
        [Test]
        public void AFileFromBeforeThereWereGroupsStillWorks()
        {

            // Exactly the shape the old version wrote: a 'stations' array, a
            // PBKDF2 hash, and not a word about groups.
            File.WriteAllText(
                logins.Path,
                new JObject(
                    new JProperty("stations", new JArray(
                        new JObject(
                            new JProperty("id",       "cs001"),
                            new JProperty("password", org.GraphDefined.Vanaheimr.Hermod.HTTP.SecurePassword.Create(ThePassword).ToString()),
                            new JProperty("enabled",  true),
                            new JProperty("addedAt",  "2025-01-01T00:00:00.0000000+00:00"),
                            new JProperty("note",     "Ladepunkt 1")
                        )
                    ))
                ).ToString()
            );

            var again = new ChargingStationLogins(logins.Path, clock);

            Assert.That(again.TryLoad(out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(again.Logins.Single().GroupId,  Is.EqualTo(LoginGroup.DefaultId));
                Assert.That(again.Verify("cs001", ThePassword), Is.True,
                            "A charging station that could sign in before the update cannot sign in after it.");
                Assert.That(again.EnabledCount,             Is.EqualTo(1));
            });

        }

        #endregion

        #region ALoginPointingAtAGroupThatIsNotThereIsAFailureAndNotAQuietRepair()

        /// <summary>
        /// Moving it into the default group would hand that station more than
        /// whoever wrote the file meant it to have.
        /// </summary>
        [Test]
        public void ALoginPointingAtAGroupThatIsNotThereIsAFailureAndNotAQuietRepair()
        {

            File.WriteAllText(
                logins.Path,
                new JObject(
                    new JProperty("groups",   new JArray()),
                    new JProperty("stations", new JArray(
                        new JObject(
                            new JProperty("id",       "cs001"),
                            new JProperty("group",    "gone"),
                            new JProperty("password", org.GraphDefined.Vanaheimr.Hermod.HTTP.SecurePassword.Create(ThePassword).ToString()),
                            new JProperty("enabled",  true)
                        )
                    ))
                ).ToString()
            );

            var again = new ChargingStationLogins(logins.Path, clock);

            Assert.Multiple(() => {
                Assert.That(again.TryLoad(out var error), Is.False);
                Assert.That(error, Does.Contain("gone"));
            });

        }

        #endregion

    }

}
