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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

//    /// <summary>
//    /// The delegate for the HTTP WebSocket request log.
//    /// </summary>
//    /// <param name="Timestamp">The timestamp of the incoming request.</param>
//    /// <param name="Server">The sending WebSocket server.</param>
//    /// <param name="JSONRequest">The incoming JSON request.</param>
//    public delegate Task WebSocketJSONRequestLogHandler          (DateTime                    Timestamp,
//                                                                  INetworkingNode             Server,
//                                                                  IWebSocketConnection        Connection,
//                                                                  NetworkingNode_Id           DestinationId,
//                                                                  EventTracking_Id            EventTrackingId,
//                                                                  DateTime                    RequestTimestamp,
//                                                                  JObject                     JSONRequest);

//    /// <summary>
//    /// The delegate for the HTTP WebSocket request log.
//    /// </summary>
//    /// <param name="Timestamp">The timestamp of the incoming request.</param>
//    /// <param name="Server">The sending WebSocket server.</param>
//    /// <param name="BinaryRequest">The incoming binary request.</param>
//    public delegate Task WebSocketBinaryRequestLogHandler        (DateTime                    Timestamp,
//                                                                  INetworkingNodeChannel      Server,
//                                                                  IWebSocketConnection        Connection,
//                                                                  NetworkingNode_Id           DestinationId,
//                                                                  EventTracking_Id            EventTrackingId,
//                                                                  DateTime                    RequestTimestamp,
//                                                                  Byte[]                      BinaryRequest);



//    // Responses

//    public delegate Task WebSocketJSONRequestJSONResponseLogHandler    (DateTime                    Timestamp,
//                                                                        INetworkingNodeChannel      Server,
//                                                                        IWebSocketConnection        Connection,
//                                                                        NetworkingNode_Id           DestinationId,
//                                                                        EventTracking_Id            EventTrackingId,
//                                                                        DateTime                    RequestTimestamp,
//                                                                        JObject                     JSONRequest,
//                                                                        DateTime                    ResponseTimestamp,
//                                                                        JObject?                    JSONResponse,
//                                                                        JArray?                     ErrorResponse,
//                                                                        TimeSpan                    Runtime);

//    public delegate Task WebSocketJSONRequestBinaryResponseLogHandler  (DateTime                    Timestamp,
//                                                                        INetworkingNodeChannel      Server,
//                                                                        IWebSocketConnection        Connection,
//                                                                        NetworkingNode_Id           DestinationId,
//                                                                        EventTracking_Id            EventTrackingId,
//                                                                        DateTime                    RequestTimestamp,
//                                                                        JObject                     JSONRequest,
//                                                                        DateTime                    ResponseTimestamp,
//                                                                        Byte[]?                     BinaryResponse,
//                                                                        JArray?                     ErrorResponse,
//                                                                        TimeSpan                    Runtime);

//    public delegate Task WebSocketBinaryRequestJSONResponseLogHandler  (DateTime                    Timestamp,
//                                                                        INetworkingNodeChannel      Server,
//                                                                        IWebSocketConnection        Connection,
//                                                                        NetworkingNode_Id           DestinationId,
//                                                                        EventTracking_Id            EventTrackingId,
//                                                                        DateTime                    RequestTimestamp,
//                                                                        Byte[]                      BinaryRequest,
//                                                                        DateTime                    ResponseTimestamp,
//                                                                        JObject?                    JSONResponse,
//                                                                        JArray?                     ErrorResponse,
//                                                                        TimeSpan                    Runtime);

//    public delegate Task WebSocketBinaryRequestBinaryResponseLogHandler(DateTime                    Timestamp,
//                                                                        INetworkingNodeChannel      Server,
//                                                                        IWebSocketConnection        Connection,
//                                                                        NetworkingNode_Id           DestinationId,
//                                                                        EventTracking_Id            EventTrackingId,
//                                                                        DateTime                    RequestTimestamp,
//                                                                        Byte[]                      BinaryRequest,
//                                                                        DateTime                    ResponseTimestamp,
//                                                                        Byte[]?                     BinaryResponse,
//                                                                        JArray?                     ErrorResponse,
//                                                                        TimeSpan                    Runtime);








//    //public delegate Task OnNewNetworkingNodeWSConnectionDelegate (DateTime                    Timestamp,
//    //                                                              INetworkingNodeChannel      NetworkingNode,
//    //                                                              IWebSocketConnection        NewWebSocketConnection,
//    //                                                              EventTracking_Id            EventTrackingId,
//    //                                                              CancellationToken           CancellationToken);



    public delegate Task OnWebSocketJSONMessageRequestDelegate   (DateTime                    Timestamp,
                                                                  INetworkingNode             Server,
                                                                  IWebSocketConnection        Connection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      RequestMessage,
                                                                  CancellationToken           CancellationToken);

    public delegate Task OnWebSocketJSONMessageResponseDelegate  (DateTime                    Timestamp,
                                                                  INetworkingNode             Server,
                                                                  IWebSocketConnection        Connection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  JArray                      ResponseMessage,
                                                                  CancellationToken           CancellationToken);

//    public delegate Task OnWebSocketTextErrorResponseDelegate    (DateTime                    Timestamp,
//                                                                  INetworkingNodeChannel      Server,
//                                                                  IWebSocketConnection        Connection,
//                                                                  EventTracking_Id            EventTrackingId,
//                                                                  DateTime                    RequestTimestamp,
//                                                                  String                      TextRequestMessage,
//                                                                  Byte[]                      BinaryRequestMessage,
//                                                                  DateTime                    ResponseTimestamp,
//                                                                  String?                     TextResponseMessage,
//                                                                  CancellationToken           CancellationToken);



    public delegate Task OnWebSocketBinaryMessageRequestDelegate (DateTime                    Timestamp,
                                                                  INetworkingNode             Server,
                                                                  IWebSocketConnection        Connection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage,
                                                                  CancellationToken           CancellationToken);

    public delegate Task OnWebSocketBinaryMessageResponseDelegate(DateTime                    Timestamp,
                                                                  INetworkingNode             Server,
                                                                  IWebSocketConnection        Connection,
                                                                  NetworkingNode_Id           DestinationId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  Byte[]?                     ResponseMessage,
                                                                  CancellationToken           CancellationToken);

//    //public delegate Task OnWebSocketBinaryErrorResponseDelegate  (DateTime                    Timestamp,
//    //                                                              INetworkingNodeChannel      Server,
//    //                                                              IWebSocketConnection        Connection,
//    //                                                              EventTracking_Id            EventTrackingId,
//    //                                                              DateTime                    RequestTimestamp,
//    //                                                              Byte[]                      RequestMessage,
//    //                                                              DateTime                    ResponseTimestamp,
//    //                                                              Byte[]?                     ResponseMessage);

}
