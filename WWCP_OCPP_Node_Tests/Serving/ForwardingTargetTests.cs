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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The forwarding targets of the logins: a priority list of peers each, kept in the file of the
    /// logins, named by a login - and never let go of quietly, as a station whose target went would be
    /// sent to a CSMS nobody chose for it.
    /// </summary>
    public class ForwardingTargetTests
    {

        #region Data

        private String           directory  = default!;
        private TestClock        clock      = default!;
        private WebSocketLogins  logins     = default!;

        private String           File
            => Path.Combine(directory, "websocket-logins.json");

        private const String ThePassword = "a-password-long-enough-for-ocpp";

        private static readonly User_Id CS001 = User_Id.Parse("cs001");

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAFile()
        {

            directory = ServingTestDirectories.TemporaryDirectory("targets");
            Directory.CreateDirectory(directory);

            clock     = TestClock.At(2026, 6, 1);
            logins    = Load();

            Assert.That(logins.TrySetPasswordAuthentication(CS001, ThePassword, null, null, out var error), Is.True, error);

        }

        [TearDown]
        public void RemoveTheDirectory()
            => ServingTestDirectories.Remove(directory);

        private WebSocketLogins Load()
        {

            var loaded = new WebSocketLogins(File, clock) {
                             KnownPeerIds = () => [ "csms-a", "csms-b", "csms-c" ]
                         };

            if (!loaded.TryLoad(out var error))
                throw new InvalidOperationException($"The test's own login store was refused: {error}");

            return loaded;

        }

        #endregion


        #region ATarget_AndTheLoginThatNamesIt_AreKeptInTheFile()

        /// <summary>
        /// A target keeps its peers in their order, and a login its target, from one start to the next.
        /// </summary>
        [Test]
        public void ATarget_AndTheLoginThatNamesIt_AreKeptInTheFile()
        {

            Assert.That(logins.TryAddOrUpdateTarget("Site-1", [ "csms-b", "csms-a" ], I18NString.Create("Site 1"), out var error), Is.True, error);
            Assert.That(logins.TrySetTarget(CS001, "site-1", out error), Is.True, error);

            var again  = Load();
            var target = again.TargetOf(CS001);

            Assert.Multiple(() => {
                Assert.That(target?.Id,                         Is.EqualTo("site-1"), "the identification is kept in lower case");
                Assert.That(target?.PeerIds,                    Is.EqualTo(new[] { "csms-b", "csms-a" }), "the order of the peers was not kept");
                Assert.That(target?.Description?.FirstText(),   Is.EqualTo("Site 1"));
                Assert.That(again.ToJSON(WithTargets: true)["targets"]?[0]?["logins"]?.Value<Int32>(), Is.EqualTo(1), "the web interface is not told how many logins name it");
                Assert.That(again.ToJSON()["targets"],                                                Is.Null, "logins that are not forwarded are told of targets");
                Assert.That(again.ToJSON()["logins"]?[0]?["target"]?.Value<String>(), Is.EqualTo("site-1"));
            });

        }

        #endregion

        #region WhatATargetNames_IsChecked()

        /// <summary>
        /// A target names 1 to 8 peers this node has, each once, under an identification of letters,
        /// digits, '-' and '_'; a login names a target there is - or none.
        /// </summary>
        [Test]
        public void WhatATargetNames_IsChecked()
        {

            Assert.Multiple(() => {

                Assert.That(logins.TryAddOrUpdateTarget("t", [],                           null, out var none),     Is.False, "a target without a peer");
                Assert.That(logins.TryAddOrUpdateTarget("t", [ "csms-a", "csms-a" ],       null, out var twice),    Is.False, "a peer named twice");
                Assert.That(twice,                                                                                  Does.Contain("once"));
                Assert.That(logins.TryAddOrUpdateTarget("t", [ "csms-x" ],                 null, out var unknown),  Is.False, "a peer this node does not have");
                Assert.That(unknown,                                                                                Does.Contain("'csms-x'"));
                Assert.That(logins.TryAddOrUpdateTarget("no target", [ "csms-a" ],         null, out _),            Is.False, "an identification with a space");
                Assert.That(logins.TryAddOrUpdateTarget("t", Enumerable.Range(0, 9).Select(i => $"csms-{i}"), null, out _), Is.False, "nine peers");

                Assert.That(logins.TrySetTarget(CS001, "nowhere", out var noSuchTarget),                            Is.False, "a target there is not");
                Assert.That(noSuchTarget,                                                                           Does.Contain("'nowhere'"));
                Assert.That(logins.TrySetTarget(User_Id.Parse("cs999"), null, out _),                              Is.False, "a login there is not");

                Assert.That(logins.Targets,                                                                         Is.Empty, "something refused was kept");

            });

        }

        #endregion

        #region ATargetALoginNames_IsNotRemoved_UntilNoneNamesIt()

        /// <summary>
        /// A target a login names is not removed: its station would go to the back end in charge, a
        /// CSMS nobody chose for it. Once the login names none, it is.
        /// </summary>
        [Test]
        public void ATargetALoginNames_IsNotRemoved_UntilNoneNamesIt()
        {

            Assert.That(logins.TryAddOrUpdateTarget("site-1", [ "csms-a" ], null, out var error), Is.True, error);
            Assert.That(logins.TrySetTarget(CS001, "site-1", out error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(logins.TryRemoveTarget("site-1", out var refused),  Is.False);
                Assert.That(refused,                                            Does.Contain("1 login(s)"));
                Assert.That(logins.TargetOf(CS001)?.Id,                         Is.EqualTo("site-1"));
            });

            Assert.That(logins.TrySetTarget(CS001, null, out error), Is.True, error);
            Assert.That(logins.TryRemoveTarget("site-1", out error),   Is.True, error);

            Assert.Multiple(() => {
                Assert.That(logins.Targets,              Is.Empty);
                Assert.That(logins.TargetOf(CS001),      Is.Null);
                Assert.That(Load().Targets,              Is.Empty, "the removal was not written");
            });

        }

        #endregion

        #region AFile_WhoseLoginNamesATargetItDoesNotHold_IsRefused()

        /// <summary>
        /// A file whose login names a target the file does not hold is refused, as one whose login
        /// names a group it does not hold: the station is not quietly sent to the back end in charge.
        /// </summary>
        [Test]
        public void AFile_WhoseLoginNamesATargetItDoesNotHold_IsRefused()
        {

            var json = JObject.Parse(System.IO.File.ReadAllText(File));
            ((JObject) json["logins"]![0]!)["target"] = "gone";
            System.IO.File.WriteAllText(File, json.ToString());

            var again = new WebSocketLogins(File, clock);

            Assert.Multiple(() => {
                Assert.That(again.TryLoad(out var error),  Is.False);
                Assert.That(error,                         Does.Contain("'gone'"));
            });

        }

        #endregion

        #region APeerThatIsGone_StaysInTheTarget()

        /// <summary>
        /// A peer removed after a target named it stays in the target, which passes it over as one
        /// that is not connected; a target is not changed behind its writer's back.
        /// </summary>
        [Test]
        public void APeerThatIsGone_StaysInTheTarget()
        {

            Assert.That(logins.TryAddOrUpdateTarget("site-1", [ "csms-c", "csms-a" ], null, out var error), Is.True, error);

            var again = new WebSocketLogins(File, clock) { KnownPeerIds = () => [ "csms-a" ] };

            Assert.Multiple(() => {
                Assert.That(again.TryLoad(out var loadError),           Is.True, loadError);
                Assert.That(again.GetTarget("site-1")?.PeerIds,         Is.EqualTo(new[] { "csms-c", "csms-a" }));
            });

        }

        #endregion

    }

}
