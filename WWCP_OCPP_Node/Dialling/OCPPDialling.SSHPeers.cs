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

using cloud.charging.open.protocols.WWCP.Node;
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.WWCP.Node.KeyPins;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.WWCP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The SSH peers of this station: the hosts of its "ssh://" connections,
    /// each with the one SSH connection the station keeps to it. OCPP is a
    /// channel of that connection, and so is whatever else goes to that host
    /// over SSH and says SSHConnection Shared - the key exchange of a time
    /// server (section 8.3 of the white paper "NTS Key Establishment over
    /// SSH"), name resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The connection is the station's, not OCPP's. Whoever needs it first has
    /// it made: a station that starts cold may have the time over it before a
    /// single OCPP message went over it. The station signs in on it with its
    /// own SSH key, under the login of the connection's entry, and the host is
    /// believed by what that entry holds it to: no second key pair, no pin of
    /// its own, no connection of its own for the others.
    /// </para>
    /// <para>
    /// Nothing but OCPP goes over a connection whose host is held to no pinned
    /// key: one that is believed by a key learned on its first connection and
    /// not confirmed yet, or that would learn its key from whoever asked first.
    /// On probation the station sends nothing but its BootNotification, and
    /// executes nothing but a signed pin set (section 10.4 of the white paper
    /// "OCPP over SSH"); time or names from that host would be time or names
    /// from a server nobody vouched for yet.
    /// </para>
    /// </remarks>
    public partial class OCPPDialling
    {

        #region (override) SignsInAtSSHPeersWithItsKey / DefaultSSHPeerUser

        /// <summary>
        /// A node of OCPP signs in at its SSH peers with the key of its
        /// connections, as a peer it dialled over ssh:// shares it.
        /// </summary>
        public override Boolean SignsInAtSSHPeersWithItsKey

            => true;

        /// <summary>
        /// It signs in as its OCPP 2.1 node is named.
        /// </summary>
        public override String? DefaultSSHPeerUser

            => Node21.Id.ToString();

        #endregion

    }

}
