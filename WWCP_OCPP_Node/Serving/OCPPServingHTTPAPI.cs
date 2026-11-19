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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The HTTP API of a node that dials a CSMS and serves the nodes below it:
    /// what every node that dials has - see <see cref="OCPPDiallingHTTPAPI"/> -
    /// and the routes of its charging station server, under
    /// /v1/configuration/ocpp-server - see OCPPServingHTTPAPI.OCPPServer.cs -
    /// and of commissioning over SSH, under /v1/configuration/commissioning -
    /// see OCPPServingHTTPAPI.Commissioning.cs.
    /// The same routes for a local controller and for a gateway, each of
    /// which adds what else it has.
    /// </summary>
    public abstract partial class OCPPServingHTTPAPI : OCPPDiallingHTTPAPI
    {

        #region Properties

        /// <summary>
        /// The node whose station server these routes are about.
        /// </summary>
        protected OCPPServingNode  Serving             { get; }

        /// <summary>
        /// The resource a user needs a right to for reading and changing the
        /// station server and the charging stations that may sign in to it;
        /// the certificates it presents and the chains it accepts are the
        /// node's "certificates".
        /// </summary>
        protected String           StationsResource    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The API of a node that dials a CSMS and serves the nodes below it,
        /// on the given HTTP server.
        /// </summary>
        /// <param name="ConnectionsResource">The resource a user needs a right to for the connections.</param>
        /// <param name="StationsResource">The resource a user needs a right to for the station server and its logins.</param>
        protected OCPPServingHTTPAPI(HTTPServer       HTTPServer,
                                     OCPPServingNode  Node,
                                     HTTPExtAPI       ExtAPI,
                                     EventLog         Log,
                                     String           ConnectionsResource,
                                     String           StationsResource,
                                     HTTPPath?        APIPath   = null,
                                     String?          Version   = null)

            : base(HTTPServer,
                   Node,
                   ExtAPI,
                   Log,
                   ConnectionsResource,
                   APIPath,
                   Version)

        {

            this.Serving           = Node;
            this.StationsResource  = StationsResource;

            RegisterOCPPServerRoutes();
            RegisterCommissioningRoutes();

        }

        #endregion

    }

}
