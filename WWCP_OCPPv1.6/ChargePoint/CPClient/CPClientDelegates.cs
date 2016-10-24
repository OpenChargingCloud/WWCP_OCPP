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
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    #region OnAuthorize

    /// <summary>
    /// A delegate called whenever an authorize request will be send to the central system.
    /// </summary>
    public delegate Task OnAuthorizeRequestDelegate (DateTime             LogTimestamp,
                                                     DateTime             RequestTimestamp,
                                                     CPClient             Sender,
                                                     String               SenderId,
                                                     EventTracking_Id     EventTrackingId,
                                                     IdToken              IdTag,
                                                     TimeSpan?            RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    public delegate Task OnAuthorizeResponseDelegate(DateTime             LogTimestamp,
                                                     DateTime             RequestTimestamp,
                                                     CPClient             Sender,
                                                     String               SenderId,
                                                     EventTracking_Id     EventTrackingId,
                                                     IdToken              IdTag,
                                                     TimeSpan?            RequestTimeout,
                                                     AuthorizeResponse    Result,
                                                     TimeSpan             Duration);

    #endregion

    #region OnBootNotification

    /// <summary>
    /// A delegate called whenever a boot notification request will be send to the central system.
    /// </summary>
    public delegate Task OnBootNotificationRequestDelegate (DateTime                    LogTimestamp,
                                                            DateTime                    RequestTimestamp,
                                                            CPClient                    Sender,
                                                            String                      SenderId,
                                                            EventTracking_Id            EventTrackingId,
                                                            String                      ChargePointVendor,
                                                            String                      ChargePointModel,
                                                            String                      ChargePointSerialNumber,
                                                            String                      FirmwareVersion,
                                                            String                      Iccid,
                                                            String                      IMSI,
                                                            String                      MeterType,
                                                            String                      MeterSerialNumber,
                                                            TimeSpan?                   RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a boot notification request was received.
    /// </summary>
    public delegate Task OnBootNotificationResponseDelegate(DateTime                    LogTimestamp,
                                                            DateTime                    RequestTimestamp,
                                                            CPClient                    Sender,
                                                            String                      SenderId,
                                                            EventTracking_Id            EventTrackingId,
                                                            String                      ChargePointVendor,
                                                            String                      ChargePointModel,
                                                            String                      ChargePointSerialNumber,
                                                            String                      FirmwareVersion,
                                                            String                      Iccid,
                                                            String                      IMSI,
                                                            String                      MeterType,
                                                            String                      MeterSerialNumber,
                                                            TimeSpan?                   RequestTimeout,
                                                            BootNotificationResponse    Result,
                                                            TimeSpan                    Duration);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A delegate called whenever a heartbeat request will be send to the central system.
    /// </summary>
    public delegate Task OnHeartbeatRequestDelegate (DateTime             LogTimestamp,
                                                     DateTime             RequestTimestamp,
                                                     CPClient             Sender,
                                                     String               SenderId,
                                                     EventTracking_Id     EventTrackingId,
                                                     TimeSpan?            RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a heartbeat request was received.
    /// </summary>
    public delegate Task OnHeartbeatResponseDelegate(DateTime             LogTimestamp,
                                                     DateTime             RequestTimestamp,
                                                     CPClient             Sender,
                                                     String               SenderId,
                                                     EventTracking_Id     EventTrackingId,
                                                     TimeSpan?            RequestTimeout,
                                                     HeartbeatResponse    Result,
                                                     TimeSpan             Duration);

    #endregion


}
