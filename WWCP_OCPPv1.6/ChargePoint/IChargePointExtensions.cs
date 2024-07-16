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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// Extension methods for all charge points
    /// </summary>
    public static class IChargePointExtensions
    {

        #region SendBootNotification              (...)

        /// <summary>
        /// Send a boot notification to the central system.
        /// </summary>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.BootNotificationResponse>

            SendBootNotification(this IChargePoint             ChargePoint,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 CustomData?                   CustomData          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => ChargePoint.BootNotification(
                       new BootNotificationRequest(

                           ChargePoint.Id,

                           ChargePoint.ChargePointVendor,
                           ChargePoint.ChargePointModel,

                           ChargePoint.ChargePointSerialNumber,
                           ChargePoint.ChargeBoxSerialNumber,
                           ChargePoint.FirmwareVersion,
                           ChargePoint.Iccid,
                           ChargePoint.IMSI,
                           ChargePoint.MeterType,
                           ChargePoint.MeterSerialNumber,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendHeartbeat                     (...)

        /// <summary>
        /// Send a boot heartbeat to the central system.
        /// </summary>
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.HeartbeatResponse>

            SendHeartbeat(this IChargePoint             ChargePoint,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<Signature>?       Signatures          = null,

                          CustomData?                   CustomData          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => ChargePoint.Heartbeat(
                       new HeartbeatRequest(

                           ChargePoint.Id,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendDiagnosticsStatusNotification (Status, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.DiagnosticsStatusNotificationResponse>

            SendDiagnosticsStatusNotification(this IChargePoint             ChargePoint,

                                              DiagnosticsStatus             Status,

                                              IEnumerable<KeyPair>?         SignKeys            = null,
                                              IEnumerable<SignInfo>?        SignInfos           = null,
                                              IEnumerable<Signature>?       Signatures          = null,

                                              CustomData?                   CustomData          = null,

                                              Request_Id?                   RequestId           = null,
                                              DateTime?                     RequestTimestamp    = null,
                                              TimeSpan?                     RequestTimeout      = null,
                                              EventTracking_Id?             EventTrackingId     = null,
                                              CancellationToken             CancellationToken   = default)


                => ChargePoint.DiagnosticsStatusNotification(
                       new DiagnosticsStatusNotificationRequest(

                           ChargePoint.Id,

                           Status,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendFirmwareStatusNotification    (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(this IChargePoint             ChargePoint,

                                           FirmwareStatus                Status,

                                           IEnumerable<KeyPair>?         SignKeys            = null,
                                           IEnumerable<SignInfo>?        SignInfos           = null,
                                           IEnumerable<Signature>?       Signatures          = null,

                                           CustomData?                   CustomData          = null,

                                           Request_Id?                   RequestId           = null,
                                           DateTime?                     RequestTimestamp    = null,
                                           TimeSpan?                     RequestTimeout      = null,
                                           EventTracking_Id?             EventTrackingId     = null,
                                           CancellationToken             CancellationToken   = default)


                => ChargePoint.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(

                           ChargePoint.Id,

                           Status,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion


        #region Authorize                         (IdTag, ...)

        /// <summary>
        /// Authorize the given (RFID) token.
        /// </summary>
        /// <param name="IdTag">The identifier tag (RFID UID) that needs to be authorized.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.AuthorizeResponse>

            Authorize(this IChargePoint             ChargePoint,

                      IdToken                       IdTag,

                      IEnumerable<KeyPair>?         SignKeys            = null,
                      IEnumerable<SignInfo>?        SignInfos           = null,
                      IEnumerable<Signature>?       Signatures          = null,

                      CustomData?                   CustomData          = null,

                      Request_Id?                   RequestId           = null,
                      DateTime?                     RequestTimestamp    = null,
                      TimeSpan?                     RequestTimeout      = null,
                      EventTracking_Id?             EventTrackingId     = null,
                      CancellationToken             CancellationToken   = default)


                => ChargePoint.Authorize(
                       new AuthorizeRequest(

                           ChargePoint.Id,

                           IdTag,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendStartTransactionNotification  (ConnectorId, IdTag, StartTimestamp, MeterStart, ReservationId = null, ...)

        /// <summary>
        /// Send a notification about a started charging process at the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.StartTransactionResponse>

            SendStartTransactionNotification(this IChargePoint             ChargePoint,

                                             Connector_Id                  ConnectorId,
                                             IdToken                       IdTag,
                                             DateTime                      StartTimestamp,
                                             UInt64                        MeterStart,
                                             Reservation_Id?               ReservationId       = null,

                                             IEnumerable<KeyPair>?         SignKeys            = null,
                                             IEnumerable<SignInfo>?        SignInfos           = null,
                                             IEnumerable<Signature>?       Signatures          = null,

                                             CustomData?                   CustomData          = null,

                                             Request_Id?                   RequestId           = null,
                                             DateTime?                     RequestTimestamp    = null,
                                             TimeSpan?                     RequestTimeout      = null,
                                             EventTracking_Id?             EventTrackingId     = null,
                                             CancellationToken             CancellationToken   = default)


                => ChargePoint.StartTransaction(
                       new StartTransactionRequest(

                           ChargePoint.Id,

                           ConnectorId,
                           IdTag,
                           StartTimestamp,
                           MeterStart,
                           ReservationId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendStatusNotification            (ConnectorId, Status, ErrorCode, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
        /// <param name="VendorErrorCode">A vendor-specific error code.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.StatusNotificationResponse>

            SendStatusNotification(this IChargePoint             ChargePoint,

                                   Connector_Id                  ConnectorId,
                                   ChargePointStatus             Status,
                                   ChargePointErrorCodes         ErrorCode,
                                   String?                       Info                = null,
                                   DateTime?                     StatusTimestamp     = null,
                                   String?                       VendorId            = null,
                                   String?                       VendorErrorCode     = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   CustomData?                   CustomData          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => ChargePoint.StatusNotification(
                       new StatusNotificationRequest(

                           ChargePoint.Id,

                           ConnectorId,
                           Status,
                           ErrorCode,
                           Info,
                           StatusTimestamp,
                           VendorId,
                           VendorErrorCode,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendMeterValues                   (ConnectorId, TransactionId = null, MeterValues = null, ...)

        /// <summary>
        /// Send meter values for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.MeterValuesResponse>

            SendMeterValues(this IChargePoint             ChargePoint,

                            Connector_Id                  ConnectorId,
                            IEnumerable<MeterValue>       MeterValues,
                            Transaction_Id?               TransactionId       = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<Signature>?       Signatures          = null,

                            CustomData?                   CustomData          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => ChargePoint.MeterValues(
                       new MeterValuesRequest(

                           ChargePoint.Id,

                           ConnectorId,
                           MeterValues,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendStopTransactionNotification   (TransactionId, StopTimestamp, MeterStop, ...)

        /// <summary>
        /// Send a notification about a stopped charging process at the given connector.
        /// </summary>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.StopTransactionResponse>

            SendStopTransactionNotification(this IChargePoint             ChargePoint,

                                            Transaction_Id                TransactionId,
                                            DateTime                      StopTimestamp,
                                            UInt64                        MeterStop,
                                            IdToken?                      IdTag               = null,
                                            Reasons?                      Reason              = null,
                                            IEnumerable<MeterValue>?      TransactionData     = null,

                                            IEnumerable<KeyPair>?         SignKeys            = null,
                                            IEnumerable<SignInfo>?        SignInfos           = null,
                                            IEnumerable<Signature>?       Signatures          = null,

                                            CustomData?                   CustomData          = null,

                                            Request_Id?                   RequestId           = null,
                                            DateTime?                     RequestTimestamp    = null,
                                            TimeSpan?                     RequestTimeout      = null,
                                            EventTracking_Id?             EventTrackingId     = null,
                                            CancellationToken             CancellationToken   = default)


                => ChargePoint.StopTransaction(
                       new StopTransactionRequest(

                           ChargePoint.Id,

                           TransactionId,
                           StopTimestamp,
                           MeterStop,
                           IdTag,
                           Reason,
                           TransactionData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion


        #region TransferData                      (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">The charge point model identification.</param>
        /// <param name="Data">Optional vendor-specific data (a JSON token).</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.DataTransferResponse>

            TransferData(this IChargePoint             ChargePoint,

                         Vendor_Id                     VendorId,
                         Message_Id?                   MessageId           = null,
                         JToken?                       Data                = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<Signature>?       Signatures          = null,

                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => ChargePoint.DataTransfer(
                       new DataTransferRequest(

                           ChargePoint.Id,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion


        //ToDo: Add Security Extensions


        // Binary Data Streams Extensions

        //#region TransferBinaryData                (VendorId, MessageId = null, Data = null, ...)

        ///// <summary>
        ///// Transfer the given binary data to the CSMS.
        ///// </summary>
        ///// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        ///// <param name="MessageId">An optional message identification field.</param>
        ///// <param name="Data">Optional message data as text without specified length or format.</param>
        ///// 
        ///// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<BinaryDataTransferResponse>

        //    TransferBinaryData(this IChargePoint             ChargePoint,

        //                       Vendor_Id                     VendorId,
        //                       Message_Id?                   MessageId           = null,
        //                       Byte[]?                       Data                = null,
        //                       BinaryFormats?                Format              = null,

        //                       IEnumerable<KeyPair>?         SignKeys            = null,
        //                       IEnumerable<SignInfo>?        SignInfos           = null,
        //                       IEnumerable<Signature>?       Signatures          = null,

        //                       Request_Id?                   RequestId           = null,
        //                       DateTime?                     RequestTimestamp    = null,
        //                       TimeSpan?                     RequestTimeout      = null,
        //                       EventTracking_Id?             EventTrackingId     = null,
        //                       CancellationToken             CancellationToken   = default)


        //        => ChargePoint.BinaryDataTransfer(
        //               new BinaryDataTransferRequest(

        //                   ChargePoint.Id,

        //                   VendorId,
        //                   MessageId,
        //                   Data,
        //                   Format,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   RequestId        ?? ChargePoint.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? ChargePoint.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath.Empty,
        //                   CancellationToken

        //               )
        //           );

        //#endregion


    }

}
