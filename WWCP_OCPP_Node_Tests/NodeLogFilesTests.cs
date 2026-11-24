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

using System.Text;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.TestKit;
using cloud.charging.open.protocols.OCPP.Node;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// The log a GetLog uploads: the node's log files of the days asked for,
    /// as far back as they go - or, without any, what its event log keeps -,
    /// the entries of the time and the kind asked for, and the newest of them
    /// where there are more than an upload may have.
    /// </summary>
    [TestFixture]
    public class NodeLogFilesTests
    {

        #region Data

        private String     directory  = "";
        private WWCPNode?  node;

        #endregion

        #region Setup / TearDown

        [SetUp]
        public void Setup()
        {
            directory = Path.Combine(Path.GetTempPath(), "node-logfiles-" + Guid.NewGuid().ToString("N")[..12]);
            Directory.CreateDirectory(directory);
        }

        [TearDown]
        public async Task TearDown()
        {

            if (node is not null)
                await node.DisposeAsync();

            node = null;

            try
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, recursive: true);
            }
            catch (Exception)
            { }

        }

        #endregion

        #region (private) ANode(WithLogFiles) / Lines(Request, MaxBytes)

        /// <summary>
        /// A node of its own directory - writing log files below it, where so.
        /// It is never started: its log is what is asked.
        /// </summary>
        private WWCPNode ANode(Boolean WithLogFiles)
        {

            File.WriteAllText(Path.Combine(directory, WWCPConfigFile.DefaultFileName),
                              """{ "nts": { "groups": { "legal": { "enabled": false } } }, "ssh": { "enabled": false } }""");

            node = new WWCPNode(HTTPPort:          TestPorts.Free(),
                                AccountsPath:      Path.Combine(directory, "accounts"),
                                ConfigFile:        new WWCPConfigFile(Path.Combine(directory, WWCPConfigFile.DefaultFileName)),
                                CertificatesPath:  Path.Combine(directory, "certificates"),
                                LogToConsole:      false,
                                LogPath:           WithLogFiles ? Path.Combine(directory, "logs") : null,
                                BridgeDebugLog:    false);

            return node;

        }

        /// <summary>
        /// The lines of the log a request asks for, read as JSON.
        /// </summary>
        private JObject[] Lines(FileRequest  Request,
                                Int64?       MaxBytes  = null)

            => [.. RawLines(Request, MaxBytes).Select(JObject.Parse)];

        /// <summary>
        /// The lines of the log a request asks for, as they are.
        /// </summary>
        private String[] RawLines(FileRequest  Request,
                                  Int64?       MaxBytes  = null)
        {

            var file = NodeLogFiles.Of(node!, Request, MaxBytes);

            Assert.That(file, Is.Not.Null);

            using var content = file!.Content;
            using var reader  = new StreamReader(content, Encoding.UTF8);

            return reader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

        }

        /// <summary>
        /// A day file the node wrote before this start - as its log files
        /// write them.
        /// </summary>
        private void AnOldDay(String Day, params String[] Lines)

            => File.WriteAllLines(Path.Combine(node!.LogPath!, $"{node.Kind.LogFilePrefix}-{Day}.log"), Lines);

        #endregion


        #region TheLogFilesAreRead_AsFarBackAsTheyGo_OfTheDaysAskedFor()

        /// <summary>
        /// A node that writes log files uploads them: the days before this
        /// start as well as today, the oldest first - and of the time asked
        /// for, only its days and its entries; of a SecurityLog only what is
        /// tagged "security".
        /// </summary>
        [Test]
        public void TheLogFilesAreRead_AsFarBackAsTheyGo_OfTheDaysAskedFor()
        {

            ANode(WithLogFiles: true);

            node!.Log.Info("Logged now.", "test");

            AnOldDay("2026-11-20", "2026-11-20T08:00:00.000Z info    [boot] Four days ago.",
                                   "2026-11-20T09:00:00.000Z warning [security auth] A refused login four days ago.");
            AnOldDay("2026-11-22", "2026-11-22T12:00:00.000Z notice  Two days ago.");

            var all       = Lines(new FileRequest("DiagnosticsLog", 1));
            var since     = Lines(new FileRequest("DiagnosticsLog", 2, Oldest: new DateTimeOffset(2026, 11, 21, 0, 0, 0, TimeSpan.Zero)));
            var between   = Lines(new FileRequest("DiagnosticsLog", 3, Oldest: new DateTimeOffset(2026, 11, 20, 8, 30, 0, TimeSpan.Zero),
                                                                       Latest: new DateTimeOffset(2026, 11, 22, 0, 0, 0, TimeSpan.Zero)));
            var security  = Lines(new FileRequest("SecurityLog",    4));

            Assert.Multiple(() => {

                Assert.That(all.Take(3).Select(line => line["message"]!.Value<String>()),
                            Is.EqualTo(new[] { "Four days ago.", "A refused login four days ago.", "Two days ago." }), "Not the days before this start, the oldest first.");
                Assert.That(all.Select(line => line["message"]!.Value<String>()),          Has.Some.EqualTo("Logged now."),          "Not what was logged now.");
                Assert.That(all[1]["tags"]!.Values<String>(),                               Is.EqualTo(new[] { "security", "auth" }));
                Assert.That(all[1]["level"]!.Value<String>(),                               Is.EqualTo("warning"));
                Assert.That(all[0].ContainsKey("id"),                                       Is.False,                                  "A line read from a file had no number to give.");

                Assert.That(since.Select(line => line["message"]!.Value<String>()),        Has.None.EqualTo("Four days ago."),       "A day before the time asked for.");
                Assert.That(since.Select(line => line["message"]!.Value<String>()),        Has.Some.EqualTo("Two days ago."));

                Assert.That(between.Select(line => line["message"]!.Value<String>()),
                            Is.EqualTo(new[] { "A refused login four days ago." }),                                                    "Not the entries of the time asked for, to the millisecond.");

                Assert.That(security.Select(line => line["message"]!.Value<String>()),
                            Is.EqualTo(new[] { "A refused login four days ago." }),                                                    "A SecurityLog of more than the entries tagged \"security\".");

            });

        }

        #endregion

        #region MoreThanAnUploadMayHave_TheNewestAreSent_AndTheFirstLineSaysHowManyAreLeftOut()

        /// <summary>
        /// The time asked for holds more than an upload may have: the newest
        /// entries are sent, the oldest first, as many as fit - and in front of
        /// them a line that says how many older ones are left out.
        /// </summary>
        [Test]
        public void MoreThanAnUploadMayHave_TheNewestAreSent_AndTheFirstLineSaysHowManyAreLeftOut()
        {

            ANode(WithLogFiles: true);

            AnOldDay("2026-11-20", [ .. Enumerable.Range(1, 100).Select(number => $"2026-11-20T08:00:{number / 10:00}.{number % 10:000}Z info    [test] Entry {number:000} of a day long ago.") ]);

            var request  = new FileRequest("DiagnosticsLog", 1, Latest: new DateTimeOffset(2026, 11, 20, 23, 0, 0, TimeSpan.Zero));
            var whole    = Lines(request);
            var oneLine  = Encoding.UTF8.GetByteCount(RawLines(request)[^1]) + 1;
            var cut      = Lines(request, MaxBytes: 10 * oneLine);

            Assert.Multiple(() => {

                Assert.That(whole,                                                           Has.Length.EqualTo(100));
                Assert.That(cut,                                                             Has.Length.EqualTo(11),                  "Not ten entries and the line in front of them.");
                Assert.That(cut[0]["message"]!.Value<String>(),                             Does.StartWith("90 older entries"),     "The first line does not say how many are left out.");
                Assert.That(cut.Skip(1).Select(line => line["message"]!.Value<String>()),
                            Is.EqualTo(Enumerable.Range(91, 10).Select(number => $"Entry {number:000} of a day long ago.")),       "Not the newest, the oldest of them first.");

            });

        }

        #endregion

        #region WithoutLogFiles_TheEventLogIsWhatIsSent()

        /// <summary>
        /// A node that writes no log files uploads what its event log keeps.
        /// </summary>
        [Test]
        public void WithoutLogFiles_TheEventLogIsWhatIsSent()
        {

            ANode(WithLogFiles: false);

            node!.Log.Info("Kept in memory.", "test");

            var lines = Lines(new FileRequest("DiagnosticsLog", 1));

            Assert.Multiple(() => {
                Assert.That(node.LogPath,                                                   Is.Null);
                Assert.That(lines.Select(line => line["message"]!.Value<String>()),        Has.Some.EqualTo("Kept in memory."));
                Assert.That(lines.Last()["id"],                                             Is.Not.Null, "An entry of the event log has its number.");
            });

        }

        #endregion

    }

}
