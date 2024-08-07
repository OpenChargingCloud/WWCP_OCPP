﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LC
{

    /// <summary>
    /// Extension methods for all local controllers.
    /// </summary>
    public static class ILocalControllerNodeExtensions
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

            SendBootNotification(this ILocalControllerNode         LocalController,

                                 BootReason                    BootReason,

                                 CustomData?                   CustomData           = null,

                                 NetworkingNode_Id?            DestinationId        = null,
                                 NetworkPath?                  NetworkPath          = null,

                                 IEnumerable<KeyPair>?         SignKeys             = null,
                                 IEnumerable<SignInfo>?        SignInfos            = null,
                                 IEnumerable<Signature>?       Signatures           = null,

                                 Request_Id?                   RequestId            = null,
                                 DateTime?                     RequestTimestamp     = null,
                                 TimeSpan?                     RequestTimeout       = null,
                                 EventTracking_Id?             EventTrackingId      = null,
                                 CancellationToken             CancellationToken    = default)


                => LocalController.OCPP.OUT.BootNotification(
                       new BootNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendFirmwareStatusNotification(this ILocalControllerNode         LocalController,

                                           FirmwareStatus                Status,
                                           Int64?                        UpdateFirmwareRequestId   = null,

                                           CustomData?                   CustomData                = null,

                                           NetworkingNode_Id?            DestinationId         = null,
                                           NetworkPath?                  NetworkPath               = null,

                                           IEnumerable<KeyPair>?         SignKeys                  = null,
                                           IEnumerable<SignInfo>?        SignInfos                 = null,
                                           IEnumerable<Signature>?       Signatures                = null,

                                           Request_Id?                   RequestId                 = null,
                                           DateTime?                     RequestTimestamp          = null,
                                           TimeSpan?                     RequestTimeout            = null,
                                           EventTracking_Id?             EventTrackingId           = null,
                                           CancellationToken             CancellationToken         = default)


                => LocalController.OCPP.OUT.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendPublishFirmwareStatusNotification(this ILocalControllerNode         LocalController,

                                                  PublishFirmwareStatus         Status,
                                                  Int32?                        PublishFirmwareStatusNotificationRequestId,
                                                  IEnumerable<URL>?             DownloadLocations,

                                                  CustomData?                   CustomData          = null,

                                                  NetworkingNode_Id?            DestinationId   = null,
                                                  NetworkPath?                  NetworkPath         = null,

                                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                                  IEnumerable<Signature>?       Signatures          = null,

                                                  Request_Id?                   RequestId           = null,
                                                  DateTime?                     RequestTimestamp    = null,
                                                  TimeSpan?                     RequestTimeout      = null,
                                                  EventTracking_Id?             EventTrackingId     = null,
                                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PublishFirmwareStatusNotification(
                       new PublishFirmwareStatusNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendHeartbeat(this ILocalControllerNode         LocalController,

                          CustomData?                   CustomData          = null,

                          NetworkingNode_Id?            DestinationId   = null,
                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<Signature>?       Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.Heartbeat(
                       new HeartbeatRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyEvent(this ILocalControllerNode         LocalController,

                        DateTime                      GeneratedAt,
                        UInt32                        SequenceNumber,
                        IEnumerable<EventData>        EventData,
                        Boolean?                      ToBeContinued       = null,

                        CustomData?                   CustomData          = null,

                        NetworkingNode_Id?            DestinationId   = null,
                        NetworkPath?                  NetworkPath         = null,

                        IEnumerable<KeyPair>?         SignKeys            = null,
                        IEnumerable<SignInfo>?        SignInfos           = null,
                        IEnumerable<Signature>?       Signatures          = null,

                        Request_Id?                   RequestId           = null,
                        DateTime?                     RequestTimestamp    = null,
                        TimeSpan?                     RequestTimeout      = null,
                        EventTracking_Id?             EventTrackingId     = null,
                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyEvent(
                       new NotifyEventRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendSecurityEventNotification(this ILocalControllerNode         LocalController,

                                          SecurityEventType             Type,
                                          DateTime                      Timestamp,
                                          String?                       TechInfo            = null,

                                          CustomData?                   CustomData          = null,

                                          NetworkingNode_Id?            DestinationId   = null,
                                          NetworkPath?                  NetworkPath         = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<Signature>?       Signatures          = null,

                                          Request_Id?                   RequestId           = null,
                                          DateTime?                     RequestTimestamp    = null,
                                          TimeSpan?                     RequestTimeout      = null,
                                          EventTracking_Id?             EventTrackingId     = null,
                                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SecurityEventNotification(
                       new SecurityEventNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyReport(this ILocalControllerNode         LocalController,

                         Int32                         NotifyReportRequestId,
                         UInt32                        SequenceNumber,
                         DateTime                      GeneratedAt,
                         IEnumerable<ReportData>       ReportData,
                         Boolean?                      ToBeContinued       = null,

                         CustomData?                   CustomData          = null,

                         NetworkingNode_Id?            DestinationId   = null,
                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<Signature>?       Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyReport(
                       new NotifyReportRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyMonitoringReport(this ILocalControllerNode         LocalController,

                                   Int32                         NotifyMonitoringReportRequestId,
                                   UInt32                        SequenceNumber,
                                   DateTime                      GeneratedAt,
                                   IEnumerable<MonitoringData>   MonitoringData,
                                   Boolean?                      ToBeContinued       = null,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyMonitoringReport(
                       new NotifyMonitoringReportRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendLogStatusNotification(this ILocalControllerNode         LocalController,

                                      UploadLogStatus               Status,
                                      Int32?                        LogRequestId        = null,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.LogStatusNotification(
                       new LogStatusNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendCertificateSigningRequest(this ILocalControllerNode         LocalController,

                                          String                        CSR,
                                          Int32                         SignCertificateRequestId,
                                          CertificateSigningUse?        CertificateType     = null,

                                          CustomData?                   CustomData          = null,

                                          NetworkingNode_Id?            DestinationId   = null,
                                          NetworkPath?                  NetworkPath         = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<Signature>?       Signatures          = null,

                                          Request_Id?                   RequestId           = null,
                                          DateTime?                     RequestTimestamp    = null,
                                          TimeSpan?                     RequestTimeout      = null,
                                          EventTracking_Id?             EventTrackingId     = null,
                                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SignCertificate(
                       new SignCertificateRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            Get15118EVCertificate(this ILocalControllerNode         LocalController,

                                  ISO15118SchemaVersion         ISO15118SchemaVersion,
                                  CertificateAction             CertificateAction,
                                  EXIData                       EXIRequest,
                                  UInt32?                       MaximumContractCertificateChains   = 1,
                                  IEnumerable<EMA_Id>?          PrioritizedEMAIds                  = null,

                                  CustomData?                   CustomData                         = null,

                                  NetworkingNode_Id?            DestinationId                  = null,
                                  NetworkPath?                  NetworkPath                        = null,

                                  IEnumerable<KeyPair>?         SignKeys                           = null,
                                  IEnumerable<SignInfo>?        SignInfos                          = null,
                                  IEnumerable<Signature>?       Signatures                         = null,

                                  Request_Id?                   RequestId                          = null,
                                  DateTime?                     RequestTimestamp                   = null,
                                  TimeSpan?                     RequestTimeout                     = null,
                                  EventTracking_Id?             EventTrackingId                    = null,
                                  CancellationToken             CancellationToken                  = default)


                => LocalController.OCPP.OUT.Get15118EVCertificate(
                       new Get15118EVCertificateRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            GetCertificateStatus(this ILocalControllerNode         LocalController,

                                 OCSPRequestData               OCSPRequestData,

                                 CustomData?                   CustomData          = null,

                                 NetworkingNode_Id?            DestinationId   = null,
                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCertificateStatus(
                       new GetCertificateStatusRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            GetCRLRequest(this ILocalControllerNode         LocalController,

                          UInt32                        GetCRLRequestId,
                          CertificateHashData           CertificateHashData,

                          CustomData?                   CustomData          = null,

                          NetworkingNode_Id?            DestinationId   = null,
                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<Signature>?       Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCRL(
                       new GetCRLRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendReservationStatusUpdate(this ILocalControllerNode         LocalController,

                                        Reservation_Id                ReservationId,
                                        ReservationUpdateStatus       ReservationUpdateStatus,

                                        CustomData?                   CustomData          = null,

                                        NetworkingNode_Id?            DestinationId   = null,
                                        NetworkPath?                  NetworkPath         = null,

                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                        IEnumerable<Signature>?       Signatures          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReservationStatusUpdate(
                       new ReservationStatusUpdateRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            Authorize(this ILocalControllerNode          LocalController,

                      IdToken                        IdToken,
                      Certificate?                   Certificate                   = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                      CustomData?                    CustomData                    = null,

                      NetworkingNode_Id?             DestinationId             = null,
                      NetworkPath?                   NetworkPath                   = null,

                      IEnumerable<KeyPair>?          SignKeys                      = null,
                      IEnumerable<SignInfo>?         SignInfos                     = null,
                      IEnumerable<Signature>?        Signatures                    = null,

                      Request_Id?                    RequestId                     = null,
                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      CancellationToken              CancellationToken             = default)


                => LocalController.OCPP.OUT.Authorize(
                       new AuthorizeRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyEVChargingNeeds(this ILocalControllerNode         LocalController,

                                  EVSE_Id                       EVSEId,
                                  ChargingNeeds                 ChargingNeeds,
                                  DateTime?                     ReceivedTimestamp   = null,
                                  UInt16?                       MaxScheduleTuples   = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyEVChargingNeeds(
                       new NotifyEVChargingNeedsRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendTransactionEvent(this ILocalControllerNode         LocalController,

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

                                 NetworkingNode_Id?            DestinationId       = null,
                                 NetworkPath?                  NetworkPath             = null,

                                 IEnumerable<KeyPair>?         SignKeys                = null,
                                 IEnumerable<SignInfo>?        SignInfos               = null,
                                 IEnumerable<Signature>?       Signatures              = null,

                                 Request_Id?                   RequestId               = null,
                                 DateTime?                     RequestTimestamp        = null,
                                 TimeSpan?                     RequestTimeout          = null,
                                 EventTracking_Id?             EventTrackingId         = null,
                                 CancellationToken             CancellationToken       = default)


                => LocalController.OCPP.OUT.TransactionEvent(
                       new TransactionEventRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendStatusNotification(this ILocalControllerNode         LocalController,

                                   EVSE_Id                       EVSEId,
                                   Connector_Id                  ConnectorId,
                                   DateTime                      Timestamp,
                                   ConnectorStatus               Status,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.StatusNotification(
                       new StatusNotificationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendMeterValues(this ILocalControllerNode         LocalController,

                            EVSE_Id                       EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>       MeterValues,

                            CustomData?                   CustomData          = null,

                            NetworkingNode_Id?            DestinationId   = null,
                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<Signature>?       Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.MeterValues(
                       new MeterValuesRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyChargingLimit(this ILocalControllerNode          LocalController,

                                ChargingLimit                  ChargingLimit,
                                IEnumerable<ChargingSchedule>  ChargingSchedules,
                                EVSE_Id?                       EVSEId              = null,

                                CustomData?                    CustomData          = null,

                                NetworkingNode_Id?             DestinationId   = null,
                                NetworkPath?                   NetworkPath         = null,

                                IEnumerable<KeyPair>?          SignKeys            = null,
                                IEnumerable<SignInfo>?         SignInfos           = null,
                                IEnumerable<Signature>?        Signatures          = null,

                                Request_Id?                    RequestId           = null,
                                DateTime?                      RequestTimestamp    = null,
                                TimeSpan?                      RequestTimeout      = null,
                                EventTracking_Id?              EventTrackingId     = null,
                                CancellationToken              CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyChargingLimit(
                       new NotifyChargingLimitRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            SendClearedChargingLimit(this ILocalControllerNode         LocalController,

                                     ChargingLimitSource           ChargingLimitSource,
                                     EVSE_Id?                      EVSEId,

                                     CustomData?                   CustomData          = null,

                                     NetworkingNode_Id?            DestinationId   = null,
                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<Signature>?       Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearedChargingLimit(
                       new ClearedChargingLimitRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            ReportChargingProfiles(this ILocalControllerNode         LocalController,

                                   Int32                         ReportChargingProfilesRequestId,
                                   ChargingLimitSource           ChargingLimitSource,
                                   EVSE_Id                       EVSEId,
                                   IEnumerable<ChargingProfile>  ChargingProfiles,
                                   Boolean?                      ToBeContinued       = null,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReportChargingProfiles(
                       new ReportChargingProfilesRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyEVChargingSchedule(this ILocalControllerNode         LocalController,

                                     DateTime                      TimeBase,
                                     EVSE_Id                       EVSEId,
                                     ChargingSchedule              ChargingSchedule,
                                     Byte?                         SelectedScheduleTupleId    = null,
                                     Boolean?                      PowerToleranceAcceptance   = null,

                                     CustomData?                   CustomData                 = null,

                                     NetworkingNode_Id?            DestinationId          = null,
                                     NetworkPath?                  NetworkPath                = null,

                                     IEnumerable<KeyPair>?         SignKeys                   = null,
                                     IEnumerable<SignInfo>?        SignInfos                  = null,
                                     IEnumerable<Signature>?       Signatures                 = null,

                                     Request_Id?                   RequestId                  = null,
                                     DateTime?                     RequestTimestamp           = null,
                                     TimeSpan?                     RequestTimeout             = null,
                                     EventTracking_Id?             EventTrackingId            = null,
                                     CancellationToken             CancellationToken          = default)


                => LocalController.OCPP.OUT.NotifyEVChargingSchedule(
                       new NotifyEVChargingScheduleRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyPriorityCharging(this ILocalControllerNode         LocalController,

                                   Transaction_Id                TransactionId,
                                   Boolean                       Activated,

                                   CustomData?                   CustomData          = null,

                                   NetworkingNode_Id?            DestinationId   = null,
                                   NetworkPath?                  NetworkPath         = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyPriorityCharging(
                       new NotifyPriorityChargingRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            PullDynamicScheduleUpdate(this ILocalControllerNode         LocalController,

                                      ChargingProfile_Id            ChargingProfileId,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PullDynamicScheduleUpdate(
                       new PullDynamicScheduleUpdateRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyDisplayMessages(this ILocalControllerNode         LocalController,

                                  Int32                         NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>      MessageInfos,
                                  Boolean?                      ToBeContinued       = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyDisplayMessages(
                       new NotifyDisplayMessagesRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

            NotifyCustomerInformation(this ILocalControllerNode         LocalController,

                                      Int64                         NotifyCustomerInformationRequestId,
                                      String                        Data,
                                      UInt32                        SequenceNumber,
                                      DateTime                      GeneratedAt,
                                      Boolean?                      ToBeContinued       = null,

                                      CustomData?                   CustomData          = null,

                                      NetworkingNode_Id?            DestinationId   = null,
                                      NetworkPath?                  NetworkPath         = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyCustomerInformation(
                       new NotifyCustomerInformationRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

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

        #region Reset                       (DestinationId, ResetType, EVSEId = null, ...)

        /// <summary>
        /// Reset the given charging station/local controller.
        /// </summary>
        /// <param name="DestinationId">The charging station/local controller identification.</param>
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

            Reset(this ILocalControllerNode         LocalController,
                  NetworkingNode_Id             DestinationId,
                  ResetType                     ResetType,
                  EVSE_Id?                      EVSEId              = null,

                  CustomData?                   CustomData          = null,

                  NetworkPath?                  NetworkPath         = null,

                  IEnumerable<KeyPair>?         SignKeys            = null,
                  IEnumerable<SignInfo>?        SignInfos           = null,
                  IEnumerable<Signature>?       Signatures          = null,

                  Request_Id?                   RequestId           = null,
                  DateTime?                     RequestTimestamp    = null,
                  TimeSpan?                     RequestTimeout      = null,
                  EventTracking_Id?             EventTrackingId     = null,
                  CancellationToken             CancellationToken   = default)

                => LocalController.OCPP.OUT.Reset(
                       new ResetRequest(
                           DestinationId,
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

        #region UpdateFirmware              (DestinationId, Firmware, UpdateFirmwareRequestId, ...)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            UpdateFirmware(this ILocalControllerNode         LocalController,
                           NetworkingNode_Id             DestinationId,
                           Firmware                      Firmware,
                           Int32                         UpdateFirmwareRequestId,
                           Byte?                         Retries             = null,
                           TimeSpan?                     RetryInterval       = null,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<Signature>?       Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UpdateFirmware(
                       new UpdateFirmwareRequest(
                           DestinationId,
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

        #region PublishFirmware             (DestinationId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            PublishFirmware(this ILocalControllerNode         LocalController,
                            NetworkingNode_Id             DestinationId,
                            Int32                         PublishFirmwareRequestId,
                            URL                           DownloadLocation,
                            String                        MD5Checksum,
                            Byte?                         Retries             = null,
                            TimeSpan?                     RetryInterval       = null,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<Signature>?       Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.PublishFirmware(
                       new PublishFirmwareRequest(
                           DestinationId,
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

        #region UnpublishFirmware           (DestinationId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            UnpublishFirmware(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              String                        MD5Checksum,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UnpublishFirmware(
                       new UnpublishFirmwareRequest(
                           DestinationId,
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

        #region GetBaseReport               (DestinationId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetBaseReport(this ILocalControllerNode         LocalController,
                          NetworkingNode_Id             DestinationId,
                          Int64                         GetBaseReportRequestId,
                          ReportBase                    ReportBase,

                          CustomData?                   CustomData          = null,

                          NetworkPath?                  NetworkPath         = null,

                          IEnumerable<KeyPair>?         SignKeys            = null,
                          IEnumerable<SignInfo>?        SignInfos           = null,
                          IEnumerable<Signature>?       Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetBaseReport(
                       new GetBaseReportRequest(
                           DestinationId,
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

        #region GetReport                   (DestinationId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetReport(this ILocalControllerNode           LocalController,
                      NetworkingNode_Id               DestinationId,
                      Int32                           GetReportRequestId,
                      IEnumerable<ComponentCriteria>  ComponentCriteria,
                      IEnumerable<ComponentVariable>  ComponentVariables,

                      CustomData?                     CustomData          = null,

                      NetworkPath?                    NetworkPath         = null,

                      IEnumerable<KeyPair>?           SignKeys            = null,
                      IEnumerable<SignInfo>?          SignInfos           = null,
                      IEnumerable<Signature>?         Signatures          = null,

                      Request_Id?                     RequestId           = null,
                      DateTime?                       RequestTimestamp    = null,
                      TimeSpan?                       RequestTimeout      = null,
                      EventTracking_Id?               EventTrackingId     = null,
                      CancellationToken               CancellationToken   = default)


                => LocalController.OCPP.OUT.GetReport(
                       new GetReportRequest(
                           DestinationId,
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

        #region GetLog                      (DestinationId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetLog(this ILocalControllerNode         LocalController,
                   NetworkingNode_Id             DestinationId,
                   LogType                       LogType,
                   Int32                         LogRequestId,
                   LogParameters                 Log,
                   Byte?                         Retries             = null,
                   TimeSpan?                     RetryInterval       = null,

                   CustomData?                   CustomData          = null,

                   NetworkPath?                  NetworkPath         = null,

                   IEnumerable<KeyPair>?         SignKeys            = null,
                   IEnumerable<SignInfo>?        SignInfos           = null,
                   IEnumerable<Signature>?       Signatures          = null,

                   Request_Id?                   RequestId           = null,
                   DateTime?                     RequestTimestamp    = null,
                   TimeSpan?                     RequestTimeout      = null,
                   EventTracking_Id?             EventTrackingId     = null,
                   CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetLog(
                       new GetLogRequest(
                           DestinationId,
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


        #region SetVariables                (DestinationId, VariableData, DataConsistencyModel = null, ...)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetVariables(this ILocalControllerNode         LocalController,
                         NetworkingNode_Id             DestinationId,
                         IEnumerable<SetVariableData>  VariableData,
                         DataConsistencyModel?         DataConsistencyModel   = null,

                         CustomData?                   CustomData             = null,

                         NetworkPath?                  NetworkPath            = null,

                         IEnumerable<KeyPair>?         SignKeys               = null,
                         IEnumerable<SignInfo>?        SignInfos              = null,
                         IEnumerable<Signature>?       Signatures             = null,

                         Request_Id?                   RequestId              = null,
                         DateTime?                     RequestTimestamp       = null,
                         TimeSpan?                     RequestTimeout         = null,
                         EventTracking_Id?             EventTrackingId        = null,
                         CancellationToken             CancellationToken      = default)


                => LocalController.OCPP.OUT.SetVariables(
                       new SetVariablesRequest(
                           DestinationId,
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

        #region GetVariables                (DestinationId, VariableData, ...)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetVariables(this ILocalControllerNode         LocalController,
                         NetworkingNode_Id             DestinationId,
                         IEnumerable<GetVariableData>  VariableData,

                         CustomData?                   CustomData          = null,

                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<Signature>?       Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetVariables(
                       new GetVariablesRequest(
                           DestinationId,
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

        #region SetMonitoringBase           (DestinationId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetMonitoringBase(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              MonitoringBase                MonitoringBase,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetMonitoringBase(
                       new SetMonitoringBaseRequest(
                           DestinationId,
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

        #region GetMonitoringReport         (DestinationId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetMonitoringReport(this ILocalControllerNode             LocalController,
                                NetworkingNode_Id                 DestinationId,
                                Int32                             GetMonitoringReportRequestId,
                                IEnumerable<MonitoringCriterion>  MonitoringCriteria,
                                IEnumerable<ComponentVariable>    ComponentVariables,

                                CustomData?                       CustomData          = null,

                                NetworkPath?                      NetworkPath         = null,

                                IEnumerable<KeyPair>?             SignKeys            = null,
                                IEnumerable<SignInfo>?            SignInfos           = null,
                                IEnumerable<Signature>?           Signatures          = null,

                                Request_Id?                       RequestId           = null,
                                DateTime?                         RequestTimestamp    = null,
                                TimeSpan?                         RequestTimeout      = null,
                                EventTracking_Id?                 EventTrackingId     = null,
                                CancellationToken                 CancellationToken   = default)


                => LocalController.OCPP.OUT.GetMonitoringReport(
                       new GetMonitoringReportRequest(
                           DestinationId,
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

        #region SetMonitoringLevel          (DestinationId, Severity, ...)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetMonitoringLevel(this ILocalControllerNode         LocalController,
                               NetworkingNode_Id             DestinationId,
                               Severities                    Severity,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<Signature>?       Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetMonitoringLevel(
                       new SetMonitoringLevelRequest(
                           DestinationId,
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

        #region SetVariableMonitoring       (DestinationId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetVariableMonitoring(this ILocalControllerNode           LocalController,
                                  NetworkingNode_Id               DestinationId,
                                  IEnumerable<SetMonitoringData>  MonitoringData,

                                  CustomData?                     CustomData          = null,

                                  NetworkPath?                    NetworkPath         = null,

                                  IEnumerable<KeyPair>?           SignKeys            = null,
                                  IEnumerable<SignInfo>?          SignInfos           = null,
                                  IEnumerable<Signature>?         Signatures          = null,

                                  Request_Id?                     RequestId           = null,
                                  DateTime?                       RequestTimestamp    = null,
                                  TimeSpan?                       RequestTimeout      = null,
                                  EventTracking_Id?               EventTrackingId     = null,
                                  CancellationToken               CancellationToken   = default)


                => LocalController.OCPP.OUT.SetVariableMonitoring(
                       new SetVariableMonitoringRequest(
                           DestinationId,
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

        #region ClearVariableMonitoring     (DestinationId, VariableMonitoringIds, ...)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            ClearVariableMonitoring(this ILocalControllerNode               LocalController,
                                    NetworkingNode_Id                   DestinationId,
                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

                                    CustomData?                         CustomData          = null,

                                    NetworkPath?                        NetworkPath         = null,

                                    IEnumerable<KeyPair>?               SignKeys            = null,
                                    IEnumerable<SignInfo>?              SignInfos           = null,
                                    IEnumerable<Signature>?             Signatures          = null,

                                    Request_Id?                         RequestId           = null,
                                    DateTime?                           RequestTimestamp    = null,
                                    TimeSpan?                           RequestTimeout      = null,
                                    EventTracking_Id?                   EventTrackingId     = null,
                                    CancellationToken                   CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearVariableMonitoring(
                       new ClearVariableMonitoringRequest(
                           DestinationId,
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

        #region SetNetworkProfile           (DestinationId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetNetworkProfile(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              Int32                         ConfigurationSlot,
                              NetworkConnectionProfile      NetworkConnectionProfile,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetNetworkProfile(
                       new SetNetworkProfileRequest(
                           DestinationId,
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

        #region ChangeAvailability          (DestinationId, OperationalStatus, EVSE = null, ...)

        /// <summary>
        /// Change the availability of the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            ChangeAvailability(this ILocalControllerNode         LocalController,
                               NetworkingNode_Id             DestinationId,
                               OperationalStatus             OperationalStatus,

                               EVSE?                         EVSE                = null,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<Signature>?       Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           DestinationId,
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

        #region TriggerMessage              (DestinationId, RequestedMessage, EVSEId = null, CustomTrigger = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            TriggerMessage(this ILocalControllerNode         LocalController,
                           NetworkingNode_Id             DestinationId,
                           MessageTrigger                RequestedMessage,
                           EVSE?                         EVSE                = null,
                           String?                       CustomTrigger       = null,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<Signature>?       Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.TriggerMessage(
                       new TriggerMessageRequest(
                           DestinationId,
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


        #region SendSignedCertificate       (DestinationId, CertificateChain, CertificateType = null, ...)

        /// <summary>
        /// Send the signed certificate to the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SendSignedCertificate(this ILocalControllerNode         LocalController,
                                  NetworkingNode_Id             DestinationId,
                                  CertificateChain              CertificateChain,
                                  CertificateSigningUse?        CertificateType     = null,

                                  CustomData?                   CustomData          = null,

                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CertificateSigned(
                       new CertificateSignedRequest(
                           DestinationId,
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

        #region InstallCertificate          (DestinationId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            InstallCertificate(this ILocalControllerNode         LocalController,
                               NetworkingNode_Id             DestinationId,
                               InstallCertificateUse         CertificateType,
                               Certificate                   Certificate,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<Signature>?       Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.InstallCertificate(
                       new InstallCertificateRequest(
                           DestinationId,
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

        #region GetInstalledCertificateIds  (DestinationId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetInstalledCertificateIds(this ILocalControllerNode              LocalController,
                                       NetworkingNode_Id                  DestinationId,
                                       IEnumerable<GetCertificateIdUse>?  CertificateTypes    = null,

                                       CustomData?                        CustomData          = null,

                                       NetworkPath?                       NetworkPath         = null,

                                       IEnumerable<KeyPair>?              SignKeys            = null,
                                       IEnumerable<SignInfo>?             SignInfos           = null,
                                       IEnumerable<Signature>?            Signatures          = null,

                                       Request_Id?                        RequestId           = null,
                                       DateTime?                          RequestTimestamp    = null,
                                       TimeSpan?                          RequestTimeout      = null,
                                       EventTracking_Id?                  EventTrackingId     = null,
                                       CancellationToken                  CancellationToken   = default)


                => LocalController.OCPP.OUT.GetInstalledCertificateIds(
                       new GetInstalledCertificateIdsRequest(
                           DestinationId,
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

        #region DeleteCertificate           (DestinationId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            DeleteCertificate(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              CertificateHashData           CertificateHashData,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.DeleteCertificate(
                       new DeleteCertificateRequest(
                           DestinationId,
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

        #region NotifyCRLAvailability       (DestinationId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            NotifyCRLAvailability(this ILocalControllerNode         LocalController,
                                  NetworkingNode_Id             DestinationId,
                                  Int32                         NotifyCRLRequestId,
                                  NotifyCRLStatus               Availability,
                                  URL?                          Location,

                                  CustomData?                   CustomData          = null,

                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyCRL(
                       new NotifyCRLRequest(
                           DestinationId,
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


        #region GetLocalListVersion         (DestinationId, ...)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetLocalListVersion(this ILocalControllerNode         LocalController,
                                NetworkingNode_Id             DestinationId,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<Signature>?       Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetLocalListVersion(
                       new GetLocalListVersionRequest(
                           DestinationId,

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

        #region SendLocalList               (DestinationId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SendLocalList(this ILocalControllerNode            LocalController,
                          NetworkingNode_Id                DestinationId,
                          UInt64                           ListVersion,
                          UpdateTypes                      UpdateType,
                          IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                          CustomData?                      CustomData               = null,

                          NetworkPath?                     NetworkPath              = null,

                          IEnumerable<KeyPair>?            SignKeys                 = null,
                          IEnumerable<SignInfo>?           SignInfos                = null,
                          IEnumerable<Signature>?          Signatures               = null,

                          Request_Id?                      RequestId                = null,
                          DateTime?                        RequestTimestamp         = null,
                          TimeSpan?                        RequestTimeout           = null,
                          EventTracking_Id?                EventTrackingId          = null,
                          CancellationToken                CancellationToken        = default)


                => LocalController.OCPP.OUT.SendLocalList(
                       new SendLocalListRequest(
                           DestinationId,
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

        #region ClearCache                  (DestinationId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            ClearCache(this ILocalControllerNode         LocalController,
                       NetworkingNode_Id             DestinationId,

                       CustomData?                   CustomData          = null,

                       NetworkPath?                  NetworkPath         = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<Signature>?       Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearCache(
                       new ClearCacheRequest(
                           DestinationId,

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


        #region ReserveNow                  (DestinationId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            ReserveNow(this ILocalControllerNode         LocalController,
                       NetworkingNode_Id             DestinationId,
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
                       IEnumerable<Signature>?       Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ReserveNow(
                       new ReserveNowRequest(
                           DestinationId,
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

        #region CancelReservation           (DestinationId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charging station.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            CancelReservation(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              Reservation_Id                ReservationId,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CancelReservation(
                       new CancelReservationRequest(
                           DestinationId,
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

        #region StartCharging               (DestinationId, RequestStartTransactionRequestId, IdToken, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            StartCharging(this ILocalControllerNode         LocalController,
                          NetworkingNode_Id             DestinationId,
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
                          IEnumerable<Signature>?       Signatures          = null,

                          Request_Id?                   RequestId           = null,
                          DateTime?                     RequestTimestamp    = null,
                          TimeSpan?                     RequestTimeout      = null,
                          EventTracking_Id?             EventTrackingId     = null,
                          CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RequestStartTransaction(
                       new RequestStartTransactionRequest(
                           DestinationId,
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

        #region StopCharging                (DestinationId, TransactionId, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            StopCharging(this ILocalControllerNode         LocalController,
                         NetworkingNode_Id             DestinationId,
                         Transaction_Id                TransactionId,

                         CustomData?                   CustomData          = null,

                         NetworkPath?                  NetworkPath         = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<Signature>?       Signatures          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RequestStopTransaction(
                       new RequestStopTransactionRequest(
                           DestinationId,
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

        #region GetTransactionStatus        (DestinationId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetTransactionStatus(this ILocalControllerNode         LocalController,
                                 NetworkingNode_Id             DestinationId,
                                 Transaction_Id?               TransactionId       = null,

                                 CustomData?                   CustomData          = null,

                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetTransactionStatus(
                       new GetTransactionStatusRequest(
                           DestinationId,
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

        #region SetChargingProfile          (DestinationId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SetChargingProfile(this ILocalControllerNode         LocalController,
                               NetworkingNode_Id             DestinationId,
                               EVSE_Id                       EVSEId,
                               ChargingProfile               ChargingProfile,

                               CustomData?                   CustomData          = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<Signature>?       Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetChargingProfile(
                       new SetChargingProfileRequest(
                           DestinationId,
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

        #region GetChargingProfiles         (DestinationId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetChargingProfiles(this ILocalControllerNode         LocalController,
                                NetworkingNode_Id             DestinationId,
                                Int64                         GetChargingProfilesRequestId,
                                ChargingProfileCriterion      ChargingProfile,
                                EVSE_Id?                      EVSEId              = null,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<Signature>?       Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetChargingProfiles(
                       new GetChargingProfilesRequest(
                           DestinationId,
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

        #region ClearChargingProfile        (DestinationId, ChargingProfileId, ChargingProfileCriteria, ...)

        /// <summary>
        /// Remove the charging profile at the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            ClearChargingProfile(this ILocalControllerNode         LocalController,
                                 NetworkingNode_Id             DestinationId,
                                 ChargingProfile_Id?           ChargingProfileId         = null,
                                 ClearChargingProfile?         ChargingProfileCriteria   = null,

                                 CustomData?                   CustomData                = null,

                                 NetworkPath?                  NetworkPath               = null,

                                 IEnumerable<KeyPair>?         SignKeys                  = null,
                                 IEnumerable<SignInfo>?        SignInfos                 = null,
                                 IEnumerable<Signature>?       Signatures                = null,

                                 Request_Id?                   RequestId                 = null,
                                 DateTime?                     RequestTimestamp          = null,
                                 TimeSpan?                     RequestTimeout            = null,
                                 EventTracking_Id?             EventTrackingId           = null,
                                 CancellationToken             CancellationToken         = default)


                => LocalController.OCPP.OUT.ClearChargingProfile(
                       new ClearChargingProfileRequest(
                           DestinationId,
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

        #region GetCompositeSchedule        (DestinationId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            GetCompositeSchedule(this ILocalControllerNode         LocalController,
                                 NetworkingNode_Id             DestinationId,
                                 TimeSpan                      Duration,
                                 EVSE_Id                       EVSEId,
                                 ChargingRateUnits?            ChargingRateUnit    = null,

                                 CustomData?                   CustomData          = null,

                                 NetworkPath?                  NetworkPath         = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetCompositeSchedule(
                       new GetCompositeScheduleRequest(
                           DestinationId,
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

        #region UpdateDynamicSchedule       (DestinationId, ChargingProfileId, Limit = null, ...)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            UpdateDynamicSchedule(this ILocalControllerNode         LocalController,
                                  NetworkingNode_Id             DestinationId,
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
                                  IEnumerable<Signature>?       Signatures            = null,

                                  Request_Id?                   RequestId             = null,
                                  DateTime?                     RequestTimestamp      = null,
                                  TimeSpan?                     RequestTimeout        = null,
                                  EventTracking_Id?             EventTrackingId       = null,
                                  CancellationToken             CancellationToken     = default)


                => LocalController.OCPP.OUT.UpdateDynamicSchedule(
                       new UpdateDynamicScheduleRequest(

                           DestinationId,
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

        #region NotifyAllowedEnergyTransfer (DestinationId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            NotifyAllowedEnergyTransfer(this ILocalControllerNode            LocalController,
                                        NetworkingNode_Id                DestinationId,
                                        IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes,

                                        CustomData?                      CustomData          = null,

                                        NetworkPath?                     NetworkPath         = null,

                                        IEnumerable<KeyPair>?            SignKeys            = null,
                                        IEnumerable<SignInfo>?           SignInfos           = null,
                                        IEnumerable<Signature>?          Signatures          = null,

                                        Request_Id?                      RequestId           = null,
                                        DateTime?                        RequestTimestamp    = null,
                                        TimeSpan?                        RequestTimeout      = null,
                                        EventTracking_Id?                EventTrackingId     = null,
                                        CancellationToken                CancellationToken   = default)


                => LocalController.OCPP.OUT.NotifyAllowedEnergyTransfer(
                       new NotifyAllowedEnergyTransferRequest(
                           DestinationId,
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

        #region UsePriorityCharging         (DestinationId, TransactionId, Activate, ...)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            UsePriorityCharging(this ILocalControllerNode         LocalController,
                                NetworkingNode_Id             DestinationId,
                                Transaction_Id                TransactionId,
                                Boolean                       Activate,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<Signature>?       Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UsePriorityCharging(
                       new UsePriorityChargingRequest(
                           DestinationId,
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

        #region UnlockConnector             (DestinationId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            UnlockConnector(this ILocalControllerNode         LocalController,
                            NetworkingNode_Id             DestinationId,
                            EVSE_Id                       EVSEId,
                            Connector_Id                  ConnectorId,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<Signature>?       Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.UnlockConnector(
                       new UnlockConnectorRequest(
                           DestinationId,
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


        #region SendAFRRSignal              (DestinationId, ActivationTimestamp, Signal, ...)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="DestinationId">The local controller identification.</param>
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

            SendAFRRSignal(this ILocalControllerNode         LocalController,
                           NetworkingNode_Id             DestinationId,
                           DateTime                      ActivationTimestamp,
                           AFRR_Signal                   Signal,

                           CustomData?                   CustomData          = null,

                           NetworkPath?                  NetworkPath         = null,

                           IEnumerable<KeyPair>?         SignKeys            = null,
                           IEnumerable<SignInfo>?        SignInfos           = null,
                           IEnumerable<Signature>?       Signatures          = null,

                           Request_Id?                   RequestId           = null,
                           DateTime?                     RequestTimestamp    = null,
                           TimeSpan?                     RequestTimeout      = null,
                           EventTracking_Id?             EventTrackingId     = null,
                           CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.AFRRSignal(
                       new AFRRSignalRequest(
                           DestinationId,
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


        #region SetDisplayMessage           (DestinationId, Message, ...)

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

            SetDisplayMessage(this ILocalControllerNode         LocalController,
                              NetworkingNode_Id             DestinationId,
                              MessageInfo                   Message,

                              CustomData?                   CustomData          = null,

                              NetworkPath?                  NetworkPath         = null,

                              IEnumerable<KeyPair>?         SignKeys            = null,
                              IEnumerable<SignInfo>?        SignInfos           = null,
                              IEnumerable<Signature>?       Signatures          = null,

                              Request_Id?                   RequestId           = null,
                              DateTime?                     RequestTimestamp    = null,
                              TimeSpan?                     RequestTimeout      = null,
                              EventTracking_Id?             EventTrackingId     = null,
                              CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetDisplayMessage(
                       new SetDisplayMessageRequest(
                           DestinationId,
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

        #region GetDisplayMessages          (DestinationId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null, ...)

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

            GetDisplayMessages(this ILocalControllerNode            LocalController,
                               NetworkingNode_Id                DestinationId,
                               Int32                            GetDisplayMessagesRequestId,
                               IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                               MessagePriority?                 Priority            = null,
                               MessageState?                    State               = null,

                               CustomData?                      CustomData          = null,

                               NetworkPath?                     NetworkPath         = null,

                               IEnumerable<KeyPair>?            SignKeys            = null,
                               IEnumerable<SignInfo>?           SignInfos           = null,
                               IEnumerable<Signature>?          Signatures          = null,

                               Request_Id?                      RequestId           = null,
                               DateTime?                        RequestTimestamp    = null,
                               TimeSpan?                        RequestTimeout      = null,
                               EventTracking_Id?                EventTrackingId     = null,
                               CancellationToken                CancellationToken   = default)


                => LocalController.OCPP.OUT.GetDisplayMessages(
                       new GetDisplayMessagesRequest(
                           DestinationId,
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

        #region ClearDisplayMessage         (DestinationId, DisplayMessageId, ...)

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

            ClearDisplayMessage(this ILocalControllerNode         LocalController,
                                NetworkingNode_Id             DestinationId,
                                DisplayMessage_Id             DisplayMessageId,

                                CustomData?                   CustomData          = null,

                                NetworkPath?                  NetworkPath         = null,

                                IEnumerable<KeyPair>?         SignKeys            = null,
                                IEnumerable<SignInfo>?        SignInfos           = null,
                                IEnumerable<Signature>?       Signatures          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.ClearDisplayMessage(
                       new ClearDisplayMessageRequest(
                           DestinationId,
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

        #region SendCostUpdated             (DestinationId, TotalCost, TransactionId, ...)

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

            SendCostUpdated(this ILocalControllerNode         LocalController,
                            NetworkingNode_Id             DestinationId,
                            Decimal                       TotalCost,
                            Transaction_Id                TransactionId,

                            CustomData?                   CustomData          = null,

                            NetworkPath?                  NetworkPath         = null,

                            IEnumerable<KeyPair>?         SignKeys            = null,
                            IEnumerable<SignInfo>?        SignInfos           = null,
                            IEnumerable<Signature>?       Signatures          = null,

                            Request_Id?                   RequestId           = null,
                            DateTime?                     RequestTimestamp    = null,
                            TimeSpan?                     RequestTimeout      = null,
                            EventTracking_Id?             EventTrackingId     = null,
                            CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.CostUpdated(
                       new CostUpdatedRequest(
                           DestinationId,
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

        #region RequestCustomerInformation  (DestinationId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

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

            RequestCustomerInformation(this ILocalControllerNode         LocalController,
                                       NetworkingNode_Id             DestinationId,
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
                                       IEnumerable<Signature>?       Signatures            = null,

                                       Request_Id?                   RequestId             = null,
                                       DateTime?                     RequestTimestamp      = null,
                                       TimeSpan?                     RequestTimeout        = null,
                                       EventTracking_Id?             EventTrackingId       = null,
                                       CancellationToken             CancellationToken     = default)


                => LocalController.OCPP.OUT.CustomerInformation(
                       new CustomerInformationRequest(
                           DestinationId,
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



        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff    (DestinationId, ChargingTariff,          EVSEIds = null, ...)

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

            SetDefaultChargingTariff(this ILocalControllerNode         LocalController,
                                     NetworkingNode_Id             DestinationId,
                                     ChargingTariff                ChargingTariff,
                                     IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                     CustomData?                   CustomData          = null,

                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<Signature>?       Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.SetDefaultChargingTariff(
                       new SetDefaultChargingTariffRequest(
                           DestinationId,
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

        #region GetDefaultChargingTariff    (DestinationId,                          EVSEIds = null, ...)

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

            GetDefaultChargingTariff(this ILocalControllerNode         LocalController,
                                     NetworkingNode_Id             DestinationId,
                                     IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                     CustomData?                   CustomData          = null,

                                     NetworkPath?                  NetworkPath         = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<Signature>?       Signatures          = null,

                                     Request_Id?                   RequestId           = null,
                                     DateTime?                     RequestTimestamp    = null,
                                     TimeSpan?                     RequestTimeout      = null,
                                     EventTracking_Id?             EventTrackingId     = null,
                                     CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.GetDefaultChargingTariff(
                       new GetDefaultChargingTariffRequest(
                           DestinationId,
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

        #region RemoveDefaultChargingTariff (DestinationId, ChargingTariffId = null, EVSEIds = null, ...)

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

            RemoveDefaultChargingTariff(this ILocalControllerNode         LocalController,
                                        NetworkingNode_Id             DestinationId,
                                        ChargingTariff_Id?            ChargingTariffId    = null,
                                        IEnumerable<EVSE_Id>?         EVSEIds             = null,

                                        CustomData?                   CustomData          = null,

                                        NetworkPath?                  NetworkPath         = null,

                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                        IEnumerable<Signature>?       Signatures          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        CancellationToken             CancellationToken   = default)


                => LocalController.OCPP.OUT.RemoveDefaultChargingTariff(
                       new RemoveDefaultChargingTariffRequest(
                           DestinationId,
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


    }

}
