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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// What a node of OCPP 1.6 offers to configuration keys that read their
    /// values from somewhere else: the sources of those keys, the sources of
    /// certificates, and the queue for security events.
    /// </summary>
    /// <remarks>
    /// The configuration keys of WWCP_OCPP_Node's bridges are wired to a node
    /// through this, so that every kind of node that offers it is wired the
    /// same way, by the same call.
    /// </remarks>
    public interface IConfigurationKeysNode
    {

        /// <summary>
        /// Add a source of configuration keys, asked before the node's own keys
        /// and after the sources added before it.
        /// </summary>
        /// <param name="Source">A source of configuration keys.</param>
        void AddConfigurationKeySource(IConfigurationKeySource Source);

        /// <summary>
        /// The sources of configuration keys, in the order they are asked.
        /// </summary>
        IReadOnlyList<IConfigurationKeySource> ConfigurationKeySources { get; }

        /// <summary>
        /// Where the certificates a central system installs, lists and deletes
        /// are kept, by their use.
        /// </summary>
        CertificateSources CertificateSources { get; }

        /// <summary>
        /// What the node keeps for the central system while it is out of reach.
        /// </summary>
        OfflineQueue OfflineQueue { get; }

        /// <summary>
        /// Send a security event, kept until the central system took it - or
        /// another back end, where one is given, in a queue of its own.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        /// <param name="Destination">Whom to tell; the central system where null.</param>
        Task QueueSecurityEventAsync(SecurityEvent        Type,
                                     String?              TechInfo      = null,
                                     DateTimeOffset?      Timestamp     = null,
                                     NetworkingNode_Id?   Destination   = null);

    }

}
