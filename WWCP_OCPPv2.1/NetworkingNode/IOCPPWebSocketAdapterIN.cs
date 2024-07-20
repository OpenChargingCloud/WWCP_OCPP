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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageReceivedDelegate         (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_JSONRequestMessage          JSONRequestMessage);

    public delegate Task OnJSONResponseMessageReceivedDelegate        (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_JSONResponseMessage         JSONResponseMessage);

    public delegate Task OnJSONRequestErrorMessageReceivedDelegate    (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage);

    public delegate Task OnJSONResponseErrorMessageReceivedDelegate   (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage);

    public delegate Task OnJSONSendMessageReceivedDelegate            (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_JSONSendMessage             JSONSendMessage);



    public delegate Task OnBinaryRequestMessageReceivedDelegate       (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_BinaryRequestMessage        BinaryRequestMessage);

    public delegate Task OnBinaryResponseMessageReceivedDelegate      (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_BinaryResponseMessage       BinaryResponseMessage);

    public delegate Task OnBinaryRequestErrorMessageReceivedDelegate  (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage);

    public delegate Task OnBinaryResponseErrorMessageReceivedDelegate (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage);

    public delegate Task OnBinarySendMessageReceivedDelegate          (DateTime                         Timestamp,
                                                                       IOCPPWebSocketAdapterIN          Server,
                                                                       OCPP_BinarySendMessage           BinarySendMessage);

    #endregion


    /// <summary>
    /// The common interface of all incoming OCPP messages processors.
    /// </summary>
    public interface IOCPPWebSocketAdapterIN : CS.  INetworkingNodeIncomingMessages,
                                               CS.  INetworkingNodeIncomingMessageEvents,

                                               CSMS.INetworkingNodeIncomingMessages,
                                               CSMS.INetworkingNodeIncomingMessageEvents,

                                               INNIncomingMessages_BinaryDataStreamsExtensions,
                                               INNIncomingMessagesEvents_BinaryDataStreamsExtensions,

                                               INNIncomingMessages_E2ESecurityExtensions,
                                               INNIncomingMessagesEvents_E2ESecurityExtensions,

                                               INNIncomingMessages_OverlayNetworkExtensions,
                                               INNIncomingMessagesEvents_OverlayNetworkExtensions

    {

        #region Properties

        HashSet<NetworkingNode_Id> AnycastIds { get; }

        #endregion

        #region Events

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

        #region JSON   messages received

        /// <summary>
        /// An event sent whenever a JSON request was received.
        /// </summary>
        event OnJSONRequestMessageReceivedDelegate?           OnJSONRequestMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON response was received.
        /// </summary>
        event OnJSONResponseMessageReceivedDelegate?          OnJSONResponseMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON request error was received.
        /// </summary>
        event OnJSONRequestErrorMessageReceivedDelegate?      OnJSONRequestErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON response error was received.
        /// </summary>
        event OnJSONResponseErrorMessageReceivedDelegate?     OnJSONResponseErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a JSON send message was received.
        /// </summary>
        event OnJSONSendMessageReceivedDelegate?              OnJSONSendMessageReceived;

        #endregion

        #region Binary messages received

        /// <summary>
        /// An event sent whenever a binary request was received.
        /// </summary>
        event OnBinaryRequestMessageReceivedDelegate?         OnBinaryRequestMessageReceived;

        /// <summary>
        /// An event sent whenever a binary response was received.
        /// </summary>
        event OnBinaryResponseMessageReceivedDelegate?        OnBinaryResponseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a binary request error was received.
        ///// </summary>
        //event OnBinaryRequestErrorMessageReceivedDelegate?    OnBinaryRequestErrorMessageReceived;

        ///// <summary>
        ///// An event sent whenever a binary response error was received.
        ///// </summary>
        //event OnBinaryResponseErrorMessageReceivedDelegate?   OnBinaryResponseErrorMessageReceived;

        /// <summary>
        /// An event sent whenever a binary send message was received.
        /// </summary>
        event OnBinarySendMessageReceivedDelegate?            OnBinarySendMessageReceived;

        #endregion

        #endregion


        Task<WebSocketTextMessageResponse>   ProcessJSONMessage   (DateTime              RequestTimestamp,
                                                                   IWebSocketConnection  WebSocketConnection,
                                                                   JArray                JSONMessage,
                                                                   EventTracking_Id      EventTrackingId,
                                                                   CancellationToken     CancellationToken);

        Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage (DateTime              RequestTimestamp,
                                                                   IWebSocketConnection  WebSocketConnection,
                                                                   Byte[]                BinaryMessage,
                                                                   EventTracking_Id      EventTrackingId,
                                                                   CancellationToken     CancellationToken);


    }

}
