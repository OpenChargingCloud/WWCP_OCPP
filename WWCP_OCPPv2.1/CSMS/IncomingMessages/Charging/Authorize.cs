﻿/*
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

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An Authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        OnAuthorizeRequestReceivedDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           AuthorizeRequest       Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime               Timestamp,
                            IEventSender           Sender,
                            IWebSocketConnection   Connection,
                            AuthorizeRequest       Request,
                            CancellationToken      CancellationToken);


    /// <summary>
    /// An Authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize response.</param>
    /// <param name="Sender">The sender of the authorize response.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize response.</param>
    public delegate Task

        OnAuthorizeResponseSentDelegate(DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        AuthorizeRequest       Request,
                                        AuthorizeResponse      Response,
                                        TimeSpan               Runtime);

}
