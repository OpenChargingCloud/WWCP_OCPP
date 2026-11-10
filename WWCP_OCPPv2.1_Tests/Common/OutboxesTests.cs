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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using OfflineQueue   = cloud.charging.open.protocols.OCPP.OfflineQueue;
using Outboxes       = cloud.charging.open.protocols.OCPP.Outboxes;
using QueuedMessage  = cloud.charging.open.protocols.OCPP.QueuedMessage;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Common
{

    /// <summary>
    /// A queue for each back end a station tells things to.
    /// </summary>
    /// <remarks>
    /// One queue per node held everything for the one CSMS a node had. With a
    /// successor beside it, a queue of its own keeps the two apart: a back end
    /// out of reach holds up only what is meant for it and is never handed what
    /// is meant for the other. And a queue that let go of messages for room or
    /// for age used to count them and tell nobody.
    /// </remarks>
    [TestFixture]
    public class OutboxesTests
    {

        #region Data

        private static readonly NetworkingNode_Id  CSMS       = NetworkingNode_Id.CSMS;
        private static readonly NetworkingNode_Id  Successor  = NetworkingNode_Id.Parse("connection-successor");

        private OfflineQueue                                              csmsQueue  = new();
        private readonly ConcurrentQueue<(String Kind, NetworkingNode_Id To)>  sent       = new();
        private readonly ConcurrentQueue<(Int64 Dropped, NetworkingNode_Id To)> told      = new();
        private readonly HashSet<NetworkingNode_Id>                       away       = [];
        private          Boolean                                          tellingFails;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            csmsQueue     = new OfflineQueue();
            tellingFails  = false;
            sent.Clear();
            told.Clear();
            away.Clear();
        }

        #endregion

        #region (private) NewOutboxes()

        /// <summary>
        /// Queues that send to whoever is not away, and tell of an overflow unless
        /// telling is to fail.
        /// </summary>
        private Outboxes NewOutboxes()

            => new (
                   CSMS,
                   () => csmsQueue,
                   (message, to) => {
                       if (away.Contains(to))
                           return Task.FromResult(false);
                       sent.Enqueue((message.Kind, to));
                       return Task.FromResult(true);
                   },
                   (dropped, to) => {
                       if (tellingFails || away.Contains(to))
                           return Task.FromResult(false);
                       told.Enqueue((dropped, to));
                       return Task.FromResult(true);
                   }
               );

        private static JObject Payload(Int32 Number)
            => new (new JProperty("n", Number));

        #endregion


        #region TheCSMSsQueueIsTheNodesOwn()

        [Test]
        public void TheCSMSsQueueIsTheNodesOwn()
        {

            var outboxes = NewOutboxes();

            Assert.That(outboxes.QueueFor(CSMS), Is.SameAs(csmsQueue));

        }

        #endregion

        #region AnotherBackEndHasAQueueOfItsOwnBoundedAsSaid()

        [Test]
        public void AnotherBackEndHasAQueueOfItsOwnBoundedAsSaid()
        {

            var outboxes  = NewOutboxes();
            var queue     = outboxes.QueueFor(Successor);

            Assert.Multiple(() => {

                Assert.That(queue,                           Is.Not.SameAs(csmsQueue));
                Assert.That(outboxes.QueueFor(Successor),    Is.SameAs(queue), "a second ask made a second queue");
                Assert.That(queue.MaxMessages,               Is.EqualTo(10_000));
                Assert.That(queue.MaxAge,                    Is.EqualTo(TimeSpan.FromDays(7)));
                Assert.That(outboxes.Destinations,           Is.EqualTo(new[] { CSMS, Successor }));

            });

        }

        #endregion

        #region ABackEndOutOfReachHoldsUpOnlyItsOwn()

        /// <summary>
        /// The successor is away: what waits for it stays, and what waits for the
        /// CSMS goes - to the CSMS, and only there.
        /// </summary>
        [Test]
        public async Task ABackEndOutOfReachHoldsUpOnlyItsOwn()
        {

            var outboxes = NewOutboxes();

            outboxes.QueueFor(Successor).Enqueue("ForTheSuccessor", Payload(1), Evidence: true);
            outboxes.QueueFor(CSMS).     Enqueue("ForTheCSMS",      Payload(2), Evidence: true);

            away.Add(Successor);

            var toTheSuccessor  = await outboxes.FlushAsync(Successor);
            var toTheCSMS       = await outboxes.FlushAsync(CSMS);

            Assert.Multiple(() => {

                Assert.That(toTheSuccessor,                              Is.Zero);
                Assert.That(toTheCSMS,                                   Is.EqualTo(1));
                Assert.That(outboxes.QueueFor(Successor).Pending.Count,  Is.EqualTo(1), "what waits for the successor went");
                Assert.That(csmsQueue.Pending,                           Is.Empty);
                Assert.That(sent,                                        Is.EqualTo(new[] { ("ForTheCSMS", CSMS) }),
                            "a message went somewhere else than to the back end it waited for");

            });

        }

        #endregion

        #region AHeldBackEndIsNotTried()

        [Test]
        public async Task AHeldBackEndIsNotTried()
        {

            var outboxes = NewOutboxes();

            outboxes.QueueFor(Successor).Enqueue("ForTheSuccessor", Payload(1), Evidence: true);
            outboxes.Hold(Successor, true);

            await Assert.MultipleAsync(async () => {

                Assert.That(await outboxes.FlushAsync(Successor),  Is.Zero);
                Assert.That(sent,                                  Is.Empty);
                Assert.That(outboxes.DueForRetry(),                Is.Empty, "a held back end was due to be tried again");

            });

            outboxes.Hold(Successor, false);

            Assert.That(await outboxes.FlushAsync(Successor), Is.EqualTo(1));

        }

        #endregion

        #region AnOverflowIsToldBeforeTheRest()

        /// <summary>
        /// Messages let go for room are told of, before what is left goes, and
        /// once.
        /// </summary>
        [Test]
        public async Task AnOverflowIsToldBeforeTheRest()
        {

            csmsQueue     = new OfflineQueue(MaxMessages: 2);

            var outboxes  = NewOutboxes();

            for (var number = 1; number <= 5; number++)
                csmsQueue.Enqueue("Event", Payload(number), Evidence: false);

            await outboxes.FlushAsync(CSMS);

            Assert.Multiple(() => {
                Assert.That(told,  Is.EqualTo(new[] { (3L, CSMS) }), "the CSMS was not told how many it will not get");
                Assert.That(sent,  Has.Count.EqualTo(2));
            });

            csmsQueue.Enqueue("Event", Payload(6), Evidence: false);

            await outboxes.FlushAsync(CSMS);

            Assert.That(told, Has.Count.EqualTo(1), "the same messages let go were told of twice");

        }

        #endregion

        #region ATellingThatFailsIsTriedAgainAndHoldsUpTheRest()

        [Test]
        public async Task ATellingThatFailsIsTriedAgainAndHoldsUpTheRest()
        {

            csmsQueue     = new OfflineQueue(MaxMessages: 1);

            var outboxes  = NewOutboxes();

            csmsQueue.Enqueue("Event", Payload(1), Evidence: false);
            csmsQueue.Enqueue("Event", Payload(2), Evidence: false);

            tellingFails = true;

            await Assert.MultipleAsync(async () => {
                Assert.That(await outboxes.FlushAsync(CSMS),  Is.Zero);
                Assert.That(sent,                             Is.Empty, "what is left went before the CSMS was told of what was let go");
            });

            tellingFails = false;

            await Assert.MultipleAsync(async () => {
                Assert.That(await outboxes.FlushAsync(CSMS),  Is.EqualTo(1));
                Assert.That(told,                             Is.EqualTo(new[] { (1L, CSMS) }));
            });

        }

        #endregion

    }

}
