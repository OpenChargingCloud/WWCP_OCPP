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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnMeterValues

    /// <summary>
    /// A meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     MeterValuesRequest   Request);


    /// <summary>
    /// Send meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime             Timestamp,
                              IEventSender         Sender,
                              MeterValuesRequest   Request,
                              CancellationToken    CancellationToken);


    /// <summary>
    /// A meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="Response">The meter values response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      MeterValuesRequest    Request,
                                      MeterValuesResponse   Response,
                                      TimeSpan              Runtime);

    #endregion


}
