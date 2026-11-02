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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// A source of the standard clock controller holding its values in a
    /// dictionary and a list a test changes: a clock set by NTS from two of
    /// the PTB's servers, with neither a time zone nor a time offset.
    /// </summary>
    public sealed class InMemoryClockSource : IClockControllerSource
    {

        #region Properties

        public Dictionary<String, String>  Values        { get; } = new (StringComparer.OrdinalIgnoreCase) {
            ["DateTime"]    = "2026-11-02T12:00:00.000Z",
            ["NtpSource"]   = "manual",
            ["TimeSource"]  = "NTP,RealTimeClock"
        };

        /// <summary>
        /// The NTP servers, the one asked first first.
        /// </summary>
        public List<String>                NtpServers    { get; } = [ "nts://ptbtime1.ptb.de", "nts://ptbtime2.ptb.de:4461" ];

        #endregion

        #region IClockControllerSource

        public IEnumerable<String> Variables
            => [ .. Values.Keys, "NtpServerUri" ];

        public String? Value(String   Variable,
                             String?  Instance   = null)
        {

            if (String.Equals(Variable, "NtpServerUri", StringComparison.OrdinalIgnoreCase))
                return Int32.TryParse(Instance, out var priority) && priority >= 1
                           ? priority <= NtpServers.Count
                                 ? NtpServers[priority - 1]
                                 : ""
                           : null;

            return Values.TryGetValue(Variable, out var value)
                       ? value
                       : null;

        }

        #endregion

    }

}
