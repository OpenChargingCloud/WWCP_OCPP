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
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

using org.GraphDefined.WWCP.OCPPv1_6.CP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    #region OnReserveNow

    /// <summary>
    /// A delegate called whenever a reserve now request will be send to a charge point.
    /// </summary>
    public delegate Task OnReserveNowRequestDelegate (DateTime              LogTimestamp,
                                                      DateTime              RequestTimestamp,
                                                      CentralSystemSOAPClient              Sender,
                                                      String                SenderId,
                                                      EventTracking_Id      EventTrackingId,

                                                      ChargeBox_Id          ChargeBoxIdentity,
                                                      Connector_Id          ConnectorId,
                                                      Reservation_Id        ReservationId,
                                                      DateTime              ExpiryDate,
                                                      IdToken               IdTag,
                                                      IdToken?              ParentIdTag,

                                                      TimeSpan?             RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a reserve now request was received.
    /// </summary>
    public delegate Task OnReserveNowResponseDelegate(DateTime              LogTimestamp,
                                                      DateTime              RequestTimestamp,
                                                      CentralSystemSOAPClient              Sender,
                                                      String                SenderId,
                                                      EventTracking_Id      EventTrackingId,

                                                      ChargeBox_Id          ChargeBoxIdentity,
                                                      Connector_Id          ConnectorId,
                                                      Reservation_Id        ReservationId,
                                                      DateTime              ExpiryDate,
                                                      IdToken               IdTag,
                                                      IdToken?              ParentIdTag,

                                                      TimeSpan?             RequestTimeout,
                                                      ReserveNowResponse    Result,
                                                      TimeSpan              Duration);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A delegate called whenever a cancel reservation request will be send to a charge point.
    /// </summary>
    public delegate Task OnCancelReservationRequestDelegate (DateTime                     LogTimestamp,
                                                             DateTime                     RequestTimestamp,
                                                             CentralSystemSOAPClient                     Sender,
                                                             String                       SenderId,
                                                             EventTracking_Id             EventTrackingId,

                                                             ChargeBox_Id                 ChargeBoxIdentity,
                                                             Reservation_Id               ReservationId,

                                                             TimeSpan?                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a cancel reservation request was received.
    /// </summary>
    public delegate Task OnCancelReservationResponseDelegate(DateTime                     LogTimestamp,
                                                             DateTime                     RequestTimestamp,
                                                             CentralSystemSOAPClient                     Sender,
                                                             String                       SenderId,
                                                             EventTracking_Id             EventTrackingId,

                                                             ChargeBox_Id                 ChargeBoxIdentity,
                                                             Reservation_Id               ReservationId,

                                                             TimeSpan?                    RequestTimeout,
                                                             CancelReservationResponse    Result,
                                                             TimeSpan                     Duration);

    #endregion

    #region OnRemoteStartTransaction

    /// <summary>
    /// A delegate called whenever a remote start transaction request will be send to a charge point.
    /// </summary>
    public delegate Task OnRemoteStartTransactionRequestDelegate (DateTime                          LogTimestamp,
                                                                  DateTime                          RequestTimestamp,
                                                                  CentralSystemSOAPClient                          Sender,
                                                                  String                            SenderId,
                                                                  EventTracking_Id                  EventTrackingId,

                                                                  ChargeBox_Id                      ChargeBoxIdentity,
                                                                  IdToken                           IdTag,
                                                                  Connector_Id?                     ConnectorId,
                                                                  ChargingProfile                   ChargingProfile,

                                                                  TimeSpan?                         RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a remote start transaction request was received.
    /// </summary>
    public delegate Task OnRemoteStartTransactionResponseDelegate(DateTime                          LogTimestamp,
                                                                  DateTime                          RequestTimestamp,
                                                                  CentralSystemSOAPClient                          Sender,
                                                                  String                            SenderId,
                                                                  EventTracking_Id                  EventTrackingId,

                                                                  ChargeBox_Id                      ChargeBoxIdentity,
                                                                  IdToken                           IdTag,
                                                                  Connector_Id?                     ConnectorId,
                                                                  ChargingProfile                   ChargingProfile,

                                                                  TimeSpan?                         RequestTimeout,
                                                                  RemoteStartTransactionResponse    Result,
                                                                  TimeSpan                          Duration);

    #endregion

    #region OnRemoteStopTransaction

    /// <summary>
    /// A delegate called whenever a remote stop transaction request will be send to a charge point.
    /// </summary>
    public delegate Task OnRemoteStopTransactionRequestDelegate (DateTime                         LogTimestamp,
                                                                 DateTime                         RequestTimestamp,
                                                                 CentralSystemSOAPClient                         Sender,
                                                                 String                           SenderId,
                                                                 EventTracking_Id                 EventTrackingId,

                                                                 ChargeBox_Id                     ChargeBoxIdentity,
                                                                 Transaction_Id                   TransactionId,

                                                                 TimeSpan?                        RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a remote stop transaction request was received.
    /// </summary>
    public delegate Task OnRemoteStopTransactionResponseDelegate(DateTime                         LogTimestamp,
                                                                 DateTime                         RequestTimestamp,
                                                                 CentralSystemSOAPClient                         Sender,
                                                                 String                           SenderId,
                                                                 EventTracking_Id                 EventTrackingId,

                                                                 ChargeBox_Id                     ChargeBoxIdentity,
                                                                 Transaction_Id                   TransactionId,

                                                                 TimeSpan?                        RequestTimeout,
                                                                 RemoteStopTransactionResponse    Result,
                                                                 TimeSpan                         Duration);

    #endregion


    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to a charge point.
    /// </summary>
    public delegate Task OnDataTransferRequestDelegate (DateTime                   LogTimestamp,
                                                        DateTime                   RequestTimestamp,
                                                        CentralSystemSOAPClient                   Sender,
                                                        String                     SenderId,
                                                        EventTracking_Id           EventTrackingId,

                                                        String                     VendorId,
                                                        String                     MessageId,
                                                        String                     Data,

                                                        TimeSpan?                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    public delegate Task OnDataTransferResponseDelegate(DateTime                   LogTimestamp,
                                                        DateTime                   RequestTimestamp,
                                                        CentralSystemSOAPClient                   Sender,
                                                        String                     SenderId,
                                                        EventTracking_Id           EventTrackingId,

                                                        String                     VendorId,
                                                        String                     MessageId,
                                                        String                     Data,

                                                        TimeSpan?                  RequestTimeout,
                                                        CP.DataTransferResponse    Result,
                                                        TimeSpan                   Duration);

    #endregion


}
