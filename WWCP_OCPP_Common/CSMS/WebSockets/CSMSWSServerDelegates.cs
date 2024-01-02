/*
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

    #region Common Connection Management

    /// <summary>
    /// A delegate for logging new HTTP Web Socket connections.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="NewConnection">The new HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="SharedSubprotocols">An enumeration of shared HTTP Web Sockets subprotocols.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCSMSNewWebSocketConnectionDelegate        (DateTime                           Timestamp,
                                                                      ICSMSChannel                       CSMSChannel,
                                                                      WebSocketServerConnection          NewConnection,
                                                                      NetworkingNode_Id                  NetworkingNodeId,
                                                                      EventTracking_Id                   EventTrackingId,
                                                                      IEnumerable<String>                SharedSubprotocols,
                                                                      CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a HTTP Web Socket CLOSE message.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusCode">The HTTP Web Socket Closing Status Code.</param>
    /// <param name="Reason">An optional HTTP Web Socket closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCSMSCloseMessageReceivedDelegate          (DateTime                           Timestamp,
                                                                      ICSMSChannel                       CSMSChannel,
                                                                      WebSocketServerConnection          Connection,
                                                                      NetworkingNode_Id                  NetworkingNodeId,
                                                                      EventTracking_Id                   EventTrackingId,
                                                                      WebSocketFrame.ClosingStatusCode   StatusCode,
                                                                      String?                            Reason,
                                                                      CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a closed TCP connection.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="Reason">An optional closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCSMSTCPConnectionClosedDelegate           (DateTime                           Timestamp,
                                                                      ICSMSChannel                       CSMSChannel,
                                                                      WebSocketServerConnection          Connection,
                                                                      NetworkingNode_Id                  NetworkingNodeId,
                                                                      EventTracking_Id                   EventTrackingId,
                                                                      String?                            Reason,
                                                                      CancellationToken                  CancellationToken);

    #endregion

    #region OCPP Requests

    /// <summary>
    /// A delegate for logging an OCPP JSON request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="JSONRequest">The incoming OCPP JSON request.</param>
    public delegate Task OnOCPPJSONRequestLogDelegate                (DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      JObject                     JSONRequest,
                                                                      CancellationToken           CancellationToken);

    /// <summary>
    /// A delegate for logging a binary OCPP request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="BinaryRequest">The incoming binary OCPP request.</param>
    public delegate Task OnOCPPBinaryRequestLogDelegate              (DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      Byte[]                      BinaryRequest,
                                                                      CancellationToken           CancellationToken);

    #endregion

    #region OCPP Responses

    /// <summary>
    /// A delegate for logging an OCPP JSON response after a JSON request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="JSONRequest">The incoming OCPP JSON request.</param>
    /// <param name="ResponseTimestamp">The timestamp of the incoming OCPP response.</param>
    /// <param name="JSONResponse">The outgoing OCPP JSON response.</param>
    /// <param name="ErrorResponse">In case of errors, the outgoing OCPP error response.</param>
    /// <param name="Runtime">The overall runtime of the request.</param>
    public delegate Task OnOCPPJSONRequestJSONResponseLogDelegate    (DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      JObject                     JSONRequest,
                                                                      DateTime                    ResponseTimestamp,
                                                                      JObject?                    JSONResponse,
                                                                      JArray?                     ErrorResponse,
                                                                      TimeSpan                    Runtime);

    /// <summary>
    /// A delegate for logging a binary OCPP response after a JSON request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="JSONRequest">The incoming OCPP JSON request.</param>
    /// <param name="ResponseTimestamp">The timestamp of the incoming OCPP response.</param>
    /// <param name="BinaryResponse">The outgoing binary OCPP response.</param>
    /// <param name="ErrorResponse">In case of errors, the outgoing OCPP error response.</param>
    /// <param name="Runtime">The overall runtime of the request.</param>
    public delegate Task OnOCPPJSONRequestBinaryResponseLogDelegate  (DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      JObject                     JSONRequest,
                                                                      DateTime                    ResponseTimestamp,
                                                                      Byte[]?                     BinaryResponse,
                                                                      JArray?                     ErrorResponse,
                                                                      TimeSpan                    Runtime);

    /// <summary>
    /// A delegate for logging an OCPP JSON response after a binary request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="BinaryRequest">The incoming binary OCPP request.</param>
    /// <param name="ResponseTimestamp">The timestamp of the incoming OCPP response.</param>
    /// <param name="JSONResponse">The outgoing OCPP JSON response.</param>
    /// <param name="ErrorResponse">In case of errors, the outgoing OCPP error response.</param>
    /// <param name="Runtime">The overall runtime of the request.</param>
    public delegate Task OnOCPPBinaryRequestJSONResponseLogDelegate  (DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      Byte[]                      BinaryRequest,
                                                                      DateTime                    ResponseTimestamp,
                                                                      JObject?                    JSONResponse,
                                                                      JArray?                     ErrorResponse,
                                                                      TimeSpan                    Runtime);

    /// <summary>
    /// A delegate for logging a binary OCPP response after a binary request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CSMSChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="RequestTimestamp">The timestamp of the incoming OCPP request.</param>
    /// <param name="BinaryRequest">The incoming binary OCPP request.</param>
    /// <param name="ResponseTimestamp">The timestamp of the incoming OCPP response.</param>
    /// <param name="BinaryResponse">The outgoing binary OCPP response.</param>
    /// <param name="ErrorResponse">In case of errors, the outgoing OCPP error response.</param>
    /// <param name="Runtime">The overall runtime of the request.</param>
    public delegate Task OnOCPPBinaryRequestBinaryResponseLogDelegate(DateTime                    Timestamp,
                                                                      ICSMSChannel                CSMSChannel,
                                                                      WebSocketServerConnection   Connection,
                                                                      NetworkingNode_Id           NetworkingNodeId,
                                                                      EventTracking_Id            EventTrackingId,
                                                                      DateTime                    RequestTimestamp,
                                                                      Byte[]                      BinaryRequest,
                                                                      DateTime                    ResponseTimestamp,
                                                                      Byte[]?                     BinaryResponse,
                                                                      JArray?                     ErrorResponse,
                                                                      TimeSpan                    Runtime);

    #endregion


    public delegate Task OnWebSocketJSONMessageRequestDelegate   (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  NetworkingNode_Id           DestinationNodeId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      RequestMessage,
                                                                  CancellationToken           CancellationToken);

    public delegate Task OnWebSocketJSONMessageResponseDelegate  (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  NetworkingNode_Id           DestinationNodeId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  JArray                      ResponseMessage,
                                                                  CancellationToken           CancellationToken);

    public delegate Task OnWebSocketTextErrorResponseDelegate    (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  String                      TextRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  String                      TextResponseMessage,
                                                                  CancellationToken           CancellationToken);



    public delegate Task OnWebSocketBinaryMessageRequestDelegate (DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  NetworkingNode_Id           DestinationNodeId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  Byte[]                      RequestMessage,
                                                                  CancellationToken           CancellationToken);

    public delegate Task OnWebSocketBinaryMessageResponseDelegate(DateTime                    Timestamp,
                                                                  ICSMSChannel                Server,
                                                                  WebSocketServerConnection   Connection,
                                                                  NetworkingNode_Id           DestinationNodeId,
                                                                  NetworkPath                 NetworkPath,
                                                                  EventTracking_Id            EventTrackingId,
                                                                  DateTime                    RequestTimestamp,
                                                                  JArray                      JSONRequestMessage,
                                                                  Byte[]                      BinaryRequestMessage,
                                                                  DateTime                    ResponseTimestamp,
                                                                  Byte[]                      ResponseMessage,
                                                                  CancellationToken           CancellationToken);

    //public delegate Task OnWebSocketBinaryErrorResponseDelegate  (DateTime                    Timestamp,
    //                                                              CSMSWSServer                Server,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              Byte[]                      RequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              Byte[]                      ResponseMessage);

}
