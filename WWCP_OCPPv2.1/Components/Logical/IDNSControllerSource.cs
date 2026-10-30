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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where the DNS controllers read how a charging station resolves names.
    /// </summary>
    /// <remarks>
    /// The controllers keep no value of their own: whatever holds the
    /// configuration answers each read, in the words of the Secure DNS white
    /// paper, so that a change made elsewhere is what the next read sees.
    /// </remarks>
    public interface IDNSControllerSource
    {

        /// <summary>
        /// The variables of DNSClientController this source can answer.
        /// </summary>
        IEnumerable<String>  ClientVariables    { get; }

        /// <summary>
        /// The variables of DNSServerController this source can answer.
        /// </summary>
        IEnumerable<String>  ServerVariables    { get; }

        /// <summary>
        /// The number of name servers the station takes: the instances of
        /// DNSServerController are "1" to this.
        /// </summary>
        UInt16               Slots              { get; }

        /// <summary>
        /// The value of a variable of DNSClientController, or null where there
        /// is no such variable.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        String? ClientValue(String Variable);

        /// <summary>
        /// The value of a variable of the DNSServerController in the given slot:
        /// "" for every variable of a slot no server is in, and null where there
        /// is no such slot or no such variable.
        /// </summary>
        /// <param name="Slot">The instance, from 1.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        String? ServerValue(UInt16  Slot,
                            String  Variable);

    }

}
