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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// What a node of OCPP 2.1 offers of its Device Model to components that
    /// read their values from somewhere else: adding and taking away
    /// components, making the instances a SetVariables request names anew, the
    /// sources of certificates, the monitors a CSMS set, and the queue for
    /// security events.
    /// </summary>
    /// <remarks>
    /// The Device Model bridges of WWCP_OCPP_Node are wired to a node through
    /// this, so that every kind of node that offers it - a charging station
    /// today, a local controller once it has a Device Model - is wired the same
    /// way, by the same call.
    /// </remarks>
    public interface IDeviceModelNode
    {

        /// <summary>
        /// The id of the node, which a request to it is addressed to.
        /// </summary>
        NetworkingNode_Id Id { get; }

        /// <summary>
        /// Add the given component, unless a component with the same name and
        /// instance is already there.
        /// </summary>
        /// <param name="Component">A component configuration.</param>
        Boolean AddComponent(ComponentConfig Component);

        /// <summary>
        /// Remove the component with the given name and instance, at the given
        /// EVSE if one is given.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="Instance">The optional case insensitive instance of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component.</param>
        Boolean RemoveComponent(String   Name,
                                String?  Instance   = null,
                                EVSE?    EVSE       = null);

        /// <summary>
        /// The components of the given name, at the given EVSE if one is given.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component.</param>
        IEnumerable<ComponentConfig> GetComponentConfigs(String  Name,
                                                         EVSE?   EVSE   = null);

        /// <summary>
        /// Every component of the node, of the station and of its EVSEs.
        /// </summary>
        IEnumerable<ComponentConfig> AllComponentConfigs { get; }

        /// <summary>
        /// Let the given source make the instances of its components that a
        /// SetVariables request names and the node does not have yet.
        /// </summary>
        /// <param name="Source">A source of new instances.</param>
        void AddNewInstancesSource(INewInstancesSource Source);

        /// <summary>
        /// The instance the given SetVariables request makes of the given
        /// component, from the first source of new instances that makes one -
        /// or null where none does.
        /// </summary>
        /// <param name="Component">A component the node does not have.</param>
        /// <param name="Request">The SetVariables request naming it.</param>
        ComponentConfig? NewInstanceOf(Component            Component,
                                       SetVariablesRequest  Request);

        /// <summary>
        /// Where the certificates a CSMS installs, lists and deletes are kept,
        /// by their use.
        /// </summary>
        CertificateSources CertificateSources { get; }

        /// <summary>
        /// The monitors a CSMS set.
        /// </summary>
        VariableMonitors VariableMonitors { get; }

        /// <summary>
        /// What the node keeps for the CSMS while it is out of reach.
        /// </summary>
        OfflineQueue OfflineQueue { get; }

        /// <summary>
        /// Read every monitored variable - those the filter lets through, where
        /// one is given - and send the events their monitors make of what
        /// changed. Answers how many there were.
        /// </summary>
        /// <param name="Which">Which monitored variables to read; all where null.</param>
        Task<Int32> CheckMonitorsAsync(Func<Component, Variable, Boolean>? Which = null);

        /// <summary>
        /// Send a security event, kept until the CSMS took it - or another back
        /// end, where one is given, in a queue of its own.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        /// <param name="Destination">Whom to tell; the CSMS where null.</param>
        Task QueueSecurityEventAsync(SecurityEventType    Type,
                                     String?              TechInfo      = null,
                                     DateTimeOffset?      Timestamp     = null,
                                     NetworkingNode_Id?   Destination   = null);

    }

}
