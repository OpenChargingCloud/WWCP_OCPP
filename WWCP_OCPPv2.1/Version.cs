/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The current OCPP version.
    /// </summary>
    public static class Version
    {

        /// <summary>
        /// This OCPP version 2.1 as text "v2.1".
        /// </summary>
        public const            String      String                   = "v2.1";

        /// <summary>
        /// This OCPP version "2.1" as version identification.
        /// </summary>
        public readonly static  Version_Id  Id                       = Version_Id.Parse(String[1..]);

        /// <summary>
        /// The current OCPP version identification for the HTTP Websocket connection setup.
        /// </summary>
        public const            String      WebSocketSubProtocolId   = "ocpp2.1";

    }

}
