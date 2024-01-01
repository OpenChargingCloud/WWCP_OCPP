/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The reachability of a networking node within the overlay network.
    /// </summary>
    public class Reachability
    {

        #region Properties

        public NetworkingNode_Id      NetworkingNodeId       { get; }

        public IOCPPWebSocketClient?  OCPPWebSocketClient    { get; }

        public IOCPPWebSocketServer?  OCPPWebSocketServer    { get; }

        public NetworkingNode_Id?     NetworkingHub          { get; }

        public Byte                   Priority               { get; }

        public DateTime               Timestamp              { get; }

        public DateTime?              Timeout                { get; }

        #endregion

        #region Constructor(s)

        #region Reachability(NetworkingNodeId, OCPPWebSocketClient, Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id     NetworkingNodeId,
                            IOCPPWebSocketClient  OCPPWebSocketClient,
                            Byte?                 Priority    = 0,
                            DateTime?             Timestamp   = null,
                            DateTime?             Timeout     = null)
        {

            this.NetworkingNodeId     = NetworkingNodeId;
            this.OCPPWebSocketClient  = OCPPWebSocketClient;
            this.Priority             = Priority  ?? 0;
            this.Timestamp            = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout              = Timeout;

        }

        #endregion

        #region Reachability(NetworkingNodeId, OCPPWebSocketServer, Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id     NetworkingNodeId,
                            IOCPPWebSocketServer  OCPPWebSocketServer,
                            Byte?                 Priority    = 0,
                            DateTime?             Timestamp   = null,
                            DateTime?             Timeout     = null)
        {

            this.NetworkingNodeId     = NetworkingNodeId;
            this.OCPPWebSocketServer  = OCPPWebSocketServer;
            this.Priority             = Priority  ?? 0;
            this.Timestamp            = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout              = Timeout;

        }

        #endregion

        #region Reachability(NetworkingNodeId, NetworkingHub,       Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id  NetworkingNodeId,
                            NetworkingNode_Id  NetworkingHub,
                            Byte?              Priority    = 0,
                            DateTime?          Timestamp   = null,
                            DateTime?          Timeout     = null)
        {

            this.NetworkingNodeId     = NetworkingNodeId;
            this.NetworkingHub        = NetworkingHub;
            this.Priority             = Priority  ?? 0;
            this.Timestamp            = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout              = Timeout;

        }

        #endregion

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (OCPPWebSocketClient is not null)
                return $"'{NetworkingNodeId}' via HTTP Web Socker client '{OCPPWebSocketClient.RemoteURL}'";

            if (OCPPWebSocketServer is not null)
                return $"'{NetworkingNodeId}' via HTTP Web Socker server '{OCPPWebSocketServer.IPSocket}'";

            if (OCPPWebSocketClient is not null)
                return $"'{NetworkingNodeId}' via networking hub '{NetworkingHub}'";

            return String.Empty;

        }

        #endregion


    }

}
