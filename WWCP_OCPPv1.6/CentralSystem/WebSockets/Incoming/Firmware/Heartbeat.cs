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

using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    #region OnHeartbeat

    /// <summary>
    /// A heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    public delegate Task

        OnHeartbeatRequestDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   HeartbeatRequest   Request);


    /// <summary>
    /// A heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The heartbeat charging station heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this charging station heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime            Timestamp,
                            IEventSender        Sender,
                            HeartbeatRequest    Request,
                            CancellationToken   CancellationToken);


    /// <summary>
    /// A heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat response.</param>
    /// <param name="Sender">The sender of the charging station heartbeat response.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    /// <param name="Response">The charging station heartbeat response.</param>
    /// <param name="Runtime">The runtime of the charging station heartbeat response.</param>
    public delegate Task

        OnHeartbeatResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    HeartbeatRequest    Request,
                                    HeartbeatResponse   Response,
                                    TimeSpan            Runtime);

    #endregion

}
