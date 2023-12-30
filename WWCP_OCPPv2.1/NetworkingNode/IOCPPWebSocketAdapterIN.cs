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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface IOCPPWebSocketAdapterIN : OCPP.NN.INetworkingNodeIncomingMessages,
                                               OCPP.NN.INetworkingNodeIncomingMessageEvents,

                                               CS.  INetworkingNodeIncomingMessages,
                                               CS.  INetworkingNodeIncomingMessageEvents,

                                               CSMS.INetworkingNodeIncomingMessages,
                                               CSMS.INetworkingNodeIncomingMessageEvents

    {

        #region HTTP Web Socket connection management

        ///// <summary>
        ///// An event sent whenever the HTTP connection switched successfully to web socket.
        ///// </summary>
        //event OnNetworkingNodeNewWebSocketConnectionDelegate?    OnNetworkingNodeNewWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever a web socket close frame was received.
        ///// </summary>
        //event OnNetworkingNodeCloseMessageReceivedDelegate?      OnNetworkingNodeCloseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a TCP connection was closed.
        ///// </summary>
        //event OnNetworkingNodeTCPConnectionClosedDelegate?       OnNetworkingNodeTCPConnectionClosed;

        #endregion



        Task<WebSocketTextMessageResponse>   ProcessJSONMessage  (DateTime              RequestTimestamp,
                                                                  IWebSocketConnection  WebSocketConnection,
                                                                  JArray                JSONMessage,
                                                                  EventTracking_Id      EventTrackingId,
                                                                  CancellationToken     CancellationToken);

        Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime              RequestTimestamp,
                                                                  IWebSocketConnection  WebSocketConnection,
                                                                  Byte[]                BinaryMessage,
                                                                  EventTracking_Id      EventTrackingId,
                                                                  CancellationToken     CancellationToken);


    }

}
