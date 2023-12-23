///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod.HTTP;

//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS.Extensions
//{

//    /// <summary>
//    /// Extension methods for all charging stations
//    /// </summary>
//    public static class INetworkingNodeExtensions
//    {

//        #region SendBootNotification                  (BootReason, ...)

//        /// <summary>
//        /// Send a boot notification.
//        /// </summary>
//        /// <param name="BootReason">The the reason for sending this boot notification to the CSMS.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.BootNotificationResponse>

//            SendBootNotification(this INetworkingNode                                       NetworkingNode,

//                                 BootReason                                                 BootReason,

//                                 IEnumerable<OCPP.Signature>?                               Signatures                                = null,
//                                 CustomData?                                                CustomData                                = null,

//                                 Request_Id?                                                RequestId                                 = null,
//                                 DateTime?                                                  RequestTimestamp                          = null,
//                                 TimeSpan?                                                  RequestTimeout                            = null,
//                                 EventTracking_Id?                                          EventTrackingId                           = null,

//                                 IEnumerable<KeyPair>?                                      SignKeys                                  = null,
//                                 IEnumerable<SignInfo>?                                     SignInfos                                 = null,
//                                 CustomJObjectSerializerDelegate<OCPPv2_1.CS.BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
//                                 CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer           = null,
//                                 CustomJObjectSerializerDelegate<OCPP.Signature>?           CustomSignatureSerializer                 = null,
//                                 CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null,

//                                 CancellationToken                                          CancellationToken                         = default)


//                => NetworkingNode.BootNotification(
//                       new OCPPv2_1.CS.BootNotificationRequest(
//                           NetworkingNode.Id,
//                           new ChargingStation(
//                               NetworkingNode.Model,
//                               NetworkingNode.VendorName,
//                               NetworkingNode.SerialNumber,
//                               NetworkingNode.Modem,
//                               NetworkingNode.FirmwareVersion,
//                               NetworkingNode.CustomData
//                           ),
//                           BootReason,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,
//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//            //var signaturePolicy = SignaturePolicy ?? NetworkingNode.SignaturePolicy;

//            //IEnumerable<SignaturePolicyEntry>? signaturePolicyEntries = null;

//            //if ((SignKeys        is not null && SignKeys.       Any()) ||
//            //    (SignInfos       is not null && SignInfos.      Any()) ||
//            //    (signaturePolicy is not null && signaturePolicy.Has(BootNotificationRequest.DefaultJSONLDContext,
//            //                                                        out signaturePolicyEntries)))
//            //{

//            //    var signInfos = new List<SignInfo>();

//            //    if (SignInfos is not null && SignInfos.Any())
//            //        signInfos.AddRange(SignInfos);

//            //    if (SignKeys  is not null && SignKeys. Any())
//            //        signInfos.AddRange(SignKeys.Select(signKey => signKey.ToSignInfo()));

//            //    if (signaturePolicyEntries is not null && signaturePolicyEntries.Any())
//            //    {
//            //        foreach (var signaturePolicyEntry in signaturePolicyEntries)
//            //        {
//            //            if (signaturePolicyEntry.KeyPair is not null)
//            //                signInfos.Add(signaturePolicyEntry.KeyPair.ToSignInfo());
//            //        }
//            //    }

//            //    if (!CryptoUtils.SignRequestMessage(
//            //            bootNotificationRequest,
//            //            bootNotificationRequest.ToJSON(
//            //                CustomBootNotificationRequestSerializer ?? NetworkingNode.CustomBootNotificationRequestSerializer,
//            //                CustomNetworkingNodeSerializer         ?? NetworkingNode.CustomNetworkingNodeSerializer,
//            //                CustomSignatureSerializer               ?? NetworkingNode.CustomSignatureSerializer,
//            //                CustomCustomDataSerializer              ?? NetworkingNode.CustomCustomDataSerializer
//            //            ),
//            //            BootNotificationRequest.DefaultJSONLDContext,
//            //            SignaturePolicy,
//            //            out var errorResponse,
//            //            signInfos.ToArray()))
//            //    {

//            //        return Task.FromResult(
//            //                   new CSMS.BootNotificationResponse(
//            //                       bootNotificationRequest,
//            //                       Result.SignatureError(errorResponse)
//            //                   )
//            //               );

//            //    }

//            //}

//        #endregion

//        #region SendFirmwareStatusNotification        (Status, ...)

//        /// <summary>
//        /// Send a firmware status notification to the CSMS.
//        /// </summary>
//        /// <param name="Status">The status of the firmware installation.</param>
//        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>

//            SendFirmwareStatusNotification(this INetworkingNode     NetworkingNode,

//                                           FirmwareStatus           Status,
//                                           Int64?                   UpdateFirmwareRequestId   = null,

//                                           IEnumerable<KeyPair>?    SignKeys                  = null,
//                                           IEnumerable<SignInfo>?   SignInfos                 = null,
//                                           IEnumerable<OCPP.Signature>?  Signatures                = null,

//                                           CustomData?              CustomData                = null,

//                                           Request_Id?              RequestId                 = null,
//                                           DateTime?                RequestTimestamp          = null,
//                                           TimeSpan?                RequestTimeout            = null,
//                                           EventTracking_Id?        EventTrackingId           = null,
//                                           CancellationToken        CancellationToken         = default)


//                => NetworkingNode.FirmwareStatusNotification(
//                       new OCPPv2_1.CS.FirmwareStatusNotificationRequest(
//                           NetworkingNode.Id,
//                           Status,
//                           UpdateFirmwareRequestId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendPublishFirmwareStatusNotification (Status, PublishFirmwareStatusNotificationRequestId, DownloadLocations, ...)

//        /// <summary>
//        /// Send a publish firmware status notification to the CSMS.
//        /// </summary>
//        /// <param name="Status">The progress status of the publish firmware request.</param>
//        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
//        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>

//            SendPublishFirmwareStatusNotification(this INetworkingNode     NetworkingNode,

//                                                  PublishFirmwareStatus    Status,
//                                                  Int32?                   PublishFirmwareStatusNotificationRequestId,
//                                                  IEnumerable<URL>?        DownloadLocations,

//                                                  IEnumerable<KeyPair>?    SignKeys            = null,
//                                                  IEnumerable<SignInfo>?   SignInfos           = null,
//                                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                                  CustomData?              CustomData          = null,

//                                                  Request_Id?              RequestId           = null,
//                                                  DateTime?                RequestTimestamp    = null,
//                                                  TimeSpan?                RequestTimeout      = null,
//                                                  EventTracking_Id?        EventTrackingId     = null,
//                                                  CancellationToken        CancellationToken   = default)


//                => NetworkingNode.PublishFirmwareStatusNotification(
//                       new OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest(
//                           NetworkingNode.Id,
//                           Status,
//                           PublishFirmwareStatusNotificationRequestId,
//                           DownloadLocations,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendHeartbeat                         (...)

//        /// <summary>
//        /// Send a heartbeat.
//        /// </summary>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.HeartbeatResponse>

//            SendHeartbeat(this INetworkingNode     NetworkingNode,


//                          IEnumerable<KeyPair>?    SignKeys            = null,
//                          IEnumerable<SignInfo>?   SignInfos           = null,
//                          IEnumerable<OCPP.Signature>?  Signatures          = null,

//                          CustomData?              CustomData          = null,

//                          Request_Id?              RequestId           = null,
//                          DateTime?                RequestTimestamp    = null,
//                          TimeSpan?                RequestTimeout      = null,
//                          EventTracking_Id?        EventTrackingId     = null,
//                          CancellationToken        CancellationToken   = default)


//                => NetworkingNode.Heartbeat(
//                       new OCPPv2_1.CS.HeartbeatRequest(
//                           NetworkingNode.Id,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyEvent                           (GeneratedAt, SequenceNumber, EventData, ToBeContinued = null, ...)

//        /// <summary>
//        /// Notify about an event.
//        /// </summary>
//        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
//        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
//        /// <param name="EventData">The enumeration of event data.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyEventResponse>

//            NotifyEvent(this INetworkingNode     NetworkingNode,

//                        DateTime                 GeneratedAt,
//                        UInt32                   SequenceNumber,
//                        IEnumerable<EventData>   EventData,
//                        Boolean?                 ToBeContinued       = null,

//                        IEnumerable<KeyPair>?    SignKeys            = null,
//                        IEnumerable<SignInfo>?   SignInfos           = null,
//                        IEnumerable<OCPP.Signature>?  Signatures          = null,

//                        CustomData?              CustomData          = null,

//                        Request_Id?              RequestId           = null,
//                        DateTime?                RequestTimestamp    = null,
//                        TimeSpan?                RequestTimeout      = null,
//                        EventTracking_Id?        EventTrackingId     = null,
//                        CancellationToken        CancellationToken   = default)


//                => NetworkingNode.NotifyEvent(
//                       new OCPPv2_1.CS.NotifyEventRequest(
//                           NetworkingNode.Id,
//                           GeneratedAt,
//                           SequenceNumber,
//                           EventData,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendSecurityEventNotification         (Type, Timestamp, TechInfo = null, TechInfo = null, ...)

//        /// <summary>
//        /// Send a security event notification.
//        /// </summary>
//        /// <param name="Type">Type of the security event.</param>
//        /// <param name="Timestamp">The timestamp of the security event.</param>
//        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.SecurityEventNotificationResponse>

//            SendSecurityEventNotification(this INetworkingNode     NetworkingNode,

//                                          SecurityEventType        Type,
//                                          DateTime                 Timestamp,
//                                          String?                  TechInfo            = null,

//                                          IEnumerable<KeyPair>?    SignKeys            = null,
//                                          IEnumerable<SignInfo>?   SignInfos           = null,
//                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                          CustomData?              CustomData          = null,

//                                          Request_Id?              RequestId           = null,
//                                          DateTime?                RequestTimestamp    = null,
//                                          TimeSpan?                RequestTimeout      = null,
//                                          EventTracking_Id?        EventTrackingId     = null,
//                                          CancellationToken        CancellationToken   = default)


//                => NetworkingNode.SecurityEventNotification(
//                       new OCPPv2_1.CS.SecurityEventNotificationRequest(
//                           NetworkingNode.Id,
//                           Type,
//                           Timestamp,
//                           TechInfo,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyReport                          (SequenceNumber, GeneratedAt, ReportData, ...)

//        /// <summary>
//        /// Notify about a report.
//        /// </summary>
//        /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
//        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
//        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
//        /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyReportResponse>

//            NotifyReport(this INetworkingNode     NetworkingNode,

//                         Int32                    NotifyReportRequestId,
//                         UInt32                   SequenceNumber,
//                         DateTime                 GeneratedAt,
//                         IEnumerable<ReportData>  ReportData,
//                         Boolean?                 ToBeContinued       = null,

//                         IEnumerable<KeyPair>?    SignKeys            = null,
//                         IEnumerable<SignInfo>?   SignInfos           = null,
//                         IEnumerable<OCPP.Signature>?  Signatures          = null,

//                         CustomData?              CustomData          = null,

//                         Request_Id?              RequestId           = null,
//                         DateTime?                RequestTimestamp    = null,
//                         TimeSpan?                RequestTimeout      = null,
//                         EventTracking_Id?        EventTrackingId     = null,
//                         CancellationToken        CancellationToken   = default)


//                => NetworkingNode.NotifyReport(
//                       new OCPPv2_1.CS.NotifyReportRequest(
//                           NetworkingNode.Id,
//                           NotifyReportRequestId,
//                           SequenceNumber,
//                           GeneratedAt,
//                           ReportData,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyMonitoringReport                (NotifyMonitoringReportRequestId, SequenceNumber, GeneratedAt, MonitoringData, ToBeContinued = null, ...)

//        /// <summary>
//        /// Notify about a monitoring report.
//        /// </summary>
//        /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
//        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
//        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
//        /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>

//            NotifyMonitoringReport(this INetworkingNode         NetworkingNode,

//                                   Int32                        NotifyMonitoringReportRequestId,
//                                   UInt32                       SequenceNumber,
//                                   DateTime                     GeneratedAt,
//                                   IEnumerable<MonitoringData>  MonitoringData,
//                                   Boolean?                     ToBeContinued       = null,

//                                   IEnumerable<KeyPair>?        SignKeys            = null,
//                                   IEnumerable<SignInfo>?       SignInfos           = null,
//                                   IEnumerable<OCPP.Signature>? Signatures          = null,

//                                   CustomData?                  CustomData          = null,

//                                   Request_Id?                  RequestId           = null,
//                                   DateTime?                    RequestTimestamp    = null,
//                                   TimeSpan?                    RequestTimeout      = null,
//                                   EventTracking_Id?            EventTrackingId     = null,
//                                   CancellationToken            CancellationToken   = default)


//                => NetworkingNode.NotifyMonitoringReport(
//                       new OCPPv2_1.CS.NotifyMonitoringReportRequest(
//                           NetworkingNode.Id,
//                           NotifyMonitoringReportRequestId,
//                           SequenceNumber,
//                           GeneratedAt,
//                           MonitoringData,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendLogStatusNotification             (Status, LogRequestId = null, ...)

//        /// <summary>
//        /// Send a log status notification.
//        /// </summary>
//        /// <param name="Status">The status of the log upload.</param>
//        /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.LogStatusNotificationResponse>

//            SendLogStatusNotification(this INetworkingNode     NetworkingNode,

//                                      UploadLogStatus          Status,
//                                      Int32?                   LogRequestId        = null,

//                                      IEnumerable<KeyPair>?    SignKeys            = null,
//                                      IEnumerable<SignInfo>?   SignInfos           = null,
//                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                      CustomData?              CustomData          = null,

//                                      Request_Id?              RequestId           = null,
//                                      DateTime?                RequestTimestamp    = null,
//                                      TimeSpan?                RequestTimeout      = null,
//                                      EventTracking_Id?        EventTrackingId     = null,
//                                      CancellationToken        CancellationToken   = default)


//                => NetworkingNode.LogStatusNotification(
//                       new OCPPv2_1.CS.LogStatusNotificationRequest(
//                           NetworkingNode.Id,
//                           Status,
//                           LogRequestId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region TransferData                          (VendorId, MessageId = null, Data = null, ...)

//        /// <summary>
//        /// Send the given vendor-specific data to the CSMS.
//        /// </summary>
//        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
//        /// <param name="MessageId">An optional message identification.</param>
//        /// <param name="Data">A vendor-specific JSON token.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.DataTransferResponse>

//            TransferData(this INetworkingNode          NetworkingNode,

//                         Vendor_Id                     VendorId,
//                         Message_Id?                   MessageId           = null,
//                         JToken?                       Data                = null,

//                         IEnumerable<KeyPair>?         SignKeys            = null,
//                         IEnumerable<SignInfo>?        SignInfos           = null,
//                         IEnumerable<OCPP.Signature>?  Signatures          = null,

//                         CustomData?                   CustomData          = null,

//                         Request_Id?                   RequestId           = null,
//                         DateTime?                     RequestTimestamp    = null,
//                         TimeSpan?                     RequestTimeout      = null,
//                         EventTracking_Id?             EventTrackingId     = null,
//                         CancellationToken             CancellationToken   = default)


//                => NetworkingNode.DataTransfer(
//                       new OCPPv2_1.CS.DataTransferRequest(
//                           NetworkingNode.Id,
//                           VendorId,
//                           MessageId,
//                           Data,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion


//        #region SendCertificateSigningRequest         (CSR, SignCertificateRequestId, CertificateType = null, ...)

//        /// <summary>
//        /// Send a heartbeat.
//        /// </summary>
//        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
//        /// <param name="SignCertificateRequestId">A sign certificate request identification.</param>
//        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.SignCertificateResponse>

//            SendCertificateSigningRequest(this INetworkingNode     NetworkingNode,

//                                          String                   CSR,
//                                          Int32                    SignCertificateRequestId,
//                                          CertificateSigningUse?   CertificateType     = null,

//                                          IEnumerable<KeyPair>?    SignKeys            = null,
//                                          IEnumerable<SignInfo>?   SignInfos           = null,
//                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                          CustomData?              CustomData          = null,

//                                          Request_Id?              RequestId           = null,
//                                          DateTime?                RequestTimestamp    = null,
//                                          TimeSpan?                RequestTimeout      = null,
//                                          EventTracking_Id?        EventTrackingId     = null,
//                                          CancellationToken        CancellationToken   = default)


//                => NetworkingNode.SignCertificate(
//                       new OCPPv2_1.CS.SignCertificateRequest(
//                           NetworkingNode.Id,
//                           CSR,
//                           SignCertificateRequestId,
//                           CertificateType,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region Get15118EVCertificate                 (ISO15118SchemaVersion, CertificateAction, EXIRequest, ...)

//        /// <summary>
//        /// Get an ISO 15118 contract certificate.
//        /// </summary>
//        /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
//        /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
//        /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
//        /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
//        /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.Get15118EVCertificateResponse>

//            Get15118EVCertificate(this INetworkingNode     NetworkingNode,

//                                  ISO15118SchemaVersion    ISO15118SchemaVersion,
//                                  CertificateAction        CertificateAction,
//                                  EXIData                  EXIRequest,
//                                  UInt32?                  MaximumContractCertificateChains   = 1,
//                                  IEnumerable<EMA_Id>?     PrioritizedEMAIds                  = null,

//                                  IEnumerable<KeyPair>?    SignKeys                           = null,
//                                  IEnumerable<SignInfo>?   SignInfos                          = null,
//                                  IEnumerable<OCPP.Signature>?  Signatures                         = null,

//                                  CustomData?              CustomData                         = null,

//                                  Request_Id?              RequestId                          = null,
//                                  DateTime?                RequestTimestamp                   = null,
//                                  TimeSpan?                RequestTimeout                     = null,
//                                  EventTracking_Id?        EventTrackingId                    = null,
//                                  CancellationToken        CancellationToken                  = default)


//                => NetworkingNode.Get15118EVCertificate(
//                       new OCPPv2_1.CS.Get15118EVCertificateRequest(
//                           NetworkingNode.Id,
//                           ISO15118SchemaVersion,
//                           CertificateAction,
//                           EXIRequest,
//                           MaximumContractCertificateChains,
//                           PrioritizedEMAIds,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region GetCertificateStatus                  (OCSPRequestData, ...)

//        /// <summary>
//        /// Get the status of a certificate.
//        /// </summary>
//        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.GetCertificateStatusResponse>

//            GetCertificateStatus(this INetworkingNode     NetworkingNode,

//                                 OCSPRequestData          OCSPRequestData,

//                                 IEnumerable<KeyPair>?    SignKeys            = null,
//                                 IEnumerable<SignInfo>?   SignInfos           = null,
//                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                 CustomData?              CustomData          = null,

//                                 Request_Id?              RequestId           = null,
//                                 DateTime?                RequestTimestamp    = null,
//                                 TimeSpan?                RequestTimeout      = null,
//                                 EventTracking_Id?        EventTrackingId     = null,
//                                 CancellationToken        CancellationToken   = default)


//                => NetworkingNode.GetCertificateStatus(
//                       new OCPPv2_1.CS.GetCertificateStatusRequest(
//                           NetworkingNode.Id,
//                           OCSPRequestData,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region GetCRLRequest                         (GetCRLRequestId, CertificateHashData, ...)

//        /// <summary>
//        /// Get a certificate revocation list from CSMS for the specified certificate.
//        /// </summary>
//        /// 
//        /// <param name="GetCRLRequestId">The identification of this request.</param>
//        /// <param name="CertificateHashData">Certificate hash data.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.GetCRLResponse>

//            GetCRLRequest(this INetworkingNode     NetworkingNode,

//                          UInt32                   GetCRLRequestId,
//                          CertificateHashData      CertificateHashData,

//                          IEnumerable<KeyPair>?    SignKeys            = null,
//                          IEnumerable<SignInfo>?   SignInfos           = null,
//                          IEnumerable<OCPP.Signature>?  Signatures          = null,

//                          CustomData?              CustomData          = null,

//                          Request_Id?              RequestId           = null,
//                          DateTime?                RequestTimestamp    = null,
//                          TimeSpan?                RequestTimeout      = null,
//                          EventTracking_Id?        EventTrackingId     = null,
//                          CancellationToken        CancellationToken   = default)


//                => NetworkingNode.GetCRL(
//                       new OCPPv2_1.CS.GetCRLRequest(
//                           NetworkingNode.Id,
//                           GetCRLRequestId,
//                           CertificateHashData,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion


//        #region SendReservationStatusUpdate           (ReservationId, ReservationUpdateStatus, ...)

//        /// <summary>
//        /// Send a reservation status update.
//        /// </summary>
//        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
//        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>

//            SendReservationStatusUpdate(this INetworkingNode     NetworkingNode,

//                                        Reservation_Id           ReservationId,
//                                        ReservationUpdateStatus  ReservationUpdateStatus,

//                                        IEnumerable<KeyPair>?    SignKeys            = null,
//                                        IEnumerable<SignInfo>?   SignInfos           = null,
//                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                        CustomData?              CustomData          = null,

//                                        Request_Id?              RequestId           = null,
//                                        DateTime?                RequestTimestamp    = null,
//                                        TimeSpan?                RequestTimeout      = null,
//                                        EventTracking_Id?        EventTrackingId     = null,
//                                        CancellationToken        CancellationToken   = default)


//                => NetworkingNode.ReservationStatusUpdate(
//                       new OCPPv2_1.CS.ReservationStatusUpdateRequest(
//                           NetworkingNode.Id,
//                           ReservationId,
//                           ReservationUpdateStatus,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region Authorize                             (IdToken, Certificate = null, ISO15118CertificateHashData = null, ...)

//        /// <summary>
//        /// Authorize the given token.
//        /// </summary>
//        /// <param name="IdToken">The identifier that needs to be authorized.</param>
//        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
//        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.AuthorizeResponse>

//            Authorize(this INetworkingNode           NetworkingNode,

//                      IdToken                        IdToken,
//                      Certificate?                   Certificate                   = null,
//                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

//                      IEnumerable<KeyPair>?          SignKeys                      = null,
//                      IEnumerable<SignInfo>?         SignInfos                     = null,
//                      IEnumerable<OCPP.Signature>?   Signatures                    = null,

//                      CustomData?                    CustomData                    = null,

//                      Request_Id?                    RequestId                     = null,
//                      DateTime?                      RequestTimestamp              = null,
//                      TimeSpan?                      RequestTimeout                = null,
//                      EventTracking_Id?              EventTrackingId               = null,
//                      CancellationToken              CancellationToken             = default)


//                => NetworkingNode.Authorize(
//                       new OCPPv2_1.CS.AuthorizeRequest(
//                           NetworkingNode.Id,
//                           IdToken,
//                           Certificate,
//                           ISO15118CertificateHashData,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyEVChargingNeeds                 (EVSEId, ChargingNeeds, ReceivedTimestamp = null, MaxScheduleTuples = null, ...)

//        /// <summary>
//        /// Notify about EV charging needs.
//        /// </summary>
//        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
//        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
//        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
//        /// <param name="MaxScheduleTuples">The optional maximum number of schedule tuples per schedule the car supports.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>

//            NotifyEVChargingNeeds(this INetworkingNode     NetworkingNode,

//                                  EVSE_Id                  EVSEId,
//                                  ChargingNeeds            ChargingNeeds,
//                                  DateTime?                ReceivedTimestamp   = null,
//                                  UInt16?                  MaxScheduleTuples   = null,

//                                  IEnumerable<KeyPair>?    SignKeys            = null,
//                                  IEnumerable<SignInfo>?   SignInfos           = null,
//                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                  CustomData?              CustomData          = null,

//                                  Request_Id?              RequestId           = null,
//                                  DateTime?                RequestTimestamp    = null,
//                                  TimeSpan?                RequestTimeout      = null,
//                                  EventTracking_Id?        EventTrackingId     = null,
//                                  CancellationToken        CancellationToken   = default)


//                => NetworkingNode.NotifyEVChargingNeeds(
//                       new OCPPv2_1.CS.NotifyEVChargingNeedsRequest(
//                           NetworkingNode.Id,
//                           EVSEId,
//                           ChargingNeeds,
//                           ReceivedTimestamp,
//                           MaxScheduleTuples,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendTransactionEvent                  (EventType, Timestamp, TriggerReason, SequenceNumber, TransactionInfo, ...)

//        /// <summary>
//        /// Send a transaction event.
//        /// </summary>
//        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
//        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
//        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
//        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
//        /// <param name="TransactionInfo">Transaction related information.</param>
//        /// 
//        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
//        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
//        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
//        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
//        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
//        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
//        /// <param name="MeterValues">An optional enumeration of meter values.</param>
//        /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.TransactionEventResponse>

//            SendTransactionEvent(this INetworkingNode      NetworkingNode,

//                                 TransactionEvents         EventType,
//                                 DateTime                  Timestamp,
//                                 TriggerReason             TriggerReason,
//                                 UInt32                    SequenceNumber,
//                                 Transaction               TransactionInfo,

//                                 Boolean?                  Offline                 = null,
//                                 Byte?                     NumberOfPhasesUsed      = null,
//                                 Ampere?                   CableMaxCurrent         = null,
//                                 Reservation_Id?           ReservationId           = null,
//                                 IdToken?                  IdToken                 = null,
//                                 EVSE?                     EVSE                    = null,
//                                 IEnumerable<MeterValue>?  MeterValues             = null,
//                                 PreconditioningStatus?    PreconditioningStatus   = null,

//                                 IEnumerable<KeyPair>?     SignKeys                = null,
//                                 IEnumerable<SignInfo>?    SignInfos               = null,
//                                 IEnumerable<OCPP.Signature>?   Signatures              = null,

//                                 CustomData?               CustomData              = null,

//                                 Request_Id?               RequestId               = null,
//                                 DateTime?                 RequestTimestamp        = null,
//                                 TimeSpan?                 RequestTimeout          = null,
//                                 EventTracking_Id?         EventTrackingId         = null,
//                                 CancellationToken         CancellationToken       = default)


//                => NetworkingNode.TransactionEvent(
//                       new OCPPv2_1.CS.TransactionEventRequest(
//                           NetworkingNode.Id,

//                           EventType,
//                           Timestamp,
//                           TriggerReason,
//                           SequenceNumber,
//                           TransactionInfo,

//                           Offline,
//                           NumberOfPhasesUsed,
//                           CableMaxCurrent,
//                           ReservationId,
//                           IdToken,
//                           EVSE,
//                           MeterValues,
//                           PreconditioningStatus,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendStatusNotification                (EVSEId, ConnectorId, Timestamp, Status, ...)

//        /// <summary>
//        /// Send a status notification for the given connector.
//        /// </summary>
//        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
//        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
//        /// <param name="Timestamp">The time for which the status is reported.</param>
//        /// <param name="Status">The current status of the connector.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.StatusNotificationResponse>

//            SendStatusNotification(this INetworkingNode     NetworkingNode,

//                                   EVSE_Id                  EVSEId,
//                                   Connector_Id             ConnectorId,
//                                   DateTime                 Timestamp,
//                                   ConnectorStatus          Status,

//                                   IEnumerable<KeyPair>?    SignKeys            = null,
//                                   IEnumerable<SignInfo>?   SignInfos           = null,
//                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                   CustomData?              CustomData          = null,

//                                   Request_Id?              RequestId           = null,
//                                   DateTime?                RequestTimestamp    = null,
//                                   TimeSpan?                RequestTimeout      = null,
//                                   EventTracking_Id?        EventTrackingId     = null,
//                                   CancellationToken        CancellationToken   = default)


//                => NetworkingNode.StatusNotification(
//                       new OCPPv2_1.CS.StatusNotificationRequest(
//                           NetworkingNode.Id,
//                           Timestamp,
//                           Status,
//                           EVSEId,
//                           ConnectorId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendMeterValues                       (EVSEId, MeterValues, ...)

//        /// <summary>
//        /// Send a meter values for the given connector.
//        /// </summary>
//        /// <param name="EVSEId">The EVSE identification at the charging station.</param>
//        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.MeterValuesResponse>

//            SendMeterValues(this INetworkingNode     NetworkingNode,

//                            EVSE_Id                  EVSEId, // 0 => main power meter; 1 => first EVSE
//                            IEnumerable<MeterValue>  MeterValues,

//                            IEnumerable<KeyPair>?    SignKeys            = null,
//                            IEnumerable<SignInfo>?   SignInfos           = null,
//                            IEnumerable<OCPP.Signature>?  Signatures          = null,

//                            CustomData?              CustomData          = null,

//                            Request_Id?              RequestId           = null,
//                            DateTime?                RequestTimestamp    = null,
//                            TimeSpan?                RequestTimeout      = null,
//                            EventTracking_Id?        EventTrackingId     = null,
//                            CancellationToken        CancellationToken   = default)


//                => NetworkingNode.MeterValues(
//                       new OCPPv2_1.CS.MeterValuesRequest(
//                           NetworkingNode.Id,
//                           EVSEId,
//                           MeterValues,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyChargingLimit                   (ChargingLimit, ChargingSchedules, EVSEId = null, ...)

//        /// <summary>
//        /// Notify about a charging limit.
//        /// </summary>
//        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
//        /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
//        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyChargingLimitResponse>

//            NotifyChargingLimit(this INetworkingNode           NetworkingNode,

//                                ChargingLimit                  ChargingLimit,
//                                IEnumerable<ChargingSchedule>  ChargingSchedules,
//                                EVSE_Id?                       EVSEId              = null,

//                                IEnumerable<KeyPair>?          SignKeys            = null,
//                                IEnumerable<SignInfo>?         SignInfos           = null,
//                                IEnumerable<OCPP.Signature>?   Signatures          = null,

//                                CustomData?                    CustomData          = null,

//                                Request_Id?                    RequestId           = null,
//                                DateTime?                      RequestTimestamp    = null,
//                                TimeSpan?                      RequestTimeout      = null,
//                                EventTracking_Id?              EventTrackingId     = null,
//                                CancellationToken              CancellationToken   = default)


//                => NetworkingNode.NotifyChargingLimit(
//                       new OCPPv2_1.CS.NotifyChargingLimitRequest(
//                           NetworkingNode.Id,
//                           ChargingLimit,
//                           ChargingSchedules,
//                           EVSEId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region SendClearedChargingLimit              (ChargingLimitSource, EVSEId, ...)

//        /// <summary>
//        /// Send a heartbeat.
//        /// </summary>
//        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
//        /// <param name="EVSEId">An optional EVSE identification.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.ClearedChargingLimitResponse>

//            SendClearedChargingLimit(this INetworkingNode     NetworkingNode,

//                                     ChargingLimitSource      ChargingLimitSource,
//                                     EVSE_Id?                 EVSEId,

//                                     IEnumerable<KeyPair>?    SignKeys            = null,
//                                     IEnumerable<SignInfo>?   SignInfos           = null,
//                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                     CustomData?              CustomData          = null,

//                                     Request_Id?              RequestId           = null,
//                                     DateTime?                RequestTimestamp    = null,
//                                     TimeSpan?                RequestTimeout      = null,
//                                     EventTracking_Id?        EventTrackingId     = null,
//                                     CancellationToken        CancellationToken   = default)


//                => NetworkingNode.ClearedChargingLimit(
//                       new OCPPv2_1.CS.ClearedChargingLimitRequest(
//                           NetworkingNode.Id,
//                           ChargingLimitSource,
//                           EVSEId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region ReportChargingProfiles                (ReportChargingProfilesRequestId, ChargingLimitSource, EVSEId, ChargingProfiles, ToBeContinued = null, ...)

//        /// <summary>
//        /// Report about all charging profiles.
//        /// </summary>
//        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
//        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
//        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
//        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.ReportChargingProfilesResponse>

//            ReportChargingProfiles(this INetworkingNode          NetworkingNode,

//                                   Int32                         ReportChargingProfilesRequestId,
//                                   ChargingLimitSource           ChargingLimitSource,
//                                   EVSE_Id                       EVSEId,
//                                   IEnumerable<ChargingProfile>  ChargingProfiles,
//                                   Boolean?                      ToBeContinued       = null,

//                                   IEnumerable<KeyPair>?         SignKeys            = null,
//                                   IEnumerable<SignInfo>?        SignInfos           = null,
//                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                   CustomData?                   CustomData          = null,

//                                   Request_Id?                   RequestId           = null,
//                                   DateTime?                     RequestTimestamp    = null,
//                                   TimeSpan?                     RequestTimeout      = null,
//                                   EventTracking_Id?             EventTrackingId     = null,
//                                   CancellationToken             CancellationToken   = default)


//                => NetworkingNode.ReportChargingProfiles(
//                       new OCPPv2_1.CS.ReportChargingProfilesRequest(
//                           NetworkingNode.Id,
//                           ReportChargingProfilesRequestId,
//                           ChargingLimitSource,
//                           EVSEId,
//                           ChargingProfiles,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyEVChargingSchedule              (NotifyEVChargingScheduleRequestId, TimeBase, EVSEId, ChargingSchedule, SelectedScheduleTupleId = null, PowerToleranceAcceptance = null, ...)

//        /// <summary>
//        /// Notify about an EV charging schedule.
//        /// </summary>
//        /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
//        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
//        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
//        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
//        /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
//        /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>

//            NotifyEVChargingSchedule(this INetworkingNode     NetworkingNode,

//                                     Int32                    NotifyEVChargingScheduleRequestId,
//                                     DateTime                 TimeBase,
//                                     EVSE_Id                  EVSEId,
//                                     ChargingSchedule         ChargingSchedule,
//                                     Byte?                    SelectedScheduleTupleId    = null,
//                                     Boolean?                 PowerToleranceAcceptance   = null,

//                                     IEnumerable<KeyPair>?    SignKeys                   = null,
//                                     IEnumerable<SignInfo>?   SignInfos                  = null,
//                                     IEnumerable<OCPP.Signature>?  Signatures                 = null,

//                                     CustomData?              CustomData                 = null,

//                                     Request_Id?              RequestId                  = null,
//                                     DateTime?                RequestTimestamp           = null,
//                                     TimeSpan?                RequestTimeout             = null,
//                                     EventTracking_Id?        EventTrackingId            = null,
//                                     CancellationToken        CancellationToken          = default)


//                => NetworkingNode.NotifyEVChargingSchedule(
//                       new OCPPv2_1.CS.NotifyEVChargingScheduleRequest(
//                           NetworkingNode.Id,
//                           TimeBase,
//                           EVSEId,
//                           ChargingSchedule,
//                           SelectedScheduleTupleId,
//                           PowerToleranceAcceptance,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyPriorityCharging                (NotifyPriorityChargingRequestId, TransactionId, Activated, ...)

//        /// <summary>
//        /// Notify about priority charging.
//        /// </summary>
//        /// <param name="NotifyPriorityChargingRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyPriorityChargingRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
//        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
//        /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>

//            NotifyPriorityCharging(this INetworkingNode     NetworkingNode,

//                                   Int32                    NotifyPriorityChargingRequestId,
//                                   Transaction_Id           TransactionId,
//                                   Boolean                  Activated,

//                                   IEnumerable<KeyPair>?    SignKeys            = null,
//                                   IEnumerable<SignInfo>?   SignInfos           = null,
//                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                   CustomData?              CustomData          = null,

//                                   Request_Id?              RequestId           = null,
//                                   DateTime?                RequestTimestamp    = null,
//                                   TimeSpan?                RequestTimeout      = null,
//                                   EventTracking_Id?        EventTrackingId     = null,
//                                   CancellationToken        CancellationToken   = default)


//                => NetworkingNode.NotifyPriorityCharging(
//                       new OCPPv2_1.CS.NotifyPriorityChargingRequest(
//                           NetworkingNode.Id,
//                           TransactionId,
//                           Activated,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region PullDynamicScheduleUpdate             (ChargingProfileId, ...)

//        /// <summary>
//        /// Report about all charging profiles.
//        /// </summary>
//        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>

//            PullDynamicScheduleUpdate(this INetworkingNode     NetworkingNode,

//                                      ChargingProfile_Id       ChargingProfileId,

//                                      IEnumerable<KeyPair>?    SignKeys            = null,
//                                      IEnumerable<SignInfo>?   SignInfos           = null,
//                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                      CustomData?              CustomData          = null,

//                                      Request_Id?              RequestId           = null,
//                                      DateTime?                RequestTimestamp    = null,
//                                      TimeSpan?                RequestTimeout      = null,
//                                      EventTracking_Id?        EventTrackingId     = null,
//                                      CancellationToken        CancellationToken   = default)


//                => NetworkingNode.PullDynamicScheduleUpdate(
//                       new OCPPv2_1.CS.PullDynamicScheduleUpdateRequest(
//                           NetworkingNode.Id,
//                           ChargingProfileId,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion


//        #region NotifyDisplayMessages                 (NotifyDisplayMessagesRequestId, MessageInfos, ToBeContinued, ...)

//        /// <summary>
//        /// NotifyDisplayMessages the given token.
//        /// </summary>
//        /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the notify display messages request.</param>
//        /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>

//            NotifyDisplayMessages(this INetworkingNode      NetworkingNode,

//                                  Int32                     NotifyDisplayMessagesRequestId,
//                                  IEnumerable<MessageInfo>  MessageInfos,
//                                  Boolean?                  ToBeContinued       = null,

//                                  IEnumerable<KeyPair>?     SignKeys            = null,
//                                  IEnumerable<SignInfo>?    SignInfos           = null,
//                                  IEnumerable<OCPP.Signature>?   Signatures          = null,

//                                  CustomData?               CustomData          = null,

//                                  Request_Id?               RequestId           = null,
//                                  DateTime?                 RequestTimestamp    = null,
//                                  TimeSpan?                 RequestTimeout      = null,
//                                  EventTracking_Id?         EventTrackingId     = null,
//                                  CancellationToken         CancellationToken   = default)


//                => NetworkingNode.NotifyDisplayMessages(
//                       new OCPPv2_1.CS.NotifyDisplayMessagesRequest(
//                           NetworkingNode.Id,
//                           NotifyDisplayMessagesRequestId,
//                           MessageInfos,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion

//        #region NotifyCustomerInformation             (NotifyCustomerInformationRequestId, Data, SequenceNumber, GeneratedAt, ToBeContinued = null, ...)

//        /// <summary>
//        /// NotifyCustomerInformation the given token.
//        /// </summary>
//        /// <param name="NotifyCustomerInformationRequestId">The unique identification of the notify customer information request.</param>
//        /// <param name="Data">The requested data or a part of the requested data. No format specified in which the data is returned.</param>
//        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
//        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
//        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>

//            NotifyCustomerInformation(this INetworkingNode     NetworkingNode,

//                                      Int64                    NotifyCustomerInformationRequestId,
//                                      String                   Data,
//                                      UInt32                   SequenceNumber,
//                                      DateTime                 GeneratedAt,
//                                      Boolean?                 ToBeContinued       = null,

//                                      IEnumerable<KeyPair>?    SignKeys            = null,
//                                      IEnumerable<SignInfo>?   SignInfos           = null,
//                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

//                                      CustomData?              CustomData          = null,

//                                      Request_Id?              RequestId           = null,
//                                      DateTime?                RequestTimestamp    = null,
//                                      TimeSpan?                RequestTimeout      = null,
//                                      EventTracking_Id?        EventTrackingId     = null,
//                                      CancellationToken        CancellationToken   = default)


//                => NetworkingNode.NotifyCustomerInformation(
//                       new OCPPv2_1.CS.NotifyCustomerInformationRequest(
//                           NetworkingNode.Id,
//                           NotifyCustomerInformationRequestId,
//                           Data,
//                           SequenceNumber,
//                           GeneratedAt,
//                           ToBeContinued,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           CustomData,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion


//        // Binary Data Streams Extensions

//        #region TransferBinaryData                    (VendorId, MessageId = null, Data = null, ...)

//        /// <summary>
//        /// Transfer the given binary data to the CSMS.
//        /// </summary>
//        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
//        /// <param name="MessageId">An optional message identification field.</param>
//        /// <param name="Data">Optional message data as text without specified length or format.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<OCPP.CSMS.BinaryDataTransferResponse>

//            TransferBinaryData(this INetworkingNode          NetworkingNode,

//                               Vendor_Id                     VendorId,
//                               Message_Id?                   MessageId           = null,
//                               Byte[]?                       Data                = null,
//                               BinaryFormats?                Format              = null,

//                               IEnumerable<KeyPair>?         SignKeys            = null,
//                               IEnumerable<SignInfo>?        SignInfos           = null,
//                               IEnumerable<OCPP.Signature>?  Signatures          = null,

//                               Request_Id?                   RequestId           = null,
//                               DateTime?                     RequestTimestamp    = null,
//                               TimeSpan?                     RequestTimeout      = null,
//                               EventTracking_Id?             EventTrackingId     = null,
//                               CancellationToken             CancellationToken   = default)


//                => NetworkingNode.BinaryDataTransfer(
//                       new OCPP.CS.BinaryDataTransferRequest(
//                           NetworkingNode.Id,
//                           VendorId,
//                           MessageId,
//                           Data,
//                           Format,

//                           SignKeys,
//                           SignInfos,
//                           Signatures,

//                           RequestId        ?? NetworkingNode.NextRequestId,
//                           RequestTimestamp ?? Timestamp.Now,
//                           RequestTimeout   ?? NetworkingNode.DefaultRequestTimeout,
//                           EventTrackingId  ?? EventTracking_Id.New,
//                           NetworkPath.Empty,
//                           CancellationToken
//                       )
//                   );

//        #endregion


//    }

//}
