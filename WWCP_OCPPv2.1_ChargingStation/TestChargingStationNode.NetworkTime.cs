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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// SyncNetworkTime and TestTimeServer of the white paper "Secure Time
    /// Synchronization", section 3.2.7, answered by the processors this
    /// station is given - see <see cref="INetworkTimeOnDemandNode"/>.
    /// </summary>
    public partial class TestChargingStationNode : INetworkTimeOnDemandNode
    {

        #region Properties

        /// <summary>
        /// What answers a SyncNetworkTime request: the time servers of whatever
        /// this station runs in. Without one, a SyncNetworkTime is answered
        /// Rejected with "UnknownGroup", since nothing here could be asked.
        /// </summary>
        public OnSyncNetworkTimeDelegate?  SyncNetworkTimeProcessor    { get; set; }

        /// <summary>
        /// What answers a TestTimeServer request. Without one, a TestTimeServer
        /// is answered Rejected with "UnknownClient".
        /// </summary>
        public OnTestTimeServerDelegate?   TestTimeServerProcessor     { get; set; }

        #endregion


        #region (private) WireNetworkTimeOnDemand()

        /// <summary>
        /// Answer SyncNetworkTime and TestTimeServer with the processors, once
        /// there are any, and with the reason there is nothing to ask before.
        /// </summary>
        private void WireNetworkTimeOnDemand()
        {

            #region OnSyncNetworkTime

            OCPP.IN.OnSyncNetworkTime += async (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SyncNetworkTime!");

                if (SyncNetworkTimeProcessor is OnSyncNetworkTimeDelegate processor)
                    return await processor(timestamp, sender, connection, request, ct);

                return SyncNetworkTimeResponse.Rejected(
                           request,
                           "UnknownGroup",
                           "This charging station has no time servers that could be asked."
                       );

            };

            #endregion

            #region OnTestTimeServer

            OCPP.IN.OnTestTimeServer += async (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming TestTimeServer!");

                if (TestTimeServerProcessor is OnTestTimeServerDelegate processor)
                    return await processor(timestamp, sender, connection, request, ct);

                return TestTimeServerResponse.Rejected(
                           request,
                           "UnknownClient",
                           "This charging station has no time servers that could be tested."
                       );

            };

            #endregion

        }

        #endregion

    }

}
