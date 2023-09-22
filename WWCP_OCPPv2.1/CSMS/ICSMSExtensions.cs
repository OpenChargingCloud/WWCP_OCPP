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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// Extention methods for all CSMS
    /// </summary>
    public static class ICSMSExtensions
    {

        #region Reset                      (this CSMS, ChargeBoxId, ResetType, EVSEId = null, ...)

        /// <summary>
        /// Reset the given charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            Reset(this ICSMS               CSMS,
                  ChargeBox_Id             ChargeBoxId,
                  ResetTypes               ResetType,
                  EVSE_Id?                 EVSEId              = null,

                  IEnumerable<Signature>?  Signatures          = null,
                  CustomData?              CustomData          = null,

                  Request_Id?              RequestId           = null,
                  DateTime?                RequestTimestamp    = null,
                  TimeSpan?                RequestTimeout      = null,
                  EventTracking_Id?        EventTrackingId     = null,
                  CancellationToken        CancellationToken   = default)


                => CSMS.Reset(new ResetRequest(
                                  ChargeBoxId,
                                  ResetType,
                                  EVSEId,

                                  Signatures,
                                  CustomData,

                                  RequestId        ?? CSMS.NextRequestId,
                                  RequestTimestamp ?? Timestamp.Now,
                                  RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                  EventTrackingId  ?? EventTracking_Id.New,
                                  CancellationToken
                              ));

        #endregion

        #region UpdateFirmware             (this CSMS, ChargeBoxId, Firmware, UpdateFirmwareRequestId, ...)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            UpdateFirmware(this ICSMS               CSMS,
                           ChargeBox_Id             ChargeBoxId,
                           Firmware                 Firmware,
                           Int32                    UpdateFirmwareRequestId,
                           Byte?                    Retries             = null,
                           TimeSpan?                RetryInterval       = null,

                           IEnumerable<Signature>?  Signatures          = null,
                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.UpdateFirmware(new UpdateFirmwareRequest(
                                           ChargeBoxId,
                                           Firmware,
                                           UpdateFirmwareRequestId,
                                           Retries,
                                           RetryInterval,

                                           Signatures,
                                           CustomData,

                                           RequestId        ?? CSMS.NextRequestId,
                                           RequestTimestamp ?? Timestamp.Now,
                                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                           EventTrackingId  ?? EventTracking_Id.New,
                                           CancellationToken
                                       ));

        #endregion

        #region PublishFirmware            (this CSMS, ChargeBoxId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            PublishFirmware(this ICSMS               CSMS,
                            ChargeBox_Id             ChargeBoxId,
                            Int32                    PublishFirmwareRequestId,
                            URL                      DownloadLocation,
                            String                   MD5Checksum,
                            Byte?                    Retries             = null,
                            TimeSpan?                RetryInterval       = null,

                            IEnumerable<Signature>?  Signatures          = null,
                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.PublishFirmware(new PublishFirmwareRequest(
                                            ChargeBoxId,
                                            PublishFirmwareRequestId,
                                            DownloadLocation,
                                            MD5Checksum,
                                            Retries,
                                            RetryInterval,

                                            Signatures,
                                            CustomData,

                                            RequestId        ?? CSMS.NextRequestId,
                                            RequestTimestamp ?? Timestamp.Now,
                                            RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                            EventTrackingId  ?? EventTracking_Id.New,
                                            CancellationToken
                                        ));

        #endregion

        #region UnpublishFirmware          (this CSMS, ChargeBoxId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            UnpublishFirmware(this ICSMS               CSMS,
                              ChargeBox_Id             ChargeBoxId,
                              String                   MD5Checksum,

                              IEnumerable<Signature>?  Signatures          = null,
                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.UnpublishFirmware(new UnpublishFirmwareRequest(
                                              ChargeBoxId,
                                              MD5Checksum,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region GetBaseReport              (this CSMS, ChargeBoxId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetBaseReport(this ICSMS               CSMS,
                          ChargeBox_Id             ChargeBoxId,
                          Int64                    GetBaseReportRequestId,
                          ReportBases              ReportBase,

                          IEnumerable<Signature>?  Signatures          = null,
                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => CSMS.GetBaseReport(new GetBaseReportRequest(
                                          ChargeBoxId,
                                          GetBaseReportRequestId,
                                          ReportBase,

                                          Signatures,
                                          CustomData,

                                          RequestId        ?? CSMS.NextRequestId,
                                          RequestTimestamp ?? Timestamp.Now,
                                          RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                          EventTrackingId  ?? EventTracking_Id.New,
                                          CancellationToken
                                      ));

        #endregion

        #region GetReport                  (this CSMS, ChargeBoxId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetReportRequestId">The charge box identification.</param>
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

            GetReport(this ICSMS                      CSMS,
                      ChargeBox_Id                    ChargeBoxId,
                      Int32                           GetReportRequestId,
                      IEnumerable<ComponentCriteria>  ComponentCriteria,
                      IEnumerable<ComponentVariable>  ComponentVariables,

                      IEnumerable<Signature>?         Signatures          = null,
                      CustomData?                     CustomData          = null,

                      Request_Id?                     RequestId           = null,
                      DateTime?                       RequestTimestamp    = null,
                      TimeSpan?                       RequestTimeout      = null,
                      EventTracking_Id?               EventTrackingId     = null,
                      CancellationToken               CancellationToken   = default)


                => CSMS.GetReport(new GetReportRequest(
                                      ChargeBoxId,
                                      GetReportRequestId,
                                      ComponentCriteria,
                                      ComponentVariables,

                                      Signatures,
                                      CustomData,

                                      RequestId        ?? CSMS.NextRequestId,
                                      RequestTimestamp ?? Timestamp.Now,
                                      RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                      EventTrackingId  ?? EventTracking_Id.New,
                                      CancellationToken
                                  ));

        #endregion

        #region GetLog                     (this CSMS, ChargeBoxId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetLog(this ICSMS               CSMS,
                   ChargeBox_Id             ChargeBoxId,
                   LogTypes                 LogType,
                   Int32                    LogRequestId,
                   LogParameters            Log,
                   Byte?                    Retries             = null,
                   TimeSpan?                RetryInterval       = null,

                   IEnumerable<Signature>?  Signatures          = null,
                   CustomData?              CustomData          = null,

                   Request_Id?              RequestId           = null,
                   DateTime?                RequestTimestamp    = null,
                   TimeSpan?                RequestTimeout      = null,
                   EventTracking_Id?        EventTrackingId     = null,
                   CancellationToken        CancellationToken   = default)


                => CSMS.GetLog(new GetLogRequest(
                                   ChargeBoxId,
                                   LogType,
                                   LogRequestId,
                                   Log,
                                   Retries,
                                   RetryInterval,

                                   Signatures,
                                   CustomData,

                                   RequestId        ?? CSMS.NextRequestId,
                                   RequestTimestamp ?? Timestamp.Now,
                                   RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                   EventTrackingId  ?? EventTracking_Id.New,
                                   CancellationToken
                               ));

        #endregion


        #region SetVariables               (this CSMS, ChargeBoxId, VariableData, ...)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VariableData">An enumeration of variable data to set/change.</param>
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

            SetVariables(this ICSMS                    CSMS,
                         ChargeBox_Id                  ChargeBoxId,
                         IEnumerable<SetVariableData>  VariableData,

                         IEnumerable<Signature>?       Signatures          = null,
                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => CSMS.SetVariables(new SetVariablesRequest(
                                         ChargeBoxId,
                                         VariableData,

                                         Signatures,
                                         CustomData,

                                         RequestId        ?? CSMS.NextRequestId,
                                         RequestTimestamp ?? Timestamp.Now,
                                         RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         CancellationToken
                                     ));

        #endregion

        #region GetVariables               (this CSMS, ChargeBoxId, VariableData, ...)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetVariables(this ICSMS                    CSMS,
                         ChargeBox_Id                  ChargeBoxId,
                         IEnumerable<GetVariableData>  VariableData,

                         IEnumerable<Signature>?       Signatures          = null,
                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => CSMS.GetVariables(new GetVariablesRequest(
                                         ChargeBoxId,
                                         VariableData,

                                         Signatures,
                                         CustomData,

                                         RequestId        ?? CSMS.NextRequestId,
                                         RequestTimestamp ?? Timestamp.Now,
                                         RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         CancellationToken
                                     ));

        #endregion

        #region SetMonitoringBase          (this CSMS, ChargeBoxId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SetMonitoringBase(this ICSMS               CSMS,
                              ChargeBox_Id             ChargeBoxId,
                              MonitoringBases          MonitoringBase,

                              IEnumerable<Signature>?  Signatures          = null,
                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.SetMonitoringBase(new SetMonitoringBaseRequest(
                                              ChargeBoxId,
                                              MonitoringBase,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region GetMonitoringReport        (this CSMS, ChargeBoxId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetMonitoringReportRequestId">The charge box identification.</param>
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

            GetMonitoringReport(this ICSMS                       CSMS,
                                ChargeBox_Id                     ChargeBoxId,
                                Int32                            GetMonitoringReportRequestId,
                                IEnumerable<MonitoringCriteria>  MonitoringCriteria,
                                IEnumerable<ComponentVariable>   ComponentVariables,

                                IEnumerable<Signature>?          Signatures          = null,
                                CustomData?                      CustomData          = null,

                                Request_Id?                      RequestId           = null,
                                DateTime?                        RequestTimestamp    = null,
                                TimeSpan?                        RequestTimeout      = null,
                                EventTracking_Id?                EventTrackingId     = null,
                                CancellationToken                CancellationToken   = default)


                => CSMS.GetMonitoringReport(new GetMonitoringReportRequest(
                                                ChargeBoxId,
                                                GetMonitoringReportRequestId,
                                                MonitoringCriteria,
                                                ComponentVariables,

                                                Signatures,
                                                CustomData,

                                                RequestId        ?? CSMS.NextRequestId,
                                                RequestTimestamp ?? Timestamp.Now,
                                                RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                EventTrackingId  ?? EventTracking_Id.New,
                                                CancellationToken
                                            ));

        #endregion

        #region SetMonitoringLevel         (this CSMS, ChargeBoxId, Severity, ...)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SetMonitoringLevel(this ICSMS               CSMS,
                               ChargeBox_Id             ChargeBoxId,
                               Severities               Severity,

                               IEnumerable<Signature>?  Signatures          = null,
                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.SetMonitoringLevel(new SetMonitoringLevelRequest(
                                               ChargeBoxId,
                                               Severity,

                                               Signatures,
                                               CustomData,

                                               RequestId        ?? CSMS.NextRequestId,
                                               RequestTimestamp ?? Timestamp.Now,
                                               RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                               EventTrackingId  ?? EventTracking_Id.New,
                                               CancellationToken
                                           ));

        #endregion

        #region SetVariableMonitoring      (this CSMS, ChargeBoxId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SetVariableMonitoring(this ICSMS                      CSMS,
                                  ChargeBox_Id                    ChargeBoxId,
                                  IEnumerable<SetMonitoringData>  MonitoringData,

                                  IEnumerable<Signature>?         Signatures          = null,
                                  CustomData?                     CustomData          = null,

                                  Request_Id?                     RequestId           = null,
                                  DateTime?                       RequestTimestamp    = null,
                                  TimeSpan?                       RequestTimeout      = null,
                                  EventTracking_Id?               EventTrackingId     = null,
                                  CancellationToken               CancellationToken   = default)


                => CSMS.SetVariableMonitoring(new SetVariableMonitoringRequest(
                                                  ChargeBoxId,
                                                  MonitoringData,

                                                  Signatures,
                                                  CustomData,

                                                  RequestId        ?? CSMS.NextRequestId,
                                                  RequestTimestamp ?? Timestamp.Now,
                                                  RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                  EventTrackingId  ?? EventTracking_Id.New,
                                                  CancellationToken
                                              ));

        #endregion

        #region ClearVariableMonitoring    (this CSMS, ChargeBoxId, VariableMonitoringIds, ...)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            ClearVariableMonitoring(this ICSMS                          CSMS,
                                    ChargeBox_Id                        ChargeBoxId,
                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

                                    IEnumerable<Signature>?             Signatures          = null,
                                    CustomData?                         CustomData          = null,

                                    Request_Id?                         RequestId           = null,
                                    DateTime?                           RequestTimestamp    = null,
                                    TimeSpan?                           RequestTimeout      = null,
                                    EventTracking_Id?                   EventTrackingId     = null,
                                    CancellationToken                   CancellationToken   = default)


                => CSMS.ClearVariableMonitoring(new ClearVariableMonitoringRequest(
                                                    ChargeBoxId,
                                                    VariableMonitoringIds,

                                                    Signatures,
                                                    CustomData,

                                                    RequestId        ?? CSMS.NextRequestId,
                                                    RequestTimestamp ?? Timestamp.Now,
                                                    RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                    EventTrackingId  ?? EventTracking_Id.New,
                                                    CancellationToken
                                                ));

        #endregion

        #region SetNetworkProfile          (this CSMS, ChargeBoxId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SetNetworkProfile(this ICSMS                CSMS,
                              ChargeBox_Id              ChargeBoxId,
                              Int32                     ConfigurationSlot,
                              NetworkConnectionProfile  NetworkConnectionProfile,

                              IEnumerable<Signature>?   Signatures          = null,
                              CustomData?               CustomData          = null,

                              Request_Id?               RequestId           = null,
                              DateTime?                 RequestTimestamp    = null,
                              TimeSpan?                 RequestTimeout      = null,
                              EventTracking_Id?         EventTrackingId     = null,
                              CancellationToken         CancellationToken   = default)


                => CSMS.SetNetworkProfile(new SetNetworkProfileRequest(
                                              ChargeBoxId,
                                              ConfigurationSlot,
                                              NetworkConnectionProfile,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region ChangeAvailability         (this CSMS, ChargeBoxId, OperationalStatus, EVSE, ...)

        /// <summary>
        /// Change the availability of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OperationalStatus">A new operational status of the charging station or EVSE.</param>
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

            ChangeAvailability(this ICSMS               CSMS,
                               ChargeBox_Id             ChargeBoxId,
                               OperationalStatus        OperationalStatus,
                               EVSE?                    EVSE,

                               IEnumerable<Signature>?  Signatures          = null,
                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.ChangeAvailability(new ChangeAvailabilityRequest(
                                               ChargeBoxId,
                                               OperationalStatus,
                                               EVSE,

                                               Signatures,
                                               CustomData,

                                               RequestId        ?? CSMS.NextRequestId,
                                               RequestTimestamp ?? Timestamp.Now,
                                               RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                               EventTrackingId  ?? EventTracking_Id.New,
                                               CancellationToken
                                           ));

        #endregion

        #region TriggerMessage             (this CSMS, ChargeBoxId, RequestedMessage, EVSEId = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="EVSE">An optional EVSE (and connector) identification whenever the message applies to a specific EVSE and/or connector.</param>
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

            TriggerMessage(this ICSMS               CSMS,
                           ChargeBox_Id             ChargeBoxId,
                           MessageTriggers          RequestedMessage,
                           EVSE?                    EVSE                = null,

                           IEnumerable<Signature>?  Signatures          = null,
                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.TriggerMessage(new TriggerMessageRequest(
                                           ChargeBoxId,
                                           RequestedMessage,
                                           EVSE,

                                           Signatures,
                                           CustomData,

                                           RequestId        ?? CSMS.NextRequestId,
                                           RequestTimestamp ?? Timestamp.Now,
                                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                           EventTrackingId  ?? EventTracking_Id.New,
                                           CancellationToken
                                       ));

        #endregion

        #region TransferData               (this CSMS, ChargeBoxId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CS.DataTransferResponse>

            TransferData(this ICSMS               CSMS,
                         ChargeBox_Id             ChargeBoxId,
                         Vendor_Id                VendorId,
                         String?                  MessageId           = null,
                         JToken?                  Data                = null,

                         IEnumerable<Signature>?  Signatures          = null,
                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => CSMS.TransferData(new DataTransferRequest(
                                         ChargeBoxId,
                                         VendorId,
                                         MessageId,
                                         Data,

                                         Signatures,
                                         CustomData,

                                         RequestId        ?? CSMS.NextRequestId,
                                         RequestTimestamp ?? Timestamp.Now,
                                         RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         CancellationToken
                                     ));

        #endregion


        #region SendSignedCertificate      (this CSMS, ChargeBoxId, CertificateChain, CertificateType = null, ...)

        /// <summary>
        /// Send the signed certificate to the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SendSignedCertificate(this ICSMS               CSMS,
                                  ChargeBox_Id             ChargeBoxId,
                                  CertificateChain         CertificateChain,
                                  CertificateSigningUse?   CertificateType     = null,

                                  IEnumerable<Signature>?  Signatures          = null,
                                  CustomData?              CustomData          = null,

                                  Request_Id?              RequestId           = null,
                                  DateTime?                RequestTimestamp    = null,
                                  TimeSpan?                RequestTimeout      = null,
                                  EventTracking_Id?        EventTrackingId     = null,
                                  CancellationToken        CancellationToken   = default)


                => CSMS.SendSignedCertificate(new CertificateSignedRequest(
                                                  ChargeBoxId,
                                                  CertificateChain,
                                                  CertificateType,

                                                  Signatures,
                                                  CustomData,

                                                  RequestId        ?? CSMS.NextRequestId,
                                                  RequestTimestamp ?? Timestamp.Now,
                                                  RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                  EventTrackingId  ?? EventTracking_Id.New,
                                                  CancellationToken
                                              ));

        #endregion

        #region InstallCertificate         (this CSMS, ChargeBoxId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            InstallCertificate(this ICSMS               CSMS,
                               ChargeBox_Id             ChargeBoxId,
                               CertificateUse           CertificateType,
                               Certificate              Certificate,

                               IEnumerable<Signature>?  Signatures          = null,
                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.InstallCertificate(new InstallCertificateRequest(
                                               ChargeBoxId,
                                               CertificateType,
                                               Certificate,

                                               Signatures,
                                               CustomData,

                                               RequestId        ?? CSMS.NextRequestId,
                                               RequestTimestamp ?? Timestamp.Now,
                                               RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                               EventTrackingId  ?? EventTracking_Id.New,
                                               CancellationToken
                                           ));

        #endregion

        #region GetInstalledCertificateIds (this CSMS, ChargeBoxId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetInstalledCertificateIds(this ICSMS                    CSMS,
                                       ChargeBox_Id                  ChargeBoxId,
                                       IEnumerable<CertificateUse>?  CertificateTypes    = null,

                                       IEnumerable<Signature>?       Signatures          = null,
                                       CustomData?                   CustomData          = null,

                                       Request_Id?                   RequestId           = null,
                                       DateTime?                     RequestTimestamp    = null,
                                       TimeSpan?                     RequestTimeout      = null,
                                       EventTracking_Id?             EventTrackingId     = null,
                                       CancellationToken             CancellationToken   = default)


                => CSMS.GetInstalledCertificateIds(new GetInstalledCertificateIdsRequest(
                                                       ChargeBoxId,
                                                       CertificateTypes,

                                                       Signatures,
                                                       CustomData,

                                                       RequestId        ?? CSMS.NextRequestId,
                                                       RequestTimestamp ?? Timestamp.Now,
                                                       RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                       EventTrackingId  ?? EventTracking_Id.New,
                                                       CancellationToken
                                                   ));

        #endregion

        #region DeleteCertificate          (this CSMS, ChargeBoxId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            DeleteCertificate(this ICSMS               CSMS,
                              ChargeBox_Id             ChargeBoxId,
                              CertificateHashData      CertificateHashData,

                              IEnumerable<Signature>?  Signatures          = null,
                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.DeleteCertificate(new DeleteCertificateRequest(
                                              ChargeBoxId,
                                              CertificateHashData,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region NotifyCRLAvailability      (this CSMS, ChargeBoxId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            NotifyCRLAvailability(this ICSMS               CSMS,
                                  ChargeBox_Id             ChargeBoxId,
                                  Int32                    NotifyCRLRequestId,
                                  NotifyCRLStatus          Availability,
                                  URL?                     Location,

                                  IEnumerable<Signature>?  Signatures          = null,
                                  CustomData?              CustomData          = null,

                                  Request_Id?              RequestId           = null,
                                  DateTime?                RequestTimestamp    = null,
                                  TimeSpan?                RequestTimeout      = null,
                                  EventTracking_Id?        EventTrackingId     = null,
                                  CancellationToken        CancellationToken   = default)


                => CSMS.NotifyCRLAvailability(new NotifyCRLRequest(
                                                  ChargeBoxId,
                                                  NotifyCRLRequestId,
                                                  Availability,
                                                  Location,

                                                  Signatures,
                                                  CustomData,

                                                  RequestId        ?? CSMS.NextRequestId,
                                                  RequestTimestamp ?? Timestamp.Now,
                                                  RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                  EventTrackingId  ?? EventTracking_Id.New,
                                                  CancellationToken
                                              ));

        #endregion


        #region GetLocalListVersion        (this CSMS, ChargeBoxId, ...)

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetLocalListVersion(this ICSMS               CSMS,
                                ChargeBox_Id             ChargeBoxId,

                                IEnumerable<Signature>?  Signatures          = null,
                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.GetLocalListVersion(new GetLocalListVersionRequest(
                                                ChargeBoxId,

                                                Signatures,
                                                CustomData,

                                                RequestId        ?? CSMS.NextRequestId,
                                                RequestTimestamp ?? Timestamp.Now,
                                                RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                EventTrackingId  ?? EventTracking_Id.New,
                                                CancellationToken
                                            ));

        #endregion

        #region SendLocalList              (this CSMS, ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SendLocalList(this ICSMS                       CSMS,
                          ChargeBox_Id                     ChargeBoxId,
                          UInt64                           ListVersion,
                          UpdateTypes                      UpdateType,
                          IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                          IEnumerable<Signature>?          Signatures               = null,
                          CustomData?                      CustomData               = null,

                          Request_Id?                      RequestId                = null,
                          DateTime?                        RequestTimestamp         = null,
                          TimeSpan?                        RequestTimeout           = null,
                          EventTracking_Id?                EventTrackingId          = null,
                          CancellationToken                CancellationToken        = default)


                => CSMS.SendLocalList(new SendLocalListRequest(
                                          ChargeBoxId,
                                          ListVersion,
                                          UpdateType,
                                          LocalAuthorizationList,

                                          Signatures,
                                          CustomData,

                                          RequestId        ?? CSMS.NextRequestId,
                                          RequestTimestamp ?? Timestamp.Now,
                                          RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                          EventTrackingId  ?? EventTracking_Id.New,
                                          CancellationToken
                                      ));

        #endregion

        #region ClearCache                 (this CSMS, ChargeBoxId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            ClearCache(this ICSMS               CSMS,
                       ChargeBox_Id             ChargeBoxId,

                       IEnumerable<Signature>?  Signatures          = null,
                       CustomData?              CustomData          = null,

                       Request_Id?              RequestId           = null,
                       DateTime?                RequestTimestamp    = null,
                       TimeSpan?                RequestTimeout      = null,
                       EventTracking_Id?        EventTrackingId     = null,
                       CancellationToken        CancellationToken   = default)


                => CSMS.ClearCache(new ClearCacheRequest(
                                       ChargeBoxId,

                                       Signatures,
                                       CustomData,

                                       RequestId        ?? CSMS.NextRequestId,
                                       RequestTimestamp ?? Timestamp.Now,
                                       RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                       EventTrackingId  ?? EventTracking_Id.New,
                                       CancellationToken
                                   ));

        #endregion


        #region ReserveNow                 (this CSMS, ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            ReserveNow(this ICSMS               CSMS,
                       ChargeBox_Id             ChargeBoxId,
                       Reservation_Id           ReservationId,
                       DateTime                 ExpiryDate,
                       IdToken                  IdToken,
                       ConnectorTypes?          ConnectorType       = null,
                       EVSE_Id?                 EVSEId              = null,
                       IdToken?                 GroupIdToken        = null,

                       IEnumerable<Signature>?  Signatures          = null,
                       CustomData?              CustomData          = null,

                       Request_Id?              RequestId           = null,
                       DateTime?                RequestTimestamp    = null,
                       TimeSpan?                RequestTimeout      = null,
                       EventTracking_Id?        EventTrackingId     = null,
                       CancellationToken        CancellationToken   = default)


                => CSMS.ReserveNow(new ReserveNowRequest(
                                       ChargeBoxId,
                                       ReservationId,
                                       ExpiryDate,
                                       IdToken,
                                       ConnectorType,
                                       EVSEId,
                                       GroupIdToken,

                                       Signatures,
                                       CustomData,

                                       RequestId        ?? CSMS.NextRequestId,
                                       RequestTimestamp ?? Timestamp.Now,
                                       RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                       EventTrackingId  ?? EventTracking_Id.New,
                                       CancellationToken
                                   ));

        #endregion

        #region CancelReservation          (this CSMS, ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            CancelReservation(this ICSMS               CSMS,
                              ChargeBox_Id             ChargeBoxId,
                              Reservation_Id           ReservationId,

                              IEnumerable<Signature>?  Signatures          = null,
                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.CancelReservation(new CancelReservationRequest(
                                              ChargeBoxId,
                                              ReservationId,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region StartCharging              (this CSMS, ChargeBoxId, RequestStartTransactionRequestId, IdToken, EVSEId, ChargingProfile, GroupIdToken, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestStartTransactionRequestId">Request identification given by the server to this start request. The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.</param>
        /// <param name="IdToken">The identification token to start the charging transaction.</param>
        /// <param name="EVSEId">An optional EVSE identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charging station for the requested charging transaction.</param>
        /// <param name="GroupIdToken">An optional group identifier.</param>
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

            StartCharging(this ICSMS               CSMS,
                          ChargeBox_Id             ChargeBoxId,
                          RemoteStart_Id           RequestStartTransactionRequestId,
                          IdToken                  IdToken,
                          EVSE_Id?                 EVSEId              = null,
                          ChargingProfile?         ChargingProfile     = null,
                          IdToken?                 GroupIdToken        = null,

                          IEnumerable<Signature>?  Signatures          = null,
                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => CSMS.StartCharging(new RequestStartTransactionRequest(
                                          ChargeBoxId,
                                          RequestStartTransactionRequestId,
                                          IdToken,
                                          EVSEId,
                                          ChargingProfile,
                                          GroupIdToken,

                                          Signatures,
                                          CustomData,

                                          RequestId        ?? CSMS.NextRequestId,
                                          RequestTimestamp ?? Timestamp.Now,
                                          RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                          EventTrackingId  ?? EventTracking_Id.New,
                                          CancellationToken
                                      ));

        #endregion

        #region StopCharging               (this CSMS, ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            StopCharging(this ICSMS               CSMS,
                         ChargeBox_Id             ChargeBoxId,
                         Transaction_Id           TransactionId,

                         IEnumerable<Signature>?  Signatures          = null,
                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => CSMS.StopCharging(new RequestStopTransactionRequest(
                                         ChargeBoxId,
                                         TransactionId,

                                         Signatures,
                                         CustomData,

                                         RequestId        ?? CSMS.NextRequestId,
                                         RequestTimestamp ?? Timestamp.Now,
                                         RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         CancellationToken
                                     ));

        #endregion

        #region GetTransactionStatus       (this CSMS, ChargeBoxId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetTransactionStatus(this ICSMS               CSMS,
                                 ChargeBox_Id             ChargeBoxId,
                                 Transaction_Id?          TransactionId       = null,

                                 IEnumerable<Signature>?  Signatures          = null,
                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 CancellationToken        CancellationToken   = default)


                => CSMS.GetTransactionStatus(new GetTransactionStatusRequest(
                                                 ChargeBoxId,
                                                 TransactionId,

                                                 Signatures,
                                                 CustomData,

                                                 RequestId        ?? CSMS.NextRequestId,
                                                 RequestTimestamp ?? Timestamp.Now,
                                                 RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                 EventTrackingId  ?? EventTracking_Id.New,
                                                 CancellationToken
                                             ));

        #endregion

        #region SetChargingProfile         (this CSMS, ChargeBoxId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SetChargingProfile(this ICSMS               CSMS,
                               ChargeBox_Id             ChargeBoxId,
                               EVSE_Id                  EVSEId,
                               ChargingProfile          ChargingProfile,

                               IEnumerable<Signature>?  Signatures          = null,
                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.SetChargingProfile(new SetChargingProfileRequest(
                                               ChargeBoxId,
                                               EVSEId,
                                               ChargingProfile,

                                               Signatures,
                                               CustomData,

                                               RequestId        ?? CSMS.NextRequestId,
                                               RequestTimestamp ?? Timestamp.Now,
                                               RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                               EventTrackingId  ?? EventTracking_Id.New,
                                               CancellationToken
                                           ));

        #endregion

        #region GetChargingProfiles        (this CSMS, ChargeBoxId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetChargingProfiles(this ICSMS                CSMS,
                                ChargeBox_Id              ChargeBoxId,
                                Int64                     GetChargingProfilesRequestId,
                                ChargingProfileCriterion  ChargingProfile,
                                EVSE_Id?                  EVSEId              = null,

                                IEnumerable<Signature>?   Signatures          = null,
                                CustomData?               CustomData          = null,

                                Request_Id?               RequestId           = null,
                                DateTime?                 RequestTimestamp    = null,
                                TimeSpan?                 RequestTimeout      = null,
                                EventTracking_Id?         EventTrackingId     = null,
                                CancellationToken         CancellationToken   = default)


                => CSMS.GetChargingProfiles(new GetChargingProfilesRequest(
                                                ChargeBoxId,
                                                GetChargingProfilesRequestId,
                                                ChargingProfile,
                                                EVSEId,

                                                Signatures,
                                                CustomData,

                                                RequestId        ?? CSMS.NextRequestId,
                                                RequestTimestamp ?? Timestamp.Now,
                                                RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                EventTrackingId  ?? EventTracking_Id.New,
                                                CancellationToken
                                            ));

        #endregion

        #region ClearChargingProfile       (this CSMS, ChargeBoxId, ChargingProfileId, ChargingProfileCriteria, ...)

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            ClearChargingProfile(this ICSMS               CSMS,
                                 ChargeBox_Id             ChargeBoxId,
                                 ChargingProfile_Id?      ChargingProfileId         = null,
                                 ClearChargingProfile?    ChargingProfileCriteria   = null,

                                 IEnumerable<Signature>?  Signatures                = null,
                                 CustomData?              CustomData                = null,

                                 Request_Id?              RequestId                 = null,
                                 DateTime?                RequestTimestamp          = null,
                                 TimeSpan?                RequestTimeout            = null,
                                 EventTracking_Id?        EventTrackingId           = null,
                                 CancellationToken        CancellationToken         = default)


                => CSMS.ClearChargingProfile(new ClearChargingProfileRequest(
                                                 ChargeBoxId,
                                                 ChargingProfileId,
                                                 ChargingProfileCriteria,

                                                 Signatures,
                                                 CustomData,

                                                 RequestId        ?? CSMS.NextRequestId,
                                                 RequestTimestamp ?? Timestamp.Now,
                                                 RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                 EventTrackingId  ?? EventTracking_Id.New,
                                                 CancellationToken
                                             ));

        #endregion

        #region GetCompositeSchedule       (this CSMS, ChargeBoxId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            GetCompositeSchedule(this ICSMS               CSMS,
                                 ChargeBox_Id             ChargeBoxId,
                                 TimeSpan                 Duration,
                                 EVSE_Id                  EVSEId,
                                 ChargingRateUnits?       ChargingRateUnit    = null,

                                 IEnumerable<Signature>?  Signatures          = null,
                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 CancellationToken        CancellationToken   = default)


                => CSMS.GetCompositeSchedule(new GetCompositeScheduleRequest(
                                                 ChargeBoxId,
                                                 Duration,
                                                 EVSEId,
                                                 ChargingRateUnit,

                                                 Signatures,
                                                 CustomData,

                                                 RequestId        ?? CSMS.NextRequestId,
                                                 RequestTimestamp ?? Timestamp.Now,
                                                 RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                 EventTrackingId  ?? EventTracking_Id.New,
                                                 CancellationToken
                                             ));

        #endregion

        #region UpdateDynamicSchedule      (this CSMS, ChargeBoxId, ChargingProfileId, Limit = null, ...)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            UpdateDynamicSchedule(this ICSMS               CSMS,
                                  ChargeBox_Id             ChargeBoxId,
                                  ChargingProfile_Id       ChargingProfileId,

                                  ChargingRateValue?       Limit                 = null,
                                  ChargingRateValue?       Limit_L2              = null,
                                  ChargingRateValue?       Limit_L3              = null,

                                  ChargingRateValue?       DischargeLimit        = null,
                                  ChargingRateValue?       DischargeLimit_L2     = null,
                                  ChargingRateValue?       DischargeLimit_L3     = null,

                                  ChargingRateValue?       Setpoint              = null,
                                  ChargingRateValue?       Setpoint_L2           = null,
                                  ChargingRateValue?       Setpoint_L3           = null,

                                  ChargingRateValue?       SetpointReactive      = null,
                                  ChargingRateValue?       SetpointReactive_L2   = null,
                                  ChargingRateValue?       SetpointReactive_L3   = null,

                                  IEnumerable<Signature>?  Signatures            = null,
                                  CustomData?              CustomData            = null,

                                  Request_Id?              RequestId             = null,
                                  DateTime?                RequestTimestamp      = null,
                                  TimeSpan?                RequestTimeout        = null,
                                  EventTracking_Id?        EventTrackingId       = null,
                                  CancellationToken        CancellationToken     = default)


                => CSMS.UpdateDynamicSchedule(new UpdateDynamicScheduleRequest(

                                                  ChargeBoxId,
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

                                                  Signatures,
                                                  CustomData,

                                                  RequestId        ?? CSMS.NextRequestId,
                                                  RequestTimestamp ?? Timestamp.Now,
                                                  RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                  EventTrackingId  ?? EventTracking_Id.New,
                                                  CancellationToken

                                              ));

        #endregion

        #region NotifyAllowedEnergyTransfer(this CSMS, ChargeBoxId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            NotifyAllowedEnergyTransfer(this ICSMS                        CSMS,
                                        ChargeBox_Id                      ChargeBoxId,
                                        IEnumerable<EnergyTransferModes>  AllowedEnergyTransferModes,

                                        IEnumerable<Signature>?           Signatures          = null,
                                        CustomData?                       CustomData          = null,

                                        Request_Id?                       RequestId           = null,
                                        DateTime?                         RequestTimestamp    = null,
                                        TimeSpan?                         RequestTimeout      = null,
                                        EventTracking_Id?                 EventTrackingId     = null,
                                        CancellationToken                 CancellationToken   = default)


                => CSMS.NotifyAllowedEnergyTransfer(new NotifyAllowedEnergyTransferRequest(
                                                        ChargeBoxId,
                                                        AllowedEnergyTransferModes,

                                                        Signatures,
                                                        CustomData,

                                                        RequestId        ?? CSMS.NextRequestId,
                                                        RequestTimestamp ?? Timestamp.Now,
                                                        RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                        EventTrackingId  ?? EventTracking_Id.New,
                                                        CancellationToken
                                                    ));

        #endregion

        #region UsePriorityCharging        (this CSMS, ChargeBoxId, TransactionId, Activate, ...)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            UsePriorityCharging(this ICSMS               CSMS,
                                ChargeBox_Id             ChargeBoxId,
                                Transaction_Id           TransactionId,
                                Boolean                  Activate,

                                IEnumerable<Signature>?  Signatures          = null,
                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.UsePriorityCharging(new UsePriorityChargingRequest(
                                                ChargeBoxId,
                                                TransactionId,
                                                Activate,

                                                Signatures,
                                                CustomData,

                                                RequestId        ?? CSMS.NextRequestId,
                                                RequestTimestamp ?? Timestamp.Now,
                                                RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                EventTrackingId  ?? EventTracking_Id.New,
                                                CancellationToken
                                            ));

        #endregion

        #region UnlockConnector            (this CSMS, ChargeBoxId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            UnlockConnector(this ICSMS               CSMS,
                            ChargeBox_Id             ChargeBoxId,
                            EVSE_Id                  EVSEId,
                            Connector_Id             ConnectorId,

                            IEnumerable<Signature>?  Signatures          = null,
                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.UnlockConnector(new UnlockConnectorRequest(
                                            ChargeBoxId,
                                            EVSEId,
                                            ConnectorId,

                                            Signatures,
                                            CustomData,

                                            RequestId        ?? CSMS.NextRequestId,
                                            RequestTimestamp ?? Timestamp.Now,
                                            RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                            EventTrackingId  ?? EventTracking_Id.New,
                                            CancellationToken
                                        ));

        #endregion


        #region SendAFRRSignal             (this CSMS, ChargeBoxId, ActivationTimestamp, Signal, ...)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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

            SendAFRRSignal(this ICSMS               CSMS,
                           ChargeBox_Id             ChargeBoxId,
                           DateTime                 ActivationTimestamp,
                           AFRR_Signal              Signal,

                           IEnumerable<Signature>?  Signatures          = null,
                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.SendAFRRSignal(new AFRRSignalRequest(
                                           ChargeBoxId,
                                           ActivationTimestamp,
                                           Signal,

                                           Signatures,
                                           CustomData,

                                           RequestId        ?? CSMS.NextRequestId,
                                           RequestTimestamp ?? Timestamp.Now,
                                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                           EventTrackingId  ?? EventTracking_Id.New,
                                           CancellationToken
                                       ));

        #endregion


        #region SetDisplayMessage          (this CSMS, ChargeBoxId, Message, ...)

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

            SetDisplayMessage(this ICSMS               CSMS,
                              ChargeBox_Id             ChargeBoxId,
                              MessageInfo              Message,

                              IEnumerable<Signature>?  Signatures          = null,
                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.SetDisplayMessage(new SetDisplayMessageRequest(
                                              ChargeBoxId,
                                              Message,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? CSMS.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          ));

        #endregion

        #region GetDisplayMessages         (this CSMS, ChargeBoxId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null, ...)

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

            GetDisplayMessages(this ICSMS                       CSMS,
                               ChargeBox_Id                     ChargeBoxId,
                               Int32                            GetDisplayMessagesRequestId,
                               IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                               MessagePriorities?               Priority            = null,
                               MessageStates?                   State               = null,

                               IEnumerable<Signature>?          Signatures          = null,
                               CustomData?                      CustomData          = null,

                               Request_Id?                      RequestId           = null,
                               DateTime?                        RequestTimestamp    = null,
                               TimeSpan?                        RequestTimeout      = null,
                               EventTracking_Id?                EventTrackingId     = null,
                               CancellationToken                CancellationToken   = default)


                => CSMS.GetDisplayMessages(new GetDisplayMessagesRequest(
                                               ChargeBoxId,
                                               GetDisplayMessagesRequestId,
                                               Ids,
                                               Priority,
                                               State,

                                               Signatures,
                                               CustomData,

                                               RequestId        ?? CSMS.NextRequestId,
                                               RequestTimestamp ?? Timestamp.Now,
                                               RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                               EventTrackingId  ?? EventTracking_Id.New,
                                               CancellationToken
                                           ));

        #endregion

        #region ClearDisplayMessage        (this CSMS, ChargeBoxId, DisplayMessageId, ...)

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

            ClearDisplayMessage(this ICSMS               CSMS,
                                ChargeBox_Id             ChargeBoxId,
                                DisplayMessage_Id        DisplayMessageId,

                                IEnumerable<Signature>?  Signatures          = null,
                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.ClearDisplayMessage(new ClearDisplayMessageRequest(
                                                ChargeBoxId,
                                                DisplayMessageId,

                                                Signatures,
                                                CustomData,

                                                RequestId        ?? CSMS.NextRequestId,
                                                RequestTimestamp ?? Timestamp.Now,
                                                RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                EventTrackingId  ?? EventTracking_Id.New,
                                                CancellationToken
                                            ));

        #endregion

        #region SendCostUpdated            (this CSMS, ChargeBoxId, TotalCost, TransactionId, ...)

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

            SendCostUpdated(this ICSMS               CSMS,
                            ChargeBox_Id             ChargeBoxId,
                            Decimal                  TotalCost,
                            Transaction_Id           TransactionId,

                            IEnumerable<Signature>?  Signatures          = null,
                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.SendCostUpdated(new CostUpdatedRequest(
                                            ChargeBoxId,
                                            TotalCost,
                                            TransactionId,

                                            Signatures,
                                            CustomData,

                                            RequestId        ?? CSMS.NextRequestId,
                                            RequestTimestamp ?? Timestamp.Now,
                                            RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                            EventTrackingId  ?? EventTracking_Id.New,
                                            CancellationToken
                                        ));

        #endregion

        #region RequestCustomerInformation (this CSMS, ChargeBoxId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

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

            RequestCustomerInformation(this ICSMS               CSMS,
                                       ChargeBox_Id             ChargeBoxId,
                                       Int64                    CustomerInformationRequestId,
                                       Boolean                  Report,
                                       Boolean                  Clear,
                                       CustomerIdentifier?      CustomerIdentifier    = null,
                                       IdToken?                 IdToken               = null,
                                       CertificateHashData?     CustomerCertificate   = null,

                                       IEnumerable<Signature>?  Signatures            = null,
                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTime?                RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       CancellationToken        CancellationToken     = default)


                => CSMS.RequestCustomerInformation(new CustomerInformationRequest(
                                                       ChargeBoxId,
                                                       CustomerInformationRequestId,
                                                       Report,
                                                       Clear,
                                                       CustomerIdentifier,
                                                       IdToken,
                                                       CustomerCertificate,

                                                       Signatures,
                                                       CustomData,

                                                       RequestId        ?? CSMS.NextRequestId,
                                                       RequestTimestamp ?? Timestamp.Now,
                                                       RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                                                       EventTrackingId  ?? EventTracking_Id.New,
                                                       CancellationToken
                                                   ));

        #endregion


    }

}
