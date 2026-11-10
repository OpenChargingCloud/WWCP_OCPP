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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A node of OCPP 2.1 that answers QueryDNS with whoever resolves its names.
    /// </summary>
    /// <remarks>
    /// The node itself has no name resolution to test: that is the WWCP node it
    /// runs in, which WWCP_OCPP does not know. So it leaves the answer to the
    /// processor it is given - see DeviceModelBridge in WWCP_OCPP_Node - and,
    /// without one, says that it has nothing that could be asked.
    ///
    /// A processor and not one more handler of OnQueryDNS: the adapter asks
    /// the first handler of an event and no other, and the node's own comes
    /// first.
    /// </remarks>
    public interface IQueryDNSNode
    {

        /// <summary>
        /// What answers a QueryDNS request; null where nothing does.
        /// </summary>
        OnQueryDNSDelegate?  QueryDNSProcessor    { get; set; }

    }

}
