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

using System.Collections.Concurrent;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all OCPP HTTP Web Socket servers.
    /// </summary>
    public interface IOCPPWebSocketServer : IWebSocketServer
    {

        IOCPPAdapter                                      OCPPAdapter              { get; }
        IEnumerable<NetworkingNode_Id>                    NetworkingNodeIds        { get; }
        ConcurrentDictionary<NetworkingNode_Id, String?>  NetworkingNodeLogins     { get; }
        TimeSpan?                                         RequestTimeout           { get; set; }
        Boolean                                           RequireAuthentication    { get; }
        Formatting                                        JSONFormatting           { get; set; }


        //event OnNetworkingNodeCloseMessageReceivedDelegate?    OnNetworkingNodeCloseMessageReceived;
        //event OnNetworkingNodeNewWebSocketConnectionDelegate?  OnNetworkingNodeNewWebSocketConnection;
        //event OnNetworkingNodeTCPConnectionClosedDelegate?     OnNetworkingNodeTCPConnectionClosed;

        event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;
        event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;
        event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;
        event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;
        event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;

        event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestReceived;
        event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestSent;
        event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;



        void    AddOrUpdateHTTPBasicAuth(NetworkingNode_Id NetworkingNodeId, String Password);
        void    AddStaticRouting        (NetworkingNode_Id DestinationNodeId, NetworkingNode_Id NetworkingHubId);
        Boolean RemoveHTTPBasicAuth     (NetworkingNode_Id NetworkingNodeId);
        void    RemoveStaticRouting     (NetworkingNode_Id DestinationNodeId, NetworkingNode_Id NetworkingHubId);


   //     Task<SendOCPPMessageResult> SendJSONData(EventTracking_Id EventTrackingId, NetworkingNode_Id DestinationNodeId, NetworkPath NetworkPath, Request_Id RequestId, string Action, JObject JSONData, DateTime RequestTimeout, CancellationToken CancellationToken = default);
        Task<SendOCPPMessageResult> SendJSONRequest(OCPP_JSONRequestMessage RequestMessage);
 //       Task<SendRequestState>      SendJSONAndWait(EventTracking_Id EventTrackingId, NetworkingNode_Id NetworkingNodeId, NetworkPath NetworkPath, Request_Id RequestId, string OCPPAction, JObject JSONPayload, TimeSpan? RequestTimeout, CancellationToken CancellationToken = default);


   //     Task<SendOCPPMessageResult> SendBinaryData(EventTracking_Id EventTrackingId, NetworkingNode_Id DestinationNodeId, NetworkPath NetworkPath, Request_Id RequestId, string Action, byte[] BinaryData, DateTime RequestTimeout, CancellationToken CancellationToken = default);
        Task<SendOCPPMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage RequestMessage);
   //     Task<SendRequestState>      SendBinaryAndWait(EventTracking_Id EventTrackingId, NetworkingNode_Id NetworkingNodeId, NetworkPath NetworkPath, Request_Id RequestId, string OCPPAction, byte[] BinaryPayload, TimeSpan? RequestTimeout, CancellationToken CancellationToken = default);


    }

}
