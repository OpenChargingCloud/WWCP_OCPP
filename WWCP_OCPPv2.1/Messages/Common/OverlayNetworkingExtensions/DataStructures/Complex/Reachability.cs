/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The reachability of a networking node within the overlay network.
    /// </summary>
    public class Reachability
    {

        #region Properties

        public NetworkingNode_Id      DestinationId          { get; }

        public IOCPPWebSocketClient?  OCPPWebSocketClient    { get; }

        public IOCPPWebSocketServer?  OCPPWebSocketServer    { get; }

        public NetworkingNode_Id?     NetworkingHub          { get; }

        public Byte                   Priority               { get; }

        public Byte                   Weight                 { get; }

        public DateTime               Timestamp              { get; }

        public DateTime?              Timeout                { get; }

        #endregion

        #region Constructor(s)

        #region Reachability(DestinationId, OCPPWebSocketClient, Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id     DestinationId,
                            IOCPPWebSocketClient  OCPPWebSocketClient,
                            Byte?                 Priority    = 0,
                            Byte?                 Weight      = 1,
                            DateTime?             Timestamp   = null,
                            DateTime?             Timeout     = null)
        {

            this.DestinationId        = DestinationId;
            this.OCPPWebSocketClient  = OCPPWebSocketClient;
            this.Priority             = Priority  ?? 0;
            this.Weight               = Weight    ?? 1;
            this.Timestamp            = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout              = Timeout;

        }

        #endregion

        #region Reachability(DestinationId, OCPPWebSocketServer, Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id     DestinationId,
                            IOCPPWebSocketServer  OCPPWebSocketServer,
                            Byte?                 Priority    = 0,
                            Byte?                 Weight      = 1,
                            DateTime?             Timestamp   = null,
                            DateTime?             Timeout     = null)
        {

            this.DestinationId        = DestinationId;
            this.OCPPWebSocketServer  = OCPPWebSocketServer;
            this.Priority             = Priority  ?? 0;
            this.Weight               = Weight    ?? 1;
            this.Timestamp            = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout              = Timeout;

        }

        #endregion

        #region Reachability(DestinationId, NetworkingHub,       Priority = 0, Timestamp = null, Timeout = null)

        public Reachability(NetworkingNode_Id  DestinationId,
                            NetworkingNode_Id  NetworkingHub,
                            Byte?              Priority    = 0,
                            Byte?              Weight      = 1,
                            DateTime?          Timestamp   = null,
                            DateTime?          Timeout     = null)
        {

            this.DestinationId  = DestinationId;
            this.NetworkingHub  = NetworkingHub;
            this.Priority       = Priority  ?? 0;
            this.Weight         = Weight    ?? 1;
            this.Timestamp      = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            this.Timeout        = Timeout;

        }

        #endregion

        #endregion


        public JObject ToJSON(Boolean IgnoreDestinationId = false)
        {

            var json = JSONObject.Create(

                           IgnoreDestinationId
                               ? null
                               : new JProperty("destinationId",     DestinationId.                ToString()),

                           OCPPWebSocketClient is not null
                               ? new JProperty("webSocketClient",   OCPPWebSocketClient.RemoteURL.ToString())
                               : null,

                           OCPPWebSocketServer is not null
                               ? new JProperty("webSocketServer",   OCPPWebSocketServer.IPSocket. ToString())
                               : null,

                           NetworkingHub.HasValue
                               ? new JProperty("networkingHub",     NetworkingHub.                ToString())
                               : null,

                                 new JProperty("priority",          Priority),
                                 new JProperty("weight",            Weight),
                                 new JProperty("timestamp",         Timestamp.                    ToIso8601()),

                           Timeout.HasValue
                               ? new JProperty("timeout",           Timeout.Value.                ToIso8601())
                               : null

                       );

            return json;

        }


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (OCPPWebSocketClient is not null)
                return $"'{DestinationId}' via HTTP Web Socker client '{OCPPWebSocketClient.RemoteURL}'";

            if (OCPPWebSocketServer is not null)
                return $"'{DestinationId}' via HTTP Web Socker server '{OCPPWebSocketServer.IPSocket}'";

            if (OCPPWebSocketClient is not null)
                return $"'{DestinationId}' via networking hub '{NetworkingHub}'";

            return String.Empty;

        }

        #endregion


    }

}
