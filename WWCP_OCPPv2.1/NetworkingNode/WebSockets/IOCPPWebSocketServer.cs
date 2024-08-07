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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    public delegate Task OnWebSocketServerJSONMessageDelegate  (DateTime                    Timestamp,
                                                                OCPPWebSocketServer         Server,
                                                                WebSocketServerConnection   Connection,
                                                                EventTracking_Id            EventTrackingId,
                                                                DateTime                    MessageTimestamp,
                                                                JArray                      Message,
                                                                CancellationToken           CancellationToken);

    public delegate Task OnWebSocketServerBinaryMessageDelegate(DateTime                    Timestamp,
                                                                OCPPWebSocketServer         Server,
                                                                WebSocketServerConnection   Connection,
                                                                EventTracking_Id            EventTrackingId,
                                                                DateTime                    MessageTimestamp,
                                                                Byte[]                      MessageMessage,
                                                                CancellationToken           CancellationToken);


    /// <summary>
    /// The common interface of all OCPP HTTP Web Socket servers.
    /// </summary>
    public interface IOCPPWebSocketServer : IWebSocketServer
    {

        OCPPAdapter                                       OCPPAdapter              { get; }
        IEnumerable<NetworkingNode_Id>                    NetworkingNodeIds        { get; }
        ConcurrentDictionary<NetworkingNode_Id, String?>  NetworkingNodeLogins     { get; }
        TimeSpan?                                         RequestTimeout           { get; set; }
        Boolean                                           RequireAuthentication    { get; }
        //Formatting                                        JSONFormatting           { get; set; }


        //event OnNetworkingNodeCloseMessageReceivedDelegate?    OnNetworkingNodeCloseMessageReceived;
        //event OnNetworkingNodeNewWebSocketConnectionDelegate?  OnNetworkingNodeNewWebSocketConnection;
        //event OnNetworkingNodeTCPConnectionClosedDelegate?     OnNetworkingNodeTCPConnectionClosed;


        /// <summary>
        /// An event sent whenever a JSON message was sent.
        /// </summary>
        event     OnWebSocketServerJSONMessageDelegate?     OnJSONMessageSent;

        /// <summary>
        /// An event sent whenever a JSON message was received.
        /// </summary>
        event     OnWebSocketServerJSONMessageDelegate?     OnJSONMessageReceived;


        /// <summary>
        /// An event sent whenever a binary message was sent.
        /// </summary>
        new event OnWebSocketServerBinaryMessageDelegate?   OnBinaryMessageSent;

        /// <summary>
        /// An event sent whenever a binary message was received.
        /// </summary>
        new event OnWebSocketServerBinaryMessageDelegate?   OnBinaryMessageReceived;


        Task<SentMessageResult> SendJSONRequest         (OCPP_JSONRequestMessage          JSONRequestMessage);
        Task<SentMessageResult> SendJSONResponse        (OCPP_JSONResponseMessage         JSONResponseMessage);
        Task<SentMessageResult> SendJSONRequestError    (OCPP_JSONRequestErrorMessage     JSONRequestErrorMessage);
        Task<SentMessageResult> SendJSONResponseError   (OCPP_JSONResponseErrorMessage    JSONResponseErrorMessage);
        Task<SentMessageResult> SendJSONSendMessage     (OCPP_JSONSendMessage             JSONSendMessage);


        Task<SentMessageResult> SendBinaryRequest       (OCPP_BinaryRequestMessage        BinaryRequestMessage);
        Task<SentMessageResult> SendBinaryResponse      (OCPP_BinaryResponseMessage       BinaryResponseMessage);
        Task<SentMessageResult> SendBinaryRequestError  (OCPP_BinaryRequestErrorMessage   BinaryRequestErrorMessage);
        Task<SentMessageResult> SendBinaryResponseError (OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage);
        Task<SentMessageResult> SendBinarySendMessage   (OCPP_BinarySendMessage           BinarySendMessage);



        HTTPBasicAuthentication  AddOrUpdateHTTPBasicAuth (NetworkingNode_Id DestinationId, String            Password);
        void                     AddStaticRouting         (NetworkingNode_Id DestinationId, NetworkingNode_Id NetworkingHubId);
        Boolean                  RemoveHTTPBasicAuth      (NetworkingNode_Id DestinationId);
        void                     RemoveStaticRouting      (NetworkingNode_Id DestinationId, NetworkingNode_Id NetworkingHubId);


    }

}
