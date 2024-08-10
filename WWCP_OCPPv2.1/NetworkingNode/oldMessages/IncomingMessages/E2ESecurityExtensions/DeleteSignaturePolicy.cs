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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A received DeleteSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the DeleteSignaturePolicy request.</param>
    /// <param name="Sender">The sender of the DeleteSignaturePolicy request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The DeleteSignaturePolicy request.</param>
    public delegate Task

        OnDeleteSignaturePolicyRequestReceivedDelegate(DateTime                       Timestamp,
                                                       IEventSender                   Sender,
                                                       IWebSocketConnection           Connection,
                                                       DeleteSignaturePolicyRequest   Request);


    /// <summary>
    /// Process a received DeleteSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the DeleteSignaturePolicy request.</param>
    /// <param name="Sender">The sender of the DeleteSignaturePolicy request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The DeleteSignaturePolicy request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DeleteSignaturePolicyResponse>

        OnDeleteSignaturePolicyDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        IWebSocketConnection           Connection,
                                        DeleteSignaturePolicyRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A sent DeleteSignaturePolicy response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the DeleteSignaturePolicy request.</param>
    /// <param name="Sender">The sender of the DeleteSignaturePolicy request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The DeleteSignaturePolicy request.</param>
    /// <param name="Response">The DeleteSignaturePolicy response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnDeleteSignaturePolicyResponseSentDelegate(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    IWebSocketConnection            Connection,
                                                    DeleteSignaturePolicyRequest    Request,
                                                    DeleteSignaturePolicyResponse   Response,
                                                    TimeSpan                        Runtime);

}
