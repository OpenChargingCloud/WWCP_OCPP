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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// Extention methods for all central system clients
    /// </summary>
    public static class ICentralSystemClientExtensions
    {

        #region Reset                 (ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ResetResponse> Reset(this ICentralSystemClient  ICentralSystemClient,
                                                ChargeBox_Id               ChargeBoxId,
                                                ResetTypes                 ResetType,

                                                Request_Id?                RequestId           = null,
                                                DateTime?                  RequestTimestamp    = null,
                                                TimeSpan?                  RequestTimeout      = null,
                                                EventTracking_Id?          EventTrackingId     = null,
                                                CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.Reset(
                   new ResetRequest(
                       ChargeBoxId,
                       ResetType,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ChangeAvailability    (ChargeBoxId, ConnectorId, Availability, ...)

        /// <summary>
        /// Change the availability of the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ChangeAvailabilityResponse> ChangeAvailability(this ICentralSystemClient  ICentralSystemClient,
                                                                          ChargeBox_Id               ChargeBoxId,
                                                                          Connector_Id               ConnectorId,
                                                                          Availabilities             Availability,

                                                                          Request_Id?                RequestId           = null,
                                                                          DateTime?                  RequestTimestamp    = null,
                                                                          TimeSpan?                  RequestTimeout      = null,
                                                                          EventTracking_Id?          EventTrackingId     = null,
                                                                          CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.ChangeAvailability(
                   new ChangeAvailabilityRequest(
                       ChargeBoxId,
                       ConnectorId,
                       Availability,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetConfiguration      (ChargeBoxId, Keys = null, ...)

        /// <summary>
        /// Get the configuration of the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetConfigurationResponse> GetConfiguration(this ICentralSystemClient  ICentralSystemClient,
                                                                      ChargeBox_Id               ChargeBoxId,
                                                                      IEnumerable<String>?       Keys                = null,

                                                                      Request_Id?                RequestId           = null,
                                                                      DateTime?                  RequestTimestamp    = null,
                                                                      TimeSpan?                  RequestTimeout      = null,
                                                                      EventTracking_Id?          EventTrackingId     = null,
                                                                      CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.GetConfiguration(
                   new GetConfigurationRequest(
                       ChargeBoxId,
                       Keys,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ChangeConfiguration   (ChargeBoxId, Key, Value, ...)

        /// <summary>
        /// Change the configuration of the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ChangeConfigurationResponse> ChangeConfiguration(this ICentralSystemClient  ICentralSystemClient,
                                                                            ChargeBox_Id               ChargeBoxId,
                                                                            String                     Key,
                                                                            String                     Value,

                                                                            Request_Id?                RequestId           = null,
                                                                            DateTime?                  RequestTimestamp    = null,
                                                                            TimeSpan?                  RequestTimeout      = null,
                                                                            EventTracking_Id?          EventTrackingId     = null,
                                                                            CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.ChangeConfiguration(
                   new ChangeConfigurationRequest(
                       ChargeBoxId,
                       Key,
                       Value,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region DataTransfer          (ChargeBoxId, VendorId, MessageId, Data, ...)

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CP.DataTransferResponse> DataTransfer(this ICentralSystemClient  ICentralSystemClient,
                                                                 ChargeBox_Id               ChargeBoxId,
                                                                 String                     VendorId,
                                                                 String                     MessageId,
                                                                 String                     Data,

                                                                 Request_Id?                RequestId           = null,
                                                                 DateTime?                  RequestTimestamp    = null,
                                                                 TimeSpan?                  RequestTimeout      = null,
                                                                 EventTracking_Id?          EventTrackingId     = null,
                                                                 CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.DataTransfer(
                   new DataTransferRequest(
                       ChargeBoxId,
                       VendorId,
                       MessageId,
                       Data,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetDiagnostics        (ChargeBoxId, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Upload diagnostics data of the given charge box to the given file location.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetDiagnosticsResponse> GetDiagnostics(this ICentralSystemClient  ICentralSystemClient,
                                                                  ChargeBox_Id               ChargeBoxId,
                                                                  String                     Location,
                                                                  DateTime?                  StartTime           = null,
                                                                  DateTime?                  StopTime            = null,
                                                                  Byte?                      Retries             = null,
                                                                  TimeSpan?                  RetryInterval       = null,

                                                                  Request_Id?                RequestId           = null,
                                                                  DateTime?                  RequestTimestamp    = null,
                                                                  TimeSpan?                  RequestTimeout      = null,
                                                                  EventTracking_Id?          EventTrackingId     = null,
                                                                  CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.GetDiagnostics(
                   new GetDiagnosticsRequest(
                       ChargeBoxId,
                       Location,
                       StartTime,
                       StopTime,
                       Retries,
                       RetryInterval,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region TriggerMessage        (ChargeBoxId, RequestedMessage, ConnectorId = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<TriggerMessageResponse> TriggerMessage(this ICentralSystemClient  ICentralSystemClient,
                                                                  ChargeBox_Id               ChargeBoxId,
                                                                  MessageTriggers            RequestedMessage,
                                                                  Connector_Id?              ConnectorId         = null,

                                                                  Request_Id?                RequestId           = null,
                                                                  DateTime?                  RequestTimestamp    = null,
                                                                  TimeSpan?                  RequestTimeout      = null,
                                                                  EventTracking_Id?          EventTrackingId     = null,
                                                                  CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.TriggerMessage(
                   new TriggerMessageRequest(
                       ChargeBoxId,
                       RequestedMessage,
                       ConnectorId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region UpdateFirmware        (ChargeBoxId, FirmwareURL, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="FirmwareURL">The URI where to download the firmware.</param>
        /// <param name="RetrieveTimestamp">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UpdateFirmwareResponse> UpdateFirmware(this ICentralSystemClient  ICentralSystemClient,
                                                                  ChargeBox_Id               ChargeBoxId,
                                                                  URL                        FirmwareURL,
                                                                  DateTime                   RetrieveTimestamp,
                                                                  Byte?                      Retries             = null,
                                                                  TimeSpan?                  RetryInterval       = null,

                                                                  Request_Id?                RequestId           = null,
                                                                  DateTime?                  RequestTimestamp    = null,
                                                                  TimeSpan?                  RequestTimeout      = null,
                                                                  EventTracking_Id?          EventTrackingId     = null,
                                                                  CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.UpdateFirmware(
                   new UpdateFirmwareRequest(
                       ChargeBoxId,
                       FirmwareURL,
                       RetrieveTimestamp,
                       Retries,
                       RetryInterval,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region ReserveNow            (ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ReserveNowResponse> ReserveNow(this ICentralSystemClient  ICentralSystemClient,
                                                          ChargeBox_Id               ChargeBoxId,
                                                          Connector_Id               ConnectorId,
                                                          Reservation_Id             ReservationId,
                                                          DateTime                   ExpiryDate,
                                                          IdToken                    IdTag,
                                                          IdToken?                   ParentIdTag         = null,

                                                          Request_Id?                RequestId           = null,
                                                          DateTime?                  RequestTimestamp    = null,
                                                          TimeSpan?                  RequestTimeout      = null,
                                                          EventTracking_Id?          EventTrackingId     = null,
                                                          CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.ReserveNow(
                   new ReserveNowRequest(
                       ChargeBoxId,
                       ConnectorId,
                       ReservationId,
                       ExpiryDate,
                       IdTag,
                       ParentIdTag,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region CancelReservation     (ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CancelReservationResponse> CancelReservation(this ICentralSystemClient  ICentralSystemClient,
                                                                        ChargeBox_Id               ChargeBoxId,
                                                                        Reservation_Id             ReservationId,

                                                                        Request_Id?                RequestId           = null,
                                                                        DateTime?                  RequestTimestamp    = null,
                                                                        TimeSpan?                  RequestTimeout      = null,
                                                                        EventTracking_Id?          EventTrackingId     = null,
                                                                        CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.CancelReservation(
                   new CancelReservationRequest(
                       ChargeBoxId,
                       ReservationId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region RemoteStartTransaction(ChargeBoxId, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Start a charging session at the given charge box connector using the given charging profile.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RemoteStartTransactionResponse> RemoteStartTransaction(this ICentralSystemClient  ICentralSystemClient,
                                                                                  ChargeBox_Id               ChargeBoxId,
                                                                                  IdToken                    IdTag,
                                                                                  Connector_Id?              ConnectorId         = null,
                                                                                  ChargingProfile?           ChargingProfile     = null,

                                                                                  Request_Id?                RequestId           = null,
                                                                                  DateTime?                  RequestTimestamp    = null,
                                                                                  TimeSpan?                  RequestTimeout      = null,
                                                                                  EventTracking_Id?          EventTrackingId     = null,
                                                                                  CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.RemoteStartTransaction(
                   new RemoteStartTransactionRequest(
                       ChargeBoxId,
                       IdTag,
                       ConnectorId,
                       ChargingProfile,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region RemoteStopTransaction (ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Stop a charging session at the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RemoteStopTransactionResponse> RemoteStopTransaction(this ICentralSystemClient  ICentralSystemClient,
                                                                                ChargeBox_Id               ChargeBoxId,
                                                                                Transaction_Id             TransactionId,

                                                                                Request_Id?                RequestId           = null,
                                                                                DateTime?                  RequestTimestamp    = null,
                                                                                TimeSpan?                  RequestTimeout      = null,
                                                                                EventTracking_Id?          EventTrackingId     = null,
                                                                                CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.RemoteStopTransaction(
                   new RemoteStopTransactionRequest(
                       ChargeBoxId,
                       TransactionId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetChargingProfile    (ChargeBoxId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetChargingProfileResponse> SetChargingProfile(this ICentralSystemClient  ICentralSystemClient,
                                                                          ChargeBox_Id               ChargeBoxId,
                                                                          Connector_Id               ConnectorId,
                                                                          ChargingProfile            ChargingProfile,

                                                                          Request_Id?                RequestId           = null,
                                                                          DateTime?                  RequestTimestamp    = null,
                                                                          TimeSpan?                  RequestTimeout      = null,
                                                                          EventTracking_Id?          EventTrackingId     = null,
                                                                          CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.SetChargingProfile(
                   new SetChargingProfileRequest(
                       ChargeBoxId,
                       ConnectorId,
                       ChargingProfile,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearChargingProfile  (ChargeBoxId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearChargingProfileResponse> ClearChargingProfile(this ICentralSystemClient  ICentralSystemClient,
                                                                              ChargeBox_Id               ChargeBoxId,
                                                                              ChargingProfile_Id?        ChargingProfileId        = null,
                                                                              Connector_Id?              ConnectorId              = null,
                                                                              ChargingProfilePurposes?   ChargingProfilePurpose   = null,
                                                                              UInt32?                    StackLevel               = null,

                                                                              Request_Id?                RequestId                = null,
                                                                              DateTime?                  RequestTimestamp         = null,
                                                                              TimeSpan?                  RequestTimeout           = null,
                                                                              EventTracking_Id?          EventTrackingId          = null,
                                                                              CancellationToken          CancellationToken        = default)

            => ICentralSystemClient.ClearChargingProfile(
                   new ClearChargingProfileRequest(
                       ChargeBoxId,
                       ChargingProfileId,
                       ConnectorId,
                       ChargingProfilePurpose,
                       StackLevel,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetCompositeSchedule  (ChargeBoxId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetCompositeScheduleResponse> GetCompositeSchedule(this ICentralSystemClient  ICentralSystemClient,
                                                                              ChargeBox_Id               ChargeBoxId,
                                                                              Connector_Id               ConnectorId,
                                                                              TimeSpan                   Duration,
                                                                              ChargingRateUnits?         ChargingRateUnit    = null,

                                                                              Request_Id?                RequestId           = null,
                                                                              DateTime?                  RequestTimestamp    = null,
                                                                              TimeSpan?                  RequestTimeout      = null,
                                                                              EventTracking_Id?          EventTrackingId     = null,
                                                                              CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.GetCompositeSchedule(
                   new GetCompositeScheduleRequest(
                       ChargeBoxId,
                       ConnectorId,
                       Duration,
                       ChargingRateUnit,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region UnlockConnector       (ChargeBoxId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UnlockConnectorResponse> UnlockConnector(this ICentralSystemClient  ICentralSystemClient,
                                                                    ChargeBox_Id               ChargeBoxId,
                                                                    Connector_Id               ConnectorId,

                                                                    Request_Id?                RequestId           = null,
                                                                    DateTime?                  RequestTimestamp    = null,
                                                                    TimeSpan?                  RequestTimeout      = null,
                                                                    EventTracking_Id?          EventTrackingId     = null,
                                                                    CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.UnlockConnector(
                   new UnlockConnectorRequest(
                       ChargeBoxId,
                       ConnectorId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region GetLocalListVersion   (ChargeBoxId, ...)

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetLocalListVersionResponse> GetLocalListVersion(this ICentralSystemClient  ICentralSystemClient,
                                                                            ChargeBox_Id               ChargeBoxId,

                                                                            Request_Id?                RequestId           = null,
                                                                            DateTime?                  RequestTimestamp    = null,
                                                                            TimeSpan?                  RequestTimeout      = null,
                                                                            EventTracking_Id?          EventTrackingId     = null,
                                                                            CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.GetLocalListVersion(
                   new GetLocalListVersionRequest(
                       ChargeBoxId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SendLocalList         (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SendLocalListResponse> SendLocalList(this ICentralSystemClient        ICentralSystemClient,
                                                                ChargeBox_Id                     ChargeBoxId,
                                                                UInt64                           ListVersion,
                                                                UpdateTypes                      UpdateType,
                                                                IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                                                                Request_Id?                      RequestId                = null,
                                                                DateTime?                        RequestTimestamp         = null,
                                                                TimeSpan?                        RequestTimeout           = null,
                                                                EventTracking_Id?                EventTrackingId          = null,
                                                                CancellationToken                CancellationToken        = default)

            => ICentralSystemClient.SendLocalList(
                   new SendLocalListRequest(
                       ChargeBoxId,
                       ListVersion,
                       UpdateType,
                       LocalAuthorizationList,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearCache            (ChargeBoxId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="ICentralSystemClient">A central system.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearCacheResponse> ClearCache(this ICentralSystemClient  ICentralSystemClient,
                                                          ChargeBox_Id               ChargeBoxId,

                                                          Request_Id?                RequestId           = null,
                                                          DateTime?                  RequestTimestamp    = null,
                                                          TimeSpan?                  RequestTimeout      = null,
                                                          EventTracking_Id?          EventTrackingId     = null,
                                                          CancellationToken          CancellationToken   = default)

            => ICentralSystemClient.ClearCache(
                   new ClearCacheRequest(
                       ChargeBoxId,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

    }


    /// <summary>
    /// The common interface of all central system clients.
    /// </summary>
    public interface ICentralSystemClient
    {

        ChargeBox_Id                            ChargeBoxIdentity    { get; }
        String                                  From                 { get; }
        String                                  To                   { get; }

        CentralSystemSOAPClient.CSClientLogger  Logger               { get; }


        #region Events


        event OnCancelReservationRequestDelegate? OnCancelReservationRequest;
        event OnCancelReservationResponseDelegate? OnCancelReservationResponse;

        event OnChangeAvailabilityRequestDelegate? OnChangeAvailabilityRequest;
        event OnChangeAvailabilityResponseDelegate? OnChangeAvailabilityResponse;

        event OnChangeConfigurationRequestDelegate? OnChangeConfigurationRequest;
        event OnChangeConfigurationResponseDelegate? OnChangeConfigurationResponse;

        event OnClearCacheRequestDelegate? OnClearCacheRequest;
        event OnClearCacheResponseDelegate? OnClearCacheResponse;

        event OnClearChargingProfileRequestDelegate? OnClearChargingProfileRequest;
        event OnClearChargingProfileResponseDelegate? OnClearChargingProfileResponse;

        event OnDataTransferRequestDelegate? OnDataTransferRequest;
        event OnDataTransferResponseDelegate? OnDataTransferResponse;

        event OnGetCompositeScheduleRequestDelegate? OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleResponseDelegate? OnGetCompositeScheduleResponse;

        event OnGetConfigurationRequestDelegate? OnGetConfigurationRequest;
        event OnGetConfigurationResponseDelegate? OnGetConfigurationResponse;

        event OnGetDiagnosticsRequestDelegate? OnGetDiagnosticsRequest;
        event OnGetDiagnosticsResponseDelegate? OnGetDiagnosticsResponse;

        event OnGetLocalListVersionRequestDelegate? OnGetLocalListVersionRequest;
        event OnGetLocalListVersionResponseDelegate? OnGetLocalListVersionResponse;

        event OnRemoteStartTransactionRequestDelegate? OnRemoteStartTransactionRequest;
        event OnRemoteStartTransactionResponseDelegate? OnRemoteStartTransactionResponse;

        event OnRemoteStopTransactionRequestDelegate? OnRemoteStopTransactionRequest;
        event OnRemoteStopTransactionResponseDelegate? OnRemoteStopTransactionResponse;

        event OnReserveNowRequestDelegate? OnReserveNowRequest;
        event OnReserveNowResponseDelegate? OnReserveNowResponse;

        event OnResetRequestDelegate? OnResetRequest;
        event OnResetResponseDelegate? OnResetResponse;

        event OnSendLocalListRequestDelegate? OnSendLocalListRequest;
        event OnSendLocalListResponseDelegate? OnSendLocalListResponse;

        event OnSetChargingProfileRequestDelegate? OnSetChargingProfileRequest;
        event OnSetChargingProfileResponseDelegate? OnSetChargingProfileResponse;

        event OnTriggerMessageRequestDelegate? OnTriggerMessageRequest;
        event OnTriggerMessageResponseDelegate? OnTriggerMessageResponse;

        event OnUnlockConnectorRequestDelegate? OnUnlockConnectorRequest;
        event OnUnlockConnectorResponseDelegate? OnUnlockConnectorResponse;

        event OnUpdateFirmwareRequestDelegate? OnUpdateFirmwareRequest;
        event OnUpdateFirmwareResponseDelegate? OnUpdateFirmwareResponse;



        #endregion


        #region Reset

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// Change the availability of the given charge box connector.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request);

        #endregion

        #region GetConfiguration

        /// <summary>
        /// Get the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A get configuration request.</param>
        Task<GetConfigurationResponse> GetConfiguration(GetConfigurationRequest Request);

        #endregion

        #region ChangeConfiguration

        /// <summary>
        /// Change the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A change configuration request.</param>
        Task<ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request);

        #endregion

        #region DataTransfer

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest Request);

        #endregion

        #region GetDiagnostics

        /// <summary>
        /// Upload diagnostics data of the given charge box to the given file location.
        /// </summary>
        /// <param name="Request">A get diagnostics request.</param>
        Task<GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest Request);

        #endregion

        #region TriggerMessage

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request);

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request);

        #endregion


        #region ReserveNow

        /// <summary>
        /// Create a charging reservation at the given charge box connector.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request);

        #endregion

        #region CancelReservation

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request);

        #endregion

        #region RemoteStartTransaction

        /// <summary>
        /// Start a charging session at the given charge box connector using the given charging profile.
        /// </summary>
        /// <param name="Request">A remote start transaction request.</param>
        Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request);

        #endregion

        #region RemoteStopTransaction

        /// <summary>
        /// Stop a charging session at the given charge box.
        /// </summary>
        /// <param name="Request">A remote stop transaction request.</param>
        Task<RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest Request);

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request);

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request);

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request);

        #endregion

        #region UnlockConnector

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request);

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request);

        #endregion

        #region SendLocalList

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request);

        #endregion

        #region ClearCache

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request);

        #endregion



        // Security extensions

        #region CertificateSigned

        /// <summary>
        /// Send the signed certificate to the charge point.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request);

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// Delete the given certificate on the charge point.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request);

        #endregion

        #region ExtendedTriggerMessage

        /// <summary>
        /// Send an extended trigger message to the charge point.
        /// </summary>
        /// <param name="Request">A extended trigger message request.</param>
        Task<ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request);

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// Retrieve a list of all installed certificates within the charge point.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request);

        #endregion

        #region GetLog

        /// <summary>
        /// Retrieve log files from the charge point.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        Task<GetLogResponse> GetLog(GetLogRequest Request);

        #endregion

        #region InstallCertificate

        /// <summary>
        /// Install the given certificate within the charge point.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request);

        #endregion

        #region SignedUpdateFirmware

        /// <summary>
        /// Update the firmware of the charge point.
        /// </summary>
        /// <param name="Request">A signed update firmware request.</param>
        Task<SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request);

        #endregion


    }

}
