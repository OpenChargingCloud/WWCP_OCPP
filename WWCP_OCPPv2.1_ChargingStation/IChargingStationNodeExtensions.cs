﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using nts = org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// Extension methods for all charging stations nodes.
    /// </summary>
    public static class IChargingStationNodeExtensions
    {

        #region SendBootNotification                  (BootReason, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="BootReason">The the reason for sending this boot notification to the CSMS.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.BootNotificationResponse>

            SendBootNotification(this IChargingStationNode  ChargingStation,

                                 BootReason                 BootReason,

                                 CustomData?                CustomData            = null,

                                 SourceRouting?             Destination           = null,

                                 IEnumerable<KeyPair>?      SignKeys              = null,
                                 IEnumerable<SignInfo>?     SignInfos             = null,
                                 IEnumerable<Signature>?    Signatures            = null,

                                 Request_Id?                RequestId             = null,
                                 DateTime?                  RequestTimestamp      = null,
                                 TimeSpan?                  RequestTimeout        = null,
                                 EventTracking_Id?          EventTrackingId       = null,
                                 SerializationFormats?      SerializationFormat   = null,
                                 CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.BootNotification(
                       new BootNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           new ChargingStation(
                               ChargingStation.Model,
                               ChargingStation.VendorName,
                               ChargingStation.SerialNumber,
                               ChargingStation.FirmwareVersion,
                               ChargingStation.Modem,
                               ChargingStation.CustomData
                           ),
                           BootReason,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(this IChargingStationNode  ChargingStation,

                                           FirmwareStatus             Status,
                                           Int64?                     UpdateFirmwareRequestId   = null,

                                           CustomData?                CustomData                = null,

                                           SourceRouting?             Destination               = null,

                                           IEnumerable<KeyPair>?      SignKeys                  = null,
                                           IEnumerable<SignInfo>?     SignInfos                 = null,
                                           IEnumerable<Signature>?    Signatures                = null,

                                           Request_Id?                RequestId                 = null,
                                           DateTime?                  RequestTimestamp          = null,
                                           TimeSpan?                  RequestTimeout            = null,
                                           EventTracking_Id?          EventTrackingId           = null,
                                           SerializationFormats?      SerializationFormat       = null,
                                           CancellationToken          CancellationToken         = default)


                => ChargingStation.OCPP.OUT.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Status,
                           UpdateFirmwareRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(this IChargingStationNode  ChargingStation,

                                                  PublishFirmwareStatus      Status,
                                                  Int32?                     PublishFirmwareStatusNotificationRequestId,
                                                  IEnumerable<URL>?          DownloadLocations,

                                                  CustomData?                CustomData            = null,

                                                  SourceRouting?             Destination           = null,

                                                  IEnumerable<KeyPair>?      SignKeys              = null,
                                                  IEnumerable<SignInfo>?     SignInfos             = null,
                                                  IEnumerable<Signature>?    Signatures            = null,

                                                  Request_Id?                RequestId             = null,
                                                  DateTime?                  RequestTimestamp      = null,
                                                  TimeSpan?                  RequestTimeout        = null,
                                                  EventTracking_Id?          EventTrackingId       = null,
                                                  SerializationFormats?      SerializationFormat   = null,
                                                  CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.PublishFirmwareStatusNotification(
                       new PublishFirmwareStatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Status,
                           PublishFirmwareStatusNotificationRequestId,
                           DownloadLocations,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendHeartbeat                         (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.HeartbeatResponse>

            SendHeartbeat(this IChargingStationNode  ChargingStation,

                          CustomData?                CustomData            = null,

                          SourceRouting?             Destination           = null,

                          IEnumerable<KeyPair>?      SignKeys              = null,
                          IEnumerable<SignInfo>?     SignInfos             = null,
                          IEnumerable<Signature>?    Signatures            = null,

                          Request_Id?                RequestId             = null,
                          DateTime?                  RequestTimestamp      = null,
                          TimeSpan?                  RequestTimeout        = null,
                          EventTracking_Id?          EventTrackingId       = null,
                          SerializationFormats?      SerializationFormat   = null,
                          CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.Heartbeat(
                       new HeartbeatRequest(

                           Destination ?? SourceRouting.CSMS,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEventResponse>

            NotifyEvent(this IChargingStationNode  ChargingStation,

                        DateTime                   GeneratedAt,
                        UInt32                     SequenceNumber,
                        IEnumerable<EventData>     EventData,
                        Boolean?                   ToBeContinued         = null,

                        CustomData?                CustomData            = null,

                        SourceRouting?             Destination           = null,

                        IEnumerable<KeyPair>?      SignKeys              = null,
                        IEnumerable<SignInfo>?     SignInfos             = null,
                        IEnumerable<Signature>?    Signatures            = null,

                        Request_Id?                RequestId             = null,
                        DateTime?                  RequestTimestamp      = null,
                        TimeSpan?                  RequestTimeout        = null,
                        EventTracking_Id?          EventTrackingId       = null,
                        SerializationFormats?      SerializationFormat   = null,
                        CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyEvent(
                       new NotifyEventRequest(

                           Destination ?? SourceRouting.CSMS,

                           GeneratedAt,
                           SequenceNumber,
                           EventData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.SecurityEventNotificationResponse>

            SendSecurityEventNotification(this IChargingStationNode  ChargingStation,

                                          SecurityEventType          Type,
                                          DateTime                   Timestamp,
                                          String?                    TechInfo              = null,

                                          CustomData?                CustomData            = null,

                                          SourceRouting?             Destination           = null,

                                          IEnumerable<KeyPair>?      SignKeys              = null,
                                          IEnumerable<SignInfo>?     SignInfos             = null,
                                          IEnumerable<Signature>?    Signatures            = null,

                                          Request_Id?                RequestId             = null,
                                          DateTime?                  RequestTimestamp      = null,
                                          TimeSpan?                  RequestTimeout        = null,
                                          EventTracking_Id?          EventTrackingId       = null,
                                          SerializationFormats?      SerializationFormat   = null,
                                          CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.SecurityEventNotification(
                       new SecurityEventNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Type,
                           Timestamp,
                           TechInfo,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyReportResponse>

            NotifyReport(this IChargingStationNode  ChargingStation,

                         Int32                      NotifyReportRequestId,
                         UInt32                     SequenceNumber,
                         DateTime                   GeneratedAt,
                         IEnumerable<ReportData>    ReportData,
                         Boolean?                   ToBeContinued         = null,

                         CustomData?                CustomData            = null,

                         SourceRouting?             Destination           = null,

                         IEnumerable<KeyPair>?      SignKeys              = null,
                         IEnumerable<SignInfo>?     SignInfos             = null,
                         IEnumerable<Signature>?    Signatures            = null,

                         Request_Id?                RequestId             = null,
                         DateTime?                  RequestTimestamp      = null,
                         TimeSpan?                  RequestTimeout        = null,
                         EventTracking_Id?          EventTrackingId       = null,
                         SerializationFormats?      SerializationFormat   = null,
                         CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyReport(
                       new NotifyReportRequest(

                           Destination ?? SourceRouting.CSMS,

                           NotifyReportRequestId,
                           SequenceNumber,
                           GeneratedAt,
                           ReportData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyMonitoringReportResponse>

            NotifyMonitoringReport(this IChargingStationNode    ChargingStation,

                                   Int32                        NotifyMonitoringReportRequestId,
                                   UInt32                       SequenceNumber,
                                   DateTime                     GeneratedAt,
                                   IEnumerable<MonitoringData>  MonitoringData,
                                   Boolean?                     ToBeContinued         = null,

                                   CustomData?                  CustomData            = null,

                                   SourceRouting?               Destination           = null,

                                   IEnumerable<KeyPair>?        SignKeys              = null,
                                   IEnumerable<SignInfo>?       SignInfos             = null,
                                   IEnumerable<Signature>?      Signatures            = null,

                                   Request_Id?                  RequestId             = null,
                                   DateTime?                    RequestTimestamp      = null,
                                   TimeSpan?                    RequestTimeout        = null,
                                   EventTracking_Id?            EventTrackingId       = null,
                                   SerializationFormats?        SerializationFormat   = null,
                                   CancellationToken            CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyMonitoringReport(
                       new NotifyMonitoringReportRequest(

                           Destination ?? SourceRouting.CSMS,

                           NotifyMonitoringReportRequestId,
                           SequenceNumber,
                           GeneratedAt,
                           MonitoringData,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.LogStatusNotificationResponse>

            SendLogStatusNotification(this IChargingStationNode  ChargingStation,

                                      UploadLogStatus            Status,
                                      Int32?                     LogRequestId          = null,

                                      CustomData?                CustomData            = null,

                                      SourceRouting?             Destination           = null,

                                      IEnumerable<KeyPair>?      SignKeys              = null,
                                      IEnumerable<SignInfo>?     SignInfos             = null,
                                      IEnumerable<Signature>?    Signatures            = null,

                                      Request_Id?                RequestId             = null,
                                      DateTime?                  RequestTimestamp      = null,
                                      TimeSpan?                  RequestTimeout        = null,
                                      EventTracking_Id?          EventTrackingId       = null,
                                      SerializationFormats?      SerializationFormat   = null,
                                      CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.LogStatusNotification(
                       new LogStatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Status,
                           LogRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region TransferData                          (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this IChargingStationNode  ChargingStation,

                         Vendor_Id                  VendorId,
                         Message_Id?                MessageId             = null,
                         JToken?                    Data                  = null,

                         IEnumerable<KeyPair>?      SignKeys              = null,
                         IEnumerable<SignInfo>?     SignInfos             = null,
                         IEnumerable<Signature>?    Signatures            = null,

                         Request_Id?                RequestId             = null,
                         DateTime?                  RequestTimestamp      = null,
                         TimeSpan?                  RequestTimeout        = null,
                         EventTracking_Id?          EventTrackingId       = null,
                         SerializationFormats?      SerializationFormat   = null,
                         CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(

                           SourceRouting.CSMS,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region SendMessage                          (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific message to the CSMS.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SentMessageResult>

            SendMessage(this IChargingStationNode  ChargingStation,

                        Vendor_Id                  VendorId,
                        Message_Id?                MessageId             = null,
                        JToken?                    Data                  = null,

                        IEnumerable<KeyPair>?      SignKeys              = null,
                        IEnumerable<SignInfo>?     SignInfos             = null,
                        IEnumerable<Signature>?    Signatures            = null,

                        Request_Id?                RequestId             = null,
                        DateTime?                  RequestTimestamp      = null,
                        EventTracking_Id?          EventTrackingId       = null,
                        SerializationFormats?      SerializationFormat   = null,
                        CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.MessageTransfer(
                       new MessageTransferMessage(

                           SourceRouting.CSMS,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion



        #region SendCertificateSigningRequest         (SignCertificateRequestId, CSR, CertificateType = null, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="SignCertificateRequestId">A sign certificate request identification.</param>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.SignCertificateResponse>

            SendCertificateSigningRequest(this IChargingStationNode  ChargingStation,

                                          Int32                      SignCertificateRequestId,
                                          String                     CSR,
                                          CertificateSigningUse?     CertificateType       = null,

                                          CustomData?                CustomData            = null,

                                          SourceRouting?             Destination           = null,

                                          IEnumerable<KeyPair>?      SignKeys              = null,
                                          IEnumerable<SignInfo>?     SignInfos             = null,
                                          IEnumerable<Signature>?    Signatures            = null,

                                          Request_Id?                RequestId             = null,
                                          DateTime?                  RequestTimestamp      = null,
                                          TimeSpan?                  RequestTimeout        = null,
                                          EventTracking_Id?          EventTrackingId       = null,
                                          SerializationFormats?      SerializationFormat   = null,
                                          CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.SignCertificate(
                       new SignCertificateRequest(

                           Destination ?? SourceRouting.CSMS,

                           SignCertificateRequestId,
                           CSR,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.Get15118EVCertificateResponse>

            Get15118EVCertificate(this IChargingStationNode  ChargingStation,

                                  ISO15118SchemaVersion      ISO15118SchemaVersion,
                                  CertificateAction          CertificateAction,
                                  EXIData                    EXIRequest,
                                  UInt32?                    MaximumContractCertificateChains   = 1,
                                  IEnumerable<EMA_Id>?       PrioritizedEMAIds                  = null,

                                  CustomData?                CustomData                         = null,

                                  SourceRouting?             Destination                        = null,

                                  IEnumerable<KeyPair>?      SignKeys                           = null,
                                  IEnumerable<SignInfo>?     SignInfos                          = null,
                                  IEnumerable<Signature>?    Signatures                         = null,

                                  Request_Id?                RequestId                          = null,
                                  DateTime?                  RequestTimestamp                   = null,
                                  TimeSpan?                  RequestTimeout                     = null,
                                  EventTracking_Id?          EventTrackingId                    = null,
                                  SerializationFormats?      SerializationFormat                = null,
                                  CancellationToken          CancellationToken                  = default)


                => ChargingStation.OCPP.OUT.Get15118EVCertificate(
                       new Get15118EVCertificateRequest(

                           Destination ?? SourceRouting.CSMS,

                           ISO15118SchemaVersion,
                           CertificateAction,
                           EXIRequest,
                           MaximumContractCertificateChains,
                           PrioritizedEMAIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region GetCertificateStatus                  (OCSPRequestData, ...)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.GetCertificateStatusResponse>

            GetCertificateStatus(this IChargingStationNode  ChargingStation,

                                 OCSPRequestData            OCSPRequestData,

                                 CustomData?                CustomData            = null,

                                 SourceRouting?             Destination           = null,

                                 IEnumerable<KeyPair>?      SignKeys              = null,
                                 IEnumerable<SignInfo>?     SignInfos             = null,
                                 IEnumerable<Signature>?    Signatures            = null,

                                 Request_Id?                RequestId             = null,
                                 DateTime?                  RequestTimestamp      = null,
                                 TimeSpan?                  RequestTimeout        = null,
                                 EventTracking_Id?          EventTrackingId       = null,
                                 SerializationFormats?      SerializationFormat   = null,
                                 CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.GetCertificateStatus(
                       new GetCertificateStatusRequest(

                           Destination ?? SourceRouting.CSMS,

                           OCSPRequestData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region GetCRLRequest                         (GetCRLRequestId, CertificateHashData, ...)

        ///// <summary>
        ///// Get a certificate revocation list from CSMS for the specified certificate.
        ///// </summary>
        ///// 
        ///// <param name="GetCRLRequestId">The identification of this request.</param>
        ///// <param name="CertificateHashData">Certificate hash data.</param>
        ///// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<CSMS.GetCRLResponse>

        //    GetCRLRequest(this IChargingStationNode  ChargingStation,

        //                  UInt32                     GetCRLRequestId,
        //                  CertificateHashData        CertificateHashData,

        //                  CustomData?                CustomData            = null,

        //                  SourceRouting?             Destination           = null,

        //                  IEnumerable<KeyPair>?      SignKeys              = null,
        //                  IEnumerable<SignInfo>?     SignInfos             = null,
        //                  IEnumerable<Signature>?    Signatures            = null,

        //                  Request_Id?                RequestId             = null,
        //                  DateTime?                  RequestTimestamp      = null,
        //                  TimeSpan?                  RequestTimeout        = null,
        //                  EventTracking_Id?          EventTrackingId       = null,
        //                  SerializationFormats?      SerializationFormat   = null,
        //                  CancellationToken          CancellationToken     = default)


        //        => ChargingStation.OCPP.OUT.GetCRL(
        //               new GetCRLRequest(

        //                   Destination ?? SourceRouting.CSMS,

        //                   GetCRLRequestId,
        //                   CertificateHashData,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   CustomData,

        //                   RequestId        ?? ChargingStation.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath.Empty,
        //                   SerializationFormat,
        //                   CancellationToken

        //               )
        //           );

        #endregion


        #region SendReservationStatusUpdate           (ReservationId, ReservationUpdateStatus, ...)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(this IChargingStationNode  ChargingStation,

                                        Reservation_Id             ReservationId,
                                        ReservationUpdateStatus    ReservationUpdateStatus,

                                        CustomData?                CustomData            = null,

                                        SourceRouting?             Destination           = null,

                                        IEnumerable<KeyPair>?      SignKeys              = null,
                                        IEnumerable<SignInfo>?     SignInfos             = null,
                                        IEnumerable<Signature>?    Signatures            = null,

                                        Request_Id?                RequestId             = null,
                                        DateTime?                  RequestTimestamp      = null,
                                        TimeSpan?                  RequestTimeout        = null,
                                        EventTracking_Id?          EventTrackingId       = null,
                                        SerializationFormats?      SerializationFormat   = null,
                                        CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.ReservationStatusUpdate(
                       new ReservationStatusUpdateRequest(

                           Destination ?? SourceRouting.CSMS,

                           ReservationId,
                           ReservationUpdateStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region Authorize                             (IdToken, CertificateChain = null, ISO15118CertificateHashData = null, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="CertificateChain">The X.509 certificate chain presented by EV and encoded in PEM format. Order of certificates in chain is from leaf up to (but excluding) root certificate. Only needed in case of central contract validation when Charging Station cannot validate the contract certificate (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.AuthorizeResponse>

            Authorize(this IChargingStationNode      ChargingStation,

                      IdToken                        IdToken,
                      OCPP.CertificateChain?         CertificateChain              = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                      CustomData?                    CustomData                    = null,

                      SourceRouting?                 Destination                   = null,

                      IEnumerable<KeyPair>?          SignKeys                      = null,
                      IEnumerable<SignInfo>?         SignInfos                     = null,
                      IEnumerable<Signature>?        Signatures                    = null,

                      Request_Id?                    RequestId                     = null,
                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      SerializationFormats?          SerializationFormat           = null,
                      CancellationToken              CancellationToken             = default)


                => ChargingStation.OCPP.OUT.Authorize(
                       new AuthorizeRequest(

                           Destination ?? SourceRouting.CSMS,

                           IdToken,
                           CertificateChain,
                           ISO15118CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(this IChargingStationNode  ChargingStation,

                                  EVSE_Id                    EVSEId,
                                  ChargingNeeds              ChargingNeeds,
                                  DateTime?                  ReceivedTimestamp     = null,
                                  UInt16?                    MaxScheduleTuples     = null,

                                  CustomData?                CustomData            = null,

                                  SourceRouting?             Destination           = null,

                                  IEnumerable<KeyPair>?      SignKeys              = null,
                                  IEnumerable<SignInfo>?     SignInfos             = null,
                                  IEnumerable<Signature>?    Signatures            = null,

                                  Request_Id?                RequestId             = null,
                                  DateTime?                  RequestTimestamp      = null,
                                  TimeSpan?                  RequestTimeout        = null,
                                  EventTracking_Id?          EventTrackingId       = null,
                                  SerializationFormats?      SerializationFormat   = null,
                                  CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyEVChargingNeeds(
                       new NotifyEVChargingNeedsRequest(

                           Destination ?? SourceRouting.CSMS,

                           EVSEId,
                           ChargingNeeds,
                           ReceivedTimestamp,
                           MaxScheduleTuples,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.TransactionEventResponse>

            SendTransactionEvent(this IChargingStationNode  ChargingStation,

                                 TransactionEvents          EventType,
                                 DateTime                   Timestamp,
                                 TriggerReason              TriggerReason,
                                 UInt32                     SequenceNumber,
                                 Transaction                TransactionInfo,

                                 Boolean?                   Offline                 = null,
                                 Byte?                      NumberOfPhasesUsed      = null,
                                 Ampere?                    CableMaxCurrent         = null,
                                 Reservation_Id?            ReservationId           = null,
                                 IdToken?                   IdToken                 = null,
                                 EVSE?                      EVSE                    = null,
                                 IEnumerable<MeterValue>?   MeterValues             = null,
                                 PreconditioningStatus?     PreconditioningStatus   = null,

                                 CustomData?                CustomData              = null,

                                 SourceRouting?             Destination             = null,

                                 IEnumerable<KeyPair>?      SignKeys                = null,
                                 IEnumerable<SignInfo>?     SignInfos               = null,
                                 IEnumerable<Signature>?    Signatures              = null,

                                 Request_Id?                RequestId               = null,
                                 DateTime?                  RequestTimestamp        = null,
                                 TimeSpan?                  RequestTimeout          = null,
                                 EventTracking_Id?          EventTrackingId         = null,
                                 SerializationFormats?      SerializationFormat     = null,
                                 CancellationToken          CancellationToken       = default)


                => ChargingStation.OCPP.OUT.TransactionEvent(
                       new TransactionEventRequest(

                           Destination ?? SourceRouting.CSMS,

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

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.StatusNotificationResponse>

            SendStatusNotification(this IChargingStationNode  ChargingStation,

                                   EVSE_Id                    EVSEId,
                                   Connector_Id               ConnectorId,
                                   DateTime                   Timestamp,
                                   ConnectorStatus            Status,

                                   CustomData?                CustomData            = null,

                                   SourceRouting?             Destination           = null,

                                   IEnumerable<KeyPair>?      SignKeys              = null,
                                   IEnumerable<SignInfo>?     SignInfos             = null,
                                   IEnumerable<Signature>?    Signatures            = null,

                                   Request_Id?                RequestId             = null,
                                   DateTime?                  RequestTimestamp      = null,
                                   TimeSpan?                  RequestTimeout        = null,
                                   EventTracking_Id?          EventTrackingId       = null,
                                   SerializationFormats?      SerializationFormat   = null,
                                   CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.StatusNotification(
                       new StatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Timestamp,
                           Status,
                           EVSEId,
                           ConnectorId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.MeterValuesResponse>

            SendMeterValues(this IChargingStationNode  ChargingStation,

                            EVSE_Id                    EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>    MeterValues,

                            CustomData?                CustomData            = null,

                            SourceRouting?             Destination           = null,

                            IEnumerable<KeyPair>?      SignKeys              = null,
                            IEnumerable<SignInfo>?     SignInfos             = null,
                            IEnumerable<Signature>?    Signatures            = null,

                            Request_Id?                RequestId             = null,
                            DateTime?                  RequestTimestamp      = null,
                            TimeSpan?                  RequestTimeout        = null,
                            EventTracking_Id?          EventTrackingId       = null,
                            SerializationFormats?      SerializationFormat   = null,
                            CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.MeterValues(
                       new MeterValuesRequest(

                           Destination ?? SourceRouting.CSMS,

                           EVSEId,
                           MeterValues,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyChargingLimitResponse>

            NotifyChargingLimit(this IChargingStationNode      ChargingStation,

                                ChargingLimit                  ChargingLimit,
                                IEnumerable<ChargingSchedule>  ChargingSchedules,
                                EVSE_Id?                       EVSEId                = null,

                                CustomData?                    CustomData            = null,

                                SourceRouting?                 Destination           = null,

                                IEnumerable<KeyPair>?          SignKeys              = null,
                                IEnumerable<SignInfo>?         SignInfos             = null,
                                IEnumerable<Signature>?        Signatures            = null,

                                Request_Id?                    RequestId             = null,
                                DateTime?                      RequestTimestamp      = null,
                                TimeSpan?                      RequestTimeout        = null,
                                EventTracking_Id?              EventTrackingId       = null,
                                SerializationFormats?          SerializationFormat   = null,
                                CancellationToken              CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyChargingLimit(
                       new NotifyChargingLimitRequest(

                           Destination ?? SourceRouting.CSMS,

                           ChargingLimit,
                           ChargingSchedules,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ClearedChargingLimitResponse>

            SendClearedChargingLimit(this IChargingStationNode  ChargingStation,

                                     ChargingLimitSource        ChargingLimitSource,
                                     EVSE_Id?                   EVSEId,

                                     CustomData?                CustomData            = null,

                                     SourceRouting?             Destination           = null,

                                     IEnumerable<KeyPair>?      SignKeys              = null,
                                     IEnumerable<SignInfo>?     SignInfos             = null,
                                     IEnumerable<Signature>?    Signatures            = null,

                                     Request_Id?                RequestId             = null,
                                     DateTime?                  RequestTimestamp      = null,
                                     TimeSpan?                  RequestTimeout        = null,
                                     EventTracking_Id?          EventTrackingId       = null,
                                     SerializationFormats?      SerializationFormat   = null,
                                     CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.ClearedChargingLimit(
                       new ClearedChargingLimitRequest(

                           Destination ?? SourceRouting.CSMS,

                           ChargingLimitSource,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.ReportChargingProfilesResponse>

            ReportChargingProfiles(this IChargingStationNode     ChargingStation,

                                   Int32                         ReportChargingProfilesRequestId,
                                   ChargingLimitSource           ChargingLimitSource,
                                   EVSE_Id                       EVSEId,
                                   IEnumerable<ChargingProfile>  ChargingProfiles,
                                   Boolean?                      ToBeContinued         = null,

                                   CustomData?                   CustomData            = null,

                                   SourceRouting?                Destination           = null,

                                   IEnumerable<KeyPair>?         SignKeys              = null,
                                   IEnumerable<SignInfo>?        SignInfos             = null,
                                   IEnumerable<Signature>?       Signatures            = null,

                                   Request_Id?                   RequestId             = null,
                                   DateTime?                     RequestTimestamp      = null,
                                   TimeSpan?                     RequestTimeout        = null,
                                   EventTracking_Id?             EventTrackingId       = null,
                                   SerializationFormats?         SerializationFormat   = null,
                                   CancellationToken             CancellationToken     = default)


                => ChargingStation.OCPP.OUT.ReportChargingProfiles(
                       new ReportChargingProfilesRequest(

                           Destination ?? SourceRouting.CSMS,

                           ReportChargingProfilesRequestId,
                           ChargingLimitSource,
                           EVSEId,
                           ChargingProfiles,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEVChargingScheduleResponse>

            NotifyEVChargingSchedule(this IChargingStationNode  ChargingStation,

                                     DateTime                   TimeBase,
                                     EVSE_Id                    EVSEId,
                                     ChargingSchedule           ChargingSchedule,
                                     Byte?                      SelectedScheduleTupleId    = null,
                                     Boolean?                   PowerToleranceAcceptance   = null,

                                     CustomData?                CustomData                 = null,

                                     SourceRouting?             Destination                = null,

                                     IEnumerable<KeyPair>?      SignKeys                   = null,
                                     IEnumerable<SignInfo>?     SignInfos                  = null,
                                     IEnumerable<Signature>?    Signatures                 = null,

                                     Request_Id?                RequestId                  = null,
                                     DateTime?                  RequestTimestamp           = null,
                                     TimeSpan?                  RequestTimeout             = null,
                                     EventTracking_Id?          EventTrackingId            = null,
                                     SerializationFormats?      SerializationFormat        = null,
                                     CancellationToken          CancellationToken          = default)


                => ChargingStation.OCPP.OUT.NotifyEVChargingSchedule(
                       new NotifyEVChargingScheduleRequest(

                           Destination ?? SourceRouting.CSMS,

                           TimeBase,
                           EVSEId,
                           ChargingSchedule,
                           SelectedScheduleTupleId,
                           PowerToleranceAcceptance,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyPriorityChargingResponse>

            NotifyPriorityCharging(this IChargingStationNode  ChargingStation,

                                   Transaction_Id             TransactionId,
                                   Boolean                    Activated,

                                   CustomData?                CustomData            = null,

                                   SourceRouting?             Destination           = null,

                                   IEnumerable<KeyPair>?      SignKeys              = null,
                                   IEnumerable<SignInfo>?     SignInfos             = null,
                                   IEnumerable<Signature>?    Signatures            = null,

                                   Request_Id?                RequestId             = null,
                                   DateTime?                  RequestTimestamp      = null,
                                   TimeSpan?                  RequestTimeout        = null,
                                   EventTracking_Id?          EventTrackingId       = null,
                                   SerializationFormats?      SerializationFormat   = null,
                                   CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyPriorityCharging(
                       new NotifyPriorityChargingRequest(

                           Destination ?? SourceRouting.CSMS,

                           TransactionId,
                           Activated,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifySettlement                      (PaymentReference, PaymentStatus, SettlementAmount, SettlementTimestamp, ...)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// 
        /// <param name="PaymentReference">The payment reference received from the payment terminal and is used as the value for idToken.</param>
        /// <param name="PaymentStatus">The status of the settlement attempt.</param>
        /// <param name="SettlementAmount">The amount that was settled, or attempted to be settled (in case of failure).</param>
        /// <param name="SettlementTimestamp">The time when the settlement was done.</param>
        /// 
        /// <param name="TransactionId">The optional transaction for which priority charging is requested.</param>
        /// <param name="StatusInfo">Optional additional information from payment terminal/payment process.</param>
        /// <param name="ReceiptId">The optional receipt id, to be used if the receipt is generated by the payment terminal or the charging station.</param>
        /// <param name="ReceiptURL">The optional receipt URL, to be used if the receipt is generated by the payment terminal or the charging station.</param>
        /// <param name="VATCompany">The optional company contact for a company receipt.</param>
        /// <param name="VATNumber">The optional VAT number for a company receipt.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifySettlementResponse>

            NotifySettlement(this IChargingStationNode  ChargingStation,

                             PaymentReference           PaymentReference,
                             PaymentStatus              PaymentStatus,
                             Decimal                    SettlementAmount,
                             DateTime                   SettlementTimestamp,

                             Transaction_Id?            TransactionId         = null,
                             String?                    StatusInfo            = null,
                             ReceiptId?                 ReceiptId             = null,
                             URL?                       ReceiptURL            = null,
                             Contact?                   VATCompany            = null,
                             String?                    VATNumber             = null,

                             CustomData?                CustomData            = null,

                             SourceRouting?             Destination           = null,

                             IEnumerable<KeyPair>?      SignKeys              = null,
                             IEnumerable<SignInfo>?     SignInfos             = null,
                             IEnumerable<Signature>?    Signatures            = null,

                             Request_Id?                RequestId             = null,
                             DateTime?                  RequestTimestamp      = null,
                             TimeSpan?                  RequestTimeout        = null,
                             EventTracking_Id?          EventTrackingId       = null,
                             SerializationFormats?      SerializationFormat   = null,
                             CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifySettlement(
                       new NotifySettlementRequest(

                           Destination ?? SourceRouting.CSMS,

                           PaymentReference,
                           PaymentStatus,
                           SettlementAmount,
                           SettlementTimestamp,

                           TransactionId,
                           StatusInfo,
                           ReceiptId,
                           ReceiptURL,
                           VATCompany,
                           VATNumber,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion


        #region PullDynamicScheduleUpdate             (ChargingProfileId, ...)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.PullDynamicScheduleUpdateResponse>

            PullDynamicScheduleUpdate(this IChargingStationNode  ChargingStation,

                                      ChargingProfile_Id         ChargingProfileId,

                                      CustomData?                CustomData            = null,

                                      SourceRouting?             Destination           = null,

                                      IEnumerable<KeyPair>?      SignKeys              = null,
                                      IEnumerable<SignInfo>?     SignInfos             = null,
                                      IEnumerable<Signature>?    Signatures            = null,

                                      Request_Id?                RequestId             = null,
                                      DateTime?                  RequestTimestamp      = null,
                                      TimeSpan?                  RequestTimeout        = null,
                                      EventTracking_Id?          EventTrackingId       = null,
                                      SerializationFormats?      SerializationFormat   = null,
                                      CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.PullDynamicScheduleUpdate(
                       new PullDynamicScheduleUpdateRequest(

                           Destination ?? SourceRouting.CSMS,

                           ChargingProfileId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(this IChargingStationNode  ChargingStation,

                                  Int32                      NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>   MessageInfos,
                                  Boolean?                   ToBeContinued         = null,

                                  CustomData?                CustomData            = null,

                                  SourceRouting?             Destination           = null,

                                  IEnumerable<KeyPair>?      SignKeys              = null,
                                  IEnumerable<SignInfo>?     SignInfos             = null,
                                  IEnumerable<Signature>?    Signatures            = null,

                                  Request_Id?                RequestId             = null,
                                  DateTime?                  RequestTimestamp      = null,
                                  TimeSpan?                  RequestTimeout        = null,
                                  EventTracking_Id?          EventTrackingId       = null,
                                  SerializationFormats?      SerializationFormat   = null,
                                  CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyDisplayMessages(
                       new NotifyDisplayMessagesRequest(

                           Destination ?? SourceRouting.CSMS,

                           NotifyDisplayMessagesRequestId,
                           MessageInfos,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyCustomerInformationResponse>

            NotifyCustomerInformation(this IChargingStationNode  ChargingStation,

                                      Int64                      NotifyCustomerInformationRequestId,
                                      String                     Data,
                                      UInt32                     SequenceNumber,
                                      DateTime                   GeneratedAt,
                                      Boolean?                   ToBeContinued         = null,

                                      CustomData?                CustomData            = null,

                                      SourceRouting?             Destination           = null,

                                      IEnumerable<KeyPair>?      SignKeys              = null,
                                      IEnumerable<SignInfo>?     SignInfos             = null,
                                      IEnumerable<Signature>?    Signatures            = null,

                                      Request_Id?                RequestId             = null,
                                      DateTime?                  RequestTimestamp      = null,
                                      TimeSpan?                  RequestTimeout        = null,
                                      EventTracking_Id?          EventTrackingId       = null,
                                      SerializationFormats?      SerializationFormat   = null,
                                      CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NotifyCustomerInformation(
                       new NotifyCustomerInformationRequest(

                           Destination ?? SourceRouting.CSMS,

                           NotifyCustomerInformationRequestId,
                           Data,
                           SequenceNumber,
                           GeneratedAt,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion


        #region NTSKE                                 (AEADAlgorithm = null, ...)

        /// <summary>
        /// Request NTS-KE server information.
        /// </summary>
        /// <param name="AEADAlgorithm">The optional AEAD algorithm to be used for the Network Time Secure Key Exchange (default: AES_SIV_CMAC_256).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NTSKEResponse>

            NTSKE(this IChargingStationNode  ChargingStation,

                  nts.AEADAlgorithms?        AEADAlgorithm         = null,
                  CustomData?                CustomData            = null,

                  SourceRouting?             Destination           = null,

                  IEnumerable<KeyPair>?      SignKeys              = null,
                  IEnumerable<SignInfo>?     SignInfos             = null,
                  IEnumerable<Signature>?    Signatures            = null,

                  Request_Id?                RequestId             = null,
                  DateTime?                  RequestTimestamp      = null,
                  TimeSpan?                  RequestTimeout        = null,
                  EventTracking_Id?          EventTrackingId       = null,
                  SerializationFormats?      SerializationFormat   = null,
                  CancellationToken          CancellationToken     = default)


                => ChargingStation.OCPP.OUT.NTSKE(
                       new NTSKERequest(

                           Destination ?? SourceRouting.CSMS,

                           AEADAlgorithm,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion


    }

}
