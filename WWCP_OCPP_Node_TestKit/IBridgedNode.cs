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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// A node with its bridges, as the kit's tests see it: the node, which
    /// stays the only truth, the bridge made for it, and the OCPP nodes the
    /// bridge was handed last.
    /// </summary>
    public interface IBridgedNode : IAsyncDisposable
    {

        /// <summary>
        /// The node.
        /// </summary>
        WWCPNode                             Node            { get; }

        /// <summary>
        /// The bridge made for it.
        /// </summary>
        DeviceModelBridge                    Bridge          { get; }

        /// <summary>
        /// The OCPP 2.1 node handed to the bridge last.
        /// </summary>
        OCPPv2_1.IDeviceModelNode            OCPPv2_1Node    { get; }

        /// <summary>
        /// The OCPP 1.6 node handed to the bridge last.
        /// </summary>
        OCPPv1_6.CP.IConfigurationKeysNode   OCPPv1_6Node    { get; }

        /// <summary>
        /// Configure a connection to a CSMS of the given version, and never
        /// dial it: what the bridge keeps resolving and whom it tells.
        /// </summary>
        /// <param name="URL">Where the CSMS is, e.g. "ws://csms.example.invalid/cs".</param>
        /// <param name="OCPPVersion">"1.6" or "2.1".</param>
        /// <param name="Error">Why the node refused it.</param>
        Boolean TryDial(String       URL,
                        String       OCPPVersion,
                        out String?  Error);

        /// <summary>
        /// Build the OCPP nodes anew and hand them to the bridge, as a node
        /// does that rebuilds them - a charging station whose EVSEs changed.
        /// </summary>
        void NewOCPPNodes();

    }


    /// <summary>
    /// How a kind of node makes a node with its bridges, for the kit's tests.
    /// </summary>
    public interface IBridgedNodes
    {

        /// <summary>
        /// A node with its bridges, built and not started: nothing is asked of
        /// a time server or a name server.
        /// </summary>
        /// <param name="Directory">Where its accounts and its configuration go; created when it does not exist.</param>
        /// <param name="Configuration">What its configuration file says.</param>
        /// <param name="Clock">Where it reads the time, for a test that needs to decide what time it is.</param>
        IBridgedNode New(String         Directory,
                         JObject        Configuration,
                         TimeProvider?  Clock   = null);

        /// <summary>
        /// The same, started, with its web interface listening - made again on
        /// fresh ports where another test run took one before it could bind it.
        /// </summary>
        /// <param name="Directory">Where its accounts and its configuration go.</param>
        /// <param name="Configuration">What its configuration file says.</param>
        Task<IBridgedNode> Started(String   Directory,
                                   JObject  Configuration);

    }

}
