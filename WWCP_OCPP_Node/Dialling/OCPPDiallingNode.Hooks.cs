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

using cloud.charging.open.protocols.OCPP.Node.Connections;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What a node that dials a CSMS does of its own where the dialling is
    /// the same for all: nothing, unless it says otherwise.
    /// </summary>
    public abstract partial class OCPPDiallingNode
    {

        #region (protected virtual) ConnectionIsUp(Connection)

        /// <summary>
        /// The given connection is up, now or again. A charging station
        /// tells its CSMS that it started and sends what waited; a local
        /// controller has nothing of the kind to say.
        /// </summary>
        protected virtual void ConnectionIsUp(ConnectionEntry Connection)
        { }

        #endregion

        #region (protected virtual) EndRegistrations(Nodes) / EndRegistrationOf(Connection)

        /// <summary>
        /// Nothing more is asked of a CSMS for the given nodes, nor waited
        /// for: they are let go of.
        /// </summary>
        protected virtual void EndRegistrations(params AWWCPNetworkingNode?[] Nodes)
        { }

        #endregion

        #region (protected virtual) FlushOfflineQueueOf(Version)

        /// <summary>
        /// Send what the node of the given version of OCPP kept for its CSMS
        /// while it was out of reach.
        /// </summary>
        protected virtual Task FlushOfflineQueueOf(OCPPVersion Version)

            => Task.CompletedTask;

        #endregion

        #region (protected virtual) QueueSecurityEvent(Version, Type, TechInfo)

        /// <summary>
        /// Tell the CSMS of a security event, by the node of the given
        /// version of OCPP, now or once it is in reach.
        /// </summary>
        protected virtual void QueueSecurityEvent(OCPPVersion  Version,
                                                  String       Type,
                                                  String       TechInfo)
        { }

        #endregion

        #region (protected virtual) SSHClientControllersOf(Node) / AddComponentTo(Node, Component) / RemoveComponentFrom(Node, Name, Instance)

        /// <summary>
        /// The SSHClientControllers the device model of the given node has.
        /// </summary>
        protected virtual IEnumerable<OCPPv2_1.SSHClientController> SSHClientControllersOf(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)

            => [];

        /// <summary>
        /// Give the device model of the given node a component.
        /// </summary>
        protected virtual void AddComponentTo(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                              OCPPv2_1.ComponentConfig                     Component)
        { }

        /// <summary>
        /// Take a component from the device model of the given node.
        /// </summary>
        protected virtual void RemoveComponentFrom(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                                   String                                       Name,
                                                   String?                                      Instance)
        { }

        #endregion

        #region (protected virtual) AnswerCustomTriggersOf(Node, Answer)

        /// <summary>
        /// Have the given node answer a TriggerMessage for a custom trigger
        /// with what is given.
        /// </summary>
        protected virtual void AnswerCustomTriggersOf(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode                                    Node,
                                                      Func<OCPPv2_1.CSMS.TriggerMessageRequest, Task<OCPPv2_1.CS.TriggerMessageResponse?>>  Answer)
        { }

        #endregion

    }

}
