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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// Extension methods for all Central System nodes.
    /// </summary>
    public static class ICentralSystemNodeExtensions
    {

        #region Certificates

        #region SendCertificateSigned       (Destination, CertificateChain, ...)

        /// <summary>
        /// Send a signed certificate to the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
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
        public static Task<CertificateSignedResponse>

            SendCertificateSigned(this ICentralSystemNode  CentralSystem,
                                  SourceRouting            Destination,
                                  CertificateChain         CertificateChain,

                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                  IEnumerable<Signature>?  Signatures            = null,

                                  CustomData?              CustomData            = null,

                                  Request_Id?              RequestId             = null,
                                  DateTime?                RequestTimestamp      = null,
                                  TimeSpan?                RequestTimeout        = null,
                                  EventTracking_Id?        EventTrackingId       = null,
                                  SerializationFormats?    SerializationFormat   = null,
                                  CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.CertificateSigned(
                       new CertificateSignedRequest(
                           Destination,
                           CertificateChain,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteCertificate           (Destination, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate from the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
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
        public static Task<DeleteCertificateResponse>

            DeleteCertificate(this ICentralSystemNode  CentralSystem,
                              SourceRouting            Destination,
                              CertificateHashData      CertificateHashData,

                              IEnumerable<KeyPair>?    SignKeys              = null,
                              IEnumerable<SignInfo>?   SignInfos             = null,
                              IEnumerable<Signature>?  Signatures            = null,

                              CustomData?              CustomData            = null,

                              Request_Id?              RequestId             = null,
                              DateTime?                RequestTimestamp      = null,
                              TimeSpan?                RequestTimeout        = null,
                              EventTracking_Id?        EventTrackingId       = null,
                              SerializationFormats?    SerializationFormat   = null,
                              CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.DeleteCertificate(
                       new DeleteCertificateRequest(
                           Destination,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetInstalledCertificateIds  (Destination, CertificateType, ...)

        /// <summary>
        /// Get the specified installed certificates from the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="CertificateType">The type of the certificates requested.</param>
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
        public static Task<GetInstalledCertificateIdsResponse>

            GetInstalledCertificateIds(this ICentralSystemNode  CentralSystem,
                                       SourceRouting            Destination,
                                       CertificateUse           CertificateType,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTime?                RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.GetInstalledCertificateIds(
                       new GetInstalledCertificateIdsRequest(
                           Destination,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region InstallCertificate          (Destination, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate at the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
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
        public static Task<InstallCertificateResponse>

            InstallCertificate(this ICentralSystemNode  CentralSystem,
                               SourceRouting            Destination,
                               CertificateUse           CertificateType,
                               Certificate              Certificate,

                               IEnumerable<KeyPair>?    SignKeys              = null,
                               IEnumerable<SignInfo>?   SignInfos             = null,
                               IEnumerable<Signature>?  Signatures            = null,

                               CustomData?              CustomData            = null,

                               Request_Id?              RequestId             = null,
                               DateTime?                RequestTimestamp      = null,
                               TimeSpan?                RequestTimeout        = null,
                               EventTracking_Id?        EventTrackingId       = null,
                               SerializationFormats?    SerializationFormat   = null,
                               CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.InstallCertificate(
                       new InstallCertificateRequest(
                           Destination,
                           CertificateType,
                           Certificate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #endregion

        #region Charging

        #region CancelReservation           (Destination, ReservationId, ...)

        /// <summary>
        /// Cancel the given reservation at the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
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
        public static Task<CancelReservationResponse>

            CancelReservation(this ICentralSystemNode  CentralSystem,
                              SourceRouting            Destination,
                              Reservation_Id           ReservationId,

                              IEnumerable<KeyPair>?    SignKeys              = null,
                              IEnumerable<SignInfo>?   SignInfos             = null,
                              IEnumerable<Signature>?  Signatures            = null,

                              CustomData?              CustomData            = null,

                              Request_Id?              RequestId             = null,
                              DateTime?                RequestTimestamp      = null,
                              TimeSpan?                RequestTimeout        = null,
                              EventTracking_Id?        EventTrackingId       = null,
                              SerializationFormats?    SerializationFormat   = null,
                              CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.CancelReservation(
                       new CancelReservationRequest(
                           Destination,
                           ReservationId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearChargingProfile        (Destination, ChargingProfileId = null, ConnectorId = null, ChargingProfilePurpose = null, StackLevel = null, ...)

        /// <summary>
        /// Clear the given charging profile at the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
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
        public static Task<ClearChargingProfileResponse>

            ClearChargingProfile(this ICentralSystemNode   CentralSystem,
                                 SourceRouting             Destination,
                                 ChargingProfile_Id?       ChargingProfileId        = null,
                                 Connector_Id?             ConnectorId              = null,
                                 ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                 UInt32?                   StackLevel               = null,

                                 IEnumerable<KeyPair>?     SignKeys                 = null,
                                 IEnumerable<SignInfo>?    SignInfos                = null,
                                 IEnumerable<Signature>?   Signatures               = null,

                                 CustomData?               CustomData               = null,

                                 Request_Id?               RequestId                = null,
                                 DateTime?                 RequestTimestamp         = null,
                                 TimeSpan?                 RequestTimeout           = null,
                                 EventTracking_Id?         EventTrackingId          = null,
                                 SerializationFormats?     SerializationFormat      = null,
                                 CancellationToken         CancellationToken        = default)

                => CentralSystem.OCPP.OUT.ClearChargingProfile(
                       new ClearChargingProfileRequest(
                           Destination,
                           ChargingProfileId,
                           ConnectorId,
                           ChargingProfilePurpose,
                           StackLevel,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCompositeSchedule        (Destination, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Get the composite schedule of the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
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
        public static Task<GetCompositeScheduleResponse>

            GetCompositeSchedule(this ICentralSystemNode  CentralSystem,
                                 SourceRouting            Destination,
                                 Connector_Id             ConnectorId,
                                 TimeSpan                 Duration,
                                 ChargingRateUnits?       ChargingRateUnit      = null,

                                 IEnumerable<KeyPair>?    SignKeys              = null,
                                 IEnumerable<SignInfo>?   SignInfos             = null,
                                 IEnumerable<Signature>?  Signatures            = null,

                                 CustomData?              CustomData            = null,

                                 Request_Id?              RequestId             = null,
                                 DateTime?                RequestTimestamp      = null,
                                 TimeSpan?                RequestTimeout        = null,
                                 EventTracking_Id?        EventTrackingId       = null,
                                 SerializationFormats?    SerializationFormat   = null,
                                 CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.GetCompositeSchedule(
                       new GetCompositeScheduleRequest(
                           Destination,
                           ConnectorId,
                           Duration,
                           ChargingRateUnit,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region RemoteStartTransaction      (Destination, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Start a charging session at the given charge point/networking node using the given identification tag.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
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
        public static Task<RemoteStartTransactionResponse>

            RemoteStart(this ICentralSystemNode  CentralSystem,
                        SourceRouting            Destination,
                        IdToken                  IdTag,
                        Connector_Id?            ConnectorId           = null,
                        ChargingProfile?         ChargingProfile       = null,

                        IEnumerable<KeyPair>?    SignKeys              = null,
                        IEnumerable<SignInfo>?   SignInfos             = null,
                        IEnumerable<Signature>?  Signatures            = null,

                        CustomData?              CustomData            = null,

                        Request_Id?              RequestId             = null,
                        DateTime?                RequestTimestamp      = null,
                        TimeSpan?                RequestTimeout        = null,
                        EventTracking_Id?        EventTrackingId       = null,
                        SerializationFormats?    SerializationFormat   = null,
                        CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.RemoteStartTransaction(
                       new RemoteStartTransactionRequest(
                           Destination,
                           IdTag,
                           ConnectorId,
                           ChargingProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region RemoteStopTransaction       (Destination, TransactionId, ...)

        /// <summary>
        /// Stop the given charging session at the given charge point/networking node using the given transaction identification.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
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
        public static Task<RemoteStopTransactionResponse>

            RemoteStopTransaction(this ICentralSystemNode  CentralSystem,
                        SourceRouting            Destination,
                        Transaction_Id           TransactionId,

                        IEnumerable<KeyPair>?    SignKeys              = null,
                        IEnumerable<SignInfo>?   SignInfos             = null,
                        IEnumerable<Signature>?  Signatures            = null,

                        CustomData?              CustomData            = null,

                        Request_Id?              RequestId             = null,
                        DateTime?                RequestTimestamp      = null,
                        TimeSpan?                RequestTimeout        = null,
                        EventTracking_Id?        EventTrackingId       = null,
                        SerializationFormats?    SerializationFormat   = null,
                        CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.RemoteStopTransaction(
                       new RemoteStopTransactionRequest(
                           Destination,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region ReserveNow                  (Destination, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Reserve the given connector at the given charge point/networking node using the given identification tag.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The unique token identification for which the reservation is being made.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
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
        public static Task<ReserveNowResponse>

            ReserveNow(this ICentralSystemNode  CentralSystem,
                       SourceRouting            Destination,
                       Connector_Id             ConnectorId,
                       Reservation_Id           ReservationId,
                       DateTime                 ExpiryDate,
                       IdToken                  IdTag,
                       IdToken?                 ParentIdTag           = null,

                       IEnumerable<KeyPair>?    SignKeys              = null,
                       IEnumerable<SignInfo>?   SignInfos             = null,
                       IEnumerable<Signature>?  Signatures            = null,

                       CustomData?              CustomData            = null,

                       Request_Id?              RequestId             = null,
                       DateTime?                RequestTimestamp      = null,
                       TimeSpan?                RequestTimeout        = null,
                       EventTracking_Id?        EventTrackingId       = null,
                       SerializationFormats?    SerializationFormat   = null,
                       CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.ReserveNow(
                       new ReserveNowRequest(
                           Destination,
                           ConnectorId,
                           ReservationId,
                           ExpiryDate,
                           IdTag,
                           ParentIdTag,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetChargingProfile          (Destination, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Set the charging profile for the given connector at the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
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
        public static Task<SetChargingProfileResponse>

            SetChargingProfile(this ICentralSystemNode  CentralSystem,
                               SourceRouting            Destination,
                               Connector_Id             ConnectorId,
                               ChargingProfile          ChargingProfile,

                               IEnumerable<KeyPair>?    SignKeys              = null,
                               IEnumerable<SignInfo>?   SignInfos             = null,
                               IEnumerable<Signature>?  Signatures            = null,

                               CustomData?              CustomData            = null,

                               Request_Id?              RequestId             = null,
                               DateTime?                RequestTimestamp      = null,
                               TimeSpan?                RequestTimeout        = null,
                               EventTracking_Id?        EventTrackingId       = null,
                               SerializationFormats?    SerializationFormat   = null,
                               CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.SetChargingProfile(
                       new SetChargingProfileRequest(
                           Destination,
                           ConnectorId,
                           ChargingProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion


        #endregion



        #region Reset                       (Destination, ResetType, ...)

        /// <summary>
        /// Reset the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
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
        public static Task<ResetResponse>

            Reset(this ICentralSystemNode  CentralSystem,
                  SourceRouting            Destination,
                  ResetType                ResetType,

                  IEnumerable<KeyPair>?    SignKeys              = null,
                  IEnumerable<SignInfo>?   SignInfos             = null,
                  IEnumerable<Signature>?  Signatures            = null,

                  CustomData?              CustomData            = null,

                  Request_Id?              RequestId             = null,
                  DateTime?                RequestTimestamp      = null,
                  TimeSpan?                RequestTimeout        = null,
                  EventTracking_Id?        EventTrackingId       = null,
                  SerializationFormats?    SerializationFormat   = null,
                  CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.Reset(
                       new ResetRequest(
                           Destination,
                           ResetType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeAvailability          (Destination, ConnectorId, Availability, ...)

        /// <summary>
        /// Change the availability of the given charge point.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
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
        public static Task<ChangeAvailabilityResponse>

            ChangeAvailability(this ICentralSystemNode  CentralSystem,
                               SourceRouting            Destination,
                               Connector_Id             ConnectorId,
                               Availabilities           Availability,

                               IEnumerable<KeyPair>?    SignKeys              = null,
                               IEnumerable<SignInfo>?   SignInfos             = null,
                               IEnumerable<Signature>?  Signatures            = null,

                               CustomData?              CustomData            = null,

                               Request_Id?              RequestId             = null,
                               DateTime?                RequestTimestamp      = null,
                               TimeSpan?                RequestTimeout        = null,
                               EventTracking_Id?        EventTrackingId       = null,
                               SerializationFormats?    SerializationFormat   = null,
                               CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           Destination,
                           ConnectorId,
                           Availability,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion


        #region GetConfiguration            (Destination, Keys = null, ...)

        /// <summary>
        /// Get the configuration of the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
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
        public static Task<GetConfigurationResponse>

            GetConfiguration(this ICentralSystemNode  CentralSystem,
                             SourceRouting            Destination,
                             IEnumerable<String>?     Keys                  = null,

                             IEnumerable<KeyPair>?    SignKeys              = null,
                             IEnumerable<SignInfo>?   SignInfos             = null,
                             IEnumerable<Signature>?  Signatures            = null,

                             CustomData?              CustomData            = null,

                             Request_Id?              RequestId             = null,
                             DateTime?                RequestTimestamp      = null,
                             TimeSpan?                RequestTimeout        = null,
                             EventTracking_Id?        EventTrackingId       = null,
                             SerializationFormats?    SerializationFormat   = null,
                             CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.GetConfiguration(
                       new GetConfigurationRequest(
                           Destination,
                           Keys,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeConfiguration         (Destination, Key, Value, ...)

        /// <summary>
        /// Change the configuration of the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
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
        public static Task<ChangeConfigurationResponse>

            ChangeConfiguration(this ICentralSystemNode  CentralSystem,
                                SourceRouting            Destination,
                                String                   Key,
                                String                   Value,

                                IEnumerable<KeyPair>?    SignKeys              = null,
                                IEnumerable<SignInfo>?   SignInfos             = null,
                                IEnumerable<Signature>?  Signatures            = null,

                                CustomData?              CustomData            = null,

                                Request_Id?              RequestId             = null,
                                DateTime?                RequestTimestamp      = null,
                                TimeSpan?                RequestTimeout        = null,
                                EventTracking_Id?        EventTrackingId       = null,
                                SerializationFormats?    SerializationFormat   = null,
                                CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.ChangeConfiguration(
                       new ChangeConfigurationRequest(
                           Destination,
                           Key,
                           Value,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion



        #region TransferData                (Destination, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charge point.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
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
        public static Task<DataTransferResponse>

            TransferData(this ICentralSystemNode  CentralSystem,
                         SourceRouting            Destination,
                         Vendor_Id                VendorId,
                         Message_Id?              MessageId             = null,
                         JToken?                  Data                  = null,

                         IEnumerable<KeyPair>?    SignKeys              = null,
                         IEnumerable<SignInfo>?   SignInfos             = null,
                         IEnumerable<Signature>?  Signatures            = null,

                         Request_Id?              RequestId             = null,
                         DateTime?                RequestTimestamp      = null,
                         TimeSpan?                RequestTimeout        = null,
                         EventTracking_Id?        EventTrackingId       = null,
                         SerializationFormats?    SerializationFormat   = null,
                         CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(
                           Destination,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion


    }

}
