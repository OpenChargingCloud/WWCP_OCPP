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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// A delegate for logging new HTTP Web Socket connections.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="NewConnection">The new HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="SharedSubprotocols">An enumeration of shared HTTP Web Sockets subprotocols.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeNewWebSocketConnectionDelegate(DateTime                           Timestamp,
                                                                        INetworkingNodeWebsocketsChannel   NetworkingNodeChannel,
                                                                        IWebSocketConnection               NewConnection,
                                                                        NetworkingNode_Id                  NetworkingNodeId,
                                                                        EventTracking_Id                   EventTrackingId,
                                                                        IEnumerable<String>                SharedSubprotocols,
                                                                        CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a HTTP Web Socket CLOSE message.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusCode">The HTTP Web Socket Closing Status Code.</param>
    /// <param name="Reason">An optional HTTP Web Socket closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeCloseMessageReceivedDelegate(DateTime                           Timestamp,
                                                                      INetworkingNodeWebsocketsChannel   NetworkingNodeChannel,
                                                                      IWebSocketConnection               Connection,
                                                                      NetworkingNode_Id                  NetworkingNodeId,
                                                                      EventTracking_Id                   EventTrackingId,
                                                                      WebSocketFrame.ClosingStatusCode   StatusCode,
                                                                      String?                            Reason,
                                                                      CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a closed TCP connection.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="Reason">An optional closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeTCPConnectionClosedDelegate(DateTime                           Timestamp,
                                                                     INetworkingNodeWebsocketsChannel   NetworkingNodeChannel,
                                                                     IWebSocketConnection               Connection,
                                                                     NetworkingNode_Id                  NetworkingNodeId,
                                                                     EventTracking_Id                   EventTrackingId,
                                                                     String?                            Reason,
                                                                     CancellationToken                  CancellationToken);

}
