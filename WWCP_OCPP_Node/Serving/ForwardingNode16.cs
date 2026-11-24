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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The node of OCPP 1.6 of a node that serves charging stations and passes on what they send:
    /// it accepts the charging stations of OCPP 1.6 and connects to central systems of OCPP 1.6, and
    /// forwards whatever is not for itself - what a gateway or a local controller does, as their
    /// nodes of OCPP 2.1 do.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OCPP 1.6 has no node of its own for this: a charge point's would answer what a central system
    /// asks of it as a charge point, and a central system's would take what the stations send for
    /// itself. This one has no handler of anything, sends no heartbeat and keeps no HTTP API of its
    /// own - the node it serves has one HTTP server, on one port.
    /// </para>
    /// <para>
    /// What it forwards is decided by the gates of the node it serves, as for its node of OCPP 2.1:
    /// see OCPPServingNode.Gates.cs and OCPPServingNode.Targets.cs.
    /// </para>
    /// </remarks>
    public sealed class ForwardingNode16 : OCPPv1_6.NetworkingNode.AOCPPNetworkingNode
    {

        /// <summary>
        /// A node of OCPP 1.6 that forwards.
        /// </summary>
        /// <param name="Id">The name it goes by: the one of the node it serves.</param>
        /// <param name="Description">What it is.</param>
        /// <param name="DNSClient">The DNS client of the node it serves.</param>
        /// <param name="Clock">The clock of the node it serves.</param>
        public ForwardingNode16(NetworkingNode_Id  Id,
                                I18NString?        Description   = null,
                                IDNSClient?        DNSClient     = null,
                                TimeProvider?      Clock         = null)

            : base(Id,
                   Description,
                   DisableSendHeartbeats:    true,
                   DisableMaintenanceTasks:  true,
                   DNSClient:                DNSClient,
                   Clock:                    Clock)

        {

            // What nobody wrote a filter for is passed on, as a gateway's node
            // of OCPP 2.1 does: OCPP 1.6's default is to drop it.
            OCPP.FORWARD.DefaultForwardingDecision = ForwardingDecisions.FORWARD;

        }

    }

}
