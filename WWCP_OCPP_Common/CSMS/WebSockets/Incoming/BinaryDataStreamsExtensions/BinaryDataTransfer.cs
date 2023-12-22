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

#endregion

namespace cloud.charging.open.protocols.OCPP.CSMS
{

    #region OnIncomingBinaryDataTransfer

    /// <summary>
    /// An incoming BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferRequestDelegate(DateTime                       Timestamp,
                                                    IEventSender                   Sender,
                                                    WebSocketServerConnection      Connection,
                                                    CS.BinaryDataTransferRequest   Request);


    /// <summary>
    /// An incoming binary data transfer from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<OCPP.CSMS.BinaryDataTransferResponse>

        OnIncomingBinaryDataTransferDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             WebSocketServerConnection      Connection,
                                             CS.BinaryDataTransferRequest   Request,
                                             CancellationToken              CancellationToken);


    /// <summary>
    /// An incoming binary data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferResponseDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     WebSocketServerConnection      Connection,
                                                     CS.BinaryDataTransferRequest   Request,
                                                     BinaryDataTransferResponse     Response,
                                                     TimeSpan                       Runtime);

    #endregion

}
