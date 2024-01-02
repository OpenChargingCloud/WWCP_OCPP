/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// Extension methods for all central systems.
    /// </summary>
    public static class ICentralSystemExtensions
    {

        #region Reset                  (NetworkingNodeId, ResetType, ...)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ResetResponse> Reset(this ICentralSystem           ICentralSystem,

                                                NetworkingNode_Id             NetworkingNodeId,
                                                ResetTypes                    ResetType,

                                                IEnumerable<KeyPair>?         SignKeys            = null,
                                                IEnumerable<SignInfo>?        SignInfos           = null,
                                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                CustomData?                   CustomData          = null,

                                                Request_Id?                   RequestId           = null,
                                                DateTime?                     RequestTimestamp    = null,
                                                TimeSpan?                     RequestTimeout      = null,
                                                EventTracking_Id?             EventTrackingId     = null,
                                                NetworkPath?                  NetworkPath         = null,
                                                CancellationToken             CancellationToken   = default)

            => ICentralSystem.Reset(
                   new ResetRequest(
                       NetworkingNodeId,
                       ResetType,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region ChangeAvailability     (NetworkingNodeId, ConnectorId, Availability, ...)

        /// <summary>
        /// Change the availability of the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ChangeAvailabilityResponse> ChangeAvailability(this ICentralSystem           ICentralSystem,

                                                                          NetworkingNode_Id             NetworkingNodeId,
                                                                          Connector_Id                  ConnectorId,
                                                                          Availabilities                Availability,

                                                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                          CustomData?                   CustomData          = null,

                                                                          Request_Id?                   RequestId           = null,
                                                                          DateTime?                     RequestTimestamp    = null,
                                                                          TimeSpan?                     RequestTimeout      = null,
                                                                          EventTracking_Id?             EventTrackingId     = null,
                                                                          NetworkPath?                  NetworkPath         = null,
                                                                          CancellationToken             CancellationToken   = default)

            => ICentralSystem.ChangeAvailability(
                   new ChangeAvailabilityRequest(
                       NetworkingNodeId,
                       ConnectorId,
                       Availability,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region GetConfiguration       (NetworkingNodeId, Keys = null, ...)

        /// <summary>
        /// Get the configuration of the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetConfigurationResponse> GetConfiguration(this ICentralSystem           ICentralSystem,

                                                                      NetworkingNode_Id             NetworkingNodeId,
                                                                      IEnumerable<String>?          Keys                = null,

                                                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                      CustomData?                   CustomData          = null,

                                                                      Request_Id?                   RequestId           = null,
                                                                      DateTime?                     RequestTimestamp    = null,
                                                                      TimeSpan?                     RequestTimeout      = null,
                                                                      EventTracking_Id?             EventTrackingId     = null,
                                                                      NetworkPath?                  NetworkPath         = null,
                                                                      CancellationToken             CancellationToken   = default)

            => ICentralSystem.GetConfiguration(
                   new GetConfigurationRequest(
                       NetworkingNodeId,
                       Keys,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region ChangeConfiguration    (NetworkingNodeId, Key, Value, ...)

        /// <summary>
        /// Change the configuration of the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ChangeConfigurationResponse> ChangeConfiguration(this ICentralSystem           ICentralSystem,

                                                                            NetworkingNode_Id             NetworkingNodeId,
                                                                            String                        Key,
                                                                            String                        Value,

                                                                            IEnumerable<KeyPair>?         SignKeys            = null,
                                                                            IEnumerable<SignInfo>?        SignInfos           = null,
                                                                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                            CustomData?                   CustomData          = null,

                                                                            Request_Id?                   RequestId           = null,
                                                                            DateTime?                     RequestTimestamp    = null,
                                                                            TimeSpan?                     RequestTimeout      = null,
                                                                            EventTracking_Id?             EventTrackingId     = null,
                                                                            NetworkPath?                  NetworkPath         = null,
                                                                            CancellationToken             CancellationToken   = default)

            => ICentralSystem.ChangeConfiguration(
                   new ChangeConfigurationRequest(
                       NetworkingNodeId,
                       Key,
                       Value,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region TransferData           (NetworkingNodeId, VendorId, MessageId, Data, ...)

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional vendor-specific data (a JSON token).</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CP.DataTransferResponse> TransferData(this ICentralSystem           ICentralSystem,

                                                                 NetworkingNode_Id             NetworkingNodeId,
                                                                 Vendor_Id                     VendorId,
                                                                 Message_Id                    MessageId,
                                                                 JToken                        Data,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                 CustomData?                   CustomData          = null,

                                                                 Request_Id?                   RequestId           = null,
                                                                 DateTime?                     RequestTimestamp    = null,
                                                                 TimeSpan?                     RequestTimeout      = null,
                                                                 EventTracking_Id?             EventTrackingId     = null,
                                                                 NetworkPath?                  NetworkPath         = null,
                                                                 CancellationToken             CancellationToken   = default)

            => ICentralSystem.DataTransfer(
                   new DataTransferRequest(
                       NetworkingNodeId,
                       VendorId,
                       MessageId,
                       Data,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region GetDiagnostics         (NetworkingNodeId, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Upload diagnostics data of the given charge box to the given file location.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetDiagnosticsResponse> GetDiagnostics(this ICentralSystem           ICentralSystem,

                                                                  NetworkingNode_Id             NetworkingNodeId,
                                                                  String                        Location,
                                                                  DateTime?                     StartTime           = null,
                                                                  DateTime?                     StopTime            = null,
                                                                  Byte?                         Retries             = null,
                                                                  TimeSpan?                     RetryInterval       = null,

                                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                  CustomData?                   CustomData          = null,

                                                                  Request_Id?                   RequestId           = null,
                                                                  DateTime?                     RequestTimestamp    = null,
                                                                  TimeSpan?                     RequestTimeout      = null,
                                                                  EventTracking_Id?             EventTrackingId     = null,
                                                                  NetworkPath?                  NetworkPath         = null,
                                                                  CancellationToken             CancellationToken   = default)

            => ICentralSystem.GetDiagnostics(
                   new GetDiagnosticsRequest(
                       NetworkingNodeId,
                       Location,
                       StartTime,
                       StopTime,
                       Retries,
                       RetryInterval,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region TriggerMessage         (NetworkingNodeId, RequestedMessage, ConnectorId = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<TriggerMessageResponse> TriggerMessage(this ICentralSystem           ICentralSystem,

                                                                  NetworkingNode_Id             NetworkingNodeId,
                                                                  MessageTrigger                RequestedMessage,
                                                                  Connector_Id?                 ConnectorId         = null,

                                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                  CustomData?                   CustomData          = null,

                                                                  Request_Id?                   RequestId           = null,
                                                                  DateTime?                     RequestTimestamp    = null,
                                                                  TimeSpan?                     RequestTimeout      = null,
                                                                  EventTracking_Id?             EventTrackingId     = null,
                                                                  NetworkPath?                  NetworkPath         = null,
                                                                  CancellationToken             CancellationToken   = default)

            => ICentralSystem.TriggerMessage(
                   new TriggerMessageRequest(
                       NetworkingNodeId,
                       RequestedMessage,
                       ConnectorId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region UpdateFirmware         (NetworkingNodeId, FirmwareURL, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="FirmwareURL">The URI where to download the firmware.</param>
        /// <param name="RetrieveTimestamp">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UpdateFirmwareResponse> UpdateFirmware(this ICentralSystem           ICentralSystem,

                                                                  NetworkingNode_Id             NetworkingNodeId,
                                                                  URL                           FirmwareURL,
                                                                  DateTime                      RetrieveTimestamp,
                                                                  Byte?                         Retries             = null,
                                                                  TimeSpan?                     RetryInterval       = null,

                                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                  CustomData?                   CustomData          = null,

                                                                  Request_Id?                   RequestId           = null,
                                                                  DateTime?                     RequestTimestamp    = null,
                                                                  TimeSpan?                     RequestTimeout      = null,
                                                                  EventTracking_Id?             EventTrackingId     = null,
                                                                  NetworkPath?                  NetworkPath         = null,
                                                                  CancellationToken             CancellationToken   = default)

            => ICentralSystem.UpdateFirmware(
                   new UpdateFirmwareRequest(
                       NetworkingNodeId,
                       FirmwareURL,
                       RetrieveTimestamp,
                       Retries,
                       RetryInterval,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion


        #region ReserveNow             (NetworkingNodeId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ReserveNowResponse> ReserveNow(this ICentralSystem           ICentralSystem,

                                                          NetworkingNode_Id             NetworkingNodeId,
                                                          Connector_Id                  ConnectorId,
                                                          Reservation_Id                ReservationId,
                                                          DateTime                      ExpiryDate,
                                                          IdToken                       IdTag,
                                                          IdToken?                      ParentIdTag         = null,

                                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                          CustomData?                   CustomData          = null,

                                                          Request_Id?                   RequestId           = null,
                                                          DateTime?                     RequestTimestamp    = null,
                                                          TimeSpan?                     RequestTimeout      = null,
                                                          EventTracking_Id?             EventTrackingId     = null,
                                                          NetworkPath?                  NetworkPath         = null,
                                                          CancellationToken             CancellationToken   = default)

            => ICentralSystem.ReserveNow(
                   new ReserveNowRequest(
                       NetworkingNodeId,
                       ConnectorId,
                       ReservationId,
                       ExpiryDate,
                       IdTag,
                       ParentIdTag,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region CancelReservation      (NetworkingNodeId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CancelReservationResponse> CancelReservation(this ICentralSystem           ICentralSystem,

                                                                        NetworkingNode_Id             NetworkingNodeId,
                                                                        Reservation_Id                ReservationId,

                                                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                        CustomData?                   CustomData          = null,

                                                                        Request_Id?                   RequestId           = null,
                                                                        DateTime?                     RequestTimestamp    = null,
                                                                        TimeSpan?                     RequestTimeout      = null,
                                                                        EventTracking_Id?             EventTrackingId     = null,
                                                                        NetworkPath?                  NetworkPath         = null,
                                                                        CancellationToken             CancellationToken   = default)

            => ICentralSystem.CancelReservation(
                   new CancelReservationRequest(
                       NetworkingNodeId,
                       ReservationId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region RemoteStartTransaction (NetworkingNodeId, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Start a charging session at the given charge box connector using the given charging profile.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RemoteStartTransactionResponse> RemoteStartTransaction(this ICentralSystem           ICentralSystem,

                                                                                  NetworkingNode_Id             NetworkingNodeId,
                                                                                  IdToken                       IdTag,
                                                                                  Connector_Id?                 ConnectorId         = null,
                                                                                  ChargingProfile?              ChargingProfile     = null,

                                                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                                  CustomData?                   CustomData          = null,

                                                                                  Request_Id?                   RequestId           = null,
                                                                                  DateTime?                     RequestTimestamp    = null,
                                                                                  TimeSpan?                     RequestTimeout      = null,
                                                                                  EventTracking_Id?             EventTrackingId     = null,
                                                                                  NetworkPath?                  NetworkPath         = null,
                                                                                  CancellationToken             CancellationToken   = default)

            => ICentralSystem.RemoteStartTransaction(
                   new RemoteStartTransactionRequest(
                       NetworkingNodeId,
                       IdTag,
                       ConnectorId,
                       ChargingProfile,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region RemoteStopTransaction  (NetworkingNodeId, TransactionId, ...)

        /// <summary>
        /// Stop a charging session at the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RemoteStopTransactionResponse> RemoteStopTransaction(this ICentralSystem           ICentralSystem,

                                                                                NetworkingNode_Id             NetworkingNodeId,
                                                                                Transaction_Id                TransactionId,

                                                                                IEnumerable<KeyPair>?         SignKeys            = null,
                                                                                IEnumerable<SignInfo>?        SignInfos           = null,
                                                                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                                CustomData?                   CustomData          = null,

                                                                                Request_Id?                   RequestId           = null,
                                                                                DateTime?                     RequestTimestamp    = null,
                                                                                TimeSpan?                     RequestTimeout      = null,
                                                                                EventTracking_Id?             EventTrackingId     = null,
                                                                                NetworkPath?                  NetworkPath         = null,
                                                                                CancellationToken             CancellationToken   = default)

            => ICentralSystem.RemoteStopTransaction(
                   new RemoteStopTransactionRequest(
                       NetworkingNodeId,
                       TransactionId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region SetChargingProfile     (NetworkingNodeId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetChargingProfileResponse> SetChargingProfile(this ICentralSystem           ICentralSystem,

                                                                          NetworkingNode_Id             NetworkingNodeId,
                                                                          Connector_Id                  ConnectorId,
                                                                          ChargingProfile               ChargingProfile,

                                                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                          CustomData?                   CustomData          = null,

                                                                          Request_Id?                   RequestId           = null,
                                                                          DateTime?                     RequestTimestamp    = null,
                                                                          TimeSpan?                     RequestTimeout      = null,
                                                                          EventTracking_Id?             EventTrackingId     = null,
                                                                          NetworkPath?                  NetworkPath         = null,
                                                                          CancellationToken             CancellationToken   = default)

            => ICentralSystem.SetChargingProfile(
                   new SetChargingProfileRequest(
                       NetworkingNodeId,
                       ConnectorId,
                       ChargingProfile,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearChargingProfile   (NetworkingNodeId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearChargingProfileResponse> ClearChargingProfile(this ICentralSystem           ICentralSystem,

                                                                              NetworkingNode_Id             NetworkingNodeId,
                                                                              ChargingProfile_Id?           ChargingProfileId        = null,
                                                                              Connector_Id?                 ConnectorId              = null,
                                                                              ChargingProfilePurposes?      ChargingProfilePurpose   = null,
                                                                              UInt32?                       StackLevel               = null,

                                                                              IEnumerable<KeyPair>?         SignKeys                 = null,
                                                                              IEnumerable<SignInfo>?        SignInfos                = null,
                                                                              IEnumerable<OCPP.Signature>?  Signatures               = null,

                                                                              CustomData?                   CustomData               = null,

                                                                              Request_Id?                   RequestId                = null,
                                                                              DateTime?                     RequestTimestamp         = null,
                                                                              TimeSpan?                     RequestTimeout           = null,
                                                                              EventTracking_Id?             EventTrackingId          = null,
                                                                              NetworkPath?                  NetworkPath              = null,
                                                                              CancellationToken             CancellationToken        = default)

            => ICentralSystem.ClearChargingProfile(
                   new ClearChargingProfileRequest(
                       NetworkingNodeId,
                       ChargingProfileId,
                       ConnectorId,
                       ChargingProfilePurpose,
                       StackLevel,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region GetCompositeSchedule   (NetworkingNodeId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetCompositeScheduleResponse> GetCompositeSchedule(this ICentralSystem           ICentralSystem,

                                                                              NetworkingNode_Id             NetworkingNodeId,
                                                                              Connector_Id                  ConnectorId,
                                                                              TimeSpan                      Duration,
                                                                              ChargingRateUnits?            ChargingRateUnit    = null,

                                                                              IEnumerable<KeyPair>?         SignKeys            = null,
                                                                              IEnumerable<SignInfo>?        SignInfos           = null,
                                                                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                              CustomData?                   CustomData          = null,

                                                                              Request_Id?                   RequestId           = null,
                                                                              DateTime?                     RequestTimestamp    = null,
                                                                              TimeSpan?                     RequestTimeout      = null,
                                                                              EventTracking_Id?             EventTrackingId     = null,
                                                                              NetworkPath?                  NetworkPath         = null,
                                                                              CancellationToken             CancellationToken   = default)

            => ICentralSystem.GetCompositeSchedule(
                   new GetCompositeScheduleRequest(
                       NetworkingNodeId,
                       ConnectorId,
                       Duration,
                       ChargingRateUnit,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region UnlockConnector        (NetworkingNodeId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UnlockConnectorResponse> UnlockConnector(this ICentralSystem           ICentralSystem,

                                                                    NetworkingNode_Id             NetworkingNodeId,
                                                                    Connector_Id                  ConnectorId,

                                                                    IEnumerable<KeyPair>?         SignKeys            = null,
                                                                    IEnumerable<SignInfo>?        SignInfos           = null,
                                                                    IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                    CustomData?                   CustomData          = null,

                                                                    Request_Id?                   RequestId           = null,
                                                                    DateTime?                     RequestTimestamp    = null,
                                                                    TimeSpan?                     RequestTimeout      = null,
                                                                    EventTracking_Id?             EventTrackingId     = null,
                                                                    NetworkPath?                  NetworkPath         = null,
                                                                    CancellationToken             CancellationToken   = default)

            => ICentralSystem.UnlockConnector(
                   new UnlockConnectorRequest(
                       NetworkingNodeId,
                       ConnectorId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion


        #region GetLocalListVersion    (NetworkingNodeId, ...)

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetLocalListVersionResponse> GetLocalListVersion(this ICentralSystem           ICentralSystem,

                                                                            NetworkingNode_Id             NetworkingNodeId,

                                                                            IEnumerable<KeyPair>?         SignKeys            = null,
                                                                            IEnumerable<SignInfo>?        SignInfos           = null,
                                                                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                                            CustomData?                   CustomData          = null,

                                                                            Request_Id?                   RequestId           = null,
                                                                            DateTime?                     RequestTimestamp    = null,
                                                                            TimeSpan?                     RequestTimeout      = null,
                                                                            EventTracking_Id?             EventTrackingId     = null,
                                                                            NetworkPath?                  NetworkPath         = null,
                                                                            CancellationToken             CancellationToken   = default)

            => ICentralSystem.GetLocalListVersion(
                   new GetLocalListVersionRequest(
                       NetworkingNodeId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region SendLocalList          (NetworkingNodeId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SendLocalListResponse> SendLocalList(this ICentralSystem              ICentralSystem,

                                                                NetworkingNode_Id                NetworkingNodeId,
                                                                UInt64                           ListVersion,
                                                                UpdateTypes                      UpdateType,
                                                                IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                                                                IEnumerable<KeyPair>?            SignKeys                 = null,
                                                                IEnumerable<SignInfo>?           SignInfos                = null,
                                                                IEnumerable<OCPP.Signature>?     Signatures               = null,

                                                                CustomData?                      CustomData               = null,

                                                                Request_Id?                      RequestId                = null,
                                                                DateTime?                        RequestTimestamp         = null,
                                                                TimeSpan?                        RequestTimeout           = null,
                                                                EventTracking_Id?                EventTrackingId          = null,
                                                                NetworkPath?                     NetworkPath              = null,
                                                                CancellationToken                CancellationToken        = default)

            => ICentralSystem.SendLocalList(
                   new SendLocalListRequest(
                       NetworkingNodeId,
                       ListVersion,
                       UpdateType,
                       LocalAuthorizationList,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearCache             (NetworkingNodeId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="ICentralSystem">A central system.</param>
        /// <param name="NetworkingNodeId">The charge box identification.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearCacheResponse> ClearCache(this ICentralSystem           ICentralSystem,

                                                          NetworkingNode_Id             NetworkingNodeId,

                                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                          CustomData?                   CustomData          = null,

                                                          Request_Id?                   RequestId           = null,
                                                          DateTime?                     RequestTimestamp    = null,
                                                          TimeSpan?                     RequestTimeout      = null,
                                                          EventTracking_Id?             EventTrackingId     = null,
                                                          NetworkPath?                  NetworkPath         = null,
                                                          CancellationToken             CancellationToken   = default)

            => ICentralSystem.ClearCache(
                   new ClearCacheRequest(
                       NetworkingNodeId,

                       SignKeys,
                       SignInfos,
                       Signatures,

                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       NetworkPath,
                       CancellationToken
                   )
               );

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData     (NetworkingNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given binary data to the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<BinaryDataTransferResponse>

            TransferBinaryData(this ICentralSystem           CentralSystem,

                               NetworkingNode_Id             NetworkingNodeId,
                               Vendor_Id                     VendorId,
                               Message_Id?                   MessageId           = null,
                               Byte[]?                       Data                = null,
                               BinaryFormats?                Format              = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => CentralSystem.BinaryDataTransfer(
                       new BinaryDataTransferRequest(
                           NetworkingNodeId,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetFile                (NetworkingNodeId, Filename, Priority = null, ...)

        /// <summary>
        /// Request to download the given file from the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="Filename">The name of the file including its absolute path.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<OCPP.CS.GetFileResponse>

            GetFile(this ICentralSystem           CentralSystem,

                    NetworkingNode_Id             NetworkingNodeId,
                    FilePath                      Filename,
                    Byte?                         Priority            = null,

                    IEnumerable<KeyPair>?         SignKeys            = null,
                    IEnumerable<SignInfo>?        SignInfos           = null,
                    IEnumerable<OCPP.Signature>?  Signatures          = null,

                    CustomData?                   CustomData          = null,

                    Request_Id?                   RequestId           = null,
                    DateTime?                     RequestTimestamp    = null,
                    TimeSpan?                     RequestTimeout      = null,
                    EventTracking_Id?             EventTrackingId     = null,
                    CancellationToken             CancellationToken   = default)


                => CentralSystem.GetFile(
                       new OCPP.CSMS.GetFileRequest(
                           NetworkingNodeId,
                           Filename,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFile               (NetworkingNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileContent">The file content.</param>
        /// <param name="FileContentType">An optional content/MIME type of the file.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
        /// <param name="FileSignatures">An optional enumeration of cryptographic signatures for the file content.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<OCPP.CS.SendFileResponse>

            SendFile(this ICentralSystem           CentralSystem,

                     NetworkingNode_Id             NetworkingNodeId,
                     FilePath                      FileName,
                     Byte[]                        FileContent,
                     ContentType?                  FileContentType     = null,
                     Byte[]?                       FileSHA256          = null,
                     Byte[]?                       FileSHA512          = null,
                     IEnumerable<OCPP.Signature>?  FileSignatures      = null,
                     Byte?                         Priority            = null,

                     IEnumerable<KeyPair>?         SignKeys            = null,
                     IEnumerable<SignInfo>?        SignInfos           = null,
                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                     CustomData?                   CustomData          = null,

                     Request_Id?                   RequestId           = null,
                     DateTime?                     RequestTimestamp    = null,
                     TimeSpan?                     RequestTimeout      = null,
                     EventTracking_Id?             EventTrackingId     = null,
                     CancellationToken             CancellationToken   = default)


                => CentralSystem.SendFile(
                       new OCPP.CSMS.SendFileRequest(
                           NetworkingNodeId,
                           FileName,
                           FileContent,
                           FileContentType,
                           FileSHA256,
                           FileSHA512,
                           FileSignatures,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteFile             (NetworkingNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<OCPP.CS.DeleteFileResponse>

            DeleteFile(this ICentralSystem           CentralSystem,

                       NetworkingNode_Id             NetworkingNodeId,
                       FilePath                      FileName,
                       Byte[]?                       FileSHA256          = null,
                       Byte[]?                       FileSHA512          = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       CustomData?                   CustomData          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => CentralSystem.DeleteFile(
                       new OCPP.CSMS.DeleteFileRequest(
                           NetworkingNodeId,
                           FileName,
                           FileSHA256,
                           FileSHA512,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


    }

}
