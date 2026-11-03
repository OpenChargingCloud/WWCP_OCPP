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

using OfflineQueue = cloud.charging.open.protocols.OCPP.OfflineQueue;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Common
{

    /// <summary>
    /// The queue messages wait in while the CSMS is out of reach: bounded by
    /// number and by age, keeping security events before monitor events, and
    /// read back from its file.
    /// </summary>
    [TestFixture]
    public class OfflineQueueTests
    {

        #region (class) AClock

        private sealed class AClock : TimeProvider
        {
            public DateTimeOffset At { get; set; } = new (2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
            public override DateTimeOffset GetUtcNow() => At;
        }

        #endregion

        private static JObject Says(String Text)
            => new (new JProperty("text", Text));

        private static String[] TextsIn(OfflineQueue Queue)
            => [.. Queue.Pending.Select(message => message.Payload.Value<String>("text")!)];


        #region AFullQueueLetsMonitorEventsGoBeforeSecurityEvents()

        /// <summary>
        /// Full, the queue lets the oldest monitor event go first, and the oldest
        /// security event only where no monitor event is left - and counts them.
        /// </summary>
        [Test]
        public void AFullQueueLetsMonitorEventsGoBeforeSecurityEvents()
        {

            var queue = new OfflineQueue(MaxMessages: 3);

            queue.Enqueue("NotifyEvent",               Says("event 1"),    Evidence: false);
            queue.Enqueue("SecurityEventNotification", Says("security 1"), Evidence: true);
            queue.Enqueue("NotifyEvent",               Says("event 2"),    Evidence: false);
            queue.Enqueue("SecurityEventNotification", Says("security 2"), Evidence: true);

            var afterOne   = TextsIn(queue);

            queue.Enqueue("SecurityEventNotification", Says("security 3"), Evidence: true);

            var afterTwo   = TextsIn(queue);

            queue.Enqueue("SecurityEventNotification", Says("security 4"), Evidence: true);

            Assert.Multiple(() => {
                Assert.That(afterOne,         Is.EqualTo(new[] { "security 1", "event 2", "security 2" }));
                Assert.That(afterTwo,         Is.EqualTo(new[] { "security 1", "security 2", "security 3" }),  "a security event went before a monitor event");
                Assert.That(TextsIn(queue),   Is.EqualTo(new[] { "security 2", "security 3", "security 4" }));
                Assert.That(queue.Dropped,    Is.EqualTo(3));
            });

        }

        #endregion

        #region AMessageTooOldGoes()

        /// <summary>
        /// A message older than the queue keeps any goes at the next change.
        /// </summary>
        [Test]
        public void AMessageTooOldGoes()
        {

            var clock = new AClock();
            var queue = new OfflineQueue(MaxAge: TimeSpan.FromHours(1), Clock: clock);

            queue.Enqueue("SecurityEventNotification", Says("old"), Evidence: true);

            clock.At += TimeSpan.FromHours(2);

            queue.Enqueue("SecurityEventNotification", Says("new"), Evidence: true);

            Assert.Multiple(() => {
                Assert.That(TextsIn(queue),  Is.EqualTo(new[] { "new" }));
                Assert.That(queue.Dropped,   Is.EqualTo(1));
            });

        }

        #endregion

        #region TheFileIsReadBackAndALineThatIsNoneIsSkipped()

        /// <summary>
        /// The file holds the queue as it is, one message to a line, strings
        /// that look like a time kept as they were; a line that is no message
        /// is skipped when it is read back.
        /// </summary>
        [Test]
        public void TheFileIsReadBackAndALineThatIsNoneIsSkipped()
        {

            var directory = Path.Combine(Path.GetTempPath(), "ocpp-queue-" + Guid.NewGuid().ToString("N")[..12]);
            var file      = Path.Combine(directory, "queue.jsonl");

            try
            {

                var queue = new OfflineQueue(file);

                queue.Enqueue("SecurityEventNotification", Says("2026-11-03T12:00:00.000Z"), Evidence: true);
                queue.Enqueue("NotifyEvent",               Says("second"),                   Evidence: false);
                queue.Remove(queue.Pending[1]);

                File.AppendAllText(file, "this is no message" + Environment.NewLine);

                var again = new OfflineQueue(file);

                Assert.Multiple(() => {
                    Assert.That(TextsIn(again),                  Is.EqualTo(new[] { "2026-11-03T12:00:00.000Z" }));
                    Assert.That(again.Pending.Single().Evidence,  Is.True);
                    Assert.That(again.Pending.Single().Id,        Is.EqualTo(queue.Pending.Single().Id));
                });

            }
            finally
            {
                try { Directory.Delete(directory, recursive: true); } catch (IOException) { }
            }

        }

        #endregion

    }

}
