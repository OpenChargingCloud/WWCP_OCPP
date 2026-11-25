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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// What a node holds back for a back end that is not connected: in the order it came, kept over
    /// a restart where it has a directory, no more than it takes - and which back end each charging
    /// session belongs to.
    /// </summary>
    public class HeldBackMessagesTests
    {

        #region Data

        private String directory = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeADirectory()
        {
            directory = Path.Combine(Path.GetTempPath(), "held-back-" + Guid.NewGuid().ToString("N")[..12]);
        }

        [TearDown]
        public void LetGo()
        {
            try
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, recursive: true);
            }
            catch (Exception)
            { }
        }

        #endregion

        #region (private static) Message(BackEnd, Number)

        private static HeldBackMessage Message(String BackEnd, Int32 Number)

            => new (BackEnd,
                    NetworkingNode_Id.Parse("cs001"),
                    OCPPVersion.OCPP2_1,
                    "TransactionEvent",
                    new JObject(new JProperty("seqNo", Number)),
                    NetworkingMode.Standard,
                    DateTimeOffset.Parse("2026-11-24T12:00:00Z").AddSeconds(Number));

        #endregion


        #region InTheOrderTheyCame_AndRemovedOneByOne()

        [Test]
        public void InTheOrderTheyCame_AndRemovedOneByOne()
        {

            var heldBack = new HeldBackMessages();

            Assert.Multiple(() => {
                Assert.That(heldBack.TryAdd(Message("a", 1)), Is.True);
                Assert.That(heldBack.TryAdd(Message("b", 1)), Is.True);
                Assert.That(heldBack.TryAdd(Message("a", 2)), Is.True);
            });

            var first = heldBack.FirstFor("a")!;

            Assert.Multiple(() => {
                Assert.That(heldBack.Count,          Is.EqualTo(3));
                Assert.That(heldBack.CountFor("a"),  Is.EqualTo(2));
                Assert.That(heldBack.BackEnds,       Is.EquivalentTo(new[] { "a", "b" }));
                Assert.That(first.Payload["seqNo"]?.Value<Int32>(), Is.EqualTo(1));
            });

            // Only the one that is first, and only once.
            heldBack.RemoveFirst("a", Message("a", 1));
            Assert.That(heldBack.CountFor("a"), Is.EqualTo(2), "a message that is not the first one was removed");

            heldBack.RemoveFirst("a", first);
            heldBack.RemoveFirst("a", first);

            Assert.Multiple(() => {
                Assert.That(heldBack.CountFor("a"),                              Is.EqualTo(1));
                Assert.That(heldBack.FirstFor("a")?.Payload["seqNo"]?.Value<Int32>(), Is.EqualTo(2));
            });

        }

        #endregion

        #region KeptOverARestart_InTheirDirectory_AndGoneFromItWhenSent()

        [Test]
        public void KeptOverARestart_InTheirDirectory_AndGoneFromItWhenSent()
        {

            var before = new HeldBackMessages(directory);

            before.TryAdd(Message("csms:01/a", 1));
            before.TryAdd(Message("csms:01/a", 2));
            before.Bind  (NetworkingNode_Id.Parse("cs001"), "tx-1", "csms:01/a");
            before.Bind  (NetworkingNode_Id.Parse("cs001"), "tx-2", "csms-02");
            before.Unbind(NetworkingNode_Id.Parse("cs001"), "tx-2");

            var after = new HeldBackMessages(directory);

            Assert.Multiple(() => {
                Assert.That(after.CountFor("csms:01/a"),                                 Is.EqualTo(2), "what was held back is gone after a restart");
                Assert.That(after.FirstFor("csms:01/a")?.Payload["seqNo"]?.Value<Int32>(), Is.EqualTo(1));
                Assert.That(after.FirstFor("csms:01/a")?.Received,                       Is.EqualTo(DateTimeOffset.Parse("2026-11-24T12:00:01Z")));
                Assert.That(after.BackEndOf(NetworkingNode_Id.Parse("cs001"), "tx-1"),   Is.EqualTo("csms:01/a"), "which back end a session belongs to is gone after a restart");
                Assert.That(after.BackEndOf(NetworkingNode_Id.Parse("cs001"), "tx-2"),   Is.Null, "a session that ended belongs to a back end after a restart");
            });

            after.RemoveFirst("csms:01/a", after.FirstFor("csms:01/a")!);
            after.RemoveFirst("csms:01/a", after.FirstFor("csms:01/a")!);
            after.Unbind(NetworkingNode_Id.Parse("cs001"), "tx-1");

            Assert.That(Directory.EnumerateFiles(directory), Is.Empty, "what was sent, and the sessions that ended, are still on disk");

        }

        #endregion

        #region NoMoreThanItTakes()

        [Test]
        public void NoMoreThanItTakes()
        {

            var heldBack = new HeldBackMessages(directory, Max: 2);

            Assert.Multiple(() => {
                Assert.That(heldBack.TryAdd(Message("a", 1)), Is.True);
                Assert.That(heldBack.TryAdd(Message("a", 2)), Is.True);
                Assert.That(heldBack.TryAdd(Message("a", 3)), Is.False, "one more than it takes was taken");
                Assert.That(heldBack.TryAdd(Message("b", 1)), Is.True,  "another back end's were counted with it");
                Assert.That(heldBack.CountFor("a"),           Is.EqualTo(2));
            });

        }

        #endregion

        #region FileNames_StandEverywhere_AndTellTheBackEndsApart()

        [TestCase("csms-01",            "csms-01.jsonl")]
        [TestCase("cd419af58ad63718",   "cd419af58ad63718.jsonl")]
        public void AFileName_IsTheBackEnd_WhereItMayStandInOne(String BackEnd, String FileName)

            => Assert.That(HeldBackMessages.FileNameOf(BackEnd), Is.EqualTo(FileName));

        [Test]
        public void FileNames_StandEverywhere_AndTellTheBackEndsApart()
        {

            var names = new[] { "a:b", "a/b", "a\\b", "a*b", "a?b", "a_b", "..", "CON", new String('x', 100) }.
                            Select(HeldBackMessages.FileNameOf).
                            ToArray();

            Assert.Multiple(() => {

                foreach (var name in names)
                    Assert.That(name.IndexOfAny([.. Path.GetInvalidFileNameChars(), ':', '/', '\\', '*', '?']), Is.EqualTo(-1), name);

                Assert.That(names.Distinct().Count(),   Is.EqualTo(names.Length), "two back ends share a file");
                Assert.That(names.Max(name => name.Length), Is.LessThanOrEqualTo(80));

            });

        }

        #endregion

        #region WhatCannotBeRead_IsLeftOut_AndSaidSo()

        [Test]
        public void WhatCannotBeRead_IsLeftOut_AndSaidSo()
        {

            Directory.CreateDirectory(directory);

            File.WriteAllText(Path.Combine(directory, "csms-01.jsonl"),
                              Message("csms-01", 1).ToJSON().ToString(Newtonsoft.Json.Formatting.None) + "\n" +
                              "{\"backEnd\":\"csms-01\"}\n");

            var problems = new List<String>();
            var heldBack = new HeldBackMessages(directory, OnProblem: problems.Add);

            Assert.Multiple(() => {
                Assert.That(heldBack.CountFor("csms-01"), Is.EqualTo(1), "what could be read was left out with what could not");
                Assert.That(problems,                     Has.One.Contains("could not be read, and is left out"), "what could not be read was left out without a word");
            });

        }

        #endregion

    }

}
