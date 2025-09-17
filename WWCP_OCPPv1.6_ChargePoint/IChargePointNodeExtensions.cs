/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// Extension methods for all Charge Points.
    /// </summary>
    public static class IChargePointNodeExtensions
    {

        #region Certificates

        #region SignCertificate                      (CSR, ...)

        /// <summary>
        /// Send a SignCertificate request.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// <param name="CSR">The PEM encoded certificate signing request (CSR) [max 5500].</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SignCertificateResponse>

            SignCertificate(this IChargePointNode    ChargePoint,
                            String                   CSR,
                            SourceRouting?           Destination           = null,

                            IEnumerable<KeyPair>?    SignKeys              = null,
                            IEnumerable<SignInfo>?   SignInfos             = null,
                            IEnumerable<Signature>?  Signatures            = null,

                            CustomData?              CustomData            = null,

                            Request_Id?              RequestId             = null,
                            DateTimeOffset?          RequestTimestamp      = null,
                            TimeSpan?                RequestTimeout        = null,
                            EventTracking_Id?        EventTrackingId       = null,
                            SerializationFormats?    SerializationFormat   = null,
                            CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.SignCertificate(
                       new SignCertificateRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           CSR,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

        #region Charging

        #region Authorize                            (IdTag, ...)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="IdTag">The identifier that needs to be Authorized.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<AuthorizeResponse>

            Authorize(this IChargePointNode    ChargePoint,
                      IdToken                  IdTag,
                      SourceRouting?           Destination           = null,

                      IEnumerable<KeyPair>?    SignKeys              = null,
                      IEnumerable<SignInfo>?   SignInfos             = null,
                      IEnumerable<Signature>?  Signatures            = null,

                      CustomData?              CustomData            = null,

                      Request_Id?              RequestId             = null,
                      DateTimeOffset?          RequestTimestamp      = null,
                      TimeSpan?                RequestTimeout        = null,
                      EventTracking_Id?        EventTrackingId       = null,
                      SerializationFormats?    SerializationFormat   = null,
                      CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.Authorize(
                       new AuthorizeRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           IdTag,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendMeterValues                      (ConnectorId, MeterValues, TransactionId = null, ...)

        /// <summary>
        /// Send MeterValues.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<MeterValuesResponse>

            SendMeterValues(this IChargePointNode    ChargePoint,
                            Connector_Id             ConnectorId,
                            IEnumerable<MeterValue>  MeterValues,
                            Transaction_Id?          TransactionId         = null,
                            SourceRouting?           Destination           = null,

                            IEnumerable<KeyPair>?    SignKeys              = null,
                            IEnumerable<SignInfo>?   SignInfos             = null,
                            IEnumerable<Signature>?  Signatures            = null,

                            CustomData?              CustomData            = null,

                            Request_Id?              RequestId             = null,
                            DateTimeOffset?          RequestTimestamp      = null,
                            TimeSpan?                RequestTimeout        = null,
                            EventTracking_Id?        EventTrackingId       = null,
                            SerializationFormats?    SerializationFormat   = null,
                            CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.MeterValues(
                       new MeterValuesRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           ConnectorId,
                           MeterValues,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendStartTransactionNotification     (ConnectorId, IdTag, StartTimestamp, MeterStart, ReservationId = null, ...)

        /// <summary>
        /// Send a StartTransaction notification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<StartTransactionResponse>

            SendStartTransactionNotification(this IChargePointNode    ChargePoint,
                                             Connector_Id             ConnectorId,
                                             IdToken                  IdTag,
                                             DateTimeOffset           StartTimestamp,
                                             UInt64                   MeterStart,
                                             Reservation_Id?          ReservationId         = null,
                                             SourceRouting?           Destination           = null,

                                             IEnumerable<KeyPair>?    SignKeys              = null,
                                             IEnumerable<SignInfo>?   SignInfos             = null,
                                             IEnumerable<Signature>?  Signatures            = null,

                                             CustomData?              CustomData            = null,

                                             Request_Id?              RequestId             = null,
                                             DateTimeOffset?          RequestTimestamp      = null,
                                             TimeSpan?                RequestTimeout        = null,
                                             EventTracking_Id?        EventTrackingId       = null,
                                             SerializationFormats?    SerializationFormat   = null,
                                             CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.StartTransaction(
                       new StartTransactionRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
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
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendStatusNotification               (ConnectorId, Status, ErrorCode, Info = null, StatusTimestamp = null, VendorId = null, VendorErrorCode = null, ...)

        /// <summary>
        /// Send a StatusNotification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">An optional identifier of a vendor-specific extension.</param>
        /// <param name="VendorErrorCode">An optional vendor-specific error code.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<StatusNotificationResponse>

            SendStatusNotification(this IChargePointNode    ChargePoint,
                                   Connector_Id             ConnectorId,
                                   ChargePointStatus        Status,
                                   ChargePointErrorCodes    ErrorCode,
                                   String?                  Info                  = null,
                                   DateTimeOffset?          StatusTimestamp       = null,
                                   String?                  VendorId              = null,
                                   String?                  VendorErrorCode       = null,
                                   SourceRouting?           Destination           = null,

                                   IEnumerable<KeyPair>?    SignKeys              = null,
                                   IEnumerable<SignInfo>?   SignInfos             = null,
                                   IEnumerable<Signature>?  Signatures            = null,

                                   CustomData?              CustomData            = null,

                                   Request_Id?              RequestId             = null,
                                   DateTimeOffset?          RequestTimestamp      = null,
                                   TimeSpan?                RequestTimeout        = null,
                                   EventTracking_Id?        EventTrackingId       = null,
                                   SerializationFormats?    SerializationFormat   = null,
                                   CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.StatusNotification(
                       new StatusNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
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
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendStopTransactionNotification      (TransactionId, StopTimestamp, MeterStop, IdTag = null, Reason = null, TransactionData = null, ...)

        /// <summary>
        /// Send a StopTransaction notification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<StopTransactionResponse>

            SendStopTransactionNotification(this IChargePointNode     ChargePoint,
                                            Transaction_Id            TransactionId,
                                            DateTimeOffset            StopTimestamp,
                                            UInt64                    MeterStop,
                                            IdToken?                  IdTag                 = null,
                                            Reasons?                  Reason                = null,
                                            IEnumerable<MeterValue>?  TransactionData       = null,
                                            SourceRouting?            Destination           = null,

                                            IEnumerable<KeyPair>?     SignKeys              = null,
                                            IEnumerable<SignInfo>?    SignInfos             = null,
                                            IEnumerable<Signature>?   Signatures            = null,

                                            CustomData?               CustomData            = null,

                                            Request_Id?               RequestId             = null,
                                            DateTimeOffset?           RequestTimestamp      = null,
                                            TimeSpan?                 RequestTimeout        = null,
                                            EventTracking_Id?         EventTrackingId       = null,
                                            SerializationFormats?     SerializationFormat   = null,
                                            CancellationToken         CancellationToken     = default)


                => ChargePoint.OCPP.OUT.StopTransaction(
                       new StopTransactionRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
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
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

        #region Common

        #region TransferData                         (                    VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given vendor-specific data to the given central system.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this IChargePointNode    ChargePoint,
                         Vendor_Id                VendorId,
                         Message_Id?              MessageId             = null,
                         JToken?                  Data                  = null,

                         IEnumerable<KeyPair>?    SignKeys              = null,
                         IEnumerable<SignInfo>?   SignInfos             = null,
                         IEnumerable<Signature>?  Signatures            = null,

                         Request_Id?              RequestId             = null,
                         DateTimeOffset?          RequestTimestamp      = null,
                         TimeSpan?                RequestTimeout        = null,
                         EventTracking_Id?        EventTrackingId       = null,
                         SerializationFormats?    SerializationFormat   = null,
                         CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(
                           SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region TransferData                         (Destination = null, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given vendor-specific data to the given destination.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this IChargePointNode    ChargePoint,
                         SourceRouting            Destination,
                         Vendor_Id                VendorId,
                         Message_Id?              MessageId             = null,
                         JToken?                  Data                  = null,

                         IEnumerable<KeyPair>?    SignKeys              = null,
                         IEnumerable<SignInfo>?   SignInfos             = null,
                         IEnumerable<Signature>?  Signatures            = null,

                         Request_Id?              RequestId             = null,
                         DateTimeOffset?          RequestTimestamp      = null,
                         TimeSpan?                RequestTimeout        = null,
                         EventTracking_Id?        EventTrackingId       = null,
                         SerializationFormats?    SerializationFormat   = null,
                         CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(
                           Destination,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

        #region Firmware

        #region SendBootNotification                 (ChargePointVendor = null, ChargePointModel = null, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="ChargePointVendor">The optional charge point vendor identification.</param>
        /// <param name="ChargePointModel">The optional charge point model identification.</param>
        /// <param name="ChargePointSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the charge point.</param>
        /// <param name="Iccid">The optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">The optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">The optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">The optional serial number of the main power meter of the charge point.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<BootNotificationResponse>

            SendBootNotification(this IChargePointNode    ChargePoint,
                                 String?                  ChargePointVendor         = null,
                                 String?                  ChargePointModel          = null,
                                 String?                  ChargePointSerialNumber   = null,
                                 String?                  ChargeBoxSerialNumber     = null,
                                 String?                  FirmwareVersion           = null,
                                 String?                  Iccid                     = null,
                                 String?                  IMSI                      = null,
                                 String?                  MeterType                 = null,
                                 String?                  MeterSerialNumber         = null,
                                 SourceRouting?           Destination               = null,

                                 IEnumerable<KeyPair>?    SignKeys                  = null,
                                 IEnumerable<SignInfo>?   SignInfos                 = null,
                                 IEnumerable<Signature>?  Signatures                = null,

                                 CustomData?              CustomData                = null,

                                 Request_Id?              RequestId                 = null,
                                 DateTimeOffset?          RequestTimestamp          = null,
                                 TimeSpan?                RequestTimeout            = null,
                                 EventTracking_Id?        EventTrackingId           = null,
                                 SerializationFormats?    SerializationFormat       = null,
                                 CancellationToken        CancellationToken         = default)


                => ChargePoint.OCPP.OUT.BootNotification(
                       new BootNotificationRequest(

                           Destination             ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),

                           ChargePointVendor       ?? ChargePoint.ChargePointVendor,
                           ChargePointModel        ?? ChargePoint.ChargePointModel,
                           ChargePointSerialNumber ?? ChargePoint.ChargePointSerialNumber,
                           ChargeBoxSerialNumber   ?? ChargePoint.ChargeBoxSerialNumber,
                           FirmwareVersion         ?? ChargePoint.FirmwareVersion,
                           Iccid                   ?? ChargePoint.Iccid,
                           IMSI                    ?? ChargePoint.IMSI,
                           MeterType               ?? ChargePoint.UplinkEnergyMeter?.Model        ?? ChargePoint.Connectors.FirstOrDefault()?.EnergyMeter?.Model,
                           MeterSerialNumber       ?? ChargePoint.UplinkEnergyMeter?.SerialNumber ?? ChargePoint.Connectors.FirstOrDefault()?.EnergyMeter?.SerialNumber,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId               ?? ChargePoint.NextRequestId,
                           RequestTimestamp        ?? Timestamp.Now,
                           RequestTimeout          ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId         ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendHeartbeat                        (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<HeartbeatResponse>

            SendHeartbeat(this IChargePointNode    ChargePoint,
                          SourceRouting?           Destination           = null,

                          IEnumerable<KeyPair>?    SignKeys              = null,
                          IEnumerable<SignInfo>?   SignInfos             = null,
                          IEnumerable<Signature>?  Signatures            = null,

                          CustomData?              CustomData            = null,

                          Request_Id?              RequestId             = null,
                          DateTimeOffset?          RequestTimestamp      = null,
                          TimeSpan?                RequestTimeout        = null,
                          EventTracking_Id?        EventTrackingId       = null,
                          SerializationFormats?    SerializationFormat   = null,
                          CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.Heartbeat(
                       new HeartbeatRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFirmwareStatusNotification       (FirmwareStatus, ...)

        /// <summary>
        /// Send a FirmwareStatusNotification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="FirmwareStatus">The firmware status.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(this IChargePointNode    ChargePoint,
                                           FirmwareStatus           FirmwareStatus,
                                           SourceRouting?           Destination           = null,

                                           IEnumerable<KeyPair>?    SignKeys              = null,
                                           IEnumerable<SignInfo>?   SignInfos             = null,
                                           IEnumerable<Signature>?  Signatures            = null,

                                           CustomData?              CustomData            = null,

                                           Request_Id?              RequestId             = null,
                                           DateTimeOffset?          RequestTimestamp      = null,
                                           TimeSpan?                RequestTimeout        = null,
                                           EventTracking_Id?        EventTrackingId       = null,
                                           SerializationFormats?    SerializationFormat   = null,
                                           CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           FirmwareStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendSignedFirmwareStatusNotification (FirmwareStatus, ...)

        /// <summary>
        /// Send a SignedFirmwareStatusNotification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="FirmwareStatus">The firmware status.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SignedFirmwareStatusNotificationResponse>

            SendSignedFirmwareStatusNotification(this IChargePointNode    ChargePoint,
                                                 FirmwareStatus           FirmwareStatus,
                                                 SourceRouting?           Destination           = null,

                                                 IEnumerable<KeyPair>?    SignKeys              = null,
                                                 IEnumerable<SignInfo>?   SignInfos             = null,
                                                 IEnumerable<Signature>?  Signatures            = null,

                                                 CustomData?              CustomData            = null,

                                                 Request_Id?              RequestId             = null,
                                                 DateTimeOffset?          RequestTimestamp      = null,
                                                 TimeSpan?                RequestTimeout        = null,
                                                 EventTracking_Id?        EventTrackingId       = null,
                                                 SerializationFormats?    SerializationFormat   = null,
                                                 CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.SignedFirmwareStatusNotification(
                       new SignedFirmwareStatusNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           FirmwareStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

        #region Monitoring

        #region SendDiagnosticsStatusNotification    (DiagnosticsStatus, ...)

        /// <summary>
        /// Send a diagnostics status notification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="DiagnosticsStatus">The diagnostics status.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DiagnosticsStatusNotificationResponse>

            SendDiagnosticsStatusNotification(this IChargePointNode    ChargePoint,
                                              DiagnosticsStatus        DiagnosticsStatus,
                                              SourceRouting?           Destination           = null,

                                              IEnumerable<KeyPair>?    SignKeys              = null,
                                              IEnumerable<SignInfo>?   SignInfos             = null,
                                              IEnumerable<Signature>?  Signatures            = null,

                                              CustomData?              CustomData            = null,

                                              Request_Id?              RequestId             = null,
                                              DateTimeOffset?          RequestTimestamp      = null,
                                              TimeSpan?                RequestTimeout        = null,
                                              EventTracking_Id?        EventTrackingId       = null,
                                              SerializationFormats?    SerializationFormat   = null,
                                              CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.DiagnosticsStatusNotification(
                       new DiagnosticsStatusNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           DiagnosticsStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendLogStatusNotification            (Status, LogRequestId = null, ...)

        /// <summary>
        /// Send a LogStatusNotification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="Status">The status of the log upload.</param>
        /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<LogStatusNotificationResponse>

            SendLogStatusNotification(this IChargePointNode    ChargePoint,
                                      UploadLogStatus          Status,
                                      Int32?                   LogRequestId          = null,
                                      SourceRouting?           Destination           = null,

                                      IEnumerable<KeyPair>?    SignKeys              = null,
                                      IEnumerable<SignInfo>?   SignInfos             = null,
                                      IEnumerable<Signature>?  Signatures            = null,

                                      CustomData?              CustomData            = null,

                                      Request_Id?              RequestId             = null,
                                      DateTimeOffset?          RequestTimestamp      = null,
                                      TimeSpan?                RequestTimeout        = null,
                                      EventTracking_Id?        EventTrackingId       = null,
                                      SerializationFormats?    SerializationFormat   = null,
                                      CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.LogStatusNotification(
                       new LogStatusNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           Status,
                           LogRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendSecurityEventNotification        (Type, Timestamp, TechInfo = null, ...)

        /// <summary>
        /// Send a SecurityEventNotification.
        /// </summary>
        /// <param name="ChargePoint">The charge point.</param>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
        /// <param name="Destination">The optional networking node identification. Default is 'CentralSystem'.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SecurityEventNotificationResponse>

            SendSecurityEventNotification(this IChargePointNode    ChargePoint,
                                          SecurityEvent            Type,
                                          DateTimeOffset           Timestamp,
                                          String?                  TechInfo              = null,
                                          SourceRouting?           Destination           = null,

                                          IEnumerable<KeyPair>?    SignKeys              = null,
                                          IEnumerable<SignInfo>?   SignInfos             = null,
                                          IEnumerable<Signature>?  Signatures            = null,

                                          CustomData?              CustomData            = null,

                                          Request_Id?              RequestId             = null,
                                          DateTimeOffset?          RequestTimestamp      = null,
                                          TimeSpan?                RequestTimeout        = null,
                                          EventTracking_Id?        EventTrackingId       = null,
                                          SerializationFormats?    SerializationFormat   = null,
                                          CancellationToken        CancellationToken     = default)


                => ChargePoint.OCPP.OUT.SecurityEventNotification(
                       new SecurityEventNotificationRequest(
                           Destination      ?? SourceRouting.To(NetworkingNode_Id.CentralSystem),
                           Type,
                           Timestamp,
                           TechInfo,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargePoint.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? ChargePoint.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(ChargePoint.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

    }

}
