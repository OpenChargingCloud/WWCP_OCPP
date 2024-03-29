﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The delegate for the HTTP WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WebSocketRequestLogHandler (DateTime                    Timestamp,
                                                     WebSocketServer             WebSocketServer,
                                                     JArray                      Request);

    /// <summary>
    /// The delegate for the HTTP WebSocket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    public delegate Task WebSocketResponseLogHandler(DateTime                    Timestamp,
                                                     WebSocketServer             WebSocketServer,
                                                     JArray                      Request,
                                                     JArray                      Response);





    // Client Requests

    /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
    /// <param name="NetworkPath">The network path of the request.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="JSONRequest">The incoming JSON request.</param>
    public delegate Task WSClientJSONRequestLogHandler(DateTime                    Timestamp,
                                                       WebSocketClientConnection   Connection,
                                                       NetworkingNode_Id           NetworkingNodeId,
                                                       NetworkPath                 NetworkPath,
                                                       EventTracking_Id            EventTrackingId,
                                                       JObject                     JSONRequest);

    /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
    /// <param name="NetworkPath">The network path of the request.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="BinaryRequest">The incoming binary request.</param>
    public delegate Task WSClientBinaryRequestLogHandler(DateTime                    Timestamp,
                                                         WebSocketClientConnection   Connection,
                                                         NetworkingNode_Id           NetworkingNodeId,
                                                         NetworkPath                 NetworkPath,
                                                         EventTracking_Id            EventTrackingId,
                                                         Byte[]                      BinaryRequest);






    // Client Responses

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The request timestamp.</param>
    /// <param name="JSONRequest">The incoming JSON WebSocket request.</param>
    /// <param name="JSONResponse">The outgoing JSON WebSocket response.</param>
    /// <param name="ErrorResponse">The outgoing WebSocket error response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task WSClientJSONRequestJSONResponseLogHandler    (DateTime                    Timestamp,
                                                                       WebSocketClientConnection   Connection,
                                                                       NetworkingNode_Id           NetworkingNodeId,
                                                                       NetworkPath                 NetworkPath,
                                                                       EventTracking_Id            EventTrackingId,
                                                                       DateTime                    RequestTimestamp,
                                                                       JObject                     JSONRequest,
                                                                       JObject?                    JSONResponse,
                                                                       JArray?                     ErrorResponse,
                                                                       TimeSpan                    Runtime);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The request timestamp.</param>
    /// <param name="JSONRequest">The incoming JSON WebSocket request.</param>
    /// <param name="BinaryResponse">The outgoing binary WebSocket response.</param>
    /// <param name="ErrorResponse">The outgoing WebSocket error response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task WSClientJSONRequestBinaryResponseLogHandler  (DateTime                    Timestamp,
                                                                       WebSocketClientConnection   Connection,
                                                                       NetworkingNode_Id           NetworkingNodeId,
                                                                       NetworkPath                 NetworkPath,
                                                                       EventTracking_Id            EventTrackingId,
                                                                       DateTime                    RequestTimestamp,
                                                                       JObject                     JSONRequest,
                                                                       Byte[]?                     BinaryResponse,
                                                                       JArray?                     ErrorResponse,
                                                                       TimeSpan                    Runtime);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The request timestamp.</param>
    /// <param name="BinaryRequest">The incoming binary WebSocket request.</param>
    /// <param name="JSONResponse">The outgoing JSON WebSocket response.</param>
    /// <param name="ErrorResponse">The outgoing WebSocket error response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task WSClientBinaryRequestJSONResponseLogHandler  (DateTime                    Timestamp,
                                                                       WebSocketClientConnection   Connection,
                                                                       NetworkingNode_Id           NetworkingNodeId,
                                                                       NetworkPath                 NetworkPath,
                                                                       EventTracking_Id            EventTrackingId,
                                                                       DateTime                    RequestTimestamp,
                                                                       Byte[]                      BinaryRequest,
                                                                       JObject?                    JSONResponse,
                                                                       JArray?                     ErrorResponse,
                                                                       TimeSpan                    Runtime);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The request timestamp.</param>
    /// <param name="BinaryRequest">The incoming binary WebSocket request.</param>
    /// <param name="BinaryResponse">The outgoing binary WebSocket response.</param>
    /// <param name="ErrorResponse">The outgoing WebSocket error response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task WSClientBinaryRequestBinaryResponseLogHandler(DateTime                    Timestamp,
                                                                       WebSocketClientConnection   Connection,
                                                                       NetworkingNode_Id           NetworkingNodeId,
                                                                       NetworkPath                 NetworkPath,
                                                                       EventTracking_Id            EventTrackingId,
                                                                       DateTime                    RequestTimestamp,
                                                                       Byte[]                      BinaryRequest,
                                                                       Byte[]?                     BinaryResponse,
                                                                       JArray?                     ErrorResponse,
                                                                       TimeSpan                    Runtime);


}
