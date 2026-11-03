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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The instance a SetVariables request makes of the given component, which
    /// the node does not have yet, standing in for it while the request is
    /// set - or null where the request makes no such instance.
    /// </summary>
    /// <param name="Component">The component, with the instance the node does not have.</param>
    /// <param name="Request">The whole request: whether it makes the instance may depend on another entry of it.</param>
    public delegate ComponentConfig? NewInstanceLookupDelegate(Component            Component,
                                                               SetVariablesRequest  Request);


    /// <summary>
    /// Where instances of components are made by setting their variables: a
    /// SetVariables request may name an instance of one of its components that
    /// is not there yet, and the source says whether the request makes it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// What a white paper means when it lets a CSMS add a time server by
    /// naming a new NTPClientController instance and setting its variables in
    /// the same request: without this, the entries of the new instance would be
    /// UnknownComponent before the source ever saw them, and with ACID the
    /// whole request would be rolled back.
    /// </para>
    /// <para>
    /// The instance returned only stands in for the new one while the request
    /// is set: it is not added to the node, and a GetVariables or a report
    /// does not see it. Its variables say what may be set on a new instance,
    /// and its SetVariablesSource takes the change - where the instance is
    /// really made, together with the request's other entries. The same
    /// instance stands in for every entry of the request that names it.
    /// </para>
    /// </remarks>
    public interface INewInstancesSource
    {

        /// <summary>
        /// The names of the components of which a request may name an instance
        /// that is not there yet.
        /// </summary>
        IEnumerable<String> NewInstanceComponents { get; }

        /// <summary>
        /// The instance the given request makes of the given component, standing
        /// in for it while the request is set, or null where it makes none.
        /// </summary>
        /// <param name="Component">The component, with the instance the node does not have.</param>
        /// <param name="Request">The whole request.</param>
        ComponentConfig? NewInstance(Component            Component,
                                     SetVariablesRequest  Request);

    }

}
