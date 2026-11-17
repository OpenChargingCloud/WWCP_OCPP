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

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// A clock a test can move.
    /// </summary>
    /// <remarks>
    /// Everything in a store of the station server that asks what time it is asks the
    /// <see cref="TimeProvider"/> it was handed, which is what makes this
    /// possible at all: a session ends twelve hours after it was last used, and
    /// waiting for that is not a test anybody runs.
    ///
    /// Timers are left to the base class, which uses the real ones. Nothing
    /// tested through this clock waits on a timer - sessions expire when they
    /// are looked at, not when something fires.
    /// </remarks>
    internal sealed class TestClock(DateTimeOffset Start) : TimeProvider
    {

        #region Properties

        /// <summary>
        /// What this clock currently says.
        /// </summary>
        public DateTimeOffset Now { get; private set; } = Start;

        #endregion


        #region (static) At(Year, Month, Day)

        /// <summary>
        /// A clock standing at midnight UTC on the given day.
        /// </summary>
        public static TestClock At(Int32 Year, Int32 Month, Int32 Day)
            => new (new DateTimeOffset(Year, Month, Day, 0, 0, 0, TimeSpan.Zero));

        #endregion

        #region Advance(By)

        /// <summary>
        /// Move the clock forward.
        /// </summary>
        public TestClock Advance(TimeSpan By)
        {

            if (By < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(By), "This clock only goes forwards!");

            Now += By;

            return this;

        }

        #endregion

        #region (override) GetUtcNow()

        public override DateTimeOffset GetUtcNow()
            => Now;

        #endregion

    }

}
