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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The delegate for the HTTP WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WebSocketRequestLogHandler              (DateTime                    Timestamp,
                                                                  WebSocketServer             WebSocketServer,
                                                                  JArray                      Request);

    /// <summary>
    /// The delegate for the HTTP WebSocket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketServer">The sending WebSocket server.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    public delegate Task WebSocketResponseLogHandler             (DateTime                    Timestamp,
                                                                  WebSocketServer             WebSocketServer,
                                                                  JArray                      Request,
                                                                  JArray                      Response);

    public delegate Task OnNewCSMSWSConnectionDelegate           (DateTime                    Timestamp,
                                                                  ICSMSChannel                CSMS,
                                                                  WebSocketServerConnection   NewWebSocketConnection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  CancellationToken           CancellationToken);



    public delegate Task OnWebSocketTextMessageRequestDelegate   (DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  String                      RequestMessage);

    public delegate Task OnWebSocketTextMessageResponseDelegate  (DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  String                      RequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  String?                     ResponseMessage);

    public delegate Task OnWebSocketTextErrorResponseDelegate    (DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  String                      RequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  String?                     ResponseMessage);


    public delegate Task OnWebSocketBinaryMessageRequestDelegate (DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage);

    public delegate Task OnWebSocketBinaryMessageResponseDelegate(DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  Byte[]?                     ResponseMessage);

    public delegate Task OnWebSocketBinaryErrorResponseDelegate  (DateTime                    Timestamp,
                                                                  CSMSWSServer                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  Byte[]?                     ResponseMessage);

}
