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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all OCPP HTTP Web Socket clients.
    /// </summary>
    public interface IOCPPWebSocketClient : IHTTPClient
    {

        IOCPPAdapter    OCPPAdapter       { get; }
        NetworkingMode  NetworkingMode    { get; }


        event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;
        event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;
        event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;
        event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestReceived;
        event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestSent;
        event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;


        Task ProcessWebSocketTextFrame  (DateTime                   RequestTimestamp,
                                         WebSocketClientConnection  ClientConnection,
                                         EventTracking_Id           EventTrackingId,
                                         String                     TextMessage,
                                         CancellationToken          CancellationToken);
        Task ProcessWebSocketBinaryFrame(DateTime                   RequestTimestamp,
                                         WebSocketClientConnection  ClientConnection,
                                         EventTracking_Id           EventTrackingId,
                                         Byte[]                     BinaryMessage,
                                         CancellationToken          CancellationToken);


        Task<SendMessageResult> SendJSONRequest       (OCPP_JSONRequestMessage        JSONRequestMessage);
        Task<SendMessageResult> SendJSONResponse      (OCPP_JSONResponseMessage       JSONResponseMessage);
        Task<SendMessageResult> SendJSONRequestError  (OCPP_JSONRequestErrorMessage   JSONRequestErrorMessage);
        Task<SendMessageResult> SendJSONResponseError (OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage);


        Task<SendMessageResult> SendBinaryRequest     (OCPP_BinaryRequestMessage      BinaryRequestMessage);
        Task<SendMessageResult> SendBinaryResponse    (OCPP_BinaryResponseMessage     BinaryResponseMessage);


    }

}
