/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    #region OnAuthorize

    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="IdToken">An OCPP identification token.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeRequestDelegate(DateTime                       Timestamp,
                                   CSServer                       Sender,
                                   CancellationToken              CancellationToken,
                                   EventTracking_Id               EventTrackingId,

                                   IdToken                        IdToken,

                                   TimeSpan?                      QueryTimeout = null);

    #endregion

    #region OnBootNotification

    /// <summary>
    /// BootNotification the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargePointVendor">The charge point vendor identification.</param>
    /// <param name="ChargePointModel">The charge point model identification.</param>
    /// <param name="ChargePointSerialNumber">The serial number of the charge point.</param>
    /// <param name="FirmwareVersion">The firmware version of the charge point.</param>
    /// <param name="Iccid">The ICCID of the charge point's SIM card.</param>
    /// <param name="IMSI">The IMSI of the charge point’s SIM card.</param>
    /// <param name="MeterType">The meter type of the main power meter of the charge point.</param>
    /// <param name="MeterSerialNumber">The serial number of the main power meter of the charge point.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationRequestDelegate(DateTime                       Timestamp,
                                          CSServer                       Sender,
                                          CancellationToken              CancellationToken,
                                          EventTracking_Id               EventTrackingId,

                                          String                         ChargePointVendor,
                                          String                         ChargePointModel,
                                          String                         ChargePointSerialNumber,
                                          String                         FirmwareVersion,
                                          String                         Iccid,
                                          String                         IMSI,
                                          String                         MeterType,
                                          String                         MeterSerialNumber,

                                          TimeSpan?                      QueryTimeout = null);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charge point heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatRequestDelegate(DateTime             Timestamp,
                                   CSServer             Sender,
                                   CancellationToken    CancellationToken,
                                   EventTracking_Id     EventTrackingId,
                                   TimeSpan?            QueryTimeout = null);

    #endregion


}
