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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that hands out the Device Model of this node:
    /// its components of OCPP 2.1 with what their variables say now, and its
    /// configuration keys of OCPP 1.6 - for its OCPP Device Model page.
    /// </summary>
    /// <remarks>
    /// At the right to read the OCPP log (Achim 2026-11-26): whoever may read
    /// every OCPP message, which a GetVariables and its answer are among, may
    /// read what they would say. A value OCPP keeps WriteOnly is shown to a
    /// systemadmin alone - a systemadmin is hidden nothing.
    /// </remarks>
    public abstract partial class OCPPServingHTTPAPI
    {

        #region (private) RegisterDeviceModelRoutes()

        /// <summary>
        /// GET /api/v1/ocpp/deviceModel.
        /// </summary>
        private void RegisterDeviceModelRoutes()
        {

            AddHandler(HTTPPath.Root + "v1/ocpp/deviceModel",  GetDeviceModel,  HTTPMethod.GET);

        }

        #endregion

        #region (private) GetDeviceModel(Request)

        /// <summary>
        /// GET /api/v1/ocpp/deviceModel: the Device Model of this node as a
        /// tree - see <see cref="OCPPServingNode.DeviceModelJSON"/> -, and
        /// whether the values OCPP keeps WriteOnly are in it.
        /// </summary>
        private Task<HTTPResponse> GetDeviceModel(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, ToReadTheOCPPMessages, false, out var user, out var refused))
                return Task.FromResult(refused);

            var showWriteOnly = Node.RolesOf(user).Any(role => role.IsSystemAdmin);

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.OK,
                           new JObject(
                               new JProperty("writeOnlyShown",  showWriteOnly),
                               new JProperty("deviceModel",     Serving.DeviceModelJSON(showWriteOnly))
                           )
                       )
                   );

        }

        #endregion

    }

}
