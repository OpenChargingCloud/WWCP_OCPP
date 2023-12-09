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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A security event notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSecurityEventNotificationRequestDelegate(DateTime                              Timestamp,
                                                   IEventSender                          Sender,
                                                   CP.SecurityEventNotificationRequest   Request);


    /// <summary>
    /// A security event notification at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket server connection.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecurityEventNotificationResponse>

        OnSecurityEventNotificationDelegate(DateTime                              Timestamp,
                                            IEventSender                          Sender,
                                            WebSocketServerConnection             Connection,
                                            CP.SecurityEventNotificationRequest   Request,
                                            CancellationToken                     CancellationToken);


    /// <summary>
    /// A security event notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSecurityEventNotificationResponseDelegate(DateTime                               Timestamp,
                                                    IEventSender                           Sender,
                                                    CP.SecurityEventNotificationRequest    Request,
                                                    CS.SecurityEventNotificationResponse   Response,
                                                    TimeSpan                               Runtime);

}
