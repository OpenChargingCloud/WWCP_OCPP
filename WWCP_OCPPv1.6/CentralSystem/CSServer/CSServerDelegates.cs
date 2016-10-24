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

    #region OnBootNotification

    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
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

                                          ChargeBox_Id                   ChargeBoxIdentity,
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
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatRequestDelegate(DateTime             Timestamp,
                                   CSServer             Sender,
                                   CancellationToken    CancellationToken,
                                   EventTracking_Id     EventTrackingId,

                                   ChargeBox_Id         ChargeBoxIdentity,

                                   TimeSpan?            QueryTimeout = null);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="IdToken">An OCPP identification token.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeRequestDelegate(DateTime                       Timestamp,
                                   CSServer                       Sender,
                                   CancellationToken              CancellationToken,
                                   EventTracking_Id               EventTrackingId,

                                   ChargeBox_Id                   ChargeBoxIdentity,
                                   IdToken                        IdToken,

                                   TimeSpan?                      QueryTimeout = null);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A start transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="ConnectorId">The connector identification of the charge point.</param>
    /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
    /// <param name="TransactionTimestamp">The timestamp of the transaction start.</param>
    /// <param name="MeterStart">The meter value in Wh for the connector at start of the transaction.</param>
    /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<StartTransactionResponse>

        OnStartTransactionRequestDelegate(DateTime             Timestamp,
                                          CSServer             Sender,
                                          CancellationToken    CancellationToken,
                                          EventTracking_Id     EventTrackingId,

                                          ChargeBox_Id         ChargeBoxIdentity,
                                          UInt16               ConnectorId,
                                          IdToken              IdTag,
                                          DateTime             TransactionTimestamp,
                                          UInt64               MeterStart,
                                          Int32?               ReservationId,

                                          TimeSpan?            QueryTimeout = null);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="ConnectorId">The connector identification of the charge point.</param>
    /// <param name="Status">The current status of the charge point.</param>
    /// <param name="ErrorCode">The error code reported by the charge point.</param>
    /// <param name="Info">Additional free format information related to the error.</param>
    /// <param name="StatusTimestamp">The time for which the status is reported.</param>
    /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
    /// <param name="VendorErrorCode">A vendor-specific error code.</param>
    /// 
    /// <param name="QueryTimeout">An optional timeout for this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationRequestDelegate(DateTime                 Timestamp,
                                            CSServer                 Sender,
                                            CancellationToken        CancellationToken,
                                            EventTracking_Id         EventTrackingId,

                                            ChargeBox_Id             ChargeBoxIdentity,
                                            UInt16                   ConnectorId,
                                            ChargePointStatus        Status,
                                            ChargePointErrorCodes    ErrorCode,
                                            String                   Info,
                                            DateTime?                StatusTimestamp,
                                            String                   VendorId,
                                            String                   VendorErrorCode,

                                            TimeSpan?                QueryTimeout = null);

    #endregion

}
