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
    /// A node of OCPP 2.1 that answers SyncNetworkTime and TestTimeServer of
    /// the white paper "Secure Time Synchronization", section 3.2.7, with
    /// whoever keeps its time.
    /// </summary>
    /// <remarks>
    /// The node itself has no time servers to ask: they are the WWCP node's it
    /// runs in, which WWCP_OCPP does not know. So it leaves the answer to the
    /// processors it is given - see DeviceModelBridge in WWCP_OCPP_Node - and,
    /// without them, says that it has nothing that could be asked.
    ///
    /// A processor answers at once, Accepted or Rejected, and sends the
    /// outcome in a NotifyNetworkTimeResult request once the round or the test
    /// is over.
    ///
    /// Processors and not more handlers of OnSyncNetworkTime and
    /// OnTestTimeServer: the adapter asks the first handler of an event and no
    /// other, and the node's own comes first.
    /// </remarks>
    public interface INetworkTimeOnDemandNode
    {

        /// <summary>
        /// What answers a SyncNetworkTime request; null where nothing does.
        /// </summary>
        OnSyncNetworkTimeDelegate?  SyncNetworkTimeProcessor    { get; set; }

        /// <summary>
        /// What answers a TestTimeServer request; null where nothing does.
        /// </summary>
        OnTestTimeServerDelegate?   TestTimeServerProcessor     { get; set; }

    }

}
