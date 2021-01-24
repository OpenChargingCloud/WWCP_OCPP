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

using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// Extention methods for the ICentralSystemClient interface
    /// </summary>
    public static class ICentralSystemClientExtentions
    {

        #region Reset                 (ChargeBoxId, ResetType, ...)

        public static Task<ResetResponse> Reset(this ICentralSystemClient  ICentralSystemClient,
                                                ChargeBox_Id               ChargeBoxId,
                                                ResetTypes                 ResetType,

                                                Request_Id?                RequestId          = null,
                                                DateTime?                  RequestTimestamp   = null,
                                                TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.Reset(new ResetRequest(ChargeBoxId,
                                                           ResetType,
                                                           RequestId,
                                                           RequestTimestamp),
                                          Timeout);

        #endregion

        #region ChangeAvailability    (ChargeBoxId, ConnectorId, Availability, ...)

        public static Task<ChangeAvailabilityResponse> ChangeAvailability(this ICentralSystemClient  ICentralSystemClient,
                                                                          ChargeBox_Id               ChargeBoxId,
                                                                          Connector_Id               ConnectorId,
                                                                          Availabilities             Availability,

                                                                          Request_Id?                RequestId          = null,
                                                                          DateTime?                  RequestTimestamp   = null,
                                                                          TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.ChangeAvailability(new ChangeAvailabilityRequest(ChargeBoxId,
                                                                                     ConnectorId,
                                                                                     Availability,
                                                                                     RequestId,
                                                                                     RequestTimestamp),
                                                       Timeout);

        #endregion

        #region ChangeConfiguration   (ChargeBoxId, Key, Value, ...)

        public static Task<ChangeConfigurationResponse> ChangeConfiguration(this ICentralSystemClient  ICentralSystemClient,
                                                                            ChargeBox_Id               ChargeBoxId,
                                                                            String                     Key,
                                                                            String                     Value,

                                                                            Request_Id?                RequestId          = null,
                                                                            DateTime?                  RequestTimestamp   = null,
                                                                            TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.ChangeConfiguration(new ChangeConfigurationRequest(ChargeBoxId,
                                                                                       Key,
                                                                                       Value,
                                                                                       RequestId,
                                                                                       RequestTimestamp),
                                                        Timeout);

        #endregion

        #region DataTransfer          (ChargeBoxId, VendorId, MessageId, Data, ...)

        public static Task<CP.DataTransferResponse> DataTransfer(this ICentralSystemClient  ICentralSystemClient,
                                                                 ChargeBox_Id               ChargeBoxId,
                                                                 String                     VendorId,
                                                                 String                     MessageId,
                                                                 String                     Data,

                                                                 Request_Id?                RequestId          = null,
                                                                 DateTime?                  RequestTimestamp   = null,
                                                                 TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.DataTransfer(new DataTransferRequest(ChargeBoxId,
                                                                         VendorId,
                                                                         MessageId,
                                                                         Data,
                                                                         RequestId,
                                                                         RequestTimestamp),
                                                 Timeout);

        #endregion

        #region GetDiagnostics        (ChargeBoxId, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null, ...)

        public static Task<GetDiagnosticsResponse> GetDiagnostics(this ICentralSystemClient  ICentralSystemClient,
                                                                  ChargeBox_Id               ChargeBoxId,
                                                                  String                     Location,
                                                                  DateTime?                  StartTime          = null,
                                                                  DateTime?                  StopTime           = null,
                                                                  Byte?                      Retries            = null,
                                                                  TimeSpan?                  RetryInterval      = null,

                                                                  Request_Id?                RequestId          = null,
                                                                  DateTime?                  RequestTimestamp   = null,
                                                                  TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.GetDiagnostics(new GetDiagnosticsRequest(ChargeBoxId,
                                                                             Location,
                                                                             StartTime,
                                                                             StopTime,
                                                                             Retries,
                                                                             RetryInterval,
                                                                             RequestId,
                                                                             RequestTimestamp),
                                                   Timeout);

        #endregion

        #region TriggerMessage        (ChargeBoxId, RequestedMessage, ConnectorId = null, ...)

        public static Task<CP.TriggerMessageResponse> TriggerMessage(this ICentralSystemClient  ICentralSystemClient,
                                                                     ChargeBox_Id               ChargeBoxId,
                                                                     MessageTriggers            RequestedMessage,
                                                                     Connector_Id?              ConnectorId        = null,

                                                                     Request_Id?                RequestId          = null,
                                                                     DateTime?                  RequestTimestamp   = null,
                                                                     TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.TriggerMessage(new TriggerMessageRequest(ChargeBoxId,
                                                                             RequestedMessage,
                                                                             ConnectorId,
                                                                             RequestId,
                                                                             RequestTimestamp),
                                                   Timeout);

        #endregion

        #region UpdateFirmware        (ChargeBoxId, Location, RetrieveDate, Retries = null, RetryInterval = null, ...)

        public static Task<CP.UpdateFirmwareResponse> UpdateFirmware(this ICentralSystemClient  ICentralSystemClient,
                                                                     ChargeBox_Id               ChargeBoxId,
                                                                     String                     Location,
                                                                     DateTime                   RetrieveDate,
                                                                     Byte?                      Retries            = null,
                                                                     TimeSpan?                  RetryInterval      = null,

                                                                     Request_Id?                RequestId          = null,
                                                                     DateTime?                  RequestTimestamp   = null,
                                                                     TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.UpdateFirmware(new UpdateFirmwareRequest(ChargeBoxId,
                                                                             Location,
                                                                             RetrieveDate,
                                                                             Retries,
                                                                             RetryInterval,
                                                                             RequestId,
                                                                             RequestTimestamp),
                                                   Timeout);

        #endregion


        #region ReserveNow            (ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        public static Task<ReserveNowResponse> ReserveNow(this ICentralSystemClient  ICentralSystemClient,
                                                          ChargeBox_Id               ChargeBoxId,
                                                          Connector_Id               ConnectorId,
                                                          Reservation_Id             ReservationId,
                                                          DateTime                   ExpiryDate,
                                                          IdToken                    IdTag,
                                                          IdToken?                   ParentIdTag        = null,

                                                          Request_Id?                RequestId          = null,
                                                          DateTime?                  RequestTimestamp   = null,
                                                          TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.ReserveNow(new ReserveNowRequest(ChargeBoxId,
                                                                     ConnectorId,
                                                                     ReservationId,
                                                                     ExpiryDate,
                                                                     IdTag,
                                                                     ParentIdTag,
                                                                     RequestId,
                                                                     RequestTimestamp),
                                               Timeout);

        #endregion

        #region CancelReservation     (ChargeBoxId, ReservationId, ...)

        public static Task<CancelReservationResponse> CancelReservation(this ICentralSystemClient  ICentralSystemClient,
                                                                        ChargeBox_Id               ChargeBoxId,
                                                                        Reservation_Id             ReservationId,

                                                                        Request_Id?                RequestId          = null,
                                                                        DateTime?                  RequestTimestamp   = null,
                                                                        TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.CancelReservation(new CancelReservationRequest(ChargeBoxId,
                                                                                   ReservationId,
                                                                                   RequestId,
                                                                                   RequestTimestamp),
                                                      Timeout);

        #endregion

        #region RemoteStartTransaction(ChargeBoxId, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        public static Task<RemoteStartTransactionResponse> RemoteStartTransaction(this ICentralSystemClient  ICentralSystemClient,
                                                                                  ChargeBox_Id               ChargeBoxId,
                                                                                  IdToken                    IdTag,
                                                                                  Connector_Id?              ConnectorId        = null,
                                                                                  ChargingProfile            ChargingProfile    = null,

                                                                                  Request_Id?                RequestId          = null,
                                                                                  DateTime?                  RequestTimestamp   = null,
                                                                                  TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.RemoteStartTransaction(new RemoteStartTransactionRequest(ChargeBoxId,
                                                                                             IdTag,
                                                                                             ConnectorId,
                                                                                             ChargingProfile,
                                                                                             RequestId,
                                                                                             RequestTimestamp),
                                                           Timeout);

        #endregion

        #region RemoteStopTransaction (ChargeBoxId, TransactionId, ...)

        public static Task<RemoteStopTransactionResponse> RemoteStopTransaction(this ICentralSystemClient  ICentralSystemClient,
                                                                                ChargeBox_Id               ChargeBoxId,
                                                                                Transaction_Id             TransactionId,

                                                                                Request_Id?                RequestId          = null,
                                                                                DateTime?                  RequestTimestamp   = null,
                                                                                TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.RemoteStopTransaction(new RemoteStopTransactionRequest(ChargeBoxId,
                                                                                           TransactionId,
                                                                                           RequestId,
                                                                                           RequestTimestamp),
                                                          Timeout);

        #endregion

        #region SetChargingProfile    (ChargeBoxId, ConnectorId, ChargingProfile, ...)

        public static Task<SetChargingProfileResponse> SetChargingProfile(this ICentralSystemClient  ICentralSystemClient,
                                                                          ChargeBox_Id               ChargeBoxId,
                                                                          Connector_Id               ConnectorId,
                                                                          ChargingProfile            ChargingProfile,

                                                                          Request_Id?                RequestId          = null,
                                                                          DateTime?                  RequestTimestamp   = null,
                                                                          TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.SetChargingProfile(new SetChargingProfileRequest(ChargeBoxId,
                                                                                     ConnectorId,
                                                                                     ChargingProfile,
                                                                                     RequestId,
                                                                                     RequestTimestamp),
                                                       Timeout);

        #endregion

        #region ClearChargingProfile  (ChargeBoxId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        public static Task<ClearChargingProfileResponse> ClearChargingProfile(this ICentralSystemClient  ICentralSystemClient,
                                                                              ChargeBox_Id               ChargeBoxId,
                                                                              ChargingProfile_Id?        ChargingProfileId        = null,
                                                                              Connector_Id?              ConnectorId              = null,
                                                                              ChargingProfilePurposes?   ChargingProfilePurpose   = null,
                                                                              UInt32?                    StackLevel               = null,

                                                                              Request_Id?                RequestId                = null,
                                                                              DateTime?                  RequestTimestamp         = null,
                                                                              TimeSpan?                  Timeout                  = null)

            => ICentralSystemClient.ClearChargingProfile(new ClearChargingProfileRequest(ChargeBoxId,
                                                                                         ChargingProfileId,
                                                                                         ConnectorId,
                                                                                         ChargingProfilePurpose,
                                                                                         StackLevel,
                                                                                         RequestId,
                                                                                         RequestTimestamp),
                                                         Timeout);

        #endregion

        #region GetCompositeSchedule  (ChargeBoxId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        public static Task<GetCompositeScheduleResponse> GetCompositeSchedule(this ICentralSystemClient  ICentralSystemClient,
                                                                              ChargeBox_Id               ChargeBoxId,
                                                                              Connector_Id               ConnectorId,
                                                                              TimeSpan                   Duration,
                                                                              ChargingRateUnits?         ChargingRateUnit   = null,

                                                                              Request_Id?                RequestId          = null,
                                                                              DateTime?                  RequestTimestamp   = null,
                                                                              TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.GetCompositeSchedule(new GetCompositeScheduleRequest(ChargeBoxId,
                                                                                         ConnectorId,
                                                                                         Duration,
                                                                                         ChargingRateUnit,
                                                                                         RequestId,
                                                                                         RequestTimestamp),
                                                         Timeout);

        #endregion

        #region UnlockConnector       (ChargeBoxId, ConnectorId, ...)

        public static Task<UnlockConnectorResponse> UnlockConnector(this ICentralSystemClient  ICentralSystemClient,
                                                                    ChargeBox_Id               ChargeBoxId,
                                                                    Connector_Id               ConnectorId,

                                                                    Request_Id?                RequestId          = null,
                                                                    DateTime?                  RequestTimestamp   = null,
                                                                    TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.UnlockConnector(new UnlockConnectorRequest(ChargeBoxId,
                                                                                 ConnectorId,
                                                                                 RequestId,
                                                                                 RequestTimestamp),
                                                      Timeout);

        #endregion


        #region GetLocalListVersion   (ChargeBoxId, ...)

        public static Task<GetLocalListVersionResponse> GetLocalListVersion(this ICentralSystemClient  ICentralSystemClient,
                                                                            ChargeBox_Id               ChargeBoxId,
                                                                            Request_Id?                RequestId          = null,
                                                                            DateTime?                  RequestTimestamp   = null,
                                                                            TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.GetLocalListVersion(new GetLocalListVersionRequest(ChargeBoxId,
                                                                                       RequestId,
                                                                                       RequestTimestamp),
                                                        Timeout);

        #endregion

        #region SendLocalList         (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        public static Task<SendLocalListResponse> SendLocalList(this ICentralSystemClient       ICentralSystemClient,
                                                                ChargeBox_Id                    ChargeBoxId,
                                                                UInt64                          ListVersion,
                                                                UpdateTypes                     UpdateType,
                                                                IEnumerable<AuthorizationData>  LocalAuthorizationList   = null,

                                                                Request_Id?                     RequestId                = null,
                                                                DateTime?                       RequestTimestamp         = null,
                                                                TimeSpan?                       Timeout                  = null)

            => ICentralSystemClient.SendLocalList(new SendLocalListRequest(ChargeBoxId,
                                                                           ListVersion,
                                                                           UpdateType,
                                                                           LocalAuthorizationList,
                                                                           RequestId,
                                                                           RequestTimestamp),
                                                  Timeout);

        #endregion

        #region ClearCache            (ChargeBoxId, ...)

        public static Task<ClearCacheResponse> ClearCache(this ICentralSystemClient  ICentralSystemClient,
                                                          ChargeBox_Id               ChargeBoxId,
                                                          Request_Id?                RequestId          = null,
                                                          DateTime?                  RequestTimestamp   = null,
                                                          TimeSpan?                  Timeout            = null)

            => ICentralSystemClient.ClearCache(new ClearCacheRequest(ChargeBoxId,
                                                                     RequestId,
                                                                     RequestTimestamp),
                                               Timeout);

        #endregion

    }


    /// <summary>
    /// The common interface of a central system.
    /// </summary>
    public interface ICentralSystem : ICentralSystemClient, ICentralSystemServer
    { }


    /// <summary>
    /// The common interface of a central system server.
    /// </summary>
    public interface ICentralSystemServer
    {

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event BootNotificationRequestDelegate OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        event BootNotificationDelegate OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        event BootNotificationResponseDelegate OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        event HeartbeatRequestDelegate OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        event HeartbeatDelegate OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        event HeartbeatResponseDelegate OnHeartbeatResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeRequestDelegate OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeDelegate OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        event OnAuthorizeResponseDelegate OnAuthorizeResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        event OnStartTransactionRequestDelegate OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        event OnStartTransactionDelegate OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        event OnStartTransactionResponseDelegate OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationRequestDelegate OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationDelegate OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        event OnStatusNotificationResponseDelegate OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesRequestDelegate OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesDelegate OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        event OnMeterValuesResponseDelegate OnMeterValuesResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        event OnStopTransactionRequestDelegate OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        event OnStopTransactionDelegate OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        event OnStopTransactionResponseDelegate OnStopTransactionResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferDelegate OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate OnIncomingDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationRequestDelegate OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationDelegate OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        event OnDiagnosticsStatusNotificationResponseDelegate OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationDelegate OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion

    }

    /// <summary>
    /// The common interface of a central system client.
    /// </summary>
    public interface ICentralSystemClient
    {

        #region Reset

        Task<ResetResponse> Reset(ResetRequest  Request,
                                  TimeSpan?     RequestTimeout = null);

        #endregion

        #region ChangeAvailability

        Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest  Request,
                                                            TimeSpan?                  RequestTimeout = null);

        #endregion

        #region GetConfiguration

        Task<GetConfigurationResponse> GetConfiguration(GetConfigurationRequest  Request,
                                                        TimeSpan?                RequestTimeout = null);

        #endregion

        #region ChangeConfiguration

        Task<ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest  Request,
                                                              TimeSpan?                   RequestTimeout = null);

        #endregion

        #region DataTransfer

        Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest  Request,
                                                   TimeSpan?            RequestTimeout = null);

        #endregion

        #region GetDiagnostics

        Task<GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest  Request,
                                                    TimeSpan?              RequestTimeout = null);

        #endregion

        #region TriggerMessage

        Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest  Request,
                                                    TimeSpan?              RequestTimeout = null);

        #endregion

        #region UpdateFirmware

        Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest  Request,
                                                    TimeSpan?              RequestTimeout = null);

        #endregion


        #region ReserveNow

        Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request,
                                            TimeSpan?          RequestTimeout = null);

        #endregion

        #region CancelReservation

        Task<CancelReservationResponse> CancelReservation(CancelReservationRequest  Request,
                                                          TimeSpan?                 RequestTimeout = null);

        #endregion

        #region RemoteStartTransaction

        Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest  Request,
                                                                    TimeSpan?                      RequestTimeout = null);

        #endregion

        #region RemoteStopTransaction

        Task<RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest  Request,
                                                                  TimeSpan?                     RequestTimeout = null);

        #endregion

        #region SetChargingProfile

        Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest  Request,
                                                            TimeSpan?                  RequestTimeout = null);

        #endregion

        #region ClearChargingProfile

        Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest  Request,
                                                                TimeSpan?                    RequestTimeout = null);

        #endregion

        #region GetCompositeSchedule

        Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest  Request,
                                                                TimeSpan?                    RequestTimeout = null);

        #endregion

        #region UnlockConnector

        Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest  Request,
                                                      TimeSpan?               RequestTimeout = null);

        #endregion


        #region GetLocalListVersion

        Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest  Request,
                                                              TimeSpan?                   RequestTimeout = null);

        #endregion

        #region SendLocalList

        Task<SendLocalListResponse> SendLocalList(SendLocalListRequest  Request,
                                                  TimeSpan?             RequestTimeout = null);

        #endregion

        #region ClearCache

        Task<ClearCacheResponse> ClearCache(ClearCacheRequest  Request,
                                            TimeSpan?          RequestTimeout = null);

        #endregion

    }

}
