/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.tests
{

    /// <summary>
    /// The received messages an endpoint has begun to handle and not yet
    /// finished with.
    /// </summary>
    /// <remarks>
    /// The tests log what every endpoint sends and receives, and the logging
    /// handlers run after the work, not before it: a charging station's
    /// WebSocket client hands a message to the station first, which answers a
    /// request before the next handler - the one counting it - is called, and
    /// a message counts as sent only once it has been written. So the far
    /// side can hold the answer, and the test go on to read the counters,
    /// while the station is still between its answer and its log. That gap
    /// is a few microseconds, until a loaded machine schedules the station's
    /// thread late, and then a test sees a request answered that the station
    /// never received.
    ///
    /// Each endpoint is told when it begins a message - at a point before the
    /// message is acted upon - and when the last of its handlers is through,
    /// and a test waits for the two to meet before it reads the counters.
    /// Only the number is kept: a message that finishes without having begun,
    /// one the station could not even parse, leaves its endpoint looking idle
    /// rather than busy forever.
    /// </remarks>
    public sealed class MessagesInHand
    {

        #region Data

        private Int64 begun;
        private Int64 finished;

        #endregion

        #region Properties

        /// <summary>
        /// Whether every message this endpoint has begun is finished.
        /// </summary>
        public Boolean None
            => Interlocked.Read(ref begun) <= Interlocked.Read(ref finished);

        #endregion


        #region Begun()

        /// <summary>
        /// A message has been begun.
        /// </summary>
        public Task Begun()
        {
            Interlocked.Increment(ref begun);
            return Task.CompletedTask;
        }

        #endregion

        #region Finished()

        /// <summary>
        /// The last handler of a message is through.
        /// </summary>
        public Task Finished()
        {
            Interlocked.Increment(ref finished);
            return Task.CompletedTask;
        }

        #endregion

    }

}
