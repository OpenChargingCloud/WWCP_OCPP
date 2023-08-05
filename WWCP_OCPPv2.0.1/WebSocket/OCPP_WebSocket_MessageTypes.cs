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

namespace cloud.charging.open.protocols.OCPPv2_0_1.WebSockets
{

    /// <summary>
    /// Extension methods for the OCPP WebSocket message types.
    /// </summary>
    public static class OCPP_WebSocket_MessageTypesExtensions
    {

        public static Byte AsByte(this OCPP_WebSocket_MessageTypes MessageType)

            => MessageType switch {
                   OCPP_WebSocket_MessageTypes.CALL        => 2,
                   OCPP_WebSocket_MessageTypes.CALLRESULT  => 3,
                   OCPP_WebSocket_MessageTypes.CALLERROR   => 4,
                   _                                       => 0
               };

        public static OCPP_WebSocket_MessageTypes ParseMessageType(Byte MessageType)

            => MessageType switch {
                   2  => OCPP_WebSocket_MessageTypes.CALL,
                   3  => OCPP_WebSocket_MessageTypes.CALLRESULT,
                   4  => OCPP_WebSocket_MessageTypes.CALLERROR,
                   _  => OCPP_WebSocket_MessageTypes.Undefined
               };

    }


    /// <summary>
    /// The OCPP WebSocket message types.
    /// </summary>
    public enum OCPP_WebSocket_MessageTypes : Byte
    {

        Undefined    = 0,

        CALL         = 2,  // Client-to-Server

        CALLRESULT   = 3,  // Server-to-Client

        CALLERROR    = 4

    }

}
