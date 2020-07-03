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

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ChargePointVendor">The charge point vendor identification.</param>
    /// <param name="ChargePointModel">The charge point model identification.</param>
    /// <param name="ChargePointSerialNumber">The serial number of the charge point.</param>
    /// <param name="ChargeBoxSerialNumber">The serial number of the charge point.</param>
    /// <param name="FirmwareVersion">The firmware version of the charge point.</param>
    /// <param name="Iccid">The ICCID of the charge point's SIM card.</param>
    /// <param name="IMSI">The IMSI of the charge point’s SIM card.</param>
    /// <param name="MeterType">The meter type of the main power meter of the charge point.</param>
    /// <param name="MeterSerialNumber">The serial number of the main power meter of the charge point.</param>
    public delegate Task

        BootNotificationRequestDelegate(DateTime                    Timestamp,
                                        CentralSystemSOAPServer     Sender,
                                        EventTracking_Id            EventTrackingId,

                                        ChargeBox_Id                ChargeBoxIdentity,

                                        String                      ChargePointVendor,
                                        String                      ChargePointModel,
                                        String                      ChargePointSerialNumber,
                                        String                      ChargeBoxSerialNumber,
                                        String                      FirmwareVersion,
                                        String                      Iccid,
                                        String                      IMSI,
                                        String                      MeterType,
                                        String                      MeterSerialNumber);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="BootNotificationRequest">A boot notification request.</param>
    public delegate Task<BootNotificationResponse>

        BootNotificationDelegate(DateTime                    Timestamp,
                                 CentralSystemSOAPServer     Sender,
                                 CancellationToken           CancellationToken,
                                 EventTracking_Id            EventTrackingId,

                                 ChargeBox_Id                ChargeBoxIdentity,
                                 CP.BootNotificationRequest  BootNotificationRequest);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ChargePointVendor">The charge point vendor identification.</param>
    /// <param name="ChargePointModel">The charge point model identification.</param>
    /// <param name="ChargePointSerialNumber">The serial number of the charge point.</param>
    /// <param name="ChargeBoxSerialNumber">The serial number of the charge point.</param>
    /// <param name="FirmwareVersion">The firmware version of the charge point.</param>
    /// <param name="Iccid">The ICCID of the charge point's SIM card.</param>
    /// <param name="IMSI">The IMSI of the charge point’s SIM card.</param>
    /// <param name="MeterType">The meter type of the main power meter of the charge point.</param>
    /// <param name="MeterSerialNumber">The serial number of the main power meter of the charge point.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Status">The registration status.</param>
    /// <param name="CurrentTime">The current time at the central system.</param>
    /// <param name="Interval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        BootNotificationResponseDelegate(DateTime                 Timestamp,
                                         CentralSystemSOAPServer  Sender,
                                         EventTracking_Id         EventTrackingId,

                                         ChargeBox_Id             ChargeBoxIdentity,

                                         String                   ChargePointVendor,
                                         String                   ChargePointModel,
                                         String                   ChargePointSerialNumber,
                                         String                   ChargeBoxSerialNumber,
                                         String                   FirmwareVersion,
                                         String                   Iccid,
                                         String                   IMSI,
                                         String                   MeterType,
                                         String                   MeterSerialNumber,

                                         Result                   Result,
                                         RegistrationStatus       Status,
                                         DateTime                 CurrentTime,
                                         TimeSpan                 Interval,
                                         TimeSpan                 Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charge point heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    public delegate Task

        HeartbeatRequestDelegate(DateTime                 Timestamp,
                                 CentralSystemSOAPServer  Sender,
                                 EventTracking_Id         EventTrackingId,

                                 ChargeBox_Id             ChargeBoxIdentity);


    /// <summary>
    /// A charge point heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="HeartbeatRequest">A heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        HeartbeatDelegate(DateTime                 Timestamp,
                          CentralSystemSOAPServer  Sender,
                          CancellationToken        CancellationToken,
                          EventTracking_Id         EventTrackingId,

                          ChargeBox_Id             ChargeBoxIdentity,
                          CP.HeartbeatRequest      HeartbeatRequest);


    /// <summary>
    /// A charge point heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="CurrentTime">The current time at the central system.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        HeartbeatResponseDelegate(DateTime                 Timestamp,
                                  CentralSystemSOAPServer  Sender,
                                  EventTracking_Id         EventTrackingId,

                                  ChargeBox_Id             ChargeBoxIdentity,

                                  Result                   Result,
                                  DateTime                 CurrentTime,
                                  TimeSpan                 Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="IdTag">The identifier that needs to be authorized.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime                 Timestamp,
                                   CentralSystemSOAPServer  Sender,
                                   EventTracking_Id         EventTrackingId,

                                   ChargeBox_Id             ChargeBoxIdentity,

                                   IdToken                  IdTag);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="AuthorizeRequest">An authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime                 Timestamp,
                            CentralSystemSOAPServer  Sender,
                            CancellationToken        CancellationToken,
                            EventTracking_Id         EventTrackingId,

                            ChargeBox_Id             ChargeBoxIdentity,
                            CP.AuthorizeRequest      AuthorizeRequest);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="IdTag">The identifier that needs to be authorized.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="IdTagInfo">An identification tag info.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime                 Timestamp,
                                    CentralSystemSOAPServer  Sender,
                                    EventTracking_Id         EventTrackingId,

                                    ChargeBox_Id             ChargeBoxIdentity,

                                    IdToken                  IdTag,

                                    Result                   Result,
                                    IdTagInfo                IdTagInfo,
                                    TimeSpan                 Runtime);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point.</param>
    /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
    /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
    /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
    /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
    public delegate Task

        OnStartTransactionRequestDelegate(DateTime                 Timestamp,
                                          CentralSystemSOAPServer  Sender,
                                          EventTracking_Id         EventTrackingId,

                                          ChargeBox_Id             ChargeBoxIdentity,

                                          Connector_Id             ConnectorId,
                                          IdToken                  IdTag,
                                          DateTime                 StartTimestamp,
                                          UInt64                   MeterStart,
                                          Reservation_Id?          ReservationId);


    /// <summary>
    /// A start transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StartTransactionRequest">A start transaction request.</param>
    public delegate Task<StartTransactionResponse>

        OnStartTransactionDelegate(DateTime                    Timestamp,
                                   CentralSystemSOAPServer     Sender,
                                   CancellationToken           CancellationToken,
                                   EventTracking_Id            EventTrackingId,

                                   ChargeBox_Id                ChargeBoxIdentity,
                                   CP.StartTransactionRequest  StartTransactionRequest);


    /// <summary>
    /// A start transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point.</param>
    /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
    /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
    /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
    /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="TransactionId">The transaction identification assigned by the central system.</param>
    /// <param name="IdTagInfo">An identification tag info.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStartTransactionResponseDelegate(DateTime                 Timestamp,
                                           CentralSystemSOAPServer  Sender,
                                           EventTracking_Id         EventTrackingId,

                                           ChargeBox_Id             ChargeBoxIdentity,

                                           Connector_Id             ConnectorId,
                                           IdToken                  IdTag,
                                           DateTime                 StartTimestamp,
                                           UInt64                   MeterStart,
                                           Reservation_Id?          ReservationId,

                                           Result                   Result,
                                           Transaction_Id           TransactionId,
                                           IdTagInfo                IdTagInfo,
                                           TimeSpan                 Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point. Id '0' (zero) is used if the status is for the charge point main controller.</param>
    /// <param name="Status">The current status of the charge point.</param>
    /// <param name="ErrorCode">The error code reported by the charge point.</param>
    /// <param name="Info">Additional free format information related to the error.</param>
    /// <param name="StatusTimestamp">The time for which the status is reported.</param>
    /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
    /// <param name="VendorErrorCode">A vendor-specific error code.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                 Timestamp,
                                            CentralSystemSOAPServer  Sender,
                                            EventTracking_Id         EventTrackingId,

                                            ChargeBox_Id             ChargeBoxIdentity,

                                            Connector_Id             ConnectorId,
                                            ChargePointStatus        Status,
                                            ChargePointErrorCodes    ErrorCode,
                                            String                   Info,
                                            DateTime?                StatusTimestamp,
                                            String                   VendorId,
                                            String                   VendorErrorCode);


    /// <summary>
    /// Send a charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StatusNotificationRequest">A status notification request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     CentralSystemSOAPServer       Sender,
                                     CancellationToken             CancellationToken,
                                     EventTracking_Id              EventTrackingId,

                                     ChargeBox_Id                  ChargeBoxIdentity,
                                     CP.StatusNotificationRequest  StatusNotificationRequest);


    /// <summary>
    /// A charge point status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point. Id '0' (zero) is used if the status is for the charge point main controller.</param>
    /// <param name="Status">The current status of the charge point.</param>
    /// <param name="ErrorCode">The error code reported by the charge point.</param>
    /// <param name="Info">Additional free format information related to the error.</param>
    /// <param name="StatusTimestamp">The time for which the status is reported.</param>
    /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
    /// <param name="VendorErrorCode">A vendor-specific error code.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                 Timestamp,
                                             CentralSystemSOAPServer  Sender,
                                             EventTracking_Id         EventTrackingId,

                                             ChargeBox_Id             ChargeBoxIdentity,

                                             Connector_Id             ConnectorId,
                                             ChargePointStatus        Status,
                                             ChargePointErrorCodes    ErrorCode,
                                             String                   Info,
                                             DateTime?                StatusTimestamp,
                                             String                   VendorId,
                                             String                   VendorErrorCode,

                                             Result                   Result,
                                             TimeSpan                 Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A charge point meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point.</param>
    /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
    /// <param name="MeterValues">The sampled meter values with timestamps.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime                 Timestamp,
                                     CentralSystemSOAPServer  Sender,
                                     EventTracking_Id         EventTrackingId,

                                     ChargeBox_Id             ChargeBoxIdentity,

                                     Connector_Id             ConnectorId,
                                     Transaction_Id?          TransactionId,
                                     IEnumerable<MeterValue>  MeterValues);


    /// <summary>
    /// Send charge point meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="MeterValuesRequest">A meter values request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime                 Timestamp,
                              CentralSystemSOAPServer  Sender,
                              CancellationToken        CancellationToken,
                              EventTracking_Id         EventTrackingId,

                              ChargeBox_Id             ChargeBoxIdentity,
                              CP.MeterValuesRequest    MeterValuesRequest);


    /// <summary>
    /// A charge point meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="ConnectorId">The connector identification at the charge point.</param>
    /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
    /// <param name="MeterValues">The sampled meter values with timestamps.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime                 Timestamp,
                                      CentralSystemSOAPServer  Sender,
                                      EventTracking_Id         EventTrackingId,

                                      ChargeBox_Id             ChargeBoxIdentity,

                                      Connector_Id             ConnectorId,
                                      Transaction_Id?          TransactionId,
                                      IEnumerable<MeterValue>  MeterValues,

                                      Result                   Result,
                                      TimeSpan                 Runtime);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
    /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
    /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
    /// <param name="IdTag">An optional identifier which requested to stop the charging. It is optional because a charge point may terminate charging without the presence of an idTag, e.g. in case of a reset.</param>
    /// <param name="Reason">An optional reason why the transaction had been stopped. MAY only be omitted when the Reason is "Local".</param>
    /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
    public delegate Task

        OnStopTransactionRequestDelegate(DateTime                 Timestamp,
                                         CentralSystemSOAPServer  Sender,
                                         EventTracking_Id         EventTrackingId,

                                         ChargeBox_Id             ChargeBoxIdentity,

                                         Transaction_Id           TransactionId,
                                         DateTime                 StopTimestamp,
                                         UInt64                   MeterStop,
                                         IdToken?                 IdTag,
                                         Reasons?                 Reason,
                                         IEnumerable<MeterValue>  TransactionData);


    /// <summary>
    /// A stop transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StopTransactionRequest">A stop transaction request.</param>
    public delegate Task<StopTransactionResponse>

        OnStopTransactionDelegate(DateTime                   Timestamp,
                                  CentralSystemSOAPServer    Sender,
                                  CancellationToken          CancellationToken,
                                  EventTracking_Id           EventTrackingId,

                                  ChargeBox_Id               ChargeBoxIdentity,
                                  CP.StopTransactionRequest  StopTransactionRequest);


    /// <summary>
    /// A stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
    /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
    /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
    /// <param name="IdTag">An optional identifier which requested to stop the charging. It is optional because a charge point may terminate charging without the presence of an idTag, e.g. in case of a reset.</param>
    /// <param name="Reason">An optional reason why the transaction had been stopped. MAY only be omitted when the Reason is "Local".</param>
    /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStopTransactionResponseDelegate(DateTime                 Timestamp,
                                          CentralSystemSOAPServer  Sender,
                                          EventTracking_Id         EventTrackingId,

                                          ChargeBox_Id             ChargeBoxIdentity,

                                          Result                   Result,
                                          Transaction_Id           TransactionId,
                                          DateTime                 StopTimestamp,
                                          UInt64                   MeterStop,
                                          IdToken?                 IdTag,
                                          Reasons?                 Reason,
                                          IEnumerable<MeterValue>  TransactionData,
                                          TimeSpan                 Runtime);

    #endregion


    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
    /// <param name="MessageId">An optional message identification field.</param>
    /// <param name="MessageData">Optional message data as text without specified length or format.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                 Timestamp,
                                              CentralSystemSOAPServer  Sender,
                                              EventTracking_Id         EventTrackingId,

                                              ChargeBox_Id             ChargeBoxIdentity,

                                              String                   VendorId,
                                              String                   MessageId,
                                              String                   MessageData);


    /// <summary>
    /// An incoming data transfer from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="DataTransferRequest">A data transfer request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                 Timestamp,
                                       CentralSystemSOAPServer  Sender,
                                       CancellationToken        CancellationToken,
                                       EventTracking_Id         EventTrackingId,

                                       ChargeBox_Id             ChargeBoxIdentity,
                                       CP.DataTransferRequest   DataTransferRequest);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
    /// <param name="MessageId">An optional message identification field.</param>
    /// <param name="MessageData">Optional message data as text without specified length or format.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Status">The success or failure status of the data transfer.</param>
    /// <param name="ResponseData">Optional response data.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                 Timestamp,
                                               CentralSystemSOAPServer  Sender,
                                               EventTracking_Id         EventTrackingId,

                                               ChargeBox_Id             ChargeBoxIdentity,

                                               String                   VendorId,
                                               String                   MessageId,
                                               String                   MessageData,

                                               Result                   Result,
                                               DataTransferStatus       Status,
                                               String                   ResponseData,
                                               TimeSpan                 Runtime);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A diagnostics status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Status">The status of the diagnostics upload.</param>
    public delegate Task

        OnDiagnosticsStatusNotificationRequestDelegate(DateTime                 Timestamp,
                                                       CentralSystemSOAPServer  Sender,
                                                       EventTracking_Id         EventTrackingId,

                                                       ChargeBox_Id             ChargeBoxIdentity,

                                                       DiagnosticsStatus        Status);


    /// <summary>
    /// A diagnostics status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request.</param>
    public delegate Task<DiagnosticsStatusNotificationResponse>

        OnDiagnosticsStatusNotificationDelegate(DateTime                                 Timestamp,
                                                CentralSystemSOAPServer                  Sender,
                                                CancellationToken                        CancellationToken,
                                                EventTracking_Id                         EventTrackingId,

                                                ChargeBox_Id                             ChargeBoxIdentity,
                                                CP.DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest);


    /// <summary>
    /// A diagnostics status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Status">The status of the diagnostics upload.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnDiagnosticsStatusNotificationResponseDelegate(DateTime                 Timestamp,
                                                        CentralSystemSOAPServer  Sender,
                                                        EventTracking_Id         EventTrackingId,

                                                        ChargeBox_Id             ChargeBoxIdentity,

                                                        DiagnosticsStatus        Status,

                                                        Result                   Result,
                                                        TimeSpan                 Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware installation status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Status">The current status of a firmware installation.</param>
    public delegate Task

        OnFirmwareStatusNotificationRequestDelegate(DateTime                 Timestamp,
                                                    CentralSystemSOAPServer  Sender,
                                                    EventTracking_Id         EventTrackingId,

                                                    ChargeBox_Id             ChargeBoxIdentity,

                                                    FirmwareStatus           Status);


    /// <summary>
    /// A firmware installation status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             CentralSystemSOAPServer               Sender,
                                             CancellationToken                     CancellationToken,
                                             EventTracking_Id                      EventTrackingId,

                                             ChargeBox_Id                          ChargeBoxIdentity,
                                             CP.FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest);


    /// <summary>
    /// A firmware installation status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// 
    /// <param name="Status">The current status of a firmware installation.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnFirmwareStatusNotificationResponseDelegate(DateTime                 Timestamp,
                                                     CentralSystemSOAPServer  Sender,
                                                     EventTracking_Id         EventTrackingId,

                                                     ChargeBox_Id             ChargeBoxIdentity,

                                                     FirmwareStatus           Status,

                                                     Result                   Result,
                                                     TimeSpan                 Runtime);

    #endregion

}
