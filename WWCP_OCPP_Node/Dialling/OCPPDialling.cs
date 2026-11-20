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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;

using cloud.charging.open.protocols.WWCP.Node.Connections;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What OCPP says of the connections of a node that dials over it: a
    /// charging station, a CSMS, a local controller or a gateway. They are
    /// dialled by the node's OCPP nodes - the one for OCPP 2.1, and the one for
    /// OCPP 1.6 where it has one - offer OCPP's subprotocols, go over the SSH
    /// subsystem of the white paper "OCPP over SSH", and are what the Device
    /// Model of OCPP 2.1 reads and changes as SSHClientController and
    /// TLSClientController.
    /// </summary>
    /// <remarks>
    /// A node gives itself one (<see cref="WWCPNode.Dialling"/>); how its
    /// connections are dialled, followed, believed, let go of and dialled
    /// again is <see cref="WWCPNode"/>'s and the same for every node.
    /// </remarks>
    public partial class OCPPDialling : DiallingProtocol
    {

        #region Data

        private readonly Func<OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?>  node16;
        private readonly Func<OCPPv2_1.NetworkingNode.AOCPPNetworkingNode>   node21;

        #endregion

        #region Properties

        /// <summary>
        /// The node that speaks OCPP 1.6 for the node, where it has one.
        /// </summary>
        public OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Node16
            => node16();

        /// <summary>
        /// The node that speaks OCPP 2.1 for the node.
        /// </summary>
        public OCPPv2_1.NetworkingNode.AOCPPNetworkingNode   Node21
            => node21();

        /// <summary>
        /// The OCPP nodes the node has: the one for OCPP 2.1, and before it
        /// the one for OCPP 1.6 where it speaks that.
        /// </summary>
        public IEnumerable<OCPP.NetworkingNode.AOCPPNetworkingNode> Nodes

            => Node16 is OCPPv1_6.NetworkingNode.AOCPPNetworkingNode node16
                   ? [ node16, Node21 ]
                   : [ Node21 ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// What OCPP says of the connections of the given node, whose OCPP
        /// nodes are asked for whenever they are needed: a node that is made
        /// anew - its EVSEs changed - is the one dialled with from then on.
        /// </summary>
        /// <param name="WWCPNode">The node whose connections these are.</param>
        /// <param name="Node16">The node that speaks OCPP 1.6 for it, where it has one.</param>
        /// <param name="Node21">The node that speaks OCPP 2.1 for it.</param>
        public OCPPDialling(WWCPNode                                            WWCPNode,
                            Func<OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?>  Node16,
                            Func<OCPPv2_1.NetworkingNode.AOCPPNetworkingNode>   Node21)

            : base(WWCPNode)

        {

            this.node16  = Node16;
            this.node21  = Node21;

        }

        #endregion

    }

}
