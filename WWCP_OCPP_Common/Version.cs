﻿/*
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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The current OCPP version.
    /// </summary>
    public static class Version
    {

        /// <summary>
        /// This OCPP version 0.1 as text "v0.1".
        /// </summary>
        public const            String      String                   = "v0.1";

        /// <summary>
        /// This OCPP version "0.1" as version identification.
        /// </summary>
        public readonly static  Version_Id  Id                       = Version_Id.Parse(String[1..]);

        /// <summary>
        /// The current OCPP version identification for the HTTP Websocket connection setup.
        /// </summary>
        public const            String      WebSocketSubProtocolId   = "ocpp0.1";

    }

}