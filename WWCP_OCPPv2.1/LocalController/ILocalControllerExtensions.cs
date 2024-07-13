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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LC
{

    /// <summary>
    /// Extension methods for all local controllers.
    /// </summary>
    public static class ILocalControllerExtensions
    {

        #region as Charging Station

        #region SendBootNotification                  (BootReason, ...)

        /// <summary>
        /// Send a boot notification to the given local controller (default: CSMS).
        /// </summary>
        /// <param name="BootReason">The the reason for sending this boot notification to the CSMS.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.BootNotificationResponse>

            SendBootNotification(this ILocalController         LocalController,

                                 BootReason                    BootReason,

                                 CustomData?                   CustomData           = null,

                                 NetworkingNode_Id?            DestinationNodeId    = null,
                                 NetworkPath?                  NetworkPath          = null,

                                 IEnumerable<KeyPair>?         SignKeys             = null,
                                 IEnumerable<SignInfo>?        SignInfos            = null,
                                 IEnumerable<OCPP.Signature>?  Signatures           = null,

                                 Request_Id?                   RequestId            = null,
                                 DateTime?                     RequestTimestamp     = null,
                                 TimeSpan?                     RequestTimeout       = null,
                                 EventTracking_Id?             EventTrackingId      = null,
                                 CancellationToken             CancellationToken    = default)


                => LocalController.OCPP.OUT.BootNotification(
                       new BootNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           new ChargingStation(
                               LocalController.Model,
                               LocalController.VendorName,
                               LocalController.SerialNumber,
                               LocalController.SoftwareVersion,
                               LocalController.Modem,
                               LocalController.CustomData
                           ),
                           BootReason,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendFirmwareStatusNotification        (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(this ILocalController         LocalController,

                                           FirmwareStatus                Status,
                                           Int64?                        UpdateFirmwareRequestId   = null,

                                           CustomData?                   CustomData                = null,

                                           NetworkingNode_Id?            DestinationNodeId         = null,
                                           NetworkPath?                  NetworkPath               = null,

                                           IEnumerable<KeyPair>?         SignKeys                  = null,
                                           IEnumerable<SignInfo>?        SignInfos                 = null,
                                           IEnumerable<OCPP.Signature>?  Signatures                = null,

                                           Request_Id?                   RequestId                 = null,
                                           DateTime?                     RequestTimestamp          = null,
                                           TimeSpan?                     RequestTimeout            = null,
                                           EventTracking_Id?             EventTrackingId           = null,
                                           CancellationToken             CancellationToken         = default)


                => LocalController.OCPP.OUT.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           Status,
                           UpdateFirmwareRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendPublishFirmwareStatusNotification (Status, PublishFirmwareStatusNotificationRequestId, DownloadLocations, ...)

        /// <summary>
        /// Send a publish firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The progress status of the publish firmware request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(this ILocalController         LocalController,

                                                  PublishFirmwareStatus         Status,
                                                  Int32?                        PublishFirmwareStatusNotificationRequestId,
                                                  IEnumerable<URL>?             DownloadLocations,

                                                  CustomData?                   CustomData          = null,

                                                  NetworkingNode_Id?            DestinationNodeId   = null,
                                                  NetworkPath?                  NetworkPath         = null,

                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                  Request_Id?                   RequestId           = null,
                                                  DateTime?                     RequestTimestamp    = null,
                                                  TimeSpan?                     RequestTimeout      = null,
                                                  EventTracking_Id?             EventTrackingId     = null,
                                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PublishFirmwareStatusNotification(
                       new PublishFirmwareStatusNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           Status,
                           PublishFirmwareStatusNotificationRequestId,
                           DownloadLocations,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendHeartbeat                         (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.HeartbeatResponse>

            SendHeartbeat(this ILocalController         LocalController,

                          CustomData?                   CustomData          = null,

                          NetworkingNode_Id?            DestinationNodeId   = null,
                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.Heartbeat(
                       new HeartbeatRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyEvent                           (GeneratedAt, SequenceNumber, EventData, ToBeContinued = null, ...)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="EventData">The enumeration of event data.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEventResponse>

            NotifyEvent(this ILocalController         LocalController,

                        DateTime                      GeneratedAt,
                        UInt32                        SequenceNumber,
                        IEnumerable<EventData>        EventData,
                        Boolean?                      ToBeContinued       = null,

                        CustomData?                   CustomData          = null,

                        NetworkingNode_Id?            DestinationNodeId   = null,
                        NetworkPath?                  NetworkPath         = null,

                        IEnumerable<KeyPair>?         SignKeys            = null,
                        IEnumerable<SignInfo>?        SignInfos           = null,
                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                        Request_Id?                   RequestId           = null,
                        DateTime?                     RequestTimestamp    = null,
                        TimeSpan?                     RequestTimeout      = null,
                        EventTracking_Id?             EventTrackingId     = null,
                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyEvent(
                       new NotifyEventRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           GeneratedAt,
                           SequenceNumber,
                           EventData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendSecurityEventNotification         (Type, Timestamp, TechInfo = null, TechInfo = null, ...)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.SecurityEventNotificationResponse>

            SendSecurityEventNotification(this ILocalController         LocalController,

                                          SecurityEventType             Type,
                                          DateTime                      Timestamp,
                                          String?                       TechInfo            = null,

                                          CustomData?                   CustomData          = null,

                                          NetworkingNode_Id?            DestinationNodeId   = null,
                                          NetworkPath?                  NetworkPath         = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                          Request_Id?                   RequestId           = null,
                                          DateTime?                     RequestTimestamp    = null,
                                          TimeSpan?                     RequestTimeout      = null,
                                          EventTracking_Id?             EventTrackingId     = null,
                                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SecurityEventNotification(
                       new SecurityEventNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           Type,
                           Timestamp,
                           TechInfo,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyReport                          (SequenceNumber, GeneratedAt, ReportData, ...)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyReportResponse>

            NotifyReport(this ILocalController         LocalController,

                         Int32                         NotifyReportRequestId,
                         UInt32                        SequenceNumber,
                         DateTime                      GeneratedAt,
                         IEnumerable<ReportData>       ReportData,
                         Boolean?                      ToBeContinued       = null,

                         CustomData?                   CustomData          = null,

                         NetworkingNode_Id?            DestinationNodeId   = null,
                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyReport(
                       new NotifyReportRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NotifyReportRequestId,
                           SequenceNumber,
                           GeneratedAt,
                           ReportData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyMonitoringReport                (NotifyMonitoringReportRequestId, SequenceNumber, GeneratedAt, MonitoringData, ToBeContinued = null, ...)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyMonitoringReportResponse>

            NotifyMonitoringReport(this ILocalController         LocalController,

                                   Int32                         NotifyMonitoringReportRequestId,
                                   UInt32                        SequenceNumber,
                                   DateTime                      GeneratedAt,
                                   IEnumerable<MonitoringData>   MonitoringData,
                                   Boolean?                      ToBeContinued       = null,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationNodeId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyMonitoringReport(
                       new NotifyMonitoringReportRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NotifyMonitoringReportRequestId,
                           SequenceNumber,
                           GeneratedAt,
                           MonitoringData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendLogStatusNotification             (Status, LogRequestId = null, ...)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Status">The status of the log upload.</param>
        /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.LogStatusNotificationResponse>

            SendLogStatusNotification(this ILocalController         LocalController,

                                      UploadLogStatus               Status,
                                      Int32?                        LogRequestId        = null,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationNodeId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.LogStatusNotification(
                       new LogStatusNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           Status,
                           LogRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion


        #region SendCertificateSigningRequest         (CSR, SignCertificateRequestId, CertificateType = null, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="SignCertificateRequestId">A sign certificate request identification.</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.SignCertificateResponse>

            SendCertificateSigningRequest(this ILocalController         LocalController,

                                          String                        CSR,
                                          Int32                         SignCertificateRequestId,
                                          CertificateSigningUse?        CertificateType     = null,

                                          CustomData?                   CustomData          = null,

                                          NetworkingNode_Id?            DestinationNodeId   = null,
                                          NetworkPath?                  NetworkPath         = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                          Request_Id?                   RequestId           = null,
                                          DateTime?                     RequestTimestamp    = null,
                                          TimeSpan?                     RequestTimeout      = null,
                                          EventTracking_Id?             EventTrackingId     = null,
                                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SignCertificate(
                       new SignCertificateRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           CSR,
                           SignCertificateRequestId,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region Get15118EVCertificate                 (ISO15118SchemaVersion, CertificateAction, EXIRequest, ...)

        /// <summary>
        /// Get an ISO 15118 contract certificate.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
        /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
        /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
        /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
        /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.Get15118EVCertificateResponse>

            Get15118EVCertificate(this ILocalController         LocalController,

                                  ISO15118SchemaVersion         ISO15118SchemaVersion,
                                  CertificateAction             CertificateAction,
                                  EXIData                       EXIRequest,
                                  UInt32?                       MaximumContractCertificateChains   = 1,
                                  IEnumerable<EMA_Id>?          PrioritizedEMAIds                  = null,

                                  CustomData?                   CustomData                         = null,

                                  NetworkingNode_Id?            DestinationNodeId                  = null,
                                  NetworkPath?                  NetworkPath                        = null,

                                  IEnumerable<KeyPair>?         SignKeys                           = null,
                                  IEnumerable<SignInfo>?        SignInfos                          = null,
                                  IEnumerable<OCPP.Signature>?  Signatures                         = null,

                                  Request_Id?                   RequestId                          = null,
                                  DateTime?                     RequestTimestamp                   = null,
                                  TimeSpan?                     RequestTimeout                     = null,
                                  EventTracking_Id?             EventTrackingId                    = null,
                                  CancellationToken             CancellationToken                  = default)


                => LocalController.OCPP.OUT.Get15118EVCertificate(
                       new Get15118EVCertificateRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ISO15118SchemaVersion,
                           CertificateAction,
                           EXIRequest,
                           MaximumContractCertificateChains,
                           PrioritizedEMAIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region GetCertificateStatus                  (OCSPRequestData, ...)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.GetCertificateStatusResponse>

            GetCertificateStatus(this ILocalController         LocalController,

                                 OCSPRequestData               OCSPRequestData,

                                 CustomData?                   CustomData          = null,

                                 NetworkingNode_Id?            DestinationNodeId   = null,
                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCertificateStatus(
                       new GetCertificateStatusRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           OCSPRequestData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region GetCRLRequest                         (GetCRLRequestId, CertificateHashData, ...)

        /// <summary>
        /// Get a certificate revocation list from CSMS for the specified certificate.
        /// </summary>
        /// 
        /// <param name="GetCRLRequestId">The identification of this request.</param>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.GetCRLResponse>

            GetCRLRequest(this ILocalController         LocalController,

                          UInt32                        GetCRLRequestId,
                          CertificateHashData           CertificateHashData,

                          CustomData?                   CustomData          = null,

                          NetworkingNode_Id?            DestinationNodeId   = null,
                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCRL(
                       new GetCRLRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           GetCRLRequestId,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion


        #region SendReservationStatusUpdate           (ReservationId, ReservationUpdateStatus, ...)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(this ILocalController         LocalController,

                                        Reservation_Id                ReservationId,
                                        ReservationUpdateStatus       ReservationUpdateStatus,

                                        CustomData?                   CustomData          = null,

                                        NetworkingNode_Id?            DestinationNodeId   = null,
                                        NetworkPath?                  NetworkPath         = null,

                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReservationStatusUpdate(
                       new ReservationStatusUpdateRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ReservationId,
                           ReservationUpdateStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region Authorize                             (IdToken, Certificate = null, ISO15118CertificateHashData = null, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.AuthorizeResponse>

            Authorize(this ILocalController          LocalController,

                      IdToken                        IdToken,
                      Certificate?                   Certificate                   = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                      CustomData?                    CustomData                    = null,

                      NetworkingNode_Id?             DestinationNodeId             = null,
                      NetworkPath?                   NetworkPath                   = null,

                      IEnumerable<KeyPair>?          SignKeys                      = null,
                      IEnumerable<SignInfo>?         SignInfos                     = null,
                      IEnumerable<OCPP.Signature>?   Signatures                    = null,

                      Request_Id?                    RequestId                     = null,
                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      CancellationToken              CancellationToken             = default)


                => LocalController.OCPP.OUT.Authorize(
                       new AuthorizeRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           IdToken,
                           Certificate,
                           ISO15118CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyEVChargingNeeds                 (EVSEId, ChargingNeeds, ReceivedTimestamp = null, MaxScheduleTuples = null, ...)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
        /// <param name="MaxScheduleTuples">The optional maximum number of schedule tuples per schedule the car supports.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(this ILocalController         LocalController,

                                  EVSE_Id                       EVSEId,
                                  ChargingNeeds                 ChargingNeeds,
                                  DateTime?                     ReceivedTimestamp   = null,
                                  UInt16?                       MaxScheduleTuples   = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationNodeId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyEVChargingNeeds(
                       new NotifyEVChargingNeedsRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           EVSEId,
                           ChargingNeeds,
                           ReceivedTimestamp,
                           MaxScheduleTuples,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendTransactionEvent                  (EventType, Timestamp, TriggerReason, SequenceNumber, TransactionInfo, ...)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
        /// <param name="TransactionInfo">Transaction related information.</param>
        /// 
        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
        /// <param name="MeterValues">An optional enumeration of meter values.</param>
        /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.TransactionEventResponse>

            SendTransactionEvent(this ILocalController         LocalController,

                                 TransactionEvents             EventType,
                                 DateTime                      Timestamp,
                                 TriggerReason                 TriggerReason,
                                 UInt32                        SequenceNumber,
                                 Transaction                   TransactionInfo,

                                 Boolean?                      Offline                 = null,
                                 Byte?                         NumberOfPhasesUsed      = null,
                                 Ampere?                       CableMaxCurrent         = null,
                                 Reservation_Id?               ReservationId           = null,
                                 IdToken?                      IdToken                 = null,
                                 EVSE?                         EVSE                    = null,
                                 IEnumerable<MeterValue>?      MeterValues             = null,
                                 PreconditioningStatus?        PreconditioningStatus   = null,

                                 CustomData?                   CustomData              = null,

                                 NetworkingNode_Id?            DestinationNodeId       = null,
                                 NetworkPath?                  NetworkPath             = null,

                                 IEnumerable<KeyPair>?         SignKeys                = null,
                                 IEnumerable<SignInfo>?        SignInfos               = null,
                                 IEnumerable<OCPP.Signature>?  Signatures              = null,

                                 Request_Id?                   RequestId               = null,
                                 DateTime?                     RequestTimestamp        = null,
                                 TimeSpan?                     RequestTimeout          = null,
                                 EventTracking_Id?             EventTrackingId         = null,
                                 CancellationToken             CancellationToken       = default)


                => LocalController.OCPP.OUT.TransactionEvent(
                       new TransactionEventRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           EventType,
                           Timestamp,
                           TriggerReason,
                           SequenceNumber,
                           TransactionInfo,

                           Offline,
                           NumberOfPhasesUsed,
                           CableMaxCurrent,
                           ReservationId,
                           IdToken,
                           EVSE,
                           MeterValues,
                           PreconditioningStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendStatusNotification                (EVSEId, ConnectorId, Timestamp, Status, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="Status">The current status of the connector.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.StatusNotificationResponse>

            SendStatusNotification(this ILocalController         LocalController,

                                   EVSE_Id                       EVSEId,
                                   Connector_Id                  ConnectorId,
                                   DateTime                      Timestamp,
                                   ConnectorStatus               Status,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationNodeId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.StatusNotification(
                       new StatusNotificationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           Timestamp,
                           Status,
                           EVSEId,
                           ConnectorId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendMeterValues                       (EVSEId, MeterValues, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="EVSEId">The EVSE identification at the charging station.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.MeterValuesResponse>

            SendMeterValues(this ILocalController         LocalController,

                            EVSE_Id                       EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>       MeterValues,

                            CustomData?                   CustomData          = null,

                            NetworkingNode_Id?            DestinationNodeId   = null,
                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.MeterValues(
                       new MeterValuesRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           EVSEId,
                           MeterValues,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyChargingLimit                   (ChargingLimit, ChargingSchedules, EVSEId = null, ...)

        /// <summary>
        /// Notify about a charging limit.
        /// </summary>
        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
        /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyChargingLimitResponse>

            NotifyChargingLimit(this ILocalController          LocalController,

                                ChargingLimit                  ChargingLimit,
                                IEnumerable<ChargingSchedule>  ChargingSchedules,
                                EVSE_Id?                       EVSEId              = null,

                                CustomData?                    CustomData          = null,

                                NetworkingNode_Id?             DestinationNodeId   = null,
                                NetworkPath?                   NetworkPath         = null,

                                IEnumerable<KeyPair>?          SignKeys            = null,
                                IEnumerable<SignInfo>?         SignInfos           = null,
                                IEnumerable<OCPP.Signature>?   Signatures          = null,

                                Request_Id?                    RequestId           = null,
                                DateTime?                      RequestTimestamp    = null,
                                TimeSpan?                      RequestTimeout      = null,
                                EventTracking_Id?              EventTrackingId     = null,
                                CancellationToken              CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyChargingLimit(
                       new NotifyChargingLimitRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ChargingLimit,
                           ChargingSchedules,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region SendClearedChargingLimit              (ChargingLimitSource, EVSEId, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ClearedChargingLimitResponse>

            SendClearedChargingLimit(this ILocalController         LocalController,

                                     ChargingLimitSource           ChargingLimitSource,
                                     EVSE_Id?                      EVSEId,

                                     CustomData?                   CustomData          = null,

                                     NetworkingNode_Id?            DestinationNodeId   = null,
                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearedChargingLimit(
                       new ClearedChargingLimitRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ChargingLimitSource,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region ReportChargingProfiles                (ReportChargingProfilesRequestId, ChargingLimitSource, EVSEId, ChargingProfiles, ToBeContinued = null, ...)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ReportChargingProfilesResponse>

            ReportChargingProfiles(this ILocalController         LocalController,

                                   Int32                         ReportChargingProfilesRequestId,
                                   ChargingLimitSource           ChargingLimitSource,
                                   EVSE_Id                       EVSEId,
                                   IEnumerable<ChargingProfile>  ChargingProfiles,
                                   Boolean?                      ToBeContinued       = null,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationNodeId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReportChargingProfiles(
                       new ReportChargingProfilesRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ReportChargingProfilesRequestId,
                           ChargingLimitSource,
                           EVSEId,
                           ChargingProfiles,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyEVChargingSchedule              (TimeBase, EVSEId, ChargingSchedule, SelectedScheduleTupleId = null, PowerToleranceAcceptance = null, ...)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
        /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
        /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEVChargingScheduleResponse>

            NotifyEVChargingSchedule(this ILocalController         LocalController,

                                     DateTime                      TimeBase,
                                     EVSE_Id                       EVSEId,
                                     ChargingSchedule              ChargingSchedule,
                                     Byte?                         SelectedScheduleTupleId    = null,
                                     Boolean?                      PowerToleranceAcceptance   = null,

                                     CustomData?                   CustomData                 = null,

                                     NetworkingNode_Id?            DestinationNodeId          = null,
                                     NetworkPath?                  NetworkPath                = null,

                                     IEnumerable<KeyPair>?         SignKeys                   = null,
                                     IEnumerable<SignInfo>?        SignInfos                  = null,
                                     IEnumerable<OCPP.Signature>?  Signatures                 = null,

                                     Request_Id?                   RequestId                  = null,
                                     DateTime?                     RequestTimestamp           = null,
                                     TimeSpan?                     RequestTimeout             = null,
                                     EventTracking_Id?             EventTrackingId            = null,
                                     CancellationToken             CancellationToken          = default)


                => LocalController.OCPP.OUT.NotifyEVChargingSchedule(
                       new NotifyEVChargingScheduleRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           TimeBase,
                           EVSEId,
                           ChargingSchedule,
                           SelectedScheduleTupleId,
                           PowerToleranceAcceptance,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyPriorityCharging                (TransactionId, Activated, ...)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyPriorityChargingResponse>

            NotifyPriorityCharging(this ILocalController         LocalController,

                                   Transaction_Id                TransactionId,
                                   Boolean                       Activated,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationNodeId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyPriorityCharging(
                       new NotifyPriorityChargingRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           TransactionId,
                           Activated,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region PullDynamicScheduleUpdate             (ChargingProfileId, ...)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.PullDynamicScheduleUpdateResponse>

            PullDynamicScheduleUpdate(this ILocalController         LocalController,

                                      ChargingProfile_Id            ChargingProfileId,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationNodeId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PullDynamicScheduleUpdate(
                       new PullDynamicScheduleUpdateRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           ChargingProfileId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion


        #region NotifyDisplayMessages                 (NotifyDisplayMessagesRequestId, MessageInfos, ToBeContinued, ...)

        /// <summary>
        /// NotifyDisplayMessages the given token.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the notify display messages request.</param>
        /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(this ILocalController         LocalController,

                                  Int32                         NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>      MessageInfos,
                                  Boolean?                      ToBeContinued       = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationNodeId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyDisplayMessages(
                       new NotifyDisplayMessagesRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NotifyDisplayMessagesRequestId,
                           MessageInfos,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyCustomerInformation             (NotifyCustomerInformationRequestId, Data, SequenceNumber, GeneratedAt, ToBeContinued = null, ...)

        /// <summary>
        /// NotifyCustomerInformation the given token.
        /// </summary>
        /// <param name="NotifyCustomerInformationRequestId">The unique identification of the notify customer information request.</param>
        /// <param name="Data">The requested data or a part of the requested data. No format specified in which the data is returned.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyCustomerInformationResponse>

            NotifyCustomerInformation(this ILocalController         LocalController,

                                      Int64                         NotifyCustomerInformationRequestId,
                                      String                        Data,
                                      UInt32                        SequenceNumber,
                                      DateTime                      GeneratedAt,
                                      Boolean?                      ToBeContinued       = null,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationNodeId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyCustomerInformation(
                       new NotifyCustomerInformationRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NotifyCustomerInformationRequestId,
                           Data,
                           SequenceNumber,
                           GeneratedAt,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #endregion

        #region as CSMS

        #region Reset                       (DestinationNodeId, ResetType, EVSEId = null, ...)

        /// <summary>
        /// Reset the given charging station/local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The charging station/local controller identification.</param>
        /// <param name="ResetType">The type of reset that the charging station should perform.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ResetResponse>

            Reset(this ILocalController         LocalController,
                  NetworkingNode_Id             DestinationNodeId,
                  ResetType                     ResetType,
                  EVSE_Id?                      EVSEId              = null,

                  CustomData?                   CustomData          = null,

                  NetworkPath?                  NetworkPath         = null,

                  IEnumerable<KeyPair>?         SignKeys            = null,
                  IEnumerable<SignInfo>?        SignInfos           = null,
                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                  Request_Id?                   RequestId           = null,
                  DateTime?                     RequestTimestamp    = null,
                  TimeSpan?                     RequestTimeout      = null,
                  EventTracking_Id?             EventTrackingId     = null,
                  CancellationToken             CancellationToken   = default)

                => LocalController.OCPP.OUT.Reset(
                       new ResetRequest(
                           DestinationNodeId,
                           ResetType,
                           EVSEId,

                           null,
                           null,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateFirmware              (DestinationNodeId, Firmware, UpdateFirmwareRequestId, ...)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="Firmware">The firmware image to be installed at the charging station.</param>
        /// <param name="UpdateFirmwareRequestId">The update firmware request identification.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.UpdateFirmwareResponse>

            UpdateFirmware(this ILocalController         LocalController,
                           NetworkingNode_Id             DestinationNodeId,
                           Firmware                      Firmware,
                           Int32                         UpdateFirmwareRequestId,
                           Byte?                         Retries             = null,
                           TimeSpan?                     RetryInterval       = null,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UpdateFirmware(
                       new UpdateFirmwareRequest(
                           DestinationNodeId,
                           Firmware,
                           UpdateFirmwareRequestId,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region PublishFirmware             (DestinationNodeId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="PublishFirmwareRequestId">The unique identification of this publish firmware request</param>
        /// <param name="DownloadLocation">An URL for downloading the firmware.onto the local controller.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
        /// <param name="Retries">The optional number of retries of a charging station for trying to download the firmware before giving up. If this field is not present, it is left to the charging station to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charging station to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.PublishFirmwareResponse>

            PublishFirmware(this ILocalController         LocalController,
                            NetworkingNode_Id             DestinationNodeId,
                            Int32                         PublishFirmwareRequestId,
                            URL                           DownloadLocation,
                            String                        MD5Checksum,
                            Byte?                         Retries             = null,
                            TimeSpan?                     RetryInterval       = null,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PublishFirmware(
                       new PublishFirmwareRequest(
                           DestinationNodeId,
                           PublishFirmwareRequestId,
                           DownloadLocation,
                           MD5Checksum,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UnpublishFirmware           (DestinationNodeId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.UnpublishFirmwareResponse>

            UnpublishFirmware(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              String                        MD5Checksum,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UnpublishFirmware(
                       new UnpublishFirmwareRequest(
                           DestinationNodeId,
                           MD5Checksum,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetBaseReport               (DestinationNodeId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="GetBaseReportRequestId">An unique identification of the get base report request.</param>
        /// <param name="ReportBase">The requested reporting base.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetBaseReportResponse>

            GetBaseReport(this ILocalController         LocalController,
                          NetworkingNode_Id             DestinationNodeId,
                          Int64                         GetBaseReportRequestId,
                          ReportBase                    ReportBase,

                          CustomData?                   CustomData          = null,

                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetBaseReport(
                       new GetBaseReportRequest(
                           DestinationNodeId,
                           GetBaseReportRequestId,
                           ReportBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetReport                   (DestinationNodeId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="GetReportRequestId">The local controller identification.</param>
        /// <param name="ComponentCriteria">An optional enumeration of criteria for components for which a report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a report is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetReportResponse>

            GetReport(this ILocalController           LocalController,
                      NetworkingNode_Id               DestinationNodeId,
                      Int32                           GetReportRequestId,
                      IEnumerable<ComponentCriteria>  ComponentCriteria,
                      IEnumerable<ComponentVariable>  ComponentVariables,

                      CustomData?                     CustomData          = null,

                      NetworkPath?                    NetworkPath         = null,

                      IEnumerable<KeyPair>?           SignKeys            = null,
                      IEnumerable<SignInfo>?          SignInfos           = null,
                      IEnumerable<OCPP.Signature>?    Signatures          = null,

                      Request_Id?                     RequestId           = null,
                      DateTime?                       RequestTimestamp    = null,
                      TimeSpan?                       RequestTimeout      = null,
                      EventTracking_Id?               EventTrackingId     = null,
                      CancellationToken               CancellationToken   = default)


                => LocalController.OCPP.OUT.GetReport(
                       new GetReportRequest(
                           DestinationNodeId,
                           GetReportRequestId,
                           ComponentCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetLog                      (DestinationNodeId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="LogType">The type of the certificates requested.</param>
        /// <param name="LogRequestId">The unique identification of this request.</param>
        /// <param name="Log">This field specifies the requested log and the location to which the log should be sent.</param>
        /// <param name="Retries">This specifies how many times the Charge Point must try to upload the log before giving up. If this field is not present, it is left to Charge Point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to Charge Point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetLogResponse>

            GetLog(this ILocalController         LocalController,
                   NetworkingNode_Id             DestinationNodeId,
                   LogType                       LogType,
                   Int32                         LogRequestId,
                   LogParameters                 Log,
                   Byte?                         Retries             = null,
                   TimeSpan?                     RetryInterval       = null,

                   CustomData?                   CustomData          = null,

                   NetworkPath?                  NetworkPath         = null,

                   IEnumerable<KeyPair>?         SignKeys            = null,
                   IEnumerable<SignInfo>?        SignInfos           = null,
                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                   Request_Id?                   RequestId           = null,
                   DateTime?                     RequestTimestamp    = null,
                   TimeSpan?                     RequestTimeout      = null,
                   EventTracking_Id?             EventTrackingId     = null,
                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetLog(
                       new GetLogRequest(
                           DestinationNodeId,
                           LogType,
                           LogRequestId,
                           Log,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SetVariables                (DestinationNodeId, VariableData, DataConsistencyModel = null, ...)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="VariableData">An enumeration of variable data to set/change.</param>
        /// <param name="DataConsistencyModel">An optional data consistency model for this request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetVariablesResponse>

            SetVariables(this ILocalController         LocalController,
                         NetworkingNode_Id             DestinationNodeId,
                         IEnumerable<SetVariableData>  VariableData,
                         DataConsistencyModel?         DataConsistencyModel   = null,

                         CustomData?                   CustomData             = null,

                         NetworkPath?                  NetworkPath            = null,

                         IEnumerable<KeyPair>?         SignKeys               = null,
                         IEnumerable<SignInfo>?        SignInfos              = null,
                         IEnumerable<OCPP.Signature>?  Signatures             = null,

                         Request_Id?                   RequestId              = null,
                         DateTime?                     RequestTimestamp       = null,
                         TimeSpan?                     RequestTimeout         = null,
                         EventTracking_Id?             EventTrackingId        = null,
                         CancellationToken             CancellationToken      = default)


                => LocalController.OCPP.OUT.SetVariables(
                       new SetVariablesRequest(
                           DestinationNodeId,
                           VariableData,
                           DataConsistencyModel,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetVariables                (DestinationNodeId, VariableData, ...)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="VariableData">An enumeration of requested variable data sets.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetVariablesResponse>

            GetVariables(this ILocalController         LocalController,
                         NetworkingNode_Id             DestinationNodeId,
                         IEnumerable<GetVariableData>  VariableData,

                         CustomData?                   CustomData          = null,

                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetVariables(
                       new GetVariablesRequest(
                           DestinationNodeId,
                           VariableData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringBase           (DestinationNodeId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="MonitoringBase">The monitoring base to be set.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetMonitoringBaseResponse>

            SetMonitoringBase(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              MonitoringBase                MonitoringBase,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetMonitoringBase(
                       new SetMonitoringBaseRequest(
                           DestinationNodeId,
                           MonitoringBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetMonitoringReport         (DestinationNodeId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="GetMonitoringReportRequestId">The local controller identification.</param>
        /// <param name="MonitoringCriteria">An optional enumeration of criteria for components for which a monitoring report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a monitoring report is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetMonitoringReportResponse>

            GetMonitoringReport(this ILocalController             LocalController,
                                NetworkingNode_Id                 DestinationNodeId,
                                Int32                             GetMonitoringReportRequestId,
                                IEnumerable<MonitoringCriterion>  MonitoringCriteria,
                                IEnumerable<ComponentVariable>    ComponentVariables,

                                CustomData?                       CustomData          = null,

                                NetworkPath?                      NetworkPath         = null,

                                IEnumerable<KeyPair>?             SignKeys            = null,
                                IEnumerable<SignInfo>?            SignInfos           = null,
                                IEnumerable<OCPP.Signature>?      Signatures          = null,

                                Request_Id?                       RequestId           = null,
                                DateTime?                         RequestTimestamp    = null,
                                TimeSpan?                         RequestTimeout      = null,
                                EventTracking_Id?                 EventTrackingId     = null,
                                CancellationToken                 CancellationToken   = default)


                => LocalController.OCPP.OUT.GetMonitoringReport(
                       new GetMonitoringReportRequest(
                           DestinationNodeId,
                           GetMonitoringReportRequestId,
                           MonitoringCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringLevel          (DestinationNodeId, Severity, ...)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="Severity">The charging station SHALL only report events with a severity number lower than or equal to this severity.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetMonitoringLevelResponse>

            SetMonitoringLevel(this ILocalController         LocalController,
                               NetworkingNode_Id             DestinationNodeId,
                               Severities                    Severity,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetMonitoringLevel(
                       new SetMonitoringLevelRequest(
                           DestinationNodeId,
                           Severity,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetVariableMonitoring       (DestinationNodeId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="MonitoringData">An enumeration of monitoring data.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetVariableMonitoringResponse>

            SetVariableMonitoring(this ILocalController           LocalController,
                                  NetworkingNode_Id               DestinationNodeId,
                                  IEnumerable<SetMonitoringData>  MonitoringData,

                                  CustomData?                     CustomData          = null,

                                  NetworkPath?                    NetworkPath         = null,

                                  IEnumerable<KeyPair>?           SignKeys            = null,
                                  IEnumerable<SignInfo>?          SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?    Signatures          = null,

                                  Request_Id?                     RequestId           = null,
                                  DateTime?                       RequestTimestamp    = null,
                                  TimeSpan?                       RequestTimeout      = null,
                                  EventTracking_Id?               EventTrackingId     = null,
                                  CancellationToken               CancellationToken   = default)


                => LocalController.OCPP.OUT.SetVariableMonitoring(
                       new SetVariableMonitoringRequest(
                           DestinationNodeId,
                           MonitoringData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearVariableMonitoring     (DestinationNodeId, VariableMonitoringIds, ...)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="VariableMonitoringIds">An enumeration of variable monitoring identifications to clear.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ClearVariableMonitoringResponse>

            ClearVariableMonitoring(this ILocalController               LocalController,
                                    NetworkingNode_Id                   DestinationNodeId,
                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

                                    CustomData?                         CustomData          = null,

                                    NetworkPath?                        NetworkPath         = null,

                                    IEnumerable<KeyPair>?               SignKeys            = null,
                                    IEnumerable<SignInfo>?              SignInfos           = null,
                                    IEnumerable<OCPP.Signature>?        Signatures          = null,

                                    Request_Id?                         RequestId           = null,
                                    DateTime?                           RequestTimestamp    = null,
                                    TimeSpan?                           RequestTimeout      = null,
                                    EventTracking_Id?                   EventTrackingId     = null,
                                    CancellationToken                   CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearVariableMonitoring(
                       new ClearVariableMonitoringRequest(
                           DestinationNodeId,
                           VariableMonitoringIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetNetworkProfile           (DestinationNodeId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ConfigurationSlot">The slot in which the configuration should be stored.</param>
        /// <param name="NetworkConnectionProfile">The network connection configuration.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetNetworkProfileResponse>

            SetNetworkProfile(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              Int32                         ConfigurationSlot,
                              NetworkConnectionProfile      NetworkConnectionProfile,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetNetworkProfile(
                       new SetNetworkProfileRequest(
                           DestinationNodeId,
                           ConfigurationSlot,
                           NetworkConnectionProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeAvailability          (DestinationNodeId, OperationalStatus, EVSE = null, ...)

        /// <summary>
        /// Change the availability of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="OperationalStatus">A new operational status of the charging station or EVSE.</param>
        /// 
        /// <param name="EVSE">Optional identification of an EVSE/connector for which the operational status should be changed.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ChangeAvailabilityResponse>

            ChangeAvailability(this ILocalController         LocalController,
                               NetworkingNode_Id             DestinationNodeId,
                               OperationalStatus             OperationalStatus,

                               EVSE?                         EVSE                = null,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           DestinationNodeId,
                           OperationalStatus,
                           EVSE,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region TriggerMessage              (DestinationNodeId, RequestedMessage, EVSEId = null, CustomTrigger = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="EVSE">An optional EVSE (and connector) identification whenever the message applies to a specific EVSE and/or connector.</param>
        /// <param name="CustomTrigger">An optional custom trigger.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.TriggerMessageResponse>

            TriggerMessage(this ILocalController         LocalController,
                           NetworkingNode_Id             DestinationNodeId,
                           MessageTrigger                RequestedMessage,
                           EVSE?                         EVSE                = null,
                           String?                       CustomTrigger       = null,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.TriggerMessage(
                       new TriggerMessageRequest(
                           DestinationNodeId,
                           RequestedMessage,
                           EVSE,
                           CustomTrigger,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SendSignedCertificate       (DestinationNodeId, CertificateChain, CertificateType = null, ...)

        /// <summary>
        /// Send the signed certificate to the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.CertificateSignedResponse>

            SendSignedCertificate(this ILocalController         LocalController,
                                  NetworkingNode_Id             DestinationNodeId,
                                  CertificateChain              CertificateChain,
                                  CertificateSigningUse?        CertificateType     = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CertificateSigned(
                       new CertificateSignedRequest(
                           DestinationNodeId,
                           CertificateChain,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region InstallCertificate          (DestinationNodeId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.InstallCertificateResponse>

            InstallCertificate(this ILocalController         LocalController,
                               NetworkingNode_Id             DestinationNodeId,
                               InstallCertificateUse         CertificateType,
                               Certificate                   Certificate,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.InstallCertificate(
                       new InstallCertificateRequest(
                           DestinationNodeId,
                           CertificateType,
                           Certificate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetInstalledCertificateIds  (DestinationNodeId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="CertificateTypes">An optional enumeration of certificate types requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetInstalledCertificateIdsResponse>

            GetInstalledCertificateIds(this ILocalController              LocalController,
                                       NetworkingNode_Id                  DestinationNodeId,
                                       IEnumerable<GetCertificateIdUse>?  CertificateTypes    = null,

                                       CustomData?                        CustomData          = null,

                                       NetworkPath?                       NetworkPath         = null,

                                       IEnumerable<KeyPair>?              SignKeys            = null,
                                       IEnumerable<SignInfo>?             SignInfos           = null,
                                       IEnumerable<OCPP.Signature>?       Signatures          = null,

                                       Request_Id?                        RequestId           = null,
                                       DateTime?                          RequestTimestamp    = null,
                                       TimeSpan?                          RequestTimeout      = null,
                                       EventTracking_Id?                  EventTrackingId     = null,
                                       CancellationToken                  CancellationToken   = default)


                => LocalController.OCPP.OUT.GetInstalledCertificateIds(
                       new GetInstalledCertificateIdsRequest(
                           DestinationNodeId,
                           CertificateTypes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteCertificate           (DestinationNodeId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.DeleteCertificateResponse>

            DeleteCertificate(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              CertificateHashData           CertificateHashData,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.DeleteCertificate(
                       new DeleteCertificateRequest(
                           DestinationNodeId,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyCRLAvailability       (DestinationNodeId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="NotifyCRLRequestId">An unique identification of this request.</param>
        /// <param name="Availability">An availability status of the certificate revocation list.</param>
        /// <param name="Location">An optional location of the certificate revocation list.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.NotifyCRLResponse>

            NotifyCRLAvailability(this ILocalController         LocalController,
                                  NetworkingNode_Id             DestinationNodeId,
                                  Int32                         NotifyCRLRequestId,
                                  NotifyCRLStatus               Availability,
                                  URL?                          Location,

                                  CustomData?                   CustomData          = null,

                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyCRL(
                       new NotifyCRLRequest(
                           DestinationNodeId,
                           NotifyCRLRequestId,
                           Availability,
                           Location,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region GetLocalListVersion         (DestinationNodeId, ...)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetLocalListVersionResponse>

            GetLocalListVersion(this ILocalController         LocalController,
                                NetworkingNode_Id             DestinationNodeId,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetLocalListVersion(
                       new GetLocalListVersionRequest(
                           DestinationNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendLocalList               (DestinationNodeId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charging station. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SendLocalListResponse>

            SendLocalList(this ILocalController            LocalController,
                          NetworkingNode_Id                DestinationNodeId,
                          UInt64                           ListVersion,
                          UpdateTypes                      UpdateType,
                          IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                          CustomData?                      CustomData               = null,

                          NetworkPath?                     NetworkPath              = null,

                          IEnumerable<KeyPair>?            SignKeys                 = null,
                          IEnumerable<SignInfo>?           SignInfos                = null,
                          IEnumerable<OCPP.Signature>?     Signatures               = null,

                          Request_Id?                      RequestId                = null,
                          DateTime?                        RequestTimestamp         = null,
                          TimeSpan?                        RequestTimeout           = null,
                          EventTracking_Id?                EventTrackingId          = null,
                          CancellationToken                CancellationToken        = default)


                => LocalController.OCPP.OUT.SendLocalList(
                       new SendLocalListRequest(
                           DestinationNodeId,
                           ListVersion,
                           UpdateType,
                           LocalAuthorizationList,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearCache                  (DestinationNodeId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ClearCacheResponse>

            ClearCache(this ILocalController         LocalController,
                       NetworkingNode_Id             DestinationNodeId,

                       CustomData?                   CustomData          = null,

                       NetworkPath?                  NetworkPath         = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearCache(
                       new ClearCacheRequest(
                           DestinationNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region ReserveNow                  (DestinationNodeId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdToken">The identifier for which the charging station has to reserve a connector.</param>
        /// <param name="ConnectorType">An optional connector type to be reserved..</param>
        /// <param name="EVSEId">The identification of the EVSE to be reserved. A value of 0 means that the reservation is not for a specific EVSE.</param>
        /// <param name="GroupIdToken">An optional group identifier for which the reservation is being made.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ReserveNowResponse>

            ReserveNow(this ILocalController         LocalController,
                       NetworkingNode_Id             DestinationNodeId,
                       Reservation_Id                ReservationId,
                       DateTime                      ExpiryDate,
                       IdToken                       IdToken,
                       ConnectorType?                ConnectorType       = null,
                       EVSE_Id?                      EVSEId              = null,
                       IdToken?                      GroupIdToken        = null,

                       CustomData?                   CustomData          = null,

                       NetworkPath?                  NetworkPath         = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReserveNow(
                       new ReserveNowRequest(
                           DestinationNodeId,
                           ReservationId,
                           ExpiryDate,
                           IdToken,
                           ConnectorType,
                           EVSEId,
                           GroupIdToken,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region CancelReservation           (DestinationNodeId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.CancelReservationResponse>

            CancelReservation(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              Reservation_Id                ReservationId,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CancelReservation(
                       new CancelReservationRequest(
                           DestinationNodeId,
                           ReservationId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region StartCharging               (DestinationNodeId, RequestStartTransactionRequestId, IdToken, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="RequestStartTransactionRequestId">Request identification given by the server to this start request. The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.</param>
        /// <param name="IdToken">The identification token to start the charging transaction.</param>
        /// <param name="EVSEId">An optional EVSE identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charging station for the requested charging transaction.</param>
        /// <param name="GroupIdToken">An optional group identifier.</param>
        /// <param name="TransactionLimits">Optional maximum cost, energy, or time allowed for this transaction.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.RequestStartTransactionResponse>

            StartCharging(this ILocalController         LocalController,
                          NetworkingNode_Id             DestinationNodeId,
                          RemoteStart_Id                RequestStartTransactionRequestId,
                          IdToken                       IdToken,
                          EVSE_Id?                      EVSEId              = null,
                          ChargingProfile?              ChargingProfile     = null,
                          IdToken?                      GroupIdToken        = null,
                          TransactionLimits?            TransactionLimits   = null,

                          CustomData?                   CustomData          = null,

                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RequestStartTransaction(
                       new RequestStartTransactionRequest(
                           DestinationNodeId,
                           RequestStartTransactionRequestId,
                           IdToken,
                           EVSEId,
                           ChargingProfile,
                           GroupIdToken,
                           TransactionLimits,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region StopCharging                (DestinationNodeId, TransactionId, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="TransactionId">An optional transaction identification for which its status is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.RequestStopTransactionResponse>

            StopCharging(this ILocalController         LocalController,
                         NetworkingNode_Id             DestinationNodeId,
                         Transaction_Id                TransactionId,

                         CustomData?                   CustomData          = null,

                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RequestStopTransaction(
                       new RequestStopTransactionRequest(
                           DestinationNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetTransactionStatus        (DestinationNodeId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="TransactionId">An optional transaction identification for which its status is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetTransactionStatusResponse>

            GetTransactionStatus(this ILocalController         LocalController,
                                 NetworkingNode_Id             DestinationNodeId,
                                 Transaction_Id?               TransactionId       = null,

                                 CustomData?                   CustomData          = null,

                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetTransactionStatus(
                       new GetTransactionStatusRequest(
                           DestinationNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetChargingProfile          (DestinationNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="EVSEId">The EVSE identification to which the charging profile applies.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetChargingProfileResponse>

            SetChargingProfile(this ILocalController         LocalController,
                               NetworkingNode_Id             DestinationNodeId,
                               EVSE_Id                       EVSEId,
                               ChargingProfile               ChargingProfile,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetChargingProfile(
                       new SetChargingProfileRequest(
                           DestinationNodeId,
                           EVSEId,
                           ChargingProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetChargingProfiles         (DestinationNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="EVSEId">The EVSE identification to which the charging profile applies.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetChargingProfilesResponse>

            GetChargingProfiles(this ILocalController         LocalController,
                                NetworkingNode_Id             DestinationNodeId,
                                Int64                         GetChargingProfilesRequestId,
                                ChargingProfileCriterion      ChargingProfile,
                                EVSE_Id?                      EVSEId              = null,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetChargingProfiles(
                       new GetChargingProfilesRequest(
                           DestinationNodeId,
                           GetChargingProfilesRequestId,
                           ChargingProfile,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearChargingProfile        (DestinationNodeId, ChargingProfileId, ChargingProfileCriteria, ...)

        /// <summary>
        /// Remove the charging profile at the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ChargingProfileId">An optional identification of the charging profile to clear.</param>
        /// <param name="ChargingProfileCriteria">An optional specification of the charging profile to clear.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ClearChargingProfileResponse>

            ClearChargingProfile(this ILocalController         LocalController,
                                 NetworkingNode_Id             DestinationNodeId,
                                 ChargingProfile_Id?           ChargingProfileId         = null,
                                 ClearChargingProfile?         ChargingProfileCriteria   = null,

                                 CustomData?                   CustomData                = null,

                                 NetworkPath?                  NetworkPath               = null,

                                 IEnumerable<KeyPair>?         SignKeys                  = null,
                                 IEnumerable<SignInfo>?        SignInfos                 = null,
                                 IEnumerable<OCPP.Signature>?  Signatures                = null,

                                 Request_Id?                   RequestId                 = null,
                                 DateTime?                     RequestTimestamp          = null,
                                 TimeSpan?                     RequestTimeout            = null,
                                 EventTracking_Id?             EventTrackingId           = null,
                                 CancellationToken             CancellationToken         = default)


                => LocalController.OCPP.OUT.ClearChargingProfile(
                       new ClearChargingProfileRequest(
                           DestinationNodeId,
                           ChargingProfileId,
                           ChargingProfileCriteria,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCompositeSchedule        (DestinationNodeId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="EVSEId">The EVSE identification for which the schedule is requested. EVSE identification is 0, the charging station will calculate the expected consumption for the grid connection.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetCompositeScheduleResponse>

            GetCompositeSchedule(this ILocalController         LocalController,
                                 NetworkingNode_Id             DestinationNodeId,
                                 TimeSpan                      Duration,
                                 EVSE_Id                       EVSEId,
                                 ChargingRateUnits?            ChargingRateUnit    = null,

                                 CustomData?                   CustomData          = null,

                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCompositeSchedule(
                       new GetCompositeScheduleRequest(
                           DestinationNodeId,
                           Duration,
                           EVSEId,
                           ChargingRateUnit,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateDynamicSchedule       (DestinationNodeId, ChargingProfileId, Limit = null, ...)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile to update.</param>
        /// 
        /// <param name="Limit">Optional charging rate limit in chargingRateUnit.</param>
        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2.</param>
        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit.</param>
        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2.</param>
        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.UpdateDynamicScheduleResponse>

            UpdateDynamicSchedule(this ILocalController         LocalController,
                                  NetworkingNode_Id             DestinationNodeId,
                                  ChargingProfile_Id            ChargingProfileId,

                                  ChargingRateValue?            Limit                 = null,
                                  ChargingRateValue?            Limit_L2              = null,
                                  ChargingRateValue?            Limit_L3              = null,

                                  ChargingRateValue?            DischargeLimit        = null,
                                  ChargingRateValue?            DischargeLimit_L2     = null,
                                  ChargingRateValue?            DischargeLimit_L3     = null,

                                  ChargingRateValue?            Setpoint              = null,
                                  ChargingRateValue?            Setpoint_L2           = null,
                                  ChargingRateValue?            Setpoint_L3           = null,

                                  ChargingRateValue?            SetpointReactive      = null,
                                  ChargingRateValue?            SetpointReactive_L2   = null,
                                  ChargingRateValue?            SetpointReactive_L3   = null,

                                  CustomData?                   CustomData            = null,

                                  NetworkPath?                  NetworkPath           = null,

                                  IEnumerable<KeyPair>?         SignKeys              = null,
                                  IEnumerable<SignInfo>?        SignInfos             = null,
                                  IEnumerable<OCPP.Signature>?  Signatures            = null,

                                  Request_Id?                   RequestId             = null,
                                  DateTime?                     RequestTimestamp      = null,
                                  TimeSpan?                     RequestTimeout        = null,
                                  EventTracking_Id?             EventTrackingId       = null,
                                  CancellationToken             CancellationToken     = default)


                => LocalController.OCPP.OUT.UpdateDynamicSchedule(
                       new UpdateDynamicScheduleRequest(

                           DestinationNodeId,
                           ChargingProfileId,

                           Limit,
                           Limit_L2,
                           Limit_L3,

                           DischargeLimit,
                           DischargeLimit_L2,
                           DischargeLimit_L3,

                           Setpoint,
                           Setpoint_L2,
                           Setpoint_L3,

                           SetpointReactive,
                           SetpointReactive_L2,
                           SetpointReactive_L3,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyAllowedEnergyTransfer (DestinationNodeId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="AllowedEnergyTransferModes">An enumeration of allowed energy transfer modes.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.NotifyAllowedEnergyTransferResponse>

            NotifyAllowedEnergyTransfer(this ILocalController            LocalController,
                                        NetworkingNode_Id                DestinationNodeId,
                                        IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes,

                                        CustomData?                      CustomData          = null,

                                        NetworkPath?                     NetworkPath         = null,

                                        IEnumerable<KeyPair>?            SignKeys            = null,
                                        IEnumerable<SignInfo>?           SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?     Signatures          = null,

                                        Request_Id?                      RequestId           = null,
                                        DateTime?                        RequestTimestamp    = null,
                                        TimeSpan?                        RequestTimeout      = null,
                                        EventTracking_Id?                EventTrackingId     = null,
                                        CancellationToken                CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyAllowedEnergyTransfer(
                       new NotifyAllowedEnergyTransferRequest(
                           DestinationNodeId,
                           AllowedEnergyTransferModes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UsePriorityCharging         (DestinationNodeId, TransactionId, Activate, ...)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activate">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.UsePriorityChargingResponse>

            UsePriorityCharging(this ILocalController         LocalController,
                                NetworkingNode_Id             DestinationNodeId,
                                Transaction_Id                TransactionId,
                                Boolean                       Activate,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UsePriorityCharging(
                       new UsePriorityChargingRequest(
                           DestinationNodeId,
                           TransactionId,
                           Activate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UnlockConnector             (DestinationNodeId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.UnlockConnectorResponse>

            UnlockConnector(this ILocalController         LocalController,
                            NetworkingNode_Id             DestinationNodeId,
                            EVSE_Id                       EVSEId,
                            Connector_Id                  ConnectorId,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UnlockConnector(
                       new UnlockConnectorRequest(
                           DestinationNodeId,
                           EVSEId,
                           ConnectorId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SendAFRRSignal              (DestinationNodeId, ActivationTimestamp, Signal, ...)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
        /// <param name="Signal">Ther value of the signal in v2xSignalWattCurve. Usually between -1 and 1.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.AFRRSignalResponse>

            SendAFRRSignal(this ILocalController         LocalController,
                           NetworkingNode_Id             DestinationNodeId,
                           DateTime                      ActivationTimestamp,
                           AFRR_Signal                   Signal,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.AFRRSignal(
                       new AFRRSignalRequest(
                           DestinationNodeId,
                           ActivationTimestamp,
                           Signal,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SetDisplayMessage           (DestinationNodeId, Message, ...)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Message">A display message to be shown at the charging station.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetDisplayMessageResponse>

            SetDisplayMessage(this ILocalController         LocalController,
                              NetworkingNode_Id             DestinationNodeId,
                              MessageInfo                   Message,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetDisplayMessage(
                       new SetDisplayMessageRequest(
                           DestinationNodeId,
                           Message,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetDisplayMessages          (DestinationNodeId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null, ...)

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="GetDisplayMessagesRequestId">The unique identification of this get display messages request.</param>
        /// <param name="Ids">An optional filter on display message identifications. This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.</param>
        /// <param name="Priority">The optional filter on message priorities.</param>
        /// <param name="State">The optional filter on message states.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetDisplayMessagesResponse>

            GetDisplayMessages(this ILocalController            LocalController,
                               NetworkingNode_Id                DestinationNodeId,
                               Int32                            GetDisplayMessagesRequestId,
                               IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                               MessagePriority?                 Priority            = null,
                               MessageState?                    State               = null,

                               CustomData?                      CustomData          = null,

                               NetworkPath?                     NetworkPath         = null,

                               IEnumerable<KeyPair>?            SignKeys            = null,
                               IEnumerable<SignInfo>?           SignInfos           = null,
                               IEnumerable<OCPP.Signature>?     Signatures          = null,

                               Request_Id?                      RequestId           = null,
                               DateTime?                        RequestTimestamp    = null,
                               TimeSpan?                        RequestTimeout      = null,
                               EventTracking_Id?                EventTrackingId     = null,
                               CancellationToken                CancellationToken   = default)


                => LocalController.OCPP.OUT.GetDisplayMessages(
                       new GetDisplayMessagesRequest(
                           DestinationNodeId,
                           GetDisplayMessagesRequestId,
                           Ids,
                           Priority,
                           State,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearDisplayMessage         (DestinationNodeId, DisplayMessageId, ...)

        /// <summary>
        /// Remove a display message.
        /// </summary>
        /// <param name="DisplayMessageId">The identification of the display message to be removed.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.ClearDisplayMessageResponse>

            ClearDisplayMessage(this ILocalController         LocalController,
                                NetworkingNode_Id             DestinationNodeId,
                                DisplayMessage_Id             DisplayMessageId,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearDisplayMessage(
                       new ClearDisplayMessageRequest(
                           DestinationNodeId,
                           DisplayMessageId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendCostUpdated             (DestinationNodeId, TotalCost, TransactionId, ...)

        /// <summary>
        /// Send updated total costs.
        /// </summary>
        /// <param name="TotalCost">The current total cost, based on the information known by the CSMS, of the transaction including taxes. In the currency configured with the configuration Variable: [Currency]</param>
        /// <param name="TransactionId">The unique transaction identification the costs are asked for.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.CostUpdatedResponse>

            SendCostUpdated(this ILocalController         LocalController,
                            NetworkingNode_Id             DestinationNodeId,
                            Decimal                       TotalCost,
                            Transaction_Id                TransactionId,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CostUpdated(
                       new CostUpdatedRequest(
                           DestinationNodeId,
                           TotalCost,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region RequestCustomerInformation  (DestinationNodeId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="CustomerInformationRequestId">An unique identification of the customer information request.</param>
        /// <param name="Report">Whether the charging station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.</param>
        /// <param name="Clear">Whether the charging station should clear all information about the customer referred to.</param>
        /// <param name="CustomerIdentifier">An optional e.g. vendor specific identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.</param>
        /// <param name="IdToken">An optional IdToken of the customer this request refers to.</param>
        /// <param name="CustomerCertificate">An optional certificate of the customer this request refers to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.CustomerInformationResponse>

            RequestCustomerInformation(this ILocalController         LocalController,
                                       NetworkingNode_Id             DestinationNodeId,
                                       Int64                         CustomerInformationRequestId,
                                       Boolean                       Report,
                                       Boolean                       Clear,
                                       CustomerIdentifier?           CustomerIdentifier    = null,
                                       IdToken?                      IdToken               = null,
                                       CertificateHashData?          CustomerCertificate   = null,

                                       CustomData?                   CustomData            = null,

                                       NetworkPath?                  NetworkPath           = null,

                                       IEnumerable<KeyPair>?         SignKeys              = null,
                                       IEnumerable<SignInfo>?        SignInfos             = null,
                                       IEnumerable<OCPP.Signature>?  Signatures            = null,

                                       Request_Id?                   RequestId             = null,
                                       DateTime?                     RequestTimestamp      = null,
                                       TimeSpan?                     RequestTimeout        = null,
                                       EventTracking_Id?             EventTrackingId       = null,
                                       CancellationToken             CancellationToken     = default)


                => LocalController.OCPP.OUT.CustomerInformation(
                       new CustomerInformationRequest(
                           DestinationNodeId,
                           CustomerInformationRequestId,
                           Report,
                           Clear,
                           CustomerIdentifier,
                           IdToken,
                           CustomerCertificate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #endregion



        // Common

        #region TransferData                (DestinationNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this ILocalController         LocalController,

                         Vendor_Id                     VendorId,
                         Message_Id?                   MessageId           = null,
                         JToken?                       Data                = null,

                         CustomData?                   CustomData          = null,

                         NetworkingNode_Id?            DestinationNodeId   = null,
                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId         ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp  ?? Timestamp.Now,
                           RequestTimeout    ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId   ?? EventTracking_Id.New,
                           NetworkPath       ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion



        // Binary Data Streams Extensions

        #region TransferBinaryData          (DestinationNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
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

            TransferBinaryData(this ILocalController         LocalController,

                               NetworkingNode_Id             DestinationNodeId,
                               Vendor_Id                     VendorId,
                               Message_Id?                   MessageId           = null,
                               Byte[]?                       Data                = null,
                               BinaryFormats?                Format              = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.BinaryDataTransfer(
                       new BinaryDataTransferRequest(
                           DestinationNodeId,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetFile                     (DestinationNodeId, Filename, Priority = null, ...)

        /// <summary>
        /// Request to download the given file from the given local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
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

            GetFile(this ILocalController         LocalController,

                    NetworkingNode_Id             DestinationNodeId,
                    FilePath                      Filename,
                    Byte?                         Priority            = null,

                    CustomData?                   CustomData          = null,

                    NetworkPath?                  NetworkPath         = null,

                    IEnumerable<KeyPair>?         SignKeys            = null,
                    IEnumerable<SignInfo>?        SignInfos           = null,
                    IEnumerable<OCPP.Signature>?  Signatures          = null,

                    Request_Id?                   RequestId           = null,
                    DateTime?                     RequestTimestamp    = null,
                    TimeSpan?                     RequestTimeout      = null,
                    EventTracking_Id?             EventTrackingId     = null,
                    CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetFile(
                       new OCPP.CSMS.GetFileRequest(
                           DestinationNodeId,
                           Filename,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFile                    (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the given local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
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

            SendFile(this ILocalController         LocalController,

                     NetworkingNode_Id             DestinationNodeId,
                     FilePath                      FileName,
                     Byte[]                        FileContent,
                     ContentType?                  FileContentType     = null,
                     Byte[]?                       FileSHA256          = null,
                     Byte[]?                       FileSHA512          = null,
                     IEnumerable<OCPP.Signature>?  FileSignatures      = null,
                     Byte?                         Priority            = null,

                     CustomData?                   CustomData          = null,

                     NetworkPath?                  NetworkPath         = null,

                     IEnumerable<KeyPair>?         SignKeys            = null,
                     IEnumerable<SignInfo>?        SignInfos           = null,
                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                     Request_Id?                   RequestId           = null,
                     DateTime?                     RequestTimestamp    = null,
                     TimeSpan?                     RequestTimeout      = null,
                     EventTracking_Id?             EventTrackingId     = null,
                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SendFile(
                       new OCPP.CSMS.SendFileRequest(
                           DestinationNodeId,
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

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteFile                  (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Delete the given file from the given local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
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

            DeleteFile(this ILocalController         LocalController,

                       NetworkingNode_Id             DestinationNodeId,
                       FilePath                      FileName,
                       Byte[]?                       FileSHA256          = null,
                       Byte[]?                       FileSHA512          = null,

                       CustomData?                   CustomData          = null,

                       NetworkPath?                  NetworkPath         = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.DeleteFile(
                       new OCPP.CSMS.DeleteFileRequest(
                           DestinationNodeId,
                           FileName,
                           FileSHA256,
                           FileSHA512,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ListDirectory               (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// List the given directory of the given local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The local controller identification.</param>
        /// <param name="DirectoryPath">The absolute path of the directory to list.</param>
        /// <param name="Format">The optional response format of the directory listing.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<OCPP.CS.ListDirectoryResponse>

            ListDirectory(this ILocalController         LocalController,

                          NetworkingNode_Id             DestinationNodeId,
                          FilePath                      DirectoryPath,
                          ListDirectoryFormat?          Format                 = null,
                          Boolean?                      WithFileSizes          = null,
                          Boolean?                      WithFileDates          = null,
                          Boolean?                      WithSHA256FileHashes   = null,
                          Boolean?                      WithSHA512FileHashes   = null,

                          CustomData?                   CustomData             = null,

                          NetworkPath?                  NetworkPath            = null,

                          IEnumerable<KeyPair>?         SignKeys               = null,
                          IEnumerable<SignInfo>?        SignInfos              = null,
                          IEnumerable<OCPP.Signature>?  Signatures             = null,

                          Request_Id?                   RequestId              = null,
                          DateTime?                     RequestTimestamp       = null,
                          TimeSpan?                     RequestTimeout         = null,
                          EventTracking_Id?             EventTrackingId        = null,
                          CancellationToken             CancellationToken      = default)


                => LocalController.OCPP.OUT.ListDirectory(
                       new OCPP.CSMS.ListDirectoryRequest(
                           DestinationNodeId,
                           DirectoryPath,
                           Format,
                           WithFileSizes,
                           WithFileDates,
                           WithSHA256FileHashes,
                           WithSHA512FileHashes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion



        // E2E Security Extensions





        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff    (DestinationNodeId, ChargingTariff,          EVSEIds = null, ...)

        /// <summary>
        /// Set a default charging tariff for the charging station,
        /// or for a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="ChargingTariff">A charging tariff.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff applies to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.SetDefaultChargingTariffResponse>

            SetDefaultChargingTariff(this ILocalController         LocalController,
                                     NetworkingNode_Id             DestinationNodeId,
                                     ChargingTariff                ChargingTariff,
                                     IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                     CustomData?                   CustomData          = null,

                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetDefaultChargingTariff(
                       new SetDefaultChargingTariffRequest(
                           DestinationNodeId,
                           ChargingTariff,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetDefaultChargingTariff    (DestinationNodeId,                          EVSEIds = null, ...)

        /// <summary>
        /// Get the default charging tariff(s) for the charging station and its EVSEs.
        /// </summary>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff should be reported on.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.GetDefaultChargingTariffResponse>

            GetDefaultChargingTariff(this ILocalController         LocalController,
                                     NetworkingNode_Id             DestinationNodeId,
                                     IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                     CustomData?                   CustomData          = null,

                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetDefaultChargingTariff(
                       new GetDefaultChargingTariffRequest(
                           DestinationNodeId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region RemoveDefaultChargingTariff (DestinationNodeId, ChargingTariffId = null, EVSEIds = null, ...)

        /// <summary>
        /// Remove the default charging tariff of the charging station,
        /// or of a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="ChargingTariffId">The optional unique charging tariff identification of the default charging tariff to be removed.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff applies to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.RemoveDefaultChargingTariffResponse>

            RemoveDefaultChargingTariff(this ILocalController         LocalController,
                                        NetworkingNode_Id             DestinationNodeId,
                                        ChargingTariff_Id?            ChargingTariffId    = null,
                                        IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                        CustomData?                   CustomData          = null,

                                        NetworkPath?                  NetworkPath         = null,

                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RemoveDefaultChargingTariff(
                       new RemoveDefaultChargingTariffRequest(
                           DestinationNodeId,
                           ChargingTariffId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken
                       )
                   );

        #endregion



        // Overlay Networking Extensions

        #region NotifyNetworkTopology                 (LocalController, ...)

        /// <summary>
        /// Transfer the given binary data to the CSMS.
        /// </summary>
        /// <param name="NetworkTopologyInformation">A network topology information.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<OCPP.NN.NotifyNetworkTopologyResponse>

            NotifyNetworkTopology(this ILocalController         LocalController,

                                  NetworkTopologyInformation    NetworkTopologyInformation,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationNodeId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyNetworkTopology(
                       new OCPP.NN.NotifyNetworkTopologyRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NetworkTopologyInformation,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? LocalController.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(LocalController.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyNetworkTopology                 (LocalController, ...)

        ///// <summary>
        ///// Transfer the given binary data to the CSMS.
        ///// </summary>
        ///// <param name="NetworkTopologyInformation">A network topology information.</param>
        ///// 
        ///// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        ///// 
        ///// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<OCPP.NN.NotifyNetworkTopologyResponse>

        //    NotifyNetworkTopology(this ILocalController2         LocalController,

        //                          NetworkingNode_Id             DestinationNodeId,
        //                          NetworkTopologyInformation    NetworkTopologyInformation,

        //                          CustomData?                   CustomData          = null,

        //                          IEnumerable<KeyPair>?         SignKeys            = null,
        //                          IEnumerable<SignInfo>?        SignInfos           = null,
        //                          IEnumerable<OCPP.Signature>?  Signatures          = null,

        //                          Request_Id?                   RequestId           = null,
        //                          DateTime?                     RequestTimestamp    = null,
        //                          TimeSpan?                     RequestTimeout      = null,
        //                          EventTracking_Id?             EventTrackingId     = null,
        //                          CancellationToken             CancellationToken   = default)


        //        => LocalController.ocppOUT.NotifyNetworkTopology(
        //               new OCPP.NN.NotifyNetworkTopologyRequest(

        //                   DestinationNodeId,
        //                   NetworkTopologyInformation,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   CustomData,

        //                   RequestId        ?? LocalController.OCPPAdapter.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? LocalController.OCPP.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath      ?? NetworkPath.From(LocalController.Id),
        //                   CancellationToken

        //               )
        //           );

        #endregion



    }

}
