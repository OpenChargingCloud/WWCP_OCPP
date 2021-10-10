/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    #region OnBootNotification

    /// <summary>
    /// A delegate called whenever a boot notification request will be send to the central system.
    /// </summary>
    public delegate Task OnBootNotificationRequestDelegate (DateTime                    LogTimestamp,
                                                            IEventSender                     Sender,
                                                            BootNotificationRequest     Request);

    /// <summary>
    /// A delegate called whenever a response to a boot notification request was received.
    /// </summary>
    public delegate Task OnBootNotificationResponseDelegate(DateTime                    LogTimestamp,
                                                            IEventSender                     Sender,
                                                            BootNotificationRequest     Request,
                                                            BootNotificationResponse    Response,
                                                            TimeSpan                    Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A delegate called whenever a heartbeat request will be send to the central system.
    /// </summary>
    public delegate Task OnHeartbeatRequestDelegate (DateTime          LogTimestamp,
                                                     IEventSender           Sender,
                                                     HeartbeatRequest  Request);

    /// <summary>
    /// A delegate called whenever a response to a heartbeat request was received.
    /// </summary>
    public delegate Task OnHeartbeatResponseDelegate(DateTime           LogTimestamp,
                                                     IEventSender            Sender,
                                                     HeartbeatRequest   Request,
                                                     HeartbeatResponse  Response,
                                                     TimeSpan           Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// A delegate called whenever an authorize request will be send to the central system.
    /// </summary>
    public delegate Task OnAuthorizeRequestDelegate (DateTime               LogTimestamp,
                                                     DateTime               RequestTimestamp,
                                                     ChargePointSOAPClient  Sender,
                                                     String                 SenderId,
                                                     EventTracking_Id       EventTrackingId,

                                                     IdToken                IdTag,

                                                     TimeSpan?              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    public delegate Task OnAuthorizeResponseDelegate(DateTime               LogTimestamp,
                                                     DateTime               RequestTimestamp,
                                                     ChargePointSOAPClient  Sender,
                                                     String                 SenderId,
                                                     EventTracking_Id       EventTrackingId,

                                                     IdToken                IdTag,

                                                     TimeSpan?              RequestTimeout,
                                                     AuthorizeResponse      Result,
                                                     TimeSpan               Runtime);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A delegate called whenever a start transaction request will be send to the central system.
    /// </summary>
    public delegate Task OnStartTransactionRequestDelegate (DateTime                    LogTimestamp,
                                                            DateTime                    RequestTimestamp,
                                                            ChargePointSOAPClient       Sender,
                                                            String                      SenderId,
                                                            EventTracking_Id            EventTrackingId,

                                                            Connector_Id                ConnectorId,
                                                            IdToken                     IdTag,
                                                            DateTime                    Timestamp,
                                                            UInt64                      MeterStart,
                                                            Reservation_Id?             ReservationId,

                                                            TimeSpan?                   RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a start transaction request was received.
    /// </summary>
    public delegate Task OnStartTransactionResponseDelegate(DateTime                    LogTimestamp,
                                                            DateTime                    RequestTimestamp,
                                                            ChargePointSOAPClient       Sender,
                                                            String                      SenderId,
                                                            EventTracking_Id            EventTrackingId,

                                                            Connector_Id                ConnectorId,
                                                            IdToken                     IdTag,
                                                            DateTime                    Timestamp,
                                                            UInt64                      MeterStart,
                                                            Reservation_Id?             ReservationId,

                                                            TimeSpan?                   RequestTimeout,
                                                            StartTransactionResponse    Result,
                                                            TimeSpan                    Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A delegate called whenever a status notification request will be send to the central system.
    /// </summary>
    public delegate Task OnStatusNotificationRequestDelegate (DateTime                      LogTimestamp,
                                                              DateTime                      RequestTimestamp,
                                                              ChargePointSOAPClient         Sender,
                                                              String                        SenderId,
                                                              EventTracking_Id              EventTrackingId,

                                                              Connector_Id                  ConnectorId,
                                                              ChargePointStatus             Status,
                                                              ChargePointErrorCodes         ErrorCode,
                                                              String                        Info,
                                                              DateTime?                     StatusTimestamp,
                                                              String                        VendorId,
                                                              String                        VendorErrorCode,

                                                              TimeSpan?                     RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a status notification request was received.
    /// </summary>s
    public delegate Task OnStatusNotificationResponseDelegate(DateTime                      LogTimestamp,
                                                              DateTime                      RequestTimestamp,
                                                              ChargePointSOAPClient         Sender,
                                                              String                        SenderId,
                                                              EventTracking_Id              EventTrackingId,

                                                              Connector_Id                  ConnectorId,
                                                              ChargePointStatus             Status,
                                                              ChargePointErrorCodes         ErrorCode,
                                                              String                        Info,
                                                              DateTime?                     StatusTimestamp,
                                                              String                        VendorId,
                                                              String                        VendorErrorCode,

                                                              TimeSpan?                     RequestTimeout,
                                                              StatusNotificationResponse    Result,
                                                              TimeSpan                      Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A delegate called whenever a meter values request will be send to the central system.
    /// </summary>
    public delegate Task OnMeterValuesRequestDelegate (DateTime                 LogTimestamp,
                                                       DateTime                 RequestTimestamp,
                                                       ChargePointSOAPClient    Sender,
                                                       String                   SenderId,
                                                       EventTracking_Id         EventTrackingId,

                                                       Connector_Id             ConnectorId,
                                                       Transaction_Id?          TransactionId,
                                                       IEnumerable<MeterValue>  MeterValues,

                                                       TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a meter values request was received.
    /// </summary>s
    public delegate Task OnMeterValuesResponseDelegate(DateTime                 LogTimestamp,
                                                       DateTime                 RequestTimestamp,
                                                       ChargePointSOAPClient    Sender,
                                                       String                   SenderId,
                                                       EventTracking_Id         EventTrackingId,

                                                       Connector_Id             ConnectorId,
                                                       Transaction_Id?          TransactionId,
                                                       IEnumerable<MeterValue>  MeterValues,

                                                       TimeSpan?                RequestTimeout,
                                                       MeterValuesResponse      Result,
                                                       TimeSpan                 Runtime);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A delegate called whenever a stop transaction request will be send to the central system.
    /// </summary>
    public delegate Task OnStopTransactionRequestDelegate (DateTime                   LogTimestamp,
                                                           DateTime                   RequestTimestamp,
                                                           ChargePointSOAPClient      Sender,
                                                           String                     SenderId,
                                                           EventTracking_Id           EventTrackingId,

                                                           Transaction_Id             TransactionId,
                                                           DateTime                   TransactionTimestamp,
                                                           UInt64                     MeterStop,
                                                           IdToken?                   IdTag,
                                                           Reasons?                   Reason,
                                                           IEnumerable<MeterValue>    TransactionData,

                                                           TimeSpan?                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a stop transaction request was received.
    /// </summary>
    public delegate Task OnStopTransactionResponseDelegate(DateTime                   LogTimestamp,
                                                           DateTime                   RequestTimestamp,
                                                           ChargePointSOAPClient      Sender,
                                                           String                     SenderId,
                                                           EventTracking_Id           EventTrackingId,

                                                           Transaction_Id             TransactionId,
                                                           DateTime                   TransactionTimestamp,
                                                           UInt64                     MeterStop,
                                                           IdToken?                   IdTag,
                                                           Reasons?                   Reason,
                                                           IEnumerable<MeterValue>    TransactionData,

                                                           TimeSpan?                  RequestTimeout,
                                                           StopTransactionResponse    Result,
                                                           TimeSpan                   Runtime);

    #endregion


    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to the central system.
    /// </summary>
    public delegate Task OnDataTransferRequestDelegate (DateTime                LogTimestamp,
                                                        DateTime                RequestTimestamp,
                                                        ChargePointSOAPClient   Sender,
                                                        String                  SenderId,
                                                        EventTracking_Id        EventTrackingId,

                                                        String                  VendorId,
                                                        String                  MessageId,
                                                        String                  Data,

                                                        TimeSpan?               RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    public delegate Task OnDataTransferResponseDelegate(DateTime                LogTimestamp,
                                                        DateTime                RequestTimestamp,
                                                        ChargePointSOAPClient   Sender,
                                                        String                  SenderId,
                                                        EventTracking_Id        EventTrackingId,

                                                        String                  VendorId,
                                                        String                  MessageId,
                                                        String                  Data,

                                                        TimeSpan?               RequestTimeout,
                                                        DataTransferResponse    Result,
                                                        TimeSpan                Runtime);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A delegate called whenever a diagnostics status notification request will be send to the central system.
    /// </summary>
    public delegate Task OnDiagnosticsStatusNotificationRequestDelegate (DateTime                                 LogTimestamp,
                                                                         DateTime                                 RequestTimestamp,
                                                                         ChargePointSOAPClient                    Sender,
                                                                         String                                   SenderId,
                                                                         EventTracking_Id                         EventTrackingId,

                                                                         DiagnosticsStatus                        Status,

                                                                         TimeSpan?                                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a diagnostics status notification request was received.
    /// </summary>
    public delegate Task OnDiagnosticsStatusNotificationResponseDelegate(DateTime                                 LogTimestamp,
                                                                         DateTime                                 RequestTimestamp,
                                                                         ChargePointSOAPClient                    Sender,
                                                                         String                                   SenderId,
                                                                         EventTracking_Id                         EventTrackingId,

                                                                         DiagnosticsStatus                        Status,

                                                                         TimeSpan?                                RequestTimeout,
                                                                         DiagnosticsStatusNotificationResponse    Result,
                                                                         TimeSpan                                 Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a firmware status notification request will be send to the central system.
    /// </summary>
    public delegate Task OnFirmwareStatusNotificationRequestDelegate (DateTime                              LogTimestamp,
                                                                      DateTime                              RequestTimestamp,
                                                                      ChargePointSOAPClient                 Sender,
                                                                      String                                SenderId,
                                                                      EventTracking_Id                      EventTrackingId,

                                                                      FirmwareStatus                        Status,

                                                                      TimeSpan?                             RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a firmware status notification request was received.
    /// </summary>
    public delegate Task OnFirmwareStatusNotificationResponseDelegate(DateTime                              LogTimestamp,
                                                                      DateTime                              RequestTimestamp,
                                                                      ChargePointSOAPClient                 Sender,
                                                                      String                                SenderId,
                                                                      EventTracking_Id                      EventTrackingId,

                                                                      FirmwareStatus                        Status,

                                                                      TimeSpan?                             RequestTimeout,
                                                                      FirmwareStatusNotificationResponse    Result,
                                                                      TimeSpan                              Runtime);

    #endregion


}
