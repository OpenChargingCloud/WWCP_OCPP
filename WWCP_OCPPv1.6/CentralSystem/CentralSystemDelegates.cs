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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    #region Connection Management

    /// <summary>
    /// A delegate for logging new HTTP web socket connections.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CentralSystem">The OCPP central system.</param>
    /// <param name="NewConnection">The new HTTP web socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="SharedSubprotocols">An enumeration of shared HTTP Web Sockets subprotocols.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCentralSystemNewWebSocketConnectionDelegate(DateTime                           Timestamp,
                                                                       ICentralSystem                     CentralSystem,
                                                                       WebSocketServerConnection          NewConnection,
                                                                       NetworkingNode_Id                  NetworkingNodeId,
                                                                       EventTracking_Id                   EventTrackingId,
                                                                       IEnumerable<String>                SharedSubprotocols,
                                                                       CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a HTTP Web Socket CLOSE message.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="CentralSystem">The OCPP central system.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusCode">The HTTP Web Socket Closing Status Code.</param>
    /// <param name="Reason">An optional HTTP Web Socket closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCentralSystemCloseMessageReceivedDelegate  (DateTime                           Timestamp,
                                                                       ICentralSystem                     CentralSystem,
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
    /// <param name="CentralSystem">The OCPP central system.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">The event tracking identification for correlating this request with other events.</param>
    /// <param name="Reason">An optional closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnCentralSystemTCPConnectionClosedDelegate   (DateTime                           Timestamp,
                                                                       ICentralSystem                     CentralSystem,
                                                                       WebSocketServerConnection          Connection,
                                                                       NetworkingNode_Id                  NetworkingNodeId,
                                                                       EventTracking_Id                   EventTrackingId,
                                                                       String?                            Reason,
                                                                       CancellationToken                  CancellationToken);

    #endregion




    //public delegate Task OnWebSocketJSONMessageRequestDelegate   (DateTime                    Timestamp,
    //                                                              ICentralSystem              CentralSystem,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              ChargeBox_Id                ChargeBoxId,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              JArray                      RequestMessage,
    //                                                              CancellationToken           CancellationToken);

    //public delegate Task OnWebSocketJSONMessageResponseDelegate  (DateTime                    Timestamp,
    //                                                              ICentralSystem              CentralSystem,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              ChargeBox_Id                ChargeBoxId,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              JArray                      JSONRequestMessage,
    //                                                              Byte[]                      BinaryRequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              JArray                      ResponseMessage,
    //                                                              CancellationToken           CancellationToken);

    //public delegate Task OnWebSocketTextErrorResponseDelegate    (DateTime                    Timestamp,
    //                                                              ICentralSystem              CentralSystem,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              String                      TextRequestMessage,
    //                                                              Byte[]                      BinaryRequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              String                      TextResponseMessage,
    //                                                              CancellationToken           CancellationToken);



    //public delegate Task OnWebSocketBinaryMessageRequestDelegate (DateTime                    Timestamp,
    //                                                              ICentralSystem              CentralSystem,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              ChargeBox_Id                ChargeBoxId,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              Byte[]                      RequestMessage,
    //                                                              CancellationToken           CancellationToken);

    //public delegate Task OnWebSocketBinaryMessageResponseDelegate(DateTime                    Timestamp,
    //                                                              ICentralSystem              CentralSystem,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              ChargeBox_Id                ChargeBoxId,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              JArray                      JSONRequestMessage,
    //                                                              Byte[]                      BinaryRequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              Byte[]                      ResponseMessage,
    //                                                              CancellationToken           CancellationToken);

    //public delegate Task OnWebSocketBinaryErrorResponseDelegate  (DateTime                    Timestamp,
    //                                                              CSMSWSServer                Server,
    //                                                              WebSocketServerConnection   Connection,
    //                                                              EventTracking_Id            EventTrackingId,
    //                                                              DateTime                    RequestTimestamp,
    //                                                              Byte[]                      RequestMessage,
    //                                                              DateTime                    ResponseTimestamp,
    //                                                              Byte[]                      ResponseMessage);


}
