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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// Configuration keys of a charge point that live somewhere else and are
    /// read from there when asked: which keys there are, what each says, and
    /// what a change of one comes to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Which keys there are is computed at every GetConfiguration that names
    /// none: a key numbered by a server's place in a list ("dns.rootPins.3")
    /// comes and goes with the list. A named key is resolved by
    /// <see cref="Get"/>, whatever <see cref="Keys"/> said before.
    /// </para>
    /// <para>
    /// A source is asked before the charge point's own keys, and the first
    /// source that knows a key answers for it. A key no source knows and the
    /// charge point does not have is NotSupported.
    /// </para>
    /// </remarks>
    public interface IConfigurationKeySource
    {

        /// <summary>
        /// The keys of this source now.
        /// </summary>
        IEnumerable<String> Keys { get; }

        /// <summary>
        /// The key of this name, with what it says now - or null where it is
        /// none of this source's. The value of a write-only key is not sent.
        /// </summary>
        /// <param name="Key">The key, whatever its case.</param>
        ConfigurationKey? Get(String Key);

        /// <summary>
        /// Change the key of this name: Accepted, Rejected, RebootRequired or
        /// NotSupported - or null where it is none of this source's.
        /// </summary>
        /// <param name="Key">The key, whatever its case.</param>
        /// <param name="Value">What it is to say, up to 500 characters.</param>
        /// <param name="Sender">Who asked, for the log: the central system's id.</param>
        /// <param name="CancellationToken">To give up.</param>
        Task<ConfigurationStatus?> ChangeAsync(String             Key,
                                               String             Value,
                                               String?            Sender,
                                               CancellationToken  CancellationToken = default);

    }

}
