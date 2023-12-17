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

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
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
    /// <param name="BinaryRequest">The incoming binary request.</param>
    public delegate Task WSClientBinaryRequestLogHandler(DateTime                    Timestamp,
                                                         WebSocketClientConnection   Connection,
                                                         NetworkingNode_Id           NetworkingNodeId,
                                                         NetworkPath                 NetworkPath,
                                                         EventTracking_Id            EventTrackingId,
                                                         Byte[]                      BinaryRequest);



    // Responses

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
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




    // Incoming JSON requests

    public delegate Task  OnWebSocketClientJSONMessageResponseDelegate  (DateTime                         Timestamp,
                                                                         INetworkingNodeWebSocketClient   Client,
                                                                         EventTracking_Id                 EventTrackingId,
                                                                         DateTime                         RequestTimestamp,
                                                                         JArray?                          JSONRequestMessage,
                                                                         Byte[]?                          BinaryRequestMessage,
                                                                         DateTime                         ResponseTimestamp,
                                                                         JArray                           ResponseMessage);


    public delegate Task  OnWebSocketClientBinaryMessageResponseDelegate(DateTime                         Timestamp,
                                                                         INetworkingNodeWebSocketClient   Client,
                                                                         EventTracking_Id                 EventTrackingId,
                                                                         DateTime                         RequestTimestamp,
                                                                         JArray?                          JSONRequestMessage,
                                                                         Byte[]?                          BinaryRequestMessage,
                                                                         DateTime                         ResponseTimestamp,
                                                                         Byte[]                           ResponseMessage);

}
