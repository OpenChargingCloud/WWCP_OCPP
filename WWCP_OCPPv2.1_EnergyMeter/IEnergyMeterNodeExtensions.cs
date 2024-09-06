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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.EM
{

    /// <summary>
    /// Extension methods for all energy meter nodes.
    /// </summary>
    public static class IEnergyMeterNodeExtensions
    {

        #region SendBootNotification                  (BootReason, ...)

        /// <summary>
        /// Send a boot notification.
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

            SendBootNotification(this IEnergyMeterNode    EnergyMeter,

                                 BootReason               BootReason,

                                 CustomData?              CustomData            = null,

                                 SourceRouting?           Destination           = null,

                                 IEnumerable<KeyPair>?    SignKeys              = null,
                                 IEnumerable<SignInfo>?   SignInfos             = null,
                                 IEnumerable<Signature>?  Signatures            = null,

                                 Request_Id?              RequestId             = null,
                                 DateTime?                RequestTimestamp      = null,
                                 TimeSpan?                RequestTimeout        = null,
                                 EventTracking_Id?        EventTrackingId       = null,
                                 SerializationFormats?    SerializationFormat   = null,
                                 CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.BootNotification(
                       new BootNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           new ChargingStation(
                               EnergyMeter.Model,
                               EnergyMeter.VendorName,
                               EnergyMeter.SerialNumber,
                               EnergyMeter.FirmwareVersion,
                               EnergyMeter.Modem,
                               EnergyMeter.CustomData
                           ),
                           BootReason,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(this IEnergyMeterNode    EnergyMeter,

                                           FirmwareStatus           Status,
                                           Int64?                   UpdateFirmwareRequestId   = null,

                                           CustomData?              CustomData                = null,

                                           SourceRouting?           Destination               = null,

                                           IEnumerable<KeyPair>?    SignKeys                  = null,
                                           IEnumerable<SignInfo>?   SignInfos                 = null,
                                           IEnumerable<Signature>?  Signatures                = null,

                                           Request_Id?              RequestId                 = null,
                                           DateTime?                RequestTimestamp          = null,
                                           TimeSpan?                RequestTimeout            = null,
                                           EventTracking_Id?        EventTrackingId           = null,
                                           SerializationFormats?    SerializationFormat       = null,
                                           CancellationToken        CancellationToken         = default)


                => EnergyMeter.OCPP.OUT.FirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Status,
                           UpdateFirmwareRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
                           SerializationFormat,
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

            SendHeartbeat(this IEnergyMeterNode    EnergyMeter,

                          CustomData?              CustomData            = null,

                          SourceRouting?           Destination           = null,

                          IEnumerable<KeyPair>?    SignKeys              = null,
                          IEnumerable<SignInfo>?   SignInfos             = null,
                          IEnumerable<Signature>?  Signatures            = null,

                          Request_Id?              RequestId             = null,
                          DateTime?                RequestTimestamp      = null,
                          TimeSpan?                RequestTimeout        = null,
                          EventTracking_Id?        EventTrackingId       = null,
                          SerializationFormats?    SerializationFormat   = null,
                          CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.Heartbeat(
                       new HeartbeatRequest(

                           Destination ?? SourceRouting.CSMS,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyEventResponse>

            NotifyEvent(this IEnergyMeterNode    EnergyMeter,

                        DateTime                 GeneratedAt,
                        UInt32                   SequenceNumber,
                        IEnumerable<EventData>   EventData,
                        Boolean?                 ToBeContinued         = null,

                        CustomData?              CustomData            = null,

                        SourceRouting?           Destination           = null,

                        IEnumerable<KeyPair>?    SignKeys              = null,
                        IEnumerable<SignInfo>?   SignInfos             = null,
                        IEnumerable<Signature>?  Signatures            = null,

                        Request_Id?              RequestId             = null,
                        DateTime?                RequestTimestamp      = null,
                        TimeSpan?                RequestTimeout        = null,
                        EventTracking_Id?        EventTrackingId       = null,
                        SerializationFormats?    SerializationFormat   = null,
                        CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.NotifyEvent(
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

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.SecurityEventNotificationResponse>

            SendSecurityEventNotification(this IEnergyMeterNode    EnergyMeter,

                                          SecurityEventType        Type,
                                          DateTime                 Timestamp,
                                          String?                  TechInfo              = null,

                                          CustomData?              CustomData            = null,

                                          SourceRouting?           Destination           = null,

                                          IEnumerable<KeyPair>?    SignKeys              = null,
                                          IEnumerable<SignInfo>?   SignInfos             = null,
                                          IEnumerable<Signature>?  Signatures            = null,

                                          Request_Id?              RequestId             = null,
                                          DateTime?                RequestTimestamp      = null,
                                          TimeSpan?                RequestTimeout        = null,
                                          EventTracking_Id?        EventTrackingId       = null,
                                          SerializationFormats?    SerializationFormat   = null,
                                          CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.SecurityEventNotification(
                       new SecurityEventNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Type,
                           Timestamp,
                           TechInfo,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyReportResponse>

            NotifyReport(this IEnergyMeterNode    EnergyMeter,

                         Int32                    NotifyReportRequestId,
                         UInt32                   SequenceNumber,
                         DateTime                 GeneratedAt,
                         IEnumerable<ReportData>  ReportData,
                         Boolean?                 ToBeContinued         = null,

                         CustomData?              CustomData            = null,

                         SourceRouting?           Destination           = null,

                         IEnumerable<KeyPair>?    SignKeys              = null,
                         IEnumerable<SignInfo>?   SignInfos             = null,
                         IEnumerable<Signature>?  Signatures            = null,

                         Request_Id?              RequestId             = null,
                         DateTime?                RequestTimestamp      = null,
                         TimeSpan?                RequestTimeout        = null,
                         EventTracking_Id?        EventTrackingId       = null,
                         SerializationFormats?    SerializationFormat   = null,
                         CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.NotifyReport(
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

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyMonitoringReportResponse>

            NotifyMonitoringReport(this IEnergyMeterNode        EnergyMeter,

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


                => EnergyMeter.OCPP.OUT.NotifyMonitoringReport(
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

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.LogStatusNotificationResponse>

            SendLogStatusNotification(this IEnergyMeterNode    EnergyMeter,

                                      UploadLogStatus          Status,
                                      Int32?                   LogRequestId          = null,

                                      CustomData?              CustomData            = null,

                                      SourceRouting?           Destination           = null,

                                      IEnumerable<KeyPair>?    SignKeys              = null,
                                      IEnumerable<SignInfo>?   SignInfos             = null,
                                      IEnumerable<Signature>?  Signatures            = null,

                                      Request_Id?              RequestId             = null,
                                      DateTime?                RequestTimestamp      = null,
                                      TimeSpan?                RequestTimeout        = null,
                                      EventTracking_Id?        EventTrackingId       = null,
                                      SerializationFormats?    SerializationFormat   = null,
                                      CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.LogStatusNotification(
                       new LogStatusNotificationRequest(

                           Destination ?? SourceRouting.CSMS,

                           Status,
                           LogRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this IEnergyMeterNode    EnergyMeter,

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


                => EnergyMeter.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(

                           SourceRouting.CSMS,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.MeterValuesResponse>

            SendMeterValues(this IEnergyMeterNode    EnergyMeter,

                            EVSE_Id                  EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>  MeterValues,

                            CustomData?              CustomData            = null,

                            SourceRouting?           Destination           = null,

                            IEnumerable<KeyPair>?    SignKeys              = null,
                            IEnumerable<SignInfo>?   SignInfos             = null,
                            IEnumerable<Signature>?  Signatures            = null,

                            Request_Id?              RequestId             = null,
                            DateTime?                RequestTimestamp      = null,
                            TimeSpan?                RequestTimeout        = null,
                            EventTracking_Id?        EventTrackingId       = null,
                            SerializationFormats?    SerializationFormat   = null,
                            CancellationToken        CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.MeterValues(
                       new MeterValuesRequest(

                           Destination ?? SourceRouting.CSMS,

                           EVSEId,
                           MeterValues,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
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
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(this IEnergyMeterNode     EnergyMeter,

                                  Int32                     NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>  MessageInfos,
                                  Boolean?                  ToBeContinued         = null,

                                  CustomData?               CustomData            = null,

                                  SourceRouting?            Destination           = null,

                                  IEnumerable<KeyPair>?     SignKeys              = null,
                                  IEnumerable<SignInfo>?    SignInfos             = null,
                                  IEnumerable<Signature>?   Signatures            = null,

                                  Request_Id?               RequestId             = null,
                                  DateTime?                 RequestTimestamp      = null,
                                  TimeSpan?                 RequestTimeout        = null,
                                  EventTracking_Id?         EventTrackingId       = null,
                                  SerializationFormats?     SerializationFormat   = null,
                                  CancellationToken         CancellationToken     = default)


                => EnergyMeter.OCPP.OUT.NotifyDisplayMessages(
                       new NotifyDisplayMessagesRequest(

                           Destination ?? SourceRouting.CSMS,

                           NotifyDisplayMessagesRequestId,
                           MessageInfos,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? EnergyMeter.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? EnergyMeter.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(EnergyMeter.Id),
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion


    }

}
