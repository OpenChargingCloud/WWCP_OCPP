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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode.CS
{

    #region OnSendFile

    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The SendFile request.</param>
    public delegate Task

        OnSendFileRequestDelegate(DateTime          Timestamp,
                                  IEventSender      Sender,
                                  SendFileRequest   Request);


    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The SendFile request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendFileResponse>

        OnSendFileDelegate(DateTime                    Timestamp,
                           IEventSender                Sender,
                           WebSocketClientConnection   Connection,
                           SendFileRequest             Request,
                           CancellationToken           CancellationToken);


    /// <summary>
    /// A SendFile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The SendFile request.</param>
    /// <param name="Response">The SendFile response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSendFileResponseDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   SendFileRequest    Request,
                                   SendFileResponse   Response,
                                   TimeSpan           Runtime);

    #endregion

}
