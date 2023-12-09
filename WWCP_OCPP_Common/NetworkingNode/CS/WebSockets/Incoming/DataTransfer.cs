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

namespace cloud.charging.open.protocols.OCPP.NetworkingNode.CS
{

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                        Timestamp,
                                              IEventSender                    Sender,
                                              OCPP.CSMS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<OCPP.CS.DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                        Timestamp,
                                       IEventSender                    Sender,
                                       WebSocketClientConnection       Connection,
                                       OCPP.CSMS.DataTransferRequest   Request,
                                       CancellationToken               CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                        Timestamp,
                                               IEventSender                    Sender,
                                               OCPP.CSMS.DataTransferRequest   Request,
                                               OCPP.CS.DataTransferResponse    Response,
                                               TimeSpan                        Runtime);

    #endregion

}
