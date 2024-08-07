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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A delegate called whenever a ReportChargingProfiles request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    public delegate Task OnReportChargingProfilesRequestSentDelegate(DateTime                        Timestamp,
                                                                     IEventSender                    Sender,
                                                                     IWebSocketConnection            Connection,
                                                                     ReportChargingProfilesRequest   Request,
                                                                     SentMessageResults              SendMessageResult);

    /// <summary>
    /// A delegate called whenever a response to a ReportChargingProfiles request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request/response.</param>
    public delegate Task OnReportChargingProfilesResponseReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          ReportChargingProfilesRequest    Request,
                                                                          ReportChargingProfilesResponse   Response,
                                                                          TimeSpan                         Runtime);

}
