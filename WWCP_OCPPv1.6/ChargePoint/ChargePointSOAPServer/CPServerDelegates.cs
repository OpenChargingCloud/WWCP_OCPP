/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    #region OnReserveNow

    /// <summary>
    /// Reserve a charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
    /// <param name="ReservationId">The unique identification of this reservation.</param>
    /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
    /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
    /// <param name="ParentIdTag">An optional ParentIdTag.</param>
    /// 
    /// <param name="RequestTimeout">An optional timeout for this request.</param>
    public delegate Task<ReserveNowResponse>

        OnReserveNowDelegate(DateTime               Timestamp,
                             ChargePointSOAPServer  Sender,
                             CancellationToken      CancellationToken,
                             EventTracking_Id       EventTrackingId,

                             ChargeBox_Id           ChargeBoxIdentity,
                             Connector_Id           ConnectorId,
                             Reservation_Id         ReservationId,
                             DateTime               ExpiryDate,
                             IdToken                IdTag,
                             IdToken?               ParentIdTag,

                             TimeSpan?              RequestTimeout = null);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// Cancel a charge point reservation.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="ReservationId">The unique identification of this reservation.</param>
    /// 
    /// <param name="RequestTimeout">An optional timeout for this request.</param>
    public delegate Task<CancelReservationResponse>

        OnCancelReservationDelegate(DateTime               Timestamp,
                                    ChargePointSOAPServer  Sender,
                                    CancellationToken      CancellationToken,
                                    EventTracking_Id       EventTrackingId,

                                    ChargeBox_Id           ChargeBoxIdentity,
                                    Reservation_Id         ReservationId,

                                    TimeSpan?              RequestTimeout = null);

    #endregion

    #region OnRemoteStartTransaction

    /// <summary>
    /// Start a charging transaction.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="IdTag">The identification tag to start the charging transaction.</param>
    /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
    /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
    /// 
    /// <param name="RequestTimeout">An optional timeout for this request.</param>
    public delegate Task<RemoteStartTransactionResponse>

        OnRemoteStartTransactionDelegate(DateTime               Timestamp,
                                         ChargePointSOAPServer  Sender,
                                         CancellationToken      CancellationToken,
                                         EventTracking_Id       EventTrackingId,

                                         ChargeBox_Id           ChargeBoxIdentity,
                                         IdToken                IdTag,
                                         Connector_Id?          ConnectorId,
                                         ChargingProfile        ChargingProfile,

                                         TimeSpan?              RequestTimeout = null);

    #endregion

    #region OnRemoteStopTransaction

    /// <summary>
    /// Stop a charging transaction.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
    /// 
    /// <param name="RequestTimeout">An optional timeout for this request.</param>
    public delegate Task<RemoteStopTransactionResponse>

        OnRemoteStopTransactionDelegate(DateTime               Timestamp,
                                        ChargePointSOAPServer  Sender,
                                        CancellationToken      CancellationToken,
                                        EventTracking_Id       EventTrackingId,

                                        ChargeBox_Id           ChargeBoxIdentity,
                                        Transaction_Id         TransactionId,

                                        TimeSpan?              RequestTimeout = null);

    #endregion


    #region OnDataTransfer

    /// <summary>
    /// A data transfer from the central system.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
    /// <param name="MessageId">The charge point model identification.</param>
    /// <param name="Data">The serial number of the charge point.</param>
    /// 
    /// <param name="RequestTimeout">An optional timeout for this request.</param>
    public delegate Task<DataTransferResponse>

        OnDataTransferDelegate(DateTime               Timestamp,
                               ChargePointSOAPServer  Sender,
                               CancellationToken      CancellationToken,
                               EventTracking_Id       EventTrackingId,

                               ChargeBox_Id           ChargeBoxIdentity,
                               String                 VendorId,
                               String                 MessageId,
                               String                 Data,

                               TimeSpan?              RequestTimeout = null);

    #endregion


}
