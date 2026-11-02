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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where the standard clock controller reads what time a charging station
    /// has and where it comes from.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever keeps the time
    /// answers each read, so that a change made elsewhere is what the next
    /// read sees.
    /// </remarks>
    public interface IClockControllerSource
    {

        /// <summary>
        /// The variables of ClockCtrlr this source can answer.
        /// </summary>
        IEnumerable<String> Variables { get; }

        /// <summary>
        /// The value of a variable of ClockCtrlr, or null where there is no such
        /// variable.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Instance">The instance of the variable: "1" to "9" for NtpServerUri, "NextTransition" for the next TimeOffset, none otherwise.</param>
        String? Value(String   Variable,
                      String?  Instance   = null);

    }

}
