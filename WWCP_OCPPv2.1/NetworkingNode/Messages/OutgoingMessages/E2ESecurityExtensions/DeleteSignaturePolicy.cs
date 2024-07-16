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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A delegate called whenever a DeleteSignaturePolicy request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the DeleteSignaturePolicy request.</param>
    /// <param name="Sender">The sender of the DeleteSignaturePolicy request.</param>
    /// <param name="Connection">The HTTP Web Socket server connection.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDeleteSignaturePolicyRequestSentDelegate(DateTime                       Timestamp,
                                                                    IEventSender                   Sender,
                                                                    //IWebSocketConnection   Connection,
                                                                    DeleteSignaturePolicyRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a DeleteSignaturePolicy request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the DeleteSignaturePolicy request.</param>
    /// <param name="Sender">The sender of the DeleteSignaturePolicy request.</param>
    /// <param name="Connection">The HTTP Web Socket server connection.</param>
    /// <param name="Request">The DeleteSignaturePolicy request.</param>
    /// <param name="Response">The DeleteSignaturePolicy response.</param>
    /// <param name="Runtime">The runtime of the DeleteSignaturePolicy request.</param>
    public delegate Task OnDeleteSignaturePolicyResponseReceivedDelegate(DateTime                        Timestamp,
                                                                         IEventSender                    Sender,
                                                                         //IWebSocketConnection    Connection,
                                                                         DeleteSignaturePolicyRequest    Request,
                                                                         DeleteSignaturePolicyResponse   Response,
                                                                         TimeSpan                        Runtime);

}
