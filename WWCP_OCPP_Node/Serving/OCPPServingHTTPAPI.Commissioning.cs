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

using cloud.charging.open.protocols.WWCP.Node.Web;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that is about commissioning over SSH - a tool
    /// speaking OCPP to this node on its SSH server - under
    /// /v1/configuration/commissioning: the same for every node that serves
    /// OCPP, under the resource "commissioning" of its role model.
    /// </summary>
    public abstract partial class OCPPServingHTTPAPI
    {

        #region (private) RegisterCommissioningRoutes()

        private void RegisterCommissioningRoutes()
        {
            AddHandler(HTTPPath.Root + "v1/configuration/commissioning", GetCommissioningConfiguration, HTTPMethod.GET);
            AddHandler(HTTPPath.Root + "v1/configuration/commissioning", PutCommissioningConfiguration, HTTPMethod.PUT);
        }

        #endregion


        #region (private) GetCommissioningConfiguration(Request) / PutCommissioningConfiguration(Request)

        /// <summary>
        /// GET /api/v1/configuration/commissioning: whether a tool may speak
        /// OCPP to this node over its SSH server, where, and who does.
        /// </summary>
        private Task<HTTPResponse> GetCommissioningConfiguration(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(OCPPServingNode.CommissioningResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.CommissioningJSON())
                   );

        }

        /// <summary>
        /// PUT /api/v1/configuration/commissioning with {"enabled"}: switch
        /// commissioning over SSH on or off, at once. Switched off, every tool
        /// connected is let go of.
        /// </summary>
        /// <remarks>
        /// Whoever may run it, as a rule - see OCPPServingNode.CommissioningResource.
        /// </remarks>
        private Task<HTTPResponse> PutCommissioningConfiguration(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(OCPPServingNode.CommissioningResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Serving.TryUpdateCommissioningConfiguration(json, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.CommissioningJSON())
                   );

        }

        #endregion

    }

}
