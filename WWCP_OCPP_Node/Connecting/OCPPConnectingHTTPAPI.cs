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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.KeyPins;

using cloud.charging.open.protocols.WWCP.Node.Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The HTTP API of a node that connects to a CSMS, for what it connects to: its
    /// connections and what they prove themselves with, the keys and
    /// certificates it holds up, the state of each connection, and the test
    /// of one. The same routes for a charging station and for a local
    /// controller, each of which adds what else it has.
    /// </summary>
    /// <remarks>
    /// The routes themselves are every node's - see
    /// <see cref="NodeHTTPAPI.RegisterConnectionRoutes"/> in WWCP_Node.
    /// </remarks>
    public abstract class OCPPConnectingHTTPAPI : NodeHTTPAPI
    {

        #region Properties

        /// <summary>
        /// The node whose connections these are.
        /// </summary>
        protected WWCPNode          ConnectingNode               { get; }

        /// <summary>
        /// The resource a user needs a right to for reading, changing and
        /// testing the connections.
        /// </summary>
        protected String            ConnectionsResource    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The API of a node that connects to a CSMS, on the given HTTP server.
        /// </summary>
        /// <param name="ConnectionsResource">The resource a user needs a right to for the connections.</param>
        protected OCPPConnectingHTTPAPI(HTTPServer        HTTPServer,
                                      WWCPNode          Node,
                                      HTTPExtAPI        ExtAPI,
                                      EventLog          Log,
                                      String            ConnectionsResource,
                                      HTTPPath?         APIPath   = null,
                                      String?           Version   = null)

            : base(HTTPServer,
                   Node,
                   ExtAPI,
                   Log,
                   APIPath,
                   Version)

        {

            this.ConnectingNode             = Node;
            this.ConnectionsResource  = ConnectionsResource;

            // The command line of the node asks for the same right.
            Node.ConnectionsResource  = ConnectionsResource;

            RegisterConnectionRoutes(ConnectionsResource);

        }

        #endregion

    }

}
