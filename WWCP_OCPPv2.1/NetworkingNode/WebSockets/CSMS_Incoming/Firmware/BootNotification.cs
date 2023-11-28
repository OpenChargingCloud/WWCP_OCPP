﻿/*
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The boot notification request.</param>
    public delegate Task

        OnBootNotificationRequestDelegate(DateTime                    Timestamp,
                                          IEventSender                Sender,
                                          WebSocketServerConnection   Connection,
                                          BootNotificationRequest     Request);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this boot notification request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   WebSocketServerConnection   Connection,
                                   BootNotificationRequest     Request,
                                   CancellationToken           CancellationToken);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification response.</param>
    /// <param name="Sender">The sender of the boot notification response.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="Response">The boot notification response.</param>
    /// <param name="Runtime">The runtime of the boot notification response.</param>
    public delegate Task

        OnBootNotificationResponseDelegate(DateTime                    Timestamp,
                                           IEventSender                Sender,
                                           WebSocketServerConnection   Connection,
                                           BootNotificationRequest     Request,
                                           BootNotificationResponse    Response,
                                           TimeSpan                    Runtime);

    #endregion



}
