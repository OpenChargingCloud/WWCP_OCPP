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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Hermod;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The delegate for the HTTP WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Server">The sending WebSocket server.</param>
    /// <param name="JSONRequest">The incoming JSON request.</param>
    public delegate Task WebSocketJSONRequestLogHandler          (DateTimeOffset              Timestamp,
                                                                  IEventSender                Server,
                                                                  IWebSocketConnection        WebSocketConnection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTimeOffset              RequestTimestamp,
                                                                  JObject                     JSONRequest);

    /// <summary>
    /// The delegate for the HTTP WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Server">The sending WebSocket server.</param>
    /// <param name="BinaryRequest">The incoming binary request.</param>
    public delegate Task WebSocketBinaryRequestLogHandler        (DateTimeOffset              Timestamp,
                                                                  IEventSender                Server,
                                                                  IWebSocketConnection        WebSocketConnection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTimeOffset              RequestTimestamp,
                                                                  Byte[]                      BinaryRequest);


    // Responses

    public delegate Task WebSocketJSONRequestJSONResponseLogHandler    (DateTimeOffset              Timestamp,
                                                                        IEventSender                Server,
                                                                        IWebSocketConnection        WebSocketConnection,
                                                                        NetworkingNode_Id           DestinationId,
                                                                        NetworkPath                 NetworkPath,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTimeOffset              RequestTimestamp,
                                                                        JObject                     JSONRequest,
                                                                        JObject?                    JSONResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketJSONRequestBinaryResponseLogHandler  (DateTimeOffset              Timestamp,
                                                                        IEventSender                Server,
                                                                        IWebSocketConnection        WebSocketConnection,
                                                                        NetworkingNode_Id           DestinationId,
                                                                        NetworkPath                 NetworkPath,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTimeOffset              RequestTimestamp,
                                                                        JObject                     JSONRequest,
                                                                        Byte[]?                     BinaryResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketBinaryRequestJSONResponseLogHandler  (DateTimeOffset              Timestamp,
                                                                        IEventSender                Server,
                                                                        IWebSocketConnection        WebSocketConnection,
                                                                        NetworkingNode_Id           DestinationId,
                                                                        NetworkPath                 NetworkPath,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTimeOffset              RequestTimestamp,
                                                                        Byte[]                      BinaryRequest,
                                                                        JObject?                    JSONResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketBinaryRequestBinaryResponseLogHandler(DateTimeOffset              Timestamp,
                                                                        IEventSender                Server,
                                                                        IWebSocketConnection        WebSocketConnection,
                                                                        NetworkingNode_Id           DestinationId,
                                                                        NetworkPath                 NetworkPath,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTimeOffset              RequestTimestamp,
                                                                        Byte[]                      BinaryRequest,
                                                                        Byte[]?                     BinaryResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

}
