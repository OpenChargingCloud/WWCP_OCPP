﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP WWCP <https://github.com/OpenChargingCloud/WWCP_WWCP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// The common interface of all OCPP HTTP WebSocket servers.
    /// </summary>
    public interface IOCPPWebSocketServer : IWWCPWebSocketServer
    {

       // OCPPAdapter  OCPPAdapter    { get; }


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


    }

}
