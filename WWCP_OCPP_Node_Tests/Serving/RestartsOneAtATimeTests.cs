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

using NUnit.Framework;

using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The restarts of HTTP/3 for another certificate: one at a time, and a
    /// change that comes while one runs is shown by one more after it - not
    /// left out, as it was while a second ask was simply dropped.
    /// </summary>
    public class RestartsOneAtATimeTests
    {

        #region Data

        private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

        #endregion

        #region (private class) HeldRestarts

        /// <summary>
        /// Restarts that each wait until the test lets them end, and say when
        /// they began - and how many ran at once.
        /// </summary>
        private sealed class HeldRestarts
        {

            private readonly BlockingCollection<TaskCompletionSource>  begun  = [];
            private          Int32                                     running;

            public Int32 Count;
            public Int32 MostAtOnce;

            public async Task Restart()
            {

                var now = Interlocked.Increment(ref running);
                InterlockedMax(ref MostAtOnce, now);
                Interlocked.Increment(ref Count);

                var end = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                begun.Add(end);

                await end.Task;

                Interlocked.Decrement(ref running);

            }

            /// <summary>
            /// The next restart to begin, once it has: the test ends it.
            /// </summary>
            public TaskCompletionSource NextBegun()
            {
                Assert.That(begun.TryTake(out var end, Patience), Is.True, "no restart began");
                return end!;
            }

            /// <summary>
            /// That no other restart begins within a while.
            /// </summary>
            public void NoneBegins()

                => Assert.That(begun.TryTake(out _, TimeSpan.FromMilliseconds(300)), Is.False, "a restart began that nothing asked for");

            private static void InterlockedMax(ref Int32 Target, Int32 Value)
            {
                Int32 seen;
                while ((seen = Volatile.Read(ref Target)) < Value &&
                       Interlocked.CompareExchange(ref Target, Value, seen) != seen) { }
            }

        }

        #endregion


        #region AnAsk_RunsARestart()

        /// <summary>
        /// Asked once while none runs, one restart runs - and no other after it.
        /// </summary>
        [Test]
        public void AnAsk_RunsARestart()
        {

            var held     = new HeldRestarts();
            var restarts = new RestartsOneAtATime(held.Restart, e => Assert.Fail(e.Message));

            restarts.Ask();
            held.NextBegun().SetResult();

            held.NoneBegins();
            Assert.That(held.Count, Is.EqualTo(1));

        }

        #endregion

        #region AChangeWhileARestartRuns_IsShownByOneMoreAfterIt()

        /// <summary>
        /// The case that was left out: a change that comes while a restart runs
        /// - which may have read the port before it - is shown by one more
        /// restart after it.
        /// </summary>
        [Test]
        public void AChangeWhileARestartRuns_IsShownByOneMoreAfterIt()
        {

            var held     = new HeldRestarts();
            var restarts = new RestartsOneAtATime(held.Restart, e => Assert.Fail(e.Message));

            restarts.Ask();
            var first = held.NextBegun();

            restarts.Ask();
            held.NoneBegins();

            first.SetResult();
            held.NextBegun().SetResult();

            held.NoneBegins();
            Assert.That(held.Count,      Is.EqualTo(2));
            Assert.That(held.MostAtOnce, Is.EqualTo(1));

        }

        #endregion

        #region ManyChangesWhileARestartRuns_AreShownByOneMore()

        /// <summary>
        /// However many changes come while a restart runs, one more restart
        /// shows them all: the last one reads the port as it is now.
        /// </summary>
        [Test]
        public void ManyChangesWhileARestartRuns_AreShownByOneMore()
        {

            var held     = new HeldRestarts();
            var restarts = new RestartsOneAtATime(held.Restart, e => Assert.Fail(e.Message));

            restarts.Ask();
            var first = held.NextBegun();

            for (var i = 0; i < 5; i++)
                restarts.Ask();

            first.SetResult();
            held.NextBegun().SetResult();

            held.NoneBegins();
            Assert.That(held.Count, Is.EqualTo(2));

        }

        #endregion

        #region AnAskAfterAllRan_RunsAgain()

        /// <summary>
        /// Once every restart ran, the next ask runs one again: nothing stays
        /// counted that would hold a later change back.
        /// </summary>
        [Test]
        public void AnAskAfterAllRan_RunsAgain()
        {

            var held     = new HeldRestarts();
            var restarts = new RestartsOneAtATime(held.Restart, e => Assert.Fail(e.Message));

            restarts.Ask();
            var first = held.NextBegun();
            restarts.Ask();
            first.SetResult();
            held.NextBegun().SetResult();
            held.NoneBegins();

            restarts.Ask();
            held.NextBegun().SetResult();

            held.NoneBegins();
            Assert.That(held.Count, Is.EqualTo(3));

        }

        #endregion

        #region ARestartThatThrows_IsSaid_AndTheNextRunsAllTheSame()

        /// <summary>
        /// A restart that throws is said so, and neither keeps the next restart
        /// back nor leaves the ask counted.
        /// </summary>
        [Test]
        public void ARestartThatThrows_IsSaid_AndTheNextRunsAllTheSame()
        {

            var held     = new HeldRestarts();
            var failures = new ConcurrentQueue<Exception>();
            var throws   = 1;

            var restarts = new RestartsOneAtATime(
                               async () => {
                                   await held.Restart();
                                   if (Interlocked.Exchange(ref throws, 0) == 1)
                                       throw new InvalidOperationException("the UDP port is not free");
                               },
                               e => {
                                   failures.Enqueue(e);
                                   throw new Exception("saying so failed as well");
                               }
                           );

            restarts.Ask();
            var first = held.NextBegun();
            restarts.Ask();
            first.SetResult();
            held.NextBegun().SetResult();
            held.NoneBegins();

            restarts.Ask();
            held.NextBegun().SetResult();
            held.NoneBegins();

            Assert.That(failures.Select(e => e.Message), Is.EqualTo(new[] { "the UDP port is not free" }));
            Assert.That(held.Count, Is.EqualTo(3));

        }

        #endregion

        #region AsksFromManyThreads_NeverRunTwoAtOnce()

        /// <summary>
        /// Asked from many threads at once, the restarts still run one at a
        /// time, and the last ask is always followed by a restart that began
        /// after it.
        /// </summary>
        [Test]
        public async Task AsksFromManyThreads_NeverRunTwoAtOnce()
        {

            var running     = 0;
            var mostAtOnce  = 0;
            var lastAsked   = 0L;
            var lastBegun   = 0L;
            var clock       = 0L;

            var restarts = new RestartsOneAtATime(
                               async () => {
                                   var now = Interlocked.Increment(ref running);
                                   if (now > Volatile.Read(ref mostAtOnce))
                                       Volatile.Write(ref mostAtOnce, now);
                                   Volatile.Write(ref lastBegun, Interlocked.Increment(ref clock));
                                   await Task.Delay(2);
                                   Interlocked.Decrement(ref running);
                               },
                               e => Assert.Fail(e.Message)
                           );

            await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(async () => {
                for (var i = 0; i < 25; i++)
                {
                    Volatile.Write(ref lastAsked, Interlocked.Increment(ref clock));
                    restarts.Ask();
                    await Task.Delay(1);
                }
            })));

            var asked = Volatile.Read(ref lastAsked);
            var until = DateTime.UtcNow + Patience;

            while (Volatile.Read(ref lastBegun) < asked && DateTime.UtcNow < until)
                await Task.Delay(10);

            Assert.That(Volatile.Read(ref lastBegun),  Is.GreaterThan(asked), "the last ask was not followed by a restart");
            Assert.That(Volatile.Read(ref mostAtOnce), Is.EqualTo(1),         "two restarts ran at once");

        }

        #endregion

    }

}
