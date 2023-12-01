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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// Extension methods for all charging stations
    /// </summary>
    public static class IChargingStationExtensions
    {

        #region SendBootNotification(BootReason, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="BootReason">The the reason for sending this boot notification to the CSMS.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.BootNotificationResponse>

            SendBootNotification(this IChargingStation                                      ChargingStation,

                                 BootReason                                                 BootReason,

                                 IEnumerable<Signature>?                                    Signatures                                = null,
                                 CustomData?                                                CustomData                                = null,

                                 Request_Id?                                                RequestId                                 = null,
                                 DateTime?                                                  RequestTimestamp                          = null,
                                 TimeSpan?                                                  RequestTimeout                            = null,
                                 EventTracking_Id?                                          EventTrackingId                           = null,

                                 IEnumerable<KeyPair>?                                      SignKeys                                  = null,
                                 IEnumerable<SignInfo>?                                     SignInfos                                 = null,
                                 CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
                                 CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer           = null,
                                 CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                                 CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null,

                                 CancellationToken                                          CancellationToken                         = default)


                => ChargingStation.SendBootNotification(
                       new BootNotificationRequest(
                           ChargingStation.Id,
                           new ChargingStation(
                               ChargingStation.Model,
                               ChargingStation.VendorName,
                               ChargingStation.SerialNumber,
                               ChargingStation.Modem,
                               ChargingStation.FirmwareVersion,
                               ChargingStation.CustomData
                           ),
                           BootReason,

                           SignKeys,
                           SignInfos,
                           Signatures,
                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

            //var signaturePolicy = SignaturePolicy ?? ChargingStation.SignaturePolicy;

            //IEnumerable<SignaturePolicyEntry>? signaturePolicyEntries = null;

            //if ((SignKeys        is not null && SignKeys.       Any()) ||
            //    (SignInfos       is not null && SignInfos.      Any()) ||
            //    (signaturePolicy is not null && signaturePolicy.Has(BootNotificationRequest.DefaultJSONLDContext,
            //                                                        out signaturePolicyEntries)))
            //{

            //    var signInfos = new List<SignInfo>();

            //    if (SignInfos is not null && SignInfos.Any())
            //        signInfos.AddRange(SignInfos);

            //    if (SignKeys  is not null && SignKeys. Any())
            //        signInfos.AddRange(SignKeys.Select(signKey => signKey.ToSignInfo()));

            //    if (signaturePolicyEntries is not null && signaturePolicyEntries.Any())
            //    {
            //        foreach (var signaturePolicyEntry in signaturePolicyEntries)
            //        {
            //            if (signaturePolicyEntry.KeyPair is not null)
            //                signInfos.Add(signaturePolicyEntry.KeyPair.ToSignInfo());
            //        }
            //    }

            //    if (!CryptoUtils.SignRequestMessage(
            //            bootNotificationRequest,
            //            bootNotificationRequest.ToJSON(
            //                CustomBootNotificationRequestSerializer ?? ChargingStation.CustomBootNotificationRequestSerializer,
            //                CustomChargingStationSerializer         ?? ChargingStation.CustomChargingStationSerializer,
            //                CustomSignatureSerializer               ?? ChargingStation.CustomSignatureSerializer,
            //                CustomCustomDataSerializer              ?? ChargingStation.CustomCustomDataSerializer
            //            ),
            //            BootNotificationRequest.DefaultJSONLDContext,
            //            SignaturePolicy,
            //            out var errorResponse,
            //            signInfos.ToArray()))
            //    {

            //        return Task.FromResult(
            //                   new CSMS.BootNotificationResponse(
            //                       bootNotificationRequest,
            //                       Result.SignatureError(errorResponse)
            //                   )
            //               );

            //    }

            //}

        #endregion

        #region SendFirmwareStatusNotification       (Status, ...)

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

            SendFirmwareStatusNotification(this IChargingStation    ChargingStation,

                                           FirmwareStatus           Status,
                                           Int64?                   UpdateFirmwareRequestId   = null,

                                           IEnumerable<KeyPair>?    SignKeys                  = null,
                                           IEnumerable<SignInfo>?   SignInfos                 = null,
                                           IEnumerable<Signature>?  Signatures                = null,

                                           CustomData?              CustomData                = null,

                                           Request_Id?              RequestId                 = null,
                                           DateTime?                RequestTimestamp          = null,
                                           TimeSpan?                RequestTimeout            = null,
                                           EventTracking_Id?        EventTrackingId           = null,
                                           CancellationToken        CancellationToken         = default)


                => ChargingStation.SendFirmwareStatusNotification(
                       new FirmwareStatusNotificationRequest(
                           ChargingStation.Id,
                           Status,
                           UpdateFirmwareRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendPublishFirmwareStatusNotification(Status, PublishFirmwareStatusNotificationRequestId, DownloadLocations, ...)

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

            SendPublishFirmwareStatusNotification(this IChargingStation    ChargingStation,

                                                  PublishFirmwareStatus    Status,
                                                  Int32?                   PublishFirmwareStatusNotificationRequestId,
                                                  IEnumerable<URL>?        DownloadLocations,

                                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                                  IEnumerable<Signature>?  Signatures          = null,

                                                  CustomData?              CustomData          = null,

                                                  Request_Id?              RequestId           = null,
                                                  DateTime?                RequestTimestamp    = null,
                                                  TimeSpan?                RequestTimeout      = null,
                                                  EventTracking_Id?        EventTrackingId     = null,
                                                  CancellationToken        CancellationToken   = default)


                => ChargingStation.SendPublishFirmwareStatusNotification(
                       new PublishFirmwareStatusNotificationRequest(
                           ChargingStation.Id,
                           Status,
                           PublishFirmwareStatusNotificationRequestId,
                           DownloadLocations,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendHeartbeat                        (...)

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

            SendHeartbeat(this IChargingStation    ChargingStation,


                          IEnumerable<KeyPair>?    SignKeys            = null,
                          IEnumerable<SignInfo>?   SignInfos           = null,
                          IEnumerable<Signature>?  Signatures          = null,

                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => ChargingStation.SendHeartbeat(
                       new HeartbeatRequest(
                           ChargingStation.Id,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyEvent                          (GeneratedAt, SequenceNumber, EventData, ToBeContinued = null, ...)

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

            NotifyEvent(this IChargingStation    ChargingStation,

                        DateTime                 GeneratedAt,
                        UInt32                   SequenceNumber,
                        IEnumerable<EventData>   EventData,
                        Boolean?                 ToBeContinued       = null,

                        IEnumerable<KeyPair>?    SignKeys            = null,
                        IEnumerable<SignInfo>?   SignInfos           = null,
                        IEnumerable<Signature>?  Signatures          = null,

                        CustomData?              CustomData          = null,

                        Request_Id?              RequestId           = null,
                        DateTime?                RequestTimestamp    = null,
                        TimeSpan?                RequestTimeout      = null,
                        EventTracking_Id?        EventTrackingId     = null,
                        CancellationToken        CancellationToken   = default)


                => ChargingStation.NotifyEvent(
                       new NotifyEventRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendSecurityEventNotification        (Type, Timestamp, TechInfo = null, TechInfo = null, ...)

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

            SendSecurityEventNotification(this IChargingStation    ChargingStation,

                                          SecurityEventType        Type,
                                          DateTime                 Timestamp,
                                          String?                  TechInfo            = null,

                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                          IEnumerable<Signature>?  Signatures          = null,

                                          CustomData?              CustomData          = null,

                                          Request_Id?              RequestId           = null,
                                          DateTime?                RequestTimestamp    = null,
                                          TimeSpan?                RequestTimeout      = null,
                                          EventTracking_Id?        EventTrackingId     = null,
                                          CancellationToken        CancellationToken   = default)


                => ChargingStation.SendSecurityEventNotification(
                       new SecurityEventNotificationRequest(
                           ChargingStation.Id,
                           Type,
                           Timestamp,
                           TechInfo,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyReport                         (SequenceNumber, GeneratedAt, ReportData, ...)

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

            NotifyReport(this IChargingStation  ChargingStation,

                         Int32                    NotifyReportRequestId,
                         UInt32                   SequenceNumber,
                         DateTime                 GeneratedAt,
                         IEnumerable<ReportData>  ReportData,
                         Boolean?                 ToBeContinued       = null,

                         IEnumerable<KeyPair>?    SignKeys            = null,
                         IEnumerable<SignInfo>?   SignInfos           = null,
                         IEnumerable<Signature>?  Signatures          = null,

                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => ChargingStation.NotifyReport(
                       new NotifyReportRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyMonitoringReport               (NotifyMonitoringReportRequestId, SequenceNumber, GeneratedAt, MonitoringData, ToBeContinued = null, ...)

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

            NotifyMonitoringReport(this IChargingStation  ChargingStation,

                                   Int32                        NotifyMonitoringReportRequestId,
                                   UInt32                       SequenceNumber,
                                   DateTime                     GeneratedAt,
                                   IEnumerable<MonitoringData>  MonitoringData,
                                   Boolean?                     ToBeContinued       = null,

                                   IEnumerable<KeyPair>?        SignKeys            = null,
                                   IEnumerable<SignInfo>?       SignInfos           = null,
                                   IEnumerable<Signature>?      Signatures          = null,

                                   CustomData?                  CustomData          = null,

                                   Request_Id?                  RequestId           = null,
                                   DateTime?                    RequestTimestamp    = null,
                                   TimeSpan?                    RequestTimeout      = null,
                                   EventTracking_Id?            EventTrackingId     = null,
                                   CancellationToken            CancellationToken   = default)


                => ChargingStation.NotifyMonitoringReport(
                       new NotifyMonitoringReportRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendLogStatusNotification            (Status, LogRequestId = null, ...)

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

            SendLogStatusNotification(this IChargingStation    ChargingStation,

                                      UploadLogStatus          Status,
                                      Int32?                   LogRequestId        = null,

                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                      IEnumerable<Signature>?  Signatures          = null,

                                      CustomData?              CustomData          = null,

                                      Request_Id?              RequestId           = null,
                                      DateTime?                RequestTimestamp    = null,
                                      TimeSpan?                RequestTimeout      = null,
                                      EventTracking_Id?        EventTrackingId     = null,
                                      CancellationToken        CancellationToken   = default)


                => ChargingStation.SendLogStatusNotification(
                       new LogStatusNotificationRequest(
                           ChargingStation.Id,
                           Status,
                           LogRequestId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region TransferData                         (VendorId, MessageId = null, Data = null, ...)

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
        public static Task<CSMS.DataTransferResponse>

            TransferData(this IChargingStation    ChargingStation,

                         Vendor_Id                VendorId,
                         Message_Id?              MessageId           = null,
                         JToken?                  Data                = null,

                         IEnumerable<KeyPair>?    SignKeys            = null,
                         IEnumerable<SignInfo>?   SignInfos           = null,
                         IEnumerable<Signature>?  Signatures          = null,

                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => ChargingStation.TransferData(
                       new DataTransferRequest(
                           ChargingStation.Id,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SendCertificateSigningRequest        (CSR, SignCertificateRequestId, CertificateType = null, ...)

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

            SendCertificateSigningRequest(this IChargingStation    ChargingStation,

                                          String                   CSR,
                                          Int32                    SignCertificateRequestId,
                                          CertificateSigningUse?   CertificateType     = null,

                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                          IEnumerable<Signature>?  Signatures          = null,

                                          CustomData?              CustomData          = null,

                                          Request_Id?              RequestId           = null,
                                          DateTime?                RequestTimestamp    = null,
                                          TimeSpan?                RequestTimeout      = null,
                                          EventTracking_Id?        EventTrackingId     = null,
                                          CancellationToken        CancellationToken   = default)


                => ChargingStation.SendCertificateSigningRequest(
                       new SignCertificateRequest(
                           ChargingStation.Id,
                           CSR,
                           SignCertificateRequestId,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region Get15118EVCertificate                (ISO15118SchemaVersion, CertificateAction, EXIRequest, ...)

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

            Get15118EVCertificate(this IChargingStation    ChargingStation,

                                  ISO15118SchemaVersion    ISO15118SchemaVersion,
                                  CertificateAction        CertificateAction,
                                  EXIData                  EXIRequest,
                                  UInt32?                  MaximumContractCertificateChains   = 1,
                                  IEnumerable<EMA_Id>?     PrioritizedEMAIds                  = null,

                                  IEnumerable<KeyPair>?    SignKeys                           = null,
                                  IEnumerable<SignInfo>?   SignInfos                          = null,
                                  IEnumerable<Signature>?  Signatures                         = null,

                                  CustomData?              CustomData                         = null,

                                  Request_Id?              RequestId                          = null,
                                  DateTime?                RequestTimestamp                   = null,
                                  TimeSpan?                RequestTimeout                     = null,
                                  EventTracking_Id?        EventTrackingId                    = null,
                                  CancellationToken        CancellationToken                  = default)


                => ChargingStation.Get15118EVCertificate(
                       new Get15118EVCertificateRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCertificateStatus                 (OCSPRequestData, ...)

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

            GetCertificateStatus(this IChargingStation    ChargingStation,

                                 OCSPRequestData          OCSPRequestData,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 CancellationToken        CancellationToken   = default)


                => ChargingStation.GetCertificateStatus(
                       new GetCertificateStatusRequest(
                           ChargingStation.Id,
                           OCSPRequestData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCRLRequest                        (GetCRLRequestId, CertificateHashData, ...)

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

            GetCRLRequest(this IChargingStation    ChargingStation,

                          UInt32                   GetCRLRequestId,
                          CertificateHashData      CertificateHashData,

                          IEnumerable<KeyPair>?    SignKeys            = null,
                          IEnumerable<SignInfo>?   SignInfos           = null,
                          IEnumerable<Signature>?  Signatures          = null,

                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => ChargingStation.GetCRL(
                       new GetCRLRequest(
                           ChargingStation.Id,
                           GetCRLRequestId,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SendReservationStatusUpdate          (ReservationId, ReservationUpdateStatus, ...)

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

            SendReservationStatusUpdate(this IChargingStation  ChargingStation,

                                        Reservation_Id           ReservationId,
                                        ReservationUpdateStatus  ReservationUpdateStatus,

                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                        IEnumerable<Signature>?  Signatures          = null,

                                        CustomData?              CustomData          = null,

                                        Request_Id?              RequestId           = null,
                                        DateTime?                RequestTimestamp    = null,
                                        TimeSpan?                RequestTimeout      = null,
                                        EventTracking_Id?        EventTrackingId     = null,
                                        CancellationToken        CancellationToken   = default)


                => ChargingStation.SendReservationStatusUpdate(
                       new ReservationStatusUpdateRequest(
                           ChargingStation.Id,
                           ReservationId,
                           ReservationUpdateStatus,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region Authorize                            (IdToken, Certificate = null, ISO15118CertificateHashData = null, ...)

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

            Authorize(this IChargingStation  ChargingStation,

                      IdToken                        IdToken,
                      Certificate?                   Certificate                   = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                      IEnumerable<KeyPair>?          SignKeys                      = null,
                      IEnumerable<SignInfo>?         SignInfos                     = null,
                      IEnumerable<Signature>?        Signatures                    = null,

                      CustomData?                    CustomData                    = null,

                      Request_Id?                    RequestId                     = null,
                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      CancellationToken              CancellationToken             = default)


                => ChargingStation.Authorize(
                       new AuthorizeRequest(
                           ChargingStation.Id,
                           IdToken,
                           Certificate,
                           ISO15118CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyEVChargingNeeds                (EVSEId, ChargingNeeds, ReceivedTimestamp = null, MaxScheduleTuples = null, ...)

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

            NotifyEVChargingNeeds(this IChargingStation    ChargingStation,

                                  EVSE_Id                  EVSEId,
                                  ChargingNeeds            ChargingNeeds,
                                  DateTime?                ReceivedTimestamp   = null,
                                  UInt16?                  MaxScheduleTuples   = null,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null,

                                  Request_Id?              RequestId           = null,
                                  DateTime?                RequestTimestamp    = null,
                                  TimeSpan?                RequestTimeout      = null,
                                  EventTracking_Id?        EventTrackingId     = null,
                                  CancellationToken        CancellationToken   = default)


                => ChargingStation.NotifyEVChargingNeeds(
                       new NotifyEVChargingNeedsRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendTransactionEvent                 (EventType, Timestamp, TriggerReason, SequenceNumber, TransactionInfo, ...)

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

            SendTransactionEvent(this IChargingStation     ChargingStation,

                                 TransactionEvents         EventType,
                                 DateTime                  Timestamp,
                                 TriggerReason            TriggerReason,
                                 UInt32                    SequenceNumber,
                                 Transaction               TransactionInfo,

                                 Boolean?                  Offline                 = null,
                                 Byte?                     NumberOfPhasesUsed      = null,
                                 Ampere?                   CableMaxCurrent         = null,
                                 Reservation_Id?           ReservationId           = null,
                                 IdToken?                  IdToken                 = null,
                                 EVSE?                     EVSE                    = null,
                                 IEnumerable<MeterValue>?  MeterValues             = null,
                                 PreconditioningStatus?    PreconditioningStatus   = null,

                                 IEnumerable<KeyPair>?     SignKeys                = null,
                                 IEnumerable<SignInfo>?    SignInfos               = null,
                                 IEnumerable<Signature>?   Signatures              = null,

                                 CustomData?               CustomData              = null,

                                 Request_Id?               RequestId               = null,
                                 DateTime?                 RequestTimestamp        = null,
                                 TimeSpan?                 RequestTimeout          = null,
                                 EventTracking_Id?         EventTrackingId         = null,
                                 CancellationToken         CancellationToken       = default)


                => ChargingStation.SendTransactionEvent(
                       new TransactionEventRequest(
                           ChargingStation.Id,

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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendStatusNotification               (EVSEId, ConnectorId, Timestamp, Status, ...)

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

            SendStatusNotification(this IChargingStation    ChargingStation,

                                   EVSE_Id                  EVSEId,
                                   Connector_Id             ConnectorId,
                                   DateTime                 Timestamp,
                                   ConnectorStatus          Status,

                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                   IEnumerable<Signature>?  Signatures          = null,

                                   CustomData?              CustomData          = null,

                                   Request_Id?              RequestId           = null,
                                   DateTime?                RequestTimestamp    = null,
                                   TimeSpan?                RequestTimeout      = null,
                                   EventTracking_Id?        EventTrackingId     = null,
                                   CancellationToken        CancellationToken   = default)


                => ChargingStation.SendStatusNotification(
                       new StatusNotificationRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendMeterValues                      (EVSEId, MeterValues, ...)

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

            SendMeterValues(this IChargingStation  ChargingStation,

                            EVSE_Id                  EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>  MeterValues,

                            IEnumerable<KeyPair>?    SignKeys            = null,
                            IEnumerable<SignInfo>?   SignInfos           = null,
                            IEnumerable<Signature>?  Signatures          = null,

                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => ChargingStation.SendMeterValues(
                       new MeterValuesRequest(
                           ChargingStation.Id,
                           EVSEId,
                           MeterValues,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyChargingLimit                  (ChargingLimit, ChargingSchedules, EVSEId = null, ...)

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

            NotifyChargingLimit(this IChargingStation  ChargingStation,

                                ChargingLimit                  ChargingLimit,
                                IEnumerable<ChargingSchedule>  ChargingSchedules,
                                EVSE_Id?                       EVSEId              = null,

                                IEnumerable<KeyPair>?          SignKeys            = null,
                                IEnumerable<SignInfo>?         SignInfos           = null,
                                IEnumerable<Signature>?        Signatures          = null,

                                CustomData?                    CustomData          = null,

                                Request_Id?                    RequestId           = null,
                                DateTime?                      RequestTimestamp    = null,
                                TimeSpan?                      RequestTimeout      = null,
                                EventTracking_Id?              EventTrackingId     = null,
                                CancellationToken              CancellationToken   = default)


                => ChargingStation.NotifyChargingLimit(
                       new NotifyChargingLimitRequest(
                           ChargingStation.Id,
                           ChargingLimit,
                           ChargingSchedules,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendClearedChargingLimit             (ChargingLimitSource, EVSEId, ...)

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

            SendClearedChargingLimit(this IChargingStation    ChargingStation,

                                     ChargingLimitSource      ChargingLimitSource,
                                     EVSE_Id?                 EVSEId,

                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                     IEnumerable<Signature>?  Signatures          = null,

                                     CustomData?              CustomData          = null,

                                     Request_Id?              RequestId           = null,
                                     DateTime?                RequestTimestamp    = null,
                                     TimeSpan?                RequestTimeout      = null,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     CancellationToken        CancellationToken   = default)


                => ChargingStation.SendClearedChargingLimit(
                       new ClearedChargingLimitRequest(
                           ChargingStation.Id,
                           ChargingLimitSource,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ReportChargingProfiles               (ReportChargingProfilesRequestId, ChargingLimitSource, EVSEId, ChargingProfiles, ToBeContinued = null, ...)

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

            ReportChargingProfiles(this IChargingStation         ChargingStation,

                                   Int32                         ReportChargingProfilesRequestId,
                                   ChargingLimitSource           ChargingLimitSource,
                                   EVSE_Id                       EVSEId,
                                   IEnumerable<ChargingProfile>  ChargingProfiles,
                                   Boolean?                      ToBeContinued       = null,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   CustomData?                   CustomData          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)


                => ChargingStation.ReportChargingProfiles(
                       new ReportChargingProfilesRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyEVChargingSchedule             (NotifyEVChargingScheduleRequestId, TimeBase, EVSEId, ChargingSchedule, SelectedScheduleTupleId = null, PowerToleranceAcceptance = null, ...)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
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

            NotifyEVChargingSchedule(this IChargingStation    ChargingStation,

                                     Int32                    NotifyEVChargingScheduleRequestId,
                                     DateTime                 TimeBase,
                                     EVSE_Id                  EVSEId,
                                     ChargingSchedule         ChargingSchedule,
                                     Byte?                    SelectedScheduleTupleId    = null,
                                     Boolean?                 PowerToleranceAcceptance   = null,

                                     IEnumerable<KeyPair>?    SignKeys                   = null,
                                     IEnumerable<SignInfo>?   SignInfos                  = null,
                                     IEnumerable<Signature>?  Signatures                 = null,

                                     CustomData?              CustomData                 = null,

                                     Request_Id?              RequestId                  = null,
                                     DateTime?                RequestTimestamp           = null,
                                     TimeSpan?                RequestTimeout             = null,
                                     EventTracking_Id?        EventTrackingId            = null,
                                     CancellationToken        CancellationToken          = default)


                => ChargingStation.NotifyEVChargingSchedule(
                       new NotifyEVChargingScheduleRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyPriorityCharging               (NotifyPriorityChargingRequestId, TransactionId, Activated, ...)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyPriorityChargingRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
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

            NotifyPriorityCharging(this IChargingStation    ChargingStation,

                                   Int32                    NotifyPriorityChargingRequestId,
                                   Transaction_Id           TransactionId,
                                   Boolean                  Activated,

                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                   IEnumerable<Signature>?  Signatures          = null,

                                   CustomData?              CustomData          = null,

                                   Request_Id?              RequestId           = null,
                                   DateTime?                RequestTimestamp    = null,
                                   TimeSpan?                RequestTimeout      = null,
                                   EventTracking_Id?        EventTrackingId     = null,
                                   CancellationToken        CancellationToken   = default)


                => ChargingStation.NotifyPriorityCharging(
                       new NotifyPriorityChargingRequest(
                           ChargingStation.Id,
                           TransactionId,
                           Activated,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region PullDynamicScheduleUpdate            (ChargingProfileId, ...)

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

            PullDynamicScheduleUpdate(this IChargingStation    ChargingStation,

                                      ChargingProfile_Id       ChargingProfileId,

                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                      IEnumerable<Signature>?  Signatures          = null,

                                      CustomData?              CustomData          = null,

                                      Request_Id?              RequestId           = null,
                                      DateTime?                RequestTimestamp    = null,
                                      TimeSpan?                RequestTimeout      = null,
                                      EventTracking_Id?        EventTrackingId     = null,
                                      CancellationToken        CancellationToken   = default)


                => ChargingStation.PullDynamicScheduleUpdate(
                       new PullDynamicScheduleUpdateRequest(
                           ChargingStation.Id,
                           ChargingProfileId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region NotifyDisplayMessages                (NotifyDisplayMessagesRequestId, MessageInfos, ToBeContinued, ...)

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

            NotifyDisplayMessages(this IChargingStation     ChargingStation,

                                  Int32                     NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>  MessageInfos,
                                  Boolean?                  ToBeContinued       = null,

                                  IEnumerable<KeyPair>?     SignKeys            = null,
                                  IEnumerable<SignInfo>?    SignInfos           = null,
                                  IEnumerable<Signature>?   Signatures          = null,

                                  CustomData?               CustomData          = null,

                                  Request_Id?               RequestId           = null,
                                  DateTime?                 RequestTimestamp    = null,
                                  TimeSpan?                 RequestTimeout      = null,
                                  EventTracking_Id?         EventTrackingId     = null,
                                  CancellationToken         CancellationToken   = default)


                => ChargingStation.NotifyDisplayMessages(
                       new NotifyDisplayMessagesRequest(
                           ChargingStation.Id,
                           NotifyDisplayMessagesRequestId,
                           MessageInfos,
                           ToBeContinued,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyCustomerInformation            (NotifyCustomerInformationRequestId, Data, SequenceNumber, GeneratedAt, ToBeContinued = null, ...)

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

            NotifyCustomerInformation(this IChargingStation    ChargingStation,

                                      Int64                    NotifyCustomerInformationRequestId,
                                      String                   Data,
                                      UInt32                   SequenceNumber,
                                      DateTime                 GeneratedAt,
                                      Boolean?                 ToBeContinued       = null,

                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                      IEnumerable<Signature>?  Signatures          = null,

                                      CustomData?              CustomData          = null,

                                      Request_Id?              RequestId           = null,
                                      DateTime?                RequestTimestamp    = null,
                                      TimeSpan?                RequestTimeout      = null,
                                      EventTracking_Id?        EventTrackingId     = null,
                                      CancellationToken        CancellationToken   = default)


                => ChargingStation.NotifyCustomerInformation(
                       new NotifyCustomerInformationRequest(
                           ChargingStation.Id,
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
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData                   (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given binary data to the CSMS.
        /// </summary>
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
        public static Task<CSMS.BinaryDataTransferResponse>

            TransferBinaryData(this IChargingStation    ChargingStation,

                               Vendor_Id                VendorId,
                               Message_Id?              MessageId           = null,
                               Byte[]?                  Data                = null,
                               BinaryFormats?           Format              = null,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<Signature>?  Signatures          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => ChargingStation.TransferBinaryData(
                       new BinaryDataTransferRequest(
                           ChargingStation.Id,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? ChargingStation.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


    }

}
