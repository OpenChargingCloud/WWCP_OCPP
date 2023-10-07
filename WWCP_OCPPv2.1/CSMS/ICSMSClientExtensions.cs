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

//using cloud.charging.open.protocols.OCPPv2_1.CS;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.CSMS.CSMSClientExtensions
//{

//    /// <summary>
//    /// Extention methods for all CSMS clients
//    /// </summary>
//    public static class ICSMSClientExtensions
//    {

//        #region Reset                      (ChargingStationId, ResetType, EVSEId = null, ...)

//        /// <summary>
//        /// Reset the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="ResetType">The type of reset that the charging station should perform.</param>
//        /// <param name="EVSEId">An optional EVSE identification.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ResetResponse> Reset(this ICSMSClient         ICSMSClient,
//                                                ChargeBox_Id             ChargingStationId,
//                                                ResetTypes               ResetType,
//                                                EVSE_Id?                 EVSEId              = null,

//                                                IEnumerable<KeyPair>?    SignKeys            = null,
//                                                IEnumerable<SignInfo>?   SignInfos           = null,
//                                                SignaturePolicy?         SignaturePolicy     = null,
//                                                IEnumerable<Signature>?  Signatures          = null,

//                                                CustomData?              CustomData          = null,

//                                                Request_Id?              RequestId           = null,
//                                                DateTime?                RequestTimestamp    = null,
//                                                TimeSpan?                RequestTimeout      = null,
//                                                EventTracking_Id?        EventTrackingId     = null,
//                                                CancellationToken        CancellationToken   = default)

//            => ICSMSClient.Reset(
//                   new ResetRequest(
//                       ChargingStationId,
//                       ResetType,
//                       EVSEId,

//                       SignKeys,
//                       SignInfos,
//                       SignaturePolicy,
//                       Signatures,

//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region UpdateFirmware             (ChargingStationId, FirmwareURL, RetrieveDate, Retries = null, RetryInterval = null, ...)

//        /// <summary>
//        /// Initiate a firmware download from the given location at the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="Firmware">The firmware image to be installed at the charging station.</param>
//        /// <param name="UpdateFirmwareRequestId">The update firmware request identification.</param>
//        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
//        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<UpdateFirmwareResponse> UpdateFirmware(this ICSMSClient         ICSMSClient,
//                                                                  ChargeBox_Id             ChargingStationId,
//                                                                  Firmware                 Firmware,
//                                                                  Int32                    UpdateFirmwareRequestId,
//                                                                  Byte?                    Retries             = null,
//                                                                  TimeSpan?                RetryInterval       = null,

//                                                                  IEnumerable<KeyPair>?    SignKeys            = null,
//                                                                  IEnumerable<SignInfo>?   SignInfos           = null,
//                                                                  SignaturePolicy?         SignaturePolicy     = null,
//                                                                  IEnumerable<Signature>?  Signatures          = null,

//                                                                  CustomData?              CustomData          = null,

//                                                                  Request_Id?              RequestId           = null,
//                                                                  DateTime?                RequestTimestamp    = null,
//                                                                  TimeSpan?                RequestTimeout      = null,
//                                                                  EventTracking_Id?        EventTrackingId     = null,
//                                                                  CancellationToken        CancellationToken   = default)

//            => ICSMSClient.UpdateFirmware(
//                   new UpdateFirmwareRequest(
//                       ChargingStationId,
//                       Firmware,
//                       UpdateFirmwareRequestId,
//                       Retries,
//                       RetryInterval,

//                       SignKeys,
//                       SignInfos,
//                       SignaturePolicy,
//                       Signatures,

//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region PublishFirmware            (ChargingStationId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

//        /// <summary>
//        /// Publish a firmware image.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="PublishFirmwareRequestId">The unique identification of this publish firmware request</param>
//        /// <param name="DownloadLocation">An URL for downloading the firmware.onto the local controller.</param>
//        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
//        /// <param name="Retries">The optional number of retries of a charging station for trying to download the firmware before giving up. If this field is not present, it is left to the charging station to decide how many times it wants to retry.</param>
//        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charging station to decide how long to wait between attempts.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<PublishFirmwareResponse> PublishFirmware(this ICSMSClient         ICSMSClient,
//                                                                    ChargeBox_Id             ChargingStationId,
//                                                                    Int32                    PublishFirmwareRequestId,
//                                                                    URL                      DownloadLocation,
//                                                                    String                   MD5Checksum,
//                                                                    Byte?                    Retries             = null,
//                                                                    TimeSpan?                RetryInterval       = null,

//                                                                    IEnumerable<KeyPair>?    SignKeys            = null,
//                                                                    IEnumerable<SignInfo>?   SignInfos           = null,
//                                                                    SignaturePolicy?         SignaturePolicy     = null,
//                                                                    IEnumerable<Signature>?  Signatures          = null,

//                                                                    CustomData?              CustomData          = null,

//                                                                    Request_Id?              RequestId           = null,
//                                                                    DateTime?                RequestTimestamp    = null,
//                                                                    TimeSpan?                RequestTimeout      = null,
//                                                                    EventTracking_Id?        EventTrackingId     = null,
//                                                                    CancellationToken        CancellationToken   = default)

//            => ICSMSClient.PublishFirmware(
//                   new PublishFirmwareRequest(
//                       ChargingStationId,
//                       PublishFirmwareRequestId,
//                       DownloadLocation,
//                       MD5Checksum,
//                       Retries,
//                       RetryInterval,

//                       SignKeys,
//                       SignInfos,
//                       SignaturePolicy,
//                       Signatures,

//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region UnpublishFirmware          (ChargingStationId, MD5Checksum, ...)

//        /// <summary>
//        /// Unpublish a firmware image.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<UnpublishFirmwareResponse> UnpublishFirmware(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        String                   MD5Checksum,

//                                                                        IEnumerable<KeyPair>?    SignKeys            = null,
//                                                                        IEnumerable<SignInfo>?   SignInfos           = null,
//                                                                        SignaturePolicy?         SignaturePolicy     = null,
//                                                                        IEnumerable<Signature>?  Signatures          = null,

//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.UnpublishFirmware(
//                   new UnpublishFirmwareRequest(
//                       ChargingStationId,
//                       MD5Checksum,

//                       SignKeys,
//                       SignInfos,
//                       SignaturePolicy,
//                       Signatures,

//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetBaseReport              (ChargingStationId, GetBaseReportRequestId, ReportBase, ...)

//        /// <summary>
//        /// Get a base report.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="GetBaseReportRequestId">An unique identification of the get base report request.</param>
//        /// <param name="ReportBase">The requested reporting base.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetBaseReportResponse> GetBaseReport(this ICSMSClient         ICSMSClient,
//                                                                ChargeBox_Id             ChargingStationId,
//                                                                Int64                    GetBaseReportRequestId,
//                                                                ReportBases              ReportBase,

//                                                                IEnumerable<KeyPair>?    SignKeys            = null,
//                                                                IEnumerable<SignInfo>?   SignInfos           = null,
//                                                                SignaturePolicy?         SignaturePolicy     = null,
//                                                                IEnumerable<Signature>?  Signatures          = null,

//                                                                CustomData?              CustomData          = null,

//                                                                Request_Id?              RequestId           = null,
//                                                                DateTime?                RequestTimestamp    = null,
//                                                                TimeSpan?                RequestTimeout      = null,
//                                                                EventTracking_Id?        EventTrackingId     = null,
//                                                                CancellationToken        CancellationToken   = default)

//            => ICSMSClient.GetBaseReport(
//                   new GetBaseReportRequest(
//                       ChargingStationId,
//                       GetBaseReportRequestId,
//                       ReportBase,

//                       SignKeys,
//                       SignInfos,
//                       SignaturePolicy,
//                       Signatures,

//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetReport                  (ChargingStationId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

//        /// <summary>
//        /// Get a report.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="GetReportRequestId">The charging station identification.</param>
//        /// <param name="ComponentCriteria">An optional enumeration of criteria for components for which a report is requested.</param>
//        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a report is requested.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetReportResponse> GetReport(this ICSMSClient                ICSMSClient,
//                                                        ChargeBox_Id                    ChargingStationId,
//                                                        Int32                           GetReportRequestId,
//                                                        IEnumerable<ComponentCriteria>  ComponentCriteria,
//                                                        IEnumerable<ComponentVariable>  ComponentVariables,

//                                                        IEnumerable<Signature>?         Signatures          = null,
//                                                        CustomData?                     CustomData          = null,

//                                                        Request_Id?                     RequestId           = null,
//                                                        DateTime?                       RequestTimestamp    = null,
//                                                        TimeSpan?                       RequestTimeout      = null,
//                                                        EventTracking_Id?               EventTrackingId     = null,
//                                                        CancellationToken               CancellationToken   = default)

//            => ICSMSClient.GetReport(
//                   new GetReportRequest(
//                       ChargingStationId,
//                       GetReportRequestId,
//                       ComponentCriteria,
//                       ComponentVariables,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetLog                     (ChargingStationId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

//        /// <summary>
//        /// Get a log(file).
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="LogType">The type of the certificates requested.</param>
//        /// <param name="LogRequestId">The unique identification of this request.</param>
//        /// <param name="Log">This field specifies the requested log and the location to which the log should be sent.</param>
//        /// <param name="Retries">This specifies how many times the Charge Point must try to upload the log before giving up. If this field is not present, it is left to Charge Point to decide how many times it wants to retry.</param>
//        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to Charge Point to decide how long to wait between attempts.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetLogResponse> GetLog(this ICSMSClient         ICSMSClient,
//                                                  ChargeBox_Id             ChargingStationId,
//                                                  LogTypes                 LogType,
//                                                  Int32                    LogRequestId,
//                                                  LogParameters            Log,
//                                                  Byte?                    Retries             = null,
//                                                  TimeSpan?                RetryInterval       = null,

//                                                  IEnumerable<Signature>?  Signatures          = null,
//                                                  CustomData?              CustomData          = null,

//                                                  Request_Id?              RequestId           = null,
//                                                  DateTime?                RequestTimestamp    = null,
//                                                  TimeSpan?                RequestTimeout      = null,
//                                                  EventTracking_Id?        EventTrackingId     = null,
//                                                  CancellationToken        CancellationToken   = default)

//            => ICSMSClient.GetLog(
//                   new GetLogRequest(
//                       ChargingStationId,
//                       LogType,
//                       LogRequestId,
//                       Log,
//                       Retries,
//                       RetryInterval,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetVariables               (ChargingStationId, VariableData, ...)

//        /// <summary>
//        /// Set variables.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="VariableData">An enumeration of set variable data.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetVariablesResponse> SetVariables(this ICSMSClient              ICSMSClient,
//                                                              ChargeBox_Id                  ChargingStationId,
//                                                              IEnumerable<SetVariableData>  VariableData,

//                                                              IEnumerable<Signature>?       Signatures          = null,
//                                                              CustomData?                   CustomData          = null,

//                                                              Request_Id?                   RequestId           = null,
//                                                              DateTime?                     RequestTimestamp    = null,
//                                                              TimeSpan?                     RequestTimeout      = null,
//                                                              EventTracking_Id?             EventTrackingId     = null,
//                                                              CancellationToken             CancellationToken   = default)

//            => ICSMSClient.SetVariables(
//                   new SetVariablesRequest(
//                       ChargingStationId,
//                       VariableData,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetVariables               (ChargingStationId, VariableData, ...)

//        /// <summary>
//        /// Get variables.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="VariableData">An enumeration of requested variable data sets.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetVariablesResponse> GetVariables(this ICSMSClient              ICSMSClient,
//                                                              ChargeBox_Id                  ChargingStationId,
//                                                              IEnumerable<GetVariableData>  VariableData,

//                                                              IEnumerable<Signature>?       Signatures          = null,
//                                                              CustomData?                   CustomData          = null,

//                                                              Request_Id?                   RequestId           = null,
//                                                              DateTime?                     RequestTimestamp    = null,
//                                                              TimeSpan?                     RequestTimeout      = null,
//                                                              EventTracking_Id?             EventTrackingId     = null,
//                                                              CancellationToken             CancellationToken   = default)

//            => ICSMSClient.GetVariables(
//                   new GetVariablesRequest(
//                       ChargingStationId,
//                       VariableData,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetMonitoringBase          (ChargingStationId, MonitoringBase, ...)

//        /// <summary>
//        /// Set the monitoring base.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="MonitoringBase">The monitoring base to be set.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetMonitoringBaseResponse> SetMonitoringBase(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        MonitoringBases          MonitoringBase,

//                                                                        IEnumerable<Signature>?  Signatures          = null,
//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SetMonitoringBase(
//                   new SetMonitoringBaseRequest(
//                       ChargingStationId,
//                       MonitoringBase,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetMonitoringReport        (ChargingStationId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

//        /// <summary>
//        /// Get a monitoring report.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="GetMonitoringReportRequestId">The charging station identification.</param>
//        /// <param name="MonitoringCriteria">An optional enumeration of criteria for components for which a monitoring report is requested.</param>
//        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a monitoring report is requested.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetMonitoringReportResponse> GetMonitoringReport(this ICSMSClient                 ICSMSClient,
//                                                                            ChargeBox_Id                     ChargingStationId,
//                                                                            Int32                            GetMonitoringReportRequestId,
//                                                                            IEnumerable<MonitoringCriteria>  MonitoringCriteria,
//                                                                            IEnumerable<ComponentVariable>   ComponentVariables,

//                                                                            IEnumerable<Signature>?          Signatures          = null,
//                                                                            CustomData?                      CustomData          = null,

//                                                                            Request_Id?                      RequestId           = null,
//                                                                            DateTime?                        RequestTimestamp    = null,
//                                                                            TimeSpan?                        RequestTimeout      = null,
//                                                                            EventTracking_Id?                EventTrackingId     = null,
//                                                                            CancellationToken                CancellationToken   = default)

//            => ICSMSClient.GetMonitoringReport(
//                   new GetMonitoringReportRequest(
//                       ChargingStationId,
//                       GetMonitoringReportRequestId,
//                       MonitoringCriteria,
//                       ComponentVariables,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetMonitoringLevel         (ChargingStationId, Severity, ...)

//        /// <summary>
//        /// Set the monitoring level.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="Severity">The charging station SHALL only report events with a severity number lower than or equal to this severity.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetMonitoringLevelResponse> SetMonitoringLevel(this ICSMSClient         ICSMSClient,
//                                                                          ChargeBox_Id             ChargingStationId,
//                                                                          Severities               Severity,

//                                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                                          CustomData?              CustomData          = null,

//                                                                          Request_Id?              RequestId           = null,
//                                                                          DateTime?                RequestTimestamp    = null,
//                                                                          TimeSpan?                RequestTimeout      = null,
//                                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SetMonitoringLevel(
//                   new SetMonitoringLevelRequest(
//                       ChargingStationId,
//                       Severity,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetVariableMonitoring      (ChargingStationId, MonitoringData, ...)

//        /// <summary>
//        /// Set a variable monitoring.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="MonitoringData">An enumeration of monitoring data.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetVariableMonitoringResponse> SetVariableMonitoring(this ICSMSClient                ICSMSClient,
//                                                                                ChargeBox_Id                    ChargingStationId,
//                                                                                IEnumerable<SetMonitoringData>  MonitoringData,

//                                                                                IEnumerable<Signature>?         Signatures          = null,
//                                                                                CustomData?                     CustomData          = null,

//                                                                                Request_Id?                     RequestId           = null,
//                                                                                DateTime?                       RequestTimestamp    = null,
//                                                                                TimeSpan?                       RequestTimeout      = null,
//                                                                                EventTracking_Id?               EventTrackingId     = null,
//                                                                                CancellationToken               CancellationToken   = default)

//            => ICSMSClient.SetVariableMonitoring(
//                   new SetVariableMonitoringRequest(
//                       ChargingStationId,
//                       MonitoringData,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region ClearVariableMonitoring    (ChargingStationId, VariableMonitoringIds, ...)

//        /// <summary>
//        /// Remove the given variable monitoring.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="VariableMonitoringIds">An enumeration of variable monitoring identifications to clear.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(this ICSMSClient                    ICSMSClient,
//                                                                                    ChargeBox_Id                        ChargingStationId,
//                                                                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

//                                                                                    IEnumerable<Signature>?             Signatures          = null,
//                                                                                    CustomData?                         CustomData          = null,

//                                                                                    Request_Id?                         RequestId           = null,
//                                                                                    DateTime?                           RequestTimestamp    = null,
//                                                                                    TimeSpan?                           RequestTimeout      = null,
//                                                                                    EventTracking_Id?                   EventTrackingId     = null,
//                                                                                    CancellationToken                   CancellationToken   = default)

//            => ICSMSClient.ClearVariableMonitoring(
//                   new ClearVariableMonitoringRequest(
//                       ChargingStationId,
//                       VariableMonitoringIds,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetNetworkProfile          (ChargingStationId, ConfigurationSlot, NetworkConnectionProfile, ...)

//        /// <summary>
//        /// Set the network profile.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ConfigurationSlot">The slot in which the configuration should be stored.</param>
//        /// <param name="NetworkConnectionProfile">The network connection configuration.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetNetworkProfileResponse> SetNetworkProfile(this ICSMSClient          ICSMSClient,
//                                                                        ChargeBox_Id              ChargingStationId,
//                                                                        Int32                     ConfigurationSlot,
//                                                                        NetworkConnectionProfile  NetworkConnectionProfile,

//                                                                        IEnumerable<Signature>?   Signatures          = null,
//                                                                        CustomData?               CustomData          = null,

//                                                                        Request_Id?               RequestId           = null,
//                                                                        DateTime?                 RequestTimestamp    = null,
//                                                                        TimeSpan?                 RequestTimeout      = null,
//                                                                        EventTracking_Id?         EventTrackingId     = null,
//                                                                        CancellationToken         CancellationToken   = default)

//            => ICSMSClient.SetNetworkProfile(
//                   new SetNetworkProfileRequest(
//                       ChargingStationId,
//                       ConfigurationSlot,
//                       NetworkConnectionProfile,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region ChangeAvailability         (ChargingStationId, OperationalStatus, EVSE, ...)

//        /// <summary>
//        /// Change the availability of the given charging station or EVSE.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="OperationalStatus">A new operational status of the charging station or EVSE.</param>
//        /// <param name="EVSE">Optional identification of an EVSE/connector for which the operational status should be changed.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ChangeAvailabilityResponse> ChangeAvailability(this ICSMSClient         ICSMSClient,
//                                                                          ChargeBox_Id             ChargingStationId,
//                                                                          OperationalStatus        OperationalStatus,
//                                                                          EVSE?                    EVSE                = null,

//                                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                                          CustomData?              CustomData          = null,

//                                                                          Request_Id?              RequestId           = null,
//                                                                          DateTime?                RequestTimestamp    = null,
//                                                                          TimeSpan?                RequestTimeout      = null,
//                                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.ChangeAvailability(
//                   new ChangeAvailabilityRequest(
//                       ChargingStationId,
//                       OperationalStatus,
//                       EVSE,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region TriggerMessage             (ChargingStationId, RequestedMessage, EVSE = null, ...)

//        /// <summary>
//        /// Create a trigger for the given message at the given charging station or EVSE.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="RequestedMessage">The message to trigger.</param>
//        /// <param name="EVSE">An optional EVSE (and connector) identification whenever the message applies to a specific EVSE and/or connector.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<TriggerMessageResponse> TriggerMessage(this ICSMSClient         ICSMSClient,
//                                                                  ChargeBox_Id             ChargingStationId,
//                                                                  MessageTriggers          RequestedMessage,
//                                                                  EVSE?                    EVSE                = null,

//                                                                  IEnumerable<Signature>?  Signatures          = null,
//                                                                  CustomData?              CustomData          = null,

//                                                                  Request_Id?              RequestId           = null,
//                                                                  DateTime?                RequestTimestamp    = null,
//                                                                  TimeSpan?                RequestTimeout      = null,
//                                                                  EventTracking_Id?        EventTrackingId     = null,
//                                                                  CancellationToken        CancellationToken   = default)

//            => ICSMSClient.TriggerMessage(
//                   new TriggerMessageRequest(
//                       ChargingStationId,
//                       RequestedMessage,
//                       EVSE,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region TransferData               (ChargingStationId, VendorId, MessageId, Data, ...)

//        /// <summary>
//        /// Send the given vendor-specific data.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
//        /// <param name="MessageId">An optional message identification field.</param>
//        /// <param name="Data">Optional message data without specified length or format.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CS.DataTransferResponse> TransferData(this ICSMSClient         ICSMSClient,
//                                                                 ChargeBox_Id             ChargingStationId,
//                                                                 Vendor_Id                VendorId,
//                                                                 String?                  MessageId           = null,
//                                                                 JToken?                  Data                = null,

//                                                                 IEnumerable<Signature>?  Signatures          = null,
//                                                                 CustomData?              CustomData          = null,

//                                                                 Request_Id?              RequestId           = null,
//                                                                 DateTime?                RequestTimestamp    = null,
//                                                                 TimeSpan?                RequestTimeout      = null,
//                                                                 EventTracking_Id?        EventTrackingId     = null,
//                                                                 CancellationToken        CancellationToken   = default)

//            => ICSMSClient.TransferData(
//                   new DataTransferRequest(
//                       ChargingStationId,
//                       VendorId,
//                       MessageId,
//                       Data,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//        #region SendSignedCertificate      (ChargingStationId, CertificateChain, CertificateType, ...)

//        /// <summary>
//        /// Send the signed certificate to the charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
//        /// <param name="CertificateType">The certificate/key usage.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CertificateSignedResponse> SendSignedCertificate(this ICSMSClient         ICSMSClient,
//                                                                            ChargeBox_Id             ChargingStationId,
//                                                                            CertificateChain         CertificateChain,
//                                                                            CertificateSigningUse?   CertificateType     = null,

//                                                                            IEnumerable<Signature>?  Signatures          = null,
//                                                                            CustomData?              CustomData          = null,
                                                                                                    
//                                                                            Request_Id?              RequestId           = null,
//                                                                            DateTime?                RequestTimestamp    = null,
//                                                                            TimeSpan?                RequestTimeout      = null,
//                                                                            EventTracking_Id?        EventTrackingId     = null,
//                                                                            CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SendSignedCertificate(
//                   new CertificateSignedRequest(
//                       ChargingStationId,
//                       CertificateChain,
//                       CertificateType,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region InstallCertificate         (ChargingStationId, CertificateType, Certificate, ...)

//        /// <summary>
//        /// Install the given certificate within the charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="CertificateType">The type of the certificate.</param>
//        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<InstallCertificateResponse> InstallCertificate(this ICSMSClient         ICSMSClient,
//                                                                          ChargeBox_Id             ChargingStationId,
//                                                                          CertificateUse           CertificateType,
//                                                                          Certificate              Certificate,
//                                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                                          CustomData?              CustomData          = null,

//                                                                          Request_Id?              RequestId           = null,
//                                                                          DateTime?                RequestTimestamp    = null,
//                                                                          TimeSpan?                RequestTimeout      = null,
//                                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.InstallCertificate(
//                   new InstallCertificateRequest(
//                       ChargingStationId,
//                       CertificateType,
//                       Certificate,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetInstalledCertificateIds (ChargingStationId, CertificateTypes, ...)

//        /// <summary>
//        /// Retrieve a list of all installed certificates within the charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="CertificateTypes">An optional enumeration of certificate types requested.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(this ICSMSClient              ICSMSClient,
//                                                                                          ChargeBox_Id                  ChargingStationId,
//                                                                                          IEnumerable<CertificateUse>?  CertificateTypes    = null,

//                                                                                          IEnumerable<Signature>?       Signatures          = null,
//                                                                                          CustomData?                   CustomData          = null,

//                                                                                          Request_Id?                   RequestId           = null,
//                                                                                          DateTime?                     RequestTimestamp    = null,
//                                                                                          TimeSpan?                     RequestTimeout      = null,
//                                                                                          EventTracking_Id?             EventTrackingId     = null,
//                                                                                          CancellationToken             CancellationToken   = default)

//            => ICSMSClient.GetInstalledCertificateIds(
//                   new GetInstalledCertificateIdsRequest(
//                       ChargingStationId,
//                       CertificateTypes,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region DeleteCertificate          (ChargingStationId, CertificateHashData, ...)

//        /// <summary>
//        /// Remove the given certificate from the charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<DeleteCertificateResponse> DeleteCertificate(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        CertificateHashData      CertificateHashData,

//                                                                        IEnumerable<Signature>?  Signatures          = null,
//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.DeleteCertificate(
//                   new DeleteCertificateRequest(
//                       ChargingStationId,
//                       CertificateHashData,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region NotifyCRLAvailability      (ChargingStationId, NotifyCRLRequestId, Availability, Location, ...)

//        /// <summary>
//        /// Notify the charging station about the status of a certificate revocation list.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="NotifyCRLRequestId">An unique identification of this request.</param>
//        /// <param name="Availability">An availability status of the certificate revocation list.</param>
//        /// <param name="Location">An optional location of the certificate revocation list.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<NotifyCRLResponse> NotifyCRLAvailability(this ICSMSClient         ICSMSClient,
//                                                                    ChargeBox_Id             ChargingStationId,
//                                                                    Int32                    NotifyCRLRequestId,
//                                                                    NotifyCRLStatus          Availability,
//                                                                    URL?                     Location,

//                                                                    IEnumerable<Signature>?  Signatures          = null,
//                                                                    CustomData?              CustomData          = null,

//                                                                    Request_Id?              RequestId           = null,
//                                                                    DateTime?                RequestTimestamp    = null,
//                                                                    TimeSpan?                RequestTimeout      = null,
//                                                                    EventTracking_Id?        EventTrackingId     = null,
//                                                                    CancellationToken        CancellationToken   = default)

//            => ICSMSClient.NotifyCRLAvailability(
//                   new NotifyCRLRequest(
//                       ChargingStationId,
//                       NotifyCRLRequestId,
//                       Availability,
//                       Location,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//        #region GetLocalListVersion        (ChargingStationId, ...)

//        /// <summary>
//        /// Return the local white list of the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetLocalListVersionResponse> GetLocalListVersion(this ICSMSClient         ICSMSClient,
//                                                                            ChargeBox_Id             ChargingStationId,

//                                                                            IEnumerable<Signature>?  Signatures          = null,
//                                                                            CustomData?              CustomData          = null,

//                                                                            Request_Id?              RequestId           = null,
//                                                                            DateTime?                RequestTimestamp    = null,
//                                                                            TimeSpan?                RequestTimeout      = null,
//                                                                            EventTracking_Id?        EventTrackingId     = null,
//                                                                            CancellationToken        CancellationToken   = default)

//            => ICSMSClient.GetLocalListVersion(
//                   new GetLocalListVersionRequest(
//                       ChargingStationId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SendLocalList              (ChargingStationId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

//        /// <summary>
//        /// Set the local white liste at the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
//        /// <param name="UpdateType">The type of update (full or differential).</param>
//        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charging station. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SendLocalListResponse> SendLocalList(this ICSMSClient                 ICSMSClient,
//                                                                ChargeBox_Id                     ChargingStationId,
//                                                                UInt64                           ListVersion,
//                                                                UpdateTypes                      UpdateType,
//                                                                IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

//                                                                IEnumerable<Signature>?          Signatures               = null,
//                                                                CustomData?                      CustomData               = null,

//                                                                Request_Id?                      RequestId                = null,
//                                                                DateTime?                        RequestTimestamp         = null,
//                                                                TimeSpan?                        RequestTimeout           = null,
//                                                                EventTracking_Id?                EventTrackingId          = null,
//                                                                CancellationToken                CancellationToken        = default)

//            => ICSMSClient.SendLocalList(
//                   new SendLocalListRequest(
//                       ChargingStationId,
//                       ListVersion,
//                       UpdateType,
//                       LocalAuthorizationList,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region ClearCache                 (ChargingStationId, ...)

//        /// <summary>
//        /// Clear the local white liste cache of the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ClearCacheResponse> ClearCache(this ICSMSClient         ICSMSClient,
//                                                          ChargeBox_Id             ChargingStationId,

//                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                          CustomData?              CustomData          = null,

//                                                          Request_Id?              RequestId           = null,
//                                                          DateTime?                RequestTimestamp    = null,
//                                                          TimeSpan?                RequestTimeout      = null,
//                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.ClearCache(
//                   new ClearCacheRequest(
//                       ChargingStationId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//        #region ReserveNow                 (ChargingStationId, ReservationId, ExpiryDate, IdToken, ConnectorType = null, EVSEId = null, GroupIdToken = null, ...)

//        /// <summary>
//        /// Create a charging reservation at the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ReservationId">The unique identification of this reservation.</param>
//        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
//        /// <param name="IdToken">The unique token identification for which the reservation is being made.</param>
//        /// <param name="ConnectorType">An optional connector type to be reserved..</param>
//        /// <param name="EVSEId">The identification of the EVSE to be reserved. A value of 0 means that the reservation is not for a specific EVSE.</param>
//        /// <param name="GroupIdToken">An optional group identifier for which the reservation is being made.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ReserveNowResponse> ReserveNow(this ICSMSClient         ICSMSClient,
//                                                          ChargeBox_Id             ChargingStationId,
//                                                          Reservation_Id           ReservationId,
//                                                          DateTime                 ExpiryDate,
//                                                          IdToken                  IdToken,
//                                                          ConnectorTypes?          ConnectorType       = null,
//                                                          EVSE_Id?                 EVSEId              = null,
//                                                          IdToken?                 GroupIdToken        = null,

//                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                          CustomData?              CustomData          = null,

//                                                          Request_Id?              RequestId           = null,
//                                                          DateTime?                RequestTimestamp    = null,
//                                                          TimeSpan?                RequestTimeout      = null,
//                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.ReserveNow(
//                   new ReserveNowRequest(
//                       ChargingStationId,
//                       ReservationId,
//                       ExpiryDate,
//                       IdToken,
//                       ConnectorType,
//                       EVSEId,
//                       GroupIdToken,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region CancelReservation          (ChargingStationId, ReservationId, ...)

//        /// <summary>
//        /// Cancel the given charging reservation.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ReservationId">The unique identification of the reservation to cancel.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CancelReservationResponse> CancelReservation(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        Reservation_Id           ReservationId,

//                                                                        IEnumerable<Signature>?  Signatures          = null,
//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.CancelReservation(
//                   new CancelReservationRequest(
//                       ChargingStationId,
//                       ReservationId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region StartCharging              (ChargingStationId, RequestStartTransactionRequestId, IdToken, EVSEId = null, ChargingProfile = null, GroupIdToken = null, ...)

//        /// <summary>
//        /// Start a charging process (transaction).
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="RequestStartTransactionRequestId">Request identification given by the server to this start request. The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.</param>
//        /// <param name="IdToken">The identification token to start the charging transaction.</param>
//        /// <param name="EVSEId">An optional EVSE identification on which the charging transaction should be started (SHALL be > 0).</param>
//        /// <param name="ChargingProfile">An optional charging profile to be used by the charging station for the requested charging transaction.</param>
//        /// <param name="GroupIdToken">An optional group identifier.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<RequestStartTransactionResponse> StartCharging(this ICSMSClient         ICSMSClient,
//                                                                          ChargeBox_Id             ChargingStationId,
//                                                                          RemoteStart_Id           RequestStartTransactionRequestId,
//                                                                          IdToken                  IdToken,
//                                                                          EVSE_Id?                 EVSEId              = null,
//                                                                          ChargingProfile?         ChargingProfile     = null,
//                                                                          IdToken?                 GroupIdToken        = null,

//                                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                                          CustomData?              CustomData          = null,

//                                                                          Request_Id?              RequestId           = null,
//                                                                          DateTime?                RequestTimestamp    = null,
//                                                                          TimeSpan?                RequestTimeout      = null,
//                                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.StartCharging(
//                   new RequestStartTransactionRequest(
//                       ChargingStationId,
//                       RequestStartTransactionRequestId,
//                       IdToken,
//                       EVSEId,
//                       ChargingProfile,
//                       GroupIdToken,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region StopCharging               (ChargingStationId, TransactionId, ...)

//        /// <summary>
//        /// Start a charging process (transaction).
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="TransactionId">The identification of the transaction which the charging station is requested to stop.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<RequestStopTransactionResponse> StopCharging(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        Transaction_Id           TransactionId,

//                                                                        IEnumerable<Signature>?  Signatures          = null,
//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.StopCharging(
//                   new RequestStopTransactionRequest(
//                       ChargingStationId,
//                       TransactionId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetTransactionStatus       (ChargingStationId, TransactionId, ...)

//        /// <summary>
//        /// Get the status of a charging process (transaction).
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="TransactionId">The identification of the transaction which the charging station is requested to stop.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetTransactionStatusResponse> GetTransactionStatus(this ICSMSClient         ICSMSClient,
//                                                                              ChargeBox_Id             ChargingStationId,
//                                                                              Transaction_Id?          TransactionId,

//                                                                              IEnumerable<Signature>?  Signatures          = null,
//                                                                              CustomData?              CustomData          = null,

//                                                                              Request_Id?              RequestId           = null,
//                                                                              DateTime?                RequestTimestamp    = null,
//                                                                              TimeSpan?                RequestTimeout      = null,
//                                                                              EventTracking_Id?        EventTrackingId     = null,
//                                                                              CancellationToken        CancellationToken   = default)

//            => ICSMSClient.GetTransactionStatus(
//                   new GetTransactionStatusRequest(
//                       ChargingStationId,
//                       TransactionId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region SetChargingProfile         (ChargingStationId, EVSEId, ChargingProfile, ...)

//        /// <summary>
//        /// Set the charging profile of the given EVSE at the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="EVSEId">The EVSE identification to which the charging profile applies.</param>
//        /// <param name="ChargingProfile">The charging profile to be set.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetChargingProfileResponse> SetChargingProfile(this ICSMSClient         ICSMSClient,
//                                                                          ChargeBox_Id             ChargingStationId,
//                                                                          EVSE_Id                  EVSEId,
//                                                                          ChargingProfile          ChargingProfile,

//                                                                          IEnumerable<Signature>?  Signatures          = null,
//                                                                          CustomData?              CustomData          = null,

//                                                                          Request_Id?              RequestId           = null,
//                                                                          DateTime?                RequestTimestamp    = null,
//                                                                          TimeSpan?                RequestTimeout      = null,
//                                                                          EventTracking_Id?        EventTrackingId     = null,
//                                                                          CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SetChargingProfile(
//                   new SetChargingProfileRequest(
//                       ChargingStationId,
//                       EVSEId,
//                       ChargingProfile,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetChargingProfiles        (ChargingStationId, GetChargingProfilesRequestId, ChargingProfile, EVSEId = null, ...)

//        /// <summary>
//        /// Get all charging profiles from the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="GetChargingProfilesRequestId">An unique identification of the get charging profiles request.</param>
//        /// <param name="ChargingProfile">Machting charging profiles.</param>
//        /// <param name="EVSEId">Optional EVSE identification of the EVSE for which the installed charging profiles SHALL be reported. If 0, only charging profiles installed on the charging station itself (the grid connection) SHALL be reported.If omitted, all installed charging profiles SHALL be reported.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetChargingProfilesResponse> GetChargingProfiles(this ICSMSClient          ICSMSClient,
//                                                                            ChargeBox_Id              ChargingStationId,
//                                                                            Int64                     GetChargingProfilesRequestId,
//                                                                            ChargingProfileCriterion  ChargingProfile,
//                                                                            EVSE_Id?                  EVSEId              = null,

//                                                                            IEnumerable<Signature>?   Signatures          = null,
//                                                                            CustomData?               CustomData          = null,

//                                                                            Request_Id?               RequestId           = null,
//                                                                            DateTime?                 RequestTimestamp    = null,
//                                                                            TimeSpan?                 RequestTimeout      = null,
//                                                                            EventTracking_Id?         EventTrackingId     = null,
//                                                                            CancellationToken         CancellationToken   = default)

//            => ICSMSClient.GetChargingProfiles(
//                   new GetChargingProfilesRequest(
//                       ChargingStationId,
//                       GetChargingProfilesRequestId,
//                       ChargingProfile,
//                       EVSEId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region ClearChargingProfile       (ChargingStationId, ChargingProfileId = null, ChargingProfileCriteria = null, ...)

//        /// <summary>
//        /// Remove matching charging profiles from the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
//        /// <param name="ChargingProfileCriteria">An optional specification of the charging profile to clear.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ClearChargingProfileResponse> ClearChargingProfile(this ICSMSClient         ICSMSClient,
//                                                                              ChargeBox_Id             ChargingStationId,
//                                                                              ChargingProfile_Id?      ChargingProfileId         = null,
//                                                                              ClearChargingProfile?    ChargingProfileCriteria   = null,

//                                                                              IEnumerable<Signature>?  Signatures                = null,
//                                                                              CustomData?              CustomData                = null,

//                                                                              Request_Id?              RequestId                 = null,
//                                                                              DateTime?                RequestTimestamp          = null,
//                                                                              TimeSpan?                RequestTimeout            = null,
//                                                                              EventTracking_Id?        EventTrackingId           = null,
//                                                                              CancellationToken        CancellationToken         = default)

//            => ICSMSClient.ClearChargingProfile(
//                   new ClearChargingProfileRequest(
//                       ChargingStationId,
//                       ChargingProfileId,
//                       ChargingProfileCriteria,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetCompositeSchedule       (ChargingStationId, Duration, EVSEId, ChargingRateUnit = null, ...)

//        /// <summary>
//        /// Return the charging schedule at the given charging station and EVSE
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="Duration">The length of requested schedule.</param>
//        /// <param name="EVSEId">The EVSE identification for which the schedule is requested. EVSE identification is 0, the charging station will calculate the expected consumption for the grid connection.</param>
//        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetCompositeScheduleResponse> GetCompositeSchedule(this ICSMSClient         ICSMSClient,
//                                                                              ChargeBox_Id             ChargingStationId,
//                                                                              TimeSpan                 Duration,
//                                                                              EVSE_Id                  EVSEId,
//                                                                              ChargingRateUnits?       ChargingRateUnit    = null,

//                                                                              IEnumerable<Signature>?  Signatures          = null,
//                                                                              CustomData?              CustomData          = null,

//                                                                              Request_Id?              RequestId           = null,
//                                                                              DateTime?                RequestTimestamp    = null,
//                                                                              TimeSpan?                RequestTimeout      = null,
//                                                                              EventTracking_Id?        EventTrackingId     = null,
//                                                                              CancellationToken        CancellationToken   = default)

//            => ICSMSClient.GetCompositeSchedule(
//                   new GetCompositeScheduleRequest(
//                       ChargingStationId,
//                       Duration,
//                       EVSEId,
//                       ChargingRateUnit,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region UpdateDynamicSchedule      (ChargingStationId, ChargingProfileId, Limit = null, ...)

//        /// <summary>
//        /// Update the dynamic charging schedule for the given charging profile.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="ChargingProfileId">The identification of the charging profile to update.</param>
//        /// 
//        /// <param name="Limit">Optional charging rate limit in chargingRateUnit.</param>
//        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2.</param>
//        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3.</param>
//        /// 
//        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit.</param>
//        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2.</param>
//        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3.</param>
//        /// 
//        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
//        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
//        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
//        /// 
//        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
//        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
//        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">An optional timeout for this request.</param>
//        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CS.UpdateDynamicScheduleResponse>

//            UpdateDynamicSchedule(this ICSMSClient         ICSMSClient,
//                                  ChargeBox_Id             ChargingStationId,
//                                  ChargingProfile_Id       ChargingProfileId,

//                                  ChargingRateValue?       Limit                 = null,
//                                  ChargingRateValue?       Limit_L2              = null,
//                                  ChargingRateValue?       Limit_L3              = null,

//                                  ChargingRateValue?       DischargeLimit        = null,
//                                  ChargingRateValue?       DischargeLimit_L2     = null,
//                                  ChargingRateValue?       DischargeLimit_L3     = null,

//                                  ChargingRateValue?       Setpoint              = null,
//                                  ChargingRateValue?       Setpoint_L2           = null,
//                                  ChargingRateValue?       Setpoint_L3           = null,

//                                  ChargingRateValue?       SetpointReactive      = null,
//                                  ChargingRateValue?       SetpointReactive_L2   = null,
//                                  ChargingRateValue?       SetpointReactive_L3   = null,

//                                  IEnumerable<Signature>?  Signatures            = null,
//                                  CustomData?              CustomData            = null,

//                                  Request_Id?              RequestId             = null,
//                                  DateTime?                RequestTimestamp      = null,
//                                  TimeSpan?                RequestTimeout        = null,
//                                  EventTracking_Id?        EventTrackingId       = null,
//                                  CancellationToken        CancellationToken     = default)


//                => ICSMSClient.UpdateDynamicSchedule(
//                       new UpdateDynamicScheduleRequest(

//                           ChargingStationId,
//                           ChargingProfileId,

//                           Limit,
//                           Limit_L2,
//                           Limit_L3,

//                           DischargeLimit,
//                           DischargeLimit_L2,
//                           DischargeLimit_L3,

//                           Setpoint,
//                           Setpoint_L2,
//                           Setpoint_L3,

//                           SetpointReactive,
//                           SetpointReactive_L2,
//                           SetpointReactive_L3,

//                           Signatures,
//                           CustomData,

//                           RequestId,
//                           RequestTimestamp,
//                           RequestTimeout,
//                           EventTrackingId ?? EventTracking_Id.New,
//                           CancellationToken

//                       )
//                   );

//        #endregion

//        #region NotifyAllowedEnergyTransfer(ChargingStationId, AllowedEnergyTransferModes, ...)

//        /// <summary>
//        /// Update the list of authorized energy services.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="AllowedEnergyTransferModes">An enumeration of allowed energy transfer modes.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(this ICSMSClient                  ICSMSClient,
//                                                                                            ChargeBox_Id                      ChargingStationId,
//                                                                                            IEnumerable<EnergyTransferModes>  AllowedEnergyTransferModes,

//                                                                                            IEnumerable<Signature>?           Signatures          = null,
//                                                                                            CustomData?                       CustomData          = null,

//                                                                                            Request_Id?                       RequestId           = null,
//                                                                                            DateTime?                         RequestTimestamp    = null,
//                                                                                            TimeSpan?                         RequestTimeout      = null,
//                                                                                            EventTracking_Id?                 EventTrackingId     = null,
//                                                                                            CancellationToken                 CancellationToken   = default)

//            => ICSMSClient.NotifyAllowedEnergyTransfer(
//                   new NotifyAllowedEnergyTransferRequest(
//                       ChargingStationId,
//                       AllowedEnergyTransferModes,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region UsePriorityCharging(ChargingStationId, TransactionId, Activate, ...)

//        /// <summary>
//        /// Whether to allow priority charging.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
//        /// <param name="Activate">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<UsePriorityChargingResponse> UsePriorityCharging(this ICSMSClient         ICSMSClient,
//                                                                            ChargeBox_Id             ChargingStationId,
//                                                                            Transaction_Id           TransactionId,
//                                                                            Boolean                  Activate,

//                                                                            IEnumerable<Signature>?  Signatures          = null,
//                                                                            CustomData?              CustomData          = null,

//                                                                            Request_Id?              RequestId           = null,
//                                                                            DateTime?                RequestTimestamp    = null,
//                                                                            TimeSpan?                RequestTimeout      = null,
//                                                                            EventTracking_Id?        EventTrackingId     = null,
//                                                                            CancellationToken        CancellationToken   = default)

//            => ICSMSClient.UsePriorityCharging(
//                   new UsePriorityChargingRequest(
//                       ChargingStationId,
//                       TransactionId,
//                       Activate,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region UnlockConnector            (ChargingStationId, EVSEId, ConnectorId, ...)

//        /// <summary>
//        /// Unlock the given EVSE/connector at the given charging station.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="EVSEId">The identifier of the EVSE to be unlocked.</param>
//        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<UnlockConnectorResponse> UnlockConnector(this ICSMSClient         ICSMSClient,
//                                                                    ChargeBox_Id             ChargingStationId,
//                                                                    EVSE_Id                  EVSEId,
//                                                                    Connector_Id             ConnectorId,

//                                                                    IEnumerable<Signature>?  Signatures          = null,
//                                                                    CustomData?              CustomData          = null,

//                                                                    Request_Id?              RequestId           = null,
//                                                                    DateTime?                RequestTimestamp    = null,
//                                                                    TimeSpan?                RequestTimeout      = null,
//                                                                    EventTracking_Id?        EventTrackingId     = null,
//                                                                    CancellationToken        CancellationToken   = default)

//            => ICSMSClient.UnlockConnector(
//                   new UnlockConnectorRequest(
//                       ChargingStationId,
//                       EVSEId,
//                       ConnectorId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//        #region SendAFRRSignal             (ChargingStationId, EVSEId, ConnectorId, ...)

//        /// <summary>
//        /// Send an automatic frequency restoration reserve (AFRR) signal.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
//        /// <param name="Signal">The value of the AFRR signal in v2xSignalWattCurve. Usually between -1 and 1.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<AFRRSignalResponse> SendAFRRSignal(this ICSMSClient         ICSMSClient,
//                                                              ChargeBox_Id             ChargingStationId,
//                                                              DateTime                 ActivationTimestamp,
//                                                              AFRR_Signal              Signal,

//                                                              IEnumerable<Signature>?  Signatures          = null,
//                                                              CustomData?              CustomData          = null,

//                                                              Request_Id?              RequestId           = null,
//                                                              DateTime?                RequestTimestamp    = null,
//                                                              TimeSpan?                RequestTimeout      = null,
//                                                              EventTracking_Id?        EventTrackingId     = null,
//                                                              CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SendAFRRSignal(
//                   new AFRRSignalRequest(
//                       ChargingStationId,
//                       ActivationTimestamp,
//                       Signal,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//        #region SetDisplayMessage          (ChargingStationId, Message, ...)

//        /// <summary>
//        /// Set a display message.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="Message">A display message to be shown at the charging station.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<SetDisplayMessageResponse> SetDisplayMessage(this ICSMSClient         ICSMSClient,
//                                                                        ChargeBox_Id             ChargingStationId,
//                                                                        MessageInfo              Message,

//                                                                        IEnumerable<Signature>?  Signatures          = null,
//                                                                        CustomData?              CustomData          = null,

//                                                                        Request_Id?              RequestId           = null,
//                                                                        DateTime?                RequestTimestamp    = null,
//                                                                        TimeSpan?                RequestTimeout      = null,
//                                                                        EventTracking_Id?        EventTrackingId     = null,
//                                                                        CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SetDisplayMessage(
//                   new SetDisplayMessageRequest(
//                       ChargingStationId,
//                       Message,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region GetDisplayMessages         (ChargingStationId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null,...)

//        /// <summary>
//        /// Get all display messages.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="GetDisplayMessagesRequestId">The unique identification of this get display messages request.</param>
//        /// <param name="Ids">An optional filter on display message identifications. This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.</param>
//        /// <param name="Priority">The optional filter on message priorities.</param>
//        /// <param name="State">The optional filter on message states.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<GetDisplayMessagesResponse> GetDisplayMessages(this ICSMSClient                 ICSMSClient,
//                                                                          ChargeBox_Id                     ChargingStationId,
//                                                                          Int32                            GetDisplayMessagesRequestId,
//                                                                          IEnumerable<DisplayMessage_Id>?  Ids                 = null,
//                                                                          MessagePriorities?               Priority            = null,
//                                                                          MessageStates?                   State               = null,

//                                                                          IEnumerable<Signature>?          Signatures          = null,
//                                                                          CustomData?                      CustomData          = null,

//                                                                          Request_Id?                      RequestId           = null,
//                                                                          DateTime?                        RequestTimestamp    = null,
//                                                                          TimeSpan?                        RequestTimeout      = null,
//                                                                          EventTracking_Id?                EventTrackingId     = null,
//                                                                          CancellationToken                CancellationToken   = default)

//            => ICSMSClient.GetDisplayMessages(
//                   new GetDisplayMessagesRequest(
//                       ChargingStationId,
//                       GetDisplayMessagesRequestId,
//                       Ids,
//                       Priority,
//                       State,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region ClearDisplayMessage        (ChargingStationId, DisplayMessageId,...)

//        /// <summary>
//        /// Remove the given display message.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="DisplayMessageId">The identification of the display message to be removed.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<ClearDisplayMessageResponse> ClearDisplayMessage(this ICSMSClient         ICSMSClient,
//                                                                            ChargeBox_Id             ChargingStationId,
//                                                                            DisplayMessage_Id        DisplayMessageId,

//                                                                            IEnumerable<Signature>?  Signatures          = null,
//                                                                            CustomData?              CustomData          = null,

//                                                                            Request_Id?              RequestId           = null,
//                                                                            DateTime?                RequestTimestamp    = null,
//                                                                            TimeSpan?                RequestTimeout      = null,
//                                                                            EventTracking_Id?        EventTrackingId     = null,
//                                                                            CancellationToken        CancellationToken   = default)

//            => ICSMSClient.ClearDisplayMessage(
//                   new ClearDisplayMessageRequest(
//                       ChargingStationId,
//                       DisplayMessageId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region CostUpdated                (ChargingStationId, TotalCost, TransactionId, ...)

//        /// <summary>
//        /// Send updated cost(s).
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="TotalCost">The current total cost, based on the information known by the CSMS, of the transaction including taxes. In the currency configured with the configuration Variable: [Currency]</param>
//        /// <param name="TransactionId">The unique transaction identification the costs are asked for.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CostUpdatedResponse> SendCostUpdated(this ICSMSClient         ICSMSClient,
//                                                                ChargeBox_Id             ChargingStationId,
//                                                                Decimal                  TotalCost,
//                                                                Transaction_Id           TransactionId,

//                                                                IEnumerable<Signature>?  Signatures          = null,
//                                                                CustomData?              CustomData          = null,

//                                                                Request_Id?              RequestId           = null,
//                                                                DateTime?                RequestTimestamp    = null,
//                                                                TimeSpan?                RequestTimeout      = null,
//                                                                EventTracking_Id?        EventTrackingId     = null,
//                                                                CancellationToken        CancellationToken   = default)

//            => ICSMSClient.SendCostUpdated(
//                   new CostUpdatedRequest(
//                       ChargingStationId,
//                       TotalCost,
//                       TransactionId,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion

//        #region RequestCustomerInformation (ChargingStationId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

//        /// <summary>
//        /// Request customer information.
//        /// </summary>
//        /// <param name="ICSMSClient">A CSMS client.</param>
//        /// <param name="ChargingStationId">The charging station identification.</param>
//        /// <param name="CustomerInformationRequestId">An unique identification of the customer information request.</param>
//        /// <param name="Report">Whether the charging station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.</param>
//        /// <param name="Clear">Whether the charging station should clear all information about the customer referred to.</param>
//        /// <param name="CustomerIdentifier">An optional e.g. vendor specific identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.</param>
//        /// <param name="IdToken">An optional IdToken of the customer this request refers to.</param>
//        /// <param name="CustomerCertificate">An optional certificate of the customer this request refers to.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public static Task<CustomerInformationResponse> RequestCustomerInformation(this ICSMSClient         ICSMSClient,
//                                                                                   ChargeBox_Id             ChargingStationId,
//                                                                                   Int64                    CustomerInformationRequestId,
//                                                                                   Boolean                  Report,
//                                                                                   Boolean                  Clear,
//                                                                                   CustomerIdentifier?      CustomerIdentifier    = null,
//                                                                                   IdToken?                 IdToken               = null,
//                                                                                   CertificateHashData?     CustomerCertificate   = null,

//                                                                                   IEnumerable<Signature>?  Signatures            = null,
//                                                                                   CustomData?              CustomData            = null,

//                                                                                   Request_Id?              RequestId             = null,
//                                                                                   DateTime?                RequestTimestamp      = null,
//                                                                                   TimeSpan?                RequestTimeout        = null,
//                                                                                   EventTracking_Id?        EventTrackingId       = null,
//                                                                                   CancellationToken        CancellationToken     = default)

//            => ICSMSClient.RequestCustomerInformation(
//                   new CustomerInformationRequest(
//                       ChargingStationId,
//                       CustomerInformationRequestId,
//                       Report,
//                       Clear,
//                       CustomerIdentifier,
//                       IdToken,
//                       CustomerCertificate,

//                       Signatures,
//                       CustomData,

//                       RequestId,
//                       RequestTimestamp,
//                       RequestTimeout,
//                       EventTrackingId ?? EventTracking_Id.New,
//                       CancellationToken
//                   )
//               );

//        #endregion


//    }

//}
