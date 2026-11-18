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

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// A restart asked for whenever something changed, run one at a time:
    /// asked while one runs, it runs once more after it - however often it
    /// was asked meanwhile - so the last change is never left out, and two
    /// restarts never run at once.
    /// </summary>
    /// <remarks>
    /// A restart that already runs may have read what changed before the last
    /// change came, or after it; it cannot tell. So any change asked for once
    /// a restart has begun is taken by another one after it.
    /// </remarks>
    /// <param name="Restart">The restart.</param>
    /// <param name="Failed">What to do with a restart that threw; the next one is run all the same.</param>
    internal sealed class RestartsOneAtATime(Func<Task>         Restart,
                                             Action<Exception>  Failed)
    {

        /// <summary>
        /// How often a restart was asked for that no restart has begun to take
        /// care of yet, the one running included: 0 when none runs.
        /// </summary>
        private Int32 asked;

        /// <summary>
        /// Ask for a restart: run now where none runs, or once more after the
        /// one that runs.
        /// </summary>
        public void Ask()
        {

            // Only the first ask of an idle one starts the runs; every other
            // is seen by the run going on when it counts down what it took.
            if (Interlocked.Increment(ref asked) == 1)
                _ = Task.Run(RunAsync);

        }

        private async Task RunAsync()
        {

            Int32 taken;

            do
            {

                // Every ask counted up to here is taken by this restart, as it
                // has not begun yet; those that come later are another's.
                taken = Volatile.Read(ref asked);

                try
                {
                    await Restart();
                }
                catch (Exception e)
                {
                    // Not even a failure to say so may leave the count up: no
                    // restart would ever be run again.
                    try { Failed(e); } catch { }
                }

            }
            while (Interlocked.Add(ref asked, -taken) != 0);

        }

    }

}
