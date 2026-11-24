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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that is about the charging station server: its
    /// socket, the chains it accepts and the charging stations that may sign
    /// in. What it presents is the node's certificate store's - TLS server
    /// identities told the listener "ocpp", and their server keys.
    /// </summary>
    /// <remarks>
    /// Three pages worth of routes, in three groups, and the groups are not the
    /// same resource. Reading is reading; changing the server and the list of
    /// charging stations is "stations", a day's work on a site; generating a
    /// key and naming a certificate authority is "certificates", which is
    /// neither. Which resource "stations" is, is the kind's to say - see
    /// <see cref="StationsResource"/>.
    /// </remarks>
    public abstract partial class OCPPServingHTTPAPI
    {

        #region (private) RegisterOCPPServerRoutes()

        /// <summary>
        /// Everything under /v1/configuration/ocpp-server.
        /// </summary>
        private void RegisterOCPPServerRoutes()
        {

            var root = HTTPPath.Root + "v1/configuration/ocpp-server";

            AddHandler(root,                          GetOCPPServer,            HTTPMethod.GET);
            AddHandler(root,                          PutOCPPServer,            HTTPMethod.PUT);

            // What the station port presents is the node's: TLS server
            // identities of its store told the listener "ocpp", and the server
            // keys under /v1/certificates/serverKeys - see WWCP_Node.

            // The logins, their groups and the chains a client certificate may
            // lead to: every WebSocket server's routes - see WWCP_Node.
            RegisterWebSocketLoginRoutes(
                new WebSocketLoginRoutes(
                    root,
                    () => Serving.WebSocketLogins,
                    () => Serving.ClientTrust,
                    StationsResource,
                    "charging station",
                    [ "ocpp", "station" ],
                    Serving.StationLoginRemoved,
                    [
                        WebSocketLoginMark.LocalController,
                        WebSocketLoginMark.Gateway
                    ]
                )
            );

        }

        #endregion

        #region (private) GetOCPPServer(Request) / PutOCPPServer(Request)

        /// <summary>
        /// GET /api/v1/configuration/ocpp-server: the server the charging
        /// stations connect to.
        /// </summary>
        private Task<HTTPResponse> GetOCPPServer(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(StationsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.OCPPServerConfigurationJSON())
                   );

        }

        /// <summary>
        /// PUT /api/v1/configuration/ocpp-server: change it.
        /// </summary>
        /// <remarks>
        /// Answers with the whole thing as it now stands - including
        /// <c>state.waitingForARestart</c>, which is how the page finds out
        /// that the port it just changed is not the port that is listening.
        /// </remarks>
        private Task<HTTPResponse> PutOCPPServer(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Serving.TryUpdateOCPPServerConfiguration(json, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            Log.Info($"'{user.Id}' changed the charging station server.", "ocpp", "station", "config", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.OCPPServerConfigurationJSON())
                   );

        }

        #endregion

    }

}
