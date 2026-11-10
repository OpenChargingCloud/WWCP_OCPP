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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// What a station keeps for the back ends it tells things to: a queue for
    /// each of them, held, sent and tried again on its own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One queue per node held everything for the one CSMS a node had. With a
    /// second back end beside it - a successor, during a migration - a queue of
    /// its own for that one keeps the two apart: a back end out of reach for
    /// hours holds up only what is meant for it, and is never handed what is
    /// meant for the other.
    /// </para>
    /// <para>
    /// The queue of the CSMS - the destination a node sends its own messages to
    /// - is the node's <c>OfflineQueue</c>, as it was, and is bounded as that
    /// one is. A queue for any other destination is made the first time
    /// something is queued for it, in memory, and bounded to
    /// <see cref="OtherMaxMessages"/> messages and <see cref="OtherMaxAge"/>.
    /// </para>
    /// <para>
    /// A queue let go of messages for room or for age, and its back end is told
    /// so with a security event "QueueOverflow" before the next message it gets
    /// from that queue: how many there were. Told once, and again only for
    /// what is let go after.
    /// </para>
    /// </remarks>
    /// <param name="CSMS">The destination a node sends its own messages to.</param>
    /// <param name="CSMSQueue">The queue of that destination: the node's own.</param>
    /// <param name="TrySend">Send one message to a destination, answering whether it was taken.</param>
    /// <param name="TellOfOverflow">Tell a destination how many messages of its queue were let go, answering whether it was told.</param>
    /// <param name="Clock">What says when it is.</param>
    public sealed class Outboxes(NetworkingNode_Id                                          CSMS,
                                 Func<OfflineQueue>                                         CSMSQueue,
                                 Func<QueuedMessage, NetworkingNode_Id, Task<Boolean>>      TrySend,
                                 Func<Int64, NetworkingNode_Id, Task<Boolean>>              TellOfOverflow,
                                 TimeProvider?                                              Clock   = null)
    {

        #region (class) Outbox

        private sealed class Outbox(OfflineQueue? Queue)
        {

            /// <summary>
            /// The queue of a destination other than the CSMS; null for the CSMS's, which is asked for each time.
            /// </summary>
            public OfflineQueue?   Queue      { get; } = Queue;

            public SemaphoreSlim   Flushing   { get; } = new (1, 1);
            public DateTimeOffset  NextTry    { get; set; }
            public Boolean         Held       { get; set; }

            /// <summary>
            /// How many of the queue's dropped messages its back end was told of.
            /// </summary>
            public Int64           Told       { get; set; }

        }

        #endregion

        #region Data

        /// <summary>
        /// The most messages kept for a destination other than the CSMS.
        /// </summary>
        public const           Int32     OtherMaxMessages  = 10_000;

        /// <summary>
        /// The oldest a message kept for a destination other than the CSMS may be.
        /// </summary>
        public static readonly TimeSpan  OtherMaxAge       = TimeSpan.FromDays(7);

        private readonly ConcurrentDictionary<NetworkingNode_Id, Outbox>  outboxes  = new();
        private readonly TimeProvider                                      clock     = Clock ?? TimeProvider.System;

        #endregion

        #region Properties

        /// <summary>
        /// The destination a node sends its own messages to.
        /// </summary>
        public NetworkingNode_Id               CSMS          { get; } = CSMS;

        /// <summary>
        /// How long to wait before a queue is tried again after its back end did not take a message.
        /// </summary>
        public TimeSpan                        Retry         { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Every destination something was queued for, the CSMS first.
        /// </summary>
        public IEnumerable<NetworkingNode_Id>  Destinations
            => [ CSMS, .. outboxes.Keys.Where(destination => destination != CSMS) ];

        #endregion


        #region QueueFor(Destination)

        /// <summary>
        /// The queue of the given destination: the node's own for the CSMS, and
        /// one made the first time it is asked for any other.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        public OfflineQueue QueueFor(NetworkingNode_Id Destination)

            => Destination == CSMS
                   ? CSMSQueue()
                   : OutboxOf(Destination).Queue!;

        #endregion

        #region Hold(Destination, Held) / IsHeld(Destination)

        /// <summary>
        /// Let what is queued for the given destination wait, or go again: for a
        /// back end that has not accepted the station yet.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        /// <param name="Held">Whether they wait.</param>
        public void Hold(NetworkingNode_Id  Destination,
                         Boolean            Held)

            => OutboxOf(Destination).Held = Held;

        /// <summary>
        /// Whether what is queued for the given destination waits.
        /// </summary>
        public Boolean IsHeld(NetworkingNode_Id Destination)

            => OutboxOf(Destination).Held;

        #endregion

        #region FlushAsync(Destination)

        /// <summary>
        /// Send what is queued for the given destination, oldest first, until its
        /// back end does not take a message - which is tried again after
        /// <see cref="Retry"/>. Answers how many were taken.
        /// </summary>
        /// <param name="Destination">Where the messages go.</param>
        public async Task<Int32> FlushAsync(NetworkingNode_Id Destination)
        {

            var outbox = OutboxOf(Destination);

            // Held: nothing goes, and nothing was refused either, so the queue
            // is not put off until its next try.
            if (outbox.Held)
                return 0;

            await outbox.Flushing.WaitAsync();

            try
            {

                var queue  = QueueFor(Destination);
                var taken  = 0;

                // Told first how many it will not get, before it gets the rest.
                var dropped = queue.Dropped;

                if (dropped > outbox.Told)
                {

                    if (!await TellOfOverflow(dropped - outbox.Told, Destination))
                    {
                        outbox.NextTry = clock.GetUtcNow() + Retry;
                        return 0;
                    }

                    outbox.Told = dropped;

                }

                while (queue.Pending.FirstOrDefault() is QueuedMessage message)
                {

                    if (!await TrySend(message, Destination))
                    {
                        outbox.NextTry = clock.GetUtcNow() + Retry;
                        break;
                    }

                    queue.Remove(message);
                    taken++;

                }

                return taken;

            }
            finally
            {
                outbox.Flushing.Release();
            }

        }

        #endregion

        #region DueForRetry()

        /// <summary>
        /// The destinations whose queue holds something, is not held, is not being
        /// sent, and whose time to be tried again has come - each put off by
        /// <see cref="Retry"/> from now, as it is about to be tried.
        /// </summary>
        public IReadOnlyList<NetworkingNode_Id> DueForRetry()
        {

            var now  = clock.GetUtcNow();
            var due  = new List<NetworkingNode_Id>();

            foreach (var destination in Destinations)
            {

                var outbox = OutboxOf(destination);

                if (outbox.Held || now < outbox.NextTry || outbox.Flushing.CurrentCount == 0)
                    continue;

                var queue = QueueFor(destination);

                if (queue.Pending.Count == 0 && queue.Dropped <= outbox.Told)
                    continue;

                outbox.NextTry = now + Retry;
                due.Add(destination);

            }

            return due;

        }

        #endregion


        #region (private) OutboxOf(Destination)

        private Outbox OutboxOf(NetworkingNode_Id Destination)

            => outboxes.GetOrAdd(
                   Destination,
                   destination => new Outbox(destination == CSMS
                                                 ? null
                                                 : new OfflineQueue(
                                                       MaxMessages:  OtherMaxMessages,
                                                       MaxAge:       OtherMaxAge,
                                                       Clock:        clock
                                                   ))
               );

        #endregion

    }

}
