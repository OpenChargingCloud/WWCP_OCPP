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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public delegate Task OnJSONMessageRequestReceivedDelegate   (DateTime                    Timestamp,
                                                                 IOCPPWebSocketAdapterIN     Server,
                                                                 OCPP_JSONRequestMessage     JSONRequestMessage);

    public delegate Task OnJSONMessageResponseReceivedDelegate  (DateTime                    Timestamp,
                                                                 IOCPPWebSocketAdapterIN     Server,
                                                                 OCPP_JSONResponseMessage    JSONResponseMessage);

    public delegate Task OnJSONErrorResponseReceivedDelegate    (DateTime                    Timestamp,
                                                                 IOCPPWebSocketAdapterIN     Server,
                                                                 OCPP_JSONErrorMessage       JSONErrorMessage);


    public delegate Task OnBinaryMessageRequestReceivedDelegate (DateTime                    Timestamp,
                                                                 IOCPPWebSocketAdapterIN     Server,
                                                                 OCPP_BinaryRequestMessage   BinaryRequestMessage);

    public delegate Task OnBinaryMessageResponseReceivedDelegate(DateTime                    Timestamp,
                                                                 IOCPPWebSocketAdapterIN     Server,
                                                                 OCPP_BinaryResponseMessage  BinaryResponseMessage);

    //public delegate Task OnBinaryErrorResponseReceivedDelegate  (DateTime                    Timestamp,
    //                                                             IOCPPWebSocketAdapterIN     Server,
    //                                                             OCPP_BinaryErrorMessage     BinaryErrorMessage);


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

        #region Properties

        HashSet<NetworkingNode_Id> AnycastIds { get; }

        #endregion

        #region Events

        #region Generic Text Messages

        /// <summary>
        /// An event sent whenever a JSON request was received.
        /// </summary>
        event OnJSONMessageRequestReceivedDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a JSON response was received.
        /// </summary>
        event OnJSONMessageResponseReceivedDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever a JSON error response was received.
        /// </summary>
        event OnJSONErrorResponseReceivedDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary request was received.
        /// </summary>
        event OnBinaryMessageRequestReceivedDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a binary response was received.
        /// </summary>
        event OnBinaryMessageResponseReceivedDelegate?    OnBinaryMessageResponseReceived;

        ///// <summary>
        ///// An event sent whenever a binary error response was received.
        ///// </summary>
        //event OnBinaryErrorResponseReceivedDelegate?      OnBinaryErrorResponseReceived;

        #endregion


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



        #region OnDataTransfer (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        event OnDataTransferRequestReceivedDelegate  OnDataTransferRequestReceived;


        event OnDataTransferDelegate?                OnDataTransfer;

        #endregion


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
