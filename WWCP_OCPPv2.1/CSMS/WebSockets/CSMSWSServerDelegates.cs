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
    /// <param name="Server">The sending WebSocket server.</param>
    /// <param name="JSONRequest">The incoming JSON request.</param>
    public delegate Task WebSocketJSONRequestLogHandler          (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  ChargingStation_Id          ChargingStationId,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JObject                     JSONRequest);

    /// <summary>
    /// The delegate for the HTTP WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="Server">The sending WebSocket server.</param>
    /// <param name="BinaryRequest">The incoming binary request.</param>
    public delegate Task WebSocketBinaryRequestLogHandler        (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  ChargingStation_Id          ChargingStationId,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      BinaryRequest);



    // Responses

    public delegate Task WebSocketJSONRequestJSONResponseLogHandler    (DateTime                    Timestamp,
                                                                        ICSMSChannel                Server,
                                                                        WebSocketServerConnection   Connection,
                                                                        ChargingStation_Id          ChargingStationId,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTime                    RequestTimestamp,
                                                                        JObject                     JSONRequest,
                                                                        DateTime                    ResponseTimestamp,
                                                                        JObject?                    JSONResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketJSONRequestBinaryResponseLogHandler  (DateTime                    Timestamp,
                                                                        ICSMSChannel                Server,
                                                                        WebSocketServerConnection   Connection,
                                                                        ChargingStation_Id          ChargingStationId,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTime                    RequestTimestamp,
                                                                        JObject                     JSONRequest,
                                                                        DateTime                    ResponseTimestamp,
                                                                        Byte[]?                     BinaryResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketBinaryRequestJSONResponseLogHandler  (DateTime                    Timestamp,
                                                                        ICSMSChannel                Server,
                                                                        WebSocketServerConnection   Connection,
                                                                        ChargingStation_Id          ChargingStationId,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTime                    RequestTimestamp,
                                                                        Byte[]                      BinaryRequest,
                                                                        DateTime                    ResponseTimestamp,
                                                                        JObject?                    JSONResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);

    public delegate Task WebSocketBinaryRequestBinaryResponseLogHandler(DateTime                    Timestamp,
                                                                        ICSMSChannel                Server,
                                                                        WebSocketServerConnection   Connection,
                                                                        ChargingStation_Id          ChargingStationId,
                                                                        EventTracking_Id            EventTrackingId,
                                                                        DateTime                    RequestTimestamp,
                                                                        Byte[]                      BinaryRequest,
                                                                        DateTime                    ResponseTimestamp,
                                                                        Byte[]?                     BinaryResponse,
                                                                        JArray?                     ErrorResponse,
                                                                        TimeSpan                    Runtime);








    public delegate Task OnNewCSMSWSConnectionDelegate           (DateTime                    Timestamp,
                                                                  ICSMSChannel                CSMS,
                                                                  WebSocketServerConnection   NewWebSocketConnection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  CancellationToken           CancellationToken);



    public delegate Task OnWebSocketJSONMessageRequestDelegate   (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      RequestMessage);

    public delegate Task OnWebSocketJSONMessageResponseDelegate  (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  JArray?                     ResponseMessage);

    public delegate Task OnWebSocketTextErrorResponseDelegate    (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  String                      TextRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  String?                     TextResponseMessage);



    public delegate Task OnWebSocketBinaryMessageRequestDelegate (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage);

    public delegate Task OnWebSocketBinaryMessageResponseDelegate(DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  Byte[]?                     ResponseMessage);

    //public delegate Task OnWebSocketBinaryErrorResponseDelegate  (DateTime                    Timestamp,
    //                                                              CSMSWSServer                Server,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              Byte[]                      RequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              Byte[]?                     ResponseMessage);

}
