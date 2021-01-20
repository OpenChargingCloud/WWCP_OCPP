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

using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    #region OnCancelReservation

    /// <summary>
    /// A delegate called whenever a cancel reservation request will be send to a charge point.
    /// </summary>
    public delegate Task OnCancelReservationRequestDelegate (DateTime                     LogTimestamp,
                                                             DateTime                     RequestTimestamp,
                                                             CentralSystemSOAPClient      Sender,
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
                                                             CentralSystemSOAPClient      Sender,
                                                             String                       SenderId,
                                                             EventTracking_Id             EventTrackingId,

                                                             ChargeBox_Id                 ChargeBoxIdentity,
                                                             Reservation_Id               ReservationId,

                                                             TimeSpan?                    RequestTimeout,
                                                             CancelReservationResponse    Result,
                                                             TimeSpan                     Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A delegate called whenever a change availability request will be send to a charge point.
    /// </summary>
    public delegate Task OnChangeAvailabilityRequestDelegate (DateTime                 LogTimestamp,
                                                              DateTime                 RequestTimestamp,
                                                              CentralSystemSOAPClient  Sender,
                                                              String                   SenderId,
                                                              EventTracking_Id         EventTrackingId,

                                                              ChargeBox_Id             ChargeBoxIdentity,
                                                              Connector_Id             ConnectorId,
                                                              AvailabilityTypes        Type,

                                                              TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a change availability request was received.
    /// </summary>
    public delegate Task OnChangeAvailabilityResponseDelegate(DateTime                    LogTimestamp,
                                                              DateTime                    RequestTimestamp,
                                                              CentralSystemSOAPClient     Sender,
                                                              String                      SenderId,
                                                              EventTracking_Id            EventTrackingId,

                                                              ChargeBox_Id                ChargeBoxIdentity,
                                                              Connector_Id                ConnectorId,
                                                              AvailabilityTypes           Type,

                                                              TimeSpan?                   RequestTimeout,
                                                              ChangeAvailabilityResponse  Result,
                                                              TimeSpan                    Runtime);

    #endregion

    #region OnChangeConfiguration

    /// <summary>
    /// A delegate called whenever a change configuration request will be send to a charge point.
    /// </summary>
    public delegate Task OnChangeConfigurationRequestDelegate (DateTime                 LogTimestamp,
                                                               DateTime                 RequestTimestamp,
                                                               CentralSystemSOAPClient  Sender,
                                                               String                   SenderId,
                                                               EventTracking_Id         EventTrackingId,

                                                               ChargeBox_Id             ChargeBoxIdentity,
                                                               String                   Key,
                                                               String                   Value,

                                                               TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a change configuration request was received.
    /// </summary>
    public delegate Task OnChangeConfigurationResponseDelegate(DateTime                     LogTimestamp,
                                                               DateTime                     RequestTimestamp,
                                                               CentralSystemSOAPClient      Sender,
                                                               String                       SenderId,
                                                               EventTracking_Id             EventTrackingId,

                                                               ChargeBox_Id                 ChargeBoxIdentity,
                                                               String                       Key,
                                                               String                       Value,

                                                               TimeSpan?                    RequestTimeout,
                                                               ChangeConfigurationResponse  Result,
                                                               TimeSpan                     Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A delegate called whenever a clear cache request will be send to a charge point.
    /// </summary>
    public delegate Task OnClearCacheRequestDelegate (DateTime                 LogTimestamp,
                                                      DateTime                 RequestTimestamp,
                                                      CentralSystemSOAPClient  Sender,
                                                      String                   SenderId,
                                                      EventTracking_Id         EventTrackingId,

                                                      ChargeBox_Id             ChargeBoxIdentity,

                                                      TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a clear cache request was received.
    /// </summary>
    public delegate Task OnClearCacheResponseDelegate(DateTime                 LogTimestamp,
                                                      DateTime                 RequestTimestamp,
                                                      CentralSystemSOAPClient  Sender,
                                                      String                   SenderId,
                                                      EventTracking_Id         EventTrackingId,

                                                      ChargeBox_Id             ChargeBoxIdentity,

                                                      TimeSpan?                RequestTimeout,
                                                      ClearCacheResponse       Result,
                                                      TimeSpan                 Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A delegate called whenever a clear charging profile request will be send to a charge point.
    /// </summary>
    public delegate Task OnClearChargingProfileRequestDelegate (DateTime                  LogTimestamp,
                                                                DateTime                  RequestTimestamp,
                                                                CentralSystemSOAPClient   Sender,
                                                                String                    SenderId,
                                                                EventTracking_Id          EventTrackingId,

                                                                ChargeBox_Id              ChargeBoxIdentity,
                                                                ChargingProfile_Id?       ChargingProfileId,
                                                                Connector_Id?             ConnectorId,
                                                                ChargingProfilePurposes?  ChargingProfilePurpose,
                                                                UInt32?                   StackLevel,

                                                                TimeSpan?                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a clear charging profile request was received.
    /// </summary>
    public delegate Task OnClearChargingProfileResponseDelegate(DateTime                      LogTimestamp,
                                                                DateTime                      RequestTimestamp,
                                                                CentralSystemSOAPClient       Sender,
                                                                String                        SenderId,
                                                                EventTracking_Id              EventTrackingId,

                                                                ChargeBox_Id                  ChargeBoxIdentity,
                                                                ChargingProfile_Id?           ChargingProfileId,
                                                                Connector_Id?                 ConnectorId,
                                                                ChargingProfilePurposes?      ChargingProfilePurpose,
                                                                UInt32?                       StackLevel,

                                                                TimeSpan?                     RequestTimeout,
                                                                ClearChargingProfileResponse  Result,
                                                                TimeSpan                      Runtime);

    #endregion

    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to a charge point.
    /// </summary>
    public delegate Task OnDataTransferRequestDelegate (DateTime                   LogTimestamp,
                                                        DateTime                   RequestTimestamp,
                                                        CentralSystemSOAPClient    Sender,
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
                                                        CentralSystemSOAPClient    Sender,
                                                        String                     SenderId,
                                                        EventTracking_Id           EventTrackingId,

                                                        String                     VendorId,
                                                        String                     MessageId,
                                                        String                     Data,

                                                        TimeSpan?                  RequestTimeout,
                                                        CP.DataTransferResponse    Result,
                                                        TimeSpan                   Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A delegate called whenever a get composite schedule request will be send to a charge point.
    /// </summary>
    public delegate Task OnGetCompositeScheduleRequestDelegate (DateTime                 LogTimestamp,
                                                                DateTime                 RequestTimestamp,
                                                                CentralSystemSOAPClient  Sender,
                                                                String                   SenderId,
                                                                EventTracking_Id         EventTrackingId,

                                                                ChargeBox_Id             ChargeBoxIdentity,
                                                                Connector_Id             ConnectorId,
                                                                TimeSpan                 Duration,
                                                                ChargingRateUnits?       ChargingRateUnit,

                                                                TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get composite schedule request was received.
    /// </summary>
    public delegate Task OnGetCompositeScheduleResponseDelegate(DateTime                      LogTimestamp,
                                                                DateTime                      RequestTimestamp,
                                                                CentralSystemSOAPClient       Sender,
                                                                String                        SenderId,
                                                                EventTracking_Id              EventTrackingId,

                                                                ChargeBox_Id                  ChargeBoxIdentity,
                                                                Connector_Id                  ConnectorId,
                                                                TimeSpan                      Duration,
                                                                ChargingRateUnits?            ChargingRateUnit,

                                                                TimeSpan?                     RequestTimeout,
                                                                GetCompositeScheduleResponse  Result,
                                                                TimeSpan                      Runtime);

    #endregion

    #region OnGetConfiguration

    /// <summary>
    /// A delegate called whenever a get configuration request will be send to a charge point.
    /// </summary>
    public delegate Task OnGetConfigurationRequestDelegate (DateTime                 LogTimestamp,
                                                            DateTime                 RequestTimestamp,
                                                            CentralSystemSOAPClient  Sender,
                                                            String                   SenderId,
                                                            EventTracking_Id         EventTrackingId,

                                                            ChargeBox_Id             ChargeBoxIdentity,
                                                            IEnumerable<String>      Keys,

                                                            TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get configuration request was received.
    /// </summary>
    public delegate Task OnGetConfigurationResponseDelegate(DateTime                  LogTimestamp,
                                                            DateTime                  RequestTimestamp,
                                                            CentralSystemSOAPClient   Sender,
                                                            String                    SenderId,
                                                            EventTracking_Id          EventTrackingId,

                                                            ChargeBox_Id              ChargeBoxIdentity,
                                                            IEnumerable<String>       Keys,

                                                            TimeSpan?                 RequestTimeout,
                                                            GetConfigurationResponse  Result,
                                                            TimeSpan                  Runtime);

    #endregion

    #region OnGetDiagnostics

    /// <summary>
    /// A delegate called whenever a get diagnostics request will be send to a charge point.
    /// </summary>
    public delegate Task OnGetDiagnosticsRequestDelegate (DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          String                   Location,
                                                          DateTime?                StartTime,
                                                          DateTime?                StopTime,
                                                          Byte?                    Retries,
                                                          TimeSpan?                RetryInterval,

                                                          TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get diagnostics request was received.
    /// </summary>
    public delegate Task OnGetDiagnosticsResponseDelegate(DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          String                   Location,
                                                          DateTime?                StartTime,
                                                          DateTime?                StopTime,
                                                          Byte?                    Retries,
                                                          TimeSpan?                RetryInterval,

                                                          TimeSpan?                RequestTimeout,
                                                          GetDiagnosticsResponse   Result,
                                                          TimeSpan                 Runtime);

    #endregion

    #region OnGetLocalListVersion

    /// <summary>
    /// A delegate called whenever a get local list version request will be send to a charge point.
    /// </summary>
    public delegate Task OnGetLocalListVersionRequestDelegate (DateTime                 LogTimestamp,
                                                               DateTime                 RequestTimestamp,
                                                               CentralSystemSOAPClient  Sender,
                                                               String                   SenderId,
                                                               EventTracking_Id         EventTrackingId,

                                                               ChargeBox_Id             ChargeBoxIdentity,

                                                               TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get local list version request was received.
    /// </summary>
    public delegate Task OnGetLocalListVersionResponseDelegate(DateTime                     LogTimestamp,
                                                               DateTime                     RequestTimestamp,
                                                               CentralSystemSOAPClient      Sender,
                                                               String                       SenderId,
                                                               EventTracking_Id             EventTrackingId,

                                                               ChargeBox_Id                 ChargeBoxIdentity,

                                                               TimeSpan?                    RequestTimeout,
                                                               GetLocalListVersionResponse  Result,
                                                               TimeSpan                     Runtime);

    #endregion

    #region OnRemoteStartTransaction

    /// <summary>
    /// A delegate called whenever a remote start transaction request will be send to a charge point.
    /// </summary>
    public delegate Task OnRemoteStartTransactionRequestDelegate (DateTime                          LogTimestamp,
                                                                  DateTime                          RequestTimestamp,
                                                                  CentralSystemSOAPClient           Sender,
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
                                                                  CentralSystemSOAPClient           Sender,
                                                                  String                            SenderId,
                                                                  EventTracking_Id                  EventTrackingId,

                                                                  ChargeBox_Id                      ChargeBoxIdentity,
                                                                  IdToken                           IdTag,
                                                                  Connector_Id?                     ConnectorId,
                                                                  ChargingProfile                   ChargingProfile,

                                                                  TimeSpan?                         RequestTimeout,
                                                                  RemoteStartTransactionResponse    Result,
                                                                  TimeSpan                          Runtime);

    #endregion

    #region OnRemoteStopTransaction

    /// <summary>
    /// A delegate called whenever a remote stop transaction request will be send to a charge point.
    /// </summary>
    public delegate Task OnRemoteStopTransactionRequestDelegate (DateTime                         LogTimestamp,
                                                                 DateTime                         RequestTimestamp,
                                                                 CentralSystemSOAPClient          Sender,
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
                                                                 CentralSystemSOAPClient          Sender,
                                                                 String                           SenderId,
                                                                 EventTracking_Id                 EventTrackingId,

                                                                 ChargeBox_Id                     ChargeBoxIdentity,
                                                                 Transaction_Id                   TransactionId,

                                                                 TimeSpan?                        RequestTimeout,
                                                                 RemoteStopTransactionResponse    Result,
                                                                 TimeSpan                         Runtime);

    #endregion

    #region OnReserveNow

    /// <summary>
    /// A delegate called whenever a reserve now request will be send to a charge point.
    /// </summary>
    public delegate Task OnReserveNowRequestDelegate (DateTime                 LogTimestamp,
                                                      DateTime                 RequestTimestamp,
                                                      CentralSystemSOAPClient  Sender,
                                                      String                   SenderId,
                                                      EventTracking_Id         EventTrackingId,

                                                      ChargeBox_Id             ChargeBoxIdentity,
                                                      Connector_Id             ConnectorId,
                                                      Reservation_Id           ReservationId,
                                                      DateTime                 ExpiryDate,
                                                      IdToken                  IdTag,
                                                      IdToken?                 ParentIdTag,

                                                      TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a reserve now request was received.
    /// </summary>
    public delegate Task OnReserveNowResponseDelegate(DateTime                 LogTimestamp,
                                                      DateTime                 RequestTimestamp,
                                                      CentralSystemSOAPClient  Sender,
                                                      String                   SenderId,
                                                      EventTracking_Id         EventTrackingId,

                                                      ChargeBox_Id             ChargeBoxIdentity,
                                                      Connector_Id             ConnectorId,
                                                      Reservation_Id           ReservationId,
                                                      DateTime                 ExpiryDate,
                                                      IdToken                  IdTag,
                                                      IdToken?                 ParentIdTag,

                                                      TimeSpan?                RequestTimeout,
                                                      ReserveNowResponse       Result,
                                                      TimeSpan                 Runtime);

    #endregion

    #region OnReset

    /// <summary>
    /// A delegate called whenever a reset request will be send to a charge point.
    /// </summary>
    public delegate Task OnResetRequestDelegate (DateTime                 LogTimestamp,
                                                 DateTime                 RequestTimestamp,
                                                 CentralSystemSOAPClient  Sender,
                                                 String                   SenderId,
                                                 EventTracking_Id         EventTrackingId,

                                                 ChargeBox_Id             ChargeBoxIdentity,
                                                 ResetTypes               Type,

                                                 TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a reset request was received.
    /// </summary>
    public delegate Task OnResetResponseDelegate(DateTime                 LogTimestamp,
                                                 DateTime                 RequestTimestamp,
                                                 CentralSystemSOAPClient  Sender,
                                                 String                   SenderId,
                                                 EventTracking_Id         EventTrackingId,

                                                 ChargeBox_Id             ChargeBoxIdentity,
                                                 ResetTypes               Type,

                                                 TimeSpan?                RequestTimeout,
                                                 ResetResponse            Result,
                                                 TimeSpan                 Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A delegate called whenever a send local list request will be send to a charge point.
    /// </summary>
    public delegate Task OnSendLocalListRequestDelegate (DateTime                        LogTimestamp,
                                                         DateTime                        RequestTimestamp,
                                                         CentralSystemSOAPClient         Sender,
                                                         String                          SenderId,
                                                         EventTracking_Id                EventTrackingId,

                                                         ChargeBox_Id                    ChargeBoxIdentity,
                                                         UInt64                          ListVersion,
                                                         UpdateTypes                     UpdateType,
                                                         IEnumerable<AuthorizationData>  LocalAuthorizationList,

                                                         TimeSpan?                       RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a send local list request was received.
    /// </summary>
    public delegate Task OnSendLocalListResponseDelegate(DateTime                        LogTimestamp,
                                                         DateTime                        RequestTimestamp,
                                                         CentralSystemSOAPClient         Sender,
                                                         String                          SenderId,
                                                         EventTracking_Id                EventTrackingId,

                                                         ChargeBox_Id                    ChargeBoxIdentity,
                                                         UInt64                          ListVersion,
                                                         UpdateTypes                     UpdateType,
                                                         IEnumerable<AuthorizationData>  LocalAuthorizationList,

                                                         TimeSpan?                       RequestTimeout,
                                                         SendLocalListResponse           Result,
                                                         TimeSpan                        Runtime);

    #endregion

    #region OnSetChargingProfile

    /// <summary>
    /// A delegate called whenever a set charging profile request will be send to a charge point.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate (DateTime                 LogTimestamp,
                                                              DateTime                 RequestTimestamp,
                                                              CentralSystemSOAPClient  Sender,
                                                              String                   SenderId,
                                                              EventTracking_Id         EventTrackingId,

                                                              ChargeBox_Id             ChargeBoxIdentity,
                                                              Connector_Id             ConnectorId,
                                                              ChargingProfile          ChargingProfile,

                                                              TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a set charging profile request was received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                    LogTimestamp,
                                                              DateTime                    RequestTimestamp,
                                                              CentralSystemSOAPClient     Sender,
                                                              String                      SenderId,
                                                              EventTracking_Id            EventTrackingId,

                                                              ChargeBox_Id                ChargeBoxIdentity,
                                                              Connector_Id                ConnectorId,
                                                              ChargingProfile             ChargingProfile,

                                                              TimeSpan?                   RequestTimeout,
                                                              SetChargingProfileResponse  Result,
                                                              TimeSpan                    Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A delegate called whenever a trigger message request will be send to a charge point.
    /// </summary>
    public delegate Task OnTriggerMessageRequestDelegate (DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          MessageTriggers          RequestedMessage,
                                                          Connector_Id?            ConnectorId,

                                                          TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a trigger message request was received.
    /// </summary>
    public delegate Task OnTriggerMessageResponseDelegate(DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          MessageTriggers          RequestedMessage,
                                                          Connector_Id?            ConnectorId,

                                                          TimeSpan?                RequestTimeout,
                                                          TriggerMessageResponse   Result,
                                                          TimeSpan                 Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// A delegate called whenever a unlock connector request will be send to a charge point.
    /// </summary>
    public delegate Task OnUnlockConnectorRequestDelegate (DateTime                 LogTimestamp,
                                                           DateTime                 RequestTimestamp,
                                                           CentralSystemSOAPClient  Sender,
                                                           String                   SenderId,
                                                           EventTracking_Id         EventTrackingId,

                                                           ChargeBox_Id             ChargeBoxIdentity,
                                                           Connector_Id             ConnectorId,

                                                           TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a unlock connector request was received.
    /// </summary>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTime                 LogTimestamp,
                                                           DateTime                 RequestTimestamp,
                                                           CentralSystemSOAPClient  Sender,
                                                           String                   SenderId,
                                                           EventTracking_Id         EventTrackingId,

                                                           ChargeBox_Id             ChargeBoxIdentity,
                                                           Connector_Id             ConnectorId,

                                                           TimeSpan?                RequestTimeout,
                                                           UnlockConnectorResponse  Result,
                                                           TimeSpan                 Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// A delegate called whenever a update firmware request will be send to a charge point.
    /// </summary>
    public delegate Task OnUpdateFirmwareRequestDelegate (DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          String                   Location,
                                                          DateTime                 RetrieveDate,
                                                          Byte?                    Retries,
                                                          TimeSpan?                RetryInterval,

                                                          TimeSpan?                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a update firmware request was received.
    /// </summary>
    public delegate Task OnUpdateFirmwareResponseDelegate(DateTime                 LogTimestamp,
                                                          DateTime                 RequestTimestamp,
                                                          CentralSystemSOAPClient  Sender,
                                                          String                   SenderId,
                                                          EventTracking_Id         EventTrackingId,

                                                          ChargeBox_Id             ChargeBoxIdentity,
                                                          String                   Location,
                                                          DateTime                 RetrieveDate,
                                                          Byte?                    Retries,
                                                          TimeSpan?                RetryInterval,

                                                          TimeSpan?                RequestTimeout,
                                                          UpdateFirmwareResponse   Result,
                                                          TimeSpan                 Runtime);

    #endregion

}
